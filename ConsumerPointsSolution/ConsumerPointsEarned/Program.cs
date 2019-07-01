using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerPointsEarned
{
    class Program
    {
        static List<Data> dataList;
        static string fileName = "MyData.txt";

        static Dictionary<string, Dictionary<string, int>> monthlyPointTotals = 
            new Dictionary<string, Dictionary<string, int>>();

        static void Main(string[] args)
        {
            // Get data from file
            try
            {
                dataList = LoadJson();
            }
            catch (Exception e) { Console.Out.WriteLine(e); }

            // Create a container of people and their monthly point totals
            foreach (Data data in dataList)
            {
                AddPoints(data);
            }

            // Print the results to console
            PrintResults();
        }

        private static void PrintResults()
        {
            // For each customer, extract their Name and Monthly Totals
            foreach (KeyValuePair<string, Dictionary<string, int>> customerTotals in monthlyPointTotals)
            {
                string name = customerTotals.Key;

                // For each Month that a customer accrued points, print the Monthly Total
                // and add it to the grand total
                int totalPoints = 0;
                foreach (KeyValuePair<string, int> monthlyTotals in customerTotals.Value)
                {
                    string month = monthlyTotals.Key;
                    int points = monthlyTotals.Value;
                    totalPoints += points;

                    Console.Out.WriteLine(name + " accrued " + points 
                        + " points in the month of " + month + ".");
                }

                // Print the Total number of points accrued by the customer
                Console.Out.WriteLine(name + " accrued " + totalPoints 
                    + " points in total.");
            }
        }

        private static void AddPoints(Data data)
        {
            // If purchase amount won't result in points, don't continue;
            if (data.PurchaseAmount < 50) return;

            // Extract values for readability
            string name = data.CustomerName;
            float amount = data.PurchaseAmount;
            string month = data.PurchaseDate.ToString("MMMM");

            // Calculate points
            int points = CalculatePoints(amount);

            // If customer is new, add customer
            if (!monthlyPointTotals.ContainsKey(name))
            {
                monthlyPointTotals.Add(name, new Dictionary<string, int>());
            }

            // If month is new, add month. Else, add points
            if (!monthlyPointTotals[name].ContainsKey(month))
            {
                monthlyPointTotals[name].Add(month, points);
            }
            else
            {
                monthlyPointTotals[name][month] += points;
            }
        }

        private static int CalculatePoints(double purchaseAmount)
        {
            int points = 0;
            int dollarAmount = (int)purchaseAmount;  // Full dollars only

            // Assign 1 point for every dollar spent above 50
            if (dollarAmount > 50)
            {
                int dollarsOverFifty = dollarAmount - 50;
                points += dollarsOverFifty;
            }

            // Dollar amounts over 100 will receive an extra point
            if (dollarAmount > 100)
            {
                int dollarsOverOneHundred = dollarAmount - 100;
                points += dollarsOverOneHundred;
            }

            return points;
        }

        private static List<Data> LoadJson()
        {
            using (StreamReader streamReader = new StreamReader(fileName))
            {
                string json = streamReader.ReadToEnd();
                List<Data> items = JsonConvert.DeserializeObject<List<Data>>(json);
                return items;
            }
        }

        class Data
        {
            private string customerName;
            private DateTime purchaseDate;
            private float purchaseAmount;
            public string CustomerName
            {
                get { return customerName; }
                set { customerName = value; }
            }
            public DateTime PurchaseDate
            {
                get { return purchaseDate; }
                set { purchaseDate = value; }
            }
            public float PurchaseAmount
            {
                get { return purchaseAmount; }
                set { purchaseAmount = value; }
            }
        }
    }
}
