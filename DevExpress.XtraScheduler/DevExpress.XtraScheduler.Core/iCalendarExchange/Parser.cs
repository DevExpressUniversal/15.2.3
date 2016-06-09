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
using System.IO;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.iCalendar.Components;
namespace DevExpress.XtraScheduler.iCalendar.Native {
	public delegate void iCalendarParseErrorHandler(object sender, iCalendarParseErrorEventArgs e);
	public class iCalendarContentLineIterator : IEnumerator<String> {
		StreamReader reader;
		string currentLine;
		string nextLine;
		int sourceLineNum;
		public iCalendarContentLineIterator(StreamReader streamReader) {
			if (streamReader == null)
				Exceptions.ThrowArgumentNullException("streamReader");
			this.reader = streamReader;
			Reset();
		}
		protected internal StreamReader Reader { get { return reader; } }
		protected internal string CurrentLine { get { return currentLine; } }
		internal int SourceLineNum { get { return sourceLineNum; } set { sourceLineNum = value; } }
		#region IEnumerator<string> Members
		public string Current {
			get { return CurrentLine; }
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			this.reader = null;
		}
		#endregion
		#region IEnumerator Members
		object System.Collections.IEnumerator.Current {
			get { return Current;  }
		}
		public bool MoveNext() {
			if (nextLine == null)
				return false;
			currentLine = nextLine;
			nextLine = Reader.ReadLine();
			SourceLineNum++;
			while (iCalendarSymbols.IsWhiteSpace(ReadFirstChar(nextLine))) {
				currentLine += nextLine.Substring(1);
				nextLine = Reader.ReadLine();
				SourceLineNum++;
			}
			return true;
		}
		public void Reset() {
			Reader.BaseStream.Seek(0, SeekOrigin.Begin);
			this.nextLine = Reader.ReadLine();
			this.currentLine = String.Empty;
			this.sourceLineNum = 0;
		}
		#endregion
		private char ReadFirstChar(string str) {
			return (!string.IsNullOrEmpty(str) && str.Length > 0) ? str[0] : (Char)0;
		}
	}
	public class iCalendarContentLineParser {
		iCalendarContentLineReader reader;
		internal iCalendarContentLineReader Reader { get { return reader; } }
		public iCalendarContentLine Parse(string contentLine, int sourceLineNum) {
			if (String.IsNullOrEmpty(contentLine))
				return null;
			iCalendarContentLine result = new iCalendarContentLine(sourceLineNum, contentLine);
			this.reader = new iCalendarContentLineReader(contentLine);
			ParseName(result);
			if (Reader.PeekChar(iCalendarSymbols.ParameterStartChar))
				ParseParameters(result.Parameters);
			if (Reader.ExpectChar(iCalendarSymbols.ValueStartChar))
				ParseValue(result);
			return result;
		}
		protected internal void ParseName(iCalendarContentLine line) {
			line.Name = Reader.Read(iCalendarSymbols.NameStopChars);
		}
		protected internal void ParseParameters(iContentLineParameters parameters) {
			while (Reader.ExpectChar(iCalendarSymbols.ParameterStartChar))
				ParseSingleParameter(parameters);
		}
		protected internal void ParseSingleParameter(iContentLineParameters parameters) {
			iContentLineParam param = new iContentLineParam();
			ParseParameterName(param);
			if (Reader.ExpectChar(iCalendarSymbols.ParamValueStartChar))
				ParseParameterValues(param.Values);
			parameters.Add(param);
		}
		protected internal void ParseParameterName(iContentLineParam param) {
			param.Name = Reader.Read(iCalendarSymbols.ParamValueStartChars);
		}
		protected internal void ParseParameterValues(List<string> values) {
			ParseSingleParameterValue(values);
			while(Reader.ExpectChar(iCalendarSymbols.ParamValueSeparatorChar))
				ParseSingleParameterValue(values);
		}
		protected internal void ParseSingleParameterValue(List<string> values) {
			if (Reader.PeekChar(iCalendarSymbols.QuotedStringChar))
				ParseQuotedString(values);
			else
				ParseParamText(values);
		}
		protected internal void ParseQuotedString(List<string> values) {
			Reader.ExpectChar(iCalendarSymbols.QuotedStringChar);
			values.Add(Reader.Read(iCalendarSymbols.QuotedStringStopChars));
			Reader.ExpectChar(iCalendarSymbols.QuotedStringChar);
		}
		protected internal void ParseParamText(List<string> values) {
			values.Add(Reader.Read(iCalendarSymbols.ParamTextStopChars));
		}
		protected internal void ParseValue(iCalendarContentLine line) {
			line.Value = Reader.Read(iCalendarSymbols.EmptyChars);
		}
	}
	public class iCalendarParserBase {
		#region Fields
		iCalendarContentLines lines;
		iCalendarContentLineIterator iterator;
		iCalendarContentLineParser parser;
		Stack<ICalendarBodyItemContainer> objectStack;
		ICalendarBodyItemContainer container;
		bool isTermination;
		#endregion
		public iCalendarParserBase(ICalendarBodyItemContainer container) {
			this.lines = new iCalendarContentLines();
			this.objectStack = new Stack<ICalendarBodyItemContainer>();
			this.container = container;
		}
		#region Properties
		internal iCalendarContentLines Lines { get { return lines; } }
		protected iCalendarContentLineIterator Iterator { get { return iterator; } }
		protected iCalendarContentLineParser Parser { get { return parser; } }
		protected Stack<ICalendarBodyItemContainer> ObjectStack { get { return objectStack; } }
		public ICalendarBodyItemContainer Container { get { return container; } }
		public bool IsTermination { get { return isTermination; } set { isTermination = value; } }
		#endregion
		#region Events
		internal event iCalendarParseErrorHandler ParseError;
		internal void RaiseParseError(iCalendarContentLine line, System.Exception originalException) {
			if(ParseError != null)
				ParseError(this, new iCalendarParseErrorEventArgs(line, originalException));
		}
		#endregion
		#region Parse
		public void Parse(Stream stream) {
			Parse(new StreamReader(stream));
		}
		public void Parse(StreamReader reader) {
			if(reader == null || reader.BaseStream == Stream.Null)
				return;
			InitParse(reader);
			PrepareStack();
			ParseCore();
		}
		#endregion
		#region InitParse
		protected virtual void InitParse(StreamReader reader) {
			isTermination = false;
			Lines.Clear();
			this.iterator = CreateLineIterator(reader);
			this.parser = CreateLineParser();
		}
		#endregion
		#region PrepareStack
		protected void PrepareStack() {
			ObjectStack.Clear();
			ObjectStack.Push(Container);
		}
		#endregion
		#region CreateLineParser
		protected virtual iCalendarContentLineParser CreateLineParser() {
			return new iCalendarContentLineParser();
		}
		#endregion
		#region CreateLineIterator
		protected virtual iCalendarContentLineIterator CreateLineIterator(StreamReader reader) {
			return new iCalendarContentLineIterator(reader);
		}
		#endregion
		#region ParseCore
		protected virtual void ParseCore() {
			CreateLines();
			ProcessLines();
		}
		#endregion
		#region CreateLines
		protected virtual void CreateLines() {
			while(Iterator.MoveNext()) {
				if(String.IsNullOrEmpty(Iterator.Current))
					continue;
				iCalendarContentLine line = Parser.Parse(Iterator.Current, Iterator.SourceLineNum);
				if(line != null)
					Lines.Add(line);
			}
		}
		#endregion
		#region ProcessLines
		protected virtual void ProcessLines() {
			int count = Lines.Count;
			for(int i = 0; i < count; i++) {
				if (IsTermination)
					break;
				iCalendarContentLine line = Lines[i];
				iCalendarBodyItem item = null;
				try {
					item = iCalendarObjectFactory.CreateBodyItem(line.Name, line.Parameters, line.Value);
				}
				catch(Exception e) {
					 System.Diagnostics.Debug.WriteLine(String.Format("iCalendarParseException: Cannot parse content line with name '{0}', near line {1}", line.Name, line.SourceLineNum));
					 RaiseParseError(line, e);
				}
				if(item == null)
					continue;
				ICalendarBodyItemContainer currentContainer = ObjectStack.Peek();
				currentContainer.AddObject(item);
				iCalendarBodyItemType type = item.BodyItemType;
				if(type == iCalendarBodyItemType.ComponentStart)
					ObjectStack.Push((ICalendarBodyItemContainer)item);
				else {
					if(type == iCalendarBodyItemType.ComponentEnd)
						ObjectStack.Pop();
				}
			}
		}
		#endregion
	}
	public class iCalendarParser : iCalendarParserBase {
		#region ctor
		public iCalendarParser(iCalendarContainer calendars) : base(calendars) {
			if(calendars == null)
				Exceptions.ThrowArgumentNullException("calendars");
		}
		#endregion
		#region Properties
		protected iCalendarContainer Calendars { get { return (iCalendarContainer)Container; } }
		#endregion
		#region InitParse
		protected override void InitParse(StreamReader reader) {
			base.InitParse(reader);
			Calendars.Clear();
		}
		#endregion
	}
}
namespace DevExpress.XtraScheduler.iCalendar {
	public class iCalendarEntryParser : DevExpress.XtraScheduler.iCalendar.Native.iCalendarParserBase {
		public iCalendarEntryParser()
			: base(new iCalendarEntryContainer()) {
		}
		iCalendarEntryContainer ParseResult { get { return (iCalendarEntryContainer)Container; } }
		public iCalendarEntryContainer ParseString(string stringValue, Encoding encoding) {
			using (MemoryStream stream = new MemoryStream()) {
				using (StreamWriter writer = new StreamWriter(stream)) {
					writer.Write(stringValue);
					writer.Flush();
					StreamReader reader = CreateStreamReader(encoding, stream);
					Parse(reader);
				}
			}
			return ParseResult;
		}
		public iCalendarEntryContainer ParseString(string stringValue) {
			return ParseString(stringValue, Encoding.ASCII);
		}
		StreamReader CreateStreamReader(Encoding encoding, MemoryStream stream) {
			return new StreamReader(stream, encoding);
		}
	}
	public class iCalendarEntryContainer : ICalendarBodyItemContainer {
		readonly List<iCalendarBodyItem> items = new List<iCalendarBodyItem>();
		public List<iCalendarBodyItem> Items { get { return items; } }
		public iCalendarPropertyBase GetProperty(string propertyName) {
			return GetItem(propertyName) as iCalendarPropertyBase;
		}
		public object GetPropertyValue(string propertyName) {
			iCalendarPropertyBase property = GetItem(propertyName) as iCalendarPropertyBase;
			return property != null ? property.GetValue() : null;
		}
		public DateTime GetDateTimePropertyValue(string itemName) {
			object value = GetPropertyValue(itemName);
			if (!(value is DateTime))
				return DateTime.MinValue;
			return (DateTime)value;
		}
		public T GetPropertyValue<T>(string itemName) where T : class {
			object value = GetPropertyValue(itemName);
			return value as T;
		}
		public iCalendarBodyItem GetItem(string itemName) {
			return Items.Find(delegate(iCalendarBodyItem item) { return itemName == item.Name; });
		}
		public void AddObject(iCalendarBodyItem item) {
			Items.Add(item);
		}
	}
}
