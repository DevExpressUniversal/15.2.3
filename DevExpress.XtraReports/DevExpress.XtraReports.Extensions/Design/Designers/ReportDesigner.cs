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
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data;
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using DevExpress.Data.Design;
using DevExpress.Data.Native;
using DevExpress.DataAccess.UI.Wizard.Clients;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Utils;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design.Adapters;
using DevExpress.XtraReports.Design.Behaviours;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.Design.MouseTargets;
using DevExpress.XtraReports.Extensions;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraReports.Native.Templates;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.UserDesigner.Native;
using DevExpress.XtraReports.Wizards3;
using System.Linq;
using System.ComponentModel.Design.Serialization;
using DevExpress.DataAccess;
using DevExpress.XtraReports.Wizards3.Builder;
using DevExpress.DataAccess.UI.Wizard;
using DataView = DevExpress.DataAccess.Native.Data.DataView;
using DevExpress.DataAccess.UI.Native.Sql;
namespace DevExpress.XtraReports.Design {
	[
	ToolboxItemFilter(AttributeSR.ToolboxItemFilter),
	ToolboxItemFilter("System.Windows.Forms", ToolboxItemFilterType.Prevent),
	ToolboxItemFilter(AttributeSR.SchedulerToolboxItem, ToolboxItemFilterType.Prevent),
	ToolboxItemFilter("System.Drawing.Printing", ToolboxItemFilterType.Prevent),
	]
	public class ReportDesigner : XRComponentDesigner, IRootDesigner, IToolboxUser {
		#region static
		static ActivationService staticActivationService;
		static ReportDesigner() {
			NotificationService.AddService(typeof(XtraReport), new ReportNotificationService());
		}
		static void ValidateDataAdapter(IDataContainer dataContainer, IComponent component) {
			if(Comparer.Equals(dataContainer.DataAdapter, component))
				dataContainer.DataAdapter = null;
		}
		static void CorrectDefaultAttributes(IDictionary properties) {
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(XtraReport));
			foreach(PropertyDescriptor property in props) {
				if(!properties.Contains(property.Name))
					continue;
				DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)property.Attributes[typeof(DefaultValueAttribute)];
				if(defaultValueAttribute == null)
					continue;
				properties[property.Name] = XtraReports.Native.XRAccessor.CreateProperty(typeof(XtraReport),
					(PropertyDescriptor)properties[property.Name], new Attribute[] { new DefaultValueAttribute(defaultValueAttribute.Value) });
			}
		}
		static string GetWarningText(ReportStringId id, string[] controlsNames) {
			string s = ReportLocalizer.GetString(id);
			if(controlsNames.Length == 0 || String.IsNullOrEmpty(s))
				return String.Empty;
			return String.Format(s, "\n" + GetWarningText(controlsNames));
		}
		static string GetWarningText(string[] controlsNames) {
			if(controlsNames.Length == 0)
				return "";
			StringBuilder toolTipTextBuilder = new StringBuilder(controlsNames[0]);
			for(int i = 1; i < controlsNames.Length; i++) {
				toolTipTextBuilder.Append(", ");
				if((i % 5) == 0)
					toolTipTextBuilder.Append("\n");
				toolTipTextBuilder.Append(controlsNames[i]);
			}
			return toolTipTextBuilder.ToString();
		}
		public static void SetDataMember(IDataContainer dataContainer, object dataSource) {
			if(String.IsNullOrEmpty(dataContainer.DataMember) && dataSource != null && !(dataSource is IList))
				dataContainer.DataMember = GetListName(dataSource);
		}
		static string GetListName(object dataSource) {
			using(DataContext dataContext = new DataContext(true)) {
				PropertyDescriptorCollection props = dataContext.GetListItemProperties(dataSource, string.Empty);
				props.Sort();
				foreach(PropertyDescriptor prop in props)
					if(prop.PropertyType != typeof(byte[]) && ListTypeHelper.IsListType(prop.PropertyType))
						return prop.Name;
			}
			return string.Empty;
		}
		protected static void DisposeObject(IDisposable obj) {
			if(obj != null)
				obj.Dispose();
		}
		internal static bool HostIsLoading(IServiceProvider servProvider) {
			IDesignerHost host = servProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
			return host != null ? host.Loading : false;
		}
		public static Rectangle GetScreenBounds(Control control) {
			return (control != null) ? control.RectangleToScreen(control.ClientRectangle) : Rectangle.Empty;
		}
		public static IList GetDataAdapters(IDesignerHost designerHost) {
			ArrayList adapters = new ArrayList();
			foreach(IComponent comp in designerHost.Container.Components)
				if(DevExpress.Data.Native.BindingHelper.IsDataAdapter(comp))
					adapters.Add(comp);
			return adapters;
		}
		static PropertyDescriptor[] CollectCollectionProperties(IDictionary properties) {
			List<PropertyDescriptor> result = new List<PropertyDescriptor>();
			foreach(DictionaryEntry entry in properties) {
				if(typeof(ICollection).IsAssignableFrom(((PropertyDescriptor)entry.Value).PropertyType))
					result.Add((PropertyDescriptor)entry.Value);
			}
			return result.ToArray();
		}
		static IComponent GetSelectedComponent(IDesignerHost designerHost) {
			ISelectionService selectionService = designerHost.GetService(typeof(ISelectionService)) as ISelectionService;
			IList components = new ArrayList(selectionService.GetSelectedComponents());
			return (components.Count == 1) ? components[0] as IComponent : null;
		}
		public static XtraReportBase GetSelectedReport(IDesignerHost designerHost) {
			XRControl selectedComponent = GetSelectedComponent(designerHost) as XRControl;
			if(selectedComponent is XtraReportBase)
				return (XtraReportBase)selectedComponent;
			return selectedComponent == null || selectedComponent.Report == null ?
				designerHost.RootComponent as XtraReportBase : selectedComponent.Report;
		}
		public static XRComponentDesigner GetSelectedReportDesigner(IDesignerHost designerHost) {
			XtraReportBase report = GetSelectedReport(designerHost);
			return report == null ? null : designerHost.GetDesigner(report) as XRComponentDesigner;
		}
		#endregion
		#region inner classes
		class ParameterService : IParameterService {
			class ParameterStub : Parameter {
				readonly INameCreationService nameCreationService;
				public ParameterStub(INameCreationService nameCreationService) {
					this.nameCreationService = nameCreationService;
				}
				[Browsable(false)]
				public override LookUpSettings LookUpSettings { get; set; }
				[Browsable(true)]
				public new string Name {
					get {
						return base.Name;
					}
					set {
						if(nameCreationService != null && !nameCreationService.IsValidName(value))
							throw new InvalidOperationException();
						base.Name = value;
					}
				}
			}
			readonly XtraReport report;
			public ParameterService(XtraReport report) {
				this.report = report;
			}
			#region IParameterOwner Members
			public IParameter CreateParameter(Type type) {
				INameCreationService nameCreationService = report.Site.GetService<INameCreationService>();
				ParameterStub parameter = new ParameterStub(nameCreationService) { Type = type };
				IDesignerHost host = report.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
				if(host != null) {
					parameter.Name = nameCreationService.CreateName(host.Container, typeof(Parameter));
				}
				return parameter;
			}
			public void AddParameter(IParameter parameter) {
				var configuredParameter = parameter as Parameter;
				var realParameter = new Parameter() {
					Name = configuredParameter.Name,
					Description = configuredParameter.Description,
					Type = parameter.Type,
					Value = parameter.Value
				};
				report.Parameters.Add(realParameter);
				IDesignerHost host = report.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
				if(host != null)
					DesignToolHelper.AddToContainer(host, realParameter as IComponent, realParameter.Name);
			}
			public bool CanCreateParameters { get { return true; }}
			public IEnumerable<IParameter> Parameters { get { return report.Parameters; } }
			public string AddParameterString { get { return "New Report Parameter..."; } } 
			public string CreateParameterString { get { return "Report Parameter"; } } 
			#endregion
		}
		class DataSourceCollectionProvider : IDataSourceCollectionProvider {
			IDesignerHost fDesignerHost;
			public DataSourceCollectionProvider(IDesignerHost host) {
				this.fDesignerHost = host;
			}
			#region IDataSourceCollectionProvider Members
			public object[] GetDataSourceCollection(IServiceProvider serviceProvider) {
				object[] result = new DataSourceDesignCollector(fDesignerHost).Collect();
				IDataContextService service = fDesignerHost.GetService(typeof(IDataContextService)) as IDataContextService;
				if(service != null && result != null) {
					using(DataContext dataContext = service.CreateDataContext(new DataContextOptions())) {
						Array.Sort(result, (x, y) => {
							if(x is DevExpress.XtraReports.Native.Parameters.ParametersDataSource)
								return 1;
							if(y is DevExpress.XtraReports.Native.Parameters.ParametersDataSource)
								return -1;
							string xName = dataContext.GetDataSourceDisplayName(x, "");
							string yName = dataContext.GetDataSourceDisplayName(y, "");
							return Comparer.Default.Compare(xName, yName);
						});
					}
				}
				return result;
			}
			#endregion
		}
		class DataContainerService : IDataContainerService {
			public bool AreAppropriate(IDataContainerBase dataContainer, object dataSource) {
				if(dataSource is DevExpress.Data.ChartDataSources.IChartDataSource && !(dataContainer is XRChart))
					return false;
				return true;
			}
		}
		protected internal class ServiceHelper : IDisposable {
			readonly IDesignerHost host;
			List<Pair<object, Type>> services = new List<Pair<object, Type>>();
#if DEBUGTEST
			internal List<Pair<object, Type>> Services {
				get { return services; }
			}
#endif
			public ServiceHelper(IDesignerHost host) {
				this.host = host;
				Initialize();
			}
			protected virtual void Initialize() {
				AddService(typeof(IUndoService), new UndoService(host));
				AddService(typeof(IIdleService), new IdleService());
				AddService(typeof(IBoundsAdapterService), new BoundsAdapterService(host));
				AddService(typeof(IDesignerBehaviourService), new DesignerBehaviourService(host));
				AddService(typeof(IMouseTargetService), new MouseTargetService(host));
				AddService(typeof(XRSmartTagService), new XRSmartTagService(host));
				AddService(typeof(CapturePaintService), new CapturePaintService(host));
				AddService(typeof(ResizeService), new ResizeService(host));
				AddService(typeof(MenuCommandHandler), new MenuCommandHandler(host));
				AddService(typeof(IDragDropService), new DragDropService(host));
				AddService(typeof(IFieldListDragDropService), new FieldListDragDropService(host));
				AddService(typeof(IReportExplorerDragDropService), new ReportExplorerDragDropService(host));
				AddService(typeof(ZoomService), new ZoomService(host));
				AddService(typeof(IChartService), new ChartService(host));
				AddService(typeof(ToolTipService), new ToolTipService(host));
				AddService(typeof(NonSerializableComponentsHelperBase), new NonSerializableComponentsHelperDesign());
				AddService(typeof(IDesignTool), new DesignToolService());
				AddService(typeof(IDataContainerService), new DataContainerService());
				AddService(typeof(IDataSourceCollectionProvider), new DataSourceCollectionProvider(host));
				if(host.GetService(typeof(IDictionaryService)) == null)
					AddService(typeof(IDictionaryService), new DevExpress.Serialization.Services.SDictionaryService());
				DisableCodeOptimizedGenerationForCorrectUndoInWebProjects();
			}
			public void AddService(object service) {
				AddService(service.GetType(), service);
			}
			public void AddService(Type serviceType, object service) {
				host.AddService(serviceType, service);
				services.Add(new Pair<object, Type>(service, serviceType));
			}
			public object ReplaceService(Type serviceType, object service) {
				object prevService = host.GetService(serviceType);
				if(prevService != null)
					host.RemoveService(serviceType);
				AddService(serviceType, service);
				return prevService;
			}
			void DisableCodeOptimizedGenerationForCorrectUndoInWebProjects() {
				WindowsFormsDesignerOptionService windowsFormsDesignerOptionService = new WindowsFormsDesignerOptionService();
				windowsFormsDesignerOptionService.CompatibilityOptions.UseOptimizedCodeGeneration = false;
				AddService(typeof(DesignerOptionService), windowsFormsDesignerOptionService);
			}
			#region IDisposable Members
			public virtual void Dispose() {
				foreach(Pair<object, Type> serviceTypePair in services) {
					var service = serviceTypePair.First;
					var serviceType = serviceTypePair.Second;
					var replaceableService = service as IReplaceableService;
					if(replaceableService == null || replaceableService.OriginalServiceType != serviceType)
						host.RemoveService(serviceType);
					IDisposable disposableService = service as IDisposable;
					if(disposableService != null)
						disposableService.Dispose();
				}
				services.Clear();
				services = null;
			}
			#endregion
		}
		protected class XRSelectionHelper {
			#region static
			static object GetNextComponent(XRControl control) {
				if(control == null)
					return null;
				XRControl container = control.Parent;
				if(container != null) {
					foreach(XRControl item in container.Controls)
						if(!control.Equals(item))
							return item;
				}
				return container;
			}
			static object GetNextFormattingComponent(XtraReport report, IComponent component) {
				if(component is XRControlStyle) {
					int currentStyleIndex = report.StyleSheet.IndexOf((XRControlStyle)component);
					if(currentStyleIndex < 0)
						return report;
					if(report.StyleSheet.Count > 1) {
						bool isLastItem = currentStyleIndex == report.StyleSheet.Count - 1;
						return report.StyleSheet[isLastItem ? currentStyleIndex - 1 : currentStyleIndex + 1];
					}
				} else if(component is FormattingRule) {
					int currentRuleIndex = report.FormattingRuleSheet.IndexOf((FormattingRule)component);
					if(currentRuleIndex < 0)
						return report;
					if(report.FormattingRuleSheet.Count > 1) {
						bool isLastItem = currentRuleIndex == report.FormattingRuleSheet.Count - 1;
						return report.FormattingRuleSheet[isLastItem ? currentRuleIndex - 1 : currentRuleIndex + 1];
					}
				}
				return report;
			}
			#endregion
			private object targetSelection;
			IServiceProvider srvProvider;
			public XRSelectionHelper(IServiceProvider srvProvider) {
				this.srvProvider = srvProvider;
			}
			public void RotateSelection() {
				ISelectionService srv = srvProvider.GetService(typeof(ISelectionService)) as ISelectionService;
				if(srv != null && targetSelection != null)
					srv.SetSelectedComponents(new object[] { targetSelection }, SelectionTypes.Replace);
				targetSelection = null;
			}
			public void SetTargetSelection(XtraReport report, IComponent component) {
				if(component is XRControl)
					targetSelection = GetNextComponent((XRControl)component);
				else if(component is XRControlStyle || component is FormattingRule) {
					targetSelection = GetNextFormattingComponent(report, component);
				} else if(!component.IsVisual() && component.IsVisible()) {
					IDesignerHost host = srvProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
					if(host != null) {
						foreach(IComponent item in host.Container.Components) {
							if(!item.IsVisual() && item.IsVisible()) {
								targetSelection = item;
								break;
							}
						};
					}
				}
				RotateSelection();
			}
		}
		#endregion
		#region fields & properties
		InheritanceHelperService inheritanceHelperService;
		protected DevExpress.Data.Utils.IToolShell reportTool;
		private ReportTabControl tabControl;
		private ReportFrame reportFrame;
		private ComponentTray compTray;
		PrintingSystem printingSystem;
		protected ServiceHelper servHelper;
		private DesignerVerbCollection verbs;
		private ISelectionService selectionService;
		private bool trayLayoutSuspended;
		private XRSelectionHelper selectionHelper;
		private object commandSet;
		private IBandViewInfoService bandViewInfoService;
		bool deactivating;
		bool activating;
		ComponentVisibility componentVisibility = ComponentVisibility.ReportExplorer;
		ComponentVisibility stylesNodeVisibility = ComponentVisibility.ReportExplorer;
		ComponentVisibility formattingRulesNodeVisibility = ComponentVisibility.ReportExplorer;
		IExtenderProvider extenderProvider;
		bool bandsValidated;
		ActivationService activationService;
		public override bool CanDragInReportExplorer {
			get { return false; }
		}
		ActivationService ActivationService {
			get {
				if(activationService == null) {
					if(staticActivationService == null)
						staticActivationService = new ActivationService();
					activationService = staticActivationService;
				}
				return activationService;
			}
		}
		public virtual bool IsActive {
			get { return Comparer.Equals(ActivationService.ActiveDesigner, this); }
		}
		public XtraReport RootReport { get { return (XtraReport)Component; } }
		internal XtraReportBase ActiveReport {
			get {
				XRControl selectedComponent = GetSelectedComponent() as XRControl;
				return (selectedComponent is XtraReportBase) ? (XtraReportBase)selectedComponent :
					(selectedComponent == null || selectedComponent.Report == null) ? RootReport :
					selectedComponent.Report;
			}
		}
		public ReportFrame ReportFrame {
			get {
				if(reportFrame == null)
					reportFrame = new ReportFrame(this.Component);
				return reportFrame;
			}
		}
		public override DesignerVerbCollection Verbs {
			get { return verbs; }
		}
		public SizeF ScaledGridSize {
			get {
				SizeF size = XRConvert.Convert(RootReport.GridSizeF, RootReport.Dpi, GraphicsDpi.Pixel);
				return ZoomService.GetInstance(fDesignerHost).ScaleSizeF(size);
			}
		}
		public int RightMargin {
			get {
				return ZoomService.GetInstance(fDesignerHost).ToScaledPixels(RootReport.Margins.Right, RootReport.Dpi);
			}
		}
		public int LeftMargin {
			get { return ZoomService.GetInstance(fDesignerHost).ToScaledPixels(RootReport.Margins.Left, RootReport.Dpi); }
		}
		public int PageWidth {
			get { return ZoomService.GetInstance(fDesignerHost).ToScaledPixels(RootReport.PageWidth, RootReport.Dpi); }
		}
		public bool DrawGrid {
			get { return RootReport.DrawGrid; }
			set { RootReport.DrawGrid = value; }
		}
		public bool DrawWatermark {
			get { return RootReport.DrawWatermark; }
			set { RootReport.DrawWatermark = value; }
		}
		public ComponentVisibility ComponentVisibility {
			get { return componentVisibility; }
			set {
				componentVisibility = value;
				UpdateComponentTrayVisibility();
			}
		}
		public ComponentVisibility StylesNodeVisibility {
			get { return stylesNodeVisibility; }
			set { stylesNodeVisibility = value; }
		}
		public ComponentVisibility FormattingRulesNodeVisibility {
			get { return formattingRulesNodeVisibility; }
			set { formattingRulesNodeVisibility = value; }
		}
		[Obsolete("Use the ComponentVisibility property instead.")]
		public Boolean ShowComponentTray {
			get { return (componentVisibility & ComponentVisibility.ComponentTray) > 0; }
			set {
				ComponentVisibility |= ComponentVisibility.ComponentTray;
			}
		}
		public bool SnapToGrid {
			get { return RootReport.SnapToGrid; }
			set { RootReport.SnapToGrid = value; }
		}
		public int DetailPrintCount {
			get { return RootReport.ReportPrintOptions.DetailCount; }
			set { RootReport.ReportPrintOptions.DetailCount = value; }
		}
		public bool ShowDesignerHints {
			get { return RootReport.DesignerOptions.ShowDesignerHints; }
			set { RootReport.DesignerOptions.ShowDesignerHints = value; }
		}
		public bool ShowExportWarnings {
			get { return RootReport.DesignerOptions.ShowExportWarnings; }
			set { RootReport.DesignerOptions.ShowExportWarnings = value; }
		}
		public bool ShowPrintingWarnings {
			get { return RootReport.DesignerOptions.ShowPrintingWarnings; }
			set { RootReport.DesignerOptions.ShowPrintingWarnings = value; }
		}
		public PrintingSystem PrintingSystem {
			get { return printingSystem; }
		}
		#endregion
		public ReportDesigner() {
			verbs = new DesignerVerbCollection();
			if(GetTemplateProvider() != null)
				verbs.Add(CreateXRDesignerVerb(DesignSR.Verb_LoadReportTemplate, ReportCommand.VerbLoadReportTemplate));
			verbs.Add(CreateXRDesignerVerb(DesignSR.Verb_ReportWizard, ReportCommand.VerbReportWizard));
			verbs.Add(CreateXRDesignerVerb(DesignSR.Verb_EditBands, ReportCommand.VerbEditBands));
			verbs.Add(CreateXRDesignerVerb(DesignSR.Verb_EditBindings, ReportCommand.VerbEditBindings));
			printingSystem = new PrintingSystem();
		}
		protected internal virtual void OnEditBindings() {
			new DevExpress.XtraReports.Design.BindingMapper.BindingMapperHelper().ProcessReport(RootReport);
		}
		protected internal virtual bool OnImport() {
			bool snapToGrid = (bool)XRAccessor.GetProperty(RootReport, DesignPropertyNames.SnapToGrid);
			try {
				DevExpress.XtraReports.Import.XRImporterBase importer = CreateImporter();
				return importer.ConvertTo(RootReport);
			} catch(Exception ex) {
				DevExpress.XtraPrinting.Tracer.TraceError(NativeSR.TraceSource, ex);
				OnImportException(ex);
				return false;
			} finally {
				XRAccessor.SetProperty(RootReport, DesignPropertyNames.SnapToGrid, snapToGrid);
			}
		}
		protected virtual void OnImportException(Exception ex) {
			NotificationService.ShowException<XtraReport>(LookAndFeelProviderHelper.GetLookAndFeel(fDesignerHost), fDesignerHost.GetOwnerWindow(), new Exception("An error occurred during the import.", ex));
		}
		protected virtual DevExpress.XtraReports.Import.XRImporterBase CreateImporter() {
			return new DevExpress.XtraReports.Import.XRImporterBase();
		}
		protected internal virtual bool LoadReportTemplate() {
			using(var templateForm = new TemplateForm() { TemplateProvider = GetTemplateProvider() }) {
				LookAndFeelProviderHelper.SetParentLookAndFeel(templateForm, fDesignerHost);
				try {
					if(DialogRunner.ShowDialog(templateForm) == DialogResult.OK) {
						using(MemoryStream stream = new MemoryStream(templateForm.LayoutStream)) {
							new DevExpress.XtraReports.Import.TemplatesConverter(stream).Convert(RootReport);
						}
						return true;
					}
				} catch {
				}
			}
			return false;
		}
		protected virtual ITemplateProvider GetTemplateProvider() {
			return ReportTemplateExtension.GetTemplateProvider();
		}
		ViewTechnology[] IRootDesigner.SupportedTechnologies {
			get { return new ViewTechnology[] { ControlConstants.ViewTechnologyDefault }; }
		}
		object IRootDesigner.GetView(ViewTechnology technology) {
			if(tabControl == null && technology == ControlConstants.ViewTechnologyDefault) {
				tabControl = CreateReportTabControl();
				servHelper.AddService(typeof(ReportTabControl), tabControl);
				if(IsEUD)
					servHelper.AddService(typeof(IUIService), new UIService(tabControl, fDesignerHost));
				ILookAndFeelService serv = GetService(typeof(ILookAndFeelService)) as ILookAndFeelService;
				serv.LookAndFeel.ParentLookAndFeel = tabControl.LookAndFeel;
				UpdateStatusInfo();
			}
			return tabControl;
		}
		protected virtual ReportTabControl CreateReportTabControl() {
			return new ReportTabControl(this);
		}
		public virtual TypePickerBase CreateReportSourcePicker() {
			return new ReportSourcePicker();
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new ReportBaseDesignerActionList(this));
			list.Add(new ReportBaseDesignerActionList2(this));
			list.Add(new RootReportDesignerActionList1(this));
			list.Add(new RootReportDesignerActionList2(this));
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			CorrectDefaultAttributes(properties);
		}
		protected override void PostFilterProperties(IDictionary properties) {
			base.PostFilterProperties(properties);
			PropertyDescriptor[] props = CollectCollectionProperties(properties);
			foreach(PropertyDescriptor property in props)
				properties[property.Name] = new PropertyDescriptorWrapper((PropertyDescriptor)properties[property.Name]);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				UnsubscribeEvents();
				inheritanceHelperService.Dispose();
				servHelper.Dispose();
				if(commandSet != null) {
					((IDisposable)commandSet).Dispose();
					FieldInfo fi = commandSet.GetType().GetField("commandSet", BindingFlags.NonPublic | BindingFlags.Instance);
					object[] commands = (object[])fi.GetValue(commandSet);
					if(commands != null && commands.Length > 0) {
						FieldInfo fiCommandStatusHash = commands[0].GetType().GetField("commandStatusHash", BindingFlags.NonPublic | BindingFlags.Static);
						Hashtable commandStatusHash = (Hashtable)fiCommandStatusHash.GetValue(commandSet);
						foreach(object item in commands) {
							FieldInfo fiStatusHandler = item.GetType().GetField("statusHandler", BindingFlags.NonPublic | BindingFlags.Instance);
							if(fiStatusHandler == null)
								continue;
							object statusHandler = fiStatusHandler.GetValue(item);
							if(statusHandler != null && commandStatusHash != null && commandStatusHash.Contains(statusHandler))
								commandStatusHash.Remove(statusHandler);
						}
						for(int i = 0; i < commands.Length; i++) {
							commands[i] = null;
						}
						fi.SetValue(commandSet, null);
					}
					fi = commandSet.GetType().GetField("primarySelection", BindingFlags.NonPublic | BindingFlags.Instance);
					fi.SetValue(commandSet, null);
					commandSet = null;
				}
				IExtenderProviderService eps = (IExtenderProviderService)fDesignerHost.GetService(typeof(IExtenderProviderService));
				if(eps != null && extenderProvider != null)
					eps.RemoveExtenderProvider(extenderProvider);
				if(printingSystem != null) {
					printingSystem.Dispose();
					printingSystem = null;
				}
				FinalizeComponentTray();
				if(tabControl != null && !tabControl.IsDisposed) {
					DisposeObject(tabControl);
					tabControl = null;
				}
				if(reportFrame != null && !reportFrame.IsDisposed) {
					reportFrame.Dispose();
					reportFrame = null;
				}
				OnDispose(reportTool);
				if(reportTool != null) {
					reportTool.Dispose();
					reportTool = null;
				}
				if(IsActive)
					ActivationService.ActiveDesigner = null;
			}
			base.Dispose(disposing);
		}
		void OnDispose(DevExpress.Data.Utils.IToolShell toolShell) {
			ActivationService.ReportCount--;
			if(ActivationService.ReportCount == 0) {
				toolShell.Close();
			}
		}
		public override void Initialize(IComponent component) {
			XtraReport report = (XtraReport)component;
			report.PrintingSystem.Extend();
			string version = report.Version;
			report.Version = "0.0";
			base.Initialize(component);
			report.Version = version;
			report.DesignerLoading = true;
			InitializeCore(component);
			DevExpress.XtraPrinting.Tracer.TraceInformationTest(NativeSR.TraceSourceTests, "ReportDesigner.Initialize.Stop");
		}
		protected virtual void InitializeServiceHelper() {
			servHelper = new ServiceHelper(fDesignerHost);
			servHelper.AddService(typeof(ILockService), CreateLockService());
			servHelper.AddService(typeof(IDataContextService), CreateDataContextService());
			ILookAndFeelService lookAndFeelService = CreateLookAndFeelService();
			servHelper.AddService(typeof(ILookAndFeelService), lookAndFeelService);
			servHelper.AddService(typeof(IParameterService), new ParameterService(RootReport));
			servHelper.AddService(typeof(ISqlWizardOptionsProvider), new SqlWizardOptionsProvider(() => SqlWizardOptions.None));
		}
		protected virtual ILookAndFeelService CreateLookAndFeelService() {
			return new LookAndFeelService();
		}
		protected virtual void InitializeCore(IComponent component) {
			new RichTextBox();
			GetServices();
			SubscribeEvents();
			IExtenderProviderService eps = (IExtenderProviderService)fDesignerHost.GetService(typeof(IExtenderProviderService));
			if(eps != null) {
				extenderProvider = CreateExtenderProvider();
				eps.AddExtenderProvider(extenderProvider);
			}
			InitializeServiceHelper();
			if(reportTool == null)
				reportTool = GetReportTool(fDesignerHost);
			CreateComponentTray();
			selectionHelper = new XRSelectionHelper(component.Site);
			if(reportTool != null)
				reportTool.InitToolItems();
			ActivateMenuCommands();
			inheritanceHelperService = GetInheritanceHelperService();
			inheritanceHelperService.AddInheritedComponents(Component, fDesignerHost.Container);
			activationService = (ActivationService)GetService(typeof(ActivationService));
			ActivationService.ReportCount++;
		}
		protected virtual IExtenderProvider CreateExtenderProvider() {
			return new NameExtender();
		}
		protected virtual InheritanceHelperService GetInheritanceHelperService() {
			InheritanceHelperService inheritanceHelperService = fDesignerHost.GetService(typeof(InheritanceHelperService)) as InheritanceHelperService;
			return inheritanceHelperService != null ? inheritanceHelperService : new InheritanceHelperService(fDesignerHost);
		}
		protected virtual LockService CreateLockService() {
			return new EUDLockService();
		}
		protected virtual IDataContextService CreateDataContextService() {
			return new DesignTimeDataContextService(fDesignerHost);
		}
		protected virtual DevExpress.Data.Utils.IToolShell GetReportTool(IServiceProvider srvProvider) {
			return (DevExpress.Data.Utils.IToolShell)fDesignerHost.GetService(typeof(DevExpress.Data.Utils.IToolShell));
		}
		private void GetServices() {
			selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
		}
		private void SubscribeEvents() {
			if(fDesignerHost != null) {
				fDesignerHost.LoadComplete += new EventHandler(OnLoadComplete);
				fDesignerHost.Activated += new EventHandler(OnDesignerActivate);
				fDesignerHost.Deactivated += new EventHandler(OnDesignerDeactivate);
				fDesignerHost.TransactionClosed += new DesignerTransactionCloseEventHandler(OnTransactionClosed);
			}
			if(selectionService != null)
				selectionService.SelectionChanged += new EventHandler(OnSelectionChanged);
			if(changeService != null) {
				changeService.ComponentAdded += new ComponentEventHandler(OnComponentAdded);
				changeService.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
				changeService.ComponentRemoving += new ComponentEventHandler(OnComponentRemoving);
				changeService.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
				changeService.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
			}
		}
		private void UnsubscribeEvents() {
			if(fDesignerHost != null) {
				fDesignerHost.LoadComplete -= new EventHandler(OnLoadComplete);
				fDesignerHost.Activated -= new EventHandler(OnDesignerActivate);
				fDesignerHost.Deactivated -= new EventHandler(OnDesignerDeactivate);
				fDesignerHost.TransactionClosed -= new DesignerTransactionCloseEventHandler(OnTransactionClosed);
			}
			if(selectionService != null)
				selectionService.SelectionChanged -= new EventHandler(OnSelectionChanged);
			if(changeService != null) {
				changeService.ComponentAdded -= new ComponentEventHandler(OnComponentAdded);
				changeService.ComponentRemoved -= new ComponentEventHandler(OnComponentRemoved);
				changeService.ComponentRemoving -= new ComponentEventHandler(OnComponentRemoving);
				changeService.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
				changeService.ComponentRename -= new ComponentRenameEventHandler(OnComponentRename);
			}
		}
		private void CreateComponentTray() {
			if(compTray == null) {
				compTray = new ComponentTrayEx(this, Component.Site);
				compTray.ShowLargeIcons = false;
				compTray.AutoArrange = false;
				compTray.AutoScroll = true;
				compTray.Height = 50;
				compTray.CreateControl();
				if(fDesignerHost != null)
					fDesignerHost.AddService(typeof(ComponentTray), compTray);
			}
		}
		private void FinalizeComponentTray() {
			if(compTray != null) {
				fDesignerHost.RemoveService(typeof(ComponentTray));
				if(!compTray.IsDisposed)
					compTray.Dispose();
				compTray = null;
			}
		}
		void UpdateStatusInfo() {
			IComponent component = GetSelectedComponent();
			UpdateStatus(component);
		}
		void OnSelectionChanged(object sender, EventArgs e) {
			if(!fDesignerHost.Loading && !DesignMethods.IsDesignerInTransaction(fDesignerHost)) {
				UpdateStatusInfo();
				selectionHelper.RotateSelection();
			}
		}
		private void UpdateStatus(IComponent component) {
			if(tabControl != null)
				tabControl.UpdateStatusValue(GetComponentStatus(component));
		}
		private string GetComponentStatus(IComponent component) {
			if(component is XtraReport)
				return GetReportStatus();
			if(component is XRControl) {
				XRControlDesignerBase designer = (XRControlDesignerBase)GetDesigner(component);
				if(designer != null)
					return designer.GetStatus();
			} else {
				FakeComponent chartElement = component as FakeComponent;
				if(chartElement == null) {
					if(component != null && component.Site != null)
						return component.Site.Name;
				} else {
					ChartElementNamed namedElement = chartElement.Object as ChartElementNamed;
					return namedElement == null ?
						chartElement.Object.ToString().TrimStart('(').TrimEnd(')') :
						namedElement.Name;
				}
			}
			return String.Empty;
		}
		private string GetReportStatus() {
			return String.Format(ReportLocalizer.GetString(ReportStringId.RepTabCtl_ReportStatus), Component.Site.Name, RootReport.PaperKind);
		}
		protected IComponent GetSelectedComponent() {
			IList components = new ArrayList(selectionService.GetSelectedComponents());
			return (components.Count == 1) ? components[0] as IComponent : null;
		}
		protected internal object[] GetSelectedComponents() {
			ArrayList comps = new ArrayList(selectionService.GetSelectedComponents());
			return (object[])comps.ToArray(typeof(object));
		}
		private void OnTransactionClosed(object sender, DesignerTransactionCloseEventArgs e) {
			SetControlsWarningToolTip();
			UpdateStatus(GetSelectedComponent());
			UpdateView();
		}
		protected virtual void OnDesignerActivate(object sender, System.EventArgs e) {
			if(activating || deactivating)
				return;
			if(!bandsValidated) {
#if DEBUGTEST
				if(!TestEnvironment.IsTestRunning())
#endif
					if(this.RootReport.GetType() == typeof(XtraReport)) {
						UndoEngine undoEngine = fDesignerHost.GetService(typeof(UndoEngine)) as UndoEngine;
						undoEngine.ExecuteWithoutUndo(() => new BandsValidator(fDesignerHost).EnsureExistence(BandKind.TopMargin, BandKind.Detail, BandKind.BottomMargin));						
					}
				bandsValidated = true;
			}
			activating = true;
			ActivateReportTool();
			ActivationService.ActiveDesigner = this;
			activating = false;
			SetControlsWarningToolTip();
		}
		protected virtual void ActivateReportTool() {
			if(reportTool != null) {
				reportTool.UpdateToolItems();
				reportTool.ShowNoActivate();
			}
		}
		private void OnDesignerDeactivate(object sender, System.EventArgs e) {
			if(activating || deactivating)
				return;
			if(NewRootComponentIsXtraReport())
				return;
			if((ActivationService.ActiveDesigner == null || IsActive)) {
				deactivating = true;
				DeactivateReportTool();
				deactivating = false;
			}
		}
		protected virtual void DeactivateReportTool() {
			if(reportTool != null)
				reportTool.Hide();
		}
		bool NewRootComponentIsXtraReport() {
			IDesignerEventService designerEventService = fDesignerHost.GetService(typeof(IDesignerEventService)) as IDesignerEventService;
			if(designerEventService == null || designerEventService.ActiveDesigner == null)
				return false;
			XtraReport newRootComponent = designerEventService.ActiveDesigner.RootComponent as XtraReport;
			return newRootComponent != null;
		}
		protected virtual void OnLoadComplete(object sender, System.EventArgs e) {
			((IDesignerHost)sender).LoadComplete -= new EventHandler(this.OnLoadComplete);
			if(trayLayoutSuspended && compTray != null) {
				compTray.ResumeLayout();
			}
			ReportFrame.OnLoadComplete();
			ReportFrame.UpdateProperties();
			RootReport.DesignerLoading = false;
			RootReport.EndInit();
			UpdateComponentTrayVisibility();
			bandViewInfoService = (IBandViewInfoService)fDesignerHost.GetService(typeof(IBandViewInfoService));
			SelectComponents(new object[] { Component });
			MenuCommandHandler commandHandler = fDesignerHost.GetService(typeof(MenuCommandHandler)) as MenuCommandHandler;
			if(commandHandler != null)
				commandHandler.OnSelectionChangedCore();
			IIdleService serv = GetService(typeof(IIdleService)) as IIdleService;
			if(serv != null)
				serv.SetEnabled(true);
			ValidateLicence();
		}
		protected virtual void ValidateLicence() {
		}
		public void SelectComponents(ICollection components) {
			if(components != null && components.Count > 0) {
				selectionService.SetSelectedComponents(components, SelectionTypes.Replace);
			}
		}
		void CreateCommandSet() {
			Assembly assembly = Serialization.XRSerializer.GetReferencedAssembly("System.Design");
			if(assembly != null) {
				Type t = assembly.GetType("System.Windows.Forms.Design.CommandSet");
				if(t != null) {
					commandSet = t.InvokeMember(null, BindingFlags.CreateInstance, null, null, new object[] { fDesignerHost.RootComponent.Site });
				}
			}
		}
		protected virtual void ActivateMenuCommands() {
			if(!fDesignerHost.RootComponent.Equals(Component))
				return;
			MenuCommandHandler commandHandler = fDesignerHost.GetService(typeof(MenuCommandHandler)) as MenuCommandHandler;
			if(commandHandler == null)
				return;
			CreateCommandSet();
			commandHandler.RegisterMenuCommands();
		}
		internal void OnEditBands(object sender, EventArgs e) {
			try {
				XRSmartTagService smartTagService = fDesignerHost.GetService<XRSmartTagService>();
				smartTagService.HidePopup();
				UITypeEditor editor = new BandCollectionEditor();
				editor.EditValue(Component.Site, ActiveReport.Bands);
			} catch { }
		}
		virtual internal bool OnReportWizard() {
			XtraReportModel model = CreateReportModel(ReportHelper.GetEffectiveDataSource(RootReport), RootReport.DataMember);
			if(model != null) {
				foreach(Band band in RootReport.Bands)
					ControlDestroyer.Destroy(band.Controls, fDesignerHost);
				ControlDestroyer.Destroy(RootReport.Bands, fDesignerHost);
				ControlDestroyer.Destroy(RootReport.CrossBandControls, fDesignerHost);
				IWizardCustomizationService serv = fDesignerHost.GetService(typeof(IWizardCustomizationService)) as IWizardCustomizationService;
				serv.CreateReportSafely(fDesignerHost, model, null, string.Empty);
				return true;
			}
			return false;
		}
		XtraReportModel CreateReportModel(object dataSource, string dataMember) {
			UserLookAndFeel lookAndFeel = fDesignerHost.GetService<ILookAndFeelService>().LookAndFeel;
			IUIService uiService = fDesignerHost.GetService<IUIService>();
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			DefaultWizardRunnerContext wizardRunnerContext = new DefaultWizardRunnerContext(lookAndFeel, owner);
			var runner = new LayoutWizardRunner<XtraReportModel>(wizardRunnerContext);
			var client = new DataSourceWizardClientUI(null, null, null, null, null);
			IWizardCustomizationService serv = fDesignerHost.GetService(typeof(IWizardCustomizationService)) as IWizardCustomizationService;
			var model = new XtraReportModel() { DataSchema = CreateDataSchema(dataSource, dataMember) };
			if(runner.Run(client, model, customization => {
				serv.CustomizeReportWizardSafely(customization);
			}))
				return runner.WizardModel;
			return null;
		}
		static object CreateDataSchema(object dataSource, string dataMember) {
			using(XRDataContext dataContext = new XRDataContext(null, true)) {
				var properties = new DataContextUtils(dataContext).GetDisplayedProperties(dataSource, dataMember, null);
				DataView dataView = new DataView(new Xpo.DB.SelectStatementResultRow[0]);
				if(properties != null) {
					foreach(PropertyDescriptor property in properties) {
						dataView.ColumnDescriptors.Add(property);
					}
				}
				return dataView;
			}
		}
		protected virtual void OnComponentAdded(object source, ComponentEventArgs e) {
			IComponent comp = e.Component;
			if(!comp.IsVisual() && comp.IsVisible()) {
				if(fDesignerHost.Loading && trayLayoutSuspended == false) {
					trayLayoutSuspended = true;
					compTray.SuspendLayout();
				}
				compTray.AddComponent(comp);
				UpdateComponentTrayVisibility();
			}
			if(fDesignerHost.Loading)
				return;
			if(comp is XRControlStyle)
				RootReport.StyleSheet.Add((XRControlStyle)comp);
			if(comp is FormattingRule)
				RootReport.FormattingRuleSheet.Add((FormattingRule)comp);
			if((comp is DataComponentBase || TypeDescriptor.GetAttributes(comp)[typeof(ReportAssociatedComponentAttribute)] != null) && !RootReport.ComponentStorage.Contains(comp))
				RootReport.ComponentStorage.Add(comp);
		}
		void UpdateReportProperties(IComponent comp) {
			if(fDesignerHost.Loading) return;
			IComponent component = GetSelectedComponent();
			XtraReportBase report = component is XtraReportBase ? (XtraReportBase)component : 
				component is XRControl ? ((XRControl)component).Report : null;
			if(report == null) return;
			if(BindingHelper.IsDataSource(comp) && report.DataSource == null) {
				PropertyDescriptor desc = TypeDescriptor.GetProperties(report)["DataSource"];
				if(desc != null)
					RaiseComponentChanging(desc);
				report.DataSource = comp;
				if(desc != null)
					RaiseComponentChanged(desc, null, null);
				if(comp is DataSet && ((DataSet)comp).Tables.Count > 0) {
					desc = TypeDescriptor.GetProperties(report)["DataMember"];
					if(desc != null)
						RaiseComponentChanging(desc);
					report.DataMember = ((DataSet)comp).Tables[0].TableName;
					if(desc != null)
						RaiseComponentChanged(desc, null, null);
				}
			}
			if(DevExpress.Data.Native.BindingHelper.IsDataAdapter(comp) && report.DataAdapter == null) {
				PropertyDescriptor desc = TypeDescriptor.GetProperties(report)["DataAdapter"];
				RaiseComponentChanging(desc);
				try {
					report.DataAdapter = comp;
				} finally {
					RaiseComponentChanged(desc, null, null);
				}
			}
		}
		void OnComponentRemoving(object source, System.ComponentModel.Design.ComponentEventArgs e) {
			if(fDesignerHost.Loading)
				return;
			IComponent component = e.Component;
			XRComponentDesigner designer = GetParentDesigner(component) as XRComponentDesigner;
			if(designer != null)
				designer.OnCollectionChanging(e.Component);
		}
		void OnComponentRemoved(object source, System.ComponentModel.Design.ComponentEventArgs e) {
			IComponent component = e.Component;
			if(!component.IsVisual() && compTray != null) {
				compTray.RemoveComponent(component);
				UpdateComponentTrayVisibility();
			}
			if(fDesignerHost.Loading)
				return;
			XRComponentDesigner designer = GetParentDesigner(component);
			if(designer != null)
				designer.OnCollectionChanged(e.Component);
			ValidateDataSources(component);
			ValidateDataAdapters(component);
			ValidateControlBindings(RootReport.Bands, component);
			selectionHelper.SetTargetSelection(RootReport, component);
			RootReport.ComponentStorage.Remove(component);
		}
		protected internal virtual bool ShouldSaveToLayout(IDataContainer dataContainer) {
			IComponent dataSource = dataContainer.GetSerializableDataSource() as IComponent;
			IComponent dataAdapter = dataContainer.DataAdapter as IComponent;
			return dataSource != null && dataAdapter != null;
		}
		void ValidateDataSources(IComponent component) {
			foreach(IDataContainerBase dataContainer in EnumerateDataContainers()) {
				ValidateDataSource(dataContainer, component);
			}
		}
		void ValidateDataAdapters(IComponent component) {
			foreach(IDataContainerBase dataContainerBase in EnumerateDataContainers()) {
				IDataContainer dataContainer = dataContainerBase as IDataContainer;
				if(dataContainer != null)
					ValidateDataAdapter(dataContainer, component);
			}
		}
		IEnumerable<IDataContainerBase> EnumerateDataContainers() {
			foreach(IDataContainerBase dataContainer in new DataContainerEnumerator().EnumerateDataContainers(RootReport)) {
				yield return dataContainer;
			}
			foreach(Parameter parameter in RootReport.Parameters) {
				DynamicListLookUpSettings settings = parameter.LookUpSettings as DynamicListLookUpSettings;
				if(settings != null)
					yield return settings;
			}
		}
		XRComponentDesigner GetParentDesigner(IComponent c) {
			return c is XRControl ? GetParentDesigner((XRControl)c) :
				c is Parameter || c is CalculatedField ? fDesignerHost.GetDesigner(fDesignerHost.RootComponent) as XRComponentDesigner :
				null;
		}
		XRComponentDesigner GetParentDesigner(XRControl c) {
			if(c == null || c.Parent == null)
				return null;
			return fDesignerHost.GetDesigner(c.Parent) as XRComponentDesigner ?? GetParentDesigner(c.Parent);
		}
		void ValidateDataSource(IDataContainerBase dataContainer, IComponent component) {
			if(!BindingHelper.IsListSource(component))
				return;
			object dataSource = dataContainer.DataSource;
			if(Comparer.Equals(dataSource, component) ||
				Comparer.Equals(DataSourceHelper.ConvertToDataSet(dataSource), component)) {
				dataContainer.DataSource = null;
				if(this.changeService != null)
					this.changeService.OnComponentChanged(dataContainer, null, null, null);
			}
		}
		private void OnComponentChanged(object source, System.ComponentModel.Design.ComponentChangedEventArgs e) {
			IComponent comp = e.Component as IComponent;
			if(comp == null || comp.Site == null)
				return;
			if(comp is XRControlStyle && e.Member != null && e.Member.Name == "Name" && !RootReport.Loading)
				RootReport.SyncStyleName((string)e.OldValue, (string)e.NewValue);
			XRComponentDesigner designer = GetDesigner(comp) as XRComponentDesigner;
			if(designer != null)
				designer.OnComponentChanged(e);
			if(!DesignMethods.IsDesignerInTransaction(fDesignerHost)) {
				UpdateStatus(comp);
				UpdateView();
			}
			if(!(comp is XRRichTextBase && e.Member == null))
				new DesignerHostExtensions(fDesignerHost).CommitInplaceEditors();
		}
		private void OnComponentRename(object source, System.ComponentModel.Design.ComponentRenameEventArgs e) {
			IComponent comp = e.Component as IComponent;
			if(comp == null || comp.Site == null)
				return;
			if(comp is Parameter && !string.IsNullOrEmpty(e.OldName) && !string.IsNullOrEmpty(e.NewName)) {
				foreach(IComponent c in fDesignerHost.Container.Components) {
					if(c is IParametersRenamer)
						((IParametersRenamer)c).RenameParameter(e.OldName, e.NewName);
				}
			}
		}
		public override void OnComponentChanged(ComponentChangedEventArgs e) {
			ReportFrame.UpdateProperties();
			if(e.Component.Equals(RootReport) && e.Member != null) {
				if(e.Member.Name == XRComponentPropertyNames.XmlDataPath)
					ValidateControlBindings(RootReport.Bands, ReportHelper.GetEffectiveDataSource(RootReport));
				else if(e.Member.Name == XRComponentPropertyNames.DataSource) {
					ValidateDataAdapter();
					SetDataMember(RootReport, e.NewValue);
				}
			}
		}
		void SetControlsWarningToolTip() {
			List<string> controlsNamesOverlapped = new List<string>();
			List<string> controlsNamesOutsideBand = new List<string>();
			List<string> unsavedSubreports = new List<string>();
			IWindowsService windowsService = (IWindowsService)GetService(typeof(IWindowsService));
			foreach(XRControl control in RootReport.AllControls<XRControl>()) {
				if(ShowPrintingWarnings && control.HasPrintingWarning())
					controlsNamesOutsideBand.Add(control.Name);
				if(ShowPrintingWarnings && windowsService != null && windowsService.ShouldSaveSubreport(control as XRSubreport)) {
					string name = windowsService.GetReportSourceDisplayName(control as XRSubreport);
					if(!unsavedSubreports.Contains(name))
						unsavedSubreports.Add(name);
				}
				if(ShowExportWarnings && control.HasExportWarning())
					controlsNamesOverlapped.Add(control.Name);
			}
			string toolTipTextOverlapped = GetWarningText(ReportStringId.Msg_WarningControlsAreOverlapped, controlsNamesOverlapped.ToArray());
			string toolTipTextOutsideBand = GetWarningText(ReportStringId.Msg_WarningControlsAreOutOfMargin, controlsNamesOutsideBand.ToArray());
			string toolTipTextUnsaved = GetWarningText(ReportStringId.Msg_WarningUnsavedReports, unsavedSubreports.ToArray());
			List<string> hints = new List<string>(3);
			if(!string.IsNullOrEmpty(toolTipTextOverlapped))
				hints.Add(toolTipTextOverlapped);
			if(!string.IsNullOrEmpty(toolTipTextOutsideBand))
				hints.Add(toolTipTextOutsideBand);
			if(!string.IsNullOrEmpty(toolTipTextUnsaved))
				hints.Add(toolTipTextUnsaved);
			ToolTipService.GetInstance(fDesignerHost).SetToolTip(string.Join(Environment.NewLine, hints.ToArray()));
			controlsNamesOverlapped.Clear();
			controlsNamesOutsideBand.Clear();
			hints.Clear();
		}
		protected virtual void ValidateDataAdapter() {
			RootReport.DataAdapter = DataAdapterHelper.ValidateDataAdapter(fDesignerHost, RootReport.DataSource, RootReport.DataMember);
		}
		protected void UpdateView() {
			if(bandViewInfoService != null)
				bandViewInfoService.UpdateView();
		}
		public virtual bool GetToolSupported(ToolboxItem tool) {
			return true;
		}
		public virtual void ToolPicked(ToolboxItem tool) {
			if(tool.TypeName == "DevExpress.XtraCharts.ChartControl")
				tool = new ToolboxItem(typeof(XRChart));
			IMenuCommandService commandService = GetService(typeof(IMenuCommandService)) as IMenuCommandService;
			if(commandService != null) {
				MenuCommand menuCommand = commandService.FindCommand(StandardCommands.TabOrder);
				if(menuCommand != null && menuCommand.Checked) {
					return;
				}
			}
			IComponent[] components = { };
			string description = String.Format("{0} '{1}'", DesignSR.Trans_CreateTool, tool.DisplayName);
			DesignerTransaction transaction = fDesignerHost.CreateTransaction(description);
			try {
				components = CreateComponents(tool);
				if(components.Length == 0)
					return;
				UpdateReportProperties(components[0]);
				IToolboxService tbxService = GetService(typeof(IToolboxService)) as IToolboxService;
				if(tbxService != null)
					tbxService.SelectedToolboxItemUsed();
				if(components[0] is Control) {
					WinControlContainer winControlContainer = AddWinControlContainer((Control)components[0]);
					components = new IComponent[] { winControlContainer };
				}
				SelectXRControls(components);
				commandService.GlobalInvoke(StandardCommands.BringToFront);
			} catch {
				transaction.Cancel();
			} finally {
				if(components.Length > 0)
					transaction.Commit();
				else
					transaction.Cancel();
			}
		}
		private void SelectXRControls(IComponent[] components) {
			ArrayList controls = new ArrayList();
			foreach(IComponent comp in components) {
				if(comp.IsVisible() || comp is XRControl)
					controls.Add(comp);
			}
			if(controls.Count > 0)
				selectionService.SetSelectedComponents(controls, SelectionTypes.Replace);
		}
		public virtual ObjectNameCollection GetRelationNames() {
			object dataSource = ReportHelper.GetEffectiveDataSource(ActiveReport);
			string dataMember = ActiveReport.DataMember;
			using(DataContext dataContext = new DataContext(true)) {
				PropertyDescriptorCollection listProperties = dataContext.GetItemProperties(dataSource, dataMember, delegate(Type type) {
					return ListTypeHelper.IsListType(type);
				});
				ObjectNameCollection objectNames = new ObjectNameCollection();
				foreach(PropertyDescriptor listProperty in listProperties) {
					ObjectName objectName = XRDataContext.CreateObjectName(dataSource, dataMember, listProperty.Name);
					if(objectName == null)
						continue;
					PropertyDescriptorCollection itemProperties = dataContext.GetListItemProperties(dataSource, objectName.FullName);
					if(itemProperties.Count > 0)
						objectNames.Add(objectName);
				}
				return objectNames;
			}
		}
		public virtual WinControlContainer AddWinControlContainer(Control control) {
			try {
				IComponent[] components = CreateComponents(new ToolboxItem(typeof(WinControlContainer)));
				WinControlContainer ctl = components[0] as WinControlContainer;
				ctl.WinControl = control;
				return ctl;
			} catch { return null; }
		}
		protected IComponent[] CreateComponents(ToolboxItem tool) {
			try {
				if(!CanCorrectlyCreateXRControlFromToolBoxItem(tool))
					return new IComponent[0];
				;
				ArrayList components = new ArrayList();
				components.AddRange(tool.CreateComponents(fDesignerHost));
				return (IComponent[])components.ToArray(typeof(IComponent));
			} catch { }
			return new IComponent[0];
		}
		bool CanCorrectlyCreateXRControlFromToolBoxItem(ToolboxItem tool) {
			Type componentType = DevExpress.XtraReports.UserDesigner.Native.XRToolboxService.GetType(tool);
			if(typeof(XRControl).IsAssignableFrom(componentType)) {
				XRControl controlParent = ParentSearchHelper.FindParent(RootReport, componentType, fDesignerHost);
				if(!LockService.GetInstance(fDesignerHost).CanChangeComponent(controlParent))
					return false;
			}
			return true;
		}
		private void ValidateControlBindings(IList bands, object dataSource) {
			NestedComponentEnumerator compEnum = new NestedComponentEnumerator(bands);
			while(compEnum.MoveNext()) {
				XRControlDesigner designer = GetDesigner(compEnum.Current) as XRControlDesigner;
				if(designer != null)
					designer.ValidateControlBindings(dataSource);
			}
		}
		private IDesigner GetDesigner(IComponent component) {
			return (component != null) ? fDesignerHost.GetDesigner(component) : null;
		}
		protected internal void CopyWatermark(DevExpress.XtraPrinting.Drawing.Watermark source, DevExpress.XtraPrinting.Drawing.Watermark dest) {
			DesignerTransaction transaction = fDesignerHost.CreateTransaction(String.Format(DesignSR.Trans_ChangeProp, XRComponentPropertyNames.Watermark));
			try {
				XRControlDesignerBase.RaiseComponentChanging(changeService, RootReport, XRComponentPropertyNames.Watermark);
				dest.CopyFrom(source);
				XRControlDesignerBase.RaiseComponentChanged(changeService, RootReport);
			} catch {
				transaction.Cancel();
			} finally {
				transaction.Commit();
			}
		}
		void UpdateComponentTrayVisibility() {
			if(ReportFrame == null || compTray == null || fDesignerHost.Loading)
				return;
			if((ComponentVisibility & ComponentVisibility.ComponentTray) > 0 && compTray.ComponentCount > 0)
				ReportFrame.AddSplitWindow(compTray);
			else
				ReportFrame.RemoveSplitWindow(compTray);
		}
		public bool IsInplaceEditorActive() {
			XRControl control = this.selectionService.PrimarySelection as XRControl;
			if(control != null) {
				XRTextControlBaseDesigner designer = this.fDesignerHost.GetDesigner(control) as XRTextControlBaseDesigner;
				return designer != null && designer.IsInplaceEditingMode;
			}
			return false;
		}
		protected override bool TryGetCollectionName(IComponent c, out string name) {
			if(c is CalculatedField) {
				name = "CalculatedFields";
				return true;
			}
			if(c is XRCrossBandControl) {
				name = "CrossBandControls";
				return true;
			}
			if(c is Parameter) {
				name = "Parameters";
				return true;
			}
			return base.TryGetCollectionName(c, out name);
		}
		protected internal override bool CanAddBand(BandKind bandKind) {
			if(IsEUD && RootReport.LockedInUserDesigner)
				return false;
			return RootReport.Bands.CanAdd(bandKind);
		}
		protected internal virtual NewParameterEditorForm CreateNewParameterEditorForm() {
			return new NewParameterEditorForm(fDesignerHost);
		}
	}
	class DataSourceDesignCollector : DataSourceCollector {
		IDesignerHost host;
		public DataSourceDesignCollector(IDesignerHost host)
			: base((XtraReport)host.RootComponent) {
			this.host = host;
		}
		protected override void CollectCore(ArrayList dataSources) {
			foreach(IComponent c in host.Container.Components)
				if(BindingHelper.IsDataSource(c))
					dataSources.Add(c);
			base.CollectCore(dataSources);
		}
	}
	[Flags]
	public enum ComponentVisibility {
		None = 0,
		[EditorBrowsable(EditorBrowsableState.Never)]
		ComponentTray = 1,
		ReportExplorer = 2
	}
}
