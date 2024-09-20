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
    public string uploadFolder; // �A�b�v����t�@�C���̂���t�H���_
    public string uploadFileName; // �A�b�v����t�@�C����
    public string s3Path; // �o�P�b�g�̉��̃t�H���_��

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

    // �t�@�C���̊g���q�Ɋ�Â���Content-Type�����肷��w���p�[���\�b�h
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
                return "application/json"; // JSON�t�@�C���p��Content-Type��ǉ�
            default:
                return "application/octet-stream";  // �f�t�H���g�̓o�C�i���t�@�C���Ƃ��ď���
        }
    }

    // �񓯊��Ńt�@�C�������烁�f�B�A�t�@�C�����A�b�v���[�h���郁�\�b�h
    public async Task UploadFileByNameAsync(string filePath, string folderName)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            return;
        }

        // ���̃t�@�C�������擾
        string fileName = Path.GetFileName(filePath);

        // �t�H���_���ƃt�@�C��������������S3�̃L�[�ɂ���
        string keyName = Path.Combine(folderName, fileName).Replace("\\", "/");  // S3�ł�'/'���t�H���_��؂�Ɏg�p

        // �t�@�C���̊g���q���擾
        string fileExtension = Path.GetExtension(filePath);
        string contentType = GetContentType(fileExtension);

        // �t�@�C���̃o�C�i���f�[�^��ǂݍ���
        byte[] fileBytes = File.ReadAllBytes(filePath);

        using (var stream = new MemoryStream(fileBytes))
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = keyName,  // �t�H���_���ƃt�@�C���������������L�[��
                InputStream = stream,
                ContentType = contentType  // �擾����Content-Type��ݒ�
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

    // �e�L�X�g�f�[�^�����̏�ŃA�b�v���[�h���郁�\�b�h�i�e�L�X�g�f�[�^�𒼐ڎw��j
    public async Task UploadTextContentAsync(string textContent, string folderName, string originalFileName)
    {
        // �t�H���_���ƃI���W�i���̃t�@�C�������������ăL�[���ɂ���
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
