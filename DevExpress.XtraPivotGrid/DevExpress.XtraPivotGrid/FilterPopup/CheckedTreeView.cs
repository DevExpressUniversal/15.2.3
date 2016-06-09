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

using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraPivotGrid.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using OpenCloseButtonPainter = DevExpress.Utils.Drawing.ObjectPainter;
namespace DevExpress.XtraEditors {
	public class RetrieveChildElementsEventArgs : EventArgs {
		readonly RetrieveChildElementsCallback callback;
		public RetrieveChildElementsEventArgs(object[] branch, RetrieveChildElementsCallback callback) {
			Branch = branch;
			IsAsync = false;
			this.callback = callback;
			if(!IsEmpty) {
				CheckStates = new List<bool?>(branch.Length);
				ChildElements = new List<object>(branch.Length);
			}
		}
		public RetrieveChildElementsEventArgs(object[] branch)
			: this(branch, null) {
		}
		public object[] Branch { get; private set; }
		public List<bool?> CheckStates { get; private set; }
		public List<object> ChildElements { get; private set; }
		public bool IsEmpty { get { return Branch == null || Branch.Length == 0; } }
		public bool IsLastLevel { get; set; }
		public bool IsAsync { get; set; }
		public void Callback() {
			if(this.callback != null)
				this.callback(this);
		}
	}
	public delegate void RetrieveChildElementsEventHandler(object sender, RetrieveChildElementsEventArgs e);
	public delegate void RetrieveChildElementsCallback(RetrieveChildElementsEventArgs e);
	public delegate void ExpandCompletedCallback();
	[ToolboxItem(false)]
	public class CheckedTreeViewControl : CheckedListBoxControl {
		static readonly object retrieveChildElements = new object();
		EventHandlerList events;
		bool isOneLevel;
		public CheckedTreeViewControl() : this(false) { }
		public CheckedTreeViewControl(bool isOneLevel) : base() {
			this.isOneLevel = isOneLevel;
			if(ItemsCore != null)
				((CheckedTreeViewItemCollection)ItemsCore).IsOneLevel = isOneLevel;
			events = new EventHandlerList();
			this.Items.TreeItemExpanded += new EventHandler(OnItemExpanded);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				events.Dispose();
			}
		}
		public event RetrieveChildElementsEventHandler RetrieveChildElements {
			add { this.Events.AddHandler(retrieveChildElements, value); }
			remove { this.Events.RemoveHandler(retrieveChildElements, value); }
		}
		public IActionsQueue ActionsQueue { get; set; }
		public bool IsQueueRunning {
			get {
				if (ActionsQueue == null)
					return false;
				return ActionsQueue.IsQueueRunning;
			}
		}
		public virtual int GetItemLevel(int index) {
			return Items[index].Level;
		}
		public virtual bool GetItemCanExpand(int index) {
			return Items[index].CanExpand;
		}
		public virtual bool GetItemIsLastLevel(int index) {
			return Items[index].IsLastLevel;
		}
		public virtual bool GetItemIsExpanded(int index) {
			return Items[index].IsExpanded;
		}
		public override void InvertCheckState() {
			foreach(CheckedTreeViewItem item in Items) {
				if(item.Level != 0) continue;
				item.InvertCheckState();
			}
		}
		public void CollapseLevelItems(CheckedTreeViewItem item) {
			ChangeLevelExpanded(item, false);
		}
		public void ExpandLevelItems(CheckedTreeViewItem item) {
			ChangeLevelExpanded(item, true);
		}
		protected virtual void SetSelectedIndex(CheckedTreeViewItem selectedItem) {
			SelectedIndex = Items.IndexOf(selectedItem);
		}
		public new CheckedTreeViewItem SelectedItem { get { return (CheckedTreeViewItem)base.SelectedItem; } }
		public new CheckedTreeViewItemCollection Items { get { return (CheckedTreeViewItemCollection)base.Items; } }
		protected virtual void RaiseRetrieveChildElements(object[] branch, RetrieveChildElementsCallback callback) {
			RetrieveChildElementsEventHandler handler = (RetrieveChildElementsEventHandler)this.Events[retrieveChildElements];
			if(handler != null) {
				RetrieveChildElementsEventArgs args = new RetrieveChildElementsEventArgs(branch, callback);
				handler(this, args);
				if(!args.IsAsync)
					args.Callback();
			}
		}
		protected override ListBoxItemCollection CreateItemsCollection() {
			return new CheckedTreeViewItemCollection(isOneLevel);
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new CheckedTreeViewInfo(this);
		}
		protected override BaseControlPainter CreatePainter() {
			return new CheckedTreeViewPainter();
		}
		protected void CheckedListBoxSetItemCheckStateCore(int index, CheckState value) {
			base.SetItemCheckStateCore(index, value);
		}
		protected override void SetItemCheckStateCore(int index, CheckState value) {
			BeginUpdate();
			CheckedListBoxSetItemCheckStateCore(index, value);
			Items[index].UpdateCheckState();
			EndUpdate();
		}
		protected virtual void OnItemExpanded(object sender, EventArgs e) {
			if(!IsBoundMode) base.OnResize(e);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			if(e.Button == MouseButtons.Left) {
				CheckedTreeViewInfo.TreeItemInfo itemViewInfo = (CheckedTreeViewInfo.TreeItemInfo)ViewInfo.GetItemInfoByPoint(e.Location);
				if(itemViewInfo != null && itemViewInfo.OpenCloseButtonInfo.HitTest(e.Location)) {
					ChangeExpanded(GetItem(e.Location));
					return;
				}
			}
			base.OnMouseDown(e);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			switch(e.KeyCode) {
				case Keys.Left:
					ProcessLeftArrowKey();
					e.Handled = true;
					break;
				case Keys.Right:
					ProcessRightArrowKey();
					e.Handled = true;
					break;
			}
			base.OnKeyDown(e);
		}
		protected virtual void ProcessLeftArrowKey() {
			if(!ProcessSelectedItem(false))
				MoveToItemRoot(SelectedItem);
		}
		protected virtual void ProcessRightArrowKey() {
			ProcessSelectedItem(true);
		}
		bool ProcessSelectedItem(bool expanded) {
			CheckedTreeViewItem item = SelectedItem;
			bool result = (item != null) && (item.IsExpanded != expanded && !item.IsLastLevel);
			if(result) {
				ChangeExpanded(item, expanded);
			}
			return result;
		}
		protected void MoveToItemRoot(CheckedTreeViewItem item) {
			if(item != null && item.Parent != null)
				base.SelectedItem = item.Parent;
		}
		#region Expand\Collapse
		protected virtual IActionContext GetQueueContext() {
			return ActionContext.Create(this);
		}
		void ChangeLevelExpanded(CheckedTreeViewItem levelItem, bool expand) {
			if(levelItem == null) return;
			CheckedTreeViewItem selectedItem = SelectedItem;
			ActionsQueue.SetQueueContext(GetQueueContext());
			ActionsQueue.QueueCompleted += (s, e) => {
				if(selectedItem != null && levelItem.IsAncestorOf(selectedItem))
					selectedItem = selectedItem.Parent;
				SetSelectedIndex(selectedItem);
			};
			int level = levelItem.Level;
			if(expand) {
				foreach(CheckedTreeViewItem item in Items) {
					if(item.Level == 0)
						EnqueueExpand(item, level);
				}
			} else {
				foreach(CheckedTreeViewItem item in Items) {
					if(item.Level == level)
						EnqueueCollapse(item);
				}
			}
			ActionsQueue.RunQueue();
		}
		void ChangeLevelExpandedCore(CheckedTreeViewItem node, int level) {
			ChangeExpanded(node, true, () => {
				if(node.Level < level && node.SubItemCount != 0) {
					foreach(CheckedTreeViewItem item in node.SubItems)
						EnqueueExpand(item, level);
				}
				ActionsQueue.CompleteAction();
			});
		}
		void EnqueueExpand(CheckedTreeViewItem node, int level) {
			ActionsQueue.EnqueueDelayed(() => ChangeLevelExpandedCore(node, level));
		}
		void EnqueueCollapse(CheckedTreeViewItem node) {
			ActionsQueue.EnqueueDelayed(() => ChangeExpanded(node, false, ActionsQueue.CompleteAction));
		}
		protected internal virtual void ChangeExpanded(CheckedTreeViewItem nodeItem) {
			ChangeExpanded(nodeItem, null);
		}
		protected internal virtual void ChangeExpanded(CheckedTreeViewItem nodeItem, ExpandCompletedCallback callback) {
			if(nodeItem == null) return;
			ChangeExpanded(nodeItem, !nodeItem.IsExpanded, callback);
		}
		protected virtual void ChangeExpanded(CheckedTreeViewItem nodeItem, bool expanded) {
			ChangeExpanded(nodeItem, expanded, null);
		}
		protected virtual void ChangeExpanded(CheckedTreeViewItem nodeItem, bool expanded, ExpandCompletedCallback callback) {
			if(nodeItem == null) return;
			Populate(nodeItem, expanded, callback);
		}
#if DEBUGTEST
		internal
#endif
		protected virtual void Populate(CheckedTreeViewItem node, bool expand, ExpandCompletedCallback callback) {
			if(node == null || node.IsExpanded == expand || node.Branch == null) {
				if(callback != null)
					callback();
				return;
			}
			if(expand && !node.IsLoaded)
				PopulateNode(node, callback);
			else
				ExpandOrCollapseNode(node, expand, callback);
		}
		void PopulateNode(CheckedTreeViewItem node, ExpandCompletedCallback callback) {
			int topIndex;
			CheckedTreeViewItem selectedItem;
			RaiseRetrieveChildElements(node.Branch, e => {
				if(e == null || e.IsEmpty) return;
				BeginPopulating(out topIndex, out selectedItem);
				ICollection subItems = node.LoadChildren(e.ChildElements, e.CheckStates, e.IsLastLevel);
				Items.InsertRange(node, subItems);
				node.IsExpanded = true;
				EndPopulating(topIndex, selectedItem, callback);
			});
		}
		void ExpandOrCollapseNode(CheckedTreeViewItem node, bool expand, ExpandCompletedCallback callback) {
			int topIndex;
			CheckedTreeViewItem selectedItem;
			BeginPopulating(out topIndex, out selectedItem);
			if(expand)
				AddSubItems(node);
			else {
				if(SelectedItem != null && node.IsAncestorOf(selectedItem))
					selectedItem = node;
				RemoveSubItems(node);
			}
			node.IsExpanded = expand;
			EndPopulating(topIndex, selectedItem, callback);
		}
		void BeginPopulating(out int topIndex, out CheckedTreeViewItem selectedItem) {
			topIndex = TopIndex;
			selectedItem = SelectedItem;
			BeginUpdate();
		}
		protected virtual void EndPopulating(int topIndex, CheckedTreeViewItem selectedItem, ExpandCompletedCallback callback) {
			TopIndex = topIndex;
			EndUpdate();
			SetSelectedIndex(selectedItem ?? SelectedItem);
			if(callback != null)
				callback();
		}
		void AddSubItems(CheckedTreeViewItem node) {
			Items.InsertRange(node, node.SubItems);
		}
		protected virtual void RemoveSubItems(CheckedTreeViewItem nodeItem) {
			Items.RemoveRange(nodeItem, nodeItem.SubItemCount);
		}
		CheckedTreeViewItem GetItem(Point location) { return (CheckedTreeViewItem)GetItem(IndexFromPoint(location)); }
		#endregion
		protected override void OnLoaded() {
			if (IsDesignMode)
				Items.Add(string.Empty, false);
			base.OnLoaded();
		}
	}
	public class CheckedTreeViewInfo : CheckedListBoxViewInfo {
		public CheckedTreeViewInfo(CheckedTreeViewControl treeView)
			: base(treeView) {
		}
		protected int OpenCloseButtonIndent { get { return (FullMarkWidth - OpenCloseButtonSize.Width) / 2; } }
		protected virtual int LevelOffset { get { return FullMarkWidth; } }
		public bool IsLoadingData { get { return OwnerControl.IsQueueRunning; } }
		public virtual int FullOpenCloseButtonWidth { get { return FullMarkWidth; } }
		public OpenCloseButtonPainter OpenCloseButtonPainter { get { return OwnerControl.LookAndFeel.Painter.OpenCloseButton; } }
		public Size OpenCloseButtonSize {
			get {
				OpenCloseButtonInfoArgs ocArgs = new OpenCloseButtonInfoArgs(null);
				return OpenCloseButtonPainter.CalcObjectMinBounds(ocArgs).Size;
			}
		}
		public override int CalcBestColumnWidth() {
			int result = base.CalcBestColumnWidth();
			result += FullOpenCloseButtonWidth;
			return result;
		}
		protected new CheckedTreeViewControl OwnerControl { get { return (CheckedTreeViewControl)base.OwnerControl; } }
		protected override BaseListBoxViewInfo.ItemInfo CreateItemInfo(Rectangle bounds, object item, string text, int index) {
			bounds = OffsetBounds(bounds, LevelOffset * OwnerControl.GetItemLevel(index));
			Rectangle checkBounds = OffsetBounds(bounds, FullOpenCloseButtonWidth);
			TreeItemInfo itemInfo = new TreeItemInfo(OwnerControl, OffsetBounds(checkBounds, FullMarkWidth), item, text, index, index == -1 ? CheckState.Unchecked : OwnerControl.GetItemCheckState(index),
				true, OwnerControl.GetItemIsExpanded(index), OwnerControl.GetItemCanExpand(index));
			itemInfo.Bounds = bounds;
			UpdateCheckMarkBounds(itemInfo.CheckArgs, checkBounds);
			UpdateOpenCloseButtonBounds(itemInfo.OpenCloseButtonArgs, bounds);
			return itemInfo;
		}
		protected void UpdateOpenCloseButtonBounds(OpenCloseButtonInfoArgs openCloseArgs, Rectangle bounds) {
			UpdateBoundsCore(openCloseArgs, OpenCloseButtonPainter, bounds, OpenCloseButtonSize, FullOpenCloseButtonWidth, OpenCloseButtonIndent);
			openCloseArgs.SetAppearance(OwnerControl.Appearance);
		}
		protected override int CalcItemMinHeight() {
			return Math.Max(base.CalcItemMinHeight(), OpenCloseButtonSize.Height + OpenCloseButtonIndent);
		}
		public class TreeItemInfo : CheckedItemInfo {
			OpenCloseButtonViewInfo ocButtonViewInfo;
			public TreeItemInfo(BaseListBoxControl ownerControl, Rectangle rect, object item, string text, int index, CheckState checkState, bool enabled,
							bool expanded, bool showOpenClose)
				: base(ownerControl, rect, item,  text, index, checkState, enabled) {
				this.ocButtonViewInfo = showOpenClose ? new OpenCloseButtonViewInfo(enabled, expanded) : new NullOpenCloseButtonViewInfo();
			}
			public OpenCloseButtonInfoArgs OpenCloseButtonArgs { get { return OpenCloseButtonInfo.InfoArgs; } }
			protected internal OpenCloseButtonViewInfo OpenCloseButtonInfo { get { return ocButtonViewInfo; } }
		}
	}
	public class OpenCloseButtonViewInfo : StyleObjectViewInfo {
		public OpenCloseButtonViewInfo(bool enabled, bool expanded)
			: base(enabled) {
			InfoArgs.Opened = expanded;
		}
		public new OpenCloseButtonInfoArgs InfoArgs { get { return (OpenCloseButtonInfoArgs)base.InfoArgs; } }
		protected override StyleObjectInfoArgs CreateStyleObjectInfoArgs() {
			return new OpenCloseButtonInfoArgs(null);
		}
	}
	public class NullOpenCloseButtonViewInfo : OpenCloseButtonViewInfo {
		public NullOpenCloseButtonViewInfo() : base(false, true) { }
		public override bool HitTest(Point point) { return false; }
		public override void Paint(ObjectPainter painter, GraphicsCache cache, AppearanceObject appearance) { }
	}
	public class CheckedTreeViewPainter : PainterCheckedListBox {
		protected override void DrawItemCore(ControlGraphicsInfoArgs info, BaseListBoxViewInfo.ItemInfo itemInfo, ListBoxDrawItemEventArgs e) {
			if(((CheckedTreeViewInfo)info.ViewInfo).IsLoadingData)
				return;
			base.DrawItemCore(info, itemInfo, e);
			CheckedTreeViewInfo.TreeItemInfo treeItemInfo = (CheckedTreeViewInfo.TreeItemInfo)itemInfo;
			treeItemInfo.OpenCloseButtonInfo.Paint(((CheckedTreeViewInfo)info.ViewInfo).OpenCloseButtonPainter, info.Cache, e.Appearance);
		}
	}
}
namespace DevExpress.XtraEditors.Controls {
	[ListBindable(false)]
	public class CheckedTreeViewItemCollection : CheckedListBoxItemCollection {
		bool isOneLevel;
		public event EventHandler TreeItemExpanded;
		internal bool IsOneLevel { get { return isOneLevel; } set { isOneLevel = value; }  }
		public CheckedTreeViewItemCollection() : base() { }
		public CheckedTreeViewItemCollection(bool isOneLevel) : this() {
			this.isOneLevel = isOneLevel;
		}
		public CheckedTreeViewItemCollection(int capacity) : base(capacity) { }
		public new CheckedTreeViewItem this[int index] {
			get { return (CheckedTreeViewItem)List[index]; }
			set { List[index] = value; }
		}
		public new CheckedTreeViewItem this[object value] { get { return (CheckedTreeViewItem)base[value]; } }
		public void AddRange(CheckedTreeViewItem[] items) { base.AddRange(items); }
		public virtual void AddRange(ICollection collection) {
			BeginUpdate();
			AttachRange(collection);
			InnerList.AddRange(collection);
			EndUpdate();
		}
		public void InsertRange(CheckedTreeViewItem item, ICollection collection) {
			InsertRange(IndexOf(item) + 1, collection);
		}
		public void RemoveRange(CheckedTreeViewItem item, int count) {
			RemoveRange(IndexOf(item) + 1, count);
		}
		protected virtual void InsertRange(int index, ICollection collection) {
			BeginUpdate();
			AttachRange(collection);
			InnerList.InsertRange(index, collection);
			EndUpdate();
		}
		protected virtual void RemoveRange(int index, int count) {
			BeginUpdate();
			DetachRange(index, count);
			InnerList.RemoveRange(index, count);
			EndUpdate();
		}
		public override int Add(object value) {
			if(value is CheckedTreeViewItem) return base.Add(value);
			if(value is CheckedListBoxItem) return Add(value);
			return Add(value, CheckState.Unchecked);
		}
		CheckedTreeViewItem Add(CheckedListBoxItem item) {
			return (CheckedTreeViewItem)CreateCheckedListBoxItem(item.Value, item.Description, item.CheckState, item.Enabled);
		}
		protected override void Attach(object item) {
			base.Attach(item);
			((CheckedTreeViewItem)item).ItemExpanded += new EventHandler(OnItemExpanded);
		}
		protected override void Detach(object item) {
			((CheckedTreeViewItem)item).ItemExpanded -= new EventHandler(OnItemExpanded);
			base.Detach(item);
		}
		protected virtual void AttachRange(ICollection collection) {
			foreach(object obj in collection) {
				Attach(obj);
			}
		}
		protected virtual void DetachRange(int index, int count) {
			for(int i = index; i < index + count; i++) {
				Detach(InnerList[i]);
			}
		}
		protected virtual void OnItemExpanded(object sender, EventArgs e) {
			OnExpanded(e);
		}
		protected virtual void OnExpanded(EventArgs e) {
			if(TreeItemExpanded != null) TreeItemExpanded(this, e);
		}
		protected override CheckedListBoxItem CreateCheckedListBoxItem(object value, string description, CheckState checkState, bool enabled) {
			return new CheckedTreeViewItem(value, description, checkState, enabled, isOneLevel);
		}
	}
	public class CheckedTreeViewItem : CheckedListBoxItem {
		List<CheckedTreeViewItem> children;
		CheckedTreeViewItem parent;
		bool expanded;
		bool isLastLevel;
		int lockUpdate;
		protected internal event EventHandler ItemExpanded;
		public CheckedTreeViewItem(object value, string description, CheckState checkState, bool enabled, bool isLastLevel)
			: base(value, description, checkState, enabled) {
			this.isLastLevel = isLastLevel;
		}
		public CheckedTreeViewItem(object value, CheckedTreeViewItem parent)
			: this(value, parent, false, parent.CheckState) {
		}
		public CheckedTreeViewItem(object value, CheckedTreeViewItem parent, bool? isChecked)
			: this(value, parent, false, GetCheckState(isChecked)) {
		}
		public CheckedTreeViewItem(object value, CheckedTreeViewItem parent, bool isLastLevel, bool? isChecked)
			: this(value, parent, isLastLevel, GetCheckState(isChecked)) {
		}
		public CheckedTreeViewItem(object value, CheckedTreeViewItem parent, bool isLastLevel, CheckState checkState)
			: base(value, checkState) {
			this.parent = parent;
			this.isLastLevel = isLastLevel;
		}
		public bool CanExpand { get { return !IsLastLevel && (!IsLoaded || HasChildren); } }
		public bool HasParent { get { return Parent != null; } }
		public bool IsAncestorOf(CheckedTreeViewItem item) {
			if(item.Parent == null) return false;
			return this == item.Parent ? true : IsAncestorOf(item.Parent);
		}
		public bool IsExpanded {
			get { return expanded; }
			set {
				if(expanded == value) return;
				expanded = value;
				if(HasChildren) {
					Expand();
				}
			}
		}
		public bool IsLoaded { get { return children != null; } }
		public bool IsLastLevel { get { return isLastLevel; } }
		public int Level { get { return HasParent ? Parent.Level + 1 : 0; } }
		protected void SetCheckStateCore(CheckState value) {
			base.CheckState = value;
		}
		public override CheckState CheckState {
			set {
				if(CheckState == value) return;
				SetCheckStateCore(value);
				UpdateCheckState();
			}
		}
		protected internal void UpdateCheckState() {
			if(IsLockUpdate) return;
			BeginUpdate();
			if(IsLoaded && this.CheckState != CheckState.Indeterminate) {
				foreach(CheckedTreeViewItem child in children) {
					child.CheckState = this.CheckState;
				}
			}
			if(HasParent)
				Parent.OnChildChangeCheckState(this.CheckState);
			EndUpdate();
		}
		protected bool IsLockUpdate { get { return lockUpdate != 0; } }
		public virtual void BeginUpdate() { lockUpdate++; }
		public virtual void EndUpdate() { CancelUpdate(); }
		public virtual void CancelUpdate() { lockUpdate--; }
		public CheckedTreeViewItem Parent { get { return parent; } }
		public object[] Branch {
			get {
				object[] branchValues = new object[Level + 1];
				AddBranchValue(branchValues);
				return branchValues;
			}
		}
		void AddBranchValue(object[] branchValues) {
			branchValues[Level] = Value;
			if(HasParent) Parent.AddBranchValue(branchValues);
		}
		protected internal List<CheckedTreeViewItem> Children { get { return children; } }
		public List<CheckedTreeViewItem> SubItems {
			get {
				List<CheckedTreeViewItem> subItems = new List<CheckedTreeViewItem>(ChildCount);
				foreach(CheckedTreeViewItem child in children) {
					subItems.Add(child);
					if(child.CanHasExpandedSubItems)
						subItems.AddRange(child.SubItems);
				}
				return subItems;
			}
		}
		public int SubItemCount {
			get {
				if(!HasChildren) return 0;
				int count = 0;
				foreach(CheckedTreeViewItem child in children) {
					count++;
					if(child.CanHasExpandedSubItems) count += child.SubItemCount;
				}
				return count;
			}
		}
		protected int ChildCount {
			get {
				return children.Count;
			}
		}
		public override void InvertCheckState() {
			if(CheckState != CheckState.Indeterminate) 
				SetCheckStateCore((CheckState == CheckState.Checked) ? CheckState.Unchecked : CheckState.Checked, false);
			if(!IsLoaded) return;
			foreach(CheckedTreeViewItem child in children) {
				child.InvertCheckState();
			}
		}
		bool HasChildren { get { return IsLoaded && (ChildCount != 0); } }
		bool IsHidden { get { return HasParent && (!Parent.IsExpanded || Parent.IsHidden); } }
		bool CanHasExpandedSubItems { get { return HasChildren && IsExpanded; } }
		public void LoadEmptyChildren() {
			LoadChildren(null, null, false);
		}
		protected virtual CheckedTreeViewItem CreateTreeViewItem(object value, bool isLastLevel, bool? checkState) {
			return new CheckedTreeViewItem(value, this, isLastLevel, checkState);
		}
		public List<CheckedTreeViewItem> LoadChildren(IList<object> values, IList<bool?> checks, bool isLastLevel) {
			bool isChecksValid = checks != null && checks.Count > 0;
			int count = values != null ? values.Count : 0;
			if((isChecksValid && count != checks.Count) || (this.IsLastLevel && count != 0))
				throw new ArgumentException("CheckedTreeViewItem.LoadChildren: parameters are incorrect.");
			CreateChildren(count);
			if(count == 0) return children;
			for(int i = 0; i < count; i++) {
				children.Add(CreateTreeViewItem(values[i], isLastLevel, isChecksValid && CheckState == CheckState.Indeterminate ? checks[i] : CheckState == CheckState.Checked));
			}
			return children;
		}
		void CreateChildren(int count) {
			if(!IsLoaded) {
				children = new List<CheckedTreeViewItem>(count);
			} else
				throw new ArgumentException("CheckedTreeViewItem.CreateChildren: children have been loaded.");
		}
		protected virtual void OnChildChangeCheckState(CheckState childCheckState) {
			if(IsLockUpdate) return;
			if(ChildCount > 1) {
				if(EnsureState(CheckState != CheckState.Indeterminate && CheckState != childCheckState)) return;
				foreach(CheckedTreeViewItem child in children) {
					if(EnsureState(child.CheckState != childCheckState)) return;
				}
			}
			CheckState = childCheckState;
		}
		bool EnsureState(bool condition) {
			if(condition)
				CheckState = CheckState.Indeterminate;
			return condition;
		}
		protected virtual void Expand() {
			if(ItemExpanded != null) ItemExpanded(this, EventArgs.Empty);
		}
	}
}
