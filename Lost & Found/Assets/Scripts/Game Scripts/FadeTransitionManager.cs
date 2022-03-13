using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using System;

public class FadeTransitionManager : MonoBehaviour
{
    public static FadeTransitionManager instance;

    [SerializeField]
    private GameObject fadeCanvas;
    [SerializeField]
    private TMP_Text textField;
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private float fadeInTime = 3f;
    [SerializeField]
    private float fadeHoldTime = 1.5f;
    [SerializeField]
    private float fadeOutTime = 3f;
    //public delegate void Unknown();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(fadeCanvas);
        }
        else
        {
            Destroy(gameObject);
            Destroy(fadeCanvas);
        }
    }

    public void SetTransitionText(string _toDisplay = "")
    {
        textField.text = _toDisplay;
    }

    public async Task StartTransition()
    {
        fadeCanvas.SetActive(true);
        animator.Play("Fade In");

        await Task.Delay(TimeSpan.FromSeconds(fadeInTime));
        PostFadeIn();
        return;
    }

    private void PostFadeIn()
    {
        StartCoroutine(nameof(FadeOut));
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(fadeHoldTime);

        animator.Play("Fade Out");

        yield return new WaitForSeconds(fadeOutTime);
        fadeCanvas.SetActive(false);

        textField.text = "";
    }
}
