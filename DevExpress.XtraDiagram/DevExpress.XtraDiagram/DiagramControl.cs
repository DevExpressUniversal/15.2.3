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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Diagram.Core;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraDiagram.Adorners;
using DevExpress.XtraDiagram.Animations;
using DevExpress.XtraDiagram.Appearance;
using DevExpress.XtraDiagram.Bars;
using DevExpress.XtraDiagram.Base;
using DevExpress.XtraDiagram.Commands;
using DevExpress.XtraDiagram.Designer;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Handlers;
using DevExpress.XtraDiagram.InplaceEditing;
using DevExpress.XtraDiagram.Layers;
using DevExpress.XtraDiagram.Layout;
using DevExpress.XtraDiagram.Options;
using DevExpress.XtraDiagram.Paint;
using DevExpress.XtraDiagram.Scrolling;
using DevExpress.XtraDiagram.Toolbox;
using DevExpress.XtraDiagram.Utils;
using DevExpress.XtraDiagram.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraToolbox;
namespace DevExpress.XtraDiagram {
	[
	DXToolboxItem(true),
	ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRoot), "DiagramControl"),
	ToolboxTabName(AssemblyInfo.DXTabData),
	Designer("DevExpress.XtraDiagram.Design.DiagramControlDesigner, " + AssemblyInfo.SRAssemblyDiagramDesignFull),
	Description("A diagramming tool."),
	]
	public partial class DiagramControl : BaseControl, ILayersHost, IServiceProvider, IMouseWheelSupport, ISupportInitialize, IDiagramInplaceEditOwner {
		static readonly object SelectionChangedEvent = new object();
		static readonly object AnimationFinishedEvent = new object();
		IDXMenuManager menuManager;
		IPropertiesEditor propertyGrid;
		IToolboxControl toolbox;
		DiagramRoot rootPage;
		bool hasChanges;
		DiagramOptionsView optionsView;
		DiagramOptionsBehavior optionsBehavior;
		DiagramOptionsDesigner optionsDesigner;
		DiagramAppearanceCollection appearance;
		XtraDiagramController diagramController;
		DiagramControlHandler diagramHandler;
		XtraLayersHostController layersHostController;
		DefaultDiagramScrollingController scrollingController;
		DiagramInplaceEditController inplaceEditController;
		DiagramAdornerController adornerController;
		DiagramControlPainter diagramPainter;
		DiagramControlViewInfo diagramViewInfo;
		DiagramAnimationController animationController;
		DiagramCommands diagramCommands;
		DiagramOptionsLayout optionsLayout;
		DiagramSerializationController serializationController;
		DiagramWeakEventManager weakEventManager;
		public static void About() {
			DevExpress.Utils.About.AboutHelper.Show(DevExpress.Utils.About.ProductKind.DXperienceWin, new DevExpress.Utils.About.ProductStringInfoWin(DevExpress.Utils.About.ProductInfoHelper.WinDiagram));
		}
		public DiagramControl() {
			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
			this.diagramPainter = CreatePainter();
			this.diagramViewInfo = CreateViewInfo();
			this.menuManager = null;
			this.propertyGrid = null;
			this.toolbox = null;
			this.rootPage = CreateRootPage();
			this.optionsView = CreateOptionsView();
			this.optionsView.Changing += OnOptionsViewChanging;
			this.optionsView.Changed += OnOptionsViewChanged;
			this.optionsBehavior = CreateOptionsBehavior();
			this.optionsBehavior.Changing += OnOptionsBehaviorChanging;
			this.optionsBehavior.Changed += OnOptionsBehaviorChanged;
			this.optionsDesigner = CreateOptionsDesigner();
			this.optionsDesigner.Changing += OnOptionsDesignerChanging;
			this.optionsDesigner.Changed += OnOptionsDesignerChanged;
			this.appearance = CreateAppearances();
			this.appearance.Changed += OnAppearanceChanged;
			this.diagramCommands = CreateDiagramCommands();
			this.diagramController = CreateDiagramController();
			this.diagramHandler = CreateDiagramHandler();
			this.layersHostController = CreateLayersHostController();
			this.scrollingController = CreateScrollingController();
			this.scrollingController.AddControls(this);
			this.scrollingController.HScrollValueChanged += OnHScrollValueChanged;
			this.scrollingController.VScrollValueChanged += OnVScrollValueChanged;
			InitializeControllers();
			this.inplaceEditController = CreateDiagramInplaceEditController();
			this.adornerController = CreateDiagramAdornerController();
			this.adornerController.StartInplaceEditing += OnStartInplaceEditing;
			this.adornerController.FinishInplaceEditing += OnFinishInplaceEditing;
			this.adornerController.InplaceEditRectChanged += OnInplaceEditRectChanged;
			this.animationController = CreateDiagramAnimationController();
			this.optionsLayout = CreateOptionsLayout();
			this.serializationController = CreateSerializationController();
			this.weakEventManager = new DiagramWeakEventManager(this);
			CanvasSizeMode = CanvasSizeMode.None;
		}
		protected virtual ToolboxController CreateToolboxController(IToolboxControl toolbox) {
			return new ToolboxController(toolbox);
		}
		protected override sealed BaseControlPainter Painter {
			get { return diagramPainter; }
		}
		protected override sealed BaseControlViewInfo ViewInfo {
			get { return diagramViewInfo; }
		}
		protected virtual DiagramControlPainter CreatePainter() {
			return new DiagramControlPainter();
		}
		protected virtual DiagramControlViewInfo CreateViewInfo() {
			return new DiagramControlViewInfo(this);
		}
		protected virtual DiagramBarCommandRepository CreateDiagramCommandRepository() {
			return new DiagramBarCommandRepository(this);
		}
		protected virtual XtraDiagramController CreateDiagramController() {
			return new XtraDiagramController(this);
		}
		protected virtual DiagramControlHandler CreateDiagramHandler() {
			return new DiagramControlHandler(this);
		}
		protected virtual DiagramCommands CreateDiagramCommands() {
			return new DiagramCommands(this);
		}
		protected virtual XtraLayersHostController CreateLayersHostController() {
			return new XtraLayersHostController(this);
		}
		protected virtual DefaultDiagramScrollingController CreateScrollingController() {
			return new DefaultDiagramScrollingController(this);
		}
		protected virtual DiagramRoot CreateRootPage() {
			return new DiagramRoot(this);
		}
		protected virtual DiagramOptionsView CreateOptionsView() {
			return new DiagramOptionsView();
		}
		protected virtual DiagramOptionsBehavior CreateOptionsBehavior() {
			return new DiagramOptionsBehavior(this);
		}
		protected virtual DiagramOptionsDesigner CreateOptionsDesigner() {
			return new DiagramOptionsDesigner();
		}
		protected virtual DiagramAppearanceCollection CreateAppearances() {
			return new DiagramAppearanceCollection();
		}
		protected virtual DiagramInplaceEditController CreateDiagramInplaceEditController() {
			return new DiagramInplaceEditController(this);
		}
		protected virtual DiagramAdornerController CreateDiagramAdornerController() {
			return new DiagramAdornerController(this);
		}
		protected virtual DiagramSerializationController CreateSerializationController() {
			return new DiagramSerializationController(this);
		}
		protected virtual DiagramAnimationController CreateDiagramAnimationController() {
			return new DiagramAnimationController(this);
		}
		protected virtual DiagramOptionsLayout CreateOptionsLayout() {
			return new DiagramOptionsLayout();
		}
		protected override Size DefaultSize { get { return new Size(250, 200); } }
		protected virtual void InitializeControllers() {
			Controller.LayersHost = this;
			LayersHostController.SetOwner(this);
		}
		protected internal DefaultDiagramScrollingController ScrollingController { get { return scrollingController; } }
		protected internal DiagramCommands DiagramCommands { get { return diagramCommands; } }
		protected override bool IsInputKey(Keys keyData) {
			if(keyData == Keys.Left || keyData == Keys.Right || keyData == Keys.Up || keyData == Keys.Down) {
				return true;
			}
			if(keyData == Keys.Tab) return OptionsBehavior.AllowTabNavigation();
			return base.IsInputKey(keyData);
		}
		protected internal DiagramAdornerController AdornerController { get { return adornerController; } }
		protected internal XtraLayersHostController LayersHostController { get { return layersHostController; } }
		DiagramBarCommandRepository commands = null;
		protected DiagramBarCommandRepository Commands {
			get {
				if(this.commands == null) {
					this.commands = CreateDiagramCommandRepository();
				}
				return this.commands;
			}
		}
		protected internal DiagramAnimationController AnimationController { get { return animationController; } }
		protected internal DiagramSerializationController SerializationController { get { return serializationController; } }
		#region Begin/End Update
		int lockUpdate = 0;
		public virtual void BeginUpdate() {
			if(this.lockUpdate == 0) {
				Controller.OnBeginInit();
			}
			this.lockUpdate++;
		}
		public virtual void CancelUpdate() { this.lockUpdate--; }
		public virtual void EndUpdate() {
			CancelUpdate();
			if(this.lockUpdate == 0) {
				Controller.OnEndInit();
			}
		}
		protected internal bool IsLockUpdate { get { return this.lockUpdate != 0 || Controller.IsInitializing; } }
		#endregion
		#region Public
		[DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DiagramAppearanceCollection Appearance { get { return appearance; } }
		void ResetAppearance() { Appearance.Reset(); }
		[DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DiagramOptionsView OptionsView { get { return optionsView; } }
		[DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DiagramOptionsBehavior OptionsBehavior { get { return optionsBehavior; } }
		[DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DiagramOptionsDesigner OptionsDesigner { get { return optionsDesigner; } }
		bool ShouldSerializeOptionsView() { return OptionsView.ShouldSerialize(this); }
		bool ShouldSerializeOptionsBehavior() { return OptionsBehavior.ShouldSerialize(this); }
		bool ShouldSerializeOptionsDesigner() { return OptionsDesigner.ShouldSerialize(this); }
		[DXCategory(CategoryName.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OptionsLayoutBase OptionsLayout { get { return optionsLayout; } }
		bool ShouldSerializeOptionsLayout() { return OptionsLayout.ShouldSerializeCore(this); }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public DiagramRoot Page { get { return rootPage; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DiagramItemCollection Items { get { return Page.Items; } }
		[DXCategory(CategoryName.Behavior), DefaultValue(null)]
		public IDXMenuManager MenuManager {
			get { return menuManager; }
			set { if(MenuManager != value) menuManager = value; }
		}
		[DXCategory(CategoryName.Behavior), DefaultValue(null)]
		public IPropertiesEditor PropertyGrid {
			get { return propertyGrid; }
			set {
				if(PropertyGrid == value)
					return;
				propertyGrid = value;
				OnPropertyGridChanged();
			}
		}
		protected virtual void OnPropertyGridChanged() {
			LayoutChanged();
		}
		ToolboxController toolboxController = null;
		protected virtual ToolboxController ToolboxController {
			get { return toolboxController; }
			set {
				if(ToolboxController == value) return;
				if(ToolboxController != null) {
					ToolboxController.Dispose();
				}
				toolboxController = value;
			}
		}
		[DXCategory(CategoryName.Behavior), DefaultValue(null)]
		public IToolboxControl Toolbox {
			get { return toolbox; }
			set {
				if(Toolbox == value)
					return;
				IToolboxControl prevValue = Toolbox;
				if(Toolbox != null) {
					Toolbox.ItemDoubleClick -= OnToolboxItemDoubleClick;
					Toolbox.DragItemStart -= OnToolboxDragItemStart;
					Toolbox.GetItemImage -= OnToolboxGetItemImage;
				}
				toolbox = value;
				if(Toolbox != null) {
					Toolbox.ItemDoubleClick += OnToolboxItemDoubleClick;
					Toolbox.DragItemStart += OnToolboxDragItemStart;
					Toolbox.GetItemImage += OnToolboxGetItemImage;
				}
				OnToolboxChanged(prevValue, Toolbox);
			}
		}
		protected virtual void OnToolboxChanged(IToolboxControl prev, IToolboxControl next) {
			if(next != null) {
				ToolboxController = CreateToolboxController(next);
				ToolboxController.InitializeToolbox();
			}
			else {
				ToolboxController = null;
			}
			LayoutChanged();
		}
		[Browsable(false)]
		public bool IsTextEditMode { get { return DiagramViewInfo.IsTextEditMode; } }
		public event DiagramSelectionChangedEventHandler SelectionChanged {
			add { Events.AddHandler(SelectionChangedEvent, value); }
			remove { Events.RemoveHandler(SelectionChangedEvent, value); }
		}
		protected virtual void OnSelectionChanged(DiagramSelectionChangedEventArgs e) {
			DiagramSelectionChangedEventHandler handler = (DiagramSelectionChangedEventHandler)Events[SelectionChangedEvent];
			if(handler != null) handler(this, e);
		}
		internal event EventHandler AnimationFinished {
			add { Events.AddHandler(AnimationFinishedEvent, value); }
			remove { Events.RemoveHandler(AnimationFinishedEvent, value); }
		}
		protected virtual void OnAnimationFinished(EventArgs e) {
			EventHandler handler = (EventHandler)Events[AnimationFinishedEvent];
			if(handler != null) handler(this, e);
		}
		protected DiagramShape CreateDefaultShape() {
			DiagramShape shape = new DiagramShape();
			InitializeShapeDefaults(shape);
			return shape;
		}
		protected virtual void InitializeShapeDefaults(DiagramShape shape) {
		}
		bool isNewDocumentLoaded = false;
		protected internal bool IsNewDocumentLoaded {
			get { return isNewDocumentLoaded; }
		}
		protected DiagramConnector CreateDefaultConnector() {
			DiagramConnector connector = new DiagramConnector();
			InitializeConnectorDefaults(connector);
			return connector;
		}
		protected virtual void InitializeConnectorDefaults(DiagramConnector connector) {
			connector.EndArrow = ArrowDescriptions.Filled90;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Color BackColor {
			get { return base.BackColor; }
			set { base.BackColor = value; }
		}
		public override void ResetBackColor() { BackColor = Color.Empty; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Color ForeColor {
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Font Font {
			get { return base.Font; }
			set { base.Font = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler ForeColorChanged {
			add { base.ForeColorChanged += value; }
			remove { base.ForeColorChanged -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler BackColorChanged {
			add { base.BackColorChanged += value; }
			remove { base.BackColorChanged -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler FontChanged {
			add { base.FontChanged += value; }
			remove { base.FontChanged -= value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Cursor Cursor {
			get { return base.Cursor; }
			set { base.Cursor = value; }
		}
		[Browsable(false)]
		public bool HasChanges {
			get { return hasChanges; }
			private set {
				if(HasChanges == value) return;
				hasChanges = value;
				OnHasChangesChanged();
			}
		}
		protected virtual void OnHasChangesChanged() {
			UpdateUI(DiagramIdleTask.UpdateAllTask);
		}
		public void SelectItem(DiagramItem item) {
			SelectItem(item, false);
		}
		public void SelectItem(DiagramItem item, bool addToSelection) {
			this.Selection().SelectItem(item, addToSelection);
		}
		public void SelectItems(IEnumerable<DiagramItem> items) {
			SelectItems(items, false);
		}
		public void SelectItems(params DiagramItem[] items) {
			SelectItems((IEnumerable<DiagramItem>)items);
		}
		public void SelectItems(IEnumerable<DiagramItem> items, bool addToSelection) {
			this.Selection().SelectItems(items, addToSelection);
		}
		public void ClearSelection() {
			this.Selection().ClearSelection();
		}
		[Browsable(false)]
		public DiagramItem[] SelectedItems {
			get { return this.SelectedItems().Cast<DiagramItem>().ToArray(); }
		}
		public void RunDesigner() {
			RunDesigner(FindForm());
		}
		public void RunDesigner(Form ownerForm) {
			using(DiagramDesignerForm designerFrm = new DiagramDesignerForm()) {
				designerFrm.Initialize(this);
				InitializeDesignerForm(designerFrm);
				designerFrm.ShowDialog(ownerForm);
				if(designerFrm.DialogResult == DialogResult.OK) {
					InitDiagramDocument(designerFrm.Diagram);
				}
			}
		}
		protected virtual void InitializeDesignerForm(DiagramDesignerForm designerFrm) {
			designerFrm.WindowState = FormWindowState.Maximized;
		}
		public void SaveDocument(string fileName) {
			DiagramExtensions.SaveDocument(this, fileName);
		}
		public void SaveDocument(Stream stream) {
			DiagramExtensions.SaveDocument(this, stream);
		}
		public void LoadDocument() {
			try {
				DiagramLayoutActions.LoadDocument(this);
				this.isNewDocumentLoaded = true;
				CanvasSizeMode = CanvasSizeMode.None;
			}
			catch(Exception) {
				throw;
			}
		}
		public void LoadDocument(string fileName) {
			Items.DestroyItems();
			try {
				DiagramExtensions.LoadDocument(this, fileName);
				this.isNewDocumentLoaded = true;
				CanvasSizeMode = CanvasSizeMode.None;
			}
			catch(Exception) {
				throw;
			}
		}
		public void LoadDocument(Stream stream) {
			Items.DestroyItems();
			try {
				DiagramExtensions.LoadDocument(this, stream);
				this.isNewDocumentLoaded = true;
				CanvasSizeMode = CanvasSizeMode.None;
			}
			catch(Exception) {
				throw;
			}
		}
		public void UpdateRoute() {
			foreach(DiagramItem item in Items) {
				DiagramConnector connector = item as DiagramConnector;
				if(connector != null)
					connector.UpdateRoute();
			}
		}
		public void CreateRibbon() {
			Control parent = Parent;
			RibbonControl ribbon = DiagramUtils.FindRibbon(this);
			if(ribbon == null) {
				ribbon = DiagramUtils.CreateRibbon();
				parent.Controls.Add(ribbon);
			}
			InitializeRibbon(ribbon);
		}
		public void InitializeRibbon(RibbonControl ribbon) {
			RibbonForm ribbonForm = Parent as RibbonForm;
			DiagramUtils.SetupRibbon(ribbon);
			if(ribbonForm != null) ribbonForm.Ribbon = ribbon;
			DiagramBarsHelper.AddDefaultBars(CreateBarsGenerator(ribbon.Manager));
			RaiseUpdateUI();
			MenuManager = ribbon;
		}
		#endregion
		internal void Cut() {
			CancelTextEditor();
			this.Commands().Execute(DiagramCommandsBase.CutCommand);
		}
		internal void Copy() {
			CancelTextEditor();
			this.Commands().Execute(DiagramCommandsBase.CopyCommand);
		}
		internal void Paste() {
			CancelTextEditor();
			this.Commands().Execute(DiagramCommandsBase.PasteCommand);
		}
		internal void Undo() {
			this.Commands().Execute(DiagramCommandsBase.UndoCommand);
		}
		internal void Redo() {
			this.Commands().Execute(DiagramCommandsBase.RedoCommand);
		}
		protected virtual void SubscribeIdleEvent(EventHandler handler) {
			Application.Idle += handler;
		}
		protected virtual void UnsubscribeIdleEvent(EventHandler handler) {
			Application.Idle -= handler;
		}
		DiagramIdleTask idleTask = DiagramIdleTask.EmptyTask;
		protected internal void UpdateUI(DiagramIdleTask task) {
			this.idleTask.Merge(task);
		}
		protected internal virtual void OnDiagramIdle() {
			if(SkipIdleTask()) return;
			if(this.idleTask.UpdateUIOnIdle) {
				if(this.idleTask.UpdateProperties) {
					if(PropertyGrid != null) PropertyGrid.InvalidateData();
				}
				if(this.idleTask.UpdateUICommands) {
					RaiseUpdateUI();
				}
				if(idleTask.CheckPaintCache) {
					DiagramViewInfo.CheckPaintCache();
				}
				this.idleTask = DiagramIdleTask.EmptyTask;
			}
		}
		protected bool SkipIdleTask() {
			if(AnimationController.InAnimation) return true;
			return false;
		}
		protected internal void InitDiagramDocument(DiagramControl other) {
			using(MemoryStream memoryStream = new MemoryStream()) {
				other.SaveDocument(memoryStream);
				memoryStream.Seek(0, SeekOrigin.Begin);
				LoadDocument(memoryStream);
				this.isNewDocumentLoaded = false;
			}
		}
		protected virtual void OnOptionsViewChanging(object sender, DiagramOptionsChangingEventArgs e) {
		}
		protected virtual void OnOptionsViewChanged(object sender, BaseOptionChangedEventArgs e) {
			DiagramViewOptionChangedEventArgs args = (DiagramViewOptionChangedEventArgs)e;
			if(args.IsGridSizeChanged) {
				OnGridSizeChanged();
			}
			if(args.IsPageSizeChanged) {
				Controller.OnExtentChanged();
			}
			if(args.IsScrollMarginChanged) {
				Controller.OnExtentChanged();
			}
			if(args.IsZoomFactorChanged) {
				Controller.OnZoomFactorChanged();
				UpdateUI(DiagramIdleTask.CheckPaintCacheTask);
			}
			if(args.IsThemeChanged) {
				Controller.OnThemeChanged();
			}
			if(args.IsDrawGridChanged || args.IsTransparentRulerBackgroundChanged) {
				DiagramViewInfo.PaintCache.ClearPageCache();
			}
			if(args.IsDrawRulersChanged) {
				UpdateViewPort();
			}
			OnPropertiesChanged();
		}
		protected virtual void OnOptionsBehaviorChanging(object sender, DiagramOptionsChangingEventArgs e) {
		}
		protected virtual void OnOptionsBehaviorChanged(object sender, BaseOptionChangedEventArgs e) {
			DiagramBehaviorOptionChangedEventArgs args = (DiagramBehaviorOptionChangedEventArgs)e;
			if(args.IsActiveToolChanged) {
				Controller.OnActiveToolChanged();
				UpdateUI(DiagramIdleTask.UpdateAllTask);
			}
			OnPropertiesChanged();
		}
		protected virtual void OnOptionsDesignerChanging(object sender, DiagramOptionsChangingEventArgs e) {
		}
		protected virtual void OnOptionsDesignerChanged(object sender, BaseOptionChangedEventArgs e) {
			OnPropertiesChanged();
		}
		protected virtual void OnAppearanceChanged(object sender, EventArgs e) {
			OnPropertiesChanged();
		}
		protected virtual DiagramRunTimeBarsGenerator CreateBarsGenerator(BarManager barManager) {
			return new DiagramRunTimeBarsGenerator(this, barManager);
		}
		#region Handlers
		protected internal DiagramControlHandler DiagramHandler { get { return diagramHandler; } }
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			DiagramHandler.OnKeyDown(e);
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			DiagramHandler.OnKeyUp(e);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			DiagramHandler.OnMouseDown(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			DiagramHandler.OnMouseUp(e);
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e) {
			base.OnMouseDoubleClick(e);
			DiagramHandler.OnMouseDoubleClick(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			DiagramHandler.OnMouseMove(e);
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			DiagramHandler.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			DiagramHandler.OnMouseLeave(e);
		}
		protected override void OnGotCapture() {
			base.OnGotCapture();
			DiagramHandler.OnGotCapture();
		}
		protected override void OnLostCapture() {
			base.OnLostCapture();
			DiagramHandler.OnLostCapture();
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			LayersHostController.UpdateOnChangeViewport();
			if(IsTextEditMode) UpdateActiveEditorBounds();
		}
		protected virtual void OnToolboxItemDoubleClick(object sender, ToolboxItemDoubleClickEventArgs e) {
			DiagramHandler.OnToolboxItemDoubleClick(e.Item);
		}
		protected virtual void OnToolboxDragItemStart(object sender, ToolboxDragItemStartEventArgs e) {
			DiagramHandler.OnToolboxDragItemStart(e);
		}
		protected virtual void OnToolboxGetItemImage(object sender, ToolboxGetItemImageEventArgs e) {
			DiagramHandler.OnToolboxGetItemImage(e);
		}
		protected internal void DoLayout() {
			LayoutChanged();
		}
		internal void DoFireChanged() { base.FireChanged(); }
		#endregion
		#region Scrolling
		int hScrollPos = 0;
		int vScrollPos = 0;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Point ScrollPos {
			get { return new Point(HScrollPos, VScrollPos); }
			set {
				if(ScrollPos == value)
					return;
				int newHScrollPos = CoerceHScrollPos(value.X);
				int newVScrollPos = CoerceVScrollPos(value.Y);
				if(newHScrollPos == HScrollPos && newVScrollPos == VScrollPos) return;
				hScrollPos = newHScrollPos;
				vScrollPos = newVScrollPos;
				OnScrollPosChanged();
			}
		}
		protected virtual void OnScrollPosChanged() {
			ScrollView(ScrollPos);
			LayoutChanged();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int HScrollPos {
			get { return hScrollPos; }
			set {
				if(HScrollPos == value)
					return;
				hScrollPos = CoerceHScrollPos(value);
				OnHScrollPosChanged();
			}
		}
		protected virtual void OnHScrollPosChanged() {
			ScrollView(new Point(HScrollPos, LayersHostControllerOffset.Y));
			LayoutChanged();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VScrollPos {
			get { return vScrollPos; }
			set {
				if(VScrollPos == value)
					return;
				vScrollPos = CoerceVScrollPos(value);
				OnVScrollPosChanged();
			}
		}
		protected virtual void OnVScrollPosChanged() {
			ScrollView(new Point(LayersHostControllerOffset.X, VScrollPos));
			LayoutChanged();
		}
		protected int CoerceHScrollPos(int value) {
			if(value < 0) return 0;
			if(value > DiagramViewInfo.MaximumHScrollValue) return DiagramViewInfo.MaximumHScrollValue;
			return value;
		}
		protected int CoerceVScrollPos(int value) {
			if(value < 0) return 0;
			if(value > DiagramViewInfo.MaximumVScrollValue) return DiagramViewInfo.MaximumVScrollValue;
			return value;
		}
		protected virtual void OnHScrollValueChanged(object sender, EventArgs e) {
			ScrollBarBase scrollBar = (ScrollBarBase)sender;
			ScrollView(new Point(scrollBar.Value, LayersHostControllerOffset.Y));
		}
		protected virtual void OnVScrollValueChanged(object sender, EventArgs e) {
			ScrollBarBase scrollBar = (ScrollBarBase)sender;
			ScrollView(new Point(LayersHostControllerOffset.X, scrollBar.Value));
		}
		[Browsable(false)]
		public bool IsHScrollVisible { get { return DiagramViewInfo.IsHScrollVisible; } }
		[Browsable(false)]
		public bool IsVScrollVisible { get { return DiagramViewInfo.IsVScrollVisible; } }
		protected virtual void ScrollView(Point scrollPos) {
			UpdateLayersHostController(scrollPos);
			if(IsTextEditMode) UpdateActiveEditorBounds();
		}
		protected virtual void UpdateLayersHostController(Point scrollPos) {
			if(LockScrollUpdate) return;
			this.lockScrollUpdate++;
			try {
				this.hScrollPos = scrollPos.X;
				this.vScrollPos = scrollPos.Y;
				LayersHostController.SetOffset(scrollPos);
			}
			finally {
				this.lockScrollUpdate--;
			}
		}
		protected Point LayersHostControllerOffset {
			get { return LayersHostController.Offset.ToWinPoint(); }
		}
		#endregion
		protected internal void ShowDiagramMenu(DiagramMenuPlacement placement) {
		}
		protected internal virtual bool ShowEditorMenu(Point point) {
			return false;
		}
		#region Inplace
		protected DiagramInplaceEditController InplaceEditController { get { return inplaceEditController; } }
		protected virtual void OnStartInplaceEditing(object sender, DiagramStartInplaceEditingEventArgs e) {
			ShowEditor(e.DisplayBounds, e.Item);
		}
		protected virtual void OnInplaceEditRectChanged(object sender, DiagramInplaceEditRectChangedEventArgs e) {
			UpdateEditorBounds(e.DisplayBounds, e.Item);
		}
		protected virtual void OnFinishInplaceEditing(object sender, DiagramFinishInplaceEditingEventArgs e) {
			if(InplaceEditController.IsEditorVisible) CloseEditor();
		}
		protected virtual void ShowEditor(Rectangle displayBounds, DiagramItem item) {
			DiagramEditInfoArgs editArgs = DiagramViewInfo.CalcDiagramEditInfoArgs(displayBounds, item);
			InplaceEditController.ShowEditor(item, editArgs);
		}
		protected internal virtual void RefreshActiveEditor() {
			Rectangle displayBounds = AdornerController.GetInplaceEditorAdornerDisplayBounds();
			RefreshEditor(displayBounds, InplaceEditController.EditItemInfo.EditItem);
		}
		protected internal virtual void RefreshEditor(Rectangle displayBounds, DiagramItem item) {
			DiagramEditInfoArgs editArgs = DiagramViewInfo.CalcDiagramEditInfoArgs(displayBounds, item);
			InplaceEditController.RefreshEditor(editArgs);
		}
		protected internal virtual void UpdateActiveEditorBounds() {
			Rectangle displayBounds = AdornerController.GetInplaceEditorAdornerDisplayBounds();
			UpdateEditorBounds(displayBounds, InplaceEditController.EditItemInfo.EditItem);
		}
		protected internal virtual void UpdateEditorBounds(Rectangle displayBounds, DiagramItem item) {
			DiagramEditInfoArgs editArgs = DiagramViewInfo.CalcDiagramEditInfoArgs(displayBounds, item);
			InplaceEditController.UpdateEditorBounds(editArgs);
		}
		protected virtual void CloseEditor() {
			PostEditor();
			InplaceEditController.HideEditor();
		}
		protected virtual bool PostEditor() {
			return InplaceEditController.PostEditor();
		}
		protected void CancelTextEditor() {
			if(InplaceEditController.IsEditorVisible) HideEditor();
		}
		protected internal virtual void HideEditor() {
			InplaceEditController.HideEditor();
			AdornerController.EnsureEditSurfaceDestroyed();
		}
		protected virtual void UpdateEditor(string editValue) {
			DiagramEditItemInfo inputArgs = InplaceEditController.EditItemInfo;
			DiagramItem item = inputArgs.EditItem;
			if(item.IsRoutable) {
				Rectangle bounds = UpdateInplaceEditorAdorner(item, editValue, inputArgs.EditArgs);
				inputArgs.EditArgs.UpdateAdornerBounds(bounds);
			}
			DiagramEditInfoArgs editArgs = DiagramViewInfo.UpdateDiagramEditInfoArgs(inputArgs.EditArgs, item, editValue);
			InplaceEditController.UpdateEditorBounds(editArgs);
		}
		protected Rectangle UpdateInplaceEditorAdorner(DiagramItem item, string editValue, DiagramEditInfoArgs editArgs) {
			DiagramInplaceEditorAdorner adorner = AdornerController.GetInplaceEditorAdorner();
			adorner.SetBestSize(DiagramViewInfo.CalcDiagramEditorBestSize(item, editValue));
			return adorner.DisplayBounds;
		}
		#endregion
		#region IDiagramInplaceEditControllerOwner
		void IDiagramInplaceEditOwner.AddControl(Control edit) {
			Controls.Add(edit);
		}
		void IDiagramInplaceEditOwner.RemoveControl(Control edit) {
			Controls.Remove(edit);
		}
		DiagramMaskBox IDiagramInplaceEditOwner.CreateDiagramEditor() {
			return CreateDiagramEditor();
		}
		void IDiagramInplaceEditOwner.OnEditKeyDown(KeyEventArgs e) {
			if(e.KeyCode == Keys.Escape) HideEditor();
		}
		void IDiagramInplaceEditOwner.OnEditKeyUp(KeyEventArgs e) {
		}
		void IDiagramInplaceEditOwner.OnEditMouseWheel(MouseEventArgs e) {
			OnMouseWheelCore(e);
		}
		void IDiagramInplaceEditOwner.OnEditTextChanged(string text) {
			UpdateEditor(text);
		}
		#endregion
		protected internal virtual DiagramMaskBox CreateDiagramEditor() {
			return new DiagramMaskBox(this);
		}
		protected override void LayoutChanged() {
			if(IsDisposing)
				return;
			if(IsLockUpdate || IsLoading) return;
			base.LayoutChanged();
			UpdateScrollBars();
		}
		protected void LayoutChanged(bool appearanceChanged) {
			if(appearanceChanged) DiagramViewInfo.OnAppearanceChanged();
			LayoutChanged();
		}
		protected override void RemoveXtraAnimator() {
			bool raiseEvent = GetDiagramActiveAnimationCount() > 0;
			base.RemoveXtraAnimator();
			if(raiseEvent) {
				OnAnimationFinished(EventArgs.Empty);
			}
		}
		protected int GetDiagramActiveAnimationCount() {
			return XtraAnimator.Current.Animations.GetAnimationsCountByObject(this);
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			UpdateViewPort();
			SubscribeIdleEvent(weakEventManager.OnApplicationIdle);
		}
		protected override void OnHandleDestroyed(EventArgs e) {
			UnsubscribeIdleEvent(weakEventManager.OnApplicationIdle);
			base.OnHandleDestroyed(e);
		}
		protected virtual void UpdateViewPort() {
			ViewInfo.CalcViewInfo(null);
			LayersHostController.UpdateOnChangeViewport();
		}
		int lockScrollUpdate = 0;
		protected virtual void UpdateScrollBars() {
			if(IsLockUpdate || IsLoading || LockScrollUpdate) return;
			lockScrollUpdate++;
			try {
				UpdateHScrollBar();
				UpdateVScrollBar();
				ScrollingController.ClientRect = ViewInfo.ClientRect;
			}
			finally {
				lockScrollUpdate--;
			}
		}
		protected bool LockScrollUpdate { get { return lockScrollUpdate != 0; } }
		protected virtual void UpdateHScrollBar() {
			ScrollingController.HScrollVisible = DiagramViewInfo.IsHScrollVisible;
			if(ScrollingController.HScrollVisible) {
				ScrollingController.HScrollArgs = DiagramViewInfo.CalcHScrollArgs();
			}
		}
		protected virtual void UpdateVScrollBar() {
			ScrollingController.VScrollVisible = DiagramViewInfo.IsVScrollVisible;
			if(ScrollingController.VScrollVisible) {
				ScrollingController.VScrollArgs = DiagramViewInfo.CalcVScrollArgs();
			}
		}
		public DiagramControlHitInfo CalcHitInfo(int x, int y) {
			return CalcHitInfo(new Point(x, y));
		}
		public DiagramControlHitInfo CalcHitInfo(Point pt) {
			return DiagramViewInfo.CalcHitInfo(pt);
		}
		[Browsable(false)]
		public DiagramControlViewInfo DiagramViewInfo {
			get { return diagramViewInfo; }
		}
		protected internal DiagramControlPainter DiagramPainter {
			get { return diagramPainter; }
		}
		protected virtual void RaiseSelectionChanged() {
			EnsureUpdatePropertyGrid();
			UpdateUI(DiagramIdleTask.UpdateAllTask);
			OnSelectionChanged(new DiagramSelectionChangedEventArgs());
		}
		protected virtual void EnsureUpdatePropertyGrid() {
			if(PropertyGrid == null) return;
			PropertyGrid.SelectedObject = SelectionModel;
			PropertyGrid.InvalidateRows();
		}
		protected internal XtraDiagramController Controller { get { return diagramController; } }
		public override bool IsLoading {
			get { return this.lockInit != 0; }
		}
		protected override bool IsLayoutLocked {
			get { return IsDisposing || IsLockUpdate || IsLoading; }
		}
		#region ISupportInitialize
		void ISupportInitialize.BeginInit() {
			OnBeginInit();
		}
		void ISupportInitialize.EndInit() {
			OnEndInit();
		}
		#endregion
		int lockInit = 0;
		protected virtual void OnBeginInit() {
			if(this.lockInit == 0) {
				Controller.OnBeginInit();
			}
			this.lockInit++;
		}
		protected virtual void OnEndInit() {
			if(--this.lockInit == 0) {
				Controller.OnEndInit();
				OnLoaded();
			}
		}
		protected virtual void OnLoaded() {
		}
		#region IMouseWheelSupport
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			OnMouseWheelCore(e);
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			if(XtraForm.ProcessSmartMouseWheel(this, e)) return;
			OnMouseWheelCore(e);
		}
		protected virtual void OnMouseWheelCore(MouseEventArgs ev) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(ev);
			try {
				base.OnMouseWheel(ee);
				if(!ee.Handled) {
					ee.Handled = true;
					DiagramHandler.OnMouseWheel(ee);
				}
			}
			finally {
				ee.Sync();
			}
		}
		#endregion
		#region Dispose
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.rootPage != null) {
					this.rootPage.Dispose();
				}
				this.rootPage = null;
				this.menuManager = null;
				this.propertyGrid = null;
				this.toolbox = null;
				this.diagramController = null;
				if(this.ViewInfo != null) {
					this.ViewInfo.Dispose();
				}
				this.layersHostController = null;
				if(this.animationController != null) {
					this.animationController.Dispose();
				}
				this.animationController = null;
				if(this.diagramHandler != null) {
					this.diagramHandler.Dispose();
					this.diagramHandler = null;
				}
				if(this.scrollingController != null) {
					this.scrollingController.HScrollValueChanged -= OnHScrollValueChanged;
					this.scrollingController.VScrollValueChanged -= OnVScrollValueChanged;
					this.scrollingController.RemoveControls(this);
					this.scrollingController.Dispose();
					this.scrollingController = null;
				}
				if(this.serializationController != null) {
					this.serializationController.Dispose();
					this.serializationController = null;
				}
				this.diagramCommands = null;
				if(this.appearance != null) {
					this.appearance.Changed -= OnAppearanceChanged;
					this.appearance.Dispose();
				}
				this.appearance = null;
				if(this.inplaceEditController != null) {
					this.inplaceEditController.Dispose();
				}
				this.inplaceEditController = null;
				if(this.adornerController != null) {
					this.adornerController.StartInplaceEditing -= OnStartInplaceEditing;
					this.adornerController.FinishInplaceEditing -= OnFinishInplaceEditing;
					this.adornerController.InplaceEditRectChanged -= OnInplaceEditRectChanged;
					this.adornerController.Dispose();
					this.adornerController = null;
				}
				if(this.toolboxController != null) {
					this.toolboxController.Dispose();
				}
				this.toolboxController = null;
			}
			base.Dispose(disposing);
		}
		#endregion
	}
}
