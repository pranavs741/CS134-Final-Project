using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeartLivesUI : MonoBehaviour
{
    [Header("Image-based hearts (one Image per life)")]
    [SerializeField] private Image[] hearts;
    [SerializeField] private Color filledColor = new Color(1f, 0.2f, 0.3f, 1f);
    [SerializeField] private Color emptyColor = new Color(0.25f, 0.25f, 0.25f, 0.6f);
    [Tooltip("If true, lost hearts are hidden entirely. If false, they fade to the empty color.")]
    [SerializeField] private bool hideLostHearts = false;

    [Header("Text fallback (use this OR images, not both)")]
    [SerializeField] private TMP_Text heartsText;
    [SerializeField] private string heartCharacter = "\u2665";
    [SerializeField] private string heartSeparator = " ";

    public void SetLives(int current, int max)
    {
        if (hearts != null && hearts.Length > 0)
        {
            for (int i = 0; i < hearts.Length; i++)
            {
                if (hearts[i] == null) continue;
                bool filled = i < current;

                if (hideLostHearts)
                {
                    hearts[i].gameObject.SetActive(filled);
                }
                else
                {
                    hearts[i].gameObject.SetActive(true);
                    hearts[i].color = filled ? filledColor : emptyColor;
                }
            }
        }

        if (heartsText != null)
        {
            string built = "";
            for (int i = 0; i < current; i++)
            {
                if (i > 0) built += heartSeparator;
                built += heartCharacter;
            }
            heartsText.text = built;
        }
    }
}
