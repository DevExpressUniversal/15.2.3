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
using DevExpress.Skins.XtraForm;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Mdi;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraEditors.ButtonPanel;
using DevExpress.XtraEditors.ButtonsPanelControl;
namespace DevExpress.XtraBars.Navigation {
	public interface INavigationPane : INavigationFrame {
		IButtonsPanel ButtonsPanel { get; }
		NavigationPaneViewInfo ViewInfo { get; }
		NavigationPaneState State { get; set; }
		Size RegularMinSize { get; }
		DockStyle Dock { get; }
		int ExternalSplitterWidth { get; set; }
		void InvalidateNC();
		void LayoutChanged();
	}
	public enum ItemShowMode { Default, Image, Text, ImageAndText, ImageOrText }
	public enum NavigationPaneState { Default, Collapsed, Regular, Expanded }
	[DXToolboxItem(true), Description("A navigation control with vertically aligned page headers and a resizeable content area aside.")]
	[ToolboxBitmap(typeof(NavigationPane), "NavigationPane")]
	[DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation)]
	[Designer("DevExpress.XtraBars.Design.NavigationPaneDesigner, " + AssemblyInfo.SRAssemblyBarsDesign)]
	public class NavigationPane : NavigationFrame, INavigationPane, IButtonsPanelOwner, IButtonPanelControlAppearanceOwner, IButtonsPanelGlyphSkinningOwner {
		public const int StickyWidth = 80;
		NavigationPaneViewInfo viewInfoCore;
		ButtonsPanel buttonsPanelCore;
		GraphicsInfo gInfo;
		NavigationPanePainter painterCore;
		INavigationPageProperties pagePropertiesCore;
		NavigationPaneState stateCore;
		Orientation itemOrientationCore;
		Size regularSizeCore;
		ButtonsPanelControlAppearance buttonsAppearanceCore;
		bool allowGlyphSkinningCore;
		INavigationPaneResizeListener liveResizeListenerCore;
		INavigationPaneResizeListener abornerResizeListenerCore;
		public NavigationPane() {
			allowGlyphSkinningCore = false;
			viewInfoCore = CreateViewInfo();
			regularSizeCore = Size.Empty;
			buttonsPanelCore = CreateButtonsPanel();
			SubscribeButtonsPanel();
			itemOrientationCore = Orientation.Horizontal;
			gInfo = new GraphicsInfo();
			stateCore = NavigationPaneState.Default;
			painterCore = CreatePainter();
			pagePropertiesCore = CreateNavigationPageProperties();
			PageProperties.Changed += OnPagePropertiesChanged;
			buttonsAppearanceCore = new ButtonsPanelControlAppearance(this);
			buttonsAppearanceCore.Changed += OnButtonsAppearanceChanged;
			abornerResizeListenerCore = new AdornerResizeListener(this);
			liveResizeListenerCore = new LiveResizeListener(this);
			DisableLiveResize = false;
		}
		void OnPagePropertiesChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		protected virtual INavigationPageProperties CreateNavigationPageProperties() {
			return new NavigationPageProperties();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(Parent != null)
					Parent.SizeChanged -= OnParentSizeChanged;
				if(buttonsAppearanceCore != null)
					buttonsAppearanceCore.Changed -= OnButtonsAppearanceChanged;
				if(buttonsPanelCore != null) {
					buttonsPanelCore.ButtonUnchecked -= OnButtonUnchecked;
					buttonsPanelCore.ButtonChecked -= OnButtonChecked;
					buttonsPanelCore.Changed -= OnButtonsPanelChanged;
				}
				if(PageProperties != null) {
					PageProperties.Changed -= OnPagePropertiesChanged;
					PageProperties.Dispose();
				}
				Docking2010.Ref.Dispose(ref abornerResizeListenerCore);
				Docking2010.Ref.Dispose(ref liveResizeListenerCore);
			}
			base.Dispose(disposing);
		}
		[Category("Appearance"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("NavigationPaneAllowGlyphSkinning"),
#endif
 DefaultValue(false)]
		public bool AllowGlyphSkinning {
			get { return allowGlyphSkinningCore; }
			set {
				allowGlyphSkinningCore = value;
				UpdateButtonPanel();
			}
		}
		[Category("Appearance"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("NavigationPaneAppearanceButton")
#else
	Description("")
#endif
]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonsPanelControlAppearance AppearanceButton {
			get {
				return buttonsAppearanceCore;
			}
		}
		void ResetAppearanceButton() { AppearanceButton.Reset(); }
		bool ShouldSerializeAppearanceButton() { return AppearanceButton.ShouldSerialize(); }
		void OnButtonsAppearanceChanged(object sender, EventArgs e) {
			UpdateButtonPanel();
		}
		protected virtual void UpdateButtonPanel() {
			InvalidateNC();
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("NavigationPanePageProperties")]
#endif
		[Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public INavigationPageProperties PageProperties {
			get { return pagePropertiesCore; }
		}
		protected internal NavigationPanePainter Painter {
			get {
				if(painterCore == null)
					painterCore = CreatePainter();
				return painterCore;
			}
		}
		protected virtual NavigationPanePainter CreatePainter() {
			return new NavigationPaneSkinPainter(LookAndFeel);
		}
		void SubscribeButtonsPanel() {
			buttonsPanelCore.ButtonUnchecked += OnButtonUnchecked;
			buttonsPanelCore.ButtonChecked += OnButtonChecked;
			buttonsPanelCore.Changed += OnButtonsPanelChanged;
			buttonsPanelCore.ContentAlignment = ContentAlignment.TopLeft;
			buttonsPanelCore.Orientation = GetOrientation();
		}
		protected virtual Orientation GetOrientation() {
			return Orientation.Vertical;
		}
		protected override void OnDockChanged(EventArgs e) {
			base.OnDockChanged(e);
			switch(Dock) {
				case DockStyle.Left:
					buttonsPanelCore.Orientation = GetOrientation();
					buttonsPanelCore.ContentAlignment = ContentAlignment.TopLeft;
					break;
				case DockStyle.Right:
					buttonsPanelCore.Orientation = GetOrientation();
					buttonsPanelCore.ContentAlignment = ContentAlignment.TopRight;
					break;
				case DockStyle.Top:
					break;
				case DockStyle.Bottom:
					break;
				default:
					break;
			}
		}
		void OnButtonsPanelChanged(object sender, EventArgs e) {
			InvalidateNC();
		}
		void OnButtonChecked(object sender, ButtonEventArgs e) {
			var page = e.Button.Properties.Tag as INavigationPageBase;
			if(page != null) {
				SelectedPageCore = page;
				if(SelectedPageCore != page) {
					if(State == NavigationPaneState.Collapsed) {
						UpdateCheckedButton(page, false);
						return;
					}
					UpdateCheckedButton(SelectedPageCore, true);
					UpdateCheckedButton(page, false);
				}
				else
					if(State == NavigationPaneState.Collapsed)
						State = NavigationPaneState.Default;
			}
		}
		[DefaultValue(NavigationPaneState.Default), Category("Behavior")]
#if !SL
	[DevExpressXtraBarsLocalizedDescription("NavigationPaneState")]
#endif
		[SmartTagProperty("State", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public virtual NavigationPaneState State {
			get { return stateCore; }
			set {
				if(stateCore == value) return;
				NavigationPaneState oldState = stateCore;
				stateCore = value;
				UpdateState(oldState);
				RaiseStateChanged(State, oldState);
			}
		}
		[DefaultValue(Orientation.Horizontal), Category("Appearance")]
#if !SL
	[DevExpressXtraBarsLocalizedDescription("NavigationPaneItemOrientation")]
#endif
		public Orientation ItemOrientation {
			get { return itemOrientationCore; }
			set {
				if(itemOrientationCore == value) return;
				itemOrientationCore = value;
				buttonsPanelCore.ButtonRotationAngle = itemOrientationCore == Orientation.Vertical ? RotationAngle.Rotate90 : RotationAngle.None;
			}
		}
		bool allowHtmlDrawCore;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("NavigationPaneAllowHtmlDraw")]
#endif
		[Category("Appearance"), DefaultValue(false), SmartTagProperty("Allow Buttons Html Draw", "")]
		public bool AllowHtmlDraw {
			get { return allowHtmlDrawCore; }
			set {
				if(allowHtmlDrawCore == value) return;
				allowHtmlDrawCore = value;
				LayoutChanged();
			}
		}
		bool allowResizeCore = true;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("NavigationPaneAllowResize")]
#endif
		[Category("Behavior"), DefaultValue(true)]
		public bool AllowResize {
			get { return allowResizeCore; }
			set {
				if(allowResizeCore == value) return;
				allowResizeCore = value;
			}
		}
		int lockUpdateState = 0;
		protected void BeginUpdateState() {
			lockUpdateState++;
		}
		protected void EndUpdateState() {
			lockUpdateState = 0;
			InvalidateNC();
		}
		bool StateUpdateLocked { get { return lockUpdateState != 0; } }
		protected virtual void UpdateState(NavigationPaneState oldState) {
			BeginUpdateState();
			if(Parent != null)
				Parent.SizeChanged -= OnParentSizeChanged;
			switch(State) {
				case NavigationPaneState.Default:
				case NavigationPaneState.Regular:
					DoRestore();
					break;
				case NavigationPaneState.Collapsed:
					DoCollapse(oldState);
					break;
				case NavigationPaneState.Expanded:
					DoExpand(oldState);
					break;
			}
			EndUpdateState();
		}
		[DefaultValue(typeof(Size), "0, 0"), Category("Behavior")]
#if !SL
	[DevExpressXtraBarsLocalizedDescription("NavigationPaneRegularSize")]
#endif
		public Size RegularSize {
			get { return regularSizeCore; }
			set {
				if(regularSizeCore == value) return;
				regularSizeCore = value;
				if(Size != RegularSize && (State == NavigationPaneState.Default || State == NavigationPaneState.Regular))
					Size = RegularSize;
			}
		}
		[DefaultValue(false)]
		public bool DisableLiveResize { get; set; }
		protected virtual void DoRestore() {
			var button = ButtonsPanel.Buttons.FindFirst(s => s.Properties.Tag == SelectedPageCore);
			if(button != null)
				button.Properties.Checked = true;
			if(ResizeListener.SizeMode) return;
			if(RegularSize == Size.Empty)
				Size = new System.Drawing.Size(300, 300);
			else
				Size = RegularSize;
		}
		protected virtual void DoExpand(NavigationPaneState oldState) {
			if(!IsHandleCreated) return;
			UpdateCheckedButton(SelectedPage, true);
			if(oldState == NavigationPaneState.Regular || oldState == NavigationPaneState.Default)
				regularSizeCore = Size;
			Size = new Size(Parent.ClientRectangle.Width - (this as INavigationPane).ExternalSplitterWidth, Size.Height);
			if(Parent != null)
				Parent.SizeChanged += OnParentSizeChanged;
		}
		void OnParentSizeChanged(object sender, EventArgs e) {
			Size = new Size(Parent.ClientRectangle.Width - (this as INavigationPane).ExternalSplitterWidth, Size.Height);
		}
		protected virtual void DoCollapse(NavigationPaneState oldState) {
			var button = ButtonsPanel.Buttons.FindFirst(s => s.Properties.Tag == SelectedPageCore);
			try {
				LockPainting();
				if(button != null)
					button.Properties.Checked = false;
				if(ViewInfo != null && IsHandleCreated) {
					InvalidateNC();
					if(oldState != NavigationPaneState.Expanded)
						regularSizeCore = Size;
					Size = new Size(ViewInfo.ButtonsBounds.Width, Size.Height);
				}
			}
			finally {
				UnlockPainting();
			}
		}
		protected internal bool IsPaintingLocked { get { return paintLocker > 0; } }
		int paintLocker;
		protected internal virtual void UnlockPainting() {
			paintLocker--;
		}
		protected internal virtual void LockPainting() {
			paintLocker++;
		}
		static readonly object stateChangedCore = new object();
		[Category("Layout")]
		public event StateChangedEventHandler StateChanged {
			add { Events.AddHandler(stateChangedCore, value); }
			remove { Events.RemoveHandler(stateChangedCore, value); }
		}
		protected virtual void RaiseStateChanged(NavigationPaneState state, NavigationPaneState oldState) {
			StateChangedEventHandler handler = (StateChangedEventHandler)Events[stateChangedCore];
			if(handler != null) handler(this, new StateChangedEventArgs(state, oldState));
		}
		protected internal override bool CanUseTransition() {
			return AllowTransitionAnimation == DevExpress.Utils.DefaultBoolean.True;
		}
		[SmartTagProperty("Dock Style", "", SmartTagActionType.RefreshAfterExecute)]
		public override DockStyle Dock {
			get { return base.Dock; }
			set { base.Dock = value; }
		}
		void OnButtonUnchecked(object sender, ButtonEventArgs e) {
			if(Pages.Count == 0) {
				LayoutChanged();
				return;
			}
			State = NavigationPaneState.Collapsed;
			InvalidateNC();
		}
		protected virtual ButtonsPanel CreateButtonsPanel() {
			return new NavigationPaneButtonsPanel(this);
		}
		protected virtual NavigationPaneViewInfo CreateViewInfo() {
			return new NavigationPaneViewInfo(this);
		}
		protected override void OnUnlockUpdate() {
			base.OnUnlockUpdate();
			if(!Pages.Contains(SelectedPageCore as NavigationPageBase)) {
				SelectedPageCore = null;
			}
		}
		protected override void OnPageAdded(NavigationPageBase navigationPage) {
			base.OnPageAdded(navigationPage);
			navigationPage.Bounds = ClientRectangle;
			var button = new NavigationButton(navigationPage);
			button.UseCaption = true;
			(navigationPage as INavigationPageBase).SetOwner(this);
			if(SelectedPageCore == navigationPage)
				button.Checked = true;
			button.Image = navigationPage.Image;
			button.Tag = navigationPage;
			button.GroupIndex = 0;
			buttonsPanelCore.Buttons.Add(button);
			navigationPage.Properties.EnsureParentProperties(PageProperties);
		}
		protected override void OnPageRemoved(NavigationPageBase navigationPage) {
			base.OnPageRemoved(navigationPage);
			(navigationPage as INavigationPageBase).SetOwner(null);
			var button = ButtonsPanel.Buttons.FindFirst(s => s.Properties.Tag == navigationPage);
			if(button != null)
				buttonsPanelCore.Buttons.Remove(button);
		}
		protected override void OnSelectedPageChanged(INavigationPageBase oldPage, INavigationPageBase newPage) {
			base.OnSelectedPageChanged(oldPage, newPage);
			if(newPage != null) {
				newPage.Bounds = ClientRectangle;
				UpdateCheckedButton(newPage, true);
			}
			if(oldPage != null) {
				UpdateCheckedButton(oldPage, false);
			}
			LayoutChanged();
		}
		void UpdateCheckedButton(INavigationPageBase page, bool @checked) {
			var pageButton = ButtonsPanel.Buttons.FindFirst((e) => e.Properties.Tag == page);
			if(pageButton != null && State != NavigationPaneState.Collapsed) {
				pageButton.Properties.LockCheckEvent();
				pageButton.Properties.Checked = @checked;
				pageButton.Properties.UnlockCheckEvent();
			}
		}
		protected override void OnPagesCollectionClear() {
			base.OnPagesCollectionClear();
			if(IsDesignMode) {
				ButtonsPanel.Buttons.BeginUpdate();
				ButtonsPanel.Buttons.Clear();
				ButtonsPanel.Buttons.CancelUpdate();
			}
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			LayoutChanged();
			if(StateUpdateLocked || !IsHandleCreated) return;
			if(State == NavigationPaneState.Regular || State == NavigationPaneState.Default)
				RegularSize = Size;
		}
		public override void LayoutChanged() {
			base.LayoutChanged();
			InvalidateNC();
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			if(SelectedPageCore != null) {
				SelectedPageCore.Bounds = ClientRectangle;
				SelectedPageCore.Visible = true;
			}
			if(State == NavigationPaneState.Collapsed) {
				Size collapsedSize = ButtonsPanel.ViewInfo.CalcMinSize(gInfo.AddGraphics(null));
				Size = new Size(collapsedSize.Width, Size.Height);
				gInfo.ReleaseGraphics();
			}
			if(State == NavigationPaneState.Expanded)
				DoExpand(NavigationPaneState.Expanded);
		}
		protected override void UpdateStyle() {
			if(State == NavigationPaneState.Collapsed) {
				DoCollapse(NavigationPaneState.Expanded);
			}
		}
		private void DoPrint(ref Message m) {
			try {
				using(Graphics g = Graphics.FromHdc(m.WParam))
					ObjectPainter.DrawObject(new GraphicsCache(g), painterCore, ViewInfo);
			}
			finally { m.Result = IntPtr.Zero; }
		}
		protected INavigationPaneResizeListener ResizeListener {
			get { return DisableLiveResize ? abornerResizeListenerCore : liveResizeListenerCore; }
		}
		protected override void WndProc(ref Message m) {
			switch(m.Msg) {
				case MSG.WM_NCCALCSIZE:
					DoNCCalcSize(ref m);
					break;
				case MSG.WM_PRINT:
					DoPrint(ref m);
					break;
				case MSG.WM_NCHITTEST:
					if(DoNCHitTest(ref m))
						return;
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
				case MSG.WM_NCPAINT:
					DoNCPaint(ref m);
					break;
				case MSG.WM_GETMINMAXINFO:
					ProcessGetMinMaxInfo(ref m);
					base.WndProc(ref m);
					break;
				case MSG.WM_SYSCOMMAND:
					ProcessSysCommand(ref m);
					break;
				case 0x0231:
					ProcessEnterSizeMove();
					m.Result = new IntPtr(1);
					break;
				case 0x0232:
					ProcessExitSizeMove();
					break;
				case MSG.WM_WINDOWPOSCHANGED:
					ProcessWindowPosChanged(m.LParam);
					break;
				case MSG.WM_WINDOWPOSCHANGING:
					ProcessWindowPosChanging(m.LParam);
					break;
			}
			base.WndProc(ref m);
		}
		void ProcessWindowPosChanging(IntPtr intPtr) {
			if(ResizeListener != null)
				ResizeListener.ProcessWindowsPosChanging(intPtr);
		}
		bool PatchedSize { get; set; }
		protected void ProcessWindowPosChanged(IntPtr lParam) { }
		protected virtual bool ProcessSysCommand(ref Message m) {
			if((WinAPIHelper.GetInt(m.WParam) & 0xFFF0) == DevExpress.Utils.Drawing.Helpers.NativeMethods.SC.SC_SIZE) {
				ResizeListener.SizeMode = true;
			}
			return true;
		}
		protected void ProcessEnterSizeMove() {
			if(ResizeListener != null)
				ResizeListener.ProcessEnterSizeMove();
		}
		protected void ProcessExitSizeMove() {
			if(ResizeListener != null)
				ResizeListener.ProcessExitSizeMove();
		}
		protected virtual void ProcessGetMinMaxInfo(ref Message m) {
			if(m.LParam == IntPtr.Zero) return;
			var minMax = (NativeMethods.MINMAXINFO)BarNativeMethods.PtrToStructure(m.LParam, typeof(NativeMethods.MINMAXINFO));
			if(ViewInfo != null) {
				Size minSize = ViewInfo.ButtonsBounds.Size;
				minMax.ptMinTrackSize = new NativeMethods.POINT(minSize.Width, minSize.Height);
				BarNativeMethods.StructureToPtr(minMax, m.LParam, false);
			}
		}
		protected virtual void DoNCMouseLeave() {
			if(ButtonsPanel != null) {
				ButtonsPanel.Handler.OnMouseLeave();
				if(IsHandleCreated)
					NativeMethods.RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero, 0x401);
				trackingMouseLeave = false;
			}
			if(SelectedPage != null) {
				SelectedPage.ButtonsPanel.Handler.OnMouseLeave();
			}
		}
		protected virtual void DoMouseUp(MouseEventArgs e) {
			MouseEventArgs es = new MouseEventArgs(MouseButtons.Left, e.Clicks, e.X, e.Y, e.Delta);
			if(ButtonsPanel != null) {
				ButtonsPanel.Handler.OnMouseUp(es);
			}
			if(SelectedPage != null) {
				SelectedPage.ButtonsPanel.Handler.OnMouseUp(es);
			}
		}
		protected virtual void DoNCRMouseUp(MouseEventArgs e) { }
		protected virtual void DoNCMouseDown(MouseEventArgs e) {
			MouseEventArgs es = new MouseEventArgs(MouseButtons.Left, e.Clicks, e.X, e.Y, e.Delta);
			if(ButtonsPanel != null) {
				ButtonsPanel.Handler.OnMouseDown(es);
			}
			if(SelectedPage != null) {
				SelectedPage.ButtonsPanel.Handler.OnMouseDown(es);
			}
		}
		bool trackingMouseLeave = false;
		protected virtual void DoNCMouseMove(MouseEventArgs e) {
			if(ButtonsPanel != null) {
				ButtonsPanel.Handler.OnMouseMove(e);
			}
			if(SelectedPage != null) {
				SelectedPage.ButtonsPanel.Handler.OnMouseMove(e);
			}
			if(!trackingMouseLeave) {
				NativeMethods.TRACKMOUSEEVENTStruct msevnt = new NativeMethods.TRACKMOUSEEVENTStruct();
				msevnt.cbSize = Marshal.SizeOf(msevnt);
				msevnt.dwFlags = 0x00000002 | 0x00000010;
				msevnt.hwndTrack = Handle;
				msevnt.dwHoverTime = 0;
				if(NativeMethods.TrackMouseEvent(msevnt)) {
					trackingMouseLeave = true;
				}
			}
		}
		protected virtual void DoNCPaint(ref Message m) {
			if(IsPaintingLocked) return;
			DrawNC(ref m);
			m.Result = IntPtr.Zero;
		}
		[System.Security.SecuritySafeCritical]
		protected virtual void DrawNC(ref Message m) {
			if(Bounds.Width <= 0 || Bounds.Height <= 0) return;
			IntPtr hDC = FormPainter.GetDC(Handle, m);
			if(ViewInfo.ButtonsBounds.Width <= 0) return;
			try {
				LockPainting();
				if(!ViewInfo.ClientBounds.IsEmpty && State != NavigationPaneState.Collapsed)
					NativeMethods.ExcludeClipRect(hDC, ViewInfo.ClientBounds.Left, ViewInfo.ClientBounds.Top, ViewInfo.ClientBounds.Right, ViewInfo.ClientBounds.Bottom);
				using(XtraBufferedGraphics bg = XtraBufferedGraphicsManager.Current.Allocate(hDC, new Rectangle(0, 0, Bounds.Width, Bounds.Height))) {
					using(GraphicsCache cache = new GraphicsCache(bg.Graphics)) {
						ObjectPainter.DrawObject(cache, painterCore, ViewInfo);
					}
					bg.Render();
				}
			}
			finally {
				UnlockPainting();
				NativeMethods.ReleaseDC(Handle, hDC); }
		}
		protected internal void InvalidateNC() {
			if(IsHandleCreated)
				FormPainter.InvalidateNC(Handle);
		}
		protected virtual void DoNCCalcSize(ref Message m) {
			if(m.WParam == IntPtr.Zero) {
				WinAPI.RECT nccsRect = (WinAPI.RECT)m.GetLParam(typeof(WinAPI.RECT));
				Rectangle rect = ViewInfo.CalcNC(gInfo.AddGraphics(null), new Rectangle(0, 0, nccsRect.ToRectangle().Width, nccsRect.ToRectangle().Height), Painter);
				gInfo.ReleaseGraphics();
				if(!rect.IsEmpty) {
					Rectangle patchedRectangle = new Rectangle(nccsRect.left + rect.Left, nccsRect.top + rect.Top, rect.Width, rect.Height);
					nccsRect.RestoreFromRectangle(patchedRectangle);
					BarNativeMethods.StructureToPtr(nccsRect, m.LParam, false);
					m.Result = IntPtr.Zero;
				}
			}
			else {
				WinAPI.NCCALCSIZE_PARAMS nccsParams = (WinAPI.NCCALCSIZE_PARAMS)m.GetLParam(typeof(WinAPI.NCCALCSIZE_PARAMS));
				Rectangle bounds = nccsParams.rgrcProposed.ToRectangle();
				Rectangle rect = ViewInfo.CalcNC(gInfo.AddGraphics(null), new Rectangle(0, 0, bounds.Width, bounds.Height), Painter);
				gInfo.ReleaseGraphics();
				if(!rect.IsEmpty) {
					Rectangle patchedRectangle = new Rectangle(bounds.Left + rect.Left, bounds.Top + rect.Top, rect.Width, rect.Height);
					nccsParams.rgrcProposed.RestoreFromRectangle(patchedRectangle);
					BarNativeMethods.StructureToPtr(nccsParams, m.LParam, false);
					m.Result = IntPtr.Zero;
				}
			}
		}
		protected virtual bool DoNCHitTest(ref Message m) {
			Point pt = WinAPIHelper.PointToFormBounds(Handle, WinAPIHelper.GetPoint(m.LParam));
			if(ViewInfo.ResizeBounds.Contains(pt)) {
				if(Dock == DockStyle.Fill || !AllowResize) return false;
				m.Result = new IntPtr(NativeMethods.HT.HTRIGHT);
				if(IsRightToLeftLayout())
					m.Result = new IntPtr(NativeMethods.HT.HTLEFT);
				return true;
			}
			if(ViewInfo.ButtonsBounds.Contains(pt) || (ViewInfo.selectedPageInfo != null && ViewInfo.selectedPageInfo.CaptionBounds.Contains(pt))) {
				m.Result = new IntPtr(NativeMethods.HT.HTOBJECT);
				return true;
			}
			return false;
		}
		public INavigationPageBase CalcHitInfo(Point pt) {
			BaseButtonInfo panelHitInfo = ButtonsPanel.ViewInfo.CalcHitInfo(pt);
			if(panelHitInfo != null)
				return Pages[ButtonsPanel.Buttons.IndexOf(panelHitInfo.Button)] as NavigationPage;
			if(SelectedPageCore != null && (SelectedPageCore.ViewInfo as NavigationPageViewInfo).Bounds.Contains(pt))
				return SelectedPageCore as NavigationPageBase;
			return null;
		}
		protected internal virtual NavigationPaneViewInfo ViewInfo {
			get { return viewInfoCore; }
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(SelectedPageCore != null)
				SelectedPageCore.Bounds = ClientRectangle;
			base.OnPaint(e);
		}
		#region IButtonsPanelOwner Members
		object IButtonsPanelOwner.ButtonBackgroundImages {
			get { return null; }
		}
		public virtual ObjectPainter GetPainter() {
			return new NavigationPaneButtonsPanelSkinPainter(LookAndFeel);
		}
		object IButtonsPanelOwner.Images {
			get { return null; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsSelected {
			get { return State == NavigationPaneState.Collapsed; }
		}
		#endregion
		#region INavigationFrame Members
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public IButtonsPanel ButtonsPanel {
			get { return buttonsPanelCore; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		NavigationPaneViewInfo INavigationPane.ViewInfo {
			get { return ViewInfo; }
		}
		void IButtonsPanelOwner.Invalidate() {
			if(IsHandleCreated)
				NativeMethods.RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero, 0x401);
		}
		void INavigationPane.InvalidateNC() {
			InvalidateNC();
		}
		int INavigationPane.ExternalSplitterWidth { get; set; }
		Size INavigationPane.RegularMinSize {
			get {
				if(ViewInfo != null && Parent != null) {
					Size minSize = ViewInfo.CalcNCMinSize(gInfo.AddGraphics(null), Parent.ClientRectangle, Painter);
					gInfo.ReleaseGraphics();
					return new Size(minSize.Width + StickyWidth, Size.Height);
				}
				return Size;
			}
		}
		#endregion
		#region IButtonPanelControlAppearanceOwner Members
		IButtonsPanelControlAppearanceProvider IButtonPanelControlAppearanceOwner.CreateAppearanceProvider() {
			return new ButtonsPanelControlAppearanceProvider();
		}
		#endregion
		#region IAppearanceOwner Members
		bool IAppearanceOwner.IsLoading {
			get { return false; }
		}
		#endregion
		#region IButtonsPanelGlyphSkinningOwner Members
		public Color GetGlyphSkinningColor(BaseButtonInfo info) {
			if(info.ButtonPanelOwner == null) return Color.Empty;
			if((info.State & ObjectState.Pressed) != 0)
				return info.ButtonPanelOwner.AppearanceButton.Pressed.ForeColor;
			if((info.State & ObjectState.Hot) != 0)
				return info.ButtonPanelOwner.AppearanceButton.Hovered.ForeColor;
			return info.ButtonPanelOwner.AppearanceButton.Normal.ForeColor;
		}
		#endregion
	}
}
