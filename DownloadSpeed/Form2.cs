using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
//using Aspose.Cells;
using System.Threading;
using System.Timers;
//using ClosedXML.Excel;





namespace DownloadSpeed
{
    public partial class Form2 : Form
    {
        
        public Form2()
        {
          
            InitializeComponent();
        }

        private async void button_download1_Click(object sender, EventArgs e)
        {
            string URL = textBox1_URL.Text.Trim();
            string OUT = textBox2_out.Text.Trim();
            string cyc = textBox_cycle.Text.Trim();
            if (string.IsNullOrEmpty(URL) == true
                || string.IsNullOrEmpty(OUT) == true
                || string.IsNullOrEmpty(cyc) == true)
            {
                MessageBox.Show(this, "参数无效！请保证 各项参数不能为空！");
                return;
            }
            Stopwatch timer = new Stopwatch();
            timer.Start();

            int cycle = Convert.ToInt32(cyc);
            for (int i = 1; i < cycle + 1; i++)
            {

                DateTime start = DateTime.Now;

                string activeDir = @"C:/testnetspeed";
                string zip = textBox1_URL.Text.Substring(textBox1_URL.Text.Length- 4, 4);

                //控制文件个数不超过三个，超过的话，依次重新覆盖
                if (i <= 3)
                {
                    textBox2_out.Text = activeDir + "/" + i.ToString() + zip;
                };
                if (i > 3)
                {
                    int j = i % 3;
                    int js = j + 1;
                    textBox2_out.Text = activeDir + "/" + js.ToString() + zip;
                };
                string url = this.textBox1_URL.Text;

                string output_filename = this.textBox2_out.Text;

                var request = System.Net.HttpWebRequest.Create(url);

                using (var response = request.GetResponse())
                {
                    // 比例转换
                    var ratio = new ProgressRatio(response.ContentLength);

                    //将下载的文件大小转换成MB格式。Mb
                    double a = response.ContentLength;
                    double b = 1024;
                    double Mb = a / b;
                    string show = Mb.ToString("0.00");

                    //计算进度条 
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
                        DateTime star = DateTime.Now;
                        // await source.CopyToAsync(destination);
                        await DumpStream(source,
                            destination,
                            (ww) =>
                            {
                                this.Invoke(new Action(() =>
                                {
                                    //显示下载进度的百分比
                                    progressBar1.Value = ratio.GetInteger(ww);
                                    double percentage = ((double)ww / response.ContentLength) * 100;
                                    label_percentage.Text = percentage.ToString("0.00") + "％";
                                    var star1 = progressBar1.Value;

                                }));
                             
                                return true;

                            });
                    }
                  
                    this.Invoke(new Action(() =>
                    {
                        //将每次下载的文件写入到textbox中
                        DateTime end = DateTime.Now;
                        TimeSpan timespan = end - start;
                       
                        double second = timespan.TotalMilliseconds / 1000;
                        double speed = Mb / second;
                        string speeds = speed.ToString("0.00");
                        
                        textBox_statistics.Text +="第"+i+"列"+ "总长度" + response.ContentLength + "\t合计：" + show + "K\t下载速度为：" + speeds + "kb/s\r\n";
                    }));
                   
                }
            }
        }

        
      
        //回调函数
        public delegate bool ProgressOutput(long offset);
        
        //
        public static async Task<long> DumpStream(Stream streamSource,
    Stream streamTarget,
    ProgressOutput progressOutputMethod)
        {
            
            int nChunkSize = 8192;
            byte[] bytes = new byte[nChunkSize];
            long lLength = 0;
           // DateTime star = DateTime.Now;
           // timer.Start();
            while (true)
            {
                if (progressOutputMethod != null)
                {
                    if (progressOutputMethod(lLength) == false)
                        break;
                }
                //	
                //从当前流异步读取字节序列，并将流中的位置提升读取的字节数。
                int n = await streamSource.ReadAsync(bytes, 0, nChunkSize);
                
                if (n != 0)
                    //将字节序列异步写入当前流，并将流的当前位置提升写入的字节数。
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

        private void button2_Click(object sender, EventArgs e)
        {
            //输出operlog文件，命名为yyyy-MM-dd-hh-mm-ss
            string us = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");
            //MessageBox.Show(us);
            string log = @"C:/testnetspeed/operlog/";

            System.IO.Directory.CreateDirectory(log);

            FileStream txt = new FileStream(log + us + ".txt", FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(txt);
            sw.WriteLine(textBox_statistics.Text.Trim());//开始写入值
            sw.Close();
            txt.Close();
            Application.Exit();
        }
        
      //测试瞬时速度
        private void button_Instantaneouspeed_Click(object sender, EventArgs e)
        {
            //string strCodeFileDir = Path.GetDirectoryName(this.CodeFileName);
            //HtmlInputDialog window = new HtmlInputDialog();

            //window.Text = "指定统计特性";
            //window.Url = strCodeFileDir + "\\input.html";
            //window.Size = new Size(800, 500);
            //window.ShowDialog();




            //SaveFileDialog dlg = new SaveFileDialog()
            //{
            //    Title = "请指定要输出文件名",
            //    FileName = "",
            //    RestoreDirectory = true,
            //    CreatePrompt = false,
            //    OverwritePrompt = false,
            //    // InitialDirectory = Environment.CurrentDirectory,
            //    Filter = "电子表格(*.xlsx)|*.xlsx",
            //};
            //this.fileName = dlg.FileName;

            //label_Instantaneousspeed.Text = progressBar1.Value.ToString();
        }

        private void label_Instantaneousspeed_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_URL_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

