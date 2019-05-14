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
    public class FRCMatch
    {
        private int matchID;
        private int fencerID1;
        private int fencerID2;
        private int scoreFencer1;
        private int scoreFencer2;
        private int tableauRankFencer1;
        private int tableauRankFencer2;
        //fencer1 won the match if true, lost if false.
        private bool fencer1Win;
        private bool fencer1Abandon = false;
        private bool fencer1Forfait = false;
        private bool fencer1Exclusion = false;
        //fencer2 won the match if true, lost if false.
        private bool fencer2Win;
        private bool fencer2Abandon = false;
        private bool fencer2Forfait = false;
        private bool fencer2Exclusion = false;

        public FRCMatch(int fencerID1, int fencerID2, int scoreFencer1, int scoreFencer2, bool fencer1Win, bool fencer2Win)
        {
            this.fencerID1 = fencerID1;
            this.fencerID2 = fencerID2;
            this.scoreFencer1 = scoreFencer1;
            this.scoreFencer2 = scoreFencer2;
            this.fencer1Win = fencer1Win;
            this.fencer2Win = fencer2Win;
        }

        public FRCMatch()
        {
        }

        /// <summary>
        /// Property for fencerID1.
        /// </summary>
        public int FencerID1
        {
            set { fencerID1 = value; }
            get { return fencerID1; }
        }

        /// <summary>
        /// Property for fencerID2.
        /// </summary>
        public int FencerID2
        {
            set { fencerID2 = value; }
            get { return fencerID2; }
        }

        /// <summary>
        /// Property for scoreFencer1.
        /// </summary>
        public int ScoreFencer1
        {
            set { scoreFencer1 = value; }
            get { return scoreFencer1; }
        }

        /// <summary>
        /// Property for ScoreFencer2.
        /// </summary>
        public int ScoreFencer2
        {
            set { scoreFencer2 = value; }
            get { return scoreFencer2; }
        }

        /// <summary>
        /// Property for tableauRankFencer1.
        /// </summary>
        public int TableauRankFencer1
        {
            set { tableauRankFencer1 = value; }
            get { return tableauRankFencer1; }
        }

        /// <summary>
        /// Property for tableauRankFencer2.
        /// </summary>
        public int TableauRankFencer2
        {
            set { tableauRankFencer2 = value; }
            get { return tableauRankFencer2; }
        }

        /// <summary>
        /// Property for fencer1Win. True if fencer1 has won.
        /// </summary>
        public bool Fencer1Win
        {
            set { fencer1Win = value; }
            get { return fencer1Win; }
        }

        /// <summary>
        /// Property for fencer2Win. True if fencer2 has won.
        /// </summary>
        public bool Fencer2Win
        {
            set { fencer2Win = value; }
            get { return fencer2Win; }
        }

        /// <summary>
        /// Property for fencer1Abandon. True if fencer1 has abandoned.
        /// </summary>
        public bool Fencer1Abandon
        {
            set { fencer1Abandon = value; }
            get { return fencer1Abandon; }
        }

        /// <summary>
        /// Property for fencer2Abandon. True if fencer2 has abandoned.
        /// </summary>
        public bool Fencer2Abandon
        {
            set { fencer2Abandon = value; }
            get { return fencer2Abandon; }
        }

        /// <summary>
        /// Property for fencer1Forfait. True if fencer1 got forfait.
        /// </summary>
        public bool Fencer1Forfait
        {
            set { fencer1Forfait = value; }
            get { return fencer1Forfait; }
        }

        /// <summary>
        /// Property for fencer2Forfait. True if fencer2 got forfait.
        /// </summary>
        public bool Fencer2Forfait
        {
            set { fencer2Forfait = value; }
            get { return fencer2Forfait; }
        }

        /// <summary>
        /// Property for fencer1Exlcusion. True if fencer1 is excluded.
        /// </summary>
        public bool Fencer1Exclusion
        {
            set { fencer1Exclusion = value; }
            get { return fencer1Exclusion; }
        }

        /// <summary>
        /// Property for fencer2Exclusion. True if fencer2 is excluded.
        /// </summary>
        public bool Fencer2Exclusion
        {
            set { fencer2Exclusion = value; }
            get { return fencer2Exclusion; }
        }

        /// <summary>
        /// Property for matchID.
        /// </summary>
        public int MatchID
        {
            set { matchID = value; }
            get { return matchID; }
        }
    }
}
