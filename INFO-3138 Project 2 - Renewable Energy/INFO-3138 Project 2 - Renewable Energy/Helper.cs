using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.XPath;

namespace INFO_3138_Project_2___Renewable_Energy
{
    abstract class Helper
    {

        public static char PrintMainMenu()
        {
            Console.WriteLine("\n\n");
            string title = "Renewable Energy Production in 2016";
            Console.WriteLine(title.PadLeft((Console.WindowWidth - 2) / 2 + title.Length / 2));

            DrawDivider();



            string userInput;
            char selection = 'X';

            do
            {
                Console.WriteLine("\n\n");
                Console.WriteLine("Enter one of the following options:\n");
                Console.WriteLine(" 'C' To select from a list of countries");
                Console.WriteLine(" 'R' To select a specific renewable energy source");
                Console.WriteLine(" 'P' To select a % range of renewable production");
                Console.WriteLine(" 'X' To Exit Program\n");
                Console.Write("  > ");

                userInput = Console.ReadLine();
                if (userInput.Length == 1)
                    selection = char.Parse(userInput.ToUpper());


            } while (userInput.Length != 1 || !"CRPX".Contains(selection));

            return selection;
        }

        /// <summary>
        /// Draws the divider.
        /// </summary>
        private static void DrawDivider()
        {
            Console.Write("+");
            for (int i = 0; i <= Console.WindowWidth - 3; i++)
            {
                Console.Write("=");
            }
            Console.WriteLine("+");
        }

        public static string PrintCountryMenu(XmlDocument doc)
        {

            string country = "";

            try
            {

                XmlNode root = doc.DocumentElement;
                XmlElement rootElement = (XmlElement)root;
                XmlNodeList allCountries = rootElement.SelectNodes("//country");
                var countryCount = allCountries.Count;

                if (countryCount > 0)
                {
                    Console.WriteLine("\n\n");
                    for (int i = 0; i < allCountries.Count; i++)
                    {
                        Console.Write($"\t{i + 1}.  {allCountries[i].Attributes[0].InnerText,-32}");

                        if ((i + 1) % 3 == 0)
                            Console.WriteLine();
                    }
                }

                bool validInput;
                int selection;
                do
                {
                    Console.WriteLine("Select a country by number:");
                    Console.Write("\n  > ");

                    var userInput = Console.ReadLine();

                    validInput = int.TryParse(userInput, out selection);

                } while (!validInput || !(selection > 0 && selection <= countryCount));

                var xmlAttributeCollection = allCountries[selection - 1].Attributes;
                if (xmlAttributeCollection != null)
                    country = xmlAttributeCollection[0].InnerText;
            }//try
            catch (XmlException ex)
            {
                Console.WriteLine($"DOM ERROR: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GENERAL ERROR: {ex.Message}");
            }

            return country;
        }

        public static void ReportOnCountry(string country, XmlDocument doc)
        {
            Console.WriteLine($"\n\nRenewable Energy Production in {country}");
            DrawDivider();
            Console.WriteLine("{0,20}{1,20}{2,20}{3,20}", " Renewable Type", "Amount (GWh)", "% of Total", "% of Renewables");
            Console.WriteLine("{0,20}{1,20}{2,20}{3,20}", "---------------", "------------", "----------", "---------------");

            try
            {
                XmlNode root = doc.DocumentElement;
                XmlElement rootElement = (XmlElement)root;
                XmlNodeList renewables = rootElement.SelectNodes($"//country[contains(@name, '{country}')]/renewable");

                for (int i = 0; i < renewables.Count; i++)
                {
                    var map = new Dictionary<int, string>();
                    map.Add(0, "n/a");
                    map.Add(1, "n/a");
                    map.Add(2, "n/a");
                    map.Add(3, "n/a");

                    for (var j = 0; j < renewables[i].Attributes.Count; j++)
                    {
                        map[j] = renewables[i].Attributes[j].InnerText;
                    }

                    if (int.TryParse(map[1], out var tmp))
                    {
                        map[1] = tmp.ToString("N0");
                    }

                    Console.WriteLine("{0,20}{1,20}{2,20}{3,20}", $"{map[0]}", $"{map[1]}", $"{map[2]}", $"{map[3]}");
                }
                Console.WriteLine($"\n\n{renewables.Count} match(es) found.");
            }
            catch (XmlException ex)
            {
                Console.WriteLine($"DOM ERROR: {ex.Message}");
            }
            catch (XPathException ex)
            {
                Console.WriteLine($"XPath ERROR: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GENERAL ERROR: {ex.Message}");
            }
        }
    }
}