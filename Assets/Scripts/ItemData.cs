using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NuevoItem", menuName = "Inventario/Item")]
public class ItemData : ScriptableObject
{
    public string nombre = "Item";
    public Sprite icono;
    public int cantidad = 1;
    public string descripcion = "";
    public bool esConsumible = true;
    public bool esClave = false;
    
    [Header("Efecto (opcional)")]
    public Action efecto; // Para efectos personalizados
}