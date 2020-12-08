using System;
using System.Collections.Generic;

namespace vvcx
{
    public class SimulatedAnnealing
    {
        double T_END = 0.01;
        public double cMaxFinal;

        public double CalculateDistanceForOrders(List<int> OrdersList)
        {
            double distance = 0;
            double sumWeight = 0;
            /*            Console.WriteLine();
                        Console.WriteLine("GetTime: " + OrdersList.Count);
                        Console.WriteLine();*/
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
                if (i - 1 >= 0)
                {
                    if (sumWeight > 0 && orderPrevious.Name != order.Name)
                    {
                        if (4 * MainWindow.distanceMatrix[orderPrevious.Shop.Id][0] > MainWindow.distanceMatrix[orderPrevious.Shop.Id][order.Shop.Id])
                        {
                            distance += MainWindow.distanceMatrix[orderPrevious.Shop.Id][order.Shop.Id];//do nowego sklepu
                            /*                            Console.WriteLine($"{orderPrevious.Name} -> {order.Name} = {MainWindow.distanceMatrix[orderPrevious.Shop.Id][order.Shop.Id]}");*/
                        }
                        else
                        {
                            /*                            Console.WriteLine($"{orderPrevious.Name} -> Budowa = {MainWindow.distanceMatrix[orderPrevious.Shop.Id][0]}");
                                                        Console.WriteLine($"Budowa -> {order.Name} = {MainWindow.distanceMatrix[0][order.Shop.Id]}");*/
                            distance += MainWindow.distanceMatrix[orderPrevious.Shop.Id][0];//do budowy
                            distance += MainWindow.distanceMatrix[0][order.Shop.Id]; // do sklepu
                            sumWeight = 0;
                        }
                    }
                }
                else
                {
                    distance += MainWindow.distanceMatrix[0][order.Shop.Id];
                    /*                    Console.WriteLine($"Budowa -> {order.Shop.Name} = {MainWindow.distanceMatrix[0][order.Shop.Id]}");*/
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
                    /*                    Console.WriteLine($"{product.Name}: {count - numberOfUnpickedProducts} - {(count - numberOfUnpickedProducts) * product.Weight} KG");
                                        Console.WriteLine($"{order.Name} -> Budowa = {MainWindow.distanceMatrix[order.Shop.Id][0]}");
                                        Console.WriteLine($"Budowa -> {order.Name} = {MainWindow.distanceMatrix[0][order.Shop.Id]}");*/
                    count = numberOfUnpickedProducts;
                }
                else
                {
                    /*                    Console.WriteLine($"{product.Name}: {count} - {count * product.Weight} KG");*/
                    count = 0;
                }
                //Console.WriteLine($"Count: {count}");
                if (count > 0)
                    i--;
                else
                    globalNumberOfPickedProducts = 0;
                if (i == OrdersList.Count - 1)
                {
                    /*                    Console.WriteLine($"{order.Name} -> Budowa = {MainWindow.distanceMatrix[order.Shop.Id][0]}");*/
                    distance += (MainWindow.distanceMatrix[order.Shop.Id][0]);
                }
            }
            //Console.WriteLine("Czas na wszystkie zelcenia: " + distance);
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
        public string wypiszElementy(int[][] taskBoard, int numberOfTasks, int numberOfMachines)
        {
            string array = "";
            for (int i = 0; i < numberOfTasks; i++)
            {
                for (int j = 0; j < numberOfMachines; j++)
                {
                    array += taskBoard[i][j] + " ";
                    Console.Write(taskBoard[i][j] + " ");
                }
                array += "\n";
                Console.WriteLine();
            }
            return array;
        }

        double calculateCMax(int[][] temporaryTable, int numberOfTasks, int numberOfMachines)
        {
            List<int> tmp = new List<int>();
            for (int i = 0; i < numberOfTasks; i++)
                tmp.Add(temporaryTable[i][0]);
            return CalculateDistanceForOrders(tmp);
        }


        double DoReorder(double current, double novel, double temperatura)
        {
            if (novel < current)
            {
                return 1;
            }
            //return Math.Exp((current - novel)/temperatura);   
            else
            {
                return Math.Exp((current - novel) / temperatura);
            }
        }


        public int[][] SimulatedAnnealingAlg(int[][] taskBoard, double temperatura, int numberOfTasks, int numberOfMachines)
        {
            // Krok 1 - Inicjalizacja

            double mi = 0.99;
            Random rnd = new Random();
            int[][] basicArray = new int[numberOfTasks][];
            for (int k = 0; k < numberOfTasks; k++)
                basicArray[k] = new int[numberOfMachines];

            int[][] swappedArray = new int[numberOfTasks][];
            for (int k = 0; k < numberOfTasks; k++)
                swappedArray[k] = new int[numberOfMachines];

            for (int i = 0; i < numberOfTasks; i++)
                for (int j = 0; j < numberOfMachines; j++)
                    basicArray[i][j] = taskBoard[i][j];

            for (int i = 0; i < numberOfTasks; i++)
                for (int j = 0; j < numberOfMachines; j++)
                    swappedArray[i][j] = taskBoard[i][j];

            // Krok 2 - generowanie ruchu

            int n1, n2;
            int finalScore = 0;

            while (temperatura > T_END)
            {

                double cmax1 = calculateCMax(basicArray, numberOfTasks, numberOfMachines);
                // Console.WriteLine($"cmax1 = {cmax1}");
                List<Tuple<int, int>> pars = new List<Tuple<int, int>>();
                for (int i = 0; i < rnd.Next() % numberOfTasks; i++)
                {
                    n1 = (rnd.Next() % numberOfTasks);
                    n2 = (rnd.Next() % numberOfTasks);

                    while (n1 == n2 && !pars.Contains(new Tuple<int, int>(n1, n2)))
                    {
                        n2 = (rnd.Next() % numberOfTasks);
                    }
                    pars.Add(new Tuple<int, int>(n1, n2));
                    int[] tmp = swappedArray[n2];
                    swappedArray = insert(swappedArray, n2, swappedArray[n1], numberOfTasks);
                    swappedArray = insert(swappedArray, n1, tmp, numberOfTasks);
                }
                double cmax2 = calculateCMax(swappedArray, numberOfTasks, numberOfMachines);
                // Console.WriteLine($"cmax2 = {cmax2}");

                // Krok 3 - Decyzja: wykonanie

                double pp = DoReorder(cmax1, cmax2, temperatura);

                double probabilityChecking = (rnd.Next() % 100); // generuje losowy numer miedzy 0 a 99
                probabilityChecking = probabilityChecking / 100; // dzieli go przez 100 aby uzyskac zakres <0,1)

                if (pp < probabilityChecking)
                {
                    for (int i = 0; i < numberOfTasks; i++)
                        for (int j = 0; j < numberOfMachines; j++)
                            swappedArray[i][j] = basicArray[i][j];

                    temperatura = temperatura * mi;
                    /*Console.WriteLine($"Prawdop. zwraca: {pp} przeciwko {probabilityChecking}");
                    Console.WriteLine($"Aktualna temperatura: {temperatura}");
                    Console.WriteLine("Nie amieniam tablice.");*/
                    finalScore = 0;
                }
                else
                {
                    for (int i = 0; i < numberOfTasks; i++)
                        for (int j = 0; j < numberOfMachines; j++)
                            basicArray[i][j] = swappedArray[i][j];

                    temperatura = temperatura * mi;
                    /*                   Console.WriteLine($"Prawdop. zwraca: {pp} przeciwko {probabilityChecking}");
                                       Console.WriteLine("Aktualna temperatura: {temperatura}");
                                       Console.WriteLine("Zamieniam tablice.");*/
                    finalScore = 1;
                }

                //wypiszElementy(basicArray, numberOfTasks, numberOfMachines);
                //wypiszElementy(swappedArray, numberOfTasks, numberOfMachines);
            }

            cMaxFinal = calculateCMax(basicArray, numberOfTasks, numberOfMachines);
            Console.WriteLine("Cmax = " + cMaxFinal);
            return basicArray;
        }

    }
}
