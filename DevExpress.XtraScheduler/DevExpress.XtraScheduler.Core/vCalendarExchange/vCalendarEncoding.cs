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
using System.Text;
using System.Text.RegularExpressions;
namespace DevExpress.XtraScheduler.VCalendar { 
	public class QuotedPrintableConverter {
		internal const int QuotedPrintableLineWidth = 76; 
		internal const string QuotedPrintableCRLF = "=0D=0A";
		internal const string QuotedPrintableEQUAL = "=3D";
		internal const string QuotedPrintableSPACE = "=20";
		internal const Char QuotedPrintableSoftLineBreak = '=';
		protected const string QuotedPrintableNewLine = "=\r\n";
		public string Encode(string text) {
			return Encode(text, 0, QuotedPrintableLineWidth);
		}
		public string Encode(string text, int startIndex, int maxLineWidth) {
			if(text == null || text.Length == 0 || maxLineWidth <= 0)
				return text;
			StringBuilder sb = new StringBuilder(text.Length + 1);
			int charsPerLine = startIndex;
			for(int i = 0; i < text.Length; i++) {
				Char ch = text[i];
				if(IsValidPrintingChar(ch)) {
					if(charsPerLine + 1 > maxLineWidth - 1) {
						sb.Append(QuotedPrintableNewLine);
						charsPerLine = 0;
					}
					sb.Append(ch);
					charsPerLine++;
				}
				else { 
					if(charsPerLine + 3 > maxLineWidth - 1) {
						sb.Append(QuotedPrintableNewLine);
						charsPerLine = 0;
					}
					sb.AppendFormat("={0:X2}", (int)ch);
			   		charsPerLine += 3;
				}
			}
			return sb.ToString();
		}
		bool IsValidPrintingChar(Char c) {
			if(c == '\t') return true;
			return (c > '\x001f') && (c < '\x007f') && (c != '=');
 		}
		public string Decode(string text) {
			if(text == null || text.Length == 0 || text.IndexOf("=") == -1)
				return text;
			StringBuilder sb = new StringBuilder(text.Length);
			for(int i = 0; i < text.Length; i++) {
				if(text[i] == QuotedPrintableSoftLineBreak && (i + 2 <= text.Length)) {
					string token = text.Substring(i, 3);
					if(token != QuotedPrintableNewLine) {
						sb.Append(DecodeHex(token));
					}
					i += 2;
				}
				else 
					sb.Append(text[i]);
			}
			return sb.ToString();
		}
		string DecodeHex(string text) {
			if(text == null || text.Length != 3)
				return text;
			Regex regex = new Regex("(\\=([0-9A-F][0-9A-F]))", RegexOptions.IgnoreCase);
			return regex.Replace(text, new MatchEvaluator(HexEvaluator));
		}
		string HexEvaluator(Match m) {
			string hexCode = m.Groups[2].Value;
			int code = Convert.ToInt32(hexCode, 16);
			Char c = Convert.ToChar(code);
			return c.ToString();
		}
	}
}
