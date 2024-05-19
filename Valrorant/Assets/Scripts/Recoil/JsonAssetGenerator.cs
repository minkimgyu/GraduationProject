using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using Newtonsoft.Json;

public class JsonAssetGenerator
{
    string _extension = ".txt";
    int _maxLoopCount = 10;

    public void CreateAndSaveJsonAsset(object objectToParse, string filePath, string fileName, bool canOverlap)
    {
        string path;

        if (canOverlap == false)
        {
            path = ReturnNotOverlapFileName(filePath, fileName); // 겹치지 않는 이름을 찾음
        }
        else
        {
            
            path = ReturnPath(filePath, fileName); // 경로 바로 적용
        }

        string jsonAsset = ToJson(objectToParse);
        File.WriteAllText(path, jsonAsset); // 이런 방식으로 생성시켜줌
        AssetDatabase.Refresh();
    }

    string ReturnPath(string filePath, string fileName)
    {
        return Application.dataPath + filePath + fileName + _extension;
    }

    string ReturnNotOverlapFileName(string filePath, string fileName)
    {
        string path = ReturnPath(filePath, fileName);
        if (File.Exists(path) == false) return path;

        Debug.LogError("이미 해당 경로에 파일이 존재함");

        string originName = fileName;

        int loopCount = 0;

        while (true)
        {
            loopCount++;
            if(loopCount > _maxLoopCount)
            {
                // 기본 경로로 지정해줌
                return ReturnPath(filePath, "Default");
            }
            else
            {
                originName = "New" + originName;

                path = ReturnPath(filePath, originName);
                if (File.Exists(path) == false) break;
            }
        }

        return path;
    }

    // newtonSoft로 대체해주기
    public string ToJson(object objectToParse)
    {
        var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        return JsonConvert.SerializeObject(objectToParse, settings);
    }


    public T JsonToObject<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }

    public T JsonToObject<T>(TextAsset tmpAsset)
    {
        T value = JsonConvert.DeserializeObject<T>(tmpAsset.text);
        return value;
    }
}
