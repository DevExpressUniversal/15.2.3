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
using System.IO;
using System.Xml;
using System.Text;
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	public interface IHtmlScannerContext {
		StringReader InputReader { get; }
		XmlNameTable NameTable { get; }
		event EventHandler Initialization;
	}
	public class HtmlScanner : IAutomatonSourceReader {
		public const char EOF = (char)0xFFFF;
		private const int MaxEntityReferenceDecimalCode = 0xFFFF;
		private const int MaxEntityReferenceHexCode = 0xFFFF;
		public const string TagTerminators = " \t\r\n=/><&";
		public const string AttributeNameTerminators = " \t\r\n='\"/><";
		public const string AttributeValueTerminators = " &\t\r\n><";
		public const string CDataTerminators = "\t\r\n[><]";
		public const string ProcessingInstructionTerminators = " \t\r\n?><";
		public const string ProcessingInstructionValueTerminators = "?><";
		private static readonly int[] C1ToWin1252Map = new int[] {
			0x20AC, 0x0081, 0x201A, 0x0192, 0x201E, 0x2026, 0x2020, 0x2021,
			0x02C6, 0x2030, 0x0160, 0x2039, 0x0152, 0x008D, 0x017D, 0x008F,
			0x0090, 0x2018, 0x2019, 0x201C, 0x201D, 0x2022, 0x2013, 0x2014,
			0x02DC, 0x2122, 0x0161, 0x203A, 0x0153, 0x009D, 0x017E, 0x0178
		};
		private IHtmlScannerContext context;
		private JsWalkAutomaton jsWalker;
		private CssWalkAutomaton cssWalker;
		private char lastChar;
		private int absolutePos;
#if DEBUG
		private StringBuilder history = new StringBuilder();
#endif
		public HtmlScanner(IHtmlScannerContext context) {
			this.context = context;
			this.context.Initialization += new EventHandler(OnContextInit);
			this.jsWalker = new JsWalkAutomaton(this);
			this.cssWalker = new CssWalkAutomaton(this);
			this.absolutePos = 0;
		}
		public int AbsolutePosition {
			get { return absolutePos; }
		}
		public char LastChar {
			get { return lastChar; }
			internal set { lastChar = value; }
		}
#if DEBUG
		public string History {
			get { return history.ToString(); }
		}
#endif
		public static bool IsWhitespace(char ch) {
			return ch == ' ' || ch == '\t' || ch == 0x0A || ch == 0x0D;
		}
		public char PeekChar() {
			return (char)this.context.InputReader.Peek();
		}
		public char ScanChar() {
			this.lastChar = (char)this.context.InputReader.Read();
			this.absolutePos++;
			if(this.lastChar == 0)
				this.lastChar = ' ';
#if DEBUG
			history.Append(this.lastChar);
#endif
			return this.lastChar;
		}
		public string ScanCSS() {
			return Scan(this.cssWalker);
		}
		public string ScanEntityReference() {
			StringBuilder sb = new StringBuilder();
			char ch = ScanChar();
			string name;
			int code = 0;
			if(ch != '#') {
				name = ScanEntityName();
				if(name.Length == 0)
					return "&amp;";
				bool lastCharIsSemi = LastChar == ';';
				if(!lastCharIsSemi) {
					sb.Append("&amp;");
					sb.Append(name);
				} else {
					if(DtdXhtml10Trans.GetEntityCodeByName(name) < 0) {
						sb.Append("&amp;");
						sb.Append(name);
					} else {
						if(name != DtdXhtml10Trans.AposEntityName) {
							sb.Append('&');
							sb.Append(name);
							sb.Append(';');
						} else
							sb.Append('\'');
						ScanChar(); 
					}
				}
				return sb.ToString();
			} else {
				sb.Append('#');
				ch = ScanChar();
				bool isDecimal = ch != 'x';
				if(isDecimal) {
					while(ch != HtmlScanner.EOF && ch >= '0' && ch <= '9') {
						sb.Append(ch);
						code = 10 * code + (ch - '0');
						if(code >= MaxEntityReferenceDecimalCode) {
							code = -1;
							ScanChar();
							break;
						}
						ch = ScanChar();
					}
				} else {
					sb.Append('x');
					ch = ScanChar();
					int digit = 0;
					while(ch != HtmlScanner.EOF) {
						if(ch >= '0' && ch <= '9') {
							digit = (ch - '0');
							sb.Append(ch);
						} else if(ch >= 'A' && ch <= 'F') {
							digit = 10 + (ch - 'A');
							sb.Append(ch);
						} else if(ch >= 'a' && ch <= 'f') {
							digit = 10 + (ch - 'a');
							sb.Append(ch);
						} else {
							break;
						}
						code = 16 * code + digit;
						if(code >= MaxEntityReferenceHexCode) {
							code = -1;
							ScanChar();
							break;
						}
						ch = ScanChar();
					}
				}
				if(code == 0)
					return isDecimal ? "&amp;#" : "&amp;#x";
				else if(code < 0)
					return "&amp;" + sb.ToString();
				if(ch == ';' && code > 0) {
					ScanChar(); 
					name = DtdXhtml10Trans.GetEntityNameByCode(code);
					if(name != null && name != DtdXhtml10Trans.AposEntityName) {
						sb.Length = 0;
						sb.Append('&');
						sb.Append(name);
						sb.Append(';');
						return sb.ToString();
					} else {
						if(code >= 0x80 && code <= 0x9F)
							code = C1ToWin1252Map[code - 0x80];
						return Convert.ToChar(code).ToString();
					}
				} else {
					return "&amp;" + sb.ToString();
				}
			}
		}
		internal string ScanEntityName() {
			StringBuilder name = new StringBuilder();
			char ch = this.LastChar;
			while(ch != EOF && (char.IsLetterOrDigit(ch) || ch == '-' || ch == '_')) {
				name.Append(ch);
				ch = ScanChar();
			}
			return name.ToString();
		}
		public string ScanAttributeValue(char quote) {
			StringBuilder sb = new StringBuilder();
			char ch = LastChar;
			while(ch != quote && ch != EOF) {
				if(ch == '&') {
					sb.Append(ScanEntityReference());
					ch = LastChar;
				} else {
					sb.Append(ch);
					ch = ScanChar();
				}
			}
			ScanChar();
			return sb.ToString();
		}
		public string ScanNameToken(string terminators) {
			StringBuilder sb = new StringBuilder();
			char ch = LastChar;
			while(ch != '_' && !char.IsLetter(ch)) {
				if(ch == EOF || terminators.IndexOf(ch) >= 0)
					return string.Empty;
				else
					ch = ScanChar();
			}
			while(ch != EOF && terminators.IndexOf(ch) < 0) {
				if(ch == '_' || ch == '.' || ch == '-' || ch == ':' || Char.IsLetterOrDigit(ch))
					sb.Append(ch);
				else {
					ch = ScanChar();
					break;
				}
				ch = ScanChar();
			}
			return this.context.NameTable.Add(sb.ToString());
		}
		public string ScanScript() {
			return Scan(this.jsWalker);
		}
		public string ScanToken(string terminators) {
			StringBuilder sb = new StringBuilder();
			char ch = LastChar;
			while(ch != EOF && terminators.IndexOf(ch) < 0) {
				sb.Append(ch);
				ch = ScanChar();
			}
			return sb.ToString();
		}
		public string ScanTo(string terminationSequence) {
			StringBuilder sb = new StringBuilder();
			char ch = ScanChar();
			int currentTermIndex = 0;
			char currentTerm = terminationSequence[0];
			while(ch != EOF) {
				if(ch == currentTerm) {
					if(currentTermIndex >= terminationSequence.Length - 1)
						break;
					currentTerm = terminationSequence[++currentTermIndex];
				} else if(currentTermIndex > 0) {
					int newCurrentTermIndex = 0;
					int i = currentTermIndex - 1;
					while(i >= 0 && newCurrentTermIndex == 0) {
						if(terminationSequence[i] == ch) {
							int j = i - 1;
							int offset = currentTermIndex - i;
							while(j >= 0) {
								if(terminationSequence[j] != terminationSequence[j + offset])
									break;
								j--;
							}
							if(j < 0) {
								newCurrentTermIndex = i + 1;
								break;
							}
						}
						i--;
					}
					if(i >= 0) {
						for(int k = 0; k < currentTermIndex - i; k++)
							sb.Append(terminationSequence[k]);
					} else {
						for(int k = 0; k < currentTermIndex - i - 1; k++)
							sb.Append(terminationSequence[k]);
						sb.Append(ch);
					}
					currentTermIndex = newCurrentTermIndex;
					currentTerm = terminationSequence[newCurrentTermIndex];
				} else {
					sb.Append(ch);
				}
				ch = ScanChar();
			}
			if(ch == EOF && currentTermIndex > 0) {
				for(int k = 0; k < currentTermIndex; k++)
					sb.Append(terminationSequence[k]);
			} else
				ScanChar();
			return sb.ToString();
		}
		public string ScanTo(char terminator) {
			StringBuilder sb = new StringBuilder();
			char ch = ScanChar();
			while(ch != EOF && ch != terminator) {
				sb.Append(ch);
				ch = ScanChar();
			}
			ScanChar();
			return sb.ToString();
		}
		public char SkipWhitespaces() {
			char ch = LastChar;
			while(ch != EOF && (ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n'))
				ch = ScanChar();
			return ch;
		}
		private void OnContextInit(object sender, EventArgs e) {
			this.lastChar = EOF;
			this.absolutePos = -1;
#if DEBUG
			this.history.Length = 0;
#endif
		}
		private string Scan(Automaton walker) {
			StringBuilder sb = new StringBuilder();
			char ch = LastChar;
			walker.Reset();
			while(walker.IsActive) {
				sb.Append(ch);
				ch = walker.PerformAction();
			}
			return sb.ToString();
		}
	}
}
