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
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.FormShadow;
using DevExpress.Utils.Internal;
using DevExpress.Utils.Win;
using DevExpress.XtraSplashForm;
using DevExpress.XtraSplashScreen;
namespace DevExpress.XtraSplashForm {
	public class SplashFormBase : StickLookAndFeelForm, ITransparentBackgroundManager {
		Timer fadeTimer;
		PostponedCommandManager postponedCommandManager;
		Color activeGlowColor, inactiveGlowColor;
		public SplashFormBase() {
			this.KeyPreview = true;
			this.fadeTimer = new Timer() { Interval = FadeInterval };
			this.StartPosition = FormStartPosition.CenterScreen;
			this.stateInfoCore = new SplashFormProperties();
			this.postponedCommandManager = null;
			this.activeGlowColor = Color.Empty;
			this.inactiveGlowColor = Color.Empty;
		}
		XtraFormShadow formShadowCore;
		protected internal XtraFormShadow FormShadow {
			get { return formShadowCore; }
		}
		protected virtual void InitFormShadow() {
			if(FormShadow == null) {
				formShadowCore = CreateFormShadow();
				FormShadow.ActiveGlowColor = ActiveGlowColor;
				FormShadow.InactiveGlowColor = InactiveGlowColor;
				FormShadow.Form = this;
			}
			FormShadow.IsGlow = true;
			XtraFormShadow.AllowResizeViaShadows = false;
		}
		protected virtual void ReleaseFormShadow() {
			if(FormShadow != null) {
				FormShadow.Dispose();
				this.formShadowCore = null;
			}
		}
		protected virtual XtraFormShadow CreateFormShadow() {
			return new XtraFormShadow();
		}
		new public void Show() { ShowCore(); }
		new public DialogResult ShowDialog() { return ShowDialogCore(); }
		protected virtual void ShowCore() {
			if(Properties.UseFadeInEffect)
				StartFadeInCore();
			base.Show();
		}
		protected virtual DialogResult ShowDialogCore() {
			if(Properties.UseFadeInEffect)
				StartFadeInCore();
			return base.ShowDialog();
		}
		protected internal virtual void OnParentInternalLoad() { }
		protected internal virtual void AssignPostponedManager(PostponedCommandManager postponedManager) {
			this.postponedCommandManager = postponedManager;
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if(Properties.AllowGlowEffect) InitFormShadow();
			OnExecutePostponedCommands();
		}
		protected virtual void OnExecutePostponedCommands() {
			if(PostponedCommandManager == null) return;
			PostponedCommandManager.Execute(PostponedCommandManager, this);
		}
		#region Delayed Closing
		public void DelayedClose(int closingDelay, Form parent) {
			NativeMethods.SetWindowPos(Handle, IntPtr.Zero, 0, 0, 0, 0, NativeMethods.SWP.SWP_NOMOVE | NativeMethods.SWP.SWP_NOSIZE);
			DelayedClosingForm = parent;
			DelayedCloseTimer = new Timer();
			DelayedCloseTimer.Interval = closingDelay;
			DelayedCloseTimer.Tick += (sender, e) => {
				if(DelayedCloseTimer != null)
					DelayedCloseTimer.Dispose();
				DelayedCloseTimer = null;
				Close();
			};
			DelayedCloseTimer.Start();
		}
		protected virtual bool AllowActivateParentOnDelayedClosing { get { return true; } }
		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			SetDialogResult();
			if(!IsDelayedClosingFormValid)
				return;
			if(AllowActivateParentOnDelayedClosing) {
				if(DelayedClosingForm != null) DoActiveForm(DelayedClosingForm);
			}
			DelayedClosingForm = null;
		}
		protected void DoActiveForm(Form form) {
			try {
				form.Invoke((ScreenAction)form.BringToFront);
				form.Invoke((ScreenAction)form.Activate);
			}
			catch { }
		}
		DialogResult? dlgRes = null;
		protected virtual void SetDialogResult() {
			if(!this.dlgRes.HasValue) return;
			DialogResult = (DialogResult)dlgRes;
			this.dlgRes = null;
		}
		protected Form DelayedClosingForm { get; set; }
		protected Timer DelayedCloseTimer { get; set; }
		protected bool IsDelayedClosingFormValid { get { return DelayedClosingForm != null && DelayedClosingForm.IsHandleCreated; } }
		#endregion
		protected override void DrawTopElement(GraphicsCache graphicsCache, Skin skin) { }
		protected virtual int FadeInterval {
			get { return 40; }
		}
		protected Timer FadeTimer { get { return this.fadeTimer; } }
		protected PostponedCommandManager PostponedCommandManager { get { return postponedCommandManager; } }
		SplashFormProperties stateInfoCore = null;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SplashFormBaseProperties"),
#endif
 Category("Display Options")]
		public SplashFormProperties Properties {
			get { return this.stateInfoCore; }
			set { this.stateInfoCore = value; }
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("SplashFormBaseActiveGlowColor")]
#endif
		public Color ActiveGlowColor {
			get { return this.activeGlowColor; }
			set { this.activeGlowColor = value; }
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("SplashFormBaseInactiveGlowColor")]
#endif
		public Color InactiveGlowColor {
			get { return this.inactiveGlowColor; }
			set { this.inactiveGlowColor = value; }
		}
		double opacityInternal;
		protected virtual void StartFadeInCore() {
			this.opacityInternal = this.Opacity;
			this.Opacity = 0;
			fadeTimer.Tick -= FadeOutTimerTickCore;
			fadeTimer.Tick += FadeInTimerTickCore;
			fadeTimer.Enabled = true;
		}
		protected virtual void StartFadeOutCore() {
			fadeTimer.Tick -= FadeInTimerTickCore;
			fadeTimer.Tick += FadeOutTimerTickCore;
			fadeTimer.Enabled = true;
		}
		protected virtual void FadeInTimerTickCore(object sender, EventArgs e) {
			this.Opacity += 0.05;
			UpdateFormShadowOpacity();
			if(this.Opacity > this.opacityInternal - 0.04) {
				fadeTimer.Enabled = false;
				if(FormShadow != null) FormShadow.Opacity = 255;
			}
		}
		protected virtual void FadeOutTimerTickCore(object sender, EventArgs e) {
			this.Opacity -= 0.05;
			UpdateFormShadowOpacity();
			if(this.Opacity < 0.05) {
				this.fadeTimer.Enabled = false;
				this.Opacity = 0;
				this.isAllowClosing = true;
				this.Close();
			}
		}
		protected void UpdateFormShadowOpacity() {
			if(FormShadow == null) return;
			FormShadow.Opacity = (byte)(this.Opacity * 255);
		}
		bool isAllowClosing = false;
		protected override void OnClosing(CancelEventArgs e) {
			if(AllowClosing) {
				this.isAllowClosing = false;
				base.OnClosing(e);
				return;
			}
			e.Cancel = true;
			StartFadeOutCore();
			this.dlgRes = DialogResult;
		}
		protected virtual bool AllowClosing { get { return !Properties.UseFadeOutEffect || this.isAllowClosing; } }
		protected override void OnKeyDown(KeyEventArgs e) {
			if(e.Alt && e.KeyCode == Keys.F4) {
				e.Handled = true;
				return;
			}
			base.OnKeyDown(e);
		}
		#region ITransparentBackgroundManager
		Color ITransparentBackgroundManager.GetForeColor(object childObject) {
			return GetForeColor();
		}
		Color ITransparentBackgroundManager.GetForeColor(Control childControl) {
			return GetForeColor();
		}
		#endregion
		Color GetForeColor() {
			SkinElement element = GetBackgroundSkinElement();
			if(element == null) return ForeColor;
			Color color = element.Color.GetForeColor();
			return color.IsEmpty ? ForeColor : color;
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			this.UpdateRegion();
		}
		protected override bool IsTopMost { get { return false; } }
		protected override bool HasSystemShadow { get { return false; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ICapture {
			get { return base.ICapture; }
			set { base.ICapture = value; }
		}
		#region IDisposable
		protected override void Dispose(bool disposing) {
			if(disposing) {
				this.fadeTimer.Tick -= FadeInTimerTickCore;
				this.fadeTimer.Tick -= FadeOutTimerTickCore;
				this.fadeTimer.Dispose();
				if(DelayedCloseTimer != null)
					DelayedCloseTimer.Dispose();
				ReleaseFormShadow();
			}
			DelayedCloseTimer = null;
			DelayedClosingForm = null;
			base.Dispose(disposing);
		}
		#endregion
		public virtual void ProcessCommand(Enum cmd, object arg) {
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(!DrawAlertWindowBackground) {
				DrawBackground(e);
				return;
			}
			base.OnPaint(e);
		}
		protected virtual void DrawBackground(PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(e)) {
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, GetBackgroundInfo());
			}
		}
		protected virtual ObjectInfoArgs GetBackgroundInfo() {
			return new SkinElementInfo(SplashFormSkinElement, ClientRectangle);
		}
		protected SkinElement SplashFormSkinElement {
			get {
				Skin skin = EditorsSkins.GetSkin(TargetLookAndFeel);
				return skin[EditorsSkins.SkinSplashForm];
			}
		}
		protected SkinElement GetBackgroundSkinElement() {
			if(DrawAlertWindowBackground)
				return BarSkins.GetSkin(TargetLookAndFeel)[BarSkins.SkinAlertWindow];
			return SplashFormSkinElement;
		}
		protected virtual bool DrawAlertWindowBackground { get { return SplashFormSkinElement == null; } }
	}
	public class SplashFormLayerBase : DXLayeredWindowEx {
		Image image;
		SplashScreenLayer parent;
		public SplashFormLayerBase(SplashScreenLayer parent, Image image) {
			this.image = image;
			this.parent = parent;
			Size = image.Size;
		}
		public void Show() {
			base.Create(parent);
			base.Show(GetTargetLocation());
			base.Update();
			this.isInitialized = true;
		}
		public void Close() {
			this.isInitialized = false;
			base.Hide();
			base.Dispose();
		}
		bool isInitialized = false;
		public new byte Alpha {
			get { return base.Alpha; }
			set {
				if(base.Alpha == value) return;
				base.Alpha = value;
				if(this.isInitialized)
					base.Update();
			}
		}
		public Image Image { get { return this.image; } }
		public Control Parent { get { return this.parent; } }
		protected internal Point GetTargetLocation() {
			if(parent.ShouldUseUserLocation)
				return parent.Location;
			return LayoutHelper.GetLocationRelParentCore(this.Size, parent.Location, parent.Size);
		}
		protected override void DrawCore(GraphicsCache cache) {
			cache.Graphics.DrawImage(image, 0, 0, image.Width, image.Height);
			if(CustomPainter != null)
				CustomPainter.Draw(cache, Bounds);
		}
		protected override void OnDisposing() {
			this.image = null;
			this.parent = null;
			this.isInitialized = false;
			base.OnDisposing();
		}
		protected SplashFormBase Owner { get { return Parent as SplashFormBase; } }
		protected ICustomImagePainter CustomPainter { get { return Owner.Properties.CustomImagePainter; } }
	}
}
namespace DevExpress.XtraSplashScreen {
	public enum ShowMode { Form, Image }
	public class SplashScreen : SplashFormBase {
		Image imageCore;
		ShowMode usageMode;
		bool allowControlsInImageMode;
		public SplashScreen() {
			this.usageMode = ShowMode.Form;
			this.allowControlsInImageMode = false;
			this.AutoFitImage = true;
		}
		protected internal override void OnParentInternalLoad() {
			base.OnParentInternalLoad();
			if(TopMost) return;
			Form form = Properties.ParentFormInternal;
			if(form != null && form.WindowState == FormWindowState.Maximized) {
				TopMost = true;
				if(IsHandleCreated) Activate();
				TopMost = false; 
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(ShowMode == ShowMode.Form) {
				base.OnPaint(e);
				return;
			}
			e.Graphics.DrawImage(SplashImage ?? DefaultSplashImage, 0, 0, this.Width, this.Height);
		}
		protected override void OnPaintBackground(PaintEventArgs e) {
			if(ShowMode == ShowMode.Form) {
				base.OnPaintBackground(e);
				return;
			}
			if(DesignMode) e.Graphics.FillRectangle(Brushes.White, ClientRectangle);
		}
		protected override bool AllowClosing { get { return base.AllowClosing || (ShowMode == ShowMode.Image); } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SplashScreenShowMode"),
#endif
 Category("Display Options")]
		public ShowMode ShowMode {
			get { return this.usageMode; }
			set {
				if(this.usageMode != value) {
					this.usageMode = value;
					PrepareFormState();
					Invalidate();
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SplashScreenSplashImage"),
#endif
 Category("Display Options"), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image SplashImage {
			get { return this.imageCore; }
			set {
				this.imageCore = value;
				PrepareFormState();
				Invalidate();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SplashScreenAllowControlsInImageMode"),
#endif
 Category("Display Options"), DefaultValue(false)]
		public bool AllowControlsInImageMode {
			get { return this.allowControlsInImageMode; }
			set {
				if(this.allowControlsInImageMode != value) {
					this.allowControlsInImageMode = value;
					PrepareFormState();
					Invalidate();
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SplashScreenAutoFitImage"),
#endif
 Category("Display Options"), DefaultValue(true)]
		public bool AutoFitImage {
			get;
			set;
		}
		static Image defImageCore = null;
		internal static Image DefaultSplashImage {
			get {
				if(defImageCore == null) defImageCore = GetDefaultSplashImage();
				return defImageCore;
			}
		}
		static Image GetDefaultSplashImage() {
			Assembly asm = typeof(SplashScreen).Assembly;
			return new Bitmap(asm.GetManifestResourceStream("DevExpress.XtraEditors.SplashScreen.DefSplashImage.png"));
		}
		Size formSize = Size.Empty;
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			if(ShowMode == ShowMode.Form) this.formSize = Size;
		}
		void PrepareFormState() {
			if(ShowMode == ShowMode.Form) PrepareFormUsageMode();
			else PrepareImageUsageMode();
		}
		void PrepareFormUsageMode() {
			this.isSwitchFromForm = true;
			SetControlsVisisblilityState(true);
			if(this.formSize != Size.Empty) Size = this.formSize;
		}
		bool isSwitchFromForm = true;
		void PrepareImageUsageMode() {
			if(this.isSwitchFromForm) this.formSize = Size;
			this.isSwitchFromForm = false;
			SetControlsVisisblilityState(AllowControlsInImageMode);
			UpdateFormSizeCore(SplashImage != null ? SplashImage.Size : DefaultSplashImage.Size);
		}
		void UpdateFormSizeCore(Size size) {
			if(AutoFitImage && this.Size != size)
				this.Size = size;
		}
		void SetControlsVisisblilityState(bool state) {
			foreach(Control control in Controls) control.Visible = state;
		}
	}
	public class SplashScreenLayer : SplashFormBase {
		protected internal SplashFormLayerBase Layer { get; private set; }
		public SplashScreenLayer(Image image, bool useUserLocation) {
			this.Opacity = 0;
			this.ShouldUseUserLocation = useUserLocation;
			this.Layer = new SplashFormLayerBase(this, image);
		}
		public bool ShouldUseUserLocation { get; private set; }
		protected override DialogResult ShowDialogCore() {
			this.Layer.Alpha = GetAlpha();
			this.Layer.Show();
			return base.ShowDialogCore();
		}
		byte GetAlpha() {
			if(Properties.UseFadeInEffect) return 0x0;
			return 0xFF;
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			this.Layer.Update();
		}
		protected override void FadeInTimerTickCore(object sender, EventArgs e) {
			int alpha = this.Layer.Alpha + 10;
			if(alpha >= 250) alpha = 0xFF;
			this.Layer.Alpha = (byte)alpha;
			if(alpha == 0xFF)
				FadeTimer.Enabled = false;
		}
		bool allowClosing = false;
		protected override void FadeOutTimerTickCore(object sender, EventArgs e) {
			int alpha = this.Layer.Alpha - 10;
			if(alpha <= 10) alpha = 0;
			this.Layer.Alpha = (byte)alpha;
			if(alpha == 0) {
				FadeTimer.Enabled = false;
				this.allowClosing = true;
				this.Layer.Close();
				this.Layer = null;
				base.Close();
			}
		}
		protected override bool AllowClosing { get { return base.AllowClosing || this.allowClosing; } }
		protected override int FadeInterval { get { return 30; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.Layer != null) this.Layer.Dispose();
			}
			this.Layer = null;
			base.Dispose(disposing);
		}
	}
}
namespace DevExpress.XtraWaitForm {
	public enum ShowFormOnTopMode { Default, AboveParent, AboveAll, ObsoleteAboveParent}
	public class WaitForm : SplashFormBase {
		public WaitForm() {
			ShowOnTopMode = ShowFormOnTopMode.Default;
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			ApplyShowStrategy();
		}
		[Obsolete("Use ShowOnTopMode instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new public bool TopMost {
			get { return base.TopMost; }
			set { base.TopMost = value; }
		}
		public virtual void SetCaption(string caption) {
		}
		public virtual void SetDescription(string description) {
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("WaitFormShowOnTopMode"),
#endif
 Category("Window Style"), DefaultValue(ShowFormOnTopMode.Default)]
		public ShowFormOnTopMode ShowOnTopMode {
			get;
			set;
		}
		protected override bool AllowActivateParentOnDelayedClosing {
			get {
				if(!SplashScreenManager.ActivateParentOnWaitFormClosing) return false;
				return base.AllowActivateParentOnDelayedClosing;
			}
		}
		protected WaitFormShowModeSupportStrategyBase ShowModeHelper { get; private set; }
		protected override int FadeInterval { get { return 20; } }
		#region Showing Strategy
		protected virtual void ApplyShowStrategy() {
			ShowModeHelper = WaitFormShowModeSupportStrategyBase.Create(this);
			ShowModeHelper.Run();
		}
		#endregion
		#region Overrides
		protected override void OnClosing(CancelEventArgs e) {
			if(ShowModeHelper != null) {
				ShowModeHelper.Dispose();
				ShowModeHelper = null;
			}
			base.OnClosing(e);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(ShowModeHelper != null) {
					ShowModeHelper.Dispose();
					ShowModeHelper = null;
				}
			}
			base.Dispose(disposing);
		}
		#endregion
	}
	#region WaitForm Show Mode Support Strategy
	public abstract class WaitFormShowModeSupportStrategyBase : IDisposable {
		protected WaitFormShowModeSupportStrategyBase(WaitForm form) {
			this.Form = form;
		}
		~WaitFormShowModeSupportStrategyBase() { Dispose(false); }
		public static WaitFormShowModeSupportStrategyBase Create(WaitForm form) {
			ShowFormOnTopMode coerceMode = CoerceShowFormOnTopMode(form);
			if(IsDefaultMode(coerceMode))
				return new WaitFormDefaultShowModeSupportStrategy(form);
			if(coerceMode == ShowFormOnTopMode.AboveAll)
				return new WaitFormAboveAllShowModeSupportStrategy(form);
			if(coerceMode == ShowFormOnTopMode.ObsoleteAboveParent)
				return new WaitFormAboveAllClassicShowModeSupportStrategy(form);
			return null;
		}
		static ShowFormOnTopMode CoerceShowFormOnTopMode(WaitForm form) {
			ShowFormOnTopMode coerceMode = form.ShowOnTopMode;
			Form parentForm = form.Properties.ParentForm;
			if(parentForm != null && parentForm.IsMdiContainer && IsDefaultMode(form.ShowOnTopMode))
				coerceMode = ShowFormOnTopMode.ObsoleteAboveParent;
			return coerceMode;
		}
		static bool IsDefaultMode(ShowFormOnTopMode mode) {
			return mode == ShowFormOnTopMode.Default || mode == ShowFormOnTopMode.AboveParent;
		}
		protected WaitForm Form { get; private set; }
		protected Form ParentForm {
			get { return Form.Properties.ParentForm; }
		}
		protected bool DesignMode {
			get { return Form.Site != null ? Form.Site.DesignMode : false; }
		}
		protected bool IsParentReady {
			get {
				if(Form == null)
					return false;
				Form parent = Form.Properties.ParentForm;
				return parent != null && parent.IsHandleCreated;
			}
		}
		protected IntPtr ParentHandle {
			get { return Form.Properties.ParentHandle; }
		}
		protected void SetAboveAll(bool topMost) {
			if(!Form.InvokeRequired)
				SetAboveAllCore(topMost);
			else Form.BeginInvoke((ScreenAction<bool>)SetAboveAllCore, topMost);
		}
		protected void SetAboveAllCore(bool topMode) {
			if(IsParentReady) DoSetTopMostValue(Form, topMode);
		}
		protected void DoSetTopMostValue(Form form, bool value) {
			if(form != null) form.TopMost = value;
		}
		public abstract void Run();
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			Form = null;
		}
		#endregion
	}
	class WaitFormAboveAllShowModeSupportStrategy : WaitFormShowModeSupportStrategyBase {
		public WaitFormAboveAllShowModeSupportStrategy(WaitForm form) : base(form) { }
		public override void Run() {
			if(DesignMode)
				return;
			DoSetTopMostValue(Form, true);
		}
	}
	class WaitFormDefaultShowModeSupportStrategy : WaitFormShowModeSupportStrategyBase {
		public WaitFormDefaultShowModeSupportStrategy(WaitForm form) : base(form) {
			Form.Deactivate += OnDeactivate;
		}
		public override void Run() {
			if(DesignMode)
				return;
			if(!IsParentReady || Form.Properties.ParentHandle == IntPtr.Zero)
				return;
			Timer = CreateParentWatchTimer();
			Timer.Start();
		}
		protected Timer Timer { get; private set; }
		#region Handlers
		void OnDeactivate(object sender, EventArgs e) {
			if(!IsParentReady || ParentHandle == IntPtr.Zero)
				return;
			Form parentForm = ParentForm;
			if(parentForm != null) {
				parentForm.BeginInvoke((ScreenAction<IntPtr, IntPtr>)SetZOrder, ParentHandle, Form.Handle);
			}
		}
		#endregion
		#region ParentWatchTimer
		protected virtual Timer CreateParentWatchTimer() {
			Timer timer = new Timer();
			timer.Interval = 200;
			timer.Tick += OnParentWatchTimerTick;
			return timer;
		}
		protected virtual void DestroyParentWatchTimer() {
			if(Timer == null)
				return;
			Timer.Tick -= OnParentWatchTimerTick;
			Timer.Dispose();
			Timer = null;
		}
		bool shouldUpdateZOrder = true;
		protected virtual void OnParentWatchTimerTick(object sender, EventArgs e) {
			if(!IsParentReady || Timer == null)
				return;
			if(System.Windows.Forms.Form.ActiveForm == null) {
				shouldUpdateZOrder = false;
				return;
			}
			if(!shouldUpdateZOrder) Form.Activate();
			shouldUpdateZOrder = true;
			if(shouldUpdateZOrder) {
				Form parentForm = ParentForm;
				if(parentForm != null) {
					parentForm.BeginInvoke((ScreenAction<IntPtr, IntPtr>)SetZOrder, ParentHandle, Form.Handle);
				}
			}
		}
		#endregion
		#region Helpers
		protected void SetZOrder(IntPtr bottom, IntPtr top) {
			if(!SplashScreenManager.ActivateParentOnWaitFormClosing)
				return;
			SetZOrderCore(bottom, top);
			if(Form != null && Form.FormShadow != null) Form.FormShadow.UpdateZOrder(bottom);
		}
		protected void SetZOrderCore(IntPtr bottom, IntPtr top) {
			const int flags = 0x0002 | 0x0001; 
			NativeMethods.SetWindowPos(bottom, top, 0, 0, 0, 0, flags);
		}
		#endregion
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(Form != null) {
					Form.Deactivate -= OnDeactivate;
				}
				if(Timer != null) {
					Timer.Dispose();
					Timer = null;
				}
			}
			base.Dispose(disposing);
		}
	}
	class WaitFormAboveAllClassicShowModeSupportStrategy : WaitFormShowModeSupportStrategyBase {
		public WaitFormAboveAllClassicShowModeSupportStrategy(WaitForm form) : base(form) { }
		public override void Run() {
			if(DesignMode || !IsParentReady)
				return;
			Subscribe();
		}
		void OnActivateStateChanged(object sender, EventArgs e) {
			if(!IsParentReady)
				return;
			SetAboveAll(IsTopMostNeeded);
		}
		protected void Subscribe() {
			Form.Activated += OnActivateStateChanged;
			Form.Deactivate += OnActivateStateChanged;
			Form parentForm = ParentForm;
			if(parentForm != null) {
				parentForm.Activated += OnActivateStateChanged;
				parentForm.Deactivate += OnActivateStateChanged;
			}
		}
		protected void Unsubscribe() {
			if(Form == null) return;
			Form.Activated -= OnActivateStateChanged;
			Form.Deactivate -= OnActivateStateChanged;
			Form parentForm = ParentForm;
			if(parentForm != null) {
				parentForm.Activated -= OnActivateStateChanged;
				parentForm.Deactivate -= OnActivateStateChanged;
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				Unsubscribe();
			}
			base.Dispose(disposing);
		}
		bool IsTopMostNeeded { get { return System.Windows.Forms.Form.ActiveForm != null; } }
	}
	#endregion
}
