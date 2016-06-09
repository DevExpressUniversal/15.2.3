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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Layout {
	#region IProtectedRangeResolver
	public interface IProtectedRangeResolver {
		bool IsAccessGranted(ModelProtectedRange protectedRange);
	}
	#endregion
	#region WebProtectionProvider
	public class WebProtectionProvider : IDisposable {
		#region Fields
		readonly DocumentModel workbook;
		readonly Dictionary<int, WebSheetProtection> sheetProtections = new Dictionary<int, WebSheetProtection>();
		#endregion
		public WebProtectionProvider(DocumentModel workbook) {
			this.workbook = workbook;
			SubscribeEvents();
		}
		public WebRangeProtection GetProtection(CellRange range) {
			CheckRange(range);
			WebSheetProtection protection = GetSheetProtection(range);
			return protection.GetProtection(range);
		}
		public bool CheckPassword(CellRange range, string password) {
			CheckRange(range);
			WebSheetProtection protection = GetSheetProtection(range);
			return protection.CheckPassword(range, password);
		}
		public void Clear() {
			foreach (WebSheetProtection item in sheetProtections.Values)
				item.Dispose();
			sheetProtections.Clear();
		}
		public void Reset() {
			foreach (WebSheetProtection item in sheetProtections.Values)
				item.Reset();
		}
		void CheckRange(CellRange range) {
			Worksheet sheet = range.Worksheet as Worksheet;
			if (sheet == null || !object.ReferenceEquals(sheet.Workbook, this.workbook))
				throw new ArgumentException("Invalid range or range from another workbook");
		}
		WebSheetProtection GetSheetProtection(CellRange range) {
			int sheetId = range.Worksheet.SheetId;
			if (sheetProtections.ContainsKey(sheetId))
				return sheetProtections[sheetId];
			WebSheetProtection item = new WebSheetProtection((Worksheet)range.Worksheet);
			sheetProtections.Add(sheetId, item);
			return item;
		}
		void SubscribeEvents() {
			this.workbook.ContentSetted += OnContentSetted;
			this.workbook.Sheets.AfterSheetRemoved += OnSheetRemoved;
			this.workbook.Sheets.AfterSheetCollectionCleared += OnSheetCollectionCleared;
		}
		void UnsubscribeEvents() {
			this.workbook.ContentSetted -= OnContentSetted;
			this.workbook.Sheets.AfterSheetRemoved -= OnSheetRemoved;
			this.workbook.Sheets.AfterSheetCollectionCleared -= OnSheetCollectionCleared;
		}
		void OnContentSetted(object sender, DocumentContentSettedEventArgs e) {
			if (e.ChangeType == DocumentModelChangeType.CreateEmptyDocument || e.ChangeType == DocumentModelChangeType.LoadNewDocument)
				Clear();
		}
		void OnSheetRemoved(object sender, WorksheetCollectionChangedEventArgs e) {
			int sheetId = e.Worksheet.SheetId;
			if (sheetProtections.ContainsKey(sheetId)) {
				sheetProtections[sheetId].Dispose();
				sheetProtections.Remove(sheetId);
			}
		}
		void OnSheetCollectionCleared(object sender, EventArgs e) {
			Clear();
		}
		#region IDisposable Members
		bool disposed = false;
		public void Dispose() {
			Dispose(true);
		}
		protected void Dispose(bool disposing) {
			if (disposing && !disposed) {
				UnsubscribeEvents();
				Clear();
				disposed = true;
			}
		}
		#endregion
	}
	#endregion
	#region WebSheetProtection
	public class WebSheetProtection : IProtectedRangeResolver, IDisposable {
		#region Fields
		readonly Worksheet sheet;
		readonly List<string> passwords = new List<string>();
		readonly HashSet<ModelProtectedRange> unlockedRanges = new HashSet<ModelProtectedRange>();
		readonly Dictionary<CellRange, WebRangeProtection> protectionTable = new Dictionary<CellRange, WebRangeProtection>();
		readonly List<WebRangeProtection> protectionItems = new List<WebRangeProtection>();
		#endregion
		public WebSheetProtection(Worksheet sheet) {
			this.sheet = sheet;
			SubscribeEvents();
			if (this.sheet.IsProtected)
				AddPassword(string.Empty);
		}
		public Worksheet Sheet { get { return sheet; } }
		public void Clear() {
			Reset();
			this.unlockedRanges.Clear();
			this.passwords.Clear();
		}
		public void Reset() {
			protectionTable.Clear();
			protectionItems.Clear();
		}
		public WebRangeProtection GetProtection(CellRange range) {
			if (!object.ReferenceEquals(sheet, range.Worksheet))
				throw new ArgumentException("Range belongs to another worksheet");
			if (protectionTable.ContainsKey(range))
				return protectionTable[range];
			WebRangeProtection item = new WebRangeProtection(range, this);
			protectionTable.Add(range, item);
			protectionItems.Add(item);
			return item;
		}
		public bool CheckPassword(CellRange range, string password) {
			if (!sheet.IsProtected)
				return true;
			if (string.IsNullOrEmpty(password))
				password = string.Empty;
			if (passwords.Contains(password))
				return true;
			foreach (ModelProtectedRange protectedRange in sheet.ProtectedRanges) {
				if (!unlockedRanges.Contains(protectedRange) && protectedRange.CellRange.Intersects(range) && protectedRange.Credentials.CheckPassword(password)) {
					unlockedRanges.Add(protectedRange);
					Invalidate(protectedRange.CellRange);
					AddPassword(password);
					return true;
				}
			}
			return false;
		}
		void AddPassword(string password) {
			if (!sheet.IsProtected)
				return;
			passwords.Add(password);
			foreach (ModelProtectedRange protectedRange in sheet.ProtectedRanges) {
				if (!unlockedRanges.Contains(protectedRange) && protectedRange.Credentials.CheckPassword(password)) {
					unlockedRanges.Add(protectedRange);
					Invalidate(protectedRange.CellRange);
				}
			}
		}
		void Invalidate(CellRangeBase range) {
			int count = protectionItems.Count;
			for (int i = count - 1; i >= 0; i--) {
				WebRangeProtection item = protectionItems[i];
				if (item.Range.Intersects(range)) {
					protectionTable.Remove(item.Range);
					protectionItems.RemoveAt(i);
				}
			}
		}
		void SubscribeEvents() {
			this.sheet.SheetProtectedChanged += OnSheetProtectedChanged;
			this.sheet.RangeInserting += OnRangeInserting;
			this.sheet.RangeRemoving += OnRangeRemoving;
			this.sheet.ColumnVisibilityChanged += OnColumnVisibilityChanged;
			this.sheet.RowVisibilityChanged += OnRowVisibilityChanged;
		}
		void UnsubscribeEvents() {
			this.sheet.SheetProtectedChanged -= OnSheetProtectedChanged;
			this.sheet.RangeInserting -= OnRangeInserting;
			this.sheet.RangeRemoving -= OnRangeRemoving;
			this.sheet.ColumnVisibilityChanged -= OnColumnVisibilityChanged;
			this.sheet.RowVisibilityChanged -= OnRowVisibilityChanged;
		}
		void OnSheetProtectedChanged(object sender, EventArgs e) {
			Clear();
			if (this.sheet.IsProtected)
				AddPassword(string.Empty);
		}
		void OnRangeInserting(object sender, SheetRangeInsertingEventArgs e) {
			InsertRangeNotificationContext notificationContext = e.NotificationContext;
			if (notificationContext.Mode == InsertCellMode.ShiftCellsDown)
				OnRowsChanging(notificationContext.Range.TopLeft.Row);
			else
				OnColumnsChanging(notificationContext.Range.TopLeft.Column);
		}
		void OnRangeRemoving(object sender, SheetRangeRemovingEventArgs e) {
			RemoveRangeNotificationContext notificationContext = e.NotificationContext;
			if (notificationContext.Mode == RemoveCellMode.ShiftCellsUp)
				OnRowsChanging(notificationContext.Range.TopLeft.Row);
			else if (notificationContext.Mode == RemoveCellMode.ShiftCellsLeft)
				OnColumnsChanging(notificationContext.Range.TopLeft.Column);
			else if (notificationContext.Mode == RemoveCellMode.NoShiftOrRangeToPasteCutRange)
				Invalidate(notificationContext.Range);
			else if (notificationContext.ClearFormat)
				Invalidate(notificationContext.Range);
		}
		void OnRowVisibilityChanged(object sender, RowColumnVisibilityChangedEventArgs e) {
			OnRowsChanging(e.StartIndex);
		}
		void OnColumnVisibilityChanged(object sender, RowColumnVisibilityChangedEventArgs e) {
			OnColumnsChanging(e.StartIndex);
		}
		void OnRowsChanging(int topRowIndex) {
			int count = protectionItems.Count;
			for (int i = count - 1; i >= 0; i--) {
				WebRangeProtection item = protectionItems[i];
				if (item.Range.BottomRight.Row >= topRowIndex) {
					protectionTable.Remove(item.Range);
					protectionItems.RemoveAt(i);
				}
			}
		}
		void OnColumnsChanging(int leftColumnIndex) {
			int count = protectionItems.Count;
			for (int i = count - 1; i >= 0; i--) {
				WebRangeProtection item = protectionItems[i];
				if (item.Range.BottomRight.Column >= leftColumnIndex) {
					protectionTable.Remove(item.Range);
					protectionItems.RemoveAt(i);
				}
			}
		}
		#region IProtectedRangeResolver Members
		bool IProtectedRangeResolver.IsAccessGranted(ModelProtectedRange protectedRange) {
			return unlockedRanges.Contains(protectedRange);
		}
		#endregion
		#region IDisposable Members
		bool disposed = false;
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing && !disposed) {
				UnsubscribeEvents();
				Clear();
				disposed = true;
			}
		}
		#endregion
	}
	#endregion
	#region WebRangeProtection
	public class WebRangeProtection {
		#region LockedState
		enum LockedState {
			Locked = 0,
			Protected = 1,
			Unlocked = 2
		}
		#endregion
		#region LockedStateData
		class LockedStateData {
			const byte stateProtected = 1;
			const byte stateUnlocked = 2;
			CellPosition topLeft;
			int width;
			int height;
			byte[,] states;
			public LockedStateData(CellPosition topLeft, CellPosition bottomRight) {
				this.topLeft = topLeft;
				width = bottomRight.Column - topLeft.Column + 1;
				height = bottomRight.Row - topLeft.Row + 1;
				states = new byte[width, height];
			}
			public LockedState this[int column, int row] {
				get {
					column -= topLeft.Column;
					row -= topLeft.Row;
					if (column < 0 || column >= width)
						return LockedState.Locked;
					if (row < 0 || row >= height)
						return LockedState.Locked;
					return (LockedState)states[column, row];
				}
				set {
					column -= topLeft.Column;
					row -= topLeft.Row;
					if (column < 0 || column >= width)
						return;
					if (row < 0 || row >= height)
						return;
					byte byteValue = (byte)value;
					if (states[column, row] < byteValue)
						states[column, row] = byteValue;
				}
			}
		}
		#endregion
		#region CellMergeHelper
		class CellMergeHelper {
			CellMergeRange currentRange = null;
			Dictionary<int, CellMergeGroup> rangeGroups = new Dictionary<int, CellMergeGroup>();
			public void Add(int column, int row) {
				if (currentRange != null && currentRange.TryMerge(column, row))
					return;
				currentRange = new CellMergeRange() { TopLeft = new CellPosition(column, row), BottomRight = new CellPosition(column, row) };
				AddToGroup(currentRange);
			}
			public List<CellMergeRange> GetMergedRanges() {
				List<CellMergeRange> result = new List<CellMergeRange>();
				foreach (CellMergeGroup group in rangeGroups.Values) {
					group.Merge();
					result.AddRange(group);
				}
				return result;
			}
			void AddToGroup(CellMergeRange range) {
				int key = range.TopLeft.Column;
				if (!rangeGroups.ContainsKey(key)) {
					CellMergeGroup group = new CellMergeGroup();
					group.Add(range);
					rangeGroups.Add(key, group);
				}
				else
					rangeGroups[key].Add(range);
			}
		}
		class CellMergeRange {
			public CellPosition TopLeft { get; set; }
			public CellPosition BottomRight { get; set; }
			public bool TryMerge(int column, int row) {
				bool canMerge = row == TopLeft.Row && column == (BottomRight.Column + 1);
				if (canMerge)
					BottomRight = new CellPosition(column, row);
				return canMerge;
			}
			public bool TryMerge(CellMergeRange range) {
				bool canMerge = BottomRight.Column == range.BottomRight.Column && BottomRight.Row == (range.TopLeft.Row - 1);
				if (canMerge)
					BottomRight = range.BottomRight;
				return canMerge;
			}
		}
		class CellMergeGroup : List<CellMergeRange> {
			public CellMergeGroup()
				: base() {
			}
			public void Merge() {
				int start = Count - 1;
				for (int i = start; i > 0; i--) {
					CellMergeRange range = this[i - 1];
					if (range.TryMerge(this[i]))
						RemoveAt(i);
				}
			}
		}
		#endregion
		#region Fields
		readonly CellRange range;
		readonly List<CellRange> unlockedRanges = new List<CellRange>();
		readonly List<CellRange> protectedRanges = new List<CellRange>();
		#endregion
		public WebRangeProtection(CellRange range, IProtectedRangeResolver resolver) {
			this.range = range;
			Worksheet sheet = (Worksheet)range.Worksheet;
			IsSheetProtected = sheet.IsProtected;
			if (IsSheetProtected)
				CalculateRanges(sheet, resolver);
		}
		#region Properties
		public CellRange Range { get { return range; } }
		public bool IsSheetProtected { get; private set; }
		public List<CellRange> UnlockedRanges { get { return unlockedRanges; } }
		public List<CellRange> ProtectedRanges { get { return protectedRanges; } }
		#endregion
		#region Internals
		void CalculateRanges(Worksheet sheet, IProtectedRangeResolver resolver) {
			LockedStateData states = new LockedStateData(range.TopLeft, range.BottomRight);
			CollectUnlockedCells(states, sheet);
			ApplyProtectedRanges(states, sheet, resolver);
			CalculateMergedRanges(states, sheet);
		}
		void CollectUnlockedCells(LockedStateData states, Worksheet sheet) {
			for (int rowIndex = range.TopLeft.Row; rowIndex <= range.BottomRight.Row; rowIndex++) {
				Row row = sheet.Rows.TryGetRow(rowIndex);
				for (int columnIndex = range.TopLeft.Column; columnIndex <= range.BottomRight.Column; columnIndex++) {
					ICell cell = row != null ? row.Cells.TryGetCell(columnIndex) : null;
					if (cell == null)
						cell = new FakeCell(new CellPosition(columnIndex, rowIndex), sheet);
					if (!cell.ActualProtection.Locked)
						states[columnIndex, rowIndex] = LockedState.Unlocked;
				}
			}
		}
		void ApplyProtectedRanges(LockedStateData states, Worksheet sheet, IProtectedRangeResolver resolver) {
			int count = sheet.ProtectedRanges.Count;
			for (int i = 0; i < count; i++) {
				ModelProtectedRange item = sheet.ProtectedRanges[i];
				VariantValue rangeIntersection = item.CellRange.IntersectionWith(range);
				if (rangeIntersection.Type == VariantValueType.CellRange) {
					LockedState itemState = resolver.IsAccessGranted(item) ? LockedState.Unlocked : LockedState.Protected;
					CellRangeBase itemRange = rangeIntersection.CellRangeValue;
					IEnumerable<CellKey> itemCellKeys = new Enumerable<CellKey>(itemRange.GetAllCellKeysEnumerator());
					foreach (CellKey key in itemCellKeys)
						states[key.ColumnIndex, key.RowIndex] = itemState;
				}
			}
		}
		void CalculateMergedRanges(LockedStateData states, Worksheet sheet) {
			CellMergeHelper unlockedHelper = new CellMergeHelper();
			CellMergeHelper protectedHelper = new CellMergeHelper();
			for (int rowIndex = range.TopLeft.Row; rowIndex <= range.BottomRight.Row; rowIndex++) {
				for (int columnIndex = range.TopLeft.Column; columnIndex <= range.BottomRight.Column; columnIndex++) {
					LockedState state = states[columnIndex, rowIndex];
					if (state == LockedState.Unlocked)
						unlockedHelper.Add(columnIndex, rowIndex);
					else if (state == LockedState.Protected)
						protectedHelper.Add(columnIndex, rowIndex);
				}
			}
			List<CellMergeRange> mergedRanges = unlockedHelper.GetMergedRanges();
			foreach (CellMergeRange item in mergedRanges)
				unlockedRanges.Add(new CellRange(sheet, item.TopLeft, item.BottomRight));
			mergedRanges = protectedHelper.GetMergedRanges();
			foreach (CellMergeRange item in mergedRanges)
				protectedRanges.Add(new CellRange(sheet, item.TopLeft, item.BottomRight));
		}
		#endregion
	}
	#endregion
}
