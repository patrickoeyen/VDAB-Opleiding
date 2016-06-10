﻿using System;
using System.Collections.Generic;
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

namespace SchuifSpel
{
    /// <summary>
    /// Interaction logic for SchuifSpelWindow.xaml
    /// </summary>
    public partial class SchuifSpelWindow : Window
    {

        public SchuifSpelWindow()
        {
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Shuffle();
        }


        private void Check()
        {
            int irij, ikolom, grij, gkolom;
            int aantalfout = 0;
            foreach (Image stukje in puzzelGrid.Children)
            {
                irij = Convert.ToInt16(stukje.Name.Substring(4, 1));
                ikolom = Convert.ToInt16(stukje.Name.Substring(5, 1));
                grij = Grid.GetRow(stukje);
                gkolom = Grid.GetColumn(stukje);
                if ((irij != grij) || (ikolom != gkolom))
                {
                    aantalfout++;
                }
            }
            if (aantalfout == 0)
                Oplossing();
        }


        private void OplossingButton_Click(object sender, RoutedEventArgs e)
        {
            Oplossing();
        }


        private void Oplossing()
        {
            puzzelGrid.Children.Clear();
            for (int r = 0; r <= 3; r++)
            {
                for (int k = 0; k <= 3; k++)
                {
                    Image stuk = new Image();
                    BitmapImage bi = new BitmapImage(new Uri(@"images/vdab" + r + k + ".jpg", UriKind.Relative));
                    stuk.Name = "stuk" + r + k;
                    stuk.Source = bi;
                    zetImage(r, k, stuk);
                }
            }
        }

        private void zetImage(int rij, int kolom, Image zetstuk)
        {
            Image stuk = new Image();
            stuk = zetstuk;
            Grid.SetColumn(stuk, kolom);
            Grid.SetRow(stuk, rij);
            //stuk.Drop += puzzelGrid_Drop;
            //stuk.MouseMove += stuk_MouseMove;
            if (stuk.Name == "stuk33")
            {
                
                stuk.AllowDrop = true;
            }
            else
            {
                
                stuk.AllowDrop = false;
            }
            puzzelGrid.Children.Add(stuk);
        }

        private void initStuff(Image stuk)
        {
            if (stuk.Name == "stuk33")
            {
                stuk.Drop += puzzelGrid_Drop;
                stuk.AllowDrop = true;
            }
            else
            {
                stuk.MouseMove += stuk_MouseMove;
                stuk.AllowDrop = false;
            }
        }


        private void Shuffle()
        {
            leegRij = 3;
            leegKolom = 3;
            puzzelGrid.Children.Clear();
            int[,] checken = new int[4, 4];
            for (int r = 0; r <= 3; r++)
            {
                for (int k = 0; k <= 3; k++)
                {
                    checken[r, k] = 0;
                }
            }
            checken[3, 3] = 1;

            Random rnd = new Random();
            int rij, kolom;
            for (int r = 0; r <= 3; r++)
            {
                for (int k = 0; k <= 3; k++)
                {
                    if (k < 3 || r < 3)
                    {
                        do
                        {
                            rij = rnd.Next(0, 4);
                            kolom = rnd.Next(0, 4);
                        } while (checken[rij, kolom] == 1);

                        checken[rij, kolom] = 1;
                        Image stuk = new Image();
                        BitmapImage bi = new BitmapImage(new Uri(@"images/vdab" + r + k + ".jpg", UriKind.Relative));
                        stuk.Name = "stuk" + r + k;
                        stuk.Source = bi;
                        zetImage(rij, kolom, stuk);
                        initStuff(stuk);
                    }
                }
            }

            Image leegstuk = new Image();
            BitmapImage bl = new BitmapImage(new Uri(@"images/leeg33.jpg", UriKind.Relative));
            leegstuk.Name = "stuk33";
            leegstuk.Source = bl;
            zetImage(3, 3, leegstuk);
            initStuff(leegstuk);
        }

        private int leegRij, leegKolom, sleepRij, sleepKolom;

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            Shuffle();
            leegRij = 3;
            leegKolom = 3;
        }

        private void stuk_MouseMove(object sender, MouseEventArgs e)
        {   
            Image stuk = new Image();
            stuk = (Image) sender;
            sleepRij = Grid.GetRow(stuk);
            sleepKolom = Grid.GetColumn(stuk);
            if ((e.LeftButton == MouseButtonState.Pressed) && (stuk.Name != "stuk33"))
            {
                DataObject sleepStuk = new DataObject("hetStuk",stuk);
                DragDrop.DoDragDrop(stuk, sleepStuk, DragDropEffects.Move);
            }

        }

        private void puzzelGrid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("hetStuk"))
            {
                Image gesleepteImage = (Image) e.Data.GetData("hetStuk");
                Image dropzone = (Image) sender;
       
               
                {
                    if (((Math.Abs(sleepRij - leegRij) <= 1) && (Math.Abs(sleepKolom - leegKolom) < 1))||((Math.Abs(sleepRij - leegRij) < 1) && (Math.Abs(sleepKolom - leegKolom) <= 1)))
                    {
                        
                        puzzelGrid.Children.Remove(gesleepteImage);
                        zetImage(leegRij,leegKolom,gesleepteImage);
                        leegRij = sleepRij;
                        leegKolom = sleepKolom;
                        puzzelGrid.Children.Remove(dropzone);
                        zetImage(sleepRij,sleepKolom,dropzone);
                        Check();

                    }
                }
            }
        }

    }
}