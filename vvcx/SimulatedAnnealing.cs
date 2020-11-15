using System;
using System.Collections.Generic;

namespace vvcx
{
    public class SimulatedAnnealing
    {
        double T_KONC = 0.01;

        public double CalculateDistanceForOrders(List<int> OrdersList)
        {
            double distance = 0;
            double sumWeight = 0;
            Console.WriteLine();
            Console.WriteLine("GetTime: " + OrdersList.Count);
            Console.WriteLine();
            int globalNumberOfPickedProducts = 0;
            for (int i = 0; i < OrdersList.Count; i++)
            {
                
                Order order = MainWindow.Orders.OrdersList[OrdersList[i]];
                Order orderPrevious = null;
                Order orderNext = null;
                if (i - 1 >= 0)
                    orderPrevious = MainWindow.Orders.OrdersList[OrdersList[i - 1]];
                if (i + 1 < OrdersList.Count)
                    orderNext = MainWindow.Orders.OrdersList[OrdersList[i + 1]];

                if (i + 1 != order.Shop.Id)
                    Console.WriteLine($"Inedks: {i + 1} Shop ID: {order.Shop.Id}");
                if (sumWeight > 0)
                {
                    if (MainWindow.distanceMatrix[orderPrevious.Shop.Id][0] > MainWindow.distanceMatrix[orderPrevious.Shop.Id][order.Shop.Id])
                    {
                        distance += MainWindow.distanceMatrix[orderPrevious.Shop.Id][order.Shop.Id];//do nowego sklepu
                        Console.WriteLine($"{orderPrevious.Name} -> {order.Name} = {MainWindow.distanceMatrix[orderPrevious.Shop.Id][order.Shop.Id]}");
                    }
                    else
                    {
                        Console.WriteLine($"{orderPrevious.Name} -> Budowa = {MainWindow.distanceMatrix[orderPrevious.Shop.Id][0]}");
                        Console.WriteLine($"Budowa -> {order.Name} = {MainWindow.distanceMatrix[0][order.Shop.Id]}");
                        distance += MainWindow.distanceMatrix[orderPrevious.Shop.Id][0];//do budowy
                        distance += MainWindow.distanceMatrix[0][order.Shop.Id]; // do sklepu
                        sumWeight = 0;
                    }
                }
                else
                {
                    distance += MainWindow.distanceMatrix[0][order.Shop.Id];
                    Console.WriteLine($"Budowa -> {order.Shop.Name} = {MainWindow.distanceMatrix[0][order.Shop.Id]}");
                }
                  

                Product product = order.OrderedProducts.Item1;
                int count = order.OrderedProducts.Item2 - globalNumberOfPickedProducts;
                //Console.WriteLine($"Count: {count}   {order.OrderedProducts[j].Item2}  -   {globalNumberOfPickedProducts}");
                sumWeight += count * product.Weight;

                if (sumWeight >= 1000)
                {
                    int numberOfUnpickedProducts = 0;
                    distance += MainWindow.distanceMatrix[order.Shop.Id][0];

                    while (sumWeight > 1000)
                    {
                        numberOfUnpickedProducts++;
                        sumWeight -= product.Weight;
                    }
                    globalNumberOfPickedProducts += (count - numberOfUnpickedProducts);

                    sumWeight = 0;
                    Console.WriteLine($"{product.Name}: {count - numberOfUnpickedProducts} - {(count - numberOfUnpickedProducts) * product.Weight} KG");
                    Console.WriteLine($"{order.Name} -> Budowa = {MainWindow.distanceMatrix[order.Shop.Id][0]}");
                    Console.WriteLine($"Budowa -> {order.Name} = {MainWindow.distanceMatrix[0][order.Shop.Id]}");
                    count = numberOfUnpickedProducts;
                }
                else
                {
                    Console.WriteLine($"{product.Name}: {count} - {count * product.Weight} KG");
                    count = 0;
                }
                //Console.WriteLine($"Count: {count}");
                if (count > 0)
                    i--;
                else
                    globalNumberOfPickedProducts = 0;
                if (i == OrdersList.Count - 1)
                {
                    Console.WriteLine($"{order.Name} -> Budowa = {MainWindow.distanceMatrix[order.Shop.Id][0]}");
                    distance += (MainWindow.distanceMatrix[order.Shop.Id][0]);
                }
            }
            Console.WriteLine("Czas na wszystkie zelcenia: " + distance);
            return distance;
        }

        int[][] insert(int[][] originalArray, int positionToInsertAt, int[] ValueToInsert, int sizeOfOriginalArrayX)
        {

            int[][] newArray = new int[sizeOfOriginalArrayX][]; //TWORZENIE TABLICY O ZADANYM ROZMIARZE sizeOfOriginalArrayX


            for (int i = 0; i < sizeOfOriginalArrayX; ++i)
            {
                if (i < positionToInsertAt) //JESLI MNIEJSZE TO PRZEPISUJEMY ELEMENTY Z TABLICY
                {
                    newArray[i] = originalArray[i];
                }

                if (i == positionToInsertAt) //JESLI ROWNE TO WSTAWIAMY DANY ELEMENT ValueToInsert
                {
                    newArray[i] = ValueToInsert;
                }

                if (i > positionToInsertAt) //JESLI WIEKSZE TO PRZEPISUJEMY ELEMENTY Z TABLICY
                {
                    newArray[i] = originalArray[i];
                }
            }
            return newArray; //ZWRACA NOWA TABLICE
        }
        public string wypiszElementy(int[][] tablicaZadan, int iloscZadan, int iloscMaszyn)
        {
            string array = "";
            for (int i = 0; i < iloscZadan; i++)
            {
                for (int j = 0; j < iloscMaszyn; j++)
                {
                    array += tablicaZadan[i][j] + " ";
                    Console.Write(tablicaZadan[i][j] + " ");
                }
                array += "\n";
                Console.WriteLine();
            }
            return array;
        }

        double obliczCMax(int[][] tablicaTymczasowa, int iloscZadan, int iloscMaszyn)
        {
            List<int> tmp = new List<int>();
            for (int i = 0; i < iloscZadan; i++)
                tmp.Add(tablicaTymczasowa[i][0]);
            return CalculateDistanceForOrders(tmp);
        }


        double czyZamienicKolejnosc(double aktualny, double nowy, double temperatura)
        {
            if (nowy < aktualny)
            {
                return 1;
                //return Math.Exp((aktualny - nowy)/temperatura);
            };

            if (nowy >= aktualny)
            {
                return Math.Exp((aktualny - nowy) / temperatura);
            }
            return 0;
        }


        public int[][] SimulatedAnnealingAlg(int[][] tablicaZadan, double temperatura, int iloscZadan, int iloscMaszyn)
        {
            // Krok 1 - Inicjalizacja

            double mi = 0.99;

            int[][] tablicaPodstawowa = new int[iloscZadan][];
            for (int k = 0; k < iloscZadan; k++)
                tablicaPodstawowa[k] = new int[iloscMaszyn];

            int[][] tablicaZamieniona = new int[iloscZadan][];
            for (int k = 0; k < iloscZadan; k++)
                tablicaZamieniona[k] = new int[iloscMaszyn];

            for (int i = 0; i < iloscZadan; i++)
                for (int j = 0; j < iloscMaszyn; j++)
                    tablicaPodstawowa[i][j] = tablicaZadan[i][j];

            for (int i = 0; i < iloscZadan; i++)
                for (int j = 0; j < iloscMaszyn; j++)
                    tablicaZamieniona[i][j] = tablicaZadan[i][j];

            // Krok 2 - generowanie ruchu

            int n1, n2;
            int wynikKoncowy = 0;

            while (temperatura > T_KONC)
            {

                double cmax1 = obliczCMax(tablicaPodstawowa, iloscZadan, iloscMaszyn);
                // Console.WriteLine($"cmax1 = {cmax1}");
                Random rnd = new Random();
                n1 = (rnd.Next() % iloscZadan);
                n2 = (rnd.Next() % iloscZadan);

                while (n1 == n2)
                {
                    n2 = (rnd.Next() % iloscZadan);
                }
                int[] tmp = tablicaZamieniona[n2];
                tablicaZamieniona = insert(tablicaZamieniona, n2, tablicaZamieniona[n1], iloscZadan);
                tablicaZamieniona = insert(tablicaZamieniona, n1, tmp, iloscZadan);
                double cmax2 = obliczCMax(tablicaZamieniona, iloscZadan, iloscMaszyn);
                // Console.WriteLine($"cmax2 = {cmax2}");

                // Krok 3 - Decyzja: wykonanie

                double pp = czyZamienicKolejnosc(cmax1, cmax2, temperatura);

                double sprawdzaniePrawdopodobienstwa = (rnd.Next() % 101); // generuje losowy numer miedzy 0 a 100
                sprawdzaniePrawdopodobienstwa = sprawdzaniePrawdopodobienstwa / 100; // dzieli go przez 100 aby uzyskac zakres <0,1>

                if (pp < sprawdzaniePrawdopodobienstwa)
                {
                    for (int i = 0; i < iloscZadan; i++)
                        for (int j = 0; j < iloscMaszyn; j++)
                            tablicaZamieniona[i][j] = tablicaPodstawowa[i][j];

                    temperatura = temperatura * mi;
/*                    Console.WriteLine($"Prawdop. zwraca: {pp} przeciwko {sprawdzaniePrawdopodobienstwa}");
                    Console.WriteLine("Aktualna temperatura: {temperatura}");
                    Console.WriteLine("Nie amieniam tablice.");*/
                    wynikKoncowy = 0;
                }
                else
                {
                    for (int i = 0; i < iloscZadan; i++)
                        for (int j = 0; j < iloscMaszyn; j++)
                            tablicaPodstawowa[i][j] = tablicaZamieniona[i][j];

                    temperatura = temperatura * mi;
 /*                   Console.WriteLine($"Prawdop. zwraca: {pp} przeciwko {sprawdzaniePrawdopodobienstwa}");
                    Console.WriteLine("Aktualna temperatura: {temperatura}");
                    Console.WriteLine("Zamieniam tablice.");*/
                    wynikKoncowy = 1;
                }

                //wypiszElementy(tablicaPodstawowa, iloscZadan, iloscMaszyn);
                //wypiszElementy(tablicaZamieniona, iloscZadan, iloscMaszyn);
            }

            double cMaxKoncowe = 0;

            if (wynikKoncowy == 0)
                cMaxKoncowe = obliczCMax(tablicaPodstawowa, iloscZadan, iloscMaszyn);
            if (wynikKoncowy == 1)
                cMaxKoncowe = obliczCMax(tablicaZamieniona, iloscZadan, iloscMaszyn);

            Console.WriteLine("Cmax = " + cMaxKoncowe);

            return tablicaPodstawowa;
        }

    }
}
