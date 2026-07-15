using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject menuPanel;
    public GameObject configPanel;
    public GameObject confirmPanel;
    
    [Header("Sonidos")]
    public AudioClip hoverSound;
    public AudioClip clickSound;
    
    private AudioSource audioSource;
    private bool menuAbierto = false;
    private GameManager gameManager;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        
        // Buscar GameManager
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager no encontrado en la escena. Crea un GameObject con GameManager.cs");
            return;
        }

        // Ocultar todos los paneles al inicio
        OcultarTodosLosPaneles();
        
        // Mostrar menú principal
        menuAbierto = true;
        menuPanel.SetActive(true);
        
        // Asegurar cursor visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        // ESC solo funciona si el juego ya empezó
        if (Input.GetKeyDown(KeyCode.Escape) && gameManager != null)
        {
            if (gameManager.EstaPausado())
            {
                // Si está pausado, reanudar
                CerrarMenu();
                gameManager.ReanudarJuego();
            }
            else
            {
                // Si no está pausado, pausar y abrir menú
                gameManager.PausarJuego();
                AbrirMenu();
            }
        }
    }

    void OcultarTodosLosPaneles()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (configPanel != null) configPanel.SetActive(false);
        if (confirmPanel != null) confirmPanel.SetActive(false);
    }

    void MostrarPanel(GameObject panel)
    {
        OcultarTodosLosPaneles();
        if (panel != null) panel.SetActive(true);
    }

    void AbrirMenu()
    {
        menuAbierto = true;
        MostrarPanel(menuPanel);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CerrarMenu()
    {
        menuAbierto = false;
        OcultarTodosLosPaneles();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    #region Acciones de Botones

    public void NuevaPartida()
    {
        PlayClickSound();
        if (gameManager != null)
        {
            gameManager.IniciarJuego();
        }
        CerrarMenu();
    }

    public void CargarPartida()
    {
        PlayClickSound();
        if (gameManager != null)
        {
            gameManager.CargarPartida();
        }
        CerrarMenu();
    }

    public void AbrirConfiguracion()
    {
        PlayClickSound();
        MostrarPanel(configPanel);
    }

    public void CerrarConfiguracion()
    {
        PlayClickSound();
        MostrarPanel(menuPanel);
    }

    public void AbrirConfirmacionSalir()
    {
        PlayClickSound();
        MostrarPanel(confirmPanel);
    }

    public void CerrarConfirmacion()
    {
        PlayClickSound();
        MostrarPanel(menuPanel);
    }

    public void SalirJuego()
    {
        PlayClickSound();
        if (gameManager != null)
        {
            gameManager.SalirJuego();
        }
    }

    public void ReanudarJuego()
    {
        PlayClickSound();
        if (gameManager != null)
        {
            gameManager.ReanudarJuego();
        }
        CerrarMenu();
    }

    #endregion

    #region Sonidos

    public void PlayHoverSound()
    {
        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound, 0.5f);
        }
    }

    void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound, 0.7f);
        }
    }

    #endregion
}