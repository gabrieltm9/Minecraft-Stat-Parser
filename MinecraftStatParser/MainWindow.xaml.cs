using Microsoft.Win32;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MinecraftStatParser
{
    //Made by Gabriel "gabrieltm9" Moncau, 5/12/2020

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> statFiles = new List<string>();
        List<string> uuids = new List<string>();
        List<string> playerNames = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void PickFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON Files (*.json)|*.json";
            openFileDialog.Multiselect = true;
            openFileDialog.Title = "Pick Stat File";
            if (openFileDialog.ShowDialog() == true)
            {
                ClearVals();
                GetSelectedStatFiles(openFileDialog);
            }
        }

        private void ClearVals()
        {
            statFiles.Clear();
            uuids.Clear();
            playerNames.Clear();
            PlayersListBox.Items.Clear();

            NameTxt.Text = "Name";
            TimePlayedTxt.Text = "Time Played";
        }

        private string GetUUID(string safeFileName)
        {
            safeFileName = safeFileName.Substring(0, safeFileName.IndexOf("."));
            safeFileName = safeFileName.Replace("-", "");
            return safeFileName;
        }

        private void GetSelectedStatFiles(OpenFileDialog dialog)
        {
            for (int i = 0; i < dialog.FileNames.Length; i++)
            {
                statFiles.Add(dialog.FileNames[i]);
                uuids.Add(GetUUID(dialog.SafeFileNames[i]));
                string temp = ParseName(dialog.SafeFileNames[i]);
                playerNames.Add(temp);
                PlayersListBox.Items.Add(temp);
            }
        }

        private void SetSkinUrl(string uuid)
        {
            SkinImage.Source = new BitmapImage(new Uri(@"https://crafatar.com/renders/body/" + uuid));
        }

        private double ParseTimePlayed(string statFilePath)
        {
            string stats = File.ReadAllText(statFilePath);
            string timePlayed = stats.Substring(stats.IndexOf("play_one_minute") + 17);
            timePlayed = timePlayed.Substring(0, timePlayed.IndexOf(","));
            double timeFloat = double.Parse(timePlayed);
            timeFloat /= 72000;
            timeFloat = Math.Round(timeFloat, 2);

            return timeFloat;
        }

        private string ParseName(string uuid)
        {
            NameTxt.IsEnabled = true;
            uuid = GetUUID(uuid);

            string url = "https://api.mojang.com/user/profiles/" + uuid + "/names";
            string names = new WebClient().DownloadString(url);

            if (names.Length > 0 && names.LastIndexOf("name") > 0)
            {
                string name = names.Substring(names.LastIndexOf("name") + 7);
                name = name.Substring(0, name.IndexOf("\""));
                return name;
            }
            else
                return "Failed to Parse Name";
        }

        private void UpdateStatDisplay(int index)
        {
            NameTxt.Text = playerNames[index];
            TimePlayedTxt.Text = "" + ParseTimePlayed(statFiles[index]) + " hours played";
            SetSkinUrl(uuids[index]);
        }

        private void PlayersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(PlayersListBox.SelectedIndex > -1)
                UpdateStatDisplay(PlayersListBox.SelectedIndex);
        }
    }
}
