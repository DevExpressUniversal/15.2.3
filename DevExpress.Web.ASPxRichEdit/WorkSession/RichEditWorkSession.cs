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

using DevExpress.Office.Utils;
using DevExpress.Web.ASPxRichEdit.Export;
using DevExpress.Web.Office.Internal;
using DevExpress.Web.Office;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using DevExpress.Web.Internal;
using System.Collections;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	[AttributeUsage(AttributeTargets.Assembly)]
	public class RichEditWorkSessionRegistration : WorkSessionRegistrationAttributeBase {
		protected override void RegisterWorkSessionFactory() {
			RichEditWorkSession.RegisterInFactory();
		}
	}
	public class RichEditWorkSession : WorkSessionBase<DocumentCapabilitiesOptions> {
		const string WorkSessionDocumentTypeName = "richedit";
		internal static void RegisterInFactory() {
			WorkSessionFactories.Register(WorkSessionDocumentTypeName, id => new RichEditWorkSession(new DocumentContentContainer(null), id, new DocumentCapabilitiesOptions()));
		}
		static RichEditWorkSession() {
			RegisterInFactory();
		}
		InternalRichEditDocumentServer richEdit;
		internal RichEditWorkSession(DocumentContentContainer documentContentContainer, Guid id, DocumentCapabilitiesOptions settings)
			: base(documentContentContainer, id, settings) {
				Initialize();
		}
		private void Initialize() {
			FontInfoCache = new WebFontInfoCache();
			Clients = new ConcurrentDictionary<Guid, WorkSessionClient>();
		}
		protected internal new void RefreshLastModifyTime() {
			base.RefreshLastModifyTime();
		}
		protected override bool DocumentControlExists { get { return this.richEdit != null; } }
		protected internal InternalRichEditDocumentServer RichEdit {
			get {
				OnBeforeDocumentAccess();
				return richEdit;
			}
			set {
				richEdit = value;
			}
		}
		protected internal WebFontInfoCache FontInfoCache { get; protected set; }
		protected internal Guid EditorClientGuid { get; set; }
		protected internal ConcurrentDictionary<Guid, WorkSessionClient> Clients { get; private set; }
		protected internal AbstractNumberingList[] NumberingListTemplates { get; private set; }
		protected override bool GetModified() {
			return RichEdit.Modified;
		}
		protected override void SetModified(bool value) {
			RichEdit.Modified = value;
			OnModifiedChanged(value); 
		}
		protected internal virtual string CurrentFileName {
			get { return RichEdit.Options.DocumentSaveOptions.CurrentFileName; }
		}
		protected void PrepareRichEdit() {
			RichEdit.Options.DocumentCapabilities.FootNotes = DocumentCapability.Disabled; 
			RichEdit.Options.DocumentCapabilities.EndNotes = DocumentCapability.Disabled; 
			RichEdit.Options.DocumentCapabilities.Comments = DocumentCapability.Disabled; 
			RichEdit.Options.DocumentCapabilities.FloatingObjects = DocumentCapability.Disabled; 
			RichEdit.Options.Printing.UpdateDocVariablesBeforePrint = false;
			RichEdit.Options.Export.Html.EmbedImages = true;
			RichEdit.Options.Export.Html.CssPropertiesExportType = XtraRichEdit.Export.Html.CssPropertiesExportType.Inline;
		}
		protected void PrepareDocumentModel() {
			RichEdit.Model.DocumentModel.SwitchToEmptyHistory(true);
			RichEdit.Model.DocumentModel.LastExecutedEditCommandId = 0;
#if DEBUGTEST
			RichEdit.Model.DocumentModel.DisableCheckIntegrityOnLastEndUpdate = true;
#endif
			NumberingListTemplates = DefaultNumberingListHelper.GetNumberingLists(RichEdit.Model.DocumentModel, RichEdit.Model.DocumentModel.UnitConverter, RichEdit.Model.DocumentModel.DocumentProperties.DefaultTabWidth);
		}
		int emptyImageCacheKey = -1;
		protected internal int EmptyImageCacheID {
			get {
				if(emptyImageCacheKey < 0) {
					OfficeImage emptyImage = OfficeImage.CreateImage(new System.Drawing.Bitmap(1, 1));
					emptyImageCacheKey = RichEdit.DocumentModel.CreateImage(emptyImage.GetImageBytesStream(OfficeImageFormat.Png)).ImageCacheKey;
				}
				return emptyImageCacheKey;
			}
		}
		protected override void CreateNewDocument() {
			RefreshLastTimeActivity();
			RichEdit.CreateNewDocument();
			OnNewDocumentCreated();
		}
		protected DocumentFormat ParseDocumentFormat(DocumentContentContainer documentContentContainer) {
			string formatName = documentContentContainer.FormatName;
			var documentFormat = ParseDocumentFormatName(formatName);
			if(documentFormat != DocumentFormat.Undefined)
				return documentFormat;
			else if(documentContentContainer.IsFile)
				return RichEdit.DocumentModel.AutodetectDocumentFormat(documentContentContainer.PathOrID);
			return DocumentFormat.Undefined;
		}
		protected DocumentFormat ParseDocumentFormatName(string formatName) {
			int formatValue;
			if(int.TryParse(formatName, out formatValue))
				return new DocumentFormat(formatValue);
			return DocumentFormat.Undefined;
		}
		protected override void LoadDocument(DocumentContentContainer documentContentContainer) {
			RefreshLastTimeActivity();
			DocumentFormat format = ParseDocumentFormat(documentContentContainer);
			if(documentContentContainer.IsFile)
				RichEdit.LoadDocument(documentContentContainer.PathOrID, format);
			else if(documentContentContainer.IsStream)
				RichEdit.LoadDocument(documentContentContainer.Stream, format);
			else if(documentContentContainer.IsArray) {
				using(Stream stream = new MemoryStream(documentContentContainer.Array))
					RichEdit.LoadDocument(stream, format);
			}
			OnNewDocumentLoaded(documentContentContainer);
		}
		protected void LoadDocument(HibernationChamber hibernationChamber) {
			var format = ParseDocumentFormatName(hibernationChamber.FormatName);
			using(Stream stream = new MemoryStream(hibernationChamber.Data))
				RichEdit.LoadDocument(stream, format);
		}
		protected override DocumentHandlerResponse ProcessRequestCore(NameValueCollection nameValueCollection) {
			var clientGuid = Guid.Parse(nameValueCollection["c"]);
			if(!Clients.ContainsKey(clientGuid))
				throw new UnauthorizedAccessException("ClientGuid is unknown");
			if(nameValueCollection.AllKeys.Contains("img")) {
				int imageID = 0;
				if(int.TryParse(nameValueCollection["img"], out imageID)) {
					var result = new BinaryDocumentHandlerResponse();
					var image = RichEdit.DocumentModel.ImageCache.GetImageByKey(imageID);
					result.BinaryBuffer = image.GetImageBytes(OfficeImageFormat.Png);
					result.ContentType = "image/png";
					return result;
				}
			}
			if(nameValueCollection.AllKeys.Contains("downloadRequestType"))
				return RichEditDownloadManager.GetResponse(this, nameValueCollection);
			return CommandFactory.ExecuteCommands(this, clientGuid, null, nameValueCollection);
		}
		protected override void CloseCore() {
			if(this.richEdit != null) {
				RichEdit.Dispose();
				this.richEdit = null;
			}
		}
		protected override WorkSessionBase Clone(Guid newWorkSessionID, string documentPathOrID) {
			RefreshLastTimeActivity();
			var documentFormat = CalculateDocumentFormat();
			using(Stream stream = new MemoryStream()) {
				RichEdit.SaveDocument(stream, documentFormat);
				stream.Position = 0;
				DocumentContentContainer tempDocumentContentContainer = new DocumentContentContainer(stream, documentFormat.ToString(), documentPathOrID);
				RichEditWorkSession clone = new RichEditWorkSession(tempDocumentContentContainer, newWorkSessionID, RichEdit.Options.DocumentCapabilities);
				clone.FontInfoCache.CopyFrom(FontInfoCache);
				return clone;
			}
		}
		protected override void SaveAs(DocumentContentContainer documentContentContainer) {
			RefreshLastTimeActivity();
			DocumentFormat format = ParseDocumentFormat(documentContentContainer);
			if(format == DocumentFormat.Undefined)
				format = CalculateDocumentFormat();
			if(documentContentContainer.IsFile)
				RichEdit.SaveDocument(documentContentContainer.PathOrID, format);
			else if(documentContentContainer.IsArray) {
				using(Stream stream = new MemoryStream()) {
					RichEdit.SaveDocument(stream, format);
					stream.Position = 0;
					documentContentContainer.Array = DevExpress.Web.Internal.CommonUtils.GetBytesFromStream(stream);
				}
			}
			else if(documentContentContainer.IsStream)
				RichEdit.SaveDocument(documentContentContainer.Stream, format);
		}
		public DocumentFormat CalculateDocumentFormat() {
			var defaultFormat = RichEdit.Options.DocumentSaveOptions.DefaultFormat;
			var currentFormat = RichEdit.Options.DocumentSaveOptions.CurrentFormat;
			return currentFormat != DocumentFormat.Undefined ? currentFormat : defaultFormat;
		}
		public void AttachClient(Guid clientGuid, string workDirectory, bool readOnly, RichEditBehaviorOptions behavior, DocumentCapabilitiesOptions documentCapabilities) {
			var client = new WorkSessionClient(workDirectory, readOnly, behavior, documentCapabilities);
			AttachClient(clientGuid, client);
		}
		public void AttachClient(Guid clientGuid, WorkSessionClient client) {
			Clients.AddOrUpdate(clientGuid, client, (id, exSettings) => client);
		}
		public void DetachClient(Guid clientGuid) {
			WorkSessionClient settings;
			Clients.TryRemove(clientGuid, out settings);
		}
		public bool HasClient(Guid clientGuid) { return Clients.ContainsKey(clientGuid); }
		public WorkSessionClient GetClient(Guid clientGuid) {
			return Clients[clientGuid];
		}
		protected override void CreateModelCore(DocumentContentContainer documentContentContainer, DocumentCapabilitiesOptions settings) {
			RichEdit = new ASPxInternalRichEditDocumentServer();
			RichEdit.Options.DocumentCapabilities.CopyFrom(settings);
			PrepareRichEdit();
			if(documentContentContainer.IsEmpty)
				CreateNewDocument();
			else
				LoadDocument(documentContentContainer);
			PrepareDocumentModel();
		}
		protected override IDocumentInfo GetCreateDocumentInfo() {
			return new RichEditDocumentInfo(this);
		}
		protected override string GetCurrentDocumentFilePath() {
			return RichEdit.DocumentModel.DocumentSaveOptions.CurrentFileName;
		}
		protected override void SaveInTheSameFileCore() {
			RichEdit.SaveDocument(GetCurrentDocumentFilePath(), RichEdit.DocumentModel.DocumentSaveOptions.CurrentFormat);
			Modified = false;
		}
		protected override string GetWorkSessionDocumentTypeName() {
			return WorkSessionDocumentTypeName;
		}
		protected override HibernationChamber SaveToHibernationChamber() {
			var documentFormat = DocumentFormat.OpenDocument;
			byte[] array;
			using(Stream stream = new MemoryStream()) {
				RichEdit.SaveDocument(stream, documentFormat);
				stream.Position = 0;
				array = CommonUtils.GetBytesFromStream(stream);
			}
			var hibernationChamber = new HibernationChamber(array, documentFormat.ToString());
			SerializeWorkSession(hibernationChamber.Metadata);
			return hibernationChamber;
		}
		protected override void UnloadDocumentFromMemory() {
			CloseCore();
		}
		protected override bool RestoreFromHibernationChamber(HibernationChamber hibernationChamber) {
			RichEdit = new ASPxInternalRichEditDocumentServer();
			PrepareRichEdit();
			Initialize();
			LoadDocument(hibernationChamber);
			DeserializeWorkSession(hibernationChamber.Metadata);
			PrepareDocumentModel();
			return true;
		}
		protected void SerializeWorkSession(Dictionary<string, string> target) {
			target["fontInfos"] = string.Join(",,", FontInfoCache.GetCustomItems().Select(i => i.Name).ToArray());
			target["editorClientGuid"] = EditorClientGuid.ToString();
			target["modified"] = Modified.ToString();
			target["DocumentPath"] = RichEdit.Options.DocumentSaveOptions.CurrentFileName;
			target["CurrentFormat"] = RichEdit.Options.DocumentSaveOptions.CurrentFormat.ToString();
			var clientsHT = new Hashtable();
			foreach (var client in Clients) {
				var clientHT = new Hashtable();
				clientHT["wd"] = client.Value.WorkDirectory;
				clientHT["ro"] = client.Value.ReadOnly;
				clientHT["bo"] = SerializeBehaviorOptions(client.Value.RichEditBehaviorOptions);
				clientHT["dco"] = SerializeDocumentCapabilitiesOptions(client.Value.DocumentCapabilitiesOptions);
				clientHT["lpids"] = string.Join(",", client.Value.LoadedPieceTableIds);
				clientsHT[client.Key.ToString()] = clientHT;
			}
			target["clients"] = HtmlConvertor.ToJSON(clientsHT, true, false, true);
		}
		protected void DeserializeWorkSession(Dictionary<string, string> source) {
			FontInfoCache = new WebFontInfoCache();
			var customFontInfos = source["fontInfos"].Split(new string[] { ",," }, StringSplitOptions.None);
			foreach (var cfi in customFontInfos) {
				FontInfoCache.AddItem(cfi);
			}
			EditorClientGuid = Guid.Parse(source["editorClientGuid"]);
			var clients = HtmlConvertor.FromJSON<Hashtable>(source["clients"]);
			RichEdit.Options.DocumentSaveOptions.CurrentFileName = source["DocumentPath"];
			RichEdit.Options.DocumentSaveOptions.CurrentFormat = new DocumentFormat(int.Parse(source["CurrentFormat"]));
			Modified = Convert.ToBoolean(source["modified"]);
			foreach (var clientGuid in clients.Keys) {
				var clientHt = (Hashtable)clients[clientGuid];
				var clientWorkingDirectory = (string)clientHt["wd"];
				var clientReadOnly = (bool)clientHt["ro"];
				var clientBehaviorHt = (Hashtable)clientHt["bo"];
				var clientDocumentCapabilitiesHt = (Hashtable)clientHt["dco"];
				WorkSessionClient client = new WorkSessionClient(clientWorkingDirectory, clientReadOnly, DeserializeBehaviorOptions(clientBehaviorHt), DeserializeDocumentCapabilitiesOptions(clientDocumentCapabilitiesHt));
				Clients.AddOrUpdate(Guid.Parse((string)clientGuid), client, (guid, exClient) => client);
			}
		}
		Hashtable SerializeDocumentCapabilitiesOptions(DocumentCapabilitiesOptions options) {
			var ht = new Hashtable();
			ht["Bookmarks"] = (int)options.Bookmarks;
			ht["CharacterFormatting"] = (int)options.CharacterFormatting;
			ht["CharacterStyle"] = (int)options.CharacterStyle;
			ht["Comments"] = (int)options.Comments;
			ht["EndNotes"] = (int)options.EndNotes;
			ht["Fields"] = (int)options.Fields;
			ht["FloatingObjects"] = (int)options.FloatingObjects;
			ht["FootNotes"] = (int)options.FootNotes;
			ht["HeadersFooters"] = (int)options.HeadersFooters;
			ht["Hyperlinks"] = (int)options.Hyperlinks;
			ht["InlinePictures"] = (int)options.InlinePictures;
			ht["NumberingBulleted"] = (int)options.Numbering.Bulleted;
			ht["NumberingMultiLevel"] = (int)options.Numbering.MultiLevel;
			ht["NumberingSimple"] = (int)options.Numbering.Simple;
			ht["ParagraphFormatting"] = (int)options.ParagraphFormatting;
			ht["ParagraphFrames"] = (int)options.ParagraphFrames;
			ht["Paragraphs"] = (int)options.Paragraphs;
			ht["ParagraphStyle"] = (int)options.ParagraphStyle;
			ht["ParagraphTabs"] = (int)options.ParagraphTabs;
			ht["Sections"] = (int)options.Sections;
			ht["TableCellStyle"] = (int)options.TableCellStyle;
			ht["Tables"] = (int)options.Tables;
			ht["TableStyle"] = (int)options.TableStyle;
			ht["TabSymbol"] = (int)options.TabSymbol;
			ht["Undo"] = (int)options.Undo;
			return ht;
		}
		DocumentCapabilitiesOptions DeserializeDocumentCapabilitiesOptions(Hashtable source) {
			var options = new ASPxRichEditDocumentCapabilitiesSettings();
			options.Bookmarks = (DocumentCapability)source["Bookmarks"];
			options.CharacterFormatting = (DocumentCapability)source["CharacterFormatting"];
			options.CharacterStyle = (DocumentCapability)source["CharacterStyle"];
			options.Comments = (DocumentCapability)source["Comments"];
			options.EndNotes = (DocumentCapability)source["EndNotes"];
			options.Fields= (DocumentCapability)source["Fields"];
			options.FloatingObjects = (DocumentCapability)source["FloatingObjects"];
			options.FootNotes = (DocumentCapability)source["FootNotes"];
			options.HeadersFooters = (DocumentCapability)source["HeadersFooters"];
			options.Hyperlinks = (DocumentCapability)source["Hyperlinks"];
			options.InlinePictures = (DocumentCapability)source["InlinePictures"];
			options.Numbering.Bulleted = (DocumentCapability)source["NumberingBulleted"];
			options.Numbering.MultiLevel = (DocumentCapability)source["NumberingMultiLevel"];
			options.Numbering.Simple = (DocumentCapability)source["NumberingSimple"];
			options.ParagraphFormatting = (DocumentCapability)source["ParagraphFormatting"];
			options.ParagraphFrames = (DocumentCapability)source["ParagraphFrames"];
			options.Paragraphs = (DocumentCapability)source["Paragraphs"];
			options.ParagraphStyle = (DocumentCapability)source["ParagraphStyle"];
			options.ParagraphTabs = (DocumentCapability)source["ParagraphTabs"];
			options.Sections = (DocumentCapability)source["Sections"];
			options.TableCellStyle = (DocumentCapability)source["TableCellStyle"];
			options.Tables = (DocumentCapability)source["Tables"];
			options.TableStyle = (DocumentCapability)source["TableStyle"];
			options.TabSymbol = (DocumentCapability)source["TabSymbol"];
			options.Undo = (DocumentCapability)source["Undo"];
			return options;
		}
		Hashtable SerializeBehaviorOptions(RichEditBehaviorOptions options) {
			var ht = new Hashtable();
			ht["Copy"] = (int)options.Copy;
			ht["CreateNew"] = (int)options.CreateNew;
			ht["Cut"] = (int)options.Cut;
			ht["Drag"] = (int)options.Drag;
			ht["Drop"] = (int)options.Drop;
			ht["FontSource"] = (int)options.FontSource;
			ht["ForeColorSource"] = (int)options.ForeColorSource;
			ht["MaxZoomFactor"] = float.IsInfinity(options.MaxZoomFactor) ? "Infinity" : options.MaxZoomFactor.ToString();
			ht["MinZoomFactor"] = options.MinZoomFactor;
			ht["OfficeScrolling"] = (int)options.OfficeScrolling;
			ht["Open"] = (int)options.Open;
			ht["OvertypeAllowed"] = options.OvertypeAllowed;
			ht["PageBreakInsertMode"] = (int)options.PageBreakInsertMode;
			ht["Paste"] = (int)options.Paste;
			ht["PasteLineBreakSubstitution"] = (int)options.PasteLineBreakSubstitution;
			ht["PasteSingleCellAsText"] = options.PasteSingleCellAsText;
			ht["Printing"] = (int)options.Printing;
			ht["Save"] = (int)options.Save;
			ht["SaveAs"] = (int)options.SaveAs;
			ht["ShowPopupMenu"] = (int)options.ShowPopupMenu;
			ht["TabMarker"] = options.TabMarker;
			ht["Touch"] = (int)options.Touch;
			ht["UseFontSubstitution"] = options.UseFontSubstitution;
			ht["Zooming"] = (int)options.Zooming;
			return ht;
		}
		RichEditBehaviorOptions DeserializeBehaviorOptions(Hashtable source) {
			var options = new ASPxRichEditBehaviorSettings();
			options.Copy = (DocumentCapability)source["Copy"];
			options.CreateNew = (DocumentCapability)source["CreateNew"];
			options.Cut = (DocumentCapability)source["Cut"];
			options.Drag = (DocumentCapability)source["Drag"];
			options.Drop = (DocumentCapability)source["Drop"];
			options.FontSource = (RichEditBaseValueSource)source["FontSource"];
			options.ForeColorSource = (RichEditBaseValueSource)source["ForeColorSource"];
			options.MaxZoomFactor = (string)source["MaxZoomFactor"] == "Infinity" ? float.PositiveInfinity : Convert.ToSingle(source["MaxZoomFactor"]);
			options.MinZoomFactor = Convert.ToSingle(source["MinZoomFactor"]);
			options.OfficeScrolling = (DocumentCapability)source["OfficeScrolling"];
			options.Open = (DocumentCapability)source["Open"];
			options.OvertypeAllowed = (bool)source["OvertypeAllowed"];
			options.PageBreakInsertMode = (PageBreakInsertMode)source["PageBreakInsertMode"];
			options.Paste = (DocumentCapability)source["Paste"];
			options.PasteLineBreakSubstitution = (LineBreakSubstitute)source["PasteLineBreakSubstitution"];
			options.PasteSingleCellAsText = (bool)source["PasteSingleCellAsText"];
			options.Printing = (DocumentCapability)source["Printing"];
			options.Save = (DocumentCapability)source["Save"];
			options.SaveAs = (DocumentCapability)source["SaveAs"];
			options.ShowPopupMenu = (DocumentCapability)source["ShowPopupMenu"];
			options.TabMarker = (string)source["TabMarker"];
			options.Touch = (DocumentCapability)source["Touch"];
			options.UseFontSubstitution = (bool)source["UseFontSubstitution"];
			options.Zooming = (DocumentCapability)source["Zooming"];
			return options;
		}
	}
}
