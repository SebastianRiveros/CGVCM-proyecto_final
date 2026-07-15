using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EscapeRoomBuilder : MonoBehaviour
{
    [Header("Configuración General")]
    public float roomWidth = 12f;
    public float roomHeight = 4f;
    public float roomDepth = 10f;
    
    [Header("Elementos del Escenario")]
    public bool generarMuebles = true;
    public bool generarObjetos = true;
    public bool generarLuces = true;
    public bool generarDecoracion = true;
    
    [Header("Colores y Estilos")]
    public Color paredColor = new Color(0.2f, 0.15f, 0.1f);
    public Color pisoColor = new Color(0.3f, 0.25f, 0.2f);
    public Color techoColor = new Color(0.9f, 0.85f, 0.8f);
    
    [Header("Contenedor")]
    public string nombreContenedor = "EscapeRoom_Generado";
    
    private List<GameObject> generatedObjects = new List<GameObject>();
    private System.Random random = new System.Random();

#if UNITY_EDITOR
    [ContextMenu("Generar Escenario (Guardar)")]
    public void GenerarYGuardar()
    {
        // Limpiar contenedor anterior
        LimpiarContenedor();
        
        // Crear contenedor
        GameObject contenedor = new GameObject(nombreContenedor);
        contenedor.transform.position = Vector3.zero;
        
        generatedObjects.Clear();
        
        // Generar todo dentro del contenedor
        CrearEstructuraHabitacion(contenedor);
        CrearPisoYTecho(contenedor);
        CrearPuerta(contenedor);
        
        if (generarMuebles) CrearMuebles(contenedor);
        if (generarObjetos) CrearObjetosInteractivos(contenedor);
        if (generarLuces) CrearIluminacion(contenedor);
        if (generarDecoracion) CrearDecoracion(contenedor);
        
        CrearPistasVisuales(contenedor);
        
        // Marcar como estático para optimización
        contenedor.isStatic = true;
        
        // Notificar
        Debug.Log($"✅ Escenario generado con {generatedObjects.Count} objetos en '{nombreContenedor}'");
        
        // Seleccionar el contenedor en la jerarquía
        Selection.activeGameObject = contenedor;
        
        // Guardar cambios en la escena
        if (EditorSceneManager.GetActiveScene().isDirty)
        {
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            Debug.Log("💾 Escena guardada automáticamente");
        }
    }
    
    [ContextMenu("Limpiar Escenario")]
    public void LimpiarEscenario()
    {
        LimpiarContenedor();
        Debug.Log("🗑️ Escenario limpiado");
    }
#endif

    void LimpiarContenedor()
    {
        GameObject contenedor = GameObject.Find(nombreContenedor);
        if (contenedor != null)
        {
            // Destruir en modo Editor
            #if UNITY_EDITOR
            DestroyImmediate(contenedor);
            #else
            Destroy(contenedor);
            #endif
        }
    }

    #region Estructura de la Habitación
    
    void CrearEstructuraHabitacion(GameObject parent)
    {
        CrearPared(parent, "ParedFrente", new Vector3(0, roomHeight/2, roomDepth/2), new Vector3(roomWidth, roomHeight, 0.2f));
        CrearPared(parent, "ParedFondo", new Vector3(0, roomHeight/2, -roomDepth/2), new Vector3(roomWidth, roomHeight, 0.2f));
        CrearPared(parent, "ParedIzquierda", new Vector3(-roomWidth/2, roomHeight/2, 0), new Vector3(0.2f, roomHeight, roomDepth));
        CrearPared(parent, "ParedDerecha", new Vector3(roomWidth/2, roomHeight/2, 0), new Vector3(0.2f, roomHeight, roomDepth));
        
        for (int i = 0; i < 4; i++)
        {
            Vector3 pos = ObtenerEsquina(i);
            CrearMolduraEsquina(parent, pos);
        }
        
        CrearZocalos(parent);
    }
    
    Vector3 ObtenerEsquina(int index)
    {
        float x = (index % 2 == 0) ? -roomWidth/2 : roomWidth/2;
        float z = (index < 2) ? roomDepth/2 : -roomDepth/2;
        return new Vector3(x, 0.1f, z);
    }
    
    void CrearPared(GameObject parent, string nombre, Vector3 pos, Vector3 escala)
    {
        GameObject pared = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pared.name = nombre;
        pared.transform.position = pos;
        pared.transform.localScale = escala;
        pared.transform.parent = parent.transform;
        pared.isStatic = true;
        
        Renderer renderer = pared.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = paredColor;
        renderer.material = mat;
        
        generatedObjects.Add(pared);
    }
    
    void CrearMolduraEsquina(GameObject parent, Vector3 pos)
    {
        GameObject moldura = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        moldura.name = "MolduraEsquina";
        moldura.transform.position = new Vector3(pos.x, 2.5f, pos.z);
        moldura.transform.localScale = new Vector3(0.3f, 0.1f, 0.3f);
        moldura.transform.parent = parent.transform;
        moldura.isStatic = true;
        
        Renderer renderer = moldura.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(0.8f, 0.75f, 0.6f);
        renderer.material = mat;
        
        generatedObjects.Add(moldura);
    }
    
    void CrearZocalos(GameObject parent)
    {
        float alturaZocalo = 0.15f;
        
        CrearZocalo(parent, "ZocaloFrente", new Vector3(0, alturaZocalo/2, roomDepth/2 + 0.1f), 
                   new Vector3(roomWidth - 0.5f, alturaZocalo, 0.15f));
        
        CrearZocalo(parent, "ZocaloFondo", new Vector3(0, alturaZocalo/2, -roomDepth/2 - 0.1f), 
                   new Vector3(roomWidth - 0.5f, alturaZocalo, 0.15f));
        
        CrearZocalo(parent, "ZocaloIzquierda", new Vector3(-roomWidth/2 - 0.1f, alturaZocalo/2, 0), 
                   new Vector3(0.15f, alturaZocalo, roomDepth - 0.5f));
        
        CrearZocalo(parent, "ZocaloDerecha", new Vector3(roomWidth/2 + 0.1f, alturaZocalo/2, 0), 
                   new Vector3(0.15f, alturaZocalo, roomDepth - 0.5f));
    }
    
    void CrearZocalo(GameObject parent, string nombre, Vector3 pos, Vector3 escala)
    {
        GameObject zocalo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        zocalo.name = nombre;
        zocalo.transform.position = pos;
        zocalo.transform.localScale = escala;
        zocalo.transform.parent = parent.transform;
        zocalo.isStatic = true;
        
        Renderer renderer = zocalo.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(0.4f, 0.35f, 0.25f);
        renderer.material = mat;
        
        generatedObjects.Add(zocalo);
    }
    
    #endregion
    
    #region Piso y Techo
    
    void CrearPisoYTecho(GameObject parent)
    {
        GameObject piso = GameObject.CreatePrimitive(PrimitiveType.Cube);
        piso.name = "Piso";
        piso.transform.position = new Vector3(0, -0.1f, 0);
        piso.transform.localScale = new Vector3(roomWidth, 0.2f, roomDepth);
        piso.transform.parent = parent.transform;
        piso.isStatic = true;
        
        Renderer rendererPiso = piso.GetComponent<Renderer>();
        Material matPiso = new Material(Shader.Find("Standard"));
        matPiso.color = pisoColor;
        rendererPiso.material = matPiso;
        
        generatedObjects.Add(piso);
        
        GameObject techo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        techo.name = "Techo";
        techo.transform.position = new Vector3(0, roomHeight + 0.1f, 0);
        techo.transform.localScale = new Vector3(roomWidth, 0.2f, roomDepth);
        techo.transform.parent = parent.transform;
        techo.isStatic = true;
        
        Renderer rendererTecho = techo.GetComponent<Renderer>();
        Material matTecho = new Material(Shader.Find("Standard"));
        matTecho.color = techoColor;
        rendererTecho.material = matTecho;
        
        generatedObjects.Add(techo);
    }
    
    #endregion
    
    #region Puerta
    
    void CrearPuerta(GameObject parent)
    {
        GameObject marco = GameObject.CreatePrimitive(PrimitiveType.Cube);
        marco.name = "MarcoPuerta";
        marco.transform.position = new Vector3(0, 1.5f, roomDepth/2 + 0.1f);
        marco.transform.localScale = new Vector3(2.2f, 3f, 0.3f);
        marco.transform.parent = parent.transform;
        marco.isStatic = true;
        
        Renderer rendererMarco = marco.GetComponent<Renderer>();
        Material matMarco = new Material(Shader.Find("Standard"));
        matMarco.color = new Color(0.4f, 0.3f, 0.2f);
        rendererMarco.material = matMarco;
        
        generatedObjects.Add(marco);
        
        GameObject puerta = GameObject.CreatePrimitive(PrimitiveType.Cube);
        puerta.name = "Puerta";
        puerta.transform.position = new Vector3(0, 1.2f, roomDepth/2 + 0.2f);
        puerta.transform.localScale = new Vector3(1.8f, 2.6f, 0.15f);
        puerta.transform.parent = parent.transform;
        puerta.isStatic = true;
        
        Renderer rendererPuerta = puerta.GetComponent<Renderer>();
        Material matPuerta = new Material(Shader.Find("Standard"));
        matPuerta.color = new Color(0.6f, 0.4f, 0.2f);
        rendererPuerta.material = matPuerta;
        
        generatedObjects.Add(puerta);
        
        GameObject pomo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pomo.name = "Pomo";
        pomo.transform.position = new Vector3(0.6f, 1.2f, roomDepth/2 + 0.3f);
        pomo.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        pomo.transform.parent = parent.transform;
        pomo.isStatic = true;
        
        Renderer rendererPomo = pomo.GetComponent<Renderer>();
        Material matPomo = new Material(Shader.Find("Standard"));
        matPomo.color = new Color(0.8f, 0.7f, 0.2f);
        matPomo.SetFloat("_Metallic", 0.9f);
        rendererPomo.material = matPomo;
        
        generatedObjects.Add(pomo);
    }
    
    #endregion
    
    #region Muebles (versión resumida - misma lógica que antes pero con parent)
    
    void CrearMuebles(GameObject parent)
    {
        CrearMesa(parent, new Vector3(0, 0.5f, 0), 2f, 1f, 1.5f);
        CrearEstanteria(parent, new Vector3(-4f, 1f, -3f));
        CrearArmario(parent, new Vector3(4f, 1f, -3f));
        CrearSilla(parent, new Vector3(-1.5f, 0.5f, 1.5f));
        CrearSilla(parent, new Vector3(1.5f, 0.5f, 1.5f));
        CrearSofa(parent, new Vector3(-3f, 0.5f, 3.5f));
        CrearEscritorio(parent, new Vector3(4.5f, 0.6f, 2f));
        CrearCama(parent, new Vector3(-3.5f, 0.3f, -3.5f));
    }
    
    void CrearMesa(GameObject parent, Vector3 pos, float ancho, float alto, float largo)
    {
        GameObject tablaMesa = new GameObject("Mesa");
        tablaMesa.transform.position = pos;
        tablaMesa.transform.parent = parent.transform;
        tablaMesa.isStatic = true;
        
        GameObject tablero = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tablero.name = "Mesa_Tablero";
        tablero.transform.position = pos + new Vector3(0, alto, 0);
        tablero.transform.localScale = new Vector3(ancho, 0.1f, largo);
        tablero.transform.parent = tablaMesa.transform;
        tablero.isStatic = true;
        AsignarColorMadera(tablero);
        generatedObjects.Add(tablero);
        
        Vector3[] posicionesPatas = new Vector3[]
        {
            new Vector3(-ancho/2 + 0.1f, 0, -largo/2 + 0.1f),
            new Vector3(ancho/2 - 0.1f, 0, -largo/2 + 0.1f),
            new Vector3(-ancho/2 + 0.1f, 0, largo/2 - 0.1f),
            new Vector3(ancho/2 - 0.1f, 0, largo/2 - 0.1f)
        };
        
        foreach (Vector3 pataPos in posicionesPatas)
        {
            GameObject pata = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pata.name = "Mesa_Pata";
            pata.transform.position = pos + pataPos + new Vector3(0, alto/2, 0);
            pata.transform.localScale = new Vector3(0.08f, alto, 0.08f);
            pata.transform.parent = tablaMesa.transform;
            pata.isStatic = true;
            AsignarColorMaderaOscura(pata);
            generatedObjects.Add(pata);
        }
    }
    
    void CrearEstanteria(GameObject parent, Vector3 pos)
    {
        GameObject estanteria = new GameObject("Estanteria");
        estanteria.transform.position = pos;
        estanteria.transform.parent = parent.transform;
        estanteria.isStatic = true;
        
        float ancho = 1.2f;
        float alto = 2f;
        float profundo = 0.6f;
        
        CrearTablon(estanteria, new Vector3(-ancho/2, alto/2, 0), new Vector3(0.05f, alto, profundo));
        CrearTablon(estanteria, new Vector3(ancho/2, alto/2, 0), new Vector3(0.05f, alto, profundo));
        
        for (int i = 0; i < 4; i++)
        {
            float y = i * (alto / 4);
            CrearTablon(estanteria, new Vector3(0, y, 0), new Vector3(ancho, 0.03f, profundo));
        }
        
        CrearTablon(estanteria, new Vector3(0, alto/2, -profundo/2 + 0.02f), new Vector3(ancho, alto, 0.03f));
    }
    
    void CrearTablon(GameObject parent, Vector3 pos, Vector3 escala)
    {
        GameObject tablon = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tablon.name = "Tablon";
        tablon.transform.position = parent.transform.position + pos;
        tablon.transform.localScale = escala;
        tablon.transform.parent = parent.transform;
        tablon.isStatic = true;
        AsignarColorMadera(tablon);
        generatedObjects.Add(tablon);
    }
    
    void CrearArmario(GameObject parent, Vector3 pos)
    {
        GameObject armario = new GameObject("Armario");
        armario.transform.position = pos;
        armario.transform.parent = parent.transform;
        armario.isStatic = true;
        
        float ancho = 1.5f;
        float alto = 2.2f;
        float profundo = 0.7f;
        
        GameObject cuerpo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cuerpo.name = "Armario_Cuerpo";
        cuerpo.transform.position = pos;
        cuerpo.transform.localScale = new Vector3(ancho, alto, profundo);
        cuerpo.transform.parent = armario.transform;
        cuerpo.isStatic = true;
        AsignarColorMadera(cuerpo);
        generatedObjects.Add(cuerpo);
        
        for (int i = -1; i <= 1; i += 2)
        {
            GameObject puerta = GameObject.CreatePrimitive(PrimitiveType.Cube);
            puerta.name = "Armario_Puerta";
            puerta.transform.position = pos + new Vector3(i * ancho/4, 0, profundo/2 + 0.01f);
            puerta.transform.localScale = new Vector3(ancho/2 - 0.05f, alto - 0.1f, 0.03f);
            puerta.transform.parent = armario.transform;
            puerta.isStatic = true;
            AsignarColorMaderaClara(puerta);
            generatedObjects.Add(puerta);
        }
    }
    
    void CrearSilla(GameObject parent, Vector3 pos)
    {
        GameObject silla = new GameObject("Silla");
        silla.transform.position = pos;
        silla.transform.parent = parent.transform;
        silla.isStatic = true;
        
        GameObject asiento = GameObject.CreatePrimitive(PrimitiveType.Cube);
        asiento.name = "Silla_Asiento";
        asiento.transform.position = pos + new Vector3(0, 0.5f, 0);
        asiento.transform.localScale = new Vector3(0.6f, 0.08f, 0.6f);
        asiento.transform.parent = silla.transform;
        asiento.isStatic = true;
        AsignarColorMadera(asiento);
        generatedObjects.Add(asiento);
        
        for (int i = -1; i <= 1; i += 2)
        {
            for (int j = -1; j <= 1; j += 2)
            {
                GameObject pata = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                pata.name = "Silla_Pata";
                pata.transform.position = pos + new Vector3(i * 0.25f, 0.25f, j * 0.25f);
                pata.transform.localScale = new Vector3(0.04f, 0.5f, 0.04f);
                pata.transform.parent = silla.transform;
                pata.isStatic = true;
                AsignarColorMaderaOscura(pata);
                generatedObjects.Add(pata);
            }
        }
        
        GameObject respaldo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        respaldo.name = "Silla_Respaldo";
        respaldo.transform.position = pos + new Vector3(0, 0.9f, -0.3f);
        respaldo.transform.localScale = new Vector3(0.6f, 0.5f, 0.05f);
        respaldo.transform.parent = silla.transform;
        respaldo.isStatic = true;
        AsignarColorMadera(respaldo);
        generatedObjects.Add(respaldo);
    }
    
    void CrearSofa(GameObject parent, Vector3 pos)
    {
        GameObject sofa = new GameObject("Sofa");
        sofa.transform.position = pos;
        sofa.transform.parent = parent.transform;
        sofa.isStatic = true;
        
        GameObject baseSofa = GameObject.CreatePrimitive(PrimitiveType.Cube);
        baseSofa.name = "Sofa_Base";
        baseSofa.transform.position = pos;
        baseSofa.transform.localScale = new Vector3(2.5f, 0.6f, 0.9f);
        baseSofa.transform.parent = sofa.transform;
        baseSofa.isStatic = true;
        AsignarColorTela(baseSofa, new Color(0.3f, 0.2f, 0.25f));
        generatedObjects.Add(baseSofa);
        
        GameObject respaldoSofa = GameObject.CreatePrimitive(PrimitiveType.Cube);
        respaldoSofa.name = "Sofa_Respaldo";
        respaldoSofa.transform.position = pos + new Vector3(0, 0.7f, -0.4f);
        respaldoSofa.transform.localScale = new Vector3(2.5f, 0.5f, 0.2f);
        respaldoSofa.transform.parent = sofa.transform;
        respaldoSofa.isStatic = true;
        AsignarColorTela(respaldoSofa, new Color(0.3f, 0.2f, 0.25f));
        generatedObjects.Add(respaldoSofa);
        
        for (int i = -1; i <= 1; i += 2)
        {
            GameObject brazo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            brazo.name = "Sofa_Brazo";
            brazo.transform.position = pos + new Vector3(i * 1.2f, 0.4f, 0);
            brazo.transform.localScale = new Vector3(0.15f, 0.5f, 0.9f);
            brazo.transform.parent = sofa.transform;
            brazo.isStatic = true;
            AsignarColorTela(brazo, new Color(0.3f, 0.2f, 0.25f));
            generatedObjects.Add(brazo);
        }
        
        for (int i = -1; i <= 1; i++)
        {
            GameObject cojin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cojin.name = "Sofa_Cojin";
            cojin.transform.position = pos + new Vector3(i * 0.7f, 0.4f, 0.15f);
            cojin.transform.localScale = new Vector3(0.5f, 0.15f, 0.5f);
            cojin.transform.parent = sofa.transform;
            cojin.isStatic = true;
            AsignarColorTela(cojin, new Color(0.5f, 0.3f, 0.2f));
            generatedObjects.Add(cojin);
        }
    }
    
    void CrearEscritorio(GameObject parent, Vector3 pos)
    {
        GameObject escritorio = new GameObject("Escritorio");
        escritorio.transform.position = pos;
        escritorio.transform.parent = parent.transform;
        escritorio.isStatic = true;
        
        GameObject tablero = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tablero.name = "Escritorio_Tablero";
        tablero.transform.position = pos + new Vector3(0, 0.8f, 0);
        tablero.transform.localScale = new Vector3(1.8f, 0.08f, 1f);
        tablero.transform.parent = escritorio.transform;
        tablero.isStatic = true;
        AsignarColorMadera(tablero);
        generatedObjects.Add(tablero);
        
        for (int i = -1; i <= 1; i += 2)
        {
            GameObject cajon = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cajon.name = "Escritorio_Cajon";
            cajon.transform.position = pos + new Vector3(i * 0.4f, 0.3f, 0);
            cajon.transform.localScale = new Vector3(0.3f, 0.2f, 0.5f);
            cajon.transform.parent = escritorio.transform;
            cajon.isStatic = true;
            AsignarColorMaderaClara(cajon);
            generatedObjects.Add(cajon);
        }
        
        for (int i = -1; i <= 1; i += 2)
        {
            for (int j = -1; j <= 1; j += 2)
            {
                GameObject pata = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                pata.name = "Escritorio_Pata";
                pata.transform.position = pos + new Vector3(i * 0.8f, 0.4f, j * 0.4f);
                pata.transform.localScale = new Vector3(0.05f, 0.8f, 0.05f);
                pata.transform.parent = escritorio.transform;
                pata.isStatic = true;
                AsignarColorMaderaOscura(pata);
                generatedObjects.Add(pata);
            }
        }
    }
    
    void CrearCama(GameObject parent, Vector3 pos)
    {
        GameObject cama = new GameObject("Cama");
        cama.transform.position = pos;
        cama.transform.parent = parent.transform;
        cama.isStatic = true;
        
        GameObject baseCama = GameObject.CreatePrimitive(PrimitiveType.Cube);
        baseCama.name = "Cama_Base";
        baseCama.transform.position = pos + new Vector3(0, 0.2f, 0);
        baseCama.transform.localScale = new Vector3(2.2f, 0.3f, 1.8f);
        baseCama.transform.parent = cama.transform;
        baseCama.isStatic = true;
        AsignarColorMadera(baseCama);
        generatedObjects.Add(baseCama);
        
        GameObject colchon = GameObject.CreatePrimitive(PrimitiveType.Cube);
        colchon.name = "Cama_Colchon";
        colchon.transform.position = pos + new Vector3(0, 0.5f, 0);
        colchon.transform.localScale = new Vector3(2f, 0.2f, 1.6f);
        colchon.transform.parent = cama.transform;
        colchon.isStatic = true;
        AsignarColorTela(colchon, new Color(0.8f, 0.75f, 0.7f));
        generatedObjects.Add(colchon);
        
        GameObject almohada = GameObject.CreatePrimitive(PrimitiveType.Cube);
        almohada.name = "Cama_Almohada";
        almohada.transform.position = pos + new Vector3(0.6f, 0.7f, 0);
        almohada.transform.localScale = new Vector3(0.5f, 0.15f, 0.7f);
        almohada.transform.parent = cama.transform;
        almohada.isStatic = true;
        AsignarColorTela(almohada, new Color(0.9f, 0.85f, 0.8f));
        generatedObjects.Add(almohada);
        
        GameObject sabana = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sabana.name = "Cama_Sabana";
        sabana.transform.position = pos + new Vector3(-0.2f, 0.55f, 0);
        sabana.transform.localScale = new Vector3(1.8f, 0.03f, 1.5f);
        sabana.transform.parent = cama.transform;
        sabana.isStatic = true;
        AsignarColorTela(sabana, new Color(0.6f, 0.5f, 0.7f));
        generatedObjects.Add(sabana);
    }
    
    #endregion
    
    #region Objetos Interactivos (versión resumida)
    
    void CrearObjetosInteractivos(GameObject parent)
    {
        for (int i = 0; i < 8; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-4.5f, -3.5f),
                Random.Range(0.5f, 1.8f),
                Random.Range(-3.3f, -2.7f)
            );
            CrearLibro(parent, pos, Random.Range(0.05f, 0.08f), Random.Range(0.15f, 0.2f), Random.Range(0.1f, 0.15f));
        }
        
        CrearVela(parent, new Vector3(1f, 0.6f, 0.5f));
        CrearVela(parent, new Vector3(-1f, 0.6f, 0.3f));
        CrearCajaFuerte(parent, new Vector3(4.8f, 1.2f, 0));
        CrearCuadro(parent, new Vector3(0, 2.5f, 4.8f), new Vector3(0.8f, 0.6f, 0.05f));
        CrearCuadro(parent, new Vector3(-2.5f, 2.5f, 4.8f), new Vector3(0.5f, 0.4f, 0.05f));
        CrearCuadro(parent, new Vector3(2.5f, 2.5f, 4.8f), new Vector3(0.5f, 0.4f, 0.05f));
        CrearReloj(parent, new Vector3(0, 3.2f, -4.8f));
        CrearLampara(parent, new Vector3(-2f, 0.8f, -1.5f));
        CrearCaja(parent, new Vector3(3.5f, 0.3f, -1f));
        CrearTelefono(parent, new Vector3(4f, 0.8f, 2.5f));
        CrearNota(parent, new Vector3(0.5f, 0.6f, 0.5f));
        CrearNota(parent, new Vector3(4.2f, 0.9f, 2.5f));
        CrearTecladoNumerico(parent, new Vector3(0, 1.2f, 5f));
        CrearTubosEnsayo(parent, new Vector3(-3.8f, 0.6f, -2.5f));
    }
    
    void CrearLibro(GameObject parent, Vector3 pos, float ancho, float alto, float profundo)
    {
        GameObject libro = GameObject.CreatePrimitive(PrimitiveType.Cube);
        libro.name = "Libro";
        libro.transform.position = pos;
        libro.transform.localScale = new Vector3(ancho, alto, profundo);
        libro.transform.parent = parent.transform;
        libro.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        libro.isStatic = true;
        
        Color colorLibro = new Color(
            Random.Range(0.3f, 0.8f),
            Random.Range(0.2f, 0.6f),
            Random.Range(0.2f, 0.6f)
        );
        AsignarColorTela(libro, colorLibro);
        generatedObjects.Add(libro);
    }
    
    void CrearVela(GameObject parent, Vector3 pos)
    {
        GameObject vela = new GameObject("Vela");
        vela.transform.position = pos;
        vela.transform.parent = parent.transform;
        
        GameObject cuerpo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cuerpo.name = "Vela_Cuerpo";
        cuerpo.transform.position = pos + new Vector3(0, 0.1f, 0);
        cuerpo.transform.localScale = new Vector3(0.08f, 0.2f, 0.08f);
        cuerpo.transform.parent = vela.transform;
        cuerpo.isStatic = true;
        AsignarColorTela(cuerpo, new Color(0.95f, 0.9f, 0.8f));
        generatedObjects.Add(cuerpo);
        
        if (generarLuces)
        {
            GameObject llama = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            llama.name = "Vela_Llama";
            llama.transform.position = pos + new Vector3(0, 0.25f, 0);
            llama.transform.localScale = new Vector3(0.04f, 0.08f, 0.04f);
            llama.transform.parent = vela.transform;
            llama.isStatic = true;
            AsignarColorTela(llama, new Color(1f, 0.6f, 0.1f));
            generatedObjects.Add(llama);
            
            Light luzVela = llama.AddComponent<Light>();
            luzVela.type = LightType.Point;
            luzVela.range = 1f;
            luzVela.intensity = 0.5f;
            luzVela.color = new Color(1f, 0.7f, 0.3f);
        }
    }
    
    void CrearCajaFuerte(GameObject parent, Vector3 pos)
    {
        GameObject cajaFuerte = new GameObject("CajaFuerte");
        cajaFuerte.transform.position = pos;
        cajaFuerte.transform.parent = parent.transform;
        
        GameObject cuerpo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cuerpo.name = "CajaFuerte_Cuerpo";
        cuerpo.transform.position = pos;
        cuerpo.transform.localScale = new Vector3(0.6f, 0.6f, 0.1f);
        cuerpo.transform.parent = cajaFuerte.transform;
        cuerpo.isStatic = true;
        AsignarColorTela(cuerpo, new Color(0.2f, 0.2f, 0.2f));
        generatedObjects.Add(cuerpo);
        
        GameObject perilla = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        perilla.name = "CajaFuerte_Perilla";
        perilla.transform.position = pos + new Vector3(0, 0, 0.08f);
        perilla.transform.localScale = new Vector3(0.15f, 0.03f, 0.15f);
        perilla.transform.parent = cajaFuerte.transform;
        perilla.isStatic = true;
        AsignarColorTela(perilla, new Color(0.8f, 0.7f, 0.2f));
        generatedObjects.Add(perilla);
        
        GameObject display = GameObject.CreatePrimitive(PrimitiveType.Cube);
        display.name = "CajaFuerte_Display";
        display.transform.position = pos + new Vector3(0, 0.1f, 0.08f);
        display.transform.localScale = new Vector3(0.3f, 0.08f, 0.02f);
        display.transform.parent = cajaFuerte.transform;
        display.isStatic = true;
        AsignarColorTela(display, new Color(0.1f, 0.8f, 0.1f));
        generatedObjects.Add(display);
    }
    
    void CrearCuadro(GameObject parent, Vector3 pos, Vector3 escala)
    {
        GameObject cuadro = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cuadro.name = "Cuadro";
        cuadro.transform.position = pos;
        cuadro.transform.localScale = escala;
        cuadro.transform.parent = parent.transform;
        cuadro.isStatic = true;
        AsignarColorMadera(cuadro);
        generatedObjects.Add(cuadro);
        
        GameObject lienzo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lienzo.name = "Cuadro_Lienzo";
        lienzo.transform.position = pos + new Vector3(0, 0, 0.03f);
        lienzo.transform.localScale = new Vector3(escala.x * 0.7f, escala.y * 0.7f, 0.01f);
        lienzo.transform.parent = parent.transform;
        lienzo.isStatic = true;
        
        Color colorLienzo = new Color(
            Random.Range(0.3f, 0.8f),
            Random.Range(0.2f, 0.6f),
            Random.Range(0.2f, 0.6f)
        );
        AsignarColorTela(lienzo, colorLienzo);
        generatedObjects.Add(lienzo);
    }
    
    void CrearReloj(GameObject parent, Vector3 pos)
    {
        GameObject reloj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        reloj.name = "Reloj";
        reloj.transform.position = pos;
        reloj.transform.localScale = new Vector3(0.4f, 0.05f, 0.4f);
        reloj.transform.parent = parent.transform;
        reloj.isStatic = true;
        AsignarColorTela(reloj, new Color(0.9f, 0.85f, 0.8f));
        generatedObjects.Add(reloj);
        
        for (int i = 0; i < 2; i++)
        {
            GameObject manecilla = GameObject.CreatePrimitive(PrimitiveType.Cube);
            manecilla.name = "Reloj_Manecilla" + i;
            manecilla.transform.position = pos + new Vector3(0, 0.03f, 0);
            manecilla.transform.localScale = new Vector3(0.005f, 0.01f, 0.15f + i * 0.05f);
            manecilla.transform.parent = parent.transform;
            manecilla.transform.rotation = Quaternion.Euler(0, i * 60 + 30, 0);
            manecilla.isStatic = true;
            AsignarColorTela(manecilla, new Color(0.1f, 0.1f, 0.1f));
            generatedObjects.Add(manecilla);
        }
    }
    
    void CrearLampara(GameObject parent, Vector3 pos)
    {
        GameObject lampara = new GameObject("Lampara");
        lampara.transform.position = pos;
        lampara.transform.parent = parent.transform;
        
        GameObject baseLampara = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        baseLampara.name = "Lampara_Base";
        baseLampara.transform.position = pos + new Vector3(0, 0.05f, 0);
        baseLampara.transform.localScale = new Vector3(0.2f, 0.05f, 0.2f);
        baseLampara.transform.parent = lampara.transform;
        baseLampara.isStatic = true;
        AsignarColorMadera(baseLampara);
        generatedObjects.Add(baseLampara);
        
        GameObject vastago = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        vastago.name = "Lampara_Vastago";
        vastago.transform.position = pos + new Vector3(0, 0.2f, 0);
        vastago.transform.localScale = new Vector3(0.02f, 0.3f, 0.02f);
        vastago.transform.parent = lampara.transform;
        vastago.isStatic = true;
        AsignarColorMaderaOscura(vastago);
        generatedObjects.Add(vastago);
        
        GameObject pantalla = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pantalla.name = "Lampara_Pantalla";
        pantalla.transform.position = pos + new Vector3(0, 0.4f, 0);
        pantalla.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        pantalla.transform.parent = lampara.transform;
        pantalla.isStatic = true;
        AsignarColorTela(pantalla, new Color(0.8f, 0.7f, 0.5f));
        generatedObjects.Add(pantalla);
        
        if (generarLuces)
        {
            Light luz = pantalla.AddComponent<Light>();
            luz.type = LightType.Spot;
            luz.range = 3f;
            luz.intensity = 0.3f;
            luz.color = new Color(1f, 0.8f, 0.5f);
            luz.spotAngle = 45f;
        }
    }
    
    void CrearCaja(GameObject parent, Vector3 pos)
    {
        GameObject caja = GameObject.CreatePrimitive(PrimitiveType.Cube);
        caja.name = "Caja";
        caja.transform.position = pos;
        caja.transform.localScale = new Vector3(0.4f, 0.3f, 0.4f);
        caja.transform.parent = parent.transform;
        caja.isStatic = true;
        AsignarColorMadera(caja);
        generatedObjects.Add(caja);
        
        GameObject tapa = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tapa.name = "Caja_Tapa";
        tapa.transform.position = pos + new Vector3(0, 0.18f, 0);
        tapa.transform.localScale = new Vector3(0.42f, 0.04f, 0.42f);
        tapa.transform.parent = parent.transform;
        tapa.isStatic = true;
        AsignarColorMaderaClara(tapa);
        generatedObjects.Add(tapa);
    }
    
    void CrearTelefono(GameObject parent, Vector3 pos)
    {
        GameObject telefono = new GameObject("Telefono");
        telefono.transform.position = pos;
        telefono.transform.parent = parent.transform;
        
        GameObject baseTelefono = GameObject.CreatePrimitive(PrimitiveType.Cube);
        baseTelefono.name = "Telefono_Base";
        baseTelefono.transform.position = pos;
        baseTelefono.transform.localScale = new Vector3(0.35f, 0.05f, 0.25f);
        baseTelefono.transform.parent = telefono.transform;
        baseTelefono.isStatic = true;
        AsignarColorTela(baseTelefono, new Color(0.1f, 0.1f, 0.1f));
        generatedObjects.Add(baseTelefono);
        
        GameObject auricular = GameObject.CreatePrimitive(PrimitiveType.Cube);
        auricular.name = "Telefono_Auricular";
        auricular.transform.position = pos + new Vector3(0, 0.08f, 0);
        auricular.transform.localScale = new Vector3(0.2f, 0.04f, 0.08f);
        auricular.transform.parent = telefono.transform;
        auricular.isStatic = true;
        AsignarColorTela(auricular, new Color(0.1f, 0.1f, 0.1f));
        generatedObjects.Add(auricular);
        
        GameObject disco = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        disco.name = "Telefono_Disco";
        disco.transform.position = pos + new Vector3(0, 0.06f, 0);
        disco.transform.localScale = new Vector3(0.2f, 0.01f, 0.2f);
        disco.transform.parent = telefono.transform;
        disco.isStatic = true;
        AsignarColorTela(disco, new Color(0.95f, 0.95f, 0.9f));
        generatedObjects.Add(disco);
    }
    
    void CrearNota(GameObject parent, Vector3 pos)
    {
        GameObject nota = GameObject.CreatePrimitive(PrimitiveType.Cube);
        nota.name = "Nota";
        nota.transform.position = pos;
        nota.transform.localScale = new Vector3(0.06f, 0.002f, 0.08f);
        nota.transform.parent = parent.transform;
        nota.transform.rotation = Quaternion.Euler(Random.Range(-10, 10), Random.Range(0, 360), Random.Range(-10, 10));
        nota.isStatic = true;
        AsignarColorTela(nota, new Color(0.95f, 0.9f, 0.7f));
        generatedObjects.Add(nota);
    }
    
    void CrearTecladoNumerico(GameObject parent, Vector3 pos)
    {
        GameObject teclado = new GameObject("TecladoNumerico");
        teclado.transform.position = pos;
        teclado.transform.parent = parent.transform;
        
        GameObject baseTeclado = GameObject.CreatePrimitive(PrimitiveType.Cube);
        baseTeclado.name = "Teclado_Base";
        baseTeclado.transform.position = pos;
        baseTeclado.transform.localScale = new Vector3(0.3f, 0.03f, 0.2f);
        baseTeclado.transform.parent = teclado.transform;
        baseTeclado.isStatic = true;
        AsignarColorTela(baseTeclado, new Color(0.2f, 0.2f, 0.2f));
        generatedObjects.Add(baseTeclado);
        
        for (int i = 0; i < 9; i++)
        {
            GameObject boton = GameObject.CreatePrimitive(PrimitiveType.Cube);
            boton.name = "Teclado_Boton" + (i + 1);
            float x = (i % 3 - 1) * 0.08f;
            float z = (i / 3 - 1) * 0.06f;
            boton.transform.position = pos + new Vector3(x, 0.02f, z);
            boton.transform.localScale = new Vector3(0.04f, 0.01f, 0.04f);
            boton.transform.parent = teclado.transform;
            boton.isStatic = true;
            AsignarColorTela(boton, new Color(0.8f, 0.7f, 0.6f));
            generatedObjects.Add(boton);
        }
    }
    
    void CrearTubosEnsayo(GameObject parent, Vector3 pos)
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject tubo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tubo.name = "TuboEnsayo" + i;
            tubo.transform.position = pos + new Vector3(i * 0.08f - 0.08f, 0.05f, 0);
            tubo.transform.localScale = new Vector3(0.02f, 0.1f, 0.02f);
            tubo.transform.parent = parent.transform;
            tubo.isStatic = true;
            
            Color colorTubo = new Color(
                0.5f + Random.Range(0, 0.5f),
                0.3f + Random.Range(0, 0.3f),
                0.8f + Random.Range(0, 0.2f)
            );
            AsignarColorTela(tubo, colorTubo);
            
            Material mat = tubo.GetComponent<Renderer>().material;
            mat.SetFloat("_Mode", 2);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.color = new Color(colorTubo.r, colorTubo.g, colorTubo.b, 0.6f);
            
            generatedObjects.Add(tubo);
        }
    }
    
    #endregion
    
    #region Iluminación
    
    void CrearIluminacion(GameObject parent)
    {
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.3f, 0.25f, 0.2f);
        
        GameObject luzPrincipal = new GameObject("LuzPrincipal");
        luzPrincipal.transform.position = new Vector3(0, roomHeight - 0.3f, 0);
        luzPrincipal.transform.parent = parent.transform;
        Light light = luzPrincipal.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = 10f;
        light.intensity = 1.5f;
        light.color = new Color(1f, 0.9f, 0.7f);
        light.shadowStrength = 0.8f;
        
        GameObject foco = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        foco.name = "Foco";
        foco.transform.position = new Vector3(0, roomHeight - 0.1f, 0);
        foco.transform.localScale = new Vector3(0.3f, 0.1f, 0.3f);
        foco.transform.parent = parent.transform;
        foco.isStatic = true;
        AsignarColorTela(foco, new Color(0.95f, 0.9f, 0.8f));
        generatedObjects.Add(foco);
        
        GameObject luzSecundaria = new GameObject("LuzSecundaria");
        luzSecundaria.transform.position = new Vector3(-4f, 3f, 4.5f);
        luzSecundaria.transform.parent = parent.transform;
        Light light2 = luzSecundaria.AddComponent<Light>();
        light2.type = LightType.Spot;
        light2.range = 5f;
        light2.intensity = 0.5f;
        light2.color = new Color(0.8f, 0.6f, 0.4f);
        light2.spotAngle = 40f;
        
        Light sun = new GameObject("Sun").AddComponent<Light>();
        sun.transform.parent = parent.transform;
        sun.type = LightType.Directional;
        sun.intensity = 0.3f;
        sun.color = new Color(0.5f, 0.4f, 0.3f);
        sun.transform.rotation = Quaternion.Euler(50, -30, 0);
    }
    
    #endregion
    
    #region Decoración
    
    void CrearDecoracion(GameObject parent)
    {
        GameObject alfombra = GameObject.CreatePrimitive(PrimitiveType.Cube);
        alfombra.name = "Alfombra";
        alfombra.transform.position = new Vector3(0, 0.01f, 0);
        alfombra.transform.localScale = new Vector3(3f, 0.02f, 2.5f);
        alfombra.transform.parent = parent.transform;
        alfombra.isStatic = true;
        AsignarColorTela(alfombra, new Color(0.6f, 0.2f, 0.3f));
        generatedObjects.Add(alfombra);
        
        for (int i = -1; i <= 1; i += 2)
        {
            GameObject cortina = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cortina.name = "Cortina" + (i == -1 ? "Izquierda" : "Derecha");
            cortina.transform.position = new Vector3(i * 4.5f, 2f, 4.8f);
            cortina.transform.localScale = new Vector3(1f, 2.5f, 0.05f);
            cortina.transform.parent = parent.transform;
            cortina.isStatic = true;
            AsignarColorTela(cortina, new Color(0.4f, 0.2f, 0.3f));
            generatedObjects.Add(cortina);
        }
        
        CrearPlanta(parent, new Vector3(4.2f, 0.3f, 3.8f));
        
        for (int i = 0; i < 3; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(1.5f, 2.5f),
                0.05f,
                Random.Range(-2f, -1f)
            );
            CrearLibro(parent, pos, Random.Range(0.05f, 0.08f), Random.Range(0.03f, 0.05f), Random.Range(0.06f, 0.09f));
        }
    }
    
    void CrearPlanta(GameObject parent, Vector3 pos)
    {
        GameObject planta = new GameObject("Planta");
        planta.transform.position = pos;
        planta.transform.parent = parent.transform;
        
        GameObject maceta = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        maceta.name = "Planta_Maceta";
        maceta.transform.position = pos + new Vector3(0, 0.1f, 0);
        maceta.transform.localScale = new Vector3(0.2f, 0.15f, 0.2f);
        maceta.transform.parent = planta.transform;
        maceta.isStatic = true;
        AsignarColorTela(maceta, new Color(0.6f, 0.3f, 0.1f));
        generatedObjects.Add(maceta);
        
        GameObject tallo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        tallo.name = "Planta_Tallo";
        tallo.transform.position = pos + new Vector3(0, 0.25f, 0);
        tallo.transform.localScale = new Vector3(0.02f, 0.25f, 0.02f);
        tallo.transform.parent = planta.transform;
        tallo.isStatic = true;
        AsignarColorTela(tallo, new Color(0.1f, 0.6f, 0.1f));
        generatedObjects.Add(tallo);
        
        for (int i = 0; i < 5; i++)
        {
            GameObject hoja = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hoja.name = "Planta_Hoja" + i;
            float angulo = i * 72f;
            float radio = Random.Range(0.08f, 0.15f);
            hoja.transform.position = pos + new Vector3(
                Mathf.Cos(angulo * Mathf.Deg2Rad) * radio,
                Random.Range(0.2f, 0.4f),
                Mathf.Sin(angulo * Mathf.Deg2Rad) * radio
            );
            hoja.transform.localScale = new Vector3(0.08f, 0.05f, 0.08f);
            hoja.transform.parent = planta.transform;
            hoja.transform.rotation = Quaternion.Euler(Random.Range(-30, 30), Random.Range(0, 360), Random.Range(-30, 30));
            hoja.isStatic = true;
            AsignarColorTela(hoja, new Color(Random.Range(0.1f, 0.3f), Random.Range(0.5f, 0.8f), Random.Range(0.1f, 0.3f)));
            generatedObjects.Add(hoja);
        }
    }
    
    #endregion
    
    #region Pistas Visuales
    
    void CrearPistasVisuales(GameObject parent)
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject numero = GameObject.CreatePrimitive(PrimitiveType.Cube);
            numero.name = "NumeroPista" + (i + 1);
            numero.transform.position = new Vector3(-3.5f + i * 0.3f, 0.8f, 4.8f);
            numero.transform.localScale = new Vector3(0.05f, 0.05f, 0.01f);
            numero.transform.parent = parent.transform;
            numero.isStatic = true;
            AsignarColorTela(numero, new Color(0.8f, 0.2f, 0.1f));
            generatedObjects.Add(numero);
        }
        
        GameObject flecha = GameObject.CreatePrimitive(PrimitiveType.Cube);
        flecha.name = "FlechaPista";
        flecha.transform.position = new Vector3(2.5f, 0.01f, -2.5f);
        flecha.transform.localScale = new Vector3(0.3f, 0.01f, 0.1f);
        flecha.transform.parent = parent.transform;
        flecha.transform.rotation = Quaternion.Euler(0, 45, 0);
        flecha.isStatic = true;
        AsignarColorTela(flecha, new Color(0.8f, 0.1f, 0.1f));
        generatedObjects.Add(flecha);
    }
    
    #endregion
    
    #region Helpers de Materiales
    
    void AsignarColorMadera(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));
        float variacion = Random.Range(-0.1f, 0.1f);
        mat.color = new Color(0.5f + variacion, 0.35f + variacion * 0.5f, 0.2f + variacion * 0.3f);
        renderer.material = mat;
    }
    
    void AsignarColorMaderaOscura(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));
        float variacion = Random.Range(-0.05f, 0.05f);
        mat.color = new Color(0.3f + variacion, 0.2f + variacion * 0.5f, 0.1f + variacion * 0.3f);
        renderer.material = mat;
    }
    
    void AsignarColorMaderaClara(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));
        float variacion = Random.Range(-0.05f, 0.05f);
        mat.color = new Color(0.7f + variacion, 0.55f + variacion * 0.5f, 0.35f + variacion * 0.3f);
        renderer.material = mat;
    }
    
    void AsignarColorTela(GameObject obj, Color color)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = color;
        renderer.material = mat;
    }
    
    #endregion
}