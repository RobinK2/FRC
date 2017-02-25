using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRC_FencingResultConverter
{
    public class FRCTeam : FRCCompetitor, ICloneable
    {
        //The name of the team.
        private string name;
        //List of fencers in the team.
        private List<FRCFencer> fencer = new List<FRCFencer>();
        //Index for list of fencers.
        private int fencerIdx = -1;
                
        public FRCTeam()
        {
        }

        /// <summary>
        /// Returns a clone of this team.
        /// </summary>
        /// <returns></returns>
        public Object Clone()
        {
            FRCTeam cTeam = new FRCTeam();
            cTeam.Abandoned = this.Abandoned;
            cTeam.Club = this.Club;
            cTeam.Excluded = this.Excluded;
            cTeam.ID = this.ID;
            cTeam.FencerNumberInPoule = this.FencerNumberInPoule;
            cTeam.FinalRanking = this.FinalRanking;
            cTeam.Name = this.Name;
            cTeam.Forfait = this.Forfait;
            cTeam.HitsGivenInPoule = this.HitsGivenInPoule;
            cTeam.HitsTakenInPoule = this.HitsTakenInPoule;
            cTeam.InitialRanking = this.InitialRanking;
            cTeam.IsStillInTheCompetition = this.IsStillInTheCompetition;
            cTeam.Nationality = this.Nationality;
            cTeam.NumberOfMatchesInPoule = this.NumberOfMatchesInPoule;
            cTeam.NumberOfVictoriesInPoule = this.NumberOfVictoriesInPoule;
            cTeam.PhaseFinalRanking = this.PhaseFinalRanking;
            cTeam.PhaseInitialRanking = this.PhaseInitialRanking;
            cTeam.PouleRanking = this.PouleRanking;
            cTeam.VM_String = this.VM_String;

            return cTeam;
        }

        /// <summary>
        /// Add a fencer to the poule.
        /// </summary>
        /// <param name="fenc"></param>
        public void addFencer(FRCFencer fencer)
        {
            this.fencer.Add(fencer);
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
        /// Property for team name.
        /// </summary>
        public string Name
        {
            set { name = value; }
            get { return name; }
        }
    }
}
