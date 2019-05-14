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
    public class FRCFencer : FRCCompetitor, ICloneable
    {
        //The first name.
        private string firstName;
        //The last name.
        private string lastName;
        
        public FRCFencer(int fencerID, string firstName, string lastName, string nationality, string club, int finalRanking)
        {
            this.ID = fencerID;
            this.firstName = firstName;
            this.lastName = lastName;
            this.Nationality = nationality;
            this.Club = club;
            this.FinalRanking = finalRanking;
        }

        public FRCFencer(int fencerID, string firstName, string lastName, string nationality, string club)
        {
            this.ID = fencerID;
            this.firstName = firstName;
            this.lastName = lastName;
            this.Nationality = nationality;
            this.Club = club;
        }

        public FRCFencer()
        {
        }

        /// <summary>
        /// Returns a clone of this fencer.
        /// </summary>
        /// <returns></returns>
        public Object Clone()
        {
            FRCFencer cFencer = new FRCFencer();
            cFencer.Abandoned = this.Abandoned;
            cFencer.Club = this.Club;
            cFencer.Excluded = this.Excluded;
            cFencer.ID = this.ID;
            cFencer.FencerNumberInPoule = this.FencerNumberInPoule;
            cFencer.FinalRanking = this.FinalRanking;
            cFencer.FirstName = this.FirstName;
            cFencer.Forfait = this.Forfait;
            cFencer.HitsGivenInPoule = this.HitsGivenInPoule;
            cFencer.HitsTakenInPoule = this.HitsTakenInPoule;
            cFencer.InitialRanking = this.InitialRanking;
            cFencer.IsStillInTheCompetition = this.IsStillInTheCompetition;
            cFencer.LastName = this.LastName;
            cFencer.Nationality = this.Nationality;
            cFencer.NumberOfMatchesInPoule = this.NumberOfMatchesInPoule;
            cFencer.NumberOfVictoriesInPoule = this.NumberOfVictoriesInPoule;
            cFencer.PhaseFinalRanking = this.PhaseFinalRanking;
            cFencer.PhaseInitialRanking = this.PhaseInitialRanking;
            cFencer.PouleRanking = this.PouleRanking;
            cFencer.VM_String = this.VM_String;

            return cFencer;
        }

        /// <summary>
        /// Property for first name.
        /// </summary>
        public string FirstName
        {
            set { firstName = value; }
            get { return firstName; }
        }

        /// <summary>
        /// Property for last name.
        /// </summary>
        public string LastName
        {
            set { lastName = value; }
            get { return lastName; }
        }

    }
}
