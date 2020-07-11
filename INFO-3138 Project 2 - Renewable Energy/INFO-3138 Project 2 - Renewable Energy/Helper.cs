using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.XPath;

/*
 * Author:  James Kidd
 * Date:    July 8, 2020
 * Purpose: Helper methods for renewable energy reporting app.
 */


namespace INFO_3138_Project_2___Renewable_Energy
{

    
    abstract class Helper
    {
        const int MAX_WIDTH = 80;


        /// <summary>
        /// Prints the main menu.
        /// </summary>
        /// <returns>
        /// System.Char - the selected menu item
        /// </returns>
        public static char PrintMainMenu()
        {
            Console.WriteLine("\n\n");
            const string title = "Renewable Energy Production in 2016";
            Console.WriteLine(title.PadLeft((MAX_WIDTH - 2) / 2 + title.Length / 2)); //center title across 80 chars
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
            for (int i = 0; i <= MAX_WIDTH-2; i++)
            {
                Console.Write("=");
            }
            Console.WriteLine("+");
        }

        /// <summary>
        /// Prints the country menu.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <returns>
        /// System.String - The country the user selected from the menu
        /// </returns>
        public static string PrintCountryMenu(XmlDocument doc)
        {

            string country = "";
            try {

                XmlNode root = doc.DocumentElement;
                XmlElement rootElement = (XmlElement)root;
                XmlNodeList allCountries = rootElement.SelectNodes("//country");
                int countryCount = allCountries.Count;

                if (countryCount > 0)
                {
                    Console.WriteLine("\n\n");
                    for (int i = 0; i < allCountries.Count; i++)
                    {
                        //print name attribute for every country
                        Console.Write($"\t{i + 1}.  {allCountries[i].Attributes[0].InnerText,-32}");

                        if ((i + 1) % 2 == 0)
                            Console.WriteLine();
                    }
                }

                bool validInput;
                int selection;
                do
                {
                    Console.WriteLine("\nSelect a country by number:");
                    Console.Write("\n  > ");

                    string userInput = Console.ReadLine();

                    validInput = int.TryParse(userInput, out selection);

                } while (!validInput || !(selection > 0 && selection <= countryCount));

                XmlAttributeCollection xmlAttributeCollection = allCountries[selection - 1].Attributes;
                if (xmlAttributeCollection != null)
                    country = xmlAttributeCollection[0].InnerText;
            }//try
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

            return country;
        }

        /// <summary>
        /// Reports the on country.
        /// </summary>
        /// <param name="country">The country.</param>
        /// <param name="doc">The document.</param>
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

                    for (int j = 0; j < renewables[i].Attributes.Count; j++)
                    {
                        map[j] = renewables[i].Attributes[j].InnerText;
                    }

                    if (int.TryParse(map[1], out int tmp))
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

        /// <summary>
        /// Report based on specific energy type.
        /// </summary>
        /// <param name="doc">The document.</param>
        public static void ReportOnEnergyType(XmlDocument doc)
        {
            try
            {
                XmlElement rootElement = (XmlElement)doc.DocumentElement;
                XmlNodeList types = rootElement.SelectNodes("//country[1]/renewable/@type");
                XmlNodeList allCountries = rootElement.SelectNodes("//country");

                int selection;
                bool validInput;

                do
                {
                    Console.WriteLine("\n\nSelect a renewable by number as shown below...\n");

                    for (int i = 0; i < types.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {types[i].InnerText}");
                    }

                    Console.Write("\n  > ");

                    string userInput = Console.ReadLine();
                    validInput = int.TryParse(userInput, out selection);

                } while (!validInput || !(selection > 0 && selection <= types.Count));

                string selectedType = types[selection - 1].InnerText;
                XmlNodeList selectedRenewables = rootElement.SelectNodes($"//country/renewable[contains(@type, '{selectedType}')]");

                int maxCountryLength = 20;
                int padding = 20;
                string title = $"{char.ToUpper(selectedType[0]) + selectedType.Substring(1)} Energy Production\n";
                Console.WriteLine(title.PadLeft((MAX_WIDTH - 2) / 2 + title.Length / 2));
                Console.Write("Country".PadLeft(padding));
                Console.Write("Amount (Gwh)".PadLeft(padding));
                Console.Write("% of Total".PadLeft(padding));
                Console.Write("% of Renewables\n".PadLeft(padding));
                DrawDivider();
                Console.WriteLine();

                for (int i = 0; i < selectedRenewables.Count; i++)
                {
                    var attrList = new List<string> { "n/a", "n/a", "n/a" };
                    var currentCountry = allCountries[i].Attributes[0].InnerText;

                    Console.Write((currentCountry.Length > maxCountryLength
                        ? currentCountry.PadLeft(padding).Remove(0, currentCountry.Length - maxCountryLength)
                        : currentCountry.PadLeft(padding)));


                    for (int j = 0; j < selectedRenewables[i].Attributes.Count; j++)
                    {
                        switch (selectedRenewables[i].Attributes[j].Name)
                        {
                            case "amount":
                                if (double.TryParse(selectedRenewables[i].Attributes[j].InnerText, out double tmpAmount))
                                {
                                    attrList[0] = tmpAmount.ToString("#,0.##");
                                }
                                break;

                            case "percent-of-all":
                                if (double.TryParse(selectedRenewables[i].Attributes[j].InnerText, out double tmpOfAll))
                                {
                                    attrList[1] = tmpOfAll.ToString("#,0.##");
                                }
                                break;

                            case "percent-of-renewables":
                                if (double.TryParse(selectedRenewables[i].Attributes[j].InnerText, out double tmpOfRenew))
                                {
                                    attrList[2] = tmpOfRenew.ToString("#,0.##");
                                }
                                break;
                        }
                    }

                    foreach (string attr in attrList)
                    {
                        Console.Write(attr.PadLeft(padding));
                    }
                    Console.WriteLine();
                }

                Console.WriteLine($"\n\n{allCountries.Count} match(es) found\n");
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

        /// <summary>
        /// Reports based on percentage of renewable energy.
        /// </summary>
        /// <param name="doc">The document.</param>
        public static void ReportOnPercent(XmlDocument doc)
        {
            const double MIN_PERCENT = 0.0, MAX_PERCENT = 100;
            double userMin = 0.0, userMax = 0.0;
            bool isValid = false, isLimited = true;

            do
            {
                Console.Write("\nEnter the minimum % of renewable produced or press enter for no minimum: > ");
                string tmpStr = Console.ReadLine();

                if (string.IsNullOrEmpty(tmpStr))
                {
                    userMin = MIN_PERCENT;
                }
                else
                {
                    if ((double.TryParse(tmpStr, out double tmpDblResult)) && tmpDblResult >= MIN_PERCENT)
                    {
                        userMin = tmpDblResult;
                    }
                    else continue;
                }

                Console.Write("\nEnter the maximum % of renewable produced or press enter for no maximum: > ");
                tmpStr = Console.ReadLine();

                if (string.IsNullOrEmpty(tmpStr))
                {
                    userMax = MAX_PERCENT;
                }
                else
                {
                    if ((double.TryParse(tmpStr, out double tmpDblResult)) && tmpDblResult <= MAX_PERCENT)
                    {
                        userMax = tmpDblResult;
                    }
                    else continue;
                }

                if (userMin <= userMax)
                {
                    isValid = true;
                }

            } while (!isValid);
            Console.WriteLine("\n");

            string searchType;
            if (userMin > MIN_PERCENT && userMax.Equals(MAX_PERCENT))
            {
                searchType = $"at least {userMin}%";
            }
            else if (userMin.Equals(MIN_PERCENT) && userMax < MAX_PERCENT)
            {
                searchType = $"up to {userMax}%";
            }
            else
            {
                searchType = $"{userMin}% to {userMax}%";
            }

            if (userMin.Equals(MIN_PERCENT) && userMax.Equals(MAX_PERCENT))
            {
                isLimited = false;
                Console.WriteLine("Combined Renewables for all countries");
            }
            else
            {
                Console.WriteLine($"Countries where renewables account for {searchType} of energy production");
            }

            Console.WriteLine();
            const int padding = 20;
            Console.Write("Country".PadLeft(padding));
            Console.Write("All Energy (Gwh)".PadLeft(padding));
            Console.Write("Renewable (Gwh)".PadLeft(padding));
            Console.Write("% Renewable\n".PadLeft(padding));
            DrawDivider();

            try
            {
                int maxCountryLength = 20;
                XmlElement rootElement = (XmlElement)doc.DocumentElement;
                 
                XmlNodeList selectedCountries = isLimited ? 
                    rootElement.SelectNodes($"//country[totals/@renewable-percent <= {userMax} and totals/@renewable-percent >= {userMin}]") :
                    rootElement.SelectNodes("//country");

                XmlNodeList selectedTotals = isLimited ?
                    rootElement.SelectNodes($"//country[totals/@renewable-percent <= {userMax} and totals/@renewable-percent >= {userMin}]/totals") :
                    rootElement.SelectNodes("//country/totals");


                for (int i = 0; i < selectedCountries.Count; i++)
                {
                    var attrList = new List<string> { "n/a", "n/a", "n/a" };

                    var currentCountry = selectedCountries[i].Attributes[0].InnerText;


                    Console.Write((currentCountry.Length > maxCountryLength
                        ? currentCountry.PadLeft(padding).Remove(0, currentCountry.Length - maxCountryLength)
                        : currentCountry.PadLeft(padding)));

                    for (int j = 0; j < selectedTotals[i].Attributes.Count; j++)
                    {
                        switch (selectedTotals[i].Attributes[j].Name)
                        {
                            case "all-sources":
                                if (double.TryParse(selectedTotals[i].Attributes[j].InnerText, out double tmpAmount))
                                {
                                    attrList[0] = tmpAmount.ToString("#,0.##");
                                }
                                break;

                            case "all-renewables":
                                if (double.TryParse(selectedTotals[i].Attributes[j].InnerText, out double tmpOfAll))
                                {
                                    attrList[1] = tmpOfAll.ToString("#,0.##");
                                }
                                break;

                            case "renewable-percent":
                                if (double.TryParse(selectedTotals[i].Attributes[j].InnerText, out double tmpOfRenew))
                                {
                                    attrList[2] = tmpOfRenew.ToString("#,0.##");
                                }
                                break;
                        }
                    }

                    foreach (string attr in attrList)
                    {
                        Console.Write(attr.PadLeft(padding));
                    }
                    Console.WriteLine();
                }

                Console.WriteLine($"\n{selectedTotals.Count} match(es) found\n\n");
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