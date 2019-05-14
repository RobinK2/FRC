using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRC_FencingResultConverter
{
    public class FRCCompetitor
    {
        //The ID number of the fencer in the xml file.
        private int id;
        //The nationality.
        private string nationality;
        //The club.
        private string club;
        //The final ranking of the fencer.
        private int finalRanking;
        //The ranking during the start of the phase.
        private int phaseIntialRanking;
        //The ranking during the end of the phase.
        private int phaseFinalRanking;
        //True if the fencer is not yet eliminated during the phase.
        private bool isStillInTheCompetiton = true;
        //True if the fencer abandoned the competition.
        private bool abandoned = false;
        //True if the fencer got forfait.
        private bool forfait = false;
        //True if the fencer got exclusion.
        private bool excluded = false;
        //True if the competitor is in the results from the poule phase, but has no status.
        private bool noStatusException = true;
        //The ranking before competition starts.
        private int initialRanking = -1;

        //The fencer number in the poule.
        private int fencerNumberInPoule;
        //Number of victories in the poule.
        private int numOfVictoriesInPoule = 0;
        //Number of matches in the poule.
        private int numOfMatchesInPoule = 0;
        //Hits given in the poule.
        private int hitsGivenInPoule = 0;
        //Hits taken in the poule.
        private int hitsTakenInPoule = 0;
        //Final ranking in the poule.
        private int pouleRanking;
        private string vmString;
        private double vm;
        private int index;

        public FRCCompetitor()
        {
        }

        /// <summary>
        /// Property for reference ID number. 
        /// </summary>
        public int ID
        {
            set { id = value; }
            get { return id; }
        }

        /// <summary>
        /// Property for nationality.
        /// </summary>
        public string Nationality
        {
            set { nationality = value; }
            get { return nationality; }
        }

        /// <summary>
        /// Property for club.
        /// </summary>
        public string Club
        {
            set { club = value; }
            get { return club; }
        }

        /// <summary>
        /// Property for the final result.
        /// </summary>
        public int FinalRanking
        {
            set { finalRanking = value; }
            get { return finalRanking; }
        }

        /// <summary>
        /// Property for the initial ranking before competition.
        /// </summary>
        public int InitialRanking
        {
            set { initialRanking = value; }
            get { return initialRanking; }
        }

        /// <summary>
        /// Property for the ranking durig the start of the phase.
        /// </summary>
        public int PhaseInitialRanking
        {
            set { phaseIntialRanking = value; }
            get { return phaseIntialRanking; }
        }

        /// <summary>
        /// Property for the ranking during the end of the phase.
        /// </summary>
        public int PhaseFinalRanking
        {
            set { phaseFinalRanking = value; }
            get { return phaseFinalRanking; }
        }

        /// <summary>
        /// Property for whether the fencer is eliminated or not. False if eliminated.
        /// </summary>
        public bool IsStillInTheCompetition
        {
            set { isStillInTheCompetiton = value; }
            get { return isStillInTheCompetiton; }
        }

        /// <summary>
        /// Property for the fencer number in the poule.
        /// </summary>
        public int FencerNumberInPoule
        {
            set { fencerNumberInPoule = value; }
            get { return fencerNumberInPoule; }
        }

        /// <summary>
        /// Property for amount of victories in the poule.
        /// </summary>
        public int NumberOfVictoriesInPoule
        {
            set { numOfVictoriesInPoule = value; }
            get { return numOfVictoriesInPoule; }
        }

        /// <summary>
        /// Property for amount of matches in the poule.
        /// </summary>
        public int NumberOfMatchesInPoule
        {
            set { numOfMatchesInPoule = value; }
            get { return numOfMatchesInPoule; }
        }

        /// <summary>
        /// Property for amount of given hits in the poule.
        /// </summary>
        public int HitsGivenInPoule
        {
            set { hitsGivenInPoule = value; }
            get { return hitsGivenInPoule; }
        }

        /// <summary>
        /// Property for amout of taken hits in the poule.
        /// </summary>
        public int HitsTakenInPoule
        {
            set { hitsTakenInPoule = value; }
            get { return hitsTakenInPoule; }
        }

        /// <summary>
        /// Property for the final ranking in the poule.
        /// </summary>
        public int PouleRanking
        {
            set { pouleRanking = value; }
            get { return pouleRanking; }
        }

        /// <summary>
        /// Property for V/M in string format.
        /// </summary>
        public string VM_String
        {
            set { vmString = value; }
            get { return vmString; }
        }

        /// <summary>
        /// Property for V/M.
        /// </summary>
        public double VM
        {
            set { vm = value; }
            get { return vm; }
        }

        /// <summary>
        /// Property for index.
        /// </summary>
        public int Index
        {
            set { index = value; }
            get { return index; }
        }

        /// <summary>
        /// Property for whether the fencer has abandoned the competition or not.
        /// </summary>
        public bool Abandoned
        {
            set { abandoned = value; }
            get { return abandoned; }
        }

        /// <summary>
        /// Property for whether fencer got forfait or not.
        /// </summary>
        public bool Forfait
        {
            set { forfait = value; }
            get { return forfait; }
        }

        /// <summary>
        /// Property for whether fencer got exclusion of not.
        /// </summary>
        public bool Excluded
        {
            set { excluded = value; }
            get { return excluded; }
        }

        /// <summary>
        /// Property which is true if the competitor is in results from the first poule phase,
        /// but has no status.
        /// </summary>
        public bool NoStatusException
        {
            set { noStatusException = value; }
            get { return noStatusException; }
        }
    }
}
