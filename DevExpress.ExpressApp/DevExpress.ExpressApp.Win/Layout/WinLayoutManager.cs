#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Adapters;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraLayout.Helpers;
using DevExpress.XtraLayout.Localization;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.ExpressApp.Win.Layout {
	public class XafLayoutConstants {
		private int itemToItemVerticalDistance;
		private int itemToItemHorizontalDistance;
		private int itemToBorderVerticalDistance;
		private int itemToBorderHorizontalDistance;
		private int invisibleGroupVerticalDistance;
		private int itemToTabBorderDistance;
		public XafLayoutConstants() {
			Reset();
		}
		public void Reset() {
			itemToItemVerticalDistance = 4;
			itemToItemHorizontalDistance = 10;
			itemToBorderVerticalDistance = 10;
			itemToBorderHorizontalDistance = 10;
			invisibleGroupVerticalDistance = 10;
			itemToTabBorderDistance = 2;
		}
		public int ItemToItemVerticalDistance {
			get { return itemToItemVerticalDistance; }
			set { itemToItemVerticalDistance = value; }
		}
		public int ItemToItemHorizontalDistance {
			get { return itemToItemHorizontalDistance; }
			set { itemToItemHorizontalDistance = value; }
		}
		public int ItemToBorderVerticalDistance {
			get { return itemToBorderVerticalDistance; }
			set { itemToBorderVerticalDistance = value; }
		}
		public int ItemToBorderHorizontalDistance {
			get { return itemToBorderHorizontalDistance; }
			set { itemToBorderHorizontalDistance = value; }
		}
		public int InvisibleGroupVerticalDistance {
			get { return invisibleGroupVerticalDistance; }
			set { invisibleGroupVerticalDistance = value; }
		}
		public int ItemToTabBorderDistance {
			get { return itemToTabBorderDistance; }
			set { itemToTabBorderDistance = value; }
		}
	}
#if DebugTest
	public
#endif
	sealed class XafLayoutControlImplementor : LayoutControlImplementor {
		protected override LayoutStyleManager CreateLayoutStyleManager() {
			return new XafLayoutStyleManager(castedOwner);
		}
		public XafLayoutControlImplementor(ILayoutControlOwner owner) : base(owner) { }
		public override BaseLayoutItem CreateLayoutItem(LayoutGroup parent) {
			XafLayoutControlItem controlItem = new XafLayoutControlItem((LayoutControlGroup)parent);
			controlItem.ControlAlignment = ContentAlignment.MiddleCenter;
			return controlItem;
		}
		public override void RestoreDefaultLayout() {
			if(CanRestoreDefaultLayout) {
				this.isLayouLoading++;
				try {
					owner.RaiseOwnerEvent_DefaultLayoutLoading(owner, EventArgs.Empty);
					owner.RaiseOwnerEvent_DefaultLayoutLoaded(owner, EventArgs.Empty);
				}
				finally {
					this.isLayouLoading--;
					RaiseLayoutLoaded();
				}
			}
		}
		public override bool CanRestoreDefaultLayout {
			get { return true; }
		}
		public XafLayoutConstants XafLayoutConstants {
			get { return ((XafLayoutStyleManager)LayoutStyleManager).XafLayoutConstants; }
			set { ((XafLayoutStyleManager)LayoutStyleManager).XafLayoutConstants = value; }
		}
		public void SetWinMode(bool isWin) {
			if(!isWin) {
				InitializeHiddenItemsCore();
				RegisterFixedItemType(typeof(EmptySpaceItem));
			}
		}
	}
	public class WinLayoutItemAppearanceAdapter : IAppearanceFormat, IAppearanceVisibility {
		private ViewItemVisibility defaultVisibility;
		private DevExpress.Utils.AppearanceObject AppearanceObject {
			get {
				if(Item is LayoutControlItem) {
					return Item.AppearanceItemCaption;
				}
				if(Item is LayoutControlGroup) {
					return ((LayoutControlGroup)Item).AppearanceGroup;
				}
				return Item.PaintAppearanceItemCaption;
			}
		}
		public WinLayoutItemAppearanceAdapter(BaseLayoutItem item) {
			Guard.ArgumentNotNull(item, "item");
			this.Item = item;
			if(Item is LayoutControlGroup) {
				((LayoutControlGroup)item).AppearanceGroup.Options.UseBackColor = true;
				((LayoutControlGroup)item).AppearanceGroup.Options.UseFont = true;
			}
			defaultVisibility = Visibility;
		}
		public BaseLayoutItem Item { get; private set; }
		#region IAppearanceFormat Members
		public System.Drawing.Color BackColor {
			get { return AppearanceObject.BackColor; }
			set { AppearanceObject.BackColor = value; }
		}
		public System.Drawing.Color FontColor {
			get { return AppearanceObject.ForeColor; }
			set {
				AppearanceObject.ForeColor = value;
				if(Item is LayoutControlGroup) {
					((LayoutControlGroup)Item).AppearanceGroup.Options.UseForeColor = true;
				}
			}
		}
		public System.Drawing.FontStyle FontStyle {
			get { return AppearanceObject.Font.Style; }
			set { AppearanceObject.Font = new System.Drawing.Font(AppearanceObject.Font, value); }
		}
		public void ResetFontStyle() {
			if(!DevExpress.Utils.AppearanceObject.DefaultFont.Equals(AppearanceObject.Font)) {
				AppearanceObject.Font = DevExpress.Utils.AppearanceObject.DefaultFont; 
			}
		}
		public void ResetFontColor() {
			if(Item is LayoutControlGroup) {
				((LayoutControlGroup)Item).AppearanceGroup.Options.UseForeColor = false;
			}
			AppearanceObject.ForeColor = Color.Empty;
		}
		public void ResetBackColor() {
			AppearanceObject.BackColor = Color.Empty;
		}
		#endregion
		#region IAppearanceVisibility Members
		public ViewItemVisibility Visibility {
			get { return Item.Visibility == LayoutVisibility.Always ? ViewItemVisibility.Show : ViewItemVisibility.Hide; }
			set {
				if(Visibility != value) {
					WinLayoutManager.SetLayoutItemVisibility(Item, value);
				}
			}
		}
		public void ResetVisibility() {
			Visibility = defaultVisibility;
		}
		#endregion
		#region Obsolete 15.2
		[Obsolete("Use the WinLayoutItemAppearanceAdapter(BaseLayoutItem item) constructor instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public WinLayoutItemAppearanceAdapter(BaseLayoutItem item, WinLayoutManager manager) : this(item) { }
		#endregion
	}
	public class XafLayoutControlGroup : LayoutControlGroup {
		private IModelLayoutGroup model;
		public IModelLayoutGroup Model {
			get { return model; }
			set { model = value; }
		}
	}
	public class WinLayoutManager : LayoutManager, IHtmlFormattingSupport, ISupportToolTip {
		private const int defaultBoundWidth = 750;
		private const int defaultBoundHeight = 750;
		private const string TempModelNodeIdSuffix = "_TempModelNode";
		private ViewItemsCollection viewItems;
		protected XafLayoutControl layoutControl;
		private bool isHtmlFormattingEnabled;
		private HashSet<Control> usedControls;
		private Dictionary<LayoutControlItem, ViewItem> viewItemByLayoutControlItem;
		private LayoutManager splitLayoutManager;
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetLayoutItemVisibility(BaseLayoutItem layoutItem, ViewItemVisibility viewItemVisibility) {
			Guard.ArgumentNotNull(layoutItem, "layoutItem");
			LayoutVisibility layoutVisibility;
			if(layoutItem is LayoutControlItem) {
				layoutVisibility = viewItemVisibility == ViewItemVisibility.Hide ? LayoutVisibility.OnlyInCustomization : LayoutVisibility.Always;
				((LayoutControlItem)layoutItem).ContentVisible = viewItemVisibility != ViewItemVisibility.ShowEmptySpace;
			}
			else {
				layoutVisibility = viewItemVisibility == ViewItemVisibility.Show ? LayoutVisibility.Always : LayoutVisibility.OnlyInCustomization;
			}
			SetLayoutItemVisibility(layoutItem, layoutVisibility);
		}
		private static void SetLayoutItemVisibility(BaseLayoutItem layoutItem, LayoutVisibility visibility) {
			layoutItem.Visibility = visibility;
			LayoutGroup group = layoutItem.Parent;
			TabbedGroup tabbedGroup = layoutItem is LayoutGroup ? ((LayoutGroup)layoutItem).ParentTabbedGroup : null;
			if(IsVisible(layoutItem)) {
				if(tabbedGroup != null && !IsVisible(tabbedGroup)) {
					tabbedGroup.Visibility = visibility;
				}
				ShowGroup(group, visibility);
			}
			else {
				if(tabbedGroup != null && !ContainsVisibleItems(tabbedGroup.TabPages)) {
					tabbedGroup.Visibility = visibility;
				}
				HideGroup(group, visibility);
			}
		}
		private static void ShowGroup(LayoutGroup group, LayoutVisibility visibility) {
			while(group != null && !IsVisible(group)) {
				group.Visibility = visibility;
				TabbedGroup tabbedGroup = group.ParentTabbedGroup;
				if(tabbedGroup != null && !IsVisible(tabbedGroup)) {
					tabbedGroup.Visibility = visibility;
				}
				group = group.Parent;
			}
		}
		private static void HideGroup(LayoutGroup group, LayoutVisibility visibility) {
			while(group != null && !ContainsVisibleItems(group.Items)) {
				group.Visibility = visibility;
				TabbedGroup tabbedGroup = group.ParentTabbedGroup;
				if(tabbedGroup != null && !ContainsVisibleItems(tabbedGroup.TabPages)) {
					tabbedGroup.Visibility = visibility;
				}
				group = group.Parent;
			}
		}
		private static bool ContainsVisibleItems(BaseItemCollection items) {
			foreach(BaseLayoutItem item in items) {
				if(!(item is EmptySpaceItem) && IsVisible(item)) {
					return true;
				}
			}
			return false;
		}
		private static bool IsVisible(BaseLayoutItem layoutItem) {
			return layoutItem.Visibility == LayoutVisibility.Always || layoutItem.Visibility == LayoutVisibility.OnlyInRuntime;
		}
		protected override void InitializeLayoutOptions(IModelViewLayout layoutModel) {
			base.InitializeLayoutOptions(layoutModel);
			IModelWinLayoutManagerOptions layoutOptions = layoutModel.Parent as IModelWinLayoutManagerOptions;
			if(layoutOptions != null) {
				CustomizationEnabled = layoutOptions.CustomizationEnabled;
				XafLayoutConstants xafLayoutConstants = new XafLayoutConstants();
				((ILayoutControl)layoutControl).DefaultValues = new LayoutControlDefaultsPropertyBag(true);
				xafLayoutConstants.ItemToItemVerticalDistance = layoutOptions.ItemToItemVerticalDistance;
				xafLayoutConstants.ItemToItemHorizontalDistance = layoutOptions.ItemToItemHorizontalDistance;
				xafLayoutConstants.ItemToBorderVerticalDistance = layoutOptions.ItemToBorderVerticalDistance;
				xafLayoutConstants.ItemToBorderHorizontalDistance = layoutOptions.ItemToBorderHorizontalDistance;
				xafLayoutConstants.InvisibleGroupVerticalDistance = layoutOptions.InvisibleGroupVerticalDistance;
				xafLayoutConstants.ItemToTabBorderDistance = layoutOptions.ItemToTabBorderDistance;
				((ILayoutControl)layoutControl).DefaultValues.TextToControlDistance = new DevExpress.XtraLayout.Utils.Padding(layoutOptions.TextToControlDistance);
				layoutControl.XafLayoutConstants = xafLayoutConstants;
			}
		}
		private void SetupBaseLayoutItem(BaseLayoutItem layoutItem, IModelLayoutElementWithCaptionOptions winLayoutElementModel) {
			if(winLayoutElementModel != null) {
				layoutItem.TextLocation = winLayoutElementModel.CaptionLocation;
				layoutItem.AppearanceItemCaption.TextOptions.WordWrap = winLayoutElementModel.CaptionWordWrap; 
				layoutItem.AppearanceItemCaption.TextOptions.HAlignment = winLayoutElementModel.CaptionHorizontalAlignment;
				layoutItem.AppearanceItemCaption.TextOptions.VAlignment = winLayoutElementModel.CaptionVerticalAlignment;
			}
		}
		private ContentAlignment GetControlAlignment(ISupportControlAlignment controlAlignment) {
			ContentAlignment result = ContentAlignment.TopLeft;
			if(controlAlignment != null) {
				switch(controlAlignment.VerticalAlign) {
					case StaticVerticalAlign.NotSet:
					case StaticVerticalAlign.Top:
						switch(controlAlignment.HorizontalAlign) {
							case StaticHorizontalAlign.NotSet:
							case StaticHorizontalAlign.Left:
								result = ContentAlignment.TopLeft;
								break;
							case StaticHorizontalAlign.Center:
								result = ContentAlignment.TopCenter;
								break;
							case StaticHorizontalAlign.Right:
								result = ContentAlignment.TopRight;
								break;
						}
						break;
					case StaticVerticalAlign.Bottom:
						switch(controlAlignment.HorizontalAlign) {
							case StaticHorizontalAlign.NotSet:
							case StaticHorizontalAlign.Left:
								result = ContentAlignment.BottomLeft;
								break;
							case StaticHorizontalAlign.Center:
								result = ContentAlignment.BottomCenter;
								break;
							case StaticHorizontalAlign.Right:
								result = ContentAlignment.BottomRight;
								break;
						}
						break;
					case StaticVerticalAlign.Middle:
						switch(controlAlignment.HorizontalAlign) {
							case StaticHorizontalAlign.NotSet:
							case StaticHorizontalAlign.Left:
								result = ContentAlignment.MiddleLeft;
								break;
							case StaticHorizontalAlign.Center:
								result = ContentAlignment.MiddleCenter;
								break;
							case StaticHorizontalAlign.Right:
								result = ContentAlignment.MiddleRight;
								break;
						}
						break;
				}
			}
			return result;
		}
		private XAFLayoutItemInfo CreateItem(IModelViewLayoutElement itemInfo, ViewItemsCollection detailViewItems, XAFLayoutGroupInfo parentAdapterGroupInfo) {
			XAFLayoutItemInfo adapterItemInfo = null;
			if(itemInfo is IModelLayoutItem) {
				adapterItemInfo = CreateItem((IModelLayoutItem)itemInfo, detailViewItems);
			}
			else if(itemInfo is IModelLayoutGroup) {
				adapterItemInfo = CreateGroup((IModelLayoutGroup)itemInfo, detailViewItems);
			}
			else if(itemInfo is IModelTabbedGroup) {
				adapterItemInfo = CreateTabbedGroup((IModelTabbedGroup)itemInfo, detailViewItems);
			}
			if(adapterItemInfo != null) {
				OnItemCreated(adapterItemInfo.Item, itemInfo);
				if(parentAdapterGroupInfo != null) {
					parentAdapterGroupInfo.Item.Add(adapterItemInfo.Item);
					parentAdapterGroupInfo.Add(adapterItemInfo);
				}
			}
			return adapterItemInfo;
		}
		private XAFLayoutItemInfo CreateItem(IModelLayoutItem itemInfo, ViewItemsCollection detailViewItems) {
			XAFLayoutItemInfo adapterItemInfo = null;
			if(itemInfo is IModelLayoutViewItem) {
				string itemId = ((IModelLayoutViewItem)itemInfo).ViewItem != null ? ((IModelLayoutViewItem)itemInfo).ViewItem.Id : itemInfo.Id;
				ViewItem item = (ViewItem)detailViewItems[itemId];
				if(item != null) {
					adapterItemInfo = CreateControlItem((IModelLayoutViewItem)itemInfo, item);
				}
				else {
					adapterItemInfo = CreateEmptySpaceItem((IModelLayoutItem)itemInfo);
				}
			}
			else if(itemInfo is IModelSplitter) {
				adapterItemInfo = CreateSpliterItem((IModelSplitter)itemInfo);
			}
			else if(itemInfo is IModelSeparator) {
				adapterItemInfo = CreateSeparatorItem((IModelSeparator)itemInfo);
			}
			else if(itemInfo is IModelLabel) {
				adapterItemInfo = CreateLabelItem((IModelLabel)itemInfo);
			}
			LayoutControlItem controlItem = (LayoutControlItem)adapterItemInfo.Item;
			controlItem.AllowHtmlStringInCaption = isHtmlFormattingEnabled;
			ApplySizeConstraints(controlItem, itemInfo);
			return adapterItemInfo;
		}
		private XAFLayoutItemInfo CreateDefaultControlItem(ViewItem detailViewItem) {
			IModelLayoutViewItem itemModel = null;
			XAFLayoutItemInfo itemInfo = null;
			try {
				itemModel = ((ModelNode)LayoutModel).AddNode<IModelLayoutViewItem>(detailViewItem.Id + TempModelNodeIdSuffix + System.Guid.NewGuid().ToString());
				itemInfo = CreateControlItem(itemModel, detailViewItem);
			}
			finally {
				if(itemModel != null) {
					itemModel.Remove();
				}
			}
			return itemInfo;
		}
		protected virtual XAFLayoutItemInfo CreateControlItem(IModelLayoutViewItem controlItemInfo, ViewItem viewItem) {
			XafLayoutControlItem controlItem = new XafLayoutControlItem();
			OnLayoutItemCreated(controlItemInfo.Id, controlItem);
			viewItemByLayoutControlItem.Add(controlItem, viewItem);
			controlItem.Name = AddUniqueSuffix(viewItem.Id);
			Control control = GetControl(viewItem, controlItem.Name, false);
			if(control != null) {
				controlItem.Control = control;
			}
			else {
				controlItem.QueryControl += controlItem_QueryControl;
			}
			MarkRequiredFieldCaptionEventArgs args = new MarkRequiredFieldCaptionEventArgs(viewItem, false);
			OnMarkRequiredFieldCaption(args);
			controlItem.Text = BuildItemCaption(viewItem.Caption, args.NeedMarkRequiredField, args.RequiredFieldMark);
			if(controlItemInfo.ShowCaption.HasValue) {
				controlItem.TextVisible = controlItemInfo.ShowCaption.Value;
			}
			else {
				controlItem.TextVisible = viewItem.IsCaptionVisible;
			}
			if(control is IFrameTemplate) {
				controlItem.TextAlignMode = TextAlignModeItem.AutoSize;
			}
			IModelWinLayoutItem winControlItemInfo = controlItemInfo as IModelWinLayoutItem;
			if(winControlItemInfo != null) {
				controlItem.TextAlignMode = winControlItemInfo.TextAlignMode;
			}
			IModelLayoutElementWithCaptionOptions winLayoutElementModel = controlItemInfo as IModelLayoutElementWithCaptionOptions;
			if(winLayoutElementModel != null) {
				SetupBaseLayoutItem(controlItem, winLayoutElementModel);
			}
			ISupportControlAlignment supportControlAlignment = null;
			if(IsAlignmentSet(controlItemInfo)) {
				supportControlAlignment = controlItemInfo;
			}
			else {
				IModelDetailView viewModel = LayoutModel.Parent as IModelDetailView;
				if(viewModel != null) {
					supportControlAlignment = viewModel.Items[viewItem.Id] as ISupportControlAlignment;
				}
			}
			if(supportControlAlignment != null) {
				controlItem.ControlAlignment = GetControlAlignment(supportControlAlignment);
				controlItem.TrimClientAreaToControl = false;
			}
			((ISupportToolTip)this).SetToolTip(controlItem, controlItemInfo);
			controlItem.TextChanged += controlItem_TextChanged;
			if(viewItem is INotifyAppearanceVisibilityChanged) {
				((INotifyAppearanceVisibilityChanged)viewItem).VisibilityChanged += ViewItem_VisibilityChanged;
			}
			return CreateXAFLayoutItemInfo<XAFLayoutItemInfo>(controlItem, controlItemInfo.RelativeSize);
		}
		private Control GetControl(ViewItem viewItem, string name, bool forceControlCreation) {
			if(forceControlCreation && viewItem.Control == null) {
				viewItem.CreateControl();
			}
			return GetValidControlOrNull((Control)viewItem.Control, name);
		}
		private Control GetValidControlOrNull(Control control, string name) {
			if(control != null && !usedControls.Contains(control)) {
				usedControls.Add(control);
				control.Name = name;
				if(control is BaseEdit) { 
					((BaseEdit)control).StyleController = layoutControl;
				}
				return control;
			}
			return null;
		}
		private void controlItem_QueryControl(object sender, QueryControlEventArgs e) {
			XafLayoutControlItem layoutControlItem = (XafLayoutControlItem)sender;
			ViewItem viewItem = FindViewItem(layoutControlItem);
			layoutControlItem.QueryControl -= controlItem_QueryControl;
			e.Control = GetControl(viewItem, layoutControlItem.Name, true);
		}
		private bool IsAlignmentSet(IModelLayoutViewItem controlItemInfo) {
			return (controlItemInfo.HorizontalAlign != StaticHorizontalAlign.NotSet || controlItemInfo.VerticalAlign != StaticVerticalAlign.NotSet);
		}
		private void ViewItem_VisibilityChanged(object sender, EventArgs e) {
			if(layoutControl.IsDisposed) return;
			ViewItem viewItem = (ViewItem)sender;
			LayoutControlItem layoutControlItem = FindLayoutControlItem(viewItem);
			if(layoutControlItem != null) {
				layoutControl.BeginUpdate();
				SetLayoutItemVisibility(layoutControlItem, ((IAppearanceVisibility)viewItem).Visibility);
				layoutControl.EndUpdate();
			}
		}
		protected virtual XAFLayoutGroupInfo CreateGroup(IModelLayoutGroup groupInfo, ViewItemsCollection detailViewItems) {
			XafLayoutControlGroup group = CreateLayoutItem<XafLayoutControlGroup>(groupInfo.Id);
			group.AppearanceGroup.Options.UseBackColor = true;
			group.AppearanceGroup.Options.UseFont = true;
			group.Model = groupInfo;
			if(groupInfo.Direction == DevExpress.ExpressApp.Layout.FlowDirection.Vertical) {
				group.DefaultLayoutType = LayoutType.Vertical;
			}
			else {
				group.DefaultLayoutType = LayoutType.Horizontal;
			}
			if(groupInfo.Parent is IModelTabbedGroup && !((ModelNode)groupInfo).HasValue("ShowCaption")) { 
				group.TextVisible = true;
			}
			else {
				group.TextVisible = groupInfo.ShowCaption.HasValue && groupInfo.ShowCaption.Value;
			}
			group.GroupBordersVisible = group.TextVisible;
			group.AllowHtmlStringInCaption = isHtmlFormattingEnabled;
			group.Text = groupInfo.Caption;
			((ISupportToolTip)this).SetToolTip(group, groupInfo);
			IModelWinLayoutGroup winGroupModel = groupInfo as IModelWinLayoutGroup;
			if(winGroupModel != null) {
				TextAlignModeGroup textAlignMode = winGroupModel.TextAlignMode;
				if(group.OptionsItemText.TextAlignMode != textAlignMode) {
					group.OptionsItemText.TextAlignMode = textAlignMode;
				}
			}
			SetupBaseLayoutItem(group, groupInfo as IModelLayoutElementWithCaption);
			SetLayoutControlGroupImage(groupInfo, group);
			XAFLayoutGroupInfo adapterGroupInfo = CreateXAFLayoutGroupInfo(group, groupInfo.RelativeSize);
			foreach(IModelViewLayoutElement itemInfo in groupInfo) {
				CreateItem(itemInfo, detailViewItems, adapterGroupInfo);
			}
			return adapterGroupInfo;
		}
		private XAFTabbedGroupInfo CreateTabbedGroup(IModelTabbedGroup tabbedGroupInfo, ViewItemsCollection items) {
			TabbedControlGroup tabbedGroup = CreateLayoutItem<TabbedControlGroup>(tabbedGroupInfo.Id);
			IModelLayoutElementWithCaptionOptions winLayoutElementModel = tabbedGroupInfo as IModelLayoutElementWithCaptionOptions;
			if(winLayoutElementModel != null) {
				SetupBaseLayoutItem(tabbedGroup, winLayoutElementModel);
			}
			if(tabbedGroupInfo.MultiLine) {
				tabbedGroup.MultiLine = DevExpress.Utils.DefaultBoolean.True;
			}
			XAFTabbedGroupInfo adapterTabbedGroupInfo = CreateXAFLayoutItemInfoWithBounds<XAFTabbedGroupInfo>(tabbedGroup, tabbedGroupInfo.RelativeSize);
			List<LayoutGroup> items_ = new List<LayoutGroup>();
			foreach(IModelLayoutGroup tabGroupModel in tabbedGroupInfo) {
				XAFLayoutGroupInfo adapterTabInfo = (XAFLayoutGroupInfo)CreateItem(tabGroupModel, items, null);
				SetLayoutControlGroupImage(tabGroupModel, (LayoutControlGroup)adapterTabInfo.Item);
				adapterTabbedGroupInfo.Add(adapterTabInfo);
				if((adapterTabInfo.Item.Items.Count == 1) && (adapterTabInfo.Item.Items[0] is LayoutControlItem)) {
					((LayoutControlItem)adapterTabInfo.Item.Items[0]).TextAlignMode = TextAlignModeItem.AutoSize;
				}
				items_.Add(adapterTabInfo.Item);
			}
			tabbedGroup.TabPages.AddRange(items_.ToArray());
			return adapterTabbedGroupInfo;
		}
		protected virtual XAFLayoutItemInfo CreateEmptySpaceItem(IModelLayoutItem emptySpaceItemInfo) {
			EmptySpaceItem emptySpaceItem = CreateLayoutItem<EmptySpaceItem>(emptySpaceItemInfo.Id);
			emptySpaceItem.Text = "";
			emptySpaceItem.TextVisible = false;
			return CreateXAFLayoutItemInfo<XAFLayoutItemInfo>(emptySpaceItem, emptySpaceItemInfo.RelativeSize);
		}
		private void ApplySizeConstraints(LayoutControlItem layoutItem, IModelLayoutItem modelLayoutItemBase) {
			if(modelLayoutItemBase.SizeConstraintsType == XafSizeConstraintsType.Default ||
				modelLayoutItemBase.SizeConstraintsType == XafSizeConstraintsType.Custom &&
				modelLayoutItemBase.MinSize.IsEmpty && modelLayoutItemBase.MaxSize.IsEmpty) {
				layoutItem.SizeConstraintsType = SizeConstraintsType.Default;
			}
			else {
				layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
			}
			layoutItem.MinSize = modelLayoutItemBase.MinSize;
			layoutItem.MaxSize = modelLayoutItemBase.MaxSize;
		}
		Dictionary<BaseLayoutItem, string> createdLayoutItemsCache;
		private void InitializeCreatedLayoutItemsCache() {
			createdLayoutItemsCache = new Dictionary<BaseLayoutItem, string>();
		}
		private void DestroyCreatedLayoutItemsCache() {
			createdLayoutItemsCache = null;
		}
		private void OnLayoutItemCreated(string name, BaseLayoutItem item) {
			item.BeginInit();
			createdLayoutItemsCache.Add(item, name);
		}
		private void EndInitCreatedLayoutItems() {
			foreach(BaseLayoutItem item in createdLayoutItemsCache.Keys) {
				item.EndInit();
			}
		}
		private XAFLayoutItemInfo CreateSpliterItem(IModelSplitter splitterItemInfo) {
			SplitterItem splitterItem = CreateLayoutItem<SplitterItem>(splitterItemInfo.Id);
			double relativeSize = 1; 
			return CreateXAFLayoutItemInfo<XAFLayoutItemInfo>(splitterItem, relativeSize);
		}
		private XAFLayoutItemInfo CreateSeparatorItem(IModelSeparator separatorItemInfo) {
			SimpleSeparator separatorItem = CreateLayoutItem<SimpleSeparator>(separatorItemInfo.Id);
			double relativeSize = 1; 
			return CreateXAFLayoutItemInfo<XAFLayoutItemInfo>(separatorItem, relativeSize);
		}
		private XAFLayoutItemInfo CreateLabelItem(IModelLabel labelItemInfo) {
			SimpleLabelItem labelItem = CreateLayoutItem<SimpleLabelItem>(labelItemInfo.Id);
			labelItem.Text = labelItemInfo.Text;
			return CreateXAFLayoutItemInfo<XAFLayoutItemInfo>(labelItem, labelItemInfo.RelativeSize);
		}
		private T CreateLayoutItem<T>(string name) where T : BaseLayoutItem, new() {
			T item = new T();
			OnLayoutItemCreated(name, item);
			item.Name = AddUniqueSuffix(name);
			return item;
		}
		private T CreateXAFLayoutItemInfo<T>(BaseLayoutItem item, double relativeSize) where T : XAFLayoutItemInfo, new() {
			T result = new T();
			result.Item = item;
			result.ID = item.Name;
			result.RelativeSize = relativeSize;
			return result;
		}
		private T CreateXAFLayoutItemInfoWithBounds<T>(BaseLayoutItem item, double relativeSize) where T : XAFLayoutItemInfo, new() {
			T result = CreateXAFLayoutItemInfo<T>(item, relativeSize);
			result.Bounds = new Rectangle(0, 0, defaultBoundWidth, defaultBoundHeight);
			return result;
		}
		private XAFLayoutGroupInfo CreateXAFLayoutGroupInfo(LayoutGroup group, double relativeSize) {
			XAFLayoutGroupInfo result = CreateXAFLayoutItemInfoWithBounds<XAFLayoutGroupInfo>(group, relativeSize);
			result.LayoutType = group.DefaultLayoutType;
			result.IsGroupBoundsVisible = group.GroupBordersVisible;
			return result;
		}
		private void SynchronizeLayoutElementModel(IModelLayoutElementWithCaptionOptions winLayoutElementModel, BaseLayoutItem layoutItem) {
			if(winLayoutElementModel != null && layoutItem != null) {
				winLayoutElementModel.CaptionLocation = layoutItem.TextLocation;
				winLayoutElementModel.CaptionWordWrap = layoutItem.AppearanceItemCaption.TextOptions.WordWrap; 
				winLayoutElementModel.CaptionHorizontalAlignment = layoutItem.AppearanceItemCaption.TextOptions.HAlignment;
				winLayoutElementModel.CaptionVerticalAlignment = layoutItem.AppearanceItemCaption.TextOptions.VAlignment;
			}
		}
		private IModelViewLayout UpdateLayoutModel(IModelViewLayout newModel) {
			TreeStyleLayoutAdapter adapter = new TreeStyleLayoutAdapter(layoutControl);
			XAFLayoutGroupInfo rootAdapterGroupInfo = (XAFLayoutGroupInfo)adapter.GetXAFLayoutInfo();
			Int32 index = 0;
			RemoveDeletedNodes((ModelNode)newModel, rootAdapterGroupInfo.Items);
			foreach(XAFLayoutItemInfo adapterItemInfo in rootAdapterGroupInfo.Items) {
				UpdateItemModel(newModel, adapterItemInfo, index++);
			}
			return newModel;
		}
		private void RemoveDeletedNodes(ModelNode modelNode, ArrayList adapterItems) {
			if(adapterItems != null) {
				List<ModelNode> adapterNodes = new List<ModelNode>();
				foreach(XAFLayoutItemInfo childAdapterItemInfo in adapterItems) {
					adapterNodes.Add(modelNode.GetNode(RemoveUniqueSuffix(childAdapterItemInfo.ID)));
				}
				foreach(ModelNode childNode in modelNode.GetUnsortedChildren()) {
					if(!adapterNodes.Contains(childNode)) {
						((IModelNode)childNode).Remove();
					}
				}
			}
		}
		private IModelViewLayoutElement CreateNode(IModelNode parentModelNode, XAFLayoutItemInfo adapterItemInfo) {
			IModelViewLayoutElement element = null;
			if(adapterItemInfo.GetType() == typeof(XAFLayoutItemInfo)) {
				if(adapterItemInfo.Item is SplitterItem) {
					element = parentModelNode.AddNode<IModelSplitter>(RemoveUniqueSuffix(adapterItemInfo.ID));
				}
				else if(adapterItemInfo.Item is SimpleSeparator) {
					element = parentModelNode.AddNode<IModelSeparator>(RemoveUniqueSuffix(adapterItemInfo.ID));
				}
				else if(adapterItemInfo.Item is SimpleLabelItem) {
					element = parentModelNode.AddNode<IModelLabel>(RemoveUniqueSuffix(adapterItemInfo.ID));
				}
				else {
					element = parentModelNode.AddNode<IModelLayoutViewItem>(RemoveUniqueSuffix(adapterItemInfo.ID));
				}
			}
			else if(adapterItemInfo.GetType() == typeof(XAFLayoutGroupInfo)) {
				element = parentModelNode.AddNode<IModelLayoutGroup>(RemoveUniqueSuffix(adapterItemInfo.ID));
			}
			else if(adapterItemInfo.GetType() == typeof(XAFTabbedGroupInfo)) {
				element = parentModelNode.AddNode<IModelTabbedGroup>(RemoveUniqueSuffix(adapterItemInfo.ID));
			}
			return element;
		}
		private void UpdateItemModel(IModelNode parentModelNode, XAFLayoutItemInfo adapterItemInfo, int index) {
			IModelViewLayoutElement element = parentModelNode.GetNode(RemoveUniqueSuffix(adapterItemInfo.ID)) as IModelViewLayoutElement;
			if(element != null) {
				if(adapterItemInfo is XAFLayoutGroupInfo) {
					RemoveDeletedNodes((ModelNode)element, ((XAFLayoutGroupInfo)adapterItemInfo).Items);
				}
				else if(adapterItemInfo is XAFTabbedGroupInfo) {
					RemoveDeletedNodes((ModelNode)element, ((XAFTabbedGroupInfo)adapterItemInfo).Items);
				}
			}
			else {
				element = CreateNode(parentModelNode, adapterItemInfo);
			}
			if(adapterItemInfo.GetType() == typeof(XAFLayoutItemInfo)) {
				if(adapterItemInfo.Item is SimpleLabelItem) {
					((IModelLabel)element).Text = adapterItemInfo.Item.Text;
				}
				if(!(adapterItemInfo.Item is SplitterItem)
					&& !(adapterItemInfo.Item is SimpleSeparator)
					&& !(adapterItemInfo.Item is SimpleLabelItem)
					&& !(adapterItemInfo.Item is EmptySpaceItem)) {
					UpdateControlModel((IModelLayoutViewItem)element, adapterItemInfo);
				}
				SynchronizeSizeConstraints(adapterItemInfo, (IModelLayoutItem)element);
			}
			else if(adapterItemInfo.GetType() == typeof(XAFLayoutGroupInfo)) {
				UpdateGroupModel((IModelLayoutGroup)element, (XAFLayoutGroupInfo)adapterItemInfo);
			}
			else if(adapterItemInfo.GetType() == typeof(XAFTabbedGroupInfo)) {
				UpdateTabbedGroupModel(element, (XAFTabbedGroupInfo)adapterItemInfo);
			}
			element.Index = index;
			element.RelativeSize = adapterItemInfo.RelativeSize;
			OnSynchronizeItemInfo(element, adapterItemInfo.Item);
		}
		private IModelLayoutViewItem FindModelLayoutItem(IModelNode model, String id) {
			IModelLayoutViewItem result = model as IModelLayoutViewItem;
			if((result != null) && (result.Id == id)) {
				return result;
			}
			for(Int32 i = 0; i < model.NodeCount; i++) {
				result = FindModelLayoutItem(model.GetNode(i), id);
				if(result != null) {
					return result;
				}
			}
			return null;
		}
		private Boolean IsShowCaptionSynchronizationRequired(ViewItem viewItem, XAFLayoutItemInfo layoutItemInfo) {
			IModelLayoutViewItem itemModel = FindModelLayoutItem(LayoutModel, RemoveUniqueSuffix(layoutItemInfo.ID));
			return (itemModel != null && itemModel.ShowCaption.HasValue)
				|| (viewItem == null) || (layoutItemInfo.Item.TextVisible != viewItem.IsCaptionVisible);
		}
		private void UpdateControlModel(IModelLayoutViewItem itemModel, XAFLayoutItemInfo adapterControlItemInfo) {
			BaseLayoutItem layoutItem = adapterControlItemInfo.Item;
			ViewItem viewItem = layoutItem is LayoutControlItem ? FindViewItem((LayoutControlItem)layoutItem) : null;
			if(IsShowCaptionSynchronizationRequired(viewItem, adapterControlItemInfo)) {
				itemModel.ShowCaption = adapterControlItemInfo.Item.TextVisible;
			}
			if(viewItem != null && viewItem.Caption != RemoveCaptionColon(layoutItem.Text)) {
				viewItem.Caption = RemoveCaptionColon(layoutItem.Text);
			}
			if(itemModel is IModelWinLayoutItem) {
				((IModelWinLayoutItem)itemModel).TextAlignMode = ((LayoutControlItem)layoutItem).TextAlignMode;
			}
			if(viewItem != null) {
				itemModel.ViewItem = ((IModelCompositeView)this.LayoutModel.Parent).Items[viewItem.Id];
			}
			else {
				itemModel.ViewItem = null;
			}
			IModelLayoutElementWithCaptionOptions winLayoutElement = itemModel as IModelLayoutElementWithCaptionOptions;
			if(winLayoutElement != null) {
				SynchronizeLayoutElementModel(winLayoutElement, adapterControlItemInfo.Item);
			}
		}
		private void SynchronizeSizeConstraints(XAFLayoutItemInfo adapterControlItemInfo, IModelLayoutItem itemModel) {
			if(((LayoutControlItem)adapterControlItemInfo.Item).SizeConstraintsType != SizeConstraintsType.Default) {
				itemModel.SizeConstraintsType = XafSizeConstraintsType.Custom;
				itemModel.MinSize = adapterControlItemInfo.Item.MinSize;
				itemModel.MaxSize = adapterControlItemInfo.Item.MaxSize;
			}
			else {
				itemModel.SizeConstraintsType = XafSizeConstraintsType.Default;
			}
		}
		private IModelViewLayoutElement UpdateGroupModel(IModelLayoutGroup groupModel, XAFLayoutGroupInfo adapterGroupInfo) {
			Boolean isAutoGeneratedGroup = adapterGroupInfo.Item == null;
			if(isAutoGeneratedGroup) {
				groupModel.ShowCaption = false;
				groupModel.Caption = "";
			}
			else {
				groupModel.ShowCaption = adapterGroupInfo.Item.TextVisible;
				groupModel.Caption = adapterGroupInfo.Item.Text;
				if(adapterGroupInfo.Item is XafLayoutControlGroup) {
					XafLayoutControlGroup layoutControlGroup = (XafLayoutControlGroup)adapterGroupInfo.Item;
					if(layoutControlGroup.Model != null) {
						groupModel.ImageName = layoutControlGroup.Model.ImageName;
					}
				}
				SynchronizeLayoutElementModel(groupModel as IModelLayoutElementWithCaption, adapterGroupInfo.Item);
				IModelWinLayoutGroup winGroupModel = groupModel as IModelWinLayoutGroup;
				if(winGroupModel != null) {
					winGroupModel.TextAlignMode = adapterGroupInfo.Item.OptionsItemText.TextAlignMode;
				}
			}
			if(adapterGroupInfo.LayoutType == LayoutType.Horizontal) {
				groupModel.Direction = DevExpress.ExpressApp.Layout.FlowDirection.Horizontal;
			}
			else {
				groupModel.Direction = DevExpress.ExpressApp.Layout.FlowDirection.Vertical;
			}
			Int32 index = 0;
			foreach(XAFLayoutItemInfo adapterItemInfo in adapterGroupInfo.Items) {
				UpdateItemModel(groupModel, adapterItemInfo, index++);
			}
			return groupModel;
		}
		private void UpdateTabbedGroupModel(IModelViewLayoutElement tabbedGroupModel, XAFTabbedGroupInfo adapterTabbedGroupInfo) {
			SynchronizeLayoutElementModel(tabbedGroupModel as IModelLayoutElementWithCaptionOptions, adapterTabbedGroupInfo.Item);
			for(int i = 0; i < adapterTabbedGroupInfo.Items.Count; i++) {
				UpdateItemModel(tabbedGroupModel, (XAFLayoutGroupInfo)adapterTabbedGroupInfo.Items[i], i);
			}
		}
		private void layoutControl_CustomizationClosed(object sender, EventArgs e) {
			if(layoutControl.IsModified) {
				UpdateLayoutGroup(layoutControl.Root);
			}
		}
		private void controlItem_TextChanged(object sender, EventArgs e) {
			BaseLayoutItem controlItem = sender as BaseLayoutItem;
			if(controlItem != null) {
				ViewItem detailViewItem = viewItems[RemoveUniqueSuffix(controlItem.Name)];
				if(detailViewItem != null && detailViewItem.Caption != controlItem.Text) {
					detailViewItem.Caption = controlItem.Text;
				}
			}
		}
		private LayoutControlItem FindLayoutControlItem(ViewItem viewItem) {
			foreach(BaseLayoutItem item in layoutControl.Items) {
				LayoutControlItem layoutControlItem = item as LayoutControlItem;
				if(layoutControlItem != null && FindViewItem(layoutControlItem) == viewItem) {
					return layoutControlItem;
				}
			}
			return null;
		}
		private void AddInvisibleControlsToCustomization(ViewItemsCollection detailViewItems, XAFLayoutGroupInfo rootAdapterGroupInfo) {
			foreach(ViewItem viewItem in detailViewItems) {
				if(FindLayoutControlItem(viewItem) == null) {
					XAFLayoutItemInfo adapterControlItemInfo = CreateDefaultControlItem(viewItem);
					OnItemCreated(adapterControlItemInfo.Item, null);
					rootAdapterGroupInfo.Item.Add(adapterControlItemInfo.Item);
					rootAdapterGroupInfo.Add(adapterControlItemInfo);
					layoutControl.HideItem(adapterControlItemInfo.Item);
				}
			}
		}
		private void layoutControl_ShowContextMenu(object sender, PopupMenuShowingEventArgs e) {
			foreach(DXMenuItem item in e.Menu.Items) {
				if(item.Caption == GetLocalizedText(LayoutStringId.ShowTextMenuItem)
					|| item.Caption == GetLocalizedText(LayoutStringId.HideTextMenuItem)) {
					item.Click += new EventHandler(WinLayoutManager_ContextMenu_Click);
				}
			}
		}
		private string GetLocalizedText(LayoutStringId sid) {
			return LayoutLocalizer.Active.GetLocalizedString(sid);
		}
		private void WinLayoutManager_ContextMenu_Click(object sender, EventArgs e) {
			SelectionHelper selectionHelper = new SelectionHelper();
			List<BaseLayoutItem> list = selectionHelper.GetItemsList(layoutControl.Root);
			if(list.Count > 0) {
				foreach(BaseLayoutItem item in list) {
					LayoutGroup layoutGroup = item as LayoutGroup;
					if(layoutGroup != null) {
						layoutGroup.GroupBordersVisible = !layoutGroup.GroupBordersVisible;
					}
				}
			}
		}
		private void layoutControl_RequestUniqueName(object sender, UniqueNameRequestArgs e) {
			List<String> existentNames = new List<String>();
			foreach(BaseLayoutItem item in layoutControl.Items) {
				existentNames.Add(RemoveUniqueSuffix(item.Name));
			}
			foreach(BaseLayoutItem item in layoutControl.HiddenItems) {
				existentNames.Add(RemoveUniqueSuffix(item.Name));
			}
			Int32 index = 1;
			String itemNameBase = "Item";
			if(e.TargetItem is SplitterItem) {
				itemNameBase = "SplitterItem";
			}
			else if(e.TargetItem is SimpleLabelItem) {
				itemNameBase = "LabelItem";
			}
			while(existentNames.IndexOf(itemNameBase + index.ToString()) >= 0) {
				index++;
			}
			e.TargetItem.Name = itemNameBase + index.ToString();
		}
		private Dictionary<string, string> groupImageNameCash = new Dictionary<string, string>();
		private void SetLayoutControlGroupImage(IModelLayoutGroup groupInfo, LayoutControlGroup group) {
			string imageName;
			if(!groupImageNameCash.TryGetValue(groupInfo.Id, out imageName)) {
				imageName = groupInfo.ImageName;
				groupImageNameCash[groupInfo.Id] = imageName;
			}
			DevExpress.ExpressApp.Utils.ImageInfo imageInfo = ImageLoader.Instance.GetImageInfo(imageName);
			if(!imageInfo.IsEmpty) {
				group.CaptionImage = imageInfo.Image;
			}
		}
		protected virtual void UpdateLayoutGroup(LayoutGroup group) { }
		protected override Object GetContainerCore() {
			return layoutControl;
		}
		protected virtual void OnSynchronizeItemInfo(IModelNode layoutElementModel, BaseLayoutItem baseLayoutItem) {
			if(SynchronizeItemInfo != null) {
				SynchronizeItemInfoEventArgs e = new SynchronizeItemInfoEventArgs(layoutElementModel, baseLayoutItem);
				SynchronizeItemInfo(this, e);
			}
		}
		protected virtual void OnItemCreated(BaseLayoutItem item, IModelViewLayoutElement model) {
			if(ItemCreated != null) {
				ViewItem viewItem = item is LayoutControlItem ? FindViewItem((LayoutControlItem)item) : null;
				ItemCreatedEventArgs e = new ItemCreatedEventArgs(item, viewItem, model);
				ItemCreated(this, e);
			}
		}
		protected virtual void OnLayoutInfoApplied() {
			if(LayoutInfoApplied != null) {
				LayoutInfoApplied(this, EventArgs.Empty);
			}
		}
		protected internal void UnsubscribeFromLayoutItemsTextChanged() {
			foreach(KeyValuePair<LayoutControlItem, ViewItem> pair in viewItemByLayoutControlItem) {
				LayoutControlItem layoutControlItem = pair.Key;
				layoutControlItem.TextChanged -= controlItem_TextChanged;
				INotifyAppearanceVisibilityChanged notifyAppearanceVisibilityChanged = pair.Value as INotifyAppearanceVisibilityChanged;
				if(notifyAppearanceVisibilityChanged != null) {
					notifyAppearanceVisibilityChanged.VisibilityChanged -= ViewItem_VisibilityChanged;
				}
			}
		}
		protected override void OnLayoutCreated() {
			base.OnLayoutCreated();
			layoutControl.BeginUpdate();
			foreach(KeyValuePair<LayoutControlItem, ViewItem> pair in viewItemByLayoutControlItem) {
				IAppearanceVisibility appearanceVisibility = pair.Value as IAppearanceVisibility;
				if(appearanceVisibility != null) {
					ViewItemVisibility viewItemVisibility = appearanceVisibility.Visibility;
					if(viewItemVisibility != ViewItemVisibility.Show) {
						LayoutControlItem layoutControlItem = pair.Key;
						SetLayoutItemVisibility(layoutControlItem, viewItemVisibility);
					}
				}
			}
			layoutControl.EndUpdate();
		}
		public override void ClearLayoutItems() {
			if(layoutControl != null && layoutControl.Root != null) {
				layoutControl.Root.Clear();
			}
		}
		public override void UpdateViewItem(ViewItem newViewItem) {
			if(layoutControl.IsDisposed) return;
			ViewItem oldViewItem = null;
			LayoutControlItem layoutControlItem = null;
			foreach(KeyValuePair<LayoutControlItem, ViewItem> pair in viewItemByLayoutControlItem) {
				ViewItem viewItem = pair.Value;
				if(viewItem.Id == newViewItem.Id) {
					oldViewItem = viewItem;
					layoutControlItem = pair.Key;
					break;
				}
			}
			if(oldViewItem != null) {
				if(oldViewItem is INotifyAppearanceVisibilityChanged) {
					((INotifyAppearanceVisibilityChanged)oldViewItem).VisibilityChanged -= new EventHandler(ViewItem_VisibilityChanged);
				}
				if(newViewItem is INotifyAppearanceVisibilityChanged) {
					((INotifyAppearanceVisibilityChanged)newViewItem).VisibilityChanged += new EventHandler(ViewItem_VisibilityChanged);
				}
				viewItemByLayoutControlItem[layoutControlItem] = newViewItem;
				ReplaceControl(layoutControlItem, (Control)newViewItem.Control);
			}
		}
		public override void ReplaceControl(string controlID, object control) {
			if(layoutControl.IsDisposed) return;
			layoutControl.BeginUpdate();
			foreach(BaseLayoutItem item in layoutControl.Items) {
				LayoutControlItem layoutControlItem = item as LayoutControlItem;
				if(layoutControlItem != null && RemoveUniqueSuffix(item.Name) == controlID) {
					ReplaceControl(layoutControlItem, (Control)control);
					break;
				}
			}
			layoutControl.EndUpdate();
		}
		private void ReplaceControl(LayoutControlItem layoutControlItem, Control control) {
			layoutControlItem.BeginInit();
			Control oldControl = layoutControlItem.Control;
			if(oldControl != null) {
				usedControls.Remove(oldControl);
			}
			layoutControlItem.Control = GetValidControlOrNull(control, layoutControlItem.Name);
			if(oldControl != null) {
				oldControl.Parent = null;
			}
			layoutControlItem.EndInit();
		}
		public WinLayoutManager() {
			usedControls = new HashSet<Control>();
			viewItemByLayoutControlItem = new Dictionary<LayoutControlItem, ViewItem>();
			splitLayoutManager = new WinSimpleLayoutManager();
			layoutControl = new XafLayoutControl();
			layoutControl.OptionsView.PaddingSpacingMode = PaddingMode.MSGuidelines;
			layoutControl.PopupMenuShowing += new PopupMenuShowingEventHandler(layoutControl_ShowContextMenu);
			layoutControl.RequestUniqueName += new UniqueNameRequestHandler(layoutControl_RequestUniqueName);
			layoutControl.CustomizationClosed += new EventHandler(layoutControl_CustomizationClosed);
		}
		public override void Dispose() {
			try {
				if(layoutControl != null) {
					UnsubscribeFromLayoutItemsTextChanged();
					layoutControl.PopupMenuShowing -= new PopupMenuShowingEventHandler(layoutControl_ShowContextMenu);
					layoutControl.RequestUniqueName -= new UniqueNameRequestHandler(layoutControl_RequestUniqueName);
					layoutControl.CustomizationClosed -= new EventHandler(layoutControl_CustomizationClosed);
					layoutControl.Dispose();
					layoutControl = null;
				}
				splitLayoutManager.Dispose();
				if(usedControls != null) {
					usedControls.Clear();
					usedControls = null;
				}
				if(viewItemByLayoutControlItem != null) {
					viewItemByLayoutControlItem.Clear();
					viewItemByLayoutControlItem = null;
				}
			}
			finally {
				base.Dispose();
			}
		}
		public ViewItem FindViewItem(LayoutControlItem layoutControlItem) {
			Guard.ArgumentNotNull(layoutControlItem, "layoutControlItem");
			ViewItem viewItem;
			if(viewItemByLayoutControlItem.TryGetValue(layoutControlItem, out viewItem)) {
				return viewItem;
			}
			return null;
		}
		public override object LayoutControls(IModelNode layoutNode, ViewItemsCollection viewItems) {
			if(layoutNode is IModelSplitLayout) {
				return splitLayoutManager.LayoutControls(layoutNode, viewItems);
			}
			IModelViewLayout layoutModel = layoutNode as IModelViewLayout;
			if(layoutModel != null) {
				base.LayoutControls(layoutNode, viewItems);
				InitializeCreatedLayoutItemsCache();
				this.viewItems = viewItems;
				layoutControl.BeginInit();
				layoutControl.SuspendLayout();
				layoutControl.Root.BeginInit();
				usedControls.Clear();
				viewItemByLayoutControlItem.Clear();
				try {
					layoutControl.Bounds = new Rectangle(0, 0, defaultBoundWidth, defaultBoundHeight);
					layoutControl.Clear(true, true);
					XAFLayoutGroupInfo rootAdapterGroupInfo = CreateXAFLayoutGroupInfo(layoutControl.Root, 0);
					InitializeLayoutOptions(layoutModel);
					foreach(IModelViewLayoutElement itemModel in (IModelViewLayout)layoutNode) {
						CreateItem(itemModel, viewItems, rootAdapterGroupInfo);
					}
					layoutControl.BestFit();
					TreeStyleLayoutAdapter adapter = new TreeStyleLayoutAdapter(layoutControl);
					adapter.SetXAFLayoutInfo(rootAdapterGroupInfo);
					OnLayoutInfoApplied();
					AddInvisibleControlsToCustomization(viewItems, rootAdapterGroupInfo);
					OnLayoutCreated();
				}
				finally {
					EndInitCreatedLayoutItems();
					if(!((ISupportImplementor)layoutControl).Implementor.CanRestoreDefaultLayout) {
						layoutControl.SetDefaultLayout();
					}
					layoutControl.Root.EndInit();
					layoutControl.EndInit();
					layoutControl.ResumeLayout(false);
					foreach(KeyValuePair<BaseLayoutItem, string> item in createdLayoutItemsCache) {
						OnCustomizeAppearance(new CustomizeAppearanceEventArgs(item.Value, new WinLayoutItemAppearanceAdapter(item.Key), null));
					}
					DestroyCreatedLayoutItemsCache();
				}
				return layoutControl;
			}
			return base.LayoutControls(layoutNode, viewItems);
		}
		public override void SaveModel() {
			if(LayoutModel is IModelSplitLayout) {
				splitLayoutManager.SaveModel();
			}
			if(LayoutModel is IModelViewLayout) {
				if((LayoutModel != null) && layoutControl.IsModified) {
					IModelViewLayout layoutModelNode = (IModelViewLayout)LayoutModel;
					UpdateLayoutModel(layoutModelNode);
				}
			}
		}
		public LayoutControlItem CreateControlItem(ViewItem detailViewItem) {
			if(detailViewItem != null) {
				InitializeCreatedLayoutItemsCache();
				XAFLayoutItemInfo adapterControlItemInfo = CreateDefaultControlItem(detailViewItem);
				OnItemCreated(adapterControlItemInfo.Item, null);
				layoutControl.BeginInit();
				layoutControl.Root.BeginInit();
				try {
					layoutControl.Root.Add(adapterControlItemInfo.Item);
					adapterControlItemInfo.Item.HideToCustomization();
				}
				finally {
					EndInitCreatedLayoutItems();
					layoutControl.Root.EndInit();
					layoutControl.EndInit();
					DestroyCreatedLayoutItemsCache();
				}
				if(detailViewItem.Control is IFrameTemplate) {
					adapterControlItemInfo.Item.TextVisible = false;
				}
				LayoutControlItem controlItem = (LayoutControlItem)adapterControlItemInfo.Item;
				return controlItem;
			}
			return null;
		}
		public new XafLayoutControl Container {
			get { return layoutControl; }
		}
		public override Boolean CustomizationEnabled {
			get { return layoutControl.AllowCustomization; }
			set { layoutControl.AllowCustomization = value; }
		}
		public event EventHandler<SynchronizeItemInfoEventArgs> SynchronizeItemInfo;
		public event EventHandler<ItemCreatedEventArgs> ItemCreated;
		public event EventHandler LayoutInfoApplied;
		#region IHtmlFormattingSupport Members
		public void SetHtmlFormattingEnabled(bool htmlFormattingEnabled) {
			isHtmlFormattingEnabled = htmlFormattingEnabled;
		}
		#endregion
		void ISupportToolTip.SetToolTip(object element, IModelToolTip model) {
			BaseLayoutItem baseItem = element as BaseLayoutItem;
			if(model != null && baseItem != null) {
				if(!string.IsNullOrEmpty(model.ToolTip)) {
					baseItem.OptionsToolTip.ToolTip = model.ToolTip;
				}
				IModelToolTipOptions options = model as IModelToolTipOptions;
				if(options != null) {
					if(!string.IsNullOrEmpty(options.ToolTipTitle)) {
						baseItem.OptionsToolTip.ToolTipTitle = options.ToolTipTitle;
					}
					if(options.ToolTipIconType != DevExpress.Utils.ToolTipIconType.None) {
						baseItem.OptionsToolTip.ToolTipIconType = options.ToolTipIconType;
					}
				}
			}
		}
	}
	public class SynchronizeItemInfoEventArgs : EventArgs {
		private IModelNode layoutElementModel;
		private BaseLayoutItem item;
		public SynchronizeItemInfoEventArgs(IModelNode layoutElementModel, BaseLayoutItem item) {
			this.layoutElementModel = layoutElementModel;
			this.item = item;
		}
		public IModelNode LayoutElementModel {
			get { return layoutElementModel; }
		}
		public BaseLayoutItem Item {
			get { return item; }
		}
	}
	public class ItemCreatedEventArgs : EventArgs {
		private BaseLayoutItem item;
		private ViewItem viewItem;
		private IModelViewLayoutElement modelLayoutElement;
		public ItemCreatedEventArgs(BaseLayoutItem item, ViewItem viewItem, IModelViewLayoutElement modelLayoutElement) {
			this.item = item;
			this.viewItem = viewItem;
			this.modelLayoutElement = modelLayoutElement;
		}
		public BaseLayoutItem Item {
			get { return item; }
		}
		public IModelViewLayoutElement ModelLayoutElement {
			get { return modelLayoutElement; }
		}
		public ViewItem ViewItem {
			get { return viewItem; }
		}
	}
}
