using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Sharp_Player
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        //The main page to return to.
        private MainPage mainPage;
        private List<string> directories = new List<string>();

        //Constructor that accepts the value of MainPage.
        public SettingsPage(MainPage main)
        {
            //Initialize the page.
            InitializeComponent();
            mainPage = main;

            //Clears the DirectoriesBox and then adds each directory to it (add from file).
            DirectoriesBox.Items.Clear();
            directories.Add("...Directory here.");
            directories.Add("...Directory here.");
            directories.Add("...Directory here.");

            DirectoriesBox.ItemsSource = directories;
        }

        //Returns to the main page.
        private void returnButton_Click(object sender, RoutedEventArgs e)
        {
            //Open the settings menu to change the directories.
            NavigationService.Navigate(mainPage);
        }

        private void dirButton_Click(object sender, RoutedEventArgs e)
        {
            //On click, check for text from add directory field, if its null (or directory doesn't exist)send error, else add the directory and update the main page directories
            directories.Add("Directory:");

            DirectoriesBox.ItemsSource = directories;
        }
    }
}
