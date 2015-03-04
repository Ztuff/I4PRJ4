﻿using System;
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
using ItemList;
using AddItem;
using AddItem = AddItem.AddItem;

namespace FakeMainWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ItemListGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //var slsc = new global::AddItem.AddItem();
            var slsc = new CtrlShowListSelection();
            ItemListGrid.Children.Add(slsc);
        }
    }
}