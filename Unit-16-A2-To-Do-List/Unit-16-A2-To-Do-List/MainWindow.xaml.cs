using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Unit_16_A2_To_Do_List
{
    // MainWindow class for the WPF application
    public partial class MainWindow : Window
    {
        // ObservableCollection to hold the list of Todo items
        public ObservableCollection<TodoListItem> TodoItems { get; set; }
        // ObservableCollection to hold the filtered/displayed list of Todo items
        public ObservableCollection<TodoListItem> DisplayedItems { get; set; }

        // Constructor for the MainWindow class
        public MainWindow()
        {
            InitializeComponent();
            // Initialize the TodoItems collection
            TodoItems = new ObservableCollection<TodoListItem>();
            // Initialize the DisplayedItems collection with the same items as TodoItems
            DisplayedItems = new ObservableCollection<TodoListItem>(TodoItems);
            // Bind the DisplayedItems collection to the masterList UI element
            masterList.ItemsSource = DisplayedItems;
        }

        // Event handler for the Add button click event
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (masterList.SelectedItem != null)
            {
                // Update the selected item's properties
                var selectedItem = (TodoListItem)masterList.SelectedItem;
                selectedItem.Description = descriptionField.Text;
                selectedItem.DueDate = dueDatePicker.SelectedDate ?? DateTime.Now;
                selectedItem.IsDone = isDoneCheckbox.IsChecked == true;
            }
            else
            {
                // Create a new item and add it to the TodoItems collection
                var newItem = new TodoListItem(titleField.Text)
                {
                    Description = descriptionField.Text,
                    DueDate = dueDatePicker.SelectedDate ?? DateTime.Now,
                    IsDone = isDoneCheckbox.IsChecked == true
                };
                TodoItems.Add(newItem);
            }
            // Update the displayed items and clear the detail fields
            UpdateDisplayedItems();
            ClearDetailFields();
        }

        // Event handler for the Edit button click event
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (masterList.SelectedItem != null)
            {
                // Load the selected item's properties into the detail fields
                var selectedItem = (TodoListItem)masterList.SelectedItem;
                titleField.Text = selectedItem.Title;
                descriptionField.Text = selectedItem.Description;
                dueDatePicker.SelectedDate = selectedItem.DueDate;
                isDoneCheckbox.IsChecked = selectedItem.IsDone;

                // Remove read-only property from description field
                descriptionField.IsReadOnly = false;
            }
        }

        // Event handler for the Delete button click event
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (masterList.SelectedItem != null)
            {
                // Remove the selected item from the TodoItems collection
                TodoItems.Remove((TodoListItem)masterList.SelectedItem);
                // Update the displayed items
                UpdateDisplayedItems();
            }
        }

        // Event handler for the selection change event of the master list
        private void MasterList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (masterList.SelectedItem != null)
            {
                // Load the selected item's properties into the detail fields
                var selectedItem = (TodoListItem)masterList.SelectedItem;
                titleField.Text = selectedItem.Title;
                descriptionField.Text = selectedItem.Description;
                dueDatePicker.SelectedDate = selectedItem.DueDate;
                isDoneCheckbox.IsChecked = selectedItem.IsDone;

                // Make description field read-only
                descriptionField.IsReadOnly = true;
            }
        }

        // Event handler for the Show All checkbox checked event
        private void ShowAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Update the displayed items based on the checkbox state
            UpdateDisplayedItems();
        }

        // Method to update the displayed items based on the Show All checkbox state
        private void UpdateDisplayedItems()
        {
            DisplayedItems.Clear();
            var itemsToShow = showAllCheckBox.IsChecked == true
                ? TodoItems // Show all items if the checkbox is checked
                : TodoItems.Where(item => !item.IsDone); // Show only unfinished items if the checkbox is unchecked
            foreach (var item in itemsToShow)
            {
                DisplayedItems.Add(item);
            }
        }

        // Method to clear the detail fields
        private void ClearDetailFields()
        {
            titleField.Clear();
            descriptionField.Clear();
            dueDatePicker.SelectedDate = null;
            isDoneCheckbox.IsChecked = false;

            descriptionField.IsReadOnly = false;
            masterList.SelectedItem = null;
        }
    }

    // TodoListItem class to represent a single Todo item, implementing INotifyPropertyChanged for data binding
    public class TodoListItem : INotifyPropertyChanged
    {
        private string description;
        private DateTime dueDate;
        private bool isDone;

        // Constructor with title parameter
        public TodoListItem(string title)
        {
            Title = title;
        }

        // Property for the title of the Todo item
        public string Title { get; }

        // Property for the description of the Todo item
        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged();
            }
        }

        // Property for the due date of the Todo item
        public DateTime DueDate
        {
            get => dueDate;
            set
            {
                dueDate = value;
                OnPropertyChanged();
            }
        }

        // Property for the completion status of the Todo item
        public bool IsDone
        {
            get => isDone;
            set
            {
                isDone = value;
                OnPropertyChanged();
            }
        }

        // Event to handle property changes for data binding
        public event PropertyChangedEventHandler PropertyChanged;

        // Method to trigger the PropertyChanged event
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}