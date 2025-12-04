using DG.Tweening;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public void on_hover_enter()
    {
        transform.DOScale(1.1f, 0.3f)
            .SetEase(Ease.OutCubic)
            .SetLink(gameObject);
    }

    public void on_hover_exit()
    {
        transform.DOScale(1.0f, 0.3f)
            .SetEase(Ease.OutCubic)
            .SetLink(gameObject);
    }
}
