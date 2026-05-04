using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool _shouldFadeIn = true;
    [SerializeField] private Image _fadeCanvasImage;
    private void Awake()
    {
        if(_shouldFadeIn)
        {
            StartCoroutine(FadeInScene(1f));
        }
        else
        {
            DialogueManager.Instance.StartDialogue();
        }
    }
    private void Update()
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

    private IEnumerator FadeInScene(float fadeDuration)
    {
        float elapsedTime = 0f;

        _fadeCanvasImage.color = new Color(_fadeCanvasImage.color.r, _fadeCanvasImage.color.g, _fadeCanvasImage.color.b, 1f);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
            _fadeCanvasImage.color = new Color(_fadeCanvasImage.color.r, _fadeCanvasImage.color.g, _fadeCanvasImage.color.b, alpha);
            yield return null;
        }

        _fadeCanvasImage.color = new Color(_fadeCanvasImage.color.r, _fadeCanvasImage.color.g, _fadeCanvasImage.color.b, 0f);

        yield return new WaitForSeconds(1f);

        DialogueManager.Instance.StartDialogue();
    }
}
