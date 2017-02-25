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
        /// <summary>
        /// Checks which format is used for tableau ranking.
        /// </summary>
        /// <param name="lastPouleRound">The last poule round.</param>
        private void checkTableauRankFormat(FRCPouleRound lastPouleRound)
        {
            lastPouleRound.sortFencerPouleRanking();
            interpreter.Tableau.sortFencerInitialRanking();
            sortRankListAllPouleRounds();
            FRCFencer pouleFencer, tableauFencer;

            //Compare ranking from last poule round with initial ranking for the tableau.
            for (int i = 0; i < lastPouleRound.amountOfFencers(); i++)
            {
                pouleFencer = lastPouleRound.getNextFencer();
                //Skips all eliminated, excluded, abandoned and scratched fencers.
                if (!pouleFencer.IsStillInTheCompetition)
                    continue;

                tableauFencer = interpreter.Tableau.getNextFencer();
                if (tableauFencer.ID != pouleFencer.ID)
                {
                    lastPouleRound.resetFencerIndex();
                    interpreter.Tableau.resetFencerIndex();
                    qualifyLastRankAll = true;
                    return;
                }
            }

            int lastPhaseFinalRanking = -1;
            bool different = false;
            //Compare ranking from last poule round with internal summing ranking of all poule rounds.
            for (int i = 0; i < rankListAllPouleRounds.Count; i++)
            {
                pouleFencer = lastPouleRound.getNextFencer();

                if (different)
                {
                    if (pouleFencer.PhaseFinalRanking == lastPhaseFinalRanking)
                    {
                        different = false;
                        continue;
                    }
                    lastPouleRound.resetFencerIndex();
                    qualifyLastRankLast = true;
                    return;
                }

                //Skips all eliminated, excluded, abandoned and scratched fencers.
                if (pouleFencer.IsStillInTheCompetition && rankListAllPouleRounds[i].IsStillInTheCompetition)
                {
                    if (rankListAllPouleRounds[i].ID != pouleFencer.ID)
                    {                        
                        different = true;
                    }
                }
                lastPhaseFinalRanking = pouleFencer.PhaseFinalRanking;
            }

            //If the last fencer is different.
            if (different)
            {
                qualifyLastRankLast = true;
                return;
            }

            lastPouleRound.resetFencerIndex();
            qualifyAllRankAll = true;
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
            FRCFencer fencer;
            int amountOfFencers = rankListAllPouleRounds.Count;
            for (int i = 0; i < rankListAllPouleRounds.Count; i++)
            {
                if (rankListAllPouleRounds[i].Forfait || rankListAllPouleRounds[i].Excluded)
                    amountOfFencers--;
            }

            string title = FRCPrinterConstants.RANKING_AFTER_ALL_POULE_ROUNDS + " (" + amountOfFencers.ToString()
                + " " + FRCPrinterConstants.FENCERS_LOWER + ")";
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
            //Rank all qualified fencers.
            for (int i = 0; i < rankListAllPouleRounds.Count; i++)
            {
                if (rankListAllPouleRounds[i].IsStillInTheCompetition)
                {
                    //Skip the assignment if the next fencer has same ranking.
                    if ((i - 1) >= 0)
                        if (rankListAllPouleRounds[i].PhaseFinalRanking == rankListAllPouleRounds[i - 1].PhaseFinalRanking)
                        {
                            rank++;
                            continue;
                        }

                    rankListAllPouleRounds[i].PhaseFinalRanking = rank;
                    rank++;
                }
            }
            //Rank all eliminated fencers.
            for (int i = 0; i < rankListAllPouleRounds.Count; i++)
            {
                if (rankListAllPouleRounds[i].Forfait || rankListAllPouleRounds[i].Excluded)
                    continue;

                if (!rankListAllPouleRounds[i].IsStillInTheCompetition)
                {
                    //Skip the assignment if next fencer has same ranking.
                    if ((i - 1) >= 0)
                        if (rankListAllPouleRounds[i].PhaseFinalRanking == rankListAllPouleRounds[i - 1].PhaseFinalRanking)
                        {
                            rank++;
                            continue;
                        }

                    rankListAllPouleRounds[i].PhaseFinalRanking = rank;
                    rank++;
                }
            }
            //Sort the fencers after their PhaseFinalRanking.
            for (int i = 0; i < rankListAllPouleRounds.Count - 1; i++)
                for (int j = i + 1; j < rankListAllPouleRounds.Count; j++)
                {
                    if (rankListAllPouleRounds[i].PhaseFinalRanking > rankListAllPouleRounds[j].PhaseFinalRanking)
                    {
                        FRCFencer tempFencer = rankListAllPouleRounds[j];
                        rankListAllPouleRounds[j] = rankListAllPouleRounds[i];
                        rankListAllPouleRounds[i] = tempFencer;
                    }
                }

            //Print all fencers.
            for (int i = 0; i < rankListAllPouleRounds.Count; i++)
            {
                fencer = rankListAllPouleRounds[i];
                //Skip if the fencer got scratch of exclusion.
                if (fencer.PhaseFinalRanking == 0)
                    continue;

                double totalY = currentYCoordinate + font2Height * 1.3;
                graphics = checkYCoordinate(graphics, totalY);

                string pr = fencer.PhaseFinalRanking.ToString();
                string name = fencer.LastName.ToUpper() + " " + fencer.FirstName;
                string club = fencer.Club;
                string ind = fencer.Index.ToString();
                string hs = fencer.HitsGivenInPoule.ToString();
                string qe;
                string vm_s = fencer.VM.ToString().Replace(',', '.');
                if (vm_s.Length == 1)
                    vm_s += ".";
                vm_s += "000";
                if (vm_s.Length > 5)
                    vm_s = vm_s.Remove(5);

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
        /// Makes a ranking for all qualified fencers from all poule rounds together.
        /// </summary>
        private void sortRankListAllPouleRounds()
        {
            List<FRCFencer> tempRankList = new List<FRCFencer>();
            //Save excluded, scratched and abandoned fencers in this list.
            List<FRCFencer> excScrAbbFencers = new List<FRCFencer>();
            double highestVM;
            int highestIND, highestHGiven;
            int rank = 1;
            //Does not use the element in these sorted list at all actually, the key is the only important part since it becomes sorted.
            SortedList<double, FRCFencer> vmList = new SortedList<double, FRCFencer>();
            SortedList<int, FRCFencer> indList = new SortedList<int, FRCFencer>();
            SortedList<int, FRCFencer> hGivenList = new SortedList<int, FRCFencer>();
            while (rankListAllPouleRounds.Count > 0)
            {
                //Counts fencers with same ranking.
                int sameRankCounter = 0;

                //Searches for highest VM.
                for (int i = 0; i < rankListAllPouleRounds.Count; i++)
                {
                    rankListAllPouleRounds[i].VM = (double)rankListAllPouleRounds[i].NumberOfVictoriesInPoule /
                        (double)rankListAllPouleRounds[i].NumberOfMatchesInPoule;

                    //When the XML-file is weird 0/0 occurs.
                    if (double.IsNaN(rankListAllPouleRounds[i].VM))
                        rankListAllPouleRounds[i].VM = 0.0;

                    vmList[rankListAllPouleRounds[i].VM] = (FRCFencer)rankListAllPouleRounds[i].Clone();
                }
                highestVM = vmList.Keys[vmList.Count - 1];

                //Searches for highest index.
                for (int i = 0; i < rankListAllPouleRounds.Count; i++)
                {
                    if (rankListAllPouleRounds[i].VM == highestVM)
                    {
                        rankListAllPouleRounds[i].Index = rankListAllPouleRounds[i].HitsGivenInPoule -
                            rankListAllPouleRounds[i].HitsTakenInPoule;

                        indList[rankListAllPouleRounds[i].Index] = (FRCFencer)rankListAllPouleRounds[i].Clone();
                    }
                }
                highestIND = indList.Keys[indList.Count - 1];

                //Searches for highest given hits.
                for (int i = 0; i < rankListAllPouleRounds.Count; i++)
                {
                    if (rankListAllPouleRounds[i].VM == highestVM)
                        if (rankListAllPouleRounds[i].Index == highestIND)
                        {
                            hGivenList[rankListAllPouleRounds[i].HitsGivenInPoule] = (FRCFencer)rankListAllPouleRounds[i].Clone();
                        }
                }
                highestHGiven = hGivenList.Keys[hGivenList.Count - 1];

                for (int i = 0; i < rankListAllPouleRounds.Count; i++)
                {
                    if (rankListAllPouleRounds[i].VM == highestVM)
                        if (rankListAllPouleRounds[i].Index == highestIND)
                            if (rankListAllPouleRounds[i].HitsGivenInPoule == highestHGiven)
                            {
                                //The abandoned, excluded and scratched fencers must be added at the end of the list.
                                if (rankListAllPouleRounds[i].Abandoned || rankListAllPouleRounds[i].Excluded ||
                                    rankListAllPouleRounds[i].Forfait)
                                {
                                    excScrAbbFencers.Add(rankListAllPouleRounds[i]);
                                    rankListAllPouleRounds.RemoveAt(i);
                                }
                                else
                                {
                                    rankListAllPouleRounds[i].PhaseFinalRanking = rank;
                                    tempRankList.Add(rankListAllPouleRounds[i]);
                                    rankListAllPouleRounds.RemoveAt(i);
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
                excScrAbbFencers[i].PhaseFinalRanking = rank;
                tempRankList.Add(excScrAbbFencers[i]);
                rank++;
            }

            rankListAllPouleRounds = tempRankList;
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
            FRCFencer fencer;
            int amountOfFencers = rankListAllPouleRounds.Count;
            for (int i = 0; i < rankListAllPouleRounds.Count; i++)
            {
                if (rankListAllPouleRounds[i].Forfait || rankListAllPouleRounds[i].Excluded)
                    amountOfFencers--;
            }

            string title = FRCPrinterConstants.RANKING_AFTER_ALL_POULE_ROUNDS + " (" + amountOfFencers.ToString()
                + " " + FRCPrinterConstants.FENCERS_LOWER + ")";
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
                fencer = rankListAllPouleRounds[i];
                //Skip if the fencer got scratch or exclusion.
                if (fencer.Forfait || fencer.Excluded)
                    continue;

                double totalY = currentYCoordinate + font2Height * 1.3;
                graphics = checkYCoordinate(graphics, totalY);

                string pr = fencer.PhaseFinalRanking.ToString();
                string name = fencer.LastName.ToUpper() + " " + fencer.FirstName;
                string club = fencer.Club;
                string ind = fencer.Index.ToString();
                string hs = fencer.HitsGivenInPoule.ToString();
                string qe;
                string vm_s = fencer.VM.ToString().Replace(',', '.');
                if (vm_s.Length == 1)
                    vm_s += ".";
                vm_s += "000";
                if (vm_s.Length > 5)
                    vm_s = vm_s.Remove(5);

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
        /// Prints the tableau.
        /// </summary>
        private void printTableau()
        {
            tableauStarted = true;
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
            //Checks and fixes if two fencers has same ranking.
            interpreter.Tableau.checkRankingIND();

            FRCTableauPart tableauPart = interpreter.Tableau.getNextTableauPart();
            if (tableauPart.Length == 2)
                title = FRCPrinterConstants.Final;
            else if (tableauPart.Length == 4)
                title = FRCPrinterConstants.SEMI_FINALS;
            else
                title = FRCPrinterConstants.TABLE_OF + " " + tableauPart.Length.ToString();

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
            drawTableauPart(graphics, font, fontClub, tableauPart, interpreter.Tableau.amountOfFencers(), tableauWidth, tableauSpace,
                x, currentYCoordinate, title, titleY);
            x += tableauWidth * 1.8;
            tableauWidth = tableauWidth * 0.7;

            //Index for building tableau.
            int tableauBreakIdx;
            assignTableauBreakIndex(tableauPart, out tableauBreakIdx);

            //Builds with several tableau parts on the first part.
            for (int i = 0; i < tableauBreakIdx; i++)
            {
                if (interpreter.Tableau.hasNextTableauPart())
                {
                    tableauPart = interpreter.Tableau.getNextTableauPart();
                    if (tableauPart.Length == 2)
                        title = FRCPrinterConstants.Final;
                    else if (tableauPart.Length == 4)
                        title = FRCPrinterConstants.SEMI_FINALS;
                    else
                        title = FRCPrinterConstants.TABLE_OF + " " + tableauPart.Length.ToString();

                    pageIdx = document.Pages.Count - tableauPageCounter;
                    graphics = XGraphics.FromPdfPage(document.Pages[pageIdx]);
                    graphics.DrawString(title, font, XBrushes.Black, (x + tableauWidth) / 1.9, titleY, XStringFormats.TopLeft);
                    drawTableauPartOnTableauPart(graphics, font, tableauPart, tableauWidth, tableauSpace, i + 1, title, (x + tableauWidth) / 1.9, titleY);
                    x += tableauWidth * 2;
                }
            }

            tableauPageCounter = 0;
            graphics.Dispose();
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

        /// <summary>
        /// Should be used to draw the first tableau part.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="font"></param>
        /// <param name="fontClub"></param>
        /// <param name="tableauPart"></param>
        /// <param name="qualifiers"></param>
        /// <param name="tableauWidth"></param>
        /// <param name="tableauSpace"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="title"></param>
        /// <param name="titleY"></param>
        private void drawTableauPart(XGraphics graphics, XFont font, XFont fontClub, FRCTableauPart tableauPart, int qualifiers,
            double tableauWidth, double tableauSpace, double x, double y, string title, double titleY)
        {
            FRCMatch match;
            FRCFencer fencer1, fencer2, winner;
            double fontHeight = font.GetHeight();
            double totalY;
            currentYCoordinate = y;
            int endPointIdx = 0;
            int rank1, rank2;
            int tableauSize = tableauPart.Length;

            interpreter.Tableau.calculateTableauNumbers(tableauSize);
            FRCFencer[] ft = fencerInTableau.ToArray();
            fencerInTableau.Clear();

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

                string name1, name2, name3, club1, club2, score;
                rank1 = interpreter.Tableau.getTableauNumber(i);
                rank2 = interpreter.Tableau.getTableauNumber(i + 1);
                if (!onlyOnce)
                {
                    fencer1 = getFencerFromTableauMatchingInitialRanking(rank1);
                    fencer2 = getFencerFromTableauMatchingInitialRanking(rank2);
                }
                else
                {
                    fencer1 = ft[i];
                    fencer2 = ft[i + 1];
                }

                if (fencer1 == null)
                {
                    name1 = "----------";
                    club1 = "";
                }
                else
                {
                    name1 = fencer1.LastName.ToUpper() + " " + fencer1.FirstName;
                    club1 = fencer1.Club;
                    if (name1.Length > 22)
                        name1 = name1.Remove(22);
                    if (club1.Length > 15)
                        club1 = club1.Remove(15);
                }
                if (fencer2 == null)
                {
                    name2 = "----------";
                    club2 = "";
                }
                else
                {
                    name2 = fencer2.LastName.ToUpper() + " " + fencer2.FirstName;
                    club2 = fencer2.Club;
                    if (name2.Length > 22)
                        name2 = name2.Remove(22);
                    if (club2.Length > 15)
                        club2 = club2.Remove(15);
                }

                if (fencer1 == null && fencer2 == null)
                {
                    winner = null;
                    name3 = "----------";
                    score = "";
                }
                else if (fencer1 == null)
                {
                    winner = fencer2;
                    name3 = fencer2.LastName.ToUpper() + " " + fencer2.FirstName;
                    score = "";
                }
                else if (fencer2 == null)
                {
                    winner = fencer1;
                    name3 = fencer1.LastName.ToUpper() + " " + fencer1.FirstName;
                    score = "";
                }
                else
                {
                    match = tableauPart.getNextMatch();
                    //Skip matches with REF=0 in extended format of xml file.
                    while (match.FencerID1 == 0 || match.FencerID2 == 0)
                        match = tableauPart.getNextMatch();

                    fencer1 = getFencerFromTableauMatchingID(match.FencerID1);
                    fencer2 = getFencerFromTableauMatchingID(match.FencerID2);
                    if (match.Fencer1Win)
                    {
                        winner = fencer1;
                        name3 = fencer1.LastName.ToUpper() + " " + fencer1.FirstName;
                        if (match.Fencer2Abandon)
                            score = "by abandonment";
                        else if (match.Fencer2Forfait)
                            score = "by scratch";
                        else if (match.Fencer2Exclusion)
                            score = "by exclusion";
                        else
                            score = match.ScoreFencer1.ToString() + "/" + match.ScoreFencer2.ToString();
                    }
                    else
                    {
                        winner = fencer2;
                        name3 = fencer2.LastName.ToUpper() + " " + fencer2.FirstName;
                        if (match.Fencer1Abandon)
                            score = "by abandonment";
                        else if (match.Fencer1Forfait)
                            score = "by scratch";
                        else if (match.Fencer1Exclusion)
                            score = "by exclusion";
                        else
                            score = match.ScoreFencer2.ToString() + "/" + match.ScoreFencer1.ToString();
                    }
                }

                if (name3.Length > 18)
                    name3 = name3.Remove(18);

                fencerInTableau.Add(winner);

                tableauEndPoint.Add(drawTableauLines(graphics, new XPoint(x, currentYCoordinate), tableauSpace, tableauWidth,
                    tableauWidth * 0.7));
                graphics.DrawString(rank1.ToString(), font, XBrushes.Black, x, currentYCoordinate - 2);
                graphics.DrawString(name1, font, XBrushes.Black, x + 20, currentYCoordinate - 2);
                graphics.DrawString(club1, fontClub, XBrushes.Black, x + 20, currentYCoordinate + fontHeight * 0.8);
                currentYCoordinate += tableauSpace;
                graphics.DrawString(rank2.ToString(), font, XBrushes.Black, x, currentYCoordinate - 2);
                graphics.DrawString(name2, font, XBrushes.Black, x + 20, currentYCoordinate - 2);
                graphics.DrawString(club2, fontClub, XBrushes.Black, x + 20, currentYCoordinate + fontHeight * 0.8);

                XPoint point = tableauEndPoint[endPointIdx];
                endPointIdx++;
                graphics.DrawString(name3, font, XBrushes.Black, point.X + 5, point.Y - 2);
                graphics.DrawString(score, font, XBrushes.Black, point.X + 15, point.Y + fontHeight);
            }
            onlyOnce = true;
            graphics.Dispose();
        }

        private void drawTableauPartOnTableauPart(XGraphics graphics, XFont font, FRCTableauPart tableauPart, double tableauWidth,
            double tableauSpace, int partNumber, string title, double titleX, double titleY)
        {
            FRCMatch match;
            FRCFencer fencer1, fencer2, winner;
            double fontHeight = font.GetHeight();
            int endPointIdx = 0;
            int tableauSize = tableauPart.Length;
            int iterationsPerPage = tableauSize / tableauPageCounter;

            FRCFencer[] ft = fencerInTableau.ToArray();
            fencerInTableau.Clear();

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

                fencer1 = ft[i];
                fencer2 = ft[i + 1];
                string name, score;

                tableauEndPoint[endPointIdx] = drawTableauLines(graphics, point, tableauSpace * Math.Pow(2, partNumber), tableauWidth, tableauWidth);
                XPoint endPoint = tableauEndPoint[endPointIdx];
                endPointIdx += (int)Math.Pow(2, partNumber);

                if (fencer1 == null && fencer2 == null)
                {
                    winner = null;
                    name = "----------";
                    score = "";
                }
                else if (fencer1 == null)
                {
                    winner = fencer2;
                    name = fencer2.LastName.ToUpper() + " " + fencer2.FirstName;
                    score = "";
                }
                else if (fencer2 == null)
                {
                    winner = fencer1;
                    name = fencer1.LastName.ToUpper() + " " + fencer1.FirstName;
                    score = "";
                }
                else
                {
                    match = tableauPart.getNextMatch();
                    fencer1 = getFencerFromTableauMatchingID(match.FencerID1);
                    fencer2 = getFencerFromTableauMatchingID(match.FencerID2);
                    if (match.Fencer1Win)
                    {
                        winner = fencer1;
                        name = fencer1.LastName.ToUpper() + " " + fencer1.FirstName;
                        if (match.Fencer2Abandon)
                            score = "by abandonment";
                        else if (match.Fencer2Forfait)
                            score = "by scratch";
                        else if (match.Fencer2Exclusion)
                            score = "by exclusion";
                        else
                            score = match.ScoreFencer1.ToString() + "/" + match.ScoreFencer2.ToString();
                    }
                    else
                    {
                        winner = fencer2;
                        name = fencer2.LastName.ToUpper() + " " + fencer2.FirstName;
                        if (match.Fencer1Abandon)
                            score = "by abandonment";
                        else if (match.Fencer1Forfait)
                            score = "by scratch";
                        else if (match.Fencer1Exclusion)
                            score = "by exclusion";
                        else
                            score = match.ScoreFencer2.ToString() + "/" + match.ScoreFencer1.ToString();
                    }
                }

                if (name.Length > 18)
                    name = name.Remove(18);

                fencerInTableau.Add(winner);
                graphics.DrawString(name, font, XBrushes.Black, endPoint.X + 5, endPoint.Y - 2);
                graphics.DrawString(score, font, XBrushes.Black, endPoint.X + 15, endPoint.Y + fontHeight);
            }
            graphics.Dispose();
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
