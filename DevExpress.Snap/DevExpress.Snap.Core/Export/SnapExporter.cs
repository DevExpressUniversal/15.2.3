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
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using DevExpress.Data;
using DevExpress.DataAccess;
using DevExpress.Office.Utils;
using DevExpress.Utils.Zip;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.BarCode;
using DevExpress.XtraReports.Native;
using DevExpress.XtraRichEdit.Export.OpenXml;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core.Native.Data.Implementations;
using DevExpress.Snap.Core.Options;
using DevExpress.DataAccess.Native;
using CalculatedFieldType = DevExpress.XtraReports.UI.FieldType;
using SnapDataContainer = DevExpress.XtraRichEdit.Model.IDataContainer;
using Theme = DevExpress.Snap.Core.Native.Theme;
using ThemeCollection = DevExpress.Snap.Core.Native.ThemeCollection;
namespace DevExpress.Snap.Core.Export {
	#region SnapExporter
	public class SnapExporter : OpenXmlExporter {
		#region Fields
		readonly SnapFieldsContextExporter fieldContextExporter;
		readonly Dictionary<Type, Action<CustomRun>> customRunHandlers;
		public const string RelsDataSettings = "http://devexpress.com/snap/2012/relationships/datasettings";
		public const string RelsFieldContext = "http://devexpress.com/snap/2012/relationships/fieldcontexts";
		public const string RelsDataContainer = "http://devexpress.com/snap/2012/relationships/datacontainer";
		public const string RelsDataSources = "http://devexpress.com/snap/2012/relationships/datasources";
		public const string RelsTheme = "http://devexpress.com/snap/2012/relationships/theme";
		public const string RelsSnapMailMergeOptions = "http://devexpress.com/snap/2012/relationships/snapmailmergeoptions";
		public const int CurrentVersion = 4;
		FieldPathService fieldPathService;
		Dictionary<SnapBookmark, string> snapBookmarkIds;
		Dictionary<SnapTemplateInterval, string> snapTemplateIntervalIds;
		Dictionary<RootFieldContext, string> rootFieldContexts;
		Dictionary<string, string> dataSettingsRelationsTable;
		Dictionary<string, string> dataSourcesRelationsTable;
		#endregion
		public SnapExporter(SnapDocumentModel documentModel, SnapDocumentExporterOptions options)
			: base(documentModel, options) {
			customRunHandlers = CreateCustomRunHandlers();
			fieldContextExporter = new SnapFieldsContextExporter(documentModel);
			this.dataSourcesRelationsTable = new Dictionary<string, string>();
		}
		#region Properties
		public Dictionary<string, string> DataSettingsRelationsTable { get { return dataSettingsRelationsTable; } }
		public new SnapDocumentModel DocumentModel { get { return (SnapDocumentModel)base.DocumentModel; } }
		Dictionary<SnapBookmark, string> SnapBookmarkIds {
			get {
				if (snapBookmarkIds == null)
					snapBookmarkIds = new Dictionary<SnapBookmark, string>();
				return snapBookmarkIds;
			}
		}
		Dictionary<SnapTemplateInterval, string> SnapTemplareIntervalIds {
			get {
				if (snapTemplateIntervalIds == null)
					snapTemplateIntervalIds = new Dictionary<SnapTemplateInterval, string>();
				return snapTemplateIntervalIds;
			}
		}
		Dictionary<RootFieldContext, string> RootFieldContexts {
			get {
				if (rootFieldContexts == null)
					rootFieldContexts = new Dictionary<RootFieldContext, string>();
				return rootFieldContexts;
			}
		}
		protected FieldPathService FieldPathService {
			get {
				if (fieldPathService == null) {
					IFieldDataAccessService service = DocumentModel.GetService<IFieldDataAccessService>();
					if (service != null)
						fieldPathService = service.FieldPathService as FieldPathService;
				}
				return fieldPathService;
			}
		}
		#endregion
		Dictionary<Type, Action<CustomRun>> CreateCustomRunHandlers() {
			Dictionary<Type, Action<CustomRun>> result = new Dictionary<Type, Action<CustomRun>>();
			result.Add(typeof(BarCodeRunObject), ExportBarCodeRun);
			result.Add(typeof(CheckBoxRunObject), ExportCheckBoxRun);
			return result;
		}
		protected internal override void InitializeExport() {
			this.dataSettingsRelationsTable = new Dictionary<string, string>();
			this.dataSettingsRelationsTable.Add("RelDS1", "dataSettings.xml");
			base.InitializeExport();
		}
		protected internal override void GenerateDocumentRelations(XmlWriter writer) {
			base.GenerateDocumentRelations(writer);
			GenerateFieldContextsRelations(writer);
			GenerateDataRelations(writer);
			GenerateThemeRelations(writer);
			GenerateMailMergeRelations(writer);
		}
		void GenerateFieldContextsRelations(XmlWriter writer) {
			Dictionary<string, string> fieldContextsRelationsTable = new Dictionary<string, string>();
			fieldContextsRelationsTable.Add("RelFC1", "fieldContexts.xml");
			GenerateFileRelationsCore(writer, fieldContextsRelationsTable, RelsFieldContext);
		}
		void GenerateDataRelations(XmlWriter writer) {
			GenerateFileRelationsCore(writer, dataSettingsRelationsTable, RelsDataSettings);
			Dictionary<string, string> dataContainerRelationsTable = new Dictionary<string, string>();
			dataContainerRelationsTable.Add("RelDC1", "dataContainer.xml");
			GenerateFileRelationsCore(writer, dataContainerRelationsTable, RelsDataContainer);
			GenerateFileRelationsCore(writer, this.dataSourcesRelationsTable, RelsDataSources);
		}
		void GenerateThemeRelations(XmlWriter writer) {
			Dictionary<string, string> themeRelationsTable = new Dictionary<string, string>();
			themeRelationsTable.Add("RelThm", "themes.xml");
			GenerateFileRelationsCore(writer, themeRelationsTable, RelsTheme);
		}
		void GenerateMailMergeRelations(XmlWriter writer) {
			Dictionary<string, string> relationsTable = new Dictionary<string, string>();
			relationsTable.Add("RelMMO", "snapMailMergeOptions.xml");
			GenerateFileRelationsCore(writer, relationsTable, RelsSnapMailMergeOptions);
		}
		protected internal override void PushVisitableDocumentIntervalBoundaryIterator() {
			VisitableDocumentIntervalsIteratorStack.Push(new SnapVisitableDocumentIntervalBasedObjectBoundaryIterator((SnapPieceTable)PieceTable));
		}
		protected internal override void RegisterNamespaces() {
			base.RegisterNamespaces();
			DocumentContentWriter.WriteAttributeString("xmlns", SnapContentWriter.SnapPrefix, null, SnapContentWriter.SnapNamespaceConstant);
		}
		protected internal virtual string CalcSnapBookmarkId(SnapBookmark bookmark) {
			string id;
			if (!SnapBookmarkIds.TryGetValue(bookmark, out id)) {
				id = String.Format("snapBookmark{0}", SnapBookmarkIds.Count);
				SnapBookmarkIds.Add(bookmark, id);
			}
			return id;
		}
		protected internal virtual string CalcSnapTemplateIntervalId(SnapTemplateInterval interval) {
			string id;
			if (!SnapTemplareIntervalIds.TryGetValue(interval, out id)) {
				id = String.Format("templateInterval{0}", SnapTemplareIntervalIds.Count);
				SnapTemplareIntervalIds.Add(interval, id);
			}
			return id;
		}
		protected internal virtual string CalcRootFieldContextId(RootFieldContext context) {
			string id;
			if (!RootFieldContexts.TryGetValue(context, out id)) {
				id = String.Format("rootContext{0}", RootFieldContexts.Count);
				RootFieldContexts.Add(context, id);
			}
			return id;
		}
		protected internal override void ExportSeparatorTextRun(SeparatorTextRun run) {
			WriteWpStartElement("r");
			try {
				ExportRunProperties(run);
				ExportSeparatorRunCore();
			}
			finally {
				WriteWpEndElement();
			}
		}
		void ExportSeparatorRunCore() {
			WriteDxStartElement("spr");
			WriteDxEndElement();
		}
		protected internal override void ExportCustomRun(CustomRun run) {
			Action<CustomRun> action;
			if (customRunHandlers.TryGetValue(run.CustomRunObject.GetType(), out action)) {
				WriteWpStartElement("r");
				WriteDxStartElement("customObject");
				try {
					action(run);
				}
				finally {
					WriteDxEndElement();
					WriteWpEndElement();
				}
			}
		}
		protected internal override void ExportDataContainerRun(DataContainerRun run) {
			SnapDataContainer dataContainer = run.DataContainer;
			WriteWpStartElement("r");
			WriteDxStartElement("dataContainer");
			WriteDxStringAttr("type", GetDataContainerType(dataContainer.GetType()));
			try {
				WriteDxByteArray(dataContainer.GetData());
			}
			finally {
				WriteDxEndElement();
				WriteWpEndElement();
			}
		}
		string GetDataContainerType(Type type) {
			if (type == typeof(Base64StringDataContainer))
				return "base64StringDataContainer";
			return String.Empty;
		}
		protected override void ExportInlinePictureImageReference(InlinePictureRun run) {
			ChartRun chartRun = run as ChartRun;
			SparklineRun sparklineRun = run as SparklineRun;
			if (chartRun != null)
				ExportChartImageReference(chartRun);
			else if(sparklineRun != null)
				ExportSparklineImageReference(sparklineRun);
			else
				base.ExportInlinePictureImageReference(run);
		}
		void ExportChartImageReference(ChartRun run) {
			ExportImageReferenceCore(run, "chart");
		}
		internal void ExportSparklineImageReference(SparklineRun run) {
			ExportImageReferenceCore(run, "sparkline");
		}
		void SetActualSize(IRectangularObject rectangularObject) {
			WriteDxStartElement("actualSize");
			WriteDxStringAttr("height", rectangularObject.ActualSize.Height.ToString(NumberFormatInfo.InvariantInfo));
			WriteDxStringAttr("width", rectangularObject.ActualSize.Width.ToString(NumberFormatInfo.InvariantInfo));
			WriteDxEndElement();
		}
		#region BarCodeRun
		void ExportBarCodeRun(CustomRun run) {
			BarCodeRunObject barCode = run.CustomRunObject as BarCodeRunObject;
			WriteDxStringAttr("type", "barCode");
			SetActualSize(barCode);
			SetBarCodeGenerator(barCode);
			SetText(barCode);
			SetAlignment(barCode);
			SetTextAlignment(barCode);
			SetOrientation(barCode);
			SetModule(barCode);
			SetAutoModule(barCode);
			SetShowText(barCode);
		}
		void SetBarCodeGenerator(BarCodeRunObject barCode) {
			WriteDxStringElement("barCodeGenerator", SNBarCodeHelper.GetGeneratorBase64String(barCode.BarCodeGenerator));
		}
		void SetText(BarCodeRunObject barCode) {
			WriteDxStringElement("text", barCode.Text);
		}
		void SetAlignment(BarCodeRunObject barCode) {
			WriteDxStringElement("alignment", Enum.GetName(typeof(TextAlignment), barCode.Alignment));
		}
		void SetTextAlignment(BarCodeRunObject barCode) {
			WriteDxStringElement("textAlignment", Enum.GetName(typeof(TextAlignment), barCode.TextAlignment));
		}
		void SetOrientation(BarCodeRunObject barCode) {
			WriteDxStringElement("orientation", Enum.GetName(typeof(BarCodeOrientation), barCode.Orientation));
		}
		void SetModule(BarCodeRunObject barCode) {
			WriteDxIntElement("module", (int)Math.Round(barCode.Module * 1000));
		}
		void SetAutoModule(BarCodeRunObject barCode) {
			WriteDxBoolElement("autoModule", barCode.AutoModule);
		}
		void SetShowText(BarCodeRunObject barCode) {
			WriteDxStartElement("showText");
			WriteDxBoolAttr("val", barCode.ShowText);
			WriteDxEndElement();
		}
		#endregion
		#region CheckBoxRun
		void ExportCheckBoxRun(CustomRun run) {
			CheckBoxRunObject checkBox = (CheckBoxRunObject)run.CustomRunObject;
			WriteDxStringAttr("type", "checkBox");
			WriteDxStartElement("checkState");
			WriteDxStringAttr("val", Enum.GetName(typeof(CheckState), checkBox.CheckState));
			WriteDxEndElement();
		}
		#endregion
		protected internal virtual void ExportSnapBookmarkStart(SnapBookmark bookmark) {
			WriteDxStartElement("bookmarkStart");
			try {
				WriteDxStringAttr("id", CalcSnapBookmarkId(bookmark));
				WriteDxStringAttr("type", "snapBookmark");
				if (bookmark.Parent != null)
					WriteDxStringAttr("parentId", CalcSnapBookmarkId(bookmark.Parent));
				IFieldContext fieldContext = bookmark.FieldContext;
				WriteDxIntAttr("fieldContextId", fieldContextExporter.ExportFieldContext(fieldContext));
				WriteDxStringAttr("templateIntervalId", CalcSnapTemplateIntervalId(bookmark.TemplateInterval));
				if (bookmark.HeaderBookmark != null)
					WriteDxStringAttr("headerBookmarkId", CalcSnapBookmarkId(bookmark.HeaderBookmark));
				if (bookmark.FooterBookmark != null)
					WriteDxStringAttr("footerBookmarkId", CalcSnapBookmarkId(bookmark.FooterBookmark));
				ExportSnapTemplateInfo(bookmark.TemplateInterval.TemplateInfo);
			}
			finally {
				WriteDxEndElement();
			}
		}
		protected internal virtual void ExportSnapBookmarkEnd(SnapBookmark bookmark) {
			WriteDxStartElement("bookmarkEnd");
			try {
				WriteDxStringAttr("id", CalcSnapBookmarkId(bookmark));
				WriteDxStringAttr("type", "snapBookmark");
			}
			finally {
				WriteDxEndElement();
			}
		}
		protected internal virtual void ExportSnapTemplateIntervalStart(SnapTemplateInterval interval) {
			WriteDxStartElement("bookmarkStart");
			try {
				WriteDxStringAttr("id", CalcSnapTemplateIntervalId(interval));
				WriteDxStringAttr("type", "snapTemplateInterval");
			}
			finally {
				WriteDxEndElement();
			}
		}
		void ExportSnapTemplateInfo(SnapTemplateInfo info) {
			SetFieldInGroupCount(info);
			SetFirstGroupIndex(info);
			SetLastGroupIndex(info);
			SetTemplateType(info);
			SetFirstGroupBookmark(info);
			SetFirstListBookmark(info);
			SetLastGroupBookmark(info);
			SetLastListBookmark(info);
		}
		protected internal override void ExportTableCellStyles() {
			TableCellStyleCollection styles = DocumentModel.TableCellStyles;
			int count = styles.Count;
			for (int i = 0; i < count; i++) {
				ExportTableCellStyle(i);
			}
		}
		protected internal override bool AllowExportTableCellProperties(TableCell cell) {
			if (base.AllowExportTableCellProperties(cell))
				return true;
			return cell.StyleIndex > 0;
		}
		protected internal override void WriteTableCellStyle(TableCell cell) {
			if (cell.StyleIndex > 0)
				WriteWpStringValue("tblCStyle", GetTableCellStyleId(cell.StyleIndex));
		}
		protected internal override void ExportTablePropertiesCore(TableProperties tableProperties) {
			base.ExportTablePropertiesCore(tableProperties);
			if (tableProperties.UseAvoidDoubleBorders)
				WriteWpBoolValue("adb", tableProperties.AvoidDoubleBorders);
		}
		void SetFieldInGroupCount(SnapTemplateInfo info) {
			WriteDxIntElement("fieldInGroupCount", info.FieldInGroupCount);
		}
		void SetFirstGroupIndex(SnapTemplateInfo info) {
			WriteDxIntElement("firstGroupIndex", info.FirstGroupIndex);
		}
		void SetLastGroupIndex(SnapTemplateInfo info) {
			WriteDxIntElement("lastGroupIndex", info.LastGroupIndex);
		}
		void SetTemplateType(SnapTemplateInfo info) {
			WriteDxStringElement("templateType", Enum.GetName(typeof(SnapTemplateIntervalType), info.TemplateType));
		}
		void SetFirstGroupBookmark(SnapTemplateInfo info) {
			SetBookmarkInfo("firstGroupBookmark", info.FirstGroupBookmark);
		}
		void SetFirstListBookmark(SnapTemplateInfo info) {
			SetBookmarkInfo("firstListBookmark", info.FirstListBookmark);
		}
		void SetLastGroupBookmark(SnapTemplateInfo info) {
			SetBookmarkInfo("lastGroupBookmark", info.LastGroupBookmark);
		}
		void SetLastListBookmark(SnapTemplateInfo info) {
			SetBookmarkInfo("lastListBookmark", info.LastListBookmark);
		}
		void SetBookmarkInfo(string attr, SnapBookmark bookmark) {
			if (bookmark != null && !bookmark.Deleted)
				WriteDxStringElement(attr, CalcSnapBookmarkId(bookmark));
		}
		protected internal virtual void ExportSnapTemplateIntervalEnd(SnapTemplateInterval interval) {
			WriteDxStartElement("bookmarkEnd");
			try {
				WriteDxStringAttr("type", "snapTemplateInterval");
				WriteDxStringAttr("id", CalcSnapTemplateIntervalId(interval));
			}
			finally {
				WriteDxEndElement();
			}
		}
		protected internal virtual void WriteDxStartElement(string tag) {
			DocumentContentWriter.WriteStartElement(SnapContentWriter.SnapPrefix, tag, SnapContentWriter.SnapNamespaceConstant);
		}
		protected internal virtual void WriteDxStringAttr(string attr, string value) {
			DocumentContentWriter.WriteAttributeString(SnapContentWriter.SnapPrefix, attr, SnapContentWriter.SnapNamespaceConstant, value);
		}
		protected internal virtual void WriteDxBoolAttr(string attr, bool value) {
			WriteDxStringAttr(attr, ConvertBoolToString(value));
		}
		protected internal virtual void WriteDxIntAttr(string attr, int value) {
			WriteDxStringAttr(attr, value.ToString(NumberFormatInfo.InvariantInfo));
		}
		protected internal virtual void WriteDxByteArray(byte[] buffer) {
			DocumentContentWriter.WriteBase64(buffer, 0, buffer.Length);
		}
		protected internal virtual void WriteDxEndElement() {
			DocumentContentWriter.WriteEndElement();
		}
		protected internal virtual void WriteXElement(XElement element) {
			element.WriteTo(DocumentContentWriter);
		}
		protected internal override void AddCompressedPackages() {
			base.AddCompressedPackages();
			AddCompressedPackageContent(@"word\dataSettings.xml", ExportDataSettings());
			AddCompressedPackageContent(@"word\fieldContexts.xml", ExportFieldContexts());
			AddCompressedPackageContent(@"word\snapMailMergeOptions.xml", ExportSnapMailMergeOptions());
			AddThemesContent();
			AddDataSourcesContent();
		}
		#region Theme
		CompressedStream ExportThemeImageRelations() {
			return CreateCompressedXmlContent(GenerateThemeImageRelations);
		}
		protected internal virtual void GenerateThemeImageRelations(XmlWriter writer) {
			writer.WriteStartElement("Relationships", PackageRelsNamespace);
			GenerateFileRelationsCore(writer, ImageRelationsTable, RelsImage);
			writer.WriteEndElement();
		}
		void AddThemesContent() {
			ThemeCollection themesForExport = GetThemesForExport();
			ImageRelationsTableStack.Push(new Dictionary<string, string>());
			ExportedImageTableStack.Push(new Dictionary<OfficeImage, string>());
			try {
				AddCompressedPackageContent(@"word\themes.xml", CreateCompressedXmlContent(xmlWriter => GenerateThemesContent(xmlWriter, themesForExport)));
				AddCompressedPackageContent(@"word\_rels\themes.xml.rels", ExportThemeImageRelations());
			}
			finally {
				ImageRelationsTableStack.Pop();
				ExportedImageTableStack.Pop();
			}
		}
		protected internal virtual void GenerateThemesContent(XmlWriter writer, ThemeCollection themes) {
			DocumentContentWriter = writer;
			WriteDxStartElement("themes");
			try {
				ExportActiveTheme();
				int themesCount = themes.Count;
				for (int i = 0; i < themesCount; i++) {
					Theme theme = themes[i];
					string imageId = String.Empty;
					if (theme.Icon != null) {
						ExportImageData(theme.Icon);
						imageId = ExportedImageTable[theme.Icon.RootImage];
					}
					ExportTheme(writer, theme, imageId);
				}
			}
			finally {
				WriteDxEndElement();
			}
		}
		void ExportActiveTheme() {
			if (DocumentModel.ActiveTheme == null)
				return;
			WriteDxStartElement("activeTheme");
			try {
				WriteDxStringAttr("val", DocumentModel.ActiveTheme.Name);
			}
			finally {
				WriteDxEndElement();
			}
		}
		protected internal ThemeCollection GetThemesForExport() {
			ThemeCollection result = new ThemeCollection();
			int count = DocumentModel.Themes.Count;
			for (int i = 0; i < count; i++) {
				Theme theme = DocumentModel.Themes[i];
				if (!theme.IsDefault || theme.IsModified)
					result.Add(theme);
			}
			return result;
		}
		void ExportTheme(XmlWriter writer, Theme theme, string imageId) {
			WriteDxStartElement("theme");
			if (!String.IsNullOrEmpty(imageId))
				WriteDxStringAttr("imageId", imageId);
			if (!String.IsNullOrEmpty(theme.NativeName) && theme.NativeName != theme.Name)
				WriteDxStringAttr("nativeName", theme.NativeName);
			try {
				WriteDxStartElement("name");
				try {
					WriteDxStringAttr("val", theme.Name);
				}
				finally {
					WriteDxEndElement();
				}
				ExportThemeStyles(writer, theme);
			}
			finally {
				WriteDxEndElement();
			}
		}
		void ExportThemeStyles(XmlWriter writer, Theme theme) {
			WriteDxStartElement("styles");
			using (SnapDocumentModel tempModel = new SnapDocumentModel(false, DocumentModel.DataSourceDispatcher.CreateNew(), DocumentModel.DocumentFormatsDependencies)) {
				theme.Apply(tempModel);
				SnapExporter exporter = new SnapExporter(tempModel, new SnapDocumentExporterOptions());
				exporter.DocumentContentWriter = writer;
				exporter.ExportTableStyles();
				exporter.ExportTableCellStyles();
			}
			WriteDxEndElement();
		}
		#endregion
		#region DataSources
		void AddDataSourcesContent() {
			List<IDataSourceExportInfo> dataSources = CollectDataSourcesForExport();
			if (dataSources.Count > 0) {
				this.dataSourcesRelationsTable.Add("RelDSrc1", "dataSources.xml");
				AddCompressedPackageContent(@"word\dataSources.xml", CreateCompressedXmlContent(writer => ExportDataSources(writer, dataSources)));
				RegisterContentTypeOverride("/word/dataSources.xml", "application/vnd.openxmlformats-officedocument.wordprocessingml.dataSources+xml");
			}
		}
		List<IDataSourceExportInfo> CollectDataSourcesForExport() {
			List<IDataSourceExportInfo> result = new List<IDataSourceExportInfo>();
			foreach (DataSourceInfo dataSource in DocumentModel.DataSources) {
				if (dataSource.DataSource == null)
					continue;
				IDataComponent dataComponent = dataSource.DataSource as IDataComponent;
				if (dataComponent != null) {
					result.Add(new SnapDataSourceExportInfo(dataComponent));
					continue;
				}
				BeforeDataSourceExportEventArgs args = new BeforeDataSourceExportEventArgs(dataSource.DataSource, dataSource.DataSourceName);
				DocumentModel.RaiseBeforeDataSourceExport(args);
				if (args.Data == null)
					continue;
				result.Add(new CustomDataSourceExportInfo(dataSource.DataSourceName, args.Data));
			}
			return result;
		}
		void ExportDataSources(XmlWriter writer, List<IDataSourceExportInfo> dataSources) {
			this.DocumentContentWriter = writer;
			WriteDxStartElement("dataSources");
			try {
				foreach (IDataSourceExportInfo info in dataSources)
					info.Write(this);
			}
			finally {
				WriteDxEndElement();
			}
		}
		#endregion
		#region DataSettings
		protected internal virtual CompressedStream ExportDataSettings() {
			return CreateCompressedXmlContent(GenerateDataSettingsContent);
		}
		protected internal virtual CompressedStream ExportFieldContexts() {
			return CreateCompressedXmlContent(GenerateFieldContextContent);
		}
		protected internal virtual void GenerateDataSettingsContent(XmlWriter writer) {
			this.DocumentContentWriter = writer;
			WriteDxStartElement("dataSettings");
			try {
				ExportCalculatedFields();
				ExportParameters();
			}
			finally {
				WriteDxEndElement();
			}
		}
		protected internal virtual void GenerateFieldContextContent(XmlWriter writer) {
			this.DocumentContentWriter = writer;
			WriteDxStartElement("fieldContexts");
			try {
				SnapContentWriter contentWriter = new SnapContentWriter(writer);
				fieldContextExporter.WriteContent(contentWriter);
			}
			finally {
				WriteDxEndElement();
			}
		}
		protected internal virtual void ExportCalculatedFields() {
			IList<ICalculatedField> fields = DocumentModel.DataSources.GetCalculatedFields();
			int count = fields.Count;
			if (count == 0)
				return;
			WriteDxStartElement("calcFields");
			for (int i = 0; i < count; i++) {
				ExportCalculatedField(fields[i]);
			}
			WriteDxEndElement();
		}
		void ExportCalculatedField(ICalculatedField calculatedField) {
			WriteDxStartElement("calculatedField");
			try {
				WriteDxStringAttr("name", calculatedField.Name);
				WriteDxStringAttr("fieldType", Enum.GetName(typeof(CalculatedFieldType), calculatedField.FieldType));
				WriteDxStringAttr("expression", calculatedField.Expression);
				WriteDxStringAttr("dataMember", calculatedField.DataMember);
				WriteDxStringAttr("dataSource", ((CalculatedField)calculatedField).DataSourceName);
			}
			finally {
				WriteDxEndElement();
			}
		}
		protected internal virtual void ExportParameters() {
			int count = DocumentModel.Parameters.Count;
			if (count == 0)
				return;
			WriteDxStartElement("params");
			for (int i = 0; i < count; i++) {
				ExportParameter(DocumentModel.Parameters[i]);
			}
			WriteDxEndElement();
		}
		void ExportParameter(Parameter parameter) {
			WriteDxStartElement("parameter");
			try {
				WriteDxStringAttr("name", parameter.Name);
				WriteDxStringAttr("type", parameter.Type.FullName);
				WriteDxStringAttr("value", TypeDescriptor.GetConverter(parameter.Type).ConvertToString(null, CultureInfo.InvariantCulture, parameter.Value));
			}
			finally {
				WriteDxEndElement();
			}
		}
		#endregion
		#region Snap Mail Merge
		protected internal virtual CompressedStream ExportSnapMailMergeOptions() {
			return CreateCompressedXmlContent(GenerateSnapMailMergeOptionsContent);
		}
		void GenerateSnapMailMergeOptionsContent(XmlWriter writer) {
			this.DocumentContentWriter = writer;
			WriteDxStartElement("snapMailMergeOptions");
			try {
				WriteSnapMailMergeOptionsCore();
			}
			finally {
				WriteDxEndElement();
			}
		}
		void WriteSnapMailMergeOptionsCore() {
			SnapMailMergeVisualOptions options = DocumentModel.SnapMailMergeVisualOptions;
			if (options.DataSourceName != null)
				WriteDxStringElement("dataSourceName", options.DataSourceName);
			if (!string.IsNullOrEmpty(options.DataMember))
				WriteDxStringElement("dataMember", options.DataMember);
			WriteDxIntElement("currentRecordIndex", options.CurrentRecordIndex);
			WriteDxStringElement("filterString", options.FilterString);
			WriteSnapMailMergeSorting(options.Sorting);
		}
		void WriteSnapMailMergeSorting(SnapListSorting sorting) {
			WriteDxStartElement("sorting");
			try {
				int count = sorting.Count;
				for (int i = 0; i < count; i++)
					WriteSortingItem(sorting[i]);
			}
			finally {
				WriteDxEndElement();
			}
		}
		void WriteSortingItem(SnapListGroupParam item) {
			WriteDxStartElement("sortingItem");
			WriteDxStringAttr("fieldName", item.FieldName);
			WriteDxStringAttr("sortOrder", Enum.GetName(typeof(ColumnSortOrder), item.SortOrder));
			WriteDxEndElement();
		}
		#endregion
		protected internal override void ExportDocumentVersion() {
			WriteDxStartElement("version");
			try {
				WriteDxIntAttr("val", CurrentVersion);
			}
			finally {
				WriteDxEndElement();
			}
		}
		void WriteDxStringElement(string startTag, string value) {
			WriteDxStartElement(startTag);
			WriteDxStringAttr("val", value);
			WriteDxEndElement();
		}
		void WriteDxIntElement(string startTag, int value) {
			WriteDxStartElement(startTag);
			WriteDxIntAttr("val", value);
			WriteDxEndElement();
		}
		void WriteDxBoolElement(string startTag, bool value) {
			WriteDxStartElement(startTag);
			WriteDxBoolAttr("val", value);
			WriteDxEndElement();
		}
	}
	#endregion
	interface IDataSourceExportInfo {
		void Write(SnapExporter exporter);
	}
	class CustomDataSourceExportInfo : IDataSourceExportInfo {
		public CustomDataSourceExportInfo(string name, byte[] data) {
			Name = name;
			Data = data;
		}
		public string Name { get; private set; }
		public byte[] Data { get; private set; }
		public void Write(SnapExporter exporter) {
			exporter.WriteDxStartElement("dataSource");
			try {
				exporter.WriteDxStringAttr("name", Name);
				exporter.WriteDxStringAttr("data", Convert.ToBase64String(Data));
			}
			finally {
				exporter.WriteDxEndElement();
			}
		}
	}
	class SnapDataSourceExportInfo : IDataSourceExportInfo {
		public SnapDataSourceExportInfo(IDataComponent component) {
			DataComponent = component;
		}
		public IDataComponent DataComponent { get; private set; }
		public void Write(SnapExporter exporter) {
			DataComponentHelper helper = new DataComponentHelper("dataComponent");
			XElement element = helper.SaveToXml(DataComponent);
			exporter.WriteXElement(element);
		}
	}
	#region SnapBookmarkStartBoundary
	public class SnapBookmarkStartBoundary : VisitableDocumentIntervalBoundary {
		public SnapBookmarkStartBoundary(SnapBookmark snapBookmark)
			: base(snapBookmark) {
		}
		public override DocumentModelPosition Position { get { return VisitableInterval.Interval.Start; } }
		public override BookmarkBoundaryOrder Order { get { return BookmarkBoundaryOrder.Start; } }
		public override void Export(DocumentModelExporter exporter) {
			((SnapExporter)exporter).ExportSnapBookmarkStart((SnapBookmark)VisitableInterval);
		}
		protected internal override VisitableDocumentIntervalBox CreateBox() {
			return new BookmarkStartBox();
		}
	}
	#endregion
	#region SnapBookmarkEndBoundary
	public class SnapBookmarkEndBoundary : VisitableDocumentIntervalBoundary {
		public SnapBookmarkEndBoundary(SnapBookmark snapBookmark)
			: base(snapBookmark) {
		}
		public override DocumentModelPosition Position { get { return VisitableInterval.Interval.End; } }
		public override BookmarkBoundaryOrder Order { get { return BookmarkBoundaryOrder.End; } }
		public override void Export(DocumentModelExporter exporter) {
			((SnapExporter)exporter).ExportSnapBookmarkEnd((SnapBookmark)VisitableInterval);
		}
		protected internal override VisitableDocumentIntervalBox CreateBox() {
			return new BookmarkEndBox();
		}
	}
	#endregion
	#region SnapTemplateIntervalStartBoundary
	public class SnapTemplateIntervalStartBoundary : VisitableDocumentIntervalBoundary {
		public SnapTemplateIntervalStartBoundary(SnapTemplateInterval templateInterval)
			: base(templateInterval) {
		}
		public override DocumentModelPosition Position { get { return VisitableInterval.Interval.Start; } }
		public override BookmarkBoundaryOrder Order { get { return BookmarkBoundaryOrder.Start; } }
		public override void Export(DocumentModelExporter exporter) {
			((SnapExporter)exporter).ExportSnapTemplateIntervalStart((SnapTemplateInterval)VisitableInterval);
		}
		protected internal override VisitableDocumentIntervalBox CreateBox() {
			return new BookmarkStartBox();
		}
	}
	#endregion
	#region SnapTemplateIntervalEndBoundary
	public class SnapTemplateIntervalEndBoundary : VisitableDocumentIntervalBoundary {
		public SnapTemplateIntervalEndBoundary(SnapTemplateInterval templateInterval)
			: base(templateInterval) {
		}
		public override DocumentModelPosition Position { get { return VisitableInterval.Interval.End; } }
		public override BookmarkBoundaryOrder Order { get { return BookmarkBoundaryOrder.End; } }
		public override void Export(DocumentModelExporter exporter) {
			((SnapExporter)exporter).ExportSnapTemplateIntervalEnd((SnapTemplateInterval)VisitableInterval);
		}
		protected internal override VisitableDocumentIntervalBox CreateBox() {
			return new BookmarkEndBox();
		}
	}
	#endregion
	#region ISnapBookmarkVisitor
	public interface ISnapBookmarkVisitor : IDocumentIntervalVisitor {
		void Visit(SnapBookmark snapBookmark);
		void Visit(SnapTemplateInterval templateInterval);
	}
	#endregion
	#region SnapBookmarkStartBoundaryFactory
	public class SnapBookmarkStartBoundaryFactory : VisitableDocumentIntervalStartBoundaryFactory, ISnapBookmarkVisitor {
		public void Visit(SnapBookmark snapBookmark) {
			SetBoundary(new SnapBookmarkStartBoundary(snapBookmark));
		}
		public void Visit(SnapTemplateInterval templateInterval) {
			SetBoundary(new SnapTemplateIntervalStartBoundary(templateInterval));
		}
	}
	#endregion
	#region SnapBookmarkEndBoundaryFactory
	public class SnapBookmarkEndBoundaryFactory : VisitableDocumentIntervalEndBoundaryFactory, ISnapBookmarkVisitor {
		public void Visit(SnapBookmark snapBookmark) {
			SetBoundary(new SnapBookmarkEndBoundary(snapBookmark));
		}
		public void Visit(SnapTemplateInterval templateInterval) {
			SetBoundary(new SnapTemplateIntervalEndBoundary(templateInterval));
		}
	}
	#endregion
	#region SnapBookmarkBasedObjectBoundaryIterator
	public class SnapVisitableDocumentIntervalBasedObjectBoundaryIterator : VisitableDocumentIntervalBasedObjectBoundaryIterator {
		public SnapVisitableDocumentIntervalBasedObjectBoundaryIterator(SnapPieceTable pieceTable)
			: base(pieceTable) {
		}
		protected internal override void PopulateBoundariesCore() {
			base.PopulateBoundariesCore();
			PopulateBoundariesCore(((SnapPieceTable)PieceTable).SnapBookmarks.InnerList);
			PopulateBoundariesCore(CollectTemplateIntervals());
		}
		protected internal override VisitableDocumentIntervalStartBoundaryFactory CreateVisitableDocumentIntervalStartBoundaryFactory() {
			return new SnapBookmarkStartBoundaryFactory();
		}
		protected internal override VisitableDocumentIntervalEndBoundaryFactory CreateVisitableDocumentIntervalEndBoundaryFactory() {
			return new SnapBookmarkEndBoundaryFactory();
		}
		List<SnapTemplateInterval> CollectTemplateIntervals() {
			Dictionary<SnapTemplateInterval, object> templateIntervals = new Dictionary<SnapTemplateInterval, object>();
			foreach (SnapBookmark bookmark in ((SnapPieceTable)PieceTable).SnapBookmarks) {
				if (!templateIntervals.ContainsKey(bookmark.TemplateInterval))
					templateIntervals.Add(bookmark.TemplateInterval, null);
			}
			return templateIntervals.Keys.ToList<SnapTemplateInterval>();
		}
	}
	#endregion
	public class SnapContentWriter {
		public const string SnapPrefix = "dx";
		public const string SnapNamespaceConstant = "http://devexpress.com/snap/2012/main";
		readonly XmlWriter documentContentWriter;
		public SnapContentWriter(XmlWriter documentContentWriter) {
			Guard.ArgumentNotNull(documentContentWriter, "documentContentWriter");
			this.documentContentWriter = documentContentWriter;
		}
		public string SnapNamespace { get { return SnapNamespaceConstant; } }
		public XmlWriter DocumentContentWriter { get { return documentContentWriter; } }
		public void WriteDxStartElement(string tag) {
			DocumentContentWriter.WriteStartElement(SnapPrefix, tag, SnapNamespace);
		}
		public void WriteDxStringAttr(string attr, string value) {
			DocumentContentWriter.WriteAttributeString(SnapPrefix, attr, SnapNamespace, value);
		}
		public void WriteDxBoolAttr(string attr, bool value) {
			WriteDxStringAttr(attr, ConvertBoolToString(value));
		}
		public void WriteDxIntAttr(string attr, int value) {
			WriteDxStringAttr(attr, value.ToString(NumberFormatInfo.InvariantInfo));
		}
		public void WriteDxEndElement() {
			DocumentContentWriter.WriteEndElement();
		}
		string ConvertBoolToString(bool value) {
			return value ? "1" : "0";
		}
	}
	public class SnapFieldsContextExporter {
		class ContextParentExporter : IFieldContextVisitor {
			readonly SnapFieldsContextExporter exporter;
			public ContextParentExporter(SnapFieldsContextExporter exporter) {
				Guard.ArgumentNotNull(exporter, "exporter");
				this.exporter = exporter;
			}
			public void Visit(EmptyFieldContext context) {
			}
			public void Visit(ProxyFieldContext context) {
				exporter.ExportFieldContext(context.Parent);
			}
			public void Visit(RootFieldContext context) {
			}
			public void Visit(SnapMailMergeRootFieldContext context) {
				Visit((RootFieldContext)context);
			}
			public void Visit(SingleListItemFieldContext context) {
				exporter.ExportFieldContext(context.Parent);
			}
			public void Visit(ListFieldContext context) {
				exporter.ExportFieldContext(context.Parent);
			}
			public void Visit(SimplePropertyFieldContext context) {
				exporter.ExportFieldContext(context.Parent);
			}
		}
		class ContextWriter : IFieldContextVisitor {
			readonly SnapFieldsContextExporter exporter;
			readonly SnapContentWriter contentWriter;
			public ContextWriter(SnapFieldsContextExporter exporter, SnapContentWriter contentWriter) {
				Guard.ArgumentNotNull(exporter, "exporter");
				Guard.ArgumentNotNull(contentWriter, "contentWriter");
				this.exporter = exporter;
				this.contentWriter = contentWriter;
			}
			public SnapContentWriter ContentWriter { get { return contentWriter; } }
			public void Write(IFieldContext context, int id) {
				ContentWriter.WriteDxStartElement("fieldContext");
				ContentWriter.WriteDxIntAttr("id", id);
				context.Accept(this);
				ContentWriter.WriteDxEndElement();
			}
			public void Visit(EmptyFieldContext context) {
				ContentWriter.WriteDxStringAttr("type", "empty");
			}
			public void Visit(ProxyFieldContext context) {
				ContentWriter.WriteDxStringAttr("type", "proxy");
				ContentWriter.WriteDxStringAttr("path", context.FieldPath);
				ContentWriter.WriteDxIntAttr("parentId", exporter.GetFieldContextId(context.Parent));
			}
			public void Visit(RootFieldContext context) {
				ContentWriter.WriteDxStringAttr("type", "root");
				ContentWriter.WriteDxStringAttr("source", exporter.DocumentModel.DataSourceDispatcher.FindDataSourceName(context.Source));
			}
			public void Visit(SnapMailMergeRootFieldContext context) {
				Visit((RootFieldContext)context);
			}
			public void Visit(SingleListItemFieldContext context) {
				ContentWriter.WriteDxStringAttr("type", "listItem");
				ContentWriter.WriteDxIntAttr("visibleIndex", context.VisibleIndex);
				ContentWriter.WriteDxIntAttr("rowHandle", context.RowHandle);
				ContentWriter.WriteDxIntAttr("indexInGroup", context.CurrentRecordIndexInGroup);
				ContentWriter.WriteDxIntAttr("parentId", exporter.GetFieldContextId(context.Parent));
			}
			public void Visit(ListFieldContext context) {
				ContentWriter.WriteDxStringAttr("type", "list");
				ContentWriter.WriteDxIntAttr("parentId", exporter.GetFieldContextId(context.Parent));
				WriteListParameters(context.ListParameters);
			}
			public void Visit(SimplePropertyFieldContext context) {
				ContentWriter.WriteDxStringAttr("type", "property");
				ContentWriter.WriteDxStringAttr("path", context.Path);
				ContentWriter.WriteDxIntAttr("parentId", exporter.GetFieldContextId(context.Parent));
			}
			void WriteListParameters(ListParameters listParameters) {
				if (listParameters == null)
					return;
				WriteFilters(listParameters.Filters);
				WriteGroups(listParameters.Groups);
			}
			void WriteFilters(FilterProperties filterProperties) {
				if (filterProperties == null)
					return;
				List<string> filters = filterProperties.Filters;
				if (filters == null || filters.Count == 0)
					return;
				ContentWriter.WriteDxStartElement("filters");
				foreach (string filter in filters) {
					ContentWriter.WriteDxStartElement("filter");
					ContentWriter.WriteDxStringAttr("filterstring", filter);
					ContentWriter.WriteDxEndElement();
				}
				ContentWriter.WriteDxEndElement();
			}
			void WriteGroups(List<GroupProperties> groups) {
				if (groups == null || groups.Count == 0)
					return;
				ContentWriter.WriteDxStartElement("groups");
				foreach (GroupProperties group in groups) {
					ContentWriter.WriteDxStartElement("group");
					if (group.HasTemplateFooter)
						ContentWriter.WriteDxStringAttr("tf", group.TemplateFooterSwitch);
					if (group.HasTemplateHeader)
						ContentWriter.WriteDxStringAttr("th", group.TemplateHeaderSwitch);
					foreach (GroupFieldInfo groupFieldInfo in group.GroupFieldInfos) {
						ContentWriter.WriteDxStartElement("groupField");
						ContentWriter.WriteDxStringAttr("name", groupFieldInfo.FieldName);
						ContentWriter.WriteDxStringAttr("sortOrder", groupFieldInfo.SortOrder.ToString());
						ContentWriter.WriteDxStringAttr("groupInterval", groupFieldInfo.GroupInterval.ToString());
						ContentWriter.WriteDxEndElement();
					}
					ContentWriter.WriteDxEndElement();
				}
				ContentWriter.WriteDxEndElement();
			}
		}
		readonly Dictionary<IFieldContext, int> fieldContextIds;
		readonly SnapDocumentModel documentModel;
		int fieldContextCount = 0;
		public SnapFieldsContextExporter(SnapDocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.fieldContextIds = new Dictionary<IFieldContext, int>();
			this.documentModel = documentModel;
			fieldContextCount = 0;
		}
		SnapDocumentModel DocumentModel { get { return documentModel; } }
		public int ExportFieldContext(IFieldContext fieldContext) {
			ExportParentContext(fieldContext);
			int result;
			if (!fieldContextIds.TryGetValue(fieldContext, out result)) {
				fieldContextCount++;
				result = fieldContextCount;
				fieldContextIds.Add(fieldContext, result);
			}
			return result;
		}
		public int GetFieldContextId(IFieldContext fieldContext) {
			return fieldContextIds[fieldContext];
		}
		void ExportParentContext(IFieldContext fieldContext) {
			ContextParentExporter parentExporter = new ContextParentExporter(this);
			fieldContext.Accept(parentExporter);
		}
		public void WriteContent(SnapContentWriter contentWriter) {
			ContextWriter writer = new ContextWriter(this, contentWriter);
			Guard.ArgumentNotNull(contentWriter, "contentWriter");
			foreach (KeyValuePair<IFieldContext, int> fieldContextIdPair in fieldContextIds) {
				writer.Write(fieldContextIdPair.Key, fieldContextIdPair.Value);
			}
		}
	}
}
