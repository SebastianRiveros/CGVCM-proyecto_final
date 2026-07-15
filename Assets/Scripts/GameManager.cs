using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Escenas")]
    public string nombreEscenaJuego = "Juego";
    public string nombreEscenaMenu = "MenuPrincipal";

    private bool isPaused = false;
    private bool gameStarted = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PausarJuego();
    }

    public void IniciarJuego()
    {
        gameStarted = true;
        ReanudarJuego();
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    public void CargarPartida()
    {
        gameStarted = true;
        ReanudarJuego();
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    public void PausarJuego()
    {
        isPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // 🔥 IMPORTANTE: El EventSystem NO se pausa, la UI sigue funcionando
        // porque usamos Time.unscaledDeltaTime en los scripts si es necesario
    }

    public void ReanudarJuego()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void TogglePausa()
    {
        if (gameStarted)
        {
            if (isPaused)
                ReanudarJuego();
            else
                PausarJuego();
        }
    }

    public bool EstaPausado()
    {
        return isPaused;
    }

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
        gameStarted = false;
        ReanudarJuego();
        SceneManager.LoadScene(nombreEscenaMenu);
    }
}