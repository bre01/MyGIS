using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConsoleApp1;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private Student[] students;
        public Form1()
        {
            InitializeComponent();
            students = new Student[5];
            students[0] = new Student("1001", "tim", false);
            students[1] = new Undergraduate("1003", "xie", false, 1, 4) { department = "CS" };
            students[2] = new Graduate("1004", "li", false, 1, 3) { department = "SE", Tutor = "ZhaoFei" };
            textBox1.Text = students[0].Class.ToString();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
