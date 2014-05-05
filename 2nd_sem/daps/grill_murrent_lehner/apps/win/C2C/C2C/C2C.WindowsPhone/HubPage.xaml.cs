﻿using C2C.Common;
using C2C.Data;

using System;
using System.Collections.Generic;
using System.Linq;

using Windows.ApplicationModel.Resources;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Universal Hub Application project template is documented at http://go.microsoft.com/fwlink/?LinkID=391955

namespace C2C
{
    using System.Threading.Tasks;

    using Windows.Devices.Enumeration;
    using Windows.Devices.WiFiDirect;
    using Windows.Foundation;
    using Windows.Networking;
    using Windows.Networking.Proximity;
    using Windows.UI.Popups;

    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        Windows.Devices.WiFiDirect.WiFiDirectDevice wiFiDirectDevice;

        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        DeviceInformationCollection devInfoCollection;
        Windows.Devices.WiFiDirect.WiFiDirectDevice wfdDevice;

        public HubPage()
        {
            this.InitializeComponent();

            // Hub is only supported in Portrait orientation
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            this.Loaded += this.HubPageLoaded;
        }

        private async void HubPageLoaded(object sender, RoutedEventArgs e)
        {
            this.GetDevices();
        }

        async void Connect(object sender, RoutedEventArgs e)
        {
            string message = "";

            DeviceInformation chosenDevInfo = null;
            EndpointPair endpointPair = null;
            try
            {

                // Connect to the selected WiFiDirect device
                wfdDevice = await Windows.Devices.WiFiDirect.WiFiDirectDevice.FromIdAsync(chosenDevInfo.Id);

                if (wfdDevice == null)
                {
                    return;
                }

                // Register for Connection status change notification
                wfdDevice.ConnectionStatusChanged += wfdDevice_ConnectionStatusChanged;

                // Get the EndpointPair collection
                var EndpointPairCollection = wfdDevice.GetConnectionEndpointPairs();
                if (EndpointPairCollection.Count > 0)
                {
                    endpointPair = EndpointPairCollection[0];
                }
                else
                {
                    return;
                }

            }
            catch (Exception err)
            {
            }
        }

        void wfdDevice_ConnectionStatusChanged(Windows.Devices.WiFiDirect.WiFiDirectDevice sender, object args)
        {
                    }

        void Disconnect(object sender, RoutedEventArgs e)
        {
            wfdDevice.Dispose();
        }

        async void GetDevices()
        {
            try
            {
                devInfoCollection = null;

                String deviceSelector = Windows.Devices.WiFiDirect.WiFiDirectDevice.GetDeviceSelector();

                devInfoCollection = await DeviceInformation.FindAllAsync(deviceSelector);
                if (devInfoCollection.Count == 0)
                {
                }
                else
                {
                    foreach (var devInfo in devInfoCollection)
                    {
                    }
                    
                }
            }
            catch (Exception err)
            {
            }

        }


        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            var sampleDataGroups = await SampleDataSource.GetGroupsAsync();
            this.DefaultViewModel["Groups"] = sampleDataGroups;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: Save the unique state of the page here.
        }

        /// <summary>
        /// Shows the details of a clicked group in the <see cref="SectionPage"/>.
        /// </summary>
        /// <param name="sender">The source of the click event.</param>
        /// <param name="e">Details about the click event.</param>
        private void GroupSection_ItemClick(object sender, ItemClickEventArgs e)
        {
            var groupId = ((SampleDataGroup)e.ClickedItem).UniqueId;
            if (!Frame.Navigate(typeof(SectionPage), groupId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        /// <summary>
        /// Shows the details of an item clicked on in the <see cref="ItemPage"/>
        /// </summary>
        /// <param name="sender">The source of the click event.</param>
        /// <param name="e">Defaults about the click event.</param>
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            if (!Frame.Navigate(typeof(ItemPage), itemId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}