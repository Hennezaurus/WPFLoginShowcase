using System;
using System.Collections.Generic;
using System.IO;
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

namespace WPFExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Default hint text for username box
        const string InputHintText = "Please enter username...";
        
        // List of valid logins - read from file
        List<LoginData> validLogins = new List<LoginData>();

        // Used instead of black for text
        SolidColorBrush darkGrey;

        // Page constructor
        public MainWindow()
        {
            InitializeComponent();

            // Find proper dark gray if possible
            darkGrey = (SolidColorBrush)TryFindResource("DarkGrey");

            // Load all our saved files into objects
            LoadSaveFiles();
        }

        // Username Box Events - Mainly setting up the hint text system
        private void Input_Initialized(object sender, EventArgs e)
        {
            // Set default text and colour
            Input.Text = InputHintText;
            Input.Foreground = Brushes.Gray;
        }

        private void Input_GotFocus(object sender, RoutedEventArgs e)
        {
            // Change text to black
            Input.Foreground = darkGrey;

            // If we only have hint text in the box, delete it
            if(Input.Text == InputHintText)
            {
                Input.Text = "";
            }
        }

        private void Input_LostFocus(object sender, RoutedEventArgs e)
        {
            // Set back to hint text, and hint colour when deselected while empty
            if(Input.Text == "")
            {
                Input.Text = InputHintText;
                Input.Foreground = Brushes.Gray;
            }
        }

        // Password Box clearing when you click in it, every time
        private void passwordInput_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox self = (PasswordBox)sender;

            self.Foreground = darkGrey;
            self.Password = "";
        }

        private void passwordInput_LostFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox self = (PasswordBox)sender;
            if (self.Password == "")
            {
                self.Foreground = Brushes.Gray;
                self.Password = "xxxxxxxx";
            }
        }

        // Login button - When clicked
        private void login_Click(object sender, RoutedEventArgs e)
        {
            // Create login attempt from data currently in username and password field
            LoginData loginAttempt = new LoginData(Input.Text, passwordInput.Password);

            // If it's a match to something in our list
            bool isValid = IsValidLogin(loginAttempt);

            // Output relevant result
            if(isValid)
            {
                //Output.Text = "Success";
            }
            else
            {
                //Output.Text = "Failure";
            }
        }

        // Create Account button - when clicked
        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            if (IsUsernameAvailable(Input.Text))
            {
                // Add to list of valid accounts
                LoginData newAccount = new LoginData(Input.Text, passwordInput.Password);
                validLogins.Add(newAccount);

                // Save this new account to the save file
                SaveAccountToFile(newAccount);

                // Show something has happened
                Input.Text = "";
                passwordInput.Password = "";
                //Output.Text = "Account Created!";
            }
            else
            {
                // Username already exists, and remove fields
                //Output.Text = "Sorry! That username is already taken.";
                Input.Text = "";
                passwordInput.Password = "";
            }
        }


        // Check if our login attempt matches something in our valid list
        private bool IsValidLogin(LoginData input)
        {
            foreach(LoginData poop in validLogins)
            {
                if(input.Username == poop.Username && input.Password == poop.Password)
                {
                    return true;
                }
            }

            return false;
        }

        // Is the attempted username already in use?
        private bool IsUsernameAvailable(string usernameAttempt)
        {
            foreach (LoginData data in validLogins)
            {
                if(usernameAttempt == data.Username)
                {
                    return false;
                }
            }

            return true;
        }

        // Read all data from save file to our valid logins list
        private void LoadSaveFiles()
        {
            // Get current application directory and add our save folder
            /* Create the aboslute file path to the location we want to read from. On my PC this will create:
             * C:\Users\Matthew\Documents\Visual Studio 2015\Projects\WPFExample\WPFExample\bin\Debug\SaveData\WriteFile.txt
             * which we need to check if the file exists, and to read from it */

            string savePath = Directory.GetCurrentDirectory() + @"\SaveData\WriteFile.txt";

            // Don't try any of this if the file doesn't exist
            /* Note that ! reverses a boolean. So this is the same as writing:
             * bool fileExists = File.Exists(savePath);
             * if(fileExists == false);
             * Instead we're simply saying "If the file doesn't exist, don't run this function, instead
             * Just return, thus do nothing" */
            
            if (!File.Exists(savePath))
                return;

            // Create save file
            /* This will read each line of the file, so each account, in the format:
             * Username,Password
             * Into it's own element of the string array: saveFiles */

            string[] saveFiles = File.ReadAllLines(savePath);

            // Convert each file line into a Login object
            foreach (string account in saveFiles)
            {
                // Split each line into two elements, username and password
                /* .Split(',') splits a string into chunks at ever specified character, in our case this
                 * is the comma. This converts our line in the file: username,password to an array with 2 elements,
                 * where details[0] is the username, before the comma
                 * and   details[1] is the password, after the comma
                 * with the comma removed */

                string[] details = account.Split(',');

                // Re-Create objects from elements
                /* Using details[0] for the username, and details[1] for the password
                 * we turn these two separate strings back into actual 'LoginAccount' objects
                 * and add them to our List of valid accounts */

                LoginData loginAccount = new LoginData(details[0], details[1]);
                validLogins.Add(loginAccount);
            }
        }

        // Save account to disk
        private void SaveAccountToFile(LoginData newAccount)
        {
            // Create save entry for new account
            /* This is so that we save the account in the following format:
             * Username,Password
             * Note that the Environment.NewLine is simply to add a 'new line' character
             * at the end of the string, so that the document is stored like this:
             * Username1,password1
             * username2,Password2
             * etc.
             * Otherwise it would all be in one line. Could be substituted for a simple \n 
             * in Windows, but this is safer, and better practice. */

            string text = newAccount.Username + "," + newAccount.Password + Environment.NewLine;

            // Get current application directory and add our save folder
            /* Directory.GetCurrentDirectory() returns the current working directory of
             * the executable. This will be in the form, for example on my computer:
             * C:\Users\Matthew\Documents\Visual Studio 2015\Projects\WPFExample\WPFExample\bin\Debug
             * This lets us just add the folder name we want to create on the end, in our case
             * making a sub-folder called 'SaveData' to store our save file neatly.
             * The '@' is just so that the string doesn't translate the backslash as a metacharacter
             * But instead treats it as a literal backslash, which is what we want here */

            string savePath = Directory.GetCurrentDirectory() + @"\SaveData";

            // Create sub-directory if needed
            /* The first time you ever create an account, this will create that sub-folder. In all
             * subsequent runs, it will simply skip this step */
            Directory.CreateDirectory(savePath);

            // Create save file
            /* Add our actual save file name to this file path (including our sub-folder)
             * And store our 'text' in it, which will be the new account we're adding. We use
             * 'AppendAllText' as it adds the required information, rather than overriding the 
             * existing data. Note that the method looks like this:
             * AppendAllText(string filePath, string textToAdd)
             * By adding @"\WriteFile.txt" we're specifying the actual file name we want to create/edit
             * to the existing savePath which we specified to create the sub-folder we want to save in */
            File.AppendAllText(savePath + @"\WriteFile.txt", text);
        }

        
    }
}
