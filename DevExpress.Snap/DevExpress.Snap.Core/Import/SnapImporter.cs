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
using System.Globalization;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Export;
using DevExpress.Snap.Localization;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core.Native.Data.Implementations;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Import.OpenXml;
using DevExpress.XtraRichEdit.Model;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Data;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.Xpo.DB;
using DevExpress.XtraPrinting.Native;
using ThemeCollection = DevExpress.Snap.Core.Native.ThemeCollection;
using DevExpress.Snap.Native.Services;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.Snap.Core.Import {
	public class SnapImporter : OpenXmlImporter {
		#region Fields
		const string xmlDataConnections = "DataConnections";
		const string xmlDataConnection = "DataConnection";
		const string xmlDataSources = "DataSources";
		const string xmlDataSource = "DataSource";
		const string xmlName = "Name";
		protected delegate IFieldContext CreateFieldContextAction(Dictionary<int, FieldContextImportInfo> importedInfos, FieldContextImportInfo info);
		Dictionary<string, CreateFieldContextAction> fieldContextHandlers;
		int documentVersion;
		Dictionary<string, SnapBookmark> createdBookmarks = new Dictionary<string, SnapBookmark>();
		Dictionary<int, IFieldContext> fieldContexts = new Dictionary<int, IFieldContext>();
		FieldPathService fieldPathService;
		List<CalculatedField> calculatedFields;
		#endregion
		public SnapImporter(SnapDocumentModel documentModel, SnapDocumentImporterOptions options)
			: base(documentModel, options) {
			this.fieldContexts = new Dictionary<int, IFieldContext>();
			this.fieldContextHandlers = CreateFieldContextHandlers();
			this.calculatedFields = new List<CalculatedField>();
		}
		#region Properties
		public new SnapDocumentModel DocumentModel { get { return (SnapDocumentModel)base.DocumentModel; } }
		FieldPathService FieldPathService {
			get {
				if (fieldPathService == null)
					fieldPathService = DocumentModel.GetService<IFieldDataAccessService>().FieldPathService as FieldPathService;
				return fieldPathService;
			}
		}
		public string SnapNamespace { get { return SnapContentWriter.SnapNamespaceConstant; } }
		public int CurrentVersion { get { return SnapExporter.CurrentVersion; } }
		public Dictionary<string, ImportSnapBookmarkInfo> SnapBookmarks { get { return ((ImportSnapPieceTableInfo)PieceTableInfo).SnapBookmarks; } }
		public Dictionary<string, ImportSnapTemplateIntervalInfo> SnapTemplateIntervals { get { return ((ImportSnapPieceTableInfo)PieceTableInfo).SnapTemplateIntervals; } }
		protected Dictionary<string, SnapBookmark> CreatedBookmarks { get { return createdBookmarks; } }
		protected internal int DocumentVersion { get { return documentVersion; } set { documentVersion = value; } }
		public List<CalculatedField> CalculatedFields { get { return calculatedFields; } }
		#endregion
		Dictionary<string, CreateFieldContextAction> CreateFieldContextHandlers() {
			Dictionary<string, CreateFieldContextAction> result = new Dictionary<string, CreateFieldContextAction>();
			result.Add("empty", CreateEmptyFieldContext);
			result.Add("root", CreateRootFieldContext);
			result.Add("proxy", CreateProxyFieldContext);
			result.Add("listItem", CreateSingleListItemFieldContext);
			result.Add("list", CreateListFieldContext);
			result.Add("property", CreateSimplePropertyFieldContext);
			return result;
		}
		protected override void BeforeImportMainDocument() {
			base.BeforeImportMainDocument();
			ImportDataSettings();
			ImportDataSources();
			ImportFieldContexts();
			ImportSnapThemes();
			ImportSnapMailMergeOptions();
		}
		protected internal override void PushCurrentPieceTable(PieceTable pieceTable) {
			PieceTableInfos.Push(new ImportSnapPieceTableInfo((SnapPieceTable)pieceTable));
		}
		protected internal override PieceTable PopCurrentPieceTable() {
			return PieceTableInfos.Pop().PieceTable;
		}
		protected internal override Destination CreateBookmarkStartElementDestination(XmlReader reader) {
			string type = reader.GetAttribute("type", SnapNamespace);
			if (type != null)
				type = type.Trim();
			switch (type) {
				case "snapBookmark": return new SnapBookmarkStartElementDestination(this);
				case "snapTemplateInterval": return new SnapTemplateIntervalStartElementDestination(this);
				default:
					return new BookmarkStartElementDestination(this);
			}
		}
		protected internal override Destination CreateBookmarkEndElementDestination(XmlReader reader) {
			string type = reader.GetAttribute("type", SnapNamespace);
			if (type != null)
				type = type.Trim();
			switch (type) {
				case "snapBookmark": return new SnapBookmarkEndElementDestination(this);
				case "snapTemplateInterval": return new SnapTemplateIntervalEndElementDestination(this);
				default:
					return new BookmarkEndElementDestination(this);
			}
		}
		protected internal override Destination CreateCustomRunDestination(XmlReader reader) {
			string type = reader.GetAttribute("type", SnapNamespace);
			if (type != null)
				type = type.Trim();
			switch (type) {
				case "barCode": return new BarCodeDestination(this);
				case "checkBox": return new CheckBoxDestination(this);
				default: return new CustomRunDestination(this);
			}
		}
		protected internal override Destination CreateDataContainerRunDestination(XmlReader reader) {
			string type = reader.GetAttribute("type", SnapNamespace);
			if (type != null)
				type = type.Trim();
			switch (type) {
				case "base64StringDataContainer": return new Base64StringDataContainerDestination(this);
				default: return new DataContainerRunDestination(this);
			}
		}
		protected internal override Destination CreateVersionDestination(XmlReader reader) {
			return new DocumentVersionDestination(this);
		}
		protected internal override RunDestination CreateRunDestination() {
			return new SNRunDestination(this);
		}
		protected internal override ParagraphDestination CreateParagraphDestination() {
			return new SNParagraphDestination(this);
		}
		protected internal override void InsertBookmarks() {
			base.InsertBookmarks();
			InsertSnapBookmarks();
		}
		protected internal virtual void InsertSnapBookmarks() {
			SnapPieceTable pieceTable = (SnapPieceTable)PieceTable;
			foreach (string id in SnapBookmarks.Keys) {
				ImportSnapBookmarkInfo bookmarkInfo = SnapBookmarks[id];
				DocumentLogInterval interval = GetTemplateIntervalById(bookmarkInfo.TemplateIntervalId);
				if (bookmarkInfo.Validate(pieceTable) && interval != null) {
					IFieldContext dataContext = GetFieldContext(bookmarkInfo.FieldContextId);
					SnapBookmark bookmark = pieceTable.CreateSnapBookmarkCore(bookmarkInfo.Start, bookmarkInfo.End - bookmarkInfo.Start, dataContext, interval, pieceTable, new SnapTemplateInfo(SnapTemplateIntervalType.DataRow));
					if (!String.IsNullOrEmpty(bookmarkInfo.ParentId))
						bookmark.Parent = CreatedBookmarks[bookmarkInfo.ParentId];
					if (!String.IsNullOrEmpty(bookmarkInfo.HeaderBookmarkId))
						bookmark.HeaderBookmark = CreatedBookmarks[bookmarkInfo.HeaderBookmarkId];
					if (!String.IsNullOrEmpty(bookmarkInfo.FooterBookmarkId))
						bookmark.FooterBookmark = CreatedBookmarks[bookmarkInfo.FooterBookmarkId];
					CreatedBookmarks.Add(bookmarkInfo.Id, bookmark);
				}
			}
			UpdateTemplateIntervals();
			CreatedBookmarks.Clear();
		}
		void UpdateTemplateIntervals() {
			foreach (string id in CreatedBookmarks.Keys) {
				ImportSnapBookmarkInfo bookmarkInfo = SnapBookmarks[id];
				ImportSnapTemplateInfo importTemplateInfo = bookmarkInfo.TemplateInfo;
				SnapBookmark bookmark = CreatedBookmarks[id];
				SnapTemplateInfo templateInfo = bookmark.TemplateInterval.TemplateInfo;
				templateInfo.FieldInGroupCount = importTemplateInfo.FieldInGroupCount;
				templateInfo.FirstGroupIndex = importTemplateInfo.FirstGroupIndex;
				templateInfo.TemplateType = importTemplateInfo.TemplateType;
				templateInfo.FirstGroupBookmark = GetBookmarkByIdSafe(importTemplateInfo.FirstGroupBookmarkId, bookmark);
				templateInfo.LastGroupBookmark = GetBookmarkByIdSafe(importTemplateInfo.LastGroupBookmarkId, bookmark);
				templateInfo.FirstListBookmark = GetBookmarkByIdSafe(importTemplateInfo.FirstListBookmarkId, bookmark);
				templateInfo.LastListBookmark = GetBookmarkByIdSafe(importTemplateInfo.LastListBookmarkId, bookmark);
#if DEBUGTEST || DEBUG
				Debug.Assert(importTemplateInfo.LastGroupIndex == importTemplateInfo.FirstGroupIndex + importTemplateInfo.FieldInGroupCount - 1);
				if (templateInfo.FirstGroupBookmark != null && templateInfo.FirstGroupBookmark.Start > templateInfo.LastGroupBookmark.Start)
					Exceptions.ThrowInternalException();
#endif
			}
		}
		SnapBookmark GetBookmarkByIdSafe(string key, SnapBookmark defaultValue) {
			if (String.IsNullOrEmpty(key))
				return null;
			SnapBookmark result;
			if (CreatedBookmarks.TryGetValue(key, out result))
				return result;
			return defaultValue;
		}
		DocumentLogInterval GetTemplateIntervalById(string id) {
			DocumentLogInterval interval;
			ImportSnapTemplateIntervalInfo intervalInfo;
			if (SnapTemplateIntervals.TryGetValue(id, out intervalInfo)) {
				interval = new DocumentLogInterval(intervalInfo.Start, intervalInfo.End - intervalInfo.Start);
				return interval;
			}
			return null;
		}
		#region Themes
		void ImportSnapThemes() {
			string fileName = LookupRelationTargetByType(DocumentRelations, SnapExporter.RelsTheme, DocumentRootFolder, "themes.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader != null)
				ImportThemesCore(reader);
		}
		void ImportThemesCore(XmlReader reader) {
			if (!ReadToDxRootElement(reader, "themes"))
				return;
			DocumentRelationsStack.Push(ImportRelations(DocumentRootFolder + "/_rels/" + "themes.xml.rels"));
			ThemeCollection themes = ((SnapDocumentModel)PieceTable.DocumentModel).Themes;
			themes.BeginUpdate();
			try {
				ImportContent(reader, new ThemesDestination(this));
			}
			finally {
				themes.EndUpdate();
				DocumentRelationsStack.Pop();
			}
		}
		#endregion
		#region DataSettings
		protected internal virtual void ImportDataSettings() {
			string fileName = LookupRelationTargetByType(DocumentRelations, SnapExporter.RelsDataSettings, DocumentRootFolder, "dataSettings.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader != null)
				ImportDataSettingsCore(reader);
		}
		protected internal virtual void ImportDataSettingsCore(XmlReader reader) {
			if (!ReadToDxRootElement(reader, "dataSettings"))
				return;
			DocumentModel.BeginUpdateDataSource();
			ImportContent(reader, new DataSettingsDestination(this));
			DocumentModel.EndUpdateDataSource();
		}
		#endregion
		#region Mail Merge
		protected internal virtual void ImportSnapMailMergeOptions() {
			string fileName = LookupRelationTargetByType(DocumentRelations, SnapExporter.RelsSnapMailMergeOptions, DocumentRootFolder, "snapMailMergeOptions.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader != null)
				ImportSnapMailMergeOptionsCore(reader);
		}
		protected internal virtual void ImportSnapMailMergeOptionsCore(XmlReader reader) {
			if (!ReadToDxRootElement(reader, "snapMailMergeOptions"))
				return;
			ImportContent(reader, new SnapMailMergeOptionsDestination(this));
		}
		#endregion
		public override void EndSetMainDocumentContent() {
			base.EndSetMainDocumentContent();
			AddCalculatedFields();
			ImportDataContainer();
		}
		void AddCalculatedFields() {
			for (int i = 0; i < calculatedFields.Count; i++) {
				CalculatedField calcField = calculatedFields[i];
				DataSourceInfo info = DocumentModel.GetNotNullDocumentModelDataSourceInfo(calcField.DataSourceName);
				info.CalculatedFields.Add(calcField);
			}
		}
		void ImportDataContainer() {
			string fileName = LookupRelationTargetByType(DocumentRelations, SnapExporter.RelsDataContainer, DocumentRootFolder, "dataContainer.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader != null)
				ImportDataContainerCore(reader);
		}
		void ImportDataContainerCore(XmlReader reader) {
			DocumentModel.BeginUpdateDataSource();
			try {
				var dataSouces = LoadDataSourcesFromContainer(reader);
				if(dataSouces != null)
					dataSouces.ForEach(d => {
						d.Fill();
						DocumentModel.AddDataSource(d); 
						 });
			}
			finally {
				DocumentModel.EndUpdateDataSource();
			}
		}
		IEnumerable<SqlDataSource> LoadDataSourcesFromContainer(XmlReader reader) {
			XDocument document = XDocument.Load(reader);
			XElement dataSourcesElement = document.Root.Element(xmlDataSources);
			if (dataSourcesElement == null)
				return null;
			Dictionary<string, SqlDataConnection> connections = new Dictionary<string, SqlDataConnection>();
			XElement dataConnectionsElement = document.Root.Element(xmlDataConnections);
			if (dataConnectionsElement != null)
				foreach (XElement connectionElement in dataConnectionsElement.Elements(xmlDataConnection)) {
					SqlDataConnection connection = new SqlDataConnection();
					connection.LoadFromXml(connectionElement);
					connections.Add(connection.Name, connection);
				}
			return LoadDataSourcesFromContainerCore(connections, dataSourcesElement);
		}
		IEnumerable<SqlDataSource> LoadDataSourcesFromContainerCore(Dictionary<string, SqlDataConnection> connections, XElement dataSourcesElement) {
			List<SqlDataSource> result = new List<SqlDataSource>();
			foreach (XElement dataSourceElement in dataSourcesElement.Elements(xmlDataSource)) {
				XElement dataProviderElement = dataSourceElement.Elements().FirstOrDefault();
				if (dataProviderElement == null)
					continue;
				string dataConnectionName = dataProviderElement.GetAttributeValue(xmlDataConnection);
				SqlDataConnection connection;
				if (!connections.TryGetValue(dataConnectionName, out connection))
					continue;
				connection.CreateDataStore(this.DocumentModel.GetService<IDataConnectionParametersService>());
#pragma warning disable 612, 618
				DataProviderBase dataProvider = new DataProviderBase(connection);
				((IDataProvider)dataProvider).LoadFromXml(dataProviderElement);
				var dbSchemaProvider = DocumentModel.GetService<IDBSchemaProvider>();
				DBSchema schema = dbSchemaProvider.GetSchema(connection);
				DBTable[] dbObjects = schema.Tables.Union(schema.Views).ToArray();
				dbSchemaProvider.LoadColumns(connection, dbObjects);
				dataProvider.DBSchema = schema;
#pragma warning restore 612, 618
				SqlDataSource sqlDataSource = new SqlDataSource("connection",
					connection.CreateDataConnectionParameters());
				CompositeDataConnectionParametersService srv = new CompositeDataConnectionParametersService();
				srv.AddService(sqlDataSource);
				srv.AddService(DocumentModel.GetService<IDataConnectionParametersService>());
				sqlDataSource.RemoveService<IDataConnectionParametersService>();
				sqlDataSource.AddService<IDataConnectionParametersService>(srv);
				sqlDataSource.Name = dataSourceElement.GetAttributeValue(xmlName);
				if (dataProvider.DataSelection.All(t => t.References.ActionType != ActionType.MasterDetailRelation))
					sqlDataSource.Queries.Add(dataProvider.GetQuery());
				else {
					var groups = dataProvider.DataSelection.GetTableGroups();
					groups.ForEach(g => sqlDataSource.Queries.Add(dataProvider.GetQuery(g.Value, g.Key)));
					var tables = dataProvider.DataSelection.Where(t => t.References.ActionType == ActionType.MasterDetailRelation);
					tables.ForEach(table => {
						table.References.ForEach(
							r => {
								var masterDetailInfo = sqlDataSource.Relations.FirstOrDefault(m => m.MasterQueryName == r.ParentDataTable.TableName);
								if (masterDetailInfo == null)
									sqlDataSource.Relations.Add(r.ParentDataTable.TableName, r.KeyDBTableName, r.ParentKeyColumn, r.KeyColumn);
								else
									masterDetailInfo.KeyColumns.Add(new RelationColumnInfo(r.ParentKeyColumn, r.KeyColumn));
							});
					});
				}
				result.Add(sqlDataSource);
			}
			return result;
		}
		void ImportDataSources() {
			string fileName = LookupRelationTargetByType(DocumentRelations, SnapExporter.RelsDataSources, DocumentRootFolder, "dataSources.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader != null)
				ImportDataSourcesCore(reader);
		}
		void ImportDataSourcesCore(XmlReader reader) {
			if (ReadToDxRootElement(reader, "dataSources"))
				ImportContent(reader, new DataSourcesDestination(this));
		}
		#region FieldContexts
		protected internal virtual void ImportFieldContexts() {
			string fileName = LookupRelationTargetByType(DocumentRelations, SnapExporter.RelsFieldContext, DocumentRootFolder, "fieldContexts.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader != null)
				ImportFieldContextsCore(reader);
		}
		protected internal virtual void ImportFieldContextsCore(XmlReader reader) {
			if (!ReadToDxRootElement(reader, "fieldContexts"))
				return;
			FieldContextsDestionation destination = new FieldContextsDestionation(this);
			ImportContent(reader, destination);
			CreateFieldContexts(destination.ImportedInfos);
		}
		void CreateFieldContexts(Dictionary<int, FieldContextImportInfo> importedInfos) {
			foreach (KeyValuePair<int, FieldContextImportInfo> importInfo in importedInfos)
				CreateFieldContext(importedInfos, importInfo.Value);
		}
		IFieldContext CreateFieldContext(Dictionary<int, FieldContextImportInfo> importedInfos, FieldContextImportInfo importInfo) {
			IFieldContext result = GetFieldContext(importInfo.Id);
			if (result != null)
				return result;
			CreateFieldContextAction action;
			if (!fieldContextHandlers.TryGetValue(importInfo.Type, out action))
				return null;
			result = action(importedInfos, importInfo);
			fieldContexts.Add(importInfo.Id, result);
			return result;
		}
		IFieldContext CreateRootFieldContext(Dictionary<int, FieldContextImportInfo> importedInfos, FieldContextImportInfo info) {
			return new RootFieldContext(DocumentModel.DataSourceDispatcher, info.Source);
		}
		IFieldContext CreateProxyFieldContext(Dictionary<int, FieldContextImportInfo> importedInfos, FieldContextImportInfo info) {
			IDataControllerListFieldContext parent = (IDataControllerListFieldContext)CreateFieldContext(importedInfos, importedInfos[info.ParentId]);
			return new ProxyFieldContext(info.Path, parent);
		}
		IFieldContext CreateEmptyFieldContext(Dictionary<int, FieldContextImportInfo> importedInfos, FieldContextImportInfo info) {
			return new EmptyFieldContext();
		}
		IFieldContext CreateSimplePropertyFieldContext(Dictionary<int, FieldContextImportInfo> importedInfos, FieldContextImportInfo info) {
			ISingleObjectFieldContext parent = (ISingleObjectFieldContext)CreateFieldContext(importedInfos, importedInfos[info.ParentId]);
			return new SimplePropertyFieldContext(parent, info.Path);
		}
		IFieldContext CreateSingleListItemFieldContext(Dictionary<int, FieldContextImportInfo> importedInfos, FieldContextImportInfo info) {
			IDataControllerListFieldContext parent = (IDataControllerListFieldContext)CreateFieldContext(importedInfos, importedInfos[info.ParentId]);
			return new SingleListItemFieldContext(parent, info.VisibleIndex, info.RowHandle, info.CurrentIndexInGroup);
		}
		IFieldContext CreateListFieldContext(Dictionary<int, FieldContextImportInfo> importedInfos, FieldContextImportInfo info) {
			ISingleObjectFieldContext parent = (ISingleObjectFieldContext)CreateFieldContext(importedInfos, importedInfos[info.ParentId]);
			FilterProperties filterProperties = null;
			if (info.Filters != null) {
				filterProperties = new FilterProperties();
				filterProperties.Filters.AddRange(info.Filters);
			}
			ListParameters listParameters = new ListParameters(info.Groups, filterProperties);
			return new ListFieldContext(parent, listParameters);
		}
		IFieldContext GetFieldContext(int id) {
			IFieldContext result;
			if (fieldContexts.TryGetValue(id, out result))
				return result;
			else
				return null;
		}
		IFieldContext CreateFieldContext(FieldContextImportInfo fieldContextImportInfo) {
			throw new NotImplementedException();
		}
		#endregion
		protected internal virtual bool ReadToDxRootElement(XmlReader reader, string name) {
			return ReadToRootElement(reader, name, SnapNamespace);
		}
		protected internal string ReadDxStringAttr(string name, XmlReader reader) {
			string result = reader.GetAttribute(name, SnapNamespace);
			if (result != null)
				result = result.Trim();
			return result;
		}
		protected internal bool ReadDxBoolAttr(string name, XmlReader reader) {
			string val = ReadDxStringAttr(name, reader);
			int result;
			if (Int32.TryParse(val, NumberStyles.None, CultureInfo.InvariantCulture, out result))
				return result != 0;
			else {
				bool bResult;
				if (bool.TryParse(val, out bResult))
					return bResult;
			}
			return false;
		}
		protected internal int ReadDxIntAttr(string name, XmlReader reader) {
			return Convert.ToInt32(ReadDxStringAttr(name, reader), NumberFormatInfo.InvariantInfo);
		}
		protected internal override void CheckVersion() {
			if (DocumentVersion == 0 || CurrentVersion < DocumentVersion)
				SnapExceptions.ThrowInvalidOperationException(SnapStringId.Msg_UnsupportedDocumentVersion);
		}
	}
	public abstract class SnapLeafElementDestination : LeafElementDestination {
		protected SnapLeafElementDestination(SnapImporter importer)
			: base(importer) {
		}
		protected internal new SnapImporter Importer { get { return (SnapImporter)base.Importer; } }
	}
}
