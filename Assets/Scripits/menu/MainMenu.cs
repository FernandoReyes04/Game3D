using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenu : MonoBehaviour
{
    // --- Paneles ---
    public GameObject mainPanel; 
    public GameObject controlsPanel; 
    public GameObject creditsPanel; 


    // --- Funciones del Menú Principal ---

    public void StartGame()
    {
        // REANUDAMOS el juego
        Time.timeScale = 1f; 
        Debug.Log("Reanudando el juego y cargando escena...");
        
        // Reemplaza "NombreDeTuEscenaDeJuego" con el nombre real de tu escena
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        Debug.Log("Cerrando el juego...");
        Application.Quit();
    }

    // --- Funciones de Navegación de Paneles ---

    public void ShowControlsPanel()
    {
        mainPanel.SetActive(false);
        controlsPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    public void ShowCreditsPanel()
    {
        mainPanel.SetActive(false);
        controlsPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    // Botón "Atrás"
    public void ShowMainPanel()
    {
        mainPanel.SetActive(true);
        controlsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }
}