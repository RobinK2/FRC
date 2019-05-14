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
    public class FRCPoule
    {
        //The poule number.
        private int pouleNumber;
        //The piste number.
        private string pisteNumber;
        //Start time of the poule.
        private string startTime;
        //List of referees in this poule.
        private List<FRCReferee> referee = new List<FRCReferee>();
        //List of fencers in this poule.
        private List<FRCFencer> fencer = new List<FRCFencer>();
        //List of teams in this poule.
        private List<FRCTeam> team = new List<FRCTeam>();
        //List of matches in this poule.
        private List<FRCMatch> match = new List<FRCMatch>();
        //Index for list of fencers.
        private int fencerIdx = -1;
        //Index for list of teams.
        private int teamIdx = -1;
        //Index for match list.
        private int matchIdx = -1;
        //Index for referee list.
        private int refereeIdx = -1;

        public FRCPoule()
        {
        }

        /// <summary>
        /// Add a team to this poule.
        /// </summary>
        /// <param name="team"></param>
        public void addTeam(FRCTeam team)
        {
            this.team.Add(team);
        }

        /// <summary>
        /// Returns team at next index.
        /// </summary>
        /// <returns></returns>
        public FRCTeam getNextTeam()
        {
            teamIdx++;

            if (teamIdx >= team.Count)
                teamIdx = 0;

            if (teamIdx < team.Count)
                return team[teamIdx];

            return null;
        }

        /// <summary>
        /// Returns amount of teams.
        /// </summary>
        /// <returns></returns>
        public int amountOfTeams()
        {
            return team.Count;
        }

        /// <summary>
        /// Sorts team after their fencer number in this poule.
        /// </summary>
        public void sortTeamsFencerNumberInPoule()
        {
            for (int i = 0; i < team.Count - 1; i++)
                for (int j = i + 1; j < team.Count; j++)
                {
                    if (team[i].FencerNumberInPoule > team[j].FencerNumberInPoule)
                    {
                        FRCTeam tempTeam = team[j];
                        team[j] = team[i];
                        team[i] = tempTeam;
                    }
                }
        }

        /// <summary>
        /// Add a referee to the poule.
        /// </summary>
        /// <param name="referee"></param>
        public void addReferee(FRCReferee referee)
        {
            this.referee.Add(referee);
        }

        /*
        /// <summary>
        /// Returns referee at current index.
        /// </summary>
        /// <returns></returns>
        public FRCReferee getCurrentReferee()
        {
            if (refereeIdx >= referee.Count)
                return null;

            return referee[refereeIdx];
        }
        */

        /// <summary>
        /// Returns referee at next index.
        /// </summary>
        /// <returns></returns>
        public FRCReferee getNextReferee()
        {
            refereeIdx++;

            if (refereeIdx >= referee.Count)
                refereeIdx = 0;

            if (refereeIdx < referee.Count)
                return referee[refereeIdx];
            
            return null;
        }

        /// <summary>
        /// Returns the amount referees.
        /// </summary>
        /// <returns></returns>
        public int amountOfReferees()
        {
            return referee.Count;
        }

        /// <summary>
        /// Returns true if the list of referees has next element.
        /// </summary>
        /// <returns></returns>
        public bool hasNextReferee()
        {
            if (refereeIdx + 1 < referee.Count)
                return true;
            else
            {
                refereeIdx = -1;
                return false;
            }
        }

        /// <summary>
        /// Add a fencer to the poule.
        /// </summary>
        /// <param name="fenc"></param>
        public void addFencer(FRCFencer fencer)
        {
            this.fencer.Add(fencer);
        }

        /*
        /// <summary>
        /// Returns fencer at current index.
        /// </summary>
        /// <returns></returns>
        public FRCFencer getCurrentFencer()
        {
            if (fencerIdx >= fencer.Count)
                return null;

            return fencer[fencerIdx];
        }
        */

        /// <summary>
        /// Returns fencer at next index.
        /// </summary>
        /// <returns></returns>
        public FRCFencer getNextFencer()
        {
            fencerIdx++;

            if (fencerIdx >= fencer.Count)
                fencerIdx = 0;

            if (fencerIdx < fencer.Count)
                return fencer[fencerIdx];

            return null;
        }

        /// <summary>
        /// Returns amount of fencers.
        /// </summary>
        /// <returns></returns>
        public int amountOfFencers()
        {
            return fencer.Count;
        }

        /// <summary>
        /// Sorts fencer after their fencer number in this poule.
        /// </summary>
        public void sortFencersFencerNumberInPoule()
        {
            for (int i = 0; i < fencer.Count - 1; i++)
                for (int j = i + 1; j < fencer.Count; j++)
                {
                    if (fencer[i].FencerNumberInPoule > fencer[j].FencerNumberInPoule)
                    {
                        FRCFencer tempFencer = fencer[j];
                        fencer[j] = fencer[i];
                        fencer[i] = tempFencer;
                    }
                }
        }

        /// <summary>
        /// Returns true if the list of fencers has next element.
        /// </summary>
        /// <returns></returns>
        public bool hasNextFencer()
        {
            if (fencerIdx + 1 < fencer.Count)
                return true;
            else
            {
                fencerIdx = -1;
                return false;
            }
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
        /// Returns amount of matches in this poule.
        /// </summary>
        /// <returns></returns>
        public int amountOfMatches()
        {
            return match.Count;
        }

        /// <summary>
        /// Property for poule number.
        /// </summary>
        public int PouleNumber
        {
            set { pouleNumber = value; }
            get { return pouleNumber; }
        }
        
        /// <summary>
        /// Property for piste number.
        /// </summary>
        public string PisteNumber
        {
            set { pisteNumber = value; }
            get { return pisteNumber; }
        }

        /// <summary>
        /// Property for the start time.
        /// </summary>
        public string StartTime
        {
            set { startTime = value; }
            get { return startTime; }
        }
    }
}
