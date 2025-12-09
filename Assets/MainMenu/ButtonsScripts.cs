using DG.Tweening;
using UnityEngine;

public class ButtonsScripts : MonoBehaviour
{
    Sequence btn_tween;

    private void Start()
    {
        btn_tween = DOTween.Sequence();
    }

    public void On_hover()
    {
        if (btn_tween.IsActive())
        {
            btn_tween.Kill();
        }

        btn_tween.Append(
            transform.DOScale(1.1f, 1f)
            .SetEase(Ease.InOutCirc)
            .SetLink(gameObject)
            );
        btn_tween.Play();
    }

    public void OnHover_exit()
    {
        if (btn_tween.IsActive())
        {
            btn_tween.Kill();
        }

        btn_tween.Append(
            transform.DOScale(1f, 0.2f)
            .SetEase(Ease.OutCirc)
            .SetLink(gameObject)
        );
        btn_tween.Play();
    }
}
