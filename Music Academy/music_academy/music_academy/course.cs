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
    public partial class course : Form
    {
        int tt;
        MySqlConnection n = new MySqlConnection("Server = localhost; Database =musical_institute; Uid = root; Pwd =123456789;");
        public course()
        {
            InitializeComponent();
        }

        private void course_Load(object sender, EventArgs e)
        {
            LoadInstruments(); // تعبئة قائمة أسماء الكورسات
            LoadRooms();       // تعبئة قائمة الغرف
            FillLevels();      // تعبئة المستويات يدوياً
            LoadCoursesGrid(); // عرض الجدول
        }

        // 1. تعبئة قائمة الآلات (لتكون هي أسماء الكورسات)
        void LoadInstruments()
        {
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT name FROM instrument", n);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // نفصل الحدث مؤقتاً لتجنب الأخطاء أثناء التحميل
                cmbInstrument.SelectedIndexChanged -= cmbInstrument_SelectedIndexChanged;

                cmbInstrument.DataSource = dt;
                cmbInstrument.DisplayMember = "name";
                cmbInstrument.ValueMember = "name"; // هنا القيمة هي الاسم نفسه
                cmbInstrument.SelectedIndex = -1;

                // نعيد ربط الحدث
                cmbInstrument.SelectedIndexChanged += new EventHandler(cmbInstrument_SelectedIndexChanged);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // 2. تعبئة قائمة الغرف
        void LoadRooms()
        {
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT id, name FROM classroom", n);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbRoom.DataSource = dt;
                cmbRoom.DisplayMember = "name"; // يظهر للمستخدم اسم القاعة
                cmbRoom.ValueMember = "id";     // يخزن للكود رقم القاعة
                cmbRoom.SelectedIndex = -1;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // 3. تعبئة المستويات (ثابتة)
        void FillLevels()
        {
            cmbLevel.Items.Clear();
            cmbLevel.Items.Add("أول");
            cmbLevel.Items.Add("ثاني");
            cmbLevel.Items.Add("ثالث");
        }


        // 4. الحدث الذكي: عند اختيار اسم الكورس (الآلة)، نجلب المدرسين المناسبين
        private void cmbInstrument_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbInstrument.SelectedIndex != -1)
            {
                try
                {
                    // اسم الآلة المختارة
                    // نستخدم Text أو SelectedValue بما أنهما متماثلان هنا
                    string selectedInstrument = cmbInstrument.Text;

                    if (n.State == ConnectionState.Open) n.Close();
                    n.Open();

                    // استعلام لجلب المدرسين الذين يدرسون هذه الآلة فقط
                    string query = "SELECT id, name FROM teacher WHERE instrument = '" + selectedInstrument + "'";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, n);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cmbTeacher.DataSource = dt;
                    cmbTeacher.DisplayMember = "name"; // يظهر اسم المدرس
                    cmbTeacher.ValueMember = "id";     // يخزن رقم المدرس
                    cmbTeacher.SelectedIndex = -1;

                    n.Close();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                finally { if (n.State == ConnectionState.Open) n.Close(); }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // التحقق من أن المستخدم اختار من القوائم
            if (cmbInstrument.SelectedIndex != -1 && cmbLevel.SelectedIndex != -1 &&
                cmbTeacher.SelectedIndex != -1 && cmbRoom.SelectedIndex != -1 && textBox5.Text != "")
            {
                try
                {
                    if (n.State == ConnectionState.Open) n.Close();
                    n.Open();

                    // تجهيز البيانات
                    string courseName = cmbInstrument.Text;       // اسم الكورس
                    string level = cmbLevel.Text;                 // المستوى
                    int teacherId = Convert.ToInt32(cmbTeacher.SelectedValue); // رقم المدرس (مخفي)
                    int roomId = Convert.ToInt32(cmbRoom.SelectedValue);       // رقم الغرفة (مخفي)
                    string price = textBox5.Text;

                    // جملة الإدخال (لاحظ البساطة الآن، لا نحتاج لبحث SELECT لأننا نملك الـ IDs جاهزة)
                    string query = "INSERT INTO `musical_institute`.`course` (`name`, `level`, `teacher_id`, `room_id`, `price`) VALUES ('" + courseName + "', '" + level + "', '" + teacherId + "', '" + roomId + "', '" + price + "')";

                    MySqlCommand c = new MySqlCommand(query, n);
                    c.ExecuteNonQuery();

                    MessageBox.Show("تم إضافة الكورس بنجاح");
                    LoadCoursesGrid(); // تحديث الجدول
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
                MessageBox.Show("الرجاء اختيار جميع الحقول المطلوبة");
            }
        }


        // دالة مساعدة لعرض الجدول (تظهر الأسماء بدلاً من الأرقام)
        void LoadCoursesGrid()
        {
            try
            {
                // نستخدم JOIN لعرض اسم المدرس واسم الغرفة بدلاً من أرقامهم
                string query = @"SELECT course.id, course.name, course.level, teacher.name AS Teacher, classroom.name AS Room, course.price 
                                 FROM course
                                 JOIN teacher ON course.teacher_id = teacher.id
                                 JOIN classroom ON course.room_id = classroom.id";

                MySqlDataAdapter da = new MySqlDataAdapter(query, n);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex) { }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (n.State == ConnectionState.Open) n.Close();
                n.Open();

                // 1. بداية جملة الاستعلام (نستخدم JOIN لنحصل على الأسماء)
                // وضعنا شرط 1=1 وهو حيلة برمجية لنتمكن من إضافة 'AND' بعدها بسهولة
                string query = @"SELECT course.id, course.name, course.level, teacher.name AS Teacher, classroom.name AS Room, course.price 
                         FROM course
                         JOIN teacher ON course.teacher_id = teacher.id
                         JOIN classroom ON course.room_id = classroom.id
                         WHERE 1=1 ";

                // 2. إضافة الشروط ديناميكياً حسب اختيار المستخدم

                // البحث باسم الكورس (الآلة)
                if (cmbInstrument.SelectedIndex != -1)
                {
                    query += " AND course.name LIKE '%" + cmbInstrument.Text + "%'";
                }

                // البحث بالمستوى
                if (cmbLevel.SelectedIndex != -1)
                {
                    query += " AND course.level LIKE '%" + cmbLevel.Text + "%'";
                }

                // البحث باسم المدرس (نستخدم ID المدرس المخفي في القائمة)
                if (cmbTeacher.SelectedIndex != -1)
                {
                    query += " AND course.teacher_id = " + cmbTeacher.SelectedValue;
                }

                // البحث برقم الغرفة (نستخدم ID الغرفة المخفي في القائمة)
                if (cmbRoom.SelectedIndex != -1)
                {
                    query += " AND course.room_id = " + cmbRoom.SelectedValue;
                }

                // البحث بالسعر
                if (textBox5.Text != "")
                {
                    query += " AND course.price LIKE '%" + textBox5.Text + "%'";
                }

                // 3. تنفيذ الاستعلام
                MySqlDataAdapter a = new MySqlDataAdapter(query, n);
                DataTable s = new DataTable();
                a.Fill(s);
                dataGridView1.DataSource = s;
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في البحث: " + ex.Message);
            }
            finally
            {
                n.Close();
            }
        }

        

        private void button3_Click_1(object sender, EventArgs e)
        {
            // تفريغ القوائم والنصوص
            cmbInstrument.SelectedIndex = -1;
            cmbLevel.SelectedIndex = -1;
            cmbTeacher.SelectedIndex = -1;
            // ملاحظة: تفريغ المدرس قد يكون تلقائياً بسبب تفريغ الآلة، لكن لا ضرر من تأكيده
            cmbRoom.SelectedIndex = -1;
            textBox5.Text = "";

            // إعادة تحميل الجدول باستخدام الدالة التي كتبتها أنت مسبقاً
            LoadCoursesGrid();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(dataGridView1.CurrentRow != null)
    {
                try
                {
                    if (n.State == ConnectionState.Open) n.Close();
                    n.Open();

                    // 1. جلب البيانات الأصلية (كأسماء) لمقارنتها مع الجدول
                    // نستخدم JOIN هنا أيضاً لنقارن "أحمد" بـ "أحمد" وليس "أحمد" بـ "1"
                    string checkQuery = @"SELECT course.name, teacher.name AS TeacherName, classroom.name AS RoomName 
                                  FROM course 
                                  JOIN teacher ON course.teacher_id = teacher.id
                                  JOIN classroom ON course.room_id = classroom.id
                                  WHERE course.id = '" + dataGridView1.CurrentRow.Cells[0].Value + "'";

                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, n);
                    MySqlDataReader reader = checkCmd.ExecuteReader();

                    bool isForbiddenChanged = false;

                    if (reader.Read())
                    {
                        // القيم الأصلية في قاعدة البيانات
                        string dbCourseName = reader["name"].ToString();
                        string dbTeacherName = reader["TeacherName"].ToString();
                        string dbRoomName = reader["RoomName"].ToString();

                        // القيم الموجودة حالياً في الجدول (Grid)
                        // الترتيب حسب استعلام LoadCoursesGrid هو:
                        // 0:id, 1:name, 2:level, 3:Teacher, 4:Room, 5:price
                        string gridCourseName = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                        string gridTeacherName = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                        string gridRoomName = dataGridView1.CurrentRow.Cells[4].Value.ToString();

                        // المقارنة
                        if (dbCourseName != gridCourseName || dbTeacherName != gridTeacherName || dbRoomName != gridRoomName)
                        {
                            isForbiddenChanged = true;
                        }
                    }
                    reader.Close();

                    // 2. التحقق
                    if (isForbiddenChanged)
                    {
                        MessageBox.Show("لا يمكن تعديل اسم الكورس أو المدرس أو الغرفة.\nفقط المستوى والسعر مسموح بتعديلهما.");
                        LoadCoursesGrid(); // إعادة البيانات لأصلها
                        n.Close();
                        return;
                    }

                    // 3. التنفيذ (تعديل المستوى والسعر فقط)
                    string query = "UPDATE musical_institute.course SET " +
                                   "`level` = '" + dataGridView1.CurrentRow.Cells[2].Value + "', " +
                                   "`price` = '" + dataGridView1.CurrentRow.Cells[5].Value + "' " +
                                   "WHERE (`id` = '" + dataGridView1.CurrentRow.Cells[0].Value + "');";

                    MySqlCommand c = new MySqlCommand(query, n);
                    c.ExecuteNonQuery();

                    MessageBox.Show("تم تعديل بيانات الكورس بنجاح");
                    LoadCoursesGrid(); // تحديث الجدول
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
        }
    }
}
