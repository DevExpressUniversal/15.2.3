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
namespace DevExpress.XtraSpreadsheet.Model {
	#region PositionType
	public enum PositionType {
		Relative = 0,
		Absolute = 1,
	}
	#endregion
}
namespace DevExpress.Office.Utils {
	using DevExpress.XtraSpreadsheet.Model;
	using System.Collections.Generic;
	#region CellReferencePart
	public class CellReferencePart {
		readonly Dictionary<char, int> table;
		PositionType type;
		int value;
		int order;
		public CellReferencePart(Dictionary<char, int> table, int order) {
			this.table = table;
			this.order = order;
		}
		public PositionType Type { get { return type; }  }
		public int Value { get { return value; } set { this.value = value; } }
		public int Parse(string reference, int from) {
			int count = reference.Length;
			for (int i = from; i < count; i++) {
				int characterValue;
				if (!table.TryGetValue(reference[i], out characterValue))
					return value == 0 ? from : i;
				if (characterValue == -1) {
					if (i == from)
						type = PositionType.Absolute;
					else
						return i;
				}
				else {
					this.value *= order;
					this.value += characterValue;
				}
			}
			return count;
		}
	}
	#endregion
	#region CellReferenceParserProvider
	public static class CellReferenceParserProvider {
		#region Letters
		public static Dictionary<char, int> Letters = CreateLetters();
		static Dictionary<char, int> CreateLetters() {
			Dictionary<char, int> result = new Dictionary<char, int>(26 + 26 + 1);
			const int codeCapitalA = 'A';
			const int codeA = 'a';
			for (int i = 0; i < 26; i++) {
				result.Add((char)(codeCapitalA + i), i + 1);
				result.Add((char)(codeA + i), i + 1);
			}
			result.Add('$', -1);
			return result;
		}
		#endregion
		#region Digits
		public static Dictionary<char, int> Digits = CreateDigits();
		static Dictionary<char, int> CreateDigits() {
			Dictionary<char, int> result = new Dictionary<char, int>(10 + 1);
			const int codeZero = '0';
			for (int i = 0; i < 10; i++) {
				result.Add((char)(codeZero + i), i);
			}
			result.Add('$', -1);
			return result;
		}
		#endregion
	}
	#endregion
}
