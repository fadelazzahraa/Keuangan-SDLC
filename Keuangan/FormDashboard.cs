using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Keuangan
{
    public partial class FormDashboard : Form
    {
        private User user;
        private List<Record> records;
        private List<User> users;
        private int defaultUser;
        private int selectedUser;

        public FormDashboard(User loggedinuser)
        {
            user = loggedinuser;
            InitializeComponent();
        }

        private async void LoadRecordData(int userId)
        {
            records = new List<Record>();
            ChangeProgressBarState(true);
            try
            {
                string responseData = "";
                if (userId.Equals(-1))
                {
                    responseData = await Connection.GetAuthorizedDataAsync(Connection.getRecordByUserURL(user.Username), user.Token);
                }
                else
                {
                    responseData = await Connection.GetAuthorizedDataAsync(Connection.getRecordByUserURL(users[userId].Username), user.Token);
                }

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
                }

                float balance = 0;
                float debitThisMonth = 0;
                float creditThisMonth = 0;
                
                if (records.Count != 0)
                {
                    foreach (Record record in records)
                    {
                        if (record.Transaction == "debit")
                        {
                            balance += record.ValueRecord;
                        }
                        else
                        {
                            balance -= record.ValueRecord;
                        }

                        if (record.Transaction == "debit" && record.Date.Month == DateTime.Now.Month)
                        {
                            debitThisMonth += record.ValueRecord;
                        }
                        else if (record.Transaction == "credit" && record.Date.Month == DateTime.Now.Month)
                        {
                            creditThisMonth += record.ValueRecord;
                        }
                    }
                }
                

                CultureInfo info = new CultureInfo("id-ID");
                CultureInfo infoEn = new CultureInfo("en-US");

                label11.Text = $"Debit {DateTime.Now.ToString("MMMM", infoEn)}";
                label3.Text = $"Credit {DateTime.Now.ToString("MMMM", infoEn)}";

                label7.Text = balance.ToString("C", info);
                label7.ForeColor = balance > 0 ? Color.Green : Color.Red;
                label8.Text = debitThisMonth.ToString("C", info);
                label8.ForeColor = Color.Green;
                label9.Text = creditThisMonth.ToString("C", info);
                label9.ForeColor = Color.Red;

                if (records.Count != 0)
                {
                    label2.Text = records[records.Count - 1].ValueRecord.ToString("C", info);
                    label2.ForeColor = records[records.Count - 1].Transaction == "debit" ? Color.Green : Color.Red;
                    label12.Text = records[records.Count - 1].Detail;
                    label13.Text = records[records.Count - 1].Date.ToString("dd/MM/yyyy");
                } else
                    {
                    label2.Text = "";
                    label2.ForeColor = Color.Black;
                    label12.Text = "No transaction yet";
                    label13.Text = "";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred while making the request: " + ex.Message);
            }
            ChangeProgressBarState(false);
        }

        private async void LoadUsersData()
        {
            users = new List<User>();
            ChangeProgressBarState(true);
            try
            {
                string responseData = await Connection.GetAuthorizedDataAsync(Connection.getUsersURL, user.Token);

                Dictionary<string, object> responseDataDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseData);

                JArray datas = (JArray)responseDataDictionary["data"];

                int idxx = 0;
                foreach (var selectedData in datas)
                {
                    int id = (int)selectedData["id"];
                    string username = (string)selectedData["username"];
                    string role = (string)selectedData["role"];
                    
                    users.Add(new User(id, username, role));

                    comboBox1.Items.Add($"{id} - {username}");

                    if (id == user.ID)
                    {
                        comboBox1.SelectedIndex = idxx;
                        defaultUser = idxx;
                        selectedUser = idxx;
                    }
                    idxx += 1;
                };

                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred while making the request: " + ex.Message);
            }
            ChangeProgressBarState(false);
        }

        private void FormDashboard_Load(object sender, EventArgs e)
        {
            if (user.Role == "user")
            {
                label14.Enabled = false;
                label14.Visible = false;
                comboBox1.Enabled = false;
                comboBox1.Visible = false;
                users = null;
                defaultUser = -1;
                LoadRecordData(-1);
            } else
            {
                LoadUsersData();
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Logout success!");
            this.Hide();
            FormLogin formLogin = new FormLogin();
            formLogin.Closed += (s, args) => this.Close();
            formLogin.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormCashFlow formCashFlow = new FormCashFlow(user);
            formCashFlow.Closed += (s, args) =>
            {
                this.Show();
                comboBox1.SelectedIndex = defaultUser;
                LoadRecordData(defaultUser);
            };
            formCashFlow.Show();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormUploadBill formUploadBill = new FormUploadBill(user);
            formUploadBill.Closed += (s, args) =>
            {
                this.Show();
                comboBox1.SelectedIndex = defaultUser;
                LoadRecordData(defaultUser);
            };
            formUploadBill.Show();
        }

        private void ChangeProgressBarState(bool isActivated = true)
        {
            if (isActivated)
            {
                toolStripStatusLabel1.Text = "Loading...";
                toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
                toolStripProgressBar1.MarqueeAnimationSpeed = 100;
            }
            else
            {
                toolStripStatusLabel1.Text = "Ready";
                toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
                toolStripProgressBar1.MarqueeAnimationSpeed = 0;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedUser = comboBox1.SelectedIndex;
            LoadRecordData(selectedUser);
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormProfile formProfile = new FormProfile(user);
            formProfile.Closed += (s, args) =>
            {
                this.Show();
                comboBox1.SelectedIndex = defaultUser;
                LoadRecordData(defaultUser);
            };
            formProfile.Show();
        }
    }
}
