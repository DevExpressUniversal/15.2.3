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
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Layout;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	public enum DashboardLayoutGroupOrientation { Vertical, Horizontal }
	public class DashboardLayoutGroup : DashboardLayoutNode {
		public const DashboardLayoutGroupOrientation DefaultOrientation = DashboardLayoutGroupOrientation.Horizontal;
		readonly DashboardLayoutNodeCollection children = new DashboardLayoutNodeCollection();
		DashboardLayoutGroupOrientation orientation;		
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardLayoutGroupChildNodes"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public DashboardLayoutNodeCollection ChildNodes { get { return children; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardLayoutGroupOrientation"),
#endif
		DefaultValue(DefaultOrientation)
		]
		public DashboardLayoutGroupOrientation Orientation {
			get {
				return orientation; 
			}
			set {
				if(orientation != value) {
					orientation = value;
					RaiseChanged();
				}
			} 
		}
		protected override bool IsGroup { get { return true; } }
		protected override DashboardLayoutGroupOrientation? OrientationInternal { 
			get { return Orientation; } 
			set { Orientation = (DashboardLayoutGroupOrientation)value; } 
		}
		protected override IEnumerable<IDashboardLayoutNode> ChildNodesInternal { get { return ChildNodes; } }
		bool IsRoot { get { return Parent == null; } }
		public DashboardLayoutGroup(DashboardItem dashboardItem)
			: this(DefaultOrientation, dashboardItem, DefaultWeight) {
		}
		public DashboardLayoutGroup(DashboardItem dashboardItem, double weight)
			: this(DefaultOrientation, dashboardItem, weight) {
		}
		public DashboardLayoutGroup()
			: this(DefaultOrientation, DefaultWeight) {
		}
		public DashboardLayoutGroup(DashboardLayoutGroupOrientation orientation, DashboardItem dashboardItem, double weight, params DashboardLayoutNode[] items)
			: base(dashboardItem, weight) {
			this.orientation = orientation;
			this.children.BeforeItemAdded += (sender, e) => {
				foreach(DashboardLayoutNode testedNode in e.Item.GetNodesRecursive(true, true, false)) {
					if(((DashboardLayoutGroup)Root).ContainsRecursive(testedNode))
						throw new InvalidOperationException(DashboardLocalizer.GetString(DashboardStringId.MessageDuplicatedLayoutItem));
					if(testedNode.DashboardItem != null && ((DashboardLayoutGroup)Root).ContainsRecursive(testedNode.DashboardItem))
						throw new InvalidOperationException(DashboardLocalizer.GetString(DashboardStringId.MessageLayoutDuplicatedDashboardItem));
				}
			};
			this.children.CollectionChanged += (sender, e) => {
				foreach(DashboardLayoutNode item in e.AddedItems)
					item.Parent = this;
				foreach(DashboardLayoutNode item in e.RemovedItems)
					item.Parent = null;
				RaiseChanged();
			};
			this.children.AddRange(items);
		}
		public DashboardLayoutGroup(DashboardLayoutGroupOrientation orientation, double weight, params DashboardLayoutNode[] items)
			: this(orientation, null, weight, items) {
		}
		public IEnumerable<DashboardLayoutItem> GetItemsRecursive() {
			foreach (DashboardLayoutItem item in GetNodesRecursive(true, false, false))
				yield return item;
		}
		public IEnumerable<DashboardLayoutGroup> GetGroupsRecursive() {
			foreach (DashboardLayoutGroup group in GetNodesRecursive(false, true, false))
				yield return group;
		}
		public IEnumerable<DashboardLayoutNode> GetNodesRecursive() {
			foreach (DashboardLayoutNode node in GetNodesRecursive(true, true, false))
				yield return node;
		}
		public DashboardLayoutItem FindRecursive(DashboardItem dashboardItem) {
			foreach (DashboardLayoutItem item in GetItemsRecursive())
				if(item.DashboardItem == dashboardItem)
					return item;
			return null;
		}
		public DashboardLayoutGroup FindRecursive(DashboardItemGroup itemGroup) {
			foreach(DashboardLayoutGroup group in GetGroupsRecursive())
				if(group.DashboardItem == itemGroup)
					return group;
			return null;
		}
		public void RemoveRecursive(DashboardItem dashboardItem) { 
			RemoveRecursive(FindRecursive(dashboardItem));
		}
		public void RemoveRecursive(DashboardLayoutNode layoutNode) { 
			if(ContainsRecursive(layoutNode) && layoutNode.Parent != null)
				layoutNode.Parent.children.Remove(layoutNode);
		}
		public bool ContainsRecursive(DashboardLayoutNode layoutNode) {
			foreach (DashboardLayoutNode item in GetNodesRecursive())
				if(item == layoutNode)
					return true;
			return false;
		}
		public bool ContainsRecursive(DashboardItem dashboardItem) { 
			return FindRecursive(dashboardItem) != null;
		}
		public bool ContainsRecursive(DashboardItemGroup itemGroup) {
			return FindRecursive(itemGroup) != null;
		}
		internal IEnumerable<DashboardLayoutNode> GetDashboardItemsRecursive() {
			foreach(DashboardLayoutNode item in GetNodesRecursive(true, true, true))
				yield return item;
		}
		internal double GetTotalChildrenWeight() {
			double totalWeight = 0;
			foreach(DashboardLayoutNode item in children)
				totalWeight += item.Weight;
			return totalWeight;
		}
		internal void UpdateActualSize() {
			DashboardLayoutNode root = Root;
			root.ActualRelativeHeight = 1;
			root.ActualRelativeWidth = 1;
			root.UpdateActualSizeCore();
		}
		protected internal override IEnumerable<DashboardLayoutNode> GetNodesRecursive(bool getItems, bool getGroups, bool getVisibleGroupsOnly) {
			if(getGroups && !getVisibleGroupsOnly || getGroups && getVisibleGroupsOnly && DashboardItem != null)
				yield return this;
			foreach (DashboardLayoutNode item in children)
				foreach(DashboardLayoutNode node in item.GetNodesRecursive(getItems, getGroups, getVisibleGroupsOnly))
					yield return node;
		}
		protected internal override void InsertLeftInternal(DashboardLayoutNode layoutNode) {
			if (IsRoot)
				UnrootNode().InsertLeftInternal(layoutNode);
			else
				base.InsertLeftInternal(layoutNode);
		}
		protected internal override void InsertRightInternal(DashboardLayoutNode layoutNode) {
			if (IsRoot)
				UnrootNode().InsertRightInternal(layoutNode);
			else
				base.InsertRightInternal(layoutNode);
		}
		protected internal override void InsertBelowInternal(DashboardLayoutNode layoutNode) {
			if (IsRoot)
				UnrootNode().InsertBelowInternal(layoutNode);
			else
				base.InsertBelowInternal(layoutNode);
		}
		protected internal override void InsertAboveInternal(DashboardLayoutNode layoutNode) {
			if (IsRoot)
				UnrootNode().InsertAboveInternal(layoutNode);
			else
				base.InsertAboveInternal(layoutNode);
		}
		protected internal override void UpdateActualSizeCore() {
			base.UpdateActualSizeCore();
			foreach (DashboardLayoutNode item in children)
				item.UpdateActualSizeCore();
		}
		protected internal override void OnEndLoading(IEnumerable<DashboardItem> dashboardItems) {
			base.OnEndLoading(dashboardItems);
			foreach (DashboardLayoutNode item in children)
				item.OnEndLoading(dashboardItems);
		}
		protected override void CopyProperties(DashboardLayoutNode copy, DashboardLayoutGroup parentCopy) {
			base.CopyProperties(copy, parentCopy);
			DashboardLayoutGroup copyGroup = (DashboardLayoutGroup)copy;
			copyGroup.orientation = orientation;
			foreach (DashboardLayoutNode item in children)
				copyGroup.children.Add(item.CopyTree(copyGroup));
		}
		protected override DashboardLayoutNode CreateLayoutItem() {
			return new DashboardLayoutGroup();
		}
		protected override bool ContainsNode(DashboardLayoutNode layoutItem) {
			return ContainsRecursive(layoutItem);
		}
		protected override void AddChildNodeInternal(IDashboardLayoutNode node) {
			ChildNodes.Add((DashboardLayoutNode)node);
		}
		DashboardLayoutGroup UnrootNode() {
			DashboardLayoutGroup copy = new DashboardLayoutGroup();
			copy.orientation = orientation;
			copy.Weight = Weight;
			while (children.Count > 0) {
				DashboardLayoutNode item = children[0];
				children.Remove(item);
				copy.children.Add(item);
			}
			children.Add(copy);
			return copy;
		}
	}
}
