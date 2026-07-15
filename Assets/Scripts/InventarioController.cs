using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventarioController : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject inventarioPanel;
    public List<GameObject> slots = new List<GameObject>();
    public GameObject emptyText;
    public float slotSize = 80f;
    public float spacing = 15f;
    
    private bool inventarioAbierto = false;
    private List<ItemData> items = new List<ItemData>();
    private GameManager gameManager;
    
    // Item pendiente de guardar (recolección)
    private ItemData itemPendiente = null;
    private bool esperandoSeleccionSlot = false;

    // Evento para notificar cambios
    public System.Action OnInventarioActualizado;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        if (inventarioPanel != null)
        {
            inventarioPanel.SetActive(false);
        }
        
        // Inicializar slots vacíos
        for (int i = 0; i < slots.Count; i++)
        {
            items.Add(null);
        }
        
        // Conectar slots a eventos de clic
        ConectarSlots();
        
        Debug.Log("✅ Inventario inicializado");
    }

    void Update()
    {
        // Abrir/cerrar con tecla I
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (esperandoSeleccionSlot)
            {
                // Si está esperando selección, cancelar
                CancelarRecoleccion();
            }
            else
            {
                ToggleInventario();
            }
        }
        
        // Cerrar con ESC (si no está esperando selección)
        if (Input.GetKeyDown(KeyCode.Escape) && inventarioAbierto && !esperandoSeleccionSlot)
        {
            CerrarInventario();
        }
    }

    void ConectarSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            GameObject slot = slots[i];
            if (slot == null) continue;
            
            Button btn = slot.GetComponent<Button>();
            if (btn != null)
            {
                int index = i; // Capturar índice
                btn.onClick.AddListener(() => OnSlotClick(index));
            }
        }
    }

    void OnSlotClick(int index)
    {
        if (esperandoSeleccionSlot && itemPendiente != null)
        {
            // Guardar item en el slot seleccionado
            GuardarItemEnSlot(index, itemPendiente);
            itemPendiente = null;
            esperandoSeleccionSlot = false;
            CerrarInventario();
            Debug.Log($"✅ Item guardado en slot {index + 1}");
        }
        else
        {
            // Si hay un item en el slot, mostrar opción de usarlo
            if (index < items.Count && items[index] != null)
            {
                Debug.Log($"🔍 Slot {index + 1}: {items[index].nombre}");
                // Aquí puedes agregar acción de usar item (opcional)
            }
        }
    }

    void GuardarItemEnSlot(int index, ItemData item)
    {
        if (index < 0 || index >= items.Count) return;
        
        // Reemplazar item si ya existe
        if (items[index] != null)
        {
            Debug.Log($"🔄 Reemplazando '{items[index].nombre}' por '{item.nombre}' en slot {index + 1}");
        }
        
        items[index] = item;
        ActualizarUI();
        
        if (OnInventarioActualizado != null)
            OnInventarioActualizado.Invoke();
    }

    // === MÉTODO PRINCIPAL: Recoger item ===
    public void RecogerItem(ItemData item)
    {
        if (item == null) return;
        
        // Buscar slot vacío automáticamente (si hay)
        int slotVacio = -1;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                slotVacio = i;
                break;
            }
        }
        
        if (slotVacio != -1 && !esperandoSeleccionSlot)
        {
            // Guardar directamente en slot vacío
            items[slotVacio] = item;
            ActualizarUI();
            Debug.Log($"✅ Item '{item.nombre}' guardado automáticamente en slot {slotVacio + 1}");
            
            if (OnInventarioActualizado != null)
                OnInventarioActualizado.Invoke();
        }
        else
        {
            // No hay slots vacíos o ya hay un item pendiente
            if (esperandoSeleccionSlot)
            {
                Debug.Log("⚠️ Ya hay un item pendiente de guardar");
                return;
            }
            
            // Abrir inventario para seleccionar slot
            itemPendiente = item;
            esperandoSeleccionSlot = true;
            AbrirInventario();
            Debug.Log($"📦 Item '{item.nombre}' recogido. Selecciona un slot para guardarlo.");
            
            // Mostrar mensaje en pantalla (opcional)
            if (emptyText != null)
            {
                emptyText.GetComponent<Text>().text = $"📦 {item.nombre} - Selecciona un slot";
                emptyText.GetComponent<Text>().color = new Color(1f, 0.8f, 0.2f);
                emptyText.SetActive(true);
            }
        }
    }

    void CancelarRecoleccion()
    {
        itemPendiente = null;
        esperandoSeleccionSlot = false;
        
        if (emptyText != null)
        {
            emptyText.GetComponent<Text>().text = "📭 Inventario vacío";
            emptyText.GetComponent<Text>().color = new Color(0.6f, 0.55f, 0.5f);
            ActualizarUI();
        }
        
        CerrarInventario();
        Debug.Log("❌ Recolección cancelada");
    }

    public void ToggleInventario()
    {
        if (inventarioAbierto)
        {
            if (esperandoSeleccionSlot)
            {
                CancelarRecoleccion();
            }
            else
            {
                CerrarInventario();
            }
        }
        else
        {
            AbrirInventario();
        }
    }

    void AbrirInventario()
    {
        inventarioAbierto = true;
        if (inventarioPanel != null)
        {
            inventarioPanel.SetActive(true);
        }
        
        if (gameManager != null)
        {
            gameManager.PausarJuego();
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        ActualizarUI();
        Debug.Log("📂 Inventario abierto");
    }

    void CerrarInventario()
    {
        inventarioAbierto = false;
        if (inventarioPanel != null)
        {
            inventarioPanel.SetActive(false);
        }
        
        if (gameManager != null && !esperandoSeleccionSlot)
        {
            gameManager.ReanudarJuego();
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Debug.Log("📂 Inventario cerrado");
    }

    // === MÉTODOS PÚBLICOS ===

    public bool AgregarItem(ItemData item)
    {
        // Buscar slot vacío
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                ActualizarUI();
                
                if (OnInventarioActualizado != null)
                    OnInventarioActualizado.Invoke();
                    
                Debug.Log($"✅ Item '{item.nombre}' agregado al inventario (slot {i + 1})");
                return true;
            }
        }
        
        Debug.Log("⚠️ Inventario lleno! Usa 'E' para reemplazar un item.");
        return false;
    }

    public void RemoverItem(int index)
    {
        if (index >= 0 && index < items.Count && items[index] != null)
        {
            Debug.Log($"🗑️ Item '{items[index].nombre}' removido del inventario");
            items[index] = null;
            ActualizarUI();
            
            if (OnInventarioActualizado != null)
                OnInventarioActualizado.Invoke();
        }
    }

    public bool TieneItem(string nombreItem)
    {
        foreach (ItemData item in items)
        {
            if (item != null && item.nombre == nombreItem)
            {
                return true;
            }
        }
        return false;
    }

    public ItemData ObtenerItem(string nombreItem)
    {
        foreach (ItemData item in items)
        {
            if (item != null && item.nombre == nombreItem)
            {
                return item;
            }
        }
        return null;
    }

    public bool EstaLleno()
    {
        foreach (ItemData item in items)
        {
            if (item == null) return false;
        }
        return true;
    }

    public bool EstaAbierto()
    {
        return inventarioAbierto;
    }

    public List<ItemData> GetItems()
    {
        return items;
    }

    void ActualizarUI()
    {
        bool tieneItems = false;
        
        for (int i = 0; i < slots.Count; i++)
        {
            GameObject slot = slots[i];
            if (slot == null) continue;
            
            Transform iconTransform = slot.transform.Find("Icono");
            GameObject iconGO = iconTransform != null ? iconTransform.gameObject : null;
            
            Transform cantTransform = slot.transform.Find("Cantidad");
            GameObject cantGO = cantTransform != null ? cantTransform.gameObject : null;
            
            if (i < items.Count && items[i] != null)
            {
                ItemData item = items[i];
                
                if (iconGO != null)
                {
                    iconGO.SetActive(true);
                    Image iconImg = iconGO.GetComponent<Image>();
                    if (iconImg != null && item.icono != null)
                    {
                        iconImg.sprite = item.icono;
                        iconImg.color = Color.white;
                    }
                }
                
                if (cantGO != null && item.cantidad > 1)
                {
                    cantGO.SetActive(true);
                    Text cantText = cantGO.GetComponent<Text>();
                    if (cantText != null)
                    {
                        cantText.text = item.cantidad.ToString();
                    }
                }
                else if (cantGO != null)
                {
                    cantGO.SetActive(false);
                }
                
                tieneItems = true;
            }
            else
            {
                if (iconGO != null)
                {
                    iconGO.SetActive(false);
                    Image iconImg = iconGO.GetComponent<Image>();
                    if (iconImg != null)
                    {
                        iconImg.color = new Color(1, 1, 1, 0);
                    }
                }
                
                if (cantGO != null)
                {
                    cantGO.SetActive(false);
                }
            }
        }
        
        // Mostrar texto de estado
        if (emptyText != null)
        {
            Text text = emptyText.GetComponent<Text>();
            if (!esperandoSeleccionSlot)
            {
                emptyText.SetActive(!tieneItems);
                if (text != null)
                {
                    text.text = "📭 Inventario vacío";
                    text.color = new Color(0.6f, 0.55f, 0.5f);
                }
            }
            else
            {
                emptyText.SetActive(true);
                if (text != null && itemPendiente != null)
                {
                    text.text = $"📦 {itemPendiente.nombre} - Selecciona un slot";
                    text.color = new Color(1f, 0.8f, 0.2f);
                }
            }
        }
    }
}