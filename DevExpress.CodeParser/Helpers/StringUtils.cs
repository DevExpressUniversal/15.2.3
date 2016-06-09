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
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Collections;
#if SL
using DevExpress.Xpf.Collections;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	#region LineTerminatorChar
	public enum LineTerminatorChar
	{
		Unknown,
		CarriageReturn,
		LineFeed,
		LineSeparator,
		ParagraphSeparator
	}
	#endregion
	#region LineTerminator
	public enum LineTerminator
	{
		None,
		MSDOS,
		Mac,
		Unix,
		LineSeparator,
		ParagraphSeparator
	}
	#endregion
  public static class StringHelper
  {
		#region GetLineTerminatorChar
		static LineTerminatorChar GetLineTerminatorChar(char character)
		{
			switch (character)
			{
				case '\u000D':
					return LineTerminatorChar.CarriageReturn;
				case '\u000A':
					return LineTerminatorChar.LineFeed;
				case '\u2028':
					return LineTerminatorChar.LineSeparator;
				case '\u2029':
					return LineTerminatorChar.ParagraphSeparator;
				default:
					return LineTerminatorChar.Unknown;
			}
		}
		#endregion
		#region CloneStringCollection
	public static StringCollection CloneStringCollection(StringCollection source)
	{
	  if (source == null)
		return null;
	  StringCollection lResult = new StringCollection();
	  string[] lStrings = new string[source.Count];
	  source.CopyTo(lStrings, 0);
	  lResult.AddRange(lStrings);
	  return lResult;
	}
		#endregion
		#region SplitLines(string text)
		public static string[] SplitLines(string text)
		{
			return SplitLines(text, true);
		}
		#endregion
		#region SplitLines(string text, bool removeLastIfEmpty)
		public static string[] SplitLines(string text, bool removeLastIfEmpty)
		{
			if (text == null || text == String.Empty)
				return new string[0];
			ArrayList lLines = new ArrayList();
			int lIndex = 0;
			int lLineIndex = 0;
			int lTextLength = text.Length;
			while (lIndex < lTextLength)
			{
				if (GetLineTerminatorChar(text[lIndex]) != LineTerminatorChar.Unknown)
				{
					lLines.Add(text.Substring(lLineIndex, lIndex - lLineIndex));
					lIndex += GetLineTerminatorLength(GetLineTerminator(text, lIndex));
					lLineIndex = lIndex;
					continue;
				}
				lIndex++;
			}
			lLines.Add(text.Substring(lLineIndex));
			if (removeLastIfEmpty && ((string)lLines[lLines.Count - 1]) == String.Empty)
				lLines.RemoveAt(lLines.Count - 1);
			string[] lResult = new string[lLines.Count];
			lLines.CopyTo(lResult);
			return lResult;
		}
		#endregion
		#region GetLineTerminator(string text)
		public static LineTerminator GetLineTerminator(string text)
		{
			return GetLineTerminator(text, 0);
		}
		#endregion
		#region GetLineTerminator(string text, int index)
		public static LineTerminator GetLineTerminator(string text, int index)
		{
			if (text == null)
				throw new ArgumentNullException("text");
			if (text == String.Empty)
				throw new ArgumentException("Cannot check empty string", "text");
			if (text.Length <= index) {
#if !SL
				throw new ArgumentOutOfRangeException("index", index, "index is out of range.");
#else
				throw new ArgumentOutOfRangeException("index", "index is out of range.");
#endif
			}
			switch (GetLineTerminatorChar(text[index]))
			{
				case LineTerminatorChar.CarriageReturn:
					if (index < text.Length - 1 && GetLineTerminatorChar(text[index + 1]) == LineTerminatorChar.LineFeed)
						return LineTerminator.MSDOS;
					else
						return LineTerminator.Mac;
				case LineTerminatorChar.LineFeed:
					return LineTerminator.Unix;
				case LineTerminatorChar.LineSeparator:
					return LineTerminator.LineSeparator;
				case LineTerminatorChar.ParagraphSeparator:
					return LineTerminator.ParagraphSeparator;
				default:
					return LineTerminator.None;
			}
		}
		#endregion
		#region GetLineTerminatorLength
		public static int GetLineTerminatorLength(LineTerminator lineTerm)
		{
			switch (lineTerm)
			{
				case LineTerminator.MSDOS:
					return 2;
				case LineTerminator.Mac:
				case LineTerminator.Unix:
				case LineTerminator.LineSeparator:
				case LineTerminator.ParagraphSeparator:
					return 1;
				default: 
					return 0;
			}
		}
		#endregion
	public static bool EndsWithLineTerminator(string text)
	{
	  if (string.IsNullOrEmpty(text))
		return false;
	  char lastCh = text[text.Length - 1];
	  return lastCh == '\n' || 
		lastCh == '\r' || 
		lastCh == '\u2028' || 
		lastCh == '\u2029';
	}
  }
}
