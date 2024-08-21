using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace CoinGecko_Asset_Tracker
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        private static readonly HttpClient client = new HttpClient();
        private GridViewColumnHeader _lastHeaderClicked = null;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;
        private List<OverviewCoin> _allOverviewCoins;

        public MainWindow()
        {
            InitializeComponent();
            // Spoof user agent to bypass stupid API '403 Forbidden' errors
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:92.0) Gecko/20100101 Firefox/92.0");
            LoadOnlineImage();
            LoadOverviewCoins();
            LoadTrendingCoins();

            // Start a timer to reload the online ad image every 5 minutes
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMinutes(5);
            _timer.Tick += (s, e) => LoadOnlineImage();
            _timer.Start();
        }

        /// <summary>
        /// Handles the click event of the grid view column header to sort the overview coins list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked != _lastHeaderClicked)
                {
                    direction = ListSortDirection.Ascending;
                }
                else
                {
                    if (_lastDirection == ListSortDirection.Ascending)
                    {
                        direction = ListSortDirection.Descending;
                    }
                    else
                    {
                        direction = ListSortDirection.Ascending;
                    }
                }

                var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

                Sort(sortBy, direction);

                _lastHeaderClicked = headerClicked;
                _lastDirection = direction;
            }
        }

        /// <summary>
        /// Sorts the list of overview coins by the specified property and direction.   
        /// </summary>
        /// <param name="sortBy"></param>
        /// <param name="direction"></param>
        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(OverviewCoinsListBox.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplySearchFilter();
        }


        /// <summary>
        /// Handles the selection changed event of the search criteria combo box to apply the search filter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchCriteriaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplySearchFilter();
        }

        /// <summary>
        /// Applies the search filter to the overview coins list based on the selected search criteria and search text.
        /// </summary>
        private void ApplySearchFilter()
        {
            if (SearchCriteriaComboBox == null || SearchTextBox == null || OverviewCoinsListBox == null || _allOverviewCoins == null)
            {
                return;
            }

            string searchCriteria = (SearchCriteriaComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string searchText = SearchTextBox.Text.ToLower();

            var filteredCoins = _allOverviewCoins.Where(coin =>
            {
                switch (searchCriteria)
                {
                    case "ID":
                        return coin.Id.ToLower().Contains(searchText);
                    case "Symbol":
                        return coin.Symbol.ToLower().Contains(searchText);
                    case "Name":
                        return coin.Name.ToLower().Contains(searchText);
                    default:
                        return false;
                }
            }).ToList();

            OverviewCoinsListBox.ItemsSource = filteredCoins;
        }



        /// <summary>
        /// Updates the timestamp in the status bar.
        /// </summary>
        private void UpdateTimestamp()
        {
            LastUpdateTextBlock.Text = $"Aktualisiert: {DateTime.Now.ToString("G")}";
        }

        /// <summary>
        /// Assigns the list of overview coins to the list box.
        /// </summary>
        private async void LoadOverviewCoins()
        {
            _allOverviewCoins = await GetOverviewCoinsAsync();
            OverviewCoinsListBox.ItemsSource = _allOverviewCoins;
            UpdateTimestamp();
        }


        private async Task<List<OverviewCoin>> GetOverviewCoinsAsync()
        {
            var response = await client.GetStringAsync("https://api.coingecko.com/api/v3/coins/markets?vs_currency=eur&order=market_cap_desc&per_page=250&page=1&sparkline=true");
            var overviewCoins = JsonConvert.DeserializeObject<List<OverviewCoin>>(response);
            return overviewCoins;
        }

        /// <summary>
        /// Assigns the list of trending coins, retrieved from GetTrendingCoinsAsync(), to the list box.
        /// </summary>
        private async void LoadTrendingCoins()
        {
            var trendingCoins = await GetTrendingCoinsAsync();
            TrendingCoinsListBox.ItemsSource = trendingCoins;
            UpdateTimestamp();
        }

        /// <summary>
        /// Retrieves the trending coins from the CoinGecko API asynchronously and returns them as a list of TrendingCoin objects.
        /// </summary>
        /// <returns></returns>
        private async Task<List<TrendingCoinItem>> GetTrendingCoinsAsync()
        {
            var response = await client.GetStringAsync("https://api.coingecko.com/api/v3/search/trending");
            var trendingCoinsResponse = JsonConvert.DeserializeObject<TrendingCoinsResponse>(response);
            var trendingCoins = trendingCoinsResponse.Coins.Select(c => c.Item).ToList();
            return trendingCoins;
        }


        /// <summary>
        /// Handles the selection changed event of the tab control to load the trending or overview coins list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl && ((TabControl)e.Source).SelectedItem is TabItem selectedTab)
            {
                if (selectedTab.Header.ToString() == "Trending")
                {
                    LoadTrendingCoins();
                }
                else if (selectedTab.Header.ToString() == "Übersicht")
                {
                    LoadOverviewCoins();
                }
            }
        }


        /// <summary>
        /// Starts the timer to auto update the trending coins list when the auto update checkbox is checked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoUpdateCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(UpdateIntervalTextBox.Text, out int interval) && interval >= 10)
            {
                _timer.Interval = TimeSpan.FromMinutes(interval);
                _timer.Tick += (s, ev) => LoadTrendingCoins();
                _timer.Start();
            }
            else
            {
                MessageBox.Show("Bitte geben Sie ein gültiges Intervall (mindestens 10 Minuten) ein.");
                AutoUpdateCheckBox.IsChecked = false;
            }
        }

        /// <summary>
        /// Stops the timer when the auto update checkbox is unchecked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoUpdateCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _timer.Tick -= (s, ev) => LoadTrendingCoins();
            _timer.Stop();
        }

        /// <summary>
        /// Handles the click event of the custom minimize button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Handles the click event of the custom close button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Make the window draggable by clicking on the title bar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        /// <summary>
        /// Loads the online image from the specified URL.
        /// </summary>
        private void LoadOnlineImage()
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("https://tpc.googlesyndication.com/simgad/1095257115919094482", UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                OnlineImage.Source = bitmap;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Fehler beim Laden des Bildes: {ex.Message}");
            }
        }

        /// <summary>
        /// Changes the cursor to a hand when the mouse enters the online image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnlineImage_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        /// <summary>
        /// Resets the cursor to the default arrow when the mouse leaves the online image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnlineImage_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Opens the specified URL in the default browser when the online image is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnlineImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://googleads.g.doubleclick.net/pcs/click?xai=AKAOjsshLl0qN2iITZDhutsIy_xSQYk_ggqjFC1ZN_VQlD7PjW3kxR676mX-SVGyCcVtdIBexnLAzwT1V8FVqToik0WRiMBUa6eYuJU_f55TxRIEifqYhXcTL0Cw7c1lzP0O-yB9_5hZi26-awv6UH62gC49o0w0RNye4Du7t4S16i8EYi6DamArsQadbfGILRi-5LcE5RG79cOOaO0wVcKO-RNa3f3r_FpVxiy3W5LcPLS0AyZu4g4l8yjJxIrGjNNuzuzllMqE9DmPveETnxe6a-u1wY1RZf4kmNsXUhrBLak9Y_CkIQTAmaPSVypxXGb9zH_5GzAifje3txWa8s7GU6AQrX9JLZsy0py6Atl9OFXntXqEiC25mYPHLPBY9kZBRHXcR1-pU7gT&sai=AMfl-YTI-bzN3lviV1xFZ8J54JjpNt0fFyab0w7d4z18DUd0mlsCKHoAmCD4JUr1AzuuNLNdyzbCCDI0LBBV20wkWUWPARuzZgbHSAgVI7mZuU-ydxL1iER6hlJyg9rKXEa3JX8WVJkusd-fyb1pFJ5kIxc&sig=Cg0ArKJSzHS_k4SU3rZm&fbs_aeid=%5Bgw_fbsaeid%5D&adurl=https://www.bitmart.com/rewards-hub%3Futm_source%3DCG_ads%26utm_campaign%3Dcoingecko_3000u_rewardshub%26_channel_track_key%3D3BZOtMmD&nm=22&nx=93&ny=-7&mb=2",
                UseShellExecute = true
            });
        }
    }
}
