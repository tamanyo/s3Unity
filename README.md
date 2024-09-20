### `README.md`

```md
# S3Upload and S3Download for Unity

This repository contains two Unity scripts, `S3Upload` and `S3Download`, which allow you to upload and download files from AWS S3 buckets directly within Unity. These scripts make it easy to integrate S3 functionality into your Unity project, allowing you to manage file transfers efficiently.

## Features

- **S3Upload**: Upload files from a local folder to an S3 bucket, supporting multiple file types (`.png`, `.jpg`, `.jpeg`, `.mp4`, `.txt`, `.json`).
- **S3Download**: Download files from an S3 bucket to a local folder with the ability to handle file overwriting based on a `force` flag.
- Both scripts support asynchronous operations to prevent blocking the Unity main thread.
- Automatic handling of special characters like spaces in file names via URL encoding.

## Requirements

- **Unity 2019.x or later**
- **AWS SDK for .NET** integrated into your Unity project.
- **AWS S3 Account** with proper IAM roles and permissions to upload and download files from S3.

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/your-username/your-repository.git
```

### 2. Setup AWS Credentials

In the `S3Upload` and `S3Download` scripts, you need to configure your AWS credentials and S3 bucket information:

```csharp
private string accessKey = "YOUR_ACCESS_KEY";
private string secretKey = "YOUR_SECRET_KEY";
private string bucketName = "YOUR_BUCKET_NAME";
private RegionEndpoint region = RegionEndpoint.APNortheast1;  // Change the region as necessary
```

Make sure your AWS account has the correct permissions to access the S3 bucket for both uploading and downloading objects. Example IAM policy:

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "s3:*"
      ],
      "Resource": "arn:aws:s3:::your-bucket-name/*"
    }
  ]
}
```

### 3. Usage in Unity

#### S3Upload

To upload a file to S3 from Unity:

```csharp
public class UploadManager : MonoBehaviour
{
    public S3Upload uploader;

    void Start()
    {
        uploader.uploadNow = true;
        uploader.uploadFolder = "C:/YourLocalFolder";
        uploader.uploadFileName = "example.png";
        uploader.s3Path = "your-s3-folder";
    }
}
```

- **`uploadFolder`**: The local folder path where the file exists.
- **`uploadFileName`**: The name of the file to be uploaded.
- **`s3Path`**: The path in the S3 bucket where the file will be uploaded.

#### S3Download

To download a file from S3 to your local folder:

```csharp
public class DownloadManager : MonoBehaviour
{
    public S3Download downloader;

    void Start()
    {
        downloader.downloadNow = true;
        downloader.s3Path = "your-s3-folder";
        downloader.downloadFileName = "example.png";
        downloader.localFolder = "C:/YourLocalFolder";
        downloader.force = true;  // If true, overwrites existing files
    }
}
```

- **`s3Path`**: The path in the S3 bucket where the file is located.
- **`downloadFileName`**: The name of the file to be downloaded.
- **`localFolder`**: The local folder where the file will be saved.
- **`force`**: If true, the file will overwrite any existing file with the same name in the destination folder.

### 4. Handling Special Characters

The scripts automatically handle spaces and special characters in file names using URL encoding. Ensure that the file names and paths are correctly formatted and encoded when interacting with S3.

## Supported File Types

The following file types are supported for upload and download:

- `.png`
- `.jpg`, `.jpeg`
- `.mp4`
- `.txt`
- `.json`

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contributing

Feel free to fork this repository and contribute via pull requests. Any feedback and contributions to improve the functionality or expand the supported file types and features are welcome!

```
