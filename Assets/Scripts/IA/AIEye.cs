using UnityEngine;

public class AIEye : MonoBehaviour
{
    // ── Configuración de visión ──────────────────────────────────────────────

    // Distancia máxima de detección del sensor (en unidades de Unity)
    public float distance = 10f;

    // Ángulo del cono de visión (limitado entre 0 y 180 grados en el Inspector)
    [Range(0, 180)]
    public float angle = 30f;

    // Altura del campo de visión (rango vertical que puede detectar)
    public float height = 1.0f;

    // Número de escaneos por segundo
    public int scanFrequency = 30;

    // Máscara de capas que define qué objetos se consideran enemigos/detectables
    public LayerMask enemyLayers;

    // Máscara de capas que define qué objetos bloquean la línea de visión
    public LayerMask occlusionLayers;

    // ── Estado de detección ──────────────────────────────────────────────────

    // Transform del jugador detectado; solo esta clase puede asignarlo (setter privado)
    public Transform ViewPlayer;

    // Transform del punto de movimiento auxiliar; solo esta clase puede asignarlo
    public Transform positionEye;

    // ── Gizmos ───────────────────────────────────────────────────────────────

    // Color de la malla de visualización del cono de visión en el editor
    // Color de la malla cuando no detecta al jugador
    public Color meshColor = Color.red;

    // Color de la malla cuando detecta al jugador
    public Color detectedColor = Color.green;

    // Activa o desactiva el dibujo de gizmos en la escena
    public bool IsDrawGizmo = false;

    // ── Internals ────────────────────────────────────────────────────────────

    // Malla generada para visualizar el cono de visión en el editor
    Mesh _mesh;

    // Intervalo de tiempo entre cada escaneo (calculado a partir de scanFrequency)
    float _scanInterval;

    // Temporizador que cuenta hacia abajo hasta el próximo escaneo
    float _scanTimer;

    // Cantidad de colliders detectados en el último escaneo
    int _count;

    // Array preallocado para almacenar los colliders detectados (evita asignaciones de memoria en runtime)
    readonly Collider[] _colliders = new Collider[50];

    // Límite inferior de altura precalculado (valor negativo de height)
    float _halfHeightNeg;

    // Límite superior de altura precalculado (valor positivo de height)
    float _halfHeightPos;

    // ────────────────────────────────────────────────────────────────────────
    // Método llamado una vez al iniciar el componente
    void Start()
    {
        // Calcula el intervalo de escaneo: 1 segundo dividido entre la frecuencia
        _scanInterval = 1f / scanFrequency;

        // Inicializa el temporizador con el intervalo para que el primer escaneo ocurra inmediatamente
        _scanTimer = _scanInterval;

        // Crea un GameObject vacío como punto de referencia para movimiento de la IA
        var pivotGO = new GameObject("TargetMoveCircle");

        // Lo emparenta al padre de este objeto para que se mueva en su mismo espacio
        pivotGO.transform.parent = transform.parent;


        // Precalcula el límite inferior de altura (-height)
        _halfHeightNeg = -height;

        // Precalcula el límite superior de altura (+height)
        _halfHeightPos = height;
    }

    // ────────────────────────────────────────────────────────────────────────
    // Método llamado cada frame por Unity
    void Update()
    {


        // Reduce el temporizador con el tiempo transcurrido desde el último frame
        _scanTimer -= Time.deltaTime;

        // Si aún no se cumplió el intervalo, sale sin escanear
        if (_scanTimer > 0f) return;

        // Reinicia el temporizador al valor del intervalo
        _scanTimer = _scanInterval;

        // Ejecuta el escaneo de detección
        Scan();
    }

    // ────────────────────────────────────────────────────────────────────────
    // Busca colliders en rango y verifica si algún jugador está en la línea de visión
    void Scan()
    {


        ViewPlayer = null;

        // Detecta colliders en una esfera alrededor de la posición actual, sin asignar memoria nueva
        _count = Physics.OverlapSphereNonAlloc(
            transform.position, distance, _colliders, enemyLayers);

        // Recorre todos los colliders detectados
        for (int i = 0; i < _count; i++)
        {
            // Si el collider tiene el tag "Player" y está dentro del campo de visión
            if (_colliders[i] != null && _colliders[i].CompareTag("Toy") && IsInSight(_colliders[i]))
            {
                // Almacena el transform del jugador detectado
                ViewPlayer = _colliders[i].transform;

                // Sale del método al encontrar al primer jugador visible
                return;
            }
        }

    }

    // ────────────────────────────────────────────────────────────────────────
    // Determina si un collider específico está dentro del campo de visión
    public bool IsInSight(Collider other)
    {
        // Obtiene la posición del sensor (este objeto)
        Vector3 origin = transform.position;

        // Obtiene la posición del objetivo a evaluar
        Vector3 dest = other.transform.position;

        // Calcula el vector dirección desde el sensor hasta el objetivo
        Vector3 direction = dest - origin;

        // 1. Comprueba si el objetivo está fuera del rango vertical permitido
        if (direction.y < _halfHeightNeg || direction.y > _halfHeightPos)
            return false;

        // Anula el componente vertical para trabajar solo en el plano horizontal
        direction.y = 0f;

        // 2. Compara la distancia al cuadrado (evita calcular raíz cuadrada, más eficiente)
        if (direction.sqrMagnitude > distance * distance) return false;

        // Comprueba si el ángulo entre la dirección al objetivo y el frente del sensor excede el límite
        if (Vector3.Angle(direction, transform.forward) > angle)
            return false;



        // Traza una línea entre el ojo y el destino; retorna true si NO hay obstáculo
        return !Physics.Linecast(positionEye.position, dest, occlusionLayers);
    }

    // Sobrecarga que acepta un GameObject y obtiene su Collider para reutilizar la lógica
    public bool IsInSight(GameObject obj) => IsInSight(obj.GetComponent<Collider>());

    // ────────────────────────────────────────────────────────────────────────
    // Genera una malla en forma de cuña que representa visualmente el cono de visión
    Mesh CreateWedgeMesh()
    {
        // Crea una nueva malla vacía
        var wedge = new Mesh();

        // Define el número de segmentos del arco (mayor = más suave)
        const int segments = 10;

        // Calcula el número total de triángulos: 4 por segmento + 4 para las caras laterales
        int numTriangles = (segments * 4) + 4;

        // Cada triángulo tiene 3 vértices
        int numVertices = numTriangles * 3;

        // Array para almacenar las posiciones de los vértices
        var vertices = new Vector3[numVertices];

        // Array para almacenar los índices de los triángulos
        var triangles = new int[numVertices];

        // Precalcula la rotación hacia la izquierda del ángulo de visión
        var rotLeft = Quaternion.Euler(0, -angle, 0);

        // Precalcula la rotación hacia la derecha del ángulo de visión
        var rotRight = Quaternion.Euler(0, angle, 0);

        // Centro inferior de la cuña (origen del cono)
        Vector3 bottomCenter = Vector3.zero;

        // Centro superior de la cuña (elevado por la altura)
        Vector3 topCenter = Vector3.up * height;

        // Esquina inferior izquierda del borde del cono
        Vector3 bottomLeft = rotLeft * Vector3.forward * distance;

        // Esquina inferior derecha del borde del cono
        Vector3 bottomRight = rotRight * Vector3.forward * distance;

        // Esquina superior izquierda del borde del cono
        Vector3 topLeft = bottomLeft + Vector3.up * height;

        // Esquina superior derecha del borde del cono
        Vector3 topRight = bottomRight + Vector3.up * height;

        // Índice actual del vértice que se está escribiendo
        int vert = 0;

        // ── Cara lateral izquierda (2 triángulos) ──
        vertices[vert++] = bottomCenter; vertices[vert++] = bottomLeft; vertices[vert++] = topLeft;
        vertices[vert++] = topLeft; vertices[vert++] = topCenter; vertices[vert++] = bottomCenter;

        // ── Cara lateral derecha (2 triángulos) ──
        vertices[vert++] = bottomCenter; vertices[vert++] = topCenter; vertices[vert++] = topRight;
        vertices[vert++] = topRight; vertices[vert++] = bottomRight; vertices[vert++] = bottomCenter;

        // Ángulo inicial (extremo izquierdo del cono)
        float currentAngle = -angle;

        // Incremento angular por segmento
        float deltaAngle = (angle * 2f) / segments;

        // Recorre cada segmento del arco para construir las caras frontal, superior e inferior
        for (int i = 0; i < segments; i++)
        {
            // Calcula la esquina inferior izquierda del segmento actual
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;

            // Calcula la esquina inferior derecha del segmento actual
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;

            // Calcula la esquina superior izquierda elevando la inferior
            topLeft = bottomLeft + Vector3.up * height;

            // Calcula la esquina superior derecha elevando la inferior
            topRight = bottomRight + Vector3.up * height;

            // ── Cara frontal del segmento (2 triángulos) ──
            vertices[vert++] = bottomLeft; vertices[vert++] = bottomRight; vertices[vert++] = topRight;
            vertices[vert++] = topRight; vertices[vert++] = topLeft; vertices[vert++] = bottomLeft;

            // ── Tapa superior del segmento (1 triángulo) ──
            vertices[vert++] = topCenter; vertices[vert++] = topLeft; vertices[vert++] = topRight;

            // ── Tapa inferior del segmento (1 triángulo) ──
            vertices[vert++] = bottomCenter; vertices[vert++] = bottomRight; vertices[vert++] = bottomLeft;

            // Avanza al siguiente ángulo del segmento
            currentAngle += deltaAngle;
        }

        // Asigna los índices de triángulos secuencialmente (cada vértice es único)
        for (int i = 0; i < numVertices; i++) triangles[i] = i;

        // Asigna los vértices a la malla
        wedge.vertices = vertices;

        // Asigna los triángulos a la malla
        wedge.triangles = triangles;

        // Recalcula las normales para una iluminación correcta
        wedge.RecalculateNormals();

        // Devuelve la malla generada
        return wedge;
    }

    // Método llamado por Unity al modificar valores en el Inspector
    void OnValidate()
    {
        // Regenera la malla del cono con los nuevos valores
        _mesh = CreateWedgeMesh();

        // Actualiza el límite inferior de altura precalculado
        _halfHeightNeg = -height;

        // Actualiza el límite superior de altura precalculado
        _halfHeightPos = height;
    }

    // Método llamado por Unity para dibujar gizmos en la vista de escena
    void OnDrawGizmos()
    {
        if (!IsDrawGizmo) return;

        if (_mesh != null)
        {
            // Cambia el color del cono según si hay un jugador detectado
            Gizmos.color = ViewPlayer != null ? detectedColor : meshColor;
            Gizmos.DrawMesh(_mesh, transform.position, transform.rotation);
        }
        if (ViewPlayer != null && positionEye != null)
        {

            if (!Physics.Linecast(positionEye.position, ViewPlayer.position, occlusionLayers))
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(positionEye.position, ViewPlayer.position);

            }

        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(positionEye.position, positionEye.position + positionEye.forward * 5);
        }
        Gizmos.color = Color.green;
        for (int i = 0; i < _count; i++)
        {
            if (_colliders[i] != null)
                Gizmos.DrawSphere(_colliders[i].transform.position, 0.5f);
        }
    }
}
