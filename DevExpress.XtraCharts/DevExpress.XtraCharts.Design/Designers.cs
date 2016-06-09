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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Design.DataAccess.UI;
using DevExpress.LookAndFeel;
using DevExpress.Utils.About;
using DevExpress.Utils.Design;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Design;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraCharts.Commands;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Designer.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.UI;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Design {
	public class ChartControlDesigner : ControlDesigner {
		static Point SnapToGrid(Point pnt, Size size) {
			return new Point((int)Math.Round((double)pnt.X / size.Width) * size.Width, 
							 (int)Math.Round((double)pnt.Y / size.Height) * size.Height);
		}
		ChartDesigner designer = null;
		IDesignerHost designerHost;
		IComponentChangeService changeService = null;
		IMenuCommandService menuCommandService = null;
		IToolboxService toolboxService = null;
		DesignerTransaction movingTransaction = null;
		bool changedInTransaction = false;
		Point mouseDragLast = ControlDesigner.InvalidPoint;
		ChartNavigationController navigationController = null;
		WinChartDesignerActionList chartDesignerActionList;
		public ChartDesigner CommonDesigner { get { return designer; } }
		public Chart Chart { get { return ((IChartContainer)Control).Chart; } }
		public IComponentChangeService ChangeService { get { return changeService; } }
		public IDesignerHost DesignerHost { get { return designerHost; } }
		ChartNavigationController NavigationController { 
			get {
				if (navigationController == null)
					navigationController = ChartDesignHelper.GetField(typeof(ChartControl), Control, "navigationController") as ChartNavigationController;
				return navigationController;
			}
		}
		public override DesignerActionListCollection ActionLists {
			get {
				DesignerActionListCollection actionLists = new DesignerActionListCollection();
				actionLists.AddRange(base.ActionLists);
				if (Component != null) {
					if (chartDesignerActionList != null)
						actionLists.Add(chartDesignerActionList);
					DXSmartTagsHelper.CreateDefaultLinks(this, actionLists);
				}
				return actionLists;
			}
		}
		[
		DefaultValue(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool ShowDesignerHints {
			get { return DesignerHintsHelper.ShowDesignerHints; } 
			set { DesignerHintsHelper.ShowDesignerHints = value; }
		}
		internal ChartControlDesigner() : base() {
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			designer = new ChartDesigner(component as IChartContainer);
			chartDesignerActionList = new WinChartDesignerActionList(this);
			designer.SetDesignerActionList(chartDesignerActionList);
			changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if (changeService != null) {
				changeService.ComponentAdded += new ComponentEventHandler(OnComponentAdded);
				changeService.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
			}
			designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
			if (designerHost != null)
				designerHost.LoadComplete += new EventHandler(OnLoadComplete);
			menuCommandService = (IMenuCommandService)GetService(typeof(IMenuCommandService));
			toolboxService = (IToolboxService)GetService(typeof(IToolboxService));
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (chartDesignerActionList != null) {
					chartDesignerActionList.Dispose();
					chartDesignerActionList = null;
				}
				if (designerHost != null)
					designerHost.LoadComplete -= new EventHandler(OnLoadComplete);
				if (changeService != null) {
					changeService.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
					changeService.ComponentAdded -= new ComponentEventHandler(OnComponentAdded);
				}
				if (designer != null) {
					designer.Dispose();
					designer = null;
				}
			}
			base.Dispose(disposing);
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			if (DesignerFormBase.ShowDesignerOnChartAdding) {
				DevExpress.XtraCharts.Designer.ChartDesigner designer =
					new DevExpress.XtraCharts.Designer.ChartDesigner((ChartControl)Control, designerHost);
				designer.ShowDialog();
			}
		}
		void SetVerbsState() {
			chartDesignerActionList.PopulateEnabled = Chart.DataContainer.CanFillDataSource();
			chartDesignerActionList.ClearDataSourceEnabled = Chart.DataContainer.CanClearDataSource();
			chartDesignerActionList.DataSnapshotEnabled = Chart.DataContainer.CanPerformDataSnapshot();
		}
		void BeginMovingTransaction() {
			if (designerHost == null || designerHost.InTransaction || changeService == null || NavigationController.InProcess)
				return;
			movingTransaction = designerHost.CreateTransaction("Move " + Control.Name);
			changedInTransaction = false;
		}
		void EndMovingTransaction() {
			if (movingTransaction != null) {
				changeService.OnComponentChanged(Control, null, null, null);
				movingTransaction.Commit();
				movingTransaction = null;
			}
		}
		void CancelMovingTransaction() {
			if (movingTransaction != null) {
				movingTransaction.Cancel();
				movingTransaction = null;
			}
		}
		internal void OnPopulate() {
			Cursor currentCursor = Cursor.Current;
			try {
				Cursor.Current = Cursors.WaitCursor;
				ChartDesignHelper.PopulateDataSource(Chart);
				SetVerbsState();
				Control.Invalidate();
			}
			finally {
				Cursor.Current = currentCursor;
			}
		}
		internal void OnClearDataSource() {
			Chart.ClearDataSource();
			Control.Invalidate();
			SetVerbsState();
		}
		internal void OnDataSnapshot() {
			Chart.DataSnapshot();
			Control.Invalidate();
			XtraMessageBox.Show((UserLookAndFeel)((IChartContainer)Control).RenderProvider.LookAndFeel,
				ChartLocalizer.GetString(ChartStringId.MsgDataSnapshot),
				Control.Name, MessageBoxButtons.OK,
				MessageBoxIcon.Information);
			SetVerbsState();
		}
		internal void OnAbout() {
		}
		void OnComponentAdded(object sender, ComponentEventArgs args) {
			if (!designerHost.Loading)
				if (Data.Native.BindingHelper.ConvertToIDataAdapter(args.Component) != null) {
					if (((ChartControl)Control).DataAdapter == null)
						((ChartControl)Control).DataAdapter = args.Component;
				}
				else if (args.Component is DataSet && ((DataSet)args.Component).Tables.Count > 0)
					if (((ChartControl)Control).DataSource == null) 
						((ChartControl)Control).DataSource = ((DataSet)args.Component).Tables[0];
		}
		void OnComponentChanged(object sender, ComponentChangedEventArgs args) {
			SetVerbsState();
		}
		void OnLoadComplete(object sender, EventArgs args) {
			SetVerbsState();
		}
		void SelectObjectByMouse() {
			Chart.SelectObjectsAt(Control.PointToClient(new Point(Cursor.Position.X, Cursor.Position.Y)));
		}
		MouseButtons DecodeButtons(IntPtr param) {
			const int MK_LBUTTON = 0x0001, MK_RBUTTON = 0x0002, MK_MBUTTON = 0x0010;
			int btns = param.ToInt32();
			MouseButtons buttons = MouseButtons.None;
			if ((btns & MK_LBUTTON) != 0) 
				buttons |= MouseButtons.Left;
			if ((btns & MK_RBUTTON) != 0) 
				buttons |= MouseButtons.Right;
			if ((btns & MK_MBUTTON) != 0) 
				buttons |= MouseButtons.Middle;
			return buttons;
		}
		void InvokeMouseMethod(string name, Message m, MouseButtons buttons) {
			if (Control == null)
				return;
			Point p = new Point(m.LParam.ToInt32());
			if (buttons == MouseButtons.None)
				buttons = DecodeButtons(m.WParam);
			ChartDesignHelper.InvokeMethod(Control, name, new object[] { new MouseEventArgs(buttons, 0, p.X, p.Y, 0)});
		}
		void DragProcedure() {
			if (movingTransaction == null || Control.Dock != DockStyle.None || 
				(SelectionRules & SelectionRules.Locked) != 0 || NavigationController.InProcess)
					return;
			Point newPos = new Point(Cursor.Position.X, Cursor.Position.Y);
			if (!newPos.Equals(mouseDragLast)) {
				Point newLocation = new Point(Control.Location.X + newPos.X - mouseDragLast.X,
											  Control.Location.Y + newPos.Y - mouseDragLast.Y);
				if (designerHost != null && designerHost.RootComponent != null) {
					Form form = designerHost.RootComponent as Form;
					if (form != null) {
						PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(form);
						PropertyDescriptor pdSnapToGrid = properties.Find("SnapToGrid", false);
						PropertyDescriptor pdGridSize = properties.Find("GridSize", false);
						if (pdSnapToGrid != null && pdGridSize != null) {
							if ((bool)pdSnapToGrid.GetValue(form)) {
								Size gridSize = (Size)pdGridSize.GetValue(form);
								newLocation = SnapToGrid(newLocation, gridSize);
								if (newLocation.Equals(SnapToGrid(Control.Location, gridSize)))
									return;
							}
						}
					}
				}
				try {
					changeService.OnComponentChanging(Control, null);
					changedInTransaction = true;
					Control.Location = newLocation;
				}
				catch {
				}
				mouseDragLast = newPos;
			}
		}
		void OnMouseMove(ref Message m) {
			MouseButtons buttons = DecodeButtons(m.WParam);
			if (Control.Capture && (buttons == MouseButtons.Left || buttons == MouseButtons.Middle)) {
				DragProcedure();
				if (designer.GetSelectedObject() == Control && !NavigationController.InProcess)
					base.WndProc(ref m);
			}
			InvokeMouseMethod("OnMouseMove", m, MouseButtons.None);
		}
		void OnMouseLeave(ref Message m) {
			InvokeMouseMethod("OnMouseLeave", m, MouseButtons.None);
		}
		void OnMouseLButtonDown(ref Message m) {
			InvokeMouseMethod("OnMouseDown", m, MouseButtons.None);
			if ((Control.ModifierKeys & Keys.Control) == Keys.Control) {
				if (Chart.Diagram is Diagram3D)
					designer.SelectChartControl();
			}
			else {
				ToolboxItem item = toolboxService.GetSelectedToolboxItem();
				if (item == null) {
					mouseDragLast = new Point(Cursor.Position.X, Cursor.Position.Y);
					if (designer.GetSelectedObject() == Control || (Chart.Diagram is Diagram3D))
						base.WndProc(ref m);
					else {
						Control.Capture = true;
						BeginMovingTransaction();
					}
				}
				else
					base.WndProc(ref m);
			}
		}
		void OnMouseRButtonDown(ref Message m) {
			InvokeMouseMethod("OnMouseDown", m, MouseButtons.Left);
		}
		void OnMouseMButtonDown(ref Message m) {
			InvokeMouseMethod("OnMouseDown", m, MouseButtons.Middle);
		}
		void OnMouseLButtonUp(ref Message m) {
			InvokeMouseMethod("OnMouseUp", m, MouseButtons.Left);
			if (Control.Capture) {
				DragProcedure();
				if (changedInTransaction)
					EndMovingTransaction();
				else
					CancelMovingTransaction();
				Control.Capture = false;
			}
			if (!(designer.GetSelectedObject() is FakeComponent) || (Chart.Diagram is Diagram3D))
				base.WndProc(ref m);
		}
		void OnMouseRButtonUp(ref Message m) {
			InvokeMouseMethod("OnMouseUp", m, MouseButtons.Left);
			if (menuCommandService != null)
				menuCommandService.ShowContextMenu(MenuCommands.SelectionMenu, 
					Cursor.Position.X, Cursor.Position.Y);
		}
		void OnMouseMButtonUp(ref Message m) {
			InvokeMouseMethod("OnMouseUp", m, MouseButtons.Middle);
		}
		void OnKeyDown(ref Message m) {
			if (Control != null)
				ChartDesignHelper.InvokeMethod(Control, "OnKeyDown", new object[] { new KeyPressEventArgs((char)m.WParam) });
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			PropertyInfo property = GetType().GetProperty(DesignerHintsHelper.ShowDesignerHintsPropertyName);
			if (property == null)
				return;
			PropertyDescriptor desc = TypeDescriptor.CreateProperty(GetType(), DesignerHintsHelper.ShowDesignerHintsPropertyName,
				property.PropertyType, new Attribute[] { CategoryAttribute.Design } );
			if (desc != null)
				properties[DesignerHintsHelper.ShowDesignerHintsPropertyName] = desc;
		}
		protected override void PostFilterProperties(IDictionary properties) {
			base.PostFilterProperties(properties);
			ChartDesigner.PostFilterProperties(Component, properties);
		}
		protected override void OnPaintAdornments(PaintEventArgs pe) {
			base.OnPaintAdornments(pe);
			BaseControlDesigner.CheckDrawExpired(pe, this);
		}
		protected override void WndProc(ref Message m) {
			const int WM_MOUSEMOVE = 0x200, WM_MOUSELEAVE = 0x2A3;
			const int WM_LBUTTONDOWN = 0x201, WM_LBUTTONUP = 0x202;
			const int WM_RBUTTONDOWN = 0x204, WM_RBUTTONUP = 0x205;
			const int WM_MBUTTONDOWN = 0x207, WM_MBUTTONUP = 0x208;
			const int WM_KEYDOWN = 0x100;
			switch (m.Msg) {
			case WM_MOUSEMOVE:
				OnMouseMove(ref m);
				break;
			case WM_MOUSELEAVE:
				OnMouseLeave(ref m);
				break;
			case WM_LBUTTONDOWN:
				OnMouseLButtonDown(ref m);
				break;
			case WM_RBUTTONDOWN:
				OnMouseRButtonDown(ref m);
				break;
			case WM_MBUTTONDOWN:
				OnMouseMButtonDown(ref m);
				break;
			case WM_LBUTTONUP:
				OnMouseLButtonUp(ref m);
				break;
			case WM_RBUTTONUP:
				OnMouseRButtonUp(ref m);
				break;
			case WM_MBUTTONUP:
				OnMouseMButtonUp(ref m);
				break;
			case WM_KEYDOWN:
				OnKeyDown(ref m);
				break;
			default:
				base.WndProc(ref m);
				break;
			}
		}
		internal void CreateDataSource() {
			DataSourceWizard.Run(designerHost, Component);
		}
	}
	public class WinChartDesignerActionList : ChartDesignerActionList, IDisposable {
		static void AddActionItem(DesignerActionItemCollection items, DesignerActionItem newItem) {
			newItem.AllowAssociate = true;
			items.Add(newItem);
		}
		DesignerActionUIService actionService;
		ChartControlDesigner chartDesigner;
		bool populateEnabled;
		bool clearDataSourceEnabled;
		bool dataSnapshotEnabled;
		public bool PopulateEnabled { get { return populateEnabled; } set { populateEnabled = value; } }
		public bool ClearDataSourceEnabled { get { return clearDataSourceEnabled; } set { clearDataSourceEnabled = value; } }
		public bool DataSnapshotEnabled { get { return dataSnapshotEnabled; } set { dataSnapshotEnabled = value; } }
		[AttributeProvider(typeof(IListSource))]
		public object DataSource {
			get { return chartDesigner.Chart.DataContainer.DataSource; }
			set {
				chartDesigner.Chart.DataContainer.DataSource = value;
				Refresh();
			}
		}
		public WinChartDesignerActionList(ChartControlDesigner chartDesigner) : base(chartDesigner.CommonDesigner, chartDesigner.Component) {
			this.chartDesigner = chartDesigner;
			actionService = GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
		}
		public void Dispose() {
			if (actionService != null)
				actionService = null;			
		}
		void Refresh() {
			if (actionService != null)
				actionService.Refresh(Component);
		}
		void OnPopulateAction() {
			chartDesigner.OnPopulate();
			Refresh();
		}
		void CreateDataSource() {
			chartDesigner.CreateDataSource();
		}
		void OnClearDataSourceAction() {
			chartDesigner.OnClearDataSource();
			Refresh();
		}
		void OnDataSnapshotAction() {
			chartDesigner.OnDataSnapshot();
			Refresh();
		}
		void OnAboutAction() {
			chartDesigner.OnAbout();
			Refresh();
		}
		void AddPopulateAction(DesignerActionItemCollection actionItems, string category) {
			if (PopulateEnabled)
				AddActionItem(actionItems, 
					new DesignerActionMethodItem(this, "OnPopulateAction", 
						ChartLocalizer.GetString(ChartStringId.VerbPopulate), category, 
						ChartLocalizer.GetString(ChartStringId.VerbPopulateDescription), false));
		}
		void AddClearDataSourceAction(DesignerActionItemCollection actionItems, string category) {
			if (ClearDataSourceEnabled)
				AddActionItem(actionItems, 
					new DesignerActionMethodItem(this, "OnClearDataSourceAction", 
						ChartLocalizer.GetString(ChartStringId.VerbClearDataSource), category, 
						ChartLocalizer.GetString(ChartStringId.VerbClearDataSourceDescription), false));
		}
		void AddDataSnapshotAction(DesignerActionItemCollection actionItems, string category) {
			if (DataSnapshotEnabled)
				AddActionItem(actionItems, 
					new DesignerActionMethodItem(this, "OnDataSnapshotAction", 
						ChartLocalizer.GetString(ChartStringId.VerbDataSnapshot), category, 
						ChartLocalizer.GetString(ChartStringId.VerbDataSnapshotDescription), false));
		}
		void AddAboutAction(DesignerActionItemCollection actionItems, string category) {
			AddActionItem(actionItems, 
				new DesignerActionMethodItem(this, "OnAboutAction", 
					ChartLocalizer.GetString(ChartStringId.VerbAbout), category, 
					ChartLocalizer.GetString(ChartStringId.VerbAboutDescription), true));
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			bool enabled = ChartDesigner.GetInheritanceAttribute(chartDesigner.Chart.Container) == InheritanceAttribute.NotInherited;
			AnnotationsActionEnabled = enabled;
			SeriesActionEnabled = enabled;
			EditPalettesEnabled = enabled;
			SaveLayoutEnabled = true;
			LoadLayoutEnabled = enabled;
			WizardEnabled = enabled;
			string dataSourceCategory = "data source";
			string commonCategory = "common";
			string serializingCategory = "serializing";
			DesignerActionItemCollection actionItems = new DesignerActionItemCollection();
			actionItems.Add(new DataSourceWizardDesignerActionMethodItem(this, dataSourceCategory));
			actionItems.Add(new DesignerActionPropertyItem("DataSource", "Choose Data Source", dataSourceCategory));
			AddPopulateAction(actionItems, dataSourceCategory);
			AddClearDataSourceAction(actionItems, dataSourceCategory);
			AddDataSnapshotAction(actionItems, dataSourceCategory);
			AddWizardAction(actionItems, commonCategory);
			AddSeriesAction(actionItems, commonCategory);
			AddAnnotationsAction(actionItems, commonCategory);
			AddEditPalettesAction(actionItems, commonCategory);
			AddSaveLayoutAction(actionItems, serializingCategory);
			AddLoadLayoutAction(actionItems, serializingCategory);
			AddAboutAction(actionItems, commonCategory);
			if (!ChartDesignTimeBarsGenerator.IsExistBarContainer(Component)) {
				actionItems.Add(new DesignerActionMethodItem(this, "CreateRibbon", "Create Ribbon", "Toolbar"));
				actionItems.Add(new DesignerActionMethodItem(this, "CreateBarManager", "Create BarManager", "Toolbar"));
			}
			else
				actionItems.Add(new DesignerActionMethodItem(this, "CreateAllBars", "Create All Bars", "Toolbar"));
			return actionItems;
		}
		[RefreshProperties(RefreshProperties.All)]
		public void CreateAllBars() {
			CreateAllBarsCore(BarInsertMode.Add);
		}
		[RefreshProperties(RefreshProperties.All)]
		public void CreateRibbon() {
			ChartControl control = chartDesigner.Control as ChartControl;
			if (control == null)
				return;
			IContainer container = chartDesigner.DesignerHost.Container;
			if (container == null)
				return;
			Control form = DevExpress.XtraBars.Design.DesignHelpers.GetContainerControl(container);
			if (form == null)
				return;
			using (DesignerTransaction transaction = chartDesigner.DesignerHost.CreateTransaction("Create Ribbon")) {
				RibbonControl ribbon = (RibbonControl)chartDesigner.DesignerHost.CreateComponent(typeof(RibbonControl));
				chartDesigner.ChangeService.OnComponentChanging(form, null);
				form.Controls.Add(ribbon);
				RibbonForm ribbonForm = form as RibbonForm;
				if (ribbonForm != null)
					ribbonForm.Ribbon = ribbon;
				chartDesigner.ChangeService.OnComponentChanging(form, null);
				transaction.Commit();
				EditorContextHelperEx.RefreshSmartPanel(Component);
				CreateAllBars();
			}
		}
		[RefreshProperties(RefreshProperties.All)]
		public void CreateBarManager() {
			ChartControl control = chartDesigner.Control as ChartControl;
			IContainer container = chartDesigner.DesignerHost.Container;
			if (container == null)
				return;
			Control form = DevExpress.XtraBars.Design.DesignHelpers.GetContainerControl(container);
			if (form == null)
				return;
			using (DesignerTransaction transaction = chartDesigner.DesignerHost.CreateTransaction("Create BarManager")) {
				chartDesigner.ChangeService.OnComponentChanging(form, null);
				chartDesigner.ChangeService.OnComponentChanged(form, null, null, null);
				BarManager barManager = (BarManager)chartDesigner.DesignerHost.CreateComponent(typeof(BarManager));
				chartDesigner.ChangeService.OnComponentChanging(container, null);
				container.Add(barManager);
				chartDesigner.ChangeService.OnComponentChanged(container, null, null, null);
				chartDesigner.ChangeService.OnComponentChanging(barManager, null);
				barManager.Form = form;
				chartDesigner.ChangeService.OnComponentChanged(barManager, null, null, null);
				transaction.Commit();
				EditorContextHelperEx.RefreshSmartPanel(Component);
				CreateAllBars();
			}
		}
		protected void AddNewBars(ControlCommandBarCreator[] creators, string barName, BarInsertMode insertMode) {
			ChartDesignTimeBarsGenerator generator = CreateGenerator();
			generator.AddNewBars(creators, barName, insertMode);
		}
		protected virtual ChartDesignTimeBarsGenerator CreateGenerator() {
			return new ChartDesignTimeBarsGenerator(chartDesigner.DesignerHost, chartDesigner.Control);
		}
		public void CreateAllBarsCore(BarInsertMode insertMode) {
			using (DesignerTransaction transaction = chartDesigner.DesignerHost.CreateTransaction("Create Chart Bars")) {
				List<ControlCommandBarCreator[]> creators = new List<ControlCommandBarCreator[]>();
				creators.Add(GetChartBarCreators());
				int first, last, step;
				if (insertMode == BarInsertMode.Add) {
					first = 0;
					last = creators.Count;
					step = 1;
				}
				else {
					first = creators.Count - 1;
					last = -1;
					step = -1;
				}
				for (int i = first; i != last; i += step)
					AddNewBars(creators[i], "Create Chart Bars", insertMode);
				transaction.Commit();
			}
		}
		protected ControlCommandBarCreator[] GetChartBarCreators() {
			ControlCommandBarCreator typesCreator = new ChartTypesBarCreator();
			ControlCommandBarCreator appearanceCreator = new ChartAppearanceBarCreator();
			ControlCommandBarCreator wizardCreator = new ChartWizardBarCreator();
			ControlCommandBarCreator templatesCreator = new ChartTemplatesBarCreator();
			ControlCommandBarCreator printExportCreator = new ChartPrintExportBarCreator();
			return new ControlCommandBarCreator[] { typesCreator, appearanceCreator, 
				wizardCreator, templatesCreator, printExportCreator };
		}
	}
	public class ChartBarControllerDesigner : ControlCommandBarControllerDesigner<IChartContainer, ChartCommandId> {
		protected override DXAboutActionList GetAboutAction() {
			return new DXAboutActionList(Component, new MethodInvoker(ChartControl.About));
		}
	}
	#region ChartDesignTimeBarsGenerator
	public class ChartDesignTimeBarsGenerator : DesignTimeBarsGenerator<IChartContainer, ChartCommandId> {
		public ChartDesignTimeBarsGenerator(IDesignerHost host, IComponent component)
			: base(host, component) {
		}
		protected override BarGenerationManagerFactory<IChartContainer, ChartCommandId> CreateBarGenerationManagerFactory() {
			return new ChartBarGenerationManagerFactory();
		}
		protected override ControlCommandBarControllerBase<IChartContainer, ChartCommandId> CreateBarController() {
			return new ChartBarController();
		}
		protected override void EnsureReferences(IDesignerHost designerHost) {
			ReferencesHelper.EnsureReferences(designerHost, AssemblyInfo.SRAssemblyChartsExtensions);
		}
	}
	#endregion
	#region ChartBarGenerationManager
	public class ChartBarGenerationManager : BarGenerationManager<IChartContainer, ChartCommandId> {
		public ChartBarGenerationManager(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<IChartContainer, ChartCommandId> barController)
			: base(creator, container, barController) {
		}
	}
	#endregion
	#region ChartRibbonGenerationManager
	public class ChartRibbonGenerationManager : RibbonGenerationManager<IChartContainer, ChartCommandId> {
		public ChartRibbonGenerationManager(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<IChartContainer, ChartCommandId> barController)
			: base(creator, container, barController) {
		}
		protected override ChartCommandId EmptyCommandId { get { return ChartCommandId.None; } }
	}
	#endregion
	#region ChartBarGenerationManagerFactory
	public class ChartBarGenerationManagerFactory : BarGenerationManagerFactory<IChartContainer, ChartCommandId> {
		protected override RibbonGenerationManager<IChartContainer, ChartCommandId> CreateRibbonGenerationManagerInstance(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<IChartContainer, ChartCommandId> barController) {
			return new ChartRibbonGenerationManager(creator, container, barController);
		}
		protected override BarGenerationManager<IChartContainer, ChartCommandId> CreateBarGenerationManagerInstance(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<IChartContainer, ChartCommandId> barController) {
			return new ChartBarGenerationManager(creator, container, barController);
		}
	}
	#endregion
	#region ChartStatusBarGenerationManager
	public class ChartStatusBarGenerationManager : StatusBarGenerationManager<IChartContainer, ChartCommandId> {
		public ChartStatusBarGenerationManager(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<IChartContainer, ChartCommandId> barController)
			: base(creator, container, barController) {
		}
	}
	#endregion
	#region ChartRibbonStatusBarGenerationManager
	public class ChartRibbonStatusBarGenerationManager : RibbonStatusBarGenerationManager<IChartContainer, ChartCommandId> {
		public ChartRibbonStatusBarGenerationManager(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<IChartContainer, ChartCommandId> barController)
			: base(creator, container, barController) {
		}
		protected override ChartCommandId EmptyCommandId { get { return ChartCommandId.None; } }
	}
	#endregion
}
namespace DevExpress.XtraCharts.Native {
	public static class ChartControlDesignerCreator {
		public static ChartControlDesigner CreateDesigner() {
			return new ChartControlDesigner();
		}
	}
}
