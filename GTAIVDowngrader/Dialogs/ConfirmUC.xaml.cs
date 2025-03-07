﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.WindowsAPICodePack.Dialogs;

using CCL;

namespace GTAIVDowngrader.Dialogs
{
    public partial class ConfirmUC : UserControl
    {

        #region Variables
        private MainWindow instance;

        private bool temp;
        #endregion

        #region Methods
        private void LogDowngradingInfos()
        {
            // MD5 Check
            Core.AddLogItem(LogType.Info, "- - - MD5 Check - - -");
            Core.AddLogItem(LogType.Info, string.Format("MD5 Hash Created: {0}", Core.CDowngradingInfo.ReceivedMD5Hash));

            string md5HashFound = Core.CDowngradingInfo.RelatedMD5Hash;
            Core.AddLogItem(LogType.Info, string.Format("MD5 Hash   Found: {0}", string.IsNullOrEmpty(md5HashFound) ? 
                "Couldn't find any MD5 Hash that relates to the created MD5 Hash. This might mean that the selected GTAIV.exe is not version 1.2.0.43." : md5HashFound));

            Core.AddLogItem(LogType.Info, string.Format("If both MD5 Hashes don't match: The selected GTA IV Installation might be modified (Contains mods)."));

            // Downgrading Informations
            Core.AddLogItem(LogType.Info, "- - - Downgrading Informations - - -");
            Core.AddLogItem(LogType.Info, string.Format("Selected downgrading version:     {0}", Core.CDowngradingInfo.DowngradeTo.ToString()));
            Core.AddLogItem(LogType.Info, string.Format("Configure for GFWL:               {0}", Core.CDowngradingInfo.ConfigureForGFWL.ToString()));
            Core.AddLogItem(LogType.Info, string.Format("Selected radio downgrader:        {0}", Core.CDowngradingInfo.SelectedRadioDowngrader.ToString()));
            Core.AddLogItem(LogType.Info, string.Format("Selected vladivostok type:        {0}", Core.CDowngradingInfo.SelectedVladivostokType.ToString()));
            Core.AddLogItem(LogType.Info, string.Format("Install No EFLC Music in IV Fix:  {0}", Core.CDowngradingInfo.InstallNoEFLCMusicInIVFix.ToString()));
            Core.AddLogItem(LogType.Info, string.Format("Install Prerequisites:            {0}", Core.CDowngradingInfo.InstallPrerequisites.ToString()));
            Core.AddLogItem(LogType.Info, string.Format("Create Backup:                    {0}", MakeABackupForMeCheckbox.IsChecked.Value.ToString()));
            Core.AddLogItem(LogType.Info, string.Format("Create Backup in zip file:        {0}", CreateBackupInZIPFileCheckBox.IsChecked.Value.ToString()));
            Core.AddLogItem(LogType.Info, "- - - Starting Downgrading Process - - -");
        }
        #endregion

        #region Functions
        public bool CheckBackupDirectory(string bPath)
        {
            if (string.IsNullOrWhiteSpace(bPath))
            {
                BackupLocationStatusLabel.Text = "Please select a backup path";
                BackupLocationStatusImage.Source = new BitmapImage(new Uri(@"..\Resources\warningWhite.png", UriKind.RelativeOrAbsolute));
            }
            else
            {
                if (Directory.Exists(bPath))
                {
                    if (CreateBackupInZIPFileCheckBox.IsChecked.Value)
                    {
                        instance.ChangeActionButtonEnabledState(true, true, true, true);
                        BackupLocationStatusLabel.Text = "Directory is valid!";
                        BackupLocationStatusImage.Source = new BitmapImage(new Uri(@"..\Resources\checkCircleWhite.png", UriKind.RelativeOrAbsolute));
                        return true;
                    }
                    else
                    {
                        if (Directory.GetFiles(bPath).Length <= 0)
                        {
                            instance.ChangeActionButtonEnabledState(true, true, true, true);
                            BackupLocationStatusLabel.Text = "Directory is valid!";
                            BackupLocationStatusImage.Source = new BitmapImage(new Uri(@"..\Resources\checkCircleWhite.png", UriKind.RelativeOrAbsolute));
                            return true;
                        }
                        else
                        {
                            BackupLocationStatusLabel.Text = "The directory you've selected is not empty!";
                            BackupLocationStatusImage.Source = new BitmapImage(new Uri(@"..\Resources\warningWhite.png", UriKind.RelativeOrAbsolute));
                        }
                    }
                }
                else
                {
                    BackupLocationStatusLabel.Text = "Directory does not exists!";
                    BackupLocationStatusImage.Source = new BitmapImage(new Uri(@"..\Resources\warningWhite.png", UriKind.RelativeOrAbsolute));
                }
            }

            instance.ChangeActionButtonEnabledState(true, true, true, false);
            return false;
        }
        public bool CheckIfOldFoldersExists()
        {
            bool pluginsFolderExists = false, scriptsFolderExists = false;

            string pluginsFolder = string.Format("{0}\\plugins", Core.CDowngradingInfo.IVWorkingDirectoy);
            if (Directory.Exists(pluginsFolder))
                pluginsFolderExists = true;

            string scriptsFolder = string.Format("{0}\\scripts", Core.CDowngradingInfo.IVWorkingDirectoy);
            if (Directory.Exists(scriptsFolder))
                scriptsFolderExists = true;

            return pluginsFolderExists || scriptsFolderExists;
        }
        #endregion

        #region Constructor
        public ConfirmUC()
        {
            InitializeComponent();
        }
        public ConfirmUC(MainWindow window)
        {
            instance = window;
            InitializeComponent();
        }
        #endregion

        #region Events
        private void Instance_BackButtonClicked(object sender, EventArgs e)
        {
            instance.PreviousStep();
        }
        private void Instance_NextButtonClicked(object sender, EventArgs e)
        {
            if (CheckIfOldFoldersExists())
            {
                switch (MessageBox.Show("We've noticed that you still have a plugins or scripts folder inside of the GTA IV directory that should be downgraded. " +
                    "If you don't want to loose them, now it's the time to make a backup of them! Just press No, and make a backup of them. " +
                    "If you don't want them anymore, you can press Yes, this will start the downgrading process, which will delete them.", "Confirm deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning))
                {
                    case MessageBoxResult.Yes: break;
                    case MessageBoxResult.No: return;
                }
            }

            if (MakeABackupForMeCheckbox.IsChecked.Value)
            {
                if (CheckBackupDirectory(BackupLocationTextbox.Text))
                {
                    Core.CDowngradingInfo.SetTargetBackupPath(BackupLocationTextbox.Text);
                    Core.CDowngradingInfo.SetCreateBackupInZipFile(CreateBackupInZIPFileCheckBox.IsChecked.Value);
                    LogDowngradingInfos();
                    instance.NextStep();
                }
            }
            else
            {
                LogDowngradingInfos();
                instance.NextStep();
            }
        }
        #endregion

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            instance.NextButton.Content = "Next";

            instance.NextButtonClicked -= Instance_NextButtonClicked;
            instance.BackButtonClicked -= Instance_BackButtonClicked;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            instance.NextButtonClicked += Instance_NextButtonClicked;
            instance.BackButtonClicked += Instance_BackButtonClicked;

            instance.ChangeActionButtonVisiblity(true, true, false, true);
            instance.ChangeActionButtonEnabledState(true, true, true, true);

            instance.NextButton.Content = "Downgrade";

            if (Core.IsInOfflineMode)
            {
                // Hide label
                DownloadSizeInfoLabel.Visibility = Visibility.Collapsed;

                // Skip download size calculation
                return;
            }

            // Calculate download size
            long size = 0;

            // Game stuff
            switch (Core.CDowngradingInfo.DowngradeTo)
            {
                case GameVersion.v1080:
                    size += Core.GetDowngradeFileSizeByFileName("1080.zip");
                    break;
                case GameVersion.v1070:
                    size += Core.GetDowngradeFileSizeByFileName("1070.zip");
                    break;
                case GameVersion.v1040:
                    size += Core.GetDowngradeFileSizeByFileName("1040.zip");
                    break;
            }

            // Radio stuff
            switch (Core.CDowngradingInfo.SelectedRadioDowngrader)
            {
                case RadioDowngrader.SneedsDowngrader:
                    size += Core.GetDowngradeFileSizeByFileName("SneedsRadioDowngrader.zip");
                    break;
                case RadioDowngrader.LegacyDowngrader:
                    size += Core.GetDowngradeFileSizeByFileName("LegacyRadioDowngrader.zip");
                    break;
            }
            switch (Core.CDowngradingInfo.SelectedVladivostokType)
            {
                case VladivostokTypes.New:
                    size += Core.GetDowngradeFileSizeByFileName("WithNewVladivostok.zip");
                    break;
                case VladivostokTypes.Old:
                    size += Core.GetDowngradeFileSizeByFileName("WithoutNewVladivostok.zip");
                    break;
            }
            if (Core.CDowngradingInfo.InstallNoEFLCMusicInIVFix)
            {
                size += Core.GetDowngradeFileSizeByFileName("EpisodeOnlyMusicCE.zip");
            }

            // Mods
            for (int i = 0; i < Core.CDowngradingInfo.SelectedMods.Count; i++)
            {
                size += Core.CDowngradingInfo.SelectedMods[i].FileSize;
            }

            // Optional Mod Stuff
            for (int i = 0; i < Core.CDowngradingInfo.SelectedOptionalComponents.Count; i++)
            {
                size += Core.CDowngradingInfo.SelectedOptionalComponents[i].FileSize;
            }

            // Prerequisites
            if (Core.CDowngradingInfo.InstallPrerequisites)
            {
                size += Core.GetDowngradeFileSizeByFileName("directx_Jun2010_redist.exe");
                size += Core.GetDowngradeFileSizeByFileName("vcredist_x86.exe");
            }
            if (Core.CDowngradingInfo.ConfigureForGFWL)
            {
                size += Core.GetDowngradeFileSizeByFileName("gfwlivesetup.exe");
                size += Core.GetDowngradeFileSizeByFileName("xliveredist.msi");

                if (Environment.Is64BitOperatingSystem)
                    size += Core.GetDowngradeFileSizeByFileName("wllogin_64.msi");
                else
                    size += Core.GetDowngradeFileSizeByFileName("wllogin_32.msi");
            }

            DownloadSizeInfoLabel.Text = string.Format("The downgrader will download {0} of data from the internet for this downgrade.", FileHelper.GetExactFileSizeAdvanced(size));
        }

        private void MakeABackupForMeCheckbox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            Core.CDowngradingInfo.SetWantsToCreateBackup(MakeABackupForMeCheckbox.IsChecked.Value);
            if (MakeABackupForMeCheckbox.IsChecked.Value)
            {
                instance.ChangeActionButtonEnabledState(true, true, true, false);
                BackupStackPanel.Visibility = Visibility.Visible;
                CheckBackupDirectory(BackupLocationTextbox.Text);
            }
            else
            {
                instance.ChangeActionButtonEnabledState(true, true, true, true);
                BackupStackPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void BrowseBackupLocationButton_Click(object sender, RoutedEventArgs e)
        {
            using (CommonOpenFileDialog ofd = new CommonOpenFileDialog("Select backup location"))
            {
                ofd.IsFolderPicker = true;
                if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    BackupLocationTextbox.Text = ofd.FileName;
                    CheckBackupDirectory(BackupLocationTextbox.Text);
                }
            }
        }
        private void CreateBackupInZIPFileCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (temp)
                CheckBackupDirectory(BackupLocationTextbox.Text);
            else
                temp = true;
        }
        private void BackupLocationTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckBackupDirectory(BackupLocationTextbox.Text);
        }
        private void BackupLocationTextbox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            CheckBackupDirectory(BackupLocationTextbox.Text);
        }

    }
}
