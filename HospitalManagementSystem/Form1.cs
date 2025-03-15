using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace HospitalManagementSystem
{
    public partial class Form1 : Form
    {

        string connectionString = "server=localhost;database=hospitaldb;user=root;password=123456;";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TestMySQLConnection();
            LoadPatients();
        }

        private void TestMySQLConnection()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MessageBox.Show("Connection Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Connection Failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadPatients()
        {
            
            string query = "SELECT * FROM Patients";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading patients: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnAddPatient_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO patients (Name, DOB, Gender, Phone, Email, Address) VALUES (@Name, @DOB, @Gender, @Phone, @Email, @Address)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", txtName.Text);
                        cmd.Parameters.AddWithValue("@DOB", dtpDOB.Value.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@Gender", cmbGender.SelectedItem?.ToString());
                        cmd.Parameters.AddWithValue("@Phone", txtPhone.Text);
                        cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                        cmd.Parameters.AddWithValue("@Address", txtAddress.Text);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Patient added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadPatients(); 
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding patient: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) 
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                txtName.Text = row.Cells["Name"].Value.ToString();
                dtpDOB.Value = Convert.ToDateTime(row.Cells["DOB"].Value);
                cmbGender.SelectedItem = row.Cells["Gender"].Value.ToString();
                txtPhone.Text = row.Cells["Phone"].Value.ToString();
                txtEmail.Text = row.Cells["Email"].Value.ToString();
                txtAddress.Text = row.Cells["Address"].Value.ToString();
            }
        }

        private void btnUpdatePatient_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0) 
            {
                int patientID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["PatientID"].Value);

                string name = txtName.Text;
                string dob = dtpDOB.Value.ToString("yyyy-MM-dd"); 
                string gender = cmbGender.SelectedItem.ToString();
                string phone = txtPhone.Text;
                string email = txtEmail.Text;
                string address = txtAddress.Text;

                string query = "UPDATE patients SET Name=@Name, DOB=@DOB, Gender=@Gender, Phone=@Phone, Email=@Email, Address=@Address WHERE PatientID=@PatientID";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@DOB", dob);
                        cmd.Parameters.AddWithValue("@Gender", gender);
                        cmd.Parameters.AddWithValue("@Phone", phone);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Address", address);
                        cmd.Parameters.AddWithValue("@PatientID", patientID);

                        try
                        {
                            conn.Open();
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Patient details updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadPatients();
                            }
                            else
                            {
                                MessageBox.Show("Update failed. No changes detected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message, "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a patient to update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void deletePatient_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0) 
            {
                int patientID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["PatientID"].Value);

                DialogResult result = MessageBox.Show("Are you sure you want to delete this patient?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    string query = "DELETE FROM patients WHERE PatientID = @PatientID";

                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@PatientID", patientID);

                            try
                            {
                                conn.Open();
                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Patient deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    LoadPatients(); 
                                }
                                else
                                {
                                    MessageBox.Show("Deletion failed.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error: " + ex.Message, "Delete Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a patient to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
