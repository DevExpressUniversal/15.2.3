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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraEditors.ButtonPanel;
using DevExpress.XtraEditors.ButtonsPanelControl;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.Helpers;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout.Localization;
using DevExpress.XtraLayout.Resizing;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.XtraLayout {
	public abstract class LayoutItemContainer : BaseLayoutItem, ILayoutItemOwner {
		protected bool isExpandButtonVisibleInternal = false;
		protected ExpandButtonMode expandButtonModeCore = ExpandButtonMode.Normal;
		protected LayoutItemContainer() : this(null) { }
		AppearanceObject appearanceGroupParent;
		AppearanceObject appearanceGroup;
		LayoutPageAppearance appearanceTabPage;
		LayoutGroupAppearance paintAppearanceGroupCore = null;
		internal LayoutItemContainer(LayoutGroup parent)
			: base(parent) {
			this.appearanceGroupParent = new AppearanceObject();
			this.appearanceGroup = new AppearanceObject(appearanceGroupParent);
			this.appearanceGroup.Changed += new EventHandler(OnAppearanceChanged);
			this.appearanceTabPage = new LayoutPageAppearance();
			this.appearanceTabPage.Changed += new EventHandler(OnAppearanceChanged);
		}
		public override LayoutGroup Parent {
			get { return base.Parent; }
			set {
				base.Parent = value;  ;
			}
		}
		protected override void CloneCommonProperties(LayoutGroup parent, ILayoutControl owner, ref BaseLayoutItem clone) {
			base.CloneCommonProperties(parent, owner, ref clone);
			LayoutItemContainer cloneItem = (LayoutItemContainer)clone;
			cloneItem.paintAppearanceGroupCore = null;
			cloneItem.appearanceGroupParent = new AppearanceObject();
			cloneItem.appearanceGroup = new AppearanceObject(cloneItem.appearanceGroupParent);
			cloneItem.appearanceGroup.AssignInternal(this.appearanceGroup);
			cloneItem.appearanceGroup.Changed += cloneItem.OnAppearanceChanged;
			cloneItem.appearanceTabPage = new LayoutPageAppearance();
			cloneItem.appearanceTabPage.Header.AssignInternal(this.appearanceTabPage.Header);
			cloneItem.appearanceTabPage.HeaderActive.AssignInternal(this.appearanceTabPage.HeaderActive);
			cloneItem.appearanceTabPage.HeaderDisabled.AssignInternal(this.appearanceTabPage.HeaderDisabled);
			cloneItem.appearanceTabPage.HeaderHotTracked.AssignInternal(this.appearanceTabPage.HeaderHotTracked);
			cloneItem.appearanceTabPage.PageClient.AssignInternal(this.appearanceTabPage.PageClient);
			cloneItem.appearanceTabPage.Changed += cloneItem.OnAppearanceChanged;
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int TextToControlDistance {
			get { return 0; }
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutItemContainerTextLocation"),
#endif
 DefaultValue(Locations.Top)]
		[RefreshProperties(RefreshProperties.All)]
		public override Locations TextLocation {
			get { return base.TextLocation; }
			set {
				base.TextLocation = value;
			}
		}
		bool ShouldSerializeAppearanceTabPage() { return AppearanceTabPage != null ? AppearanceTabPage.ShouldSerialize() : false; }
		void ResetAppearanceTabPage() { AppearanceTabPage.Reset(); }
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutItemContainerAppearanceTabPage"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public LayoutPageAppearance AppearanceTabPage { get { return appearanceTabPage; } }
		void XtraDeserializeAppearanceTabPage(XtraEventArgs ea) {
			if(Owner != null && Owner.OptionsSerialization.RestoreAppearanceTabPage) {
				DeserializeHelper dh = new DeserializeHelper();
				if(ea.Info != null) dh.DeserializeObject(AppearanceTabPage, ea.Info.ChildProperties, OptionsLayoutBase.FullLayout);
			}
		}
		[Browsable(false)]
		public override AppearanceObject PaintAppearanceItemCaption {
			get { return PaintAppearanceGroup.AppearanceItemCaption; }
		}
		[Browsable(false)]
		public virtual LayoutGroupAppearance PaintAppearanceGroup {
			get {
				if(Owner != null)
					return Owner.AppearanceController.GetAppearanceItemContainer(this);
				else {
					if(paintAppearanceGroupCore == null) paintAppearanceGroupCore = new LayoutGroupAppearance();
					return paintAppearanceGroupCore;					 
				}
			}
		}
		bool ShouldSerializeAppearanceGroup() { return AppearanceGroup != null ? AppearanceGroup.ShouldSerialize() : false; }
		void ResetAppearanceGroup() { AppearanceGroup.Reset(); }
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutItemContainerAppearanceGroup"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public AppearanceObject AppearanceGroup { get { return appearanceGroup; } }
		void XtraDeserializeAppearanceGroup(XtraEventArgs ea) {
			if(Owner != null && Owner.OptionsSerialization.RestoreLayoutGroupAppearanceGroup) {
				DeserializeHelper dh = new DeserializeHelper();
				if(ea.Info != null) dh.DeserializeObject(AppearanceGroup, ea.Info.ChildProperties, OptionsLayoutBase.FullLayout);
			}
		}
		ILayoutControl ILayoutItemOwner.LayoutControl { get { return Owner == null ? null : Owner; } }
		void ILayoutItemOwner.SelectionChanged(IComponent component) {
			SelectionChanged(component);
		}
		protected override bool ShouldSerializeMaxSize() {
			return false;
		}
		protected override bool ShouldSerializeMinSize() {
			return false;
		}																																	   
		protected internal bool BeginChangeUpdate(bool setVisibilityState = true) {
			bool result = StartChange();
			if(result) BeginUpdate();
			if(Parent != null && Parent.ResizeManager != null && setVisibilityState) Parent.ResizeManager.VisibilityState = true;
			return result;
		}
		protected internal void EndChangeUpdate(bool setVisibilityState = true) {
			if(Parent != null && Parent.ResizeManager != null && setVisibilityState) Parent.ResizeManager.VisibilityState = false;
			EndUpdate();
			EndChange();
		}
		bool allowCustomize = true;
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutItemContainerAllowCustomizeChildren"),
#endif
 DefaultValue(true)]
		public virtual bool AllowCustomizeChildren {
			get { return allowCustomize; }
			set { allowCustomize = value; Invalidate(); }
		}
		protected internal void BeginChangeInit() {
			BeginInit();
			StartChange();
		}
		protected internal void EndChangeInit() {
			EndInit();
			EndChange();
		}
		public void BeginUpdate() {
			BeginInit();
		}
		public virtual void EndUpdate() {
			EndInit();
			if(!IsUpdateLocked) {
				ComplexUpdate(true, true);
			}
		}
		protected internal override void UpdateChildren(bool visible) { base.UpdateChildren(visible); }
		public bool Contains(BaseLayoutItem item) {
			ContainsItemVisitor visitor = new ContainsItemVisitor(item);
			Accept(visitor);
			return visitor.Contains;
		}
		void ILayoutItemOwner.AddComponent(BaseLayoutItem component) {
			if(Owner != null) Owner.AddComponent(component);
		}
		void ILayoutItemOwner.RemoveComponent(BaseLayoutItem component) {
			if(Owner != null) Owner.RemoveComponent(component);
		}
		protected override void UpdateLabelPlace() {
			shouldResetBorderInfoCore = true;
		}
		protected internal void ProcessChildTextChanged() {
			shouldResetBorderInfoCore = true;
			ComplexUpdate();
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public override Size TextSize {
			get { return Size.Empty; }
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(appearanceGroup != null) {
					this.appearanceGroup.Changed -= OnAppearanceChanged;
					this.appearanceGroup.Dispose();
					this.appearanceGroup = null;
				}
				if(appearanceGroupParent != null) {
					this.appearanceGroupParent.Dispose();
					this.appearanceGroupParent = null;
				}
				if(appearanceTabPage != null) {
					this.appearanceTabPage.Changed -= OnAppearanceChanged;
					this.appearanceTabPage.Dispose();
					this.appearanceTabPage = null;
				}
				if(paintAppearanceGroupCore != null) {
					this.paintAppearanceGroupCore.Dispose();
					this.paintAppearanceGroupCore = null;
				}
			}
			base.Dispose(disposing);
		}
		protected internal virtual Size DefaultItemSize { get { return new Size(50, 25); } }
		protected internal virtual Size DefaultMinItemSize { get { return new Size(50, 25); } }
		protected internal virtual Size DefaultMaxItemSize { get { return new Size(100, 25); } }
		int tabPageWidthCore = 0;
		[XtraSerializableProperty(), DefaultValue(0)]
		public int TabPageWidth {
			get {
				return tabPageWidthCore;
			}
			set {
				if(value == tabPageWidthCore || value < 0) return;
				tabPageWidthCore = value;
				shouldResetBorderInfoCore = true;
				ComplexUpdate();
			}
		}
	}
	class UpdateControlVisibilityHelper : UpdateControlsHelper {
		protected override void Update(LayoutControlItem item) {
			if(item != null && item.Control != null && item.Owner != null && item.Owner.Control != null)
				item.Control.Parent = item.Owner.Control.Parent;
		}
	}
	class TextToControlDistanseSetter : BaseVisitor {
		int distanse;
		public TextToControlDistanseSetter(int newDistanse)
			: base() {
			distanse = newDistanse;
		}
		public override void Visit(BaseLayoutItem item) {
			LayoutControlItem citem = item as LayoutControlItem;
			if(citem != null) {
				citem.TextToControlDistance = distanse;
			}
		}
	}
	public class RotateLayoutHelper : BaseVisitor {
		public override void Visit(BaseLayoutItem item) {
			if(item.Parent == null) return;
			Rectangle rotateBounds = item.Parent.ViewInfo.ClientArea;
			double wFactor = (double)rotateBounds.Width / (double)rotateBounds.Height;
			double hFactor = (double)rotateBounds.Height / (double)rotateBounds.Width;
			Rectangle itemBounds = item.Bounds;
			item.BeginInit();
			item.Size = new Size(
					Round((double)itemBounds.Height * wFactor),
					Round((double)itemBounds.Width * hFactor)
				);
			item.Location = new Point(
					Round((double)itemBounds.Y * wFactor),
					Round((double)itemBounds.X * hFactor)
				);
			item.EndInit();
		}
		static int Round(double d) {
			return d > 0 ? (int)(d + 0.5) : (int)(d - 0.5);
		}
	}
	public class FlipLayoutHelper : BaseVisitor {
		public override void Visit(BaseLayoutItem item) {
			if(item.Parent == null) return;
			Rectangle flipBounds = item.Parent.ViewInfo.ClientArea;
			Rectangle itemBounds = item.Bounds;
			item.BeginInit();
			item.Location = new Point(flipBounds.Width - itemBounds.Right, itemBounds.Y);
			item.EndInit();
		}
	}
	public class ContainsHelper : BaseVisitor {
		bool contains = false;
		BaseLayoutItem testItem = null;
		public override void Visit(BaseLayoutItem item) {
			if(item == testItem) contains = true;
		}
		public BaseLayoutItem Item { get { return testItem; } set { testItem = value; contains = false; } }
		public bool Contains { get { return contains; } }
	}
	class CompactHelper {
		readonly LayoutGroup[] Groups;
		static readonly LayoutGroup[] EmptyResult = new LayoutGroup[0];
		public CompactHelper(List<LayoutGroup> groups)
			: this(groups.ToArray()) {
		}
		public CompactHelper(LayoutGroup[] groups) {
			Groups = groups;
		}
		public LayoutGroup[] GetGroupsToRemove() {
			return Groups.Length > 1 ?
				Array.FindAll(Groups, HasParentInGroups) : EmptyResult;
		}
		LayoutGroup CurrentGroup, CurrentParent;
		bool HasParentInGroups(LayoutGroup group) {
			CurrentGroup = group;
			CurrentParent = group.Parent;
			while(CurrentParent != null) {
				if(Array.Exists(Groups, IsPotentialParent)) return true;
				CurrentParent = CurrentParent.Parent;
			}
			return false;
		}
		bool IsPotentialParent(LayoutGroup potentialParent) {
			return potentialParent != CurrentGroup && potentialParent == CurrentParent;
		}
	}
	public class ResizeManager : IDisposable {
		internal Resizer resizer;
		List<LayoutGroup> affectedGroups;
		LayoutGroup ownerGroup = null;
		bool shouldUpdateProportions = false;
		SplitterItem allowResetElementsInSplitterGroupsSplitterItem = null;
		bool shouldUpdateAfterRemoving = false;
		bool allowResetResizerCore = true;
		public ResizeManager(LayoutGroup owner) {
			this.ownerGroup = owner;
			affectedGroups = new List<LayoutGroup>();
		}
		public virtual void Dispose() {
			if(resizer != null) {
				if(resizer.resultH != null) resizer.resultH.Dispose();
				if(resizer.resultV != null) resizer.resultV.Dispose();
				resizer.resultH = null;
				resizer.resultV = null;
				resizer = null;
			}
			this.ownerGroup = null;
		}
		protected virtual Resizer CreateResizer(LayoutGroup ownerGroupCore) {
			return new Resizer(ownerGroupCore);
		}
		int refreshResizerRequestsCount = 0;
		protected virtual void DoRefreshResizer() {
			try {
				if(refreshResizerRequestsCount < 10 && ownerGroup != null) {
					refreshResizerRequestsCount++;
					resizer = CreateResizer(ownerGroup);
					AfterResizerHardReset();
				} else resizer = null;
			} catch(Exception e) {
				if(e is LayoutControlException) throw e;
				DoRefreshResizer();
				ownerGroup.UpdateLayout();
			} finally {
				affectedGroups.Clear();
				refreshResizerRequestsCount--;
			}
		}
		protected void CompactAffectedGroups() {
			CompactHelper helper = new CompactHelper(affectedGroups);
			LayoutGroup[] groupsToRemove = helper.GetGroupsToRemove();
			for(int i = 0; i < groupsToRemove.Length; i++)
				affectedGroups.Remove(groupsToRemove[i]);
		}
		protected void DoPartialResizerRefresh() {
			CompactAffectedGroups();
			foreach(LayoutGroup currentGroup in affectedGroups) {
				DoUpdateResizerSimple(currentGroup);
			}
			LayoutGroup[] groups = affectedGroups.ToArray();
			DoPostUpdateResizer();
			Array.ForEach(groups, DoUpdateResizerConstraints);
		}
		protected void DoUpdateResizerProportions() {
			resizer.UpdateProportions(allowResetElementsInSplitterGroupsSplitterItem);
		}
		int isCalculatingResizer = 0;
		protected void CalculateResizer() {
			if(resizer == null) {
				DoRefreshResizer();
				SetVisibilityStateForceUpdate(false);
				return;
			}
			if(isCalculatingResizer != 0) return;
			try {
				isCalculatingResizer++;
				if(affectedGroups.Count > 0) {
					try {
						DoPartialResizerRefresh();
					} catch(LayoutControlInternalException) {
						if(ownerGroup.Owner != null) ownerGroup.Owner.ExceptionsThrown = true;
						affectedGroups.Clear();
						DoRefreshResizer();
					}
					SetVisibilityStateForceUpdate(false);
				}
				if(shouldUpdateProportions) {
					DoUpdateResizerProportions();
					shouldUpdateProportions = false;
					allowResetElementsInSplitterGroupsSplitterItem = null;
				}
				if(shouldUpdateAfterRemoving) {
					resizer.UpdateConstraints();
					shouldUpdateAfterRemoving = false;
				}
			} finally {
				isCalculatingResizer--;
			}
		}
		protected void DoUpdateResizerSimple(LayoutGroup group) {
			resizer.UpdateResizer(group);
		}
		protected void DoPostUpdateResizer() {
			affectedGroups.Clear();
		}
		protected void DoUpdateResizerConstraints(LayoutGroup group) {
			resizer.UpdateConstraints();
		}
		public void CompleteIfNeeded(LayoutGroup group) {
			if(Resizer.CompleteIfNeeded(group)) {
				ApplyActualVisibilityStatus(true);
			}
		}
		public Resizer Resizer {
			get {
				CalculateResizer();
				return resizer;
			}
		}
		public void SetSplitterSizing(SplitterItem si) {
			allowResetElementsInSplitterGroupsSplitterItem = si;
		}
		public void ResetResizerProportions() {
			shouldUpdateProportions = true;
			SetIsModified();
		}
		public bool AllowResetResizer {
			get { return allowResetResizerCore; }
			set { allowResetResizerCore = value; }
		}
		public void ModifyTree(BaseLayoutItem item, LayoutGroup group) {
			TremporaryDumb(group);
		}
		protected void TremporaryDumb(LayoutGroup group) {
			ResetResizer(group);
		}
		public void ResetResizer(LayoutGroup group) {
			if(!allowResetResizerCore) return;
			VisibilityState = true;
			if(group == ownerGroup) {
				BeforeResizerHardReset();
				resizer = null;
				affectedGroups.Clear();
			} else {
				if(!affectedGroups.Contains(group))
					affectedGroups.Add(group);
			}
			SetIsModified();
		}
		protected virtual void BeforeResizerHardReset() {
		}
		protected virtual void AfterResizerHardReset() {
		}
		protected void SetIsModified() {
			if(ownerGroup.Owner != null) ownerGroup.Owner.SetIsModified(true);
		}
		public void UpdateVisibility() {
			ApplyActualVisibilityStatus(false);
		}
		public void UpdateVisibility(BaseLayoutItem item) {
			ApplyActualVisibilityStatusItem(item, false);
		}
		protected void ApplyActualVisibilityStatus(bool force) {
			if(ownerGroup.IsDisposing) return;
			List<BaseLayoutItem> reverseList = new FlatItemsList().GetItemsList(ownerGroup);
			reverseList.Reverse();
			BaseLayoutItem[] items = reverseList.ToArray();
			ownerGroup.BeginInit();
			bool needUpdate = false;
			Array.ForEach(items,
				delegate(BaseLayoutItem item) {
					if(!item.IsDisposing) needUpdate |= ApplyActualVisibilityStatusItem(item, force);
				}
			);
			if(needUpdate) UpdateTabbedGroups(items); 
			ownerGroup.EndInit();
			if(needUpdate) ownerGroup.Invalidate();
		}
		static void UpdateTabbedGroups(BaseLayoutItem[] items) {
			Array.ForEach(items,
				delegate(BaseLayoutItem item) {
					if(item is TabbedGroup) ((TabbedGroup)item).ResetVisibleTabPages();
				}
			);
		}
		protected virtual void HideItemFromLayout(BaseLayoutItem bitem) {
			LayoutClassificationArgs result = LayoutClassifier.Default.Classify(bitem);
			if(result.IsTabPage) result.Group.ParentTabbedGroup.ResetVisibleTabPages();
		}
		protected virtual void AddItemToLayout(BaseLayoutItem bitem) {
			LayoutClassificationArgs result = LayoutClassifier.Default.Classify(bitem);
			if(result.IsTabPage) result.Group.ParentTabbedGroup.ResetVisibleTabPages();
		}
		public void RotateLayout(LayoutGroup group) {
			RotateLayout(group, true);
		}
		public void FlipLayout(LayoutGroup group) {
			FlipLayout(group, true);
		}
		public void RotateLayout(LayoutGroup group, bool processChildGroups) {
			AcceptGroup(new RotateLayoutHelper(), group, processChildGroups);
		}
		public void FlipLayout(LayoutGroup group, bool processChildGroups) {
			AcceptGroup(new FlipLayoutHelper(), group, processChildGroups);
		}
		void AcceptGroup(BaseVisitor visitor, LayoutGroup group, bool processChildGroups) {
			VisibilityState = true;
			if(processChildGroups)
				group.Accept(visitor);
			else {
				foreach(BaseLayoutItem bli in group.Items) {
					visitor.Visit(bli);
				}
			}
			ResetResizer(group);
			ownerGroup.ComplexUpdate(true, true, true);
		}
		protected bool ApplyActualVisibilityStatusItem(BaseLayoutItem bitem, bool force) {
			bool requiredVisibility = bitem.RequiredItemVisibility | VisibilityState;
			bool result = true;
			if(bitem.ActualItemVisibility != requiredVisibility | force) {
				if(resizer == null) return false;
				if(!requiredVisibility) {
					resizer.MarkRemoved(bitem);
					HideItemFromLayout(bitem);
				} else {
					resizer.MarkRestored(bitem);
					AddItemToLayout(bitem);
				}
				bitem.ActualItemVisibility = requiredVisibility;
				shouldUpdateAfterRemoving = true;
				ownerGroup.ComplexUpdate(true, true, true);
			} else result = false;
			return result;
		}
		bool visibilityState = true;
		protected internal bool VisibilityState {
			get { return visibilityState; }
			set {
				visibilityState = value;
				ApplyActualVisibilityStatus(false);
			}
		}
		protected void SetVisibilityStateForceUpdate(bool value) {
			visibilityState = value;
			ApplyActualVisibilityStatus(true);
		}
	}
	public class LayoutGroup : LayoutItemContainer, IGroupBoxButtonsPanelOwner, IButtonPanelControlAppearanceOwner {
		ResizeManager resizeManager;
		bool expanded;
		bool isCloning = false;
		protected TabbedGroup tabbedGroupParentCore;
		bool isGroupBoundsVisible;
		bool isGroupBackgroundVisible = true;
		bool isCaptionImageVisible = true;
		LayoutType defaultLayoutType;
		LayoutMode defaultLayoutMode = LayoutMode.Regular;
		BaseItemCollection selectedItems;
		LayoutGroupItemCollection items;
		Image captionImageCore = null;
		int captionImageIndexCore = -1;
		GroupElementLocation captionImageLocationCore = GroupElementLocation.Default;
		GroupElementLocation buttonsLocation = GroupElementLocation.Default;
		Size prefredSize;
		Size cellSize;
		bool prefredSizeDirty;
		bool allowBorderColorBlending;
		OptionsItemTextGroup optionsItemTextCore;
		bool enabledCore = true;
		internal LayoutGroup(LayoutGroup parent)
			: base(parent) {
			this.defaultLayoutType = LayoutType.Vertical;
			this.items = new LayoutGroupItemCollection();
			this.SetInternalSize(this.DefaultItemSize);
			Items.Changed += OnItems_Changed;
			this.selectedItems = new BaseItemCollection();
			this.expanded = true;
			this.optionsItemTextCore = new OptionsItemTextGroup(this);
			this.customHeaderButtonsCore = CreateButtonCollection();
			this.CustomHeaderButtons.CollectionChanged += OnButtonCollectionChanged;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(viewInfoCore != null) viewInfoCore.Destroy();
				if(Items != null) Items.Changed -= OnItems_Changed;
				if(CustomHeaderButtons != null) CustomHeaderButtons.CollectionChanged -= OnButtonCollectionChanged;
				if(selectedItems != null) {
					selectedItems.Clear();
					selectedItems = null;
				}
				if(optionsItemTextCore != null) {
					optionsItemTextCore.Dispose();
					optionsItemTextCore = null;
				}
				if(resizeManager != null) {
					resizeManager.Dispose();
					resizeManager = null;
				}
				DisposeTableOptionsAndManagers();
				optionsTableLayout = null;
			}
			base.Dispose(disposing);
		}
		protected internal override void SetPropertiesDefault() {
			base.SetPropertiesDefault();
			this.isGroupBoundsVisible = true;
		}
		protected virtual BaseButtonCollection CreateButtonCollection() {
			return new BaseButtonCollection(null);
		}
		protected override void FakeOwnerUpdate() {
			Resizer.UpdateConstraints();
			UpdateLayout();
			ViewInfo.Offset = Location;
			UpdateChildren(Expanded);
		}
		protected internal override void AssignInternal(BaseLayoutItem item) {
			LayoutGroup group = item as LayoutGroup;
			if(group != null) {
				base.AssignInternal(group);
				if(items != null && items.Count == group.items.Count) {
					for(int i = 0; i < items.Count; i++) {
						items[i].AssignInternal(group.items[i]);
					}
				}
				this.captionImageIndexCore = group.captionImageIndexCore;
				this.captionImageLocationCore = group.captionImageLocationCore;
				this.enabledCore = group.enabledCore;
				this.buttonsLocation = group.buttonsLocation;
				this.isCaptionImageVisible = group.isCaptionImageVisible;
				this.isGroupBoundsVisible = group.isGroupBoundsVisible;
				this.isGroupBackgroundVisible = group.isGroupBackgroundVisible;
				this.defaultLayoutType = group.defaultLayoutType;
				this.expanded = group.expanded;
				this.prefredSize = group.prefredSize;
				this.prefredSizeDirty = group.prefredSizeDirty;
				this.resizeManager = null;
			}
		}
		protected override void CloneCommonProperties(LayoutGroup parent, ILayoutControl owner, ref BaseLayoutItem clone) {
			LayoutGroupItemCollection itemsCopy = this.items;
			base.CloneCommonProperties(parent, owner, ref clone);
			LayoutGroup cloneGroup = (LayoutGroup)clone;
			cloneGroup.isCloning = true;
			if(this.backgroundImageCore != null) { cloneGroup.backgroundImageCore = (Image)this.backgroundImageCore.Clone(); }
			if(this.captionImageCore != null) { cloneGroup.captionImageCore = (Image)this.captionImageCore.Clone(); }
			if(this.contentImageCore != null) { cloneGroup.contentImageCore = (Image)this.contentImageCore.Clone(); }
			cloneGroup.items = new LayoutGroupItemCollection();
			cloneGroup.selectedItems = new BaseItemCollection();
			cloneGroup.items.Changed -= this.OnItems_Changed;
			cloneGroup.items.Changed += cloneGroup.OnItems_Changed;
			foreach(BaseLayoutItem item in itemsCopy) {
				BaseLayoutItem itemClone = item.Clone(cloneGroup, owner);
				cloneGroup.items.Add(itemClone);
			}
			cloneGroup.optionsItemTextCore = new OptionsItemTextGroup(cloneGroup);
			cloneGroup.optionsItemTextCore.AssignInternal(this.optionsItemTextCore);
			cloneGroup.resizeManager = null;
			cloneGroup.isCloning = false;
		}
		public override string GetDefaultText() {
			if(Name != "")
				return Name;
			String itemText = LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.ControlGroupDefaultText);
			return itemText;
		}
		[XtraSerializableProperty(), DefaultValue(""), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string TypeName {
			get { return "LayoutGroup"; }
		}
		internal void CreateLayout(int Rows, int Cols) {
			BeginUpdate();
			Items.Clear();
			int width = 100;
			int height = 25;
			for(int i = 0; i < Rows; i++) {
				for(int j = 0; j < Cols; j++) {
					BaseLayoutItem item = CreateLayoutItem();
					Items.Add(item);
					item.SetBounds(new Rectangle(j * width, i * height, width, height));
					item.MaxSize = new Size(150, 50);
				}
			}
			EndUpdate();
			Size = new Size(width * Cols, height * Rows);
		}
		string tabbedGroupParentName;
		[XtraSerializableProperty(), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public string TabbedGroupParentName {
			get {
				return tabbedGroupParentName;
			}
			set {
				tabbedGroupParentName = value;
			}
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TabbedGroup ParentTabbedGroup {
			get { return tabbedGroupParentCore; }
		}
		protected internal override bool DisposingFlag {
			get { return base.DisposingFlag || tabbedGroupParentCore != null && tabbedGroupParentCore.DisposingFlag; }
		}
		protected internal void SetTabbedGroupParent(TabbedGroup group) {
			var prevGroup = tabbedGroupParentCore;
			tabbedGroupParentCore = group;
			if(group != null) EnsureExpanded(this);
			if(group != prevGroup)
				UpdateControlsBackColor(this);
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutGroupGroupBordersVisible"),
#endif
 DefaultValue(true)]
		[XtraSerializableProperty()]
		public bool GroupBordersVisible {
			get { return isGroupBoundsVisible; }
			set {
				if(isGroupBoundsVisible == value) return;
				isGroupBoundsVisible = value;
				shouldResetBorderInfoCore = true;
				ShouldUpdateConstraintsDoUpdate = true;
				ResetAppearance();
				ComplexUpdate(true, true);
			}
		}
		void ResetAppearance() {
			if(Owner != null)
				Owner.AppearanceController.SetDefaultAppearanceDirty(this);
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutGroupGroupBordersVisible"),
#endif
 DefaultValue(true)]
		[XtraSerializableProperty()]
		public bool AllowDrawBackground {
			get { return isGroupBackgroundVisible; }
			set {
				if(AllowDrawBackground == value) return;
				isGroupBackgroundVisible = value;
				shouldResetBorderInfoCore = true;
				ShouldUpdateConstraintsDoUpdate = true;
				ComplexUpdate(true, true);
			}
		}
		DefaultBoolean enableIndentionsWithoutBordersCore = DefaultBoolean.Default;
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutGroupEnableIndentsWithoutBorders"),
#endif
 DefaultValue(DefaultBoolean.Default)]
		[XtraSerializableProperty(XtraSerializationVisibility.Visible)]
		public DefaultBoolean EnableIndentsWithoutBorders {
			get {
				return enableIndentionsWithoutBordersCore;
			}
			set {
				if(enableIndentionsWithoutBordersCore == value) return;
				enableIndentionsWithoutBordersCore = value;
				shouldResetBorderInfoCore = true;
				ShouldUpdateConstraintsDoUpdate = true;
				ComplexUpdate(true, true);
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutGroupOptionsItemText"),
#endif
 DefaultValue(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Category("Text")]
		public OptionsItemTextGroup OptionsItemText {
			get { return optionsItemTextCore; }
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutGroupCaptionImageVisible"),
#endif
 DefaultValue(true)]
		[XtraSerializableProperty()]
		public bool CaptionImageVisible {
			get { return isCaptionImageVisible; }
			set {
				if(isCaptionImageVisible == value) return;
				isCaptionImageVisible = value;
				shouldResetBorderInfoCore = true;
				ShouldUpdateConstraintsDoUpdate = true;
				ComplexUpdate(true, true);
			}
		}
		[XtraSerializableProperty()]
		public virtual Size CellSize {
			get {
				return cellSize;
			}
			set {
				if(value.Height >= 10 && value.Width >= 10) {
					cellSize = value;
					Invalidate();
				}
			}
		}
		bool ShouldSerializeCellSize() { return defaultLayoutMode == Utils.LayoutMode.Flow; }
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutGroupLayoutMode"),
#endif
 DefaultValue(LayoutMode.Regular)]
		[XtraSerializableProperty()]
		public virtual LayoutMode LayoutMode {
			get { return defaultLayoutMode; }
			set {
				if(defaultLayoutMode == value) return;
				switch(value) {
					case LayoutMode.Regular:
						defaultLayoutMode = value;
						if(IsInitializing) return;
						if(IsRoot) try { Resizer.CreateNewResizing(); } catch { }
						else try { Resizer.UpdateResizer(this); } catch { }
						ResetAppearance();
						DisposeTableOptionsAndManagers();
						break;
					case LayoutMode.Flow:
						if(IsRoot && value == Utils.LayoutMode.Flow) return;
						defaultLayoutMode = value;
						if(IsInitializing) return;
						if(!CanChangeLayoutMode() && value == Utils.LayoutMode.Flow) return;
						SortItems();
						try { Resizer.UpdateResizer(this); } catch { }
						ResetAppearance();
						SetCellFactors();
						UpdateFlowLayoutItems(true);
						DisposeTableOptionsAndManagers();
						break;
					case LayoutMode.Table:
						defaultLayoutMode = value;
						if(IsInitializing) return;
						if(optionsTableLayout == null || (optionsTableLayout.ColumnCount == 0 && optionsTableLayout.RowCount == 0)) {
							optionsTableLayout = new OptionsTableLayoutGroup(Owner, this);
							optionsTableLayout.Add(new RowDefinition(this, 100, SizeType.Percent)); 
							optionsTableLayout.Add(new RowDefinition(this, 100, SizeType.Percent));
							optionsTableLayout.Add(new ColumnDefinition(this, 100, SizeType.Percent));
							optionsTableLayout.Add(new ColumnDefinition(this, 100, SizeType.Percent));
						}
						SortItems();
						if(Items != null) {
							CheckRowColumnTable();
						}
						if(IsRoot) try { Resizer.CreateNewResizing(); } catch { } 
						else try { Resizer.UpdateResizer(this); } catch { }
						ResetAppearance();
						break;
				}
				UpdateCustomizationPropertyGrid();
				ComplexUpdate(true, true);
			}
		}
		private void UpdateCustomizationPropertyGrid() {
			if(Owner != null && Owner.CustomizationForm != null && Owner.EnableCustomizationMode && Owner.CustomizationForm is CustomizationForm) {
				CustomizationForm customizationForm = Owner.CustomizationForm as CustomizationForm;
				if(customizationForm.customizationPropertyGrid1 == null) return;
				customizationForm.customizationPropertyGrid1.OwnerSelectionChanged(null, null);
			}
		}
		bool CanChangeLayoutMode() {
			if(Parent != null && !IsRoot) {
				if(Items == null) return true;
				foreach(BaseLayoutItem bli in Items) if(bli is LayoutItemContainer) return false;
				return true;
			} else return false;
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutGroupCaptionImageLocation"),
#endif
 DefaultValue(GroupElementLocation.Default)]
		[XtraSerializableProperty()]
		public GroupElementLocation CaptionImageLocation {
			get { return captionImageLocationCore; }
			set {
				if(captionImageLocationCore == value) return;
				captionImageLocationCore = value;
				shouldResetBorderInfoCore = true;
				ShouldUpdateConstraintsDoUpdate = true;
				ComplexUpdate(true, true);
			}
		}
		[Obsolete("Use HeaderButtonsLocation instead")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public GroupElementLocation ExpandButtonLocation {
			get { return buttonsLocation; }
			set {
				if(buttonsLocation == value) return;
				buttonsLocation = value;
				DoUpdateBorderInfo();
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object ImageList {
			get { return Owner == null ? null : Owner.Images; }
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutGroupCaptionImageIndex"),
#endif
 DefaultValue(-1)]
		[XtraSerializableProperty()]
		[Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(UITypeEditor)), ImageListAttribute("ImageList")]
		public int CaptionImageIndex {
			get { return captionImageIndexCore; }
			set {
				if(captionImageIndexCore == value) return;
				captionImageIndexCore = value;
				DoUpdateBorderInfo();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutGroupAllowBorderColorBlending"),
#endif
 DefaultValue(false)]
		[XtraSerializableProperty()]
		public bool AllowBorderColorBlending {
			get {
				return allowBorderColorBlending;
			}
			set {
				if(allowBorderColorBlending == value) return;
				allowBorderColorBlending = value;
				DoUpdateBorderInfo();
			}
		}
		protected Image GetOwnerImage() {
			if(CaptionImageIndex < 0 || Owner == null) return null;
			if(Owner.Images == null) return null;
			return ImageCollection.GetImageListImage(Owner.Images, CaptionImageIndex);
		}
		protected void DoUpdateBorderInfo() {
			shouldResetBorderInfoCore = true;
			ShouldUpdateConstraintsDoUpdate = true;
			ComplexUpdate(true, true);
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutGroupCaptionImage"),
#endif
 DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image CaptionImage {
			get {
				Image img = GetOwnerImage();
				return img != null ? img : captionImageCore;
			}
			set {
				captionImageCore = value;
				DoUpdateBorderInfo();
			}
		}
		XtraLayout.Utils.Padding captionImagePadding = new DevExpress.XtraLayout.Utils.Padding(1, 2, 1, 2);
		bool ShouldSerializeCaptionImagePadding() { return !IsDefaultCaptionImagePadding(); }
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("LayoutGroupCaptionImagePadding")]
#endif
		public XtraLayout.Utils.Padding CaptionImagePadding {
			get { return captionImagePadding; }
			set {
				if(CaptionImagePadding == value) return;
				captionImagePadding = value;
				DoUpdateBorderInfo();
			}
		}
		protected internal virtual bool IsDefaultCaptionImagePadding() {
			return CaptionImagePadding == new XtraLayout.Utils.Padding(1, 2, 1, 2);
		}
		Image backgroundImageCore = null;
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutGroupBackgroundImage"),
#endif
 DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image BackgroundImage {
			get {
				return backgroundImageCore;
			}
			set {
				backgroundImageCore = value;
				DoUpdateBorderInfo();
			}
		}
		bool backgroundImageVisibleCore = false;
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutGroupBackgroundImageVisible"),
#endif
 DefaultValue(false)]
		public bool BackgroundImageVisible {
			get {
				return backgroundImageVisibleCore;
			}
			set {
				if(backgroundImageVisibleCore == value) return;
				backgroundImageVisibleCore = value;
				DoUpdateBorderInfo();
			}
		}
		ImageLayout backgroundImageLayoutCore = ImageLayout.Stretch;
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutGroupBackgroundImageLayout"),
#endif
 DefaultValue(ImageLayout.Stretch)]
		public ImageLayout BackgroundImageLayout {
			get {
				return backgroundImageLayoutCore;
			}
			set {
				if(backgroundImageLayoutCore == value) return;
				backgroundImageLayoutCore = value;
				DoUpdateBorderInfo();
			}
		}
		Image contentImageCore = null;
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutGroupContentImage"),
#endif
 DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image ContentImage {
			get {
				return contentImageCore;
			}
			set {
				contentImageCore = value;
				DoUpdateBorderInfo();
			}
		}
		ContentAlignment contentImageAlignmentCore = ContentAlignment.BottomRight;
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutGroupContentImageAlignment"),
#endif
 DefaultValue(ContentAlignment.BottomRight)]
		public ContentAlignment ContentImageAlignment {
			get {
				return contentImageAlignmentCore;
			}
			set {
				if(contentImageAlignmentCore == value) return;
				contentImageAlignmentCore = value;
				DoUpdateBorderInfo();
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutGroupEnabled"),
#endif
 DefaultValue(true), Browsable(false)]
		public bool Enabled {
			get { return enabledCore; }
			set {
				if(enabledCore == value) return;
				enabledCore = value;
				InvalidateEnabledState();
				SetShouldUpdateViewInfo();
				shouldResetBorderInfoCore = true;
				ComplexUpdate(true, true);
			}
		}
		bool pageEnabledCore = true;
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutGroupPageEnabled"),
#endif
 DefaultValue(true), Browsable(false)]
		public bool PageEnabled {
			get { return pageEnabledCore; }
			set {
				if(pageEnabledCore == value) return;
				pageEnabledCore = value;
				SetShouldUpdateViewInfo();
				shouldResetBorderInfoCore = true;
				ComplexUpdate(true, true);
			}
		}
		protected internal override void RestoreChildren(BaseItemCollection items) {
			Items.Clear();
			if(Name == "") throw new LayoutControlInternalException("Incorrect name");
			foreach(BaseLayoutItem item in items) {
				if(item.ParentName == Name) {
					LayoutGroup group = item as LayoutGroup;
					if(group != null && group.TabbedGroupParentName != null && group.TabbedGroupParentName.Length != 0) {
						continue;
					}
					string oldName = item.Name;
					Items.Add(item);
					if(item.Name != oldName) {
						SafeRenameItem(item, oldName, item.Name);
					}
				}
			}
		}
		protected internal virtual LayoutItem CreateLayoutItem() { return new LayoutItem(null); }
		protected internal virtual LayoutGroup CreateLayoutGroup() { return new LayoutGroup(null); }
		protected internal virtual TabbedGroup CreateTabbedGroup() { return new TabbedGroup(null); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected internal virtual BaseLayoutItemHandler Handler {
			get {
				if(Parent == null) {
					if(handlerInternal == null) {
						if(Owner != null)
							handlerInternal = Owner.CreateRootHandler(this);
						else
							handlerInternal = new LayoutGroupHandlerWithTabHelper(this);
					}
				} else
					return Parent.Handler;
				return handlerInternal;
			}
		}
		[Browsable(false)]
		public virtual bool IsInTabbedGroup {
			get {
				return tabbedGroupParentCore != null;
			}
		}
		protected override ViewInfo.BaseLayoutItemViewInfo CreateViewInfo() {
			return new ViewInfo.LayoutGroupViewInfo(this);
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ViewInfo.LayoutGroupViewInfo ViewInfo {
			get { return base.ViewInfo as ViewInfo.LayoutGroupViewInfo; }
		}
		bool expandOnDoubleClickCore = false;
		[ RefreshProperties(RefreshProperties.All)]
		[XtraSerializableProperty()]
		[DefaultValue(false)]
		public virtual bool ExpandOnDoubleClick {
			get { return expandOnDoubleClickCore; }
			set { expandOnDoubleClickCore = value; }
		}
		int expanding = 0;
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutGroupExpanded"),
#endif
 XtraSerializableProperty()]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Browsable(true)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		public override bool Expanded {
			get { return expanded; }
			set {
				bool oldValue = expanded;
				LayoutGroupCancelEventArgs ea = new LayoutGroupCancelEventArgs(this, false);
				if(Owner != null) Owner.RaiseGroupExpandChanging(ea);
				if(ea.Cancel) return;
				using(new SafeBaseLayoutItemChanger(this)) {
					if(!IsUpdateLocked && Owner != null && Owner.FocusHelper != null && Owner.FocusHelper.SelectedComponent != null && Owner.FocusHelper.SelectedComponent != this && !value) {
						FillItemsHelper fih = new FillItemsHelper(Owner);
						Accept(fih);
						var focusedBli = Owner.FocusHelper.SelectedComponent as BaseLayoutItem;
						if(fih.Items.Contains(focusedBli)) Owner.FocusHelper.FocusElement(this, false);
					}
					expanding++;
					try {
						expanded = value;
						ViewInfo.BorderInfo.Expanded = expanded;
						ViewInfo.ResetHotTrackHelper();
						if(IsUpdateLocked) {
							if(Owner == null || !Owner.InitializationFinished)
								return;
							else {
								ResizeManager.ResetResizer(this);
								ShouldUpdateConstraintsDoUpdate = true;
							}
						} else {
							ShouldResize = true;
							ShouldUpdateConstraints = true;
							if(oldValue != expanded) ResizeManager.CompleteIfNeeded(this);
							ComplexUpdate();
						}
					} finally {
						expanding--;
						if(!value && ResizeManager.resizer != null)
							Resizer.UpdateConstraints();
						if(Owner != null) Owner.RaiseGroupExpandChanged(ea);
					}
				}
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutGroupDefaultLayoutType"),
#endif
 DefaultValue(LayoutType.Vertical)]
		[XtraSerializableProperty()]
		public LayoutType DefaultLayoutType {
			get { return defaultLayoutType; }
			set { defaultLayoutType = value; }
		}
		bool showTabPageCloseButtonCore = false;
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutGroupShowTabPageCloseButton"),
#endif
 DefaultValue(false)]
		[XtraSerializableProperty()]
		public bool ShowTabPageCloseButton {
			get { return showTabPageCloseButtonCore; }
			set {
				if(showTabPageCloseButtonCore == value) return;
				showTabPageCloseButtonCore = value;
				shouldResetBorderInfoCore = true;
				ShouldUpdateConstraintsDoUpdate = true;
				ComplexUpdate(true, true);
			}
		}
		bool allowHtmlStringInCaptionCore;
		[XtraSerializableProperty(), DefaultValue(false)]
		public virtual bool AllowHtmlStringInCaption {
			get { return allowHtmlStringInCaptionCore; }
			set {
				if(value == AllowHtmlStringInCaption) return;
				allowHtmlStringInCaptionCore = value;
				UpdateText();
			}
		}
		DefaultBoolean allowGlyphSkinningCore = DefaultBoolean.Default;
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutGroupAllowGlyphSkinning"),
#endif
 DefaultValue(DefaultBoolean.Default)]
		[XtraSerializableProperty()]
		public DefaultBoolean AllowGlyphSkinning {
			get { return allowGlyphSkinningCore; }
			set {
				if(allowGlyphSkinningCore == value) return;
				allowGlyphSkinningCore = value;
				shouldResetBorderInfoCore = true;
				ComplexUpdate(true, false);
			}
		}
		OptionsPrintGroup optionsPrintCore;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public virtual OptionsPrintGroup OptionsPrint {
			get {
				if(optionsPrintCore == null) optionsPrintCore = new OptionsPrintGroup(this);
				return optionsPrintCore;
			}
		}
		protected internal virtual bool GetAllowGlyphSkinning() {
			if(AllowGlyphSkinning != DefaultBoolean.Default)
				return AllowGlyphSkinning == DefaultBoolean.True;
			return (Owner != null) && Owner.OptionsView.AllowGlyphSkinning;
		}
		[Browsable(false)]
		public virtual BaseItemCollection SelectedItems {
			get { return selectedItems; }
		}
		protected internal bool allowChangeComponents = true;
		protected internal virtual void OnItems_Changed(object sender, CollectionChangeEventArgs e) {
			if(e != null) {
				ShouldArrangeTextSize = true;
				BaseLayoutItem item = ((BaseLayoutItem)e.Element);
				if(item == null) return;
				SetShouldUpdateViewInfo();
				switch(e.Action) {
					case CollectionChangeAction.Add:
					if(!isCloning) ResizerModifyTree(item);
					if(allowChangeComponents) ((ILayoutItemOwner)this).AddComponent(item);
					item.Parent = this;
					LayoutGroup lGroup = item as LayoutGroup;
					if(lGroup != null) lGroup.SetTabbedGroupParent(null);
					item.Owner = Owner;
					UpdateControlsBackColor(e.Element as BaseLayoutItem);
					break;
					case CollectionChangeAction.Remove:
					if(allowChangeComponents) ((ILayoutItemOwner)this).RemoveComponent(e.Element as BaseLayoutItem);
					ResizerMarkRemoved(item);
					if(item.Owner != null) {
						item.Owner.AppearanceController.SetDefaultAppearanceDirty(item);
						item.Owner.EnabledStateController.SetItemEnabledStateDirty(item);
					}
					item.Parent = null;
					break;
				}
				if(LayoutMode == Utils.LayoutMode.Flow) {
					SetCellFactors();
					UpdateFlowLayoutItems(true);
				}
				if(LayoutMode == Utils.LayoutMode.Table) {
					if(item.OptionsTableLayoutItem.RowIndex > OptionsTableLayoutGroup.RowCount)  item.OptionsTableLayoutItem.RowIndex = 0;
					if(item.OptionsTableLayoutItem.ColumnIndex > OptionsTableLayoutGroup.ColumnCount) item.OptionsTableLayoutItem.ColumnIndex = 0;
					if(item.OptionsTableLayoutItem.RowIndex + item.OptionsTableLayoutItem.RowSpan - 1 > OptionsTableLayoutGroup.RowCount) item.OptionsTableLayoutItem.RowSpan = 1;
					if(item.OptionsTableLayoutItem.ColumnIndex + item.OptionsTableLayoutItem.ColumnSpan - 1 > OptionsTableLayoutGroup.ColumnCount) item.OptionsTableLayoutItem.ColumnSpan = 1;
					UpdateTableLayoutCore();
				}
				if(Parent != null && Parent.LayoutMode == Utils.LayoutMode.Table) {
					Parent.ResetResizer();
				}
			}
		}
		#region FlowLayout
		FlowLayoutMap flowLayoutMap;
		protected virtual void UpdateFlowItems(bool force) {
			UpdateFlowLayoutItems(force);
		}
		internal virtual void UpdateFlowLayoutItems(bool force) {
			if(IsDisposing || Items == null || CellSize.Height == 0 || CellSize.Width == 0) return;
			SetCellFactors();
			if(flowLayoutMap != null && ((SubLabelIdentionsForFlowLayoutSize(Bounds.Size).Width) / CellSize.Width) == flowLayoutMap.MapWidth && !force) return;
			flowLayoutMap = new FlowLayoutMap((SubLabelIdentionsForFlowLayoutSize(Bounds.Size).Width) / CellSize.Width, CellSize.Width, CellSize.Height);
			UpdateFlowLayoutItemsCore();
			if(Owner != null && Owner is LayoutControl && (Owner as LayoutControl).layoutAdornerWindowHandler != null) {
				(Owner as LayoutControl).layoutAdornerWindowHandler.Invalidate();
			}
		}
		Size SubLabelIdentionsForFlowLayoutSize(Size size) {
			if(ParentTabbedGroup == null) return ViewInfo.SubLabelSizeIndentions(size);
			else return size;
		}
		void SetCellFactors() {
			if(Items == null || Items.Count == 0 || CellSize.Height != 0 || CellSize.Width != 0) return;
			CellSize = DefaultMinItemSize;
		}
		static internal Size GetItemCellSize(BaseLayoutItem item, Size cellSize) {
			return new Size(CalculateCellSize(item, LayoutType.Horizontal, cellSize.Width), CalculateCellSize(item, LayoutType.Vertical, cellSize.Height));
		}
		static internal int CalculateCellSize(BaseLayoutItem item, LayoutType layoutType, int cell) {
			if(layoutType == LayoutType.Horizontal) {
				int result = item.Width > 0 ? item.Width / cell : 5;
				if(item.MaxSize.Width < result * cell && item.MaxSize.Width != 0)
					result = item.MaxSize.Width / cell;
				if(item.MinSize.Width > result * cell && item.MinSize.Width != 0) {
					result = item.MinSize.Width / cell;
					if((item.MinSize.Width - result * cell) % cell > 0) result++;
				}
				if(result <= 0) return 1;
				return result;
			} else {
				int result = item.Height > 0 ? item.Height / cell : 1;
				if(item.MaxSize.Height < result * cell && item.MaxSize.Height != 0)
					result = item.MaxSize.Height / cell;
				if(item.MinSize.Height > result * cell && item.MinSize.Height != 0) {
					result = item.MinSize.Height / cell;
					if((item.MinSize.Height - result * cell) % cell > 0) result++;
				}
				if(result <= 0) return 1;
				return result;
			}
		}
		void UpdateFlowLayoutItemsCore() {
			flowLayoutMap.ClearMap();
			int xLocation = 0, yLocation = 0;
			foreach(BaseLayoutItem item in Items) {
				if(!item.Visible) continue;
				if(item.Visibility == LayoutVisibility.Never) continue;
				if(Owner != null) {
					if(Owner.EnableCustomizationMode) {
						if(item.Visibility == LayoutVisibility.OnlyInRuntime) continue;
					} else {
						if(item.Visibility == LayoutVisibility.OnlyInCustomization) continue;
					}
				}
				Size cellSize = GetItemCellSize(item, CellSize);
				Rectangle checkRectangle = new Rectangle(xLocation, yLocation, cellSize.Width * CellSize.Width, cellSize.Height * CellSize.Height);
				if(flowLayoutMap.CheckLayoutMap(ref checkRectangle, item.StartNewLine)) {
					item.SetBounds(checkRectangle);
					xLocation += item.Bounds.Width;
				} else {
					xLocation = 0;
					yLocation = CellSize.Height;
					checkRectangle = new Rectangle(xLocation, yLocation, cellSize.Width * CellSize.Width, cellSize.Height * CellSize.Height);
					int watchdog = 1000;
					while(!flowLayoutMap.CheckLayoutMap(ref checkRectangle, item.StartNewLine) && watchdog-- > 0) {
						yLocation += CellSize.Height;
						checkRectangle = new Rectangle(xLocation, yLocation, cellSize.Width * CellSize.Width, cellSize.Height * CellSize.Height);
					}
					item.SetBounds(checkRectangle);
					xLocation += item.Bounds.Width;
				}
			}
			ResizeManager.Resizer.UpdateConstraints();
		}
		#endregion
		#region TableLayout
		Size ClientSize { get {
			if(ParentTabbedGroup != null) return Size;
			if(ViewInfo != null) return this.ViewInfo.SubLabelSizeIndentions(Size);
			return Size;
		}
		}
		int freeHeightForPercent { get { return OptionsTableLayoutGroup.GetFreeHeightForPercent(ClientSize, tableLayoutManagerVertical.GroupForTable.Items); } }
		int freeWidthForPercent { get { return OptionsTableLayoutGroup.GetFreeWidthForPercent(ClientSize, tableLayoutManagerHorizontal.GroupForTable.Items); } }
		SizeF minSizeForPercent = SizeF.Empty;
		internal bool ShouldRecreateTableLayoutManager = false;
		internal Size sizeToSetTableManagerGroup { get { return tableLayoutManagerHorizontal.GroupForTable.ViewInfo.AddLabelIndentions(ClientSize); } }
		OptionsTableLayoutGroup optionsTableLayout;
		bool ShouldSerializeOptionsTableLayoutGroup() { if(LayoutMode == Utils.LayoutMode.Table) return true; return false; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public OptionsTableLayoutGroup OptionsTableLayoutGroup {
			get {
				if(optionsTableLayout == null) optionsTableLayout = new OptionsTableLayoutGroup(Owner, this);
				return optionsTableLayout;
			}
		}
		internal Resizer tableLayoutManagerHorizontal;																	  
		internal Resizer tableLayoutManagerVertical;
		protected virtual void UpdateTableLayout() {
			UpdateTableLayoutCore();
		}
		internal void UpdateTableLayoutCore() {
			if(IsInitializing || IsUpdateLocked || IsDisposing) return;
			if(OptionsTableLayoutGroup.ColumnCount == 0 || OptionsTableLayoutGroup.RowCount == 0) return;
			bool isFirstCalculateAfterRecreate = ShouldRecreateTableLayoutManager;
			if(ShouldRecreateTableLayoutManager || tableLayoutManagerHorizontal == null || tableLayoutManagerVertical == null
			 || tableLayoutManagerHorizontal.GroupForTable.ItemCount != OptionsTableLayoutGroup.ColumnCount || tableLayoutManagerVertical.GroupForTable.ItemCount != OptionsTableLayoutGroup.RowCount) {
				CreateTableManagers();
				ShouldRecreateTableLayoutManager = false;
			}
			SetMinMaxSizeForManagers();
			CorrectPercentRowsAndColumns();
			foreach(ColumnDefinition column in OptionsTableLayoutGroup.ColumnDefinitions.OrderBy<ColumnDefinition, SizeType>(q => q.SizeType)) {
				UpdateTableLayoutCore(column, isFirstCalculateAfterRecreate);
			}
			foreach(RowDefinition row in OptionsTableLayoutGroup.RowDefinitions.OrderBy<RowDefinition, SizeType>(q => q.SizeType)) {
				UpdateTableLayoutCore(row, isFirstCalculateAfterRecreate);
			}
			SetRowColumnSizeFromManagers();
			Resizer.UpdateConstraints();
			SetBoundsForTableItems();
		}
		private void SetMinMaxSizeForManagers() {
			tableLayoutManagerHorizontal.GroupForTable.SetPreferredSize(ClientSize);
			tableLayoutManagerHorizontal.GroupForTable.Size = ClientSize;
			tableLayoutManagerVertical.GroupForTable.SetPreferredSize(ClientSize);
			tableLayoutManagerVertical.GroupForTable.Size = ClientSize;
		}
		void UpdateTableLayoutCore(ColumnDefinition columnDefinition, bool isFirstCalculateAfterRecreateTableManagers) {
			if(tableLayoutManagerHorizontal != null) {
				LayoutControlItem lci = TableLayoutHelper.GetItemFromColumn(tableLayoutManagerHorizontal, columnDefinition, OptionsTableLayoutGroup);
				switch(columnDefinition.SizeType) {
					case SizeType.Absolute:
						lci.MinSize = new Size((int)columnDefinition.Width, lci.MinSize.Height);
						lci.MaxSize = new Size((int)columnDefinition.Width, lci.MaxSize.Height);
						break;
					case SizeType.AutoSize:
						int indexOfColumn = optionsTableLayout.ColumnDefinitions.IndexOf(columnDefinition);
						IEnumerable<BaseLayoutItem> itemsInColumn = Items.Where(
							e => e.OptionsTableLayoutItem.ColumnIndex <= indexOfColumn &&
							e.OptionsTableLayoutItem.ColumnIndex + e.OptionsTableLayoutItem.ColumnSpan - 1 >= indexOfColumn && e.Visible);
						if(itemsInColumn.Count() == 0) {
							lci.MinSize = new Size(OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength, 1);
							lci.MaxSize = new Size(OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength, 0);
							return;
						}
						Size minSize = GetMinSizeAutoSize(itemsInColumn, LayoutType.Horizontal);
						Size maxSize = GetMaxSizeAutoSize(itemsInColumn, LayoutType.Horizontal);
						lci.MinSize = minSize;
						lci.MaxSize = maxSize;
						if(isFirstCalculateAfterRecreateTableManagers) lci.Width = (int)columnDefinition.Width;
						break;
					case SizeType.Percent:
						int result = (int)Math.Round(columnDefinition.Percent / 100 * freeWidthForPercent, MidpointRounding.ToEven);
						lci.MaxSize = Size.Empty;
						lci.MinSize = new Size(minSizeForPercent.Width != 0 ? (int)Math.Round(minSizeForPercent.Width * columnDefinition.Percent,MidpointRounding.ToEven) : 1, 1);
						lci.Size = new Size(result, lci.Height);
						break;
				}
			}
		}
		void UpdateTableLayoutCore(RowDefinition rowDefinition, bool isFirstCalculateAfterRecreate) {
			if(tableLayoutManagerVertical != null) {
				LayoutControlItem lci = TableLayoutHelper.GetItemFromRow(tableLayoutManagerVertical, rowDefinition, OptionsTableLayoutGroup);
				switch(rowDefinition.SizeType) {
					case SizeType.Absolute:
						lci.MinSize = new Size(lci.MinSize.Width, (int)rowDefinition.Height);
						lci.MaxSize = new Size(lci.MaxSize.Width, (int)rowDefinition.Height);
						break;
					case SizeType.AutoSize:
						int indexOfRow = optionsTableLayout.RowDefinitions.IndexOf(rowDefinition);
						IEnumerable<BaseLayoutItem> itemsInRow = Items.Where(
						e => e.OptionsTableLayoutItem.RowIndex <= indexOfRow &&
						e.OptionsTableLayoutItem.RowIndex + e.OptionsTableLayoutItem.RowSpan - 1 >= indexOfRow && e.Visible);
						if(itemsInRow.Count() == 0) {
							lci.MinSize = new Size(1, OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength);
							lci.MaxSize = new Size(0, OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength);
							return;
						}
						Size minSize = GetMinSizeAutoSize(itemsInRow, LayoutType.Vertical);
						Size maxSize = GetMaxSizeAutoSize(itemsInRow, LayoutType.Vertical);
						lci.MinSize = minSize;
						lci.MaxSize = maxSize;
						if(isFirstCalculateAfterRecreate) lci.Height = (int)rowDefinition.Height;
						break;
					case SizeType.Percent:
						int result = (int)Math.Round(rowDefinition.Percent / 100 * freeHeightForPercent, MidpointRounding.ToEven);
						lci.MaxSize = Size.Empty;
						lci.MinSize = new Size(1, minSizeForPercent.Height != 0 ? (int)Math.Round(minSizeForPercent.Height * rowDefinition.Percent, MidpointRounding.ToEven) : 1);
						lci.Size = new Size(lci.Width, result);
						break;
				}
			}
		}
		void SetBoundsForTableItems() {
			foreach(BaseLayoutItem bli in Items) {
				FixRowColumnIndexIfNeed(bli);
				SetBoundsForTableItemCore(bli);
			}
		}
		void FixRowColumnIndexIfNeed(BaseLayoutItem bli) {
			int watchDog = 30;
			while((bli.OptionsTableLayoutItem.RowIndex + bli.OptionsTableLayoutItem.RowSpan > OptionsTableLayoutGroup.RowCount || bli.OptionsTableLayoutItem.ColumnIndex + bli.OptionsTableLayoutItem.ColumnSpan > OptionsTableLayoutGroup.ColumnCount) && watchDog-- > 0) {
				if(bli.OptionsTableLayoutItem.RowIndex + bli.OptionsTableLayoutItem.RowSpan > OptionsTableLayoutGroup.RowCount) {
					if(bli.OptionsTableLayoutItem.RowIndex > 0) bli.OptionsTableLayoutItem.SetRowColumnIndex(bli.OptionsTableLayoutItem.RowIndex - 1, bli.OptionsTableLayoutItem.ColumnIndex);
					else
						if(bli.OptionsTableLayoutItem.RowSpan > 1) bli.OptionsTableLayoutItem.SetRowColumnSpan(bli.OptionsTableLayoutItem.RowSpan - 1, bli.OptionsTableLayoutItem.ColumnSpan);
				}
				if(bli.OptionsTableLayoutItem.ColumnIndex + bli.OptionsTableLayoutItem.ColumnSpan > OptionsTableLayoutGroup.ColumnCount) {
					if(bli.OptionsTableLayoutItem.ColumnIndex > 0) bli.OptionsTableLayoutItem.SetRowColumnIndex(bli.OptionsTableLayoutItem.RowIndex, bli.OptionsTableLayoutItem.ColumnIndex - 1);
					else
						if(bli.OptionsTableLayoutItem.ColumnSpan > 1) bli.OptionsTableLayoutItem.SetRowColumnSpan(bli.OptionsTableLayoutItem.ColumnSpan, bli.OptionsTableLayoutItem.ColumnSpan - 1);
				}
			}
		}
		void SetBoundsForTableItemCore(BaseLayoutItem bli) {
			Rectangle rectToSet = GetTableItemBounds(bli.OptionsTableLayoutItem.RowIndex, bli.OptionsTableLayoutItem.ColumnIndex);
			if(bli.OptionsTableLayoutItem.RowSpan > 1) rectToSet.Height += GetTableItemBounds(bli.OptionsTableLayoutItem.RowIndex + bli.OptionsTableLayoutItem.RowSpan - 1, bli.OptionsTableLayoutItem.ColumnIndex).Bottom - rectToSet.Bottom;
			if(bli.OptionsTableLayoutItem.ColumnSpan > 1) rectToSet.Width += GetTableItemBounds(bli.OptionsTableLayoutItem.RowIndex, bli.OptionsTableLayoutItem.ColumnIndex + bli.OptionsTableLayoutItem.ColumnSpan - 1).Right - rectToSet.Right;
			if(bli is LayoutItemContainer && bli.Bounds != rectToSet) {
				ShouldResize = true;
			}
			bli.SetBounds(rectToSet);
		}
		void CreateTableManagers() {
			tableLayoutManagerHorizontal = TableLayoutHelper.CreateFakeTableResizer(OptionsTableLayoutGroup, ClientSize, LayoutType.Horizontal);
			tableLayoutManagerVertical = TableLayoutHelper.CreateFakeTableResizer(OptionsTableLayoutGroup, ClientSize, LayoutType.Vertical);
		}
		void CorrectPercentRowsAndColumns() {
			minSizeForPercent = SizeF.Empty;
			CorrectPercentColumns();
			CorrectPercentRows();
		}
		void CorrectPercentRows() {
			double sumPercent = OptionsTableLayoutGroup.RowDefinitions.Sum<RowDefinition>(e => e.Percent);
			for(int j = 0; j < OptionsTableLayoutGroup.RowDefinitions.Count; j++) {
				if(OptionsTableLayoutGroup.RowDefinitions[j].SizeType == SizeType.Percent) {
					if(Math.Round(sumPercent, MidpointRounding.ToEven) != 100) OptionsTableLayoutGroup.RowDefinitions[j].Percent = (100 / sumPercent) * OptionsTableLayoutGroup.RowDefinitions[j].Percent;
					if(OptionsTableLayoutGroup.RowDefinitions[j].Percent == 0 || double.IsNaN(OptionsTableLayoutGroup.RowDefinitions[j].Percent)) OptionsTableLayoutGroup.RowDefinitions[j].Percent = 1;
					SetHeightMinSizeFromItemsIfNeeded(j);
				}
			}
		}
		void CorrectPercentColumns() {
			double sumPercent = OptionsTableLayoutGroup.ColumnDefinitions.Sum<ColumnDefinition>(e => e.Percent);
			for(int i = 0; i < OptionsTableLayoutGroup.ColumnDefinitions.Count; i++) {
				if(OptionsTableLayoutGroup.ColumnDefinitions[i].SizeType == SizeType.Percent) {
					if(Math.Round(sumPercent, MidpointRounding.ToEven) != 100) OptionsTableLayoutGroup.ColumnDefinitions[i].Percent = (100 / sumPercent) * OptionsTableLayoutGroup.ColumnDefinitions[i].Percent;
					if(OptionsTableLayoutGroup.ColumnDefinitions[i].Percent == 0 || double.IsNaN(OptionsTableLayoutGroup.ColumnDefinitions[i].Percent)) OptionsTableLayoutGroup.ColumnDefinitions[i].Percent = 1;
					SetWidthMinSizeFromItemsIfNeeded(i);
				}
			}
		}
		void SetHeightMinSizeFromItemsIfNeeded(int j) {
			IEnumerable<BaseLayoutItem> itemsInRow = Items.Where(e => e.OptionsTableLayoutItem.RowIndex == j && e.OptionsTableLayoutItem.RowSpan == 1);
			if(itemsInRow.Count() == 0) return;
			Size minSize = GetMinSizeAutoSize(itemsInRow, LayoutType.Vertical);
			if((minSize.Height / OptionsTableLayoutGroup.RowDefinitions[j].Percent) > minSizeForPercent.Height) {
				minSizeForPercent.Height = (float)(minSize.Height / OptionsTableLayoutGroup.RowDefinitions[j].Percent);
			}
		}
		void SetWidthMinSizeFromItemsIfNeeded(int i) {
			IEnumerable<BaseLayoutItem> itemsInColumn = Items.Where(e => e.OptionsTableLayoutItem.ColumnIndex == i && e.OptionsTableLayoutItem.ColumnSpan == 1);
			if(itemsInColumn.Count() == 0) return; 
			Size minSize = GetMinSizeAutoSize(itemsInColumn, LayoutType.Horizontal);
			if((minSize.Width / OptionsTableLayoutGroup.ColumnDefinitions[i].Percent) > minSizeForPercent.Width) {
				minSizeForPercent.Width = (float)(minSize.Width / OptionsTableLayoutGroup.ColumnDefinitions[i].Percent);
			}
		}
		void SetRowColumnSizeFromManagers() {
			for(int i = 0; i < OptionsTableLayoutGroup.ColumnCount; i++) {
				OptionsTableLayoutGroup.ColumnDefinitions[i].SetWidthWithoutCorrection(tableLayoutManagerHorizontal.GroupForTable.Items[i].Width);
			}
			for(int i = 0; i < OptionsTableLayoutGroup.RowCount; i++) {
				OptionsTableLayoutGroup.RowDefinitions[i].SetHeightWithoutCorrection(tableLayoutManagerVertical.GroupForTable.Items[i].Height);
			}
		}
		Size GetMaxSizeAutoSize(IEnumerable<BaseLayoutItem> ItemsInRow, LayoutType layoutType) {
			Size result = new Size(10000, 10000);
			foreach(BaseLayoutItem bli in ItemsInRow) {
				if(layoutType == LayoutType.Horizontal) {
					if(result.Width > bli.MinSize.Width / bli.OptionsTableLayoutItem.ColumnSpan && bli.MaxSize.Width / bli.OptionsTableLayoutItem.ColumnSpan > 10) result.Width = bli.MaxSize.Width / bli.OptionsTableLayoutItem.ColumnSpan;
				} else {
					if(result.Height > bli.MinSize.Height / bli.OptionsTableLayoutItem.RowSpan && bli.MaxSize.Height / bli.OptionsTableLayoutItem.RowSpan > 10) result.Height = bli.MaxSize.Height / bli.OptionsTableLayoutItem.RowSpan;
				}
			}
			if(result == new Size(10000, 10000)) return Size.Empty;
			return result;
		}
		Size GetMinSizeAutoSize(IEnumerable<BaseLayoutItem> ItemsInRow, LayoutType layoutType) {
			Size result = Size.Empty;
			foreach(BaseLayoutItem bli in ItemsInRow) {
				if(layoutType == LayoutType.Horizontal) {
					if(result.Width < bli.MinSize.Width / bli.OptionsTableLayoutItem.ColumnSpan) result.Width = bli.MinSize.Width / bli.OptionsTableLayoutItem.ColumnSpan;
				} else {
					if(result.Height < bli.MinSize.Height / bli.OptionsTableLayoutItem.RowSpan) result.Height = bli.MinSize.Height / bli.OptionsTableLayoutItem.RowSpan;
				}
			}
			return result;
		}
		internal Rectangle GetTableItemBounds(int row, int col) {
			Rectangle result = Rectangle.Empty;
			result.Y = tableLayoutManagerVertical.GroupForTable[row].Y;
			result.Height = tableLayoutManagerVertical.GroupForTable[row].Height;
			result.X = tableLayoutManagerHorizontal.GroupForTable[col].X;
			result.Width = tableLayoutManagerHorizontal.GroupForTable[col].Width;
			return result;
		}
		void CheckRowColumnTable() {
			int checkItemsCount = -1;
			foreach(BaseLayoutItem item in Items) {
				SetRowColumnForItem(item, checkItemsCount);
				checkItemsCount++;
			}
		}
		void SetRowColumnForItem(BaseLayoutItem item, int checkItemsCount) {
			CheckColumnCount();
			while(!TrySetRowColumnForItemCore(item, checkItemsCount)) {
				OptionsTableLayoutGroup.RowDefinitions.InternalAdd(new RowDefinition(this));
			}
			for(int i = OptionsTableLayoutGroup.RowCount; i < item.OptionsTableLayoutItem.RowIndex + item.OptionsTableLayoutItem.RowSpan; i++) {
				OptionsTableLayoutGroup.RowDefinitions.InternalAdd(new RowDefinition(this));
			}
		}
		bool TrySetRowColumnForItemCore(BaseLayoutItem item, int checkItemsCount) {
			if(CanSetRowColumnForItem(item.OptionsTableLayoutItem.RowIndex, item.OptionsTableLayoutItem.ColumnIndex, item, Items.Count - 1)) {
				return true;
			}
			for(int j = 0; j < optionsTableLayout.RowCount; j++) {
				for(int i = 0; i < optionsTableLayout.ColumnCount; i++) {
					if(CanSetRowColumnForItem(j, i, item, checkItemsCount)) {
						item.OptionsTableLayoutItem.SetRowColumnIndex(j, i);
						return true;
					}
				}
			}
			return false;
		}
		bool CanSetRowColumnForItem(int row, int col, BaseLayoutItem item, int checkItemsCount) {
			if(col + item.OptionsTableLayoutItem.ColumnSpan - 1 >= optionsTableLayout.ColumnCount || row + item.OptionsTableLayoutItem.RowSpan - 1 >= optionsTableLayout.RowCount) {
				return false;
			}
			Rectangle rectItem = new Rectangle(col, row, item.OptionsTableLayoutItem.ColumnSpan, item.OptionsTableLayoutItem.RowSpan);
			for(int i = 0; i <= checkItemsCount; i++) {
				if(Items[i] == item) continue;
				Rectangle rectBli = Items[i].OptionsTableLayoutItem.GetRectangleFromRowColumn();
				if(rectBli.IntersectsWith(rectItem)) return false;
			}
			return true;
		}
		void CheckColumnCount() {
			int columnCount = Items.Count == 0 ? 0 : Items.Max(e => e.OptionsTableLayoutItem.ColumnSpan);
			if(OptionsTableLayoutGroup.ColumnCount < columnCount) {
				for(int i = OptionsTableLayoutGroup.ColumnCount; i < columnCount; i++) {
					OptionsTableLayoutGroup.ColumnDefinitions.InternalAdd(new ColumnDefinition(this));
				}
			}
		}
		internal Size GetTableMaxSize() {
			if(tableLayoutManagerHorizontal == null || tableLayoutManagerVertical == null) CreateTableManagers();
			Size result = tableLayoutManagerHorizontal.GroupForTable.MaxSize;
			result.Height = tableLayoutManagerVertical.GroupForTable.MaxSize.Height;
			return result;
		}
		internal Size GetTableMinSize() {
			if(tableLayoutManagerHorizontal == null || tableLayoutManagerVertical == null) CreateTableManagers();
			if(!Expanded) return Size.Empty;
			Size result = tableLayoutManagerHorizontal.GroupForTable.MinSize;
			result.Height = tableLayoutManagerVertical.GroupForTable.MinSize.Height;
			return result;
		}
		private void DisposeTableOptionsAndManagers() {
			tableLayoutManagerHorizontal = null;
			tableLayoutManagerVertical = null;
		}
		BaseLayoutItem GetTableItem(int row, int col) {
			foreach(BaseLayoutItem item in Items) {
				int beginRowIndex = item.OptionsTableLayoutItem.RowIndex;
				int endRowIndex = item.OptionsTableLayoutItem.RowIndex + item.OptionsTableLayoutItem.RowSpan - 1;
				int beginColumnIndex = item.OptionsTableLayoutItem.ColumnIndex;
				int endColumnIndex = item.OptionsTableLayoutItem.ColumnIndex + item.OptionsTableLayoutItem.ColumnSpan - 1;
				if(beginRowIndex <= row && endRowIndex >= row && beginColumnIndex <= col && endColumnIndex >= col) return item;
			}
			return null;
		}
		#endregion
		void UpdateControlsBackColor(BaseLayoutItem item) {
			LayoutControlItem cItem = item as LayoutControlItem;
			if(cItem != null && cItem.Control != null) {
				DevExpress.Utils.Drawing.BackgroundPaintHelper.PerformBackColorChanged(cItem.Control);
			}
			LayoutGroup group = item as LayoutGroup;
			if(group != null) {
				foreach(BaseLayoutItem childItem in group.Items)
					UpdateControlsBackColor(childItem);
			}
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("LayoutGroupItem")]
#endif
		public BaseLayoutItem this[int Index] { get { return Items[Index]; } }
		[Browsable(false)]
		public int Count { get { if(Items != null) return Items.Count; else return 0; } }
		protected internal LayoutGroup GetGroup(int index) { return Items.GetGroup(index); }
		protected internal LayoutItem GetItem(int index) { return Items.GetItem(index); }
		[Browsable(false)]
		protected internal int GroupCount { get { return Items.GroupCount; } }
		[Browsable(false)]																													  
		protected internal int ItemCount { get { return Items.ItemCount; } }
		[Browsable(false)]
		public virtual LayoutGroupItemCollection Items { get { return items; } }
		public override void Accept(BaseVisitor visitor) {
			if(visitor.StartVisit(this)) {
				base.Accept(visitor);
				IList list = visitor.ArrangeElements(this);
				if(list == null) list = Items;
				for(int i = 0; i < list.Count; i++)
					((BaseLayoutItem)list[i]).Accept(visitor);
			}
			visitor.EndVisit(this);
		}
		void SortItems() {
			LayoutControlWalker walker = new LayoutControlWalker(this);
			ArrayList arrayList = walker.ArrangeElements(new OptionsFocus(MoveFocusDirection.AcrossThenDown, false));
			IEnumerable<BaseLayoutItem> list = arrayList.Cast<BaseLayoutItem>();
			LayoutGroupItemCollection layoutGroupItemCollection = new LayoutGroupItemCollection();
			layoutGroupItemCollection.AddRange(list.ToArray<BaseLayoutItem>());
			items = layoutGroupItemCollection;
			items.Changed += OnItems_Changed;
		}
		protected internal BaseLayoutItem GetSelectedItem(int index) {
			if(index < SelectedItems.Count && index >= 0)
				return SelectedItems[index];
			else return null;
		}
		protected internal bool IsItemSelected(BaseLayoutItem item) { return SelectedItems.Contains(item); }
		protected internal virtual void AddItemToSelectedList(BaseLayoutItem item) {
			if(item != null) {
				if(item.Parent == this && !IsItemSelected(item)) {
					SelectedItems.Add(item);
				}
			}
		}
		public class ClearSelectionHelper : BaseVisitor {
			bool wasSelectedCore = false;
			public bool WasSelectionModified { get { return wasSelectedCore; } }
			public override void Visit(BaseLayoutItem item) {
				if(item.Selected) wasSelectedCore = true;
				item.Selected = false;
				LayoutGroup group = item as LayoutGroup;
				if(group != null && group.SelectedItems != null) group.SelectedItems.Clear();
			}
		}
		public virtual bool ClearSelection() {
			StartChangeSelection();
			ClearSelectionHelper helper = new ClearSelectionHelper();
			Accept(helper);
			SelectedItems.Clear();
			if(SelectedItems.Count > 0) {
				throw new LayoutControlInternalException("selected items list should be empty here");
			}
			if(helper.WasSelectionModified)
				EndChangeSelection();
			else
				CancelChangeSelection();
			return helper.WasSelectionModified;
		}
		[Browsable(false)]
		public bool CanGroupSelectedItems {
			get {
				if(SelectedItems.Count == 1) {
					return !(SelectedItems[0] is LayoutGroup) && LayoutMode != LayoutMode.Flow;
				} else return CanMergeItems(SelectedItems) && LayoutMode == LayoutMode.Regular;
			}
		}
		protected virtual void SetNewGroupDefaults(LayoutGroup group) { }
		protected internal virtual LayoutGroup GroupItems() {
			return GroupItems(new BaseItemCollection(Items, null));
		}
		protected internal virtual LayoutGroup GroupItems(BaseItemCollection items) {
			BeginChangeUpdate();
			BaseItemCollection itemsCol = new BaseItemCollection(items, null);
			LayoutGroup newGroup = CreateLayoutGroup();
			newGroup.SetBounds(itemsCol.ItemsBounds);
			Point offset = new Point(-newGroup.Bounds.X, -newGroup.Bounds.Y);
			allowChangeComponents = false;
			int intemsToGroupCount = itemsCol.Count;
			for(int i = 0; i < intemsToGroupCount; i++) {
				BaseLayoutItem currentItem = itemsCol[i];
				RemoveItem(this, currentItem);
				AddItem(newGroup, currentItem);
				currentItem.ChangeLocation(offset.X, LayoutType.Horizontal);
				currentItem.ChangeLocation(offset.Y, LayoutType.Vertical);
			}
			allowChangeComponents = true;
			SetNewGroupDefaults(newGroup);
			AddItem(this, newGroup);
			EndChangeUpdate();
			return newGroup;
		}
		protected internal LayoutGroup GetSelectedGroup() {
			foreach(BaseLayoutItem item in Items) {
				if(item.IsGroup) {
					LayoutGroup group = (LayoutGroup)item;
					if(group.Selected && group.Expanded)
						return group;
					else {
						group = group.GetSelectedGroup();
						if(group != null)
							return group;
					}
				}
			}
			return null;
		}
		public void UngroupSelected() {
			if(!CanUngroupSelectedGroup) return;
			LayoutGroup group = SelectedItems[0] as LayoutGroup;
			if(group != null) {
				group.Selected = false;
				group.UngroupItems();
			}
		}
		protected internal virtual void UngroupItemsCore(Size resizeSize) { }
		protected internal virtual void UngroupItems() {
			if(!this.Expanded) return;
			if(Parent == null) return;
			if(ParentTabbedGroup != null) return;
				if(Items.Count == 0) { Parent.Remove(this); return; }
			LayoutGroup group = Parent;
			try {
				group.BeginChangeUpdate();
				this.IsOwnerInvalidating = true;
				Size resizeSize = new Size(Width + (Width - ViewInfo.ClientArea.Width), Height + (Height - ViewInfo.ClientArea.Height));
				DevExpress.XtraLayout.Utils.Padding oldP = Padding, oldS = Spacing;
				this.Parent = null;
				Padding = oldP;
				Spacing = oldS;
				Resizer.SizeIt(resizeSize, this, true);
				UngroupItemsCore(resizeSize);
				Point offset = new Point(Bounds.X, Bounds.Y);
				int count = Items.Count;
				BaseLayoutItem currentItem = null;
				for(int i = 0; i < count; i++) {
					currentItem = Items[i];
					currentItem.IsOwnerInvalidating = true;
					currentItem.ChangeLocation(offset.X, LayoutType.Horizontal);
					currentItem.ChangeLocation(offset.Y, LayoutType.Vertical);
					if(group.LayoutMode == Utils.LayoutMode.Table) {
						currentItem.OptionsTableLayoutItem.SetRowColumnIndex(OptionsTableLayoutItem.RowIndex,OptionsTableLayoutItem.ColumnIndex);
					}
				}
				allowChangeComponents = false;
				for(int i = 0; i < count; i++) {
					currentItem = Items[0];
					group.Items.Add(currentItem);
					currentItem.IsOwnerInvalidating = false;
				}
			} finally {
				group.EndChangeUpdate();
				allowChangeComponents = true;
				this.IsOwnerInvalidating = false;
				this.Owner = null;
			}
		}
		public LayoutGroup CreateGroupForSelectedItems() {
			BeginChangeUpdate();
			LayoutGroup group = null;
			try {
				if(this.CanGroupSelectedItems) {
					BaseLayoutItem item = SelectedItems.FirstOrDefault();
					bool setTable = item != null && item.Parent != null && item.Parent.LayoutMode == Utils.LayoutMode.Table;
					group = GroupItems(new BaseItemCollection(SelectedItems, null));
					if(setTable) {
						group.OptionsTableLayoutItem.ColumnIndex = item.OptionsTableLayoutItem.ColumnIndex;
						group.OptionsTableLayoutItem.RowIndex = item.OptionsTableLayoutItem.RowIndex;
					}
					return group;
				}
				return null;
			} finally {
				EndChangeUpdate();
				if(group != null) group.Selected = true;
			}
		}
		protected void SetPreferredSize(Size size) {
			prefredSize = size;
		}
		protected override XtraLayout.Utils.Padding DefaultPaddings {
			get {
				if(Owner != null) {
					if(Owner.RootGroup == this)
						return GroupBordersVisible ? Owner.DefaultValues.RootGroupPadding : Owner.DefaultValues.RootGroupWithoutBordersPadding;
					else return GroupBordersVisible ? Owner.DefaultValues.GroupPadding : Owner.DefaultValues.GroupWithoutBordersPadding;
				}
				return new XtraLayout.Utils.Padding(0);
			}
		}
		protected override void NameChanged() {
			foreach(BaseLayoutItem item in Items) {
				item.ParentName = Name;
			}
		}
		protected virtual bool ShouldSerializePreferredSize() {
			return false;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override LayoutGroup Parent {
			get {
				if(ParentTabbedGroup != null)
					return ParentTabbedGroup.Parent;
				else
					return base.Parent;
			}
			set {
				if(ParentTabbedGroup != null) {
				} else
					base.Parent = value;
			}
		}
		[Browsable(false)]
		protected internal Size PreferredSize {
			get { return prefredSize; }
			set { SetPreferredSize(value); }
		}
		[Browsable(false)]
		protected internal bool PreferredSizeDirty {
			get { return prefredSizeDirty; }
			set { prefredSizeDirty = value; }
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("LayoutGroupSize")]
#endif
		public override Size Size {
			get {
				return base.Size;
			}
			set {
				if(IsUpdateLocked) {
					SetSizeWithoutCorrection(value);
					return;
				}
				BeginChangeUpdate();
				if(Parent == null)
					Resizer.SizeIt(value);
				else {
					Parent.ChangeItemSize(this, value);
				}
				if(this.Parent != null) Parent.ResetResizerProportions();
				EndChangeUpdate();
			}
		}
		protected bool ShouldSerializeIsExpandedButtonVisible {
			get { return false; }
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutGroupExpandButtonVisible"),
#endif
 DefaultValue(false)]
		[XtraSerializableProperty()]
		public bool ExpandButtonVisible {
			get { return isExpandButtonVisibleInternal; }
			set { isExpandButtonVisibleInternal = value; shouldResetBorderInfoCore = true; ComplexUpdate(); }
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutGroupExpandButtonMode"),
#endif
 DefaultValue(ExpandButtonMode.Normal)]
		[XtraSerializableProperty()]
		public ExpandButtonMode ExpandButtonMode {
			get { return expandButtonModeCore; }
			set { expandButtonModeCore = value; shouldResetBorderInfoCore = true; ComplexUpdate(); }
		}
		protected LayoutGroup FindTopMostGroupKillIntermidiateResizers(LayoutGroup group) {
			LayoutGroup result = group;
			int unloopCounter = 100;
			while(result.Parent != null && unloopCounter-- > 0) {
				LayoutGroup tempGroup = result;
				result = result.Parent;
				tempGroup.resizeManager = null;
			}
			return result;
		}
		public void RotateLayout() {
			ResizeManager.RotateLayout(this);
		}
		public void RotateLayout(bool processChildGroups) {
			ResizeManager.RotateLayout(this, processChildGroups);
		}
		public void FlipLayout() {
			ResizeManager.FlipLayout(this);
		}
		public void FlipLayout(bool processChildGroups) {
			ResizeManager.FlipLayout(this, processChildGroups);
		}
		protected internal void DenyResetResizer(LayoutGroup group) {
			if(group != null && group.ResizeManager != null)
				group.ResizeManager.AllowResetResizer = false;
		}
		protected internal ResizeManager ResizeManager {
			get {
				if(Parent != null) {
					LayoutGroup topMostGroup = FindTopMostGroupKillIntermidiateResizers(this);
					if(topMostGroup != null) {
						return topMostGroup.ResizeManager;
					} else throw new LayoutControlInternalException("Cant find topmost group");
				} else {
					if(resizeManager == null) {
						resizeManager = CreateResizeManager();
					}
					return resizeManager;
				}
			}
		}
		protected virtual ResizeManager CreateResizeManager() {
			return new ResizeManager(this);
		}
		protected internal Resizer Resizer {
			get {
				if(DisposingFlag) return null;
				return ResizeManager.Resizer;
			}
		}
		internal void ResetResizerProportions() {
			ResizeManager.ResetResizerProportions();
		}
		internal void ResizerMarkRemoved(BaseLayoutItem item) {
			ResetResizer();
		}
		internal void ResizerModifyTree(BaseLayoutItem item) {
			ResizeManager.ModifyTree(item, this);
		}
		internal void ResetResizer() {
			ResizeManager.ResetResizer(this);
		}
		protected internal virtual bool IsRoot { get { return Owner != null && Owner.RootGroup == this; } }
		protected internal void ChangeItemPosition(BaseLayoutItem item, Point newLocation) {
			ChangeItemPosition(item, newLocation, false);
		}
		protected void ChangeItemPosition(BaseLayoutItem item, Point newLocation, bool changeGroupSize) {
			if(item == null) return;
			if(newLocation.X < 0) newLocation.X = 0;
			if(newLocation.Y < 0) newLocation.Y = 0;
			BeginChangeUpdate(false);
			try {
				UpdateItemPosition(item, newLocation, LayoutType.Horizontal);
				UpdateItemPosition(item, newLocation, LayoutType.Vertical);
			}
			finally {
				EndChangeUpdate(false);
			}
		}
		protected internal void ChangeItemSize(BaseLayoutItem item, Size itemSize) {
			ChangeItemSize(item, itemSize, Size.Empty);
		}
		protected internal void ChangeItemSize(BaseLayoutItem item, Size itemSize, Size oldGroupSize) {
			ChangeItemSize(item, itemSize, oldGroupSize, false);
		}
		protected internal void ChangeItemSize(BaseLayoutItem item, Size itemSize, Size oldGroupSize, bool force) {
			if(item == null) return;
			BeginChangeInit();
			if(oldGroupSize != Size.Empty) Resizer.SizeIt(oldGroupSize, true);
			Resizer.SafeSetSize(item, itemSize);
			EndChangeInit();
		}
		protected override Size GetLabelSize() {
			Size textSize = TextSize;
			textSize.Width += 20;
			textSize.Width = Math.Max(textSize.Width, Items.ItemsBounds.Width);
			return textSize;
		}
		[Browsable(false)]
		public override Size MinSize {
			get {
				if(IsDisposing) return Size;
				Size itemsSize = Size.Empty;
				if(Count > 0 && Expanded) {
					try {
						itemsSize = Resizer.GetItemMinSize(this);
					} catch(LayoutControlInternalException) {
						itemsSize = DefaultMinItemSize;
						SetExceptionsThrown();
					}
				} else
					itemsSize = DefaultMinItemSize;
				Size labelIndentionsSize = ViewInfo.AddLabelIndentions(Size.Empty);
				if(Expanded) {
					if(Count == 0) itemsSize = labelIndentionsSize;
					if(itemsSize.Width == 0) itemsSize.Width++;
					if(itemsSize.Height == 0) itemsSize.Height++;
					return itemsSize;
				} else {
					if(TextLocation == Locations.Default || TextLocation == Locations.Top || TextLocation == Locations.Bottom) {
						return new Size(itemsSize.Width + labelIndentionsSize.Width, labelIndentionsSize.Height + 3);
					} else {
						return new Size(labelIndentionsSize.Width + 3, labelIndentionsSize.Height + itemsSize.Height);
					}
				}
			}
		}
		protected override void UpdateVisibility() {
			if(ParentTabbedGroup != null) {
				ParentTabbedGroup.ResetVisibleTabPages();
				if(!RequiredItemVisibility && ParentTabbedGroup.SelectedTabPage == this) ParentTabbedGroup.SelectSomePageNearThis(this);
			}
		}
		protected void SetExceptionsThrown() {
			if(Owner != null) Owner.ExceptionsThrown = true;
		}
		[Browsable(false)]
		public override Size MaxSize {
			get {
				if(IsDisposing) return Size.Empty;
				if(Expanded) {
					Size size = Size.Empty;
					size = ViewInfo.AddLabelIndentions(size);
					if(Count > 0) {
						Size minSize;
						Size maxSize;
						CalcContentConstraints(out minSize, out maxSize, Size);
						if(maxSize.Width == 0)
							size.Width = 0;
						else
							size.Width = Math.Max(minSize.Width, maxSize.Width);
						if(maxSize.Height == 0)
							size.Height = 0;
						else
							size.Height = Math.Max(minSize.Height, maxSize.Height);
					} else
						size = DefaultMaxSize;
					return size;
				} else {
					Size size = DefaultMaxSize;
					if(TextLocation == Locations.Default || TextLocation == Locations.Top || TextLocation == Locations.Bottom) {
						if(expanding > 0)
							size.Width = Size.Width;
						return new Size(size.Width, MinSize.Height);
					} else
						return new Size(MinSize.Width, size.Height);
				}
			}
		}
		bool isCalculatingConstraints = false;
		private void CalcContentConstraints(out Size minSize, out Size maxSize, Size currentSize) {
			if(isCalculatingConstraints) {
				minSize = DefaultMinItemSize;
				maxSize = currentSize;
				return;
			}
			try {
				isCalculatingConstraints = true;
				minSize = Resizer.GetItemMinSize(this);
				maxSize = Resizer.GetItemMaxSize(this);
			} catch(LayoutControlInternalException) {
				minSize = DefaultMinItemSize;
				maxSize = DefaultMaxItemSize;
			} finally {
				isCalculatingConstraints = false;
			}
		}
		public void Clear() {
			BeginUpdate();
			SelectedItems.Clear();
			ArrayList controls = new ArrayList();
			UpdateControlVisibilityHelper helper = new UpdateControlVisibilityHelper();
			Accept(helper);
			ArrayList list = new ArrayList(Items);
			foreach(BaseLayoutItem item in list) {
				Remove(item);
			}
			EndUpdate();
		}
		public virtual void Remove(BaseLayoutItem item) {
			if(this.Items.Contains(item)) {
				RemoveItem(item);
				item.UpdateChildren(false);
			}
		}
		public void RemoveAt(int index) {
			Remove(this[index]);
		}
		public LayoutItem AddItem() {
			return AddItem(String.Empty);
		}
		public LayoutItem AddItem(String text) {
			return AddItem(text, null, null, DefaultInsertType);
		}
		public LayoutItem AddItem(BaseLayoutItem baseItem, InsertType insertType) {
			return AddItem("", null, baseItem, insertType);
		}
		public LayoutItem AddItem(String text, BaseLayoutItem baseItem, InsertType insertType) {
			return AddItem(text, null, baseItem, insertType);
		}
		public LayoutItem AddItem(BaseLayoutItem newItem) {
			return AddItem("", newItem, null, DefaultInsertType);
		}
		public LayoutItem AddItem(BaseLayoutItem newItem, BaseLayoutItem baseItem, InsertType insertType) {
			return AddItem("", newItem, baseItem, insertType);
		}
		public LayoutItem AddItem(String text, BaseLayoutItem newItem, BaseLayoutItem baseItem, InsertType insertType) {
			LayoutType layoutType;
			InsertLocation insertLocation;
			InsertTypeToInsertLocationLayoutTypesConverter.Convert(insertType, out insertLocation, out layoutType);
			LayoutItem result;
			using(new SafeBaseLayoutItemChanger(this)) {
				result = Add(baseItem, insertLocation, layoutType, false, newItem) as LayoutItem;
				if(text.Length > 0) result.Text = text;
			}
			return result;
		}
		protected internal void AddItem(LayoutItem item) {
			Add(null, InsertLocation.After, DefaultLayoutType, false, item);
		}
		protected internal void AddItem(LayoutGroup group, BaseLayoutItem item) {
			if(group != null)
				group.Items.Add(item);
		}
		protected internal InsertType DefaultInsertType {
			get {
				return InsertLocationLayoutTypeToInsertTypeConverter.Convert(InsertLocation.After, DefaultLayoutType);
			}
		}
		public LayoutGroup AddGroup() {
			return AddGroup(String.Empty);
		}
		public LayoutGroup AddGroup(String text) {
			return AddGroup(text, null, null, DefaultInsertType);
		}
		public LayoutGroup AddGroup(BaseLayoutItem baseItem, InsertType insertType) {
			return AddGroup("", null, baseItem, insertType);
		}
		public LayoutGroup AddGroup(String text, BaseLayoutItem baseItem, InsertType insertType) {
			return AddGroup(text, null, baseItem, insertType);
		}
		public LayoutGroup AddGroup(LayoutGroup newGroup) {
			return AddGroup("", newGroup, null, DefaultInsertType);
		}
		public LayoutGroup AddGroup(LayoutGroup newGroup, BaseLayoutItem baseItem, InsertType insertType) {
			return AddGroup("", newGroup, baseItem, insertType);
		}
		protected LayoutGroup AddGroup(String text, LayoutGroup newGroup, BaseLayoutItem baseItem, InsertType insertType) {
			LayoutType layoutType;
			InsertLocation insertLocation;
			InsertTypeToInsertLocationLayoutTypesConverter.Convert(insertType, out insertLocation, out layoutType);
			LayoutGroup result = Add(baseItem, insertLocation, layoutType, true, newGroup) as LayoutGroup;
			if(text.Length > 0) result.Text = text;
			return result;
		}
		public TabbedGroup AddTabbedGroup() {
			return AddTabbedGroup(null, DefaultInsertType);
		}
		public TabbedGroup AddTabbedGroup(TabbedGroup newTabbedGroup) {
			return AddTabbedGroup(newTabbedGroup, null, DefaultInsertType);
		}
		public TabbedGroup AddTabbedGroup(BaseLayoutItem baseItem, InsertType insertType) {
			return AddTabbedGroup(null, baseItem, insertType);
		}
		public TabbedGroup AddTabbedGroup(TabbedGroup newTabbedGroup, BaseLayoutItem baseItem, InsertType insertType) {
			LayoutType layoutType;
			InsertLocation insertLocation;
			BeginChangeUpdate();
			InsertTypeToInsertLocationLayoutTypesConverter.Convert(insertType, out insertLocation, out layoutType);
			if(newTabbedGroup == null) {
				newTabbedGroup = CreateTabbedGroup(Rectangle.Empty);
			}
			TabbedGroup tgroup = (TabbedGroup)Add(baseItem, insertLocation, layoutType, false, newTabbedGroup);
			EndChangeUpdate();
			return tgroup;
		}
		protected internal BaseLayoutItem Add(bool isGroup) {
			return Add(DefaultLayoutType, isGroup);
		}
		protected internal BaseLayoutItem Add(LayoutType layoutType, bool isGroup) {
			return Add(null, InsertLocation.After, layoutType, isGroup, null);
		}
		protected internal BaseLayoutItem Add(BaseLayoutItem baseItem, InsertLocation insertLocation, bool isGroup) {
			return Add(baseItem, insertLocation, DefaultLayoutType, isGroup, null);
		}
		protected internal virtual BaseLayoutItem AddCore(BaseLayoutItem item) {
			return AddCore(item, false);
		}
		protected internal virtual BaseLayoutItem AddCore(BaseLayoutItem item, bool disableParentCheck) {
			return Add(null, InsertLocation.After, DefaultLayoutType, false, item, disableParentCheck);
		}
		public void Add(BaseLayoutItem item) {
			AddCore(item);
		}
		protected internal virtual BaseLayoutItem Add(BaseLayoutItem baseItem, InsertLocation insertLocation, LayoutType layoutType, bool isGroup, BaseLayoutItem item) {
			return Add(baseItem, insertLocation, layoutType, isGroup, item, false);
		}
		protected internal virtual BaseLayoutItem Add(BaseLayoutItem baseItem, InsertLocation insertLocation, LayoutType layoutType, bool isGroup, BaseLayoutItem item, bool disableParentCheck) {
			if(item == this) throw new Exception("Can not add an item into itself");
			if(LayoutMode == Utils.LayoutMode.Flow && item is LayoutItemContainer) return null;
			if(!disableParentCheck && item != null && Items.Contains(item) && item.Bounds != Rectangle.Empty && item.Bounds.Size != DefaultItemSize) return item;
			LayoutRectangle bounds = GetInsertedItemBounds(item, baseItem, insertLocation, layoutType);
			if(baseItem != null && item != null && baseItem == item) return null;
			BeginChangeUpdate();
			if(baseItem == null) {
				UpdateItemsAfterInsert(bounds);
			} else {
				LayoutRectangle baseItemLayoutRect = new LayoutRectangle(baseItem.Bounds, layoutType);
				if(baseItemLayoutRect.Width == 1) {
					baseItemLayoutRect.Width++;
					baseItem.Size = baseItemLayoutRect.Size;
				}
				int diff = baseItemLayoutRect.Width / 2;
				if(item is SplitterItem && bounds.Width > 0) diff = bounds.Width;
				if(diff == 0) throw new LayoutControlInternalException("can't insert item.");
				bounds.Width = diff;
				baseItemLayoutRect.Width -= diff;
				if(insertLocation == InsertLocation.After) {
					bounds.X -= diff;
				} else {
					baseItemLayoutRect.X += diff;
				}
				baseItem.SetBounds(baseItemLayoutRect);
			}
			BaseLayoutItem newItem = null;
			if(item == null) newItem = CreateLayoutItem(bounds, isGroup);
			else {
				newItem = item;
				newItem.Parent = this;
				newItem.SetBounds(bounds);
				newItem.Owner = Owner;
			}
			EndChangeUpdate();
			return newItem;
		}
		BaseLayoutItem CreateLayoutItem(LayoutRectangle bounds, bool isGroup) {
			BaseLayoutItem item;
			if(isGroup)
				item = CreateLayoutGroup();
			else item = CreateLayoutItem();
			item.SetBounds(bounds);
			item.MaxSize = DefaultMaxItemSize;
			Items.Add(item);
			((ILayoutItemOwner)this).AddComponent(item);
			item.Owner = Owner;
			return item;
		}
		protected virtual bool RemoveItem(LayoutGroup group, BaseLayoutItem item) {
			return RemoveItem(group, item, true);
		}
		protected virtual bool RemoveItem(LayoutGroup group, BaseLayoutItem item, bool shouldRemoveFromSelection) {
			if(shouldRemoveFromSelection) RemoveItemFromSelectedList(item);
			LayoutGroup itemGroup = item as LayoutGroup;
			bool result;
			if(itemGroup != null && itemGroup.ParentTabbedGroup != null) {
				result = itemGroup.ParentTabbedGroup.RemoveTabPage(itemGroup);
				item.Parent = null;
			} else {
				if(group.Items.Contains(item)) {
					item.Parent = null;
					group.Items.RaiseOnChanged(null);
					result = true;
				} else
					result = false;
			}
			return result;
		}
		protected internal void RemoveItemFromSelectedList(BaseLayoutItem item) {
			if(item != null) {
				if(item.Parent == this && IsItemSelected(item)) {
					SelectedItems.Remove(item);
				}
			}
		}
		bool RemoveItem(BaseLayoutItem item) {
			return RemoveItem(item, true);
		}
		bool RemoveItem(BaseLayoutItem item, bool shouldRemoveFromSelection) {
			if(Items.Count == 1 && !DisposingFlag) { MinSize = item.Size; }
			Size originalSize = new Size(this.Size.Width, this.Size.Height);
			if(item == null || item.Parent != this) return false;
			ResizeManager.VisibilityState = true;
			BeginChangeUpdate();
			try {
				RemoveItemProcessing(item, shouldRemoveFromSelection);
				return true;
			} finally {
				ResizeManager.VisibilityState = false;
				EndChangeUpdate();
			}
		}
		BaseItemCollection GetItemNeighbors(BaseLayoutItem item, LayoutType lt, InsertLocation il) {
			LayoutRectangle irect = new LayoutRectangle(item.Bounds, lt);
			BaseItemCollection col = new BaseItemCollection();
			LayoutRectangle trect;
			foreach(BaseLayoutItem bItem in Items) {
				trect = new LayoutRectangle(bItem.Bounds, lt);
				if(trect.Width <= irect.Width &&
					trect.X >= irect.X
					&& trect.Right <= irect.Right) {
					if(il == InsertLocation.Before) {
						if(trect.Bottom == irect.Y)
							col.Add(bItem);
					} else {
						if(irect.Bottom == trect.Y)
							col.Add(bItem);
					}
				}
			}
			if(irect.Width != new LayoutSize(col.ItemsBounds.Size, lt).Width) col.Clear();
			return col;
		}
		protected internal void RemoveItemProcessing(BaseLayoutItem item) {
			RemoveItemProcessing(item, true);
		}
		protected virtual bool IsStandartRemoveItemNeighborsInsertLocation { get { return true; } }
		protected internal void RemoveItemProcessing(BaseLayoutItem item, bool shouldRemoveFromSelection) {
			if(item != null) {
				bool needReduceGroupHeight = false;
				if(Owner != null && !Owner.OptionsView.FitControlsToDisplayAreaHeight) {
					needReduceGroupHeight = LayoutAlgorithmRemoveHelper.Process(item.Parent.Items, LayoutType.Horizontal, item);
				}
				RemoveItem(item.Parent, item, shouldRemoveFromSelection);
				if(LayoutMode == Utils.LayoutMode.Flow) return;
				LayoutType lt = LayoutType.Vertical;
				InsertLocation il = IsStandartRemoveItemNeighborsInsertLocation ? InsertLocation.After : InsertLocation.Before;
				BaseItemCollection items = GetItemNeighbors(item, lt, il);
				if(items.Count == 0) {
					lt = LayoutType.Horizontal;
					il = IsStandartRemoveItemNeighborsInsertLocation ? InsertLocation.After : InsertLocation.Before;
					items = GetItemNeighbors(item, lt, il);
				}
				if(items.Count == 0) {
					lt = LayoutType.Vertical;
					il = IsStandartRemoveItemNeighborsInsertLocation ? InsertLocation.Before : InsertLocation.After;
					items = GetItemNeighbors(item, lt, il);
				}
				if(items.Count == 0) {
					lt = LayoutType.Horizontal;
					il = IsStandartRemoveItemNeighborsInsertLocation ? InsertLocation.Before : InsertLocation.After;
					items = GetItemNeighbors(item, lt, il);
				}
				if(!needReduceGroupHeight) {
					if(items.Count == 0) return;
					ArrayList sizes = new ArrayList();
					foreach(BaseLayoutItem tItem in items) {
						sizes.Add(new Size(tItem.Width, tItem.Height));
						LayoutRectangle newBounds = new LayoutRectangle(tItem.Bounds, lt);
						LayoutSize ls = new LayoutSize(item.Size, lt);
						if(il == InsertLocation.After)
							newBounds.Y -= ls.Height;
						newBounds.Height += ls.Height;
						tItem.SetBounds(newBounds);
					}
				} else {
					if(ViewInfo == null) return;
					Size itemsBounds = Items.ItemsBounds.Size;
					Size newGroupSize = new Size(itemsBounds.Width + ViewInfo.Padding.Width, itemsBounds.Height + ViewInfo.Padding.Height);
					if(Parent != null && !Parent.IsDisposing) {
						Parent.ChangeItemSize(this, newGroupSize);
						Parent.ResetResizerProportions();
					} else Size = newGroupSize;
				}
			}
		}
		protected internal virtual bool InsertItem(LayoutItemDragController controller) {
			BaseLayoutItem item = controller.DragItem;
			BaseLayoutItem insertTo = controller.Item;
			if(item == null) return false;
			ResizerModifyTree(item);
			BeginChangeUpdate();
			if(insertTo != null) {
#if DEBUGTEST
				if(item.Tag == null) System.Diagnostics.Debug.Assert(insertTo.Parent == this || this.Items.Count == 0 || LayoutMode != Utils.LayoutMode.Regular, "InsertTo item parent should be equals to this");
				else {
					if(!item.Tag.Equals("LayoutGroupForRestore")) System.Diagnostics.Debug.Assert(insertTo.Parent == this || this.Items.Count == 0, "InsertTo item parent should be equals to this");
				}
#endif
				LayoutGroup InsertToGroup = insertTo as LayoutGroup;
				LayoutGroup InsertGroup = item as LayoutGroup;
				TabbedGroup InsertTabbedGroup = item as TabbedGroup;
				if(InsertToGroup != null && InsertToGroup.ParentTabbedGroup != null) {
					if(InsertGroup != null) {
						if(InsertToGroup.Items.Count == 0 && InsertToGroup.ParentTabbedGroup.ViewInfo.RealClientArea.Contains(controller.HitPoint)) {
							InsertToGroup.AddGroup(InsertGroup);
						} else {
							InsertToGroup.ParentTabbedGroup.InsertTabPage(InsertToGroup, InsertGroup, controller.InsertLocation);
						}
						InsertToGroup.ParentTabbedGroup.SelectedTabPage = InsertToGroup;
						EndChangeUpdate();
						return true;
					} else {
						if(InsertTabbedGroup != null) {
							InsertToGroup.AddTabbedGroup(InsertTabbedGroup);
							EndChangeUpdate();
							return true;
						}
						EndChangeUpdate();
						return false;
					}
				}
				if(controller.MoveType == MoveType.Inside) {
					if(InsertToGroup != null && InsertToGroup.LayoutMode == Utils.LayoutMode.Table && controller.HitInfo is LayoutGroupHitInfo && (controller.HitInfo as LayoutGroupHitInfo).AdditionalHitType == LayoutGroupHitTypes.TableDefinition) {
						if(!InsertToGroup.Items.Contains(item))InsertToGroup.Items.Add(item);
						if(item.OptionsTableLayoutItem.CanSetIndexAndSpan(InsertToGroup, (controller.HitInfo as LayoutGroupHitInfo).columnIndex + item.OptionsTableLayoutItem.ColumnSpan, LayoutType.Vertical) &&
						item.OptionsTableLayoutItem.CanSetIndexAndSpan(InsertToGroup, (controller.HitInfo as LayoutGroupHitInfo).rowIndex + item.OptionsTableLayoutItem.RowSpan, LayoutType.Horizontal)) {
							item.OptionsTableLayoutItem.ColumnIndex = (controller.HitInfo as LayoutGroupHitInfo).columnIndex;
							item.OptionsTableLayoutItem.RowIndex = (controller.HitInfo as LayoutGroupHitInfo).rowIndex;
						}
					} else {
						if(insertTo != null && insertTo.Parent != null && insertTo.Parent.LayoutMode == Utils.LayoutMode.Flow && items.Count != 0) {
							int insertIndex = insertTo.Parent.Items.IndexOf(controller.Item);
							if(controller.InsertLocation == InsertLocation.Before)
								Items.Insert(insertIndex, item);
							else {
								if(insertIndex == items.Count)
									Items.Add(item);
								else {
									Items.Insert(insertIndex + 1, item);
								}
							}
							item.Parent.UpdateFlowLayoutItemsCore();
						} else {
							LayoutRectangle left = new LayoutRectangle(insertTo.Bounds, controller.LayoutType);
							LayoutRectangle right = new LayoutRectangle(insertTo.Bounds, controller.LayoutType);
							int dif = left.Width;
							int newWidth = dif >> 1;
							left.Width = newWidth;
							right.X = left.X + left.Width;
							right.Width = dif - left.Width;
							if(newWidth == 0) left.Width = 1;
							if(items.Count == 0) {
								AddCore(item);
							} else {
								if(controller.InsertLocation == InsertLocation.After) {
									insertTo.SetBounds(left.Rectangle);
									item.SetBounds(right.Rectangle);
									Items.Add(item);
									Resizer.SafeSetSize(insertTo, insertTo.Size);
								} else {
									insertTo.SetBounds(right.Rectangle);
									item.SetBounds(left.Rectangle);
									Items.Add(item);
									Resizer.SafeSetSize(item, item.Size);
								}
							}
						}
					}
				} else {
					LayoutRectangle insertBounds = GetMovedOutsideItemRectangle(Size.Empty, controller);
					BaseItemCollection items = GetOutsideMovedItemsCollection(controller);
					if(items.Count == 0) {
						items.Add(insertTo);
						insertBounds = new LayoutRectangle(insertTo.Bounds, controller.LayoutType);
					}
					InsertOutside(controller, item, insertBounds, items);
				}
			} else {
				AddCore(item);
			}
			EndChangeUpdate();
			item.Owner = Owner;
			SetShouldUpdateViewInfo();
			return true;
		}
		protected virtual int GetInsertOutsideSizeDelta(LayoutItemDragController controller, BaseItemCollection items) {
			return 1;
		}
		protected internal virtual void InsertOutside(LayoutItemDragController controller, BaseLayoutItem item, LayoutRectangle insertBounds, BaseItemCollection items) {
			if(items.Contains(item)) items.Remove(item);
			ArrayList sizes = new ArrayList();
			int delta = GetInsertOutsideSizeDelta(controller, items);
			foreach(BaseLayoutItem bItem in items) {
				sizes.Add(bItem.Size);
				LayoutRectangle lrect = new LayoutRectangle(bItem.Bounds, controller.LayoutType);
				lrect.Width = lrect.Width - delta;
				if(controller.InsertLocation == InsertLocation.Before) {
					lrect.X = lrect.X + delta;
				}
				bItem.SetBounds(lrect.Rectangle);
			}
			if(controller.InsertLocation == InsertLocation.After) {
				insertBounds.X = insertBounds.Right - delta;
			}
			insertBounds.Width = delta;
			Size originalSize = item.Size;
			item.SetBounds(insertBounds.Rectangle);
			Items.Add(item);
			UpdateLayout();
			int i = 0;
			if(!IsUpdateLocked) {
				foreach(BaseLayoutItem bItem in items) {
					Resizer.SafeSetSize(bItem, (Size)sizes[i]);
					i++;
				}
				if(controller.ShouldRestoreOriginalSize)
					Resizer.SafeSetSize(item, originalSize);
			} else
				ShouldResize = true;
		}
		BaseItemCollection GetRightRow(LayoutType lt) {
			BaseItemCollection itemCollection = new BaseItemCollection(this);
			LayoutRectangle tRect = new LayoutRectangle(Items.ItemsBounds, lt);
			LayoutPoint lp = new LayoutPoint(new Point(tRect.Right - 1, 1), lt);
			BaseLayoutItem item = ViewInfo.GetItemAtPoint(new Point(lp.X, lp.Y));
			if(item == null) return itemCollection;
			LayoutRectangle iRect = new LayoutRectangle(item.Bounds, lt);
			itemCollection.Add(item);
			while(iRect.Bottom != tRect.Bottom) {
				lp = new LayoutPoint(new Point(tRect.Right - 1, iRect.Bottom), lt);
				if(lt == LayoutType.Horizontal)
					item = ViewInfo.GetItemAtPoint(new Point(lp.X - 1, lp.Y + 1));
				else
					item = ViewInfo.GetItemAtPoint(new Point(lp.X + 1, lp.Y));
				if(item != null) iRect = new LayoutRectangle(item.Bounds, lt);
				else return itemCollection;
				itemCollection.Add(item);
			}
			return itemCollection;
		}
		protected internal virtual BaseLayoutItem[] AddColumnRow(BaseLayoutItem item, InsertLocation insertLocation, LayoutType layoutType) {
			ResetResizer();
			BeginChangeUpdate();
			BaseLayoutItem[] items;
			if(item == null) {
				BaseItemCollection insertItems = GetRightRow(layoutType);
				BaseItemCollection newItems = new BaseItemCollection();
				items = insertItems.Count == 0 ? new BaseLayoutItem[1] : new BaseLayoutItem[insertItems.Count];
				if(insertItems.Count == 0) {
					items[0] = Add(null, insertLocation, layoutType, false, null);
					items[0].Text = "0";
				} else {
					LayoutSize defaultSize = new LayoutSize(this.DefaultItemSize, layoutType);
					int i = 0;
					foreach(BaseLayoutItem bItem in insertItems) {
						BaseLayoutItem newItem = CreateLayoutItem();
						newItem.MaxSize = bItem.MaxSize;
						newItems.Add(newItem);
						Items.Add(newItem);
						LayoutRectangle newBounds = new LayoutRectangle(bItem.Bounds, layoutType);
						newBounds.X = newBounds.Right;
						newBounds.Width = defaultSize.Width;
						newItem.SetBounds(newBounds.Rectangle);
						items[i] = newItem;
						newItem.Text = Items.IndexOf(newItem).ToString();
						i++;
						((ILayoutItemOwner)this).AddComponent(newItem);
					}
				}
			} else {
				BaseItemCollection itemCollection = new BaseItemCollection();
				GetColumnRowsWidthAndInsertedBounds(item, insertLocation, layoutType, itemCollection);
				items = itemCollection.Count == 0 ? new BaseLayoutItem[1] : new BaseLayoutItem[itemCollection.Count];
				int i = 0;
				foreach(BaseLayoutItem newItem in itemCollection) {
					if(newItem.Parent != null) {
						BaseLayoutItem nItem = CreateLayoutItem();
						InsertItem(new LayoutItemDragController(nItem, MoveType.Inside, insertLocation, layoutType));
						items[i] = nItem;
						i++;
					}
				}
			}
			SetSize(ViewInfo.AddLabelIndentions(Items.ItemsBounds.Size));
			if(PreferredSize != Size.Empty) Resizer.ExactResize(PreferredSize);
			EndChangeUpdate();
			return items;
		}
		protected internal bool CanMerge { get { return CanMergeItems(SelectedItems); } }
		protected internal bool CanSplit { get { return CanSplitItem(SelectedItems); } }
		[Browsable(false)]
		protected internal bool CanUngroupSelectedGroup {
			get {
				bool temp = CanSplitItem(SelectedItems);
				if(SelectedItems.Count == 0) return false;
				LayoutGroup group = SelectedItems[0] as LayoutGroup;
				if(temp && group != null) {
					if(group.ParentTabbedGroup != null) return false;
					if(group.Expanded) return true;
				}
				return false;
			}
		}
		protected internal bool CanSplitItem(BaseItemCollection lc) {
			if(lc.Count == 1) return true;
			else return false;
		}
		protected internal bool CanMergeItem(params BaseLayoutItem[] items) {
			return CanMergeItems(new BaseItemCollection(items));
		}
		protected virtual void CalculateSizing(BaseLayoutItemHitInfo hitInfo) {
			CalculateSizing(LayoutType.Horizontal, hitInfo);
			if(hitInfo.Item == null)
				CalculateSizing(LayoutType.Vertical, hitInfo);
		}
		protected virtual void CalculateSizing(LayoutType layoutType, BaseLayoutItemHitInfo hitInfo) {
			LayoutSize itemMaxSize, itemMinSize, itemSize;
			LayoutItemHitTest sizingType;
			hitInfo.SetItem(GetSizingItem(layoutType, hitInfo));
			if(hitInfo.Item != null) {
				itemMaxSize = new LayoutSize(hitInfo.Item.MaxSize, layoutType);
				itemMinSize = new LayoutSize(hitInfo.Item.MinSize, layoutType);
				itemSize = new LayoutSize(hitInfo.Item.Size, layoutType);
				if(layoutType == LayoutType.Horizontal) sizingType = LayoutItemHitTest.HSizing;
				else sizingType = LayoutItemHitTest.VSizing;
				if(itemMaxSize.Width != itemMinSize.Width || (Owner.OptionsView.DrawAdornerLayer == DefaultBoolean.True && !Owner.DesignMode)) hitInfo.SetHitTestType(sizingType);
				else hitInfo.SetItem(null);
			}
		}
		public void BestFit() {
			if(IsUpdateLocked || Resizer == null) return;
			BaseLayoutItem horizontalParent = Resizer.FindGroupForModifiedGroup(this, Resizer.resultH);
			if(horizontalParent is GroupResizeGroup) {
				horizontalParent.BestFitCore();
				Resizer.UpdateConstraints();
			}
			BaseLayoutItem verticalParent = Resizer.FindGroupForModifiedGroup(this, Resizer.resultV);
			if(verticalParent is GroupResizeGroup) {
				verticalParent.BestFitCore();
				Resizer.UpdateConstraints();
			}
			ShouldResize = true;
			if(Owner != null) Owner.Invalidate();
		}
		protected internal override BaseLayoutItem GetSizingItem(LayoutType layoutType, BaseLayoutItemHitInfo hitInfo) {
			if(Expanded) {
				foreach(BaseLayoutItem item in Items) {
					if(!item.ActualItemVisibility) continue;
					BaseLayoutItem sizingItem = item.GetSizingItem(layoutType, hitInfo);
					if(sizingItem != null) return sizingItem;
				}
			}
			return base.GetSizingItem(layoutType, hitInfo);
		}
		protected virtual bool IsInExpandedButtonArea(Point point) {
			return ViewInfo.BorderInfo.ButtonsPanelBounds.Contains(point);
		}
		protected internal override BaseLayoutItemHitInfo CalcHitInfo(Point hitPoint, bool calcForHandler) {
			return CalcHitInfo(hitPoint, true, calcForHandler);
		}
		protected internal BaseLayoutItemHitInfo CalcHitInfo(Point hitPoint, bool shouldCalculateSizing, bool calcForHandler) {
			bool fullGroupCalculation = AllowCustomizeChildren || (Owner != null && Owner.DesignMode);
			return CalcHitInfo(hitPoint, shouldCalculateSizing, calcForHandler, fullGroupCalculation);
		}
		protected internal BaseLayoutItemHitInfo CalcHitInfo(Point hitPoint, bool shouldCalculateSizing, bool calcForHandler, bool fullCalculation) {
			BaseLayoutItemHitInfo hitInfo = new LayoutGroupHitInfo(base.CalcHitInfo(hitPoint, calcForHandler), LayoutGroupHitTypes.None);
			if(hitInfo.HitType == LayoutItemHitTest.ControlsArea) {
				if(fullCalculation) {
					BaseLayoutItem item = hitInfo.Item;
					CalcTableHitInfo(hitPoint, hitInfo);
					if(shouldCalculateSizing) CalculateSizing(hitInfo);
					if((shouldCalculateSizing && hitInfo.Item == null) || !shouldCalculateSizing) {
						hitInfo.SetItem(item);
						for(int i = 0; i < Items.Count; i++) {
							if(!Items[i].ActualItemVisibility) continue;
							if(Items[i].ViewInfo.BoundsRelativeToControl.Contains(hitPoint)) {
								hitInfo = Items[i].CalcHitInfo(hitPoint, calcForHandler);
								break;
							}
						}
					}
				} else {
					hitInfo.SetHitTestType(LayoutItemHitTest.Item);
				}
			} else {
				CalcTableHitInfo(hitPoint, hitInfo);
				if(IsInExpandedButtonArea(hitPoint)) {
					hitInfo = new LayoutGroupHitInfo(hitInfo, LayoutGroupHitTypes.ExpandedButton);
				}
			}
			return hitInfo;
		}
		private void CalcTableHitInfo(Point hitPoint, BaseLayoutItemHitInfo hitInfo) {
			if(LayoutMode != Utils.LayoutMode.Table) return;
			Point offset = ViewInfo.ClientAreaRelativeToControl.Location;
			Point internalOffset = offset;
			for(int row = 0; row < OptionsTableLayoutGroup.RowCount; row++) {
				for(int column = 0; column < OptionsTableLayoutGroup.ColumnCount; column++) {
					Rectangle tableDefenitionRectangle = new Rectangle(internalOffset, new Size((int)OptionsTableLayoutGroup.ColumnDefinitions[column].realSize, (int)OptionsTableLayoutGroup.RowDefinitions[row].realSize));
					if(tableDefenitionRectangle.Contains(hitPoint) && !Items.Contains(row, column)) {
						hitInfo.SetHitTestType(LayoutItemHitTest.None);
						(hitInfo as LayoutGroupHitInfo).SetAdditionalHitType(LayoutGroupHitTypes.TableDefinition);
						(hitInfo as LayoutGroupHitInfo).SetRowColumnIndex(row, column);
						(hitInfo as LayoutGroupHitInfo).SetItem(this);
						break;
					}
					internalOffset.X += (int)OptionsTableLayoutGroup.ColumnDefinitions[column].realSize;
				}
				if((hitInfo as LayoutGroupHitInfo).AdditionalHitType == LayoutGroupHitTypes.TableDefinition) break;
				internalOffset.X = offset.X;
				internalOffset.Y += (int)OptionsTableLayoutGroup.RowDefinitions[row].realSize;
			}
		}
		bool CanLockItems(BaseItemCollection itemCollection) {
			if(itemCollection.Count > 1) return CanGroupSelectedItems;
			else return false;
		}
		bool CanUnlockItem(BaseItemCollection itemCollection) {
			bool temp = CanSplitItem(SelectedItems);
			if(temp && SelectedItems[0].IsGroup)
				if(((LayoutGroup)SelectedItems[0]).Expanded) return true;
			return false;
		}
		bool CanMergeItems(BaseItemCollection itemCollection) {
			if(itemCollection.Count < 2) return false;
			Rectangle sBounds = itemCollection.ItemsBounds;
			for(int i = 0; i < Count; i++) {
				if(!itemCollection.Contains(this[i]) && sBounds.IntersectsWith(this[i].Bounds))
					return false;
			}
			return Items.CanMergeItems(itemCollection);
		}
		public override bool Move(LayoutItemDragController controller) {
			if(!CanMove(controller)) return false;
			if(ParentTabbedGroup == null || CheckTabbedGroupOutsideDrag(controller)) {
				return base.Move(controller);
			} else {
				return ParentTabbedGroup.MoveTabPage(controller);
			}
		}
		bool CheckTabbedGroupOutsideDrag(LayoutItemDragController controller) {
			if(ParentTabbedGroup == null) return false;
			if(controller.MoveType == MoveType.Inside) return false;
			if(!(controller.HitInfo is TabbedGroupHitInfo)) return false;
			if((controller.HitInfo as TabbedGroupHitInfo).TabbedGroupHotPageIndex > 0) return false;
			return true;
		}
		protected BaseLayoutItem MoveItemOutsideOfItself(LayoutItemDragController controller) {
			BaseLayoutItem baseItem = controller.Item;
			BaseLayoutItem item = controller.DragItem;
			if(baseItem != null) {
				if(item == baseItem)
					if(controller.MoveType == MoveType.Outside) {
						BaseLayoutItem e1, e2;
						LayoutType lt = controller.LayoutType == LayoutType.Horizontal ? LayoutType.Vertical : LayoutType.Horizontal;
						e1 = baseItem.Parent.GetMovedOutsideNeighbor(baseItem, controller.InsertLocation, InsertLocation.Before, lt);
						e2 = baseItem.Parent.GetMovedOutsideNeighbor(baseItem, controller.InsertLocation, InsertLocation.After, lt);
						if(e1 != null) baseItem = e1;
						if(e2 != null) baseItem = e2;
						if(baseItem == item)
							return null;
						else
							return baseItem;
					}
			}
			return null;
		}
		protected bool ProcessRemovingItem(BaseLayoutItem item) {
			if(item.IsHidden)
				return true;
			else
				return RemoveItem(item);
		}
		protected internal bool MoveItem(LayoutItemDragController controller) {
			BaseLayoutItem baseItem = controller.Item;
			BaseLayoutItem item = controller.DragItem;
			BaseLayoutItem newMoveTo = MoveItemOutsideOfItself(controller);
			if(newMoveTo != null) { baseItem = newMoveTo; controller.SetItem(baseItem); }
			if(baseItem == null || baseItem.Parent == null) {
				if((item.IsHidden || (baseItem != null && item.Owner != baseItem.Owner)) && baseItem.Parent == null && baseItem is LayoutGroup) {
					((LayoutGroup)baseItem).AddCore(item);
					return true;
				} else {
					if(controller.HitInfo != null && controller.HitInfo is LayoutGroupHitInfo && (controller.HitInfo as LayoutGroupHitInfo).AdditionalHitType == LayoutGroupHitTypes.TableDefinition) {
						if(baseItem is LayoutGroup && (baseItem as LayoutGroup).LayoutMode == Utils.LayoutMode.Table) {
						   return (baseItem as LayoutGroup).InsertItem(controller);
						}
					}
					return false;
				}
			}
			try {
				if(item != null && item.Site != null) allowChangeComponents = false;
				StartChange();
				LayoutGroup oldParent = item.Parent;
				if(!item.IsHidden) {
					if(!RemoveItem(item, false)) return false;
				}
				LayoutGroup tempGroup = baseItem as LayoutGroup;
				if(tempGroup != null && controller.MoveType == MoveType.Inside && tempGroup.Items.Count == 0 && tempGroup.Expanded) {
					if(tempGroup.ParentTabbedGroup == null) {
						if(!tempGroup.InsertItem(controller)) {
							throw new LayoutControlInternalException("Move item error");
						}
					} else {
						if(item is LayoutGroup) {
							if(!tempGroup.InsertItem(controller)) {
								throw new LayoutControlInternalException("Move item error");
							}
						} else {
							controller.ShouldRestoreOriginalSize = (oldParent == baseItem.Parent);
							tempGroup.AddCore(item);
							return true;
						}
					}
				} else {
					if(baseItem is TabbedGroup) {
						TabbedGroupHitInfo tgHitInfo = controller.HitInfo as TabbedGroupHitInfo;
						if(controller.MoveType == MoveType.Inside && item.IsGroup && item.Expanded && (tgHitInfo == null ||
							(tgHitInfo.Item is TabbedGroup && ((TabbedGroup)tgHitInfo.Item).TabPages.Count == 0))) {
							((TabbedGroup)baseItem).AddTabPage((LayoutGroup)item);
							return true;
						}
						if(tgHitInfo != null) {
							if(tgHitInfo.TabbedGroupHotPageIndex >= 0 && item is LayoutGroup) {
								((TabbedGroup)baseItem).InsertTabPage(((TabbedGroup)baseItem).TabPages[tgHitInfo.TabbedGroupHotPageIndex], (LayoutGroup)item, tgHitInfo.TabbedGroupInsertLocation);
								return true;
							} else {
							}
						}
						if(!baseItem.Parent.InsertItem(controller)) {
							throw new LayoutControlInternalException("Move item error");
						}
					} else {
						controller.ShouldRestoreOriginalSize = oldParent == baseItem.Parent;
						if(tempGroup != null && tempGroup.Items.Count == 0 && item is LayoutItem && tempGroup.ParentTabbedGroup != null) {
							tempGroup.AddCore(item);
							return true;
						} else {
							if(!baseItem.Parent.InsertItem(controller)) {
								throw new LayoutControlInternalException("Move item error");
							}
						}
					}
				}
			} finally {
				ResetResizerProportions();
				ShouldArrangeTextSize = true;
				SetShouldUpdateViewInfo();
				Invalidate();
				EndChange();
				allowChangeComponents = true;
			}
			return true;
		}
		public bool CanMoveItem(LayoutItemDragController controller) {
			BaseLayoutItem item = controller.DragItem;
			BaseLayoutItem baseItem = controller.Item;
			if(item == null) return false;
			if(controller.MoveType == MoveType.Inside) {
				if(baseItem is LayoutGroup && (baseItem as LayoutGroup).Items.Count == 0) return true;
				if(baseItem is LayoutGroup && (baseItem as LayoutGroup).LayoutMode == Utils.LayoutMode.Table) return true;
				if(baseItem is TabbedGroup) return true;
				if(item is TabbedGroup) {
					TabbedGroup tGroup = item as TabbedGroup;
					if(tGroup.TabPages.Contains(baseItem)) return false;
				}
				return (item != baseItem) && item != GetItemNeighbor(baseItem, controller.InsertLocation, controller.LayoutType);
			} else {
				if(item == baseItem) {
					baseItem = GetCorrectedbaseItem(item, controller.InsertLocation, controller.LayoutType);
					if(baseItem == null)
						return false;
				}
				controller.HitInfo.SetItem(baseItem);
				LayoutRectangle newBounds = GetMovedOutsideItemRectangle(item.Size, controller);
				return newBounds.Rectangle != item.Bounds;
			}
		}
		BaseLayoutItem GetCorrectedbaseItem(BaseLayoutItem item, InsertLocation insertLocation, LayoutType layoutType) {
			LayoutPoint pt = insertLocation == InsertLocation.Before ? item.GetLayoutLocation(layoutType) : new LayoutPoint(new Point(item.Bounds.Right, item.Bounds.Bottom), layoutType);
			BaseItemCollection items = GetPositionSizeNeighbours(pt, insertLocation);
			if(items.Count > 0) {
				insertLocation = insertLocation == InsertLocation.After ? InsertLocation.Before : InsertLocation.After;
				return items[0];
			}
			LayoutRectangle itemBounds = item.GetLayoutBounds(layoutType);
			for(int i = 0; i < Count; i++) {
				if(this[i] == item) continue;
				LayoutRectangle lBounds = this[i].GetLayoutBounds(layoutType);
				if((itemBounds.Y == lBounds.Bottom || itemBounds.Bottom == lBounds.Y)
					&& ((insertLocation == InsertLocation.After && itemBounds.Right == lBounds.Right)
					|| (insertLocation == InsertLocation.Before) && itemBounds.Y == lBounds.Y))
					return this[i];
			}
			return null;
		}
		int CalculateDistance(BaseLayoutItem item, BaseLayoutItem testItem, LayoutType lt, Size rating) {
			LayoutRectangle irect = new LayoutRectangle(item.ViewInfo.BoundsRelativeToControl, LayoutGeometry.InvertLayout(lt));
			LayoutRectangle testrect = new LayoutRectangle(testItem.ViewInfo.BoundsRelativeToControl, LayoutGeometry.InvertLayout(lt));
			return Math.Abs((irect.X + (int)(irect.Width * ((float)rating.Height / 100))) - (testrect.X + (testrect.Width >> 1)));
		}
		protected class RatingListMember : IComparable {
			int rating;
			BaseLayoutItem item;
			public BaseLayoutItem Item {
				get { return item; }
			}
			public int Rating {
				get { return rating; }
			}
			public RatingListMember(int rating, BaseLayoutItem item) {
				this.item = item;
				this.rating = rating;
			}
			public int CompareTo(object obj) {
				RatingListMember tempRatingListMember = obj as RatingListMember;
				if(tempRatingListMember != null) {
					return this.Rating - tempRatingListMember.Rating;
				}
				throw new ArgumentException("object is not a RatingListMember");
			}
		}
		internal void ArrangeItemsByRating(BaseLayoutItem item, BaseItemCollection items, Size rating, InsertLocation il, LayoutType lt) {
			if(items.Count == 0) return;
			ArrayList ratingList = new ArrayList();
			foreach(BaseLayoutItem bItem in items) {
				int dist = CalculateDistance(item, bItem, lt, rating);
				ratingList.Add(new RatingListMember(dist, bItem));
			}
			ratingList.Sort();
			int maxDist = 3 * ((RatingListMember)ratingList[ratingList.Count - 1]).Rating >> 1;
			int maxAllowedDist = maxDist * rating.Width / 100;
			try {
				items.Clear();
				foreach(RatingListMember member in ratingList) {
					if(member.Rating <= maxAllowedDist)
						items.Add(member.Item);
					else {
						if(rating.Width != 0 && rating.Width != 100)
							return;
					}
				}
			} finally {
				if(items.Count == 0 && ratingList.Count != 0) items.Add(((RatingListMember)ratingList[0]).Item);
				if(items.Count == 0) throw new LayoutControlInternalException("items count cannot be 0");
			}
		}
		BaseItemCollection GetOutsideMovedItemsCollection(LayoutItemDragController controller) {
			if(controller.insertToItems != null) return controller.insertToItems;
			BaseItemCollection items = new BaseItemCollection(new BaseLayoutItem[] { controller.Item });
			int originalCount = items.Count;
			GetMovedOutsideNeighbors(controller.Item, controller.InsertLocation, InsertLocation.Before, controller.LayoutType, items);
			GetMovedOutsideNeighbors(controller.Item, controller.InsertLocation, InsertLocation.After, controller.LayoutType, items);
			ArrangeItemsByRating(controller.Item, items, controller.Rating, controller.InsertLocation, controller.LayoutType);
			int currientCount = items.Count;
			return items;
		}
		internal LayoutRectangle GetMovedOutsideItemRectangle(Size itemSize, LayoutItemDragController controller) {
			BaseItemCollection items = GetOutsideMovedItemsCollection(controller);
			LayoutRectangle lBounds = items.GetLayoutItemsBounds(controller.LayoutType);
			LayoutRectangle lMoveToBounds = controller.Item.GetLayoutBounds(controller.LayoutType);
			double tempWidth = (new LayoutSize(itemSize, controller.LayoutType)).Width;
			double widthDelta = 1 - items.Count * 0.1;
			tempWidth = tempWidth * (widthDelta > 0.2 ? widthDelta : 0.2);
			lBounds.Width = (int)tempWidth;
			lBounds.X = controller.InsertLocation == InsertLocation.Before ? lMoveToBounds.X : lMoveToBounds.Right;
			lBounds.X -= lBounds.Width / 2;
			return lBounds;
		}
		internal void GetMovedOutsideNeighbors(BaseLayoutItem item, InsertLocation insertLocation, InsertLocation movedInsertLocation, LayoutType layoutType, IList items) {
			LayoutType movedLayoutType = layoutType == LayoutType.Horizontal ? LayoutType.Vertical : LayoutType.Horizontal;
			int watchdog = Count;
			while(item != null && watchdog-- > 0) {
				item = GetMovedOutsideNeighbor(item, insertLocation, movedInsertLocation, movedLayoutType);
				if(item != null)
					items.Add(item);
			}
		}
		internal BaseLayoutItem GetMovedOutsideNeighbor(BaseLayoutItem item, InsertLocation insertLocation, InsertLocation movedInsertLocation, LayoutType layoutType) {
			LayoutRectangle lItemBounds = item.GetLayoutBounds(layoutType);
			if(item != null && !item.ActualItemVisibility && item.Parent != null && item.Parent.Resizer != null)
				lItemBounds = new LayoutRectangle(item.Parent.Resizer.GetHiddenItemRealBounds(item), layoutType);
			for(int i = 0; i < Count; i++) {
				if(item == this[i]) continue;
				LayoutRectangle lBounds = this[i].GetLayoutBounds(layoutType);
				if(!this[i].ActualItemVisibility) {
					if(item != null && item.Parent != null && item.Parent.Resizer != null)
						lBounds = new LayoutRectangle(item.Parent.Resizer.GetHiddenItemRealBounds(this[i]), layoutType);
				}
				if(((insertLocation == InsertLocation.Before && lBounds.Y == lItemBounds.Y)
					|| (insertLocation == InsertLocation.After && lBounds.Bottom == lItemBounds.Bottom))
					&& ((movedInsertLocation == InsertLocation.Before && lBounds.Right == lItemBounds.X)
					|| (movedInsertLocation == InsertLocation.After && lBounds.X == lItemBounds.Right)))
					return this[i];
			}
			return null;
		}
		BaseItemCollection GetPositionNeighbours(BaseLayoutItem item, LayoutType layoutType) {
			return GetPositionSizeNeighbours(item.GetLayoutLocation(layoutType), InsertLocation.Before);
		}
		BaseItemCollection GetPositionSizeNeighbours(LayoutPoint pt, InsertLocation insertLocation) {
			BaseItemCollection items = new BaseItemCollection();
			for(int i = 0; i < Count; i++) {
				LayoutRectangle lItemBounds = this[i].GetLayoutBounds(pt.LayoutType);
				if((insertLocation == InsertLocation.After && lItemBounds.X == pt.X)
					|| (insertLocation == InsertLocation.Before && lItemBounds.Right == pt.X))
					items.Add(this[i]);
			}
			return items;
		}
		BaseLayoutItem GetItemNeighbor(BaseLayoutItem item, InsertLocation insertLocation, LayoutType layoutType) {
			BaseItemCollection items = new BaseItemCollection();
			GetItemsNeighbor(item, insertLocation, layoutType, items);
			return items.Count == 1 ? items[0] : null;
		}
		void GetItemsNeighbor(BaseLayoutItem item, InsertLocation insertLocation, LayoutType layoutType, BaseItemCollection items) {
			LayoutRectangle itemBounds = item.GetLayoutBounds(layoutType);
			bool hasTop = false, hasBottom = false;
			for(int i = 0; i < Count; i++) {
				if(this[i] == item) continue;
				LayoutRectangle lBounds = this[i].GetLayoutBounds(layoutType);
				if((insertLocation == InsertLocation.Before && lBounds.Right == itemBounds.X)
					|| (insertLocation == InsertLocation.After && lBounds.X == itemBounds.Right)) {
					if(lBounds.Y >= itemBounds.Y && lBounds.Bottom <= itemBounds.Bottom)
						items.Add(this[i]);
					if(lBounds.Y == itemBounds.Y) hasTop = true;
					if(lBounds.Bottom == itemBounds.Bottom) hasBottom = true;
				}
			}
			if(!hasTop || !hasBottom)
				items.Clear();
		}
		public virtual TabbedGroup CreateTabbedGroupForSelectedGroup() {
			if(CanCreateTabbedGroupForSelectedGroup)
				return CreateTabbedGroupForGroup((LayoutGroup)SelectedItems[0]);
			else
				return null;
		}
		[Browsable(false)]
		public bool CanCreateTabbedGroupForSelectedGroup {
			get {
				if(SelectedItems.Count == 1 &&
					SelectedItems[0] is LayoutGroup &&
					SelectedItems[0].Parent != null &&
					((LayoutGroup)SelectedItems[0]).ParentTabbedGroup == null) return true;
				else return false;
			}
		}
		[Browsable(false)]
		public virtual bool CanChangeLayoutModeForChildSelectedGroup {
			get {
				if(SelectedItems.Count == 1 && SelectedItems[0] is LayoutGroup) {
					return (SelectedItems[0] as LayoutGroup).CanChangeLayoutMode();
				}
				return false;
			}
		}
		protected override internal bool CanCustomize {
			get {
				return ParentTabbedGroup != null ? ParentTabbedGroup.AllowCustomizeChildren && base.CanCustomize : base.CanCustomize;
			}
		}
		[Browsable(false)]
		protected internal bool CanAddTab {
			get {
				if(SelectedItems.Count == 1 && SelectedItems[0] is TabbedGroup) return true;
				else return false;
			}
		}
		[Browsable(false)]
		protected internal bool CanUngroupTabbedGroup {
			get {
				return CanAddTab;
			}
		}
		internal void UngroupTabbedGroup() {
			if(SelectedItems.Count > 0 && SelectedItems[0] is TabbedGroup) {
				TabbedGroup group = SelectedItems[0] as TabbedGroup;
				group.Ungroup();
			}
		}
		protected void EnsureExpanded(LayoutGroup item) {
			if(!item.Expanded) item.Expanded = true;
		}
		protected virtual TabbedGroup CreateTabbedGroupForGroup(LayoutGroup item) {
			BeginChangeUpdate();
			EnsureExpanded(item);
			TabbedGroup group = CreateTabbedGroup();
			if(item.Parent != null && item.Parent.LayoutMode == Utils.LayoutMode.Table) {
				group.OptionsTableLayoutItem.ColumnIndex = item.OptionsTableLayoutItem.ColumnIndex;
				group.OptionsTableLayoutItem.RowIndex = item.OptionsTableLayoutItem.RowIndex;
			}
			Items.Add(group);
			group.Owner = Owner;
			RemoveItem(this, item);
			group.SetBounds(item.Bounds);
			group.AddTabPage(item);
			group.Selected = true;
			EndChangeUpdate();
			return group;
		}
		protected virtual TabbedGroup CreateTabbedGroup(Rectangle bounds) {
			BeginChangeUpdate();
			TabbedGroup group = CreateTabbedGroup();
			Items.Add(group);
			group.Owner = Owner;
			group.SetBounds(bounds);
			EndChangeUpdate();
			return group;
		}
		protected internal override void UpdateAfterRestore() {
			Point leftTop = Items.ItemsBounds.Location;
			if(leftTop != Point.Empty) {
				BeginInit();
				foreach(BaseLayoutItem bitem in Items) {
					Point p = bitem.Location;
					p.Offset(-leftTop.X, -leftTop.Y);
					bitem.Location = p;
				}
				EndInit();
			}
			System.Diagnostics.Debug.Assert(Items.ItemsBounds.Location == Point.Empty);
			if(Parent == null && PreferredSize != Size.Empty)
				Resizer.SizeIt(PreferredSize);
			else
				Resizer.SizeIt(Size);
			SetViewInfoAndPainter(null, null);
			System.Diagnostics.Debug.Assert(Items.ItemsBounds.Size == ViewInfo.ClientArea.Size);
			ShouldArrangeTextSize = true;
			ShouldUpdateConstraintsDoUpdate = true;
			ShouldUpdateLookAndFeel = true;
			ResetResizer();
			Invalidate();
		}
		void UpdateItemsAfterInsert(LayoutRectangle lBounds) {
			UpdateItemsPositionAndSize(lBounds, false);
		}
		public override void Invalidate() {
			if(Parent == null && Owner == null && !IsDisposing)
				ViewInfo.Offset = Location;
			if(IsUpdateLocked) return;
			if(Owner != null && Owner.DisposingFlag) return;
			if(disposingFlagCore) return;
			if(LayoutMode == Utils.LayoutMode.Flow) UpdateFlowLayoutItems(true);
			if(LayoutMode == Utils.LayoutMode.Table) UpdateTableLayoutCore();
			base.Invalidate();
		}
		protected internal void UpdateLayout() {
			if(Parent == null) ChangeItemSize(this, Size, Size, true);
			else Parent.UpdateLayout();
		}
		protected virtual void UpdateChild(BaseLayoutItem item, bool visible) {
			if(item == null || item.IsDisposing) return;
			item.ViewInfo.Offset = new Point(
					ViewInfo.ClientAreaRelativeToControl.Location.X +
					item.Location.X,
					ViewInfo.ClientAreaRelativeToControl.Location.Y +
					item.Location.Y);
			item.UpdateChildren(visible && Expanded && ActualItemVisibility);
		}
		protected internal override void UpdateChildren(bool visible) {
			base.UpdateChildren(visible);
			foreach(BaseLayoutItem item in Items) {
				UpdateChild(item, visible);
			}
			if(LayoutMode == Utils.LayoutMode.Flow) UpdateFlowLayoutItems(true);
			if(LayoutMode == Utils.LayoutMode.Table) UpdateTableLayoutCore();
		}
		protected internal override void SetRTL(bool isRTL,bool updatePosition) {
			base.SetRTL(isRTL, updatePosition);
			foreach(BaseLayoutItem item in Items) {
				item.SetRTL(IsRTL, updatePosition);
			}
		}
		protected override void OnRTLChanged() {
			base.OnRTLChanged();
			if(LayoutMode == Utils.LayoutMode.Table) {
			   OptionsTableLayoutGroup.ColumnDefinitions.Reverse();
			}
		}
		void UpdateItemsPositionAndSize(LayoutRectangle lBounds, bool reduce) {
			LayoutSize lSize = new LayoutSize(Size, lBounds.LayoutType);
			for(int i = 0; i < Count; i++) {
				LayoutRectangle lItemBounds = this[i].GetLayoutBounds(lBounds.LayoutType);
				if(lItemBounds.Right > lBounds.X) {
					int difWidth = 0;
					if(!lItemBounds.IntersectsWith(lBounds)) {
						int itemRight = lItemBounds.Right;
						if(lItemBounds.Right + lBounds.Width > lSize.Width)
							itemRight = lSize.Width + lBounds.Width - (lSize.Width - lItemBounds.Right);
						difWidth = Math.Min(lBounds.Right, itemRight) - Math.Max(lBounds.X, lItemBounds.X);
						if(difWidth < 0) difWidth = 0;
					}
					int difLocation = lBounds.Width - difWidth;
					if(reduce) {
						difWidth = -difWidth;
						difLocation = -difLocation;
					}
					this[i].ChangeSize(difWidth, lBounds.LayoutType);
					this[i].ChangeLocation(difLocation, lBounds.LayoutType);
				}
			}
		}
		bool Intersects(int min1, int max1, int min2, int max2) {
			if(min1 < min2 && max1 < min2) return false;
			if(min2 < min1 && max2 <= min1) return false;
			return true;
		}
		void UpdateItemPosition(BaseLayoutItem item, Point newPosition, LayoutType layoutType) {
			BaseItemCollection items = GetPositionNeighbours(item, layoutType);
			if(items.Count > 0) {
				int dif = (new LayoutPoint(newPosition, layoutType)).X - item.GetLayoutLocation(layoutType).X;
				if(dif == 0) return;
				BaseLayoutItem resizeItem = items[0];
				foreach(BaseLayoutItem bli in items) {
					LayoutRectangle lr = new LayoutRectangle(item.Bounds, layoutType);
					LayoutRectangle blilr = new LayoutRectangle(bli.Bounds, layoutType);
					if(Intersects(lr.Y, lr.Bottom, blilr.Y, blilr.Bottom)) { resizeItem = bli; break; }
				}
				LayoutSize lSize = new LayoutSize(resizeItem.Size, layoutType);
				lSize.Width = lSize.Width + dif;
				Resizer.SafeSetSize(resizeItem, lSize.Size);
				ResetResizerProportions();
			}
		}
		protected override void TextChangedNotifyParent() {
			if(ParentTabbedGroup != null) ParentTabbedGroup.ProcessChildTextChanged();
			else base.TextChangedNotifyParent();
		}
		protected internal BaseLayoutItem Split(BaseLayoutItem item) {
			return Split(item, DefaultLayoutType);
		}
		protected void SetTextToControlDistance(int newDistance) {
			TextToControlDistanseSetter setter = new TextToControlDistanseSetter(newDistance);
			BeginUpdate();
			Accept(setter);
			EndUpdate();
		}
		protected internal BaseLayoutItem Split(BaseLayoutItem item, LayoutType layoutType) {
			return Split(item, InsertLocation.After, layoutType);
		}
		protected internal BaseLayoutItem Split(BaseLayoutItem item, InsertLocation insertLocation, LayoutType layoutType) {
			if(item == null) return null;
			BeginUpdate();
			BaseLayoutItem nItem = CreateLayoutItem();
			nItem.SetBounds(item.Bounds);
			Items.Add(nItem);
			if(layoutType == LayoutType.Horizontal) {
				int w = (item.Width) / 2;
				item.Width = item.Width - w;
				nItem.Width = w;
				nItem.X = item.Bounds.Right;
			} else {
				int h = (item.Height) / 2;
				item.Height = item.Height - h;
				nItem.Height = h;
				nItem.Y = item.Bounds.Bottom;
			}
			ResetResizer();
			EndUpdate();
			return nItem;
		}
		LayoutRectangle GetColumnRowsWidthAndInsertedBounds(BaseLayoutItem item, InsertLocation insertLocation, LayoutType layoutType, BaseItemCollection items) {
			LayoutRectangle insertedBounds = new LayoutRectangle(new Rectangle(Point.Empty, Items.ItemsBounds.Size), layoutType);
			LayoutRectangle itemBounds = item != null ? item.GetLayoutBounds(layoutType) : insertedBounds.Clone();
			insertedBounds.X = insertLocation == InsertLocation.Before ? itemBounds.X : itemBounds.Right;
			int top = insertedBounds.Bottom;
			int bottom = insertedBounds.Top;
			for(int i = 0; i < Count; i++) {
				LayoutRectangle lBounds = new LayoutRectangle(this[i].Bounds, layoutType);
				if((insertLocation == InsertLocation.Before && lBounds.Left == itemBounds.Left)
					|| (insertLocation == InsertLocation.After && lBounds.Right == itemBounds.Right)) {
					if(insertedBounds.Width > lBounds.Width) insertedBounds.Width = lBounds.Width;
					if(top > lBounds.Y) top = lBounds.Y;
					if(bottom < lBounds.Bottom) bottom = lBounds.Bottom;
					if(items != null)
						items.Add(this[i]);
				}
			}
			insertedBounds.Y = top;
			insertedBounds.Height = bottom - top;
			return insertedBounds;
		}
		protected BaseLayoutItem FindRightBottomItem() {
			int right = 0; int bottom = 0;
			for(int i = 0; i < Count; i++) {
				Rectangle r = this[i].Bounds;
				if(r.Right > right) right = r.Right;
				if(r.Bottom > bottom) bottom = r.Bottom;
			}
			return ViewInfo.GetItemAtPoint(new Point(right - 2, bottom - 2));
		}
		LayoutRectangle GetInsertedItemBounds(BaseLayoutItem item, BaseLayoutItem baseItem, InsertLocation insertLocation, LayoutType layoutType) {
			if(Count == 0 || (Count == 1 && Items[0] == item)) {
				Size size = ViewInfo.AddLabelIndentions(Size.Empty);
				return new LayoutRectangle(new Rectangle(Point.Empty, new Size(Math.Max(Width - size.Width, 1), Math.Max(Height - size.Height, 1))), layoutType);
			}
			Rectangle bounds = baseItem == null ? new Rectangle(Point.Empty, Items.ItemsBounds.Size) : baseItem.Bounds;
			LayoutRectangle lBounds = new LayoutRectangle(bounds, layoutType);
			if(insertLocation == InsertLocation.After)
				lBounds.X = lBounds.Right;
			lBounds.Width = (new LayoutSize(DefaultItemSize, layoutType)).Width;
			if(item is SplitterItem) lBounds.Width = ((SplitterItem)item).SplitterWidth;
			return lBounds;
		}
		protected override void XtraDeserializePadding(XtraEventArgs e) {
			if(Owner == null) base.XtraDeserializePadding(e);
			else {
				if(Name == "Root") {
					if(Owner.OptionsSerialization.RestoreRootGroupPadding) base.XtraDeserializePadding(e);
				} else if(Owner.OptionsSerialization.RestoreGroupPadding) base.XtraDeserializePadding(e);
			}
		}
		protected override void XtraDeserializeSpacing(XtraEventArgs e) {
			if(Owner == null) base.XtraDeserializeSpacing(e);
			else {
				if(Name == "Root") {
					if(Owner.OptionsSerialization.RestoreRootGroupSpacing) base.XtraDeserializeSpacing(e);
				} else if(Owner.OptionsSerialization.RestoreGroupSpacing) base.XtraDeserializeSpacing(e);
			}
		}
		#region IControlBoxButtonsPanelOwner Members
		bool IGroupBoxButtonsPanelOwner.IsRightToLeft { get { return this.IsRTL; } }
		#endregion
		#region IButtonsPanelOwner Members
		object IButtonsPanelOwner.ButtonBackgroundImages {
			get { return null; }
		}
		ButtonsPanelControlAppearance IButtonsPanelOwner.AppearanceButton {
			get { return new ButtonsPanelControlAppearance(this); }
		}
		bool IButtonsPanelOwner.Enabled {
			get { return Enabled; }
		}
		bool IButtonsPanelOwner.AllowHtmlDraw {
			get { return AllowHtmlStringInCaption; }
		}
		bool IButtonsPanelOwner.AllowGlyphSkinning {
			get { return AllowGlyphSkinning.ToBoolean(false); }
		}
		object IButtonsPanelOwner.Images {
			get { return null; }
		}
		bool IButtonsPanelOwner.IsSelected {
			get { return false; }
		}
		void IButtonsPanelOwner.Invalidate() {
		}
		ObjectPainter IButtonsPanelOwner.GetPainter() {
			if(Owner == null) return new GroupBoxButtonsPanelPainter();
			if(Owner.LookAndFeel.ActiveStyle == DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Skin)
				return new GroupBoxButtonsPanelSkinPainter(Owner.LookAndFeel);
			if(Owner.LookAndFeel.ActiveStyle == DevExpress.LookAndFeel.ActiveLookAndFeelStyle.WindowsXP)
				return new GroupBoxButtonsPanelWindowsXpPainter();
			if(Owner.LookAndFeel.ActiveStyle == DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Office2003)
				return new BaseButtonsPanelOffice2003Painter();
			return new GroupBoxButtonsPanelPainter();
		}
		#endregion
		#region IButtonPanelControlAppearanceOwner Members
		IButtonsPanelControlAppearanceProvider IButtonPanelControlAppearanceOwner.CreateAppearanceProvider() {
			var provider = new ButtonsPanelControlAppearanceProvider();
			return provider;
		}
		#endregion
		#region IAppearanceOwner Members
		bool IAppearanceOwner.IsLoading {
			get { return false; }
		}
		#endregion
		void OnButtonCollectionChanged(object sender, CollectionChangeEventArgs e) {
			if(ButtonsPanel == null) return;
			ButtonsPanel.Buttons.Merge(CustomHeaderButtons);
			OnChanged();
		}
		protected void OnChanged() {
			DoUpdateBorderInfo();
		}
		#region ButtonsPanelEvents
		static readonly object customButtonClick = new object();
		[ Category("Behavior")]
		public event BaseButtonEventHandler CustomButtonClick {
			add { this.Events.AddHandler(customButtonClick, value); }
			remove { this.Events.RemoveHandler(customButtonClick, value); }
		}
		static readonly object customButtonUnchecked = new object();
		[ Category("Behavior")]
		public event BaseButtonEventHandler CustomButtonUnchecked {
			add { this.Events.AddHandler(customButtonUnchecked, value); }
			remove { this.Events.RemoveHandler(customButtonUnchecked, value); }
		}
		static readonly object customButtonChecked = new object();
		[ Category("Behavior")]
		public event BaseButtonEventHandler CustomButtonChecked {
			add { this.Events.AddHandler(customButtonChecked, value); }
			remove { this.Events.RemoveHandler(customButtonChecked, value); }
		}
		void IGroupBoxButtonsPanelOwner.RaiseButtonsPanelButtonClick(BaseButtonEventArgs ea) {
			BaseButtonEventHandler handler = (BaseButtonEventHandler)Events[customButtonClick];
			if(handler != null)
				handler(this, ea);
		}
		void IGroupBoxButtonsPanelOwner.RaiseButtonsPanelButtonChecked(BaseButtonEventArgs ea) {
			BaseButtonEventHandler handler = (BaseButtonEventHandler)Events[customButtonChecked];
			if(handler != null)
				handler(this, ea);
		}
		void IGroupBoxButtonsPanelOwner.RaiseButtonsPanelButtonUnchecked(BaseButtonEventArgs ea) {
			BaseButtonEventHandler handler = (BaseButtonEventHandler)Events[customButtonUnchecked];
			if(handler != null)
				handler(this, ea);
		}
		void IGroupBoxButtonsPanelOwner.LayoutChanged() {
			if(viewInfoCore != null) {
				viewInfoCore.UpdateBorder();
				Invalidate();
			}
		}
		#endregion
		[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraEditors.Design.GroupBoxButtonCollectionEditor, " + AssemblyInfo.SRAssemblyDesign,
			typeof(System.Drawing.Design.UITypeEditor)), Category("Header Buttons"), Localizable(true)]
		public virtual BaseButtonCollection CustomHeaderButtons {
			get { return customHeaderButtonsCore; }
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Visible)]
		[ DefaultValue(GroupElementLocation.Default)]
		public GroupElementLocation HeaderButtonsLocation {
			get { return buttonsLocation; }
			set {
				if(buttonsLocation == value) return;
				buttonsLocation = value;
				DoUpdateBorderInfo();
			}
		}
		BaseButtonCollection customHeaderButtonsCore;
		protected internal virtual BaseButtonsPanel ButtonsPanel {
			get { return ViewInfo != null ? ViewInfo.BorderInfo != null ? ViewInfo.BorderInfo.ButtonsPanel : null : null; }
		}
	}
}
