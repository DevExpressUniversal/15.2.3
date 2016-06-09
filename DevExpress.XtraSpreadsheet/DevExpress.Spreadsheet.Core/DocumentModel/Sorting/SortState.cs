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
using System.Diagnostics;
using DevExpress.Office;
using System.Runtime.InteropServices;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Utils;
using DevExpress.Office.History;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region SortMethod
	public enum SortMethod { 
		None = 0,
		Stroke = 1,
		PinYin = 2
	}
	#endregion
	#region SortStateInfo
	public class SortStateInfo : ICloneable<SortStateInfo>, ISupportsCopyFrom<SortStateInfo>, ISupportsSizeOf {
		#region Fields
		const uint MaskSortMethod = 0x00000003;	 
		const uint MaskSortByColumns = 0x00000004;  
		const uint MaskCaseSensitive = 0x00000008;  
		uint packedValues;
		#endregion
		#region Properties
		#region SortMethod
		public SortMethod SortMethod {
			get { return (SortMethod)(packedValues & MaskSortMethod); }
			set {
				packedValues &= ~MaskSortMethod;
				packedValues |= (uint)value & MaskSortMethod;
			}
		}
		#endregion
		public bool SortByColumns { get { return GetBooleanVal(MaskSortByColumns); } set { SetBooleanVal(MaskSortByColumns, value); } }
		public bool CaseSensitive { get { return GetBooleanVal(MaskCaseSensitive); } set { SetBooleanVal(MaskCaseSensitive, value); } }
		#endregion
		#region GetBooleanVal/SetBooleanVal helpers
		void SetBooleanVal(uint mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		bool GetBooleanVal(uint mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
		#region ICloneable<SortStateInfo> Members
		public SortStateInfo Clone() {
			SortStateInfo clone = new SortStateInfo();
			clone.CopyFrom(this);
			return clone;
		}
		#endregion
		#region ISupportsCopyFrom<SortStateInfo> Members
		public void CopyFrom(SortStateInfo value) {
			this.packedValues = value.packedValues;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			SortStateInfo info = obj as SortStateInfo;
			if (info == null)
				return false;
			return this.packedValues == info.packedValues;
		}
		public override int GetHashCode() {
			return (int)packedValues;
		}
	}
	#endregion
	#region SortStateInfoCache
	public class SortStateInfoCache : UniqueItemsCache<SortStateInfo> {
		public SortStateInfoCache(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override SortStateInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			SortStateInfo info = new SortStateInfo();
			info.SortByColumns = false;
			info.CaseSensitive = false;
			info.SortMethod = SortMethod.None;
			return info;
		}
	}
	#endregion
	#region SortState
	public class SortState : SpreadsheetUndoableIndexBasedObject<SortStateInfo> {
		#region Fields
		SortConditionCollection sortConditions;
		CellRange sortRange;
		#endregion
		public SortState(Worksheet sheet)
			: base(sheet) {
			this.sortConditions = new SortConditionCollection();
		}
		#region Properties
		public Worksheet Sheet { get { return (Worksheet)DocumentModelPart; } }
		#region SortMethod
		public SortMethod SortMethod {
			get { return Info.SortMethod; }
			set {
				if (SortMethod == value)
					return;
				SetPropertyValue(SetSortMethodCore, value);
			}
		}
		DocumentModelChangeActions SetSortMethodCore(SortStateInfo info, SortMethod value) {
			info.SortMethod = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region SortByColumns
		public bool SortByColumns {
			get { return Info.SortByColumns; }
			set {
				if (SortByColumns == value)
					return;
				SetPropertyValue(SetSortByColumnsCore, value);
			}
		}
		DocumentModelChangeActions SetSortByColumnsCore(SortStateInfo info, bool value) {
			info.SortByColumns = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region CaseSensitive
		public bool CaseSensitive {
			get { return Info.CaseSensitive; }
			set {
				if (CaseSensitive == value)
					return;
				SetPropertyValue(SetCaseSensitiveCore, value);
			}
		}
		DocumentModelChangeActions SetCaseSensitiveCore(SortStateInfo info, bool value) {
			info.CaseSensitive = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region SortRange
		public CellRange SortRange {
			get { return sortRange; }
			set {
				if (CellRange.Equals(sortRange, value))
					return;
				if (value != null && !Object.ReferenceEquals(this.Sheet, value.Worksheet))
					throw new ArgumentException();
				DocumentHistory history = DocumentModel.History;
				ChangeSortStateSortRangeHistoryItem item = new ChangeSortStateSortRangeHistoryItem(this.Sheet, this, SortRange, value);
				history.Add(item);
				item.Execute();
			}
		}
		protected internal virtual void SetSortRangeCore(CellRange newValue) {
			if (newValue != null)
				Debug.Assert(Object.ReferenceEquals(this.Sheet, newValue.Worksheet));
			this.sortRange = newValue;
		}
		#endregion
		public SortConditionCollection SortConditions { get { return sortConditions; } }
		public bool IsDefault { get { return Index == 0 && sortConditions.Count == 0 && SortRange == null; } }
		#endregion
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<SortStateInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.SortStateInfoCache;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			Sheet.ApplyChanges(changeActions);
		}
		#region OnRangeRemoving
		internal void OnRangeRemovingShiftCellsLeft(CellRange cellRange) {
			if (SortRange == null || cellRange.TopLeft.Row > SortRange.BottomRight.Row || cellRange.BottomRight.Row < SortRange.TopLeft.Row || cellRange.TopLeft.Column > SortRange.BottomRight.Column)
				return;
			SortConditions.OnColumnsRemoving(cellRange.TopLeft.Column, cellRange.Width);
			if (SortConditions.Count == 0)
				Clear();
			else
				OnRangeRemovingShiftCellsLeftCore(cellRange);
		}
		void OnRangeRemovingShiftCellsLeftCore(CellRange cellRange) {
			int deletedWidth;
			if (cellRange.TopLeft.Column < SortRange.TopLeft.Column) {
				if (cellRange.BottomRight.Column < SortRange.TopLeft.Column)
					deletedWidth = -cellRange.Width;
				else
					deletedWidth = cellRange.TopLeft.Column - SortRange.TopLeft.Column;
				SortRange = SortRange.GetResized(deletedWidth, 0, deletedWidth, 0);
			}
			else {
				deletedWidth = cellRange.BottomRight.Column > SortRange.BottomRight.Column ? SortRange.BottomRight.Column : cellRange.BottomRight.Column;
				deletedWidth -= cellRange.TopLeft.Column - 1;
				SortRange = SortRange.GetResized(0, 0, -deletedWidth, 0);
			}
		}
		internal void OnRangeRemovingShiftCellsUp(CellRange cellRange) { 
			if (SortRange == null || cellRange.TopLeft.Column > SortRange.BottomRight.Column || cellRange.BottomRight.Column < SortRange.TopLeft.Column || cellRange.TopLeft.Row > SortRange.BottomRight.Row)
				return;
			int deletedHeight;
			if (cellRange.TopLeft.Row < SortRange.TopLeft.Row) {
				if (cellRange.BottomRight.Row < SortRange.TopLeft.Row)
					deletedHeight = -cellRange.Height;
				else
					deletedHeight = cellRange.TopLeft.Row - SortRange.TopLeft.Row;
				SortRange = SortRange.GetResized(0, deletedHeight, 0, deletedHeight);
			}
			else {
				deletedHeight = cellRange.TopLeft.Row - 1;
				deletedHeight -= cellRange.BottomRight.Row > SortRange.BottomRight.Row ? SortRange.BottomRight.Row : cellRange.BottomRight.Row;
				SortRange = SortRange.GetResized(0, 0, 0, deletedHeight);
			}
		}
		#endregion
		#region OnRangeInserting
		internal void OnRangeInsertingShiftRight(CellRange cellRange) {
			if (SortRange == null || cellRange.TopLeft.Row > SortRange.BottomRight.Row || cellRange.BottomRight.Row < SortRange.TopLeft.Row || cellRange.TopLeft.Column > SortRange.BottomRight.Column)
				return;
			SortConditions.OnColumnsInserting(cellRange.TopLeft.Column, cellRange.Width);
			int insertedWidth = cellRange.Width;
			if (cellRange.TopLeft.Column <= SortRange.TopLeft.Column)
				SortRange = SortRange.GetResized(insertedWidth, 0, insertedWidth, 0);
			else
				SortRange = SortRange.GetResized(0, 0, insertedWidth, 0);
		}
		internal void OnRangeInsertingShiftDown(CellRange cellRange) { 
			if (SortRange == null || cellRange.TopLeft.Column > SortRange.BottomRight.Column || cellRange.BottomRight.Column < SortRange.TopLeft.Column || cellRange.TopLeft.Row > SortRange.BottomRight.Row)
				return;
			int insertedHeight = cellRange.Height;
			if (cellRange.TopLeft.Row <= SortRange.TopLeft.Row)
				SortRange = SortRange.GetResized(0, insertedHeight, 0, insertedHeight);
			else
				SortRange = SortRange.GetResized(0, 0, 0, insertedHeight);
		}
		#endregion
		public override bool Equals(object obj) {
			SortState other = obj as SortState;
			if (other == null)
				return false;
			return base.Equals(other) && CellRange.Equals(sortRange, other.sortRange);
		}
		public override int GetHashCode() {
			return SortRange != null ? (int)(base.GetHashCode() ^ SortRange.GetHashCode()) : base.GetHashCode();
		}
		public void Clear() {
			DocumentModel.BeginUpdate();
			try {
				SortRange = null;
				SortConditions.Clear();
				ReplaceInfo(DocumentModel.Cache.SortStateInfoCache.DefaultItem, DocumentModelChangeActions.None);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public void Apply(IErrorHandler errorHandler) {
			ApplySortCommand command = new ApplySortCommand(this, errorHandler);
			command.Execute();
		}
		public override void CopyFrom(SpreadsheetUndoableIndexBasedObject<SortStateInfo> source) {
			CellRange unnesessaryRange = null;
			CopyFromCore(source, CopySortRange, AddSortCondition, unnesessaryRange);
		}
		void CopySortRange(SortState sortState, CellRange sourceSortRange, CellRange rangeForOffset) {
			CellRange targetCellRange = null;
			if (sourceSortRange != null)
				targetCellRange = new CellRange(Sheet, sourceSortRange.TopLeft, sourceSortRange.BottomRight);
			SortRange = targetCellRange;
		}
		void AddSortCondition(SortConditionCollection collection, SortCondition item, CellRange rangeForOffset) {
			CellRange sortReference = item.SortReference;
			CellRange targetRange = new CellRange(Sheet, sortReference.TopLeft, sortReference.BottomRight);
			SortCondition targetItem = new SortCondition(Sheet, targetRange);
			targetItem.CopyFrom(item);
			collection.Add(targetItem);
		}
		protected internal void CopyFromCore(SpreadsheetUndoableIndexBasedObject<SortStateInfo> source, 
			Action<SortState, CellRange, CellRange> setSortRange,
			Action<SortConditionCollection, SortCondition, CellRange> addSortCondition,
			CellRange currentTargetRange) {
			SortState sourceSortState = source as SortState;
			if (sourceSortState == null)
				throw new ArgumentException();
			DocumentModel.BeginUpdate();
			try {
				base.CopyFrom(source);
				setSortRange(this, sourceSortState.SortRange, currentTargetRange);
				SortConditions.CopyFrom(sourceSortState.SortConditions, addSortCondition, currentTargetRange);
			} finally {
				DocumentModel.EndUpdate();
			}
		}
	}
	#endregion
}
