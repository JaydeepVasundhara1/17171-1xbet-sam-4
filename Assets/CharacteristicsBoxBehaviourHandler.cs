using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharacteristicsBoxBehaviourHandler : MonoBehaviour
{
    public float RegularSize;
    public float ExpandSize;
    public Image ExpandButton;

    public Sprite ExpandSprite;
    public Sprite ShrinkSprite;

    public GameObject ExpandView;
    public GameObject ShrinkView;

    
    private void OnEnable()
    {
        ShrinkView.SetActive(true);
        ExpandView.SetActive(false);

        Vector2 currentSize = GetComponent<RectTransform>().sizeDelta;

        GetComponent<RectTransform>().sizeDelta = new Vector2(currentSize.x, RegularSize);

        ExpandButton.sprite = ExpandSprite;
    }


    public void ExpandableButton()
    {
        if (ExpandButton.sprite == ExpandSprite)
        {
            ShrinkView.SetActive(false);
            ExpandView.SetActive(true);

            // Get the current sizeDelta
            Vector2 currentSize = GetComponent<RectTransform>().sizeDelta;

            // Create a new sizeDelta with the new height
            Vector2 newSize = new Vector2(currentSize.x, ExpandSize);

            // Animate the sizeDelta change using DOTween
            GetComponent<RectTransform>().DOSizeDelta(newSize, 0.5f).OnComplete(() =>
            {
                ExpandButton.sprite = ShrinkSprite;
            });
        }
        else
        {
            ShrinkView.SetActive(true);
            ExpandView.SetActive(false);

            // Get the current sizeDelta
            Vector2 currentSize = GetComponent<RectTransform>().sizeDelta;

            // Create a new sizeDelta with the new height
            Vector2 newSize = new Vector2(currentSize.x, RegularSize);

            // Animate the sizeDelta change using DOTween
            GetComponent<RectTransform>().DOSizeDelta(newSize, 0.5f).OnComplete(() =>
            {
                ExpandButton.sprite = ExpandSprite;
            });
        }
    }
}
