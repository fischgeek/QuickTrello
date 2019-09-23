using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrelloUtility;

namespace QuickTrello
{
    public partial class MainForm : Form
    {
        private string token = ConfigurationManager.AppSettings["trelloapitoken"];
        private string key = ConfigurationManager.AppSettings["trelloapikey"];
        private string listId = ConfigurationManager.AppSettings["defaultlist"];
        public MainForm()
        {
            InitializeComponent();
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            lblResult.Text = "Submitting...";
            var tb = FireUpTrello();
            var card = new TrelloCard() {
                name = txtTitle.Text
                , desc = txtDescription.Text
            };
            var res = tb.AddCard(card, listId);
            if (res.Successful) {
                lblResult.ForeColor = Color.Green;
                lblResult.Text = "New card created successfully!";
            } else {
                lblResult.ForeColor = Color.Red;
                lblResult.Text = "Failed to create new card.";
            }
            this.Refresh();
            System.Threading.Thread.Sleep(3000);
            this.Close();
        }

        private TrelloBase FireUpTrello()
        {
            if (key == null || token == null) {
                throw new Exception("missing api key or token.");
            }
            var tb = TrelloBase.GetInstance();
            tb.Init(key, token);
            return tb;
        }
    }
}
