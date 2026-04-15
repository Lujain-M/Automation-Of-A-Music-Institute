using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace music_academy
{
    public partial class main : Form
    {
        public main()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            teachers t = new teachers();
            t.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            student s = new student();
            s.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            course c = new course();
            c.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            classroom clr = new classroom();
            clr.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            payment p = new payment();
            p.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnOpenBillOfSale_Click(object sender, EventArgs e)
        {
            // إنشاء نسخة جديدة من واجهة فواتير البيع
            BillOfSaleForm b = new BillOfSaleForm();

            // إظهار الواجهة
            b.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CourseRegistrationForm frm = new CourseRegistrationForm();
            frm.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            OldTeachersForm frm = new OldTeachersForm();
            frm.Show();
        }
    }
}
