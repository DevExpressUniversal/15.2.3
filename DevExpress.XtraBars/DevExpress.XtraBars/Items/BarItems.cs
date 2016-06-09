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
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Collections;
using System.Drawing.Design;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.Utils.Serializing;
using DevExpress.Utils;
using System.ComponentModel.Design;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraBars.Ribbon.Internal;
using DevExpress.Utils.Text.Internal;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Design;
using DevExpress.XtraBars.Helpers;
using DevExpress.XtraEditors;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Menu;
namespace DevExpress.XtraBars {
	public enum CheckBoxVisibility { None, BeforeText, AfterText };
	public enum BarCheckStyles { Standard, Radio }
	[ToolboxItem(false), DesignTimeVisible(false)] 
	public class BarBaseButtonItem : BarItem {
		private static object downChanged = new object();
		bool closeSubMenuOnClick, allowAllUp;
		protected bool fDown;
		int groupIndex;
		public BarBaseButtonItem() {
			allowAllUp = false;
			fDown = false;
			closeSubMenuOnClick = true;
			groupIndex = 0;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public virtual BarButtonStyle ButtonStyle { get { return BarButtonStyle.Default; } set { } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarBaseButtonItemCloseSubMenuOnClick"),
#endif
 DefaultValue(true), Category("Behavior")]
		public virtual bool CloseSubMenuOnClick {
			get { return closeSubMenuOnClick; }
			set {
				if(CloseSubMenuOnClick == value) return;
				closeSubMenuOnClick = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarBaseButtonItemAllowAllUp"),
#endif
 DefaultValue(false), Category("Behavior")]
		public virtual bool AllowAllUp {
			get { return allowAllUp; }
			set {
				if(AllowAllUp == value) return;
				allowAllUp = value;
				if(GroupIndex != 0)
					SetAllowAllUp(value);
			}
		}
		[Browsable(false)]
		public virtual bool CanDown { get { return IsCheckButtonStyle; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarBaseButtonItemDown"),
#endif
 DefaultValue(false), Category("Appearance")]
		public virtual bool Down {
			get { return !CanDown ? false : fDown; }
			set {
				if(Down == value) return;
				if(!CanDown && value) return;
				if(GroupIndex != 0) {
					if(!AllowAllUp) {
						if(!value) return;
						SetDown(value);
						UnToggleAllItems();
					} else {
						SetDown(value);
						if(value) UnToggleAllItems(); ;
					}
				}
				SetDown(value);
				RaiseDownChanged();
				OnItemChanged(true);
			}
		}
		protected virtual void SetDown(bool value) {
			fDown = value;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarBaseButtonItemGroupIndex"),
#endif
 DefaultValue(0), Category("Behavior")]
		public virtual int GroupIndex {
			get { return groupIndex; }
			set {
				if(GroupIndex == value) return;
				groupIndex = value;
				OnItemChanged();
			}
		}
		protected internal virtual bool IsCheckButtonStyle {
			get {
				return ButtonStyle == BarButtonStyle.Check || ButtonStyle == BarButtonStyle.CheckDropDown;
			}
		}
		protected internal virtual bool IsDropDownButtonStyle {
			get {
				return ButtonStyle == BarButtonStyle.DropDown || ButtonStyle == BarButtonStyle.CheckDropDown;
			}
		}
		protected internal override BarItemPaintStyle CalcRealPaintStyle(BarItemLink link) {
			BarItemPaintStyle ps = link == null ? PaintStyle : link.PaintStyle;
			if(ps == BarItemPaintStyle.Standard) {
				if(link.IsLinkInMenu)
					ps = BarItemPaintStyle.CaptionGlyph;
				else {
					ps = BarItemPaintStyle.CaptionInMenu;
					if(!link.IsImageExist)
						ps = BarItemPaintStyle.Caption;
				}
			}
			return ps;
		}
		protected void SetAllowAllUp(bool value) {
			foreach(BarItem item in Manager.Items) {
				if(!(item is BarBaseButtonItem)) continue;
				BarBaseButtonItem bt = item as BarBaseButtonItem;
				if(bt.GroupIndex == GroupIndex) {
					bt.allowAllUp = value;
				}
			}
		}
		protected void UnToggleAllItems() {
			if(Manager == null) return;
			foreach(BarItem item in Manager.Items) {
				if(!(item is BarBaseButtonItem)) continue;
				BarBaseButtonItem bt = item as BarBaseButtonItem;
				if(bt.GroupIndex == GroupIndex && bt.Down) {
					bt.fDown = false;
					bt.OnItemChanged(true);
				}
			}
		}
		protected internal override bool CanCloseSubOnClick(BarItemLink link) { 
			return CloseSubMenuOnClick; 
		}
		protected internal override void OnClick(BarItemLink link) {
			if(IsCheckButtonStyle)
				Toggle(link);
			base.OnClick(link);
		}
		BarItemLink toggleLink = null;
		protected BarItemLink LastToggleLink { get { return toggleLink; } }
		public virtual void Toggle(BarItemLink link) {
			if(!Enabled || (Manager != null && Manager.IsCustomizing)) return;
			toggleLink = link;
			Down = !Down;
			toggleLink = null;
		}
		public virtual void Toggle() {
			Toggle(null);
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarBaseButtonItemDownChanged"),
#endif
 Category("Events")]
		public event ItemClickEventHandler DownChanged {
			add { Events.AddHandler(downChanged, value); }
			remove { Events.RemoveHandler(downChanged, value); }
		}
		protected internal virtual void RaiseDownChanged() {
			ItemClickEventHandler handler = (ItemClickEventHandler)Events[downChanged];
			if(handler != null) handler(this, new ItemClickEventArgs(this, toggleLink));
		}
		protected void EnsureRibbonInitialized() {
			Ribbon.RibbonBarManager ribbonManager = Manager as Ribbon.RibbonBarManager;
			if(ribbonManager != null && !ribbonManager.GetIsInitialized() && ribbonManager.Ribbon != null)
				ribbonManager.Ribbon.ForceInitialize();
		}
	}
	[DXToolboxItem(false), DesignTimeVisible(false), SmartTagAction(typeof(BarButtonItemActionsProvider), "AddDropDownMenu", "Add Dropdown Menu",
		SmartTagActionType.RefreshAfterExecute), SmartTagAction(typeof(BarButtonItemActionsProvider), "AddDropDownPopupControlContainer", "Add Dropdown PopupControlContainer",
		SmartTagActionType.RefreshAfterExecute),
	SmartTagAction(typeof(BarButtonItemActionsProvider), "AddGalleryDropDown", "Add Dropdown Gallery",
		SmartTagActionType.RefreshAfterExecute),
	SmartTagFilter(typeof(BarButtonItemFilter))]
	public class BarButtonItem : BarBaseButtonItem, DevExpress.Utils.MVVM.ISupportCommandBinding {
		BarButtonStyle buttonStyle;
		PopupControl dropDownControl;
		bool dropDownEnabled, actAsDropDown;
		bool closeRadialMenuOnItemClick;
		public BarButtonItem() { 
			this.dropDownEnabled = true;
			this.actAsDropDown = false;
			this.dropDownControl = null;
			this.buttonStyle = BarButtonStyle.Default;
			this.closeRadialMenuOnItemClick = false;
			RememberLastCommand = false;
		}
		public BarButtonItem(BarManager manager, string caption) : this(manager, caption, -1) {
		}
		public BarButtonItem(BarManager manager, string caption, int imageIndex) : this() {
			Caption = caption;
			ImageIndex = imageIndex;
			Manager = manager;
		}
		public BarButtonItem(BarManager manager, string caption, int imageIndex, BarShortcut shortcut) : this() {
			Caption = caption;
			ImageIndex = imageIndex;
			ItemShortcut = shortcut;
			Manager = manager;
		}
		protected internal override void OnClick(BarItemLink link) {
			if(LastClickedLink != null) {
				if(IsLastCommandValid(LastClickedLink)) LastClickedLink.Item.OnClick(LastClickedLink);
				if(IsCheckButtonStyle) base.OnClick(link);
			}
			else {
				base.OnClick(link);
			}
		}
		protected virtual bool IsLastCommandValid(BarItemLink link) {
			if(!link.Enabled || (IsCheckButtonStyle && Down)) return false;
			return true;
		}
		bool allowDrawArrrowInMenu = true;
		[DefaultValue(true)]
		public bool AllowDrawArrowInMenu {
			get { return allowDrawArrrowInMenu; }
			set {
				if(AllowDrawArrowInMenu == value)
					return;
				allowDrawArrrowInMenu = value;
				OnItemChanged();
			}
		}
		bool allowDrawArrrow = true;
		[DefaultValue(true)]
		public bool AllowDrawArrow {
			get { return allowDrawArrrow; }
			set {
				if(AllowDrawArrow == value)
					return;
				allowDrawArrrow = value;
				OnItemChanged();
			}
		}
		public override SuperToolTip SuperTip {
			get {
				if(LastClickedLink != null) return LastClickedLink.Item.SuperTip;
				return base.SuperTip;
			}
			set {
				base.SuperTip = value;
			}
		}
		public override string Hint {
			get {
				if(LastClickedLink != null) return LastClickedLink.Item.Hint;
				return base.Hint;
			}
			set {
				base.Hint = value;
			}
		}
		public override string Description {
			get {
				if(LastClickedLink != null) return LastClickedLink.Item.Description;
				return base.Description;
			}
			set {
				base.Description = value;
			}
		}
		public override string Caption {
			get {
				if(LastClickedLink != null) return LastClickedLink.Item.Caption;
				return base.Caption;
			}
			set {
				base.Caption = value;
			}
		}
		public override Image Glyph {
			get {
				if(LastClickedLink != null) return LastClickedLink.Item.Glyph;
				return base.Glyph;
			}
			set {
				base.Glyph = value;
			}
		}
		public override Image GlyphDisabled {
			get {
				if(LastClickedLink != null) return LastClickedLink.Item.GlyphDisabled;
				return base.GlyphDisabled;
			}
			set {
				base.GlyphDisabled = value;
			}
		}
		public override Image LargeGlyph {
			get {
				if(LastClickedLink != null) return LastClickedLink.Item.LargeGlyph;
				return base.LargeGlyph;
			}
			set {
				base.LargeGlyph = value;
			}
		}
		public override Image LargeGlyphDisabled {
			get {
				if(LastClickedLink != null) return LastClickedLink.Item.LargeGlyphDisabled;
				return base.LargeGlyphDisabled;
			}
			set {
				base.LargeGlyphDisabled = value;
			}
		}
		public override DxImageUri ImageUri {
			get {
				if(LastClickedLink != null && LastClickedLink.Item != null) return LastClickedLink.Item.ImageUri;
				return base.ImageUri;
			}
			set { base.ImageUri = value; }
		}
		public override int ImageIndex {
			get {
				if(LastClickedLink != null) return LastClickedLink.Item.ImageIndex;
				return base.ImageIndex;
			}
			set {
				base.ImageIndex = value;
			}
		}
		public override int LargeImageIndex {
			get {
				if(LastClickedLink != null) return LastClickedLink.Item.LargeImageIndex;
				return base.LargeImageIndex;
			}
			set {
				base.LargeImageIndex = value;
			}
		}
		internal BarButtonItem(BarManager manager, bool privateItem) : this() {
			this.fIsPrivateItem = privateItem;
			this.Manager = manager;
		}
		BarItemLink lastClickedLink;
		protected internal BarItemLink LastClickedLink {
			get {
				if(lastClickedLink == null)
					lastClickedLink = GetDefaultLastClickedLink();
				return lastClickedLink;
			}
			set {
				if(LastClickedLink == value)
					return;
				BarItemLink prev = lastClickedLink;
				lastClickedLink = value;
				OnLastClickedLinkChanged(prev, value);
			}
		}
		protected virtual BarItemLink GetDefaultLastClickedLink() {
			return null;
		}
		protected virtual void OnLastClickedLinkChanged(BarItemLink prev, BarItemLink next) {
			if(prev != null) prev.LastCommandOwnerItem = null;
			if(next != null) next.LastCommandOwnerItem = this;
			OnItemChanged();
		}
		[DefaultValue(false)]
		public bool RememberLastCommand { get; set; }
		protected internal virtual bool CanAddItemToMenu(BarItemLink link) { 
			PopupMenu menu = DropDownControl as PopupMenu;
			if(menu == null) return true;
			if(link.Holder == menu) return false;
			return true;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarButtonItemActAsDropDown")
#else
	Description("")
#endif
, Category("Behavior")]
		public virtual bool ActAsDropDown {
			get { return actAsDropDown; }
			set {
				if(ActAsDropDown == value) return;
				actAsDropDown = value;
				OnItemChanged();
			}
		}
		protected internal virtual bool ShouldSerializeActAsDropDown() {
			return ActAsDropDown != false;
		}
		[Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarButtonItemButtonStyle"),
#endif
  Category("Appearance")]
		public override BarButtonStyle ButtonStyle {
			get { return buttonStyle; }
			set {
				if(ButtonStyle == value) return;
				buttonStyle = value;
				OnItemChanged();
			}
		}
		protected internal virtual bool ShouldSerializeButtonStyle() {
			return ButtonStyle != BarButtonStyle.Default;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarButtonItemDropDownControl"),
#endif
 DefaultValue(null), Category("Behavior")]
		public virtual PopupControl DropDownControl {
			get { return dropDownControl; }
			set {
				if(DropDownControl == value) return;
				dropDownControl = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarButtonItemDropDownEnabled"),
#endif
 DefaultValue(true), Category("Behavior")]
		public virtual bool DropDownEnabled {
			get { return dropDownEnabled; }
			set {
				if(DropDownEnabled == value) return;
				dropDownEnabled = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarButtonItemCloseRadialMenuOnItemClick"),
#endif
 DefaultValue(false), Category("Behavior")]
		public virtual bool CloseRadialMenuOnItemClick {
			get { return closeRadialMenuOnItemClick; }
			set {
				if(closeRadialMenuOnItemClick == value) return;
				closeRadialMenuOnItemClick = value;
			}
		}
		[Browsable(false)]
		public virtual bool CanPressDropDownButton {
			get { return Enabled && DropDownControl != null && DropDownEnabled && IsDropDownButtonStyle; }
		}
		protected internal override bool CanCloseSubOnClick(BarItemLink link) {
			if(CloseSubMenuOnClick && (!ActAsDropDown)) {
				if(IsCheckButtonStyle) {
					return link.RibbonItemInfo == null;
				}
				return true;
			}
			return false;
		}
		#region Commands
		public IDisposable BindCommand(object command, Func<object> queryCommandParameter = null) {
			EnsureRibbonInitialized();
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(barButtonItem, execute) => barButtonItem.ItemClick += (s, e) => execute(),
				(barButtonItem, canExecute) => barButtonItem.Enabled = canExecute(),
				command, queryCommandParameter);
		}
		public IDisposable BindCommand(System.Linq.Expressions.Expression<Action> commandSelector, object source, Func<object> queryCommandParameter = null) {
			EnsureRibbonInitialized();
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(barButtonItem, execute) => barButtonItem.ItemClick += (s, e) => execute(),
				(barButtonItem, canExecute) => barButtonItem.Enabled = canExecute(),
				commandSelector, source, queryCommandParameter);
		}
		public IDisposable BindCommand<T>(System.Linq.Expressions.Expression<Action<T>> commandSelector, object source, Func<T> queryCommandParameter = null) {
			EnsureRibbonInitialized();
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(barButtonItem, execute) => barButtonItem.ItemClick += (s, e) => execute(),
				(barButtonItem, canExecute) => barButtonItem.Enabled = canExecute(),
				commandSelector, source, () => (queryCommandParameter != null) ? queryCommandParameter() : default(T));
		}
		#endregion Commands
		protected override void Dispose(bool disposing) {
			if(disposing) {
				this.lastClickedLink = null;
			}
			base.Dispose(disposing);
		}
	}
	[ToolboxItem(false), DesignTimeVisible(false), DefaultEvent("CheckedChanged")]
	public class BarToggleSwitchItem : BarItem {
		private static readonly object checkedChanged = new object();
		private static readonly object bindableCheckedChanged = new object();
		bool isChecked;
		[DefaultValue(false), Bindable(false), Category("Behavior")]
		public bool Checked {
			get { return isChecked; }
			set { 
				if(Checked == value)
					return;
				isChecked = value;
				OnItemChanged();
				RaiseBindableCheckedChanged();
				RaiseCheckedChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarToggleSwitchItemBindableChecked"),
#endif
 DefaultValue(false), Category("Behavior"), Bindable(true), Browsable(false)]
		public virtual bool BindableChecked {
			get { return Checked; }
			set { Checked = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarToggleSwitchItemBindableCheckedChanged"),
#endif
 Category("Events"), Browsable(false)]
		public event EventHandler BindableCheckedChanged {
			add { Events.AddHandler(bindableCheckedChanged, value); }
			remove { Events.RemoveHandler(bindableCheckedChanged, value); }
		}
		protected internal virtual void RaiseBindableCheckedChanged() {
			EventHandler handler = (EventHandler)Events[bindableCheckedChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarCheckItemCheckedChanged"),
#endif
 Category("Events")]
		public event ItemClickEventHandler CheckedChanged {
			add { Events.AddHandler(checkedChanged, value); }
			remove { Events.RemoveHandler(checkedChanged, value); }
		}
		protected internal virtual void RaiseCheckedChanged() {
			ItemClickEventHandler handler = (ItemClickEventHandler)Events[checkedChanged];
			if(handler != null) handler(this, new ItemClickEventArgs(this, null));
		}
	}
	[ToolboxItem(false), DesignTimeVisible(false), DefaultEvent("CheckedChanged"),]
	public class BarCheckItem : BarBaseButtonItem, DevExpress.Utils.MVVM.ISupportCommandBinding {
		private static object checkedChanged = new object();
		private static object bindableCheckedChanged = new object();
		public BarCheckItem() {
			checkBoxVisibility = CheckBoxVisibility.None;
		}
		public BarCheckItem(BarManager manager) : this() {
			Manager = manager;
		}
		public BarCheckItem(BarManager manager, bool check) : this() {
			Down = check;
			Manager = manager;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override BarButtonStyle ButtonStyle { 
			get { return BarButtonStyle.Check; }
			set { }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarCheckItemChecked"),
#endif
 DefaultValue(false), Category("Behavior"), SmartTagProperty("Checked", "Behavior", 2), Bindable(false)]
		public virtual bool Checked {
			get { return Down; }
			set { Down = value; }
		}
		private CheckBoxVisibility checkBoxVisibility;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarCheckItemCheckBoxVisibility"),
#endif
 DefaultValue(CheckBoxVisibility.None), Category("Appearance"), SmartTagProperty("Check Box Visibility", "Appearance", 0)]
		public virtual CheckBoxVisibility CheckBoxVisibility {
			get { return checkBoxVisibility; }
			set {
				if(CheckBoxVisibility == value) return;
				checkBoxVisibility = value;
				OnItemChanged();
			}
		}
		protected internal CheckStyles GetCheckStyle() {
			if(CheckStyle == BarCheckStyles.Standard)
				return CheckStyles.Standard;
			return CheckStyles.Radio;
		}
		private BarCheckStyles checkStyle = BarCheckStyles.Standard;
		[DefaultValue(BarCheckStyles.Standard), SmartTagProperty("Check Box Style", "Appearance", 1)]
		public BarCheckStyles CheckStyle {
			get { return checkStyle; }
			set {
				if(CheckStyle == value)
					return;
				checkStyle = value;
				OnItemChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Down {
			get { return base.Down; }
			set { base.Down = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarCheckItemBindableChecked"),
#endif
 DefaultValue(false), Category("Behavior"), Bindable(true), Browsable(false)]
		public virtual bool BindableChecked {
			get { return Checked; }
			set { Checked = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarCheckItemBindableCheckedChanged"),
#endif
 Category("Events"), Browsable(false)]
		public event EventHandler BindableCheckedChanged {
			add { Events.AddHandler(bindableCheckedChanged, value); }
			remove { Events.RemoveHandler(bindableCheckedChanged, value); }
		}
		protected internal virtual void RaiseBindableCheckedChanged() {
			EventHandler handler = (EventHandler)Events[bindableCheckedChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal override void RaiseDownChanged() {
			RaiseBindableCheckedChanged();
			RaiseCheckedChanged();
			base.RaiseDownChanged();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarCheckItemCheckedChanged"),
#endif
 Category("Events")]
		public event ItemClickEventHandler CheckedChanged {
			add { Events.AddHandler(checkedChanged, value); }
			remove { Events.RemoveHandler(checkedChanged, value); }
		}
		protected internal virtual void RaiseCheckedChanged() {
			ItemClickEventHandler handler = (ItemClickEventHandler)Events[checkedChanged];
			if(handler != null) handler(this, new ItemClickEventArgs(this, LastToggleLink));
		}
		#region Commands
		public IDisposable BindCommand(object command, Func<object> queryCommandParameter = null) {
			EnsureRibbonInitialized();
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(barCheckItem, execute) => barCheckItem.ItemClick += (s, e) => execute(),
				(barCheckItem, canExecute) => barCheckItem.Checked = !canExecute(),
				command, queryCommandParameter);
		}
		public IDisposable BindCommand(System.Linq.Expressions.Expression<Action> commandSelector, object source, Func<object> queryCommandParameter = null) {
			EnsureRibbonInitialized();
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(barCheckItem, execute) => barCheckItem.ItemClick += (s, e) => execute(),
				(barCheckItem, canExecute) => barCheckItem.Checked = !canExecute(),
				commandSelector, source, queryCommandParameter);
		}
		public IDisposable BindCommand<T>(System.Linq.Expressions.Expression<Action<T>> commandSelector, object source, Func<T> queryCommandParameter = null) {
			EnsureRibbonInitialized();
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(barCheckItem, execute) => barCheckItem.ItemClick += (s, e) => execute(),
				(barCheckItem, canExecute) => barCheckItem.Checked = !canExecute(),
				commandSelector, source, () => (queryCommandParameter != null) ? queryCommandParameter() : default(T));
		}
		#endregion Commands
	}
	[ToolboxItem(false), DesignTimeVisible(false)] 
	public class BarLargeButtonItem : BarButtonItem {
		bool showCaptionOnBar;
		BarItemCaptionAlignment captionAlignment;
		int largeImageIndexHot;
		Image largeGlyphHot;
		Size minSize;
		public BarLargeButtonItem() {
			this.minSize = Size.Empty;
			this.showCaptionOnBar = true;
			this.captionAlignment = BarItemCaptionAlignment.Bottom;
			this.largeImageIndexHot = -1;
			this.largeGlyphHot = null;
		}
		public BarLargeButtonItem(BarManager manager, string caption) : this(manager, caption, -1) {
		}
		public BarLargeButtonItem(BarManager manager, string caption, int imageIndex) : this() {
			Caption = caption;
			ImageIndex = imageIndex;
			Manager = manager;
		}
		public BarLargeButtonItem(BarManager manager, string caption, int imageIndex, BarShortcut shortcut) : this() {
			Caption = caption;
			ImageIndex = imageIndex;
			ItemShortcut = shortcut;
			Manager = manager;
		}
		protected override RibbonItemStyles GetRibbonDefaultAllowedStyles(RibbonItemViewInfo itemInfo) {
			return RibbonItemStyles.All;
		}
		bool ShouldSerializeMinSize() { return !MinSize.IsEmpty; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarLargeButtonItemMinSize"),
#endif
 Category("Appearance")]
		public Size MinSize {
			get { return minSize; }
			set {
				value.Width = Math.Max(0, value.Width);
				value.Height = Math.Max(0, value.Height);
				if(value == MinSize) return;
				minSize = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarLargeButtonItemShowCaptionOnBar"),
#endif
 DefaultValue(true), Category("Appearance")]
		public virtual bool ShowCaptionOnBar {
			get { return showCaptionOnBar; }
			set {
				if(ShowCaptionOnBar == value) return;
				showCaptionOnBar = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarLargeButtonItemCaptionAlignment"),
#endif
 DefaultValue(BarItemCaptionAlignment.Bottom), Category("Appearance")]
		public virtual BarItemCaptionAlignment CaptionAlignment {
			get { return captionAlignment; }
			set {
				if(CaptionAlignment == value) return;
				captionAlignment = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarLargeButtonItemLargeImageIndexHot"),
#endif
 DefaultValue(-1), Category("Appearance"), Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(UITypeEditor)), DevExpress.Utils.ImageList("LargeImages")]
		public virtual int LargeImageIndexHot {
			get { return largeImageIndexHot; }
			set {
				if(LargeImageIndexHot == value) return;
				largeImageIndexHot = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarLargeButtonItemLargeGlyphHot"),
#endif
 DefaultValue(null), Category("Appearance"), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image LargeGlyphHot {
			get { return largeGlyphHot; }
			set {
				if(LargeGlyphHot == value) return;
				largeGlyphHot = UpdateImage(value);
				OnItemChanged();
			}
		}
		protected internal override BarItemPaintStyle CalcRealPaintStyle(BarItemLink lnk) {
			BarLargeButtonItemLink link = lnk as BarLargeButtonItemLink;
			if(link.IsLinkInMenu || !link.IsLargeImageExist) return base.CalcRealPaintStyle(link);
			BarItemPaintStyle ps = link == null ? PaintStyle : link.PaintStyle;
			if(ps == BarItemPaintStyle.Standard) {
				if(link.IsLinkInMenu)
					ps = BarItemPaintStyle.CaptionGlyph;
				else {
					ps = BarItemPaintStyle.CaptionInMenu;
					if(!link.IsLargeImageExist)
						ps = BarItemPaintStyle.Caption;
				}
			}
			if(!link.IsLinkInMenu) {
				if(ShowCaptionOnBar) {
					switch(ps) {
						case BarItemPaintStyle.CaptionInMenu:
							ps = BarItemPaintStyle.CaptionGlyph;
							break;
					}
				} else {
					switch(ps) {
						case BarItemPaintStyle.CaptionGlyph:
							ps = BarItemPaintStyle.CaptionInMenu;
							break;
					}
				}
			}
			return ps;
		}
	}
	#region BarHeaderItem
	public class BarHeaderItem : BarStaticItem, IOptionsMultiColumnOwner {
		private StringAlignment textAlignment;
		bool showImageInToolbar;
		internal BarHeaderItem(BarManager barManager, bool privateItem)
			: base() {
			this.fIsPrivateItem = privateItem;
			this.Manager = barManager;
			this.textAlignment = StringAlignment.Far;
			this.showImageInToolbar = false;
		}
		public BarHeaderItem() {
			this.textAlignment = StringAlignment.Far;
			this.showImageInToolbar = false;
		}
		OptionsMultiColumn optionsMultiColumn;
		void ResetOptionsMultiColumn() { OptionsMultiColumn.Reset(); }
		bool ShouldSerializeOptionsMultiColumn() { return OptionsMultiColumn.ShouldSerializeCore(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OptionsMultiColumn OptionsMultiColumn {
			get {
				if(optionsMultiColumn == null)
					optionsMultiColumn = new OptionsMultiColumn(this);
				return optionsMultiColumn;
			}
		}
		DefaultBoolean multiColumn = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean MultiColumn {
			get { return multiColumn; }
			set {
				if(MultiColumn == value)
					return;
				multiColumn = value;
				OnItemChanged();
			}
		}
		protected internal override Design.BarLinkInfoProvider CreateLinkInfoProvider(BarItemLink link) {
			return new BarHeaderItemLinkInfoProvider(link);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BorderStyles Border {
			get {
				return base.Border;
			}
			set {
				base.Border = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarStaticItemSize AutoSize {
			get {
				return BarStaticItemSize.Content;
			}
			set {
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Image Glyph {
			get {
				return base.Glyph;
			}
			set {
				base.Glyph = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Image GlyphDisabled {
			get {
				return base.GlyphDisabled;
			}
			set {
				base.GlyphDisabled = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DxImageUri ImageUri {
			get { return base.ImageUri; }
			set { base.ImageUri = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Hint {
			get {
				return base.Hint;
			}
			set {
				base.Hint = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean AllowGlyphSkinning {
			get {
				return base.AllowGlyphSkinning;
			}
			set {
				base.AllowGlyphSkinning = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Image LargeGlyph {
			get {
				return base.LargeGlyph;
			}
			set {
				base.LargeGlyph = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Image LargeGlyphDisabled {
			get {
				return base.LargeGlyphDisabled;
			}
			set {
				base.LargeGlyphDisabled = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarStaticItemTextAlignment"),
#endif
 Category("Appearance"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new public virtual StringAlignment TextAlignment {
			get { return textAlignment; }
			set {
				if (TextAlignment == value) return;
				textAlignment = value;
				OnItemChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new public virtual bool ShowImageInToolbar {
			get { return showImageInToolbar; }
			set {
				if (ShowImageInToolbar == value) return;
				showImageInToolbar = value;
				OnItemChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarItemLinkAlignment Alignment {
			get {
				return base.Alignment;
			}
			set {
				base.Alignment = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarShortcut ItemShortcut {
			get {
				return base.ItemShortcut;
			}
			set {
				base.ItemShortcut = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string ShortcutKeyDisplayString {
			get {
				return base.ShortcutKeyDisplayString;
			}
			set {
				base.ShortcutKeyDisplayString = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int ImageIndex {
			get {
				return base.ImageIndex;
			}
			set {
				base.ImageIndex = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int ImageIndexDisabled {
			get {
				return base.ImageIndexDisabled;
			}
			set {
				base.ImageIndexDisabled = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int LargeImageIndex {
			get {
				return base.LargeImageIndex;
			}
			set {
				base.LargeImageIndex = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int LargeImageIndexDisabled {
			get {
				return base.LargeImageIndexDisabled;
			}
			set {
				base.LargeImageIndexDisabled = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarItemAppearances ItemAppearance {
			get {
				return base.ItemAppearance;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarItemAppearances ItemInMenuAppearance {
			get {
				return base.ItemInMenuAppearance;
			}
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override BarItemAppearance Appearance {
			get {
				return (BarItemAppearance)ItemInMenuAppearance.Normal;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool CausesValidation {
			get {
				return base.CausesValidation;
			}
			set {
				base.CausesValidation = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Enabled {
			get {
				return base.Enabled;
			}
			set {
				base.Enabled = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int LeftIndent {
			get {
				return base.LeftIndent;
			}
			set {
				base.LeftIndent = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int RightIndent {
			get {
				return base.RightIndent;
			}
			set {
				base.RightIndent = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarItemPaintStyle PaintStyle {
			get {
				return base.PaintStyle;
			}
			set {
				base.PaintStyle = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SuperToolTip SuperTip {
			get {
				return base.SuperTip;
			}
			set {
				base.SuperTip = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Size Size {
			get {
				return base.Size;
			}
			set {
				base.Size = value;
			}
		}
		void IOptionsMultiColumnOwner.OnChanged() {
			OnItemChanged();
		}
	}
	#endregion
	#region BarStaticItem
	public class BarStaticItem : BarItem {
		internal int width;
		private int leftIndent, rightIndent;
		private StringAlignment textAlignment;
		private BarStaticItemSize autoSize;
		bool showImageInToolbar;
		public BarStaticItem() {
			this.autoSize = BarStaticItemSize.Content;
			this.width = 0;
			this.leftIndent = this.rightIndent = 0;
			this.textAlignment = StringAlignment.Near;
			this.showImageInToolbar = true;
		}
		protected override BorderStyles DefaultBorder { get { return BorderStyles.Default; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int LargeWidth {
			get { return base.LargeWidth; }
			set { base.LargeWidth = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int SmallWithoutTextWidth {
			get { return base.SmallWithoutTextWidth; }
			set { base.SmallWithoutTextWidth = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int SmallWithTextWidth {
			get { return Size.Width; }
			set { Size = new Size(value, Size.Height); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarStaticItemAutoSize"),
#endif
 DefaultValue(BarStaticItemSize.Content), Category("Behavior")]
		public virtual BarStaticItemSize AutoSize {
			get { return autoSize; }
			set {
				if(AutoSize == value) return;
				autoSize = value;
				if(AutoSize == BarStaticItemSize.Content)
					width = 0;
				else {
					if(width == 0)
						width = 32;
				}
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarStaticItemLeftIndent"),
#endif
 DefaultValue(0), Category("Appearance")]
		public virtual int LeftIndent {
			get { return leftIndent; }
			set {
				if(value < 0) value = 0;
				if(LeftIndent == value) return;
				leftIndent = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarStaticItemRightIndent"),
#endif
 DefaultValue(0), Category("Appearance")]
		public virtual int RightIndent {
			get { return rightIndent; }
			set {
				if(value < 0) value = 0;
				if(RightIndent == value) return;
				rightIndent = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarStaticItemTextAlignment"),
#endif
 Category("Appearance")]
		public virtual StringAlignment TextAlignment {
			get { return textAlignment; }
			set {
				if(TextAlignment == value) return;
				textAlignment = value;
				OnItemChanged();
			}
		}
		protected internal override BarItemPaintStyle CalcRealPaintStyle(BarItemLink link) {
			BarItemPaintStyle ps = link == null ? PaintStyle : link.PaintStyle;
			if(ps == BarItemPaintStyle.Standard) {
				if(link.IsLinkInMenu)
					ps = BarItemPaintStyle.CaptionGlyph;
				else {
					ps = BarItemPaintStyle.Caption;
				}
			}
			return ps;
		}
		[DefaultValue(true), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarStaticItemShowImageInToolbar"),
#endif
 Category("Appearance")]
		public virtual bool ShowImageInToolbar {
			get { return showImageInToolbar; }
			set {
				if(ShowImageInToolbar == value) return;
				showImageInToolbar = value;
				OnItemChanged();
			}
		}
	}
	#endregion
	#region BarEditItem
	public class BarEditItem : BarItem {
		bool visibleWhenVertical, canOpenEdit, autoFillWidth, ieBehavior, autoHideEdit;
		DefaultBoolean autoFillWidthInMenu;
		internal DevExpress.XtraEditors.Repository.RepositoryItem edit;
		int editHeight;
		object editValue;
		HorzAlignment captionAlignment;
		EditorShowMode editorShowMode;
		DefaultBoolean closeOnMouseOuterClick = DefaultBoolean.Default;
		Padding editorPadding;
		protected BarEditItem(BarManager manager, string editorName) : base(manager) {
			this.autoHideEdit = true;
			this.ieBehavior = false;
			this.autoFillWidth = false;
			this.autoFillWidthInMenu = DefaultBoolean.Default;
			this.editHeight = -1;
			this.canOpenEdit = true;
			this.visibleWhenVertical = false;
			this.edit = null;
			this.editValue = null;
			this.editorShowMode = EditorShowMode.Default;
			this.captionAlignment = HorzAlignment.Default;
			this.editorPadding = new Padding();
			if(Manager != null && editorName != null && editorName.Length > 0) {
				Edit = Manager.Helper.CreateLinkEditor(editorName);
			}
		}
		public BarEditItem(BarManager manager, RepositoryItem edit) : this(manager, string.Empty) { 
			Edit = edit;
		}
		public BarEditItem(BarManager manager) : this(manager, string.Empty) { }
		public BarEditItem() : this(null, string.Empty) { }
		internal BarEditItem(bool isPrivateItem) : this() {
			this.fIsPrivateItem = isPrivateItem;
		}
		protected override void Dispose(bool disposing) {
			if(Edit != null)
				Edit.Disconnect(this);
			base.Dispose(disposing);
		}
		protected override bool UpdateLayoutOnEnabledChanged {
			get { return true; }
		}
		public override int Width {
			get { return EditWidth; }
			set { EditWidth = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Padding EditorPadding {
			get { return editorPadding; }
			set { editorPadding = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DefaultBoolean CloseOnMouseOuterClick {
			get { return closeOnMouseOuterClick; }
			set { closeOnMouseOuterClick = value; }
		}
		protected internal override RibbonItemStyles GetRibbonAllowedStyles(RibbonItemViewInfo itemInfo) {
			return base.GetRibbonAllowedStyles(itemInfo) & (~(RibbonItemStyles.Large));
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int LargeWidth {
			get { return base.LargeWidth; }
			set { base.LargeWidth = value;}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int SmallWithTextWidth {
			get { return base.SmallWithTextWidth; }
			set { base.SmallWithTextWidth = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int SmallWithoutTextWidth {
			get { return base.SmallWithoutTextWidth; }
			set { base.SmallWithoutTextWidth = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarEditItemIEBehavior"),
#endif
 DefaultValue(false), Category("Behavior")]
		public virtual bool IEBehavior {
			get { return ieBehavior; }
			set {
				if(IEBehavior == value) return;
				ieBehavior = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarEditItemEditorShowMode"),
#endif
 DefaultValue(EditorShowMode.Default), Category("Behavior")]
		public EditorShowMode EditorShowMode {
			get { return editorShowMode; }
			set {
				editorShowMode = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarEditItemCaptionAlignment"),
#endif
 DefaultValue(HorzAlignment.Default), Category("Behavior")]
		public HorzAlignment CaptionAlignment {
			get { return captionAlignment; }
			set {
				if(CaptionAlignment == value) return;
				captionAlignment = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarEditItemAutoFillWidth"),
#endif
 DefaultValue(false), Category("Appearance"), SupportedByRibbon(SupportedByRibbonKind.NonSupported)]
		public virtual bool AutoFillWidth {
			get { return autoFillWidth; }
			set {
				if(AutoFillWidth == value) return;
				autoFillWidth = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarEditItemAutoFillWidthInMenu"),
#endif
 DefaultValue(DefaultBoolean.Default), Category("Appearance")]
		public virtual DefaultBoolean AutoFillWidthInMenu {
			get { return autoFillWidthInMenu; }
			set {
				if(AutoFillWidthInMenu == value) return;
				autoFillWidthInMenu = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarEditItemAutoHideEdit"),
#endif
 DefaultValue(true), Category("Behavior")]
		public virtual bool AutoHideEdit {
			get { return autoHideEdit; }
			set {
				autoHideEdit = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarEditItemCanOpenEdit"),
#endif
 DefaultValue(true), Category("Behavior")]
		public virtual bool CanOpenEdit {
			get { return canOpenEdit; }
			set {
				canOpenEdit = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarEditItemEditHeight"),
#endif
 DefaultValue(-1), Category("Appearance")]
		public virtual int EditHeight {
			get { return editHeight; }
			set {
				if(value < -1) value = -1;
				editHeight = value;
				OnItemChanged();
			}
		}
		int editWidth = -1;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarEditItemEditWidth"),
#endif
 DefaultValue(-1), Browsable(true), Category("Appearance"), SmartTagProperty("EditWidth", "Appearance", 8, SmartTagActionType.RefreshAfterExecute), Localizable(true)]
		public virtual int EditWidth {
			get { return editWidth; }
			set {
				if(value < -1) value = -1;
				editWidth = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarEditItemEditValue"),
#endif
 Browsable(true), DefaultValue(null), Category("Data"),
	   Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)), TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public virtual object EditValue {
			get { return editValue; }
			set {
				if(object.Equals(EditValue, value)) return;
				editValue = value;
				OnEditValueChanged();
			}
		}
		internal new bool ShouldSerializeVisibleWhenVertical() { return VisibleWhenVertical; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarEditItemVisibleWhenVertical"),
#endif
 Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Category("Behavior")]
		public override bool VisibleWhenVertical {
			get { return visibleWhenVertical; }
			set {
				if(value == VisibleWhenVertical) return;
				visibleWhenVertical = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarEditItemEdit"),
#endif
 Category("Behavior"),
		Editor("DevExpress.XtraBars.Design.RepositoryEditEditor, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(System.Drawing.Design.UITypeEditor)), SmartTagSearchNestedProperties("Properties editor")]
		public DevExpress.XtraEditors.Repository.RepositoryItem Edit {
			get { return edit; }
			set {
				if(edit != value) {
					OnEditChanging();
					DevExpress.XtraEditors.Repository.RepositoryItem old = edit;
					edit = value;
					if(old != null) old.Disconnect(this);
					if(Edit != null)
						Edit.Connect(this);
					if(Manager == null) return;
					OnEditChanged();
					OnItemChanged();
				}
			}
		}
		protected internal virtual void OnEditChanging() {
		}
		protected internal virtual void OnEditChanged() {
		}
		protected internal override void OnItemCreated(object creationArguments) {
			Edit = Manager.Helper.CreateLinkEditor(creationArguments == null ? "TextEdit" : creationArguments.ToString());
		}
		protected internal override BarItemPaintStyle CalcRealPaintStyle(BarItemLink link) {
			BarItemPaintStyle ps = link == null ? PaintStyle : link.PaintStyle;
			if(ps == BarItemPaintStyle.Standard) {
				if(link.IsLinkInMenu)
					ps = BarItemPaintStyle.CaptionGlyph;
				else {
					ps = BarItemPaintStyle.CaptionInMenu;
				}
			}
			return ps;
		}
		private static object showingEditor = new object();
		private static object shownEditor = new object();
		private static object hiddenEditor = new object();
		private static object editValueChanged = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarEditItemEditValueChanged"),
#endif
 Category("Events")]
		public event EventHandler EditValueChanged {
			add { Events.AddHandler(editValueChanged, value); }
			remove { Events.RemoveHandler(editValueChanged, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarEditItemHiddenEditor"),
#endif
 Category("Events")]
		public event ItemClickEventHandler HiddenEditor {
			add { Events.AddHandler(hiddenEditor, value); }
			remove { Events.RemoveHandler(hiddenEditor, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarEditItemShownEditor"),
#endif
 Category("Events")]
		public event ItemClickEventHandler ShownEditor {
			add { Events.AddHandler(shownEditor, value); }
			remove { Events.RemoveHandler(shownEditor, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarEditItemShowingEditor"),
#endif
 Category("Events")]
		public event ItemCancelEventHandler ShowingEditor {
			add { Events.AddHandler(showingEditor, value); }
			remove { Events.RemoveHandler(showingEditor, value); }
		}
		internal bool RaiseHiddenEditor(BarItemLink link) {
			ItemClickEventHandler handler = (ItemClickEventHandler)Events[hiddenEditor];
			if(handler == null) return true;
			try {
				ItemClickEventArgs e = new ItemClickEventArgs(this, link);
				handler(this, e);
			}
			catch {
				return false;
			}
			return true;
		}
		internal void RaiseShownEditor(BarItemLink link) {
			ItemClickEventHandler handler = (ItemClickEventHandler)Events[shownEditor];
			if(handler == null) return;
			ItemClickEventArgs e = new ItemClickEventArgs(this, link);
			handler(this, e);
		}
		internal bool RaiseShowingEditor(BarItemLink link) {
			ItemCancelEventHandler handler = (ItemCancelEventHandler)Events[showingEditor];
			if(handler == null) return true;
			ItemCancelEventArgs e = new ItemCancelEventArgs(this, link, false);
			handler(this, e);
			return !e.Cancel;
		}
		protected internal override void OnClick(BarItemLink link) {
			if(link == null && this.Links.Count > 0) {
				if(!RaiseVisibleLinkClick()) {
					RaiseEmbeddedVisibleLinkClick();
				}
				RaiseEmbeddedVisibleLinkClick();
			}
			base.OnClick(link);
		}
		void RaiseEmbeddedVisibleLinkClick() {
			if(Manager == null) return;
			foreach(Bar bar in Manager.Bars) {
				if(bar.BarControl == null) continue;
				foreach(BarItemLink linkItem in bar.BarControl.VisibleLinks) {
					BarEditItemLink editLink = linkItem as BarEditItemLink;
					if(editLink != null && linkItem.ClonedFromLink != null && linkItem.ClonedFromLink.Item == this) {
						editLink.ShowLinkEditor(false);
						break;
					}
				}
			}
		}
		bool RaiseVisibleLinkClick() {
			bool linkFound = false;
			foreach(BarEditItemLink linkItem in Links) {
				if(linkItem.Visible && linkItem.Enabled) {
					if((linkItem.BarControl != null && linkItem.BarControl.Visible) || (linkItem.Ribbon != null && linkItem.Ribbon.Visible)) {
						linkItem.ShowLinkEditor(false);
						linkFound = true;
						break;
					}
				}
			}
			return linkFound;
		}
		protected bool HasActiveEditor(BarManager manager) {
			return manager.ActiveEditor != null && manager.ActiveEditItemLink != null && manager.ActiveEditItemLink.Item == this;
		}
		protected BaseEdit GetActiveEditor() {
			if(Manager == null) return null;
			if(HasActiveEditor(Manager)) return Manager.ActiveEditor;
			if(Manager.MergedOwner != null && HasActiveEditor(Manager.MergedOwner))
				return Manager.MergedOwner.ActiveEditor;
			if(Ribbon != null && Ribbon.MergeOwner != null && Ribbon.MergeOwner.Manager != null && HasActiveEditor(Ribbon.MergeOwner.Manager))
				return Ribbon.MergeOwner.Manager.ActiveEditor;
			return null;
		}
		protected virtual void OnEditValueChanged() {
			if(IsLockUpdate || (Manager != null && Manager.IsLoading)) return;
			BaseEdit activeEditor = GetActiveEditor();
			if(activeEditor != null) activeEditor.EditValue = EditValue;
			OnItemChanged(false);
			FireChanged();
			RaiseEditValueChanged();
		}
		protected virtual void RaiseEditValueChanged() {
			EventHandler handler = (EventHandler)Events[editValueChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal virtual int DefaultEditWidth { get { return 50; } }
	}
	#endregion
	internal class BarPopupController : DevExpress.XtraEditors.Controls.WinPopupController {
		BarManager manager;
		internal BarPopupController(BarManager manager) {
			this.manager = manager;
		}
		public BarManager Manager {
			get { return manager; }
		}
		Form prevActiveForm = null;
		Control prevFocusedControl = null;
		public override void PopupClosed(DevExpress.Utils.Win.IPopupControl popup) {
			Manager.SelectionInfo.internalFocusLock ++;
			try {
				if(prevActiveForm != null && !prevActiveForm.Disposing && prevActiveForm.Visible) {
					prevActiveForm.Activate();
					if(prevFocusedControl != null && !prevFocusedControl.Disposing && prevFocusedControl.Visible)
						prevActiveForm.ActiveControl = prevFocusedControl;
				}
				prevActiveForm = null;
				prevFocusedControl = null;
			}
			finally {
				Manager.SelectionInfo.internalFocusLock --;
			}
		}
		public override void PopupShowing(DevExpress.Utils.Win.IPopupControl popup) {
			prevFocusedControl = null;
			prevActiveForm = ControlHelper.GetSafeActiveForm();
			if(prevActiveForm != null) 
				prevFocusedControl = prevActiveForm.ActiveControl;
		}
	}
	[Designer("DevExpress.XtraBars.Ribbon.Design.RibbonGalleryBarItemDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner)), SmartTagSupport(typeof(BarItemDesignTimeBoundsProvider), SmartTagSupportAttribute.SmartTagCreationMode.UseComponentDesigner)]
	public class RibbonGalleryBarItem : BarButtonItem {
		GalleryDropDown galleryDropDown;
		InRibbonGallery gallery;
		public RibbonGalleryBarItem() : this(null) { }
		public RibbonGalleryBarItem(BarManager manager)
			: base(manager, false) {
			this.gallery = new InRibbonGallery(this);
		}
		protected override void Dispose(bool disposing) {
			if(disposing && Gallery != null)
				Gallery.Dispose();
			base.Dispose(disposing);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int LargeWidth {
			get { return base.LargeWidth; }
			set { base.LargeWidth = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int SmallWithTextWidth {
			get { return base.SmallWithTextWidth; }
			set { base.SmallWithTextWidth = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int SmallWithoutTextWidth {
			get { return base.SmallWithoutTextWidth; }
			set { base.SmallWithoutTextWidth = value; }
		}
		protected virtual void OnGalleryToolbarItemClick(object sender, ItemClickEventArgs e) {
			RibbonToolbarPopupItemLink link = e.Link as RibbonToolbarPopupItemLink;
			RibbonGalleryBarItemLink galleryLink = link != null ? link.GalleryLink : null; 
			if(galleryLink == null) return;
			galleryLink.OnGalleryDropDown(e.Link);
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonGalleryBarItemGallery"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), System.ComponentModel.Category("Behavior")]
		public InRibbonGallery Gallery { get { return gallery; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonGalleryBarItemGalleryDropDown"),
#endif
DefaultValue(null), System.ComponentModel.Category("Behavior")]
		public GalleryDropDown GalleryDropDown {
			get { return galleryDropDown; }
			set { galleryDropDown = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new BarShortcut ItemShortcut {
			get { return base.ItemShortcut; }
			set { base.ItemShortcut = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarButtonStyle ButtonStyle {
			get { return BarButtonStyle.DropDown;  }
		}
		[System.ComponentModel.Category("Action")]
		public event GalleryItemClickEventHandler GalleryItemClick {
			add { Gallery.ItemClick += value; }
			remove { Gallery.ItemClick -= value; }
		}
		[System.ComponentModel.Category("Action")]
		public event GalleryItemClickEventHandler GalleryItemDoubleClick {
			add { Gallery.ItemDoubleClick += value; }
			remove { Gallery.ItemDoubleClick -= value; }
		}
		[System.ComponentModel.Category("Action")]
		public event GalleryItemEventHandler GalleryItemCheckedChanged {
			add { Gallery.ItemCheckedChanged += value; }
			remove { Gallery.ItemCheckedChanged -= value; }
		}
		[System.ComponentModel.Category("Events")]
		public event GalleryItemCustomDrawEventHandler GalleryCustomDrawItemImage {
			add { Gallery.CustomDrawItemImage += value; }
			remove { Gallery.CustomDrawItemImage -= value; }
		}
		[System.ComponentModel.Category("Events")]
		public event GalleryItemCustomDrawEventHandler GalleryCustomDrawItemText {
			add { Gallery.CustomDrawItemText += value; }
			remove { Gallery.CustomDrawItemText -= value; }
		}
		[System.ComponentModel.Category("Action")]
		public event GalleryFilterMenuItemClickEventHandler GalleryFilterMenuItemClick {
			add { Gallery.FilterMenuItemClick += value; }
			remove { Gallery.FilterMenuItemClick -= value; }
		}
		[System.ComponentModel.Category("Action")]
		public event GalleryItemEventHandler GalleryItemHover {
			add { Gallery.GalleryItemHover += value; }
			remove { Gallery.GalleryItemHover -= value; }
		}
		[System.ComponentModel.Category("Action")]
		public event GallerySelectionEventHandler MarqueeSelectionCompleted {
			add { Gallery.MarqueeSelectionCompleted += value; }
			remove { Gallery.MarqueeSelectionCompleted -= value; }
		}
		[System.ComponentModel.Category("Action")]
		public event GalleryItemEventHandler GalleryItemLeave {
			add { Gallery.GalleryItemLeave += value; }
			remove { Gallery.GalleryItemLeave -= value; }
		}
		[System.ComponentModel.Category("Layout")]
		public event GalleryFilterMenuEventHandler GalleryFilterMenuPopup {
			add { Gallery.FilterMenuPopup += value; }
			remove { Gallery.FilterMenuPopup -= value; }
		}
		[System.ComponentModel.Category("Layout")]
		public event InplaceGalleryEventHandler GalleryInitDropDownGallery {
			add { Gallery.InitDropDownGallery += value; }
			remove { Gallery.InitDropDownGallery -= value; }
		}
		[System.ComponentModel.Category("Layout")]
		public event InplaceGalleryEventHandler GalleryPopupClose {
			add { Gallery.PopupClose += value; }
			remove { Gallery.PopupClose -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Down {
			get { return base.Down; }
			set { base.Down = value; }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonGalleryBarItemGlyphDisabled")]
#endif
public override Image GlyphDisabled {
			get { return base.GlyphDisabled; }
			set { base.GlyphDisabled = value; }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonGalleryBarItemImageIndexDisabled")]
#endif
public override int ImageIndexDisabled {
			get { return base.ImageIndexDisabled; }
			set { base.ImageIndexDisabled = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Image LargeGlyph {
			get { return base.LargeGlyph; }
			set { base.LargeGlyph = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Image LargeGlyphDisabled {
			get { return base.LargeGlyphDisabled; }
			set { base.LargeGlyphDisabled = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int LargeImageIndex {
			get { return base.LargeImageIndex; }
			set { base.LargeImageIndex = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int LargeImageIndexDisabled {
			get { return base.LargeImageIndexDisabled; }
			set { base.LargeImageIndexDisabled = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ActAsDropDown {
			get { return true; }
			set {  }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AllowAllUp {
			get { return base.AllowAllUp; }
			set { base.AllowAllUp = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool CloseSubMenuOnClick {
			get { return base.CloseSubMenuOnClick; }
			set { base.CloseSubMenuOnClick = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override PopupControl DropDownControl {
			get { return base.DropDownControl; }
			set { base.DropDownControl = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool DropDownEnabled {
			get { return base.DropDownEnabled; }
			set { base.DropDownEnabled = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int GroupIndex {
			get { return base.GroupIndex; }
			set { base.GroupIndex = value; }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonGalleryBarItemGlyph")]
#endif
public new Image Glyph {
			get { return base.Glyph; }
			set { base.Glyph = value; }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonGalleryBarItemImageIndex")]
#endif
public new int ImageIndex {
			get { return base.ImageIndex; }
			set { base.ImageIndex = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new BarItemPaintStyle PaintStyle { 
			get { return base.PaintStyle; }
			set { base.PaintStyle = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new RibbonItemStyles RibbonStyle {
			get { return base.RibbonStyle; }
			set { base.RibbonStyle = value; }
		}
		public void MakeVisible(GalleryItem item) {
			for(int n = 0; n < Links.Count; n++) {
				((RibbonGalleryBarItemLink)Links[n]).MakeVisible(item);
			}
		}
	}
	public class SkinRibbonGalleryBarItem : RibbonGalleryBarItem {
		public SkinRibbonGalleryBarItem() : this(null) { }
		public SkinRibbonGalleryBarItem(BarManager manager)
			: base(manager) {
		}
		public void Initialize() {
			OnInitialize();
		}
		protected internal virtual void OnInitialize() {
			SkinHelper.InitSkinGallery(this);
		}
	}
}
