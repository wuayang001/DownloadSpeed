using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DownloadSpeed
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            download("https://dev.mysql.com/get/Downloads/MySQLInstaller/mysql-installer-web-community-8.0.27.1.msi", @"C:\123.rar", progressBar1, label1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="URL">下载文件地址</param>
        /// <param name="filename">存放在本地地址</param>
        ///
        /// <param name="state">显示进度条</param>
        ///
        /// <param name="labelT">进度条中提示信息</param>
        public void download(string URL, string filename, System.Windows.Forms.ProgressBar state, System.Windows.Forms.Label labelT)
        {
            float fl = 0;
            try
            {
                //
                System.Net.HttpWebRequest my01 = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(URL);
                //
                System.Net.HttpWebResponse my02 = (System.Net.HttpWebResponse)my01.GetResponse();
                //
                long bytes = my02.ContentLength;
                //
                if (state != null)
                {
                    state.Maximum = (int)bytes;
                }
                //
                System.IO.Stream st = my02.GetResponseStream();
                //
                System.IO.Stream so = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                //
                long bytedown = 0;
                //
                byte[] by = new byte[1024];
                //
                int osize = st.Read(by, 0, (int)by.Length);
                //
                while (osize > 0)
                {

                    bytedown = osize + bytedown;
                    System.Windows.Forms.Application.DoEvents();
                    so.Write(by, 0, osize);
                    if (state != null)
                    {
                        state.Value = (int)bytedown;
                    }
                    osize = st.Read(by, 0, (int)by.Length);

                    fl = (float)bytedown / (float)bytes * 100;
                    label1.Text = "当前补丁下载进度" + fl.ToString() + "%";
                    System.Windows.Forms.Application.DoEvents(); //必须加注这句代码，否则label1将因为循环执行太快而来不及显示信息
                }
                so.Close();
                st.Close();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private async void button_download1_Click(object sender, EventArgs e)
        {
            string url = this.textBox_url1.Text;
            string output_filename = this.textBox_outputFileName.Text;

            var request = System.Net.HttpWebRequest.Create(url);

            using (var response = request.GetResponse())
            {
                // 比例转换
                var ratio = new ProgressRatio(response.ContentLength);
                float a = response.ContentLength;
                float b = 1048576;
                float Mb = a / b;
                string show = Mb.ToString("0.00");

                this.Invoke(
                    new Action(() =>
                {
                    progressBar1.Minimum = 0;
                    progressBar1.Maximum = ratio.GetInteger(response.ContentLength);
                }
                ));

                using (var source = response.GetResponseStream())
                
                using (var destination = File.Create(output_filename))
                {
                    // await source.CopyToAsync(destination);
                    await DumpStream(source,
                        destination,
                        (offset) =>
                        {
                            this.Invoke(new Action(() =>
                            {
                                progressBar1.Value = ratio.GetInteger(offset);
                            }));
                            return true;
                        });
                }

                this.Invoke(new Action(() =>
                {
                    MessageBox.Show(this, $"下载完成。总长度 {response.ContentLength}个字节，合计{show}M");
                }));
            }
        }
        //定义下面函数的progressoutput 的类型
        public delegate bool ProgressOutput(long offset);

        public static async Task<long> DumpStream(Stream streamSource,
    Stream streamTarget,
    ProgressOutput progressOutputMethod)
        {
            int nChunkSize = 8192;
            byte[] bytes = new byte[nChunkSize];
            long lLength = 0;
            while (true)
            {
                if (progressOutputMethod != null)
                {
                    if (progressOutputMethod(lLength) == false)
                        break;
                }

                int n = await streamSource.ReadAsync(bytes, 0, nChunkSize);

                if (n != 0)
                    await streamTarget.WriteAsync(bytes, 0, n);

                if (n <= 0)
                    break;

                lLength += n;
            }

            if (progressOutputMethod != null)
            {
                progressOutputMethod(lLength);
            }

            return lLength;
        }


    }
}
