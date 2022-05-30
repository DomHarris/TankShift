using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace UI
{
    public class IntroCrawl : MonoBehaviour
    {
        [Serializable]
        public struct TextPage
        {
            public Color Color; 
            public string Body;
            public List<int> BreakPoints;
        }

        [SerializeField] private TextPage[] pages;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private int lettersToRevealPerTick = 1;
        [SerializeField] private int ticksPerReveal = 3;
        [SerializeField] private CanvasGroup canvas;
        [SerializeField] private float timeToCanvasFade = 3f;
        [SerializeField] private GameObject mcText;
        [SerializeField] private Transform[] clouds;
        [SerializeField] private GameObject cloudCam;
        [SerializeField] private GameObject tank;
        [SerializeField] private Vector3 tankNewPos;
        [SerializeField] private CanvasGroup fadeScreen;

        private const string path = "hasPlayedBefore";
        
        
        private bool _keyPress = false;

        private IEnumerator Start()
        {
            if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + path))
            {
                SceneManager.LoadScene("Level 1");
                yield break;
            }

            foreach (var page in pages)
            {
                var counter = 0;
                var currentPos = 0;

                foreach (var breakpoint in page.BreakPoints)
                {
                    text.text = page.Body;
                    // while there are still characters to reveal, and no key was pressed
                    while (counter < breakpoint + currentPos && !_keyPress)
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

                    _keyPress = false;
                    // show all characters
                    text.maxVisibleCharacters = breakpoint + currentPos;
                    currentPos = breakpoint + currentPos;
                    counter = currentPos;
                    while (!_keyPress)
                        yield return null;
                    _keyPress = false;
                }
            }

            foreach (var cloud in clouds)
                cloud.DOMoveX(cloud.position.x - 2f, Random.Range(10f, 15f));

            canvas.DOFade(0, timeToCanvasFade);
            for (int i = 0; i < 5; ++i)
            {
                mcText.SetActive(false);
                yield return new WaitForSeconds(0.25f);
                mcText.SetActive(true);
                yield return new WaitForSeconds(0.25f);
            }

            cloudCam.SetActive(true);
            tank.transform.DOMove(tankNewPos, 2.5f).SetDelay(1.5f).SetEase(Ease.Linear); 
            fadeScreen.DOFade(1, 0.3f).SetDelay(3.5f).OnComplete(() =>
            {
                SceneManager.LoadScene("Level 1");
            });
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(tankNewPos, 0.5f);
        }

        public void ButtonPressed(InputAction.CallbackContext ctx)
        {
            if (!ctx.ReadValueAsButton()) return;
            _keyPress = true;   
        }
    }
}