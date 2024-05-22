using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Auto_Run_Update : Form
    {
        public string filepath_system = "C:\\Auto_Trade_Kiwoom\\system_setting.txt";
        public string filepath_run = "C:\\Auto_Trade_Kiwoom\\Auto_Trade_Kiwoom_Main\\Trade_Auto_Kiwoom.exe";
        public bool auto_run;
        public string program_start;
        public string program_stop;
        public bool load_complete = false;

        public Auto_Run_Update()
        {
            InitializeComponent();

            File_load();

            timer1.Start();

            //
            button1.Click += Button1_Click;
            button2.Click += Button2_Click;
            button3.Click += Button3_Click;

        }

        private void File_load()
        {
            StreamReader reader = new StreamReader(filepath_system);

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

        private void Timetimer(object sender, EventArgs e)
        {
            //시간표시
            Time_label.Text = DateTime.Now.ToString("yy MM-dd (ddd) HH:mm:ss");

            //
            if (load_complete && auto_run) Opeartion_Time();
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
                isTradeAutoOpened = true;

                Button1_Click(null, EventArgs.Empty);

                label7.Text = "실행";
            }
            else if (isTradeAutoOpened && t_now > t_end)
            {
                isTradeAutoOpened = false;
                //
                Button3_Click(null, EventArgs.Empty);
                //
                label7.Text = "종료";
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                WriteLog_System("키움 프로세스를 실행합니다.\n");
                Process.Start(filepath_run);
                label7.Text = "실행";
            }
            catch (Exception ex)
            {
                WriteLog_System("키움 매매 프로그램을 올바른 위치에 설치해주세요.\n");
                WriteLog_System("Error message: " + ex.Message + "\n");
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "파일 저장 경로 지정하세요",
                Filter = "텍스트 파일 (*.txt)|*.txt"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string textToSave = "자동실행/" + checkBox1.Checked.ToString() + "\n" + "자동운영시간/" + start_time_text.Text + "/" + end_time_text.Text;

                // 사용자가 선택한 파일 경로
                string filePath = saveFileDialog.FileName;

                //파일에 텍스트 저장
                System.IO.File.WriteAllText(filePath, textToSave);

                WriteLog_System("파일이 저장되었습니다.\n");
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            // 프로세스 목록에서 filepath_run에 해당하는 프로세스 찾기
            Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(filepath_run));

            // 각 프로세스에 대해 종료 요청
            foreach (Process process in processes)
            {
                try
                {
                    WriteLog_System("키움 프로세스를 종료합니다.\n");
                    // 메인 창을 닫기 위해 요청
                    if (process.CloseMainWindow())
                    {
                        // 창이 닫히는 시간을 기다림
                        process.WaitForExit(10000); // 최대 10초 대기
                        if (!process.HasExited)
                        {
                            // 여전히 종료되지 않은 경우 강제 종료
                            WriteLog_System("키움 프로세스가 직접 종료하십시요.\n");
                        }
                        else
                        {
                            process.Close();
                            WriteLog_System("키움 프로세스가 정상 종료되었습니다.\n");
                            label7.Text = "종료";
                        }
                    }
                    else
                    {
                        WriteLog_System("키움 프로세스를 찾을 수 없습니다.\n");
                    }
                }
                catch (Exception ex)
                {
                    WriteLog_System($"키움 프로세스 종료 오류 발생: {ex.Message}\n");
                }
            }
        }

        private void WriteLog_System(string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            richTextBox1.AppendText($@"{"[" + time + "] " + message}");
        }
    }
}
