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
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Win;
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
	[ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "TileNavPane")]
	[ToolboxTabName(AssemblyInfo.DXTabNavigation), Docking(DockingBehavior.AutoDock)]
	[Description("A hierarchical tile menu, providing navigation in a touch-friendly manner.")]
	[Designer("DevExpress.XtraBars.Design.TileNavPaneDesigner, " + AssemblyInfo.SRAssemblyBarsDesign)]
	[DXToolboxItem(true)]
	public class TileNavPane : Control, ITileElementCollectionOwner, ITileBarWindowOwner,
		IAppearanceOwner, ISupportLookAndFeel, ITileNavPaneDropDownOptionsOwner, ITileBarOwner, IToolTipControlClient {
		CollectionBase ITileElementCollectionOwner.Collection { get { return Categories; } }
		static readonly object elementClick = new object();
		static readonly object tileClick = new object();
		static readonly object selectedElementChanging = new object();
		static readonly object selectedElementChanged = new object();
		static readonly object dropDownShow = new object();
		static readonly object dropDownHide = new object();
		public TileNavPane() {
			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.Selectable | ControlStyles.SupportsTransparentBackColor |
				ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
			ToolTipController.DefaultController.AddClientControl(this);
			this.buttonPadding = DefaultButtonPadding;
			this.lookAndFeel = new UserLookAndFeel(this);
			this.lookAndFeel.StyleChanged += OnLookAndFeelChanged;
			EnsureDefaultCategory();
			this.mainButtonBehaviorCore = TileNavPaneMainButtonBehavior.Default;
			this.showDropDownOnHoverCore = false;
			this.NeedTileBarAppearanceUpdate = true;
			this.ContinuousNavigation = false;
			this.ToolTipShowMode = ToolTipShowMode.AllElements;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				this.lookAndFeel.StyleChanged -= OnLookAndFeelChanged;
				RemoveTooltipControllers();
				DisposeDropDowns();
			}
			base.Dispose(disposing);
		}
		void RemoveTooltipControllers() {
			ToolTipController.DefaultController.RemoveClientControl(this);
			ToolTipController = null;
		}
		void DisposeDropDowns() {
			if(categoriesDropDown != null && !categoriesDropDown.IsDisposed)
				CategoriesDropDown.Dispose();
			for(int i = 0; i < Categories.Count; i++) {
				Categories[i].Dispose();
			}
			if(DefaultCategory != null && DefaultCategory.DropDown != null && !DefaultCategory.DropDown.IsDisposed)
				DefaultCategory.DropDown.Dispose();
		}
		void EnsureDefaultCategory() {
			this.defaultCategoryCore = new TileNavCategory() { DefaultCategoryOwner = this };
		}
		ToolTipController toolTipController;
		[DXCategory(CategoryName.Appearance),  DefaultValue(null)]
		public ToolTipController ToolTipController {
			get { return toolTipController; }
			set {
				if(ToolTipController == value) return;
				if(ToolTipController != null)
					ToolTipController.RemoveClientControl(this);
				toolTipController = value;
				if(ToolTipController != null) {
					ToolTipController.DefaultController.RemoveClientControl(this);
					ToolTipController.AddClientControl(this);
				}
				else ToolTipController.DefaultController.AddClientControl(this);
				OnToolTipControllerChanged();
			}
		}
		void OnToolTipControllerChanged() {
			DropDownAppearanceChanged(); 
		}
		public override ISite Site {
			get { return base.Site; }
			set {
				base.Site = value;
				if(value != null)
					AddDefaultCategoryToContainer();
			}
		}
		void AddDefaultCategoryToContainer() {
			if(Site == null || this.defaultCategoryCore == null) return;
			Site.Container.Add(this.defaultCategoryCore);
		}
		protected override Size DefaultSize {
			get { return new Size(500, 40); }
		}
		void OnLookAndFeelChanged(object sender, EventArgs e) {
			ViewInfo.ResetAppearance();
			ViewInfo.ResetDefaultAppearances();
			(this as ITileNavPaneDropDownOptionsOwner).OnDropDownOptionsChanged(true);
		}
		[DefaultValue(ToolTipShowMode.AllElements), Category(CategoryName.Behavior)]
		public ToolTipShowMode ToolTipShowMode { get; set; }
		[DefaultValue(false), Category(CategoryName.Behavior)]
		public bool ContinuousNavigation { get; set; }
		[Obsolete("use СontinuousNavigation property instead")]
		[DefaultValue(false), Category(CategoryName.Behavior), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShowDropDownOnSelectedElementChanged { get; set; }
		bool showDropDownOnHoverCore;
		[DefaultValue(false)]
		bool ShowDropDownOnHover {
			get { return showDropDownOnHoverCore; }
			set {
				if(showDropDownOnHoverCore == value) return;
				showDropDownOnHoverCore = value;
			}
		}
		TileNavPaneDropDownOptions optionsPrimary;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Behavior)]
		public TileNavPaneDropDownOptions OptionsPrimaryDropDown {
			get {
				if(optionsPrimary == null)
					optionsPrimary = new TileNavPaneDropDownOptions(this);
				return optionsPrimary;
			}
		}
		TileNavPaneDropDownOptions optionsSecondary;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Behavior)]
		public TileNavPaneDropDownOptions OptionsSecondaryDropDown {
			get {
				if(optionsSecondary == null)
					optionsSecondary = new TileNavPaneDropDownOptions(this);
				return optionsSecondary;
			}
		}
		protected internal bool IsDesignMode { get { return DesignMode; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Font Font {
			get { return base.Font; }
			set { base.Font = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color ForeColor {
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override RightToLeft RightToLeft {
			get { return base.RightToLeft; }
			set { base.RightToLeft = value; }
		}
		UserLookAndFeel lookAndFeel;
		bool ShouldSerializeLookAndFeel() { return LookAndFeel.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Appearance)]
		public UserLookAndFeel LookAndFeel {
			get { return lookAndFeel; }
		}
		bool allowGlyphSkinning;
		[DefaultValue(false), Category(CategoryName.Appearance)]
		public virtual bool AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(allowGlyphSkinning == value)
					return;
				allowGlyphSkinning = value;
				OnPropertiesChanged();
			}
		}
		ShowDirection showDirection = ShowDirection.Normal;
		[DefaultValue(ShowDirection.Normal), Category(CategoryName.Behavior)]
		public ShowDirection DropDownShowDirection {
			get { return showDirection; }
			set { showDirection = value; }
		}
		public static Padding DefaultButtonPadding { get { return new Padding(12); } }
		void ResetButtonPadding() { ButtonPadding = DefaultButtonPadding; }
		bool ShouldButtonPadding() { return ButtonPadding != DefaultButtonPadding; }
		Padding buttonPadding;
		[Category(CategoryName.Appearance)]
		public Padding ButtonPadding {
			get { return buttonPadding; }
			set {
				if(ButtonPadding == value)
					return;
				buttonPadding = value;
				OnPropertiesChanged();
			}
		}
		NavElement selectedElement;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public NavElement SelectedElement {
			get { return selectedElement; }
			set {
				ViewInfo.HideActiveDropDown();
				if(selectedElement != value)
					ProceedSelectedElementChanging(value as TileNavElement, selectedElement as TileNavElement);
				ShowLastDropDown();
				OnPropertiesChanged();
			}
		}
		void ProceedSelectedElementChanging(TileNavElement element, TileNavElement prevElement) {
			var e = RaiseSelectedElementChanging(element, prevElement);
			if(e.Cancel)
				return;
			selectedElement = e.Element;
			RaiseSelectedElementChanged(e.Element as TileNavElement);
		}
		void ShowLastDropDown() {
			if(ContinuousNavigation && !(SelectedElement is TileNavSubItem)) {
				ViewInfo.IsReady = false;
				CheckViewInfo();
				ViewInfo.ShowLastDropDownInPath();
			}
		}
		TileNavCategory defaultCategoryCore;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TileNavCategory DefaultCategory {
			get { return defaultCategoryCore; }
		}
		TileNavPaneMainButtonBehavior mainButtonBehaviorCore;
		[DefaultValue(TileNavPaneMainButtonBehavior.Default), Category(CategoryName.Behavior)]
		public TileNavPaneMainButtonBehavior MainButtonBehavior {
			get { return mainButtonBehaviorCore; }
			set {
				if(mainButtonBehaviorCore == value) return;
				mainButtonBehaviorCore = value;
				OnPropertiesChanged();
				DropDownAppearanceChanged();
			}
		}
		internal bool ShouldSerializeMainButtonBehavior {
			get { return MainButtonBehavior != TileNavPaneMainButtonBehavior.Default; }
		}
		internal void ResetMainButtonBehavior() {
			MainButtonBehavior = TileNavPaneMainButtonBehavior.Default;
		}
		TileNavPaneNavigator navigatorCore;
		protected internal TileNavPaneNavigator Navigator {
			get {
				if(navigatorCore == null) navigatorCore = CreateNavigator();
				return navigatorCore;
			}
		}
		protected virtual TileNavPaneNavigator CreateNavigator() {
			return new TileNavPaneNavigator(this);
		}
		TileNavCategoryCollection categories;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TileNavCategoryCollection Categories {
			get {
				if(categories == null)
					categories = CreateCategories();
				return categories;
			}
		}
		protected virtual TileNavCategoryCollection CreateCategories() {
			return new TileNavCategoryCollection(this);
		}
		TileBarWindow categoriesDropDown;
		protected internal TileBarWindow CategoriesDropDown {
			get {
				if(categoriesDropDown == null)
					categoriesDropDown = CreateCategoriesDropDown();
				return categoriesDropDown;
			}
		}
		protected virtual TileBarWindow CreateCategoriesDropDown() {
			TileBarWindow dropDown = new TileBarWindow(this, this);
			dropDown.Text = "Categories DropDown";
			dropDown.ApplyOptions();
			return dropDown;
		}
		Control dropDownContentCore;
		Control DropDownContent {
			get {
				if(dropDownContentCore == null)
					dropDownContentCore = CreateDropDownContent();
				return dropDownContentCore;
			}
		}
		protected virtual Control CreateDropDownContent() {
			return new TileBar() { Owner = this };
		}
		void ITileBarWindowOwner.OnDropDownClosed() { ViewInfo.OnDropDownClosed(); }
		TileBarWindow ITileBarWindowOwner.GetDropDown() { return CategoriesDropDown; }
		bool ITileBarWindowOwner.CloseOnOuterClick { get { return OptionsPrimaryDropDown.CloseOnOuterClick.ToBoolean(true); } }
		Control ITileBarWindowOwner.GetDropDownContent() { return DropDownContent; }
		FlyoutPanelOptions ITileBarWindowOwner.DropDownOptions { get { return GetDropDownOptions(null); } }
		int ITileBarWindowOwner.DropDownHeight { get { return GetDropDownHeight(true); } }
		Color ITileBarWindowOwner.DropDownBackColor { get { return Color.Empty; } }
		bool ITileBarWindowOwner.IsTileNavPane { get { return true; } }
		Orientation ITileBarWindowOwner.Orientation { get { return Orientation.Horizontal; } }
		void ITileBarWindowOwner.UpdateTileBar(TileBar tb) { OnUpdateTileBar(tb); }
		protected virtual void OnUpdateTileBar(TileBar tileBar) {
			tileBar.BeginUpdate();
			try {
				if(NeedTileBarItemsUpdate) UpdateTileBarItems(tileBar);
				if(NeedTileBarAppearanceUpdate) UpdateTileBarAppearances(tileBar);
			}
			finally { tileBar.EndUpdate(); }
		}
		protected virtual void UpdateTileBarAppearances(TileBar tileBar) {
			tileBar.WideTileWidth = OptionsPrimaryDropDown.WideItemWidth;
			tileBar.ItemSize = OptionsPrimaryDropDown.ItemHeight;
			tileBar.BackColor = OptionsPrimaryDropDown.BackColor.IsEmpty ?
				GetButtonDropDownBackColor(GetMainButtonAppearanceSelected()) : OptionsPrimaryDropDown.BackColor;
			tileBar.AllowGlyphSkinning = OptionsPrimaryDropDown.AllowGlyphSkinning != DefaultBoolean.Default ?
				OptionsPrimaryDropDown.AllowGlyphSkinning.ToBoolean(false) : AllowGlyphSkinning;
			tileBar.AppearanceItem.Assign(OptionsPrimaryDropDown.AppearanceItem);
			tileBar.AppearanceGroupText.Assign(OptionsPrimaryDropDown.AppearanceGroupText);
			tileBar.ShowItemShadow = OptionsPrimaryDropDown.ShowItemShadow.ToBoolean(false);
			tileBar.LookAndFeel.SkinName = this.LookAndFeel.ActiveSkinName;
			tileBar.LookAndFeel.UseDefaultLookAndFeel = false;
			tileBar.ToolTipController = this.ToolTipController;
			NeedTileBarAppearanceUpdate = false;
		}
		protected virtual void UpdateTileBarItems(TileBar tileBar) {
			TileNavPaneDropDownHelper.ClearTileBar(tileBar);
			if(Categories.Count == 0) return;
			CreateTileBarItems(tileBar);
			NeedTileBarItemsUpdate = false;
		}
		protected virtual void CreateTileBarItems(TileBar tileBar) {
			TileBarGroup group = new TileBarGroup();
			foreach(TileNavCategory cat in Categories) {
				TileNavPaneDropDownHelper.AddTile(tileBar, cat.Tile, cat.GroupName, group);
			}
			tileBar.Groups.Add(group);
		}
		protected internal bool NeedTileBarItemsUpdate { get; set; }
		protected internal bool NeedTileBarAppearanceUpdate { get; set; }
		protected internal FlyoutPanelOptions GetDropDownOptions(TileNavElement element) {
			FlyoutPanelOptions opt = new FlyoutPanelOptions();
			opt.AnchorType = PopupToolWindowAnchor.Manual;
			opt.Location = GetDropDownLocationCore(element);
			opt.AnimationType = PopupToolWindowAnimation.Fade;
			return opt;
		}
		protected internal virtual Point GetDropDownLocationCore(TileNavElement element) {
			if(DropDownShowDown)
				return new Point(0, this.Height);
			int height = element == null ? GetDropDownHeight(true) : element.GetDropDownHeight(this);
			return new Point(0, -height);
		}
		protected internal bool DropDownShowDown {
			get {
				switch(Dock) {
					case DockStyle.Bottom: return DropDownShowDirection == ShowDirection.Normal ? false : true;
					default: return DropDownShowDirection == ShowDirection.Normal ? true : false;
				}
			}
		}
		protected internal Color GetButtonDropDownBackColor(AppearanceObject appearance) {
			if(!appearance.BackColor.IsEmpty && !appearance.BackColor2.IsEmpty &&
				appearance.GradientMode == System.Drawing.Drawing2D.LinearGradientMode.Vertical)
				return appearance.BackColor2;
			return appearance.BackColor;
		}
		TileNavButtonCollection buttons;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TileNavButtonCollection Buttons {
			get {
				if(buttons == null)
					buttons = CreateButtons(this);
				return buttons;
			}
		}
		protected virtual TileNavButtonCollection CreateButtons(TileNavPane tileNavPane) {
			return new TileNavButtonCollection(this);
		}
		bool firstPaintComplete;
		NavElement defferedShowDropDownElement;
		void ExecuteDefferedShowDropDown() {
			if(defferedShowDropDownElement != null)
				ShowDropDown(defferedShowDropDownElement);
			defferedShowDropDownElement = null;
		}
		protected override void OnPaint(PaintEventArgs e) {
			CheckViewInfo();
			using(GraphicsCache cache = new GraphicsCache(e)) {
				ObjectPainter.DrawObject(cache, Painter, ViewInfo);
			}
			firstPaintComplete = true;
			RaisePaintEvent(this, e);
			ExecuteDefferedShowDropDown();
		}
		TileNavPaneViewInfo viewInfo;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TileNavPaneViewInfo ViewInfo {
			get {
				if(viewInfo == null)
					viewInfo = CreateViewInfo();
				return viewInfo;
			}
		}
		protected virtual TileNavPaneViewInfo CreateViewInfo() {
			return new TileNavPaneViewInfo(this);
		}
		TileNavPanePainter painter;
		protected TileNavPanePainter Painter {
			get {
				if(painter == null)
					painter = CreatePainter();
				return painter;
			}
		}
		protected virtual TileNavPanePainter CreatePainter() {
			return new TileNavPanePainter();
		}
		TileNavPaneHandler handler;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected internal TileNavPaneHandler Handler {
			get {
				if(handler == null)
					handler = CreateHandler();
				return handler;
			}
		}
		protected virtual TileNavPaneHandler CreateHandler() {
			return new TileNavPaneHandler(this);
		}
		public TileNavPaneHitInfo CalcHitInfo(Point pt) {
			return ViewInfo.CalcHitInfo(pt);
		}
		protected internal virtual void CheckViewInfo() {
			if(!ViewInfo.IsReady) {
				ViewInfo.Calc(ClientRectangle);
			}
		}
		void ResetAppearance() { Appearance.Reset(); }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		AppearanceObject appearance;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Appearance)]
		public AppearanceObject Appearance {
			get {
				if(appearance == null) {
					appearance = new AppearanceObject();
					appearance.Changed += new EventHandler(OnAppearanceNormalChanged);
				}
				return appearance;
			}
		}
		void ResetAppearanceSelected() { AppearanceSelected.Reset(); }
		bool ShouldSerializeAppearanceSelected() { return AppearanceSelected.ShouldSerialize(); }
		AppearanceObject appearanceSelected;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Appearance)]
		public AppearanceObject AppearanceSelected {
			get {
				if(appearanceSelected == null) {
					appearanceSelected = new AppearanceObject();
					appearanceSelected.Changed += new EventHandler(OnAppearanceSelectedChanged);
				}
				return appearanceSelected;
			}
		}
		void ResetAppearanceHovered() { AppearanceHovered.Reset(); }
		bool ShouldSerializeAppearanceHovered() { return AppearanceHovered.ShouldSerialize(); }
		AppearanceObject appearanceHovered;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Appearance)]
		public AppearanceObject AppearanceHovered {
			get {
				if(appearanceHovered == null) {
					appearanceHovered = new AppearanceObject();
					appearanceHovered.Changed += new EventHandler(OnAppearanceChanged);
				}
				return appearanceHovered;
			}
		}
		void OnAppearanceSelectedChanged(object sender, EventArgs e) {
			OnAppearanceChanged(sender, e);
			DropDownAppearanceChanged();
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			OnPropertiesChanged();
		}
		void OnAppearanceNormalChanged(object sender, EventArgs e) {
			ViewInfo.ResetAppearance();
			OnPropertiesChanged();
		}
		int UpdateCount { get; set; }
		public void BeginUpdate() {
			UpdateCount++;
		}
		public void EndUpdate() {
			if(UpdateCount == 0)
				return;
			UpdateCount--;
			if(UpdateCount == 0)
				OnPropertiesChanged();
		}
		[Browsable(false)]
		protected internal bool IsLockUpdate { get { return UpdateCount > 0; } }
		protected internal void OnPropertiesChanged() {
			Refresh();
		}
		public override void Refresh() {
			if(IsLockUpdate)
				return;
			ViewInfo.IsReady = false;
			Invalidate();
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			OnPropertiesChanged();
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			base.OnMouseWheel(e);
			CheckViewInfo();
			Handler.OnMouseWheel(e);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			CheckViewInfo();
			Handler.OnMouseDown(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			CheckViewInfo();
			Handler.OnMouseUp(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			CheckViewInfo();
			Handler.OnMouseMove(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			CheckViewInfo();
			Handler.OnMouseLeave(e);
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			CheckViewInfo();
			Handler.OnMouseEnter(e);
		}
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			CheckViewInfo();
			if(Handler.ProcessCmdKey(keyData))
				return true;
			return base.ProcessCmdKey(ref msg, keyData);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			CheckViewInfo();
			Handler.OnKeyDown(e);
		}
		protected override void OnLostFocus(EventArgs e) {
			Navigator.ClearFocus();
		}
		const int primaryDropDownHeight = 120;
		const int secondaryDropDownHeight = 98;
		internal int GetDropDownHeight(bool primary) {
			int height = 0;
			if(primary)
				height = OptionsPrimaryDropDown.Height > 0 ? OptionsPrimaryDropDown.Height : primaryDropDownHeight;
			else
				height = OptionsSecondaryDropDown.Height > 0 ? OptionsSecondaryDropDown.Height : secondaryDropDownHeight;
			return height;
		}
		public void HideDropDownWindow() {
			ViewInfo.HideActiveDropDown();
			OnPropertiesChanged();
		}
		protected virtual TileNavElement FindInDefaultCategory(TileBarItem tile) {
			TileNavElement element = null;
			foreach(TileNavItem item in DefaultCategory.Items) {
				if(item.Tile == tile) {
					element = item;
					break;
				}
				FindElementByTileCore(item, tile, out element);
				if(element != null) break;
			}
			return element;
		}
		protected virtual TileNavElement FindInButtons(TileBarItem tile) {
			TileNavElement element = null;
			foreach(ITileNavButton button in Buttons) {
				if(button is TileNavCategory) {
					FindElementByTileCore((TileNavCategory)button, tile, out element);
					if(element != null) break;
				}
			}
			return element;
		}
		protected internal bool FindElementByTileCore(ITileElementCollectionOwner owner, TileBarItem item, out TileNavElement result) {
			result = null;
			bool found = false;
			if(owner == null) return false;
			foreach(object obj in owner.Collection) {
				if(found) return true;
				TileNavElement element = obj as TileNavElement;
				if(element.Tile == item) {
					result = element;
					return true;
				}
				found = FindElementByTileCore(element as ITileElementCollectionOwner, item, out result);
			}
			return found;
		}
		protected virtual void RaiseElementClickByTile(TileNavElement element) {
			element.OnElementClick(true);
			RaiseTileClick(element);
			RaiseElementClick(element, true);
		}
		protected override void OnPaintBackground(PaintEventArgs pevent) {
			if(!ShouldDrawBackground)
				return;
			base.OnPaintBackground(pevent);
		}
		protected virtual bool ShouldDrawBackground {
			get {
				if(BackgroundImage == null)
					return true;
				return BackgroundImageLayout != ImageLayout.Tile;
			}
		}
		#region EVENTS
		[Category(CategoryName.Behavior)]
		public event TileNavPaneDropDownEventHandler DropDownShown {
			add { Events.AddHandler(dropDownShow, value); }
			remove { Events.RemoveHandler(dropDownShow, value); }
		}
		[Category(CategoryName.Behavior)]
		public event TileNavPaneDropDownEventHandler DropDownHidden {
			add { Events.AddHandler(dropDownHide, value); }
			remove { Events.RemoveHandler(dropDownHide, value); }
		}
		[Category(CategoryName.Behavior)]
		public event NavElementClickEventHandler TileClick {
			add { Events.AddHandler(tileClick, value); }
			remove { Events.RemoveHandler(tileClick, value); }
		}
		[Category(CategoryName.Behavior)]
		public event NavElementClickEventHandler ElementClick {
			add { Events.AddHandler(elementClick, value); }
			remove { Events.RemoveHandler(elementClick, value); }
		}
		[Category(CategoryName.Behavior)]
		public event TileNavPaneSelectedElementEventHandler SelectedElementChanging {
			add { Events.AddHandler(selectedElementChanging, value); }
			remove { Events.RemoveHandler(selectedElementChanging, value); }
		}
		[Category(CategoryName.Behavior)]
		public event TileNavPaneElementEventHandler SelectedElementChanged {
			add { Events.AddHandler(selectedElementChanged, value); }
			remove { Events.RemoveHandler(selectedElementChanged, value); }
		}
		protected internal void OnElementClick(NavElement Element) {
			RaiseElementClick(Element, false);
		}
		protected void RaiseElementClick(NavElement element, bool isTile) {
			RaiseElementClickCore(element, isTile);
			if(!isTile) return;
			TileNavElement tileElement = element as TileNavElement;
			if(tileElement == null || !tileElement.IsInCollection) return;
			SelectedElement = tileElement;
		}
		protected void RaiseElementClickCore(NavElement element, bool isTile) {
			NavElementEventArgs e = new NavElementEventArgs() { Element = element, IsTile = isTile };
			NavElementClickEventHandler handler = Events[elementClick] as NavElementClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected void RaiseTileClick(NavElement element) {
			NavElementEventArgs e = new NavElementEventArgs() { Element = element, IsTile = true };
			NavElementClickEventHandler handler = Events[tileClick] as NavElementClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected TileNavPaneSelectedElementEventArgs RaiseSelectedElementChanging(TileNavElement newElement, TileNavElement oldElement) {
			TileNavPaneSelectedElementEventArgs e = new TileNavPaneSelectedElementEventArgs() { Element = newElement, PreviousElement = oldElement };
			TileNavPaneSelectedElementEventHandler handler = Events[selectedElementChanging] as TileNavPaneSelectedElementEventHandler;
			if(handler != null)
				handler(this, e);
			return e;
		}
		protected void RaiseSelectedElementChanged(TileNavElement element) {
			TileNavElementEventArgs e = new TileNavElementEventArgs() { Element = element };
			TileNavPaneElementEventHandler handler = Events[selectedElementChanged] as TileNavPaneElementEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal virtual void OnDropDownShown(NavElement element) {
			RaiseDropDownShown(element);
		}
		protected internal virtual void OnDropDownHidden(NavElement element) {
			RaiseDropDownHidden(element);
		}
		protected void RaiseDropDownShown(NavElement element) {
			DropDownEventArgs e = new DropDownEventArgs() { Element = element };
			TileNavPaneDropDownEventHandler handler = Events[dropDownShow] as TileNavPaneDropDownEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected void RaiseDropDownHidden(NavElement element) {
			DropDownEventArgs e = new DropDownEventArgs() { Element = element };
			TileNavPaneDropDownEventHandler handler = Events[dropDownHide] as TileNavPaneDropDownEventHandler;
			if(handler != null)
				handler(this, e);
		}
		#endregion
		ITileNavPaneDesigner Designer {
			get {
				IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
				if(host != null) return host.GetDesigner(this) as ITileNavPaneDesigner;
				return null;
			}
		}
		protected internal bool DebuggingState {
			get {
				if(Designer == null)
					return false;
				return Designer.DebuggingState;
			}
		}
		bool IAppearanceOwner.IsLoading { get { return false; } }
		bool ISupportLookAndFeel.IgnoreChildren { get { return true; } }
		UserLookAndFeel ISupportLookAndFeel.LookAndFeel { get { return LookAndFeel; } }
		void ITileNavPaneDropDownOptionsOwner.OnDropDownOptionsChanged(bool updateDropDown) {
			OnPropertiesChanged();
			if(updateDropDown)
				DropDownAppearanceChanged();
		}
		internal AppearanceObject GetElementAppearanceSelected(TileNavElement tileNavElement) {
			TileNavButtonViewInfo buttonInfo = ViewInfo.GetButtonInfoByElement(tileNavElement);
			if(buttonInfo == null) return AppearanceSelected;
			return buttonInfo.GetAppearanceSelected();
		}
		internal AppearanceObject GetMainButtonAppearanceSelected() {
			TileNavButtonViewInfo buttonInfo = ViewInfo.MainButton;
			if(buttonInfo == null) return AppearanceSelected;
			return buttonInfo.GetAppearanceSelected();
		}
		bool ITileBarOwner.ShowTileToolTips {
			get { return ToolTipShowMode == ToolTipShowMode.AllElements || ToolTipShowMode == ToolTipShowMode.Tiles; }
		}
		void ITileBarOwner.RaiseItemClick(TileItem tileBarItem) {
			if(IsDesignMode) {
				ViewInfo.DesignTimeManager.SelectComponent(tileBarItem);
				return;
			}
			TileBarItem tile = tileBarItem as TileBarItem;
			if(tile == null) return;
			var element = FindElementByTile(tile);
			if(element != null)
				RaiseElementClickByTile(element);
		}
		TileNavElement ITileBarOwner.GetElementByTile(TileItem tileBarItem) {
			var tile = tileBarItem as TileBarItem;
			if(tile == null) return null;
			return FindElementByTile(tile);
		}
		TileNavElement FindElementByTile(TileBarItem tile) {
			TileNavElement element;
			FindElementByTileCore(this, tile, out element);
			if(element == null) element = FindInButtons(tile);
			if(element == null) element = FindInDefaultCategory(tile);
			return element;
		}
		public void Assign(TileNavPane source) {
			BeginUpdate();
			try {
				this.ContinuousNavigation = source.ContinuousNavigation;
				this.AllowGlyphSkinning = source.AllowGlyphSkinning;
				this.Appearance.Assign(source.Appearance);
				this.AppearanceHovered.Assign(source.AppearanceHovered);
				this.AppearanceSelected.Assign(source.AppearanceSelected);
				this.BackColor = source.BackColor;
				this.BackgroundImage = source.BackgroundImage;
				this.BackgroundImageLayout = source.BackgroundImageLayout;
				this.ButtonPadding = source.ButtonPadding;
				this.ForeColor = source.ForeColor;
				this.Size = source.Size;
				this.Tag = source.Tag;
				this.Visible = source.Visible;
				this.OptionsPrimaryDropDown.Assign(source.OptionsPrimaryDropDown);
				this.OptionsSecondaryDropDown.Assign(source.OptionsSecondaryDropDown);
				this.Categories.Assign(source.Categories);
				this.DefaultCategory.Assign(source.DefaultCategory);
				this.Buttons.Assign(source.Buttons);
			}
			finally { EndUpdate(); }
		}
		internal void DropDownAppearanceChanged() {
			NeedTileBarAppearanceUpdate = true;
			(this as ITileBarWindowOwner).GetDropDown().UpdateTileBar();
		}
		internal void DropDownItemsChanged() {
			NeedTileBarItemsUpdate = true;
			if(categoriesDropDown == null) return;
			(this as ITileBarWindowOwner).GetDropDown().UpdateTileBar();
		}
		internal void OnMainButtonChanged(NavButton mainButton) {
			foreach(ITileNavButton but in Buttons) {
				if(!(but is NavButton)) continue;
				var button = but as NavButton;
				if(button.Equals(mainButton)) continue;
				button.IsMain = false;
			}
			OnPropertiesChanged();
		}
		#region ToolTips_implementation
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			return GetToolTipInfo(point);
		}
		bool IToolTipControlClient.ShowToolTips {
			get { return ShowElementsToolTipsCore; }
		}
		bool ShowElementsToolTipsCore {
			get { return ToolTipShowMode == ToolTipShowMode.NavBarItems || ToolTipShowMode == ToolTipShowMode.AllElements; }
		}
		protected virtual ToolTipControlInfo GetToolTipInfo(Point point) {
			if(CanShowToolTip) {
				object obj = this;
				SuperToolTip superTip = null;
				ToolTipControlInfo res = new ToolTipControlInfo();
				var hitInfo = ViewInfo.CalcHitInfo(point);
				if(hitInfo.InButton && hitInfo.ButtonInfo != null && hitInfo.ButtonInfo.Element != null) {
					obj = hitInfo.ButtonInfo.Element;
					superTip = hitInfo.ButtonInfo.Element.SuperTip;
				}
				res.Object = obj;
				res.SuperTip = superTip;
				return res;
			}
			return null;
		}
		protected virtual bool CanShowToolTip {
			get { return ShowElementsToolTipsCore && !IsDesignMode; }
		}
		#endregion
		public void ShowDropDown(NavElement element) {
			if(!firstPaintComplete) {
				defferedShowDropDownElement = element;
				return;
			}
			var owner = ViewInfo.ActiveDropDownOwner;
			if(element.ViewInfo == null || !element.ViewInfo.IsVisible) return;
			if(element is NavButton && (element as NavButton).IsMain && owner == this) return;
			if(element is ITileBarWindowOwner && (element as ITileBarWindowOwner) == owner) return;
			element.ViewInfo.OnElementClick(false);
		}
	}
	[SmartTagSupport(typeof(TileNavCategoryDesignTimeBoundsProvider), SmartTagSupportAttribute.SmartTagCreationMode.Auto),
	SmartTagAction(typeof(TileNavCategoryDesignTimeActionsProvider), "EditItems", "Edit Items", SmartTagActionType.CloseAfterExecute),
	Designer("DevExpress.XtraBars.Design.TileNavCategoryDesigner, " + AssemblyInfo.SRAssemblyBarsDesign),
	ToolboxItem(false), DesignTimeVisible(false)]
	public class TileNavCategory : TileNavElement, ITileElementCollectionOwner, ITileNavButton {
		CollectionBase ITileElementCollectionOwner.Collection { get { return Items; } }
		[Browsable(false)]
		public TileNavCategoryCollection OwnerCollection { get; set; }
		public TileNavCategory() {
			Tile = CreateTile();
		}
		protected virtual TileBarItem CreateTile() {
			return new TileBarItem();
		}
		protected virtual TileNavItemCollection CreateItems() {
			return new TileNavItemCollection(this);
		}
		protected override TileBarWindow CreateDropDown() {
			return CreateItemsDropDown();
		}
		protected override Control CreateDropDownContent() {
			return new TileBar() { Owner = this.Owner };
		}
		TileNavItemCollection items;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Items")]
		public TileNavItemCollection Items {
			get {
				if(items == null)
					items = CreateItems();
				return items;
			}
		}
		[Browsable(false)]
		public TileNavPane Owner { get { return GetOwner(); } }
		protected internal TileNavPane DefaultCategoryOwner { get; set; }
		[Browsable(false)]
		public bool IsDefaultCategory { get { return DefaultCategoryOwner != null; } }
		protected override TileNavPane GetOwner() {
			if(IsDefaultCategory) return DefaultCategoryOwner;
			TileNavPane customCollectionOwner = null;
			if(((ITileNavButton)this).OwnerCollection != null)
				customCollectionOwner = ((ITileNavButton)this).OwnerCollection.Owner;
			if(OwnerCollection == null && customCollectionOwner != null)
				return customCollectionOwner;
			return OwnerCollection == null ? null : OwnerCollection.Owner;
		}
		protected virtual TileBarWindow CreateItemsDropDown() {
			if(Owner == null) return null;
			TileBarWindow dropDown = new TileBarWindow(Owner, this);
			dropDown.ApplyOptions();
			dropDown.Text = "Items DropDown";
			return dropDown;
		}
		protected internal override void UpdateTileBarItems(TileBar tileBar) {
			TileNavPaneDropDownHelper.ClearTileBar(tileBar);
			if(Items.Count == 0) return;
			CreateTileBarItems(tileBar);
			NeedTileBarItemsUpdate = false;
		}
		protected virtual void CreateTileBarItems(TileBar tileBar) {
			TileBarGroup group = new TileBarGroup();
			foreach(TileNavItem item in Items) {
				TileNavPaneDropDownHelper.AddTile(tileBar, item.Tile, item.GroupName, group);
				if(TileNavPaneDropDownHelper.HasVisibleTiles(item.SubItems)) {
					item.Tile.DropDownOwner = item;
					item.Tile.TileBar = (item as ITileBarWindowOwner).GetDropDownContent() as TileBar;
					item.Tile.TileBar.ParentPopup = this.DropDown;
				}
				else
					item.Tile.TileBar = null;
			}
			tileBar.Groups.Add(group);
		}
		protected internal override void UpdateTileBarAppearances(TileBar tileBar) {
			tileBar.DropDownShowDirection = Owner.DropDownShowDown ? ShowDirection.Normal : ShowDirection.Inverted;
			tileBar.ItemSize = GetDropDownItemSize(Owner);
			tileBar.WideTileWidth = GetDropDownWideItemWidth(Owner);
			tileBar.BackColor = GetDropDownBackColor(Owner);
			tileBar.Dock = DockStyle.Fill;
			tileBar.AllowGlyphSkinning = GetAllowGlyphSkinning(Owner);
			tileBar.AppearanceItem.Assign(GetCombinedItemAppearances(Owner));
			tileBar.AppearanceGroupText.Assign(CombineAppearances(OptionsDropDown.AppearanceGroupText, Owner.OptionsPrimaryDropDown.AppearanceGroupText));
			tileBar.ShowItemShadow = GetShowItemShadow(Owner);
			tileBar.LookAndFeel.SkinName = Owner.LookAndFeel.ActiveSkinName;
			tileBar.LookAndFeel.UseDefaultLookAndFeel = false;
			NeedTileBarAppearanceUpdate = false;
		}
		protected internal override void OnPropertiesChanged(bool updateDropDown) {
			if(Owner == null) return;
			if(updateDropDown)
				Owner.DropDownItemsChanged();
			Owner.OnPropertiesChanged();
		}
		protected internal override void OnDropDownClosed() {
			if(Owner != null)
				(Owner as ITileBarWindowOwner).OnDropDownClosed();
		}
		protected internal override bool IsInCollection {
			get {
				if(this.OwnerCollection == null ||
					this.OwnerCollection.Owner == null ||
					!this.OwnerCollection.Owner.Categories.Contains(this))
					return false;
				return true;
			}
		}
		protected internal override void UpdateContainingDropDownItems() {
			var tnp = GetOwner();
			if(tnp != null)
				tnp.DropDownItemsChanged();
		}
		IComponent ITileNavButton.Component { get { return this; } }
		NavElement ITileNavButton.Element { get { return this; } }
		TileBarWindow ITileNavButton.DropDown { get { return DropDown; } }
		TileNavButtonCollection ITileNavButton.OwnerCollection { get; set; }
		void ITileNavButton.OnPropertiesChanged(bool updateDropDown) { OnPropertiesChanged(updateDropDown); }
		object ITileNavButton.Clone() { return Clone(); }
		protected internal TileNavCategory Clone() {
			TileNavCategory res = new TileNavCategory();
			res.Assign(this);
			return res;
		}
		public override void Assign(TileNavElement src) {
			base.Assign(src);
			this.Items.Assign((src as TileNavCategory).Items);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				for(int i = 0; i < Items.Count; i++)
					Items[i].Dispose();
			}
			base.Dispose(disposing);
		}
	}
	[Designer("DevExpress.XtraBars.Design.TileNavItemDesigner, " + AssemblyInfo.SRAssemblyBarsDesign)]
	public class TileNavItem : TileNavElement, ITileElementCollectionOwner {
		CollectionBase ITileElementCollectionOwner.Collection { get { return SubItems; } }
		public TileNavItem() {
			Tile = CreateTile();
		}
		[Browsable(false)]
		public TileNavCategory Category {
			get { return GetCategory(); }
		}
		protected virtual TileBarItem CreateTile() {
			return new TileBarItem();
		}
		protected virtual TileNavSubItemCollection CreateSubItems() {
			return new TileNavSubItemCollection(this);
		}
		protected override TileBarWindow CreateDropDown() {
			return CreateSubItemsDropDown();
		}
		protected override Control CreateDropDownContent() {
			return new TileBar() { Owner = GetOwner() };
		}
		internal DropDownShowMode DropDownMode { get; set; }
		[Browsable(false)]
		public TileNavItemCollection OwnerCollection { get; set; }
		TileNavSubItemCollection subItems;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TileNavSubItemCollection SubItems {
			get {
				if(subItems == null)
					subItems = CreateSubItems();
				return subItems;
			}
		}
		protected virtual TileBarWindow CreateSubItemsDropDown() {
			TileNavPane owner = GetOwner();
			if(owner == null) return null;
			TileBarWindow dropDown = new TileBarWindow(owner, this);
			dropDown.Text = "SubItems DropDown";
			dropDown.ApplyOptions();
			return dropDown;
		}
		protected internal override void UpdateTileBarItems(TileBar tileBar) {
			TileNavPaneDropDownHelper.ClearTileBar(tileBar);
			if(SubItems.Count == 0) return;
			CreateTileBarItems(tileBar);
			NeedTileBarItemsUpdate = false;
		}
		protected virtual void CreateTileBarItems(TileBar tileBar) {
			TileBarGroup group = new TileBarGroup();
			foreach(TileNavSubItem subitem in SubItems) {
				TileNavPaneDropDownHelper.AddTile(tileBar, subitem.Tile, subitem.GroupName, group);
			}
			tileBar.Groups.Add(group);
		}
		protected internal override void UpdateTileBarAppearances(TileBar tileBar) {
			TileNavPane owner = GetOwner();
			tileBar.Owner = owner;
			tileBar.ItemSize = GetDropDownItemSize(owner, 50);
			tileBar.WideTileWidth = GetDropDownWideItemWidth(owner, 125);
			tileBar.AllowGlyphSkinning = GetAllowGlyphSkinning(owner);
			tileBar.ShowItemShadow = GetShowItemShadow(owner);
			tileBar.LookAndFeel.SkinName = owner.LookAndFeel.ActiveSkinName;
			tileBar.LookAndFeel.UseDefaultLookAndFeel = false;
			tileBar.AppearanceItem.Assign(GetCombinedItemAppearances(owner));
			tileBar.AppearanceGroupText.Assign(CombineAppearances(OptionsDropDown.AppearanceGroupText, owner.OptionsSecondaryDropDown.AppearanceGroupText));
			if(DropDownMode == DropDownShowMode.FromTileNavPane)
				SetInvertedAppearance(tileBar);
			else {
				tileBar.SetItemsInvertedAppearance(true);
				tileBar.BackColor = GetDropDownBackColor(owner);
			}
			NeedTileBarAppearanceUpdate = false;
		}
		void SetInvertedAppearance(TileBar tileBar) {
			tileBar.SetItemsInvertedAppearance(false);
			if(OptionsDropDown.BackColor.IsEmpty)
				tileBar.BackColor = TileNavPane.GetButtonDropDownBackColor(TileNavPane.GetElementAppearanceSelected(this));
		}
		protected override FlyoutPanelOptions GetDropDownOptions() {
			var res = base.GetDropDownOptions();
			if(GetOwner().DropDownShowDown)
				PatchDropDownLocation(res);
			return res;
		}
		void PatchDropDownLocation(FlyoutPanelOptions opt) {
			var cat = GetCategory();
			if(cat == null || DropDownMode != DropDownShowMode.FromTileBar) return;
			opt.Location = new Point(0, (cat as ITileBarWindowOwner).GetDropDownContent().Height);
		}
		protected internal override void OnDropDownClosed() {
			var owner = GetOwner() as ITileBarWindowOwner;
			if(owner != null && DropDownMode == DropDownShowMode.FromTileNavPane)
				owner.OnDropDownClosed();
			else
				OnDropDownHide();
			DropDownMode = DropDownShowMode.FromTileBar;
			if(Tile.ItemInfo == null) return;
			(Tile.ItemInfo as ITileBarWindowOwner).OnDropDownClosed();
		}
		protected virtual void OnDropDownHide() {
			var owner = GetOwner();
			if(owner == null) return;
			owner.OnDropDownHidden(this);
		}
		protected internal override void OnPropertiesChanged(bool resetDropDownOptions) {
			TileNavPane owner = GetOwner();
			if(owner != null) owner.OnPropertiesChanged();
		}
		protected override TileNavPane GetOwner() {
			var cat = GetCategory();
			if(cat == null || cat.Owner == null)
				return null;
			return cat.Owner;
		}
		protected internal override bool IsInCollection {
			get {
				var cat = GetCategory();
				if(cat == null) return false;
				return cat.IsDefaultCategory ? true : cat.IsInCollection;
			}
		}
		protected internal override void UpdateContainingDropDownItems() {
			var cat = GetCategory();
			if(cat == null) return;
			cat.DropDownItemsChanged();
		}
		protected internal TileNavCategory GetCategory() {
			if(this.OwnerCollection == null || this.OwnerCollection.Category == null)
				return null;
			return this.OwnerCollection.Category;
		}
		protected internal TileNavItem Clone() {
			TileNavItem res = new TileNavItem();
			res.Assign(this);
			return res;
		}
		public override void Assign(TileNavElement src) {
			base.Assign(src);
			this.SubItems.Assign((src as TileNavItem).SubItems);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				foreach(TileNavSubItem si in SubItems)
					si.Dispose();
			}
			base.Dispose(disposing);
		}
	}
	public class TileNavSubItem : TileNavElement {
		public TileNavSubItem() {
			Tile = CreateTile();
			Tile.NavElement = this;
		}
		protected virtual TileBarItem CreateTile() {
			return new TileBarItem();
		}
		TileNavSubItemCollection collection;
		[Browsable(false)]
		public TileNavSubItemCollection OwnerCollection {
			get { return collection; }
			protected internal set {
				if(collection == value)
					return;
				collection = value;
				OnCollectionChanged();
			}
		}
		void OnCollectionChanged() { }
		protected internal override bool IsInCollection {
			get {
				var item = GetItem();
				return item != null && item.IsInCollection;
			}
		}
		protected override TileNavPane GetOwner() {
			if(!IsInCollection)
				return null;
			return GetItem().OwnerCollection.Category.Owner;
		}
		protected internal override void UpdateContainingDropDownItems() {
			var item = GetItem();
			if(item == null) return;
			item.DropDownItemsChanged();
		}
		protected internal TileNavItem GetItem() {
			if(OwnerCollection == null || OwnerCollection.Item == null)
				return null;
			return OwnerCollection.Item;
		}
		protected internal TileNavSubItem Clone() {
			TileNavSubItem res = new TileNavSubItem();
			res.Assign(this);
			return res;
		}
		[Browsable(false)]
		public TileNavItem Item {
			get { return GetItem(); }
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				Tile.NavElement = null;
			}
			base.Dispose(disposing);
		}
	}
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class TileNavElement : NavElement, ITileBarWindowOwner, ITileNavPaneDropDownOptionsOwner {
		TileBarItem tile;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Tile properties")]
		public TileBarItem Tile {
			get { return tile; }
			internal set {
				if(Tile == value)
					return;
				tile = value;
				OnPropertiesChanged(false);
			}
		}
		[DefaultValue(null)]
		public override string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		[Category("Tile properties")]
		[DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string TileText {
			get { return Tile.Text; }
			set { Tile.Text = value; }
		}
		[Category("Tile properties")]
		[DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), 
		Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image TileImage {
			get { return Tile != null ? Tile.Image : null; }
			set { if(Tile != null) Tile.Image = value; }
		}
		string groupName;
		[DefaultValue(null), Category(CategoryName.Properties)]
		public virtual string GroupName {
			get { return groupName; }
			set {
				if(groupName == value)
					return;
				groupName = value;
				UpdateContainingDropDownItems();
			}
		}
		TileNavPaneDropDownOptions optionsDropDown;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Behavior)]
		public TileNavPaneDropDownOptions OptionsDropDown {
			get {
				if(optionsDropDown == null)
					optionsDropDown = new TileNavPaneDropDownOptions(this);
				return optionsDropDown;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public TileNavPane TileNavPane { get { return GetOwner(); } }
		bool IsPrimaryDropDown {
			get { return !(this is TileNavItem); }
		}
		Control GetDropDownContent() { return DropDownContent; }
		Control dropDownContentCore;
		Control DropDownContent {
			get {
				if(dropDownContentCore == null)
					dropDownContentCore = CreateDropDownContent();
				return dropDownContentCore;
			}
		}
		protected internal TileBarWindow GetDropDown() { return DropDown; }
		TileBarWindow dropDownCore;
		protected internal TileBarWindow DropDown {
			get {
				if(dropDownCore == null)
					dropDownCore = CreateDropDown();
				return dropDownCore;
			}
		}
		protected internal void DropDownItemsChanged() {
			NeedTileBarItemsUpdate = true;
			if(dropDownCore != null)
				dropDownCore.UpdateTileBar();
		}
		protected internal bool NeedTileBarItemsUpdate { get; set; }
		protected internal bool NeedTileBarAppearanceUpdate { get { return true; } set { } }
		protected virtual TileNavPane GetOwner() { return null; }
		[Browsable(false)]
		protected virtual TileBarWindow CreateDropDown() { return null; }
		protected virtual Control CreateDropDownContent() { return null; }
		protected internal virtual bool IsInCollection { get { return false; } }
		protected internal virtual void OnDropDownClosed() { }
		protected internal virtual void UpdateTileBarItems(TileBar tileBar) { }
		protected internal virtual void UpdateTileBarAppearances(TileBar tileBar) { }
		protected internal virtual void UpdateContainingDropDownItems() { }
		protected virtual void OnUpdateTileBar(TileBar tileBar) {
			tileBar.BeginUpdate();
			try {
				if(NeedTileBarItemsUpdate) UpdateTileBarItems(tileBar);
				if(NeedTileBarAppearanceUpdate) { 
					UpdateTileBarAppearances(tileBar);
					UpdateTileBarToolTipController(tileBar);
				}
			}
			finally { tileBar.EndUpdate(); }
		}
		protected virtual void UpdateTileBarToolTipController(TileBar tileBar) {
			var owner = GetOwner();
			if(owner == null || tileBar == null) return;
			tileBar.ToolTipController = owner.ToolTipController;
		}
		protected internal virtual bool GetCloseOnOuterClick() {
			TileNavPane owner = GetOwner();
			TileNavPaneDropDownOptions opt = GetOwnerOptions(owner);
			if(OptionsDropDown.CloseOnOuterClick != DefaultBoolean.Default)
				return OptionsDropDown.CloseOnOuterClick.ToBoolean(true);
			return opt.CloseOnOuterClick.ToBoolean(true);
		}
		protected internal int GetDropDownHeight(TileNavPane owner) {
			if(owner == null) return 0;
			if(OptionsDropDown.Height > 0)
				return OptionsDropDown.Height;
			return owner.GetDropDownHeight(IsPrimaryDropDown);
		}
		protected virtual TileItemAppearances GetCombinedItemAppearances(TileNavPane owner) {
			TileItemAppearances result = new TileItemAppearances();
			AppearanceObject norm = new AppearanceObject();
			AppearanceObject pressed = new AppearanceObject();
			AppearanceObject hovered = new AppearanceObject();
			AppearanceObject selected = new AppearanceObject();
			TileNavPaneDropDownOptions ownerOptions = GetOwnerOptions(owner);
			AppearanceHelper.Combine(norm, new AppearanceObject[] { OptionsDropDown.AppearanceItem.Normal, ownerOptions.AppearanceItem.Normal });
			AppearanceHelper.Combine(pressed, new AppearanceObject[] { OptionsDropDown.AppearanceItem.Pressed, ownerOptions.AppearanceItem.Pressed });
			AppearanceHelper.Combine(hovered, new AppearanceObject[] { OptionsDropDown.AppearanceItem.Hovered, ownerOptions.AppearanceItem.Hovered });
			AppearanceHelper.Combine(selected, new AppearanceObject[] { OptionsDropDown.AppearanceItem.Selected, ownerOptions.AppearanceItem.Selected });
			result.Normal.Assign(norm);
			result.Pressed.Assign(pressed);
			result.Hovered.Assign(hovered);
			result.Selected.Assign(selected);
			return result;
		}
		protected AppearanceObject CombineAppearances(AppearanceObject childAppearance, AppearanceObject rootAppearance) {
			AppearanceObject result = new AppearanceObject();
			AppearanceHelper.Combine(result, new AppearanceObject[] { childAppearance, rootAppearance });
			return result;
		}
		protected bool GetAllowGlyphSkinning(TileNavPane owner) {
			TileNavPaneDropDownOptions opt = GetOwnerOptions(owner);
			if(OptionsDropDown.AllowGlyphSkinning != DefaultBoolean.Default)
				return OptionsDropDown.AllowGlyphSkinning.ToBoolean(false);
			if(opt.AllowGlyphSkinning != DefaultBoolean.Default)
				return opt.AllowGlyphSkinning.ToBoolean(false);
			return owner.AllowGlyphSkinning;
		}
		protected internal int GetDropDownItemSize(TileNavPane owner, int defaultValue) {
			TileNavPaneDropDownOptions opt = GetOwnerOptions(owner);
			if(OptionsDropDown.ItemHeight > 0)
				return OptionsDropDown.ItemHeight;
			return opt.ItemHeight > 0 ? opt.ItemHeight : defaultValue;
		}
		protected internal int GetDropDownItemSize(TileNavPane owner) {
			return GetDropDownItemSize(owner, 0);
		}
		protected internal int GetDropDownWideItemWidth(TileNavPane owner, int defaultValue) {
			TileNavPaneDropDownOptions opt = GetOwnerOptions(owner);
			if(OptionsDropDown.WideItemWidth > 0)
				return OptionsDropDown.WideItemWidth;
			return opt.WideItemWidth > 0 ? opt.WideItemWidth : defaultValue;
		}
		protected internal int GetDropDownWideItemWidth(TileNavPane owner) {
			return GetDropDownWideItemWidth(owner, 0);
		}
		protected internal Color GetDropDownBackColor(TileNavPane owner) {
			TileNavPaneDropDownOptions opt = GetOwnerOptions(owner);
			if(OptionsDropDown.BackColor.IsEmpty)
				return opt.BackColor.IsEmpty ? GetDropDownBackColorCore(owner) : opt.BackColor;
			return OptionsDropDown.BackColor;
		}
		Color GetDropDownBackColorCore(TileNavPane owner) {
			if(this is TileNavItem) {
				TileBar tileBar = Tile.Control as TileBar;
				if(tileBar != null)
					return tileBar.GetItemNormalAppearance(Tile).BackColor;
				return Color.Red;
			}
			return owner.GetButtonDropDownBackColor(owner.GetElementAppearanceSelected(this));
		}
		protected bool GetShowItemShadow(TileNavPane owner) {
			TileNavPaneDropDownOptions opt = GetOwnerOptions(owner);
			if(OptionsDropDown.ShowItemShadow != DefaultBoolean.Default)
				return OptionsDropDown.ShowItemShadow.ToBoolean(false);
			return opt.ShowItemShadow.ToBoolean(false);
		}
		TileNavPaneDropDownOptions GetOwnerOptions(TileNavPane owner) {
			return IsPrimaryDropDown ? owner.OptionsPrimaryDropDown : owner.OptionsSecondaryDropDown;
		}
		void ITileNavPaneDropDownOptionsOwner.OnDropDownOptionsChanged(bool updateDropDown) {
		}
		bool ITileBarWindowOwner.IsTileNavPane { get { return true; } }
		TileBarWindow ITileBarWindowOwner.GetDropDown() { return GetDropDown(); }
		void ITileBarWindowOwner.OnDropDownClosed() { OnDropDownClosed(); }
		bool ITileBarWindowOwner.CloseOnOuterClick { get { return GetCloseOnOuterClick(); } }
		void ITileBarWindowOwner.UpdateTileBar(TileBar tb) { OnUpdateTileBar(tb); }
		Control ITileBarWindowOwner.GetDropDownContent() { return GetDropDownContent(); }
		FlyoutPanelOptions ITileBarWindowOwner.DropDownOptions { get { return GetDropDownOptions(); } }
		int ITileBarWindowOwner.DropDownHeight { get { return GetDropDownHeight(GetOwner()); } }
		Color ITileBarWindowOwner.DropDownBackColor { get { return GetDropDownBackColor(GetOwner()); } }
		Orientation ITileBarWindowOwner.Orientation { get { return Orientation.Horizontal; } }
		protected virtual FlyoutPanelOptions GetDropDownOptions() {
			var owner = GetOwner();
			if(owner == null) return null;
			return owner.GetDropDownOptions(this);
		}
		public virtual void Assign(TileNavElement src) {
			Appearance.Assign(src.Appearance);
			AppearanceHovered.Assign(src.AppearanceHovered);
			AppearanceSelected.Assign(src.AppearanceSelected);
			this.Alignment = src.Alignment;
			this.AllowGlyphSkinning = src.AllowGlyphSkinning;
			this.Caption = src.Caption;
			this.Enabled = src.Enabled;
			this.Glyph = src.Glyph;
			this.GlyphAlignment = src.GlyphAlignment;
			this.GroupName = src.GroupName;
			this.OptionsDropDown.Assign(src.OptionsDropDown);
			this.Padding = src.Padding;
			this.Tag = src.Tag;
			this.TileImage = src.TileImage;
			this.TileText = src.TileText;
			this.Visible = src.Visible;
			(this.Tile as ITileItem).Properties.Assign(src.Tile);
			this.Tile.AppearanceItem.Assign(src.Tile.AppearanceItem);
			this.Tile.Elements.Assign(src.Tile.Elements);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(dropDownCore != null) DropDown.Dispose();
				dropDownCore = null;
				TileImage = null;
				if(Tile != null) Tile.Dispose();
				Tile = null;
			}
			base.Dispose(disposing);
		}
	}
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class NavButton : NavElement, ITileNavButton {
		NavElement ITileNavButton.Element { get { return this; } }
		bool isMain;
		[DefaultValue(false), Category(CategoryName.Properties)]
		public bool IsMain {
			get { return isMain; }
			set {
				if(isMain == value) return;
				isMain = value;
				if(isMain)
					OnIsMainChanged();
			}
		}
		protected void OnIsMainChanged() { 
			var owner = GetOwner();
			if(owner != null)
				owner.OnMainButtonChanged(this);
		}
		TileBarWindow ITileNavButton.DropDown { get { return null; } }
		TileNavButtonCollection ITileNavButton.OwnerCollection { get; set; }
		void ITileNavButton.OnPropertiesChanged(bool updateDropDown) {
			var owner = GetOwner();
			if(owner != null)
				owner.OnPropertiesChanged();
		}
		TileNavPane GetOwner() {
			var collection = ((ITileNavButton)this).OwnerCollection;
			if(collection == null || collection.Owner == null) return null;
			return collection.Owner;
		}
		IComponent ITileNavButton.Component { get { return this; } }
		object ITileNavButton.Clone() { return Clone(); }
		NavButton Clone() {
			NavButton res = new NavButton();
			res.Assign(this);
			return res;
		}
		public void Assign(NavButton src) {
			this.Alignment = src.Alignment;
			this.AllowGlyphSkinning = src.AllowGlyphSkinning;
			this.Appearance.Assign(src.Appearance);
			this.AppearanceHovered.Assign(src.AppearanceHovered);
			this.AppearanceSelected.Assign(src.AppearanceSelected);
			this.Caption = src.Caption;
			this.Enabled = src.Enabled;
			this.Glyph = src.Glyph;
			this.GlyphAlignment = src.GlyphAlignment;
			this.Padding = src.Padding;
			this.Tag = src.Tag;
			this.Visible = src.Visible;
			this.IsMain = src.IsMain;
		}
	}
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class NavElement : Component {
		private static readonly object elementClick = new object();
		string name = "";
		[DefaultValue(""), Browsable(false), XtraSerializableProperty]
		public virtual string Name {
			get {
				if(Site == null) return name;
				return Site.Name;
			}
			set {
				if(Site != null) {
					Site.Name = value;
					name = Site.Name;
				}
				else {
					name = value;
				}
			}
		}
		SuperToolTip superTip;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemSuperTip"),
#endif
 Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(UITypeEditor)), SmartTagProperty("Super Tip", "Appearance", 7), Category("Appearance")]
		public virtual SuperToolTip SuperTip {
			get { return superTip; }
			set { superTip = value; }
		}
		protected virtual bool ShouldSerializeSuperTip() { return SuperTip != null && !SuperTip.IsEmpty; }
		public virtual void ResetSuperTip() { SuperTip = null; }
		bool enabled = true;
		[DefaultValue(true), Category(CategoryName.Behavior)]
		public bool Enabled {
			get { return enabled; }
			set {
				if(value == enabled) return;
				enabled = value;
				OnPropertiesChanged(false);
			}
		}
		[DefaultValue(null), Category(CategoryName.Data), SmartTagProperty("Tag", ""),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), 
			typeof(UITypeEditor)), TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public object Tag { get; set; }
		Padding padding = DefaultPadding;
		internal static Padding DefaultPadding { get { return new Padding(-1); } }
		bool ShouldSerializePadding() { return Padding != DefaultPadding; }
		void ResetPadding() { Padding = DefaultPadding; }
		[Category(CategoryName.Appearance)]
		public Padding Padding {
			get { return padding; }
			set {
				if(Padding == value)
					return;
				padding = value;
				OnPropertiesChanged(false);
			}
		}
		string caption;
		[Category(CategoryName.Properties)]
		public virtual string Caption {
			get { return caption; }
			set {
				if(Caption == value)
					return;
				caption = value;
				OnPropertiesChanged(false);
			}
		}
		Image glyph;
		[DefaultValue(null), Category(CategoryName.Appearance), 
		Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image Glyph {
			get { return glyph; }
			set {
				if(Glyph == value)
					return;
				glyph = value;
				OnPropertiesChanged(false);
			}
		}
		bool visible = true;
		[DefaultValue(true), Category(CategoryName.Properties)]
		public virtual bool Visible {
			get { return visible; }
			set {
				if(visible == value) return;
				visible = value;
				OnPropertiesChanged(false);
			}
		}
		NavButtonAlignment alignment;
		[DefaultValue(NavButtonAlignment.Default), Category(CategoryName.Appearance)]
		public virtual NavButtonAlignment Alignment {
			get { return alignment; }
			set {
				if(alignment == value) return;
				alignment = value;
				OnPropertiesChanged(false);
			}
		}
		NavButtonAlignment glyphAlignment;
		[DefaultValue(NavButtonAlignment.Default), Category(CategoryName.Appearance)]
		public virtual NavButtonAlignment GlyphAlignment {
			get { return glyphAlignment; }
			set {
				if(glyphAlignment == value) return;
				glyphAlignment = value;
				OnPropertiesChanged(false);
			}
		}
		DefaultBoolean allowGlyphSkinning = DefaultBoolean.Default;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemAllowGlyphSkinning"),
#endif
 DefaultValue(DefaultBoolean.Default), 
		Category("Appearance"), SmartTagProperty("Allow Glyph Skinning", "Image", 4)]
		public virtual DefaultBoolean AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value)
					return;
				allowGlyphSkinning = value;
				OnPropertiesChanged(false);
			}
		}
		void ResetAppearance() { Appearance.Reset(); }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		AppearanceObject appearance;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Appearance)]
		public AppearanceObject Appearance {
			get {
				if(appearance == null) {
					appearance = new AppearanceObject();
					appearance.Changed += OnAppearanceChanged;
				}
				return appearance;
			}
		}
		void ResetAppearanceSelected() { AppearanceSelected.Reset(); }
		bool ShouldSerializeAppearanceSelected() { return AppearanceSelected.ShouldSerialize(); }
		AppearanceObject appearanceSelected;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Appearance)]
		public AppearanceObject AppearanceSelected {
			get {
				if(appearanceSelected == null) {
					appearanceSelected = new AppearanceObject();
					appearanceSelected.Changed += OnAppearanceChanged;
				}
				return appearanceSelected;
			}
		}
		void ResetAppearanceHovered() { AppearanceHovered.Reset(); }
		bool ShouldSerializeAppearanceHovered() { return AppearanceHovered.ShouldSerialize(); }
		AppearanceObject appearanceHovered;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Appearance)]
		public AppearanceObject AppearanceHovered {
			get {
				if(appearanceHovered == null) {
					appearanceHovered = new AppearanceObject();
					appearanceHovered.Changed += OnAppearanceChanged;
				}
				return appearanceHovered;
			}
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			OnPropertiesChanged(false);
		}
		protected internal virtual void OnPropertiesChanged(bool updateDropDown) {
			if(this is ITileNavButton) (this as ITileNavButton).OnPropertiesChanged(updateDropDown);
		}
		[Category(CategoryName.Behavior)]
		public event NavElementClickEventHandler ElementClick {
			add { Events.AddHandler(elementClick, value); }
			remove { Events.RemoveHandler(elementClick, value); }
		}
		protected internal virtual void OnElementClick(bool isTile) {
			RaiseElementClick(isTile);
		}
		protected internal void RaiseElementClick(bool isTile) {
			NavElementEventArgs e = new NavElementEventArgs() { Element = this, IsTile = isTile };
			NavElementClickEventHandler handler = Events[elementClick] as NavElementClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				Glyph = null;
				ViewInfo = null;
				if(appearance != null) 
					appearance.Changed -= OnAppearanceChanged;
				if(appearanceHovered != null)
					appearanceHovered.Changed -= OnAppearanceChanged;
				if(appearanceSelected != null)
					appearanceSelected.Changed -= OnAppearanceChanged;
			}
			base.Dispose(disposing);
		}
		protected internal TileNavButtonViewInfo ViewInfo { get; set; }
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class TileNavPaneDropDownOptions {
		ITileNavPaneDropDownOptionsOwner Owner { get; set; }
		public TileNavPaneDropDownOptions(ITileNavPaneDropDownOptionsOwner owner) {
			this.showItemShadow = DefaultBoolean.Default;
			this.height = 0;
			this.Owner = owner;
			this.allowGlyphSkinningCore = DefaultBoolean.Default;
			this.closeOnOuterClickCore = DefaultBoolean.Default;
		}
		public void Assign(TileNavPaneDropDownOptions src) {
			this.AppearanceGroupText.Assign(src.AppearanceGroupText);
			this.AppearanceItem.Assign(src.AppearanceItem);
			this.BackColor = src.BackColor;
			this.Height = src.Height;
			this.ItemHeight = src.ItemHeight;
			this.WideItemWidth = src.WideItemWidth;
			this.ShowItemShadow = src.ShowItemShadow;
			this.AllowGlyphSkinning = src.AllowGlyphSkinning;
			this.CloseOnOuterClick = src.CloseOnOuterClick;
		}
		int itemHeightCore;
		[DefaultValue(0)]
		public int ItemHeight {
			get { return itemHeightCore; }
			set {
				if(itemHeightCore == value) return;
				itemHeightCore = value;
				OnPropertiesChanged(true);
			}
		}
		int wideItemWidthCore;
		[DefaultValue(0)]
		public int WideItemWidth {
			get { return wideItemWidthCore; }
			set {
				if(wideItemWidthCore == value) return;
				wideItemWidthCore = value;
				OnPropertiesChanged(true);
			}
		}
		DefaultBoolean closeOnOuterClickCore;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean CloseOnOuterClick {
			get { return closeOnOuterClickCore; }
			set {
				if(closeOnOuterClickCore == value) return;
				closeOnOuterClickCore = value;
			}
		}
		DefaultBoolean allowGlyphSkinningCore;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean AllowGlyphSkinning {
			get { return allowGlyphSkinningCore; }
			set {
				if(allowGlyphSkinningCore == value) return;
				allowGlyphSkinningCore = value;
				OnPropertiesChanged(true);
			}
		}
		void ResetAppearanceItem() { AppearanceItem.Reset(); }
		bool ShouldSerializeAppearanceItem() { return AppearanceItem.ShouldSerialize(); }
		TileItemAppearances appearances;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Appearance)]
		public TileItemAppearances AppearanceItem {
			get {
				if(appearances == null) {
					appearances = new TileItemAppearances((IAppearanceOwner)null);
					appearances.Changed += appearances_Changed;
				}
				return appearances;
			}
		}
		void ResetAppearanceGroupText() { AppearanceGroupText.Reset(); }
		bool ShouldSerializeAppearanceGroupText() { return AppearanceGroupText.ShouldSerialize(); }
		AppearanceObject appearanceGroupText;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Appearance)]
		public AppearanceObject AppearanceGroupText {
			get {
				if(appearanceGroupText == null) {
					appearanceGroupText = new AppearanceObject();
					appearanceGroupText.Changed += new EventHandler(appearances_Changed);
				}
				return appearanceGroupText;
			}
		}
		Color backColorCore;
		public Color BackColor {
			get { return backColorCore; }
			set {
				if(backColorCore == value) return;
				backColorCore = value;
				OnPropertiesChanged(true);
			}
		}
		bool ShouldSerializeBackColor { get { return !BackColor.IsEmpty; } }
		void ResetBackColor() { BackColor = Color.Empty; }
		DefaultBoolean showItemShadow;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ShowItemShadow {
			get { return showItemShadow; }
			set {
				if(showItemShadow == value) return;
				showItemShadow = value;
				OnPropertiesChanged(true);
			}
		}
		int height;
		[DefaultValue(0)]
		public int Height {
			get { return height; }
			set {
				if(height == value) return;
				height = value;
				OnPropertiesChanged(false);
			}
		}
		void appearances_Changed(object sender, EventArgs e) {
			OnPropertiesChanged(true);
		}
		void OnPropertiesChanged(bool updateDropDown) {
			Owner.OnDropDownOptionsChanged(updateDropDown);
		}
	}
	internal static class TileNavPaneDropDownHelper {
		public static void ClearTileBar(TileBar tileBar) {
			while(tileBar.Groups.Count > 0) {
				while(tileBar.Groups[0].Items.Count > 0)
					tileBar.Groups[0].Items.RemoveAt(0);
				tileBar.Groups.RemoveAt(0);
			}
		}
		public static void UpdateDropDownLocation(TileBarWindow dropDown, Control owner) {
			if(dropDown == null || owner == null) return;
			dropDown.Options.Location = new Point(0, owner.Height);
		}
		public static void AddTile(TileBar tileBar, TileBarItem tile, string groupName, TileBarGroup groupWithoutText) {
			if(!string.IsNullOrEmpty(groupName)) {
				TileBarGroup targetGroup = GetGroupByGroupText(tileBar, groupName);
				targetGroup.Items.Add(tile);
				if(targetGroup.Control == null)
					tileBar.Groups.Add(targetGroup);
			}
			else
				groupWithoutText.Items.Add(tile);
		}
		public static TileBarGroup GetGroupByGroupText(TileBar tileBar, string text) {
			foreach(TileBarGroup g in tileBar.Groups) {
				if(g.Text == text) return g;
			}
			return new TileBarGroup() { Text = text };
		}
		public static bool HasVisibleTiles(IEnumerable<TileNavElement> collection) {
			foreach(TileNavElement element in collection) {
				if(element.Tile.Visible) return true;
			}
			return false;
		} 
	}
#region INTERFACES
	public interface ITileNavButton {
		IComponent Component { get; }
		bool Visible { get; set; }
		string Caption { get; set; }
		Image Glyph { get; set; }
		NavElement Element { get; }
		TileBarWindow DropDown { get; }
		NavButtonAlignment Alignment { get; set; }
		NavButtonAlignment GlyphAlignment { get; set; }
		TileNavButtonCollection OwnerCollection { get; set; }
		void OnPropertiesChanged(bool updateDropDown);
		object Clone();
	}
	public interface ITileElementCollectionOwner {
		CollectionBase Collection { get; }
	}
	public interface ITileNavPaneDropDownOptionsOwner {
		void OnDropDownOptionsChanged(bool updateDropDown);
	}
	public interface ITileNavPaneDesigner {
		bool DebuggingState { get; }
	}
#endregion INTERFACES
#region COLLECTIONS
	public class TileNavElementCollectionBase<T> : CollectionBase where T : TileNavElement {
		public void AddRange(T[] elements) {
			BeginUpdate();
			try {
				foreach(T element in elements)
					Add(element);
			}
			finally { EndUpdate(); }
		}
		public int Add(T element) {
			RemoveFromCurrentCollection(element);
			return List.Add(element);
		}
		public void Insert(int index, T element) {
			RemoveFromCurrentCollection(element);
			if(index >= Count)
				Add(element);
			else
				List.Insert(index, element);
		}
		public void Remove(T element) {
			if(List.Contains(element)) List.Remove(element);
		}
		public int IndexOf(T element) { return List.IndexOf(element); }
		public bool Contains(T element) { return List.Contains(element); }
		public T this[int index] { get { return (T)List[index]; } set { List[index] = value; } }
		protected override void OnClearComplete() {
			base.OnClearComplete();
			RaiseCollectionChanged();
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			SetCollection((T)value);
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			ResetSelectedElement((T)value);
			ClearCollection((T)value);
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			ClearCollection((T)oldValue);
			SetCollection((T)newValue);
		}
		void SetCollection(T element) {
			OnSetCollection(element);
			RaiseCollectionChanged();
		}
		void ClearCollection(T element) {
			OnClearCollection(element);
			RaiseCollectionChanged();
		}
		int lockUpdate = 0;
		public void BeginUpdate() {
			lockUpdate++;
		}
		public void EndUpdate() {
			if(--lockUpdate == 0)
				OnCollectionChanged();
		}
		void RaiseCollectionChanged() {
			if(lockUpdate == 0)
				OnCollectionChanged();
		}
		protected virtual void OnCollectionChanged() { }
		protected virtual void OnSetCollection(T element) { }
		protected virtual void OnClearCollection(T element) { }
		protected virtual void ResetSelectedElement(T element) { }
		protected virtual void RemoveFromCurrentCollection(T element) { }
	}
	public class TileNavCategoryCollection : TileNavElementCollectionBase<TileNavCategory>, IEnumerable<TileNavCategory> {
		public TileNavPane Owner { get; private set; }
		public TileNavCategoryCollection(TileNavPane owner) {
			Owner = owner;
		}
		protected override void OnClearCollection(TileNavCategory element) {
			if(element != null)
				element.OwnerCollection = null;
		}
		protected override void OnSetCollection(TileNavCategory element) {
			if(element != null)
				element.OwnerCollection = this;
		}
		protected override void RemoveFromCurrentCollection(TileNavCategory cat) {
			if(cat.OwnerCollection != null)
				cat.OwnerCollection.Remove(cat);
			if(((ITileNavButton)cat).OwnerCollection != null)
				((ITileNavButton)cat).OwnerCollection.Remove(cat);
		}
		protected void ResetDropDown() {
			if(Owner == null) return;
			Owner.DropDownItemsChanged();
		}
		protected override void ResetSelectedElement(TileNavCategory category) {
			NavElement selected = Owner.SelectedElement;
			if(selected is TileNavCategory) {
				if(selected == category) ResetSelectedElementCore();
			}
			else if(selected is TileNavItem) {
				TileNavItem item = selected as TileNavItem;
				if(item.OwnerCollection != null && item.OwnerCollection.Category != null && item.OwnerCollection.Category == category)
					ResetSelectedElementCore();
			}
			else if(selected is TileNavSubItem) {
				TileNavSubItem si = selected as TileNavSubItem;
				if(si.OwnerCollection != null && si.OwnerCollection.Item != null && si.OwnerCollection.Item.OwnerCollection != null &&
					si.OwnerCollection.Item.OwnerCollection.Category != null && si.OwnerCollection.Item.OwnerCollection.Category == category)
					ResetSelectedElementCore();
			}
		}
		void ResetSelectedElementCore() {
			Owner.SelectedElement = null;
		}
		protected override void OnCollectionChanged() {
			ResetDropDown();
			if(Owner != null)
				Owner.OnPropertiesChanged();
		}
		public void Assign(TileNavCategoryCollection src) {
			Clear();
			foreach(TileNavCategory cat in src) {
				Add((TileNavCategory)cat.Clone());
			}
		}
		IEnumerator<TileNavCategory> IEnumerable<TileNavCategory>.GetEnumerator() {
			foreach(TileNavCategory cat in InnerList)
				yield return cat;
		}
	}
	public class TileNavItemCollection : TileNavElementCollectionBase<TileNavItem>, IEnumerable<TileNavItem> {
		public TileNavItemCollection(TileNavCategory category) {
			Category = category;
		}
		public TileNavCategory Category { get; private set; }
		protected override void RemoveFromCurrentCollection(TileNavItem item) {
			if(item.OwnerCollection != null) item.OwnerCollection.Remove(item);
		}
		protected override void ResetSelectedElement(TileNavItem item) {
			if(!item.IsInCollection) return;
			if(item.TileNavPane.SelectedElement == null) return;
			NavElement selected = item.TileNavPane.SelectedElement;
			if(selected is TileNavItem) {
				if(selected as TileNavItem == item)
					ResetSelectedElementCore(Category);
			}
			else if(selected is TileNavSubItem) {
				TileNavSubItem si = selected as TileNavSubItem;
				if(si.OwnerCollection != null && si.OwnerCollection.Item != null && si.OwnerCollection.Item == item)
					ResetSelectedElementCore(Category);
			}
		}
		void ResetSelectedElementCore(TileNavElement element) {
			Category.Owner.SelectedElement = element;
		}
		protected override void OnCollectionChanged() {
			if(Category != null)
				Category.DropDownItemsChanged();
			if(Category.Owner != null) 
				Category.Owner.OnPropertiesChanged();
		}
		protected override void OnClearCollection(TileNavItem element) {
			if(element != null) element.OwnerCollection = null;
		}
		protected override void OnSetCollection(TileNavItem element) {
			if(element != null) element.OwnerCollection = this;
		}
		internal void Assign(TileNavItemCollection src) {
			Clear();
			foreach(TileNavItem item in src) {
				Add((TileNavItem)item.Clone());
			}
		}
		IEnumerator<TileNavItem> IEnumerable<TileNavItem>.GetEnumerator() {
			foreach(TileNavItem item in InnerList)
				yield return item;
		}
	}
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)),
	RefreshProperties(RefreshProperties.All)]
	public class TileNavSubItemCollection : TileNavElementCollectionBase<TileNavSubItem>, IEnumerable<TileNavSubItem> {
		public TileNavSubItemCollection() { }
		public TileNavSubItemCollection(TileNavItem item) { Item = item; }
		public TileNavItem Item { get; private set; }
		protected override void RemoveFromCurrentCollection(TileNavSubItem subItem) {
			if(subItem.OwnerCollection != null) subItem.OwnerCollection.Remove(subItem);
		}
		protected override void ResetSelectedElement(TileNavSubItem subItem) {
			if(!subItem.IsInCollection)
				return;
			if(subItem.TileNavPane.SelectedElement == null)
				return;
			NavElement selected = subItem.TileNavPane.SelectedElement;
			if(selected is TileNavSubItem) {
				if(selected as TileNavSubItem == subItem)
					ResetSelectedElementCore(Item);
			}
		}
		void ResetSelectedElementCore(TileNavElement element) {
			Item.OwnerCollection.Category.Owner.SelectedElement = element;
		}
		protected override void OnCollectionChanged() {
			if(Item == null) return;
			Item.DropDownItemsChanged();
			Item.OnPropertiesChanged(false);
			var cat = Item.GetCategory();
			if(cat == null) return;
			cat.DropDownItemsChanged();
		}
		protected override void OnSetCollection(TileNavSubItem element) {
			if(element != null) element.OwnerCollection = this;
		}
		protected override void OnClearCollection(TileNavSubItem element) {
			if(element != null) element.OwnerCollection = null;
		}
		internal void Assign(TileNavSubItemCollection src) {
			Clear();
			foreach(TileNavSubItem subitem in src) {
				Add((TileNavSubItem)subitem.Clone());
			}
		}
		IEnumerator<TileNavSubItem> IEnumerable<TileNavSubItem>.GetEnumerator() {
			foreach(TileNavSubItem subItem in InnerList)
				yield return subItem;
		}
	}
	public class TileNavButtonCollection : CollectionBase, IEnumerable<ITileNavButton> {
		public TileNavButtonCollection() { }
		public TileNavButtonCollection(TileNavPane ownerControl) { Owner = ownerControl; }
		public TileNavPane Owner;
		public bool Contains(ITileNavButton button) { return List.Contains(button); }
		public int IndexOf(ITileNavButton button) { return List.IndexOf(button); }
		public int Add(ITileNavButton button) {
			RemoveFromCurrentCollection(button);
			return List.Add(button);
		}
		public void Insert(int index, ITileNavButton button) {
			RemoveFromCurrentCollection(button);
			if(index >= List.Count)
				List.Add(button);
			else List.Insert(index, button);
		}
		public void Remove(ITileNavButton button) { List.Remove(button); }
		public ITileNavButton this[int index] { get { return (ITileNavButton)List[index]; } set { List[index] = value; } }
		void RemoveFromCurrentCollection(ITileNavButton button) {
			if(button.OwnerCollection != null)
				button.OwnerCollection.Remove(button);
			if(button is TileNavCategory && ((TileNavCategory)button).OwnerCollection != null && ((TileNavCategory)button).OwnerCollection.Owner != null)
				((TileNavCategory)button).OwnerCollection.Owner.Categories.Remove(button as TileNavCategory);
		}
		protected override void OnInsertComplete(int index, object value) {
			((ITileNavButton)value).OwnerCollection = this;
			base.OnInsertComplete(index, value);
			OnCollectionChanged();
		}
		protected override void OnRemoveComplete(int index, object value) {
			((ITileNavButton)value).OwnerCollection = null;
			base.OnRemoveComplete(index, value);
			OnCollectionChanged();
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			((ITileNavButton)newValue).OwnerCollection = this;
			((ITileNavButton)oldValue).OwnerCollection = null;
			base.OnSetComplete(index, oldValue, newValue);
			OnCollectionChanged();
		}
		void OnCollectionChanged() {
			if(Owner != null) {
				Owner.OnPropertiesChanged();
			}
		}
		internal void Assign(TileNavButtonCollection src) {
			Clear();
			foreach(ITileNavButton but in src) {
				Add((ITileNavButton)but.Clone());
			}
		}
		IEnumerator<ITileNavButton> IEnumerable<ITileNavButton>.GetEnumerator() {
			foreach(ITileNavButton button in InnerList)
				yield return button;
		}
	}
#endregion COLLECTIONS
#region DESIGNTIME
	public class TileNavCategoryDesignTimeActionsProvider {
		public void AddItem(IComponent component) {
			TileNavPaneDesignTimeManagerBase designTimeManger = GetDesignTimeManager(component);
			if(designTimeManger != null) designTimeManger.OnAddItemToCategoryClick(component as TileNavCategory);
		}
		public void RemoveCategory(IComponent component) {
			TileNavCategory cat = (TileNavCategory)component;
			TileNavPaneDesignTimeManagerBase designTimeManger = GetDesignTimeManager(component);
			if(designTimeManger != null) designTimeManger.RemoveCategory(cat, true);
		}
		public void EditItems(IComponent component) {
			TileNavCategory cat = (TileNavCategory)component;
			TileNavPaneDesignTimeManagerBase designTimeManger = GetDesignTimeManager(component);
			if(designTimeManger != null) designTimeManger.EditItems(cat);
		}
		protected TileNavPaneDesignTimeManagerBase GetDesignTimeManager(IComponent component) {
			TileNavCategory cat = (TileNavCategory)component;
			return cat.Owner.ViewInfo.DesignTimeManager;
		}
	}
	public class TileNavCategoryDesignTimeBoundsProvider : ISmartTagClientBoundsProvider {
		public Rectangle GetBounds(IComponent component) {
			TileNavCategory cat = (TileNavCategory)component;
			TileNavPaneViewInfo vi = cat.Owner.ViewInfo;
			foreach(TileNavButtonViewInfo buttonInfo in vi.Buttons) {
				if(buttonInfo.Element == null || !(buttonInfo.Element is TileNavCategory)) continue;
				if(object.ReferenceEquals((TileNavCategory)buttonInfo.Element, cat))
					return buttonInfo.Bounds;
			}
			return Rectangle.Empty;
		}
		public Control GetOwnerControl(IComponent component) {
			TileNavCategory cat = (TileNavCategory)component;
			return cat.Owner as TileNavPane;
		}
	}
	public class TileNavPaneDesignTimeManagerBase : BaseDesignTimeManager {
		public TileNavPaneDesignTimeManagerBase(IComponent component, TileNavPane tileNavPane)
			: base(component, tileNavPane.Site) {
			TileNavPane = tileNavPane;
			Component = component;
			if(ComponentChangeService != null)
				ComponentChangeService.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
		}
		public TileNavPane TileNavPane { get; private set; }
		public IComponent Component { get; private set; }
		IComponentChangeService componentChangeService;
		public IComponentChangeService ComponentChangeService {
			get {
				if(componentChangeService == null)
					componentChangeService = TileNavPane.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				return componentChangeService;
			}
		}
		void OnComponentRemoved(object sender, ComponentEventArgs e) {
			TileNavPane tileNavPane = e.Component as TileNavPane;
			if(tileNavPane != null) {
				while(tileNavPane.Categories.Count > 0)
					RemoveCategory(tileNavPane.Categories[0]);
				while(tileNavPane.Buttons.Count > 0)
					RemoveButton(tileNavPane.Buttons[0]);
				RemoveCategory(tileNavPane.DefaultCategory);
			}
			else if(e.Component is TileNavItem)
				RemoveItem((TileNavItem)e.Component, false);
			else if(e.Component is TileNavCategory)
				RemoveCategory((TileNavCategory)e.Component, false);
			if(e.Component is ITileNavButton)
				RemoveButton((ITileNavButton)e.Component, false);
		}
		public virtual void RemoveButton(ITileNavButton button) { RemoveButton(button, true); }
		public virtual void RemoveButton(ITileNavButton button, bool removeFromContainer) {
			if(TileNavPane.DebuggingState)
				return;
			if(removeFromContainer && button.OwnerCollection != null && button.OwnerCollection.Owner != null)
				button.OwnerCollection.Owner.Container.Remove(button.Component);
			if(button.OwnerCollection != null && button.OwnerCollection.Owner != null) {
				if(button is TileNavCategory) {
					while(((TileNavCategory)button).Items.Count > 0) {
						RemoveItem(((TileNavCategory)button).Items[0], true);
					}
				}
				button.OwnerCollection.Owner.Buttons.Remove(button);
			}
			FireChanged(TileNavPane);
		}
		public virtual void RemoveCategory(TileNavCategory category) { RemoveCategory(category, true); }
		public virtual void RemoveCategory(TileNavCategory category, bool removeFromContainer) {
			if(TileNavPane.DebuggingState)
				return;
			while(category.Items.Count > 0) {
				RemoveItem(category.Items[0], true);
			}
			if(category.Container != null && category.Tile != null)
				category.Container.Remove(category.Tile);
			if(removeFromContainer && category.Owner != null)
				category.Owner.Container.Remove(category);
			if(category.Owner != null)
				category.Owner.Categories.Remove(category);
			FireChanged(TileNavPane);
		}
		public virtual void RemoveItem(TileNavItem item) { RemoveItem(item, true); }
		public virtual void RemoveItem(TileNavItem item, bool removeFromContainer) {
			if(TileNavPane.DebuggingState)
				return;
			while(item.SubItems.Count > 0)
				RemoveSubItem(item.SubItems[0]);
			if(item.Container != null && item.Tile != null)
				item.Container.Remove(item.Tile);
			TileNavCategory cat = item.OwnerCollection != null ? item.OwnerCollection.Category : null;
			if(removeFromContainer && cat != null && cat.Owner != null)
				cat.Owner.Container.Remove(item);
			if(item.OwnerCollection != null && item.OwnerCollection.Category != null)
				item.OwnerCollection.Category.Items.Remove(item);
			if(cat != null)
				FireChanged(cat);
			else
				FireChanged(TileNavPane);
		}
		public virtual void RemoveSubItem(TileNavSubItem subItem) { RemoveSubItem(subItem, true); }
		public virtual void RemoveSubItem(TileNavSubItem subItem, bool removeFromContainer) {
			if(TileNavPane.DebuggingState) return;
			TileNavItem item = subItem.GetItem();
			if(subItem.Container != null && subItem.Tile != null)
				subItem.Container.Remove(subItem.Tile);
			if(removeFromContainer && subItem.TileNavPane != null
				&& subItem.TileNavPane.Container != null) {
				subItem.TileNavPane.Container.Remove(subItem);
			}
			if(item != null) {
				item.SubItems.Remove(subItem);
				FireChanged(item);
			}
			else
				FireChanged(TileNavPane);
		}
		public virtual void OnAddCategoryClick() {
			if(TileNavPane.DebuggingState)
				return;
			TileNavCategory cat = new TileNavCategory();
			TileNavPane.Categories.Add(cat);
			if(TileNavPane.Container != null)
				TileNavPane.Container.Add(cat);
			if(cat.Container != null)
				cat.Container.Add(cat.Tile);
			cat.Name = NameService.CreateName(TileNavPane.Container, typeof(TileNavCategory));
			cat.Tile.Name = NameService.CreateName(cat.Container, typeof(TileBarItem));
			cat.Caption = cat.Name;
			cat.TileText = cat.Name;
			FireChanged(TileNavPane);
		}
		public virtual void OnAddButtonClick() {
			if(TileNavPane.DebuggingState)
				return;
			NavButton button = new NavButton();
			TileNavPane.Buttons.Add(button);
			if(TileNavPane.Container != null)
				TileNavPane.Container.Add(button);
			button.Name = NameService.CreateName(TileNavPane.Container, typeof(NavButton));
			button.Caption = button.Name;
			button.Alignment = NavButtonAlignment.Right;
			FireChanged(TileNavPane);
		}
		public virtual void OnAddCategoryButtonClick() {
			if(TileNavPane.DebuggingState)
				return;
			TileNavCategory cat = new TileNavCategory();
			TileNavPane.Buttons.Add(cat);
			if(TileNavPane.Container != null)
				TileNavPane.Container.Add(cat);
			cat.Name = NameService.CreateName(TileNavPane.Container, typeof(TileNavCategory));
			cat.Caption = cat.Name;
			cat.Alignment = NavButtonAlignment.Right;
			FireChanged(TileNavPane);
		}
		INameCreationService nameService;
		public INameCreationService NameService {
			get {
				if(nameService == null)
					nameService = TileNavPane.Site.GetService(typeof(INameCreationService)) as INameCreationService;
				return nameService;
			}
		}
		public virtual void OnAddItemToCategoryClick(TileNavCategory cat) {
			if(TileNavPane.DebuggingState || cat == null) return;
			TileNavItem item = new TileNavItem();
			item.Name = NameService.CreateName(TileNavPane.Container, typeof(TileNavItem));
			item.Caption = item.Name;
			item.TileText = item.Name;
			cat.Items.Add(item);
			if(cat.Container != null)
				cat.Container.Add(item);
			if(item.Container != null)
				item.Container.Add(item.Tile);
			FireChanged(cat);
		}
		public virtual void OnAddSubItemToItemClick(TileNavItem item) {
			if(TileNavPane.DebuggingState || item == null) return;
			TileNavSubItem subItem = new TileNavSubItem();
			subItem.Name = NameService.CreateName(TileNavPane.Container, typeof(TileNavSubItem));
			subItem.Caption = subItem.Name;
			subItem.TileText = subItem.Caption;
			item.SubItems.Add(subItem);
			if(item.Container != null)
				item.Container.Add(subItem);
			if(subItem.Container != null)
				subItem.Container.Add(subItem.Tile);
			FireChanged(item);
		}
		public virtual void EditItems(TileNavCategory cat) { }
		public NavElement GetElement() { 
			if(Component is NavElement)
				return (NavElement)Component;
			ICollection coll = SelectionService.GetSelectedComponents();
			foreach(IComponent comp in coll) {
				if(comp is NavElement)
					return (NavElement)comp;
			}
			return null;
		}
		void FireChanged(object component) {
			if(component == null) return;
			ComponentChangeService.OnComponentChanged(component, null, null, null);
		}
	}
#endregion DESIGNTIME
#region FOR_EVENTS
	public class TileNavPanePaintEventArgs : EventArgs {
		public TileNavPanePaintEventArgs(GraphicsCache cache, TileNavPaneViewInfo viewInfo) {
			Cache = cache;
			ViewInfo = viewInfo;
		}
		public GraphicsCache Cache { get; private set; }
		public TileNavPaneViewInfo ViewInfo { get; private set; }
	}
	public class NavElementEventArgs : EventArgs {
		public NavElement Element { get; set; }
		public bool IsTile { get; set; }
	}
	public class TileNavElementEventArgs : EventArgs {
		public TileNavElement Element { get; set; }
	}
	public class TileNavPaneSelectedElementEventArgs : CancelEventArgs {
		public TileNavElement Element { get; set; }
		public TileNavElement PreviousElement { get; internal set; }
	}
	public class DropDownEventArgs : EventArgs {
		public NavElement Element { get; set; }
	}
	public delegate void TileNavPaneDropDownEventHandler(object sender, DropDownEventArgs e);
	public delegate void NavElementClickEventHandler(object sender, NavElementEventArgs e);
	public delegate void TileNavPaneSelectedElementEventHandler(object sender, TileNavPaneSelectedElementEventArgs e);
	public delegate void TileNavPaneElementEventHandler(object sender, TileNavElementEventArgs e);
#endregion FOR_EVENTS
#region ENUMS
	public enum TileNavPaneMainButtonBehavior {
		ShowCategories,
		ShowDefaultCategoryItems,
		Default
	}
	public enum NavButtonAlignment { 
		Default,
		Left, 
		Right 
	}
	public enum ToolTipShowMode {
		AllElements,
		Tiles,
		NavBarItems,
		None
	}
	internal enum DropDownShowMode { FromTileBar, FromTileNavPane }
#endregion ENUMS
}
