using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityGoogleDrive;

public class GoogleDriveManager : MonoBehaviour
{
    /*
    private static void DownloadFile(string fileId, string fileName)
    {
        var path = Path.Combine(Application.dataPath, fileName).Relace("\\", "/");
        var request = GoogleDriveFiles.Download(fileId: fileId);
        request.Send().OnDone() += (file) =>
        {
            File.WriteAllBytes(path, file.Content.byteData);
            AssetDatabase.Refresh();
        };
    }*/
}
