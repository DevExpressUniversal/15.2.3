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
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.LookAndFeel;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Styles;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.InternalItems;
using System.Runtime.Serialization;
using System.CodeDom;
using System.ComponentModel.Design.Serialization;
using System.Collections.Generic;
using DevExpress.Utils.Menu;
using DevExpress.LookAndFeel.Helpers;
namespace DevExpress.XtraBars {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class BarManagerProperties : IOptionsMultiColumnOwner {
		BarAndDockingController controller;
		int barItemHorzIndent, barItemVertIndent, mostRecentItemsPercent;
		bool scaleIcons, scaleEditors, largeIcons, allowLinkLighting;
		Size glyphSize, largeGlyphSize;
		AnimationType menuAnimationType;
		bool submenuHasShadow; 
		protected internal int MostRecentlyUsedClickCount;
		public BarManagerProperties(BarAndDockingController controller) {
			this.menuAnimationType = AnimationType.System;
			this.controller = controller;
			this.barItemHorzIndent = this.barItemVertIndent = -1;
			this.mostRecentItemsPercent = 95;
			MostRecentlyUsedClickCount = CalcMostRecentlyUsedClickCount();
			this.allowLinkLighting = true;
			this.submenuHasShadow = true;
			this.largeIcons = false;
			this.scaleEditors = false;
			this.scaleIcons = true;
			this.glyphSize = DefaultGlyphSizeCore;
			this.largeGlyphSize = DefaultLargeGlyphSizeCore;
		}
		OptionsMultiColumn optionsMultiColumn;
		void ResetOptionsMultiColumn() { OptionsMultiColumn.Reset(); }
		bool ShouldSerializeOptionsMultiColumn() { return OptionsMultiColumn.ShouldSerializeCore(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OptionsMultiColumn OptionsMultiColumn {
			get {
				if(optionsMultiColumn == null)
					optionsMultiColumn = new OptionsMultiColumn(this);
				return optionsMultiColumn;
			}
		}
		protected BarAndDockingController Controller { get { return controller; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerPropertiesMenuAnimationType"),
#endif
 DefaultValue(AnimationType.System), Category("Appearance")]
		public AnimationType MenuAnimationType {
			get { return menuAnimationType; }
			set { menuAnimationType = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerPropertiesBarItemVertIndent"),
#endif
 DefaultValue(-1), Category("Appearance"), NotifyParentProperty(true)]
		public virtual int BarItemVertIndent {
			get { return barItemVertIndent; }
			set {
				if(value < 0) value = -1;
				if(BarItemVertIndent == value) return;
				barItemVertIndent = value;
				OnChanged();
			}
		}
		Size DefaultGlyphSizeCore { get { return new Size(16, 16); } }
		void ResetGlyphSize() { DefaultGlyphSize = DefaultGlyphSizeCore; }
		bool ShouldSerializeGlyphSize() { return DefaultGlyphSize == DefaultGlyphSizeCore; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerPropertiesDefaultGlyphSize"),
#endif
 Category("Appearance"), NotifyParentProperty(true)]
		public virtual Size DefaultGlyphSize {
			get { return glyphSize; }
			set {
				if(DefaultGlyphSize == value) return;
				glyphSize = value;
				OnChanged();
			}
		}
		Size DefaultLargeGlyphSizeCore { get { return new Size(32, 32); } }
		void ResetLargeGlyphSize() { DefaultLargeGlyphSize = DefaultLargeGlyphSizeCore; }
		bool ShouldSerializeLargeGlyphSize() { return DefaultLargeGlyphSize == DefaultLargeGlyphSizeCore; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerPropertiesDefaultLargeGlyphSize"),
#endif
 Category("Appearance"), NotifyParentProperty(true)]
		public virtual Size DefaultLargeGlyphSize {
			get { return largeGlyphSize; }
			set {
				if(DefaultLargeGlyphSize == value) return;
				largeGlyphSize = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerPropertiesBarItemHorzIndent"),
#endif
 DefaultValue(-1), Category("Appearance"), NotifyParentProperty(true)]
		public virtual int BarItemHorzIndent {
			get { return barItemHorzIndent; }
			set {
				if(value < 0) value = -1;
				if(BarItemHorzIndent == value) return;
				barItemHorzIndent = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerPropertiesSubmenuHasShadow"),
#endif
 DefaultValue(true), Category("Appearance"), NotifyParentProperty(true)]
		public virtual bool SubmenuHasShadow {
			get { return submenuHasShadow; }
			set {
				submenuHasShadow = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerPropertiesMostRecentItemsPercent"),
#endif
 DefaultValue(95), NotifyParentProperty(true)]
		public int MostRecentItemsPercent {
			get { return mostRecentItemsPercent; }
			set {
				if(value < 0) value = 0;
				if(value > 100) value = 100;
				if(value == MostRecentItemsPercent) return;
				mostRecentItemsPercent = value;
				MostRecentlyUsedClickCount = CalcMostRecentlyUsedClickCount();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerPropertiesAllowLinkLighting"),
#endif
 DefaultValue(true), Category("Appearance"), NotifyParentProperty(true)]
		public virtual bool AllowLinkLighting {
			get { return allowLinkLighting; }
			set {
				if(AllowLinkLighting == value) return;
				allowLinkLighting = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerPropertiesScaleEditors"),
#endif
 DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool ScaleEditors {
			get { return scaleEditors; }
			set {
				if(ScaleEditors == value) return;
				scaleEditors = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerPropertiesScaleIcons"),
#endif
 DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool ScaleIcons {
			get { return scaleIcons; }
			set {
				if(ScaleIcons == value) return;
				scaleIcons = value;
				OnChanged();
			}
		}
		DefaultBoolean largeIconsInMenu = DefaultBoolean.Default;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerPropertiesScaleIcons"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), NotifyParentProperty(true)]
		public DefaultBoolean LargeIconsInMenu {
			get { return largeIconsInMenu; }
			set {
				if(LargeIconsInMenu == value) return;
				largeIconsInMenu = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerPropertiesLargeIcons"),
#endif
 DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool LargeIcons {
			get { return largeIcons; }
			set {
				if(LargeIcons == value) return;
				largeIcons = value;
				OnChanged();
			}
		}
		public int GetLinkHorzIndent() {
			if(BarItemHorzIndent != -1) return BarItemHorzIndent;
			return PaintStyle.DrawParameters.Constants.BarItemHorzIndent;
		}
		public int GetLinkVertIndent() {
			if(BarItemVertIndent != -1) return BarItemVertIndent;
			return PaintStyle.DrawParameters.Constants.BarItemVertIndent;
		}
		protected BarManagerPaintStyle PaintStyle { get { return Controller.PaintStyle; } }
		public virtual void ResetStyleDefaults() {
			AllowLinkLighting = PaintStyle.DrawParameters.Constants.AllowLinkLighting;
		}
		int CalcMostRecentlyUsedClickCount() {
			if(MostRecentItemsPercent > 99) return 0;
			for(int i = 1; i < int.MaxValue; i++) {
				if((i * (100 - MostRecentItemsPercent) / 100) > 0) return i;
			}
			return 0;
		}
		protected internal virtual void OnChanged() {
			Controller.OnChanged();
		}
		void IOptionsMultiColumnOwner.OnChanged() {
			OnChanged();
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class RibbonProperties {
		BarAndDockingController controller;
		bool scaleIcons = false;
		bool scaleEditors = false;
		public RibbonProperties(BarAndDockingController controller) {
			this.controller = controller;
		}
		protected BarAndDockingController Controller { get { return controller; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPropertiesScaleIcons"),
#endif
 DefaultValue(false), Category("Appearance")]
		public bool ScaleIcons {
			get { return scaleIcons; }
			set {
				if(ScaleIcons == value)
					return;
				scaleIcons = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonPropertiesScaleEditors"),
#endif
 DefaultValue(false), Category("Appearance")]
		public bool ScaleEditors {
			get { return scaleEditors; }
			set {
				if(ScaleEditors == value)
					return;
				scaleEditors = value;
				OnChanged();
			}
		}
		protected virtual void OnChanged() {
			Controller.OnChanged();
		}
	}
	[DesignerCategory("Component"),
	 Description("Provides default settings for all XtraBars controls (bars, dock panels, Ribbon controls and tabbed windows of the XtraTabbedMdiManager component)."),
	ToolboxTabName(AssemblyInfo.DXTabNavigation), DXToolboxItem(true),
	ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "DefaultBarAndDockingController")
	]
	public class DefaultBarAndDockingController : Component {
		public DefaultBarAndDockingController() { }
		public DefaultBarAndDockingController(IContainer container) : this() {
			container.Add(this);
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DefaultBarAndDockingControllerController"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BarAndDockingController Controller {
			get { return BarAndDockingController.Default; }
		}
	}
	public class BarAndDockingControllerLookAndFeel : UserLookAndFeel, IFormLookAndFeel {
		public BarAndDockingControllerLookAndFeel(object ownerControl) : base(ownerControl) { }
		float touchScaleFactor = 2.0f;
		[DefaultValue(2.0f),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual float FormTouchScaleFactor {
			get { return touchScaleFactor; }
			set {
				if(FormTouchScaleFactor == value)
					return;
				touchScaleFactor = value;
				OnStyleChanged();
			}
		}
		bool IFormLookAndFeel.IsUserControl { get { return false; } }
		DefaultBoolean touchUIMode = DefaultBoolean.Default;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual DefaultBoolean FormTouchUIMode {
			get { return touchUIMode; }
			set {
				if(FormTouchUIMode == value)
					return;
				touchUIMode = value;
				OnStyleChanged();
			}
		}
		public override bool GetTouchUI() {
			if(FormTouchUIMode != DefaultBoolean.Default)
				return FormTouchUIMode == DefaultBoolean.True;
			return base.GetTouchUI();
		}
		public override float GetTouchScaleFactor() {
			if(FormTouchUIMode == DefaultBoolean.Default)
				return base.GetTouchScaleFactor();
			return FormTouchUIMode == DefaultBoolean.True ? FormTouchScaleFactor : 1.0f;
		}
	}
	[Designer("DevExpress.XtraBars.Design.BarAndDockingControllerDesigner, " + AssemblyInfo.SRAssemblyBarsDesignFull, typeof(IDesigner)),
	DesignerCategory("Component"),
	Description("Provides settings for controls included in the XtraBars Suite (bars, dock panels, Ribbon controls and tabbed windows of the XtraTabbedMdiManager component). The component manipulates settings for one or multiple controls."),
	ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "BarAndDockingController"),
	ToolboxTabName(AssemblyInfo.DXTabNavigation), DXToolboxItem(true)
	#if DXWhidbey
	, System.ComponentModel.Design.Serialization.DesignerSerializer("DevExpress.Serialization.CodeDom.DXComponentCodeDomSerializer, " + AssemblyInfo.SRAssemblyDesignFull, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")
	#endif
	]
	public class BarAndDockingController : Component, ISupportInitialize, IAppearanceOwner {
		Hashtable bitmaps;
		BarAndDockingControllerLookAndFeel lookAndFeel;
		ArrayList cursors;
		BackstageViewAppearances appearancesBackstageView;
		RibbonAppearances appearancesRibbon;
		BarManagerAppearances appearancesBar;
		BarManagerProperties propertiesBar;
		RibbonProperties propertiesRibbon;
		BarManagerPaintStyleCollection paintStyles;
		DocumentManagerAppearances appearancesDocumentManager;
		EventInfo eventInfo;
		bool hasRibbon = false;
		int paintStyleIndex, lockUpdate, lockInitialize;
		string paintStyleName;
		Hashtable clients = new Hashtable();
		public event EventHandler Changed;
		DockManagerAppearances appearancesDocking;
		protected virtual BackstageViewAppearances CreateAppearancesBackstageView() { return new BackstageViewAppearances(this); }
		protected virtual RibbonAppearances CreateAppearancesRibbon() { return new RibbonAppearances(this); }
		protected virtual DocumentManagerAppearances CreateAppearancesDocumentManager() { return new DocumentManagerAppearances(this); }
		protected virtual BarManagerAppearances CreateAppearancesBar() { return new BarManagerAppearances(this); }
		protected virtual BarManagerProperties CreatePropertiesBar() { return new BarManagerProperties(this); }
		protected virtual RibbonProperties CreatePropertiesRibbon() { return new RibbonProperties(this); }
		protected virtual DockManagerAppearances CreateAppearancesDocking() { return new DockManagerAppearances(this); }
		IContainer container;
		public BarAndDockingController(IContainer container) : this() {
			this.container = container;
			this.container.Add(this);
			XtraForm form = container as XtraForm;
			if(form != null) {
				LookAndFeel.ParentLookAndFeel = form.LookAndFeel;
			}
		}
		class EventInfo {
			WeakReference weakRef;
			System.Threading.SynchronizationContext context;
			public EventInfo(BarAndDockingController controller) {
				weakRef = new WeakReference(controller);
				context = AsyncOperationManager.SynchronizationContext;
			}
			public void OnUserPreferenceChanged(object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e) {
				BarAndDockingController controller = (BarAndDockingController)weakRef.Target;
				if(controller == null)
					return;
				if(context == null)
					controller.OnUserPreferenceChanged(sender, e);
				else {
					try {
#if DEBUGTEST
						if(BarNativeMethods.HasNoMessageQueue())
							controller.OnUserPreferenceChanged(sender, e);
						else
#endif
							context.Send((state) => controller.OnUserPreferenceChanged(sender, e), null);
					}
					catch(InvalidAsynchronousStateException) {
						controller.OnUserPreferenceChanged(sender, e);
					}
				}
			}
		}
		static readonly object controllersSyncObj = new object();
		static List<EventInfo> controllers = new List<EventInfo>();
		static BarAndDockingController() {
			try {
				Microsoft.Win32.SystemEvents.InvokeOnEventsThread(new MethodInvoker(SubscribeUserPreferenceChanged));
			}
			catch(InvalidOperationException) { }
		}
		static void SubscribeUserPreferenceChanged() {
			Microsoft.Win32.SystemEvents.UserPreferenceChanged += OnUserPreferenceChangedInternal;
		}
		static void UnsubscribeUserPreferenceChanged() {
			Microsoft.Win32.SystemEvents.UserPreferenceChanged -= OnUserPreferenceChangedInternal;
		}
		static void OnUserPreferenceChangedInternal(object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e) {
			List<EventInfo> eventInfos = null;
			lock(controllersSyncObj)
				eventInfos = new List<EventInfo>(controllers);
			foreach(EventInfo eventInfo in eventInfos) {
				try { eventInfo.OnUserPreferenceChanged(sender, e); }
				catch { }
			}
		}
		public BarAndDockingController() {
			this.lookAndFeel = new BarAndDockingControllerLookAndFeel(this);
			this.lookAndFeel.StyleChanged += new EventHandler(OnLookAndFeelChanged);
			this.appearancesBackstageView = CreateAppearancesBackstageView();
			this.appearancesBackstageView.Changed += new EventHandler(OnAppearanceBackstageViewChanged);
			this.appearancesDocumentManager = CreateAppearancesDocumentManager();
			this.appearancesDocumentManager.Changed += new EventHandler(OnAppearancesDocumentManagerChanged);
			this.appearancesRibbon = CreateAppearancesRibbon();
			this.appearancesRibbon.Changed += new EventHandler(OnAppearancesRibbonChanged);
			this.appearancesBar = CreateAppearancesBar(); 
			this.propertiesBar = CreatePropertiesBar();
			this.propertiesRibbon = CreatePropertiesRibbon();
			this.lockInitialize = 0;
			this.lockUpdate = 0;
			this.paintStyles = new BarManagerPaintStyleCollection(this);
			this.paintStyleIndex = -1;
			this.paintStyleName = "Default"; 
			this.appearancesDocking = CreateAppearancesDocking();
			RegisterPaintStyles();
			eventInfo = new EventInfo(this);
			lock(controllersSyncObj)
				controllers.Add(eventInfo);
		}
		void OnAppearanceBackstageViewChanged(object sender, EventArgs e) {
			OnChanged();
		}
		void OnAppearancesRibbonChanged(object sender, EventArgs e) {
			OnChanged();
		}
		void OnAppearancesDocumentManagerChanged(object sender, EventArgs e) {
			OnChanged();
		}
		bool disposing = false;
		internal bool Disposing { get { return disposing; } }
		protected override void Dispose(bool disposing) {
			this.disposing = true;
			lock(controllersSyncObj)
				controllers.Remove(eventInfo);
			if(disposing) {
				NotifyClients(true);
				this.clients.Clear();
				appearancesDocumentManager.Dispose();
				appearancesBackstageView.Dispose();
				appearancesRibbon.Dispose();
				appearancesBar.Dispose();
				appearancesDocking.Dispose();
				if(LookAndFeel != null) {
					LookAndFeel.StyleChanged -= new EventHandler(OnLookAndFeelChanged);
					LookAndFeel.Dispose();
				}
				DestroyPaintStyles();
				DestroyBitmaps();
			}
			base.Dispose(disposing);
		}
		public virtual void AddClient(IBarAndDockingControllerClient client) {
			this.clients[client] = 1;
			CheckForRibbon();
		}
		public void RemoveClient(IBarAndDockingControllerClient client) {
			if(this.clients.ContainsKey(client)) this.clients.Remove(client);
			CheckForRibbon();
		}
		void CheckForRibbon() {
			this.hasRibbon = false;
			foreach(object obj in this.clients.Keys) {
				if(obj is DevExpress.XtraBars.Ribbon.RibbonControl) {
					this.hasRibbon = true;
					break;
				}
			}
		}
		protected virtual void DestroyBitmaps() {
			if(bitmaps != null) {
				foreach(DictionaryEntry entry in bitmaps) {
					Image img = entry.Value as Image;
					if(img != null) img.Dispose();
				}
				bitmaps.Clear();
			}
			if(cursors != null) {
				foreach(Cursor cur in this.cursors) {
					cur.Dispose();
				}
				this.cursors.Clear();
			}
		}
		protected virtual void DestroyPaintStyles() {
			for(int n = PaintStyles.Count - 1; n >= 0; n--) {
				BarManagerPaintStyle paintStyle = PaintStyles[n];
				paintStyle.Dispose();
				PaintStyles.RemoveAt(n);
			}
		}
		public virtual void BeginInit() {
			this.lockInitialize ++;
		}
		public virtual void EndInit() {
			if(--this.lockInitialize == 0) {
				OnChanged();
			}
		}
		public virtual void BeginUpdate() {
			this.lockUpdate ++;
		}
		public virtual void EndUpdate() {
			if(--this.lockUpdate == 0) {
				OnChanged();
			}
		}
		protected internal bool IsLoading { get { return this.lockInitialize != 0; } }
		protected virtual void OnUserPreferenceChanged(object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e) {
			if(e.Category != Microsoft.Win32.UserPreferenceCategory.Color) return;
			this.BeginUpdate();
			try {
				paintStyleIndex = -1;
				this.AppearancesBar.ResetTheme();
				DevExpress.Utils.WXPaint.Painter.ThemeChanged();
			} finally {
				this.EndUpdate();
			}
		}
		protected virtual void RegisterPaintStyles() {
			PaintStyles.Add(new FakeBarManagerPaintStyle(delegate(BarManagerPaintStyleCollection paintStyles) { return new BarManagerOfficeXPPaintStyle(paintStyles); }, "OfficeXP"));
			PaintStyles.Add(new FakeBarManagerPaintStyle(delegate(BarManagerPaintStyleCollection paintStyles) { return new BarManagerOffice2000PaintStyle(paintStyles); }, "Office2000"));
			PaintStyles.Add(new FakeBarManagerPaintStyle(delegate(BarManagerPaintStyleCollection paintStyles) { return new BarManagerWindowsXPPaintStyle(paintStyles); }, "WindowsXP"));
			PaintStyles.Add(new FakeBarManagerPaintStyle(delegate(BarManagerPaintStyleCollection paintStyles) { return new BarManagerOffice2003PaintStyle(paintStyles); }, "Office2003"));
			PaintStyles.Add(new FakeBarManagerPaintStyle(delegate(BarManagerPaintStyleCollection paintStyles) { return new SkinBarManagerPaintStyle(paintStyles); }, "Skin"));
		}
		bool ShouldSerializeAppearancesDocumentManager() { return DevExpress.Utils.Design.UniversalTypeConverter.ShouldSerializeObject(AppearancesDocumentManager); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarAndDockingControllerAppearancesDocumentManager"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual DocumentManagerAppearances AppearancesDocumentManager {
			get { return appearancesDocumentManager; }
		}
		bool ShouldSerializeAppearancesBackstageView() { return DevExpress.Utils.Design.UniversalTypeConverter.ShouldSerializeObject(AppearancesBackstageView); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarAndDockingControllerAppearancesBackstageView"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BackstageViewAppearances AppearancesBackstageView {
			get { return appearancesBackstageView; }
		}
		bool ShouldSerializeAppearancesRibbon() { return DevExpress.Utils.Design.UniversalTypeConverter.ShouldSerializeObject(AppearancesRibbon); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarAndDockingControllerAppearancesRibbon"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual RibbonAppearances AppearancesRibbon {
			get { return appearancesRibbon; }
		}
		bool ShouldSerializeLookAndFeel() { return LookAndFeel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarAndDockingControllerLookAndFeel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BarAndDockingControllerLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		bool ShouldSerializeAppearancesBar() { return DevExpress.Utils.Design.UniversalTypeConverter.ShouldSerializeObject(AppearancesBar); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarAndDockingControllerAppearancesBar"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BarManagerAppearances AppearancesBar {
			get { return appearancesBar; }
		}
		bool ShouldSerializePropertiesBar() { return DevExpress.Utils.Design.UniversalTypeConverter.ShouldSerializeObject(PropertiesBar); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarAndDockingControllerPropertiesBar"),
#endif
 Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BarManagerProperties PropertiesBar {
			get { return propertiesBar; }
		}
		bool ShouldSerializePropertiesRibbon() { return DevExpress.Utils.Design.UniversalTypeConverter.ShouldSerializeObject(PropertiesRibbon); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarAndDockingControllerPropertiesRibbon"),
#endif
 Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual RibbonProperties PropertiesRibbon {
			get { return propertiesRibbon; }
		}
		bool ShouldSerializeAppearancesDocking() { return DevExpress.Utils.Design.UniversalTypeConverter.ShouldSerializeObject(AppearancesDocking); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarAndDockingControllerAppearancesDocking"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual DockManagerAppearances AppearancesDocking { get { return appearancesDocking; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual BarManagerPaintStyleCollection PaintStyles {
			get { return paintStyles; } 
		}
		[ThreadStatic]
		static BarAndDockingController defaultController = null;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarAndDockingControllerDefault")]
#endif
		public static BarAndDockingController Default {
			get {
				if(defaultController == null) defaultController = new BarAndDockingController();
				return defaultController;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarAndDockingControllerPaintStyleName"),
#endif
 DefaultValue("Default"), Category("Appearance"), TypeConverter("DevExpress.XtraBars.TypeConverters.PaintStyleNameTypeConverter, " + AssemblyInfo.SRAssemblyBarsDesign)]
		public virtual string PaintStyleName {
			get { return paintStyleName; }
			set {
				if(!PaintStyles.Contains(value)) value = "Default";
				if(PaintStyleName == value) return;
				paintStyleName = value;
				paintStyleIndex = -1;
				OnChanged();
				if(DesignMode && !IsLoading) {
					ResetStyleDefaults();
				}
			}
		}
		[Browsable(false)]
		public virtual ArrayList DragCursors {
			get {
				if(cursors == null)
					InitBitmaps();
				return cursors;
			}
		}
		public virtual void ResetStyleDefaults() {
			PropertiesBar.ResetStyleDefaults();
		}
		protected internal virtual string GetPaintStyleName() {
			if(this.hasRibbon) return "Skin";
			string name = PaintStyleName;
			if(name == "Default") {
				switch(LookAndFeel.ActiveStyle) {
					case ActiveLookAndFeelStyle.WindowsXP : 
						if(LookAndFeel.ActiveLookAndFeel.UseDefaultLookAndFeel) 
							name = "Office2003";
						else
							name = "WindowsXP"; 
						break;
					case ActiveLookAndFeelStyle.Flat: name = "Office2000"; break;
					case ActiveLookAndFeelStyle.Skin : name = "Skin"; break;
					case ActiveLookAndFeelStyle.Style3D : name = "Office2000"; break;
					case ActiveLookAndFeelStyle.UltraFlat : name = "OfficeXP"; break;
					case ActiveLookAndFeelStyle.Office2003 : name = "Office2003"; break;
					default:
						name = "OfficeXP";
						break;
				}
			}
			BarManagerPaintStyle style = PaintStyles[name];
			if(style != null && !style.IsAllowUse) style = PaintStyles[style.AlternatePaintStyleName];
			if(style == null) return PaintStyles[0].Name;
			return style.Name;
		}
		protected virtual void UpdatePaintStyleIndex() {
			if(this.paintStyleIndex != -1) return;
			if(paintStyleIndex == -1) {
				paintStyleIndex = GetPaintStyleIndex();
			}
			if(paintStyleIndex == -1) paintStyleIndex = 0;
		}
		protected virtual int GetPaintStyleIndex() {
			string name = GetPaintStyleName();
			return PaintStyles.IndexOf(name);
		}
		protected virtual int PaintStyleIndex { 
			get {
				UpdatePaintStyleIndex();
				return paintStyleIndex;
			}
		}
		protected internal virtual void OnChanged() {
			if(this.lockUpdate != 0 || this.lockInitialize != 0) return;
			lock(this) {
				PaintStyle.DrawParameters.UpdateScheme(DesignMode);
				if(Changed != null) Changed(this, EventArgs.Empty);
				NotifyClients(false);
			}
			FireChanged();
		}
		protected void NotifyClients(bool disposed) {
			object[] keys = new object[this.clients.Keys.Count];
			this.clients.Keys.CopyTo(keys, 0);
			if(disposed) this.clients.Clear();
			foreach(IBarAndDockingControllerClient client in keys) {
				if(disposed)
					client.OnDisposed(this);
				else
					client.OnControllerChanged(this);
			}
		}
		protected virtual void OnLookAndFeelChanged(object sender, EventArgs e) {
			this.paintStyleIndex = -1;
			if(IsLoading) return;
			if(DesignMode && !IsLoading) {
				ResetStyleDefaults();
			}
			OnChanged();
		}
		protected void FireChanged() {
			if(Site == null || !DesignMode) return;
			IComponentChangeService srv = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) srv.OnComponentChanged(this, null, null, null);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual BarManagerPaintStyle PaintStyle { get { return PaintStyles[PaintStyleIndex]; } }
		protected virtual void InitBitmaps() {
			cursors = new ArrayList();
			cursors.Add(CreateCursorFromResources("DevExpress.XtraBars.Images.DragCursor.cur", typeof(BarAndDockingController).Assembly));
			cursors.Add(CreateCursorFromResources("DevExpress.XtraBars.Images.CopyCursor.cur", typeof(BarAndDockingController).Assembly));
			cursors.Add(CreateCursorFromResources("DevExpress.XtraBars.Images.NoDropCursor.cur", typeof(BarAndDockingController).Assembly));
			cursors.Add(CreateCursorFromResources("DevExpress.XtraBars.Images.EditSizingCursor.cur", typeof(BarAndDockingController).Assembly));
			bitmaps = new Hashtable();
			bitmaps.Add("MaxImage", GetEmbeddedBitmap("MaxImage"));
			bitmaps.Add("MinImage", GetEmbeddedBitmap("MinImage"));
			bitmaps.Add("RestoreImage", GetEmbeddedBitmap("RestoreImage"));
			bitmaps.Add("CloseImage", GetEmbeddedBitmap("CloseImage"));
			Bitmap bmp = GetEmbeddedBitmap("Office2003Expand");
			bmp.MakeTransparent();
			bitmaps.Add("Office2003ExpandButton", bmp);
		}
		protected Hashtable Bitmaps {
			get {
				if(bitmaps == null)
					InitBitmaps();
				return bitmaps;
			}
		}
		object lockThis = new object();
		Cursor CreateCursorFromResources(string name, System.Reflection.Assembly asm) {
			lock(lockThis) {
				System.IO.Stream stream = asm.GetManifestResourceStream(name);
				return new Cursor(stream);
			}
		}
		protected internal Bitmap GetEmbeddedBitmap(string bitmapName) {
			return ResourceImageHelper.CreateImageFromResources(string.Format("DevExpress.XtraBars.Images.{0}.bmp", bitmapName), typeof(BarAndDockingController).Assembly) as Bitmap;
		}
		protected internal Bitmap GetBitmap(string bitmapName) {
			return Bitmaps[bitmapName] as Bitmap;
		}
		protected virtual void DockingSettings_Changed(object sender, EventArgs e) {
			OnChanged();
		}
		bool IAppearanceOwner.IsLoading { get { return IsLoading; } }
		internal bool NotifyBarMouseActivateClients(BarManager barManager, ref Message msg) {
			IntPtr prevResult = msg.Result;
			object[] keys = new object[this.clients.Keys.Count];
			this.clients.Keys.CopyTo(keys, 0);
			foreach(IBarAndDockingControllerClient c in keys) {
				IBarMouseActivateClient client = c as IBarMouseActivateClient;
				if(client != null)
					client.OnBarMouseActivate(barManager, ref msg);
			}
			return msg.Result != prevResult;
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class BarManagerAppearances : IDisposable, IAppearanceOwner {
		AppearanceObject dock;
		StateAppearances barAppearance, mainMenuAppearance, statusBarAppearance;
		bool fontStored = false;
		MenuAppearance subMenu;
		Font itemsFont;
		int lockUpdate;
		BarAndDockingController controller;
		public BarManagerAppearances(BarAndDockingController controller) {
			this.itemsFont = AppearanceObject.DefaultMenuFont;
			this.dock = CreateAppearance();
			this.subMenu = CreateMenuAppearance();
			this.barAppearance = CreateStateAppearance();
			this.mainMenuAppearance = CreateStateAppearance();
			this.statusBarAppearance = CreateStateAppearance();
			this.lockUpdate = 0;
			this.controller = controller;
		}
		private StateAppearances CreateStateAppearance() {
			StateAppearances res = new StateAppearances(this);
			res.Changed += new EventHandler(OnAppearanceChanged);
			return res;
		}
		public virtual void Reset() {
			BeginUpdate();
			try {
				ResetItemsFont();
				BarAppearance.Reset();
				MainMenuAppearance.Reset();
				StatusBarAppearance.Reset();
				Dock.Reset();
				SubMenu.Reset();
			}
			finally {
				EndUpdate();
			}
		}
		bool IAppearanceOwner.IsLoading { get { return Controller == null || Controller.IsLoading; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BarAndDockingController Controller { get { return controller; } }
		public virtual void Dispose() {
			DestroyStateAoppearance(BarAppearance);
			DestroyStateAoppearance(MainMenuAppearance);
			DestroyStateAoppearance(StatusBarAppearance);
			DestroyAppearance(this.dock);
			DestroyMenuAppearance(this.subMenu);
		}
		private void DestroyStateAoppearance(StateAppearances app) {
			app.Changed -= new EventHandler(OnAppearanceChanged);
			app.Dispose();
		}
		public void BeginUpdate() {
			this.lockUpdate ++;
		}
		public void EndUpdate() {
			if(--this.lockUpdate == 0) {
				OnAppearanceChanged(this, EventArgs.Empty);
			}
		}
		internal bool ShouldSerializeItemsFont() { return ItemsFont != AppearanceObject.DefaultMenuFont && !ItemsFont.Equals(AppearanceObject.DefaultMenuFont); }
		internal void ResetItemsFont() { ItemsFont = AppearanceObject.DefaultMenuFont; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerAppearancesItemsFont"),
#endif
 Category("Appearance")]
		public virtual Font ItemsFont {
			get { return itemsFont; }
			set {
				if(value == null) return;
				if(ItemsFont == value) return;
				itemsFont = value;
				this.fontStored = ShouldSerializeItemsFont();
				Controller.OnChanged();
			}
		}
		internal void ResetTheme() {
			if(!this.fontStored) ResetItemsFont();
		}
		void ResetDock() { Dock.Reset(); }
		bool ShouldSerializeDock() { return Dock.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerAppearancesDock"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Dock { get { return dock; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerAppearancesBar"),
#endif
 Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AppearanceObject Bar { get { return BarAppearance.Normal; } }
		void ResetBarAppearance() { BarAppearance.Reset(); }
		bool ShouldSerializeBarAppearance() { return BarAppearance.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerAppearancesBarAppearance"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public StateAppearances BarAppearance { get { return barAppearance; } }
		void ResetMainMenuAppearance() { MainMenuAppearance.Reset(); }
		bool ShouldSerializeMainMenuAppearance() { return MainMenuAppearance.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerAppearancesMainMenuAppearance"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public StateAppearances MainMenuAppearance { get { return mainMenuAppearance; } }
		void ResetStatusBarAppearance() { StatusBarAppearance.Reset(); }
		bool ShouldSerializeStatusBarAppearance() { return StatusBarAppearance.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerAppearancesStatusBarAppearance"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public StateAppearances StatusBarAppearance { get { return statusBarAppearance; } }
		bool ShouldSerializeSubMenu() { return SubMenu.ShouldSerialize(); }
		void ResetSubMenu() { SubMenu.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerAppearancesSubMenu"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MenuAppearance SubMenu { get { return subMenu; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerAppearancesStatusBar"),
#endif
 Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AppearanceObject StatusBar { get { return StatusBarAppearance.Normal; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerAppearancesMainMenu"),
#endif
 Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AppearanceObject MainMenu { get { return MainMenuAppearance.Normal; } }
		protected virtual void DestroyAppearance(AppearanceObject appearance) {
			if(appearance == null) return;
			appearance.Changed -= new EventHandler(OnAppearanceChanged);
			appearance.Dispose();
		}
		protected virtual void DestroyMenuAppearance(MenuAppearance appearance) {
			if(appearance == null) return;
			appearance.Changed -= new EventHandler(OnAppearanceChanged);
			appearance.Dispose();
		}
		protected virtual AppearanceObject CreateAppearance() {
			AppearanceObject appearance = new AppearanceObject(this, true);
			appearance.Changed += new EventHandler(OnAppearanceChanged);
			return appearance;
		}
		protected virtual MenuAppearance CreateMenuAppearance() {
			MenuAppearance appearance = new MenuAppearance(this);
			appearance.Changed += new EventHandler(OnAppearanceChanged);
			return appearance;
		}
		protected virtual void OnAppearanceChanged(object sender, EventArgs e) {
			if(this.lockUpdate != 0 || Controller.IsLoading) return;
			Controller.OnChanged();
		}
	}
	public interface IBarAndDockingControllerClient {
		void OnControllerChanged(BarAndDockingController controller);
		void OnDisposed(BarAndDockingController controller);
	}
	internal interface IBarMouseActivateClient : IBarAndDockingControllerClient {
		void OnBarMouseActivate(BarManager barManager, ref Message msg);
	}
}
