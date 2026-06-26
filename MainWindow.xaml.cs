using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private ResponseHandler botBrain = new ResponseHandler();
        private Random random = new Random();
        private List<Brush> backgrounds = new List<Brush>()
        {
            Brushes.DarkSlateBlue, Brushes.DarkOliveGreen, Brushes.DarkCyan, Brushes.DarkRed, Brushes.MidnightBlue
        };

        private string historyFile = "History/chat_history.txt";

        public MainWindow()
        {
            InitializeComponent();
            StartBackgroundAnimation();
            PlayGreeting();
            AddBotMessage("Hello! I'm your Cyber Security assistant, SideKick. What is your name?");
        }

        private void StartBackgroundAnimation()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += (s, e) => { MainGrid.Background = backgrounds[random.Next(backgrounds.Count)]; };
            timer.Start();
        }

        private void PlayGreeting()
        {
            try
            {
                Voicegreeting voice = new Voicegreeting();
                voice.PlayGreeting();
            }
            catch
            {
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string message = UserInput.Text.Trim();

                if (string.IsNullOrWhiteSpace(message))
                {
                    PlayGreeting();
                    AddBotMessage("Please type something to interact with SideKick ");
                    return;
                }

                AddUserMessage(message);
                SaveMessage("USER", message);
                UserInput.Clear();

                await TypingAnimation();

                string botResponse = botBrain.GetResponse(message);
                AddBotMessage(botResponse);
            }
            catch (Exception ex)
            {
                AddBotMessage(" Error: " + ex.Message);
            }
        }

        private void AddBotMessage(string message)
        {
            StackPanel horizontalLayout = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(5) };
            System.Windows.Shapes.Ellipse avatarContainer = new System.Windows.Shapes.Ellipse() { Width = 35, Height = 35, Margin = new Thickness(8, 15, 0, 0), VerticalAlignment = VerticalAlignment.Top };

            try
            {
                // Updated to look for bot.png exactly!
                string imgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bot.png");
                if (File.Exists(imgPath))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imgPath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    avatarContainer.Fill = new ImageBrush(bitmap);
                }
                else
                {
                    avatarContainer.Fill = Brushes.SlateGray;
                }
            }
            catch
            {
                avatarContainer.Fill = Brushes.SlateGray;
            }

            StackPanel messageContent = new StackPanel() { HorizontalAlignment = HorizontalAlignment.Right };
            TextBlock time = new TextBlock() { Text = DateTime.Now.ToString("HH:mm"), Foreground = Brushes.LightGray, FontSize = 11, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 0, 5, 2) };
            Border border = new Border() { Background = Brushes.DarkSlateBlue, CornerRadius = new CornerRadius(15, 0, 15, 15), Padding = new Thickness(12), MaxWidth = 400 };
            TextBlock text = new TextBlock() { Text = "SideKick:\n" + message, Foreground = Brushes.White, FontSize = 14, TextWrapping = TextWrapping.Wrap };

            border.Child = text;
            messageContent.Children.Add(time);
            messageContent.Children.Add(border);
            horizontalLayout.Children.Add(messageContent);
            horizontalLayout.Children.Add(avatarContainer);
            ChatPanel.Children.Add(horizontalLayout);
            SaveMessage("BOT", message);
        }

        private void AddUserMessage(string message)
        {
            StackPanel horizontalLayout = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(5) };
            System.Windows.Shapes.Ellipse avatarContainer = new System.Windows.Shapes.Ellipse() { Width = 35, Height = 35, Margin = new Thickness(0, 15, 8, 0), VerticalAlignment = VerticalAlignment.Top };

            try
            {
                string imgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "user.jpeg");
                if (File.Exists(imgPath))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imgPath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    avatarContainer.Fill = new ImageBrush(bitmap);
                }
                else
                {
                    avatarContainer.Fill = Brushes.LightSeaGreen;
                }
            }
            catch
            {
                avatarContainer.Fill = Brushes.LightSeaGreen;
            }

            StackPanel messageContent = new StackPanel() { HorizontalAlignment = HorizontalAlignment.Left };
            TextBlock time = new TextBlock() { Text = DateTime.Now.ToString("HH:mm"), Foreground = Brushes.LightGray, FontSize = 11, Margin = new Thickness(5, 0, 0, 2) };
            Border border = new Border() { Background = Brushes.Teal, CornerRadius = new CornerRadius(0, 15, 15, 15), Padding = new Thickness(12), MaxWidth = 400 };
            TextBlock text = new TextBlock() { Text = message, Foreground = Brushes.White, FontSize = 14, TextWrapping = TextWrapping.Wrap };

            border.Child = text;
            messageContent.Children.Add(time);
            messageContent.Children.Add(border);

            horizontalLayout.Children.Add(avatarContainer);
            horizontalLayout.Children.Add(messageContent);
            ChatPanel.Children.Add(horizontalLayout);
        }

        private async Task TypingAnimation()
        {
            Border typingBorder = new Border() { Background = Brushes.Gray, CornerRadius = new CornerRadius(10), Padding = new Thickness(10), Margin = new Thickness(5), HorizontalAlignment = HorizontalAlignment.Right };
            TextBlock typingText = new TextBlock() { Text = "SideKick is Typing...", Foreground = Brushes.White };
            typingBorder.Child = typingText;
            ChatPanel.Children.Add(typingBorder);
            await Task.Delay(1200);
            ChatPanel.Children.Remove(typingBorder);
        }

        private void SaveMessage(string sender, string message)
        {
            Directory.CreateDirectory("History");
            string line = $"{DateTime.Now} [{sender}] {message}";
            File.AppendAllText(historyFile, line + Environment.NewLine);
        }
    }
}