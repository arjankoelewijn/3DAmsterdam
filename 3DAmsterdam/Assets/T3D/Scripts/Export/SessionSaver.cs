using System;
using System.Collections;
using System.Collections.Generic;
using Netherlands3D.Sharing;
using SimpleJSON;
using UnityEngine;

public static class SessionSaver
{
    public static bool LoadPreviousSession = false;

    public static IDataLoader Loader { get { return JSONSessionLoader.Instance; } }
    private static IDataSaver Saver { get { return JsonSessionSaver.Instance; } } 

    public static string SessionId { get; private set; }
    public static bool HasLoaded => Loader.HasLoaded;

    static SessionSaver()
    {
        SessionId = Application.absoluteURL.GetUrlParamValue("sessionId");
        Debug.Log(SessionId);
        if (SessionId == null)
        {
            Debug.Log("Session id not found, using testID");
            //Guid g = Guid.NewGuid();
            SessionId = "TestSession0";
        }

        Debug.Log("session id: " + SessionId);
    }

    public static void ClearAllSaveData()
    {
        Saver.ClearAllData(SessionId);
    }

    public static void SaveFloat(string key, float value)
    {   
        Saver.SaveFloat(key, value);
    }

    public static void SaveInt(string key, int value)
    {
        Saver.SaveInt(key, value);
    }

    public static void SaveString(string key, string value)
    {
        Saver.SaveString(key, value);
    }

    public static float LoadFloat(string key)
    {
        return Loader.LoadFloat(key);
    }

    public static int LoadInt(string key)
    {
        return Loader.LoadInt(key);
    }

    public static string LoadString(string key)
    {
        return Loader.LoadString(key);
    }

    public static void ExportSavedData()
    {
         Saver.ExportSaveData(SessionId);
    }

    public static void LoadSaveData()
    {
        Loader.ReadSaveData(SessionId);
        Loader.LoadingCompleted += Loader_LoadingCompleted;
    }

    private static void Loader_LoadingCompleted(bool loadSucceeded)
    {
        T3DInit.Instance.LoadBuilding();
        Loader.LoadingCompleted -= Loader_LoadingCompleted;
    }
}