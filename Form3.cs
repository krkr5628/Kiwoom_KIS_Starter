using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Auto_Run_Update : Form
    {
        public string filepath = "C:\\Users\\krkr5\\OneDrive\\바탕 화면\\project\\password\\system_setting.txt";
        public bool auto_run;
        public string program_start;
        public string program_stop;
        public bool load_complete = false;

        public Auto_Run_Update()
        {
            InitializeComponent();

            file_load();

            timer1.Start();

            //
            button1.Click += Button1_Click;
            button2.Click += Button2_Click;

        }

        private void Button1_Click(object sender, EventArgs e)
        {

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "파일 저장 경로 지정하세요";
            saveFileDialog.Filter = "텍스트 파일 (*.txt)|*.txt";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string textToSave = "자동실행/" + checkBox1.Checked.ToString() + "\n" + "자동운영시간/" + start_time_text.Text + "/" + end_time_text.Text;

                // 사용자가 선택한 파일 경로
                string filePath = saveFileDialog.FileName;

                //파일에 텍스트 저장
                System.IO.File.WriteAllText(filePath, textToSave);
            }
        }

        private void timetimer(object sender, EventArgs e)
        {
            //시간표시
            Time_label.Text = DateTime.Now.ToString("yy MM-dd (ddd) HH:mm:ss");

            //
            if (load_complete && auto_run) Opeartion_Time();
        }

        private void file_load()
        {
            StreamReader reader = new StreamReader(filepath);

            //자동실행
            String[] program_auto_run_allow = reader.ReadLine().Split('/');
            auto_run = Convert.ToBoolean(program_auto_run_allow[1]);
            checkBox1.Checked = auto_run;

            //자동 운영 시간
            String[] time_tmp = reader.ReadLine().Split('/');
            program_start = time_tmp[1];
            start_time_text.Text = time_tmp[1];
            program_stop = time_tmp[2];
            end_time_text.Text = time_tmp[2];

            reader.Close();
            //
            load_complete = true;
        }

        private bool isTradeAutoOpened = false;
        

        private void Opeartion_Time()
        {
            //운영시간 확인
            DateTime t_now = DateTime.Now;
            DateTime t_start = DateTime.Parse(program_start);
            DateTime t_end = DateTime.Parse(program_stop);

            //운영시간 아님
            if (!isTradeAutoOpened && t_now >= t_start && t_now <= t_end)
            {
                //
                isTradeAutoOpened = true;
                label7.Text = "실행";
            }
            else if (isTradeAutoOpened && t_now > t_end)
            {
                //
                isTradeAutoOpened = false;
                label7.Text = "종료";
            }
        }

    }
}
