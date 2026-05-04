using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private Image _canvasPanel;

    private void Awake()
    {
        _canvasPanel = GetComponent<Image>();
    }
    public void StartGame()
    {
        StartCoroutine(TransitionFade(1f, "GameScene")); // Adjust the fade duration as needed
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private IEnumerator TransitionFade(float fadeDuration, string sceneName)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            _canvasPanel.color = new Color(_canvasPanel.color.r, _canvasPanel.color.g, _canvasPanel.color.b, alpha);
            yield return null;
        }

        _canvasPanel.color = new Color(_canvasPanel.color.r, _canvasPanel.color.g, _canvasPanel.color.b, 1f);

        SceneManager.LoadScene(sceneName);
    }
}
