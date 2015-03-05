using System;
using System.Data.Objects;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Builders;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;




namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        MongoClient mongoClient = new MongoClient();
        MongoServer server;
        MongoDatabase database;
        MongoCollection<facultyData> collection;
        ObservableCollection<facultyData> resultBinding;

       

        public MainWindow()
        {
            InitializeComponent();

            

            var dateTime = DateTime.Now;
            dateTime_label.Content = dateTime;

        }

        
        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
               server = mongoClient.GetServer();
              database = server.GetDatabase("facultyDataAndSchedule");
               collection = database.GetCollection<facultyData>("faculty");
                var query = collection.FindAllAs<facultyData>().SetFields(Fields.Include("facultyID", "term","acadYear","age","program","lastName","firstName","middleName","dateOfBirth","rank","yearsOfTeachingS","yearsOfTeachingO","status","services"));
                ObservableCollection<facultyData> resultBinding = new ObservableCollection<facultyData>(query);
                facultyDataGrid.ItemsSource = resultBinding;


                if (resultBinding.Count() > 0)
                {
                    Binding bind = new Binding(); //create a new binding to be used on the wpf 
                    facultyDataGrid.DataContext = resultBinding; //sets the data binding for the control
                    facultyDataGrid.SetBinding(DataGrid.ItemsSourceProperty, bind); //syncs the data
                    facultyID_Textbox.DataContext = resultBinding;
                    facultyID_Textbox.SetBinding(DataGrid.ItemsSourceProperty, bind);
                    lastName_TextBox.DataContext = resultBinding;
                    lastName_TextBox.SetBinding(DataGrid.ItemsSourceProperty, bind);
                    firstName_TextBox.DataContext = resultBinding;
                    firstName_TextBox.SetBinding(DataGrid.ItemsSourceProperty, bind);
                    middleName_TextBox.DataContext = resultBinding;
                    middleName_TextBox.SetBinding(DataGrid.ItemsSourceProperty, bind);
                    dateOfBirth_TextBox.DataContext = resultBinding;
                    dateOfBirth_TextBox.SetBinding(DataGrid.ItemsSourceProperty, bind);
                    age_TextBox.DataContext = resultBinding;
                    age_TextBox.SetBinding(DataGrid.ItemsSourceProperty, bind);
                    rankTextBox.DataContext = resultBinding;
                    rankTextBox.SetBinding(DataGrid.ItemsSourceProperty, bind);
                    expSACTextBox.DataContext = resultBinding;
                    expSACTextBox.SetBinding(DataGrid.ItemsSourceProperty, bind);
                    expOTextBox.DataContext = resultBinding;
                    expOTextBox.SetBinding(DataGrid.ItemsSourceProperty, bind);


                }
                
                
      
        }



        private void viewMenu_click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Hello", "hiThere");
        }

        private void fRViewMenu_click(object sender, RoutedEventArgs e)
        {
            documentViewer docViewer = new documentViewer();
            docViewer.ShowDialog();
        }
      
        public void selectionChanged(object sender, SelectionChangedEventArgs e)
        {
            acadTab.IsEnabled = true;
            certTab.IsEnabled = true;
            schedTab.IsEnabled = true;

            try
            {

                facultyData row = (facultyData)facultyDataGrid.SelectedItem;
                facultyID_Textbox.Text = row.facultyID;
                lastName_TextBox.Text = row.lastName;
                firstName_TextBox.Text = row.firstName;
                middleName_TextBox.Text = row.middleName;
                age_TextBox.Text = row.age.ToString();
                termComboBox.SelectedItem = row.term;
                programComboBox.SelectedItem = row.program;
                rankTextBox.Text = row.rank;
                expSACTextBox.Text = row.yearsOfTeachingS.ToString();
                expOTextBox.Text = row.yearsOfTeachingO.ToString();
                aYComboBox.SelectedItem = row.acadYear;


            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine("There is a null data from the database,      " + ex.Message);
            }
            
           
        }
        public void termComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> data = new List<string>();
            data.Add("1st");
            data.Add("2nd");
            termComboBox.ItemsSource = data;
         
        }

        public void addData_Click(object sender, RoutedEventArgs e)
        {
            addSaveButton.Visibility = Visibility.Visible;
            addButton.Visibility = Visibility.Hidden;
            clearTextBoxes();
    
            
        }
        void clearTextBoxes()
        {
            facultyID_Textbox.Text = null;
            age_TextBox.Text = null;
            firstName_TextBox.Text = null;
            lastName_TextBox.Text = null;
            middleName_TextBox.Text = null;
            dateOfBirth_TextBox.Text = null;
            expOTextBox.Text = null;
            expSACTextBox.Text = null;
            fullTimeButton.IsChecked = false;
            permanentButton.IsChecked = false;
            rankTextBox.Text = null;
            termComboBox.SelectedIndex = -1;
            aYComboBox.SelectedIndex = -1;
            programComboBox.SelectedIndex = -1;


            acadTab.IsEnabled = false;
            certTab.IsEnabled = false;
            schedTab.IsEnabled = false;
       
            
        }

        public void addSaveData_Click(object sender, RoutedEventArgs e)
        {
    
            server = mongoClient.GetServer();
            database = server.GetDatabase("facultyDataAndSchedule");
            collection = database.GetCollection<facultyData>("faculty");
            var query = collection.FindAllAs<facultyData>().SetFields(Fields.Include("facultyID", "term", "acadYear", "age", "program", "lastName", "firstName", "middleName", "dateOfBirth", "rank", "yearsOfTeachingS", "yearsOfTeachingO", "status", "services"));
            resultBinding = new ObservableCollection<facultyData>(query);
            facultyDataGrid.ItemsSource = resultBinding;

            string services = "";
            if (fullTimeButton.IsChecked == true)
                services = "Full-time";
            else
                services = "Part-time";
            string status = "";
            if (permanentButton.IsChecked == true)
                status = "Permanent";
            else
                status = "Permanent";

   
            try
            {
                try
                {

                    var bsonDocument = new BsonDocument
                        {
                            { "_id", ObjectId.GenerateNewId() },
                            { "term", termComboBox.SelectedItem.ToString()},
                            { "facultyID", facultyID_Textbox.Text},
                            { "acadYear", aYComboBox.SelectedItem.ToString()},
                            { "program", programComboBox.SelectedItem.ToString()},
                            { "lastName", lastName_TextBox.Text},
                            { "firstName", firstName_TextBox.Text},
                            { "middleName", middleName_TextBox.Text},
                            { "dateOfBirth", dateOfBirth_TextBox.Text},
                            { "age", int.Parse(age_TextBox.Text)},
                            {"rank", rankTextBox.Text},
                            {"yearsOfTeachingS", int.Parse(expSACTextBox.Text)},
                            {"yearsOfTeachingO",  int.Parse(expOTextBox.Text)},
                            {"status",status},
                            {"services",services}};
                    collection.Insert(bsonDocument);
                }
                catch (FormatException ex)
                {
                    Console.WriteLine(ex);
                }
            }
            catch (MongoConnectionException ex)
            {
                Console.WriteLine(ex);
            }
            var queryupdated = collection.FindAllAs<facultyData>().SetFields(Fields.Include("facultyID", "term", "acadYear", "age", "program", "lastName", "firstName", "middleName", "dateOfBirth", "rank", "yearsOfTeachingS", "yearsOfTeachingO", "status", "services"));
            resultBinding = new ObservableCollection<facultyData>(query);
            facultyDataGrid.ItemsSource = resultBinding;
            
            addSaveButton.Visibility = Visibility.Hidden;
            addButton.Visibility = Visibility.Visible;
        }

        private void certficateDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
           
            facultyData row = (facultyData)facultyDataGrid.SelectedItem;
            var userID = row._id;
            MongoClient mongoClient = new MongoClient();
            MongoServer server = mongoClient.GetServer();
            MongoDatabase database = server.GetDatabase("facultyDataAndSchedule");
            MongoCollection<facultyData> collection = database.GetCollection<facultyData>("faculty");
            var result = collection.Aggregate(
                new BsonDocument { { "$match", new BsonDocument { { "_id",userID } } } },
                new BsonDocument { { "$unwind", "$validCertificateOrLicense" } },
                new BsonDocument {{"$project", new BsonDocument { {"title", "$validCertificateOrLicense.title"},{"number", "$validCertificateOrLicense.number"}, {"dateIssued","$validCertificateOrLicense.dateIssued"},{"validUntil","$validCertificateOrLicense.validUntil"},{"issuingAgency","$validCertificateOrLicense.issuingAgency"}}}}
            );
            List<certification> d = BsonSerializer.Deserialize<List<certification>>(result.ResultDocuments.ToJson());
            BindingList<certification> certBinding = new BindingList<certification>(d);
            certficateDataGrid.ItemsSource = certBinding;

            if (certBinding.Count() > 0)
            {
                Binding bind = new Binding(); //create a new binding to be used on the wpf 
                certficateDataGrid.DataContext = certBinding;
                certficateDataGrid.SetBinding(DataGrid.ItemsSourceProperty, bind);
            }
           
        }



        private void schedDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            facultyData row = (facultyData)facultyDataGrid.SelectedItem;
            var userID = row._id;
            MongoClient mongoClient = new MongoClient();
            MongoServer server = mongoClient.GetServer();
            MongoDatabase database = server.GetDatabase("facultyDataAndSchedule");
            MongoCollection<facultyData> collection = database.GetCollection<facultyData>("faculty");
            var result = collection.Aggregate(
                new BsonDocument { { "$match", new BsonDocument { { "_id",userID } } } },
                new BsonDocument { { "$unwind", "$scheduleOfClasses" } },
                new BsonDocument { { "$project", new BsonDocument { { "time", "$scheduleOfClasses.time" }, { "courseNumber", "$scheduleOfClasses.courseNumber" }, { "courseName", "$scheduleOfClasses.courseName" }, { "courseYear", "$scheduleOfClasses.courseYear" }, { "creditUnits", "$scheduleOfClasses.creditUnits" }, { "numTeachingHours", "$scheduleOfClasses.numTeachingHours" }, { "room", "$scheduleOfClasses.room" }, { "numStudents", "$scheduleOfClasses.numStudents" } } } }
            );
            MongoCollection<schedule> c = database.GetCollection<schedule>("certification");
            List<schedule> bsondoc = BsonSerializer.Deserialize<List<schedule>>(result.ResultDocuments.ToJson());
            BindingList<schedule> schedBinding = new BindingList<schedule>(bsondoc);
       
            if (schedBinding.Count() > 0)
            {
                Binding bind = new Binding(); //create a new binding to be used on the wpf 
                schedDataGrid.DataContext = schedBinding;
                schedDataGrid.SetBinding(DataGrid.ItemsSourceProperty, bind);
                schedTime_Texbox.DataContext = schedBinding;
                schedTime_Texbox.SetBinding(DataGrid.ItemsSourceProperty, bind);
                courseName_Texbox.DataContext = schedBinding;
                courseName_Texbox.SetBinding(DataGrid.ItemsSourceProperty, bind);
                courseNumber_Texbox.DataContext = schedBinding;
                courseNumber_Texbox.SetBinding(DataGrid.ItemsSourceProperty, bind);
                courseYear_Texbox.DataContext = schedBinding;
                courseYear_Texbox.SetBinding(DataGrid.ItemsSourceProperty, bind);
                creditUnits_Texbox.DataContext = schedBinding;
                creditUnits_Texbox.SetBinding(DataGrid.ItemsSourceProperty, bind);
                numTeachingHours_Texbox.DataContext = schedBinding;
                numTeachingHours_Texbox.SetBinding(DataGrid.ItemsSourceProperty, bind);
                room_Textbox.DataContext = schedBinding;
                room_Textbox.SetBinding(DataGrid.ItemsSourceProperty, bind);
                numStudents_Textbox.DataContext = schedBinding;
                numStudents_Textbox.SetBinding(DataGrid.ItemsSourceProperty, bind);

            }

            Console.WriteLine(userID.ToString());
        }

        private void acadDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            facultyData row = (facultyData)facultyDataGrid.SelectedItem;
            var userID = row._id;
            MongoServer server = mongoClient.GetServer();
            database = server.GetDatabase("facultyDataAndSchedule");
           collection = database.GetCollection<facultyData>("faculty");
            var result = collection.Aggregate(
                new BsonDocument { { "$match", new BsonDocument { { "_id", userID } } } },
                new BsonDocument { { "$unwind", "$academicQualifications" } },
                new BsonDocument { { "$project", new BsonDocument { { "degreeCompleted", "$academicQualifications.degreeCompleted" }, { "degree", "$academicQualifications.degree" }, { "when", "$academicQualifications.when" }, { "where", "$academicQualifications.where" }, { "remarks", "$academicQualifications.remarks" }} } }
            );
            MongoCollection<acadQualifications> c = database.GetCollection<acadQualifications>("certification");
            List<acadQualifications> bsondoc = BsonSerializer.Deserialize<List<acadQualifications>>(result.ResultDocuments.ToJson());
            BindingList<acadQualifications> schedBinding = new BindingList<acadQualifications>(bsondoc);
            schedBinding.RaiseListChangedEvents = true;

            if (schedBinding.Count() > 0)
            {
                Binding bind = new Binding(); //create a new binding to be used on the wpf 
                acadDataGrid.DataContext = schedBinding;
                acadDataGrid.SetBinding(DataGrid.ItemsSourceProperty, bind);
                degreeTextbox.DataContext = schedBinding;
                degreeTextbox.SetBinding(DataGrid.ItemsSourceProperty, bind);
                whenTextbox.DataContext = schedBinding;
                whenTextbox.SetBinding(DataGrid.ItemsSourceProperty, bind);
                institutionTextbox.DataContext = schedBinding;
                institutionTextbox.SetBinding(DataGrid.ItemsSourceProperty, bind);


            }
        }

        private void programComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> data = new List<string>();
            data.Add("Engineering");
            data.Add("Nursing");
            programComboBox.ItemsSource = data;
     
          
        }

        private void aYComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> data = new List<string>();
            data.Add("2014-2015");
            data.Add("2015-2016");
            aYComboBox.ItemsSource = data;
        }

        private void degreeCompletedComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> data = new List<string>();
            data.Add("2014-2015");
            data.Add("2015-2016");
            aYComboBox.ItemsSource = data;
        }
        private void schedInsertButton_Click(object sender, RoutedEventArgs e)
        {
            facultyData row = (facultyData)facultyDataGrid.SelectedItem;
            var userID = row._id;
            MongoClient mongoClient = new MongoClient();
            MongoServer server = mongoClient.GetServer();
            MongoDatabase database = server.GetDatabase("facultyDataAndSchedule");
            MongoCollection<facultyData> collection = database.GetCollection<facultyData>("faculty");




            collection.Update(Query.EQ("_id", userID), Update.Push("scheduleOfClasses", new BsonDocument {  {"time",schedTime_Texbox.Text},
                {"courseNumber", courseName_Texbox.Text},
                {"courseName",courseName_Texbox.Text},
                {"courseYear",courseYear_Texbox.Text},
                {"room",room_Textbox.Text},
                {"numTeachingHours",numTeachingHours_Texbox.Text},
                {"creditUnits", creditUnits_Texbox.Text},
                {"numStudents", numStudents_Textbox.Text}}));

        }

        private void schedRemoveButton_Click(object sender, RoutedEventArgs e)
        {

            facultyData row = (facultyData)facultyDataGrid.SelectedItem;
            var userID = row._id;


            MongoClient mongoClient = new MongoClient();
            MongoServer server = mongoClient.GetServer();
            MongoDatabase database = server.GetDatabase("facultyDataAndSchedule");
            MongoCollection<facultyData> collection = database.GetCollection<facultyData>("faculty");

            schedule schedRow = (schedule)schedDataGrid.SelectedItem;




            collection.Update(Query.EQ("_id", userID), Update.Pull("scheduleOfClasses", new BsonDocument {  {"time", schedRow.time},
                {"courseNumber",schedRow.courseNumber},
                {"courseName", schedRow.courseName},
                {"validUntil",schedRow.creditUnits},
                {"courseYear",schedRow.courseYear}}));
        }
        private void certRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            facultyData row = (facultyData)facultyDataGrid.SelectedItem;
            var userID = row._id;
            

            MongoClient mongoClient = new MongoClient();
            MongoServer server = mongoClient.GetServer();
            MongoDatabase database = server.GetDatabase("facultyDataAndSchedule");
            MongoCollection<facultyData> collection = database.GetCollection<facultyData>("faculty");

            certification certRow = (certification)certficateDataGrid.SelectedItem;
           



            collection.Update(Query.EQ("_id", userID), Update.Pull("validCertificateOrLicense", new BsonDocument {  {"title",certRow.title},
                {"number",certRow.number},
                {"dateIssued",certRow.dateIssued},
                {"validUntil",certRow.validUntil},
                {"issuingAgency",certRow.issuingAgency}}));
        }
        private void certInsertButton_Click(object sender, RoutedEventArgs e)
        {
           
            facultyData row = (facultyData)facultyDataGrid.SelectedItem;
            var userID = row._id;
            MongoClient mongoClient = new MongoClient();
            MongoServer server = mongoClient.GetServer();
            MongoDatabase database = server.GetDatabase("facultyDataAndSchedule");
            MongoCollection<facultyData> collection = database.GetCollection<facultyData>("faculty");

    


            collection.Update(Query.EQ("_id", userID), Update.Push("validCertificateOrLicense", new BsonDocument {  {"title",title_Textbox.Text},
                {"number",number_Textbox.Text},
                {"dateIssued",dateIssued_Textbox.Text},
                {"validUntil",validUntil_Textbox.Text},
                {"issuingAgency",issuingAgency_Textbox.Text}}));
        }

             
        
        private void acadInsertButton_Click(object sender, RoutedEventArgs e)
        {
            facultyData row = (facultyData)facultyDataGrid.SelectedItem;
            var userID = row._id;
            MongoClient mongoClient = new MongoClient();
            MongoServer server = mongoClient.GetServer();
            MongoDatabase database = server.GetDatabase("facultyDataAndSchedule");
            MongoCollection<facultyData> collection = database.GetCollection<facultyData>("faculty");




            collection.Update(Query.EQ("_id", userID), Update.Push("academicQualifications", new BsonDocument {  {"degreeCompleted",degreeCompletedComboBox.SelectedItem.ToString()},
                {"degree",degreeTextbox.Text},
                {"when",whenTextbox.Text},
                {"where",whenTextbox.Text},
                {"remarks",remarksTextbox.Text}}));
        }
        private void acadRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            facultyData row = (facultyData)facultyDataGrid.SelectedItem;
            var userID = row._id;


            MongoClient mongoClient = new MongoClient();
            MongoServer server = mongoClient.GetServer();
            MongoDatabase database = server.GetDatabase("facultyDataAndSchedule");
            MongoCollection<facultyData> collection = database.GetCollection<facultyData>("faculty");

            acadQualifications acadRow = (acadQualifications)certficateDataGrid.SelectedItem;




            collection.Update(Query.EQ("_id", userID), Update.Pull("academicQualifications", new BsonDocument {  {"degreeCompleted",acadRow.degreeCompleted},
                {"degree",acadRow.degree},
                {"when",acadRow.when},
                {"where",acadRow.where},
                {"remarks",acadRow.remarks}}));
        }




        

       
    }

    class facultyData 
    {
        
        public ObjectId _id { get; set; }
        public string term { get; set; }
        public string facultyID { get; set; }
        public string acadYear { get; set; }
        public string program { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string dateOfBirth { get; set; }
        public int age { get; set; }
        public string rank { get; set; }
        public int yearsOfTeachingS { get; set; }
        public int yearsOfTeachingO { get; set; }
        public string status { get; set; }
        public string services { get; set; }
        [BsonIgnoreIfNull]
        [BsonElementAttribute("validcertorlicense")]
        public List<certification> validCertificateOrLicense { get; set; }
        [BsonIgnoreIfNull]
        public List<schedule> scheduleOfClasses { get; set; }
        [BsonIgnoreIfNull]
        public List<acadQualifications> academicQualifications { get; set; }

      
    }

    class certification
    {
        public ObjectId _id { get; set; }
        public string title { get; set; }
        public string number { get; set; }
        public string dateIssued { get; set; }
        public string validUntil { get; set; }
        public string issuingAgency { get; set; }
    }

    class schedule
    {
        public ObjectId _id { get; set; }
        public string time { get; set; }
        public string courseNumber { get; set; }
        public string courseName { get; set; }
        public string courseYear { get; set; }
        public int creditUnits { get; set; }
        public int numTeachingHours { get; set; }
        public string room { get; set; }
        public int numStudents { get; set; }
    }

    class acadQualifications
    {
        public ObjectId _id { get; set; }
        public string degreeCompleted { get; set; }
        public string degree { get; set; }
        public string when { get; set; }
        public string where { get; set; }
        public string remarks { get; set; }
    }


}
