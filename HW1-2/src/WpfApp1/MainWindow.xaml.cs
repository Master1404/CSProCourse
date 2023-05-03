using Logistic.Core;
using Logistic.DAL;
using Logistic.Model;
using Logistic.Models;
using Logistic.Models.Enum;
using Microsoft.Win32;
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
using System.IO;
using Logistic.ConsoleClient.Enum;

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
        private string exportPath;
        private string importPathTextBox;
        private ReportType reportType;
        private ReportService<Vehicle> _reportService;
        private readonly IReportRepository<Vehicle> _jsonRepository;
        private readonly IReportRepository<Vehicle> _xmlRepository;

        public MainWindow()
        {
            InitializeComponent();
            var vehicle = new Vehicle();
            this.DataContext = vehicle;
            _memoryRepository = new InMemoryRepository<Vehicle>(vehicle => vehicle.Id);
            vehicleListView.ItemsSource = Vehicles;
            _vehicleService = new VehicleService(_memoryRepository);
            var vehicles = new ObservableCollection<Vehicle>();
            var cargos = new ObservableCollection<Cargo>();
            vehicleListView.ItemsSource = vehicles;
            _jsonRepository = new JsonRepository<Vehicle>(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reports"));
            _xmlRepository = new XmlRepository<Vehicle>(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reports"));
            _reportService = new ReportService<Vehicle>(_jsonRepository, _xmlRepository,
                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reports", "report.json"),
                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reports", "report.xml"));
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var selectedVehicle = (Vehicle)e.AddedItems[0];

                FillFormFields(selectedVehicle);

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
                        MessageBox.Show("Номер должен содержать не более 6 символов и не быть пустым!", "Warning",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
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
                        MessageBox.Show("Максимальный вес груза должен быть числом!", "Warning",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
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
                        MessageBox.Show("Максимальный объем груза должен быть числом.", "Ошибка",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        textBox.Text = "0";
                    }
                }
            }
        }
        private void Button_Click_Create(object sender, RoutedEventArgs e)
        {
            var vehicle = new Vehicle
            {
                Id = Vehicles.Count + 1 
            };

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

            Vehicles.Add(vehicle);
            _vehicleService.Create(vehicle);
            vehicleListView.ItemsSource = Vehicles;
            NumberTextBox.Text = "введите номер";
            MaxCargoWeightTextBox.Text = "0";
            MaxCargoVolumeTextBox.Text = "0";
            ComboBox.SelectedIndex = 0;
            Cargos.Clear();
        }

        private void Button_Click_Update(object sender, RoutedEventArgs e)
        {
            Vehicle selectedVehicle = (Vehicle)vehicleListView.SelectedItem;

            if (selectedVehicle == null)
            {
                MessageBox.Show("Please select a vehicle to update.");
                return;
            }

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
            _vehicleService.Update(selectedVehicle);
            NumberTextBox.Text = "введите номер";
            MaxCargoWeightTextBox.Text = "0";
            MaxCargoVolumeTextBox.Text = "0";
            ComboBox.SelectedIndex = 0;
            Cargos.Clear();
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
            var selectedVehicle = vehicleListView.SelectedItem as Vehicle;

            if (selectedVehicle != null)
            {
                _vehicleService.Delete(selectedVehicle.Id);
                Vehicles.Remove(selectedVehicle);
            }

            if (Vehicles.Count > 0)
            {
                vehicleListView.SelectedIndex = 0;
            }
            else
            {
                vehicleListView.SelectedItem = null;
            }
        }
        private void LoadCargoButton_Click(object sender, RoutedEventArgs e)
        {
            var cargos = new ObservableCollection<Cargo>(Cargos);
            var selectedVehicle = vehicleListView.SelectedItem as Vehicle;
            if (selectedVehicle == null)
            {
                MessageBox.Show("Выберите транспорт для загрузки грузов.", "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            var cargoWindow = new CargoWindow(selectedVehicle, cargos, cargo =>
            {
                _vehicleService.LoadCargo(cargo, selectedVehicle.Id);
                selectedVehicle.Cargos.Add(cargo);
                _vehicleService.Update(selectedVehicle);

            }, cargo =>
            {
                _vehicleService.UnloadCargo(selectedVehicle.Id, cargo.Id);
                selectedVehicle.Cargos.Remove(cargo);
                _vehicleService.Update(selectedVehicle);
            });
            cargoWindow.ShowDialog();

        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl tabControl && tabControl.SelectedItem is TabItem tabItem && tabItem.Header.Equals("Report"))
            {
                var vehicles = _vehicleService.GetAll();
                ListBoxExport.ItemsSource = vehicles;
            }
            else if (e.Source is TabControl tabControl2 && tabControl2.SelectedItem is TabItem tabItem2 && tabItem2.Header.Equals("Vehicles"))
            {
                vehicleListView.ItemsSource = Vehicles;
            }
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxReport.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content is string selectedValue)
            {
                if (selectedValue == "JSON")
                {
                    reportType = ReportType.Json;
                }
                else if (selectedValue == "XML")
                {
                    reportType = ReportType.Xml;
                }
            }
        }

        private void Button_Click_Export(object sender, RoutedEventArgs e)
        {
            if (ComboBoxReport.SelectedItem == null)
            {
                MessageBox.Show("Please select a report type.");
                return;
            }

            ReportType reportType = (ComboBoxReport.SelectedItem as ComboBoxItem)?.Content.ToString() switch
            {
                "XML" => ReportType.Xml,
                "JSON" => ReportType.Json,
                _ => throw new InvalidOperationException("Invalid report type"),
            };

            var allVehicles = _vehicleService.GetAll();
            SaveFileDialog saveDialog = new SaveFileDialog();

            switch (reportType)
            {
                case ReportType.Json:
                    saveDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                    saveDialog.DefaultExt = ".json";
                    break;
                case ReportType.Xml:
                    saveDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                    saveDialog.DefaultExt = ".xml";
                    break;
                default:
                    MessageBox.Show("Please select an export format");
                    return;
            }

            bool? result = saveDialog.ShowDialog();

            if (result == true)
            {
                exportPath = saveDialog.FileName;
                TextBoxExportPath.Text = exportPath; 
                _reportService.CreateReport("vehicles", reportType, allVehicles);
                MessageBox.Show("Report saved successfully.");
            }
        }

        private void TextBox_TextChanged_Export(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text != exportPath)
            {
                exportPath = textBox.Text;
            }
        }

        private void Button_Click_Import(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "JSON and XML files (*.json, *.xml)|*.json;*.xml|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;

                    List<Vehicle> vehicles = _reportService.LoadReport(filePath, filePath.EndsWith(".json") ? ReportType.Json : ReportType.Xml);

                    if (vehicles != null && vehicles.Count > 0)
                    {
                        ListBoxImport.ItemsSource = vehicles;
                        importPathTextBox = filePath;

                        if (TextBoxImportPath != null)
                        {
                            TextBoxImportPath.Text = importPathTextBox;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No valid vehicles data found in the selected file.", "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading the report: {ex.Message}", "Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        private void TextBox_TextChanged_ImportPath(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                string text = textBox.Text;
                if (text != importPathTextBox) 
                {
                    importPathTextBox = text;
                }
            }
        }
    }
}
