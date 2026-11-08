using UnityEngine;
using UnityEngine.SceneManagement;

namespace Golf.ScriptsGolf.Buttons
{
    public class MenuButton : MonoBehaviour
    {
        [SerializeField] private string sceneName = "GameScene";
        [SerializeField] private string menuSceneName = "Menu";

        private string scene;

        private void Update()
        {
            scene = SceneManager.GetActiveScene().name;
        }

        public void ContnueBut()
        {
            /*menuImg.gameObject.SetActive(false);
            menuButton1.gameObject.SetActive(false);
            menuButton2.gameObject.SetActive(false);
            menuButton3.gameObject.SetActive(false);

            checkMenu = false;
            */

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void ToMenu()
        {
            /*
            menuImg.gameObject.SetActive(false);
            menuButton1.gameObject.SetActive(false);
            menuButton2.gameObject.SetActive(false); 
            menuButton3.gameObject.SetActive(false);
            */

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SceneManager.LoadScene(menuSceneName);
        }

        public void LoadScene()
        {
            SceneManager.LoadScene(sceneName);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void ExitGame()
        {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
                Application.Quit();
    #endif
        }
        /*
        public void OpenMenu()
        {
            if ((scene != "MainMenu") && (!checkMenu) && (Input.GetKeyDown(KeyCode.Escape)))
            {
                menuImg.gameObject.SetActive(true);
                menuButton1.gameObject.SetActive(true);
                menuButton2.gameObject.SetActive(true);
                menuButton3.gameObject.SetActive(true);

                menuButton1.interactable = true;
                menuButton2.interactable = true;
                menuButton3.interactable = true;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                checkMenu = true;
            }
            else if ((checkMenu) && (Input.GetKeyDown(KeyCode.Escape)))
            {
                ContnueBut();
            }
        }
        */
    }
}
