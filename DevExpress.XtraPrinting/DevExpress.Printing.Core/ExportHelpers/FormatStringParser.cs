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

using System.Text;
namespace DevExpress.Printing.ExportHelpers {
	internal class FormatStringParser {
		enum AutomatStates {
			PREFIX,
			FORMAT,
			POSTFIX
		}
		#region Constants
		const char OpenBrace = '{';
		const char ClosedBrace = '}';
		const char Zero = '0';
		const char One = '1';
		const char Colon = ':';
		const char MPostfix = 'M';
		const string DefaultFormatString = "0";
		#endregion
		#region Fields
		string prefix;
		string valueFormat;
		string postfix;
		string unionString;
		#endregion
		public string Prefix {
			get {
				return string.IsNullOrEmpty(prefix) ? string.Empty : prefix;
			}
		}
		public string ValueFormat {
			get {
				return string.IsNullOrEmpty(valueFormat) ? DefaultFormatString : valueFormat;
			}
		}
		public string Postfix {
			get {
				return string.IsNullOrEmpty(postfix) ? string.Empty : postfix;
			}
		}
		internal string UnionString {
			get {
				if(unionString == null) unionString = CreateUnionString();
				return unionString;
			}
		}
		private string CreateUnionString() {
			StringBuilder stringBuilder = new StringBuilder(string.Empty);
			stringBuilder.Append(prefix);
			stringBuilder.Append(valueFormat);
			stringBuilder.Append(postfix);
			return stringBuilder.ToString();
		}
		public FormatStringParser(string itemFormat, string itemFieldName) {
			if(string.IsNullOrEmpty(itemFormat)) return;
			if(itemFormat.Length < 3) valueFormat = itemFormat;
			else TryParse(itemFormat, itemFieldName);
		}
		void TryParse(string itemFormat, string itemFieldName) {
			AutomatStates automatState = AutomatStates.PREFIX;
			char previousСharacter = char.MinValue;
			foreach(char itemFormatCh in itemFormat) {
				switch(automatState) {
					case AutomatStates.PREFIX:
					switch(itemFormatCh) {
						case OpenBrace: previousСharacter = OpenBrace; break;
						case Zero:
							if(previousСharacter == OpenBrace) automatState = AutomatStates.FORMAT;
							else prefix += itemFormatCh;
						break;
						case One:
							if(previousСharacter == OpenBrace) prefix += itemFieldName;
							else prefix += itemFormatCh;
						break;
						case ClosedBrace: break;
						default: prefix += itemFormatCh; break;
					}
					break;
					case AutomatStates.FORMAT:
					switch(itemFormatCh) {
						case Colon: break;
						case MPostfix:
							if(!string.IsNullOrEmpty(prefix)) postfix += itemFormatCh;
							else valueFormat += itemFormatCh;
							break;
						case ClosedBrace: automatState = AutomatStates.POSTFIX; break;
						default: valueFormat += itemFormatCh; break;
					}
					break;
					case AutomatStates.POSTFIX: postfix += itemFormatCh; break;
				}
			}
		}
	}
}
