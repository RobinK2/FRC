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
using System.Xml;

namespace FRC_FencingResultConverter
{
    public class FRCXmlInterpreter
    {
        //Individual competition if true, team competition if false.
        private bool isIndividualCompetition;
        //List of fencers.
        private List<FRCFencer> fencer = new List<FRCFencer>();
        //List of referees.
        private List<FRCReferee> referee = new List<FRCReferee>();
        //List of teams.
        private List<FRCTeam> team = new List<FRCTeam>();
        //List of poule rounds.
        private List<FRCPouleRound> pouleRound = new List<FRCPouleRound>();
        //The formual of the competition.
        private FRCCompetitionFormula formula;
        //List of tables of the competition.
        private List<FRCTableau> tableau = new List<FRCTableau>();
        //The xml reader.
        private XmlTextReader reader;
        //Check whether the element Tireurs has ended or not. True if the element has ended.
        private bool endOfTireurs = false;
        //Check whether the element Arbitres has ended or not. True if the element has ended.
        private bool endOfArbitres = false;
        //Check whether the element Equipes has ended or not. True if the element has ended.
        private bool endOfEquipes = false;
        //The name of the competition.
        private string competitionTitle; 
        //The short format of competition name.
        private string competitionTitleShort;
        //True if the reader is at the intro of the poule round.
        private bool pouleRoundIntro = false;
        //True if the reader is at poule phase.
        private bool poulePhase = false;
        //True if the reader is at tableau phase.
        private bool tableauPhase = false;
        //True if the reader is at a match.
        private bool inMatch = false;
        //True if the focus is on fencer1, false if the focus is on fencer2.
        private bool focusOnFencer1 = false;
        private bool onlyOnce = true;
        
        //Constants for all elements in the xml file.
        private const string TIREURS = "Tireurs";
        private const string COMP_INDIVID = "CompetitionIndividuelle";
        private const string BASE_COMP_INDIVID = "BaseCompetitionIndividuelle";
        private const string COMP_PAR = "CompetitionParEquipes";
        private const string BASE_COMP_PAR = "BaseCompetitionParEquipes";
        private const string TIREUR = "Tireur";
        private const string ARBITRES = "Arbitres";
        private const string ARBITRE = "Arbitre";
        private const string TOUR_DE_POULES = "TourDePoules";
        private const string POULE = "Poule";
        private const string MATCH = "Match";
        private const string PHASE_DE_TABLEAUX = "PhaseDeTableaux";
        private const string TABLEAU = "Tableau";
        private const string EQUIPES = "Equipes";
        private const string EQUIPE = "Equipe";
        private const string SUITE_DE_TABLEAUX = "SuiteDeTableaux";

        //Constants for all attributes in the xml file.
        private const string TITRE_LONG = "TitreLong";
        private const string TITRE_COURT = "TitreCourt";
        private const string ID = "ID";
        private const string NOM = "Nom";
        private const string PRENOM = "Prenom";
        private const string NATION = "Nation";
        private const string CATEGORIE = "Categorie";
        private const string CLUB = "Club";
        private const string CLASSEMENT = "Classement";
        private const string REF = "REF";
        private const string NB_DE_POULES = "NbDePoules";
        private const string NB_QUALIFIES_PAR_INDICE = "NbQualifiesParIndice";
        private const string RANG_INITIAL = "RangInitial";
        private const string RANG_FINAL = "RangFinal";
        private const string STATUT = "Statut";
        private const string PISTE = "Piste";
        private const string DATE = "Date";
        private const string NO_DANS_LA_POULE = "NoDansLaPoule";
        private const string NB_VICTOIRES = "NbVictoires";
        private const string NB_MATCHES = "NbMatches";
        private const string TD = "TD";
        private const string TR = "TR";
        private const string RANG_POULE = "RangPoule";
        private const string SCORE = "Score";
        private const string TITRE = "Titre";
        private const string TAILLE = "Taille";
        private const string PLACE = "Place";


        public FRCXmlInterpreter(string source)
        {
            reader = new XmlTextReader(source);
            tableau.Add(new FRCTableau());
        }

        /// <summary>
        /// Returns a list of all fencers in the competition.
        /// </summary>
        /// <returns></returns>
        public List<FRCFencer> getFencerList()
        {
            return fencer;
        }

        /// <summary>
        /// Returns a list of all referees in the competition.
        /// </summary>
        /// <returns></returns>
        public List<FRCReferee> getRefereeList()
        {
            return referee;
        }

        /// <summary>
        /// Returns a list of all teams in the competition.
        /// </summary>
        /// <returns></returns>
        public List<FRCTeam> getTeamList()
        {
            return team;
        }

        /// <summary>
        /// Returns a list of all poules rounds in the competition.
        /// </summary>
        /// <returns></returns>
        public List<FRCPouleRound> getPouleRoundList()
        {
            return pouleRound;
        }

        /// <summary>
        /// Read only property for the competition formula.
        /// </summary>
        public FRCCompetitionFormula Formula
        {
            get { return formula; }
        }

        /// <summary>
        /// Read only property for the main tableau in the competition.
        /// </summary>
        public FRCTableau Tableau
        {
            get { return tableau[0]; }
        }

        /// <summary>
        /// Returns list of all tableaus.
        /// </summary>
        /// <returns></returns>
        public List<FRCTableau> getTableauList()
        {
            return tableau;
        }

        /// <summary>
        /// Reads and interprets all contents of the xml file.
        /// </summary>
        public void interpretXml()
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        interpretElement();
                        //Console.Write("<" + reader.Name);
                        //Console.WriteLine(">");
                        break;
                    case XmlNodeType.EndElement: //Display the end of the element.
                        endOfElementHandler();
                        //Console.Write("</" + reader.Name);
                        //Console.WriteLine(">");
                        break;
                }
            }

            interpretCompetitionFormula();
        }

        /// <summary>
        /// Switches focus between fencer 1 or 2.
        /// </summary>
        private void switchFencer()
        {
            if (focusOnFencer1)
                focusOnFencer1 = false;
            else
                focusOnFencer1 = true;
        }

        /// <summary>
        /// Interprets information about competition formula from the xml file.
        /// </summary>
        private void interpretCompetitionFormula()
        {
            formula = new FRCCompetitionFormula(competitionTitle, competitionTitleShort, fencer.Count, referee.Count, pouleRound.Count);
            formula.IsIndiviualCompetiton = isIndividualCompetition;
        }

        /// <summary>
        /// Interprets all elements of the xml file.
        /// </summary>
        private void interpretElement()
        {
            interpretCompetitionType();
            interpretPoule();
            interpretPouleRound();
            //If the competition is individual.
            if (isIndividualCompetition)
                interpretFencers();
            else //If the competition is team.
                interpretEquipe();
            interpretReferees();
            interpretTableau();
        }

        /// <summary>
        /// Does necessary arrangements when elements ends.
        /// </summary>
        private void endOfElementHandler()
        {
            if (reader.Name == TIREURS)
                endOfTireurs = true;

            if (reader.Name == ARBITRES)
                endOfArbitres = true;

            if (reader.Name == EQUIPES)
                endOfEquipes = true;

            if (reader.Name == TOUR_DE_POULES)
                poulePhase = false;

            if (reader.Name == MATCH)
                inMatch = false;

            if (reader.Name == PHASE_DE_TABLEAUX)
                tableauPhase = false;
        }

        /// <summary>
        /// Interprets info of all teams.
        /// </summary>
        private void interpretEquipe()
        {
            if (!endOfEquipes)
            {
                if (reader.Name == EQUIPE)
                {
                    FRCTeam tempTeam = new FRCTeam();
                    while (reader.MoveToNextAttribute())
                        if (reader.Name == ID)
                            tempTeam.ID = int.Parse(reader.Value);
                        else if (reader.Name == NOM)
                            tempTeam.Name = reader.Value;
                        else if (reader.Name == NATION)
                            tempTeam.Nationality = reader.Value;
                        else if (reader.Name == CLUB)
                            tempTeam.Club = reader.Value;
                        else if (reader.Name == CLASSEMENT)
                            tempTeam.FinalRanking = int.Parse(reader.Value);

                    team.Add(tempTeam);
                }
                else if (reader.Name == TIREUR)
                {
                    FRCFencer tempFencer = new FRCFencer();
                    while (reader.MoveToNextAttribute())
                        if (reader.Name == ID)
                            tempFencer.ID = int.Parse(reader.Value);
                        else if (reader.Name == NOM)
                            tempFencer.LastName = reader.Value;
                        else if (reader.Name == PRENOM)
                            tempFencer.FirstName = reader.Value;
                        else if (reader.Name == NATION)
                            tempFencer.Nationality = reader.Value;
                        else if (reader.Name == CLUB)
                            tempFencer.Club = reader.Value;

                    team[team.Count-1].addFencer(tempFencer);
                }
            }
        }

        /// <summary>
        /// Interprets info about the tableau from the xml file.
        /// </summary>
        private void interpretTableau()
        {
            if (tableauPhase)
            {
                if (!inMatch)
                {
                    if ((reader.Name == TIREUR && isIndividualCompetition) || (reader.Name == EQUIPE && !isIndividualCompetition))
                        interpretTireurOrEquipeInTableau();
                    else if (reader.Name == SUITE_DE_TABLEAUX)
                    {
                        //Skip this add-method only once.
                        if (!onlyOnce)
                            tableau.Add(new FRCTableau());
                        else
                            onlyOnce = false;
                    }
                    else if (reader.Name == TABLEAU)
                        interpretLengthInTableau();
                    else if (reader.Name == MATCH)
                    {
                        inMatch = true;
                        FRCMatch match = new FRCMatch();
                        while (reader.MoveToNextAttribute())
                            if (reader.Name == ID)
                                match.MatchID = int.Parse(reader.Value);

                        tableau[tableau.Count - 1].getCurrentTableauPart().addMatch(match);
                        //Just to increment index of list in FRCTableauPart.
                        tableau[tableau.Count - 1].getCurrentTableauPart().getNextMatch();
                    }
                }
                else
                    interpretMatchInTableau();                
            }

            if (reader.Name == PHASE_DE_TABLEAUX)
                tableauPhase = true;
        }

        /// <summary>
        /// A method to be called from interpretTableau().
        /// </summary>
        private void interpretMatchInTableau()
        {
            if (reader.Name == ARBITRE)
            {
                int id = -1;
                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name == REF)
                        id = int.Parse(reader.Value);
                }

                for (int i = 0; i < referee.Count; i++)
                {
                    if (id == referee[i].RefereeID)
                    {
                        if (tableau[tableau.Count - 1].getCurrentTableauPart().Length == 8)
                            referee[i].RefereedQuarterFinals++;
                        else if (tableau[tableau.Count - 1].getCurrentTableauPart().Length <= 4)
                            referee[i].RefereedFinals++;
                        else
                            referee[i].RefereedMatches++;

                        break;
                    }
                }
            }
            else if ((reader.Name == TIREUR && isIndividualCompetition) || (reader.Name == EQUIPE && !isIndividualCompetition))
            {
                FRCMatch currentMatch = tableau[tableau.Count-1].getCurrentTableauPart().getCurrentMatch();
                switchFencer();
                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name == REF)
                        if (focusOnFencer1)
                            currentMatch.FencerID1 = int.Parse(reader.Value);
                        else
                            currentMatch.FencerID2 = int.Parse(reader.Value);
                    else if (reader.Name == SCORE)
                        if (focusOnFencer1)
                            currentMatch.ScoreFencer1 = int.Parse(reader.Value);
                        else
                            currentMatch.ScoreFencer2 = int.Parse(reader.Value);
                    else if (reader.Name == PLACE)
                        if (focusOnFencer1)
                            currentMatch.TableauRankFencer1 = int.Parse(reader.Value);
                        else
                            currentMatch.TableauRankFencer2 = int.Parse(reader.Value);
                    else if (reader.Name == STATUT)
                        if (focusOnFencer1)
                        {
                            if (reader.Value == "V")
                                currentMatch.Fencer1Win = true;
                            else if (reader.Value == "A")
                            {
                                currentMatch.Fencer1Abandon = true;
                                currentMatch.Fencer1Win = false;
                            }
                            else if (reader.Value == "F")
                            {
                                currentMatch.Fencer1Forfait = true;
                                currentMatch.Fencer1Win = false;
                            }
                            else if (reader.Value == "E")
                            {
                                currentMatch.Fencer1Exclusion = true;
                                currentMatch.Fencer1Win = false;
                            }
                            else if (reader.Value == "D")
                                currentMatch.Fencer1Win = false;
                        }
                        else
                        {
                            if (reader.Value == "V")
                                currentMatch.Fencer2Win = true;
                            else if (reader.Value == "A")
                            {
                                currentMatch.Fencer2Abandon = true;
                                currentMatch.Fencer2Win = false;
                            }
                            else if (reader.Value == "F")
                            {
                                currentMatch.Fencer2Forfait = true;
                                currentMatch.Fencer2Win = false;
                            }
                            else if (reader.Value == "E")
                            {
                                currentMatch.Fencer2Exclusion = true;
                                currentMatch.Fencer2Win = false;
                            }
                            else if (reader.Value == "D")
                                currentMatch.Fencer2Win = false;
                        }
                    //Console.Write(" " + reader.Name + "='" + reader.Value + "'");
                }
            }
        }

        /// <summary>
        /// A method to be called from interpretTableau().
        /// </summary>
        private void interpretLengthInTableau()
        {
            FRCTableauPart tPart = new FRCTableauPart();
            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == TITRE)
                    tPart.Title = reader.Value;
                else if (reader.Name == TAILLE)
                    tPart.Length = int.Parse(reader.Value);

                //Console.Write(" " + reader.Name + "='" + reader.Value + "'");
            }
            tableau[tableau.Count-1].addTableauPart(tPart);
            //Just to increment the index of list in tableau.
            tableau[tableau.Count - 1].getNextTableauPart();
        }

        /// <summary>
        /// A method to be called from interpretTableau().
        /// </summary>
        private void interpretTireurOrEquipeInTableau()
        {
            FRCCompetitor athlete;
            if (isIndividualCompetition)
                athlete = new FRCFencer();
            else
                athlete = new FRCTeam();

            int i = 0;
            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == REF)
                {
                    athlete.ID = int.Parse(reader.Value);
                    if (isIndividualCompetition) //Copying info from this.fencer if individual competition.
                    {
                        for (i = 0; i < this.fencer.Count; i++)
                            if (athlete.ID == this.fencer[i].ID)
                            {
                                athlete = (FRCFencer)this.fencer[i].Clone();
                                break;
                            }
                    }
                    else
                    {
                        for(i=0; i<this.team.Count; i++)
                            if (athlete.ID == this.team[i].ID)
                            {
                                athlete = (FRCTeam)this.team[i].Clone();
                                break;
                            }
                    }
                }
                else if (reader.Name == RANG_INITIAL)
                    athlete.PhaseInitialRanking = int.Parse(reader.Value);
                else if (reader.Name == RANG_FINAL)
                    athlete.PhaseFinalRanking = int.Parse(reader.Value);
                else if (reader.Name == STATUT)
                        if (reader.Value == "A")
                        {
                            athlete.IsStillInTheCompetition = false;
                            athlete.Abandoned = true;
                            if(isIndividualCompetition)
                                fencer[i].Abandoned = true;
                            else
                                team[i].Abandoned = true;
                        }
                        else if (reader.Value == "F")
                        {
                            athlete.IsStillInTheCompetition = false;
                            athlete.Forfait = true;
                            if(isIndividualCompetition)
                                fencer[i].Forfait = true; 
                            else
                                team[i].Forfait = true;
                        }
                        else if (reader.Value == "E")
                        {
                            athlete.IsStillInTheCompetition = false;
                            athlete.Excluded = true;
                            if(isIndividualCompetition)
                                fencer[i].Excluded = true;
                            else
                                team[i].Excluded = true;
                        }

                //Console.Write(" " + reader.Name + "='" + reader.Value + "'");
            }
            if (isIndividualCompetition)
                tableau[tableau.Count - 1].addFencer((FRCFencer)athlete);
            else
                tableau[tableau.Count - 1].addTeam((FRCTeam)athlete);
        }

        /// <summary>
        /// Interprets information of the poule from the xml file.
        /// This method must be called before interpretPoulRound() in the main loop.
        /// </summary>
        private void interpretPoule()
        {
            if (poulePhase)
            {
                if (reader.Name == POULE)
                {
                    pouleRoundIntro = false;
                    FRCPoule poule = new FRCPoule();
                    
                    while (reader.MoveToNextAttribute())
                    {
                        if (reader.Name == ID)
                            poule.PouleNumber = int.Parse(reader.Value);
                        else if (reader.Name == PISTE)
                            poule.PisteNumber = reader.Value;
                        else if (reader.Name == DATE)
                            poule.StartTime = reader.Value;

                        //Console.Write(" " + reader.Name + "='" + reader.Value + "'");
                    }
                    pouleRound[pouleRound.Count - 1].addPoule(poule);
                    pouleRound[pouleRound.Count - 1].getNextPoule();
                }

                if (!pouleRoundIntro)
                {
                    if (!inMatch)
                    {
                        if (reader.Name == TIREUR || reader.Name == EQUIPE)
                            interpretTireurOrEquipeInPoule();
                        else if (reader.Name == ARBITRE)
                            interpretArbitreInPoule();
                        else if (reader.Name == MATCH)
                        {
                            inMatch = true;
                            pouleRound[pouleRound.Count - 1].getCurrentPoule().addMatch(new FRCMatch());
                            pouleRound[pouleRound.Count - 1].getCurrentPoule().getNextMatch();
                        }
                    }
                    else
                    {
                        if (isIndividualCompetition)
                        {
                            if (reader.Name == TIREUR)
                                interpretMatchInPoule();
                        }
                        else
                        {
                            if (reader.Name == EQUIPE)
                                interpretMatchInPoule();
                        }                       
                    }
                }
            }
        }

        /// <summary>
        /// A method to be called from interpretPoule().
        /// </summary>
        private void interpretArbitreInPoule()
        {
            int id = -1;
            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == REF)
                    id = int.Parse(reader.Value);
            }
            
            for (int i = 0; i < referee.Count; i++)
                if (referee[i].RefereeID == id)
                {
                    referee[i].RefereedPoules++;
                    pouleRound[pouleRound.Count - 1].getCurrentPoule().addReferee(referee[i]);
                    break;
                }
        }

        /// <summary>
        /// A method to be called from interpretPoule().
        /// </summary>
        private void interpretMatchInPoule()
        {
            FRCMatch currentMatch = pouleRound[pouleRound.Count - 1].getCurrentPoule().getCurrentMatch();
            switchFencer();
            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == REF)
                    if (focusOnFencer1)
                        currentMatch.FencerID1 = int.Parse(reader.Value);
                    else
                        currentMatch.FencerID2 = int.Parse(reader.Value);
                else if (reader.Name == SCORE)
                    if (focusOnFencer1)
                        currentMatch.ScoreFencer1 = int.Parse(reader.Value);
                    else
                        currentMatch.ScoreFencer2 = int.Parse(reader.Value);
                else if (reader.Name == STATUT)
                    if (focusOnFencer1)
                    {
                        if (reader.Value == "V")
                            currentMatch.Fencer1Win = true;
                        else if (reader.Value == "A")
                        {
                            currentMatch.Fencer1Abandon = true;
                            currentMatch.Fencer1Win = false;
                        }
                        else if (reader.Value == "F")
                        {
                            currentMatch.Fencer1Forfait = true;
                            currentMatch.Fencer1Win = false;
                        }
                        else if (reader.Value == "E")
                        {
                            currentMatch.Fencer1Exclusion = true;
                            currentMatch.Fencer1Win = false;
                        }
                        else if (reader.Value == "D")
                            currentMatch.Fencer1Win = false;
                    }
                    else
                    {
                        if (reader.Value == "V")
                            currentMatch.Fencer2Win = true;
                        else if (reader.Value == "A")
                        {
                            currentMatch.Fencer2Abandon = true;
                            currentMatch.Fencer2Win = false;
                        }
                        else if (reader.Value == "F")
                        {
                            currentMatch.Fencer2Forfait = true;
                            currentMatch.Fencer2Win = false;
                        }
                        else if (reader.Value == "E")
                        {
                            currentMatch.Fencer2Exclusion = true;
                            currentMatch.Fencer2Win = false;
                        }
                        else if (reader.Value == "D")
                            currentMatch.Fencer2Win = false;
                    }

                //Console.Write(" " + reader.Name + "='" + reader.Value + "'");
            }
        }

        /// <summary>
        /// A method to be called from interpretPoule().
        /// </summary>
        private void interpretTireurOrEquipeInPoule()
        {
            FRCCompetitor athlete;
            if(isIndividualCompetition)
                athlete = new FRCFencer();
            else
                athlete = new FRCTeam();

            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == REF)
                {
                    athlete.ID = int.Parse(reader.Value);

                    if (isIndividualCompetition) //Copying info from this.fencer if individual competition
                    {
                        for (int i = 0; i < this.fencer.Count; i++)
                            if (athlete.ID == this.fencer[i].ID)
                            {
                                athlete = (FRCFencer)fencer[i].Clone();
                                break;
                            }
                    }
                    else //Copying info from this.team if team competition
                    {
                        for(int i=0; i<this.team.Count; i++)
                            if (athlete.ID == this.team[i].ID)
                            {
                                athlete = (FRCTeam) team[i].Clone();
                                break;
                            }
                    }
                    
                }
                else if (reader.Name == NO_DANS_LA_POULE)
                    athlete.FencerNumberInPoule = int.Parse(reader.Value);
                else if (reader.Name == NB_VICTOIRES)
                    athlete.NumberOfVictoriesInPoule = int.Parse(reader.Value);
                else if (reader.Name == NB_MATCHES)
                    athlete.NumberOfMatchesInPoule = int.Parse(reader.Value);
                else if (reader.Name == TD)
                    athlete.HitsGivenInPoule = int.Parse(reader.Value);
                else if (reader.Name == TR)
                    athlete.HitsTakenInPoule = int.Parse(reader.Value);
                else if (reader.Name == RANG_POULE)
                    athlete.PouleRanking = int.Parse(reader.Value);

                //Console.Write(" " + reader.Name + "='" + reader.Value + "'");
            }
            athlete.VM = (double) athlete.NumberOfVictoriesInPoule / (double) athlete.NumberOfMatchesInPoule;
            athlete.Index = athlete.HitsGivenInPoule - athlete.HitsTakenInPoule;

            if(isIndividualCompetition)
                pouleRound[pouleRound.Count - 1].getCurrentPoule().addFencer((FRCFencer) athlete);
            else
                pouleRound[pouleRound.Count - 1].getCurrentPoule().addTeam((FRCTeam) athlete);

        }

        /// <summary>
        /// Intreprets the information of the poule round from the xml file.
        /// </summary>
        private void interpretPouleRound()
        {
            if (reader.Name == TOUR_DE_POULES)
            {
                pouleRoundIntro = true;
                poulePhase = true;
                FRCPouleRound pr = new FRCPouleRound();
                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name == ID)
                        pr.RoundNumber = int.Parse(reader.Value);
                    else if (reader.Name == NB_DE_POULES)
                        pr.NumOfPoules = int.Parse(reader.Value);
                    else if (reader.Name == NB_QUALIFIES_PAR_INDICE)
                        pr.NumOfQualifiers = int.Parse(reader.Value);

                    //Console.Write(" " + reader.Name + "='" + reader.Value + "'");
                }

                pouleRound.Add(pr);
            } //Checks that TIREUR or EQUIPE appears during the intro of the poule round.
            else if ((reader.Name == TIREUR || reader.Name == EQUIPE) && (pouleRoundIntro))
            {
                FRCCompetitor athlete;
                if (isIndividualCompetition)
                    athlete = new FRCFencer();
                else
                    athlete = new FRCTeam();

                int i = -1;
               
                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name == REF)
                    {
                        athlete.ID = int.Parse(reader.Value);
                        if (isIndividualCompetition) //Copying info from this.fencer if individual competition
                        {
                            for (i = 0; i < this.fencer.Count; i++)
                                if (athlete.ID == this.fencer[i].ID)
                                {
                                    athlete = (FRCFencer)this.fencer[i].Clone();
                                    break;
                                }
                        }
                        else //Copying info from this.team if team competition
                        {
                            for(i=0; i<this.team.Count; i++)
                                if (athlete.ID == this.team[i].ID)
                                {
                                    athlete = (FRCTeam)this.team[i].Clone();
                                    break;
                                }
                        }
                    }
                    else if (reader.Name == RANG_INITIAL)
                    {
                        athlete.PhaseInitialRanking = int.Parse(reader.Value);
                        //Sets initial ranking of the competitor.
                        if (isIndividualCompetition)
                        {
                            if (fencer[i].InitialRanking == -1)
                                fencer[i].InitialRanking = int.Parse(reader.Value);
                        }
                        else
                        {
                            if (team[i].InitialRanking == -1)
                                team[i].InitialRanking = int.Parse(reader.Value);
                        }
                        
                    }
                    else if (reader.Name == RANG_FINAL)
                        athlete.PhaseFinalRanking = int.Parse(reader.Value);
                    else if (reader.Name == STATUT)
                    {
                        athlete.NoStatusException = false;
                        if (reader.Value == "Q")
                            athlete.IsStillInTheCompetition = true;
                        else if (reader.Value == "N")
                            athlete.IsStillInTheCompetition = false;
                        else if (reader.Value == "A")
                        {
                            athlete.IsStillInTheCompetition = false;
                            athlete.Abandoned = true;
                            if (isIndividualCompetition)
                                fencer[i].Abandoned = true;
                            else
                                team[i].Abandoned = true;
                        }
                        else if (reader.Value == "F")
                        {
                            athlete.IsStillInTheCompetition = false;
                            athlete.Forfait = true;
                            if (isIndividualCompetition)
                                fencer[i].Forfait = true;
                            else
                                team[i].Forfait = true;
                        }
                        else if (reader.Value == "E")
                        {
                            athlete.IsStillInTheCompetition = false;
                            athlete.Excluded = true;
                            if (isIndividualCompetition)
                                fencer[i].Excluded = true;
                            else
                                team[i].Excluded = true;
                        }
                    }
                    //Console.Write(" " + reader.Name + "='" + reader.Value + "'");
                }
                if (isIndividualCompetition)
                    pouleRound[pouleRound.Count - 1].addFencer((FRCFencer)athlete);
                else
                    pouleRound[pouleRound.Count - 1].addTeam((FRCTeam)athlete);
            }
        }

        /// <summary>
        /// Interprets information about fencers from the xml file.
        /// </summary>
        private void interpretFencers()
        {
            //Register the fencer if the interpreter finds a new fencer.
            if (!endOfTireurs)
                if (reader.Name == TIREUR)
                {
                    registerFencer();
                }
        }

        /// <summary>
        /// Interprets information about referees from the xml file.
        /// </summary>
        private void interpretReferees()
        {
            //Register referee if the interpreter finds a new referee.
            if (!endOfArbitres)
                if (reader.Name == ARBITRE)
                {
                    registerReferee();
                }
        }

        /// <summary>
        /// Interprets the competition element from the xml file.
        /// </summary>
        private void interpretCompetitionType()
        {
            //Checks the competition type and gets competition name.
            if ((reader.Name == COMP_INDIVID) || (reader.Name == BASE_COMP_INDIVID))
            {
                isIndividualCompetition = true;
                while(reader.MoveToNextAttribute())
                {
                    if (reader.Name == TITRE_LONG)
                        competitionTitle = reader.Value;
                    else if (reader.Name == TITRE_COURT)
                        competitionTitleShort = reader.Value;
                }
                
            }
            else if ((reader.Name == COMP_PAR) || (reader.Name == BASE_COMP_PAR))
            {
                isIndividualCompetition = false;
                while(reader.MoveToNextAttribute())
                {
                    if(reader.Name == TITRE_LONG)
                        competitionTitle = reader.Value;
                }
       
            }
        }

        /// <summary>
        /// Register a fencer.
        /// </summary>
        private void registerFencer()
        {
            int id = 0;
            string firstName = "";
            string lastName = "";
            string nationality = "";
            string club = "";
            int result = 0;

            while (reader.MoveToNextAttribute()) // Read the attributes.
            {
                if (reader.Name == ID)
                    id = int.Parse(reader.Value);
                else if (reader.Name == PRENOM)
                    firstName = reader.Value;
                else if (reader.Name == NOM)
                    lastName = reader.Value;
                else if (reader.Name == NATION)
                    nationality = reader.Value;
                else if (reader.Name == CLUB)
                    club = reader.Value;
                else if (reader.Name == CLASSEMENT)
                    result = int.Parse(reader.Value);

                //Console.Write(" " + reader.Name + "='" + reader.Value + "'");
            }
            FRCFencer competitor = new FRCFencer(id, firstName, lastName, nationality, club, result);
            fencer.Add(competitor);
        }

        /// <summary>
        /// Register a referee.
        /// </summary>
        private void registerReferee()
        {
            int id = 0;
            string firstName = "";
            string lastName = "";
            string club = "";
            string nationality = "";
            string category = "";

            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == ID)
                    id = int.Parse(reader.Value);
                else if (reader.Name == PRENOM)
                    firstName = reader.Value;
                else if (reader.Name == NOM)
                    lastName = reader.Value;
                else if (reader.Name == CLUB)
                    club = reader.Value;
                else if (reader.Name == NATION)
                    nationality = reader.Value;
                else if (reader.Name == CATEGORIE)
                    category = reader.Value;

            }

            FRCReferee refe = new FRCReferee(id, firstName, lastName, club, nationality, category);
            referee.Add(refe);
        }
    }
}
