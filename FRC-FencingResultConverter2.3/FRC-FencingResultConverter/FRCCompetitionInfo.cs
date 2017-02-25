using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRC_FencingResultConverter
{
    public class FRCCompetitionInfo
    {
        //The name of the competition.
        private string competitionName;
        //Individual competition if true, team competition if false.
        private bool individualCompetitionType;
        //Amout of fencers.
        private int numOfFencers;
        //Amount of referees.
        private int numOfReferees;
        //Number of rounds of poules.
        private int roundsOfPoules;
        //Number of poules per round.
        private int[] numOfPoules;
        //Amount of fencers in the poule. First index is the poule round. Second index is poule number.
        private int[,] numOfFencersInPoule;
        //Number of qualifiers per poule round. Index is the poule round.
        private int[] numOfQualifiers;
        //Number of fencer on the direct elimination.
        private int numOfElimFencers;

        public FRCCompetitionInfo()
        {
        }

        public FRCCompetitionInfo(string competitionName, int numOfFencers, int numOfReferees, int roundsOfPoules)
        {
            this.competitionName = competitionName;
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

        /// <summary>
        /// Property for the competition type. Individual competiton if true, team competitionn otherwise.
        /// </summary>
        public bool IsIndiviualCompetiton
        {
            set { individualCompetitionType = value; }
            get { return individualCompetitionType; }
        }

        /// <summary>
        /// Property for the amount of fencers.
        /// </summary>
        public int NumberOfFencers
        {
            set { numOfFencers = value; }
            get { return numOfFencers; }
        }

        /// <summary>
        /// Property for the amount of referees.
        /// </summary>
        public int NumberOfReferees
        {
            set { numOfReferees = value; }
            get { return numOfReferees; }
        }

        /// <summary>
        /// Property for the rounds of poules.
        /// </summary>
        public int RoundsOfPoules
        {
            set 
            {
                roundsOfPoules = value;
                numOfPoules = new int[roundsOfPoules];
                numOfQualifiers = new int[roundsOfPoules];
            }
            get { return roundsOfPoules; }
        }

        /// <summary>
        /// Property for the amount of fencers participating in the direct elimination.
        /// </summary>
        public int NumberOfFencersInElimination
        {
            set { numOfElimFencers = value; }
            get { return numOfElimFencers; }
        }

        /// <summary>
        /// Sets the number of poules at given poule round. numOfPoules must have been initiated before calling this method.
        /// </summary>
        /// <param name="index"> The poule round </param>
        /// <param name="numberOfPoules"> Value to assign </param>
        public void setNumOfPoules(int index, int numberOfPoules)
        {
            numOfPoules[index] = numberOfPoules;
        }

        /// <summary>
        /// Gets the number of poules at given poule round.
        /// </summary>
        /// <param name="index"> The poule round </param>
        /// <returns></returns>
        public int getNumOfPoules(int index)
        {
            return numOfPoules[index];
        }

        /// <summary>
        /// This method should be called before setNumOfFencersInPoule() and after all rounds of poules and number of poules are given.
        /// </summary>
        public void initializeNumOfFencersInPoule()
        {
            numOfFencersInPoule = new int[roundsOfPoules, numOfPoules.Max()];
        }

        /// <summary>
        /// Sets the amount of fencers at given poule.
        /// </summary>
        /// <param name="pouleRound"> The poule round. </param>
        /// <param name="pouleNumber"> The poule number. </param>
        /// <param name="numberOfFencers"> The value to be assigned. </param>
        public void setNumOfFencersInPoule(int pouleRound, int pouleNumber, int numberOfFencers)
        {
            numOfFencersInPoule[pouleRound, pouleNumber] = numberOfFencers;
        }

        /// <summary>
        /// Gets the amount of fencers at given poule.
        /// </summary>
        /// <param name="pouleRound"> The poule round. </param>
        /// <param name="pouleNumber"> The poule number. </param>
        /// <returns></returns>
        public int getNumOfFencerInPoule(int pouleRound, int pouleNumber)
        {
            return numOfFencersInPoule[pouleRound, pouleNumber];
        }

        /// <summary>
        /// Sets the amount of qualified fencers from given poule round. 
        /// </summary>
        /// <param name="index"> The poule round. </param>
        /// <param name="numberOfQualifiers"> The value to be assigned. </param>
        public void setNumOfQualifiers(int index, int numberOfQualifiers)
        {
            numOfQualifiers[index] = numberOfQualifiers;
        }

        /// <summary>
        /// Gets the amount of qualified fencers from given poule round.
        /// </summary>
        /// <param name="index"> The poule round. </param>
        /// <returns></returns>
        public int getNumOfQualifiers(int index)
        {
            return numOfQualifiers[index];
        }
    }
}
