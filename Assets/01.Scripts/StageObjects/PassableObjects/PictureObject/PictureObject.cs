using System.Collections.Generic;
using AxisConvertSystem;
using UnityEngine;

public class PictureObject : ObjectUnit, IPassable
{
    [SerializeField] private Material _enableMat;
    [SerializeField] private Material _disableMat;

    private List<PictureUnit> _units;

    public bool PassableLastAxis { get; set; }
    public bool PassableAfterAxis { get; set; }

    public override void Awake()
    {
        base.Awake();
        _units = new List<PictureUnit>();
        transform.GetComponentsInChildren(_units);
    }

    public override void Init(AxisConverter converter)
    {
        base.Init(converter);
        foreach (var unit in _units)
        {
            unit.SetPictureUnit(this, _enableMat, _disableMat);
        }
    }

    public override void Convert(AxisType axis)
    {
        foreach (var unit in _units)
        {
            unit.ChangeAxis(axis);
        }

        base.Convert(axis);
    }

    public override void ApplyUnitInfo(AxisType axis)
    {
        base.ApplyUnitInfo(axis);

        if (axis == AxisType.Y)
        {
            return;
        }

        if (Collider.isTrigger != PassableAfterAxis)
        {
            Collider.isTrigger = PassableAfterAxis;
            Dissolve(Collider.isTrigger ? 0.55f : 0f, 0.5f, false);
        }
    }

    public override void ApplyDepth()
    {
        base.ApplyDepth();
        PassableLastAxis = PassableAfterAxis;
    }

    public void PassableCheck(AxisType axis)
    {
        if (axis == AxisType.Y)
        {
            PassableAfterAxis = false;
        }
        else
        {
            PassableAfterAxis = axis != AxisType.None;
        }
    }
}