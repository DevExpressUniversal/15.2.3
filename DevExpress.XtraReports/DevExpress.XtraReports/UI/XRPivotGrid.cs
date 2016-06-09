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

using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraReports;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Serialization;
using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid;
using DevExpress.Data.Design;
using DevExpress.XtraReports.Design;
using System.Drawing.Design;
using DevExpress.Data.Browsing;
using System.Collections;
using System.Collections.Generic;
using DevExpress.PivotGrid.Printing;
using DevExpress.XtraReports.Native.Printing;
using DevExpress.Utils;
using System.Drawing.Drawing2D;
using DevExpress.Utils.Controls;
using DevExpress.XtraReports.Native.Data;
using DevExpress.Utils.Serializing;
using DevExpress.Data.PivotGrid;
using DevExpress.Utils.Serializing.Helpers;
using System.IO;
using DevExpress.Data;
using DevExpress.XtraReports.Native.CalculatedFields;
namespace DevExpress.XtraReports.UI {
	using DevExpress.XtraReports.UI.PivotGrid;
	using System.Drawing.Printing;
	using System.IO;
	using DevExpress.XtraReports.UserDesigner;
	using DevExpress.XtraPrintingLinks;
	using DevExpress.Data.ChartDataSources;
	using DevExpress.Charts.Native;
	using System.Windows.Forms;
	using System.ComponentModel.Design;
	using DevExpress.XtraReports.Native.Presenters;
	using DevExpress.XtraReports.Native.LayoutView;
	using DevExpress.Utils.Design;
	[
	XRDesigner("DevExpress.XtraReports.Design.XRPivotGridDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design._XRPivotGridDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "XRPivotGrid.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRPivotGrid", "PivotGrid"),
	ToolboxItem(true),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabReportControls),
	XRToolboxSubcategoryAttribute(2, 3),
	ToolboxBitmap24("DevExpress.XtraReports.Images.Toolbox24x24.XRPivotGrid.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	ToolboxBitmap32("DevExpress.XtraReports.Images.Toolbox32x32.XRPivotGrid.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	]
	public class XRPivotGrid : XRControl, IDataContainer, IPivotGridEventsImplementorBase, IPivotGridPrinterOwner,
		IComponentLoading, IBindingList, ITypedList, IPivotGrid, IPivotGridDataContainer, 
		IXtraSerializableLayoutEx, IXtraSupportDeserializeCollection {
		protected const int LayoutIdAppearance = 1, LayoutIdData = 2, LayoutIdLayout = 3, LayoutIdOptionsView = 4, LayoutIdColumns = 5;
		#region inner classes
		new public static class EventNames {
			public const string CustomCellDisplayText = "CustomCellDisplayText";
			public const string CustomCellValue = "CustomCellValue";
			public const string CustomColumnWidth = "CustomColumnWidth";
			public const string CustomFieldSort = "CustomFieldSort";
			public const string CustomServerModeSort = "CustomServerModeSort";
			public const string CustomFieldValueCells = "CustomFieldValueCells";
			public const string CustomGroupInterval = "CustomGroupInterval";
			public const string CustomChartDataSourceData = "CustomChartDataSourceData";
			public const string CustomChartDataSourceRows = "CustomChartDataSourceRows";
			public const string CustomRowHeight = "CustomRowHeight";
			public const string CustomSummary = "CustomSummary";
			public const string CustomUnboundFieldData = "CustomUnboundFieldData";
			public const string EvaluateBinding = "EvaluateBinding";
			public const string FieldValueDisplayText = "FieldValueDisplayText";
			public const string PrefilterCriteriaChanged = "PrefilterCriteriaChanged";
			public const string PrintCell = "PrintCell";
			public const string PrintFieldValue = "PrintFieldValue";
			public const string PrintHeader = "PrintHeader";
		}
		class XRPivotGridPrinter : PivotGridWebPrinter, IPrintable {
			public static XRPivotGridPrinter CreateInstance(XRPivotGrid xrPivotGrid) {
				return xrPivotGrid.DesignMode ? new XRPivotGridDesignPrinter(xrPivotGrid) :
					new XRPivotGridPrinter(xrPivotGrid);
			} 
			public virtual bool DesignMode {
				get { return false; }
			}
			public XRPivotGridPrinter(XRPivotGrid xrPivotGrid) 
				: base(xrPivotGrid, xrPivotGrid.Data, xrPivotGrid.AppearancePrint) {
			}
			protected override PivotPrintBestFitter CreatePivotPrintBestFitter() {
				return new PivotPrintBestFitter(Data, this, new XRPrintCellSizeProvider(Data, Data.VisualItems, this));
			}
#region IPrintable Members
			void IPrintable.AcceptChanges() {
				base.AcceptChanges();
			}
			bool IPrintable.CreatesIntersectedBricks {
				get { return false; }
			}
			bool IPrintable.HasPropertyEditor() {
				return false;
			}
			UserControl IPrintable.PropertyEditorControl {
				get { return null; }
			}
			void IPrintable.RejectChanges() {
				base.RejectChanges();
			}
			void IPrintable.ShowHelp() {
			}
			bool IPrintable.SupportsHelp() {
				return false;
			}
			#endregion
			#region IBasePrintable Members
			void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
				base.CreateArea(areaName, graph);
			}
			void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) { }
			void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
				base.Initialize(ps, link);
			}
			#endregion
			internal int GetRowAreaY() {
				return RowAreaY;
			}
		}
		class XRPrintCellSizeProvider : PrintCellSizeProvider {
			public XRPrintCellSizeProvider(PivotGridData data, PivotVisualItemsBase visualItems, PivotGridPrinterBase printer)
				: base(data, visualItems, printer) {
			}
			protected override int GetRowFieldHeight(PivotFieldValueItem item) {
				int height = base.GetRowFieldHeight(item);
				if(item != null) {
					PivotCustomRowHeightEventArgs args = new PivotCustomRowHeightEventArgs(item, height);
					((XRPivotGrid)Data.EventsImplementor).RaiseCustomRowHeight(args);
					return args.RowHeight;
				}
				return height;
			}
			protected override int GetColumnFieldWidth(PivotFieldValueItem item) {
				int width = base.GetColumnFieldWidth(item);
				if(item != null) {
					PivotCustomColumnWidthEventArgs args = new PivotCustomColumnWidthEventArgs(item, width);
					((XRPivotGrid)Data.EventsImplementor).RaiseCustomColumnWidth(args);
					return args.ColumnWidth;
				}
				return width;
			}
		}
		class XRPivotGridDesignPrinter : XRPivotGridPrinter {
			public override bool DesignMode {
				get { return true; }
			}
			public XRPivotGridDesignPrinter(XRPivotGrid xrPivotGrid)
				: base(xrPivotGrid) {
			}
			protected override IVisualBrick DrawHeaderBrick(PivotFieldItemBase field, Rectangle bounds) {
				IVisualBrick brick = base.DrawHeaderBrick(field, bounds);
				XRPivotGridField xrField = this.Data.GetField(field) as XRPivotGridField;
				brick.Value = xrField;
				return brick;
			}
			protected override ITextBrick CreateTextBrick() {
				return new DesignPivotBrick();
			}
		}
		class XRPivotGridComponentLink : PrintableComponentLinkBase {
			XRPivotGridPrinter printer;
			public XRPivotGridPrinter Printer {
				get { return printer; }
			}
			public XRPivotGridComponentLink(XRPivotGridPrinter printer) {
				this.printer = printer;
				this.SetDataObject(printer);
			}
			protected override void ApplyPageSettings() {
			}
		}
		class BestFitHelper {
			ArrayList objects = new ArrayList();
			public void ApplyBestFit(object obj) {
				if(!objects.Contains(obj))
					objects.Add(obj);
			}
			public void BestFit(object obj, PivotGridBestFitterBase bestFitter) {
				if(obj is XRPivotGridData) {
					bestFitter.BestFit();
				} else if(obj is XRPivotGridField) {
					XRPivotGridField field = obj as XRPivotGridField;
					bestFitter.BestFit(((IPivotGridDataContainer)field).Data.GetFieldItem(field));
				}
			}
			public void EnsureBestFit(PivotGridBestFitterBase bestFitter) {
				foreach(object obj in objects)
					BestFit(obj, bestFitter);
				objects.Clear();
			}
		}
		class BindingListHelper : IDisposable {
			IBindingList bindingList;
			bool wasChanged;
			public void CheckListChanged(IList list, Action action) {
				if(ReferenceEquals(this.bindingList, list) && wasChanged)
					action();
				wasChanged = false;
				if(bindingList != null)
					bindingList.ListChanged -= OnListChanged;
				bindingList = list as IBindingList;
				if(bindingList != null)
					bindingList.ListChanged += OnListChanged;
			}
			void OnListChanged(object sender, ListChangedEventArgs e) {
				wasChanged = true;
			}
			public void Dispose() {
				if(bindingList != null) {
					bindingList.ListChanged -= OnListChanged;
					bindingList = null;
				}
			}
		}
		#endregion
		#region hidden properties
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override bool CanPublish { get { return true; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public new Font Font {
			get { return base.Font; }
			set { base.Font = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public new Color ForeColor {
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public new Color BackColor {
			get { return base.BackColor; }
			set { base.BackColor = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public new Color BorderColor {
			get { return base.BorderColor; }
			set { base.BorderColor = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public new float BorderWidth {
			get { return base.BorderWidth; }
			set { base.BorderWidth = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string XlsxFormatString { get { return ""; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override TextAlignment TextAlignment {
			get { return base.TextAlignment; }
			set { base.TextAlignment = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override bool WordWrap {
			get { return base.WordWrap; }
			set { base.WordWrap = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string StyleName { get { return ""; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string EvenStyleName { get { return ""; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OddStyleName { get { return ""; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override XRBindingCollection DataBindings {
			get { return base.DataBindings; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override PaddingInfo Padding {
			get { return base.Padding; }
			set { base.Padding = value; }
		}
		[
		SRCategory(ReportStringId.CatAppearance),
		Browsable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Always),
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridStyles"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGrid.Styles"),
		]
		public new XRPivotGridStyles Styles {
			get { return base.Styles as XRPivotGridStyles; }
		}
		#endregion
		#region hidden events
		[ Browsable(false),  EditorBrowsable(EditorBrowsableState.Never) ]
		public override event PrintOnPageEventHandler PrintOnPage {
			add { }
			remove { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event PreviewMouseEventHandler PreviewMouseMove {
			add { }
			remove { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event PreviewMouseEventHandler PreviewMouseDown {
			add { }
			remove { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event PreviewMouseEventHandler PreviewMouseUp {
			add { }
			remove { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event PreviewMouseEventHandler PreviewClick {
			add { }
			remove { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event PreviewMouseEventHandler PreviewDoubleClick {
			add { }
			remove { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event DrawEventHandler Draw {
			add { }
			remove { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event HtmlEventHandler HtmlItemCreated {
			add { }
			remove { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event EventHandler TextChanged {
			add { }
			remove { }
		}
		#endregion
		XRPivotGridData viewInfoData;
		string olapConnectionString;
		string dataMember = String.Empty;
		object dataSource = null;
		object dataAdapter;
		bool isDataSourceActive;
		XRPivotGridPrinter printer;
		XRPivotGridComponentLink link;
		PrintingSystemBase fakedPrintingSystem;
		XRPivotGridAppearances appearance;
		XRPivotGridAppearances appearancePrint;
		bool loading;
		bool wasRefreshed;
		BestFitHelper bestFitHelper = new BestFitHelper();
		BindingListHelper bindingListHelper = new BindingListHelper();
		PivotChartDataSourceBase ChartDataSource { get { return Data != null ? ((XRPivotGridData)Data).ChartDataSource : null; } }
		protected internal override bool ReportIsLoading {
			get { return base.ReportIsLoading || loading; }
		}
		PivotGridData IPivotGridDataContainer.Data {
			get { return this.Data; }
		}
		PrintingSystemBase FakedPrintingSystem {
			get {
				if(fakedPrintingSystem == null) {
					fakedPrintingSystem = new PrintingSystemBase();
					fakedPrintingSystem.PageSettings.Assign(
						new Margins(100, 100, 100, 100),
						new Margins(0, 0, 0, 0),
						PaperKind.Custom,
						new Size(int.MaxValue / 100, XtraReport.DefaultPageSize.Height),
						false);
				}
				return fakedPrintingSystem;
			}
		}
		internal override IList VisibleComponents {
			get {
				ArrayList components = new ArrayList();
				components.AddRange(this.Fields);
				return components;
			}
		}
		protected internal override int DefaultWidth {
			get { return DefaultSizes.PivotGrid.Width; }
		}
		protected internal override int DefaultHeight {
			get { return DefaultSizes.PivotGrid.Height; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGrid.ActualWidth"),
		SRCategory(ReportStringId.CatLayout),
		Browsable(false)
		]
		public int ActualWidth {
			get {
				return GraphicsUnitConverter.Convert(Printer.BestFitter.CellSizeProvider.GetWidthDifference(false, 0, Data.VisualItems.GetLevelCount(false)) +
														Printer.BestFitter.CellSizeProvider.GetWidthDifference(true, 0, Data.VisualItems.ColumnCount),
													GraphicsDpi.DeviceIndependentPixel, Dpi);
			}
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGrid.ActualHeight"),
		SRCategory(ReportStringId.CatLayout),
		Browsable(false)
		 ]
		public int ActualHeight {
			get {
				return GraphicsUnitConverter.Convert(Printer.GetRowAreaY() +
					   Printer.BestFitter.CellSizeProvider.GetHeightDifference(false, 0, Data.VisualItems.RowCount), GraphicsDpi.DeviceIndependentPixel, Dpi);
			}
		}
		protected internal override bool HasUndefinedBounds {
			get { return true; }
		}
		protected LinkBase Link {
			get {
				ValidateLink();
				if(link == null) {
					link = new XRPivotGridComponentLink(Printer);
				}
				return link;
			}
		}
		void ValidateLink() {
			if(link != null && link.Printer.DesignMode != this.DesignMode) {
				link.Dispose();
				link = null;
			}
		}
		XRPivotGridPrinter Printer {
			get {
				ValidatePrinter();
				if(printer == null) {
					printer = XRPivotGridPrinter.CreateInstance(this);
					printer.Owner = this;
				}
				return printer;
			}
		}
		void ValidatePrinter() {
			if(printer != null && printer.DesignMode != this.DesignMode) {
				if(printer.DesignMode) {
					wasRefreshed = false;
					DoRefreshData();
				} else {
					object oldDatasource = DataSource;
					DataSource = null;
					ActivateDataSource();
					DataSource = oldDatasource;
				}
				printer.Dispose();
				printer = null;
			}
		}
		protected PivotGridData Data {
			get {
				ActivateDataSource();
				return viewInfoData;
			}
		}
		void ResetAppearance() { Appearance.Reset(); }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridAppearance"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGrid.Appearance"),
		SRCategory(ReportStringId.CatAppearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdAppearance)
		]
		public XRPivotGridAppearances Appearance {
			get {
				if(appearance == null)
					appearance = new XRPivotGridAppearances();
				return appearance;
			}
		}
		[
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdAppearance)
		]
		public XRPivotGridAppearances AppearancePrint {
			get {
				if(appearancePrint == null)
					appearancePrint = new XRPivotGridAppearances();
				return appearancePrint;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridDataAdapter"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGrid.DataAdapter"),
		DefaultValue(null),
		SRCategory(ReportStringId.CatData),
		Editor("DevExpress.XtraReports.Design.DataAdapterEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(UITypeEditor)),
		TypeConverterAttribute(typeof(DevExpress.XtraReports.Design.DataAdapterConverter))
		]
		public object DataAdapter {
			get { return dataAdapter; }
			set {
				if (dataAdapter != value) {
					dataAdapter = value;
				}
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridDataMember"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGrid.DataMember"),
		SRCategory(ReportStringId.CatData),
		DefaultValue(""),
		TypeConverter(typeof(DevExpress.XtraReports.Design.DataMemberTypeConverter)),
		Editor("DevExpress.XtraReports.Design.DataContainerDataMemberEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(UITypeEditor)),
		XtraSerializableProperty,
		]
		public string DataMember {
			get { return dataMember == null ? string.Empty : dataMember; }
			set {
				if (value == null)
					value = String.Empty;
				if (dataMember != value) {
					isDataSourceActive = false;
					dataMember = value;
				}
			}
		}
		protected override void AdjustDataSource() {
			base.AdjustDataSource();
			if(dataSource == ParentDataSource)
				dataSource = null;
		}
		bool ShouldSerializeDataSource() {
			return dataSource is IComponent;
		}
		bool ShouldSerializeXmlDataSource() {
			return dataSource != null;
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridDataSource"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGrid.DataSource"),
		SRCategory(ReportStringId.CatData),
		RefreshProperties(RefreshProperties.Repaint),
		Editor("DevExpress.XtraReports.Design.DataSourceEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(UITypeEditor)),
		TypeConverter(typeof(DataSourceConverter)),
		XtraSerializableProperty(XtraSerializationVisibility.Reference),
		]
		public object DataSource {
			get { return dataSource; }
			set {
				if (dataSource != value && !(DataSource is string)) {
					isDataSourceActive = false;
					dataSource = value;
					ValidateDataMember(dataSource);
				}
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridOLAPConnectionString"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGrid.OLAPConnectionString"),
		SRCategory(ReportStringId.CatData),
		Localizable(true),
		DefaultValue(null),
		Editor("DevExpress.XtraPivotGrid.Design.OLAPConnectionEditor," + AssemblyInfo.SRAssemblyPivotGrid + AssemblyInfo.FullAssemblyVersionExtension, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty
		]
		public string OLAPConnectionString {
			get { return olapConnectionString; }
			set {
				if (olapConnectionString != value) {
					olapConnectionString = value;
					isDataSourceActive = false;
					if (!String.IsNullOrEmpty(value)) DataSource = null;
					ActivateDataSource();
				}
			}
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGrid.OLAPDataProvider"),
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridOLAPDataProvider"),
#endif
		SRCategory(ReportStringId.CatData), 
		Localizable(true), 
		DefaultValue(OLAPDataProvider.Default),
		XtraSerializableProperty
		]
		public OLAPDataProvider OLAPDataProvider {
			get { return Data.OLAPDataProvider; }
			set { Data.OLAPDataProvider = value; }
		}
		bool ShouldSerializeOptionsPrint() { return OptionsPrint.ShouldSerialize(); }
		void ResetOptionsPrint() { OptionsPrint.Reset(); }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridOptionsPrint"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGrid.OptionsPrint"),
		SRCategory(ReportStringId.CatOptions),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)
		]
		public XRPivotGridOptionsPrint OptionsPrint {
			get { return Data.OptionsPrint as XRPivotGridOptionsPrint; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridOptionsView"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGrid.OptionsView"),
		SRCategory(ReportStringId.CatOptions),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), 
		XtraSerializablePropertyId(LayoutIdOptionsView)
		]
		public XRPivotGridOptionsView OptionsView {
			get { return Data.OptionsView as XRPivotGridOptionsView; }
		}
		bool ShouldSerializeOptionsDataField() { return OptionsDataField.ShouldSerialize(); }
		void ResetOptionsDataField() { OptionsDataField.Reset(); }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridOptionsDataField"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGrid.OptionsDataField"),
		SRCategory(ReportStringId.CatOptions),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsDataField OptionsDataField {
			get { return Data.OptionsDataField; }
		}
		bool ShouldSerializeOptionsData() { return OptionsData.ShouldSerialize(); }
		void ResetOptionsData() { OptionsData.Reset(); }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridOptionsData"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGrid.OptionsData"),
		SRCategory(ReportStringId.CatOptions),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsData OptionsData {
			get { return Data.OptionsData; }
		}
		bool ShouldSerializeOptionsLayout() { return OptionsLayout.ShouldSerialize(); }
		void ResetOptionsLayout() { OptionsLayout.Reset(); }
		PivotGridOptionsLayout optionsLayout;
		protected virtual PivotGridOptionsLayout CreateOptionsLayout() {
			return new PivotGridOptionsLayout();
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridOptionsLayout"),
#endif
		SRCategory(ReportStringId.CatOptions),
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PivotGridOptionsLayout OptionsLayout {
			get {
				if(optionsLayout == null)
					optionsLayout = CreateOptionsLayout();
				return optionsLayout;
			}
		}
		bool ShouldSerializeFields() {
			return Fields.Count != 0;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public virtual PivotGridOptionsOLAP OptionsOLAP { get { return Data.OptionsOLAP; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsCustomization OptionsCustomization { get { return Data.OptionsCustomization; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsBehaviorBase OptionsBehavior { get { return Data.OptionsBehavior; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true, 100),
		XtraSerializablePropertyId(LayoutIdLayout)
		]
		public PivotGridGroupCollection Groups { get { return Data.Groups; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridFields"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGrid.Fields"),
		Browsable(true),
		SRCategory(ReportStringId.CatData),
		Editor("DevExpress.XtraPivotGrid.Design.FieldsCollectionEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, true, 0, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIdColumns)
		]
		public XRPivotGridFieldCollection Fields {
			get { return (XRPivotGridFieldCollection)Data.Fields; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridPrefilter"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGrid.Prefilter"),
		SRCategory(ReportStringId.CatData),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public XRPrefilter Prefilter {
			get { return (XRPrefilter)Data.Prefilter; }
		}
		bool ShouldSerializeScripts() {
			return !fEventScripts.IsDefault();
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridScripts"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGrid.Scripts"),
		SRCategory(ReportStringId.CatBehavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public new XRPivotGridScripts Scripts { get { return (XRPivotGridScripts)fEventScripts; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridOptionsChartDataSource"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPivotGrid.OptionsChartDataSource"),
		SRCategory(ReportStringId.CatOptions),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)
		]
		public PivotGridOptionsChartDataSource OptionsChartDataSource { get { return (PivotGridOptionsChartDataSource)Data.OptionsChartDataSource; } }
		[Browsable(false)]
		public int RowCount { get { return Data.VisualItems.RowCount; } }
		[Browsable(false)]
		public int ColumnCount { get { return Data.VisualItems.ColumnCount; } }
		public PivotCellBaseEventArgs GetCellInfo(int columnIndex, int rowIndex) {
			PivotGridCellItem cellItem = Data.VisualItems.CreateCellItem(columnIndex, rowIndex);
			return cellItem != null ? new PivotCellBaseEventArgs(cellItem) : null;
		}
		#region Styles
		string cellStyleName = String.Empty;
		bool ShouldSerializeCellStyleName() {
			return this.Styles.CellStyle != null;
		}
		[
		Browsable(false),
		XtraSerializableProperty
		]
		public virtual string CellStyleName { get { return cellStyleName; } set { cellStyleName = value != null ? value : string.Empty; } }
		internal XRControlStyle CellStyleCore { get { return GetStyle(CellStyleName); } set { CellStyleName = RegisterStyle(value); } }
		string customTotalCellStyleName = String.Empty;
		bool ShouldSerializeCustomTotalCellStyleName() {
			return this.Styles.CustomTotalCellStyle != null;
		}
		[
		Browsable(false),
		XtraSerializableProperty
		]
		public virtual string CustomTotalCellStyleName { get { return customTotalCellStyleName; } set { customTotalCellStyleName = value != null ? value : string.Empty; } }
		internal XRControlStyle CustomTotalCellStyleCore { get { return GetStyle(CustomTotalCellStyleName); } set { CustomTotalCellStyleName = RegisterStyle(value); } }
		string fieldHeaderStyleName = String.Empty;
		bool ShouldSerializeFieldHeaderStyleName() {
			return this.Styles.FieldHeaderStyle != null;
		}
		[
		Browsable(false),
		XtraSerializableProperty
		]
		public virtual string FieldHeaderStyleName { get { return fieldHeaderStyleName; } set { fieldHeaderStyleName = value != null ? value : string.Empty; } }
		internal XRControlStyle FieldHeaderStyleCore { get { return GetStyle(FieldHeaderStyleName); } set { FieldHeaderStyleName = RegisterStyle(value); } }
		string fieldValueStyleName = String.Empty;
		bool ShouldSerializeFieldValueStyleName() {
			return this.Styles.FieldValueStyle != null;
		}
		[
		Browsable(false),
		XtraSerializableProperty
		]
		public virtual string FieldValueStyleName { get { return fieldValueStyleName; } set { fieldValueStyleName = value != null ? value : string.Empty; } }
		internal XRControlStyle FieldValueStyleCore { get { return GetStyle(FieldValueStyleName); } set { FieldValueStyleName = RegisterStyle(value); } }
		string fieldValueGrandTotalStyleName = String.Empty;
		bool ShouldSerializeFieldValueGrandTotalStyleName() {
			return this.Styles.FieldValueGrandTotalStyle != null;
		}
		[
		Browsable(false),
		XtraSerializableProperty
		]
		public virtual string FieldValueGrandTotalStyleName { get { return fieldValueGrandTotalStyleName; } set { fieldValueGrandTotalStyleName = value != null ? value : string.Empty; } }
		internal XRControlStyle FieldValueGrandTotalStyleCore { get { return GetStyle(FieldValueGrandTotalStyleName); } set { FieldValueGrandTotalStyleName = RegisterStyle(value); } }
		string fieldValueTotalStyleName = String.Empty;
		bool ShouldSerializeFieldValueTotalStyleName() {
			return this.Styles.FieldValueTotalStyle != null;
		}
		[
		Browsable(false),
		XtraSerializableProperty
		]
		public virtual string FieldValueTotalStyleName { get { return fieldValueTotalStyleName; } set { fieldValueTotalStyleName = value != null ? value : string.Empty; } }
		internal XRControlStyle FieldValueTotalStyleCore { get { return GetStyle(FieldValueTotalStyleName); } set { FieldValueTotalStyleName = RegisterStyle(value); } }
		string filterSeparatorStyleName = String.Empty;
		bool ShouldSerializeFilterSeparatorStyleName() {
			return this.Styles.FilterSeparatorStyle != null;
		}
		[
		Browsable(false),
		XtraSerializableProperty
		]
		public virtual string FilterSeparatorStyleName { get { return filterSeparatorStyleName; } set { filterSeparatorStyleName = value != null ? value : string.Empty; } }
		internal XRControlStyle FilterSeparatorStyleCore { get { return GetStyle(FilterSeparatorStyleName); } set { FilterSeparatorStyleName = RegisterStyle(value); } }
		string grandTotalCellStyleName = String.Empty;
		bool ShouldSerializeGrandTotalCellStyleName() {
			return this.Styles.GrandTotalCellStyle != null;
		}
		[
		Browsable(false),
		XtraSerializableProperty
		]
		public virtual string GrandTotalCellStyleName { get { return grandTotalCellStyleName; } set { grandTotalCellStyleName = value != null ? value : string.Empty; } }
		internal XRControlStyle GrandTotalCellStyleCore { get { return GetStyle(GrandTotalCellStyleName); } set { GrandTotalCellStyleName = RegisterStyle(value); } }
		string headerGroupLineStyleName = String.Empty;
		bool ShouldSerializeHeaderGroupLineStyleName() {
			return this.Styles.HeaderGroupLineStyle != null;
		}
		[
		Browsable(false),
		XtraSerializableProperty
		]
		public virtual string HeaderGroupLineStyleName { get { return headerGroupLineStyleName; } set { headerGroupLineStyleName = value != null ? value : string.Empty; } }
		internal XRControlStyle HeaderGroupLineStyleCore { get { return GetStyle(HeaderGroupLineStyleName); } set { HeaderGroupLineStyleName = RegisterStyle(value); } }
		string linesStyleName = String.Empty;
		bool ShouldSerializeLinesStyleName() {
			return this.Styles.LinesStyle != null;
		}
		[
		Browsable(false),
		XtraSerializableProperty
		]
		public virtual string LinesStyleName { get { return linesStyleName; } set { linesStyleName = value != null ? value : string.Empty; } }
		internal XRControlStyle LinesStyleCore { get { return GetStyle(LinesStyleName); } set { LinesStyleName = RegisterStyle(value); } }
		string totalCellStyleName = String.Empty;
		bool ShouldSerializeTotalCellStyleName() {
			return this.Styles.TotalCellStyle != null;
		}
		[
		Browsable(false),
		XtraSerializableProperty
		]
		public virtual string TotalCellStyleName { get { return totalCellStyleName; } set { totalCellStyleName = value != null ? value : string.Empty; } }
		internal XRControlStyle TotalCellStyleCore { get { return GetStyle(TotalCellStyleName); } set { TotalCellStyleName = RegisterStyle(value); } }
		#endregion
		public XRPivotGrid() {
			viewInfoData = CreatePivotGridData();
			OptionsView.ShowAllTotals();
		}
		protected virtual XRPivotGridData CreatePivotGridData() {
			return new XRPivotGridData(this);
		}
		protected override XRControlScripts CreateScripts() {
			return new XRPivotGridScripts(this);
		}
		public object GetCellValue(int columnIndex, int rowIndex) {
			return Data.VisualItems.GetCellValue(columnIndex, rowIndex);
		}
		public object GetCellValue(object[] columnValues, object[] rowValues) {
			if(Data.GetFieldCountByArea(PivotArea.DataArea) != 1)
				throw new Exception("This method can be used if there is just one field in the data area only.");
			PivotGridFieldBase dataField = GetFieldByArea(PivotArea.DataArea, 0);
			return GetCellValueCore(columnValues, rowValues, dataField);
		}
		public object GetCellValue(object[] columnValues, object[] rowValues, XRPivotGridField dataField) {
			return GetCellValueCore(columnValues, rowValues, dataField);
		}
		object GetCellValueCore(object[] columnValues, object[] rowValues, PivotGridFieldBase dataField) {
			return Data.GetCellValue(columnValues, rowValues, dataField);
		}
		public object GetFieldValue(PivotGridFieldBase field, int lastLevelIndex) {
			return Data.VisualItems.GetFieldValue(field, lastLevelIndex);
		}
		public PivotGridValueType GetFieldValueType(bool isColumn, int lastLevelIndex) {
			return Data.VisualItems.GetLastLevelItem(isColumn, lastLevelIndex).ValueType;
		}
		public PivotGridFieldBase GetFieldByArea(PivotArea area, int areaIndex) {
			return (PivotGridFieldBase)Data.GetFieldByArea(area, areaIndex);
		}
		public void RetrieveFields() {
			Data.RetrieveFields();
		}
		public void CollapseAll() {
			Data.ChangeExpandedAll(false);
		}
		public void CollapseAllRows() {
			Data.ChangeExpandedAll(false, false);
		}
		public void CollapseAllColumns() {
			Data.ChangeExpandedAll(true, false);
		}
		public bool IsObjectCollapsed(XRPivotGridField field, int lastLevelIndex) {
			return Data.VisualItems.IsObjectCollapsed(field, lastLevelIndex);
		}
		public bool IsObjectCollapsed(bool isColumn, int lastLevelIndex, int level) {
			return Data.VisualItems.IsObjectCollapsed(isColumn, lastLevelIndex, level);
		}
		public void ExpandAll() {
			Data.ChangeExpandedAll(true);
		}
		public void ExpandAllRows() {
			Data.ChangeExpandedAll(false, true);
		}
		public void ExpandAllColumns() {
			Data.ChangeExpandedAll(true, true);
		}
		internal void ActivateViewInfoData(PivotGridData viewInfoData) {
			if(!String.IsNullOrEmpty(olapConnectionString))
				viewInfoData.OLAPConnectionString = olapConnectionString;
			else if(DataSource is IPivotGridDataSource)
				viewInfoData.PivotDataSource = (IPivotGridDataSource)DataSource;
			else {
				if(viewInfoData.VisualItems.RowCount <= 1)
					viewInfoData.ListSource = null;
				viewInfoData.ListSource = GetDataSource(GetEffectiveDataSource(), GetEffectiveDataMember());
				bindingListHelper.CheckListChanged(viewInfoData.ListSource, () => viewInfoData.ReloadData());
			}
		}
		internal string GetEffectiveDataMember() {
			return string.IsNullOrEmpty(dataMember) && Parent != null && Parent.Report != null ? 
				Parent.Report.DataMember : 
				DataMember;
		}
		object IDataContainer.GetEffectiveDataSource() {
			return this.GetEffectiveDataSource();
		}
		object ParentDataSource {
			get { return Report != null ? Report.GetEffectiveDataSource() : null; }
		}
		internal object GetEffectiveDataSource() {
			return dataSource != null ? dataSource : ParentDataSource;
		}
		object IDataContainer.GetSerializableDataSource() {
			return DataSourceHelper.ConvertToSerializableDataSource(dataSource);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				appearance = null;
				appearancePrint = null;
				if(viewInfoData != null) {
					viewInfoData.Dispose();
					viewInfoData = null;
				}
				if(link != null) {
					link.Dispose();
					link = null;
				}
				if(printer != null) {
					printer.Dispose();
					printer = null;
				}
				if(fakedPrintingSystem != null) {
					fakedPrintingSystem.Dispose();
					fakedPrintingSystem = null;
				}
				if(bindingListHelper != null) {
					bindingListHelper.Dispose();
					bindingListHelper = null;
				}
			} 
			base.Dispose(disposing);
		}
		protected internal override void OnReportInitialize() {
			base.OnReportInitialize();
			if(RootReport.VersionLessThen(v12_2))
				foreach(XRPivotGridField field in Fields)
					for(int i = field.CustomTotals.Count - 1; i >= 0; i--)
						field.CustomTotals.ValidateItem(i);
		}
		public void BestFit(XRPivotGridField field) {
			BestFitCore(field);
		}
		public void BestFit() {
			BestFitCore(Data);
		}
		void BestFitCore(object obj) {
			if(isDataSourceActive) {
				SynchAppearancePrint();
				bestFitHelper.BestFit(obj, Printer.BestFitter);
			} else
				bestFitHelper.ApplyBestFit(obj);
		}
		protected override XRControlStyles CreateStyles() {
			return new XRPivotGridStyles(this);
		}
		protected override void CollectAssociatedComponents(DesignItemList components) {
			base.CollectAssociatedComponents(components);
			components.AddDataSource(((IDataContainer)this).GetSerializableDataSource() as IComponent);
			components.AddDataAdapter(dataAdapter);
			SuspendDataAndExec(delegate() {
				foreach(object item in Fields)
					components.Add(item);
			});
		}
		protected internal override void CopyDataProperties(XRControl control) {
			XRPivotGrid pivotGrid = control as XRPivotGrid;
			if (pivotGrid != null) {
				DataSource = pivotGrid.DataSource;
				DataAdapter = pivotGrid.DataAdapter;
				if(pivotGrid.Fields != null) {
					foreach(XRPivotGridField src in pivotGrid.Fields) {
						XRPivotGridField dst = Fields[src.FieldName];
						if(dst == null)
							continue;
						dst.FilterValues.Values = src.FilterValues.Values;
						dst.FilterValues.ShowBlanks = src.FilterValues.ShowBlanks;
						dst.FilterValues.FilterType = src.FilterValues.FilterType;
					}
				}
			}
			base.CopyDataProperties(control);
		}
		void ValidateDataMember(object dataSource) {
			if(!this.ReportIsLoading)
				dataMember = XRDataContext.ValidateDataMember(dataSource, dataMember);
		}
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			ActivateDataSource();
			SynchAppearancePrint();
			bestFitHelper.EnsureBestFit(Printer.BestFitter);
			if(DesignMode)
				DoRefreshData();
			return CreatePresenter().CreateBrick(childrenBricks);
		}
		protected override ControlPresenter CreatePresenter() {
			return CreatePresenter<ControlPresenter>(
				delegate() { return new PivotGridPresenter(this); },
				delegate() { return new DesignPivotGridPresenter(this, FakedPrintingSystem, Link); },
				delegate() { return new LayoutViewPivotGridPresenter(this, FakedPrintingSystem, Link); }
			);
		}
		void DoRefreshData() {
			if(!this.ReportIsLoading && !wasRefreshed) {
				viewInfoData.DoRefresh();
				wasRefreshed = true;
			}
		}
		protected override void WriteContentToCore(XRWriteInfo writeInfo, VisualBrick brick) {
			SubreportBrick subrepBrick = (SubreportBrick)brick;
			SubreportDocumentBand subrepBand = new SubreportDocumentBand(brick.Rect);
			subrepBrick.DocumentBand = subrepBand;
			PrintingSystemBase ps = writeInfo.PrintingSystem;
			Link.PrintingSystemBase = ps;
			bool oringinalDip = ps.Graph.DeviceIndependentPixel;
			ps.Graph.DeviceIndependentPixel = true;
			Link.AddSubreport(writeInfo.PrintingSystem, subrepBand, PointF.Empty);
			ps.Graph.PageUnit = GraphicsUnit.Document;
			ps.Graph.DeviceIndependentPixel = oringinalDip;
			DocumentBand band = FindFirstBandWithBricks(subrepBand);
			if(band != null) {
				VisualBrick topLeftBrick = GetTopLeftBrick(band);
				if(topLeftBrick != null)
					topLeftBrick.BookmarkInfo = CreateBookmarkInfo();
			}
			base.WriteContentToCore(writeInfo, brick);
		}
		DocumentBand FindFirstBandWithBricks(DocumentBand band) {
			if(band.Bricks.Count > 0)
				return band;
			foreach(DocumentBand innerBand in band.Bands) {
				DocumentBand foundBand = FindFirstBandWithBricks(innerBand);
				if(foundBand != null)
					return foundBand;
			}
			return null;
		}
		VisualBrick GetTopLeftBrick(DocumentBand band) {
			VisualBrick topLeftBrick = null;
			foreach(VisualBrick brick in band.Bricks) {
				if(topLeftBrick == null)
					topLeftBrick = brick;
				else if(brick.Y < topLeftBrick.Y)
					topLeftBrick = brick;
				else if(brick.Y == topLeftBrick.Y && brick.X < topLeftBrick.X)
					topLeftBrick = brick;
			}
			return topLeftBrick;
		}
		void ActivateDataSource() {
			if (!this.ReportIsLoading && !isDataSourceActive) {
				isDataSourceActive = true;
				ActivateViewInfoData(viewInfoData);
			}
		}
		internal IList<PropertyDescriptor> GetCalculatedFields() {
			return RootReport.GetAppropriatedCalculatedFields(GetEffectiveDataSource(), GetEffectiveDataMember());
		}
		IList GetDataSource(object dataSource, string dataMember) {
			if(Report != null && Report.DataContext != null)
				return GetDataSourceCore(Report.DataContext, dataSource, dataMember);
			using(DataContext temporaryDataContext = new DataContext()) {
				return GetDataSourceCore(temporaryDataContext, dataSource, dataMember);
			}
		}
		static IList GetDataSourceCore(DataContext dataContext, object dataSource, string dataMember) {
			ListBrowser dataBrowser = dataContext.GetDataBrowser(dataSource, dataMember, true) as ListBrowser;
			return dataBrowser != null ? dataBrowser.List : null;
		}
		protected override void UpdateBindingCore(XRDataContext dataContext, ImagesContainer images) {
			isDataSourceActive = false;
			base.UpdateBindingCore(dataContext, images);
		}
		public void RestoreLayoutFromStream(Stream stream) {
			RestoreLayoutFromStream(stream, OptionsLayout);
		}
		public void RestoreLayoutFromStream(Stream stream, OptionsLayoutBase options) {
			RestoreLayoutCore(new DevExpress.Utils.Serializing.XmlXtraSerializer(), stream, options);
			SynchAppearances(this, Appearance, AppearancePrint);
		}
		void SynchAppearancePrint() {
			SynchAppearance(this, AppearancePrint.Cell, this.Appearance.Cell, this.CellStyleCore);
			SynchAppearance(this, AppearancePrint.CustomTotalCell, this.Appearance.CustomTotalCell, this.CustomTotalCellStyleCore);
			SynchAppearance(this, AppearancePrint.FieldHeader, this.Appearance.FieldHeader, this.FieldHeaderStyleCore);
			SynchAppearance(this, AppearancePrint.FieldValue, this.Appearance.FieldValue, this.FieldValueStyleCore);
			SynchAppearance(this, AppearancePrint.FieldValueGrandTotal, this.Appearance.FieldValueGrandTotal, this.FieldValueGrandTotalStyleCore);
			SynchAppearance(this, AppearancePrint.FieldValueTotal, this.Appearance.FieldValueTotal, this.FieldValueTotalStyleCore);
			SynchAppearance(this, AppearancePrint.GrandTotalCell, this.Appearance.GrandTotalCell, this.GrandTotalCellStyleCore);
			SynchAppearance(this, AppearancePrint.Lines, this.Appearance.Lines, this.LinesStyleCore);
			SynchAppearance(this, AppearancePrint.TotalCell, this.Appearance.TotalCell, this.TotalCellStyleCore);
		}
		static void SynchAppearances(XRPivotGrid xrPivotGrid, XRPivotGridAppearances destAppearances, XRPivotGridAppearances sourceAppearances) {
			SynchAppearance(xrPivotGrid, destAppearances.Cell, sourceAppearances.Cell, null);
			SynchAppearance(xrPivotGrid, destAppearances.CustomTotalCell, sourceAppearances.CustomTotalCell, null);
			SynchAppearance(xrPivotGrid, destAppearances.FieldHeader, sourceAppearances.FieldHeader, null);
			SynchAppearance(xrPivotGrid, destAppearances.FieldValue, sourceAppearances.FieldValue, null);
			SynchAppearance(xrPivotGrid, destAppearances.FieldValueGrandTotal, sourceAppearances.FieldValueGrandTotal, null);
			SynchAppearance(xrPivotGrid, destAppearances.FieldValueTotal, sourceAppearances.FieldValueTotal, null);
			SynchAppearance(xrPivotGrid, destAppearances.GrandTotalCell, sourceAppearances.GrandTotalCell, null);
			SynchAppearance(xrPivotGrid, destAppearances.Lines, sourceAppearances.Lines, null);
			SynchAppearance(xrPivotGrid, destAppearances.TotalCell, sourceAppearances.TotalCell, null);
		}
		static void SynchAppearance(XRPivotGrid xrPivotGrid, PrintAppearanceObject destAppearance, PrintAppearanceObject sourceAppearance, XRControlStyle style) {
			if(destAppearance == null) {
				return;
			}
			destAppearance.Reset();
			if(sourceAppearance.IsSetBackColor)
				destAppearance.BackColor = sourceAppearance.BackColor;
			else if(BrickStyle.PropertyIsSet(style, StyleProperty.BackColor)) {
				destAppearance.BackColor = style.BackColor;
			}	  
			if(sourceAppearance.IsSetBorderColor)
				destAppearance.BorderColor = sourceAppearance.BorderColor;
			else if(BrickStyle.PropertyIsSet(style, StyleProperty.BorderColor)) {
				destAppearance.BorderColor = style.BorderColor;
			}		   
			if(sourceAppearance.IsSetFont)
				destAppearance.Font = sourceAppearance.Font;
			else if(BrickStyle.PropertyIsSet(style, StyleProperty.Font)) {
				destAppearance.Font = style.Font;
			}
			if(sourceAppearance.IsSetForeColor)
				destAppearance.ForeColor = sourceAppearance.ForeColor;
			else if(BrickStyle.PropertyIsSet(style, StyleProperty.ForeColor)) {
				destAppearance.ForeColor = style.ForeColor;
			}
			if(((XRAppearanceObject)sourceAppearance).IsSetTextHorizontalAlignment) {
				destAppearance.TextHorizontalAlignment = sourceAppearance.TextHorizontalAlignment;
			} else if(BrickStyle.PropertyIsSet(style, StyleProperty.TextAlignment)) {
				destAppearance.TextHorizontalAlignment = XRAppearanceObject.ToHorzAlignment(style.TextAlignment);
			}
			if(((XRAppearanceObject)sourceAppearance).IsSetTextVerticalAlignment) {
				destAppearance.TextVerticalAlignment = sourceAppearance.TextVerticalAlignment;
			} else if(BrickStyle.PropertyIsSet(style, StyleProperty.TextAlignment)) {
				destAppearance.TextVerticalAlignment = XRAppearanceObject.ToVertAlignment(style.TextAlignment);
			}
			destAppearance.WordWrap = sourceAppearance.WordWrap;
			destAppearance.Trimming = sourceAppearance.Trimming;
		}
		public virtual void RestoreLayoutCore(XtraSerializer serializer, object path, OptionsLayoutBase options) {
			serializer.DeserializeObject(this, path, Data.AppName, options);
		}
		#region CustomFieldValueCells event
		static readonly object CustomFieldValueCellsEvent = new object();
		public event EventHandler<PivotCustomFieldValueCellsEventArgs> CustomFieldValueCells {
			add { this.Events.AddHandler(CustomFieldValueCellsEvent, value); }
			remove { this.Events.RemoveHandler(CustomFieldValueCellsEvent, value); }
		}
		protected virtual bool RaiseCustomFieldValueCells(PivotVisualItemsBase items) {
			if(Data == null)
				return false;
			PivotCustomFieldValueCellsEventArgs e = new PivotCustomFieldValueCellsEventArgs(items);
			bool scriptWasRun = RunEventScript(CustomFieldValueCellsEvent, EventNames.CustomFieldValueCells, e);
			EventHandler<PivotCustomFieldValueCellsEventArgs> handler = (EventHandler<PivotCustomFieldValueCellsEventArgs>)this.Events[CustomFieldValueCellsEvent];
			if(handler != null) handler(this, e);
			if(handler != null || scriptWasRun)
				return e.IsUpdateRequired;
			else
				return false;
		}
		#endregion
		#region CustomUnboundFieldData event
		static readonly object CustomUnboundFieldDataEvent = new object();
		public event EventHandler<CustomFieldDataEventArgs> CustomUnboundFieldData {
			add { this.Events.AddHandler(CustomUnboundFieldDataEvent, value); }
			remove { this.Events.RemoveHandler(CustomUnboundFieldDataEvent, value); }
		}
		protected virtual void RaiseCustomUnboundColumnData(CustomFieldDataEventArgs e) {
			RunEventScript(CustomUnboundFieldDataEvent, EventNames.CustomUnboundFieldData, e);
			EventHandler<CustomFieldDataEventArgs> handler = (EventHandler<CustomFieldDataEventArgs>)this.Events[CustomUnboundFieldDataEvent];
			if(handler != null) handler(this, e);
		}
		#endregion
		#region CustomFieldSort event
		static readonly object CustomFieldSortEvent = new object();
		public event EventHandler<PivotGridCustomFieldSortEventArgs> CustomFieldSort {
			add { this.Events.AddHandler(CustomFieldSortEvent, value); }
			remove { this.Events.RemoveHandler(CustomFieldSortEvent, value); }
		}
		static readonly object CustomServerModeSortEvent = new object();
		public event EventHandler<PivotGridCustomServerModeSortEventArgs> CustomServerModeSort {
			add { this.Events.AddHandler(CustomServerModeSortEvent, value); }
			remove { this.Events.RemoveHandler(CustomServerModeSortEvent, value); }
		}
		PivotGridCustomFieldSortEventArgs fieldSortEventArgs = null;
		protected virtual int? RaiseCustomFieldSort(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotGridFieldBase field, PivotSortOrder sortOrder) {
			if(fieldSortEventArgs != null && (fieldSortEventArgs.Field != field || fieldSortEventArgs.Data != Data))
				fieldSortEventArgs = null;
			if(fieldSortEventArgs == null)
				fieldSortEventArgs = new PivotGridCustomFieldSortEventArgs(Data, field as XRPivotGridField);
			fieldSortEventArgs.SetArgs(listSourceRow1, listSourceRow2, value1, value2, sortOrder);
			bool scriptWasRun = RunEventScript(CustomFieldSortEvent, EventNames.CustomFieldSort, fieldSortEventArgs);
			EventHandler<PivotGridCustomFieldSortEventArgs> handler = (EventHandler<PivotGridCustomFieldSortEventArgs>)this.Events[CustomFieldSortEvent];
			if(handler != null) handler(this, fieldSortEventArgs);
			if(handler != null || scriptWasRun)
				return fieldSortEventArgs.GetSortResult();
			else
				return null;
		}
		PivotGridCustomServerModeSortEventArgs serverModeSortEventArgs = null;
		protected virtual int? RaiseCustomServerModeSort(DevExpress.PivotGrid.QueryMode.Sorting.IQueryMemberProvider value0, DevExpress.PivotGrid.QueryMode.Sorting.IQueryMemberProvider value1, PivotGridFieldBase field, DevExpress.PivotGrid.QueryMode.Sorting.ICustomSortHelper helper) {
			if(serverModeSortEventArgs != null && serverModeSortEventArgs.Field != field)
				serverModeSortEventArgs = null;
			if(serverModeSortEventArgs == null)
				serverModeSortEventArgs = new PivotGridCustomServerModeSortEventArgs((XRPivotGridField)field);
			serverModeSortEventArgs.SetArgs(value0, value1, helper);
			RunEventScript(CustomServerModeSortEvent, EventNames.CustomServerModeSort, serverModeSortEventArgs);
			EventHandler<PivotGridCustomServerModeSortEventArgs> handler = (EventHandler<PivotGridCustomServerModeSortEventArgs>)this.Events[CustomServerModeSortEvent];
			if(handler != null)
				handler(this, serverModeSortEventArgs);
			return serverModeSortEventArgs.Result;
		}
		#endregion
		#region CustomGroupInterval event
		static readonly object CustomGroupIntervalEvent = new object();
		public event EventHandler<PivotCustomGroupIntervalEventArgs> CustomGroupInterval {
			add { this.Events.AddHandler(CustomGroupIntervalEvent, value); }
			remove { this.Events.RemoveHandler(CustomGroupIntervalEvent, value); }
		}
		protected virtual void RaiseCustomGroupInterval(PivotCustomGroupIntervalEventArgs e) {
			RunEventScript(CustomGroupIntervalEvent, EventNames.CustomGroupInterval, e);
			EventHandler<PivotCustomGroupIntervalEventArgs> handler = (EventHandler<PivotCustomGroupIntervalEventArgs>)this.Events[CustomGroupIntervalEvent];
			if(handler != null) handler(this, e);
		}
		#endregion
		#region CustomChartDataSourceData event
		static readonly object CustomChartDataSourceDataEvent = new object();
		public event EventHandler<PivotCustomChartDataSourceDataEventArgs> CustomChartDataSourceData {
			add { this.Events.AddHandler(CustomChartDataSourceDataEvent, value); }
			remove { this.Events.RemoveHandler(CustomChartDataSourceDataEvent, value); }
		}
		protected virtual void RaiseCustomChartDataSourceData(PivotCustomChartDataSourceDataEventArgs e) {
			RunEventScript(CustomChartDataSourceDataEvent, EventNames.CustomChartDataSourceData, e);
			EventHandler<PivotCustomChartDataSourceDataEventArgs> handler = (EventHandler<PivotCustomChartDataSourceDataEventArgs>)this.Events[CustomChartDataSourceDataEvent];
			if(handler != null) handler(this, e);
		}
		#endregion
		#region CustomChartDataSourceRows event
		static readonly object CustomChartDataSourceRowsEvent = new object();
		public event EventHandler<PivotCustomChartDataSourceRowsEventArgs> CustomChartDataSourceRows {
			add { this.Events.AddHandler(CustomChartDataSourceRowsEvent, value); }
			remove { this.Events.RemoveHandler(CustomChartDataSourceRowsEvent, value); }
		}
		protected virtual void RaiseCustomChartDataSourceRows(PivotCustomChartDataSourceRowsEventArgs e) {
			RunEventScript(CustomChartDataSourceRowsEvent, EventNames.CustomChartDataSourceRows, e);
			EventHandler<PivotCustomChartDataSourceRowsEventArgs> handler = (EventHandler<PivotCustomChartDataSourceRowsEventArgs>)this.Events[CustomChartDataSourceRowsEvent];
			if(handler != null) handler(this, e);
		}
		#endregion
		#region PrefilterCriteria event
		private static readonly object PrefilterCriteriaChangedEvent = new object();
		public event EventHandler PrefilterCriteriaChanged {
			add { this.Events.AddHandler(PrefilterCriteriaChangedEvent, value); }
			remove { this.Events.RemoveHandler(PrefilterCriteriaChangedEvent, value); }
		}
		void RaisePrefilterCriteriaChanged(EventArgs e) {
			RunEventScript(PrefilterCriteriaChangedEvent, EventNames.PrefilterCriteriaChanged, e);
			EventHandler handler = (EventHandler)this.Events[PrefilterCriteriaChangedEvent];
			if(handler != null) handler(this, e);
		}
		#endregion
		#region CustomCellDisplayText event
		static readonly object CustomCellDisplayTextEvent = new object();
		public event EventHandler<PivotCellDisplayTextEventArgs> CustomCellDisplayText {
			add { this.Events.AddHandler(CustomCellDisplayTextEvent, value); }
			remove { this.Events.RemoveHandler(CustomCellDisplayTextEvent, value); }
		}
		protected virtual void RaiseCustomCellDisplayText(PivotCellDisplayTextEventArgs e) {
			RunEventScript(CustomCellDisplayTextEvent, EventNames.CustomCellDisplayText, e);
			EventHandler<PivotCellDisplayTextEventArgs> handler = (EventHandler<PivotCellDisplayTextEventArgs>)this.Events[CustomCellDisplayTextEvent];
			if(handler != null) handler(this, e);
		}
		#endregion
		#region CustomCellValue event
		static readonly object CustomCellValueEvent = new object();
		public event EventHandler<PivotCellValueEventArgs> CustomCellValue {
			add { this.Events.AddHandler(CustomCellValueEvent, value); }
			remove { this.Events.RemoveHandler(CustomCellValueEvent, value); }
		}
		protected virtual void RaiseCustomCellValue(PivotCellValueEventArgs e) {
			RunEventScript(CustomCellValueEvent, EventNames.CustomCellValue, e);
			EventHandler<PivotCellValueEventArgs> handler = (EventHandler<PivotCellValueEventArgs>)this.Events[CustomCellValueEvent];
			if(handler != null) handler(this, e);
		}
		#endregion
		#region CustomColumnWidth event
		static readonly object CustomColumnWidthEvent = new object();
		public event EventHandler<PivotCustomColumnWidthEventArgs> CustomColumnWidth {
			add { this.Events.AddHandler(CustomColumnWidthEvent, value); }
			remove { this.Events.RemoveHandler(CustomColumnWidthEvent, value); }
		}
		protected virtual void RaiseCustomColumnWidth(PivotCustomColumnWidthEventArgs e) {
			RunEventScript(CustomColumnWidthEvent, EventNames.CustomColumnWidth, e);
			EventHandler<PivotCustomColumnWidthEventArgs> handler = (EventHandler<PivotCustomColumnWidthEventArgs>)this.Events[CustomColumnWidthEvent];
			if(handler != null) handler(this, e);
		}
		#endregion
		#region CustomRowHeight event
		static readonly object CustomRowHeightEvent = new object();
		public event EventHandler<PivotCustomRowHeightEventArgs> CustomRowHeight {
			add { this.Events.AddHandler(CustomRowHeightEvent, value); }
			remove { this.Events.RemoveHandler(CustomRowHeightEvent, value); }
		}
		protected virtual void RaiseCustomRowHeight(PivotCustomRowHeightEventArgs e) {
			RunEventScript(CustomRowHeightEvent, EventNames.CustomRowHeight, e);
			EventHandler<PivotCustomRowHeightEventArgs> handler = (EventHandler<PivotCustomRowHeightEventArgs>)this.Events[CustomRowHeightEvent];
			if(handler != null) handler(this, e);
		}
		#endregion
		#region FieldValueDisplayText event
		static readonly object FieldValueDisplayTextEvent = new object();
		public event EventHandler<PivotFieldDisplayTextEventArgs> FieldValueDisplayText {
			add { this.Events.AddHandler(FieldValueDisplayTextEvent, value); }
			remove { this.Events.RemoveHandler(FieldValueDisplayTextEvent, value); }
		}
		protected virtual void RaiseFieldValueDisplayText(PivotFieldDisplayTextEventArgs e) {
			RunEventScript(FieldValueDisplayTextEvent, EventNames.FieldValueDisplayText, e);
			EventHandler<PivotFieldDisplayTextEventArgs> handler = (EventHandler<PivotFieldDisplayTextEventArgs>)this.Events[FieldValueDisplayTextEvent];
			if(handler != null) handler(this, e);
		}
		#endregion
		#region CustomSummary event
		static readonly object CustomSummaryEvent = new object();
		public event EventHandler<PivotGridCustomSummaryEventArgs> CustomSummary {
			add { this.Events.AddHandler(CustomSummaryEvent, value); }
			remove { this.Events.RemoveHandler(CustomSummaryEvent, value); }
		}
		protected virtual void RaiseCustomSummary(PivotGridCustomSummaryEventArgs e) {
			RunEventScript(CustomSummaryEvent, EventNames.CustomSummary, e);
			EventHandler<PivotGridCustomSummaryEventArgs> handler = (EventHandler<PivotGridCustomSummaryEventArgs>)this.Events[CustomSummaryEvent];
			if(handler != null) handler(this, e);
		}
		#endregion
		#region IPivotGridPrinterOwner Members
		static readonly object PrintHeaderEvent = new object();
		static readonly object PrintFieldValueEvent = new object();
		static readonly object PrintCellEvent = new object();
		public event EventHandler<CustomExportHeaderEventArgs> PrintHeader {
			add { this.Events.AddHandler(PrintHeaderEvent, value); }
			remove { this.Events.RemoveHandler(PrintHeaderEvent, value); }
		}
		public event EventHandler<CustomExportFieldValueEventArgs> PrintFieldValue {
			add { this.Events.AddHandler(PrintFieldValueEvent, value); }
			remove { this.Events.RemoveHandler(PrintFieldValueEvent, value); }
		}
		public event EventHandler<CustomExportCellEventArgs> PrintCell {
			add { this.Events.AddHandler(PrintCellEvent, value); }
			remove { this.Events.RemoveHandler(PrintCellEvent, value); }
		}
		protected virtual bool RaisePrintHeader(ref IVisualBrick brick, PivotFieldItemBase fieldItem, IPivotPrintAppearance appearance, ref Rectangle rect) {
			CustomExportHeaderEventArgs e = new CustomExportHeaderEventArgs(brick, fieldItem, (XRAppearanceObject)appearance, Data.GetField(fieldItem), ref rect);
			bool scriptWasRun = RunEventScript(PrintHeaderEvent, EventNames.PrintHeader, e);
			EventHandler<CustomExportHeaderEventArgs> handler = (EventHandler<CustomExportHeaderEventArgs>)this.Events[PrintHeaderEvent];
			if(handler == null && !scriptWasRun)
				return false;
			if(handler != null) handler(this, e);
			appearance = e.Appearance;
			brick = e.Brick;
			return e.ApplyAppearanceToBrickStyle;
		}
		protected virtual bool RaisePrintFieldValue(ref IVisualBrick brick, PivotFieldValueItem item, IPivotPrintAppearance appearance, ref Rectangle rect) {
			CustomExportFieldValueEventArgs e = new CustomExportFieldValueEventArgs(brick, item, (XRAppearanceObject)appearance, ref rect);
			bool scriptWasRun = RunEventScript(PrintFieldValueEvent, EventNames.PrintFieldValue, e);
			EventHandler<CustomExportFieldValueEventArgs> handler = (EventHandler<CustomExportFieldValueEventArgs>)this.Events[PrintFieldValueEvent];
			if(handler == null && !scriptWasRun)
				return false;
			if(handler != null) handler(this, e);
			appearance = e.Appearance;
			brick = e.Brick;
			return e.ApplyAppearanceToBrickStyle;
		}
		protected virtual bool RaisePrintCell(ref IVisualBrick brick, PivotGridCellItem cellItem, IPivotPrintAppearance appearance, GraphicsUnit graphicsUnit, ref Rectangle rect) {
			CustomExportCellEventArgs e = new CustomExportCellEventArgs(brick, cellItem, (XRAppearanceObject)appearance, Data, printer, graphicsUnit, ref rect);
			bool scriptWasRun = RunEventScript(PrintCellEvent, EventNames.PrintCell, e);
			EventHandler<CustomExportCellEventArgs> handler = (EventHandler<CustomExportCellEventArgs>)this.Events[PrintCellEvent];
			if(handler == null && !scriptWasRun)
				return false;
			if(handler != null) handler(this, e);
			appearance = e.Appearance;
			brick = e.Brick;
			return e.ApplyAppearanceToBrickStyle;
		}
		bool IPivotGridPrinterOwner.CustomExportHeader(ref IVisualBrick brick, PivotFieldItemBase field, IPivotPrintAppearance appearance, ref Rectangle rect) {
			return RaisePrintHeader(ref brick, field, appearance, ref rect);
		}
		bool IPivotGridPrinterOwner.CustomExportFieldValue(ref IVisualBrick brick, PivotFieldValueItem item, IPivotPrintAppearance appearance, ref Rectangle rect) {
			return RaisePrintFieldValue(ref brick, item, appearance, ref rect);
		}
		bool IPivotGridPrinterOwner.CustomExportCell(ref IVisualBrick brick, PivotGridCellItem cellItem, IPivotPrintAppearance appearance, GraphicsUnit graphicsUnit, ref Rectangle rect) {
			return RaisePrintCell(ref brick, cellItem, appearance, graphicsUnit, ref rect);
		}
		#endregion
		#region IPivotGridEventsImplementor Members
		void IPivotGridEventsImplementorBase.AfterFieldValueChangeExpanded(PivotFieldValueItem item) {
		}
		void IPivotGridEventsImplementorBase.AfterFieldValueChangeNotExpanded(PivotFieldValueItem item, PivotGridFieldBase field) {
		}
		bool IPivotGridEventsImplementorBase.BeforeFieldValueChangeExpanded(PivotFieldValueItem item) {
			return true;
		}
		void IPivotGridEventsImplementorBase.BeginRefresh() {
		}
		void IPivotGridEventsImplementorBase.CalcCustomSummary(PivotGridFieldBase field, DevExpress.Data.PivotGrid.PivotCustomSummaryInfo customSummaryInfo) {
			PivotGridCustomSummaryEventArgs e = new PivotGridCustomSummaryEventArgs(Data, field as XRPivotGridField, customSummaryInfo);
			RaiseCustomSummary(e);
		}
		string IPivotGridEventsImplementorBase.CustomCellDisplayText(PivotGridCellItem cellItem) {
			PivotCellDisplayTextEventArgs e = new PivotCellDisplayTextEventArgs(cellItem);
			RaiseCustomCellDisplayText(e);
			return e.DisplayText;
		}
		object IPivotGridEventsImplementorBase.CustomCellValue(PivotGridCellItem cellItem) {
			PivotCellValueEventArgs e = new PivotCellValueEventArgs(cellItem);
			RaiseCustomCellValue(e);
			return e.Value;
		}
		object IPivotGridEventsImplementorBase.CustomGroupInterval(PivotGridFieldBase field, object value) {
			PivotCustomGroupIntervalEventArgs e = new PivotCustomGroupIntervalEventArgs((XRPivotGridField)field, value);
			RaiseCustomGroupInterval(e);
			return e.GroupValue;
		}
		object IPivotGridEventsImplementorBase.CustomChartDataSourceData(PivotChartItemType itemType, PivotChartItemDataMember itemDataMember, PivotFieldValueItem fieldValueItem, PivotGridCellItem cellItem, object value) {
			PivotCustomChartDataSourceDataEventArgs e = new PivotCustomChartDataSourceDataEventArgs(itemType, itemDataMember, fieldValueItem, cellItem, value);
			RaiseCustomChartDataSourceData(e);
			return e.Value;
		}
		void IPivotGridEventsImplementorBase.CustomChartDataSourceRows(IList<PivotChartDataSourceRowBase> rows) {
			PivotCustomChartDataSourceRowsEventArgs e = new PivotCustomChartDataSourceRowsEventArgs(this.ChartDataSource, rows);
			RaiseCustomChartDataSourceRows(e);
		}
		bool IPivotGridEventsImplementorBase.CustomFilterPopupItems(PivotGridFilterItems items) {
			return false;
		}
		bool IPivotGridEventsImplementorBase.CustomFieldValueCells(PivotVisualItemsBase items) {
			return RaiseCustomFieldValueCells(items);
		}		
		void IPivotGridEventsImplementorBase.DataSourceChanged() {
		}
		void IPivotGridEventsImplementorBase.EndRefresh() {
		}
		void IPivotGridEventsImplementorBase.GroupFilterChanged(PivotGridGroup group) {
		}
		void IPivotGridEventsImplementorBase.FieldAreaChanged(PivotGridFieldBase field) {
		}
		bool IPivotGridEventsImplementorBase.FieldAreaChanging(PivotGridFieldBase field, PivotArea newArea, int newAreaIndex) {
			return true;
		}
		void IPivotGridEventsImplementorBase.FieldExpandedInFieldsGroupChanged(PivotGridFieldBase field) {
		}
		bool IPivotGridEventsImplementorBase.FieldFilterChanging(PivotGridFieldBase field, PivotFilterType filterType, bool showBlanks, IList<object> values) {
			return false;
		}
		void IPivotGridEventsImplementorBase.FieldFilterChanged(PivotGridFieldBase field) {
		}
		string IPivotGridEventsImplementorBase.FieldValueDisplayText(PivotGridFieldBase field, object value) {
			PivotFieldDisplayTextEventArgs e = new PivotFieldDisplayTextEventArgs((XRPivotGridField)field, value);
			RaiseFieldValueDisplayText(e);
			return e.DisplayText;
		}
		string IPivotGridEventsImplementorBase.FieldValueDisplayText(PivotGridFieldBase field, IOLAPMember member) {
			PivotFieldDisplayTextEventArgs e = new PivotFieldDisplayTextEventArgs((XRPivotGridField)field, member);
			RaiseFieldValueDisplayText(e);
			return e.DisplayText;
		}
		string IPivotGridEventsImplementorBase.FieldValueDisplayText(PivotFieldValueItem item, string defaultText) {
			PivotFieldDisplayTextEventArgs e = new PivotFieldDisplayTextEventArgs(item, defaultText);
			RaiseFieldValueDisplayText(e);
			return e.DisplayText;
		}
		void IPivotGridEventsImplementorBase.FieldWidthChanged(PivotGridFieldBase field) {
		}
		void IPivotGridEventsImplementorBase.FieldUnboundExpressionChanged(PivotGridFieldBase field) { }
		void IPivotGridEventsImplementorBase.FieldAreaIndexChanged(PivotGridFieldBase field) { }
		void IPivotGridEventsImplementorBase.FieldVisibleChanged(PivotGridFieldBase field) { }
		void IPivotGridEventsImplementorBase.FieldPropertyChanged(PivotGridFieldBase field, PivotFieldPropertyName propertyName) { }
		int? IPivotGridEventsImplementorBase.GetCustomSortRows(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotGridFieldBase field, PivotSortOrder sortOrder) {
			return RaiseCustomFieldSort(listSourceRow1, listSourceRow2, value1, value2, field, sortOrder);
		}
		int? IPivotGridEventsImplementorBase.QuerySorting(DevExpress.PivotGrid.QueryMode.Sorting.IQueryMemberProvider value0, DevExpress.PivotGrid.QueryMode.Sorting.IQueryMemberProvider value1, PivotGridFieldBase field, DevExpress.PivotGrid.QueryMode.Sorting.ICustomSortHelper helper) {
			return RaiseCustomServerModeSort(value0, value1, field, helper);
		}
		object IPivotGridEventsImplementorBase.GetUnboundValue(PivotGridFieldBase field, int listSourceRowIndex, object value) {
			CustomFieldDataEventArgs e = new CustomFieldDataEventArgs(Data, field as XRPivotGridField, listSourceRowIndex, value);
			RaiseCustomUnboundColumnData(e);
			return e.Value;
		}
		void IPivotGridEventsImplementorBase.LayoutChanged() {
		}
		void IPivotGridEventsImplementorBase.OLAPQueryTimeout() {
		}
		bool IPivotGridEventsImplementorBase.QueryException(Exception ex) {
			return false;
		}
		void IPivotGridEventsImplementorBase.PrefilterCriteriaChanged() {
			RaisePrefilterCriteriaChanged(new EventArgs());
		}
		#endregion
		#region IComponentLoading Members
		bool IComponentLoading.IsLoading {
			get { return this.ReportIsLoading; }
		}
		#endregion
		#region ChartDataSource
		#region ITypedList Members
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
			return ChartDataSource.GetItemProperties(listAccessors);
		}
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
			return this.Name;
		}
		#endregion
		#region IBindingList Members
		void IBindingList.AddIndex(PropertyDescriptor property) { ((IBindingList)ChartDataSource).AddIndex(property); }
		object IBindingList.AddNew() { return ((IBindingList)ChartDataSource).AddNew(); }
		bool IBindingList.AllowEdit { get { return ((IBindingList)ChartDataSource).AllowEdit; } }
		bool IBindingList.AllowNew { get { return ((IBindingList)ChartDataSource).AllowNew; } }
		bool IBindingList.AllowRemove { get { return ((IBindingList)ChartDataSource).AllowRemove; } }
		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction) {
			((IBindingList)ChartDataSource).ApplySort(property, direction);
		}
		int IBindingList.Find(PropertyDescriptor property, object key) { return ((IBindingList)ChartDataSource).Find(property, key); }
		bool IBindingList.IsSorted { get { return ((IBindingList)ChartDataSource).IsSorted; } }
		event ListChangedEventHandler IBindingList.ListChanged {
			add { ((IBindingList)ChartDataSource).ListChanged += value; }
			remove {
				SuspendDataAndExec(delegate() {
					if(Data != null)
						((IBindingList)ChartDataSource).ListChanged -= value;
				});
			}
		}
		void IBindingList.RemoveIndex(PropertyDescriptor property) { ((IBindingList)ChartDataSource).RemoveIndex(property); }
		void IBindingList.RemoveSort() { ((IBindingList)ChartDataSource).RemoveSort(); }
		ListSortDirection IBindingList.SortDirection { get { return ((IBindingList)ChartDataSource).SortDirection; } }
		PropertyDescriptor IBindingList.SortProperty { get { return ((IBindingList)ChartDataSource).SortProperty; } }
		bool IBindingList.SupportsChangeNotification { get { return ChartDataSource != null ? ((IBindingList)ChartDataSource).SupportsChangeNotification : false; } }
		bool IBindingList.SupportsSearching { get { return ((IBindingList)ChartDataSource).SupportsSearching; } }
		bool IBindingList.SupportsSorting { get { return ((IBindingList)ChartDataSource).SupportsSorting; } }
		#endregion
		#region IList Members
		int IList.Add(object value) { return ChartDataSource.Add(value); }
		void IList.Clear() { ChartDataSource.Clear(); }
		bool IList.Contains(object value) { return ChartDataSource.Contains(value); }
		int IList.IndexOf(object value) { return ChartDataSource.IndexOf(value); }
		void IList.Insert(int index, object value) { ChartDataSource.Insert(index, value); }
		bool IList.IsFixedSize { get { return ChartDataSource.IsFixedSize; } }
		bool IList.IsReadOnly { get { return ChartDataSource.IsReadOnly; } }
		void IList.Remove(object value) { ChartDataSource.Remove(value); }
		void IList.RemoveAt(int index) { ChartDataSource.RemoveAt(index); }
		object IList.this[int index] {
			get { return ChartDataSource[index]; }
			set { ChartDataSource[index] = value; }
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) { ChartDataSource.CopyTo(array, index); }
		int ICollection.Count { get { return ChartDataSource.Count; } }
		bool ICollection.IsSynchronized { get { return ChartDataSource.IsSynchronized; } }
		object ICollection.SyncRoot { get { return ChartDataSource.SyncRoot; } }
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable)ChartDataSource).GetEnumerator();
		}
		#endregion
		#region IPivotGrid Members
		event DataChangedEventHandler IChartDataSource.DataChanged {
			add { ((IChartDataSource)ChartDataSource).DataChanged += value; }
			remove {
				SuspendDataAndExec(delegate() {
					if(Data != null && ChartDataSource != null)
						((IChartDataSource)ChartDataSource).DataChanged -= value;
				});
			}
		}
		IList<string> IPivotGrid.ArgumentColumnNames {
			get { return ((IPivotGrid)ChartDataSource).ArgumentColumnNames; }
		}
		IList<string> IPivotGrid.ValueColumnNames {
			get { return ((IPivotGrid)ChartDataSource).ValueColumnNames; }
		}
		void SuspendDataAndExec(Action0 action) {
			try {
				loading = true;
				action();
			} finally {
				loading = false;
			}
		}
		string IChartDataSource.SeriesDataMember { get { return ((IPivotGrid)ChartDataSource).SeriesDataMember; } }
		string IChartDataSource.ArgumentDataMember { get { return ((IPivotGrid)ChartDataSource).ArgumentDataMember; } }
		string IChartDataSource.ValueDataMember { get { return ((IPivotGrid)ChartDataSource).ValueDataMember; } }
		DateTimeMeasureUnitNative? IChartDataSource.DateTimeArgumentMeasureUnit { 
			get { return ((IPivotGrid)ChartDataSource).DateTimeArgumentMeasureUnit; } 
		}
		bool IPivotGrid.RetrieveDataByColumns {
			get { return ((IPivotGrid)ChartDataSource).RetrieveDataByColumns; }
			set { ((IPivotGrid)ChartDataSource).RetrieveDataByColumns = value; }
		}
		bool IPivotGrid.RetrieveEmptyCells {
			get { return ((IPivotGrid)ChartDataSource).RetrieveEmptyCells; }
			set { ((IPivotGrid)ChartDataSource).RetrieveEmptyCells = value; }
		}
		DefaultBoolean IPivotGrid.RetrieveFieldValuesAsText {
			get { return ((IPivotGrid)ChartDataSource).RetrieveFieldValuesAsText; }
			set { ((IPivotGrid)ChartDataSource).RetrieveFieldValuesAsText = value; }
		}
		bool IPivotGrid.SelectionSupported { get { return false; } }
		bool IPivotGrid.SelectionOnly {
			get { return ((IPivotGrid)ChartDataSource).SelectionOnly; }
			set {}
		}
		bool IPivotGrid.SinglePageSupported { get { return ((IPivotGrid)ChartDataSource).SinglePageSupported; } }
		bool IPivotGrid.SinglePageOnly {
			get { return ((IPivotGrid)ChartDataSource).SinglePageOnly; }
			set { ((IPivotGrid)ChartDataSource).SinglePageOnly = value; }
		}
		bool IPivotGrid.RetrieveColumnTotals {
			get { return ((IPivotGrid)ChartDataSource).RetrieveColumnTotals; }
			set { ((IPivotGrid)ChartDataSource).RetrieveColumnTotals = value; }
		}
		bool IPivotGrid.RetrieveColumnGrandTotals {
			get { return ((IPivotGrid)ChartDataSource).RetrieveColumnGrandTotals; }
			set { ((IPivotGrid)ChartDataSource).RetrieveColumnGrandTotals = value; }
		}
		bool IPivotGrid.RetrieveColumnCustomTotals {
			get { return ((IPivotGrid)ChartDataSource).RetrieveColumnCustomTotals; }
			set { ((IPivotGrid)ChartDataSource).RetrieveColumnCustomTotals = value; }
		}
		bool IPivotGrid.RetrieveRowTotals {
			get { return ((IPivotGrid)ChartDataSource).RetrieveRowTotals; }
			set { ((IPivotGrid)ChartDataSource).RetrieveRowTotals = value; }
		}
		bool IPivotGrid.RetrieveRowGrandTotals {
			get { return ((IPivotGrid)ChartDataSource).RetrieveRowGrandTotals; }
			set { ((IPivotGrid)ChartDataSource).RetrieveRowGrandTotals = value; }
		}
		bool IPivotGrid.RetrieveRowCustomTotals {
			get { return ((IPivotGrid)ChartDataSource).RetrieveRowCustomTotals; }
			set { ((IPivotGrid)ChartDataSource).RetrieveRowCustomTotals = value; }
		}
		bool IPivotGrid.RetrieveDateTimeValuesAsMiddleValues {
			get { return ((IPivotGrid)ChartDataSource).RetrieveDateTimeValuesAsMiddleValues; }
			set { ((IPivotGrid)ChartDataSource).RetrieveDateTimeValuesAsMiddleValues = value; }
		}
		int IPivotGrid.MaxAllowedSeriesCount {
			get { return ((IPivotGrid)ChartDataSource).MaxAllowedSeriesCount; }
			set { ((IPivotGrid)ChartDataSource).MaxAllowedSeriesCount = value; }
		}
		int IPivotGrid.MaxAllowedPointCountInSeries {
			get { return ((IPivotGrid)ChartDataSource).MaxAllowedPointCountInSeries; }
			set { ((IPivotGrid)ChartDataSource).MaxAllowedPointCountInSeries = value; }
		}
		int IPivotGrid.UpdateDelay { 
			get { return ((IPivotGrid)ChartDataSource).UpdateDelay; } 
			set {} 
		}
		int IPivotGrid.AvailableSeriesCount { get { return ((IPivotGrid)ChartDataSource).AvailableSeriesCount; } }
		IDictionary<object, int> IPivotGrid.AvailablePointCountInSeries { 
			get { return ((IPivotGrid)ChartDataSource).AvailablePointCountInSeries; } 
		}
		IDictionary<DateTime, DateTimeMeasureUnitNative> IPivotGrid.DateTimeMeasureUnitByArgument { 
			get { return ((IPivotGrid)ChartDataSource).DateTimeMeasureUnitByArgument; } 
		}
		void IPivotGrid.LockListChanged() {
			((IPivotGrid)ChartDataSource).LockListChanged();
		}
		void IPivotGrid.UnlockListChanged() {
			((IPivotGrid)ChartDataSource).UnlockListChanged();
		}
		#endregion
		#endregion
		#region Serialization
		PivotGridSerializationHelper<XRPivotGridField> serializationHelper;
		protected PivotGridSerializationHelper<XRPivotGridField> SerializationHelper {
			get {
				if(serializationHelper == null)
					serializationHelper = new PivotGridSerializationHelper<XRPivotGridField>(Data);
				return serializationHelper;
			}
		}
		#region IXtraSerializable
		protected override void OnStartDeserializing(LayoutAllowEventArgs e) {
			if(!e.Allow) return;
			Data.SetIsDeserializing(true);
			Data.BeginUpdate();
		}
		protected override void OnEndDeserializing(string restoredVersion) {
			Data.OnDeserializationComplete();
			Data.SetIsDeserializing(false);
			Data.EndUpdate();
		}
		#endregion
		#region IXtraSerializableLayoutEx Members
		bool IXtraSerializableLayoutEx.AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			return OnAllowSerializationProperty(options, propertyName, id);
		}
		void IXtraSerializableLayoutEx.ResetProperties(OptionsLayoutBase options) {
			OnResetSerializationProperties(options);
		}
		protected virtual bool OnAllowSerializationProperty(OptionsLayoutBase options, string propertyName, int id) {
			PivotGridOptionsLayout opts = options as PivotGridOptionsLayout;
			if(opts == null) return true;
			if(opts.StoreAllOptions || opts.Columns.StoreAllOptions) return true;
			switch(id) {
				case LayoutIdAppearance:
					return opts.StoreAppearance || opts.Columns.StoreAppearance;
				case LayoutIdData:
					return opts.StoreDataSettings;
				case LayoutIdOptionsView:
					return opts.StoreVisualOptions;
				case LayoutIdLayout:
					return opts.Columns.StoreLayout;
			}
			return true;
		}
		protected virtual void OnResetSerializationProperties(OptionsLayoutBase options) {
			PivotGridOptionsLayout opts = options as PivotGridOptionsLayout;
			if(opts == null || opts.StoreAppearance || opts.Columns.StoreAppearance || opts.StoreAllOptions) {
				Appearance.Reset();
			}
			if(opts == null || opts.StoreVisualOptions || opts.StoreAllOptions) {
				OptionsView.Reset();
			}
			if(opts == null || (opts.ResetOptions & PivotGridResetOptions.OptionsPrint) != 0)
				OptionsPrint.Reset();
			if(opts == null || (opts.ResetOptions & PivotGridResetOptions.OptionsDataField) != 0)
				OptionsDataField.Reset();
			if(opts == null) return;
			if((opts.ResetOptions & PivotGridResetOptions.OptionsBehavior) != 0)
				OptionsBehavior.Reset();
			if((opts.ResetOptions & PivotGridResetOptions.OptionsChartDataSource) != 0)
				OptionsChartDataSource.Reset();
			if((opts.ResetOptions & PivotGridResetOptions.OptionsCustomization) != 0)
				OptionsCustomization.Reset();
			if((opts.ResetOptions & PivotGridResetOptions.OptionsData) != 0)
				OptionsData.Reset();
			if((opts.ResetOptions & PivotGridResetOptions.OptionsOLAP) != 0)
				OptionsOLAP.Reset();
		}
		#endregion
		protected override object CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if (propertyName == XtraReportsSerializationNames.PivotFieldsPropertyName)
				return SerializationHelper.DeserializeField(e);
			if (propertyName == XtraReportsSerializationNames.PivotGroupsPropertyName)
				return SerializationHelper.DeserializeGroup(e);
			return base.CreateCollectionItem(propertyName, e);
		}
		#region IXtraSupportDeserializeCollection Members
		void IXtraSupportDeserializeCollection.AfterDeserializeCollection(string propertyName, XtraItemEventArgs e) { }
		void IXtraSupportDeserializeCollection.BeforeDeserializeCollection(string propertyName, XtraItemEventArgs e) { }
		bool IXtraSupportDeserializeCollection.ClearCollection(string propertyName, XtraItemEventArgs e) {
			if (propertyName == XtraReportsSerializationNames.PivotFieldsPropertyName)
				return SerializationHelper.ClearFields(e);
			if (propertyName == XtraReportsSerializationNames.PivotGroupsPropertyName)
				return SerializationHelper.ClearGroups(e);
			return false;
		}
		#endregion
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraFindFieldsItem(XtraItemEventArgs e) {
			return SerializationHelper.FindField(e);
		}
		#endregion
		#region IPrintable Members
		void IPrintable.AcceptChanges() {
			Printer.AcceptChanges();
		}
		bool IPrintable.CreatesIntersectedBricks {
			get { return false; }
		}
		bool IPrintable.HasPropertyEditor() {
			return false;
		}
		UserControl IPrintable.PropertyEditorControl {
			get { return null; }
		}
		void IPrintable.RejectChanges() {
			Printer.RejectChanges();
		}
		void IPrintable.ShowHelp() { }
		bool IPrintable.SupportsHelp() {
			return false;
		}
		#endregion
		#region IBasePrintable Members
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
			Printer.CreateArea(areaName, graph);
		}
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) { }
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
			Printer.Initialize(ps, link);
		}
		#endregion
	}
}
namespace DevExpress.XtraReports.UI {
	public class XRAppearanceObject : PrintAppearanceObject {
		[EditorBrowsable(EditorBrowsableState.Never)]
		public class OptionsCompatability {
			[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
			public bool UseBackColor { get { return true; } set { } }
			[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
			public bool UseBorderColor { get { return true; } set { } }
			[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
			public bool UseFont { get { return true; } set { } }
			[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
			public bool UseForeColor { get { return true; } set { } }
			[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
			public bool UseImage { get { return true; } set { } }
			[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
			public bool UseTextOptions { get { return true; } set { } }
		}
		internal static DevExpress.Utils.HorzAlignment ToHorzAlignment(TextAlignment textAlignment) {
			if((textAlignment & (TextAlignment.TopCenter | TextAlignment.MiddleCenter | TextAlignment.BottomCenter)) > 0)
				return DevExpress.Utils.HorzAlignment.Center;
			if((textAlignment & (TextAlignment.TopRight | TextAlignment.MiddleRight | TextAlignment.BottomRight)) > 0)
				return DevExpress.Utils.HorzAlignment.Far;
			return DevExpress.Utils.HorzAlignment.Near;
		}
		internal static DevExpress.Utils.VertAlignment ToVertAlignment(TextAlignment textAlignment) {
			if((textAlignment & (TextAlignment.MiddleCenter | TextAlignment.MiddleJustify | TextAlignment.MiddleLeft | TextAlignment.MiddleRight)) > 0)
				return DevExpress.Utils.VertAlignment.Center;
			if((textAlignment & (TextAlignment.BottomCenter | TextAlignment.BottomJustify | TextAlignment.BottomLeft | TextAlignment.BottomRight)) > 0)
				return DevExpress.Utils.VertAlignment.Bottom;
			return DevExpress.Utils.VertAlignment.Top;
		}
		OptionsCompatability options = new OptionsCompatability();
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public OptionsCompatability Options {
			get {
				return options;
			}
		}
		void ResetTextOptions() { TextOptions.Reset(); }
		bool ShouldSerializeTextOptions() { return DevExpress.Utils.Design.UniversalTypeConverter.ShouldSerializeObject(TextOptions); }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		]
		public override PrintTextOptions TextOptions {
			get {
				return base.TextOptions;
			}
		}
		[
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRAppearanceObject.TextHorizontalAlignment"),
		]
		public override HorzAlignment TextHorizontalAlignment {
			get { return base.TextHorizontalAlignment; }
			set { base.TextHorizontalAlignment = value; }
		}
		void ResetTextHorizontalAlignment() { TextHorizontalAlignment = HorzAlignment.Default; }
		bool ShouldSerializeTextHorizontalAlignment() { return TextHorizontalAlignment != HorzAlignment.Default; }
		internal bool IsSetTextHorizontalAlignment { get { return ShouldSerializeTextHorizontalAlignment(); } }
		[
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRAppearanceObject.TextVerticalAlignment"),
		]
		public override VertAlignment TextVerticalAlignment {
			get { return base.TextVerticalAlignment; }
			set { base.TextVerticalAlignment = value; }
		}
		void ResetTextVerticalAlignment() { TextVerticalAlignment = VertAlignment.Default; }
		bool ShouldSerializeTextVerticalAlignment() { return TextVerticalAlignment != VertAlignment.Default; }
		internal bool IsSetTextVerticalAlignment { get { return ShouldSerializeTextVerticalAlignment(); } }
		[
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRAppearanceObject.WordWrap"),
		]
		public override bool WordWrap {
			get { return base.WordWrap; }
			set { base.WordWrap = value; }
		}
		void ResetWordWrap() { WordWrap = false; }
		bool ShouldSerializeWordWrap() { return WordWrap != false; }
		[
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRAppearanceObject.Trimming"),
		]
		public override StringTrimming Trimming {
			get { return base.Trimming; }
			set { base.Trimming = value; }
		}
		void ResetTrimming() { Trimming = StringTrimming.None; }
		bool ShouldSerializeTrimming() { return Trimming != StringTrimming.None; }
		public override void Reset() {
			base.Reset();
			ResetTextHorizontalAlignment();
			ResetTextVerticalAlignment();
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete("Use XRAppearanceObject.TextHorizontalAlignment and XRAppearanceObject.TextVerticalAlignment properties instead."),
		]
		public TextAlignment TextAlignment {
			get { return DevExpress.XtraPrinting.TextAlignment.MiddleLeft; }
			set {
				TextHorizontalAlignment = ToHorzAlignment(value);
				TextVerticalAlignment = ToVertAlignment(value);
			}
		}
		protected override Font InnerDefaultFont { get { return BrickStyle.DefaultFont; } }
		protected override PrintAppearanceObject CreateNewInstance() {
			return new XRAppearanceObject();
		}
	}
}
namespace DevExpress.XtraReports.UI.PivotGrid {
	class XRPivotDataController : PivotDataController {
		public event EventHandler<CancelEventArgs> Refresh;
		public IComponent Component { get; set; }
		protected override void DoRefresh(bool useRowsKeeper) {
			if(Refresh != null) {
				CancelEventArgs e = new CancelEventArgs(false);
				Refresh(this, e);
				if(e.Cancel) return;
			}
			base.DoRefresh(useRowsKeeper);
		}
	}
	class XRPivotGridNativeDataSource : PivotGridNativeDataSource {
		public XRPivotGridNativeDataSource(PivotGridData data)
			: base(data) {
		}
		protected override PivotDataController CreateDataController() {
			return new XRPivotDataController();
		}
	}
	class XRPivotFieldItem : PivotFieldItemBase, IPrintAppearanceOwner {
		XRPivotGridAppearances appearance;
		public XRPivotFieldItem(PivotGridData data, PivotGroupItemCollection groupItems, XRPivotGridField field)
			: base(data, groupItems, field) {
			if(field.IsRowTreeField)
				return;
			this.appearance = field.Appearance;
		}
		public XRPivotGridAppearances Appearance {
			get { return appearance; }
		}
		#region IPrintAppearanceOwner Members
		PrintAppearanceObject IPrintAppearanceOwner.Cell {
			get { return appearance.Cell; }
		}
		PrintAppearanceObject IPrintAppearanceOwner.CustomTotalCell {
			get { return appearance.CustomTotalCell; }
		}
		PrintAppearanceObject IPrintAppearanceOwner.Field {
			get { return appearance.FieldHeader; }
		}
		PrintAppearanceObject IPrintAppearanceOwner.FieldValue {
			get { return appearance.FieldValue; }
		}
		PrintAppearanceObject IPrintAppearanceOwner.FieldValueGrandTotal {
			get { return appearance.FieldValueGrandTotal; }
		}
		PrintAppearanceObject IPrintAppearanceOwner.FieldValueTotal {
			get { return appearance.FieldValueTotal; }
		}
		PrintAppearanceObject IPrintAppearanceOwner.GrandTotalCell {
			get { return appearance.GrandTotalCell; }
		}
		PrintAppearanceObject IPrintAppearanceOwner.TotalCell {
			get { return appearance.TotalCell; }
		}
		#endregion
	}
	class XRVisualItems : PivotVisualItemsBase {
		public XRVisualItems(PivotGridData data)
			: base(data) {
		}
		protected override PivotFieldItemBase CreateFieldItem(PivotGridData data, PivotGroupItemCollection groupItems, PivotGridFieldBase field) {
			XRPivotGridField xrField = field as XRPivotGridField;
			return xrField == null ? base.CreateFieldItem(data, groupItems, field) : new XRPivotFieldItem(data, groupItems, xrField);
		}
	}
	[DXHelpExclude(true), DXBrowsable(false)]
	public class XRPivotGridData : PivotGridData {
		XRPivotGrid xrPivotGrid;
		public XRPivotGrid XRPivotGrid {
			get { return xrPivotGrid; }
		}
		public override bool IsControlReady {
			get {
				if(XRPivotGrid != null)
					return !XRPivotGrid.ReportIsLoading;
				return false;
			}
		}
		public override bool IsLoading {
			get {
				return xrPivotGrid.ReportIsLoading || base.IsLoading;
			}
		}
		public override bool IsDesignMode {
			get {
				return xrPivotGrid.Site != null && xrPivotGrid.Site.DesignMode;
			}
		}
		public XRPivotGridData(XRPivotGrid xrPivotGrid)
			: base() {
			this.xrPivotGrid = xrPivotGrid;
			this.EventsImplementor = xrPivotGrid as IPivotGridEventsImplementorBase;
		}
		protected override PrefilterBase CreatePrefilter() {
			return new XRPrefilter(this);
		}
		protected override IPivotListDataSource CreateListDataSource() {
			XRPivotGridNativeDataSource result = new XRPivotGridNativeDataSource(this);
			((XRPivotDataController)result.DataController).Refresh += OnRefresh;
			((XRPivotDataController)result.DataController).BeforePopulateColumns += OnBeforePopulateColumns;
			return result;
		}
		void OnBeforePopulateColumns(object sender, EventArgs e) {
			if(xrPivotGrid != null && xrPivotGrid.Site == null)
				PopulateCalculatedProperties(((XRPivotGridNativeDataSource)ListDataSource).DataController, xrPivotGrid.GetCalculatedFields());
		}
		void OnRefresh(object sender, CancelEventArgs e) {
			e.Cancel = xrPivotGrid != null && xrPivotGrid.Site != null;
		}
		void PopulateCalculatedProperties(DataControllerBase dataController, IList<PropertyDescriptor> properties) {
			if(properties != null) {
				foreach(PropertyDescriptor property in properties) {
					if(dataController.Columns != null)
						dataController.Columns.Add(new LightDataColumnInfo(property, ((PivotDataController)dataController).CaseSensitive));
				}
			}
		}
		protected override PivotGridOptionsPrint CreatePivotGridOptionsPrint() {
			return new XRPivotGridOptionsPrint(OnOptionsPrintChanged);
		}
		protected override PivotGridOptionsViewBase CreateOptionsView() {
			return new XRPivotGridOptionsView(OnOptionsViewChanged);
		}
		protected override void OnOptionsViewChanged(object sender, EventArgs e) {
			base.OnOptionsViewChanged(sender, e);
			this.OptionsPrint.FilterSeparatorBarPadding = ((XRPivotGridOptionsView)OptionsView).FilterSeparatorBarPadding * 2 + 1;
		}
		protected override PivotGridOptionsChartDataSourceBase CreateOptionsChartDataSource() {
			return new XRPivotGridOptionsChartDataSource(this);
		}
		protected override PivotGridFieldBase CreateDataField() {
			return new XRPivotGridField(this);
		}
		protected override PivotGridFieldCollectionBase CreateFieldCollection() {
			return new XRPivotGridFieldCollection(this);
		}
		protected virtual XRPivotGridAppearances CreatePivotGridAppearancesPrint() {
			return new XRPivotGridAppearances();
		}
		protected override PivotChartDataSourceBase CreateChartDataSource() {
			return new XRPivotChartDataSource(this);
		}
		protected override void OnCalcCustomSummary(PivotGridFieldBase field, DevExpress.Data.PivotGrid.PivotCustomSummaryInfo customSummaryInfo) {
			if(!IsDesignMode)
				base.OnCalcCustomSummary(field, customSummaryInfo);
		}
		internal PivotChartDataSourceBase ChartDataSource { get { return ChartDataSourceInternal; } }
		protected override PivotVisualItemsBase CreateVisualItems() {
			return new XRVisualItems(this);
		}
	}
	public class XRPivotGridAppearances : PrintAppearance, IXtraSerializable2 {
		XRAppearanceObject filterSeparator;
		XRAppearanceObject headerGroupLine;
		public XRPivotGridAppearances() {
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridAppearancesCell"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.Cell"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public new XRAppearanceObject Cell { get { return (XRAppearanceObject)base.Cell; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridAppearancesFieldHeader"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.FieldHeader"),
	   DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public new XRAppearanceObject FieldHeader { get { return (XRAppearanceObject)base.FieldHeader; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridAppearancesTotalCell"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.TotalCell"),
	   DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public new XRAppearanceObject TotalCell { get { return (XRAppearanceObject)base.TotalCell; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridAppearancesGrandTotalCell"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.GrandTotalCell"),
	   DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public new XRAppearanceObject GrandTotalCell { get { return (XRAppearanceObject)base.GrandTotalCell; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridAppearancesCustomTotalCell"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.CustomTotalCell"),
	   DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public new XRAppearanceObject CustomTotalCell { get { return (XRAppearanceObject)base.CustomTotalCell; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridAppearancesFieldValue"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.FieldValue"),
	   DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public new XRAppearanceObject FieldValue { get { return (XRAppearanceObject)base.FieldValue; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridAppearancesFieldValueTotal"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.FieldValueTotal"),
	   DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public new XRAppearanceObject FieldValueTotal { get { return (XRAppearanceObject)base.FieldValueTotal; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridAppearancesFieldValueGrandTotal"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.FieldValueGrandTotal"),
	   DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public new XRAppearanceObject FieldValueGrandTotal { get { return (XRAppearanceObject)base.FieldValueGrandTotal; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridAppearancesLines"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.Lines"),
	   DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public new XRAppearanceObject Lines { get { return (XRAppearanceObject)base.Lines; } }
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		ObsoleteAttribute("This property is no longer used.")
		]
		public XRAppearanceObject FilterSeparator { 
			get { 
				if(filterSeparator == null)
					filterSeparator = new XRAppearanceObject();
				return filterSeparator;
			} 
		}
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		ObsoleteAttribute("This property is no longer used.")
		]
		public XRAppearanceObject HeaderGroupLine { 
			get {
				if(headerGroupLine == null)
					headerGroupLine = new XRAppearanceObject();
				return headerGroupLine; 
			} 
		}
		protected override PrintAppearanceObject CreateAppearanceObject() {
			return new XRAppearanceObject();
		}
		IDictionary<string, PrintAppearanceObject> GetAppearances() {
			Dictionary<string, PrintAppearanceObject> result = new Dictionary<string, PrintAppearanceObject>();
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(PrintAppearance), new Attribute[] { new XtraSerializableProperty(XtraSerializationVisibility.Content) });
			foreach(PropertyDescriptor prop in props)
				result.Add(prop.Name, (PrintAppearanceObject)prop.GetValue(this));
			return result;
		} 
		XtraPropertyInfo[] IXtraSerializable2.Serialize() {
			List<XtraPropertyInfo> list = new List<XtraPropertyInfo>();
			foreach(KeyValuePair<string, PrintAppearanceObject> entry in GetAppearances()) {
				XtraPropertyInfo pInfo = new XtraPropertyInfo(entry.Key, typeof(string), entry.Key, true);
				SerializeHelper helper = new SerializeHelper();
				pInfo.ChildProperties.AddRange(helper.SerializeObject(entry.Value, XtraSerializationFlags.DefaultValue, OptionsLayoutBase.FullLayout));
				if(pInfo.ChildProperties.Count > 0) list.Add(pInfo);
			}
			return list.ToArray();
		}
		void IXtraSerializable2.Deserialize(IList list) {
			Reset();
			IDictionary<string, PrintAppearanceObject> appearances = GetAppearances();
			foreach(XtraPropertyInfo info in list) {
				PrintAppearanceObject app;
				if(appearances.TryGetValue(info.Name, out app)) {
					DeserializeHelper helper = new DeserializeHelper();
					helper.DeserializeObject(app, info.ChildProperties, OptionsLayoutBase.FullLayout);
				}
			}
		}
		#region serializing 
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
		public virtual void SaveLayoutToStream(Stream stream) {
			SaveLayoutCore(new XmlXtraSerializer(), stream);
		}
		public virtual void RestoreLayoutFromStream(Stream stream) {
			RestoreLayoutCore(new XmlXtraSerializer(), stream);
		}
		protected virtual void SaveLayoutCore(XtraSerializer serializer, object path) {
			Stream stream = path as Stream;
			if(stream != null)
				serializer.SerializeObject(this, stream, "Appearances");
			else
				serializer.SerializeObject(this, path.ToString(), "Appearances");
		}
		protected virtual void RestoreLayoutCore(XtraSerializer serializer, object path) {
			Stream stream = path as Stream;
			if(stream != null)
				serializer.DeserializeObject(this, stream, "Appearances");
			else
				serializer.DeserializeObject(this, path.ToString(), "Appearances");
		}
		#endregion
	}
	[
	TypeConverter(typeof(DevExpress.Utils.Design.CollectionTypeConverter)),
	]
	public class XRPivotGridFieldCollection : PivotGridFieldCollectionBase {
		[EditorBrowsable(EditorBrowsableState.Always)]
		public new XRPivotGridField this[int index] { get { return (XRPivotGridField)base[index]; } }
		[EditorBrowsable(EditorBrowsableState.Always)]
		public new XRPivotGridField this[string fieldName] { get { return (XRPivotGridField)base[fieldName]; } }
		public XRPivotGridFieldCollection(PivotGridData data)
			: base(data) {
		}
		protected override PivotGridFieldBase CreateField(string fieldName, PivotArea area) {
			return new XRPivotGridField(fieldName, area);
		}
		protected override void OnRemoveComplete(int index, object obj) {
			base.OnRemoveComplete(index, obj);
			DisposeField((PivotGridFieldBase)obj);
		}
		protected override void DisposeField(PivotGridFieldBase field) {
			IContainer componentContainer = field.Site != null ? field.Site.Container : null;
			if(componentContainer != null)
				componentContainer.Remove(field);
			base.DisposeField(field);
		}
		public void AddRange(XRPivotGridField[] fields) {
			foreach(XRPivotGridField field in fields) {
				AddCore(field);
			}
		}
	}
	public interface IPivotGridDataContainer {
		PivotGridData Data { get; }
	}
	public class XRPivotGridFieldAppearances : XRPivotGridAppearances {
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public new XRAppearanceObject Lines { get { return (XRAppearanceObject)base.Lines; } }
	}
	public class XRPivotGridField : PivotGridFieldBase, IPivotGridDataContainer {
		XRPivotGridFieldAppearances appearance;
		void ResetAppearance() { Appearance.Reset(); }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[
		SRCategory(ReportStringId.CatAppearance), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIdAppearance), 
		Localizable(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), "DevExpress.XtraPivotGrid.PivotGridField.Appearance")
		]
		public XRPivotGridFieldAppearances Appearance {
			get {
				if(appearance == null)
					appearance = new XRPivotGridFieldAppearances();
				return appearance;
			}
		}
		PivotGridData IPivotGridDataContainer.Data {
			get { return Data; }
		}
		[
		SRCategory(ReportStringId.CatData),
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridFieldFieldName"),
#endif
		Editor("DevExpress.XtraReports.Design.PivotGrid.XRPivotFieldNameEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(UITypeEditor))
		]
		public override string FieldName {
			get {
				return base.FieldName;
			}
			set {
				base.FieldName = value;
			}
		}
		[
		SRCategory(ReportStringId.CatOptions),
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridFieldOptions"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), "DevExpress.XtraPivotGrid.PivotGridFieldBase.Options")
		]
		public new XRPivotGridFieldOptions Options { get { return (XRPivotGridFieldOptions)base.Options; } }
		public XRPivotGridField() {
		}
		public XRPivotGridField(PivotGridData data) : base(data) {
		}
		public XRPivotGridField(string fieldName, PivotArea area)
			: base(fieldName, area) {
		}
		[
		SRCategory(ReportStringId.CatBehavior), 
		 Browsable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, true, 0, XtraSerializationFlags.DefaultValue),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), "DevExpress.XtraPivotGrid.PivotGridFieldBase.CustomTotals"),
		TypeConverter(typeof(DevExpress.Utils.Design.CollectionTypeConverter))
		]
		public new XRPivotGridCustomTotalCollection CustomTotals {
			get { return (XRPivotGridCustomTotalCollection)base.CustomTotals; }
		}
		protected override PivotGridFieldOptions CreateOptions(PivotOptionsChangedEventHandler eventHandler, string name) {
			return new XRPivotGridFieldOptions(eventHandler, this, "Options");
		}
		protected override PivotGridCustomTotalCollectionBase CreateCustomTotals() {
			return new XRPivotGridCustomTotalCollection(this);
		}
		public override string ToString() {
			string displayText = base.ToString();
			return !string.IsNullOrEmpty(displayText) ? displayText : Name;
		}
		public void BestFit() {
			if(!(Data is XRPivotGridData))
				throw new InvalidOperationException("The PivotGridFieldBase.Data property has an invalid value");
			((XRPivotGridData)Data).XRPivotGrid.BestFit(this);
		}
	}
	public class XRPivotGridCustomTotalCollection : PivotGridCustomTotalCollectionBase {
		public XRPivotGridCustomTotalCollection() { }
		public XRPivotGridCustomTotalCollection(XRPivotGridField field) : base(field) { }
		public XRPivotGridCustomTotalCollection(XRPivotGridCustomTotal[] totals)
			: base(totals) { }
		public new XRPivotGridCustomTotal this[int index] { 
			get {
				ValidateItem(index);
				return (XRPivotGridCustomTotal)InnerList[index];
			}
		}
		internal void ValidateItem(int index) {
			if(InnerList[index] is XRPivotGridCustomTotal)
				return;
			XRPivotGridCustomTotal item = new XRPivotGridCustomTotal();
			((PivotGridCustomTotalBase)InnerList[index]).CloneTo(item);
			InnerList[index] = item;
		}
		[Browsable(false)]
		public new XRPivotGridField Field { get { return (XRPivotGridField)base.Field; } }
		public virtual void AddRange(PivotGridCustomTotalBase[] customSummaries) {
			foreach(PivotGridCustomTotalBase customTotal in customSummaries) {
				AddCore(customTotal);
			}
		}
		protected override PivotGridCustomTotalBase CreateCustomTotal() {
			return new XRPivotGridCustomTotal();
		}
	}
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverterEx))]
	public class XRPivotGridCustomTotal : PivotGridCustomTotalBase {
		XRAppearanceObject appearance;
		[
		Obsolete("This property is now obsolete."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public XRAppearanceObject Appearance { 
			get { 
				if(appearance == null)
					this.appearance = new XRAppearanceObject();
				return appearance;
			}
		}
		public XRPivotGridCustomTotal()
			: this(PivotSummaryType.Sum) {
		}
		public XRPivotGridCustomTotal(PivotSummaryType summaryType)
			: base(summaryType) {
		}
	}
	public class XRPivotGridFieldOptions : PivotGridFieldOptions {
		public XRPivotGridFieldOptions(PivotOptionsChangedEventHandler optionsChanged, DevExpress.WebUtils.IViewBagOwner viewBagOwner, string objectPath)
			: base(optionsChanged, viewBagOwner, objectPath) {
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public override bool ShowInCustomizationForm {
			get { return base.ShowInCustomizationForm; }
			set { base.ShowInCustomizationForm = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public override bool ShowInPrefilter {
			get { return base.ShowInPrefilter; }
			set { base.ShowInPrefilter = value; }
		}
	}
	public class XRPivotGridStyles : DevExpress.XtraReports.UI.XRControl.XRControlStyles {
		XRPivotGrid PivotGrid { get { return (XRPivotGrid)control; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override XRControlStyle EvenStyle {
			get { return null; }
			set { }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override XRControlStyle OddStyle {
			get { return null; }
			set { }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override XRControlStyle Style {
			get { return null; }
			set { }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridStylesCellStyle"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridStyles.CellStyle"),
		DefaultValue(null),
		Editor("DevExpress.XtraReports.Design.XRControlStyleEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlStyleConverter)),
		]
		public virtual XRControlStyle CellStyle {
			get { return PivotGrid.CellStyleCore; }
			set { PivotGrid.CellStyleCore = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridStylesCustomTotalCellStyle"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridStyles.CustomTotalCellStyle"),
		DefaultValue(null),
		Editor("DevExpress.XtraReports.Design.XRControlStyleEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlStyleConverter)),
		]
		public virtual XRControlStyle CustomTotalCellStyle {
			get { return PivotGrid.CustomTotalCellStyleCore; }
			set { PivotGrid.CustomTotalCellStyleCore = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridStylesFieldHeaderStyle"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridStyles.FieldHeaderStyle"),
		DefaultValue(null),
		Editor("DevExpress.XtraReports.Design.XRControlStyleEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlStyleConverter)),
		]
		public virtual XRControlStyle FieldHeaderStyle {
			get { return PivotGrid.FieldHeaderStyleCore; }
			set { PivotGrid.FieldHeaderStyleCore = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridStylesFieldValueStyle"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridStyles.FieldValueStyle"),
		DefaultValue(null),
		Editor("DevExpress.XtraReports.Design.XRControlStyleEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlStyleConverter)),
		]
		public virtual XRControlStyle FieldValueStyle {
			get { return PivotGrid.FieldValueStyleCore; }
			set { PivotGrid.FieldValueStyleCore = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridStylesFieldValueGrandTotalStyle"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridStyles.FieldValueGrandTotalStyle"),
		DefaultValue(null),
		Editor("DevExpress.XtraReports.Design.XRControlStyleEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlStyleConverter)),
		]
		public virtual XRControlStyle FieldValueGrandTotalStyle {
			get { return PivotGrid.FieldValueGrandTotalStyleCore; }
			set { PivotGrid.FieldValueGrandTotalStyleCore = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridStylesFieldValueTotalStyle"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridStyles.FieldValueTotalStyle"),
		DefaultValue(null),
		Editor("DevExpress.XtraReports.Design.XRControlStyleEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlStyleConverter)),
		]
		public virtual XRControlStyle FieldValueTotalStyle {
			get { return PivotGrid.FieldValueTotalStyleCore; }
			set { PivotGrid.FieldValueTotalStyleCore = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridStylesFilterSeparatorStyle"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridStyles.FilterSeparatorStyle"),
		DefaultValue(null),
		Editor("DevExpress.XtraReports.Design.XRControlStyleEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlStyleConverter)),
		]
		public virtual XRControlStyle FilterSeparatorStyle {
			get { return PivotGrid.FilterSeparatorStyleCore; }
			set { PivotGrid.FilterSeparatorStyleCore = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridStylesGrandTotalCellStyle"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridStyles.GrandTotalCellStyle"),
		DefaultValue(null),
		Editor("DevExpress.XtraReports.Design.XRControlStyleEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlStyleConverter)),
		]
		public virtual XRControlStyle GrandTotalCellStyle {
			get { return PivotGrid.GrandTotalCellStyleCore; }
			set { PivotGrid.GrandTotalCellStyleCore = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridStylesHeaderGroupLineStyle"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridStyles.HeaderGroupLineStyle"),
		DefaultValue(null),
		Editor("DevExpress.XtraReports.Design.XRControlStyleEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlStyleConverter)),
		]
		public virtual XRControlStyle HeaderGroupLineStyle {
			get { return PivotGrid.HeaderGroupLineStyleCore; }
			set { PivotGrid.HeaderGroupLineStyleCore = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridStylesLinesStyle"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridStyles.LinesStyle"),
		DefaultValue(null),
		Editor("DevExpress.XtraReports.Design.XRControlStyleEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlStyleConverter)),
		]
		public virtual XRControlStyle LinesStyle {
			get { return PivotGrid.LinesStyleCore; }
			set { PivotGrid.LinesStyleCore = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPivotGridStylesTotalCellStyle"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridStyles.TotalCellStyle"),
		DefaultValue(null),
		Editor("DevExpress.XtraReports.Design.XRControlStyleEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlStyleConverter)),
		]
		public virtual XRControlStyle TotalCellStyle {
			get { return PivotGrid.TotalCellStyleCore; }
			set { PivotGrid.TotalCellStyleCore = value; }
		}
		public XRPivotGridStyles(XRPivotGrid control)
			: base(control) {
		}
	}
	public class XRPivotGridOptionsPrint : PivotGridOptionsPrint {
		public XRPivotGridOptionsPrint(EventHandler optionsChanged)
			: base(optionsChanged) { } 
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public override PivotGridPageSettings PageSettings { get { return base.PageSettings; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public override bool UsePrintAppearance { get { return true; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public override VerticalContentSplitting VerticalContentSplitting {
			get { return base.VerticalContentSplitting; }
			set { }
		}
	}
	public class XRPivotGridOptionsView : PivotGridOptionsViewBase {
		int filterSeparatorBarPadding = 1;
		public XRPivotGridOptionsView(EventHandler optionsChanged)
			: base(optionsChanged) { }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public int FilterSeparatorBarPadding {
			get { return filterSeparatorBarPadding; }
			set {
				if(filterSeparatorBarPadding != value) {
					filterSeparatorBarPadding = value;
					OnOptionsChanged();
				}
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public new bool DrawFocusedCellRect {
			get { return base.DrawFocusedCellRect; }
			set { }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public new int HeaderHeightOffset {
			get { return base.HeaderHeightOffset; }
			set { }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public new int HeaderWidthOffset {
			get { return base.HeaderWidthOffset; }
			set { }
		}
	}
	public class XRPivotGridOptionsChartDataSource : PivotGridOptionsChartDataSource {
		public XRPivotGridOptionsChartDataSource(PivotGridData data) : base(data) {
			base.SelectionOnlyInternal = false;
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		]
		public override bool SelectionOnly {
			get { return base.SelectionOnly; }
			set { ; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false), 
		]
		public override int UpdateDelay {
			get { return base.UpdateDelay; }
			set { ; }
		}
	}
	public class FieldValueCell : FieldValueCellBase<XRPivotGridField> {
		public FieldValueCell(PivotFieldValueItem item)
			: base(item) {
		}
	}
	public class PivotCustomFieldValueCellsEventArgs : PivotCustomFieldValueCellsEventArgsBase<XRPivotGridField, FieldValueCell> {
		internal PivotCustomFieldValueCellsEventArgs(PivotVisualItemsBase items) : base(items) { }
		protected override FieldValueCell GetCellCore(PivotFieldValueItem item) {
			if(item == null) return null;
			return new FieldValueCell(item);
		}
	}
	public class CustomFieldDataEventArgs : CustomFieldDataEventArgsBase<XRPivotGridField> {
		public CustomFieldDataEventArgs(PivotGridData data, XRPivotGridField field, int listSourceRow, object _value) :
			base(data, field, listSourceRow, _value) {
		}
	}
	public class PivotGridCustomFieldSortEventArgs : PivotGridCustomFieldSortEventArgsBase<XRPivotGridField> {
		public PivotGridCustomFieldSortEventArgs(PivotGridData data, XRPivotGridField field)
			: base(data, field) {
		}
	}
	public class PivotGridCustomServerModeSortEventArgs : CustomServerModeSortEventArgsBase<XRPivotGridField> {
		public PivotGridCustomServerModeSortEventArgs(XRPivotGridField field) : base(field) { }
		internal new void SetArgs(DevExpress.PivotGrid.QueryMode.Sorting.IQueryMemberProvider value0, DevExpress.PivotGrid.QueryMode.Sorting.IQueryMemberProvider value1, DevExpress.PivotGrid.QueryMode.Sorting.ICustomSortHelper helper) {
			base.SetArgs(value0, value1, helper);
		}
	}
	public class PivotCustomGroupIntervalEventArgs : PivotCustomGroupIntervalEventArgsBase<XRPivotGridField> {
		public PivotCustomGroupIntervalEventArgs(XRPivotGridField field, object value)
			: base(field, value) {
		}
	}
	public class PivotCustomChartDataSourceDataEventArgs : EventArgs {
		PivotChartItemType itemType;
		PivotChartItemDataMember itemDataMember;
		PivotFieldValueEventArgs fieldValueInfo;
		PivotCellValueEventArgs cellInfo;
		object _value;
		public PivotCustomChartDataSourceDataEventArgs(PivotChartItemType itemType, PivotChartItemDataMember itemDataMember, PivotFieldValueItem fieldValueItem, PivotGridCellItem cellItem, object value) {
			this.itemType = itemType;
			this.itemDataMember = itemDataMember;
			this.fieldValueInfo = null;
			this.cellInfo = null;
			switch(ItemType) {
				case PivotChartItemType.ColumnItem:
				case PivotChartItemType.RowItem:
					this.fieldValueInfo = new PivotFieldValueEventArgs(fieldValueItem);
					this.cellInfo = null;
					break;
				case PivotChartItemType.CellItem:
					this.fieldValueInfo = null;
					this.cellInfo = new PivotCellValueEventArgs(cellItem);
					break;
			}
			this._value = value;
		}
		public PivotChartItemType ItemType { get { return itemType; } }
		public PivotChartItemDataMember ItemDataMember { get { return itemDataMember; } }
		public PivotFieldValueEventArgs FieldValueInfo { get { return fieldValueInfo; } }
		public PivotCellValueEventArgs CellInfo { get { return cellInfo; } }
		public object Value {
			get { return _value; }
			set { _value = value; }
		}
	}
	public class PivotCustomChartDataSourceRowsEventArgs : EventArgs {
		readonly PivotChartDataSourceBase ds;
		readonly IList<PivotChartDataSourceRow> rows;
		internal PivotCustomChartDataSourceRowsEventArgs(PivotChartDataSourceBase ds, IList<PivotChartDataSourceRowBase> rows) {
			this.ds = ds;
			this.rows = new PivotChartDataSourceRowBaseListWrapper<PivotChartDataSourceRow>(rows);
		}
		public IList<PivotChartDataSourceRow> Rows {
			get { return rows; }
		}
		public PivotChartDataSourceRow CreateRow(object series, object argument, object value) {
			return new PivotChartDataSourceRow(ds)
			{
				Series = series,
				Argument = argument,
				Value = value
			};
		}
	}
	public class XRPivotChartDataSource : PivotChartDataSourceBase {
		public XRPivotChartDataSource(PivotGridData data) 
			: base(data) {
		}
		protected override PivotChartDataSourceRowBase CreateDataSourceRow(Point point) {
			return new PivotChartDataSourceRow(this, point);
		}
	}
	public class PivotChartDataSourceRow : PivotChartDataSourceRowBase {
		public PivotChartDataSourceRow(PivotChartDataSourceBase ds)
			: base(ds) {
		}
		public PivotChartDataSourceRow(PivotChartDataSourceBase ds, Point cell)
			: base(ds, cell) {
		}
	}
	public class PivotCustomColumnWidthEventArgs : PivotFieldValueEventArgs {
		int width;
		public PivotCustomColumnWidthEventArgs(PivotFieldValueItem item, int width)
			: base(item) {
			this.width = width;
		}
		public int ColumnWidth {
			get { return width; }
			set { width = value; }
		}
		public int ColumnIndex {
			get { return MinIndex; }
		}
		public int RowCount { get { return Item.Data.VisualItems.RowCount; } }
		public object GetColumnCellValue(int rowIndex) {
			return GetCellValue(ColumnIndex, rowIndex);
		}
	}
	public class PivotCustomRowHeightEventArgs : PivotFieldValueEventArgs {
		int height;
		public PivotCustomRowHeightEventArgs(PivotFieldValueItem item, int height)
			: base(item) {
			this.height = height;
		}
		public int RowHeight {
			get { return height; }
			set { height = value; }
		}
		public int RowIndex {
			get { return MinIndex; }
		}
		public int ColumnCount { get { return Item.Data.VisualItems.ColumnCount; } }
		public object GetRowCellValue(int columnIndex) {
			return GetCellValue(columnIndex, RowIndex);
		}
	}
	public class PivotCellBaseEventArgs : EventArgs {
		PivotGridCellItem cellItem;
		public PivotCellBaseEventArgs(PivotGridCellItem cellItem) {
			this.cellItem = cellItem;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PivotGridCellItem Item { get { return cellItem; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PivotGridData Data { get { return Item.Data; } }
		public XRPivotGridField DataField { get { return (XRPivotGridField)Data.GetField(Item.DataField); } }
		public int ColumnIndex { get { return Item.ColumnIndex; } }
		public int RowIndex { get { return Item.RowIndex; } }
		public int ColumnFieldIndex { get { return Item.ColumnFieldIndex; } }
		public int RowFieldIndex { get { return Item.RowFieldIndex; } }
		public XRPivotGridField ColumnField { get { return (XRPivotGridField)Data.GetField(Item.ColumnField); } }
		public XRPivotGridField RowField { get { return (XRPivotGridField)Data.GetField(Item.RowField); } }
		public object Value { get { return Item.Value; } }
		public PivotSummaryType SummaryType { get { return Item.SummaryType; } }
		public PivotSummaryValue SummaryValue { get { return Data.GetCellSummaryValue(ColumnFieldIndex, RowFieldIndex, DataField); } }
		public PivotGridValueType ColumnValueType { get { return Item.ColumnValueType; } }
		public PivotGridValueType RowValueType { get { return Item.RowValueType; } }
		public PivotGridCustomTotalBase ColumnCustomTotal { get { return Item.ColumnCustomTotal; } }
		public PivotGridCustomTotalBase RowCustomTotal { get { return Item.RowCustomTotal; } }
		public PivotDrillDownDataSource CreateDrillDownDataSource() {
			return Data.GetDrillDownDataSource(ColumnFieldIndex, RowFieldIndex, Item.DataIndex);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int maxRowCount, List<string> customColumns) {
			return Data.GetQueryModeDrillDownDataSource(ColumnFieldIndex, RowFieldIndex, Item.DataIndex, maxRowCount,
				customColumns);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(List<string> customColumns) {
			return CreateDrillDownDataSource(-1, customColumns);
		}
		[Obsolete("The CreateOLAPDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateOLAPDrillDownDataSource(int maxRowCount, List<string> customColumns) {
			return CreateDrillDownDataSource(maxRowCount, customColumns);
		}
		[Obsolete("The CreateOLAPDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateOLAPDrillDownDataSource(List<string> customColumns) {
			return CreateDrillDownDataSource(customColumns);
		}
		[Obsolete("The CreateServerModeDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateServerModeDrillDownDataSource(int maxRowCount, List<string> customColumns) {
			return CreateDrillDownDataSource(maxRowCount, customColumns);
		}
		[Obsolete("The CreateServerModeDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateServerModeDrillDownDataSource(List<string> customColumns) {
			return CreateDrillDownDataSource(customColumns);
		}
		public PivotSummaryDataSource CreateSummaryDataSource() {
			return Data.CreateSummaryDataSource(ColumnFieldIndex, RowFieldIndex);
		}
		public object GetFieldValue(XRPivotGridField field) {
			return Item.GetFieldValue(Data.GetFieldItem(field));
		}
		public object GetFieldValue(XRPivotGridField field, int cellIndex) {
			if(field == null)
				throw new ArgumentNullException("field");
			if(cellIndex < 0)
				throw new ArgumentOutOfRangeException("cellIndex");
			return Data.VisualItems.GetFieldValue(field, cellIndex);
		}
		protected PivotFieldItemBase GetFieldItem(XRPivotGridField field) {
			return Data.GetFieldItem(field);
		}
		public bool IsOthersFieldValue(XRPivotGridField field) {
			return Item.IsOthersFieldValue(GetFieldItem(field));
		}
		public bool IsFieldValueExpanded(XRPivotGridField field) {
			return Item.IsFieldValueExpanded(GetFieldItem(field));
		}
		public bool IsFieldValueRetrievable(XRPivotGridField field) {
			return Item.IsFieldValueRetrievable(GetFieldItem(field));
		}
		public XRPivotGridField[] GetColumnFields() {
			return GetFields(PivotArea.ColumnArea, Data.GetColumnLevel(ColumnFieldIndex) + 1);
		}
		public XRPivotGridField[] GetRowFields() {
			return GetFields(PivotArea.RowArea, Data.GetRowLevel(RowFieldIndex) + 1);
		}
		XRPivotGridField[] GetFields(PivotArea area, int fieldCount) {
			if(fieldCount <= 0 || fieldCount > Data.GetFieldCountByArea(area)) return new XRPivotGridField[0];
			XRPivotGridField[] fields = new XRPivotGridField[fieldCount];
			for(int i = 0; i < fields.Length; i++)
				fields[i] = Data.GetFieldByArea(area, i) as XRPivotGridField;
			return fields;
		}
		public object GetCellValue(XRPivotGridField dataField) {
			return Data.GetCellValue(ColumnFieldIndex, RowFieldIndex, dataField);
		}
		public object GetCellValue(object[] columnValues, object[] rowValues, XRPivotGridField dataField) {
			return Data.GetCellValue(columnValues, rowValues, dataField);
		}
		public object GetCellValue(int columnIndex, int rowIndex) {
			return Data.VisualItems.GetCellValue(columnIndex, rowIndex);
		}
		public object GetPrevRowCellValue(XRPivotGridField dataField) {
			return Data.GetNextOrPrevRowCellValue(ColumnFieldIndex, RowFieldIndex, dataField, false);
		}
		public object GetNextRowCellValue(XRPivotGridField dataField) {
			return Data.GetNextOrPrevRowCellValue(ColumnFieldIndex, RowFieldIndex, dataField, true);
		}
		public object GetPrevColumnCellValue(XRPivotGridField dataField) {
			return Data.GetNextOrPrevColumnCellValue(ColumnFieldIndex, RowFieldIndex, dataField, false);
		}
		public object GetNextColumnCellValue(XRPivotGridField dataField) {
			return Data.GetNextOrPrevColumnCellValue(ColumnFieldIndex, RowFieldIndex, dataField, true);
		}
		public object GetColumnGrandTotal(XRPivotGridField dataField) {
			return Data.GetCellValue(-1, RowFieldIndex, dataField);
		}
		public object GetColumnGrandTotal(object[] rowValues, XRPivotGridField dataField) {
			return Data.GetCellValue(null, rowValues, dataField);
		}
		public object GetRowGrandTotal(XRPivotGridField dataField) {
			return Data.GetCellValue(ColumnFieldIndex, -1, dataField);
		}
		public object GetRowGrandTotal(object[] columnValues, XRPivotGridField dataField) {
			return Data.GetCellValue(columnValues, null, dataField);
		}
		public object GetGrandTotal(XRPivotGridField dataField) {
			return Data.GetCellValue(-1, -1, dataField);
		}
	}
	public class PivotCellDisplayTextEventArgs : PivotCellBaseEventArgs {
		string displayText;
		public PivotCellDisplayTextEventArgs(PivotGridCellItem cellItem)
			: base(cellItem) {
			this.displayText = Item.Text;
		}
		public string DisplayText { get { return displayText; } set { displayText = value; } }
	}
	public class PivotCellValueEventArgs : PivotCellBaseEventArgs {
		object value;
		public PivotCellValueEventArgs(PivotGridCellItem cellItem)
			: base(cellItem) {
			this.value = Item.Value;
		}
		public new object Value {
			get { return this.value; }
			set { this.value = value; }
		}
	}
	public class PivotFieldValueEventArgs : PivotFieldValueEventArgsBase<XRPivotGridField> {
		public PivotFieldValueEventArgs(PivotFieldValueItem item)
			: base(item) {
		}
		public PivotFieldValueEventArgs(XRPivotGridField field)
			: base(field) {
		}
	}
	public class PivotFieldDisplayTextEventArgs : PivotFieldDisplayTextEventArgsBase<XRPivotGridField> {
		public PivotFieldDisplayTextEventArgs(PivotFieldValueItem item, string defaultText)
			: base(item, defaultText) {
		}
		public PivotFieldDisplayTextEventArgs(XRPivotGridField field, IOLAPMember member)
			: base(field, member) {
		}
		public PivotFieldDisplayTextEventArgs(XRPivotGridField field, object value)
			: base(field, value) {
		}
	}
	public class CustomExportFieldValueEventArgs : CustomExportFieldValueEventArgsBase<XRPivotGridField> {
		XRAppearanceObject appearance;
		public CustomExportFieldValueEventArgs(IVisualBrick brick, PivotFieldValueItem fieldValueItem, XRAppearanceObject appearance, ref Rectangle rect)
			: base(brick, fieldValueItem, ref rect) {
			this.appearance = appearance;
		}
		public XRAppearanceObject Appearance {
			get { return appearance; }
			set {
				if(value == null)
					return;
				appearance = value;
			}
		}
	}
	public class CustomExportHeaderEventArgs : CustomExportHeaderEventArgsBase<XRPivotGridField> {
		XRAppearanceObject appearance;
		public CustomExportHeaderEventArgs(IVisualBrick brick, PivotFieldItemBase fieldItem, XRAppearanceObject appearance, PivotGridFieldBase field, ref Rectangle rect)
			: base(brick, fieldItem, field, ref rect) {
			this.appearance = appearance;
		}
		public XRAppearanceObject Appearance {
			get { return appearance; }
			set {
				if(value == null) return;
				appearance = value;
			}
		}
	}
	public class CustomExportCellEventArgs : CustomExportCellEventArgsBase {
		XRAppearanceObject appearance;
		PivotGridData data;
		public CustomExportCellEventArgs(IVisualBrick brick, PivotGridCellItem cellItem,
				XRAppearanceObject appearance, PivotGridData data, PivotGridPrinterBase printer, GraphicsUnit graphicsUnit, ref Rectangle rect)
			: base(brick, cellItem, graphicsUnit, appearance, printer, ref rect) {
			this.appearance = appearance;
			this.data = data;
		}
		public XRAppearanceObject Appearance {
			get { return appearance; }
			set {
				if(value == null) return;
				appearance = value;
			}
		}
		public XRPivotGridField ColumnField { get { return (XRPivotGridField)data.GetField(CellItem.ColumnField); } }
		public XRPivotGridField RowField { get { return (XRPivotGridField)data.GetField(CellItem.RowField); } }
		public XRPivotGridField DataField { get { return (XRPivotGridField)data.GetField(CellItem.DataField); } }
	}
	public class PivotGridCustomSummaryEventArgs : PivotGridCustomSummaryEventArgsBase<XRPivotGridField> {
		public PivotGridCustomSummaryEventArgs(PivotGridData data, XRPivotGridField field, PivotCustomSummaryInfo customSummaryInfo)
			: base(data, field, customSummaryInfo) {
		}
	}
	[
	TypeConverter(typeof(DevExpress.XtraPrinting.Native.LocalizableObjectConverter)),
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.PivotGrid.XRPrefilter"),
	]
	public class XRPrefilter : PrefilterBase {
		[
		Editor("DevExpress.XtraReports.Design.PivotCriteriaEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(UITypeEditor)),
		]
		public override DevExpress.Data.Filtering.CriteriaOperator Criteria {
			get { return base.Criteria; }
			set { base.Criteria = value; }
		}
		bool ShouldSerializeCriteria() {
			return false;
		}
		[XtraSerializableProperty]
		public override string CriteriaString {
			get { return base.CriteriaString; }
			set { base.CriteriaString = value; }
		}
		public XRPrefilter(PivotGridData owner)
			: base(owner) { 
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public PivotGridData Data {
			get {
				return (PivotGridData)base.Owner;
			}
		}
	}
}
