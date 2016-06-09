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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.DataAccess.Native;
using DevExpress.Xpf.Diagram;
using DevExpress.Xpf.Reports.UserDesigner.FieldList;
using DevExpress.Xpf.Reports.UserDesigner.FontProperties;
using DevExpress.Xpf.Reports.UserDesigner.FontProperties.Native;
using DevExpress.Xpf.Reports.UserDesigner.Layout;
using DevExpress.Xpf.Reports.UserDesigner.Layout.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtension;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
using DevExpress.Xpf.Reports.UserDesigner.ReportExplorer;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel.Native;
using DevExpress.Xpf.Reports.UserDesigner.Toolbox;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.XtraReports;
using DevExpress.XtraReports.Configuration;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Parameters;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel {
	public sealed class XRControlModelFactory : ModelFactory<XtraReportModelBase, IComponent, XRComponentModelBase> {
		internal XRControlModelFactory(XtraReportModelBase owner)
			: base(owner, x => x.XRObject) {
			Initialize(this, owner.Owner.ModelRegistries);
		}
	}
	public abstract class XtraReportModelBase : XRModelBase, IReportExplorerContainerItem {
		XRControlModelFactory factory;
		public XRControlModelFactory Factory {
			get {
				if(factory == null)
					factory = new XRControlModelFactory(this);
				return factory;
			}
		}
		readonly IObjectTracker xrObjectTracker;
		public XtraReportModelBase(IReportModelOwner owner, XtraReport report, ImageSource icon)
			: base(report, null) {
			Guard.ArgumentNotNull(owner, "owner");
			Owner = owner;
			DXDisplayNameAttribute.UseResourceManager = Settings.Default.ShowUserFriendlyNamesInUserDesigner;
			this.icon = icon;
			this.designerHost = DesignSite.EnableDesignMode(XRObject);
			Layout = LayoutProvider.GetLayout(XRObject);
			this.PropertyChanged += (s, e) => OnThisPropertyChanged(e);
			Layout.PropertyChanged += (s, e) => OnLayoutPropertyChanged(e);
			this.controls = XRObjectModelCollection<XRComponentModelBase>.Create(this, GetXRControlsCollection(), Factory);
			this.scripts = new ObservableCollection<string>();
			this.nameProperty = CreateXRPropertyModel(() => XRObject.Name, () => XRObject.Name, x => XRObject.Site.Name = x);
			this.nameProperty.ValueChanged += (s, e) => {
				RaisePropertyChanged(() => Name);
				DiagramItem.Diagram.InvalidateRenderLayer();
			};
			this.dataSourceWizardExtensions = new DataSourceWizardExtensions(this);
			this.parameterService = new XRParameterService(controls, this.Factory);
			Controls.CollectionChanged += (s, e) => {
				UpdateReportExplorerItems(e);
				DiagramItem.Diagram.InvalidateRenderLayer();
			};
			UpdateInnerBottomBand();
			Tracker.GetTracker(XRObject, out xrObjectTracker);
			xrObjectTracker.ObjectPropertyChanged += (s, e) => {
				if(e.PropertyName == ExpressionHelper.GetPropertyName((XtraReport x) => x.DataSource))
					UpdateDataSources();
			};
		}
		protected virtual IList<IComponent> GetXRControlsCollection() {
			var controls = ListAdapter<XRControl>.FromTwoObjectLists<XRCrossBandControl, Band>(XRObject.CrossBandControls, XRObject.Bands);
			IList<FormattingRule> formattingRuleSheet = XRObject.FormattingRuleSheet;
			var styles = ListAdapter<XRControlStyle>.FromObjectList(XRObject.StyleSheet);
			IList<CalculatedField> calculatedFields = XRObject.CalculatedFields;
			IList<IComponent> components = XRObject.ComponentStorage;
			var collection = ListAdapter<IComponent>.FromTwoLists(styles, components);
			collection = ListAdapter<IComponent>.FromTwoLists(formattingRuleSheet, collection);
			collection = ListAdapter<IComponent>.FromTwoLists(XRObject.Parameters, collection);
			collection = ListAdapter<IComponent>.FromTwoLists(calculatedFields, collection);
			collection = ListAdapter<IComponent>.FromTwoLists(controls, collection);
			return collection;
		}
		public readonly IReportModelOwner Owner;
		readonly IDesignerHost designerHost;
		public IDesignerHost DesignerHost { get { return designerHost; } }
		protected BaseReportElementLayout Layout { get; private set; }
		public new XtraReport XRObject { get { return (XtraReport)base.XRObject; } }
		public new XRDiagramRoot DiagramItem { get { return (XRDiagramRoot)base.DiagramItem; } }
		readonly ObservableCollection<XRComponentModelBase> controls;
		public ObservableCollection<XRComponentModelBase> Controls { get { return controls; } }
		readonly IDataSourceWizardExtensions dataSourceWizardExtensions;
		public IDataSourceWizardExtensions DataSourceWizardExtensions { get { return dataSourceWizardExtensions; } }
		readonly ObservableCollection<string> scripts;
		public ObservableCollection<string> Scripts { get { return scripts; } }
		public string ScriptsSource {
			get { return XRObject.ScriptsSource; }
			set {
				XRObject.ScriptsSource = value;
				RaisePropertyChanged(() => ScriptsSource);
			}
		}
		readonly IParameterService parameterService;
		public IParameterService ParameterService { get { return parameterService; } }
		protected virtual void OnLayoutPropertyChanged(PropertyChangedEventArgs e) {
			if(e.PropertyName == GetPropertyName(() => Layout.Param3))
				RaisePropertyChanged(() => Width);
			else if(e.PropertyName == GetPropertyName(() => Layout.Param1))
				RaisePropertyChanged(() => LeftPadding);
			else if(e.PropertyName == GetPropertyName(() => Layout.Param2))
				RaisePropertyChanged(() => RightPadding);
			else if(e.PropertyName == GetPropertyName(() => Layout.Ref2))
				RaisePropertyChanged(() => Height);
		}
		protected virtual void OnThisPropertyChanged(PropertyChangedEventArgs e) {
			if(e.PropertyName == GetPropertyName(() => Width))
				RaisePropertyChanged(() => Size);
			else if(e.PropertyName == GetPropertyName(() => Height))
				RaisePropertyChanged(() => Size);
		}
		public virtual double LeftPadding { get { return Layout.Param1; } }
		public virtual double RightPadding { get { return Layout.Param2; } }
		public virtual double Width {
			get { return Layout.Param3; }
			set { Layout.Param3 = value; }
		}
		public Size Size {
			get { return new Size(Width, Height); }
			set {
				Width = value.Width;
				Height = value.Height;
			}
		}
		BandModelBase innerBottomBand;
		void UpdateInnerBottomBand() {
			innerBottomBand = Layout.Ref2.With(x => (BandModelBase)Factory.GetModel(x));
			if(innerBottomBandTracker != null)
				innerBottomBandTracker.Dispose();
			innerBottomBandTracker = innerBottomBand.With(x => XRControlModelBoundsTracker.NewBottomTracker(x, () => RaisePropertyChanged(() => Height)));
			RaisePropertyChanged(() => Height);
		}
		IDisposable innerBottomBandTracker;
		public virtual double Height {
			get { return innerBottomBand.Return(x => x.Bottom, () => 0.0); }
			set { }
		}
		IReportLayoutProvider layoutProvider;
		public IReportLayoutProvider LayoutProvider {
			get {
				if(layoutProvider == null)
					layoutProvider = CreateReportLayoutProvider();
				return layoutProvider;
			}
		}
		protected virtual IReportLayoutProvider CreateReportLayoutProvider() {
			return new ReportLayoutProvider(XRObject);
		}
		IFontPropertiesProvider fontPropertiesProvider;
		public IFontPropertiesProvider FontPropertiesProvider {
			get {
				if(fontPropertiesProvider == null)
					fontPropertiesProvider = CreateFontPropertiesProvider();
				return fontPropertiesProvider;
			}
		}
		protected virtual IFontPropertiesProvider CreateFontPropertiesProvider() {
			return new FontPropertiesProvider();
		}
		readonly ImageSource icon;
		public ImageSource Icon { get { return icon; } }
		string IReportExplorerItem.TypeString { get { return XRObject.GetDisplayName(); } }
		readonly XRPropertyModel<string> nameProperty;
		[ParenthesizePropertyName(true)]
		[SRCategory(ReportStringId.CatDesign)]
		public string Name {
			get { return nameProperty.Value; }
			set { nameProperty.Value = value; }
		}
		public XRModelBase SelectedModel {
			get { return DiagramItem.With(x => x.Diagram.PrimarySelection).With(x => GetXRModel(x)); }
			set {
				if(value == null)
					DiagramItem.Do(x => x.Diagram.ClearSelection());
				else
					DiagramItem.Do(x => x.Diagram.SelectItem(value.DiagramItem));
			}
		}
		protected override IEnumerable<PropertyDescriptor> GetEditableProperties() {
			return base.GetEditableProperties().InjectProperty(this, x => x.Name);
		}
		protected override void AttachDiagramItem() {
			base.AttachDiagramItem();
			DiagramItem.Diagram.XRCommands = CreateXRCommands();
			DiagramItem.Diagram.ItemFactory = x => Factory.GetModel(x, true).DiagramItem;
			DiagramItem.Diagram.SerializeCallback = (items, stream) => XRSerializationHelper.SerializeControl(items.Select(GetXRModel).Cast<XRControlModelBase>().Select(x => x.XRObject), stream);
			DiagramItem.Diagram.DeserializeCallback = data => XRSerializationHelper.DeserializeControl(data).Select(x => Factory.GetModel(x).DiagramItem);
			ToolboxTools = CreateToolboxTools(DiagramItem.Diagram).ToList().AsReadOnly();
			SelectedToolboxTool = ToolboxTools.With(x => x.FirstOrDefault());
			UpdateReportExplorerItems(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			BindDiagram(XRDiagramControl.LeftPaddingProperty, () => LeftPadding, BindingMode.OneWay);
			BindDiagram(XRDiagramControl.RightPaddingProperty, () => RightPadding, BindingMode.OneWay);
			BindDiagram(XRDiagramControl.PageSizeProperty, () => Size, BindingMode.OneWay);
			BindDiagramItemToXRObject(() => DiagramItem.Diagram.MeasureUnit = GetMeasureUnit(XRObject.ReportUnit), () => XRObject.ReportUnit);
			BindDiagramItems(Controls, DiagramItem.Items);
			BindDiagramItemToXRObject(() => DiagramItem.Diagram.SnapToItems = XRObject.SnappingMode == SnappingMode.SnapLines, () => XRObject.SnappingMode);
			DiagramItem.Diagram.SelectionChanged += (s, e) => this.RaisePropertyChanged(() => SelectedModel);
			BindDiagramItemToXRObject(() => DiagramItem.Diagram.GridSize = new Size(BoundsConverter.ToDouble(XRObject.SnapGridSize, XRObject.Dpi), BoundsConverter.ToDouble(XRObject.SnapGridSize, XRObject.Dpi)), () => XRObject.SnapGridSize);
			UpdateDiagramActiveTool();
		}
		void UpdateDataSources() {
			DebugHelper2.Assert(DiagramItem.Diagram != null, "UpdateDataSources");
			var dataSources = FieldListDataSource<XRDiagramControl>.GetDataSources(XRObject, DiagramItem.Diagram);
			var parameters = FieldListDataSource<XRDiagramControl>.GetParameters(XRObject, DiagramItem.Diagram);
			FieldListNodes = dataSources.Concat(parameters.Yield()).ToArray();
			DataSources = dataSources;
		}
		static readonly MeasureUnit inches = new MeasureUnit(MeasureUnits.Inches.Dpi, MeasureUnits.Inches.Multiplier, MeasureUnits.Inches.Name, MeasureUnits.Inches.StepsData.TakeWhile(x => x.ZoomLevel <= 1.0).ToArray());
		static readonly MeasureUnit pixels = new MeasureUnit(MeasureUnits.Pixels.Dpi, MeasureUnits.Pixels.Multiplier, MeasureUnits.Pixels.Name, MeasureUnits.Pixels.StepsData.TakeWhile(x => x.ZoomLevel <= 1.0).ToArray());
		static MeasureUnit GetMeasureUnit(ReportUnit unit) {
			switch(unit) {
				case ReportUnit.HundredthsOfAnInch: return inches;
				case ReportUnit.TenthsOfAMillimeter: return MeasureUnits.Millimeters;
				default: return pixels;
			}
		}
		protected abstract BaseXRCommands CreateXRCommands();
		public virtual BandRendererBase CreateBandRenderer(Band band, XtraReport report) { return new BandRenderer(band, report); }
		protected override DiagramItem CreateDiagramItem() {
			return new XRDiagramControl().RootItem;
		}
		protected override void OnAttachItem() {
			((DesignSite)XRObject.Site).OnAddToDesignerHost();
		}
		protected override void OnDetachItem() {
			((DesignSite)XRObject.Site).OnRemoveFromDesignerHost();
		}
		IEnumerable<ToolViewModel> toolboxTools;
		public IEnumerable<ToolViewModel> ToolboxTools {
			get { return toolboxTools; }
			private set { SetProperty(ref toolboxTools, value, () => ToolboxTools); }
		}
		ToolViewModel selectedToolboxTool;
		public ToolViewModel SelectedToolboxTool {
			get { return selectedToolboxTool; }
			set { SetProperty(ref selectedToolboxTool, value, () => SelectedToolboxTool, UpdateDiagramActiveTool); }
		}
		void UpdateDiagramActiveTool() {
			DiagramItem.Diagram.ActiveTool = SelectedToolboxTool.With(x => x.Tool);
		}
		IEnumerable<FieldListNodeBase<XRDiagramControl>> fieldListNodes;
		public IEnumerable<FieldListNodeBase<XRDiagramControl>> FieldListNodes {
			get { return fieldListNodes; }
			private set { SetProperty(ref fieldListNodes, value, () => FieldListNodes); }
		}
		IEnumerable<FieldListNodeBase<XRDiagramControl>> dataSources;
		public IEnumerable<FieldListNodeBase<XRDiagramControl>> DataSources {
			get { return dataSources; }
			private set { SetProperty(ref dataSources, value, () => DataSources); }
		}
		public bool HasDataBindings { get { return false; } }
		IEnumerable<ToolViewModel> CreateToolboxTools(XRDiagramControl diagram) {
			return LinqExtensions.Yield(new ToolViewModel(new XRPoinerTool(diagram), PointerIcon)).Concat(CreateToolboxToolsCore(diagram));
		}
		protected abstract IEnumerable<ToolViewModel> CreateToolboxToolsCore(XRDiagramControl diagram);
		protected virtual ImageSource PointerIcon { get { return DXImageHelper.GetImageSource(@"Images/Toolbox Items/Pointer_32x32.png"); } }
		#region Report Explorer
		public IEnumerable<IReportExplorerItem> ReportExplorerItems {
			get {
				return new IReportExplorerItem[] {
					this,
					styles,
					formattingRules,
					components,
				};
			}
		}
		sealed class ComponentGroup<T> : ImmutableObject, IReportExplorerContainerItem where T : class, IComponent {
			readonly XtraReportModelBase report;
			readonly ImageSource icon;
			readonly string name;
			readonly ObservableCollection<IReportExplorerItem> items;
			readonly IList<T> list;
			public ComponentGroup(XtraReportModelBase report, ImageSource icon, string name, IList<T> list) {
				this.report = report;
				this.list = list;
				this.icon = icon;
				this.name = name;
				this.items = new ObservableCollection<IReportExplorerItem>(list.Select(x => this.report.Factory.GetModel(x)));
			}
			public bool HasDataBindings { get { return false; } }
			public ImageSource Icon { get { return icon; } }
			public ObservableCollection<IReportExplorerItem> Items2 { get { return items; } }
			public IEnumerable<IReportExplorerItem> Items { get { return items; } }
			public string Name { get { return name; } }
			public string TypeString { get { return name; } }
			public bool TryAddItem(XRComponentModelBase component) {
				var item = component.XRObject as T;
				if(item == null) return false;
				int listIndex = list.IndexOf(item);
				var previousItem = listIndex == 0 ? null : report.Factory.GetModel(list[listIndex - 1]);
				int index = previousItem == null ? 0 : items.IndexOf(previousItem) + 1;
				items.Insert(index, component);
				return true;
			}
		}
		public IEnumerable<IReportExplorerItem> Items { get { return items; } }
		ObservableCollection<IReportExplorerItem> items;
		ComponentGroup<FormattingRule> formattingRules;
		ComponentGroup<XRControlStyle> styles;
		ComponentGroup<CalculatedField> calculatedFields;
		ComponentGroup<IComponent> components;
		ComponentGroup<Parameter> parameters;
		void UpdateReportExplorerItems(NotifyCollectionChangedEventArgs e) {
			if(e.Action == NotifyCollectionChangedAction.Reset) {
				items = new ObservableCollection<IReportExplorerItem>(Controls.OfType<XRControlModelBase>());
				formattingRules = new ComponentGroup<FormattingRule>(this, XRComponentModelBase.GetDefaultIcon("FormattingRuleContainer"), "Formatting Rules", XRObject.FormattingRuleSheet);
				styles = new ComponentGroup<XRControlStyle>(this, XRComponentModelBase.GetDefaultIcon("StyleContainer"), "Styles", ListAdapter<XRControlStyle>.FromObjectList(XRObject.StyleSheet));
				calculatedFields = new ComponentGroup<CalculatedField>(this, XRComponentModelBase.GetDefaultIcon("ComponentContainer"), "Calculated Fields", XRObject.CalculatedFields);
				parameters.Do(x => x.Items2.CollectionChanged -= OnComponentsCollectionChanged);
				parameters = new ComponentGroup<Parameter>(this, XRComponentModelBase.GetDefaultIcon("ComponentContainer"), "Parameters", XRObject.Parameters);
				parameters.Items2.CollectionChanged += OnComponentsCollectionChanged;
				components.Do(x => x.Items2.CollectionChanged -= OnComponentsCollectionChanged);
				components = new ComponentGroup<IComponent>(this, XRComponentModelBase.GetDefaultIcon("ComponentContainer"), "Components", XRObject.ComponentStorage);
				components.Items2.CollectionChanged += OnComponentsCollectionChanged;
				RaisePropertyChanged(() => ReportExplorerItems);
				UpdateDataSources();
				return;
			}
			if(items == null) return;
			switch(e.Action) {
			case NotifyCollectionChangedAction.Add:
				AddReportExplorerItems(e);
				break;
			case NotifyCollectionChangedAction.Remove:
				RemoveReportExplorerItems(e);
				break;
			case NotifyCollectionChangedAction.Replace:
				RemoveReportExplorerItems(e);
				AddReportExplorerItems(e);
				break;
			default: 
				throw new NotImplementedException();
			}
			UpdateDataSources();
		}
		void OnComponentsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateDataSources();
		}
		void AddReportExplorerItems(NotifyCollectionChangedEventArgs e) {
			foreach(XRComponentModelBase item in e.NewItems) {
				var control = item as XRControlModelBase;
				if(control != null) {
					items.Add(control);
				} else {
					bool added = parameters.TryAddItem(item) || formattingRules.TryAddItem(item) || styles.TryAddItem(item) || calculatedFields.TryAddItem(item) || components.TryAddItem(item);
					DebugHelper2.Assert(added, "AddReportExplorerItems");
				}
			}
		}
		void RemoveReportExplorerItems(NotifyCollectionChangedEventArgs e) {
			foreach(XRComponentModelBase item in e.OldItems) {
				var control = item as XRControlModelBase;
				if(control != null) {
					items.Remove(control);
				} else {
					parameters.Items2.Remove(item);
					formattingRules.Items2.Remove(item);
					styles.Items2.Remove(item);
					calculatedFields.Items2.Remove(item);
					components.Items2.Remove(item);
				}
			}
		}
		#endregion
		#region Bindings
		readonly List<DependencyProperty> diagramBindings = new List<DependencyProperty>();
		protected void BindDiagram<TProperty, TDiagramItemProperty>(DependencyProperty diagramProperty, Expression<Func<TProperty>> modelProperty, Func<TProperty, TDiagramItemProperty> convertMethod, Func<TDiagramItemProperty, TProperty> convertBackMethod = null) {
			BindDiagram(diagramProperty, modelProperty, this, convertMethod, convertBackMethod);
		}
		protected void BindDiagram<TProperty, TDiagramItemProperty>(DependencyProperty diagramProperty, Expression<Func<TProperty>> modelProperty, object source, Func<TProperty, TDiagramItemProperty> convertMethod, Func<TDiagramItemProperty, TProperty> convertBackMethod = null) {
			BindDiagram(diagramProperty, modelProperty, source, convertBackMethod == null ? BindingMode.OneWay : BindingMode.TwoWay, GenericValueConverter.Create(convertMethod, convertBackMethod));
		}
		protected void BindDiagram<TProperty>(DependencyProperty diagramProperty, Expression<Func<TProperty>> modelProperty, BindingMode mode = BindingMode.TwoWay, IValueConverter converter = null) {
			BindDiagram(diagramProperty, modelProperty, this, mode, converter);
		}
		protected void BindDiagram<TProperty>(DependencyProperty diagramProperty, Expression<Func<TProperty>> modelProperty, object source, BindingMode mode = BindingMode.TwoWay, IValueConverter converter = null) {
			var binding = new Binding(GetPropertyName(modelProperty)) { Source = source, Mode = mode, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Converter = converter };
			DiagramItem.Diagram.SetBinding(diagramProperty, binding);
			diagramBindings.Add(diagramProperty);
		}
		#endregion
	}
}
