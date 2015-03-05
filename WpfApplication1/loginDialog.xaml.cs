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


using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Builders;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for loginDialog.xaml
    /// </summary>
    public partial class loginDialog : Window
    {
        MongoClient mongoClient = new MongoClient();
        MongoServer server;
        MongoDatabase database;
        MongoCollection<admin> collection;

        public loginDialog()
        {
           InitializeComponent();
            

            
        }

        private void enterButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            server = mongoClient.GetServer();
            database = server.GetDatabase("facultyDataAndSchedule");
            collection = database.GetCollection<admin>("admin");


            var query = collection.FindOne(Query.EQ("username", userTexbox.Text));
       
         
            var username =query.username;
            var password = query.password;
            
            if (userTexbox.Text == username.ToString() && passwordbox.Password == password.ToString())
                main.ShowDialog();
            else
                MessageBox.Show("Wrong Credentials", "Re-enter Credentials");
       
           
        }

        
    }
    class admin
    {
       public ObjectId _id { get; set; }
       public string username { get; set; }
       public string password { get; set; }
    }


}
