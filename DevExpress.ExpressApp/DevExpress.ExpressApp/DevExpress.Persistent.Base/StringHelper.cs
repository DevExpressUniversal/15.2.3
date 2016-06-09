#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
namespace DevExpress.Persistent.Base {
	public static class StringHelper {
		private static readonly string[] PreConvertedNumbers = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11" };
		public static readonly string[] EmptyList = new string[0];
		public static string GetLastPart(char delimiter, string str) {
			int lastIndex = str.LastIndexOf(delimiter);
			if (lastIndex < 0) {
				return str;
			}
			else {
				return string.Intern(str.Substring(lastIndex + 1));
			}
		}
		public static string GetFirstPart(char delimiter, string str) {
			int firstIndex = str.IndexOf(delimiter);
			if (firstIndex < 0) {
				return str;
			}
			else {
				return string.Intern(str.Substring(0, firstIndex));
			}
		}
		public static bool ParseToBool(string str, bool defaultResult) {
			bool result;
			return (bool.TryParse(str, out result) ? result : defaultResult);
		}
		public static int ParseToInt(string str, int defaultResult) {
			int result;
			return (Int32.TryParse(str, out result) ? result : defaultResult);
		}
		public static string IntToString(int value) {
			if(value == -1) {
				return "-1";
			} else if((value >= 0) && (value < PreConvertedNumbers.Length)) {
				return PreConvertedNumbers[value];
			}
			else {
				return string.Intern(value.ToString());
			}		
		}
	}
}
