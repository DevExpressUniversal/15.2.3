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
using System.Reflection;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using DevExpress.Utils;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Sql;
using DevExpress.Office.Services.Implementation;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Localization;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.API.Native.Implementation;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Options;
using ApiField = DevExpress.XtraRichEdit.API.Native.Field;
using DocumentModel = DevExpress.Snap.Core.Native.SnapDocumentModel;
using InternalSnapDocumentServer = DevExpress.Snap.Core.Native.InternalSnapDocumentServer;
using ModelField = DevExpress.XtraRichEdit.Model.Field;
using ModelSection = DevExpress.XtraRichEdit.Model.Section;
using ModelPieceTable = DevExpress.Snap.Core.Native.SnapPieceTable;
using ModelTableCellStyle = DevExpress.XtraRichEdit.Model.TableCellStyle;
using ModelLogPosition = DevExpress.XtraRichEdit.Model.DocumentLogPosition;
using ModelPosition = DevExpress.XtraRichEdit.Model.DocumentModelPosition;
using SnapFieldCalculatorService = DevExpress.Snap.Core.Native.SnapFieldCalculatorService;
using SnapPieceTable = DevExpress.Snap.Core.Native.SnapPieceTable;
using ApiTheme = DevExpress.Snap.Core.API.Theme;
using System.Diagnostics;
namespace DevExpress.Snap.API.Native {
	public class SnapNativeDocument : NativeDocument, SnapDocument {
		#region Fields
		SnapNativeFieldOwner fieldOwner;
		NativeTableCellStyleCollection tableCellStyles;
		NativeThemeCollection themes;
		#endregion
		protected internal SnapNativeDocument(ModelPieceTable pieceTable, InnerRichEditDocumentServer server)
			: base(pieceTable, server) {
			this.fieldOwner = new SnapNativeFieldOwner(this, this);
			this.tableCellStyles = new NativeTableCellStyleCollection(this);
			SubscribeEvents(server);
		}
		#region Properties
		protected internal new ModelPieceTable PieceTable { get { return (ModelPieceTable)base.PieceTable; } }
		protected internal new DocumentModel DocumentModel { get { return (DocumentModel)base.DocumentModel; } }
		public new SnapDocumentRange Range { get { return (SnapDocumentRange)base.Range; } }
		public string ActiveThemeName { get { return DocumentModel.ActiveTheme != null ? DocumentModel.ActiveTheme.Name : null; } }
		public DevExpress.XtraRichEdit.API.Native.TableCellStyleCollection TableCellStyles { get { return tableCellStyles; } }
		#endregion
		void SubscribeEvents(InnerRichEditDocumentServer server) {
			DocumentModel.BeforeInsertSnList += (sender, e) => {
				if (BeforeInsertSnList == null)
					return;
				SnapDocumentPosition apiPos = (SnapDocumentPosition)ActiveSubDocument.CreatePosition(((IConvertToInt<DocumentLogPosition>)e.Position).ToInt());
				List<DataFieldInfo> apiFields = new List<DataFieldInfo>(e.DataFields.Length);
				foreach (Core.Native.Data.SNDataInfo field in e.DataFields)
					apiFields.Add(new DataFieldInfo(field));
				var apiE = new Core.API.BeforeInsertSnListEventArgs(apiPos, apiFields);
				BeforeInsertSnList(this, apiE);
				DocumentLogPosition pos = apiE.Position.LogPosition;
				DocumentLogPosition endLogPosition = ActiveSubDocument.EndPosition.LogPosition;
				if (pos < DocumentLogPosition.Zero)
					pos = DocumentLogPosition.Zero;
				else if (pos >= endLogPosition)
					pos = endLogPosition - 1;
				e.Position = pos;
				e.DataFields = (apiE.DataFields == null) ? null : apiE.DataFields.ConvertAll(item => item.ToSNDataInfo()).ToArray();
			};
			DocumentModel.PrepareSnList += (sender, e) => {
				if (PrepareSnList == null)
					return;
				SnapPieceTable templatePieceTable = e.Template.MainPieceTable as SnapPieceTable;
				if (templatePieceTable != null)
					PrepareSnList(this, new Core.API.PrepareSnListEventArgs(templatePieceTable, server));
			};
			DocumentModel.AfterInsertSnList += (sender, e) => {
				if (AfterInsertSnList != null) {
					IThreadSyncService service = DocumentModel.GetService<IThreadSyncService>();
					if (service == null)
						return;
					service.EnqueueInvokeInUIThread(() => { AfterInsertSnList(this, new Core.API.AfterInsertSnListEventArgs((SnapDocumentRange)ActiveSubDocument.CreateRange(e.Start, e.Length))); });
				}
			};
			DocumentModel.BeforeInsertSnListColumns += (sender, e) => {
				if (BeforeInsertSnListColumns == null)
					return;
				ApiField apiTargetField = new NativeField(ActiveSubDocument, e.TargetField);
				List<DataFieldInfo> apiFields = new List<DataFieldInfo>(e.DataFields.Length);
				foreach (Core.Native.Data.SNDataInfo field in e.DataFields)
					apiFields.Add(new DataFieldInfo(field));
				var apiE = new Core.API.BeforeInsertSnListColumnsEventArgs((SnapSubDocument)ActiveSubDocument, apiTargetField, e.TargetColumnIndex, apiFields);
				BeforeInsertSnListColumns(this, apiE);
				e.DataFields = apiE.DataFields == null ? null : apiE.DataFields.ConvertAll(item => item.ToSNDataInfo()).ToArray();
				e.TargetColumnIndex = apiE.TargetColumnIndex;
			};
			DocumentModel.PrepareSnListColumns += (sender, e) => {
				if (PrepareSnListColumns != null)
					PrepareSnListColumns(this, new Core.API.PrepareSnListColumnsEventArgs(e, server));
			};
			DocumentModel.AfterInsertSnListColumns += (sender, e) => {
				if (AfterInsertSnListColumns != null)
					AfterInsertSnListColumns(this, new Core.API.AfterInsertSnListColumnsEventArgs((SnapSubDocument)ActiveSubDocument, new NativeField(ActiveSubDocument, e.SnList)));
			};
			DocumentModel.BeforeInsertSnListDetail += (sender, e) => {
				if (Object.ReferenceEquals(BeforeInsertSnListDetail, null))
					return;
				List<DataFieldInfo> apiFields = new List<DataFieldInfo>(e.DataFields.Length);
				foreach (Core.Native.Data.SNDataInfo field in e.DataFields)
					apiFields.Add(new DataFieldInfo(field));
				var apiE = new Core.API.BeforeInsertSnListDetailEventArgs((SnapSubDocument)ActiveSubDocument, new NativeField(ActiveSubDocument, e.Master), apiFields);
				BeforeInsertSnListDetail(this, apiE);
				e.DataFields = (apiE.DataFields == null) ? null : apiE.DataFields.ConvertAll(item => item.ToSNDataInfo()).ToArray();
			};
			DocumentModel.PrepareSnListDetail += (sender, e) => {
				if (Object.ReferenceEquals(PrepareSnListDetail, null))
					return;
				SnapPieceTable templatePieceTable = e.Template.MainPieceTable as SnapPieceTable;
				PrepareSnListDetail(this, new Core.API.PrepareSnListDetailEventArgs(templatePieceTable, server));
			};
			DocumentModel.AfterInsertSnListDetail += (sender, e) => {
				if (Object.ReferenceEquals(AfterInsertSnListDetail, null))
					return;
				AfterInsertSnListDetail(this, new Core.API.AfterInsertSnListDetailEventArgs((SnapSubDocument)ActiveSubDocument, new NativeField(ActiveSubDocument, e.Master)));
			};
			DocumentModel.BeforeInsertSnListRecordData += (sender, e) => {
				if (object.ReferenceEquals(BeforeInsertSnListRecordData, null))
					return;
				List<DataFieldInfo> apiFields = new List<DataFieldInfo>(e.DataFields.Length);
				foreach (Core.Native.Data.SNDataInfo field in e.DataFields)
					apiFields.Add(new DataFieldInfo(field));
				var apiE = new Core.API.BeforeInsertSnListRecordDataEventArgs((SnapSubDocument)ActiveSubDocument, new NativeField(ActiveSubDocument, e.TargetField), e.TargetColumnIndex, apiFields);
				BeforeInsertSnListRecordData(this, apiE);
				e.DataFields = (apiE.DataFields == null) ? null : apiE.DataFields.ConvertAll(item => item.ToSNDataInfo()).ToArray();
			};
			DocumentModel.AfterInsertSnListRecordData += (sender, e) => {
				if (object.ReferenceEquals(AfterInsertSnListRecordData, null))
					return;
				AfterInsertSnListRecordData(this, new Core.API.AfterInsertSnListRecordDataEventArgs((SnapDocumentRange)ActiveSubDocument.CreateRange(e.Start, e.Length)));
			};
		}
		#region SnapDocument Members
		public new SnapSectionCollection Sections { get { return (SnapSectionCollection)base.Sections; } }
		public event ConnectionErrorEventHandler ConnectionError {
			add { this.DocumentModel.ConnectionError += value; }
			remove { this.DocumentModel.ConnectionError -= value; }
		}
		public event ConfigureDataConnectionEventHandler ConfigureDataConnection {
			add { this.DocumentModel.ConfigureDataConnection += value; }
			remove { this.DocumentModel.ConfigureDataConnection -= value; }
		}
		public event CustomFilterExpressionEventHandler CustomFilterExpression {
			add { this.DocumentModel.CustomFilterExpression += value; }
			remove { this.DocumentModel.CustomFilterExpression -= value; }
		}
		public IDataSourceOwner GetDataSourceOwner(object dataSource) {
			return DocumentModel.DataSources.GetInfo(dataSource);
		}
		public DataSourceInfoCollection DataSources {
			get { return this.DocumentModel.DataSources; }
		}
		public object DataSource { 
			get { return this.DocumentModel.DataSources.DefaultDataSourceInfo.DataSource; } 
			set { this.DocumentModel.DataSources.SetDefaultDataSource(value); } 
		}
		public CalculatedFieldCollection CalculatedFields { get { return this.DocumentModel.DataSources.DefaultDataSourceInfo.CalculatedFields; } }
		public DevExpress.Snap.Core.API.ParameterCollection Parameters { get { return DocumentModel.Parameters; } }
		public event EventHandler DataSourceChanged {
			add { this.DocumentModel.DataSourceChanged += value; }
			remove { this.DocumentModel.DataSourceChanged -= value; }
		}
		public void BeginUpdateDataSource() {
			DocumentModel.BeginUpdateDataSource();
		}
		public void EndUpdateDataSource() {
			DocumentModel.EndUpdateDataSource();
		}
		public Core.API.ThemeCollection Themes { get { return themes; } }
		#endregion
		public void SaveDocument(string fileName) {
			SaveDocument(fileName, SnapDocumentFormat.Snap);
		}
		public void SaveDocument(Stream stream) {
			SaveDocument(stream, SnapDocumentFormat.Snap);
		}
		public void ExportDocument(string fileName, DocumentFormat format) {
			SaveDocument(fileName, format);
		}
		public void ExportDocument(Stream stream, DocumentFormat format) {
			SaveDocument(stream, format);
		}
		public void SaveCurrentTheme(Stream stream) {
			DocumentModel.SaveCurrentTheme(stream);
		}
		public void SaveCurrentTheme(string fileName) {
			using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Write)) {
				SaveCurrentTheme(stream);
			}
		}
		public void SaveCurrentTheme(Stream stream, string newName) {
			DocumentModel.SaveCurrentTheme(stream, newName);
		}
		public ApiTheme LoadTheme(Stream stream) {
			DocumentModel.LoadTheme(stream);
			return Themes[ActiveThemeName];
		}
		public ApiTheme LoadTheme(string fileName) {
			using(FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				DocumentModel.LoadTheme(stream);
			}
			return Themes[ActiveThemeName];
		}
		public void ApplyTheme(string themeName) {
			DocumentModel.ApplyTheme(themeName);
		}
		protected internal override void Invalidate() {
			base.Invalidate();
			this.themes.Invalidate();
		}
		protected internal override void Initialize() {
			base.Initialize();
			this.themes = new NativeThemeCollection(this);
		}
		#region Snap Fields API
		public SnapEntity ActiveEntity { get { return fieldOwner.ActiveEntity; } }
		internal void SetActiveEntity(SnapEntity value) {
			this.fieldOwner.SetActiveEntity(value);
		}
		public SnapList FindListByName(string name) {
			return this.fieldOwner.FindListByName(name);
		}
		public SnapList CreateSnList(DocumentPosition position, string name) {
			return this.fieldOwner.CreateSnList(position, name);
		}
		public void RemoveSnList(string name) {
			this.fieldOwner.RemoveSnList(name);
		}
		public SnapEntity ParseField(ApiField field) {
			return this.fieldOwner.ParseField(field);
		}
		public SnapEntity ParseField(int index) {
			return this.fieldOwner.ParseField(index);
		}
		public void RemoveField(int index) {
			this.fieldOwner.RemoveField(index);
		}
		public SnapText CreateSnText(DocumentPosition position, string dataFieldName) {
			return this.fieldOwner.CreateSnText(position, dataFieldName);
		}
		public SnapImage CreateSnImage(DocumentPosition position, string dataFieldName) {
			return this.fieldOwner.CreateSnImage(position, dataFieldName);
		}
		public SnapCheckBox CreateSnCheckBox(DocumentPosition position, string dataFieldName) {
			return this.fieldOwner.CreateSnCheckBox(position, dataFieldName);
		}
		public SnapBarCode CreateSnBarCode(DocumentPosition position) {
			return this.fieldOwner.CreateSnBarCode(position);
		}
		public SnapSparkline CreateSnSparkline(DocumentPosition position, string dataFieldName) {
			return this.fieldOwner.CreateSnSparkline(position, dataFieldName);
		}
		public SnapHyperlink CreateSnHyperlink(DocumentPosition position, string dataFieldName) {
			return this.fieldOwner.CreateSnHyperlink(position, dataFieldName);
		}
		#endregion
		public new SnapSection InsertSection(DocumentPosition pos) {
			return (SnapSection)base.InsertSection(pos);
		}
		public new SnapSection AppendSection() {
			return (SnapSection)base.AppendSection();
		}
		public new SnapSection GetSection(DocumentPosition pos) {
			return (SnapSection)base.GetSection(pos);
		}
		protected override NativeSection CreateNativeSection(ModelSection section) {
			return new NativeSnapSection(this, section);
		}
		protected override NativeSubDocument CreateNativeSubDocument(PieceTable pieceTable, InnerRichEditDocumentServer server) {
			return new SnapNativeSubDocument((SnapPieceTable)pieceTable, server, this);
		}
		#region Snap Mail Merge
		public SnapMailMergeExportOptions CreateSnapMailMergeExportOptions() {
			ISnapDocumentServer server = DocumentServer as ISnapDocumentServer ?? new InternalSnapDocumentServer(DocumentModel).InnerServer;
			return server.CreateSnapMailMergeExportOptions();
		}
		public void SnapMailMerge(string fileName, DocumentFormat format) {
			ISnapDocumentServer server = DocumentServer as ISnapDocumentServer ?? new InternalSnapDocumentServer(DocumentModel).InnerServer;
			server.SnapMailMerge(fileName, format);
		}
		public void SnapMailMerge(Stream stream, DocumentFormat format) {
			ISnapDocumentServer server = DocumentServer as ISnapDocumentServer ?? new InternalSnapDocumentServer(DocumentModel).InnerServer;
			server.SnapMailMerge(stream, format);
		}
		public void SnapMailMerge(SnapDocument document) {
			ISnapDocumentServer server = DocumentServer as ISnapDocumentServer ?? new InternalSnapDocumentServer(DocumentModel).InnerServer;
			server.SnapMailMerge(document);
		}
		public void SnapMailMerge(SnapMailMergeExportOptions options, string fileName, DocumentFormat format) {
			ISnapDocumentServer server = DocumentServer as ISnapDocumentServer ?? new InternalSnapDocumentServer(DocumentModel).InnerServer;
			server.SnapMailMerge(options, fileName, format);
		}
		public void SnapMailMerge(SnapMailMergeExportOptions options, Stream stream, DocumentFormat format) {
			ISnapDocumentServer server = DocumentServer as ISnapDocumentServer ?? new InternalSnapDocumentServer(DocumentModel).InnerServer;
			server.SnapMailMerge(options, stream, format);
		}
		public void SnapMailMerge(SnapMailMergeExportOptions options, SnapDocument targetDocument) {
			ISnapDocumentServer server = DocumentServer as ISnapDocumentServer ?? new InternalSnapDocumentServer(DocumentModel).InnerServer;
			server.SnapMailMerge(options, targetDocument);
		}
		#endregion
		protected internal override DevExpress.XtraRichEdit.API.Native.TableCellStyle GetTableCellStyle(DevExpress.XtraRichEdit.Model.TableCell cell) {
			NativeTableCellStyleCollection styles = (NativeTableCellStyleCollection)TableCellStyles;
			return styles.GetStyle(cell.TableCellStyle);
		}
		protected internal override DevExpress.XtraRichEdit.Model.TableCellStyle GetInnerTableCellStyle(DevExpress.XtraRichEdit.API.Native.TableCellStyle style) {
			return ((NativeTableCellStyle)style).InnerStyle;
		}
		protected override NativeSectionCollection CreateNativeSections() {
			return new NativeSnapSectionCollection();
		}
		public new SnapDocumentRange CreateRange(int start, int length) {
			return (SnapDocumentRange)base.CreateRange(start, length);
		}
		public new SnapDocumentRange CreateRange(DocumentPosition start, int length) {
			return (SnapDocumentRange)base.CreateRange(start, length);
		}
		public new SnapDocumentPosition CreatePosition(int start) {
			return (SnapDocumentPosition)base.CreatePosition(start);
		}
		protected internal new SnapDocumentRange CreateRange(ModelLogPosition start, int length) {
			return CreateRange(((IConvertToInt<ModelLogPosition>)start).ToInt(), length);
		}
		protected internal new NativeDocumentRange CreateZeroLengthRange(ModelLogPosition logPosition) {
			return new SnapDocumentRange((SnapDocumentPosition)CreatePositionCore(logPosition), (SnapDocumentPosition)CreatePositionCore(logPosition));
		}
		protected override NativeDocumentPosition CreateNativePosition(XtraRichEdit.Model.DocumentModelPosition pos) {
			return new SnapDocumentPosition(this, pos);
		}
		protected override NativeDocumentRange CreateNativeRange(ModelPosition start, ModelPosition end) {
			return new SnapDocumentRange(this, start, end);
		}
		public event Core.API.PrepareSnListEventHandler PrepareSnList;
		public event Core.API.BeforeInsertSnListEventHandler BeforeInsertSnList;
		public event Core.API.AfterInsertSnListEventHandler AfterInsertSnList;
		public event Core.API.BeforeInsertSnListColumnsEventHandler BeforeInsertSnListColumns;
		public event Core.API.PrepareSnListColumnsEventHandler PrepareSnListColumns;
		public event Core.API.AfterInsertSnListColumnsEventHandler AfterInsertSnListColumns;
		public event Core.API.BeforeInsertSnListDetailEventHandler BeforeInsertSnListDetail;
		public event Core.API.PrepareSnListDetailEventHandler PrepareSnListDetail;
		public event Core.API.AfterInsertSnListDetailEventHandler AfterInsertSnListDetail;
		public event Core.API.BeforeInsertSnListRecordDataEventHandler BeforeInsertSnListRecordData;
		public event Core.API.AfterInsertSnListRecordDataEventHandler AfterInsertSnListRecordData;
	}
	public class NativeTableCellStyle : DevExpress.XtraRichEdit.API.Native.TableCellStyle {
		readonly NativeDocument document;
		readonly ModelTableCellStyle innerStyle;
		readonly TableCellPropertiesBase tableCellProperties;
		readonly ParagraphPropertiesWithTabs paragraphProperties;
		readonly CharacterPropertiesBase characterProperties;
		internal NativeTableCellStyle(NativeDocument document, ModelTableCellStyle innerStyle) {
			this.document = document;
			this.innerStyle = innerStyle;
			this.tableCellProperties = new NativeStyleTableCellProperties(document, innerStyle.TableCellProperties);
			this.paragraphProperties = new NativeStyleParagraphProperties(document, innerStyle.Tabs, innerStyle.ParagraphProperties);
			this.characterProperties = new NativeSimpleCharacterProperties(innerStyle.CharacterProperties);
		}
		public ModelTableCellStyle InnerStyle { get { return innerStyle; } }
		#region TableCellStyle Members
		public string Name { get { return innerStyle.StyleName; } set { innerStyle.StyleName = value; } }
		public bool IsDeleted { get { return innerStyle.Deleted; } }
		public DevExpress.XtraRichEdit.API.Native.TableCellStyle Parent {
			get {
				NativeTableCellStyleCollection styles = (NativeTableCellStyleCollection)((SnapNativeDocument)document).TableCellStyles;
				return styles.GetStyle(innerStyle.Parent);
			}
			set {
				ModelTableCellStyle style = value != null ? ((NativeTableCellStyle)value).InnerStyle : null;
				innerStyle.Parent = style;
			}
		}
		void TableCellPropertiesBase.Reset() {
			tableCellProperties.Reset();
		}
		public void Reset(TableCellPropertiesMask mask) {
			tableCellProperties.Reset(mask);
		}
		#endregion
		#region CharacterProperties Members
		public string FontName { get { return characterProperties.FontName; } set { characterProperties.FontName = value; } }
		public float? FontSize { get { return characterProperties.FontSize; } set { characterProperties.FontSize = value; } }
		public bool? Bold { get { return characterProperties.Bold; } set { characterProperties.Bold = value; } }
		public bool? Italic { get { return characterProperties.Italic; } set { characterProperties.Italic = value; } }
		public bool? Superscript { get { return characterProperties.Superscript; } set { characterProperties.Superscript = value; } }
		public bool? Subscript { get { return characterProperties.Subscript; } set { characterProperties.Subscript = value; } }
		public bool? AllCaps { get { return characterProperties.AllCaps; } set { characterProperties.AllCaps = value; } }
		public DevExpress.XtraRichEdit.API.Native.UnderlineType? Underline { get { return characterProperties.Underline; } set { characterProperties.Underline = value; } }
		public DevExpress.XtraRichEdit.API.Native.StrikeoutType? Strikeout { get { return characterProperties.Strikeout; } set { characterProperties.Strikeout = value; } }
		public Color? UnderlineColor { get { return characterProperties.UnderlineColor; } set { characterProperties.UnderlineColor = value; } }
		public Color? ForeColor { get { return characterProperties.ForeColor; } set { characterProperties.ForeColor = value; } }
		Color? CharacterPropertiesBase.BackColor { get { return characterProperties.BackColor; } set { characterProperties.BackColor = value; } }
		public bool? Hidden { get { return characterProperties.Hidden; } set { characterProperties.Hidden = value; } }
		public bool? NoProof { get { return characterProperties.NoProof; } set { characterProperties.NoProof = value; } }
		void CharacterPropertiesBase.Reset() {
			characterProperties.Reset();
		}
		public void Reset(CharacterPropertiesMask mask) {
			characterProperties.Reset(mask);
		}
		#endregion
		#region ParagraphProperties Members
		public DevExpress.XtraRichEdit.API.Native.ParagraphAlignment? Alignment { get { return paragraphProperties.Alignment; } set { paragraphProperties.Alignment = value; } }
		public float? LeftIndent { get { return paragraphProperties.LeftIndent; } set { paragraphProperties.LeftIndent = value; } }
		public float? RightIndent { get { return paragraphProperties.RightIndent; } set { paragraphProperties.RightIndent = value; } }
		public float? SpacingBefore { get { return paragraphProperties.SpacingBefore; } set { paragraphProperties.SpacingBefore = value; } }
		public float? SpacingAfter { get { return paragraphProperties.SpacingAfter; } set { paragraphProperties.SpacingAfter = value; } }
		public DevExpress.XtraRichEdit.API.Native.ParagraphLineSpacing? LineSpacingType { get { return paragraphProperties.LineSpacingType; } set { paragraphProperties.LineSpacingType = value; } }
		public float? LineSpacing { get { return paragraphProperties.LineSpacing; } set { paragraphProperties.LineSpacing = value; } }
		public float? LineSpacingMultiplier { get { return paragraphProperties.LineSpacingMultiplier; } set { paragraphProperties.LineSpacingMultiplier = value; } }
		public DevExpress.XtraRichEdit.API.Native.ParagraphFirstLineIndent? FirstLineIndentType { get { return paragraphProperties.FirstLineIndentType; } set { paragraphProperties.FirstLineIndentType = value; } }
		public float? FirstLineIndent { get { return paragraphProperties.FirstLineIndent; } set { paragraphProperties.FirstLineIndent = value; } }
		public bool? SuppressHyphenation { get { return paragraphProperties.SuppressHyphenation; } set { paragraphProperties.SuppressHyphenation = value; } }
		public bool? SuppressLineNumbers { get { return paragraphProperties.SuppressLineNumbers; } set { paragraphProperties.SuppressLineNumbers = value; } }
		public int? OutlineLevel { get { return paragraphProperties.OutlineLevel; } set { paragraphProperties.OutlineLevel = value; } }
		public bool? KeepLinesTogether { get { return paragraphProperties.KeepLinesTogether; } set { paragraphProperties.KeepLinesTogether = value; } }
		public bool? PageBreakBefore { get { return paragraphProperties.PageBreakBefore; } set { paragraphProperties.PageBreakBefore = value; } }
		public bool? ContextualSpacing { get { return paragraphProperties.ContextualSpacing; } set { paragraphProperties.ContextualSpacing = value; } }
		Color? ParagraphPropertiesBase.BackColor { get { return paragraphProperties.BackColor; } set { paragraphProperties.BackColor = value; } }
		public TabInfoCollection BeginUpdateTabs(bool onlyOwnTabs) {
			return paragraphProperties.BeginUpdateTabs(onlyOwnTabs);
		}
		public void EndUpdateTabs(TabInfoCollection tabs) {
			paragraphProperties.EndUpdateTabs(tabs);
		}
		void ParagraphPropertiesBase.Reset() {
			paragraphProperties.Reset();
		}
		public void Reset(ParagraphPropertiesMask mask) {
			paragraphProperties.Reset(mask);
		}
		#endregion
		#region TableCellPropertiesBase Members
		float? TableCellPropertiesBase.CellTopPadding { get { return tableCellProperties.CellTopPadding; } set { tableCellProperties.CellTopPadding = value; } }
		float? TableCellPropertiesBase.CellBottomPadding { get { return tableCellProperties.CellBottomPadding; } set { tableCellProperties.CellBottomPadding = value; } }
		float? TableCellPropertiesBase.CellLeftPadding { get { return tableCellProperties.CellLeftPadding; } set { tableCellProperties.CellLeftPadding = value; } }
		float? TableCellPropertiesBase.CellRightPadding { get { return tableCellProperties.CellRightPadding; } set { tableCellProperties.CellRightPadding = value; } }
		public TableCellVerticalAlignment? VerticalAlignment { get { return tableCellProperties.VerticalAlignment; } set { tableCellProperties.VerticalAlignment = value; } }
		public bool? NoWrap { get { return tableCellProperties.NoWrap; } set { tableCellProperties.NoWrap = value; } }
		public DevExpress.XtraRichEdit.API.Native.TableCellBorders TableCellBorders { get { return tableCellProperties.TableCellBorders; } }
		public Color? CellBackgroundColor { get { return tableCellProperties.CellBackgroundColor; } set { tableCellProperties.CellBackgroundColor = value; } }
		#endregion
		public LangInfo? Language
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
	}
	public class NativeTableCellStyleCollection : NativeStyleCollectionBase<DevExpress.XtraRichEdit.API.Native.TableCellStyle, ModelTableCellStyle>, DevExpress.XtraRichEdit.API.Native.TableCellStyleCollection {
		internal NativeTableCellStyleCollection(NativeDocument document)
			: base(document) {
		}
		protected internal override StyleCollectionBase<ModelTableCellStyle> InnerStyles { get { return DocumentModel.TableCellStyles; } }
		protected internal override DevExpress.XtraRichEdit.API.Native.TableCellStyle CreateNew(ModelTableCellStyle style) {
			return new NativeTableCellStyle(Document, style);
		}
		protected internal override ModelTableCellStyle CreateModelStyle() {
			return new ModelTableCellStyle(DocumentModel);
		}
		protected internal override ModelTableCellStyle GetModelStyle(DevExpress.XtraRichEdit.API.Native.TableCellStyle style) {
			return ((NativeTableCellStyle)style).InnerStyle;
		}
	}
	public class SnapNativeFieldOwner : ISnapFieldOwner {
		#region static
		static readonly Dictionary<Type, ConstructorInfo> fieldTypes;
		static SnapNativeFieldOwner() {
			fieldTypes = new Dictionary<Type, ConstructorInfo>();
			fieldTypes.Add(typeof(SNListField), typeof(NativeSnapList).GetConstructor(new[] { typeof(SnapSubDocument), typeof(SnapNativeDocument), typeof(ApiField) }));
			fieldTypes.Add(typeof(SNTextField), typeof(NativeSnapText).GetConstructor(new[] { typeof(SnapSubDocument), typeof(SnapNativeDocument), typeof(ApiField) }));
			fieldTypes.Add(typeof(SNImageField), typeof(NativeSnapImage).GetConstructor(new[] { typeof(SnapSubDocument), typeof(SnapNativeDocument), typeof(ApiField) }));
			fieldTypes.Add(typeof(SNBarCodeField), typeof(NativeSnapBarCode).GetConstructor(new[] { typeof(SnapSubDocument), typeof(SnapNativeDocument), typeof(ApiField) }));
			fieldTypes.Add(typeof(SNCheckBoxField), typeof(NativeSnapCheckBox).GetConstructor(new[] { typeof(SnapSubDocument), typeof(SnapNativeDocument), typeof(ApiField) }));
			fieldTypes.Add(typeof(SNSparklineField), typeof(NativeSnapSparkline).GetConstructor(new[] { typeof(SnapSubDocument), typeof(SnapNativeDocument), typeof(ApiField) }));
			fieldTypes.Add(typeof(SNHyperlinkField), typeof(NativeSnapHyperlink).GetConstructor(new[] { typeof(SnapSubDocument), typeof(SnapNativeDocument), typeof(ApiField) }));
		}
		#endregion
		#region Fields
		readonly SnapNativeDocument document;
		readonly NativeSubDocument subDocument;
		protected Dictionary<string, NativeSnapList> lists;
		SnapEntity activeEntity;
		#endregion
		public SnapNativeFieldOwner(NativeSubDocument subDocument, SnapNativeDocument document) {
			Guard.ArgumentNotNull(subDocument, "subDocument");
			Guard.ArgumentNotNull(document, "document");
			this.subDocument = subDocument;
			this.document = document;
			this.lists = new Dictionary<string, NativeSnapList>();
		}
		#region Properties
		protected ModelPieceTable PieceTable { get { return (ModelPieceTable)subDocument.PieceTable; } }
		protected DocumentModel DocumentModel { get { return (DocumentModel)subDocument.DocumentModel; } }
		protected SnapSubDocument SnapSubDocument { get { return (SnapSubDocument)subDocument; } }
		protected NativeSubDocument NativeSubDocument { get { return subDocument; } }
		public SnapNativeDocument Document { get { return document; } }
		protected internal static Dictionary<Type, ConstructorInfo> FieldTypes { get { return fieldTypes; } }
		#endregion
		internal void SetActiveEntity(SnapEntity value) {
			if (Object.ReferenceEquals(this.activeEntity, value))
				return;
			if (activeEntity == null)
				activeEntity = value;
			else if (activeEntity.Active)
				throw new InvalidOperationException();
			else {
				NativeSnapList list = activeEntity as NativeSnapList;
				if (list != null)
					lists.Remove(list.Name);
				activeEntity = value;
			}
		}
		#region ISnapFieldOwner members
		public SnapEntity ActiveEntity { get { return activeEntity; } }
		public SnapList FindListByName(string name) {
			if (lists.ContainsKey(name))
				return lists[name];
			ModelField field = GetListField(name);
			if (field == null)
				return null;
			NativeSnapList list = new NativeSnapList(SnapSubDocument, Document, new NativeField(NativeSubDocument, field));
			lists.Add(name, list);
			return list;
		}
		public SnapList CreateSnList(DocumentPosition position, string name) {
			name = name ?? string.Empty;
			string fieldCode = !string.IsNullOrEmpty(name) ? String.Format("SnList \\name {0}", name) : "SnList";
			ModelField field = CreateField(position, fieldCode, "SnapList");
			NativeSnapList result = new NativeSnapList(SnapSubDocument, Document, new NativeField(NativeSubDocument, field));
			if (!lists.ContainsKey(name))
				lists.Add(name, result);
			else {
				Debug.WriteLine(string.Format("List named '{0}' already exists. Replacing.", name));
				lists[name] = result;
			}
			return result;
		}
		public void RemoveSnList(string name) {
			if (activeEntity != null)
				throw new InvalidOperationException(SnapLocalizer.GetString(SnapStringId.SnapEntityRemoveLock));
			ModelField field = GetListField(name);
			if (field == null)
				return;
			if (lists.ContainsKey(name))
				lists.Remove(name);
			NativeSubDocument.BeginUpdate();
			try {
				PieceTable.DeleteFieldWithResult(field);
			}
			finally {
				NativeSubDocument.EndUpdate();
			}
		}
		ModelField CreateField(DocumentPosition position, string fieldCode, string fieldType) {
			if (activeEntity != null)
				throw new InvalidOperationException(String.Format(SnapLocalizer.GetString(SnapStringId.SnapEntityAddLock), fieldType));
			NativeSubDocument.BeginUpdate();
			ModelField field;
			try {
				DocumentRange codeRange = NativeSubDocument.InsertText(position, fieldCode);
				field = PieceTable.CreateField(codeRange.Start.LogPosition, codeRange.Length);
			}
			finally {
				NativeSubDocument.EndUpdate();
			}
			return field;
		}
		ModelField GetListField(string name) {
			return PieceTable.FindFieldNearestToLogPosition(name, true, DocumentLogPosition.Zero);
		}
		public SnapEntity ParseField(ApiField field) {
			SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
			CalculatedFieldBase parsedField = calculator.ParseField(PieceTable, ((NativeField)field).Field);
			if ((parsedField == null) || !FieldTypes.ContainsKey(parsedField.GetType()))
				return null;
			ConstructorInfo ctor = FieldTypes[parsedField.GetType()];
			return (SnapEntity)ctor.Invoke(new object[] { SnapSubDocument, Document, field });
		}
		public SnapEntity ParseField(int index) {
			return ParseField(NativeSubDocument.Fields[index]);
		}
		public void RemoveField(int index) {
			if (activeEntity != null)
				throw new InvalidOperationException(SnapLocalizer.GetString(SnapStringId.SnapEntityRemoveLock));
			NativeSubDocument.BeginUpdate();
			try {
				PieceTable.DeleteFieldWithResult(((NativeField)NativeSubDocument.Fields[index]).Field);
			}
			finally {
				NativeSubDocument.EndUpdate(); 
			}
		}
		public SnapText CreateSnText(DocumentPosition position, string dataFieldName) {
			string fieldCode = String.Format("SnText {0}", dataFieldName);
			ModelField field = CreateField(position, fieldCode, "SnapText");
			NativeSnapText result = new NativeSnapText(SnapSubDocument, Document, new NativeField(NativeSubDocument, field));
			return result;
		}
		public SnapImage CreateSnImage(DocumentPosition position, string dataFieldName) {
			string fieldCode = String.Format("SnImage {0}", dataFieldName);
			ModelField field = CreateField(position, fieldCode, "SnapImage");
			return new NativeSnapImage(SnapSubDocument, Document, new NativeField(NativeSubDocument, field));
		}
		public SnapCheckBox CreateSnCheckBox(DocumentPosition position, string dataFieldName) {
			string fieldCode = String.Format("SnCheckBox {0}", dataFieldName);
			ModelField field = CreateField(position, fieldCode, "SnapCheckBox");
			NativeSnapCheckBox result = new NativeSnapCheckBox(SnapSubDocument, Document, new NativeField(NativeSubDocument, field));
			return result;
		}
		public SnapBarCode CreateSnBarCode(DocumentPosition position) {
			const string fieldCode = "SnBarCode";
			ModelField field = CreateField(position, fieldCode, "SnapBarCode");
			NativeSnapBarCode result = new NativeSnapBarCode(SnapSubDocument, Document, new NativeField(NativeSubDocument, field));
			return result;
		}
		public SnapSparkline CreateSnSparkline(DocumentPosition position, string dataFieldName) {
			string fieldCode = String.Format("SnSparkline {0}", dataFieldName);
			ModelField field = CreateField(position, fieldCode, "SnapSparkline");
			return new NativeSnapSparkline(SnapSubDocument, Document, new NativeField(NativeSubDocument, field));
		}
		public SnapHyperlink CreateSnHyperlink(DocumentPosition position, string dataFieldName) {
			string fieldCode = String.Format("SnHyperlink {0}", dataFieldName);
			ModelField field = CreateField(position, fieldCode, "SnapHyperlink");
			NativeSnapHyperlink result = new NativeSnapHyperlink(SnapSubDocument, Document, new NativeField(NativeSubDocument, field));
			return result;
		}
		#endregion
	}
}
