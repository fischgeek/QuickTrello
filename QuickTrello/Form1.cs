using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrelloConnect;

namespace QuickTrello
{
    public partial class MainForm : Form
    {
        private string settingsFile = "trello_config.json";
        LocalTrelloSettings settings = new LocalTrelloSettings();
        private bool firstRun = true;
        private bool trelloIsValid = false;

        public MainForm()
        {
            InitializeComponent();
            txtTitle.KeyDown += new KeyEventHandler(CheckForCtrlEnter);
            txtTitle.KeyUp += new KeyEventHandler(CheckTitleForTab);
            txtTitle.KeyPress += new KeyPressEventHandler(Silence);
            txtDescription.KeyDown += new KeyEventHandler(CheckForCtrlEnter);
            txtDescription.KeyUp += new KeyEventHandler(CheckDescriptionForTab);
            txtDescription.KeyPress += new KeyPressEventHandler(Silence);
            txtLabels.KeyDown += new KeyEventHandler(CheckForCtrlEnter);
            txtLabels.KeyUp += new KeyEventHandler(CheckLabelsForTab);
            txtLabels.KeyPress += new KeyPressEventHandler(Silence);
            this.KeyDown += new KeyEventHandler(CheckForCtrlEnter);
            AdjustHeight(HeightStep.One);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ShowDescription(false);
            ShowLabels(false);
            ShowAttachments(false);
            this.Refresh();
            this.ActiveControl = txtTitle;
            ValidateTrello();
        }

        private void Submit()
        {
            if (!trelloIsValid) {
                ShowFeedback("Missing requirements.");
                return;
            }
            ShowFeedback("Submitting...");
            var tb = FireUpTrello();

            // check desc for link
            var cardId = "";
            if (txtDescription.Text.StartsWith("http")) {
                var card = tb.CreateCard(settings.TargetListId, txtTitle.Text, null);
                tb.AttachUrlToCard(txtDescription.Text, card.Id);
                cardId = card.Id;
            } else {
                var card = tb.CreateCard(settings.TargetListId, txtTitle.Text, txtDescription.Text);
                cardId = card.Id;
            }
            var labels = txtLabels.Text.Replace(" ", "");
            var lbls = labels.Split(',');
            if (lbls.Count() > 0) {
                tb.AddLablesToCardByNameArray(cardId, lbls);
            }
            ShowFeedback("New card created successfully!", MessageType.Success);
            System.Threading.Thread.Sleep(1000);
            this.Close();
        }

        private void ValidateTrello()
        {
            FindFirstValidTrelloSettings();
            if (settings.ApiKey == null) {
                ShowFeedback("Missing API Key", MessageType.Error);
            } else if (settings.ApiToken == null) {
                ShowFeedback("Missing API Token", MessageType.Error);
            } else if (settings.TargetListId == null) {
                ShowFeedback("Target List has not been set", MessageType.Error);
            } else {
                trelloIsValid = true;
            }
        }

        private void FindFirstValidTrelloSettings()
        {
            var temptoken = ConfigurationManager.AppSettings["trelloapitoken"];
            var tempkey = ConfigurationManager.AppSettings["trelloapikey"];
            var temptargetlist = ConfigurationManager.AppSettings["targetlist"];
            if (temptoken != null && tempkey != null && temptargetlist != null) {
                settings.ApiKey = tempkey;
                settings.ApiToken = temptoken;
                settings.TargetListId = temptargetlist;
            } else if (File.Exists(settingsFile)) {
                var existing = File.ReadAllText(settingsFile);
                settings = JsonConvert.DeserializeObject<LocalTrelloSettings>(existing);
            }
        }

        private Trello.TrelloWorker FireUpTrello()
        {
            var tb = new Trello.TrelloWorker(settings.ApiKey, settings.ApiToken);
            return tb;
        }

        private void ShowDescription(bool show = true)
        {
            lblDescription.Visible = show;
            txtDescription.Visible = show;
        }

        private void ShowLabels(bool show = true)
        {
            lblLabels.Visible = show;
            txtLabels.Visible = show;
        }

        private void ShowAttachments(bool show = true)
        {
            lblAttachments.Visible = show;
            pnlFileAttachments.Visible = show;
        }

        private void CheckForCtrlEnter(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Return) {
                Submit();
                Console.WriteLine("Submit!");
            }
        }

        private void CheckTitleForTab(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab) {
                ShowDescription();
                txtDescription.Focus();
                AdjustHeight(HeightStep.Two);
            }
        }

        private void CheckDescriptionForTab(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.Tab) {
                txtTitle.Focus();
            } else if (e.KeyCode == Keys.Tab) {
                ShowLabels();
                txtLabels.Focus();
                AdjustHeight(HeightStep.Three);
            }
            
        }

        private void CheckLabelsForTab(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.Tab) {
                txtDescription.Focus();
            } else if (e.KeyCode == Keys.Tab) {
                ShowAttachments();
                txtTitle.Focus();
                AdjustHeight(HeightStep.Four);
            }
        }

        private void Silence(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Tab) {
                e.Handled = true;
            }
        }

        private void AdjustHeight(HeightStep step)
        {
            var h = (int)step;
            if (this.firstRun) {
                this.Size = new Size(this.Size.Width, h);
                this.firstRun = false;
            } else if (this.Size.Height < h) {
                this.Size = new Size(this.Size.Width, h);
            }
        }

        private void ShowFeedback(string message, MessageType messageType = MessageType.Default)
        {
            var bClr = Color.White;
            var fClr = Color.Black;
            switch (messageType) {
                case MessageType.Success:
                    fClr = Color.Green;
                    break;
                case MessageType.Error:
                    fClr = Color.Crimson;
                    break;
                default:
                    break;
            }
            lblResult.Text = message;
            lblResult.ForeColor = fClr;
            lblResult.BackColor = bClr;
            this.Refresh();
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();
        }

        private void pnlFileAttachments_DragDrop(object sender, DragEventArgs e)
        {
            MessageBox.Show("Dropped " + e.Data);
        }
    }

    public enum MessageType
    {
        Default,
        Success,
        Error
    }

    public enum HeightStep
    {
        One = 135,
        Two = 300,
        Three = 360,
        Four = 500,
        Default = 500
    }
}
