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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PhotoAlbum
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PhotoList plist = new PhotoList();
        CollectionViewSource viewsource = new CollectionViewSource();
        string searchterm;
        string[] keywords;

        public MainWindow()
        {
            
            
            //Check whether a database exists and create one if necessary
            if (!DatabaseUtil.CheckDatabaseExists())
            {
                DatabaseUtil.CreatePhotoDatabase();
            }
            

            //Check whether there is data in the database and add the initial data if necessary
            if (!DatabaseUtil.CheckDatabaseData())
            {
                DatabaseUtil.PopulateEmptyDatabase();
            }
            
            
            //Populate the list with data from the database.
            DatabaseUtil.ReadDatabase(plist);

            InitializeComponent();

            //Set the viewsource to the list of photos created
            viewsource.Source = plist;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(viewsource);

            //Set up the datagrid
            dgPhotoInfo.ItemsSource = viewsource.View;

            DataGridTextColumn title = new DataGridTextColumn();
            title.Header = "Title";
            title.Binding = new Binding("Title");
            dgPhotoInfo.Columns.Add(title);

            DataGridTextColumn datetaken = new DataGridTextColumn();
            datetaken.Header = "Date Taken";
            datetaken.Binding = new Binding("DateTaken");
            datetaken.Binding.StringFormat = "{0:d}";
            dgPhotoInfo.Columns.Add(datetaken);

            DataGridTextColumn description = new DataGridTextColumn();
            description.Header = "Description";
            description.Binding = new Binding("Description");
            dgPhotoInfo.Columns.Add(description);

            //Set up Image box
            image.SetBinding(Image.SourceProperty, new Binding("Location"));
            //image.Source = new BitmapImage(new Uri(location, UriKind.Absolute));
            image.Stretch = Stretch.Uniform;

            //Set up Bound text boxes for navigation
            DataContext = viewsource.View;
            tbtitle.SetBinding(TextBox.TextProperty, new Binding("Title"));
            Binding binddate = new Binding("DateTaken");
            binddate.StringFormat = "{0:d}";
            tbdatetaken.SetBinding(TextBox.TextProperty, binddate);
            Binding binddatemod = new Binding("DateMod");
            binddatemod.StringFormat = "{0:d}";
            tbdatemod.SetBinding(TextBox.TextProperty, binddatemod);
            tbdescription.SetBinding(TextBox.TextProperty, new Binding("Description"));
            tbphotog.SetBinding(TextBox.TextProperty, new Binding("Photog"));
            tbkeywords.SetBinding(TextBox.TextProperty, new Binding("Keywords"));
            tblocation.SetBinding(TextBox.TextProperty, new Binding("Location"));
        }


        private void btnAddnew_Click(object sender, RoutedEventArgs e)
        {
            //Create a photo object to pass in to the addnew form - the arguments will be supplied within that form
            Photograph newphoto = new Photograph(null,DateTime.MinValue,DateTime.MinValue,null,null,null,null);
            //Create an editing form, passing in a new photo and a bool that tells the form whether we're editing or adding a record.
            addnew frm = new addnew(newphoto, false);
            if (frm.ShowDialog() == true)
            {
                //Add newphoto to plist
                plist.Add(newphoto);
                DatabaseUtil.AddRecord(newphoto);
                CollectionViewSource.GetDefaultView(dgPhotoInfo.ItemsSource).Refresh();
            }
        }


        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            //Reuses the 'addnew' form to edit an existing record
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(dgPhotoInfo.ItemsSource);
            //This accesses the currently selected photo object
            Photograph currentphoto = view.CurrentItem as Photograph;
            //then creates the form, passing in the currentphoto object for editing, along with the boolean true for 'isedit'
            addnew frm = new addnew(currentphoto, true);
            if (frm.ShowDialog() == true)
            {
                //Refresh the view if we've changed a record
                CollectionViewSource.GetDefaultView(dgPhotoInfo.ItemsSource).Refresh();
                DatabaseUtil.EditRecord(currentphoto);
            }
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //This is the active search code
            searchterm = tbSearch.Text;
            //split the search terms into the keywords array by commas
            keywords = searchterm.Split(',');
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(dgPhotoInfo.ItemsSource);
            //Then apply the SearchFilter
            view.Filter = SearchFilter;
        }

        /// <summary>
        /// This filter can be applied to a Collection View in order to limit the view to items that match the search string
        /// </summary>
        /// <param name="photo"> this is the photo being tested for filtering</param>
        /// <returns>the filter returns true or false as to whether the entry should appear in the filtered list</returns>
        private bool SearchFilter(object photo)
        {
            Photograph photograph = photo as Photograph;
            //This iterates through each keyword in the array of strings entered by the user
            foreach (string term in keywords)
            {
                if (photograph.Title.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
                else if (photograph.Photog.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
                else if (photograph.Description.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
                else if (photograph.Keywords.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void btnRemFilters_Click(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(dgPhotoInfo.ItemsSource);
            view.Filter = null;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(dgPhotoInfo.ItemsSource);
            //This accesses the currently selected photo object
            Photograph currentphoto = view.CurrentItem as Photograph;
            //then creates the form, passing in the currentphoto object for editing, along with the boolean true for 'isedit'
            //addnew frm = new addnew(currentphoto, true);
            MessageBoxResult confirmdelete = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmdelete == MessageBoxResult.Yes)
            {
                CollectionViewSource.GetDefaultView(dgPhotoInfo.ItemsSource).Refresh();
                DatabaseUtil.DeleteRecord(currentphoto, plist);
            } 
        }
    }
}
