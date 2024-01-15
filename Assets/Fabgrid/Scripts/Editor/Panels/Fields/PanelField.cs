using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.UIElements;
using System.Linq;

public abstract class PanelField
{
    private VisualElement _root;
    protected VisualElement _fieldRoot;
    

    protected PanelField(VisualElement root,VisualTreeAsset field, FieldInfo info)
    {
        _root = root;
        
        //루트에 자식으로 넣는데 기존에 있는 자식이 사라지고 있는 것 같음
        var container = field.Instantiate();
        _root?.Add(container);

        _fieldRoot = container;
    }
    
    public abstract void Init(FieldInfo info);
}