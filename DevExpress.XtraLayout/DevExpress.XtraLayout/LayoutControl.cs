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
using System.Collections;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Controls;
using DevExpress.Utils;
using DXUtils = DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.LookAndFeel;
using DevExpress.XtraLayout.Printing;
using DevExpress.Accessibility;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout.Registrator;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.Helpers;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.Adapters;
using DevExpress.XtraLayout.Customization;
using Padding = DevExpress.XtraLayout.Utils.Padding;
using DevExpress.XtraLayout.Accessibility;
using DevExpress.XtraEditors.Container;
using System.Collections.Generic;
using DevExpress.Utils.About;
using System.ComponentModel.Design;
using System.Diagnostics;
using DevExpress.Skins;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Gesture;
using DevExpress.XtraLayout.Customization.UserCustomization;
using DevExpress.XtraLayout.Customization.Behaviours;
using DevExpress.Utils.Filtering;
namespace DevExpress.XtraLayout {
	[DXToolboxItem(true)]
	[DevExpress.XtraBars.Docking.ProhibitUsingAsDockingContainer]
	[Designer(LayoutControlConstants.LayoutControlDesignerName, typeof(System.ComponentModel.Design.IDesigner)),
	 ToolboxBitmap(typeof(LayoutControl), "Images.layoutControl256.bmp"),
	 Description("Provides advanced capabilities to create, customize and maintain a consistent layout of controls within a form."),
	 ToolboxTabName(AssemblyInfo.DXTabNavigation)
]
	public class LayoutControl : ContainerControl, ILayoutControlOwner, IDisposable, IToolTipControlClient, ISupportInitialize, IXtraSerializable, IXtraSerializableLayout, IExtenderProvider, ISupportLookAndFeel, ILayoutControl, IStyleController, ILayoutDesignerMethods, IPrintable, IBasePrintable, IControlIterator, IDragDropDispatcherClient, ITransparentBackgroundManager, ITransparentBackgroundManagerEx, IXtraResizableControl, IMouseWheelSupport, ISupportXtraSerializer, IGestureClient, ILayoutControlOwnerEx, ITemplateManagerImplementor, IDXMenuManagerProvider, IFilteringUIProvider {
		internal LayoutControlImplementor implementorCore = null;
		internal ILayoutControl implementor = null;
		LayoutControlPrinter printer;
		OptionsPrintControl optionsPrintCore;
		LayoutControlGroup rootGroupCore;
		event EventHandler propertiesChanged;
		event EventHandler changed;
		event CancelEventHandler changing;
		event LayoutGroupEventHandler closeButtonClick;
		public event EventHandler ItemSelectionChanged;
		public event EventHandler ItemAdded;
		public event EventHandler ItemRemoved;
		public event EventHandler DefaultLayoutLoading;
		public event EventHandler DefaultLayoutLoaded;
		public event EventHandler ShowCustomization;
		public event EventHandler HideCustomization;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'PopupMenuShowing' instead", false)]
		public event LayoutMenuEventHandler ShowContextMenu;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'LayoutTreeViewPopupMenuShowing' instead", false)]
		public event LayoutMenuEventHandler ShowLayoutTreeViewContextMenu;
		public event PopupMenuShowingEventHandler PopupMenuShowing;
		public event PopupMenuShowingEventHandler LayoutTreeViewPopupMenuShowing;
		protected internal event EventHandler Updated;
		bool ILayoutControlOwner.IsDesignMode { get { return DesignMode; } }
		void ILayoutControlOwner.RaiseOwnerEvent_ShowCustomization(object sender, EventArgs e) {
			if(ShowCustomization != null) ShowCustomization(sender, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_HideCustomization(object sender, EventArgs e) {
			if(HideCustomization != null) HideCustomization(sender, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_PropertiesChanged(object sender, EventArgs e) {
			if(propertiesChanged != null) propertiesChanged(sender, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_ItemSelectionChanged(object sender, EventArgs e) {
			if(ItemSelectionChanged != null) ItemSelectionChanged(sender, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_ItemRemoved(object sender, EventArgs e) {
			if(ItemRemoved != null) ItemRemoved(sender, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_ItemAdded(object sender, EventArgs e) {
			if(ItemAdded != null) ItemAdded(sender, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_Updated(object sender, EventArgs e) {
			if(Updated != null) Updated(sender, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_GroupExpandChanging(LayoutGroupCancelEventArgs e) {
			LayoutGroupCancelEventHandler handler = (LayoutGroupCancelEventHandler)this.Events[groupExpandChanging];
			if(handler != null) handler(this, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_GroupExpandChanged(LayoutGroupEventArgs e) {
			LayoutGroupEventHandler handler = (LayoutGroupEventHandler)this.Events[groupExpandChanged];
			if(handler != null) handler(this, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_ShowCustomizationMenu(PopupMenuShowingEventArgs e) {
			if(PopupMenuShowing != null)
				PopupMenuShowing(this, e);
#pragma warning disable 612 // Obsolete
#pragma warning disable 618 // Obsolete
			if(ShowContextMenu != null) {
				LayoutMenuEventArgs args = new LayoutMenuEventArgs(e.Menu, e.HitInfo, e.Allow);
				ShowContextMenu(this, args);
				e.Menu = args.Menu;
				e.Allow = args.Allow;
			}
#pragma warning restore 618 // Obsolete
#pragma warning restore 612 // Obsolete
		}
		void ILayoutControlOwner.RaiseOwnerEvent_ShowLayoutTreeViewContextMenu(PopupMenuShowingEventArgs e) {
			if(LayoutTreeViewPopupMenuShowing != null)
				LayoutTreeViewPopupMenuShowing(this, e);
#pragma warning disable 612 // Obsolete
#pragma warning disable 618 // Obsolete
			if(ShowLayoutTreeViewContextMenu != null) {
				LayoutMenuEventArgs args = new LayoutMenuEventArgs(e.Menu, e.HitInfo, e.Allow);
				ShowLayoutTreeViewContextMenu(this, args);
				e.Menu = args.Menu;
				e.Allow = args.Allow;
			}
#pragma warning restore 618 // Obsolete
#pragma warning restore 612 // Obsolete
		}
		void ILayoutControlOwner.RaiseOwnerEvent_LayoutUpdate(EventArgs e) {
			RaiseLayoutUpdate(e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_Changed(object sender, EventArgs e) {
			RaiseChanged(e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_CloseButtonClick(object sender, LayoutGroupEventArgs e) {
			if(closeButtonClick != null) closeButtonClick(this, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_Changing(object sender, CancelEventArgs e) {
			if(changing != null && DenyFireChangingChangedEvents == 0) changing(this, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_UniqueNameRequest(object sender, UniqueNameRequestArgs e) {
			UniqueNameRequestHandler handler = (UniqueNameRequestHandler)this.Events[uniqueNameRequest];
			if(handler != null) handler(sender, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_LayoutUpgrade(object sender, LayoutUpgradeEventArgs e) {
			LayoutUpgradeEventHandler handler = (LayoutUpgradeEventHandler)this.Events[layoutUpgrade];
			if(handler != null) handler(sender, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_BeforeLoadLayout(object sender, LayoutAllowEventArgs e) {
			LayoutAllowEventHandler handler = (LayoutAllowEventHandler)this.Events[beforeLoadLayoutCore];
			if(handler != null) handler(sender, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_DefaultLayoutLoading(object sender, EventArgs e) {
			if(DefaultLayoutLoading != null) DefaultLayoutLoading(this, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_DefaultLayoutLoaded(object sender, EventArgs e) {
			if(DefaultLayoutLoaded != null) DefaultLayoutLoaded(this, e);
		}
		ISupportLookAndFeel ILayoutControlOwner.GetISupportLookAndFeel() { return this; }
		IStyleController ILayoutControlOwner.GetIStyleController() { return this; }
		LayoutControlImplementor ISupportImplementor.Implementor { get { return implementorCore; } }
		bool ILayoutControl.ShouldArrangeTextSize {
			get { return implementor.ShouldArrangeTextSize; }
			set { implementor.ShouldArrangeTextSize = value; }
		}
		bool ILayoutControl.ShouldUpdateConstraints {
			get { return implementor.ShouldUpdateConstraints; }
			set { implementor.ShouldUpdateConstraints = value; }
		}
		bool ILayoutControl.ShouldResize {
			get { return implementor.ShouldResize; }
			set { implementor.ShouldResize = value; }
		}
		bool ILayoutControl.ShouldUpdateLookAndFeel {
			get { return implementor.ShouldUpdateLookAndFeel; }
			set { implementor.ShouldUpdateLookAndFeel = value; }
		}
		bool ILayoutControl.ShouldUpdateControlsLookAndFeel {
			get { return implementor.ShouldUpdateControlsLookAndFeel; }
			set { implementor.ShouldUpdateControlsLookAndFeel = value; }
		}
		bool ILayoutControl.ShouldUpdateControls {
			get { return implementor.ShouldUpdateControls; }
			set { implementor.ShouldUpdateControls = value; }
		}
		bool ILayoutControl.LockUpdateOnChangeUICuesRequest {
			get { return implementor.LockUpdateOnChangeUICuesRequest; }
			set { implementor.LockUpdateOnChangeUICuesRequest = value; }
		}
		int ILayoutControl.DelayPainting {
			get { return implementor.DelayPainting; }
			set { implementor.DelayPainting = value; }
		}
		static readonly object layoutUpgrade = new object();
		private static readonly object uniqueNameRequest = new object();
		private static readonly object groupExpandChanging = new object();
		private static readonly object groupExpandChanged = new object();
		private static readonly object layoutUpdate = new object();
		private static readonly object beforeLoadLayoutCore = new object();
		internal static bool useSimplePainting;
		static LayoutControl() {
			useSimplePainting = Environment.OSVersion.Version.Major < 5;
			sizeableChanged = new object();
		}
		public static void About() {
		}
		protected void CreateLayoutControl(LayoutControlRoles role, bool fAllowUseSplitters, bool fAllowUseTabbedGroup, bool createFast) {
			CreateILayoutControlImplementor();
			implementor.ControlRole = role;
			implementorCore.AllowUseTabbedGroups = fAllowUseTabbedGroup;
			implementorCore.AllowUseSplitters = fAllowUseSplitters;
			implementorCore.AllowCreateRootElement = !createFast;
			InitCore();
		}
		internal LayoutControl(LayoutControlRoles role) {
			CreateLayoutControl(role, true, true, false);
		}
		public LayoutControl() {
			bool createFast = false;
			StackTrace st = new StackTrace(System.Threading.Thread.CurrentThread, false);
			if(st != null) {
				StackFrame sf = st.GetFrame(1);
				if(sf != null && sf.GetMethod().Name == "InitializeComponent") createFast = true;
			}
			CreateLayoutControl(LayoutControlRoles.MainControl, true, true, createFast);
			oldForeColor = ForeColor;
		}
		public LayoutControl(bool fAllowUseSplitters, bool fAllowUseTabbedGroup) {
			CreateLayoutControl(LayoutControlRoles.MainControl, fAllowUseSplitters, fAllowUseTabbedGroup, false);
		}
		public LayoutControl(bool createFast) {
			CreateLayoutControl(LayoutControlRoles.MainControl, true, true, createFast);
		}
		bool useLocalBindingContextCore = false;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool UseLocalBindingContext {
			get { return useLocalBindingContextCore; }
			set { useLocalBindingContextCore = value; }
		}
		public override BindingContext BindingContext {
			get {
				if(!UseLocalBindingContext) {
					try {
						Form form = FindForm();
						if(form != null)
							return form.BindingContext;
						else
							if(DesignMode) return base.BindingContext;
							else return null;
					} catch {
					}
				}
				return base.BindingContext;
			}
			set {
				OnBindingContextChanging();
				base.BindingContext = value;
				OnBindingContextChanged();
			}
		}
		protected virtual void OnBindingContextChanging() { }
		protected virtual void OnBindingContextChanged() {
			foreach(IBindableComponent ibc in Items) ibc.BindingContext = BindingContext;
		}
		protected override void OnForeColorChanged(EventArgs e) {
			if(!IsInitialized) return;
			base.OnForeColorChanged(e);
			AppearanceController.SetDefaultAppearanceDirty(Root);
			Invalidate();
		}
		public void BestFit() {
			implementorCore.BestFit();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Size AutoScrollPosition { get { return Size.Empty; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Size AutoScrollMinSize { get { return Size.Empty; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Size AutoScrollMargin { get { return Size.Empty; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public IComparer HiddenItemsSortComparer {
			get { return implementor.HiddenItemsSortComparer; }
			set { implementor.HiddenItemsSortComparer = value; }
		}
		LayoutControlRoles ILayoutControl.ControlRole {
			get { return implementor.ControlRole; }
			set { implementor.ControlRole = value; }
		}
		EditorContainer ILayoutControl.GetEditorContainer() {
			return implementor.GetEditorContainer();
		}
		bool ILayoutControl.CheckIfControlIsInLayout(Control control) {
			return implementor.CheckIfControlIsInLayout(control);
		}
		bool ILayoutControl.ExceptionsThrown {
			get { return implementor.ExceptionsThrown; }
			set { implementor.ExceptionsThrown = value; }
		}
		protected void InitCore() {
			BeginInit();
			this.SuspendLayout();
			implementorCore.InitializeLookAndFeelCore(this);
			implementorCore.InitializeAppearanceCore();
			implementorCore.InitializePaintStylesCore();
			InitializeToolTipController();
			SetScrollState(1, false);
			Initialize();
			implementorCore.InitializeFixedHiddenItems();
			this.optionsPrintCore = CreateOptionsPrint();
			this.printer = CreatePrinter();
			AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			EndInit();
		}
		protected virtual OptionsPrintControl CreateOptionsPrint() {
			return new OptionsPrintControl(this);											  
		}
		protected virtual void InitializeToolTipController() {
			ToolTipController.DefaultController.AddClientControl(this);
		}
		protected virtual void DisposeToolTipController() {
			ToolTipController = null;
			ToolTipController.DefaultController.RemoveClientControl(this);
		}
		LongPressControl ILayoutControl.LongPressControl {
			get { return implementor.LongPressControl; }
		}
		private ToolTipController toolTipController;
		[DefaultValue((string)null), 
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlToolTipController")
#else
	Description("")
#endif
]
		public ToolTipController ToolTipController {
			get {
				return this.toolTipController;
			}
			set {
				if(this.ToolTipController != value) {
					if(this.ToolTipController != null) {
						this.ToolTipController.RemoveClientControl(this);
					}
					this.toolTipController = value;
					if(this.ToolTipController != null) {
						ToolTipController.DefaultController.RemoveClientControl(this);
						this.ToolTipController.AddClientControl(this);
					}
					else {
						ToolTipController.DefaultController.AddClientControl(this);
					}
				}
			}
		}
		#region IToolTipControlClient
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			if(implementor.DesignMode || implementor.EnableCustomizationMode) return null;
			else return GetToolTipObjectInfo(point);
		}
		bool IToolTipControlClient.ShowToolTips { get { return true; } }
		#endregion
		protected virtual ToolTipControlInfo GetToolTipObjectInfo(Point pt) {
			ToolTipControlInfo toolTipInfo = null;
			BaseLayoutItemHitInfo hi = CalcHitInfo(pt);
			if(hi != null && hi.Item != null && hi.Item.RequiredItemVisibility) {
				toolTipInfo = implementorCore.GetItemToolTipInfo(pt, hi);
				if(toolTipInfo == null && hi.Item is LayoutGroup && (hi.Item as LayoutGroup).ButtonsPanel != null) {
					toolTipInfo = (hi.Item as LayoutGroup).ButtonsPanel.GetObjectInfo(pt);
				}
			}
			return toolTipInfo;
		}
		protected virtual void InitializeAsDragDropDispatcherClient() {
			if(implementorCore.controlsRole == LayoutControlRoles.MainControl && DragDropDispatcherClientHelper.ClientDescriptor == null) {
				DragDropDispatcherClientHelper.Initialize();
			}
		}
		protected virtual void CreateILayoutControlImplementor() {
			implementorCore = CreateILayoutControlImplementorCore();
			implementor = implementorCore as ILayoutControl;
		}
		protected virtual LayoutControlImplementor CreateILayoutControlImplementorCore() {
			return new LayoutControlImplementor(this);
		}
		bool ILayoutControl.IsHidden(BaseLayoutItem item) {
			return implementor.IsHidden(item);
		}
		public void SetDefaultLayout() {
			implementorCore.defaultLayout = new MemoryStream();
			SaveLayoutToStream(implementorCore.defaultLayout);
		}
		public virtual void LayoutChanged() {
			implementorCore.LayoutChanged();
		}
		public void RestoreDefaultLayout() {
			implementor.RestoreDefaultLayout();
		}
		bool IExtenderProvider.CanExtend(object extendee) {
			Control control = extendee as Control;
			if(control == null) return false;
			if(Controls.Contains(control)) {
				return !(Controls.IndexOf(control) < 2);
			}
			return false;
		}
		protected void SetRoot(LayoutControlGroup group) { implementorCore.SetRoot(group); }
		bool ILayoutControl.SelectedChangedFlag {
			get { return implementor.SelectedChangedFlag; }
			set { implementor.SelectedChangedFlag = value; }
		}
		int ILayoutControl.SelectedChangedCount {
			get { return implementor.SelectedChangedCount; }
			set { implementor.SelectedChangedCount = value; }
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlAutoScroll"),
#endif
 RefreshProperties(RefreshProperties.All), DefaultValue(true)]
		public override bool AutoScroll {
			get { return implementorCore.isAutoScroll; }
			set { base.AutoScroll = false; implementorCore.isAutoScroll = value; OnAutoScrollChanged(); }
		}
		[ DefaultValue(""), Category("Data")]
		public virtual string LayoutVersion {
			get { return implementorCore.layoutVersion; }
			set {
				if(value == null) value = "";
				implementorCore.layoutVersion = value;
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlIsModified"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public bool IsModified { get { return implementorCore.isModifiedCore; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public bool IsLayoutModified { get { return implementorCore.isLayoutModifiedCore; } }
		List<IComponent> ILayoutControl.Components { get { return implementor.Components; } }
		[Browsable(false)]
		public UndoManager UndoManager { get { return implementor.UndoManager; } }
		void ILayoutControl.RebuildCustomization() {
			implementor.RebuildCustomization();
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlOptionsFocus"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public OptionsFocus OptionsFocus {
			get { return implementor.OptionsFocus; }
		}
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LayoutSerializationOptions OptionsSerialization {
			get { return implementor.OptionsSerialization; }
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlOptionsPrint"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OptionsPrintControl OptionsPrint {
			get { return optionsPrintCore; }
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlOptionsCustomizationForm"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OptionsCustomizationForm OptionsCustomizationForm {
			get { return implementor.OptionsCustomizationForm; }
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlOptionsView"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OptionsView OptionsView {
			get { return implementor.OptionsView; }
		}
		protected virtual FocusHelperBase CreateFocusHelper() { return new FocusHelperBase(this); }
		[Browsable(false)]
		public FocusHelperBase FocusHelper {
			get { return implementor.FocusHelper; }
			set { implementor.FocusHelper = value; }
		}
		[Browsable(false)]
		public AppearanceController AppearanceController {
			get { return implementor.AppearanceController; }
			set { implementor.AppearanceController = value; }
		}
		[Browsable(false)]
		public EnabledStateController EnabledStateController {
			get { return implementor.EnabledStateController; }
			set { implementor.EnabledStateController = value; }
		}
		RightButtonMenuManager ILayoutControlOwner.CreateRightButtonMenuManager() { return CreateRightButtonMenuManager(); }
		protected virtual RightButtonMenuManager CreateRightButtonMenuManager() { return new RightButtonMenuManager(this); }
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("LayoutControlCustomizationMenuManager")]
#endif
		[Browsable(false)]
		public RightButtonMenuManager CustomizationMenuManager {
			get { return implementor.CustomizationMenuManager; }
		}
		protected override void OnEnabledChanged(EventArgs e) {
			ResetEnabledStateController();
			BeginUpdate();
			base.OnEnabledChanged(e);
			implementor.ShouldUpdateLookAndFeel = true;
			EndUpdate();
		}
		protected void ResetEnabledStateController() {
			if(IsInitialized) { EnabledStateController.SetItemEnabledStateDirty(Root); ((ILayoutControl)this).ShouldUpdateControlsLookAndFeel = true; }
		}
		protected override AccessibleObject CreateAccessibilityInstance() {
			if(DXAccessible == null) return base.CreateAccessibilityInstance();
			return DXAccessible.Accessible;
		}
		protected internal virtual LayoutControlAccessible CreateAccessibleInstance() {
			return new LayoutControlAccessible(this);
		}
		protected internal virtual BaseAccessible DXAccessible {
			get { return implementorCore.DXAccessible; }
		}
		protected internal LayoutControl Clone() {
			LayoutControl control = new LayoutControl(LayoutControlRoles.ClonedControl);
			control.BeginInit();
			LayoutControlGroup newRoot = (LayoutControlGroup)Root.Clone(null, control);
			control.SetRoot(newRoot);
			control.Size = Size;
			control.Location = Location;
			control.AutoScroll = AutoScroll;
			control.HiddenItems.Clear();
			foreach(BaseLayoutItem item in HiddenItems) {
				BaseLayoutItem clonedItem = item.Clone(null, control);
				control.HiddenItems.Add(clonedItem);
			}
			control.EndInit();
			return control;
		}
		protected void OnAutoScrollChanged() {
			((ILayoutControl)this).Invalidate();
		}
		Color ITransparentBackgroundManagerEx.GetEmptyBackColor(Control childControl) {
			if(implementorCore.DisposingFlagInternal) return Color.Empty;
			return implementorCore.GetEmptyBackColor(childControl, BackColor);
		}
		Color ITransparentBackgroundManager.GetForeColor(object childObject) {
			if(implementorCore.DisposingFlagInternal) return Color.Empty;
			return implementorCore.GetForeColor(childObject as BaseLayoutItem, ForeColor);
		}
		Color ITransparentBackgroundManager.GetForeColor(Control childControl) {
			if(implementorCore.DisposingFlagInternal) return Color.Empty;
			return implementorCore.GetForeColor(childControl, ForeColor);
		}
		AppearanceObject IStyleController.Appearance { get { return StyleController == null ? Appearance.Control : StyleController.Appearance; } }
		AppearanceObject IStyleController.AppearanceDisabled { get { return StyleController == null ? Appearance.ControlDisabled : StyleController.AppearanceDisabled; } }
		AppearanceObject IStyleController.AppearanceReadOnly { get { return StyleController == null ? Appearance.ControlReadOnly : StyleController.AppearanceReadOnly; } }
		AppearanceObject IStyleController.AppearanceFocused { get { return StyleController == null ? Appearance.ControlFocused : StyleController.AppearanceFocused; } }
		AppearanceObject IStyleController.AppearanceDropDown { get { return StyleController == null ? Appearance.ControlDropDown : StyleController.AppearanceDropDown; } }
		AppearanceObject IStyleController.AppearanceDropDownHeader { get { return StyleController == null ? Appearance.ControlDropDownHeader : StyleController.AppearanceDropDownHeader; } }
		UserLookAndFeel IStyleController.LookAndFeel { get { return null; } }
		BorderStyles IStyleController.BorderStyle { get { return StyleController == null ? BorderStyles.Default : StyleController.BorderStyle; } }
		BorderStyles IStyleController.ButtonsStyle { get { return StyleController == null ? BorderStyles.Default : StyleController.ButtonsStyle; } }
		PopupBorderStyles IStyleController.PopupBorderStyle { get { return StyleController == null ? PopupBorderStyles.Default : StyleController.PopupBorderStyle; } }
		event EventHandler IStyleController.PropertiesChanged {
			add { propertiesChanged += value; }
			remove { propertiesChanged -= value; }
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("LayoutControlChanged")]
#endif
		public event EventHandler Changed {
			add { changed += value; }
			remove { changed -= value; }
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("LayoutControlChanging")]
#endif
		public event CancelEventHandler Changing {
			add { changing += value; }
			remove { changing -= value; }
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("LayoutControlTabPageCloseButtonClick")]
#endif
		public event LayoutGroupEventHandler TabPageCloseButtonClick {
			add { closeButtonClick += value; }
			remove { closeButtonClick -= value; }
		}
		[Browsable(false)]
		int ILayoutControl.UpdatedCount {
			get { return implementor.UpdatedCount; }
			set { implementor.UpdatedCount = value; }
		}
		[Browsable(false)]
		public bool IsInitialized {
			get { return implementorCore.initializedCount == 0 && (this as ILayoutControl).InitializationFinished; }
		}
		void ILayoutControl.FireCloseButtonClick(LayoutGroup component) { implementor.FireCloseButtonClick(component); }
		void ILayoutControl.FireChanged(IComponent component) { implementor.FireChanged(component); }
		bool ILayoutControl.FireChanging(IComponent component) { return implementor.FireChanging(component); }
		protected override void OnControlAdded(ControlEventArgs e) {
			OnChildControlAdded(e.Control);
			base.OnControlAdded(e);
		}
		protected override void OnControlRemoved(ControlEventArgs e) {
			base.OnControlRemoved(e);
			OnChildControlRemoved(e.Control);
		}
		public LayoutControlItem GetItemByControl(Control control) {
			return implementor.GetItemByControl(control);
		}
		bool IsAllowedDialogKey(Keys keyData) { return true; }
		bool ILayoutControl.ShowKeyboardCues {
			get {
				try {
					((ILayoutControl)this).LockUpdateOnChangeUICuesRequest = true;
					bool allow = true;
					Control currentControl = this;
					while(currentControl != null && allow) {
						allow = currentControl.IsHandleCreated;
						currentControl = currentControl.Parent;
					}
					return allow && ShowKeyboardCues;
				} finally {
					((ILayoutControl)this).LockUpdateOnChangeUICuesRequest = false;
				}
			}
		}
		protected override void OnChangeUICues(UICuesEventArgs e) {
			((ILayoutControl)this).OnChangeUICues();
			base.OnChangeUICues(e);
		}
		void ILayoutControl.OnChangeUICues() {
			if(implementor != null) implementor.OnChangeUICues();
		}
		protected override void Select(bool directed, bool forward) {
			if(!OptionsFocus.EnableAutoTabOrder) {
				base.Select(directed, forward);
				return;
			}
			bool correctParentActiveControl = true;
			Component selectedComponentLocal = FocusHelper.SelectedComponent;
			FocusHelper.MoveBack = !forward;
			if(Parent != null) {
				IContainerControl container = Parent.GetContainerControl();
				if(container != null) {
					container.ActiveControl = this;
					correctParentActiveControl = (container.ActiveControl == this);
				}
			}
			if(correctParentActiveControl) {
				if(!forward) {
					if(!FocusHelper.IsValidSelectCandidate(selectedComponentLocal))
						FocusHelper.FocusLast();
					else
						FocusHelper.FocusElement(selectedComponentLocal, true);
				} else {
					if(!FocusHelper.IsValidSelectCandidate(selectedComponentLocal))
						FocusHelper.FocusFirst();
					else
						FocusHelper.FocusElement(selectedComponentLocal, true);
				}
				selectedComponentLocal = FocusHelper.SelectedComponent;
				if(!selectInVB && (selectedComponentLocal == null || ((ILayoutDesignerMethods)this).IsInternalControl(this.Controls[Controls.Count - 1]))) {
					selectInVB = true;
					Form f = FindForm();
					if(f != null && f.ParentForm != null) return;
					if(IsHandleCreated) { 
						SelectDelegate del = Select;
						BeginInvoke(del, new object[] { directed, forward });
					}
				}
			}
		}
		bool selectInVB = false;
		delegate void SelectDelegate(bool directed, bool forward);
		protected override bool ProcessMnemonic(char charCode) {
			if(FocusHelper.ProcessMnemonic(charCode)) {
				return true;
			}
			return base.ProcessMnemonic(charCode);
		}
		public void RenameSelectedItem() {
			SelectionHelper sh = new SelectionHelper();
			List<BaseLayoutItem> list = sh.GetItemsList(Root);
			if(list.Count == 1) {
				BaseLayoutItem selectedItem = list[0] as BaseLayoutItem;
				if(selectedItem.TextVisible) {
					Rectangle rect = Rectangle.Empty;
					if(selectedItem is LayoutControlItem) {
						rect = selectedItem.ViewInfo.TextAreaRelativeToControl;
						rect.Location = selectedItem.Owner.Control.PointToScreen(rect.Location);
					}
					LayoutControlGroup group = selectedItem as LayoutControlGroup;
					if(group != null) {
						if(group.ParentTabbedGroup == null) {
							rect = group.ViewInfo.BorderInfo.TextBounds;
							rect.Location = selectedItem.Owner.Control.PointToScreen(rect.Location);
						} else {
							int pageIndex = group.ParentTabbedGroup.VisibleTabPages.IndexOf(group);
							rect = group.ParentTabbedGroup.ViewInfo.GetScreenTabCaptionRect(pageIndex);
							rect.Location = selectedItem.Owner.Control.PointToScreen(rect.Location);
						}
					}
					implementorCore.RenameItemManager.Rename(selectedItem, rect);
				}
			}
		}
		public void HideSelectedItems() {
			implementor.HideSelectedItems();
		}
		public void SelectParentGroup() {
			implementor.SelectParentGroup();
		}
		protected internal static bool AllowHandleDeleteKey(ILayoutControl lc) {
			return lc.OptionsCustomizationForm.AllowHandleDeleteKey && !lc.DesignMode && ((ILayoutControl)lc).EnableCustomizationMode;
		}
		private void ProcessLayoutKeyDown(Keys keyCode) {
			switch(keyCode) {
				case Keys.F2:
					RenameSelectedItem();
					break;
				case Keys.Delete:
					if(AllowHandleDeleteKey(this)) HideSelectedItems();
					break;
				case Keys.Escape:
					SelectParentGroup();
					break;
			}
		}
		protected override bool ProcessKeyMessage(ref Message m) {
			bool result = false;
			try {
				if(((ILayoutControl)this).EnableCustomizationMode) {
					if(m.Msg == 0x100) {
						ProcessLayoutKeyDown((Keys)m.WParam);
					}
				}
			} finally {
				result = base.ProcessKeyMessage(ref m);
			}
			return result;
		}
		protected override bool ProcessKeyPreview(ref Message m) {
			bool result = false;
			try {
				if(((ILayoutControl)this).EnableCustomizationMode) {
					if(m.Msg == 0x100) {
						ProcessLayoutKeyDown((Keys)m.WParam);
					}
				}
			} finally {
				result = base.ProcessKeyPreview(ref m);
			}
			return result;
		}
		protected bool ProcessDialogKey_old(Keys keyData) {
			if(!FocusHelper.ProcessDialogKey(keyData)) {
				ProcessCustomizationFormEvents(keyData);
				if(this.ContainsFocus && IsAllowedDialogKey(keyData)) return base.ProcessDialogKey(keyData);
				else
					return false;
			} else return true;
		}
		private void ProcessCustomizationFormEvents(Keys keyData) {
			if((this as ILayoutControl).ControlRole == LayoutControlRoles.CustomizationFormControl) {
				UserCustomizationForm form = Parent as UserCustomizationForm;
				KeyEventArgs e = new KeyEventArgs(keyData);
				if(form != null) form.OwnerControl.ActiveHandler.OnKeyDown(this, e);
				else ActiveHandler.OnKeyDown(this, e);
			}
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if(FocusHelper.SelectedComponent == null) return base.ProcessDialogKey(keyData);
			if(!FocusHelper.ProcessDialogKey(keyData)) {
				ProcessCustomizationFormEvents(keyData);
				if(FocusHelper.IsSingleFocusManagerOnTheForm(Parent) && (keyData & Keys.KeyCode) == Keys.Tab && OptionsFocus.EnableAutoTabOrder) return true;
				else return base.ProcessDialogKey(keyData);
			} else return true;
		}
		public LayoutControlItem GetItemByControl(Control control, LayoutControlGroup group) {
			return implementorCore.GetItemByControl(control, group);
		}
		void ResetAppearance() { Appearance.Reset(); }
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlAppearance"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LayoutAppearanceCollection Appearance { get { return implementorCore.appearanceCore; } }
		LayoutPaintStyle ILayoutControl.PaintStyle { get { return implementor.PaintStyle; } }
		[Browsable(false)]
		public virtual LayoutPaintStyleCollection PaintStyles { get { return implementorCore.paintStylesCore; } }
		protected internal virtual LayoutPaintStyle ActiveStyle {
			get { return implementorCore.ActiveStyle; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public ConstraintsManager ConstraintsManager {
			get {
				return implementor.ConstraintsManager;
			}
		}
		protected internal void ResetRootGroupResizer() {
			implementorCore.RootGroup.ResetResizer();
			implementorCore.RootGroup.Resizer.UpdateConstraints();
		}
		protected internal void ResetAllViewInfo() {
			implementorCore.ResetAllViewInfo();
		}
		protected internal void ResetAllViewInfo(LayoutGroup group) {
			implementorCore.ResetAllViewInfo(group);
		}
		UserLookAndFeel ISupportLookAndFeel.LookAndFeel { get { return LookAndFeel; } }
		bool ISupportLookAndFeel.IgnoreChildren { get { return LookAndFeel.ParentLookAndFeel == null; } }
		bool ShouldSerializeLookAndFeel() { return StyleController == null && LookAndFeel != null && LookAndFeel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlLookAndFeel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public virtual SerializeableUserLookAndFeel LookAndFeel {
			get { return implementor.LookAndFeel; }
		}
		LayoutControlDefaultsPropertyBag ILayoutControl.DefaultValues {
			get { return implementor.DefaultValues; }
			set { implementor.DefaultValues = value; }
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlOptionsItemText"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OptionsItemText OptionsItemText {
			get { return implementor.OptionsItemText; }
		}
		protected void AlignTextWidth() {
			implementorCore.AlignTextWidth();
		}
		public HitInfo.BaseLayoutItemHitInfo CalcHitInfo(Point hitPoint) {
			return Root != null ? Root.CalcHitInfo(hitPoint, false) : null;
		}
		protected void CheckForSameNames() {
			Hashtable htable = new Hashtable();
			foreach(BaseLayoutItem item in Items) {
				if(!htable.Contains(item.Name)) htable.Add(item.Name, item);
				else {
					item.Name = item.GetDefaultText();
				}
			}
		}
		[XtraSerializableProperty(false, true, false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public ReadOnlyItemCollection Items {
			get { return implementor.Items; }
			set { implementor.Items = value; }
		}
		bool ILayoutControl.IsDeserializing {
			get { return implementor.IsDeserializing; }
			set { implementor.IsDeserializing = value; }
		}
		bool ILayoutControl.IsSerializing {
			get { return implementor.IsSerializing; }
		}
		internal object XtraFindItemsItem(XtraItemEventArgs e) {
			return implementorCore.XtraFindItemsItem(e);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		Dictionary<Control, LayoutControlItem> ILayoutControl.ControlsAndItems {
			get { return implementor.ControlsAndItems; }
			set { implementor.ControlsAndItems = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		Dictionary<string, BaseLayoutItem> ILayoutControl.ItemsAndNames {
			get { return implementor.ItemsAndNames; }
			set { implementor.ItemsAndNames = value; }
		}
		public static string GetControlName(Control control) {
			return LayoutControlImplementor.GetControlName(control);
		}
		public Control GetControlByName(String name) { return ((ISupportImplementor)this).Implementor.GetControlByName(name); }
		protected internal virtual void RaiseLayoutUpgrade(LayoutUpgradeEventArgs e) {
			LayoutUpgradeEventHandler handler = (LayoutUpgradeEventHandler)this.Events[layoutUpgrade];
			if(handler != null) handler(this, e);
		}
		string IXtraSerializableLayout.LayoutVersion { get { return LayoutVersion; } }
		void IXtraSerializable.OnStartSerializing() {
			implementorCore.OnStartSerializing();
		}
		void IXtraSerializable.OnEndSerializing() {
			implementorCore.OnEndSerializing();
		}
		void IXtraSerializable.OnStartDeserializing(DevExpress.Utils.LayoutAllowEventArgs e) {
			implementorCore.OnStartDeserializing(e);
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			implementorCore.OnEndDeserializing(restoredVersion);
		}
#if DEBUG
		[Browsable(false)]
		public void DiagnosticRestoreLayoutFromXml(string path) {
			implementorCore.isDiagnosticsDeserializing = true;
			new XmlXtraSerializer().DeserializeObject(this, path, this.GetType().Name);
			implementorCore.isDiagnosticsDeserializing = false;
		}
		[Browsable(false)]
		public void DiagnosticRestoreLayoutFromXml(Stream stream) {
			implementorCore.isDiagnosticsDeserializing = true;
			new XmlXtraSerializer().DeserializeObject(this, stream, this.GetType().Name);
			implementorCore.isDiagnosticsDeserializing = false;
		}
#endif
		public virtual void SaveLayoutToXml(string xmlFile) {
			implementor.SaveLayoutToXml(xmlFile);
		}
		public virtual void RestoreLayoutFromXml(string xmlFile) {
			implementor.RestoreLayoutFromXml(xmlFile);
		}
		public virtual void SaveLayoutToRegistry(string path) {
			implementor.SaveLayoutToRegistry(path);
		}
		public virtual void RestoreLayoutFromRegistry(string path) {
			implementor.RestoreLayoutFromRegistry(path);
		}
		public virtual void SaveLayoutToStream(Stream stream) {
			implementor.SaveLayoutToStream(stream);
		}
		public virtual void RestoreLayoutFromStream(Stream stream) {
			implementor.RestoreLayoutFromStream(stream);
		}
		protected void SetControlsLookAndFeel(Control control, IStyleController styleController) {
			implementorCore.SetControlsLookAndFeel(control, styleController);
		}
		protected void OnChildControlRemoved(Control control) {
			SetControlsLookAndFeel(control, null);
		}
		protected void OnChildControlAdded(Control control) {
			if(IsUpdateLocked) { ((ILayoutControl)this).ShouldUpdateControlsLookAndFeel = true; return; }
			SetControlsLookAndFeel(control, this);
		}
		protected void UpdateControlsLookAndFeel() {
			((ILayoutDesignerMethods)this).BeginChangeUpdate();
			try {
				for(int i = Controls.Count - 1; i >= 0; i--) {
					OnChildControlAdded(Controls[i]);
				}
			} finally {
				AppearanceController.SetDefaultAppearanceDirty(Root);
				RaisePropertiesChanged();
				((ILayoutDesignerMethods)this).EndChangeUpdate();
			}
		}
		protected void RaisePropertiesChanged() {
			if(propertiesChanged != null) propertiesChanged(this, EventArgs.Empty);
		}
		protected void UpdateCustomizationVisibility() {
			if(((ILayoutControl)this).EnableCustomizationMode && ((ILayoutControl)this).EnableCustomizationForm)
				if(CustomizationForm != null) CustomizationForm.Visible = Visible;
		}
		protected override void OnParentVisibleChanged(EventArgs e) {
			base.OnParentVisibleChanged(e);
			UpdateCustomizationVisibility();
		}
		Rectangle? lastVisibleBounds = null;
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			InvalidateAdornerWindowHandler();
			try {
				DenyFireChangingChangedEvents++;
				if(implementorCore.DisposingFlagInternal) return;
				if(lastVisibleBounds != null && lastVisibleBounds.Value == ClientRectangle && Scroller.VScrollVisible == VerticalScroll.Visible && Scroller.HScrollVisible == HorizontalScroll.Visible) {
					UpdateCustomizationVisibility();
					return;
				}
				if(Visible) {
					this.lastVisibleBounds = ClientRectangle;
					ProcessSizeChanged(true);
				}
				UpdateCustomizationVisibility();
			} finally {
				DenyFireChangingChangedEvents--;
			}
		}
		void ILayoutControl.UpdateChildControlsLookAndFeel() {
			implementorCore.OnLookAndFeelStyleChanged(null, EventArgs.Empty);
		}
		void DrawBorder(PaintEventArgs e, GraphicsCache cache) {
			return;
		}
		protected bool EnterSizeMove() {
			return ((ILayoutControl)this).FireChanging(this);
		}
		protected void ExitSizeMove() {
			((ILayoutControl)this).FireChanged(this);
		}
		protected override void WndProc(ref System.Windows.Forms.Message m) {
			int num1 = m.Msg;
			switch(num1) {
				case 0x231: {
						EnterSizeMove();
						DefWndProc(ref m);
						return;
					}
				case 0x232: {
						ExitSizeMove();
						DefWndProc(ref m);
						return;
					}
			}
			if(GestureHelper.WndProc(ref m))
				return;
			base.WndProc(ref m);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		void DrawFocusRect(GraphicsCache cache) {
			Rectangle focusRect = Rectangle.Empty;
			if(FocusHelper.SelectedComponent != null) {
				TabbedGroup tg = FocusHelper.SelectedComponent as TabbedGroup;
				LayoutGroup lg = FocusHelper.SelectedComponent as LayoutGroup;
				if(tg != null) {
					if(!OptionsFocus.AllowFocusTabbedGroups) return;
					if(tg.SelectedTabPage != null && tg.Visible) {
						focusRect = tg.ViewInfo.GetScreenTabFocusRect(tg.SelectedTabPageIndex);
						DrawFocusRectCore(cache, focusRect);
						return;
					}
				}
				if(lg != null && lg.Visible && lg.ExpandButtonVisible && lg.GroupBordersVisible && lg.ParentTabbedGroup == null) {
					if(!OptionsFocus.AllowFocusGroups) return;
					focusRect = lg.ViewInfo.BorderInfo.ButtonBounds;
				}
			}
			if(focusRect != Rectangle.Empty) {
				focusRect.Inflate(-1, -1);
				DrawFocusRectCore(cache, focusRect);
			}
		}
		protected virtual void DrawFocusRectCore(GraphicsCache cache, Rectangle focusRect) {
			Color focusForeColor = LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.ControlText);
			Color focusBackColor = LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Control);
			cache.Paint.DrawFocusRectangle(cache.Graphics, focusRect, focusForeColor, focusBackColor);
		}
		protected Color GetBackColor {
			get {
				Control parent = Parent;
				Color resultColor = Color.Transparent;
				while(parent != null && resultColor == Color.Transparent) {
					if(parent.BackColor != Color.Transparent && parent.BackColor != Color.Empty)
						resultColor = parent.BackColor;
					parent = parent.Parent;
				}
				if(resultColor == Color.Empty) resultColor = SystemColors.Control;
				return LookAndFeelHelper.GetSystemColor(ActiveStyle.LookAndFeel, resultColor != Color.Transparent ? resultColor : SystemColors.Control);
			}
		}
		protected Color oldForeColor;
		protected internal virtual void PaintIt(PaintEventArgs e) {
			if(Root == null) return;
			try {
				DenyFireChangingChangedEvents++;
				CheckForeColor();
				using(GraphicsCache cache = new GraphicsCache(e)) {
					if(!OptionsView.EnableTransparentBackColor) cache.Graphics.FillRectangle(cache.GetSolidBrush(GetBackColor), e.ClipRectangle);
					ObjectPainter.DrawObject(cache, ActiveStyle.GetPainter(Root), Root.ViewInfo);
					DrawBorder(e, cache);
					if(implementorCore.scrollerCore.EmptyArea.Visible) {
						cache.Graphics.FillRectangle(cache.GetSolidBrush(GetBackColor), implementorCore.scrollerCore.EmptyArea.Bounds);
					}
					DrawFocusRect(cache);
				}
			} finally {
				DenyFireChangingChangedEvents--;
			}
		}
		void CheckForeColor() {
			if(oldForeColor != ForeColor && !DesignMode) {
				oldForeColor = ForeColor;
				OnForeColorUpdated();
			}
		}
		protected virtual void OnForeColorUpdated() {
			OnForeColorChanged(EventArgs.Empty);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public override Image BackgroundImage {
			get {
				return base.BackgroundImage;
			}
			set {
				base.BackgroundImage = value;
			}
		}
#if DXWhidbey
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override ImageLayout BackgroundImageLayout {
			get {
				return base.BackgroundImageLayout;
			}
			set {
				base.BackgroundImageLayout = value;
			}
		}
#endif
		internal int paintCount = 0;
		protected override void OnPaint(PaintEventArgs e) {
			if(implementorCore.DisposingFlagInternal) return;
			PaintIt(e);
			RaisePaintEvent(this, e);
			paintCount++;
		}
		protected override void OnPaintBackground(PaintEventArgs e) {
			if(OptionsView.EnableTransparentBackColor)
				base.OnPaintBackground(e);
		}
		protected internal void ScrollBarMouseEnter(object sender, EventArgs e) { }
		protected internal void ScrollBarMouseLeave(object sender, EventArgs e) { }
		Size ClientSizeCore {
			get {
				return new Size(
					Width - (Scroller.VScroll.Visible ? Scroller.ScrollSize : 0),
					Height - (Scroller.HScroll.Visible ? Scroller.ScrollSize : 0));
			}
		}
		internal Scrolling.ScrollInfo Scroller {
			get { return ((ILayoutControl)this).Scroller; }
		}
		Scrolling.ScrollInfo ILayoutControl.Scroller {
			get { return implementor.Scroller; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected override Control.ControlCollection CreateControlsInstance() {
			return new LayoutControlCollection(this);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public bool IsUpdateLocked { get { return implementor.IsUpdateLocked; } }
		[Obsolete("Use the Images property instead.")]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlCaptionImages"),
#endif
 DefaultValue((string)null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object CaptionImages {
			get { return implementorCore.Images; }
			set { implementorCore.Images = value; }
		}
		[ DefaultValue((string)null)]
		[TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public virtual object Images {
			get { return implementorCore.Images; }
			set { implementorCore.Images = value; }
		}
		[DefaultValue((string)null), 
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlMenuManager"),
#endif
 Category("BarManager")]
		public IDXMenuManager MenuManager {
			get { return implementor.MenuManager; }
			set { implementor.MenuManager = value; }
		}
		public void ExportLayout(Stream stream) {
			if(stream != null) {
				Root.Resizer.ExportLayout(stream);
			}
		}
		void ILayoutControl.Invalidate() { implementor.Invalidate(); }
		void ILayoutControl.SelectionChanged(IComponent component) { implementor.SelectionChanged(component); }
		public event LayoutGroupCancelEventHandler GroupExpandChanging {
			add { this.Events.AddHandler(groupExpandChanging, value); }
			remove { this.Events.RemoveHandler(groupExpandChanging, value); }
		}
		public event LayoutUpgradeEventHandler LayoutUpgrade {
			add { this.Events.AddHandler(layoutUpgrade, value); }
			remove { this.Events.RemoveHandler(layoutUpgrade, value); }
		}
		public event LayoutAllowEventHandler BeforeLoadLayout {
			add { this.Events.AddHandler(beforeLoadLayoutCore, value); }
			remove { this.Events.RemoveHandler(beforeLoadLayoutCore, value); }
		}
		public event UniqueNameRequestHandler RequestUniqueName {
			add { this.Events.AddHandler(uniqueNameRequest, value); }
			remove { this.Events.RemoveHandler(uniqueNameRequest, value); }
		}
		[Obsolete("Use the RequestUniqueName event instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event UniqueNameRequestHandler UniqueNameRequest {
			add { this.Events.AddHandler(uniqueNameRequest, value); }
			remove { this.Events.RemoveHandler(uniqueNameRequest, value); }
		}
		public event EventHandler LayoutUpdate {
			add { this.Events.AddHandler(layoutUpdate, value); }
			remove { this.Events.RemoveHandler(layoutUpdate, value); }
		}
		public event LayoutGroupEventHandler GroupExpandChanged {
			add { this.Events.AddHandler(groupExpandChanged, value); }
			remove { this.Events.RemoveHandler(groupExpandChanged, value); }
		}
		void ILayoutControl.RaiseGroupExpandChanging(LayoutGroupCancelEventArgs e) {
			LayoutGroupCancelEventHandler handler = (LayoutGroupCancelEventHandler)this.Events[groupExpandChanging];
			if(handler != null) handler(this, e);
		}
		void ILayoutControl.SetIsModified(bool newVal) {
			implementor.SetIsModified(newVal);
		}
		protected virtual void RaiseLayoutUpdate(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[layoutUpdate];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseChanged(EventArgs e) {
			if(changed != null && DenyFireChangingChangedEvents == 0) changed(this, e);
		}
		protected void RaiseUniqueNameRequest(UniqueNameRequestArgs e) {
			UniqueNameRequestHandler handler = (UniqueNameRequestHandler)this.Events[uniqueNameRequest];
			if(handler != null) handler(this, e);
		}
		void ILayoutControl.RaiseGroupExpandChanged(LayoutGroupEventArgs e) {
			LayoutGroupEventHandler handler = (LayoutGroupEventHandler)this.Events[groupExpandChanged];
			if(handler != null) handler(this, e);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use the Refresh method instead")]
		public void HardUpdate() {
			Refresh();
		}
		public virtual void RegisterFixedItemType(Type itemType) {
			implementorCore.RegisterFixedItemType(itemType);
		}
		public virtual void UnRegisterFixedItemType(Type itemType) {
			implementorCore.UnRegisterFixedItemType(itemType);
		}
		public override void Refresh() {
			implementor.ShouldArrangeTextSize = true;
			implementor.ShouldUpdateConstraints = true;
			implementor.ShouldUpdateControls = true;
			implementor.ShouldUpdateControlsLookAndFeel = true;
			implementor.ShouldUpdateLookAndFeel = true;
			implementor.Invalidate();
			Update();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false)]
		public HiddenItemsCollection HiddenItems {
			get { return implementor.HiddenItems; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public Rectangle CustomizationFormBounds {
			get {
				if(CustomizationForm != null)
					return CustomizationForm.Bounds;
				else
					return Rectangle.Empty;
			}
			set {
				if(CustomizationForm != null)
					CustomizationForm.Bounds = value;
			}
		}
		Control ILayoutControl.Control { get { return this; } }
		public void HideItem(BaseLayoutItem item) {
			if(item != null) {
				item.HideToCustomization();
			}
		}
		bool ILayoutControl.AllowManageDesignSurfaceComponents {
			get { return implementor.AllowManageDesignSurfaceComponents; }
			set { implementor.AllowManageDesignSurfaceComponents = value; }
		}
		bool ILayoutControl.EnableCustomizationForm {
			get { return implementor.EnableCustomizationForm; }
			set { implementor.EnableCustomizationForm = value; }
		}
		void ILayoutControl.RegisterLayoutAdapter(ILayoutAdapter adapter) {
			implementor.RegisterLayoutAdapter(adapter);
		}
		LayoutGroupHandlerWithTabHelper ILayoutControl.CreateRootHandler(LayoutGroup group) {
			return implementor.CreateRootHandler(group);
		}
		FakeFocusContainer ILayoutControl.FakeFocusContainer {
			get { return implementor.FakeFocusContainer; }
		}
		bool ILayoutControl.AllowSetIsModified {
			get { return implementor.AllowSetIsModified; }
			set { implementor.AllowSetIsModified = value; }
		}
		bool ILayoutControl.EnableCustomizationMode {
			get { return implementor.EnableCustomizationMode; }
			set { implementor.EnableCustomizationMode = value; InvalidateAdornerWindowHandler(); }
		}
		public virtual void ShowCustomizationForm() {
			implementor.ShowCustomizationForm();
		}
		public virtual UserCustomizationForm CreateCustomizationForm() {
			UserCustomizationForm ucf = null;
			if(userCustomizationFormType != null) ucf = Activator.CreateInstance(userCustomizationFormType) as UserCustomizationForm;
#if DEBUGTEST
			if(ucf == null) ucf = new TestCustomizationForm();
#else
			if(ucf == null) ucf = new CustomizationForm();
#endif
			(ucf as ICustomizationContainer).Register(this);
			return ucf;
		}
		public void HideCustomizationForm() {
			implementor.HideCustomizationForm();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public UserCustomizationForm CustomizationForm {
			get { return implementor.CustomizationForm; }
		}
		#region styleController ds44094
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlStyleController"),
#endif
 DefaultValue(null)]
		public virtual IStyleController StyleController {
			get { return implementorCore.fStyleController; }
			set { implementorCore.SetStyleController(value); }
		}
		#endregion
		LayoutControlCustomizeHandler ILayoutControlOwner.CreateLayoutControlCustomizeHandler() { return CreateLayoutControlCustomizeHandler(); }
		protected virtual LayoutControlCustomizeHandler CreateLayoutControlCustomizeHandler() {
			return new LayoutControlCustomizeHandler(this);
		}
		LayoutControlHandler ILayoutControlOwner.CreateLayoutControlRuntimeHandler() { return CreateLayoutControlRuntimeHandler(); }
		protected virtual LayoutControlHandler CreateLayoutControlRuntimeHandler() {
			return new LayoutControlHandler(this);
		}
		protected internal LayoutControlHandler ActiveHandler {
			get { return implementor.ActiveHandler; }
		}
		UserInteractionHelper ILayoutControl.UserInteractionHelper {
			get { return implementor.UserInteractionHelper; }
		}
		protected internal TextAlignManager TextAlignManager {
			get { return implementor.TextAlignManager; }
		}
		object IControlIterator.GetNextObject(object current) { return FocusHelper.GetNextControl(current); }
		object IControlIterator.GetFirstObject() { return FocusHelper.GetFirstControl(); }
		bool IControlIterator.IsLastObject(object obj) { return (((IControlIterator)this).GetNextObject(obj) == null); }
		TextAlignManager ILayoutControl.TextAlignManager {
			get { return implementor.TextAlignManager; }
		}
		LayoutControlHandler ILayoutControl.ActiveHandler { get { return implementor.ActiveHandler; } }
		bool ILayoutControl.DesignMode { get { return GetDesignMode(); } }
		protected virtual bool GetDesignMode(){return implementor.DesignMode;  }
		bool allowPaintEmptyRootGroupTextCore = true;
		bool ILayoutControl.AllowPaintEmptyRootGroupText { get { return allowPaintEmptyRootGroupTextCore; } set { allowPaintEmptyRootGroupTextCore = value; } }
		public void BeginUpdate() { implementor.BeginUpdate(); lastVisibleBounds = null; }
		public void EndUpdate() { implementor.EndUpdate(); }
		public virtual void BeginInit() { implementor.BeginInit(); }
		void ILayoutDesignerMethods.BeginChangeUpdate() {
			implementorCore.layoutDesignerMethodProvider.BeginChangeUpdate();
		}
		bool ILayoutDesignerMethods.CanInvokePainting {
			get { return implementorCore.layoutDesignerMethodProvider.CanInvokePainting; }
		}
		void ILayoutDesignerMethods.RecreateHandle() {
		}
		bool ILayoutDesignerMethods.AllowDisposeControlOnItemDispose {
			get {
				if(this.Site == null) return true;
				if(this.Site.Container == null) return true;
				if(!(this.Site.Container is IDesignerHost)) return true;
				try {
					IUndoEngine e = UndoEngineHelper.GetUndoEngine(this);
					if(e == null) return true;
					return !e.UndoInProgress;
				} catch {
					return true;
				}
			}
			set { }
		}
		void ILayoutDesignerMethods.ResetResizer() {
			((ILayoutDesignerMethods)implementorCore.layoutDesignerMethodProvider).ResetResizer();
		}
		bool ILayoutDesignerMethods.AllowHandleEvents {
			get { return implementorCore.layoutDesignerMethodProvider.AllowHandleEvents; }
			set { implementorCore.layoutDesignerMethodProvider.AllowHandleEvents = value; }
		}
		bool ILayoutDesignerMethods.AllowHandleControlRemovedEvent {
			get { return implementorCore.layoutDesignerMethodProvider.AllowHandleControlRemovedEvent; }
			set { implementorCore.layoutDesignerMethodProvider.AllowHandleControlRemovedEvent = value; }
		}
		bool ILayoutDesignerMethods.IsInternalControl(Control control) {
			return implementorCore.layoutDesignerMethodProvider.IsInternalControl(control);
		}
		void ILayoutDesignerMethods.EndChangeUpdate() {
			implementorCore.layoutDesignerMethodProvider.EndChangeUpdate();
		}
		LayoutControlHandler ILayoutDesignerMethods.RuntimeHandler {
			get { return implementorCore.RuntimeHandler; }
			set { implementorCore.RuntimeHandler = value; }
		}
		event DeleteSelectedItemsEventHandler ILayoutDesignerMethods.DeleteSelectedItems {
			add { ((ILayoutDesignerMethods)implementorCore.layoutDesignerMethodProvider).DeleteSelectedItems += value; }
			remove { ((ILayoutDesignerMethods)implementorCore.layoutDesignerMethodProvider).DeleteSelectedItems -= value; }
		}
		 void ILayoutDesignerMethods.RaiseDeleteSelectedItems(DeleteSelectedItemsEventArgs e) {
			((ILayoutDesignerMethods)implementorCore.layoutDesignerMethodProvider).RaiseDeleteSelectedItems(e);
		}
		bool ILayoutControl.InitializationFinished {
			get { return implementor.InitializationFinished; }
			set { implementor.InitializationFinished = value; }
		}
		public virtual void Clear(bool clearHiddenItems, bool disposeControls) {
			BeginUpdate();
			ArrayList tempItems = new ArrayList(Items);
			Control tempControl = null;
			LayoutControlItem lci;
			tempItems.Reverse();
			foreach(BaseLayoutItem item in tempItems) {
				if(item == Root) continue;
				if(item.IsHidden && !clearHiddenItems) continue;
				lci = item as LayoutControlItem;
				if(lci != null) tempControl = lci.Control;
				item.Dispose();
				if(tempControl != null && disposeControls) {
					tempControl.Parent = null;
					tempControl.Dispose();
					tempControl = null;
				}
				implementorCore.ClearReferences(item);
			}
			if(clearHiddenItems) HiddenItems.Clear();
			EndUpdate();
		}
		public virtual void Clear() {
			Clear(false, true);
		}
		public virtual void EndInit() { implementor.EndInit(); }
		bool ILayoutControl.DisposingFlag { get { return implementor.DisposingFlag; } }
		protected override void Dispose(bool disposing) {
			if(!disposing) {
				base.Dispose(disposing);
				return;
			}
			DisposeAdorner();
			if(componentPrinterCore != null) {
				componentPrinterCore.Dispose();
				componentPrinterCore = null;
			}
			if(implementorCore.DisposingFlagInternal) return;
			try {
				implementorCore.DisposingFlagInternal = true;
				DisposeAsDragDropDispatcherClient();
				implementorCore.DisposeLongPressControl();
				implementorCore.DisposeUserInteractionHelper();
				implementorCore.DisposeUndoManagerCore();
				implementorCore.DisposeCustomizationFormCore();
				implementorCore.DestroyTimerHandler();
				implementorCore.DisposeHiddenItemsCore();
				implementorCore.DisposeHandlers();
				implementorCore.DisposeRootGroupCore();
				implementorCore.DisposeStyleControllerCore();
				implementorCore.DisposePaintStylesCore();
				implementorCore.DisposeAppearanceCore();
				implementorCore.DisposeLookAndFeelCore();
				implementorCore.DisposeFakeFocusContainerCore();
				implementorCore.DisposeScrollerCore();
				DisposeToolTipController();
				Items = null;
				LayoutPaintStyle.Reset();
				((IDisposable)FocusHelper).Dispose();
				if(true ) {
					DisposeStaticWindows();
				}
				if(implementorCore.AppearanceController != null) {
					implementorCore.AppearanceController.Dispose();
					implementorCore.appearanceControllerCore = null;
				}
				OptionsFocus.EnableAutoTabOrder = false;
			} finally {
				base.Dispose(disposing);
			}
		}
		private void DisposeAdorner() {
			if(layoutAdorner != null) {
				if(layoutAdorner.adornerWindow != null) layoutAdorner.adornerWindow.Dispose();
				layoutAdorner = null;
			}
			if(layoutAdornerWindowHandler != null) layoutAdornerWindowHandler = null;
		}
		protected void DisposeStaticWindows() {
			TreeViewVisualizerFrame.Reset();
			DragDropItemCursor.Reset();
		}
		protected virtual void DisposeAsDragDropDispatcherClient() {
			if(this.DragDropDispatcherClientHelper != null) this.DragDropDispatcherClientHelper.Dispose();
			dragDropDispatcherClientHelperCore = null;
		}
		public virtual LayoutGroup CreateLayoutGroup(LayoutGroup parent) {
			return implementor.CreateLayoutGroup(parent);
		}
		public virtual BaseLayoutItem CreateLayoutItem(LayoutGroup parent) {
			return implementor.CreateLayoutItem(parent);
		}
		public virtual TabbedGroup CreateTabbedGroup(LayoutGroup parent) {
			return implementor.CreateTabbedGroup(parent);
		}
		public virtual EmptySpaceItem CreateEmptySpaceItem(LayoutGroup parent) {
			return implementor.CreateEmptySpaceItem(parent);
		}
		public virtual SplitterItem CreateSplitterItem(LayoutGroup parent) {
			return implementor.CreateSplitterItem(parent);
		}
		private void Initialize() {
			SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor, true);
			implementorCore.InitializeComponents();
		}
		protected virtual void InvokeBaseMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			InvokeBaseMouseUp(e);
			if(ControlValid && implementor != null) 
				ActiveHandler.OnMouseUp(this, e);
			if(dragDropDispatcherClientHelperCore != null)
				DragDropDispatcherFactory.Default.ProcessMouseEvent(DragDropDispatcherClientHelper.ClientDescriptor, new ProcessEventEventArgs(EventType.MouseUp, e));
			if(Capture) Capture = false;
		}
		protected virtual void InvokeBaseMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
		}
		protected override void OnMouseEnter(EventArgs e) {
			InvokeBaseMouseEnter(e);
			if(ControlValid) ActiveHandler.OnMouseEnter(this, e);
		}
		protected virtual void InvokeBaseMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			InvokeBaseMouseLeave(e);
			if(!DesignMode) OnMouseLeaveCore(e);
			InvalidateAdornerWindowHandler();
		}
		protected void OnMouseLeaveCore(EventArgs e) {
			if(ControlValid) ActiveHandler.OnMouseLeave(this, e);
		}
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			OnMouseWheelCore(e);
			InvalidateAdornerWindowHandler();
		}
		protected sealed override void OnMouseWheel(MouseEventArgs e) {
			if(MouseWheelHelper.ProcessSmartMouseWheel(this, e)) return;
			OnMouseWheelCore(e);
		}
		protected virtual void OnMouseWheelCore(MouseEventArgs e) {
			HandledMouseEventArgs hmea = e as HandledMouseEventArgs;
			if(hmea != null) {
				if(ControlValid && !hmea.Handled) {
					hmea.Handled = ActiveHandler.OnMouseWheel(this, e);
				}
			}
			base.OnMouseWheel(e);
		}
		protected virtual void InvokeBaseMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(!implementor.EnableCustomizationMode && !DesignMode) {
				BaseLayoutItemHitInfo hitInfo = CalcHitInfo(e.Location);
				if(hitInfo.IsExpandButton || hitInfo.TabPageIndex >= 0) {
					implementor.FakeFocusContainer.Focus();
					if(!implementor.FakeFocusContainer.ContainsFocus) return;
				}
			}
			if(ControlValid) ActiveHandler.OnMouseDown(this, e);
			DragDropDispatcherFactory.Default.ProcessMouseEvent(
					DragDropDispatcherClientHelper.ClientDescriptor, new ProcessEventEventArgs(EventType.MouseDown, e)
				);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			ActiveHandler.OnKeyDown(this, e);
		}
		protected override void OnMouseClick(MouseEventArgs e) {
			base.OnMouseClick(e);
			ActiveHandler.OnClick(this, e);
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e) {
			base.OnMouseDoubleClick(e);
			ActiveHandler.OnDoubleClick(this, e);
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("LayoutControlBackColor")]
#endif
		public override Color BackColor {					   
			get {
				if(base.BackColor == Color.Empty && LookAndFeel != null) return LookAndFeelHelper.GetEmptyBackColor(LookAndFeel, this);
				return base.BackColor;
			}
			set { 
				base.BackColor = value;
			}
		}
		[DefaultValue(true)]
		[Browsable(false)]
		[Obsolete("Use AllowCustomization instead")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AllowCustomizationMenu {
			get { return implementor.AllowCustomizationMenu; }
			set { implementor.AllowCustomizationMenu = value; }
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlAllowCustomizationMenu"),
#endif
 DefaultValue(true)]
		public bool AllowCustomization {
			get { return implementor.AllowCustomizationMenu; }
			set { implementor.AllowCustomizationMenu = value; }
		}
		[DefaultValue(CustomizationModes.Default)]
		public CustomizationModes CustomizationMode {
			get { return implementor.CustomizationMode; }
			set {
				implementor.CustomizationMode = value;			   
			}
		}
		internal int GetSizingInerval() {
			return implementor.CustomizationMode == CustomizationModes.Default ? 2 : 7;
		}
		Point oldMovePoint = Point.Empty;
#if DEBUGTEST
		protected internal void CallOnMouseDown(MouseEventArgs e) { OnMouseDown(e); }
		protected internal void CallOnMouseUp(MouseEventArgs e) { OnMouseUp(e); }
		protected internal void CallOnMouseMove(MouseEventArgs e) { OnMouseMove(e); }
#endif
		protected virtual void InvokeBaseMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			Scroller.OnAction(ScrollNotifyAction.MouseMove);
			InvokeBaseMouseMove(e);
			if(ControlValid && (oldMovePoint.X != e.X || oldMovePoint.Y != e.Y)) {
				ActiveHandler.OnMouseMove(this, e);
				oldMovePoint = e.Location;
			}
			InvalidateAdornerWindowHandler();
			if(DragDropDispatcherClientHelper.ClientDescriptor == null) InitializeAsDragDropDispatcherClient();
			DragDropDispatcherFactory.Default.ProcessMouseEvent(DragDropDispatcherClientHelper.ClientDescriptor, new ProcessEventEventArgs(EventType.MouseMove, e));
		}
		protected override void OnLostFocus(EventArgs e) {
			base.OnLostFocus(e);
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			if(!IsHandleCreated) return;
			if(FocusHelper == null) return;
			if(!Visible) return;
			if(implementorCore == null || implementorCore.DisposingFlagInternal) return;
			Component selectedComponent = OptionsFocus.ActivateSelectedControlOnGotFocus ?
				FocusHelper.SelectedComponent : null;
			if(selectedComponent == null && ActiveControl == null) {
				if((Control.ModifierKeys & Keys.Shift) != 0)
					FocusHelper.FocusLast();
				else
					FocusHelper.FocusFirst();
			} else {
				LayoutItem item = null;
				Control tempActiveControl = ActiveControl;
				while((item == null || tempActiveControl != this) && tempActiveControl != null) {
					item = GetItemByControl(tempActiveControl);
					tempActiveControl = tempActiveControl.Parent;
				}
				FocusHelper.FocusElement(item != null ? item : selectedComponent, false);
			}
			InvalidateAdornerWindowHandler();
		}
		void UpdatePreferredSizeByClientRect() {
			Size sz = ClientSizeCore;
			if(sz.Width < Root.MinSize.Width)
				sz.Width = Root.MinSize.Width;
			if(sz.Height < Root.MinSize.Height)
				sz.Height = Root.MinSize.Height;
			Root.PreferredSize = sz;
			if(Root.PreferredSize != Root.Size)
				Root.Resizer.SizeIt(Root.PreferredSize);
		}
		internal void DoDragEnter(DragEventArgs dragEvents) {
			OnDragEnter(dragEvents);
		}
		internal void DoDragOver(DragEventArgs dragEvents) {
			OnDragOver(dragEvents);
		}
		internal void DoDragLeave(EventArgs eventArgs) {
			OnDragLeave(eventArgs);
		}
		internal void SetControlDefaultsLast() { (this as ILayoutControl).SetControlDefaultsLast(); }
		internal void SetControlDefaults() { (this as ILayoutControl).SetControlDefaults(); }
		internal void UpdateLayoutControlPrinter() {
			printer = CreatePrinter();
		}
		void ILayoutControl.SetControlDefaultsLast() {
			implementor.SetControlDefaultsLast();
		}
		void ILayoutControl.SetControlDefaults() {
			implementor.SetControlDefaults();
		}
		void ILayoutControl.UpdateFocusedElement(BaseLayoutItem item) {
			implementor.UpdateFocusedElement(item);
		}
		public void SetCursor(Cursor cursor) {
			implementor.SetCursor(cursor);
		}
		public override Cursor Cursor {
			get {
				if(implementorCore.cursorCore == Cursors.Arrow) return base.Cursor;
				return implementorCore.cursorCore;
			}
			set { base.Cursor = value; }
		}
		void ILayoutControl.RaiseShowCustomizationMenu(PopupMenuShowingEventArgs ma) {
			implementor.RaiseShowCustomizationMenu(ma);
		}
		void ILayoutControl.RaiseShowLayoutTreeViewContextMenu(PopupMenuShowingEventArgs ma) {
			implementor.RaiseShowLayoutTreeViewContextMenu(ma);
		}
		LayoutControlGroup ILayoutControl.RootGroup {
			get { return implementor.RootGroup; }
			set { implementor.RootGroup = value; }
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("LayoutControlRoot")]
#endif
		public LayoutControlGroup Root {
			get { return rootGroupCore; }
			set {
				if(DesignMode && !IsUpdateLocked && value == null) return;
				if(value != null && (value.Parent != null || value.ParentTabbedGroup != null)) {
					throw new ArgumentException("A group that has a parent cannot be assigned to the Root property!");
				}
				if(Root != null) Root.Owner = null;
				rootGroupCore = value;
				implementor.RootGroup = value;
				if(Disposing) return;
			}
		}
		protected override void OnRightToLeftChanged(EventArgs e) {
			base.OnRightToLeftChanged(e);
			implementor.Invalidate();
		}
		void ILayoutControl.AddComponent(BaseLayoutItem component) {
			implementor.AddComponent(component);
		}
		void ILayoutControl.RemoveComponent(BaseLayoutItem component) {
			implementor.RemoveComponent(component);
		}
		void ILayoutControl.FireItemAdded(BaseLayoutItem component, EventArgs ea) {
			implementor.FireItemAdded(component, ea);
		}
		void ILayoutControl.FireItemRemoved(BaseLayoutItem component, EventArgs ea) {
			implementor.FireItemRemoved(component, ea);
		}
		string ILayoutControl.GetUniqueName(BaseLayoutItem item) {
			return implementor.GetUniqueName(item);
		}
		SizeF scaleFactor = new SizeF(1f, 1f);
		protected override void ScaleControl(SizeF factor, BoundsSpecified specified) {
			this.scaleFactor = new SizeF(scaleFactor.Width * factor.Width, scaleFactor.Height * factor.Height);
			base.ScaleControl(factor, specified);
			ScaleChildControlsIfNeed();
		}
		int startScaleIndexOfControl = 0;
		protected override void OnFontChanged(EventArgs e) {
			base.OnFontChanged(e);
			SetStartScaleIndex();
		}
		void ScaleChildControlsIfNeed() {
			for(int i = startScaleIndexOfControl; i < Controls.Count; i++) {
				Controls[i].Scale(scaleFactor);
			}
			startScaleIndexOfControl = Controls.Count;
		}
		void SetStartScaleIndex() {
			if(!implementorCore.InitializationFinished) {
				startScaleIndexOfControl = Controls.Count;
			}
		}
		protected override bool ScaleChildren {
			get {
				return false;
			}																											   
		}
		SizeF ILayoutControl.AutoScaleFactor {
			get {
				if(implementorCore != null && (implementorCore.IsSerializing || implementorCore.isDeserializingCore)) return new SizeF(1f, 1f);
				if(OptionsView != null && OptionsView.UseParentAutoScaleFactor) return scaleFactor;
				return new SizeF(1f, 1f);
			}
		}
		LayoutStyleManager ILayoutControl.LayoutStyleManager { get { return implementor.LayoutStyleManager; } }
		public LayoutGroup GetGroupAtPoint(Point p) {
			return implementor.GetGroupAtPoint(p);
		}
		bool ControlValid { get { return Root != null; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int ClientWidth {
			get {
				if(WindowsFormsSettings.GetIsRightToLeft(this))
					return Math.Max(0, ClientSize.Width);
				else {
					int vw = this.ClientSize.Width - (Scroller.VScrollVisible ? Scroller.ScrollSize : 0);
					return Math.Max(0, vw);
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int ClientHeight {
			get {
				int vh = this.ClientSize.Height - (Scroller.HScrollVisible ? Scroller.ScrollSize : 0);
				return Math.Max(0, vh);
			}
		}
		protected override void OnLayout(LayoutEventArgs le) {}
		protected int allowFireChangedEvents = 0;
		protected internal int DenyFireChangingChangedEvents {
			get { return allowFireChangedEvents; }
			set { allowFireChangedEvents = value; }
		}
		protected bool IsScaled() {
			if(Scroller == null || Scroller.EmptyArea == null) return false;
			return Scroller.EmptyArea.MaximumSize.Width != 1000 || Scroller.EmptyArea.MaximumSize.Height != 1000;
		}
		protected override void OnHandleDestroyed(EventArgs e) {
			base.OnHandleDestroyed(e);
			try {
				if(implementorCore != null && implementorCore.LongPressControl != null) {
					implementorCore.LongPressControl.Dispose();
					implementorCore.longPressControlCore = null;
				}
			} catch { }
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			if(CanShowAdorner) {
				CreateAdornerHandlerAndWindow();
				ShowAdorner();
			}
			try {
				DenyFireChangingChangedEvents++;
				if(implementorCore.DisposingFlagInternal) return;
				if(implementorCore.DesignMode) return;
				implementorCore.OnLookAndFeelStyleChanged(this, null);
				InitializeAsDragDropDispatcherClient();
				if(Root == null) return;
				OnBindingContextChanged();
				if(implementor.UpdatedCount != 0) return;
				if(implementor.Items.Any(item => (item.Text != null) && item.Text.Contains("&&")))
					implementor.ShouldUpdateLookAndFeel = true;
				implementorCore.ShouldResize |= IsScaled();
				implementorCore.ShouldUpdateConstraints |= IsScaled();
				ProcessSizeChanged(true);
				foreach(BaseLayoutItem bli in Items) {
					if(bli.Visible) bli.RaiseShowHide(true);
				}
				if(CustomizationMode == CustomizationModes.Quick && !DesignTimeTools.IsDesignMode && AllowCustomization) implementor.LongPressControl.Enabled = true;
			} finally {
				DenyFireChangingChangedEvents--;
			}
		}
		protected virtual void ShowAdorner() {
			layoutAdorner.Show();
		}
		protected virtual void CreateAdornerHandlerAndWindow() {
			if(layoutAdorner == null)layoutAdorner = new LayoutAdorner(this);
			if(layoutAdornerWindowHandler == null) layoutAdornerWindowHandler = new LayoutAdornerWindowHandler(this);
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			ProcessSizeChanged(false);
			InvalidateAdornerWindowHandler();
		}
		public override Size GetPreferredSize(Size proposedSize) {
			Size baseSize = base.GetPreferredSize(proposedSize);
			if(Root == null) return baseSize;
			if(baseSize.Width < Root.MinSize.Width) baseSize.Width = Root.MinSize.Width;
			if(baseSize.Height < Root.MinSize.Height) baseSize.Height = Root.MinSize.Height;
			return baseSize;
		}
		private void ProcessSizeChangedCore() {
			implementorCore.ProcessSizeChangedCore();
		}
		Size oldSize = Size.Empty;
		protected void ProcessSizeChanged(bool force) {
			if(Size.Height == 0) { oldSize = Size; return; }
			if(oldSize == Size && !force) return;
			oldSize = Size;
			try {
				implementorCore.isSizeChanging = true;
				DenyFireChangingChangedEvents++;
				ProcessSizeChangedCore();
			} finally {
				implementorCore.isSizeChanging = false;
				DenyFireChangingChangedEvents--;
			}
		}
		protected void UpdateSplitterItemsLayout() {
			SplitterItemLayoutTypeUpdater updater = new SplitterItemLayoutTypeUpdater(this);
			Root.Accept(updater);
		}
		protected override Size DefaultSize { get { return new Size(180, 120); } }
		public LayoutControlItem AddItem() {
			return Root.AddItem();
		}
		public LayoutControlItem AddItem(String text) {
			return Root.AddItem(text);
		}
		public LayoutControlItem AddItem(String text, Control control) {
			return Root.AddItem(text, control);
		}
		public LayoutControlItem AddItem(BaseLayoutItem baseItem, InsertType insertType) {
			return Root.AddItem(baseItem, insertType);
		}
		public LayoutControlItem AddItem(String text, Control control, BaseLayoutItem baseItem, InsertType insertType) {
			return Root.AddItem(text, control, baseItem, insertType);
		}
		public LayoutControlItem AddItem(BaseLayoutItem newItem) {
			return Root.AddItem(newItem);
		}
		public LayoutControlItem AddItem(BaseLayoutItem newItem, BaseLayoutItem baseItem, InsertType insertType) {
			return Root.AddItem(newItem, baseItem, insertType);
		}
		[Obsolete()]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public LayoutControlItem AddItem(String text, Control control, BaseLayoutItem newItem, BaseLayoutItem baseItem, InsertType insertType) {
			return Root.AddItem(text, newItem, baseItem, insertType) as LayoutControlItem;
		}
		public LayoutControlGroup AddGroup() {
			return Root.AddGroup();
		}
		public LayoutControlGroup AddGroup(String text) {
			return Root.AddGroup(text);
		}
		public LayoutControlGroup AddGroup(BaseLayoutItem baseItem, InsertType insertType) {
			return Root.AddGroup(baseItem, insertType);
		}
		public LayoutControlGroup AddGroup(String text, BaseLayoutItem baseItem, InsertType insertType) {
			return Root.AddGroup(text, baseItem, insertType);
		}
		public LayoutControlGroup AddGroup(LayoutGroup newGroup) {
			return Root.AddGroup(newGroup);
		}
		public LayoutControlGroup AddGroup(LayoutGroup newGroup, BaseLayoutItem baseItem, InsertType insertType) {
			return Root.AddGroup(newGroup, baseItem, insertType);
		}
		public TabbedControlGroup AddTabbedGroup() {
			return Root.AddTabbedGroup();
		}
		public TabbedControlGroup AddTabbedGroup(TabbedGroup newTabbedGroup) {
			return Root.AddTabbedGroup(newTabbedGroup);
		}
		public TabbedControlGroup AddTabbedGroup(BaseLayoutItem baseItem, InsertType insertType) {
			return Root.AddTabbedGroup(baseItem, insertType);
		}
		public TabbedControlGroup AddTabbedGroup(TabbedGroup newTabbedGroup, BaseLayoutItem baseItem, InsertType insertType) {
			return Root.AddTabbedGroup(newTabbedGroup, baseItem, insertType);
		}
		#region IPrintable
		protected virtual LayoutControlPrinter CreatePrinter() {
			if(!OptionsPrint.OldPrinting) return new LayoutFlowPrinter(this);
			return new LayoutControlPrinter(this);
		}
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
			if(printer != null) {
				printer.Release();
			}
		}
		protected virtual void SetCommandsVisibility(IPrintingSystem ps) {
			ps.SetCommandVisibility(PrintingSystemCommand.ExportRtf, true);
			ps.SetCommandVisibility(PrintingSystemCommand.ExportHtm, true);
			ps.SetCommandVisibility(PrintingSystemCommand.ExportRtf, true);
			ps.SetCommandVisibility(PrintingSystemCommand.ExportXls, true);
			ps.SetCommandVisibility(PrintingSystemCommand.ExportXlsx, true);
		}
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
			SetCommandsVisibility(ps);
			printer.Initialize(ps, link, Root.ViewInfo);
		}
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
			printer.CreateArea(areaName, graph);
		}
		bool IPrintable.CreatesIntersectedBricks {
			get { return false; }
		}
		void IPrintable.AcceptChanges() {
			printer.AcceptChanges();
		}
		void IPrintable.RejectChanges() {
			printer.RejectChanges();
		}
		void IPrintable.ShowHelp() {
			printer.ShowHelp();
		}
		bool IPrintable.SupportsHelp() {
			return printer.SupportsHelp();
		}
		bool IPrintable.HasPropertyEditor() {
			return printer.HasPropertyEditor();
		}
		UserControl IPrintable.PropertyEditorControl { get { return printer.PropertyEditorControl; } }
		protected void ExecutePrintExport(Action0 method) {
			try {
				if(ComponentPrinter == null) return;
				ComponentPrinter.ClearDocument();
				ComponentPrinter.PrintingSystemBase.Document.ScaleFactor = 1.0f;
				method();
			} finally { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsPrintingAvailable { get { return ComponentPrinterBase.IsPrintingAvailable(false); } }
		public void ShowPrintPreview() {
			ExecutePrintExport(delegate() {ComponentPrinter.ShowPreview(LookAndFeel);});
		}
		public void ShowRibbonPrintPreview() { ExecutePrintExport(delegate() {ComponentPrinter.ShowRibbonPreview(LookAndFeel);}); }
		public void Print() { ExecutePrintExport(delegate() {ComponentPrinter.Print();});}
#if DEBUGTEST
		internal void CreateDocument() { ExecutePrintExport(delegate() { ComponentPrinter.CreateDocument(); }); }
#endif
		#endregion
		#region Export
		public void ExportToXlsx(string filePath) {
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Xlsx, filePath);});
		}
		public void ExportToXlsx(Stream stream) {
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Xlsx, stream);});
		}
		public void ExportToXlsx(Stream stream, XlsxExportOptions options) {
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Xlsx, stream, options);});
		}
		public void ExportToXlsx(string filePath, XlsxExportOptions options) {
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Xlsx, filePath, options);});
		}
		public void ExportToXls(string filePath) {
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Xls, filePath);});
		}
		public void ExportToXls(Stream stream) {
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Xls, stream);});
		}
		[Obsolete("Use the ExportToXls(string filePath, XlsExportOptions options) method instead")]
		public void ExportToXls(string filePath, bool useNativeFormat) {
			XlsExportOptions options = new XlsExportOptions(useNativeFormat ? TextExportMode.Value : TextExportMode.Text);
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Xls, filePath, options);});
		}
		[Obsolete("Use the ExportToXls(Stream stream, XlsExportOptions options) method instead")]
		public void ExportToXls(Stream stream, bool useNativeFormat) {
			XlsExportOptions options = new XlsExportOptions(useNativeFormat ? TextExportMode.Value : TextExportMode.Text);
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Xls, stream, options);});
		}
		public void ExportToXls(Stream stream, XlsExportOptions options) {
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Xls, stream, options);});
		}
		public void ExportToXls(string filePath, XlsExportOptions options) {
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Xls, filePath, options);});
		}
		public void ExportToRtf(string filePath) {
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Rtf, filePath);});
		}
		public void ExportToRtf(Stream stream) {
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Rtf, stream);});
		}
		public void ExportToHtml(string filePath) {
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Html, filePath);});
		}
		public void ExportToHtml(Stream stream) {
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Html, stream);});
		}
		public void ExportToHtml(Stream stream, HtmlExportOptions options) {
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Html, stream, options);});
		}
		public void ExportToHtml(String filePath, HtmlExportOptions options) {
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Html, filePath, options);});
		}
		[Obsolete("Use the ExportToHtml(String filePath, HtmlExportOptions options) method instead")]
		public void ExportToHtml(string filePath, string htmlCharSet) {
			HtmlExportOptions options = new HtmlExportOptions(htmlCharSet);
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Html, filePath, options);});
		}
		[Obsolete("Use the ExportToHtml(String filePath, HtmlExportOptions options) method instead")]
		public void ExportToHtml(string filePath, string htmlCharSet, string title, bool compressed) {
			HtmlExportOptions options = new HtmlExportOptions(htmlCharSet, title, compressed);
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Html, filePath, options);});
		}
		[Obsolete("Use the ExportToHtml(Stream stream, HtmlExportOptions options) method instead")]
		public void ExportToHtml(Stream stream, string htmlCharSet, string title, bool compressed) {
			HtmlExportOptions options = new HtmlExportOptions(htmlCharSet, title, compressed);
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Html, stream, options);});
		}
		public void ExportToMht(string filePath) {
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Mht, filePath);});
		}
		public void ExportToMht(string filePath, MhtExportOptions options) {
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Mht, filePath, options);});
		}
		public void ExportToMht(Stream stream, MhtExportOptions options) {
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Mht, stream, options);});
		}
		[Obsolete("Use the ExportToMht(Stream stream, MhtExportOptions options) method instead")]
		public void ExportToMht(string filePath, string htmlCharSet) {
			MhtExportOptions options = new MhtExportOptions(htmlCharSet);
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Mht, filePath, options);});
		}
		[Obsolete("Use the ExportToMht(string filePath, MhtExportOptions options) method instead")]
		public void ExportToMht(string filePath, string htmlCharSet, string title, bool compressed) {
			MhtExportOptions options = new MhtExportOptions(htmlCharSet, title, compressed);
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Mht, filePath, options);});
		}
		[Obsolete("Use the ExportToMht(Stream stream, MhtExportOptions options) method instead")]
		public void ExportToMht(Stream stream, string htmlCharSet, string title, bool compressed) {
			MhtExportOptions options = new MhtExportOptions(htmlCharSet, title, compressed);
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Mht, stream, options);});
		}
		public void ExportToPdf(string filePath) {
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Pdf, filePath);});
		}
		public void ExportToPdf(Stream stream) {
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Pdf, stream);});
		}
		public void ExportToText(Stream stream) {
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Text, stream);});
		}
		public void ExportToText(string filePath) {
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Text, filePath);});
		}
		public void ExportToText(string filePath, TextExportOptions options) {
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Text, filePath, options);});
		}
		public void ExportToText(Stream stream, TextExportOptions options) {
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Text, stream, options);});
		}
		[Obsolete("Use the ExportToText(string filePath, TextExportOptions options) method instead")]
		public void ExportToText(string filePath, string separator) {
			TextExportOptions options = new TextExportOptions(separator);
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Text, filePath, options);});
		}
		[Obsolete("Use the ExportToText(string filePath, TextExportOptions options) method instead")]
		public void ExportToText(string filePath, string separator, bool quoteStringsWithSeparators) {
			TextExportOptions options = new TextExportOptions(separator);
			options.QuoteStringsWithSeparators = quoteStringsWithSeparators;
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Text, filePath, options);});
		}
		[Obsolete("Use the ExportToText(string filePath, TextExportOptions options) method instead")]
		public void ExportToText(string filePath, string separator, bool quoteStringsWithSeparators, Encoding encoding) {
			TextExportOptions options = new TextExportOptions(separator, encoding);
			options.QuoteStringsWithSeparators = quoteStringsWithSeparators;
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Text, filePath, options);});
		}
		[Obsolete("Use the ExportToText(Stream stream, TextExportOptions options) method instead")]
		public void ExportToText(Stream stream, string separator) {
			TextExportOptions options = new TextExportOptions(separator);
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Text, stream, options);});
		}
		[Obsolete("Use the ExportToText(Stream stream, TextExportOptions options) method instead")]
		public void ExportToText(Stream stream, string separator, bool quoteStringsWithSeparators) {
			TextExportOptions options = new TextExportOptions(separator);
			options.QuoteStringsWithSeparators = quoteStringsWithSeparators;
			ExecutePrintExport(delegate() {ComponentPrinter.Export(ExportTarget.Text, stream, options);});
		}
		[Obsolete("Use the ExportToText(Stream stream, TextExportOptions options) method instead")]
		public void ExportToText(Stream stream, string separator, bool quoteStringsWithSeparators, Encoding encoding) {
			TextExportOptions options = new TextExportOptions(separator, encoding);
			options.QuoteStringsWithSeparators = quoteStringsWithSeparators;
			ExecutePrintExport(delegate() { ComponentPrinter.Export(ExportTarget.Text, stream, options); });
		}
		#endregion
		#region IDragDropDispatcherClient
		LayoutControlDragDropHelper dragDropDispatcherClientHelperCore;
		internal protected LayoutControlDragDropHelper DragDropDispatcherClientHelper {
			get {
				if(dragDropDispatcherClientHelperCore == null && implementorCore != null && !implementorCore.DisposingFlagInternal) dragDropDispatcherClientHelperCore = CreateDragDropHelper();
				return dragDropDispatcherClientHelperCore;
			}
		}
		LayoutControlDragDropHelper ILayoutDesignerMethods.DragDropDispatcherClientHelper {
			get {
				return DragDropDispatcherClientHelper;
			}
		}
		bool IDragDropDispatcherClient.ExcludeChildren { get { return !DesignMode; } }
		protected virtual LayoutControlDragDropHelper CreateDragDropHelper() {
			return new LayoutControlDragDropHelper(this);
		}
		IntPtr IDragDropDispatcherClient.ClientHandle { get { return Handle; } }
		DragDropClientDescriptor IDragDropDispatcherClient.ClientDescriptor { get { return DragDropDispatcherClientHelper.ClientDescriptor; } }
		DragDropClientGroupDescriptor IDragDropDispatcherClient.ClientGroup { get { return DragDropDispatcherClientHelper.ClientGroup; } }
		Rectangle IDragDropDispatcherClient.ScreenBounds { get { return RectangleToScreen(ClientRectangle); } }
		bool IDragDropDispatcherClient.IsActiveAndCanProcessEvent {
			get { return CalcIsActiveAndCanProcessEvent(); }
		}
		bool IDragDropDispatcherClient.AllowProcessDragging {
			get { return CalcAllowProcessDragging(); }
			set { }
		}
		bool IDragDropDispatcherClient.AllowNonItemDrop {
			get { return DragDropDispatcherClientHelper.AllowNonItemDrop; }
			set { DragDropDispatcherClientHelper.AllowNonItemDrop = value; }
		}
		protected virtual bool CalcAllowProcessDragging() {
			bool fIsMainControl = ((this as ILayoutControl).ControlRole == LayoutControlRoles.MainControl);
			return fIsMainControl && (implementorCore.isCustomizationMode || DesignMode);
		}
		protected virtual bool CalcIsActiveAndCanProcessEvent() {
			return Enabled && Visible && (implementorCore.isCustomizationMode || DesignMode);
		}
		bool IDragDropDispatcherClient.IsPointOnItem(Point clientPoint) {
			if(DragDropDispatcherFactory.Default.State == DragDropDispatcherState.ClientDragging) {
				return DragDropDispatcherClientHelper.IsPointOnItem(clientPoint);
			} else {
				return DragDropDispatcherClientHelper.IsPointOnItemAndNotSizing(clientPoint);
			}
		}
		BaseLayoutItem IDragDropDispatcherClient.GetItemAtPoint(Point clientPoint) {
			return DragDropDispatcherClientHelper.GetItemAtPoint(clientPoint);
		}
		BaseLayoutItem IDragDropDispatcherClient.ProcessDragItemRequest(Point clientPoint) {
			return DragDropDispatcherClientHelper.ProcessDragItemRequest(clientPoint);
		}
		void IDragDropDispatcherClient.OnDragModeKeyDown(KeyEventArgs kea) {
			DragDropDispatcherClientHelper.OnDragModeKeyDown(kea);
		}
		void IDragDropDispatcherClient.OnDragEnter() {
			DragDropDispatcherClientHelper.OnDragEnter();
		}
		void IDragDropDispatcherClient.OnDragLeave() {
			DragDropDispatcherClientHelper.OnDragLeave();
		}
		void IDragDropDispatcherClient.DoDragging(Point clientPoint) {
			DragDropDispatcherClientHelper.DoDragging(clientPoint);
		}
		void IDragDropDispatcherClient.DoDrop(Point clientPoint) {
			DragDropDispatcherClientHelper.DoDrop(clientPoint);
		}
		void IDragDropDispatcherClient.DoBeginDrag() {
			DragDropDispatcherClientHelper.DoBeginDrag();
		}
		void IDragDropDispatcherClient.DoDragCancel() {
			DragDropDispatcherClientHelper.DoDragCancel();
		}
		#endregion
		Type userCustomizationFormType = null;
		public void RegisterUserCustomizationForm(Type customizationFormType) {
			if(customizationFormType.IsSubclassOf(typeof(UserCustomizationForm))) {
				userCustomizationFormType = customizationFormType;
			} else throw new ArgumentException("wrong user customization form type");
		}
		public void RegisterCustomPropertyGridWrapper(Type itemType, Type customWrapperType) {
			implementorCore.RegisterCustomPropertyGridWrapper(itemType, customWrapperType);
		}
		public void UnRegisterCustomPropertyGridWrapper(Type itemType) {
			implementorCore.UnRegisterCustomPropertyGridWrapper(itemType);
		}
		void ILayoutControl.RaiseSizeableChanged() {
			RaiseSizeableChanged();
		}
		#region IXtraResizableControl Members
		private static readonly object sizeableChanged;
		Size lastMinSize = Size.Empty, lastMaxSize = Size.Empty;
		protected void RaiseSizeableChanged() {
			RaiseSizeableChangedCore(false);
		}
		protected internal void RaiseSizeableChangedCore(bool force) {
			SetAutoSizeLayout();
			if(DesignMode && !force) return;
			EventHandler changed = (EventHandler)base.Events[sizeableChanged];
			IXtraResizableControl ixrc = this as IXtraResizableControl;
			if(changed != null && (ixrc.MinSize != lastMinSize || ixrc.MaxSize != lastMaxSize)) {
				lastMinSize = ixrc.MinSize;																					 
				lastMaxSize = ixrc.MaxSize;
				changed(this, EventArgs.Empty);
			}																																			   
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),Browsable(true),DefaultValue(false)]
		public override bool AutoSize {
			get {
				return base.AutoSize;
			}
			set {																															   
				base.AutoSize = value;
				if(value) SetAutoSizeLayout();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true), DefaultValue(AutoSizeMode.GrowAndShrink)]
		public AutoSizeMode	 AutoSizeMode {
			get {
				return base.GetAutoSizeMode();
			}
			set {
				if(base.GetAutoSizeMode() == value) return;
				base.SetAutoSizeMode(value);
				SetAutoSizeLayout();
			}
		}
		void SetAutoSizeLayout() {
			if(Parent != null && AutoSize && Root != null && Root.Items != null && Root.Items.Count != 0) {
				switch(Dock) {
					case DockStyle.Bottom:
					case DockStyle.Top:
						if(AutoSizeMode == System.Windows.Forms.AutoSizeMode.GrowAndShrink) Height = Root.MinSize.Height;
						else if(Height < Root.MinSize.Height) Height = Root.MinSize.Height;
						break;
					case DockStyle.Left:
					case DockStyle.Right:
						if(AutoSizeMode == System.Windows.Forms.AutoSizeMode.GrowAndShrink) Width = Root.MinSize.Width;
						else if(Width < Root.MinSize.Width) Width = Root.MinSize.Width;
						break;
					default:
						if(AutoSizeMode == System.Windows.Forms.AutoSizeMode.GrowAndShrink) {
							Height = Root.MinSize.Height;
							Width = Root.MinSize.Width;
						} else {
							if(Height < Root.MinSize.Height) Height = Root.MinSize.Height;
							if(Width < Root.MinSize.Width) Width = Root.MinSize.Width;
						}
						break;
				}
			}																			   
		}				 
		event EventHandler IXtraResizableControl.Changed {
			add {
				base.Events.AddHandler(sizeableChanged, value);
			}
			remove {
				base.Events.RemoveHandler(sizeableChanged, value);
			}
		}
		bool IXtraResizableControl.IsCaptionVisible {
			get { return false; }
		}
		Size IXtraResizableControl.MaxSize {
			get {
				if(Root != null) {
					switch(OptionsView.AutoSizeInLayoutControl) {
						case AutoSizeModes.UseMinAndMaxSize: return Root.MaxSize;
						case AutoSizeModes.ResizeToMinSize: return Root.MinSize;
						case AutoSizeModes.UseMinSizeAndGrow: return new Size(0, 0);
					}
				}
				return new Size(0, 0);
			}
		}
		Size IXtraResizableControl.MinSize {
			get { return Root != null ? Root.MinSize : new Size(0, 0); }
		}
		#endregion
		IDisposable componentPrinterCore;
		protected ComponentPrinterBase ComponentPrinter {
			get {
				if(componentPrinterCore == null) componentPrinterCore = CreateComponentPrinter();
				return (ComponentPrinterBase)componentPrinterCore;
			}
		}
		protected virtual ComponentPrinterBase CreateComponentPrinter() {
			return new ComponentPrinterDynamic(this);
		}
		GestureHelper gestureHelper;
		GestureHelper GestureHelper {
			get {
				if(gestureHelper == null) gestureHelper = new GestureHelper(this);
				gestureHelper.TogglePressAndHold(false);
				return gestureHelper;
			}
		}
		#region IGestureClient Members
		GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) {
			GestureAllowArgs[] res;
			if(Scroller.HScrollVisible || Scroller.VScrollVisible) res = new GestureAllowArgs[] { GestureAllowArgs.Pan };
			else res = new GestureAllowArgs[] { };
			return res;
		}
		IntPtr IGestureClient.Handle {
			get { return IsHandleCreated ? Handle : IntPtr.Zero; }
		}
		void IGestureClient.OnEnd(GestureArgs info) { }
		void IGestureClient.OnBegin(GestureArgs info) {
		}
		void IGestureClient.OnPan(GestureArgs info, Point delta, ref Point overPan) {
			if(info.IsBegin) {
				return;
			}
			if(delta.Y == 0 && delta.X == 0) return;
			if(!this.Scroller.HScrollVisible && !this.Scroller.VScrollVisible) return;
			Scroller.VScrollPos += -delta.Y;
			Scroller.HScrollPos += -delta.X;
		}
		void IGestureClient.OnPressAndTap(GestureArgs info) {
		}
		void IGestureClient.OnRotate(GestureArgs info, Point center, double degreeDelta) {
		}
		void IGestureClient.OnTwoFingerTap(GestureArgs info) {
		}
		void IGestureClient.OnZoom(GestureArgs info, Point center, double zoomDelta) {
		}
		IntPtr IGestureClient.OverPanWindowHandle {
			get { return GestureHelper.FindOverpanWindow(this); }
		}
		Point IGestureClient.PointToClient(Point p) {
			return PointToClient(p);
		}
		#endregion
		bool ILayoutControlOwnerEx.IsLayoutLoading {
			get { return implementorCore.isDeserializingCore || implementorCore.isLayouLoading > 0; }
		}
		event EventHandler ILayoutControlOwnerEx.LayoutLoaded {
			add { implementorCore.LayoutLoaded += value; }
			remove { implementorCore.LayoutLoaded -= value; }
		}
		event TemplateManagerImplementorEventHandler serializeControlCore;
		event TemplateManagerImplementorEventHandler ITemplateManagerImplementor.SerializeControl {
			add { serializeControlCore += value; }
			remove { serializeControlCore -= value; }
		}																										 
		void ITemplateManagerImplementor.RaiseSerializeControl(TemplateManagerImplementorEventArgs e) {
			if(serializeControlCore != null) serializeControlCore(this, e);
		}
		internal LayoutAdorner layoutAdorner;
		protected internal LayoutAdornerWindowHandler layoutAdornerWindowHandler;
		protected virtual bool CanShowAdorner { get { return !RDPHelper.IsRemoteSession && OptionsView.DrawAdornerLayer == DefaultBoolean.True; } }
		internal void InvalidateAdornerWindowHandler() {
			if(!(this as ILayoutControl).EnableCustomizationMode && !DesignMode && layoutAdorner != null) {
				layoutAdorner.Hide(); return;
			}
			if(layoutAdornerWindowHandler != null) layoutAdornerWindowHandler.Invalidate();
		}
		internal void CreateOrDisposeAdorner(DefaultBoolean value) {
			if(value == DefaultBoolean.True) {
				CreateAdornerHandlerAndWindow();
				ShowAdorner();
			} else {
				DisposeAdorner();
			}
		}
		public event EventHandler<ItemDraggingEventArgs> ItemDragging;
		internal bool RaiseOnItemDragging(LayoutItemDragController dragController) {
			if(ItemDragging == null) return true;
			ItemDraggingEventArgs ea = new ItemDraggingEventArgs() { DragController = dragController, AllowDrop = true};
			ItemDragging(this, ea);
			return ea.AllowDrop;
		}
		void IFilteringUIProvider.RetrieveFields(object filteringViewModel, Type filteringViewModelType) {
			LayoutControlItemsCreator creator = new LayoutControlItemsCreator(this) { DataSource = filteringViewModel };
			ElementBindingInfoHelper helper = new ElementBindingInfoHelper();
			creator.CreateLayout(helper.CreateDataLayoutElementsBindingInfo(filteringViewModelType));
		}
	}
	public class ItemDraggingEventArgs :EventArgs {
		public LayoutItemDragController DragController { get; internal set; }
		public bool AllowDrop { get; set; }
	}
}
