using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public Image progressBar;
    private static string _nextSceneName = "GameScene";

    void Start()
    {
        Time.timeScale = 1f;
        progressBar.fillAmount = 0;
        
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(_nextSceneName);
        op.allowSceneActivation = false;
        yield return new WaitForSeconds(0.5f);  // 防止进入加载界面直接开始加载造成观感不佳
        
        // 加载太快了，手动使其缓慢过渡（好像也没有多慢）
        float time = 0;
        while (progressBar.fillAmount < 1f || op.progress < 0.9f)
        {
            time += Time.deltaTime;
            progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, time);
            Debug.Log("场景加载中... " + progressBar.fillAmount);
            yield return null;
        }
        
        progressBar.fillAmount = 1f;
        yield return new WaitForSeconds(0.5f);  // 防止退出加载界面直接退出加载造成观感不佳
        op.allowSceneActivation = true;
    }

    public static void LoadScene(string sceneName)
    {
        _nextSceneName = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }
}
