using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ezexternal
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (csgo_opened())
            {
                memory.handlecsgo();
            }
        }

        static bool csgo_opened()
        {
            foreach (Process csgo in Process.GetProcessesByName("csgo"))
            {
                return true;
            }

            MessageBox.Show("please start csgo", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        public void Run()
        {
            int localplayer = memory.Read<int>(memory.client + signatures.dwLocalPlayer);
            int localteam = memory.Read<int>(localplayer + netvars.m_iTeamNum);

            int crossid = memory.Read<int>(localplayer + netvars.m_iCrosshairId);

            if (checkBox1.Checked)
            {
                if (triggerbot(localteam, crossid) == true)
                {
                    memory.Write<int>(memory.client + signatures.dwForceAttack, 6);
                }
            }

            Thread.Sleep(trackBar1.Value);
        }

        public bool triggerbot(int localteam, int crossid)
        {
            int entity = memory.Read<int>(memory.client + signatures.dwEntityList + (crossid - 1) * 0x10);
            int entity_Team = memory.Read<int>(entity + netvars.m_iTeamNum);

            if (localteam != entity_Team && !checkBox2.Checked)/*enemy*/ return false;
            if (localteam == entity_Team && !checkBox3.Checked)/*teammate*/ return false;

            return (crossid > 0 && crossid < 65);
        }
        
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            new Thread(() =>
            {
                while (true)
                {
                    Run();
                    Thread.Sleep(trackBar1.Value);
                }
            }).Start();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
