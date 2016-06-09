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
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.Printing.Native {
	public static class PageRangeParser {
		public static int[] GetIndices(string range, int maxIndex) {
			if(range == null)
				throw new ArgumentException("range");
			return GetValues(range, maxIndex);
		}
		static int[] GetValues(string range, int maxIndex) {
			List<int> indexes = new List<int>();
			string s = ValidateString(range);
			if(s.Length > 0) {
				string[] items = s.Split(',');
				for(int i = 0; i < items.Length; i++) {
					try {
						int[] values = ParseElement(items[i], maxIndex);
						foreach(int val in values) {
							if(!indexes.Contains(val - 1))
								indexes.Add(val - 1);
						}
					} catch { }
				}
			}
			int[] result = (string.IsNullOrEmpty(range)) ? GetAllIndexes(maxIndex) :
				indexes.ToArray();
			Array.Sort<int>(result);
			return result;
		}
		public static string ValidateString(string s) {
			if(s == null)
				return "";
			char[] chars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', ',' };
			for(int i = s.Length - 1; i >= 0; i--) {
				if(Array.IndexOf(chars, s[i]) < 0)
					s = s.Remove(i, 1);
			}
			s = Replace(s, "--", "-");
			s = Replace(s, ",,", ",");
			s = Replace(s, ",0-", ",");
			s = Replace(s, "-0,", ",");
			s = Replace(s, ",0,", ",");
			s = s.TrimStart(',', '-');
			if(s.StartsWith("0,"))
				s = s.Substring(2);
			if(s.StartsWith("0-"))
				s = s.Substring(2);
			if(s.EndsWith(",0"))
				s = s.Substring(0, s.Length - 2);
			if(s.EndsWith("-0"))
				s = s.Substring(0, s.Length - 2);
			s = s.TrimEnd(',');
			return s;
		}
		static int[] GetAllIndexes(int count) {
			int[] indexes = new int[count];
			for(int i = 0; i < count; i++)
				indexes[i] = i;
			return indexes;
		}
		static int[] ParseElement(string s, int maxIndex) {
			string[] items = s.Split('-');
			int val1 = Convert.ToInt32(items[0]);
			if(items.Length == 1) {
				if(val1 <= maxIndex)
					return new int[] { val1 };
				else
					return new int[0];
			}
			int val2 = items[1].Length > 0 ? Convert.ToInt32(items[1]) : maxIndex;
			if(val1 > val2) {
				int val = val2;
				val2 = val1;
				val1 = val;
			}
			List<int> values = new List<int>();
			do {
				if(val1 <= maxIndex)
					values.Add(val1);
				val1++;
			} while(val1 <= val2);
			return values.ToArray();
		}
		static string Replace(string s, string oldValue, string newValue) {
			while(s.IndexOf(oldValue) >= 0)
				s = s.Replace(oldValue, newValue);
			return s;
		}
	}
}
