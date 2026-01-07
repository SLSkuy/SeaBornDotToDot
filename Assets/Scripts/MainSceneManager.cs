using UnityEngine;

public class MainSceneManager : MonoBehaviour
{
    public void UI_StartGameButton()
    {
        SceneLoader.LoadScene("GameScene");
    }
}
