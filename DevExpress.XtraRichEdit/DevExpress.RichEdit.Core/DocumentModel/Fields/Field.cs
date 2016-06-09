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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model.History;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model {
	public interface ISupportHierarchy<T> where T : class, ISupportHierarchy<T> {
		T Parent { get; }
		bool Inside(T item);
	}
	#region Field
	public class Field : ICloneable<Field>, ISupportsCopyFrom<Field>, ISupportHierarchy<Field> {
		const byte isCodeViewMask = 1;
		const byte isNotCodeViewMask = 0xFE;
		const byte lockedMask = 2;
		const byte notLockedMask = 0xFD;
		#region Fields
		readonly FieldRunInterval code;
		readonly FieldRunInterval result;
		readonly PieceTable pieceTable;
		byte bitProperties;
		Field parent;
		int index = -1;
		bool disableUpdate;
		CalculatedFieldBase preparedCalculatedField;
		bool hideByParent;
		#endregion
		public Field(PieceTable pieceTable) {
			this.code = new FieldRunInterval();
			this.result = new FieldRunInterval();
			this.pieceTable = pieceTable;
		}
		internal Field(PieceTable pieceTable, int index)
			: this(pieceTable) {
			this.index = index;
		}
		#region Properties
		public FieldRunInterval Code { get { return code; } }
		public FieldRunInterval Result { get { return result; } }
		public PieceTable PieceTable { get { return pieceTable; } }
		public DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }		
		public bool IsCodeView { get { return (bitProperties & isCodeViewMask) != 0; } set { if (value)bitProperties |= isCodeViewMask; else bitProperties &= isNotCodeViewMask; } }
		public bool Locked { get { return (bitProperties & lockedMask) != 0; } set { if (value)bitProperties |= lockedMask; else bitProperties &= notLockedMask; } }
		public bool DisableUpdate {
			get { return disableUpdate; }
			set {
				if (value != disableUpdate)
					SetDisableUpdate();
			}
		}
		public RunIndex FirstRunIndex { get { return Code.Start; } }
		public RunIndex LastRunIndex { get { return Result.End; } }
		public Field Parent { get { return parent; } set { parent = value; } }
		public int Index { get { return index; } set { index = value; } }
		public CalculatedFieldBase PreparedCalculatedField { get { return preparedCalculatedField; } set { preparedCalculatedField = value; } }
		internal bool HideByParent { get { return hideByParent; } set { hideByParent = value; } }
		#endregion
		void SetDisableUpdate() {
			if (Index < 0) {
				ToggleDisableUpdate();
				return;
			}
			DocumentModel.BeginUpdate();
			try {
				DisableUpdateChangedHistoryItem item = new DisableUpdateChangedHistoryItem(PieceTable, Index);
				DocumentModel.History.Add(item);
				item.Execute();
			}
			finally {
				DocumentModel.EndUpdate();
			}			
		}
		protected internal void ToggleDisableUpdate() {
			this.disableUpdate = !this.disableUpdate;
		}
		protected internal bool ContainsRun(RunIndex runIndex) {
			return Code.Contains(runIndex) || Result.Contains(runIndex);
		}
		public virtual bool ContainsField(Field field) {
			return FirstRunIndex < field.FirstRunIndex && field.LastRunIndex < LastRunIndex;
		}
		#region ICloneable<Field> Members
		public Field Clone() {
			Field result = new Field(PieceTable);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		public Field CloneToNewPieceTable(PieceTable pieceTable) {
			Field result = new Field(pieceTable);
			result.CopyFrom(this);
			return result;
		}
		#region ISupportsCopyFrom<Field> Members
		public void CopyFrom(Field value) {
			Code.CopyFrom(value.Code);
			Result.CopyFrom(value.Result);
			bitProperties = value.bitProperties;
			disableUpdate = value.DisableUpdate;
			HideByParent = value.HideByParent;
		}
		#endregion
		#region ISupportHierarchy<Field> Members
		bool ISupportHierarchy<Field>.Inside(Field field) {
			return field.ContainsField(this);
		}
		#endregion
		public virtual int GetResultLength(PieceTable pieceTable) {
			int result = 0;
			for (RunIndex runIndex = Result.Start; runIndex <= Result.End - 1; runIndex++)
				result += pieceTable.Runs[runIndex].Length;
			return result;
		}
		public Field GetTopLevelParent() {
			Field result = this;
			while(result.Parent != null)
				result = result.Parent;
			return result;
		}
	}
	#endregion
	#region FieldCollection
	public class FieldCollection : IList<Field> {
		readonly List<Field> innerList = new List<Field>();
		readonly SequentialFieldManager sequentialFieldManager;
		public FieldCollection(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.sequentialFieldManager = new SequentialFieldManager(pieceTable);
		}
		public SequentialFieldManager SequentialFieldManager { get { return sequentialFieldManager; } }
		public Field First { get { return Count > 0 ? this[0] : null; } }
		public Field Last { get { return Count > 0 ? this[Count - 1] : null; } }
		public int Count { get { return innerList.Count; } }
		public void Add(Field field) {
			ClearCounters();
			innerList.Add(field);
		}
		public int LookupParentFieldIndex(int index) {
			for (; this[index].Parent != null; index++);
			return index;
		}
		public void ForEach(Action<Field> action) {
			Guard.ArgumentNotNull(action, "action");
			int count = innerList.Count;
			for (int i = 0; i < count; i++)
				action(innerList[i]);
		}
		protected internal virtual void ClearCounters() {
			sequentialFieldManager.ClearCounters();
		}
		#region IList<Field> Members
		public int IndexOf(Field item) {
			return innerList.IndexOf(item);
		}
		public void Insert(int index, Field item) {
			ClearCounters();
			innerList.Insert(index, item);
		}
		public void RemoveAt(int index) {
			ClearCounters();
			innerList.RemoveAt(index);
		}
		public Field this[int index] {
			get {
				return innerList[index];
			}
			set {
				ClearCounters();
				innerList[index] = value;
			}
		}
		#endregion
		#region ICollection<Field> Members
		public void Clear() {
			ClearCounters();
			innerList.Clear();
		}
		public bool Contains(Field item) {
			return innerList.Contains(item);
		}
		public void CopyTo(Field[] array, int arrayIndex) {
			innerList.CopyTo(array, arrayIndex);
		}
		public bool IsReadOnly {
			get {
				IList<Field> list = innerList;
				return list.IsReadOnly;
			}
		}
		public bool Remove(Field item) {
			ClearCounters();
			return innerList.Remove(item);
		}
		#endregion
		#region IEnumerable<Field> Members
		public IEnumerator<Field> GetEnumerator() {
			return innerList.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			System.Collections.IEnumerable enumerable = innerList;
			return enumerable.GetEnumerator();
		}
		#endregion
	}
	#endregion
	#region FieldRunInterval
	public class FieldRunInterval : ICloneable<FieldRunInterval>, ISupportsCopyFrom<FieldRunInterval> {
		#region Fields
		RunIndex start = new RunIndex(-1);
		RunIndex end = new RunIndex(-1);
		#endregion
		public FieldRunInterval(RunIndex start, RunIndex end) {
			this.start = start;
			this.end = end;
		}
		internal FieldRunInterval() {
		}
		#region Properties
		public RunIndex Start { get { return start; } }
		public RunIndex End { get { return end; } }
		#endregion
		protected internal virtual void SetInterval(RunIndex start, RunIndex end) {
			if (start < RunIndex.Zero)
				Exceptions.ThrowArgumentException("start", start);
			if (end < RunIndex.Zero || end < start)
				Exceptions.ThrowArgumentException("end", end);
			this.start = start;
			this.end = end;
		}
		protected internal virtual bool Contains(Field field) {
			return field.FirstRunIndex >= Start && field.LastRunIndex <= End;
		}
		public virtual bool Contains(RunIndex runIndex) {
			return runIndex >= Start && runIndex <= End;
		}
		public virtual string GetText(PieceTable pieceTable) {
			Debug.Assert(Start >= RunIndex.Zero && End >= RunIndex.Zero);
			StringBuilder result = new StringBuilder();
			for (RunIndex runIndex = Start; runIndex <= End; runIndex++)
				result.Append(pieceTable.GetRunPlainText(runIndex));
			return result.ToString();
		}
		protected internal virtual void ShiftRunIndex(int offset) {
			Debug.Assert(Start >= RunIndex.Zero && End >= RunIndex.Zero);
			RunIndex newStart = this.start + offset;
			if (newStart < RunIndex.Zero)
				Exceptions.ThrowArgumentException("offset", offset);
			this.start = newStart;
			this.end += offset;
		}
		protected internal virtual void ShiftEndRunIndex(int offset) {
			RunIndex newEnd = this.end + offset;
			if (newEnd < this.start)
				Exceptions.ThrowArgumentException("offset", offset);
			this.end = newEnd;
		}
		#region ICloneable<FieldRunInterval> Members
		public FieldRunInterval Clone() {
			FieldRunInterval result = new FieldRunInterval();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<FieldRunInterval> Members
		public void CopyFrom(FieldRunInterval value) {
			this.start = value.Start;
			this.end = value.End;
		}
		#endregion
	}
	#endregion
	#region FieldRunIndexComparable
	public class FieldRunIndexComparable : IComparable<Field> {
		readonly RunIndex runIndex;
		public FieldRunIndexComparable(RunIndex runIndex) {
			this.runIndex = runIndex;
		}
		#region IComparable<Field> Members
		public int CompareTo(Field field) {
			if (field.LastRunIndex < runIndex)
				return -1;
			if (field.LastRunIndex > runIndex)
				return 1;
			return 0;
		}
		#endregion
	}
	#endregion
	public class FieldUpdateOnLoadOptions {
		readonly bool updateDateField;
		readonly bool updateTimeField;
		public FieldUpdateOnLoadOptions(bool updateDateField, bool updateTimeField) {
			this.updateDateField = updateDateField;
			this.updateTimeField = updateTimeField;
		}
		public bool UpdateDateField { get { return updateDateField; } }
		public bool UpdateTimeField { get { return updateTimeField; } }		
	}
	public class FieldLastRunIndexComparable : IComparable<Field> {
		readonly RunIndex runIndex;
		public FieldLastRunIndexComparable(RunIndex runIndex) {
			this.runIndex = runIndex;
		}
		public int CompareTo(Field field) {
			if (field.LastRunIndex < runIndex)
				return -1;
			if (field.LastRunIndex > runIndex)
				return 1;
			return 0;
		}
	}
	public class FieldCodeStartRunIndexComparable : IComparable<Field> {
		readonly RunIndex runIndex;
		public FieldCodeStartRunIndexComparable(RunIndex runIndex) {
			this.runIndex = runIndex;
		}
		public int CompareTo(Field field) {
			if (field.Code.Start < runIndex)
				return -1;
			if (field.Code.Start > runIndex)
				return 1;
			return 0;
		}
	}
	public interface IFieldCalculatorService {
		CalculateFieldResult CalculateField(PieceTable pieceTable, Field field, MailMergeDataMode mailMergeDataMode, UpdateFieldOperationType updateType);
		void PrepareField(PieceTable pieceTable, Field field, UpdateFieldOperationType updateType);
		void BeginUpdateFieldsOnLoad(FieldUpdateOnLoadOptions options);
		void EndUpdateFieldsOnLoad();
	}
	[Flags]
	public enum FieldResultOptions {
		None = 0,
		MergeFormat = 1,
		CharFormat = 2,
		MailMergeField = 4,
		KeepOldResult = 8,
		HyperlinkField = 16,		
		DoNotApplyFieldCodeFormatting = 32,
		SuppressUpdateInnerCodeFields = 64,
		SuppressMergeUseFirstParagraphStyle = 128,
	}
	public class CalculateFieldResult : IDisposable {
		CalculatedFieldValue value;		
		readonly UpdateFieldOperationType updateType;
		public CalculateFieldResult(CalculatedFieldValue value, UpdateFieldOperationType updateType) {
			Guard.ArgumentNotNull(value, "value");
			this.value = value;			
			this.updateType = updateType;
		}
		public CalculatedFieldValue Value { get { return value; } }
		public FieldResultOptions Options { get { return value.Options; } }
		public UpdateFieldOperationType UpdateType { get { return updateType; } }
		public bool MergeFormat { get { return (Options & FieldResultOptions.MergeFormat) != 0; } }
		public bool CharFormat { get { return (Options & FieldResultOptions.CharFormat) != 0; } }
		public bool MailMergeField { get { return (Options & FieldResultOptions.MailMergeField) != 0; } }
		public bool KeepOldResult { get { return (Options & FieldResultOptions.KeepOldResult) != 0; } }
		public bool ApplyFieldCodeFormatting { get { return (Options & FieldResultOptions.DoNotApplyFieldCodeFormatting) == 0; } }
		public bool SuppressMergeUseFirstParagraphStyle { get { return (Options & FieldResultOptions.SuppressMergeUseFirstParagraphStyle) != 0; } }
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
		}
		public virtual void Dispose(bool disposing) {
			if (disposing) {
				if (this.value != null) {
					this.value.Dispose();
					this.value = null;
				}
			}
		}
		#endregion
	}   
}
