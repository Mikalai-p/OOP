using System.Windows;
using AutoServiceShop.ViewModels;

namespace AutoServiceShop.Views
{
    public partial class AddEditProductWindow : Window
    {
        public AddEditProductWindow(AddEditProductViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.SaveRequested += (s, product) =>
            {
                DialogResult = true;
                Close();
            };
            viewModel.CancelRequested += (s, e) =>
            {
                DialogResult = false;
                Close();
            };
        }

        private void ListBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (DataContext is AddEditProductViewModel viewModel)
                {
                    foreach (string file in files)
                    {
                        string ext = System.IO.Path.GetExtension(file).ToLower();
                        if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp")
                        {
                            viewModel.AddImagePath(file);
                        }
                    }
                }
            }
        }
    }
}