using System.Collections.Generic;
using ManagingSystem;
using UnityEngine;

public class UIManager : BaseManager<UIManager>
{
    [SerializeField] private Canvas _mainCanvas;

    public UIComponent TopComponent => _componentStack.Peek();
    
    private Stack<UIComponent> _componentStack;

    public override void Init()
    {
        base.Init();
        _componentStack = new Stack<UIComponent>();
    }

    public override void StartManager()
    {
        
    }

    public UIComponent GenerateUI(string key, Transform parent = null)
    {
        if (parent is null)
        {
            parent = _mainCanvas.transform;
        }

        var component = SceneControlManager.Instance.AddObject(key) as UIComponent;
        return ComponentSetting(component, parent);
    }
    
    public UIComponent GenerateUI(UIComponent component, Transform parent = null)
    {
        if (parent is null)
        {
            parent = _mainCanvas.transform;
        }
        
        return ComponentSetting(component, parent);
    }

    private UIComponent ComponentSetting(UIComponent component, Transform parent)
    {
        if (component != null)
        {
            if (component.componentType == UIComponentType.Screen)
            {
                ClearPanel();
            }

            if (component.componentType is UIComponentType.Screen or UIComponentType.Popup)
            {
                _componentStack.Push(component);
            }

            component.Appear(parent);
        }

        return component;
    }

    public void RemoveUI()
    {
        if (_componentStack.Count <= 0)
        {
            return;
        }
        
        var top = _componentStack.Pop();
        top.Disappear();
    }

    public void ReturnUI()
    {
        RemoveUI();
        if (TopComponent is not null)
        {
            GenerateUI(TopComponent, TopComponent.ParentTrm);
        }
    }

    private void ClearPanel()
    {
        while (_componentStack.Count > 0)
        {
            RemoveUI();
        }
    }
}