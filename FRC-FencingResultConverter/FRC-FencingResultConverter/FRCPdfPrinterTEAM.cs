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
    public class FRCPdfPrinterTEAM
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
        //True when the first eliminated team is printed in current poule round.
        private bool eliminatedStarted = false;
        //True if a new page is added to the document.
        private bool pageAdded = false;
        //Internal ranking for all qualified teams from all poule rounds.
        private List<FRCTeam> rankListAllPouleRounds = new List<FRCTeam>();
        private PdfPage currentPage;
        private List<FRCTeam> team;
        private List<FRCPouleRound> pouleRound;
        private List<XPoint> tableauEndPoint = new List<XPoint>();
        private List<FRCTableau> tableau;
        private int pageIdx = 0;
        private int tableauPageCounter = 0;
        //Counter for tableau.
        private int tableauCounter = 0;
        //True if printing of tableau has started.
        private bool tableauStarted = false;
        /*
        //True if the competition format is "qualify on last poule and rank on last poule".
        private bool qualifyLastRankLast = false;
        //True if the competition format is "qualify on last poule and rank on all poules".
        private bool qualifyLastRankAll = false;
        //True if the competition format is "qualify on all poules and rank on all poules".
        private bool qualifyAllRankAll = false;
        */
                
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
        public FRCPdfPrinterTEAM(string title, FRCXmlInterpreter interpreter)
        {
            //Creates a new pdf document with title and file name from the parameter.
            document = new PdfDocument();
            document.Info.Title = title;
            currentPage = document.AddPage();

            this.interpreter = interpreter;
            this.team = interpreter.getTeamList();
            this.tableau = interpreter.getTableauList();
            this.pouleRound = interpreter.getPouleRoundList();
            for (int i = 0; i < this.team.Count; i++)
                rankListAllPouleRounds.Add((FRCTeam) this.team[i].Clone());
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
            printStartingTeamsPage();
            if(pouleRound.Count > 0)
                printPouleRounds();
            //if (qualifyLastRankAll)
            //    printRankingQualifyLastRankAll();

            for (int i = 0; i < tableau.Count; i++)
            {
                tableau[i].resetTableauPartIndex();
                while (tableau[i].hasNextTableauPart())
                {
                    printTableau(tableau[i]);
                    tableauCounter++;
                }
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
        /// Prints a special ranking if the formula is qualifyLastRankAll.
        /// </summary>
        private void printRankingQualifyLastRankAll()
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
            FRCTeam team;
            int amountOfTeams = rankListAllPouleRounds.Count;
            for (int i = 0; i < rankListAllPouleRounds.Count; i++)
            {
                if (rankListAllPouleRounds[i].Forfait || rankListAllPouleRounds[i].Excluded)
                    amountOfTeams--;
            }

            string title = FRCPrinterConstants.RANKING_AFTER_ALL_POULE_ROUNDS + " (" + amountOfTeams.ToString()
                + " " + FRCPrinterConstants.TEAMS_LOWER + ")";
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

            int rank = 1;
            //Rank all qualified teams.
            for(int i=0; i<rankListAllPouleRounds.Count; i++)
            {
                if (rankListAllPouleRounds[i].IsStillInTheCompetition)
                {
                    rankListAllPouleRounds[i].PhaseFinalRanking = rank;
                    rank++;
                }
            }
            //Rank all eliminated teams.
            for (int i = 0; i < rankListAllPouleRounds.Count; i++)
            {
                if (rankListAllPouleRounds[i].Forfait || rankListAllPouleRounds[i].Excluded)
                    continue;

                if (!rankListAllPouleRounds[i].IsStillInTheCompetition)
                {
                    rankListAllPouleRounds[i].PhaseFinalRanking = rank;
                    rank++;
                }
            }
            //Sort the teams after their PhaseFinalRanking.
            for (int i = 0; i < rankListAllPouleRounds.Count - 1; i++)
                for (int j = i + 1; j < rankListAllPouleRounds.Count; j++)
                {
                    if (rankListAllPouleRounds[i].PhaseFinalRanking > rankListAllPouleRounds[j].PhaseFinalRanking)
                    {
                        FRCTeam tempTeam = rankListAllPouleRounds[j];
                        rankListAllPouleRounds[j] = rankListAllPouleRounds[i];
                        rankListAllPouleRounds[i] = tempTeam;
                    }
                }

            //Print all teams.
            for (int i = 0; i < rankListAllPouleRounds.Count; i++)
            {
                team = rankListAllPouleRounds[i];
                //Skip if the team got scratch of exclusion.
                if (team.PhaseFinalRanking == 0)
                    continue;

                double totalY = currentYCoordinate + font2Height * 1.3;
                graphics = checkYCoordinate(graphics, totalY);

                string pr = team.PhaseFinalRanking.ToString();
                string name = team.Name;
                string club = team.Club;
                string ind = team.Index.ToString();
                string hs = team.HitsGivenInPoule.ToString();
                string qe;
                string vm_s = team.VM.ToString().Replace(',', '.');
                if (vm_s.Length == 1)
                    vm_s += ".";
                vm_s += "000";
                if (vm_s.Length > 5)
                    vm_s = vm_s.Remove(5);

                if (team.IsStillInTheCompetition)
                    qe = FRCPrinterConstants.QUALIFIED;
                else if (team.Abandoned)
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
                    graphics.DrawString(ind, font2, XBrushes.Black, x + 377 - (ind.Length - 1) * 5, currentYCoordinate);
                else
                    graphics.DrawString(ind, font2, XBrushes.Black, x + 380 - ind.Length * 5, currentYCoordinate);
                graphics.DrawString(hs, font2, XBrushes.Black, x + 412 - hs.Length * 5, currentYCoordinate);
                graphics.DrawString(qe, font2, XBrushes.Black, x + 425, currentYCoordinate);

                currentYCoordinate += font2Height * 1.3;
            }

            qualifiedEnd = false;
            eliminatedStarted = false;
            graphics.Dispose();
        }

        /// <summary>
        /// Prints a summing ranking of all poule rounds if needed.
        /// </summary>
        private void printRankingAfterAllPouleRounds()
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
            FRCTeam team;
            int amountOfTeams = rankListAllPouleRounds.Count;
            for (int i = 0; i < rankListAllPouleRounds.Count; i++)
            {
                if (rankListAllPouleRounds[i].Forfait || rankListAllPouleRounds[i].Excluded)
                    amountOfTeams--;
            }

            string title = FRCPrinterConstants.RANKING_AFTER_ALL_POULE_ROUNDS + " (" + amountOfTeams.ToString() 
                + " " + FRCPrinterConstants.TEAMS_LOWER + ")";
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

            for (int i = 0; i < rankListAllPouleRounds.Count; i++)
            {
                team = rankListAllPouleRounds[i];
                //Skip if the team got scratch of exclusion.
                if (team.PhaseFinalRanking == 0)
                    continue;

                double totalY = currentYCoordinate + font2Height * 1.3;
                graphics = checkYCoordinate(graphics, totalY);

                string pr = team.PhaseFinalRanking.ToString();
                string name = team.Name;
                string club = team.Club;
                string ind = team.Index.ToString();
                string hs = team.HitsGivenInPoule.ToString();
                string qe;
                string vm_s = team.VM.ToString().Replace(',', '.');
                if (vm_s.Length == 1)
                    vm_s += ".";
                vm_s += "000";
                if (vm_s.Length > 5)
                    vm_s = vm_s.Remove(5);

                if (team.IsStillInTheCompetition)
                    qe = FRCPrinterConstants.QUALIFIED;
                else if (team.Abandoned)
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
                    graphics.DrawString(ind, font2, XBrushes.Black, x + 377 - (ind.Length - 1) * 5, currentYCoordinate);
                else
                    graphics.DrawString(ind, font2, XBrushes.Black, x + 380 - ind.Length * 5, currentYCoordinate);
                graphics.DrawString(hs, font2, XBrushes.Black, x + 412 - hs.Length * 5, currentYCoordinate);
                graphics.DrawString(qe, font2, XBrushes.Black, x + 425, currentYCoordinate);

                currentYCoordinate += font2Height * 1.3;
            }

            qualifiedEnd = false;
            eliminatedStarted = false;
            graphics.Dispose();
        }

        /// <summary>
        /// Assigns appropriate title to the parameter.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="tableauLength">Length of tableau part.</param>
        private void assignTitle(int tableauLength, out string title)
        {
            if (tableauCounter == 0)
                if (tableauLength == 2)
                    title = FRCPrinterConstants.Final;
                else if (tableauLength == 4)
                    title = FRCPrinterConstants.SEMI_FINALS;
                else
                    title = FRCPrinterConstants.TABLE_OF + " " + tableauLength.ToString();
            else if (tableauCounter == 1)
                if (tableauLength == 8)
                    title = FRCPrinterConstants.TABLE_9_16;
                else if (tableauLength == 4)
                    title = FRCPrinterConstants.TABLE_9_12;
                else
                    title = FRCPrinterConstants.PLACE9;
            else if (tableauCounter == 2)
                if (tableauLength == 4)
                    title = FRCPrinterConstants.TABLE_13_16;
                else
                    title = FRCPrinterConstants.PLACE13;
            else if (tableauCounter == 3)
                title = FRCPrinterConstants.PLACE15;
            else if (tableauCounter == 4)
                title = FRCPrinterConstants.PLACE11;
            else if (tableauCounter == 5)
                if (tableauLength == 4)
                    title = FRCPrinterConstants.TABLE_5_8;
                else
                    title = FRCPrinterConstants.PLACE5;
            else if (tableauCounter == 6)
                title = FRCPrinterConstants.PLACE7;
            else if (tableauCounter == 7)
                title = FRCPrinterConstants.PLACE3;
            else
                title = "";
        }

        /// <summary>
        /// Prints the tableau.
        /// </summary>
        /// <param name="tableau">The tableau to print.</param>
        private void printTableau(FRCTableau tableau)
        {
            tableauStarted = true;
            if (noFencerInTableau(tableau))
                return;

            //Starting on a new page if the given page is not empty.
            if (currentYCoordinate > pageTopSize)
                currentYCoordinate = 810;

            XGraphics graphics = XGraphics.FromPdfPage(getCurrentPage());
            XFont font = new XFont(FONT_TYPE, 10);
            XFont fontClub = new XFont(FONT_TYPE, 9);
            double fontHeight = font.GetHeight();
            double x = DEFAULT_START_X;
            double titleY = currentYCoordinate;
            double tableauWidth = TABLEAU_WIDTH;
            double tableauSpace = TABLEAU_SPACE;
            string title;
            pageAdded = false;
            tableau.checkRankingTEAM();

            FRCTableauPart tableauPart = tableau.getNextTableauPart();
            assignTitle(tableauPart.Length, out title);
            
            if (tableauPart.Length > 16)
            {
                font = new XFont(FONT_TYPE, 8.5);
                fontClub = new XFont(FONT_TYPE, 7);
                fontHeight = font.GetHeight();
                tableauSpace = tableauSpace * 0.68;
                tableauWidth = tableauWidth * 0.9;
            }
            
            graphics.DrawString(title, font, XBrushes.Black, (x + tableauWidth) / 2, titleY, XStringFormats.TopLeft);
            currentYCoordinate += fontHeight;                                
            tableauEndPoint.Clear();
            //Draws the first part of tableau on the page.
            drawTableauPart(graphics, font, fontClub, tableauPart, tableau.amountOfTeams(), tableauWidth, tableauSpace,
                x, currentYCoordinate, title, titleY);
            x += tableauWidth * 1.8;
            tableauWidth = tableauWidth * 0.7;

            //Index for building tableau.
            int tableauBreakIdx;
            assignTableauBreakIndex(tableauPart, out tableauBreakIdx);

            //Builds with several tableau parts on the first part.
            for (int i = 0; i < tableauBreakIdx; i++)
            {
                if (tableau.hasNextTableauPart())
                {
                    tableauPart = tableau.getNextTableauPart();
                    assignTitle(tableauPart.Length, out title);

                    pageIdx = document.Pages.Count - tableauPageCounter;
                    graphics = XGraphics.FromPdfPage(document.Pages[pageIdx]);
                    graphics.DrawString(title, font, XBrushes.Black, (x + tableauWidth) / 1.9, titleY, XStringFormats.TopLeft);
                    drawTableauPartOnTableauPart(graphics, font, tableauPart, tableauWidth, tableauSpace, i + 1, title,(x + tableauWidth) / 1.9, titleY);
                    x += tableauWidth * 2;
                }
            }

            tableauPageCounter = 0;
            graphics.Dispose();
        }

        /// <summary>
        /// Return true if there is no fencer in given tableau.
        /// </summary>
        /// <param name="tableau"></param>
        /// <returns></returns>
        private bool noFencerInTableau(FRCTableau tableau)
        {
            bool noFencer = true;
            FRCTableauPart tPart;
            FRCMatch match;
            for (int i = 0; i < tableau.amountOfTableauParts(); i++)
            {
                tPart = tableau.getNextTableauPart();
                for (int j = 0; j < tPart.amountOfMatches(); j++)
                {
                    match = tPart.getNextMatch();
                    if (match.FencerID1 != 0 || match.FencerID2 != 0)
                        noFencer = false;
                }
            }

            return noFencer;
        }

        private void assignTableauBreakIndex(FRCTableauPart tableauPart, out int tableauBreakIdx)
        {
            if (tableauPart.Length == 128)
                tableauBreakIdx = 2;
            else if (tableauPart.Length == 64)
                tableauBreakIdx = 2;
            else if (tableauPart.Length == 32)
                tableauBreakIdx = 1;
            else
                tableauBreakIdx = 3;
        }

        private void drawTableauPart(XGraphics graphics, XFont font, XFont fontClub, FRCTableauPart tableauPart, int qualifiers,
            double tableauWidth, double tableauSpace, double x, double y, string title, double titleY)
        {
            FRCMatch match;
            FRCTeam team1, team2, winner;
            double fontHeight = font.GetHeight();
            double totalY;
            currentYCoordinate = y;
            int endPointIdx = 0;
            int rank1, rank2;
            int tableauSize = tableauPart.Length;

            interpreter.Tableau.calculateTableauNumbers(tableauSize);

                for (int i = 0; i < tableauSize; i += 2)
                {
                    totalY = currentYCoordinate + tableauSpace * 2;
                    graphics = checkYCoordinate(graphics, totalY);
                    currentYCoordinate += tableauSpace;
                    
                    if (pageAdded)
                    {
                        graphics.DrawString(title, font, XBrushes.Black, (x + tableauWidth) / 2, titleY, XStringFormats.TopLeft);
                        currentYCoordinate += fontHeight;
                        pageAdded = false;
                    }                    
                    
                    string name1, name2, name3, nat1, nat2, score;
                    rank1 = interpreter.Tableau.getTableauNumber(i);
                    rank2 = interpreter.Tableau.getTableauNumber(i + 1);

                    match = tableauPart.getNextMatch();

                    if (match.FencerID1 == 0)
                        team1 = null;
                    else
                        team1 = getTeamFromTableauMatchingID(match.FencerID1);

                    if (match.FencerID2 == 0)
                        team2 = null;
                    else
                        team2 = getTeamFromTableauMatchingID(match.FencerID2);

                    if (team1 == null)
                    {
                        name1 = "----------";
                        nat1 = "";
                    }
                    else
                    {
                        name1 = team1.Name;
                        nat1 = team1.Nationality;
                        if (name1.Length > 17)
                            name1 = name1.Remove(17);
                    }
                    if (team2 == null)
                    {
                        name2 = "----------";
                        nat2 = "";
                    }
                    else
                    {
                        name2 = team2.Name;
                        nat2 = team2.Nationality;
                        if (name2.Length > 17)
                            name2 = name2.Remove(17);
                    }

                    if (match.Fencer1Win)
                    {
                        winner = team1;
                        name3 = name1;
                        if (match.Fencer2Abandon)
                            score = "by abandonment";
                        else if (match.Fencer2Forfait)
                            score = "by scratch";
                        else if (match.Fencer2Exclusion)
                            score = "by exclusion";
                        else if (team2 == null)
                            score = "";
                        else
                            score = match.ScoreFencer1.ToString() + "/" + match.ScoreFencer2.ToString();
                    }
                    else
                    {
                        winner = team2;
                        name3 = name2;
                        if (match.Fencer1Abandon)
                            score = "by abandonment";
                        else if (match.Fencer1Forfait)
                            score = "by scratch";
                        else if (match.Fencer1Exclusion)
                            score = "by exclusion";
                        else if (team1 == null)
                            score = "";
                        else
                            score = match.ScoreFencer2.ToString() + "/" + match.ScoreFencer1.ToString();
                    }


                    if (name3.Length > 18)
                        name3 = name3.Remove(18);

                    tableauEndPoint.Add(drawTableauLines(graphics, new XPoint(x, currentYCoordinate), tableauSpace, tableauWidth,
                        tableauWidth * 0.7));
                    graphics.DrawString(rank1.ToString(), font, XBrushes.Black, x, currentYCoordinate - 2);
                    graphics.DrawString(name1, font, XBrushes.Black, x + 20, currentYCoordinate - 2);
                    graphics.DrawString(nat1, fontClub, XBrushes.Black, x + 115, currentYCoordinate - 2);
                    currentYCoordinate += tableauSpace;
                    graphics.DrawString(rank2.ToString(), font, XBrushes.Black, x, currentYCoordinate - 2);
                    graphics.DrawString(name2, font, XBrushes.Black, x + 20, currentYCoordinate - 2);
                    graphics.DrawString(nat2, fontClub, XBrushes.Black, x + 115, currentYCoordinate - 2);
                                    
                    XPoint point = tableauEndPoint[endPointIdx];
                    endPointIdx++;
                    graphics.DrawString(name3, font, XBrushes.Black, point.X + 5, point.Y - 2);
                    graphics.DrawString(score, font, XBrushes.Black, point.X + 15, point.Y + fontHeight);
                }
            
            graphics.Dispose();
        }

        private void drawTableauPartOnTableauPart(XGraphics graphics, XFont font, FRCTableauPart tableauPart, double tableauWidth,
            double tableauSpace, int partNumber, string title, double titleX, double titleY)
        {
            FRCMatch match;
            FRCTeam team1, team2, winner;
            double fontHeight = font.GetHeight();
            int endPointIdx = 0;
            int tableauSize = tableauPart.Length;
            int iterationsPerPage = tableauSize / tableauPageCounter;

            for (int i = 0; i < tableauSize; i += 2)
            {
                XPoint point = tableauEndPoint[endPointIdx];
                if ((i >= iterationsPerPage) && (i % iterationsPerPage == 0))
                {
                    graphics.Dispose();
                    pageIdx++;
                    graphics = XGraphics.FromPdfPage(document.Pages[pageIdx]);
                    graphics.DrawString(title, font, XBrushes.Black, titleX, titleY, XStringFormats.TopLeft);
                }

                string name, score;
                
                tableauEndPoint[endPointIdx] = drawTableauLines(graphics, point, tableauSpace * Math.Pow(2, partNumber), tableauWidth, tableauWidth);
                XPoint endPoint = tableauEndPoint[endPointIdx];
                endPointIdx += (int) Math.Pow(2, partNumber);

                match = tableauPart.getNextMatch();
                if (match.FencerID1 == 0)
                    team1 = null;
                else
                    team1 = getTeamFromTableauMatchingID(match.FencerID1);

                if (match.FencerID2 == 0)
                    team2 = null;
                else
                    team2 = getTeamFromTableauMatchingID(match.FencerID2);

                if (match.Fencer1Win)
                {
                    winner = team1;
                    if (winner == null)
                        name = "----------";
                    else
                        name = team1.Name;

                    if (match.Fencer2Abandon)
                        score = "by abandonment";
                    else if (match.Fencer2Forfait)
                        score = "by scratch";
                    else if (match.Fencer2Exclusion)
                        score = "by exclusion";
                    else if (team2 == null)
                        score = "";
                    else
                        score = match.ScoreFencer1.ToString() + "/" + match.ScoreFencer2.ToString();
                }
                else
                {
                    winner = team2;
                    if (winner == null)
                        name = "----------";
                    else
                        name = team2.Name;

                    if (match.Fencer1Abandon)
                        score = "by abandonment";
                    else if (match.Fencer1Forfait)
                        score = "by scratch";
                    else if (match.Fencer1Exclusion)
                        score = "by exclusion";
                    else if (team1 == null)
                        score = "";
                    else
                        score = match.ScoreFencer2.ToString() + "/" + match.ScoreFencer1.ToString();
                }

                if (name.Length > 18)
                    name = name.Remove(18);

                graphics.DrawString(name, font, XBrushes.Black, endPoint.X + 5, endPoint.Y - 2);
                graphics.DrawString(score, font, XBrushes.Black, endPoint.X + 15, endPoint.Y + fontHeight);
            }

            graphics.Dispose();
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
            FRCTeam team, teamInList;
            int amountOfTeams = pouleRound.amountOfTeams();
            for (int i = 0; i < pouleRound.amountOfTeams(); i++)
            {
                team = pouleRound.getNextTeam();
                if (team.Forfait || team.Excluded)
                    amountOfTeams--;
            }

            string title = FRCPrinterConstants.RANKING_OF_POULES +  ", " + FRCPrinterConstants.ROUND + " " + FRCPrinterConstants.NUMBER +
                " " + pouleRound.RoundNumber.ToString() + " (" + amountOfTeams.ToString() + " " + FRCPrinterConstants.TEAMS_LOWER + ")";
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

            pouleRound.sortTeamPouleRanking();
            for (int i = 0; i < pouleRound.amountOfTeams(); i++)
            {
                team = pouleRound.getNextTeam();
                //Skip if the team got scratch of exclusion.
                if (team.PhaseFinalRanking == 0)
                    continue;
                
                teamInList = getTeamMatchingID(team.ID);
                double totalY = currentYCoordinate + font2Height * 1.3;
                graphics = checkYCoordinate(graphics, totalY);

                string pr = team.PhaseFinalRanking.ToString();
                string name = team.Name;
                string club = team.Club;
                string ind = (teamInList.HitsGivenInPoule - teamInList.HitsTakenInPoule).ToString();
                string hs = teamInList.HitsGivenInPoule.ToString();
                string qe;
                string vm_s = teamInList.VM_String;
                if (team.IsStillInTheCompetition)
                    qe = FRCPrinterConstants.QUALIFIED;
                else if (team.Abandoned)
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
                    graphics.DrawString(ind, font2, XBrushes.Black, x + 377 - (ind.Length - 1) * 5, currentYCoordinate);
                else
                    graphics.DrawString(ind, font2, XBrushes.Black, x + 380 - ind.Length * 5, currentYCoordinate);
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
                    double totalY = currentYCoordinate + tempFont.GetHeight() * (4.9 + 1.25 * poule.amountOfTeams());
                    graphics = checkYCoordinate(graphics, totalY);
                    printPoule(graphics, poule, j + 1);
                }

                /*
                registerEliminationOnRankList(pouleRound[i]);
                if (i == pouleRound.Count - 1)
                {
                    checkTableauRankFormat(pouleRound[i]);

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
                */
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

            drawPouleGraph(graphics, x + 250, currentYCoordinate, poule.amountOfTeams());
            double savedY = currentYCoordinate;
            currentYCoordinate += fontHeight;
            poule.sortTeamsFencerNumberInPoule();
            for (int i = 0; i < poule.amountOfTeams(); i++)
            {
                FRCTeam team = poule.getNextTeam();
                copyTeamInfo(team);
                registerPouleResult(team);
                string name = team.Name;
                string club = team.Club;
                double vm = team.VM;
                string vm_s = vm.ToString().Replace(',', '.');
                string ind = team.Index.ToString();
                string hs = team.HitsGivenInPoule.ToString();
                string rank = team.PouleRanking.ToString();
                if (vm_s.Length == 1)
                    vm_s += ".";
                vm_s += "000";
                if (vm_s.Length > 5)
                    vm_s = vm_s.Remove(5);
                //Saving the value for team list in this class.
                getTeamMatchingID(team.ID).VM_String = vm_s;
                
                if (name.Length > 30)
                    name = name.Remove(30);
                if (club.Length > 15)
                    club = club.Remove(15);

                if (team.Abandoned || team.Forfait || team.Excluded)
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
        /// Fills a poule protocole with given poule at given coordinates.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="font"></param>
        /// <param name="poule"></param>
        /// <param name="x">The x coordinate where a poule is already drawn.</param>
        /// <param name="y">The y coordinate where a poule is already drawn.</param>
        private void fillPouleProtocole(XGraphics graphics, XFont font, FRCPoule poule, double x, double y)
        {
            int length = poule.amountOfTeams();
            double fontHeight = font.GetHeight();
            double pouleRectWidth = POULE_TOTAL_WIDTH / length;
            FRCTeam team1, team2;
            FRCMatch match;
            string res1, res2;
            double x1, y1, x2, y2;
            y += fontHeight;

            for(int i=0; i<poule.amountOfMatches(); i++)
            {
                res1 = "";
                res2 = "";
                match = poule.getNextMatch();
                team1 = getTeamMatchingID(match.FencerID1);
                team2 = getTeamMatchingID(match.FencerID2);

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

                y1 = y + (team1.FencerNumberInPoule - 1) * fontHeight * 1.25;
                y2 = y + (team2.FencerNumberInPoule - 1) * fontHeight * 1.25;
                x1 = x + pouleRectWidth * (team2.FencerNumberInPoule - 1);
                x2 = x + pouleRectWidth * (team1.FencerNumberInPoule - 1);
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

        /*
        /// <summary>
        /// Checks which format is used for tableau ranking.
        /// </summary>
        /// <param name="lastPouleRound">The last poule round.</param>
        private void checkTableauRankFormat(FRCPouleRound lastPouleRound)
        {
            lastPouleRound.sortTeamPouleRanking();
            interpreter.Tableau.sortTeamInitialRanking();
            sortRankListAllPouleRound();
            FRCTeam pouleTeam, tableauTeam;
            //Compare ranking from last poule round with initial ranking for the tableau.
            for (int i = 0; i < lastPouleRound.amountOfTeams(); i++)
            {
                pouleTeam = lastPouleRound.getNextTeam();
                //Skips all eliminated, excluded, abandoned and scratched teams.
                if (!pouleTeam.IsStillInTheCompetition)
                    continue;

                tableauTeam = interpreter.Tableau.getNextTeam();
                if (tableauTeam.ID != pouleTeam.ID)
                {
                    lastPouleRound.resetTeamIndex();
                    interpreter.Tableau.resetTeamIndex();
                    qualifyLastRankAll = true;
                    return;
                }
            }
           

            //Compare ranking from last poule round with internal summing ranking of all poule rounds.
            for (int i = 0; i < rankListAllPouleRounds.Count; i++)
            {
                pouleTeam = lastPouleRound.getNextTeam();
                //Skips all eliminated, excluded, abandoned and scratched teams.
                if(pouleTeam.IsStillInTheCompetition && rankListAllPouleRounds[i].IsStillInTheCompetition)
                    if (rankListAllPouleRounds[i].ID != pouleTeam.ID)
                    {
                        lastPouleRound.resetTeamIndex();
                        qualifyLastRankLast = true;
                        return;
                    }
            }

            lastPouleRound.resetTeamIndex();
            qualifyAllRankAll = true;
        }
        */

        private FRCTeam getTeamFromTableauMatchingInitialRanking(int rank)
        {
            FRCTeam team;
            for(int i=0; i<interpreter.Tableau.amountOfTeams(); i++)
            {
                team = interpreter.Tableau.getNextTeam();
                if (team.PhaseInitialRanking == rank)
                    return team;
            }
            return null;
        }
        
        private FRCTeam getTeamFromTableauMatchingID(int id)
        {
            FRCTeam team;
            for (int i = 0; i < interpreter.Tableau.amountOfTeams(); i++)
            {
                team = interpreter.Tableau.getNextTeam();
                if (team.ID == id)
                    return team;
            }

            return null;
        }

        /// <summary>
        /// Copies the IsStillInCompetition value from all teams in given poule round to the internal rank list.
        /// </summary>
        /// <param name="pouleRound"></param>
        private void registerEliminationOnRankList(FRCPouleRound pouleRound)
        {
            FRCTeam team;
            for (int i = 0; i < pouleRound.amountOfTeams(); i++)
            {
                team = pouleRound.getNextTeam();
                for (int j = 0; j < rankListAllPouleRounds.Count; j++)
                {
                    if (team.ID == rankListAllPouleRounds[j].ID)
                    {
                        rankListAllPouleRounds[j].IsStillInTheCompetition = team.IsStillInTheCompetition;
                        break;
                    }
                }
            }
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
        /// Registers result from poule round for given team to an internal ranking of all poule rounds.
        /// </summary>
        /// <param name="team"></param>
        private void registerPouleResult(FRCTeam team)
        {
            for(int i=0; i<this.rankListAllPouleRounds.Count; i++)
                if (this.rankListAllPouleRounds[i].ID == team.ID)
                {
                    this.rankListAllPouleRounds[i].IsStillInTheCompetition = team.IsStillInTheCompetition;
                    this.rankListAllPouleRounds[i].Forfait = team.Forfait;
                    this.rankListAllPouleRounds[i].Abandoned = team.Abandoned;
                    this.rankListAllPouleRounds[i].Excluded = team.Excluded;
                    this.rankListAllPouleRounds[i].HitsGivenInPoule += team.HitsGivenInPoule;
                    this.rankListAllPouleRounds[i].HitsTakenInPoule += team.HitsTakenInPoule;
                    this.rankListAllPouleRounds[i].NumberOfMatchesInPoule += team.NumberOfMatchesInPoule;
                    this.rankListAllPouleRounds[i].NumberOfVictoriesInPoule += team.NumberOfVictoriesInPoule;

                    return;
                }
        }

        /// <summary>
        /// Copying info from given team to the teams in the list of this class.
        /// </summary>
        /// <param name="team"></param>
        private void copyTeamInfo(FRCTeam team)
        {
            for (int i = 0; i < this.team.Count; i++)
                if (this.team[i].ID == team.ID)
                {
                    this.team[i] = (FRCTeam) team.Clone();
                    return;
                }
        }

        /// <summary>
        /// Prints a page with all fencers in each team.
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
            int amountOfFencers = 0;
            for (int i = 0; i < team.Count; i++)
                amountOfFencers += team[i].amountOfFencers();

            string title = FRCPrinterConstants.FENCERS_UPPER + " (" + amountOfFencers + " " + FRCPrinterConstants.FENCERS_LOWER + ")";
            graphics.DrawString(title, font1, XBrushes.Black, x, currentYCoordinate);
            currentYCoordinate += font1Height * 2;
            x += 5;
            graphics.DrawString(FRCPrinterConstants.NAME, font2, XBrushes.Black, x, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.NATION, font2, XBrushes.Black, x + 190, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.TEAM, font2, XBrushes.Black, x + 240, currentYCoordinate);
            currentYCoordinate += font2Height * 2;

            for (int i = 0; i < team.Count; i++)
                for(int j=0; j<team[i].amountOfFencers(); j++)
                {
                    FRCFencer tempFencer = team[i].getNextFencer();

                    double totalY = currentYCoordinate + font2Height * 1.3;
                    graphics = checkYCoordinate(graphics, totalY);
                    
                    string name = tempFencer.LastName.ToUpper() + " " + tempFencer.FirstName;
                    string team_s = team[i].Name;
                    if (name.Length > 37)
                        name = name.Remove(37);
                    if (team_s.Length > 100)
                        team_s = team_s.Remove(100);

                    graphics.DrawString(name, font2, XBrushes.Black, x, currentYCoordinate);
                    graphics.DrawString(tempFencer.Nationality, font2, XBrushes.Black, x + 190, currentYCoordinate);
                    graphics.DrawString(team_s, font2, XBrushes.Black, x + 240, currentYCoordinate);
                    currentYCoordinate += font2Height * 1.3;
                }
            graphics.Dispose();
        }

        /// <summary>
        /// Prints a page with all teams with their initial rankings.
        /// </summary>
        private void printStartingTeamsPage()
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

            string title = FRCPrinterConstants.INITIAL_RANKING + " (" + team.Count + " " + FRCPrinterConstants.TEAMS_LOWER + ")";
            graphics.DrawString(title, font1, XBrushes.Black, x, currentYCoordinate);
            currentYCoordinate += font1Height * 2;
            x += 5;
            graphics.DrawString(FRCPrinterConstants.RANKING, font2, XBrushes.Black, x, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.TEAM, font2, XBrushes.Black, x + 45, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.NATION, font2, XBrushes.Black, x + 235, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.CLUB, font2, XBrushes.Black, x + 285, currentYCoordinate);
            currentYCoordinate += font2Height * 2;

            sortTeamInitialRanking();
            for (int i = 0; i < team.Count; i++)
            {
                double totalY = currentYCoordinate + font2Height * 1.3;
                graphics = checkYCoordinate(graphics, totalY);

                string ir = team[i].InitialRanking.ToString();
                graphics.DrawString(ir, font2, XBrushes.Black, x + 30 - ir.Length * 5, currentYCoordinate);

                string name = team[i].Name;
                string club = team[i].Club;
                if (name.Length > 37)
                    name = name.Remove(37);
                if (club.Length > 15)
                    club = club.Remove(15);

                graphics.DrawString(name, font2, XBrushes.Black, x + 45, currentYCoordinate);
                graphics.DrawString(team[i].Nationality, font2, XBrushes.Black, x + 235, currentYCoordinate);
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
            string s1 = team.Count.ToString() + " " + FRCPrinterConstants.TEAMS_LOWER;
            graphics.DrawString(s1, font2, XBrushes.Black, x, currentYCoordinate);
            currentYCoordinate += font2Height * 3;

            for (int i = 0; i < pouleRound.Count; i++)
            {
                double totalY = currentYCoordinate + font2Height * 4.6;
                graphics = checkYCoordinate(graphics, totalY);

                string s2 = FRCPrinterConstants.POULES_ROUND + " " + (i + 1).ToString() + ":";
                graphics.DrawString(s2, font2, XBrushes.Black, x, currentYCoordinate);
                currentYCoordinate += font2Height * 2;
                string s3 = pouleRound[i].amountOfTeams().ToString() + " " + FRCPrinterConstants.TEAMS_LOWER;
                graphics.DrawString(s3, font2, XBrushes.Black, x, currentYCoordinate);
                currentYCoordinate += font2Height * 1.3;
                string s4 = pouleRound[i].amountOfPoules().ToString() + " " + FRCPrinterConstants.POULES_LOWER + " ";
                graphics.DrawString(s4, font2, XBrushes.Black, x, currentYCoordinate);
                currentYCoordinate += font2Height * 1.3;
                string s5 = pouleRound[i].NumOfQualifiers.ToString() + " " + FRCPrinterConstants.QUALIFIERS;
                graphics.DrawString(s5, font2, XBrushes.Black, x, currentYCoordinate);
                currentYCoordinate += font2Height * 2;
            }

            if (interpreter.Tableau.amountOfTeams() > 0)
            {
                double totalY = currentYCoordinate + font2Height * 3;
                graphics = checkYCoordinate(graphics, totalY);

                string s6 = "----------------------------------------------------";
                graphics.DrawString(s6, font2, XBrushes.Black, x, currentYCoordinate);
                currentYCoordinate += font2Height * 3;
                string s7 = FRCPrinterConstants.DIRECT_ELIMINATION + ": " + interpreter.Tableau.amountOfTeams().ToString() +
                    " " + FRCPrinterConstants.TEAMS_LOWER;
                graphics.DrawString(s7, font2, XBrushes.Black, x, currentYCoordinate);
                currentYCoordinate += font2Height * 2;
            }
            graphics.Dispose();
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
            int amountOfTeams = team.Count;

            for (int i = 0; i < team.Count; i++)
                if (team[i].Forfait || team[i].Excluded)
                    amountOfTeams--;

            string title = FRCPrinterConstants.FINAL_RESULTS + " (" + amountOfTeams.ToString() + " " +
                FRCPrinterConstants.TEAMS_LOWER + ")";

            graphics.DrawString(title, font1, XBrushes.Black, x, currentYCoordinate);
            currentYCoordinate += font1Height * 2;
            x += 5;
            graphics.DrawString(FRCPrinterConstants.PLACE, font2, XBrushes.Black, x, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.TEAM, font2, XBrushes.Black, x + 30, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.NATION, font2, XBrushes.Black, x + 220, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.CLUB, font2, XBrushes.Black, x + 270, currentYCoordinate);
            currentYCoordinate += font2Height * 2;

            sortTeamFinalRanking();
            for (int i = 0; i < team.Count; i++)
            {
                if (team[i].FinalRanking == 0)
                    continue;

                double totalY = currentYCoordinate + font2Height * 1.3;
                graphics = checkYCoordinate(graphics, totalY);

                string fr = team[i].FinalRanking.ToString();
                graphics.DrawString(fr, font2, XBrushes.Black, x + 20 - fr.Length * 5, currentYCoordinate);

                string name = team[i].Name;
                string club = team[i].Club;
                if (name.Length > 37)
                    name = name.Remove(37);
                if (club.Length > 15)
                    club = club.Remove(15);

                graphics.DrawString(name, font2, XBrushes.Black, x + 30, currentYCoordinate);
                graphics.DrawString(team[i].Nationality, font2, XBrushes.Black, x + 220, currentYCoordinate);
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
        /// Sorts all teamss after their final placements.
        /// </summary>
        private void sortTeamFinalRanking()
        {
            for(int i=0; i<team.Count - 1; i++)
                for (int j = i+1; j < team.Count; j++)
                {
                    if (team[i].FinalRanking > team[j].FinalRanking)
                    {
                        FRCTeam tempTeam = team[j];
                        team[j] = team[i];
                        team[i] = tempTeam;
                    }
                }
        }

        /// <summary>
        /// Sorts all teams after their initial rankings.
        /// </summary>
        private void sortTeamInitialRanking()
        {
            for(int i=0; i<team.Count - 1; i++)
                for (int j = i+1; j < team.Count; j++)
                {
                    if (team[i].InitialRanking > team[j].InitialRanking)
                    {
                        FRCTeam tempTeam = team[j];
                        team[j] = team[i];
                        team[i] = tempTeam;
                    }
                }
        }

        /// <summary>
        /// Makes an ranking for all qualified teams from all poule rounds together.
        /// </summary>
        private void sortRankListAllPouleRound()
        {
            List<FRCTeam> tempRankList = new List<FRCTeam>();
            double highestVM;
            int highestIND, highestHGiven;
            int rank = 1;
            //Does not use the element in these sorted list at all actually, the key is the only important part since it becomes sorted.
            SortedList<double, FRCTeam> vmList = new SortedList<double, FRCTeam>();
            SortedList<int, FRCTeam> indList = new SortedList<int, FRCTeam>();
            SortedList<int, FRCTeam> hGivenList = new SortedList<int, FRCTeam>();
            while (rankListAllPouleRounds.Count > 0)
            {
                for (int i = 0; i < rankListAllPouleRounds.Count; i++)
                {
                    rankListAllPouleRounds[i].VM = (double)rankListAllPouleRounds[i].NumberOfVictoriesInPoule / 
                        (double)rankListAllPouleRounds[i].NumberOfMatchesInPoule;

                    vmList[rankListAllPouleRounds[i].VM] = (FRCTeam)rankListAllPouleRounds[i].Clone();
                }
                highestVM = vmList.Keys[vmList.Count - 1];

                for (int i = 0; i < rankListAllPouleRounds.Count; i++)
                {
                    if (rankListAllPouleRounds[i].VM == highestVM)
                    {
                        rankListAllPouleRounds[i].Index = rankListAllPouleRounds[i].HitsGivenInPoule - 
                            rankListAllPouleRounds[i].HitsTakenInPoule;

                        indList[rankListAllPouleRounds[i].Index] = (FRCTeam)rankListAllPouleRounds[i].Clone();
                    }
                }
                highestIND = indList.Keys[indList.Count - 1];

                for (int i = 0; i < rankListAllPouleRounds.Count; i++)
                {
                    if(rankListAllPouleRounds[i].VM == highestVM)
                        if (rankListAllPouleRounds[i].Index == highestIND)
                        {
                            hGivenList[rankListAllPouleRounds[i].HitsGivenInPoule] = (FRCTeam)rankListAllPouleRounds[i].Clone();
                        }
                }
                highestHGiven = hGivenList.Keys[hGivenList.Count - 1];

                for (int i = 0; i < rankListAllPouleRounds.Count; i++)
                {
                    if(rankListAllPouleRounds[i].VM == highestVM)
                        if(rankListAllPouleRounds[i].Index == highestIND)
                            if (rankListAllPouleRounds[i].HitsGivenInPoule == highestHGiven)
                            {
                                rankListAllPouleRounds[i].PhaseFinalRanking = rank;
                                tempRankList.Add(rankListAllPouleRounds[i]);
                                rankListAllPouleRounds.RemoveAt(i);
                                i--;
                            }
                }
                vmList.Clear();
                indList.Clear();
                hGivenList.Clear();
                rank++;
            }
            rankListAllPouleRounds = tempRankList;
        }

        /// <summary>
        /// Prints the comptition title on the top of the pdf page. This method should be called once for each new page.
        /// </summary>
        private void printPageTop()
        {
            XGraphics graphics = XGraphics.FromPdfPage(currentPage);
            XFont font1 = new XFont(FONT_TYPE, 12);
            XFont font2 = new XFont(FONT_TYPE, 11);
            XFont pageNumberFont = new XFont(FONT_TYPE, 9);
            //Prints page number.
            string pageNumber = document.PageCount.ToString();
            double y = 20;
            double x;
            if (pageNumber.Length > 1)
                x = 572;
            else
                x = 575;
            graphics.DrawString(pageNumber, pageNumberFont, XBrushes.Black, x, y);

            string title = interpreter.Formula.CompetitionName;
            string[] str;
            str = title.Split('-');
            x = 300;
            y = 30;
            double fontHeight;

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

            y += font2.GetHeight() * 2;
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
        /// <param name="length">Amount of teams in the poule.</param>
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

        /// <summary>
        /// Prints referee activity page.
        /// </summary>
        private void printRefereeActivity()
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

            string title = FRCPrinterConstants.REFEREE_ACTIVITY;
            graphics.DrawString(title, font1, XBrushes.Black, x, currentYCoordinate);
            currentYCoordinate += font1Height * 2;
            x += 5;
            graphics.DrawString(FRCPrinterConstants.NAME, font2, XBrushes.Black, x, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.NATION, font2, XBrushes.Black, x + 190, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.CLUB, font2, XBrushes.Black, x + 235, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.CATEGORY, font2, XBrushes.Black, x + 320, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.POULES, font2, XBrushes.Black, x + 365, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.MATCHES, font2, XBrushes.Black, x + 410, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.QUARTER_FINALS, font2, XBrushes.Black, x + 455, currentYCoordinate);
            graphics.DrawString(FRCPrinterConstants.FINALS, font2, XBrushes.Black, x + 500, currentYCoordinate);
            currentYCoordinate += font2Height * 2;

            List<FRCReferee> referee = interpreter.getRefereeList();
            for (int i = 0; i < referee.Count; i++)
            {
                double totalY = currentYCoordinate + font2Height * 1.3;
                graphics = checkYCoordinate(graphics, totalY);

                string name = referee[i].LastName.ToUpper() + " " + referee[i].FirstName;
                string club = referee[i].Club;
                if (name.Length > 37)
                    name = name.Remove(37);
                if (club.Length > 15)
                    club = club.Remove(15);

                graphics.DrawString(name, font2, XBrushes.Black, x, currentYCoordinate);
                graphics.DrawString(referee[i].Nationality, font2, XBrushes.Black, x + 190, currentYCoordinate);
                graphics.DrawString(club, font2, XBrushes.Black, x + 235, currentYCoordinate);
                graphics.DrawString(referee[i].Category, font2, XBrushes.Black, x + 320, currentYCoordinate);
                graphics.DrawString(referee[i].RefereedPoules.ToString(), font2, XBrushes.Black, x + 365, currentYCoordinate);
                graphics.DrawString(referee[i].RefereedMatches.ToString(), font2, XBrushes.Black, x + 410, currentYCoordinate);
                graphics.DrawString(referee[i].RefereedQuarterFinals.ToString(), font2, XBrushes.Black, x + 455, currentYCoordinate);
                graphics.DrawString(referee[i].RefereedFinals.ToString(), font2, XBrushes.Black, x + 500, currentYCoordinate);
                currentYCoordinate += font2Height * 1.3;
            }

            graphics.Dispose();
        }
    }
}
