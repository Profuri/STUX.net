using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class ChapterScene : Scene, IProvideLoad
{
    private Dictionary<ChapterType, Chapter> _chapterDictionary;

    public UnityEvent<ChapterType,SaveData> OnChapterClear;
    public UnityEvent OnSubChaptersClear;
    
    public override void OnPop()
    {
        base.OnPop();
        _chapterDictionary = new Dictionary<ChapterType, Chapter>();

        var chapters = GetComponentsInChildren<Chapter>();

        foreach( var chapter in chapters )
        {
            ChapterType chapterType = chapter.Data.chapter;
            _chapterDictionary.Add(chapterType,chapter);
        }
        LoadToDataManager();

        DataManager.Instance.LoadData(this);
    }

    private void LoadChapterEvents(SaveData saveData)
    {
        if (saveData == null)
            return;

        
        Dictionary<ChapterType,bool> dictionary = saveData.ChapterProgressDictionary;

        int clearCnt = 0;
        foreach(KeyValuePair<ChapterType,bool> kvp in dictionary)
        {
            ChapterType chapterType = kvp.Key;

            if (kvp.Value)
            {
                clearCnt++;
                OnChapterClear?.Invoke(chapterType,saveData);
            }
        }

        if(clearCnt > 4)
        {
            OnSubChaptersClear?.Invoke();
        }
    }


    public Action<SaveData> GetLoadAction()
    {
        return (saveData) =>
        {
            LoadChapterEvents(saveData);
        };
    }
    
    public void LoadToDataManager()
    {
        DataManager.Instance.SettingDataProvidable(null,this);
    }
}