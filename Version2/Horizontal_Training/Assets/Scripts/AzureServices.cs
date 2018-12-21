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

    private const string userDirectory = "Demo5";

    public int Repetitions { get; private set; }
    public int ForwardSpeed { get; private set; }
    public int BackwardSpeed { get; private set; }

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
    private CloudFile DownloadCloudFile;

    private CloudFile ErrorUploadCloudFile;
    private CloudFile BoxUploadCloudFile;
    private CloudFile PathUploadCloudFile;
    private CloudFile WindowErrorUploadCloudFile;

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

    private CloudFileDirectory dirUser;

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
        azureStatusText.text = "Select Training..";
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

      

        // Create a file client for interacting with the file service.
        fileClient = storageAccount.CreateCloudFileClient();
        

        // Create a share for organizing files and directories within the storage account.
        share = fileClient.GetShareReference(fileShare);
       

        if(!await share.ExistsAsync())
        {
            azureStatusText.text = "Share file not found";
        }

        await share.CreateIfNotExistsAsync();

        CloudFileDirectory root;
        // Get a reference to the root directory of the share.
        try {
            root = share.GetRootDirectoryReference();
        }
        catch (StorageException ex)
        {
            RequestResult requestInformation = ex.RequestInformation;
            if (requestInformation != null)
                Debug.Log("Storage Excpetion: " + requestInformation);
            throw;
        }
        
        

        // Create a directory under the root directory
        dir = root.GetDirectoryReference(storageDirectory);

        dirUser = dir.GetDirectoryReference(userDirectory);
       

        await dir.CreateIfNotExistsAsync();
        await dirUser.CreateIfNotExistsAsync();

        if(DownloadCloudFile != null)
        {
            //Check if the there is a stored text file containing the list
            DownloadCloudFile = dir.GetFileReference(DownloadCloudFile + ".txt");


            if (!await DownloadCloudFile.ExistsAsync())
            {
                azureStatusText.text = "File not found!";

            }



            else
            {
                azureStatusText.text = "Start Training!";

            }
        }
        
        // Download file error
        // await ReplicateListFromAzureAsync();

    
    }

    /// <summary>
    /// Upload the locally stored List to Azure
    /// </summary>
    /// 
    int downloadCount = 0;
    public async Task DownloadTrainingSettings(string filename)
    {
        DownloadCloudFile = dir.GetFileReference(filename + ".txt");

        if (!await DownloadCloudFile.ExistsAsync())
        {
            azureStatusText.text = "File not found!";

        }
        else
        {
            azureStatusText.text = "File found!";

        }
        string azureFileContent = await DownloadCloudFile.DownloadTextAsync().ConfigureAwait(false);
        string[] content = azureFileContent.Split(',');
        AzureServices.instance.Repetitions = Int32.Parse(content[0]);
        AzureServices.instance.ForwardSpeed = Int32.Parse(content[1]);
        AzureServices.instance.BackwardSpeed = Int32.Parse(content[2]);
        //await Task.Delay(100).ConfigureAwait(false);
    }










    /*******************************
     * UPLOAD
     * ******************************/
    public string ErrorFileName;
    public string BoxPosFileName;
    public string PathPosFileName;
    public string WindowErrorFileName;
    
    public async Task UploadTrainingInfo(string trainingType)
    {
        if (trainingType != null)
        {
            ErrorFileName = "Error" + trainingType;
            BoxPosFileName = "BoxPosition" + trainingType;
            PathPosFileName = "PathPosition" + trainingType;
            WindowErrorFileName = "WindowError" + trainingType;
        }


        ErrorUploadCloudFile = dirUser.GetFileReference("Horizontal" + ErrorFileName + ".txt");
        BoxUploadCloudFile = dirUser.GetFileReference("Horizontal" + BoxPosFileName + ".txt");
        PathUploadCloudFile = dirUser.GetFileReference("Horizontal" + PathPosFileName + ".txt");
        WindowErrorUploadCloudFile = dirUser.GetFileReference("Horizontal" + WindowErrorFileName + ".txt");

        string errorUpload = string.Join(" ", PathFollower.Instance.errorList.ToArray());
        string boxUpload = string.Join("", PathFollower.Instance.boxPositonList.ToArray());
        string pathUpload = string.Join("", PathFollower.Instance.pathPositionList.ToArray());
        string windowErrorUpload = string.Join("", PathFollower.Instance.windowErrorList.ToArray());

        await ErrorUploadCloudFile.UploadTextAsync(errorUpload);
        await BoxUploadCloudFile.UploadTextAsync(boxUpload);
        await PathUploadCloudFile.UploadTextAsync(pathUpload);
        await WindowErrorUploadCloudFile.UploadTextAsync(windowErrorUpload);
    }

}
