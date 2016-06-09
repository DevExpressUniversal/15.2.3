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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraBars.Docking2010.Customization {
	public class FlyoutAdornerElementInfoArgs : AdornerElementInfoArgs, IFlyoutAdorner, IFlyoutObjectInfoArgsOwner {
		WindowsUIView ownerCore;
		bool isDisposing;
		FlyoutObjectInfoArgs infoArgsCore;
		public WindowsUIView View {
			get { return ownerCore; }
		}
		internal FlyoutObjectInfoArgs InfoArgs {
			get { return infoArgsCore; }
		}
		public FlyoutAdornerElementInfoArgs(WindowsUIView owner) {
			ownerCore = owner;
			infoArgsCore = CreateFlyoutObjectInfoArgs();
		}
		protected virtual FlyoutObjectInfoArgs CreateFlyoutObjectInfoArgs() {
			return new FlyoutObjectInfoArgs(this);
		}
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				Ref.Dispose(ref infoArgsCore);
				flyoutDialogFormCore = null;
				flyoutContainerCore = null;
			}
		}
		void OnButtonsPanelChanged(object sender, EventArgs e) {
			if(View != null)
				View.LayoutChanged();
		}
		public DialogResult? Result { get; set; }
		public bool IsOwnControl(Control control) {
			Control parent = null;
			if(FlyoutContainer == null || FlyoutContainer.Document == null)
				return false;
			parent = View.Manager.GetChild(FlyoutContainer.Document);
			return DocumentsHostContext.IsChild(control, parent);
		}
		public void RaiseShown() {
			if(View != null)
				View.RaiseFlyoutShown();
		}
		public void RaiseHidden() {
			if(View != null)
				View.RaiseFlyoutHidden(Result.HasValue ? Result.Value : DialogResult.None);
			Result = null;
		}
		static DevExpress.Utils.DXMouseEventArgs GetArgs(Point pt) {
			return GetArgs(Control.MouseButtons, pt);
		}
		static DevExpress.Utils.DXMouseEventArgs GetArgs(MouseButtons buttons, Point pt) {
			return new DevExpress.Utils.DXMouseEventArgs(buttons, 0, pt.X, pt.Y, 0);
		}
		MouseEventArgs hitArgs;
		public AdornerHitTest HitTest(Point screenPoint, bool isOwnControl) {
			Point clientPoint = View.Manager.ScreenToClient(screenPoint);
			hitArgs = GetArgs(clientPoint);
			if(InfoArgs.ControlBounds.Contains(clientPoint) || isOwnControl)
				return AdornerHitTest.Control;
			if(InfoArgs.FlyoutBounds.Contains(clientPoint))
				return AdornerHitTest.Adorner;
			return AdornerHitTest.None;
		}
		public void HitTestFlyoutForm(Point screenPoint) {
			hitArgs = GetArgs(screenPoint);
		}
		AdornerHitTest IBaseAdorner.HitTest(Point screenPoint) {
			return HitTest(screenPoint, false);
		}
		public void OnMouseDown(Point screenPoint) {
			if(InfoArgs.ButtonsPanel != null)
				InfoArgs.ButtonsPanel.Handler.OnMouseDown(hitArgs);
		}
		public void OnMouseMove(Point screenPoint) {
			if(InfoArgs.ButtonsPanel != null) {
				InfoArgs.ButtonsPanel.Handler.OnMouseMove(hitArgs);
			}
		}
		public void OnMouseUp(Point screenPoint) {
			if(InfoArgs.ButtonsPanel != null)
				InfoArgs.ButtonsPanel.Handler.OnMouseUp(GetArgs(MouseButtons.Left, hitArgs.Location));
		}
		public void OnMouseLeave() {
			if(InfoArgs.ButtonsPanel != null)
				InfoArgs.ButtonsPanel.Handler.OnMouseLeave();
		}
		public void Update() {
			if(!IsAttachedToDialogForm) Bounds = View.Bounds;
			View.Manager.Adorner.UpdateTransparentLayerSize();
			SetDirty();
			View.Manager.Adorner.Invalidate();
		}
		public bool Show() {
			this.flyoutDialogFormCore = null;
			View.SetCursor(null);
			Bounds = View.Bounds;
			flyoutContainerCore = null;
			if(Bounds.Height > 0 || Bounds.Height > 0) {
				View.Manager.Adorner.hWndInsertAfter = new System.IntPtr(-2);
				View.Manager.Adorner.Show(View.flyoutInfo);
				View.Manager.Adorner.TransparentLayer.TransparentForEvents = false;
				Control control = GetControlForCapture();
				if(control != null && FlyoutContainer != null && FlyoutContainer.Document == null) control.Capture = true;
				SetDirty();
				View.Manager.Adorner.Invalidate();
				return true;
			}
			return false;
		}
		protected bool IsAttachedToDialogForm {
			get { return flyoutDialogFormCore != null; }
		}
		FlyoutDialogForm flyoutDialogFormCore;
		protected virtual FlyoutDialogForm FlyoutDialogForm {
			get { return flyoutDialogFormCore; }
		}
		Flyout savedflyoutContainer;
		public void AttachToFlyoutDialogForm(FlyoutDialogForm form) {
			this.flyoutDialogFormCore = form;
			savedflyoutContainer = flyoutContainerCore;
			flyoutContainerCore = form.Flyout;
			Bounds = new Rectangle(Point.Empty, form.Bounds.Size);
			InfoArgs.ControlBounds = InfoArgs.FlyoutBounds = Rectangle.Empty;
			SetDirty();
		}
		public void DetachFromFlyoutDialogForm() {
			flyoutContainerCore = savedflyoutContainer;
			Bounds = View.Bounds;
			InfoArgs.ControlBounds = InfoArgs.FlyoutBounds = Rectangle.Empty;
			SetDirty();
			this.flyoutDialogFormCore = null;
		}
		public void Hide() {
			HideCore();
		}
		public void Cancel() {
			HideCore();
		}
		protected void HideCore() {
			if(View == null || View.Manager == null) return;
			View.SetCursor(null);
			if(View.Manager.Adorner != null) {
				View.Manager.Adorner.hWndInsertAfter = System.IntPtr.Zero;
				View.Manager.Adorner.TransparentLayer.TransparentForEvents = true;
				View.Manager.Adorner.Reset(View.flyoutInfo);
				View.Manager.Adorner.Clear();
			}
			ResetBounds();
			savedflyoutContainer = null;
			Control control = GetControlForCapture();
			if(control != null) {
				control.Location = new Point(-control.Width, -control.Height);
				control.Capture = false;
			}
		}
		protected Control GetControlForCapture() {
			Control result = null;
			if(FlyoutContainer != null && FlyoutContainer.Document != null && FlyoutContainer.Document.IsControlLoaded)
				result = View.Manager.GetChild(FlyoutContainer.Document);
			if(result == null)
				result = View.Manager.GetOwnerControl();
			return result;
		}
		void ResetBounds() {
			InfoArgs.ControlBounds = Rectangle.Empty;
			InfoArgs.FlyoutBounds = Rectangle.Empty;
		}
		Flyout flyoutContainerCore;
		public Flyout FlyoutContainer {
			get {
				if(flyoutContainerCore != null)
					return flyoutContainerCore;
				return ownerCore != null ? ownerCore.ActiveFlyoutContainer as Flyout : null;
			}
		}
		public ObjectPainter GetFlyoutPainter() {
			return ((WindowsUIViewPainter)View.Painter).GetFlyoutPainter();
		}
		protected override int CalcCore() {
			using(IMeasureContext context = View.BeginMeasure()) {
				InitInfoArgs();
				InfoArgs.Calc(context.Graphics);
				return -1;
			}
		}
		void InitInfoArgs() {
			if(FlyoutContainer == null) return;
			InfoArgs.Action = FlyoutContainer.Action ?? FlyoutContainer.LoadedAction;
			InfoArgs.Control = FlyoutContainer.Document != null ? FlyoutContainer.Document.Control : null;
			InfoArgs.Properties = FlyoutContainer.Properties;
			InfoArgs.Buttons = FlyoutContainer.FlyoutButtons;
			InfoArgs.Bounds = Bounds;
			InfoArgs.Caption = FlyoutContainer.Caption;
			InfoArgs.Description = FlyoutContainer.Subtitle;
			InfoArgs.Image = FlyoutContainer.Image;
			InfoArgs.SetDirty();
		}
		protected override IEnumerable<Rectangle> GetRegionsCore(bool opaque) {
			return InfoArgs.GetRegionsCore(opaque);
		}
		public static FlyoutAdornerElementInfoArgs EnsureInfoArgs(ref AdornerElementInfo target, WindowsUIView owner) {
			FlyoutAdornerElementInfoArgs args;
			if(target == null) {
				args = new FlyoutAdornerElementInfoArgs(owner);
				target = new AdornerElementInfo(new FlyoutAdornerPainter(), new FlyoutAdornerOpaquePainter(), args);
			}
			else args = target.InfoArgs as FlyoutAdornerElementInfoArgs;
			args.SetDirty();
			return args;
		}
		FlyoutStyle IFlyoutAdorner.Style {
			get { return FlyoutContainer.Properties.ActualStyle; }
		}
		#region IFlyoutObjectInfoArgsOwner Members
		void IFlyoutObjectInfoArgsOwner.Invalidate(Rectangle bounds) {
			if(IsAttachedToDialogForm) {
				FlyoutDialogForm.Invalidate(bounds);
				return;
			}
			if(View.Manager != null && View.Manager.Adorner != null)
				View.Manager.Adorner.InvalidateOpaqueLayer();
		}
		void IFlyoutObjectInfoArgsOwner.Invalidate() {
			if(IsAttachedToDialogForm) {
				FlyoutDialogForm.Invalidate();
				return;
			}
			if(View.Manager != null && View.Manager.Adorner != null)
				View.Manager.Adorner.InvalidateOpaqueLayer();
		}
		void IFlyoutObjectInfoArgsOwner.LayoutChanged() {
			if(View != null)
				View.LayoutChanged();
		}
		bool IFlyoutObjectInfoArgsOwner.CanExecuteCommand(FlyoutCommand flyoutCommand) {
			return flyoutCommand.CanExecute(FlyoutContainer);
		}
		void IFlyoutObjectInfoArgsOwner.ExecuteCommand(FlyoutCommand flyoutCommand) {
			if(IsAttachedToDialogForm) {
				FlyoutDialogForm.DialogResult = flyoutCommand.Result;
				DetachFromFlyoutDialogForm();
				return;
			}
			if(flyoutCommand.CanExecute(FlyoutContainer)) {
				flyoutCommand.Execute(FlyoutContainer);
				Result = flyoutCommand.Result;
				View.HideFlyout();
			}
		}
		Rectangle IFlyoutObjectInfoArgsOwner.GetControlBounds() {
			if(FlyoutContainer.Document != null && !IsAttachedToDialogForm)
				return View.Manager.GetChild(FlyoutContainer.Document).Bounds;
			return Rectangle.Empty;
		}
		#endregion
	}
	public class FlyoutAdornerPainter : AdornerPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			FlyoutAdornerElementInfoArgs info = e as FlyoutAdornerElementInfoArgs;
			if(info.FlyoutContainer.Properties.ActualStyle == FlyoutStyle.MessageBox)
				DrawFlyoutTransparentLayer(e.Cache, info);
		}
		static SolidBrush BackBrush = new SolidBrush(Color.FromArgb(50, Color.Black));
		protected void DrawFlyoutTransparentLayer(GraphicsCache cache, FlyoutAdornerElementInfoArgs info) {
			Rectangle bar = new Rectangle(0, 0, info.Bounds.Width, info.Bounds.Height);
			if(bar.Height == 0 || bar.Width == 0) return;
			using(Bitmap barImage = new Bitmap(bar.Width, bar.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb)) {
				Point clientOffset = new Point(info.Bounds.X, info.Bounds.Y);
				using(Graphics g = Graphics.FromImage(barImage)) {
					using(XtraBufferedGraphics bg = XtraBufferedGraphicsManager.Current.Allocate(g, bar)) {
						bg.Graphics.TranslateTransform(-info.Bounds.X, -info.Bounds.Y);
						using(GraphicsCache bufferedCache = new GraphicsCache(bg.Graphics)) {
							bufferedCache.Graphics.FillRectangle(BackBrush, info.Bounds);
						}
						bg.Render();
					}
				}
				cache.Graphics.DrawImageUnscaled(barImage, clientOffset);
			}
		}
	}
	public class FlyoutAdornerOpaquePainter : AdornerOpaquePainter {
		public override void DrawObject(ObjectInfoArgs e) {
			FlyoutObjectInfoArgs info = (e as FlyoutAdornerElementInfoArgs).InfoArgs;
			DrawFlyoutContent(e.Cache, info);
		}
		protected void DrawFlyoutContent(GraphicsCache cache, FlyoutObjectInfoArgs info) {
			Rectangle bar = new Rectangle(0, 0, info.FlyoutBounds.Width, info.FlyoutBounds.Height);
			if(bar.Height == 0 || bar.Width == 0) return;
			using(Bitmap barImage = new Bitmap(bar.Width, bar.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb)) {
				Point clientOffset = new Point(info.FlyoutBounds.X, info.FlyoutBounds.Y);
				using(Graphics g = Graphics.FromImage(barImage)) {
					using(XtraBufferedGraphics bg = XtraBufferedGraphicsManager.Current.Allocate(g, bar)) {
						bg.Graphics.TranslateTransform(-info.FlyoutBounds.X, -info.FlyoutBounds.Y);
						using(GraphicsCache bufferedCache = new GraphicsCache(bg.Graphics)) {
							ObjectPainter.DrawObject(bufferedCache, info.GetFlyoutPainter(), info);
						}
						bg.Render();
					}
				}
				cache.Graphics.DrawImageUnscaled(barImage, clientOffset);
			}
		}
	}
	public class FlyoutPainter : ObjectPainter {
		static Font DefaultCaptionFont = SegoeUIFontsCache.GetSegoeUILightFont(12f);
		static Font DefaultDescriptionFont = SegoeUIFontsCache.GetSegoeUIFont(4f);
		static Font DefaultButtonFont = SegoeUIFontsCache.GetSegoeUIFont(4f);
		public Color ParentBackColor { get; set; }
		public Color ParentForeColor { get; set; }
		AppearanceDefault defaultAppearance;
		public sealed override AppearanceDefault DefaultAppearance {
			get {
				if(defaultAppearance == null)
					defaultAppearance = CreateDefaultAppearance();
				return defaultAppearance;
			}
		}
		AppearanceDefault defaultAppearanceCaption;
		public AppearanceDefault DefaultAppearanceCaption {
			get {
				if(defaultAppearanceCaption == null)
					defaultAppearanceCaption = CreateDefaultAppearanceCaption();
				return defaultAppearanceCaption;
			}
		}
		AppearanceDefault defaultAppearanceLoadingDescription;
		public AppearanceDefault DefaultAppearanceDescription {
			get {
				if(defaultAppearanceLoadingDescription == null)
					defaultAppearanceLoadingDescription = CreateDefaultAppearanceDescription();
				return defaultAppearanceLoadingDescription;
			}
		}
		AppearanceDefault defaultAppearanceButton;
		public AppearanceDefault DefaultAppearanceButtons {
			get {
				if(defaultAppearanceButton == null)
					defaultAppearanceButton = CreateDefaultAppearanceButton();
				return defaultAppearanceButton;
			}
		}
		protected virtual AppearanceDefault CreateDefaultAppearance() {
			return new AppearanceDefault(ParentForeColor.IsEmpty ? SystemColors.ControlText : ParentForeColor, ParentBackColor);
		}
		protected virtual AppearanceDefault CreateDefaultAppearanceCaption() {
			return new AppearanceDefault(ParentForeColor.IsEmpty ? SystemColors.ControlText : ParentForeColor, ParentBackColor, DefaultCaptionFont);
		}
		protected virtual AppearanceDefault CreateDefaultAppearanceDescription() {
			return new AppearanceDefault(ParentForeColor.IsEmpty ? SystemColors.ControlText : ParentForeColor, ParentBackColor, DefaultDescriptionFont);
		}
		protected virtual AppearanceDefault CreateDefaultAppearanceButton() {
			return new AppearanceDefault(ParentForeColor.IsEmpty ? SystemColors.ControlText : ParentForeColor, ParentBackColor, DefaultButtonFont);
		}
		public sealed override void DrawObject(ObjectInfoArgs e) {
			FlyoutObjectInfoArgs args = e as FlyoutObjectInfoArgs;
			DrawBackgroud(args);
			DrawContent(args);
		}
		protected virtual void DrawContent(FlyoutObjectInfoArgs args) {
			DrawCaption(args);
			DrawDescription(args);
			DrawButtons(args);
		}
		protected virtual void DrawCaption(FlyoutObjectInfoArgs args) {
			if(args.CaptionBounds.Size.IsEmpty) return;
			if(args.CaptionInfo != null)
				StringPainter.Default.DrawString(args.Cache, args.PaintAppearanceCaption, args.Caption, args.CaptionBounds);
			else
				args.PaintAppearanceCaption.DrawString(args.Cache, args.Caption, args.CaptionBounds);
		}
		protected virtual void DrawButtons(FlyoutObjectInfoArgs args) {
			if(!args.ButtonBounds.Size.IsEmpty) {
				ObjectPainter.DrawObject(args.Cache, args.GetPainter(), args.ButtonsPanel.ViewInfo as ObjectInfoArgs);
			}
		}
		protected virtual void DrawDescription(FlyoutObjectInfoArgs args) {
			if(!args.ImageBounds.Size.IsEmpty)
				args.Cache.Graphics.DrawImage(args.Image, args.ImageBounds);
			if(args.DescriptionBounds.Size.IsEmpty) return;
			if(args.DescriptionInfo != null)
				StringPainter.Default.DrawString(args.Cache, args.PaintAppearanceDescription, args.Description, args.DescriptionBounds);
			else
				args.PaintAppearanceDescription.DrawString(args.Cache, args.Description, args.DescriptionBounds);
		}
		protected virtual void DrawBackgroud(FlyoutObjectInfoArgs args) {
			if(!args.FlyoutBounds.Size.IsEmpty)
				args.PaintAppearance.DrawBackground(args.Cache, args.FlyoutBounds);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			client.Inflate(10, 10);
			return client;
		}
	}
	public class FlyoutSkinPainter : FlyoutPainter {
		ISkinProvider provider;
		public FlyoutSkinPainter(ISkinProvider provider) {
			this.provider = provider;
		}
		protected override AppearanceDefault CreateDefaultAppearanceCaption() {
			AppearanceDefault appearance = base.CreateDefaultAppearanceCaption();
			ApplyColors(appearance);
			return appearance;
		}
		protected void ApplyColors(AppearanceDefault appearance) {
			Color backgroudColor = LookAndFeelHelper.GetSystemColorEx(provider, SystemColors.Control);
			Color foreColor = LookAndFeelHelper.GetSystemColorEx(provider, SystemColors.ControlText);
			appearance.ForeColor = ParentForeColor.IsEmpty ? foreColor : ParentForeColor;
			appearance.BackColor = ParentBackColor.IsEmpty ? backgroudColor : ParentBackColor;
		}
		protected override AppearanceDefault CreateDefaultAppearanceDescription() {
			AppearanceDefault appearance = base.CreateDefaultAppearanceDescription();
			ApplyColors(appearance);
			return appearance;
		}
		protected override AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault appearance = base.CreateDefaultAppearance();
			ApplyColors(appearance);
			return appearance;
		}
		protected override AppearanceDefault CreateDefaultAppearanceButton() {
			AppearanceDefault appearance = base.CreateDefaultAppearanceButton();
			ApplyColors(appearance);
			return appearance;
		}
		protected virtual Skin GetSkin() {
			return MetroUISkins.GetSkin(provider);
		}
		protected virtual SkinElement GetFlyout() {
			return GetSkin()[MetroUISkins.SkinActionsBar];
		}
	}
	public interface IFlyoutObjectInfoArgsOwner {
		void Invalidate();
		void Invalidate(Rectangle bounds);
		void LayoutChanged();
		bool CanExecuteCommand(FlyoutCommand flyoutCommand);
		void ExecuteCommand(FlyoutCommand flyoutCommand);
		ObjectPainter GetFlyoutPainter();
		Rectangle GetControlBounds();
	}
	public class FlyoutObjectInfoArgs : ObjectInfoArgs, IButtonsPanelOwner, IDisposable {
		ButtonsPanel buttonsPanelCore;
		bool isDisposing;
		public FlyoutObjectInfoArgs(IFlyoutObjectInfoArgsOwner onwer) {
			Owner = onwer;
			buttonsPanelCore = CreateButtonsPanel();
			ButtonsPanel.ButtonInterval = 10;
			ButtonsPanel.Changed += OnButtonsPanelChanged;
			ButtonsPanel.ButtonClick += OnButtonClick;
		}
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				ButtonsPanel.Changed -= OnButtonsPanelChanged;
				ButtonsPanel.ButtonClick -= OnButtonClick;
				Ref.Dispose(ref buttonsPanelCore);
			}
		}
		public Rectangle ControlBounds { get; set; }
		void OnButtonClick(object sender, ButtonEventArgs e) {
			FlyoutButton button = e.Button as FlyoutButton;
			if(button == null) return;
			Owner.ExecuteCommand(button.Command);
		}
		public IFlyoutDefaultProperties Properties { get; set; }
		public FlyoutAction Action { get; set; }
		public Control Control { get; set; }
		public ObjectPainter GetFlyoutPainter() {
			return Owner.GetFlyoutPainter();
		}
		StringInfo descriptionInfoCore;
		public StringInfo DescriptionInfo {
			get { return descriptionInfoCore; }
			set { descriptionInfoCore = value; }
		}
		public IEnumerable<FlyoutCommand> Commands {
			get { return Action != null ? Action.Commands : null; }
		}
		protected IEnumerable<FlyoutCommand> GetActualCommands() {
			if(Commands == null || Owner == null) return null;
			return System.Linq.Enumerable.Where(Commands, fc => Owner.CanExecuteCommand(fc));
		}
		const int DefaultInterval = 20;
		public MessageBoxButtons? Buttons { get; set; }
		public void Calc(Graphics g) {
			if(IsReady) return;
			if(Action == null && Control == null) return;
			if(Control != null)
				ControlBounds = Owner.GetControlBounds();
			if(Action == null) {
				FlyoutBounds = ControlBounds;
			}
			FlyoutPainter painter = GetFlyoutPainter() as FlyoutPainter;
			InitPaintApearances(painter);
			EnsureButtons(Properties.ActualButtonSize, Buttons, GetActualCommands());
			Caption = GetFlyoutCaption();
			Description = GetFlyoutDescription();
			Image = GetFlyoutImage();
			bool hasCaption = !string.IsNullOrEmpty(Caption);
			bool hasDescription = (!string.IsNullOrEmpty(Description) || Image != null) && ControlBounds.IsEmpty;
			bool hasButtons = ButtonsPanel.Buttons.Count > 0;
			bool hasImage = Image != null && ControlBounds.IsEmpty;
			Size captionSize = hasCaption ? CalcTextSize(g, PaintAppearanceCaption, Caption, ref captionInfoCore) : Size.Empty;
			Size descriptionSize = hasDescription ? CalcTextSize(g, PaintAppearanceDescription, Description, ref descriptionInfoCore) : Size.Empty;
			Size buttonSize = ButtonsPanel.ViewInfo.CalcMinSize(g);
			Size imageSize = hasImage ? Image.Size : Size.Empty;
			int buttonToDescriptionInterval = hasButtons && (hasDescription || hasCaption) ? DefaultInterval : 0;
			int captionToDescriptionInterval = hasCaption && hasDescription ? DefaultInterval : 0;
			int imageToDescriptionInterval = hasCaption && hasDescription ? DefaultInterval : 0;
			int contentHeight = captionSize.Height;
			contentHeight += Math.Max(descriptionSize.Height, imageSize.Height);
			contentHeight += contentHeight > 0 && (hasDescription || hasImage) ? captionToDescriptionInterval : 0;
			contentHeight += contentHeight >= 0 && buttonSize.Height > 0 ? buttonSize.Height + buttonToDescriptionInterval : 0;
			int descriptionWidth = hasImage && hasDescription ? DefaultInterval : 0;
			descriptionWidth += descriptionSize.Width + imageSize.Width;
			int contentWidth = Math.Max(captionSize.Width, Math.Max(descriptionWidth, buttonSize.Width));
			Size contentSize = new Size(contentWidth, contentHeight);
			if(contentSize.IsEmpty && ControlBounds.IsEmpty) return;
			Content = ControlBounds.IsEmpty ? PlacementHelper.Arrange(contentSize, Bounds, ContentAlignment.MiddleCenter) :
				new Rectangle(ControlBounds.X, ControlBounds.Y - captionSize.Height, ControlBounds.Width, ControlBounds.Height + contentHeight);
			ContentAlignment textAlign = ContentAlignment.TopLeft;
			Rectangle clientRect = CalcClientRectangle();
			if(PaintAppearanceCaption.HAlignment == HorzAlignment.Center)
				textAlign = ContentAlignment.TopCenter;
			else if(PaintAppearanceCaption.HAlignment == HorzAlignment.Far)
				textAlign = ContentAlignment.TopRight;
			CaptionBounds = PlacementHelper.Arrange(captionSize, clientRect, textAlign);
			if(buttonSize.Width < clientRect.Width)
				ButtonBounds = PlacementHelper.Arrange(buttonSize, clientRect, ContentAlignment.BottomRight);
			else
				ButtonBounds = PlacementHelper.Arrange(buttonSize, clientRect, ContentAlignment.BottomCenter);
			ButtonsPanel.ViewInfo.SetDirty();
			ButtonsPanel.ViewInfo.Calc(g, ButtonBounds);
			Rectangle imageAndDescriptionRect = new Rectangle(CaptionBounds.X, CaptionBounds.Bottom + DefaultInterval, descriptionWidth, Math.Max(descriptionSize.Height, imageSize.Height));
			ImageBounds = PlacementHelper.Arrange(imageSize, imageAndDescriptionRect, ContentAlignment.MiddleLeft);
			DescriptionBounds = PlacementHelper.Arrange(descriptionSize, imageAndDescriptionRect, ContentAlignment.MiddleRight);
			if(hasCaption || hasButtons || hasDescription) {
				flyoutBoundsCore = painter.CalcBoundsByClientRectangle(this, Content);
			}
			else if(Action != null && Control != null)
				flyoutBoundsCore = Content;
			if(Properties.ActualStyle == FlyoutStyle.MessageBox) {
				flyoutBoundsCore.Width = Bounds.Width;
				flyoutBoundsCore.X = Bounds.X;
			}
			isReadyCore = true;
		}
		Rectangle CalcClientRectangle() {
			if(Control != null && Control.Padding != Padding.Empty)
				return new Rectangle(Content.Left + Control.Padding.Left, Content.Top, Content.Width - Control.Padding.Horizontal, Content.Height);
			return Content;
		}
		protected virtual string GetFlyoutCaption() {
			return (Action != null) && !string.IsNullOrEmpty(Action.Caption) ? Action.Caption : Caption;
		}
		protected virtual string GetFlyoutDescription() {
			return (Action != null) && !string.IsNullOrEmpty(Action.Description) ? Action.Description : Description;
		}
		protected virtual Image GetFlyoutImage() {
			return (Action != null) && (Action.Image != null) ? Action.Image : Image;
		}
		protected Size CalcTextSize(Graphics graphics, AppearanceObject appearance, string text, ref StringInfo info) {
			if(Properties.CanHtmlDraw) {
				info = StringPainter.Default.Calculate(graphics, appearance, text, Bounds.Width);
				return info.Bounds.Size;
			}
			else
				return Size.Round(appearance.CalcTextSize(graphics, text, Bounds.Width));
		}
		protected void InitPaintApearances(FlyoutPainter painter) {
			PaintAppearance = new FrozenAppearance();
			PaintAppearanceCaption = new FrozenAppearance();
			PaintAppearanceDescription = new FrozenAppearance();
			PaintAppearanceButtons = new FrozenAppearance();
			if(!ControlBounds.IsEmpty) {
				painter.ParentBackColor = Properties.ActualAppearance.BackColor.IsEmpty ? Control.BackColor : Properties.ActualAppearance.BackColor;
				painter.ParentForeColor = Properties.ActualAppearance.ForeColor.IsEmpty ? Control.ForeColor : Properties.ActualAppearance.ForeColor;
			}
			else {
				painter.ParentBackColor = Properties.ActualAppearance.BackColor;
				painter.ParentForeColor = Properties.ActualAppearance.ForeColor;
			}
			AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { Properties.ActualAppearance }, painter.DefaultAppearance);
			AppearanceHelper.Combine(PaintAppearanceCaption, new AppearanceObject[] { Properties.ActualAppearanceCaption }, painter.DefaultAppearanceCaption);
			AppearanceHelper.Combine(PaintAppearanceDescription, new AppearanceObject[] { Properties.ActualAppearanceDescription }, painter.DefaultAppearanceDescription);
			AppearanceHelper.Combine(PaintAppearanceButtons, new AppearanceObject[] { Properties.ActualAppearanceButtons }, painter.DefaultAppearanceButtons);
			PaintAppearanceButtons.TextOptions.HotkeyPrefix = PaintAppearanceButtons.TextOptions.HotkeyPrefix == HKeyPrefix.Default ? HKeyPrefix.Hide : PaintAppearanceButtons.TextOptions.HotkeyPrefix;
		}
		public IEnumerable<Rectangle> GetRegionsCore(bool opaque) {
			Rectangle controlRect;
			if(opaque)
				controlRect = ControlBounds;
			else
				controlRect = FlyoutBounds;
			Rectangle leftRect = new Rectangle(Bounds.Location, new Size(controlRect.X - Bounds.X, Bounds.Height));
			Rectangle topRect = new Rectangle(new Point(leftRect.Right, Bounds.Y), new Size(controlRect.Width, controlRect.Y - Bounds.Y));
			Rectangle bottomRect = new Rectangle(new Point(leftRect.Right, controlRect.Bottom), new Size(controlRect.Width, Bounds.Bottom - controlRect.Bottom));
			Rectangle rightRect = new Rectangle(new Point(controlRect.Right, Bounds.Y), new Size(Bounds.Right - controlRect.Right, Bounds.Height));
			return new Rectangle[] { leftRect, topRect, bottomRect, rightRect };
		}
		void OnButtonsPanelChanged(object sender, EventArgs e) {
			Owner.LayoutChanged();
		}
		protected virtual ButtonsPanel CreateButtonsPanel() {
			return new FlyoutButtonsPanel(this);
		}
		public Image Image { get; set; }
		public string Caption { get; set; }
		StringInfo captionInfoCore;
		public StringInfo CaptionInfo {
			get { return captionInfoCore; }
			set { captionInfoCore = value; }
		}
		public string Description { get; set; }
		public IFlyoutObjectInfoArgsOwner Owner { get; set; }
		public AppearanceObject PaintAppearance { get; set; }
		public AppearanceObject PaintAppearanceCaption { get; set; }
		public AppearanceObject PaintAppearanceDescription { get; set; }
		public AppearanceObject PaintAppearanceButtons { get; set; }
		public Rectangle CaptionBounds { get; set; }
		public Rectangle DescriptionBounds { get; set; }
		public Rectangle ImageBounds { get; set; }
		public Rectangle ButtonBounds { get; set; }
		public Rectangle Content { get; set; }
		Rectangle flyoutBoundsCore;
		public Rectangle FlyoutBounds {
			get { return flyoutBoundsCore; }
			set { flyoutBoundsCore = value; ; }
		}
		public ButtonsPanel ButtonsPanel {
			get { return buttonsPanelCore; }
		}
		protected void EnsureButtons(Size buttonSize, MessageBoxButtons? buttons, IEnumerable<FlyoutCommand> commands) {
			ButtonsPanel.BeginUpdate();
			if(!buttonSize.IsEmpty)
				(ButtonsPanel as FlyoutButtonsPanel).ButtonSize = buttonSize;
			ButtonsPanel.Buttons.Clear();
			if(ButtonsPanel.ViewInfo != null)
				ButtonsPanel.ViewInfo.SetDirty();
			if(buttons.HasValue) {
				switch(buttons.Value) {
					case MessageBoxButtons.OK:
						ButtonsPanel.Buttons.Add(new FlyoutButton(FlyoutCommand.OK));
						break;
					case MessageBoxButtons.AbortRetryIgnore:
						ButtonsPanel.Buttons.AddRange(new IBaseButton[]{ 
							new FlyoutButton(FlyoutCommand.Abort),
							new FlyoutButton(FlyoutCommand.Retry),
							new FlyoutButton(FlyoutCommand.Ignore)});
						break;
					case MessageBoxButtons.OKCancel:
						ButtonsPanel.Buttons.AddRange(new IBaseButton[]{ 
							new FlyoutButton(FlyoutCommand.OK),
							new FlyoutButton(FlyoutCommand.Cancel)});
						break;
					case MessageBoxButtons.RetryCancel:
						ButtonsPanel.Buttons.AddRange(new IBaseButton[]{ 
							 new FlyoutButton(FlyoutCommand.Retry),
							 new FlyoutButton(FlyoutCommand.Cancel)});
						break;
					case MessageBoxButtons.YesNo:
						ButtonsPanel.Buttons.AddRange(new IBaseButton[]{ 
							new FlyoutButton(FlyoutCommand.Yes),
							new FlyoutButton(FlyoutCommand.No)});
						break;
					case MessageBoxButtons.YesNoCancel:
						ButtonsPanel.Buttons.AddRange(new IBaseButton[]{ 
							new FlyoutButton(FlyoutCommand.Yes),
							new FlyoutButton(FlyoutCommand.No),
							new FlyoutButton(FlyoutCommand.Cancel)});
						break;
				}
			}
			if(commands != null)
				foreach(FlyoutCommand command in commands)
					ButtonsPanel.Buttons.Add(new FlyoutButton(command));
			AppearanceObject appearance = new FrozenAppearance();
			ButtonsPanel.Buttons.ForEach(new Action<IBaseButton>((button) => button.Properties.Appearance.AssignInternal(PaintAppearanceButtons)));
			ButtonsPanel.CancelUpdate();
		}
		#region IButtonsPanelOwner Members
		ObjectPainter buttonsPanelPainter;
		public ObjectPainter GetPainter() {
			if(buttonsPanelPainter == null)
				buttonsPanelPainter = new FlyoutButtonsPanelSkinPainter();
			return buttonsPanelPainter;
		}
		public object Images {
			get { return null; }
		}
		public void Invalidate(Rectangle bounds) {
			Owner.Invalidate(bounds);
		}
		public void Invalidate() {
			Owner.Invalidate();
		}
		public bool IsSelected {
			get { return false; }
		}
		bool IButtonsPanelOwner.AllowHtmlDraw {
			get { return false; }
		}
		bool IButtonsPanelOwner.AllowGlyphSkinning {
			get { return false; }
		}
		DevExpress.XtraEditors.ButtonsPanelControl.ButtonsPanelControlAppearance IButtonsPanelOwner.AppearanceButton {
			get { return null; }
		}
		object IButtonsPanelOwner.ButtonBackgroundImages {
			get { return null; }
		}
		bool IButtonsPanelOwner.Enabled {
			get { return true; }
		}
		#endregion
		public void SetDirty() {
			isReadyCore = false;
		}
		bool isReadyCore;
		public bool IsReady {
			get { return isReadyCore; }
		}
	}
}
