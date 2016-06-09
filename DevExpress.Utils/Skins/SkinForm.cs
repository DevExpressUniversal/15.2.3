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
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Xml;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.NonclientArea;
using System.Collections.Specialized;
using DevExpress.Utils.WXPaint;
using DevExpress.Utils.Text;
using System.Security;
using System.Threading;
namespace DevExpress.Skins.XtraForm {
	public enum FrameKind { Left, Right, Bottom, Top };
	public class FromNativeWindow : NativeWindow {
		FormPainter painter;
		public FromNativeWindow(Control owner, ISkinProvider provider) {
			this.painter = new FormPainter(owner, provider);
			this.painter.BaseWndProc += new FormPainter.WndProcMethod(DoBase);
		}
		protected override void WndProc(ref Message msg) {
			if(painter.DoWndProc(ref msg)) {
				return;
			}
			base.WndProc(ref msg);
		}
		void DoBase(ref Message msg) {
			base.WndProc(ref msg);
		}
	}
	public class FormClientPainter : FormPainter {
		public FormClientPainter(Control owner, ISkinProvider provider) : base(owner, provider) { }
		protected override void NCCalcSize(ref Message m) {
			if(m.WParam == IntPtr.Zero || m.WParam == new IntPtr(1)) {
				NativeMethods.NCCALCSIZE_PARAMS csp = NativeMethods.NCCALCSIZE_PARAMS.GetFrom(m.LParam);
				csp.SetTo(m.LParam);
			}
		}
		public override void Draw(PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(e)) {
				DrawBackground(cache);
				DrawBorders(cache, false);
				DrawButtons(cache, false);
				DrawIcon(cache);
				DrawText(cache);
			}
		}
		protected override void DrawFrameNC(Message msg, bool drawBorders) { }
		public override bool RequirePaint { get { return true; } }
		public override bool RequirePaintBackground { get { return false; } }
		public override Rectangle ClientBounds {
			get {
				if(Owner == null) return Rectangle.Empty;
				Rectangle bounds = Owner.Bounds;
				bounds.Location = Point.Empty;
				return Margins.Deflate(bounds);
			}
		}
		public override void DrawBackground(PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(e)) {
				Rectangle r = ClientBounds;
				cache.FillRectangle(cache.GetSolidBrush(Owner.BackColor), r);
			}
		}
	}
	public class FormPainter : IDisposable, ISkinProviderEx, IStringImageProvider, IServiceProvider {
		protected internal static int IconLeftMargin = 2;
		bool isDestroyed = false;
		object iconImage = null;
		Control owner;
		FormCaptionButtonCollection buttons;
		ObjectPainter buttonPainter;
		bool isReady = false;
		ISkinProvider provider;
		GraphicsInfo ginfo;
		public FormPainter(Control owner, ISkinProvider provider) {
			this.ginfo = new GraphicsInfo();
			this.provider = provider;
			this.owner = owner;
			this.buttons = CreateFormCaptionButtonCollection();
			this.buttonPainter = CreateFormCaptionButtonSkinPainter();
			Owner.HandleCreated += new EventHandler(OnHandleCreated);
			Owner.HandleDestroyed += new EventHandler(OnHandleDestroyed);
			Owner.TextChanged += new EventHandler(OnOwnerTextChanged);
			if(Owner.IsHandleCreated) OnHandleCreated(Owner, EventArgs.Empty);
		}
		protected virtual FormCaptionButtonCollection CreateFormCaptionButtonCollection() {
			return new FormCaptionButtonCollection(this);
		}
		protected virtual FormCaptionButtonSkinPainter CreateFormCaptionButtonSkinPainter() {
			return new FormCaptionButtonSkinPainter(this);
		}
		string text = null;
		void OnOwnerTextChanged(object sender, EventArgs e) {
			this.text = null;
			if(AllowHtmlDraw) {
				if(StringPainter.Default.RemoveFormat(Text) != Text) {
					CheckFormHeightChanged();
				}
			}
		}
		public string Text {
			get {
				if(text == null && Owner != null) {
					if(AllowHtmlDraw) 
						text = XtraFormOwner.HtmlText;
					else if(XtraFormOwner != null && XtraFormOwner.ShowMdiChildCaptionInParentTitle) {
						text = GetMdiCombinedCaption();
					}
					else
						text = Owner.Text;
				}
				return text;
			}
		}
		protected virtual string GetMdiCombinedCaption() {
			string mdiChild = GetMdiChildCaption();
			if(string.IsNullOrEmpty(mdiChild)) return GetMdiParentCaption();
			string mdiParent = GetMdiParentCaption();
			return string.Format(XtraFormOwner.MdiChildCaptionFormatString, mdiParent, mdiChild);
		}
		protected virtual string GetMdiChildCaption() {
			if(XtraFormOwner == null || !XtraFormOwner.IsMdiContainer || XtraFormOwner.ActiveMdiChild == null) return string.Empty;
			return XtraFormOwner.ActiveMdiChild.Text;
		}
		protected virtual string GetMdiParentCaption() {
			return Owner.Text;
		}
		public GraphicsInfo GInfo { get { return ginfo; } }
		string ISkinProvider.SkinName { get { return provider.SkinName; } }
		public ISkinProvider Provider { get { return provider; } }
		public bool IsDestroyed { get { return isDestroyed; } }
		public virtual void Dispose() {
			this.isDestroyed = true;
			this.BaseWndProc = null;
			Owner.HandleCreated -= new EventHandler(OnHandleCreated);
			Owner.HandleDestroyed -= new EventHandler(OnHandleDestroyed);
			Owner.TextChanged -= new EventHandler(OnOwnerTextChanged);
			Buttons.Clear();
			this.owner = null;
			DestroyIcon();
		}
		public virtual Rectangle ClientBounds { get { return Owner != null ? Owner.ClientRectangle : Rectangle.Empty; } }
		public virtual bool RequirePaintBackground { get { return false; } }
		public virtual bool RequirePaint { get { return RenderSizeGrip; } }
		public virtual bool RenderSizeGrip {
			get {
				Form frm = Owner as Form;
				if(frm == null) return false;
				Type tp = typeof(Form);
				FieldInfo fi = tp.GetField("formState", BindingFlags.NonPublic | BindingFlags.Instance);
				if(fi == null) return false;
				BitVector32 state = (BitVector32)fi.GetValue(frm);
				fi = tp.GetField("FormStateRenderSizeGrip", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
				if(fi == null) return false;
				return state[(BitVector32.Section)fi.GetValue(frm)] != 0;
			}
		}
		public virtual void Draw(PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
				DrawSizeGrip(cache);
			}
		}
		protected virtual SkinElementInfo GetSizeGripInfo() {
			return new SkinElementInfo(CommonSkins.GetSkin(Provider)[CommonSkins.SkinSizeGrip], Rectangle.Empty);
		}
		protected virtual Rectangle SizeGripBounds {
			get {
				SkinElementInfo info = GetSizeGripInfo();
				if(info == null || info.Element.Image == null) return Rectangle.Empty;
				Size sz = info.Element.Image.GetImageBounds(0).Size;
				return new Rectangle(new Point(ClientBounds.Right - sz.Width, ClientBounds.Bottom - sz.Height), sz);
			}
		}
		protected virtual void DrawSizeGrip(GraphicsCache cache) {
			SkinElementInfo info = GetSizeGripInfo();
			if(info == null) return;
			Size sz = info.Element.Image.GetImageBounds(0).Size;
			info.Bounds = new Rectangle(new Point(ClientBounds.Right - sz.Width, ClientBounds.Bottom - sz.Height), sz);
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
		}
		public virtual void DrawBackground(PaintEventArgs e) { }
		void DestroyIcon() {
			Icon ico = iconImage as Icon;
			if(ico != null) ico.Dispose();
			this.iconImage = null;
		}
		void OnHandleCreated(object sender, EventArgs e) {
		}
		void OnHandleDestroyed(object sender, EventArgs e) {
			closeButtonDisabled = null;
			SetDirty();
			DestroyIcon();
		}
		protected virtual Size GetIconSize() {
			if(Handle == IntPtr.Zero && !IsToolWindow) {
				return (XtraFormOwner != null && !XtraFormOwner.ShowIcon) ? Size.Empty : SystemInformation.SmallIconSize;
			}
			Icon ico = GetIcon();
			if(ico != null) {
				return SystemInformation.SmallIconSize;
			}
			return Size.Empty;
		}
		protected FormCaptionButtonCollection Buttons { get { return buttons; } }
		protected ObjectPainter ButtonPainter { get { return buttonPainter; } }
		protected internal Control Owner { get { return owner; } }
		internal SkinPaddingEdges margins = null;
		protected virtual Point PointToFormBounds(Point pt) {
			Point res = pt;
			NativeMethods.RECT r = new NativeMethods.RECT();
			NativeMethods.GetWindowRect(Handle, ref r);
			res.X -= r.Left;
			res.Y -= r.Top;
			return res;
		}
		public Point PointToFormBounds(IntPtr ptr) {
			Point p = Point.Empty;
			try {
				p = new Point((int)ptr);
			}
			catch (Exception) {
				p = Point.Empty;
			}
			return PointToFormBounds(p);
		}
		IntPtr TranslateCoord(IntPtr ptr) {
			Point p = new Point((int)ptr);
			Size rel = new Size(Owner.PointToClient(Owner.Bounds.Location));
			p = Owner.PointToScreen(p);
			Point test = PointToFormBounds(new IntPtr((p.Y << 16) | p.X));
			return new IntPtr(((int)((ushort)p.Y) << 16) | (int)((ushort)p.X));
		}
		protected bool IsMdiForm { get { return (Owner is Form) && ((Form)Owner).IsMdiChild; } }
		protected IntPtr Handle {
			get {
				if(!Owner.IsHandleCreated) return IntPtr.Zero;
				return Owner.Handle;
			}
		}
		bool isTracking = false;
		protected virtual void TrackMouseLeaveMessage() {
			if(isTracking) return;
			NativeMethods.TRACKMOUSEEVENTStruct track = new NativeMethods.TRACKMOUSEEVENTStruct();
			track.dwFlags = 0x13;
			track.hwndTrack = Owner.Handle;
			if(!NativeMethods.TrackMouseEvent(track)) {
				return;
			}
			isTracking = true;
		}
		public delegate void WndProcMethod(ref Message msg);
		public event WndProcMethod BaseWndProc;
		protected virtual void DoBaseWndProc(ref Message msg) {
			if(BaseWndProc != null) BaseWndProc(ref msg);
		}
		protected internal virtual bool IsAllowNCDraw { get { return true; } }
		public virtual bool DoWndProc(ref Message msg) {
			bool temp = false;
			switch(msg.Msg) {
				case MSG.WM_GETMINMAXINFO:
					WMGetMinMaxInfo(ref msg);
					return true;
				case MSG.WM_NCCALCSIZE:
					NCCalcSize(ref msg);
					return true;
				case MSG.WM_SIZE:
					return WMSize(ref msg);
				case MSG.WM_PRINT:
					DrawFrameNC(msg);
					break;
				case MSG.WM_SYNCPAINT:
				case MSG.WM_NCPAINT:
					if(!IsAllowNCDraw) return false;
					SetDirty();
					DrawFrameNC(msg);
					msg.Result = IntPtr.Zero;
					return true;
				case MSG.WM_NCACTIVATE:
					return NCActivate(ref msg);
				case MSG.WM_NCHITTEST:
					WMNCHitTest(ref msg);
					return true;
				case MSG.WM_SYSCOMMAND:
					return WMSysCommand(ref msg);
				case MSG.WM_SETICON:
					DestroyIcon();
					InvalidateNC();
					break;
				case MSG.WM_SETTEXT:
					return WMSetText(ref msg);
				case MSG.WM_NCUAHDRAWCAPTION:
				case MSG.WM_NCUAHDRAWFRAME:
					text = null;
					DrawFrameNC(msg);
					msg.Result = IntPtr.Zero;
					return true;
				case MSG.WM_MOUSEACTIVATE:
					if(!IsAllowNCDraw) return false;
					DoBaseWndProc(ref msg);
					DrawFrameNC(msg);
					return true;
				case MSG.WM_CAPTURECHANGED:
					if(!IsAllowNCDraw) return false;
					if(msg.LParam != this.Handle) {
						HotButton = PressedButton = FormCaptionButtonKind.None;
					}
					break;
				case MSG.WM_SETCURSOR:
					WMSetCursor(ref msg);
					return true;
				case MSG.WM_MOUSEMOVE:
					if(IsAllowButtonMessages)
						OnMouseMove(GetMouseArgs(TranslateCoord(msg.LParam)));
					break;
				case MSG.WM_NCMOUSELEAVE:
					if(IsAllowButtonMessages) {
						isTracking = false;
						OnMouseLeave();
					}
					break;
				case MSG.WM_NCMOUSEMOVE:
					if(IsAllowButtonMessages) {
						TrackMouseLeaveMessage();
						temp = OnMouseMove(GetMouseArgs(ref msg));
					}
					return temp;
				case MSG.WM_NCRBUTTONUP:
					return NCRButtonUp(GetMouseArgs(msg.LParam));
				case MSG.WM_NCLBUTTONDOWN:
					if(IsAllowButtonMessages) {
						temp = OnMouseDown(GetMouseArgs(msg.LParam));
					}
					if(!temp) DoBaseWndProc(ref msg);
					OnNCLButtonDown(msg);
					return true;
				case MSG.WM_LBUTTONDOWN:
					if(IsAllowButtonMessages) {
						return OnMouseDown(GetMouseArgs(TranslateCoord(msg.LParam)));
					}
					return false;
				case MSG.WM_LBUTTONUP:
					if(IsAllowButtonMessages) {
						return OnMouseUp(GetMouseArgs(TranslateCoord(msg.LParam))); 
					}
					return false;
				case MSG.WM_WINDOWPOSCHANGING:
					this.text = null;
					return false;
				case MSG.WM_NCLBUTTONUP:
					if(IsAllowButtonMessages) {
						return OnMouseUp(GetMouseArgs(msg.LParam));
					}
					return false;
			}
			return false;
		}
		protected virtual void ForceUpdateMdiChild() {
			if (XtraFormOwner != null && XtraFormOwner.ActiveMdiChild != null && XtraFormOwner.ActiveMdiChild.WindowState == FormWindowState.Maximized) {
				if(lastHackedChildHandle != XtraFormOwner.ActiveMdiChild.Handle || lastShowMdiBar == null || ShouldShowMdiBar != lastShowMdiBar) {
					lastHackedChildHandle = XtraFormOwner.ActiveMdiChild.Handle;
					UpdateMdiClient();
				}
			}
			else {
				lastHackedChildHandle = IntPtr.Zero;
			}
		}
		Rectangle regionRect = Rectangle.Empty;
		protected virtual bool WMSize(ref Message msg) {
			DoBaseWndProc(ref msg);
			if(ShouldUpdateFormRegion) {
				UpdateFormRegion();
			}
			if(lastShowMdiBar == null || lastShowMdiBar.Value != ShouldShowMdiBar) {
				lastShowMdiBar = ShouldShowMdiBar;
				UpdateMdiClient();
			}
			return true;
		}
		protected bool ShouldUpdateFormRegion {
			get { return XtraFormOwner != null && XtraFormOwner.IsHandleCreated; }
		}
		bool regionPainted = true;
		protected internal bool RegionPainted { 
			get { return regionPainted; }
			set { regionPainted = value; }
		}
		protected void UpdateFormRegion() {
			if(Owner == null || IsMdiChild)
				return;
			if(IsZoomedBorder) {
				Screen screen = Screen.FromControl(Owner);
				if(screen == null) return;
				Rectangle bounds = XtraFormOwner != null && XtraFormOwner.FormBorderStyle == FormBorderStyle.None? screen.Bounds: screen.WorkingArea;
				Rectangle formBounds = FormBounds;
				Rectangle r = new Rectangle(bounds.X - formBounds.X, bounds.Y - formBounds.Y, formBounds.Width - (formBounds.Width - bounds.Width), formBounds.Height - (formBounds.Height - bounds.Height));
				if(XtraFormOwner != null && XtraFormOwner.RightToLeftLayout)
					r.X -= r.X + r.X +1;
				SetRegion(new Region(r), r);
			}
			else if(IsWindowMinimized)
				return;
			else {
				Rectangle rect = new Rectangle();
				Region region = GetDefaultFormRegion(ref rect);
				SetRegion(region, rect);
			}
		}
		void SetRegion(Region region, Rectangle rect) {
			this.regionPainted = false;
			if(this.regionRect == rect) {
				if(region != null)
					region.Dispose();
				return;
			}
			if(Owner.Region != null) {
				Owner.Region.Dispose();
			}
			Owner.Region = region;
			this.regionRect = rect;
		}
		protected virtual Region GetDefaultFormRegion(ref Rectangle rect) { 
			rect = Rectangle.Empty; 
			return null; 
		}
		protected virtual void OnNCLButtonDown(Message msg) {
			DrawFrameNC(msg);
			UpdateMdiClient();
		}
		protected void UpdateMdiClient() {
			if(XtraFormOwner == null)
				return;
			UpdateMdiClient(XtraFormOwner.GetMdiClient());
		}
		public static void UpdateMdiClient(MdiClient mdiClient) {
			if(mdiClient == null || !mdiClient.IsHandleCreated) return;
			Form frm = mdiClient.Parent as Form;
			if(frm != null && frm.ActiveMdiChild == null)
				return;
			Utils.Mdi.MdiClientSubclasserService.BeginUpdateMdiClient(mdiClient);
			try {
				mdiClient.SuspendLayout();
				mdiClient.SetBounds(mdiClient.Bounds.X, mdiClient.Bounds.Y, mdiClient.Bounds.Width - 1, mdiClient.Bounds.Height);
			}
			finally {
				mdiClient.ResumeLayout();
				Utils.Mdi.MdiClientSubclasserService.EndUpdateMdiClient(mdiClient);
			}
		}
		protected virtual void WMGetMinMaxInfo(ref Message msg) {
			DevExpress.XtraEditors.XtraForm form = Owner as DevExpress.XtraEditors.XtraForm;
			DoBaseWndProc(ref msg);
			NativeMethods.MINMAXINFO mi = NativeMethods.MINMAXINFO.GetFrom(msg.LParam);
			if(form != null && form.WindowState == FormWindowState.Maximized && form.creatingHandle) {
				if(!form.IsMdiChild)
					mi.ptMaxPosition.Y = 0;
				mi.SetTo(msg.LParam);
			}
		}
		protected bool ShowSystemMenu() {
			return ShowSystemMenu(Owner as Form, Control.MousePosition);
		}
		protected bool ShowSystemMenu(Form frm, Point pt) {
			const int TPM_LEFTALIGN = 0x0000, TPM_TOPALIGN = 0x0000, TPM_RETURNCMD = 0x0100;
			if(frm == null)
				return false;
			IntPtr menuHandle = DevExpress.XtraEditors.XtraForm.GetSystemMenu(frm.Handle, false);
			IntPtr command = NativeMethods.TrackPopupMenu(menuHandle, TPM_RETURNCMD | TPM_TOPALIGN | TPM_LEFTALIGN,
				pt.X, pt.Y, 0, frm.Handle, IntPtr.Zero);
			if(frm.IsDisposed)
				return false;
			PostSysCommandMessage(frm.Handle, command);
			return true;
		}
		protected virtual bool NCRButtonUp(MouseEventArgs e) {
			if(CaptionBounds.Contains(e.X, e.Y)) return ShowSystemMenu();
			return false;
		}
		protected MouseEventArgs GetMouseArgs(IntPtr lparam) {
			Point pt = PointToFormBounds(lparam);
			return new MouseEventArgs(MouseButtons.Left, 1, pt.X, pt.Y, 0);
		}
		protected MouseEventArgs GetMouseArgs(ref Message msg) {
			int btns = msg.WParam.ToInt32();
			MouseButtons buttons = MouseButtons.None;
			if((btns & MSG.MK_LBUTTON) != 0) buttons |= MouseButtons.Left;
			if((btns & MSG.MK_RBUTTON) != 0) buttons |= MouseButtons.Right;
			Point pt = PointToFormBounds(msg.LParam);
			MouseEventArgs e = new MouseEventArgs(buttons, 1, pt.X, pt.Y, 0);
			return e;
		}
		FormCaptionButtonKind hotButton = FormCaptionButtonKind.None, pressedButton = FormCaptionButtonKind.None;
		protected FormCaptionButtonKind HotButton {
			get { return hotButton; }
			set {
				if(HotButton == value) return;
				hotButton = value;
				RedrawButtons();
			}
		}
		protected FormCaptionButtonKind PressedButton {
			get { return pressedButton; }
			set {
				if(PressedButton == value) return;
				pressedButton = value;
				RedrawButtons();
			}
		}
		bool IsPressed { get { return PressedButton != FormCaptionButtonKind.None; } }
		bool IsHot { get { return HotButton != FormCaptionButtonKind.None; } }
		public virtual void OnMouseLeave() {
			OnMouseMove(new MouseEventArgs(Control.MouseButtons, 0, -10000, -10000, 0));
		}
		public virtual bool OnMouseMove(MouseEventArgs e) {
			FormCaptionButton button = Buttons.GetButton(e, this);
			if(!IsWindowActive) button = null;
			HotButton = (button == null || !button.IsEnabled || (IsPressed && PressedButton != button.Kind)) ? FormCaptionButtonKind.None : button.Kind;
			return false;
		}
		protected bool IsMdiChildMaximized() {
			return XtraFormOwner != null && XtraFormOwner.ActiveMdiChild != null && XtraFormOwner.ActiveMdiChild.WindowState == FormWindowState.Maximized;
		}
		protected virtual bool AllowShowMdiChildMenu(Point pt) {
			return IsMdiChildMaximized() && MdiChildIconBounds.Contains(pt);
		}
		protected virtual void ShowMdiChildSystemMenu() {
			ShowSystemMenu(XtraFormOwner.ActiveMdiChild, XtraFormOwner.PointToScreen(Point.Empty));
		}
		public virtual bool OnMouseDown(MouseEventArgs e) {
			if(e.Button != MouseButtons.Left)
				return false;
			PressedButton = FormCaptionButtonKind.None;
			FormCaptionButton button = Buttons.GetButton(e, this);
			if(button != null) {
				if(!button.IsEnabled) return true;
				PressedButton = button.Kind;
				if(IsAllowButtonMessages) NativeMethods.SetCapture(Handle);
				return true;
			}
			if(AllowShowMdiChildMenu(e.Location)) {
				ShowMdiChildSystemMenu();
				return true;
			}
			return false;
		}
		public virtual bool OnMouseUp(MouseEventArgs e) {
			FormCaptionButton button = Buttons.GetButton(e, this);
			FormCaptionButtonKind prev = PressedButton;
			PressedButton = FormCaptionButtonKind.None;
			if(prev == FormCaptionButtonKind.None) return false;
			if(IsAllowButtonMessages) NativeMethods.ReleaseCapture();
			if(button != null && button.Kind == prev && e.Button == MouseButtons.Left) {
				OnClick(e, button.GetActionCore(Handle, button.Kind));
			}
			return true;
		}
		protected virtual void OnClick(MouseEventArgs e, FormCaptionButtonAction kind) {
			int cmd = -1;
			switch(kind) {
				case FormCaptionButtonAction.Close: cmd = NativeMethods.SC.SC_CLOSE; break;
				case FormCaptionButtonAction.Restore: cmd = NativeMethods.SC.SC_RESTORE; break;
				case FormCaptionButtonAction.Maximize: cmd = NativeMethods.SC.SC_MAXIMIZE; break;
				case FormCaptionButtonAction.Minimize: cmd = NativeMethods.SC.SC_MINIMIZE; break;
				case FormCaptionButtonAction.Help: cmd = NativeMethods.SC.SC_CONTEXTHELP; break;
				case FormCaptionButtonAction.MdiRestore: cmd = NativeMethods.SC.SC_RESTORE; break;
				case FormCaptionButtonAction.MdiClose: cmd = NativeMethods.SC.SC_CLOSE; break;
				case FormCaptionButtonAction.MdiMinimize: cmd = NativeMethods.SC.SC_MINIMIZE; break;
			}
			if(cmd == -1) return;
			IntPtr h = IsMdiButton(kind) ? XtraFormOwner.ActiveMdiChild.Handle : Handle;
			PostSysCommandMessage(h, cmd);
		}
		void PostSysCommandMessage(IntPtr handle, IntPtr cmd) {
			int intCmd = (IntPtr.Size == 8) ? (int)cmd.ToInt64() : cmd.ToInt32();
			PostSysCommand(XtraFormOwner, handle, intCmd);
		}
		void PostSysCommandMessage(IntPtr handle, int cmd) {
			PostSysCommand(XtraFormOwner, handle, cmd);
		}
		public static void PostSysCommand(Form owner, IntPtr handle, int cmd) {
			MdiClient client = GetMdiClient(owner);
			if(client != null) {
				DevExpress.Utils.Mdi.ISysCommandListener listener =
					DevExpress.Utils.Mdi.MdiClientSubclasserService.FromMdiClient(client) as DevExpress.Utils.Mdi.ISysCommandListener;
				if(listener != null)
					listener.PreviewMessage(handle, cmd);
			}
			NativeMethods.PostMessage(handle, MSG.WM_SYSCOMMAND, new IntPtr(cmd), IntPtr.Zero);
		}
		static MdiClient GetMdiClient(Form form) {
			if(form == null) return null;
			MdiClient result = form.Parent as MdiClient;
			if(result == null && form.MdiParent != null)
				result = DevExpress.Utils.Mdi.MdiClientSubclasserService.GetMdiClient(form.MdiParent);
			if(result == null && form.Owner != null)
				result = DevExpress.Utils.Mdi.MdiClientSubclasserService.GetMdiClient(form.Owner);
			return result;
		}
		private bool IsMdiButton(FormCaptionButtonAction kind) {
			return kind == FormCaptionButtonAction.MdiClose || kind == FormCaptionButtonAction.MdiMinimize || kind == FormCaptionButtonAction.MdiRestore;
		}
		public virtual bool AllowResize {
			get {
				Form form = Owner as Form;
				return form == null ||
					(form.FormBorderStyle == FormBorderStyle.Sizable ||
					form.FormBorderStyle == FormBorderStyle.SizableToolWindow);
			}
		}
		public int WMNCHitTestResize(Point p, int hitTestCurrent) {
			int res = hitTestCurrent;
			if(AllowResize) {
				Rectangle bottom = FrameBottom;
				Rectangle frameTop = FrameTop;
				if(frameTop.Height < 2) frameTop.Height = 3 - frameTop.Height;
				if(bottom.Height < 2) { bottom.Height += 2; bottom.Y -= 2; }
				if(frameTop.Contains(p) && !IsXtraFormMaximized) res = NativeMethods.HT.HTTOP;
				if(FrameLeft.Contains(p)) res = NativeMethods.HT.HTLEFT;
				if(FrameRight.Contains(p)) res = NativeMethods.HT.HTRIGHT;
				if(bottom.Contains(p)) res = NativeMethods.HT.HTBOTTOM;
				if(new Rectangle(FrameLeft.X, bottom.Y, 16, bottom.Height).Contains(p)) res = NativeMethods.HT.HTBOTTOMLEFT;
				if(new Rectangle(FrameRight.Right - 16, bottom.Y, 16, bottom.Height).Contains(p)) res = NativeMethods.HT.HTBOTTOMRIGHT;
				if(new Rectangle(0, 0, 16, frameTop.Height).Contains(p)) res = NativeMethods.HT.HTTOPLEFT;
				if(new Rectangle(FrameRight.Right - 16, 0, 16, frameTop.Height).Contains(p)) res = NativeMethods.HT.HTTOPRIGHT;
			}
			return res;
		}
		protected internal bool ShouldInvertButtons {
			get { return IsRightToLeftLayout && IsRibbonForm; }
		}
		protected bool IsRibbonForm {
			get {
				if(XtraFormOwner == null) return false;
				return XtraFormOwner.IsRibbonForm;
			}
		}
		protected virtual void WMNCHitTest(ref Message msg) {
			Point p = PointToFormBounds(msg.LParam);
			Point clientPoint = p;
			if(IsRightToLeftLayout && !IsRibbonForm) p.X = Owner.Width - p.X;
			int res = NativeMethods.HT.HTNOWHERE;
			Rectangle client = Owner.ClientRectangle;
			client.Offset(FrameLeft.Width, CaptionBounds.Bottom);
			if(client.Contains(p)) res = NativeMethods.HT.HTCLIENT;
			if(CaptionBounds.Contains(p)) res = NativeMethods.HT.HTCAPTION;
			res = WMNCHitTestResize(clientPoint, res);
			if(IsXtraFormMaximized) {
				if(!IconBounds.IsEmpty && CaptionBounds.Contains(p)) {
					if(!IsRibbonForm || !IsRightToLeftLayout) {
						if(IconBounds.Right > p.X) res = NativeMethods.HT.HTSYSMENU;
					}
					else {
						if(p.X > IconBounds.X) res = NativeMethods.HT.HTSYSMENU;
					}
				}
			}
			else if(IconBounds.Contains(p)) {
				res = NativeMethods.HT.HTSYSMENU;
			}
			FormCaptionButton button = Buttons.GetButton(p);
			if(button != null)
				res = Action2Ht(button.Action);
			p = Owner.PointToClient(new Point((int)msg.LParam.ToInt64()));
			if(SizeGripBounds.Contains(p) && RenderSizeGrip) {
				if(Owner.IsMirrored) res = NativeMethods.HT.HTBOTTOMLEFT;
				else res = NativeMethods.HT.HTBOTTOMRIGHT;
			}
			msg.Result = new IntPtr(res);
		}
		internal int Action2Ht(FormCaptionButtonAction action) {
			if(action == FormCaptionButtonAction.Close) return DevExpress.Utils.Drawing.Helpers.NativeMethods.HT.HTCLOSE;
			if(action == FormCaptionButtonAction.Minimize) return DevExpress.Utils.Drawing.Helpers.NativeMethods.HT.HTMINBUTTON;
			if(action == FormCaptionButtonAction.Maximize) return DevExpress.Utils.Drawing.Helpers.NativeMethods.HT.HTMAXBUTTON;
			if(action == FormCaptionButtonAction.Restore) return DevExpress.Utils.Drawing.Helpers.NativeMethods.HT.HTMAXBUTTON;
			if(action == FormCaptionButtonAction.Help) return DevExpress.Utils.Drawing.Helpers.NativeMethods.HT.HTHELP;
			return DevExpress.Utils.Drawing.Helpers.NativeMethods.HT.HTBORDER;
		}
		bool shouldUseSmallButtons = false;
		protected internal bool ShouldUseSmallButtons {
			get { return shouldUseSmallButtons; }
			set { shouldUseSmallButtons = value; }
		}
		protected internal virtual bool IsXtraFormMaximized {
			get {
				if(XtraFormOwner == null) return false;
				NativeMethods.ShowWindowCommands command = GetShowWindowCommand();
				return command == NativeMethods.ShowWindowCommands.ShowMaximized || command == NativeMethods.ShowWindowCommands.Maximize;
			}
		}
		public virtual SkinPaddingEdges Margins {
			get {
				CheckReady();
				if(margins == null) margins = GetMargins();
				return margins;
			}
		}
		protected virtual bool ShouldAddMaxMinButtons(Form form) {
			return form.FormBorderStyle != FormBorderStyle.FixedToolWindow && form.FormBorderStyle != FormBorderStyle.SizableToolWindow;
		}
		protected virtual bool ShouldAddMinimizeButton(Form form) {
			return ShouldAddMaxMinButtons(form);
		}
		protected virtual bool ShouldAddMaximizeButton(Form form) {
			return ShouldAddMaxMinButtons(form);
		}
		protected virtual FormCaptionButtonKind GetVisibleButtons() {
			Form form = Owner as Form;
			if(form == null) return FormCaptionButtonKind.All;
			FormCaptionButtonKind res = FormCaptionButtonKind.None;
			if(ShouldShowMdiBar) res |= FormCaptionButtonKind.AllMdi;
			if(!form.ControlBox) return res;
			if(ShouldAddCloseButton(form)) res |= FormCaptionButtonKind.Close;
			if(IsToolWindow || form.FormBorderStyle == FormBorderStyle.FixedToolWindow || form.FormBorderStyle == FormBorderStyle.SizableToolWindow) return res;
			if(form.MaximizeBox && ShouldAddMaximizeButton(form)) res |= FormCaptionButtonKind.Maximize;
			if(form.MinimizeBox && ShouldAddMinimizeButton(form)) res |= FormCaptionButtonKind.Minimize;
			if(form.HelpButton && ShouldAddHelpButton(form)) res |= FormCaptionButtonKind.Help;
			return res;
		}
		protected virtual bool ShouldAddHelpButton(Form form) {
			return !form.MinimizeBox && !form.MaximizeBox;
		}
		protected virtual bool ShouldAddCloseButton(Form form) {
			DevExpress.XtraEditors.XtraForm xf = form as DevExpress.XtraEditors.XtraForm;
			return xf == null || xf.CloseBox;
		}
		protected virtual bool IsAllowButtonMessages { get { return true; } }
		protected virtual bool AllowDrawEmptyText { 
			get {
				DevExpress.XtraEditors.XtraForm form = Owner as DevExpress.XtraEditors.XtraForm;
				if(form == null) return false;
				return form.AllowSkinForEmptyText;
			}
		}
		protected virtual bool IsDrawCaption {
			get {
				if(!IsAllowNCDraw) return false;
				Form form = Owner as Form;
				if(form != null &&
					(form.FormBorderStyle == FormBorderStyle.None || (!form.ControlBox && form.Text == "" && !AllowDrawEmptyText))) {
					return false;
				}
				return true;
			}
		}
		protected internal bool IsRightToLeftLayout {
			get { return WindowsFormsSettings.GetIsRightToLeftLayout(Owner); }
		}
		protected int captionHeight = 0;
		Rectangle iconBounds = Rectangle.Empty, textBounds = Rectangle.Empty;
		protected virtual bool IsReady { 
			get { return isReady; }
			set { isReady = value; }
		}
		protected virtual void CheckReady() {
			if(IsReady) return;
			this.isReady = true;
			ResetDefaultAppearance();
			GInfo.AddGraphics(null);
			try {
				this.textBounds = Rectangle.Empty;
				Size icon = Size.Empty;
				if(IsDrawCaption) {
					this.iconBounds = Rectangle.Empty;
					this.mdiBarHeight = CalcMdiBarHeight(GInfo);
					Buttons.Clear();
					AppearanceObject appearance = new AppearanceObject(GetDefaultAppearance());
					Buttons.CreateButtons(GetVisibleButtons());
					ShouldUseSmallButtons = false;
					Size buttons = Buttons.CalcSize(GInfo.Graphics, ButtonPainter);
					icon = GetIconSize();
					captionHeight = Math.Max(buttons.Height, Buttons.CalcMinSize(GInfo.Graphics, ButtonPainter).Height);
					captionHeight = Math.Max(captionHeight, this.lastTextHeight = CalcTextHeight(GInfo.Graphics, appearance));
					captionHeight = Math.Max(captionHeight, icon.Height);
					captionHeight = ObjectPainter.CalcBoundsByClientRectangle(null, SkinElementPainter.Default, new SkinElementInfo(GetSkinCaption()), new Rectangle(0, 0, 10, captionHeight)).Height;
				} else {
					captionHeight = 0;
				}
				this.margins = GetMargins();
				this.zoomedMargins = null;
				if(!IsDrawCaption) return;
				Rectangle captionClient = GetCaptionClient(CaptionBounds);
				if(NativeMethods.IsZoomed(Handle)) {
					SkinPaddingEdges zoomed = GetZoomedMargins();
					captionClient.X += zoomed.Left;
					captionClient.Width -= zoomed.Width;
				}
				ShouldUseSmallButtons = IsXtraFormMaximized;
				Rectangle restBounds = CalcButtonsBounds(captionClient);
				if(!icon.IsEmpty) {
					if(restBounds.Height >= icon.Height && restBounds.Width >= icon.Width) {
						this.iconBounds = restBounds;
						if(Owner.IsHandleCreated) {
							if(IsRightToLeftLayout)
								this.iconBounds.X = FormBounds.Right - Owner.PointToScreen(new Point(0, 0)).X + IconLeftMargin;
							else
								this.iconBounds.X = Owner.PointToScreen(new Point(0, 0)).X - FormBounds.X + IconLeftMargin;
						}
						else {
							if(IsRightToLeftLayout)
								this.iconBounds.X = captionClient.Right - (IconLeftMargin + icon.Width);
						}
						this.iconBounds.Width = icon.Width;
						this.iconBounds = RectangleHelper.GetCenterBounds(this.iconBounds, icon);
						if(this.iconBounds.Right + IconToCaptionIndent > restBounds.X) {
							restBounds.Width -= (IconBounds.Right + IconToCaptionIndent - restBounds.X);
							restBounds.X = (IconBounds.Right + IconToCaptionIndent);
							if(IsRightToLeftLayout) restBounds.X--;
						}
					}
				}
				if(restBounds.Width > 0)
					this.textBounds = restBounds;
				Buttons.CalcMdiBounds(null, ButtonPainter, GetMdiBarClient(MdiBarBounds));
			} finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected virtual Rectangle CalcButtonsBounds(Rectangle captionClient) {
			return Buttons.CalcBounds(null, ButtonPainter, captionClient);
		}
		int lastTextHeight = 0;
		void CheckFormHeightChanged() {
			GInfo.AddGraphics(null);
			try {
				int height = CalcTextHeight(GInfo.Graphics, new AppearanceObject(GetDefaultAppearance()));
				if(height != lastTextHeight) {
					this.lastTextHeight = height;
					ForceFrameChanged();
				}
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected virtual int CalcTextHeight(Graphics graphics, AppearanceObject appearance) {
			if(AllowHtmlDraw) {
				return CalcHtmlTextHeight(graphics, appearance);
			}
			return Math.Max(appearance.CalcDefaultTextSize(graphics).Height, (int)appearance.CalcTextSize(graphics, Text, 0).Height);
		}
		protected virtual int CalcHtmlTextHeight(Graphics graphics, AppearanceObject appearance) {
			if(string.IsNullOrEmpty(Text)) return appearance.CalcDefaultTextSize(graphics).Height;
			return StringPainter.Default.Calculate(graphics, appearance, null, Text, 0, null, HtmlContext).Bounds.Height;
		}
		protected virtual Rectangle GetMdiBarClient(Rectangle mdiBarBounds) {
			return ObjectPainter.GetObjectClientRectangle(null, SkinElementPainter.Default, GetMdiBarInfo(mdiBarBounds));
		}
		protected internal virtual Rectangle GetCaptionClient(Rectangle captionBounds) {
			Rectangle rect = ObjectPainter.GetObjectClientRectangle(null, SkinElementPainter.Default, new SkinElementInfo(GetSkinCaption(), captionBounds));
			int bottomMargin = captionBounds.Bottom - rect.Bottom;
			if(!IsXtraFormMaximized || (XtraFormOwner.Parent != null && !XtraFormOwner.TopLevel))
				return rect;
			Screen sc = Screen.FromRectangle(FormBounds);
			int invisibleHeight = sc.WorkingArea.Y - FormBounds.Y;
			int delta = invisibleHeight - rect.Y;
			rect.Y += delta;
			rect.Height -= delta;
			rect.Height += bottomMargin;
			rect.Width = sc.WorkingArea.Right - FormBounds.X + (FormBounds.Right - sc.WorkingArea.Right) - rect.X - (captionBounds.Right - rect.Right);
			return rect;
		}
		protected virtual int IconToCaptionIndent { get { return 4; } }
		public void SetDirty() {
			this.isReady = false;
			this.margins = null;
		}
		public virtual bool IsToolWindow {
			get {
				Form frm = Owner as Form;
				if(frm == null) return false;
				if(frm.FormBorderStyle == FormBorderStyle.FixedToolWindow || frm.FormBorderStyle == FormBorderStyle.SizableToolWindow) return true;
				return false;
			}
		}
		protected Skin Skin { get { return FormSkins.GetSkin(Provider); } }
		protected SkinPaddingEdges GetMargins() {
			SkinPaddingEdges margins = new SkinPaddingEdges();
			margins.Left = ObjectPainter.CalcObjectMinBounds(null, SkinElementPainter.Default, new SkinElementInfo(GetSkinFrameLeft())).Width;
			margins.Right = ObjectPainter.CalcObjectMinBounds(null, SkinElementPainter.Default, new SkinElementInfo(GetSkinFrameRight())).Width;
			margins.Bottom = ObjectPainter.CalcObjectMinBounds(null, SkinElementPainter.Default, new SkinElementInfo(GetSkinFrameBottom())).Bottom;
			margins.Top = IsDrawFrameTop ? margins.Bottom : GetCaptionHeight();
			return margins;
		}
		protected virtual bool ShouldShowMdiBar { get { return XtraFormOwner != null && XtraFormOwner.ShouldShowMdiBar; } }
		protected virtual SkinElementInfo GetMdiBarInfo() { return GetMdiBarInfo(MdiBarBounds); }
		protected virtual SkinElementInfo GetMdiBarInfo(Rectangle bounds) {
			return new SkinElementInfo(FormSkins.GetSkin(Provider)[FormSkins.SkinFormMdiBar], bounds);
		}
		int mdiBarHeight = 0;
		protected internal virtual int MdiBarHeight { get { return mdiBarHeight; } }
		protected internal virtual Rectangle MdiBarBounds { get { return new Rectangle(FrameLeft.Width, CaptionBounds.Bottom, FrameBottom.Width - FrameLeft.Width - FrameRight.Width, MdiBarHeight); } }
		protected virtual int CalcMdiBarHeight(GraphicsInfo ginfo) {
			SkinElementInfo info = GetMdiBarInfo(Rectangle.Empty);
			return ObjectPainter.CalcObjectMinBounds(ginfo.Graphics, SkinElementPainter.Default, info).Height;
		}
		protected bool IsZoomedBorder {
			get {
				if(!IsZoomed) return false;
				DevExpress.XtraEditors.XtraForm frm = Owner as DevExpress.XtraEditors.XtraForm;
				return frm == null || !frm.IsMaximizedBoundsSet;
			}
		}
		internal SkinPaddingEdges zoomedMargins = null;
		protected bool IsZoomed {
			get {
				DevExpress.XtraEditors.XtraForm form = Owner as DevExpress.XtraEditors.XtraForm;
				if(form == null) return false;
				return Handle == IntPtr.Zero ? form.WindowState == FormWindowState.Maximized : NativeMethods.IsZoomed(Handle);
			}
		}
		protected SkinPaddingEdges GetTaskBarMargins() {
			SkinPaddingEdges res = new SkinPaddingEdges();
			if(!IsZoomedBorder) return res;
			int edge;
			if(Taskbar.IsAutoHide(out edge)) {
				res.Bottom += 5;
			}
			return res;
		}
		protected void FillInCreateParamsBorderStyles(CreateParams cp, FormBorderStyle borderStyle) {
			cp.Style |= WS_CAPTION;
			switch(borderStyle) {
				case FormBorderStyle.None:
					break;
				case FormBorderStyle.FixedSingle:
					break;
				case FormBorderStyle.Fixed3D:
					cp.Style |= 0x800000;
					cp.ExStyle |= 0x200;
					return;
				case FormBorderStyle.FixedDialog:
					cp.Style |= 0x800000;
					cp.ExStyle |= 1;
					return;
				case FormBorderStyle.Sizable:
					cp.Style |= 0x840000;
					return;
				case FormBorderStyle.FixedToolWindow:
					cp.Style |= 0x800000;
					cp.ExStyle |= 0x80;
					return;
				case FormBorderStyle.SizableToolWindow:
					cp.Style |= 0x840000;
					cp.ExStyle |= 0x80;
					return;
				default:
					return;
			}
			cp.Style |= 0x800000;
		}
		protected virtual SkinPaddingEdges GetZoomedMargins() {
			if(zoomedMargins != null) return zoomedMargins;
			this.zoomedMargins = new SkinPaddingEdges();
			DevExpress.XtraEditors.XtraForm form = Owner as DevExpress.XtraEditors.XtraForm;
			if(form == null) return zoomedMargins;
			if(!IsZoomedBorder) return zoomedMargins;
			CreateParams pars = new CreateParams();
			FillInCreateParamsBorderStyles(pars, form.FormBorderStyle);
			NativeMethods.RECT wr = new NativeMethods.RECT(Rectangle.Empty);
			NativeMethods.AdjustWindowRectEx(ref wr, pars.Style, false, pars.ExStyle);
			SkinPaddingEdges zm = new SkinPaddingEdges();
			zm.Left = Math.Abs(wr.Left) - Margins.Left;
			zm.Top = Math.Abs(wr.Top) - Margins.Top;
			if((DevExpress.Utils.WXPaint.WXPPainter.Default.ThemesEnabled && (zm.Top > 0)) || !IsMdi())
				zm.Top = NativeVista.IsWindows7? Math.Max(0, zm.Top) : 0;
			zm.Right = Math.Abs(wr.Right) - Margins.Right;
			zm.Bottom = Math.Abs(wr.Bottom) - Margins.Bottom;
			if(ShouldResetTopMargin) zm.Top = 0;
			this.zoomedMargins = zm;
			return this.zoomedMargins;
		}
		protected virtual bool ShouldResetTopMargin { get { return !IsDrawCaption; } }
		protected internal virtual bool IsZoomedMarginsReady { get { return zoomedMargins != null; } }
		protected virtual SkinElement GetSkinCaption() {
			if(IsWindowMinimized) return Skin[FormSkins.SkinFormCaptionMinimized];
			return Skin[IsToolWindow ? "Small" + FormSkins.SkinFormCaption : FormSkins.SkinFormCaption];
		}
		protected internal virtual SkinElement GetSkinFrameLeft() { return Skin[FormSkins.SkinFormFrameLeft]; }
		protected virtual SkinElement GetSkinFrameRight() { return Skin[FormSkins.SkinFormFrameRight]; }
		protected virtual SkinElement GetSkinFrameBottom() { return Skin[FormSkins.SkinFormFrameBottom]; }
		protected virtual SkinElement GetSkinFrameTop() { return GetSkinCaption(); }
		const int GCL_STYLE = -26;
		const int CS_NOCLOSE = 0x200;
		bool? closeButtonDisabled;
		protected internal virtual bool IsCloseButtonDisabled {
			get {
				if(Owner == null || !Owner.IsHandleCreated)
					return false;
				if(!closeButtonDisabled.HasValue)
					closeButtonDisabled = ((NativeMethods.GetClassLong(Handle, GCL_STYLE) & CS_NOCLOSE) != 0);
				return closeButtonDisabled.Value;
			}
		}
		bool? isWindowActive = null;
		protected internal virtual bool IsWindowActive {
			get {
				if(DevExpress.XtraEditors.XtraForm.SuppressDeactivation && 
					(DevExpress.XtraEditors.XtraForm.DeactivatedForm == null ||
					DevExpress.XtraEditors.XtraForm.DeactivatedForm == XtraFormOwner))
					return true;
				if(!isWindowActive.HasValue) {
					if(Owner == null || !Owner.IsHandleCreated)
						return false;
					if(XtraFormOwner != null && XtraFormOwner.creatingHandle)
						return true;
					if(IsMdi()) {
						isWindowActive = XtraFormOwner.MdiParent.ActiveMdiChild == XtraFormOwner;
					} else {
						isWindowActive = Form.ActiveForm == FindParentForm();
					}
				}
				return isWindowActive.Value;
			}
			set {
				if(IsWindowActive == value) return;
				this.appDefault = null;
				isWindowActive = value;
				OnWindowActiveChanged();
			}
		}
		Form FindParentForm() {
			Control node = Owner;
			while(node.Parent != null) {
				node = node.Parent;
			}
			return node is Form ? (Form)node : null;
		}
		NativeMethods.WINDOWPLACEMENT windowPlacement = new NativeMethods.WINDOWPLACEMENT();
		NativeMethods.ShowWindowCommands GetShowWindowCommand() {
			if(XtraFormOwner == null || !XtraFormOwner.IsHandleCreated || !XtraFormOwner.Visible)
				return NativeMethods.ShowWindowCommands.Normal;
			this.windowPlacement.Length = Marshal.SizeOf(this.windowPlacement);
			NativeMethods.GetWindowPlacement(XtraFormOwner.Handle, out this.windowPlacement);
			return this.windowPlacement.ShowCmd;
		}
		protected internal virtual bool IsWindowMinimized {
			get {
				if(XtraFormOwner == null) return false;
				NativeMethods.ShowWindowCommands command = GetShowWindowCommand();
				return command == NativeMethods.ShowWindowCommands.ShowMinimized || command == NativeMethods.ShowWindowCommands.Minimize;
			}
		}
		protected virtual void OnWindowActiveChanged() { }
		protected virtual int GetCaptionHeight() { return captionHeight; }
		protected virtual bool WMSysCommand(ref Message msg) {
			if(msg.WParam.ToInt32() == NativeMethods.SC.SC_DRAGMOVE && IsMdiChild) {
				DrawFrameNC(msg);
				return false;
			}
			isReady = false;
			if(msg.WParam.ToInt32() == NativeMethods.SC.SC_MAXIMIZE || msg.WParam.ToInt32() == NativeMethods.SC.SC_RESTORE) {
				DoBaseWndProc(ref msg);
				DrawFrameNC(msg);
				return true;
			}
			return false;
		}
		protected virtual bool IsGlassForm {
			get {
				IGlassForm form = Owner as IGlassForm;
				return form != null && form.IsGlassForm;
			}
		}
		protected virtual bool IsMdiContainer { get { return XtraFormOwner != null && XtraFormOwner.IsMdiContainer; } }
		protected virtual bool IsMdiChild { get { return XtraFormOwner != null && XtraFormOwner.IsMdiChild; } }
		protected virtual bool NCActivate(ref Message msg) {
			if(msg.Msg == MSG.WM_NCACTIVATE) {
				IsWindowActive = msg.WParam != IntPtr.Zero;
				msg.Result = new IntPtr(1);
			}
			if(IsMdiContainer || IsMdi() || IsGlassForm) {
				BaseWithTrick(ref msg);
				if(XtraFormOwner.ActiveMdiChild != null) {
					NativeMethods.SendMessage(XtraFormOwner.ActiveMdiChild.Handle, msg.Msg, msg.WParam, msg.LParam);
				}
			}
			ResetStringInfo();
			DrawFrameNC(msg);
			UpdateMDIChildren();
			return true;
		}
		void ResetStringInfo() {
			this.info = null;
		}
		void UpdateMDIChildren() {
			if(!IsMdiContainer || XtraFormOwner == null) return;
			foreach(Form form in new ArrayList(XtraFormOwner.MdiChildren)) {
				InvalidateNC(form);
			}
		}
		protected virtual XtraEditors.XtraForm XtraFormOwner { get { return Owner as XtraEditors.XtraForm; } }
		protected virtual void BaseWithTrick(ref Message msg) {
			if(XtraFormOwner != null && !XtraFormOwner.SupportAdvancedTitlePainting) {
				if(msg.Msg == MSG.WM_NCACTIVATE)
					return;
				DoBaseWndProc(ref msg);
				return;
			}
			if(IsMdiContainer && msg.Msg != MSG.WM_SETCURSOR)
				System.Threading.Thread.Sleep(50);
			int style = NativeMethods.GetWindowLong(Handle, GWL_STYLE);
			NativeMethods.SetWindowLong(Handle, GWL_STYLE, style & (~WS_VISIBLE));
			try { DoBaseWndProc(ref msg); }
			finally { NativeMethods.SetWindowLong(Handle, GWL_STYLE, style); }
		}
		protected virtual void WMSetCursor(ref Message msg) {
			if(Form.ActiveForm != Owner && !IsMdi()) {
				DoBaseWndProc(ref msg);
				return;
			}
			BaseWithTrick(ref msg);
		}
		protected virtual bool WMSetText(ref Message msg) {
			this.text = null;
			BaseWithTrick(ref msg);
			DrawBordersCaption(msg, false, true, true);
			return true;
		}
		const int WS_CAPTION = 0x00C00000, WS_BORDER = 0x00800000, WS_DLGFRAME = 0x00400000, WS_VSCROLL = 0x00200000,
			WS_HSCROLL = 0x00100000, WS_SYSMENU = 0x00080000, WS_THICKFRAME = 0x00040000;
		protected virtual bool IsSuspendRedraw {
			get {
				DevExpress.XtraEditors.XtraForm form = Owner as DevExpress.XtraEditors.XtraForm;
				return form != null && form.IsSuspendRedraw;
			}
		}
		protected virtual void DrawBordersCaption(Message msg, bool drawBorders, bool drawCaption, bool doubleBuffer) {
			if(IsSuspendRedraw || !IsAllowNCDraw) return;
			if(Owner == null) return; 
			if(!Owner.IsHandleCreated || !NativeMethods.IsWindowVisible(Owner.Handle)) return;
			if(drawBorders && IsZoomedBorder) drawBorders = false;
			IntPtr dc = msg.Msg == MSG.WM_PRINT? msg.WParam: GetDC(msg);
			using(Graphics g = Graphics.FromHdc(dc)) {
				using(GraphicsCache cache = new GraphicsCache(g)) {
					if(drawCaption) DrawCaption(cache, doubleBuffer);
					if(drawBorders) DrawBorders(cache, true);
				}
			}
			if(msg.Msg != MSG.WM_PRINT)
				NativeMethods.ReleaseDC(Handle, dc);
		}
		protected void DrawFrameNC(Message msg) {
			DrawFrameNC(msg, true);
		}
		protected virtual void DrawFrameNC(Message msg, bool drawBorders) {
			DrawBordersCaption(msg, drawBorders, true, true);
		}
		protected virtual void RedrawButtons() {
			if(!IsReady) {
				InvalidateNC();
				return;
			}
			DrawFrameNC(new Message(), false);
		}
		protected virtual ObjectState CalcButtonState(FormCaptionButton button) {
			ObjectState res = ObjectState.Normal;
			if(button.Kind == FormCaptionButtonKind.Close && IsCloseButtonDisabled)
				res = ObjectState.Disabled;
			button.Inactive = !IsWindowActive;
			if(!button.Inactive) {
				if(button.Kind == HotButton) {
					res |= ObjectState.Hot;
					if(button.Kind == PressedButton) res |= ObjectState.Pressed;
				}
			}
			return res;
		}
		public virtual void DrawButtons(GraphicsCache cache, bool useClip) {
			foreach(FormCaptionButton button in Buttons) {
				button.UpdateAction(Handle);
				button.State = CalcButtonState(button);
			}
			FormCaptionButtonSkinPainter painter = (FormCaptionButtonSkinPainter)ButtonPainter;
			painter.CorrectImageFormRTL = GetCorrectButtonImagesInRTLMode();
			Buttons.Draw(cache, ButtonPainter, useClip);
		}
		protected virtual bool GetCorrectButtonImagesInRTLMode() {
			return XtraFormOwner != null && XtraFormOwner.RightToLeftLayout;
		}
		protected virtual Rectangle GetCaptionDrawBounds() {
			Rectangle rect = CaptionBounds;
			if(ShouldShowMdiBar)
				rect.Height += MdiBarHeight;
			return rect;
		}
		protected virtual void DrawCaption(GraphicsCache cache, bool doubleBuffer) {
			if(!IsDrawCaption) return;
			if(CaptionBounds.Width < 1 || CaptionBounds.Height < 1) return;
			if(doubleBuffer) {
				CheckReady();
				try {
					using(XtraBufferedGraphics bg = XtraBufferedGraphicsManager.Current.Allocate(cache.Graphics, GetCaptionDrawBounds())) {
						using(GraphicsCache tempCache = new GraphicsCache(bg.Graphics)) {
							DrawBackground(tempCache);
							if(ShouldShowMdiBar)
								DrawMdiBar(tempCache);
							DrawButtons(tempCache, false);
							DrawIcon(tempCache);
							DrawText(tempCache);
						}
						bg.Render();
					}
				}
				catch(Exception e) {
					if(ControlPaintHelper.IsCriticalException(e)) throw e;
					doubleBuffer = false;
				}
			} 
			if(!doubleBuffer) {
				DrawBackground(cache);
				if(ShouldShowMdiBar)
					DrawMdiBar(cache);
				DrawButtons(cache, false);
				DrawIcon(cache);
				DrawText(cache);
			}
		}
		protected virtual Rectangle GetMdiChildIconBounds(Icon ico) {
			Size sz = GetMdiChildIconSize(ico);
			int offset = (MdiBarHeight - sz.Height) / 2;
			return new Rectangle(new Point(MdiBarBounds.X + offset + 4, MdiBarBounds.Y + offset), sz);
		}
		protected virtual void DrawMdiBar(GraphicsCache cache) {
			SkinElementInfo backInfo = GetMdiBarInfo();
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, backInfo);
			DrawMdiBarIcon(cache);
			DrawFrame(cache, GetSkinFrameLeft(), FrameLeft, false, FrameKind.Left);
			DrawFrame(cache, GetSkinFrameRight(), FrameRight, false, FrameKind.Right);
		}
		protected virtual Size GetMdiChildIconSize(Icon ico) { return new Size(16, 16); }
		protected virtual Icon GetMdiChildIcon() {
			return IconHelper.GetSmallIcon(XtraFormOwner.ActiveMdiChild);
		}
		protected virtual Rectangle MdiChildIconBounds {
			get {
				Icon ico = GetMdiChildIcon();
				if(ico == null) return Rectangle.Empty;
				return GetMdiChildIconBounds(ico);
			}
		}
		protected virtual void DrawMdiBarIcon(GraphicsCache cache) {
			Icon ico = GetMdiChildIcon();
			if(ico == null) return;
			cache.Graphics.DrawIcon(ico, GetMdiChildIconBounds(ico));
		}
		protected virtual bool AllowCorrectCaptionImageInRTL { get { return true; } }
		protected SkinElementInfo GetCaptionInfo() {
			SkinElementInfo info = new SkinElementInfo(GetSkinCaption(), CaptionBounds);
			if(XtraFormOwner != null && AllowCorrectCaptionImageInRTL)
				info.CorrectImageFormRTL = XtraFormOwner.RightToLeftLayout;
			return info;
		}
		protected virtual void DrawBackground(GraphicsCache cache) {
			SkinElementInfo info = GetCaptionInfo();
			info.ImageIndex = IsWindowActive ? 0 : 1;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
		}
		AppearanceDefault appDefault;
		public virtual AppearanceDefault GetDefaultAppearance() {
			if(appDefault == null) {
				appDefault = GetSkinCaption().GetAppearanceDefault(Provider);
				if(appDefault.ForeColor == Color.Empty && !IsWindowMinimized && IsToolWindow) appDefault = FormSkins.GetSkin(Provider)[FormSkins.SkinFormCaption].GetAppearanceDefault(Provider);
				if(appDefault.ForeColor == Color.Empty) appDefault.ForeColor = LookAndFeelHelper.GetSystemColorEx(Provider, SystemColors.Control);
				if(!IsWindowActive)
					appDefault.ForeColor = FormSkins.GetSkin(Provider).Colors[FormSkins.OptInactiveColor];
			}
			return appDefault;
		}
		Color GetTextShadowColor() {
			if(!FormSkins.GetSkin(Provider).Colors.Contains(FormSkins.OptTextShadowColor)) return Color.Empty;
			return FormSkins.GetSkin(Provider).Colors[FormSkins.OptTextShadowColor];
		}
		public void ResetDefaultAppearance() {
			this.appDefault = null;
		}
		protected virtual bool AllowHtmlDraw { get { return XtraFormOwner != null && !string.IsNullOrEmpty(XtraFormOwner.HtmlText); } }
		protected virtual void DrawText(GraphicsCache cache) {
			string text = Text;
			if(text == null || text.Length == 0 || TextBounds.IsEmpty) return;
			AppearanceObject appearance = GetTextAppearance();
			if(AllowHtmlDraw) {
				DrawHtmlText(cache, appearance);
				return;
			}
			Rectangle r = RectangleHelper.GetCenterBounds(TextBounds, new Size(TextBounds.Width, CalcTextHeight(cache.Graphics, appearance)));
			DrawTextShadow(cache, appearance, r);
			cache.DrawString(text, appearance.Font, appearance.GetForeBrush(cache), r, appearance.GetStringFormat());
		}
		protected AppearanceObject GetTextAppearance() {
			AppearanceObject obj = new AppearanceObject(GetDefaultAppearance());
			obj.TextOptions.Trimming = Trimming.EllipsisCharacter;
			return obj;
		}
		StringInfo info = null;
		protected virtual void DrawHtmlText(GraphicsCache cache, AppearanceObject appearance) {
			if(info == null || info.originalBounds != TextBounds || info.SourceString != Text) {
				info = StringPainter.Default.Calculate(cache.Graphics, appearance, null, Text, TextBounds, null, HtmlContext);
			}
			StringPainter.Default.DrawString(cache, info);
		}
		protected virtual object HtmlContext { get { return this; } }
		protected virtual void DrawTextShadow(GraphicsCache cache, AppearanceObject appearance, Rectangle bounds) {
			if(!IsWindowActive) return;
			Color shadow = GetTextShadowColor();
			if(shadow == Color.Empty) return;
			Color prev = appearance.ForeColor;
			appearance.ForeColor = shadow;
			bounds.Offset(1, 1);
			cache.DrawString(text, appearance.Font, appearance.GetForeBrush(cache), bounds, appearance.GetStringFormat());
			appearance.ForeColor = prev;
		}
		protected virtual Icon GetIcon() {
			if(IsToolWindow || Handle == IntPtr.Zero) return null;
			Icon ico = iconImage as Icon;
			if(iconImage == null || (!IsIconReady(ico))) {
				iconImage = IconHelper.GetSmallIcon(Owner);
				if(!IsIconReady(iconImage as Icon)) iconImage = null;
				if(iconImage == null) iconImage = false;
			}
			return iconImage as Icon;
		}
		FieldInfo fiHandle;
		bool IsIconReady(Icon icon) {
			if(icon == null) return false;
			if(fiHandle == null) fiHandle = typeof(Icon).GetField("handle", BindingFlags.Instance | BindingFlags.NonPublic);
			if(fiHandle != null && ((IntPtr)fiHandle.GetValue(icon)) != IntPtr.Zero && icon.Width > 0 && icon.Height > 0) return true;
			return false;
		}
		protected bool HasIcon { get { return GetIcon() != null; } }
		public virtual void DrawIcon(GraphicsCache cache) {
			Icon icon = GetIcon();
			if(this.iconBounds.IsEmpty || icon == null) return;
			cache.Graphics.DrawIcon(icon, IconBounds);
		}
		protected Rectangle FormBounds {
			get {
				if(Handle == IntPtr.Zero) return new Rectangle(0, 0, Owner.Bounds.Width, Owner.Bounds.Height);
				NativeMethods.RECT r = new NativeMethods.RECT();
				NativeMethods.GetWindowRect(Handle, ref r);
				return r.ToRectangle();
			}
		}
		protected Rectangle IconBounds { get { return iconBounds; } set { iconBounds = value; } }
		protected Rectangle TextBounds { get { return textBounds; } set { textBounds = value; } }
		protected internal virtual Rectangle CaptionBounds { 
			get {
				return new Rectangle(0, GetZoomedMargins().Top, FormBounds.Width, Margins.Top);
			} 
		}
		protected virtual Rectangle FrameTop { get { return new Rectangle(0, 0, FormBounds.Width, Margins.Bottom); } }
		protected virtual Rectangle FrameLeft { get { return new Rectangle(0, Margins.Top, Margins.Left, FormBounds.Height - (Margins.Top + Margins.Bottom)); } }
		protected virtual Rectangle FrameRight { get { return new Rectangle(FormBounds.Width - Margins.Right, Margins.Top, Margins.Right, FormBounds.Height - (Margins.Top + Margins.Bottom)); } }
		protected virtual Rectangle FrameBottom { get { return new Rectangle(0, FormBounds.Height - Margins.Bottom, FormBounds.Width, Margins.Bottom); } }
		protected Rectangle FrameBottomPaintBounds {
			get {
				Rectangle caption = CaptionBounds;
				Rectangle res = FrameBottom;
				if(caption.IsEmpty || res.Top >= CaptionBounds.Bottom) return res;
				int delta = caption.Bottom - res.Y;
				res.Y += delta;
				res.Height -= delta;
				if(res.Height < 1) res = Rectangle.Empty;
				return res;
			}
		}
		void DrawBorders(GraphicsCache cache) { DrawBorders(cache, false); }
		protected virtual bool IsDrawFrameTop { get { return XtraFormOwner != null && XtraFormOwner.FormBorderStyle != FormBorderStyle.None && !IsDrawCaption; } }
		protected virtual void DrawBorders(GraphicsCache cache, bool doubleBuffer) {
			if(!IsAllowNCDraw) return;
			if(IsDrawFrameTop)
				DrawFrame(cache, GetSkinFrameTop(), FrameTop, doubleBuffer, FrameKind.Top);
			DrawFrame(cache, GetSkinFrameLeft(), FrameLeft, doubleBuffer, FrameKind.Left);
			DrawFrame(cache, GetSkinFrameRight(), FrameRight, doubleBuffer, FrameKind.Right);
			DrawFrame(cache, GetSkinFrameBottom(), FrameBottomPaintBounds, doubleBuffer, FrameKind.Bottom);
		}
		void DrawFrame(GraphicsCache cache, SkinElement element, Rectangle bounds, bool doubleBuffer, FrameKind kind) {
			if(bounds.Width < 1 || bounds.Height < 1) return;
			SkinElementInfo info = new SkinElementInfo(element, bounds);
			info.ImageIndex = IsWindowActive ? 0 : 1;
			if(XtraFormOwner != null)
				info.CorrectImageFormRTL = XtraFormOwner.RightToLeftLayout;
			if(doubleBuffer) {
				try {
					using(XtraBufferedGraphics bg = XtraBufferedGraphicsManager.Current.Allocate(cache.Graphics, bounds)) {
						using(GraphicsCache tempCache = new GraphicsCache(bg.Graphics)) {
							DrawFrameCore(tempCache, info, kind);
						}
						bg.Render();
					}
					return;
				}
				catch(Exception e) {
					if(ControlPaintHelper.IsCriticalException(e)) throw e;
				}
			}
			DrawFrameCore(cache, info, kind);
		}
		protected virtual void DrawFrameCore(GraphicsCache cache, SkinElementInfo info, FrameKind kind) {
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
		}
		protected internal IntPtr GetDC(Message msg) { return GetDC(Handle, msg); }
		public static IntPtr GetDC(IntPtr handle, Message msg) {
			IntPtr res = IntPtr.Zero;
			if(msg.Msg == MSG.WM_NCPAINT) {
				int flags = NativeMethods.DC.DCX_CACHE | NativeMethods.DC.DCX_CLIPSIBLINGS | NativeMethods.DC.DCX_WINDOW | NativeMethods.DC.DCX_VALIDATE;
				IntPtr hrgnCopy = IntPtr.Zero;
				if(msg.WParam.ToInt32() != 1) {
					flags |= NativeMethods.DC.DCX_INTERSECTRGN;
					hrgnCopy = NativeMethods.CreateRectRgn(0, 0, 1, 1);
					NativeMethods.CombineRgn(hrgnCopy, msg.WParam, IntPtr.Zero, NativeMethods.RGN_COPY);
				}
				res = NativeMethods.GetDCEx(handle, hrgnCopy, flags);
				return res;
			}
			return NativeMethods.GetWindowDC(handle);
		}
		bool IsMdi() {
			Form form = XtraFormOwner;
			if(form == null)
				return false;
			return form.MdiParent != null;
		}
		IntPtr lastHackedChildHandle = IntPtr.Zero;
		bool? lastShowMdiBar = null;
		protected virtual bool IsSizeableToolWindowOnSingleScreen {
			get {
				return IsZoomedBorder && XtraFormOwner.FormBorderStyle == FormBorderStyle.SizableToolWindow && Screen.AllScreens.Length == 1;
			}
		}
		NativeMethods.ShowWindowCommands? PrevShowCommand { get; set; }
		void UpdateFormByShowCommand() {
			if(PrevShowCommand.HasValue) {
				NativeMethods.ShowWindowCommands current = GetShowWindowCommand();
				if(PrevShowCommand != current) {
					SetDirty();
					CheckReady();
					ResetDefaultAppearance();
				}
				PrevShowCommand = current;
			} else {
				PrevShowCommand = GetShowWindowCommand();
			}
		}
		protected void ResetZoomedMargins() { this.zoomedMargins = null; }
		[SecuritySafeCritical]
		protected virtual void NCCalcSize(ref Message m) {
			UpdateFormByShowCommand();
			if(m.WParam == new IntPtr(1)) {
				NativeMethods.NCCALCSIZE_PARAMS csp = (NativeMethods.NCCALCSIZE_PARAMS)Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.NCCALCSIZE_PARAMS));
				if(WXPPainter.Default.ThemesEnabled)
					DoBaseWndProc(ref m);
				if(IsZoomedBorder) {
					this.zoomedMargins = null;
					SkinPaddingEdges zMargins = GetZoomedMargins();
					csp.rgrc0.Left += zMargins.Left;
					csp.rgrc0.Right -= zMargins.Right;
					csp.rgrc0.Bottom -= zMargins.Bottom;
					csp.rgrc0.Top += zMargins.Top;
					if(!IsDrawCaption) {
						if(IsMdi() && csp.rgrc0.Top < 0) csp.rgrc0.Top = 0;
							Taskbar.CorrectRECTByAutoHide(ref csp.rgrc0);
					}
				}
				if(IsWindowMinimized) {
					csp.rgrc0.Right = csp.rgrc0.Left;
					csp.rgrc0.Bottom = csp.rgrc0.Top;
				}
				else {
					csp.rgrc0.Top += Margins.Top;
					if(ShouldShowMdiBar)
						csp.rgrc0.Top += MdiBarHeight;
					csp.rgrc0.Bottom -= Margins.Bottom;
					csp.rgrc0.Left += Margins.Left;
					csp.rgrc0.Right -= Margins.Right;
					if(IsSizeableToolWindowOnSingleScreen) {
						Screen sc = Screen.FromControl(XtraFormOwner);
						csp.rgrc0.Right = sc.WorkingArea.Right;
					}
				}
				if(IsMdi())
					BaseWndProc(ref m);
				ForceUpdateMdiChild();
				Marshal.StructureToPtr(csp, m.LParam, false);
				m.Result = IntPtr.Zero;
			} else {
				NativeMethods.RECT rect = (NativeMethods.RECT)Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.RECT));
				if(IsZoomedBorder) {
					this.zoomedMargins = null;
					SkinPaddingEdges zMargins = GetZoomedMargins();
					rect.Left += zMargins.Left;
					rect.Right -= zMargins.Right;
					rect.Top += zMargins.Top;
					rect.Bottom -= zMargins.Bottom;
				}
				if(IsWindowMinimized) {
					rect.Bottom = rect.Top;
					rect.Right = rect.Left;
				}
				else {
					rect.Top += Margins.Top;
					if(ShouldShowMdiBar)
						rect.Top += MdiBarHeight;
					rect.Bottom -= Margins.Bottom;
					rect.Left += Margins.Left;
					rect.Right -= Margins.Right;
				}
				if(IsMdi())
					BaseWndProc(ref m);
				Marshal.StructureToPtr(rect, m.LParam, false);
				m.Result = IntPtr.Zero;
			}
		}
		protected internal virtual Size CalcSizeFromClientSize(Size client) {
			client.Width += Margins.Left + Margins.Right;
			client.Height += Margins.Bottom + Margins.Top;
			if(IsZoomedBorder) {
				SkinPaddingEdges zMargins = GetZoomedMargins();
				client.Width += zMargins.Left + zMargins.Right;
				client.Height += zMargins.Top + zMargins.Bottom;
			}
			if(Handle == IntPtr.Zero) SetDirty();
			return client;
		}
		internal static void ForceFrameChanged(Control control) {
			if(control == null || !control.IsHandleCreated || !control.Visible) return;
			NativeMethods.SetWindowPos(control.Handle, IntPtr.Zero, 0, 0, 0, 0,
				NativeMethods.SWP.SWP_NOMOVE | NativeMethods.SWP.SWP_NOSIZE | NativeMethods.SWP.SWP_NOZORDER |
				NativeMethods.SWP.SWP_NOACTIVATE | NativeMethods.SWP.SWP_FRAMECHANGED | NativeMethods.SWP.SWP_DRAWFRAME);
		}
		bool lockFrameChanged = false;
		void FrameChanged() {
			if(!Owner.IsHandleCreated || !Owner.Visible) return;
			if(this.lockFrameChanged) return;
			this.lockFrameChanged = true;
			try {
				NativeMethods.SetWindowPos(Handle, IntPtr.Zero, 0, 0, 0, 0,
					NativeMethods.SWP.SWP_NOMOVE | NativeMethods.SWP.SWP_NOSIZE | NativeMethods.SWP.SWP_NOZORDER | NativeMethods.SWP.SWP_NOACTIVATE |
					NativeMethods.SWP.SWP_FRAMECHANGED | NativeMethods.SWP.SWP_DRAWFRAME);
			} finally {
				this.lockFrameChanged = false;
			}
		}
		internal void ForceFrameChanged() {
			SetDirty();
			FrameChanged();
		}
		internal void InvalidateNC() {
			if(Owner == null || !Owner.IsHandleCreated || !Owner.Visible) return;
			InvalidateNC(Handle);
		}
		public static void InvalidateNC(Control control) {
			if(control == null || !control.IsHandleCreated || !control.Visible) return;
			InvalidateNC(control.Handle);
		}
		public static void InvalidateNC(IntPtr handle) {
			NativeMethods.SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0,
				NativeMethods.SWP.SWP_NOMOVE | NativeMethods.SWP.SWP_NOSIZE | NativeMethods.SWP.SWP_NOZORDER | NativeMethods.SWP.SWP_NOACTIVATE |
				NativeMethods.SWP.SWP_DRAWFRAME | NativeMethods.SWP.SWP_NOSENDCHANGING);
		}
		const int WS_VISIBLE = 0x10000000
										  ;
		const int GWL_STYLE = -16;
		#region IStringImageProvider Members
		Image IStringImageProvider.GetImage(string id) {
			if(XtraFormOwner == null || XtraFormOwner.HtmlImages == null) return null;
			return XtraFormOwner.HtmlImages.Images[id];
		}
		#endregion
		#region IServiceProvider Members
		object IServiceProvider.GetService(Type serviceType) {
			if(serviceType == typeof(IStringImageProvider)) return this;
			if(serviceType == typeof(ISite)) return Owner == null ? null : Owner.Site;
			return null;
		}
		#endregion
		ISkinProviderEx ProviderEx { get { return this.provider as ISkinProviderEx; } }
		bool ISkinProviderEx.GetTouchUI() {
			if(ProviderEx == null)
				return false;
			return ProviderEx.GetTouchUI();
		}
		float ISkinProviderEx.GetTouchScaleFactor() {
			if(ProviderEx == null)
				return 1.0f;
			return ProviderEx.GetTouchScaleFactor();
		}
		Color ISkinProviderEx.GetMaskColor() {
			if(ProviderEx == null)
				return Color.Empty;
			return ProviderEx.GetMaskColor();
		}
		Color ISkinProviderEx.GetMaskColor2() {
			if(ProviderEx == null)
				return Color.Empty;
			return ProviderEx.GetMaskColor2();
		}
		internal void ResetRegionRect() {
			this.regionRect = new Rectangle(-10000, -10000, 10000, 10000);
		}
		protected internal virtual bool IsRightToLeft { get { return XtraFormOwner != null && XtraFormOwner.IsRightToLeftCaption; } }
		protected internal virtual int FormWidth { get { return XtraFormOwner != null ? XtraFormOwner.Width : FrameBottom.Width; } }
	}
	public class IconHelper {
		public static IntPtr GetSmallIconHandle(IntPtr hWnd) {
			try {
				int result = NativeMethods.SendMessage(hWnd, WM_GETICON, new IntPtr(ICON_SMALL), IntPtr.Zero);
				IntPtr hIcon = new IntPtr(result);
				if(hIcon == IntPtr.Zero) {
					result = NativeMethods.GetClassLong(hWnd, GCL_HICONSM);
					hIcon = new IntPtr(result);
				}
				if(hIcon == IntPtr.Zero) {
					hIcon = new IntPtr(NativeMethods.SendMessage(hWnd, WM_QUERYDRAGICON, IntPtr.Zero, IntPtr.Zero));
				}
				return hIcon;
			} catch { }
			return IntPtr.Zero;
		}
		public static Icon GetSmallIcon(Control owner) {
			if(!owner.IsHandleCreated) 
				return null;
			Form form = owner as Form;
			if(form != null) {
				if(!form.ControlBox) return null;
#if DXWhidbey
				if(!form.ShowIcon) return null;
#endif
			}
			IntPtr hSmallIcon = GetSmallIconHandle(owner.Handle);
			if(hSmallIcon == IntPtr.Zero && (form != null) && form.FormBorderStyle != FormBorderStyle.FixedDialog && (form.Icon != null)) {
				return new Icon(form.Icon, 16, 16);
			}
			if(hSmallIcon != IntPtr.Zero) {
				using(Icon smallIcon = Icon.FromHandle(hSmallIcon)) {
					if(smallIcon.Size == Size.Empty)
						return (form.Icon != null) ? new Icon(form.Icon, 16, 16) : null;
					return new Icon(smallIcon, smallIcon.Size);
				}
			}
			return null;
		}
		const int
			WM_GETICON = 0x007F,
			WM_QUERYDRAGICON = 0x0037,
			ICON_SMALL = 0,
			GCL_HICONSM = (-34);
	}
	[Flags]
	public enum FormCaptionButtonKind {
		None = 0, Minimize = 0x1, Maximize = 0x2, Help = 0x4, Close = 0x8, MdiClose = 0x10, MdiRestore = 0x20, MdiMinimize = 0x40, FullScreen = 0x80,
		All = Minimize | Maximize | Help | Close,
		AllMdi = MdiClose | MdiMinimize | MdiRestore
	}
	public enum FormCaptionButtonAction {
		None, Minimize, Maximize, Help, Close, Restore, MdiMinimize, MdiRestore, MdiClose, FullScreen
	}
	public class FormCaptionButton : ObjectInfoArgs {
		FormCaptionButtonKind kind;
		FormCaptionButtonAction action;
		bool inactive;
		public FormCaptionButton(FormCaptionButtonKind kind) {
			this.kind = kind;
			this.inactive = false;
			UpdateAction(IntPtr.Zero);
		}
		public FormCaptionButtonKind Kind { get { return kind; } set { kind = value; } }
		public FormCaptionButtonAction Action { get { return action; } }
		public bool Inactive { get { return inactive; } set { inactive = value; } }
		public bool IsEnabled { get { return State != ObjectState.Disabled; } }
		public void UpdateAction(IntPtr handle) {
			action = GetActionCore(handle, kind);
		}
		protected internal virtual FormCaptionButtonAction GetActionCore(IntPtr handle, FormCaptionButtonKind kind) {
			return GetAction(handle, kind);
		}
		public static FormCaptionButtonAction GetAction(IntPtr handle, FormCaptionButtonKind kind) {
			switch(kind) {
				case FormCaptionButtonKind.MdiClose:
					return FormCaptionButtonAction.MdiClose;
				case FormCaptionButtonKind.MdiMinimize:
					return FormCaptionButtonAction.MdiMinimize;
				case FormCaptionButtonKind.MdiRestore:
					return FormCaptionButtonAction.MdiRestore;
				case FormCaptionButtonKind.Close:
					return FormCaptionButtonAction.Close;
				case FormCaptionButtonKind.Help:
					return FormCaptionButtonAction.Help;
				case FormCaptionButtonKind.Maximize:
					if(handle != IntPtr.Zero && NativeMethods.IsZoomed(handle))
						return FormCaptionButtonAction.Restore;
					else
						return FormCaptionButtonAction.Maximize;
				case FormCaptionButtonKind.Minimize:
					if(handle != IntPtr.Zero && NativeMethods.IsIconic(handle))
						return FormCaptionButtonAction.Restore;
					else
						return FormCaptionButtonAction.Minimize;
				default:
					return FormCaptionButtonAction.None;
			}
		}
	}
	public class FormCaptionButtonCollection : CollectionBase {
		FormPainter owner;
		Rectangle buttonsBounds;
		Rectangle mdiButtonsBounds;
		public FormCaptionButtonCollection(FormPainter owner) {
			this.owner = owner;
			this.buttonsBounds = Rectangle.Empty;
			this.mdiButtonsBounds = Rectangle.Empty;
		}
		public const int ButtonInterval = 2;
		public FormCaptionButton this[int index] { get { return List[index] as FormCaptionButton; } }
		public FormCaptionButton this[FormCaptionButtonKind kind] {
			get {
				for(int n = Count - 1; n >= 0; n--) {
					if(this[n].Kind == kind) return this[n];
				}
				return null;
			}
		}
		public FormPainter Owner { get { return owner; } }
		public Rectangle ButtonsBounds { get { return buttonsBounds; } protected set { buttonsBounds = value; } }
		public Rectangle MdiButtonsBounds { get { return mdiButtonsBounds; } protected set { mdiButtonsBounds = value; } }
		public int Add(FormCaptionButton button) { return List.Add(button); }
		public void Insert(int index, FormCaptionButton button) { List.Insert(index, button); }
		public Size CalcMinSize(Graphics g, ObjectPainter painter) {
			FormCaptionButton button = new FormCaptionButton(FormCaptionButtonKind.Close);
			return ObjectPainter.CalcObjectMinBounds(g, painter, button).Size;
		}
		public Size CalcSize(Graphics g, ObjectPainter painter) {
			Size res = Size.Empty;
			for(int n = 0; n < Count; n++) {
				FormCaptionButton button = this[n];
				Size size = ObjectPainter.CalcObjectMinBounds(g, painter, button).Size;
				res.Height = Math.Max(size.Height, res.Height);
				res.Width += size.Width + (n != Count - 1 ? ButtonInterval : 0);
			}
			return res;
		}
		protected bool IsMdiButton(FormCaptionButtonKind kind) {
			return kind == FormCaptionButtonKind.MdiClose || kind == FormCaptionButtonKind.MdiMinimize || kind == FormCaptionButtonKind.MdiRestore;
		}
		protected virtual Rectangle CalcBoundsCore(Graphics g, ObjectPainter painter, Rectangle bounds, bool isMdi) {
			if(!isMdi)
				this.buttonsBounds = Rectangle.Empty;
			else
				this.mdiButtonsBounds = Rectangle.Empty;
			int bSize = bounds.Height;
			Rectangle res = bounds;
			for(int n = Count - 1; n >= 0; n--) {
				FormCaptionButton button = this[n];
				if(IsMdiButton(button.Kind) != isMdi) 
					continue;
				this[n].Bounds = Rectangle.Empty;
				Size size = ObjectPainter.CalcObjectMinBounds(g, painter, button).Size;
				int bx = res.Right - size.Width - (isMdi ? 4 : 0);
				if(ShouldInvertButtons) bx = res.X + (isMdi ? 4 : 0);
				Rectangle bt = new Rectangle(bx, res.Y + (res.Height - size.Height) / 2, size.Width, size.Height);
				if(ShouldInvertButtons) {
					if(bt.Right > bounds.Right) break;
				} else
					if(bt.X < bounds.X) break;
				button.Bounds = bt;
				if(!isMdi)
					this.buttonsBounds = new Rectangle(bt.X, bounds.Y, bounds.Right - bt.X, bt.Height);
				else
					this.mdiButtonsBounds = new Rectangle(bt.X, bounds.Y, bounds.Right - bt.X, bt.Height);
				res.Width -= (ButtonInterval + size.Width);
				if(ShouldInvertButtons) res.X += (ButtonInterval + size.Width);
			}
			return res;
		}
		protected bool ShouldInvertButtons {
			get {
				if(Owner == null) return false;
				return Owner.ShouldInvertButtons;
			}
		}
		public Rectangle CalcMdiBounds(Graphics g, ObjectPainter painter, Rectangle mdiBar) {
			return CalcBoundsCore(g, painter, mdiBar, true);
		}
		public Rectangle CalcBounds(Graphics g, ObjectPainter painter, Rectangle caption) {
			return CalcBoundsCore(g, painter, caption, false);
		}
		public FormCaptionButton GetButton(MouseEventArgs e) {
			return GetButton(new Point(e.X, e.Y));
		}
		public FormCaptionButton GetButton(Point pt) {
			foreach(FormCaptionButton button in this) {
				if(button.Bounds.IsEmpty) continue;
				if(button.Bounds.Contains(pt.X, pt.Y)) return button;
			}
			return null;
		}
		protected bool RectangleContainsX(Rectangle rect, Point pt) {
			return rect.X < pt.X && rect.Right > pt.X;
		}
		public FormCaptionButton GetButton(MouseEventArgs e, FormPainter painter) {
			int x = ShouldInvertButtons ? Owner.FormWidth - e.X: e.X;
			return GetButton(new Point(x, e.Y), painter);
		}
		bool IsFormMaximized(Form frm) { return frm != null && frm.WindowState == FormWindowState.Maximized; }
		public FormCaptionButton GetButton(Point pt, FormPainter painter) {
			Form frm = painter.Owner as Form;
			if(frm != null && frm.RightToLeftLayout)
				pt.X = frm.Width - pt.X;
			foreach (FormCaptionButton button in this) {
				if (IsFormMaximized(frm) && pt.Y < button.Bounds.Y && RectangleContainsX(button.Bounds, pt)) return button;
				if (button.Bounds.Contains(pt.X, pt.Y)) return button;
			}
			if (IsFormMaximized(frm)) {
				if(IsRightToLeft) {
					if(pt.X != -10000 && Count > 0 && this[Count - 1].Bounds.Right > pt.X && this[Count - 1].Bounds.Bottom > pt.Y) return this[Count - 1];
				} else
					if(Count > 0 && this[Count - 1].Bounds.X < pt.X && this[Count - 1].Bounds.Bottom > pt.Y) return this[Count - 1];
			}
			return null;
		}
		public void Draw(GraphicsCache cache, ObjectPainter painter, bool useClip) {
			for(int n = 0; n < Count; n++) {
				if(this[n].Bounds.IsEmpty) continue;
				ObjectPainter.DrawObject(cache, painter, this[n]);
				if(useClip) cache.ClipInfo.ExcludeClip(this[n].Bounds);
			}
		}
		public virtual void CreateButtons(FormCaptionButtonKind visibleButtons) {
			Clear();
			AddButton(FormCaptionButtonKind.Help, visibleButtons);
			AddButton(FormCaptionButtonKind.Minimize, visibleButtons);
			AddButton(FormCaptionButtonKind.Maximize, visibleButtons);
			AddButton(FormCaptionButtonKind.Close, visibleButtons);
			AddButton(FormCaptionButtonKind.MdiMinimize, visibleButtons);
			AddButton(FormCaptionButtonKind.MdiRestore, visibleButtons);
			AddButton(FormCaptionButtonKind.MdiClose, visibleButtons);
		}
		public bool IsRightToLeft { get; set; }
		protected void AddButton(FormCaptionButtonKind button, FormCaptionButtonKind visibleButtons) {
			if((visibleButtons & button) != 0) {
				  Add(CreateFormCaptionButton(button));
			}
		}
		protected void InsertButton(int index, FormCaptionButtonKind button, FormCaptionButtonKind visibleButtons) {
			if((visibleButtons & button) != 0) Insert(index, CreateFormCaptionButton(button));
		}
		protected virtual FormCaptionButton CreateFormCaptionButton(FormCaptionButtonKind kind) {
			return new FormCaptionButton(kind);
		}
	}
	public class FormCaptionButtonSkinPainter : SkinCustomPainter {
		FormPainter owner;
		public FormCaptionButtonSkinPainter(ISkinProvider owner)
			: base(owner) {
			this.owner = owner as FormPainter;
		}
		public bool CorrectImageFormRTL { get; set; }
		protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) {
			FormCaptionButton button = e as FormCaptionButton;
			SkinElementInfo info = new SkinElementInfo(GetSkinElement(e), e.Bounds);
			info.CorrectImageFormRTL = CorrectImageFormRTL;
			info.State = e.State;
			info.ImageIndex = -1;
			if(button.Inactive) {
				info.ImageIndex = 4;
				info.State = ObjectState.Normal;
			}
			return info;
		}
		protected virtual SkinElement GetSkinElement(ObjectInfoArgs e) {
			FormCaptionButton button = e as FormCaptionButton;
			string res;
			switch(button.Action) {
				case FormCaptionButtonAction.MdiClose:
					res = FormSkins.SkinFormButtonMdiClose; break;
				case FormCaptionButtonAction.MdiMinimize:
					res = FormSkins.SkinFormButtonMdiMinimize; break;
				case FormCaptionButtonAction.MdiRestore:
					res = FormSkins.SkinFormButtonMdiRestore; break;
				case FormCaptionButtonAction.Close:
					res = FormSkins.SkinFormButtonClose; break;
				case FormCaptionButtonAction.Help:
					res = FormSkins.SkinFormButtonHelp; break;
				case FormCaptionButtonAction.Maximize:
				default:
					res = FormSkins.SkinFormButtonMaximize; break;
				case FormCaptionButtonAction.Minimize:
					res = FormSkins.SkinFormButtonMinimize; break;
				case FormCaptionButtonAction.Restore:
					res = FormSkins.SkinFormButtonRestore; break;
			}
			if(!IsMdiBarButton(button) && owner != null && (owner.IsToolWindow || owner.ShouldUseSmallButtons)) res = "Small" + res;
			return FormSkins.GetSkin(Provider)[res];
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			SkinElement elem = GetSkinElement(e);
			Size size = elem.Glyph != null? elem.Glyph.GetImageSize(): Size.Empty;
			if(elem.Image != null && elem.Image.Stretch == SkinImageStretch.NoResize) {
				size = elem.Image.GetImageSize();
			}
			Rectangle rect = CalcBoundsByClientRectangle(e, new Rectangle(Point.Empty, size));
			Size minSize = elem.Size.MinSize;
			rect.Size = new Size(Math.Max(minSize.Width, rect.Width), Math.Max(minSize.Height, rect.Height));
			return rect;
		}
		protected bool IsMdiBarButton(FormCaptionButton button) {
			return button.Kind == FormCaptionButtonKind.MdiClose || button.Kind == FormCaptionButtonKind.MdiMinimize || button.Kind == FormCaptionButtonKind.MdiRestore;
		}
	}
	public class Taskbar {
		const int SW_HIDE = 0, SW_SHOW = 1;
		protected static IntPtr Handle {
			get { return NativeMethods.FindWindow("Shell_TrayWnd", ""); }
		}
		public static bool IsAutoHide(out int edge) {
			edge = 0;
			NativeMethods.APPBARDATA data = new NativeMethods.APPBARDATA();
			data.cbSize = Marshal.SizeOf(data);
			data.hWnd = Handle;
			int res = NativeMethods.SHAppBarMessage((int)ABMsg.ABM_GETSTATE, ref data);
			if((res & ABS_AUTOHIDE) == 0) return false;
			for(int n = 0; n < 4; n++) {
				edge = n;
				data.uEdge = (uint)n;
				if(NativeMethods.SHAppBarMessage((int)ABMsg.ABM_GETAUTOHIDEBAR, ref data) != 0) return true;
			}
			edge = (int)ABEdge.ABE_BOTTOM;
			return true;
		}
		public static void CorrectRECTByAutoHide(ref NativeMethods.RECT rect) {
			int edge;
			if(!IsAutoHide(out edge)) return;
			switch((ABEdge)edge) {
				case ABEdge.ABE_BOTTOM: rect.Bottom--; break;
				case ABEdge.ABE_TOP: rect.Bottom--; break;
				case ABEdge.ABE_LEFT: rect.Right--; break;
				case ABEdge.ABE_RIGHT: rect.Right--; break;
			}
		}
		const int ABS_AUTOHIDE = 1;
		internal enum ABMsg : int {
			ABM_NEW = 0,
			ABM_REMOVE,
			ABM_QUERYPOS,
			ABM_SETPOS,
			ABM_GETSTATE,
			ABM_GETTASKBARPOS,
			ABM_ACTIVATE,
			ABM_GETAUTOHIDEBAR,
			ABM_SETAUTOHIDEBAR,
			ABM_WINDOWPOSCHANGED,
			ABM_SETSTATE
		}
		internal enum ABEdge : int {
			ABE_LEFT = 0,
			ABE_TOP,
			ABE_RIGHT,
			ABE_BOTTOM
		}
	}
}
