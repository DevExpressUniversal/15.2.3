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
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Browsing;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.DrillDown;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraReports.Configuration;
using DevExpress.XtraReports.Data;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraReports.Native.DrillDown;
using DevExpress.XtraReports.Native.LayoutView;
using DevExpress.XtraReports.Native.Navigation;
using DevExpress.XtraReports.Native.Parameters;
using DevExpress.XtraReports.Native.Presenters;
using DevExpress.XtraReports.Native.Printing;
using DevExpress.XtraReports.Native.Summary;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.UserDesigner;
namespace DevExpress.XtraReports.UI {
	[
#if !DEBUG
#endif // DEBUG
Designer("DevExpress.XtraReports.Design._ReportDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull, typeof(IRootDesigner)),
Designer("DevExpress.XtraReports.Design._ComponentReportDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
XRDesigner("DevExpress.XtraReports.Design.ReportDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(IRootDesigner)),
XRDesigner("DevExpress.XtraReports.Design.ComponentReportDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
TypeConverter(typeof(DevExpress.XtraReports.Design.XtraReportConverter)),
DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XtraReport", "Report"),
InitAssemblyResolver,
RootClass,
ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "XtraReport.bmp"),
System.ComponentModel.Design.Serialization.DesignerSerializer(
	"DevExpress.XtraReports.Design.ReportCodeDomSerializer, " + AssemblyInfo.SRAssemblyReportsDesignFull,
	"System.ComponentModel.Design.Serialization.TypeCodeDomSerializer, " + AttributeConstants.SystemDesignAssembly),
	DevExpress.Utils.Serializing.Helpers.SerializationContext(typeof(XtraReportsSerializationContext)),
	DocumentSource(true),
]
	public class XtraReport : XtraReportBase, ISupportInitialize, IServiceProvider, IReport, IParameterSupplier, IRootXmlObject {
		#region inner classes
		private class XtraReportSerializationSurrogate : ISerializationSurrogate {
			ReportStorage reportStorage;
			XtraReport report;
			public XtraReportSerializationSurrogate(XtraReport report) {
				this.report = report;
			}
			[System.Security.SecurityCritical]
			void ISerializationSurrogate.GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
				reportStorage = new ReportStorage(report);
				((ISerializable)reportStorage).GetObjectData(info, context);
			}
			[System.Security.SecurityCritical]
			object ISerializationSurrogate.SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
				try {
					reportStorage = new ReportStorage(info, context);
					obj = report;
					return null;
				} catch(SerializationException e) {
					throw (e);
				}
			}
			public void InitReportStructure() {
				if(reportStorage != null)
					reportStorage.InitReportStructure(report);
			}
		}
		#endregion
		const float pageBuildFactor = 0.3f;
		const float imageFactor = 0.3f;
		const float textFactor = 0.5f;
		const float htmFactor = 0.7f;
		const float pdfFactor = 2.0f;
		const float progressRangeBase = 50f;
		public const PaperKind DefaultPaperKind = PaperKind.Letter;
		public readonly static Size DefaultPageSize = new Size(850, 1100); 
		private readonly static Margins DefaultMargins = new Margins(100, 100, 100, 100);
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static HorizontalContentSplitting DefaultHorizontalContentSplitting = HorizontalContentSplitting.Exact;
		NonSerializableComponentsHelperBase nonSerializableComponentsHelper;
		internal NonSerializableComponentsHelperBase GetNonSerializableComponentsHelper() {
			if(nonSerializableComponentsHelper != null)
				return nonSerializableComponentsHelper;
			if(this.DesignMode)
				return this.Site.GetService(typeof(NonSerializableComponentsHelperBase)) as NonSerializableComponentsHelperBase;
			return new NonSerializableComponentsHelper();
		}
		#region static
		internal static string SerializationNamespace {
			get { return "XtraReportSerialization"; }
		}
		[Obsolete("Use the DevExpress.XtraReports.Configuration.Settings.ShowUserFriendlyNamesInUserDesigner property instead."),
		EditorBrowsable(EditorBrowsableState.Never)]
		public static bool ShowUserFriendlyNamesInUserDesigner {
			get;
			set;
		}
		static EventHandler<FilterComponentPropertiesEventArgs> FilterComponentPropertiesEvent;
		public static event EventHandler<FilterComponentPropertiesEventArgs> FilterComponentProperties {
			add { FilterComponentPropertiesEvent = System.Delegate.Combine(FilterComponentPropertiesEvent, value) as EventHandler<FilterComponentPropertiesEventArgs>; }
			remove { FilterComponentPropertiesEvent = System.Delegate.Remove(FilterComponentPropertiesEvent, value) as EventHandler<FilterComponentPropertiesEventArgs>; }
		}
		internal static void RaiseFilterComponentProperties(XtraReport report, IComponent component, IDictionary properties) {
			if(FilterComponentPropertiesEvent != null) FilterComponentPropertiesEvent(report, new FilterComponentPropertiesEventArgs(component, properties));
			if(component is XRControl)
				RaiseFilterControlProperties(report, new FilterControlPropertiesEventArgs(component as XRControl, properties));
		}
		#region obsolete
		static FilterControlPropertiesEventHandler onFilterControlProperties;
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use XtraReport.FilterComponentProperties event instead."),
		]
		public static event FilterControlPropertiesEventHandler FilterControlProperties {
			add { onFilterControlProperties = System.Delegate.Combine(onFilterControlProperties, value) as FilterControlPropertiesEventHandler; }
			remove { onFilterControlProperties = System.Delegate.Remove(onFilterControlProperties, value) as FilterControlPropertiesEventHandler; }
		}
		static void RaiseFilterControlProperties(XtraReport report, FilterControlPropertiesEventArgs e) {
			if(onFilterControlProperties != null) onFilterControlProperties(report, e);
		}
		#endregion
		static XtraReport() {
			DevExpress.Utils.Design.DXAssemblyResolverEx.Init();
			BrickResolver.EnsureStaticConstructor();
			DrillDownKey.EnsureStaticConstructor();
		}
		public static void EnsureStaticConstructor() {
		}
		private static System.Windows.Forms.Form CreateDummyForm() {
			System.Windows.Forms.Form form = new FormEx();
			form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			form.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			form.Location = new Point(-100, -100);
			form.Size = new Size(1, 1);
			return form;
		}
		public static XtraReport FromFile(string path, bool loadState) {
			FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
			try {
				return CreateFrom(stream, loadState, Assembly.GetCallingAssembly());
			} finally {
				stream.Close();
			}
		}
		public static XtraReport FromStream(Stream stream, bool loadState) {
			Guard.ArgumentNotNull(stream, "stream");
			return CreateFrom(stream, loadState, Assembly.GetCallingAssembly());
		}
		private static XtraReport CreateFrom(Stream stream, bool loadState, Assembly entryAssembly) {
			Type reportType = XRSerializer.GetReportType(stream, entryAssembly);
			if(reportType != null) {
				XtraReport report = Activator.CreateInstance(reportType) as XtraReport;
				if(loadState) report.LoadLayout(stream);
				return report;
			}
			return null;
		}
		public static PrintingSystemBase CreatePrintingSystem() {
			PrintingSystemBase result = new PrintingSystemBase();
			return result;
		}
		internal static XtraReport GetDefaultReport() {
			XtraReport report = new XtraReport();
			report.Bands.AddRange(new Band[] { CreateBand(typeof(TopMarginBand), "TopMargin", -1), CreateBand(typeof(DetailBand), "Detail", 100), CreateBand(typeof(BottomMarginBand), "BottomMargin", -1) });
			return report;
		}
		static Band CreateBand(Type type, string name, int height) {
			Band band = (Band)Activator.CreateInstance(type);
			band.Name = name;
			if(height > 0)
				band.HeightF = height;
			return band;
		}
		[Obsolete("Use the ReportUnit.ToDpi method instead.")]
		public static float UnitToDpi(ReportUnit reportUnit) {
			return reportUnit.ToDpi();
		}
		internal static FieldInfo[] GetFields(Type type, Type baseType) {
			System.Diagnostics.Debug.Assert(baseType.IsAssignableFrom(type));
			ArrayList fields = new ArrayList();
			while(type != baseType) {
				fields.AddRange(type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly));
				type = type.BaseType;
			}
			return (FieldInfo[])fields.ToArray(typeof(FieldInfo));
		}
		static FieldInfo FindField(string fieldName, IList fields) {
			foreach(FieldInfo field in fields) {
				if(fieldName == GetFieldName(field))
					return field;
			}
			return null;
		}
#if DEBUGTEST
		static void CheckPrintingSystemsInSubreports(XtraReport report) {
			NestedComponentEnumerator en = new NestedComponentEnumerator(report.Bands);
			while(en.MoveNext()) {
				SubreportBase subreport = en.Current as SubreportBase;
				if(subreport != null && subreport.ReportSource != null) {
					System.Diagnostics.Debug.Assert(0 == subreport.ReportSource.PrintingSystem.Styles.Count);
					CheckPrintingSystemsInSubreports(subreport.ReportSource);
				}
			}
		}
#endif
		static Version CurrentVersion {
			get { return new Version(AssemblyInfo.VersionShort); }
		}
		#endregion
		#region Fields & Properties
		const string defaultHtmlCharSet = "utf-8";
		readonly Collection<IObject> objectStorage = new Collection<IObject>();
		readonly XRComponentCollection componentStorage = new XRComponentCollection();
		readonly Dictionary<object, string> componentNames = new Dictionary<object, string>();
		Version version = CurrentVersion;
		bool initialized;
		ReportUnit reportUnit = ReportUnit.HundredthsOfAnInch;
		Size pageSize = DefaultPageSize;
		Size loadingPageSize = DefaultPageSize;
		PaperKind paperKind = DefaultPaperKind;
		string paperName = "";
		string printerName = "";
		bool landscape;
		bool rollPaper;
		Margins margins;
		int initCount;
		DevExpress.XtraPrinting.ExportOptions exportOptions;
		protected PrintingSystemBase fPrintingSystem;
		PrintingSystemBase creatingPrintingSystem;
		XRControlStyleSheet styleSheet;
		XRControlStyleContainer styleContainer;
		VerticalContentSplitting verticalContentSplitting = VerticalContentSplitting.Exact;
		HorizontalContentSplitting horizontalContentSplitting = DefaultHorizontalContentSplitting;
		NavigationManager navigationManager;
		ScriptSecurityPermissionCollection scriptSecurityPermissions = new ScriptSecurityPermissionCollection();
		XREventsScriptManager eventsScriptMgr;
		ScriptLanguage scriptLanguage = ScriptLanguage.CSharp;
		string scriptReferences = String.Empty;
		XRWatermark watermark;
		XRPrinterSettingsUsing defaultPrinterSettingsUsing;
		XRCrossBandControlCollection crossBandControls;
		List<Band> allBands;
		XRSummaryContainer summaries;
		CalculatedFieldCollection calculatedFields;
		string dataSourceSchema = string.Empty;
		object schemaDataSource;
		SnappingMode snappingMode = SnappingMode.SnapLines;
		ParameterCollection parameters;
		object parametersDataSource;
		FormattingRuleSheet formattingRuleSheet;
		Size gridSize = System.Drawing.Size.Empty;
		Nullable<float> snapGridSize = null;
		System.Windows.Forms.Form dummyForm;
		Color pageColor = Color.White;
		string scriptsSource = string.Empty;
		DesignerOptions designerOptions;
		ComponentsCollector componentsCollector = new ComponentsCollector();
		IReportPrintTool printTool;
		SerializableStringDictionary extensions = new SerializableStringDictionary();
		string displayName = string.Empty;
		int previewRowCount;
		BrickPresentation brickPresentation = BrickPresentation.Runtime;
		IDrillDownService drillDownService = null;
		UrlResolver urlResolver = null;
		EmptySpaceBand reusableEmptySpaceBand;
		PageSummaryBuilder pageSummaryBuilder;
		internal object SchemaDataSource {
			get { return schemaDataSource; }
		}
		protected internal BrickPresentation BrickPresentation {
			get { return DesignMode ? BrickPresentation.Design : brickPresentation; }
		}
		[
		DefaultValue(""),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.DisplayName"),
		SRCategory(ReportStringId.CatBehavior),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlNameDependedConverter)),
		Localizable(true),
		XtraSerializableProperty,
		]
		public string DisplayName {
			get { return displayName; }
			set { displayName = value; }
		}
		internal string EffectiveDisplayName {
			get {
				return StringHelper.GetNonEmptyValue(DisplayName, Name);
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, -2, XtraSerializationFlags.DeserializeCollectionItemBeforeCallSetIndex),
		]
		public IDictionary<String, String> Extensions {
			get { return extensions; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, 1000, XtraSerializationFlags.Cached),
		]
		public Collection<IObject> ObjectStorage {
			get { return objectStorage; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, 900, XtraSerializationFlags.Cached),
		]
		public XRComponentCollection ComponentStorage {
			get { return componentStorage; }
		}
		internal IDictionary<object, string> ComponentNames {
			get { return componentNames; }
		}
		IReportPrintTool IReport.PrintTool {
			get { return printTool; }
			set { printTool = value; }
		}
		protected IReportPrintTool ReportPrintTool {
			get { return printTool; }
		}
		[
		DefaultValue(""),
		Browsable(false),
		XtraSerializableProperty,
		]
		public string ScriptsSource {
			get { return scriptsSource; }
			set {
				scriptsSource = value;
				DevExpress.XtraPrinting.Tracer.TraceInformationTest(NativeSR.TraceSourceTests, "XtraReport.set_ScriptsSource");
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportPageColor"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.PageColor"),
		SRCategory(ReportStringId.CatAppearance),
		XtraSerializableProperty,
		]
		public Color PageColor { get { return pageColor; } set { pageColor = value; } }
		internal System.Windows.Forms.Form DummyForm {
			get {
				if(dummyForm == null) {
					dummyForm = CreateDummyForm();
					dummyForm.Show();
				}
				return dummyForm;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportSnapGridSize"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.SnapGridSize"),
		SRCategory(ReportStringId.CatDesign),
		DefaultValue(12.5f),
		Localizable(true),
		XtraSerializableProperty,
		]
		public float SnapGridSize {
			get {
				if(gridSize != System.Drawing.Size.Empty) {
					snapGridSize = XRConvert.Convert((float)gridSize.Width, GraphicsDpi.Pixel, Dpi);
					gridSize = System.Drawing.Size.Empty;
				}
				return snapGridSize.HasValue ? snapGridSize.Value : 12.5f;
			}
			set {
				snapGridSize = Math.Max(value, 0.1f);
			}
		}
#if DEBUGTEST
		[Browsable(false)]
		public
#else
		internal
#endif
		SizeF GridSizeF {
			get { return new SizeF(SnapGridSize, SnapGridSize); }
		}
		internal static int GridCellCount {
			get { return 4; }
		}
		internal Measurer Measurer { get { return ((IPrintingSystemContext)PrintingSystem).Measurer; } }
		internal NavigationManager NavigationManager { get { return navigationManager; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportFormattingRuleSheet"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.FormattingRuleSheet"),
		SRCategory(ReportStringId.CatAppearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, -1, XtraSerializationFlags.Cached),
		]
		public FormattingRuleSheet FormattingRuleSheet {
			get { return formattingRuleSheet; }
		}
		internal object ParametersDataSource {
			get {
				if(parametersDataSource == null) {
					DXDisplayNameAttribute attr = TypeDescriptor.GetProperties(typeof(XtraReport))[XRComponentPropertyNames.Parameters].Attributes[typeof(DXDisplayNameAttribute)] as DXDisplayNameAttribute;
					parametersDataSource = new DevExpress.XtraReports.Native.Parameters.ParametersDataSource(this.Parameters, attr.DisplayName);
				}
				return parametersDataSource;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportBookmark"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.Bookmark"),
		Browsable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Bindable(true),
		SRCategory(ReportStringId.CatNavigation),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlNameDependedConverter)),
		XtraSerializableProperty,
		]
		public override string Bookmark {
			get { return fBookmark; }
			set { fBookmark = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportParameters"),
#endif
		SRCategory(ReportStringId.CatParameters),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.Parameters"),
		Editor("DevExpress.XtraReports.Design.ParameterCollectionEditor, " + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, -1, XtraSerializationFlags.Cached),
		]
		public ParameterCollection Parameters {
			get { return parameters; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportDrawGrid"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.DrawGrid"),
		DefaultValue(true),
		SRCategory(ReportStringId.CatDesign),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public bool DrawGrid { get; set; }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportDrawWatermark"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.DrawWatermark"),
		DefaultValue(false),
		SRCategory(ReportStringId.CatDesign),
		TypeConverter(typeof(DevExpress.XtraReports.Design.DrawWatermarkConverter)),
		XtraSerializableProperty,
		]
		public bool DrawWatermark { get; set; }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportSnapToGrid"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.SnapToGrid"),
		DefaultValue(true),
		SRCategory(ReportStringId.CatDesign),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public bool SnapToGrid { get; set; }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportPreviewRowCount"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.PreviewRowCount"),
		DefaultValue(0),
		SRCategory(ReportStringId.CatDesign),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("The PreviewRowCount property is now obsolete. Use the XtraReportBase.DetailPrintCount property instead.")
		]
		public int PreviewRowCount { get { return previewRowCount; } set { previewRowCount = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportDesignerOptions"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.DesignerOptions"),
		TypeConverter(typeof(LocalizableObjectConverter)),
		SRCategory(ReportStringId.CatDesign),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public DesignerOptions DesignerOptions {
			get { return designerOptions; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use DesignerOptions.ShowDesignerHints instead."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool ShowDesignerHints {
			get { return DesignerOptions.ShowDesignerHints; }
			set { DesignerOptions.ShowDesignerHints = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use DesignerOptions.ShowExportWarnings instead."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool ShowExportWarnings {
			get { return DesignerOptions.ShowExportWarnings; }
			set { DesignerOptions.ShowExportWarnings = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use DesignerOptions.ShowPrintingWarnings instead."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool ShowPrintingWarnings {
			get { return DesignerOptions.ShowPrintingWarnings; }
			set { DesignerOptions.ShowPrintingWarnings = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportDataSourceSchema"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.DataSourceSchema"),
		DefaultValue(""),
		SRCategory(ReportStringId.CatDesign),
		Editor("DevExpress.Utils.UI.XmlSchemaEditor," + AssemblyInfo.SRAssemblyUtilsUIFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty,
		]
		public string DataSourceSchema {
			get { return dataSourceSchema; }
			set {
				if(dataSourceSchema != value) {
					dataSourceSchema = value;
					schemaDataSource = XmlDataHelper.CreateDataSetBySchema(value);
				}
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete("Use SnapGridSize instead."),
		]
		public Size GridSize {
			get { return gridSize; }
			set { gridSize = value; }
		}
		[
		SRCategory(ReportStringId.CatDesign),
		DefaultValue(SnappingMode.SnapLines),
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportSnappingMode"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.SnappingMode"),
		XtraSerializableProperty,
		]
		public SnappingMode SnappingMode {
			get { return snappingMode; }
			set { snappingMode = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override bool Expanded {
			get { return true; }
			set { }
		}
		internal List<Band> AllBands {
			get {
				if(allBands == null)
					allBands = new List<Band>();
				if(allBands.Count == 0)
					allBands.AddRange(EnumBandsRecursive().Where<Band>(item => !(item is XtraReportBase)));
				return allBands;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportCalculatedFields"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.CalculatedFields"),
		SRCategory(ReportStringId.CatData),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraReports.Design.CalculatedFieldCollectionEditor, " + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, -1, XtraSerializationFlags.Cached),
		]
		public CalculatedFieldCollection CalculatedFields {
			get { return calculatedFields; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, 0, XtraSerializationFlags.Cached),
		]
		public XRCrossBandControlCollection CrossBandControls {
			get { return crossBandControls; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override bool LockedInUserDesigner {
			get { return base.LockedInUserDesigner; }
			set { base.LockedInUserDesigner = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportScripts"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.Scripts"),
		SRCategory(ReportStringId.CatBehavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public new XtraReportScripts Scripts { get { return (XtraReportScripts)fEventScripts; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		Obsolete("The ShrinkSubreportIconArea property is now obsolete. Instead, you should use the XRSubreport.CanShrink property of every particular subreport."),
		]
		public bool ShrinkSubReportIconArea { get; set; }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportDefaultPrinterSettingsUsing"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.DefaultPrinterSettingsUsing"),
		SRCategory(ReportStringId.CatPageSettings),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public PrinterSettingsUsing DefaultPrinterSettingsUsing { get { return defaultPrinterSettingsUsing; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public XREventsScriptManager EventsScriptManager {
			get {
				if(eventsScriptMgr == null) {
					eventsScriptMgr = new XREventsScriptManager(this);
				}
				return eventsScriptMgr;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportShowPreviewMarginLines"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.ShowPreviewMarginLines"),
		SRCategory(ReportStringId.CatBehavior),
		DefaultValue(true),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public bool ShowPreviewMarginLines { get; set; }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportVerticalContentSplitting"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.VerticalContentSplitting"),
		TypeConverter(typeof(VerticalContentSplittingConverter)),
		SRCategory(ReportStringId.CatBehavior),
		DefaultValue(VerticalContentSplitting.Exact),
		XtraSerializableProperty,
		]
		public VerticalContentSplitting VerticalContentSplitting { get { return verticalContentSplitting; } set { verticalContentSplitting = value; } }
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.HorizontalContentSplitting"),
		TypeConverter(typeof(HorizontalContentSplittingConverter)),
		SRCategory(ReportStringId.CatBehavior),
		DefaultValue(HorizontalContentSplitting.Exact),
		XtraSerializableProperty,
		]
		public HorizontalContentSplitting HorizontalContentSplitting { get { return horizontalContentSplitting; } set { horizontalContentSplitting = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportBookmarkDuplicateSuppress"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.BookmarkDuplicateSuppress"),
		SRCategory(ReportStringId.CatNavigation),
		DefaultValue(true),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public bool BookmarkDuplicateSuppress { get; set; }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override PageBreak PageBreak { get { return PageBreak.None; } set { } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public XtraReport MasterReport { get; set; }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public string SourceUrl { get; set; }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public PrintingSystemBase PrintingSystem {
			get {
				if(fPrintingSystem == null)
					SetPrintingSystemValue(CreatePrintingSystemCore());
				return fPrintingSystem;
			}
		}
		protected virtual PrintingSystemBase CreatePrintingSystemCore() {
			return CreatePrintingSystem();
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportExportOptions"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.ExportOptions"),
		SRCategory(ReportStringId.CatBehavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public DevExpress.XtraPrinting.ExportOptions ExportOptions { get { return exportOptions; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportReportUnit"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.ReportUnit"),
		DefaultValue(ReportUnit.HundredthsOfAnInch),
		RefreshProperties(RefreshProperties.All),
		SRCategory(ReportStringId.CatBehavior),
		Localizable(true),
		XtraSerializableProperty,
		]
		public ReportUnit ReportUnit {
			get { return reportUnit; }
			set {
				if(reportUnit != value) {
					reportUnit = value;
					if(!Loading) {
						float dpi = reportUnit.ToDpi();
						SyncDpi(dpi);
					}
				}
			}
		}
		bool IReport.IsMetric {
			get {
				return ReportUnit.Equals(ReportUnit.TenthsOfAMillimeter);
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportRollPaper"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.RollPaper"),
		SRCategory(ReportStringId.CatPageSettings),
		RefreshProperties(RefreshProperties.All),
		DefaultValue(false),
		TypeConverter(typeof(BooleanTypeConverter)),
		Localizable(true),
		XtraSerializableProperty,
		]
		public bool RollPaper {
			get { 
				return rollPaper; 
			}
			set {
			   rollPaper = value;
			   if(rollPaper)
				   PaperKind = System.Drawing.Printing.PaperKind.Custom;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportLandscape"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.Landscape"),
		SRCategory(ReportStringId.CatPageSettings),
		DefaultValue(false),
		TypeConverter(typeof(DevExpress.XtraReports.Design.LandscapeConverter)),
		RefreshProperties(RefreshProperties.All),
		Localizable(true),
		XtraSerializableProperty,
		]
		public bool Landscape {
			get { return landscape; }
			set {
				bool oldLandscape = landscape;
				landscape = value;
				if(oldLandscape != landscape && !Loading)
					OnLandscapeChanged();
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportMargins"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.Margins"),
		SRCategory(ReportStringId.CatPageSettings),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRMarginsConverter)),
		DefaultValue(typeof(Margins), "100, 100, 100, 100"),
		Localizable(true),
		XtraSerializableProperty,
		]
		public Margins Margins {
			get {
				if(!Loading)
					margins = GetValidMargins();
				return margins;
			}
			set {
				if(value != null) {
					Margins oldMargins = margins;
					margins = value;
					OnSetMargins(value, oldMargins);
				} else {
					margins = (Margins)DefaultMargins.Clone();
					OnSetMargins((Margins)DefaultMargins.Clone());
				}
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportPrinterName"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.PrinterName"),
		DefaultValue(""),
		SRCategory(ReportStringId.CatPageSettings),
		TypeConverter(typeof(DevExpress.XtraReports.Design.PrinterNameConverter)),
		Localizable(true),
		XtraSerializableProperty,
		]
		public string PrinterName { get { return printerName; } set { printerName = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportPaperKind"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.PaperKind"),
		SRCategory(ReportStringId.CatPageSettings),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(DevExpress.XtraReports.Design.ReportPaperKindConverter)),
		DefaultValue(DefaultPaperKind),
		Localizable(true),
		XtraSerializableProperty,
		]
		public PaperKind PaperKind {
			get { return paperKind; }
			set {
				paperKind = value;
				if(!Loading && paperKind != PaperKind.Custom) {
					Size oldPageSize = pageSize;
					pageSize = PageSizeInfo.GetPageSize(paperKind, Dpi);
					OnPaperKindChanged(oldPageSize, pageSize);
				}
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportPaperName"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.PaperName"),
		DefaultValue(""),
		SRCategory(ReportStringId.CatPageSettings),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(DevExpress.XtraReports.Design.PaperNameConverter)),
		Editor("DevExpress.XtraReports.Design.PaperNameEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		Localizable(true),
		XtraSerializableProperty,
		]
		public string PaperName {
			get { return paperName; }
			set {
				paperName = value;
				if(!Loading && paperKind == PaperKind.Custom && printerName.Length > 0)
					pageSize = XRConvert.Convert(PageSizeInfo.GetPageSize(paperName, printerName, pageSize), GraphicsDpi.HundredthsOfAnInch, Dpi);
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public Size PageSize {
			get { return new Size(PageWidth, PageHeight); }
			set {
				PageWidth = value.Width;
				PageHeight = value.Height;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportPageWidth"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.PageWidth"),
		SRCategory(ReportStringId.CatPageSettings),
		TypeConverter(typeof(DevExpress.XtraReports.Design.PageWidthConverter)),
		DefaultValue(-1),
		Localizable(true),
		XtraSerializableProperty,
		]
		public int PageWidth {
			get { return landscape ? pageSize.Height : pageSize.Width; }
			set {
				if(Loading) {
					loadingPageSize.Width = value;
					return;
				}
				Size oldPageSize = pageSize;
				if(landscape) {
					SetPageSize(pageSize.Width, value);
					OnPageSizeChanged(value - oldPageSize.Height);
				} else {
					SetPageSize(value, pageSize.Height);
					OnPageSizeChanged(value - oldPageSize.Width);
				}
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportPageHeight"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.PageHeight"),
		SRCategory(ReportStringId.CatPageSettings),
		TypeConverter(typeof(DevExpress.XtraReports.Design.PageHeightConverter)),
		DefaultValue(-1),
		Localizable(true),
		XtraSerializableProperty,
		]
		public int PageHeight {
			get { return landscape ? pageSize.Width : pageSize.Height; }
			set {
				if(Loading) {
					loadingPageSize.Height = value;
					return;
				}
				if(landscape)
					SetPageSize(value, pageSize.Height);
				else
					SetPageSize(pageSize.Width, value);
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportStyleSheet"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.StyleSheet"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraReports.Design.XRControlStylesEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		SRCategory(ReportStringId.CatAppearance),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, 0, XtraSerializationFlags.Cached),
		]
		public XRControlStyleSheet StyleSheet { get { return styleSheet; } }
		internal IXRControlStyleContainer StyleContainer { get { return styleContainer; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportStyleSheetPath"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.StyleSheetPath"),
		DefaultValue(""),
		SRCategory(ReportStringId.CatAppearance),
		Editor("DevExpress.XtraReports.Design.StyleSheetFileNameEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty,
		]
		public string StyleSheetPath { get { return styleContainer.FileName; } set { styleContainer.SetFileName(value, Dpi); } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportWatermark"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.Watermark"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraReports.Design.XRWatermarkEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		SRCategory(ReportStringId.CatAppearance),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public XRWatermark Watermark { get { return watermark; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportScriptLanguage"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.ScriptLanguage"),
		SRCategory(ReportStringId.CatBehavior),
		DefaultValue(ScriptLanguage.CSharp),
		XtraSerializableProperty,
		]
		public ScriptLanguage ScriptLanguage { get { return scriptLanguage; } set { scriptLanguage = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportScriptReferences"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.ScriptReferences"),
		SRCategory(ReportStringId.CatBehavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Editor("DevExpress.Utils.UI.StringArrayEditor," + AssemblyInfo.SRAssemblyUtilsUIFull, typeof(System.Drawing.Design.UITypeEditor)),
		]
		public string[] ScriptReferences { get { return XRConvert.StringToStringArray(scriptReferences); } set { scriptReferences = XRConvert.StringArrayToString(value); } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportScriptSecurityPermissions"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.ScriptSecurityPermissions"),
		SRCategory(ReportStringId.CatBehavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Browsable(false),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public ScriptSecurityPermissionCollection ScriptSecurityPermissions {
			get { return scriptSecurityPermissions; }
		}
		[
		SRCategory(ReportStringId.CatBehavior),
		Browsable(false),
		DefaultValue(""),
		XtraSerializableProperty,
		]
		public string ScriptReferencesString { get { return scriptReferences; } set { scriptReferences = value; } }
		[Browsable(false)]
		public PageList Pages { get { return PrintingSystem.Pages; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty,
		]
		public string Version {
			get {
				return version.ToString();
			}
			set { version = new Version(value); }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportRequestParameters"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.RequestParameters"),
		SRCategory(ReportStringId.CatParameters),
		DefaultValue(true),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public bool RequestParameters { get; set; }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportShowPrintStatusDialog"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.ShowPrintStatusDialog"),
		SRCategory(ReportStringId.CatPrinting),
		DefaultValue(true),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public bool ShowPrintStatusDialog { get; set; }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportShowPrintMarginsWarning"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XtraReport.ShowPrintMarginsWarning"),
		SRCategory(ReportStringId.CatPrinting),
		DefaultValue(true),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public bool ShowPrintMarginsWarning { get; set; }
		internal bool Loading {
			get {
				return initCount > 0 || DesignerLoading;
			}
		}
		internal bool DesignerLoading { get; set; }
		internal PrintingDocument Document { get { return PrintingSystem.PrintingDocument; } }
		internal Rectangle PageBounds { get { return new Rectangle(0, 0, PageWidth, PageHeight); } }
		internal XRSummaryContainer Summaries { get { return summaries; } }
		internal bool VersionLessThen(Version value) { return !Loading && version < value && !initialized; }
#if DEBUGTEST
		public
#else
		internal
#endif
		override object GetEffectiveDataSource() {
			return base.GetEffectiveDataSource() != null ? base.GetEffectiveDataSource() :
				schemaDataSource != null ? schemaDataSource : null;
		}
		IPrintingSystem ILink.PrintingSystem { get { return PrintingSystem; } }
		PrintingSystemBase IDocumentSource.PrintingSystemBase { get { return PrintingSystem; } }
		Watermark IReport.Watermark { get { return watermark; } }
		#endregion
		#region Events
		private static readonly object SerializeEvent = new object();
		private static readonly object DeserializeEvent = new object();
		private static readonly object DesignerLoadedEvent = new object();
		private static readonly object FillEmptySpaceEvent = new object();
		private static readonly object PrintProgressEvent = new object();
		private static readonly object ParametersRequestBeforeShowEvent = new object();
		private static readonly object ParametersRequestValueChangedEvent = new object();
		private static readonly object ParametersRequestSubmitEvent = new object();
		private static readonly object SaveComponentsEvent = new object();
		private static readonly object PageSizeChangedEvent = new object();
		internal event EventHandler<EventArgs> PageSizeChanged {
			add { Events.AddHandler(PageSizeChangedEvent, value); }
			remove { Events.RemoveHandler(PageSizeChangedEvent, value); }
		}
		public event EventHandler<SaveComponentsEventArgs> SaveComponents {
			add { Events.AddHandler(SaveComponentsEvent, value); }
			remove { Events.RemoveHandler(SaveComponentsEvent, value); }
		}
		protected internal virtual void OnSaveComponents(SaveComponentsEventArgs e) {
			EventHandler<SaveComponentsEventArgs> handler = (EventHandler<SaveComponentsEventArgs>)Events[SaveComponentsEvent];
			if(handler != null) handler(this, e);
		}
		public event PrintProgressEventHandler PrintProgress {
			add { Events.AddHandler(PrintProgressEvent, value); }
			remove { Events.RemoveHandler(PrintProgressEvent, value); }
		}
		void OnPrintProgress(object sender, PrintProgressEventArgs e) {
			RunEventScript(PrintProgressEvent, XRControl.EventNames.PrintProgress, e);
			PrintProgressEventHandler handler = (PrintProgressEventHandler)Events[PrintProgressEvent];
			if(handler != null) handler(this, e);
		}
		public virtual event BandEventHandler FillEmptySpace {
			add { Events.AddHandler(FillEmptySpaceEvent, value); }
			remove { Events.RemoveHandler(FillEmptySpaceEvent, value); }
		}
		protected virtual void OnFillEmptySpace(BandEventArgs e) {
			RunEventScript(FillEmptySpaceEvent, XRControl.EventNames.FillEmptySpace, e);
			BandEventHandler handler = (BandEventHandler)Events[FillEmptySpaceEvent];
			if(handler != null) handler(this, e);
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XtraReportDesignerLoaded")]
#endif
		public event DesignerLoadedEventHandler DesignerLoaded {
			add { Events.AddHandler(DesignerLoadedEvent, value); }
			remove { Events.RemoveHandler(DesignerLoadedEvent, value); }
		}
		protected internal virtual void OnDesignerLoaded(DesignerLoadedEventArgs e) {
			DesignerLoadedEventHandler handler = (DesignerLoadedEventHandler)Events[DesignerLoadedEvent];
			if(handler != null) handler(this, e);
		}
		internal event XRSerializerEventHandler Serialize {
			add { Events.AddHandler(SerializeEvent, value); }
			remove { Events.RemoveHandler(SerializeEvent, value); }
		}
		internal event XRSerializerEventHandler Deserialize {
			add { Events.AddHandler(DeserializeEvent, value); }
			remove { Events.RemoveHandler(DeserializeEvent, value); }
		}
		[Obsolete("Use the PrintingSystem.CreateDocumentException event instead.")]
		internal event XRScriptExceptionEventHandler ScriptException {
			add { }
			remove { }
		}
		[Obsolete()]
		protected virtual void OnScriptException(ScriptExceptionEventArgs args) {
		}
		protected virtual internal void OnSerialize(XRSerializerEventArgs e) {
			XRSerializerEventHandler handler = (XRSerializerEventHandler)Events[SerializeEvent];
			if(handler != null) handler(this, e);
		}
		protected virtual internal void OnDeserialize(XRSerializerEventArgs e) {
			XRSerializerEventHandler handler = (XRSerializerEventHandler)Events[DeserializeEvent];
			if(handler != null) handler(this, e);
		}
		protected override void OnBandHeightChanged(BandEventArgs e) {
			if(Loading) return;
			margins = GetValidMargins();
			base.OnBandHeightChanged(e);
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XtraReportHtmlItemCreated"),
#endif
 EditorBrowsable(EditorBrowsableState.Never)]
		public override event HtmlEventHandler HtmlItemCreated {
			add { }
			remove { }
		}
		public event EventHandler<ParametersRequestEventArgs> ParametersRequestBeforeShow {
			add { Events.AddHandler(ParametersRequestBeforeShowEvent, value); }
			remove { Events.RemoveHandler(ParametersRequestBeforeShowEvent, value); }
		}
		public event EventHandler<ParametersRequestValueChangedEventArgs> ParametersRequestValueChanged {
			add { Events.AddHandler(ParametersRequestValueChangedEvent, value); }
			remove { Events.RemoveHandler(ParametersRequestValueChangedEvent, value); }
		}
		public event EventHandler<ParametersRequestEventArgs> ParametersRequestSubmit {
			add { Events.AddHandler(ParametersRequestSubmitEvent, value); }
			remove { Events.RemoveHandler(ParametersRequestSubmitEvent, value); }
		}
		void IReport.ReleasePrintingSystem() {
			ReleasePrintingSystemCore();
			fPrintingSystem = null;
		}
		void IReport.RaiseParametersRequestBeforeShow(IList<DevExpress.XtraReports.Parameters.ParameterInfo> parametersInfo) {
			OnParametersRequestBeforeShow(new ParametersRequestEventArgs(parametersInfo));
			RaiseParametersRequestBeforeShow(parametersInfo);
		}
		void IReport.RaiseParametersRequestValueChanged(IList<DevExpress.XtraReports.Parameters.ParameterInfo> parametersInfo, DevExpress.XtraReports.Parameters.ParameterInfo changedParameterInfo) {
			OnParametersRequestValueChanged(new ParametersRequestValueChangedEventArgs(parametersInfo, changedParameterInfo));
		}
		void IReport.RaiseParametersRequestSubmit(IList<DevExpress.XtraReports.Parameters.ParameterInfo> parametersInfo, bool createDocument) {
			RaiseParametersRequestSubmit(parametersInfo, createDocument);
		}
		protected virtual void RaiseParametersRequestBeforeShow(IList<DevExpress.XtraReports.Parameters.ParameterInfo> parametersInfo) {
			using(DataContext dataContext = new XRDataContext()) {
				foreach(DevExpress.XtraReports.Parameters.ParameterInfo parameterInfo in parametersInfo) {
					DynamicListLookUpSettings settings = parameterInfo.Parameter != null ? parameterInfo.Parameter.LookUpSettings as DynamicListLookUpSettings : null;
					if(settings == null) continue;
					DataSourceFiller filler = DataSourceFiller.CreateInstance(settings, this, false);
					if(filler != null) {
						filler.Execute();
						dataContext.GetDataBrowser(settings.DataSource, settings.DataMember, true);
					}
				}
			}
		}
		protected virtual void RaiseParametersRequestSubmit(IList<DevExpress.XtraReports.Parameters.ParameterInfo> parametersInfo, bool createDocument) {
			OnParametersRequestSubmit(new ParametersRequestEventArgs(parametersInfo));
			if(createDocument) {
				ResetTokenSource();
				if(!RaiseBeforePrint().Cancel)
					CreateDocumentCore(0, true);
				else {
					DisposeTokenSource();
					ClearPrintingSystem();
				}
			}
		}
		protected virtual void OnParametersRequestBeforeShow(ParametersRequestEventArgs e) {
			RunEventScript(ParametersRequestBeforeShowEvent, XtraReport.EventNames.ParametersRequestBeforeShow, e);
			EventHandler<ParametersRequestEventArgs> handler = (EventHandler<ParametersRequestEventArgs>)Events[ParametersRequestBeforeShowEvent];
			if(handler != null) handler(this, e);
		}
		protected virtual void OnParametersRequestValueChanged(ParametersRequestValueChangedEventArgs e) {
			RunEventScript(ParametersRequestValueChangedEvent, XtraReport.EventNames.ParametersRequestValueChanged, e);
			EventHandler<ParametersRequestValueChangedEventArgs> handler = (EventHandler<ParametersRequestValueChangedEventArgs>)Events[ParametersRequestValueChangedEvent];
			if(handler != null) handler(this, e);
		}
		protected virtual void OnParametersRequestSubmit(ParametersRequestEventArgs e) {
			RunEventScript(ParametersRequestSubmitEvent, XtraReport.EventNames.ParametersRequestSubmit, e);
			EventHandler<ParametersRequestEventArgs> handler = (EventHandler<ParametersRequestEventArgs>)Events[ParametersRequestSubmitEvent];
			if(handler != null) handler(this, e);
		}
		#endregion
		public XtraReport() {
#if !DEBUG
#endif
			DrawGrid = true;
			DrawWatermark = false;
			SnapToGrid = true;
			RequestParameters = true;
			BookmarkDuplicateSuppress = true;
			DesignerLoading = false;
			ShowPrintStatusDialog = true;
			ShowPrintMarginsWarning = true;
			ShowPreviewMarginLines = true;
#pragma warning disable 0618
			ShrinkSubReportIconArea = true;
#pragma warning restore 0618
			ResetWatermark();
			ResetDefaultPrinterSettingsUsing();
			ResetExportOptions();
			ResetDesignerOptions();
			margins = (Margins)DefaultMargins.Clone();
			styleSheet = new XRControlStyleSheet(this);
			styleContainer = new XRControlStyleContainer(styleSheet);
			navigationManager = new NavigationManager(this);
			crossBandControls = new XRCrossBandControlCollection(this);
			calculatedFields = new CalculatedFieldCollection(this);
			parameters = new ReportParameterCollection(this);
			formattingRuleSheet = new FormattingRuleSheet(this);
			ParentStyleUsing.Assign(StyleUsing.CreateEmptyStyleUsing());
		}
		public IEnumerable<XRControl> HasExportWarningControls() {
			foreach(XRControl control in AllControls<XRControl>()) {
				if(control.HasExportWarning())
					yield return control;
			}
		}
		internal override IEnumerable<XRControl> AllControls(Predicate<XRControl> enumChildren) {
			foreach(XRControl item in base.AllControls(enumChildren)) {
				yield return item;
			}
			foreach(XRControl item in CrossBandControls) {
				yield return item;
			}
		}
		internal bool HasUnusedRules { get { return GetUnusedRules().Count > 0; } }
		internal List<FormattingRule> GetUnusedRules() {
			List<FormattingRule> unusedRules = new List<FormattingRule>();
			List<FormattingRule> attachedRules = new List<FormattingRule>();
			FillAttachedRules(attachedRules);
			foreach(FormattingRule rule in FormattingRuleSheet) {
				InheritanceAttribute atrt = (InheritanceAttribute)TypeDescriptor.GetAttributes(rule)[typeof(InheritanceAttribute)];
				if(!attachedRules.Contains(rule) && rule.Site != null && atrt.InheritanceLevel == InheritanceLevel.NotInherited)
					unusedRules.Add(rule);
			}
			return unusedRules;
		}
		internal bool HasUnusedStyles { get { return GetUnusedStyles().Count > 0; } }
		internal List<XRControlStyle> GetUnusedStyles() {
			List<XRControlStyle> unusedStyles = new List<XRControlStyle>();
			List<XRControlStyle> attachedStyles = new List<XRControlStyle>();
			FillAttachedStyles(attachedStyles);
			foreach(XRControlStyle style in StyleSheet) {
				InheritanceAttribute atrt = (InheritanceAttribute)TypeDescriptor.GetAttributes(style)[typeof(InheritanceAttribute)];
				if(!attachedStyles.Contains(style) && style.Site != null && atrt.InheritanceLevel == InheritanceLevel.NotInherited)
					unusedStyles.Add(style);
			}
			return unusedStyles;
		}
		protected override XRControlScripts CreateScripts() {
			return new XtraReportScripts(this);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void CollectParameters(IList<Parameter> list, Predicate<Parameter> condition) {
			NestedParameterCollector parameterCollector = new NestedParameterCollector();
			var parameters = parameterCollector.EnumerateParameters(this).Where(parameter => condition(parameter));
			foreach(var parameter in parameters)
				list.Add(parameter);
		}
		ICollection<Parameter> IParameterSupplier.GetParameters() {
			return Parameters;
		}
		IEnumerable<DevExpress.Data.IParameter> DevExpress.Data.IParameterSupplierBase.GetIParameters() {
			return Parameters;
		}
		protected internal ArrayList GetBandPrintableControls(Band band) {
			BandPresenter bandPresenter = CreateBandPresenter();
			return bandPresenter.GetPrintableControls(band);
		}
		protected virtual BandPresenter CreateBandPresenter() {
			return CreatePresenter<BandPresenter>(
				delegate() { return new RuntimeBandPresenter(this); },
				delegate() { return new DesignBandPresenter(this); },
				delegate() { return new LayoutViewBandPresenter(this); }
		   );
		}
		protected virtual void SetPrintingSystemValue(PrintingSystemBase val) {
			fPrintingSystem = val;
			fPrintingSystem.AddService<IReport>(this);
			fPrintingSystem.AddService<IParameterSupplierBase>(this);
			fPrintingSystem.AddService<GroupingManager>(new GroupingManager());
			fPrintingSystem.AddService<ICancellationService>(new CancellationService());
			SubscribePSEvents();
		}
		void ReleasePrintingSystemCore() {
			if(fPrintingSystem != null) {
				fPrintingSystem.RemoveService(typeof(IReport));
				fPrintingSystem.RemoveService(typeof(IParameterSupplierBase));
				fPrintingSystem.RemoveService(typeof(GroupingManager));
				fPrintingSystem.RemoveService(typeof(ICancellationService));
				UnsubscribePSEvents();
			}
		}
		protected override void Dispose(bool disposing) {
			initCount++;
			if(disposing) {
				componentsCollector.Collect();
				if(crossBandControls != null) {
					crossBandControls.Dispose();
					crossBandControls = null;
				}
				if(watermark != null) {
					watermark.Dispose();
					watermark = null;
				}
				if(formattingRuleSheet != null) {
					formattingRuleSheet.Dispose();
					formattingRuleSheet = null;
				}
				ReleasePrintingSystemCore();
				if(fPrintingSystem != null) {
					fPrintingSystem.Document.Dispose();
					fPrintingSystem.Dispose();
					fPrintingSystem = null;
				}
				if(styleSheet != null) {
					styleSheet.DisposeStyles();
					styleSheet = null;
				}
				if(styleContainer != null) {
					styleContainer.Dispose();
					styleContainer = null;
				}
				if(navigationManager != null) {
					navigationManager.Clear();
					navigationManager = null;
				}
				if(eventsScriptMgr != null) {
					eventsScriptMgr.Dispose(); 
					eventsScriptMgr = null;
				}
				if(dummyForm != null) {
					dummyForm.Dispose();
					dummyForm = null;
				}
				if(calculatedFields != null) {
					calculatedFields.Dispose();
					calculatedFields = null;
				}
				if(printTool != null) {
					printTool.Dispose();
					printTool = null;
				}
				if(componentStorage != null) {
					foreach(IComponent component in componentStorage) {
						ConstructorInfo constructor = component.GetType().GetConstructor(new Type[] { typeof(IContainer) });
						if(constructor != null && component.Site == null)
							component.Dispose();
					}
					componentStorage.Clear();
				}
			}
			base.Dispose(disposing);
		}
		protected override XtraReportBase GetReport() {
			return this;
		}
		protected override XtraReport GetRootReport() {
			return this;
		}
		internal PrintingSystemBase GetCreatingPrintingSystem() {
			foreach(XtraReport item in this.AllReports()) {
				if(item.creatingPrintingSystem != null)
					return item.creatingPrintingSystem;
			}
			return null;
		}
		#region Serialization
		protected override void SerializeProperties(XRSerializer serializer) {
			if(!serializer.HasSoapInfo)
				return;
			base.SerializeProperties(serializer);
			serializer.SerializeBoolean("UseDefaultPrinterSettings", false);
			serializer.SerializeEnum("ReportUnit", reportUnit);
			serializer.SerializeBoolean("Landscape", landscape);
			serializer.SerializeEnum("PaperKind", paperKind);
			serializer.SerializeString("PaperName", paperName);
			serializer.SerializeMargins("Margins", margins);
			serializer.SerializeSize("PageSize", new Size(PageWidth, PageHeight));
			serializer.Serialize("StyleSheet", styleSheet);
			serializer.SerializeBoolean("ShowPreviewMarginLines", ShowPreviewMarginLines);
			serializer.SerializeEnum("ScriptLanguage", scriptLanguage);
			serializer.SerializeString("ScriptReferences", scriptReferences, String.Empty);
			serializer.Serialize("Watermark", watermark);
			serializer.Serialize("DefaultPrinterSettingsUsing", defaultPrinterSettingsUsing);
		}
		protected override void DeserializeProperties(XRSerializer serializer) {
			if(!serializer.HasSoapInfo)
				return;
			base.DeserializeProperties(serializer);
			bool useDefaultPrinterSettings = serializer.DeserializeBoolean("UseDefaultPrinterSettings", false);
			reportUnit = (ReportUnit)serializer.DeserializeEnum("ReportUnit", typeof(ReportUnit), ReportUnit.HundredthsOfAnInch);
			Dpi = reportUnit.ToDpi();
			landscape = serializer.DeserializeBoolean("Landscape", false);
			paperKind = (PaperKind)serializer.DeserializeEnum("PaperKind", typeof(PaperKind), DefaultPaperKind);
			paperName = serializer.DeserializeString("PaperName", "");
			margins = serializer.DeserializeMargins("Margins", DefaultMargins);
			exportOptions.Html.CharacterSet = exportOptions.Mht.CharacterSet = serializer.DeserializeString("HtmlCharSet", defaultHtmlCharSet);
			exportOptions.Html.RemoveSecondarySymbols = exportOptions.Mht.RemoveSecondarySymbols = serializer.DeserializeBoolean("HtmlCompressed", false);
			loadingPageSize = serializer.DeserializeSize("PageSize", DefaultPageSize);
			serializer.Deserialize("StyleSheet", styleSheet);
			ShowPreviewMarginLines = serializer.DeserializeBoolean("ShowPreviewMarginLines", true);
			scriptLanguage = (ScriptLanguage)serializer.DeserializeEnum("ScriptLanguage", typeof(ScriptLanguage), ScriptLanguage.CSharp);
			scriptReferences = serializer.DeserializeString("ScriptReferences", String.Empty);
			serializer.Deserialize("Watermark", watermark);
			serializer.Deserialize("DefaultPrinterSettingsUsing", defaultPrinterSettingsUsing);
		}
		void LoadStateInternal(string streamString, Stream stream, bool ignoreAssemblyVersionInfo) {
			XRTypeInfo typeInfo = XRTypeInfo.Deserialize(streamString);
			if(!ignoreAssemblyVersionInfo && !typeInfo.TypeEquals(GetType(), ignoreAssemblyVersionInfo))
				throw (new XRSerializationException(ReportLocalizer.GetString(ReportStringId.Msg_WrongReportClassName)));
			((ISupportInitialize)this).BeginInit();
			try {
				SetLoadingReportAssembly(typeInfo);
				styleSheet.Clear();
				XRSerializer.Begin(this, false);
				XtraReportSerializationSurrogate surrogate = new XtraReportSerializationSurrogate(this);
				IFormatter formatter = CreateLoadFormatter(surrogate);
				formatter.Binder = new MySerializationBinder();
				if(stream.CanSeek)
					stream.Seek(0, SeekOrigin.Begin);
				ReportStorage reportStorage = formatter.Deserialize(stream) as ReportStorage;
				if(reportStorage != null) reportStorage.InitReportStructure(this);
				else surrogate.InitReportStructure();
				InitFields();
				NestedComponentEnumerator en = new NestedComponentEnumerator(Bands);
				while(en.MoveNext()) {
					DetailReportBand detailReport = en.Current as DetailReportBand;
					if(detailReport != null)
						detailReport.ValidateDataSource(DataSource, null, "");
				}
				ClearPrintingSystem();
			} finally {
				((ISupportInitialize)this).EndInit();
				XRSerializer.End(this);
			}
		}
		void SetLoadingReportAssembly(XRTypeInfo typeInfo) {
			try {
				if(System.IO.File.Exists(typeInfo.AssemblyLocation))
					XRSerializer.LoadingAssembly = Assembly.LoadFrom(typeInfo.AssemblyLocation);
			} catch {
			} finally {
				XRSerializer.LoadingAssembly = GetType().Assembly;
			}
		}
		void InitFields() {
			FieldInfo[] fields = GetFields(GetType(), typeof(XtraReport));
			if(fields.Length > 0) {
				NestedComponentEnumerator en = new NestedComponentEnumerator(Bands);
				while(en.MoveNext())
					InitField(fields, en.Current.Name, en.Current);
				foreach(Parameter parameter in Parameters)
					InitField(fields, parameter.Name, parameter);
				foreach(CalculatedField calculatedField in CalculatedFields)
					InitField(fields, calculatedField.Name, calculatedField);
			}
		}
		void InitField(FieldInfo[] fields, string name, object component) {
			FieldInfo fieldInfo = FindField(name, fields);
			if(fieldInfo != null && fieldInfo.FieldType == component.GetType())
				fieldInfo.SetValue(this, component);
		}
		internal static string GetFieldName(FieldInfo field) {
			object[] attribs = field.GetCustomAttributes(typeof(AccessedThroughPropertyAttribute), false);
			if(attribs.Length == 1) {
				AccessedThroughPropertyAttribute attr = (AccessedThroughPropertyAttribute)attribs[0];
				return attr.PropertyName;
			}
			return field.Name;
		}
		void InitFieldsInternal(XtraReport source) {
			FieldInfo[] destFields = GetFields(GetType(), typeof(XtraReport));
			FieldInfo[] srcFields = GetFields(source.GetType(), typeof(XtraReport));
			foreach(FieldInfo destField in destFields) {
				FieldInfo srcField = FindField(GetFieldName(destField), srcFields);
				if(srcField != null) {
					if(srcField != null && srcField.FieldType == destField.FieldType)
						destField.SetValue(this, srcField.GetValue(source));
					else if(srcField.FieldType.FullName == destField.FieldType.FullName)
						destField.SetValue(this, null);
				}
			}
		}
		protected internal XtraReport CloneReport() {
			return CloneReport(false);
		}
#if DEBUGTEST
		[Browsable(false)]
		public IXRControlStyleContainer Test_StyleContainer {
			get { return StyleContainer; }
		}
		public XtraReport Test_CloneReport() {
			return CloneReport();
		}
#endif
		internal XtraReport CloneReport(bool useXmlSerialization) {
			System.Diagnostics.Debug.Assert(this.Site != null);
			try {
				nonSerializableComponentsHelper = new NonSerializableComponentsHelperBase();
				XtraReport result = CreateClone();
				using (Stream stream = new MemoryStream()) {
					if(useXmlSerialization)
						this.SaveLayoutToXml(stream);
					else
						ReportStorageService.SetData(this, stream);
					XtraReport src = null;
					result.LoadLayoutInternal(stream, ref src, false);
				}
				string projectDirectory = GetProjectFullName();
				if(projectDirectory.Length > 0)
					result.SetPictureSourceDirectory(projectDirectory);
				return result;
			} finally {
				nonSerializableComponentsHelper = null;
			}
		}
		protected virtual XtraReport CreateClone() {
			Type type = GetReportType();
			if(!typeof(XtraReport).IsAssignableFrom(type))
				throw new ArgumentException();
			return (XtraReport)Activator.CreateInstance(type);
		}
		internal IList GetReportComponents() {
			DesignItemList components = this.GetAssociatedComponents();
			CollectComponentsFromHost(components);
			List<IComponent> nonSerializableComponents = GetNonSerializableComponentsHelper().GetNonSerializableComponents(this);
			foreach(IComponent item in nonSerializableComponents)
				components.Remove(item);
			System.Diagnostics.Debug.Assert(!components.Contains(this));
			components.Insert(0, this);
			OnSaveComponents(new SaveComponentsEventArgs(components));
			Hashtable ht = GetComponentNamesHT(components);
			ArrayList componentFormFileds = new ArrayList(ht.Keys);
			foreach(object obj in componentFormFileds) {
				components.Remove(obj);
				components.Insert(1, obj);
			}
			return components;
		}
		internal Hashtable GetComponentNamesHT(IList components) {
			Hashtable result = new Hashtable();
			FieldInfo[] fields = XtraReport.GetFields(GetType(), typeof(XtraReport));
			foreach(FieldInfo field in fields) {
				object val = field.GetValue(this);
				if(components.Contains(val)) {
					result[val] = XtraReport.GetFieldName(field);
				}
			}
			foreach(KeyValuePair<object, string> item in componentNames)
				if(components.Contains(item.Key))
					result[item.Key] = item.Value;
			return result;
		}
		string GetProjectFullName() {
			IDTEService svc = (IDTEService)GetService(typeof(IDTEService));
			return svc != null ? svc.ProjectFullName : String.Empty;
		}
		Type GetReportType() {
			NestedComponentEnumerator en = new NestedComponentEnumerator(Bands);
			while(en.MoveNext()) {
				if(en.Current is SubreportBase) {
					XtraReport report = ((SubreportBase)en.Current).ReportSource;
					if(report != null) {
						IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
						System.Diagnostics.Debug.Assert(designerHost != null);
						Type type = report.GetType().Assembly.GetType(designerHost.RootComponentClassName);
						return type != null ? type : GetType();
					}
				}
			}
			return GetType();
		}
		#region new serialization
		public override string ControlType { get { return GetType().AssemblyQualifiedName; } }
		public void SaveLayoutToXml(string fileName) {
			FileStream fileStream = File.Create(fileName);
			try {
				SaveLayoutToXml(fileStream);
			} finally {
				fileStream.Close();
			}
		}
		public void SaveLayoutToXml(Stream stream) {
			XtraReportsXmlSerializer serializer = CreateXmlSerializer();
			serializer.SerializeRootObject(this, stream);
		}
		public void LoadLayoutFromXml(string fileName) {
			FileStream fileStream = File.Open(fileName, FileMode.Open);
			try {
				LoadLayoutFromXml(fileStream);
			} finally {
				fileStream.Close();
			}
		}
		public void LoadLayoutFromXml(Stream stream) {
			ClearContent();
			if(stream.CanSeek)
				stream.Seek(0, SeekOrigin.Begin);
			XtraReportsXmlSerializer serializer = CreateXmlSerializer();
			serializer.DeserializeObject(this, stream, "");
			OnReportEndDeserializing();
		}
		internal void ClearContent() {
			Bands.Clear();
			CrossBandControls.Clear();
			FormattingRuleSheet.Clear();
			StyleSheet.Clear();
			StyleSheetPath = string.Empty;
			CalculatedFields.Clear();
			Parameters.Clear();
			Extensions.Clear();
			ScriptSecurityPermissions.Clear();
			ObjectStorage.Clear();
			if(ComponentStorage.Contains(DataSource)) 
				PerformAsLoad(() =>  DataSource = null);
			ComponentStorage.Clear();
			componentNames.Clear();
		}
		XtraReportsXmlSerializer CreateXmlSerializer() {
			return new XtraReportsXmlSerializer(this);
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty,
		DefaultValue(""),
		]
		public string EventsInfo {
			get { return new EventSerializer().Serialize(this); }
			set { new EventSerializer().Deserialize(this, value); }
		}
		public void SaveLayout(string fileName) {
			SaveLayout(fileName, false);
		}
		public void LoadLayout(string fileName) {
			if(!File.Exists(fileName))
				throw new ArgumentException(ReportLocalizer.GetString(ReportStringId.Msg_FileNotFound));
			FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			try {
				LoadLayout(stream);
			} finally {
				stream.Close();
			}
		}
		public void SaveLayout(Stream stream) {
			SaveLayout(stream, false);
		}
		public void LoadLayout(Stream stream) {
			XtraReport source = null;
			LoadLayoutInternal(stream, ref source, true);
		}
		public void SaveLayout(string fileName, bool throwOnError) {
			FileStream stream = File.Create(fileName);
			try {
				SaveLayout(stream, throwOnError);
			} finally {
				stream.Close();
			}
		}
		public void SaveLayout(Stream stream, bool throwOnError) {
			CodeGeneratorAccessorBase.Instance.GenerateCSharpCode(this, GetReportComponents(), stream, throwOnError);
		}
		internal void LoadLayoutInternal(Stream stream, ref XtraReport compiledReport, bool forceDataSource) {
			if(stream.CanSeek)
				stream.Seek(0, SeekOrigin.Begin);
			string streamString = new StreamReader(stream).ReadToEnd();
			bool useResourceManager = DXDisplayNameAttribute.UseResourceManager;
			try {
				DXDisplayNameAttribute.UseResourceManager = Settings.Default.ShowUserFriendlyNamesInUserDesigner;
				if(stream == null)
					throw new ArgumentNullException("stream");
				if(FileFormatDetector.CreateSoapDetector().FormatExists(streamString)) {
					LoadStateInternal(streamString, stream, true);
					return;
				}
				if(FileFormatDetector.CreateXmlDetector().FormatExists(streamString)) {
					LoadLayoutFromXml(stream);
					return;
				}
				compiledReport = ReportCompiler.Compile(streamString, Site);
				if(compiledReport == null)
					throw new ArgumentException("Stream is corrupted");
				CopyFrom(compiledReport, forceDataSource);
			} finally {
				DXDisplayNameAttribute.UseResourceManager = useResourceManager;
			}
		}
		protected internal virtual void CopyFrom(XtraReport source, bool forceDataSource) {
			object dataSource = DataSource;
			((ISupportInitialize)this).BeginInit();
			try {
				CopyProperties(source);
			} finally {
				((ISupportInitialize)this).EndInit();
			}
			CopyPropertiesFromResources(source);
			if(forceDataSource && DataSource == null)
				DataSource = dataSource;
		}
		void CopyProperties(XtraReport source) {
			CalculatedFields.CopyFrom(source.CalculatedFields);
			source.CalculatedFields.Clear();
			this.FormattingRuleSheet.CopyFrom(source.FormattingRuleSheet);
			this.Parameters.Clear();
			this.Parameters.AddRange(source.Parameters.ToArray<Parameter>());
			Bands.CopyFrom(source.Bands.ToArray<Band>());
			source.Bands.Clear();
			XRControlStyle[] styles = source.StyleSheet.ToArray<XRControlStyle>();
			source.StyleSheet.Clear();
			StyleSheet.Assign(styles);
			StyleSheetPath = source.StyleSheetPath;
			ScriptSecurityPermissions.Clear();
			ScriptSecurityPermissions.AddRange(source.ScriptSecurityPermissions.Cast<ScriptSecurityPermission>().ToArray<ScriptSecurityPermission>());
			this.CrossBandControls.Clear();
			this.CrossBandControls.AddRange(source.CrossBandControls.Cast<XRCrossBandControl>().ToArray<XRCrossBandControl>());
			source.CrossBandControls.Clear();
			this.Extensions.Clear();
			foreach(KeyValuePair<string, string> item in source.Extensions)
				this.Extensions.Add(item);
			this.ComponentStorage.Clear();
			foreach(IComponent item in source.ComponentStorage)
				this.ComponentStorage.Add(item);
			this.ExportOptions.Email.AdditionalRecipients.Clear();
			foreach(Recipient item in source.ExportOptions.Email.AdditionalRecipients)
				this.ExportOptions.Email.AdditionalRecipients.Add(item);
			CopyProperties(source, this);
			InitFieldsInternal(source);
		}
		void CopyPropertiesFromResources(XtraReport source) {
			System.Reflection.PropertyInfo pi = source.GetType().GetProperty("resources", BindingFlags.Instance | BindingFlags.NonPublic);
			if(pi != null) {
				System.Resources.ResourceManager resourceManager = (System.Resources.ResourceManager)pi.GetValue(source, null);
				if(resourceManager != null) {
					string s = (string)resourceManager.GetObject(EventSerializer.ResourceName);
					if(s != null)
						new EventSerializer().Deserialize(this, s);
					DeserializeProperties(new XRSerializer(new ResourceSerializationInfo(resourceManager)));
				}
				OnDeserialize(new XRSerializerEventArgs(resourceManager));
			}
		}
		internal static void CopyProperties(object src, object dest) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(dest.GetType());
			PropertyDescriptorCollection srcProperties = TypeDescriptor.GetProperties(src.GetType());
			foreach(PropertyDescriptor property in properties) {
				PropertyDescriptor srcProperty = srcProperties[property.Name];
				if(property.SerializationVisibility == DesignerSerializationVisibility.Hidden ||
					srcProperty == null || !srcProperty.PropertyType.Equals(property.PropertyType))
					continue;
				if(property.SerializationVisibility == DesignerSerializationVisibility.Content &&
					!typeof(IList).IsAssignableFrom(property.PropertyType))
					CopyProperties(srcProperty.GetValue(src), property.GetValue(dest));
				try {
					if(!property.IsReadOnly)
						property.SetValue(dest, srcProperty.GetValue(src));
				} catch { }
			}
		}
		#endregion
		protected override void CollectAssociatedComponents(DesignItemList components) {
			base.CollectAssociatedComponents(components);
			foreach(FormattingRule formattingRule in this.FormattingRuleSheet)
				components.Add(formattingRule);
			if(this.Site == null && GetType().IsSubclassOf(typeof(XtraReport))) {
				CollectComponentsFromFields(this, components);
			}
			foreach(XRControlStyle style in StyleSheet)
				components.Add(style);
			foreach(XRCrossBandControl cbControl in this.CrossBandControls)
				components.Add(cbControl);
			foreach(CalculatedField calcField in this.CalculatedFields)
				components.Add(calcField);
			foreach(Parameter parameter in this.Parameters) {
				components.Add(parameter);
				DynamicListLookUpSettings lookUpSettings = parameter.LookUpSettings as DynamicListLookUpSettings;
				if(lookUpSettings != null) {
					components.AddDataAdapter(lookUpSettings.DataAdapter);
					components.AddDataSource(lookUpSettings.DataSource as IComponent);
				}
			}
			foreach(IComponent item in this.ComponentStorage) {
				if(item is IComponent)
					components.Add(item);
			}
		}
		void CollectComponentsFromHost(DesignItemList components) {
			if(!DesignMode) return;
			IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
			if(designerHost == null) return;
			foreach(IComponent component in designerHost.Container.Components) {
				if(component.Equals(this)) continue;
				components.Add(component);
				components.AddIDbDataAdapterComponents(component as IDbDataAdapter);
			}
		}
		#endregion
		internal override void SyncStyleName(string oldName, string newName) {
			styleSheet.ClearCache();
			base.SyncStyleName(oldName, newName);
		}
		protected override void OnDataSourceChanging() {
			base.OnDataSourceChanging();
			foreach(CalculatedField item in CalculatedFields)
				item.AdjustDataSourse();
			foreach(FormattingRule item in FormattingRuleSheet)
				item.AdjustDataSourse();
			ClearDataContext();
		}
		protected internal override void CopyDataProperties(XRControl source) {
			initCount++;
			try {
				foreach(ISupportInitialize ctrl in DataContainersForInitialize())
					ctrl.BeginInit();
				this.PerformAsLoad(delegate() {
					CopyDataPropertiesCore(source);
				});
				foreach(ISupportInitialize ctrl in DataContainersForInitialize())
					ctrl.EndInit();
			} finally {
				initCount--;
			}
		}
		IEnumerable<ISupportInitialize> DataContainersForInitialize() {
			foreach(XRControl ctrl in new NestedComponentEnumerator(this.Bands))
				if(ctrl is IDataContainer && ctrl is ISupportInitialize)
					yield return ((ISupportInitialize)ctrl);
		}
		void CopyDataPropertiesCore(XRControl source) {
			base.CopyDataProperties(source);
			XtraReport sourceReport = (XtraReport)source;
			if(sourceReport != null) {
				for(int i = 0; i < CalculatedFields.Count; i++)
					CalculatedFields[i].CopyDataProperties(sourceReport.CalculatedFields[i]);
				for(int i = 0; i < FormattingRuleSheet.Count; i++)
					FormattingRuleSheet[i].CopyDataProperties(sourceReport.FormattingRuleSheet[i]);
				for(int i = 0; i < Parameters.Count && i < sourceReport.Parameters.Count; i++) {
					DynamicListLookUpSettings targetSettings = Parameters[i].LookUpSettings as DynamicListLookUpSettings;
					if(targetSettings != null) {
						DynamicListLookUpSettings sourceSettings = sourceReport.Parameters[i].LookUpSettings as DynamicListLookUpSettings;
						if(sourceSettings != null) {
							targetSettings.DataSource = sourceSettings.DataSource;
							targetSettings.DataAdapter = sourceSettings.DataAdapter;
						}
					}
				}
			}
		}
		void IRootXmlObject.PerformAsLoad(Action0 action) {
			PerformAsLoad(action);
		}
		internal void PerformAsLoad(Action0 action) {
			initCount++;
			try {
				action();
			} finally {
				initCount--;
			}
		}
		protected internal override PrintingSystemBase GetPrintingSystem() {
			return fPrintingSystem;
		}
		protected internal override void OnBeforePrint(PrintEventArgs e) {
			IDrillDownServiceBase serv = ((IServiceProvider)this).GetService(typeof(IDrillDownServiceBase)) as IDrillDownServiceBase;
			bool skipIfFilled = serv != null && serv.IsDrillDowning;
			FillDataSources(skipIfFilled);
			base.OnBeforePrint(e);
			BeforeReportPrint();
			if(!ReportPrintOptions.PrintOnEmptyDataSource && DataBrowser.Count == 0)
				e.Cancel = true;
		}
		protected internal override void OnAfterPrint(EventArgs e) {
			AfterReportPrint();
			base.OnAfterPrint(e);
			foreach(XRCrossBandControl item in CrossBandControls)
				item.AfterReportPrintInternal();
			EventsScriptManager.Release();
		}
		protected internal override void InitializeScripts() {
			EventsScriptManager.Initialize(ScriptReferences, scriptSecurityPermissions);
			base.InitializeScripts();
			foreach(CalculatedField item in this.CalculatedFields)
				item.InitializeScripts();
		}
		protected override void BeforeReportPrint() {
			base.BeforeReportPrint();
			foreach(XRCrossBandControl item in CrossBandControls)
				item.BeforeReportPrintInternal();
			MergeBrickHelper mergeHelper = PrintingSystem.GetService<MergeBrickHelper>();
			if(!(mergeHelper is XRMergeBrickHelper)) {
				PrintingSystem.RemoveService(typeof(MergeBrickHelper));
				PrintingSystem.AddService(typeof(MergeBrickHelper), new XRMergeBrickHelper());
			}
			summaries = new XRSummaryContainer(AllControls<XRLabel>());
			navigationManager.Initialize();
			foreach(CalculatedField calculatedField in this.CalculatedFields)
				calculatedField.ValidateExpression();
		}
		protected internal override bool HasPrintingWarning() {
			foreach(Band band in this.Bands) {
				IList controls = band.GetPrintableControls();
				NestedComponentEnumerator en = new NestedComponentEnumerator(controls);
				while(en.MoveNext())
					if(en.Current.HasPrintingWarning())
						return true;
			}
			return false;
		}
		protected override bool GetSuppressClearDataContext() {
			if(MasterReport != null)
				return MasterReport.GetSuppressClearDataContext();
			return base.GetSuppressClearDataContext();
		}
		internal void SetPictureSourceDirectory(string sourceDirectory) {
			NestedComponentEnumerator en = new NestedComponentEnumerator(Bands);
			while(en.MoveNext()) {
				if(en.Current is XRPictureBox) {
					((XRPictureBox)en.Current).SourceDirectory = sourceDirectory;
				} else if(en.Current is SubreportBase) {
					SubreportBase subreport = (SubreportBase)en.Current;
					if(subreport.ReportSource != null)
						subreport.ReportSource.SetPictureSourceDirectory(sourceDirectory);
				}
			}
		}
		internal Size GetClientSize() {
			return new Size(PageBounds.Width - margins.Left - margins.Right, PageBounds.Height - margins.Top - margins.Bottom);
		}
		Margins GetValidMargins() {
			Margins margins = (Margins)this.margins.Clone();
			Band band = Bands.GetBandByType(typeof(TopMarginBand));
			if(band != null) margins.Top = (int)Math.Round(band.HeightF);
			band = Bands.GetBandByType(typeof(BottomMarginBand));
			if(band != null) margins.Bottom = (int)Math.Round(band.HeightF);
			return margins;
		}
		private void OnSetMargins(Margins val) {
			if(!Loading) {
				Band band = Bands.GetBandByType(typeof(TopMarginBand));
				if(band != null) XRAccessor.SetProperty(band, XRComponentPropertyNames.Height, (float)val.Top);
				band = Bands.GetBandByType(typeof(BottomMarginBand));
				if(band != null) XRAccessor.SetProperty(band, XRComponentPropertyNames.Height, (float)val.Bottom);
			}
		}
		private void OnSetMargins(Margins newMargins, Margins oldMargins) {
			if(!Loading) {
				OnSetMargins(newMargins);
				float leftDelta = oldMargins.Left - newMargins.Left;
				float rightDelta = oldMargins.Right - newMargins.Right;
				if(Math.Abs(leftDelta) > 0.001)
					OnPageSizeChanged(leftDelta);
				if(Math.Abs(rightDelta) > 0.001)
					OnPageSizeChanged(rightDelta);
			}
		}
		void OnPageSizeChanged(float delta) {
			EventHandler<EventArgs> handler = (EventHandler<EventArgs>)Events[PageSizeChangedEvent];
			if(handler != null) handler(this, EventArgs.Empty);
			foreach(Band band in AllBands)
				foreach(XRControl control in band.Controls)
					SetHorizontalAnchorBounds(control, delta);
			foreach(XRCrossBandControl cbControl in this.CrossBandControls)
				SetHorizontalAnchorBounds(cbControl, delta);
		}
		void SetHorizontalAnchorBounds(XRControl control, float delta) {
			switch(control.AnchorHorizontal) {
				case HorizontalAnchorStyles.Right:
					control.LeftF += delta;
					break;
				case HorizontalAnchorStyles.None:
				case HorizontalAnchorStyles.Left:
					break;
				case HorizontalAnchorStyles.Both:
					control.WidthF += delta;
					break;
			}
		}
		void OnPaperKindChanged(Size oldSize, Size newSize) {
			RollPaper = false;
			if(landscape)
				OnPageSizeChanged(newSize.Height - oldSize.Height);
			else
				OnPageSizeChanged(newSize.Width - oldSize.Width);
		}
		void OnLandscapeChanged() { 
			if(landscape)
				OnPageSizeChanged(pageSize.Height - pageSize.Width);
			else
				OnPageSizeChanged(pageSize.Width - pageSize.Height);
		}
		public void BeginUpdate() {
			initCount++;
		}
		public void EndUpdate() {
			if(!CanDoEndUpdate)
				return;
			initCount--;
			if(paperKind != PaperKind.Custom)
				pageSize = PageSizeInfo.GetPageSize(paperKind, Dpi);
			else
				pageSize = landscape ? new Size(loadingPageSize.Height, loadingPageSize.Width) :
					new Size(loadingPageSize.Width, loadingPageSize.Height);
			SyncDpi(reportUnit.ToDpi());
			if(VersionLessThen(v8_1))
				new ReportConverter_v8_1(this).Convert();
			if(VersionLessThen(v9_1))
				new ReportConverter_v9_1(this).Convert();
			if(VersionLessThen(v9_3))
				new ReportConverter_v9_3(this).Convert();
			StyleContainer.ClearDirty();
			OnReportInitialize();
			if(RootReport.VersionLessThen(v10_2))
				new ReportConverter_v10_2(this).Convert();
			if(VersionLessThen(v12_2))
				new ReportConverter_v12_2(this).Convert();
			initialized = true;
		}
		bool CanDoEndUpdate {
			get {
				return !DesignerLoading && initCount > 0;
			}
		}
		public void BeginInit() {
			if(initCount <= 0 && !initialized)
				version = new Version();
			BeginUpdate();
		}
		public void EndInit() {
			if(!CanDoEndUpdate)
				return;
			EndUpdate();
			if(version < CurrentVersion)
				version = CurrentVersion;
		}
		private void SetPageSize(int width, int height) {
			if(paperKind == PaperKind.Custom) {
				pageSize = new Size(width, height);
				ValidateMargins();
			}
		}
		void ValidateMargins() {
			float delta = GetMaxControlRight() - GetClientSize().Width;
			if(delta > 0) {
				delta = (int)Math.Ceiling(delta / 2f);
				Margins = new Margins(Math.Max(0, Margins.Left - (int)delta), Math.Max(0, Margins.Right - (int)delta), Margins.Top, Margins.Bottom);
			}
		}
		private float GetMaxControlRight() {
			float val = 0;
			NestedComponentEnumerator compEnum = new NestedComponentEnumerator(Bands);
			while(compEnum.MoveNext()) {
				if(!(compEnum.Current is Band))
					continue;
				Band band = (Band)compEnum.Current;
				foreach(XRControl control in band.Controls)
					val = Math.Max(control.RightF, val);
			}
			return val;
		}
		void UnsubscribePSEvents() {
			if(fPrintingSystem == null)
				return;
			fPrintingSystem.AfterChange -= new DevExpress.XtraPrinting.ChangeEventHandler(printingSystem_AfterChange);
			fPrintingSystem.AfterBandPrint -= new PageEventHandler(printingSystem_AfterBandPrint);
			fPrintingSystem.AfterPagePrint -= new PageEventHandler(printingSystem_AfterPagePrint);
			fPrintingSystem.FillEmptySpace -= new EmptySpaceEventHandler(printingSystem_FillEmptySpace);
			fPrintingSystem.PrintProgress -= new PrintProgressEventHandler(OnPrintProgress);
		}
		void SubscribePSEvents() {
			if(fPrintingSystem == null)
				return;
			fPrintingSystem.AfterChange += new DevExpress.XtraPrinting.ChangeEventHandler(printingSystem_AfterChange);
			fPrintingSystem.AfterBandPrint += new PageEventHandler(printingSystem_AfterBandPrint);
			fPrintingSystem.AfterPagePrint += new PageEventHandler(printingSystem_AfterPagePrint);
			fPrintingSystem.FillEmptySpace += new EmptySpaceEventHandler(printingSystem_FillEmptySpace);
			fPrintingSystem.PrintProgress += new PrintProgressEventHandler(OnPrintProgress);
		}
		void printingSystem_AfterChange(object sender, DevExpress.XtraPrinting.ChangeEventArgs e) {
			VisualBrick brick = e.ValueOf(DevExpress.XtraPrinting.SR.Brick) as VisualBrick;
			if(brick == null)
				return;
			XRControl control = brick.BrickOwner as XRControl;
			if(control == null)
				return;
			PreviewMouseEventArgs args = new PreviewMouseEventArgs(brick, e);
			RaisePreviewEvent(control, e.EventName, args);
		}
		void RaisePreviewEvent(XRControl control, string eventName, PreviewMouseEventArgs args) {
			switch(eventName) {
				case DevExpress.XtraPrinting.SR.BrickClick:
					control.RaisePreviewClick(args);
					break;
				case DevExpress.XtraPrinting.SR.BrickMouseMove:
					control.RaisePreviewMouseMove(args);
					break;
				case DevExpress.XtraPrinting.SR.BrickMouseDown:
					control.RaisePreviewMouseDown(args);
					break;
				case DevExpress.XtraPrinting.SR.BrickMouseUp:
					control.RaisePreviewMouseUp(args);
					break;
				case DevExpress.XtraPrinting.SR.BrickDoubleClick:
					control.RaisePreviewDoubleClick(args);
					break;
			}
		}
		void printingSystem_AfterBandPrint(object sender, DevExpress.XtraPrinting.PageEventArgs e) {
			CalculatePageSummary(e.DocumentBands.First<DocumentBand>());
		}
		void printingSystem_AfterPagePrint(object sender, DevExpress.XtraPrinting.PageEventArgs e) {
			PSPage page = e.Page as PSPage;
			if(page == null || !page.Additional)
				FinishPageSummary();
		}
		void printingSystem_FillEmptySpace(object sender, DevExpress.XtraPrinting.EmptySpaceEventArgs e) {
			XRMergeBrickHelper mergeHelper = PrintingSystem.GetService<MergeBrickHelper>() as XRMergeBrickHelper;
			if(mergeHelper != null)
				mergeHelper.PageContentBottomDictionary.Add(e.Page, e.EmptySpaceOffset + Math.Max(0, e.EmptySpace));
			if(e.EmptySpace <= 0)
				return;
			if(reusableEmptySpaceBand == null) {
				reusableEmptySpaceBand = new EmptySpaceBand(this);
				componentsCollector.Add(reusableEmptySpaceBand);
			}
			EmptySpaceBand emptySpaceBand = reusableEmptySpaceBand;
			emptySpaceBand.HeightF = XRConvert.Convert(e.EmptySpace, GraphicsDpi.Document, this.Dpi);
			OnFillEmptySpace(new BandEventArgs(emptySpaceBand));
			if(emptySpaceBand.Controls.Count > 0) {
				XRWriteInfo writeInfo = new XRWriteInfo(this.PrintingSystem, this.Dpi, e.SpaceDocumentBand, PageBuildInfo.Empty);
				foreach(XRControl control in emptySpaceBand.Controls)
					control.WriteContentTo(writeInfo);
				reusableEmptySpaceBand = null;
			}
		}
		protected internal override void FinishPageSummary() {
			if(pageSummaryBuilder != null)
				pageSummaryBuilder.FinishPageSummary();
			pageSummaryBuilder = null;
			NestedComponentEnumerator en = new NestedComponentEnumerator(Bands);
			while(en.MoveNext()) {
				en.Current.FinishPageSummary();
			}
		}
		protected internal override void CalculatePageSummary(DocumentBand pageBand) {
			if(pageSummaryBuilder == null && !Summaries.PageSummaries.IsEmpty())
				pageSummaryBuilder = new PageSummaryBuilder(new PageSummaryContainer(Summaries.PageSummaries));
			if(pageSummaryBuilder != null)
				pageSummaryBuilder.BuildPageSummary(pageBand);
			NestedComponentEnumerator en = new NestedComponentEnumerator(Bands);
			while(en.MoveNext()) {
				en.Current.CalculatePageSummary(pageBand);
			}
		}
		void IReport.ApplyPageSettings(XtraPageSettingsBase destSettings) {
			MarginsF marginsInDocuments = XRConvert.Convert(new MarginsF(Margins.Left, Margins.Right, Margins.Top, Margins.Bottom), Dpi, GraphicsDpi.Document);
			SizeF pageSizeInDocuments = (PaperKind != PaperKind.Custom) ? PageSizeInfo.GetPageSizeF(PaperKind, GraphicsDpi.Document) : 
				XRConvert.Convert(new SizeF(pageSize), Dpi, GraphicsDpi.Document);
			destSettings.Assign(
				marginsInDocuments, 
				paperKind, 
				pageSizeInDocuments,
				landscape);
			destSettings.AssignPrinterSettings(this.PrinterName, this.PaperName, this.DefaultPrinterSettingsUsing);
			destSettings.RollPaper = RollPaper;
		}
		void IReport.UpdatePageSettings(XtraPageSettingsBase sourceSettings, string paperName) {
			if(!DefaultPrinterSettingsUsing.UseMargins)
				Margins = XRConvert.Convert(sourceSettings.MarginsF, GraphicsDpi.Document, Dpi).Round();
			if(!DefaultPrinterSettingsUsing.UsePaperKind) {
				PaperKind = sourceSettings.PaperKind;
				if(PaperKind == PaperKind.Custom) {
					PaperName = paperName;
					Size pageSize = XRConvert.Convert(sourceSettings.Bounds.Size, GraphicsDpi.HundredthsOfAnInch, Dpi);
					if(sourceSettings.Landscape)
						SetPageSize(pageSize.Height, pageSize.Width);
					else
						SetPageSize(pageSize.Width, pageSize.Height);
					Landscape = sourceSettings.Landscape;
				}
			}
			if(!DefaultPrinterSettingsUsing.UseLandscape)
				Landscape = sourceSettings.Landscape;
		}
		protected internal override void SyncDpi(float dpi) {
			if(dpi != Dpi) {
				if(snapGridSize.HasValue)
					snapGridSize = XRConvert.Convert(snapGridSize.Value, Dpi, dpi);
				margins = XRConvert.ConvertMargins(margins, Dpi, dpi);
				pageSize = (PaperKind != PaperKind.Custom) ? PageSizeInfo.GetPageSize(PaperKind, dpi) : XRConvert.Convert(pageSize, Dpi, dpi);
				styleContainer.SyncDpi(dpi);
				foreach(XRCrossBandControl cbControl in this.CrossBandControls) {
					cbControl.SyncDpi(dpi);
				}
			}
			base.SyncDpi(dpi);
		}
		#region export to text
		public void ExportToText(string path) {
			Guard.ArgumentIsNotNullOrEmpty(path, "path");
			ExportToText(path, exportOptions.Text);
		}
		public void ExportToText(string path, TextExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(path, "path");
			Guard.ArgumentNotNull(options, "options");
			CreateIfEmpty(textFactor * progressRangeBase);
			PrintingSystem.ExportToText(path, options);
		}
		public void ExportToText(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToText(stream, exportOptions.Text);
		}
		public void ExportToText(Stream stream, TextExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			CreateIfEmpty(textFactor * progressRangeBase);
			PrintingSystem.ExportToText(stream, options);
		}
		#endregion
		#region export to csv
		public void ExportToCsv(string path) {
			Guard.ArgumentIsNotNullOrEmpty(path, "path");
			ExportToCsv(path, exportOptions.Csv);
		}
		public void ExportToCsv(string path, CsvExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(path, "path");
			Guard.ArgumentNotNull(options, "options");
			CreateIfEmpty(textFactor * progressRangeBase);
			PrintingSystem.ExportToCsv(path, options);
		}
		public void ExportToCsv(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToCsv(stream, exportOptions.Csv);
		}
		public void ExportToCsv(Stream stream, CsvExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			CreateIfEmpty(textFactor * progressRangeBase);
			PrintingSystem.ExportToCsv(stream, options);
		}
		#endregion
		#region export to xls
		public void ExportToXls(string path) {
			Guard.ArgumentIsNotNullOrEmpty(path, "path");
			ExportToXls(path, exportOptions.Xls);
		}
		public void ExportToXls(string path, XlsExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(path, "path");
			Guard.ArgumentNotNull(options, "options");
			CreateIfEmpty(textFactor * progressRangeBase);
			PrintingSystem.ExportToXls(path, options);
		}
		public void ExportToXls(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToXls(stream, exportOptions.Xls);
		}
		public void ExportToXls(Stream stream, XlsExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			CreateIfEmpty(textFactor * progressRangeBase);
			PrintingSystem.ExportToXls(stream, options);
		}
		#endregion
		#region export to xlsx
		public void ExportToXlsx(string path) {
			Guard.ArgumentIsNotNullOrEmpty(path, "path");
			ExportToXlsx(path, exportOptions.Xlsx);
		}
		public void ExportToXlsx(string path, XlsxExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(path, "path");
			Guard.ArgumentNotNull(options, "options");
			CreateIfEmpty(textFactor * progressRangeBase);
			PrintingSystem.ExportToXlsx(path, options);
		}
		public void ExportToXlsx(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToXlsx(stream, exportOptions.Xlsx);
		}
		public void ExportToXlsx(Stream stream, XlsxExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			CreateIfEmpty(textFactor * progressRangeBase);
			PrintingSystem.ExportToXlsx(stream, options);
		}
		#endregion
		#region export to rtf
		public void ExportToRtf(string path) {
			Guard.ArgumentIsNotNullOrEmpty(path, "path");
			ExportToRtf(path, exportOptions.Rtf);
		}
		public void ExportToRtf(string path, RtfExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(path, "path");
			Guard.ArgumentNotNull(options, "options");
			CreateIfEmpty(textFactor * progressRangeBase);
			PrintingSystem.ExportToRtf(path, options);
		}
		public void ExportToRtf(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToRtf(stream, exportOptions.Rtf);
		}
		public void ExportToRtf(Stream stream, RtfExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			CreateIfEmpty(textFactor * progressRangeBase);
			PrintingSystem.ExportToRtf(stream, options);
		}
		#endregion
		#region export to html
		public void ExportToHtml(string path) {
			Guard.ArgumentIsNotNullOrEmpty(path, "path");
			ExportToHtml(path, exportOptions.Html);
		}
		public void ExportToHtml(string path, HtmlExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(path, "path");
			Guard.ArgumentNotNull(options, "options");
			CreateIfEmpty(htmFactor * progressRangeBase);
			PrintingSystem.ExportToHtml(path, options);
		}
		public void ExportToHtml(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToHtml(stream, exportOptions.Html);
		}
		public void ExportToHtml(Stream stream, HtmlExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			CreateIfEmpty(htmFactor * progressRangeBase);
			PrintingSystem.ExportToHtml(stream, options);
		}
		#endregion
		#region export to mht
		public void ExportToMht(string path) {
			Guard.ArgumentIsNotNullOrEmpty(path, "path");
			ExportToMht(path, exportOptions.Mht);
		}
		public void ExportToMht(string path, MhtExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(path, "path");
			Guard.ArgumentNotNull(options, "options");
			CreateIfEmpty(htmFactor * progressRangeBase);
			PrintingSystem.ExportToMht(path, options);
		}
		public void ExportToMht(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToMht(stream, exportOptions.Mht);
		}
		public void ExportToMht(Stream stream, MhtExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			CreateIfEmpty(htmFactor * progressRangeBase);
			PrintingSystem.ExportToMht(stream, options);
		}
		#endregion
		#region Export to Mail
		public AlternateView ExportToMail() {
			return ExportToMail(exportOptions.MailMessage);
		}
		public AlternateView ExportToMail(MailMessageExportOptions options) {
			Guard.ArgumentNotNull(options, "options");
			CreateIfEmpty(htmFactor * progressRangeBase);
			return PrintingSystem.ExportToMail(options);
		}
		public MailMessage ExportToMail(string from, string to, string subject) {
			return ExportToMail(exportOptions.MailMessage, from, to, subject);
		}
		public MailMessage ExportToMail(MailMessageExportOptions options, string from, string to, string subject) {
			Guard.ArgumentNotNull(options, "options");
			CreateIfEmpty(htmFactor * progressRangeBase);
			return PrintingSystem.ExportToMail(options, from, to, subject);
		}
		#endregion
		#region export to pdf
		public void ExportToPdf(string path) {
			Guard.ArgumentIsNotNullOrEmpty(path, "path");
			ExportToPdf(path, exportOptions.Pdf);
		}
		public void ExportToPdf(string path, PdfExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(path, "path");
			Guard.ArgumentNotNull(options, "options");
			CreateIfEmpty(pdfFactor * progressRangeBase);
			PrintingSystem.ExportToPdf(path, options);
		}
		public void ExportToPdf(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToPdf(stream, exportOptions.Pdf);
		}
		public void ExportToPdf(Stream stream, PdfExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			CreateIfEmpty(pdfFactor * progressRangeBase);
			PrintingSystem.ExportToPdf(stream, options);
		}
		#endregion
		#region export to image
		public void ExportToImage(string path) {
			Guard.ArgumentIsNotNullOrEmpty(path, "path");
			ExportToImage(path, exportOptions.Image);
		}
		public void ExportToImage(string path, ImageExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(path, "path");
			Guard.ArgumentNotNull(options, "options");
			CreateIfEmpty(imageFactor * progressRangeBase);
			PrintingSystem.ExportToImage(path, options);
		}
		public void ExportToImage(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToImage(stream, exportOptions.Image);
		}
		public void ExportToImage(Stream stream, ImageExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			CreateIfEmpty(imageFactor * progressRangeBase);
			PrintingSystem.ExportToImage(stream, options);
		}
		public void ExportToImage(string path, System.Drawing.Imaging.ImageFormat format) {
			Guard.ArgumentIsNotNullOrEmpty(path, "path");
			Guard.ArgumentNotNull(format, "format");
			ExportToImage(path, ExportOptionsHelper.ChangeOldImageProperties(exportOptions.Image, format));
		}
		public void ExportToImage(Stream stream, System.Drawing.Imaging.ImageFormat format) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(format, "format");
			ExportToImage(stream, ExportOptionsHelper.ChangeOldImageProperties(exportOptions.Image, format));
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void CreateLayoutViewDocument() {
			CreateLayoutViewDocument(BrickPresentation.LayoutView);
		}
		internal void CreateLayoutViewDocument(BrickPresentation brickPresentation) {
			if(Bands[BandKind.TopMargin] == null) {
				Band band = new TopMarginBand();
				band.Name = "TopMargin";
				Bands.Add(band);
			}
			if(Bands[BandKind.BottomMargin] == null) {
				Band band = new BottomMarginBand();
				band.Name = "BottomMargin";
				Bands.Add(band);
			}
			if(Bands[BandKind.Detail] == null) {
				Band band = new DetailBand();
				band.Name = "Detail";
				Bands.Add(band);
			}
			DocumentCreator = delegate(PrintingSystemBase ps, Action0 afterBuildPages) {
				return new LayoutViewDocument(ps, afterBuildPages);
			};
			try {
				IterateReportsRecursive(delegate(XtraReportBase item) {
					item.ReportPrintOptions.ActivateDesignTimeProperties(true);
				});
				this.brickPresentation = brickPresentation;
				CreateDocument(false);
			} finally {
				DocumentCreator = null;
				this.brickPresentation = BrickPresentation.Runtime;
				IterateReportsRecursive(delegate(XtraReportBase item) {
					item.ReportPrintOptions.ActivateDesignTimeProperties(false);
				});
			}
		}
		public void CreateDocument() {
			CreateDocument(false);
		}
		public virtual void CreateDocument(bool buildPagesInBackground) {
			TryCreateDocument(0, buildPagesInBackground, false);
		}
		internal void CreateDocument(bool buildPagesInBackground, bool suppressClearDataContext) {
			TryCreateDocument(0, buildPagesInBackground, suppressClearDataContext);
		}
		internal void CreateDocument(float progressRange, bool buildPagesInBackground) {
			TryCreateDocument(progressRange, buildPagesInBackground, false);
		}
		void TryCreateDocument(float progressRange, bool buildPagesInBackground, bool suppressClearDataContext) {
			try {
				if(PrintingSystem.Document.IsCreating)
					PrintingSystem.ClearContent();
				CreateDocument(progressRange, buildPagesInBackground, suppressClearDataContext);
			} catch(ReportCanceledException exception) {
				DevExpress.XtraPrinting.Tracer.TraceError(NativeSR.TraceSource, exception);
			} catch(Exception exception) {
				ExceptionEventArgs args = new ExceptionEventArgs(exception);
				PrintingSystem.OnCreateDocumentException(args);
				if(!args.Handled) {
					DevExpress.XtraPrinting.Tracer.TraceError(NativeSR.TraceSource, exception);
					throw;
				}
			}
		}
		void CreateDocument(float progressRange, bool buildPagesInBackground, bool suppressClearDataContext) {
			if(this.Document.IsCreating)
				throw new Exception("Cannot start creating a document while another document is being created.");
			if(this.IsDisposed)
				throw new Exception("Attempt of using the disposed XtraReport.");
			if(DesignMode)
				throw new InvalidOperationException(ReportLocalizer.GetString(ReportStringId.Msg_InvalidMethodCall));
			styleSheet.ClearCache();
			TraceCenter.ClearHistory();
			this.suppressClearDataContext = suppressClearDataContext;
			InitializeScripts();
			if(!EventsScriptManager.ValidateScripts())
				throw new Exception(String.Format(ReportLocalizer.GetString(ReportStringId.Msg_ScriptError), EventsScriptManager.ErrorMessage));
			List<Parameter> parameters = new List<Parameter>();
			CollectParameters(parameters, p => true);
			bool approved = ReportPrintTool != null ? ReportPrintTool.ApproveParameters(parameters.ToArray(), RequestParameters) : true;
			if(approved || !RequestParameters || parameters.Count<Parameter>(p => p.Visible) == 0) {
				ResetTokenSource();
				if(!RaiseBeforePrint().Cancel)
					CreateDocumentCore(progressRange, buildPagesInBackground);
				else {
					DisposeTokenSource();
					ClearPrintingSystem();
				}
			}
		}
		PrintEventArgs RaiseBeforePrint() {
			PrintEventArgs args = new PrintEventArgs();
			OnBeforePrint(args);
			return args;
		}
		private void ClearPrintingSystem() {
			if(fPrintingSystem != null)
				fPrintingSystem.Document.Dispose();
		}
		void DisposeTokenSource() {
			CancellationService serv = ((IServiceProvider)this).GetService(typeof(ICancellationService)) as CancellationService;
			if(serv != null) serv.DisposeTokenSource();
		}
		void ResetTokenSource() {
			CancellationService serv = ((IServiceProvider)this).GetService(typeof(ICancellationService)) as CancellationService;
			if(serv != null) serv.ResetTokenSource();
		}
		internal void WriteToDocument(DocumentBuilderBase docBuilder, PrintingSystemBase ps) {
			PrintingSystemBase save = fPrintingSystem;
			fPrintingSystem = ps;
			creatingPrintingSystem = ps;
			try {
				WriteToDocument(docBuilder);
			} finally {
				fPrintingSystem = save;
				creatingPrintingSystem = null;
			}
		}
		private void CreateDocumentCore(float progressRange, bool buildPagesInBackground) {
			PrintingSystemBase ps = PrintingSystem;
			if(ps.GetService<ICancellationService>().IsCancellationRequested())
				return;
			creatingPrintingSystem = ps;
			IDrillDownService serv = ((IServiceProvider)this).GetService(typeof(IDrillDownService)) as IDrillDownService;
			if(serv != null) {
				serv.Clear();
				if(!serv.IsDrillDowning)
					serv.Reset();
			}
			ps.PerformIfNotNull<GroupingManager>(item => item.GroupKeys.Clear());
			DevExpress.XtraPrinting.Tracer.TraceData(NativeSR.TraceSource, TraceEventType.Start, () => { return this.GetTraceName() + TraceSR.MessageSeparator + TraceSR.DocumentCreation; });
			ps.Graph.PageBackColor = PageColor;
			ps.Unlock();
			ps.Graph.PageUnit = GraphicsUnit.Document;
			ps.ClearContent();
			reusableEmptySpaceBand = null;
			componentsCollector.Collect();
			PSDocument doc = DocumentCreator(ps, AfterBuildPagesProc);
			doc.VerticalContentSplitting = this.VerticalContentSplitting;
			doc.HorizontalContentSplitting = this.HorizontalContentSplitting;
			doc.BookmarkDuplicateSuppress = this.BookmarkDuplicateSuppress;
			doc.CopyScaleParameters(ps.Document as PrintingDocument);
			doc.NavigationInfo = new NavigationInfo();
			ps.SetDocument(doc);
			ps.Begin();
			SetProgressRanges(progressRange);
			if(!ps.PageSettings.IsPresetted)
				((IReport)this).ApplyPageSettings(ps.PageSettings);
			ps.PageSettings.IsPresetted = false;
			ps.ShowMarginsWarning = this.ShowPrintMarginsWarning;
			ps.ShowPrintStatusDialog = this.ShowPrintStatusDialog;
			DocumentBuilderBase documentBuilderBase = CreateDocumentBuilder(doc);
			doc.Root.BandManager = documentBuilderBase;
			BuildDocument(documentBuilderBase);
			ps.Watermark.CopyFrom(Watermark);
			ps.ExportOptions.Assign(exportOptions);
			ps.Document.Name = StringHelper.GetNonEmptyValue(DisplayName, Name, GetType().Name);
			ps.Document.Bookmark = StringHelper.GetNonEmptyValue(Bookmark, Name, ps.Document.Name);
			ps.End(buildPagesInBackground);
		}
		protected virtual void SetProgressRanges(float progressRange) {
			List<object> components;
			int rangeCount = GetRangeCount(out components);
			float singleRange = progressRangeBase / rangeCount;
			List<float> ranges = new List<float>(rangeCount);
			for(int i = 0; i < rangeCount; i++)
				ranges.Add(singleRange);
			ranges.Add(pageBuildFactor * progressRangeBase);
			if(progressRange > 0)
				ranges.Add(progressRange);
			PrintingSystem.ProgressReflector.SetProgressRanges(
				ranges.ToArray(),
				new XRProgressReflectorLogic(PrintingSystem.ProgressReflector, components));
		}
		int GetRangeCount(out List<object> accessors) {
			accessors = new List<object>();
			int result = 0;
			DetailBand detailBand = null;
			foreach(Band item in this.Bands) {
				if(item is DetailReportBand) {
					result++;
					accessors.Add(item);
				}
				if(detailBand == null && item is DetailBand)
					detailBand = (DetailBand)item;
			}
			if(detailBand != null)
				foreach(XRControl item in detailBand.Controls)
					if(item is XRSubreport) {
						result++;
						accessors.Add(item);
					}
			if(result == 0) {
				accessors.Add(this);
				return 1;
			}
			int rowCount = DisplayableRowCount != 0 ? DisplayableRowCount :
				detailBand != null ? 1 :
				0;
			return result * rowCount;
		}
		[Obsolete("Use CreatePSDocument(PrintingSystem, Action0) overload.")]
		protected virtual PSDocument CreatePSDocument(PrintingSystemBase ps, MethodInvoker afterBuildPages) {
			return new PSDocument(ps, afterBuildPages.Invoke);
		}
		Function2<PSDocument, PrintingSystemBase, Action0> documentCreator;
		protected Function2<PSDocument, PrintingSystemBase, Action0> DocumentCreator {
			get {
				return documentCreator != null ? documentCreator :
					delegate(PrintingSystemBase ps, Action0 afterBuildPages) {
						return new PSDocument(ps, afterBuildPages);
					};
			}
			set {
				documentCreator = value;
			}
		}
		protected virtual DocumentBuilderBase CreateDocumentBuilder(PSDocument doc) {
			return MasterReport != null && BrickPresentation == BrickPresentation.LayoutView ?
				(DocumentBuilderBase)new DetailReportDocumentBuilder(this, doc.Root) :
				new RootReportBuilder(this, doc.Root);
		}
		private void AfterBuildPagesProc() {
			if(!Summaries.PageSummaries.IsEmpty())
			   new PageSummaryContainer(Summaries.PageSummaries).OnReportFinished();
			NavigationInfo navigationInfo = Document.NavigationInfo;
			if(navigationInfo != null) {
				new DocumentMapBuilder(Document).Build(navigationInfo.BookmarkBricks);
				new NavigationBuilder(navigationManager).SetNavigationPairs(navigationInfo.NavigationBricks, Pages);
				Document.NavigationInfo.Clear();
			}
			PrintingSystem.Lock();
			PrintingSystem.OnDocumentChanged(EventArgs.Empty);
			OnAfterPrint(new System.Drawing.Printing.PrintEventArgs());
			this.Summaries.ResetRunningCalculation();
#if DEBUGTEST
			if(BrickPresentation != BrickPresentation.LayoutView)
				CheckPrintingSystemsInSubreports(this);
#endif
			creatingPrintingSystem = null;
			DisposeTokenSource();
			DevExpress.XtraPrinting.Tracer.TraceData(NativeSR.TraceSource, TraceEventType.Stop, () => { return this.GetTraceName() + TraceSR.MessageSeparator + TraceSR.DocumentCreation; });
		}
		private void CreateIfEmpty(float progressRange) {
			CreateIfEmpty(progressRange, false);
		}
		internal void CreateIfEmpty(float progressRange, bool buildPagesInBackground) {
			if(PrintingSystem.Document.IsCreating && buildPagesInBackground)
				return;
			if(PrintingSystem.Document.IsCreating && !buildPagesInBackground)
				PrintingSystem.ClearContent();
			if(PrintingSystem.Document.PageCount == 0)
				CreateDocument(progressRange, buildPagesInBackground);
		}
		internal int GetReportHorizSpace() {
			return PageWidth - Margins.Left;
		}
		protected internal override void SaveLayoutInternal(Stream stream) {
			DesignItemList components = new DesignItemList(this);
			base.CollectAssociatedComponents(components);
			components.Add(this);
			CodeGeneratorAccessorBase.Instance.GenerateCSharpCode(this, components, stream, false);
		}
		internal protected override bool ShouldSerializeBackColor() {
			return BackColor != XRControlStyle.Default.BackColor && base.ShouldSerializeBackColor();
		}
		internal protected override bool ShouldSerializeForeColor() {
			return ForeColor != XRControlStyle.Default.ForeColor && base.ShouldSerializeForeColor();
		}
		internal protected override bool ShouldSerializeBorderColor() {
			return BorderColor != XRControlStyle.Default.BorderColor && base.ShouldSerializeBorderColor();
		}
		internal protected override bool ShouldSerializeBorderWidth() {
			return BorderWidth != XRControlStyle.Default.BorderWidth && base.ShouldSerializeBorderWidth();
		}
		internal protected override bool ShouldSerializePadding() {
			return Padding != XRControlStyle.Default.Padding && base.ShouldSerializePadding();
		}
		internal protected override bool ShouldSerializeBorders() {
			return Borders != XRControlStyle.Default.Borders && base.ShouldSerializeBorders();
		}
		internal protected override bool ShouldSerializeBorderDashStyle() {
			return BorderDashStyle != XRControlStyle.Default.BorderDashStyle && base.ShouldSerializeBorderDashStyle();
		}
		internal protected override bool ShouldSerializeFont() {
			return Font != XRControlStyle.Default.Font && base.ShouldSerializeFont();
		}
		internal protected override bool ShouldSerializeTextAlignment() {
			return TextAlignment != XRControlStyle.Default.TextAlignment && base.ShouldSerializeTextAlignment();
		}
		bool ShouldSerializeWatermark() {
			return !XRWatermark.Equals(Watermark, new XRWatermark());
		}
		void ResetWatermark() {
			watermark = new XRWatermark();
		}
		bool ShouldSerializeFormattingRuleSheet() {
			return FormattingRuleSheet.Count != 0;
		}
		bool ShouldSerializeExportOptions() {
			return exportOptions.ShouldSerialize();
		}
		void ResetExportOptions() {
			exportOptions = new DevExpress.XtraPrinting.ExportOptions();
		}
		bool ShouldSerializeStyleSheet() {
			return styleSheet.Count != 0;
		}
		bool ShouldSerializeScriptReferences() {
			return !String.IsNullOrEmpty(scriptReferences);
		}
		bool ShouldSerializeScripts() {
			return !Scripts.IsDefault();
		}
		void ResetScripts() {
			CreateScripts();
		}
		bool ShouldSerializeCalculatedFields() {
			return calculatedFields.Count != 0;
		}
		bool ShouldSerializeDesignerOptions() {
			return designerOptions.ShouldSerialize();
		}
		void ResetDesignerOptions() {
			designerOptions = new DesignerOptions();
		}
		bool ShouldSerializeParameters() {
			return parameters.Count != 0;
		}
		bool ShouldSerializeDefaultPrinterSettingsUsing() {
			return defaultPrinterSettingsUsing.ShouldSerialize();
		}
		void ResetDefaultPrinterSettingsUsing() {
			defaultPrinterSettingsUsing = new XRPrinterSettingsUsing();
		}
		void ResetPageColor() {
			pageColor = Color.White;
		}
		bool ShouldSerializeVersion() {
			return true;
		}
		bool ShouldSerializePageColor() {
			return pageColor != Color.White;
		}
		public void StopPageBuilding() {
			this.PrintingSystem.Document.StopPageBuilding();
		}
		public CompilerErrorCollection ValidateScripts() {
			return ValidateScripts(new string[] { });
		}
		internal virtual CompilerErrorCollection ValidateScripts(string[] references) {
			List<string> scriptReferences = new List<string>(ScriptReferences);
			scriptReferences.AddRange(references);
			string[] savedReferences = ScriptReferences;
			try {
				ScriptReferences = scriptReferences.ToArray();
				InitializeScripts();
				if(!EventsScriptManager.ValidateScripts())
					return EventsScriptManager.Errors;
				return new CompilerErrorCollection();
			} finally {
				ScriptReferences = savedReferences;
			}
		}
		#region IServiceProvider Members
		object IServiceProvider.GetService(Type serviceType) {
			if(serviceType == typeof(DataContext))
				return DataContext;
			object service = ((IServiceProvider)PrintingSystem).GetService(serviceType);
			if(service == null && MasterReport != null)
				service = ((IServiceProvider)MasterReport).GetService(serviceType);
			if(service != null)
				return service;
			if(serviceType == typeof(IDrillDownService) || serviceType == typeof(IDrillDownServiceBase)) {
				if(drillDownService == null)
					drillDownService = new DrillDownService();
				return drillDownService;
			} else if(serviceType == typeof(UrlResolver)) {
				if(urlResolver == null)
					urlResolver = new UrlResolver();
				return urlResolver;
			}
			return service;
		}
		#endregion
		protected override void SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			if(propertyName == XtraReportsSerializationNames.StyleSheet)
				StyleSheet.Add((XRControlStyle)e.Item.Value);
			else if(propertyName == XtraReportsSerializationNames.CalculatedFields)
				CalculatedFields.Add((CalculatedField)e.Item.Value);
			else if(propertyName == XtraReportsSerializationNames.FormattingRuleSheet)
				formattingRuleSheet.Add((FormattingRule)e.Item.Value);
			else if(propertyName == XtraReportsSerializationNames.Parameters)
				Parameters.Add((Parameter)e.Item.Value);
			else if(propertyName == XtraReportsSerializationNames.CrossBandControls)
				crossBandControls.Add((XRCrossBandControl)e.Item.Value);
			else if(propertyName == XtraReportsSerializationNames.ScriptSecurityPermissions)
				ScriptSecurityPermissions.Add((ScriptSecurityPermission)e.Item.Value);
			else if(propertyName == XtraReportsSerializationNames.Extensions) {
				SerializableKeyValuePair<string, string> pair = (SerializableKeyValuePair<string, string>)e.Item.Value;
				Extensions.Add(pair.Key, pair.Value);
			} else if(propertyName == XtraReportsSerializationNames.ComponentStorage)
				ComponentStorage.Add((IComponent)e.Item.Value);
			else if(propertyName == XtraReportsSerializationNames.ObjectStorage)
				ObjectStorage.Add((IObject)e.Item.Value);
			else
				base.SetIndexCollectionItem(propertyName, e);
		}
		protected override object CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if(propertyName == XtraReportsSerializationNames.StyleSheet)
				return new XRControlStyle();
			else if(propertyName == XtraReportsSerializationNames.CalculatedFields)
				return new CalculatedField();
			else if(propertyName == XtraReportsSerializationNames.FormattingRuleSheet)
				return new FormattingRule();
			else if(propertyName == XtraReportsSerializationNames.Parameters)
				return CreateParameter(e);
			else if(propertyName == XtraReportsSerializationNames.CrossBandControls)
				return CreateControl(e);
			else if(propertyName == XtraReportsSerializationNames.ScriptSecurityPermissions)
				return new ScriptSecurityPermission(e.Item.ChildProperties["Name"].Value as string);
			else if(propertyName == XtraReportsSerializationNames.Extensions)
				return new SerializableKeyValuePair<string, string>();
			else if(propertyName == XtraReportsSerializationNames.ComponentStorage) {
				string typeName;
				if(e.Item.TryGetChildPropertyValue("ObjectType", out typeName)) {
					Type itemType = TypeResolver.GetType(typeName, TypeResolvingOptions.IgnoreAssemblyName);
					return Activator.CreateInstance(itemType);
				}
				string content;
				if(e.Item.TryGetChildPropertyValue("Type", out typeName) && e.Item.TryGetChildPropertyValue("Content", out content)) {
					object result;
					if(SerializationService.DeserializeObject(content, typeName, out result, this)) {
						string name;
						if(e.Item.TryGetChildPropertyValue("Name", out name))
							componentNames[result] = name;
						return result;
					}
				}
				return null;
			} else if(propertyName == XtraReportsSerializationNames.ObjectStorage) {
				string typeName;
				if(e.Item.TryGetChildPropertyValue("ObjectType", out typeName)) {
					Type itemType = TypeResolver.GetType(typeName, TypeResolvingOptions.IgnoreAssemblyName);
					return Activator.CreateInstance(itemType);
				}
				return new ObjectStorageInfo();
			}
			return base.CreateCollectionItem(propertyName, e);
		}
		protected override void OnStartDeserializing(LayoutAllowEventArgs e) {
			base.OnStartDeserializing(e);
			componentNames.Clear();
		}
		protected override void OnEndDeserializing(string restoredVersion) {
			base.OnEndDeserializing(restoredVersion);
			InitFields();
		}
		protected internal override void OnReportEndDeserializing() {
			NestedComponentEnumerator nestedComponentEnumerator = new NestedComponentEnumerator(Bands);
			while (nestedComponentEnumerator.MoveNext()) {
				nestedComponentEnumerator.Current.OnReportEndDeserializing();
			}
			foreach (XRCrossBandControl control in CrossBandControls) {
				control.OnReportEndDeserializing();
			}
		}
		protected static Parameter CreateParameter(XtraItemEventArgs e) {
			string typeName;
			if(e.Item.TryGetChildPropertyValue("ObjectType", out typeName))
				return Activator.CreateInstance(XRControlExtensions.GetObjectType(typeName)) as Parameter;
			return new Parameter();
		}
		internal void ChildDataSourceChanged() {
			if(Summaries != null)
				Summaries.ResetReportCache();
		}
	}
}
namespace DevExpress.XtraReports {
	public class ReportCanceledException : Exception {
		public ReportCanceledException(string message)
			: base(message) {
		}
		protected ReportCanceledException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
	}
}
