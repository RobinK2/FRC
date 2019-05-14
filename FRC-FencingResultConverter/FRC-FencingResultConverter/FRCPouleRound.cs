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
    public class FRCPouleRound
    {
        //The round of the poule.
        private int roundNumber;
        //The amount of poules.
        private int numOfPoules;
        //The amount of fencer qualifying from this round of poule.
        private int numOfQualifiers;

        //List of all fencers during this poule round.
        private List<FRCFencer> fencer = new List<FRCFencer>();
        //List of all teams during this poule round.
        private List<FRCTeam> team = new List<FRCTeam>();
        //List of all poules during this poule round.
        private List<FRCPoule> poule = new List<FRCPoule>();
        //Index for list of fencers.
        private int fencerIdx = -1;
        //Index for list of teams.
        private int teamIdx = -1;
        //Index for list of poules.
        private int pouleIdx = -1;

        public FRCPouleRound()
        {
        }

        public FRCPouleRound(int roundNumber, int numOfPoules, int numOfQualifiers)
        {
            this.roundNumber = roundNumber;
            this.numOfPoules = numOfPoules;
            this.numOfQualifiers = numOfQualifiers;
        }

        /// <summary>
        /// Add a poule to the poule round.
        /// </summary>
        /// <param name="poule"></param>
        public void addPoule(FRCPoule poule)
        {
            this.poule.Add(poule);
        }

        /// <summary>
        /// Returns poule at current index.
        /// </summary>
        /// <returns></returns>
        public FRCPoule getCurrentPoule()
        {
            if (pouleIdx >= poule.Count)
                return null;

            return poule[pouleIdx];
        }

        /// <summary>
        /// Returns poule at next index. Starting at the first poule in the list.
        /// </summary>
        /// <returns></returns>
        public FRCPoule getNextPoule()
        {
            pouleIdx++;
            
            if (pouleIdx >= poule.Count)
                pouleIdx = 0;

            if (pouleIdx < poule.Count)
                return poule[pouleIdx];
            
            return null;
        }

        /// <summary>
        /// Returns amount of poules.
        /// </summary>
        /// <returns></returns>
        public int amountOfPoules()
        {
            return poule.Count;
        }

        /// <summary>
        /// Returns true if the list of poules has next element.
        /// </summary>
        /// <returns></returns>
        public bool hasNextPoule()
        {
            if (pouleIdx + 1 < poule.Count)
                return true;
            else
            {
                pouleIdx = -1;
                return false;
            }
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
        /// Sorts team after their ranking after this poule round (PhaseFinalRanking). Lowest number first.
        /// </summary>
        public void sortTeamPouleRanking()
        {
            for (int i = 0; i < team.Count - 1; i++)
                for (int j = i + 1; j < team.Count; j++)
                {
                    if (team[i].PhaseFinalRanking > team[j].PhaseFinalRanking)
                    {
                        FRCTeam tempteam = team[j];
                        team[j] = team[i];
                        team[i] = tempteam;
                    }
                }
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
        /// Resets teamIdx.
        /// </summary>
        public void resetTeamIndex()
        {
            teamIdx = -1;
        }

         /// <summary>
        /// Add a fencer to the poule round.
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
        /// Copies poule result information from given fencer to fencer in this poule round.
        /// </summary>
        public void CopyPouleResult(FRCFencer fencer)
        {
            for (int i = 0; i < this.fencer.Count; i++)
            {
                if (fencer.ID == this.fencer[i].ID)
                {
                    this.fencer[i].HitsGivenInPoule = fencer.HitsGivenInPoule;
                    this.fencer[i].HitsTakenInPoule = fencer.HitsTakenInPoule;
                    this.fencer[i].NumberOfMatchesInPoule = fencer.NumberOfMatchesInPoule;
                    this.fencer[i].NumberOfVictoriesInPoule = fencer.NumberOfVictoriesInPoule;
                    this.fencer[i].VM = fencer.VM;
                    this.fencer[i].Forfait = fencer.Forfait;
                    this.fencer[i].Abandoned = fencer.Abandoned;
                    this.fencer[i].Excluded = fencer.Excluded;
                    this.fencer[i].VM_String = fencer.VM_String;
                    break;
                }
            }
        }
        

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
        /// Calculates PhaseFinalRanking for all fencer in this poule. (This function adjusts PhaseFinalRanking to correct value)
        /// Needed if the format is QualifyLastRankAll.
        /// Since EnGarde changed the meaning of PhaseFinalRanking, it is good to use this function always in anyway. (In fact used always now)
        /// </summary>
        public void calculateFencerPouleRanking()
        {
            //Save excluded, scratched and abandoned fencers in this list.
            List<FRCFencer> excScrAbbFencers = new List<FRCFencer>();
            double highestVM;
            int highestIND, highestHGiven;
            int rank = 1;
            //Does not use the element in these sorted list at all actually, the key is the only important part since it becomes sorted.
            SortedList<double, FRCFencer> vmList = new SortedList<double, FRCFencer>();
            SortedList<int, FRCFencer> indList = new SortedList<int, FRCFencer>();
            SortedList<int, FRCFencer> hGivenList = new SortedList<int, FRCFencer>();
            List<FRCFencer> tempFencer = new List<FRCFencer>();
            for (int i = 0; i < fencer.Count; i++)
            {
                tempFencer.Add((FRCFencer) fencer[i].Clone());
            }
            while (tempFencer.Count > 0)
            {
                //Counts fencers with same ranking.
                int sameRankCounter = 0;

                //Searches for highest VM.
                for (int i = 0; i < tempFencer.Count; i++)
                {
                    tempFencer[i].VM = (double)tempFencer[i].NumberOfVictoriesInPoule /
                        (double)tempFencer[i].NumberOfMatchesInPoule;

                    //When the XML-file is weird 0/0 occurs.
                    if (double.IsNaN(tempFencer[i].VM))
                        tempFencer[i].VM = 0.0;

                    vmList[tempFencer[i].VM] = (FRCFencer)tempFencer[i].Clone();
                }
                highestVM = vmList.Keys[vmList.Count - 1];

                //Searches for highest index.
                for (int i = 0; i < tempFencer.Count; i++)
                {
                    if (tempFencer[i].VM == highestVM)
                    {
                        tempFencer[i].Index = tempFencer[i].HitsGivenInPoule -
                            tempFencer[i].HitsTakenInPoule;

                        indList[tempFencer[i].Index] = (FRCFencer)tempFencer[i].Clone();
                    }
                }
                highestIND = indList.Keys[indList.Count - 1];

                //Searches for highest given hits.
                for (int i = 0; i < tempFencer.Count; i++)
                {
                    if (tempFencer[i].VM == highestVM)
                        if (tempFencer[i].Index == highestIND)
                        {
                            hGivenList[tempFencer[i].HitsGivenInPoule] = (FRCFencer)tempFencer[i].Clone();
                        }
                }
                highestHGiven = hGivenList.Keys[hGivenList.Count - 1];

                for (int i = 0; i < tempFencer.Count; i++)
                {
                    if (tempFencer[i].VM == highestVM)
                        if (tempFencer[i].Index == highestIND)
                            if (tempFencer[i].HitsGivenInPoule == highestHGiven)
                            {
                                //The abandoned, excluded and scratched fencers must be added at the end of the list.
                                if (tempFencer[i].Abandoned || tempFencer[i].Excluded ||
                                    tempFencer[i].Forfait)
                                {
                                    excScrAbbFencers.Add(tempFencer[i]);
                                    tempFencer.RemoveAt(i);
                                }
                                else
                                {
                                    //tempFencer[i].PhaseFinalRanking = rank;
                                    for (int j = 0; j < fencer.Count; j++)
                                    {
                                        if (fencer[j].ID == tempFencer[i].ID) {
                                            fencer[j].PhaseFinalRanking = rank;
                                            break;
                                        }
                                    }
                                    tempFencer.RemoveAt(i);
                                    sameRankCounter++;
                                }                          
                            }
                }
                vmList.Clear();
                indList.Clear();
                hGivenList.Clear();
                rank += sameRankCounter;
            }

            //Add all abandoned, scratched and excluded fencers at the end.
            for (int i = 0; i < excScrAbbFencers.Count; i++)
            {
                //excScrAbbFencers[i].PhaseFinalRanking = rank;
                for (int j = 0; j < fencer.Count; j++)
                {
                    if (fencer[j].ID == excScrAbbFencers[i].ID){
                        fencer[j].PhaseFinalRanking = rank;
                        break;
                    }
                }
                rank++;
            }
            excScrAbbFencers.Clear();
            tempFencer.Clear();
        }

        /// <summary>
        /// Sorts fencer after their ranking after this poule round (PhaseFinalRanking). Lowest number first.
        /// </summary>
        public void sortFencerPouleRanking()
        {
            for (int i = 0; i < fencer.Count - 1; i++)
                for (int j = i + 1; j < fencer.Count; j++)
                {
                    if (fencer[i].PhaseFinalRanking > fencer[j].PhaseFinalRanking)
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
        /// Resets fencerIdx.
        /// </summary>
        public void resetFencerIndex()
        {
            fencerIdx = -1;
        }

        /// <summary>
        /// Property for the round number.
        /// </summary>
        public int RoundNumber
        {
            set { roundNumber = value; }
            get { return roundNumber; }
        }

        /// <summary>
        /// Property for the amount of poules during the round.
        /// </summary>
        public int NumOfPoules
        {
            set { numOfPoules = value; }
            get { return numOfPoules; }
        }

        /// <summary>
        /// Property for amount fencers qualifying after this round.
        /// </summary>
        public int NumOfQualifiers
        {
            set { numOfQualifiers = value; }
            get { return numOfQualifiers; }
        }
    }
}
