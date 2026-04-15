using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace music_academy
{
    public partial class payment : Form
    {
        // تعريف الاتصال
        MySqlConnection n = new MySqlConnection("Server=localhost;Database=musical_institute;Uid=root;Pwd=123456789;");

        public payment()
        {
            InitializeComponent();
        }

        private void payment_Load(object sender, EventArgs e)
        {
            LoadTeachersList(); // تعبئة القائمة
            LoadPaymentsGrid(); // عرض الجدول باستخدام المنظار
        }

        // دالة لجلب أسماء المعلمين للقائمة المنسدلة
        void LoadTeachersList()
        {
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT id, name FROM teacher", n);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbTeachers.DataSource = dt;
                cmbTeachers.DisplayMember = "name";
                cmbTeachers.ValueMember = "id";
                cmbTeachers.SelectedIndex = -1;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // ------------------------------------------------------------------
        // التعديل تم هنا: استخدام View بدلاً من الاستعلام الطويل
        // ------------------------------------------------------------------
        void LoadPaymentsGrid()
        {
            try
            {
                if (n.State == ConnectionState.Open) n.Close();
                n.Open();

                // بدلاً من كتابة JOIN هنا، نستدعي المنظار الجاهز salary
                // يمكنك إضافة ORDER BY لترتيب النتائج حسب الأحدث
                string query = "SELECT * FROM salary ORDER BY id DESC";

                MySqlDataAdapter da = new MySqlDataAdapter(query, n);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;

                n.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        // ------------------------------------------------------------------

        // زر الإدخال
        private void button1_Click(object sender, EventArgs e)
        {
            if (cmbTeachers.SelectedIndex != -1 && textBox2.Text != "")
            {
                try
                {
                    if (n.State == ConnectionState.Open) n.Close();
                    n.Open();

                    int teacherId = Convert.ToInt32(cmbTeachers.SelectedValue);
                    string payDate = dateTimePicker1.Value.ToString("yyyy-MM-dd");
                    string salary = textBox2.Text;

                    string query = "INSERT INTO `musical_institute`.`payment` (`teacher_id`, `date`, `salary`) VALUES ('" + teacherId + "', '" + payDate + "', '" + salary + "');";

                    MySqlCommand c = new MySqlCommand(query, n);
                    c.ExecuteNonQuery();

                    MessageBox.Show("تم إضافة الراتب بنجاح");

                    // عند تحديث الجدول الآن، سيقوم بجلب البيانات الجديدة عبر المنظار تلقائياً
                    LoadPaymentsGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطأ: " + ex.Message);
                }
                finally
                {
                    if (n.State == ConnectionState.Open) n.Close();
                }
            }
            else
            {
                MessageBox.Show("الرجاء اختيار المدرس وإدخال قيمة الراتب");
            }
        }

        // بقية الأحداث الفارغة
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
    }
}