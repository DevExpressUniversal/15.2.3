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
using System.Runtime.InteropServices;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Office.History;
namespace DevExpress.XtraSpreadsheet.Model {
	#region SortBy
	public enum SortBy { 
		Value = 0,
		CellColor = 1,
		FontColor = 2,
		Icon = 3
	}
	#endregion
	#region IconSetType
	public enum IconSetType { 
		None = 0,
		Arrows3 = 1,
		ArrowsGray3 = 2,
		Flags3 = 3,
		TrafficLights13 = 4,
		TrafficLights23 = 5,
		Signs3 = 6,
		Symbols3 = 7,
		Symbols23 = 8,
		Stars3 = 9,
		Triangles3 = 10,
		Arrows4 = 11,
		ArrowsGray4 = 12,
		RedToBlack4 = 13,
		Rating4 = 14,
		TrafficLights4 = 15,
		Arrows5 = 16,
		ArrowsGray5 = 17,
		Rating5 = 18,
		Quarters5 = 19,
		Boxes5 = 20
	}
	#endregion
	#region SortConditionInfo
	public class SortConditionInfo : ICloneable<SortConditionInfo>, ISupportsCopyFrom<SortConditionInfo>, ISupportsSizeOf {
		#region Fields
		const uint MaskSortBy = 0x00000003;	 
		const uint MaskIconSet = 0x0000007C;	
		const uint MaskDescending = 0x00000080; 
		uint packedValues;
		string customList;
		int iconId;
		#endregion
		#region Properties
		#region SortBy
		public SortBy SortBy {
			get { return (SortBy)(packedValues & MaskSortBy); }
			set {
				packedValues &= ~MaskSortBy;
				packedValues |= (uint)value & MaskSortBy;
			}
		}
		#endregion
		#region IconSet
		public IconSetType IconSet {
			get { return (IconSetType)((packedValues & MaskIconSet) >> 2); }
			set {
				packedValues &= ~MaskIconSet;
				packedValues |= ((uint)value << 2) & MaskIconSet;
			}
		}
		#endregion
		public bool Descending { get { return GetBooleanVal(MaskDescending); } set { SetBooleanVal(MaskDescending, value); } }
		public string CustomList { get { return customList; } set { customList = value; } }
		public int IconId { get { return iconId; } set { iconId = value; } }
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
		#region ICloneable<SortConditionInfo> Members
		public SortConditionInfo Clone() {
			SortConditionInfo clone = new SortConditionInfo();
			clone.CopyFrom(this);
			return clone;
		}
		#endregion
		#region ISupportsCopyFrom<SortConditionInfo> Members
		public void CopyFrom(SortConditionInfo value) {
			this.packedValues = value.packedValues;
			this.customList = value.customList;
			this.iconId = value.iconId;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			SortConditionInfo info = obj as SortConditionInfo;
			if (info == null)
				return false;
			return this.packedValues == info.packedValues &&
				this.customList == info.customList && this.iconId == info.iconId;
		}
		public override int GetHashCode() {
			if (customList == null)
				return (int)(packedValues ^ iconId);
			return (int)(packedValues ^ customList.GetHashCode() ^ iconId);
		}
	}
	#endregion
	#region SortConditionInfoCache
	public class SortConditionInfoCache : UniqueItemsCache<SortConditionInfo> {
		public SortConditionInfoCache(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override SortConditionInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			SortConditionInfo info = new SortConditionInfo();
			info.SortBy = SortBy.Value;
			info.IconSet = IconSetType.None;
			info.Descending = false;
			info.IconId = -1;
			return info;
		}
	}
	#endregion
	#region SortCondition
	public class SortCondition : SpreadsheetUndoableIndexBasedObject<SortConditionInfo> {
		#region Fields
		CellRange sortReference;
		#endregion
		public SortCondition(Worksheet sheet, CellRange sortReference)
			: base(sheet) {
			Guard.ArgumentNotNull(sortReference, "sortReference");
			this.sortReference = sortReference;
		}
		#region Properties
		public Worksheet Sheet { get { return (Worksheet)DocumentModelPart; } }
		#region SortBy
		public SortBy SortBy {
			get { return Info.SortBy; }
			set {
				if (SortBy == value)
					return;
				SetPropertyValue(SetSortByCore, value);
			}
		}
		DocumentModelChangeActions SetSortByCore(SortConditionInfo info, SortBy value) {
			info.SortBy = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region IconSet
		public IconSetType IconSet {
			get { return Info.IconSet; }
			set {
				if (IconSet == value)
					return;
				SetPropertyValue(SetIconSetCore, value);
			}
		}
		DocumentModelChangeActions SetIconSetCore(SortConditionInfo info, IconSetType value) {
			info.IconSet = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Descending
		public bool Descending {
			get { return Info.Descending; }
			set {
				if (Descending == value)
					return;
				SetPropertyValue(SetDescendingCore, value);
			}
		}
		DocumentModelChangeActions SetDescendingCore(SortConditionInfo info, bool value) {
			info.Descending = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region IconId
		public int IconId {
			get { return Info.IconId; }
			set {
				if (IconId == value)
					return;
				SetPropertyValue(SetIconIdCore, value);
			}
		}
		DocumentModelChangeActions SetIconIdCore(SortConditionInfo info, int value) {
			info.IconId = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region SortReference
		public CellRange SortReference {
			get { return sortReference; }
			set {
				if(CellRange.Equals(sortReference, value))
					return;
				DocumentHistory history = DocumentModel.History;
				ChangeSortConditionSortReferenceHistoryItem item = new ChangeSortConditionSortReferenceHistoryItem(this, sortReference, value);
				history.Add(item);
				item.Execute();
			}
		}
		#endregion
		#region CustomList
		public string CustomList {
			get { return Info.CustomList; }
			set {
				if (CustomList == value)
					return;
				SetPropertyValue(SetCustomListCore, value);
			}
		}
		DocumentModelChangeActions SetCustomListCore(SortConditionInfo info, string value) {
			info.CustomList = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#endregion
		internal void SetSortReferenceInternal(CellRange sortReference) {
			this.sortReference = sortReference;
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<SortConditionInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.SortConditionInfoCache;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			Sheet.ApplyChanges(changeActions);
		}
		public bool IsDefault() {
			return Index == 0;
		}
	}
	#endregion
	#region SortConditionCollection
	public class SortConditionCollection : SimpleCollection<SortCondition> {
		#region Notifications
		protected internal void OnColumnsRemoving(int startIndex, int count) {
			RemoveByColumnIndex(startIndex, count);
			CorrectSortReferences(startIndex, count, false);
		}
		protected internal void OnColumnsInserting(int startIndex, int count) {
			CorrectSortReferences(startIndex, count, true);
		}
		#endregion
		#region Remove
		protected internal void RemoveByColumnIndex(int firstColumnIndex, int count) {
			int endColumnIndex = firstColumnIndex + count - 1;
			for (int i = 0; i < Count;) {
				SortCondition condition = InnerList[i];
				int conditionColumnIndex = condition.SortReference.TopLeft.Column;
				if (conditionColumnIndex >= firstColumnIndex && conditionColumnIndex <= endColumnIndex)
					Remove(condition);
				else
					++i;
			}
		}
		public override bool Remove(SortCondition item) {
			Guard.ArgumentNotNull(item, "item");
			DocumentHistory history = item.DocumentModel.History;
			SortConditionDeleteHistoryItem historyItem = new SortConditionDeleteHistoryItem(this, item);
			history.Add(historyItem);
			historyItem.Execute();
			return true;
		}
		public override void RemoveAt(int index) {
			SortCondition deletedCondition = InnerList[index];
			Remove(deletedCondition);
		}
		protected internal virtual void RemoveCore(SortCondition item) {
			Guard.ArgumentNotNull(item, "item");
			InnerList.Remove(item);
		}
		#endregion
		#region Insert
		public override int Add(SortCondition item) {
			Guard.ArgumentNotNull(item, "item");
			Insert(Count, item);
			return Count - 1;
		}
		protected internal virtual void AddCore(SortCondition item) {
			Guard.ArgumentNotNull(item, "item");
			InsertCore(item, Count);
		}
		public override void Insert(int index, SortCondition item) {
			Guard.ArgumentNotNull(item, "item");
			foreach (SortCondition condition in InnerList) {
				if (condition.SortBy == item.SortBy &&
					condition.SortReference.TopLeft.EqualsPosition(item.SortReference.TopLeft))
					return;
			}
			DocumentHistory history = item.DocumentModel.History;
			SortConditionInsertHistoryItem historyItem = new SortConditionInsertHistoryItem(this, item, index);
			history.Add(historyItem);
			historyItem.Execute();
		}
		protected internal virtual void InsertCore(SortCondition item, int index) {
			Guard.ArgumentNotNull(item, "item");
			InnerList.Insert(index, item);
		}
		#endregion
		#region Clear
		public override void Clear() {
			for (int i = Count - 1; i >= 0; --i)
				RemoveAt(i);
		}
		#endregion
		protected internal bool CorrectSortReferences(int firstColumnIndex, int count, bool insertMode) {
			int startIndex;
			int offset;
			if (insertMode) {
				startIndex = firstColumnIndex;
				offset = count;
			}
			else {
				startIndex = firstColumnIndex + count;
				offset = -count;
			}
			bool indexesChanged = false;
			foreach (SortCondition currentItem in InnerList)
				if (currentItem.SortReference.TopLeft.Column >= startIndex) {
					currentItem.SortReference = currentItem.SortReference.GetResized(offset, 0, offset, 0);
					indexesChanged = true;
				}
			return indexesChanged;
		}
		protected internal void CopyFrom(SortConditionCollection sourceCollection, 
			Action<SortConditionCollection, SortCondition, CellRange> addSortCondition, CellRange currentTargetRange) {
			Clear();
			int count = sourceCollection.Count;
			for (int i = 0; i < count; i++)
				addSortCondition(this, sourceCollection[i], currentTargetRange);
		}
	}
	#endregion
}
