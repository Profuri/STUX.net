using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;

public class Chapter : InteractableObject
{
    [SerializeField] private ChapterData _data;
    
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        SceneControlManager.Instance.LoadScene(SceneType.Stage, () =>
        {
            StageManager.Instance.StartNewChapter(_data);
        });
    }
}