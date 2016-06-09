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
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using DevExpress.XtraTab;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraLayout.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraLayout.Helpers;
using DevExpress.XtraTab.ViewInfo;
using DevExpress.XtraLayout.Tab;
namespace DevExpress.XtraLayout {
	public delegate void LayoutTabPageChangedEventHandler(object sender, LayoutTabPageChangedEventArgs e);
	public delegate void LayoutTabPageChangingEventHandler(object sender, LayoutTabPageChangingEventArgs e);
	public class LayoutTabPageChangedEventArgs : EventArgs {
		LayoutGroup page;
		LayoutGroup prevPage;
		public LayoutTabPageChangedEventArgs(LayoutGroup prevPage, LayoutGroup page) {
			this.page = page;
			this.prevPage = prevPage;
		}
		public LayoutGroup Page {
			get { return page; }
			set { page = value; }
		}
		public LayoutGroup PrevPage {
			get { return prevPage; }
			set { prevPage = value; }
		}
	}
	public class LayoutTabPageChangingEventArgs : LayoutTabPageChangedEventArgs {
		bool cancel = false;
		public LayoutTabPageChangingEventArgs(LayoutGroup prevPage, LayoutGroup page, bool cancel)
			: base(prevPage, page) {
			this.cancel = cancel;
		}
		public bool Cancel {
			get { return cancel; }
			set { cancel = value; }
		}
	}
	public class TabbedGroup : LayoutItemContainer {
		TabbedGroupsCollection tabs, visibleTabPages;
		LayoutGroup activeTab;
		DefaultBoolean showTabHeader = DefaultBoolean.Default;
		TabPageImagePosition imagePosition;
		internal TabbedGroup(LayoutGroup parent)
			: base(parent) {
			if(parent != null)
				Owner = parent.Owner;
			this.TextLocation = Locations.Top;
			this.imagePosition = TabPageImagePosition.Near;
		}
		protected override void Dispose(bool disposing) {
			if(!disposing) { base.Dispose(disposing); return; }
			if(disposingFlagCore) return;
			disposingFlagCore = true;
			if(Owner != null && Owner.RootGroup != null) {
				LayoutGroupHandlerWithTabHelper handler = Owner.RootGroup.Handler as LayoutGroupHandlerWithTabHelper;
				if(handler != null) {
					handler.ResetReferences(this);
				}
			}
			using(new SafeBaseLayoutItemChanger(this)) {
				ArrayList itemsToDispose = new ArrayList(TabPages);
				for(int i = 0; i < itemsToDispose.Count; i++) {
					BaseLayoutItem item = itemsToDispose[i] as BaseLayoutItem;
					if(item != null) {
						item.Dispose();
					}
				}
			}
			if(tabs != null) {
				tabs.Clear();
				tabs = null;
			}
			if(visibleTabPages != null) {
				visibleTabPages.Clear();
				visibleTabPages = null;
			}
			if(viewInfoCore != null) viewInfoCore.Destroy();
			viewInfoCore = null;
			base.Dispose(disposing);
		}
		protected internal override void SetViewInfoAndPainter(ViewInfo.BaseLayoutItemViewInfo vi, Painting.BaseLayoutItemPainter painter) {
			if(vi == null) StoreTabPagePropertiesBeforeReset();
			base.SetViewInfoAndPainter(vi, painter);
		}
		int storedFirstVisibleTabIndex = -1;
		void StoreTabPagePropertiesBeforeReset() {
			if(ViewInfo != null && ViewInfo.IsViewInfoCalculated && ViewInfo.BorderInfo != null) {
				storedFirstVisibleTabIndex = ViewInfo.BorderInfo.Tab.Handler.ViewInfo.FirstVisiblePageIndex;
			}
		}
		void RestoreTabPagePropertiesAfterReset(ViewInfo.TabbedGroupViewInfo vi) {
			if(storedFirstVisibleTabIndex != -1) {
				vi.BorderInfo.Tab.Handler.ViewInfo.FirstVisiblePageIndex = storedFirstVisibleTabIndex;
			}
		}
		[XtraSerializableProperty(), DefaultValue(""), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string TypeName {
			get { return "TabbedGroup"; }
		}
		public override void Accept(BaseVisitor visitor) {
			if(visitor.StartVisit(this)) {
				base.Accept(visitor);
				foreach(BaseLayoutItem bItem in TabPages) {
					bItem.Accept(visitor);
				}
			}
			visitor.EndVisit(this);
		}
		DefaultBoolean multiLineTabsCore = DefaultBoolean.Default;
		[Localizable(true), DefaultValue(DefaultBoolean.Default)]
		[XtraSerializableProperty]
		public virtual DefaultBoolean MultiLine {
			get { return multiLineTabsCore; }
			set {
				if(MultiLine != value) {
					multiLineTabsCore = value;
					shouldResetBorderInfoCore = true;
					ShouldResize = true;
					ComplexUpdate();
				}
			}
		}
		XtraLayout.Utils.Padding captionImagePadding = new DevExpress.XtraLayout.Utils.Padding(0);
		bool ShouldSerializeCaptionImagePadding() { return !IsDefaultCaptionImagePadding(); }
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("TabbedGroupCaptionImagePadding")]
#endif
		public XtraLayout.Utils.Padding CaptionImagePadding {
			get { return captionImagePadding; }
			set {
				if(CaptionImagePadding == value) return;
				captionImagePadding = value;
				shouldResetBorderInfoCore = true;
				ShouldResize = true;
				ComplexUpdate();
			}
		}
		protected internal virtual bool IsDefaultCaptionImagePadding() {
			return CaptionImagePadding == new XtraLayout.Utils.Padding(0);
		}
 DefaultBoolean headerAutoFillCore = DefaultBoolean.Default;
		[Localizable(true), DefaultValue(DefaultBoolean.Default)]
		[XtraSerializableProperty]
		public virtual DefaultBoolean HeaderAutoFill {
			get { return headerAutoFillCore; }
			set {
				if(HeaderAutoFill != value) {
					headerAutoFillCore = value;
					shouldResetBorderInfoCore = true;
					ShouldResize = true;
					ComplexUpdate();
				}
			}
		}
		[Localizable(true), DefaultValue(DefaultBoolean.Default)]
		[XtraSerializableProperty]
		public virtual DefaultBoolean ShowTabHeader {
			get {
				if(Owner != null && Owner.EnableCustomizationMode)
					return DefaultBoolean.Default;
				return this.showTabHeader;
			}
			set {
				if(this.ShowTabHeader != value) {
					this.showTabHeader = value;
					shouldResetBorderInfoCore = true;
					ShouldResize = true;
					ComplexUpdate();
				}
			}
		}
		[XtraSerializableProperty, DefaultValue(TabPageImagePosition.Near)]
		public virtual TabPageImagePosition PageImagePosition {
			get { return imagePosition; }
			set {
				if(PageImagePosition == value) return;
				imagePosition = value;
				shouldResetBorderInfoCore = true;
				ComplexUpdate();
			}
		}
		public void InsertTabPage(LayoutGroup insertTo, LayoutGroup insertGroup, InsertLocation insertLocation) {
			if(insertTo == null || !TabPages.Contains(insertTo) || insertGroup == null) return;
			int insertIndex = TabPages.IndexOf(insertTo);
			if(insertLocation == InsertLocation.After) insertIndex++;
			InsertTabPage(insertIndex, insertGroup);
		}
		protected internal bool SelectFirstPage() {
			if(TabPages.Count == 0) return false;
			int nextIndex;
			if(TrySelectNextPage(0, out nextIndex)) {
				SelectedTabPageIndex = nextIndex;
				return true;
			}
			return false;
		}
		protected internal bool SelectLastPage() {
			if(TabPages.Count == 0) return false;
			int nextIndex;
			if(TrySelectPrevPage(TabPages.Count - 1, out nextIndex)) {
				SelectedTabPageIndex = nextIndex;
				return true;
			}
			return false;
		}
		protected internal bool SelectNextPage(bool moveBack) {
			if(TabPages.Count == 0) return false;
			if(SelectedTabPage == null) return false;
			int nextIndex;
			if(!moveBack && SelectedTabPageIndex == TabPages.Count - 1) {
				if(TrySelectNextPage(0, out nextIndex)) {
					SelectedTabPageIndex = nextIndex;
					return true;
				}
			}
			if(!moveBack) {
				if(TrySelectNextPage(SelectedTabPageIndex + 1, out nextIndex)) {
					SelectedTabPageIndex = nextIndex;
					return true;
				}
			}
			if(moveBack && SelectedTabPageIndex == 0) {
				if(TrySelectPrevPage(TabPages.Count - 1, out nextIndex)) {
					SelectedTabPageIndex = nextIndex;
					return true;
				}
			}
			if(moveBack) {
				if(TrySelectPrevPage(SelectedTabPageIndex - 1, out nextIndex)) {
					SelectedTabPageIndex = nextIndex;
					return true;
				}
			}
			return false;
		}
		protected bool TrySelectNextPage(int index, out int nextIndex) {
			for(nextIndex = index; nextIndex < TabPages.Count; nextIndex++) {
				if(CanSelectPage(TabPages[nextIndex])) 
					return true;
			}
			return false;
		}
		protected bool TrySelectPrevPage(int index, out int nextIndex) {
			for(nextIndex = index; nextIndex >= 0; nextIndex--) {
				if(CanSelectPage(TabPages[nextIndex]))
					return true;
			}
			return false;
		}
		protected bool CanSelectPage(LayoutGroup page) {
			return (page != null) && (page.Owner != null && !page.Owner.EnableCustomizationMode ? page.PageEnabled : true);
		}
		public void InsertTabPage(int insertIndex, LayoutGroup group) {
			if(group != null && !TabPages.Contains(group)) {
				BeginChangeUpdate();
				if(group.Parent != null) group.Parent.Remove(group);
				group.Owner = Owner;
				Size oldMinSize = MinSize;
				TabPages.Insert(insertIndex, group);
				OnTabs_Changed(null, new CollectionChangeEventArgs(CollectionChangeAction.Add, group));
				SelectedTabPage = group;
				EndChangeUpdate();
			}
		}
		protected internal override void AssignInternal(BaseLayoutItem item) {
			TabbedGroup tab = item as TabbedGroup;
			if(tab != null) {
				base.AssignInternal(tab);
				this.activeTab = null;
				if(tabs != null && tabs.Count == tab.tabs.Count) {
					for(int i = 0; i < tabs.Count; i++) {
						LayoutGroup t = tab.tabs[i];
						tabs[i].AssignInternal(t);
						if(tab.activeTab == t) this.activeTab = this.tabs[i];
					}
				}
				this.headerOrientation = tab.headerOrientation;
				this.isInSelectedTabPage = tab.isInSelectedTabPage;
				this.multiLineTabsCore = tab.multiLineTabsCore;
				this.selectedTabPageName = tab.selectedTabPageName;
				this.showTabHeader = tab.showTabHeader;
				this.storedFirstVisibleTabIndex = tab.storedFirstVisibleTabIndex;
			}
		}
		protected override void CloneCommonProperties(LayoutGroup parent, ILayoutControl owner, ref BaseLayoutItem clone) {
			TabbedGroupsCollection tabsCopy = this.TabPages;
			TabbedGroupsCollection visibleTabsCopy = this.VisibleTabPages;
			LayoutGroup activeTabCopy = this.activeTab;
			base.CloneCommonProperties(parent, owner, ref clone);
			TabbedGroup cloneItem = (TabbedGroup)clone;
			cloneItem.constraintsHelper = new TabbedConstraintsHelper();
			cloneItem.constraintsHelper.minHItem = this.constraintsHelper.minHItem;
			cloneItem.constraintsHelper.minWItem = this.constraintsHelper.minWItem;
			cloneItem.activeTab = null;
			cloneItem.visibleTabPages = null;
			cloneItem.tabs = new TabbedGroupsCollection();
			cloneItem.tabs.Changed += new CollectionChangeEventHandler(cloneItem.OnTabs_Changed);
			foreach(LayoutGroup tabGroup in tabsCopy) {
				LayoutGroup newTabGroup = (LayoutGroup)tabGroup.Clone(parent, owner);
				cloneItem.tabs.Add(newTabGroup);
				if(activeTabCopy == tabGroup) {
					cloneItem.activeTab = newTabGroup;
				}
			}
		}
		protected int CorrectInsertIndex(LayoutGroup dragGroup, LayoutGroup moveToGroup, TabbedGroup tabbedGroup, InsertLocation insertLocation) {
			if(moveToGroup == null || dragGroup == null || tabbedGroup == null) return -1;
			if(this != tabbedGroup) return -1;
			if(TabPages.IndexOf(moveToGroup) < 0) return -1;
			if(!CanDragDroup(TabPages.IndexOf(moveToGroup), insertLocation, dragGroup)) return -1;
			int dragGroupIndex = TabPages.IndexOf(dragGroup);
			int insertIndex = TabPages.IndexOf(moveToGroup);
			if(dragGroupIndex >= 0) {
				if(insertIndex == dragGroupIndex) return -1;
				if(insertIndex > dragGroupIndex) insertIndex--;
			}
			if(insertLocation == InsertLocation.After) insertIndex++;
			return insertIndex;
		}
		public bool MoveTabPage(LayoutItemDragController controller) {
			if(Parent != null && Parent.ResizeManager != null) Parent.ResizeManager.VisibilityState = true;
			bool result = MoveTabPageCore(controller);
			if(Parent != null && Parent.ResizeManager != null) Parent.ResizeManager.VisibilityState = false;
			return result;
		}
		private bool MoveTabPageCore(LayoutItemDragController controller) {
			LayoutGroup tab = controller.DragItem as LayoutGroup;
			if(tab != null && controller.Item != null) {
				if(tab == controller.Item)
					return false;
				int newIndex = -1;
				TabbedGroupHitInfo hitInfo = controller.HitInfo as TabbedGroupHitInfo;
				LayoutGroup moveToGroup = controller.Item as LayoutGroup;
				TabbedGroup moveToTabbedGroup = controller.Item as TabbedGroup;
				if(moveToGroup != null)
					newIndex = CorrectInsertIndex(tab, moveToGroup, this, controller.InsertLocation);
				if(moveToTabbedGroup != null && hitInfo != null)
					if(hitInfo.TabbedGroupHotPageIndex >= 0) {
						newIndex = moveToTabbedGroup.CorrectInsertIndex(tab, moveToTabbedGroup.TabPages[hitInfo.TabbedGroupHotPageIndex], moveToTabbedGroup, hitInfo.TabbedGroupInsertLocation);
					}
					else {
						newIndex = moveToTabbedGroup.TabPages.Count;
					}
				if(newIndex < 0 && hitInfo != null && moveToTabbedGroup != null && moveToTabbedGroup.TabPages.Count > 0) return false;
				if(!RemoveTabPage(tab))
					return false;
				else {
					TabbedGroup tGroup = controller.Item as TabbedGroup;
					if(hitInfo == null && tGroup != null && moveToTabbedGroup.TabPages.Count == 0 && controller.MoveType == MoveType.Inside) {
						moveToTabbedGroup.AddTabPage(tab);
						return true;
					}
					if(hitInfo != null) {
						if(tGroup != this && hitInfo.TabbedGroupHotPageIndex >= 0) {
							tGroup.InsertTabPage(tGroup.TabPages[hitInfo.TabbedGroupHotPageIndex], tab, hitInfo.TabbedGroupInsertLocation);
						}
						else {
							if(newIndex >= 0) {
								if(newIndex >= TabPages.Count)
									AddTabPage(tab);
								else
									InsertTabPage(newIndex, tab);
							}
							else
								return false;
						}
						return true;
					}
					else {
						LayoutGroup target = controller.Item.Parent;
						if(target == null) 
							target = controller.Item as LayoutGroup;
						if(target != null)
							return target.InsertItem(controller);
						else return false;
					}
				}
			}
			else
				return false;
		}
		protected internal override void RestoreChildren(BaseItemCollection items) {
			TabPages.Clear();
			string selectedTabPageNameCore = SelectedTabPageName;
			foreach(BaseLayoutItem item in items) {
				LayoutGroup tempGroup = item as LayoutGroup;
				if(tempGroup != null && tempGroup.TabbedGroupParentName == Name) {
					string oldName = item.Name;
					TabPages.Add(tempGroup);
					if(tempGroup.Name != oldName) {
						SafeRenameItem(tempGroup, oldName, tempGroup.Name);
					}
					SelectedTabPage = tempGroup;
					tempGroup.ParentName = "";
				}
			}
			foreach(LayoutGroup group in TabPages) {
				if(group.Name == selectedTabPageNameCore) {
					SelectedTabPage = group;
					return;
				}
			}
		}
		[Browsable(false)]
		public TabbedGroupsCollection TabPages {
			get {
				if(tabs == null) {
					tabs = new TabbedGroupsCollection();
					tabs.Changed += new CollectionChangeEventHandler(OnTabs_Changed);
				}
				return tabs;
			}
		}
		[Browsable(false)]
		protected internal TabbedGroupsCollection VisibleTabPages {
			get {
				if(visibleTabPages == null) visibleTabPages = CreateFillVisibleTabPages();
				return visibleTabPages;
			}
		}
		protected internal void ResetVisibleTabPages() {
			visibleTabPages = null;
		}
		public override void EndInit() {
			base.EndInit();
			ResetVisibleTabPages();
		}
		protected TabbedGroupsCollection CreateFillVisibleTabPages() {
			TabbedGroupsCollection visibleTabPagesCore = new TabbedGroupsCollection();
			foreach(LayoutGroup group in TabPages)
				if(group.RequiredItemVisibility) visibleTabPagesCore.Add(group);
			foreach(LayoutGroup group in TabPages)
				if(!group.RequiredItemVisibility)
					if(SelectedTabPage == group)
						if(visibleTabPagesCore.Count > 0) activeTab = visibleTabPagesCore[0];
			if(SelectedTabPage == null && visibleTabPagesCore.Count > 0) activeTab = visibleTabPagesCore[0];
			return visibleTabPagesCore;
		}
		[RefreshProperties(RefreshProperties.All), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(false)]
		public override bool TextVisible { get { return false; } }
		TabOrientation headerOrientation = TabOrientation.Default;
		[XtraSerializableProperty]
		[ Localizable(true), DefaultValue(TabOrientation.Default)]
		public virtual TabOrientation HeaderOrientation {
			get { return this.headerOrientation; }
			set {
				if(this.HeaderOrientation != value) {
					this.headerOrientation = value;
					this.ShouldUpdateLookAndFeel = true;
					this.ComplexUpdate(true, true);
				}
			}
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Size TextSize {
			get { return Size.Empty; }
		}
		protected virtual LayoutGroup GetHotTrackedGroup(Point point) {
			return null;
		}
		protected internal LayoutType GetLayoutTypeForHinInfo(){
			if (TextLocation == Locations.Top || TextLocation == Locations.Bottom || TextLocation == Locations.Default) return LayoutType.Horizontal;
			else return LayoutType.Vertical;
		}
		protected internal override HitInfo.BaseLayoutItemHitInfo CalcHitInfo(Point hitPoint, bool calcForHandler) {
			TabbedGroupHitInfo hitInfo = new TabbedGroupHitInfo(base.CalcHitInfo(hitPoint, calcForHandler));
			if(((Owner != null && Owner.DesignMode) || AllowCustomizeChildren) && hitInfo.HitType == LayoutItemHitTest.ControlsArea && SelectedTabPage != null && SelectedTabPage.ViewInfo.BoundsRelativeToControl.Contains(hitPoint)) {
				return SelectedTabPage.CalcHitInfo(hitPoint, calcForHandler);
			}
			else {
				Rectangle captionsArea = this.ViewInfo.TabsCaptionArea;
				if(captionsArea.Contains(hitPoint)) {
					if (calcForHandler) {
						if (GetLayoutTypeForHinInfo() == LayoutType.Horizontal) {
							captionsArea.Height /= 2;
							captionsArea.Y += captionsArea.Height;
						} else {
							captionsArea.Width /= 2;
							captionsArea.X += captionsArea.Width;
						}
					}
					if(captionsArea.Contains(hitPoint))
						CalculateTabbedGroupHitTest(hitPoint, hitInfo);
				}
			}
			return hitInfo;
		}
		protected internal override void SetRTL(bool isRTL,bool updatePosition) {
			base.SetRTL(isRTL, updatePosition);
			foreach(LayoutGroup group in TabPages) {
				group.SetRTL(isRTL, updatePosition);
			}
		}
		protected bool CalculateTabbedGroupHitTest(Point hitPoint, TabbedGroupHitInfo hitInfo) {
			ViewInfo.CalculateViewInfo();
			var tabHeaderInfo = ViewInfo.BorderInfo.Tab.ViewInfo.HeaderInfo;
			Rectangle[] tabCaptionScreenRects = new Rectangle[tabHeaderInfo.AllPages.Count];
			for(int i = 0; i < tabCaptionScreenRects.Length; i++)
				tabCaptionScreenRects[i] = ViewInfo.GetScreenTabCaptionRect(i);
			for(int i = (tabCaptionScreenRects.Length - 1); i >= 0; i--) {
				Rectangle rect = tabCaptionScreenRects[i];
				if(rect.Contains(hitPoint)) {
					if(GetLayoutTypeForHinInfo() == LayoutType.Horizontal) {
						if((hitPoint.X - rect.X) < rect.Width / 3 || (rect.Right - hitPoint.X) < rect.Width / 3)
							hitInfo.TabbedGroupMoveType = MoveType.Outside;
						else
							hitInfo.TabbedGroupMoveType = MoveType.Inside;
						if(hitPoint.X > rect.X + rect.Width / 2)
							hitInfo.TabbedGroupInsertLocation = InsertLocation.After;
						else
							hitInfo.TabbedGroupInsertLocation = InsertLocation.Before;
					}
					else {
						if((hitPoint.Y - rect.Y) < rect.Height / 3 || (rect.Bottom - hitPoint.Y) < rect.Height / 3)
							hitInfo.TabbedGroupMoveType = MoveType.Outside;
						else
							hitInfo.TabbedGroupMoveType = MoveType.Inside;
						if(hitPoint.Y > rect.Y + rect.Height / 2)
							hitInfo.TabbedGroupInsertLocation = InsertLocation.After;
						else
							hitInfo.TabbedGroupInsertLocation = InsertLocation.Before;
					}
					hitInfo.TabbedGroupHotPageIndex = i;
					hitInfo.IsLastRow = ViewInfo.IsLastRow(i);
					return true;
				}
			}
			return false;
		}
		internal bool CanDragDroup(int hotTabIndex, InsertLocation insertLocation, LayoutGroup dragGroup) {
			if(hotTabIndex < 0) return false;
			if(dragGroup == TabPages[hotTabIndex]) return false;
			if(dragGroup == null) return false;
			if(dragGroup.Parent == null) return false;
			if(hotTabIndex > 0 && dragGroup == TabPages[hotTabIndex - 1] && insertLocation == InsertLocation.Before) return false;
			if(hotTabIndex < (TabPages.Count - 1) && dragGroup == TabPages[hotTabIndex + 1] && insertLocation == InsertLocation.After) return false;
			return true;
		}
		protected void OnTabs_Changed(object sender, CollectionChangeEventArgs e) {
			LayoutGroup group;
			if(e != null) {
				SetShouldUpdateViewInfo();
				group = e.Element as LayoutGroup;
				if(group == null) return;
				switch(e.Action) {
					case CollectionChangeAction.Add:
						if(Owner != null) ((ILayoutItemOwner)this).AddComponent(group);
						if(group != null) {
							group.Location = Point.Empty;
							group.SetTabbedGroupParent(this);
							group.Owner = Owner;
						}
						break;
					case CollectionChangeAction.Remove:
						if(Owner != null) ((ILayoutItemOwner)this).RemoveComponent(group);
						if(group != null) {
							group.SetTabbedGroupParent(null);
							group.Parent = null;
							group.Owner = Owner;
						}
						break;
				}
				shouldResetBorderInfoCore = true;
				if(Parent != null) Parent.ResizerModifyTree(group);
				ResetVisibleTabPages();
			}
		}
		protected override void UpdateLabelPlace() {
			shouldResetBorderInfoCore = true;
			ShouldResize = true;
			ComplexUpdate();
		}
		public LayoutGroup AddTabPage() {
			return AddTabPage(null, String.Empty);
		}
		public LayoutGroup AddTabPage(String text) {
			return AddTabPage(null, text);
		}
		public LayoutGroup AddTabPage(LayoutGroup newItem) {
			return AddTabPage(newItem, String.Empty);
		}
		public LayoutGroup AddTabPage(LayoutGroup newItem, String text) {
			if(newItem == null) {
				newItem = CreateTab();
				if(text.Length > 0) newItem.Text = text;
				newItem.Owner = Owner;
			}
			if(TabPages.Contains(newItem)) return null;
			BeginChangeUpdate();
			TabPages.Add(newItem);
			newItem.Size = new Size(100, 100);
			SelectedTabPage = newItem;
			NameChanged();
			EndChangeUpdate();
			return newItem;
		}
		protected internal void OnCloseButtonClick(object sender, EventArgs e) {
			ClosePageButtonEventArgs cpbea = e as ClosePageButtonEventArgs;
			if(cpbea == null) return;
			LayoutTabPage tabPage = cpbea.Page as LayoutTabPage;
			if(tabPage == null) return;
			Owner.FireCloseButtonClick(tabPage.Group);
		}
		protected internal void OnSelectedTabChanged(object sender, EventArgs e) {
			if(Parent != null) {
				LayoutGroupHandlerWithTabHelper handler = Parent.Handler as LayoutGroupHandlerWithTabHelper;
				if(handler != null) {
					handler.ActivateTab(this, TabPages.IndexOf(ViewInfo.BorderInfo.Tab.SelectedPage.Group));
				}
			}
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("TabbedGroupOwner")]
#endif
		public override ILayoutControl Owner {
			get { return base.Owner; }
			set {
				base.Owner = value;
				foreach(BaseLayoutItem item in TabPages) {
					item.Owner = value;
				}
			}
		}
		protected internal LayoutGroup AddTabSetDefaults() {
			LayoutGroup group = AddTabPage();
			group.Text = "Tab " + TabPages.Count.ToString();
			group.AddItem().Text = "1";
			group.AddItem().Text = "2";
			group.Padding = Owner != null ? Owner.DefaultValues.LayoutItemPadding : new DevExpress.XtraLayout.Utils.Padding(5);
			return group;
		}
		public bool RemoveTabPage(LayoutGroup item) {
			int index = TabPages.IndexOf(item);
			if(index >= 0) {
				LayoutGroup oldSelectedTabPage = SelectedTabPage;
				item.UpdateChildren(false);
				TabPages.Remove(item);
				shouldResetBorderInfoCore = true;
				if(DisposingFlag) return true;
				if(oldSelectedTabPage == item) {
					if(TabPages.Count > index)
						SelectedTabPage = TabPages[index];
					else {
						if(TabPages.Count > 0)
							SelectedTabPage = TabPages[index - 1];
						else
							SelectedTabPage = null;
					}
				}
				ComplexUpdate();
				return true;
			}
			return false;
		}
		protected internal override void UpdateChildren(bool visible) {
			if(IsDisposing) return;
			base.UpdateChildren(visible);
			foreach(LayoutGroup group in TabPages) {
				Point offset = ViewInfo.ClientAreaRelativeToControl.Location;
				offset.X -= group.ViewInfo.ClientArea.X;
				offset.Y -= group.ViewInfo.ClientArea.Y;
				group.ViewInfo.Offset = offset;
				bool visibleStatus = visible && group == SelectedTabPage && ActualItemVisibility && group.ActualItemVisibility;
				group.SetVisible(visibleStatus);
				group.UpdateChildren(visibleStatus);
				EnsurePageEnabled(group, visibleStatus);
			}
		}
		void EnsurePageEnabled(LayoutGroup group, bool visible) {
			if(!visible || group.PageEnabled) return;
			Tab.LayoutTabPage page = ViewInfo.BorderInfo.Tab.GetPageByGroup(group);																																																										 
			if(page != null) {
				BaseTabControlViewInfo tabInfo = page.TabControl.ViewInfo;
				if(tabInfo != null) tabInfo.SetSelectedTabPageCore(tabInfo.GetSelectablePage());
			}
		}
		protected override ViewInfo.BaseLayoutItemViewInfo CreateViewInfo() {
			if(viewInfoCore != null) viewInfoCore.Destroy();
			ViewInfo.TabbedGroupViewInfo vi = new ViewInfo.TabbedGroupViewInfo(this, TextLocation);
			RestoreTabPagePropertiesAfterReset(vi);
			return vi;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public override BaseLayoutItemOptionsToolTip OptionsToolTip {
			get { return base.OptionsToolTip; }
			set { base.OptionsToolTip = value; }
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ViewInfo.TabbedGroupViewInfo ViewInfo {
			get { return base.ViewInfo as ViewInfo.TabbedGroupViewInfo; }
		}
		protected override void NameChanged() {
			foreach(LayoutGroup group in TabPages) {
				group.TabbedGroupParentName = Name;
			}
		}
		protected virtual LayoutGroup CreateLayoutGroup() {
			LayoutControlGroup group;
			if(Owner != null)
				group = (LayoutControlGroup)Owner.CreateLayoutGroup(null);
			else
				group = new LayoutControlGroup();
			return group;
		}
		protected LayoutGroup CreateTab() {
			return CreateLayoutGroup();
		}
		string selectedTabPageName = String.Empty;
		public void Ungroup() {
			int pagesCount = TabPages.Count;
			if(Parent == null) return;
			LayoutGroup parent = Parent;
			parent.BeginChangeUpdate();
			for(int i = 0; i < pagesCount; i++) {
				LayoutGroup tab = TabPages[0];
				tab.Move(new LayoutItemDragController(tab, this, MoveType.Outside, InsertLocation.Before, LayoutType.Vertical, new Size(10, 10)));
			}
			parent.Remove(this);
			parent.EndChangeUpdate();
		}
		protected internal void UpdateSelectedTabPage() {
			LayoutGroup target = null;
			foreach(LayoutGroup group in TabPages) {
				if(group.Name == SelectedTabPageName) {
					target = group;
				}
			}
			if(target != null) {
				SelectedTabPage = target;
			}
		}
		[XtraSerializableProperty(), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public string SelectedTabPageName {
			get { return selectedTabPageName; }
			set { selectedTabPageName = value; }
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("TabbedGroupSelectedPageChanged")]
#endif
		public event LayoutTabPageChangedEventHandler SelectedPageChanged {
			add { base.Events.AddHandler(selectedPageChangedCore, value); }
			remove { base.Events.RemoveHandler(selectedPageChangedCore, value); }
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("TabbedGroupSelectedPageChanging")]
#endif
		public event LayoutTabPageChangingEventHandler SelectedPageChanging {
			add { base.Events.AddHandler(selectedPageChangingCore, value); }
			remove { base.Events.RemoveHandler(selectedPageChangingCore, value); }
		}
		protected void RaiseTabChangedEvent(LayoutGroup oldValue, LayoutGroup newValue) {
			if((LayoutTabPageChangedEventHandler)this.Events[selectedPageChangedCore] != null) {
				LayoutTabPageChangedEventHandler handler = (LayoutTabPageChangedEventHandler)this.Events[selectedPageChangedCore];
				LayoutTabPageChangedEventArgs e = new LayoutTabPageChangedEventArgs(oldValue, newValue);
				if(handler != null) handler(this, e);
			}
		}
		protected bool RaiseTabChangingEvent(LayoutGroup oldValue, LayoutGroup newValue) {
			if(Owner != null && Owner.IsDeserializing) return false;
			if((LayoutTabPageChangingEventHandler)this.Events[selectedPageChangingCore] != null) {
				LayoutTabPageChangingEventHandler handler = (LayoutTabPageChangingEventHandler)this.Events[selectedPageChangingCore];
				LayoutTabPageChangingEventArgs e = new LayoutTabPageChangingEventArgs(oldValue, newValue, false);
				if(handler != null) handler(this, e);
				return e.Cancel;
			}
			return false;
		}
		protected LayoutGroup FindPageToSelectBefore(int pageIndex) {
			LayoutGroup result = null;
			for(int i = 0; i < pageIndex; i++) {
				if(TabPages[i].RequiredItemVisibility) result = TabPages[i];
			}
			return result;
		}
		protected LayoutGroup FindPageToSelectAfter(int pageIndex) {
			LayoutGroup result = null;
			for(int i = (TabPages.Count - 1); i > pageIndex; i--) {
				if(TabPages[i].RequiredItemVisibility) result = TabPages[i];
			}
			return result;
		}
		protected internal void SelectSomePageNearThis(LayoutGroup page) {
			int removedPageIndex = TabPages.IndexOf(page);
			LayoutGroup selectPageCandidate = FindPageToSelectBefore(removedPageIndex);
			if(selectPageCandidate == null) {
				selectPageCandidate = FindPageToSelectAfter(removedPageIndex);
			}
			if(selectPageCandidate != null)
				SelectedTabPage = selectPageCandidate;
			else
				SelectedTabPage = null;
		}
		protected internal bool isInSelectedTabPage = false;
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("TabbedGroupSelectedTabPage"),
#endif
 TypeConverter(typeof(TypeConverter))]
		public LayoutGroup SelectedTabPage {
			get {
				if(activeTab != null && !activeTab.ActualItemVisibility) return null;
				return activeTab;
			}
			set {
				using(new SafeBaseLayoutItemChanger(this)) {
					try {
						if(isInSelectedTabPage) return;
						if(TabPages.IndexOf(value) >= 0) {
							if(ViewInfo.BorderInfo.Tab.GetPageByGroup(value) == null) {
								ViewInfo.BorderInfo.Tab.Populate();
							}
							LayoutGroup oldValue = activeTab;
							if(activeTab == value) return;
							if(RaiseTabChangingEvent(oldValue, value)) {
								value = oldValue;
							}
							activeTab = value;
							try {
								isInSelectedTabPage = true;
								DevExpress.XtraLayout.Tab.LayoutTabPage selectedCandidate = ViewInfo.BorderInfo.Tab.GetPageByGroup(activeTab);
								if(selectedCandidate != null) {
									ViewInfo.BorderInfo.Tab.SelectedPage = selectedCandidate;
									ViewInfo.BorderInfo.Tab.ViewInfo.CheckFirstPageIndex();
								} 
								if(activeTab == oldValue) return;
								ShouldResize = true;
								ComplexUpdate();
								if (Owner != null && (Owner.ShouldResize || Owner.ShouldUpdateConstraints || Owner.ShouldUpdateControls || Owner.ShouldUpdateControlsLookAndFeel)) ComplexUpdate();
							}
							finally {
								isInSelectedTabPage = false;
							}
							RaiseTabChangedEvent(oldValue, activeTab);
						}
						else {
							if(!IsUpdateLocked && value != null)
								throw new LayoutControlInternalException("No such tab in TabPages collection!");
							activeTab = value;
						}
					}
					finally {
						if(activeTab != null) {
							SelectedTabPageName = activeTab.Name;
						}
						else
							SelectedTabPageName = "";
					}
				}
			}
		}
		[Browsable(false)]
		public int SelectedTabPageIndex {
			get { return activeTab == null ? -1 : VisibleTabPages.IndexOf(activeTab); }
			set {
				if(value >= 0 && value < VisibleTabPages.Count)
					SelectedTabPage = VisibleTabPages[value];
			}
		}
		protected internal override void ChangeSize(int dif, LayoutType layoutType) {
			base.ChangeSize(dif, layoutType);
		}
		TabbedConstraintsHelper constraintsHelper = new TabbedConstraintsHelper();
		protected virtual Size CalculateMinSize() {
			Size minSize = constraintsHelper.CalculateMinSize(VisibleTabPages);
			if(TabPages.Count == 0) return DefaultItemSize;
			return minSize;
		}
		[Browsable(false)]
		public override Size MinSize {
			get {
				if(ViewInfo == null) return DefaultItemSize;
				return ViewInfo.AddLabelIndentions(CalculateMinSize());
			}
		}
		[Browsable(false)]
		public override Size MaxSize {
			get {
				return base.MaxSize;
			}
		}
		protected override void XtraDeserializePadding(XtraEventArgs e) {
			if(Owner == null) base.XtraDeserializePadding(e);
			else {
				if(Owner.OptionsSerialization.RestoreTabbedGroupPadding) base.XtraDeserializePadding(e);
			}
		}
		protected override void XtraDeserializeSpacing(XtraEventArgs e) {
			if(Owner == null) base.XtraDeserializeSpacing(e);
			else {
				if(Owner.OptionsSerialization.RestoreTabbedGroupSpacing) base.XtraDeserializeSpacing(e);
			}
		}
	}
	[DesignTimeVisible(false)]
	[Designer(LayoutControlConstants.TabbedGroupDesignerName, typeof(System.ComponentModel.Design.IDesigner))]
	[DesignerCategory("Component")]
	public class TabbedControlGroup : TabbedGroup {
		public TabbedControlGroup()
			: base(null) {
		}
		protected override DevExpress.XtraLayout.Utils.Padding DefaultSpaces {
			get {
				return Owner != null ? Owner.DefaultValues.TabbedGroupSpacing : new DevExpress.XtraLayout.Utils.Padding(1);
			}
		}
		protected override DevExpress.XtraLayout.Utils.Padding DefaultPaddings {
			get {
				return Owner != null ? Owner.DefaultValues.TabbedGroupPadding : base.DefaultPaddings;
			}
		}
		public new LayoutControlGroup AddTabPage() {
			return base.AddTabPage() as LayoutControlGroup;
		}
		public new LayoutControlGroup AddTabPage(String text) {
			return base.AddTabPage(text) as LayoutControlGroup;
		}
		public new LayoutControlGroup AddTabPage(LayoutGroup newItem) {
			return base.AddTabPage(newItem) as LayoutControlGroup;
		}
		public new LayoutControlGroup AddTabPage(LayoutGroup newItem, String text) {
			return base.AddTabPage(newItem, text) as LayoutControlGroup;
		}
		public TabbedControlGroup(LayoutGroup owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("TabbedControlGroupTabPages"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		new public TabbedGroupsCollection TabPages {
			get {
				return base.TabPages;
			}
		}
		protected override Type GetDefaultWrapperType() {
			if(Parent != null && Parent.LayoutMode == LayoutMode.Table) return typeof(TableTabbedControlGroupWrapper);
			return typeof(TabbedControlGroupWrapper);
		}
	}
	public class TabbedControlGroupWrapper : BaseLayoutItemWrapper {
		protected TabbedControlGroup Tab { get { return WrappedObject as TabbedControlGroup; } }
		[Category("Appearance")]
		public virtual LayoutPageAppearance AppearanceTabPage { get { return Tab.AppearanceTabPage; } }
		[Category("Appearance")]
		public virtual AppearanceObject AppearanceGroup { get { return Tab.AppearanceGroup; } }
		[DefaultValue(true)]
		public virtual bool AllowCustomizeChildren { get { return Tab.AllowCustomizeChildren; } set { Tab.AllowCustomizeChildren = value; OnSetValue(Item, value); } }
		[DefaultValue(TabOrientation.Default), Category("TabHeader")]
		public virtual TabOrientation HeaderOrientation { get { return Tab.HeaderOrientation; } set { Tab.HeaderOrientation = value; OnSetValue(Item, value); } }
		[DefaultValue(DefaultBoolean.Default), Category("TabHeader")]
		public virtual DefaultBoolean MultiLine { get { return Tab.MultiLine; } set { Tab.MultiLine = value; OnSetValue(Item, value); } }
		[DefaultValue(DefaultBoolean.Default), Category("TabHeader")]
		public virtual DefaultBoolean ShowTabHeader { get { return Tab.ShowTabHeader; } set { Tab.ShowTabHeader = value; OnSetValue(Item, value); } }
		[Browsable(false)]
		public override int TextToControlDistance { get { return base.TextToControlDistance; } set { base.TextToControlDistance = value; OnSetValue(Item, value); } }
		public override BasePropertyGridObjectWrapper Clone() {
			return new TabbedControlGroupWrapper();
		}
	}
	public class FlowTabbedControlGroupWrapper : TabbedControlGroupWrapper {
		[Category("OptionsFlowLayoutItem"), DefaultValue(false)]
		public virtual bool NewLineInFlowLayout { get { return Item.StartNewLine; } set { Item.StartNewLine = value; OnSetValue(Item, value); } }
		public override BasePropertyGridObjectWrapper Clone() {
			return new FlowTabbedControlGroupWrapper();
		}																								   
	}
	public class TableTabbedControlGroupWrapper :TabbedControlGroupWrapper {
		[Category("OptionsTableLayoutItem"), DefaultValue(1)]
		public virtual int RowSpan { get { return Item.OptionsTableLayoutItem.RowSpan; } set { Item.OptionsTableLayoutItem.RowSpan = value; OnSetValue(Item, value); } }
		[Category("OptionsTableLayoutItem"), DefaultValue(1)]
		public virtual int ColumnSpan { get { return Item.OptionsTableLayoutItem.ColumnSpan; } set { Item.OptionsTableLayoutItem.ColumnSpan = value; OnSetValue(Item, value); } }
		[Category("OptionsTableLayoutItem")]
		public virtual int RowIndex { get { return Item.OptionsTableLayoutItem.RowIndex; } set { Item.OptionsTableLayoutItem.RowIndex = value; OnSetValue(Item, value); } }
		[Category("OptionsTableLayoutItem")]
		public virtual int ColumnIndex { get { return Item.OptionsTableLayoutItem.ColumnIndex; } set { Item.OptionsTableLayoutItem.ColumnIndex = value; OnSetValue(Item, value); } }
		public override BasePropertyGridObjectWrapper Clone() {
			return new TableTabbedControlGroupWrapper();
		}
	}
}
