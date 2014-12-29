/* FileName: Form1.cs
Project Name: SaveImageDB
Date Created: 12/20/2014 7:20:08 PM
Description: Auto-generated
Version: 1.0.0.0
Author:	Lê Thanh Tuấn - Khoa CNTT
Author Email: tuanitpro@gmail.com
Author Mobile: 0976060432
Author URI: http://tuanitpro.com
License: 

*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.SqlClient;
using System.IO;
namespace SaveImageToDB
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string sqlConnectionString = @"Data Source = .\SQLEXPRESS; Initial Catalog = SaveImage; User ID=sa; Password=sa";
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Chọn file cho PictureBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "JPG|*.jpg|PNG|*.png|GIF|*.gif|All Files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
               
                pictureBox1.ImageLocation = openFileDialog1.FileName;
                txtImage.Text = openFileDialog1.FileName;
                openFileDialog1.Dispose();
            }
        }
        /// <summary>
        /// Hàm thêm sinh viên vào CSDL
        /// </summary>
        /// <param name="ten">Tên sinh viên</param>
        /// <param name="fileName">Đường dẫn file</param>
        /// <returns>Số dòng bị ảnh hưởng, > 0 thành công, ngược lại thất bại.</returns>
        int AddStudent(string ten, string fileName)
        {
            byte[] imageData = null;
            // Đọc file chuyển sang mảng
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                imageData = new Byte[fs.Length];
                fs.Read(imageData, 0, (int)fs.Length);
            }

            using (SqlConnection conn = new SqlConnection(sqlConnectionString))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO STUDENT(NAME, IMAGE) values(@Name, @Image)", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Name", ten);
                cmd.Parameters["@Name"].Direction = ParameterDirection.Input;
                cmd.Parameters.Add("@Image", SqlDbType.Image);
                cmd.Parameters["@Image"].Direction = ParameterDirection.Input;
                // Lưu trữ mảng byte vào cột Image
                cmd.Parameters["@Image"].Value = imageData;
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int rs = AddStudent(txtName.Text, txtImage.Text);
            if (rs > 0)
            {
                MessageBox.Show("OK");
                LoadData();
            }
            else MessageBox.Show("ERROR");
        }
        /// <summary>
        /// Lấy danh sách sinh viên
        /// </summary>
        /// <returns></returns>
        public void LoadData()
        {
            using (SqlConnection conn = new SqlConnection(sqlConnectionString))
            {
                DataTable table = new System.Data.DataTable();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Student", conn);
                cmd.CommandType = CommandType.Text;
                conn.Open();
                table.Load(cmd.ExecuteReader());
                conn.Close();

                dataGridView1.DataSource = table;
            }
        }
         
        private void btnLoadData_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtName.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();

            Image myImage = ByteArrayToImage((byte[])dataGridView1.CurrentRow.Cells[2].Value);
            pictureBox1.Image = myImage;
        }

        /// <summary>
        /// Hàm chuyển byte sang Image
        /// </summary>
        /// <param name="byteArrayIn"></param>
        /// <returns></returns>
        public Image ByteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
    }
}
