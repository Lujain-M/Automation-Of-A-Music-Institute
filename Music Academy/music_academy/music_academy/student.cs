using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace music_academy
{
    public partial class student : Form
    {
        // تعريف الاتصال (تأكد من كلمة المرور الخاصة بك)
        MySqlConnection n = new MySqlConnection("Server=localhost;Database=musical_institute;Uid=root;Pwd=123456789;");

        public student()
        {
            InitializeComponent();
        }

        // --- حدث تحميل الواجهة ---
        private void student_Load(object sender, EventArgs e)
        {
            LoadAvailableInstruments(); // تعبئة الآلات
            FillLevels();               // تعبئة المستويات
            LoadStudentsGrid();         // عرض البيانات
        }

        // دالة لجلب أسماء الآلات من جدول الكورسات
        void LoadAvailableInstruments()
        {
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT DISTINCT name FROM course", n);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbInstrument.DataSource = dt;
                cmbInstrument.DisplayMember = "name";
                cmbInstrument.ValueMember = "name";
                cmbInstrument.SelectedIndex = -1; // فارغ في البداية
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // دالة لتعبئة المستويات يدوياً
        void FillLevels()
        {
            cmbLevel.Items.Clear();
            cmbLevel.Items.Add("أول");
            cmbLevel.Items.Add("ثاني");
            cmbLevel.Items.Add("ثالث");
        }

        // دالة لتحديث الجدول
        void LoadStudentsGrid()
        {
            try
            {
                if (n.State == ConnectionState.Open) n.Close();
                n.Open();
                MySqlDataAdapter a = new MySqlDataAdapter("SELECT * FROM musical_institute.student", n);
                DataTable s = new DataTable();
                a.Fill(s);
                dataGridView1.DataSource = s;
                n.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // --- زر البحث ---
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (n.State == ConnectionState.Open) n.Close();
                n.Open();

                // البحث باستخدام النصوص الموجودة في القوائم ومربعات النص
                string query = "SELECT * FROM musical_institute.student WHERE " +
                       "name LIKE '%" + textBox1.Text + "%' AND " +
                       "instrument LIKE '%" + cmbInstrument.Text + "%' AND " +
                       "level LIKE '%" + cmbLevel.Text + "%' AND " +
                       "phone LIKE '%" + textBox4.Text + "%' AND " +
                       "parent_phone LIKE '%" + textBox5.Text + "%'";

                MySqlDataAdapter a = new MySqlDataAdapter(query, n);
                DataTable s = new DataTable();
                a.Fill(s);
                dataGridView1.DataSource = s;
                n.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { if (n.State == ConnectionState.Open) n.Close(); }
        }

        // --- زر الإدخال ---
        private void button1_Click(object sender, EventArgs e)
        {
            // التحقق من تعبئة الحقول والقوائم
            if (textBox1.Text != "" && cmbInstrument.SelectedIndex != -1 && cmbLevel.SelectedIndex != -1)
            {
                try
                {
                    if (n.State == ConnectionState.Open) n.Close();
                    n.Open();

                    string query = "INSERT INTO `musical_institute`.`student` (`name`, `instrument`, `level`, `phone`, `parent_phone`) VALUES ('" + textBox1.Text + "', '" + cmbInstrument.Text + "', '" + cmbLevel.Text + "', '" + textBox4.Text + "', '" + textBox5.Text + "');";

                    MySqlCommand c = new MySqlCommand(query, n);
                    c.ExecuteNonQuery();

                    MessageBox.Show("تمت الإضافة بنجاح");
                    LoadStudentsGrid();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                finally { if (n.State == ConnectionState.Open) n.Close(); }
            }
            else
            {
                MessageBox.Show("الرجاء إدخال الاسم واختيار الآلة والمستوى");
            }
        }

        // --- زر الحذف ---
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                try
                {
                    if (n.State == ConnectionState.Open) n.Close();
                    n.Open();

                    string query = "DELETE FROM musical_institute.student WHERE id = '" + dataGridView1.CurrentRow.Cells[0].Value + "'";

                    MySqlCommand c = new MySqlCommand(query, n);
                    c.ExecuteNonQuery();

                    MessageBox.Show("تم حذف الطالب بنجاح");
                    LoadStudentsGrid();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                finally { if (n.State == ConnectionState.Open) n.Close(); }
            }
            else
            {
                MessageBox.Show("الرجاء تحديد طالب للحذف");
            }
        }

        // --- زر التعديل (المنطق المطلوب) ---
        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                try
                {
                    if (n.State == ConnectionState.Open) n.Close();
                    n.Open();

                    // 1. تحديد القيم الجديدة
                    // الاسم والهواتف نأخذها من الجدول مباشرة (Grid) كما طلبت
                    string newName = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                    string newPhone = dataGridView1.CurrentRow.Cells[4].Value.ToString();
                    string newParentPhone = dataGridView1.CurrentRow.Cells[5].Value.ToString();

                    // الآلة والمستوى:
                    // إذا قام المستخدم باختيار قيمة من القائمة المنسدلة، نأخذها.
                    // إذا ترك القائمة فارغة، نأخذ القيمة القديمة الموجودة في الجدول حتى لا نمسح البيانات بالخطأ.
                    string newInstrument = (cmbInstrument.SelectedIndex != -1) ? cmbInstrument.Text : dataGridView1.CurrentRow.Cells[2].Value.ToString();
                    string newLevel = (cmbLevel.SelectedIndex != -1) ? cmbLevel.Text : dataGridView1.CurrentRow.Cells[3].Value.ToString();

                    // 2. تنفيذ التعديل
                    string query = "UPDATE musical_institute.student SET " +
                                   "`name` = '" + newName + "', " +
                                   "`instrument` = '" + newInstrument + "', " +
                                   "`level` = '" + newLevel + "', " +
                                   "`phone` = '" + newPhone + "', " +
                                   "`parent_phone` = '" + newParentPhone + "' " +
                                   "WHERE (`id` = '" + dataGridView1.CurrentRow.Cells[0].Value + "');";

                    MySqlCommand c = new MySqlCommand(query, n);
                    c.ExecuteNonQuery();

                    MessageBox.Show("تم التعديل بنجاح");
                    LoadStudentsGrid();

                    // نقوم بتفريغ القوائم بعد التعديل لترتيب الواجهة
                    cmbInstrument.SelectedIndex = -1;
                    cmbLevel.SelectedIndex = -1;
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                finally { if (n.State == ConnectionState.Open) n.Close(); }
            }
        }

        // --- زر التحديث / عرض الكل ---
        private void button5_Click(object sender, EventArgs e)
        {
            // تفريغ كل الخانات والقوائم
            textBox1.Text = "";
            cmbInstrument.SelectedIndex = -1;
            cmbLevel.SelectedIndex = -1;
            textBox4.Text = "";
            textBox5.Text = "";

            LoadStudentsGrid();
        }

        // دوال الأحداث الفارغة (يمكنك حذفها إذا لم تكن مرتبطة في التصميم)
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void textBox5_TextChanged(object sender, EventArgs e) { }
    }
}