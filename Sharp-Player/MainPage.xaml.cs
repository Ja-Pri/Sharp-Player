using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Sharp_Player
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private string directories = @"D:\Music\";
        private MediaPlayer player = new MediaPlayer();
        //Lists to hold the artist/album/song paths.
        private List<string> artistPaths = new List<string>();
        private List<string> albumPaths = new List<string>();
        private List<string> songPaths = new List<string>();
        //Holds the now playing information.
        private List<string> nowPlaying = new List<string>();
        private int currentlyPlaying = 0;
        //Create a list to hold items for the listview.
        private List<LoadFiles> albumNames = new List<LoadFiles>();
        private List<string> songNames = new List<string>();
        //Holds bools for repeat and shuffle.
        private bool repeat = true;
        private bool shuffle = false;
        private string songTime;

        public MainPage()
        {
            InitializeComponent();
            //Clear ListViews.
            ArtistBox.Items.Clear();
            SongBox.Items.Clear();

            //Populates the listviews of the music player.
            GetArtists();
            nowPlaying = songPaths.ToList();

            //Creates event if end of song is reached.
            player.MediaEnded += Player_MediaEnded;

            //Creates event for song timer.
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

            //Creates event if media is opened.
            player.MediaOpened += player_MediaOpened;
        }

        //Gets the length of the song.
        private void player_MediaOpened(object sender, EventArgs e)
        {
            songTime = player.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
        }

        //Displays a song timer.
        void timer_Tick(object sender, EventArgs e)
        {
            if (player.Source != null)
                PlayerTimer.Text = String.Format("{0} / {1}", player.Position.ToString(@"mm\:ss"), songTime);
            else
                PlayerTimer.Text = "";
        }

        //Moves now playing to the next song or ends playlist.
        private void Player_MediaEnded(object sender, EventArgs e)
        {
            //If the player is currently playing music and has a next song.
            if (Play_Pause.Content == FindResource("Pause") && shuffle == false && currentlyPlaying + 1 < nowPlaying.Count)
            {
                //Get the next song.
                currentlyPlaying += 1;

                //Open the song and update the now playing data.
                GetPlayerInfo(nowPlaying[currentlyPlaying]);
            }
            else if (Play_Pause.Content == FindResource("Pause") && shuffle == true && currentlyPlaying + 1 < nowPlaying.Count)
            {
                //Get the next song.
                Random x = new Random();
                currentlyPlaying = x.Next(nowPlaying.Count);

                //Open the song and update the now playing data.
                GetPlayerInfo(nowPlaying[currentlyPlaying]);
            }
            else
            {
                if (Play_Pause.Content == FindResource("Pause") && repeat == true)
                {
                    //Get the next song.
                    currentlyPlaying = 0;

                    //Open the song and update the now playing data.
                    GetPlayerInfo(nowPlaying[currentlyPlaying]);
                }
                else
                {
                    //Stop playing and move now playing to the beginning of the list.
                    Play_Pause.Content = FindResource("Play");
                    currentlyPlaying = 0;
                    GetPlayerInfo(nowPlaying[currentlyPlaying]);
                    player.Stop();
                }
            }
        }

        //Stop music from playing and close the program.
        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            player.Stop();
            Application.Current.Shutdown();
        }

        //Maximizes the window or returns it to its previous state.
        private void maximizeButton_Click(object sender, RoutedEventArgs e)
        {
            //If the window is maximized, return it to its previous state.
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }
            else
            {
                //Maximize the window.
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
        }

        //Minimize the window.
        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        //Return the artists/songs to initial lists.
        private void returnButton_Click(object sender, RoutedEventArgs e)
        {
            //Clear ListViews.
            artistPaths.Clear();
            albumPaths.Clear();
            songPaths.Clear();

            GetArtists();
        }

        //Open the settings menu.
        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsPage x = new SettingsPage(this);
            //Create a small settings menu to allow users to change the directories.
            NavigationService.Navigate(x);
        }

        //Plays the previous song.
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            //If the player is currently playing music and has a next song.
            if (Play_Pause.Content == FindResource("Pause") && shuffle == false && currentlyPlaying > 0)
            {
                //Get the next song.
                currentlyPlaying -= 1;

                //Open the song and update the now playing data.
                GetPlayerInfo(nowPlaying[currentlyPlaying]);
            }
            else if (Play_Pause.Content == FindResource("Pause") && shuffle == false && currentlyPlaying == 0)
            {
                //Get the next song.
                currentlyPlaying = 0;

                //Open the song and update the now playing data.
                GetPlayerInfo(nowPlaying[currentlyPlaying]);
            }
            else if (Play_Pause.Content == FindResource("Pause") && shuffle == true)
            {
                Random x = new Random();

                //Get the next song.
                currentlyPlaying = x.Next(nowPlaying.Count);

                //Open the song and update the now playing data.
                GetPlayerInfo(nowPlaying[currentlyPlaying]);
            }
        }

        //Play or pause the music.
        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            //If the player is currently paused.
            if (Play_Pause.Content == FindResource("Play"))
            {
                //Change the icon.
                Play_Pause.Content = FindResource("Pause");

                //If a song is paused, resume the song.
                if (player.Position.ToString(@"mm\:ss") != "0:00")
                {
                    player.Play();
                }
                //Else start a song from the beginning.
                else
                {
                    //Open the song and update the now playing data.
                    GetPlayerInfo(nowPlaying[currentlyPlaying]);
                }
            }
            else
            {
                //Change the icon and pause music.
                Play_Pause.Content = FindResource("Play");
                player.Pause();
            }
        }

        private void repeatButton_Click(object sender, RoutedEventArgs e)
        {
            //If repeat is on.
            if (repeatButton.Content == FindResource("Repeat"))
            {
                //Change the icon and stop repeat.
                repeatButton.Content = FindResource("NoRepeat");
                repeat = false;
            }
            else
            {
                //Change the icon and allow repeat.
                repeatButton.Content = FindResource("Repeat");
                repeat = true;
            }
        }

        private void shuffleButton_Click(object sender, RoutedEventArgs e)
        {
            //If shuffle is on.
            if (shuffleButton.Content == FindResource("Shuffle"))
            {
                //Change the icon and stop shuffle.
                shuffleButton.Content = FindResource("NoShuffle");
                shuffle = false;
            }
            else
            {
                //Change the icon and allow shuffle.
                shuffleButton.Content = FindResource("Shuffle");
                shuffle = true;
            }
        }

        //Mutes or Unmutes the music player volume and changes the button icon.
        private void volumeButton_Click(object sender, RoutedEventArgs e)
        {
            //If volume is on.
            if (volumeButton.Content == FindResource("Volume"))
            {
                //Change the icon and mute the player.
                volumeButton.Content = FindResource("Mute");
                player.IsMuted = true;
            }
            else
            {
                //Change the icon and unmute the player.
                volumeButton.Content = FindResource("Volume");
                player.IsMuted = false;
            }
        }

        //Gets the currently playing artist and select them in the artists listview.
        private void currentArtistButton_Click(object sender, RoutedEventArgs e)
        {
            //Gets the currently playing song, and finds the artist that plays the song.
            FileInfo file = new FileInfo(nowPlaying[currentlyPlaying]);
            DirectoryInfo album = file.Directory;
            DirectoryInfo artist = album.Parent;

            //Gets the index of the arist in the artistPath index and selects them in artist listview.
            int index = artistPaths.FindIndex(x => x.StartsWith(artist.FullName));
            ArtistBox.SelectedIndex = index;
        }

        //Play the selected artist.
        private void artistBox_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Filter the albums/songs to those by the artist.
            if (ArtistBox.SelectedIndex != -1)
            {
                //Get the selected artist.
                DirectoryInfo dir = new DirectoryInfo(artistPaths[ArtistBox.SelectedIndex]);

                //Clear ListViews.
                albumPaths.Clear();
                songPaths.Clear();

                //Create a list to hold items for each listview.
                albumNames = new List<LoadFiles>();
                songNames = new List<string>();

                GetAlbums(dir);

                //Populate the listviews with their respective lists.
                AlbumBox.ItemsSource = albumNames;
                SongBox.ItemsSource = songNames;
            }
            nowPlaying = songPaths.ToList();

            //If the player is currently paused.
            if (Play_Pause.Content == FindResource("Play"))
            {
                //Change icon and play music.
                Play_Pause.Content = FindResource("Pause");
            }
            //Update the now playing data.
            SongBox.SelectedIndex = 0;
            currentlyPlaying = 0;
            GetPlayerInfo(nowPlaying[0]);

        }

        //Play the selected album.
        private void albumBox_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Filter the songs to those on the album.
            if (AlbumBox.SelectedIndex != -1)
            {
                //Get the selected album.
                DirectoryInfo alb = new DirectoryInfo(albumPaths[AlbumBox.SelectedIndex]);

                //Clear ListView.
                songPaths.Clear();
                songNames = new List<string>();

                GetSongs(alb);

                //Populate the listview.
                SongBox.ItemsSource = songNames;
            }
            nowPlaying = songPaths.ToList();

            //If the player is currently paused.
            if (Play_Pause.Content == FindResource("Play"))
            {
                //Change icon and play music.
                Play_Pause.Content = FindResource("Pause");
            }
            //Update the now playing data.
            SongBox.SelectedIndex = 0;
            currentlyPlaying = 0;
            GetPlayerInfo(nowPlaying[0]);
        }

        //Play the selected song.
        private void songBox_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Update the now playing list.
            nowPlaying = songPaths.ToList();

            //If the player is currently paused and a song is selected.
            if (Play_Pause.Content == FindResource("Play") && SongBox.SelectedIndex != -1)
            {
                //Change icon and play music.
                Play_Pause.Content = FindResource("Pause");
            }
            //Update the now playing data.
            currentlyPlaying = SongBox.SelectedIndex;
            GetPlayerInfo(nowPlaying[SongBox.SelectedIndex]);
        }

        //When an artist is selected, filter the albums/songs.
        private void artistBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Filter the albums/songs to those by the artist.
            if (ArtistBox.SelectedIndex != -1)
            {
                //Get the selected artist.
                DirectoryInfo dir = new DirectoryInfo(artistPaths[ArtistBox.SelectedIndex]);

                //Clear ListViews.
                albumPaths.Clear();
                songPaths.Clear();

                //Create a list to hold items for each listview.
                albumNames = new List<LoadFiles>();
                songNames = new List<string>();

                GetAlbums(dir);

                //Populate the listviews with their respective lists.
                AlbumBox.ItemsSource = albumNames;
                SongBox.ItemsSource = songNames;
            }
        }

        //When an album is selected, filter the songs.
        private void albumBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Filter the songs to those on the album.
            if (AlbumBox.SelectedIndex != -1)
            {
                //Get the selected album.
                DirectoryInfo alb = new DirectoryInfo(albumPaths[AlbumBox.SelectedIndex]);

                //Clear ListView.
                songPaths.Clear();
                songNames = new List<string>();

                GetSongs(alb);

                //Populate the listview.
                SongBox.ItemsSource = songNames;
            }
        }

        //Get the artists/albums/songs.
        private void GetArtists()
        {
            //Load directory information
            try
            {
                //Update current directory.
                DirectoryInfo artistDirectory = new DirectoryInfo(directories);

                //Get the artist directories.
                DirectoryInfo[] artistArray = artistDirectory.GetDirectories();

                //Create a list to hold items for each listview.
                List<string> artistNames = new List<string>();
                albumNames = new List<LoadFiles>();
                songNames = new List<string>();

                //Add artists to the list.
                foreach (DirectoryInfo dir in artistArray)
                {
                    artistNames.Add(dir.Name);
                    artistPaths.Add(dir.FullName.ToString());

                    GetAlbums(dir);
                }

                //Populate the listviews with their respective lists.
                ArtistBox.ItemsSource = artistNames;
                AlbumBox.ItemsSource = albumNames;
                SongBox.ItemsSource = songNames;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Get the albums/songs.
        private void GetAlbums(DirectoryInfo dir)
        {
            //Get the album directories.
            DirectoryInfo[] albumArray = dir.GetDirectories();

            //Add albums to the list.
            foreach (DirectoryInfo alb in albumArray)
            {
                //Get the album art.
                var albumArt = alb.GetFiles("*.jpg");

                //Add the Albums and thier art to the list.
                if (albumArt.Length > 0)
                {
                    albumNames.Add(new LoadFiles { Title = alb.Name, ImageData = new BitmapImage(new Uri(albumArt.Last().FullName)) });
                    albumPaths.Add(alb.FullName.ToString());
                }

                GetSongs(alb);
            }
        }

        //Get the songs.
        private void GetSongs(DirectoryInfo alb)
        {
            //Get the song files.
            FileInfo[] songArray = alb.GetFiles("*.mp3");

            //Add songs to the list.
            foreach (FileInfo song in songArray)
            {
                songNames.Add(Title = song.Name);
                songPaths.Add(song.FullName.ToString());
            }
        }

        //Gets info for the currently playing song and display is on the player.
        private void GetPlayerInfo(string filepath)
        {
            player.Open(new Uri(filepath));
            player.Play();

            //Get the paths for the file, album, and artist.
            FileInfo file = new FileInfo(filepath);
            DirectoryInfo album = file.Directory;
            DirectoryInfo artist = album.Parent;

            //Gets the album art.
            var albumArt = album.GetFiles("*.jpg");

            //Updates the player with the current artist/album/song information.
            PlayerArtist.Text = artist.Name;
            PlayerAlbum.ImageSource = new BitmapImage(new Uri(albumArt.Last().FullName));
            PlayerSong.Text = file.Name;
        }
    }
}
