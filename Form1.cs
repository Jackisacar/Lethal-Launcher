using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace LethalLauncher
{
    public partial class Form1 : Form
    {
        private ComboBox cmbProfiles = null!;
        private TextBox txtConsole = null!;
        private Button btnLaunchModded = null!;
        private Button btnLaunchVanilla = null!;
        private Label lblHeader = null!;

        // Hardcoded or default paths based on your configuration
        private string profilesDir = @"C:\LethalCompanyProfiles\Profiles";
        private string steamGameDir = @"C:\Program Files (x86)\Steam\steamapps\common\Lethal Company";

        public Form1()
        {
            InitializeTerminalLayout();
            LoadProfiles();
        }

        private void InitializeTerminalLayout()
        {
            // Window Configuration (CRT Terminal Aesthetic)
            this.Text = "Lethal Company Terminal // OS-v8.0";
            this.Size = new Size(650, 480);
            this.BackColor = Color.Black;
            this.ForeColor = Color.FromArgb(255, 140, 0); // Amber Phosphor Glow
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            Font terminalFont = new Font("Consolas", 10F, FontStyle.Regular, GraphicsUnit.Point);

            // Header Label
            lblHeader = new Label();
            lblHeader.Text = "COMPANY SYSTEM TERMINAL // PROFILE SELECTOR";
            lblHeader.Font = new Font("Consolas", 11F, FontStyle.Bold, GraphicsUnit.Point);
            lblHeader.ForeColor = Color.FromArgb(255, 140, 0);
            lblHeader.Location = new Point(20, 20);
            lblHeader.Size = new Size(590, 25);
            this.Controls.Add(lblHeader);

            // Profile Dropdown
            cmbProfiles = new ComboBox();
            cmbProfiles.Location = new Point(20, 60);
            cmbProfiles.Size = new Size(400, 25);
            cmbProfiles.Font = terminalFont;
            cmbProfiles.BackColor = Color.FromArgb(20, 10, 0);
            cmbProfiles.ForeColor = Color.FromArgb(255, 140, 0);
            cmbProfiles.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Controls.Add(cmbProfiles);

            // Launch Modded Button
            btnLaunchModded = new Button();
            btnLaunchModded.Text = "[ EXECUTE MODDED ]";
            btnLaunchModded.Location = new Point(435, 58);
            btnLaunchModded.Size = new Size(175, 30);
            btnLaunchModded.Font = terminalFont;
            btnLaunchModded.BackColor = Color.FromArgb(40, 20, 0);
            btnLaunchModded.ForeColor = Color.FromArgb(255, 140, 0);
            btnLaunchModded.FlatStyle = FlatStyle.Flat;
            btnLaunchModded.Click += BtnLaunchModded_Click;
            this.Controls.Add(btnLaunchModded);

            // Launch Vanilla Button
            btnLaunchVanilla = new Button();
            btnLaunchVanilla.Text = "[ LAUNCH VANILLA ]";
            btnLaunchVanilla.Location = new Point(435, 100);
            btnLaunchVanilla.Size = new Size(175, 30);
            btnLaunchVanilla.Font = terminalFont;
            btnLaunchVanilla.BackColor = Color.FromArgb(40, 20, 0);
            btnLaunchVanilla.ForeColor = Color.FromArgb(255, 140, 0);
            btnLaunchVanilla.FlatStyle = FlatStyle.Flat;
            btnLaunchVanilla.Click += BtnLaunchVanilla_Click;
            this.Controls.Add(btnLaunchVanilla);

            // Real-Time Console Log Box
            txtConsole = new TextBox();
            txtConsole.Location = new Point(20, 145);
            txtConsole.Size = new Size(590, 275);
            txtConsole.Multiline = true;
            txtConsole.ReadOnly = true;
            txtConsole.ScrollBars = ScrollBars.Vertical;
            txtConsole.BackColor = Color.FromArgb(5, 2, 0);
            txtConsole.ForeColor = Color.FromArgb(255, 140, 0);
            txtConsole.Font = terminalFont;
            this.Controls.Add(txtConsole);
        }

        private void LoadProfiles()
        {
            try
            {
                if (!Directory.Exists(profilesDir))
                {
                    Directory.CreateDirectory(profilesDir);
                    LogMessage($"Created missing profiles directory at: {profilesDir}");
                }

                cmbProfiles.Items.Clear();
                string[] profileDirs = Directory.GetDirectories(profilesDir);

                foreach (string dir in profileDirs)
                {
                    string profileName = Path.GetFileName(dir);
                    cmbProfiles.Items.Add(profileName);
                }

                if (cmbProfiles.Items.Count > 0)
                {
                    cmbProfiles.SelectedIndex = 0;
                    LogMessage($"Loaded {cmbProfiles.Items.Count} modpack profile(s) successfully.");
                }
                else
                {
                    LogMessage("WARNING: No modpack profiles found in directory.");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"ERROR loading profiles: {ex.Message}");
            }
        }

        private void LogMessage(string message)
        {
            if (txtConsole.InvokeRequired)
            {
                txtConsole.Invoke(new Action<string>(LogMessage), message);
            }
            else
            {
                txtConsole.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
            }
        }

        private void BtnLaunchVanilla_Click(object? sender, EventArgs e)
        {
            try
            {
                LogMessage("Initiating protocol: VANILLA RUN");
                LogMessage($"Target Game Directory: {steamGameDir}");

                string targetBepInEx = Path.Combine(steamGameDir, "BepInEx");
                string targetWinHttp = Path.Combine(steamGameDir, "winhttp.dll");
                string targetDoorstop = Path.Combine(steamGameDir, "doorstop_config.ini");

                // Purge BepInEx and loaders for clean vanilla state
                if (Directory.Exists(targetBepInEx)) Directory.Delete(targetBepInEx, true);
                if (File.Exists(targetWinHttp)) File.Delete(targetWinHttp);
                if (File.Exists(targetDoorstop)) File.Delete(targetDoorstop);

                LogMessage("Purged active mod files. Launching vanilla via Steam App ID 1966720...");
                Process.Start(new ProcessStartInfo("steam://rungameid/1966720") { UseShellExecute = true });
                LogMessage("EXECUTION COMPLETE. Good luck out there, employee.");
            }
            catch (Exception ex)
            {
                LogMessage($"CRITICAL ERROR: {ex.Message}");
            }
        }

        private void BtnLaunchModded_Click(object? sender, EventArgs e)
        {
            if (cmbProfiles.SelectedItem == null)
            {
                MessageBox.Show("Please select a profile from the terminal dropdown first!", "Terminal Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedProfile = cmbProfiles.SelectedItem.ToString()!;
            try
            {
                LogMessage($"Target profile locked: [{selectedProfile}]");
                LogMessage($"Target Game Directory: {steamGameDir}");

                string targetBepInEx = Path.Combine(steamGameDir, "BepInEx");
                string targetWinHttp = Path.Combine(steamGameDir, "winhttp.dll");
                string targetDoorstop = Path.Combine(steamGameDir, "doorstop_config.ini");

                // 1. Purge old game mods and loaders
                LogMessage("Purging old mods from game directory...");
                if (Directory.Exists(targetBepInEx)) Directory.Delete(targetBepInEx, true);
                if (File.Exists(targetWinHttp)) File.Delete(targetWinHttp);
                if (File.Exists(targetDoorstop)) File.Delete(targetDoorstop);

                // 2. Locate source profile directory
                string sourceProfileDir = Path.Combine(profilesDir, selectedProfile);
                string sourceBepInEx = Path.Combine(sourceProfileDir, "BepInEx");

                if (Directory.Exists(sourceBepInEx))
                {
                    LogMessage("Injecting profile BepInEx contents and core loaders...");

                    // Recreate BepInEx folder structure
                    Directory.CreateDirectory(targetBepInEx);

                    foreach (string dirPath in Directory.GetDirectories(sourceBepInEx, "*", SearchOption.AllDirectories))
                    {
                        string relativePath = dirPath.Substring(sourceBepInEx.Length + 1);
                        Directory.CreateDirectory(Path.Combine(targetBepInEx, relativePath));
                    }

                    foreach (string filePath in Directory.GetFiles(sourceBepInEx, "*.*", SearchOption.AllDirectories))
                    {
                        string relativePath = filePath.Substring