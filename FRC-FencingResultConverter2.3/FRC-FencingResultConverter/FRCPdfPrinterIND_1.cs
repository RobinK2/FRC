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
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace FRC_FencingResultConverter
{
    public partial class FRCPdfPrinterIND
    {
         //The pdf document.
        private PdfDocument document;
        //The interpreter that holds info from the xml file.
        private FRCXmlInterpreter interpreter;
        //This must be updated with y coordinate at last drawing all the time.
        private double currentYCoordinate = 0;
        //The size of the page top.
        private double pageTopSize = 0;
        //True if there is no more qualifiers in current poule round.
        private bool qualifiedEnd = false;
        //True when the first eliminated fencer is printed in current poule round.
        private bool eliminatedStarted = false;
        //True if a new page is added to the document.
        private bool pageAdded = false;
        //Internal ranking for all qualified fencers from all poule rounds.
        private List<FRCFencer> rankListAllPouleRounds = new List<FRCFencer>();
        private PdfPage currentPage;
        private List<FRCFencer> fencer;
        private List<FRCPouleRound> pouleRound;
        private List<XPoint> tableauEndPoint = new List<XPoint>();
        private List<FRCFencer> fencerInTableau = new List<FRCFencer>();
        private bool onlyOnce = false;
        private int pageIdx = 0;
        private int tableauPageCounter = 0;
        //True if printing of tableau has started.
        private bool tableauStarted = false;
        //True if the competition format is "qualify on last poule and rank on last poule".
        private bool qualifyLastRankLast = false;
        //True if the competition format is "qualify on last poule and rank on all poules".
        private bool qualifyLastRankAll = false;
        //True if the competition format is "qualify on all poules and rank on all poules".
        private bool qualifyAllRankAll = false;
                
        //The default font type to be used in the document.
        private const string FONT_TYPE = "Arial Narrow";
        //The width of a poule protocole. 48mm * 2.8 
        private const double POULE_TOTAL_WIDTH = 48 * 2.8;
        //The height of a rectangle used for poule protocole. 5mm * 2.8
        private const double POULE_RECTANGLE_HEIGHT = 5 * 2.8;
        //The space between two lines when drawing the smallest tableau. 12mm * 2.8
        private const double TABLEAU_SPACE = 12 * 2.8;
        //The width of each tableau. 50mm * 2.8
        private const double TABLEAU_WIDTH = 50 * 2.8;
        //Drawings should start from this x coordinate by default.
        private const double DEFAULT_START_X = 40;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title">The title of a file.</param>
        /// <param name="interpreter">The xml interpreter.</param>
        public FRCPdfPrinterIND(string title, FRCXmlInterpreter interpreter)
        {
            //Creates a new pdf document with title and file name from the parameter.
            document = new PdfDocument();
            document.Info.Title = title;
            currentPage = document.AddPage();

            this.interpreter = interpreter;
            this.fencer = interpreter.getFencerList();
            this.pouleRound = interpreter.getPouleRoundList();
            for (int i = 0; i < this.fencer.Count; i++)
                rankListAllPouleRounds.Add((FRCFencer)this.fencer[i].Clone());
        }

        /// <summary>
        /// Prints everything and returns the printed document.
        /// </summary>
        /// <returns></returns>
        public PdfDocument printResults()
        {
            printPageTop();
            printOverallRanking();
            printCompetitionFormula();
            printStartingFencersPage();
            //Print poule rounds.
            if (pouleRound.Count > 0)
            {
                printPouleRounds();
                if (qualifyLastRankAll)
                    printRankingQualifyLastRankAll();
            }
            //Print tableau.
            interpreter.Tableau.resetTableauPartIndex();
            if (interpreter.Tableau.amountOfFencers() > 0)
            {
                while (interpreter.Tableau.hasNextTableauPart())
                    printTableau();
            }

            printRefereeActivity();
            return document;
        }

        /// <summary>
        /// Adds a new page if the current page reaches its limit.
        /// </summary>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        private PdfPage getCurrentPage()
        {
            if (currentYCoordinate >= 810)
            {
                currentPage = document.AddPage();
                printPageTop();
                pageAdded = true;
                if (tableauStarted)
                    tableauPageCounter++;
            }

            return currentPage;
        }

        /// <summary>
        /// Prints the result of given poule round.
        /// </summary>
        /// <param name="pouleRound"></param>
        private void printRankingAfterPouleRound(FRCPouleRound pouleRound)
        {
            //Starting on a new page if the given page is not empty.
            if (currentYCoordinate > pageTopSize)
                currentYCoordinate = 810;

            XGraphics graphics = XGraphics.FromPdfPage(getCurrentPage());
            XFont font1 = new XFont(FONT_TYPE, 11, XFontStyle.Bold);
            XFont font2 = new XFont(FONT_TYPE, 10);
            double font1Height = font1.GetHeight();
            double font2Height = font2.GetHeight();
            double x = DEFAULT_START_X;
            FRCFencer fencer, fencerInList;
            int amountOfFencers = pouleRound.amountOfFencers();
            for (int i = 0; i < pouleRound.amountOfFencers(); i++)
            {
                fencer = pouleRound.getNextFencer();
                fencerInList = getFencerMatchingID(fencer.ID);
                if (fencerInList.Forfait || fencerInList.Excluded)
                    amountOfFencers--;
            }
            string title = FRCPrinterConstants.RANKING_OF_POULES +  ", " + FRCPrinterConstants.ROUND + " " + FRCPrinterConstants.NUMBER +
                " " + pouleRound.RoundNumber.ToString() + " (" + amountOfFencers.ToString() + " " + FRCPrinterConstants.FENCERS_LOWER + ")";
            graphics.DrawString(title, font1, XBrushes.Black, x, currentYCoordinate);
            currentYCoordinate += font1Height * 2;
            x += 5;
            graphics.DrawString(FRCPrinterConstants.RANK, font2, XBrushes.Black, x, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.NAME, font2, XBrushes.Black, x + 45, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.CLUB, font2, XBrushes.Black, x + 235, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.V_M, font2, XBrushes.Black, x + 330, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.HS_HR, font2, XBrushes.Black, x + 365, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.HS, font2, XBrushes.Black, x + 400, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.Q_E, font2, XBrushes.Black, x + 425, currentYCoordinate);
            currentYCoordinate += font2Height * 2;

            //Added since the meaning of RangFinal is not consistent for all versions of EnGarde
            for (int i = 0; i < pouleRound.amountOfFencers(); i++)
            {
                fencer = pouleRound.getNextFencer();
                fencerInList = getFencerMatchingID(fencer.ID);
                pouleRound.CopyPouleResult(fencerInList);
            }
            pouleRound.calculateFencerPouleRanking();

            pouleRound.sortFencerPouleRanking();
            for (int i = 0; i < pouleRound.amountOfFencers(); i++)
            {
                fencer = pouleRound.getNextFencer();
                //fencerInList = getFencerMatchingID(fencer.ID);
                //Skip if the fencer got scratch or exclusion.
                if (fencer.Forfait || fencer.Excluded)
                    continue;                
                //Skip if the fencer does not have a status, since that is odd and happens sometime.
                if (fencer.NoStatusException == true)
                    continue;

                double totalY = currentYCoordinate + font2Height * 1.3;
                graphics = checkYCoordinate(graphics, totalY);

                string pr = fencer.PhaseFinalRanking.ToString();
                string name = fencer.LastName.ToUpper() + " " + fencer.FirstName;
                string club = fencer.Club;
                string ind = (fencer.HitsGivenInPoule - fencer.HitsTakenInPoule).ToString();
                string hs = fencer.HitsGivenInPoule.ToString();
                string qe;
                string vm_s = fencer.VM_String;
                if (fencer.IsStillInTheCompetition)
                    qe = FRCPrinterConstants.QUALIFIED;
                else if (fencer.Abandoned)
                {
                    qe = FRCPrinterConstants.ABANDONED;
                    ind = " ";
                    hs = "";
                    vm_s = "";
                }
                else
                {
                    qe = FRCPrinterConstants.ELIMINATED;
                    qualifiedEnd = true;
                }

                if (name.Length > 37)
                    name = name.Remove(37);
                if (club.Length > 15)
                    club = club.Remove(15);

                if (qualifiedEnd && !eliminatedStarted)
                {
                    XPen pen = new XPen(XColors.Black, 0.5);
                    graphics = checkYCoordinate(graphics, totalY + font2Height * 0.5);
                    //currentYCoordinate += font2Height * 0.25;
                    graphics.DrawLine(pen, x, currentYCoordinate - font2Height * 0.7, x + 470, currentYCoordinate - font2Height * 0.7);
                    currentYCoordinate += font2Height * 0.5;
                    eliminatedStarted = true;
                }

                graphics.DrawString(pr, font2, XBrushes.Black, x + 15 - pr.Length * 5, currentYCoordinate);
                graphics.DrawString(name, font2, XBrushes.Black, x + 45, currentYCoordinate);
                graphics.DrawString(club, font2, XBrushes.Black, x + 235, currentYCoordinate);
                graphics.DrawString(vm_s, font2, XBrushes.Black, x + 330, currentYCoordinate);
                if (ind[0] == '-')
                    graphics.DrawString(ind, font2, XBrushes.Black, x + 374 - (ind.Length - 1) * 5, currentYCoordinate);
                else
                    graphics.DrawString(ind, font2, XBrushes.Black, x + 377 - ind.Length * 5, currentYCoordinate);
                graphics.DrawString(hs, font2, XBrushes.Black, x + 412 - hs.Length * 5, currentYCoordinate);
                graphics.DrawString(qe, font2, XBrushes.Black, x + 425, currentYCoordinate);

                currentYCoordinate += font2Height * 1.3;
            }

            qualifiedEnd = false;
            eliminatedStarted = false;
            graphics.Dispose();
        }

        /// <summary>
        /// Prints all poule rounds. And ranking after each poule round as well.
        /// </summary>
        private void printPouleRounds()
        {
            XGraphics graphics = null;
            XFont font = new XFont(FONT_TYPE, 11, XFontStyle.Bold);
            double fontHeight = font.GetHeight();
            double x = DEFAULT_START_X;
            for (int i = 0; i < pouleRound.Count; i++)
            {
                //Starting on a new page if the given page is not empty.
                if (currentYCoordinate > pageTopSize)
                    currentYCoordinate = 810;

                graphics = XGraphics.FromPdfPage(getCurrentPage());
                string title = FRCPrinterConstants.POULES_ROUND + " " + (i + 1).ToString();
                graphics.DrawString(title, font, XBrushes.Black, x, currentYCoordinate);
                currentYCoordinate += fontHeight * 3;

                for (int j = 0; j < pouleRound[i].NumOfPoules; j++)
                {
                    XFont tempFont = new XFont(FONT_TYPE, 10);
                    FRCPoule poule = pouleRound[i].getNextPoule();
                    double totalY = currentYCoordinate + tempFont.GetHeight() * (4.9 + 1.25 * poule.amountOfFencers());
                    graphics = checkYCoordinate(graphics, totalY);
                    printPoule(graphics, poule, j + 1);
                }

                registerEliminationOnRankList(pouleRound[i]);

                if (i == pouleRound.Count - 1)
                {
                    checkTableauRankFormat(pouleRound[i]);
                    /*
                    if (qualifyAllRankAll)
                        Console.WriteLine("d");
                    if (qualifyLastRankAll)
                        Console.WriteLine("b");
                    if (qualifyLastRankLast)
                        Console.WriteLine("a");
                    */

                    //Different competition formats.
                    if (qualifyLastRankLast)
                    {
                        //Do nothing.
                    }
                    else if (qualifyAllRankAll)
                    {
                        //Print summing ranking after all poule rounds and skip the ranking of the last poule.
                        printRankingAfterAllPouleRounds();
                        break;
                    }
                    else if (qualifyLastRankAll)
                    {
                        //Do nothing. Print summing ranking later.
                    }
                }
                printRankingAfterPouleRound(pouleRound[i]);
            }
        }

        /// <summary>
        /// Prints the poule with given number.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="poule">The poule to be printed.</param>
        /// <param name="pouleNumber">The poule number.</param>
        private void printPoule(XGraphics graphics, FRCPoule poule, int pouleNumber)
        {
            XFont font = new XFont(FONT_TYPE, 10);
            double fontHeight = font.GetHeight();
            double x = DEFAULT_START_X;

            string s1 = FRCPrinterConstants.POULE + " " + FRCPrinterConstants.NUMBER + " " + pouleNumber + "  " +
                poule.StartTime + "  " + FRCPrinterConstants.PISTE + " " + FRCPrinterConstants.NUMBER + " " + poule.PisteNumber;
            string s2 = FRCPrinterConstants.REFEREE + ": ";
            for(int i=0; i<poule.amountOfReferees(); i++)
            {
                FRCReferee referee = poule.getNextReferee();
                s2 += referee.LastName.ToUpper() + " " + referee.FirstName + " " + referee.Club;
                if (poule.hasNextReferee())
                    s2 += ", ";
            }
            graphics.DrawString(s1, font, XBrushes.Black, x, currentYCoordinate);
            currentYCoordinate += fontHeight * 1.3;
            graphics.DrawString(s2, font, XBrushes.Black, x, currentYCoordinate);
            currentYCoordinate += fontHeight * 1.3;
            graphics.DrawString(FRCPrinterConstants.V_M, font, XBrushes.Black, x + 400, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.HS_HR, font, XBrushes.Black, x + 435, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.HS, font, XBrushes.Black, x + 470, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.RANK, font, XBrushes.Black, x + 495, currentYCoordinate);
            currentYCoordinate += fontHeight * 1.3;

            drawPouleGraph(graphics, x + 250, currentYCoordinate, poule.amountOfFencers());
            double savedY = currentYCoordinate;
            currentYCoordinate += fontHeight;
            poule.sortFencersFencerNumberInPoule();
            
            for (int i = 0; i < poule.amountOfFencers(); i++)
            {
                FRCFencer fencer = poule.getNextFencer();
                fencer = registerFencerStatusInPouleFromMatch(poule, fencer);
                copyFencerInfo(fencer);
                registerPouleResult(fencer);
                string name = fencer.LastName.ToUpper() + " " + fencer.FirstName;
                string club = fencer.Club;
                double vm = fencer.VM;
                string vm_s = vm.ToString().Replace(',', '.');
                string ind = fencer.Index.ToString();
                string hs = fencer.HitsGivenInPoule.ToString();
                string rank = fencer.PouleRanking.ToString();
                if (vm_s.Length == 1)
                    vm_s += ".";
                vm_s += "000";
                if (vm_s.Length > 5)
                    vm_s = vm_s.Remove(5);
                //Saving the value for fencer list in this class.
                getFencerMatchingID(fencer.ID).VM_String = vm_s;
                
                if (name.Length > 30)
                    name = name.Remove(30);
                if (club.Length > 15)
                    club = club.Remove(15);

                if (fencer.Abandoned || fencer.Forfait || fencer.Excluded)
                {
                    vm_s = "";
                    ind = " ";
                    hs = "";
                    rank = ""; 
                }

                graphics.DrawString(name, font, XBrushes.Black, x + 5, currentYCoordinate);
                graphics.DrawString(club, font, XBrushes.Black, x + 160, currentYCoordinate);
                graphics.DrawString(vm_s, font, XBrushes.Black, x + 400, currentYCoordinate);
                if (ind[0] == '-')
                    graphics.DrawString(ind, font, XBrushes.Black, x + 447 - (ind.Length-1) * 5, currentYCoordinate);
                else
                    graphics.DrawString(ind, font, XBrushes.Black, x + 450 - ind.Length * 5, currentYCoordinate);
                graphics.DrawString(hs, font, XBrushes.Black, x + 482 - hs.Length * 5, currentYCoordinate);
                graphics.DrawString(rank, font, XBrushes.Black, x + 513 - rank.Length * 5, currentYCoordinate);

                currentYCoordinate += fontHeight * 1.25;
            }
            fillPouleProtocole(graphics, font, poule, x + 250, savedY);
            currentYCoordinate += fontHeight * 2;
        }

        /// <summary>
        /// Registers abandon, forfait, exclusion and IsStillInCompetition from given poule to given fencer.
        /// Since it is not always given in the poule phase.
        /// </summary>
        /// <param name="poule"></param>
        /// <returns></returns>
        private FRCFencer registerFencerStatusInPouleFromMatch(FRCPoule poule, FRCFencer fencer)
        {
            FRCMatch match;
            for (int i = 0; i < poule.amountOfMatches(); i++)
            {
                match = poule.getNextMatch();

                if (fencer.ID == match.FencerID1)
                {
                    if (match.Fencer1Abandon)
                    {
                        fencer.Abandoned = true;
                        fencer.IsStillInTheCompetition = false;
                    }
                    else if (match.Fencer1Forfait)
                    {
                        fencer.Forfait = true;
                        fencer.IsStillInTheCompetition = false;
                    }
                    else if (match.Fencer1Exclusion)
                    {
                        fencer.Excluded = true;
                        fencer.IsStillInTheCompetition = false;
                    }
                    return fencer;
                }
                else if (fencer.ID == match.FencerID2)
                {
                    if (match.Fencer2Abandon)
                    {
                        fencer.Abandoned = true;
                        fencer.IsStillInTheCompetition = false;
                    }
                    else if (match.Fencer2Forfait)
                    {
                        fencer.Forfait = true;
                        fencer.IsStillInTheCompetition = false;
                    }
                    else if (match.Fencer2Exclusion)
                    {
                        fencer.Excluded = true;
                        fencer.IsStillInTheCompetition = false;
                    }
                    return fencer;
                }
            }

            return fencer;
        }

        /// <summary>
        /// Fills a poule protocole with given poule at given coordinates.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="font"></param>
        /// <param name="poule"></param>
        /// <param name="x">The x coordinate where a poule is already drawn.</param>
        /// <param name="y">The y coordinate where a poule is already drawn.</param>
        private void fillPouleProtocole(XGraphics graphics, XFont font, FRCPoule poule, double x, double y)
        {
            int length = poule.amountOfFencers();
            double fontHeight = font.GetHeight();
            double pouleRectWidth = POULE_TOTAL_WIDTH / length;
            FRCFencer fencer1, fencer2;
            FRCMatch match;
            string res1, res2;
            double x1, y1, x2, y2;
            y += fontHeight;

            for(int i=0; i<poule.amountOfMatches(); i++)
            {
                match = poule.getNextMatch();
                assignMatchResult(out res1, out res2, out fencer1, out fencer2, match);

                y1 = y + (fencer1.FencerNumberInPoule - 1) * fontHeight * 1.25;
                y2 = y + (fencer2.FencerNumberInPoule - 1) * fontHeight * 1.25;
                x1 = x + pouleRectWidth * (fencer2.FencerNumberInPoule - 1);
                x2 = x + pouleRectWidth * (fencer1.FencerNumberInPoule - 1);
                if (res1.Length == 1)
                    x1 += pouleRectWidth * 0.3;
                else if (res1.Length == 2)
                    x1 += pouleRectWidth * 0.2;

                if (res2.Length == 1)
                    x2 += pouleRectWidth * 0.3;
                else if (res2.Length == 2)
                    x2 += pouleRectWidth * 0.2;

                graphics.DrawString(res1, font, XBrushes.Black, x1, y1);
                graphics.DrawString(res2, font, XBrushes.Black, x2, y2);
            }
        }

        /// <summary>
        /// Assigns match results from given match to given strings, and given fencers as fencers in the match.
        /// </summary>
        /// <param name="res1"></param>
        /// <param name="res2"></param>
        /// <param name="fencer1"></param>
        /// <param name="fencer2"></param>
        /// <param name="match"></param>
        private void assignMatchResult(out string res1, out string res2, out FRCFencer fencer1, 
            out FRCFencer fencer2, FRCMatch match)
        {
            res1 = "";
            res2 = "";
            fencer1 = getFencerMatchingID(match.FencerID1);
            fencer2 = getFencerMatchingID(match.FencerID2);

            if (match.Fencer1Win)
            {
                res1 += "V";
                if (match.ScoreFencer1 < 5)
                    res1 += match.ScoreFencer1.ToString();
                res2 += match.ScoreFencer2.ToString();
            }
            else if (match.Fencer2Win)
            {
                res2 += "V";
                if (match.ScoreFencer2 < 5)
                    res2 += match.ScoreFencer2.ToString();
                res1 += match.ScoreFencer1.ToString();
            }
            else if (match.Fencer1Abandon)
            {
                res1 += "A";
                res2 += "X";
            }
            else if (match.Fencer2Abandon)
            {
                res2 += "A";
                res1 += "X";
            }
            else if (match.Fencer1Forfait)
            {
                res1 += "S";
                res2 += "X";
            }
            else if (match.Fencer2Forfait)
            {
                res2 += "S";
                res1 += "X";
            }
            else if (match.Fencer1Exclusion)
            {
                res1 += "E";
                res2 += "X";
            }
            else if (match.Fencer2Exclusion)
            {
                res2 += "E";
                res1 += "X";
            }
        }

        private FRCFencer getFencerFromTableauMatchingInitialRanking(int rank)
        {
            FRCFencer fencer;
            for(int i=0; i<interpreter.Tableau.amountOfFencers(); i++)
            {
                fencer = interpreter.Tableau.getNextFencer();
                if (fencer.PhaseInitialRanking == rank)
                    return fencer;
            }
            return null;
        }
        
        private FRCFencer getFencerFromTableauMatchingID(int id)
        {
            FRCFencer fencer;
            for (int i = 0; i < interpreter.Tableau.amountOfFencers(); i++)
            {
                fencer = interpreter.Tableau.getNextFencer();
                if (fencer.ID == id)
                    return fencer;
            }

            return null;
        }

        
        /// <summary>
        /// Copies the IsStillInCompetition, Abandoned, Forfait and Excluded values from all fencers in given poule round
        /// to the internal ranking.
        /// </summary>
        /// <param name="pouleRound"></param>
        private void registerEliminationOnRankList(FRCPouleRound pouleRound)
        {
            FRCFencer fencer;
            pouleRound.resetFencerIndex();

            for (int i = 0; i < pouleRound.amountOfFencers(); i++)
            {
                fencer = pouleRound.getNextFencer();
                for (int j = 0; j < rankListAllPouleRounds.Count; j++)
                {
                    if (fencer.ID == rankListAllPouleRounds[j].ID)
                    {
                        if(rankListAllPouleRounds[j].IsStillInTheCompetition)
                            rankListAllPouleRounds[j].IsStillInTheCompetition = fencer.IsStillInTheCompetition;

                        //These abandoned, excluded, forfait assignments are probably unnecessary, but they are not wrong and it works.
                        if(!rankListAllPouleRounds[j].Abandoned)
                            rankListAllPouleRounds[j].Abandoned = fencer.Abandoned;
                        if(!rankListAllPouleRounds[j].Excluded)
                            rankListAllPouleRounds[j].Excluded = fencer.Excluded;
                        if(!rankListAllPouleRounds[j].Forfait)
                            rankListAllPouleRounds[j].Forfait = fencer.Forfait;
                        break;
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
        /// Registers result from poule round for given fencer to an internal ranking of all poule rounds.
        /// </summary>
        /// <param name="fencer"></param>
        private void registerPouleResult(FRCFencer fencer)
        {
            for(int i=0; i<this.rankListAllPouleRounds.Count; i++)
                if (this.rankListAllPouleRounds[i].ID == fencer.ID)
                {
                    this.rankListAllPouleRounds[i].IsStillInTheCompetition = fencer.IsStillInTheCompetition;
                    this.rankListAllPouleRounds[i].Abandoned = fencer.Abandoned;
                    this.rankListAllPouleRounds[i].Excluded = fencer.Excluded;
                    this.rankListAllPouleRounds[i].Forfait = fencer.Forfait;
                    this.rankListAllPouleRounds[i].HitsGivenInPoule += fencer.HitsGivenInPoule;
                    this.rankListAllPouleRounds[i].HitsTakenInPoule += fencer.HitsTakenInPoule;
                    this.rankListAllPouleRounds[i].NumberOfMatchesInPoule += fencer.NumberOfMatchesInPoule;
                    this.rankListAllPouleRounds[i].NumberOfVictoriesInPoule += fencer.NumberOfVictoriesInPoule;
                    
                    return;
                }
        }

        /// <summary>
        /// Copying info from given fencer to the fencers in the list of this class.
        /// </summary>
        /// <param name="fencer"></param>
        private void copyFencerInfo(FRCFencer fencer)
        {
            for (int i = 0; i < this.fencer.Count; i++)
                if (this.fencer[i].ID == fencer.ID)
                {
                    this.fencer[i] = (FRCFencer) fencer.Clone();
                    return;
                }
        }

        /// <summary>
        /// Prints a page with all fencers with their initial rankings.
        /// </summary>
        private void printStartingFencersPage()
        {
            //Starting on a new page if the given page is not empty.
            if (currentYCoordinate > pageTopSize)
                currentYCoordinate = 810;

            XGraphics graphics = XGraphics.FromPdfPage(getCurrentPage());
            XFont font1 = new XFont(FONT_TYPE, 11, XFontStyle.Bold);
            XFont font2 = new XFont(FONT_TYPE, 10);
            double font1Height = font1.GetHeight();
            double font2Height = font2.GetHeight();
            double x = DEFAULT_START_X;

            string title = FRCPrinterConstants.INITIAL_RANKING + " (" + fencer.Count + " " + FRCPrinterConstants.FENCERS_LOWER + ")";
            graphics.DrawString(title, font1, XBrushes.Black, x, currentYCoordinate);
            currentYCoordinate += font1Height * 2;
            x += 5;
            graphics.DrawString(FRCPrinterConstants.RANKING, font2, XBrushes.Black, x, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.NAME, font2, XBrushes.Black, x + 45, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.NATION, font2, XBrushes.Black, x + 235, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.CLUB, font2, XBrushes.Black, x + 285, currentYCoordinate);
            currentYCoordinate += font2Height * 2;

            sortFencerInitialRanking();
            for (int i = 0; i < fencer.Count; i++)
            {
                double totalY = currentYCoordinate + font2Height * 1.3;
                graphics = checkYCoordinate(graphics, totalY);

                string ir = fencer[i].InitialRanking.ToString();
                graphics.DrawString(ir, font2, XBrushes.Black, x + 30 - ir.Length * 5, currentYCoordinate);

                string name = fencer[i].LastName.ToUpper() + " " + fencer[i].FirstName;
                string club = fencer[i].Club;
                if (name.Length > 37)
                    name = name.Remove(37);
                if (club.Length > 15)
                    club = club.Remove(15);

                graphics.DrawString(name, font2, XBrushes.Black, x + 45, currentYCoordinate);
                graphics.DrawString(fencer[i].Nationality, font2, XBrushes.Black, x + 235, currentYCoordinate);
                graphics.DrawString(club, font2, XBrushes.Black, x + 285, currentYCoordinate);
                currentYCoordinate += font2Height * 1.3;
            }
            graphics.Dispose();
        }

        /// <summary>
        /// Prints the formula of the competition.
        /// </summary>
        private void printCompetitionFormula()
        {
            //Starting on a new page if the given page is not empty.
            if (currentYCoordinate > pageTopSize)
                currentYCoordinate = 810;

            XGraphics graphics = XGraphics.FromPdfPage(getCurrentPage());
            XFont font1 = new XFont(FONT_TYPE, 11, XFontStyle.Bold);
            XFont font2 = new XFont(FONT_TYPE, 10);
            double font1Height = font1.GetHeight();
            double font2Height = font2.GetHeight();
            double x = DEFAULT_START_X;

            graphics.DrawString(FRCPrinterConstants.FORMULA_OF_COMPETITION, font1, XBrushes.Black, x, currentYCoordinate);
            currentYCoordinate += font1Height * 2;
            string s1 = fencer.Count.ToString() + " " + FRCPrinterConstants.FENCERS_LOWER;
            graphics.DrawString(s1, font2, XBrushes.Black, x, currentYCoordinate);
            currentYCoordinate += font2Height * 3;

            for (int i = 0; i < pouleRound.Count; i++)
            {
                double totalY = currentYCoordinate + font2Height * 4.6;
                graphics = checkYCoordinate(graphics, totalY);

                string s2 = FRCPrinterConstants.POULES_ROUND + " " + (i + 1).ToString() + ":";
                graphics.DrawString(s2, font2, XBrushes.Black, x, currentYCoordinate);
                currentYCoordinate += font2Height * 2;
                string s3 = pouleRound[i].amountOfFencers().ToString() + " " + FRCPrinterConstants.FENCERS_LOWER;
                graphics.DrawString(s3, font2, XBrushes.Black, x, currentYCoordinate);
                currentYCoordinate += font2Height * 1.3;
                string s4 = pouleRound[i].amountOfPoules().ToString() + " " + FRCPrinterConstants.POULES_LOWER + " ";
                graphics.DrawString(s4, font2, XBrushes.Black, x, currentYCoordinate);
                currentYCoordinate += font2Height * 1.3;
                string s5 = pouleRound[i].NumOfQualifiers.ToString() + " " + FRCPrinterConstants.QUALIFIERS;
                graphics.DrawString(s5, font2, XBrushes.Black, x, currentYCoordinate);
                currentYCoordinate += font2Height * 2;
            }

            if (interpreter.Tableau.amountOfFencers() > 0)
            {
                double totalY = currentYCoordinate + font2Height * 3;
                graphics = checkYCoordinate(graphics, totalY);

                string s6 = "----------------------------------------------------";
                graphics.DrawString(s6, font2, XBrushes.Black, x, currentYCoordinate);
                currentYCoordinate += font2Height * 3;
                string s7 = FRCPrinterConstants.DIRECT_ELIMINATION + ": " + interpreter.Tableau.amountOfFencers().ToString() +
                    " " + FRCPrinterConstants.FENCERS_LOWER;
                graphics.DrawString(s7, font2, XBrushes.Black, x, currentYCoordinate);
                currentYCoordinate += font2Height * 2;
            }
            graphics.Dispose();
        }

        /// <summary>
        /// If a fencer got forfait or exclusion at the poule, that will be registered to this.fencer.
        /// This is necessary, since it is not given at beginning of poule phase of the xml file sometimes.
        /// </summary>
        private void registerForfaitExclusionFromPoules()
        {
            FRCPoule poule;
            FRCMatch match;
            FRCFencer fencer;
            for (int i = 0; i < pouleRound.Count; i++)
            {
                for (int j = 0; j < pouleRound[i].amountOfPoules(); j++)
                {
                    poule = pouleRound[i].getNextPoule();
                    for (int k = 0; k < poule.amountOfMatches(); k++)
                    {
                        match = poule.getNextMatch();
                        if (match.Fencer1Exclusion || match.Fencer1Forfait)
                        {
                            fencer = getFencerMatchingID(match.FencerID1);
                            fencer.Excluded = match.Fencer1Exclusion;
                            fencer.Forfait = match.Fencer1Forfait;
                        }
                        else if (match.Fencer2Exclusion || match.Fencer2Forfait)
                        {
                            fencer = getFencerMatchingID(match.FencerID2);
                            fencer.Excluded = match.Fencer2Exclusion;
                            fencer.Forfait = match.Fencer2Forfait;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Prints the overall ranking page.
        /// </summary>
        private void printOverallRanking()
        {
            XGraphics graphics = XGraphics.FromPdfPage(getCurrentPage());
            XFont font1 = new XFont(FONT_TYPE, 11, XFontStyle.Bold);
            XFont font2 = new XFont(FONT_TYPE, 10);
            double font1Height = font1.GetHeight();
            double font2Height = font2.GetHeight();
            double x = DEFAULT_START_X;
            int amountOfFencers = fencer.Count;
            for (int i = 0; i < interpreter.Tableau.amountOfFencers(); i++)
                copyFencerInfo(interpreter.Tableau.getNextFencer());

            //For a bug fix, Forfait and Exclusion must be checked from all poule matches here.
            registerForfaitExclusionFromPoules();

            for (int i = 0; i < fencer.Count; i++)
                if (fencer[i].Forfait || fencer[i].Excluded)
                    amountOfFencers--;

            string title = FRCPrinterConstants.FINAL_RESULTS + " (" + amountOfFencers.ToString() + " " +
                FRCPrinterConstants.FENCERS_LOWER + ")";

            graphics.DrawString(title, font1, XBrushes.Black, x, currentYCoordinate);
            currentYCoordinate += font1Height * 2;
            x += 5;
            graphics.DrawString(FRCPrinterConstants.PLACE, font2, XBrushes.Black, x, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.NAME, font2, XBrushes.Black, x + 30, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.NATION, font2, XBrushes.Black, x + 220, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.CLUB, font2, XBrushes.Black, x + 270, currentYCoordinate);
            currentYCoordinate += font2Height * 2;

            sortFencerFinalRanking();
            for (int i = 0; i < fencer.Count; i++)
            {
                if (fencer[i].FinalRanking == 0)
                    continue;

                double totalY = currentYCoordinate + font2Height * 1.3;
                graphics = checkYCoordinate(graphics, totalY);

                string fr = fencer[i].FinalRanking.ToString();
                graphics.DrawString(fr, font2, XBrushes.Black, x + 20 - fr.Length * 5, currentYCoordinate);

                string name = fencer[i].LastName.ToUpper() + " " + fencer[i].FirstName;
                string club = fencer[i].Club;
                if (name.Length > 37)
                    name = name.Remove(37);
                if (club.Length > 15)
                    club = club.Remove(15);

                graphics.DrawString(name, font2, XBrushes.Black, x + 30, currentYCoordinate);
                graphics.DrawString(fencer[i].Nationality, font2, XBrushes.Black, x + 220, currentYCoordinate);
                graphics.DrawString(club, font2, XBrushes.Black, x + 270, currentYCoordinate);
                currentYCoordinate += font2Height * 1.3;
            }

            graphics.Dispose();
        }

        /// <summary>
        /// Check that the given y coordinate is within range before a new page must be added.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private XGraphics checkYCoordinate(XGraphics graphics, double y)
        {
            if (y >= 810)
            {
                currentYCoordinate = 810;
                graphics.Dispose();
                graphics = XGraphics.FromPdfPage(getCurrentPage());
            }

            return graphics;
        }

        /// <summary>
        /// Returns fencer from given tableau matching given initial rank for the tableau.
        /// </summary>
        /// <param name="tableau"></param>
        /// <param name="initialRank"></param>
        /// <returns></returns>
        private FRCFencer searchFencerInitialRankingTableau(FRCTableau tableau, int initialRank)
        {
            FRCFencer fencer;
            for (int i = 0; i < tableau.amountOfFencers(); i++)
            {
                fencer = tableau.getNextFencer();
                if (fencer.PhaseInitialRanking == initialRank)
                    return fencer;
            }
            return null;            
        }

        /// <summary>
        /// Sorts all fencers after their final placements.
        /// </summary>
        private void sortFencerFinalRanking()
        {
            for(int i=0; i<fencer.Count - 1; i++)
                for (int j = i+1; j < fencer.Count; j++)
                {
                    if (fencer[i].FinalRanking > fencer[j].FinalRanking)
                    {
                        FRCFencer tempFencer = fencer[j];
                        fencer[j] = fencer[i];
                        fencer[i] = tempFencer;
                    }
                }
        }

        /// <summary>
        /// Sorts all fencers after their initial rankings.
        /// </summary>
        private void sortFencerInitialRanking()
        {
            for(int i=0; i<fencer.Count - 1; i++)
                for (int j = i+1; j < fencer.Count; j++)
                {
                    if (fencer[i].InitialRanking > fencer[j].InitialRanking)
                    {
                        FRCFencer tempFencer = fencer[j];
                        fencer[j] = fencer[i];
                        fencer[i] = tempFencer;
                    }
                }
        }

        /// <summary>
        /// Prints the comptition title on the top of the pdf page. This method should be called once for each new page.
        /// </summary>
        private void printPageTop()
        {
            double fontSize1 = 12;
            double fontSize2 = 11;
            XGraphics graphics = XGraphics.FromPdfPage(currentPage);
            XFont font1 = new XFont(FONT_TYPE, fontSize1);
            XFont font2 = new XFont(FONT_TYPE, fontSize2);
            XFont pageNumberFont = new XFont(FONT_TYPE, 9);
            //Prints page number.
            string pageNumber = document.PageCount.ToString();
            double y = 20;
            double x;
            if(pageNumber.Length > 1)
                x = 572;
            else
                x = 575;
            graphics.DrawString(pageNumber, pageNumberFont, XBrushes.Black, x, y);

            string title = interpreter.Formula.CompetitionName;
            string[] str;
            //There is no space for several lines in title if the tableau is larger than 16. So use TitreCourt in that case.
            if (interpreter.Tableau.amountOfFencers() <= 16)
                str = title.Split('-');
            else
            {          
                str = new string[1];
                str[0] = interpreter.Formula.CompetitionNameShort;
                //str[0] = title;
            }
            x = 300;
            y = 30;
            double fontHeight;
            /*
            double sumOfY;
            do{
                //Calculate sum of y.
                sumOfY = y + font2.GetHeight()*2;
                for(int i=0; i<str.Length; i++)
                {
                    if(i == 0)
                        sumOfY += font1.GetHeight();
                    else
                        sumOfY += font2.GetHeight();
                }

                if(sumOfY >= 60)
                {
                    fontSize1--;
                    fontSize2--;
                    font1 = new XFont(FONT_TYPE, fontSize1);
                    font2 = new XFont(FONT_TYPE, fontSize2);
                    
                }
            }while(sumOfY >= 60);
            */

            //Print title.
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = str[i].Trim();
                if (i == 0)
                {
                    graphics.DrawString(str[i], font1, XBrushes.Black, x, y, XStringFormats.Center);
                    fontHeight = font1.GetHeight();
                }
                else
                {
                    graphics.DrawString(str[i], font2, XBrushes.Black, x, y, XStringFormats.Center);
                    fontHeight = font2.GetHeight();
                }

                y += fontHeight;
            }

            y += font2.GetHeight()*2;
            pageTopSize = y;
            currentYCoordinate = y;
            graphics.Dispose();
        }

        /// <summary>
        /// Draws rectangles for the poule on pdf file.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="x">Top left x coordinate of the poule.</param>
        /// <param name="y">Top left y coordinate of the poule.</param>
        /// <param name="length">Amount of fencers in the poule.</param>
        private void drawPouleGraph(XGraphics graphics, double x, double y, int length)
        {
            XPen pen = new XPen(XColors.Black, 0.5);
            double pouleRectWidth = POULE_TOTAL_WIDTH / length;
            double filledRectWidth = pouleRectWidth * 0.7;
            double filledRectHeight = POULE_RECTANGLE_HEIGHT * 0.7;

            graphics.DrawRectangle(pen, x, y, POULE_TOTAL_WIDTH, POULE_RECTANGLE_HEIGHT * length);
            for (int i = 1; i < length; i++)
            {
                //Draws horizontal lines.
                graphics.DrawLine(pen, x, y + (POULE_RECTANGLE_HEIGHT * i), x + POULE_TOTAL_WIDTH, y + (POULE_RECTANGLE_HEIGHT * i));
                //Draws vertical lines.
                graphics.DrawLine(pen, x + (pouleRectWidth * i), y, x + (pouleRectWidth * i), 
                    y + (POULE_RECTANGLE_HEIGHT * length));
            }
            //Draws middle diagonal filled rectangles.
            for (int i = 0; i < length; i++)
                graphics.DrawRectangle(XBrushes.Gray, x + (pouleRectWidth - filledRectWidth)/2 + pouleRectWidth*i,
                    y + (POULE_RECTANGLE_HEIGHT - filledRectHeight)/2 + POULE_RECTANGLE_HEIGHT*i,
                    filledRectWidth, filledRectHeight);
        }

        /// <summary>
        /// Draws tableau lines for one match.
        /// </summary>
        /// <param name="point">The top left coordinate where the upper horizontal line begins.</param>
        /// <param name="space">Space between the two horizontal lines.</param>
        /// <returns></returns>
        private XPoint drawTableauLines(XGraphics graphics, XPoint point, double space, double width1, double width2)
        {
            XPen pen = new XPen(XColors.Black, 0.5);
            double retX = point.X + width1;
            double retY = point.Y + space / 2;

            //Upper horizontal line
            graphics.DrawLine(pen, point.X, point.Y, point.X + width1, point.Y);
            //Under horizontal line
            graphics.DrawLine(pen, point.X, point.Y + space, point.X + width1, point.Y + space);
            //Vertical line
            graphics.DrawLine(pen, point.X + width1, point.Y, point.X + width1, point.Y + space);
            //Horizontal middle line
            graphics.DrawLine(pen, retX, retY, retX + width2, retY);

            return new XPoint(retX, retY);
        }
    }
}
