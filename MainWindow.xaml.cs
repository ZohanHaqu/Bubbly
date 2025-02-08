using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Bubbly
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private bool isDragging = false;

        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Media files|*.mp4;*.mp3;*.avi;*.mkv;*.wav;*.wma";
            if (openFileDialog.ShowDialog() == true)
            {
                mediaElement.Source = new Uri(openFileDialog.FileName);
                mediaElement.LoadedBehavior = MediaState.Manual;
                mediaElement.UnloadedBehavior = MediaState.Manual;
                mediaElement.Play();
                rewatchButton.IsEnabled = false;
                timer.Start();
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Play();
            timer.Start();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Pause();
            timer.Stop();
        }

        private void SeekSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mediaElement.NaturalDuration.HasTimeSpan && !isDragging)
            {
                mediaElement.Position = TimeSpan.FromSeconds(seekSlider.Value);
                currentTimeTextBlock.Text = TimeSpan.FromSeconds(seekSlider.Value).ToString(@"m\:ss");
            }
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (mediaElement.NaturalDuration.HasTimeSpan)
            {
                seekSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            }
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            playButton.IsEnabled = false;
            rewatchButton.IsEnabled = true;
            timer.Stop();
        }

        private void Rewatch_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Position = TimeSpan.Zero;
            mediaElement.Play();
            playButton.IsEnabled = true;
            rewatchButton.IsEnabled = false;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (mediaElement.NaturalDuration.HasTimeSpan && !isDragging)
            {
                seekSlider.Value = mediaElement.Position.TotalSeconds;
                currentTimeTextBlock.Text = mediaElement.Position.ToString(@"m\:ss");
            }
        }

        private void SeekSlider_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isDragging = true;
        }

        private void SeekSlider_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isDragging = false;
            mediaElement.Position = TimeSpan.FromSeconds(seekSlider.Value);
        }
    }
}
