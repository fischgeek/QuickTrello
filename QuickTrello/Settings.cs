using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickTrello
{
    public partial class Settings : Form
    {
        private string settingsFile = "trello_config.json";
        public Settings()
        {
            InitializeComponent();
            if (File.Exists(settingsFile)) {
                var existing = File.ReadAllText(settingsFile);
                var settings = JsonConvert.DeserializeObject<LocalTrelloSettings>(existing);
                txtApiKey.Text = settings.ApiKey;
                txtApiToken.Text = settings.ApiToken;
                txtTargetListId.Text = settings.TargetListId;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var settings = new LocalTrelloSettings() {
                ApiKey = txtApiKey.Text
                , ApiToken = txtApiToken.Text
                , TargetListId = txtTargetListId.Text
            };
            var json = JsonConvert.SerializeObject(settings);
            File.WriteAllText("trello_config.json", json);
            this.Close();
        }
    }
}
