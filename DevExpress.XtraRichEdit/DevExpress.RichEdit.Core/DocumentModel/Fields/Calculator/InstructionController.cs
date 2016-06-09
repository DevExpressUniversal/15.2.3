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
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Fields {
	#region InstructionController
	public class InstructionController : IDisposable {
		#region Fields
		static readonly char[] charsToEscape = new char[] { ' ', '\\', '"' };
		static readonly char[] brackets = new char[] { '(', ')', '[', ']' };
		InstructionCollection instructions;
		PieceTable pieceTable;
		Field field;
		DocumentLogPosition fieldCodeEnd;
		DeferredChanges deferredChanges;
		CalculatedFieldBase parsedInfo;
		HashSet<string> removingSwitches;
		#endregion
		public InstructionController(PieceTable pieceTable, CalculatedFieldBase parsedInfo, Field field) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			Guard.ArgumentNotNull(parsedInfo, "parsedInfo");
			Guard.ArgumentNotNull(field, "field");
			this.pieceTable = pieceTable;
			this.parsedInfo = parsedInfo;
			this.instructions = parsedInfo.Switches;
			this.field = field;
			UpdateFieldCodeEnd();
			this.deferredChanges = new DeferredChanges();
			this.removingSwitches = new HashSet<string>();
		}
		void UpdateFieldCodeEnd() {
			this.fieldCodeEnd = pieceTable.GetRunLogPosition(field.Code.End);
		}
		#region Properties
		public InstructionCollection Instructions { get { return instructions; } }
		public CalculatedFieldBase ParsedInfo { get { return parsedInfo; } }
		public Field Field { get { return field; } }
		public bool SuppressFieldsUpdateAfterUpdateInstruction { get; set; }
		protected internal PieceTable PieceTable { get { return pieceTable; } }
		protected internal DocumentModel DocumentModel { get { return PieceTable.DocumentModel; } }
		protected internal DocumentLogPosition FieldCodeEnd { get { return fieldCodeEnd; } }
		protected internal DeferredChanges DeferredChanges { get { return deferredChanges; } }
		#endregion
		#region Events
		EventHandler beforeUpdateFields;
		public event EventHandler BeforeUpdateFields {
			add { beforeUpdateFields += value; }
			remove { beforeUpdateFields -= value; }
		}
		protected virtual void RaiseBeforeUpdateFields() {
			if (beforeUpdateFields != null)
				beforeUpdateFields(this, EventArgs.Empty);
		}
		#endregion
		public string GetArgumentAsString(int index) {
			return Instructions.GetArgumentAsString(index);
		}
		public string GetFieldType() {
			DocumentFieldIterator iterator = new DocumentFieldIterator(PieceTable, Field);
			FieldScanner scanner = new FieldScanner(iterator, PieceTable);
			Token token = scanner.Scan();
			return token.Value;
		}
		public void UpdateFieldType(string fieldType) {
			DocumentFieldIterator iterator = new DocumentFieldIterator(PieceTable, Field);
			FieldScanner scanner = new FieldScanner(iterator, PieceTable);
			Token token = scanner.Scan();
			DocumentLogInterval interval = new DocumentLogInterval(token.Position, token.Length);
			DeferredChanges.UpdateFieldType(new UpdateStringFieldInfo(interval, fieldType));
			string[] nativeSwithces = ParsedInfo.GetNativeSwithes();
			string[] invariableSwitches = ParsedInfo.GetInvariableSwitches();
			int count = nativeSwithces.Length;
			for (int i = 0; i < count; i++) {
				bool shouldRemoveSwitch = true;
				for (int j = 0; j < invariableSwitches.Length; j++) {
					if (invariableSwitches[j] == nativeSwithces[i]) {
						shouldRemoveSwitch = false;
						break;
					}
				}
				if (shouldRemoveSwitch)
					RemoveSwitch(nativeSwithces[i]);
			}
		}
		public void RemoveSwitch(string fieldSwitch) {
			DeferredChanges.UndoSwitchModifications(fieldSwitch);
			if (!Instructions.GetBool(fieldSwitch))
				return;
			removingSwitches.Add(fieldSwitch);
			DocumentLogInterval switchInterval = Instructions.GetSwitchDocumentInterval(fieldSwitch);
			DocumentLogInterval argumentInterval = Instructions.GetSwitchArgumentDocumentInterval(fieldSwitch, true);
			DocumentLogInterval switchWithArgument;
			if (argumentInterval == null)
				switchWithArgument = switchInterval;
			else
				switchWithArgument = new DocumentLogInterval(switchInterval.Start, argumentInterval.Start + argumentInterval.Length - switchInterval.Start);
			DeferredChanges.UpdateSwitch(fieldSwitch, new UpdateStringFieldInfo(switchWithArgument));
		}
		public void SetSwitch(string fieldSwitch) {
			SetSwitch(fieldSwitch, String.Empty);
		}
		public void SetSwitch(string fieldSwitch, string argument) {
			SetSwitch(fieldSwitch, argument, true);
		}
		public void SetSwitch(string fieldSwitch, string argument, bool trimArgument) {
			removingSwitches.Remove(fieldSwitch);
			if (!Instructions.GetBool(fieldSwitch))
				DeferredChanges.AddSwitch(fieldSwitch, GetEscapedArgument(argument, trimArgument));
			else
				UpdateSwitch(fieldSwitch, argument);
		}
		public void SetSwitch(string fieldSwitch, DocumentModel argument) {
			SetSwitch(fieldSwitch, argument, true);
		}
		public void SetSwitch(string fieldSwitch, DocumentModel argument, bool wrapToField) {
			removingSwitches.Remove(fieldSwitch);
			if (!Instructions.GetBool(fieldSwitch))
				DeferredChanges.AddSwitch(fieldSwitch, argument, wrapToField);
			else
				UpdateSwitch(fieldSwitch, argument, wrapToField);
		}
		public bool ContainsSwitch(string fieldSwitch) {
			return Instructions.GetBool(fieldSwitch) || DeferredChanges.ContainsSwitch(fieldSwitch);
		}
		public bool IsSwitchRemoving(string fieldSwitch) { return removingSwitches.Contains(fieldSwitch); }
		public bool WillContainSwitch(string fieldSwitch) { 
			return ContainsSwitch(fieldSwitch) && ! removingSwitches.Contains(fieldSwitch);
		}
		protected internal virtual string GetNonUsedSwitch() {
			char[] alphabet = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
			List<int> values = new List<int>();
			values.Add(0);
			while (true) {
				string templateSwitch = "tg";
				for (int i = 0; i < values.Count; i++)
					templateSwitch += alphabet[values[i]];
				if (!ContainsSwitch(templateSwitch))
					return templateSwitch;
				values[0]++;
				for (int i = 0; values[i] >= alphabet.Length; i++) {
					values[i] = 0;
					if (i + 1 >= values.Count) {
						values.Add(0);
						break;
					}
					values[i + 1]++;
				}
			}
		}
		void UpdateSwitch(string fieldSwitch, string argument) {
			DocumentLogInterval argumentInterval = Instructions.GetSwitchArgumentDocumentInterval(fieldSwitch, true);
			if (argumentInterval == null)
				return;
			DeferredChanges.UpdateSwitch(fieldSwitch, new UpdateStringFieldInfo(argumentInterval, GetEscapedArgument(argument)));
		}
		void UpdateSwitch(string fieldSwitch, DocumentModel argument) {
			UpdateSwitch(fieldSwitch, argument, true);
		}
		void UpdateSwitch(string fieldSwitch, DocumentModel argument, bool wrapToField) {
			DocumentLogInterval argumentInterval = Instructions.GetSwitchArgumentDocumentInterval(fieldSwitch, true);
			if (argumentInterval == null)
				return;
			IUpdateFieldInfo fieldInfo;
			if (wrapToField)
				fieldInfo = new UpdateDocumentModelAsField(argumentInterval, argument);
			else
				fieldInfo = new UpdateDocumentModel(argumentInterval, argument);
			DeferredChanges.UpdateSwitch(fieldSwitch, fieldInfo);
		}
		public void SetArgument(int index, string argument) {
			if (index >= Instructions.Arguments.Count)
				DeferredChanges.AddArgument(index, GetEscapedArgument(argument));
			else
				UpdateArgument(index, argument);
		}
		public void SetArgument(int index, DocumentModel argument) {
			if (index >= Instructions.Arguments.Count)
				DeferredChanges.AddArgument(index, argument);
			else
				UpdateArgument(index, argument);
		}
		void UpdateArgument(int index, string argument) {
			DocumentLogInterval interval = GetArgumentAsLogInterval(index);
#if DEBUG || DEBUGTEST
			System.Diagnostics.Debug.Assert(interval != null);
#endif
			DeferredChanges.UpdateArgument(index, new UpdateStringFieldInfo(interval, GetEscapedArgument(argument)));
		}
		void UpdateArgument(int index, DocumentModel argument) {
			DocumentLogInterval interval = GetArgumentAsLogInterval(index);
#if DEBUG || DEBUGTEST
			System.Diagnostics.Debug.Assert(interval != null);
#endif
			DeferredChanges.UpdateArgument(index, new UpdateDocumentModelAsField(interval, argument));
		}
		DocumentLogInterval GetArgumentAsLogInterval(int index) {
			Token token = Instructions.GetArgument(index);
			switch(token.ActualKind) {
				case TokenKind.QuotedText:
					return new DocumentLogInterval(token.Position - 1, token.Length + 2);
				case TokenKind.Template:
					return new DocumentLogInterval(token.Position - 1, token.Length + 3);
				default:
					return new DocumentLogInterval(token.Position, token.Length);
			}
		}
		public static string GetEscapedFieldName(string fieldName) {
			if (fieldName.IndexOfAny(brackets) >= 0)
				fieldName = fieldName.Replace("(", "\\(").Replace(")", "\\)").Replace("[", "\\[").Replace("]", "\\]");
			return GetEscapedArgument(fieldName);
		}
		public static string GetEscapedArgument(string argument) {
			return GetEscapedArgument(argument, true);
		}
		public static string GetEscapedArgument(string argument, bool trimArgument) {
			if (String.IsNullOrEmpty(argument))
				return argument;
			if (trimArgument) argument = argument.Trim();
			if (argument.IndexOfAny(charsToEscape) >= 0)
				argument = String.Format("\"{0}\"", argument.Replace("\\", "\\\\").Replace("\"","\\\""));
			return argument;
		}
		public void ApplyDeferredActions() {
			DocumentModel.BeginUpdate();
			try {
				bool hasDeferredInsertion = ApplyDeferredInsertions();
				bool hasDeferredChanges = ApplyDeferredChanges();
				removingSwitches.Clear();
				if(!hasDeferredChanges && !hasDeferredInsertion)
					return;
				TrimEndSpace();
				if(!SuppressFieldsUpdateAfterUpdateInstruction) {
					RaiseBeforeUpdateFields();
					PieceTable.FieldUpdater.UpdateFieldAndNestedFields(Field);
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}			
		}
		void TrimEndSpace() {
			RunIndex index = Field.Code.End - 1;
			if (pieceTable.Runs[index].GetType() != typeof(TextRun))
				return;
			string plainText = pieceTable.GetRunPlainText(index);
			int offset = plainText.Length - 1;
			if (plainText[offset] == ' ')
				pieceTable.DeleteContent(pieceTable.GetRunLogPosition(index) + offset, 1, false, true, true);
		}
		bool ApplyDeferredInsertions() {
			int count = DeferredChanges.DeferredInsertions.Count;
			if (count == 0)
				return false;
			for (int i = 0; i < count; i++) {
				DeferredChanges.DeferredInsertions[i].Insert(PieceTable, FieldCodeEnd);
				PieceTable.InsertText(FieldCodeEnd, " ");
				UpdateFieldCodeEnd();
			}
			DeferredChanges.DeferredInsertions.Clear();
			return true;
		}
		bool ApplyDeferredChanges() {
			int count = DeferredChanges.DeferredUpdates.Count;
			if (count == 0)
				return false;
			DeferredChanges.DeferredUpdates.Sort(new UpdateFieldInfoComparer());
			for (int i = 0; i < count; i++) {
				DeferredChanges.DeferredUpdates[i].Update(PieceTable);
			}
			DeferredChanges.DeferredUpdates.Clear();
			return true;
		}
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing)
				ApplyDeferredActions();
		}
		#endregion
	}
	#endregion
	#region DeferredChanges
	public class DeferredChanges {
		#region Fields
		bool canUpdateFieldType = true;
		Dictionary<string, IInsertFieldInfo> insertingSwitches;
		Dictionary<string, IUpdateFieldInfo> updatingSwitches;
		List<int> argumentPositions;
		List<IInsertFieldInfo> deferredInsertions;
		List<IUpdateFieldInfo> deferredUpdates;
		#endregion
		public DeferredChanges() {
			this.deferredInsertions = new List<IInsertFieldInfo>();
			this.deferredUpdates = new List<IUpdateFieldInfo>();
			this.insertingSwitches = new Dictionary<string, IInsertFieldInfo>();
			this.updatingSwitches = new Dictionary<string, IUpdateFieldInfo>();
			this.argumentPositions = new List<int>();
		}
		public List<IInsertFieldInfo> DeferredInsertions { get { return deferredInsertions; } }
		public List<IUpdateFieldInfo> DeferredUpdates { get { return deferredUpdates; } }
		public void UpdateFieldType(IUpdateFieldInfo info) {
			if (!canUpdateFieldType)
				Exceptions.ThrowInternalException();
			DeferredUpdates.Add(info);
			canUpdateFieldType = false;
		}
		public void UpdateSwitch(string fieldSwitch, IUpdateFieldInfo info) {
			CheckSwitch(fieldSwitch, info);
			DeferredUpdates.Add(info);
		}
		public void UpdateArgument(int index, IUpdateFieldInfo info) {
			CheckArgument(index);
			DeferredUpdates.Add(info);
		}
		public void AddSwitch(string fieldSwitch, string argument) {			
			string switchWithArgument = String.Format("\\{0} {1}", fieldSwitch, argument);
			InsertStringFieldInfo insertStringFieldInfo = new InsertStringFieldInfo(switchWithArgument);
			CheckSwitch(fieldSwitch, insertStringFieldInfo);
			DeferredInsertions.Add(insertStringFieldInfo);
		}
		public void AddSwitch(string fieldSwitch, DocumentModel argument) {
			AddSwitch(fieldSwitch, argument, true);
		}
		public void AddSwitch(string fieldSwitch, DocumentModel argument, bool wrapToField) {
			IInsertFieldInfo fieldInfo;
			if (wrapToField)
				fieldInfo = new InsertStringWithDocumentModelAsField(fieldSwitch, argument);
			else
				fieldInfo = new InsertStringWithDocumentModel(fieldSwitch, argument);
			CheckSwitch(fieldSwitch, fieldInfo);
			DeferredInsertions.Add(fieldInfo);			
		}
		public void AddArgument(int index, string argument) {
			CheckArgument(index);
			DeferredInsertions.Add(new InsertStringFieldInfo(argument));
		}
		public void AddArgument(int index, DocumentModel argument) {
			CheckArgument(index);
			DeferredInsertions.Add(new InsertDocumentModelFieldInfo(argument));
		}
		public bool ContainsSwitch(string fieldSwitch) {
			return this.insertingSwitches.ContainsKey(fieldSwitch) || this.updatingSwitches.ContainsKey(fieldSwitch);
		}
		void CheckSwitch(string fieldSwitch, IInsertFieldInfo insertion) {
			UndoSwitchModifications(fieldSwitch);
			this.insertingSwitches.Add(fieldSwitch, insertion);
		}
		void CheckSwitch(string fieldSwitch, IUpdateFieldInfo update) {
			UndoSwitchModifications(fieldSwitch);
			this.updatingSwitches.Add(fieldSwitch, update);
		}
		public void UndoSwitchModifications(string fieldSwitch) {
			if(this.insertingSwitches.ContainsKey(fieldSwitch)) {
				this.deferredInsertions.Remove(this.insertingSwitches[fieldSwitch]);
				this.insertingSwitches.Remove(fieldSwitch);
			}
			if(this.updatingSwitches.ContainsKey(fieldSwitch)) {
				this.deferredUpdates.Remove(this.updatingSwitches[fieldSwitch]);
				this.updatingSwitches.Remove(fieldSwitch);
			}
		}
		void CheckArgument(int index) {
			if (this.argumentPositions.Contains(index))
				Exceptions.ThrowInternalException();
			this.argumentPositions.Add(index);
		}
	}
	#endregion
	#region IUpdateFieldInfo
	public interface IUpdateFieldInfo {
		DocumentLogInterval Interval { get; }
		void Update(PieceTable pieceTable);
	}
	#endregion
	#region UpdateFieldInfoBase (abstract class)
	public abstract class UpdateFieldInfoBase : IUpdateFieldInfo {
		readonly DocumentLogInterval interval;
		protected UpdateFieldInfoBase(DocumentLogInterval interval) {
			Guard.ArgumentNotNull(interval, "interval");
			this.interval = interval;
		}
		public DocumentLogInterval Interval { get { return interval; } }
		public void Update(PieceTable pieceTable) {
			pieceTable.DeleteContent(interval.Start, interval.Length, false, true, true, true);
			UpdateCore(pieceTable);
		}
		protected void TrimEndSpace(PieceTable pieceTable) {
			RunInfo info = pieceTable.FindRunInfo(Interval.Start, 1);
			if (info == null)
				return;
			DocumentModelPosition start = info.Start;
			if (pieceTable.Runs[start.RunIndex].GetType() != typeof(TextRun))
				return;
			if (pieceTable.GetRunPlainText(start.RunIndex)[start.RunOffset] == ' ')
				pieceTable.DeleteContent(start.RunStartLogPosition + start.RunOffset, 1, false, true, true);
		}
		protected abstract void UpdateCore(PieceTable pieceTable);
	}
	#endregion
	#region UpdateStringFieldInfo
	public class UpdateStringFieldInfo : UpdateFieldInfoBase {
		string newContent;
		public UpdateStringFieldInfo(DocumentLogInterval interval)
			: base(interval) {
		}
		public UpdateStringFieldInfo(DocumentLogInterval interval, string newContent)
			: base(interval) {
			this.newContent = newContent;
		}
		protected string NewContent { get { return newContent; } }
		protected override void UpdateCore(PieceTable pieceTable) {
			if (!String.IsNullOrEmpty(NewContent))
				pieceTable.InsertText(Interval.Start, NewContent);
			else
				TrimEndSpace(pieceTable);
		}
	}
	#endregion
	public abstract class UpdateDocumentModelFieldInfo : UpdateFieldInfoBase {
		DocumentModel model;
		protected UpdateDocumentModelFieldInfo(DocumentLogInterval interval, DocumentModel model)
			: base(interval) {
			Guard.ArgumentNotNull(model, "model");
			this.model = model;
		}
		protected DocumentModel DocumentModel { get { return model; } }
	}
	public class UpdateDocumentModelAsField : UpdateDocumentModelFieldInfo {
		public UpdateDocumentModelAsField(DocumentLogInterval interval, DocumentModel model)
			: base(interval, model) {
		}
		protected override void UpdateCore(PieceTable pieceTable) {
			Field field = CopyHelper.CopyAndWrapToField(DocumentModel.MainPieceTable, pieceTable, new DocumentLogInterval(DocumentLogPosition.Zero, DocumentModel.MainPieceTable.DocumentEndLogPosition - DocumentModel.MainPieceTable.DocumentStartLogPosition), Interval.Start);
			field.DisableUpdate = true;
		}
	}
	public class UpdateDocumentModel : UpdateDocumentModelFieldInfo {
		public UpdateDocumentModel(DocumentLogInterval interval, DocumentModel model)
			: base(interval, model) {
		}
		protected override void UpdateCore(PieceTable pieceTable) {
			CopyHelper.CopyCore(DocumentModel.MainPieceTable, pieceTable, new DocumentLogInterval(DocumentLogPosition.Zero, DocumentModel.MainPieceTable.DocumentEndLogPosition - DocumentModel.MainPieceTable.DocumentStartLogPosition), Interval.Start);
		}
	}
	public class UpdateFieldInfoComparer : IComparer<IUpdateFieldInfo> {
		public int Compare(IUpdateFieldInfo x, IUpdateFieldInfo y) {
			int xStart = ((IConvertToInt<DocumentLogPosition>)x.Interval.Start).ToInt();
			int yStart = ((IConvertToInt<DocumentLogPosition>)y.Interval.Start).ToInt();
			return Comparer<int>.Default.Compare(yStart, xStart);
		}
	}
	#region IInsertFieldInfo
	public interface IInsertFieldInfo {
		void Insert(PieceTable pieceTable, DocumentLogPosition pos);
	}
	#endregion
	#region InsertFieldInfo
	public class InsertStringFieldInfo : IInsertFieldInfo {
		string content;
		public InsertStringFieldInfo(string content) {
			this.content = content;
		}
		public void Insert(PieceTable pieceTable, DocumentLogPosition pos) {
			if (String.IsNullOrEmpty(content))
				return;
			pieceTable.InsertText(pos, content);
		}
	}
	public class InsertDocumentModelFieldInfo : IInsertFieldInfo {
		DocumentModel model;
		public InsertDocumentModelFieldInfo(DocumentModel model) {
			Guard.ArgumentNotNull(model, "model");
			this.model = model;
		}
		public void Insert(PieceTable pieceTable, DocumentLogPosition pos) {
			Field field = CopyHelper.CopyAndWrapToField(model.MainPieceTable, pieceTable, new DocumentLogInterval(DocumentLogPosition.Zero, model.MainPieceTable.DocumentEndLogPosition - model.MainPieceTable.DocumentStartLogPosition), pos);
			field.DisableUpdate = true;
		}
	}
	public abstract class InsertStringWithDocumentModelFieldInfoBase : IInsertFieldInfo {
		string fieldSwitch;
		DocumentModel argument;
		protected InsertStringWithDocumentModelFieldInfoBase(string fieldSwitch, DocumentModel argument) {
			Guard.ArgumentIsNotNullOrEmpty(fieldSwitch, "fieldSwitch");
			Guard.ArgumentNotNull(argument, "argument");
			this.fieldSwitch = String.Format("\\{0} ", fieldSwitch);
			this.argument = argument;
		}
		protected DocumentModel Argument { get { return argument; } }
		public void Insert(PieceTable pieceTable, DocumentLogPosition pos) {
			pieceTable.InsertText(pos, fieldSwitch);
			pos += fieldSwitch.Length;
			InsertArgument(pieceTable, pos);
		}
		protected abstract void InsertArgument(PieceTable pieceTable, DocumentLogPosition pos);
	}
	public class InsertStringWithDocumentModelAsField : InsertStringWithDocumentModelFieldInfoBase {
		public InsertStringWithDocumentModelAsField(string fieldSwitch, DocumentModel argument)
			: base(fieldSwitch, argument) {
		}
		protected override void InsertArgument(PieceTable pieceTable, DocumentLogPosition pos) {
			PieceTable argumentPieceTable = Argument.MainPieceTable;
			Field field = CopyHelper.CopyAndWrapToField(argumentPieceTable, pieceTable, new DocumentLogInterval(DocumentLogPosition.Zero, argumentPieceTable.DocumentEndLogPosition - argumentPieceTable.DocumentStartLogPosition), pos);
			field.DisableUpdate = true;
		}
	}
	public class InsertStringWithDocumentModel : InsertStringWithDocumentModelFieldInfoBase {
		public InsertStringWithDocumentModel(string fieldSwitch, DocumentModel argument)
			: base(fieldSwitch, argument) {
		}
		protected override void InsertArgument(PieceTable pieceTable, DocumentLogPosition pos) {
			PieceTable argumentPieceTable = Argument.MainPieceTable;
			CopyHelper.CopyCore(argumentPieceTable, pieceTable, new DocumentLogInterval(DocumentLogPosition.Zero, argumentPieceTable.DocumentEndLogPosition - argumentPieceTable.DocumentStartLogPosition), pos);
		}
	}
	#endregion
}
