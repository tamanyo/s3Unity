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
    public string s3Path; // S3��̃t�H���_��
    public string downloadFileName; // �_�E�����[�h����t�@�C����
    public string localFolder; // �_�E�����[�h��̃��[�J���t�H���_
    public bool force = true; // �����t�@�C�����������ꍇ�A�f�t�H���g�ŏ㏑��

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

    // �񓯊���S3��̃t�@�C�������[�J���Ƀ_�E�����[�h���郁�\�b�h
    public async Task DownloadFileByNameAsync(string folderName, string fileName, string localFolder, bool force)
    {
        string keyName = $"{folderName}/{fileName}";  // S3�̃L�[�i�t�H���_���{�t�@�C�����j
//        string keyName = Path.Combine(folderName, fileName).Replace("\\", "/");  // S3�̃L�[�i�t�H���_���{�t�@�C�����j
        Debug.Log($"keyName : {keyName}");
        string localFilePath = Path.Combine(localFolder, fileName); // ���[�J���t�@�C���̃p�X

        // ���[�J���ɓ����t�@�C�������݂��邩�m�F
        if (File.Exists(localFilePath) && !force)
        {
            Debug.LogError($"File already exists: {localFilePath}. Use force=true to overwrite.");
            return;
        }

        // S3����t�@�C�����_�E�����[�h
        try
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = keyName
            };

            using (var response = await s3Client.GetObjectAsync(request))
            {
                // �t�@�C�������[�J���ɕۑ�
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
