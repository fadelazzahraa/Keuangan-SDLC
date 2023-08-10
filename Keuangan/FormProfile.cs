using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Keuangan
{
    public partial class FormProfile : Form
    {
        private User user;
        public FormProfile(User loggedinUser)
        {
            user = loggedinUser;
            InitializeComponent();
        }

        private void ChangeLoadingState(bool stats)
        {
            if (stats)
            {
                groupBox2.Visible = false;
                groupBox1.Visible = false;
                label1.Visible = true;
                label1.Enabled = true;
                
            } else
            {
                groupBox2.Visible = true;
                groupBox1.Visible = true;
                label1.Visible = false;
                label1.Enabled = false;
            }
        }      

        private async void button2_Click(object sender, EventArgs e)
        {
            ChangeLoadingState(true);
            DialogResult result = MessageBox.Show("Do you want to change your username?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                ChangeLoadingState(true);
                try
                {
                    string username = textBox2.Text;

                    Dictionary<string, string> data = new Dictionary<string, string>
                    {
                        { "username", username },
                    };

                    string requestBody = System.Text.Json.JsonSerializer.Serialize(data);

                    string responseData = await Connection.PostAuthorizedDataAsync(Connection.postUserProfileURL, requestBody, user.Token);

                    Dictionary<string, object> responseDataDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseData);

                    MessageBox.Show(responseDataDictionary["message"].ToString());

                    user.Username = textBox2.Text;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error occurred while making the request: " + ex.Message);
                }

                /*try
                {
                    string responseData = "";
                    responseData = await Connection.GetAuthorizedDataAsync(Connection.getRecordByUserURL(user.Username), user.Token);

                    Dictionary<string, object> responseDataDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseData);

                    JArray datas = (JArray)responseDataDictionary["data"];

                    foreach (var selectedData in datas)
                    {
                        int id = (int)selectedData["id"];
                        string transaction = (string)selectedData["transaction"];
                        float value = (float)selectedData["value"];
                        string detail = (string)selectedData["detail"];
                        DateTime date = (DateTime)selectedData["date"];
                        int? photoRecordId = (int?)selectedData["photoRecordId"];

                        records.Add(new Record(id, transaction, value, detail, date, user.Username, photoRecordId));
                    };
                    dataGridView1.DataSource = records;
                    AdjustDataGridView();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error occurred while making the request: " + ex.Message);
                }*/
                ChangeLoadingState(false);

            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            ChangeLoadingState(true);
            DialogResult result = MessageBox.Show("Do you want to change your password?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                ChangeLoadingState(true);
                try
                {
                    string oldpassword = textBox4.Text;
                    string newpassword = textBox1.Text;

                    Dictionary<string, string> data = new Dictionary<string, string>
                    {
                        { "oldpassword", oldpassword },
                        { "newpassword", newpassword },
                    };

                    string requestBody = System.Text.Json.JsonSerializer.Serialize(data);

                    string responseData = await Connection.PostAuthorizedDataAsync(Connection.changePasswordURL, requestBody, user.Token);

                    Dictionary<string, object> responseDataDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseData);

                    MessageBox.Show(responseDataDictionary["message"].ToString());
                    textBox4.Text = "";
                    textBox1.Text = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error occurred while making the request: " + ex.Message);
                }
                ChangeLoadingState(false);
            }

        }

        private void FormProfile_Load(object sender, EventArgs e)
        {
            textBox2.Text = user.Username;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox4.UseSystemPasswordChar = textBox4.UseSystemPasswordChar != true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.UseSystemPasswordChar = textBox1.UseSystemPasswordChar != true;
        }
    }
}
