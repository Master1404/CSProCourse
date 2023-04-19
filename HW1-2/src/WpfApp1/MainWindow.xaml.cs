using Logistic.Core;
using Logistic.DAL;
using Logistic.Model;
using Logistic.Models;
using Logistic.Models.Enum;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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

namespace WpfApp1
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        ObservableCollection<Cargo> Cargos = new ObservableCollection<Cargo>();
        ObservableCollection<Vehicle> Vehicles = new ObservableCollection<Vehicle>();
        private VehicleService _vehicleService;
        private InMemoryRepository<Vehicle> _memoryRepository;
        public MainWindow()
        {
            InitializeComponent();
            // создаем экземпляр Vehicle и устанавливаем его в качестве DataContext
            var vehicle = new Vehicle();
            this.DataContext = vehicle;
            _memoryRepository = new InMemoryRepository<Vehicle>(vehicle => vehicle.Id);
            vehicleListView.ItemsSource = Vehicles;
            _vehicleService = new VehicleService(_memoryRepository);
        }

        
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                // Получаем выбранный элемент
                var selectedVehicle = (Vehicle)e.AddedItems[0];

                // Заполняем поля формы
                FillFormFields(selectedVehicle);

                // Выделяем выбранный элемент в ListView
                vehicleListView.SelectedItem = selectedVehicle;
                var selectedItem = vehicleListView.SelectedItem as Vehicle;

                if (selectedItem != null)
                {
                    var item = vehicleListView.ItemContainerGenerator.ContainerFromItem(selectedItem) as ListViewItem;
                    if (item != null)
                    {
                        item.IsSelected = true;
                    }
                }

                vehicleListView.DataContext = selectedVehicle;
            }
        }

        private void NumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                var vehicle = DataContext as Vehicle;
                if (vehicle != null)
                {
                    if (string.IsNullOrWhiteSpace(textBox.Text) || textBox.Text.Length > 20)
                    {
                        MessageBox.Show("Номер должен содержать не более 6 символов и не быть пустым!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        vehicle.Number = textBox.Text;
                        if (!string.IsNullOrWhiteSpace(textBox.Text))
                        {
                            textBox.Foreground = Brushes.Black;
                        }
                    }
                }
            }
        }
       
        private void MaxCargoWeightTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                var vehicle = DataContext as Vehicle;
                if (vehicle != null)
                {
                    int maxCargoWeightKg;
                    if (int.TryParse(textBox.Text, out maxCargoWeightKg))
                    {
                        vehicle.MaxCargoWeightKg = maxCargoWeightKg;
                        textBox.Foreground = Brushes.Black;
                    }
                    else
                    {
                        textBox.Foreground = Brushes.Red;
                        MessageBox.Show("Максимальный вес груза должен быть числом!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = ComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                
                string selectedTransportType = selectedItem.Content.ToString();
                VehicleType transportType;
                switch (selectedTransportType)
                {
                    case "Car":
                        transportType = VehicleType.Car;
                        break;
                    case "Ship":
                        transportType = VehicleType.Ship;
                        break;
                    case "Plane":
                        transportType = VehicleType.Plane;
                        break;
                    case "Train":
                        transportType = VehicleType.Train;
                        break;
                    default:
                        throw new ArgumentException("Invalid transport type selected.");
                }

            }
        }

        private void Volume_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                var vehicle = DataContext as Vehicle;
                if (vehicle != null)
                {
                    double maxCargoVolume;
                    if (double.TryParse(textBox.Text, out maxCargoVolume))
                    {
                        vehicle.CurrentCargoVolume = maxCargoVolume;
                    }
                    else
                    {
                        MessageBox.Show("Максимальный объем груза должен быть числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        textBox.Text = "0";
                    }
                }
            }
        }
        private void Button_Click_Create(object sender, RoutedEventArgs e)
        {
            // Создаем новый экземпляр Vehicle.

            var vehicle = new Vehicle
            {
                Id = Vehicles.Count + 1 // или какое-то другое значение, которое должно быть у нового транспорта
            };

            // Заполняем свойства Vehicle значениями из соответствующих элементов управления.
            vehicle.Number = NumberTextBox.Text;
            vehicle.MaxCargoWeightKg = int.Parse(MaxCargoWeightTextBox.Text);
            vehicle.MaxCargoVolume = double.Parse(MaxCargoVolumeTextBox.Text);
            var selectedItem = ComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                var selectedValue = selectedItem.Content.ToString();
                switch (selectedValue)
                {
                    case "Car":
                        vehicle.Type = VehicleType.Car;
                        break;
                    case "Ship":
                        vehicle.Type = VehicleType.Ship;
                        break;
                    case "Plane":
                        vehicle.Type = VehicleType.Plane;
                        break;
                    case "Train":
                        vehicle.Type = VehicleType.Train;
                        break;
                    default:
                        throw new ArgumentException("Invalid transport type selected.");
                }
            }

            // Обновляем список грузов в Vehicle.
            vehicle.Cargos.Clear();
            foreach (var cargo in Cargos)
            {
                vehicle.Cargos.Add(cargo as Cargo);
            }

            // Добавляем новый экземпляр Vehicle в нашу коллекцию Vehicles и сохраняем его в репозитории.
            Vehicles.Add(vehicle);
            _vehicleService.Create(vehicle);
            //_memoryRepository.Update(vehicle);
            NumberTextBox.Text = "введите номер";
            MaxCargoWeightTextBox.Text = "0";
            MaxCargoVolumeTextBox.Text = "0";
            ComboBox.SelectedIndex = 0; // сброс выбранного значения в ComboBox на первый элемент
            Cargos.Clear();
        }

        private void Button_Click_Update(object sender, RoutedEventArgs e)
        {
            // получаем выбранный элемент Vehicle из списка vehicleListView
            Vehicle selectedVehicle = (Vehicle)vehicleListView.SelectedItem;
            
            // проверяем, что элемент выбран
            if (selectedVehicle == null)
            {
                MessageBox.Show("Please select a vehicle to update.");
                return;
            }
            // обновляем свойства выбранного элемента Vehicle
            selectedVehicle.Number = NumberTextBox.Text;
            selectedVehicle.MaxCargoWeightKg = int.Parse(MaxCargoWeightTextBox.Text);
            selectedVehicle.MaxCargoVolume = double.Parse(MaxCargoVolumeTextBox.Text);
            var selectedItem = ComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                var selectedValue = selectedItem.Content.ToString();
                switch (selectedValue)
                {
                    case "Car":
                        selectedVehicle.Type = VehicleType.Car;
                        break;
                    case "Ship":
                        selectedVehicle.Type = VehicleType.Ship;
                        break;
                    case "Plane":
                        selectedVehicle.Type = VehicleType.Plane;
                        break;
                    case "Train":
                        selectedVehicle.Type = VehicleType.Train;
                        break;
                    default:
                        throw new ArgumentException("Invalid transport type selected.");
                }
            }

            // обновляем список грузов в Vehicle
            selectedVehicle.Cargos.Clear();
            foreach (var cargo in Cargos)
            {
                selectedVehicle.Cargos.Add(cargo as Cargo);
            }

            // сохраняем изменения в репозитории
            _memoryRepository.Update(selectedVehicle);

            // сбрасываем значения элементов управления
            NumberTextBox.Text = "введите номер";
            MaxCargoWeightTextBox.Text = "0";
            MaxCargoVolumeTextBox.Text = "0";
            ComboBox.SelectedIndex = 0; // сброс выбранного значения в ComboBox на первый элемент
            Cargos.Clear();

            // обновляем список vehicleListView
            vehicleListView.Items.Refresh();
        }

        private void FillFormFields(Vehicle vehicle)
        {
            NumberTextBox.Text = vehicle.Number;
            MaxCargoWeightTextBox.Text = vehicle.MaxCargoWeightKg.ToString();
            MaxCargoVolumeTextBox.Text = vehicle.MaxCargoVolume.ToString();

            switch (vehicle.Type)
            {
                case VehicleType.Car:
                    ComboBox.SelectedItem = CarComboBoxItem;
                    break;
                case VehicleType.Ship:
                    ComboBox.SelectedItem = ShipComboBoxItem;
                    break;
                case VehicleType.Plane:
                    ComboBox.SelectedItem = PlaneComboBoxItem;
                    break;
                case VehicleType.Train:
                    ComboBox.SelectedItem = TrainComboBoxItem;
                    break;
                default:
                    throw new ArgumentException("Invalid transport type.");
            }
        }
       
        private void Button_Click_Delete(object sender, RoutedEventArgs e)
        {
            // Получаем выбранный элемент
            var selectedVehicle = vehicleListView.SelectedItem as Vehicle;

            // Проверяем, что элемент не null
            if (selectedVehicle != null)
            {
                // Удаляем выбранный элемент из репозитория
                _memoryRepository.DeleteById(selectedVehicle.Id);

                // Обновляем отображение в ListView
                Vehicles.Remove(selectedVehicle);
            }

            // Выбираем первый элемент в списке (если он есть)
            if (Vehicles.Count > 0)
            {
                vehicleListView.SelectedIndex = 0;
            }
            else
            {
                // Если список пустой, сбрасываем выделение
                vehicleListView.SelectedItem = null;
            }
        }
        
        private void LoadCargoButton_Click(object sender, RoutedEventArgs e)
        {
            CargoWindow cargoWindow = new CargoWindow();
            cargoWindow.Show();
        }

    }
}
