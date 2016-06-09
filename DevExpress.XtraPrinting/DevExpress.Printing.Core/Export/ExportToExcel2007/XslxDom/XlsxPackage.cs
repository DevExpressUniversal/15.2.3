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
using System.Text;
using DevExpress.Utils.Zip;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Drawing.Imaging;
using System.Xml;
using System.Globalization;
namespace DevExpress.XtraExport {
	public class XlsxPackage {
		#region inner classes
		class StreamProviderCreator {
			bool? canUseFileStream;
			bool CanUseFileStream {
				get {
					if(!canUseFileStream.HasValue) {
						try {
							using(FileStreamProvider provider = new FileStreamProvider()) {
								System.Diagnostics.Debug.Assert(provider.Stream != null);
							}
							canUseFileStream = true;
						} catch {
							canUseFileStream = false;
						}
					}
					return canUseFileStream.Value;
				}
			}
			public StreamProvider Create() {
				return CanUseFileStream ? new FileStreamProvider() : new StreamProvider();
			}
		}
		class StreamProvider : IDisposable {
			Stream stream;
			public Stream Stream { get { return stream; } }
			public StreamProvider() {
				stream = CreateStream();
			}
			protected virtual Stream CreateStream() {
				return new MemoryStream();
			}
			public virtual void Dispose() {
				if(stream != null) {
					stream.Dispose();
					stream = null;
				}
			}
		}
		class FileStreamProvider : StreamProvider {
			string path = string.Empty;
			protected override Stream CreateStream() {
				path = Path.GetTempFileName();
				return File.Create(path);
			}
			public override void Dispose() {
				base.Dispose();
				if(!string.IsNullOrEmpty(path)) {
					File.Delete(path);
					path = string.Empty;
				}
			}
		}
		class XmlStreamWriter : StreamWriter {
			public XmlStreamWriter(Stream stream, Encoding encoding)
				: base(stream, encoding) {
			}
			public override void Close() {
				Flush();
			}
		}
		class XmlDOMTextWriter : XmlTextWriter {
			public XmlDOMTextWriter(TextWriter w)
				: base(w) {
			}
			public XmlDOMTextWriter(Stream w, Encoding encoding)
				: base(w, encoding) {
			}
			public XmlDOMTextWriter(string filename, Encoding encoding)
				: base(filename, encoding) {
			}
			public override void WriteStartAttribute(string prefix, string localName, string ns) {
				if(string.IsNullOrEmpty(ns) && !string.IsNullOrEmpty(prefix)) {
					prefix = "";
				}
				base.WriteStartAttribute(prefix, localName, ns);
			}
			public override void WriteStartElement(string prefix, string localName, string ns) {
				if(string.IsNullOrEmpty(ns) && !string.IsNullOrEmpty(prefix)) {
					prefix = "";
				}
				base.WriteStartElement(prefix, localName, ns);
			}
		}
		#endregion
		static readonly DateTime fileTime =
#if DEBUGTEST
 DateTime.MinValue;
#else
			DateTime.Now;
#endif
		Dictionary<Image, string> currentImageToPath;
		List<Dictionary<Image, string>> imageToPathList = new List<Dictionary<Image, string>>();
		string fileName;
		Stream stream;
		bool isStreamMode;
		SharedStringsDocument sharedStringsDocument = new SharedStringsDocument();
		WorksheetDocument currentWorksheetDocument;
		List<WorksheetDocument> worksheetDocumentList = new List<WorksheetDocument>();
		RelsDocument currentWorksheetRelsDocument;
		List<RelsDocument> worksheetRelsDocumentList = new List<RelsDocument>();
		WorkbookDocument workbookDocument = new WorkbookDocument();
		RelsDocument relsDocument;
		StyleSheetDocument styleSheetDocument = new StyleSheetDocument();
		RelsDocument currentDrawingRelsDocument;
		List<RelsDocument> drawingRelsDocumentList = new List<RelsDocument>();
		DrawingDocument currentDrawingDocument;
		List<DrawingDocument> drawingDocumentList = new List<DrawingDocument>();
		StreamProviderCreator streamProviderCreator = new StreamProviderCreator();
		public XlsxPackage(string fileName) {
			this.fileName = fileName;
			CreateRelsDocument();
		}
		public XlsxPackage(Stream stream) {
			this.stream = stream;
			isStreamMode = true;
			CreateRelsDocument();
		}
		public SharedStringsDocument SharedStringsDocument { get { return sharedStringsDocument; } }
		public WorksheetDocument WorksheetDocument { get { return currentWorksheetDocument; } }
		public WorkbookDocument WorkbookDocument { get { return workbookDocument; } }
		public StyleSheetDocument StyleSheetDocument { get { return styleSheetDocument; } }
		public RelsDocument WorksheetRelsDocument { get { return currentWorksheetRelsDocument; } }
		public RelsDocument DrawingRelsDocument { get { return currentDrawingRelsDocument; } }
		public DrawingDocument DrawingDocument { get { return currentDrawingDocument; } }
		public string AppendImage(Image image) {
			string path;
			for(int i = 0; i < imageToPathList.Count; i++)
				if(imageToPathList[i].TryGetValue(image, out path))
					return path;
			path = GenerateImagePath();
			currentImageToPath.Add(image, path);
			return path;
		}
		public void CreateSheet(string sheetName) {
			workbookDocument.AppendSheet(sheetName);
			WorksheetDocument worksheetDocument = new WorksheetDocument();
			RelsDocument worksheetRelsDocument = new RelsDocument();
			Dictionary<Image, string> imageToPath = new Dictionary<Image, string>();
			DrawingDocument drawingDocument = new DrawingDocument();
			RelsDocument drawingRelsDocument = new RelsDocument();
			worksheetDocumentList.Add(worksheetDocument);
			worksheetRelsDocumentList.Add(worksheetRelsDocument);
			imageToPathList.Add(imageToPath);
			drawingDocumentList.Add(drawingDocument);
			drawingRelsDocumentList.Add(drawingRelsDocument);
			this.currentWorksheetDocument = worksheetDocument;
			this.currentWorksheetRelsDocument = worksheetRelsDocument;
			this.currentImageToPath = imageToPath;
			this.currentDrawingDocument = drawingDocument;
			this.currentDrawingRelsDocument = drawingRelsDocument;
		}
		public void CreateXlsxFile() {
			using(InternalZipArchive archive = isStreamMode ? new InternalZipArchive(stream) : new InternalZipArchive(fileName)) {
				AddDrawings(archive);
				AddToArchive(archive, XlsxHelper.StyleSheetPath, styleSheetDocument);
				AddToArchive(archive, XlsxHelper.ContentTypesPath, new ContentTypesDocument(worksheetDocumentList.Count, imageToPathList.Count));
				AddToArchive(archive, XlsxHelper.SharedStringsPath, sharedStringsDocument);
				AddWorkSheets(archive);
				AddToArchive(archive, XlsxHelper.WorkbookPath, workbookDocument);
				AddToArchive(archive, XlsxHelper.WorkbookRelsPath, CreateWoorkbookRelsDocument(worksheetDocumentList.Count));
				AddToArchive(archive, XlsxHelper.RelsPath, relsDocument);
				AddWorkSheetsRels(archive);
			}
		}
		void AddToArchive(InternalZipArchive archive, string path, XmlNode node) {
			using(StreamProvider provider = streamProviderCreator.Create()) {
				AddToArchive(archive, path, node, provider.Stream);
			}
		}
		void AddToArchive(InternalZipArchive archive, string path, IXmlWriteTo node) {
			using(StreamProvider provider = streamProviderCreator.Create()) {
				AddToArchive(archive, path, node, provider.Stream);
			}
		}
		static void AddToArchive(InternalZipArchive archive, string path, XmlNode node, Stream stream) {
			using(XmlStreamWriter writer = new XmlStreamWriter(stream, new UTF8Encoding(false))) {
				XmlDOMTextWriter writer2 = new XmlDOMTextWriter(writer);
				try {
					node.WriteTo(writer2);
				} finally {
					writer2.Close();
				}
				stream.Flush();
				stream.Position = 0;
				archive.Add(path, fileTime, stream);
			}
		}
		static void AddToArchive(InternalZipArchive archive, string path, IXmlWriteTo node, Stream stream) {
			using(XmlStreamWriter writer = new XmlStreamWriter(stream, new UTF8Encoding(false))) {
				XmlDOMTextWriter writer2 = new XmlDOMTextWriter(writer);
				try {
					node.WriteTo(writer2);
				}
				finally {
					writer2.Close();
				}
				stream.Flush();
				stream.Position = 0;
				archive.Add(path, fileTime, stream);
			}
		}
		RelsDocument CreateRelsDocument() {
			relsDocument = new RelsDocument();
			relsDocument.RelationshipsNode.AppendRelationshipNode(XlsxHelper.WorkbookPath, @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument", XlsxHelper.WorkbookRelID, false);
			return relsDocument;
		}
		RelsDocument CreateWoorkbookRelsDocument(int sheetCount) {
			RelsDocument workbookRelsDocument = new RelsDocument();
			workbookRelsDocument.RelationshipsNode.AppendRelationshipNode(
				XlsxHelper.StyleSheetName,
				XlsxHelper.StylesTargetType,
				XlsxHelper.StylesRelID, false);
			for(int i = 0; i < sheetCount; i++)
				workbookRelsDocument.RelationshipsNode.AppendRelationshipNode(
					string.Format(XlsxHelper.WorksheetName, i),
					XlsxHelper.WorksheetTargetType,
					string.Format(XlsxHelper.SheetRelID, i), false);
			workbookRelsDocument.RelationshipsNode.AppendRelationshipNode(
				XlsxHelper.SharedStringsName,
				XlsxHelper.SharedStringsTargetType,
				XlsxHelper.SharedStringsRelID, false);
			return workbookRelsDocument;
		}
		string GenerateImagePath() {
			return @"xl/media/image" + imageToPathList.IndexOf(currentImageToPath).ToString() + "_" + currentImageToPath.Count.ToString() + ".png";
		}
		void AddDrawings(InternalZipArchive archive) {
			for(int i = 0; i < imageToPathList.Count; i++) {
				if(drawingDocumentList[i].WsDrNode.ChildNodes.Count > 0) {
					AddToArchive(archive, string.Format(XlsxHelper.DrawingPath, i), drawingDocumentList[i]);
					string drawingId = worksheetRelsDocumentList[i].RelationshipsNode.GetRelationshipNodeWithCache(string.Format(XlsxHelper.DrawingName, i), XlsxHelper.DrawingTargetType, false).Id;
					worksheetDocumentList[i].WorksheetNode.DrawingId = drawingId;
				}
				if(drawingRelsDocumentList[i].RelationshipsNode.ChildNodes.Count > 0) {
					AddToArchive(archive, string.Format(XlsxHelper.DrawingRelsPath, i), drawingRelsDocumentList[i]);
					foreach(KeyValuePair<Image, string> pair in imageToPathList[i]) {
						using(MemoryStream imageStream = new MemoryStream()) {
							pair.Key.Save(imageStream, ImageFormat.Png);
							imageStream.Position = 0;
							archive.Add(pair.Value, fileTime, imageStream);
						}
					}
				} 
			}
		}
		void AddWorkSheets(InternalZipArchive archive) {
			for(int i = 0; i < worksheetDocumentList.Count; i++)
				AddToArchive(archive, string.Format(XlsxHelper.WorksheetPath, i), worksheetDocumentList[i]);
		}
		void AddWorkSheetsRels(InternalZipArchive archive) {
			for(int i = 0; i < worksheetDocumentList.Count; i++)
				if(worksheetRelsDocumentList[i].RelationshipsNode.ChildNodes.Count > 0)
					AddToArchive(archive, string.Format(XlsxHelper.WorksheetRelsPath, i), worksheetRelsDocumentList[i]);
		}
		public FileHyperlinkNode GetFileHyperlinkByText(string hyperlinkText) {
			foreach(WorksheetDocument document in worksheetDocumentList) {
				foreach(HyperlinkNodeBase hyperlinkNodeBase in document.WorksheetNode.HyperlinksNode) {
					FileHyperlinkNode fileHyperlinkNode = hyperlinkNodeBase as FileHyperlinkNode;
					if(fileHyperlinkNode != null && fileHyperlinkNode.HyperlinkText == hyperlinkText) {
						fileHyperlinkNode.SheetName = workbookDocument.SheetsNode.SheetNode.SheetName;
						if(fileHyperlinkNode.Col == 0 && fileHyperlinkNode.Row == 0)
							return fileHyperlinkNode;
						else return WorksheetDocument.WorksheetNode.HyperlinksNode.AppendFileHyperlinkNode(fileHyperlinkNode);
					}
				}
			}
			return WorksheetDocument.WorksheetNode.HyperlinksNode.AppendFileHyperlinkNode(hyperlinkText);
		}
		public List<FileHyperlinkNode> GetFileHyperlinksByText(string hyperlinkText) {
			List<FileHyperlinkNode> result = new List<FileHyperlinkNode>();
			foreach(WorksheetDocument document in worksheetDocumentList) {
				foreach(HyperlinkNodeBase hyperlinkNodeBase in document.WorksheetNode.HyperlinksNode) {
					FileHyperlinkNode fileHyperlinkNode = hyperlinkNodeBase as FileHyperlinkNode;
					if(fileHyperlinkNode != null && fileHyperlinkNode.HyperlinkText == hyperlinkText) {
						fileHyperlinkNode.SheetName = workbookDocument.SheetsNode.SheetNode.SheetName;
						result.Add(fileHyperlinkNode);
					}
				}
			}
			if(result.Count == 0)
				result.Add(WorksheetDocument.WorksheetNode.HyperlinksNode.AppendFileHyperlinkNode(hyperlinkText));
			return result;
		}
	}
}
