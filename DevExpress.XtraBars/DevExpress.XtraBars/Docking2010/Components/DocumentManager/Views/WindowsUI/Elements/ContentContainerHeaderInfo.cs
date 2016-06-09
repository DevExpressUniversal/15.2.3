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
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;
using DevExpress.XtraBars.Docking2010.DragEngine;
using DevExpress.Utils.Gesture;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface IHeaderButton {
		IButtonsPanelOwner Owner { get; }
	}
	public interface IContentContainerHeaderInfo : IBaseElementInfo, IUIElement, IStringImageProvider, IInteractiveElementInfo {
		Size CalcMinSize(Graphics g, Rectangle bounds);
		string Text { get; }
		BaseButtonInfo ButtonInfo { get; }
		Rectangle TextBounds { get; }
		Rectangle ButtonBounds { get; }
		Rectangle SubtitleBounds { get; }
		string Subtitle { get; }
		AppearanceObject PaintAppearanceHeader { get; }
		AppearanceObject PaintAppearanceSubtitle { get; }
		AppearanceObject PaintAppearanceItemHeader { get; }
		WindowsUIButtonsPanel ButtonsPanel { get; }		
		ObjectState CalcButtonState(IBaseButton button);
		BaseButtonInfo CalcHitInfo(Point point);
		void PerformClick(IBaseButton button);
		void Invalidate();
		bool DragMoveHitTest(Point point);
	}
	class ContentContainerHeaderInfo : BaseElementInfo, IContentContainerHeaderInfo {
		IContentContainerInfo containerInfoCore;
		BaseButton buttonCore;
		BaseButtonInfo buttonInfoCore;
		ButtonHandler handlerCore;
		AppearanceObject paintAppearanceCore;
		AppearanceObject paintAppearanceTitleCore;
		AppearanceObject paintAppearanceItemHeaderCore;
		ObjectPainter painterCore;
		protected ICustomButtonsOwner CustomButtonOwner {
			get { return ContainerInfo!= null && ContainerInfo.Container is ICustomButtonsOwner ? ((ICustomButtonsOwner)ContainerInfo.Container) : null; }
		}
		public ContentContainerHeaderInfo(WindowsUIView view, IContentContainerInfo containerInfo)
			: base(view) {
			paintAppearanceCore = new FrozenAppearance();
			paintAppearanceTitleCore = new FrozenAppearance();
			paintAppearanceItemHeaderCore = new FrozenAppearance();
			containerInfoCore = containerInfo;
			buttonCore = CreateHeaderButton();
			buttonInfoCore = new WindowsUIBaseButtonInfo(buttonCore);
			handlerCore = new ButtonHandler(this);
			UpdateStyleCore();
		}
		protected override void OnDispose() {
			Ref.Dispose(ref handlerCore);
			Ref.Dispose(ref paintAppearanceCore);
			Ref.Dispose(ref paintAppearanceTitleCore);
			Ref.Dispose(ref paintAppearanceItemHeaderCore);
			Ref.Dispose(ref buttonCore);
			buttonInfoCore = null;
			painterCore = null;
			base.OnDispose();
			containerInfoCore = null;
		}
		public WindowsUIButtonsPanel ButtonsPanel {
			get { return CustomButtonOwner != null ? CustomButtonOwner.ButtonsPanel : null; }
		}
		public override Type GetUIElementKey() {
			return typeof(IContentContainerHeaderInfo);
		}
		protected virtual BaseButton CreateHeaderButton() {
			return new BackButton() { Container = ContainerInfo.Container };
		}
		public new WindowsUIView Owner {
			get { return base.Owner as WindowsUIView; }
		}
		public AppearanceObject PaintAppearanceHeader {
			get { return paintAppearanceCore; }
		}
		public AppearanceObject PaintAppearanceSubtitle{
			get { return paintAppearanceTitleCore; }
		}
		public AppearanceObject PaintAppearanceItemHeader {
			get { return paintAppearanceItemHeaderCore; }
		}
		public ContentContainerHeaderInfoPainter Painter {
			get { return painterCore as ContentContainerHeaderInfoPainter; }
		}
		public BaseButtonHandler Handler {
			get { return handlerCore; }
		}
		public IContentContainerInfo ContainerInfo {
			get { return containerInfoCore; }
		}
		public string Text { get; private set; }
		public string Subtitle { get; private set; }
		public Rectangle TextBounds { get; private set; }
		public Rectangle ButtonBounds { get; private set; }
		public Rectangle SubtitleBounds { get; private set; }
		public StringInfo TitleInfo { get; private set; }
		public int ButtonsPanelHeight { get; private set; }
		public BaseButtonInfo ButtonInfo {
			get { return buttonInfoCore; }
		}
		BaseButtonPainter ButtonPainter;
		protected override void UpdateStyleCore() {
			painterCore = ((WindowsUIViewPainter)Owner.Painter).GetHeaderPainter(ContainerInfo);
			ButtonPainter = ((WindowsUIViewPainter)Owner.Painter).GetHeaderButtonPainter();
			ColoredElementsCache.Reset();
		}
		public Size CalcMinSize(Graphics g, Rectangle bounds) {
			AppearanceHelper.Combine(PaintAppearanceHeader, new AppearanceObject[] { Owner.AppearanceCaption }, 
				Painter.DefaultAppearanceHeader);
			AppearanceHelper.Combine(PaintAppearanceSubtitle, new AppearanceObject[] { ContainerInfo.Container.AppearanceSubtitle },
				Painter.DefaultAppearanceTitle);
			PaintAppearanceItemHeader.Assign(Painter.DefaultAppearanceItemHeader);
			using(GraphicsCache cache = new GraphicsCache(g)) {
				Text = GetContentContainerText();
				Subtitle = GetContentContainerSubTitle();
				bool hasText = !string.IsNullOrEmpty(Text);
				bool hasButton = ContainerInfo.Container.Parent != null;
				Rectangle headerRect = Painter.GetHeaderContentRectangle(new Rectangle(bounds.Left, bounds.Top, bounds.Width, 100));
				TitleInfo = StringPainter.Default.Calculate(g, PaintAppearanceSubtitle, PaintAppearanceSubtitle.TextOptions, Subtitle, Rectangle.Empty, cache.Paint, Owner);
				Size textSize = hasText ? CalcTextSize(cache, Text, PaintAppearanceHeader) : Size.Empty;
				Size buttonSize = hasButton ? ButtonInfo.CalcMinSize(cache.Graphics, ButtonPainter) : Size.Empty;
				int interval = (hasButton && hasText) ? Painter.ButtonToTextInterval : 0;
				Size contentSize = CalcMinContentSize(cache, textSize, buttonSize, interval);
				contentSize.Height += TitleInfo.Bounds.Height > 0 ? TitleInfo.Bounds.Height + TitleToTextInterval : 0;
				return new Size(contentSize.Width < 0 ? bounds.Width : 
					contentSize.Width - (headerRect.Width - bounds.Width),
					contentSize.Height - (headerRect.Height - 100));
			}
		}
		protected virtual Size CalcMinContentSize(GraphicsCache cache, Size textSize, Size buttonSize, int interval) {
			Size buttonsPanelSize = GetButtonsPanelMinSize(cache);
			return new Size(-1, Math.Max(buttonsPanelSize.Height, Math.Max(buttonSize.Height, textSize.Height)));
		}
		protected Size GetButtonsPanelMinSize(GraphicsCache cache) {
			return ButtonsPanel != null ? ButtonsPanel.ViewInfo.CalcMinSize(cache.Graphics) : Size.Empty; ;
		}
		static int TitleToTextInterval = 10;
		static int ButtonsToTextInterval = 21;
		protected sealed override void CalcCore(Graphics g, Rectangle bounds) {
			using(GraphicsCache cache = new GraphicsCache(g)) {
				Text = GetContentContainerText();
				bool hasText = !string.IsNullOrEmpty(Text);
				bool hasButton = ContainerInfo.Container.Parent != null;
				Rectangle headerRect = Painter.GetHeaderContentRectangle(new Rectangle(bounds.Left, bounds.Top, bounds.Width, 100));
				Size textSize = hasText ? CalcTextSize(cache, Text, PaintAppearanceHeader) : Size.Empty;
				Size buttonSize = hasButton ? ButtonInfo.CalcMinSize(cache.Graphics, ButtonPainter) : Size.Empty;
				int interval = (hasButton && hasText) ? Painter.ButtonToTextInterval : 0;
				int contentHeight = CalcMinContentSize(cache, textSize, buttonSize, interval).Height;
				CalcElements(headerRect.Location, textSize, buttonSize, new Size(Bounds.Width, contentHeight), buttonSize.Width + interval);
				CalcTitleBounds(cache, headerRect, textSize, buttonSize);
				if(hasButton) {
					ButtonInfo.Calc(cache.Graphics, ButtonPainter, new Point(ButtonBounds.Right, ButtonBounds.Top), ButtonBounds, true);
					ButtonInfo.PaintAppearance.BackColor = Owner.GetBackColor();
				}
				if(ButtonsPanel != null && CanShowCustomButtons()) {
					ButtonsPanel.ViewInfo.SetDirty();
					ButtonsPanel.ViewInfo.Calc(cache.Graphics, new Rectangle(TextBounds.Right + ButtonsToTextInterval, headerRect.Y + Painter.ButtonVerticalOffset,
						headerRect.Right - TextBounds.Right, ButtonsPanelHeight));
				}
			}
		}
		protected virtual string GetContentContainerText() {
			return string.IsNullOrEmpty(ContainerInfo.Container.Caption) ? Owner.Caption : ContainerInfo.Container.Caption;
		}
		protected virtual string GetContentContainerSubTitle() {
			return ContainerInfo.Container.Subtitle;
		}
		protected virtual bool CanShowCustomButtons() {
			return true;
		}
		protected virtual void CalcTitleBounds(GraphicsCache cache, Rectangle headerRect, Size textSize, Size buttonSize) {
			if(TitleInfo == null) return;
			Size buttonsPanelSize = GetButtonsPanelMinSize(cache);
			int y = headerRect.Y + Math.Max(buttonsPanelSize.Height, Math.Max(textSize.Height, buttonSize.Height));
			y += TitleInfo.Bounds.Height > 0 ? TitleToTextInterval : 0;
			SubtitleBounds = new Rectangle(headerRect.X, y, headerRect.Width, TitleInfo.Bounds.Height);
		}
		protected virtual void CalcElements(Point offset, Size textSize, Size buttonSize, Size contentSize, int textOffset) {
			ButtonBounds = CalcButtonBounds(offset, buttonSize, contentSize.Height);
			TextBounds = CalcTextBounds(offset, textSize, contentSize.Height, textOffset);
			ButtonsPanelHeight = contentSize.Height;
		}
		protected sealed override void DrawCore(GraphicsCache cache) {
			Painter.DrawObject(new ContentContainerHeaderInfoArgs(cache, this));
		}
		protected Size CalcTextSize(GraphicsCache cache, string text, AppearanceObject appearance) {
			return Size.Ceiling(appearance.CalcTextSize(cache, text, 0));
		}
		Rectangle CalcButtonBounds(Point offset, Size buttonSize, int contentHeight) {
			return new Rectangle(offset.X + Painter.ButtonHorizontalOffset,
				offset.Y + Painter.ButtonVerticalOffset + GetAdditionalOffset(buttonSize, contentHeight),
				buttonSize.Width, buttonSize.Height);
		}
		protected int GetAdditionalOffset(Size buttonSize, int contentHeight) {
			int offset = (contentHeight - buttonSize.Height) / 2;
			return offset;
		}
		public virtual bool DragMoveHitTest(Point hitPoint) {
			return (ButtonsPanel != null) && !ButtonBounds.Contains(hitPoint) && !ButtonsPanel.Bounds.Contains(hitPoint);
		}
		Rectangle CalcTextBounds(Point offset, Size textSize, int contentHeight, int textOffset) {
			return new Rectangle(offset.X + textOffset, offset.Y + (contentHeight - textSize.Height) / 2, textSize.Width, textSize.Height);
		}
		#region IUIElement
		IUIElement IUIElement.Scope { get { return ContainerInfo; } }
		UIChildren uiChildren = new UIChildren();
		UIChildren IUIElement.Children {
			get { return uiChildren; }
		}
		#endregion IUIElement
		Image IStringImageProvider.GetImage(string id) {
			if(Owner != null && Owner.Manager.Images != null)
				return ImageCollection.GetImageListImage(Owner.Manager.Images, id);
			return null;
		}
		void IContentContainerHeaderInfo.Invalidate() {
			if(!IsDisposing) Owner.Invalidate(Bounds);
		}
		BaseButtonInfo IContentContainerHeaderInfo.CalcHitInfo(Point point) {
			if(ButtonInfo.Bounds.Contains(point))
				return ButtonInfo;
			return null;
		}
		bool CanPerformClick(IBaseButton button) {
			if(button is BackButton) return Owner.RaiseBackButtonClick();
			return Owner.RaiseNavigationBarsButtonClick(button as BaseButton);
		}
		void IContentContainerHeaderInfo.PerformClick(IBaseButton button) {
			if(!CanPerformClick(button)) return;
			if(button == buttonCore)
				Owner.Controller.Back();
		}
		ObjectState IContentContainerHeaderInfo.CalcButtonState(IBaseButton button) {
			ObjectState state = ObjectState.Normal;
			if(!button.Properties.Enabled)
				state |= ObjectState.Disabled;
			if(button == Handler.HotButton)
				state |= ObjectState.Hot;
			if(button == Handler.PressedButton)
				state |= ObjectState.Pressed;
			return state;
		}
		internal class BackButton : BaseButton, IHeaderButton {
			Image image;
			internal BackButton() {
				image = Resources.ContentContainterActionResourceLoader.GetImage(DocumentManagerStringId.CommandBack);
			}
			protected internal IContentContainer Container { get; set; }
			protected override void OnDispose() {
				image = null;
				base.OnDispose();
			}
			public override string Caption {
				get { return null; }
				set { }
			}
			public override Image Image {
				get { return image; }
				set { }
			}
			IButtonsPanelOwner IHeaderButton.Owner {
				get { return Container as IButtonsPanelOwner; }
			}
		}
		class ButtonHandler : BaseButtonHandler, IDisposable {
			IContentContainerHeaderInfo info;
			public ButtonHandler(IContentContainerHeaderInfo info) {
				this.info = info;
			}
			protected override void Invalidate() {
				if(info != null) info.Invalidate();
			}
			protected override BaseButtonInfo CalcHitInfo(Point point) {
				return info.CalcHitInfo(point);
			}
			protected override void PerformClick(IBaseButton button) {
				if(info != null) info.PerformClick(button);
			}
			public void Dispose() {
				info = null;
			}
		}
		#region IInteractiveElementInfo Members
		void IInteractiveElementInfo.ProcessMouseDown(MouseEventArgs e) {
			if(!IsDisposing) {
				OnMouseDown(e);
				if(ButtonsPanel != null)
					ButtonsPanel.Handler.OnMouseDown(e);
			}
		}
		void IInteractiveElementInfo.ProcessMouseMove(MouseEventArgs e) {
			if(!IsDisposing) {
				OnMouseMove(e);
				if(ButtonsPanel != null)
					ButtonsPanel.Handler.OnMouseMove(e);
			}
		}
		void IInteractiveElementInfo.ProcessMouseUp(MouseEventArgs e) {
			if(!IsDisposing) {
				OnMouseUp(e);
				if(ButtonsPanel != null)
					ButtonsPanel.Handler.OnMouseUp(e);
			}
		}
		void IInteractiveElementInfo.ProcessMouseLeave() {
			if(!IsDisposing) {
				OnMouseLeave();
				if(ButtonsPanel != null)
					ButtonsPanel.Handler.OnMouseLeave();
			}
		}
		void IInteractiveElementInfo.ProcessMouseWheel(MouseEventArgs e) { }
		bool IInteractiveElementInfo.ProcessFlick(Point point, FlickGestureArgs args) { return false; }
		bool IInteractiveElementInfo.ProcessGesture(GestureID gid, GestureArgs args, object[] parameters) { return false; }
		protected virtual void OnMouseDown(MouseEventArgs e) {
			if(Handler != null) Handler.OnMouseDown(e);
		}
		protected virtual void OnMouseMove(MouseEventArgs e) {
			if(Handler != null) Handler.OnMouseMove(e);
		}
		protected virtual void OnMouseUp(MouseEventArgs e) {
			if(Handler != null) Handler.OnMouseUp(e);
		}
		protected virtual void OnMouseLeave() {
			if(Handler != null) Handler.OnMouseLeave();
		}
		protected virtual void OnMouseWheel(MouseEventArgs e) { }
		#endregion
	}
	class ContentContainerHeaderInfoArgs : ObjectInfoArgs {
		IContentContainerHeaderInfo infoCore;
		public IContentContainerHeaderInfo Info {
			get { return infoCore; }
		}
		public ContentContainerHeaderInfoArgs(GraphicsCache cache, IContentContainerHeaderInfo info)
			: base(cache) {
			this.infoCore = info;
		}
	}
	class ContentContainerHeaderInfoPainter : ObjectPainter {
		public sealed override void DrawObject(ObjectInfoArgs e) {
			ContentContainerHeaderInfoArgs args = e as ContentContainerHeaderInfoArgs;
			DrawHeader(e.Cache, args.Info);
		}
		AppearanceDefault appearanceDefaultHeaderCore;
		public AppearanceDefault DefaultAppearanceHeader {
			get {
				if(appearanceDefaultHeaderCore == null) 
					appearanceDefaultHeaderCore = CreateDefaultAppearanceHeader();
				return appearanceDefaultHeaderCore;
			}
		}
		AppearanceDefault appearanceDefaultTitleCore;
		public AppearanceDefault DefaultAppearanceTitle {
			get {
				if(appearanceDefaultTitleCore == null)
					appearanceDefaultTitleCore = CreateDefaultAppearanceTitle();
				return appearanceDefaultTitleCore;
			}
		}
		protected virtual AppearanceDefault CreateDefaultAppearanceHeader() {
			return new AppearanceDefault(Color.Empty, Color.Transparent, WindowsUIViewPainter.DefaultFont2);
		}
		protected virtual AppearanceDefault CreateDefaultAppearanceTitle() {
			return new AppearanceDefault(Color.Empty, Color.Transparent, WindowsUIViewPainter.DefaultFontSplashScreen);
		}
		AppearanceDefault appearanceDefaultItemHeaderCore;
		public AppearanceDefault DefaultAppearanceItemHeader {
			get {
				if(appearanceDefaultItemHeaderCore == null) 
					appearanceDefaultItemHeaderCore = CreateDefaultAppearanceItemHeader();
				return appearanceDefaultItemHeaderCore;
			}
		}
		protected virtual AppearanceDefault CreateDefaultAppearanceItemHeader() {
			return new AppearanceDefault(Color.Empty, Color.Transparent, WindowsUIViewPainter.DefaultFont);
		}
		public virtual int ButtonVerticalOffset {
			get { return 0; }
		}
		public virtual int ButtonHorizontalOffset {
			get { return 0; }
		}
		public virtual int ButtonToTextInterval {
			get { return 5; }
		}
		public virtual Rectangle GetHeaderContentRectangle(Rectangle header) {
			return Rectangle.Inflate(header, -40, -12);
		}
		public virtual Rectangle GetItemsBoundsByContentRectangle(Rectangle items) {
			return Rectangle.Inflate(items, 40, 2);
		}
		protected virtual void DrawHeader(GraphicsCache cache, IContentContainerHeaderInfo info) {
			if(info.Bounds.IsEmpty) return;
			if(!info.TextBounds.Size.IsEmpty)
				info.PaintAppearanceHeader.DrawString(cache, info.Text, info.TextBounds);
			if(!info.SubtitleBounds.Size.IsEmpty)
				StringPainter.Default.DrawString(cache, info.PaintAppearanceSubtitle, info.Subtitle, info.SubtitleBounds, info.PaintAppearanceSubtitle.TextOptions, info.Owner);
			if(!info.ButtonBounds.Size.IsEmpty) {
				BaseButtonPainter buttonPainter = ((WindowsUIViewPainter)info.Owner.Painter).GetHeaderButtonPainter();
				info.ButtonInfo.State = info.CalcButtonState(info.ButtonInfo.Button);
				var view = (WindowsUI.WindowsUIView)info.Owner;
				info.ButtonInfo.PaintAppearance.BackColor = view.GetBackColor();
				if(!view.RaiseCustomDrawBackButton(cache, info.ButtonInfo, buttonPainter, info.ButtonInfo.PaintAppearance))
					ObjectPainter.DrawObject(cache, buttonPainter, info.ButtonInfo);
			}
			if(info.ButtonsPanel != null && info.ButtonsPanel.ViewInfo != null)
				ObjectPainter.DrawObject(cache, info.ButtonsPanel.Owner.GetPainter(), (ObjectInfoArgs)info.ButtonsPanel.ViewInfo);
		}
	}
	class ContentContainerHeaderInfoSkinPainter : ContentContainerHeaderInfoPainter {
		ISkinProvider skinProvider;
		public ContentContainerHeaderInfoSkinPainter(ISkinProvider provider) {
			this.skinProvider = provider;
		}
		protected override AppearanceDefault CreateDefaultAppearanceHeader() {
			AppearanceDefault appearance = base.CreateDefaultAppearanceHeader();
			SkinElement header = GetHeader();
			if(header != null) {
				header.Apply(appearance);
				header.ApplyForeColorAndFont(appearance);
			}
			return appearance;
		}
		protected override AppearanceDefault CreateDefaultAppearanceTitle() {
			AppearanceDefault appearance = base.CreateDefaultAppearanceTitle();
			SkinElement header = GetHeader();
			if(header != null) {
				appearance.ForeColor =  header.Color.GetForeColor();
			}
			return appearance;
		}
		protected override AppearanceDefault CreateDefaultAppearanceItemHeader() {
			AppearanceDefault appearance = base.CreateDefaultAppearanceItemHeader();
			SkinElement itemHeader = GetItemHeader();
			if(itemHeader != null) {
				itemHeader.Apply(appearance);
				itemHeader.ApplyForeColorAndFont(appearance);
			}
			return appearance;
		}
		public override int ButtonVerticalOffset {
			get {
				object voffset = GetSkin().Properties[MetroUISkins.HeaderButtonVerticalOffset];
				if(voffset != null)
					return (int)voffset;
				return base.ButtonVerticalOffset;
		   }
		}
		public override int ButtonHorizontalOffset {
			get {
				object hoffset = GetSkin().Properties[MetroUISkins.HeaderButtonHorizontalOffset];
				if(hoffset != null)
					return (int)hoffset;
				return base.ButtonHorizontalOffset;
			}
		}
		public override int ButtonToTextInterval {
			get {
				object interval = GetSkin().Properties[MetroUISkins.HeaderButtonToTextInterval];
				if(interval != null)
					return (int)interval;
				return base.ButtonToTextInterval;
			}
		}
		public override Rectangle GetHeaderContentRectangle(Rectangle header) {
			SkinElement headerElement = GetHeader();
			if(headerElement != null)
				return headerElement.ContentMargins.Deflate(header);
			return base.GetHeaderContentRectangle(header);
		}
		public override Rectangle GetItemsBoundsByContentRectangle(Rectangle items) {
			SkinElement itemHeaderElement = GetItemHeader();
			if(itemHeaderElement != null)
				return itemHeaderElement.ContentMargins.Inflate(items);
			return base.GetItemsBoundsByContentRectangle(items);
		}
		protected virtual SkinElement GetHeader() {
			return GetSkin()[MetroUISkins.SkinHeader];
		}
		protected virtual SkinElement GetItemHeader() {
			return GetSkin()[MetroUISkins.SkinItemHeader];
		}
		protected virtual Skin GetSkin() {
			return MetroUISkins.GetSkin(skinProvider);
		}
	}
}
