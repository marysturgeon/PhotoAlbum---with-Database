using Microsoft.Win32;
using System;
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
using System.Windows.Shapes;

namespace PhotoAlbum
{
    /// <summary>
    /// Interaction logic for addnew.xaml
    /// </summary>
    public partial class addnew : Window
    {
        Photograph newphoto;
        public addnew(Photograph photo, bool isedit)
        {
            InitializeComponent();

            //Set up the window based on whether we are adding or editing
            if (isedit)
            {
                //Sets up the record for editing if this is an edit
                this.Title = "Edit Image";
                tbTitle.Text = photo.Title;
                tbPhotog.Text = photo.Photog;
                tbLocation.Text = photo.Location;
                tbKeywords.Text = photo.Keywords;
                tbDesc.Text = photo.Description;
                dtTaken.SelectedDate = photo.DateTaken;
            }
            else
            {
                //sets Title if we're adding a new record
                this.Title = "Add New Image";
                dtTaken.SelectedDate = DateTime.Today;
            }

            
            newphoto = photo;
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files (*.jpg)|*.jpg|All files (*.*)|*.*";
            if (ofd.ShowDialog() == true)
            {
                tbLocation.Text = ofd.FileName;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateForm())
            {
                this.DialogResult = true;
                newphoto.Title = tbTitle.Text;
                newphoto.DateTaken = dtTaken.SelectedDate.Value;
                newphoto.DateMod = DateTime.Now;
                newphoto.Description = tbDesc.Text;
                newphoto.Photog = tbPhotog.Text;
                newphoto.Keywords = tbKeywords.Text;
                newphoto.Location = tbLocation.Text;
                this.Close();
            }
            else
            {
                MessageBox.Show("Validation Error.");
            }

        }

        /// <summary>
        /// ValidateForm is a method that allows setting of Validation logic before the results are saved.
        /// </summary>
        /// <returns>true if all logic passes, false otherwise</returns>
        private bool ValidateForm()
        {
            //Any validation logic can be set here
            if (tbLocation.Text == "")
            {
                MessageBox.Show("Please enter a location for a photo.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;

        }
    }
}
