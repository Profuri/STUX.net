using AxisConvertSystem;
using InteractableSystem;
using UnityEngine;

public class TutorialObjectUnit : InteractableObject
{
    [Header("Tutorial System")] 
    [SerializeField] private TutorialInfo _info;   
    [SerializeField] private Transform _markAppearTransform;

    private TutorialMark _tutorialMark;

    public override void Init(AxisConverter converter)
    {
        base.Init(converter);
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    private void OnDisable()
    {
        if(_tutorialMark != null)
        {
            _tutorialMark.Off();
        }
    }

    public override void OnPush()
    {
        base.OnPush();
        if(_tutorialMark != null)   
        {
            _tutorialMark.Off();
        }
    }

    public override void ApplyUnitInfo(AxisType axis)
    {
        base.ApplyUnitInfo(axis);

        if(axis != AxisType.None)
        {
            if(_tutorialMark != null)
            {
                _tutorialMark.Off();
                _tutorialMark = null;
            }
            Dissolve(1, 0.5f);
        }
        else
        {
            Dissolve(0, 0.5f);
        }
    }

    private void LoadTutorialMark()
    {
        _tutorialMark = SceneControlManager.Instance.AddObject("TutorialMark") as TutorialMark;

        if (_markAppearTransform != null)
        {
            _tutorialMark.transform.position = _markAppearTransform.position;
        }
        else
        {
            _tutorialMark.transform.position = Collider.bounds.center + Vector3.up * 2.0f;
        }

        _tutorialMark.transform.SetParent(Section.transform);
        _tutorialMark.On();
    }


    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        if (!TutorialManager.Instance.OnTutorial)
        {
            TutorialManager.Instance.SetUpTutorial(_info);

            _tutorialMark?.Off();
            _tutorialMark = null;
        }
        else
        {
            TutorialManager.Instance.StopTutorial();

            if (_tutorialMark == null)
            {
                LoadTutorialMark();
            }
        }
    }
    
    public override void OnDetectedEnter()
    {
        base.OnDetectedEnter();
        if (_tutorialMark == null)
        {
            LoadTutorialMark();
        }
    }

    public override void OnDetectedLeave()
    {
        base.OnDetectedLeave();

        if(_tutorialMark != null)
        {
            _tutorialMark.Off();
            _tutorialMark = null;
        }
    }
}
