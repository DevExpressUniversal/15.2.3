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
using System.IO;
using System.Xml;
using System.Text;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Utils.Zip;
using DevExpress.Office.Utils;
using DevExpress.Office.History;
using System.Collections.Generic;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Import.Html;
using DevExpress.Office.Services;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Import.EPub {
	#region EPubImporter
	public class EPubImporter : RichEditDestinationAndXmlBasedImporter, IDisposable, IUriStreamProvider {
		#region Fields
		PackageFileCollection packageFiles;
		PackageFileStreams packageFileStreams;
		readonly Dictionary<string, string> htmlFiles;
		readonly Dictionary<string, string> xmlFiles;
		readonly List<string> orderedHtmlFiles;
		string tableOfContentsPartId;
		string documentRootFolder;
		string rootFileName;
		string currentFileName;
		readonly Dictionary<string, string> bookmarkedHtmlFiles;
		readonly Dictionary<string, string> bookmarkedHtmlFilePaths;
		readonly Dictionary<string, string> bookmarksTable;
		#endregion
		public EPubImporter(DocumentModel documentModel, EPubDocumentImporterOptions options)
			: base(documentModel, options) {
			this.htmlFiles = new Dictionary<string, string>();
			this.xmlFiles = new Dictionary<string, string>();
			this.orderedHtmlFiles = new List<string>();
			this.bookmarkedHtmlFiles = new Dictionary<string, string>(StringExtensions.ComparerInvariantCultureIgnoreCase);
			this.bookmarkedHtmlFilePaths = new Dictionary<string, string>(StringExtensions.ComparerInvariantCultureIgnoreCase);
			this.bookmarksTable = new Dictionary<string, string>();
		}
		#region Properties
		protected EPubDocumentImporterOptions Options { get { return (EPubDocumentImporterOptions)InnerOptions; } }
		public PackageFileCollection PackageFiles { get { return packageFiles; } }
		protected internal PackageFileStreams PackageFileStreams { get { return packageFileStreams; } }
		public string RootFileName { get { return rootFileName; } set { rootFileName = value; } }
		public override string DocumentRootFolder { get { return documentRootFolder; } set { documentRootFolder = value; } }
		public override string RelationsNamespace { get { return String.Empty; } }
		public override OpenXmlRelationCollection DocumentRelations { get { return null; } }
		public Dictionary<string, string> HtmlFiles { get { return htmlFiles; } }
		public Dictionary<string, string> XmlFiles { get { return xmlFiles; } }
		public List<string> OrderedHtmlFiles { get { return orderedHtmlFiles; } }
		public string TableOfContentsPartId { get { return tableOfContentsPartId; } set { tableOfContentsPartId = value; } }
		public Dictionary<string, string> BookmarkedHtmlFiles { get { return bookmarkedHtmlFiles; } }
		public Dictionary<string, string> BookmarkedHtmlFilePaths { get { return bookmarkedHtmlFilePaths; } }
		public Dictionary<string, string> BookmarksTable { get { return bookmarksTable; } }
		public string CurrentFileName { get { return currentFileName; } }
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				DisposePackageFiles();
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		#region Package Utils
		protected internal virtual void DisposePackageFiles() {
			if (PackageFiles == null)
				return;
			int count = PackageFiles.Count;
			for (int i = 0; i < count; i++)
				PackageFiles[i].Stream.Dispose();
			PackageFiles.Clear();
			this.packageFiles = null;
		}
		protected internal virtual void OpenPackage(Stream stream) {
			this.packageFiles = GetPackageFiles(stream);
		}
		protected internal virtual PackageFileCollection GetPackageFiles(Stream stream) {
			InternalZipFileCollection files = InternalZipArchive.Open(stream);
			int count = files.Count;
			PackageFileCollection result = new PackageFileCollection();
			for (int i = 0; i < count; i++) {
				InternalZipFile file = files[i];
				result.Add(new PackageFile(file.FileName.Replace('\\', '/'), file.FileDataStream, (int)file.UncompressedSize));
			}
			return result;
		}
		protected internal virtual Stream GetCachedPackageFileStream(string fileName) {
			Stream stream;
			if (PackageFileStreams.TryGetValue(fileName, out stream)) {
				stream.Seek(0, SeekOrigin.Begin);
				return stream;
			}
			stream = CreatePackageFileStreamCopy(fileName);
			if (stream == null)
				return null;
			PackageFileStreams.Add(fileName, stream);
			return stream;
		}
		protected internal virtual Stream CreatePackageFileStreamCopy(string fileName) {
			PackageFile file = GetPackageFile(fileName);
			if (file == null)
				return null;
			byte[] bytes = new byte[file.StreamLength];
			file.Stream.Read(bytes, 0, bytes.Length);
			return new MemoryStream(bytes);
		}
		protected internal virtual Stream GetPackageFileStream(string fileName) {
			PackageFile file = GetPackageFile(fileName);
			if (file == null)
				return null;
			return file.Stream;
		}
		protected internal virtual PackageFile GetPackageFile(string fileName) {
			if (PackageFiles == null)
				return null;
			fileName = fileName.Replace('\\', '/');
			if (fileName.StartsWith("/"))
				fileName = fileName.Substring(1);
			int count = PackageFiles.Count;
			for (int i = 0; i < count; i++) {
				PackageFile file = PackageFiles[i];
				if (String.Compare(file.FileName, fileName, StringComparison.CurrentCultureIgnoreCase) == 0)
					return file;
			}
			return null;
		}
		protected internal virtual XmlReader GetPackageFileXmlReader(string fileName) {
			return GetPackageFileXmlReader(fileName, CreateXmlReaderSettings());
		}
		protected internal virtual XmlReader GetPackageFileXmlReader(string fileName, XmlReaderSettings settings) {
			Stream stream = GetPackageFileStream(fileName);
			if (stream == null)
				return null;
			return XmlReader.Create(stream, settings);
		}
		#endregion
		public void Import(Stream stream) {
			OpenPackage(stream);
			ReadRootFileName();
			if (String.IsNullOrEmpty(RootFileName))
				return;
			this.documentRootFolder = Path.GetDirectoryName(RootFileName);
			this.packageFileStreams = new PackageFileStreams();
			XmlReaderSettings settings = base.CreateXmlReaderSettings();
			settings.DtdProcessing = DtdProcessing.Ignore;
#if !DXPORTABLE
#if !SL
			settings.ValidationType = ValidationType.None;
#endif
			settings.XmlResolver = null;
#endif
			XmlReader reader = GetPackageFileXmlReader(RootFileName, settings);
			if (!ReadToRootElement(reader, "package", "http://www.idpf.org/2007/opf"))
				return;
			ImportMainDocument(reader, stream);
		}
		protected override void AfterImportMainDocument() {
			base.AfterImportMainDocument();
			DocumentModel.NormalizeZOrder();
		}
		public override void ImportContent(XmlReader reader, Destination initialDestination) {
			base.ImportContent(reader, initialDestination);
			ImportTableOfContents(reader);
			CalculatePartsImportOrder();
			ImportParts();
		}
		protected internal virtual void ImportTableOfContents(XmlReader reader) {
			if (String.IsNullOrEmpty(TableOfContentsPartId))
				return;
			string fileName;
			if (!XmlFiles.TryGetValue(TableOfContentsPartId, out fileName))
				return;
			XmlReaderSettings settings = base.CreateXmlReaderSettings();
			settings.DtdProcessing = DtdProcessing.Ignore;
#if !DXPORTABLE
#if !SL
			settings.ValidationType = ValidationType.None;
#endif
			settings.XmlResolver = null;
#endif
			XmlReader tocReader = GetPackageFileXmlReader(documentRootFolder + "/" + fileName, settings);
			if (tocReader != null && ReadToRootElement(tocReader, "ncx"))
				base.ImportContent(tocReader, new TableOfContentDestination(this));
		}
		protected internal virtual void CalculatePartsImportOrder() {
			if (OrderedHtmlFiles.Count > 0)
				return;
			OrderedHtmlFiles.AddRange(HtmlFiles.Keys);
		}
		protected internal virtual void ImportParts() {
			int count = OrderedHtmlFiles.Count;
			for (int i = 0; i < count; i++)
				ImportPart(OrderedHtmlFiles[i]);
		}
		protected internal virtual void ImportPart(string id) {
			string fileName;
			if (HtmlFiles.TryGetValue(id, out fileName))
				ImportHtmlFileAsSection(fileName);
		}
		protected internal void ReadRootFileName() {
			XmlReader reader = GetPackageFileXmlReader("META-INF/container.xml");
			if (reader != null)
				ReaderRootFileName(reader);
		}
		protected internal virtual void ReaderRootFileName(XmlReader reader) {
			if (!ReadToRootElement(reader, "container", "urn:oasis:names:tc:opendocument:xmlns:container"))
				return;
			ImportContent(reader, new PackageContainerDestination(this));
		}
		protected override Destination CreateMainDocumentDestination() {
			return new DocumentDestination(this);
		}
		public override string ReadAttribute(XmlReader reader, string attributeName) {
			return reader.GetAttribute(attributeName);
		}
		public override string ReadAttribute(XmlReader reader, string attributeName, string ns) {
			return reader.GetAttribute(attributeName, ns);
		}
		public override bool ConvertToBool(string value) {
			return false; 
		}
		public override void ThrowInvalidFile() {
			throw new Exception("Invalid Electronic Publication file");
		}
		protected internal virtual void ImportHtmlFileAsSection(string fileName) {
			this.currentFileName = DocumentRootFolder + "/" + fileName;
			Stream stream = GetPackageFileStream(currentFileName);
			if (stream != null) {
				if (!DocumentModel.IsEmpty)
					InsertSection();
				string bookmarkName;
				BookmarkedHtmlFiles.TryGetValue(fileName, out bookmarkName);
				DocumentLogPosition start = Position.LogPosition;
				ImportSectionContent(stream);
				if (!String.IsNullOrEmpty(bookmarkName))
					PieceTable.CreateBookmark(start, 0, bookmarkName);
			}
		}
		protected internal virtual void InsertSection() {
			Debug.Assert(PieceTable.IsMain);
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				PieceTable.InsertParagraphCore(Position);
				ParagraphIndex paragraphIndex = Position.ParagraphIndex;
				PieceTable.InsertSectionParagraphCore(Position);
				DocumentModel.SafeEditor.PerformInsertSectionCore(paragraphIndex);
				CurrentSection = DocumentModel.Sections.Last;
			}
		}
		protected internal virtual void ImportSectionContent(Stream stream) {
			RegisterUriStreamProvider();
			try {
				EPubHtmlImporter importer = new EPubHtmlImporter(this, DocumentModel, new EPubHtmlDocumentImporterOptions());
				HtmlInputPosition pos = new HtmlInputPosition(PieceTable);
				pos.LogPosition = Position.LogPosition;
				pos.ParagraphIndex = Position.ParagraphIndex;
				importer.ImportCore(stream, pos);
				Position.LogPosition = PieceTable.DocumentEndLogPosition;
				Position.ParagraphIndex = new ParagraphIndex(PieceTable.Paragraphs.Count - 1);
			}
			finally {
				UnregisterUriStreamProvider();
			}
		}
		protected internal virtual void RegisterUriStreamProvider() {
			IUriStreamService service = DocumentModel.GetService<IUriStreamService>();
			if (service != null)
				service.RegisterProvider(this);
		}
		protected internal virtual void UnregisterUriStreamProvider() {
			IUriStreamService service = DocumentModel.GetService<IUriStreamService>();
			if (service != null)
				service.UnregisterProvider(this);
		}
		protected override void PrepareOfficeTheme() {
		} 
#region IUriStreamProvider Members
		Stream IUriStreamProvider.GetStream(string uri) {
			if (uri.StartsWith("..")) {
				string path = Path.Combine(Path.GetDirectoryName(this.currentFileName), uri);
				return GetCachedPackageFileStream(EPubPath.Normalize(path));
			}
			else
				return GetCachedPackageFileStream(DocumentRootFolder + "/" + uri);
		}
#endregion
	}
#endregion
#region EPubHtmlDocumentImporterOptions
	public class EPubHtmlDocumentImporterOptions : HtmlDocumentImporterOptions {
		public EPubHtmlDocumentImporterOptions() {
			AsyncImageLoading = false;
		}
	}
#endregion
#region EPubPath
	public static class EPubPath {
		public static string Normalize(string path) {
			if (String.IsNullOrEmpty(path))
				return path;
			List<string> normalizedParts = CreateNormalizedParts(path);
			StringBuilder result = new StringBuilder();
			int count = normalizedParts.Count;
			for (int i = 0; i < count; i++) {
				if (result.Length > 0)
					result.Append('/');
				result.Append(normalizedParts[i]);
			}
			return result.ToString();
		}
		static List<string> CreateNormalizedParts(string path) {
			string[] parts = path.Split('/', '\\');
			List<string> normalizedParts = new List<string>();
			int count = parts.Length;
			for (int i = 0; i < count; i++) {
				string part = parts[i];
				if (!String.IsNullOrEmpty(part)) {
					if (part == "..")
						normalizedParts.RemoveAt(normalizedParts.Count - 1);
					else
						normalizedParts.Add(part);
				}
			}
			return normalizedParts;
		}
	}
#endregion
#region EPubHtmlImporter
	public class EPubHtmlImporter : HtmlImporter {
		readonly EPubImporter importer;
		public EPubHtmlImporter(EPubImporter importer, DocumentModel documentModel, EPubHtmlDocumentImporterOptions options)
			: base(documentModel, options) {
			Guard.ArgumentNotNull(importer, "importer");
			this.importer = importer;
		}
		public EPubImporter Importer { get { return importer; } }
		protected override string ValidateBookmarkName(string anchorName) {
			return Path.GetFileName(Importer.CurrentFileName) + "_" + anchorName;
		}
		protected internal override void ValidateHyperlinkInfo(HyperlinkInfo hyperlinkInfo) {
			if (TryValidateDirectPartLink(hyperlinkInfo))
				return;
			if (TryValidateInnerLink(hyperlinkInfo))
				return;
			base.ValidateHyperlinkInfo(hyperlinkInfo);
		}
		string TryGetPartBookmark(HyperlinkInfo hyperlinkInfo) {
			try {
				if (hyperlinkInfo.NavigateUri.Contains("javascript:")) {
					hyperlinkInfo.NavigateUri = String.Empty;
					return String.Empty;
				}
				string directory = Path.GetDirectoryName(Importer.CurrentFileName);
				string fullPath = Path.Combine(directory, hyperlinkInfo.NavigateUri);
				fullPath = fullPath.Replace('\\', '/');
				int anchorIndex = fullPath.LastIndexOf('#');
				if (anchorIndex >= 0)
					fullPath = fullPath.Substring(0, anchorIndex);
				string bookmarkName;
				if (Importer.BookmarkedHtmlFilePaths.TryGetValue(fullPath, out bookmarkName))
					return bookmarkName;
				else
					return String.Empty;
			}
			catch {
				return String.Empty;
			}
		}
		bool IsInnerPackageLink(HyperlinkInfo hyperlinkInfo) {
			return !String.IsNullOrEmpty(TryGetPartBookmark(hyperlinkInfo));
		}
		protected internal virtual bool TryValidateDirectPartLink(HyperlinkInfo hyperlinkInfo) {
			if (String.IsNullOrEmpty(hyperlinkInfo.Anchor) && hyperlinkInfo.NavigateUri.IndexOf('#') < 0) {
				string bookmarkName = TryGetPartBookmark(hyperlinkInfo);
				if (!String.IsNullOrEmpty(bookmarkName)) {
					hyperlinkInfo.NavigateUri = String.Empty;
					hyperlinkInfo.Anchor = bookmarkName;
					return true;
				}
			}
			return false;
		}
		protected internal virtual bool TryValidateInnerLink(HyperlinkInfo hyperlinkInfo) {
			int anchorPosition = hyperlinkInfo.NavigateUri.IndexOf('#');
			if (anchorPosition == 0) {
				hyperlinkInfo.Anchor = ValidateBookmarkName(hyperlinkInfo.NavigateUri.Substring(1));
				hyperlinkInfo.NavigateUri = String.Empty;
				return true;
			}
			if (anchorPosition > 0 && anchorPosition < hyperlinkInfo.NavigateUri.Length - 1) {
				if (IsInnerPackageLink(hyperlinkInfo)) {
					hyperlinkInfo.Anchor = hyperlinkInfo.NavigateUri.Substring(0, anchorPosition) + "_" + hyperlinkInfo.NavigateUri.Substring(anchorPosition + 1);
					hyperlinkInfo.NavigateUri = String.Empty;
					return true;
				}
			}
			return false;
		}
	}
#endregion
}
