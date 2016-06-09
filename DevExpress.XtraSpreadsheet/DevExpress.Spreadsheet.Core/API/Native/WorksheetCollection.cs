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
using DevExpress.Office;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace DevExpress.Spreadsheet {
	#region WorksheetCollection
	public interface WorksheetCollection : ISimpleCollection<Worksheet> {
		Worksheet this[string name] { get;}
		Worksheet Add();
		Worksheet Add(string name);
#if DATA_SHEET
		VirtualWorksheet AddVirtual(string name, IVirtualData virtualData);
#endif
		Worksheet Insert(int index);
		Worksheet Insert(int index, string name); 
		void Remove(Worksheet worksheet);
		void RemoveAt(int index);
		int IndexOf(Worksheet worksheet);
		bool Contains(Worksheet worksheet);
		bool Contains(string name);
		Worksheet ActiveWorksheet { get; set; }
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Utils;
	using DevExpress.Office.Utils;
	using DevExpress.XtraSpreadsheet.Utils;
	using ModelCell = DevExpress.XtraSpreadsheet.Model.ICell;
	using ModelCellKey = DevExpress.XtraSpreadsheet.Model.CellKey;
	using ModelCellPosition = DevExpress.XtraSpreadsheet.Model.CellPosition;
	using ModelWorkbook = DevExpress.XtraSpreadsheet.Model.DocumentModel;
	using ModelWorksheet = DevExpress.XtraSpreadsheet.Model.Worksheet;
	using DevExpress.XtraSpreadsheet.Localization;
	using System.Diagnostics;
	using DevExpress.Spreadsheet;
	using DevExpress.XtraPrinting;
	using DevExpress.XtraSpreadsheet.Internal;
#if !SL
	using System.Windows.Forms;
	using DevExpress.Compatibility.System.Windows.Forms;
#else
	using System.Windows.Controls;
#endif
	#region NativeWorksheetCollection
	partial class NativeWorksheetCollection : WorksheetCollection {
		readonly List<NativeWorksheet> innerList;
		readonly NativeWorkbook workbook;
		public NativeWorksheetCollection(NativeWorkbook workbook) {
			Guard.ArgumentNotNull(workbook, "workbook");
			this.workbook = workbook;
			this.innerList = new List<NativeWorksheet>();
		}
		#region Properties
		public NativeWorkbook NativeWorkbook { get { return this.workbook; } }
		protected internal ModelWorkbook ModelWorkbook { [DebuggerStepThrough] get { return this.workbook.ModelWorkbook; } }
		List<NativeWorksheet> InnerList { get { return innerList; } }
		public Worksheet this[int index] { 
			get {
				CheckWorksheetIndexArgument(index);
				return InnerList[index]; 
			} 
		}
		public int Count { get { return ModelWorkbook.Sheets.Count; } }
		public int InnerListCount { get { return InnerList.Count; } }
		#region Item
		public Worksheet this[string name] {
			get {
				int index = ModelWorkbook.Sheets.GetSheetIndexByName(name);
				if (index == int.MinValue)
					SpreadsheetExceptions.ThrowArgumentOutOfRangeException(XtraSpreadsheetStringId.Msg_ErrorWorksheetWithNameNotFound, "name");
				return this[index];
			}
		}
		#endregion
		#region ActiveWorksheet
		public Worksheet ActiveWorksheet {
			get {
				return this[this.ModelWorkbook.ActiveSheet.Name];
			}
			set {
				NativeWorkbook.CheckValid();
				Guard.ArgumentNotNull(value, "value");
				NativeWorksheet other = (NativeWorksheet)value;
				if(!Object.ReferenceEquals(this.workbook, other.Workbook))
					SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorWorksheetToBeDeletedNotFound);
				int index = InnerList.IndexOf(other);
				if(index < 0)
					SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorWorksheetToBeDeletedNotFound);
				this.ModelWorkbook.ActiveSheet = other.ModelWorksheet;
			}
		}
		#endregion
		#endregion
		#region GetEnumerator
		public IEnumerator<Worksheet> GetEnumerator() {
			return new EnumeratorAdapter<Worksheet, NativeWorksheet>(innerList.GetEnumerator());
		}
		#endregion
		#region IEnumerable.GetEnumerator
		IEnumerator IEnumerable.GetEnumerator() {
			return innerList.GetEnumerator();
		}
		#endregion
		#region ICollection members
		object ICollection.SyncRoot {
			get {
				ICollection collection = innerList;
				return collection.SyncRoot;
			}
		}
		bool ICollection.IsSynchronized {
			get {
				ICollection collection = innerList;
				return collection.IsSynchronized;
			}
		}
		void ICollection.CopyTo(Array array, int index) {
			Array.Copy(innerList.ToArray(), 0, array, index, innerList.Count);
		}
		#endregion
		#region Add
		public Worksheet Add() {
			return workbook.AddWorksheet("");
		}
		#endregion
		#region Add by name
		public Worksheet Add(string name) {
			return workbook.AddWorksheet(name);
		}
		#endregion
#if DATA_SHEET
		#region Add virtual
		public VirtualWorksheet AddVirtual(string name, IVirtualData virtualData){
			return workbook.AddVirtualWorksheet(name, virtualData);
		}
		#endregion
#endif
		#region IndexOf
		public int IndexOf(Worksheet worksheet) {
			return InnerList.IndexOf((NativeWorksheet)worksheet);
		}
		#endregion
		#region Remove
		public void Remove(Worksheet worksheet) {
			NativeWorkbook.CheckValid();
			Guard.ArgumentNotNull(worksheet, "worksheet");
			int index = InnerList.IndexOf((NativeWorksheet)worksheet);
			if (index < 0)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorWorksheetToBeDeletedNotFound);
			NativeWorkbook.ModelWorkbook.Sheets.RemoveAt(index);
		}
		#endregion
		#region RemoveAt
		public void RemoveAt(int index) {
			NativeWorkbook.CheckValid();
			CheckWorksheetIndexArgument(index);
			NativeWorkbook.ModelWorkbook.Sheets.RemoveAt(index);
		}
		#endregion
		#region InsertAt
		public Worksheet Insert(int index) {
			return this.Insert(index, String.Empty);
		}
		#endregion
		#region AddCore
		public void AddCore(NativeWorksheet worksheet) {
			InnerList.Add(worksheet);
		}
		#endregion
		#region InsertAt
		public Worksheet Insert(int index, string name) { 
			if (!String.IsNullOrEmpty(name))
				this.ModelWorkbook.ActiveSheet.CheckValidName(name); 
			if (index < 0 && index > ModelWorkbook.Sheets.Count)
				SpreadsheetExceptions.ThrowArgumentOutOfRangeException(XtraSpreadsheetStringId.Msg_ErrorIncorrectIndexToInsert, "index");
			Model.Worksheet newWorksheet = this.ModelWorkbook.CreateWorksheet(name);
			this.ModelWorkbook.Sheets.Insert(index, newWorksheet);
			this.ModelWorkbook.ActiveSheet = newWorksheet;
			return this[index];
		}
		#endregion
		#region SubscribeModelWorksheetsEvents
		protected internal void InternalAPI_AfterSheetInserted(object sender, Model.WorksheetCollectionChangedEventArgs e) {
			ModelWorksheet sheet = ModelWorkbook.Sheets[e.Index];
			NativeWorksheet nativeSheet;
#if DATA_SHEET
			Model.VirtualWorksheet virtualWorksheet = sheet as Model.VirtualWorksheet;
			if (virtualWorksheet != null)
				nativeSheet = new NativeVirtualWorksheet(NativeWorkbook, virtualWorksheet);
			else
#endif
				nativeSheet = new NativeWorksheet(NativeWorkbook, sheet);
			InnerList.Insert(e.Index, nativeSheet);
		}
		protected internal void InternalAPI_BeforeSheetRemoving(object sender, Model.WorksheetCollectionChangedEventArgs e) {
			NativeWorksheet nativeWorksheet = InnerList[e.Index];
			nativeWorksheet.IsValid = false;
			nativeWorksheet.Invalidate();
			InnerList.RemoveAt(e.Index);
		}
		protected internal void InternalAPI_AfterSheetCollectionCleared(object sender, EventArgs e) {
		}
		protected internal void InternalAPI_AfterWoksheetMoved(object sender, Model.WorksheetMovedEventArgs e) {
			NativeWorksheet nativeWorksheet = InnerList[e.OldIndex];
			InnerList.RemoveAt(e.OldIndex);
			InnerList.Insert(e.NewIndex, nativeWorksheet);
		}
		#endregion
		#region ClearCore
		protected internal void ClearCore(){
			InnerList.Clear();
		}
		#endregion
		#region Invalidate
		protected internal virtual void Invalidate() {
			InvalidateItems();
			InnerList.Clear();
		}
		#endregion
		#region InvalidateItems
		protected internal void InvalidateItems() {
			foreach (NativeWorksheet sheet in this) {
				sheet.Invalidate();
			}
		}
		#endregion
		#region Contains
		public bool Contains(Worksheet worksheet) {
			return InnerList.Contains((NativeWorksheet)worksheet);
		}
		public bool Contains(string name) {
			int index = ModelWorkbook.Sheets.GetSheetIndexByName(name);
			return index != int.MinValue;
		}
		#endregion
		void CheckWorksheetIndexArgument(int index) {
			if (!(index >= 0 && index < ModelWorkbook.Sheets.Count))
				SpreadsheetExceptions.ThrowArgumentOutOfRangeException(XtraSpreadsheetStringId.Msg_ErrorWorksheetIndexOutside, "index");
		}
	}
	#endregion
	#region NativeWorksheet
	partial class NativeWorksheet : Worksheet, IPrintable {
		#region IPrintable
		void IPrintable.AcceptChanges() {
			workbook.PrintableImplementation.AcceptChanges();
		}
		bool IPrintable.CreatesIntersectedBricks {
			get { return workbook.PrintableImplementation.CreatesIntersectedBricks; }
		}
		bool IPrintable.HasPropertyEditor() {
			return workbook.PrintableImplementation.HasPropertyEditor();
		}
		UserControl IPrintable.PropertyEditorControl {
			get { return workbook.PrintableImplementation.PropertyEditorControl; }
		}
		void IPrintable.RejectChanges() {
			workbook.PrintableImplementation.RejectChanges();
		}
		void IPrintable.ShowHelp() {
			workbook.PrintableImplementation.ShowHelp();
		}
		bool IPrintable.SupportsHelp() {
			return workbook.PrintableImplementation.SupportsHelp();
		}
		#endregion
		#region IBasePrintable
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
			workbook.PrintableImplementation.CreateArea(areaName, graph);
		}
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
			workbook.PrintableImplementation.Finalize(ps, link);
		}
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
			(workbook.PrintableImplementation as InnerSpreadsheetDocumentServer).ResetPrintableImplementation(true, Index);
			workbook.PrintableImplementation.Initialize(ps, link);
		}
		#endregion
	}
	#endregion
}
