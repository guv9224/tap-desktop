﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TheAirline.Model.AirlinerModel;
using TheAirline.Model.AirlineModel;
using TheAirline.Model.AirportModel;
using TheAirline.Model.GeneralModel;
using TheAirline.GraphicsModel.PageModel.GeneralModel;
using TheAirline.GraphicsModel.PageModel.PageAirlineModel;
using TheAirline.GraphicsModel.UserControlModel.MessageBoxModel;
using TheAirline.GraphicsModel.UserControlModel;
using TheAirline.GraphicsModel.Converters;
using TheAirline.Model.PassengerModel;
using TheAirline.Model.GeneralModel.HolidaysModel;
using TheAirline.Model.GeneralModel.Helpers.WorkersModel;
using TheAirline.Model.GeneralModel.Helpers;
using TheAirline.GraphicsModel.UserControlModel.PopUpWindowsModel;
using System.Windows.Controls.Primitives;
using System.Threading;
using System.Windows.Threading;


namespace TheAirline.GraphicsModel.PageModel.PageGameModel
{
    /// <summary>
    /// Interaction logic for PageStartNewGame.xaml
    /// </summary>
    public partial class PageNewGame : StandardPage
    {
        private TextBox txtName, txtNarrative;
        private TextBlock txtIATA;
        private ContentControl cntCountry;
        private ComboBox cbAirport, cbAirline, cbOpponents, cbStartYear, cbTimeZone, cbDifficulty, cbRegion, cbFocus;
        private ICollectionView airportsView;
        private Rectangle airlineColorRect;
        private Popup popUpSplash;
        private CheckBox cbLocalCurrency;
        public PageNewGame()
        {
            InitializeComponent();

            popUpSplash = new Popup();

            popUpSplash.Child = createSplashWindow();
            popUpSplash.Placement = PlacementMode.Center;
            popUpSplash.PlacementTarget = PageNavigator.MainWindow;
            popUpSplash.IsOpen = false;

                    StackPanel panelContent = new StackPanel();
            panelContent.Margin = new Thickness(10, 0, 10, 0);
            panelContent.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            Panel panelLogo = UICreator.CreateGameLogo();
            panelLogo.Margin = new Thickness(0, 0, 0, 20);

            panelContent.Children.Add(panelLogo);

            TextBlock txtHeader = new TextBlock();
            txtHeader.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            txtHeader.SetResourceReference(TextBlock.BackgroundProperty, "HeaderBackgroundBrush2");
            txtHeader.FontWeight = FontWeights.Bold;
            txtHeader.Uid = "1001";
            txtHeader.Text = Translator.GetInstance().GetString("PageNewGame", txtHeader.Uid);
            panelContent.Children.Add(txtHeader);

            ListBox lbContent = new ListBox();
            lbContent.ItemContainerStyleSelector = new ListBoxItemStyleSelector();
            lbContent.SetResourceReference(ListBox.ItemTemplateProperty, "QuickInfoItem");
            lbContent.MaxHeight = GraphicsHelpers.GetContentHeight()/2;

            txtNarrative = new TextBox();
            txtNarrative.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            txtNarrative.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            txtNarrative.Background = Brushes.Transparent;
            txtNarrative.TextWrapping = TextWrapping.Wrap;
            txtNarrative.FontStyle = FontStyles.Italic;
            txtNarrative.Width = 500;
            txtNarrative.Height = 100;
            txtNarrative.Uid = "1015";
            txtNarrative.IsReadOnly = true;
            txtNarrative.Text = Translator.GetInstance().GetString("PageNewGame", txtNarrative.Uid);
            
            panelContent.Children.Add(txtNarrative);

            panelContent.Children.Add(lbContent);

            cbRegion = new ComboBox();
            cbRegion.SetResourceReference(ComboBox.StyleProperty, "ComboBoxTransparentStyle");
            cbRegion.Width = 200;
            cbRegion.DisplayMemberPath = "Name";
            cbRegion.SelectedValuePath = "Name";

            cbRegion.Items.Add(Regions.GetRegion("100"));
            foreach (Region region in Regions.GetRegions().FindAll(r => Airlines.GetAirlines(r).Count > 0).OrderBy(r => r.Name))
                cbRegion.Items.Add(region);

            cbRegion.SelectionChanged += new SelectionChangedEventHandler(cbRegion_SelectionChanged);

            lbContent.Items.Add(new QuickInfoValue(Translator.GetInstance().GetString("PageNewGame", "1012"), cbRegion));
            // chs, 2011-19-10 added for the possibility of creating a new airline
            WrapPanel panelAirline = new WrapPanel();

            cbAirline = new ComboBox();
            cbAirline.SetResourceReference(ComboBox.ItemTemplateProperty, "AirlineLogoItem");
            cbAirline.SetResourceReference(ComboBox.StyleProperty, "ComboBoxTransparentStyle");
            cbAirline.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            cbAirline.SelectionChanged += new SelectionChangedEventHandler(cbAirline_SelectionChanged);
            cbAirline.Width = 200;

            List<Airline> airlines = Airlines.GetAllAirlines();
            airlines.Sort((delegate(Airline a1, Airline a2) { return a1.Profile.Name.CompareTo(a2.Profile.Name); }));

            cbAirline.ItemsSource = airlines;

            panelAirline.Children.Add(cbAirline);

           

            Button btnAddAirline = new Button();
            btnAddAirline.Margin = new Thickness(5, 0, 0, 0);
            btnAddAirline.Background = Brushes.Transparent;
            btnAddAirline.Click += new RoutedEventHandler(btnAddAirline_Click);

            Image imgAddAirline = new Image();
            imgAddAirline.Source = new BitmapImage(new Uri(@"/Data/images/add.png", UriKind.RelativeOrAbsolute));
            imgAddAirline.Height = 16;
            RenderOptions.SetBitmapScalingMode(imgAddAirline, BitmapScalingMode.HighQuality);

            btnAddAirline.Content = imgAddAirline;

            panelAirline.Children.Add(btnAddAirline);

            lbContent.Items.Add(new QuickInfoValue(Translator.GetInstance().GetString("PageNewGame", "1002"), panelAirline));

            txtIATA = UICreator.CreateTextBlock("");
            lbContent.Items.Add(new QuickInfoValue(Translator.GetInstance().GetString("PageNewGame", "1003"), txtIATA));

            StackPanel panelCountry = new StackPanel();
            cntCountry = new ContentControl();
            cntCountry.SetResourceReference(ContentControl.ContentTemplateProperty, "CountryFlagLongItem");

            panelCountry.Children.Add(cntCountry);

            cbLocalCurrency = new CheckBox();
            cbLocalCurrency.FlowDirection = System.Windows.FlowDirection.RightToLeft;
            cbLocalCurrency.Content = Translator.GetInstance().GetString("PageNewGame", "1014");
            cbLocalCurrency.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
  
            panelCountry.Children.Add(cbLocalCurrency);

            lbContent.Items.Add(new QuickInfoValue(Translator.GetInstance().GetString("PageNewGame", "1004"), panelCountry));

         
            txtName = new TextBox();
            txtName.Background = Brushes.Transparent;
            txtName.BorderBrush = Brushes.Black;
            txtName.Width = 200;


            lbContent.Items.Add(new QuickInfoValue(Translator.GetInstance().GetString("PageNewGame", "1005"), txtName));

            // chs, 2011-19-10 added to show the airline color
            airlineColorRect = new Rectangle();
            airlineColorRect.Width = 40;
            airlineColorRect.Height = 20;
            airlineColorRect.StrokeThickness = 1;
            airlineColorRect.Stroke = Brushes.Black;
            airlineColorRect.Fill = new AirlineBrushConverter().Convert(Airlines.GetAirline("ZA")) as Brush;
            airlineColorRect.Margin = new Thickness(0, 2, 0, 2);

            lbContent.Items.Add(new QuickInfoValue(Translator.GetInstance().GetString("PageNewGame", "1006"), airlineColorRect));

            cbAirport = new ComboBox();
            cbAirport.SetResourceReference(ComboBox.ItemTemplateProperty, "AirportCountryItem");
            cbAirport.SetResourceReference(ComboBox.StyleProperty, "ComboBoxTransparentStyle");
            cbAirport.IsSynchronizedWithCurrentItem = true;
            cbAirport.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            cbAirport.SelectionChanged += new SelectionChangedEventHandler(cbAirports_SelectionChanged);

            List<Airport> airportsList = Airports.GetAllAirports();

            airportsView = CollectionViewSource.GetDefaultView(airportsList);
            airportsView.SortDescriptions.Add(new SortDescription("Profile.Name", ListSortDirection.Ascending));

            cbAirport.ItemsSource = airportsView;

            lbContent.Items.Add(new QuickInfoValue(Translator.GetInstance().GetString("PageNewGame", "1007"), cbAirport));

            cbStartYear = new ComboBox();
            cbStartYear.SetResourceReference(ComboBox.StyleProperty, "ComboBoxTransparentStyle");
            cbStartYear.Width = 60;
            for (int i = GameObject.StartYear; i < DateTime.Now.Year + 2; i++)
                cbStartYear.Items.Add(i);
          
            cbStartYear.SelectionChanged += new SelectionChangedEventHandler(cbStartYear_SelectionChanged);

            lbContent.Items.Add(new QuickInfoValue(Translator.GetInstance().GetString("PageNewGame", "1008"), cbStartYear));

            cbTimeZone = new ComboBox();
            cbTimeZone.SetResourceReference(ComboBox.StyleProperty, "ComboBoxTransparentStyle");
            cbTimeZone.Width = 300;
            cbTimeZone.DisplayMemberPath = "DisplayName";
            cbTimeZone.SelectedValuePath = "DisplayName";


            foreach (GameTimeZone gtz in TimeZones.GetTimeZones())
                cbTimeZone.Items.Add(gtz);

            cbTimeZone.SelectedItem = TimeZones.GetTimeZones().Find(delegate(GameTimeZone gtz) { return gtz.UTCOffset == new TimeSpan(0, 0, 0); });

            lbContent.Items.Add(new QuickInfoValue(Translator.GetInstance().GetString("PageNewGame", "1009"), cbTimeZone));
            
            cbFocus = new ComboBox();
            cbFocus.SetResourceReference(ComboBox.StyleProperty, "ComboBoxTransparentStyle");
            cbFocus.Width = 100;

            foreach (Airline.AirlineFocus focus in Enum.GetValues(typeof(Airline.AirlineFocus)))
                cbFocus.Items.Add(focus);

            cbFocus.SelectedIndex = 0;

            lbContent.Items.Add(new QuickInfoValue(Translator.GetInstance().GetString("PageNewGame", "1013"), cbFocus));

            WrapPanel panelDifficulty = new WrapPanel();

            cbDifficulty = new ComboBox();
            cbDifficulty.SetResourceReference(ComboBox.StyleProperty, "ComboBoxTransparentStyle");
            cbDifficulty.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            cbDifficulty.Width = 100;
            cbDifficulty.DisplayMemberPath = "Name";
            cbDifficulty.SelectedValuePath = "Name";

            foreach (DifficultyLevel difficulty in DifficultyLevels.GetDifficultyLevels())
                cbDifficulty.Items.Add(difficulty);
         
            cbDifficulty.SelectedIndex = 0;

            panelDifficulty.Children.Add(cbDifficulty);

            Button btnAddDifficulty = new Button();
            btnAddDifficulty.Margin = new Thickness(5, 0, 0, 0);
            btnAddDifficulty.Background = Brushes.Transparent;
            btnAddDifficulty.Click += new RoutedEventHandler(btnAddDifficulty_Click);
          
            Image imgAddDifficulty = new Image();
            imgAddDifficulty.Source = new BitmapImage(new Uri(@"/Data/images/add.png", UriKind.RelativeOrAbsolute));
            imgAddDifficulty.Height = 16;
            RenderOptions.SetBitmapScalingMode(imgAddDifficulty, BitmapScalingMode.HighQuality);

            btnAddDifficulty.Content = imgAddDifficulty;

            panelDifficulty.Children.Add(btnAddDifficulty);

            lbContent.Items.Add(new QuickInfoValue(Translator.GetInstance().GetString("PageNewGame", "1011"), panelDifficulty));

            
            cbOpponents = new ComboBox();
            cbOpponents.SetResourceReference(ComboBox.StyleProperty, "ComboBoxTransparentStyle");
            cbOpponents.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            cbOpponents.Width = 50;
            cbOpponents.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
            for (int i = 0; i < Airlines.GetAllAirlines().Count; i++)
                cbOpponents.Items.Add(i);

            cbOpponents.SelectedIndex = 3;

            lbContent.Items.Add(new QuickInfoValue(Translator.GetInstance().GetString("PageNewGame", "1010"), cbOpponents));


            WrapPanel panelButtons = new WrapPanel();
            panelButtons.Margin = new Thickness(0, 5, 0, 0);
            panelContent.Children.Add(panelButtons);


            Button btnCreate = new Button();
            btnCreate.SetResourceReference(Button.StyleProperty, "RoundedButton");
            btnCreate.Click += new RoutedEventHandler(btnCreate_Click);
            btnCreate.Height = Double.NaN;
            btnCreate.Width = Double.NaN;
            btnCreate.Uid = "201";
            btnCreate.Content = Translator.GetInstance().GetString("PageNewGame", btnCreate.Uid);
            btnCreate.SetResourceReference(Button.BackgroundProperty, "ButtonBrush");
            panelButtons.Children.Add(btnCreate);

            Button btnExit = new Button();
            btnExit.SetResourceReference(Button.StyleProperty, "RoundedButton");
            btnExit.Height = double.NaN;
            btnExit.Width = double.NaN;
            btnExit.Uid = "202";
            btnExit.Content = Translator.GetInstance().GetString("PageNewGame", btnExit.Uid);
            btnExit.Margin = new Thickness(5, 0, 0, 0);
            btnExit.Click += new RoutedEventHandler(btnCancel_Click);
            btnExit.SetResourceReference(Button.BackgroundProperty, "ButtonBrush");
            panelButtons.Children.Add(btnExit);

            base.setTopMenu(new PageTopMenu());

            base.hideNavigator();

            base.hideBottomMenu();

            base.setContent(panelContent);

            base.setHeaderContent(Translator.GetInstance().GetString("PageNewGame", "200"));

            cbStartYear.SelectedItem = DateTime.Now.Year;
        
          
        
            showPage(this);



        }
        //creates the splash window
        private Border createSplashWindow()
        {
        
            Border brdSplasInner = new Border();
            brdSplasInner.BorderBrush = Brushes.Black;
            brdSplasInner.BorderThickness = new Thickness(2, 2, 0, 0);

            Border brdSplashOuter = new Border();
            brdSplashOuter.BorderBrush = Brushes.White;
            brdSplashOuter.BorderThickness = new Thickness(0,0,2,2);

            brdSplasInner.Child = brdSplashOuter;

            Image imgSplash = new Image();
            imgSplash.Source = new BitmapImage(new Uri(AppSettings.getDataPath() + "\\graphics\\TheAirlne_Splash.jpg", UriKind.RelativeOrAbsolute));
            imgSplash.Height = 200;
            imgSplash.Width = 400;
            RenderOptions.SetBitmapScalingMode(imgSplash, BitmapScalingMode.HighQuality);

            brdSplashOuter.Child = imgSplash;

         
            return brdSplasInner;

        }
      
        private void cbRegion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Region region = (Region)cbRegion.SelectedItem;

            if (region == null)
            {
                cbRegion.SelectedIndex = 0;
                region = (Region)cbRegion.SelectedItem;
            }

            var source = cbAirline.Items as ICollectionView;
            source.Filter = delegate(object item)
            {
                var airline = item as Airline;
                return airline.Profile.Country.Region == region || region.Uid == "100";

            };
            source.Refresh();

            cbAirline.SelectedIndex = 0;

        
            cbOpponents.Items.Clear();

            for (int i = 0; i < cbAirline.Items.Count; i++)
                cbOpponents.Items.Add(i);

            cbOpponents.SelectedIndex = Math.Min(cbOpponents.Items.Count-1, 3);
        }


        private void cbStartYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Region region = (Region)cbRegion.SelectedItem;
            if (region == null)
            {
                cbRegion.SelectedIndex = 0;
                region = (Region)cbRegion.SelectedItem;
            }
            
            int year = (int)cbStartYear.SelectedItem;
            
            var source = cbAirline.Items as ICollectionView;
            source.Filter = delegate(object item)
            {
                var airline = item as Airline;
                return (airline.Profile.Country.Region == region || region.Uid == "100") && airline.Profile.Founded<=year && airline.Profile.Folded>year;

            };

            
            source.Refresh();

            cbAirline.SelectedIndex = 0;
  
            setAirportsView(year, ((Airline)cbAirline.SelectedItem).Profile.Country);

            cbOpponents.Items.Clear();

            for (int i = 0; i < cbAirline.Items.Count; i++)
                cbOpponents.Items.Add(i);

            cbOpponents.SelectedIndex = Math.Min(cbOpponents.Items.Count - 1, 3);

        }

        private void cbDifficulty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            Airline airline = (Airline)cbDifficulty.SelectedItem;
            int year = (int)cbStartYear.SelectedItem;

            setAirportsView(year, airline.Profile.Country);
        }



        private void cbAirline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            Airline airline = (Airline)cbAirline.SelectedItem;

            if (airline != null)
            {
                int year = (int)cbStartYear.SelectedItem;

                setAirportsView(year, airline.Profile.Country);

                if (airline.Profile.PreferedAirport != null && cbAirport.Items.Contains(airline.Profile.PreferedAirport))
                    cbAirport.SelectedItem = airline.Profile.PreferedAirport;
                else
                {
                    var aa = cbAirport.Items.Cast<Airport>().ToList();
                    Airport homeAirport =  aa.Find(a => a.Profile.Country == airline.Profile.Country);

                    cbAirport.SelectedItem = homeAirport == null ? cbAirport.Items[0] : homeAirport;
                }

                airlineColorRect.Fill = new AirlineBrushConverter().Convert(airline) as Brush;
                txtName.Text = airline.Profile.CEO;
                txtIATA.Text = airline.Profile.IATACode;
                cntCountry.Content = airline.Profile.Country;
                cbLocalCurrency.Visibility = airline.Profile.Country.Currencies.Count > 0  ? Visibility.Visible : System.Windows.Visibility.Collapsed;
                cbLocalCurrency.IsChecked = airline.Profile.Country.Currencies.Count>0;

                txtNarrative.Text = airline.Profile.Narrative;    
            }

        }
        private void btnAddAirline_Click(object sender, RoutedEventArgs e)
        {
            PageNavigator.NavigateTo(new PageNewAirline());

        }
        private void btnAddDifficulty_Click(object sender, RoutedEventArgs e)
        {

           object o = PopUpDifficulty.ShowPopUp((DifficultyLevel)cbDifficulty.SelectedItem);

           if (o != null && o is DifficultyLevel)
           {
               DifficultyLevel level = (DifficultyLevel)o;

               if (DifficultyLevels.GetDifficultyLevel("Custom") != null)
               {
                   DifficultyLevel customLevel = DifficultyLevels.GetDifficultyLevel("Custom");

                   DifficultyLevels.RemoveDifficultyLevel(customLevel);
                   cbDifficulty.Items.Remove(customLevel);
               }

               DifficultyLevels.AddDifficultyLevel(level);

               cbDifficulty.Items.Add(level);
               cbDifficulty.SelectedItem = level;
           }
        }



        private void cbAirports_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTimeZone != null)
            {
                Airport airport = (Airport)cbAirport.SelectedItem;
                cbTimeZone.SelectedItem = airport.Profile.TimeZone;
            }


        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            PageNavigator.NavigateTo(new PageFrontMenu());

        }
        public void DoEvents()
        {
            DispatcherFrame f = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
            (SendOrPostCallback)delegate(object arg)
            {
                DispatcherFrame fr = arg as DispatcherFrame;
                fr.Continue = false;
            }, f);
            Dispatcher.PushFrame(f); 
        }
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {

            if (txtName.Text.Trim().Length > 2)
            {
                popUpSplash.IsOpen = true;

                DoEvents();
 
                GameTimeZone gtz = (GameTimeZone)cbTimeZone.SelectedItem;
                GameObject.GetInstance().TimeZone = gtz;
                GameObject.GetInstance().Difficulty = (DifficultyLevel)cbDifficulty.SelectedItem;
                int startYear = (int)cbStartYear.SelectedItem;
                GameObject.GetInstance().GameTime = new DateTime(startYear, 1, 1);
                GameObject.GetInstance().StartDate = GameObject.GetInstance().GameTime;
                //sets the fuel price
                GameObject.GetInstance().FuelPrice = Inflations.GetInflation(GameObject.GetInstance().GameTime.Year).FuelPrice;

                int opponents = (int)cbOpponents.SelectedItem;
                Airline airline = (Airline)cbAirline.SelectedItem;

                airline.Profile.CEO = txtName.Text.Trim();

                GameObject.GetInstance().HumanAirline = airline;
                GameObject.GetInstance().MainAirline = GameObject.GetInstance().HumanAirline;

                if (cbLocalCurrency.IsChecked.Value)
                    GameObject.GetInstance().CurrencyCountry = airline.Profile.Country;
               // AppSettings.GetInstance().resetCurrencyFormat();

                Airport airport = (Airport)cbAirport.SelectedItem;
                
                airport.Terminals.rentGate(airline);
                airport.Terminals.rentGate(airline);

                AirportFacility checkinFacility = AirportFacilities.GetFacilities(AirportFacility.FacilityType.CheckIn).Find(f => f.TypeLevel == 1);
                AirportFacility facility = AirportFacilities.GetFacilities(AirportFacility.FacilityType.Service).Find((delegate(AirportFacility f) { return f.TypeLevel == 1; }));

                airport.addAirportFacility(GameObject.GetInstance().HumanAirline, facility, GameObject.GetInstance().GameTime);
                airport.addAirportFacility(GameObject.GetInstance().HumanAirline, checkinFacility, GameObject.GetInstance().GameTime);

                Region region = (Region)cbRegion.SelectedItem;

                if (region.Uid != "100")
                {
                    Airports.RemoveAirports(a => a.Profile.Country.Region != region);
                    Airlines.RemoveAirlines(a => a.Profile.Country.Region != region);
                }

                PassengerHelpers.CreateDestinationPassengers();

                AirlinerHelpers.CreateStartUpAirliners();

                Setup.SetupTestGame(opponents);

                airline.MarketFocus = (Airline.AirlineFocus)cbFocus.SelectedItem;

                GeneralHelpers.CreateHolidays(GameObject.GetInstance().GameTime.Year);
                GameTimer.GetInstance().start();
                GameObjectWorker.GetInstance().start();
               // AIWorker.GetInstance().start();

                PageNavigator.NavigateTo(new PageAirline(GameObject.GetInstance().HumanAirline));

                PageNavigator.ClearNavigator();

               // GameObject.GetInstance().HumanAirline.Money = 1000000000;

                GameObject.GetInstance().NewsBox.addNews(new News(News.NewsType.Standard_News, GameObject.GetInstance().GameTime, Translator.GetInstance().GetString("News", "1001"), string.Format(Translator.GetInstance().GetString("News", "1001", "message"), GameObject.GetInstance().HumanAirline.Profile.CEO, GameObject.GetInstance().HumanAirline.Profile.IATACode)));

                popUpSplash.IsOpen = false;
            }
            else
                WPFMessageBox.Show(Translator.GetInstance().GetString("MessageBox", "2403"), Translator.GetInstance().GetString("MessageBox", "2403"), WPFMessageBoxButtons.Ok);

        }
        //sets the airports view
        private void setAirportsView(int year, Country country)
        {
            GameObject.GetInstance().GameTime = new DateTime(year, 1, 1);

            try
            {
                airportsView.Filter = o =>
                {
                    Airport a = o as Airport;
                    return ((Country)new CountryCurrentCountryConverter().Convert(a.Profile.Country)) == (Country)new CountryCurrentCountryConverter().Convert(country) && GeneralHelpers.IsAirportActive(a);// && a.Terminals.getNumberOfGates() > 10 //a.Profile.Period.From<=GameObject.GetInstance().GameTime && a.Profile.Period.To > GameObject.GetInstance().GameTime;
                };
            }
            catch (Exception ex)
            {
                string exception = ex.ToString();
            }

            if (cbAirport.SelectedIndex == -1) cbAirport.SelectedIndex = 0;
        }


    }

}
