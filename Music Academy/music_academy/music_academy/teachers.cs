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
    public partial class teachers : Form
    {
        MySqlConnection n = new MySqlConnection("Server = localhost; Database =musical_institute; Uid = root; Pwd =123456789;");
        public teachers()
        {
            InitializeComponent();
        }
        private void teachers_Load(object sender, EventArgs e)
        {
            n.Open();
            MySqlDataAdapter a = new MySqlDataAdapter("SELECT * FROM musical_institute.teacher", n);
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
                MySqlCommand c = new MySqlCommand("INSERT INTO `musical_institute`.`teacher` (`name`, `phone`, `instrument`)  VALUES ('" + textBox1.Text + "', '" + textBox2.Text + "', '" + textBox3.Text + "');");
                c.Connection = n;
                c.ExecuteNonQuery();
                MySqlDataAdapter b = new MySqlDataAdapter("SELECT * FROM musical_institute.teacher;", n);
                DataTable t = new DataTable();
                b.Fill(t);
                dataGridView1.DataSource = t;
                n.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            n.Open();
            string query = "SELECT * FROM musical_institute.teacher WHERE " +
               "name LIKE '%" + textBox1.Text + "%' AND " +
               "instrument LIKE '%" + textBox3.Text + "%' AND " +
               "phone LIKE '%" + textBox2.Text + "%'";

            MySqlDataAdapter a = new MySqlDataAdapter(query, n);
            DataTable s = new DataTable();
            a.Fill(s);
            dataGridView1.DataSource = s;
            n.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // 1. تفريغ خانات البحث ليعرف المستخدم أن البحث انتهى
              textBox1.Text = "";
              textBox2.Text = "";
              textBox3.Text = "";
             

              // 2. جلب جميع البيانات مرة أخرى (بدون شرط Where)
              try
              {
                  // التأكد من أن الاتصال مغلق قبل فتحه لتجنب الأخطاء
                  if (n.State == ConnectionState.Open) n.Close();

                  n.Open();
                  MySqlDataAdapter a = new MySqlDataAdapter("SELECT * FROM musical_institute.teacher", n);
                  DataTable s = new DataTable();
                  a.Fill(s);
                  dataGridView1.DataSource = s;
              }
              catch (Exception ex)
              {
                  MessageBox.Show(ex.Message);
              }
              finally
              {
                  n.Close();
              }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            // التأكد من أن هناك سطر محدد للحذف
            if (dataGridView1.CurrentRow != null)
            {
                // رسالة تأكيد إضافية لأن العملية ستحذف سجلات مالية
                DialogResult dialogResult = MessageBox.Show("هل أنت متأكد؟ سيتم حذف المعلم وكافة سجلات رواتبه وكورساته نهائياً!", "تحذير هام", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        // نأخذ رقم المعلم في متغير لاستخدامه في عدة استعلامات
                        string teacherId = dataGridView1.CurrentRow.Cells[0].Value.ToString();

                        if (n.State == ConnectionState.Open) n.Close();
                        n.Open();

                        // 1. حذف الرواتب المرتبطة بهذا المعلم أولاً
                        string deletePayments = "DELETE FROM musical_institute.payment WHERE teacher_id = '" + teacherId + "'";
                        MySqlCommand cmd1 = new MySqlCommand(deletePayments, n);
                        cmd1.ExecuteNonQuery();

                        // 2. حذف الكورسات المرتبطة بهذا المعلم ثانياً
                        // (ملاحظة: إذا كان للكورسات طلاب مسجلين فيها، قد تحتاج لحذف التفاصيل details أيضاً، لكن سنكتفي بهذا الآن)
                        string deleteCourses = "DELETE FROM musical_institute.course WHERE teacher_id = '" + teacherId + "'";
                        MySqlCommand cmd2 = new MySqlCommand(deleteCourses, n);
                        cmd2.ExecuteNonQuery();

                        // 3. أخيراً.. حذف المعلم نفسه
                        string deleteTeacher = "DELETE FROM musical_institute.teacher WHERE id = '" + teacherId + "'";
                        MySqlCommand cmd3 = new MySqlCommand(deleteTeacher, n);
                        cmd3.ExecuteNonQuery();

                        // تحديث الجدول
                        MySqlDataAdapter b = new MySqlDataAdapter("SELECT * FROM musical_institute.teacher;", n);
                        DataTable t = new DataTable();
                        b.Fill(t);
                        dataGridView1.DataSource = t;

                        MessageBox.Show("تم حذف سجلات المعلم بنجاح");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("حدث خطأ: " + ex.Message);
                    }
                    finally
                    {
                        n.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("الرجاء تحديد مدرس للحذف");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                try
                {
                    n.Open();

                    // جملة التعديل الخاصة بالمدرسين
                    // نقوم بتحديث الاسم والهاتف والآلة بناءً على رقم المدرس (ID)
                    string query = "UPDATE musical_institute.teacher SET " +
                                   "`name` = '" + dataGridView1.CurrentRow.Cells[1].Value + "', " +
                                   "`phone` = '" + dataGridView1.CurrentRow.Cells[2].Value + "', " +
                                   "`instrument` = '" + dataGridView1.CurrentRow.Cells[3].Value + "' " +
                                   "WHERE (`id` = '" + dataGridView1.CurrentRow.Cells[0].Value + "');";

                    MySqlCommand c = new MySqlCommand(query, n);
                    c.ExecuteNonQuery();

                    // تحديث الجدول
                    MySqlDataAdapter b = new MySqlDataAdapter("SELECT * FROM musical_institute.teacher;", n);
                    DataTable t = new DataTable();
                    b.Fill(t);
                    dataGridView1.DataSource = t;

                    MessageBox.Show("تم تعديل بيانات المدرس بنجاح");
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
    }
}
