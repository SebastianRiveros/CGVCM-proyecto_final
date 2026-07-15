using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventarioController : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject inventarioPanel;
    public List<GameObject> slots = new List<GameObject>();
    public GameObject emptyText;
    
    private List<ItemData> items = new List<ItemData>();
    private GameManager gameManager;
    private ItemData itemPendiente;
    private bool abierto;
    private bool esperandoSlot;
    
    public System.Action OnInventarioActualizado;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        inventarioPanel?.SetActive(false);
        
        for (int i = 0; i < slots.Count; i++)
            items.Add(null);
        
        for (int i = 0; i < slots.Count; i++)
        {
            int idx = i;
            slots[i]?.GetComponent<Button>()?.onClick.AddListener(() => OnSlotClick(idx));
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (esperandoSlot) CancelarRecoleccion();
            else ToggleInventario();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape) && abierto && !esperandoSlot)
            CerrarInventario();
    }

    void OnSlotClick(int index)
    {
        if (esperandoSlot && itemPendiente != null)
        {
            if (index < items.Count)
            {
                items[index] = itemPendiente;
                itemPendiente = null;
                esperandoSlot = false;
                ActualizarUI();
                CerrarInventario();
                OnInventarioActualizado?.Invoke();
            }
        }
        else
        {
            if (index < items.Count && items[index] != null)
                Debug.Log("slot " + (index + 1) + ": " + items[index].nombre);
        }
    }

    public void RecogerItem(ItemData item)
    {
        if (item == null || esperandoSlot) return;
        
        int vacio = items.FindIndex(i => i == null);
        
        if (vacio != -1)
        {
            items[vacio] = item;
            ActualizarUI();
            OnInventarioActualizado?.Invoke();
            Debug.Log("item guardado en slot " + (vacio + 1));
        }
        else
        {
            itemPendiente = item;
            esperandoSlot = true;
            AbrirInventario();
            Debug.Log("selecciona un slot para " + item.nombre);
            if (emptyText != null)
            {
                var txt = emptyText.GetComponent<Text>();
                txt.text = "selecciona un slot";
                txt.color = Color.yellow;
                emptyText.SetActive(true);
            }
        }
    }

    void CancelarRecoleccion()
    {
        itemPendiente = null;
        esperandoSlot = false;
        CerrarInventario();
        if (emptyText != null)
        {
            var txt = emptyText.GetComponent<Text>();
            txt.text = "inventario vacio";
            txt.color = Color.gray;
        }
        ActualizarUI();
    }

    public void ToggleInventario()
    {
        if (abierto)
        {
            if (esperandoSlot) CancelarRecoleccion();
            else CerrarInventario();
        }
        else AbrirInventario();
    }

    void AbrirInventario()
    {
        abierto = true;
        inventarioPanel?.SetActive(true);
        gameManager?.PausarJuego();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ActualizarUI();
    }

    void CerrarInventario()
    {
        abierto = false;
        inventarioPanel?.SetActive(false);
        if (gameManager != null && !esperandoSlot)
            gameManager.ReanudarJuego();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool AgregarItem(ItemData item)
    {
        int vacio = items.FindIndex(i => i == null);
        if (vacio == -1) return false;
        
        items[vacio] = item;
        ActualizarUI();
        OnInventarioActualizado?.Invoke();
        return true;
    }

    public void RemoverItem(int index)
    {
        if (index >= 0 && index < items.Count && items[index] != null)
        {
            items[index] = null;
            ActualizarUI();
            OnInventarioActualizado?.Invoke();
        }
    }

    public bool TieneItem(string nombre)
    {
        return items.Exists(i => i != null && i.nombre == nombre);
    }

    public ItemData ObtenerItem(string nombre)
    {
        return items.Find(i => i != null && i.nombre == nombre);
    }

    public bool EstaLleno() => items.TrueForAll(i => i != null);
    public bool EstaAbierto() => abierto;
    public List<ItemData> GetItems() => items;

    void ActualizarUI()
    {
        bool tieneItems = false;
        
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == null) continue;
            
            var icono = slots[i].transform.Find("Icono")?.gameObject;
            var cantidad = slots[i].transform.Find("Cantidad")?.gameObject;
            
            if (i < items.Count && items[i] != null)
            {
                var item = items[i];
                tieneItems = true;
                
                if (icono != null)
                {
                    icono.SetActive(true);
                    var img = icono.GetComponent<Image>();
                    if (img != null && item.icono != null)
                    {
                        img.sprite = item.icono;
                        img.color = Color.white;
                    }
                }
                
                if (cantidad != null)
                {
                    var txt = cantidad.GetComponent<Text>();
                    if (item.cantidad > 1 && txt != null)
                    {
                        cantidad.SetActive(true);
                        txt.text = item.cantidad.ToString();
                    }
                    else cantidad.SetActive(false);
                }
            }
            else
            {
                if (icono != null)
                {
                    icono.SetActive(false);
                    var img = icono.GetComponent<Image>();
                    if (img != null) img.color = new Color(1, 1, 1, 0);
                }
                if (cantidad != null) cantidad.SetActive(false);
            }
        }
        
        if (emptyText != null)
        {
            var txt = emptyText.GetComponent<Text>();
            if (!esperandoSlot)
            {
                emptyText.SetActive(!tieneItems);
                if (txt != null)
                {
                    txt.text = "inventario vacio";
                    txt.color = Color.gray;
                }
            }
            else
            {
                emptyText.SetActive(true);
                if (txt != null && itemPendiente != null)
                {
                    txt.text = "selecciona un slot";
                    txt.color = Color.yellow;
                }
            }
        }
    }
}