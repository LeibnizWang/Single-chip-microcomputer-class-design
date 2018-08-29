using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Threading; //添线程引用
using System.Text.RegularExpressions;
namespace C_review_shangweiji
{
    public partial class Form1 : Form
    {
        public string WorkFlag="1";
        public System.DateTime last_time = new System.DateTime();
        public System.DateTime current_time = new System.DateTime();
        public delegate void Displaydelegate(byte[] InputBuf);
        Byte[] OutputBuf = new Byte[128];
        public Displaydelegate disp_delegate;
        public Form1()
        {

            InitializeComponent();
            disp_delegate = new Displaydelegate(DispUI);
            Control.CheckForIllegalCrossThreadCalls = false;
            foreach (string portname in SerialPort.GetPortNames())
            {
                comboBox1.Items.Add(portname);
            }
            foreach (SerialPort1Baudset rate in Enum.GetValues(typeof(SerialPort1Baudset)))
            {
                comboBox2.Items.Add(((int)rate).ToString());
            }
            Control.CheckForIllegalCrossThreadCalls = false;    //意图见解释  
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(sp1_DataReceived); //订阅委托 
        }
        public void DispUI(byte[] InputBuf)
        {
            //textBox1.Text = Convert.ToString(InputBuf);  

            ASCIIEncoding encoding = new ASCIIEncoding();
            textBox1.Text = encoding.GetString(InputBuf);
        }
        public enum SerialPort1Baudset : int //波特率参数
        {           
            BaudRate_9600 = 9600,
            BaudRate_14400 = 14400,
            BaudRate_19200 = 19200,
            BaudRate_28800 = 28800,
            BaudRate_38400 = 38400,
            BaudRate_56000 = 56000,
            BaudRate_57600 = 57600,
            BaudRate_115200 = 115200,
            BaudRate_128000 = 128000,
            BaudRate_230400 = 230400,
            BaudRate_256000 = 256000,
            BaudRate_500000 = 500000
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                
                serialPort1.BaudRate = int.Parse(comboBox2.Text);
                serialPort1.DataBits = 8;
                serialPort1.Parity = System.IO.Ports.Parity.None;
                serialPort1.StopBits = System.IO.Ports.StopBits.One;
                serialPort1.PortName = comboBox1.Text;
                serialPort1.Open();
                //MessageBox.Show("打开串口成功");
                linkLabel1.Text = "已连接";
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            //MessageBox.Show("关闭串口成功");
            linkLabel1.Text = "未连接";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                MessageBox.Show("发送区内数据不能为空"); return;
            }
            try
            {
                    SendStringData(serialPort1);
            }
            catch (Exception el)
            {
                MessageBox.Show(el.Message, "发送数据异常");
            }
        }

        private void ResetPort()
        {
            // this.serialPort1.BreakState = true; //中断状态
            serialPort1.Encoding = Encoding.GetEncoding("GB2312");
            serialPort1.PortName = comboBox1.Text;
            serialPort1.BaudRate = int.Parse(comboBox2.Text);
            serialPort1.DataBits = 8;
            serialPort1.Parity = System.IO.Ports.Parity.None;
             serialPort1.StopBits = System.IO.Ports.StopBits.One;
            serialPort1.ReadTimeout = 500;
            serialPort1.WriteTimeout = 1000;
            try
            {
                serialPort1.Open();
                
                linkLabel1.Text = "已连接";
                timer1.Enabled = true;
            }
            catch
            {
                MessageBox.Show("串口使用中，请选其它端口 \n ", "操作提示");
                linkLabel1.Text = "未连接";
            }
        }

        private void SendStringData(SerialPort serialPort1) //传输发送区数据
        {
            
            serialPort1.Write(textBox2.Text);
            //serialPort1.Write("\r\n");//单片机串口协议要求最后必须为换行和回车
            label10.Text = (int.Parse(label10.Text.Trim()) + 1).ToString(); //统计发送区数据           
        }

        private void button4_Click(object sender, EventArgs e)
        {
            label10.Text = "0";
            label9.Text = "0";
            textBox1.Text = "";
        }
        
        private void ReceiveData(SerialPort serialPort1) //开启接收线程
        {
            Thread threadReceiveSub = new Thread(new ParameterizedThreadStart(AsyReceiveData));
            threadReceiveSub.Start(serialPort1);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            ReceiveData(serialPort1); //开启接收数据线程
            
        }
        private void AsyReceiveData(object serialPortobj) //数据接收
        {
            SerialPort serialPort = (SerialPort)serialPortobj;
            System.Threading.Thread.Sleep(500);
            try
            {
                //last_time = System.DateTime.Now;
                textBox1.Text = serialPort.ReadExisting();
                label9.Text = (int.Parse(label9.Text.Trim()) + 1).ToString(); //统计接收区数据 组数
                //label13.Text = serialPort.ReadLine();
                //label14.Text = serialPort.ReadLine();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "接收数据异常"); //处理错误
            }
        }
        
       

        
        void sp1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string read_temp;
            string str_T;
            byte[] byteRead = new byte[serialPort1.BytesToRead];    //BytesToRead:sp1接收的字符个数  
            //last_time = current_time;
            //current_time = System.DateTime.Now;
            if (true)
            {
                try
                {
                    //文件操作
                    string result1 = @"C:\Users\Leibniz\Desktop\C_review_shangweiji\C_review_shangweiji\C_review_shangweiji\txt\result.txt";//结果保存到F:\result1.txt
                    FileStream fs = new FileStream(result1, FileMode.Append);
                    StreamWriter wr = null;
                    wr = new StreamWriter(fs);

                    System.DateTime current_time = new System.DateTime();
                    current_time = System.DateTime.Now;
                    serialPort1.Encoding = System.Text.Encoding.GetEncoding("GB2312");
                    read_temp = serialPort1.ReadLine();
                    str_T = current_time.ToString("t");
                    textBox1.Text += "[" + str_T + ":" + current_time.Second.ToString() + ":" + current_time.Millisecond.ToString() + "]  " + read_temp + "\r\n"; //注意：回车换行必须这样写，单独使用"\r"和"\n"都不会有效果  
                                                                                                                                                                  //textBox1.Text += serialPort1.ReadLine();
                                                                                                                                                                  //显示电压值
                    label13.Text = read_temp;
                    //显示角度值
                    float float_temp;
                    //////int int_temp;
                    float_temp = float.Parse(read_temp);
                    float_temp = float_temp / 5 * 180;
                    //int_temp = int.Parse(float_temp.ToString());
                    label14.Text = float_temp.ToString();
                    if (current_time.Second > last_time.Second)
                    {
                        wr.WriteLine("[" + current_time.ToString("f") + ":" + current_time.Second.ToString() + "]  电压值:" + read_temp + "  角度值:" + float_temp.ToString() + "°\n");
                        last_time = current_time;
                    }
                    //fs.Close();
                    wr.Close();
                    serialPort1.DiscardInBuffer();   //清空SerialPort控件的Buffer  
                    label9.Text = (int.Parse(label9.Text.Trim()) + 1).ToString(); //统计接收区数据 组数
                }
                catch (Exception d)
                {
                    MessageBox.Show(d.Message, "发送格式不正确"); //处理错误
                }

            }
            else                                            //'发送16进制按钮'  
            {
                try
                {
                    Byte[] receivedData = new Byte[serialPort1.BytesToRead];        //创建接收字节数组  
                    serialPort1.Read(receivedData, 0, receivedData.Length);         //读取数据                         
                    serialPort1.DiscardInBuffer();                                  //清空SerialPort控件的Buffer  
                    string strRcv = null;

                    for (int i = 0; i < receivedData.Length; i++) //窗体显示  
                    {

                        strRcv += receivedData[i].ToString("X2");  //16进制显示  
                    }
                    textBox1.Text += strRcv + "\r\n";
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message, "出错提示");
                    //txtSend.Text = "";
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.Text.Length;

            textBox1.ScrollToCaret();
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Write("1");
                label19.Text = "1";
                //serialPort1.Write("\r\n");//单片机串口协议要求最后必须为换行和回车
                //label10.Text = (int.Parse(label10.Text.Trim()) + 1).ToString();
            }
            catch (Exception el)
            {
                MessageBox.Show(el.Message, "开始工作指令异常");
            }
        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Write("0");
                label19.Text = "0";
                //serialPort1.Write("\r\n");//单片机串口协议要求最后必须为换行和回车
                //label10.Text = (int.Parse(label10.Text.Trim()) + 1).ToString();
            }
            catch (Exception el)
            {
                MessageBox.Show(el.Message, "开始工作指令异常");
            }
        }
    }
}
