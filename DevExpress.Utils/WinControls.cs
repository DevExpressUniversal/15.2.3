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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Specialized;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Skins;
using DevExpress.XtraEditors.Drawing;
using System.Reflection;
using DevExpress.Skins.XtraForm;
using DevExpress.Utils.Drawing.Helpers;
using System.Drawing.Imaging;
using DevExpress.Utils.Controls;
using System.Diagnostics;
using DevExpress.LookAndFeel.Design;
using DevExpress.Data;
using DevExpress.Utils.Text;
using FormShadowHelpers = DevExpress.Utils.FormShadow.Helpers;
using DevExpress.Utils.FormShadow;
using System.Security;
using DevExpress.Utils.About;
using System.Collections.Generic;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraEditors {
	[Description("Allows an end-user to resize neighboring controls."),
	 ToolboxTabName(AssemblyInfo.DXTabNavigation),
	  ToolboxBitmap(typeof(DevExpress.Utils.ToolBoxIcons.ToolboxIconsRootNS), "SplitterControl")
	]
	[DXToolboxItem(DXToolboxItemKind.Free)]
	public class SplitterControl : Splitter, ISupportLookAndFeel, IXtraSerializable, IXtraSerializable2 { 
		UserLookAndFeel lookAndFeel;
		AppearanceObject appearance;
		ObjectPainter painter;
		bool isLoading = true;
		public SplitterControl() {
			SetStyle(ControlConstants.DoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
			this.lookAndFeel = new ControlUserLookAndFeel(this);
			this.lookAndFeel.StyleChanged += new EventHandler(OnLookAndFeelStyleChanged);
			this.appearance = new AppearanceObject();
			this.appearance.Changed += new EventHandler(OnAppearance_Changed);
			UpdatePainter();
			this.isLoading = false;
			SetBoundsCore(Location.X, Location.Y, Width, Height, BoundsSpecified.Size);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				this.appearance.Changed -= new EventHandler(OnAppearance_Changed);
				if(this.lookAndFeel != null) {
					this.lookAndFeel.StyleChanged -= new EventHandler(OnLookAndFeelStyleChanged);
					this.lookAndFeel.Dispose();
					this.lookAndFeel = null;
				}
			}
			base.Dispose(disposing);
		}
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		protected virtual void UpdatePainter() {
			this.painter = SplitterHelper.GetPainter(LookAndFeel);
		}
		protected ObjectPainter Painter { get { return painter; } }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("SplitterControlBackColor"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor {
			get { return GetColor(Appearance.BackColor, DefaultAppearance.BackColor); }
			set {
				if(value == Color.Transparent) return;
				Appearance.BackColor = value;
			}
		}
		void ResetAppearance() { Appearance.Reset(); }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("SplitterControlAppearance"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Appearance { get { return appearance; } }
		bool ShouldSerializeLookAndFeel() { return LookAndFeel.ShouldSerialize(); }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("SplitterControlLookAndFeel"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		void OnLookAndFeelStyleChanged(object sender, System.EventArgs e) {
			this.defaultAppearance = null;
			UpdatePainter();
			OnAppearance_Changed(sender, e);
			SetBoundsCore(Location.X, Location.Y, Width, Height, BoundsSpecified.Size);
		}
		protected void OnAppearance_Changed(object sender, EventArgs e) {
			if(IsHandleCreated) {
				OnBackColorChanged(e);
				Invalidate();
			}
		}
		AppearanceDefault defaultAppearance = null;
		protected AppearanceDefault DefaultAppearance {
			get {
				if(defaultAppearance == null)
					defaultAppearance = CreateDefaultAppearance();
				return defaultAppearance;
			}
		}
		protected virtual AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault result;
			if(Painter != null)
				result = Painter.DefaultAppearance.Clone() as AppearanceDefault;
			else
				result = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control);
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				SkinElement element = CommonSkins.GetSkin(LookAndFeel)[IsVertical ? CommonSkins.SkinSplitter : CommonSkins.SkinSplitterHorz];
				if(element != null) {
					element.Apply(result, LookAndFeel);
				}
			}
			return result;
		}
		protected override void OnPaint(PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(e)) {
				Form frm = Parent as Form;
				if(frm != null && frm.IsMdiContainer) {
					using(SolidBrush sb = new SolidBrush(CommonSkins.GetSkin(LookAndFeel).GetSystemColor(SystemColors.Control))) {
						cache.FillRectangle(sb, ClientRectangle);
					}
				}
				ObjectPainter.DrawObject(cache, Painter, CreateViewInfo());
			}
		}
		bool IsVertical {
			get { return Dock == DockStyle.Left || Dock == DockStyle.Right; }
		}
		protected virtual SplitterInfoArgs CreateViewInfo() {
			SplitterInfoArgs res = new SplitterInfoArgs(IsVertical);
			AppearanceObject paintAppearance = new AppearanceObject(Appearance, DefaultAppearance);
			res.SetAppearance(paintAppearance);
			res.Bounds = ClientRectangle;
			if(Enabled && IsHandleCreated && res.Bounds.Contains(PointToClient(MousePosition))) {
				res.State = ObjectState.Hot;
			}
			return res;
		}
		protected override void OnMouseEnter(EventArgs e) {
			Invalidate();
			base.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			Invalidate();
			base.OnMouseLeave(e);
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			Size prevSize = Size;
			if(!isLoading) {
				Size size = ObjectPainter.CalcObjectMinBounds(null, Painter, CreateViewInfo()).Size;
				if(!size.IsEmpty) {
					if(IsVertical)
						width = size.Width;
					else
						height = size.Height;
				}
			}
			base.SetBoundsCore(x, y, width, height, specified);
			if(!isLoading && Visible && Parent != null) {
				bool equals = (IsVertical ? Width == prevSize.Width : Height == prevSize.Height);
				if(!equals) {
					Parent.PerformLayout(this, "Size");
				}
			}
		}
		Color GetColor(Color color, Color defColor) {
			if(color == Color.Empty) return defColor;
			return color;
		}
		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);
			DevExpress.Utils.CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		#region IXtraSerializable Members
		public void OnEndDeserializing(string restoredVersion) { }
		public void OnEndSerializing() { }
		public void OnStartDeserializing(LayoutAllowEventArgs e) { }
		public void OnStartSerializing() { }
		#endregion
		#region IXtraSerializable2 Members
		void IXtraSerializable2.Deserialize(IList props) {
			foreach(XtraPropertyInfo item in props) {
				if(item.Name == "SplitPosition") {
					try {
						SplitPosition = (int)Convert.ChangeType(item.Value, item.PropertyType);
					}
					catch { }
				}
			}
		}
		XtraPropertyInfo[] IXtraSerializable2.Serialize() {
			XtraPropertyInfo info = new XtraPropertyInfo("SplitPosition", typeof(object), SplitPosition);
			return new XtraPropertyInfo[] { info };
		}
		#endregion
	}
	[System.Runtime.InteropServices.ComVisible(false)]
	public interface IDXControl {
		void OnAppearanceChanged(object sender);
		void OnLookAndFeelStyleChanged(object sender);
		Control Control { get; }
	}
	public class FormAppearanceObject : AppearanceObject {
		protected override bool ShouldSerializeFont() {
			return false;
		}
	}
	public class FormControlHelper : ControlHelper {
		public FormControlHelper(IDXControl owner, bool isForm) : base(owner, isForm) { }
		protected override AppearanceObject CreateAppearance() {
			return new FormAppearanceObject();
		}
	}
	public class ControlHelper : IDisposable {
		IDXControl owner;
		UserLookAndFeel lookAndFeel;
		AppearanceObject appearance;
		public ControlHelper(IDXControl owner, bool isForm, bool isUserControl) {
			this.owner = owner;
			this.lookAndFeel = CreateLookAndFeel(isForm, isUserControl);
			this.lookAndFeel.StyleChanged += new EventHandler(OnLookAndFeelStyleChanged);
			this.appearance = CreateAppearance();
			this.appearance.Changed += new EventHandler(OnAppearance_Changed);
		}
		public ControlHelper(IDXControl owner, bool isForm) : this(owner, isForm, false) { }
		protected virtual UserLookAndFeel CreateLookAndFeel(bool isForm, bool isUserControl) {
			if(isForm)
				return new FormUserLookAndFeel(this.owner.Control as Form);
			else if(isUserControl)
				return new ContainerUserLookAndFeel(this.owner.Control);
			else
				return new ControlUserLookAndFeel(this.owner.Control);
		}
		protected virtual AppearanceObject CreateAppearance() {
			return new AppearanceObject();
		}
		public static Form GetSafeActiveForm() {
			Form res = Form.ActiveForm;
			if(res != null && res.InvokeRequired) return null;
			return res;
		}
		public static bool IsHMouseWheel(MouseEventArgs e) {
			DXMouseEventArgs de = e as DXMouseEventArgs;
			return de != null && de.IsHMouseWheel;
		}
		public static DXMouseEventArgs GenerateMouseHWheel(ref Message m, Control c) {
			Int16 tilt = (Int16)DevExpress.XtraPrinting.Native.Win32.HiWord(m.WParam);
			Int32 keys = DevExpress.XtraPrinting.Native.Win32.LoWord(m.WParam);
			Point p = new Point();
			p.X = DevExpress.XtraPrinting.Native.Win32.LoWord(m.LParam);
			p.Y = DevExpress.XtraPrinting.Native.Win32.HiWord(m.LParam);
			if(c.IsHandleCreated) p = c.PointToClient(p);
			DXMouseEventArgs res = new DXMouseEventArgs(MouseButtons.None, 0, p.X, p.Y, tilt);
			res.ishMouseWheel = true;
			return res;
		}
		public static void SuspendLayout(Control control) {
			if(control is Form)
				SuspendFormLayout((Form)control);
			else
				control.SuspendLayout();
		}
		public static void ResumeLayout(Control control, bool performLayout) {
			if(control is Form)
				ResumeFormLayout((Form)control, performLayout);
			else
				control.ResumeLayout(performLayout);
		}
		public static void ResumeLayout(Control control) {
			if(control is Form)
				ResumeFormLayout((Form)control);
			else
				control.ResumeLayout();
		}
		static void ResumeFormLayout(Form form, bool performLayout) {
			form.ResumeLayout(performLayout);
		}
		static void ResumeFormLayout(Form form) {
			form.ResumeLayout();
		}
		static void SuspendFormLayout(Form form) {
			form.SuspendLayout();
		}
		public IDXControl Owner { get { return owner; } }
		public virtual void Dispose() {
			this.appearance.Changed -= new EventHandler(OnAppearance_Changed);
			if(this.lookAndFeel != null) {
				this.lookAndFeel.StyleChanged -= new EventHandler(OnLookAndFeelStyleChanged);
				this.lookAndFeel.Dispose();
				this.lookAndFeel = null;
			}
		}
		public AppearanceObject Appearance { get { return appearance; } }
		public UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		void OnLookAndFeelStyleChanged(object sender, System.EventArgs e) {
			Owner.OnLookAndFeelStyleChanged(sender);
		}
		protected void OnAppearance_Changed(object sender, EventArgs e) {
			Owner.OnAppearanceChanged(sender);
		}
	}
	[ToolboxTabName(AssemblyInfo.DXTabNavigation), ToolboxBitmap(typeof(DevExpress.Utils.ToolBoxIcons.ToolboxIconsRootNS), "XtraUserControl"),
	Description("A skinnable UserControl."),
	HiddenToolboxItem, DXToolboxItem(DXToolboxItemKind.Free)]
	public class XtraUserControl : UserControl, IDXControl, ISupportLookAndFeel, IXtraResizableControl {
		ControlHelper helper;
		ObjectPainter painter = null;
		public XtraUserControl() {
#if DEBUGTEST
			Debug.Assert(!IsHandleCreated);
			base.DestroyHandle();
#endif
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			this.helper = new ControlHelper(this, false, true);
			OnAppearance_Changed(helper);
			EnableIXtraResizeableControlInterfaceProxy = true;
		}
		internal bool IsRootInDesigner {
			get {
				if(!DesignMode) return false;
				return Parent != null && Parent.GetType().Name == "OverlayControl";
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				this.helper.Dispose();
			}
			base.Dispose(disposing);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ISite Site {
			get {
				return base.Site;
			}
			set {
				base.Site = value;
				if(value != null) {
					((DevExpress.LookAndFeel.Design.UserLookAndFeelDefault)DevExpress.LookAndFeel.UserLookAndFeel.Default).UpdateDesignTimeLookAndFeelEx(this);
				}
			}
		}
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		protected ControlHelper Helper { get { return helper; } }
		void ResetAppearance() { Appearance.Reset(); }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("XtraUserControlAppearance"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Appearance {
			get { return (Helper == null) ? AppearanceObject.Dummy : Helper.Appearance; }
		}
		bool ShouldSerializeLookAndFeel() { return (Helper != null && Helper.LookAndFeel != null) && LookAndFeel.ShouldSerialize(); }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("XtraUserControlLookAndFeel"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ContainerUserLookAndFeel LookAndFeel { get { return (ContainerUserLookAndFeel)Helper.LookAndFeel; } }
		UserLookAndFeel ISupportLookAndFeel.LookAndFeel { get { return LookAndFeel; } }
		void IDXControl.OnLookAndFeelStyleChanged(object sender) {
			this.defaultAppearance = null;
			this.painter = null;
			OnAppearance_Changed(sender);
		}
		void IDXControl.OnAppearanceChanged(object sender) {
			OnAppearance_Changed(sender);
		}
		Font GetFont() {
			AppearanceObject app = Appearance.GetAppearanceByOption(AppearanceObject.optUseFont);
			if(app.Options.UseFont || DefaultAppearance.Font == null) return app.Font;
			return DefaultAppearance.Font;
		}
		Control IDXControl.Control { get { return this; } }
		internal Font GetBaseFont() { return base.Font; }
		protected void OnAppearance_Changed(object sender) {
			if(IsHandleCreated) {
				OnBackColorChanged(EventArgs.Empty);
				Invalidate();
			}
			if(!base.Font.Equals(GetFont())) {
				useBaseFont = true;
				base.Font = GetFont();
				useBaseFont = false;
			}
		}
		bool useBaseFont = false;
		[
#if !SL
	DevExpressUtilsLocalizedDescription("XtraUserControlFont"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Font Font {
			get {
				if(useBaseFont) return base.Font;
				return GetFont();
			}
			set { Appearance.Font = value; }
		}
		public override void ResetBackColor() { BackColor = Color.Empty; }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("XtraUserControlBackColor"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor {
			get { return GetBackColor(Appearance.BackColor, DefaultAppearance.BackColor); }
			set {
				if(GetDesigner() != null && GetDesigner().Loading) return;
				Appearance.BackColor = value;
			}
		}
		public override void ResetForeColor() { ForeColor = Color.Empty; }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("XtraUserControlForeColor"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color ForeColor {
			get { return LookAndFeelHelper.CheckTransparentForeColor(LookAndFeel, GetColor(Appearance.ForeColor, DefaultAppearance.ForeColor), this); }
			set { Appearance.ForeColor = value; }
		}
		protected virtual Color GetBackColor(Color color, Color defColor) {
			if(color == Color.Empty) {
				if(FindForm() == null) return defColor;
				return LookAndFeelHelper.GetEmptyBackColor(LookAndFeel, this);
			}
			return color;
		}
		Color GetColor(Color color, Color defColor) {
			if(color == Color.Empty)
				return defColor;
			return color;
		}
		AppearanceDefault defaultAppearance = null;
		protected AppearanceDefault DefaultAppearance {
			get {
				if(defaultAppearance == null)
					defaultAppearance = CreateDefaultAppearance();
				return defaultAppearance;
			}
		}
		protected virtual AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault res = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control);
			if(LookAndFeel != null && LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				SkinElement form = GetSkin();
				form.Apply(res, LookAndFeel);
				LookAndFeelHelper.CheckColors(LookAndFeel, res, this);
			}
			return res;
		}
		SkinElement GetSkin() {
			return CommonSkins.GetSkin(LookAndFeel)[CommonSkins.SkinGroupPanelNoBorder];
		}
		protected ObjectPainter Painter {
			get {
				if(painter == null) painter = CreatePainter();
				return painter;
			}
		}
		protected virtual ObjectPainter CreatePainter() {
			switch(LookAndFeel.ActiveStyle) {
				case ActiveLookAndFeelStyle.Skin: return new ControlSkinPainter(LookAndFeel);
			}
			return new ControlPainter();
		}
		class ControlPainter : StyleObjectPainter {
			public override void DrawObject(ObjectInfoArgs e) {
				GetStyle(e).DrawBackground(e.Cache, e.Bounds);
			}
		}
		class ControlSkinPainter : SkinCustomPainter {
			public ControlSkinPainter(ISkinProvider provider) : base(provider) { }
			protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) {
				SkinElementInfo info = new SkinElementInfo(CommonSkins.GetSkin(Provider)[CommonSkins.SkinGroupPanelNoBorder], e.Bounds);
				return info;
			}
		}
		bool loadFired = false;
		protected override void OnLoad(EventArgs e) {
			if(this.loadFired) return;
			if(IsHandleCreated)
				BeginInvoke(new MethodInvoker(UpdateBackColor));
			OnFirstLoad();
			this.loadFired = true;
			base.OnLoad(e);
		}
		void UpdateBackColor() {
			OnBackColorChanged(EventArgs.Empty);
		}
		protected override void OnForeColorChanged(EventArgs e) {
			this.defaultAppearance = null;
			base.OnForeColorChanged(e);
		}
		protected override void OnBackColorChanged(EventArgs e) {
			this.defaultAppearance = null;
			base.OnBackColorChanged(e);
		}
		protected override void OnControlAdded(ControlEventArgs e) {
			base.OnControlAdded(e);
			IXtraResizableControl ixChild = GetInnerIXtraResizableControl();
			if(ixChild != null) foreach(EventHandler eh in new List<EventHandler>(subscriptions)) {
					SubscribeIXChild(ixChild, eh, false);
					eh(this, null);
				}
		}
		protected override void OnControlRemoved(ControlEventArgs e) {
			base.OnControlRemoved(e);
			IXtraResizableControl ixChild = e.Control as IXtraResizableControl;
			if(subscriptions.Count > 0 && ixChild != null && Controls.Count == 0) foreach(EventHandler eh in new List<EventHandler>(subscriptions)) {
					UnSubscribeIXChild(ixChild, eh, false);
					eh(this, null);
				}
		}
		protected override void OnParentChanged(EventArgs e) {
			this.defaultAppearance = null;
			base.OnParentChanged(e);
			OnBackColorChanged(e);
		}
		protected virtual void OnFirstLoad() { }
		bool HasSkinnedBackground {
			get {
				if(LookAndFeel.ActiveLookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin)
					return false;
				SkinElement elem = CommonSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[CommonSkins.SkinGroupPanelNoBorder];
				return elem.Image != null && elem.Image.Image != null;
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(BackgroundImage == null) {
				if(BackColor == Color.Transparent && Appearance.BackColor2.IsEmpty && !HasSkinnedBackground) {
					base.OnPaint(e);
					return;
				}
				using(GraphicsCache cache = new GraphicsCache(e)) {
					StyleObjectInfoArgs info = new StyleObjectInfoArgs();
					info.Bounds = ClientRectangle;
					info.SetAppearance(new AppearanceObject(Appearance, DefaultAppearance));
					ObjectPainter.DrawObject(cache, Painter, info);
				}
			}
			RaisePaintEvent(this, e);
		}
		protected override object GetService(Type service) {
			return base.GetService(service);
		}
		AmbientProperties current;
		AmbientProperties GetAmbientProperties() {
			if(current != null) {
				if(current.Font.Equals(AppearanceObject.DefaultFont)) return current;
				current = null;
			}
			if(current == null) {
				current = new AmbientProperties() { Font = AppearanceObject.DefaultFont };
			}
			return current;
		}
		IDesignerHost GetDesigner() {
			if(Site == null) return null;
			return Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
		}
		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);
			DevExpress.Utils.CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		[DefaultValue(true), Browsable(false)]
		public bool EnableIXtraResizeableControlInterfaceProxy { get; set; }
		protected virtual IXtraResizableControl GetInnerIXtraResizableControl() {
			if(Disposing || Controls == null || Controls.Count != 1 || !EnableIXtraResizeableControlInterfaceProxy || IsDockFill()) return null;
			IXtraResizableControl ixChild = Controls[0] as IXtraResizableControl;
			return ixChild;
		}
		bool IsDockFill() {
			return Controls.Count == 1 && Controls[0].Dock != DockStyle.Fill;
		}
		Size IXtraResizableControl.MinSize {
			get {
				IXtraResizableControl ixChild = GetInnerIXtraResizableControl();
				return ixChild == null ? new Size(1, 1) : ixChild.MinSize;
			}
		}
		Size IXtraResizableControl.MaxSize {
			get {
				IXtraResizableControl ixChild = GetInnerIXtraResizableControl();
				return ixChild == null ? new Size(0, 0) : ixChild.MaxSize;
			}
		}
		void SubscribeIXChild(IXtraResizableControl ixChild, EventHandler eh, bool addToCollection) {
			if(ixChild != null) ixChild.Changed += eh;
			if(addToCollection) subscriptions.Add(eh);
		}
		void UnSubscribeIXChild(IXtraResizableControl ixChild, EventHandler eh, bool removeFromCollection) {
			if(ixChild != null) ixChild.Changed -= eh;
			if(removeFromCollection) subscriptions.Remove(eh);
		}
		List<EventHandler> subscriptions = new List<EventHandler>();
		event EventHandler IXtraResizableControl.Changed {
			add {
				IXtraResizableControl ixChild = GetInnerIXtraResizableControl();
				SubscribeIXChild(ixChild, value, true);
			}
			remove {
				IXtraResizableControl ixChild = GetInnerIXtraResizableControl();
				UnSubscribeIXChild(ixChild, value, true);
			}
		}
		bool IXtraResizableControl.IsCaptionVisible {
			get {
				IXtraResizableControl ixChild = GetInnerIXtraResizableControl();
				return ixChild == null ? false : ixChild.IsCaptionVisible;
			}
		}
	}
	[DXToolboxItem(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class MouseWheelContainerForm : Form, IMouseWheelContainer {
		#region IMouseWheelContainer
		Control IMouseWheelContainer.Control {
			get { return this; }
		}
		void IMouseWheelContainer.BaseOnMouseWheel(MouseEventArgs e) {
			if(ControlHelper.IsHMouseWheel(e))
				OnMouseHWheel(e);
			else base.OnMouseWheel(e);
		}
		#endregion IMouseWheelContainer
		protected override void OnMouseWheel(MouseEventArgs e) {
			MouseWheelHelper.DoMouseWheel(this, e);
		}
		protected virtual void OnMouseHWheel(MouseEventArgs e) { }
	}
	public enum FormBorderEffect {
		None,
		Default,
		Shadow,
		Glow
	}
	public interface ICustomDrawNonClientArea {
		bool IsCustomDrawNonClientArea { get; }
	}
	public interface IGlassForm {
		bool IsGlassForm { get; }
	}
	public class XtraForm : MouseWheelContainerForm, ISupportLookAndFeel, IDXControl, ICustomDrawNonClientArea, IGlassForm {
		static bool suppressDeactivation;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool SuppressDeactivation { get { return suppressDeactivation; } set { suppressDeactivation = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static Form DeactivatedForm { get; set; }
		bool allowMdiBar = false;
		bool closeBox = true;
		private static readonly object formLayoutChangedEvent = new object();
		ImageCollection htmlImages;
		string htmlText = string.Empty;
		ControlHelper helper;
		bool allowFormSkin;
		FormPainter painter = null;
		Form prevMdiChild = null;
		Size minimumClientSize;
		Size maximumClientSize;
		string textMdiTab;
		FormShowStrategyBase showingStrategy;
		static XtraForm() { DevExpress.Utils.Design.DXAssemblyResolverEx.Init(); }
		public XtraForm() {
#if DEBUGTEST
			Debug.Assert(!IsHandleCreated);
			base.DestroyHandle();
#endif
			this.htmlImages = null;
			this.allowFormSkin = true;
			CheckRightToLeft();
			this.helper = CreateHelper();
			OnAppearance_Changed(helper);
			UpdateSkinPainter();
			this.minimumClientSize = Size.Empty;
			this.maximumClientSize = Size.Empty;
			this.showingStrategy = CreateShowingStrategy();
		}
		bool showMdiChildCaptionInParentTitle;
		[DefaultValue(false)]
		public virtual bool ShowMdiChildCaptionInParentTitle {
			get { return showMdiChildCaptionInParentTitle; }
			set {
				if(ShowMdiChildCaptionInParentTitle == value)
					return;
				showMdiChildCaptionInParentTitle = value;
				OnShowMdiChildCaptionInParentTitle();
			}
		}
		string mdiChildCaptionFormatString = "{1} - {0}";
		[DefaultValue("{1} - {0}")]
		public virtual string MdiChildCaptionFormatString {
			get { return mdiChildCaptionFormatString; }
			set {
				if(MdiChildCaptionFormatString == value)
					return;
				mdiChildCaptionFormatString = value;
				OnShowMdiChildCaptionInParentTitle();
			}
		}
		protected virtual void OnShowMdiChildCaptionInParentTitle() {
			if(FormPainter != null)
				FormPainter.ForceFrameChanged();
			Refresh();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event LayoutEventHandler FormLayoutChanged {
			add { Events.AddHandler(formLayoutChangedEvent, value); }
			remove { Events.RemoveHandler(formLayoutChangedEvent, value); }
		}
		protected void RaiseFormLayoutChanged(LayoutEventArgs e) {
			LayoutEventHandler handler = (LayoutEventHandler)Events[formLayoutChangedEvent];
			if(handler != null) handler(this, e);
		}
		protected virtual void CheckRightToLeft() {
			RightToLeft = WindowsFormsSettings.GetRightToLeft(this);
			RightToLeftLayout = WindowsFormsSettings.GetIsRightToLeftLayout(this);
		}
		protected virtual ControlHelper CreateHelper() {
			return new FormControlHelper(this, true);
		}
		protected FormShowStrategyBase ShowingStrategy { get { return showingStrategy; } }
		protected virtual FormShowStrategyBase CreateShowingStrategy() {
			return FormShowStrategyBase.Create(this, ShowMode);
		}
		protected virtual bool InvisibleDueShowingStrategy {
			get { return (showingStrategy is AfterInitializationFormShowStrategy) && ShowingStrategy.AllowRestore; }
		}
		public static bool CheckInvisibleDueShowingStrategy(XtraForm form) {
			return (form != null) && form.InvisibleDueShowingStrategy;
		}
		protected virtual FormShowMode ShowMode { get { return FormShowMode.Default; } }
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			if(ShowingStrategy.AllowRestore)
				ShowingStrategy.Restore();
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				if(painter != null) painter.Dispose();
				this.painter = null;
				if(helper != null) this.helper.Dispose();
				helper = null;
				ReleaseFormShadow();
			}
		}
		int isInitializing = 0;
		bool forceInitialized = false;
		protected internal virtual bool ShouldShowMdiBar {
			get {
				if(!IsMdiContainer || ActiveMdiChild == null || ActiveMdiChild.WindowState != FormWindowState.Maximized) return false;
				if(MainMenuStrip != null || Menu != null) return false;
				return AllowMdiBar;
			}
		}
		protected bool IsInitializing { get { return !forceInitialized && (this.isInitializing != 0 || IsLayoutSuspendedCore); } }
		protected bool IsApplyingLocalizableResourcesInDesignTime {
			get { return DesignMode && StackTraceHelper.CheckStackFrame("ApplyResources", typeof(ComponentResourceManager)); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ISite Site {
			get {
				return base.Site;
			}
			set {
				base.Site = value;
				if(value != null) {
					((DevExpress.LookAndFeel.Design.UserLookAndFeelDefault)DevExpress.LookAndFeel.UserLookAndFeel.Default).UpdateDesignTimeLookAndFeelEx(this);
				}
			}
		}
		FormBorderEffect formBorderEffect = FormBorderEffect.Default;
		[DefaultValue(FormBorderEffect.Default)]
		public FormBorderEffect FormBorderEffect {
			get { return formBorderEffect; }
			set {
				if(formBorderEffect == value)
					return;
				formBorderEffect = value;
				OnFormBorderEffectChanged();
			}
		}
		void OnFormBorderEffectChanged() {
			if(IsInitializing || DesignMode) return;
			UpdateFormBorderEffect();
		}
		Color activeGlowColorCore;
		public Color ActiveGlowColor {
			get { return activeGlowColorCore; }
			set {
				if(ActiveGlowColor == value) return;
				activeGlowColorCore = value;
				OnActiveGlowColorChanged();
			}
		}
		bool ShouldSerializeActiveGlowColor() { return !ActiveGlowColor.IsEmpty; }
		void ResetActiveGlowColor() { ActiveGlowColor = Color.Empty; }
		Color inactiveGlowColorCore;
		public Color InactiveGlowColor {
			get { return inactiveGlowColorCore; }
			set {
				if(InactiveGlowColor == value) return;
				inactiveGlowColorCore = value;
				OnInactiveGlowColorChanged();
			}
		}
		bool ShouldSerializeInactiveGlowColor() { return !InactiveGlowColor.IsEmpty; }
		void ResetInactiveGlowColor() { InactiveGlowColor = Color.Empty; }
		protected virtual void OnActiveGlowColorChanged() {
			if(IsInitializing || DesignMode) return;
			if(FormShadow != null)
				FormShadow.ActiveGlowColor = ActiveGlowColor;
		}
		protected virtual void OnInactiveGlowColorChanged() {
			if(IsInitializing || DesignMode) return;
			if(FormShadow != null)
				FormShadow.InactiveGlowColor = InactiveGlowColor;
		}
		bool IsOnePixelBorder {
			get {
				if(FormPainter == null)
					return false;
				SkinElement border = FormPainter.GetSkinFrameLeft();
				return border.Size.MinSize.Width == 1;
			}
		}
		protected virtual FormBorderEffect GetFormBorderEffect() {
			if(FormBorderEffect != XtraEditors.FormBorderEffect.Default)
				return FormBorderEffect;
			if(FormPainter == null)
				return XtraEditors.FormBorderEffect.None;
			if(IsOnePixelBorder) return XtraEditors.FormBorderEffect.Shadow;
			return XtraEditors.FormBorderEffect.None;
		}
		void UpdateFormBorderEffect() {
			if(DesignMode) return;
			if(GetFormBorderEffect() != XtraEditors.FormBorderEffect.None && !IsMdiChild && Parent == null) {
				InitFormShadow();
				if(Owner != null)
					FormShadow.SetOwner(Owner.Handle);
			}
			else ReleaseFormShadow();
		}
		FormShadow formShadowCore;
		protected FormShadow FormShadow {
			get { return formShadowCore; }
		}
		void InitFormShadow() {
			if(FormShadow == null) {
				formShadowCore = CreateFormShadow();
				FormShadow.Form = this;
			}
			FormShadow.BeginUpdate();
			FormShadow.ActiveGlowColor = activeGlowColorCore;
			FormShadow.InactiveGlowColor = inactiveGlowColorCore;
			FormShadow.IsGlow = (GetFormBorderEffect() == XtraEditors.FormBorderEffect.Glow);
			FormShadow.AllowResizeViaShadows = IsOnePixelBorder && IsSizableWindow;
			FormShadow.EndUpdate();
		}
		protected virtual bool IsSizableWindow {
			get { return FormBorderStyle == FormBorderStyle.Sizable || FormBorderStyle == FormBorderStyle.SizableToolWindow; }
		}
		void ReleaseFormShadow() {
			if(FormShadow != null) {
				FormShadow.Form = null;
				FormShadow.Dispose();
				this.formShadowCore = null;
			}
		}
		protected virtual FormShadow CreateFormShadow() {
			return new XtraFormShadow();
		}
		#region MouseWheel
		public static bool ProcessSmartMouseWheel(Control control, MouseEventArgs e) {
			return MouseWheelHelper.ProcessSmartMouseWheel(control, e);
		}
		public static bool IsAllowSmartMouseWheel(Control control, MouseEventArgs e) {
			return MouseWheelHelper.IsAllowSmartMouseWheel(control, e);
		}
		public static bool IsAllowSmartMouseWheel(Control control) {
			return MouseWheelHelper.IsAllowSmartMouseWheel(control);
		}
		#endregion MouseWheel
		public new void SuspendLayout() {
			base.SuspendLayout();
			isInitializing++;
		}
		void CheckForceLookAndFeelChangedCore() {
			if(this.isInitializing != 0) return;
			if(LayoutSuspendCountCore == 1 && this.shouldUpdateLookAndFeelOnResumeLayout) {
				this.forceInitialized = true;
				try {
					OnLookAndFeelChangedCore();
				}
				finally {
					this.forceInitialized = false;
				}
			}
		}
		public new void ResumeLayout() { ResumeLayout(true); }
		public new void ResumeLayout(bool performLayout) {
			if(this.isInitializing > 0)
				this.isInitializing--;
			if(this.isInitializing == 0) {
				if(delayedFontChanged) OnFontChanged(EventArgs.Empty);
				CheckMinimumSize();
				CheckMaximumSize();
				CheckForceLookAndFeelChangedCore();
			}
			base.ResumeLayout(performLayout);
			if(!IsInitializing) {
				CheckMinimumSize();
				CheckMaximumSize();
			}
		}
		void CheckMaximumSize() {
			if(this.maximumSize == null) return;
			Size msize = (Size)maximumSize;
			if(!msize.IsEmpty) {
				if(msize.Width > 0) msize.Width = Math.Max(msize.Width, Size.Width);
				if(msize.Height > 0) msize.Height = Math.Max(msize.Height, Size.Height);
				if(this.minimumSize != null && !this.minimumSize.Value.IsEmpty) {
					if(this.maximumSize.Value.Width == this.minimumSize.Value.Width)
						msize.Width = Size.Width;
					if(this.maximumSize.Value.Height == this.minimumSize.Value.Height)
						msize.Height = Size.Height;
				}
			}
			this.maximumSize = null;
			base.MaximumSize = msize;
		}
		void CheckMinimumSize() {
			if(this.minimumSize == null) return;
			Size msize = (Size)minimumSize;
			if(!msize.IsEmpty) {
				if(msize.Width > 0) msize.Width = Math.Min(msize.Width, Size.Width);
				if(msize.Height > 0) msize.Height = Math.Min(msize.Height, Size.Height);
				if(this.maximumSize != null && !this.maximumSize.Value.IsEmpty) {
					if(this.maximumSize.Value.Width == this.minimumSize.Value.Width)
						msize.Width = Size.Width;
					if(this.maximumSize.Value.Height == this.minimumSize.Value.Height)
						msize.Height = Size.Height;
				}
			}
			this.minimumSize = null;
			base.MinimumSize = msize;
		}
		protected override void OnLayout(LayoutEventArgs levent) {
			if(this.shouldUpdateLookAndFeelOnResumeLayout) {
				CheckForceLookAndFeelChangedCore();
			}
			base.OnLayout(levent);
			RaiseFormLayoutChanged(levent);
		}
		protected override void ScaleCore(float x, float y) {
			MaximumClientSize = new Size((int)Math.Round(MaximumClientSize.Width * x), (int)Math.Round(MaximumClientSize.Height * y));
			base.ScaleCore(x, y);
			MinimumClientSize = new Size((int)Math.Round(MinimumClientSize.Width * x), (int)Math.Round(MinimumClientSize.Height * y));
		}
		PropertyInfo piLayout = null;
		internal bool IsLayoutSuspendedCore {
			get {
				if(piLayout == null) piLayout = typeof(Control).GetProperty("IsLayoutSuspended", BindingFlags.Instance | BindingFlags.NonPublic);
				if(piLayout != null) return (bool)piLayout.GetValue(this, null);
				return false;
			}
		}
		FieldInfo fiLayoutSuspendCount = null;
		[Browsable(false)]
		protected internal byte LayoutSuspendCountCore {
			get {
				if(fiLayoutSuspendCount == null) fiLayoutSuspendCount = typeof(Control).GetField("layoutSuspendCount", BindingFlags.Instance | BindingFlags.NonPublic);
				if(fiLayoutSuspendCount != null) return (byte)fiLayoutSuspendCount.GetValue(this);
				return 1;
			}
		}
		protected internal FormPainter FormPainter { get { return painter; } }
		Control IDXControl.Control { get { return this; } }
		bool shouldUpdateLookAndFeelOnResumeLayout = false;
		void IDXControl.OnLookAndFeelStyleChanged(object sender) {
			this.defaultAppearance = null;
			CheckRightToLeft();
			OnAppearance_Changed(sender);
			OnLookAndFeelChangedCore();
		}
		protected override void OnParentChanged(EventArgs e) {
			this.defaultAppearance = null;
			OnAppearance_Changed(null);
			OnFormBorderEffectChanged();
			base.OnParentChanged(e);
		}
		protected internal virtual bool IsRightToLeftCaption { get { return RightToLeft == System.Windows.Forms.RightToLeft.Yes; } }
		protected internal void SetLayoutDeferred() {
			const int STATE_LAYOUTDEFERRED = 512;
			MethodInfo mi = typeof(Control).GetMethod("SetState", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(int), typeof(bool) }, null);
			mi.Invoke(this, new object[] { STATE_LAYOUTDEFERRED, true });
		}
		void OnLookAndFeelChangedCore() {
			if(IsInitializing) {
				if(Visible && IsLayoutSuspendedCore) {
					SetLayoutDeferred();
				}
				shouldUpdateLookAndFeelOnResumeLayout = true;
				return;
			}
			this.shouldUpdateLookAndFeelOnResumeLayout = false;
			bool shouldUpdateSize = CheckUpdateSkinPainter();
			Size clientSize = ClientSize;
			OnMinimumClientSizeChanged();
			OnMaximumClientSizeChanged();
			FieldInfo fiBounds = typeof(Form).GetField("restoredWindowBounds", BindingFlags.NonPublic | BindingFlags.Instance);
			FieldInfo fiBoundsSpec = typeof(Form).GetField("restoredWindowBoundsSpecified", BindingFlags.NonPublic | BindingFlags.Instance);
			FieldInfo fiBounds2 = typeof(Form).GetField("restoreBounds", BindingFlags.NonPublic | BindingFlags.Instance);
			Rectangle restoredWinBounds = (Rectangle)fiBounds.GetValue(this);
			Rectangle restoreBounds = (Rectangle)fiBounds2.GetValue(this);
			BoundsSpecified restoredWinBoundsSpec = (BoundsSpecified)fiBoundsSpec.GetValue(this);
			int frmStateExWindowBoundsWidthIsClientSize, frmStateExWindowBoundsHeightIsClientSize;
			GetFormStateExWindowBoundsIsClientSize(out frmStateExWindowBoundsWidthIsClientSize, out frmStateExWindowBoundsHeightIsClientSize);
			int windowState = SaveFormStateWindowState();
			bool normalState = SaveControlStateNormalState();
			if(shouldUpdateSize)
				Size = SizeFromClientSize(clientSize);
			if((restoredWinBoundsSpec & BoundsSpecified.Width) != 0 && (restoredWinBoundsSpec & BoundsSpecified.Height) != 0) restoreBounds.Size = SizeFromClientSize(restoredWinBounds.Size);
			if(WindowState != FormWindowState.Normal && IsHandleCreated) {
				fiBounds.SetValue(this, restoredWinBounds);
				fiBounds2.SetValue(this, restoreBounds);
				SetFormStateExWindowBoundsIsClientSize(frmStateExWindowBoundsWidthIsClientSize, frmStateExWindowBoundsHeightIsClientSize);
			}
			if(IsMdiChild) {
				RestoreFormStateWindowState(windowState);
				RestoreControlStateNormalState(normalState);
			}
			if(IsHandleCreated)
				UpdateFormBorderEffect();
		}
		bool SaveControlStateNormalState() {
			FieldInfo state = typeof(Control).GetField("state", BindingFlags.NonPublic | BindingFlags.Instance);
			return ((int)state.GetValue(this) & 0x10000) != 0;
		}
		void RestoreControlStateNormalState(bool isNormal) {
			FieldInfo state = typeof(Control).GetField("state", BindingFlags.NonPublic | BindingFlags.Instance);
			int value = (int)state.GetValue(this);
			state.SetValue(this, isNormal ? (value | 0x10000) : (value & (~0x10000)));
		}
		int SaveFormStateWindowState() {
			FieldInfo formState = typeof(Form).GetField("formState", BindingFlags.NonPublic | BindingFlags.Instance);
			FieldInfo formStateWindowState = typeof(Form).GetField("FormStateWindowState", BindingFlags.NonPublic | BindingFlags.Static);
			BitVector32.Section formStateWindowStateSection = ((BitVector32.Section)formStateWindowState.GetValue(this));
			BitVector32 formStateData = (BitVector32)formState.GetValue(this);
			return formStateData[formStateWindowStateSection];
		}
		void RestoreFormStateWindowState(int state) {
			FieldInfo formState = typeof(Form).GetField("formState", BindingFlags.NonPublic | BindingFlags.Instance);
			FieldInfo formStateWindowState = typeof(Form).GetField("FormStateWindowState", BindingFlags.NonPublic | BindingFlags.Static);
			BitVector32.Section formStateWindowStateSection = ((BitVector32.Section)formStateWindowState.GetValue(this));
			BitVector32 formStateData = (BitVector32)formState.GetValue(this);
			formStateData[formStateWindowStateSection] = state;
			formState.SetValue(this, formStateData);
		}
		void GetFormStateExWindowBoundsIsClientSize(out int width, out int height) {
			FieldInfo formStateExInfo = typeof(Form).GetField("formStateEx", BindingFlags.NonPublic | BindingFlags.Instance);
			FieldInfo formStateExWindowBoundsWidthIsClientSizeInfo = typeof(Form).GetField("FormStateExWindowBoundsWidthIsClientSize", BindingFlags.NonPublic | BindingFlags.Static);
			FieldInfo formStateExWindowBoundsHeightIsClientSizeInfo = typeof(Form).GetField("FormStateExWindowBoundsHeightIsClientSize", BindingFlags.NonPublic | BindingFlags.Static);
			BitVector32.Section widthSection = (BitVector32.Section)formStateExWindowBoundsWidthIsClientSizeInfo.GetValue(this);
			BitVector32.Section heightSection = (BitVector32.Section)formStateExWindowBoundsHeightIsClientSizeInfo.GetValue(this);
			BitVector32 formState = (BitVector32)formStateExInfo.GetValue(this);
			width = formState[widthSection];
			height = formState[heightSection];
		}
		void SetFormStateExWindowBoundsIsClientSize(int width, int height) {
			FieldInfo formStateExInfo = typeof(Form).GetField("formStateEx", BindingFlags.NonPublic | BindingFlags.Instance);
			FieldInfo formStateExWindowBoundsWidthIsClientSizeInfo = typeof(Form).GetField("FormStateExWindowBoundsWidthIsClientSize", BindingFlags.NonPublic | BindingFlags.Static);
			FieldInfo formStateExWindowBoundsHeightIsClientSizeInfo = typeof(Form).GetField("FormStateExWindowBoundsHeightIsClientSize", BindingFlags.NonPublic | BindingFlags.Static);
			BitVector32.Section widthSection = (BitVector32.Section)formStateExWindowBoundsWidthIsClientSizeInfo.GetValue(this);
			BitVector32.Section heightSection = (BitVector32.Section)formStateExWindowBoundsHeightIsClientSizeInfo.GetValue(this);
			BitVector32 formState = (BitVector32)formStateExInfo.GetValue(this);
			formState[widthSection] = width;
			formState[heightSection] = height;
			formStateExInfo.SetValue(this, formState);
		}
		bool IsFormStateClientSizeSet() {
			FieldInfo fi1 = typeof(Form).GetField("FormStateSetClientSize", BindingFlags.NonPublic | BindingFlags.Static);
			FieldInfo fiFormState = typeof(Form).GetField("formState", BindingFlags.NonPublic | BindingFlags.Instance);
			BitVector32.Section bi1 = (BitVector32.Section)fi1.GetValue(this);
			BitVector32 state = (BitVector32)fiFormState.GetValue(this);
			return state[bi1] == 1;
		}
		protected override void OnSizeChanged(EventArgs e) {
			if(FormPainter != null && FormBorderStyle != FormBorderStyle.None && (ControlBox || !string.IsNullOrEmpty(Text)) && WindowState == FormWindowState.Normal) {
				PatchClientSize();
			}
			else if(ShouldPatchClientSize && WindowState != FormWindowState.Minimized)
				PatchClientSize();
			if(!IsRibbonForm) CalcFormBounds();
			base.OnSizeChanged(e);
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			OnMinimumClientSizeChanged();
			OnMaximumClientSizeChanged();
			UpdateFormBorderEffect();
			if(IsHandleCreated) 
				BeginInvoke(new MethodInvoker(() => {
					DevExpress.Utils.TouchHelpers.TouchKeyboardSupport.CheckEnableTouchSupport(this);
				}));
			CalcFormBounds();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string TextMdiTab {
			get { return textMdiTab; }
			set {
				if(textMdiTab == value) return;
				textMdiTab = value;
				OnTextMdiTabChanged();
			}
		}
		protected virtual void OnTextMdiTabChanged() {
			if(MdiParent == null) return;
			OnTextChanged(EventArgs.Empty);
		}
		protected Size ConstrainMinimumClientSize(Size value) {
			value.Width = Math.Max(0, value.Width);
			value.Height = Math.Max(0, value.Height);
			return value;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Size MinimumClientSize {
			get { return minimumClientSize; }
			set {
				value = ConstrainMinimumClientSize(value);
				if(MinimumClientSize == value) return;
				minimumClientSize = value;
				OnMinimumClientSizeChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Size MaximumClientSize {
			get { return maximumClientSize; }
			set {
				if(MaximumClientSize == value) return;
				maximumClientSize = value;
				OnMaximumClientSizeChanged();
			}
		}
		protected virtual void OnMinimumClientSizeChanged() {
			if(IsInitializing) return;
			MinimumSize = GetConstrainSize(MinimumClientSize);
		}
		protected virtual void OnMaximumClientSizeChanged() {
			if(IsInitializing) return;
			MaximumSize = GetConstrainSize(MaximumClientSize);
		}
		protected virtual Size GetConstrainSize(Size clientSize) {
			if(clientSize == Size.Empty) return Size.Empty;
			return SizeFromClientSize(clientSize);
		}
		protected virtual Size ClientSizeFromSize(Size formSize) {
			if(formSize == Size.Empty)
				return Size.Empty;
			Size sz = SizeFromClientSize(Size.Empty);
			return new Size(formSize.Width - sz.Width, formSize.Height - sz.Height);
		}
		[Localizable(true)]
		public override Size MinimumSize {
			get {
				if(inScaleControl && minimumSize.HasValue)
					return minimumSize.Value;
				return base.MinimumSize;
			}
			set {
				minimumSize = value;
				if(IsInitializing || IsApplyingLocalizableResourcesInDesignTime) {
					return;
				}
				Size maxSize = MaximumSize;
				base.MinimumSize = value;
				if(maxSize != MaximumSize)
					MaximumClientSize = ClientSizeFromSize(MaximumSize);
			}
		}
		[Localizable(true)]
		public override Size MaximumSize {
			get {
				if(inScaleControl && maximumSize.HasValue)
					return maximumSize.Value;
				return base.MaximumSize;
			}
			set {
				maximumSize = value;
				if(IsInitializing || IsApplyingLocalizableResourcesInDesignTime) {
					return;
				}
				Size minSize = MinimumSize;
				base.MaximumSize = value;
				if(MinimumSize != minSize)
					MinimumClientSize = ClientSizeFromSize(MinimumSize);
			}
		}
		protected override void OnMinimumSizeChanged(EventArgs e) {
			base.OnMinimumSizeChanged(e);
			MinimumClientSize = ClientSizeFromSize(MinimumSize);
		}
		protected override void OnMaximumSizeChanged(EventArgs e) {
			base.OnMaximumSizeChanged(e);
			MaximumClientSize = ClientSizeFromSize(MaximumSize);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected internal virtual bool SupportAdvancedTitlePainting { get { return true; } }
		protected bool CheckUpdateSkinPainter() {
			bool needReset = UpdateSkinPainter();
			if(IsHandleCreated) {
				if(this.painter != null) this.painter.ResetDefaultAppearance();
				if(needReset) {
					if(FormPainter == null || !FormPainter.IsAllowNCDraw)
						EnableTheme();
					else
						DisableTheme();
					if(!DevExpress.Utils.WXPaint.WXPPainter.Default.ThemesEnabled) {
						RecreateHandle();
					}
					else {
						DevExpress.Skins.XtraForm.FormPainter.ForceFrameChanged(this);
						ForceRefresh();
					}
					OnSkinPainterChanged();
				}
				if(FormPainter == null) ForceRefresh();
			}
			return needReset;
		}
		protected virtual void ForceRefresh() {
			Refresh();
		}
		protected virtual void OnSkinPainterChanged() {
		}
		void IDXControl.OnAppearanceChanged(object sender) {
			OnAppearance_Changed(sender);
		}
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		int inWmWindowPosChanged = 0;
		protected void UpdateClientBackgroundBounds(int msg) {
			if(msg == MSG.WM_NCPAINT && WindowState != FormWindowState.Normal) {
				CalcFormBounds();
			}
		}
		protected virtual int ClientBackgroundTop { get { return 0; } }
		protected internal virtual bool IsRibbonForm { get { return false; } }
		Rectangle GetFormXAddedRectangle() {
			if(formBounds.Width < prevFormBounds.Width) return Rectangle.Empty;
			int x = prevFormBounds.Width;
			int y = ClientBackgroundTop;
			int width = Math.Abs(formBounds.Width - prevFormBounds.Width);
			int height = Math.Abs(formBounds.Height);
			return new Rectangle(x, y, width, height);
		}
		Rectangle GetFormYAddedRectangle() {
			if(formBounds.Height < prevFormBounds.Height) return Rectangle.Empty;
			int x = 0;
			int y = prevFormBounds.Height;
			int width = Math.Abs(formBounds.Width);
			int height = Math.Abs(formBounds.Height - prevFormBounds.Height);
			return new Rectangle(x, y, width, height);
		}
		protected virtual void DrawClientBackgroundCore(GraphicsCache cache) {
			SkinElement elem = CommonSkins.GetSkin(FormPainter.Provider)[CommonSkins.SkinForm];
			SkinElementInfo info = new SkinElementInfo(elem, GetFormXAddedRectangle());
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
			info = new SkinElementInfo(elem, GetFormYAddedRectangle());
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
		}
		protected bool IsRegionPainted { get { return FormPainter != null && FormPainter.RegionPainted; } }
		protected void CalcClippedRegion(Graphics g) {
			if(IsRegionPainted || Region == null) return;
			RectangleF region = Region.GetBounds(g);
			this.formBounds.Width = Math.Min(formBounds.Width, (int)(region.X + region.Width));
			this.formBounds.Height = Math.Min(formBounds.Height, (int)(region.Y + region.Height));
		}
		protected virtual void DrawClientBackground(Message msg) {
			boundsUpdated = false;
			if(FormPainter == null || formBounds == prevFormBounds) return;
			IntPtr dc = FormPainter.GetDC(msg);
			using(Graphics g = Graphics.FromHdc(dc)) {
				using(GraphicsCache cache = new GraphicsCache(g)) {
					DrawClientBackgroundCore(cache);
				}
				CalcClippedRegion(g);
			}
			NativeMethods.ReleaseDC(Handle, dc);
		}
		protected Rectangle prevFormBounds = Rectangle.Empty;
		protected Rectangle formBounds = Rectangle.Empty;
		protected bool boundsUpdated = false;
		protected bool lockDrawingClientBackground = false;
		protected bool isFormPainted = false;
		protected bool CanDrawClientBackground { get { return (boundsUpdated || !IsRegionPainted) && !isFormPainted; } }
		protected virtual bool ShouldDrawClientBackground(int msg) {
			if(msg == MSG.WM_NCPAINT) {
				lockDrawingClientBackground = WindowState == FormWindowState.Maximized;
			}
			if(!CanDrawClientBackground || IsMdiChild || lockDrawingClientBackground) return false;
			if(msg == MSG.WM_WINDOWPOSCHANGED && WindowState == FormWindowState.Maximized) {
				lockDrawingClientBackground = true;
				return true;
			}
			return msg == MSG.WM_NCPAINT && WindowState != FormWindowState.Maximized;
		}
		Point minimizedFormLocation = new Point(-32000, -32000);
		protected internal bool IsMinimizedState(Rectangle bounds) {
			return WindowState == FormWindowState.Minimized && bounds.Location == minimizedFormLocation;
		}
		protected internal void CalcFormBounds() {
			if(IsMdiChild || !IsHandleCreated) return;
			NativeMethods.RECT correctFormBounds = new NativeMethods.RECT();
			NativeMethods.GetWindowRect(this.Handle, ref correctFormBounds);
			Rectangle currentBounds = correctFormBounds.ToRectangle();
			if(IsMinimizedState(currentBounds)) currentBounds = Rectangle.Empty;
			if(formBounds == currentBounds && (boundsUpdated || !IsRegionPainted))
				return;
			this.isFormPainted = false;
			prevFormBounds = formBounds;
			formBounds = currentBounds;
			if(prevFormBounds != formBounds) boundsUpdated = true;
		}
		protected override void WndProc(ref Message msg) {
			UpdateClientBackgroundBounds(msg.Msg);
			if(ShouldDrawClientBackground(msg.Msg)) {
				DrawClientBackground(msg);
				if(FormPainter != null) FormPainter.RegionPainted = true;
			}
			if(msg.Msg == MSG.WM_NCPAINT || msg.Msg == MSG.WM_PAINT || msg.Msg == MSG.WM_ACTIVATE) this.isFormPainted = true;
			if((msg.Msg == MSG.WM_ACTIVATE || msg.Msg == MSG.WM_NCACTIVATE) && SuppressDeactivation) {
				msg.WParam = new IntPtr(1);
			}
			if(msg.Msg == MSG.WM_MOUSEHWHEEL) {
				DXMouseEventArgs me = ControlHelper.GenerateMouseHWheel(ref msg, this);
				OnMouseWheel(me);
				if(me.Handled) {
					msg.Result = new IntPtr(1);
					return;
				}
			}
			if(msg.Msg == MSG.WM_DWMCOMPOSITIONCHANGED)
				CheckUpdateSkinPainter();
			if(painter != null) {
				bool res = painter.DoWndProc(ref msg);
				if(res) {
					if(ShouldUpdateFrame(msg)) FormPainter.ForceFrameChanged(this);
					return;
				}
				if(msg.Msg == MSG.WM_MOVE) {
					if(FormPainter != null && MdiParent == null && WindowState != FormWindowState.Maximized)
						ShouldPatchClientSize = true;
				}
				if(msg.Msg == MSG.WM_WINDOWPOSCHANGED) {
					if(FormPainter != null && MdiParent == null && WindowState != FormWindowState.Maximized)
						ShouldPatchClientSize = true;
					this.inWmWindowPosChanged++;
				}
				if(NeedActivateForm && msg.Msg == MSG.WM_NCPAINT) ForceActivateWindow(msg);
				base.WndProc(ref msg);
				if(msg.Msg == MSG.WM_WINDOWPOSCHANGED) {
					this.inWmWindowPosChanged--;
				}
				Utils.CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref msg, this);
				return;
			}
			base.WndProc(ref msg);
			ShouldPatchClientSize = false;
			Utils.CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref msg, this);
		}
		protected bool ShouldUpdateFrame(Message msg) {
			return msg.Msg == MSG.WM_SIZE && IsRibbonForm && WindowState == FormWindowState.Maximized;
		}
		protected virtual bool NeedActivateForm {
			get {
				return false;
			}
		}
		protected virtual void ForceActivateWindow(Message msg) {
			ActivateWindow(msg.HWnd, MSG.WM_NCACTIVATE, msg.WParam, msg.LParam);
		}
		[SecuritySafeCritical]
		protected void ActivateWindow(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam) {
			DefWindowProc(hWnd, msg, wParam, lParam);
		}
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		protected static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
		void DoBase(ref Message msg) {
			base.WndProc(ref msg);
		}
		protected bool IsCustomPainter { get { return painter != null; } }
		bool ShouldPatchClientSize { get; set; }
		Size SavedClientSize { get; set; }
		protected override void OnStyleChanged(EventArgs e) {
			if(FormPainter != null)
				ShouldPatchClientSize = true;
			SavedClientSize = ClientSize;
			try {
				if(UpdateSkinPainter() && this.clientSizeSet) {
					ClientSize = ClientSize;
					this.clientSizeSet = false;
				}
			}
			finally {
				ShouldPatchClientSize = false;
			}
			base.OnStyleChanged(e);
		}
		bool clientSizeSet = false;
		protected override void SetClientSizeCore(int x, int y) {
			this.clientSizeSet = false;
			UpdateSkinPainter();
			FieldInfo fiWidth = typeof(Control).GetField("clientWidth", BindingFlags.Instance | BindingFlags.NonPublic);
			FieldInfo fiHeight = typeof(Control).GetField("clientHeight", BindingFlags.Instance | BindingFlags.NonPublic);
			FieldInfo fi1 = typeof(Form).GetField("FormStateSetClientSize", BindingFlags.NonPublic | BindingFlags.Static),
				fiFormState = typeof(Form).GetField("formState", BindingFlags.NonPublic | BindingFlags.Instance);
			if(IsCustomPainter && fiWidth != null && fiHeight != null && fiFormState != null && fi1 != null) {
				this.clientSizeSet = true;
				this.Size = SizeFromClientSize(new Size(x, y));
				fiWidth.SetValue(this, x);
				fiHeight.SetValue(this, y);
				BitVector32.Section bi1 = (BitVector32.Section)fi1.GetValue(this);
				BitVector32 state = (BitVector32)fiFormState.GetValue(this);
				state[bi1] = 1;
				fiFormState.SetValue(this, state);
				this.OnClientSizeChanged(EventArgs.Empty);
				state[bi1] = 0;
				fiFormState.SetValue(this, state);
			}
			else {
				base.SetClientSizeCore(x, y);
			}
		}
		protected override void OnMdiChildActivate(EventArgs e) {
			base.OnMdiChildActivate(e);
			if(IsCustomPainter)
				BeginInvoke(new MethodInvoker(DrawMenuBarCore));
			if(prevMdiChild != null)
				prevMdiChild.TextChanged -= new EventHandler(OnMdiChildTextChanged);
			if(ActiveMdiChild != null)
				ActiveMdiChild.TextChanged += new EventHandler(OnMdiChildTextChanged);
			prevMdiChild = ActiveMdiChild;
			if(IsCustomPainter)
				OnTextChanged(e);
			if(ActiveMdiChild == null) {
				FormPainter.UpdateMdiClient(GetMdiClient());
			}
		}
		void DrawMenuBarCore() {
			NativeMethods.DrawMenuBar(Handle);
		}
		protected virtual void OnMdiChildTextChanged(object sender, EventArgs e) {
			OnTextChanged(e);
			FormPainter.InvalidateNC(this);
		}
		protected override Size SizeFromClientSize(Size clientSize) {
			if((!ControlBox && !AllowSkinForEmptyText && (Text == "" || Text == null)) || painter == null)
				return base.SizeFromClientSize(clientSize);
			return painter.CalcSizeFromClientSize(clientSize);
		}
		protected override Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified) {
			Rectangle rect = base.GetScaledBounds(bounds, factor, specified);
			if(!IsCustomPainter)
				return rect;
			Size sz = SizeFromClientSize(Size.Empty);
			if(!GetStyle(ControlStyles.FixedWidth) && ((specified & BoundsSpecified.Width) != BoundsSpecified.None)) {
				int clientWidth = bounds.Width - sz.Width;
				rect.Width = ((int)Math.Round((double)(clientWidth * factor.Width))) + sz.Width;
			}
			if(!GetStyle(ControlStyles.FixedHeight) && ((specified & BoundsSpecified.Height) != BoundsSpecified.None)) {
				int clientHeight = bounds.Height - sz.Height;
				rect.Height = ((int)Math.Round((double)(clientHeight * factor.Height))) + sz.Height;
			}
			return rect;
		}
		bool inScaleControl = false;
		protected override void ScaleControl(SizeF factor, BoundsSpecified specified) {
			this.inScaleControl = true;
			if(!IsCustomPainter) {
				base.ScaleControl(factor, specified);
				this.inScaleControl = false;
				return;
			}
			Size minSize = MinimumSize;
			Size maxSize = MaximumSize;
			Size sz = Size.Empty;
			try {
				ShouldPatchClientSize = true;
				Size = SizeFromClientSize(ClientSize);
				ShouldPatchClientSize = true;
				sz = SizeFromClientSize(Size.Empty);
				base.ScaleControl(factor, specified);
			}
			finally {
				ShouldPatchClientSize = false;
			}
			if(minSize != Size.Empty) {
				minSize -= sz;
				minSize = new Size((int)Math.Round(minSize.Width * factor.Width), (int)Math.Round(minSize.Height * factor.Height)) + sz;
			}
			if(maxSize != Size.Empty) {
				maxSize -= sz;
				maxSize = new Size((int)Math.Round(maxSize.Width * factor.Width), (int)Math.Round(maxSize.Height * factor.Height)) + sz;
			}
			MinimumSize = minSize;
			MaximumSize = maxSize;
			this.inScaleControl = false;
		}
		protected virtual Size PatchFormSizeInRestoreWindowBoundsIfNecessary(int width, int height) {
			if(WindowState == FormWindowState.Normal && inWmWindowPosChanged > 0 && IsCustomPainter) {
				try {
					FieldInfo fiRestoredBoundsSpecified = typeof(Form).GetField("restoredWindowBoundsSpecified", BindingFlags.NonPublic | BindingFlags.Instance);
					BoundsSpecified restoredSpecified = (BoundsSpecified)fiRestoredBoundsSpecified.GetValue(this);
					if((restoredSpecified & BoundsSpecified.Size) != BoundsSpecified.None) {
						FieldInfo fi1 = typeof(Form).GetField("FormStateExWindowBoundsWidthIsClientSize", BindingFlags.NonPublic | BindingFlags.Static),
						fiFormState = typeof(Form).GetField("formStateEx", BindingFlags.NonPublic | BindingFlags.Instance),
						fiBounds = typeof(Form).GetField("restoredWindowBounds", BindingFlags.NonPublic | BindingFlags.Instance);
						if(fi1 != null && fiFormState != null && fiBounds != null) {
							Rectangle restoredWindowBounds = (Rectangle)fiBounds.GetValue(this);
							BitVector32.Section bi1 = (BitVector32.Section)fi1.GetValue(this);
							BitVector32 state = (BitVector32)fiFormState.GetValue(this);
							if(state[bi1] == 1) {
								width = restoredWindowBounds.Width + painter.Margins.Left + painter.Margins.Right;
								height = restoredWindowBounds.Height + painter.Margins.Top + painter.Margins.Bottom;
							}
						}
					}
				}
				catch {
				}
			}
			return new Size(width, height);
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			Size size = PatchFormSizeInRestoreWindowBoundsIfNecessary(width, height);
			size = CalcPreferredSizeCore(size);
			base.SetBoundsCore(x, y, size.Width, size.Height, specified);
		}
		protected virtual Size CalcPreferredSizeCore(Size size) {
			return size;
		}
		internal bool creatingHandle = false;
		protected override void CreateHandle() {
			bool shouldPatchSize = !this.creatingHandle;
			this.creatingHandle = true;
			if(shouldPatchSize)
				if(WindowState != FormWindowState.Minimized && !DesignMode)
					Size = SizeFromClientSize(ClientSize);
			if(!IsHandleCreated)
				base.CreateHandle();
			this.creatingHandle = false;
		}
		internal bool IsMaximizedBoundsSet {
			get { return !MaximumSize.IsEmpty || !MaximizedBounds.IsEmpty; }
		}
		int lockRedraw = 0;
		bool isWindowActive = false;
		XtraForm suspendRedrawMdiParent = null;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsSuspendRedraw { get { return lockRedraw != 0; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SuspendRedraw() {
			if(lockRedraw++ == 0) this.isWindowActive = painter == null ? false : painter.IsWindowActive;
			suspendRedrawMdiParent = MdiParent as XtraForm;
			if(suspendRedrawMdiParent != null) suspendRedrawMdiParent.SuspendRedraw();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ResumeRedraw() {
			if(lockRedraw < 1) return;
			if(--lockRedraw == 0) {
				if(painter != null && painter.IsWindowActive != this.isWindowActive) painter.InvalidateNC();
			}
			if(suspendRedrawMdiParent != null) {
				suspendRedrawMdiParent.ResumeRedraw();
				if(suspendRedrawMdiParent.lockRedraw == 0)
					suspendRedrawMdiParent = null;
			}
		}
		protected ControlHelper Helper {
			get { return helper; }
			set {
				if(Helper == value)
					return;
				helper = value;
				OnHelperChanged();
			}
		}
		private void OnHelperChanged() {
			OnAppearance_Changed(Helper);
		}
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		void ResetAppearance() { Appearance.Reset(); }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("XtraFormAppearance"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Appearance { get { return Helper == null ? AppearanceObject.Dummy : Helper.Appearance; } }
		bool ShouldSerializeLookAndFeel() { return LookAndFeel.ShouldSerialize(); }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("XtraFormLookAndFeel"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FormUserLookAndFeel LookAndFeel { get { return Helper == null ? null : (FormUserLookAndFeel)Helper.LookAndFeel; } }
		protected void OnAppearance_Changed(object sender) {
			this.defaultAppearance = null;
			if(IsHandleCreated) {
				OnBackColorChanged(EventArgs.Empty);
				Invalidate();
			}
			if(!base.Font.Equals(GetFont())) {
				this.useBaseFont = true;
				base.Font = GetFont();
				this.useBaseFont = false;
			}
		}
		bool delayedFontChanged = false;
		protected override void OnFontChanged(EventArgs e) {
			if(this.isInitializing > 0) {
				delayedFontChanged = true;
			}
			delayedFontChanged = false;
			base.OnFontChanged(e);
		}
		[DefaultValue(false), 
#if !SL
	DevExpressUtilsLocalizedDescription("XtraFormAllowMdiBar")
#else
	Description("")
#endif
]
		public virtual bool AllowMdiBar {
			get { return allowMdiBar; }
			set {
				if(allowMdiBar == value) return;
				allowMdiBar = value;
				if(FormPainter != null && IsHandleCreated) {
					FormPainter.ForceFrameChanged();
					Refresh();
				}
			}
		}
		[DefaultValue(true), Browsable(false), 
#if !SL
	DevExpressUtilsLocalizedDescription("XtraFormCloseBox")
#else
	Description("")
#endif
]
		public virtual bool CloseBox {
			get { return closeBox; }
			set {
				if(CloseBox == value) return;
				closeBox = value;
				if(FormPainter != null && IsHandleCreated) {
					FormPainter.ForceFrameChanged();
					Refresh();
				}
			}
		}
		[DefaultValue(""), 
#if !SL
	DevExpressUtilsLocalizedDescription("XtraFormHtmlImages"),
#endif
 RefreshProperties(RefreshProperties.All)]
		public string HtmlText {
			get { return htmlText; }
			set {
				if(value == null) value = string.Empty;
				if(HtmlText == value) return;
				htmlText = value;
				this.lockTextChanged++;
				try {
					Text = StringPainter.Default.RemoveFormat(HtmlText);
				}
				finally {
					this.lockTextChanged--;
				}
				OnTextChanged(EventArgs.Empty);
			}
		}
		bool ShouldSerializeText() { return string.IsNullOrEmpty(HtmlText) && !string.IsNullOrEmpty(Text); }
		[RefreshProperties(RefreshProperties.All)]
		public override string Text {
			get {
				return base.Text;
			}
			set {
				if(this.lockTextChanged == 0 && !IsInitializing) this.htmlText = string.Empty;
				base.Text = value;
			}
		}
		int lockTextChanged = 0;
		protected override void OnTextChanged(EventArgs e) {
			if(lockTextChanged != 0) return;
			base.OnTextChanged(e);
		}
		[DefaultValue(null), 
#if !SL
	DevExpressUtilsLocalizedDescription("XtraFormHtmlImages")
#else
	Description("")
#endif
]
		public ImageCollection HtmlImages {
			get { return htmlImages; }
			set {
				if(htmlImages == value) return;
				if(htmlImages != null) htmlImages.Changed -= new EventHandler(OnHtmlImagesChanged);
				htmlImages = value;
				if(htmlImages != null) htmlImages.Changed += new EventHandler(OnHtmlImagesChanged);
			}
		}
		void OnHtmlImagesChanged(object sender, EventArgs e) {
		}
		[Browsable(false), DefaultValue(true), 
#if !SL
	DevExpressUtilsLocalizedDescription("XtraFormAllowFormSkin"),
#endif
 Category("Appearance")]
		public virtual bool AllowFormSkin {
			get { return allowFormSkin; }
			set {
				if(AllowFormSkin == value) return;
				allowFormSkin = value;
				UpdateSkinPainter();
				if(IsHandleCreated) RecreateHandle();
			}
		}
		protected virtual object GetSkinPainterType() {
			if(!GetAllowSkin()) return null;
			return typeof(FormPainter);
		}
		object currentPainterType = null;
		Size? minimumSize = null, maximumSize = null;
		protected bool UpdateSkinPainter() { return UpdateSkinPainter(false); }
		protected bool UpdateSkinPainter(bool fromHandleCreated) {
			bool needReset = false;
			object painterType = GetSkinPainterType();
			if(currentPainterType == painterType || object.Equals(currentPainterType, painterType)) {
				if(painter != null) {
					this.painter.ResetDefaultAppearance();
					if(!fromHandleCreated) {
						this.painter.ResetRegionRect();
						this.painter.ForceFrameChanged();
					}
				}
				return false;
			}
			this.currentPainterType = IsInitializing ? typeof(bool) : painterType;
			if(painter != null) {
				painter.Dispose();
				painter = null;
				needReset = true;
			}
			if(painterType != null) {
				if(!IsInitializing) {
					this.painter = CreateFormBorderPainter();
					this.painter.BaseWndProc += new FormPainter.WndProcMethod(DoBase);
					needReset = true;
				}
				else {
					shouldUpdateLookAndFeelOnResumeLayout = true;
				}
			}
			return needReset;
		}
		void PatchClientSize() {
			SavedClientSize = ClientSizeFromSize(Size);
			FieldInfo fiWidth = typeof(Control).GetField("clientWidth", BindingFlags.Instance | BindingFlags.NonPublic);
			FieldInfo fiHeight = typeof(Control).GetField("clientHeight", BindingFlags.Instance | BindingFlags.NonPublic);
			fiWidth.SetValue(this, SavedClientSize.Width);
			fiHeight.SetValue(this, SavedClientSize.Height);
		}
		protected override void OnClientSizeChanged(EventArgs e) {
			if(ShouldPatchClientSize)
				PatchClientSize();
			base.OnClientSizeChanged(e);
		}
		protected virtual FormPainter CreateFormBorderPainter() {
			return new FormPainter(this, LookAndFeel);
		}
		protected internal virtual bool AllowSkinForEmptyText { get { return false; } }
		protected virtual bool GetAllowMdiFormSkins() {
			return SkinManager.AllowMdiFormSkins;
		}
		protected virtual bool GetAllowSkin() {
			if(Disposing || IsDisposed)
				return false;
			if(DesignMode && !UserLookAndFeelDefault.EnableDesignTimeFormSkin)
				return false;
			if(MdiParent != null && !GetAllowMdiFormSkins())
				return false;
			if(!AllowFormSkin)
				return false;
			if(!SkinManager.AllowFormSkins) {
				if(!DesignMode || (DesignMode && !UserLookAndFeelDefault.EnableDesignTimeFormSkin))
					return false;
			}
			if(FormBorderStyle == FormBorderStyle.None)
				return false;
			if(!ControlBox && !AllowSkinForEmptyText && (Text == "" || Text == null)) {
				if(IsInitializing && LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
					this.shouldUpdateLookAndFeelOnResumeLayout = true;
				}
				else
					return false;
			}
			return LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin;
		}
		[System.Runtime.InteropServices.DllImport("USER32.dll")]
		static extern bool DestroyMenu(IntPtr menu);
		[DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
		static extern IntPtr GetSystemMenuCore(IntPtr hWnd, bool bRevert);
		[System.Security.SecuritySafeCritical]
		static internal IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert) {
			return GetSystemMenuCore(hWnd, bRevert);
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			UpdateSkinPainter(true);
			BeginInvoke(new MethodInvoker(UpdateTheme));
			GetSystemMenu(Handle, true);
			UpdateWindowThemeCore();
		}
		protected override void SetVisibleCore(bool value) {
			if(value && ShowingStrategy.AllowInitialize) {
				ShowingStrategy.Initialize();
			}
			base.SetVisibleCore(value);
		}
		protected internal bool DesignModeCore { get { return DesignMode; } }
		[SecuritySafeCritical]
		static int SetWindowTheme(IntPtr hWnd, String pszSubAppName, String pszSubIdList) { return SetWindowThemeCore(hWnd, pszSubAppName, pszSubIdList); }
		[DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode, EntryPoint = "SetWindowTheme")]
		static extern int SetWindowThemeCore(IntPtr hWnd, String pszSubAppName, String pszSubIdList);
		protected virtual void UpdateWindowThemeCore() {
			if(NativeVista.IsVista && !DesignMode && FormPainter != null) {
				SetWindowTheme(Handle, "", "");
				themeEnabled = false;
			}
		}
		internal virtual MdiClient GetMdiClient() {
			foreach(Control ctrl in Controls) {
				if(ctrl is MdiClient) return ctrl as MdiClient;
			}
			return null;
		}
		bool themeEnabled = true;
		void EnableTheme() {
			if(themeEnabled) return;
			DevExpress.Utils.WXPaint.WXPPainter.Default.ResetWindowTheme(this);
			Region = null;
			this.themeEnabled = true;
		}
		void DisableTheme() {
			if(!ControlBox && !themeEnabled) return;
			DevExpress.Utils.WXPaint.WXPPainter.Default.DisableWindowTheme(this);
			this.themeEnabled = false;
			UpdateWindowThemeCore();
		}
		void UpdateTheme() {
			if(FormPainter != null && FormPainter.IsAllowNCDraw) {
				DisableTheme();
			}
			else {
				EnableTheme();
			}
		}
		protected override CreateParams CreateParams { get { return GetCreateParams(base.CreateParams); } }
		protected virtual CreateParams GetCreateParams(CreateParams par) {
			if(IsDisposed || Disposing) return par;
			if(GetAllowSkin()) {
			}
			else {
				UpdateSkinPainter();
			}
			if(this.creatingHandle && IsFormStateClientSizeSet() && IsCustomPainter) {
				par.Width = Width;
				par.Height = Height;
			}
			return par;
		}
		public override void ResetBackColor() { BackColor = Color.Empty; }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("XtraFormBackColor"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor {
			get { return GetColor(Appearance.BackColor, DefaultAppearance.BackColor); }
			set {
				if(GetDesigner() != null && GetDesigner().Loading) return;
				Appearance.BackColor = value;
			}
		}
		protected Image GetSkinBackgroundImage() {
			if(GetSkin() == null) return null;
			if(GetSkin().Image == null) return null;
			return GetSkin().Image.Image;
		}
		protected SkinImageStretch GetSkinBackgroundImageStretch() {
			if(GetSkin() == null) return SkinImageStretch.NoResize;
			if(GetSkin().Image == null) return SkinImageStretch.NoResize;
			return GetSkin().Image.Stretch;
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("XtraFormBackgroundImage"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Image BackgroundImage {
			get { return base.BackgroundImage == null ? GetSkinBackgroundImage() : base.BackgroundImage; }
			set {
				if(value != null && value == GetSkinBackgroundImage()) value = null;
				base.BackgroundImage = value;
			}
		}
		protected bool ShouldSerializeBackgroundImageStore() {
			return base.BackgroundImage != null;
		}
		protected bool ShouldSerializeBackgroundImageLayoutStore() {
			return base.BackgroundImage != null;
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public Image BackgroundImageStore {
			get { return base.BackgroundImage; }
			set { base.BackgroundImage = value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public ImageLayout BackgroundImageLayoutStore {
			get { return base.BackgroundImageLayout; }
			set { base.BackgroundImageLayout = value; }
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("XtraFormBackgroundImageLayout"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageLayout BackgroundImageLayout {
			get {
				if(base.BackgroundImage == null && !IsInitializing) {
					if(GetSkinBackgroundImageStretch() == SkinImageStretch.NoResize) return ImageLayout.None;
					if(GetSkinBackgroundImageStretch() == SkinImageStretch.Stretch) return ImageLayout.Stretch;
					if(GetSkinBackgroundImageStretch() == SkinImageStretch.Tile) return ImageLayout.Tile;
				}
				return base.BackgroundImageLayout;
			}
			set {
				base.BackgroundImageLayout = value;
			}
		}
		IDesignerHost GetDesigner() {
			if(Site == null) return null;
			return Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
		}
		public override void ResetForeColor() { ForeColor = Color.Empty; }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("XtraFormForeColor"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color ForeColor {
			get { return LookAndFeelHelper.CheckTransparentForeColor(LookAndFeel, GetColor(Appearance.ForeColor, DefaultAppearance.ForeColor), null); }
			set { Appearance.ForeColor = value; }
		}
		bool useBaseFont = false;
#if !SL
	[DevExpressUtilsLocalizedDescription("XtraFormFont")]
#endif
		public override Font Font {
			get {
				if(useBaseFont)
					return base.Font;
				return GetFont();
			}
			set { Appearance.Font = value; }
		}
		Font GetFont() {
			AppearanceObject app = Appearance.GetAppearanceByOption(AppearanceObject.optUseFont);
			if(app.Options.UseFont || DefaultAppearance.Font == null) return app.Font;
			return DefaultAppearance.Font;
		}
		internal Font GetBaseFont() { return base.Font; }
		Color GetColor(Color color, Color defColor) {
			if(color == Color.Empty) return defColor;
			return color;
		}
		AppearanceDefault defaultAppearance = null;
		protected AppearanceDefault DefaultAppearance {
			get {
				AppearanceDefault d = CreateDefaultAppearance();
				if(defaultAppearance == null)
					defaultAppearance = CreateDefaultAppearance();
				if(d.BackColor != defaultAppearance.BackColor) {
				}
				return defaultAppearance;
			}
		}
		protected virtual AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault res = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control);
			if(LookAndFeel != null && LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				SkinElement form = GetSkin();
				form.Apply(res, LookAndFeel);
				CheckFormAppearanceAsMdiTabPage(res);
				LookAndFeelHelper.CheckColors(LookAndFeel, res, this);
			}
			return res;
		}
		protected void CheckFormAppearanceAsMdiTabPage(AppearanceDefault defaultAppearance) {
			SkinElement tabPane;
			if(IsMdiTab(out tabPane) && tabPane != null) {
				tabPane.ApplyForeColorAndFont(defaultAppearance);
				Color solidColor1 = tabPane.Color.GetSolidImageCenterColor();
				if(solidColor1 != Color.Empty) {
					defaultAppearance.BackColor = solidColor1;
					Color solidColor2 = tabPane.Color.GetSolidImageCenterColor2();
					if(solidColor2 != Color.Empty) {
						defaultAppearance.BackColor2 = solidColor2;
						defaultAppearance.GradientMode = tabPane.Color.SolidImageCenterGradientMode;
					}
				}
			}
		}
		protected bool IsMdiTab(out SkinElement tabPane) {
			tabPane = null;
			MdiClient mdiClient = GetMdiClient(MdiParent);
			if(mdiClient != null) {
				object subclasser = GetMdiClientSubclasser(mdiClient);
				if(subclasser != null) {
					if(subclasser.GetType().Name.EndsWith("XtraMdiClientSubclasser"))
						tabPane = TabSkins.GetSkin(LookAndFeel)[TabSkins.SkinTabPane];
					if(subclasser.GetType().Name.EndsWith("DocumentManagerMdiClientSubclasser"))
						tabPane = DockingSkins.GetSkin(LookAndFeel)[DockingSkins.SkinDocumentGroupTabPane] ??
							TabSkins.GetSkin(LookAndFeel)[TabSkins.SkinTabPane];
					return true;
				}
			}
			return false;
		}
		static MdiClient GetMdiClient(Form mdiParent) {
			return DevExpress.Utils.Mdi.MdiClientSubclasserService.GetMdiClient(mdiParent);
		}
		static object GetMdiClientSubclasser(MdiClient mdiClient) {
			return DevExpress.Utils.Mdi.MdiClientSubclasserService.FromMdiClient(mdiClient);
		}
		static bool IsBarsAssembly(Assembly asm) {
			return string.Equals(asm.GetName().Name, AssemblyInfo.SRAssemblyBars);
		}
		SkinElement GetSkin() {
			if(LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin) return null;
			return CommonSkins.GetSkin(LookAndFeel)[CommonSkins.SkinForm];
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(FormPainter != null && FormPainter.RequirePaint) {
				FormPainter.Draw(e);
				return;
			}
			base.OnPaint(e);
		}
		protected override void OnPaintBackground(PaintEventArgs e) {
			if(FormPainter != null && FormPainter.RequirePaintBackground) {
				FormPainter.DrawBackground(e);
				return;
			}
			base.OnPaintBackground(e);
		}
		bool IGlassForm.IsGlassForm { get { return CheckGlassForm(); } }
		protected virtual bool CheckGlassForm() { return !(FormPainter is FormPainter); }
		internal CreateParams GetCreateParams() {
			return CreateParams;
		}
		#region ICustomDrawNonClientArea Members
		bool ICustomDrawNonClientArea.IsCustomDrawNonClientArea {
			get { return CheckCustomDrawNonClientArea(); }
		}
		protected virtual bool CheckCustomDrawNonClientArea() {
			return SkinManager.AllowFormSkins && !CheckGlassForm();
		}
		#endregion
		UserLookAndFeel ISupportLookAndFeel.LookAndFeel {
			get { return LookAndFeel; }
		}
	}
	public enum FormShowMode { Default, AfterInitialization }
	public abstract class FormShowStrategyBase {
		XtraForm form;
		public FormShowStrategyBase(XtraForm form) {
			this.form = form;
		}
		public XtraForm Form { get { return form; } }
		public static FormShowStrategyBase Create(XtraForm form, FormShowMode showMode) {
			switch(showMode) {
				case FormShowMode.Default:
					return new DefaultFormShowStrategy(form);
				case FormShowMode.AfterInitialization:
					return new AfterInitializationFormShowStrategy(form);
				default: throw new ArgumentException("showMode");
			}
		}
		public abstract void Initialize();
		public abstract void Restore();
		public abstract bool AllowInitialize { get; }
		public abstract bool AllowRestore { get; }
	}
	public class DefaultFormShowStrategy : FormShowStrategyBase {
		public DefaultFormShowStrategy(XtraForm form)
			: base(form) {
		}
		public override void Initialize() { }
		public override void Restore() { }
		public override bool AllowInitialize { get { return false; } }
		public override bool AllowRestore { get { return false; } }
	}
	public class AfterInitializationFormShowStrategy : FormShowStrategyBase {
		double opacity;
		bool initialized, restored;
		public AfterInitializationFormShowStrategy(XtraForm form)
			: base(form) {
			this.initialized = this.restored = false;
		}
		public override void Initialize() {
			this.opacity = Form.Opacity;
			Form.Opacity = 0f;
			this.initialized = true;
		}
		public override void Restore() {
			Form.Refresh();
			Form.Opacity = Opacity;
			this.restored = true;
		}
		public override bool AllowInitialize {
			get {
				if(Form.DesignModeCore) return false;
				return !this.initialized;
			}
		}
		public override bool AllowRestore {
			get {
				if(Form.DesignModeCore) return false;
				return !this.restored;
			}
		}
		public double Opacity { get { return opacity; } }
	}
	public enum BeakFormAlignment {
		Left,
		Top,
		Right,
		Bottom
	}
	public enum BeakLocation {
		Near,
		Center,
		Far,
		Custom
	}
	public class BeakForm : Form, ISupportLookAndFeel, IDXControl {
		public BeakForm()
			: base() {
			SetStyle(ControlStyles.FixedWidth | ControlStyles.FixedHeight, true);
			StartPosition = FormStartPosition.Manual;
			FormBorderStyle = FormBorderStyle.None;
			this.alignment = BeakFormAlignment.Left;
			this.helper = new ControlHelper(this, true);
			ShowBeak = true;
		}
		ControlHelper helper;
		protected ControlHelper Helper {
			get { return helper; }
			set {
				if(Helper == value)
					return;
				helper = value;
				OnHelperChanged();
			}
		}
		private void OnHelperChanged() {
			OnAppearanceChanged(Helper);
		}
		DevExpress.Utils.FormShadow.FormShadow formShadowCore;
		protected DevExpress.Utils.FormShadow.FormShadow FormShadow {
			get { return formShadowCore; }
			private set {
				if(FormShadow == value)
					return;
				formShadowCore = value;
				OnFormShadowChanged();
			}
		}
		protected virtual void OnFormShadowChanged() {
			UpdateBorderSize();
			UpdateFormShadowOwner();
		}
		protected virtual void UpdateBorderSize() {
			BorderSize = GetShadowWindowSize(Alignment);
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			InitializeFormShadow();
		}
		protected virtual void InitializeFormShadow() {
			if(!IsHandleCreated || FormShadow != null)
				return;
			FormShadow = new DevExpress.Utils.FormShadow.BeakFormShadow();
			FormShadow.Form = this;
			BorderSize = GetShadowWindowSize(Alignment);
		}
		protected override void Dispose(bool disposing) {
			if(FormShadow != null) FormShadow.Dispose();
			if(targetCtr != null) targetCtr = null;
			if(helper != null) helper.Dispose();
			helper = null;
			base.Dispose(disposing);
		}
		const int WS_MAXIMIZE = 0x01000000,
			WS_MAXIMIZEBOX = 0x00010000,
			WS_EX_CONTROLPARENT = 0x00010000;
		protected override CreateParams CreateParams {
			get {
				CreateParams res = base.CreateParams;
				res.ExStyle = FormShadowHelpers.NativeHelper.WS_EX_TOOLWINDOW | WS_EX_CONTROLPARENT;
				res.Style &= ~(FormShadowHelpers.NativeHelper.WS_MINIMIZE | WS_MAXIMIZE | FormShadowHelpers.NativeHelper.WS_CHILDWINDOW | FormShadowHelpers.NativeHelper.WS_DISABLED | WS_MAXIMIZEBOX); 
				return res;
			}
		}
		protected Size BorderSize { get; private set; }
		private BeakFormAlignment alignment;
		[DefaultValue(BeakFormAlignment.Left)]
		public BeakFormAlignment Alignment {
			get { return alignment; }
			set {
				if(alignment == value) return;
				alignment = value;
				OnOrientationChanged();
			}
		}
		protected virtual void OnOrientationChanged() {
			BorderSize = GetShadowWindowSize(Alignment);
		}
		[DefaultValue(BeakLocation.Near)]
		public BeakLocation BeakLocation {
			get;
			set;
		}
		bool showBeak;
		public bool ShowBeak {
			get { return showBeak; }
			set {
				if(ShowBeak == value) return;
				showBeak = value;
				OnShowBeakChanged();
			}
		}
		private void OnShowBeakChanged() {
			var window = GetShadowWindowByType(ShadowWindowTypes.Composite);
			if(window != null)
				window.IsBeakVisible = ShowBeak;
		}
		private Size GetShadowWindowSize(BeakFormAlignment alignment) {
			if(FormShadow == null || FormShadow.Windows.Count == 0) return Size.Empty;
			Rectangle winRect = new Rectangle(0, 0, Width, Height);
			var shadowWindow = GetShadowWindowByBeakAlignment(alignment);
			bool behind = false;
			if(shadowWindow == null) {
				shadowWindow = GetShadowWindowByType(ShadowWindowTypes.Composite);
				behind = true;
				if(shadowWindow == null)
					return Size.Empty;
			}
			var border = ShadowWindowLayoutCalculator.Calculate(BeakFormAlignment2ShadowWindowType(alignment), winRect, shadowWindow.GetPainter());
			switch(alignment) {
				case BeakFormAlignment.Left:
				case BeakFormAlignment.Right:
					if(behind) return new Size(border.Width - shadowWindow.Offset, border.Height);
					else return new Size(shadowWindow.Width - shadowWindow.Offset, shadowWindow.Height);
				case BeakFormAlignment.Top:
				case BeakFormAlignment.Bottom:
					if(behind) return new Size(border.Width, border.Height - shadowWindow.Offset);
					else return new Size(shadowWindow.Width, shadowWindow.Height - shadowWindow.Offset);
			}
			return Size.Empty;
		}
		ShadowWindowTypes BeakFormAlignment2ShadowWindowType(BeakFormAlignment alignment) {
			switch(alignment) {
				case BeakFormAlignment.Left: return ShadowWindowTypes.Right;
				case BeakFormAlignment.Top: return ShadowWindowTypes.Bottom;
				case BeakFormAlignment.Right: return ShadowWindowTypes.Left;
				default: return ShadowWindowTypes.Top;
			}
		}
		private BeakFormShadowWindow GetShadowWindowByBeakAlignment(BeakFormAlignment alignment) {
			return GetShadowWindowByType(BeakFormAlignment2ShadowWindowType(alignment));
		}
		private BeakFormShadowWindow GetShadowWindowByType(ShadowWindowTypes type) {
			if(FormShadow == null || FormShadow.Windows.Count == 0) return null;
			foreach(var window in FormShadow.Windows) {
				if(window.WindowType == type)
					return window as BeakFormShadowWindow;
			}
			return null;
		}
		Control targetCtr;
		public Control TargetControl {
			get { return targetCtr; }
			set {
				if(TargetControl == value)
					return;
				OnTargetControlChanging();
				targetCtr = value;
				OnTargetControlChanged();
			}
		}
		protected virtual void OnTargetControlChanging() {
			Owner = null;
			UpdateFormShadowOwner();
			if(TargetControl != null)
				UnSubscribeControlsEvents();
		}
		protected virtual void OnTargetControlChanged() {
			Owner = GetParentFormFromControl(targetCtr);
			UpdateFormShadowOwner();
			SubscribeControlsEvents();
			TargetBounds = TargetControl != null ? new Rectangle(TargetControl.PointToScreen(new Point(0, 0)), TargetControl.Size) : Rectangle.Empty;
		}
		protected virtual void UpdateFormShadowOwner() {
			if(FormShadow != null)
				FormShadow.SetOwner(Owner != null ? Owner.Handle : IntPtr.Zero);
		}
		protected virtual void SetOwner(Form owner) {
			Owner = owner;
			UpdateFormShadowOwner();
		}
		protected virtual Form GetParentFormFromControl(Control control) {
			return control.FindForm();
		}
		System.Collections.Generic.List<Control> ctrlRelativeTree = new System.Collections.Generic.List<Control>();
		private void UnSubscribeControlsEvents() {
			foreach(var ctrl in ctrlRelativeTree) {
				if(ctrl != null)
					ctrl.LocationChanged -= control_LocationChanged;
			}
		}
		private void SubscribeControlsEvents() {
			CreateControlRelativeTree(targetCtr);
			foreach(var ctrl in ctrlRelativeTree) {
				if(ctrl != null)
					ctrl.LocationChanged += control_LocationChanged;
			}
		}
		void CreateControlRelativeTree(Control control) {
			ctrlRelativeTree.Clear();
			ctrlRelativeTree.Add(control);
			Control ctrl = control.Parent;
			while(ctrl != null) {
				ctrlRelativeTree.Add(ctrl);
				ctrl = ctrl.Parent;
			}
		}
		void control_LocationChanged(object sender, EventArgs e) {
			if(targetCtr == null) {
				UnSubscribeControlsEvents();
				return;
			}
			Rectangle rect = new Rectangle(targetCtr.PointToScreen(new Point(0, 0)), targetCtr.Size);
			ShowForm(rect, Alignment, BeakLocation);
		}
		public void ShowForm(Form owner, Control control, BeakFormAlignment alignment, BeakLocation location) {
			SetOwner(owner);
			ShowForm(control, alignment, location);
		}
		public void ShowForm(Control control, BeakFormAlignment alignment, BeakLocation location) {
			TargetControl = control;
			Rectangle rect = new Rectangle(control.PointToScreen(new Point(0, 0)), control.Size);
			ShowForm(rect, alignment, location);
		}
		protected override void SetVisibleCore(bool value) {
			base.SetVisibleCore(value);
			if(value) {
				if(FormShadow == null) InitializeFormShadow();
				UpdateFormShadowOwner();
				UpdateLocation();
				UpdateBeakLocation();
				UpdateShadowWindow();
			}
		}
		public void ShowForm(Form owner, Point pt, BeakFormAlignment alignment, BeakLocation location) {
			SetOwner(owner);
			ShowForm(pt, alignment, location);
		}
		public void ShowForm(Point pt, BeakFormAlignment alignment, BeakLocation location) {
			ShowForm(new Rectangle(pt, Size.Empty), alignment, location);
		}
		public Rectangle TargetBounds { get; set; }
		public void ShowForm(Form owner, Rectangle rect, BeakFormAlignment alignment, BeakLocation location) {
			SetOwner(owner);
			ShowForm(rect, alignment, location);
		}
		public void ShowForm(Rectangle rect, BeakFormAlignment alignment, BeakLocation location) {
			Alignment = alignment;
			BeakLocation = location;
			TargetBounds = rect;
			Show();
		}
		private void UpdateShadowWindow() {
			var window = GetShadowWindowByType(ShadowWindowTypes.Composite);
			if(window != null)
				window.BeakFormAlignment = Alignment;
			OnShowBeakChanged();
		}
		protected virtual void UpdateLocation() {
			CalculateLocation(TargetBounds, Alignment, BeakLocation);
			if(ShouldChangeAlignment) {
				Alignment = GetOppositeOrientation(Alignment);
				CalculateLocation(TargetBounds, Alignment, BeakLocation);
			}
		}
		protected virtual void UpdateBeakLocation() {
			var direction = GetChangeBeakOrientationDirection();
			while(direction != ChangeBeakOrientDirection.None) {
				var beakPrev = BeakLocation;
				switch(direction) {
					case ChangeBeakOrientDirection.Left:
					case ChangeBeakOrientDirection.Up:
						if(BeakLocation == BeakLocation.Near) BeakLocation = BeakLocation.Center;
						else if(BeakLocation == BeakLocation.Center) BeakLocation = BeakLocation.Far;
						break;
					case ChangeBeakOrientDirection.Right:
					case ChangeBeakOrientDirection.Down:
						if(BeakLocation == BeakLocation.Center) BeakLocation = BeakLocation.Near;
						else if(BeakLocation == BeakLocation.Far) BeakLocation = BeakLocation.Center;
						break;
				}
				if(beakPrev == BeakLocation)
					break;
				BeakLocation = BeakLocation;
				CalculateLocation(TargetBounds, Alignment, BeakLocation);
				direction = GetChangeBeakOrientationDirection();
			}
		}
		private static BeakFormAlignment GetOppositeOrientation(BeakFormAlignment alignment) {
			if(alignment == BeakFormAlignment.Left) return BeakFormAlignment.Right;
			else if(alignment == BeakFormAlignment.Top) return BeakFormAlignment.Bottom;
			else if(alignment == BeakFormAlignment.Right) return BeakFormAlignment.Left;
			return BeakFormAlignment.Top;
		}
		enum ChangeBeakOrientDirection {
			None,
			Left,
			Right,
			Up,
			Down
		}
		ChangeBeakOrientDirection GetChangeBeakOrientationDirection() {
			Screen scr = Screen.FromControl(this);
			switch(Alignment) {
				case BeakFormAlignment.Left:
				case BeakFormAlignment.Right: {
						if(Location.Y < scr.WorkingArea.Y) return ChangeBeakOrientDirection.Down;
						if(Location.Y + Height > scr.WorkingArea.Bottom) return ChangeBeakOrientDirection.Up;
						break;
					}
				case BeakFormAlignment.Top:
				case BeakFormAlignment.Bottom: {
						if(Location.X < scr.WorkingArea.X) return ChangeBeakOrientDirection.Right;
						if(Location.X + Width > scr.WorkingArea.Right) return ChangeBeakOrientDirection.Left;
						break;
					}
			}
			return ChangeBeakOrientDirection.None;
		}
		bool ShouldChangeAlignment {
			get {
				Screen scr = Screen.FromControl(this);
				switch(Alignment) {
					case BeakFormAlignment.Left:
						return Location.X < scr.WorkingArea.X;
					case BeakFormAlignment.Top:
						return Location.Y < scr.WorkingArea.Y;
					case BeakFormAlignment.Right:
						return Location.X + Width > scr.WorkingArea.Right;
					case BeakFormAlignment.Bottom:
						return Location.Y + Height > scr.WorkingArea.Bottom;
				}
				return false;
			}
		}
		void CalculateLocation(Rectangle rect, BeakFormAlignment alignment, BeakLocation location) {
			var offset = GetOffsetByBeakOrientation(alignment, location);
			var window = GetShadowWindowByType(ShadowWindowTypes.Composite);
			if(window != null)
				window.BeakOffset = offset;
			switch(alignment) {
				case BeakFormAlignment.Left:
					Location = new Point(rect.X - Width - BorderSize.Width, rect.Y + rect.Height / 2 - offset);
					break;
				case BeakFormAlignment.Top:
					Location = new Point(rect.X + rect.Width / 2 - offset, rect.Y - Height - BorderSize.Height);
					break;
				case BeakFormAlignment.Right:
					Location = new Point(rect.Right + BorderSize.Width, rect.Y + rect.Height / 2 - offset);
					break;
				case BeakFormAlignment.Bottom:
					Location = new Point(rect.X + rect.Width / 2 - offset, rect.Bottom + BorderSize.Height);
					break;
			}
		}
		int customOffset;
		public int CustomOffset {
			get { return customOffset; }
			set { customOffset = value; }
		}
		int GetOffsetByBeakOrientation(BeakFormAlignment alignment, BeakLocation location) {
			if(location == BeakLocation.Custom)
				return CustomOffset;
			int res = 0;
			int step = 0;
			switch(alignment) {
				case BeakFormAlignment.Right:
				case BeakFormAlignment.Left:
					step = Height / 4;
					break;
				case BeakFormAlignment.Bottom:
				case BeakFormAlignment.Top:
					step = Width / 4;
					break;
			}
			if(location == BeakLocation.Near)
				res = step;
			if(location == BeakLocation.Center)
				res = 2 * step;
			if(location == BeakLocation.Far)
				res = 3 * step;
			return res;
		}
		#region ISupportLookAndFeel Members
		[ Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Appearance { get { return Helper == null ? AppearanceObject.Dummy : Helper.Appearance; } }
		[ Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public UserLookAndFeel LookAndFeel { get { return Helper == null ? null : Helper.LookAndFeel; } }
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		#endregion
		#region IDXControl Members
		bool useBaseFont = false;
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Font Font {
			get {
				if(useBaseFont)
					return base.Font;
				return Appearance.Font;
			}
			set { Appearance.Font = value; }
		}
		public void OnAppearanceChanged(object sender) {
			if(IsHandleCreated) {
				OnBackColorChanged(EventArgs.Empty);
				Invalidate();
			}
			if(!base.Font.Equals(Appearance.Font)) {
				this.useBaseFont = true;
				base.Font = Appearance.Font;
				this.useBaseFont = false;
			}
		}
		AppearanceDefault defaultAppearance = null;
		protected AppearanceDefault DefaultAppearance {
			get {
				if(defaultAppearance == null)
					defaultAppearance = CreateDefaultAppearance();
				return defaultAppearance;
			}
		}
		protected virtual AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault res = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control);
			if(LookAndFeel != null && LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				SkinElement form = GetSkin();
				form.Apply(res, LookAndFeel);
				LookAndFeelHelper.CheckColors(LookAndFeel, res, this);
			}
			return res;
		}
		SkinElement GetSkin() {
			if(LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin) return null;
			return CommonSkins.GetSkin(LookAndFeel)[CommonSkins.SkinForm];
		}
		void IDXControl.OnLookAndFeelStyleChanged(object sender) {
			this.defaultAppearance = null;
			OnAppearanceChanged(sender);
		}
		Control IDXControl.Control { get { return this; } }
		#endregion
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class SplitterInfoArgs : StyleObjectInfoArgs {
		bool isVertical = false;
		bool collapsableCore = false;
		bool invertedCore = false;
		Rectangle buttonCore;
		public SplitterInfoArgs(bool isVertical)
			: this(isVertical, false) {
		}
		public SplitterInfoArgs(bool isVertical, bool collapsable) {
			this.isVertical = isVertical;
			this.collapsableCore = collapsable;
		}
		public Rectangle Button { get { return buttonCore; } set { buttonCore = value; } }
		public bool IsVertical { get { return isVertical; } set { isVertical = value; } }
		public bool IsHorizontal { get { return !IsVertical; } set { isVertical = !value; } }
		public bool IsCollapsable { get { return collapsableCore; } set { collapsableCore = value; } }
		public bool Inverted { get { return invertedCore; } set { invertedCore = value; } }
	}
	public class SplitterPainter : StyleObjectPainter {
		ImageCollection verticalImgCore;
		ImageCollection horizontalImgCore;
		protected ImageCollection VerticalSplitGlyphs {
			get {
				if(verticalImgCore == null) {
					Image img = ResourceImageHelper.CreateImageFromResources(
							"DevExpress.Utils.Images.flatSplitterGlyphV.png", typeof(SplitterPainter).Assembly
						);
					verticalImgCore = new ImageCollection();
					verticalImgCore.ImageSize = new Size(img.Width, img.Height / 2);
					verticalImgCore.AddImageStripVertical(img);
				}
				return verticalImgCore;
			}
		}
		protected ImageCollection HorizontalSplitGlyphs {
			get {
				if(horizontalImgCore == null) {
					Image img = ResourceImageHelper.CreateImageFromResources(
							"DevExpress.Utils.Images.flatSplitterGlyphH.png", typeof(SplitterPainter).Assembly
						);
					horizontalImgCore = new ImageCollection();
					horizontalImgCore.ImageSize = new Size(img.Width / 2, img.Height);
					horizontalImgCore.AddImageStrip(img);
				}
				return horizontalImgCore;
			}
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			SplitterInfoArgs ee = e as SplitterInfoArgs;
			Size size = new Size(4, 4);
			return new Rectangle(Point.Empty, size);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			SplitterInfoArgs ee = e as SplitterInfoArgs;
			ee.Appearance.DrawBackground(e.Cache, e.Bounds);
			HatchStyle style = HatchStyle.Percent50;
			Rectangle r = e.Bounds;
			r.Inflate(-1, -1);
			if(ee.State == ObjectState.Hot) {
				e.Graphics.FillRectangle(e.Cache.GetSolidBrush(DevExpress.Utils.ColorUtils.FlatBarItemHighLightBackColor), e.Bounds);
			}
			using(HatchBrush brush = new HatchBrush(style, SystemColors.ControlDark, ee.Appearance.BackColor)) {
				e.Graphics.FillRectangle(brush, r);
			}
			if(ee.IsCollapsable) {
				Image img = GetCollapsableGlyph(r.Width < r.Height, ee.Inverted);
				Rectangle imgRect = CalcCollapsableGlyphRect(r, img);
				if(!imgRect.IsEmpty) e.Graphics.DrawImageUnscaled(img, imgRect);
			}
		}
		Rectangle CalcCollapsableGlyphRect(Rectangle splitter, Image img) {
			Rectangle imgRect = Rectangle.Empty;
			bool fVertical = splitter.Width < splitter.Height;
			if((fVertical && (splitter.Height >= img.Height)) || (!fVertical && (splitter.Width >= img.Width))) {
				imgRect = new Rectangle(
						splitter.Left + (splitter.Width - img.Width) / 2, splitter.Top + (splitter.Height - img.Height) / 2,
						img.Width, img.Height
					);
			}
			return imgRect;
		}
		Image GetCollapsableGlyph(bool vertical, bool inverted) {
			ImageCollection collection = vertical ? VerticalSplitGlyphs : HorizontalSplitGlyphs;
			return collection.Images[inverted ? 0 : 1];
		}
	}
	public class SplitterLineHelper {
		[ThreadStatic]
		static SplitterLineHelper _default;
		public static SplitterLineHelper Default {
			get {
				if(_default == null) _default = new SplitterLineHelper();
				return _default;
			}
		}
		public void DrawReversibleSplitter(IntPtr handle, Rectangle bounds) {
			DrawReversibleObject(handle, bounds, CreateSplitBrush());
		}
		public void DrawReversibleLine(IntPtr handle, Rectangle bounds) {
			DrawReversibleObject(handle, bounds, CreateLineBrush());
		}
		public void DrawReversibleLine(IntPtr handle, Point startPoint, Point endPoint) {
			Rectangle bounds;
			if(startPoint.Y == endPoint.Y) 
				bounds = new Rectangle(startPoint.X, startPoint.Y, endPoint.X - startPoint.X, 2);
			else
				bounds = new Rectangle(startPoint.X, startPoint.Y, 2, endPoint.Y - startPoint.Y);
			DrawReversibleObject(handle, bounds, CreateLineBrush());
		}
		public void DrawReversibleFrame(IntPtr handle, Rectangle bounds) {
			DrawReversibleFrame(handle, bounds, CreateLineBrush());
		}
		[System.Security.SecuritySafeCritical]
		protected void DrawReversibleObject(IntPtr handle, Rectangle bounds, IntPtr splitBrush) {
			Rectangle r = bounds;
			IntPtr dc = GetDCEx(handle, IntPtr.Zero, DCX_CACHE | DCX_LOCKWINDOWUPDATE);
			IntPtr saveBrush = SelectObject(dc, splitBrush);
			PatBlt(dc, r.X, r.Y, r.Width, r.Height, PATINVERT);
			SelectObject(dc, saveBrush);
			DeleteObject(splitBrush);
			ReleaseDC(handle, dc);
		}
		const int frameWidth = 2;
		[System.Security.SecuritySafeCritical]
		protected void DrawReversibleFrame(IntPtr handle, Rectangle bounds, IntPtr splitBrush) {
			Rectangle r = bounds;
			IntPtr dc = GetDCEx(handle, IntPtr.Zero, DCX_CACHE | DCX_LOCKWINDOWUPDATE);
			IntPtr saveBrush = SelectObject(dc, splitBrush);
			PatBlt(dc, r.X, r.Y, frameWidth, r.Height, PATINVERT);
			PatBlt(dc, r.X + frameWidth, r.Y, r.Width - frameWidth * 2, frameWidth, PATINVERT);
			PatBlt(dc, r.X + frameWidth, r.Bottom - frameWidth, r.Width - frameWidth * 2, frameWidth, PATINVERT);
			PatBlt(dc, r.Right - frameWidth, r.Y, frameWidth, r.Height, PATINVERT);
			SelectObject(dc, saveBrush);
			DeleteObject(splitBrush);
			ReleaseDC(handle, dc);
		}
		#region API
		[System.Security.SecuritySafeCritical]
		IntPtr CreateLineBrush() {
			short[] grayPattern = new short[8];
			for(int i = 0; i < 2; i++)
				grayPattern[i] = 0xff; 
			IntPtr hBitmap = CreateBitmap(2, 2, 1, 1, grayPattern);
			LOGBRUSH lb = new LOGBRUSH(BS_PATTERN, 0, hBitmap);
			IntPtr brush = CreateBrushIndirect(lb);
			DeleteObject(hBitmap);
			return brush;
		}
		[System.Security.SecuritySafeCritical]
		IntPtr CreateSplitBrush() {
			short[] grayPattern = new short[8];
			for(int i = 0; i < 8; i++)
				grayPattern[i] = (short)(0x5555 << (i & 1));
			IntPtr hBitmap = CreateBitmap(8, 8, 1, 1, grayPattern);
			LOGBRUSH lb = new LOGBRUSH(BS_PATTERN, 0, hBitmap);
			IntPtr brush = CreateBrushIndirect(lb);
			DeleteObject(hBitmap);
			return brush;
		}
		[StructLayout(LayoutKind.Sequential)]
		class LOGBRUSH {
			public LOGBRUSH(int style, int color, IntPtr hatch) {
				this.Style = style;
				this.Color = color;
				this.Hatch = hatch;
			}
			public int Style;
			public int Color;
			public IntPtr Hatch;
		}
		[DllImport("GDI32.dll", CharSet = CharSet.Auto)]
		static extern bool PatBlt(IntPtr hdc, int left, int top, int width, int height, int rop);
		[DllImport("GDI32.dll", CharSet = CharSet.Auto)]
		static extern IntPtr CreateBrushIndirect(LOGBRUSH lb);
		[DllImport("GDI32.dll", CharSet = CharSet.Auto)]
		static extern IntPtr CreateBitmap(int width, int height, int planes, int bitsPerPixel, short[] lpvBits);
		[DllImport("GDI32.dll")]
		static extern bool DeleteObject(IntPtr hObject);
		[DllImport("GDI32.dll")]
		static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);
		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		static extern IntPtr GetDCEx(IntPtr hwnd, IntPtr clip, int flags);
		const int DCX_CACHE = 0x00000002,
				  BS_PATTERN = 3,
				  PATINVERT = 0x005A0049,
				  DCX_LOCKWINDOWUPDATE = 0x00000400
									   ;
		#endregion
	}
	public class SkinSplitterPainter : SkinCustomPainter {
		public SkinSplitterPainter(ISkinProvider provider) : base(provider) { }
		protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) {
			SplitterInfoArgs ee = e as SplitterInfoArgs;
			SkinElement skinElement = CommonSkins.GetSkin(Provider)[ee.IsVertical ? CommonSkins.SkinSplitter : CommonSkins.SkinSplitterHorz];
			SkinElementInfo info = new SkinElementInfo(skinElement, e.Bounds);
			info.State = e.State;
			info.ImageIndex = 0;
			if(info.State == ObjectState.Hot) info.ImageIndex = 1;
			if(info.Element.Glyph != null && info.Element.Glyph.ImageCount == 6) {
				if(ee.IsCollapsable) info.GlyphIndex = (ee.Inverted ? 2 : 1) * 2 + info.ImageIndex;
			}
			return info;
		}
	}
	public class SplitterHelper {
		public static ObjectPainter GetPainter(UserLookAndFeel lookAndFeel) {
			switch(lookAndFeel.ActiveStyle) {
				case ActiveLookAndFeelStyle.Skin:
					return new SkinSplitterPainter(lookAndFeel);
			}
			return new SplitterPainter();
		}
	}
	public interface IMouseWheelSupportIgnore {
		bool Ignore { get; }
	}
	public interface IChildControlsIgnoreMouseWheel {
		bool Ignore { get; }
	}
	public interface IMouseWheelSupport {
		void OnMouseWheel(MouseEventArgs e);
	}
	public interface IMouseWheelContainer {
		void BaseOnMouseWheel(MouseEventArgs e);
		Control Control { get; }
	}
	public class MouseWheelHelper {
		public static bool SmartMouseWheelProcessing = true;
		public static bool ProcessSmartMouseWheel(Control control, MouseEventArgs e) {
			if(!SmartMouseWheelProcessing) return false;
			MouseWheelContainerForm form = control.FindForm() as MouseWheelContainerForm;
			if(form != null && form.IsMdiChild) {
				MouseWheelContainerForm parentForm = form.MdiParent as MouseWheelContainerForm;
				if(parentForm != null)
					form = parentForm;
			}
			if(form == null || !control.IsHandleCreated) return false;
			Point screen = control.PointToScreen(e.Location);
			IMouseWheelContainer wheelContainer = form as IMouseWheelContainer;
			Point p = wheelContainer.Control.PointToClient(screen);
			DXMouseEventArgs eh = Clone(DXMouseEventArgs.GetMouseArgs(e), p);
			ProcessMouseWheel(wheelContainer, eh);
			if(e is HandledMouseEventArgs) {
				((HandledMouseEventArgs)e).Handled = true;
			}
			return true;
		}
		static DXMouseEventArgs Clone(DXMouseEventArgs source, Point newLocation) {
			DXMouseEventArgs res = new DXMouseEventArgs(source.Button, source.Clicks, newLocation.X, newLocation.Y, source.Delta);
			res.ishMouseWheel = source.IsHMouseWheel;
			return res;
		}
		public static bool IsAllowSmartMouseWheel(Control control, MouseEventArgs e) {
			return ProcessSmartMouseWheel(control, e);
		}
		public static bool IsAllowSmartMouseWheel(Control control) {
			if(!SmartMouseWheelProcessing) return false;
			return (control.FindForm() is IMouseWheelContainer);
		}
		public static void DoMouseWheel(IMouseWheelContainer container, MouseEventArgs e) {
			if(SmartMouseWheelProcessing) {
				ProcessMouseWheel(container, e);
			}
			container.BaseOnMouseWheel(e);
		}
		static void ProcessMouseWheel(IMouseWheelContainer container, MouseEventArgs e) {
			container.BaseOnMouseWheel(e);
			HandledMouseEventArgs ee = e as HandledMouseEventArgs;
			if(ee != null && ee.Handled) return;
			Control controlInPosition = FindControl(e.Location, container.Control);
			if(controlInPosition != container.Control) {
				Point client = controlInPosition.PointToClient(container.Control.PointToScreen(e.Location));
				ProcessControlMouseWheel(Clone(DXMouseEventArgs.GetMouseArgs(e), client), controlInPosition);
			}
		}
		static void ProcessControlMouseWheel(MouseEventArgs e, Control controlInPosition) {
			IMouseWheelSupport control = controlInPosition as IMouseWheelSupport;
			if(control != null) control.OnMouseWheel(e);
		}
		static Control FindControl(Point point, Control parent) {
			for(int n = 0; n < parent.Controls.Count; n++) {
				Point screenPoint = parent.IsHandleCreated ? parent.PointToScreen(point) : point;
				Control ctrl = parent.Controls[n];
				if(CanIgnoreControl(ctrl)) continue;
				if(ctrl.Bounds.Contains(point)) {
					if(ctrl is IChildControlsIgnoreMouseWheel && ((IChildControlsIgnoreMouseWheel)ctrl).Ignore) return ctrl;
					Control res = FindControl(ctrl.IsHandleCreated ? ctrl.PointToClient(screenPoint) : point, ctrl);
					return res == null ? ctrl : res;
				}
			}
			return parent;
		}
		static bool CanIgnoreControl(Control ctrl) {
			if(!ctrl.Visible || !ctrl.Enabled) return true;
			if(ctrl is ScrollBarBase) return true;
			if(ctrl is IMouseWheelSupportIgnore && ((IMouseWheelSupportIgnore)ctrl).Ignore) return true;
			return false;
		}
	}
}
namespace DevExpress.Utils.Mdi {
	public static class ControlState {
		static FieldInfo fiState = null;
		static bool GetState(Control control, int flag) {
			if(fiState == null) {
				fiState = typeof(Control).GetField("state", BindingFlags.NonPublic | BindingFlags.Instance);
			}
			if(fiState == null) return false;
			return GetStateCore((int)fiState.GetValue(control), flag);
		}
		static bool GetStateCore(int state, int flag) {
			return (state & flag) != 0;
		}
		public const int STATE_CREATINGHANDLE = 0x40000;
		public const int STATE_PAINTEXCEPTION = 0x400000;
		public static bool IsCreatingHandle(Control control) {
			return (control != null) && GetState(control, STATE_CREATINGHANDLE);
		}
		public static bool IsPaintError(Control control) {
			return control != null && GetState(control, STATE_PAINTEXCEPTION);
		}
		public static void CheckPaintError(Control control) {
		}
	}
	public static class MdiClientSubclasserService {
		static readonly IDictionary clients = new Hashtable();
		static readonly System.Collections.Generic.IDictionary<MdiClient, int> updatingClients = 
			new System.Collections.Generic.Dictionary<MdiClient, int>();
		public static void Register(MdiClient mdiClient, NativeWindow subclasser) {
			lock(clients.SyncRoot) {
				if(mdiClient != null)
					clients.Add(mdiClient, subclasser);
			}
		}
		public static void Unregister(MdiClient mdiClient) {
			lock(clients.SyncRoot) {
				if(mdiClient != null)
					clients.Remove(mdiClient);
			}
		}
		public static NativeWindow FromMdiClient(MdiClient client) {
			lock(clients.SyncRoot) {
				return clients[client] as NativeWindow;
			}
		}
		public static MdiClient GetMdiClient(Form mdiParent) {
			if(mdiParent == null)
				return null;
			if(!mdiParent.IsMdiContainer)
				return null;
			foreach(Control control in mdiParent.Controls) {
				MdiClient clientCandidate = control as MdiClient;
				if(clientCandidate != null) {
					return clientCandidate;
				}
			}
			return null;
		}
		public static bool IsUpdatingMdiClient(MdiClient mdiClient) {
			int updateCount;
			return (mdiClient != null) && 
				updatingClients.TryGetValue(mdiClient, out updateCount) && updateCount > 0;
		}
		public static void BeginUpdateMdiClient(MdiClient mdiClient) {
			if(mdiClient != null) {
				int updateCount;
				if(!updatingClients.TryGetValue(mdiClient, out updateCount)) 
					updatingClients.Add(mdiClient, 1);
				else updatingClients[mdiClient] = (updateCount++);
			}
		}
		public static void EndUpdateMdiClient(MdiClient mdiClient) {
			if(mdiClient != null) {
				int updateCount;
				if(updatingClients.TryGetValue(mdiClient, out updateCount)) {
					if(--updateCount == 0)
						updatingClients.Remove(mdiClient);
					else updatingClients[mdiClient] = updateCount;
				}
			}
		}
	}
	public interface ISysCommandListener {
		void PreviewMessage(IntPtr hWnd, int cmd);
	}
}
