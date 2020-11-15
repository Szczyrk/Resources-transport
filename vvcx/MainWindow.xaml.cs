using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.Maps.MapControl.WPF.Design;
using System.Xml;
using System.Net;
using System.Xml.XPath;
using System.Windows.Controls.Primitives;
using vvcx.BD;
using System.Windows.Media.TextFormatting;


// CO TRZEBA ZROBIC
// 1) Baza danych
// 2) jakieś konwertery czy coś typu paczka cegły -> 100kg , 100kg -> paczka cegły itp itd, i to także powinno być wyświetlane w liście orderdów i przy robieniu zakupów
// 3) Stworzenie macierzy odległości
// 4) Nie jestem webowcem, a Ty siedzisz chyba w webówce jeśli dobrze pamiętam: fajnie jakbyś jakoś ogarnął te zapytania do serwera, bo teraz jezeli damy 10 sklepów to nasra nam naście/kilkadziesiąt zapytań pod rząd i program wtedy muli. Może jakiś async czy coś?
// 5) Ewentualnie poprawki/improvemnt tego co ja tu zrobiłem.
// 6) Twoje własne pomysły :)

// notatka: sorry za bałagan, szczególnie w .xaml. Wiele wierszy/kolumn to pozostałości po poprzednich elementach, a nie chciałem sie bawić w czyszczenie tego, bo cięzko potem wszystko ogarnąć

// CO potrzebujesz: ściągnij sobie odpowiedni komponent do bing maps. wpisz po prostu bing maps c# i na pewno Cie przekieruje do tego komponentu i dodaj go do projektu (jesli nie będzie)
// jest tez property bingMapsKey -> jeśli to Ci nie będzie działało, to wejdź na stronke bing maps i wygeneruj sobie własny klucz.

namespace vvcx
{
    public partial class MainWindow : Window
    {
        enum UIState
        {
            Initial,
            ShopSearch,
            OrderCreation,
            Results
        }

        private string bingMapsKey = "t0rpcLo2qrFih6XYw9HW~LnPj6A47KttiWvGItlMh6Q~AlxR_MQipzzPv_DumNyNBtsPlCrjm3t8GApisOF17CcIxTqdamcHa7CyKMPrcjl0";
        private int counter = 0;

        private GeoLocalHandler geoHandler;
        private Dictionary<string, Shop> shops;
        private static Orders orders;
        private BrushConverter brushConverter;
        public static double[][] distanceMatrix;

        internal static Orders Orders { get => orders; set => orders = value; }

        public MainWindow()
        {
            geoHandler = new GeoLocalHandler(bingMapsKey);
            brushConverter = new BrushConverter();
            shops = new Dictionary<string, Shop>();
            Orders = new Orders();

            // Przykładowe dane, zastapisz to jakos baza danych ogólnie
            SQLite.CreateBD();


            InitializeComponent();
            setState(UIState.Initial);
        }

        private void removePoints_Click(object sender, RoutedEventArgs e)
        {
            addedPoints.Items.Clear();

            for (int i = (myMap.Children.Count - 1); i > 0; i--)
            {
                myMap.Children.RemoveAt(i);
            }
            for (int i = 0; i < Orders.OrdersList.Count; i++)
            {
                Orders.OrdersList.RemoveAt(i);
            }
            Orders.OrdersList.Clear();
                        //// Shop workplace = shops["workplace"];
                        //shops = new Dictionary<string, Shop>();
                        //shops.Add("workplace", workplace);

                        counter = 1;
            resultsLabel.Visibility = Visibility.Collapsed;
            results.Visibility = Visibility.Collapsed;
        }

        private void searchWorkPlaceButtonClicked(object sender, RoutedEventArgs e)
        {
            string city = "";
            string street = "";
            int separatorCount = searchWorkPlace.Text.Split(',').Length - 1;

            if ((separatorCount > 1) || (searchWorkPlace.Text.Length == 0))
            {
                MessageBox.Show("Podana lokalizacja jest błędna \n Poprawny wzór: Miejscowość, Adres");
                return;
            }
            else if (separatorCount < 1)
            {
                city = searchWorkPlace.Text;
            }
            else
            {
                string[] geoData = searchWorkPlace.Text.Split(',');
                city = geoData[0].TrimStart(' ');
                street = geoData[1].TrimStart(' ');
            }

            Tuple<double, double> coordinates = geoHandler.getLocationPoint(city, street);
            Shop workPlace = new Shop(0, "Budowa", city, street, coordinates.Item1, coordinates.Item2, null);
            createAndDisplayPushpin(coordinates.Item1, coordinates.Item2);
            setState(UIState.ShopSearch);
            shops.Add("workplace", workPlace);
            foreach (Shop shop in SQLite.GetShops())
            {
                shops[shop.Name] = shop;
            }
            foreach (var item in shops)
            {
                if (item.Key != "workplace")
                {
                    shopsList.Items.Add(item.Value);
                }
            }
        }

        private void createAndDisplayPushpin(double longitude, double latitude)
        {
            Location location = new Location(longitude, latitude);
            Pushpin pin = new Pushpin();
            pin.Location = location;
            pin.Content = Convert.ToString(counter++);
            myMap.Children.Add(pin);
        }

        private void ShopItemClicked(object sender, SelectionChangedEventArgs e)
        {
            var selectedShop = (Shop)shopsList.SelectedItem;
            productsList.ItemsSource = selectedShop.Products.Select(p => p.Name);
            shopName.Text = selectedShop.Name;
            shopAddress.Text = selectedShop.City + ", " + selectedShop.Address;

            setState(UIState.OrderCreation);
        }

        private void refreshCurrentOrdersList()
        {
            addedPoints.Items.Clear();
            for (int i = 0; i < Orders.OrdersList.Count; i++)
            {
                addedPoints.Items.Add(Orders.OrdersList[i]);
            }
        }

        private void addOrderButtonClicked(object sender, RoutedEventArgs e)
        {
            List<Tuple<Product, int>> orderedProductList = new List<Tuple<Product, int>>();

            for (int i = 0; i < (productsList.Items.Count); i++)
            {
                ListBoxItem productItem = (ListBoxItem)(productsList.ItemContainerGenerator.ContainerFromIndex(i));
                ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(productItem);
                DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;
                TextBox myTextBox = (TextBox)myDataTemplate.FindName("productCount", myContentPresenter);

                if ((myTextBox.Text.Length != 0) && (myTextBox.Text.All(char.IsDigit)))
                {
                    string productName = productItem.Content.ToString();
                    Console.WriteLine(productName);
                    Tuple<Product, int> orderedProduct = new Tuple<Product, int>(shops[shopName.Text].Products.First(p => p.Name == productName), Convert.ToInt32(myTextBox.Text));
                    orderedProductList.Add(orderedProduct);
                }
            }

            if (orderedProductList.Count > 0)
            {
                foreach (Tuple<Product, int> orderedProduct in orderedProductList)
                {
                    Order newOrder = new Order(shops[shopName.Text], orderedProduct);
                    Orders.addOrder(newOrder);
                }

                createAndDisplayPushpin(shops[shopName.Text].Latitude, shops[shopName.Text].Longitude);
                refreshCurrentOrdersList();
                setState(UIState.ShopSearch);
            }
            else
            {
                MessageBox.Show("Nie złożono żadnego zamówienia lub podano błędne ilości.");
            }
        }

        // function source: https://docs.microsoft.com/en-us/dotnet/desktop/wpf/data/how-to-find-datatemplate-generated-elements?redirectedfrom=MSDN&view=netframeworkdesktop-4.8

        private childItem FindVisualChild<childItem>(DependencyObject obj)
         where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                {
                    return (childItem)child;
                }
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        private void cancelOrderButtonPressed(object sender, RoutedEventArgs e)
        {
            setState(UIState.ShopSearch);
        }

        private void startAlgorytmClicked(object sender, RoutedEventArgs e)
        {
            if (shops.Count < 2)
            {
                MessageBox.Show("Podana ilość lokalizacji jest zbyt mała: Trasa musi zawierać minimum 3 punkty");
                return;
            }

            List<Shop> shopList = shops.Select(s => s.Value).ToList();
            distanceMatrix = geoHandler.getDistanceMatrix(shopList);

            SimulatedAnnealing simulatedAnnealing = new SimulatedAnnealing();
            int[][] array = new int[orders.OrdersList.Count][];
            for (int i = 0; i < orders.OrdersList.Count; i++)
                array[i] = new int[1] { i };
            int[][] trasa = simulatedAnnealing.SimulatedAnnealingAlg(array, 30, orders.OrdersList.Count, 1);
            string SA = simulatedAnnealing.wypiszElementy(trasa, orders.OrdersList.Count, 1);
            List<int> tmp = new List<int>();
            for (int i = 0; i < orders.OrdersList.Count; i++)
                tmp.Add(trasa[i][0]);
            MessageBox.Show(SA + "\n" + simulatedAnnealing.CalculateDistanceForOrders(tmp));

        }

        void ShowMatrix(List<Shop> shopList)
        {
            string matrix = "            ";
            matrix += string.Join(" ", shopList.Select(c => string.Format("{0,10}", c.Name)));
            matrix += "\n";
            int rawLength = distanceMatrix.GetLength(0);

            for (int i = 0; i < rawLength; i++)
            {
                matrix += string.Format("{0,10}", shopList[i].Name);
                for (int j = 0; j < rawLength; j++)
                {
                    matrix += string.Format("{0,10}", distanceMatrix[i][j]);
                }
                matrix += "\n";
            }
            MessageBox.Show(matrix);
        }


        private void setState(UIState newState)
        {
            // tak tak, wygląda to tragicznie, ale wolałem brzydką funkcję niż robienie dynamicznych elementów :)

            switch (newState)
            {
                case UIState.Initial:
                    {
                        addedPoints.Visibility = Visibility.Collapsed;
                        addedPointsLabel.Visibility = Visibility.Collapsed;
                        algStackPanel.Visibility = Visibility.Collapsed;
                        shopsListLabel.Visibility = Visibility.Collapsed;
                        shopsList.Visibility = Visibility.Collapsed;
                        resultsLabel.Visibility = Visibility.Collapsed;
                        results.Visibility = Visibility.Collapsed;
                        makeOrder.Visibility = Visibility.Collapsed;
                        shopName.Visibility = Visibility.Collapsed;
                        shopAddress.Visibility = Visibility.Collapsed;
                        productsListLabel.Visibility = Visibility.Collapsed;
                        productsList.Visibility = Visibility.Collapsed;
                        algStackPane1l.Visibility = Visibility.Collapsed;
                        break;
                    }

                case UIState.ShopSearch:
                    {
                        makeOrder.Visibility = Visibility.Collapsed;
                        shopName.Visibility = Visibility.Collapsed;
                        shopAddress.Visibility = Visibility.Collapsed;
                        productsListLabel.Visibility = Visibility.Collapsed;
                        productsList.Visibility = Visibility.Collapsed;
                        algStackPane1l.Visibility = Visibility.Collapsed;
                        workPlacePanel.Visibility = Visibility.Collapsed;
                        addedPointsLabel.Visibility = Visibility.Visible;
                        addedPoints.Visibility = Visibility.Visible;
                        algStackPanel.Visibility = Visibility.Visible;
                        workPlaceLocationLabel.Visibility = Visibility.Collapsed;
                        shopsListLabel.Visibility = Visibility.Visible;
                        shopsList.Visibility = Visibility.Visible;
                        break;
                    }

                case UIState.OrderCreation:
                    {
                        makeOrder.Visibility = Visibility.Visible;
                        shopName.Visibility = Visibility.Visible;
                        shopAddress.Visibility = Visibility.Visible;
                        productsListLabel.Visibility = Visibility.Visible;
                        productsList.Visibility = Visibility.Visible;
                        algStackPane1l.Visibility = Visibility.Visible;
                        break;
                    }

                case UIState.Results:
                    {
                        makeOrder.Visibility = Visibility.Collapsed;
                        shopName.Visibility = Visibility.Collapsed;
                        shopAddress.Visibility = Visibility.Collapsed;
                        productsListLabel.Visibility = Visibility.Collapsed;
                        productsList.Visibility = Visibility.Collapsed;
                        algStackPane1l.Visibility = Visibility.Collapsed;
                        results.Visibility = Visibility.Visible;
                        resultsLabel.Visibility = Visibility.Visible;
                        break;
                    }
            }

        }
    }
}
