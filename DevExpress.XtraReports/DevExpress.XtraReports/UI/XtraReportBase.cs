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
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using DevExpress.Data.Browsing;
using DevExpress.Data.Design;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Native.Printing;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraReports.Parameters;
using DevExpress.Data.Filtering;
using DevExpress.Data.Browsing.Design;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Data;
using DevExpress.XtraPrinting.Preview;
using System.Collections.Generic;
using DevExpress.XtraReports.Native.DrillDown;
using DevExpress.XtraReports.Native.Summary;
namespace DevExpress.XtraReports.UI {
	[
	ToolboxItem(false), 
	DesignTimeVisible(false),
	DefaultProperty("Bands"),
	DefaultCollectionName("Bands"),
	]
	abstract public class XtraReportBase : Band, IDataContainer, IFilterStringContainer, IDisplayNamePropertyContainer {
		#region static
		protected static BandFactory bandFactory = new BandFactory();
		internal static Type[] XRComponentTypes = new Type[] { typeof(XRControl), typeof(XRControlStyle), typeof(Parameter), typeof(CalculatedField) };
		public static Band CreateBand(BandKind bandKind) {
			return (bandFactory != null) ? bandFactory.CreateInstance(bandKind) : null;
		}
		protected static void CollectComponentsFromFields(object obj, UniqueItemList components) {
			if(obj.GetType().Equals(typeof(object)))
				return;
			FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
			foreach(FieldInfo field in fields) {
				if(typeof(IComponent).IsAssignableFrom(field.FieldType) && !XRComponentTypes.Any<Type>(item => item.IsAssignableFrom(field.FieldType))) {
					components.Add(field.GetValue(obj));
				}
			}
		}
		#endregion
		private object dataSource;
		private object xmlDataSource;
		object dataAdapter;
		string xmlDataPath = String.Empty;
		string fDataMember = ""; 
		private XRGroupCollection groups;
		int repeatIndexOnEmptyDataSource = -1;
		protected DevExpress.Data.Browsing.ListBrowser fDataBrowser;
		protected ReportDataContext fDataContext;
		private GroupFieldCollection sortFields;
		string filterString = string.Empty;
		ReportGroupsSynchronizer groupsSynchronizer;
		ReportPrintOptions reportPrintOptions;
		DetailReportCollection detailReportBands;
		protected bool suppressClearDataContext;
		#region Events
		private static readonly object BandHeightChangedEvent = new object();
		private static readonly object DataSourceRowChangedEvent = new object();
		private static readonly object DataSourceDemandedEvent = new object();
		public event EventHandler<EventArgs> DataSourceDemanded {
			add { Events.AddHandler(DataSourceDemandedEvent, value); }
			remove { Events.RemoveHandler(DataSourceDemandedEvent, value); }
		}
		protected virtual void OnDataSourceDemanded(EventArgs e) {
			RunEventScript(DataSourceDemandedEvent, EventNames.DataSourceDemanded, e);
			EventHandler<EventArgs> handler = (EventHandler<EventArgs>)Events[DataSourceDemandedEvent];
			if(handler != null) handler(this, e);
		}		
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportBaseBandHeightChanged")
#else
	Description("")
#endif
		]
		public event BandEventHandler BandHeightChanged {
			add { Events.AddHandler(BandHeightChangedEvent, value); }
			remove { Events.RemoveHandler(BandHeightChangedEvent, value); }
		}
		protected virtual void OnBandHeightChanged(BandEventArgs e) { 
			RunEventScript(BandHeightChangedEvent, EventNames.BandHeightChanged, e);
			BandEventHandler handler = (BandEventHandler)this.Events[BandHeightChangedEvent];
			if(handler != null) handler(this, e) ;
		}
		internal void RaiseBandHeightChanged(BandEventArgs e) {
			OnBandHeightChanged(e);
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XtraReportBaseDataSourceRowChanged")]
#endif
		public event DataSourceRowEventHandler DataSourceRowChanged {
			add { Events.AddHandler(DataSourceRowChangedEvent, value); }
			remove { Events.RemoveHandler(DataSourceRowChangedEvent, value); }
		}
		protected virtual void OnDataSourceRowChanged(DataSourceRowEventArgs e) { 
			RunEventScript(DataSourceRowChangedEvent, EventNames.DataSourceRowChanged, e);
			DataSourceRowEventHandler handler = (DataSourceRowEventHandler)this.Events[DataSourceRowChangedEvent];
			if(handler != null) handler(this, e) ;
		}
		void RaiseDataSourceRowChanged() {
			RootReport.Summaries.OnRowChangedOnReport(this);
			OnDataSourceRowChanged(new DataSourceRowEventArgs(DataBrowser.Position, DataBrowser.Count));		
		}
		#endregion
		internal DetailReportCollection DetailReportBands {
			get {
				if(detailReportBands == null)
					detailReportBands = new DetailReportCollection(this);
				return detailReportBands;
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public override SubBandCollection SubBands {
			get { return SubBandCollection.Empty; }
		}
		[
		SRCategory(ReportStringId.CatPrinting),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.ReportPrintOptions"),
		]
		public ReportPrintOptions ReportPrintOptions {
			get {
				if(reportPrintOptions == null)
					reportPrintOptions = new ReportPrintOptions();
				return reportPrintOptions;
			}
		}
		bool ShouldSerializeReportPrintOptions() {
			return reportPrintOptions != null && reportPrintOptions.ShouldSerialize();
		}
		void ResetReportPrintOptions() {
			reportPrintOptions = null;
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete("You should use the 'ReportPrintOptions.DetailCount' instead."),
		]
		public int DetailPrintCount { get { return ReportPrintOptions.DetailCount; } set { ReportPrintOptions.DetailCount = value; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete("You should use the 'ReportPrintOptions.DetailCountOnEmptyDataSource' instead."),
		]
		public int DetailPrintCountOnEmptyDataSource { get { return ReportPrintOptions.DetailCountOnEmptyDataSource; } set { ReportPrintOptions.DetailCountOnEmptyDataSource = value; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete("You should use the 'ReportPrintOptions.PrintOnEmptyDataSource' instead."),
		]
		public bool PrintOnEmptyDataSource { get { return ReportPrintOptions.PrintOnEmptyDataSource; } set { ReportPrintOptions.PrintOnEmptyDataSource = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportBaseScripts"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.Scripts"),
		SRCategory(ReportStringId.CatBehavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public new XtraReportBaseScripts Scripts { get { return (XtraReportBaseScripts)fEventScripts; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override PaddingInfo SnapLinePadding {
			get { return base.SnapLinePadding; }
			set { base.SnapLinePadding = value; }
		}
		internal int RepeatIndexOnEmptyDataSource { get { return repeatIndexOnEmptyDataSource; } set { repeatIndexOnEmptyDataSource = value; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override float HeightF {
			get { return base.HeightF; }
			set { base.HeightF = value; }
		}	
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override XRControlStyles Styles {
			get { return base.Styles; }
		}
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override StylePriority StylePriority {
			get { return base.StylePriority; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportBaseStyleName"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string StyleName { get { return ""; } set {} }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportBaseEvenStyleName"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string EvenStyleName { get { return ""; } set {} }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportBaseOddStyleName"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OddStyleName { get { return ""; } set {} }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportBaseFilterString"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReportBase.FilterString"),
		DefaultValue(""),
		SRCategory(ReportStringId.CatData),
		Editor("DevExpress.XtraReports.Design.FilterStringEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty,
		TypeConverter(typeof(DevExpress.XtraReports.Design.TextPropertyTypeConverter)),
		]
		public virtual string FilterString {
			get { return filterString; }
			set {
				string newValue = DevExpress.XtraReports.Native.Parameters.ParametersReplacer.UpgradeFilterString(value);
				if(filterString != newValue) {
					filterString = newValue;
					SetFilterString(fDataBrowser, filterString);
				}
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportBaseDataMember"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReportBase.DataMember"),
		SRCategory(ReportStringId.CatData),
		DefaultValue(""),
		TypeConverter(typeof(DevExpress.XtraReports.Design.DataMemberTypeConverter)),
		Editor("DevExpress.XtraReports.Design.ReportDataMemberEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(UITypeEditor)),
		XtraSerializableProperty,
		]
		public string DataMember {
			get { return fDataMember ?? string.Empty; }
			set { fDataMember = value ?? string.Empty; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportBaseDataSource"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReportBase.DataSource"),
		SRCategory(ReportStringId.CatData),
		RefreshProperties(RefreshProperties.Repaint),
		Editor("DevExpress.XtraReports.Design.DataSourceEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DataSourceConverter)),
		XtraSerializableProperty(XtraSerializationVisibility.Reference),
		]
		public object DataSource {
			get { return dataSource; }
			set {
				if(dataSource == value)
					return;
				if(ReportIsLoading)
					dataSource = value;
				else {
					OnDataSourceChanging();
					object oldValue = dataSource;
					dataSource = value;
					OnDataSourceChanged(oldValue);
				}
			}
		}
		bool ShouldSerializeDataSource() {
			return dataSource is IComponent;
		}
		bool ShouldSerializeXmlDataSource() {
			return dataSource != null;
		}
		internal IDataAdapter DataAdapterInternal { get { return DevExpress.Data.Native.BindingHelper.ConvertToIDataAdapter(dataAdapter); } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportBaseDataAdapter"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReportBase.DataAdapter"),
		SRCategory(ReportStringId.CatData),
		Editor("DevExpress.XtraReports.Design.DataAdapterEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverterAttribute(typeof(DevExpress.XtraReports.Design.DataAdapterConverter)),
		XtraSerializableProperty(XtraSerializationVisibility.Reference),
		]
		public object DataAdapter { get { return dataAdapter; } set { dataAdapter = value; } }
		bool ShouldSerializeDataAdapter() {
			return dataAdapter is IComponent;
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportBaseXmlDataPath"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReportBase.XmlDataPath"),
		SRCategory(ReportStringId.CatData),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.XmlFileNameEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		RefreshProperties(RefreshProperties.Repaint),
		XtraSerializableProperty,
		]
		public virtual string XmlDataPath {
			get { return xmlDataPath; }
			set {
				if(xmlDataPath != value) {
					xmlDataPath = value;
					object oldSource = xmlDataSource;
					xmlDataSource = System.IO.File.Exists(xmlDataPath) ?
						XmlDataHelper.CreateDataSetByXmlUrl(xmlDataPath, false) : null;
					if(xmlDataSource != null && oldSource != null && Object.Equals(oldSource, DataSource))
						DataSource = xmlDataSource;
				}
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public new XRControlCollection Controls { get { return base.Controls; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public int CurrentRowIndex { 
			get { 
				DataBrowser dataBrowser = DataBrowser;
				return dataBrowser == null ? -1 : dataBrowser.Position;
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public int RowCount {
			get {
				DataBrowser dataBrowser = DataBrowser;
				return dataBrowser == null ? 0 : dataBrowser.Count;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportBaseBands"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReportBase.Bands"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraReports.Design.BandCollectionEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		SRCategory(ReportStringId.CatStructure),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, -1, XtraSerializationFlags.Cached | XtraSerializationFlags.DeserializeCollectionItemBeforeCallSetIndex),
		]
		public BandCollection Bands {
			get { return (BandCollection)fXRControls; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override bool KeepTogether { get { return false; } set {} }
		internal DevExpress.Data.Browsing.ListBrowser DataBrowser {
			get {
				if((fDataBrowser == null || fDataBrowser.IsClosed) && DataContext != null) {
					fDataBrowser = DataContext[DataSource, fDataMember] as DevExpress.Data.Browsing.ListBrowser;
					if(fDataBrowser == null)
						throw new Exception("An object assigned to the DataSource property cannot be used as a report's datasource, because it does not implement any of supported interfaces.\r\nFor more information, refer to http://help.devexpress.com/#XtraReports/CustomDocument1179");
					InitializeSortedController();
					fDataBrowser.RaiseCurrentChanged();
				}
				return fDataBrowser;
			} 
		}
		internal DataController DataController {
			get {
				return DataBrowser != null && DataBrowser.ListController is  SortedListController ?
					((SortedListController)DataBrowser.ListController).DataController : null;
			}
		}
		protected virtual bool SuppressListFillingInDataContext {
			get { return DesignMode || ReportIsLoading; }
		}
		void InitializeSortedController() {
			SortedListController sortedListController = fDataBrowser.ListController as SortedListController;
			if(sortedListController != null) {
				DetailBand detailBand = Bands[BandKind.Detail] as DetailBand;
				sortedListController.Initialize(ReportCalculatedFields, CollectGroupFields(Groups, detailBand), CollectGroupSortingSummary(Groups), GetFilterCriteria(filterString));
			}
		}
		internal static GroupField[] CollectGroupFields(XRGroupCollection groups, DetailBand detailBand) {
			List<GroupField> groupFields = new List<GroupField>(GroupConverter.GetGroupFields(groups));
			if(detailBand != null && detailBand.SortFields != null)
				groupFields.AddRange(detailBand.SortFields);
			return groupFields.ToArray();
		}
		internal static XRGroupSortingSummary[] CollectGroupSortingSummary(XRGroupCollection groups) {
			return GroupConverter.GetSortingSummary(groups);
		}
		internal XRGroupCollection Groups { 
			get {
				if(groups == null)
					groups = new XRGroupCollection(this.Bands);
				return groups;
			}
		}
		protected internal virtual int DisplayableRowCount {
			get {
				return  ReportPrintOptions.ActualDetailCount > 0 ? 
					Math.Min(ReportPrintOptions.ActualDetailCount, DataBrowser.Count) : 
					DataBrowser.Count;
			}
		}
		internal ReportDataContext DataContext { 
			get { 
				if(this.fDataContext == null)
					this.fDataContext = GetDataContext();
				return fDataContext;
			} 
		}
		protected internal override GroupFieldCollection SortFieldsInternal { get { return sortFields; } }
		protected XtraReportBase()  { 
			sortFields = new GroupFieldCollection(this);
			groupsSynchronizer = new ReportGroupsSynchronizer(this);
		}
		protected override XRControlScripts CreateScripts() {
			return new XtraReportBaseScripts(this);
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
				ClearDataContext();
			base.Dispose(disposing);
		}
		public void ApplyFiltering() {
			SetFilterString(fDataBrowser, this.FilterString);
		}
		void SetFilterString(DevExpress.Data.Browsing.ListBrowser dataBrowser, string filterString) {
			if(dataBrowser == null) 
				return;
			CriteriaOperator filterCriteria = GetFilterCriteria(filterString);
			if(ReferenceEquals(filterCriteria, null))
				return;
			SortedListController listController = dataBrowser.ListController as SortedListController;
			if(listController != null) {
				listController.FilterCriteria = filterCriteria;
				dataBrowser.RaiseCurrentChanged();
			}
			RootReport.PrintingSystem.Images.ResetHash();
		}
		protected CriteriaOperator GetFilterCriteria(string filterString) {		   
			try {
				CriteriaOperator result = CriteriaOperator.Parse(filterString);
				if(!ReferenceEquals(result, null)) {
					CriteriaOperator processedResult = ProcessCriteriaOperator(result);
					DevExpress.XtraReports.Native.Parameters.ParametersValueSetter.Process(processedResult, RootReport.Parameters);
					return processedResult;
				}
			} catch {
			}
			return null;
		}
		CriteriaOperator ProcessCriteriaOperator(CriteriaOperator criteriaOperator) {
			if(this.RootReport != null) {
				using(DataContext dataContext = new XRDataContext(RootReport.CalculatedFields, true)) {
					DeserializationFilterStringVisitor visitor = new DeserializationFilterStringVisitor(RootReport, dataContext, DataSource, DataMember);
					return (CriteriaOperator)criteriaOperator.Accept(visitor);
				}
			}
			return criteriaOperator;
		}
		bool HasData(object dataSource, string dataMember) {
			using( XRDataContext dataContext = new XRDataContext(null, true) ) {
				return dataContext.DataEqual(this.dataSource,this.DataMember,dataSource,dataMember);
			}
		}
		static bool DataHaveRelation(object dataSource, string dataMember, object relatedDataSource, string relatedDataMember) {
			using(XRDataContext dataContext = new XRDataContext(null, true)) {
				return dataContext.DataHaveRelation(dataSource, dataMember, relatedDataSource, relatedDataMember);
			}
		}
		ReportDataContext GetDataContext() {
			XtraReportBase parent = (XtraReportBase)Parent;
			while(parent != null) {
				if(parent.HasData(dataSource, fDataMember) || DataHaveRelation(dataSource, fDataMember, parent.dataSource, parent.DataMember))
					return CreateDataContext(this);
				if(parent.DataContext.HasOwner(parent))
					return parent.DataContext;
				parent = (XtraReportBase)parent.Parent;
			}
			return CreateDataContext(this);
		}
		internal ReportDataContext GetChildDataContext(object owner, object dataSource, string dataMember) {
			XtraReportBase parent = this;
			while(parent != null) {
				if(parent.DataContext.HasOwner(parent) && (
						dataSource == null ||
						parent.HasData(dataSource, dataMember) ||
						DataHaveRelation(parent.dataSource, parent.DataMember, dataSource, dataMember)
					))
					return parent.DataContext;
				parent = (XtraReportBase)parent.Parent;
			}
			return CreateDataContext(owner);
		}
		ReportDataContext CreateDataContext(object owner) {
			return new ReportDataContext(RootReport, owner, this.ReportCalculatedFields, SuppressListFillingInDataContext);
		} 
		protected virtual void ClearDataContext() {
			if(fDataContext != null && fDataContext.HasOwner(this))
				fDataContext.Dispose();
			fDataContext = null;
			fDataBrowser = null;
		}
		protected virtual bool GetSuppressClearDataContext() {
			return suppressClearDataContext;
		}
		#region Serialization
		protected override void SerializeProperties(XRSerializer serializer) {
			base.SerializeProperties(serializer);
			serializer.SerializeReference("DataAdapter", dataAdapter); 
			serializer.SerializeString("DataMember", DataMember, "");
			serializer.SerializeDataSource("DataSource", dataSource, null);
			serializer.SerializeString("XMLDataPath", xmlDataPath, String.Empty);
		}
		protected override void DeserializeProperties(XRSerializer serializer) {
			base.DeserializeProperties(serializer);
			object da = serializer.DeserializeReference("DataAdapter", null); 
			if(da != null)
				dataAdapter = da;
			fDataMember = serializer.DeserializeString("DataMember", "");
			object ds = serializer.DeserializeDataSource("DataSource", null);
			if(ds != null)
				dataSource = ds;
			XmlDataPath = serializer.DeserializeString("XMLDataPath", String.Empty);
		}
		protected override void CollectAssociatedComponents(DesignItemList components) {
			base.CollectAssociatedComponents(components);
			components.AddDataAdapter(dataAdapter);
			object dataSource = ((IDataContainer)this).GetSerializableDataSource();
			if(dataSource != xmlDataSource)
				components.AddDataSource(dataSource as IComponent);
		}
		#endregion
		object IDataContainer.GetEffectiveDataSource() {
			return this.GetEffectiveDataSource();
		}
		object IDataContainer.GetSerializableDataSource() {
			return DataSourceHelper.ConvertToSerializableDataSource(dataSource);
		}
#if DEBUGTEST
		public
#else	
		internal
#endif
		virtual object GetEffectiveDataSource() {
			return dataSource != null ? dataSource : 
				xmlDataSource != null ? xmlDataSource :
				Parent != null ? ((XtraReportBase)Parent).GetEffectiveDataSource() :
				null;
		}
		internal int GetEffectiveRowIndex() {
			return repeatIndexOnEmptyDataSource >= 0 ? repeatIndexOnEmptyDataSource : 
				(ReportIsLoading || DesignMode) ? 0 :
				DataBrowser.Position;
		}
		protected override XRControlCollection CreateChildControls() {
			return new BandCollection(this);
		}
		protected override void OnControlCollectionChanged(object sender, CollectionChangeEventArgs e) {
			if(RootReport != null) {
				RootReport.AllBands.Clear();
				if(!Report.ReportIsLoading)
					groupsSynchronizer.HandleCollectionChanged(e);
			}
		}
		protected virtual void OnDataSourceChanging() {
			AdjustDataSource();
		}
		void OnDataSourceChanged(object oldValue) {
			fDataBrowser = null;
			ValidateDataMember(DataSource);
			if(DataSource != null) {
				NestedComponentEnumerator compEnum = new NestedComponentEnumerator(Bands);
				while(compEnum.MoveNext())
					compEnum.Current.ValidateDataSource(DataSource, oldValue, DataMember);
			}
			RootReport.ChildDataSourceChanged();
		}
		private void ValidateDataMember(object dataSource) {
			fDataMember = XRDataContext.ValidateDataMember(dataSource, fDataMember);
		}
		public object GetPreviousRow() {
			return DataBrowser.GetRow(DataBrowser.Position - 1);
		}
		public object GetNextRow() {
			return DataBrowser.GetRow(DataBrowser.Position + 1);
		}
		public object GetCurrentRow() {
			return DataBrowser.Current;
		}
		public object GetPreviousColumnValue(string columnName) {
			return DataBrowser.GetColumnValue(DataBrowser.Position - 1, columnName);
		}
		public object GetNextColumnValue(string columnName) {
			return DataBrowser.GetColumnValue(DataBrowser.Position + 1, columnName);
		}
		public object GetCurrentColumnValue(string columnName) {
			return DataBrowser.GetColumnValue(DataBrowser.Position, columnName);
		}
		public string FromDisplayName(string displayName) {
			string dataMember = GetDataMemberFromDisplayName(displayName);
			if(string.IsNullOrEmpty(dataMember))
				return null;
			return dataMember.Split('.').Last();
		}
		public T GetPreviousColumnValue<T>(string columnName) {
			object value = GetPreviousColumnValue(columnName);
			return value != null ? (T)value : default(T);
		}
		public T GetNextColumnValue<T>(string columnName) {
			object value = GetNextColumnValue(columnName);
			return value != null ? (T)value : default(T);
		}
		public T GetCurrentColumnValue<T>(string columnName) {
			object value = GetCurrentColumnValue(columnName);
			return value != null ? (T)value : default(T);
		}
		protected internal override void CopyDataProperties(XRControl control) {
			base.CopyDataProperties(control);
			XtraReportBase report = control as XtraReportBase;
			if(report != null) {
				DataMember = report.DataMember;
				DataSource = report.DataSource;
				DataAdapter = report.DataAdapter;
			}
		}
		protected internal virtual bool EndOfData() {
			return DataBrowser.Position >= DisplayableRowCount - 1;
		}
		protected internal override bool CanAddControl(Type componentType, XRControl control) {
			return Controls.CanAdd(componentType);
		}
		protected internal virtual PrintingSystemBase GetPrintingSystem() {
			throw new MethodAccessException();
		}
		protected void BuildDocument(DocumentBuilderBase builder) {
			DataBrowser.Position = 0; 
			RootReport.Summaries.ResetCalculation(this);
			if(DataBrowser.Count > 0)
				RaiseDataSourceRowChanged();
			try {
				builder.Build();
				InsertPageBreaks(builder.RootBand);
			}
			finally {
			}
			DetailBand detail = Bands[BandKind.Detail] as DetailBand;
			if(detail != null)
				detail.ApplyMultiColumn(builder.RootBand, XRConvert.Convert(RootReport.GetClientSize().Width, Dpi, GraphicsDpi.Document));
		}
		protected void FillDataSources(bool skipIfFilled) {
			NestedComponentEnumerator en = new NestedComponentEnumerator(
				new IComponent[] { this }, 
				control => control is XtraReportBase ? control.Controls : null
				);
			while(en.MoveNext()) {
				XtraReportBase reportBase = en.Current as XtraReportBase;
				if(reportBase != null)
					reportBase.PrepareDataSource();
			}
			bool isCancellationRequested = false;
			ICancellationService serv = ((IServiceProvider)RootReport).GetService(typeof(ICancellationService)) as ICancellationService;
			serv.TryRegister(() => {
				isCancellationRequested = true;
			}, true);
			UniqueDataContainerEnumerator enumerator = new UniqueDataContainerEnumerator();
			foreach(IDataContainerBase item in enumerator.EnumerateDataContainers(RootReport, false)) {
				DataSourceFiller filler = DataSourceFiller.CreateInstance(item, RootReport, skipIfFilled);
				if(filler != null) filler.Execute();
				if(IsDisposed) throw new ReportCanceledException(string.Format("'{0}' report generation was canceled", !string.IsNullOrEmpty(Name) ? Name : "Unknown"));
				if(isCancellationRequested) break;
			}
		}
		void PrepareDataSource() {
			OnDataSourceDemanded(EventArgs.Empty);
			ClearDataContext();
			if(DataSource == null)
				DataSource = xmlDataSource;
		}
		public void FillDataSource() {
			PrepareDataSource();
			DataSourceFiller filler = DataSourceFiller.CreateInstance(this, RootReport, false);
			if(filler != null) filler.Execute();
		}
		protected override void BeforeReportPrint() {
			DetailBand detailBand = Bands[BandKind.Detail] as DetailBand;
			if (detailBand != null && detailBand.SortFields.Count == 0)
				detailBand.SortFields.AddRange((GroupField[])new ArrayList(sortFields).ToArray(typeof(GroupField)));
			base.BeforeReportPrint();
		}
		internal void MoveNextRow() {
			if(!EndOfData()) {
				DataBrowser.Position++;
				RaiseDataSourceRowChanged();
			}
		}
		internal void OnGroupFinished(XRGroup group) {
			RootReport.Summaries.OnGroupFinished(this, group);
		}
		internal void OnGroupBegin(XRGroup group) {
			RootReport.Summaries.OnGroupBegin(this, group);
		}
		internal void CollectAllReports(System.Collections.Generic.List<XtraReportBase> allReports) {
			allReports.Add(this);
			foreach(Band band in this.Bands) {
				if(band is XtraReportBase)
					((XtraReportBase)band).CollectAllReports(allReports);
			}
		}
		public override IEnumerable<Band> OrderedBands {
			get {
				return Bands.OrderBy<Band, int>(item => item.GetWeightingFactor());
			}
		}
		internal void WriteToDocument(DocumentBuilderBase docBuilder) {
			BuildDocument(docBuilder);
		}
		protected override void AfterReportPrint() {
			if(!GetSuppressClearDataContext()) {
				ClearDataContext();
			}
			base.AfterReportPrint();
		}
		protected override void GenerateContent(DocumentBand docBand, int rowIndex, bool fireBeforePrint) {
		}
		protected internal override object SetDataSource(object dataSource) {
			return null;
		}
		internal bool CanBandBeMoved(DetailReportBand band, BandReorderDirection direction) {
			return FindDetailReportBand(band.Level, direction) != null;
		}
		internal void MoveBand(DetailReportBand band, BandReorderDirection direction) {
			DetailReportBand foundBand = FindDetailReportBand(band.Level, direction);
			if(foundBand != null) {
				band.Level = foundBand.Level;
			}
		}
		DetailReportBand FindDetailReportBand(int currentBandLevel, BandReorderDirection direction) {
			int level = currentBandLevel + (direction == BandReorderDirection.Up ? -1 : 1);
			return DetailReportBands.GetByLevel(level);
		}
		internal string GetShortFieldDisplayName(string fieldName) {
			if(string.IsNullOrEmpty(fieldName))
				return fieldName;
			string dataMember = EmbeddedFieldsHelper.GetDataMember(DataMember, fieldName);
			return GetFieldDisplayName(dataMember);
		}
		internal string GetFieldDisplayName(string fieldName) {
			return GetFieldDisplayName(DataMember, fieldName);
		}
		string GetFieldDisplayName(string dataMember, string fieldName) {
			return EmbeddedFieldsHelper.GetFieldDisplayName(this, dataMember, GetEffectiveDataSource(), fieldName);
		}
		internal string GetDataMemberFromDisplayName(string displayName) {
			using(DataContextProvider provider = new DataContextProvider(this)) {
				string result = provider.DataContext.GetDataMemberFromDisplayName(GetEffectiveDataSource(), DataMember, displayName);
				if(string.IsNullOrEmpty(result) && !string.IsNullOrEmpty(displayName) && displayName.IndexOf(".") >= 0)
					result = provider.DataContext.GetDataMemberFromDisplayName(RootReport.GetEffectiveDataSource(), "", displayName);
				return result;
			}
		}
		internal bool IsFieldNameValid(string fieldName) {
			return EmbeddedFieldsHelper.IsFieldNameValid(this, fieldName, GetEffectiveDataSource(), DataMember);
		}
		protected internal override void OnReportInitialize() {
			base.OnReportInitialize();
			groups = null;
			groupsSynchronizer.EnforceGroupsUpdate();
			DetailReportBands.Update();
		}
#if !DEBUGTEST
		[EditorBrowsable(EditorBrowsableState.Never)]
#endif
		public void IterateReportsRecursive(Action<XtraReportBase> action) {
			action(this);
			foreach(DetailReportBand detailReport in DetailReportBands)
				detailReport.IterateReportsRecursive(action);
		}
		protected override object CreateCollectionItem(string propertyName, DevExpress.Utils.Serializing.XtraItemEventArgs e) {
			if(propertyName == XtraReportsSerializationNames.Bands)
				return CreateControl(e);
			return base.CreateCollectionItem(propertyName, e);
		}
		protected override void SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			if(propertyName == XtraReportsSerializationNames.Bands)
				Bands.Add(e.Item.Value as Band);
			else 
				base.SetIndexCollectionItem(propertyName, e);
		}
		#region IDisplayNamePropertyContainer Members
		string IDisplayNamePropertyContainer.GetDisplayPropertyValue(string source) {
			return CriteriaFieldNameConverter.ToDisplayNames(this, GetEffectiveDataSource(), DataMember, source);
		}
		string IDisplayNamePropertyContainer.GetRealPropertyValue(string source) {
			return CriteriaFieldNameConverter.ToRealNames(this, GetEffectiveDataSource(), DataMember, source);
		}
		string IDisplayNamePropertyContainer.GetDisplayName(string source) {
			return CriteriaFieldNameConverter.ToDisplayName(this, GetEffectiveDataSource(), DataMember, source);
		}
		bool IDisplayNamePropertyContainer.CanSetPropertyValue {
			get { return true; }
		}
		#endregion
	}
	[
	XRDesigner("DevExpress.XtraReports.Design.DetailReportBandDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design._DetailReportBandDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.DetailReportBand", "DetailReport"), 
	BandKind(BandKind.DetailReport),
	]
	public class DetailReportBand : XtraReportBase, IMoveableBand, IDrillDownNode {
		const int defaultLevel = -1;
		#region Fields & Properties
		int level = defaultLevel;
		DrillDownHelper drillDownHelper;
		internal override bool DrillDownExpandedInternal {
			get {
				return DrillDownExpanded;
			}
			set {
				DrillDownExpanded = value;
			}
		}
		[
		SRCategory(ReportStringId.CatBehavior),
		DefaultValue(true),
		TypeConverter(typeof(BooleanTypeConverter)),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.Band.DrillDownExpanded"),
		XtraSerializableProperty,
		]
		public bool DrillDownExpanded {
			get;
			set;
		}
		[
		SRCategory(ReportStringId.CatBehavior),
		DefaultValue(null),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.Band.DrillDownControl"),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlReferenceConverter)),
		Editor("DevExpress.XtraReports.Design.DrillDownReportEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty(XtraSerializationVisibility.Reference),
		]
		public XRControl DrillDownControl {
			get;
			set;
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("DetailReportBandLevel"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.DetailReportBand.Level"),
		DefaultValue(defaultLevel),
		SRCategory(ReportStringId.CatBehavior),
		XtraSerializableProperty,
		]
		public int Level { 
			get { return level; }
			set {
				if(ReportIsLoading)
					level = value;
				else
					Report.DetailReportBands.AssignLevel(this, value);
			}
		}
		#endregion
		public DetailReportBand() {
			weightingFactor = DetailReportWeight;
		}
		protected override void BeforeReportPrint() {
			base.BeforeReportPrint();
			NullDrillDownHelper();
			drillDownHelper = DrillDownHelper.CreateInstance(this, RootReport);
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
				NullDrillDownHelper();
			base.Dispose(disposing);
		}
		void NullDrillDownHelper() {
			if(drillDownHelper != null) {
				drillDownHelper.Dispose();
				drillDownHelper = null;
			}
		}
		protected override XRControlCollection CreateChildControls() {
			return new DetailReportBandCollection(this);
		}
		protected internal override int GetWeightingFactor() {
			return base.GetWeightingFactor() + level;
		}
		protected internal override PrintingSystemBase GetPrintingSystem() {
			return RootReport != null ? RootReport.PrintingSystem : null;
		}
		protected internal override void OnBeforePrint(PrintEventArgs e) {
			base.OnBeforePrint(e);
			if(e.Cancel || DesignMode) return;
			UpdateSortedController();
			IDrillDownService serv = RootReport.GetService<IDrillDownService>();
			if((serv != null && !serv.BandExpanded(this)) || (!ReportPrintOptions.PrintOnEmptyDataSource && DataBrowser.Count == 0))
				e.Cancel = true;
		}
		void UpdateSortedController() {
			SortedListController sortedListController = DataBrowser.ListController as SortedListController;
			if(sortedListController != null) {
				DetailBand detailBand = Bands[BandKind.Detail] as DetailBand;
				sortedListController.UpdateDataController(ReportCalculatedFields, CollectGroupFields(Groups, detailBand), CollectGroupSortingSummary(Groups), GetFilterCriteria(this.FilterString));
			}
		}
		protected internal override void ValidateDataSource(object newSource, object oldSource, string dataMember) {
			if(newSource != null && (DataSource == null || Object.Equals(DataSource, oldSource)))  
				DataSource = newSource;
		}
		#region IMoveableBand
		bool IMoveableBand.CanBeMoved(BandReorderDirection direction) {
			return Report.CanBandBeMoved(this, direction); 
		}
		void IMoveableBand.Move(BandReorderDirection direction) {
			Report.MoveBand(this, direction); 
		}
		#endregion
		internal void SetLevelCore(int level) {
			this.level = level;
		}
	}
}
