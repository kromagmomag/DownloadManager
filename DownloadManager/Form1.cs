namespace DownloadManager
{
    #region [ Using Directives ]

    using System;
    //using System.Collections.Generic;
    //using System.ComponentModel;
    //using System.Data;
    //using System.Drawing;
    //using System.Linq;
    //using System.Text;
    //using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Net;
    using System.IO;
    using System.Threading;


    #endregion


    public partial class Form1: Form
    {
        private Thread thrDownload;
        private Stream strResponse;
        private Stream strLocal;
        private HttpWebRequest webRequest;
        private HttpWebResponse webResponse;
        private static int PercentProgress;
        private delegate void UpdateProgressCallback(long BytesRead, long TotalBytes);


        public Form1()
        {
            InitializeComponent();
        }


        private void Download()
        {
            using (WebClient wcDownload = new WebClient())
            {
                try
                {

                    webRequest = (HttpWebRequest)WebRequest.Create(UrlTextBox?.Text);
                    webRequest.Credentials = CredentialCache.DefaultCredentials;
                    webResponse = (HttpWebResponse)webRequest.GetResponse();
                    long fileSize = webResponse.ContentLength;
                    strResponse = wcDownload.OpenRead(UrlTextBox.Text);
                    strLocal = new FileStream(PathTextBox.Text, FileMode.Create, FileAccess.Write, FileShare.None);
                    int byteSize = 0;
                    byte[] downBuffer = new byte[2048];
                    while ((byteSize = strResponse.Read(downBuffer, 0, downBuffer.Length)) > 0)
                    {
                        strLocal.Write(downBuffer, 0, byteSize);
                        this.Invoke(new UpdateProgressCallback(this.UpdateProgress), new object[] { strLocal.Length, fileSize });
                    }
                }
                catch
                {

                }
                finally
                {
                    strResponse.Close();
                    strLocal.Close();
                }
            }
        }


        private void UpdateProgress(long BytesRead, long TotalBytes)
        {
            PercentProgress = Convert.ToInt32((BytesRead * 100) / TotalBytes);
            prgDownload.Value = PercentProgress;
            lblProgress.Text = "Downloaded " + BytesRead + "out of " + TotalBytes + " (" + PercentProgress + "%)";
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            lblProgress.Text = "Download Starting...";
            thrDownload = new Thread(Download);
            thrDownload?.Start();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            webResponse?.Close();
            strResponse?.Close();
            strLocal?.Close();
            thrDownload?.Abort();
            prgDownload.Value = 0;
            lblProgress.Text = "Download Stopped!";
        }
    }
}
