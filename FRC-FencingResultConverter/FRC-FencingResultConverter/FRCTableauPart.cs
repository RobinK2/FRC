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
    public class FRCTableauPart
    {
        //List of matches in this part of tableau.
        private List<FRCMatch> match = new List<FRCMatch>();
        //The title of this part of the tableau.
        private string title;
        //Maximum number of fencers in this part of tableau.
        private int length;
        //Index for match list.
        private int matchIdx = -1;

        public FRCTableauPart()
        {
        }
        
        /// <summary>
        /// Add a new match to the match list.
        /// </summary>
        /// <param name="match"></param>
        public void addMatch(FRCMatch match)
        {
            this.match.Add(match);
        }

        /// <summary>
        /// Returns match at current index.
        /// </summary>
        /// <returns></returns>
        public FRCMatch getCurrentMatch()
        {
            if (matchIdx >= match.Count)
                return null;

            return match[matchIdx];
        }

        /// <summary>
        /// Returns match at next index.
        /// </summary>
        /// <returns></returns>
        public FRCMatch getNextMatch()
        {
            matchIdx++;

            if (matchIdx >= match.Count)
                matchIdx = 0;

            if (matchIdx < match.Count)
                return match[matchIdx];
            
            return null;
        }

        /// <summary>
        /// Returns amount of matches in this tableau part.
        /// </summary>
        /// <returns></returns>
        public int amountOfMatches()
        {
            return match.Count;
        }

        /// <summary>
        /// Return true if the list of matches has next element.
        /// </summary>
        /// <returns></returns>
        public bool hasNextMatch()
        {
            if (matchIdx + 1 < match.Count)
                return true;
            else
            {
                matchIdx = -1;
                return false;
            }
        }


        /// <summary>
        /// Property for title of the tableau part.
        /// </summary>
        public string Title
        {
            set { title = value; }
            get { return title; }
        }

        /// <summary>
        /// Property for the maximum number of fencers in this part of tableau.
        /// </summary>
        public int Length
        {
            set { length = value; }
            get { return length; }
        }
    }
}
