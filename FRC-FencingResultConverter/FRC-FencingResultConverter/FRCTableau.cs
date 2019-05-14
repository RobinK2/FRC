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
    public class FRCTableau
    {
        //List of fencers in the tableau.
        private List<FRCFencer> fencer = new List<FRCFencer>();
        //List of all teams during this poule round.
        private List<FRCTeam> team = new List<FRCTeam>();
        //List of tableau parts.
        private  List<FRCTableauPart> tableauPart = new List<FRCTableauPart>();
        //Index for list of fencers.
        private int fencerIdx = -1;
        //Index for list of teams.
        private int teamIdx = -1;
        //Index for list of tableau parts.
        private int tableauPartIdx = -1;
        private List<int> tableauNumber = new List<int>();

        public FRCTableau()
        {
        }

        /// <summary>
        /// Add a team to the poule round.
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
        /// Returns true if the list of teams has next element.
        /// </summary>
        /// <returns></returns>
        public bool hasNextTeam()
        {
            if (teamIdx + 1 < team.Count)
                return true;
            else
            {
                teamIdx = -1;
                return false;
            }
        }

        /// <summary>
        /// Add a fencer to the tableau.
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
        /// Add tableau part.
        /// </summary>
        /// <param name="tableauPart"></param>
        public void addTableauPart(FRCTableauPart tableauPart)
        {
            this.tableauPart.Add(tableauPart);
        }

        /// <summary>
        /// Returns tableau part at current index.
        /// </summary>
        /// <returns></returns>
        public FRCTableauPart getCurrentTableauPart()
        {
            if (tableauPartIdx >= tableauPart.Count)
                return null;

            return tableauPart[tableauPartIdx];
        }

        /// <summary>
        /// Returns amount of tableau parts.
        /// </summary>
        /// <returns></returns>
        public int amountOfTableauParts()
        {
            return tableauPart.Count;
        }

        /// <summary>
        /// Returns tableau part at next index.
        /// </summary>
        /// <returns></returns>
        public FRCTableauPart getNextTableauPart()
        {
            tableauPartIdx++;

            if (tableauPartIdx >= tableauPart.Count)
                tableauPartIdx = 0;

            if (tableauPartIdx < tableauPart.Count)
                return tableauPart[tableauPartIdx];

            return null;            
        }

        /// <summary>
        /// Returns true if list of tableau parts has next element.
        /// </summary>
        /// <returns></returns>
        public bool hasNextTableauPart()
        {
            if (tableauPartIdx + 1 < tableauPart.Count)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Resets tableauPartIdx.
        /// </summary>
        public void resetTableauPartIndex()
        {
            tableauPartIdx = -1;
        }

        /// <summary>
        /// Resets fencerIdx.
        /// </summary>
        public void resetFencerIndex()
        {
            fencerIdx = -1;
        }

        /// <summary>
        /// Resets teamIdx.
        /// </summary>
        public void resetTeamIndex()
        {
            teamIdx = -1;
        }
                
        /// <summary>
        /// Checks if there are fencers with same initial rankings, and fix them if needed. 
        /// This method must be called before printing a tableau.
        /// </summary>
        public void checkRankingIND()
        {
            int rank1, rank2;
            for (int i = 0; i < amountOfFencers(); i++)
            {
                rank1 = fencer[i].PhaseInitialRanking;
                for (int j = i + 1; j < amountOfFencers(); j++)
                {
                    rank2 = fencer[j].PhaseInitialRanking;
                    if (rank1 == rank2)
                        correctRankingIND(rank1);
                }
            }
        }

        private void correctRankingIND(int rank)
        {
            FRCFencer fencer1, fencer2;
            FRCMatch match;
            int c = 0;

            for (int i = 0; i < amountOfTableauParts(); i++)
            {
                calculateTableauNumbers(tableauPart[i].Length);

                for (int j = 0; j < tableauPart[i].amountOfMatches(); j++)
                {
                    match = tableauPart[i].getNextMatch();
                    fencer1 = getFencerMatchingID(match.FencerID1);
                    fencer2 = getFencerMatchingID(match.FencerID2);

                    //This skip is needed if the xml file is extended format.
                    if (fencer1 == null || fencer2 == null)
                        continue;

                    if ((fencer1.PhaseInitialRanking == rank) && (c < 2))
                    {
                        int idx = (int) ((match.MatchID-1) * 2);
                        fencer1.PhaseInitialRanking = tableauNumber[idx];
                        c++;
                    }

                    if ((fencer2.PhaseInitialRanking == rank) && (c < 2))
                    {
                        int idx = (int)((match.MatchID-1) * 2) + 1;
                        fencer2.PhaseInitialRanking = tableauNumber[idx];
                        c++;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if there are teams with same initial rankings, and fix them if needed. 
        /// This method must be called before printing a tableau.
        /// </summary>
        public void checkRankingTEAM()
        {
            int rank1, rank2;
            for (int i = 0; i < amountOfTeams(); i++)
            {
                rank1 = team[i].PhaseInitialRanking;
                for (int j = i + 1; j < amountOfTeams(); j++)
                {
                    rank2 = team[j].PhaseInitialRanking;
                    if (rank1 == rank2)
                        correctRankingIND(rank1);
                }
            }
        }

        private void correctRankingTeam(int rank)
        {
            FRCTeam team1, team2;
            FRCMatch match;
            int c = 0;

            for (int i = 0; i < amountOfTableauParts(); i++)
            {
                calculateTableauNumbers(tableauPart[i].Length);

                for (int j = 0; j < tableauPart[i].amountOfMatches(); j++)
                {
                    match = tableauPart[i].getNextMatch();
                    team1 = getTeamMatchingID(match.FencerID1);
                    team2 = getTeamMatchingID(match.FencerID2);

                    //This skip is needed if the xml file is extended format.
                    if (team1 == null || team2 == null)
                        continue;

                    if ((team1.PhaseInitialRanking == rank) && (c != 2))
                    {
                        int idx = (int)((match.MatchID - 1) * 2);
                        team1.PhaseInitialRanking = tableauNumber[idx];
                        c++;
                    }

                    if ((team2.PhaseInitialRanking == rank) && (c != 2))
                    {
                        int idx = (int)((match.MatchID - 1) * 2) + 1;
                        team2.PhaseInitialRanking = tableauNumber[idx];
                        c++;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the fencer that corresponds to given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private FRCFencer getFencerMatchingID(int id)
        {
            for (int i = 0; i < fencer.Count; i++)
            {
                if (fencer[i].ID == id)
                    return fencer[i];
            }

            return null;
        }

        /// <summary>
        /// Returns the team that corresponds to given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private FRCTeam getTeamMatchingID(int id)
        {
            for (int i = 0; i < team.Count; i++)
            {
                if (team[i].ID == id)
                    return team[i];
            }

            return null;
        }

        /// <summary>
        /// Sorts all fencers in this tableau after their initial rankings. Lowest number first.
        /// </summary>
        public void sortFencerInitialRanking()
        {
            for (int i = 0; i < fencer.Count - 1; i++)
                for (int j = i + 1; j < fencer.Count; j++)
                {
                    if (fencer[i].PhaseInitialRanking > fencer[j].PhaseInitialRanking)
                    {
                        FRCFencer tempFencer = fencer[j];
                        fencer[j] = fencer[i];
                        fencer[i] = tempFencer;
                    }
                }
        }

        /// <summary>
        /// Sorts all teams in this tableau after their initial rankings. Lowest number first.
        /// </summary>
        public void sortTeamInitialRanking()
        {
            for (int i = 0; i < team.Count - 1; i++)
                for (int j = i + 1; j < team.Count; j++)
                {
                    if (team[i].PhaseInitialRanking > team[j].PhaseInitialRanking)
                    {
                        FRCTeam tempTeam = team[j];
                        team[j] = team[i];
                        team[i] = tempTeam;
                    }
                }
        }
        
        /// <summary>
        /// Returns tableau number at given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int getTableauNumber(int index)
        {
            return tableauNumber[index];
        }

        /// <summary>
        /// Calculates the order of rank to print on tableau with given size.
        /// </summary>
        /// <param name="tableauSize"></param>
        public void calculateTableauNumbers(int tableauSize)
        {
            tableauNumber.Clear();
            tableauNumber.Add(1);
            tableauNumber.Add(2);
            calculateTableauNumbers2(tableauSize);
        }

        /// <summary>
        /// Calculates the order of rank to print on tableau with given size.
        /// tableauNumber must be added with 1 and 2 before calling this method.
        /// </summary>
        /// <param name="tableauSize"></param>
        private void calculateTableauNumbers2(int tableauSize)
        {
            if (tableauNumber.Count == tableauSize)
                return;

            if (tableauSize > 4)
                calculateTableauNumbers(tableauSize / 2);

            bool b = true;
            for (int i = 0; i < tableauSize; i += 2)
            {
                if (b)
                {
                    tableauNumber.Insert(i + 1, tableauSize + 1 - tableauNumber[i]);
                    b = false;
                }
                else
                {
                    tableauNumber.Insert(i, tableauSize + 1 - tableauNumber[i]);
                    b = true;
                }
            }
        }
    }
}
