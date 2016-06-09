#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Internal {
	public class IStrComparer : IComparer<string> {
		public static int CompareStrings(string s1, string s2) {
			return CompareStrings(s1, s2, false);
		}
		public static int CompareStrings(string s1, string s2, bool ignoreDiacritic) {
			CompareOptions options = CompareOptions.IgnoreCase;
			if(ignoreDiacritic)
				options = options | CompareOptions.IgnoreNonSpace;
			return CultureInfo.CurrentCulture.CompareInfo.Compare(s1, s2, options);
		}
		public int Compare(string s1, string s2) {
			return IStrComparer.CompareStrings(s1, s2);
		}
	}
	public class SortUtils {
		public static Dictionary<string, int[]> ArrangeIntoLetter(Dictionary<int, string> stringPosList, LanguageInfo langInfo) {
			Dictionary<string, int[]> ret = new Dictionary<string, int[]>();
			List<StrInfo> strInfoList = new List<StrInfo>();
			IStrInfoComparer comparer = new IStrInfoComparer(langInfo.CultureInfo);
			foreach (int key in stringPosList.Keys) {
				StrInfo strInfo = new StrInfo();
				strInfo.Str = stringPosList[key].Trim();
				strInfo.StrPosition = key;
				strInfoList.Add(strInfo);
			}
			strInfoList.Sort(comparer);
			string[] sortedKeyStrings = GetKeyStrings(strInfoList);
			string[] firstLetters = GetFirstLetterInStrings(sortedKeyStrings, langInfo);
			for (int i = 0; i < firstLetters.Length; i++) {
				int[] positionsForLetter = GetPositionsByLetter(firstLetters[i], strInfoList, langInfo);
				ret.Add(firstLetters[i], positionsForLetter);
			}
			return ret;
		}
		public static Dictionary<object, List<TitleIndexNode>> SortGroupValues(Dictionary<object, List<TitleIndexNode>> itemsInGroups, LanguageInfo langInfo) {
			Dictionary<object, List<TitleIndexNode>> ret = new Dictionary<object, List<TitleIndexNode>>();
			ArrayList groupValues = new ArrayList();
			foreach (object groupValue in itemsInGroups.Keys)
				groupValues.Add(groupValue);
			GroupValueComparer comparer = new GroupValueComparer(langInfo.CultureInfo);
			groupValues.Sort(comparer);
			foreach (object group in groupValues)
				ret.Add(group, itemsInGroups[group]);
			return ret;
		}
		public static void SortItemsInGroup(Dictionary<object, List<TitleIndexNode>> sortedGoups, 
			LanguageInfo langInfo) {
			ItemComparer comparer = new ItemComparer(langInfo.CultureInfo);
			foreach (object groupValue in sortedGoups.Keys)
				sortedGoups[groupValue].Sort(comparer);
		}
		public static int[] SortStrings(string[] values, LanguageInfo langInfo) {
			List<StrInfo> strInfoList = new List<StrInfo>();
			IStrInfoComparer comparer = new IStrInfoComparer(langInfo.CultureInfo);
			for (int i = 0; i < values.Length; i++) {
				StrInfo strInfo = new StrInfo();
				strInfo.Str = values[i].Trim();
				strInfo.StrPosition = i;
				strInfoList.Add(strInfo);
			}
			strInfoList.Sort(comparer);
			List<int> ret = new List<int>();
			foreach (StrInfo info in strInfoList)
				ret.Add(info.StrPosition);
			return ret.ToArray();
		}
		protected static string[] GetFirstLetterInStrings(string[] strings, LanguageInfo langInfo) {
			List<string> firstLetters = new List<string>();
			for(int i = 0; i < strings.Length; i++) {
				string str = strings[i].ToUpper(langInfo.CultureInfo);
				if(str.Length > 0) {
					string fistLetter = str[0].ToString();
					if(!firstLetters.Contains(fistLetter))
						firstLetters.Add(fistLetter);
				}
				else
					if(!firstLetters.Contains(string.Empty))
						firstLetters.Add(string.Empty);
			}
			IStrComparer comparer = new IStrComparer();
			firstLetters.Sort(comparer);
			for(int i = firstLetters.Count - 1; i > 0; i--) {
				if(IStrComparer.CompareStrings(firstLetters[i], firstLetters[i - 1], true) == 0) {
					firstLetters.RemoveAt(i);
				}
			}
			return firstLetters.ToArray();
		}
		protected static string[] GetKeyStrings(List<StrInfo> strInfoList) {
			List<string> keyString = new List<string>();
			foreach (StrInfo info in strInfoList)
				keyString.Add(info.Str);
			return keyString.ToArray();
		}
		protected static int[] GetPositionsByLetter(string letter, List<StrInfo> strInfoList, LanguageInfo langInfo) {
			List<int> ret = new List<int>();
			string str = "";
			int index = 0;
			do {
				str = strInfoList[index].Str.ToUpper(langInfo.CultureInfo);
				index++;
			} while((index < strInfoList.Count) && !AreLettersEqual(GetFirstLetterInString(str), letter));
			index--;
			str = strInfoList[index].Str.ToUpper(langInfo.CultureInfo);
			while ((index < strInfoList.Count) &&
				(AreLettersEqual(str, letter) || (str.Length > 0 && AreLettersEqual(str[0].ToString(), letter)))) {
				ret.Add(strInfoList[index].StrPosition);
				index++;
				str = index < strInfoList.Count ? strInfoList[index].Str.ToUpper(langInfo.CultureInfo) : "";
			}
			return ret.ToArray();
		}
		protected static string GetFirstLetterInString(string str) {
			if (str.Length > 0)
				return str[0].ToString();
			else
				return string.Empty;
		}
		protected static bool AreLettersEqual(string letter1, string letter2) {
			return IStrComparer.CompareStrings(letter1, letter2, true) == 0;
		}
	}
	public class GroupInfo {
		public int index;
		public object Data;
	}
	public class ItemComparer : IComparer<TitleIndexNode> {
		private CultureInfo fCultureInfo;
		private StringComparer fComparer;
		public ItemComparer(CultureInfo cultureInfo) {
			fCultureInfo = cultureInfo;
			fComparer = StringComparer.Create(cultureInfo, true);
		}
		public int Compare(TitleIndexNode x, TitleIndexNode y) {
			return fComparer.Compare(x.Text, y.Text);
		}
	}
	public class StrInfo {
		public string Str;
		public int StrPosition;
	}
	public class IStrInfoComparer : IComparer<StrInfo> {
		private CultureInfo fCultureInfo;
		public IStrInfoComparer(CultureInfo cultureInfo) {
			fCultureInfo = cultureInfo;
		}
		public int Compare(StrInfo x, StrInfo y) {
			return IStrComparer.CompareStrings(x.Str, y.Str);
		}
	}
	public class GroupValueComparer : IComparer {
		private CultureInfo fCultureInfo;
		private Comparer fObjectComparer = null;
		public GroupValueComparer(CultureInfo cultureInfo) {
			fCultureInfo = cultureInfo;
			fObjectComparer = new Comparer(cultureInfo);
		}
		public int Compare(object x, object y) {
			if (x.GetType() == y.GetType())
				return fObjectComparer.Compare(x, y);
			else
				return string.Compare(x.ToString(), y.ToString(), true, CultureInfo.CurrentCulture); 
		}
	}
	public abstract class LanguageInfo {
		public abstract char[] AlphabetLetters { get; }
		public abstract CultureInfo CultureInfo { get; }
		public LanguageInfo() { }
		public static LanguageInfo CreateAlphabetInfo(string name) {
			return new LatinLanguageInfo();
		}
		public abstract bool IsAlphabetSymbol(char s);
	}
	public class LatinLanguageInfo : LanguageInfo {
		private static char[] LatinAlphabetLetters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
		private static List<char> LatinAlphabetLetterList = new List<char>(LatinAlphabetLetters);
		public static IStrInfoComparer StringComparer = new IStrInfoComparer(CultureInfo.GetCultureInfo("en-US"));
		public override char[] AlphabetLetters {
			get { return LatinAlphabetLetters; }
		}
		public override CultureInfo CultureInfo {
			get { return CultureInfo.GetCultureInfo("en-US"); }
		}
		public LatinLanguageInfo() { }
		public override bool IsAlphabetSymbol(char s) {
			return LatinAlphabetLetterList.Contains(s);
		}
	}
}
