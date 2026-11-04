using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LethalLauncher
{
    public partial class Form1 : Form
    {
        private readonly string steamGameDir = @"C:\Program Files (x86)\Steam\steamapps\common\Lethal Company";
        private readonly string profilesDir = @"C:\LethalCompanyProfiles\Profiles";

        // UI Controls (Terminal Aesthetic)
        private readonly Label lblHeader = new Label();
        private readonly Label lblSubHeader = new Label();
        private readonly Label lblSelect = new Label();
        private readonly ComboBox cmbProfiles = new ComboBox();
        private readonly Button btnLaunchModded = new Button();
        private readonly Button btnLaunchVanilla = new Button();
        private readonly RichTextBox txtLog = new RichTextBox();
        private readonly IContainer? components = null;

        public Form1()
        {
            SetupTerminalUI();
            LoadAvailableProfiles();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void SetupTerminalUI()
        {
            // Form Window Configuration (Authentic Terminal Dark Theme)
            this.Text = "LETHAL COMPANY // OS TERMINAL";
            this.Size = new Size(540, 500);
            this.BackColor = Color.FromArgb(8, 8, 10); // Deep CRT Black
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Header Title (Classic Amber CRT Glow)
            lblHeader.Text = "> LETHAL COMPANY MODPACK MANAGER v1.0";
            lblHeader.ForeColor = Color.FromArgb(255, 150, 0); // Iconic Amber
            lblHeader.Font = new Font("Consolas", 11, FontStyle.Bold);
            lblHeader.Location = new Point(20, 15);
            lblHeader.AutoSize = true;
            this.Controls.Add(lblHeader);

            // Sub-divider line label
            lblSubHeader.Text = "----------------------------------------------------";
            lblSubHeader.ForeColor = Color.FromArgb(80, 50, 0);
            lblSubHeader.Font = new Font("Consolas", 9, FontStyle.Regular);
            lblSubHeader.Location = new Point(20, 32);
            lblSubHeader.AutoSize = true;
            this.Controls.Add(lblSubHeader);

            // Profile Selection Label
            lblSelect.Text = "SELECT TARGET PROFILE:";
            lblSelect.ForeColor = Color.FromArgb(200, 150, 80);
            lblSelect.Font = new Font("Consolas", 9, FontStyle.Bold);
            lblSelect.Location = new Point(20, 55);
            lblSelect.AutoSize = true;
            this.Controls.Add(lblSelect);

            // Profile Dropdown ComboBox (Terminal Styled)
            cmbProfiles.Location = new Point(20, 80);
            cmbProfiles.Size = new Size(480, 26);
            cmbProfiles.Font = new Font("Consolas", 10, FontStyle.Regular);
            cmbProfiles.BackColor = Color.FromArgb(15, 15, 18);
            cmbProfiles.ForeColor = Color.FromArgb(255, 170, 50);
            cmbProfiles.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Controls.Add(cmbProfiles);

            // Launch Modded Button (Amber Industrial Accent)
            btnLaunchModded.Text = "[ EXECUTE MODDED ]";
            btnLaunchModded.Location = new Point(20, 122);
            btnLaunchModded.Size = new Size(235, 45);
            btnLaunchModded.BackColor = Color.FromArgb(220, 110, 0);
            btnLaunchModded.ForeColor = Color.Black;
            btnLaunchModded.FlatStyle = FlatStyle.Flat;
            btnLaunchModded.Font = new Font("Consolas", 10, FontStyle.Bold);
            btnLaunchModded.Cursor = Cursors.Hand;
            btnLaunchModded.FlatAppearance.BorderColor = Color.FromArgb(255, 140, 0);
            btnLaunchModded.Click += (s, e) => LaunchSelectedProfile();
            this.Controls.Add(btnLaunchModded);

            // Launch Vanilla Button (Dark Steel Terminal Style)
            btnLaunchVanilla.Text = "[ LAUNCH VANILLA ]";
            btnLaunchVanilla.Location = new Point(265, 122);
            btnLaunchVanilla.Size = new Size(235, 45);
            btnLaunchVanilla.BackColor = Color.FromArgb(25, 25, 30);
            btnLaunchVanilla.ForeColor = Color.FromArgb(180, 180, 180);
            btnLaunchVanilla.FlatStyle = FlatStyle.Flat;
            btnLaunchVanilla.Font = new Font("Consolas", 10, FontStyle.Bold);
            btnLaunchVanilla.Cursor = Cursors.Hand;
            btnLaunchVanilla.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 70);
            btnLaunchVanilla.Click += (s, e) => LaunchVanilla();
            this.Controls.Add(btnLaunchVanilla);

            // Terminal Log Box (Pitch Black with Amber Phosphor Text)
            txtLog.Location = new Point(20, 185);
            txtLog.Size = new Size(480, 250);
            txtLog.BackColor = Color.FromArgb(5, 5, 7);
            txtLog.ForeColor = Color.FromArgb(255, 145, 0); // Amber terminal stream
            txtLog.Font = new Font("Consolas", 9, FontStyle.Regular);
            txtLog.ReadOnly = true;
            txtLog.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtLog);

            LogMessage("SYSTEM BOOT SUCCESSFUL.");
            LogMessage("Connected to local profile directory.");
        }

        private void LoadAvailableProfiles()
        {
            cmbProfiles.Items.Clear();
            try
            {
                if (Directory.Exists(profilesDir))
                {
                    string[] profileFolders = Directory.GetDirectories(profilesDir);
                    foreach (string folder in profileFolders)
                    {
                        string folderName = Path.GetFileName(folder);
                        cmbProfiles.Items.Add(folderName);
                    }

                    if (cmbProfiles.Items.Count > 0)
                    {
                        cmbProfiles.SelectedIndex = 0;
                        LogMessage($"Loaded {cmbProfiles.Items.Count} profile(s) successfully.");
                    }
                    else
                    {
                        LogMessage("WARNING: No modpack profiles detected in C:\\LethalCompanyProfiles\\Profiles");
                    }
                }
                else
                {
                    Directory.CreateDirectory(profilesDir);
                    LogMessage("Initialized missing Profiles directory.");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"ERROR loading profiles: {ex.Message}");
            }
        }

        private void LogMessage(string message)
        {
            txtLog.AppendText($"> [{DateTime.Now:HH:mm:ss}] {message}\n");
            txtLog.ScrollToCaret();
        }

        private void LaunchVanilla()
        {
            try
            {
                LogMessage("Purging BepInEx plugins for Vanilla run...");
                string targetPlugins = Path.Combine(steamGameDir, "BepInEx", "plugins");
                string targetConfig = Path.Combine(steamGameDir, "BepInEx", "config");

                if (Directory.Exists(targetPlugins)) Directory.Delete(targetPlugins, true);
                if (Directory.Exists(targetConfig)) Directory.Delete(targetConfig, true);

                LogMessage("Initializing Steam protocol [Vanilla]...");
                Process.Start(new ProcessStartInfo("steam://rungameid/1948280") { UseShellExecute = true });
                LogMessage("EXECUTION COMPLETE. Good luck out there, employee.");
            }
            catch (Exception ex)
            {
                LogMessage($"CRITICAL ERROR: {ex.Message}");
            }
        }

        private void LaunchSelectedProfile()
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
                string targetPlugins = Path.Combine(steamGameDir, "BepInEx", "plugins");
                string targetConfig = Path.Combine(steamGameDir, "BepInEx", "config");

                LogMessage("Purging old mods from game directory...");
                if (Directory.Exists(targetPlugins)) Directory.Delete(targetPlugins, true);
                if (Directory.Exists(targetConfig)) Directory.Delete(targetConfig, true);

                string sourcePlugins = Path.Combine(profilesDir, selectedProfile, "BepInEx", "plugins");
                string sourceConfig = Path.Combine(profilesDir, selectedProfile, "BepInEx", "config");

                LogMessage("Injecting mod files into BepInEx directory...");
                if (Directory.Exists(sourcePlugins)) CopyDirectory(sourcePlugins, targetPlugins);
                if (Directory.Exists(sourceConfig)) CopyDirectory(sourceConfig, targetConfig);

                LogMessage("Launching Lethal Company via Steam protocol...");
                Process.Start(new ProcessStartInfo("steam://rungameid/1948280") { UseShellExecute = true });
                LogMessage("EXECUTION COMPLETE. Modpack active.");
            }
            catch (Exception ex)
            {
                LogMessage($"CRITICAL ERROR: {ex.Message}");
                MessageBox.Show($"Launch failed: {ex.Message}", "Terminal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void CopyDirectory(string sourceDir, string destinationDir)
        {
            Directory.CreateDirectory(destinationDir);
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                File.Copy(file, Path.Combine(destinationDir, Path.GetFileName(file)), true);
            }
            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                CopyDirectory(subDir, Path.Combine(destinationDir, Path.GetFileName(subDir)));
            }
        }
    }
}