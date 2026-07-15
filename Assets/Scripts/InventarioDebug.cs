using UnityEngine;

public class InventarioDebug : MonoBehaviour
{
    [Header("items de prueba")]
    public ItemData llave;
    public ItemData pocion;
    public ItemData nota;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) Recoger(llave);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Recoger(pocion);
        if (Input.GetKeyDown(KeyCode.Alpha3)) Recoger(nota);
        if (Input.GetKeyDown(KeyCode.Alpha0)) Limpiar();
    }

    void Recoger(ItemData item)
    {
        if (item == null) return;
        var inv = FindObjectOfType<InventarioController>();
        inv?.RecogerItem(item);
    }

    void Limpiar()
    {
        var inv = FindObjectOfType<InventarioController>();
        if (inv == null) return;
        
        var items = inv.GetItems();
        for (int i = 0; i < items.Count; i++)
            inv.RemoverItem(i);
    }
}