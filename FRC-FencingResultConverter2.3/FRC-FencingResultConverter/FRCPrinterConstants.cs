#region PDFsharp - A .NET library for processing PDF
//
// Copyright (c) 2005-2012 empira Software GmbH, Troisdorf (Germany)
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRC_FencingResultConverter
{
    public class FRCPrinterConstants
    {
        //These constants are used for FRCPdfPrinter.
        public const string FINAL_RESULTS = "Final results";
        public const string INITIAL_RANKING = "Initial ranking";
        public const string REFEREE_ACTIVITY = "Referee activity";
        public const string FENCERS_UPPER = "Fencers";
        public const string FENCERS_LOWER = "fencers";
        public const string TEAMS_UPPER = "Teams";
        public const string TEAMS_LOWER = "teams";
        public const string PLACE = "Place";
        public const string NAME = "Name";
        public const string NATION = "Nation";
        public const string CLUB = "Club";
        public const string FORMULA_OF_COMPETITION = "Formula of the competition";
        public const string POULES_ROUND = "Poules round";
        public const string POULES_UPPER = "Poules";
        public const string POULES_LOWER = "poules";
        public const string QUALIFIERS = "qualifiers";
        public const string DIRECT_ELIMINATION = "Direct elimination";
        public const string NUMBER = "number";
        public const string RANKING = "Ranking";
        public const string RANK = "Rank";
        public const string POULE = "Poule";
        public const string POULES = "Poules";
        public const string PISTE = "Piste";
        public const string REFEREE = "Referee";
        public const string RANKING_OF_POULES = "Ranking of poules";
        public const string V_M = "V/M";
        public const string HS_HR = "HS-HR";
        public const string HS = "HS";
        public const string Q_E = "Q/E";
        public const string TABLE_OF = "Table of";
        public const string SEMI_FINALS = "Semi-finals";
        public const string Final = "Final";
        public const string ROUND = "round";
        public const string QUALIFIED = "qualified";
        public const string ELIMINATED = "eliminated";
        public const string ABANDONED = "abandoned";
        public const string RANKING_AFTER_ALL_POULE_ROUNDS = "Ranking after all poule rounds";
        public const string TEAM = "Team";
        public const string TABLE_9_16 = "Table 9-16";
        public const string TABLE_9_12 = "Table 9-12";
        public const string TABLE_13_16 = "Table 13-16";
        public const string TABLE_5_8 = "Table 5-8";
        public const string PLACE3 = "3rd place";
        public const string PLACE5 = "5th place";
        public const string PLACE7 = "7th place";
        public const string PLACE9 = "9th place";
        public const string PLACE11 = "11th place";
        public const string PLACE13 = "13th place";
        public const string PLACE15 = "15th place";
        public const string CATEGORY = "Category";
        public const string MATCHES = "Matches";
        public const string QUARTER_FINALS = "Q-Finals";
        public const string FINALS = "Finals";


        /*
        private static int[] tableauOf2 = { 1, 2 };
        private static int[] tableauOf4 = { 1, 4, 3, 2 };
        private static int[] tableauOf8 = { 1, 8, 5, 4, 3, 6, 7, 2 };
        private static int[] tableauOf16 = { 1, 16, 9, 8, 5, 12, 13, 4, 3, 14, 11, 6, 7, 10, 15, 2 };
        private static int[] tableauOf32 = { 1, 32, 17, 16, 9, 24, 25, 8, 5, 28, 21, 12, 13, 20, 29, 4, 3, 30, 19, 14, 11, 22, 27, 6, 
                                               7, 26, 23, 10, 15, 18, 31, 2 };
        private static int[] tableauOf64 = { 1, 64, 33, 32, 17, 48, 49, 16, 9, 56, 41, 24, 25, 40, 57, 8, 5, 60, 37, 28, 21, 44, 53, 12,
                                               13, 52, 45, 20, 29, 36, 61, 4, 3, 62, 35, 30, 19, 46, 51, 14, 11, 54, 43, 22, 27, 38, 59, 6,
                                               7, 58, 39, 26, 23, 42, 55, 10, 15, 50, 47, 18, 31, 34, 63, 2 };
        */

        public FRCPrinterConstants()
        {
        }

        /*
        /// <summary>
        /// Returns rank of the fencer at given line number in tableau part.
        /// </summary>
        /// <param name="lineNumber"></param>
        /// <param name="tableauSize">The size of the tableau part.</param>
        /// <returns></returns>
        public static int getTableauNumber(int lineNumber, int tableauSize)
        {
            if (tableauSize == 2)
                return tableauOf2[lineNumber - 1];
            else if (tableauSize == 4)
                return tableauOf4[lineNumber - 1];
            else if (tableauSize == 8)
                return tableauOf8[lineNumber - 1];
            else if (tableauSize == 16)
                return tableauOf16[lineNumber - 1];
            else if (tableauSize == 32)
                return tableauOf32[lineNumber - 1];
            else if (tableauSize == 64)
                return tableauOf64[lineNumber - 1];
            else
                return -1;
        }
        */
    }
}
