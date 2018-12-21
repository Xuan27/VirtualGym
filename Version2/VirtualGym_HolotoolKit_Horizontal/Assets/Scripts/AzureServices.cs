using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System.IO;
using System.Net;

public class AzureServices : MonoBehaviour {

    /// <summary>
    /// Provides Singleton-like behavior to this class.
    /// </summary>
    public static AzureServices instance;

    /// <summary>
    /// Reference Target for AzureStatusText Text Mesh object
    /// </summary>
    public Text azureStatusText;

    /// <summary>
    /// Holds the Azure Function endpoint - Insert your Azure Function
    /// Connection String here.
    /// </summary>
    //private string azureFunctionEndpoint = "https://academyfunctionapp0243.azurewebsites.net/api/HttpTriggerCSharp1?code=rhrJ/G9mXmwBPxTxsCBHBAHUYpjkqh5SUvHyPqfC32i4uXzsaFBObw==";

    /// <summary>
    /// Holds the Storage Connection String - Insert your Azure Storage
    /// Connection String here.
    /// </summary>
    private string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=virtualgym;AccountKey=M1NYBoW/JD7ZCNUPUaxv4g/1FBIPtNw5+cAnXpCXonm0E2CizFgmRQSvohOL8HVrVdGdYQLdn2GRH/1f1iLBFA==;EndpointSuffix=core.windows.net";

    /// <summary>
    /// Name of the Cloud Share - Hosts directories.
    /// </summary>
    private const string fileShare = "gymsharefiles";

    /// <summary>
    /// Name of a Directory within the Share
    /// </summary>
    private const string storageDirectory = "data";

    /// <summary>
    /// The Cloud File
    /// </summary>
    private CloudFile errorCloudFile;

    /// <summary>
    /// The Linked Storage Account
    /// </summary>
    private CloudStorageAccount storageAccount;

    /// <summary>
    /// The Cloud Client
    /// </summary>
    private CloudFileClient fileClient;

    /// <summary>
    /// The Cloud Share - Hosts Directories
    /// </summary>
    private CloudFileShare share;

    /// <summary>
    /// The Directory in the share that will host the Cloud file
    /// </summary>
    private CloudFileDirectory dir;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    private void Start()
    {
        // Disable TLS cert checks only while in Unity Editor (until Unity adds support for TLS)
#if UNITY_EDITOR
        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
#endif
        azureStatusText.text = "Connecting..";
        //Creating the references necessary to log into Azure and check if the Storage Directory is empty
        CreateCloudIdentityAsync();
    }

    /// <summary>
    /// Create the references necessary to log into Azure
    /// </summary>
    private async void CreateCloudIdentityAsync()
    {
        // Retrieve storage account information from connection string
        storageAccount = CloudStorageAccount.Parse(storageConnectionString);

        Debug.Log(storageAccount.ToString());

        // Create a file client for interacting with the file service.
        fileClient = storageAccount.CreateCloudFileClient();

        // Create a share for organizing files and directories within the storage account.
        share = fileClient.GetShareReference(fileShare);

        if(!await share.ExistsAsync())
        {
            azureStatusText.text = "Share file not found";
        }

        await share.CreateIfNotExistsAsync();

        // Get a reference to the root directory of the share.
        CloudFileDirectory root = share.GetRootDirectoryReference();

        // Create a directory under the root directory
        dir = root.GetDirectoryReference(storageDirectory);

        await dir.CreateIfNotExistsAsync();

        //Check if the there is a stored text file containing the list
        errorCloudFile = dir.GetFileReference("Error.txt");

        if (!await errorCloudFile.ExistsAsync())
            azureStatusText.text = "File not found!";
        
        else
        {
            azureStatusText.text = "File found!";
           // Download file error
           // await ReplicateListFromAzureAsync();
        }
    }

        ///<summary>
    /// Get the List stored in Azure and use the data retrieved to replicate 
    /// a Shape creation pattern
    ///</summary>
    /*private async Task ReplicateListFromAzureAsync()
    {
        string azureTextFileContent = await errorCloudFile.DownloadTextAsync();

        string[] errors = azureTextFileContent.Split(',');

        foreach (string error in errors)
        {
            float i;

            float.TryParse(error.ToString(), out i);

            ShapeFactory.instance.shapeHistoryList.Add(i);

            await Task.Delay(500);
        }

        azureStatusText.text = "Load Complete!";
    }*/

    /// <summary>
    /// Call to the Azure Function App to request a Shape.
    /// </summary>
    /*public async void CallAzureFunctionForNextShape()
    {
        int azureRandomInt = 0;

        // Call Azure function
        HttpWebRequest webRequest = WebRequest.CreateHttp(azureFunctionEndpoint);

        WebResponse response = await webRequest.GetResponseAsync();

        // Read response as string
        using (Stream stream = response.GetResponseStream())
        {
            StreamReader reader = new StreamReader(stream);

            String responseString = reader.ReadToEnd();

            //parse result as integer
            Int32.TryParse(responseString, out azureRandomInt);
        }

        //add random int from Azure to the ShapeIndexList
        ShapeFactory.instance.shapeHistoryList.Add(azureRandomInt);

        ShapeFactory.instance.CreateShape(azureRandomInt, false);

        //Save to Azure storage
        await UploadListToAzureAsync();
    }*/

    /// <summary>
    /// Upload the locally stored List to Azure
    /// </summary>
    public async Task UploadListToAzureAsync()
    {
        // Uploading a local file to the directory created above
        string listToString = string.Join(",", PathFollower.Instance.errorList.ToArray());
        await errorCloudFile.UploadTextAsync(listToString);
    }

}
