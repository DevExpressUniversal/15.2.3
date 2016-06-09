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
using System.Linq;
using System.CodeDom;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.Xpo;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Design;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.UI;
using System.Collections.Generic;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars;
using System.Resources;
using DevExpress.XtraScheduler.Design.ItemTemplates;
using System.Net;
using EnvDTE;
using DevExpress.XtraEditors;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.IO;
using EnvDTE80;
namespace DevExpress.XtraScheduler.Design {
	#region SchedulerControlAppointmentsActionList
	public class SchedulerControlAppointmentsActionList : SchedulerStorageAppointmentsActionList {
		public SchedulerControlAppointmentsActionList(IComponent component)
			: base(component) {
		}
		protected override IPersistentObjectStorage<Appointment> ObjectStorage {
			get {
				ISchedulerStorageBase storage = Storage;
				return storage != null ? storage.Appointments : null;
			}
		}
		protected override ISchedulerStorageBase Storage { get { return Component != null ? ((SchedulerControl)Component).DataStorage : null; } }
	}
	#endregion
	public class SchedulerControlDBCreationActionList : DesignerActionList {
		string SRHowToCreateDB = "Create Sample Database";
		string SRHowToCreateGanttDB = "Create Sample Database for Gantt View";
		readonly SchedulerControlDesigner designer;
		public SchedulerControlDBCreationActionList(SchedulerControlDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		#region Properties
		SchedulerControl SchedulerControl { get { return (SchedulerControl)Component; } }
		public SchedulerControlDesigner Designer { get { return designer; } }
		#endregion
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection result = new DesignerActionItemCollection();
			if (SchedulerControl.DataStorage != null && SchedulerControl.DataStorage.Appointments.DataSource == null) {
				result.Add(new DesignerActionMethodItem(this, "CreateDB", SRHowToCreateDB));
				result.Add(new DesignerActionMethodItem(this, "CreateGanttDB", SRHowToCreateGanttDB));
			}
			return result;
		}
		public void CreateDB() {
			Form form = new DataBaseCreationScriptForm(DBCreationScriptInfo.Sample);
			form.ShowDialog();
		}
		public void CreateGanttDB() {
			Form form = new DataBaseCreationScriptForm(DBCreationScriptInfo.SampleGantt);
			form.ShowDialog();
		}
	}
	#region SchedulerControlResourcesActionList
	public class SchedulerControlResourcesActionList : SchedulerStorageResourcesActionList {
		public SchedulerControlResourcesActionList(IComponent component)
			: base(component) {
		}
		protected override IPersistentObjectStorage<Resource> ObjectStorage {
			get {
				ISchedulerStorageBase storage = Storage;
				return storage != null ? storage.Resources : null;
			}
		}
		protected override ISchedulerStorageBase Storage { get { return Component != null ? ((SchedulerControl)Component).DataStorage : null; } }
	}
	#endregion
	#region SchedulerControlAppointmentDependenciesActionList
	public class SchedulerControlAppointmentDependenciesActionList : SchedulerStorageAppointmentDependenciesActionList {
		public SchedulerControlAppointmentDependenciesActionList(IComponent component)
			: base(component) {
		}
		protected override IPersistentObjectStorage<AppointmentDependency> ObjectStorage {
			get {
				ISchedulerStorageBase storage = Storage;
				return storage != null ? storage.AppointmentDependencies : null;
			}
		}
		protected override ISchedulerStorageBase Storage { get { return Component != null ? ((SchedulerControl)Component).DataStorage : null; } }
	}
	#endregion
	#region XtraSchedulerSuiteControlDesigner
	public class XtraSchedulerSuiteControlDesigner : BaseControlDesigner, IServiceProvider {
		#region Fields
		readonly DesignerVerbCollection verbs;
		IComponentChangeService changeService;
		IDesignerHost host;
		#endregion
		public XtraSchedulerSuiteControlDesigner() {
			SchedulerRepositoryItemsRegistrator.Register();
			this.verbs = new DesignerVerbCollection(new DesignerVerb[] { new DesignerVerb("About...", OnAboutClick) });
		}
		#region Properties
		protected IComponentChangeService ChangeService { get { return changeService; } }
		public IDesignerHost DesignerHost {
			get {
				if (host == null)
					host = (IDesignerHost)GetService(typeof(IDesignerHost));
				return host;
			}
		}
		protected override bool AllowHookDebugMode { get { return true; } }
		public override DesignerVerbCollection DXVerbs { get { return verbs; } }
		#endregion
		void OnAboutClick(object sender, EventArgs e) {
			SchedulerControl.About();
		}
		object IServiceProvider.GetService(Type type) {
			return base.GetService(type);
		}
		protected override DXAboutActionList GetAboutAction() {
			return new DXAboutActionList(Component, new MethodInvoker(SchedulerControl.About));
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			SchedulerAutomaticBindHelper.BindToSchedulerControl(this);
			SchedulerAutomaticBindHelper.BindToSchedulerStorage(this);
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new BindSchedulerAndStorageActionList(this));
			base.RegisterActionLists(list);
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			this.changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if (changeService != null) {
				changeService.ComponentAdded += new ComponentEventHandler(OnComponentAdded);
				changeService.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
			}
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (changeService != null) {
						changeService.ComponentAdded -= new ComponentEventHandler(OnComponentAdded);
						changeService.ComponentRemoved -= new ComponentEventHandler(OnComponentRemoved);
					}
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		protected virtual void OnComponentRemoved(object sender, ComponentEventArgs e) {
			SchedulerAutomaticBindHelper.UnbindFromRemovedComponent(this, e.Component, "SchedulerControl", typeof(SchedulerControl));
			SchedulerAutomaticBindHelper.UnbindFromRemovedComponent(this, e.Component, "Storage", typeof(SchedulerStorage));
		}
		protected virtual void OnComponentAdded(object sender, ComponentEventArgs e) {
			IDesignerHost host = sender as IDesignerHost;
			if (host != null && host.Loading)
				return;
			bool isAttachableComponent = e.Component is SchedulerControl || e.Component is SchedulerStorage;
			if (!isAttachableComponent)
				return;
			SchedulerAutomaticBindHelper.BindToSchedulerControl(this);
			SchedulerAutomaticBindHelper.BindToSchedulerStorage(this);
		}
	}
	#endregion
	#region XtraSchedulerSuiteImageComboBoxDesigner
	[CLSCompliant(false)]
	public class XtraSchedulerSuiteImageComboBoxDesigner : ImageComboBoxEditDesigner, IServiceProvider {
		#region Fields
		IComponentChangeService changeService;
		IDesignerHost host;
		#endregion
		public XtraSchedulerSuiteImageComboBoxDesigner() {
			SchedulerRepositoryItemsRegistrator.Register();
		}
		#region Properties
		protected IComponentChangeService ChangeService { get { return changeService; } }
		protected IDesignerHost DesignerHost { get { return host; } }
		protected override bool AllowHookDebugMode { get { return true; } }
		#endregion
		object IServiceProvider.GetService(Type type) {
			return base.GetService(type);
		}
		protected override DXAboutActionList GetAboutAction() {
			return new DXAboutActionList(Component, new MethodInvoker(SchedulerControl.About));
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			this.host = (IDesignerHost)GetService(typeof(IDesignerHost));
			SchedulerAutomaticBindHelper.BindToSchedulerControl(this);
			SchedulerAutomaticBindHelper.BindToSchedulerStorage(this);
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new BindSchedulerAndStorageActionList(this));
			base.RegisterActionLists(list);
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			this.changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if (changeService != null) {
				changeService.ComponentAdded += new ComponentEventHandler(OnComponentAdded);
				changeService.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
			}
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (changeService != null) {
						changeService.ComponentAdded -= new ComponentEventHandler(OnComponentAdded);
						changeService.ComponentRemoved -= new ComponentEventHandler(OnComponentRemoved);
					}
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		protected virtual void OnComponentRemoved(object sender, ComponentEventArgs e) {
			SchedulerAutomaticBindHelper.UnbindFromRemovedComponent(this, e.Component, "SchedulerControl", typeof(SchedulerControl));
			SchedulerAutomaticBindHelper.UnbindFromRemovedComponent(this, e.Component, "Storage", typeof(SchedulerStorage));
		}
		protected virtual void OnComponentAdded(object sender, ComponentEventArgs e) {
			SchedulerAutomaticBindHelper.BindToSchedulerControl(this);
			SchedulerAutomaticBindHelper.BindToSchedulerStorage(this);
		}
	}
	#endregion
	#region XtraSchedulerSuitePopupContainerEditDesigner
	[CLSCompliant(false)]
	public class XtraSchedulerSuitePopupContainerEditDesigner : PopupContainerEditDesigner, IServiceProvider {
		#region Fields
		IComponentChangeService changeService;
		IDesignerHost host;
		#endregion
		public XtraSchedulerSuitePopupContainerEditDesigner() {
			SchedulerRepositoryItemsRegistrator.Register();
		}
		#region Properties
		protected IComponentChangeService ChangeService { get { return changeService; } }
		protected IDesignerHost DesignerHost { get { return host; } }
		protected override bool AllowHookDebugMode { get { return true; } }
		#endregion
		object IServiceProvider.GetService(Type type) {
			return base.GetService(type);
		}
		protected override DXAboutActionList GetAboutAction() {
			return new DXAboutActionList(Component, new MethodInvoker(SchedulerControl.About));
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			this.host = (IDesignerHost)GetService(typeof(IDesignerHost));
			SchedulerAutomaticBindHelper.BindToSchedulerControl(this);
			SchedulerAutomaticBindHelper.BindToSchedulerStorage(this);
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new BindSchedulerAndStorageActionList(this));
			base.RegisterActionLists(list);
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			this.changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if (changeService != null) {
				changeService.ComponentAdded += new ComponentEventHandler(OnComponentAdded);
				changeService.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
			}
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (changeService != null) {
						changeService.ComponentAdded -= new ComponentEventHandler(OnComponentAdded);
						changeService.ComponentRemoved -= new ComponentEventHandler(OnComponentRemoved);
					}
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		protected virtual void OnComponentRemoved(object sender, ComponentEventArgs e) {
			SchedulerAutomaticBindHelper.UnbindFromRemovedComponent(this, e.Component, "SchedulerControl", typeof(SchedulerControl));
			SchedulerAutomaticBindHelper.UnbindFromRemovedComponent(this, e.Component, "Storage", typeof(SchedulerStorage));
		}
		protected virtual void OnComponentAdded(object sender, ComponentEventArgs e) {
			SchedulerAutomaticBindHelper.BindToSchedulerControl(this);
			SchedulerAutomaticBindHelper.BindToSchedulerStorage(this);
		}
	}
	#endregion
	#region SchedulerAutomaticBindHelper
	public sealed class SchedulerAutomaticBindHelper : AutomaticBindHelper {
		SchedulerAutomaticBindHelper() {
		}
		public static bool BindToSchedulerControl(ControlDesigner designer) {
			return BindToComponent(designer, "SchedulerControl", typeof(SchedulerControl));
		}
		public static bool BindToSchedulerStorage(ControlDesigner designer) {
			return BindToComponent(designer, "Storage", typeof(SchedulerStorage));
		}
		public static PropertyInfo GetSchedulerControlPropertyInfo(ControlDesigner designer) {
			return GetPropertyInfo(designer, "SchedulerControl", typeof(SchedulerControl));
		}
		public static void BindToRangeControl(SchedulerControl scheduler) {
			ISite site = scheduler.Site;
			if (site == null)
				return;
			foreach (IComponent component in site.Container.Components) {
				RangeControl rangeControl = component as RangeControl;
				if (rangeControl == null || rangeControl.Client != null)
					continue;
				rangeControl.Client = scheduler;
				break;
			}
		}
	}
	#endregion
	#region SchedulerControlActionList
	public class SchedulerControlActionList : DesignerActionList {
		public SchedulerControlActionList(SchedulerControlDesigner designer)
			: base(designer.Component) {
		}
		#region Properties
		SchedulerControl SchedulerControl { get { return (SchedulerControl)Component; } }
		public IDXMenuManager MenuManager {
			get { return SchedulerControl.MenuManager; }
			set {
				EditorContextHelper.SetPropertyValue(Component.Site, Component, "MenuManager", value);
			}
		}
		public ToolTipController ToolTipController {
			get { return SchedulerControl.ToolTipController; }
			set {
				EditorContextHelper.SetPropertyValue(Component.Site, Component, "ToolTipController", value);
			}
		}
		#endregion
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionPropertyItem("MenuManager", "Menu Manager"));
			res.Add(new DesignerActionPropertyItem("ToolTipController", "Tooltip Controller"));
			return res;
		}
	}
	#endregion
	#region SchedulerControlEnableDisableViewsActionList
	public class SchedulerControlEnableDisableViewsActionList : DesignerActionList {
		public SchedulerControlEnableDisableViewsActionList(SchedulerControlDesigner designer)
			: base(designer.Component) {
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionPropertyItem("DayViewEnabled", "Enable DayView"));
			res.Add(new DesignerActionPropertyItem("WorkWeekViewEnabled", "Enable WorkWeekView"));
			res.Add(new DesignerActionPropertyItem("WeekViewEnabled", "Enable WeekView"));
			res.Add(new DesignerActionPropertyItem("FullWeekViewEnabled", "Enable FullWeekView"));
			res.Add(new DesignerActionPropertyItem("MonthViewEnabled", "Enable MonthView"));
			res.Add(new DesignerActionPropertyItem("TimelineViewEnabled", "Enable TimelineView"));
			res.Add(new DesignerActionPropertyItem("GanttViewEnabled", "Enable GanttView"));
			return res;
		}
		SchedulerControl SchedulerControl { get { return (SchedulerControl)Component; } }
		public bool DayViewEnabled {
			get { return SchedulerControl.DayView.Enabled; }
			set {
				using (ComponentChangeHelper helper = new ComponentChangeHelper(SchedulerControl, "Setting DayView.Enabled")) {
					SchedulerControl.DayView.Enabled = value;
				}
			}
		}
		public bool WorkWeekViewEnabled {
			get { return SchedulerControl.WorkWeekView.Enabled; }
			set {
				using (ComponentChangeHelper helper = new ComponentChangeHelper(SchedulerControl, "Setting WorkWeekView.Enabled")) {
					SchedulerControl.WorkWeekView.Enabled = value;
				}
			}
		}
		public bool WeekViewEnabled {
			get { return SchedulerControl.WeekView.Enabled; }
			set {
				using (ComponentChangeHelper helper = new ComponentChangeHelper(SchedulerControl, "Setting WeekView.Enabled")) {
					SchedulerControl.WeekView.Enabled = value;
				}
			}
		}
		public bool FullWeekViewEnabled {
			get { return SchedulerControl.FullWeekView.Enabled; }
			set {
				using (ComponentChangeHelper helper = new ComponentChangeHelper(SchedulerControl, "Settin FullWeekView.Eanbled")) {
					SchedulerControl.FullWeekView.Enabled = value;
				}
			}
		}
		public bool MonthViewEnabled {
			get { return SchedulerControl.MonthView.Enabled; }
			set {
				using (ComponentChangeHelper helper = new ComponentChangeHelper(SchedulerControl, "Setting MonthView.Enabled")) {
					SchedulerControl.MonthView.Enabled = value;
				}
			}
		}
		public bool TimelineViewEnabled {
			get { return SchedulerControl.TimelineView.Enabled; }
			set {
				using (ComponentChangeHelper helper = new ComponentChangeHelper(SchedulerControl, "Setting TimelineView.Enabled")) {
					SchedulerControl.TimelineView.Enabled = value;
				}
			}
		}
		public bool GanttViewEnabled {
			get { return SchedulerControl.GanttView.Enabled; }
			set {
				using (ComponentChangeHelper helper = new ComponentChangeHelper(SchedulerControl, "Setting GanttView.Enabled")) {
					SchedulerControl.GanttView.Enabled = value;
				}
			}
		}
	}
	#endregion
	#region SchedulerControlCreateBarContainersActionList
	public class SchedulerControlCustomizeCodeActionList : DesignerActionList {
		readonly SchedulerControlDesigner designer;
		public SchedulerControlCustomizeCodeActionList(SchedulerControlDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		#region Properties
		SchedulerControlDesigner Designer { get { return designer; } }
		SchedulerControl SchedulerControl { get { return (SchedulerControl)Component; } }
		#endregion
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			if (!SchedulerDesignTimeBarsGenerator.IsExistBarContainer(Component))
				PopulateCreateBarsItems(res);
			res.Add(new DesignerActionMethodItem(this, "CustomAppointmentFormWizard", CustomAppointmentFormGenerator.SRCustomAppointmentFormWizardDescription));
			return res;
		}
		protected internal virtual void PopulateCreateBarsItems(DesignerActionItemCollection items) {
			items.Add(new DesignerActionMethodItem(this, "CreateRibbon", "Create Ribbon"));
			items.Add(new DesignerActionMethodItem(this, "CreateBarManager", "Create BarManager"));
		}
		[RefreshProperties(RefreshProperties.All)]
		public void CustomAppointmentFormWizard() {
			if (SchedulerControl == null)
				return;
			CustomAppointmentFormGenerator generator = new CustomAppointmentFormGenerator(Component, Designer.SchedulerControl);
			generator.Execute();
		}
		[RefreshProperties(RefreshProperties.All)]
		public void CreateRibbon() {
			if (SchedulerControl == null)
				return;
			IContainer container = Designer.DesignerHost.Container;
			if (container == null)
				return;
			Control form = DevExpress.XtraBars.Design.DesignHelpers.GetContainerControl(container);
			if (form == null)
				return;
			using (DesignerTransaction transaction = Designer.DesignerHost.CreateTransaction("Create Ribbon")) {
				RibbonControl ribbon = (RibbonControl)Designer.DesignerHost.CreateComponent(typeof(RibbonControl));
				Designer.ChangeServiceProtected.OnComponentChanging(form, null);
				form.Controls.Add(ribbon);
				RibbonForm ribbonForm = form as RibbonForm;
				if (ribbonForm != null)
					ribbonForm.Ribbon = ribbon;
				Designer.ChangeServiceProtected.OnComponentChanging(form, null);
				Designer.ChangeServiceProtected.OnComponentChanging(SchedulerControl, null);
				SchedulerControl.MenuManager = ribbon;
				Designer.ChangeServiceProtected.OnComponentChanged(SchedulerControl, null, null, null);
				transaction.Commit();
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
		[RefreshProperties(RefreshProperties.All)]
		public void CreateBarManager() {
			if (SchedulerControl == null)
				return;
			IContainer container = Designer.DesignerHost.Container;
			if (container == null)
				return;
			Control form = DevExpress.XtraBars.Design.DesignHelpers.GetContainerControl(container);
			if (form == null)
				return;
			using (DesignerTransaction transaction = Designer.DesignerHost.CreateTransaction("Create BarManager")) {
				Designer.ChangeServiceProtected.OnComponentChanging(form, null);
				Designer.ChangeServiceProtected.OnComponentChanged(form, null, null, null);
				BarManager barManager = (BarManager)Designer.DesignerHost.CreateComponent(typeof(BarManager));
				Designer.ChangeServiceProtected.OnComponentChanging(container, null);
				container.Add(barManager);
				Designer.ChangeServiceProtected.OnComponentChanged(container, null, null, null);
				Designer.ChangeServiceProtected.OnComponentChanging(barManager, null);
				barManager.Form = form;
				Designer.ChangeServiceProtected.OnComponentChanged(barManager, null, null, null);
				Designer.ChangeServiceProtected.OnComponentChanging(SchedulerControl, null);
				SchedulerControl.MenuManager = barManager;
				Designer.ChangeServiceProtected.OnComponentChanged(SchedulerControl, null, null, null);
				transaction.Commit();
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
	}
	#endregion
	#region SchedulerControlBarsActionList
	public class SchedulerControlBarsActionList : DesignerActionList {
		readonly SchedulerControlDesigner designer;
		public SchedulerControlBarsActionList(SchedulerControlDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		public SchedulerControlDesigner Designer { get { return designer; } }
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection result = new DesignerActionItemCollection();
			if (!SchedulerDesignTimeBarsGenerator.IsExistBarContainer(Component))
				return result;
			PopulateSortedActionItems(result);
			return result;
		}
		protected internal virtual void PopulateSortedActionItems(DesignerActionItemCollection items) {
			items.Add(new DesignerActionMethodItem(this, "CreateFileBars", " Create File Bars"));
			items.Add(new DesignerActionMethodItem(this, "CreateHomeBars", " Create Home Bars"));
			items.Add(new DesignerActionMethodItem(this, "CreateViewBars", " Create View Bars"));
			items.Add(new DesignerActionMethodItem(this, "CreateAppointmentBars", " Create Appointment Bars"));
		}
		[RefreshProperties(RefreshProperties.All)]
		public void CreateFileBars() {
			AddNewBars(GetFileBarsCreators(), "File", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetFileBarsCreators() {
			ControlCommandBarCreator common = new SchedulerCommonBarCreator();
			ControlCommandBarCreator print = new SchedulerPrintBarCreator();
			return new ControlCommandBarCreator[] { common, print };
		}
		[RefreshProperties(RefreshProperties.All)]
		public void CreateHomeBars() {
			AddNewBars(GetHomeBarsCreators(), "Home", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetHomeBarsCreators() {
			ControlCommandBarCreator appointment = new SchedulerAppointmentBarCreator();
			ControlCommandBarCreator navigator = new SchedulerNavigatorBarCreator();
			ControlCommandBarCreator arrange = new SchedulerArrangeBarCreator();
			ControlCommandBarCreator groupBy = new SchedulerGroupByBarCreator();
			return new ControlCommandBarCreator[] { appointment, navigator, arrange, groupBy };
		}
		[RefreshProperties(RefreshProperties.All)]
		public void CreateViewBars() {
			AddNewBars(GetViewBarsCreators(false), "View", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetViewBarsCreators(bool skipActiveViewBarCreation) {
			List<ControlCommandBarCreator> result = new List<ControlCommandBarCreator>();
			result.Add(new SchedulerActiveViewBarCreator(skipActiveViewBarCreation));
			result.Add(new SchedulerTimeScaleBarCreator());
			result.Add(new SchedulerLayoutBarCreator());
			return result.ToArray();
		}
		[RefreshProperties(RefreshProperties.All)]
		public void CreateAppointmentBars() {
			AddNewBars(GetAppointmentBarsCreators(), "Appointment", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetAppointmentBarsCreators() {
			ControlCommandBarCreator appointment = new SchedulerActionsBarCreator();
			ControlCommandBarCreator options = new SchedulerOptionsBarCreator();
			return new ControlCommandBarCreator[] { appointment, options };
		}
		protected void AddNewBars(ControlCommandBarCreator[] creators, string barName, BarInsertMode insertMode) {
			SchedulerDesignTimeBarsGenerator generator = new SchedulerDesignTimeBarsGenerator(this.Designer.DesignerHost, Designer.Control);
			generator.AddNewBars(creators, barName, insertMode);
		}
		public void ClearExistingItems() {
			ControlCommandBarCreator fakeBarCreator = new SchedulerActionsBarCreator();
			SchedulerDesignTimeBarsGenerator generator = new SchedulerDesignTimeBarsGenerator(this.Designer.DesignerHost, Designer.Control);
			generator.ClearExistingItems(fakeBarCreator);
		}
	}
	#endregion
	#region SchedulerAllBarsActionList
	public class SchedulerControlAllBarsActionList : SchedulerControlBarsActionList {
		public SchedulerControlAllBarsActionList(SchedulerControlDesigner designer)
			: base(designer) {
		}
		protected internal override void PopulateSortedActionItems(DesignerActionItemCollection items) {
			items.Add(new DesignerActionMethodItem(this, "CreateAllBars", " Create All Bars"));
		}
		[RefreshProperties(RefreshProperties.All)]
		public void CreateAllBars() {
			CreateAllBarsCore(false, BarInsertMode.Add);
		}
		public void CreateAllBarsCore(bool clearExistingItemsBefore, BarInsertMode insertMode) {
			using (DesignerTransaction transaction = Designer.DesignerHost.CreateTransaction("Create Scheduler Bars")) {
				if (clearExistingItemsBefore)
					ClearExistingItems();
				List<ControlCommandBarCreator[]> creators = new List<ControlCommandBarCreator[]>();
				creators.Add(GetFileBarsCreators());
				creators.Add(GetHomeBarsCreators());
				creators.Add(GetViewBarsCreators(true));
				creators.Add(GetAppointmentBarsCreators());
				int first, last, step;
				if (insertMode == BarInsertMode.Add) {
					first = 0;
					last = creators.Count;
					step = 1;
				} else {
					first = creators.Count - 1;
					last = -1;
					step = -1;
				}
				for (int i = first; i != last; i += step)
					AddNewBars(creators[i], "Create Scheduler Bars", insertMode);
				transaction.Commit();
			}
		}
	}
	#endregion
	#region SchedulerControlDesigner
	public class SchedulerControlDesigner : XtraSchedulerSuiteControlDesigner {
		DesignTimeFeatureInfo designTimeIndicator;
		public SchedulerControlDesigner() {
		}
		public SchedulerControl SchedulerControl { get { return Component as SchedulerControl; } }
		internal IComponentChangeService ChangeServiceProtected { get { return base.ChangeService; } }
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			SchedulerControl.BeginInit();
			SchedulerControl.EndInit();
			SchedulerControl.DayView.TimeRulers.Add(new TimeRuler());
			SchedulerControl.WorkWeekView.TimeRulers.Add(new TimeRuler());
			SchedulerControl.WeekView.Enabled = false;
			SchedulerControl.FullWeekView.Enabled = true;
			SchedulerControl.FullWeekView.TimeRulers.Add(new TimeRuler());
			SchedulerControl.Refresh();
			AssignStorage();
			SchedulerAutomaticBindHelper.BindToRangeControl(SchedulerControl);
			try {
				AddSchedulerNamespaceImport();
			} catch {
			}
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new SchedulerControlDBCreationActionList(this));
			list.Add(new SchedulerControlAllBarsActionList(this));
			list.Add(new SchedulerControlBarsActionList(this));
			base.RegisterActionLists(list);
			list.Add(new SchedulerControlAppointmentsActionList(this.Component));
			list.Add(new SchedulerControlResourcesActionList(this.Component));
			list.Add(new SchedulerControlAppointmentDependenciesActionList(this.Component));
			list.Add(new SchedulerControlActionList(this));
			list.Add(new SchedulerControlEnableDisableViewsActionList(this));
			list.Add(new SchedulerControlCustomizeCodeActionList(this));
		}
		void AssignStorage() {
			if (SchedulerControl.DataStorage == null) {
				SchedulerStorage storage = new SchedulerStorage();
				DesignerHost.Container.Add(storage);
				SchedulerControl.DataStorage = storage;
			}
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			if (ChangeService != null) {
				ChangeService.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
			}
			designTimeIndicator = new DesignTimeFeatureInfo(SchedulerControl);
			SchedulerControl.Paint += new PaintEventHandler(OnControlPaint);
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (ChangeService != null) {
						ChangeService.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
					}
					SchedulerControl.Paint -= new PaintEventHandler(OnControlPaint);
					designTimeIndicator.Dispose();
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		protected override void OnComponentAdded(object sender, ComponentEventArgs e) {
			SchedulerAutomaticBindHelper.BindToSchedulerStorage(this);
			SchedulerAutomaticBindHelper.BindToComponent(this, "MenuManager", typeof(IDXMenuManager));
			SchedulerAutomaticBindHelper.BindToComponent(this, "ToolTipController", typeof(ToolTipController));
		}
		void AddSchedulerNamespaceImport() {
			string schedulerNamespace = "DevExpress.XtraScheduler";
			ProjectItem projectItem = (ProjectItem)Component.Site.GetService(typeof(ProjectItem));
			FileCodeModel2 codeModel = (FileCodeModel2)projectItem.FileCodeModel;
			foreach (CodeElement codeElement in codeModel.CodeElements) {
				CodeImport codeImport = codeElement as CodeImport;
				if (codeImport != null && codeImport.Namespace == schedulerNamespace)
					return;
			}
			codeModel.AddImport(schedulerNamespace, -1);
		}
		void OnComponentChanged(object sender, ComponentChangedEventArgs e) {
			if (SchedulerControl.ShowFeaturesIndicator) {
				if (e.Component == SchedulerControl || e.Component == SchedulerControl.DataStorage ||
					e.Component is MappingInfoBase<Appointment> ||
					e.Component is MappingInfoBase<Resource>) {
					SchedulerControl.Invalidate();
					SchedulerControl.Update();
				}
			}
		}
		protected override void OnComponentRemoved(object sender, ComponentEventArgs e) {
			ISchedulerStorageBase storage = SchedulerControl.DataStorage;
			if (storage == null)
				return;
			IList schedulerData = DevExpress.XtraScheduler.Native.DataHelper.GetList(storage.Appointments.DataSource, storage.Appointments.DataMember);
			IList componentData = GetListFromRemoved(e.Component);
			if (Comparer.Equals(schedulerData, componentData)) {
				SchedulerControl.DataStorage.Appointments.DataSource = null;
				SchedulerControl.Invalidate();
				SchedulerControl.Update();
			}
			SchedulerAutomaticBindHelper.UnbindFromRemovedComponent(this, e.Component, "MenuManager", typeof(IDXMenuManager));
			SchedulerAutomaticBindHelper.UnbindFromRemovedComponent(this, e.Component, "ToolTipController", typeof(ToolTipController));
		}
		IList GetListFromRemoved(IComponent c) {
			IList list = DevExpress.XtraScheduler.Native.DataHelper.GetList(c, SchedulerControl.DataStorage.Appointments.DataMember);
			if (list == null)
				list = DevExpress.XtraScheduler.Native.DataHelper.GetList(c, null);
			return list;
		}
		void OnControlPaint(object sender, PaintEventArgs e) {
			if (SchedulerControl.ShowFeaturesIndicator) {
				GraphicsCache cache = new GraphicsCache(e);
				designTimeIndicator.PrepareInfo();
				designTimeIndicator.CalcLayout(cache);
				designTimeIndicator.Draw(cache);
				cache.Dispose();
			}
		}
		public void AddAllBars(bool clearExistingItemsBefore, BarInsertMode insertMode) {
			if (SchedulerDesignTimeBarsGenerator.IsExistBarContainer(Component)) {
				SchedulerControlAllBarsActionList actionList = new SchedulerControlAllBarsActionList(this);
				actionList.CreateAllBarsCore(clearExistingItemsBefore, insertMode);
			}
		}
	}
	#endregion
	#region XPOApplyChangesHandlerCodeGenerator
	public class XPOApplyChangesHandlerCodeGenerator : IEventHandlerCodeGenerator {
		ISchedulerStorageBase storage;
		public XPOApplyChangesHandlerCodeGenerator(ISchedulerStorageBase storage, IDesignerHost host) {
			if (storage == null)
				Exceptions.ThrowArgumentNullException("storage");
			if (host == null)
				return;
			this.storage = storage;
		}
		#region IEventHandlerCodeGenerator Members
		public void GenerateCode(CodeGenerationTarget handlerCode) {
			CodeMemberField storageField = LookupStorageField(handlerCode.Type);
			if (storageField == null)
				return;
			if (handlerCode.Method.Parameters.Count != 2)
				return;
			const string objectIndexVariableName = "objectIndex";
			const string countVariableName = "count";
			const string aptVariableName = "apt";
			const string xpObjectVariableName = "xpObj";
			CodeVariableDeclarationStatement countVariableDeclaration = new CodeVariableDeclarationStatement(typeof(int), countVariableName, new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression(handlerCode.Method.Parameters[1].Name), "Objects"), "Count"));
			CodeVariableDeclarationStatement initLoopStatement = new CodeVariableDeclarationStatement(typeof(int), objectIndexVariableName, new CodePrimitiveExpression(0));
			CodeBinaryOperatorExpression loopExpression = new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(objectIndexVariableName), CodeBinaryOperatorType.LessThan, new CodeVariableReferenceExpression(countVariableName));
			CodeStatement incrementStatement = new CodeAssignStatement(new CodeVariableReferenceExpression(objectIndexVariableName), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(objectIndexVariableName), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1)));
			CodeIterationStatement statementLoop = new CodeIterationStatement(initLoopStatement, loopExpression, incrementStatement);
			handlerCode.MethodStatements.Add(countVariableDeclaration);
			handlerCode.MethodStatements.Add(statementLoop);
			CodeExpression objectIndexedExpression = new CodeCastExpression(typeof(Appointment), new CodeIndexerExpression(new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression(handlerCode.Method.Parameters[1].Name), "Objects"), new CodeVariableReferenceExpression(objectIndexVariableName)));
			CodeVariableDeclarationStatement aptDeclaration = new CodeVariableDeclarationStatement(typeof(Appointment), aptVariableName, objectIndexedExpression);
			statementLoop.Statements.Add(aptDeclaration);
			CodeExpression xpObjectAccessExpression = new CodeCastExpression(typeof(XPBaseObject), new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(aptVariableName), "GetSourceObject", new CodeExpression[] { new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), storageField.Name) }));
			CodeVariableDeclarationStatement xpObjectDeclaration = new CodeVariableDeclarationStatement(typeof(XPBaseObject), xpObjectVariableName, xpObjectAccessExpression);
			statementLoop.Statements.Add(xpObjectDeclaration);
			CodeExpressionStatement saveStatement = new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(xpObjectVariableName), "Save", new CodeExpression[] { }));
			statementLoop.Statements.Add(saveStatement);
		}
		#endregion
		protected internal virtual CodeMemberField LookupStorageField(CodeTypeDeclaration type) {
			foreach (CodeTypeMember member in type.Members) {
				CodeMemberField field = member as CodeMemberField;
				if (field != null && field.Type.BaseType == storage.GetType().FullName) {
					return field;
				}
			}
			return null;
		}
	}
	#endregion
	public class CustomAppointmentFormHandlerCodeGenerator : IEventHandlerCodeGenerator {
		const string SRSchedulerControlTypeName = "DevExpress.XtraScheduler.SchedulerControl";
		IDesignerHost host;
		string customAppointmentFormTypeFullName;
		SchedulerControl scheduler;
		public CustomAppointmentFormHandlerCodeGenerator(IDesignerHost host, string customAppointmentFormTypeFullName, SchedulerControl scheduler) {
			Guard.ArgumentNotNull(host, "host");
			Guard.ArgumentIsNotNullOrEmpty(customAppointmentFormTypeFullName, "customAppointmentFormTypeFullName");
			Guard.ArgumentNotNull(scheduler, "scheduler");
			this.host = host;
			this.customAppointmentFormTypeFullName = customAppointmentFormTypeFullName;
			this.scheduler = scheduler;
		}
		public IDesignerHost Host { get { return host; } }
		public string CustomAppointmentFormTypeFullName { get { return customAppointmentFormTypeFullName; } }
		public SchedulerControl Scheduler { get { return scheduler; } }
		#region IEventHandlerCodeGenerator Members
		public void GenerateCode(CodeGenerationTarget handlerCode) {
			handlerCode.MethodStatements.AddRange(ShowAppointmentFormStatements(handlerCode.Method, handlerCode.Type));
		}
		CodeStatementCollection ShowAppointmentFormStatements(CodeMemberMethod codeMemberMethod, CodeTypeDeclaration type) {
			CodeStatementCollection result = new CodeStatementCollection();
			CodeParameterDeclarationExpression eParameter = FindParameterByName(codeMemberMethod.Parameters, typeof(AppointmentFormEventArgs).FullName);
			CodeParameterDeclarationExpression senderParameter = FindParameterByName(codeMemberMethod.Parameters, typeof(object).FullName);
			if (eParameter == null || senderParameter == null)
				return result;
			CodeArgumentReferenceExpression codeSenderArgumentReferenceException = new CodeArgumentReferenceExpression(senderParameter.Name);
			CodeArgumentReferenceExpression codeEArgumentReferenceException = new CodeArgumentReferenceExpression(eParameter.Name);
			CodeCastExpression senderCastToSchedulerControlExpression = new CodeCastExpression(SRSchedulerControlTypeName, codeSenderArgumentReferenceException);
			result.Add(new CodeVariableDeclarationStatement(SRSchedulerControlTypeName, "scheduler", senderCastToSchedulerControlExpression));
			CodeVariableReferenceExpression schedulerReferenceExpression = new CodeVariableReferenceExpression("scheduler");
			CodePropertyReferenceExpression appointmentReferenceExpression = new CodePropertyReferenceExpression(codeEArgumentReferenceException, "Appointment");
			CodePropertyReferenceExpression openRecurrenceFormReferenceExpression = new CodePropertyReferenceExpression(codeEArgumentReferenceException, "OpenRecurrenceForm");
			CodePropertyReferenceExpression dialogResultReferenceExpression = new CodePropertyReferenceExpression(codeEArgumentReferenceException, "DialogResult");
			CodePropertyReferenceExpression handledReferenceExpression = new CodePropertyReferenceExpression(codeEArgumentReferenceException, "Handled");
			CodeObjectCreateExpression customAppointmentFormCreationExpression = new CodeObjectCreateExpression(CustomAppointmentFormTypeFullName, schedulerReferenceExpression, appointmentReferenceExpression, openRecurrenceFormReferenceExpression);
			CodeVariableDeclarationStatement formDeclarationStatement = new CodeVariableDeclarationStatement(CustomAppointmentFormTypeFullName, "form", customAppointmentFormCreationExpression);
			result.Add(formDeclarationStatement);
			CodeVariableReferenceExpression formReferenceExpression = new CodeVariableReferenceExpression("form");
			CodeMethodInvokeExpression showDialogInvokeExpression = new CodeMethodInvokeExpression(formReferenceExpression, "ShowDialog");
			CodeTryCatchFinallyStatement tryFinalyStatement = new CodeTryCatchFinallyStatement();
			result.Add(tryFinalyStatement);
			CodeAssignStatement dialogResultAssignStatement = new CodeAssignStatement(dialogResultReferenceExpression, showDialogInvokeExpression);
			tryFinalyStatement.TryStatements.Add(dialogResultAssignStatement);
			CodeAssignStatement handledAssignStatement = new CodeAssignStatement(handledReferenceExpression, new CodeSnippetExpression("true"));
			tryFinalyStatement.TryStatements.Add(handledAssignStatement);
			tryFinalyStatement.FinallyStatements.Add(new CodeMethodInvokeExpression(formReferenceExpression, "Dispose"));
			return result;
		}
		#endregion
		CodeParameterDeclarationExpression FindParameterByName(CodeParameterDeclarationExpressionCollection codeParameterDeclarationExpressionCollection, string name) {
			foreach (CodeParameterDeclarationExpression parameter in codeParameterDeclarationExpressionCollection) {
				if (parameter.Type.BaseType == name)
					return parameter;
			}
			return null;
		}
	}
	#region TableAdapterApplyChangesHandlerCodeGenerator
	public class TableAdapterApplyChangesHandlerCodeGenerator : IEventHandlerCodeGenerator {
		IDesignerHost host;
		ISchedulerStorageBase storage;
		public TableAdapterApplyChangesHandlerCodeGenerator(ISchedulerStorageBase storage, IDesignerHost host) {
			if (storage == null)
				Exceptions.ThrowArgumentNullException("storage");
			if (host == null)
				return;
			this.storage = storage;
			this.host = host;
		}
		#region IEventHandlerCodeGenerator Members
		public void GenerateCode(CodeGenerationTarget handlerCode) {
			BindingSource bindingSource = storage.Appointments.DataSource as BindingSource;
			if (bindingSource == null)
				return;
			DataSet dataSet = bindingSource.DataSource as DataSet;
			if (dataSet == null)
				return;
			CodeMemberField dataSetField = LookupDataSetField(handlerCode.Type, dataSet.GetType());
			if (dataSetField == null)
				return;
			CodeMemberField appointmentsDataAdapterField = LookupTableAdapterField(handlerCode.Type, bindingSource.DataMember);
			if (appointmentsDataAdapterField == null)
				return;
			handlerCode.MethodStatements.AddRange(GenerateDataSetUpdateStatement(dataSetField, appointmentsDataAdapterField));
			handlerCode.MethodStatements.AddRange(GenerateAcceptChangesStatement(dataSetField));
		}
		#endregion
		protected internal virtual CodeStatementCollection GenerateDataSetUpdateStatement(CodeMemberField dataSetField, CodeMemberField appointmentsDataAdapterField) {
			CodeThisReferenceExpression refThis = new CodeThisReferenceExpression();
			CodeFieldReferenceExpression refDataSet = new CodeFieldReferenceExpression(refThis, dataSetField.Name);
			CodeFieldReferenceExpression refTableAdapter = new CodeFieldReferenceExpression(refThis, appointmentsDataAdapterField.Name);
			CodeMethodInvokeExpression refUpdate = new CodeMethodInvokeExpression(refTableAdapter, "Update", new CodeExpression[] { refDataSet });
			CodeExpressionStatement statUpdate = new CodeExpressionStatement(refUpdate);
			CodeCommentStatement comment = new CodeCommentStatement(String.Format("Update the '{0}' content", dataSetField.Name));
			CodeStatementCollection result = new CodeStatementCollection();
			result.Add(comment);
			result.Add(statUpdate);
			return result;
		}
		protected internal virtual CodeStatementCollection GenerateAcceptChangesStatement(CodeMemberField dataSetField) {
			CodeThisReferenceExpression refThis = new CodeThisReferenceExpression();
			CodeFieldReferenceExpression refDataSet = new CodeFieldReferenceExpression(refThis, dataSetField.Name);
			CodeMethodInvokeExpression refAcceptChanges = new CodeMethodInvokeExpression(refDataSet, "AcceptChanges", new CodeExpression[] { });
			CodeExpressionStatement statAcceptChanges = new CodeExpressionStatement(refAcceptChanges);
			CodeCommentStatement comment = new CodeCommentStatement(String.Format("Persist '{0}' changes", dataSetField.Name));
			CodeStatementCollection result = new CodeStatementCollection();
			result.Add(comment);
			result.Add(statAcceptChanges);
			return result;
		}
		protected internal virtual CodeMemberField LookupDataSetField(CodeTypeDeclaration type, Type dataSetType) {
			foreach (CodeTypeMember member in type.Members) {
				CodeMemberField field = member as CodeMemberField;
				if (field != null && field.Type.BaseType == dataSetType.FullName)
					return field;
			}
			return null;
		}
		protected internal virtual CodeMemberField LookupTableAdapterField(CodeTypeDeclaration type, string dataMember) {
			ITypeResolutionService typeService = (ITypeResolutionService)host.GetService(typeof(ITypeResolutionService));
			if (typeService == null)
				return null;
			foreach (CodeTypeMember member in type.Members) {
				CodeMemberField field = member as CodeMemberField;
				if (field != null) {
					Type dataAdapterType = typeService.GetType(field.Type.BaseType, false);
					if (dataAdapterType != null) {
						FieldInfo fieldInfo = dataAdapterType.GetField("_adapter", BindingFlags.Instance | BindingFlags.NonPublic);
						if (fieldInfo != null && typeof(DataAdapter).IsAssignableFrom(fieldInfo.FieldType)) {
							if (field.Name.ToLower() == dataMember.ToLower() + "tableadapter")
								return field;
						}
					}
				}
			}
			return null;
		}
	}
	#endregion
	#region BindSchedulerAndStorageActionList
	public class BindSchedulerAndStorageActionList : DesignerActionList {
		ControlDesigner designer;
		public BindSchedulerAndStorageActionList(ControlDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			if (SchedulerAutomaticBindHelper.GetPropertyInfo(designer, "Storage", typeof(SchedulerStorage)) != null)
				res.Add(new DesignerActionPropertyItem("Storage", "Scheduler Storage"));
			if (SchedulerAutomaticBindHelper.GetPropertyInfo(designer, "SchedulerControl", typeof(SchedulerControl)) != null)
				res.Add(new DesignerActionPropertyItem("SchedulerControl", "Scheduler Control"));
			return res;
		}
		public SchedulerStorage Storage {
			get {
				PropertyInfo pi = SchedulerAutomaticBindHelper.GetPropertyInfo(designer, "Storage", typeof(SchedulerStorage));
				return pi != null ? pi.GetValue(Component, null) as SchedulerStorage : null;
			}
			set {
				PropertyInfo pi = SchedulerAutomaticBindHelper.GetPropertyInfo(designer, "Storage", typeof(SchedulerStorage));
				if (pi != null)
					EditorContextHelper.SetPropertyValue(designer, Component, "Storage", value);
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
		public SchedulerControl SchedulerControl {
			get {
				PropertyInfo pi = SchedulerAutomaticBindHelper.GetPropertyInfo(designer, "SchedulerControl", typeof(SchedulerControl));
				return pi != null ? pi.GetValue(Component, null) as SchedulerControl : null;
			}
			set {
				PropertyInfo pi = SchedulerAutomaticBindHelper.GetPropertyInfo(designer, "SchedulerControl", typeof(SchedulerControl));
				if (pi != null)
					EditorContextHelper.SetPropertyValue(designer, Component, "SchedulerControl", value);
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
	}
	#endregion
	[CLSCompliant(false)]
	public class TimeZoneEditDesigner : ButtonEditDesigner {
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			RegisterAboutAction(list);
		}
	}
	#region XtraSchedulerSuiteComboBoxEditDesigner
	[CLSCompliant(false)]
	public class XtraSchedulerSuiteComboBoxEditDesigner : ComboBoxEditDesigner {
		IComponentChangeService changeSvc;
		protected override bool AllowHookDebugMode { get { return true; } }
		public XtraSchedulerSuiteComboBoxEditDesigner() {
			SchedulerRepositoryItemsRegistrator.Register();
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			this.changeSvc = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if (changeSvc != null) {
				changeSvc.ComponentAdded += new ComponentEventHandler(OnComponentAdded);
				changeSvc.ComponentRemoved += new ComponentEventHandler(OnComponentRemove);
			}
		}
		protected override DXAboutActionList GetAboutAction() {
			return new DXAboutActionList(Component, new MethodInvoker(SchedulerControl.About));
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			SchedulerAutomaticBindHelper.BindToSchedulerControl(this);
			SchedulerAutomaticBindHelper.BindToSchedulerStorage(this);
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new BindSchedulerAndStorageActionList(this));
			base.RegisterActionLists(list);
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (changeSvc != null) {
					changeSvc.ComponentRemoved -= new ComponentEventHandler(OnComponentRemove);
					changeSvc.ComponentAdded -= new ComponentEventHandler(OnComponentAdded);
				}
			}
			base.Dispose(disposing);
		}
		void OnComponentRemove(object sender, ComponentEventArgs e) {
			SchedulerAutomaticBindHelper.UnbindFromRemovedComponent(this, e.Component, "SchedulerControl", typeof(SchedulerControl));
			SchedulerAutomaticBindHelper.UnbindFromRemovedComponent(this, e.Component, "Storage", typeof(SchedulerStorage));
		}
		void OnComponentAdded(object sender, ComponentEventArgs e) {
			SchedulerAutomaticBindHelper.BindToSchedulerControl(this);
			SchedulerAutomaticBindHelper.BindToSchedulerStorage(this);
		}
	}
	#endregion
	#region SchedulerDesignTimeBarsGenerator
	public class SchedulerDesignTimeBarsGenerator : DesignTimeBarsGenerator<SchedulerControl, SchedulerCommandId> {
		public SchedulerDesignTimeBarsGenerator(IDesignerHost host, IComponent component)
			: base(host, component) {
		}
		protected override BarGenerationManagerFactory<SchedulerControl, SchedulerCommandId> CreateBarGenerationManagerFactory() {
			return new SchedulerBarGenerationManagerFactory();
		}
		protected override ControlCommandBarControllerBase<SchedulerControl, SchedulerCommandId> CreateBarController() {
			return new SchedulerBarController();
		}
		protected override void EnsureReferences(IDesignerHost designerHost) {
			ReferencesHelper.EnsureReferences(designerHost, AssemblyInfo.SRAssemblyBars);
			ReferencesHelper.EnsureReferences(designerHost, AssemblyInfo.SRAssemblySchedulerExtensions);
		}
	}
	#endregion
	#region SchedulerBarGenerationManager
	public class SchedulerBarGenerationManager : BarGenerationManager<SchedulerControl, SchedulerCommandId> {
		public SchedulerBarGenerationManager(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<SchedulerControl, SchedulerCommandId> barController)
			: base(creator, container, barController) {
		}
	}
	#endregion
	#region SchedulerRibbonGenerationManager
	public class SchedulerRibbonGenerationManager : RibbonGenerationManager<SchedulerControl, SchedulerCommandId> {
		public SchedulerRibbonGenerationManager(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<SchedulerControl, SchedulerCommandId> barController)
			: base(creator, container, barController) {
		}
		protected override SchedulerCommandId EmptyCommandId { get { return SchedulerCommandId.None; } }
	}
	#endregion
	#region SchedulerBarGenerationManagerFactory
	public class SchedulerBarGenerationManagerFactory : BarGenerationManagerFactory<SchedulerControl, SchedulerCommandId> {
		protected override RibbonGenerationManager<SchedulerControl, SchedulerCommandId> CreateRibbonGenerationManagerInstance(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<SchedulerControl, SchedulerCommandId> barController) {
			return new SchedulerRibbonGenerationManager(creator, container, barController);
		}
		protected override BarGenerationManager<SchedulerControl, SchedulerCommandId> CreateBarGenerationManagerInstance(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<SchedulerControl, SchedulerCommandId> barController) {
			return new SchedulerBarGenerationManager(creator, container, barController);
		}
	}
	#endregion
	#region SchedulerBarControllerDesigner
	public class SchedulerBarControllerDesigner : ControlCommandBarControllerDesigner<SchedulerControl, SchedulerCommandId> {
		protected override DXAboutActionList GetAboutAction() {
			return new DXAboutActionList(Component, new MethodInvoker(SchedulerControl.About));
		}
	}
	#endregion
}
