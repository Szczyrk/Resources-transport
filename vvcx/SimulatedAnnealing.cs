using System;

namespace vvcx
{
    public class SimulatedAnnealing
    {
        double T_KONC = 0.01;
        double[][] insert(double[][] originalArray, int positionToInsertAt, double[] ValueToInsert, int sizeOfOriginalArrayX)
        {

            double[][] newArray = new double[sizeOfOriginalArrayX][]; //TWORZENIE TABLICY O ZADANYM ROZMIARZE sizeOfOriginalArrayX


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
        public string wypiszElementy(double[][] tablicaZadan, int iloscZadan, int iloscMaszyn)
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

        double obliczCMax(double[][] tablicaTymczasowa, int iloscZadan, int iloscMaszyn)
        {
            double[][] harmonogram = new double[iloscZadan][];
            for (int i = 0; i < iloscZadan; i++)
                harmonogram[i] = new double[iloscMaszyn];

            double suma = 0;

            for (int i = 0; i < iloscZadan; i++)
                for (int j = 0; j < iloscMaszyn; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        harmonogram[i][j] = tablicaTymczasowa[i][j];
                        suma += harmonogram[i][j];
                    }
                    else if (i == 0 && j != 0)
                    {
                        harmonogram[i][j] = harmonogram[i][j - 1] + tablicaTymczasowa[i][j];
                        suma += harmonogram[i][j];
                    }
                    else if (i != 0 && j == 0)
                    {
                        harmonogram[i][j] = harmonogram[i - 1][j] + tablicaTymczasowa[i][j];
                        suma += harmonogram[i][j];
                    }
                    else
                    {
                        harmonogram[i][j] = Math.Max(harmonogram[i][j - 1], harmonogram[i - 1][j]) + tablicaTymczasowa[i][j];
                        suma += harmonogram[i][j];
                    }
                }
            double cMax = harmonogram[iloscZadan - 1][iloscMaszyn - 1];
            return cMax;
        }

        /** Wersja podstawowa **/

        double czyZamienicKolejnosc(double aktualny, double nowy, double temperatura)
        {
            if (nowy < aktualny)
            {
                return 1;
                //return exp((aktualny - nowy)/temperatura);
            };

            if (nowy >= aktualny)
            {
                return Math.Exp((aktualny - nowy) / temperatura);
            }
            return 0;
        }

        /** Wersja inna

double czyZamienicKolejnosc (int aktualny, int nowy, double temperatura)
{
            if (nowy == aktualny)
            {
                return 0;
            };

            if (nowy != aktualny)
            {
                //return exp((aktualny - nowy)/temperatura);
                return 1;
            }
}

        **/

        double[][] SimulatedAnnealingAlg(double[][] tablicaZadan, double temperatura, int iloscZadan, int iloscMaszyn)
        {
            // Krok 1 - Inicjalizacja

            double mi = 0.99;

            double[][] tablicaPodstawowa = new double[iloscZadan][];
            for (int k = 0; k < iloscZadan; k++)
                tablicaPodstawowa[k] = new double[iloscMaszyn];

            double[][] tablicaZamieniona = new double[iloscZadan][];
            for (int k = 0; k < iloscZadan; k++)
                tablicaZamieniona[k] = new double[iloscMaszyn];

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
                //Sleep(100);

                double cmax1 = obliczCMax(tablicaPodstawowa, iloscZadan, iloscMaszyn);
                ////cout << "cmax1 = " << cmax1 << endl;
                Random rnd = new Random();
                n1 = (rnd.Next() % iloscZadan);
                n2 = (rnd.Next() % iloscZadan);

                while (n1 == n2)
                {
                    n2 = (rnd.Next() % iloscZadan);
                }
                //swap(tablicaZamieniona[n1],tablicaZamieniona[n2]);
                double[] tmp = tablicaZamieniona[n2];
                tablicaZamieniona = insert(tablicaZamieniona, n2, tablicaZamieniona[n1], iloscZadan);
                tablicaZamieniona = insert(tablicaZamieniona, n1, tmp, iloscZadan);
                double cmax2 = obliczCMax(tablicaZamieniona, iloscZadan, iloscMaszyn);
                ////cout << "cmax2 = " << cmax2 << endl;

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
                    ///cout << "Prawdop. zwraca: " << pp << " przeciwko " << sprawdzaniePrawdopodobienstwa << endl;
                    ///cout << "Aktualna temperatura: " << temperatura << endl;
                    ///cout << "Nie zamieniam tablic." << endl;
                    wynikKoncowy = 0;
                }
                else
                {
                    for (int i = 0; i < iloscZadan; i++)
                        for (int j = 0; j < iloscMaszyn; j++)
                            tablicaPodstawowa[i][j] = tablicaZamieniona[i][j];

                    temperatura = temperatura * mi;
                    ///cout << "Prawdop. zwraca: " << pp << " przeciwko " << sprawdzaniePrawdopodobienstwa << endl;
                    //cout << "Aktualna temperatura: " << temperatura << endl;
                    ///cout << "Zamieniam tablice." << endl;
                    wynikKoncowy = 1;
                }

                //wypiszElementy(tablicaPodstawowa, iloscZadan, iloscMaszyn);
                //wypiszElementy(tablicaZamieniona, iloscZadan, iloscMaszyn);
                //system("pause");
            }

            double cMaxKoncowe;

            if (wynikKoncowy == 0)
                cMaxKoncowe = obliczCMax(tablicaPodstawowa, iloscZadan, iloscMaszyn);
            if (wynikKoncowy == 1)
                cMaxKoncowe = obliczCMax(tablicaZamieniona, iloscZadan, iloscMaszyn);

            /* cout << "Cmax = " << cMaxKoncowe << endl;
             fstream output;
             output.open("outputData.txt", ios::out|ios::app);
             output << cMaxKoncowe << endl;
             output.close();
         */
            return tablicaPodstawowa;
        }

        double obliczNEH(double[][] tablicaKolejnosci, int iloscZadan, int iloscMaszyn)
        {
            double czasAktualny = 0;
            double czasMinimalny = 99999;

            double[][] tablicaTymczasowa = new double[iloscZadan][];
            for (int k = 0; k < iloscZadan; k++)
                tablicaTymczasowa[k] = new double[iloscMaszyn];

            for (int i = 0; i < iloscZadan; i++)
                for (int j = 0; j < iloscMaszyn; j++)
                    tablicaTymczasowa[i][j] = tablicaKolejnosci[i][j];

            double[][] harmonogram = new double[iloscZadan][];
            for (int i = 0; i < iloscZadan; i++)
                harmonogram[i] = new double[iloscMaszyn];

            if (iloscZadan == 1)
            {
                for (int j = 0; j < iloscMaszyn; j++)
                {
                    harmonogram[0][j] = tablicaTymczasowa[0][j];
                    czasAktualny += harmonogram[0][j];
                    czasMinimalny = czasAktualny;
                }

                for (int x = 0; x < iloscZadan; x++)
                    for (int z = 0; z < iloscMaszyn; z++)
                        tablicaKolejnosci[x][z] = tablicaTymczasowa[x][z];
            }
            else
            {
                for (int i = 1; i < iloscZadan + 1; i++)
                {
                    if (i != iloscZadan)
                    {
                        for (int n = 0; n < iloscZadan; n++)
                            for (int m = 0; m < iloscMaszyn; m++)
                            {
                                czasAktualny = 0;
                                if (n == 0 && m == 0)
                                {
                                    harmonogram[n][m] = tablicaTymczasowa[n][m];
                                    czasAktualny += harmonogram[n][m];
                                }
                                else if (n == 0 && m != 0)
                                {
                                    harmonogram[n][m] = harmonogram[n][m - 1] + tablicaTymczasowa[n][m];
                                    czasAktualny += harmonogram[n][m];
                                }
                                else if (n != 0 && m == 0)
                                {
                                    harmonogram[n][m] = harmonogram[n - 1][m] + tablicaTymczasowa[n][m];
                                    czasAktualny += harmonogram[n][m];
                                }
                                else
                                {
                                    harmonogram[n][m] = Math.Max(harmonogram[n][m - 1], harmonogram[n - 1][m]) + tablicaTymczasowa[n][m];
                                    czasAktualny += harmonogram[n][m];
                                }
                            }

                        if (czasAktualny <= czasMinimalny)
                        {
                            czasMinimalny = czasAktualny;

                            for (int x = 0; x < iloscZadan; x++)
                                for (int z = 0; z < iloscMaszyn; z++)
                                    tablicaKolejnosci[x][z] = tablicaTymczasowa[x][z];
                        }

                        //swap(tablicaTymczasowa[iloscZadan-i-1],tablicaTymczasowa[iloscZadan-i]);;
                        double[] tmp = tablicaTymczasowa[iloscZadan - i];
                        tablicaTymczasowa = insert(tablicaTymczasowa, iloscZadan - i, tablicaTymczasowa[iloscZadan - i - 1], iloscZadan);
                        tablicaTymczasowa = insert(tablicaTymczasowa, iloscZadan - i - 1, tmp, iloscZadan);
                    }
                    else
                    {
                        for (int n = 0; n < iloscZadan; n++)
                            for (int m = 0; m < iloscMaszyn; m++)
                            {
                                czasAktualny = 0;
                                if (n == 0 && m == 0)
                                {
                                    harmonogram[n][m] = tablicaTymczasowa[n][m];
                                    czasAktualny += harmonogram[n][m];
                                }
                                else if (n == 0 && m != 0)
                                {
                                    harmonogram[n][m] = harmonogram[n][m - 1] + tablicaTymczasowa[n][m];
                                    czasAktualny += harmonogram[n][m];
                                }
                                else if (n != 0 && m == 0)
                                {
                                    harmonogram[n][m] = harmonogram[n - 1][m] + tablicaTymczasowa[n][m];
                                    czasAktualny += harmonogram[n][m];
                                }
                                else
                                {
                                    harmonogram[n][m] = Math.Max(harmonogram[n][m - 1], harmonogram[n - 1][m]) + tablicaTymczasowa[n][m];
                                    czasAktualny += harmonogram[n][m];
                                }
                            }

                        if (czasAktualny <= czasMinimalny)
                        {
                            czasMinimalny = czasAktualny;

                            for (int x = 0; x < iloscZadan; x++)
                                for (int z = 0; z < iloscMaszyn; z++)
                                    tablicaKolejnosci[x][z] = tablicaTymczasowa[x][z];
                        }
                    }
                }
            }

            return czasMinimalny;
        }

        public double[][] algorytmNEH(double[][] tablicaZadan, int iloscZadan, int iloscMaszyn)
        {
            double wagaAktualna = 0;
            int indeksMaksymalnego = 0;
            double wagaMaksymalna = 0;

            double Cmax = 0;

            double[][] tablicaKolejnosci = new double[iloscZadan][];
            for (int k = 0; k < iloscZadan; k++)
                tablicaKolejnosci[k] = new double[iloscMaszyn];

            for (int k = 0; k < iloscZadan; k++)
            {
                wagaMaksymalna = 0;
                for (int i = 0; i < iloscZadan; i++)
                {
                    wagaAktualna = 0;

                    for (int j = 0; j < iloscMaszyn; j++)
                    {
                        wagaAktualna += tablicaZadan[i][j];
                    }
                    if (wagaAktualna > wagaMaksymalna)
                    {
                        wagaMaksymalna = wagaAktualna;
                        indeksMaksymalnego = i;
                    }
                }

                for (int j = 0; j < iloscMaszyn; j++)
                    tablicaKolejnosci[k][j] = tablicaZadan[indeksMaksymalnego][j];

                for (int j = 0; j < iloscMaszyn; j++)
                    tablicaZadan[indeksMaksymalnego][j] = 0;

                double czas = obliczNEH(tablicaKolejnosci, k + 1, iloscMaszyn);

                Cmax = czas;

            }
            //  cout << "Cmax = " << Cmax << endl;

            for (int i = 0; i < 10; i++)
            {
                SimulatedAnnealingAlg(tablicaKolejnosci, 30, iloscZadan, iloscMaszyn);
                //       Sleep(1000);
            }
            return tablicaKolejnosci;
        }
    }
}
