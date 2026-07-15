using UnityEngine;
using UnityEngine.UI;

public class ItemCollector : MonoBehaviour
{
    [Header("Configuración")]
    public ItemData itemData;
    public float rotationSpeed = 50f;
    public float floatSpeed = 1f;
    public float floatHeight = 0.3f;
    public string mensajeRecoleccion = "Presiona E para recoger";
    
    private Vector3 startPos;
    private bool canCollect = true;
    private bool playerNear = false;
    private InventarioController inventario;
    private GameObject mensajeUI;

    void Start()
    {
        startPos = transform.position;
        
        if (itemData == null)
        {
            itemData = ScriptableObject.CreateInstance<ItemData>();
            itemData.nombre = gameObject.name;
        }
        
        // Buscar inventario
        inventario = FindObjectOfType<InventarioController>();
        
        // Crear mensaje UI (opcional)
        CrearMensajeUI();
    }

    void Update()
    {
        // Rotación flotante
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = startPos + Vector3.up * offset;
        
        // Mostrar mensaje si el jugador está cerca
        if (mensajeUI != null)
        {
            mensajeUI.SetActive(playerNear && canCollect);
        }
        
        // Recoger con E
        if (playerNear && canCollect && Input.GetKeyDown(KeyCode.E))
        {
            Recoger();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Gato"))
        {
            playerNear = true;
            Debug.Log($"👀 Presiona E para recoger '{itemData.nombre}'");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Gato"))
        {
            playerNear = false;
        }
    }

    void Recoger()
    {
        if (!canCollect || inventario == null) return;
        
        // Verificar si el inventario está lleno
        if (inventario.EstaLleno())
        {
            Debug.Log("⚠️ Inventario lleno! Usa 'I' para abrir y hacer espacio.");
            return;
        }
        
        // Recoger el item
        canCollect = false;
        inventario.RecogerItem(itemData);
        
        // Ocultar mensaje
        if (mensajeUI != null)
        {
            mensajeUI.SetActive(false);
        }
        
        // Destruir el objeto después de recogerlo
        Destroy(gameObject, 0.3f);
        Debug.Log($"🎯 Item '{itemData.nombre}' recogido!");
    }

    void CrearMensajeUI()
    {
        // Buscar canvas existente o crear uno
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas_Mensajes");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        
        // Crear mensaje
        mensajeUI = new GameObject("Mensaje_Recoger");
        mensajeUI.transform.SetParent(canvas.transform);
        
        RectTransform rect = mensajeUI.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.8f);
        rect.anchorMax = new Vector2(0.5f, 0.8f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(300, 50);
        
        Text text = mensajeUI.AddComponent<Text>();
        text.text = mensajeRecoleccion;
        text.font = Font.CreateDynamicFontFromOSFont("Arial", 14);
        text.fontSize = 20;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        
        // Fondo semitransparente
        Image bg = mensajeUI.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.7f);
        
        mensajeUI.SetActive(false);
        Debug.Log("✅ Mensaje UI creado");
    }
}