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
using DevExpress.XtraRichEdit.Model;
using System.Globalization;
using DevExpress.XtraRichEdit.Native;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Fields;
namespace DevExpress.XtraRichEdit.Fields {
	public class InstructionCollection {
		readonly Dictionary<string, List<Token>> switches = new Dictionary<string, List<Token>>();
		readonly Dictionary<string, List<DocumentLogInterval>> switchIntervals = new Dictionary<string, List<DocumentLogInterval>>();
		readonly List<Token> arguments = new List<Token>();
		public IList<Token> Arguments { get { return arguments; } }
		public Dictionary<string, List<Token>> Switches { get { return switches; } }
		public Dictionary<string, List<DocumentLogInterval>> SwitchIntervals { get { return switchIntervals; } }
		public void AddSwitch(string fieldSwitch, DocumentLogInterval interval) {
			AddSwitch(fieldSwitch, interval, null);
		}
		public void AddSwitch(string fieldSwitch, DocumentLogInterval interval, Token fieldArgument) {
			List<Token> tokens;
			if (!switches.TryGetValue(fieldSwitch, out tokens)) {
				tokens = new List<Token>();
				tokens.Add(fieldArgument);
				switches.Add(fieldSwitch, tokens);
				List<DocumentLogInterval> intervals = new List<DocumentLogInterval>();
				intervals.Add(interval);
				switchIntervals.Add(fieldSwitch, intervals);
			}
			else {
				tokens.Add(fieldArgument);
				switchIntervals[fieldSwitch].Add(interval);
			}
		}
		public void AddArgument(Token fieldArgument) {
			arguments.Add(fieldArgument);
		}
		public int GetInt(string fieldSwitch) {
			List<Token> result;
			if (switches.TryGetValue("\\" + fieldSwitch, out result)) {
				if (result == null)
					return 0;
				Token token = GetDefaultToken(result);
				if (token == null)
					return 0;
				return Int32.Parse(token.Value);
			}
			else
				return 0;
		}
		public string GetString(string fieldSwitch) {
			List<Token> result;
			if (switches.TryGetValue("\\" + fieldSwitch, out result)) {
				if (result == null)
					return null;
				Token token = GetDefaultToken(result);
				if (token == null)
					return String.Empty;
				return token.Value;
			}
			else
				return null;
		}
		public string[] GetStringArray(string fieldSwitch) {
			List<Token> result;
			if (switches.TryGetValue("\\" + fieldSwitch, out result)) {
				if (result == null)
					return null;
				int count = result.Count;
				List<string> array = new List<string>();
				for (int i = 0; i < count; i++)
					if (result[i] != null)
						array.Add(result[i].Value);
				return array.ToArray();
			}
			else
				return null;
		}
		public bool GetBool(string fieldSwitch) {
			return switches.ContainsKey("\\" + fieldSwitch);
		}
		public int? GetNullableInt(string fieldSwitch) {
			List<Token> result;
			if (switches.TryGetValue("\\" + fieldSwitch, out result)) {
				if (result == null)
					return null;
				Token token = GetDefaultToken(result);
				if (token == null)
					return null;
				return Int32.Parse(token.Value);
			}
			else
				return null;
		}
		public string GetBase64String(string fieldSwitch, PieceTable pieceTable) {
			List<Token> tokens;
			if (switches.TryGetValue("\\" + fieldSwitch, out tokens)) {
				if (tokens == null)
					return null;
				Token token = GetDefaultToken(tokens);
				if (token == null)
					return null;
				RunInfo info = pieceTable.FindRunInfo(token.Position, token.Length);
				DataContainerRun run = pieceTable.Runs[info.Start.RunIndex] as DataContainerRun;
				if (run == null || run.DataContainer == null)
					return null;
				return Convert.ToBase64String(run.DataContainer.GetData());
			}
			return null;
		}
		Token GetDefaultToken(List<Token> tokens) {
			return tokens[tokens.Count - 1];
		}
		DocumentLogInterval GetDefaultInterval(List<DocumentLogInterval> intervals) {
			return intervals[intervals.Count - 1];
		}
		public Token GetArgument(int index) {
			return arguments[index];
		}
		public string GetArgumentAsString(int index) {
			if (index >= arguments.Count)
				return String.Empty;
			Token token = GetArgument(index);
			return token.Value;
		}
		public DocumentLogInterval GetArgumentAsDocumentInterval(int index) {
			Token token = GetArgument(index);
			return new DocumentLogInterval(token.Position, token.Length);
		}
		public DocumentLogInterval GetSwitchArgumentDocumentInterval(string fieldSwitch) {
			return GetSwitchArgumentDocumentInterval(fieldSwitch, false);
		}
		public DocumentLogInterval GetSwitchArgumentDocumentInterval(string fieldSwitch, bool includeFullInterval) {
			List<Token> result;
			if (!switches.TryGetValue("\\" + fieldSwitch, out result))
				return null;
			if (result == null)
				return null;
			Token token = GetDefaultToken(result);
			if (token == null)
				return null;
			if (includeFullInterval) {
				if (token.ActualKind == TokenKind.QuotedText)
					return new DocumentLogInterval(token.Position - 1, token.Length + 2);
				else if(token.ActualKind == TokenKind.Template)
					return new DocumentLogInterval(token.Position - 1, token.Length + 3);
			}
			return new DocumentLogInterval(token.Position, token.Length);
		}
		public DocumentLogInterval GetSwitchDocumentInterval(string fieldSwitch) {
			List<DocumentLogInterval> result;
			if (!switchIntervals.TryGetValue("\\" + fieldSwitch, out result))
				return null;
			return GetDefaultInterval(result);
		}
		public IEnumerable<string> GetAllSwitchKeys() {
			foreach(string key in Switches.Keys)
				yield return key.Substring(1);
		}
	}
	public class DocumentLogInterval {
		DocumentLogPosition start;
		int length;
		public DocumentLogInterval(DocumentLogPosition start, int length) {
			this.start = start;
			this.length = length;
		}
		public DocumentLogPosition Start { get { return start; } }
		public int Length { get { return length; } }
	}
}
