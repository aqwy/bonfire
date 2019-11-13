using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class WinReward : MonoBehaviour
{
    public Image[] starImgs;

    private Color _normalColor;
    private Color _disabledColor;
    private Color _endColor;

    private void Awake()
    {
        _normalColor = Color.white;
        _disabledColor = new Color(1, 1, 1, 0);
        _endColor = new Color(255f, 221f, 9f, 255f);
        resetAllStars();
    }

    public void resetAllStars()
    {
        for (int i = 0; i < starImgs.Length; i++)
        {
            if (starImgs[i] != null)
            {
                starImgs[i].enabled = false;
                starImgs[i].color = _disabledColor;
            }
        }
    }

    public void playStarFest()
    {
        StartCoroutine(activisionStars());
    }

    private IEnumerator activisionStars()
    {
        
        foreach (Image star in starImgs)
        {
            star.enabled = true;

            star.DOFade(1f, 2.5f).SetLoops(1).SetEase(Ease.Linear);
            star.rectTransform.DOSizeDelta(new Vector2(57f, 57f), 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);           
            star.DOColor(Color.yellow, 2f).SetLoops(-1,LoopType.Yoyo).SetEase(Ease.Linear).SetDelay(2f);

            yield return new WaitForSeconds(0.3f);
        }
    }
}
