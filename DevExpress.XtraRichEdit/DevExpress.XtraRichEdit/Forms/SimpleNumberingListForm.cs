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
using System.ComponentModel;
using System.Globalization;
using System.Text;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Internal;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Forms {
	#region SimpleNumberingListForm
	[DXToolboxItem(false)]
	public partial class SimpleNumberingListForm : SimpleNumberingListFormBase {
		public SimpleNumberingListForm() {
			InitializeComponent();
		}
		public SimpleNumberingListForm(ListLevelCollection<ListLevel> levels, int levelIndex, RichEditControl control, int startNumber, IFormOwner formOwner)
			: base(levels, levelIndex, control, formOwner) {
			InitializeComponent();
			this.Controller.Start = startNumber;
			UpdateForm();
		}
	}
	#endregion
	#region DisplayFormatHelper
	public class DisplayFormatHelper {
		static readonly string[] fieldNames = { "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8" };
		const string fieldCodeTemplate = "MERGEFIELD {0}";
		delegate State State(char ch);
		State currentState;
		readonly SimpleRichEditControl control;
		readonly StringBuilder sb;
		readonly List<NumberingListTextSource> source;
		readonly ListLevelCollection<ListLevel> levels;
		class NumberingListTextSource {
			readonly DisplayFormatHelper owner;
			public NumberingListTextSource(DisplayFormatHelper owner) {
				Guard.ArgumentNotNull(owner, "owner");
				this.owner = owner;
			}
			ListLevelCollection<ListLevel> Levels { get { return owner.levels; } }
			public string F0 { get { return FormatLevelText(0); } }
			public string F1 { get { return FormatLevelText(1); } }
			public string F2 { get { return FormatLevelText(2); } }
			public string F3 { get { return FormatLevelText(3); } }
			public string F4 { get { return FormatLevelText(4); } }
			public string F5 { get { return FormatLevelText(5); } }
			public string F6 { get { return FormatLevelText(6); } }
			public string F7 { get { return FormatLevelText(7); } }
			public string F8 { get { return FormatLevelText(8); } }
			private string FormatLevelText(int levelIndex) {
				string formatstring = String.Format("{{{0}}}", levelIndex);
				return Paragraph.Format(formatstring, GetListCounters(), Levels);
			}
			private int[] GetListCounters() {
				int count = Levels.Count;
				int[] result = new int[count];
				for (int i = 0; i < count; i++) {
					result[i] = Levels[i].ListLevelProperties.Start;
				}
				return result;
			}
		}
		public DisplayFormatHelper(SimpleRichEditControl control, ListLevelCollection<ListLevel> levels, int levelIndex) {
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(levels, "levels");
			Guard.ArgumentNonNegative(levelIndex, "levelIndex");
			this.control = control;
			this.levels = levels;
			source = new List<NumberingListTextSource>();
			source.Add(new NumberingListTextSource(this));
			control.BeginUpdate();
			try {
				control.Options.MailMerge.DataSource = source;
				control.Options.MailMerge.ViewMergedData = true;
				control.Options.Fields.HighlightMode = FieldsHighlightMode.Always;
			}
			finally {
				control.EndUpdate();
			}
			sb = new StringBuilder();
		}
		public void SetDisplayFormat(string format) {
			currentState = TextState;
			sb.Length = 0;
			control.BeginUpdate();
			try {
				control.CreateNewDocument();
				control.DocumentModel.BeginUpdate();
				try {
					int count = format.Length;
					for (int i = 0; i < count; i++)
						ProcessChar(format[i]);
					ProcessEndOfFormat();
					Update();
				}
				finally {
					control.DocumentModel.EndUpdate();
				}
			}
			finally {
				control.EndUpdate();
			}
		}
		public void Update() {
			PieceTable pieceTable = control.DocumentModel.MainPieceTable;
			FieldCollection fields = pieceTable.Fields;
			int count = fields.Count;
			FieldUpdater updater = pieceTable.FieldUpdater;
			for(int i = 0; i < count; i++)
				updater.UpdateField(fields[i], MailMergeDataMode.ViewMergedData);
		}
		public string GetDisplayFormatString() {
			PieceTable pieceTable = control.DocumentModel.MainPieceTable;
			ChunkedStringBuilder textBuffer = pieceTable.TextBuffer;
			TextRunCollection runs = pieceTable.Runs;
			RunIndex index = RunIndex.Zero;
			RunIndex maxIndex = new RunIndex(runs.Count - 1);
			StringBuilder result = new StringBuilder();
			while (index <= maxIndex) {
				TextRunBase run = runs[index];
				if (run is FieldCodeStartRun)
					index = AppendFormatFromField(result, index);
				else if (run is TextRun)
					result.Append(run.GetTextFast(textBuffer).Replace("{", "{{").Replace("}", "}}"));
				index++;
			}
			return result.ToString();
		}
		RunIndex AppendFormatFromField(StringBuilder sb, RunIndex startIndex) {
			RunIndex index = startIndex;
			PieceTable pieceTable = control.DocumentModel.MainPieceTable;
			ChunkedStringBuilder textBuffer = pieceTable.TextBuffer;
			TextRunCollection runs = pieceTable.Runs;
			Debug.Assert(runs[index] is FieldCodeStartRun);
			index++;
			StringBuilder code = new StringBuilder();
			while (!(runs[index] is FieldCodeEndRun)) {
				TextRunBase run = runs[index];
				if (run is TextRun)
					code.Append(run.GetTextFast(textBuffer));
				index++;
			}
			while (!(runs[index] is FieldResultEndRun))
				index++;
			string[] codes = code.ToString().Trim().ToUpper(CultureInfo.InvariantCulture).Split(' ');
			if (codes.Length < 2)
				return index;
			string fieldCode = codes[0].Trim();
			int levelIndex = Array.IndexOf(fieldNames, codes[1].Trim());
			if (fieldCode == "MERGEFIELD" && levelIndex >= 0) {
				sb.AppendFormat("{{{0}}}", levelIndex);
			}
			return index;
		}
		void FlushAsText() {
			if (sb.Length > 0)
				control.DocumentModel.MainPieceTable.InsertText(control.DocumentModel.MainPieceTable.DocumentEndLogPosition, sb.ToString());
			sb.Length = 0;
		}
		void FlushAsFormat() {
			string fieldName = String.Format(String.Format("{{{0}}}", sb), fieldNames);
			string fieldCode = String.Format(fieldCodeTemplate, fieldName);
			DocumentLogPosition insertPosition = control.DocumentModel.MainPieceTable.DocumentEndLogPosition;
			control.DocumentModel.MainPieceTable.InsertText(insertPosition, fieldCode);
			control.DocumentModel.MainPieceTable.CreateField(insertPosition, fieldCode.Length);
			sb.Length = 0;
		}
		void ProcessChar(char ch) {
			State newState = currentState(ch);
			if (newState == null)
				Exceptions.ThrowInternalException();
			currentState = newState;
		}
		void ProcessEndOfFormat() {
			if (currentState == CloseBracketState)
				currentState = TextState;
			if (currentState != TextState)
				Exceptions.ThrowInternalException();
			FlushAsText();
		}
		State TextState(char ch) {
			switch (ch) {
				case '{':
					return OpenBracketState;
				case '}':
					return CloseBracketInTextState;
				default:
					sb.Append(ch);
					return TextState;
			}
		}
		State CloseBracketInTextState(char ch) {
			if (ch != '}')
				return null;
			else {
				sb.Append('}');
				return TextState;
			}
		}
		State OpenBracketState(char ch) {
			switch (ch) {
				case '{':
					sb.Append('{');
					return TextState;
				case '}':
					return null;
				default:
					FlushAsText();
					return FormatState(ch);
			}
		}
		State FormatState(char ch) {
			switch (ch) {
				case '}':
					FlushAsFormat();
					return CloseBracketState;
				case '{':
					return null;
				default:
					sb.Append(ch);
					return FormatState;
			}
		}
		State CloseBracketState(char ch) {
			switch (ch) {
				case '}':
					return null;
				case '{':
					return OpenBracketState;
				default:
					sb.Append(ch);
					return TextState;
			}
		}
	}
	#endregion
}
