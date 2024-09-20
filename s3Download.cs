using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using UnityEngine;

public class s3Download : MonoBehaviour
{
    public bool downloadNow;
    public string s3Path; // S3上のフォルダ名
    public string downloadFileName; // ダウンロードするファイル名
    public string localFolder; // ダウンロード先のローカルフォルダ
    public bool force = true; // 同名ファイルがあった場合、デフォルトで上書き

    private string accessKey = "accessKey";
    private string secretKey = "secretKey";
    private string bucketName = "bucketName";
    private RegionEndpoint region = RegionEndpoint.APNortheast1;

    private AmazonS3Client s3Client;

    void Start()
    {
        AmazonS3Config s3Config = new AmazonS3Config()
        {
            RegionEndpoint = region
        };

        s3Client = new AmazonS3Client(accessKey, secretKey, s3Config);
    }

    private void Update()
    {
        string downloadPath = Path.Combine(localFolder, downloadFileName);
        if (downloadNow && downloadPath != "")
        {
            downloadNow = false;
            DownloadFileByNameAsync(s3Path, downloadFileName, localFolder, force).ConfigureAwait(false);
        }
    }

    // 非同期でS3上のファイルをローカルにダウンロードするメソッド
    public async Task DownloadFileByNameAsync(string folderName, string fileName, string localFolder, bool force)
    {
        string keyName = $"{folderName}/{fileName}";  // S3のキー（フォルダ名＋ファイル名）
//        string keyName = Path.Combine(folderName, fileName).Replace("\\", "/");  // S3のキー（フォルダ名＋ファイル名）
        Debug.Log($"keyName : {keyName}");
        string localFilePath = Path.Combine(localFolder, fileName); // ローカルファイルのパス

        // ローカルに同名ファイルが存在するか確認
        if (File.Exists(localFilePath) && !force)
        {
            Debug.LogError($"File already exists: {localFilePath}. Use force=true to overwrite.");
            return;
        }

        // S3からファイルをダウンロード
        try
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = keyName
            };

            using (var response = await s3Client.GetObjectAsync(request))
            {
                // ファイルをローカルに保存
                using (var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
                {
                    await response.ResponseStream.CopyToAsync(fileStream);
                }

                Debug.Log($"{fileName} successfully downloaded to {localFilePath}");
            }
        }
        catch (AmazonS3Exception e)
        {
            Debug.LogError($"Error downloading file from S3: {e.Message}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Unknown error: {e.Message}");
        }
    }
}
