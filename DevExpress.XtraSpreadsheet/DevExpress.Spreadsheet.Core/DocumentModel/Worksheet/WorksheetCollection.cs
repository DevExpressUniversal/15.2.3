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
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Model {
	#region WorksheetCollectionBase (abstract class)
	public abstract class WorksheetCollectionBase<T> : SimpleCollection<T> where T : SheetBase {
		#region Fields
		readonly IModelWorkbook workbook;
		#endregion
		protected WorksheetCollectionBase(IModelWorkbook workbook) {
			Guard.ArgumentNotNull(workbook, "workbook");
			this.workbook = workbook;
		}
		#region Properties
		public T this[string name] {
			get {
				int index = GetSheetIndexByName(name);
				if (index < 0)
					return null;
				return InnerList[index];
			}
		}
		public int GetSheetIndexByName(string name) {
			int count = InnerList.Count;
			for (int i = 0; i < count; i++) {
				if (String.Compare(InnerList[i].Name, name, StringComparison.CurrentCultureIgnoreCase) == 0)
					return i;
			}
			return int.MinValue;
		}
		public IModelWorkbook Workbook { get { return this.workbook; } }
		public List<T> GetRange(string startSheetName, string endSheetName) {
			T startSheet = this[startSheetName];
			T endSheet = this[endSheetName];
			if (startSheet == null || endSheet == null)
				return null;
			int startIndex = this.IndexOf(startSheet);
			int endIndex = this.IndexOf(endSheet);
			return InnerList.GetRange(Math.Min(startIndex, endIndex), Math.Abs(endIndex - startIndex) + 1);
		}
		public List<T> GetRange() {
			return InnerList;
		}
		#endregion
		public string[] GetSheetNames() {
			int count = Count;
			string[] result = new string[Count];
			for (int i = 0; i < count; i++)
				result[i] = this[i].Name;
			return result;
		}
	}
	#endregion
	#region WorksheetCollection
	public class WorksheetCollection : WorksheetCollectionBase<Worksheet> {
		public WorksheetCollection(DocumentModel workbook)
			: base(workbook) {
		}
		#region Properties
		public new DocumentModel Workbook { get { return (DocumentModel)base.Workbook; } }
		#endregion
		#region Events
		#region BeforeSheetRemoving
		WorksheetCollectionChangedEventHandler onBeforeSheetRemoving;
		internal event WorksheetCollectionChangedEventHandler BeforeSheetRemoving { add { onBeforeSheetRemoving += value; } remove { onBeforeSheetRemoving -= value; } }
		protected internal virtual void RaiseBeforeSheetRemoving(Worksheet sheet, int sheetIndex) {
			if (onBeforeSheetRemoving != null) {
				WorksheetCollectionChangedEventArgs args = new WorksheetCollectionChangedEventArgs(sheet, sheetIndex);
				onBeforeSheetRemoving(this, args);
			}
		}
		#endregion
		#region AfterSheetRemoved
		WorksheetCollectionChangedEventHandler onAfterSheetRemoved;
		internal event WorksheetCollectionChangedEventHandler AfterSheetRemoved { add { onAfterSheetRemoved += value; } remove { onAfterSheetRemoved -= value; } }
		protected internal virtual void RaiseAfterSheetRemoved(Worksheet sheet, int sheetIndex) {
			if (onAfterSheetRemoved != null) {
				WorksheetCollectionChangedEventArgs args = new WorksheetCollectionChangedEventArgs(sheet, sheetIndex);
				onAfterSheetRemoved(this, args);
			}
		}
		#endregion
		#region AfterSheetInserted
		WorksheetCollectionChangedEventHandler onAfterSheetInserted;
		internal event WorksheetCollectionChangedEventHandler AfterSheetInserted { add { onAfterSheetInserted += value; } remove { onAfterSheetInserted -= value; } }
		protected internal virtual void RaiseAfterSheetInserted(Worksheet sheet, int sheetIndex) {
			if (onAfterSheetInserted != null) {
				WorksheetCollectionChangedEventArgs args = new WorksheetCollectionChangedEventArgs(sheet, sheetIndex);
				onAfterSheetInserted(this, args);
			}
		}
		#endregion
		#region BeforeSheetCollectionCleared
		EventHandler onBeforeSheetCollectionCleared;
		internal event EventHandler BeforeSheetCollectionCleared { add { onBeforeSheetCollectionCleared += value; } remove { onBeforeSheetCollectionCleared -= value; } }
		protected internal virtual void RaiseBeforeSheetCollectionCleared() {
			if (onBeforeSheetCollectionCleared != null) {
				EventArgs args = new EventArgs();
				onBeforeSheetCollectionCleared(this, args);
			}
		}
		#endregion
		#region AfterSheetCollectionCleared
		EventHandler onAfterSheetCollectionCleared;
		internal event EventHandler AfterSheetCollectionCleared { add { onAfterSheetCollectionCleared += value; } remove { onAfterSheetCollectionCleared -= value; } }
		protected internal virtual void RaiseAfterSheetCollectionCleared() {
			if (onAfterSheetCollectionCleared != null) {
				EventArgs args = new EventArgs();
				onAfterSheetCollectionCleared(this, args);
			}
		}
		#endregion
		#region AfterWoksheetMoved
		WorksheetMovedEventHandler onAfterWoksheetMoved;
		internal event WorksheetMovedEventHandler AfterWoksheetMoved { add { onAfterWoksheetMoved += value; } remove { onAfterWoksheetMoved -= value; } }
		protected internal virtual void RaiseAfterWoksheetMoved(Worksheet sheet, int oldIndex, int newIndex) {
			if (onAfterWoksheetMoved != null) {
				WorksheetMovedEventArgs args = new WorksheetMovedEventArgs(sheet, oldIndex, newIndex);
				onAfterWoksheetMoved(this, args);
			}
		}
		#endregion
		#endregion
		public override int Add(Worksheet sheet) {
			sheet.GenerateSheetName();
			InnerList.Add(sheet);
			RaiseAfterSheetInserted(sheet, InnerList.Count - 1);
			return Count - 1;
		}
		public Worksheet GetById(int sheetId) {
			int count = this.Count;
			for (int i = 0; i < count; i++) {
				if (this[i].SheetId == sheetId)
					return this[i];
			}
			return null;
		}
		protected internal int GetIndexById(int sheetId) {
			int count = this.Count;
			for (int i = 0; i < count; i++) {
				if (this[i].SheetId == sheetId)
					return i;
			}
			return -1;
		}
		public override void Insert(int index, Worksheet sheet) {
			sheet.GenerateSheetName();
			Workbook.CalculationChain.OnBeforeSheetInserting(sheet);
			InnerList.Insert(index, sheet);
			Workbook.CalculationChain.OnAfterSheetInserting(sheet);
			RaiseAfterSheetInserted(sheet, index);
		}
		public override bool Remove(Worksheet sheet) {
			if (!Object.ReferenceEquals(sheet.Workbook, this.Workbook))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorWorksheetToBeDeletedNotFound);
			if (Workbook.Sheets.Count < 2)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorDeleteSingularWorksheet);
			int index = InnerList.IndexOf(sheet);
			if (index < 0)
				return false;
			RemoveAt(index);
			return true;
		}
		public bool Move(int newIndex, Worksheet sheet) {
			if (!Object.ReferenceEquals(sheet.Workbook, this.Workbook))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorWorksheetToBeDeletedNotFound);
			if (Workbook.Sheets.Count < 2)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorDeleteSingularWorksheet);
			int oldIndex = InnerList.IndexOf(sheet);
			if (oldIndex < 0)
				return false;
			base.RemoveAt(oldIndex);
			base.Insert(newIndex, sheet);
			RaiseAfterWoksheetMoved(sheet, oldIndex, newIndex);
			return true;
		}
		public override void RemoveAt(int index) {
			if (Workbook.Sheets.Count < 2)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorDeleteSingularWorksheet);
			Worksheet sheet = InnerList[index];
			RaiseBeforeSheetRemoving(sheet, index);
			Workbook.CalculationChain.OnBeforeSheetRemoving(sheet);
			base.RemoveAt(index);
			Workbook.CalculationChain.OnAfterSheetRemoving(sheet);
			RaiseAfterSheetRemoved(sheet, index);
			sheet.Tag = null;
			Workbook.History.Clear();
		}
		public override void Clear() {
			RaiseBeforeSheetCollectionCleared();
			for (int i = Count - 1; i > 0; i--) {
				ClearSheetCore(i);
			}
			RaiseAfterSheetCollectionCleared();
		}
		void ClearSheetCore(int index) {
			Worksheet sheet = this[index];
			Remove(sheet);
			sheet.Clear();
			sheet = null;
		}
		public void ResetCachedContentVersions() {
			for (int i = Count - 1; i >= 0; i--)
				this[i].ResetCachedContentVersions();
		}
		public void ResetCachedTransactionVersions() {
			for (int i = Count - 1; i >= 0; i--)
				this[i].ResetCachedTransactionVersions();
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			int count = InnerList.Count;
			for (int i = 0; i < count; i++)
				InnerList[i].OnRangeRemoving(context);
		}
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			for (int i = 0; i < InnerList.Count; i++)
				InnerList[i].OnRangeInserting(context);
		}
	}
	#endregion
}
