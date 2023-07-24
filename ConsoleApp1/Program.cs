using System.Globalization;

internal class PHTracker
{
    private readonly Dictionary<string, double> pHData;

    public PHTracker()
    {
        pHData = new Dictionary<string, double>();
    }

    public void Run()
    {
        bool exitProgram = false;
        string userInputQuit = string.Empty;
        int count = 0;
        string[] dates = new string[31];
        double[] pHValues = new double[31];

        Console.WriteLine("========================================");
        Console.WriteLine("=                                      =");
        Console.WriteLine("=          Riverbed pH Logger          =");
        Console.WriteLine("=                                      =");
        Console.WriteLine("========================================");

        do
        {
            // Display main menu options
            DisplayMainMenu();
            string userInput = Prompt("Enter main menu option ('D' to display menu): ");

            switch (userInput.ToUpper())
            {
                case "N":
                    // Enter pH values
                    count = EnterLogEntries(pHValues, dates);
                    break;

                case "S":
                    string fileName;
                    do
                    {
                        fileName = Prompt("Enter the filename to save the pH log: ");
                    } while (string.IsNullOrEmpty(fileName));

                    // Save pH log
                    SaveLogFile(fileName, pHValues, dates, count);
                    //SavePHLog();
                    break;

                case "E":
                    // Load pH log
                    //LoadPHLog();
                    EditEntries(pHValues, dates, count);
                    break;

                case "L":
                    // Load pH log
                    count = LoadLogFile(pHValues, dates);
                    break;
                case "V":
                    // View and edit pH values
                    DisplayEntries(pHValues, dates, count);
                    break;

                case "D":
                    // View analysis of pH log
                    DisplayAnalysisMenu();
                    break;

                case "Q":
                    // Quit the program
                    exitProgram = true;
                    userInputQuit = Prompt("\nAre you sure you want to quit (y/n)?: ");

                    break;

                default:
                    Console.WriteLine("Invalid input. Please try again.");
                    break;
            }
        } while ((!exitProgram && !userInputQuit.ToLower().Equals("y")) ||
             (exitProgram && !userInputQuit.ToLower().Equals("y")));

        Console.Write("\nProgram terminated. Press any key to exit...");
        _ = Console.ReadLine();
    }

    private void DisplayMainMenu()
    {
        //Console.WriteLine("=== Main Menu ===");
        //Console.WriteLine("1. Enter pH values");
        //Console.WriteLine("2. Save pH log");
        //Console.WriteLine("3. Load pH log");
        //Console.WriteLine("4. View and edit pH values");
        //Console.WriteLine("5. View analysis of pH log");
        //Console.WriteLine("6. Quit");
        Console.WriteLine("\nMAIN MENU");
        Console.WriteLine("----------------------------------------");
        Console.WriteLine("[N]ew Daily pH Entry");
        Console.WriteLine("[S]ave entries to file");
        Console.WriteLine("[E]dit pH Entries");
        Console.WriteLine("[L]oad pH Log File");
        Console.WriteLine("[V]iew loaded Log File");
        Console.WriteLine("[M]onthly Statistics");
        Console.WriteLine("[D]isplay Main Menu");
        Console.WriteLine("[Q]uit Program\n");
    }

    private void DisplayAnalysisMenu()
    {
        Console.WriteLine("=== Analysis Menu ===");
        Console.WriteLine("1. Mean average daily pH");
        Console.WriteLine("2. Median of daily pH");
        Console.WriteLine("3. Highest daily pH");
        Console.WriteLine("4. Lowest daily pH");
        Console.WriteLine("5. Chart of daily pH");
        Console.WriteLine("6. Back to main menu");
    }

    private string Prompt(string promptString)
    {
        Console.Write(promptString);
        return Console.ReadLine();
    }

    private double PromptDouble(string promptString)
    {
        double result;
        bool validInput;

        do
        {
            Console.Write(promptString);
            validInput = double.TryParse(Console.ReadLine(), out result);

            if (!validInput || result < 0.1 || result > 14)
            {
                Console.WriteLine("Invalid input. Please enter a valid number between 0.1 and 14.");
            }

        }

        while (!validInput);

        return result;
    }

    //private void EnterPHValues()
    //{
    //    // Prompt the user to enter pH values
    //    int entryCount = EnterLogEntries();

    //    if (entryCount > 0)
    //    {
    //        Console.WriteLine("pH values entered successfully.");
    //    }

    //    else
    //    {
    //        Console.WriteLine("No pH values entered.");
    //    }
    //}

    private int EnterLogEntries(double[] pHValues, string[] dates)
    {
        Console.WriteLine("\nEnter pH values. Press Enter without input to stop.");

        int count = 1;
        bool isValid;

        //while (true)
        //{        
        do
        {
            // Prompt for date
            string date = Prompt("Enter date (MM-dd-yyyy): ");

            if (string.IsNullOrEmpty(date))
            {
                isValid = false;
            }
            else
            {
                try
                {
                    DateOnly newDate = DateOnly.Parse(date);

                    if (dates.Contains(newDate.ToString("MM-dd-yyyy")))
                    {
                        _ = Prompt($"The specified date already exists. Please enter a different date.");
                        isValid = false;
                    }
                    else
                    {
                        if (dates[0] == null || string.IsNullOrEmpty(dates[0]))
                        {
                            dates[0] = date;
                        }
                        else
                        {
                            for (int i = 1; i < dates.Length; i++)
                            {
                                if (dates[i] == null || string.IsNullOrEmpty(dates[i]))
                                {
                                    dates[i] = date;
                                    break;
                                }
                            }
                        }

                        isValid = true;
                    }
                }
                catch
                {
                    isValid = false;
                }
            }
        } while (!isValid);


        // Prompt for pH value
        double pHValue = PromptDouble("Enter pH value: ");

        if (pHValues[0] == 0)
        {
            pHValues[0] = pHValue;
        }
        else
        {
            for (int i = 1; i < pHValues.Length; i++)
            {
                if (pHValues[i] == 0)
                {
                    pHValues[i] = pHValue;
                    count++;
                    break;
                }
            }
        }

        //if (string.IsNullOrEmpty(date))
        //{
        //    break;
        //}

        //if (pHData.ContainsKey(date))
        //      {
        //          Console.WriteLine("pH value for the specified date already exists. Please enter a different date.");
        //          //continue;
        //      }

        //// Add the pH value to the dictionary
        //pHData.Add(date, pHValue);
        //count++;

        //}

        return count;
    }

    private void SaveLogFile(string filename, double[] pHValues, string[] dates, int count)
    {
        if (count == 0)
        {
            Console.WriteLine("No pH values to save.");
            return;
        }

        try
        {
            using (StreamWriter writer = new(filename))
            {
                writer.WriteLine("Date,pH Level");

                for (int i = 0; i < count; i++)
                {
                    if (dates[i] == null)
                    {
                        break;
                    }

                    writer.WriteLine($"{dates[i]},{pHValues[i]:F1}");
                }
            }

            Console.WriteLine("pH log saved successfully.");
            _ = Console.ReadLine();
        }
        catch (IOException e)
        {
            Console.WriteLine($"Error detected when saving file: {e.Message}");
        }
    }

    //private void SavePHLog()
    //   {
    //       if (pHData.Count == 0)
    //       {
    //           Console.WriteLine("No pH values to save.");
    //           return;
    //       }

    //       // Prompt for filename
    //       string filename = Prompt("Enter the filename to save the pH log: ");
    //       string filePath = $"{filename}.csv";

    //       using (StreamWriter writer = new(filePath))
    //       {
    //           // Write header record
    //           writer.WriteLine("Date,pH Level");

    //           // Write pH values to the file
    //           foreach (KeyValuePair<string, double> entry in pHData)
    //           {
    //               string formattedDate = entry.Key;
    //               double pHValue = entry.Value;

    //               writer.WriteLine($"{formattedDate},{pHValue.ToString("0.0", CultureInfo.InvariantCulture)}");
    //           }
    //       }

    //       Console.WriteLine("pH log saved successfully.");
    //   }

    private int LoadLogFile(double[] pHValues, string[] dates)
    {
        string fileName = Prompt("Enter the filename to load the pH log: ");
        _ = $"{fileName}.csv";

        //if (!File.Exists(filePath))
        //      {
        //          Console.WriteLine("File not found.");
        //          return 0;
        //      }

        try
        {
            int count = 0;

            using (StreamReader reader = new(fileName))
            {
                _ = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');

                    if (values.Length == 2)
                    {
                        dates[count] = values[0];

                        if (double.TryParse(values[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double pHValue))
                        {
                            pHValues[count] = pHValue;
                        }

                        count++;
                    }
                }
            }

            Console.WriteLine("pH log loaded successfully.");
            _ = Console.ReadLine();

            return count;
        }
        catch (IOException e)
        {
            Console.WriteLine($"Error detected in attempting to load file: {e.Message}");

            return 0;
        }
    }

    //private void LoadPHLog()
    //{
    //    // Prompt for filename
    //    string filename = Prompt("Enter the filename to load the pH log: ");

    //    string filePath = $"{filename}.csv";

    //    if (!File.Exists(filePath))
    //    {
    //        Console.WriteLine("File not found.");
    //        return;
    //    }

    //    // Clear existing pH data
    //    pHData.Clear();

    //    using (StreamReader reader = new(filePath))
    //    {
    //        // Skip header record
    //        _ = reader.ReadLine();

    //        // Read and parse pH values from the file
    //        while (!reader.EndOfStream)
    //        {
    //            string line = reader.ReadLine();
    //            string[] values = line.Split(',');

    //            if (values.Length == 2)
    //            {
    //                string date = values[0];

    //                if (double.TryParse(values[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double pHValue))
    //                {
    //                    pHData.Add(date, pHValue);
    //                }
    //            }
    //        }
    //    }

    //    Console.WriteLine("pH log loaded successfully.");
    //}

    private void EditEntries(double[] pHValues, string[] dates, int count)
    {
        int logID;

        for (int i = 0; i < count; i++)
        {
            Console.WriteLine($" {i + 1}  {dates[i]}      {pHValues[i]:N1}");
        }

        do
        {
            Console.Write("\nEnter the ID to edit: ");
            _ = int.TryParse(Console.ReadLine(), out logID);

            if (logID < 1 || logID > count)
            {
                Console.WriteLine($"Enter ID between 1 and {count}");
            }
            else
            {
                pHValues[logID - 1] = PromptDouble("Enter pH values: ");
            }
        } while (logID == 0 || logID > count);

        _ = Console.ReadLine();
    }

    private void ViewAndEditPHValues()
    {
        if (pHData.Count == 0)
        {
            Console.WriteLine("No pH values to view or edit.");
            return;
        }

        // Display current log entries
        //DisplayEntries();

        // Prompt for the date to edit
        string dateToEdit = Prompt("Enter the date to edit the pH value: ");

        if (pHData.ContainsKey(dateToEdit))
        {
            // Prompt for the new pH value
            double newPHValue = PromptDouble("Enter the new pH value: ");

            // Update the pH value in the dictionary
            pHData[dateToEdit] = newPHValue;
            Console.WriteLine("pH value edited successfully.");
        }

        else
        {
            Console.WriteLine("Invalid date. Please try again.");
        }
    }

    private void DisplayEntries(double[] pHValues, string[] dates, int count)
    {
        //Console.WriteLine("=== Current Log entries ===");
        //Console.WriteLine("Date\t\t\tPH Value");
        //Console.WriteLine("---------------------------");

        //// Display each log entry
        //foreach (KeyValuePair<string, double> entry in pHData)
        //{
        //    string formattedDate = entry.Key;
        //    double pHValue = entry.Value;

        //    Console.WriteLine($"{formattedDate}\t{pHValue.ToString("0.0", CultureInfo.InvariantCulture)}");
        //}

        Console.WriteLine("\nCurrent Log Entries");
        Console.WriteLine("===================\n");
        Console.WriteLine("Date       pH Value");
        Console.WriteLine("-------------------");

        for (int i = 0; i < count; i++)
        {
            Console.WriteLine($"{dates[i]}      {pHValues[i]:N1}");
        }

        _ = Console.ReadLine();
    }

    private void DisplayChart()
    {
        if (pHData.Count == 0)
        {
            Console.WriteLine("No pH values to display.");
            return;
        }

        // Determine the highest and lowest pH values
        double highestPH = double.MinValue;
        double lowestPH = double.MaxValue;

        foreach (double pHValue in pHData.Values)
        {
            if (pHValue > highestPH)
            {
                highestPH = pHValue;
            }

            if (pHValue < lowestPH)
            {
                lowestPH = pHValue;
            }
        }

        _ = pHData.Count;
        _ = (int)Math.Ceiling(highestPH - lowestPH) + 1;

        Console.WriteLine("=== Chart of Entry Data ===");
        Console.WriteLine("pH");
        Console.WriteLine("-----------------------------");

        // Display the chart
        for (double i = highestPH; i >= lowestPH; i--)
        {
            Console.Write($"{i.ToString("0.0", CultureInfo.InvariantCulture)}|");

            foreach (KeyValuePair<string, double> entry in pHData)
            {
                string date = entry.Key;
                double pHValue = entry.Value;

                if (Math.Abs(pHValue - i) < 0.05)
                {
                    Console.Write(" *");
                }

                else
                {
                    Console.Write("  ");
                }
            }

            Console.WriteLine();
        }

        Console.WriteLine("---------------------------------");

        Console.Write("Day |");

        foreach (KeyValuePair<string, double> entry in pHData)
        {
            string date = entry.Key.Substring(3, 2); // Extract day from date (MM-dd-yyyy)
            Console.Write($" {date}");
        }

        Console.WriteLine();
    }

    private void DisplayPHAnalysis()
    {
        if (pHData.Count == 0)
        {
            Console.WriteLine("No pH values to analyze.");
            return;
        }

        // Calculate mean average pH
        double meanAverage = CalculateMeanAverage();

        // Calculate median pH
        double median = CalculateMedian();

        // Find highest and lowest pH values
        double highestPH = double.MinValue;
        double lowestPH = double.MaxValue;

        foreach (double pHValue in pHData.Values)
        {
            if (pHValue > highestPH)
            {
                highestPH = pHValue;
            }

            if (pHValue < lowestPH)
            {
                lowestPH = pHValue;
            }
        }

        Console.WriteLine($"Mean average pH: {meanAverage.ToString("0.0", CultureInfo.InvariantCulture)}");
        Console.WriteLine($"Median pH: {median.ToString("0.0", CultureInfo.InvariantCulture)}");
        Console.WriteLine($"Highest pH: {highestPH.ToString("0.0", CultureInfo.InvariantCulture)}");
        Console.WriteLine($"Lowest pH: {lowestPH.ToString("0.0", CultureInfo.InvariantCulture)}");
    }

    private double CalculateMeanAverage()
    {
        double sum = 0;

        // Calculate the sum of all pH values
        foreach (double pHValue in pHData.Values)
        {
            sum += pHValue;
        }

        // Calculate the mean average
        return sum / pHData.Count;
    }

    private double CalculateMedian()
    {
        int count = pHData.Count;
        List<double> sortedValues = new(pHData.Values);
        sortedValues.Sort();

        if (count % 2 == 0)
        {
            int middleIndex1 = (count / 2) - 1;
            int middleIndex2 = count / 2;
            return (sortedValues[middleIndex1] + sortedValues[middleIndex2]) / 2;
        }

        else
        {
            int middleIndex = count / 2;
            return sortedValues[middleIndex];
        }
    }
}

internal class Program
{
    private static void Main()
    {
        PHTracker phTracker = new();
        phTracker.Run();
    }
}


///	<summary>
///	Purpose:CPSC1012 Assignment 3
///	input:  Ph logs
///	output: saved ph logs
/// process(es):  enter daily pH values, save daily pH values to a log file,  load previous monthly pH log files, o view and edit previously entered pH values (current or previous monthly data), view simple analysis of the currently loaded log file:
//o Mean average daily pH
//o Median of daily pH
//o Highest daily pH
//o Lowest daily pH
//o Chart of daily pH
/// Author: Michael Essex
/// Last modified: 2023.07.23
///	</summary>
