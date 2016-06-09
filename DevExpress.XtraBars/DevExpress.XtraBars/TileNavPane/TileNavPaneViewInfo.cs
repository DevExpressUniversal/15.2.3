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

using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Text;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Navigation {
	public class TileNavPaneViewInfo : ObjectInfoArgs {
		public TileNavPaneViewInfo(TileNavPane tileNavBar) {
			this.tileNavPane = tileNavBar;
		}
		TileNavPane tileNavPane;
		public TileNavPane TileNavPane { get { return tileNavPane; } }
		public virtual bool IsReady { get; internal set; }
		public Rectangle ClientBounds { get; private set; }
		public TileNavButtonViewInfo MainButton { get; private set; }
		public TileNavButtonViewInfo CategoryButton { get; private set; }
		public TileNavButtonViewInfo ItemButton { get; private set; }
		public TileNavButtonViewInfo SubItemButton { get; private set; }
		internal Dictionary<TileNavButtonViewInfo, ITileBarWindowOwner> DropDownsToAdd = new Dictionary<TileNavButtonViewInfo, ITileBarWindowOwner>();
		internal List<TileNavButtonViewInfo> Buttons = new List<TileNavButtonViewInfo>();
		GraphicsInfo ginfo;
		public GraphicsInfo GInfo {
			get {
				if(ginfo == null)
					ginfo = new GraphicsInfo();
				return ginfo;
			}
		}
		AppearanceObject paintAppearance;
		public AppearanceObject PaintAppearance {
			get {
				if(paintAppearance == null)
					paintAppearance = GetPaintAppearance();
				return paintAppearance;
			}
		}
		AppearanceObject GetPaintAppearance() {
			AppearanceObject res = new AppearanceObject();
			AppearanceHelper.Combine(res, new AppearanceObject[] { TileNavPane.Appearance }, DefaultAppearance);
			return res;
		}
		public void ResetAppearance() {
			paintAppearance = null;
		}
		public void ResetDefaultAppearances() {
			defaultAppearance = null;
			defaultAppearanceHovered = null;
			defaultAppearanceSelected = null;
		}
		Color GetDisabledControlColor() {
			Color c = CommonSkins.GetSkin(TileNavPane.LookAndFeel.ActiveLookAndFeel).Colors["DisabledControl"];
			if(c.IsEmpty)
				c = CommonSkins.GetSkin(DefaultSkinProvider.Default).Colors["DisabledControl"];
			return c;
		}
		const string foreColor = "ForeColor";
		const string foreColorHot = "ForeColorHot";
		const string foreColorSel = "ForeColorSelected";
		const string backColor = "BackColor";
		const string backColorHot = "BackColorHot";
		const string backColorSel = "BackColorSelected";
		SkinElement GetTileNavPaneElement() {
			return NavPaneSkins.GetSkin(TileNavPane.LookAndFeel.ActiveLookAndFeel)["TileNavPane"];
		}
		Color GetNormalForeColor() {
			var elem = GetTileNavPaneElement();
			if(elem != null && elem.Properties.ContainsProperty(foreColor))
				return elem.Properties.GetColor(foreColor);
			return GetDefaultSkinElement(MetroUISkins.SkinActionsBarButton).Color.GetForeColor();
		}
		Color GetNormalBackColor() {
			var elem = GetTileNavPaneElement();
			if(elem != null && elem.Properties.ContainsProperty(backColor))
				return elem.Properties.GetColor(backColor);
			return GetDefaultSkinElement(MetroUISkins.SkinActionsBar).Color.GetBackColor();
		}
		Color GetHoveredForeColor() {
			var elem = GetTileNavPaneElement();
			if(elem != null && elem.Properties.ContainsProperty(foreColorHot))
				return elem.Properties.GetColor(foreColorHot);
			return GetDefaultSkinElement(MetroUISkins.SkinActionsBar).Color.GetBackColor();
		}
		Color GetHoveredBackColor() {
			var elem = GetTileNavPaneElement();
			if(elem != null && elem.Properties.ContainsProperty(backColorHot))
				return elem.Properties.GetColor(backColorHot);
			return GetDisabledControlColor();
		}
		Color GetSelectedForeColor() {
			var elem = GetTileNavPaneElement();
			if(elem != null && elem.Properties.ContainsProperty(foreColorSel))
				return elem.Properties.GetColor(foreColorSel);
			return GetDefaultSkinElement(MetroUISkins.SkinActionsBar).Color.GetBackColor();
		}
		Color GetSelectedBackColor() {
			var elem = GetTileNavPaneElement();
			if(elem != null && elem.Properties.ContainsProperty(backColorSel)) 
				return elem.Properties.GetColor(backColorSel);
			return GetDefaultSkinElement(MetroUISkins.SkinActionsBarButton).Color.GetForeColor();
		}
		SkinElement GetDefaultSkinElement(string elementName) {
			SkinElement elem = MetroUISkins.GetSkin(TileNavPane.LookAndFeel.ActiveLookAndFeel)[elementName];
			if(elem == null)
				elem = MetroUISkins.GetSkin(DefaultSkinProvider.Default)[elementName];
			return elem;
		}
		Font DefaultFont { get { return new Font(AppearanceObject.DefaultFont.FontFamily, AppearanceObject.DefaultFont.Size + 2f); } }
		AppearanceDefault defaultAppearance;
		protected internal virtual AppearanceDefault DefaultAppearance {
			get {
				if(defaultAppearance == null) {
					defaultAppearance = new AppearanceDefault(GetNormalForeColor(), GetNormalBackColor(), Color.Empty, Color.Empty);
					defaultAppearance.Font = DefaultFont;
				}
				return defaultAppearance;
			}
		}
		AppearanceDefault defaultAppearanceHovered;
		protected internal virtual AppearanceDefault DefaultAppearanceHovered {
			get {
				if(defaultAppearanceHovered == null) {
					defaultAppearanceHovered = new AppearanceDefault(GetHoveredForeColor(), GetHoveredBackColor(), Color.Empty, Color.Empty);
					defaultAppearanceHovered.Font = DefaultFont;
				}
				return defaultAppearanceHovered;
			}
		}
		AppearanceDefault defaultAppearanceSelected;
		protected internal virtual AppearanceDefault DefaultAppearanceSelected {
			get {
				if(defaultAppearanceSelected == null) {
					defaultAppearanceSelected = new AppearanceDefault(GetSelectedForeColor(), GetSelectedBackColor(), Color.Empty, Color.Empty);
					defaultAppearanceSelected.Font = DefaultFont;
				}
				return defaultAppearanceSelected;
			}
		}
		public virtual void Calc(Rectangle bounds) {
			GInfo.AddGraphics(null);
			try {
				CalcCore(bounds);
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected virtual void CalcCore(Rectangle bounds) {
			bool updateMainDropDown = MainButton == null;
			Bounds = bounds;
			ClientBounds = CalcClientBounds(Bounds);
			ClearButtons();
			CreateButtons();
			LayoutButtons();
			CreateDropDowns();
			IsReady = true;
			if(!TileNavPane.NeedTileBarAppearanceUpdate)
				TileNavPane.NeedTileBarAppearanceUpdate = updateMainDropDown;
		}
		void RecalcButtons() {
			GInfo.AddGraphics(null);
			try {
				LayoutButtons();
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected virtual void CreateDropDowns() {
			if(MainButton != null) MainButton.DropDown = GetMainButtonDropDown();
			foreach(var pair in DropDownsToAdd) {
				var dropDown = pair.Value.GetDropDown();
				if(dropDown != null) {
					pair.Key.DropDown = dropDown;
					pair.Key.ActAsButton = false;
				}
			}
		}
		protected virtual Rectangle CalcClientBounds(Rectangle rect) {
			return rect;
		}
		protected virtual void ClearButtons() {
			MainButton = null;
			CategoryButton = null;
			ItemButton = null;
			SubItemButton = null;
			Buttons.Clear();
			DropDownsToAdd.Clear();
			ClearViewInfoReferences();
		}
		void ClearViewInfoReferences() {
			foreach(ITileNavButton button in TileNavPane.Buttons) {
				if(button is NavElement)
					(button as NavElement).ViewInfo = null;
			}
			foreach(TileNavCategory cat in TileNavPane.Categories) {
				foreach(TileNavItem item in cat.Items) {
					foreach(TileNavSubItem sitem in item.SubItems)
						sitem.ViewInfo = null;
					item.ViewInfo = null;
				}
				cat.ViewInfo = null;
			}
			foreach(TileNavItem item in TileNavPane.DefaultCategory.Items) {
				foreach(TileNavSubItem sitem in item.SubItems)
					sitem.ViewInfo = null;
				item.ViewInfo = null;
			}
		}
		protected virtual void SetSelectedElement(NavElement selectedElement) {
			if(selectedElement is TileNavCategory)
				CreateCategoryButton(selectedElement);
			else if(selectedElement is TileNavItem)
				CreateItemButton(selectedElement);
			else if(selectedElement is TileNavSubItem)
				CreateSubItemButton(selectedElement);
		}
		protected virtual void CreateCategoryButton(NavElement selectedElement) {
			TileNavCategory category = selectedElement as TileNavCategory;
			if(!TileNavPane.Categories.Contains(category)) return;
			CategoryButton = CreatePathButton(category, category, TileNavPaneDropDownHelper.HasVisibleTiles(category.Items));
		}
		protected virtual void CreateItemButton(NavElement selectedElement) {
			TileNavItem item = selectedElement as TileNavItem;
			if(!item.IsInCollection) return;
			CreateCategoryButton(item.OwnerCollection.Category);
			ItemButton = CreatePathButton(item, item, TileNavPaneDropDownHelper.HasVisibleTiles(item.SubItems));
		}
		protected virtual void CreateSubItemButton(NavElement selectedElement) {
			TileNavSubItem subItem = selectedElement as TileNavSubItem;
			if(!subItem.IsInCollection) return;
			CreateItemButton(subItem.OwnerCollection.Item);
			SubItemButton = CreatePathButton(subItem, null, false);
		}
		protected virtual TileNavButtonViewInfo CreatePathButton(NavElement item, ITileBarWindowOwner dropDownOwner, bool allowDropDown) {
			CreateArrowButton();
			TileNavButtonViewInfo button = new TileNavButtonViewInfo(this, item);
			button.Text = item.Caption;
			button.Glyph = item.Glyph;
			button.AutoGenerated = true;
			if(!allowDropDown) {
				button.DropDown = null;
				button.DropDownVisible = false;
			}
			button.CalcViewInfo(GInfo.Graphics);
			Buttons.Add(button);
			if(allowDropDown) DropDownsToAdd.Add(button, dropDownOwner);
			return button;
		}
		protected virtual void CreateButton(ITileNavButton button) {
			if(button is NavButton) (button as NavButton).IsMain = false;
			TileNavButtonViewInfo buttonInfo = new TileNavButtonViewInfo(this, button.Element);
			buttonInfo.Text = button.Caption;
			buttonInfo.Glyph = button.Glyph;
			buttonInfo.DropDownVisible = false;
			buttonInfo.CalcViewInfo(GInfo.Graphics);
			Buttons.Add(buttonInfo);
			if(button is ITileBarWindowOwner)
				DropDownsToAdd.Add(buttonInfo, (button as ITileBarWindowOwner));
		}
		protected virtual void CreateMainButton(NavElement element) {
			MainButton = new TileNavButtonViewInfo(this, element);
			MainButton.ActAsButton = false;
			MainButton.IsMain = true;
			MainButton.Glyph = element.Glyph;
			MainButton.Text = element.Caption;
			MainButton.CalcViewInfo(GInfo.Graphics);
			Buttons.Add(MainButton);
		}
		protected virtual void CreateArrowButton() {
			TileNavButtonViewInfo buttonInfo = new TileNavButtonViewInfo(this, null);
			buttonInfo.IsInteractive = false;
			buttonInfo.Glyph = ArrowGlyph;
			buttonInfo.Padding = new Padding(8, 8, 8, 8);
			buttonInfo.GlyphOpacity = 0.35f;
			buttonInfo.DropDownVisible = false;
			buttonInfo.AutoGenerated = true;
			buttonInfo.CalcViewInfo(GInfo.Graphics);
			Buttons.Add(buttonInfo);
		}
		Image arrowGlyph;
		Image ArrowGlyph {
			get { 
				if(arrowGlyph == null)
					arrowGlyph = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraBars.TileBar.Images.Arrow.png", typeof(TileBar).Assembly);
				return arrowGlyph;
			}
		}
		protected virtual TileBarWindow GetMainButtonDropDown() {
			if(TileNavPane.MainButtonBehavior == TileNavPaneMainButtonBehavior.Default)
				return TileNavPane.Categories.Count > 0 ? TileNavPane.CategoriesDropDown : TileNavPane.DefaultCategory.DropDown;
			return TileNavPane.MainButtonBehavior == TileNavPaneMainButtonBehavior.ShowCategories ? TileNavPane.CategoriesDropDown : TileNavPane.DefaultCategory.DropDown;
		}
		protected virtual void CreatePathButtons(NavElement mainButton) {
			CreateMainButton(mainButton);
			if(TileNavPane.SelectedElement != null)
				SetSelectedElement(TileNavPane.SelectedElement);
		}
		protected virtual void CreateButtons() {
			if(TileNavPane.Buttons.Count == 0) return;
			bool hasMainButton = false;
			foreach(ITileNavButton button in TileNavPane.Buttons) {
				if(!TileNavPane.IsDesignMode && !button.Visible) continue;
				if(button is NavButton && ((NavButton)button).IsMain && !hasMainButton) {
					CreatePathButtons(button as NavButton);
					hasMainButton = true;
				}
				else
					CreateButton(button);
			}
		}
		protected virtual void LayoutButtons() {
			ResetIsReady();
			int x = ClientBounds.Width;
			int goRightIndex = -1;
			TileNavButtonViewInfo firstRight = Buttons.FirstOrDefault(i => CheckRightAlignment(i));
			goRightIndex = firstRight == null ? -1 : Buttons.IndexOf(firstRight);
			if(goRightIndex != -1) {
				x = ClientBounds.Width;
				for(int i = Buttons.Count - 1; i >= goRightIndex; i--) {
					var bInfo = Buttons[i];
					bInfo.CalcViewInfo(GInfo.Graphics);
					x -= bInfo.Size.Width;
					bInfo.UpdateLocation(GInfo.Graphics, new Point(x, bInfo.Location.Y));
				}
			}
			int rightX = x;
			x = ClientBounds.Left;
			foreach(TileNavButtonViewInfo buttonInfo in Buttons) {
				if(CheckRightAlignment(buttonInfo)) return;
				else {
					buttonInfo.UpdateLocation(GInfo.Graphics, new Point(x, 0));
					x += buttonInfo.Size.Width;
				}
				UpdateButtonVisibility(buttonInfo, rightX);
			}
		}
		void ResetIsReady() {
			foreach(TileNavButtonViewInfo bInfo in Buttons) {
				bInfo.IsReady = false;
			}
		}
		void UpdateButtonVisibility(TileNavButtonViewInfo buttonInfo, int maxRight) {
			if(buttonInfo.Bounds.Right <= maxRight)
				return;
			Rectangle buttonRect = new Rectangle(buttonInfo.Location, new Size(maxRight - buttonInfo.Bounds.X, buttonInfo.Bounds.Height));
			if(buttonInfo.Bounds.X > maxRight || buttonInfo.ShouldHide(buttonRect)) {
				HideButton(buttonInfo);
				return;
			}
			buttonInfo.CalcViewInfo(GInfo.Graphics, buttonRect);
		}
		protected virtual void HideButton(TileNavButtonViewInfo buttonInfo) {
			buttonInfo.IsVisible = false;
			int prevIndex = Buttons.IndexOf(buttonInfo) - 1;
			if(prevIndex < 0) return;
			TileNavButtonViewInfo prevItem = Buttons[prevIndex];
			if(buttonInfo.AutoGenerated && prevItem.AutoGenerated && !prevItem.IsInteractive)
				prevItem.IsVisible = false;
		}
		bool CheckRightAlignment(TileNavButtonViewInfo buttonInfo) {
			return !buttonInfo.AutoGenerated && buttonInfo.Element != null && buttonInfo.Element.Alignment == NavButtonAlignment.Right;
		}
		public virtual TileNavPaneHitInfo CalcHitInfo(Point point) {
			return CalcHitInfoCore(point);
		}
		public virtual TileNavPaneHitInfo CalcHitInfoCore(Point point) {
			TileNavPaneHitInfo hitInfo = CreateHitInfo();
			hitInfo.HitPoint = point;
			CalcHitInfo(hitInfo);
			return hitInfo;
		}
		protected virtual TileNavPaneHitInfo CreateHitInfo() {
			return new TileNavPaneHitInfo();
		}
		protected virtual void CalcHitInfo(TileNavPaneHitInfo hitInfo) {
			if(!hitInfo.ContainsSet(Bounds, TileNavPaneHitTest.Control)) {
				hitInfo.HitTest = TileNavPaneHitTest.Outside;
				return;
			}
			hitInfo.ViewInfo = this;
			for(int i = Buttons.Count - 1; i > -1; i--) {
				if(Buttons[i].CheckHitInfo(hitInfo))
					return;
			}
		}
		TileNavPaneHitInfo hoverInfo = TileNavPaneHitInfo.Empty;
		public TileNavPaneHitInfo HoverInfo {
			get { return hoverInfo; }
			set {
				if(value == null)
					value = TileNavPaneHitInfo.Empty;
				if(HoverInfo.Equals(value))
					return;
				TileNavPaneHitInfo oldInfo = HoverInfo;
				hoverInfo = value;
				OnHoverInfoChanged(hoverInfo, oldInfo);
			}
		}
		TileNavPaneHitInfo pressedInfo = TileNavPaneHitInfo.Empty;
		public TileNavPaneHitInfo PressedInfo {
			get { return pressedInfo; }
			set {
				if(value == null)
					value = TileNavPaneHitInfo.Empty;
				if(PressedInfo.Equals(value))
					return;
				TileNavPaneHitInfo oldInfo = PressedInfo;
				pressedInfo = value;
				OnPressedInfoChanged(pressedInfo, oldInfo);
			}
		}
		void OnPressedInfoChanged(TileNavPaneHitInfo newInfo, TileNavPaneHitInfo oldInfo) {
			if(oldInfo != null && oldInfo.ButtonInfo != null)
				UpdateButtonAppearance(oldInfo.ButtonInfo);
			if(newInfo != null && newInfo.ButtonInfo != null)
				UpdateButtonAppearance(newInfo.ButtonInfo);
		}
		void OnHoverInfoChanged(TileNavPaneHitInfo newInfo, TileNavPaneHitInfo oldInfo) {
			if(oldInfo != null && oldInfo.ButtonInfo != null) 
				UpdateButtonAppearance(oldInfo.ButtonInfo);
			if(newInfo != null && newInfo.ButtonInfo != null) 
				UpdateButtonAppearance(newInfo.ButtonInfo);
		}
		void UpdateButtonAppearance(TileNavButtonViewInfo button) {
			if(button == null) return;
			var norm = button.GetAppearanceNormal();
			var hover = button.GetAppearanceHovered();
			var select = button.GetAppearanceSelected();
			if(norm.Font.Equals(hover.Font) && norm.Font.Equals(select.Font))
				UpdateButtonAppearanceFast(button);
			else
				UpdateButtonAppearanceHeavy(button);
		}
		void UpdateButtonAppearanceFast(TileNavButtonViewInfo button) {
			button.ResetAppearance();
			TileNavPane.Invalidate(button.Bounds);
		}
		void UpdateButtonAppearanceHeavy(TileNavButtonViewInfo button) {
			button.ResetAppearance();
			RecalcButtons();
			TileNavPane.Invalidate();
		}
		protected internal void HideActiveDropDown() {
			if(ActiveDropDown != null) {
				ActiveDropDown.HideDropDown();
				ActiveDropDown = null;
			}
		}
		protected internal TileBarWindow ActiveDropDown { get; set; }
		internal void ProceedButtonClick(TileNavButtonViewInfo buttonInfo) {
			if(TileNavPane.IsDesignMode) {
				ResetPressedInfo();
				return;
			}
			bool onlyClose = ActiveDropDown != null && ActiveDropDown == buttonInfo.DropDown && ActiveDropDown.Visible ? true : false;
			HideActiveDropDown();
			if(onlyClose) {
				ResetPressedInfo();
				return;
			}
			if(buttonInfo.ShowDropDown()) {
				ActiveDropDown = buttonInfo.DropDown;
				SelectedElement = buttonInfo.Element;
			}
			else {
				ResetPressedInfo();
			}
		}
		NavElement selectedElementCore;
		public NavElement SelectedElement {
			get { return selectedElementCore; }
			set {
				if(SelectedElement != null && SelectedElement.Equals(value))
					return;
				NavElement prevElement = SelectedElement;
				selectedElementCore = value;
				OnSelectedElementInfoChanged(selectedElementCore, prevElement);
			}
		}
		void OnSelectedElementInfoChanged(NavElement newElement, NavElement prevElement) {
			if(prevElement != null && prevElement.ViewInfo != null) 
				UpdateButtonAppearance(prevElement.ViewInfo);
			if(newElement != null && newElement.ViewInfo != null) 
				UpdateButtonAppearance(newElement.ViewInfo);
			TileNavPane.OnPropertiesChanged();
		}
		internal void OnDropDownClosed() {
			var prevSelectedElement = SelectedElement;
			SelectedElement = null;
			ActiveDropDown = null;
			TileNavPane.OnDropDownHidden(prevSelectedElement);
		}
		protected internal void ResetPressedInfo() {
			PressedInfo = TileNavPaneHitInfo.Empty;
		}
		internal TileNavButtonViewInfo GetButtonInfoByElement(TileNavElement element) {
			foreach(TileNavButtonViewInfo button in Buttons) {
				if(button.Element == element)
					return button;
			}
			if(TileNavPane.DefaultCategory == element)
				return MainButton;
			return null;
		}
		protected internal virtual AppearanceObject GetAppearance(TileNavButtonViewInfo buttonInfo) {
			if(buttonInfo.IsSelected) {
				AppearanceObject res = new AppearanceObject();
				AppearanceHelper.Combine(res, new AppearanceObject[] { TileNavPane.AppearanceSelected }, DefaultAppearanceSelected);
				return res;
			}
			if(buttonInfo.IsHovered)
				return TileNavPane.AppearanceHovered;
			return TileNavPane.Appearance;
		}
		TileNavPaneDesignTimeManagerBase designTimeManager;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public TileNavPaneDesignTimeManagerBase DesignTimeManager {
			get {
				if(designTimeManager == null)
					designTimeManager = new TileNavPaneDesignTimeManagerBase(TileNavPane.Site.Component, TileNavPane);
				return designTimeManager;
			}
			set { designTimeManager = value; }
		}
		public void CreateMainButton() {
			NavButton mButton = new NavButton();
			TileNavPane.Buttons.Add(mButton);
			mButton.Caption = "Main Menu";
			mButton.IsMain = true;
			if(TileNavPane.Container != null) {
				TileNavPane.Container.Add(mButton);
				mButton.Name = DesignTimeManager.NameService.CreateName(TileNavPane.Container, typeof(NavButton));
			}
		}
		TileNavPaneDropInfo dropInfo;
		public TileNavPaneDropInfo DropInfo {
			get { return dropInfo; }
			set {
				if(dropInfo == value)
					return;
				OnDropInfoChanged(dropInfo, value);
				dropInfo = value;
			}
		}
		private void OnDropInfoChanged(TileNavPaneDropInfo oldDropInfo, TileNavPaneDropInfo newDropInfo) {
			if(oldDropInfo != null && oldDropInfo.ButtonInfo != null) {
				oldDropInfo.ButtonInfo.DrawDropSplitter = false;
				TileNavPane.Invalidate(oldDropInfo.ButtonInfo.Bounds);
			}
			if(newDropInfo != null && newDropInfo.ButtonInfo != null) {
				newDropInfo.ButtonInfo.DrawDropSplitter = true;
				newDropInfo.ButtonInfo.DropSplitterIsLeft = !newDropInfo.IsLast;
				TileNavPane.Invalidate(newDropInfo.ButtonInfo.Bounds);
			}
		}
		internal void UpdateDropInfo(Point pt) {
			foreach(TileNavButtonViewInfo bInfo in Buttons) {
				if(bInfo.Bounds.Contains(new Point(pt.X, bInfo.Bounds.Top))) {
					DropInfo = CalcDropInfo(bInfo, pt);
					return;
				}
			}
			DropInfo = null;
		}
		private TileNavPaneDropInfo CalcDropInfo(TileNavButtonViewInfo bInfo, Point pt) {
			TileNavPaneDropInfo res = new TileNavPaneDropInfo();
			res.ButtonInfo = bInfo;
			if(bInfo != null && Buttons[Buttons.Count - 1] == bInfo)
				res.IsLast = pt.X > (bInfo.Bounds.Left + (bInfo.Bounds.Width / 2));
			return res;
		}
		internal void OnButtonDrop() {
			if(DropInfo == null || DragButtonInfo == null) return;
			int insertIndex = TileNavPane.Buttons.IndexOf(DropInfo.ButtonInfo.Element as ITileNavButton);
			int dragButtonIndex = TileNavPane.Buttons.IndexOf(DragButtonInfo.Element as ITileNavButton);
			insertIndex = insertIndex > dragButtonIndex ? insertIndex - 1 : insertIndex;
			insertIndex = Math.Max(Math.Min(insertIndex, TileNavPane.Buttons.Count - 1), 0);
			if(DropInfo.IsLast)
				TileNavPane.Buttons.Add(DragButtonInfo.Element as ITileNavButton);
			else
				TileNavPane.Buttons.Insert(insertIndex, DragButtonInfo.Element as ITileNavButton);
			if(TileNavPane.IsDesignMode)
				DesignTimeManager.ComponentChangeService.OnComponentChanged(TileNavPane, null, null, null);
			DragButtonInfo = null;
		}
		public TileNavButtonViewInfo DragButtonInfo { get; set; }
		internal void ShowDropDown(TileNavButtonViewInfo buttonInfo) {
		}
		NavElement focusedElementCore;
		public NavElement FocusedElement {
			get { return focusedElementCore; }
			set {
				if(FocusedElement != null && FocusedElement.Equals(value))
					return;
				NavElement prevElement = FocusedElement;
				focusedElementCore = value;
				OnFocusedElementChanged(focusedElementCore, prevElement);
			}
		}
		private void OnFocusedElementChanged(NavElement focusedElement, NavElement prevElement) {
			if(prevElement != null && prevElement.ViewInfo != null)
				UpdateButtonAppearance(prevElement.ViewInfo);
			if(focusedElement != null && focusedElement.ViewInfo != null)
				UpdateButtonAppearance(focusedElement.ViewInfo);
		}
		protected internal virtual void ShowLastDropDownInPath() {
			var element = GetLastDropDownElement();
			if(element == null) return;
			element.OnElementClick(false);
		}
		TileNavButtonViewInfo GetLastDropDownElement() {
			if(ItemButton != null) return ItemButton;
			if(CategoryButton != null) return CategoryButton;
			if(MainButton != null) return MainButton;
			return null;
		}
		protected internal ITileBarWindowOwner ActiveDropDownOwner {
			get {
				if(ActiveDropDown != null)
					return ActiveDropDown.DropDownOwner;
				return null;
			}
		}
	}
	public class TileNavButtonViewInfo : ObjectInfoArgs {
		internal static XPaint GdiPaint = new XPaintMixed();
		GraphicsInfo gInfo;
		TileNavPaneViewInfo controlInfo;
		public TileNavButtonViewInfo(TileNavPaneViewInfo controlInfo, NavElement element) {
			this.controlInfo = controlInfo;
			this.Element = element;
			this.DropDownVisible = true;
			this.IsInteractive = true;
			this.DrawDropSplitter = false;
			this.IsVisible = true;
			this.IsReady = false;
			this.ActAsButton = true;
			this.GlyphOpacity = 1.0f;
			gInfo = new GraphicsInfo();
			if(element != null)
				element.ViewInfo = this;
		}
		public virtual TileNavPaneViewInfo ControlInfo { get { return controlInfo; } }
		public TileBarWindow DropDown { get; internal set; }
		public GraphicsInfo GInfo { get { return gInfo; } }
		public virtual Size Size { get; internal set; }
		public Point Location { get; internal set; }
		public StringInfo StringInfo { get; set; }
		public virtual string Text { get; set; }
		public NavElement Element { get; set; }
		public Image Glyph { get; set; }
		bool IsDropDownSplitted { get; set; }
		bool ShowText { get { return true; } }
		public bool AutoGenerated { get; set; }
		public bool IsVisible { get; set; }
		public bool IsReady { get; set; }
		public bool DropDownVisible { get; set; }
		internal bool IsInteractive { get; set; }
		protected internal bool ActAsButton { get; set; }
		protected internal bool IsMain { get; set; }
		public float GlyphOpacity { get; set; }
		public Rectangle ContentBounds { get; set; }
		public Rectangle GlyphBounds { get; set; }
		public Rectangle TextBounds { get; set; }
		public Rectangle DropDownGlyphBounds { get; set; }
		public override Rectangle Bounds {
			get { return new Rectangle(Location, Size); }
			set { }
		}
		Padding padding;
		public Padding Padding {
			get {
				if(Element == null) return padding;
				if(Element.Padding != NavButton.DefaultPadding) return Element.Padding;
				return ControlInfo.TileNavPane.ButtonPadding;
			}
			internal set { padding = value; }
		}
		protected virtual int GlyphToTextIndent { get { return 5; } }
		public bool ShouldDrawGlyph { get { return Glyph != null && GlyphBounds != Rectangle.Empty; } }
		public bool Enabled {
			get {
				if(ControlInfo.TileNavPane.IsDesignMode)
					return true;
				else
					return Element == null ? false : Element.Enabled;
			}
		}
		AppearanceObject paintAppearance;
		public AppearanceObject PaintAppearance {
			get {
				if(paintAppearance == null)
					paintAppearance = GetPaintAppearance();
				return paintAppearance;
			}
		}
		public void ResetAppearance() {
			paintAppearance = null;
			ForceUpdateAppearanceColorsCore();
		}
		int DropDownWidth { get { return 20; } }
		Image dropDownGlyph;
		public Image DropDownGlyph {
			get {
				if(dropDownGlyph == null)
					dropDownGlyph = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraBars.TileBar.Images.DropDownButton.png", typeof(TileBar).Assembly);
				return RotateByDropDownDirection(dropDownGlyph);
			}
		}
		Image RotateByDropDownDirection(Image input) {
			if(ControlInfo.TileNavPane.DropDownShowDown)
				return input;
			else {
				Image rotated = input.Clone() as Image;
				rotated.RotateFlip(RotateFlipType.Rotate180FlipNone);
				input = null;
				return rotated;
			}
		}
		public virtual bool IsSelected {
			get {
				if(Element == null) return false;
				return ControlInfo.PressedInfo.ButtonInfo == this || ControlInfo.SelectedElement == Element;
			}
		}
		public virtual bool IsHovered { get { return ControlInfo.HoverInfo.ButtonInfo == this; } }
		public virtual bool ShouldDrawFocusRect { get { return Element != null && ControlInfo.FocusedElement == Element; } }
		Size GetGlyphSize() {
			if(Glyph == null) return Size.Empty;
			return Glyph.Size;
		}
		int GetGlyphtoTextIndent() {
			if(string.IsNullOrEmpty(Text) || Glyph == null) return 0;
			return GlyphToTextIndent;
		}
		Size GetDropDownGlyphSize() {
			if(!DropDownVisible) return Size.Empty;
			return new Size(DropDownWidth, ControlInfo.ClientBounds.Height);
		}
		public void UpdateLocation(Graphics g, Point loc) {
			Location = loc;
			CalcViewInfo(g);
		}
		public void CalcViewInfo(Graphics g) {
			Size = CalcButtonSize(g);
			CalcViewInfo(g, Bounds);
		}
		public void CalcViewInfo(Graphics g, Rectangle bounds) {
			Size = bounds.Size;
			ContentBounds = CalcContentBounds(bounds);
			CalcTextBounds(ContentBounds);
			CalcTextAndGlyphBounds();
			CalcDropDownGlyphBounds();
			ConstraintTextBounds();
			if(StringInfo != null)
				StringInfo.SetLocation(TextBounds.Location);
			IsReady = true;
		}
		void ConstraintTextBounds() {
			if(ContentBounds.Right - TextBounds.X < 1) {
				TextBounds = Rectangle.Empty;
				return;
			}
			if(TextBounds.Left >= ContentBounds.Left && TextBounds.Right <= ContentBounds.Right)
				return;
			int contW = ContentBounds.Width;
			int contH = ContentBounds.Height;
			var contPt = ContentBounds.Location;
			if(ContentBounds.Height < 0) {
				contH = Size.Height;
				contPt = new Point(ContentBounds.Location.X, 0);
			}
			var contRect = new Rectangle(contPt, new Size(contW, contH));
			var bounds = Rectangle.Intersect(contRect, TextBounds);
			CalcTextBounds(bounds);
		}
		protected virtual Size CalcButtonSize(Graphics g) {
			Size res = Size.Empty;
			res.Height = ControlInfo.ClientBounds.Height;
			res.Width += CalcTextSize(g).Width;
			res.Width += GetGlyphSize().Width;
			res.Width += GetGlyphtoTextIndent();
			res.Width += Padding.Horizontal;
			if(DropDownVisible)
				res.Width += (DropDownWidth - Padding.Right);
			return res;
		}
		protected internal Rectangle CalcContentBounds(Rectangle bounds) {
			Rectangle res = bounds;
			res.X += Padding.Left;
			res.Y += Padding.Top;
			res.Width -= Padding.Horizontal;
			res.Height -= Padding.Vertical;
			if(DropDownVisible)
				res.Width += (Padding.Right - DropDownWidth);
			return res;
		}
		protected internal void CalcTextBounds(Rectangle bounds) {
			if(string.IsNullOrEmpty(Text) || !ShowText) 
				return;
			if(bounds.Width <= 0) {
				StringInfo = null;
				TextBounds = Rectangle.Empty;
				return;
			}
			TextBounds = new Rectangle(Point.Empty, bounds.Size);
			StringInfo = CalcHtmlText(TextBounds.Size);
			StringInfo = CalcHtmlText(StringInfo.Bounds.Size);
			TextBounds = TileItemCalculator.LayoutContent(bounds, StringInfo.Bounds.Size, TileItemContentAlignment.MiddleCenter, Point.Empty);
		}
		protected internal void CalcTextAndGlyphBounds() {
			if(Glyph == null) return;
			if(TextBounds.Size != Size.Empty)
				ArrangeTextAndGlyph();
			else
				ArrangeGlyphOnly();
		}
		protected internal virtual void ArrangeGlyphOnly() {
			Rectangle panelRect = ContentBounds;
			int x = (panelRect.Width / 2) - (GetGlyphSize().Width / 2);
			int y = (panelRect.Height / 2) - (GetGlyphSize().Height / 2);
			GlyphBounds = new Rectangle(new Point(ContentBounds.X + x, ContentBounds.Y + y), GetGlyphSize());
		}
		protected internal virtual void ArrangeTextAndGlyph() {
			Items2Panel ItemsPanel = new Items2Panel();
			ItemsPanel.HorizontalIndent = GlyphToTextIndent;
			ItemsPanel.VerticalIndent = 0;
			ItemsPanel.Content1HorizontalAlignment = ItemHorizontalAlignment.Center;
			ItemsPanel.Content1VerticalAlignment = ItemVerticalAlignment.Center;
			ItemsPanel.Content2HorizontalAlignment = ItemHorizontalAlignment.Center;
			ItemsPanel.Content2VerticalAlignment = ItemVerticalAlignment.Center;
			ItemsPanel.Content1Location = GetImageLocation();
			Rectangle panelRect = ContentBounds;
			Rectangle content1Bounds = Rectangle.Empty, content2Bounds = Rectangle.Empty;
			ItemsPanel.ArrangeItems(panelRect, GetGlyphSize(), TextBounds.Size, ref content1Bounds, ref content2Bounds);
			if(ContentBounds.Right < content1Bounds.Right) {
				int xoff = ContentBounds.Right - content1Bounds.Right;
				content1Bounds.Offset(xoff, 0);
				content2Bounds.Offset(xoff, 0);
			}
			GlyphBounds = content1Bounds;
			TextBounds = content2Bounds;
			StringInfo = CalcHtmlText(TextBounds.Size);
		}
		protected virtual void CalcDropDownGlyphBounds() {
			if(!DropDownVisible) return;
			Rectangle gPanel = new Rectangle(new Point(Bounds.Right - DropDownWidth, ContentBounds.Top), new Size(DropDownWidth, ContentBounds.Height));
			int x = gPanel.Right - (int)(gPanel.Width / 2);
			x -= DropDownGlyph.Size.Width / 2;
			int y = gPanel.Bottom - (int)(gPanel.Height / 2);
			y -= DropDownGlyph.Size.Height / 2;
			DropDownGlyphBounds = new Rectangle(new Point(x, y), DropDownGlyph.Size);
		}
		Size CalcTextSize(Graphics g) {
			if(string.IsNullOrEmpty(Text)) return Size.Empty;
			string s = Text;
			if(s == null || s.Length == 0) s = "Wg";
			Size res = CalcTextSize(g, s);
			return res;
		}
		Size CalcTextSize(Graphics g, string text) {
			Size size = Size.Empty;
			g = GInfo.AddGraphics(g);
			try {
				size = StringPainter.Default.Calculate(g, PaintAppearance, Text, 0).Bounds.Size;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return size;
		}
		bool HasGlyph { get { return Glyph != null; } }
		bool HasText { get { return String.IsNullOrEmpty(Text); } }
		protected virtual internal bool ShouldHide(Rectangle buttonRect) {
			int contentWidth = CalcContentBounds(buttonRect).Width;
			if(HasGlyph)
				if(Glyph.Size.Width > contentWidth) return true;
			return contentWidth < 1;
		}
		Font Font { get { return PaintAppearance.GetFont(); } }
		private ItemLocation GetImageLocation() {
			if(Element == null) return ItemLocation.Left;
			switch(Element.GlyphAlignment) {
				case NavButtonAlignment.Right: return ItemLocation.Right;
				default: return ItemLocation.Left;
			}
		}
		protected virtual StringInfo CalcHtmlText(Size maxTextSize) {
			return CalcHtmlText(new Rectangle(0, 0, maxTextSize.Width, maxTextSize.Height));
		}
		protected virtual StringInfo CalcHtmlText() {
			return CalcHtmlText(new Rectangle(0, 0, ContentBounds.Width, 0));
		}
		protected virtual StringInfo CalcHtmlTextConsiderImageBounds() {
			return CalcHtmlText(new Rectangle(Point.Empty, new Size(ContentBounds.Right - TextBounds.X, ContentBounds.Height)));
		}
		protected virtual StringInfo CalcHtmlText(Rectangle bounds) {
			return StringPainter.Default.Calculate(ControlInfo.GInfo.Graphics, PaintAppearance, Text, bounds, TileNavButtonViewInfo.GdiPaint);
		}
		public virtual bool CheckHitInfo(TileNavPaneHitInfo hitInfo) {
			if(this.IsVisible && this.Enabled && this.IsInteractive && this.Bounds.Contains(hitInfo.HitPoint)) {
				hitInfo.HitTest = TileNavPaneHitTest.Button;
				hitInfo.ButtonInfo = this;
				return true;
			}
			return false;
		}
		public virtual bool ShowDropDown() {
			if(DropDown == null) return false;
			if(Element != null && Element is TileNavItem)
				(Element as TileNavItem).DropDownMode = DropDownShowMode.FromTileNavPane;
			if(!DropDown.TryToShowToolWindow()) 
				return false;
			ControlInfo.TileNavPane.OnDropDownShown(Element);
			return true;
		}
		protected internal virtual AppearanceObject GetPaintAppearance() {
			AppearanceObject res = new AppearanceObject();
			AppearanceHelper.Combine(res, new AppearanceObject[] { GetAppearance(), ControlInfo.GetAppearance(this) }, ControlInfo.DefaultAppearance);
			return res;
		}
		protected internal AppearanceObject GetAppearance() {
			if(IsSelected)
				return GetAppearanceSelected();
			if(IsHovered)
				return GetAppearanceHovered();
			return GetAppearanceNormal();
		}
		protected internal AppearanceObject GetAppearanceSelected() {
			if(Element == null) return ControlInfo.TileNavPane.AppearanceSelected;
			AppearanceObject selected = new AppearanceObject();
			AppearanceHelper.Combine(selected, new AppearanceObject[] { 
					Element.AppearanceSelected, ControlInfo.TileNavPane.AppearanceSelected }, ControlInfo.DefaultAppearanceSelected);
			return selected;
		}
		protected internal AppearanceObject GetAppearanceHovered() {
			if(Element == null) return ControlInfo.TileNavPane.AppearanceHovered;
			AppearanceObject hovered = new AppearanceObject();
			AppearanceHelper.Combine(hovered, new AppearanceObject[] {
					Element.AppearanceHovered, ControlInfo.TileNavPane.AppearanceHovered }, ControlInfo.DefaultAppearanceHovered);
			return hovered;
		}
		protected internal AppearanceObject GetAppearanceNormal() {
			if(Element == null) return ControlInfo.TileNavPane.Appearance;
			AppearanceObject normal = new AppearanceObject();
			AppearanceHelper.Combine(normal, new AppearanceObject[] { Element.Appearance, ControlInfo.TileNavPane.Appearance }, ControlInfo.DefaultAppearance);
			return normal;
		}
		protected internal void ForceUpdateAppearanceColorsCore() {
			if(StringInfo != null)
				StringInfo.UpdateAppearanceColors(PaintAppearance);
		}
		internal void OnElementClick() {
			OnElementClick(true);
		}
		internal void OnElementClick(bool canRiseEvent) {
			if(ControlInfo == null)
				return;
			ControlInfo.ProceedButtonClick(this);
			if(IsMain || !canRiseEvent) return;
			Element.OnElementClick(false);
			if(ControlInfo.TileNavPane != null)
				ControlInfo.TileNavPane.OnElementClick(Element);
		}
		public bool ShouldDrawBackground {
			get { return IsSelected || IsHovered || GetShouldDrawBackground(); }
		}
		bool GetShouldDrawBackground() {
			var app = GetAppearanceNormal();
			AppearanceObject normal = new AppearanceObject();
			AppearanceHelper.Combine(normal, new AppearanceObject[] { ControlInfo.TileNavPane.Appearance }, ControlInfo.DefaultAppearance);
			if(app.BackColor.Equals(normal.BackColor) && 
				app.BackColor2.Equals(normal.BackColor2) &&
				!normal.BackColor.Equals(normal.BackColor2) &&
				normal.GradientMode != System.Drawing.Drawing2D.LinearGradientMode.Vertical)
				return false;
			return true;
		}
		public bool AllowGlyphSkinning { get { return GetAllowGlyphSkinning(); } }
		protected virtual bool GetAllowGlyphSkinning() {
			if(Element == null) return true;
			return Element.AllowGlyphSkinning != DefaultBoolean.Default ? Element.AllowGlyphSkinning.ToBoolean(false) : ControlInfo.TileNavPane.AllowGlyphSkinning;
		}
		public bool IsToggle { get { return !ActAsButton; } }
		public bool DrawDropSplitter { get; set; }
		public bool DropSplitterIsLeft { get; set; }
		TextOptions textOptions;
		public TextOptions TextOptions {
			get {
				if(textOptions == null) {
					textOptions = TextOptions.DefaultOptionsCenteredWithEllipsis;
					textOptions.HAlignment = HorzAlignment.Near;
				}
				return textOptions;
			}
		}
	}
	public class TileNavPaneDropInfo {
		public TileNavButtonViewInfo ButtonInfo { get; set; }
		public bool IsLast { get; set; }
	}
	public class TileNavPaneHitInfo {
		static TileNavPaneHitInfo empty = new TileNavPaneHitInfo();
		public static TileNavPaneHitInfo Empty { get { return empty; } }
		public Point HitPoint { get; internal set; }
		public TileNavPaneHitTest HitTest { get; internal set; }
		public TileNavPaneViewInfo ViewInfo { get; internal set; }
		public TileNavButtonViewInfo ButtonInfo { get; internal set; }
		public bool InButton { get { return HitTest == TileNavPaneHitTest.Button && ButtonInfo != null; } }
		public bool ContainsSet(Rectangle rect, TileNavPaneHitTest hitTest) {
			if(rect.Contains(HitPoint)) {
				HitTest = hitTest;
				return true;
			}
			return false;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			TileNavPaneHitInfo hitInfo = obj as TileNavPaneHitInfo;
			if(hitInfo == null || hitInfo.HitTest != HitTest)
				return false;
			if(hitInfo.HitTest == TileNavPaneHitTest.Control)
				return hitInfo.ViewInfo == ViewInfo;
			return hitInfo.ButtonInfo == ButtonInfo;
		}
	}
	public enum TileNavPaneHitTest { Outside, Control, Button }
}
