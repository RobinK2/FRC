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
    public class FRCReferee
    {
        //The ID number of the referee in the xml file.
        private int refereeID;
        //The first name.
        private string firstName;
        //The last name.
        private string lastName;
        //The club.
        private string club;
        //The nationality.
        private string nationality;
        //The referee category.
        private string category;
        //Amount of refereed poules.
        private int refereedPoules = 0;
        //Amount of refereed matches.
        private int refereedMatches = 0;
        //Amount of refereed quarter finals.
        private int refereedQuarterFinals = 0;
        //Amount of refereed semi finals and finals.
        private int refereedFinals = 0;


        public FRCReferee(int refereeID, string firstName, string lastName, string club, string nationality, string category)
        {
            this.refereeID = refereeID;
            this.firstName = firstName;
            this.lastName = lastName;
            this.club = club;
            this.nationality = nationality;
            this.category = category;
        }

        /// <summary>
        /// Read only property for refereeID.
        /// </summary>
        public int RefereeID
        {
            get { return refereeID; }
        }

        /// <summary>
        /// Read only property for first name.
        /// </summary>
        public string FirstName
        {
            get { return firstName; }
        }

        /// <summary>
        /// Read only property for last name.
        /// </summary>
        public string LastName
        {
            get { return lastName; }
        }

        /// <summary>
        /// Read only property for club.
        /// </summary>
        public string Club
        {
            get { return club; }
        }

        /// <summary>
        /// Read only property for nationality.
        /// </summary>
        public string Nationality
        {
            get { return nationality; }
        }

        /// <summary>
        /// Read only property for category.
        /// </summary>
        public string Category
        {
            get { return category; }
        }

        public int RefereedPoules
        {
            set { refereedPoules = value; }
            get { return refereedPoules; }
        }

        public int RefereedMatches
        {
            set { refereedMatches = value; }
            get { return refereedMatches; }
        }

        public int RefereedQuarterFinals
        {
            set { refereedQuarterFinals = value; }
            get { return refereedQuarterFinals; }
        }

        public int RefereedFinals
        {
            set { refereedFinals = value; }
            get { return refereedFinals; }
        }
    }
}
