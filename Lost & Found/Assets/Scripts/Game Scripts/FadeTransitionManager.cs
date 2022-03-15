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
    private float fadeInTime = 1f;
    [SerializeField]
    private float fadeHoldTime = 0.25f;
    [SerializeField]
    [Tooltip("Used when text is displayed during a transition")]
    private float fadeTextHoldTime = 1.25f;
    [SerializeField]
    [Tooltip("Used when text is not displayed during a transition")]
    private float fadeOutTime = 1f;
    [SerializeField]
    [Range(0, 1)]
    [Tooltip("How much the screen has to fade in before the player is given back control after a scene transition")]
    private float percentFadeForMovement = 0.75f;
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
        StopAllCoroutines();
        animator.StopPlayback();
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
        //If there is text, the fade should hold longer
        //else, don't impede flow with a super long fade
        if(textField.text != "")
        {
            yield return new WaitForSeconds(fadeTextHoldTime);
        }
        else
        {
            yield return new WaitForSeconds(fadeHoldTime);
        }

        animator.Play("Fade Out");

        yield return new WaitForSeconds(fadeOutTime * percentFadeForMovement);
        GameManager.instance.EnablePlayerInput();
        yield return new WaitForSeconds(fadeOutTime * (1 - percentFadeForMovement));
        fadeCanvas.SetActive(false);

        textField.text = "";
    }
}
