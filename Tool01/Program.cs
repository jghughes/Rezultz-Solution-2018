using NetStd.Prism.July2018;

namespace Tool01
{
    internal class Program
    {
        private const string Description = "This program is intended to be a dirty little throw-away scratch pad. Do your work in Main01(). Erase it when you are finished.";


        private static void Main()
        {
            JghConsoleHelper.WriteLineWrappedInOne("Welcome.");
            JghConsoleHelper.WriteLineFollowedByOne(Description);
            JghConsoleHelper.WriteLineWrappedInOne("Are you ready to go? Press enter to continue.");
            JghConsoleHelper.ReadLine();
            JghConsoleHelper.WriteLineWrappedInTwo("Working. Please wait...");

            try
            {
                Main01();

                JghConsoleHelper.WriteLineWrappedInOne("FINISH. Press enter to close.");
                JghConsoleHelper.ReadLine();
            }

            #region trycatch

            catch (Exception ex)
            {
                JghConsoleHelper.WriteLineWrappedInOne(ex.Message);
                JghConsoleHelper.WriteLineWrappedInOne("Sorry. Unable to continue. Rectify the error and try again.");
            }

            #endregion
        }

        private static void Main01()
        {
            DateTime startDate = new DateTime(2023, 9, 19);
            DateTime endDate = new DateTime(2024, 3, 6);

            double elapsedPeriodFromStartOfZsunToFourthCategoryChange = (endDate - startDate).TotalDays;

            int zz = (int) (elapsedPeriodFromStartOfZsunToFourthCategoryChange + 90);

            var yy = new TimeSpan(zz, 0, 0, 0);

            DateTime theoreticalCommencementOfLoggingOfPowerDataByZwift = endDate - yy;

            var altStartDate = endDate.AddDays(-90);

            //https://www.bing.com/search?pglt=41&q=c%23+prism+Delegate+command+ObservesProperty+examples+how+to+use&cvid=48efceb7847c4fe2a1c362602bbcbc3b&gs_lcrp=EgZjaHJvbWUyBggAEEUYOTIGCAEQRRg60gEJNDc0MDJqMGoxqAIAsAIA&FORM=ANSPA1&PC=DCTS&showconv=1

            JghConsoleHelper.WriteLineWrappedInOne("The DelegateCommand.ObservesProperty method allows you to specify one or more properties that are used to determine if the command\r\n" +
                                                   "can execute, and automatically raises the CanExecuteChanged event when any of the observed properties change.\r\n" +
                                                   "This is useful when you want to enable or disable a command based on the state of your view model properties.\r\n\r\n" +
                                                   "To use this method, you need to pass an expression that returns the property you want to observe, such as () => PropertyName.\r\n" +
                                                   "You can chain multiple calls to this method to observe more than one property, such as .ObservesProperty(() => Property1).ObservesProperty(() => Property2).\r\n" +
                                                   "You also need to implement the INotifyPropertyChanged interface in your view model and raise the PropertyChanged event in the setters of the observed properties.");

            var xx = new MyViewModel();


            xx.SomethingChanged = true;
            xx.SomethingChanged = false;
        }

        #region constants

        //private const int LhsWidth = 50;

        //private const string InputFolder = @"C:\Users\johng\holding pen\StuffByJohn\Input";
        //private const string OutputFolderForXml = @"C:\Users\johng\holding pen\StuffByJohn\Output";
        //private const string OutputFolderForJson = @"C:\Users\johng\holding pen\StuffByJohn\Output";

        //private const bool MustDoWorkForXmlOutput = true;
        //private const bool MustDoWorkForJsonOutput = true;

        #endregion

        public class MyViewModel : BindableBase
        {
            // A simple example that observes a single property called SomethingChanged and executes a method called WriteLine when the command is invoked:
            private bool _somethingChanged;
            public bool SomethingChanged
            {
                get { return _somethingChanged; }
                set { SetProperty(ref _somethingChanged, value); }
            }

            public DelegateCommand SaveCommand { get; set; }

            public MyViewModel()
            {
                SaveCommand = new DelegateCommand(WriteLine, () => true).ObservesProperty(() => SomethingChanged);
            }

            private void WriteLine()
            {
                Console.WriteLine("Something changed. The observation worked!");
                // WriteLine logic here
            }
        }


        public class PersonViewModel : BindableBase
        {
            // A more complex example that observes two properties called FirstName and LastName and executes a method called Register when the command is invoked.
            // The command also checks if the properties are not null or empty before enabling the command
            private string _firstName;
            public string FirstName
            {
                get { return _firstName; }
                set { SetProperty(ref _firstName, value); }
            }

            private string _lastName;
            public string LastName
            {
                get { return _lastName; }
                set { SetProperty(ref _lastName, value); }
            }

            public DelegateCommand RegisterCommand { get; set; }

            public PersonViewModel()
            {
                RegisterCommand = new DelegateCommand(Register, CanRegister).ObservesProperty(() => FirstName).ObservesProperty(() => LastName);
            }

            private bool CanRegister()
            {
                return !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName);
            }

            private void Register()
            {
                // Register logic here
            }
        }
    }
}
