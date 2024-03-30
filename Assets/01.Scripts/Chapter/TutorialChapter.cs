using UnityEngine;
using DG.Tweening;

public class TutorialChapter : Chapter
{
    [SerializeField] private Transform _cpuTrm;
    [SerializeField] private float _upOffset = 3f;

    private bool _isShowing = false;

    public override void ShowingSequence(ChapterType chapterType,SaveData saveData)
    {
        if (_isShowing)
        {
            return;
        }

        if(chapterType == Data.chapter)
        {
            _isShowing = true;
            if (saveData.IsShowSequence)
            {
                _cpuTrm.position += Vector3.up *_upOffset;
                transform.position -= Vector3.up * _upOffset;
                
                Activate(false);
                return;
            }

            Sequence sequence = DOTween.Sequence();
            sequence.Append(_cpuTrm.DOLocalMoveY(_cpuTrm.position.y + _upOffset, s_sequenceTime));
            sequence.Join(transform.DOLocalMoveY(-_upOffset, s_sequenceTime));
            sequence.AppendCallback(() =>
            {
                this.gameObject.SetActive(false);
            });
        }
        OnShowSequence?.Invoke();
    }
}
