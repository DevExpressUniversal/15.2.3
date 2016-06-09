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
using System.IO;
using System.Text;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Import;
using DevExpress.XtraRichEdit.Export.Rtf;
using DevExpress.XtraRichEdit.Import.Rtf;
using DevExpress.XtraRichEdit.Export.Html;
using DevExpress.XtraRichEdit.Import.Html;
using DevExpress.XtraRichEdit.Export.Mht;
using DevExpress.XtraRichEdit.Import.Mht;
using DevExpress.XtraRichEdit.Export.OpenXml;
using DevExpress.XtraRichEdit.Import.OpenXml;
using DevExpress.XtraRichEdit.Export.OpenDocument;
using DevExpress.XtraRichEdit.Import.OpenDocument;
using DevExpress.XtraRichEdit.Export.PlainText;
using DevExpress.XtraRichEdit.Import.WordML;
using DevExpress.XtraRichEdit.Export.WordML;
using DevExpress.XtraRichEdit.Import.Xaml;
using DevExpress.XtraRichEdit.Export.Xaml;
using DevExpress.XtraRichEdit.Services.Implementation;
using DevExpress.Office.Services.Implementation;
using DevExpress.Office.Services;
using DevExpress.XtraRichEdit.Import.Doc;
using DevExpress.XtraRichEdit.Export.Doc;
using DevExpress.Office.API.Internal;
using OfficeUnitConverter = DevExpress.Office.API.Internal.UnitConverter;
using DevExpress.Internal;
namespace DevExpress.XtraRichEdit.API.Internal {
	#region InternalAPI
	public class InternalAPI : IDocumentModelStructureChangedListener {
		#region Fields
		readonly DocumentModel documentModel;
		string cachedRtfContent;
		string cachedHtmlContent;
		string cachedMhtContent;
		string cachedWordMLContent;
		string cachedPlainTextContent;
		string cachedXamlContent;
		byte[] cachedOpenXmlContent;
		byte[] cachedOpenDocumentContent;
		byte[] cachedDocContent;
		readonly List<WeakReference> documentPositions;
		readonly List<IDocumentModelModifier> activeModifiers;
		internal readonly Dictionary<DocumentUnit, OfficeUnitConverter> UnitConverters;
		IDocumentExportersFactory exportersFactory;
		IDocumentImportersFactory importersFactory;
		#endregion
		public InternalAPI(DocumentModel documentModel, IDocumentExportersFactory exportersFactory, IDocumentImportersFactory importersFactory) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			Guard.ArgumentNotNull(exportersFactory, "exportersFactory");
			Guard.ArgumentNotNull(importersFactory, "importersFactory");
			this.documentModel = documentModel;
			this.documentPositions = new List<WeakReference>();
			this.activeModifiers = new List<IDocumentModelModifier>();
			this.exportersFactory = exportersFactory;
			this.importersFactory = importersFactory;
			UnitConverters = CreateUnitConverters();
			SubscribeDocumentModelEvents();
		}
		#region Properties
		public DocumentModel DocumentModel { get { return documentModel; } }
		public bool Modified { get { return DocumentModel.Modified; } set { DocumentModel.Modified = value; } }
		#region RtfText
		public string RtfText {
			get {
				if (cachedRtfContent == null)
					cachedRtfContent = GetDocumentRtfContent();
				return cachedRtfContent;
			}
			set {
				if (cachedRtfContent != null && cachedRtfContent == value)
					return;
				if (String.IsNullOrEmpty(value))
					this.Text = String.Empty;
				else
					SetDocumentContent(value, SetDocumentRtfContent);
			}
		}
		#endregion
		#region HtmlText
		public string HtmlText {
			get {
				if (cachedHtmlContent == null)
					cachedHtmlContent = GetDocumentHtmlContent(new DevExpress.Office.Services.Implementation.DataStringUriProvider());
				return cachedHtmlContent;
			}
			set {
				if (cachedHtmlContent != null && cachedHtmlContent == value)
					return;
				if (String.IsNullOrEmpty(value))
					this.Text = String.Empty;
				else
					SetDocumentContent(value, SetDocumentHtmlContent);
			}
		}
		#endregion
		#region MhtText
		public string MhtText {
			get {
				if (cachedMhtContent == null)
					cachedMhtContent = GetDocumentMhtContent();
				return cachedMhtContent;
			}
			set {
				if (cachedMhtContent != null && cachedMhtContent == value)
					return;
				if (String.IsNullOrEmpty(value))
					this.Text = String.Empty;
				else
					SetDocumentContent(value, SetDocumentMhtContent);
			}
		}
		#endregion
		#region WordMLText
		public string WordMLText {
			get {
				if (cachedWordMLContent == null)
					cachedWordMLContent = GetDocumentWordMLContent();
				return cachedWordMLContent;
			}
			set {
				if (cachedWordMLContent != null && cachedWordMLContent == value)
					return;
				if (String.IsNullOrEmpty(value))
					this.Text = String.Empty;
				else
					SetDocumentContent(value, SetDocumentWordMLContent);
			}
		}
		#endregion
		#region XamlText
		public string XamlText {
			get {
				if (cachedXamlContent == null)
					cachedXamlContent = GetDocumentXamlContent();
				return cachedXamlContent;
			}
			set {
				if (cachedXamlContent != null && cachedXamlContent == value)
					return;
				if (String.IsNullOrEmpty(value))
					this.Text = String.Empty;
				else
					SetDocumentContent(value, SetDocumentXamlContent);
			}
		}
		#endregion
		#region OpenXmlBytes
		public byte[] OpenXmlBytes {
			get {
				if (cachedOpenXmlContent == null)
					cachedOpenXmlContent = GetDocumentOpenXmlContent();
				return cachedOpenXmlContent;
			}
			set {
				if (cachedOpenXmlContent != null && cachedOpenXmlContent == value)
					return;
				if (value == null)
					Text = String.Empty;
				else
					SetDocumentContent(value, SetDocumentOpenXmlContent);
			}
		}
		#endregion
		#region OpenDocumentBytes
		public byte[] OpenDocumentBytes {
			get {
				if (cachedOpenDocumentContent == null)
					cachedOpenDocumentContent = GetDocumentOpenDocumentContent();
				return cachedOpenDocumentContent;
			}
			set {
				if (cachedOpenDocumentContent != null && cachedOpenDocumentContent == value)
					return;
				if (value == null)
					this.Text = String.Empty;
				else
					SetDocumentContent(value, SetDocumentOpenDocumentContent);
			}
		}
		#endregion
		#region Doc
		public byte[] DocBytes {
			get {
				if (cachedDocContent == null)
					cachedDocContent = GetDocumentDocContent();
				return cachedDocContent;
			}
			set {
				if (cachedDocContent != null && cachedDocContent == value)
					return;
				if (value == null)
					this.Text = String.Empty;
				else
					SetDocumentContent(value, SetDocumentDocContent);
			}
		}
		#endregion
		#region Text
		public string Text {
			get {
				if (cachedPlainTextContent == null)
					cachedPlainTextContent = GetDocumentPlainTextContent();
				return cachedPlainTextContent;
			}
			set {
				if (cachedPlainTextContent != null && cachedPlainTextContent == value)
					if (cachedPlainTextContent.Length == 0 && !DocumentModel.IsEmpty)
						SetDocumentContent(value, SetDocumentPlainTextContent);
					else
						return;
				SetDocumentContent(value, SetDocumentPlainTextContent);
			}
		}
		#endregion
		#region ExporterFactory
		public IDocumentExportersFactory ExporterFactory {
			get {
				return this.exportersFactory;
			}
			set { this.exportersFactory = value; }
		}
		#endregion
		#region ImporterFactory
		public IDocumentImportersFactory ImporterFactory {
			get {
				return this.importersFactory;
			}
			set { this.importersFactory = value; }
		}
		#endregion
		#endregion
		#region Events
		#region ParagraphInserted
		ParagraphEventHandler onParagraphInserted;
		public event ParagraphEventHandler ParagraphInserted { add { onParagraphInserted += value; } remove { onParagraphInserted -= value; } }
		protected internal virtual void RaiseParagraphInserted(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex) {
			if (onParagraphInserted != null)
				onParagraphInserted(this, new ParagraphEventArgs(pieceTable, sectionIndex, paragraphIndex));
		}
		#endregion
		#region ParagraphRemoved
		ParagraphEventHandler onParagraphRemoved;
		public event ParagraphEventHandler ParagraphRemoved { add { onParagraphRemoved += value; } remove { onParagraphRemoved -= value; } }
		protected internal virtual void RaiseParagraphRemoved(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex) {
			if (onParagraphRemoved != null)
				onParagraphRemoved(this, new ParagraphEventArgs(pieceTable, sectionIndex, paragraphIndex));
		}
		#endregion
		#region ParagraphMerged
		ParagraphEventHandler onParagraphMerged;
		public event ParagraphEventHandler ParagraphMerged { add { onParagraphMerged += value; } remove { onParagraphMerged -= value; } }
		protected internal virtual void RaiseParagraphMerged(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex) {
			if (onParagraphMerged != null)
				onParagraphMerged(this, new ParagraphEventArgs(pieceTable, sectionIndex, paragraphIndex));
		}
		#endregion
		#region SectionInserted
		SectionEventHandler onSectionInserted;
		internal event SectionEventHandler SectionInserted { add { onSectionInserted += value; } remove { onSectionInserted -= value; } }
		protected internal virtual void RaiseSectionInserted(SectionEventArgs e) {
			if (onSectionInserted != null)
				onSectionInserted(this, e);
		}
		#endregion
		#region SectionRemoved
		SectionEventHandler onSectionRemoved;
		internal event SectionEventHandler SectionRemoved { add { onSectionRemoved += value; } remove { onSectionRemoved -= value; } }
		protected internal virtual void RaiseSectionRemoved(SectionEventArgs e) {
			if (onSectionRemoved != null)
				onSectionRemoved(this, e);
		}
		#endregion
		#region DocumentReplaced
		EventHandler onDocumentReplaced;
		internal event EventHandler DocumentReplaced { add { onDocumentReplaced += value; } remove { onDocumentReplaced -= value; } }
		protected internal virtual void RaiseDocumentReplaced() {
			if (onDocumentReplaced != null)
				onDocumentReplaced(this, EventArgs.Empty);
		}
		#endregion
		#region FieldInserted
		FieldEventHandler onFieldInserted;
		public event FieldEventHandler FieldInserted { add { onFieldInserted += value; } remove { onFieldInserted -= value; } }
		protected internal virtual void RaiseFieldInserted(PieceTable pieceTable, int fieldIndex) {
			if (onFieldInserted != null)
				onFieldInserted(this, new FieldEventArgs(pieceTable, fieldIndex));
		}
		#endregion
		#region FieldRemoved
		FieldEventHandler onFieldRemoved;
		public event FieldEventHandler FieldRemoved { add { onFieldRemoved += value; } remove { onFieldRemoved -= value; } }
		protected internal virtual void RaiseFieldRemoved(PieceTable pieceTable, int fieldIndex) {
			if (onFieldRemoved != null)
				onFieldRemoved(this, new FieldEventArgs(pieceTable, fieldIndex));
		}
		#endregion
		#region HyperlinkInfoInserted
		HyperlinkInfoEventHandler onHyperlinkInfoInserted;
		internal event HyperlinkInfoEventHandler HyperlinkInfoInserted { add { onHyperlinkInfoInserted += value; } remove { onHyperlinkInfoInserted -= value; } }
		protected internal virtual void RaiseHyperlinkInfoInserted(HyperlinkInfoEventArgs args) {
			if (onHyperlinkInfoInserted != null)
				onHyperlinkInfoInserted(this, args);
		}
		#endregion
		#region HyperlinkInfoDeleted
		HyperlinkInfoEventHandler onHyperlinkInfoDeleted;
		internal event HyperlinkInfoEventHandler HyperlinkInfoDeleted { add { onHyperlinkInfoDeleted += value; } remove { onHyperlinkInfoDeleted -= value; } }
		protected internal virtual void RaiseHyperlinkInfoDeleted(HyperlinkInfoEventArgs args) {
			if (onHyperlinkInfoDeleted != null)
				onHyperlinkInfoDeleted(this, args);
		}
		#endregion
		#region CustomMarkInserted
		CustomMarkEventHandler onCustomMarkInserted;
		internal event CustomMarkEventHandler CustomMarkInserted { add { onCustomMarkInserted += value; } remove { onCustomMarkInserted -= value; } }
		protected internal virtual void RaiseCustomMarkInserted(CustomMarkEventArgs args) {
			if (onCustomMarkInserted != null)
				onCustomMarkInserted(this, args);
		}
		#endregion
		#region CustomMarkDeleted
		CustomMarkEventHandler onCustomMarkDeleted;
		internal event CustomMarkEventHandler CustomMarkDeleted { add { onCustomMarkDeleted += value; } remove { onCustomMarkDeleted -= value; } }
		protected internal virtual void RaiseCustomMarkDeleted(CustomMarkEventArgs args) {
			if (onCustomMarkDeleted != null)
				onCustomMarkDeleted(this, args);
		}
		#endregion
		#endregion
		protected internal virtual void ResetCachedContent() {
			this.cachedPlainTextContent = null;
			this.cachedRtfContent = null;
			this.cachedHtmlContent = null;
			this.cachedMhtContent = null;
			this.cachedWordMLContent = null;
			this.cachedXamlContent = null;
			this.cachedOpenXmlContent = null;
			this.cachedOpenDocumentContent = null;
			this.cachedDocContent = null;
		}
		protected internal void SetDocumentContent<T>(T value, Action<T> setterMethod) {
			ResetCachedContent();
			UnsubscribeDocumentModelEvents();
			SubscribeDocumentModelEventsSetContentMode();
			try {
				SuspendProgressIndication();
				try {
					setterMethod(value);
				}
				finally {
					ResumeProgressIndication();
				}
			}
			finally {
				UnsubscribeDocumentModelEventsSetContentMode();
				SubscribeDocumentModelEvents();
			}
		}
		protected internal void SubscribeDocumentModelEvents() {
			DocumentModel.InnerContentChanged += OnDocumentModelContentChanged;
			DocumentModel.BeforeEndDocumentUpdate += OnEndDocumentUpdate;
			DocumentModel.AfterEndDocumentUpdate += OnEndDocumentUpdate;
			DocumentModel.SectionInserted += OnSectionInserted;
			DocumentModel.SectionRemoved += OnSectionRemoved;
			DocumentModel.HyperlinkInfoInserted += OnHyperlinkInfoInserted;
			DocumentModel.HyperlinkInfoDeleted += OnHyperlinkInfoDeleted;
			DocumentModel.CustomMarkInserted += OnCustomMarkInserted;
			DocumentModel.CustomMarkDeleted += OnCustomMarkDeleted;
		}
		protected internal void UnsubscribeDocumentModelEvents() {
			DocumentModel.InnerContentChanged -= OnDocumentModelContentChanged;
			DocumentModel.BeforeEndDocumentUpdate -= OnEndDocumentUpdate;
			DocumentModel.AfterEndDocumentUpdate -= OnEndDocumentUpdate;
			DocumentModel.SectionInserted -= OnSectionInserted;
			DocumentModel.SectionRemoved -= OnSectionRemoved;
			DocumentModel.HyperlinkInfoInserted -= OnHyperlinkInfoInserted;
			DocumentModel.HyperlinkInfoDeleted -= OnHyperlinkInfoDeleted;
			DocumentModel.CustomMarkInserted -= OnCustomMarkInserted;
			DocumentModel.CustomMarkDeleted -= OnCustomMarkDeleted;
		}
		protected internal void SubscribeDocumentModelEventsSetContentMode() {
			DocumentModel.BeforeEndDocumentUpdate += OnEndDocumentUpdateSetContentMode;
			DocumentModel.AfterEndDocumentUpdate += OnEndDocumentUpdateSetContentMode;
		}
		protected internal void UnsubscribeDocumentModelEventsSetContentMode() {
			DocumentModel.BeforeEndDocumentUpdate -= OnEndDocumentUpdateSetContentMode;
			DocumentModel.AfterEndDocumentUpdate -= OnEndDocumentUpdateSetContentMode;
		}
		protected internal virtual void SuspendProgressIndication() {
			IRichEditProgressIndicationService service = DocumentModel.GetService<IRichEditProgressIndicationService>();
			if (service != null)
				service.SuspendProgressIndication();
		}
		protected internal virtual void ResumeProgressIndication() {
			IRichEditProgressIndicationService service = DocumentModel.GetService<IRichEditProgressIndicationService>();
			if (service != null)
				service.ResumeProgressIndication();
		}
		protected internal virtual void OnDocumentModelContentChanged(object sender, EventArgs e) {
			ResetCachedContent();
		}
		protected internal virtual void OnEndDocumentUpdate(object sender, DocumentUpdateCompleteEventArgs e) {
			DocumentModelChangeActions changeActions = e.DeferredChanges.ChangeActions;
			if ((changeActions & DocumentModelChangeActions.RaiseDocumentLoaded) != 0 ||
				(changeActions & DocumentModelChangeActions.RaiseEmptyDocumentCreated) != 0 ||
				(changeActions & DocumentModelChangeActions.RaiseContentChanged) != 0)
				ResetCachedContent();
			if ((changeActions & DocumentModelChangeActions.RaiseDocumentLoaded) != 0 ||
				(changeActions & DocumentModelChangeActions.RaiseEmptyDocumentCreated) != 0)
				OnDocumentReplaced();
		}
		protected internal virtual void OnEndDocumentUpdateSetContentMode(object sender, DocumentUpdateCompleteEventArgs e) {
			DocumentModelChangeActions changeActions = e.DeferredChanges.ChangeActions;
			if ((changeActions & DocumentModelChangeActions.RaiseDocumentLoaded) != 0 ||
				(changeActions & DocumentModelChangeActions.RaiseEmptyDocumentCreated) != 0)
				OnDocumentReplaced();
		}
		protected internal virtual void OnDocumentReplaced() {
			documentPositions.Clear();
			RaiseDocumentReplaced();
		}
		protected internal virtual void OnHyperlinkInfoDeleted(object sender, HyperlinkInfoEventArgs e) {
			RaiseHyperlinkInfoDeleted(e);
		}
		protected internal virtual void OnHyperlinkInfoInserted(object sender, HyperlinkInfoEventArgs e) {
			RaiseHyperlinkInfoInserted(e);
		}
		protected internal virtual void OnCustomMarkInserted(object sender, CustomMarkEventArgs e) {
			RaiseCustomMarkInserted(e);
		}
		protected internal virtual void OnCustomMarkDeleted(object sender, CustomMarkEventArgs e) {
			RaiseCustomMarkDeleted(e);
		}
		public virtual void RegisterAnchor(DocumentModelPositionAnchor pos) {
			documentPositions.Add(new WeakReference(pos));
		}
		public virtual void UnregisterAnchor(DocumentModelPositionAnchor pos) {
			for (int i = documentPositions.Count - 1; i >= 0; i--) {
				WeakReference reference = documentPositions[i];
				DocumentModelPositionAnchor anchor = (DocumentModelPositionAnchor)reference.Target;
				if (anchor != null) {
					if (Object.ReferenceEquals(anchor, pos)) {
						documentPositions.RemoveAt(i);
						break;
					}
				}
				else
					documentPositions.RemoveAt(i);
			}
		}
		protected internal virtual void UpdateAnchors(Action<DocumentModelPositionAnchor> anchorAction) {
			for (int i = documentPositions.Count - 1; i >= 0; i--) {
				WeakReference reference = documentPositions[i];
				DocumentModelPositionAnchor anchor = (DocumentModelPositionAnchor)reference.Target;
				if (anchor != null)
					anchorAction(anchor);
				else
					documentPositions.RemoveAt(i);
			}
		}
		protected internal virtual void ClearAnchors() {
			documentPositions.Clear();
		}
		Dictionary<DocumentUnit, OfficeUnitConverter> CreateUnitConverters() {
			UnitConvertersCreator creator = new UnitConvertersCreator();
			return creator.CreateUnitConverters(documentModel.UnitConverter);
		}
		public void CreateNewDocument() {
			DocumentModel.BeginUpdate();
			try {
				DocumentModel.DocumentSaveOptions.ResetCurrentFileName();
				DocumentModel.DocumentSaveOptions.ResetCurrentFormat();
				SetDocumentPlainTextContentCore(String.Empty, DocumentModelChangeType.CreateEmptyDocument);
				DocumentModel.SetActivePieceTable(DocumentModel.MainPieceTable, null);
			}
			finally {
				DocumentModel.EndUpdate();
			}			
		}
		protected void ApplyDefaultOptions(DocumentImporterOptions options) {
			DocumentImporterOptions defaultOptions = DocumentModel.DocumentImportOptions.GetOptions(options.Format);
			if (defaultOptions != null)
				options.CopyFrom(defaultOptions);
		}
		protected void ApplyDefaultOptions(DocumentExporterOptions options) {
			DocumentExporterOptions defaultOptions = DocumentModel.DocumentExportOptions.GetOptions(options.Format);
			if (defaultOptions != null)
				options.CopyFrom(defaultOptions);
		}
		#region Rtf
		public string GetDocumentRtfContent() {
			return GetDocumentRtfContent(null);
		}
		public string GetDocumentRtfContent(RtfDocumentExporterOptions options) {
			return GetDocumentRtfContent(options, false, false, false);
		}
		public string GetDocumentRtfContent(RtfDocumentExporterOptions options, bool lastParagraphRunNotSelected, bool forceRaiseBeforeExport, bool forceRaiseAfterExport) {
			return GetDocumentRtfContent(options, lastParagraphRunNotSelected, false, forceRaiseBeforeExport, forceRaiseAfterExport);
		}
		internal string GetDocumentRtfContent(RtfDocumentExporterOptions options, bool lastParagraphRunNotSelected, bool keepFieldCodeViewState, bool forceRaiseBeforeExport, bool forceRaiseAfterExport) {
			return GetDocumentRtfContentCore(exporter => exporter.Export(), options, lastParagraphRunNotSelected,keepFieldCodeViewState, forceRaiseBeforeExport, forceRaiseAfterExport);
		}
		ChunkedStringBuilder GetDocumentRtfContentSaveMemory(RtfDocumentExporterOptions options) {
			return GetDocumentRtfContentSaveMemory(options, false, false, false);
		}
		ChunkedStringBuilder GetDocumentRtfContentSaveMemory(RtfDocumentExporterOptions options, bool lastParagraphRunNotSelected, bool forceRaiseBeforeExport, bool forceRaiseAfterExport) {
			return GetDocumentRtfContentCore(exporter => exporter.ExportSaveMemory(), options, lastParagraphRunNotSelected, false, forceRaiseBeforeExport, forceRaiseAfterExport);
		}
		public T GetDocumentRtfContentCore<T>(Function<T, IRtfExporter> exportFunc, RtfDocumentExporterOptions options, bool lastParagraphRunNotSelected, bool keepFieldCodeViewState, bool forceRaiseBeforeExport, bool forceRaiseAfterExport) {
			DocumentModel modelForExport = PrepareModelForExport(DocumentFormat.Rtf);
			forceRaiseBeforeExport = (options == null);
			forceRaiseAfterExport = (options == null);
			if (options == null) {
				options = new RtfDocumentExporterOptions();
				ApplyDefaultOptions(options);
			}
			if (forceRaiseBeforeExport)
				modelForExport.RaiseBeforeExport(DocumentFormat.Rtf, options);
			IRtfExporter exporter = exportersFactory.CreateRtfExporter(modelForExport, options);
			exporter.LastParagraphRunNotSelected = lastParagraphRunNotSelected;
			exporter.KeepFieldCodeViewState = keepFieldCodeViewState;
			T result = exportFunc(exporter);
			if (forceRaiseAfterExport)
				modelForExport.RaiseAfterExport();
			return result;
		}
		public void SetDocumentRtfContent(string rtfContent) {
			RtfDocumentImporterOptions options = new RtfDocumentImporterOptions();
			ApplyDefaultOptions(options);
			DocumentModel.RaiseBeforeImport(DocumentFormat.Rtf, options);
			SetDocumentRtfContent(rtfContent, options);
		}
		public void SetDocumentRtfContent(string rtfContent, RtfDocumentImporterOptions options) {
			byte[] bytes = options.ActualEncoding.GetBytes(rtfContent);
			using (MemoryStream stream = new MemoryStream(bytes)) {
				LoadDocumentRtfContent(stream, options);
			}
		}
		public void SaveDocumentRtfContent(Stream stream, RtfDocumentExporterOptions options) {
			DocumentModel.RaiseBeforeExport(DocumentFormat.Rtf, options);
			ChunkedStringBuilder content = GetDocumentRtfContentSaveMemory(options);
			DocumentModel.RaiseAfterExport();
			stream.Write(content, options.ActualEncoding);
		}
		public void LoadDocumentRtfContent(Stream stream, RtfDocumentImporterOptions options) {
			DocumentModel.RaiseBeforeImport(DocumentFormat.Rtf, options);
			DocumentModel.BeginUpdate();
			try {
				try {
					using (IRtfImporter importer = importersFactory.CreateRtfImporter(DocumentModel, options)) {
						importer.Import(stream);
					}
				}
				catch (Exception e) {
					DocumentModel.RaiseInvalidFormatException(e);
					CreateNewDocument();
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#region PlainText
		public string GetDocumentPlainTextContent() {
			return GetDocumentPlainTextContent(null);
		}
		public string GetDocumentPlainTextContent(PlainTextDocumentExporterOptions options) {
			return PreparePlainTextExport(exporter => exporter.Export(), options);
		}
		StringBuilder GetDocumentPlainTextContentSaveMemory(PlainTextDocumentExporterOptions options) {
			return PreparePlainTextExport(exporter => exporter.ExportSaveMemory(), options);
		}
		T PreparePlainTextExport<T>(Function<T, IPlainTextExporter> exportFunc, PlainTextDocumentExporterOptions options) {
			DocumentModel modelForExport = PrepareModelForExport(DocumentFormat.PlainText);
			bool raiseEvents = options == null;
			if (raiseEvents) {
				options = new PlainTextDocumentExporterOptions();
				ApplyDefaultOptions(options);
				modelForExport.RaiseBeforeExport(DocumentFormat.PlainText, options);
			}
			IPlainTextExporter exporter = exportersFactory.CreatePlainTextExporter(modelForExport, options);
			T result = exportFunc(exporter);
			if (raiseEvents)
				modelForExport.RaiseAfterExport();
			return result;
		}
		public void SetDocumentPlainTextContent(string plainTextContent) {
			PlainTextDocumentImporterOptions options = new PlainTextDocumentImporterOptions();
			ApplyDefaultOptions(options);
			DocumentModel.RaiseBeforeImport(DocumentFormat.PlainText, options);
			SetDocumentPlainTextContent(plainTextContent, options);
		}
		public void SetDocumentPlainTextContent(string plainTextContent, PlainTextDocumentImporterOptions options) {
			SetDocumentPlainTextContentCore(plainTextContent, DocumentModelChangeType.LoadNewDocument);
		}
		protected void SetDocumentPlainTextContentCore(string plainTextContent, DocumentModelChangeType changeType) {
			DocumentModel.BeginSetContent();
			try {
				if (!DocumentModel.DocumentCapabilities.ParagraphsAllowed)
					plainTextContent = RemoveParagraphs(plainTextContent);
				TextManipulatorHelper.SetTextCore(DocumentModel.MainPieceTable, plainTextContent);
			}
			finally {
				DocumentModel.EndSetContent(changeType, false, null);
			}
		}
		protected internal virtual string RemoveParagraphs(string plainTextContent) {
			string result = plainTextContent.Replace(Characters.SectionMark, Characters.ParagraphMark);
			String[] newLineStrings = new String[] { "\r\n", "\n\r", "\r", "\n" };
			for (int i = 0; i < newLineStrings.Length; i++) {
				result = result.Replace(newLineStrings[i], " ");
			}
			return result;
		}
		public void SaveDocumentPlainTextContent(Stream stream, PlainTextDocumentExporterOptions options) {
			DocumentModel.RaiseBeforeExport(DocumentFormat.PlainText, options);
			StringBuilder content = GetDocumentPlainTextContentSaveMemory(options);
			stream.Write(content, options.ActualEncoding);
			DocumentModel.RaiseAfterExport();
		}
		public void LoadDocumentPlainTextContent(Stream stream, PlainTextDocumentImporterOptions options) {
			DocumentModel.RaiseBeforeImport(DocumentFormat.PlainText, options);
			LoadDocumentPlainTextContentCore(stream, options);
		}
		protected internal void LoadDocumentPlainTextContentCore(Stream stream, PlainTextDocumentImporterOptions options) {
			if (options.AutoDetectEncoding && !options.ShouldSerializeEncoding()) {
				InternalEncodingDetector detector = new InternalEncodingDetector();
				Encoding encoding = detector.Detect(stream);
				if (encoding != null)
					options.Encoding = encoding;
			}
			byte[] bytes = new byte[stream.Length];
			stream.Read(bytes, 0, (int)stream.Length);
			try {
				byte[] preamble = options.Encoding.GetPreamble();
				int offset = ArrayStartsWith(bytes, preamble) ? preamble.Length : 0;
				string content = options.Encoding.GetString(bytes, offset, bytes.Length - offset);
				SetDocumentPlainTextContent(content, options);
			}
			catch (Exception e) {
				DocumentModel.RaiseInvalidFormatException(e);
			}
		}
		bool ArrayStartsWith(byte[] bytes, byte[] preamble) {
			if (preamble == null || preamble.Length <= 0 || preamble.Length > bytes.Length)
				return false;
			int count = preamble.Length;
			for (int i = 0; i < count; i++) {
				if (bytes[i] != preamble[i])
					return false;
			}
			return true;
		}
		#endregion
		#region Html
		public virtual void LoadDocumentHtmlContent(Stream stream, HtmlDocumentImporterOptions options) {
			DocumentModel.RaiseBeforeImport(DocumentFormat.Html, options);
			try {
				IHtmlImporter importer = ImporterFactory.CreateHtmlImporter(DocumentModel, options);
				importer.Import(stream);
			}
			catch (Exception e) {
				DocumentModel.RaiseInvalidFormatException(e);
			}
		}
		public virtual void SaveDocumentHtmlContent(Stream stream, HtmlDocumentExporterOptions options) {
			DocumentModel.RaiseBeforeExport(DocumentFormat.Html, options);
			IHtmlExporter exporter = PrepareHtmlExport(options);
			StreamWriter writer = new StreamWriter(stream, options.Encoding);
			exporter.Export(writer);
			DocumentModel.RaiseAfterExport();
		}
		public string GetDocumentHtmlContent(IUriProvider provider) {
			return GetDocumentHtmlContent(provider, null);
		}
		public string GetDocumentHtmlContent(IUriProvider provider, HtmlDocumentExporterOptions options) {
			bool raiseEvents = options == null;
			if (raiseEvents) {
				options = new HtmlDocumentExporterOptions();
				ApplyDefaultOptions(options);
				options.CssPropertiesExportType = CssPropertiesExportType.Style;
				DocumentModel.RaiseBeforeExport(DocumentFormat.Html, options);
			}
			if (provider == null)
				provider = new DevExpress.Office.Services.Implementation.DataStringUriProvider();
			IUriProviderService service = DocumentModel.GetService<IUriProviderService>();
			if (service != null)
				service.RegisterProvider(provider);
			try {
				return GetDocumentHtmlContent(options);
			}
			finally {
				if (service != null)
					service.UnregisterProvider(provider);
				if (raiseEvents)
					DocumentModel.RaiseAfterExport();
			}
		}
		protected internal string GetDocumentHtmlContent(HtmlDocumentExporterOptions options) {
			IHtmlExporter exporter = PrepareHtmlExport(options);
			return exporter.Export();
		}
		protected internal IHtmlExporter PrepareHtmlExport(HtmlDocumentExporterOptions options) {
			return ExporterFactory.CreateHtmlExporter(PrepareModelForExport(DocumentFormat.Html), options);
		}
		public void SetDocumentHtmlContent(string content) {
			HtmlDocumentImporterOptions options = new HtmlDocumentImporterOptions();
			ApplyDefaultOptions(options);
			options.IgnoreMetaCharset = true;
			options.AutoDetectEncoding = false;
			DocumentModel.RaiseBeforeImport(DocumentFormat.Html, options);
			SetDocumentHtmlContent(content, options);
		}
		public void SetDocumentHtmlContent(string content, HtmlDocumentImporterOptions options) {
			byte[] bytes = options.Encoding.GetBytes(content);
			using (MemoryStream stream = new MemoryStream(bytes)) {
				LoadDocumentHtmlContent(stream, options);
			}
		}
		#endregion
		#region Mht
		public virtual void LoadDocumentMhtContent(Stream stream, MhtDocumentImporterOptions options) {
			DocumentModel.RaiseBeforeImport(DocumentFormat.Mht, options);
			try {
				IMhtImporter importer = ImporterFactory.CreateMhtImporter(DocumentModel, options);
				importer.Import(stream);
			}
			catch (Exception e) {
				DocumentModel.RaiseInvalidFormatException(e);
			}
		}
		public virtual void SaveDocumentMhtContent(Stream stream, MhtDocumentExporterOptions options) {
			DocumentModel.RaiseBeforeExport(DocumentFormat.Mht, options);
			Function<IMhtExporter, IMhtExporter> exportFunc = exporter => {
				StreamWriter writer = new StreamWriter(stream, options.Encoding);
				exporter.Export(writer);
				return exporter;
			};
			PrepareMhtExport(exportFunc, options);
			DocumentModel.RaiseAfterExport();
		}
		public string GetDocumentMhtContent() {
			return GetDocumentMhtContent(null);
		}
		protected internal string GetDocumentMhtContent(MhtDocumentExporterOptions options) {
			return PrepareMhtExport(exporter => exporter.Export(), options);
		}
		T PrepareMhtExport<T>(Function<T, IMhtExporter> exportFunc, MhtDocumentExporterOptions options) {
			DocumentModel modelForExport = PrepareModelForExport(DocumentFormat.Mht);
			bool raiseEvents = options == null;
			if (raiseEvents) {
				options = new MhtDocumentExporterOptions();
				ApplyDefaultOptions(options);
				modelForExport.RaiseBeforeExport(DocumentFormat.Mht, options);
			}
			IMhtExporter exporter = ExporterFactory.CreateMhtExporter(modelForExport, options);
			T result = exportFunc(exporter);
			if (raiseEvents)
				modelForExport.RaiseAfterExport();
			return result;
		}
		public void SetDocumentMhtContent(string content) {
			MhtDocumentImporterOptions options = new MhtDocumentImporterOptions();
			ApplyDefaultOptions(options);
			options.Encoding = EmptyEncoding.Instance;
			DocumentModel.RaiseBeforeImport(DocumentFormat.Mht, options);
			SetDocumentMhtContent(content, options);
		}
		public void SetDocumentMhtContent(string content, MhtDocumentImporterOptions options) {
			byte[] bytes = options.Encoding.GetBytes(content);
			using (MemoryStream stream = new MemoryStream(bytes)) {
				LoadDocumentMhtContent(stream, options);
			}
		}
		#endregion
		#region OpenXml
		public virtual void LoadDocumentOpenXmlContent(Stream stream, OpenXmlDocumentImporterOptions options) {
			DocumentModel.RaiseBeforeImport(DocumentFormat.OpenXml, options);
			try {
				IOpenXmlImporter importer = ImporterFactory.CreateOpenXmlImporter(DocumentModel, options);
				importer.Import(stream);
			}
			catch (Exception e) {
				DocumentModel.RaiseInvalidFormatException(e);
			}
		}
		public virtual void SaveDocumentOpenXmlContent(Stream stream, OpenXmlDocumentExporterOptions options) {
			DocumentModel.RaiseBeforeExport(DocumentFormat.OpenXml, options);
			IOpenXmlExporter exporter = ExporterFactory.CreateOpenXmlExporter(DocumentModel, options);
			exporter.Export(stream);
			DocumentModel.RaiseAfterExport();
		}
		public byte[] GetDocumentOpenXmlContent() {
			return GetDocumentOpenXmlContent(null);
		}
		protected internal byte[] GetDocumentOpenXmlContent(OpenXmlDocumentExporterOptions options) {
			DocumentModel modelForExport = PrepareModelForExport(DocumentFormat.OpenXml);
			bool raiseEvents = options == null;
			if (raiseEvents) {
				options = new OpenXmlDocumentExporterOptions();
				ApplyDefaultOptions(options);
				modelForExport.RaiseBeforeExport(DocumentFormat.OpenXml, options);
			}
			IOpenXmlExporter exporter = ExporterFactory.CreateOpenXmlExporter(modelForExport, options);
			try {
				using (MemoryStream stream = new MemoryStream()) {
					exporter.Export(stream);
					return stream.ToArray();
				}
			}
			finally {
				if (raiseEvents)
					modelForExport.RaiseAfterExport();
			}
		}
		public void SetDocumentOpenXmlContent(byte[] content) {
			OpenXmlDocumentImporterOptions options = new OpenXmlDocumentImporterOptions();
			ApplyDefaultOptions(options);
			DocumentModel.RaiseBeforeImport(DocumentFormat.OpenXml, options);
			SetDocumentOpenXmlContent(content, options);
		}
		public void SetDocumentOpenXmlContent(byte[] content, OpenXmlDocumentImporterOptions options) {
			using (MemoryStream stream = new MemoryStream(content)) {
				LoadDocumentOpenXmlContent(stream, options);
			}
		}
		#endregion
		#region WordML
		public string GetDocumentWordMLContent() {
			return GetDocumentWordMLContent(null);
		}
		public string GetDocumentWordMLContent(WordMLDocumentExporterOptions options) {
			return PrepareWordMLExport(exporter => exporter.Export(), options);
		}
		T PrepareWordMLExport<T>(Function<T, IWordMLExporter> exportFunc, WordMLDocumentExporterOptions options) {
			DocumentModel modelForExport = PrepareModelForExport(DocumentFormat.WordML);
			bool raiseEvents = options == null;
			if (raiseEvents) {
				options = new WordMLDocumentExporterOptions();
				ApplyDefaultOptions(options);
				modelForExport.RaiseBeforeExport(DocumentFormat.WordML, options);
			}
			IWordMLExporter exporter = ExporterFactory.CreateWordMLExporter(modelForExport, options);
			T result = exportFunc(exporter);
			if (raiseEvents)
				modelForExport.RaiseAfterExport();
			return result;
		}
		public void SetDocumentWordMLContent(string content) {
			WordMLDocumentImporterOptions options = new WordMLDocumentImporterOptions();
			ApplyDefaultOptions(options);
			DocumentModel.RaiseBeforeImport(DocumentFormat.WordML, options);
			SetDocumentWordMLContent(content, options);
		}
		public void SetDocumentWordMLContent(string content, WordMLDocumentImporterOptions options) {
			byte[] bytes = options.ActualEncoding.GetBytes(content);
			using (MemoryStream stream = new MemoryStream(bytes)) {
				LoadDocumentWordMLContent(stream, options);
			}
		}
		public void SaveDocumentWordMLContent(Stream stream, WordMLDocumentExporterOptions options) {
			DocumentModel.RaiseBeforeExport(DocumentFormat.WordML, options);
			Function<IWordMLExporter, IWordMLExporter> exportFunc = exporter => {
				exporter.Export(stream);
				return exporter;
			};
			PrepareWordMLExport(exportFunc, options);
			DocumentModel.RaiseAfterExport();
		}
		public void LoadDocumentWordMLContent(Stream stream, WordMLDocumentImporterOptions options) {
			DocumentModel.RaiseBeforeImport(DocumentFormat.WordML, options);
			try {
				IWordMLImporter importer = ImporterFactory.CreateWordMLImporter(DocumentModel, options);
				importer.Import(stream);
			}
			catch (Exception e) {
				DocumentModel.RaiseInvalidFormatException(e);
				CreateNewDocument();
			}
		}
		#endregion
		#region OpenDocument
		public virtual void LoadDocumentOpenDocumentContent(Stream stream, OpenDocumentImporterOptions options) {
			DocumentModel.RaiseBeforeImport(DocumentFormat.OpenDocument, options);
			try {
				IOpenDocumentTextImporter importer = ImporterFactory.CreateOpenDocumentTextImporter(DocumentModel, options);
				importer.Import(stream);
			}
			catch (Exception e) {
				DocumentModel.RaiseInvalidFormatException(e);
			}
		}
		public virtual void SaveDocumentOpenDocumentContent(Stream stream, OpenDocumentExporterOptions options) {
			DocumentModel.RaiseBeforeExport(DocumentFormat.OpenDocument, options);
			IOpenDocumentTextExporter exporter = ExporterFactory.CreateOpenDocumentTextExporter(DocumentModel, options);
			exporter.Export(stream);
			DocumentModel.RaiseAfterExport();
		}
		public byte[] GetDocumentOpenDocumentContent() {
			OpenDocumentExporterOptions options = new OpenDocumentExporterOptions();
			ApplyDefaultOptions(options);
			DocumentModel.RaiseBeforeExport(DocumentFormat.OpenDocument, options);
			return GetDocumentOpenDocumentContent(options);
		}
		protected internal byte[] GetDocumentOpenDocumentContent(OpenDocumentExporterOptions options) {
			IOpenDocumentTextExporter exporter = ExporterFactory.CreateOpenDocumentTextExporter(PrepareModelForExport(DocumentFormat.OpenDocument), options);
			using (MemoryStream stream = new MemoryStream()) {
				exporter.Export(stream);
				return stream.ToArray();
			}
		}
		public void SetDocumentOpenDocumentContent(byte[] content) {
			OpenDocumentImporterOptions options = new OpenDocumentImporterOptions();
			ApplyDefaultOptions(options);
			DocumentModel.RaiseBeforeImport(DocumentFormat.OpenDocument, options);
			SetDocumentOpenDocumentContent(content, options);
		}
		public void SetDocumentOpenDocumentContent(byte[] content, OpenDocumentImporterOptions options) {
			using (MemoryStream stream = new MemoryStream(content)) {
				LoadDocumentOpenDocumentContent(stream, options);
			}
		}
		#endregion
		#region Xaml
		public string GetDocumentXamlContent() {
			XamlDocumentExporterOptions options = new XamlDocumentExporterOptions();
			ApplyDefaultOptions(options);
			DocumentModel.RaiseBeforeExport(DocumentFormat.Xaml, options);
			string result = GetDocumentXamlContent(options);
			DocumentModel.RaiseAfterExport();
			return result;
		}
		public string GetDocumentXamlContent(XamlDocumentExporterOptions options) {
			IXamlExporter exporter = PrepareXamlExport(options);
			return exporter.Export();
		}
		IXamlExporter PrepareXamlExport(XamlDocumentExporterOptions options) {
			return ExporterFactory.CreateXamlExporter(PrepareModelForExport(DocumentFormat.Xaml), options);
		}
		public void SetDocumentXamlContent(string XamlContent) {
			XamlDocumentImporterOptions options = new XamlDocumentImporterOptions();
			ApplyDefaultOptions(options);
			DocumentModel.RaiseBeforeImport(DocumentFormat.Xaml, options);
			SetDocumentXamlContent(XamlContent, options);
		}
		public void SetDocumentXamlContent(string XamlContent, XamlDocumentImporterOptions options) {
			byte[] bytes = options.ActualEncoding.GetBytes(XamlContent);
			using (MemoryStream stream = new MemoryStream(bytes)) {
				LoadDocumentXamlContent(stream, options);
			}
		}
		public void SaveDocumentXamlContent(Stream stream, XamlDocumentExporterOptions options) {
			DocumentModel.RaiseBeforeExport(DocumentFormat.Xaml, options);
			IXamlExporter exporter = PrepareXamlExport(options);
			exporter.Export(stream);
			DocumentModel.RaiseAfterExport();
		}
		public void LoadDocumentXamlContent(Stream stream, XamlDocumentImporterOptions options) {
			DocumentModel.RaiseBeforeImport(DocumentFormat.Xaml, options);
			try {
				IXamlImporter importer = ImporterFactory.CreateXamlImporter(DocumentModel, options);
				importer.Import(stream);
			}
			catch (Exception e) {
				DocumentModel.RaiseInvalidFormatException(e);
				CreateNewDocument();
			}
		}
		#endregion
		#region Doc
		public virtual void LoadDocumentDocContent(Stream stream, DocDocumentImporterOptions options) {
			DocumentModel.RaiseBeforeImport(DocumentFormat.Doc, options);
			try {
				IDocImporter importer = ImporterFactory.CreateDocImporter(DocumentModel, options);
				importer.Import(stream);
			}
			catch (Exception e) {
				DocumentModel.RaiseInvalidFormatException(e);
			}
		}
		public virtual void SaveDocumentDocContent(Stream stream, DocDocumentExporterOptions options) {
			DocumentModel.RaiseBeforeExport(DocumentFormat.Doc, options);
			IDocExporter exporter = ExporterFactory.CreateDocExporter(DocumentModel, options);
			exporter.Export(stream);
			DocumentModel.RaiseAfterExport();
		}
		public byte[] GetDocumentDocContent() {
			return GetDocumentDocContent(null);
		}
		protected internal byte[] GetDocumentDocContent(DocDocumentExporterOptions options) {
			DocumentModel modelForExport = PrepareModelForExport(DocumentFormat.Doc);
			bool raiseEvents = options == null;
			if (raiseEvents) {
				options = new DocDocumentExporterOptions();
				ApplyDefaultOptions(options);
				modelForExport.RaiseBeforeExport(DocumentFormat.Doc, options);
			}
			IDocExporter exporter = ExporterFactory.CreateDocExporter(modelForExport, options);
			try {
				using (MemoryStream stream = new MemoryStream()) {
					exporter.Export(stream);
					return stream.ToArray();
				}
			}
			finally {
				if (raiseEvents)
					modelForExport.RaiseAfterExport();
			}
		}
		public void SetDocumentDocContent(byte[] content) {
			DocDocumentImporterOptions options = new DocDocumentImporterOptions();
			ApplyDefaultOptions(options);
			DocumentModel.RaiseBeforeImport(DocumentFormat.Doc, options);
			SetDocumentDocContent(content, options);
		}
		public void SetDocumentDocContent(byte[] content, DocDocumentImporterOptions options) {
			using (MemoryStream stream = new MemoryStream(content)) {
				LoadDocumentDocContent(stream, options);
			}
		}
		#endregion
		DocumentModel PrepareModelForExport(DocumentFormat format) {
			if (!DocumentModel.SeparateModelForApiExport)
				return DocumentModel;
			DocumentModel modelForExport = DocumentModel.ModelForExport ? DocumentModel : DocumentModel.CreateDocumentModelForExport(model => { });
			modelForExport.PreprocessContentBeforeExport(format);
			return modelForExport;
		}
		protected internal virtual void OnSectionInserted(object sender, SectionEventArgs e) {
			RaiseSectionInserted(e);
		}
		protected internal virtual void OnSectionRemoved(object sender, SectionEventArgs e) {
			RaiseSectionRemoved(e);
		}
		#region IDocumentModelStructureChangedListener Members
		void IDocumentModelStructureChangedListener.OnParagraphInserted(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
			Action<DocumentModelPositionAnchor> anchorAction = delegate(DocumentModelPositionAnchor anchor) {
				DocumentModelStructureChangedNotifier.NotifyParagraphInserted(anchor, pieceTable, sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
			};
			UpdateAnchors(anchorAction);
			RaiseParagraphInserted(pieceTable, sectionIndex, actualParagraphIndex);
		}
		void IDocumentModelStructureChangedListener.OnParagraphRemoved(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			Action<DocumentModelPositionAnchor> anchorAction = delegate(DocumentModelPositionAnchor anchor) {
				DocumentModelStructureChangedNotifier.NotifyParagraphRemoved(anchor, pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			};
			UpdateAnchors(anchorAction);
			RaiseParagraphRemoved(pieceTable, sectionIndex, paragraphIndex);
		}
		void IDocumentModelStructureChangedListener.OnParagraphMerged(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			Action<DocumentModelPositionAnchor> anchorAction = delegate(DocumentModelPositionAnchor anchor) {
				DocumentModelStructureChangedNotifier.NotifyParagraphMerged(anchor, pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			};
			UpdateAnchors(anchorAction);
			RaiseParagraphMerged(pieceTable, sectionIndex, paragraphIndex);
		}
		void IDocumentModelStructureChangedListener.OnRunInserted(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			Action<DocumentModelPositionAnchor> anchorAction = delegate(DocumentModelPositionAnchor anchor) {
				DocumentModelStructureChangedNotifier.NotifyRunInserted(anchor, pieceTable, paragraphIndex, newRunIndex, length, historyNotificationId);
			};
			UpdateAnchors(anchorAction);
		}
		void IDocumentModelStructureChangedListener.OnRunRemoved(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			Action<DocumentModelPositionAnchor> anchorAction = delegate(DocumentModelPositionAnchor anchor) {
				DocumentModelStructureChangedNotifier.NotifyRunRemoved(anchor, pieceTable, paragraphIndex, runIndex, length, historyNotificationId);
			};
			UpdateAnchors(anchorAction);
		}
		void IDocumentModelStructureChangedListener.OnBeginMultipleRunSplit(PieceTable pieceTable) {
		}
		void IDocumentModelStructureChangedListener.OnEndMultipleRunSplit(PieceTable pieceTable) {
		}
		void IDocumentModelStructureChangedListener.OnRunSplit(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			Action<DocumentModelPositionAnchor> anchorAction = delegate(DocumentModelPositionAnchor anchor) {
				DocumentModelStructureChangedNotifier.NotifyRunSplit(anchor, pieceTable, paragraphIndex, runIndex, splitOffset);
			};
			UpdateAnchors(anchorAction);
		}
		void IDocumentModelStructureChangedListener.OnRunJoined(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
			Action<DocumentModelPositionAnchor> anchorAction = delegate(DocumentModelPositionAnchor anchor) {
				DocumentModelStructureChangedNotifier.NotifyRunJoined(anchor, pieceTable, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
			};
			UpdateAnchors(anchorAction);
		}
		void IDocumentModelStructureChangedListener.OnRunMerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			Action<DocumentModelPositionAnchor> anchorAction = delegate(DocumentModelPositionAnchor anchor) {
				DocumentModelStructureChangedNotifier.NotifyRunMerged(anchor, pieceTable, paragraphIndex, runIndex, deltaRunLength);
			};
			UpdateAnchors(anchorAction);
		}
		void IDocumentModelStructureChangedListener.OnRunUnmerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			Action<DocumentModelPositionAnchor> anchorAction = delegate(DocumentModelPositionAnchor anchor) {
				DocumentModelStructureChangedNotifier.NotifyRunUnmerged(anchor, pieceTable, paragraphIndex, runIndex, deltaRunLength);
			};
			UpdateAnchors(anchorAction);
		}
		void IDocumentModelStructureChangedListener.OnFieldInserted(PieceTable pieceTable, int fieldIndex) {
			RaiseFieldInserted(pieceTable, fieldIndex);
		}
		void IDocumentModelStructureChangedListener.OnFieldRemoved(PieceTable pieceTable, int fieldIndex) {
			RaiseFieldRemoved(pieceTable, fieldIndex);
		}
		#endregion
		public void RegisterModifier(IDocumentModelModifier modifier) {
			modifier.Changed += OnDocumentModelChangedByModifier;
		}
		public void UnregisterModifier(IDocumentModelModifier modifier) {
			modifier.Changed -= OnDocumentModelChangedByModifier;
		}
		protected internal virtual void OnDocumentModelChangedByModifier(object sender, EventArgs e) {
			IDocumentModelModifier modifier = (IDocumentModelModifier)sender;
			int count = activeModifiers.Count;
			for (int i = 0; i < count; i++) {
				if (!Object.ReferenceEquals(activeModifiers[i], modifier))
					activeModifiers[i].ResetCachedValues();
			}
		}
	}
	#endregion
	public interface IDocumentModelModifier {
		event EventHandler Changed;
		void ResetCachedValues();
	}
}
