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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Skins.XtraForm;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Mdi;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Docking.Helpers;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraBars.Docking2010 {
	public abstract class BaseFloatDocumentForm : XtraEditors.MouseWheelContainerForm, IFloatDocumentFormInfoOwner, IButtonsPanelOwner, DevExpress.XtraEditors.ICustomDrawNonClientArea {
		FloatDocumentFormInfo infoCore;
		ButtonsPanel buttonsPanelCore;
		public BaseFloatDocumentForm() {
			this.ShowIcon = false;
			this.MinimumSize = DockConsts.DefaultFloatFormMinSize;
			this.ShowInTaskbar = false;
			this.FormBorderStyle = FormBorderStyle.None;
			this.StartPosition = FormStartPosition.Manual;
			SetStyle(ControlStyles.Selectable, false);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			DoubleBuffered = true;
			infoCore = CreateInfo();
			this.buttonsPanelCore = CreateButtonsPanel();
			ButtonsPanel.Buttons.AddRange(new IButton[] { 
				new CloseButton(), new PinButton(), new MaximizeButton() });
			SubscribeEmbeddedButtonPanel();
		}
		bool isDisposingCore;
		public bool IsDisposing {
			get { return isDisposingCore; }
		}
		protected sealed override void Dispose(bool disposing) {
			if(disposing) {
				if(!isDisposingCore) {
					isDisposingCore = true;
					Redraw.Lock(this);
					UnsubscribeEmbeddedButtonPanel();
					OnDispose();
					Redraw.UnLock(this);
				}
			}
			base.Dispose(disposing);
		}
		protected virtual void OnDispose() {
			Ref.Dispose(ref infoCore);
			Ref.Dispose(ref buttonsPanelCore);
		}
		protected virtual FloatDocumentFormInfo CreateInfo() {
			return new FloatDocumentFormInfo(this);
		}
		protected internal abstract DocumentManager Manager { get; }
		public FloatDocumentFormInfo Info {
			get { return infoCore; }
		}
		public ButtonsPanel ButtonsPanel {
			get { return buttonsPanelCore; }
		}
		protected abstract Image Image { get; }
		protected abstract string Caption { get; }
		protected virtual ButtonsPanel CreateButtonsPanel() {
			return new ButtonsPanel(this);
		}
		BaseDocument IFloatDocumentFormInfoOwner.Document {
			get { return GetDocument(); }
		}
		protected virtual BaseDocument GetDocument() {
			return null;
		}
		protected void SubscribeEmbeddedButtonPanel() {
			ButtonsPanel.Changed += EmbeddedButtonPanelChanged;
			ButtonsPanel.ButtonClick += OnDefaultButtonClick;
			ButtonsPanel.ButtonChecked += OnDefaultButtonClick;
			ButtonsPanel.ButtonUnchecked += OnDefaultButtonClick;
		}
		protected void UnsubscribeEmbeddedButtonPanel() {
			ButtonsPanel.Changed -= EmbeddedButtonPanelChanged;
			ButtonsPanel.ButtonClick -= OnDefaultButtonClick;
			ButtonsPanel.ButtonChecked -= OnDefaultButtonClick;
			ButtonsPanel.ButtonUnchecked -= OnDefaultButtonClick;
		}
		void OnDefaultButtonClick(object sender, ButtonEventArgs e) {
			IButton button = e.Button;
			if(!(button is DefaultButton)) return;
			if(button is CloseButton)
				OnButtonClick(FormCaptionButtonAction.Close);
			if(button is MaximizeButton)
				OnButtonClick(button.Properties.Checked ? FormCaptionButtonAction.Maximize : FormCaptionButtonAction.Restore);
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			bool callBaseSetBoundsCore = true;
			StackTrace st = new StackTrace(System.Threading.Thread.CurrentThread, false);
			if(st != null) {
				StackFrame sf = st.GetFrame(2);
				if(sf != null && sf.GetMethod().Name == "RestoreWindowBoundsIfNecessary")
					if(Info != null && Info.bounds.Width > 0 && Info.bounds.Height > 0) {
						callBaseSetBoundsCore = false;
						base.SetBoundsCore(x, y, Info.bounds.Width, Info.bounds.Height, specified);
					}
			}
			if(callBaseSetBoundsCore)
				base.SetBoundsCore(x, y, width, height, specified);
		}
		void EmbeddedButtonPanelChanged(object sender, EventArgs e) {
			InvalidateNC();
		}
		bool isRestoring = false;
		protected override void WndProc(ref Message m) {
			switch(m.Msg) {
				case 0xA3: 
					CheckMaximizedBounds();
					base.WndProc(ref m);
					break;
				case MSG.WM_SYSCOMMAND:
					var flag = WinAPIHelper.GetInt(m.WParam);
					var sc_flag = (flag & 0xFFF0);
					if(sc_flag == NativeMethods.SC.SC_MAXIMIZE)
						if(!OnSCMaximize()) {
							m.Result = IntPtr.Zero;
							break;
						}
					var isMovingMaximized = (flag == 0xf012) && WindowState == FormWindowState.Maximized;
					using(isMovingMaximized ? CreateDragMoveContext() : null) {
						isRestoring = (sc_flag == NativeMethods.SC.SC_RESTORE);
						base.WndProc(ref m);
					}
					break;
				case MSG.WM_NCMOUSELEAVE:
					DoNCMouseLeave();
					base.WndProc(ref m);
					break;
				case MSG.WM_NCMOUSEMOVE:
					DoNCMouseMove(WinAPIHelper.GetMouseArgs(Handle, m));
					base.WndProc(ref m);
					break;
				case MSG.WM_NCLBUTTONDOWN:
					DoNCMouseDown(WinAPIHelper.GetMouseArgs(Handle, m));
					base.WndProc(ref m);
					break;
				case MSG.WM_LBUTTONUP:
					DoMouseUp(WinAPIHelper.GetClientMouseArgs(Handle, m));
					base.WndProc(ref m);
					break;
				case MSG.WM_NCRBUTTONUP:
					DoNCRMouseUp(WinAPIHelper.GetMouseArgs(Handle, m));
					base.WndProc(ref m);
					break;
				case MSG.WM_NCLBUTTONUP:
					DoMouseUp(WinAPIHelper.GetMouseArgs(Handle, m));
					base.WndProc(ref m);
					break;
				case MSG.WM_NCCALCSIZE:
					DoNCCalcSize(ref m);
					break;
				case MSG.WM_NCHITTEST:
					DoNCHitTest(ref m);
					break;
				case MSG.WM_NCPAINT:
					DoNCPaint(ref m);
					break;
				case MSG.WM_NCACTIVATE:
					DoNCActivate(ref m);
					break;
				case MSG.WM_ERASEBKGND:
					m.Result = new IntPtr(1);
					break;
				case MSG.WM_WINDOWPOSCHANGING:
					OnWindowPosChanging(m.LParam);
					base.WndProc(ref m);
					break;
				case MSG.WM_WINDOWPOSCHANGED:
					OnWindowPosChanged(m.LParam);
					base.WndProc(ref m);
					break;
				case MSG.WM_GETMINMAXINFO:
					ProcessGetMinMaxInfo(ref m);
					base.WndProc(ref m);
					break;
				case MSG.WM_SHOWWINDOW:
					if(m.LParam == new IntPtr(1) && WindowState == FormWindowState.Maximized) {
						parentRestoring = true;
					}
					base.WndProc(ref m);
					break;
				default:
					base.WndProc(ref m);
					break;
			}
		}
		IFloatDocumentFormDragMoveContext CreateDragMoveContext() {
			return FloatDocumentFormDragMoveContext.Create(this);
		}
		internal static IFloatDocumentFormDragMoveContext GetDragMoveContext(Form form) {
			return FloatDocumentFormDragMoveContext.Get(form as BaseFloatDocumentForm);
		}
		void ProcessGetMinMaxInfo(ref Message msg) {
			if(msg.LParam == IntPtr.Zero) return;
			if(parentRestoring) {
				BeginInvoke(new MethodInvoker(() =>
				{
					WindowState = FormWindowState.Maximized;
					parentRestoring = false;
					InvalidateNC();
				}));
			}
		}
		bool parentRestoring;
		const int SWP_STATECHANGED = 0x8000;
		protected virtual void OnWindowPosChanging(IntPtr param) {
			WinAPI.WINDOWPOS pos = (WinAPI.WINDOWPOS)BarNativeMethods.PtrToStructure(param, typeof(WinAPI.WINDOWPOS));
			if((pos.flags & SWP_STATECHANGED) == SWP_STATECHANGED) {
				var context = GetDragMoveContext(this);
				if(context != null && !context.StateChangedWhenMovingMaximized)
					context.StateChangedWhenMovingMaximized = true;
			}
		}
		protected virtual void OnWindowPosChanged(IntPtr param) { }
		protected void UpdateRegion() {
			if(Info.bounds != Rectangle.Empty) {
				Region = new Region(Info.bounds);
			}
		}
		protected virtual bool Borderless {
			get { return false; }
		}
		protected virtual void DoNCMouseLeave() {
			if(ButtonsPanel != null) {
				ButtonsPanel.Handler.OnMouseLeave();
				InvalidateNC();
			}
		}
		protected virtual void DoNCRMouseUp(MouseEventArgs e) {
			Info.OnContextMenu(e);
		}
		protected virtual void DoNCMouseDown(MouseEventArgs e) {
			if(ButtonsPanel != null) {
				MouseEventArgs es = new MouseEventArgs(MouseButtons.Left, e.Clicks, e.X, e.Y, e.Delta);
				ButtonsPanel.Handler.OnMouseDown(es);
				if(ButtonsPanel.Handler.PressedButton != null)
					NativeMethods.SetCapture(Handle);
			}
		}
		protected virtual void DoNCMouseMove(MouseEventArgs e) {
			if(ButtonsPanel != null)
				ButtonsPanel.Handler.OnMouseMove(e);
		}
		protected virtual void DoMouseUp(MouseEventArgs e) {
			NativeMethods.ReleaseCapture();
			if(ButtonsPanel != null) {
				MouseEventArgs es = new MouseEventArgs(MouseButtons.Left, e.Clicks, e.X, e.Y, e.Delta);
				ButtonsPanel.Handler.OnMouseUp(es);
			}
		}
		protected virtual void DoNCCalcSize(ref Message m) {
			if(m.WParam == IntPtr.Zero) {
				WinAPI.RECT nccsRect = (WinAPI.RECT)m.GetLParam(typeof(WinAPI.RECT));
				Rectangle patchedRectangle = CalculateNC(nccsRect.ToRectangle());
				nccsRect.RestoreFromRectangle(patchedRectangle);
				BarNativeMethods.StructureToPtr(nccsRect, m.LParam, false);
				m.Result = IntPtr.Zero;
			}
			else {
				WinAPI.NCCALCSIZE_PARAMS nccsParams = (WinAPI.NCCALCSIZE_PARAMS)m.GetLParam(typeof(WinAPI.NCCALCSIZE_PARAMS));
				Rectangle bounds = nccsParams.rgrcProposed.ToRectangle();
				Rectangle patchedRectangle = CalculateNC(bounds);
				nccsParams.rgrcProposed.RestoreFromRectangle(patchedRectangle);
				BarNativeMethods.StructureToPtr(nccsParams, m.LParam, false);
				m.Result = IntPtr.Zero;
			}
			if(ShouldCallWndProc())
				base.WndProc(ref m);
		}
		protected virtual bool ShouldCallWndProc() {
			return false;
		}
		protected virtual void DoNCPaint(ref Message m) {
			if(!Borderless)
				DrawNC(ref m);
			m.Result = IntPtr.Zero;
		}
		protected virtual void DoNCHitTest(ref Message m) {
			int result = NativeMethods.HT.HTTRANSPARENT;
			if(!Borderless) {
				Point pt = WinAPIHelper.PointToFormBounds(Handle, WinAPIHelper.GetPoint(m.LParam));
				result = Info.HitTest(pt);
			}
			m.Result = new IntPtr(result);
		}
		public void UpdateStyle() {
			painterCore = null;
			if(ButtonsPanel != null && ButtonsPanel.Buttons != null) {
				ButtonsPanel.UpdateStyle();
			}
			InvalidateNC();
		}
		protected internal void InvalidateNC() {
			if(IsHandleCreated)
				FormPainter.InvalidateNC(Handle);
		}
		protected virtual void DoNCActivate(ref Message m) {
			bool isActive = (m.WParam != IntPtr.Zero);
			if(!IsDisposing && !Borderless) {
				Info.IsActive = isActive;
				InvalidateNC();
			}
			m.Result = new IntPtr(1);
		}
		protected override void OnActivated(EventArgs e) {
			base.OnActivated(e);
			if(Info != null) Info.IsActive = true;
		}
		FloatDocumentFormPainter painterCore;
		protected FloatDocumentFormPainter Painter {
			get {
				if(painterCore == null && !IsDisposing)
					painterCore = CreatePainter();
				return painterCore;
			}
		}
		protected virtual void DrawNC(ref Message m) {
			IntPtr hDC = FormPainter.GetDC(Handle, m);
			try {
				if(!IsDisposing && Info.IsReady) {
					Painter.Draw(hDC, Info);
				}
			}
			finally { NativeMethods.ReleaseDC(Handle, hDC); }
		}
		protected FloatDocumentFormPainter CreatePainter() {
			if(Manager.PaintStyleName == "Skin")
				return new ColoredFloatDocumentFormSkinPainter(Manager.LookAndFeel); 
			return new FloatDocumentFormPainter();
		}
		public override Color BackColor {
			get { return GetBackColor(base.BackColor); }
			set { base.BackColor = value; }
		}
		protected virtual Color GetBackColor(Color baseColor) {
			if(IsDisposing || Manager == null) return baseColor;
			return Painter.GetBackColor(baseColor);
		}
		protected virtual Rectangle CalculateNC(Rectangle rect) {
			if(IsDisposed || Manager == null) return rect;
			Info.Text = Caption;
			Info.Image = Image;
			Info.AllowGlyphSkinning = CanUseGlyphSkinning;
			Info.ShowCloseButton = HasCloseButton;
			Info.ShowMaximizeButton = HasMaximizeButton;
			bool maximized = WindowState == FormWindowState.Maximized && !isRestoring;
			isRestoring = false;
			return Info.CalculateNC(rect, Painter, Borderless || (maximized && IsMdiChild), maximized);
		}
		protected virtual void OnButtonClick(FormCaptionButtonAction kind) {
			int cmd = -1;
			switch(kind) {
				case FormCaptionButtonAction.Close: cmd = NativeMethods.SC.SC_CLOSE; break;
				case FormCaptionButtonAction.Maximize: cmd = NativeMethods.SC.SC_MAXIMIZE; break;
				case FormCaptionButtonAction.Restore: cmd = NativeMethods.SC.SC_RESTORE; break;
			}
			if(cmd != -1) {
				DevExpress.Skins.XtraForm.FormPainter.PostSysCommand(this, Handle, cmd);
				BeginInvoke(new MethodInvoker(InvalidateNC));
			}
		}
		protected virtual bool OnSCMaximize() {
			CheckMaximizedBounds();
			return true;
		}
		protected internal void CheckMaximizedBounds() {
			Screen currentScreen = Screen.FromPoint(Location);
			Point actualWorkingAreaLocation = Point.Empty;
			actualWorkingAreaLocation.X = Math.Max(currentScreen.WorkingArea.X - currentScreen.Bounds.X, 0);
			actualWorkingAreaLocation.Y = Math.Max(currentScreen.WorkingArea.Y - currentScreen.Bounds.Y, 0);
			MaximizedBounds = new Rectangle(actualWorkingAreaLocation, currentScreen.WorkingArea.Size);
			if(IsHandleCreated)
				BeginInvoke(new MethodInvoker(InvalidateNC));
		}
		protected virtual bool CanUseGlyphSkinning {
			get { return false; }
		}
		protected virtual bool HasCloseButton {
			get { return !Borderless; }
		}
		protected virtual bool HasMaximizeButton {
			get { return !Borderless; }
		}
		#region IFloatDocumentFormInfoOwner
		BarAndDockingController IFloatDocumentFormInfoOwner.GetController() {
			return Manager.GetBarAndDockingController();
		}
		void IFloatDocumentFormInfoOwner.InvalidateNC() {
			InvalidateNC();
		}
		void IFloatDocumentFormInfoOwner.SetCapture() {
			NativeMethods.SetCapture(Handle);
		}
		void IFloatDocumentFormInfoOwner.ButtonClick(FormCaptionButtonAction formCaptionButtonAction) {
			OnButtonClick(formCaptionButtonAction);
		}
		void IFloatDocumentFormInfoOwner.ShowContextMenu(Point pt) {
			OnShowContextMenu(pt);
		}
		protected abstract void OnShowContextMenu(Point pt);
		#endregion
		#region IButtonsPanelOwner Members
		object IButtonsPanelOwner.Images {
			get { return Manager != null ? Manager.Images : null; }
		}
		ObjectPainter IButtonsPanelOwner.GetPainter() {
			if(Manager.PaintStyleName == "Skin")
				return new ButtonsPanelSkinPainter(Manager.LookAndFeel);
			return new ButtonPanelOffice2000Painter();
		}
		bool IButtonsPanelOwner.IsSelected {
			get { return Info != null ? Info.IsActive : false; }
		}
		void IButtonsPanelOwner.Invalidate() {
			if(Manager != null)
				InvalidateNC();
		}
		#endregion
		#region IButtonsPanelOwner Members
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
		#endregion
		#region ICustomDrawNonClientArea Members
		bool DevExpress.XtraEditors.ICustomDrawNonClientArea.IsCustomDrawNonClientArea {
			get { return true; }
		}
		#endregion
		#region IFloatDocumentFormDragMoveContext
		class FloatDocumentFormDragMoveContext : IFloatDocumentFormDragMoveContext {
			static IDictionary<BaseFloatDocumentForm, IFloatDocumentFormDragMoveContext> contexts = new Dictionary<BaseFloatDocumentForm, IFloatDocumentFormDragMoveContext>();
			BaseFloatDocumentForm form;
			FloatDocumentFormDragMoveContext(BaseFloatDocumentForm form) {
				this.form = form;
				if(form != null)
					contexts.Add(form, this);
			}
			void IDisposable.Dispose() {
				if(queuedAction != null)
					queuedAction();
				this.queuedAction = null;
				if(form != null)
					contexts.Remove(form);
				this.form = null;
			}
			public bool StateChangedWhenMovingMaximized { get; set; }
			Action queuedAction;
			public void Queue(Action endDragging) {
				if(StateChangedWhenMovingMaximized)
					this.queuedAction = endDragging;
				else
					endDragging();
			}
			internal static IFloatDocumentFormDragMoveContext Create(BaseFloatDocumentForm form) {
				return Get(form) ?? new FloatDocumentFormDragMoveContext(form);
			}
			internal static IFloatDocumentFormDragMoveContext Get(BaseFloatDocumentForm form) {
				IFloatDocumentFormDragMoveContext context;
				return (form != null) && contexts.TryGetValue(form, out context) ? context : null;
			}
		}
		#endregion
	}
	[System.ComponentModel.Browsable(false)]
	public class FloatDocumentFormInfo : IDisposable {
		IFloatDocumentFormInfoOwner ownerCore;
		public string Text;
		public Image Image;
		public bool IsActive;
		public bool ShowCloseButton;
		public bool ShowPinButton;
		public bool ShowMaximizeButton;
		public bool AllowGlyphSkinning;
		public Rectangle bounds, caption, border, client;
		public Rectangle captionText, captionImage;
		public Rectangle controlBox;
		protected Rectangle topLeft, top, topRight, right, bottomRight, bottom, bottomLeft, left;
		Padding captionMargins;
		public FloatDocumentFormInfo(IFloatDocumentFormInfoOwner owner) {
			ownerCore = owner;
			PaintAppearanceCaption = new FrozenAppearance();
		}
		bool isDisposingCore;
		public void Dispose() {
			if(!isDisposingCore) {
				isDisposingCore = true;
				OnDispose();
				ownerCore = null;
			}
			GC.SuppressFinalize(this);
		}
		protected virtual void OnDispose() {
			Ref.Dispose(ref PaintAppearanceCaption);
		}
		public IFloatDocumentFormInfoOwner Owner {
			get { return ownerCore; }
		}
		protected Docking.DockElementsParameters Parameters {
			get { return Owner.GetController().PaintStyle.ElementsParameters; }
		}
		protected Docking.DockManagerAppearances AppearancesSettings {
			get { return Owner.GetController().AppearancesDocking; }
		}
		protected Docking.Paint.DockElementsPainter Painter {
			get { return Owner.GetController().PaintStyle.ElementsPainter; }
		}
		public Size GetNCMinSize() {
			return new Size(controlBox.Width + captionMargins.Horizontal +
				(captionImage.Width == 0 ? captionImage.Width + 2 : 0),
				bounds.Height - client.Height);
		}
		public bool NCHitTest(Point clientPoint) {
			Point pt = new Point(client.Left + clientPoint.X, client.Y + clientPoint.Y);
			return bounds.Contains(pt) && !client.Contains(pt);
		}
		public int HitTest(Point pt) {
			return HitTestCore(pt);
		}
		protected virtual int HitTestCore(Point pt) {
			int result = NativeMethods.HT.HTTRANSPARENT;
			if(caption.Contains(pt)) result = NativeMethods.HT.HTCAPTION;
			if(client.Contains(pt)) result = NativeMethods.HT.HTCLIENT;
			if(topLeft.Contains(pt)) result = NativeMethods.HT.HTTOPLEFT;
			if(top.Contains(pt)) result = NativeMethods.HT.HTTOP;
			if(topRight.Contains(pt)) result = NativeMethods.HT.HTTOPRIGHT;
			if(right.Contains(pt)) result = NativeMethods.HT.HTRIGHT;
			if(bottomRight.Contains(pt)) result = NativeMethods.HT.HTBOTTOMRIGHT;
			if(bottom.Contains(pt)) result = NativeMethods.HT.HTBOTTOM;
			if(bottomLeft.Contains(pt)) result = NativeMethods.HT.HTBOTTOMLEFT;
			if(left.Contains(pt)) result = NativeMethods.HT.HTLEFT;
			if(controlBox.Contains(pt)) result = NativeMethods.HT.HTOBJECT;
			return result;
		}
		public virtual void OnContextMenu(MouseEventArgs e) {
			if(caption.Contains(e.Location)) {
				if(!controlBox.Contains(e.Location)) {
					Point pt = new Point(e.X + bounds.X - client.X, e.Y + bounds.Y - client.Y);
					Owner.ShowContextMenu(pt);
				}
			}
		}
		public bool IsReady { get; private set; }
		public virtual Rectangle CalculateNC(Rectangle rect, FloatDocumentFormPainter painter, bool borderless, bool maximized) {
			if(!borderless)
				UpdatePaintAppearanceCaption(painter);
			Padding bm = !borderless ? painter.BorderMargin : Padding.Empty;
			Padding cm = !borderless ? painter.CaptionMargin : Padding.Empty;
			bounds = new Rectangle(0, 0, rect.Width, rect.Height);
			Rectangle borderInside = new Rectangle(bm.Left, cm.Top,
				rect.Width - bm.Horizontal,
				rect.Height - cm.Top - bm.Bottom);
			if(!maximized) {
				topLeft = new Rectangle(bounds.Left, bounds.Top, bm.Left, cm.Top);
				top = new Rectangle(borderInside.Left, bounds.Top, borderInside.Width, cm.Top);
				topRight = new Rectangle(borderInside.Right, bounds.Top, bm.Right, cm.Top);
				right = new Rectangle(borderInside.Right, borderInside.Top, bm.Right, borderInside.Height);
				bottomRight = new Rectangle(borderInside.Right, borderInside.Bottom, bm.Right, bm.Bottom);
				bottom = new Rectangle(borderInside.Left, borderInside.Bottom, borderInside.Width, bm.Bottom);
				bottomLeft = new Rectangle(bounds.Left, borderInside.Bottom, bm.Left, bm.Bottom);
				left = new Rectangle(bounds.Left, borderInside.Top, bm.Left, borderInside.Height);
			}
			else topLeft = top = topRight = right = bottomRight = bottom = bottomLeft = left = Rectangle.Empty;
			int captionHeight = borderless ? 0 : CalcCaptionHeight(maximized);
			if(painter.IsSkinPainter) {
				caption = new Rectangle(bounds.Left, bounds.Top, bounds.Width, captionHeight);
				border = new Rectangle(0, captionHeight, rect.Width, rect.Height - captionHeight);
				client = new Rectangle(bm.Left, caption.Bottom + bm.Top,
					border.Width - bm.Horizontal, border.Height - bm.Vertical);
			}
			else {
				int m = borderless ? 0 : 2;
				caption = new Rectangle(bounds.Left + m, bounds.Top + m, bounds.Width - m * 2, captionHeight);
				border = new Rectangle(m, captionHeight + m, rect.Width - m * 2, rect.Height - captionHeight - m * 2);
				client = new Rectangle(m, caption.Bottom, border.Width, border.Height);
			}
			Rectangle captionClientBounds = Painter.WindowPainter.GetCaptionClientBounds(caption, true);
			int pos = captionClientBounds.Right - Painter.WindowPainter.CaptionButtonInterval;
			captionMargins = new Padding(0);
			if(!borderless) {
				controlBox = Rectangle.Empty;
				if(Owner != null && Owner.ButtonsPanel != null) {
					SubscribeDefaultButton(maximized);
					Graphics g = Painter.AddGraphics(null);
					try {
						Rectangle buttons = new Rectangle(
							captionClientBounds.X,
							captionClientBounds.Top,
							pos - captionClientBounds.X,
							captionClientBounds.Height);
						Owner.ButtonsPanel.ViewInfo.SetDirty();
						Owner.ButtonsPanel.ViewInfo.Calc(g, buttons);
						pos = GetNextPos(Owner.ButtonsPanel.ViewInfo.Bounds);
					}
					finally { Painter.ReleaseGraphics(); }
					captionMargins = new Padding(
						captionClientBounds.Left - caption.Left,
						caption.Right - captionClientBounds.Right,
						captionClientBounds.Top - caption.Top,
						captionClientBounds.Bottom - captionClientBounds.Bottom);
					controlBox = Owner.ButtonsPanel.ViewInfo.Bounds;
				}
			}
			captionText = borderless ? Rectangle.Empty : CalcCaptionTextBounds(captionClientBounds, pos);
			captionImage = borderless ? Rectangle.Empty : CalcCaptionImageBounds(captionClientBounds, pos);
			IsReady = true;
			return new Rectangle(rect.Left + client.Left, rect.Top + client.Top, client.Width, client.Height);
		}
		void SubscribeDefaultButton(bool maximized) {
			Owner.ButtonsPanel.BeginUpdate();
			MergeCustomButtons(Owner.ButtonsPanel);
			IBaseButton pinButton = Owner.ButtonsPanel.Buttons[1];
			IBaseButton closeButton = Owner.ButtonsPanel.Buttons[0];
			IBaseButton maximizeButton = Owner.ButtonsPanel.Buttons[2];
			pinButton.Properties.LockCheckEvent();
			pinButton.Properties.Visible = false;
			pinButton.Properties.UnlockCheckEvent();
			maximizeButton.Properties.LockCheckEvent();
			maximizeButton.Properties.Visible = ShowMaximizeButton;
			maximizeButton.Properties.Checked = maximized;
			maximizeButton.Properties.UnlockCheckEvent();
			closeButton.Properties.Visible = ShowCloseButton;
			Owner.ButtonsPanel.CancelUpdate();
		}
		protected virtual void MergeCustomButtons(ButtonsPanel ownerPanel) { }
		int GetNextPos(Rectangle bounds) {
			return bounds.Left - Painter.WindowPainter.CaptionButtonInterval;
		}
		Rectangle GetNextRect(Rectangle bounds, int pos) {
			Size btnSize = Painter.ButtonSize;
			return new Rectangle(pos - btnSize.Width, bounds.Top + (bounds.Height - btnSize.Height) / 2,
				btnSize.Width, btnSize.Height);
		}
		Rectangle CalcCaptionTextBounds(Rectangle bounds, int pos) {
			int interval = Painter.WindowPainter.CaptionButtonInterval;
			Rectangle result = bounds;
			result.X += 2 * interval + CaptionImageSize.Width;
			result.Width = pos - bounds.Left - interval - CaptionImageSize.Width;
			return result;
		}
		protected Size CaptionImageSize {
			get {
				if(Image != null)
					return Image.Size;
				return Size.Empty;
			}
		}
		Rectangle CalcCaptionImageBounds(Rectangle bounds, int pos) {
			Rectangle result = bounds;
			result.X += Painter.WindowPainter.CaptionButtonInterval;
			result.Y += (bounds.Height - CaptionImageSize.Height) / 2;
			result.Size = CaptionImageSize;
			if(pos - result.X < CaptionImageSize.Width)
				result.Size = Size.Empty;
			return result;
		}
		protected internal virtual int CalcCaptionHeight(bool maximized) {
			int buttonsPanelHeight = 0;
			if(Owner != null && Owner.ButtonsPanel != null) {
				SubscribeDefaultButton(maximized);
				Graphics g = Painter.AddGraphics(null);
				try {
					buttonsPanelHeight = Owner.ButtonsPanel.ViewInfo.CalcMinSize(g).Height;
					var painter = Painter.WindowPainter as DevExpress.XtraBars.Docking.Paint.WindowSkinPainter;
					if(painter != null)
						buttonsPanelHeight = painter.GetCaptionBoundsByClientRectangle(new Rectangle(0, 0, 0, buttonsPanelHeight + 2 * painter.CaptionVertIndent), true).Height;
				}
				finally { Painter.ReleaseGraphics(); }
			}
			return Math.Max(Painter.GetCaptionHeight(PaintAppearanceCaption, true), buttonsPanelHeight);
		}
		protected internal virtual Docking.Paint.CaptionButtonStatus GetApplicationButtonStatus(bool active) {
			return (active ? Docking.Paint.CaptionButtonStatus.ActiveApplicationButton :
				Docking.Paint.CaptionButtonStatus.InactiveApplicationButton);
		}
		public AppearanceObject PaintAppearanceCaption;
		public virtual void UpdatePaintAppearanceCaption(FloatDocumentFormPainter painter) {
			BaseDocument document = Owner.Document;
			if(document != null && document.Manager != null) {
				UpdateActualCaptionAppearance(document, document.Manager.View);
				Parameters.InitApplicationCaptionAppearance(PaintAppearanceCaption,
					IsActive ? actualActiveCaptionAppearanceCore : actualCaptionAppearanceCore, IsActive);
				PaintAppearanceCaption.BorderColor = IsActive ? actualActiveCaptionAppearanceCore.BackColor : actualCaptionAppearanceCore.BackColor;
			}
			else {
				Parameters.InitApplicationCaptionAppearance(PaintAppearanceCaption,
					IsActive ? AppearancesSettings.FloatFormCaptionActive : AppearancesSettings.FloatFormCaption, IsActive);
			}
			if(!IsActive && PaintAppearanceCaption.ForeColor.IsEmpty)
				PaintAppearanceCaption.ForeColor = painter.DefaultAppearance.ForeColor;
		}
		void UpdateActualCaptionAppearance(BaseDocument localAppearanceOwner, BaseView globalAppearanceOwner) {
			IDocumentCaptionAppearanceProvider localAppearanceProvider = localAppearanceOwner as IDocumentCaptionAppearanceProvider;
			IDocumentCaptionAppearanceProvider globalAppearanceProvider = globalAppearanceOwner as IDocumentCaptionAppearanceProvider;
			AppearanceHelper.Combine(actualCaptionAppearanceCore, new AppearanceObject[] { localAppearanceProvider.CaptionAppearance, globalAppearanceProvider.CaptionAppearance, AppearancesSettings.FloatFormCaption });
			AppearanceHelper.Combine(actualActiveCaptionAppearanceCore, new AppearanceObject[] { localAppearanceProvider.ActiveCaptionAppearance, globalAppearanceProvider.ActiveCaptionAppearance, AppearancesSettings.FloatFormCaptionActive });
			AppearanceHelper.Combine(PaintAppearanceCaption, new AppearanceObject[] { PaintAppearanceCaption, localAppearanceProvider.CaptionAppearance });
			allowCaptionColorBlendingCore = localAppearanceProvider.AllowCaptionColorBlending;
			allowDocumentsCaptionColorBlendingCore = globalAppearanceProvider.AllowCaptionColorBlending;
		}
		bool allowCaptionColorBlendingCore;
		bool allowDocumentsCaptionColorBlendingCore;
		protected internal bool AllowDocumentsCaptionColorBlending {
			get { return allowDocumentsCaptionColorBlendingCore; }
		}
		protected internal bool AllowCaptionColorBlending {
			get { return allowCaptionColorBlendingCore; }
		}
		FrozenAppearance actualCaptionAppearanceCore = new FrozenAppearance();
		FrozenAppearance actualActiveCaptionAppearanceCore = new FrozenAppearance();
	}
	[System.ComponentModel.Browsable(false)]
	public class FloatDocumentFormPainter {
		AppearanceDefault defaultAppearance;
		public virtual AppearanceDefault DefaultAppearance {
			get {
				if(defaultAppearance == null)
					defaultAppearance = CreateDefaultAppearance();
				return defaultAppearance;
			}
		}
		protected virtual AppearanceDefault CreateDefaultAppearance() {
			return new AppearanceDefault();
		}
		AppearanceDefault defaultAppearanceActive;
		public virtual AppearanceDefault DefaultAppearanceActive {
			get {
				if(defaultAppearanceActive == null)
					defaultAppearanceActive = CreateDefaultAppearanceActive();
				return defaultAppearanceActive;
			}
		}
		protected virtual AppearanceDefault CreateDefaultAppearanceActive() {
			return new AppearanceDefault();
		}
		[System.Security.SecuritySafeCritical]
		public virtual void Draw(IntPtr hDC, FloatDocumentFormInfo info) {
			NativeMethods.ExcludeClipRect(hDC, info.client.Left, info.client.Top, info.client.Right, info.client.Bottom);
			if(info.bounds.Width == 0 || info.bounds.Height == 0) return;
			using(XtraBufferedGraphics bg = XtraBufferedGraphicsManager.Current.Allocate(hDC, info.bounds)) {
				DrawCore(bg, info);
			}
		}
		public void Draw(Graphics g, FloatDocumentFormInfo info) {
			g.ExcludeClip(info.client);
			using(XtraBufferedGraphics bg = XtraBufferedGraphicsManager.Current.Allocate(g, info.bounds)) {
				DrawCore(bg, info);
			}
		}
		void DrawCore(XtraBufferedGraphics bg, FloatDocumentFormInfo info) {
			bg.Graphics.ExcludeClip(info.client);
			using(GraphicsCache cache = new GraphicsCache(bg.Graphics)) {
				DrawCaption(cache, info);
				DrawBorder(cache, info);
			}
			bg.Render();
		}
		protected virtual void DrawCaption(GraphicsCache cache, FloatDocumentFormInfo info) {
			DrawCaptionBackground(cache, info);
			DrawCaptionImage(cache, info);
			DrawCaptionText(cache, info);
			DrawButtons(cache, info);
		}
		protected virtual void DrawCaptionBackground(GraphicsCache cache, FloatDocumentFormInfo info) {
			info.PaintAppearanceCaption.DrawBackground(cache, info.caption);
		}
		protected virtual void DrawButtons(GraphicsCache cache, FloatDocumentFormInfo info) {
			if(info.Owner is IButtonsPanelOwner)
				ObjectPainter.DrawObject(cache, ((IButtonsPanelOwner)info.Owner).GetPainter(),
					(ObjectInfoArgs)info.Owner.ButtonsPanel.ViewInfo);
		}
		protected virtual void DrawCaptionImage(GraphicsCache cache, FloatDocumentFormInfo info) {
			if(info.Image != null && info.captionImage.Size != Size.Empty) {
				if(info.AllowGlyphSkinning) {
					var attributes = ImageColorizer.GetColoredAttributes(info.PaintAppearanceCaption.GetForeColor());
					cache.Graphics.DrawImage(info.Image, info.captionImage, 0, 0, info.captionImage.Width, info.captionImage.Height,
						GraphicsUnit.Pixel, attributes);
				}
				else cache.Graphics.DrawImage(info.Image, info.captionImage);
			}
		}
		protected virtual void DrawCaptionText(GraphicsCache cache, FloatDocumentFormInfo info) {
			info.PaintAppearanceCaption.DrawString(cache, info.Text, info.captionText,
				info.PaintAppearanceCaption.GetStringFormat(TextOptions.DefaultOptionsNoWrapEx));
		}
		protected virtual void DrawBorder(GraphicsCache cache, FloatDocumentFormInfo info) {
			cache.DrawRectangle(info.IsActive ? SystemPens.ActiveBorder : SystemPens.InactiveBorder, info.bounds);
			cache.DrawRectangle(SystemPens.Window, Rectangle.Inflate(info.bounds, -1, -1));
		}
		public virtual Padding CaptionMargin {
			get { return new Padding(2, 2, 2, 0); }
		}
		public virtual Padding BorderMargin {
			get { return new Padding(2, 0, 2, 2); }
		}
		public virtual Color GetBackColor(Color baseColor) {
			return baseColor.IsEmpty ? SystemColors.Window : baseColor;
		}
		public virtual Color GetForeColor(Color baseColor) {
			return baseColor.IsEmpty ? SystemColors.Control : baseColor;
		}
		public virtual bool IsSkinPainter { get { return false; } }
	}
	[System.ComponentModel.Browsable(false)]
	public class FloatDocumentFormSkinPainter : FloatDocumentFormPainter {
		ISkinProvider provider;
		public FloatDocumentFormSkinPainter(ISkinProvider provider) {
			this.provider = provider;
		}
		public override bool IsSkinPainter { get { return true; } }
		protected override AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault appearance = base.CreateDefaultAppearance();
			SkinElement element = GetDockWindowCaptionSkinElement();
			if(element != null)
				appearance.ForeColor = element.Properties.GetColor(DockingSkins.SkinUnFocusCaptionColor);
			return appearance;
		}
		protected override void DrawCaptionBackground(GraphicsCache cache, FloatDocumentFormInfo info) {
			SkinElementInfo skinInfo = new SkinElementInfo(GetCaptionSkinElement(), info.caption);
			skinInfo.ImageIndex = info.IsActive ? 0 : 1;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, skinInfo);
		}
		protected override void DrawBorder(GraphicsCache cache, FloatDocumentFormInfo info) {
			SkinElementInfo skinInfo = new SkinElementInfo(GetBorderSkinElement(), info.border);
			skinInfo.ImageIndex = info.IsActive ? 0 : 1;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, skinInfo);
		}
		protected internal virtual SkinElement GetDockWindowCaptionSkinElement() {
			return GetSkin()[DockingSkins.SkinDockWindow];
		}
		protected internal virtual SkinElement GetCaptionSkinElement() {
			return GetSkin()[DockingSkins.SkinFloatDocument] ?? GetSkin()[DockingSkins.SkinFloatingWindow];
		}
		protected internal virtual SkinElement GetBorderSkinElement() {
			return GetSkin()[DockingSkins.SkinFloatDocumentBorder] ?? GetSkin()[DockingSkins.SkinFloatingWindowBorder];
		}
		protected virtual Skin GetSkin() {
			return DockingSkins.GetSkin(provider);
		}
		public override Padding BorderMargin {
			get {
				SkinElement element = GetBorderSkinElement();
				SkinPaddingEdges e = element.ContentMargins;
				return new Padding(e.Left, e.Top, e.Right, e.Bottom);
			}
		}
		public override Padding CaptionMargin {
			get {
				SkinElement element = GetCaptionSkinElement();
				SkinPaddingEdges e = element.ContentMargins;
				return new Padding(e.Left, e.Top, e.Right, e.Bottom);
			}
		}
		public override Color GetBackColor(Color baseColor) {
			SkinElement element = GetBorderSkinElement();
			return (element != null) ? element.Color.GetBackColor() : baseColor;
		}
		public override Color GetForeColor(Color baseColor) {
			SkinElement element = GetBorderSkinElement();
			return (element != null) ? element.Color.GetForeColor() : baseColor;
		}
	}
	public class ColoredFloatDocumentFormSkinPainter : FloatDocumentFormSkinPainter {
		public ColoredFloatDocumentFormSkinPainter(ISkinProvider provider) : base(provider) { }
		protected override void DrawCaptionText(GraphicsCache cache, FloatDocumentFormInfo info) {
			Color maskColor = info.PaintAppearanceCaption.BorderColor;
			if(maskColor.IsEmpty) {
				base.DrawCaptionText(cache, info);
				return;
			}
			Color color = info.PaintAppearanceCaption.ForeColor;
			if(info.AllowCaptionColorBlending && info.AllowDocumentsCaptionColorBlending) {
				if(SkinImageColorizer.CheckDarkColor(color))
					info.PaintAppearanceCaption.ForeColor = SkinImageColorizer.ConvertColorByColor(color, maskColor);
				else
					info.PaintAppearanceCaption.ForeColor = SkinImageColorizer.MultiplyColors(color, maskColor);
			}
			base.DrawCaptionText(cache, info);
			info.PaintAppearanceCaption.ForeColor = color;
		}
		public override void Draw(IntPtr hDC, FloatDocumentFormInfo info) {
			Color color = info.PaintAppearanceCaption.BorderColor;
			if(color.IsEmpty) { base.Draw(hDC, info); return; }
			using(SkinElementCustomColorizer colorizer = new SkinElementCustomColorizer(color)) {
				base.Draw(hDC, info);
			}
		}
	}
	static class WinAPIHelper {
		public static int GetInt(IntPtr ptr) {
			return IntPtr.Size == 8 ? unchecked((int)ptr.ToInt64()) : ptr.ToInt32();
		}
		public static Point GetPoint(IntPtr ptr) {
			return new Point(GetInt(ptr));
		}
		public static Point PointToFormBounds(IntPtr hWnd, Point pt) {
			NativeMethods.RECT r = new NativeMethods.RECT();
			NativeMethods.GetWindowRect(hWnd, ref r);
			return new Point(pt.X - r.Left, pt.Y - r.Top);
		}
		public static MouseButtons GetButtons(Message msg) {
			int btns = GetInt(msg.WParam);
			MouseButtons buttons = MouseButtons.None;
			if((btns & MSG.MK_LBUTTON) != 0) buttons |= MouseButtons.Left;
			if((btns & MSG.MK_RBUTTON) != 0) buttons |= MouseButtons.Right;
			return buttons;
		}
		public static int GetDelta(IntPtr wParam) {
			return (short)((((long)wParam) >> 0x10) & 0xffff);
		}
		public static MouseEventArgs GetMouseArgs(IntPtr hWnd, Message msg) {
			Point pt = PointToFormBounds(hWnd, GetPoint(msg.LParam));
			return new MouseEventArgs(GetButtons(msg), 1, pt.X, pt.Y, GetDelta(msg.WParam));
		}
		public static MouseEventArgs GetClientMouseArgs(IntPtr hWnd, Message msg) {
			Point pt = PointToFormBounds(hWnd, Translate(hWnd, IntPtr.Zero, GetPoint(msg.LParam)));
			return new MouseEventArgs(GetButtons(msg), 1, pt.X, pt.Y, 0);
		}
		public static Point Translate(IntPtr source, IntPtr target, Point p) {
			NativeMethods.POINT pt = new NativeMethods.POINT(p.X, p.Y);
			NativeMethods.MapWindowPoints(source, target, ref pt, 1);
			return new Point(pt.X, pt.Y);
		}
		public static Rectangle GetBounds(IntPtr hWnd) {
			NativeMethods.RECT r = new NativeMethods.RECT();
			NativeMethods.GetWindowRect(hWnd, ref r);
			return r.ToRectangle();
		}
		public static Control FindControl(IntPtr handle) {
			Control control = Control.FromHandle(handle);
			while(control == null && handle != IntPtr.Zero) {
				handle = BarNativeMethods.GetParent(handle);
				control = Control.FromHandle(handle);
			}
			return control;
		}
		public static Control FindChildControl(Control control, Point point) {
			Point screenPoint = control.PointToScreen(point);
			NativeMethods.POINT pt = new NativeMethods.POINT(screenPoint);
			return FindControl(BarNativeMethods.WindowFromPoint(pt));
		}
		public static Control FindControl(Point screenPoint) {
			NativeMethods.POINT pt = new NativeMethods.POINT(screenPoint);
			return FindControl(BarNativeMethods.WindowFromPoint(pt));
		}
	}
	static class ParentNotifyHelper {
		public static bool ShouldSelectChild<TControl>(Control container, Point pt, out TControl child) where TControl : Control {
			child = container.GetChildAtPoint(pt) as TControl;
			return ShouldSelectChild(container, pt, child);
		}
		public static bool ShouldSelectChild(Control container, Point pt, Control child) {
			if(child == null || !child.IsHandleCreated) return false;
			Control target = WinAPIHelper.FindChildControl(container, pt);
			if(target == null || target is Docking.ControlContainer) return false;
			bool isUserMouse = ControlStylesHelper.IsUserMouse(target);
			bool isSelectable = ControlStylesHelper.IsSelectable(target);
			return !isUserMouse || !isSelectable;
		}
	}
	static class ControlStylesHelper {
		public static bool IsUserMouse(Control target) {
			return GetControlStyle(target, ControlStyles.UserMouse);
		}
		public static bool IsSelectable(Control target) {
			return GetControlStyle(target, ControlStyles.Selectable);
		}
		public static bool IsSelectableRecursive(Control target) {
			return IsSelectable(target) || Any(target.Controls, (c) => IsSelectableRecursive(c));
		}
		static bool Any(Control.ControlCollection controls, Func<Control, bool> predicate) {
			foreach(Control c in controls) {
				if(predicate(c)) return true;
			}
			return false;
		}
		static Func<Control, ControlStyles, bool> getControlStyleMethod;
		static bool GetControlStyle(Control control, ControlStyles flag) {
			if(getControlStyleMethod == null)
				getControlStyleMethod = ControlMethodBuilder.BuildControlInvoke<ControlStyles, bool>("GetStyle");
			return getControlStyleMethod(control, flag);
		}
	}
}
