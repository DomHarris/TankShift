using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Image mainImage;
    [SerializeField] private CanvasGroup titleScreen;
    [SerializeField] private CanvasGroup wholeScreen;
    [SerializeField] private TextMeshProUGUI nextText;
    [SerializeField] private Color nextErrorColor;
    [SerializeField] private GameObject player;
    [SerializeField] private MonoBehaviour[] playerComponentsToDisable;
    [SerializeField] private RectTransform textBox;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private int lettersToRevealPerTick = 1;
    [SerializeField] private int ticksPerReveal = 3;
    [SerializeField] private GameObject glow;
    [SerializeField] private IntroCrawl.TextPage[] speech;
    
    private readonly string[] NextButtonPrompts = 
    {
        "Press 'any' key to continue",
        "No, not that one",
        "Try again",
        "We're not very good at this, are we?",
        "Fine, I'll let you in anyway"
    };
    
    private bool _keyPressed = false;
    private int _currentPrompt;
    
    private const string path = "hasPlayedBefore";
    
    // Start is called before the first frame update
    private IEnumerator Start()
    {
        var skip = File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + path);
        mainImage.DOColor(Color.black, 2f);
        titleScreen.DOFade(1, 0.5f);
        yield return new WaitForSeconds(2f);
        _keyPressed = false;
        
        var color = nextText.color;

        foreach (var prompt in NextButtonPrompts)
        {
            nextText.text = prompt;

            while (!_keyPressed && !prompt.Contains("Fine"))
            {
                yield return null;
            }

            _keyPressed = false;

            if (skip)
                break;
            
            nextText.DOColor(nextErrorColor, 0.25f);
            nextText.DOColor(color, 0.25f).SetDelay(3f);
            nextText.transform.DOShakePosition(0.25f, 10f, 3);
        }


        File.WriteAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + path, "tutorial=true");

        player.SetActive(true);
        foreach (var behaviour in playerComponentsToDisable)
            behaviour.enabled = false;

        
        titleScreen.DOFade(0, 0.5f).SetDelay(skip ? 0f : 2f);
        wholeScreen.DOFade(0, 0.5f).SetDelay(skip ? 0.25f : 2.25f);
        wholeScreen.blocksRaycasts = false;

        if (!skip)
            yield return new WaitForSeconds(3f);

        textBox.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.5f);
        
        foreach (var page in speech)
        {
            var counter = 0;
            var currentPos = 0;

            foreach (var breakpoint in page.BreakPoints)
            {
                text.text = page.Body;
                text.color = page.Color;
                // while there are still characters to reveal, and no key was pressed
                while (counter < breakpoint + currentPos && !_keyPressed)
                {
                    // set the maximum amount of characters to see
                    text.maxVisibleCharacters = counter;
                    counter += lettersToRevealPerTick;

                    for (int i = 0; i < ticksPerReveal; ++i)
                    {
                        // wait a frame
                        yield return null;
                    }
                }

                _keyPressed = false;
                // show all characters
                text.maxVisibleCharacters = breakpoint + currentPos;
                currentPos = breakpoint + currentPos;
                counter = currentPos;

                while (!_keyPressed)
                    yield return null;
                _keyPressed = false;
            }
        }
        glow.SetActive(false);
        textBox.DOAnchorPosY(-textBox.rect.height, 0.5f).SetEase(Ease.InBack);
        foreach (var behaviour in playerComponentsToDisable)
            behaviour.enabled = true;
    }

    public void AnyKeyPressed(InputAction.CallbackContext ctx)
    {
        if (!ctx.ReadValueAsButton()) return;
        _keyPressed = true;
    }
}
