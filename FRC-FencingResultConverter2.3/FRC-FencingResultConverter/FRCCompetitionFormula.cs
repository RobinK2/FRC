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
    public class FRCCompetitionFormula
    {
        //The name of the competition.
        private string competitionName;
        //Short version of the competition name.
        private string competitionNameShort;
        //Individual competition if true, team competition if false.
        private bool individualCompetitionType;
        //Amout of fencers.
        private int numOfFencers;
        //Amount of referees.
        private int numOfReferees;
        //Amount of rounds of poules.
        private int roundsOfPoules;
        //Amount of poules per round. The index is the poule round -1.
        private int[] numOfPoules;
        //Number of qualifiers per poule round. Index is the poule round -1.
        private int[] numOfQualifiers;
        
        public FRCCompetitionFormula()
        {
        }

        public FRCCompetitionFormula(string competitionName, string competitionNameShort, int numOfFencers, int numOfReferees, 
            int roundsOfPoules)
        {
            this.competitionName = competitionName;
            this.competitionNameShort = competitionNameShort;
            this.numOfFencers = numOfFencers;
            this.numOfReferees = numOfReferees;
            this.roundsOfPoules = roundsOfPoules;
            numOfPoules = new int[roundsOfPoules];
            numOfQualifiers = new int[roundsOfPoules];
        }

        /// <summary>
        /// Property for the competition name.
        /// </summary>
        public string CompetitionName
        {
            set { competitionName = value; }
            get { return competitionName; }
        }

        public string CompetitionNameShort
        {
            set { competitionNameShort = value; }
            get { return competitionNameShort; }
        }

        /// <summary>
        /// Property for the competition type. Individual competiton if true, team competitionn otherwise.
        /// </summary>
        public bool IsIndiviualCompetiton
        {
            set { individualCompetitionType = value; }
            get { return individualCompetitionType; }
        }
    }
}
