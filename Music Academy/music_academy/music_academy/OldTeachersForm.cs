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
    public partial class OldTeachersForm : Form
    {
        MySqlConnection n = new MySqlConnection("Server=localhost;Database=musical_institute;Uid=root;Pwd=123456789;");
        public OldTeachersForm()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void OldTeachersForm_Load(object sender, EventArgs e)
        {
            LoadArchivedTeachers();
        }

        void LoadArchivedTeachers()
        {
            try
            {
                if (n.State == ConnectionState.Open) n.Close();
                n.Open();

                // جلب البيانات من جدول الأرشيف وترتيبها حسب الأحدث حذفاً
                string query = "SELECT id, name AS 'Teacher Name', phone, instrument, deletion_date AS 'Deleted At' FROM old_teacher ORDER BY deletion_date DESC";

                MySqlDataAdapter da = new MySqlDataAdapter(query, n);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;

                // تنسيق عرض التاريخ (اختياري)
                dataGridView1.Columns["Deleted At"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";

                n.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ: " + ex.Message);
            }
        }
    }
}
