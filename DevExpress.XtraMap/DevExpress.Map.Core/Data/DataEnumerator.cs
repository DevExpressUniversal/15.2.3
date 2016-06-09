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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data;
using DevExpress.Utils;
namespace DevExpress.Map.Native {
	public abstract class MapDataEnumeratorBase : IDisposable, IMapDataEnumerator {
		public abstract void Reset();
		public abstract bool MoveNext();
		object IEnumerator.Current { get { return GetCurrent(); } }
		protected abstract object GetCurrent();
		public virtual void Accept(IMapDataItem item, int itemIndex) {
		}
		int IMapDataEnumerator.GetCurrentRowIndex() {
			return GetCurrentRowIndex();
		}
		protected abstract int GetCurrentRowIndex();
		public void Dispose(){
		}
	}
	public abstract class MapDataEnumeratorBase<T> : MapDataEnumeratorBase, IEnumerator<T> {
		IMapDataController controller;
		protected IMapDataController Controller { get { return controller; } }
		protected MapDataEnumeratorBase(IMapDataController controller) {
			this.controller = controller;
		}
		public T Current { get { return (T)GetCurrent(); } }
	}
	public class ListDataEnumerator : MapDataEnumeratorBase<object> {
		int currentRowIndex = -1;
		public ListDataEnumerator(IMapDataController controller)
			: base(controller) {
		}
		protected override int GetCurrentRowIndex() {
			return currentRowIndex;
		}
		protected override object GetCurrent() {
			return Controller.GetRow(currentRowIndex);
		}
		public override void Reset() {
			currentRowIndex = -1;
		}
		public override bool MoveNext() {
			if(++currentRowIndex < Controller.ListSourceRowCount) {
				return true;
			}
			return false;
		}
		public override void Accept(IMapDataItem item, int itemIndex) {
			int sourceIndex = GetCurrentRowIndex();
			Controller.LoadItemProperties(item, sourceIndex);
			Controller.SetItemVisibleRowIndex(item, sourceIndex, new int[] { sourceIndex });
		}
	}
	public abstract class GroupBasedDataEnumerator : MapDataEnumeratorBase<object> {
		IPieSegmentDataLoader segmentLoader;
		int groupRowIndex = -1;
		protected GroupBasedDataEnumerator(IMapDataController controller, IPieSegmentDataLoader segmentLoader)
			: base(controller) {
			this.segmentLoader = segmentLoader;
		}
		protected internal IPieSegmentDataLoader SegmentLoader { get { return segmentLoader; } }
		public GroupRowInfo CurrentGroup { get { return GetGroupByIndex(GroupRowIndex); } }
		public int GroupRowIndex { get { return groupRowIndex; } }
		protected override int GetCurrentRowIndex() {
			return CurrentGroup != null ? CurrentGroup.ChildControllerRow : -1;
		}
		protected GroupRowInfo GetGroupByIndex(int index) {
			return index >= 0 && index < Controller.GroupInfo.Count ? Controller.GroupInfo[index] : null;
		}
		protected void SetGroupRowIndex(int index) {
			groupRowIndex = index;
		}
		protected void MoveGroupRowIndex(int offset) {
			groupRowIndex += offset;
		}
		protected override object GetCurrent() {
			int sourceRowIndex = GetCurrentRowIndex();
			return Controller.GetRow(sourceRowIndex);
		}
		public override void Reset() {
			groupRowIndex = -1;
		}
		protected int[] GetChildRowIndices(GroupRowInfo group) {
			int childCount = group.ChildControllerRowCount;
			int[] result = new int[childCount];
			for(int r = 0; r < childCount; r++) {
				result[r] = r + group.ChildControllerRow;
			}
			return result;
		}
		protected internal int[] GetListSourceRowIndices() {
			int[] childRowIndices = GetChildRowIndices(CurrentGroup);
			int count = childRowIndices.Length;
			int[] result = new int[count];
			for(int i = 0; i < count; i++) {
				result[i] = Controller.GetListSourceRowIndex(childRowIndices[i]);
			}
			return result;
		}
		protected object GetGroupSummaryValue(GroupRowInfo group, int index) {
			if(index < 0 || index > Controller.GroupSummary.ActiveCount - 1)
				return null;
			SummaryItem sumItem = Controller.GroupSummary[index];
			return group.GetSummaryValue(sumItem);
		}
	}
	public class AggregatedDataEnumerator : GroupBasedDataEnumerator {
		public AggregatedDataEnumerator(IMapDataController controller, IPieSegmentDataLoader segmentLoader)
			: base(controller, segmentLoader) {
		}
		protected int GetGroupLevel(int groupRowIndex) {
			GroupRowInfo row = GetGroupByIndex(groupRowIndex);
			return row != null ? row.Level : -1;
		}
		public override bool MoveNext() {
			int offset = CalculateGroupOffset();
			MoveGroupRowIndex(offset);
			if(GroupRowIndex < Controller.GroupRowCount) {
				return true;
			}
			return false;
		}
		protected int CalculateGroupOffset() {
			int offset = 1;
			int maxGroupIndex = Controller.GroupedColumnCount - 1;
			if(maxGroupIndex == 0)
				return offset;
			int level = GetGroupLevel(GroupRowIndex + offset);
			while(level >= 0) {
				level = GetGroupLevel(GroupRowIndex + offset);
				if(level == 0)
					break;
				offset++;
			}
			return offset;
		}
		public override void Accept(IMapDataItem item, int itemIndex) {
			int sourceIndex = GetCurrentRowIndex();
			Controller.LoadItemProperties(item, sourceIndex);
			Controller.SetItemVisibleRowIndex(item, sourceIndex, GetListSourceRowIndices());
			Controller.LoadItemAttributeArray(item, GetChildRowIndices(CurrentGroup));
			if(SegmentLoader != null)
				LoadPieSegments(item);
			else
				Controller.SetMapItemValueSummary(item, GetGroupSummaryValue(CurrentGroup, 0));
		}
		void LoadPieSegments(IMapDataItem item) {
			PieSegmentDataEnumerator en = new PieSegmentDataEnumerator(Controller, CurrentGroup, SegmentLoader);
			int iterateNumber = 0;
			while(en.MoveNext()) {
				en.Accept(item, iterateNumber++);
			}
		}
	}
	public class PieSegmentDataEnumerator : GroupBasedDataEnumerator {
		GroupRowInfo parentGroup;
		public PieSegmentDataEnumerator(IMapDataController controller, GroupRowInfo parentGroup, IPieSegmentDataLoader segmentLoader)
			: base(controller, segmentLoader) {
			Guard.ArgumentNotNull(parentGroup, "parentGroup");
			this.parentGroup = parentGroup;
			Reset();
		}
		protected GroupRowInfo GetGroupParent(int groupRowIndex) {
			GroupRowInfo row = GetGroupByIndex(groupRowIndex);
			return row != null ? row.ParentGroup : null;
		}
		public override void Reset() {
			SetGroupRowIndex(parentGroup.Index);
		}
		public override bool MoveNext() {
			int offset = 1;
			bool isGroup = IsSameParentGroup(GroupRowIndex + offset);
			if(!isGroup) return false;
			MoveGroupRowIndex(offset);
			if(GroupRowIndex < Controller.GroupRowCount) {
				return true;
			}
			return false;
		}
		protected bool IsSameParentGroup(int index) {
			GroupRowInfo parent = GetGroupParent(index);
			return object.Equals(parent, parentGroup);
		}
		public override void Accept(IMapDataItem item, int itemIndex) {
			int[] childrenRowIndices = GetChildRowIndices(CurrentGroup);
			object sum = GetGroupSummaryValue(CurrentGroup, 0);
			MapChartDataItemSegmentDataLoader loader = SegmentLoader as MapChartDataItemSegmentDataLoader;
			if(loader != null) {
				loader.SetEnumerator(this);
				loader.LoadItem(item, sum, childrenRowIndices);
			}
		}
	}
}
