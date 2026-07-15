using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public string escenaJuego = "Juego";
    public string escenaMenu = "MenuPrincipal";
    
    private bool pausado;
    private bool enJuego;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start() => PausarJuego();

    // metodos con nombres originales para compatibilidad
    public void IniciarJuego()
    {
        enJuego = true;
        ReanudarJuego();
        SceneManager.LoadScene(escenaJuego);
    }

    public void CargarPartida()
    {
        enJuego = true;
        ReanudarJuego();
        SceneManager.LoadScene(escenaJuego);
    }

    public void PausarJuego()
    {
        pausado = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ReanudarJuego()
    {
        pausado = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void TogglePausa()
    {
        if (!enJuego) return;
        if (pausado) ReanudarJuego();
        else PausarJuego();
    }

    public bool EstaPausado() => pausado;

    public void SalirJuego()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void VolverAlMenu()
    {
        enJuego = false;
        ReanudarJuego();
        SceneManager.LoadScene(escenaMenu);
    }
}