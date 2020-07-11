using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

/*
 * Author:  James Kidd
 * Date:    July 8, 2020
 * Purpose: Main execution client for renewable energy reporting application.
 */

namespace INFO_3138_Project_2___Renewable_Energy
{
    class Program {

        const string XML_FILE = "renewable-energy.xml";

        static void Main(string[] args)
        {

            // Create and populate the DOM
            XmlDocument document = new XmlDocument();
            try
            {
                document.Load(XML_FILE);
            }
            catch (XmlException ex)
            {
                Console.WriteLine($"DOM ERROR: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GENERAL ERROR: {ex.Message}");
            }

            //user input loop
            bool exit = false;
            do
            {
                char menuSelection = Helper.PrintMainMenu();
                switch (menuSelection)
                {
                    case 'C':
                        Helper.ReportOnCountry(Helper.PrintCountryMenu(document), document);
                        break;

                    case 'R':
                        Helper.ReportOnEnergyType(document);
                        break;

                    case 'P':
                        Helper.ReportOnPercent(document);
                        break;

                    case 'X':
                        exit = true;
                        break;
                } 
            } while (!exit);
        }
    }
}
