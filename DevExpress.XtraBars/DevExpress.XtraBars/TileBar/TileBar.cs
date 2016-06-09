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

using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Win;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Navigation {
	[ToolboxTabName(AssemblyInfo.DXTabNavigation)]
	[ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "TileBar")]
	[Description("A bar with tiles that support the dropdown functionality.")]
	[Designer("DevExpress.XtraBars.Design.TileBarDesigner, " + AssemblyInfo.SRAssemblyBarsDesign)]
	[Docking(DockingBehavior.Never)]
	public class TileBar : TileControl, ITileBarDropDownOptionsOwner {
		public TileBar() : base() {
			base.RowCount = 1;
			base.ColumnCount = 1;
			base.AllowDrag = false;
			base.IndentBetweenGroups = defaultIndentBetweenItems;
			base.IndentBetweenItems = defaultIndentBetweenItems;
			base.ItemSize = tileBarItemDefaultSize;
			((ITileControl)this).Properties.LargeItemWidth = wideTileWidthDefault;
			base.AllowItemHover = true;
			base.AnimateArrival = false;
			base.ScrollMode = TileControlScrollMode.ScrollButtons;
			base.ShowGroupText = defaultShowGroupText;
			base.ItemPadding = DefaultItemPadding;
			groupTextToItemsIndent = groupTextToItemsIndentDefault;
			selectionWidthCore = 1;
			showDirection = ShowDirection.Normal;
			selectionColorMode = SelectionColorMode.Default;
			CloseDropDownOnItemClick = true;
		}
		[DefaultValue(true), Category(CategoryName.Behavior)]
		public bool CloseDropDownOnItemClick { get; set; }
		SelectionColorMode selectionColorMode;
		[DefaultValue(SelectionColorMode.Default), Category(CategoryName.Appearance)]
		public SelectionColorMode SelectionColorMode {
			get { return selectionColorMode; }
			set {
				if(selectionColorMode == value) return;
				selectionColorMode = value;
				OnPropertiesChanged();
			}
		}
		ShowDirection showDirection;
		[DefaultValue(ShowDirection.Normal), Category(CategoryName.Behavior)]
		public ShowDirection DropDownShowDirection {
			get { return showDirection; }
			set { showDirection = value; }
		}
		[DefaultValue(defaultIndentBetweenItems), Category(CategoryName.Appearance)]
		new public int IndentBetweenGroups {
			get { return base.IndentBetweenGroups; }
			set { base.IndentBetweenGroups = value; }
		}
		[DefaultValue(true), Category(CategoryName.Behavior)]
		new public bool AllowItemHover {
			get { return base.AllowItemHover; }
			set { base.AllowItemHover = value; }
		}
		const int wideTileWidthDefault = 158;
		[DefaultValue(wideTileWidthDefault), Category(CategoryName.Appearance)]
		public int WideTileWidth {
			get { return ((ITileControl)this).Properties.LargeItemWidth; }
			set {
				if(value <= 0) return;
				((ITileControl)this).Properties.LargeItemWidth = value;
				OnPropertiesChanged();
			}
		}
		int selectionWidthCore;
		[DefaultValue(1), Category(CategoryName.Appearance)]
		public int SelectionBorderWidth {
			get { return selectionWidthCore; }
			set { 
				if(selectionWidthCore == value) return;
				selectionWidthCore = value;
				OnPropertiesChanged();
			}
		}
		TileBarDropDownOptions dropDownOptions;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Behavior)]
		public TileBarDropDownOptions DropDownOptions {
			get {
				if(dropDownOptions == null)
					dropDownOptions = new TileBarDropDownOptions(this);
				return dropDownOptions;
			}
		}
		const int groupTextToItemsIndentDefault = 6;
		int groupTextToItemsIndent;
		[DefaultValue(groupTextToItemsIndentDefault), Category(CategoryName.Behavior)]
		public int GroupTextToItemsIndent {
			get { return groupTextToItemsIndent; }
			set {
				if(groupTextToItemsIndent == value)
					return;
				groupTextToItemsIndent = value;
				OnPropertiesChanged();
			}
		}
		bool showItemShadow = false;
		[DefaultValue(false), Category(CategoryName.Appearance)]
		public bool ShowItemShadow {
			get { return showItemShadow; }
			set {
				if(showItemShadow == value) return;
				showItemShadow = value;
				OnPropertiesChanged();
			}
		}
		protected override Padding DefaultPadding {
			get { return new Padding(22, 7, 22, 7); }
		}
		[DefaultValue(1), Browsable(false)]
		new public int RowCount {
			get { return base.RowCount; }
			set { base.RowCount = value; }
		}
		const int tileBarItemDefaultSize = 60;
		[DefaultValue(tileBarItemDefaultSize), Category(CategoryName.Appearance)]
		new public int ItemSize {
			get { return base.ItemSize; }
			set { 
				if(value > 0)
				base.ItemSize = value; 
			}
		}
		const int defaultIndentBetweenItems = 13;
		[DefaultValue(defaultIndentBetweenItems), Category(CategoryName.Appearance)]
		new public int IndentBetweenItems {
			get { return base.IndentBetweenItems; }
			set { base.IndentBetweenItems = value; }
		}
		const bool defaultShowGroupText = true;
		[DefaultValue(defaultShowGroupText), Category(CategoryName.Appearance)]
		new public bool ShowGroupText {
			get { return base.ShowGroupText; }
			set { base.ShowGroupText = value; }
		}
		void ResetItemPadding() { ItemPadding = DefaultItemPadding; }
		bool ShouldSerializeItemPadding() { return ItemPadding != DefaultItemPadding; }
		[Category(CategoryName.Appearance)]
		new public Padding ItemPadding {
			get { return base.ItemPadding; }
			set {
				if(ItemPadding == value)
					return;
				base.ItemPadding = value;
				OnPropertiesChanged();
			}
		}
		new public static Padding DefaultItemPadding { get { return new Padding(12, 6, 12, 6); } }
		internal const int dropDownButtonWidthDefault = 25;
		int dropDownButtonWidth = dropDownButtonWidthDefault;
		[DefaultValue(dropDownButtonWidthDefault), Category(CategoryName.Appearance)]
		public int DropDownButtonWidth {
			get { return dropDownButtonWidth; }
			set {
				if(dropDownButtonWidth == value) return;
				dropDownButtonWidth = value;
				OnPropertiesChanged();
			}
		}
		protected override void OnAppearanceChanged(object sender, EventArgs e) {
			ViewInfo.ResetDropDownAppearances();
			base.OnAppearanceChanged(sender, e);
		}
		public void HideDropDownWindow() {
			ViewInfo.CloseDropDown();
		}
		protected override TileControlViewInfo CreateViewInfo() {
			return new TileBarViewInfo(this);
		}
		protected override TileControlHandler CreateHandler() {
			return new TileBarHandler(this);
		}
		protected override TileControlPainter CreatePainter() {
			return new TileBarPainter();
		}
		protected override TileControlNavigator CreateNavigator() {
			return new TileBarNavigator(this);
		}
		TileBarViewInfo ViewInfo { get { return (TileBarViewInfo)ViewInfoCore; } }
		protected override void OnItemClickCore(TileItem item) {
			base.OnItemClickCore(item);
			if(Owner == null) return;
			Owner.RaiseItemClick(item);
		}
		protected override ToolTipControlInfo GetToolTipInfo(Point point) {
			if(Owner == null) 
				return base.GetToolTipInfo(point);
			if(CanShowToolTip && Owner.ShowTileToolTips) {
				object obj = this;
				SuperToolTip superTip = null;
				ToolTipControlInfo res = new ToolTipControlInfo();
				var hitInfo = ViewInfoCore.CalcHitInfo(point);
				if(hitInfo.InItem && hitInfo.ItemInfo != null && hitInfo.ItemInfo.Item != null) {
					var item = hitInfo.ItemInfo.Item;
					obj = item;
					superTip = GetSuperTip(item);
				}
				res.Object = obj;
				res.SuperTip = superTip;
				return res;
			}
			return null;
		}
		protected virtual SuperToolTip GetSuperTip(TileItem item) { 
			if((item.SuperTip == null || item.SuperTip.IsEmpty) && Owner != null){
				var element = Owner.GetElementByTile(item);
				if(element != null) return element.SuperTip;
			}
			return item.SuperTip;
		}
		protected internal TileBarWindow ParentPopup { get; set; }
		protected internal ITileBarOwner Owner { get; set; }
		void ITileBarDropDownOptionsOwner.OnDropDownOptionsChanged() { OnPropertiesChanged(); }
		void ITileBarDropDownOptionsOwner.OnDropDownHeightChanged() { }
		protected internal bool InvertedGroupTextAppearance { get; set; }
		internal void SetItemsInvertedAppearance(bool inverted) {
			foreach(TileBarGroup g in Groups) {
				foreach(TileBarItem it in g.Items) {
					it.InvertedAppearance = inverted;
				}
			}
			InvertedGroupTextAppearance = inverted;
		}
		protected internal AppearanceObject GetItemNormalAppearance(TileBarItem item) {
			AppearanceObject res = new AppearanceObject();
			AppearanceHelper.Combine(res, new AppearanceObject[] { item.Appearance, AppearanceItem.Normal }, GetDefaultAppearanceItem(item));
			if(res.TextOptions.WordWrap == WordWrap.Default)
				res.TextOptions.WordWrap = WordWrap.Wrap;
			return res;
		}
		protected internal AppearanceObject GetItemSelectedAppearance(TileBarItem item) {
			AppearanceObject res = new AppearanceObject();
			AppearanceHelper.Combine(res, new AppearanceObject[] { item.AppearanceSelected, AppearanceItem.Selected, GetItemNormalAppearance(item) }, GetDefaultAppearanceItem(item));
			if(res.TextOptions.WordWrap == WordWrap.Default)
				res.TextOptions.WordWrap = WordWrap.Wrap;
			return res;
		}
		protected internal AppearanceDefault GetDefaultAppearanceItem(TileBarItem item) {
			return item.InvertedAppearance ? DefaultAppearanceItemNormalInverted : DefaultAppearanceItemNormal;
		}
		const string foreColor = "ForeColor";
		const string backColor = "BackColor";
		const string backColorSelected = "BackColorSelected";
		SkinElement GetTileNavPaneElement() {
			return NavPaneSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)["TileNavPane"];
		}
		protected internal Color GetItemForeColor() {
			var elem = GetTileNavPaneElement();
			if(elem != null && elem.Properties.ContainsProperty(foreColor))
				return elem.Properties.GetColor(foreColor);
			return ViewInfo.GetSkinElement(MetroUISkins.SkinActionsBarButton).Color.GetForeColor();
		}
		protected internal Color GetItemBackColor() {
			var elem = GetTileNavPaneElement();
			if(elem != null && elem.Properties.ContainsProperty(backColor))
				return elem.Properties.GetColor(backColor);
			return ViewInfo.GetSkinElement(MetroUISkins.SkinActionsBar).Color.GetBackColor();
		}
		Color GetInvertedItemBackColor() {
			var elem = GetTileNavPaneElement();
			if(elem != null && elem.Properties.ContainsProperty(backColorSelected))
				return elem.Properties.GetColor(backColorSelected);
			return ViewInfo.GetSkinElement(MetroUISkins.SkinActionsBarButton).Color.GetForeColor();
		}
		internal AppearanceDefault DefaultAppearanceItemNormal {
			get {
				return new AppearanceDefault(GetItemForeColor(), GetItemBackColor(), Color.Transparent, AppearanceObject.DefaultFont);
			}
		}
		AppearanceDefault DefaultAppearanceItemNormalInverted {
			get {
				return new AppearanceDefault(GetItemBackColor(), GetInvertedItemBackColor(), Color.Transparent, AppearanceObject.DefaultFont);
			}
		}
		public void ShowDropDown(TileBarItem item) {
			if(item == null) return;
			item.ShowDropDown();
		}
		public TileBarDropDownContainer GetDropDown() {
			var window = (ViewInfoCore as TileBarViewInfo).DropDownWindow;
			if(window == null) return null;
			var containers = window.Controls.OfType<TileBarDropDownContainer>().ToList();
			if(containers.Count == 0) return null;
			return containers[0];
		}
		protected static object dropDownShowingObj = new object();
		public event TileBarDropDownShowingEventHandler DropDownShowing {
			add { Events.AddHandler(dropDownShowingObj, value); }
			remove { Events.RemoveHandler(dropDownShowingObj, value); }
		}
		protected internal TileBarDropDownShowingEventArgs RaiseDropDownShowing(TileBarItem item, TileBarDropDownContainer container) {
			TileBarDropDownShowingEventArgs e = new TileBarDropDownShowingEventArgs() { Item = item, DropDownContainer = container };
			TileBarDropDownShowingEventHandler handler = Events[dropDownShowingObj] as TileBarDropDownShowingEventHandler;
			if(handler != null)
				handler(this, e);
			return e;
		}
		#region Hided properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new public int ColumnCount { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new public Size DragSize { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new public TileItemContentAnimationType ItemContentAnimation { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new public int ScrollButtonFadeAnimationTime { get; set; }
		[Browsable(false)]
		new public bool AllowDrag { get; set; }
		#endregion Hided properties
	}
	public enum ShowDirection { Normal, Inverted }
	public delegate void TileBarDropDownShowingEventHandler(object sender, TileBarDropDownShowingEventArgs e);
	public class TileBarDropDownShowingEventArgs : CancelEventArgs {
		public TileBarItem Item { get; set; }
		public TileBarDropDownContainer DropDownContainer { get; set; }
	}
	[SmartTagFilter(typeof(TileBarItemSmartTagFilter))]
	[Designer("DevExpress.XtraBars.Design.TileBarItemDesigner, " + AssemblyInfo.SRAssemblyBarsDesign)]
	public class TileBarItem : TileItem, ITileBarDropDownOptionsOwner {
		private static readonly object dropDownPress = new object();
		TileBar ControlCore { get { return Control as TileBar; } }
		TileBarDropDownOptions dropDownOptions;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Behavior)]
		public TileBarDropDownOptions DropDownOptions {
			get {
				if(dropDownOptions == null)
					dropDownOptions = new TileBarDropDownOptions(this);
				return dropDownOptions;
			}
		}
		protected override TileItemViewInfo CreateViewInfo() {
			return new TileBarItemViewInfo(this);
		}
		DefaultBoolean showItemShadow = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default), Category(CategoryName.Appearance)]
		public DefaultBoolean ShowItemShadow {
			get { return showItemShadow; }
			set {
				if(showItemShadow == value) return;
				showItemShadow = value;
				OnPropertiesChanged();
			}
		}
		DefaultBoolean showDropDownButton = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ShowDropDownButton {
			get { return showDropDownButton; }
			set {
				if(showDropDownButton == value)
					return;
				showDropDownButton = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(TileItemSize.Default), Category(CategoryName.Appearance), XtraSerializableProperty]
		new public TileBarItemSize ItemSize {
			get { return GetTileBarItemSize(base.ItemSize); }
			set { base.ItemSize = GetItemSize(value); }
		}
		TileBarItemSize GetTileBarItemSize(TileItemSize itemSize) {
			switch(itemSize) {
				case TileItemSize.Large:
				case TileItemSize.Wide:
					return TileBarItemSize.Wide;
				case TileItemSize.Medium:
					return TileBarItemSize.Medium;
				default:
					return TileBarItemSize.Default;
			}
		}
		TileItemSize GetItemSize(TileBarItemSize itemSize) {
			switch(itemSize) {
				case TileBarItemSize.Medium:
					return TileItemSize.Medium;
				case TileBarItemSize.Wide:
					return TileItemSize.Wide;
				default:
					return TileItemSize.Default;
			}
		}
		protected override bool GetIsMedium() {
			return base.ItemSize == TileItemSize.Medium;
		}
		protected override bool GetIsLarge() {
			return base.GetIsLarge() || ItemSize == TileBarItemSize.Default;
		}
		TileBarDropDownContainer dropDownControl;
		[DefaultValue(null), Category(CategoryName.Behavior)]
		public TileBarDropDownContainer DropDownControl {
			get { return dropDownControl; }
			set {
				dropDownControl = value;
				if(dropDownControl != null) dropDownControl.OwnerItem = this;
				OnPropertiesChanged();
			}
		}
		TileBar tileBarCore;
		protected internal TileBar TileBar {
			get { return tileBarCore; }
			set {
				tileBarCore = value;
				OnPropertiesChanged();
			}
		}
		protected internal ITileBarWindowOwner DropDownOwner { get; set; }
		void ITileBarDropDownOptionsOwner.OnDropDownOptionsChanged() {
			OnPropertiesChanged();
		}
		void ITileBarDropDownOptionsOwner.OnDropDownHeightChanged() {
		}
		bool invertedAppearanceCore = false;
		protected internal bool InvertedAppearance {
			get { return invertedAppearanceCore; }
			set {
				if(invertedAppearanceCore == value) return;
				invertedAppearanceCore = value;
				if(ItemInfo != null)
					ItemInfo.ForceUpdateAppearanceColors();
			}
		}
		public void ShowDropDown() {
			var itemInfo = ItemInfo as TileBarItemViewInfo;
			if(itemInfo == null) return;
			var tileBarInfo = itemInfo.ControlInfo as TileBarViewInfo;
			if(tileBarInfo == null) return;
			tileBarInfo.ShowDropDownWindow(itemInfo);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void OnItemClick() {
			var control = Control as TileBar;
			if(control != null && control.CloseDropDownOnItemClick)
				((TileBarViewInfo)control.ViewInfoCore).CloseDropDown();
			base.OnItemClick();
		}
		protected internal TileNavElement NavElement { get; set; }
		protected override void OnVisibilityChanged() {
			base.OnVisibilityChanged();
			UpdateParentDropDown();
		}
		void UpdateParentDropDown() {
			if(NavElement != null) {
				var subItem = NavElement as TileNavSubItem;
				if(subItem != null && subItem.Item != null) {
					subItem.Item.UpdateContainingDropDownItems();
				}
				if(NavElement.TileNavPane != null) {
					NavElement.TileNavPane.OnPropertiesChanged();
				}
			}
		}
		protected override void OnAppearanceChanged(object sender, EventArgs e) {
			var itemInfo = ItemInfo as TileBarItemViewInfo;
			if(itemInfo != null) {
				itemInfo.ResetDropDownAppearance();
				itemInfo.ResetDefaultAppearance();
			}
			base.OnAppearanceChanged(sender, e);
		}
		#region Hidden properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new public bool AllowAnimation { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new public TileItemContentAnimationType ContentAnimation { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new public int CurrentFrameIndex { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new public int FrameAnimationInterval { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new public TileItemContentShowMode TextShowMode { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DefaultValue(null), XtraSerializableProperty(XtraSerializationVisibility.Collection, false, true, false)]
		new public TileItemFrameCollection Frames { get { return base.Frames; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new public TileItemFrame CurrentFrame { get { return base.CurrentFrame; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		new public void NextFrame() { base.NextFrame(); }
		#endregion Hidden properties
	}
	[ToolboxTabName(AssemblyInfo.DXTabNavigation), DXToolboxItem(true)]
	[ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "TileBarDropDownContainer")]
	public class TileBarDropDownContainer : PanelControl {
		public TileBarDropDownContainer() {
			SetStyle(ControlStyles.Selectable, false);
			BorderStyle = XtraEditors.Controls.BorderStyles.NoBorder;
			base.Visible = false;
			this.TabStop = false;
			this.ownerItem = null;
		}
		TileBarItem ownerItem;
		protected override void OnControlAdded(ControlEventArgs e) {
			if(e.Control is Form && ((Form)e.Control).TopLevel)
				throw new InvalidOperationException("TileBarDropDownContainer can't contains Form");
			base.OnControlAdded(e);
		}
		protected override DevExpress.XtraEditors.Controls.BorderStyles DefaultBorderStyle { 
			get { return DevExpress.XtraEditors.Controls.BorderStyles.NoBorder; } 
		}
		protected internal virtual TileBarItem OwnerItem {
			get { return ownerItem; }
			set {
				if(OwnerItem == value) return;
				TileBarItem prev = OwnerItem;
				this.ownerItem = null;
				if(prev != null) prev.DropDownControl = null;
				this.ownerItem = value;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool TabStop {
			get { return base.TabStop; }
			set { base.TabStop = false; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Visible {
			get { return base.Visible; }
			set { base.Visible = false; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Localizable(false)]
		public override DockStyle Dock {
			get { return DockStyle.None; }
		}
		protected internal virtual void SetVisible(bool newVisible) {
			base.Visible = newVisible;
		}
	}
	[SmartTagFilter(typeof(TileBarGroupSmartTagFilter))]
	[Designer("DevExpress.XtraBars.Design.TileBarGroupDesigner, " + AssemblyInfo.SRAssemblyBarsDesign)]
	public class TileBarGroup : TileGroup {
		protected override TileGroupViewInfo CreateViewInfo() {
			return new TileBarGroupViewInfo(this);
		}
	}
	class TileBarItemSmartTagFilter : ISmartTagFilter {
		TileBarItem tileBarItem;
		public bool FilterMethod(string MethodName, object actionMethodItem) {
			if(MethodName == "EditElements") return true;
			return false;
		}
		public bool FilterProperty(MemberDescriptor descriptor) {
			string name = descriptor.Name;
			switch(name) { 
				case "Checked":
				case "TextShowMode":
				case "ContentAnimation":
				case "AllowAnimation": return false;
			}
			return true;
		}
		public void SetComponent(IComponent component) { tileBarItem = component as TileBarItem; }
	}
	class TileBarGroupSmartTagFilter : ISmartTagFilter {
		TileBarGroup tileBarGroup;
		public bool FilterMethod(string MethodName, object actionMethodItem) {
			switch(MethodName) { 
				case "AddSmallItem":
				case "AddLargeItem": return false;
			}
			return true;
		}
		public bool FilterProperty(MemberDescriptor descriptor) {
			return true;
		}
		public void SetComponent(IComponent component) { tileBarGroup = component as TileBarGroup; }
	}
	public interface ITileBarDropDownOptionsOwner {
		void OnDropDownOptionsChanged();
		void OnDropDownHeightChanged();
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class TileBarDropDownOptions {
		public TileBarDropDownOptions(ITileBarDropDownOptionsOwner owner) {
			this.ownerCore = owner;
			height = 0;
			closeOnOuterClick = DefaultBoolean.Default;
		}
		DefaultBoolean autoheight = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean AutoHeight {
			get { return autoheight; }
			set {
				if(autoheight == value) return;
				autoheight = value;
				Owner.OnDropDownHeightChanged();
			}
		}
		DefaultBoolean closeOnOuterClick;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean CloseOnOuterClick {
			get { return closeOnOuterClick; }
			set {
				if(closeOnOuterClick == value) return;
				closeOnOuterClick = value;
			}
		}
		ITileBarDropDownOptionsOwner ownerCore;
		ITileBarDropDownOptionsOwner Owner {
			get { return ownerCore; }
		}
		Color beakColor = Color.Empty;
		[Category(CategoryName.Appearance)]
		public Color BeakColor {
			get { return beakColor; }
			set {
				if(beakColor == value) return;
				beakColor = value;
				Owner.OnDropDownOptionsChanged();
			}
		}
		void ResetBeakColor() { BeakColor = Color.Empty; }
		bool ShouldSerializeBeakColor { get { return BeakColor != Color.Empty; } }
		int height;
		[DefaultValue(0), Category(CategoryName.Appearance)]
		public int Height {
			get { return height; }
			set {
				if(height == value) return;
				height = value;
				Owner.OnDropDownHeightChanged();
			}
		}
		BackColorMode backColorCore = BackColorMode.Default;
		[DefaultValue(BackColorMode.Default)]
		public BackColorMode BackColorMode {
			get { return backColorCore; }
			set {
				if(backColorCore == value) return;
				backColorCore = value;
				Owner.OnDropDownOptionsChanged();
			}
		}
	}
	public enum TileBarItemSize { Default, Medium, Wide }
	public enum BackColorMode { Default, UseTileBackColor, UseBeakColor }
	public enum SelectionColorMode { Default, UseItemBackColor }
	public interface ITileBarOwner {
		void RaiseItemClick(TileItem tileBarItem);
		TileNavElement GetElementByTile(TileItem tileBarItem);
		bool ShowTileToolTips { get; }
	}
}
