using System.Collections.Generic;
using UnityEngine;
using ManagingSystem;
using System;

public class StoryManager : BaseManager<StoryManager>,IProvideSave,IProvideLoad
{
    private MessageWindow _messagePanel;

    [SerializeField] private List<StoryInfo> _storyList = new List<StoryInfo>();

    private Tuple<StoryInfo,int> GetStory(StoryAppearType timing, ChapterType chapterType, int stageIndex = 7)
    {
        bool Predicate(StoryInfo cs)
        {
            return cs.stageIndex == stageIndex && cs.chapterType == chapterType && cs.appearType == timing;
        }
        
        int index = _storyList.FindIndex(0, _storyList.Count, Predicate);
        if (index != -1 && !_storyList[index].isShown)
        {
            return Tuple.Create(_storyList[index],index);
        }
        
        return null;
    }

    public override void StartManager()
    {
        DataManager.Instance.SettingDataProvidable(this, this);
        DataManager.Instance.LoadData(this);
    }

    public void ResetMessage()
    {
        _messagePanel = null;
    }

    public void StartStory(StoryData storyData,int storyIndex = 0,bool isTypingStory = false)
    {
        if (_messagePanel != null) return;

        _messagePanel = UIManager.Instance.GenerateUI("MessageWindow") as MessageWindow;
        _messagePanel.SetData(storyData);
        DataManager.Instance.SaveData(this);
    }

    public bool StartStoryIfCan(StoryAppearType condition, ChapterType chapterType, int stageIndex = 7)
    {
        var tuple = GetStory(condition,chapterType,stageIndex);

        if (tuple == null) return false;

        StoryData storyData = tuple.Item1.storyData;
        int index = tuple.Item2;

        _storyList[index].isShown = true;

        StartStory(storyData,index, true);
        return true;
    }



    public Action<SaveData> GetSaveAction()
    {
        return (saveData) =>
        {
            for(int i =0; i < _storyList.Count; i++)
            {
                if(saveData.StoryShowList.Count <= i)
                {
                    saveData.StoryShowList.Add(_storyList[i].isShown);
                }
                saveData.StoryShowList[i] = _storyList[i].isShown;
            }
        };
    }

    public Action<SaveData> GetLoadAction()
    {
        return (saveData) =>
        {
            for (int i = 0; i < _storyList.Count; i++)
            {
                try
                {
                    _storyList[i].isShown = saveData.StoryShowList[i];
                }
                catch
                {
                    _storyList[i].isShown = false;
                }
            }
        };
    }
}
