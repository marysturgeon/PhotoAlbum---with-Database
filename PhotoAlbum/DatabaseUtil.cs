using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PhotoAlbum
{
    public static class DatabaseUtil
    {
        //Initial connection string in case the Photos database doesn't exist yet
        static string connectionStringInitial = @"Data Source =.\SQLEXPRESS; Integrated Security = True";
        //Regular connection string so that we can start in our Photos database as soon as it exists
        static string connectionString = @"Data Source =.\SQLEXPRESS; Initial Catalog = Photos; Integrated Security = True";
        static string databaseName = "Photos";
        /// <summary>
        /// This method checks whether the Photos database exists
        /// </summary>
        /// <returns>true if the Photos database exists, false if not</returns>
        public static bool CheckDatabaseExists()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStringInitial))
                {
                    using (SqlCommand command = new SqlCommand($"SELECT db_id('{databaseName}')", connection))
                    {
                        connection.Open();
                        return (command.ExecuteScalar() != DBNull.Value);
                    }
                }
            }
            catch (SqlException e)
            {
                MessageBox.Show("An exception occurred:" + e.Message);
                return false;
            }
        }
        /// <summary>
        /// This method will create the Photos database and the table too. 
        /// </summary>
        public static void CreatePhotoDatabase()
        {
            //create the photo database
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStringInitial))
                {
                    SqlCommand command = new SqlCommand($"CREATE DATABASE Photos", connection);
                    SqlCommand move = new SqlCommand("use Photos", connection);
                    SqlCommand command2 = new SqlCommand($"CREATE TABLE tblPhotoAlbum(ID INT IDENTITY PRIMARY KEY, Title VARCHAR(50), DateTaken DATETIME, DateMod DATETIME, Description VARCHAR(50), Photographer VARCHAR(50), Keywords VARCHAR(50), FileLocation VARCHAR(100))", connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    move.ExecuteNonQuery();
                    command2.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                MessageBox.Show("An exception occurred:" + e.Message);
            }
        }
        /// <summary>
        /// This tests whether there is data in the database
        /// </summary>
        /// <returns>true if there is existing data, false otherwise</returns>
        public static bool CheckDatabaseData()
        {
            //Use ExecuteScalar to check whether any records exist

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand($"SELECT * FROM tblPhotoAlbum", connection))
                    {
                        connection.Open();
                        return (command.ExecuteScalar() != null);
                    }
                }
            }
            catch (SqlException e)
            {
                MessageBox.Show("An exception occurred:" + e.Message);
                return false;
            }
        }
        /// <summary>
        /// If the database is empty, this method just creates some initial data to populate the database
        /// </summary>
        public static void PopulateEmptyDatabase()
        {
            //Add some initial records to the database
            Photograph photo = new Photograph("Cadence and Gawen Studio", Convert.ToDateTime("6/14/2005"), Convert.ToDateTime("2/28/2017"), "Studio photo of both kids together.", "Picture People", "studio, basket, cadence, gawen", @"C:\Users\Mary Sturgeon\Pictures\CadenceandGawen3.jpg");
            Photograph photo2 = new Photograph("Cadence and Gawen Easter", Convert.ToDateTime("4/30/2008"), Convert.ToDateTime("2/18/2017"), "Studio photo of both kids at Easter", "Picture People", "spring, easter, bunny, cadence, gawen", @"C:\Users\Mary Sturgeon\Pictures\Picture2.jpg");
            Photograph photo3 = new Photograph("Orange Crush", Convert.ToDateTime("3/08/2014"), Convert.ToDateTime("2/15/2017"), "Anaia Latrix", "Mary Sturgeon", "roller derby, cadence, skating", @"C:\Users\Mary Sturgeon\Pictures\20140308_175124.jpg");
            Photograph photo4 = new Photograph("Before Camp", Convert.ToDateTime("8/1/2015"), Convert.ToDateTime("2/16/2017"), "Kids together before summer camp", "Jason Sturgeon", "orkila, camp, cadence, gawen", @"C:\Users\Mary Sturgeon\Pictures\20150801_085752.jpg");
            Photograph photo5 = new Photograph("Fall Out Boy Concert", Convert.ToDateTime("8/2/2015"), Convert.ToDateTime("2/1/2017"), "Mom and Gawen at Fall Out Boy Concert", "Jason Sturgeon", "concert, mom, gawen", @"C:\Users\Mary Sturgeon\Pictures\20150802_185025.jpg");
            Photograph photo6 = new Photograph("Hawaii Zipline", Convert.ToDateTime("11/10/2017"), Convert.ToDateTime("2/4/2017"), "Kids on zipline in Hawaii", "Kauai Adventures", "hawaii, zipline, cadence, gawen", @"C:\Users\Mary Sturgeon\Pictures\ZiplineHawaii.jpg");

            AddRecord(photo);
            AddRecord(photo2);
            AddRecord(photo3);
            AddRecord(photo4);
            AddRecord(photo5);
            AddRecord(photo6);
        }
        /// <summary>
        /// This method Reads all records from the database into the list
        /// </summary>
        /// <param name="list">This is the list that should be populated with records from the database</param>
        public static void ReadDatabase(PhotoList list)
        {
            //Read all database records into photolist
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM tblPhotoAlbum", connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Photograph photo = new Photograph((string)reader["Title"], Convert.ToDateTime(reader["DateTaken"]), Convert.ToDateTime(reader["DateMod"]), (string)reader["Description"], (string)reader["Photographer"], (string)reader["Keywords"], (string)reader["FileLocation"]);
                        photo.Id = (int)reader["ID"];
                        list.Add(photo);
                    }
                }
                catch (SqlException e)
                {
                    MessageBox.Show("An exception occurred:" + e.Message);
                }
            }

        }
        /// <summary>
        /// This method adds a new record to the database
        /// </summary>
        /// <param name="photo">This is the photo to be added</param>
        public static void AddRecord(Photograph photo)
        {
            //Add photo to database
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand($"INSERT INTO tblPhotoAlbum VALUES (@Title, @DateTaken, @DateMod, @Desc, @Photog, @Keywords, @Location)", connection))
                    {
                        command.Parameters.AddWithValue("@Title", photo.Title);
                        command.Parameters.AddWithValue("@DateTaken", photo.DateTaken);
                        command.Parameters.AddWithValue("@DateMod", photo.DateMod);
                        command.Parameters.AddWithValue("@Desc", photo.Description);
                        command.Parameters.AddWithValue("@Photog", photo.Photog);
                        command.Parameters.AddWithValue("@Keywords", photo.Keywords);
                        command.Parameters.AddWithValue("@Location", photo.Location);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException e)
            {
                MessageBox.Show("An exception occurred:" + e.Message);
            }

        }
        /// <summary>
        /// This method edits a record in the database
        /// </summary>
        /// <param name="photo">This is the photo to be added</param>
        public static void EditRecord(Photograph photo)
        {
            //update photo being accessed
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand($"UPDATE tblPhotoAlbum SET Title = @Title, DateTaken = @DateTaken, DateMod = @DateMod, Description = @Desc, Photographer = @Photog, Keywords = @Keywords, FileLocation = @Location WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Title", photo.Title);
                        command.Parameters.AddWithValue("@DateTaken", photo.DateTaken);
                        command.Parameters.AddWithValue("@DateMod", photo.DateMod);
                        command.Parameters.AddWithValue("@Desc", photo.Description);
                        command.Parameters.AddWithValue("@Photog", photo.Photog);
                        command.Parameters.AddWithValue("@Keywords", photo.Keywords);
                        command.Parameters.AddWithValue("@Location", photo.Location);
                        command.Parameters.AddWithValue("@Id", photo.Id);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException e)
                {
                    MessageBox.Show("An exception occurred:" + e.Message);
                }
            }
        }

        public static void DeleteRecord(Photograph photo, PhotoList list)
        {
            list.Remove(photo);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand($"DELETE FROM tblPhotoAlbum WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", photo.Id);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException e)
                {
                    MessageBox.Show("An exception occurred:" + e.Message);
                }
            }
        }
    }

}
