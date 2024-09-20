using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using UnityEngine;

public class S3Uploader : MonoBehaviour
{
    public bool uploadNow;
    public string uploadFolder; // アップするファイルのあるフォルダ
    public string uploadFileName; // アップするファイル名
    public string s3Path; // バケットの下のフォルダ名

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
        string uploadPath = Path.Combine(uploadFolder, uploadFileName);
        if (uploadNow && uploadPath != "")
        {
            uploadNow = false;
            UploadFileByNameAsync(uploadPath, s3Path).ConfigureAwait(false);
        }
    }

    // ファイルの拡張子に基づいてContent-Typeを決定するヘルパーメソッド
    private string GetContentType(string fileExtension)
    {
        switch (fileExtension.ToLower())
        {
            case ".png":
                return "image/png";
            case ".jpg":
            case ".jpeg":
                return "image/jpeg";
            case ".mp4":
                return "video/mp4";
            case ".txt":
                return "text/plain";
            case ".json":
                return "application/json"; // JSONファイル用のContent-Typeを追加
            default:
                return "application/octet-stream";  // デフォルトはバイナリファイルとして処理
        }
    }

    // 非同期でファイル名からメディアファイルをアップロードするメソッド
    public async Task UploadFileByNameAsync(string filePath, string folderName)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            return;
        }

        // 元のファイル名を取得
        string fileName = Path.GetFileName(filePath);

        // フォルダ名とファイル名を結合してS3のキーにする
        string keyName = Path.Combine(folderName, fileName).Replace("\\", "/");  // S3では'/'をフォルダ区切りに使用

        // ファイルの拡張子を取得
        string fileExtension = Path.GetExtension(filePath);
        string contentType = GetContentType(fileExtension);

        // ファイルのバイナリデータを読み込む
        byte[] fileBytes = File.ReadAllBytes(filePath);

        using (var stream = new MemoryStream(fileBytes))
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = keyName,  // フォルダ名とファイル名を結合したキー名
                InputStream = stream,
                ContentType = contentType  // 取得したContent-Typeを設定
            };

            try
            {
                var response = await s3Client.PutObjectAsync(request);
                Debug.Log($"{fileExtension} file uploaded successfully as {keyName}");
            }
            catch (AmazonS3Exception e)
            {
                Debug.LogError($"Error uploading {fileExtension} file: {e.Message}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Unknown error: {e.Message}");
            }
        }
    }

    // テキストデータをその場でアップロードするメソッド（テキストデータを直接指定）
    public async Task UploadTextContentAsync(string textContent, string folderName, string originalFileName)
    {
        // フォルダ名とオリジナルのファイル名を結合してキー名にする
        string keyName = Path.Combine(folderName, originalFileName).Replace("\\", "/");

        using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(textContent)))
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = keyName,
                InputStream = stream,
                ContentType = "text/plain"
            };

            try
            {
                var response = await s3Client.PutObjectAsync(request);
                Debug.Log("Text content uploaded successfully");
            }
            catch (AmazonS3Exception e)
            {
                Debug.LogError($"Error uploading text content: {e.Message}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Unknown error: {e.Message}");
            }
        }
    }
}
