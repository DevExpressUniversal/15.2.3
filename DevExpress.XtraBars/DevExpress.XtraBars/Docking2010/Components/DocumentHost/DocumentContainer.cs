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
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Skins.XtraForm;
using DevExpress.Utils;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Mdi;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraBars.Docking2010 {
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class DocumentContainer : Control, IFloatDocumentFormInfoOwner, IButtonsPanelOwner, ISupportLookAndFeel {
		DocumentManager managerCore;
		BaseDocument documentCore;
		FloatDocumentFormInfo infoCore;
		ButtonsPanel buttonsPanelCore;
		UserLookAndFeel lookAndFeelCore;
		static readonly Size DefaultInitialSize = new Size(300, 300);
		public DocumentContainer(BaseDocument document) {
			SetManager(document.Manager);
			lookAndFeelCore = CreateLookAndFeel();
			Size? initialSize = null;
			if(document.Manager != null)
				initialSize = GetInitialSize(document, initialSize);
			InitializeDocumentContainer(document, initialSize.GetValueOrDefault(DefaultInitialSize));
		}
		Size? GetInitialSize(BaseDocument document, Size? initialSize) {
			if(Manager.View != null) {
				var bounds = Manager.View.GetBounds(document);
				if(bounds.Width > 0 && bounds.Height > 0)
					initialSize = bounds.Size;
			}
			if(!initialSize.HasValue) {
				if(document.IsDockPanel) {
					var panel = document.GetDockPanel();
					if(panel != null)
						initialSize = panel.Size;
				}
			}
			return initialSize;
		}
		void InitializeDocumentContainer(BaseDocument document, Size initialSize) {
			SuspendLayout();
			this.documentCore = document;
			base.SetStyle(ControlStyles.Selectable, false);
			base.SetStyle(ControlStyles.CacheText, true);
			InitializeDocumentContainerBounds(document, initialSize);
			Document.Disposed += OnDocumentDisposed;
			this.infoCore = CreateInfo();
			this.buttonsPanelCore = CreateButtonsPanel();
			ButtonsPanel.Buttons.BeginUpdate();
			ButtonsPanel.Buttons.AddRange(new IButton[] { 
				new CloseButton(), new PinButton(), new MaximizeButton() });
			SubscribeEmbeddedButtonPanel();
			ButtonsPanel.Buttons.CancelUpdate();
			ResumeLayout(false);
		}
		void InitializeDocumentContainerBounds(BaseDocument document, Size initialSize) {
			Rectangle bounds = document.GetBoundsCore();
			if(bounds.IsEmpty)
				bounds = new Rectangle(Point.Empty, initialSize);
			Bounds = bounds;
		}
		protected virtual UserLookAndFeel CreateLookAndFeel() {
			return new EmbeddedUserLookAndFeel(this);
		}
		public override System.Drawing.Color ForeColor {
			get { return GetForeColor(base.ForeColor); }
			set { base.ForeColor = value; }
		}
		protected override CreateParams CreateParams {
			get {
				CreateParams createParams = base.CreateParams;
				createParams.ExStyle |= 0x00100000;
				return createParams;
			}
		}
		bool ProcessArrowKey(Control child, bool forward) {
			return SelectNextControl(child, forward, false, false, true);
		}
		bool ProcessTabKey(Control child, bool forward) {
			return SelectNextControl(child, forward, true, true, true);
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if((keyData & (Keys.Alt | Keys.Control)) == Keys.None) {
				Control child = (Controls.Count > 0) ? Controls[0] : null;
				if(child != null) {
					Keys keys = keyData & Keys.KeyCode;
					switch(keys) {
						case Keys.Left:
						case Keys.Up:
						case Keys.Right:
						case Keys.Down:
							if(!ProcessArrowKey(child, (keys == Keys.Right) || (keys == Keys.Down)))
								break;
							return true;
						case Keys.Tab:
							if(ProcessTabKey(child, (keyData & Keys.Shift) == Keys.None))
								return true;
							break;
					}
				}
			}
			return base.ProcessDialogKey(keyData);
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
					if(Document != null)
						Document.Disposed -= OnDocumentDisposed;
					UnsubscribeEmbeddedButtonPanel();
					OnDispose();
					this.managerCore = null;
					Redraw.UnLock(this);
				}
			}
			base.Dispose(disposing);
		}
		public DocumentManager Manager {
			get { return managerCore; }
		}
		public FloatDocumentFormInfo Info {
			get { return infoCore; }
		}
		protected virtual FloatDocumentFormInfo CreateInfo() {
			return new FloatDocumentFormInfo(this);
		}
		internal void SetManager(DocumentManager manager) {
			managerCore = manager;
		}
		int lockDocumentDisposingCore = 0;
		protected bool IsDocumentDisposingLocked {
			get { return lockDocumentDisposingCore > 0; }
		}
		protected internal void LockDocumentDisposing() {
			lockDocumentDisposingCore++;
		}
		protected internal void UnLockDocumentDisposing() {
			lockDocumentDisposingCore--;
		}
		protected virtual void OnDispose() {
			Ref.Dispose(ref infoCore);
			Ref.Dispose(ref buttonsPanelCore);
			if(!IsDocumentDisposingLocked)
				Ref.Dispose(ref documentCore);
		}
		void OnDocumentDisposed(object sender, EventArgs e) {
			Dispose();
		}
		public BaseDocument Document {
			get { return documentCore; }
		}
		public ButtonsPanel ButtonsPanel {
			get { return buttonsPanelCore; }
		}
		protected virtual ButtonsPanel CreateButtonsPanel() {
			return new ButtonsPanel(this);
		}
		#region ControlCollection
		protected override Control.ControlCollection CreateControlsInstance() {
			return new DocumentContainerControlCollection(this);
		}
		public class DocumentContainerControlCollection : Control.ControlCollection {
			public DocumentContainerControlCollection(DocumentContainer owner)
				: base(owner) {
			}
			public override void Add(Control value) {
				if(value is DocumentContainer)
					throw new NotSupportedException(value.ToString());
				base.Add(value);
			}
		}
		#endregion
		#region IFloatDocumentFormInfoOwner Members
		BarAndDockingController IFloatDocumentFormInfoOwner.GetController() {
			return Manager.GetBarAndDockingController();
		}
		void IFloatDocumentFormInfoOwner.SetCapture() {
			NativeMethods.SetCapture(Handle);
		}
		void IFloatDocumentFormInfoOwner.InvalidateNC() {
			InvalidateNC();
		}
		void IFloatDocumentFormInfoOwner.ButtonClick(FormCaptionButtonAction formCaptionButtonAction) {
			OnButtonClick(formCaptionButtonAction);
		}
		void IFloatDocumentFormInfoOwner.ShowContextMenu(Point pt) {
			Manager.View.Controller.ShowContextMenu(Document, pt);
		}
		#endregion
		#region IButtonsPanelOwner Members
		object IButtonsPanelOwner.Images {
			get { return Manager != null ? Manager.Images : null; }
		}
		DevExpress.Utils.Drawing.ObjectPainter IButtonsPanelOwner.GetPainter() {
			if(Manager != null && Manager.PaintStyleName == "WindowsXP")
				return new BaseButtonsPanelWindowsXpPainter();
			if(Manager != null && Manager.PaintStyleName == "Skin")
				return new ButtonsPanelSkinPainter(Manager.LookAndFeel);
			return new ButtonsPanelPainter();
		}
		public bool IsSelected {
			get { return (Info != null) && Info.IsActive; }
		}
		void IButtonsPanelOwner.Invalidate() {
			NativeMethods.RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero, 0x401);
		}
		#endregion
		public ToolTipController ToolTipController {
			get {
				if(Manager == null) return ToolTipController.DefaultController;
				return Manager.ToolTipController ?? ToolTipController.DefaultController;
			}
		}
		protected void SubscribeEmbeddedButtonPanel() {
			ButtonsPanel.Changed += EmbeddedButtonPanelChanged;
			ButtonsPanel.ButtonClick += OnDefaultButtonClick;
			ButtonsPanel.ButtonChecked += OnDefaultButtonClick;
			ButtonsPanel.ButtonUnchecked += OnDefaultButtonClick;
			ButtonsPanel.ButtonClick += OnButtonClick;
			ButtonsPanel.ButtonChecked += OnButtonChecked;
			ButtonsPanel.ButtonUnchecked += OnButtonUnchecked;
		}
		protected void UnsubscribeEmbeddedButtonPanel() {
			ButtonsPanel.Changed -= EmbeddedButtonPanelChanged;
			ButtonsPanel.ButtonClick -= OnDefaultButtonClick;
			ButtonsPanel.ButtonChecked -= OnDefaultButtonClick;
			ButtonsPanel.ButtonUnchecked -= OnDefaultButtonClick;
			ButtonsPanel.ButtonClick -= OnButtonClick;
			ButtonsPanel.ButtonChecked -= OnButtonChecked;
			ButtonsPanel.ButtonUnchecked -= OnButtonUnchecked;
		}
		internal void UpdateInnerRibbonVisibility() {
			RibbonControl innerRibbon = FindRibbon(this);
			if(innerRibbon != null)
				innerRibbon.UpdateIsMdiChildRibbon();
		}
		RibbonControl FindRibbon(Control parentControl) {
			if(parentControl == null) return null;
			RibbonControl result = parentControl as RibbonControl;
			if(result != null) return result;
			foreach(Control ctrl in parentControl.Controls) {
				result = FindRibbon(ctrl);
				if(result != null) return result;
			}
			return result;
		}
		protected virtual void OnButtonChecked(object sender, ButtonEventArgs e) { }
		protected virtual void OnButtonUnchecked(object sender, ButtonEventArgs e) { }
		protected virtual void OnButtonClick(object sender, ButtonEventArgs e) { }
		protected virtual void OnDefaultButtonClick(object sender, ButtonEventArgs e) {
			IButton button = e.Button;
			if(!(button is DefaultButton)) return;
			if(button is CloseButton)
				OnButtonClick(FormCaptionButtonAction.Close);
			if(button is MaximizeButton) {
				IsMaximized = button.Properties.Checked;
				OnButtonClick(button.Properties.Checked ? FormCaptionButtonAction.Maximize : FormCaptionButtonAction.Restore);
			}
		}
		void EmbeddedButtonPanelChanged(object sender, EventArgs e) {
			InvalidateNC();
		}
		protected virtual void OnButtonClick(FormCaptionButtonAction kind) {
			int cmd = -1;
			switch(kind) {
				case FormCaptionButtonAction.Close: cmd = NativeMethods.SC.SC_CLOSE; break;
				case FormCaptionButtonAction.Maximize: cmd = NativeMethods.SC.SC_MAXIMIZE; break;
				case FormCaptionButtonAction.Restore: cmd = NativeMethods.SC.SC_RESTORE; break;
			}
			if(cmd != -1) {
				NativeMethods.PostMessage(Handle, MSG.WM_SYSCOMMAND, new IntPtr(cmd), IntPtr.Zero);
				BeginInvoke(new MethodInvoker(InvalidateNC));
			}
		}
		protected internal void InvalidateNC() {
			if(!IsDisposing && IsHandleCreated && Manager != null && !Manager.View.IsDisposing)
				FormPainter.InvalidateNC(Handle);
		}
		protected virtual void Activate(bool isActive) {
			IDocumentsHost host = Parent as IDocumentsHost;
			if(IsDisposing || host == null) return;
			if(isActive) {
				host.SetActiveContainer(this);
				if(!Borderless)
					BringToFront();
			}
		}
		protected bool Borderless {
			get { return Manager == null || Document.IsFloating || Document.Borderless; }
		}
		protected override void WndProc(ref Message m) {
			switch(m.Msg) {
				case MSG.WM_KILLFOCUS:
					ProcessFocus(ref m, false);
					base.WndProc(ref m);
					break;
				case MSG.WM_PRINT:
					DoPrint(ref m);
					break;
				case MSG.WM_SETFOCUS:
					ProcessFocus(ref m, true);
					base.WndProc(ref m);
					break;
				case MSG.WM_SYSCOMMAND:
					if(ProcessSysCommand(ref m))
						base.WndProc(ref m);
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
				case MSG.WM_ERASEBKGND:
					m.Result = new IntPtr(1);
					break;
				case MSG.WM_WINDOWPOSCHANGED:
					ProcessWindowPosChanged(m.LParam);
					base.WndProc(ref m);
					break;
				case MSG.WM_GETMINMAXINFO:
					ProcessGetMinMaxInfo(ref m);
					base.WndProc(ref m);
					break;
				default:
					base.WndProc(ref m);
					break;
			}
			XtraBars.CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		protected virtual void DoBaseWndProc(ref Message m) {
			base.WndProc(ref m);
		}
		protected virtual bool ProcessSysCommand(ref Message m) {
			switch((WinAPIHelper.GetInt(m.WParam) & 0xFFF0)) {
				case DevExpress.Utils.Drawing.Helpers.NativeMethods.SC.SC_CLOSE:
					if(Manager.View.Controller.Close(Document))
						m.Result = IntPtr.Zero;
					return false;
				case DevExpress.Utils.Drawing.Helpers.NativeMethods.SC.SC_MAXIMIZE:
					if(!Document.CanMaximize())
						return false;
					break;
			}
			return true;
		}
		protected void ProcessFocus(ref Message m, bool setFocus) {
			if(IsDisposing || Parent == null) return;
			Control ctrl = WinAPIHelper.FindControl(m.WParam);
			while(ctrl != null) {
				DocumentContainer container = ctrl as DocumentContainer;
				if(container != null) {
					container.Activate(!setFocus);
					break;
				}
				ctrl = ctrl.Parent;
			}
		}
		protected virtual void ProcessWindowPosChanged(IntPtr lParam) {
			if(lParam == IntPtr.Zero || Document == null) return;
			if(Parent is IDocumentsHost) {
				var pos = (NativeMethods.WINDOWPOS)BarNativeMethods.PtrToStructure(lParam, typeof(NativeMethods.WINDOWPOS));
				if(((pos.flags & NativeMethods.SWP.SWP_NOMOVE) == 0) || ((pos.flags & NativeMethods.SWP.SWP_NOSIZE)) == 0)
					Document.SetBoundsCore(new Rectangle(pos.x, pos.y, pos.cx, pos.cy));
				UpdateIsMaximized();
			}
		}
		protected virtual void ProcessGetMinMaxInfo(ref Message m) {
			if(m.LParam == IntPtr.Zero || Document == null) return;
			var minMax = (NativeMethods.MINMAXINFO)BarNativeMethods.PtrToStructure(m.LParam, typeof(NativeMethods.MINMAXINFO));
			if(!Borderless && Info != null && Info.IsReady) {
				Size ncMinSize = Info.GetNCMinSize();
				minMax.ptMinTrackSize = new NativeMethods.POINT(ncMinSize.Width, ncMinSize.Height);
				BarNativeMethods.StructureToPtr(minMax, m.LParam, false);
			}
		}
		protected void UpdateIsMaximized() {
			if(IsDisposing) return;
			bool prevMaximized = IsMaximized;
			IsMaximized = MdiChildHelper.GetIsMaximized(Handle);
			if(prevMaximized != IsMaximized && ButtonsPanel != null) {
				var maximizeButton = ButtonsPanel.Buttons.FindFirst((b) => b is MaximizeButton) as MaximizeButton;
				if(maximizeButton != null) {
					maximizeButton.Checked = prevMaximized;
					InvalidateNC();
				}
				if(prevMaximized && RestoreBounds.HasValue) {
					Bounds = RestoreBounds.Value;
					RestoreBounds = null;
				}
				if(prevMaximized && Parent is IDocumentsHost)
					Parent.Update();
			}
		}
		protected virtual void DoNCMouseLeave() {
			if(ButtonsPanel != null) {
				ButtonsPanel.Handler.OnMouseLeave();
				InvalidateNC();
				trackingMouseLeave = false;
			}
			ToolTipController.HideHint();
		}
		protected virtual void DoNCRMouseUp(MouseEventArgs e) {
			if(Info.IsActive) Info.OnContextMenu(e);
		}
		protected virtual void DoNCMouseDown(MouseEventArgs e) {
			if(ButtonsPanel != null) {
				MouseEventArgs es = new MouseEventArgs(MouseButtons.Left, e.Clicks, e.X, e.Y, e.Delta);
				ButtonsPanel.Handler.OnMouseDown(es);
				if(ButtonsPanel.Handler.PressedButton != null) {
					NativeMethods.SetCapture(Handle);
				}
				else {
					if(!Borderless)
						BringToFront();
				}
			}
		}
		bool trackingMouseLeave = false;
		protected virtual void DoNCMouseMove(MouseEventArgs e) {
			if(ButtonsPanel != null)
				ButtonsPanel.Handler.OnMouseMove(e);
			ProcessNCMouseMove(e.Location);
			if(!trackingMouseLeave) {
				NativeMethods.TRACKMOUSEEVENTStruct msevnt = new NativeMethods.TRACKMOUSEEVENTStruct();
				msevnt.cbSize = Marshal.SizeOf(msevnt);
				msevnt.dwFlags = BarNativeMethods.TME_NONCLIENT | BarNativeMethods.TME_LEAVE;
				msevnt.hwndTrack = Handle;
				msevnt.dwHoverTime = 0;
				if(NativeMethods.TrackMouseEvent(msevnt)) {
					trackingMouseLeave = true;
				}
			}
		}
		protected virtual void ProcessNCMouseMove(Point p) { }
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
		protected override void OnEnter(EventArgs e) {
			base.OnEnter(e);
			SetIsActive(true);
		}
		protected override void OnLeave(EventArgs e) {
			SetIsActive(false);
			base.OnLeave(e);
		}
		protected virtual void SetIsActive(bool isActive) {
			bool shouldInvalidate = false;
			if(Info != null && Info.IsActive != isActive) {
				shouldInvalidate = true;
				Info.IsActive = isActive;
			}
			if(shouldInvalidate && !Borderless) {
				BringToFront();
				InvalidateNC();
			}
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
			if(Document == null || Document.IsDisposing) return;
			IntPtr hDC = FormPainter.GetDC(Handle, m);
			try {
				if(!IsDisposing) {
					if(!isReady)
						CalculateNC(new Rectangle(Point.Empty, Size));
					Painter.Draw(hDC, Info);
				}
			}
			finally { NativeMethods.ReleaseDC(Handle, hDC); }
		}
		void DoPrint(ref Message m) {
			if(Document == null || Document.IsDisposing) return;
			IntPtr hDC = m.WParam;
			if(!IsDisposing) {
				if(!isReady)
					CalculateNC(new Rectangle(Point.Empty, Size));
				Painter.Draw(hDC, Info);
			}
		}
		protected virtual FloatDocumentFormPainter CreatePainter() {
			if(Manager != null && Manager.PaintStyleName == "Skin")
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
		protected virtual Color GetForeColor(Color baseColor) {
			if(IsDisposing || Manager == null) return baseColor;
			Color result = Painter.GetForeColor(baseColor);
			return result.IsEmpty ? baseColor : result;
		}
		protected internal virtual bool IsMaximized { get; protected set; }
		protected internal Rectangle? RestoreBounds { get; set; }
		bool isReady;
		protected virtual Rectangle CalculateNC(Rectangle rect) {
			if(IsDisposed || Manager == null) return rect;
			if(Document == null || Document.IsDisposing) return rect;
			isReady = true;
			Info.Text = Document.Caption;
			Info.Image = Document.GetActualImage() ?? DevExpress.Utils.ImageCollection.GetImageListImage(Manager.Images, Document.ImageIndex);
			Info.AllowGlyphSkinning = CanUseGlyphSkinning;
			Info.ShowCloseButton = HasCloseButton;
			Info.ShowMaximizeButton = HasMaximizeButton;
			Info.ShowPinButton = HasPinButton;
			return Info.CalculateNC(rect, Painter, Borderless, IsMaximized);
		}
		protected virtual bool CanUseGlyphSkinning {
			get { return Document.Properties.CanUseGlyphSkinning; }
		}
		protected virtual bool HasCloseButton {
			get { return !Borderless && Document.HasCloseButton(); }
		}
		protected virtual bool HasMaximizeButton {
			get { return !Borderless && Document.HasMaximizeButton(); }
		}
		protected virtual bool HasPinButton {
			get { return false; }
		}
		public void UpdateStyle() {
			painterCore = null;
			if(ButtonsPanel != null && ButtonsPanel.Buttons != null) {
				ButtonsPanel.UpdateStyle();
			}
			InvalidateNC();
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			base.SetBoundsCore(x, y, width, height, specified);
			if(0 != (specified & BoundsSpecified.Width) && 0 != (specified & BoundsSpecified.Height)) {
				if(Document == null) return;
				if(Document.IsDockPanel) {
					DockPanel panel = Document.GetDockPanel();
					if(panel == null) return;
					bool panelHostedWithinFloatForm = panel.Parent is FloatForm;
					if(panelHostedWithinFloatForm)
						panel.setBoundsInDocumentContainer++;
					try {
						if(panel.Bounds.Size != ClientSize)
							panel.Bounds = new Rectangle(Point.Empty, ClientSize);
					}
					finally {
						if(panelHostedWithinFloatForm)
							panel.setBoundsInDocumentContainer--;
					}
				}
			}
		}
		#region IButtonsPanelOwner Members
		bool IButtonsPanelOwner.AllowGlyphSkinning {
			get { return Document.Properties.CanUseGlyphSkinning; }
		}
		bool IButtonsPanelOwner.AllowHtmlDraw {
			get { return false; }
		}
		DevExpress.XtraEditors.ButtonsPanelControl.ButtonsPanelControlAppearance IButtonsPanelOwner.AppearanceButton {
			get { return null; }
		}
		object IButtonsPanelOwner.ButtonBackgroundImages {
			get { return null; }
		}
		#endregion
		internal static DocumentContainer FromControl(Control control) {
			while(control != null) {
				DocumentContainer container = control as DocumentContainer;
				if(container != null)
					return container;
				Form form = control as Form;
				if(form != null) {
					var popupForm = form as DevExpress.XtraEditors.Popup.PopupBaseForm;
					if(popupForm != null && popupForm.OwnerEdit != null)
						return FromControl(popupForm.OwnerEdit);
				}
				control = control.Parent;
			}
			return null;
		}
		protected internal bool NCHitTest(Point pt) {
			return (Info != null) && Info.NCHitTest(pt);
		}
		protected internal bool CanValidateChild() {
			return (Document != null) && Document.IsControlLoaded;
		}
		protected internal bool ValidateChild() {
			return !ContainerValidationHelper.PerformContainerValidation(Document.Control, ValidationConstraints.Selectable);
		}
		static class ContainerValidationHelper {
			static Func<Control, ValidationConstraints, bool> performContainerValidationMethod;
			internal static bool PerformContainerValidation(Control control, ValidationConstraints constraints) {
				if(performContainerValidationMethod == null)
					performContainerValidationMethod = ControlMethodBuilder.BuildControlInvoke<ValidationConstraints, bool>("PerformContainerValidation");
				return performContainerValidationMethod(control, constraints);
			}
		}
		#region ISupportLookAndFeel Members
		bool ISupportLookAndFeel.IgnoreChildren {
			get { return false; }
		}
		UserLookAndFeel ISupportLookAndFeel.LookAndFeel {
			get { return lookAndFeelCore; }
		}
		#endregion
		class EmbeddedUserLookAndFeel : ContainerUserLookAndFeel {
			public EmbeddedUserLookAndFeel(object owner)
				: base(owner) {
				Container.HandleCreated += OnContainerChanged;
				Container.ParentChanged += OnContainerChanged;
			}
			void OnContainerChanged(object sender, EventArgs e) {
				UpdateParentControlLookAndFeel();
			}
			public override void Dispose() {
				Container.HandleCreated -= OnContainerChanged;
				Container.ParentChanged -= OnContainerChanged;
			}
			protected UserLookAndFeel GetParentControlLookAndFeel() {
				ISupportLookAndFeel res = Container.FindForm() as ISupportLookAndFeel;
				DocumentContainer dc = Container as DocumentContainer;
				if(res != null) {
					Control ctrl = Container;
					while(ctrl != null) {
						ILookAndFeelProvider provider = ctrl as ILookAndFeelProvider;
						if(provider != null)
							return provider.LookAndFeel;
						ISupportLookAndFeel sl = ctrl as ISupportLookAndFeel;
						if(sl != null && sl.IgnoreChildren)
							return null;
						ctrl = ctrl.Parent;
					}
				}
				if(CheckBarAndDockingController(dc))
					return dc.Manager.BarAndDockingController.LookAndFeel;
				else if(res != null)
					return res.LookAndFeel;					
				return null;
			}
			protected internal void UpdateParentControlLookAndFeel() {
				UserLookAndFeel lf = GetParentControlLookAndFeel();
				if(lf != null) SetControlParentLookAndFeel(lf);
			}
			protected virtual bool CheckBarAndDockingController(DocumentContainer dc) {
				return dc != null && dc.Manager != null && dc.Manager.BarAndDockingController != null;
			}
		}
	}
}
