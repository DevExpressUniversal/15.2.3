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

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraBars.Navigation {
	public class NavigationButton : BaseButton, IButton {
		INavigationPageBase pageCore;
		public NavigationButton(INavigationPageBase page) {
			pageCore = page;
		}
		public override bool UseCaption {
			get {
				return pageCore.Properties.ActualShowMode == ItemShowMode.ImageAndText ||
					pageCore.Properties.ActualShowMode == ItemShowMode.Text ||
					((pageCore.Properties.ActualShowMode == ItemShowMode.ImageOrText || pageCore.Properties.ActualShowMode == ItemShowMode.Default) & Image == null);
			}
		}
		public override bool UseImage {
			get { return pageCore.Properties.ActualShowMode != ItemShowMode.Text; }
		}
		public override ButtonStyle Style {
			get { return ButtonStyle.CheckButton; }
		}
		public override Image Image {
			get {
				if(pageCore.Image == null) {
					if(pageCore.ImageUri.HasDefaultImage)
						return pageCore.ImageUri.GetDefaultImage();
					if(pageCore.ImageUri.HasLargeImage)
						return pageCore.ImageUri.GetLargeImage();
					if(pageCore.ImageUri.HasImage)
						return pageCore.ImageUri.GetImage();
				}
				return pageCore.Image; 
			}
		}
		public override string Caption {
			get { return System.String.IsNullOrEmpty(Page.PageText) ? Page.Caption : Page.PageText; }
		}
		public override bool Visible {
			get { return Page.PageVisible; }
		}
		public INavigationPageBase Page {
			get { return pageCore; }
		}
	}
	public class NavigationPaneButtonsPanelHandler : ButtonsPanelHandler {
		public NavigationPaneButtonsPanelHandler(IButtonsPanel panel)
			: base(panel) {
		}
		public override void OnMouseDown(System.Windows.Forms.MouseEventArgs e) {
			if(e.Button == MouseButtons.Left && HotInfo != null) {
				BaseButtonInfo hitInfo = CalcHitInfo(e.Location);
				if(hitInfo != null && AreEqual(hitInfo.Button, HotInfo.Button))
					PerformClick(hitInfo.Button);
			}
			Reset();
		}
		public override void OnMouseUp(System.Windows.Forms.MouseEventArgs e) { }
	}
	public class NavigationPaneButtonsPanelViewInfo : ButtonsPanelViewInfo {
		public NavigationPaneButtonsPanelViewInfo(IButtonsPanel panel)
			: base(panel) {
		}
		protected override BaseButtonInfo CreateButtonInfo(IBaseButton button) {
			bool rotete = Panel != null && Panel.ButtonRotationAngle != RotationAngle.None;
			var info = new BaseButtonInfo(button, rotete);
			return info;
		}
	}
	public class NavigationPaneButtonsPanel : ButtonsPanel, IButtonsPanel {
		public NavigationPaneButtonsPanel(IButtonsPanelOwner owner)
			: base(owner) {
		}
		protected override IButtonsPanelHandler CreateHandler() {
			return new NavigationPaneButtonsPanelHandler(this);
		}
		protected override IButtonsPanelViewInfo CreateViewInfo() {
			return new NavigationPaneButtonsPanelViewInfo(this);
		}
		void IButtonsPanel.PerformClick(IBaseButton button) {
			if(Buttons.Contains(button)) {
				IButtonsPanelOwnerEx ownerEx = Owner as IButtonsPanelOwnerEx;
				if(ownerEx != null && !ownerEx.CanPerformClick(button)) return;
				if(button.Properties.Style == ButtonStyle.CheckButton) {
					button.Properties.BeginUpdate();
					CheckButtonGroupIndex(button);
					button.Properties.Checked = CalcNewCheckedState(button);
					button.Properties.CancelUpdate();
					Owner.Invalidate();
				}
				else RaiseButtonClick(button);
			}
		}
		public override void CheckButtonGroupIndex(IBaseButton clickedButton) {
			if(clickedButton.Properties.GroupIndex == -1) return;
			BeginUpdate();
			foreach(IBaseButton button in Buttons) {
				button.Properties.LockCheckEvent();
				if(button.Properties.GroupIndex == clickedButton.Properties.GroupIndex && button != clickedButton)
					button.Properties.Checked = false;
				button.Properties.UnlockCheckEvent();
			}
			CancelUpdate();
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public new virtual bool CalcNewCheckedState(IBaseButton button) {
			return !button.Properties.Checked;
		}
	}
	public class NavigationPaneButtonsPanelSkinPainter : BaseButtonsPanelSkinPainter {
		public NavigationPaneButtonsPanelSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override void DrawButtons(GraphicsCache cache, IButtonsPanelViewInfo info) {
			BaseButtonPainter painter = GetButtonPainter();
			if(info.Buttons == null) return;
			foreach(BaseButtonInfo buttonInfo in info.Buttons) {
				buttonInfo.State = CalcButtonState(buttonInfo.Button, info.Panel);
				if((buttonInfo.State & ObjectState.Pressed) != 0) continue;
				BaseButtonPainter actualPainter = painter.GetButtonPainter(buttonInfo.Button.GetType());
				ObjectPainter.DrawObject(cache, actualPainter, buttonInfo);
			}
			IEnumerable<BaseButtonInfo> pressedInfos = info.Buttons.Where((buttonInfo) => ((buttonInfo.State & ObjectState.Pressed) != 0));
			foreach(var item in pressedInfos) {
				ObjectPainter.DrawObject(cache, painter, item);
			}
		}
		public override BaseButtonPainter GetButtonPainter() {
			return new NavigationButtonSkinPainter(Provider);
		}
	}
	public class NavigationButtonSkinPainter : BaseButtonSkinPainter {
		public NavigationButtonSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override void DrawStandartBackground(GraphicsCache cache, BaseButtonInfo info) {
			Rectangle saveBounds = info.Bounds;
			try {
				info.Bounds = GetInfoBounds(info);
				DrawNavigationPaneButtonBackground(info);
			}
			finally {
				info.Bounds = saveBounds;
			}
			RotateFlipType rotateFlipType = GetRotateBackgroundFlipType();
			BitmapRotate.RotateBitmap(rotateFlipType);
			info.Paint.DrawImage(cache.Graphics, BitmapRotate.BufferBitmap, info.Bounds, new Rectangle(Point.Empty, info.Bounds.Size), true);
			BitmapRotate.RestoreBitmap(rotateFlipType);
			DrawArrow(info, cache);
		}
		protected virtual Rectangle GetInfoBounds(BaseButtonInfo info) {
			 return new Rectangle(0, 0, info.Bounds.Height, info.Bounds.Width);
		}
		protected override void CheckForeColor(BaseButtonInfo info) {
			if(PaintAppearance.ForeColor == DefaultAppearance.ForeColor || PaintAppearance.ForeColor == Color.Empty) {
				if((info.ButtonPanelOwner as NavigationPane).State != NavigationPaneState.Collapsed) {
					if(info.Hot)
						PaintAppearance.ForeColor = GetSkinHotColor(DefaultAppearance.ForeColor);
					if(info.Pressed)
						PaintAppearance.ForeColor = GetSkinPressedColor(DefaultAppearance.ForeColor);
					if(!info.Hot && !info.Pressed)
						PaintAppearance.ForeColor = GetSkinNormalColor(DefaultAppearance.ForeColor);
				}
				else {
					if(info.Hot)
						PaintAppearance.ForeColor = GetSkinCollapsedHotColor(DefaultAppearance.ForeColor);
					else
						PaintAppearance.ForeColor = GetSkinCollapsedNormalColor(DefaultAppearance.ForeColor);
				}
			}
			base.CheckForeColor(info);
		}
		protected virtual Color GetSkinHotColor(Color defaultColor) {
			Color result = NavigationPaneSkins.GetSkin(SkinProvider).Colors.GetColor(NavigationPaneSkins.PaneButtonRegularStateHotColor, defaultColor);
			return result;
		}
		protected virtual Color GetSkinNormalColor(Color defaultColor) {
			Color result = NavigationPaneSkins.GetSkin(SkinProvider).Colors.GetColor(NavigationPaneSkins.PaneButtonRegularStateNormalColor, defaultColor);
			return result;
		}
		protected virtual Color GetSkinPressedColor(Color defaultColor) {
			Color result = NavigationPaneSkins.GetSkin(SkinProvider).Colors.GetColor(NavigationPaneSkins.PaneButtonRegularStatePressedColor, defaultColor);
			return result;
		}
		protected virtual Color GetSkinCollapsedHotColor(Color defaultColor) {
			Color result = NavigationPaneSkins.GetSkin(SkinProvider).Colors.GetColor(NavigationPaneSkins.PaneButtonCollapsedStateHotColor, defaultColor);
			return result;
		}
		protected virtual Color GetSkinCollapsedNormalColor(Color defaultColor) {
			Color result = NavigationPaneSkins.GetSkin(SkinProvider).Colors.GetColor(NavigationPaneSkins.PaneButtonCollapsedStateNormalColor, defaultColor);
			return result;
		}
		protected virtual RotateFlipType GetRotateBackgroundFlipType() {
			SkinElement element = null;
			var pane = Info.ButtonPanelOwner as NavigationPane;
			if(pane == null) return RotateFlipType.Rotate90FlipX;
			if(info != null) {
				if(pane.State == NavigationPaneState.Collapsed)
					element = GetNavigationPaneSkin()[NavigationPaneSkins.SkinPaneButtonCollapsedState];
			}
			if(element == null)
				element = GetNavigationPaneSkin()[NavigationPaneSkins.SkinPaneButtonRegularState];
			if(element == null || pane.IsRightToLeftLayout())
				return RotateFlipType.Rotate270FlipXY;
			return RotateFlipType.Rotate90FlipX;
		}
		protected virtual void DrawNavigationPaneButtonBackground(BaseButtonInfo info) {
			BitmapRotate.CreateBufferBitmap(info.Bounds.Size, true);
			SkinElementInfo elementInfo = new SkinElementInfo(GetBackground(), info.Bounds);
			elementInfo.ImageIndex = GetBackgroundImageIndex(info);
			elementInfo.State = info.State;
			ObjectPainter.DrawObject(BitmapRotate.BufferCache, SkinElementPainter.Default, elementInfo);
		}
		protected virtual void DrawArrow(BaseButtonInfo info, GraphicsCache cache) {
			var navigationPane = info.ButtonPanelOwner as NavigationPane;
			if(navigationPane != null && navigationPane.State == NavigationPaneState.Collapsed) return;
			SkinElement element = GetButtonArrow();
			if(element == null || element.Image == null) return;
			bool verticalLayout = (element.Image.Layout == SkinImageLayout.Vertical);
			Size arrowSize = Size.Empty;
			if(verticalLayout)
				arrowSize = new Size(element.Image.Image.Width, element.Image.Image.Height / element.Image.ImageCount);
			else
				arrowSize = new Size(element.Image.Image.Width / element.Image.ImageCount, element.Image.Image.Height);
			Rectangle bounds = DevExpress.Utils.PlacementHelper.Arrange(arrowSize, info.Bounds, navigationPane.Dock == DockStyle.Right ?  ContentAlignment.MiddleLeft : ContentAlignment.MiddleRight);
			SkinElementInfo arrowInfo = new SkinElementInfo(GetButtonArrow(), bounds);
			   arrowInfo.ImageIndex = GetBackgroundImageIndex(info);
			arrowInfo.State = info.State;
			RotateFlipType rotateFlipType = navigationPane.Dock == DockStyle.Right ? RotateFlipType.RotateNoneFlipXY : RotateFlipType.RotateNoneFlipNone;
			new RotateObjectPaintHelper().DrawRotated(cache, arrowInfo, SkinElementPainter.Default, rotateFlipType);
		}
		protected virtual int GetBackgroundImageIndex(BaseButtonInfo info) {
			var button = info.Button as NavigationButton;
			if(button == null || button.Page == null) return -1;
			NavigationPane navigationPane = info.ButtonPanelOwner as NavigationPane;
			return navigationPane != null && navigationPane.ButtonsPanel.ViewInfo.Buttons.IndexOf(info) == 0 && info.Pressed ? 3 : -1;
		}
		protected virtual SkinElement GetButtonArrow() {
			return GetNavigationPaneSkin()[NavigationPaneSkins.SkinPaneButtonArrow];
		}
		protected override SkinElement GetBackground() {
			SkinElement result = null;
			if(info != null) {
				var pane = Info.ButtonPanelOwner as NavigationPane;
				if(pane != null && pane.State == NavigationPaneState.Collapsed)
					result = GetNavigationPaneSkin()[NavigationPaneSkins.SkinPaneButtonCollapsedState];
			}
			if(result == null)
				result = GetNavigationPaneSkin()[NavigationPaneSkins.SkinPaneButtonRegularState];
			return result ?? GetSkin()[DockingSkins.SkinTabHeaderHideBar];
		}
		BaseButtonInfo info;
		public override void DrawObject(ObjectInfoArgs e) {
			Rectangle saveBounds = e.Bounds;
			info = e as BaseButtonInfo;
			var button = info.Button as NavigationButton;
			if((e.State & ObjectState.Pressed) != 0) {
				CalcSelectedButtonBounds(e);
				if(button.Page is NavigationPage && (button.Page as NavigationPage).Properties.CanBorderColorBlending) {
					Color borderColor = (button.Page as NavigationPage).Properties.ActualAppearanceCaption.BorderColor;
					using(SkinElementCustomColorizer colorizer = new SkinElementCustomColorizer(borderColor)) {
						base.DrawObject(e);
						e.Bounds = saveBounds;
						return;
					}
				}
			}
			base.DrawObject(e);
			e.Bounds = saveBounds;
		}
		protected virtual void CalcSelectedButtonBounds(ObjectInfoArgs e) {
			int overlapValue = GetOverlapValue();
			int expandValue = GetExpandValue();
			BaseButtonInfo info = e as BaseButtonInfo;
			Point location = new Point(e.Bounds.X, e.Bounds.Y - expandValue);
			Size size = new Size(e.Bounds.Width + overlapValue, e.Bounds.Height + expandValue * 2);
			var navigationPane = info.ButtonPanelOwner as NavigationPane;
			if(info != null) {
				var frame = info.ButtonPanelOwner as NavigationFrame;
				if(frame != null && frame.IsRightToLeftLayout()) {
					location.X -= overlapValue;
				}
			}
			e.Bounds = new Rectangle(location, size);
		}
		protected int GetOverlapValue() {
			int result = 0;
			var overlap = GetOverlapValueFromSkin();
			if(overlap != null)
				result = (int)overlap;
			return result;
		}
		protected virtual object GetOverlapValueFromSkin() {
			return GetNavigationPaneSkin().Properties[NavigationPaneSkins.SelectedPageOverlapValue];
		}
		protected int GetExpandValue() {
			int result = 0;
			var overlap = GetExpandValueFromSkin();
			if(overlap != null)
				result = (int)overlap;
			return result;
		}
		protected virtual object GetExpandValueFromSkin() {
			return GetNavigationPaneSkin().Properties[NavigationPaneSkins.SelectedPageExpandValue];
		}
		protected virtual Skin GetNavigationPaneSkin() {
			return NavigationPaneSkins.GetSkin(SkinProvider);
		}
		protected override Skin GetSkin() {
			return DockingSkins.GetSkin(SkinProvider);
		}
	}
	public class TabPaneButtonsPanel : NavigationPaneButtonsPanel {
		public TabPaneButtonsPanel(IButtonsPanelOwner owner)
			: base(owner) {
		}
		protected override IButtonsPanelViewInfo CreateViewInfo() {
			return new TabPaneButtonsPanelViewInfo(this);
		}
		public override bool CalcNewCheckedState(IBaseButton button) {
			return true;
		}
	}
	public class TabPaneButtonsPanelViewInfo : ButtonsPanelViewInfo {
		public TabPaneButtonsPanelViewInfo(IButtonsPanel panel)
			: base(panel) {
		}
		protected override Point CalcButtonInfos(Graphics g, BaseButtonPainter buttonPainter, int interval, bool horz, IEnumerable<BaseButtonInfo> buttons, Point offset) {
			for(int i = buttons.Count() - 1; i >= 0; i--) {
				buttons.ToList()[i].Calc(g, buttonPainter, offset, Content, horz);
				if(horz)
					offset.X -= (Panel.RightToLeft ? -1 : 1) * (buttons.ToList()[i].Bounds.Width + interval);
				else
					offset.Y += (Panel.RightToLeft ? -1 : 1) * (buttons.ToList()[i].Bounds.Height + interval);
			}
			return offset;
		}
	}
	public class TabPaneButtonsPanelSkinPainter : NavigationPaneButtonsPanelSkinPainter {
		public TabPaneButtonsPanelSkinPainter(ISkinProvider provider)
			: base(provider) { }
		public override BaseButtonPainter GetButtonPainter() {
			return new TabPaneNavigationButtonSkinPainter(Provider);
		}
	}
	public class TabPaneNavigationButtonSkinPainter : NavigationButtonSkinPainter {
		public TabPaneNavigationButtonSkinPainter(ISkinProvider provider) : base(provider) { }
		protected override RotateFlipType GetRotateBackgroundFlipType() {
			return RotateFlipType.RotateNoneFlipX;
		}
		protected override void CalcSelectedButtonBounds(ObjectInfoArgs e) {
			int overlapValue = GetOverlapValue();
			int expandValue = GetExpandValue();
			BaseButtonInfo info = e as BaseButtonInfo;
			Point location = e.Bounds.Location;
			Size size = new Size(e.Bounds.Width, e.Bounds.Height + overlapValue);
			location.X -= expandValue;
			size.Width += (expandValue * 2);
			e.Bounds = new Rectangle(location, size);
		}
		protected override int GetBackgroundImageIndex(BaseButtonInfo info) {
			return -1;
		}
		protected override object GetOverlapValueFromSkin() {
			return GetNavigationPaneSkin().Properties[NavigationPaneSkins.SelectedTabPageOverlapValue];
		}
		protected override object GetExpandValueFromSkin() {
			return GetNavigationPaneSkin().Properties[NavigationPaneSkins.SelectedTabPageExpandValue];
		}
		protected override Rectangle GetInfoBounds(BaseButtonInfo info) {
			return new Rectangle(0, 0, info.Bounds.Width, info.Bounds.Height);
		}
		protected override SkinElement GetBackground() {
			SkinElement result = null;
			result = GetNavigationPaneSkin()[NavigationPaneSkins.SkinTabPaneButton];
			return result ?? GetSkin()[DockingSkins.SkinTabHeaderHideBar];
		}
		protected override SkinElement GetButtonArrow() {
			return GetNavigationPaneSkin()[NavigationPaneSkins.SkinTabPaneButtonArrow];
		}
		protected override void DrawArrow(BaseButtonInfo info, GraphicsCache cache) {
			var navigationPane = info.ButtonPanelOwner as NavigationPane;
			SkinElement element = GetButtonArrow();
			if(element == null || element.Image == null) return;
			bool verticalLayout = (element.Image.Layout == SkinImageLayout.Vertical);
			Size arrowSize = Size.Empty;
			if(verticalLayout)
				arrowSize = new Size(element.Image.Image.Width, element.Image.Image.Height / element.Image.ImageCount);
			else
				arrowSize = new Size(element.Image.Image.Width / element.Image.ImageCount, element.Image.Image.Height);
			int width = arrowSize.Width;
			arrowSize.Width = arrowSize.Height;
			arrowSize.Height = width;
			Rectangle bounds = DevExpress.Utils.PlacementHelper.Arrange(arrowSize, info.Bounds, ContentAlignment.BottomCenter);
			SkinElementInfo arrowInfo = new SkinElementInfo(GetButtonArrow(), bounds);
			arrowInfo.ImageIndex = -1;
			arrowInfo.State = info.State;
			RotateObjectPaintHelper helper = new RotateObjectPaintHelper();
			helper.DrawRotated(cache, arrowInfo, SkinElementPainter.Default, RotateFlipType.Rotate90FlipNone);
		}
	}
}
