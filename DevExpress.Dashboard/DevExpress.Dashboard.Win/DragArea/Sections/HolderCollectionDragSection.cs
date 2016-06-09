#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.DragDrop;
namespace DevExpress.DashboardWin.Native {
	public abstract class HolderCollectionDragSection<THolder> : CaptionDragSection where THolder : class, IDataItemHolder {
		readonly List<HolderCollectionDragGroup<THolder>> groups = new List<HolderCollectionDragGroup<THolder>>();
		IDataItemHolderCollection<THolder> holders;
		HolderCollectionDragGroup<THolder> newGroup;
		internal IList<HolderCollectionDragGroup<THolder>> Groups { get { return groups; } }
		public HolderCollectionDragGroup<THolder> NewGroup { get { return newGroup; } }
		int SpaceBetweenGroups { get { return Area.ParentControl.DrawingContext.Painters.GroupPainter.GroupIndent; } }
		public override int ActualGroupCount { get { return groups.Count + 1; } }
		public override bool AllowDragGroups { get { return groups.Count > 0; } }
		protected virtual bool AllowNonNumericMeasures { get { return false; } }
		protected internal override IEnumerable<DragGroup> ActualGroups {
			get {
				foreach(HolderCollectionDragGroup<THolder> group in groups)
					yield return group;
				yield return newGroup;
			}
		}
		protected HolderCollectionDragSection(DragArea area, string caption, string itemName, IDataItemHolderCollection<THolder> holders)
			: this(area, caption, itemName, null, holders) {
		}
		protected HolderCollectionDragSection(DragArea area, string caption, string itemName, string itemNamePlural, IDataItemHolderCollection<THolder> holders)
			: base(area, caption, itemName, itemNamePlural) {
			SetHolders(holders);
		}
		protected void SetHolders(IDataItemHolderCollection<THolder> holders) {
			this.holders = holders;
		}
		public bool IsNewGroup(HolderCollectionDragGroup<THolder> group) {
			return group == newGroup;
		}
		public int GetGroupIndex(HolderCollectionDragGroup<THolder> group) {
			return IsNewGroup(group) ? groups.Count : groups.IndexOf(group);
		}
		public void CleanupGroups() {
			foreach(DragGroup group in groups)
				group.ResetState();
			NewGroup.ResetState();
		}   
		void OnDataItemHolderChanged(object sender, EventArgs e) {
			UpdateGroups();
		}
		void UpdateGroups() {
			ClearGroups();
			FillGroups();
			Area.Arrange();
		}
		void ClearGroups() {
			foreach(THolder holder in holders)
				holder.Changed -= OnDataItemHolderChanged;
			foreach (HolderCollectionDragGroup<THolder> group in groups)
				group.Dispose();
			groups.Clear();
		}
		void FillGroups() {
			foreach (THolder holder in holders) {
				holder.Changed += OnDataItemHolderChanged;
				HolderCollectionDragGroup<THolder> foundGroup = FindGroup(holder);
				if (foundGroup != null)
					foundGroup.AddContainer(holder);
				else
					groups.Add(CreateGroup(holder));
			}
			UpdateNewGroup(null);
		}
		HolderCollectionDragGroup<THolder> FindGroup(THolder holder) {
			int currentGroupIndex = holder.GroupIndex;
			if(currentGroupIndex != -1) {
				foreach(HolderCollectionDragGroup<THolder> group in groups) {
					if(group.Holder.GroupIndex == currentGroupIndex)
						return group;
				}
			}
			return null;
		}
		public virtual void UpdateNewGroup(THolder holder) {
			if (newGroup != null)
				newGroup.Dispose();
			newGroup = CreateGroup(holder);
		}
		public override void ApplyHistoryItemRecord(DragAreaHistoryItemRecord record) {
			int groupIndex = record.GroupIndex;
			switch(record.Operation) {
				case DragAreaHistoryItemOperation.InsertGroup: {
					InsertGroup(groupIndex, (THolder)record.Data);
					break;
				}
				case DragAreaHistoryItemOperation.RemoveGroup:
					RemoveGroup((THolder)record.Data);
					break;
				case DragAreaHistoryItemOperation.CustomOperation:
					ICustomOperation<THolder> operation = record.Data as ICustomOperation<THolder>;
					THolder holder = groups[groupIndex].Holder;
					if(operation != null && holder != null)
						operation.Perform(holder);
					UpdateGroups();
					break;
				default:
					((groupIndex < 0 || groupIndex >= groups.Count) ? newGroup : groups[groupIndex]).ApplyHistoryItemRecord(record);
					break;
			}
		}
		public void InsertGroup(int groupIndex, THolder holder) {
			if(groupIndex < 0)
				UpdateNewGroup(holder);
			else
				holders.Insert(ContainerIndex(groupIndex), holder);
		}
		int ContainerIndex(int groupIndex) {
			int result = 0;
			for(int i = 0; i < groupIndex; i++) {
				result += groups[i].Holders.Count;
			}
			return result;
		}
		public void RemoveGroup(THolder holder) {
			holders.Remove(holder);
		}
		public override bool AcceptableDragObject(IDragObject dragObject) {
			if(dragObject.IsGroup)
				return true;
			return DragGroup.GetNumericMeasure(dragObject, Area.DashboardItem, !AllowNonNumericMeasures) != null;
		}
		protected internal override void Initialize() {
			base.Initialize();
			FillGroups();
			holders.CollectionChanged += OnDataItemHolderChanged;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				holders.CollectionChanged -= OnDataItemHolderChanged;
				ClearGroups();
				newGroup.Dispose();
			}
		}
		protected abstract HolderCollectionDragGroup<THolder> CreateGroupInternal(THolder holder);
		public HolderCollectionDragGroup<THolder> CreateGroup(THolder holder) {
			HolderCollectionDragGroup<THolder> group = CreateGroupInternal(holder);
			group.Initialize(this);
			return group;
		}
		protected override IDropAction GetGroupDropAction(IDragObject dragObject, Point point) {
			for(int i = 0; i < ActualGroupCount; i++) {
				HolderCollectionDragGroup<THolder> previousGroup = i > 0 ? (HolderCollectionDragGroup<THolder>)GetActualGroup(i - 1) : null;
				HolderCollectionDragGroup<THolder> nextGroup = i + 1 < ActualGroupCount ? (HolderCollectionDragGroup<THolder>)GetActualGroup(i + 1) : null;
				HolderCollectionDragGroup<THolder> currentGroup = (HolderCollectionDragGroup<THolder>)GetActualGroup(i);
				if(currentGroup.Bounds.Contains(point)) {
					if(!dragObject.IsGroup && IsOverDragItem(currentGroup, point))
						return null;
					if(dragObject.SameDragGroup(currentGroup))
						return GetGroupDropActionForSameDragGroup(dragObject, previousGroup, currentGroup, nextGroup, point);
					if(currentGroup.IsEmpty)
						return GetDropActionForEmptyGroup(dragObject, previousGroup, currentGroup, point);
					if(point.Y < currentGroup.Bounds.Y + currentGroup.Bounds.Height / 2 + 1)
						return GetGroupDropActionForTopHalf(dragObject, previousGroup, currentGroup);
					return GetDropActionForBottomHalfOfGroup(dragObject, nextGroup, currentGroup);
				}
				Rectangle spaceAfter = new Rectangle(currentGroup.Bounds.X, currentGroup.Bounds.Y + currentGroup.Bounds.Height, currentGroup.Bounds.Width, SpaceBetweenGroups);
				if(!currentGroup.IsEmpty && spaceAfter.Contains(point))
					return GetDropActionForSpaceAfterGroup(dragObject, nextGroup, currentGroup, point);
			}
			return null;
		}
		IDropAction GetGroupDropActionForSameDragGroup(IDragObject dragObject, HolderCollectionDragGroup<THolder> previousGroup, HolderCollectionDragGroup<THolder> currentGroup, HolderCollectionDragGroup<THolder> nextGroup, Point point) {
			if(dragObject.IsGroup)
				return new SelfDropAction(dragObject);
			if(point.Y < currentGroup.Bounds.Y + currentGroup.Bounds.Height / 2 + 1)
				return new InsertDragGroupDropAction<THolder>(dragObject, previousGroup, currentGroup);
			return new InsertDragGroupDropAction<THolder>(dragObject, currentGroup, nextGroup);
		}
		IDropAction GetDropActionForEmptyGroup(IDragObject dragObject, HolderCollectionDragGroup<THolder> previousGroup, HolderCollectionDragGroup<THolder> currentGroup, Point point) {
			if(dragObject.IsGroup && dragObject.SameDragGroup(previousGroup))
				return new SelfDropAction(dragObject);
			if(!dragObject.IsGroup && point.Y > currentGroup.Bounds.Y + currentGroup.Bounds.Height / 2 + 1)
				return null;
			return new InsertDragGroupDropAction<THolder>(dragObject, previousGroup, currentGroup);
		}
		IDropAction GetDropActionForSpaceAfterGroup(IDragObject dragObject, DragGroup nextGroup, DragGroup currentGroup, Point point) {
			if(dragObject.IsGroup && (dragObject.SameDragGroup(currentGroup) || dragObject.SameDragGroup(nextGroup)))
				return new SelfDropAction(dragObject);
			return new InsertDragGroupDropAction<THolder>(dragObject, (HolderCollectionDragGroup<THolder>)currentGroup, (HolderCollectionDragGroup<THolder>)nextGroup);
		}
		IDropAction GetDropActionForBottomHalfOfGroup(IDragObject dragObject, HolderCollectionDragGroup<THolder> nextGroup, HolderCollectionDragGroup<THolder> currentGroup) {
			if(dragObject.IsGroup && dragObject.SameDragGroup(nextGroup))
				return new SelfDropAction(dragObject);
			return new InsertDragGroupDropAction<THolder>(dragObject, currentGroup, nextGroup);
		}
		IDropAction GetGroupDropActionForTopHalf(IDragObject dragObject, HolderCollectionDragGroup<THolder> previousGroup, HolderCollectionDragGroup<THolder> currentGroup) {
			if(dragObject.IsGroup && dragObject.SameDragGroup(previousGroup))
				return new SelfDropAction(dragObject);
			return new InsertDragGroupDropAction<THolder>(dragObject, previousGroup, currentGroup);
		}
		bool IsOverDragItem(DragGroup group, Point point) {
			foreach(DragItem dragItem in group.ItemList)
				if(dragItem.Bounds.Contains(point))
					return true;
			return false;
		}
		public override int IndexOf(DragGroup item) {
			HolderCollectionDragGroup<THolder> group = item as HolderCollectionDragGroup<THolder>;
			if(group == null)
				return -1;
			int index = groups.IndexOf(group);
			if(index != -1)
				return index;
			return newGroup == group ? groups.Count : -1;
		}
		public override DragGroup GetActualGroup(int index) {
			if(index <= groups.Count - 1)
				return groups[index];
			if(index == ActualGroupCount - 1)
				return newGroup;
			throw new IndexOutOfRangeException();
		}
	}
}
