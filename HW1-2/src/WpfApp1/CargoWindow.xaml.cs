using Logistic.DAL;
using Logistic.Model;
using Logistic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace WpfApp1
{
	/// <summary>
	/// Interaction logic for Cargo.xaml
	/// </summary>
	public partial class CargoWindow : Window
	{
        private readonly Vehicle _selectedVehicle;
        private readonly ObservableCollection<Cargo> _cargos;
        private Action<Cargo> _updateCargo;
        private Action<Cargo> _removeCargo;
        private CargoWindow _cargoWindow;

        public CargoWindow(Vehicle selectedVehicle, ObservableCollection<Cargo> cargos, Action<Cargo> updateCargo, Action<Cargo> removeCargo)
		{
			InitializeComponent();
            _selectedVehicle = selectedVehicle;
            DataContext = this;
            _cargos = cargos;
            _selectedVehicle = selectedVehicle;
            _updateCargo= updateCargo;
            _removeCargo = removeCargo;
            existingCargoListView.ItemsSource = _selectedVehicle.Cargos;
        }
       
        private void SaveDataButton_Click(object sender, RoutedEventArgs e)
        {
            existingCargoListView.Items.Refresh();
            var cargo = new Cargo();
            var invoice = new Invoice()
            {
                Id = Guid.NewGuid(),
                RecipientAddress = RecipientAddressTextBox.Text,
                SenderAddress = SenderAddressTextBox.Text,
                RecipientPhoneNumber = RecipientPhoneNumberTextBox.Text,
                SenderPhoneNumber = SenderPhoneNumberTextBox.Text,
            };

            cargo.Code = CargoCodeTextBox.Text;
            cargo.Volume = double.Parse(CargoVolumeTextBox.Text);
            cargo.Weight = int.Parse(CargoWeightTextBox.Text);
            cargo.Invoice = invoice;
            cargo.Id = Guid.NewGuid();
            _updateCargo(cargo);
            existingCargoListView.Items.Refresh();
            existingCargoListView.ItemsSource = _selectedVehicle.Cargos;
            CargoCodeTextBox.Text = string.Empty;
            CargoWeightTextBox.Text = "0";
            CargoVolumeTextBox.Text = "0";
            RecipientAddressTextBox.Text = string.Empty;
            SenderAddressTextBox.Text = string.Empty;
            RecipientPhoneNumberTextBox.Text = string.Empty;
            SenderPhoneNumberTextBox.Text = string.Empty;
        }

        private void UnloadSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            if (existingCargoListView.SelectedItem != null)
            {
                var cargo = (Cargo)existingCargoListView.SelectedItem;
                _selectedVehicle.Cargos.Remove(cargo);
                existingCargoListView.Items.Refresh();
                _removeCargo(cargo);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CargoCodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                var cargo = DataContext as Cargo;
                if (cargo != null)
                {
                    if (string.IsNullOrEmpty(textBox.Text))
                    {
                        textBox.Foreground = Brushes.Red;
                        MessageBox.Show("Код должен быть задан!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        cargo.Code = textBox.Text;
                        textBox.Foreground = Brushes.Black;
                    }
                }
            }
        }

        private void CargoWeightTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                int cargoWeight;
                if (int.TryParse(textBox.Text, out cargoWeight))
                {
                    textBox.Foreground = Brushes.Black;
                }
                else
                {
                    textBox.Foreground = Brushes.Red;
                    MessageBox.Show("Code must be a number!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CargoVolumeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                var cargo = DataContext as Cargo;
                if (cargo != null)
                {
                    double volume;
                    if (double.TryParse(textBox.Text, out volume))
                    {
                        cargo.Volume = volume;
                        textBox.Foreground = Brushes.Black;
                    }
                    else
                    {
                        textBox.Foreground = Brushes.Red;
                        MessageBox.Show("Объем груза должен быть числом!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }
        
        private void RecipientAddressTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                var invoice = DataContext as Invoice;
                if (invoice != null)
                {
                    if (string.IsNullOrWhiteSpace(textBox.Text) || textBox.Text.Length > 20)
                    {
                        MessageBox.Show("Номер должен содержать не более 6 символов и не быть пустым!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        invoice.RecipientAddress = textBox.Text;
                        if (!string.IsNullOrWhiteSpace(textBox.Text))
                        {
                            textBox.Foreground = Brushes.Black;
                        }
                    }
                }
            }
        }

        private void SenderAddressTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                var invoice = DataContext as Invoice;
                if (invoice != null)
                {
                    if (string.IsNullOrWhiteSpace(textBox.Text) || textBox.Text.Length > 20)
                    {
                        MessageBox.Show("Номер должен содержать не более 6 символов и не быть пустым!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        invoice.SenderAddress = textBox.Text;
                        if (!string.IsNullOrWhiteSpace(textBox.Text))
                        {
                            textBox.Foreground = Brushes.Black;
                        }
                    }
                }
            }
        }

        private void RecipientPhoneNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                var invoice = DataContext as Invoice;
                if (invoice != null)
                {
                    if (string.IsNullOrWhiteSpace(textBox.Text) || textBox.Text.Length > 20)
                    {
                        MessageBox.Show("Номер должен содержать не более 6 символов и не быть пустым!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        invoice.RecipientPhoneNumber = textBox.Text;
                        if (!string.IsNullOrWhiteSpace(textBox.Text))
                        {
                            textBox.Foreground = Brushes.Black;
                        }
                    }
                }
            }
        }

        private void SenderPhoneNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                var invoice = DataContext as Invoice;
                if (invoice != null)
                {
                    if (string.IsNullOrWhiteSpace(textBox.Text) || textBox.Text.Length > 20)
                    {
                        MessageBox.Show("Номер должен содержать не более 6 символов и не быть пустым!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        invoice.SenderPhoneNumber = textBox.Text;
                        if (!string.IsNullOrWhiteSpace(textBox.Text))
                        {
                            textBox.Foreground = Brushes.Black;
                        }
                    }
                }
            }
        }

        private void FillFormFields(Cargo cargo)
        {
            CargoCodeTextBox.Text = cargo.Code;
            CargoWeightTextBox.Text = cargo.Weight.ToString();
            CargoVolumeTextBox.Text = cargo.Volume.ToString();
            RecipientAddressTextBox.Text = cargo.Invoice.RecipientPhoneNumber.ToString();
            SenderAddressTextBox.Text = cargo.Invoice.SenderAddress.ToString();
            RecipientPhoneNumberTextBox.Text = cargo.Invoice.RecipientPhoneNumber.ToString();
            SenderPhoneNumberTextBox.Text = cargo.Invoice.SenderPhoneNumber.ToString();
        }

        private void ExistingCargoListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var selectedCargo = (Cargo)e.AddedItems[0];
                FillFormFields(selectedCargo);
                existingCargoListView.SelectedItem = selectedCargo;
                var selectedItem = existingCargoListView.SelectedItem as Cargo;

                if (selectedItem != null)
                {
                    var item = existingCargoListView.ItemContainerGenerator.ContainerFromItem(selectedItem) as ListViewItem;
                    if (item != null)
                    {
                        item.IsSelected = true;
                    }
                }
                existingCargoListView.DataContext = selectedCargo;
            }
        }
    }
}
