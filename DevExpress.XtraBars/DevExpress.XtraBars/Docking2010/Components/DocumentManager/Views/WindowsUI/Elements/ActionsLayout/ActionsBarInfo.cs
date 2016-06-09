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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking2010.DragEngine;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	class ActionButton : DevExpress.XtraEditors.ButtonPanel.BaseButton, IButton {
		IContentContainerAction action;
		public ActionButton(IContentContainerAction action) {
			this.action = action;
		}
		protected override DevExpress.XtraEditors.ButtonPanel.ImageLocation ImageLocationCore {
			get { return DevExpress.XtraEditors.ButtonPanel.ImageLocation.AboveText; }
		}
		public override string Caption {
			get { return action.Caption; }
		}
		public override string ToolTip {
			get { return action.Description; }
		}
		public new bool IsLeft {
			get { return GetIsLeft(); }
			set { SetIsLeft(value); }
		}
	}
	public interface IContentContainerActionsBarInfo : IBaseElementInfo, IUIElement, IInteractiveElementInfo {
		void Update();
		IUIElementInfo HitTest(Point location);
		Size CalcMinSize(Graphics g);
		void AttachToContainer(IContentContainer container, IEnumerable<IContentContainerAction> actualActions);
		void DetachFromContainer(IContentContainer container);
	}
	abstract class BaseContentContainerActionsBarInfo : BaseElementInfo, IContentContainerActionsBarInfo, DevExpress.XtraEditors.ButtonPanel.IButtonsPanelOwnerEx {
		ButtonsPanel buttonsPanelCore;
		ObjectPainter painterCore;
		ObjectPainter buttonsPanelPainterCore;
		AppearanceObject paintAppearanceCore;
		IPopupActionInfo popupActionInfo;
		public BaseContentContainerActionsBarInfo(WindowsUIView view)
			: base(view) {
			paintAppearanceCore = new FrozenAppearance();
			buttonsPanelCore = CreateButtonsPanel();
			popupActionInfo = CreatePopupActionInfo();
			ButtonsPanel.ButtonClick += OnActionButtonClick;
			UpdateStyleCore();
		}
		protected override void OnDispose() {
			ButtonsPanel.ButtonClick -= OnActionButtonClick;
			LayoutHelper.Unregister(this);
			Ref.Dispose(ref paintAppearanceCore);
			Ref.Dispose(ref buttonsPanelCore);
			base.OnDispose();
		}
		public new WindowsUIView Owner {
			get { return base.Owner as WindowsUIView; }
		}
		public DocumentManager Manager {
			get { return base.Owner.Manager; }
		}
		public AppearanceObject PaintAppearance {
			get { return paintAppearanceCore; }
		}
		public ButtonsPanel ButtonsPanel {
			get { return buttonsPanelCore; }
		}
		protected virtual ButtonsPanel CreateButtonsPanel() {
			return new ButtonsPanel(this);
		}
		protected virtual IPopupActionInfo CreatePopupActionInfo() { return null; }
		#region IContentContainerActionsBarInfo Members
		IContentContainer containerCore;
		protected abstract ActionType ActionType { get; }
		IUIElementInfo IContentContainerActionsBarInfo.HitTest(Point location) {
			if(popupActionInfo != null) {
				Point screenPoint = Owner.Manager.ClientToScreen(location);
				if(popupActionInfo.HitTest(screenPoint))
					return popupActionInfo;
			}
			return Bounds.Contains(location) ? this : null;
		}
		void IContentContainerActionsBarInfo.AttachToContainer(IContentContainer container, IEnumerable<IContentContainerAction> actualActions) {
			UpdateStyleCore();
			containerCore = container;
			ButtonsPanel.BeginUpdate();
			IContentContainerInternal contentContainer = container as IContentContainerInternal;
			if(contentContainer != null) {
				foreach(IContentContainerAction action in actualActions) {
					if(ContentContainerAction.GetActionLayout(action).Type == ActionType)
						ButtonsPanel.Buttons.Add(CreateActionButton(container, action));
				}
			}
			ButtonsPanel.ViewInfo.SetDirty();
			ButtonsPanel.CancelUpdate();
		}
		void IContentContainerActionsBarInfo.DetachFromContainer(IContentContainer container) {
			ButtonsPanel.BeginUpdate();
			IButton[] buttons = ButtonsPanel.Buttons.CleanUp<IButton>() as IButton[];
			for(int i = 0; i < buttons.Length; i++)
				buttons[i].Dispose();
			ButtonsPanel.CancelUpdate();
			containerCore = null;
			PopupCancel();
		}
		void IContentContainerActionsBarInfo.Update() {
			ButtonsPanel.BeginUpdate();
			IButton[] buttons = ButtonsPanel.Buttons.ToArray<IButton>() as IButton[];
			for(int i = 0; i < buttons.Length; i++) {
				ActionButton button = buttons[i] as ActionButton;
				var tag = button.Tag as ContentContainerAction.Tag;
				button.Visible = ContentContainerAction.CanExecute(tag.Args);
				AppearanceHelper.Combine(button.Appearance, new AppearanceObject[] { 
				Owner.AppearanceActionsBarButton, Owner.AppearanceActionsBar }, Painter.DefaultAppearanceActionsButton);
			}
			ButtonsPanel.ViewInfo.SetDirty();
			ButtonsPanel.EndUpdate();
			if(popupActionInfo != null)
				popupActionInfo.Update();
			UpdateStyleCore();
		}
		ActionButton CreateActionButtonCore(IContentContainerAction action) {
			if(action is IContentContainerPopupMenuAction) return new PopupActionButton((IContentContainerPopupMenuAction)action);
			if(action is IContentContainerPopupControlAction) return new PopupActionButton((IContentContainerPopupControlAction)action);
			return new ActionButton(action);
		}
		protected internal IButton CreateActionButton(IContentContainer container, IContentContainerAction action) {
			ActionButton button = CreateActionButtonCore(action);
			ContentContainerAction.Args args = new ContentContainerAction.Args(action, container);
			button.Tag = new ContentContainerAction.Tag(Owner, args);
			button.Visible = ContentContainerAction.CanExecute(args);
			AppearanceHelper.Combine(button.Appearance, new AppearanceObject[] { 
				Owner.AppearanceActionsBarButton, Owner.AppearanceActionsBar }, Painter.DefaultAppearanceActionsButton);
			button.IsLeft = ContentContainerAction.GetActionLayout(action).Edge == ActionEdge.Left;
			button.Image = action.Image ?? Resources.ContentContainterActionResourceLoader.GetImage("Default");
			return button;
		}
		void OnActionButtonClick(object sender, ButtonEventArgs e) {
			ActionButton button = e.Button as ActionButton;
			if(button == null) return;
			PopupCancel();
			OnActionButtonClickCore(button);
		}
		void PopupCancel() {
			if(popupActionInfo == null || !popupActionInfo.IsShown) return;
			popupActionInfo.ProcessCancel();
			popupActionInfo.DetachContentControl();
		}
		void PopupShow(PopupActionButton button) {
			if(popupActionInfo == null) return;
			if(popupActionInfo.AttachActionButton(button)) {
				CalcPopupAction(button);
				popupActionInfo.ProcessShow(Point.Empty, true);
			}
		}
		void CalcPopupAction(ActionButton button) {
			using(IMeasureContext context = Owner.BeginMeasure()) {
				popupActionInfo.SetDirty();
				Size panelSize = popupActionInfo.CalcMinSize(context.Graphics);
				Point realLocation = Owner.Manager.ClientToScreen(CalcPopupActionLocation(button));
				popupActionInfo.Calc(context.Graphics, new Rectangle(realLocation, panelSize));
			}
		}
		Point CalcPopupActionLocation(ActionButton button) {
			if(ButtonsPanel.ViewInfo.Buttons == null) return Point.Empty;
			foreach(DevExpress.XtraEditors.ButtonPanel.BaseButtonInfo info in ButtonsPanel.ViewInfo.Buttons) {
				if(info.Button == button) {
					Rectangle bounds = info.Bounds;
					if(popupActionInfo.BeakLocation == BeakPanelBeakLocation.Top)
						return new Point(bounds.X + bounds.Width / 2, Bounds.Bottom);
					return new Point(bounds.X + bounds.Width / 2, Bounds.Top);
				}
			}
			return Point.Empty;
		}
		protected virtual void OnActionButtonClickCore(ActionButton button) {
			if(button is PopupActionButton) {
				PopupShow((PopupActionButton)button);
				return;
			}
			var tag = button.Tag as ContentContainerAction.Tag;
			if(tag == null) return;
			ContentContainerAction.Execute(tag.View, tag.Args);
			PerformBehavior(ContentContainerAction.GetActionBehavior(tag.Args.Action).Behavior);
		}
		void PerformBehavior(ActionBehavior behavior) {
			if(behavior == ActionBehavior.HideBarOnClick)
				Owner.HideNavigationAdorner();
			if(behavior == ActionBehavior.UpdateBarOnClick)
				Owner.UpdateNavigationBars();
		}
		void IInteractiveElementInfo.ProcessMouseDown(MouseEventArgs e) {
			PopupCancel();
			ButtonsPanel.Handler.OnMouseDown(e);
		}
		void IInteractiveElementInfo.ProcessMouseMove(MouseEventArgs e) {
			ButtonsPanel.Handler.OnMouseMove(e);
			Owner.SetCursor(ButtonsPanel.Handler.HotButton != null ? Cursors.Hand : null);
		}
		void IInteractiveElementInfo.ProcessMouseUp(MouseEventArgs e) {
			PopupCancel();
			ButtonsPanel.Handler.OnMouseUp(e);
		}
		void IInteractiveElementInfo.ProcessMouseLeave() {
			ButtonsPanel.Handler.OnMouseLeave();
		}
		void IInteractiveElementInfo.ProcessMouseWheel(MouseEventArgs e) { }
		bool IInteractiveElementInfo.ProcessFlick(Point point, DevExpress.Utils.Gesture.FlickGestureArgs args) { return false; }
		bool IInteractiveElementInfo.ProcessGesture(GestureID gid, DevExpress.Utils.Gesture.GestureArgs args, object[] parameters) { return false; }
		#endregion
		#region IButtonsPanelOwner Members
		object DevExpress.XtraEditors.ButtonPanel.IButtonsPanelOwner.Images {
			get { return Manager != null ? Manager.Images : null; }
		}
		ObjectPainter DevExpress.XtraEditors.ButtonPanel.IButtonsPanelOwner.GetPainter() {
			return buttonsPanelPainterCore;
		}
		bool DevExpress.XtraEditors.ButtonPanel.IButtonsPanelOwner.IsSelected {
			get { return false; }
		}
		void DevExpress.XtraEditors.ButtonPanel.IButtonsPanelOwner.Invalidate() {
			if(base.Owner.Manager != null && base.Owner.Manager.Adorner != null)
				base.Owner.Manager.Adorner.Invalidate();
		}
		#endregion
		#region IUIElement
		IUIElement IUIElement.Scope { get { return Owner; } }
		UIChildren IUIElement.Children {
			get { return null; }
		}
		#endregion IUIElement
		Rectangle prevBounds = Rectangle.Empty;
		public Size CalcMinSize(Graphics g) {
			Size buttonsSize = ButtonsPanel.ViewInfo.CalcMinSize(g);
			if(!buttonsSize.IsEmpty) {
				Rectangle r = Painter.GetActionsBarContentRectangle(new Rectangle(Point.Empty, new Size(100, 100)));
				return new Size(buttonsSize.Width + (100 - r.Width), buttonsSize.Height + (100 - r.Height));
			}
			return Size.Empty;
		}
		protected override void CalcCore(Graphics g, Rectangle bounds) {
			AppearanceHelper.Combine(PaintAppearance,
			 new AppearanceObject[] { Owner.AppearanceActionsBar }, Painter.DefaultAppearanceActionsBar);
			if(prevBounds != bounds)
				ButtonsPanel.ViewInfo.SetDirty();
			ButtonsPanel.ViewInfo.Calc(g, Painter.GetActionsBarContentRectangle(bounds));
			if(popupActionInfo != null && popupActionInfo.IsShown)
				CalcPopupAction(popupActionInfo.ActualButton);
			prevBounds = bounds;
		}
		public BaseContentContainerActionsBarInfoPainter Painter {
			get { return painterCore as BaseContentContainerActionsBarInfoPainter; }
		}
		protected override void DrawCore(GraphicsCache cache) {
			Painter.DrawObject(new BaseContentContainerActionsBarInfoArgs(cache, this));
			if(popupActionInfo == null) return;
			AppearanceHelper.Combine(popupActionInfo.AppearanceFlyoutPanel, new AppearanceObject[] { Owner.AppearanceActionsBar }, Painter.DefaultAppearanceActionsBar);
			popupActionInfo.Draw(cache);
		}
		protected override void UpdateStyleCore() {
			painterCore = ((WindowsUIViewPainter)Owner.Painter).GetPainter(this);
			buttonsPanelPainterCore = ((WindowsUIViewPainter)Owner.Painter).GetActionBarButtonsPanelPainter();
		}
		protected override void ResetStyleCore() {
			painterCore = null;
			buttonsPanelPainterCore = null;
		}
		#region IButtonsPanelOwner Members
		bool DevExpress.XtraEditors.ButtonPanel.IButtonsPanelOwner.AllowHtmlDraw {
			get { return false; }
		}
		bool DevExpress.XtraEditors.ButtonPanel.IButtonsPanelOwner.AllowGlyphSkinning {
			get { return false; }
		}
		DevExpress.XtraEditors.ButtonsPanelControl.ButtonsPanelControlAppearance DevExpress.XtraEditors.ButtonPanel.IButtonsPanelOwner.AppearanceButton {
			get { return null; }
		}
		object DevExpress.XtraEditors.ButtonPanel.IButtonsPanelOwner.ButtonBackgroundImages {
			get { return Owner.ActionButtonBackgroundImages; }
		}
		bool DevExpress.XtraEditors.ButtonPanel.IButtonsPanelOwner.Enabled {
			get { return true; }
		}
		#endregion
		#region IButtonsPanelOwnerEx Members
		Padding DevExpress.XtraEditors.ButtonPanel.IButtonsPanelOwnerEx.ButtonBackgroundImageMargin {
			get { return Padding.Empty; }
			set { }
		}
		bool DevExpress.XtraEditors.ButtonPanel.IButtonsPanelOwnerEx.CanPerformClick(XtraEditors.ButtonPanel.IBaseButton button) {
			return Owner.RaiseNavigationBarsButtonClick(button as DevExpress.XtraEditors.ButtonPanel.BaseButton);
		}
		#endregion
	}
	class BaseContentContainerActionsBarInfoArgs : ObjectInfoArgs {
		BaseContentContainerActionsBarInfo infoCore;
		public BaseContentContainerActionsBarInfoArgs(GraphicsCache cache, BaseContentContainerActionsBarInfo info)
			: base(cache) {
			this.infoCore = info;
		}
		public BaseContentContainerActionsBarInfo Info {
			get { return infoCore; }
		}
	}
	class BaseContentContainerActionsBarInfoPainter : ObjectPainter {
		AppearanceDefault appearanceDefaultActionsBarCore;
		AppearanceDefault appearanceDefaultActionsButtonCore;
		public BaseContentContainerActionsBarInfoPainter() {
			appearanceDefaultActionsBarCore = new AppearanceDefault(Color.White, Color.Black,
				WindowsUIViewPainter.DefaultFont);
			appearanceDefaultActionsButtonCore = new AppearanceDefault(Color.White, Color.Empty,
				WindowsUIViewPainter.DefaultFont);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			BaseContentContainerActionsBarInfoArgs args = e as BaseContentContainerActionsBarInfoArgs;
			DrawBackground(args.Cache, args.Info);
			DrawButtons(args.Cache, args.Info);
		}
		public void DrawActionsBar(GraphicsCache cache, BaseContentContainerActionsBarInfo info) {
			DrawBackground(cache, info);
			DrawButtons(cache, info);
		}
		protected virtual void DrawBackground(GraphicsCache cache, BaseContentContainerActionsBarInfo info) {
			info.PaintAppearance.DrawBackground(cache, info.Bounds);
		}
		protected virtual void DrawButtons(GraphicsCache cache, BaseContentContainerActionsBarInfo info) {
			ObjectPainter.DrawObject(cache, ((DevExpress.XtraEditors.ButtonPanel.IButtonsPanelOwner)info).GetPainter(), (ObjectInfoArgs)info.ButtonsPanel.ViewInfo);
		}
		public virtual AppearanceDefault DefaultAppearanceActionsBar {
			get { return appearanceDefaultActionsBarCore; }
		}
		public virtual AppearanceDefault DefaultAppearanceActionsButton {
			get { return appearanceDefaultActionsButtonCore; }
		}
		public virtual Rectangle GetActionsBarContentRectangle(Rectangle header) {
			return Rectangle.Inflate(header, -40, -18);
		}
	}
	class BaseContentContainerActionsBarInfoSkinPainter : BaseContentContainerActionsBarInfoPainter {
		ISkinProvider skinProvider;
		public BaseContentContainerActionsBarInfoSkinPainter(ISkinProvider provider) {
			this.skinProvider = provider;
		}
		public override AppearanceDefault DefaultAppearanceActionsBar {
			get {
				SkinElement barElement = GetActionsBarElement();
				if(barElement != null)
					barElement.Apply(base.DefaultAppearanceActionsBar);
				return base.DefaultAppearanceActionsBar;
			}
		}
		public override AppearanceDefault DefaultAppearanceActionsButton {
			get {
				SkinElement buttonElement = GetActionsBarButtonElement();
				if(buttonElement != null)
					buttonElement.Apply(base.DefaultAppearanceActionsButton);
				return base.DefaultAppearanceActionsButton;
			}
		}
		protected override void DrawBackground(GraphicsCache cache, BaseContentContainerActionsBarInfo info) {
			if(info.PaintAppearance.BackColor != DefaultAppearanceActionsBar.BackColor) {
				base.DrawBackground(cache, info);
				return;
			}
			SkinElement barElement = GetActionsBarElement();
			if(barElement != null)
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, new SkinElementInfo(barElement, info.Bounds));
			else base.DrawBackground(cache, info);
		}
		public override Rectangle GetActionsBarContentRectangle(Rectangle header) {
			SkinElement barElement = GetActionsBarElement();
			if(barElement != null)
				return barElement.ContentMargins.Deflate(header);
			return base.GetActionsBarContentRectangle(header);
		}
		protected virtual Skin GetSkin() {
			return MetroUISkins.GetSkin(skinProvider);
		}
		protected virtual SkinElement GetActionsBarElement() {
			return GetSkin()[MetroUISkins.SkinActionsBar];
		}
		protected virtual SkinElement GetActionsBarButtonElement() {
			return GetSkin()[MetroUISkins.SkinActionsBarButton];
		}
	}
}
