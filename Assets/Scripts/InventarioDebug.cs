using UnityEngine;

public class InventarioDebug : MonoBehaviour
{
    [Header("Items de Prueba")]
    public ItemData itemLlave;
    public ItemData itemPocion;
    public ItemData itemNota;

    void Update()
    {
        // Tecla 1: Agregar Llave (directo al inventario)
        if (Input.GetKeyDown(KeyCode.Alpha1) && itemLlave != null)
        {
            InventarioController inv = FindObjectOfType<InventarioController>();
            if (inv != null) inv.RecogerItem(itemLlave);
        }
        
        // Tecla 2: Agregar Pocion
        if (Input.GetKeyDown(KeyCode.Alpha2) && itemPocion != null)
        {
            InventarioController inv = FindObjectOfType<InventarioController>();
            if (inv != null) inv.RecogerItem(itemPocion);
        }
        
        // Tecla 3: Agregar Nota
        if (Input.GetKeyDown(KeyCode.Alpha3) && itemNota != null)
        {
            InventarioController inv = FindObjectOfType<InventarioController>();
            if (inv != null) inv.RecogerItem(itemNota);
        }
        
        // Tecla 0: Limpiar inventario
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            InventarioController inv = FindObjectOfType<InventarioController>();
            if (inv != null)
            {
                var items = inv.GetItems();
                for (int i = 0; i < items.Count; i++)
                {
                    inv.RemoverItem(i);
                }
            }
        }
    }
}