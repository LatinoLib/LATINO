/*=====================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    Tutorial4_3.cs
 *  Desc:    Tutorial 4.3: Supervised machine learning
 *  Created: Apr-2010
 *
 *  Authors: Miha Grcar
 *
 **********************************************************************/

using System;

namespace Tutorial.Example
{
    public class Tutorial4_3 : Tutorial<Tutorial4_3>
    {
        static void Test(params object[] waka)
        {
            Console.WriteLine(waka == null);
        }

        public override void Run(string[] args)
        {
            /*
            DatabaseConnection dbCon = new DatabaseConnection();
            dbCon.SetConnectionString(DatabaseType.SqlServer2005);
            dbCon.Username = "sa";
            dbCon.Password = "sa";
            dbCon.Server = "\\SQLEXPRESS";
            dbCon.Database = "SloWebCorpus";
            dbCon.Connect();
            // ...
            dbCon.Disconnect();
            


            //Console.WriteLine(WebUtils.NormalizeQuery("ata +fabo + - - -\"maMma   mia\"       -dec\"\" -\"\""));
            Console.WriteLine("\"{0}\"", WebUtils.NormalizeQuery(""));
            Test();
            return;
             
            YahooSearchEngine searchEngine = new YahooSearchEngine("internet");
            searchEngine.Language = Language.French;
            searchEngine.ResultSetMaxSize = 300;
            searchEngine.Search();
            int c = 0;
            foreach (SearchEngineResultItem item in searchEngine.ResultSet)
            {
                Console.WriteLine("{0}. {1}\r\n{2}\r\n", ++c, item.Title, item.Snippet);
            }
             * 
             */
        }
    }
}