namespace AutoReload
{
    public partial class MainPage : ContentPage
    {
        private bool isRunning = false;
        // List of websites from local.html
        private List<string> websites = new List<string>
            {
                "https://www.google.com",
                "https://www.youtube.com",
                "https://www.facebook.com",
                "https://www.instagram.com",
                "https://www.twitter.com",
                "https://www.whatsapp.com",
                "https://www.openai.com/chatgpt",
                "https://www.reddit.com",
                "https://www.yahoo.com",
                "https://www.amazon.com",
                "https://account.microsoft.com",
                "https://www.linkedin.com",
                "https://www.netflix.com",
                "https://outlook.live.com",
                "https://www.microsoft.com/microsoft-365",
                "https://www.bing.com",
                "https://www.pinterest.com",
                "https://www.microsoft.com",
                "https://www.twitch.tv",
                "https://www.microsoft.com/sharepoint",
                "https://weather.com",
                "https://www.fandom.com",
                "https://www.duckduckgo.com",
                "https://www.nytimes.com",
                "https://www.zoom.us",
                "https://www.quora.com"
            };


        public MainPage()
        {
            InitializeComponent();
            PopulateWebsitesGrid();
        }

        private void PopulateWebsitesGrid()
        {
            int row = 1; // Start after the header row
            foreach (var website in websites)
            {
                // Checkbox
                var checkBox = new CheckBox
                {
                    HorizontalOptions = LayoutOptions.Start
                };
                checkBox.CheckedChanged += OnWebsiteCheckedChanged;

                // Website Link
                var link = new Label
                {
                    Text = website,
                    TextDecorations = TextDecorations.Underline,
                    TextColor = Colors.Blue,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center
                };
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) => OpenWebsite(website);
                link.GestureRecognizers.Add(tapGesture);

                // Add to Grid
                WebsitesGrid.Add(checkBox, 0, row);
                WebsitesGrid.Add(link, 1, row);

                row++;
            }
        }

        private void OnSelectAllCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            foreach (var child in WebsitesGrid.Children)
            {
                if (child is CheckBox checkBox)
                {
                    checkBox.IsChecked = e.Value;
                }
            }
        }

        private void OnWebsiteCheckedChanged(object? sender, CheckedChangedEventArgs e)
        {
            // Enable or disable the action button based on any checkbox being checked
            ActionButton.IsEnabled = WebsitesGrid.Children.OfType<CheckBox>().Any(cb => cb.IsChecked);
        }

        private async void OnActionButtonClicked(object sender, EventArgs e)
        {
            isRunning = !isRunning;
            ActionButton.Text = isRunning ? "Stop 🛑" : "Start 🚀";

            foreach (var child in WebsitesGrid.Children)
            {
                if (child is CheckBox checkBox)
                {
                    checkBox.IsEnabled = !isRunning;
                    if (checkBox.IsEnabled)
                    {
                        if (Application.Current?.Resources.TryGetValue("Primary", out var color) == true)
                            checkBox.Color = (Color)color;
                    }
                    else
                    {
                        if (Application.Current?.Resources.TryGetValue("Secondary", out var color) == true)
                            checkBox.Color = (Color)color;
                    }
                }
                else if (child is Label label)
                {
                    if (isRunning)
                    {
                        label.TextColor = Colors.Gray;
                        label.GestureRecognizers.Clear();
                    }
                    else
                    {
                        label.TextColor = Colors.Blue;
                        var tapGesture = new TapGestureRecognizer();
                        tapGesture.Tapped += (s, e) => OpenWebsite(label.Text);
                        label.GestureRecognizers.Add(tapGesture);
                    }
                }
            }

            List<string> urlList;

            if (WebsitesGrid.Children.OfType<CheckBox>().First().IsChecked)
            {
                urlList = websites;
            }
            else
            {
                urlList = new List<string>();
                int index = -1; // because there is select-all checkbox at the top 
                foreach (var child in WebsitesGrid.Children)
                {
                    if (child is CheckBox checkBox)
                    {
                        if (checkBox.IsChecked)
                        {
                            urlList.Add(websites[index] + "?nocache=" + DateTime.Now.Ticks);
                        }
                        index++;
                    }
                }
            }

            await Task.Run(async () =>
            {
                while (isRunning)
                {
                    foreach (var url in urlList)
                    {
                        // Open the website on the main thread
                        await MainThread.InvokeOnMainThreadAsync(() => OpenWebsite(url));

                        // Wait for 5 seconds
                        await Task.Delay(1000);
                    }
                }
            });
        }

        private void OpenWebsite(string url)
        {
            try
            {
                WebView.Source = url; // Set the WebView's source to the URL
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Unable to load {url}: {ex.Message}", "OK");
            }
        }
    }

}
