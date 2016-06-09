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
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
using DevExpress.Utils.Animation;
namespace DevExpress.XtraBars.Docking2010.Customization {
	public class FlyoutDialogForm : Form {
		ITransparentFlyoutForm transparentFlyoutFormCore;
		AdornerElementInfo elementInfoCore;
		Flyout flyoutCore;
		public virtual Flyout Flyout {
			get { return flyoutCore; }
		}
		ITransparentFlyoutForm TransparentForm {
			get { return transparentFlyoutFormCore; }
		}
		protected virtual AdornerElementInfo ElementInfo {
			get { return elementInfoCore; }
		}
		protected virtual FlyoutAdornerElementInfoArgs FlyoutInfoArgs {
			get { return ElementInfo.InfoArgs as FlyoutAdornerElementInfoArgs; }
		}
		public FlyoutDialogForm(AdornerElementInfo elementInfo, Flyout flyout) {
			InitStyle();
			elementInfoCore = elementInfo;
			flyoutCore = flyout;
		}
		protected virtual void InitStyle() {
			FormBorderStyle = FormBorderStyle.None;
			ShowIcon = false;
			ShowInTaskbar = false;
			KeyPreview = true;
			FormBorderStyle = FormBorderStyle.None;
			StartPosition = FormStartPosition.Manual;
			SetStyle(ControlStyles.Selectable, false);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			CreateTransparentFlyoutWindow();
		}
		protected virtual void CreateTransparentFlyoutWindow() {
			transparentFlyoutFormCore = FlyoutDialogFactory.CreateTransparentFlyoutForm(this);
			FlyoutInfoArgs.AttachToFlyoutDialogForm(this);
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
				List<Rectangle> rects = new List<Rectangle>();
				rects.AddRange(ElementInfo.InfoArgs.CalculateRegions(false));
				ObjectPainter.DrawObject(cache, ElementInfo.OpaquePainter, ElementInfo.InfoArgs);
				CheckWindowRegion(rects);
			}
		}
		protected void CheckWindowRegion(IEnumerable<Rectangle> regions) {
			using(Region region = new Region()) {
				region.MakeEmpty();
				foreach(Rectangle r in regions) {
					if(r.IsEmpty) continue;
					r.Offset(Point.Empty);
					region.Union(r);
				}
				using(Graphics g = Graphics.FromHwndInternal(TransparentForm.Handle)) {
					BarNativeMethods.SetWindowRgn(TransparentForm.Handle, region.GetHrgn(g), false);
				}
				Region = new Region(FlyoutInfoArgs != null ? FlyoutInfoArgs.InfoArgs.FlyoutBounds : Bounds);
			}
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			FlyoutInfoArgs.HitTestFlyoutForm(e.Location);
			FlyoutInfoArgs.OnMouseDown(e.Location);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			FlyoutInfoArgs.HitTestFlyoutForm(e.Location);
			FlyoutInfoArgs.OnMouseMove(e.Location);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			FlyoutInfoArgs.HitTestFlyoutForm(e.Location);
			FlyoutInfoArgs.OnMouseUp(e.Location);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			FlyoutInfoArgs.OnMouseLeave();
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if(keyData == Keys.Escape) {
				Close();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
	}
	public class FlyoutDialog : DevExpress.XtraEditors.XtraForm, IFlyoutObjectInfoArgsOwner, IDXMenuManagerProvider {
		ITransparentFlyoutForm transparentFlyoutFormCore;
		FlyoutObjectInfoArgs infoCore;
		IFlyoutDefaultProperties propertiesCore;
		Predicate<DialogResult> canCloseCore;
		IDXMenuManager menuManagerCore;
		protected FlyoutDialog() {
			LookAndFeel.StyleChanged += OnLookAndFeelStyleChanged;
		}
		protected FlyoutDialog(Form owner, FlyoutProperties parameters)
			: this() {
			SuspendLayout();
			UpdateRightToLeft(owner);
			InitStyle();
			infoCore = CreateFlyoutInfo();
			propertiesCore = CreateFlyoutProperties(parameters);
			Info.Properties = propertiesCore;
			Info.Owner = this;
			Owner = owner;
			if(owner != null)
				TopMost = owner.TopMost;
			UseControlBackColor = true;
			ResumeLayout(false);
		}
		internal void UpdateRightToLeft(Form owner) {
			if(owner != null) {
				RightToLeft = owner.RightToLeft;
				RightToLeftLayout = owner.RightToLeftLayout;
			}
		}
		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			LookAndFeel.StyleChanged -= OnLookAndFeelStyleChanged;
		}
		void Owner_LocationChanged(object sender, EventArgs e) {
			UpdateAdornerLayout();
			SetInfoDirty();
		}
		void Owner_SizeChanged(object sender, EventArgs e) {
			UpdateAdornerLayout();
			SetInfoDirty();
		}
		void OnLookAndFeelStyleChanged(object sender, EventArgs e) {
			SetInfoDirty();
		}
		void SetInfoDirty() {
			if(Info != null) Info.SetDirty();
		}
		void UpdateAdornerLayout() {
			Rectangle flyoutRect = UpdateScreenBounds();
			FlyoutDialogFactory.ApplyTransparentFlyoutFormSettings(TransparentForm, this);
			UpdateFlyoutControlBounds(flyoutRect);
		}
		protected virtual FlyoutObjectInfoArgs CreateFlyoutInfo() {
			return new FlyoutObjectInfoArgs(this);
		}
		protected virtual IFlyoutDefaultProperties CreateFlyoutProperties(FlyoutProperties parameters) {
			return new FlyoutDefaultProperties(parameters);
		}
		public FlyoutDialog(Form owner, Control flyoutControl)
			: this(owner, flyoutControl, (IDXMenuManager)null) {
		}
		public FlyoutDialog(Form owner, Control flyoutControl, IDXMenuManager menuManager)
			: this(owner, new FlyoutProperties()) {
			FlyoutControl = flyoutControl;
			menuManagerCore = menuManager;
		}
		public FlyoutDialog(Form owner, FlyoutAction action)
			: this(owner, new FlyoutProperties()) {
			Info.Action = action;
		}
		public FlyoutDialog(Form owner, FlyoutAction action, Control flyoutControl)
			: this(owner, action) {
			FlyoutControl = flyoutControl;
		}
		public FlyoutDialog(Form owner, FlyoutAction action, Control flyoutControl, bool useControlBackColor)
			: this(owner, action) {
			UseControlBackColor = useControlBackColor;
			FlyoutControl = flyoutControl;
		}
		public FlyoutDialog(Form owner, FlyoutAction action, Control flyoutControl, FlyoutProperties parameters)
			: this(owner, parameters) {
			FlyoutControl = flyoutControl;
			Info.Action = action;
		}
		public FlyoutDialog(Form owner, FlyoutAction action, Control flyoutControl, FlyoutProperties parameters, bool useControlBackColor)
			: this(owner, parameters) {
			UseControlBackColor = useControlBackColor;
			FlyoutControl = flyoutControl;
			Info.Action = action;
		}
		public FlyoutDialog(Form owner, Control flyoutControl, FlyoutProperties parameters)
			: this(owner, parameters) {
			FlyoutControl = flyoutControl;
		}
		public FlyoutDialog(Form owner, FlyoutAction action, FlyoutProperties parameters)
			: this(owner, parameters) {
			Info.Action = action;
		}
		public static DialogResult Show(Form owner, FlyoutAction action, Control control) {
			FlyoutDialog flyoutDialogForm = new FlyoutDialog(owner, action, control);
			return ShowCore(flyoutDialogForm);
		}
		public static DialogResult Show(Form owner, Control flyoutControl) {
			FlyoutDialog flyoutDialogForm = new FlyoutDialog(owner, flyoutControl);
			return ShowCore(flyoutDialogForm);
		}
		public static DialogResult Show(Form owner, FlyoutAction action) {
			FlyoutDialog flyoutDialogForm = new FlyoutDialog(owner, action);
			return ShowCore(flyoutDialogForm);
		}
		public static DialogResult Show(Form owner, Control flyoutControl, Predicate<DialogResult> canClose) {
			FlyoutDialog flyoutDialogForm = new FlyoutDialog(owner, flyoutControl);
			flyoutDialogForm.CanClose = canClose;
			return ShowCore(flyoutDialogForm);
		}
		public static DialogResult Show(Form owner, FlyoutAction action, Predicate<DialogResult> canClose) {
			FlyoutDialog flyoutDialogForm = new FlyoutDialog(owner, action);
			flyoutDialogForm.CanClose = canClose;
			return ShowCore(flyoutDialogForm);
		}
		public static DialogResult Show(Form owner, Control flyoutControl, FlyoutAction action, Predicate<DialogResult> canClose) {
			FlyoutDialog flyoutDialogForm = new FlyoutDialog(owner, action, flyoutControl);
			flyoutDialogForm.CanClose = canClose;
			return ShowCore(flyoutDialogForm);
		}
		public static DialogResult Show(Form owner, Control flyoutControl, FlyoutProperties parameters) {
			FlyoutDialog flyoutDialogForm = new FlyoutDialog(owner, flyoutControl, parameters);
			return ShowCore(flyoutDialogForm);
		}
		public static DialogResult Show(Form owner, FlyoutAction action, FlyoutProperties parameters) {
			FlyoutDialog flyoutDialogForm = new FlyoutDialog(owner, action, parameters);
			return ShowCore(flyoutDialogForm);
		}
		public static DialogResult Show(Form owner, Control flyoutControl, FlyoutAction action, FlyoutProperties parameters) {
			FlyoutDialog flyoutDialogForm = new FlyoutDialog(owner, action, flyoutControl, parameters);
			return ShowCore(flyoutDialogForm);
		}
		public static DialogResult Show(Form owner, Control flyoutControl, FlyoutProperties parameters, Predicate<DialogResult> canClose) {
			FlyoutDialog flyoutDialogForm = new FlyoutDialog(owner, flyoutControl, parameters);
			flyoutDialogForm.CanClose = canClose;
			return ShowCore(flyoutDialogForm);
		}
		public static DialogResult Show(Form owner, FlyoutAction action, FlyoutProperties parameters, Predicate<DialogResult> canClose) {
			FlyoutDialog flyoutDialogForm = new FlyoutDialog(owner, action, parameters);
			flyoutDialogForm.CanClose = canClose;
			return ShowCore(flyoutDialogForm);
		}
		public static DialogResult Show(Form owner, Control flyoutControl, FlyoutAction action, FlyoutProperties parameters, Predicate<DialogResult> canClose) {
			FlyoutDialog flyoutDialogForm = new FlyoutDialog(owner, action, flyoutControl, parameters);
			flyoutDialogForm.CanClose = canClose;
			return ShowCore(flyoutDialogForm);
		}
		public static DialogResult Show(Form owner, Control flyoutControl, FlyoutAction action, FlyoutProperties parameters, Predicate<DialogResult> canClose, bool useControlBackColor) {
			FlyoutDialog flyoutDialogForm = new FlyoutDialog(owner, action, flyoutControl, parameters, useControlBackColor);
			flyoutDialogForm.CanClose = canClose;
			return ShowCore(flyoutDialogForm);
		}
		static DialogResult ShowCore(FlyoutDialog flyoutDialogForm) {
			try {
				if(flyoutDialogForm.Owner == null) {
					flyoutDialogForm.Owner = GuessOwner();
					flyoutDialogForm.UpdateRightToLeft(flyoutDialogForm.Owner);
				}
				if(flyoutDialogForm.Owner != null && flyoutDialogForm.Owner.WindowState == FormWindowState.Minimized)
					NativeMethods.SendMessage(flyoutDialogForm.Owner.Handle, MSG.WM_SYSCOMMAND, new IntPtr(NativeMethods.SC.SC_RESTORE), IntPtr.Zero);
				return flyoutDialogForm.ShowDialog(flyoutDialogForm.Owner);
			}
			finally {
				if(flyoutDialogForm.FlyoutControl != null)
					flyoutDialogForm.Controls.Remove(flyoutDialogForm.FlyoutControl);
				flyoutDialogForm.Dispose();
			}
		}
		internal static Form GuessOwner() {
			Form frm = Form.ActiveForm;
			if(frm == null || frm.InvokeRequired)
				return null;
			while(frm != null && frm.Owner != null && !frm.ShowInTaskbar && !frm.TopMost)
				frm = frm.Owner;
			return frm;
		}
		ITransparentFlyoutForm TransparentForm {
			get { return transparentFlyoutFormCore; }
		}
		protected FlyoutObjectInfoArgs Info {
			get { return infoCore; }
		}
		public IFlyoutDefaultProperties Properties {
			get { return propertiesCore; }
		}
		public bool UseControlBackColor { get; set; }
		protected Predicate<DialogResult> CanClose {
			get { return canCloseCore; }
			set { canCloseCore = value; }
		}
		Control flyoutControlCore;
		public Control FlyoutControl {
			get { return flyoutControlCore; }
			protected set {
				Controls.Remove(flyoutControlCore);
				flyoutControlCore = value;
				Info.Control = flyoutControlCore;
				if(UseControlBackColor)
					BackColor = flyoutControlCore.BackColor;
				Controls.Add(flyoutControlCore);
			}
		}
		protected virtual void InitStyle() {
			FormBorderStyle = FormBorderStyle.None;
			ShowIcon = false;
			ShowInTaskbar = false;
			KeyPreview = true;
			FormBorderStyle = FormBorderStyle.None;
			StartPosition = FormStartPosition.Manual;
			SetStyle(ControlStyles.Selectable, false);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			if(Owner != null) {
				Owner.LocationChanged += Owner_LocationChanged;
				Owner.SizeChanged += Owner_SizeChanged;
			}
			Rectangle flyoutRect = UpdateScreenBounds();
			if(TransparentForm == null)
				transparentFlyoutFormCore = FlyoutDialogFactory.CreateTransparentFlyoutForm(this, Properties.ActualAppearance.BorderColor);
			UpdateFlyoutControlBounds(flyoutRect);
		}
		protected override void OnHandleDestroyed(EventArgs e) {
			Ref.Dispose(ref transparentFlyoutFormCore);
			base.OnHandleDestroyed(e);
		}
		void UpdateFlyoutControlBounds(Rectangle flyoutRect) {
			if(FlyoutControl == null) return;
			if(FlyoutControl.Dock == DockStyle.Fill) {
				FlyoutControl.Dock = DockStyle.None;
				FlyoutControl.Bounds = PlacementHelper.Arrange(new Size(ClientSize.Width, FlyoutControl.Height), flyoutRect, ContentAlignment.MiddleCenter);
			}
			else FlyoutControl.Bounds = PlacementHelper.Arrange(FlyoutControl.Size, flyoutRect, ContentAlignment.MiddleCenter);
		}
		Rectangle UpdateScreenBounds() {
			Rectangle bounds = Rectangle.Empty;
			if(Owner == null)
				bounds = Screen.PrimaryScreen.WorkingArea;
			else {
				bounds = Owner.Bounds;
				if(Owner.WindowState == FormWindowState.Maximized)
					bounds = Owner.FormBorderStyle == System.Windows.Forms.FormBorderStyle.None ? Screen.FromControl(Owner).Bounds : Screen.FromControl(Owner).WorkingArea;
			}
			Bounds = bounds;
			Rectangle flyoutRect = new Rectangle(Point.Empty, new Size(ClientSize.Width, Bounds.Height));
			Info.Bounds = new Rectangle(Point.Empty, (Owner != null) ? Owner.Bounds.Size : bounds.Size);
			return flyoutRect;
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			Info.Calc(e.Graphics);
			using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
				ObjectPainter.DrawObject(cache, ((IFlyoutObjectInfoArgsOwner)this).GetFlyoutPainter(), Info);
			}
			CheckWindowRegion(Info.GetRegionsCore(false));
		}
		protected void CheckWindowRegion(IEnumerable<Rectangle> regions) {
			using(Region region = new Region()) {
				region.MakeEmpty();
				foreach(Rectangle r in regions) {
					if(r.IsEmpty) continue;
					r.Offset(Point.Empty);
					region.Union(r);
				}
				using(Graphics g = Graphics.FromHwndInternal(TransparentForm.Handle)) {
					BarNativeMethods.SetWindowRgn(TransparentForm.Handle, region.GetHrgn(g), false);
				}
				Region = new Region(Info != null ? Info.FlyoutBounds : Bounds);
			}
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			Info.ButtonsPanel.Handler.OnMouseDown(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			Info.ButtonsPanel.Handler.OnMouseMove(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			Info.ButtonsPanel.Handler.OnMouseUp(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			Info.ButtonsPanel.Handler.OnMouseLeave();
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if(keyData == Keys.Escape) {
				Close();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
			if(Owner != null) {
				Owner.LocationChanged -= Owner_LocationChanged;
				Owner.SizeChanged -= Owner_SizeChanged;
			}
			base.OnClosing(e);
			if(CanClose != null)
				e.Cancel = !CanClose(DialogResult);
		}
		#region IFlyoutObjectInfoArgsOwner
		void IFlyoutObjectInfoArgsOwner.LayoutChanged() {
			Info.SetDirty();
			Invalidate();
		}
		bool IFlyoutObjectInfoArgsOwner.CanExecuteCommand(FlyoutCommand flyoutCommand) {
			return flyoutCommand.CanExecute(null);
		}
		void IFlyoutObjectInfoArgsOwner.ExecuteCommand(FlyoutCommand flyoutCommand) {
			if(flyoutCommand.CanExecute(null)) {
				flyoutCommand.Execute(null);
				DialogResult = flyoutCommand.Result;
			}
		}
		ObjectPainter IFlyoutObjectInfoArgsOwner.GetFlyoutPainter() {
			if(LookAndFeel.ActiveStyle != DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Skin)
				return new FlyoutPainter();
			return new FlyoutSkinPainter(LookAndFeel);
		}
		Rectangle IFlyoutObjectInfoArgsOwner.GetControlBounds() {
			return FlyoutControl != null ? FlyoutControl.Bounds : Rectangle.Empty;
		}
		#endregion
		#region IDXMenuManagerProvider
		IDXMenuManager IDXMenuManagerProvider.MenuManager {
			get { return menuManagerCore; }
		}
		#endregion
	}
	internal interface ITransparentFlyoutForm : IWin32Window, IDisposable {
		byte Alpha { get; set; }
		Color BackColor { get; set; }
		Rectangle Bounds { get; set; }
		void Show();
		void Hide();
	}
	static class FlyoutDialogFactory {
		public static ITransparentFlyoutForm CreateTransparentFlyoutForm(Control topLevelControl) {
			ITransparentFlyoutForm form = CreateTransparentFlyoutFormInstance(topLevelControl);
			ApplyTransparentFlyoutFormSettings(form, topLevelControl);
			return form;
		}
		public static ITransparentFlyoutForm CreateTransparentFlyoutForm(Control topLevelControl, Color backColor) {
			ITransparentFlyoutForm form = CreateTransparentFlyoutFormInstance(topLevelControl);
			if(!backColor.IsEmpty) {
				form.Alpha = backColor.A != 255 ? backColor.A : form.Alpha;
				form.BackColor = backColor;
			}
			ApplyTransparentFlyoutFormSettings(form, topLevelControl);
			return form;
		}
		public static void ApplyTransparentFlyoutFormSettings(ITransparentFlyoutForm form, Control topLevelControl) {
			form.Bounds = topLevelControl.Bounds;
			form.Show();
			NativeMethods.SetWindowPos(form.Handle, topLevelControl.Handle, 0, 0, 0, 0,
				NativeMethods.SWP.SWP_NOMOVE | NativeMethods.SWP.SWP_NOSIZE | NativeMethods.SWP.SWP_NOACTIVATE);
		}
		static ITransparentFlyoutForm CreateTransparentFlyoutFormInstance(Control topLevelControl) {
			return new TransparentFlyoutForm(topLevelControl.Handle);
		}
	}
	class TransparentFlyoutForm : DevExpress.Utils.Internal.DXLayeredWindowEx, ITransparentFlyoutForm {
		IntPtr topLevelControlHandleCore;
		public TransparentFlyoutForm(IntPtr topLevelControlHandle) {
			topLevelControlHandleCore = topLevelControlHandle;
			Alpha = 160;
			BackColor = Color.Black;
		}
		public Color BackColor { get; set; }
		protected override void DrawCore(GraphicsCache cache) {
			cache.Graphics.FillRectangle(cache.GetSolidBrush(BackColor), new Rectangle(0, 0, Bounds.Width, Bounds.Height));
		}
		protected override void WndProc(ref Message m) {
			switch(m.Msg) {
				case DevExpress.Utils.Internal.LayeredWindowMessanger.WM_CREATE:
					Invalidate();
					break;
				case DevExpress.Utils.Internal.LayeredWindowMessanger.WM_INVALIDATE:
					Invalidate();
					break;
			}
			base.WndProc(ref m);
		}
		#region ITransparentFlyoutForm Members
		Point locationCore;
		Rectangle ITransparentFlyoutForm.Bounds {
			get { return new Rectangle(locationCore, Size); }
			set {
				Size = value.Size;
				locationCore = value.Location;
			}
		}
		void ITransparentFlyoutForm.Show() {
			Create(topLevelControlHandleCore);
			Show(locationCore);
		}
		#endregion
	}
}
namespace DevExpress.XtraBars {
	public static class FlyoutMessageBox {
		public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton) {
			Image img = null;
			if(icon != MessageBoxIcon.None)
				img = XtraEditors.XtraMessageBox.MessageBoxIconToIcon(icon).ToBitmap();
			var action = new FlyoutAction(XtraEditors.XtraMessageBox.MessageBoxButtonsToDialogResults(buttons))
			{
				Caption = caption,
				Description = text,
				Image = img
			};
			using(img)
				return DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog.Show(null, action);
		}
	}
}
namespace DevExpress.XtraBars.MVVM.Services {
	using DevExpress.Utils.MVVM.Services;
	using DevExpress.XtraBars.Docking2010.Customization;
	public sealed class FlyoutDialogFormFactory : IDialogFormFactory, IDocumentFormFactory {
		IDialogForm IDialogFormFactory.Create() {
			return new DialogForm();
		}
		Form IDocumentFormFactory.Create() {
			return new DialogForm();
		}
		class DialogForm : FlyoutDialog, IDialogForm {
			public DialogForm()
				: base((Form)null, new FlyoutProperties()) {
			}
			Control IDialogForm.Content {
				get { return FlyoutControl; }
			}
			DialogResult IDialogForm.ShowDialog(IWin32Window owner, Control content, string caption, DialogResult[] buttons) {
				Owner = (owner as Form) ?? (owner is Control ? ((Control)owner).FindForm() : null) ?? GuessOwner();
				UpdateRightToLeft(Owner);
				FlyoutControl = content;
				Info.Action = new FlyoutAction(buttons) { Caption = caption };
				return ShowDialog();
			}
		}
	}
}
