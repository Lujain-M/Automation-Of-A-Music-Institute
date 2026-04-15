using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace music_academy
{
    public partial class classroom : Form
    {
        MySqlConnection n = new MySqlConnection("Server = localhost; Database =musical_institute; Uid = root; Pwd =123456789;");
        public classroom()
        {
            InitializeComponent();
        }

        private void classroom_Load(object sender, EventArgs e)
        {
            n.Open();
            MySqlDataAdapter a = new MySqlDataAdapter("SELECT * FROM musical_institute.classroom", n);
            DataTable s = new DataTable();
            a.Fill(s);
            dataGridView1.DataSource = s;
            n.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                n.Open();
                MySqlCommand c = new MySqlCommand("INSERT INTO `musical_institute`.`classroom` (`name`, `size`, `notes`) VALUES ('" + textBox1.Text + "', '" + textBox2.Text + "', '" + richTextBox1.Text + "');");
                c.Connection = n;
                c.ExecuteNonQuery();
                MySqlDataAdapter b = new MySqlDataAdapter("SELECT * FROM musical_institute.classroom;", n);
                DataTable t = new DataTable();
                b.Fill(t);
                dataGridView1.DataSource = t;
                n.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                try
                {
                    n.Open();

                    // جملة التعديل الخاصة بالغرف
                    // نقوم بتحديث الاسم والحجم والملاحظات بناءً على رقم الغرفة (ID)
                    string query = "UPDATE musical_institute.classroom SET " +
                                   "`name` = '" + dataGridView1.CurrentRow.Cells[1].Value + "', " +
                                   "`size` = '" + dataGridView1.CurrentRow.Cells[2].Value + "', " +
                                   "`notes` = '" + dataGridView1.CurrentRow.Cells[3].Value + "' " +
                                   "WHERE (`id` = '" + dataGridView1.CurrentRow.Cells[0].Value + "');";

                    MySqlCommand c = new MySqlCommand(query, n);
                    c.ExecuteNonQuery();

                    // تحديث الجدول
                    MySqlDataAdapter b = new MySqlDataAdapter("SELECT * FROM musical_institute.classroom;", n);
                    DataTable t = new DataTable();
                    b.Fill(t);
                    dataGridView1.DataSource = t;

                    MessageBox.Show("تم تعديل بيانات الغرفة بنجاح");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطأ في التعديل: " + ex.Message + "\n تأكد من صحة البيانات المدخلة");
                }
                finally
                {
                    n.Close();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // التأكد من أن هناك سطر محدد للحذف أولاً لتجنب توقف البرنامج
            if (dataGridView1.CurrentRow != null)
            {
                n.Open();

                // تم تصحيح نص الاستعلام هنا بإزالة الرموز الزائدة
                string query = "DELETE FROM musical_institute.classroom WHERE id = '" + dataGridView1.CurrentRow.Cells[0].Value + "'";

                MySqlCommand c = new MySqlCommand(query, n); // مررنا الاتصال n مباشرة هنا للاختصار
                c.ExecuteNonQuery();

                // تحديث الجدول بعد الحذف
                MySqlDataAdapter b = new MySqlDataAdapter("SELECT * FROM musical_institute.classroom;", n);
                DataTable t = new DataTable();
                b.Fill(t);
                dataGridView1.DataSource = t;

                n.Close();

                MessageBox.Show("تم حذف القاعة بنجاح");
            }
            else
            {
                MessageBox.Show("الرجاء تحديد سجل القاعة للحذف");
            }
        }
    }
}
