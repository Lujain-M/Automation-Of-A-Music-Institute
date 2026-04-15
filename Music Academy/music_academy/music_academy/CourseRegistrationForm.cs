using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace music_academy
{
    public partial class CourseRegistrationForm : Form
    {
        MySqlConnection n = new MySqlConnection("Server=localhost;Database=musical_institute;Uid=root;Pwd=123456789;");

        // متغير عالمي لحفظ السعر لاستخدامه لاحقاً عند الحفظ
        int currentCoursePrice = 0;

        public CourseRegistrationForm()
        {
            InitializeComponent();
        }

        private void CourseRegistrationForm_Load(object sender, EventArgs e)
        {
            LoadStudents();
            LoadRegistrations();

            // تنظيف الكومبوبوكس الخاص بالكورسات في البداية
            cmbCourse.DataSource = null;
            lblPrice.Text = "السعر: -";
        }

        // 1. تحميل قائمة الطلاب فقط
        void LoadStudents()
        {
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT id, name FROM student", n);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // نفصل الرابط هنا مؤقتاً لتجنب تفعيل الأحداث أثناء التحميل
                cmbStudent.SelectedIndexChanged -= cmbStudent_SelectedIndexChanged;

                cmbStudent.DataSource = dt;
                cmbStudent.DisplayMember = "name";
                cmbStudent.ValueMember = "id";
                cmbStudent.SelectedIndex = -1;

                // نعيد ربط الحدث
                cmbStudent.SelectedIndexChanged += new EventHandler(cmbStudent_SelectedIndexChanged);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // 2. الحدث الأهم: عند اختيار طالب، نبحث عن الكورسات المناسبة له
        private void cmbStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbStudent.SelectedIndex != -1 && cmbStudent.SelectedValue is int)
            {
                try
                {
                    if (n.State == ConnectionState.Open) n.Close();
                    n.Open();

                    // أ) جلب معلومات الطالب (الآلة والمستوى)
                    string studentId = cmbStudent.SelectedValue.ToString();
                    string studentInstrument = "";
                    string studentLevel = "";

                    MySqlCommand cmdInfo = new MySqlCommand("SELECT instrument, level FROM student WHERE id = " + studentId, n);
                    MySqlDataReader reader = cmdInfo.ExecuteReader();
                    if (reader.Read())
                    {
                        studentInstrument = reader["instrument"].ToString();
                        studentLevel = reader["level"].ToString();
                    }
                    reader.Close();

                    // ب) جلب الكورسات التي تطابق هذه المعلومات فقط
                    // الشرط: اسم الكورس يطابق آلة الطالب، ومستوى الكورس يطابق مستوى الطالب
                    string courseQuery = "SELECT id, name, price FROM course WHERE name = '" + studentInstrument + "' AND level = '" + studentLevel + "'";

                    MySqlDataAdapter daCourse = new MySqlDataAdapter(courseQuery, n);
                    DataTable dtCourse = new DataTable();
                    daCourse.Fill(dtCourse);

                    n.Close();

                    // ج) تعبئة قائمة الكورسات بالنتائج
                    if (dtCourse.Rows.Count > 0)
                    {
                        // نفصل حدث تغيير الكورس مؤقتاً
                        cmbCourse.SelectedIndexChanged -= cmbCourse_SelectedIndexChanged;

                        cmbCourse.DataSource = dtCourse;
                        cmbCourse.DisplayMember = "name"; // سيعرض اسم الآلة
                        cmbCourse.ValueMember = "id";
                        cmbCourse.SelectedIndex = -1; // لا نختار شيء تلقائياً
                        lblPrice.Text = "السعر: -";   // تصفير السعر
                        currentCoursePrice = 0;

                        // نعيد ربط الحدث
                        cmbCourse.SelectedIndexChanged += new EventHandler(cmbCourse_SelectedIndexChanged);
                    }
                    else
                    {
                        // إذا لم نجد كورسات مناسبة
                        cmbCourse.DataSource = null;
                        lblPrice.Text = "السعر: -";
                        MessageBox.Show("لا يوجد كورسات متاحة حالياً لآلة (" + studentInstrument + ") بالمستوى (" + studentLevel + ")");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطأ في جلب الكورسات: " + ex.Message);
                }
                finally { if (n.State == ConnectionState.Open) n.Close(); }
            }
        }

        // 3. عند اختيار كورس (من القائمة المفلترة)، نعرض سعره
        private void cmbCourse_SelectedIndexChanged(object sender, EventArgs e)
        {
            // نتأكد أن هناك عنصر تم اختياره فعلياً
            if (cmbCourse.SelectedIndex != -1 && cmbCourse.DataSource != null)
            {
                try
                {
                    // بما أننا جلبنا السعر سابقاً في الـ DataTable (في الخطوة السابقة)
                    // يمكننا أخذه مباشرة دون الاتصال بقاعدة البيانات مرة أخرى!

                    DataRowView row = (DataRowView)cmbCourse.SelectedItem;
                    currentCoursePrice = Convert.ToInt32(row["price"]);

                    lblPrice.Text =  currentCoursePrice.ToString();
                }
                catch (Exception ex)
                {
                    // في حال حدث خطأ في التحويل، نحاول جلبه من القاعدة كحل بديل
                    FetchPriceFromDB();
                }
            }
        }

        // دالة احتياطية لجلب السعر من القاعدة (تحل مشكلة اختفاء السعر)
        void FetchPriceFromDB()
        {
            try
            {
                if (n.State == ConnectionState.Open) n.Close();
                n.Open();
                string query = "SELECT price FROM course WHERE id = " + cmbCourse.SelectedValue;
                MySqlCommand cmd = new MySqlCommand(query, n);
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    currentCoursePrice = Convert.ToInt32(result);
                    lblPrice.Text =   currentCoursePrice.ToString();
                }
                n.Close();
            }
            catch { }
        }

        // 4. زر الحفظ (كما هو، مع التأكد من وجود currentCoursePrice)
        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (cmbStudent.SelectedIndex != -1 && cmbCourse.SelectedIndex != -1 && currentCoursePrice > 0)
            {
                try
                {
                    if (n.State == ConnectionState.Open) n.Close();
                    n.Open();

                    // إدخال الفاتورة
                    string date = dateTimePicker1.Value.ToString("yyyy-MM-dd");
                    string insertBill = "INSERT INTO bill (billdate, price) VALUES ('" + date + "', '" + currentCoursePrice + "')";
                    MySqlCommand cmdBill = new MySqlCommand(insertBill, n);
                    cmdBill.ExecuteNonQuery();
                    long lastBillId = cmdBill.LastInsertedId;

                    // إدخال التفاصيل
                    int studentId = Convert.ToInt32(cmbStudent.SelectedValue);
                    int courseId = Convert.ToInt32(cmbCourse.SelectedValue);

                    string insertDetails = "INSERT INTO details (bill_id, corse_id, student_id) VALUES ('" + lastBillId + "', '" + courseId + "', '" + studentId + "')";
                    MySqlCommand cmdDetails = new MySqlCommand(insertDetails, n);
                    cmdDetails.ExecuteNonQuery();

                    n.Close();
                    MessageBox.Show("تم التسجيل بنجاح");
                    LoadRegistrations();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                finally { if (n.State == ConnectionState.Open) n.Close(); }
            }
            else
            {
                MessageBox.Show("تأكد من اختيار الطالب والكورس وظهور السعر");
            }
        }

        void LoadRegistrations()
        {
            // (نفس كود عرض الجدول السابق)
            try
            {
                string query = @"SELECT bill.id AS Bill_Num, bill.billdate, student.name AS Student, course.name AS Course, bill.price 
                                 FROM details
                                 JOIN bill ON details.bill_id = bill.id
                                 JOIN student ON details.student_id = student.id
                                 JOIN course ON details.corse_id = course.id
                                 ORDER BY bill.id DESC";

                MySqlDataAdapter da = new MySqlDataAdapter(query, n);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}