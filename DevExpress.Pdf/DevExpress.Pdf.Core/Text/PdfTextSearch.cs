#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace DevExpress.Pdf.Native {
	public class PdfTextSearch {
		static readonly Regex splitter = new Regex(@"(?<=[,.!@#$%^&*()+_=`\{\}\[\];:""<>\\/?\|-])|[\s]");
		static PdfTextLine FindLine(PdfWord word, IList<PdfTextLine> pageStrings) {
			foreach(PdfTextLine str in pageStrings)
				if (str.Contains(word))
					return str;
			return null;
		}
		static string CheckSeparators(string pageWord, string searchWord) {
			string pattern = @"[,\.!@#\$%&\^*()+_=`\{\}\[\];:""<>\\/?\|-]$";
			if (!Regex.IsMatch(searchWord, pattern) && Regex.IsMatch(pageWord, pattern))
				return pageWord.Remove(pageWord.Length - 1);
			return pageWord;
		}
		static void AddRectangle(IList<PdfOrientedRectangle> results, double left, double top, double right, double height, double angle) {
			results.Add(new PdfOrientedRectangle(PdfTextUtils.RotatePoint(new PdfPoint(left, top), angle), right - left, height, angle));
		}
		readonly List<PdfWord> foundWords = new List<PdfWord>();
		IList<PdfTextLine> pageLines;
		IList<PdfWord> pageWords;
		IList<string> searchWords;
		PdfTextSearchParameters searchParameters;
		PdfTextSearchResults lastSearchResults;
		StringComparison comparisonType;
		string searchString;
		int pageIndex;
		int wordIndex;
		int startPageIndex;
		int startWordIndex;
		bool searchStart = true;
		bool hasResults = false;
		Action MoveNext;
		PdfPageDataCache cache;
		public PdfTextSearchResults SearchResults { get { return lastSearchResults; } }
		public int WordIndex { get { return wordIndex; } }
		public PdfPageDataCache Cache { get { return cache; } }
		public PdfTextSearch(IList<PdfPage> pages) {
			this.cache = new PdfPageDataCache(pages, true);
		}
		public void ClearCache(int pageIndex) {
			cache.Clear(pageIndex);
		}
		public PdfTextSearchResults Find(string text, PdfTextSearchParameters parameters, int currentPage) {
			return Find(text, parameters, currentPage, null);
		}
		public PdfTextSearchResults Find(string text, PdfTextSearchParameters parameters, int currentPage, Func<int, bool> terminate) {
			lastSearchResults = new PdfTextSearchResults(null, 0, null, null, PdfTextSearchStatus.NotFound);
			if (String.IsNullOrEmpty(text) || cache.DocumentPages.Count == 0)
				return lastSearchResults;
			PdfTextSearchDirection direction = parameters.Direction;
			if (direction == PdfTextSearchDirection.Forward)
				MoveNext = StepForward;
			else
				MoveNext = StepBackward;
			if (text != searchString || !parameters.EqualsTo(searchParameters)) {
				pageIndex = currentPage - 1;
				wordIndex = -1;
				int recognitionStartIndex = pageIndex;
				RecognizeCurrentPage(); 
				while (pageWords == null || pageWords.Count == 0) {
					MoveNext();
					RecognizeCurrentPage();
					if (pageIndex == recognitionStartIndex)
						return lastSearchResults;
				} 
				startPageIndex = pageIndex;
				startWordIndex = -1;
				searchString = text;
				searchWords = new List<string>();
				IList<string> temp = splitter.Split(text);
				Regex whitespaceRemove = new Regex("[\\s]");
				foreach (string str in temp) {
					string tmp = whitespaceRemove.Replace(str, "");
					if (!String.IsNullOrEmpty(tmp))
						searchWords.Add(tmp);
				}
				searchStart = true;
				hasResults = false;
			}
			if (searchParameters == null || searchParameters.Direction != direction)
				searchStart = true;
			searchParameters = parameters.CloneParameters();
			comparisonType = parameters.CaseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
			if (direction == PdfTextSearchDirection.Forward)
				MoveNext = StepForward;
			else
				MoveNext = StepBackward;
			int previousPage = -1;
			do {
				MoveNext();
				foundWords.Clear();
				if (TryCompare()) {
					if (startWordIndex == -1)
						startWordIndex = 0;
					if (searchStart) {
						startWordIndex = wordIndex;
						startPageIndex = pageIndex;
						searchStart = false;
						hasResults = true;
					}
					else if (pageIndex == startPageIndex && wordIndex == startWordIndex) {
						searchStart = true;
						wordIndex = searchParameters.Direction == PdfTextSearchDirection.Forward ? wordIndex - 1 : wordIndex + 1;
						break; 
					}
					lastSearchResults = new PdfTextSearchResults(cache.DocumentPages[pageIndex], pageIndex + 1, foundWords, BuildRectangles(), PdfTextSearchStatus.Found);
					return lastSearchResults;
				}
				if (pageIndex == startPageIndex && wordIndex == startWordIndex)
					break;
				if (startWordIndex == -1)
					startWordIndex = 0;
				if (previousPage != pageIndex) {
					previousPage = pageIndex;
					if (terminate != null && terminate(pageIndex))
						break;
				}
			}
			while (searchStart || !(pageIndex == startPageIndex && wordIndex == startWordIndex));
			lastSearchResults = new PdfTextSearchResults(null, 0, null, null, hasResults ? PdfTextSearchStatus.Finished : PdfTextSearchStatus.NotFound);
			return lastSearchResults;
		}
		void RecognizeCurrentPage() {
			PdfPageData pageData = cache[pageIndex];
			pageLines = pageData.TextData;
			pageWords = pageData.Words;
		}
		void StepBackward() {
			if (wordIndex <= 0) {
				pageIndex = pageIndex <= 0 ? cache.DocumentPages.Count - 1 : pageIndex - 1;
				RecognizeCurrentPage();
				wordIndex = pageWords.Count - 1;
			}
			else
				wordIndex--;
		}
		void StepForward() {
			if (wordIndex >= pageWords.Count - 1) {
				pageIndex = pageIndex >= cache.DocumentPages.Count - 1 ? pageIndex = 0 : pageIndex + 1;
				wordIndex = 0;
				RecognizeCurrentPage();
			}
			else
				wordIndex++;
		}
		IList<PdfOrientedRectangle> BuildRectangles() {
			List<PdfOrientedRectangle> searchResult = new List<PdfOrientedRectangle>();
			int foundWordsCount = foundWords.Count;
			if (foundWordsCount == 0)
				return searchResult;
			IList<PdfWord> currentLine = null;
			currentLine = FindLine(foundWords[0], pageLines);
			if (currentLine == null)
				return searchResult;
			PdfOrientedRectangle firstRectangle = foundWords[0].Rectangles[0]; 
			double angle = firstRectangle.Angle;
			PdfPoint firstTopLeft = PdfTextUtils.RotatePoint(firstRectangle.TopLeft, -angle);
			double top = firstTopLeft.Y;
			double left = firstTopLeft.X;
			double right = left + firstRectangle.Width;
			double height = firstRectangle.Height;
			for (int i = 1; i < foundWordsCount; i++) {
				PdfOrientedRectangle rect = foundWords[i].Rectangles[0];
				if (currentLine.Contains(foundWords[i])) {
					PdfPoint topLeft = PdfTextUtils.RotatePoint(rect.TopLeft, -angle);
					if (topLeft.X < left) {
						AddRectangle(searchResult, left, top, right, height, angle);
						left = topLeft.X;
					}
					right = topLeft.X + rect.Width;
					top = PdfMathUtils.Max(top, topLeft.Y);
					height = PdfMathUtils.Max(height, rect.Height);
				}
				else {
					AddRectangle(searchResult, left, top, right, height, angle);
					angle = rect.Angle;
					IList<PdfOrientedRectangle> previousRectangles = foundWords[i - 1].Rectangles;
					int previousRectsCount = previousRectangles.Count;
					if (previousRectsCount > 1) {
						for (int r = 1; r < previousRectsCount - 1; r++)
							searchResult.Add(previousRectangles[r]);
						PdfOrientedRectangle lastRect = previousRectangles[previousRectsCount - 1];
						PdfPoint lastRectTopLeft = PdfTextUtils.RotatePoint(lastRect.TopLeft, -lastRect.Angle);
						top = lastRectTopLeft.Y;
						left = lastRectTopLeft.X;
						right = rect.Left + rect.Width;
						height = PdfMathUtils.Max(rect.Height, lastRect.Height);
					}
					else {
						PdfPoint rectTopLeft = PdfTextUtils.RotatePoint(rect.TopLeft, -angle);
						top = rectTopLeft.Y;
						left = rectTopLeft.X;
						right = left + rect.Width;
						height = rect.Height;					  
					}
					currentLine = FindLine(foundWords[i], pageLines);
					if (currentLine == null) {
						AddRectangle(searchResult, left, top, right, height, angle);
						return searchResult;
					}
				}
			}
			AddRectangle(searchResult, left, top, right, height, angle);
			IList<PdfOrientedRectangle> lastWordRectangles = foundWords[foundWordsCount - 1].Rectangles;
			int lastWordRectCount = lastWordRectangles.Count;
			for (int r = 1; r < lastWordRectCount; r++)
				searchResult.Add(lastWordRectangles[r]);
			return searchResult;
		}
		bool TryCompare() {
			if (pageWords.Count == 0 || searchWords == null || searchWords.Count == 0)
				return false;
			int searchWordsLength = searchWords.Count;
			bool wholeWords = searchParameters.WholeWords;
			if (searchWordsLength == 1) {
				if (wholeWords) {
					if (CheckSeparators(pageWords[wordIndex].Text, searchWords[0]).Equals(searchWords[0], comparisonType)) {
						foundWords.Add(pageWords[wordIndex]);
						return true;
					}
					else
						return false;
				}
				else {
					if (CheckSeparators(pageWords[wordIndex].Text, searchWords[0]).IndexOf(searchWords[0], comparisonType) >= 0) {
						foundWords.Add(pageWords[wordIndex]);
						return true;
					}
					else
						return false;
				}
			}
			int range = wordIndex + searchWordsLength;
			if (range > pageWords.Count)
				return false;
			if (wholeWords) {
				if (!CheckSeparators(pageWords[wordIndex].Text, searchWords[0]).Equals(searchWords[0], comparisonType))
					return false;
			}
			else 
				if (!CheckSeparators(pageWords[wordIndex].Text, searchWords[0]).EndsWith(searchWords[0], comparisonType))
					return false;
			foundWords.Add(pageWords[wordIndex]);
			for (int i = wordIndex + 1, searchIndex = 1; i < range; i++, searchIndex++) {
				if (searchIndex == searchWordsLength - 1) {
					if (wholeWords) {
						if (!CheckSeparators(pageWords[i].Text, searchWords[searchIndex]).Equals(searchWords[searchIndex], comparisonType))
							return false;
					}
					else
						if (!CheckSeparators(pageWords[i].Text, searchWords[searchIndex]).StartsWith(searchWords[searchIndex], comparisonType))
							return false;
				}
				else
					if (!CheckSeparators(pageWords[i].Text, searchWords[searchIndex]).Equals(searchWords[searchIndex], comparisonType))
						return false;
				foundWords.Add(pageWords[i]);
			}
			return true;
		}
	}
}
