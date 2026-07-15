using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("paneles")]
    public GameObject menuPanel;
    public GameObject configPanel;
    public GameObject confirmPanel;
    
    [Header("sonidos")]
    public AudioClip hoverSound;
    public AudioClip clickSound;
    
    private AudioSource audioSource;
    private GameManager gameManager;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        gameManager = FindObjectOfType<GameManager>();
        
        if (gameManager == null)
        {
            Debug.LogError("game manager no encontrado");
            return;
        }

        OcultarPaneles();
        menuPanel?.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gameManager != null)
        {
            if (gameManager.EstaPausado())
            {
                gameManager.ReanudarJuego();
                CerrarMenu();
            }
            else
            {
                gameManager.PausarJuego();
                AbrirMenu();
            }
        }
    }

    void OcultarPaneles()
    {
        menuPanel?.SetActive(false);
        configPanel?.SetActive(false);
        confirmPanel?.SetActive(false);
    }

    void MostrarPanel(GameObject panel)
    {
        OcultarPaneles();
        panel?.SetActive(true);
    }

    void AbrirMenu()
    {
        MostrarPanel(menuPanel);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CerrarMenu()
    {
        OcultarPaneles();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // acciones de botones
    public void NuevaPartida()
    {
        SonidoClick();
        gameManager?.IniciarJuego();
        CerrarMenu();
    }

    public void CargarPartida()
    {
        SonidoClick();
        gameManager?.CargarPartida();
        CerrarMenu();
    }

    public void AbrirConfiguracion()
    {
        SonidoClick();
        MostrarPanel(configPanel);
    }

    public void CerrarConfiguracion()
    {
        SonidoClick();
        MostrarPanel(menuPanel);
    }

    public void AbrirConfirmacionSalir()
    {
        SonidoClick();
        MostrarPanel(confirmPanel);
    }

    public void CerrarConfirmacion()
    {
        SonidoClick();
        MostrarPanel(menuPanel);
    }

    public void SalirJuego()
    {
        SonidoClick();
        gameManager?.SalirJuego();
    }

    public void ReanudarJuego()
    {
        SonidoClick();
        gameManager?.ReanudarJuego();
        CerrarMenu();
    }

    // sonidos
    public void PlayHoverSound()
    {
        if (hoverSound != null && audioSource != null)
            audioSource.PlayOneShot(hoverSound, 0.5f);
    }

    void SonidoClick()
    {
        if (clickSound != null && audioSource != null)
            audioSource.PlayOneShot(clickSound, 0.7f);
    }
}