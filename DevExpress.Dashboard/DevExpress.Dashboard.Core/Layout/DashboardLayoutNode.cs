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
using System.ComponentModel;
using System.Xml;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DataAccess.Native;
using DevExpress.DashboardCommon.Layout;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	public abstract class DashboardLayoutNode : IDashboardLayoutNode {
		enum LayoutItemInsertPosition { Above, Below, Left, Right }
		internal const double DefaultWeight = 1.0;
		readonly Locker changedLocker = new Locker();
		DashboardLayoutGroup parent;
		DashboardItem dashboardItem;
		string dashboardItemName;
		double weight;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardLayoutNodeDashboardItem")
#else
	Description("")
#endif
		]
		public DashboardItem DashboardItem {
			get {
				return dashboardItem;
			}
			set {
				if(dashboardItem != value) {
					dashboardItem = value;
					RaiseChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardLayoutNodeRoot"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public DashboardLayoutNode Root {
			get {
				if(parent != null)
					return parent.Root;
				else
					return this;
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardLayoutNodeParent"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public DashboardLayoutGroup Parent { get { return parent; } internal set { parent = value; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardLayoutNodeWeight"),
#endif
		DefaultValue(DefaultWeight)
		]
		public double Weight {
			get { return weight; }
			set {
				if (value != weight) {
					CheckWeight(value);
					weight = value;
					RaiseChanged();
				}
			}
		}
		internal double ActualRelativeWidth { get; set; }
		internal double ActualRelativeHeight { get; set; }		
		protected abstract bool IsGroup { get; }
		protected abstract DashboardLayoutGroupOrientation? OrientationInternal { get; set; }
		protected abstract IEnumerable<IDashboardLayoutNode> ChildNodesInternal { get; }
		Locker ChangedLocker { get { return Root.changedLocker; } }
		internal event EventHandler Changed;
		protected DashboardLayoutNode(DashboardItem dashboardItem, double weight) {
			this.dashboardItem = dashboardItem;
			CheckWeight(weight);
			this.weight = weight;
		}
		public void MoveLeft(DashboardLayoutNode layoutNode) {
			Move(LayoutItemInsertPosition.Left, layoutNode);
		}
		public void MoveLeft(DashboardLayoutNode layoutNode, double weight) {
			Move(LayoutItemInsertPosition.Left, layoutNode);
			Weight = weight;
		}
		public void MoveRight(DashboardLayoutNode layoutNode) {
			Move(LayoutItemInsertPosition.Right, layoutNode);
		}
		public void MoveRight(DashboardLayoutNode layoutNode, double weight) {
			Move(LayoutItemInsertPosition.Right, layoutNode);
			Weight = weight;
		}
		public void MoveAbove(DashboardLayoutNode layoutNode) {
			Move(LayoutItemInsertPosition.Above, layoutNode);
		}
		public void MoveAbove(DashboardLayoutNode layoutNode, double weight) {
			Move(LayoutItemInsertPosition.Above, layoutNode);
			Weight = weight;
		}
		public void MoveBelow(DashboardLayoutNode layoutNode) {
			Move(LayoutItemInsertPosition.Below, layoutNode);
		}
		public void MoveBelow(DashboardLayoutNode layoutNode, double weight) {
			Move(LayoutItemInsertPosition.Below, layoutNode);
			Weight = weight;
		}
		public void InsertLeft(DashboardItem dashboardItem) {
			DashboardLayoutItem layoutItem = new DashboardLayoutItem(dashboardItem, DefaultWeight);
			InsertLeft(layoutItem);
		}
		public void InsertLeft(DashboardLayoutNode layoutNode) {
			InsertLeftInternal(layoutNode);
		}
		public void InsertRight(DashboardItem dashboardItem) {
			DashboardLayoutItem layoutItem = new DashboardLayoutItem(dashboardItem, DefaultWeight);
			InsertRight(layoutItem);
		}
		public void InsertRight(DashboardLayoutNode layoutNode) {
			InsertRightInternal(layoutNode);
		}
		public void InsertAbove(DashboardItem dashboardItem) {
			DashboardLayoutItem layoutItem = new DashboardLayoutItem(dashboardItem, DefaultWeight);
			InsertAbove(layoutItem);
		}
		public void InsertAbove(DashboardLayoutNode layoutNode) {
			InsertAboveInternal(layoutNode);
		}
		public void InsertBelow(DashboardItem dashboardItem) {
			DashboardLayoutItem layoutItem = new DashboardLayoutItem(dashboardItem, DefaultWeight);
			InsertBelow(layoutItem);
		}
		public void InsertBelow(DashboardLayoutNode layoutNode) {
			InsertBelowInternal(layoutNode);
		}
		void CheckWeight(double weight) {
			if (weight <= 0)
				throw new ArgumentException(DashboardLocalizer.GetString(DashboardStringId.MessageIncorrectLayoutItemWeight));
		}
		protected void RaiseChanged() {
			Root.RaiseChangedInternal(this);
		}
		void RaiseChangedInternal(DashboardLayoutNode sender) {
			if(!ChangedLocker.IsLocked && Changed != null)
				Changed(sender, EventArgs.Empty);
		}
		protected internal abstract IEnumerable<DashboardLayoutNode> GetNodesRecursive(bool getItems, bool getGroups, bool getVisibleGroupsOnly);	   
		protected internal virtual void OnEndLoading(IEnumerable<DashboardItem> dashboardItems) {
			if(dashboardItem == null) {
				foreach(DashboardItem item in dashboardItems) {
					if(item.ComponentName == dashboardItemName) {
						dashboardItem = item;
						break;
					}
				}
			}
		}
		protected internal virtual void InsertLeftInternal(DashboardLayoutNode layoutNode) {
			if (parent.Orientation == DashboardLayoutGroupOrientation.Horizontal) {
				parent.ChildNodes.Insert(parent.ChildNodes.IndexOf(this), layoutNode);
			}
			else {
				DashboardLayoutGroup newGroup = new DashboardLayoutGroup(DashboardLayoutGroupOrientation.Horizontal, weight);
				parent.ChildNodes.Insert(parent.ChildNodes.IndexOf(this), newGroup);
				parent.ChildNodes.Remove(this);
				newGroup.ChildNodes.Add(layoutNode);
				newGroup.ChildNodes.Add(this);
			}
		}
		protected internal virtual void InsertRightInternal(DashboardLayoutNode layoutNode) {
			if (parent.Orientation == DashboardLayoutGroupOrientation.Horizontal) {
				parent.ChildNodes.Insert(parent.ChildNodes.IndexOf(this) + 1, layoutNode);
			}
			else {
				DashboardLayoutGroup newGroup = new DashboardLayoutGroup(DashboardLayoutGroupOrientation.Horizontal, weight);
				parent.ChildNodes.Insert(parent.ChildNodes.IndexOf(this), newGroup);
				parent.ChildNodes.Remove(this);
				newGroup.ChildNodes.Add(this);
				newGroup.ChildNodes.Add(layoutNode);
			}
		}
		protected internal virtual void InsertAboveInternal(DashboardLayoutNode layoutNode) {
			if (parent.Orientation == DashboardLayoutGroupOrientation.Vertical) {
				parent.ChildNodes.Insert(parent.ChildNodes.IndexOf(this), layoutNode);
			}
			else {
				DashboardLayoutGroup newGroup = new DashboardLayoutGroup(DashboardLayoutGroupOrientation.Vertical, weight);
				parent.ChildNodes.Insert(parent.ChildNodes.IndexOf(this), newGroup);
				parent.ChildNodes.Remove(this);
				newGroup.ChildNodes.Add(layoutNode);
				newGroup.ChildNodes.Add(this);
			}
		}
		protected internal virtual void InsertBelowInternal(DashboardLayoutNode layoutNode) {
			if (parent.Orientation == DashboardLayoutGroupOrientation.Vertical) {
				parent.ChildNodes.Insert(parent.ChildNodes.IndexOf(this) + 1, layoutNode);
			}
			else {
				DashboardLayoutGroup newGroup = new DashboardLayoutGroup(DashboardLayoutGroupOrientation.Vertical, weight);
				parent.ChildNodes.Insert(parent.ChildNodes.IndexOf(this), newGroup);
				parent.ChildNodes.Remove(this);
				newGroup.ChildNodes.Add(this);
				newGroup.ChildNodes.Add(layoutNode);
			}
		}
		protected abstract bool ContainsNode(DashboardLayoutNode layoutNode);
		void Move(LayoutItemInsertPosition insertPosition, DashboardLayoutNode layoutItem) {
			bool shouldLockChanged = Root == layoutItem.Root;
			if(ContainsNode(layoutItem))
				throw new InvalidOperationException(DashboardLocalizer.GetString(DashboardStringId.MessageIncorrectMoveTarget));
			if(shouldLockChanged)
				BeginUpdate();
			try {
				if(parent != null) {
					parent.ChildNodes.Remove(this);
					parent = null;
				}
				switch(insertPosition) {
					case LayoutItemInsertPosition.Above:
						layoutItem.InsertAbove(this);
						break;
					case LayoutItemInsertPosition.Below:
						layoutItem.InsertBelow(this);
						break;
					case LayoutItemInsertPosition.Left:
						layoutItem.InsertLeft(this);
						break;
					case LayoutItemInsertPosition.Right:
						layoutItem.InsertRight(this);
						break;
				}
			}
			finally {
				if(shouldLockChanged)
					EndUpdate();
			}
		}
		internal DashboardLayoutNode CopyTree() {
			return CopyTree(null);
		}
		protected virtual void CopyProperties(DashboardLayoutNode copy, DashboardLayoutGroup parentCopy) {
			copy.dashboardItem = dashboardItem;
			copy.weight = weight;
		}
		internal DashboardLayoutNode CopyTree(DashboardLayoutGroup parentCopy) {
			DashboardLayoutNode copy = CreateLayoutItem();
			CopyProperties(copy, parentCopy);
			return copy;
		}
		protected internal virtual void UpdateActualSizeCore() {
			if(parent != null) {
				double relativeSize = weight / parent.GetTotalChildrenWeight();
				if(parent.Orientation == DashboardLayoutGroupOrientation.Horizontal) {
					ActualRelativeHeight = parent.ActualRelativeHeight;
					ActualRelativeWidth = parent.ActualRelativeWidth * relativeSize;
				}
				else {
					ActualRelativeWidth = parent.ActualRelativeWidth;
					ActualRelativeHeight = parent.ActualRelativeHeight * relativeSize;
				}
			}
		}
		protected abstract DashboardLayoutNode CreateLayoutItem();
		protected abstract void AddChildNodeInternal(IDashboardLayoutNode node);
		internal void BeginUpdate() {
			ChangedLocker.Lock();
		}
		internal void CancelUpdate() {
			ChangedLocker.Unlock();
		}
		void EndUpdate() {
			CancelUpdate();
			if(!ChangedLocker.IsLocked)
				RaiseChanged();
		}
		bool IDashboardLayoutNode.IsGroup { get { return IsGroup; } }
		double IDashboardLayoutNode.Weight { get { return Weight; } set { Weight = value; } }
		string IDashboardLayoutNode.DashboardItemName { 
			get {
				if(DashboardItem != null)
					return DashboardItem.ComponentName;
				return null;
			} 
			set { dashboardItemName = value; }
		}
		DashboardLayoutGroupOrientation? IDashboardLayoutNode.Orientation { 
			get { return OrientationInternal; } 
			set { OrientationInternal = value; } 
		}
		IEnumerable<IDashboardLayoutNode> IDashboardLayoutNode.ChildNodes { get { return ChildNodesInternal; } }
		void IDashboardLayoutNode.AddChildNode(IDashboardLayoutNode node) {
			AddChildNodeInternal(node);
		}
	}
}
