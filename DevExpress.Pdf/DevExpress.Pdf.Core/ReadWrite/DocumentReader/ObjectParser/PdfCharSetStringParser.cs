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

using System.Text;
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public static class PdfCharSetStringParser {
		static int? ConvertToHexadecimalDigit(char c) {
			if (c >= '0' && c <= '9')
				return c - '0';
			if (c >= 'A' && c <= 'F')
				return c - 'A' + 10;
			if (c >= 'a' && c <= 'f')
				return c - 'a' + 10;
			return null;
		}
		public static IList<string> Parse(string charSetString) {
			charSetString = charSetString.Trim();
			if (charSetString[0] == '(') {
				int closePosition = charSetString.LastIndexOf(')');
				if (closePosition < 0)
					PdfDocumentReader.ThrowIncorrectDataException();
				charSetString = charSetString.Substring(1, closePosition - 1);
			}
			List<string> charSet = new List<string>();
			StringBuilder sb = null;
			bool isStarted = false;
			int length = charSetString.Length;
			for (int position = 0; position < length; position++) {
				char c = charSetString[position];
				switch (c) {
					case '\0':
					case '\x09':
					case '\x0a':
					case '\x0c':
					case '\x0d':
					case ' ':
						if (sb != null) {
							charSet.Add(sb.ToString());
							sb = null;
							isStarted = false;
						}
						break;
					case '/':
						if (isStarted) {
							if (sb == null)
								PdfDocumentReader.ThrowIncorrectDataException();
							charSet.Add(sb.ToString());
							sb = null;
						}
						isStarted = true;
						break;
					case '#':
						if (length - position < 3)
							goto default;
						int next = position;
						int? high = ConvertToHexadecimalDigit(charSetString[++next]);
						int? low = ConvertToHexadecimalDigit(charSetString[++next]);
						if (!high.HasValue || !low.HasValue)
							goto default;
						if (sb == null)
							sb = new StringBuilder();
						sb.Append((char)(high.Value * 16 + low.Value));
						position = next;
						break;
					default:
						if (!isStarted || c < '!')
							PdfDocumentReader.ThrowIncorrectDataException();
						if (sb == null)
							sb = new StringBuilder();
						sb.Append(c);
						break;
				}
			}
			if (sb != null)
				charSet.Add(sb.ToString());
			return charSet;
		}
	}
}
