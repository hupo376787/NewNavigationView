using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using MUXC = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NewNavigationView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(188, 188));
        }

        private Type currentPage;
        
        // List of ValueTuple holding the Navigation Tag and the relative Navigation Page 
        private readonly IList<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("home", typeof(HomePage)),
            ("apps", typeof(AppsPage)),
            ("games", typeof(GamesPage)),
            ("moviestvs", typeof(MovieandTVPage)),
            ("music", typeof(MusicPage)),
            ("books", typeof(BooksPage)),
        };

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // You can also add items in code behind
            //NavView.MenuItems.Add(new NavigationViewItemSeparator());
            //NavView.MenuItems.Add(new NavigationViewItem
            //{
            //    Content = "Settings",
            //    Icon = new SymbolIcon(Symbol.Folder),
            //    Tag = "content"
            //});
            //_pages.Add(("content", typeof(SettingsPage)));

            ContentFrame.Navigated += On_Navigated;

            // NavView doesn't load any page by default: you need to specify it
            NavView_Navigate("home");

            // Add keyboard accelerators for backwards navigation
            var goBack = new KeyboardAccelerator { Key = VirtualKey.GoBack };
            goBack.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(goBack);

            // ALT routes here
            var altLeft = new KeyboardAccelerator
            {
                Key = VirtualKey.Left,
                Modifiers = VirtualKeyModifiers.Menu
            };
            altLeft.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(altLeft);
        }

        private void NavView_ItemInvoked(MUXC.NavigationView sender, MUXC.NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItem == null)
                return;

            if (args.IsSettingsInvoked)
                ContentFrame.Navigate(typeof(SettingsPage));
            else
            {
                // Getting the Tag from Content (args.InvokedItem is the content of NavigationViewItem)
                var navItemTag = NavView.MenuItems
                    .OfType<MUXC.NavigationViewItem>()
                    .First(i => args.InvokedItem.Equals(i.Content))
                    .Tag.ToString();

                NavView_Navigate(navItemTag);
            }
        }

        private void NavView_Navigate(string navItemTag)
        {
            var item = _pages.First(p => p.Tag.Equals(navItemTag));
            if (currentPage == item.Page)
                return;
            ContentFrame.Navigate(item.Page);

            currentPage = item.Page;
        }

        private void NavView_BackRequested(MUXC.NavigationView sender, MUXC.NavigationViewBackRequestedEventArgs args)
        {
            On_BackRequested();
        }

        private void BackInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            On_BackRequested();
            args.Handled = true;
        }

        private bool On_BackRequested()
        {
            if (!ContentFrame.CanGoBack)
                return false;

            // Don't go back if the nav pane is overlayed
            if (NavView.IsPaneOpen &&
                (NavView.DisplayMode == MUXC.NavigationViewDisplayMode.Compact ||
                NavView.DisplayMode == MUXC.NavigationViewDisplayMode.Minimal))
                return false;

            ContentFrame.GoBack();
            return true;
        }

        private void On_Navigated(object sender, NavigationEventArgs e)
        {
            NavView.IsBackEnabled = ContentFrame.CanGoBack;

            if (ContentFrame.SourcePageType == typeof(SettingsPage))
            {
                // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag
                NavView.SelectedItem = (MUXC.NavigationViewItem)NavView.SettingsItem;
            }
            else
            {
                var item = _pages.First(p => p.Page == e.SourcePageType);

                NavView.SelectedItem = NavView.MenuItems
                    .OfType<MUXC.NavigationViewItem>()
                    .First(n => n.Tag.Equals(item.Tag));
            }
        }
    }
}
