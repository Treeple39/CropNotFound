using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandler(string _dataDirPath,string dataFlieName)
    {
        dataDirPath = _dataDirPath;
        dataFileName = dataFlieName;
    }

    public void Save(GameData _data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string dataToStore = JsonUtility.ToJson(_data, true);
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }

            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error on trying to save data to file:" + fullPath + "\n" + ex);
        }
    }

    public GameData Load() { 
        string fullPath = Path.Combine(dataDirPath,dataFileName);
        GameData loadData = null;
        if (File.Exists(fullPath)) {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception ex) {
                Debug.LogError("没找到存档的说" + ex);
            }
        }
        return loadData;
    }

    public void Delete()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        if (File.Exists(fullPath)) { 
            File.Delete(fullPath);
        }
    }
}
