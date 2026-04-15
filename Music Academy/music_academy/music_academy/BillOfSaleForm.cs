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
    public partial class BillOfSaleForm : Form
    {
        MySqlConnection n = new MySqlConnection("Server = localhost; Database =musical_institute; Uid = root; Pwd =123456789;");

        public BillOfSaleForm()
        {
            InitializeComponent();
        }

        private void BillOfSaleForm_Load(object sender, EventArgs e)
        {
            LoadStudents();     // استدعاء دالة تعبئة الطلاب
            LoadInstruments();  // استدعاء دالة تعبئة الآلات
            LoadBills();        // استدعاء دالة عرض الجدول
        }

        // دالة مساعدة لتعبئة قائمة الطلاب
        void LoadStudents()
        {
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT id, name FROM student", n);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // ربط الكومبوبوكس بالبيانات
                cmbStudent.DataSource = dt;
                cmbStudent.DisplayMember = "name"; // الشيء الذي سيراه المستخدم (الاسم)
                cmbStudent.ValueMember = "id";     // القيمة المخفية التي سيتعامل معها الكود (الرقم)
                cmbStudent.SelectedIndex = -1;     // لجعله فارغاً في البداية
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // دالة مساعدة لتعبئة قائمة الآلات
        void LoadInstruments()
        {
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT id, name FROM instrument", n);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbInstrument.DataSource = dt;
                cmbInstrument.DisplayMember = "name";
                cmbInstrument.ValueMember = "id";
                cmbInstrument.SelectedIndex = -1;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // دالة مساعدة لعرض الفواتير في الجدول
        void LoadBills()
        {
            try
            {
                if (n.State == ConnectionState.Open) n.Close();
                n.Open();
                // استعلام يعرض أسماء الطلاب والآلات بدلاً من الأرقام ليكون الجدول مقروءاً
                string query = @"SELECT bill_of_sale.id, student.name AS Student_Name, instrument.name AS Instrument, bill_of_sale.date, bill_of_sale.price 
                                 FROM bill_of_sale 
                                 JOIN student ON bill_of_sale.student_id = student.id 
                                 JOIN instrument ON bill_of_sale.instrument_id = instrument.id";

                MySqlDataAdapter da = new MySqlDataAdapter(query, n);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                n.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // التحقق من تعبئة الحقول
            if (cmbStudent.SelectedIndex != -1 && cmbInstrument.SelectedIndex != -1 && txtPrice.Text != "")
            {
                try
                {
                    if (n.State == ConnectionState.Open) n.Close();
                    n.Open();

                    // 1. الخطوة الأولى: التحقق من الكمية المتوفرة
                    int instrumentId = Convert.ToInt32(cmbInstrument.SelectedValue);
                    int currentStock = 0;

                    MySqlCommand checkCmd = new MySqlCommand("SELECT size FROM instrument WHERE id = " + instrumentId, n);
                    object result = checkCmd.ExecuteScalar();

                    if (result != null && result.ToString() != "")
                    {
                        currentStock = Convert.ToInt32(result);
                    }

                    // إذا لم يعد هناك قطع (العدد 0 أو أقل)
                    if (currentStock <= 0)
                    {
                        MessageBox.Show("عذراً، هذه الآلة نفدت من المستودع (العدد 0) ولا يمكن بيعها!");
                        n.Close();
                        return; // إيقاف العملية والخروج
                    }

                    // 2. الخطوة الثانية: إضافة الفاتورة (عملية البيع)
                    int studentId = Convert.ToInt32(cmbStudent.SelectedValue);
                    string date = dateTimePicker1.Value.ToString("yyyy-MM-dd");
                    string price = txtPrice.Text;

                    string insertQuery = "INSERT INTO bill_of_sale (student_id, instrument_id, date, price) VALUES ('" + studentId + "', '" + instrumentId + "', '" + date + "', '" + price + "')";
                    MySqlCommand insertCmd = new MySqlCommand(insertQuery, n);
                    insertCmd.ExecuteNonQuery();

                    // 3. الخطوة الثالثة: إنقاص العدد في المخزن
                    // العدد الجديد = العدد القديم - 1
                    int newStock = currentStock - 1;
                    string updateQuery = "UPDATE instrument SET size = '" + newStock + "' WHERE id = " + instrumentId;
                    MySqlCommand updateCmd = new MySqlCommand(updateQuery, n);
                    updateCmd.ExecuteNonQuery();

                    n.Close();

                    MessageBox.Show("تمت عملية البيع وتحديث المخزون بنجاح");

                    // تحديث الواجهة
                    LoadBills(); // تحديث جدول الفواتير

                    // تحديث العدد الظاهر في الـ Label ليرى المستخدم الرقم الجديد فوراً
                    lblStock.Text = "العدد المتوفر: " + newStock.ToString();
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
                MessageBox.Show("الرجاء اختيار الطالب والآلة وتحديد السعر");
            }

        }

        private void cmbInstrument_SelectedIndexChanged(object sender, EventArgs e)
        {
            // نتأكد أن القيمة المختارة هي رقم صحيح (لتجنب الأخطاء أثناء تحميل الواجهة)
            if (cmbInstrument.SelectedIndex != -1 && cmbInstrument.SelectedValue is int)
            {
                try
                {
                    // إغلاق الاتصال إذا كان مفتوحاً
                    if (n.State == ConnectionState.Open) n.Close();
                    n.Open();

                    // جلب قيمة size بناءً على رقم الآلة المختار
                    string query = "SELECT size FROM instrument WHERE id = " + cmbInstrument.SelectedValue;
                    MySqlCommand cmd = new MySqlCommand(query, n);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        lblStock.Text = "العدد المتوفر: " + result.ToString();
                    }

                    n.Close();
                }
                catch (Exception ex)
                {
                    // لا نظهر رسالة خطأ هنا كي لا نزعج المستخدم أثناء التنقل السريع
                }
                finally
                {
                    if (n.State == ConnectionState.Open) n.Close();
                }
            }
        }
    }
}
