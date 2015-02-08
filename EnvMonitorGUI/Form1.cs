using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnvMonitorGUI
{
    public partial class Form1 : Form
    {
        bool connected = false;
        bool activateLED = true;
        string prevTemp, prevHumid, prevBright;
        public Form1()
        {
            InitializeComponent();

            string[] serialPorts = System.IO.Ports.SerialPort.GetPortNames();
            comboBox1.Items.AddRange(serialPorts);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch(connected)
            {
                case true:
                    if (serialPort1.IsOpen)
                    {
                        serialPort1.Close();
                        listBox1.Items.Clear();
                        listBox1.Items.Add("尚未連線");
                        button2.Enabled = false;
                        timer1.Enabled = false;
                        button1.Text = "建立連線";
                        connected = false;
                    }
                    break;
                case false:
                    try
                    {
                        serialPort1.PortName = comboBox1.SelectedItem.ToString();
                        serialPort1.BaudRate = 115200;
                    }
                    catch (Exception ex)
                    {
                        listBox1.Items.Add("錯誤：請選擇正確的 COM Port");
                        break;
                    }
                    if (!serialPort1.IsOpen)
                    {
                        try
                        {
                            serialPort1.Open();
                            timer1.Enabled = true;
                            button1.Text = "關閉連線";
                            button2.Enabled = true;
                            connected = true;
                        }
                        catch (Exception ex)
                        {
                            listBox1.Items.Add("錯誤：當前 COM Port 無法建立連線。");
                            break;
                        }
                    }
                    break;
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {

            string str = serialPort1.ReadExisting();
            string[] value = str.Split('/');


            string thisTemp = "0", thisHumid = "0", thisBright = "0";
            bool thisLED = false;

            try
            {
                thisTemp = value[0];
                thisHumid = value[1];
                thisBright = value[2];
            }
            catch (Exception ex)
            {
                thisTemp = prevTemp;
                thisHumid = prevHumid;
                thisBright = prevBright;
                thisLED = activateLED;
            }
            finally
            {
                listBox1.Items.Clear();
                listBox1.Items.Add("溫度：" + thisTemp + " °C");
                listBox1.Items.Add("濕度：" + thisHumid + " %");
                listBox1.Items.Add("環境亮度：" + thisBright + " %");
                listBox1.Items.Add("照明裝置：" + ((activateLED) ? "開" : "關"));

                prevTemp = thisTemp;
                prevHumid = thisHumid;
                prevBright = thisBright;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (activateLED)
            {
                if (!serialPort1.IsOpen) return;
                serialPort1.Write("0");
            }
            else
            {
                if (!serialPort1.IsOpen) return;
                serialPort1.Write("1");
            }

            activateLED = !activateLED;
            button2.Text = (activateLED) ? "黑黑來了，快關燈！" : "好黑好黑，快開燈！";
        }
    }
}
