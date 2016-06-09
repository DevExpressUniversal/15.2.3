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
using System.Drawing;
using DevExpress.Utils;
using System.Collections.Generic;
namespace DevExpress.XtraBars.Docking {
	public enum DockingStyle  { Float, Top, Bottom, Left, Right, Fill }
	public enum DockVisibility { Visible, Hidden, AutoHide }
}
namespace DevExpress.XtraBars.Docking.Helpers {
	public abstract class BaseLayoutInfo: CollectionBase {
		BaseLayoutInfo parent;
		int updateCounter;
		bool allowInvokeLayoutChanged = true;
		bool disableOptimizations = false;
		protected BaseLayoutInfo() {
			this.parent = null;
			this.updateCounter = 0;
		}
		protected abstract LayoutInfo CreateLayout(DockingStyle dock);
		public virtual LayoutInfo AddLayout(DockingStyle dock) {
			if(!CanAddLayout(dock)) return null;
			return AddLayoutCore(dock);
		}
		public virtual LayoutInfo InsertLayout(DockingStyle dock, int index) {
			if(!CanAddLayout(dock)) return null;
			LayoutInfo info = AddLayoutCore(dock);
			info.Index = index;
			return info;
		}
		protected internal virtual void InsertLayoutCore(int index, LayoutInfo info) {
			BeginUpdate();
			try {
				AddLayoutCore(info);
				info.Index = index;
			}
			finally {
				EndUpdate();
			}
		}
		protected virtual LayoutInfo AddLayoutCore(DockingStyle dock) {
			LayoutInfo info = CreateLayout(dock);
			AddLayoutCore(info);
			return info;
		}
		protected internal virtual void AddLayoutCore(LayoutInfo info) {
			List.Add(info);
		}
		protected internal virtual void AddLayoutInternal(LayoutInfo info) {
			InnerList.Add(info);
		}
		protected internal virtual bool CanAddLayout(DockingStyle dock) {
			if(dock == DockingStyle.Fill) {
				for(int i = 0; i < Count; i ++) {
					if(this[i].Dock == dock)
						return false;
				}
			}
			return true;
		}
		public virtual void RemoveLayout(LayoutInfo info) {
			if(List.Contains(info))
			List.Remove(info);
		}
		protected internal void InternalRemoveLayout(LayoutInfo info) {
			InnerList.Remove(info);
			info.parent = null;
		}
		protected internal virtual BaseLayoutInfo CheckDestInfoOnRemoveLayout(LayoutInfo childToRemove, BaseLayoutInfo destInfo) {
			return destInfo;
		}
		protected internal virtual void SetChildIndex(LayoutInfo info, int index) {
			info.Index = Math.Min(Count - 1, index);
		}
		protected internal int CheckDockChildren(LayoutInfo info, DockingStyle dock, int index) {
			if(!CanDockChildren(info, dock)) return index;
			return DockChildrenCore(info, dock, index);
		}
		protected virtual int DockChildrenCore(LayoutInfo info, DockingStyle dock, int index) {
			for(int i = info.Count - 1; i > 0; i--) {
				LayoutInfo child = info[i];
				child.Initializer = info.Initializer;
				try {
					child.InternalDockTo(this, dock, index);
				}
				finally {
					child.Initializer = null;
				}
			}
			return index;
		}
		protected internal virtual bool CanDockChildren(LayoutInfo info, DockingStyle dock) { return (info.HasChildren && info.Parent != this && info.Parent != null); }
		protected internal int GetTailNewItemIndex() {
			int index = (Count > 0 ? Count : 1);
			index = GetActualTailItemIndex(index);
			return index; 
		}
		protected virtual int GetActualTailItemIndex(int index) {
			return index;
		}
		protected virtual void AssignChildContent(LayoutInfo child) {
			child.parent = this;
		}
		protected internal void LayoutChanged() {
			if(IsLayoutLocked) return;
			LayoutChangedCore();
		}
		protected virtual void LayoutChangedCore() {}
		protected virtual bool IsLayoutLocked { get { return (updateCounter != 0); } }
		protected internal virtual void BeginUpdate() { updateCounter++ ;}
		protected virtual void CancelUpdate() { updateCounter-- ;}
		protected internal virtual void EndUpdate() {
			CancelUpdate();
			LayoutChanged();
		}
		protected internal abstract bool AcceptSettingsOnChildDestroy { get; }
		public abstract Rectangle Bounds { get; }
		public virtual Rectangle ClientBounds { get { return Bounds; } }
		public Size ClientSize { get { return ClientBounds.Size; } }
		public LayoutInfo this[int index] { get {return InnerList[index] as LayoutInfo; } }
		public LayoutInfo this[DockingStyle dock] { 
			get {
				for(int i = 0; i < Count; i ++) {
					if(this[i].Dock == dock)
						return this[i];
				}
				return null;
			}
		}
		protected internal BaseLayoutInfoManager Root {
			get {
				BaseLayoutInfo root = this;
				while(root.Parent != null)
					root = root.Parent;
				return root as BaseLayoutInfoManager;
			}
		}
		public int IndexOf(LayoutInfo info) { return InnerList.IndexOf(info); } 
		protected internal abstract bool CanChangeFillLayoutIndex { get; }
		protected internal void ChangeIndex(LayoutInfo info, int index) {
			if(index < 0 || index >= Count || index == info.Index) return;
			if(index == Count - 1 && this[DockingStyle.Fill] != null && !CanChangeFillLayoutIndex) return;
			ChangeIndexCore(info, index);
		}
		protected virtual void ChangeIndexCore(LayoutInfo info, int index) {
			InnerList.Remove(info);
			InnerList.Insert(index, info);
			OnChildIndexChanged(info);
			LayoutChanged();
		}
		protected virtual void OnChildIndexChanged(LayoutInfo info) {}
		protected internal Rectangle GetRestBounds(LayoutInfo info) {
			Rectangle restBounds = ClientBounds;
			int index = info == null ? Count : info.Index;
			for(int i = 0; i < index; i ++) {
				restBounds = this[i].GetDockBounds(restBounds);	
			}
			return restBounds;
		}
		protected internal LayoutInfo FindAdjacentLayout(LayoutInfo info, int beginIndex, int endIndex, int step, DockingStyle adjacentFillDock) {
			for(int i = beginIndex; i != endIndex; i += step) {
				if(info.AcceptDock(this[i], adjacentFillDock))
					return this[i];
			}
			return null;
		}
		protected override void OnInsertComplete(int index, object value) {
			SetParent(value, true);
			LayoutChanged();
		}
		public void SuspendInvokeLayoutChanged() { allowInvokeLayoutChanged = false; }
		public void ResumeInvokeLayoutChanged() { allowInvokeLayoutChanged = true; }
		protected override void OnRemoveComplete(int index, object value) {
			SetParent(value, false);
			if(Root != null)
				Root.OnItemRemoved((LayoutInfo)value);
			if(allowInvokeLayoutChanged || disableOptimizations) LayoutChanged();
		}
		protected override void OnClear() {
			for(int i = 0; i < Count; i ++)
				SetParent(this[i], false);
		}
		protected internal virtual void OnItemRemoved(LayoutInfo info) {
			foreach(BaseLayoutInfo child in this) {
				child.OnItemRemoved(info);
			}
		}
		protected void MoveChildren(BaseLayoutInfo info, IList children) {
			for(int i = 0; i < children.Count; i++) {
				LayoutInfo child = (LayoutInfo)children[i];
				info.AssignChildContent(child);
				info.InnerList.Add(child);
			}
			children.Clear();
		}
		protected virtual void SetParent(object info, bool assignThis) {
			((BaseLayoutInfo)info).Parent = (assignThis ? this : null);
		}
		protected internal virtual BaseLayoutInfo Parent { get { return parent; } set { parent = value; } }
	}
	public abstract class LayoutInfo : BaseLayoutInfo, IDisposable  {
		DockingStyle dock;
		Point floatLocation;
		Size originalSize;
		double sizeFactor;
		DockVisibility visibility;
		bool tabbed, disposing, smartAdjacent, floatVertical, canDockSourceChildren, canParentLayout;
		DefaultBoolean dockVertical;
		SavedInfo savedInfo;
		LayoutInfo activeChild;
		ILayoutInfoInitializer initializer;
		protected LayoutInfo() : this(DockingStyle.Float) { }
		protected LayoutInfo(DockingStyle dock) : this(dock, LayoutConsts.InvalidSize) { }
		protected LayoutInfo(DockingStyle dock, Size size) {
			this.dock = dock;
			this.visibility = LayoutConsts.DefaultVisibility;
			this.tabbed = false;
			this.savedInfo = CreateSavedInfo();
			this.canDockSourceChildren = false;
			this.canParentLayout = LayoutConsts.DefaultCanParentLayout;
			this.floatVertical = LayoutConsts.DefaultFloatVertical;
			this.dockVertical = DefaultBoolean.Default;
			this.smartAdjacent = true;
			if(size.IsEmpty) size = DefaultSize;
			this.originalSize = size;
			this.floatLocation = Point.Empty;
			this.sizeFactor = 1.0;
			this.activeChild = null;
			this.initializer = null;
		}
		public void Dispose() {
			if(!disposing) {
				disposing = true;
				OnDisposing();
				RaiseDisposed();
				Disposed = null;
			}
			GC.SuppressFinalize(this);
		}
		protected virtual void OnDisposing() {
			BaseLayoutInfoManager manager = InnerManager;
			if(manager != null && Visibility != DockVisibility.Visible) {
				bool autoHide = AutoHide;
				this.visibility = DockVisibility.Visible;
				if(autoHide)
					manager.OnLayoutInfoAutoHideChanged(this);
				else
					manager.OnLayoutInfoVisibleChanged(this);
			}
			if(Parent != null)
				Parent.RemoveLayout(this);
		}
		void RaiseDisposed() {
			if(Disposed != null)
				Disposed(this, EventArgs.Empty);
		}
		public event EventHandler Disposed;
		protected virtual DockMode DragVisualizationStyle {
			get { return DockMode.Standard; }
		}
		protected virtual SavedInfo CreateSavedInfo() { 
			return new SavedInfo(); 
		}
		protected override void OnRemoveComplete(int index, object value) {
			if(Disposing) return;
			if(CanDestroyOnRemoveComplete) {
				this[0].AssignContentTo(this);
				DoDestroyLayout(this[0]);
			}
			CheckActiveChildOnRemove(index, value);
			UpdateSavedSizeFactors();
			base.OnRemoveComplete(index, value);
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			UpdateSavedSizeFactors();
		}
		protected virtual bool CanDestroyOnRemoveComplete { get { return (Count == 1); } }
		void CheckActiveChildOnRemove(int index, object value) {
			if(Count == 0) ActiveChild = null;
			else {
				if(Tabbed) {
					if(index <= ActiveChildIndex || ActiveChildIndex < 0 || index < 0) ActiveChildIndex = Math.Min(Math.Max(ActiveChildIndex, Math.Min(index, Count - 1)), Count - 1);
				}
				else if(ActiveChild == value) ActiveChild = null;
			}
		}
		protected internal override bool AcceptSettingsOnChildDestroy { get { return false; } }
		void DoDestroyLayout(LayoutInfo info) {
			InnerManager.OnCreateDestroyLayout(this[0], this, null);
			DestroyLayout(this[0]);
		}
		protected virtual void DestroyLayout(LayoutInfo info) {
			RemoveLayout(info);
		}
		protected internal override void OnItemRemoved(LayoutInfo info) {
			base.OnItemRemoved(info);
			if(SavedInfo.SavedParent == info) {
				SavedInfo.SavedParent = null;
			}
		}
		public virtual Rectangle GetBoundingResizeRectangle() {
			if(AdjacentSplit != null) {
				DirectionRectangle result = new DirectionRectangle(Rectangle.Union(Bounds, AdjacentSplit.Bounds), IsHorizontal);
				result.CutFromTail(AdjacentSplit.MinSize);
				result.CutFromHead(MinSize);
				return result.Bounds;
			}
			LayoutRectangle boundingRectangle = new LayoutRectangle(Parent == null ? InnerManager.ClientBounds : Parent.GetRestBounds(this), Dock);
			return boundingRectangle.RemoveSize(MinSize);
		}
		public LayoutInfo GetPrevValidChild(int index) { return FindValidChild(index - 1, -1, -1); }
		public LayoutInfo GetNextValidChild(int index) { return FindValidChild(index + 1, Count, 1); }
		LayoutInfo FindValidChild(int beginIndex, int endIndex, int step) {
			for(int i = beginIndex; i != endIndex; i += step) {
				if(this[i].IsValid)
					return this[i];
			}
			return null;
		}
		public int ValidChildCount {
			get {
				int result = 0;
				for(int i = 0; i < Count; i++) {
					if(this[i].IsValid)
						result++;
				}
				return result;
			}
		}
		public bool Disposing { get { return disposing; } }
		public override Rectangle ClientBounds { get { return Bounds; } }
		public new BaseLayoutInfo Parent { get { return base.Parent; } }
		protected abstract Size DefaultSize { get; }
		public virtual DockingStyle Dock {
			get { return dock; }
			set { 
				if(IsLoading) dock = value;
				if(Dock == value || !IsVisible) return;
				DockTo(Parent, value, Index); 
			}
		}
		protected override int GetActualTailItemIndex(int index) {
			if(index <= Index && Parent is LayoutInfo)
				index = Index + 1;
			return index;
		}
		public DockVisibility Visibility {
			get { return visibility; }
			set {
				if(!CanSetVisibity(value)) return;
				DockVisibility oldVisibility = Visibility;
				OnChangeVisibility(oldVisibility, value);
			}
		}
		protected internal void SetVisibilityCore(DockVisibility value) {
			this.visibility = value;
		}
		protected virtual bool CanSetVisibity(DockVisibility value) {
			if(Visibility == value) return false;
			if(value == DockVisibility.AutoHide) {
				if(RootLayout.Dock == DockingStyle.Float) return IsMdiDocument;
				if(Dock == DockingStyle.Fill && LayoutParent == null) return false;
				if(Count > 1 && !Tabbed && IsDesignMode) return false;
				return !IsParentAutoHide;
			}
			return true;
		}
		protected virtual void OnChangeVisibility(DockVisibility oldVisibility, DockVisibility newVisibility) {
			if(oldVisibility == DockVisibility.Visible || newVisibility == DockVisibility.Visible) {
				UpdateVisibility(oldVisibility == DockVisibility.Visible ? newVisibility : oldVisibility);
			}
			else {
				SetVisibilityCore(newVisibility);
				if(newVisibility == DockVisibility.Hidden) {
					UpdateAutoHideInfo();
					InnerManager.OnLayoutInfoVisibleChanged(this);
				}
				else {
					InnerManager.OnLayoutInfoVisibleChanged(this);
					UpdateAutoHideInfo();
				}
			}
		}
		void UpdateVisibility(DockVisibility value) {
			if(Visibility != DockVisibility.Visible) Restore();
			else {
				SetVisibilityCore(value);
				if(value == DockVisibility.AutoHide)
					OnSetAutoHide();
				if(value == DockVisibility.Hidden)
					OnSetHidden();
			}
		}
		internal virtual void OnRestoreVisibility() {
			DockVisibility oldVisibility = Visibility;
			SetVisibilityCore(DockVisibility.Visible);
			if(oldVisibility == DockVisibility.AutoHide)
				UpdateAutoHideInfo();
			else
				InnerManager.OnLayoutInfoVisibleChanged(this);
		}
		protected virtual void OnSetAutoHide() {
			SavePrevSettings();
			UpdateAutoHideInfo();
			DockingStyle parentDock = Dock;
			if(LayoutParent != null)
				parentDock = LayoutParent.Dock;
			if(Parent != null)
				Parent.RemoveLayout(this);
			if(AutoHide && Dock == DockingStyle.Fill && LayoutParent == null)
				this.dock = parentDock;
		}
		protected virtual void OnSetHidden() {
			SavePrevSettings();
			if(Parent != null) {
				InnerManager.OnLayoutInfoVisibleChanged(this);
				Parent.RemoveLayout(this);
			}
		}
		public bool AutoHide {
			get { return (Visibility == DockVisibility.AutoHide); }
			set { Visibility = (value ? DockVisibility.AutoHide : DockVisibility.Visible); }
		}
		protected bool CheckForCycle(LayoutInfo info) {
			if(info.SavedInfo == null) return false;
			if(info.SavedInfo.SavedParent == null) return false;
			if(info.SavedInfo.SavedParent == info) return true;
			LayoutInfo savedParentInfo = info.SavedInfo.SavedParent as LayoutInfo;
			if(savedParentInfo == null) return false;
			if(savedParentInfo.SavedInfo.SavedParent == null) return false;
			if(savedParentInfo.SavedInfo.SavedParent == info) return true;
			return false;
		}
		public DockingStyle AutoHideDock {
			get {
				if(Saved && !CheckForCycle(this)) {
					SavedInfo.UpdateAutoHideDock(Dock);
					return SavedInfo.AutoHideDock;
				}
				if(LayoutParent != null)
					return LayoutParent.AutoHideDock;
				return Dock; 
			}
		}
		public bool IsTab { 
			get { return (LayoutParent != null && LayoutParent.Tabbed);} 
		}
		public virtual bool CanDockPanelInCaptionRegion() {
			return false;
		}
		public virtual bool IsMdiDocument { 
			get { return false; } 
		}
		public virtual bool DockAsMdiDocument() {
			return false;
		} 
		public LayoutInfo RootLayout {
			get {
				LayoutInfo rootLayout = this;
				while(rootLayout.LayoutParent != null)
					rootLayout = rootLayout.LayoutParent;
				return rootLayout;
			}
		}
		protected LayoutInfo ParentAutoHide {
			get {
				LayoutInfo infoParent = LayoutParent;
				while(infoParent != null) {
					if(infoParent.AutoHide) return infoParent;
					infoParent = infoParent.LayoutParent;
				}
				return null;
			}
		}
		protected internal bool IsParentAutoHide { get { return (ParentAutoHide != null); } }
		public bool Visible { get { return (Visibility == DockVisibility.Visible); } }
		public bool IsVisible {
			get {
				LayoutInfo infoLevel = this;
				while(infoLevel != null) {
					if(!infoLevel.Visible) return false;
					infoLevel = infoLevel.LayoutParent;
				}
				return true;
			}
		}
		protected internal virtual bool IsLoading { get { return (Manager == null); } }
		public Point Location {
			get { return Dock == DockingStyle.Float ? FloatLocation : Bounds.Location; }
			set { FloatLocation = value; }
		}
		public Point FloatLocation { get { return floatLocation; } set { floatLocation = value; } }
		public virtual int Index { 
			get { return Parent == null ? LayoutConsts.InvalidIndex : Parent.IndexOf(this); }
			set {
				if(Index == value || Parent == null) return;
				if(Dock == DockingStyle.Fill && !Parent.CanChangeFillLayoutIndex) return;
				Parent.ChangeIndex(this, value);
			}
		}
		public Size Size {
			get {
				return Bounds.Size;
			}
			set {
				value = new Size(Math.Max(value.Width, MinSize.Width), Math.Max(value.Height, MinSize.Height));
				if(value == Size) return;
				ResizeLayout(value);
			}
		}
		public LayoutInfo ActiveChild {
			get { return activeChild; }
			set {
				if(ActiveChild == value) return;
				if(Count > 0 && Tabbed && value == null) return;
				if(value != null && value.LayoutParent != this && !IsLoading) return;
				SetActiveChildCore(value);
				OnActiveChildChanged();
			}
		}
		protected void SetActiveChildCore(LayoutInfo info) {
			this.activeChild = info;
		}
		public int ActiveChildIndex {
			get { return (ActiveChild == null ? LayoutConsts.InvalidIndex : IndexOf(ActiveChild)) ; }
			set {
				value = Math.Max(LayoutConsts.InvalidIndex, Math.Min(value, Count - 1));
				ActiveChild = (value == LayoutConsts.InvalidIndex ? null : this[value]);
			}
		}
		protected virtual void OnActiveChildChanged() {
			LayoutChanged();
		}
		public bool HasChildren { get { return Count > 0; } }
		public bool HasAsParent(LayoutInfo info) {
			if(info == null) return false;
			LayoutInfo infoParent = Parent as LayoutInfo;
			while(infoParent != null) {
				if(infoParent == info) return true;
				infoParent = infoParent.Parent as LayoutInfo;
			}
			return false;
		}
		public bool HasAsChild(LayoutInfo info) {
			if(info == null) return false;
			return info.HasAsParent(this);
		}
		BaseLayoutInfo GetSavedParentRoot() {
			if(SavedInfo.Saved)
				return SavedInfo.SavedParent.Root;
			return null;
		}
		int dockingAsTab = 0;
		protected override bool IsLayoutLocked {
			get { return base.IsLayoutLocked || IsDockingAsTab; }
		}
		protected internal bool IsDockingAsTab { get { return dockingAsTab > 0; } }
		protected internal void BeginDockingAsTab() {
			dockingAsTab++;
		}
		protected internal void EndDockingAsTab() {
			if(--dockingAsTab == 0)
				LayoutChanged();
		}
		int CorrectedIndex(int destIndex, DockingStyle dockStyle) {
			if(dockStyle == DockingStyle.Right) return destIndex + 1;
			if(dockStyle == DockingStyle.Top) return destIndex - 1;
			return destIndex;
		}
		internal void DockTo(BaseLayoutInfo destInfo, DockingStyle dockStyle) {
			BaseLayoutInfo dest = destInfo;
			if(AutoHide || IsParentAutoHide) return;
			if(!CanDockTo(destInfo, dockStyle))
				dest = destInfo.Parent;
			int index = CorrectedIndex(destInfo.GetTailNewItemIndex(), dockStyle);
			DockTo(dest, dockStyle, index);
		}
		public void DockTo(BaseLayoutInfo destInfo, DockingStyle dock, int index) {
			if(destInfo == null) {
				destInfo = Manager;
				if(destInfo == null)
					destInfo = GetSavedParentRoot();
			}
			if(dock == DockingStyle.Float && (destInfo is LayoutInfo)) {
				destInfo = Manager;
				if(destInfo == null)
					destInfo = GetSavedParentRoot();
				index = destInfo.Count - 1;
			}
			if(!CanDockTo(destInfo, dock)) return;
			if(index == LayoutConsts.InvalidIndex) index = destInfo.GetTailNewItemIndex();
			if(dock == DockingStyle.Float) {
				SavePrevSettings();
				SetFloatVerticalCore(IsVertical);
			}
			else if(Dock == DockingStyle.Float) SavedInfo.Clear();
			DockToCore(destInfo, dock, index);
		}
		public virtual void DockAsTab(LayoutInfo info, int index) {
			if(CanDockToLayoutParent(info)) {
				info = info.LayoutParent;
			}
			if(index == LayoutConsts.InvalidIndex) index = (info.HasChildren ? info.Count : 1);
			info.CanDockSourceChildren = true;
			try {
				DockTo(info, DockingStyle.Fill, index);
				if(LayoutParent == info) {
					try {
						info.Tabbed = true;
						info.BreakdownIntoAtoms(index);
					}
					finally {
						info.ActiveChild = this;
						LayoutChanged();
					}
				}
			}
			finally {
				info.CanDockSourceChildren = false;
				if(Manager != null)
					Manager.LayoutChanged();
			}
		}
		void BreakdownIntoAtoms(int layoutIndex) {
			List<LayoutInfo> children = new List<LayoutInfo>();
			GetChildrenCore(InnerList.ToArray(), children);
			foreach(var child in children)
				if(child.HasChildren)
					child.DockTo(this, DockingStyle.Fill, layoutIndex);
		}
		void GetChildrenCore(object[] currentNodes, List<LayoutInfo> objects) {
			List<object> children = new List<object>();
			foreach(LayoutInfo node in currentNodes) {
				if(node.HasChildren) {
					objects.Add(node);
					children.AddRange(node.InnerList.ToArray());
				}
			}
			if(children.Count == 0) return;
			GetChildrenCore(children.ToArray(), objects);
		}
		bool CanDockToLayoutParent(LayoutInfo info) {
			bool result = false;
			if(!info.HasChildren && info.LayoutParent != null)
				result = LayoutParent == info.LayoutParent && LayoutParent.Count == 2 || info.IsTab;
			return result;
		}
		protected internal IList<LayoutInfo> GetChildren() {
			List<LayoutInfo> children = new List<LayoutInfo>();
			CopyChildren(children);
			return children;
		}
		void CopyChildren(IList<LayoutInfo> atoms) {
			for(int i = 0; i < Count; i++) {
				LayoutInfo info = this[i];
				if(info.HasChildren) 
					info.CopyChildren(atoms);
				else
					atoms.Add(info);
			}
		}
		public virtual void MakeFloat() {
			Dock = DockingStyle.Float;
		}
		public void MakeFloat(Point location) {
			Location = location;
			MakeFloat();
		}
		protected internal void ReplaceSavedParent(LayoutInfo oldParent, BaseLayoutInfo newParent, bool replaceSaved) {
			SavedInfo.ReplaceSavedParent(oldParent, newParent, replaceSaved);
		}
		protected internal void OnCreateDestroyLayout(LayoutInfo info, BaseLayoutInfo owner, LayoutInfo newInfo) {
			SavedInfo.OnCreateDestroyLayout(info, owner, newInfo);
		}
		protected internal SavedInfo SavedInfo { get { return savedInfo; } }
		protected internal bool Saved { get { return SavedInfo.Saved || Dock == DockingStyle.Float; } }
		void SavePrevSettings() {
			if(Saved && Manager == null) return;
			SavedInfo.SaveSettings(this);
		}
		public virtual void Restore() {
			CheckSavedParent();
			SavedInfo.Restore(this);
		}
		LayoutInfo GetTopSavedParent() {
			LayoutInfo result = this;
			int i = 0;
			while(true) {
				LayoutInfo info = result.SavedInfo.SavedParent as LayoutInfo;
				if(info != null && info.SavedInfo.Saved && info.SavedInfo.SavedParent != null && i < 100 && info.Visibility != DockVisibility.Visible) {
					if(info.SavedInfo.SavedParent == this && info.SavedInfo.SavedDock == DockingStyle.Fill && this.SavedInfo.SavedDock == DockingStyle.Fill) {
						info.SavedInfo.SavedParent = null;
						result = this;
						break;
					}
					result = info;
					i++;
				}
				else
					break;
			}
			if(i >= 100) {
				result = this;
				UpdateRecursive();
			}
			return result;
		}
		protected virtual void UpdateRecursive() { 
		}
		internal void CheckSavedParent() {
			LayoutInfo info = GetTopSavedParent();
			if(info == this) return;
			SavedInfo tmpInfo = new SavedInfo(this, SavedInfo.SavedParent != info);
			SavedInfo.Assign(info.SavedInfo);
			info.SavedInfo.Assign(tmpInfo);
			InnerManager.ReplaceSavedParent(info, this);
		}
		protected bool CanDockTo(BaseLayoutInfo destInfo, DockingStyle dock) {
			if(destInfo == null || destInfo == this) return false;
			if(!destInfo.CanAddLayout(dock)) return false;
			if(HasAsChild(destInfo as LayoutInfo)) return false;
			if(Dock == DockingStyle.Fill && !Saved && Parent != null && !Parent.CanChangeFillLayoutIndex) return false;
			if(Dock == DockingStyle.Float && AutoHide || IsParentAutoHide) return false;
			return true;
		}
		protected virtual void DockToCore(BaseLayoutInfo destInfo, DockingStyle dock, int index) {
			index = destInfo.CheckDockChildren(this, dock, index);
			InternalDockTo(destInfo, dock, index);
		}
		internal bool CanDockSourceChildren { get { return canDockSourceChildren; } set { canDockSourceChildren = value; } }
		protected internal override bool CanDockChildren(LayoutInfo info, DockingStyle dock) {
			if(CanDockSourceChildren) return true;
			return (base.CanDockChildren(info, dock) && (Tabbed == info.Tabbed || Tabbed) && dock == DockingStyle.Fill);
		}
		protected internal void InternalDockTo(BaseLayoutInfo destInfo, DockingStyle dock, int index) {
			if(CanChangeChildIndex(destInfo, dock) ) {
				InnerManager.ReplaceSavedParent(this, Parent, true);
				this.dock = dock;
				Parent.SetChildIndex(this, index);
			}
			else {
				if(Parent != null) {
					InnerManager.ReplaceSavedParent(this, Parent, true);
					destInfo = Parent.CheckDestInfoOnRemoveLayout(this, destInfo);
					index = UpdateIndex(destInfo, dock, index);
					BaseLayoutInfo sParent = Parent;
					sParent.SuspendInvokeLayoutChanged();
					sParent.RemoveLayout(this);
					sParent.ResumeInvokeLayoutChanged();
				}
				this.dock = dock;
				destInfo.InsertLayoutCore(index, this);
			}
		}
		int UpdateIndex(BaseLayoutInfo destInfo, DockingStyle dock, int index) {
			if(Index < index) {
				if((Dock == DockingStyle.Float) && (dock != DockingStyle.Fill && dock != DockingStyle.Float) && destInfo is DockLayoutManager) {
					index = index > 0 ? index - 1 : index;
				}
			}
			return index;
		}
		protected internal override BaseLayoutInfo CheckDestInfoOnRemoveLayout(LayoutInfo childToRemove, BaseLayoutInfo destInfo) {
			if(Count == 2 && destInfo.Parent == this) {
				SetFloatVerticalCore(((LayoutInfo)destInfo).FloatVertical);
				return this;
			}
			return base.CheckDestInfoOnRemoveLayout(childToRemove, destInfo);
		}
		bool CanChangeChildIndex(BaseLayoutInfo destInfo, DockingStyle dock) {
			if(Dock == DockingStyle.Float || dock == DockingStyle.Float || Saved) return false;
			if(destInfo == Parent) {
				if(LayoutParent != null && LayoutParent.Tabbed) return (dock == DockingStyle.Fill);
				return true;
			}
			return false;
		}
		public LayoutInfo LayoutParent { get { return Parent as LayoutInfo; } }
		protected internal override bool CanChangeFillLayoutIndex { get { return true; } }
		protected Size cachedSizeCore;
		protected Size SizeCore {
			get {
				if(IsSizeFactorUsed) {
					return LayoutParent.GetChildFactorSize(this);
				}
				else {
					if(Parent == null) return OriginalSize;
					if(cachedSizeCore.IsEmpty) {
						cachedSizeCore = OriginalSize;
						if(cachedSizeCore.Height < MinSize.Height)
							cachedSizeCore.Height = MinSize.Height;
					}
					return cachedSizeCore;
				}
			}
			set {
				if(IsSizeFactorUsed)
					LayoutParent.SetChildFactorSize(this, value);
				else {
					if(Dock == DockingStyle.Float || DockVertical != DefaultBoolean.Default || IsLoading) this.OriginalSize = value;
					else if(IsHorizontal) this.originalSize.Width = value.Width;
					else this.originalSize.Height = value.Height;
					if(originalSize.Height < MinSize.Height)
						originalSize.Height = MinSize.Height;
					cachedSizeCore = Size.Empty;
				}
			}
		}
		public Size OriginalSize { get { return this.originalSize; } set { this.originalSize = value; } }
		protected internal double SizeFactor {
			get { return sizeFactor; }
			set { sizeFactor = value; } 
		}
		bool IsSizeFactorUsed {
			get {
				if(AutoHide) return false;
				return LayoutParent != null && LayoutParent.CanUpdateChildrenSizeFactor;
			}
		}
		Rectangle FactorBounds {
			get { return new Rectangle(LayoutParent.GetChildFactorLocation(this), SizeCore); }
		}
		Size GetChildFactorSize(LayoutInfo info) {
			DirectionSize dSize = new DirectionSize(ClientBounds.Size, info.IsHorizontal);
			if(info.Index == Count - 1) {
				for(int i = 0; i < Count - 1; i++) {
					dSize.Width -= GetChildFactorSizeCore(this[i]);
				}
			}
			else {
				dSize.Width = GetChildFactorSizeCore(info);
			}
			return dSize.Size;
		}
		int GetChildFactorSizeCore(LayoutInfo info) {
			DirectionSize dSize = new DirectionSize(ClientBounds.Size, info.IsHorizontal);
			DirectionSize minSize = new DirectionSize(MinSize, info.IsHorizontal);
			return Math.Max(minSize.Width, Round(dSize.Width * info.SizeFactor / ((savedAllSizeFactor == 0 ? GetAllFactorSize() : savedAllSizeFactor) * 1.0)));
		}
		void SetChildFactorSize(LayoutInfo info, Size size) {
			SetChildFactorSize(info, size, ClientBounds.Size);
		}
		double GetAllFactorSize() {
			double result = 0;
			for(int i = 0; i < Count; i++) {
				if(!this[i].Visible) continue;
				result += this[i].SizeFactor;
			}
			return result > 0 ? Math.Round(result, 5) : 1;
		}
		double savedAllSizeFactor;
		int Round(double value) {
			return (int)Math.Round(value, MidpointRounding.AwayFromZero);
		}
		int deserializeCounter;
		protected void LockDeserialize() { deserializeCounter++; }
		protected void UnlockDeserialize() { deserializeCounter--; }
		bool IsDeserializing { get { return deserializeCounter != 0; } }
		internal void SetChildFactorSize(LayoutInfo info, Size size, Size clientSize) {
			DirectionSize dSize = new DirectionSize(clientSize, info.IsHorizontal);
			DirectionSize newDSize = new DirectionSize(size, info.IsHorizontal);
			if(IsDeserializing) {
				info.SizeFactor = Math.Round(savedAllSizeFactor * newDSize.Width * 1.0 / dSize.Width, 5);
				return;
			}
			if(GetAllFactorSize() != savedAllSizeFactor)
				info.SizeFactor += Math.Round(savedAllSizeFactor - GetAllFactorSize(), 5);
			else
				info.SizeFactor = Math.Round(info.SizeFactor * newDSize.Width * 1.0 / (GetLayoutInfoLength(info) * 1.0), 5);
		}
		protected virtual int GetLayoutInfoLength(LayoutInfo info) {
			int length = info.IsHorizontal ? info.Bounds.Width : info.Bounds.Height;
			return length;
		}
		protected Rectangle GetChildBounds(LayoutInfo info) {
			if(Tabbed)
				return ClientBounds;
			DirectionRectangle dBounds = new DirectionRectangle(ClientBounds, info.IsHorizontal);
			if(ActiveChildIndex == -1 && info != null && info.Index >= 0) {
				return GetSimpleChildBounds(dBounds, info.Index);
			}
			if(info.Index == ActiveChildIndex) {
				CutBounds(dBounds, 0, info.Index, 1, true);
				CutBounds(dBounds, Count - 1, info.Index, -1, false);
			}
			else {
				if(info.Index < ActiveChildIndex)
					CutBounds(dBounds, 0, info.Index, 1, true);
				else {
					CutBounds(dBounds, Count - 1, info.Index - 1, -1, false);
					dBounds.SetLocation(new Point(dBounds.Bounds.Right, dBounds.Bounds.Bottom));
				}
				dBounds.SetSize(info.MinSize);
			}
			return dBounds.Bounds;
		}
		void CutBounds(DirectionRectangle dBounds, int beginIndex, int endIndex, int step, bool fromHead) {
			for(int i = beginIndex; i != endIndex; i += step) {
				if(fromHead)
					dBounds.CutFromHead(this[i].MinSize);
				else
					dBounds.CutFromTail(this[i].MinSize);
			}
		}
		Rectangle GetSimpleChildBounds(DirectionRectangle dBounds, int index) {
			for(int i = 0; i < index; i++) {
				dBounds.CutFromHead(this[i].OriginalSize);
			}
			dBounds.SetSize(this[index].OriginalSize);
			return dBounds.Bounds;
		}
		protected internal virtual int DecreaseSize(int size) {
			if(!IsSide) return size;
			BeginDecreaseSize();
			try {
				DirectionSize dSize = new DirectionSize(Size, IsHorizontal),
					dMinSize = new DirectionSize(MinSize, IsHorizontal);
				int dif = Math.Min(dSize.Width - dMinSize.Width, size);
				dSize.Width -= dif;
				size -= dif;
				Size = dSize.Size;
			}
			finally {
				EndDecreaseSize();
			}
			return size;
		}
		protected bool IsSide { get { return (IsHorizontal || IsVertical); } }
		protected virtual bool CanUpdateChildrenSizeFactor { get { return HasChildren && ActiveChild == null; } }
		protected void UpdateSavedSizeFactors() {
			savedAllSizeFactor = GetAllFactorSize();
		}
		void UpdateSizeFactorsOnNullActiveChild(Size[] childSizes) {
			if(!CanUpdateChildrenSizeFactor) return;
			UpdateSizeFactors(childSizes);
		}
		protected void UpdateSizeFactors(Size[] childSizes) {
			double allSizeFactors = GetAllFactorSize();
			double sum = 0;
			for(int i = 0; i < Count; i++) {
				var info = this[i];
				var size = childSizes[i];
				DirectionSize dSize = new DirectionSize(ClientBounds.Size, info.IsHorizontal);
				DirectionSize newDSize = new DirectionSize(size, info.IsHorizontal);
				if(i == Count - 1) {
					info.SizeFactor = Math.Round(allSizeFactors - sum, 5);
					return;
				}
				info.SizeFactor = Math.Round((newDSize.Width * allSizeFactors) / dSize.Width, 5);
				sum += info.SizeFactor;
			}
		}
		Point GetChildFactorLocation(LayoutInfo info) {
			int x = 0;
			for(int i = 0; i < info.Index; i ++)
				x += GetChildFactorSizeCore(this[i]);
			Point location = ClientBounds.Location;
			if(info.IsHorizontal)
				location.X += x;
			else location.Y += x;
			return location;
		}
		public bool IsHorizontal {
			get {
				if(IsValidParentLayoutDirection) {
					if(LayoutParent.DockVertical != DefaultBoolean.Default)
						return LayoutParent.IsVerticalCore;
					return LayoutParent.IsVertical;
				}
				return !IsVerticalCore;
			}
		}
		public bool IsVertical {
			get {
				if(IsValidParentLayoutDirection) {
					if(LayoutParent.DockVertical != DefaultBoolean.Default)
						return !LayoutParent.IsVerticalCore;
					return LayoutParent.IsHorizontal;
				}
				return IsVerticalCore;
			} 
		}
		internal bool IsVerticalCore { 
			get {
				if(Dock == DockingStyle.Float) return FloatVertical;
				if(DockVertical != DefaultBoolean.Default)
					return DockVertical == DefaultBoolean.True;
				return LayoutRectangle.GetIsVertical(Dock);
			}
		}
		public virtual bool FloatVertical { get { return floatVertical; } set { SetFloatVerticalCore(value); } }
		public virtual DefaultBoolean DockVertical { get { return dockVertical; } set { dockVertical = value; } }  
		protected internal void SetFloatVerticalCore(bool value) {
			this.floatVertical = value;
		}
		bool IsValidParentLayoutDirection { get { return (LayoutParent != null && LayoutParent.HasChildren); } }
		protected internal void ResizeLayout(Size value) {
			if(Dock != DockingStyle.Float) {
				AdjacentInfo[] adjacentLayouts = GetAdjacentLayouts(ref value);
				if(value == Size) return;
				if(adjacentLayouts != null && Manager != null && !Manager.FormResizing && !SizeDecreases) {
					if(IsValidAdjacentSiblingLayouts(adjacentLayouts))
						LayoutParent.UnsetActiveChild();
					for(int i = 0; i < adjacentLayouts.Length; i++) {
						adjacentLayouts[i].SetSize();
					}
				}
			}
			SetSize(value);
			UpdateLayoutOnResize();
		}
		int decreaseSizeCount = 0;
		protected bool SizeDecreases { get { return decreaseSizeCount > 0; } }
		protected void BeginDecreaseSize() {
			decreaseSizeCount++;
		}
		protected void EndDecreaseSize() {
			decreaseSizeCount--;
		}
		protected virtual void UpdateLayoutOnResize() {
			Parent.LayoutChanged();
		}
		bool IsValidAdjacentSiblingLayouts(AdjacentInfo[] adjacentLayouts) {
			if(LayoutParent == null || LayoutParent.Tabbed) return false;
			return IsValidAdjacentSiblingLayout(adjacentLayouts[0]) || IsValidAdjacentSiblingLayout(adjacentLayouts[1]);
		}
		bool IsValidAdjacentSiblingLayout(AdjacentInfo adjacentInfo) {
			return (adjacentInfo.AdjacentLayout != null) && (adjacentInfo.AdjacentLayout.LayoutParent == LayoutParent);
		}
		void UnsetActiveChild() {
			if(ActiveChild == null) return;
			Size[] childSizes = GetChildSizes();
			SetActiveChildCore(null);
			UpdateSizeFactorsOnNullActiveChild(childSizes);
			OnActiveChildChanged();
		}
		protected Size[] GetChildSizes() {
			Size[] result = new Size[Count];
			for(int i = 0; i < Count; i++)
				result[i] = this[i].Size;
			return result;
		}
		#region Adjacent
		public LayoutInfo LeftAdjacentLayout {
			get {
				bool frontDirection = (LayoutParent != null ? true : Dock != DockingStyle.Right);
				return GetAdjacentLayoutFrom(DockingStyle.Left, frontDirection);
			}
		}
		public LayoutInfo RightAdjacentLayout {
			get {
				bool frontDirection = (LayoutParent != null ? false : Dock != DockingStyle.Left);
				return GetAdjacentLayoutFrom(DockingStyle.Right, frontDirection);
			}
		}
		public LayoutInfo TopAdjacentLayout {
			get {
				bool frontDirection = (LayoutParent != null ? true : Dock != DockingStyle.Bottom);
				return GetAdjacentLayoutFrom(DockingStyle.Top, frontDirection);
			}
		}
		public LayoutInfo BottomAdjacentLayout {
			get {
				bool frontDirection = (LayoutParent != null ? false : Dock != DockingStyle.Top);
				return GetAdjacentLayoutFrom(DockingStyle.Bottom, frontDirection);
			}
		}
		protected LayoutInfo AdjacentSplit {
			get {
				Size sizeArg = Size;
				AdjacentInfo[] adjacentInfos = GetAdjacentLayouts(ref sizeArg);
				return (adjacentInfos == null ? null : adjacentInfos[0].AdjacentLayout);
			}
		}
		AdjacentInfo[] GetAdjacentLayouts(ref Size sizeArg) {
			LayoutInfo[] infos = new LayoutInfo[2] { null, null }; 
			DockingStyle infoDock = Dock;
			if(LayoutParent != null) {
				if(LayoutParent.Dock == DockingStyle.Float) infoDock = DockingStyle.Fill;
				else if(!IsTab) {
					if(Index == LayoutParent.Count - 1) infoDock = LayoutRectangle.GetTail(IsHorizontal);
					else infoDock = LayoutRectangle.GetHead(IsHorizontal);
				}
			}
			switch(infoDock) {
				case DockingStyle.Left:
					infos[0] = RightAdjacentLayout;
					break;
				case DockingStyle.Top:
					infos[0] = BottomAdjacentLayout;
					break;
				case DockingStyle.Right:
					infos[0] = LeftAdjacentLayout;
					break;
				case DockingStyle.Bottom:
					infos[0] = TopAdjacentLayout;
					break;
				case DockingStyle.Fill:
					infos[0] = (RightAdjacentLayout != null ? RightAdjacentLayout : LeftAdjacentLayout);
					infos[1] = (BottomAdjacentLayout != null ? BottomAdjacentLayout : TopAdjacentLayout);
					if(infos[0] == null) {
						infos[0] = infos[1];
						infos[1] = null;
					}
					break;
			}
			int length = (infos[0] == null ? 0 : 1) + (infos[1] == null ? 0 : 1);
			if(length == 0) return null;
			AdjacentInfo[] adjacentInfos = new AdjacentInfo[length];
			for(int i = 0; i < length; i++)
				adjacentInfos[i] = new AdjacentInfo(infos[i], infos[i].GetAdjacentSize(this, ref sizeArg));
			return adjacentInfos;
		}
		protected LayoutInfo GetAdjacentLayoutFrom(DockingStyle adjacentFillDock, bool frontDirection) {
			if(IsHorizontal && !AcceptHorizontal(Dock)) return null;
			if(IsVertical && !AcceptVertical(Dock)) return null;
			if(Parent == null) return null;
			if(LayoutParent != null && !LayoutParent.Tabbed) {
				if(LayoutParent.IsHorizontal == LayoutRectangle.GetIsHorizontal(adjacentFillDock)) return null;
				if(frontDirection && Index > 0) return Parent[Index - 1];
				if(!frontDirection && Index < Parent.Count - 1) return Parent[Index + 1];
				return null;
			}
			LayoutInfo result = null;
			if(frontDirection) {
				result = Parent.FindAdjacentLayout(this, Index - 1, -1, -1, adjacentFillDock);
			} 
			else {
				result = Parent.FindAdjacentLayout(this, Index + 1, Parent.Count, 1, adjacentFillDock);
			}
			return CheckSmartAdjacent(result);
		}
		protected internal bool AcceptDock(LayoutInfo adjacent, DockingStyle adjacentFillDock) {
			if(!adjacent.IsValid) return false;
			if(Dock == DockingStyle.Fill && (LayoutParent == null || LayoutParent.Dock == DockingStyle.Float)) return (adjacent.Dock == adjacentFillDock);
			return (adjacent.Dock == Dock || adjacent.Dock == DockingStyle.Fill);
		}
		bool AcceptHorizontal(DockingStyle dock) {
			return dock == DockingStyle.Fill || LayoutRectangle.GetIsHorizontal(dock);
		}
		bool AcceptVertical(DockingStyle dock) {
			return dock == DockingStyle.Fill || LayoutRectangle.GetIsVertical(dock);
		}
		LayoutInfo CheckSmartAdjacent(LayoutInfo info) {
			if(!SmartAdjacent || info == null) return info;
			LayoutInfo result = info;
			int minIndex = Math.Min(Index, info.Index),
				maxIndex = Math.Max(Index, info.Index);
			for(int i = minIndex + 1; i < maxIndex; i++) {
				LayoutInfo intermediate = Parent[i];
				if(intermediate.Dock != Dock && intermediate.Dock != LayoutRectangle.GetOppositeDockingStyle(Dock)) {
					result = null;
					break;
				}
			}
			return result;
		}
		protected internal Size GetAdjacentSize(LayoutInfo info, ref Size sizeArg) {
			Size adjacentSize = Size;
			if(info.IsHorizontal || info.Dock == DockingStyle.Fill) {
				adjacentSize.Width = Math.Max(MinSize.Width, adjacentSize.Width + (info.Size.Width - sizeArg.Width));
				sizeArg.Width = (Size.Width + info.Size.Width) - adjacentSize.Width;
			}
			if(info.IsVertical || info.Dock == DockingStyle.Fill){
				adjacentSize.Height = Math.Max(MinSize.Height, adjacentSize.Height + (info.Size.Height - sizeArg.Height));
				sizeArg.Height = (Size.Height + info.Size.Height) - adjacentSize.Height;
			}
			return adjacentSize;
		}
		protected bool SmartAdjacent { get { return smartAdjacent; } set { smartAdjacent = value; } }
		#endregion Adjacent
		protected internal virtual bool IsValid { get { return (Visibility == DockVisibility.Visible); } }
		protected internal void SetSize(Size size) {
			SizeCore = size;
		}
		public abstract Size MinSize { get; }
		public override Rectangle Bounds {
			get { 
				if(Dock == DockingStyle.Float) {
					return new Rectangle(Location, SizeCore); 
				}
				if(!IsSizeFactorUsed) {
					if(AutoHide) return GetAutoHideBounds();
					if(LayoutParent != null) return LayoutParent.GetChildBounds(this);
					return Parent == null ? Rectangle.Empty : GetBounds(Parent.GetRestBounds(this), Dock); 
				}
				else return FactorBounds;
			}
		}
		public bool Tabbed { 
			get { return tabbed; }
			set {
				if(Tabbed == value || !CanSetTabbed(value)) return;
				SetTabbedCore(value);
				OnTabbedChanged();
			}
		}
		public int Level {
			get {
				int level = 0;
				BaseLayoutInfo infoParent = Parent;
				while(infoParent != null) {
					level ++;
					infoParent = infoParent.Parent;
				}
				return level - 1;
			}
		}
		public ILayoutInfoInitializer Initializer { get { return initializer; } set { initializer = value; } }
		void SetTabbed(bool value) {
			if(CanSetTabbed(value))
				SetTabbedCore(value);
		}
		protected internal virtual void SetTabbedCore(bool value) {
			this.tabbed = value;
		}
		bool CanSetTabbed(bool value) {
			if(IsLoading) return true;
			return (value ? HasChildren : true);
		}
		protected virtual void OnTabbedChanged() {
			if(Tabbed && ActiveChild == null && (!IsLoading || Saved))
				ActiveChildIndex = Math.Min(0, Count - 1);
			LayoutChanged();
		}
		public BaseLayoutInfoManager Manager { get { return Root; } }
		protected internal virtual BaseLayoutInfoManager InnerManager {
			get {
				BaseLayoutInfoManager result = Manager;
				if(result == null)
					result = SavedInfo.GetManager(this);
				return result;
			}
		}
		protected virtual void UpdateAutoHideInfo() {
			InnerManager.OnLayoutInfoAutoHideChanged(this);
		}
		protected internal override bool CanAddLayout(DockingStyle dock) {
			if(dock == DockingStyle.Float)
				return false;
			for(int i = 0; i < Count; i ++) {
				if(IsNotSameLayoutDockStyle(dock, this[i].Dock))
					return false;
			}
			return true;
		}
		bool IsNotSameLayoutDockStyle(DockingStyle style1, DockingStyle style2) {
			if(style1 == DockingStyle.Fill || style2 == DockingStyle.Fill) 
				return false;
			bool isHorizontal1 = LayoutRectangle.GetIsHorizontal(style1);
			bool isHorizontal2 = LayoutRectangle.GetIsHorizontal(style2);
			return isHorizontal1 != isHorizontal2;
		}
		protected internal override void AddLayoutCore(LayoutInfo info) {
			LayoutInfo newInfo = null;
			Point oldFloatLocation = FloatLocation;
			if(CanCreateParentLayout(info)) {
				newInfo = CreateParentLayout(info.Dock);
				if(InnerManager != null)
					InnerManager.OnCreateDestroyLayout(info, this, newInfo);
				if(info.Initializer != null)
					info.Initializer.InitializeBeforeAssignContent(this);
				AssignContentTo(newInfo);
				if(info.Initializer != null)
					info.Initializer.InitializeAfterAssignContent(this);
				base.AddLayoutCore(newInfo);
			}
			base.AddLayoutCore(info);
			if(Count == 2) {
				OnMadeContainer(info);
			}
			FloatLocation = oldFloatLocation;
		}
		protected virtual void OnMadeContainer(LayoutInfo addedInfo) {
			addedInfo.Index = 0;
			if(Tabbed)
				ActiveChild = addedInfo;
		}
		internal bool CanParentLayout { get { return canParentLayout; } set { canParentLayout = value; } }
		bool CanCreateParentLayout(LayoutInfo info) {
			if(Tabbed) {
				if(LayoutRectangle.GetIsHorizontal(info.Dock) || LayoutRectangle.GetIsVertical(info.Dock)) return true;
				if(info.Dock == DockingStyle.Fill)  
					return info.Saved && !info.SavedInfo.SavedTabbed && info.SavedInfo.SavedParent == this;		 
				return false;
			}
			if(info.CanParentLayout) return true;
			if(!HasChildren) return true;
			return false;
		}
		protected virtual LayoutInfo CreateParentLayout(DockingStyle dock) {
			return CreateLayout(dock);
		}
		void AssignContentTo(LayoutInfo assignTo) {
			if(Disposing) return;
			AssignContentToCore(assignTo);
		}
		protected virtual void AssignContentToCore(LayoutInfo assignTo) {
			MoveChildren(assignTo);
			assignTo.SetTabbedCore(Tabbed);
			assignTo.SetActiveChildCore(ActiveChild);
			assignTo.FloatLocation = FloatLocation;
			assignTo.SetFloatVerticalCore(FloatVertical);
			assignTo.DockVertical = DockVertical;
			SetTabbedCore(false);
			SetActiveChildCore(null);
			FloatLocation = Point.Empty;
		}
		protected void MoveChildren(BaseLayoutInfo info) {
			MoveChildren(info, InnerList);
		}
		protected Rectangle GetBounds(Rectangle resBounds, DockingStyle dock) {
			Size size = SizeCore;
			switch(dock) {
				case DockingStyle.Left:
					resBounds.Width = size.Width;
					break;
				case DockingStyle.Right:
					resBounds.X += resBounds.Width - size.Width;
					resBounds.Width = size.Width;
					break;
				case DockingStyle.Top:
					resBounds.Height = size.Height;
					break;
				case DockingStyle.Bottom:
					resBounds.Y += resBounds.Height - size.Height;
					resBounds.Height = size.Height;
					break;
			}
			return resBounds;
		}
		protected virtual Rectangle GetAutoHideBounds() {
			if(InnerManager == null) return Rectangle.Empty;
			return GetBounds(InnerManager.ClientBounds, AutoHideDock);
		}
		protected internal Rectangle GetDockBounds(Rectangle resBounds) {
			if(!IsValid || Dock == DockingStyle.Float || this.IsMdiDocument) return resBounds;
			return (new LayoutRectangle(resBounds, Dock)).RemoveSize(SizeCore);
		}
		protected virtual bool IsDesignMode { get { return false; } }
	}
	public abstract class BaseLayoutInfoManager : BaseLayoutInfo {
		AutoHideInfoCollection autoHideInfoCollection;
		LayoutInfoCollection invisibleCollection;
		public BaseLayoutInfoManager() {
			this.autoHideInfoCollection = new AutoHideInfoCollection();
			this.invisibleCollection = new LayoutInfoCollection();
		}
		public abstract int AutoHidePanelSize { get; }
		public abstract Dictionary<DockingStyle, int> AutoHidePanelsSize { get; }
		public virtual LayoutInfo AddLayout(DockingStyle dock, Point floatLocation) {
			if(!CanAddLayout(dock)) return null;
			LayoutInfo info = CreateLayout(dock);
			info.Location = floatLocation;
			AddLayoutCore(info);
			return info;
		}
		protected internal override void AddLayoutCore(LayoutInfo info) {
			base.AddLayoutCore(info);
			if(info.Dock != DockingStyle.Fill && Count > 1 && this[Count - 2].Dock == DockingStyle.Fill)
				info.Index = Count - 2;
		}
		protected internal override bool CanDockChildren(LayoutInfo info, DockingStyle dock) {
			if(dock == DockingStyle.Float) return false;
			if(info.HasChildren) {
				if(info.Tabbed) return false;
				else if(info.Dock == DockingStyle.Float) return (info.IsHorizontal != LayoutRectangle.GetIsHorizontal(dock));
			}
			return base.CanDockChildren(info, dock);
		}
		protected override int DockChildrenCore(LayoutInfo info, DockingStyle dock, int index) {
			if(LayoutRectangle.GetIsHead(dock)) {
				index = base.DockChildrenCore(info, dock, index);
			}
			else {
				for(int i = info.Count - 1; i > 0; i--) {
					info[i].InternalDockTo(this, dock, index++);
				}
			}
			return index;
		}
		protected internal virtual void OnLayoutInfoAutoHideChanged(LayoutInfo info) {
			AutoHideInfoCollection.OnLayoutInfoAutoHideChanged(info);
			LayoutChanged();
		}
		protected internal override bool AcceptSettingsOnChildDestroy { get { return true; } }
		protected override void OnRemoveComplete(int index, object value) {
			LayoutInfo info = (LayoutInfo)value;
			if(info.Disposing) {
				OnCreateDestroyLayout(info, this, null);
			}
			base.OnRemoveComplete(index, value);
		}
		protected internal void ReplaceSavedParent(LayoutInfo oldParent, BaseLayoutInfo newParent) {
			ReplaceSavedParent(oldParent, newParent, false);
		}
		protected internal void ReplaceSavedParent(LayoutInfo oldParent, BaseLayoutInfo newParent, bool replaceSaved) {
			AutoHideInfoCollection.ReplaceSavedParent(oldParent, newParent, replaceSaved);
			InvisibleCollection.ReplaceSavedParent(oldParent, newParent, replaceSaved);
		}
		protected internal void OnCreateDestroyLayout(LayoutInfo info, BaseLayoutInfo owner, LayoutInfo newInfo) {
			AutoHideInfoCollection.OnCreateDestroyLayout(info, owner, newInfo);
			InvisibleCollection.OnCreateDestroyLayout(info, owner, newInfo);
		}
		protected internal virtual void OnLayoutInfoVisibleChanged(LayoutInfo info) {
			InvisibleCollection.OnLayoutInfoVisibleChanged(info);
			LayoutChanged();
		}
		internal bool FormResizing = false;
		protected void DecreaseSize(int size, params DockingStyle[] docks) {
			for(int i = Count - 1; i > -1 && size > 0; i--) {
				LayoutInfo info = this[i];
				if(!info.IsValid) continue;
				if(Array.IndexOf(docks, info.Dock) == LayoutConsts.InvalidIndex) continue;
				size = info.DecreaseSize(size);
			}
		}
		public LayoutInfo this[DockingStyle dock, int index] { 
			get {
				int count = 0;
				for(int i = 0; i < Count; i ++) {
					if(this[i].Dock == dock) {
						if(index == count++)
							return this[i];
					}
				}
				return null;
			}
		}
		public int GetCount(DockingStyle dock) {
			int count = 0;
			for(int i = 0; i < Count; i ++) {
				if(this[i].Dock == dock)
					count++;
			}
			return count;
		}
		public int InvisibleCount { get { return InvisibleCollection.Count; } }
		public LayoutInfo GetInvisibleLayout(int index) { return InvisibleCollection[index] ; }
		protected internal override bool CanChangeFillLayoutIndex { get { return false; } }
		protected internal AutoHideInfoCollection AutoHideInfoCollection { get { return autoHideInfoCollection; } }
		protected LayoutInfoCollection InvisibleCollection { get { return invisibleCollection; } }
		protected virtual AutoHideInfoCollection GetAutoHideCollectionOnGetClientBounds() { return AutoHideInfoCollection; }
		public override Rectangle ClientBounds { 
			get { return GetAutoHideCollectionOnGetClientBounds().GetClientBounds(Bounds, AutoHidePanelSize, AutoHidePanelsSize); } 
		}
	}
}
