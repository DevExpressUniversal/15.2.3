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
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.iCalendar.Components;
namespace DevExpress.XtraScheduler.iCalendar.Components {
	public class iContentLineParameters : iCalendarNamedObjectCollection<iContentLineParam> {
	}
	public class iContentLineParam : iCalendarContentLineObject {
		List<string> values;
		public iContentLineParam() {
			values = new List<string>();
		}
		public List<string> Values { get { return values; } }
	}
	public class iCalendarContentLineObject : ICalendarNamedObject {
		string name = string.Empty;
		#region ICalendarNamedObject Members
		string ICalendarNamedObject.Name { get { return name; } }
		#endregion
		public string Name { get { return name; } set { name = value; } }
	}
}
namespace DevExpress.XtraScheduler.iCalendar.Native {
	public class iCalendarContentLines : iCalendarNamedObjectCollection<iCalendarContentLine> {
	}
	public class iCalendarContentLine : iCalendarContentLineObject {
		iContentLineParameters parameters;
		int sourceLineNum;
		string value;
		string sourceLineText;
		public iCalendarContentLine(int sourceLineNum, string sourceLineText)
			: this(sourceLineNum) {
			this.sourceLineText = sourceLineText;
		}
		public iCalendarContentLine(int sourceLineNum) {
			this.parameters = new iContentLineParameters();
			this.sourceLineNum = sourceLineNum;
		}
		public iContentLineParameters Parameters { get { return parameters; } }
		public string Value { get { return value; } set { this.value = value; } }
		public int SourceLineNum { get { return sourceLineNum; } }
		public string SourceLineText { get { return sourceLineText; } }
	}
	public static class iCalendarSymbols {
		public const Char Space = '\u0020';
		public const Char Tab = '\u0009';
		public const char ParameterStartChar = ';';
		public const char ParamValueStartChar = '=';
		public const char ValueStartChar = ':';
		public const char QuotedStringChar = '"';
		public const char ParamValueSeparatorChar = ',';
		public static readonly string ParamValueSeparator = Convert.ToString(ParamValueSeparatorChar);
		static readonly char[] emptyChars = new char[] { };
		static readonly char[] nameStopChars = new char[] { ParameterStartChar, ValueStartChar };
		static readonly char[] paramValueSeparatorChars = new char[] { ParamValueSeparatorChar };
		static readonly char[] paramValueStartChars = new char[] { ParamValueStartChar };
		static readonly char[] quotedStringStopChars = new char[] { QuotedStringChar };
		static readonly char[] paramTextStopChars = new char[] { QuotedStringChar, ParameterStartChar, ValueStartChar, ParamValueSeparatorChar };
		public static char[] EmptyChars { get { return emptyChars; } }
		public static char[] NameStopChars { get { return nameStopChars; } }
		public static char[] ParamValueSeparatorChars { get { return paramValueSeparatorChars; } }
		public static char[] ParamValueStartChars { get { return paramValueStartChars; } }
		public static char[] QuotedStringStopChars { get { return quotedStringStopChars; } }
		public static char[] ParamTextStopChars { get { return paramTextStopChars; } }
		public static string ParameterStart = Convert.ToString(ParameterStartChar);
		public static bool IsWhiteSpace(Char ch) {
			return ch == Space || ch == Tab;
		}
	}
	public class iCalendarContentLineReader {
		int currentIndex;
		string sourceLine;
		public iCalendarContentLineReader(string sourceLine) {
			if (sourceLine == null)
				Exceptions.ThrowArgumentNullException("sourceLine");
			this.sourceLine = sourceLine;
		}
		public Char Current { get { return SourceLine[currentIndex]; } }
		protected internal int CurrentIndex { get { return currentIndex; } }
		public bool EndOfLine { get { return CurrentIndex == SourceLine.Length; } }
		public string SourceLine { get { return sourceLine; } }
		public string Read(Char[] stopChars) {
			if (stopChars == null)
				Exceptions.ThrowArgumentNullException("stopChars");
			if (String.IsNullOrEmpty(SourceLine) || EndOfLine)
				return string.Empty;
			int prevIndex = CurrentIndex;
			this.currentIndex = FindStopCharFirstIndex(stopChars);
			return SourceLine.Substring(prevIndex, CurrentIndex - prevIndex);
		}
		public bool ExpectChar(Char ch) {
			if (PeekChar(ch)) {
				this.currentIndex++;
				return true;
			}
			return false;
		}
		protected internal bool PeekChar(char ch) {
			return !EndOfLine && Current == ch;
		}
		protected int FindStopCharFirstIndex(Char[] stopChars) {
			int index = SourceLine.IndexOfAny(stopChars, CurrentIndex);
			return index >= 0 ? index : SourceLine.Length;
		}
	}
}
