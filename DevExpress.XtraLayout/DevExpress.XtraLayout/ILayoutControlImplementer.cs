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
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Controls;
using DevExpress.Utils;
using DXUtils=DevExpress.Utils;
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
using Padding=DevExpress.XtraLayout.Utils.Padding;
using DevExpress.XtraLayout.Accessibility;
using DevExpress.XtraEditors.Container;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.XtraTab.ViewInfo;
using DevExpress.Skins;
using DevExpress.XtraLayout.Localization;
using DevExpress.XtraDashboardLayout;
using System.Security;
using DevExpress.XtraLayout.Customization.UserCustomization;
using DevExpress.Utils.Design;
using System.Linq;
namespace DevExpress.XtraLayout {
	public interface ISupportImplementor {
		LayoutControlImplementor Implementor { get;}
	}
	public interface ILayoutControlOwner : IComponent, ISupportImplementor {
		Size Size { get;set;}
		int Width { get;set;}
		int Height { get;set;}
		int ClientWidth { get;}
		int ClientHeight { get;}
		Rectangle Bounds { get;}
		bool IsDesignMode { get;}
		System.Windows.Forms.Control.ControlCollection Controls { get;}
		Control Parent { get; set; }
		event EventHandler Changed;
		event CancelEventHandler Changing;
		event EventHandler ShowCustomization;
		event EventHandler HideCustomization;
		event EventHandler ItemRemoved;
		event EventHandler ItemAdded;
		event EventHandler ItemSelectionChanged;
		event EventHandler LayoutUpdate;
		event UniqueNameRequestHandler RequestUniqueName;
		event UniqueNameRequestHandler UniqueNameRequest;
		event LayoutGroupCancelEventHandler GroupExpandChanging;
		event LayoutGroupEventHandler GroupExpandChanged;
		event PopupMenuShowingEventHandler PopupMenuShowing;
		event PopupMenuShowingEventHandler LayoutTreeViewPopupMenuShowing;
		void RaiseOwnerEvent_Changed(object sender, EventArgs e);
		void RaiseOwnerEvent_Changing(object sender, CancelEventArgs e);
		void RaiseOwnerEvent_ShowCustomization(object sender, EventArgs e);
		void RaiseOwnerEvent_HideCustomization(object sender, EventArgs e);
		void RaiseOwnerEvent_PropertiesChanged(object sender, EventArgs e);
		void RaiseOwnerEvent_GroupExpandChanging(LayoutGroupCancelEventArgs e);
		void RaiseOwnerEvent_GroupExpandChanged(LayoutGroupEventArgs e);
		void RaiseOwnerEvent_ShowCustomizationMenu(PopupMenuShowingEventArgs ma);
		void RaiseOwnerEvent_ShowLayoutTreeViewContextMenu(PopupMenuShowingEventArgs ma);
		void RaiseOwnerEvent_LayoutUpdate(EventArgs e);
		void RaiseOwnerEvent_ItemSelectionChanged(object sender, EventArgs e);
		void RaiseOwnerEvent_ItemRemoved(object sender, EventArgs e);
		void RaiseOwnerEvent_ItemAdded(object sender, EventArgs e);
		void RaiseOwnerEvent_Updated(object sender, EventArgs e);
		void RaiseOwnerEvent_UniqueNameRequest(object sender, UniqueNameRequestArgs e);
		void RaiseOwnerEvent_LayoutUpgrade(object sender, LayoutUpgradeEventArgs e);
		void RaiseOwnerEvent_CloseButtonClick(object sender, LayoutGroupEventArgs e);
		void RaiseOwnerEvent_BeforeLoadLayout(object sender, LayoutAllowEventArgs e);
		void RaiseOwnerEvent_DefaultLayoutLoading(object sender, EventArgs e);
		void RaiseOwnerEvent_DefaultLayoutLoaded(object sender, EventArgs e);
		RightButtonMenuManager CreateRightButtonMenuManager();
		UserCustomizationForm CreateCustomizationForm();
		LayoutControlCustomizeHandler CreateLayoutControlCustomizeHandler();
		LayoutControlHandler CreateLayoutControlRuntimeHandler();
		ISupportLookAndFeel GetISupportLookAndFeel();
		IStyleController GetIStyleController();
		LayoutControlGroup Root { get; set;}
	}
	public interface ILayoutControlOwnerEx {
		bool IsLayoutLoading { get; }
		event EventHandler LayoutLoaded;
	}
	public class TemplateManagerImplementorEventArgs : EventArgs {
		public object TemplateManager { get; set; }
	}
	public delegate void TemplateManagerImplementorEventHandler(object sender, TemplateManagerImplementorEventArgs e);
	public interface ITemplateManagerImplementor {
		event TemplateManagerImplementorEventHandler SerializeControl;
		void RaiseSerializeControl(TemplateManagerImplementorEventArgs e);
	}
	public class LayoutControlImplementor : ILayoutControl {
		protected ILayoutControlOwner owner;
		protected ILayoutControl castedOwner;
		public LayoutControlImplementor(ILayoutControlOwner owner) {
			this.owner = owner;
			castedOwner = owner as ILayoutControl;
			layoutDesignerMethodProvider = new LayoutDesignerMethodsProvider(this);
			UpdateDefaultValuesCore();
		}
		protected internal ILayoutDesignerMethods layoutDesignerMethodProvider = null;
		protected internal LayoutControlRoles controlsRole = LayoutControlRoles.MainControl;
		protected internal bool fAllowUseGroupCollapsing = true;
		protected internal bool fAllowUseTabbedGroups = true;
		protected internal bool fAllowCreateRootElement = true;
		protected internal bool fAllowUseSplitters = true;
		protected internal bool isDeserializingCore = false, isSerializingCore = false;
		protected internal int isLayouLoading = 0;
		protected internal bool isDiagnosticsDeserializing = false;
		protected internal bool fAllowManageDesignSurfaceComponents = true;
		protected internal bool fEnableCustomizationFormCore = true;
		protected internal bool fAllowCustomizationMenu = true;
		protected internal CustomizationModes fCustomizationMode = CustomizationModes.Default;
		protected internal bool isCustomizationMode = false;
		protected internal int iCloneCounter = 0;
		protected internal bool disposingFlagInternalCore = false;
		protected internal bool initializationFinishedCore = true;
		protected internal bool exceptionsThrownCore = false;
		protected internal bool resizerBrokenCore = false;
		protected internal bool shouldArrangeTextSizeCore = false;
		protected internal bool shouldResizeCore = false;
		protected internal bool shouldUpdateConstraintsCore = false;
		protected internal bool shouldUpdateLookAndFeelCore = true;
		protected internal bool lockUpdateOnChangeUICuesRequestCore = false;
		protected internal bool shouldUpdateControlsLookAndFeelCore = false;
		protected internal bool shouldUpdateControlsCore = false;
		protected internal bool selectedChangedFlagCore = false;
		protected internal int delayPaintingCore = 0;
		protected internal int selectedChangedCountCore = 0;
		protected internal UserCustomizationForm customizationFormCore = null;
		protected internal UndoManager undoManagerCore = null;
		protected internal MemoryStream defaultLayout = null;
		protected internal string layoutVersion = string.Empty;
		protected internal int initializedCount = 0;
		protected internal int changedCounter = 0;
		protected internal int viewInfoResetedCount = 0;
		protected internal int resizesCount = 0;
		protected internal int updatedCountInternal = 0;
		protected internal bool isAutoScroll = true;
		protected internal bool isSizeChanging = false;
		protected internal bool isDesignTesting = false;
		protected internal bool isEndInitCore = false;
		protected internal bool isModifiedCore = false;
		protected internal bool isLayoutModifiedCore = false;
		protected internal bool fNeedRaiseLayoutUpdate = false;
		protected internal IStyleController fStyleController = null;
		protected internal LayoutPaintStyleCollection paintStylesCore = null;
		protected internal Scrolling.ScrollInfo scrollerCore = null;
		protected internal ReadOnlyItemCollection itemsCore = null;
		protected internal HiddenItemsCollection hiddenItemsCore = null;
		protected internal RightButtonMenuManager customizationMenuManagerCore = null;
		protected internal SerializeableUserLookAndFeel lookAndFeelCore = null;
		protected internal LayoutSerializationOptions optionsSerializationCore = null;
		protected internal LayoutAppearanceCollection appearanceCore = null;
		protected internal AppearanceController appearanceControllerCore = null;
		protected internal EnabledStateController enabledStateControllerCore = null;
		protected internal LayoutControlCustomizeHandler customizeHandler = null;
		protected internal LayoutControlHandler handler = null;
		protected internal LayoutEditorContainer layoutEditorContainer = null;
		protected internal FocusHelperBase focusHelperInternal = null;
		protected internal ConstraintsManager constraintsSetter = null;
		protected internal FakeFocusContainer fakeFocusContainerCore = null;
		protected internal TextAlignManager textAlignManagerCore = null;
		protected internal ILayoutAdapter layoutAdapter = null;
		protected internal OptionsView viewOptions = null;
		protected internal OptionsItemText itemTextOptions = null;
		protected internal OptionsFocus focusOptions = null;
		protected internal OptionsCustomizationForm customizationFormOptions = null;
		protected internal IDXMenuManager menuManagerCore = null;
		internal ComponentsUpdateHelper addComponentHelperCore = null;
		internal ComponentsUpdateHelper removeComponentHelperCore = null;
		protected internal List<IComponent> componentsCore = null;
		internal Timer internalTimerCore = null;
		protected internal Dictionary<string, BaseLayoutItem> itemsAndNamesCore = null;
		protected internal Dictionary<Control, LayoutControlItem> controlsAndItemsCore = null;
		protected internal LayoutControlAccessible layoutControlAccessible = null;
		protected internal IComparer hiddenItemsComparer = null;
		protected internal Cursor cursorCore = Cursors.Arrow;
		protected internal object captionImagesCore = null;
		protected internal object imagesCore = null;
		public bool AllowUseGroupCollapsing {
			get { return fAllowUseGroupCollapsing; }
			set { fAllowUseGroupCollapsing = value; }
		}
		public bool AllowUseTabbedGroups {
			get { return fAllowUseTabbedGroups; }
			set { fAllowUseTabbedGroups = value; }
		}
		public bool AllowPaintEmptyRootGroupText { get { return true; } set { } }
		public bool AllowUseSplitters {
			get { return fAllowUseSplitters; }
			set { fAllowUseSplitters = value; }
		}
		public bool AllowCreateRootElement {
			get { return fAllowCreateRootElement; }
			set { fAllowCreateRootElement = value; }
		}
		internal Timer InternalTimer {
			get {return internalTimerCore; }
		}
		public bool DisposingFlagInternal {
			get { return disposingFlagInternalCore; }
			set { disposingFlagInternalCore = value; }
		}
		public virtual void InitializeComponents() {
			if(AllowCreateRootElement) InitializeRootGroupCore();
			InitializeCollections();
			InitializeTimerHandler();
			InitializeScrollerCore(as_I);
			InitializeFakeFocusContainerCore(as_I);
		}
		public virtual void InitializeLookAndFeelCore(Control owner) {
			DisposeLookAndFeelCore();
			lookAndFeelCore = new SerializeableUserLookAndFeel(owner);
			lookAndFeelCore.StyleChanged += new EventHandler(OnLookAndFeelStyleChanged);
		}
		public virtual void DisposeLookAndFeelCore() {
			if (lookAndFeelCore != null) {
				lookAndFeelCore.StyleChanged -= new EventHandler(OnLookAndFeelStyleChanged);
				lookAndFeelCore.Dispose();
				lookAndFeelCore = null;
			}
		}
		public virtual void InitializeAppearanceCore() {
			appearanceCore = new LayoutAppearanceCollection();
			appearanceCore.Changed += new EventHandler(OnAppearanceChanged);
		}
		public virtual void DisposeAppearanceCore() {
			if(DefaultValues != null) {
				if(defaultValuesCore != null) defaultValuesCore.Changed -= new EventHandler(defaultValuesCore_Changed);
				defaultValuesCore = null;
				if(defaultValuesDefaultValueCore != null) defaultValuesDefaultValueCore.Changed -= new EventHandler(defaultValuesCore_Changed);
				defaultValuesDefaultValueCore = null;
			}
			if (appearanceCore != null) {
				appearanceCore.Changed -= new EventHandler(OnAppearanceChanged);
				appearanceCore.Dispose();
				appearanceCore = null;
			}
		}
		public void ClearReferences(BaseLayoutItem item) {
			EnabledStateController.ClearReferences(item);
			AppearanceController.ClearReferences(item);
		}
		public virtual void InitializePaintStylesCore() {
			if (paintStylesCore != null) ClearPaintStyles();
			else paintStylesCore = new LayoutPaintStyleCollection();
			InitializePaintStyles();
		}
		protected void ClearPaintStyles() {
			if (paintStylesCore == null) return;
			foreach (LayoutPaintStyle i in paintStylesCore)
				i.Destroy();
			paintStylesCore.Clear();
		}
		public virtual void DisposePaintStylesCore() {
			ClearPaintStyles();
			paintStylesCore = null;
		}
		public virtual void InitializeFakeFocusContainerCore(ILayoutControl owner) {
			DisposeFakeFocusContainerCore();
			fakeFocusContainerCore = new FakeFocusContainer(owner);
			fakeFocusContainerCore.Size = new Size(0, 0);
			fakeFocusContainerCore.Location = Point.Empty;
			fakeFocusContainerCore.TabStop = false;
			fakeFocusContainerCore.Parent = owner as Control;
			fakeFocusContainerCore.LostFocus += new EventHandler(fakeFocusContainer_LostFocus);
		}
		public virtual void DisposeFakeFocusContainerCore() {
			if (fakeFocusContainerCore != null) {
				fakeFocusContainerCore.LostFocus -= new EventHandler(fakeFocusContainer_LostFocus);
				fakeFocusContainerCore = null;
			}
		}
		[ThreadStatic]
		static MethodInfo beginUpdate, cancelUpdate;
		public virtual void DisposeStyleControllerCore() {
			if(as_I.Control == null || as_I.Control.Controls == null) return;
			int controlsCount = as_I.Control.Controls.Count;
			for(int i = 0; i < controlsCount; i++) {
				ISupportStyleController sc = as_I.Control.Controls[i] as ISupportStyleController;
				if(sc != null) {
					BaseEdit be = as_I.Control.Controls[i] as BaseEdit;
					if(be != null) {
						BindingFlags bf = BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance;
						if(beginUpdate == null) beginUpdate = typeof(BaseEdit).GetMethod("BeginUpdate", bf);
						if(cancelUpdate == null) cancelUpdate = typeof(BaseEdit).GetMethod("CancelUpdate", bf);
						if(beginUpdate != null || cancelUpdate != null) {
							beginUpdate.Invoke(be, new object[] { });
							try {
								sc.StyleController = null;
							}
							finally {
								cancelUpdate.Invoke(be, new object[] { });
							}
						}
					}
					sc.StyleController = null;
				}
			}
			SetStyleController(null);
		}
		public virtual void InitializeScrollerCore(ILayoutControl owner) {
			DisposeScrollerCore();
			scrollerCore = new DevExpress.XtraLayout.Scrolling.ScrollInfo(owner);
			scrollerCore.VScroll.Parent = owner as Control;
			scrollerCore.HScroll.Parent = owner as Control;
			scrollerCore.HScroll.TabStop = false;
			scrollerCore.EmptyArea.Parent = owner as Control;
			scrollerCore.EmptyArea.TabStop = false;
			scrollerCore.VScroll.TabStop = false;
			as_I.Scroller.VScroll.ValueChanged += new EventHandler(OnVScroll);
			as_I.Scroller.HScroll.ValueChanged += new EventHandler(OnHScroll);
			as_I.Scroller.HScrollRange = 10;
			as_I.Scroller.VScrollRange = 10;
			as_I.Scroller.HScrollPos = 0;
			as_I.Scroller.VScrollPos = 0;
		}
		public virtual void DisposeScrollerCore() {
			if (scrollerCore != null) {
				as_I.Scroller.VScroll.ValueChanged -= new EventHandler(OnVScroll);
				as_I.Scroller.HScroll.ValueChanged -= new EventHandler(OnHScroll);
				scrollerCore.Dispose();
				scrollerCore = null;
			}
			textAlignManagerCore = null;
		}
		public void SetRoot(LayoutControlGroup group) { owner.Root = group; }
		public virtual void InitializeRootGroupCore() {
			owner.Root = CreateRoot();
			owner.Root.Owner = as_I;
			as_I.BeginInit();
			owner.Root.BeginInit();
			owner.Root.Spacing = DefaultValues.RootGroupSpacing;
			owner.Root.ExpandButtonVisible = false;
			owner.Root.GroupBordersVisible = false;
			owner.Root.EnableIndentsWithoutBorders = DefaultBoolean.True;
			owner.Root.EndInit();
			as_I.EndInit();
		}
		public virtual void DisposeRootGroupCore() {
			if (owner.Root != null) {
				owner.Root.Dispose();
				owner.Root = null;
			}
		}
		public virtual void DisposeCustomizationFormCore() {
			if (customizationFormCore != null) {
				if (!customizationFormCore.IsDisposed) {
					((ICustomizationContainer)customizationFormCore).UnRegister();
					if (customizationFormCore.LookAndFeel != null) customizationFormCore.LookAndFeel.ParentLookAndFeel = null;
					customizationFormCore.Disposed -= new EventHandler(customizationFormCore_Disposed);
					customizationFormCore.Dispose();
				}
				customizationFormCore = null;
			}
		}
		public virtual void InitializeHiddenItemsCore() {
			hiddenItemsCore = CreateHiddenItemsList();
			hiddenItemsCore.Changed += new CollectionChangeEventHandler(HiddenItemsChanged);
		}
		public virtual void DisposeHiddenItemsCore() {
			if (hiddenItemsCore != null) {
				hiddenItemsCore.Changed -= new CollectionChangeEventHandler(HiddenItemsChanged);
				foreach (IDisposable hitem in hiddenItemsCore)
					hitem.Dispose();
				hiddenItemsCore.DestroyCollection();
				hiddenItemsCore = null;
			}
		}
		protected virtual bool AllowTimer {
			get { return true; }
		}
		public virtual void InitializeTimerHandler() {
			if(AllowTimer) {
				this.internalTimerCore = new Timer();
				InternalTimer.Interval = 300;
				InternalTimer.Tick += OnTimedEvent;
				InternalTimer.Enabled = true;
			}
		}
		public virtual void DestroyTimerHandler() {
			if(AllowTimer && internalTimerCore != null) {
				InternalTimer.Enabled = false;
				InternalTimer.Tick -= OnTimedEvent;
				InternalTimer.Dispose();
				this.internalTimerCore = null;
			}
		}
		public virtual void DisposeUndoManagerCore() {
			if (undoManagerCore != null) {
				undoManagerCore.Dispose();
			}
		}
		public virtual void InitializeCollections() {
			InitializeHiddenItemsCore();
		}
		public virtual void CreateUserInteractionHelper() {
			if(userInteractionHelperCore == null) userInteractionHelperCore = new UserInteractionHelper(this, Control.FindForm());
		}
		public virtual void DisposeUserInteractionHelper() {
			if(userInteractionHelperCore != null) {
				userInteractionHelperCore.Dispose();
				userInteractionHelperCore = null;
			}
		}
		public virtual void DisposeHandlers() {
			if (customizeHandler != null) {
				customizeHandler.Dispose();
				customizeHandler = null;
			}
			if (handler != null) {
				handler.Dispose();
				handler = null;
			}
		}
		internal void HiddenItemsChanged(object sender, CollectionChangeEventArgs args) {
			if (isDeserializingCore) return;
			UpdateHiddenItems();
			Items = null;
		}
		internal void OnHScroll(object sender, EventArgs e) {
			using (new AllowSetIsModifiedHelper(as_I)) {
				LockUndoManager();
				AllowSetIsModified = false;
				as_I.FireChanging(owner);
				RootGroup.Location = new Point(-as_I.Scroller.HScrollPos, RootGroup.Location.Y);
				as_I.Invalidate();
				as_I.Control.Update();
				as_I.FireChanged(owner);
				UnLockUndoManager();
			}
		}
		internal void OnVScroll(object sender, EventArgs e) {
			using (new AllowSetIsModifiedHelper(as_I)) {
				LockUndoManager();	   
				AllowSetIsModified = false;
				as_I.FireChanging(owner);
				RootGroup.Location = new Point(RootGroup.Location.X, -as_I.Scroller.VScrollPos);
				as_I.Invalidate();
				as_I.Control.Update();
				as_I.FireChanged(owner);
				UnLockUndoManager();
			}
		}
		internal void OnTimedEvent(object sender, EventArgs e) {
			if (as_I.IsUpdateLocked) return;
			as_I.ActiveHandler.OnTimer();
		}
		protected virtual HiddenItemsCollection CreateHiddenItemsList() {
			return new HiddenItemsCollection(as_I);
		}
		protected internal void LayoutChanged() {
			ResetAllViewInfo();
			shouldResizeCore = true;
			as_I.Invalidate();
		}
		internal bool IsDirty {
			get {
				return shouldArrangeTextSizeCore | shouldResizeCore | shouldUpdateConstraintsCore | shouldUpdateControlsCore |
					shouldUpdateControlsLookAndFeelCore | shouldUpdateLookAndFeelCore;
			}
		}
		void defaultValuesCore_Changed(object sender, EventArgs e) {
			DoUpdateDefaultValuesChanged();
		}
		protected virtual void DoUpdateDefaultValuesChanged() {
			ShouldUpdateConstraints = true;
			ShouldArrangeTextSize = true;
			ShouldUpdateLookAndFeel = true;
			as_I.TextAlignManager.Reset();
			Invalidate();
		}
		protected Padding GetSkinPadding(Skin skin, string cs) {
			SkinElement el = skin[cs];
			if(el == null) return Padding.Empty;
			SkinElementInfo seli = new SkinElementInfo(el, Rectangle.Empty);
			if(seli == null) return Padding.Empty;
			if(seli.Element == null) return Padding.Empty;
			SkinPaddingEdges edges = seli.Element.ContentMargins;
			if(castedOwner != null && castedOwner.OptionsView.UseParentAutoScaleFactor) edges = seli.Element.ContentMargins.Scale(1f / skin.DpiScaleFactor);
			return new Padding(edges.Left, edges.Right, edges.Top, edges.Bottom);
		}
		protected virtual void UpdateDefaultValuesCore() {
			if(defaultValuesDefaultValueCore != null) defaultValuesDefaultValueCore.Changed -= new EventHandler(OnAppearanceChanged);
			TextAlignManager.Reset();
			defaultValuesDefaultValueCore = new LayoutControlDefaultsPropertyBag(true);
			if(LookAndFeel != null && LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin && as_I.OptionsView.UseSkinIndents) {
				UpdateDefaultValues(defaultValuesDefaultValueCore);
			}
			defaultValuesDefaultValueCore.Changed += new EventHandler(OnAppearanceChanged);
		}
		protected virtual void UpdateDefaultValues(LayoutControlDefaultsPropertyBag defaults) {
			Skin skin = CommonSkins.GetSkin(LookAndFeel);
			defaults.GroupPadding = GetSkinPadding(skin, CommonSkins.SkinLayoutGroupPadding);
			defaults.GroupSpacing = GetSkinPadding(skin, CommonSkins.SkinLayoutGroupSpacing);
			defaults.GroupWithoutBordersPadding = GetSkinPadding(skin, CommonSkins.SkinLayoutGroupWithoutBordersPadding);
			defaults.GroupWithoutBordersSpacing = GetSkinPadding(skin, CommonSkins.SkinLayoutGroupWithoutBordersSpacing);
			defaults.LayoutItemPadding = GetSkinPadding(skin, CommonSkins.SkinLayoutItemPadding);
			defaults.LayoutItemSpacing = GetSkinPadding(skin, CommonSkins.SkinLayoutItemSpacing);
			defaults.RootGroupPadding = GetSkinPadding(skin, CommonSkins.SkinLayoutRootGroupPadding);
			defaults.RootGroupSpacing = GetSkinPadding(skin, CommonSkins.SkinLayoutRootGroupSpacing);
			defaults.RootGroupWithoutBordersPadding = GetSkinPadding(skin, CommonSkins.SkinLayoutRootGroupWithoutBordersPadding);
			defaults.RootGroupWithoutBordersSpacing = GetSkinPadding(skin, CommonSkins.SkinLayoutRootGroupWithoutBordersSpacing);
			defaults.TabbedGroupPadding = GetSkinPadding(skin, CommonSkins.SkinLayoutTabbedGroupPadding);
			defaults.TabbedGroupSpacing = GetSkinPadding(skin, CommonSkins.SkinLayoutTabbedGroupSpacing);
			defaults.TextToControlDistance = GetSkinPadding(skin, CommonSkins.SkinLayoutItemTextToControlDistance);
		}
		LayoutControlDefaultsPropertyBag defaultValuesCore = null, defaultValuesDefaultValueCore;
		public LayoutControlDefaultsPropertyBag DefaultValues {
			get {
				if(defaultValuesCore == null)
					return defaultValuesDefaultValueCore;
				else return defaultValuesCore;
			}
			set {
				if(defaultValuesCore != value) {
					if(defaultValuesCore != null) defaultValuesCore.Changed -= new EventHandler(defaultValuesCore_Changed);
					defaultValuesCore = value;
					if(defaultValuesCore != null) defaultValuesCore.Changed += new EventHandler(defaultValuesCore_Changed);
					DoUpdateDefaultValuesChanged();
				}
			}
		}
		internal void OnAppearanceChanged(object sender, EventArgs e) {
			if (!IsInitialized) return;
			layoutDesignerMethodProvider.BeginChangeUpdate();
			try {
				owner.RaiseOwnerEvent_PropertiesChanged(owner, EventArgs.Empty);
			} finally {
				layoutDesignerMethodProvider.EndChangeUpdate();
			}
			AppearanceController.SetDefaultAppearanceDirty(RootGroup);
			AppearanceController.SetAppearanceDirty(RootGroup);
			LayoutChanged();
		}
		void fakeFocusContainer_LostFocus(object sender, EventArgs e) {
			as_I.FocusHelper.FakeFocusContainerLostFocus();
		}
		protected internal void ScrollBarMouseEnter(object sender, EventArgs e) { }
		protected internal void ScrollBarMouseLeave(object sender, EventArgs e) { }
		public void SetStyleController(IStyleController sController) {
			if(fStyleController == sController || sController == owner as IStyleController) return;
			if (fStyleController != null) UnsubscribeStyleController();
			fStyleController = sController;
			if (fStyleController != null) SubscribeStyleController();
			if (!DisposingFlag) fStyleController_PropertiesChanged(null, null);
		}
		protected void UnsubscribeStyleController() {
			fStyleController.PropertiesChanged -= new EventHandler(fStyleController_PropertiesChanged);
		}
		protected void SubscribeStyleController() {
			fStyleController.PropertiesChanged += new EventHandler(fStyleController_PropertiesChanged);
		}
		void fStyleController_PropertiesChanged(object sender, EventArgs e) {
			if (IsInitialized) {
				FireChanging(owner);
				styleControllerLookAndFeelCore = null;
				OnLookAndFeelStyleChanged(null, null);
				FireChanged(owner);
			}
		}
		LayoutAppearanceCollection CreateAppearanceCollection() { return new LayoutAppearanceCollection(); }
		SerializeableUserLookAndFeel CreateLookAndFill() { return new SerializeableUserLookAndFeel(as_I.Control); }
		LayoutPaintStyleCollection CreatePaintStyle() { return new LayoutPaintStyleCollection(); }
		void ResetEnabledStateController() {
			if (IsInitialized) {
				as_I.EnabledStateController.SetItemEnabledStateDirty(RootGroup);
				as_I.ShouldUpdateControlsLookAndFeel = true;
			}
		}
		public virtual void SaveLayoutCore(XtraSerializer serializer, object path, OptionsLayoutBase options) {
			StartStoreRestore(true);
			Stream stream = path as Stream;
			if(stream != null)
				serializer.SerializeObject(owner, stream, owner.GetType().Name, options);
			else
				serializer.SerializeObject(owner, path.ToString(), owner.GetType().Name, options);
			EndStoreRestore(true);
		}
		public virtual void RestoreLayoutCore(XtraSerializer serializer, object path, OptionsLayoutBase options) {
			StartStoreRestore(false);
			Stream stream = path as Stream;
			if(stream != null)
				serializer.DeserializeObject(owner, stream, owner.GetType().Name, options);
			else
				serializer.DeserializeObject(owner, path.ToString(), owner.GetType().Name, options);
			EndStoreRestore(false);
		}
		public bool ShowKeyboardCues { get { return true; } }
		public virtual void SaveLayoutCore(XtraSerializer serializer, object path) {
			StartStoreRestore(true);
			Stream stream = path as Stream;
			if (stream != null)
				serializer.SerializeObject(owner, stream, owner.GetType().Name);
			else
				serializer.SerializeObject(owner, path.ToString(), owner.GetType().Name);
			EndStoreRestore(true);
		}
		public virtual void RestoreLayoutCore(XtraSerializer serializer, object path) {
			StartStoreRestore(false);
			Stream stream = path as Stream;
			if (stream != null)
				serializer.DeserializeObject(owner, stream, owner.GetType().Name);
			else
				serializer.DeserializeObject(owner, path.ToString(), owner.GetType().Name);
			EndStoreRestore(false);
		}
		protected virtual void StartStoreRestore(bool isStoring) { }
		protected virtual void EndStoreRestore(bool isStoring) {
			if(!isStoring) {
				ShouldUpdateConstraints = true;
				ShouldUpdateControls = true;
				Invalidate();
				RaiseLayoutLoaded();
			}
		}
		protected void RaiseLayoutLoaded() {
			if(LayoutLoaded != null) LayoutLoaded(owner, EventArgs.Empty);
		}
		protected internal event EventHandler LayoutLoaded;
		protected bool IsInitialized { get { return initializedCount == 0 && as_I.InitializationFinished; } }
		protected virtual void EndInitCore() {
			if (RootGroup == null || isEndInitCore || as_I.InitializationFinished) return;
			isEndInitCore = true;
			SetParentsAndOwners();
			as_I.Invalidate();
			isEndInitCore = false;
			as_I.InitializationFinished = true;
			OnLookAndFeelStyleChanged(owner, EventArgs.Empty);
			ProcessSizeChangedCore();
			UpdateHiddenItems();
			UpdateSplitterItemsLayout();
			as_I.SetIsModified(false);
		}
		protected void SetParentsAndOwners() {
			SetParentsAndOwners(RootGroup);
		}
		protected void SetParentsAndOwners(LayoutGroup group) {
			if (group != null) {
				ParentsAndOwnersSetter setter = new ParentsAndOwnersSetter(castedOwner);
				group.Accept(setter);
			}
		}
		bool isProcessLookAndFeelStyleChangedInvoked = false;
		public virtual void OnLookAndFeelStyleChanged(object sender, EventArgs e) {
			if(Control != null && Control.IsHandleCreated && !isProcessLookAndFeelStyleChangedInvoked && !DesignMode && 
				(HasUndoManager && !UndoManager.IsUndoLocked)) {
				isProcessLookAndFeelStyleChangedInvoked = true;
				Control.BeginInvoke(new MethodInvoker(ProcessLookAndFeelStyleChanged));
			}
			else ProcessLookAndFeelStyleChanged();
		}
		protected virtual void ProcessLookAndFeelStyleChanged() {
			ResetSkinElements();
			UpdateDefaultValuesCore();
			isProcessLookAndFeelStyleChangedInvoked = false;
			if (!IsInitialized) return;
			try {
				layoutDesignerMethodProvider.BeginChangeUpdate();
				ReInitializePaintStyles();
				UpdateControlsLookAndFeel();
				ResetAllViewInfo();
			} finally {
				layoutDesignerMethodProvider.EndChangeUpdate();
				RaiseSizeableChanged();
			}
		}
		void UpdateControlsLookAndFeel() {
			try {
				for (int i = owner.Controls.Count - 1; i >= 0; i--) {
					OnChildControlAdded(owner.Controls[i]);
				}
			} finally {
				as_I.AppearanceController.SetDefaultAppearanceDirty(RootGroup);
				if (CanRaisePropertiesChanged()) owner.RaiseOwnerEvent_PropertiesChanged(owner, EventArgs.Empty);
			}
		}
		SkinElement groupSkinElement;
		SkinElement tabSkinElement;
		protected virtual void EnsureSkinElements() {
			if(LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin) return;
			if(groupSkinElement == null)
				groupSkinElement = CommonSkins.GetSkin(LookAndFeel)[CommonSkins.SkinGroupPanel];
			if(tabSkinElement == null)
				tabSkinElement = TabSkins.GetSkin(LookAndFeel)[TabSkins.SkinTabPane];
		}
		protected virtual void ResetSkinElements() {
			groupSkinElement = tabSkinElement = null;
		}
		public Color GetEmptyBackColor(BaseLayoutItem item, Color ownerBackColor) {
			if(item == null) return ownerBackColor;
			EnsureSkinElements();
			Color result = ownerBackColor;
			while(true) {
				if(item.Parent == null) {
					if(result == ownerBackColor)	
						result = GetControlEmptyBackColor();
					break;
				}
				if(item.Parent.ParentTabbedGroup != null) {
					result = GetTabEmptyBackColor();
					break;
				}
				if(IsGroupElementVisible(item.Parent)) {
					result = GetGroupEmptyBackColor();
					break;
				}
				item = item.Parent;
			}
			return result;
		}
		public Color GetForeColor(BaseLayoutItem item, Color ownerForeColor) {
			if(item == null) return ownerForeColor;
			EnsureSkinElements();
			Color result = ownerForeColor;
			while(true) {
				if(item.Parent == null) {
					if(result == ownerForeColor)	
						result = GetControlColor(item);
					break;
				}
				if(item.Parent.ParentTabbedGroup != null) {
					result = GetTabColor();
					break;
				}
				if(IsGroupElementVisible(item.Parent)) {
					result = GetGroupColor(item.Parent);
					break;
				}
				item = item.Parent;
			}
			return result;
		}
		Color GetSkinBackColor(SkinColor color) {
			Color solidColor = color.GetSolidImageCenterColor();
			if(solidColor != Color.Empty && solidColor != Color.Transparent) 
				return solidColor;
			Color backColor = color.GetBackColor();
			if(backColor != Color.Empty && backColor != Color.Transparent && backColor != SkinElementPainter.DefaultColor)
				return backColor;
			return Color.Empty;
		}
		protected virtual Color GetTabColor() {
			return (tabSkinElement != null) ?
				tabSkinElement.Color.GetForeColor() :
				LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.ControlText);
		}
		protected virtual Color GetTabEmptyBackColor() {
			return (tabSkinElement != null) ?
				GetSkinBackColor(tabSkinElement.Color) :
				LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Control);
		}
		protected virtual Color GetGroupColor(LayoutGroup group) {
			return (groupSkinElement != null) ?
				groupSkinElement.Color.GetForeColor() :
				LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.ControlText);
		}
		protected virtual Color GetGroupEmptyBackColor() {
			return (groupSkinElement != null) ?
				GetSkinBackColor(groupSkinElement.Color) :
				LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Control);
		}
		protected virtual Color GetControlEmptyBackColor() {
			return LookAndFeelHelper.GetEmptyBackColor(LookAndFeel, Control);
		}
		protected virtual Color GetControlColor(BaseLayoutItem item) {
			return LookAndFeelHelper.GetTransparentForeColor(LookAndFeel, Control);
		}
		protected virtual bool IsGroupElementVisible(LayoutGroup group) {
			return group.GroupBordersVisible;
		}
		public Color GetForeColor(Control childControl, Color ownerForeColor) {
			return GetForeColor(GetItemByControl(childControl), ownerForeColor);
		}
		public Color GetEmptyBackColor(Control childControl, Color ownerBackColor) {
			return GetEmptyBackColor(GetItemByControl(childControl), ownerBackColor);
		}
		protected bool CanRaisePropertiesChanged() {
			Form form = (Control == null) ? null : Control.FindForm();
			if (form == null) return false;
			return form.IsHandleCreated;
		}
		protected virtual void OnChildControlAdded(Control control) {
			if (as_I.IsUpdateLocked) { as_I.ShouldUpdateControlsLookAndFeel = true; return; }
			SetControlsLookAndFeel(control, owner.GetIStyleController());
		}
		internal void SetControlsLookAndFeel(Control control, IStyleController styleController) {
			if (control == null) return;
			ISupportLookAndFeel lf = control as ISupportLookAndFeel;
			ISupportStyleController sc = control as ISupportStyleController;
			if(scrollerCore != null) {
				if((control == scrollerCore.HScroll) || (control == scrollerCore.VScroll)) {
					if(lf != null && lf.LookAndFeel != null) lf.LookAndFeel.ParentLookAndFeel = as_I.LookAndFeel.ActiveLookAndFeel;
					return;
				}
			}
			if (as_I.OptionsView.ShareLookAndFeelWithChildren) {
				if (lf != null && lf.LookAndFeel != null) lf.LookAndFeel.ParentLookAndFeel = as_I.LookAndFeel;
				if (sc != null) SetStyleController(sc, styleController);
			} else {
				if (lf != null && lf.LookAndFeel != null) lf.LookAndFeel.ParentLookAndFeel = null;
				if (sc != null && sc.StyleController == styleController) SetStyleController(sc, null);
			}
		}
		void SetStyleController(ISupportStyleController element, IStyleController controller) {
			BaseEdit baseEdit = element as BaseEdit;
			if(baseEdit != null) {
				SetControllerCore(element, controller);
			} else
				SetControllerCore(element, controller);
		}
		void SetControllerCore(ISupportStyleController element, IStyleController controller) {
			if((element.StyleController != controller && controller == null) || DesignMode) {
				element.StyleController = controller;
			}
		}
		void OnStyleController_PropertiesChanged(object sender, EventArgs e) {
			OnStyleControllerChanged();
		}
		void OnStyleControllerChanged() {
			OnLookAndFeelStyleChanged(owner, EventArgs.Empty);
		}
		void OnStyleController_Disposed(object sender, EventArgs e) {
			StyleController = null;
		}
		public virtual ToolTipControlInfo GetLayoutControlItemToolTipInfo(Point pt, LayoutControlItem citem) {
			Rectangle tooltipTextRect = Rectangle.Empty;
			Rectangle tooltipIconRect = Rectangle.Empty;
			if(citem != null) {
				tooltipTextRect = citem.ViewInfo.LabelInfo.TextRect;
				tooltipIconRect = citem.ViewInfo.LabelInfo.ImageBounds;
			}
			return GetToolTipControlInfo(pt, citem, tooltipTextRect, tooltipIconRect);
		}
		public virtual ToolTipControlInfo GetItemToolTipInfo(Point pt, BaseLayoutItemHitInfo hi) {
			if(hi == null) return null;
			BaseLayoutItem item = hi.Item;
			BaseLayoutItem tooltipOwner = item;
			Rectangle tooltipTextRect = Rectangle.Empty;
			Rectangle tooltipIconRect = Rectangle.Empty;
			LayoutControlItem citem = item as LayoutControlItem;
			if(citem != null) {
				tooltipTextRect = citem.ViewInfo.LabelInfo.TextRect;
				tooltipIconRect = citem.ViewInfo.LabelInfo.ImageBounds;
			}
			LayoutControlGroup group = item as LayoutControlGroup;
			if(group != null) {
				tooltipTextRect = group.ViewInfo.BorderInfo.TextBounds;
				tooltipIconRect = group.ViewInfo.BorderInfo.CaptionImageBounds;
			}
			DashboardLayoutControlItemBase dashboard = item as DashboardLayoutControlItemBase;
			if(dashboard != null && dashboard.ViewInfo is DashboardLayoutControlItemViewInfo) {
				var viewInfo = dashboard.ViewInfo as DashboardLayoutControlItemViewInfo;
				tooltipTextRect = viewInfo.BorderInfo.TextBounds;
				tooltipIconRect = viewInfo.BorderInfo.CaptionImageBounds;
			}
			DashboardLayoutControlGroupBase dashboardgroup = item as DashboardLayoutControlGroupBase;
			if(dashboardgroup != null && dashboardgroup.ViewInfo is DashboardLayoutControlGroupViewInfo) {
				var viewInfo = dashboardgroup.ViewInfo as DashboardLayoutControlGroupViewInfo;
				tooltipTextRect = viewInfo.BorderInfo.TextBounds;
				tooltipIconRect = viewInfo.BorderInfo.CaptionImageBounds;
			}
			TabbedControlGroup tabGroup = item as TabbedControlGroup;
			if(tabGroup != null) {
				BaseTabControlViewInfo tabInfo = tabGroup.ViewInfo.BorderInfo.Tab.ViewInfo;
				if(tabInfo != null && (hi.TabPageIndex != -1)) {
					int index = hi.TabPageIndex - tabInfo.FirstVisiblePageIndex;
					if(index >= 0 && index < tabInfo.HeaderInfo.VisiblePages.Count) {
						tooltipTextRect = tabInfo.HeaderInfo.VisiblePages[index].Text;
						tooltipIconRect = tabInfo.HeaderInfo.VisiblePages[index].Image;
						tooltipOwner = tabGroup.VisibleTabPages[hi.TabPageIndex];
					}
				}
			}
			return GetToolTipControlInfo(pt, tooltipOwner, tooltipTextRect, tooltipIconRect);
		}
		protected ToolTipControlInfo GetToolTipControlInfo(Point pt, BaseLayoutItem item, Rectangle text, Rectangle icon) {
			if(item == null) return null;
			ToolTipControlInfo resultToolTipInfo = null;
			bool pointInTextArea = !text.IsEmpty && text.Contains(pt);
			bool pointInIconArea = !icon.IsEmpty && icon.Contains(pt);
			ToolTipControlInfo iconToolTipInfo = CreateIconToolTipControlInfoByOptions(item, item.OptionsToolTip);
			ToolTipControlInfo textToolTipInfo = CreateLabelToolTipControlInfoByOptions(item, item.OptionsToolTip);
			if(pointInTextArea) resultToolTipInfo = textToolTipInfo;
			if(pointInIconArea) resultToolTipInfo = (iconToolTipInfo == null) ? textToolTipInfo : iconToolTipInfo;
			return resultToolTipInfo;
		}
		protected ToolTipControlInfo CreateIconToolTipControlInfoByOptions(BaseLayoutItem item, BaseLayoutItemOptionsToolTip options) {
			return options.CanShowIconToolTip() ? new ToolTipControlInfo(item, options.IconToolTip, options.IconToolTipTitle,options.IconImmediateToolTip, options.IconToolTipIconType,options.IconAllowHtmlString) : null;
		}
		protected internal ToolTipControlInfo CreateLabelToolTipControlInfoByOptions(BaseLayoutItem item, BaseLayoutItemOptionsToolTip options) {
			return options.CanShowToolTip() ? new ToolTipControlInfo(item, options.ToolTip, options.ToolTipTitle, options.ImmediateToolTip,options.ToolTipIconType, options.AllowHtmlString) : null;
		}
		internal void ProcessSizeChangedCore() {
			try {
				as_I.FireChanging(owner);
				CheckGroupBounds();
				as_I.Invalidate();
			} finally {
				as_I.FireChanged(owner);
			}
		}
		bool CheckGroupBounds() {
			Scrolling.ScrollInfo scrollerCore = as_I.Scroller;
			if(scrollerCore == null || RootGroup == null) return false;
			scrollerCore.ClientRect = new Rectangle(Point.Empty, new Size(Width, Height));
			bool result = false;
			if(!RootGroup.IsUpdateLocked && IsInitialized) {
				if(isAutoScroll) {
					Size realSize = new System.Drawing.Size(RootGroup.MinSize.Width, OptionsView.FitControlsToDisplayAreaHeight ? RootGroup.MinSize.Height : RootGroup.Size.Height);
					result = UpdateScrollsVisibility(realSize);
					UpdatePreferredSizeByClientRect();
					if(WindowsFormsSettings.GetIsRightToLeft(Control)){
						RootGroup.Location = new Point(
							(ClientWidth - RootGroup.Width) + scrollerCore.HScrollPos , 
							-scrollerCore.VScrollPos);
					}
					else{
					RootGroup.Location = new Point(-scrollerCore.HScrollPos, -scrollerCore.VScrollPos);
					}
					if(RootGroup.Location.Y < -(RootGroup.Height + (scrollerCore.HScrollVisible ? scrollerCore.HScroll.Bounds.Height : 0) - Height)) 
						RootGroup.Location = new Point(RootGroup.Location.X,
							(!OptionsView.FitControlsToDisplayAreaHeight && !scrollerCore.VScrollVisible && RootGroup.Height < ClientHeight) ? 0 : -(RootGroup.Height - ClientHeight));
					if(RootGroup.Location.X < -(RootGroup.Width + (scrollerCore.VScrollVisible ? scrollerCore.VScroll.Bounds.Width : 0) - Width))
						RootGroup.Location = new Point(-(RootGroup.Width - ClientWidth), RootGroup.Location.Y);
				}
				else {
					result = UpdateScrolls(false, false, RootGroup.Size);
					UpdatePreferredSizeByClientRect();																				  
				}
				if(result) {																								
					as_I.ShouldUpdateLookAndFeel = true;
					as_I.Invalidate();
					return true;
				}
			}
			else {
				RootGroup.PreferredSize = new Size(ClientWidth, ClientHeight);
				RootGroup.PreferredSizeDirty = true;
			}
			return false;
		}
		void UpdatePreferredSizeByClientRect() {
			Size sz = ClientSizeCore;
			if(!OptionsView.FitControlsToDisplayAreaHeight) sz.Height = RootGroup.Height;
			if(sz.Width < RootGroup.MinSize.Width) sz.Width = RootGroup.MinSize.Width;
			if(sz.Height < RootGroup.MinSize.Height) sz.Height = RootGroup.MinSize.Height;
			RootGroup.PreferredSize = sz;
			if((RootGroup.PreferredSize != RootGroup.Size) || RootGroup.PreferredSizeDirty) {
				RootGroup.Resizer.SizeIt(RootGroup.PreferredSize);
				RootGroup.PreferredSizeDirty = false;
			}
		}
		bool UpdateScrollsVisibility(Size realSize) {
			Scrolling.ScrollInfo scrollerCore = as_I.Scroller;
			if(Width >= realSize.Width && Height >= realSize.Height) {
				return UpdateScrolls(false, false, realSize);
			}
			if(Width < realSize.Width && (Height - scrollerCore.ScrollSize) < realSize.Height) {
				return UpdateScrolls(true, true, realSize);
			}
			if(Height < realSize.Height && (Width - scrollerCore.ScrollSize) < realSize.Width) {
				return UpdateScrolls(true, true, realSize);
			}
			if(Width < realSize.Width && Height > realSize.Height) {
				return UpdateScrolls(true, false, realSize);
			}
			if(Width >= realSize.Width && Height < realSize.Height) {
				return UpdateScrolls(false, true, realSize);
			}
			if(Width < realSize.Width && Height < realSize.Height) {
				return UpdateScrolls(true, true, realSize);
			}
			return false;
		}
		Size ClientSizeCore {
			get {
				return new Size(
			(Width - (as_I.Scroller.VScroll.Visible ? as_I.Scroller.ScrollSize : 0)),
			(Height - (as_I.Scroller.HScroll.Visible ? as_I.Scroller.ScrollSize : 0)));
			}
		}
		bool UpdateScrolls(bool hScrollVisible, bool vScrollVisible, Size realSize) {
			bool result = false;
			Scrolling.ScrollInfo scrollerCore = as_I.Scroller;
			if (scrollerCore.HScrollVisible != hScrollVisible) { scrollerCore.HScrollPos = 0; result = true; }
			if (scrollerCore.VScrollVisible != vScrollVisible) { scrollerCore.VScrollPos = 0; result = true; }
			scrollerCore.HScrollVisible = hScrollVisible;
			scrollerCore.UpdateClientRect(new Rectangle(Point.Empty, Size));
			scrollerCore.VScrollVisible = vScrollVisible;
			scrollerCore.UpdateClientRect(new Rectangle(Point.Empty, Size));
			if (hScrollVisible) {
				scrollerCore.HScroll.LargeChange = ClientSizeCore.Width;
				scrollerCore.HScroll.Maximum = realSize.Width;
				scrollerCore.HScroll.SmallChange = scrollerCore.HScroll.LargeChange / 10;
			} else {
				scrollerCore.HScrollRange = 0;
				scrollerCore.HScroll.LargeChange = 1;
			}
			if (vScrollVisible) {
				scrollerCore.VScroll.LargeChange = ClientSizeCore.Height;
				scrollerCore.VScroll.SmallChange = scrollerCore.VScroll.LargeChange / 10;
				scrollerCore.VScroll.Maximum = realSize.Height;
			} else {
				scrollerCore.VScrollRange = 0;
				scrollerCore.VScroll.LargeChange = 1;
			}
			return result;
		}
		void UpdateSplitterItemsLayout() {
			SplitterItemLayoutTypeUpdater updater = new SplitterItemLayoutTypeUpdater(as_I);
			RootGroup.Accept(updater);
		}
		protected virtual void InitializePaintStyles() {
			ISupportLookAndFeel lookAndFeelOwner = owner.GetISupportLookAndFeel();
			if (lookAndFeelOwner != null) {
				PaintStyles.Add(new LayoutOffice2003PaintStyle(lookAndFeelOwner));
				PaintStyles.Add(new LayoutWindowsXPPaintStyle(lookAndFeelOwner));
				PaintStyles.Add(new LayoutSkinPaintStyle(lookAndFeelOwner));
				PaintStyles.Add(new Style3DPaintStyle(lookAndFeelOwner));
				PaintStyles.Add(new UltraFlatPaintStyle(lookAndFeelOwner));
				PaintStyles.Add(new FlatPaintStyle(lookAndFeelOwner));
			}
			lookAndFeelOwner = null;
		}
		protected void ReInitializePaintStyles() {
			ClearPaintStyles();
			InitializePaintStyles();
		}
		protected internal void ResetAllViewInfo() {
			ResetAllViewInfo(RootGroup);
		}
		protected internal void ResetAllViewInfo(LayoutGroup group) {
			if (group != null) {
				ViewInfoResetHelper resetter = new ViewInfoResetHelper();
				group.Accept(resetter);
			}
		}
		public LayoutPaintStyleCollection PaintStyles { get { return paintStylesCore; } }
		public void AssignNames() {
			int i = 0;
			FillItemsInternal();
			foreach (BaseLayoutItem item in Items) {
				if (item != RootGroup)
					item.Name = (i++).ToString();
			}																															   
			Items = null;
		}
		void FillItemsInternal() {
			if(RootGroup == null) return;
			FillItemsHelper helper = new FillItemsHelper(as_I);
			RootGroup.Accept(helper);
			itemsCore = helper.Items;
			if(as_I.HiddenItems != null)
				foreach(BaseLayoutItem item in as_I.HiddenItems) {
					if(item is EmptySpaceItem) continue;
					item.Accept(helper);
				}
		}
		void CheckForSameNames() {
			if (RootGroup == null) return;
			Dictionary<string, BaseLayoutItem> itemTable = new Dictionary<string, BaseLayoutItem>();
			foreach (BaseLayoutItem item in Items) {
				if (!itemTable.ContainsKey(item.Name)) itemTable.Add(item.Name, item);
				else item.Name = item.GetDefaultText();
			}
		}
		IStyleController StyleController {
			get { return fStyleController; }
			set {
				if (StyleController == value || value == owner) return;
				as_I.FireChanging(owner);
				if (StyleController != null) {
					StyleController.PropertiesChanged -= new EventHandler(OnStyleController_PropertiesChanged);
					StyleController.Disposed -= new EventHandler(OnStyleController_Disposed);
				}
				fStyleController = value;
				if (StyleController != null) {
					StyleController.PropertiesChanged += new EventHandler(OnStyleController_PropertiesChanged);
					StyleController.Disposed += new EventHandler(OnStyleController_Disposed);
				}
				OnStyleControllerChanged();
				as_I.FireChanged(owner);
			}
		}
		internal LayoutPaintStyle ActiveStyle { get { return PaintStyles[as_I.LookAndFeel.ActiveLookAndFeel]; } }
		internal LayoutControlHandler CustomizeHandler {
			get {
				if (customizeHandler == null) customizeHandler = owner.CreateLayoutControlCustomizeHandler();
				return customizeHandler;
			}
		}
		internal LayoutControlHandler RuntimeHandler {
			get {
				if (handler == null) handler = owner.CreateLayoutControlRuntimeHandler();
				return handler;
			}
			set { handler = value; }
		}
		FocusHelperBase CreateFocusHelper() { return new FocusHelperBase(as_I); }
		protected virtual void OnSelectionChanged(object sender, EventArgs e) {
			if (as_I.ActiveHandler != null && sender != null && as_I.ActiveHandler.LastSelectedItem != null) {
				as_I.ActiveHandler.LastSelectedItem = ((BaseLayoutItem)sender).Parent;
			}
			as_I.Control.Invalidate();
		}
		protected virtual ComponentsUpdateHelper CreateAddComponentHelper() {
			return new ComponentsUpdateHelper(ComponentsUpdateHelperRoles.Add, as_I);
		}
		protected virtual ComponentsUpdateHelper CreateRemoveComponentHelper() {
			return new ComponentsUpdateHelper(ComponentsUpdateHelperRoles.Remove, as_I);
		}
		public ComponentsUpdateHelper AddComponentHelper {
			get {
				if (addComponentHelperCore == null) addComponentHelperCore = CreateAddComponentHelper();
				return addComponentHelperCore;
			}
		}
		public ComponentsUpdateHelper RemoveComponentHelper {
			get {
				if (removeComponentHelperCore == null)
					removeComponentHelperCore = CreateRemoveComponentHelper();
				return removeComponentHelperCore;
			}
		}
		protected internal void AlignTextWidth() {
			as_I.TextAlignManager.Reset();
		}
		void UpdateRoot() {
			if (RootGroup.ViewInfo.Offset != RootGroup.Location) shouldUpdateControlsCore = true;
			RootGroup.ViewInfo.Offset = RootGroup.Location;
			RootGroup.UpdateChildren(RootGroup.Expanded && RootGroup.ActualItemVisibility);
		}
		bool DoGroupChanged() {
			try {
				LockUndoManager();
				return CheckGroupBounds();
			} finally { UnLockUndoManager(); }
		}
		protected virtual void UpdateControls() {
			if (RootGroup == null) return;
			UpdateControlsHelper helper = new UpdateControlsHelper();
			RootGroup.Accept(helper);
		}
		internal BaseAccessible DXAccessible {
			get {
				if (layoutControlAccessible == null) layoutControlAccessible = CreateAccessibleInstance();
				return layoutControlAccessible;
			}
		}
		LayoutControlAccessible CreateAccessibleInstance() {
			return new LayoutControlAccessible(as_I);
		}
		void InitializeGroup() {
			if (RootGroup == null) RootGroup = CreateRoot();
			as_I.AddComponent(RootGroup);
			RootGroup.Owner = as_I;
			as_I.BeginInit();
			RootGroup.BeginInit();
			RootGroup.Spacing = DefaultValues.RootGroupSpacing;
			RootGroup.ExpandButtonVisible = false;
			RootGroup.TextVisible = false;
			RootGroup.EndInit();
			as_I.EndInit();
		}
		protected virtual LayoutControlGroup CreateRoot() { return (LayoutControlGroup)CreateLayoutGroup(null); }
		protected virtual void CreateCustomizationFormCore() {
			if (customizationFormCore == null && as_I.EnableCustomizationForm) {
				customizationFormCore = owner.CreateCustomizationForm();
				customizationFormCore.Disposed += new EventHandler(customizationFormCore_Disposed);
				customizationFormCore.CreateCustomization(CalcFormPosition(), false);
				customizationFormCore.LookAndFeel.ParentLookAndFeel = as_I.LookAndFeel;
			}
		}
		void customizationFormCore_Disposed(object sender, EventArgs e) {
			DisposeCustomizationFormCore();
		}
		protected virtual Point CalcFormPosition() {
			Point pos = Point.Empty;
			if (customizationFormCore == null) return pos;
			if (castedOwner == null) return pos;
			pos.X = castedOwner.Bounds.Width - customizationFormCore.Width;
			if (pos.X < castedOwner.Bounds.Width / 2) pos.X = castedOwner.Bounds.Width;
			pos.Y = castedOwner.Bounds.Height - customizationFormCore.Height;
			if (pos.X < castedOwner.Bounds.Height / 2) pos.Y = castedOwner.Bounds.Height;
			pos = castedOwner.Control.PointToScreen(pos);
			if (DesignMode && OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize.HasValue) {
				customizationFormCore.Size = OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize.Value.Size;
				pos = OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize.Value.Location;
				pos = DevExpress.Utils.ControlUtils.CalcLocation(pos, pos, Size);
				return pos;
			}
			Rectangle ownerBounds = owner.Bounds;
			pos = DevExpress.Utils.ControlUtils.CalcLocation(pos, pos, Size);
			CheckCustomizatioFormLocation(ref pos, ref ownerBounds);
			return pos;
		}
		private void CheckCustomizatioFormLocation(ref Point pos, ref Rectangle ownerBounds) {
			if (pos.X < 0 || pos.Y < 0) {
				pos = castedOwner.Control.PointToScreen(new Point(ownerBounds.Right, ownerBounds.Bottom));
				pos.Offset(-ownerBounds.Width, -ownerBounds.Height);
			}
		}
		internal LayoutControlItem GetItemByControl(Control control, LayoutControlGroup group) {
			if (control != null && group != null) {
				ItemByControlSearcher searcher = new ItemByControlSearcher(control);
				group.Accept(searcher);
				return searcher.ResultItem;
			}
			return null;
		}
		protected void RaiseUniqueNameRequest(UniqueNameRequestArgs e) {
			owner.RaiseOwnerEvent_UniqueNameRequest(owner, e);
		}
		protected virtual string GetUniqueNameInternal(ArrayList names) {
			string name;
			for (int i = 0; i <= Items.Count + 1; i++) {
				name = "item" + i.ToString();
				if (!names.Contains(name)) {
					return name;
				}
			}
			throw new LayoutControlInternalException("Auto set names failed");
		}
		protected virtual void AssignUniqueName(BaseLayoutItem item, Hashtable table) {
			ArrayList names = new ArrayList(table.Keys);
			owner.RaiseOwnerEvent_UniqueNameRequest(owner, new UniqueNameRequestArgs(item, names));
			if (item.Name == "" || names.Contains(item.Name))
				item.Name = GetUniqueNameInternal(names);
		}
		internal static string GetControlName(Control control) {
			if (control == null) return string.Empty;
			if (control.Site != null && control.Site.Name != null) {
				return control.Site.Name;
			}
			return control.Name;
		}
		public virtual Control GetControlByName(String name) {
			foreach (Control control in owner.Controls) {
				if (GetControlName(control) == name) return control;
			}
			return null;
		}
		protected virtual void UpdateNames() {
			foreach (BaseLayoutItem item in Items) {
				FillItemsHelper.UpdateName(item, this);
				TabbedGroup tgroup = item as TabbedGroup;
				LayoutControlItem citem = item as LayoutControlItem;
				if (citem != null) {
					if (citem.Control != null) {
						citem.ControlName = GetControlName(citem.Control);
					}
				}
				if (tgroup != null) {
					if (tgroup.SelectedTabPage != null) {
						tgroup.SelectedTabPageName = tgroup.SelectedTabPage.Name;
					}
				}
			}
		}
		bool needNamesUpdatingOnce = true;
		public virtual void CheckNames() {
			int i = 0;
			Hashtable itemNames = new Hashtable();
			bool needNamesUpdating = needNamesUpdatingOnce;
			IOrderedEnumerable<BaseLayoutItem> sortedItems = Items.OrderBy(e => e.Name == string.Empty);
			foreach (BaseLayoutItem item in sortedItems) {
				i++;
				string rootName = CreateRootName();
				item.BeginInit();
				if (item == RootGroup && item.Name != rootName) item.Name = rootName;
				if(HiddenItems.Contains(item)) 
					item.SetCustomizationParentName();
				item.EndInit();
				if (item.Name == "") {
					AssignUniqueName(item, itemNames);
					needNamesUpdating = true;
				}
				if (!itemNames.Contains(item.Name))
					itemNames.Add(item.Name, item);
				else {
					AssignUniqueName(item, itemNames);
					itemNames.Add(item.Name, item);
					needNamesUpdating = true;
				}
			}
			if (needNamesUpdating) {
				UpdateNames();
				needNamesUpdatingOnce = false;
			}
		}
		public virtual void CheckControlNames() {
			Hashtable controlNames = new Hashtable();
			ArrayList list = new ArrayList(Items);
			foreach(BaseLayoutItem item in list) {
				LayoutControlItem citem = item as LayoutControlItem;
				if(citem != null && citem.Control != null) {
					if(citem is IFixedLayoutControlItem) {
						citem.ControlName = null;
					}
					else {
						citem.ControlName = GetControlName(citem.Control);
						if(!controlNames.Contains(citem.ControlName)) 
							controlNames.Add(citem.ControlName, citem);
						else 
							throw new LayoutControlInternalException("Controls in LayoutControl should have unique names. Duplicate name : " + (citem.ControlName.Length == 0 ? "unnamed" : citem.ControlName));
					}
				}
			}
		}
		bool isUpdatingCustomizationFormVisibilityInDesignMode = false;
		public virtual void UpdateCustomizationFormVisibilityInDesignMode() {
			if (DesignMode && CustomizationForm != null && CustomizationForm.Visible && !isUpdatingCustomizationFormVisibilityInDesignMode) {
				isUpdatingCustomizationFormVisibilityInDesignMode = true;
				CustomizationForm.Hide();
				CustomizationForm.Show();
				isUpdatingCustomizationFormVisibilityInDesignMode = false;
			}
		}
		public virtual void CheckControlsWithoutItems() {
			List<Control> controlsInItems = new List<Control>();
			List<LayoutControlItem> itemsToRemove = new List<LayoutControlItem>();
			BeginUpdate();
			foreach (BaseLayoutItem item in Items) {
				LayoutControlItem citem = item as LayoutControlItem;
				if (citem != null && (citem.ControlName != null && citem.ControlName.Length != 0) && citem.Control == null) {
					itemsToRemove.Add(citem);
				}
				if (citem != null && citem.Control != null) {
					controlsInItems.Add(citem.Control);
				}
				if (OptionsSerialization.DiscardOldItems) {
					if (citem != null && citem.Control == null && citem.ControlName != String.Empty && citem.IsHidden && HiddenItems.Contains(citem)) {
						HiddenItems.Remove(citem);
					}
				}
			}
			Items = null;
#if DEBUG
			if (!isDiagnosticsDeserializing)
#endif
				foreach (LayoutControlItem citem in itemsToRemove) {
					if (citem.Parent != null)
						citem.Parent.Remove(citem);
				}
			foreach (Control control in owner.Controls) {
				if (!controlsInItems.Contains(control)) {
					if (!layoutDesignerMethodProvider.IsInternalControl(control) && !(control is PopupContainerControl)) {
						CreateLayoutItemInHiddenItems(control);
					}
				}
			}
			EndUpdate();
		}
		public virtual void CreateLayoutItemInHiddenItems(Control control) {
			bool fExisting = false;
			LayoutControlItem item;
			if(ControlsAndItems.ContainsKey(control) && ControlsAndItems[control].Control == null && !HiddenItems.Contains(ControlsAndItems[control])) {
				item = (LayoutControlItem)ControlsAndItems[control];
				fExisting = true;
			} else {
				item = (LayoutControlItem)CreateLayoutItem(null);
				GetUniqueName(item);
			}
			control.Visible = false;
			item.Control = control;
			item.Owner = as_I;
			BaseLayoutItem hiddenItem = item;
			if(fExisting) {
				BaseItemCollection collection = new BaseItemCollection();
				while(!string.IsNullOrEmpty(hiddenItem.ParentName) && !hiddenItem.HasCustomizationParentName && ItemsAndNames.ContainsKey(hiddenItem.ParentName)) {
					LayoutGroup group = ItemsAndNames[hiddenItem.ParentName] as LayoutGroup;
					if(group == null || Items.Contains(group)) break;
					group.Add(hiddenItem);
					hiddenItem = group;
				}
				if(hiddenItem != item) {
					if(HiddenItems.Contains(hiddenItem)) return;
					HiddenItems.Add(hiddenItem);
					hiddenItem.UpdateChildren(false);
					((LayoutGroup)hiddenItem).SetTabbedGroupParent(null);
					hiddenItem.Parent = null;
					hiddenItem.Owner = as_I;
					return;
				}
			}
			HiddenItems.Add(hiddenItem);
			if(itemsInHiddenItems == null) {
				itemsInHiddenItems = new List<BaseLayoutItem>();
			}
			itemsInHiddenItems.Add(hiddenItem);
		}
		List<BaseLayoutItem> itemsInHiddenItems;
		Dictionary<Type, Type> customWrappersCore = null;
		protected internal Dictionary<Type, Type> CustomWrappers {
			get {
				if (customWrappersCore == null) customWrappersCore = new Dictionary<Type, Type>();
				return customWrappersCore;
			}
		}
		public void RegisterCustomPropertyGridWrapper(Type itemType, Type customWrapper) {
			if (customWrapper == null || itemType == null) return;
			if (!customWrapper.IsSubclassOf(typeof(BasePropertyGridObjectWrapper))) return;
			if (!CustomWrappers.ContainsKey(itemType)) CustomWrappers.Add(itemType, customWrapper);
		}
		public void UnRegisterCustomPropertyGridWrapper(Type itemType) {
			if (CustomWrappers.ContainsKey(itemType)) CustomWrappers.Remove(itemType);
		}
		public bool IsCustomPropertyGridWrapperExist(BaseLayoutItem item) {
			return (item != null) ? CustomWrappers.ContainsKey(item.GetType()) : false;
		}
		protected ILayoutControl as_I { get { return castedOwner; } }
		public event EventHandler ItemSelectionChanged {
			add { owner.ItemSelectionChanged += value; }
			remove { owner.ItemSelectionChanged -= value; }
		}
		public event EventHandler Changed {
			add { owner.Changed += value; }
			remove { owner.Changed -= value; }
		}
		public event CancelEventHandler Changing {
			add { owner.Changing += value; }
			remove { owner.Changing -= value; }
		}
		public virtual ISite Site { get { return owner.Site; } set { owner.Site = value; } }
		public Size Size { get { return owner.Size; } set { owner.Size = value; } }
		public int Width { get { return owner.Width; } set { owner.Width = value; } }
		public int Height { get { return owner.Height; } set { owner.Height = value; } }
		public int ClientWidth { get { return owner.ClientWidth; } }
		public int ClientHeight { get { return owner.ClientHeight; } }
		public Rectangle Bounds { get { return owner.Bounds; } }
		public Control Parent { get { return owner.Parent; } set { owner.Parent = value; } }
		public virtual bool IsDeserializing { get { return isDeserializingCore || (!IsInitialized && !as_I.DesignMode); } set { isDeserializingCore = value; } }
		public virtual bool IsSerializing { get { return isSerializingCore; } }
		public virtual bool IsUpdateLocked { get { return as_I.RootGroup == null ? UpdatedCount != 0 : as_I.RootGroup.IsUpdateLocked || as_I.RootGroup.IsOwnerInvalidating; } }
		public virtual bool AllowManageDesignSurfaceComponents {
			get { return fAllowManageDesignSurfaceComponents; }
			set { fAllowManageDesignSurfaceComponents = value; }
		}
		public virtual bool AllowCustomizationMenu {
			get { return fAllowCustomizationMenu; }
			set { fAllowCustomizationMenu = value; }
		}
		public virtual CustomizationModes CustomizationMode {
			get { return fCustomizationMode; }
			set {
				fCustomizationMode = value;
				LongPressControl.Enabled = (CustomizationModes.Quick == value && AllowCustomizationMenu && !DesignTimeTools.IsDesignMode);
			}
		}
		public virtual bool DesignMode {
			get { return !isDesignTesting ? owner.IsDesignMode : true; }
		}
		public virtual bool SelectedChangedFlag {
			get { return selectedChangedFlagCore; }
			set { selectedChangedFlagCore = value; }
		}
		public virtual bool CloneInProgress {
			get { return iCloneCounter > 0; }
		}
		bool allowSetIsModifiedCore = false;
		public bool AllowSetIsModified {
			get { return allowSetIsModifiedCore; }
			set { allowSetIsModifiedCore = value; }
		}
		public virtual bool EnableCustomizationMode {
			get { return isCustomizationMode; }
			set {
				if(!owner.IsDesignMode) { as_I.UndoManager.Enabled = false; }
				if(isCustomizationMode == value) return;
				try {
					AllowSetIsModified = false;
					FireChanging(RootGroup);
					isCustomizationMode = value;
					if(CustomizationForm != null) {
						CustomizationForm.Visible = isCustomizationMode;
						if(as_I.EnableCustomizationForm && (as_I.CustomizationMode != CustomizationModes.Quick || as_I.DesignMode)) {							
							if(CustomizationForm.Visible) CustomizationForm.ShowCustomization(CalcFormPosition());
							else {
								CustomizationForm.ResetActiveControl();
								CustomizationForm.Visible = false;
							}
						}
						if(CustomizationForm.Visible) {
							DesignTimeActivateCustomizationForm();
							owner.RaiseOwnerEvent_ShowCustomization(owner, EventArgs.Empty);
						} else {
							CustomizationForm.ResetActiveControl();
							owner.RaiseOwnerEvent_HideCustomization(owner, EventArgs.Empty);
						}
					}
					as_I.SelectedChangedCount = 0;
					if(RootGroup != null) {
						if(!DesignMode) RootGroup.ClearSelection();
						RootGroup.ResizeManager.UpdateVisibility();
						ResetEnabledStateController();
						RootGroup.shouldResetBorderInfoCore = true;
						RootGroup.SetShouldUpdateViewInfo();
						as_I.EnabledStateController.SetItemEnabledStateDirty(RootGroup);
						ShouldUpdateLookAndFeel = true;
					}
					as_I.Invalidate();
					as_I.ActiveHandler.Reset();
				}
				finally {
					FireChanged(RootGroup);
					AllowSetIsModified = isCustomizationMode;
					CheckNames();
					if(!owner.IsDesignMode) as_I.UndoManager.Enabled = value;
				}
			}
		}
		protected virtual void DesignTimeActivateCustomizationForm() {
			if(!DesignMode) return;
			if(CustomizationForm == null) return;
			CustomizationForm.Activate();
		}
		public virtual bool EnableCustomizationForm {
			get { return fEnableCustomizationFormCore; }
			set {
				fEnableCustomizationFormCore = value;
				if (value == false && CustomizationForm != null) {
					CustomizationForm.Close();
					CustomizationForm = null;
				}
			}
		}
		public virtual bool DisposingFlag { get { return disposingFlagInternalCore; } }
		public virtual bool InitializationFinished {
			get { return initializationFinishedCore; }
			set { initializationFinishedCore = value; }
		}
		public virtual bool ExceptionsThrown {
			get { return exceptionsThrownCore; }
			set { exceptionsThrownCore = value; }
		}
		public virtual bool ResizerBroken {
			get { return resizerBrokenCore; }
			set { resizerBrokenCore = value; }
		}
		public virtual bool CanRestoreDefaultLayout { get { return defaultLayout != null; } }
		public virtual bool ShouldArrangeTextSize {
			get { return shouldArrangeTextSizeCore; }
			set { shouldArrangeTextSizeCore = value; }
		}
		public virtual bool ShouldUpdateConstraints {
			get { return shouldUpdateConstraintsCore; }
			set { shouldUpdateConstraintsCore = value; }
		}
		public virtual bool ShouldResize {
			get { return shouldResizeCore; }
			set { shouldResizeCore = value; }
		}
		public virtual bool ShouldUpdateLookAndFeel {
			get { return shouldUpdateLookAndFeelCore; }
			set { shouldUpdateLookAndFeelCore = value; }
		}
		public virtual bool LockUpdateOnChangeUICuesRequest {
			get { return lockUpdateOnChangeUICuesRequestCore; }
			set { lockUpdateOnChangeUICuesRequestCore = value; }
		}
		public virtual bool ShouldUpdateControlsLookAndFeel {
			get { return shouldUpdateControlsLookAndFeelCore; }
			set { shouldUpdateControlsLookAndFeelCore = value; }
		}
		public virtual bool ShouldUpdateControls {
			get { return shouldUpdateControlsCore; }
			set { shouldUpdateControlsCore = value; }
		}
		public virtual int DelayPainting {
			get { return delayPaintingCore; }
			set { delayPaintingCore = value; }
		}
		public virtual int SelectedChangedCount {
			get { return selectedChangedCountCore; }
			set {
				selectedChangedCountCore = value;
				CustomizationForm customization =  customizationFormCore as CustomizationForm;
				IComponent sender = customization != null && customization.layoutTreeView1 != null ? customization.layoutTreeView1.lastSelectedItem : null;
				sender = sender != null ? sender : owner;
				if (selectedChangedCountCore == 0 && selectedChangedFlagCore) as_I.SelectionChanged(sender);
			}
		}
		public virtual int UpdatedCount {
			get { return initializedCount; }
			set {
				initializedCount = value;
				if (initializedCount == 0) EndInitCore();
			}
		}
		public LayoutControlGroup RootGroup {
			get { return owner.Root; }
			set {
				if (owner.Root != value) {
					owner.Root = value;
					if (owner.Root != null) owner.Root.Owner = null;
				}
				if (owner.Root != null) {
					owner.Root.Owner = as_I;
					as_I.AddComponent(owner.Root);
					ShouldUpdateLookAndFeel = true;
					shouldResizeCore = true;
					as_I.Invalidate();
				}
			}
		}
		public ReadOnlyItemCollection Items {
			get {
				if (itemsCore == null) {
					FillItemsInternal();
					CheckForSameNames();
				}
				return itemsCore;
			}
			set {
				if (isDeserializingCore) return;
				itemsCore = value;
			}
		}
		public Dictionary<string, BaseLayoutItem> ItemsAndNames {
			get { return itemsAndNamesCore; }
			set { itemsAndNamesCore = value; }
		}
		public Dictionary<Control, LayoutControlItem> ControlsAndItems {
			get { return controlsAndItemsCore; }
			set { controlsAndItemsCore = value; }
		}
		public UserCustomizationForm CustomizationForm {
			get { return customizationFormCore; }
			set { customizationFormCore = value; }
		}
		public virtual List<IComponent> Components {
			get {
				if (componentsCore == null) componentsCore = new List<IComponent>();
				return componentsCore;
			}
		}
		public virtual LayoutPaintStyle PaintStyle { get { return ActiveStyle; } }
		public virtual HiddenItemsCollection HiddenItems { get { return hiddenItemsCore; } }
		public virtual RightButtonMenuManager CustomizationMenuManager {
			get {
				if (customizationMenuManagerCore == null) customizationMenuManagerCore = owner.CreateRightButtonMenuManager();
				return customizationMenuManagerCore;
			}
		}
		public virtual Control Control { get { return owner as Control; } }
		public virtual Scrolling.ScrollInfo Scroller { get { return scrollerCore; } }
		public virtual LayoutControlRoles ControlRole {
			get { return controlsRole; }
			set {
				controlsRole = value;
				if (controlsRole == LayoutControlRoles.CustomizationFormControl)
					as_I.EnableCustomizationForm = false;
				else
					as_I.EnableCustomizationForm = true;
			}
		}
		protected SerializeableUserLookAndFeel styleControllerLookAndFeelCore = null;
		protected bool AreEqual(SerializeableUserLookAndFeel lf1, UserLookAndFeel lf2) {
			return
				lf1.ActiveStyle == lf2.ActiveStyle &&
				lf1.SkinName == lf2.SkinName &&
				lf1.Style == lf2.Style &&
				lf1.UseDefaultLookAndFeel == lf2.UseDefaultLookAndFeel &&
				lf1.UseWindowsXPTheme == lf2.UseWindowsXPTheme;
		}
		public virtual SerializeableUserLookAndFeel LookAndFeel {
			get {
				if (StyleController != null && StyleController.LookAndFeel != null) {
					isSizeChanging = true;
					if (styleControllerLookAndFeelCore == null) {
						styleControllerLookAndFeelCore = new SerializeableUserLookAndFeel(as_I.Control);
						styleControllerLookAndFeelCore.Assign(StyleController.LookAndFeel);
					}
					if (AreEqual(styleControllerLookAndFeelCore, StyleController.LookAndFeel))
						styleControllerLookAndFeelCore.Assign(StyleController.LookAndFeel);
					isSizeChanging = false;
					return styleControllerLookAndFeelCore;
				}
				return lookAndFeelCore;
			}
		}
		public virtual LayoutSerializationOptions OptionsSerialization {
			get {
				if (optionsSerializationCore == null) optionsSerializationCore = new LayoutSerializationOptions();
				return optionsSerializationCore;
			}
		}
		public virtual LayoutAppearanceCollection Appearance { get { return appearanceCore; } }
		public virtual AppearanceController AppearanceController {
			get {
				if (appearanceControllerCore == null) appearanceControllerCore = new AppearanceController(as_I);
				return appearanceControllerCore;
			}
			set { appearanceControllerCore = value; }
		}
		protected virtual EnabledStateController CreateEnabledStateController(){
			return new EnabledStateController(as_I);
		}
		public virtual EnabledStateController EnabledStateController {
			get {
				if (enabledStateControllerCore == null) enabledStateControllerCore = CreateEnabledStateController();
				return enabledStateControllerCore;
			}
			set { enabledStateControllerCore = value; }
		}
		public SizeF AutoScaleFactor { get { return castedOwner.AutoScaleFactor; } }
		protected UserInteractionHelper userInteractionHelperCore;
		public UserInteractionHelper UserInteractionHelper {
			get {
				return userInteractionHelperCore;					
			}
		}
		public HitInfo.BaseLayoutItemHitInfo CalcHitInfo(Point hitPoint) {
			return RootGroup != null ? RootGroup.CalcHitInfo(hitPoint, false) : null;
		}
		protected virtual void LongPressControl_LongPress(object sender, MouseEventArgs e) {		  
			BeginInit();
			ShowCustomizationForm();
			Point point = Control.PointToClient(e.Location);
			BaseLayoutItemHitInfo hitInfo = CalcHitInfo(point);
			if(hitInfo != null && hitInfo.Item != null) hitInfo.Item.Selected = true;
			EndInit();
		}
		internal LongPressControl longPressControlCore = null;
		public LongPressControl LongPressControl {
			get {
				if(longPressControlCore == null) longPressControlCore = CreateLongPressControl();
				return longPressControlCore;
			}
		}
		protected virtual LongPressControl CreateLongPressControl() {
			var result =  new LongPressControl(Control,OptionsCustomizationForm);
			result.LongPress += LongPressControl_LongPress;
			return result;
		}
		public void DisposeLongPressControl() {
			if(longPressControlCore != null) {
				longPressControlCore.LongPress -= LongPressControl_LongPress;
				longPressControlCore.Dispose();
				longPressControlCore = null;
			}
		}
		public void BestFit() {
			RootGroup.Resizer.BestFit();
			ShouldResize = true;
			Invalidate();
		}
	   public virtual LayoutControlHandler ActiveHandler {
			get {
				if (as_I.EnableCustomizationForm && as_I.EnableCustomizationMode)
					return CustomizeHandler;
				else return layoutDesignerMethodProvider.RuntimeHandler;
			}
		}
		public virtual EditorContainer GetEditorContainer() {
			if (layoutEditorContainer == null) layoutEditorContainer = new LayoutEditorContainer();
			return layoutEditorContainer;
		}
		public virtual FocusHelperBase FocusHelper {
			get {
				if (focusHelperInternal == null) focusHelperInternal = CreateFocusHelper();
				return focusHelperInternal;
			}
			set { focusHelperInternal = value; }
		}
		public virtual ConstraintsManager ConstraintsManager {
			get {
				if (constraintsSetter == null) constraintsSetter = new ConstraintsManager();
				return constraintsSetter;
			}
		}
		public virtual FakeFocusContainer FakeFocusContainer { get { return fakeFocusContainerCore; } }
		public virtual TextAlignManager TextAlignManager {
			get {
				if(textAlignManagerCore == null && !DisposingFlagInternal) textAlignManagerCore = new TextAlignManager(as_I);
				return textAlignManagerCore;
			}
		}
		public virtual LayoutGroupHandlerWithTabHelper CreateRootHandler(LayoutGroup group) {
			if (layoutAdapter != null) return layoutAdapter.CreateRootHandler(group);
			else return new LayoutGroupHandlerWithTabHelper(group);
		}
		public virtual OptionsView OptionsView {
			get {
				if (viewOptions == null) viewOptions = new OptionsView(as_I);
				return viewOptions;
			}
		}
		public virtual OptionsItemText OptionsItemText {
			get {
				if (itemTextOptions == null) itemTextOptions = new OptionsItemText(as_I);
				return itemTextOptions;
			}
		}
		public virtual OptionsFocus OptionsFocus {
			get {
				if (focusOptions == null) focusOptions = new OptionsFocus(as_I);
				return focusOptions;
			}
		}
		public virtual OptionsCustomizationForm OptionsCustomizationForm {
			get {
				if (customizationFormOptions == null) customizationFormOptions = new OptionsCustomizationForm(as_I);
				return customizationFormOptions;
			}
		}
		public virtual PaintingType PaintingType {
			get {
				SerializeableUserLookAndFeel lookAndFeel = as_I.LookAndFeel;
				if(lookAndFeel == null) return PaintingType.Normal;
				if(lookAndFeel.ActiveStyle == DevExpress.LookAndFeel.ActiveLookAndFeelStyle.WindowsXP) return PaintingType.XP;
				if(lookAndFeel.ActiveStyle == DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Skin) return PaintingType.Skinned;
				return PaintingType.Normal;
			}
		}
		public virtual IDXMenuManager MenuManager { get { return menuManagerCore; } set { menuManagerCore = value; } }
		public bool HasUndoManager { 
			get { return undoManagerCore != null; } 
		}
		public virtual UndoManager UndoManager {
			get {
				if (!owner.IsDesignMode && undoManagerCore == null) undoManagerCore = new UndoManager(as_I);
				return undoManagerCore;
			}
		}
		protected void LockUndoManager() {
			if (undoManagerCore == null) return;
			UndoManager.LockUndo();
		}
		protected void UnLockUndoManager() {
			if (undoManagerCore == null) return;
			UndoManager.UnlockUndo();
		}
		public virtual IComparer HiddenItemsSortComparer {
			get {
				if (hiddenItemsComparer == null) hiddenItemsComparer = new LayoutControlCaseInsensitiveComparer();
				return hiddenItemsComparer;
			}
			set { hiddenItemsComparer = value; }
		}
		public virtual void RestoreLayoutFromStream(Stream stream) { RestoreLayoutCore(new XmlXtraSerializer(), stream); }
		public virtual void SaveLayoutToStream(Stream stream) { SaveLayoutCore(new XmlXtraSerializer(), stream); }
		public virtual void RegisterLayoutAdapter(ILayoutAdapter adapter) { layoutAdapter = adapter; }
		public virtual void RaiseGroupExpandChanging(LayoutGroupCancelEventArgs e) {
			owner.RaiseOwnerEvent_GroupExpandChanging(e);
		}
		public virtual void RaiseGroupExpandChanged(LayoutGroupEventArgs e) {
			owner.RaiseOwnerEvent_GroupExpandChanged(e);
		}
		public virtual void RaiseShowCustomizationMenu(PopupMenuShowingEventArgs ma) {
			owner.RaiseOwnerEvent_ShowCustomizationMenu(ma);
		}
		public virtual void RaiseShowLayoutTreeViewContextMenu(PopupMenuShowingEventArgs ma) {
			owner.RaiseOwnerEvent_ShowLayoutTreeViewContextMenu(ma);
		}
		public virtual bool FireChanging(IComponent component) {
			bool result = true;
			try {
				if (changedCounter == 0 && IsInitialized && !isSizeChanging) {
					CancelEventArgs e = new CancelEventArgs();
					e.Cancel = false;
					owner.RaiseOwnerEvent_Changing(component, e);
					result = !e.Cancel;
				}
			} finally {
				if (result) changedCounter++;
			}
			return result;
		}
		public virtual void FireCloseButtonClick(LayoutGroup component) {
			owner.RaiseOwnerEvent_CloseButtonClick(this, new LayoutGroupEventArgs(component));
		}
		public virtual void FireChanged(IComponent component) {
			try {
				changedCounter--;
#if DEBUGTEST
				if(changedCounter < 0) throw new Exception("error occurs");
#endif
				if(changedCounter == 0 && IsInitialized && !(DesignMode & isSizeChanging)) {
					if(fNeedRaiseLayoutUpdate) {
						owner.RaiseOwnerEvent_LayoutUpdate(EventArgs.Empty);
						fNeedRaiseLayoutUpdate = false;
					}
					owner.RaiseOwnerEvent_Changed(component, EventArgs.Empty);
				}
			} catch(Exception e) {
				if(!DesignMode) throw e;
			}
		}
		public virtual void FireItemAdded(BaseLayoutItem component, EventArgs ea) {
			owner.RaiseOwnerEvent_ItemAdded(component, EventArgs.Empty);
		}
		public virtual void FireItemRemoved(BaseLayoutItem component, EventArgs ea) {
			owner.RaiseOwnerEvent_ItemRemoved(component, EventArgs.Empty);
		}
		protected bool AllowSetIsModifiedInternal {
			get {
				return as_I.AllowSetIsModified;
			}
		}
		public virtual void SetIsModified(bool newVal) {
			isLayoutModifiedCore = newVal;
			if (!AllowSetIsModifiedInternal && newVal == true) return;
			isModifiedCore = newVal;
		}
		public virtual void SelectionChanged(IComponent component) {
			selectedChangedCountCore = 0;
			selectedChangedFlagCore = false;
			if (!(as_I.IsUpdateLocked && shouldResizeCore)) OnSelectionChanged(component, EventArgs.Empty);
			owner.RaiseOwnerEvent_ItemSelectionChanged(component, EventArgs.Empty);
		}
		public virtual void AddComponent(BaseLayoutItem component) {
			component.Accept(AddComponentHelper);
			Items = null;
			as_I.FireItemAdded(component, EventArgs.Empty);
		}
		public virtual void RemoveComponent(BaseLayoutItem component) {
			component.Accept(RemoveComponentHelper);
			Items = null;
			as_I.FireItemRemoved(component, EventArgs.Empty);
		}
		public virtual void BeginInit() {
			InitializationFinished = false;
			UpdatedCount++;
			if (RootGroup != null) RootGroup.ResetResizer();
		}
		public virtual void EndInit() { UpdatedCount--; }
		public virtual void BeginUpdate() { UpdatedCount++; }
		public virtual void EndUpdate() {
			UpdatedCount--;
			if (UpdatedCount == 0) Invalidate();
		}
		public virtual bool CanInvalidate() {
			return !(as_I.Control.IsDisposed || !IsInitialized || RootGroup == null || RootGroup.IsUpdateLocked || ResizerBroken);
		}
		public virtual void Invalidate() {
			if (!CanInvalidate()) return;
			DoOwnerInvalidate();
			DoResize();
			RaiseSizeableChanged();
			InvalidateRootOffsetControlsAndScrolls();
			updatedCountInternal++;
		}
		protected virtual void InvalidateRootOffsetControlsAndScrolls() {
			UpdateRoot();
			if (delayPaintingCore == 0) {
				if (!DoGroupChanged()) {
					UpdateRoot();
					owner.RaiseOwnerEvent_Updated(null, EventArgs.Empty);
					if (shouldUpdateControlsCore) {
						UpdateControls();
						shouldUpdateControlsCore = false;
					}
					layoutControlAccessible = null;
					as_I.Control.Invalidate();
				}
			}
		}
		private void DoResize() {
			if (shouldResizeCore) {
				RootGroup.UpdateLayout();
				fNeedRaiseLayoutUpdate = true;
				shouldResizeCore = false;
				resizesCount++;
				as_I.ShouldUpdateControls = true;
			}
			if (RootGroup != null && RootGroup.ResizeManager.resizer == null) {
				fNeedRaiseLayoutUpdate = true;
			}
		}
		void DoOwnerInvalidate() {
			RootGroup.IsOwnerInvalidating = true;
			InvalidateLookAndFeel();
			ArrangeTextSize();
			ResetViewInfo();
			InvalidateConstraints();
			SetRTLIfNeeded();
			RootGroup.IsOwnerInvalidating = false;
		}
		void SetRTLIfNeeded() {
			if(WindowsFormsSettings.GetIsRightToLeft(Control) != RootGroup.IsRTL) {
				RootGroup.ResetResizer();
				RootGroup.BeginUpdate();
				RootGroup.SetRTL(WindowsFormsSettings.GetIsRightToLeft(Control), !OptionsView.RightToLeftMirroringApplied);
				OptionsView.RightToLeftMirroringApplied = true;
				RootGroup.EndUpdate();
				Control.Refresh();
			}
		}
		private void InvalidateConstraints() {
			if(ShouldUpdateConstraints) {
				RootGroup.Resizer.UpdateConstraints();
				ShouldUpdateConstraints = false;
				shouldResizeCore = true;
			}
		}
		private void ResetViewInfo() {
			if(ShouldUpdateLookAndFeel) {
				ResetAllViewInfo();
				shouldResizeCore = true;
				ShouldUpdateLookAndFeel = false;
				ShouldUpdateConstraints = true;
				viewInfoResetedCount++;
			}
		}
		private void ArrangeTextSize() {
			if (shouldArrangeTextSizeCore) {
				AlignTextWidth();
				shouldArrangeTextSizeCore = false;
				ShouldUpdateLookAndFeel = true;
			}
		}
		private void InvalidateLookAndFeel() {
			if (shouldUpdateControlsLookAndFeelCore) {
				IStyleController controller = owner.GetIStyleController();
				foreach(Control control in as_I.Control.Controls)
					SetControlsLookAndFeel(control, controller);
				shouldUpdateControlsLookAndFeelCore = false;
			}
		}
		public virtual void SetControlDefaults() {
			RootGroup.BeginChangeUpdate();
			RootGroup.CreateLayout(2, 2);
			RootGroup.Text = "Root 0";
			RootGroup.PreferredSize = new Size(RootGroup.Items[0].MinSize.Width * 2, RootGroup.Items[0].MinSize.Height * 3);
			Size = new Size(RootGroup.PreferredSize.Width + 50, RootGroup.PreferredSize.Height + 50);
			RootGroup.EndChangeUpdate();
		}
		public virtual void SetControlDefaultsLast() {
			as_I.BeginInit();
			InitializeGroup();
			as_I.EndInit();
		}
		public virtual void ShowCustomizationForm() {		   
			if(CustomizationForm != null && as_I.CustomizationMode != CustomizationForm.CustomizationMode ) {
				CustomizationForm.Dispose();
				DisposeUserInteractionHelper();
				CustomizationForm = null;
			}
			if(CustomizationForm == null || !CustomizationForm.IsHandleCreated) {
				customizationFormCore = null;
				CreateCustomizationFormCore();
			}		   
			as_I.EnableCustomizationMode = true;
			if(!DesignMode) {
				as_I.Control.Focus();
				LongPressControl.Enabled = false;
				if(as_I.CustomizationMode == CustomizationModes.Quick) {
					CreateUserInteractionHelper();
					CustomizationForm.CustomizationMode = CustomizationModes.Quick;
				}
			}
		}
		RenameItemManager renameManagerCore = null;
		public virtual RenameItemManager RenameItemManager {
			get {
				if (renameManagerCore == null) renameManagerCore = CreateRenameItemManager();
				return renameManagerCore;
			}
			set { renameManagerCore = value; }
		}
		protected virtual RenameItemManager CreateRenameItemManager() {
			return new RenameItemManager();
		}
		public virtual void HideSelectedItems() {
			SelectionHelper sh = new SelectionHelper();
			List<BaseLayoutItem> list = sh.GetItemsList(RootGroup);
			if (list.Count > 0) {
				foreach (BaseLayoutItem item in list)
					item.HideToCustomization();
			}
		}
		public virtual void SelectParentGroup() {
			SelectionHelper sh = new SelectionHelper();
			List<BaseLayoutItem> list = sh.GetItemsList(RootGroup);
			if (list.Count > 0) {
				BaseLayoutItem selectedItem = list[0] as BaseLayoutItem;
				RootGroup.ClearSelection();
				if (selectedItem.Parent != null) {
					if (selectedItem.IsGroup) {
						LayoutGroup selectedGroup = selectedItem as LayoutGroup;
						if (selectedGroup.ParentTabbedGroup != null) {
							selectedGroup.ParentTabbedGroup.Selected = true;
							return;
						}
					}
					selectedItem.Parent.Selected = true;
				}
			}
		}
		public virtual void HideCustomizationForm() {
			as_I.EnableCustomizationMode = false;
		}
		public virtual void RestoreDefaultLayout() {
			if(CanRestoreDefaultLayout) {
				owner.RaiseOwnerEvent_DefaultLayoutLoading(owner, EventArgs.Empty);
				defaultLayout.Seek(0, SeekOrigin.Begin);
				RestoreLayoutFromStream(defaultLayout);
				owner.RaiseOwnerEvent_DefaultLayoutLoaded(owner, EventArgs.Empty);
			}
		}
		public virtual void UpdateHiddenItems() {
			if (isDeserializingCore) return;
			HiddenItemsVisibilityHelper helper = new HiddenItemsVisibilityHelper();
			foreach (BaseLayoutItem item in as_I.HiddenItems) {
				item.Accept(helper);
			}
		}
		public virtual void OnItemAppearanceChanged(BaseLayoutItem item) { }
		public virtual void SaveLayoutToXml(string xmlFile) {
			SaveLayoutCore(new XmlXtraSerializer(), xmlFile);
		}
		public virtual void RestoreLayoutFromXml(string xmlFile) {
			RestoreLayoutCore(new XmlXtraSerializer(), xmlFile);
		}
		public virtual void SaveLayoutToRegistry(string path) {
			SaveLayoutCore(new RegistryXtraSerializer(), path);
		}
		public virtual void RestoreLayoutFromRegistry(string path) {
			RestoreLayoutCore(new RegistryXtraSerializer(), path);
		}
		public virtual void RebuildCustomization() {
			if (customizationFormCore == null) return;
			bool shouldShowCustomization = customizationFormCore.IsHandleCreated && customizationFormCore.Visible;
			DisposeCustomizationFormCore();
			if (shouldShowCustomization) CreateCustomizationFormCore();
		}
		public virtual void UpdateChildControlsLookAndFeel() {
			OnLookAndFeelStyleChanged(null, EventArgs.Empty);
		}
		public virtual void UpdateFocusedElement(BaseLayoutItem item) {
			if (as_I.OptionsFocus.EnableAutoTabOrder) as_I.FocusHelper.SelectedComponent = item;
		}
		[SecuritySafeCritical]
		public virtual void SetCursor(Cursor cursor) {
#if DXWhidbey
			if (as_I.Control.UseWaitCursor) return;
#else
			if(((ILayoutControl)owner).DesignMode) Cursor.Current = cursor;
#endif
			bool shoudUpdateCursor = cursorCore != cursor;
			cursorCore = cursor;
			if(shoudUpdateCursor) DevExpress.Utils.Drawing.Helpers.NativeMethods.SendMessage(Control.Handle, 0x20, Control.Handle, (IntPtr)0x1);
		}
		public virtual LayoutGroup CreateLayoutGroup(LayoutGroup parent) {
			return new LayoutControlGroup(parent);
		}
		public virtual BaseLayoutItem CreateLayoutItem(LayoutGroup parent) {
			return new LayoutControlItem((LayoutControlGroup)parent);
		}
		public virtual TabbedGroup CreateTabbedGroup(LayoutGroup parent) {
			return new TabbedControlGroup(parent);
		}
		public virtual EmptySpaceItem CreateEmptySpaceItem(LayoutGroup parent) {
			return new EmptySpaceItem((LayoutControlGroup)parent);
		}
		public virtual SplitterItem CreateSplitterItem(LayoutGroup parent) {
			return new SplitterItem((LayoutControlGroup)parent);
		}
		protected virtual IFixedLayoutControlItem CreateFixedLayoutItem(string typeName) {
			IFixedLayoutControlItem instance = null;
			Type fixedItemType = null;
			if (HiddenItems.FixedItemsCount == 0) InitializeFixedHiddenItems();
			foreach (Type t in HiddenItems.FixedTypes) {
				if (t.Name == typeName) {
					fixedItemType = t;
					break;
				}
			}
			if (fixedItemType != null) {
				instance = Activator.CreateInstance(fixedItemType, false) as IFixedLayoutControlItem;
				if (instance != null) {
					instance.Owner = this;
				}
			}
			return instance;
		}
		public virtual object Images {
			get { return imagesCore; }
			set {
				imagesCore = value;
				ShouldUpdateLookAndFeel = true;
				ShouldUpdateConstraints = true;
				as_I.Invalidate();
			}
		}
		public virtual bool CheckIfControlIsInLayout(Control control) {
			foreach (BaseLayoutItem item in Items) {
				LayoutControlItem citem = item as LayoutControlItem;
				if (citem != null && citem.Control == control) return true;
			}
			return false;
		}
		public virtual LayoutGroup GetGroupAtPoint(Point p) {
			BaseLayoutItemHitInfo lht = RootGroup.CalcHitInfo(p, false);
			if (lht.Item != null) {
				if (lht.Item.IsGroup) return (LayoutGroup)lht.Item;
				else return lht.Item.Parent;
			}
			return null;
		}
		public virtual LayoutControlItem GetItemByControl(Control control) {
			LayoutControlItem result = GetItemByControl(control, RootGroup);
			if (result == null) {
				if (as_I.HiddenItems == null) return null;
				foreach (BaseLayoutItem item in as_I.HiddenItems) {
					LayoutClassificationArgs arg = LayoutClassifier.Default.Classify(item);
					if (arg.IsGroup) {
						LayoutControlItem citem = GetItemByControl(control, arg.Group);
						if (citem != null) return citem;
					}
					if (arg.IsTabbedGroup) {
						foreach (LayoutControlGroup tempGroup in arg.TabbedGroup.TabPages) {
							LayoutControlItem citem = GetItemByControl(control, tempGroup);
							if (citem != null) return citem;
						}
					}
					if (arg.IsLayoutControlItem) {
						if (arg.LayoutControlItem.Control == control) return arg.LayoutControlItem;
					}
				}
			}
			return result;
		}
		public virtual bool IsHidden(BaseLayoutItem item) {
			if (as_I.HiddenItems == null) return false;
			ContainsItemVisitor helper = new ContainsItemVisitor(item);
			int count = as_I.HiddenItems.Count;
			for(int i = 0; i < count; i++)
				as_I.HiddenItems[i].Accept(helper);
			count = as_I.HiddenItems.FixedItems.Count;
			for(int i = 0; i < count; i++)
				((BaseLayoutItem)as_I.HiddenItems.FixedItems[i]).Accept(helper);
			return helper.Contains;
		}
		public virtual string GetUniqueName(BaseLayoutItem item) {
			if (Items.Contains(item)) {
				CheckNames();
				return item.Name;
			} else {
				int index = Items.IndexOf(item);
				return "LayoutItem" + (index > 0 ? index.ToString() : Guid.NewGuid().ToString());
			}
		}
		LayoutStyleManager psHelperCore = null;
		public virtual LayoutStyleManager LayoutStyleManager {
			get {
				if(psHelperCore == null) psHelperCore = CreateLayoutStyleManager();
				return psHelperCore;
			}
		}
		protected virtual LayoutStyleManager CreateLayoutStyleManager() {
			return new LayoutStyleManager(castedOwner);
		}
		public virtual void OnStartSerializing() {
			try {
				LockUndoManager();
				delayPaintingCore++;
				isSerializingCore = true;
				this.TextAlignManager.Reset();
				RootGroup.ResizeManager.VisibilityState = true;
				if (!resizerBrokenCore) {
					RootGroup.Resizer.RestoreSize();
				}
				Items = null;
				CheckNames();
				CheckControlNames();
			} finally {
				UnLockUndoManager();
			}
		}
		public virtual void OnChangeUICues() {
			if(LockUpdateOnChangeUICuesRequest) return;
			if(DisposingFlag || RootGroup == null || RootGroup.IsDisposing) return;
			ShouldUpdateLookAndFeel = true;
			if(DelayPainting == 0 && !isSizeChanging) Invalidate();
		}
		public virtual void OnEndSerializing() {
			try {
				LockUndoManager();
				isSerializingCore = false;
				if (!resizerBrokenCore) {
					RootGroup.ResizeManager.VisibilityState = false;
					RootGroup.Size = RootGroup.PreferredSize;
					UpdateCustomizationFormVisibilityInDesignMode();
				}
				delayPaintingCore--;
			} finally {
				UnLockUndoManager();
			}
		}
		public virtual string CreateRootName() {
			return "Root";
		}
		public void RaiseSizeableChanged() {
			castedOwner.RaiseSizeableChanged();
		}
		protected internal virtual void RaiseBeforeLoadLayout(LayoutAllowEventArgs e) {
			owner.RaiseOwnerEvent_BeforeLoadLayout(this, e);
		}
		public virtual void OnStartDeserializing(DevExpress.Utils.LayoutAllowEventArgs e) {
			try {
				LockUndoManager();
				RaiseBeforeLoadLayout(e);
				if (!e.Allow) return;
				CheckNames();
				owner.Root.ResizeManager.VisibilityState = true;
				BeginInit();
				isDeserializingCore = true;
				controlsAndItemsCore = new Dictionary<Control, LayoutControlItem>();
				itemsAndNamesCore = new Dictionary<string, BaseLayoutItem>();
				ArrayList localItems = new ArrayList(Items);
				localItems.Reverse();
				owner.Root.Name = CreateRootName();
				for (int i = 0; i < localItems.Count; i++) {
					BaseLayoutItem bitem = localItems[i] as BaseLayoutItem;
					LayoutClassificationArgs arg = LayoutClassifier.Default.Classify(bitem);
					if (bitem.Site != null && bitem != owner.Root) bitem.Name = bitem.Site.Name;
					bitem.Parent = null;
					bitem.Owner = null;
					try {
						ItemsAndNames.Add(bitem.Name, bitem);
					} catch (ArgumentException innerException) {
						throw new LayoutControlInternalException(
							string.Format("The item with the same name (BaseLayoutItem.Name='{0}') already exists within the control (Control.Name='{1}').",
							bitem.Name, (as_I.Control != null) ? as_I.Control.Name : null),
							innerException);
					}
					if (arg.IsLayoutControlItem && arg.LayoutControlItem.Control != null) {
						ControlsAndItems.Add(arg.LayoutControlItem.Control, arg.LayoutControlItem);
						arg.LayoutControlItem.BeginInit();
						Control control = arg.LayoutControlItem.Control;
						if (arg.LayoutControlItem is IFixedLayoutControlItem) {
							arg.LayoutControlItem.Control = null;
							control.Parent = null;
							control.Dispose();
							control = null;
						} else {
							arg.LayoutControlItem.Control = null;
							if (!DesignMode && control != null) control.Visible = false;
						}
						arg.LayoutControlItem.EndInit();
					}
					if (arg.IsGroup) {
						arg.Group.Items.Clear();
					}
					if (arg.IsTabbedGroup) {
						arg.TabbedGroup.TabPages.Clear();
					}
					Items.Remove(bitem);
				}
			} finally {
				UnLockUndoManager();
			}
		}
		public virtual void OnEndDeserializing(string restoredVersion) {
			try {
				needNamesUpdatingOnce = false;
				LockUndoManager();
				as_I.HiddenItems.Clear();
				CheckNotRestoredItems(ItemsAndNames);
				foreach (BaseLayoutItem item in Items) {
					if (item.Name == "") throw new LayoutControlInternalException("Wrong name");
					LayoutItem citem = item as LayoutItem;
					if (citem != null && citem.ParentName == "" && item.Parent != null) throw new LayoutControlInternalException("Wrong parentName name");
					LayoutGroup group = item as LayoutGroup;
					TabbedGroup tgroup = item as TabbedGroup;
					LayoutControlItem controlItem = item as LayoutControlItem;
					SplitterItem splitter = item as SplitterItem;
					if (splitter != null)
						splitter.UpdateLayoutType();
					if (citem != null) {
						if (item is IFixedLayoutControlItem) {
							LayoutControlItem.InitializeItemAsFixed(controlItem, as_I);
						} else {
							controlItem.BeginInit();
							controlItem.Control = GetControlByName(controlItem.ControlName);
							controlItem.EndInit();
						}
					}
					if (group != null) {
						if (group.ParentTabbedGroup != null && group.TabbedGroupParentName == "")
							throw new LayoutControlInternalException("Wrong tabbedGroupParent name");
						if ((group.Parent != null && group.ParentName == "") && group.ParentTabbedGroup == null)
							throw new LayoutControlInternalException("Wrong parent name");
					}
					item.RestoreChildren(Items);
					if (tgroup != null)
						tgroup.UpdateSelectedTabPage();
					if (item.HasCustomizationParentName) {
						HiddenItems.Add(item);
						as_I.AddComponent(item);
					}
					item.UpdateChildren(false);
				}
				RootGroup.ResetResizer();
				RootGroup.Resizer.UpdateConstraints();
				RootGroup.Size = RootGroup.PreferredSize;
				EndInit();
				Invalidate();
				CheckControlsWithoutItems();
				isDeserializingCore = false;
				HiddenItemsChanged(owner, new CollectionChangeEventArgs(CollectionChangeAction.Add, null));
				UpdateCustomizationFormVisibilityInDesignMode();
				RootGroup.ResizeManager.VisibilityState = false;
				if (RootGroup.Site != null) RootGroup.Name = RootGroup.Site.Name;
				if(restoredVersion != layoutVersion) {
					if(itemsInHiddenItems != null && itemsInHiddenItems.Count > 0)owner.RaiseOwnerEvent_LayoutUpgrade(owner, new LayoutUpgradeEventArgs(restoredVersion, itemsInHiddenItems.ToArray()));
					else owner.RaiseOwnerEvent_LayoutUpgrade(owner, new LayoutUpgradeEventArgs(restoredVersion));
					itemsInHiddenItems = null;
				}
			} finally {
				UnLockUndoManager();
			}
		}
		public virtual object XtraFindItemsItem(XtraItemEventArgs e) {
			XtraPropertyInfo info = e.Item.ChildProperties["Name"];
			XtraPropertyInfo infoType = e.Item.ChildProperties["TypeName"];
			if(info == null || infoType == null) return null;
			BaseLayoutItem item = FindItemInCache(ItemsAndNames, (string)info.Value, (string)infoType.Value);
			if(item is IFixedLayoutControlItem && OptionsSerialization.RecreateIFixedItems) item = null;
			if(item != null) {
				item.Owner = as_I;
				item.Parent = null;
				ClearReferences(item);
				LayoutControlGroup group = item as LayoutControlGroup;
				if(group != null) {
					group.SetTabbedGroupParent(null);
					string rootName = CreateRootName();
					if((string)info.Value == rootName) {
						owner.Root = group;
					}
				}
				AddItemInItems(item);
				return item;
			}
			return XtraCreateNewItem(e);
		}
		public virtual BaseLayoutItem XtraCreateNewItem(XtraItemEventArgs e) {
			XtraPropertyInfo info = e.Item.ChildProperties["Name"];
			XtraPropertyInfo infoType = e.Item.ChildProperties["TypeName"];
			if(info == null || infoType == null) return null;
			BaseLayoutItem newItem = CreateItem(info, infoType);
			if(newItem != null) {
				InitNewCreatedItem((string)info.Value, newItem);
				AddItemInItems(newItem);
			}
			return newItem;
		}
		protected void InitNewCreatedItem(string name, BaseLayoutItem newItem) {
			newItem.Owner = as_I;
			newItem.Name = name;
		}
		protected void AddItemInItems(BaseLayoutItem item) {
			if(Items != null) Items.Add(item);
		}
		protected BaseLayoutItem CreateItem(XtraPropertyInfo info, XtraPropertyInfo infoType) {
			string typeStr = (string)infoType.Value;
			string prefix = "DevExpress.XtraLayout.";
			if(typeStr.StartsWith(prefix)) {
				typeStr = typeStr.Remove(0, prefix.Length);
			}
			return CreateItemByType(info, typeStr);
		}
		protected BaseLayoutItem FindItemInCache(Dictionary<string, BaseLayoutItem> cache, string itemName, string itemType) {
			BaseLayoutItem item = null;
			string cacheKey = (string)itemName;
			if(cache.ContainsKey(cacheKey)) {
				item = cache[cacheKey];
				if(item.GetType().ToString() == itemType || item.TypeName == itemType) {
					OnItemFinded(cache, item, cacheKey);
				}
				else item = null;
			}
			return item;
		}
		protected virtual void CheckNotRestoredItems(Dictionary<string, BaseLayoutItem> cache) { }
		protected virtual void OnItemFinded(Dictionary<string, BaseLayoutItem> cache, BaseLayoutItem item, string cacheKey) { }
		protected virtual BaseLayoutItem CreateItemByType(XtraPropertyInfo info, string typeStr) {
			BaseLayoutItem newItem = null;
			newItem = CreateFixedLayoutItem(typeStr) as BaseLayoutItem;
			if (newItem != null) return newItem;
			switch (typeStr) {
				case "EmptySpaceItem":
					newItem = CreateEmptySpaceItem(null);
					break;
				case "SplitterItem":
					newItem = CreateSplitterItem(null);
					break;
				case "LayoutRepositoryItem":
					newItem = new LayoutRepositoryItem();
					break;
				case "LayoutControlItem":
					newItem = CreateLayoutItem(null);
					break;
				case "TabbedControlGroup":
				case "TabbedGroup":
					newItem = CreateTabbedGroup(null);
					break;
				case "LayoutControlGroup":
				case "LayoutGroup":
					newItem = CreateLayoutGroup(null);
					string rootName = CreateRootName();
					if ((string)info.Value == rootName)
						owner.Root = (LayoutControlGroup)newItem;
					break;
			}
			return newItem;
		}
		public virtual void InitializeFixedHiddenItems() {
			HiddenItems.FixedTypes.Clear();
			HiddenItems.BeginRegistration();
			RegisterFixedItemType(typeof(EmptySpaceItem));
			RegisterFixedItemType(typeof(SimpleLabelItem));
			RegisterFixedItemType(typeof(SimpleSeparator));
			if (AllowUseSplitters) RegisterFixedItemType(typeof(SplitterItem));
			InitializeCustomFixedItems();
			HiddenItems.EndRegistration();
		}
		public virtual void RegisterFixedItemType(Type itemType) {
			HiddenItems.RegisterFixedItemType(itemType);
		}
		public virtual void UnRegisterFixedItemType(Type itemType) {
			HiddenItems.UnRegisterFixedItemType(itemType);
		}
		protected virtual void InitializeCustomFixedItems() { }
	}
	public class LayoutDesignerMethodsProvider : ILayoutDesignerMethods {
		LayoutControlImplementor owner = null;
		bool allowHandleEvents = true;
		bool allowHandleControlRemovedEvent = true;
		public LayoutDesignerMethodsProvider(LayoutControlImplementor owner) {
			this.owner = owner;
		}
		void ILayoutDesignerMethods.BeginChangeUpdate() {
			owner.RootGroup.BeginChangeUpdate();
		}
		LayoutControlDragDropHelper ILayoutDesignerMethods.DragDropDispatcherClientHelper {
			get {
				return null;
			}
		}
		bool ILayoutDesignerMethods.CanInvokePainting {
			get { return !owner.IsDirty; }
		}
		void ILayoutDesignerMethods.ResetResizer() {
			if (owner.RootGroup != null) owner.RootGroup.ResetResizer();
		}
		bool ILayoutDesignerMethods.AllowHandleEvents {
			get { return allowHandleEvents; }
			set { allowHandleEvents = value; }
		}
		bool ILayoutDesignerMethods.AllowHandleControlRemovedEvent {
			get { return allowHandleControlRemovedEvent; }
			set { allowHandleControlRemovedEvent = value; }
		}
		bool ILayoutDesignerMethods.IsInternalControl(Control control) {
			return control == owner.scrollerCore.HScroll ||
				control == owner.scrollerCore.VScroll ||
				control == owner.scrollerCore.EmptyArea ||
				control == owner.fakeFocusContainerCore;
		}
		void ILayoutDesignerMethods.RecreateHandle() {
		}
		bool ILayoutDesignerMethods.AllowDisposeControlOnItemDispose { get { return true; } set { } }
		void ILayoutDesignerMethods.EndChangeUpdate() {
			owner.RootGroup.EndChangeUpdate();
		}
		LayoutControlHandler ILayoutDesignerMethods.RuntimeHandler {
			get { return owner.RuntimeHandler; }
			set { owner.RuntimeHandler = value; }
		}
		event DeleteSelectedItemsEventHandler deleteSelectedItemsCore;
		event DeleteSelectedItemsEventHandler ILayoutDesignerMethods.DeleteSelectedItems {
			add { deleteSelectedItemsCore += value; }
			remove { deleteSelectedItemsCore -= value; }
		}
		void ILayoutDesignerMethods.RaiseDeleteSelectedItems(DeleteSelectedItemsEventArgs e) {
			if(deleteSelectedItemsCore != null) deleteSelectedItemsCore(owner != null ? owner.Control : null, e);
		}
	}
}
