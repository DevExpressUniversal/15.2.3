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
using DevExpress.Export.Xl;
using System.Drawing;
using System.Drawing.Imaging;
namespace DevExpress.XtraExport.Xlsx {
	using DevExpress.Office;
	using DevExpress.XtraExport.Implementation;
	using DevExpress.XtraPrinting;
	using DevExpress.Utils.Crypt;
	using DevExpress.Compatibility.System.Drawing.Imaging;
	#region XlsxPicture
	class XlsxPicture : XlDrawingObjectBase {
		public string RelationId { get; set; }
		public int PictureId { get; set; }
		public XlPictureHyperlink HyperlinkClick { get; set; }
	}
	#endregion
	#region XlsxPictureFileInfo
	class XlsxPictureFileInfo {
		public XlsxPictureFileInfo(string fileName, byte[] fileDigest) {
			FileName = fileName;
			FileDigest = fileDigest;
		}
		public string FileName { get; private set; }
		public byte[] FileDigest { get; private set; }
		public bool EqualsDigest(byte[] digest) {
			if(digest == null)
				return false;
			if(FileDigest.Length != digest.Length)
				return false;
			for(int i = 0; i < FileDigest.Length; i++) {
				if(FileDigest[i] != digest[i])
					return false;
			}
			return true;
		}
	}
	#endregion
	partial class XlsxDataAwareExporter {
		#region Statics
		static readonly Dictionary<XlAnchorType, string> anchorTypeTable = CreateAnchorTypeTable();
		static Dictionary<XlAnchorType, string> CreateAnchorTypeTable() {
			Dictionary<XlAnchorType, string> result = new Dictionary<XlAnchorType, string>();
			result.Add(XlAnchorType.TwoCell, "twoCell");
			result.Add(XlAnchorType.OneCell, "oneCell");
			result.Add(XlAnchorType.Absolute, "absolute");
			return result;
		}
		#endregion
		#region Fields
		const string relsImageType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image";
		const string relsDrawingType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/drawing";
		const string drawingContentType = "application/vnd.openxmlformats-officedocument.drawing+xml";
		const string spreadsheetDrawingNamespace = "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing";
		const string drawingMLNamespace = "http://schemas.openxmlformats.org/drawingml/2006/main";
		int shapeId;
		int drawingId;
		int imageId;
		readonly OpenXmlRelationCollection drawingRelations = new OpenXmlRelationCollection();
		readonly List<XlsxPicture> drawingObjects = new List<XlsxPicture>();
		readonly List<XlsxPictureFileInfo> imageFiles = new List<XlsxPictureFileInfo>();
		readonly Dictionary<string, string> exportedPictureHyperlinkTable = new Dictionary<string, string>();
		#endregion
		#region Statics
		static readonly Dictionary<ImageFormat, string> imageContentTypeTable = CreateImageContentTypeTable();
		static Dictionary<ImageFormat, string> CreateImageContentTypeTable() {
			Dictionary<ImageFormat, string> result = new Dictionary<ImageFormat, string>();
			result.Add(ImageFormat.Jpeg, "image/jpeg");
			result.Add(ImageFormat.Png, "image/png");
			result.Add(ImageFormat.Bmp, "image/bitmap");
			result.Add(ImageFormat.Tiff, "image/tiff");
			result.Add(ImageFormat.Gif, "image/gif");
			result.Add(ImageFormat.Icon, "image/x-icon");
			result.Add(ImageFormat.Wmf, "application/x-msmetafile");
			result.Add(ImageFormat.Emf, "application/x-msmetafile");
			return result;
		}
		static readonly Dictionary<ImageFormat, string> imageExtenstionTable = CreateImageExtenstionTable();
		static Dictionary<ImageFormat, string> CreateImageExtenstionTable() {
			Dictionary<ImageFormat, string> result = new Dictionary<ImageFormat, string>();
			result.Add(ImageFormat.Bmp, "bmp");
			result.Add(ImageFormat.Emf, "emf");
			result.Add(ImageFormat.Gif, "gif");
			result.Add(ImageFormat.Icon, "ico");
			result.Add(ImageFormat.Jpeg, "jpg");
			result.Add(ImageFormat.Png, "png");
			result.Add(ImageFormat.Tiff, "tif");
			result.Add(ImageFormat.Wmf, "wmf");
			return result;
		}
		#endregion
		XlPicture currentPicture = null;
		public IXlPicture BeginPicture() {
			CheckCurrentSheet();
			currentPicture = new XlPicture(this);
			currentPicture.Name = string.Format("Picture {0}", shapeId + 1);
			currentPicture.Format = ImageFormat.Png;
			currentPicture.AnchorType = XlAnchorType.TwoCell;
			currentPicture.AnchorBehavior = XlAnchorType.OneCell;
			return currentPicture;
		}
		public void EndPicture() {
			CheckCurrentSheet();
			if(currentPicture == null || currentPicture.Image == null || !imageExtenstionTable.ContainsKey(currentPicture.Format)) {
				currentPicture = null;
				return;
			}
			shapeId++;
			if(drawingRelations.Count == 0)
				drawingId++;
			byte[] imageBytes;
			try {
				imageBytes = currentPicture.GetImageBytes(currentPicture.Format);
			}
			catch {
				currentPicture.Format = ImageFormat.Png;
				imageBytes = currentPicture.GetImageBytes(currentPicture.Format);
			}
			string extension = GetImageExtension(currentPicture.Format);
			string contentType = GetImageContentType(currentPicture.Format);
			Builder.UsedContentTypes[extension] = contentType;
			MD4HashCalculator calculator = new MD4HashCalculator();
			uint[] hash = calculator.InitialCheckSumValue;
			calculator.UpdateCheckSum(hash, imageBytes, 0, imageBytes.Length);
			byte[] imageDigest = MD4HashConverter.ToArray(calculator.GetFinalCheckSum(hash));
			string rId;
			string imageFileName = FindImageFile(imageDigest);
			if(string.IsNullOrEmpty(imageFileName)) {
				imageId++;
				imageFileName = string.Format("image{0}.{1}", imageId, extension);
				string targetName = @"../media/" + imageFileName;
				rId = drawingRelations.GenerateId();
				OpenXmlRelation relation = new OpenXmlRelation(rId, targetName, relsImageType);
				drawingRelations.Add(relation);
				imageFiles.Add(new XlsxPictureFileInfo(imageFileName, imageDigest));
				Builder.Package.Add(@"xl/media/" + imageFileName, Builder.Now, imageBytes);
			}
			else {
				string targetName = @"../media/" + imageFileName;
				OpenXmlRelation relation = drawingRelations.LookupRelationByTargetAndType(targetName, relsImageType);
				if(relation != null) {
					rId = relation.Id;
				}
				else {
					rId = drawingRelations.GenerateId();
					relation = new OpenXmlRelation(rId, targetName, relsImageType);
					drawingRelations.Add(relation);
				}
			}
			XlsxPicture pictureObject = new XlsxPicture();
			pictureObject.RelationId = rId;
			pictureObject.PictureId = shapeId + 1;
			pictureObject.Name = currentPicture.Name;
			pictureObject.AnchorType = currentPicture.AnchorType;
			pictureObject.AnchorBehavior = currentPicture.AnchorBehavior;
			pictureObject.TopLeft = currentPicture.TopLeft;
			pictureObject.BottomRight = currentPicture.BottomRight;
			pictureObject.HyperlinkClick = currentPicture.HyperlinkClick.Clone();
			drawingObjects.Add(pictureObject);
			currentPicture = null;
		}
		#region Utils
		string FindImageFile(byte[] digest) {
			foreach(XlsxPictureFileInfo info in imageFiles) {
				if(info.EqualsDigest(digest))
					return info.FileName;
			}
			return string.Empty;
		}
		string GetImageExtension(ImageFormat format) {
			string result;
			if(imageExtenstionTable.TryGetValue(format, out result))
				return result;
			else
				return "png";
		}
		string GetImageContentType(ImageFormat format) {
			string result;
			if(imageContentTypeTable.TryGetValue(format, out result))
				return result;
			else
				return "image/png";
		}
		void CheckCurrentSheet() {
			if(currentSheet == null)
				throw new InvalidOperationException("No current sheet!");
		}
		#endregion
		#region GenerateDrawingRef
		void GenerateDrawingRef() {
			if(drawingObjects.Count == 0 && !ShouldExportShapes())
				return;
			if(drawingObjects.Count == 0)
				drawingId++;
			string rId = this.sheetRelations.GenerateId();
			string target = string.Format(@"../drawings/drawing{0}.xml", drawingId);
			OpenXmlRelation relation = new OpenXmlRelation(rId, target, relsDrawingType);
			this.sheetRelations.Add(relation);
			WriteShStartElement("drawing");
			try {
				WriteStringAttr(XlsxPackageBuilder.RelsPrefix, "id", null, rId);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region GenerateDrawingContent
		void GenerateDrawingContent() {
			if(drawingObjects.Count == 0 && !ShouldExportShapes())
				return;
			BeginWriteXmlContent();
			GenerateDrawingContentCore();
			AddPackageContent(String.Format(@"xl\drawings\drawing{0}.xml", drawingId), EndWriteXmlContent());
			GenerateDrawingRelations();
			Builder.OverriddenContentTypes.Add(String.Format(@"/xl/drawings/drawing{0}.xml", drawingId), drawingContentType);
		}
		void GenerateDrawingContentCore() {
			WriteStartElement("xdr", "wsDr", spreadsheetDrawingNamespace);
			try {
				WriteStringAttr("xmlns", "xdr", null, spreadsheetDrawingNamespace);
				WriteStringAttr("xmlns", "a", null, drawingMLNamespace);
				foreach(XlsxPicture drawingObject in drawingObjects)
					GenerateDrawingObject(drawingObject);
				IXlShapeContainer container = currentSheet as IXlShapeContainer;
				if(container != null) {
					foreach(XlShape shape in container.Shapes)
						GenerateShape(shape);
				}
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateDrawingObject(XlsxPicture drawingObject) {
			switch(drawingObject.AnchorType) {
				case XlAnchorType.TwoCell:
					WriteTwoCellAnchor(drawingObject, WritePicture);
					break;
				case XlAnchorType.OneCell:
					WriteOneCellAnchor(drawingObject, WritePicture);
					break;
				case XlAnchorType.Absolute:
					WriteAbsoluteAnchor(drawingObject, WritePicture);
					break;
			}
		}
		void WriteTwoCellAnchor(XlDrawingObjectBase drawingObject, Action<XlDrawingObjectBase> action) {
			WriteStartElement("twoCellAnchor", spreadsheetDrawingNamespace);
			try {
				WriteStringValue("editAs", anchorTypeTable[drawingObject.AnchorBehavior]);
				WriteAnchorPoint("from", drawingObject.TopLeft, false);
				WriteAnchorPoint("to", drawingObject.BottomRight, drawingObject.IsSingleCellAnchor());
				action(drawingObject);
				WriteClientData(drawingObject);
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteOneCellAnchor(XlDrawingObjectBase drawingObject, Action<XlDrawingObjectBase> action) {
			WriteStartElement("oneCellAnchor", spreadsheetDrawingNamespace);
			try {
				WriteAnchorPoint("from", drawingObject.TopLeft, false);
				WriteShapeExtent(drawingObject);
				action(drawingObject);
				WriteClientData(drawingObject);
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteAbsoluteAnchor(XlDrawingObjectBase drawingObject, Action<XlDrawingObjectBase> action) {
			WriteStartElement("absoluteAnchor", spreadsheetDrawingNamespace);
			try {
				WriteShapePosition(drawingObject);
				WriteShapeExtent(drawingObject);
				action(drawingObject);
				WriteClientData(drawingObject);
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteAnchorPoint(string tag, XlAnchorPoint point, bool singleCellAnchor) {
			WriteStartElement(tag, spreadsheetDrawingNamespace);
			try {
				int column = singleCellAnchor ? point.Column + 1 : point.Column;
				int row = singleCellAnchor ? point.Row + 1 : point.Row;
				long columnOffset = PixelsToEMU(point.ColumnOffsetInPixels);
				long rowOffset = PixelsToEMU(point.RowOffsetInPixels);
				WriteAnchorTag("col", spreadsheetDrawingNamespace, column.ToString());
				WriteAnchorTag("colOff", spreadsheetDrawingNamespace, columnOffset.ToString());
				WriteAnchorTag("row", spreadsheetDrawingNamespace, row.ToString());
				WriteAnchorTag("rowOff", spreadsheetDrawingNamespace, rowOffset.ToString());
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteAnchorTag(string tag, string ns, string value) {
			WriteStartElement(tag, ns);
			try {
				WriteShString(value);
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteShapePosition(XlDrawingObjectBase drawingObject) {
			long x = PixelsToEMU(drawingObject.TopLeft.ColumnOffsetInPixels);
			long y = PixelsToEMU(drawingObject.TopLeft.RowOffsetInPixels);
			WriteStartElement("pos", spreadsheetDrawingNamespace);
			try {
				WriteStringValue("x", x.ToString());
				WriteStringValue("y", y.ToString());
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteShapeExtent(XlDrawingObjectBase drawingObject) {
			long cx = PixelsToEMU(drawingObject.BottomRight.ColumnOffsetInPixels - drawingObject.TopLeft.ColumnOffsetInPixels);
			long cy = PixelsToEMU(drawingObject.BottomRight.RowOffsetInPixels - drawingObject.TopLeft.RowOffsetInPixels);
			WriteStartElement("ext", spreadsheetDrawingNamespace);
			try {
				WriteStringValue("cx", cx.ToString());
				WriteStringValue("cy", cy.ToString());
			}
			finally {
				WriteEndElement();
			}
		}
		void WritePicture(XlDrawingObjectBase drawingObject) {
			XlsxPicture picture = (XlsxPicture)drawingObject;
			WriteStartElement("pic", spreadsheetDrawingNamespace);
			try {
				WritePictureNonVisualProperties(picture);
				WriteBlipFill(picture);
				WriteShapeProperties();
			}
			finally {
				WriteEndElement();
			}
		}
		void WritePictureNonVisualProperties(XlsxPicture drawingObject) {
			WriteStartElement("nvPicPr", spreadsheetDrawingNamespace);
			try {
				WriteNonVisualDrawingProperties(drawingObject);
				WriteNonVisualPictureDrawingProperties(drawingObject);
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteNonVisualDrawingProperties(XlsxPicture drawingObject) {
			WriteStartElement("cNvPr", spreadsheetDrawingNamespace);
			try {
				WriteIntValue("id", drawingObject.PictureId);
				WriteStringValue("name", drawingObject.Name);
				WritePictureHyperlinkClick(drawingObject);
			}
			finally {
				WriteEndElement();
			}
		}
		void WritePictureHyperlinkClick(XlsxPicture drawingObject) {
			if(drawingObject.HyperlinkClick == null || string.IsNullOrEmpty(drawingObject.HyperlinkClick.TargetUri))
				return;
			string hyperlinkRelationId = GetHyperlinkRelationId(drawingObject);
			WriteStartElement("hlinkClick", drawingMLNamespace);
			try {
				WriteStringAttr("xmlns", "r", null, XlsxPackageBuilder.RelsNamespace);
				WriteStringAttr("r", "id", XlsxPackageBuilder.RelsNamespace, hyperlinkRelationId);
				if(!string.IsNullOrEmpty(drawingObject.HyperlinkClick.Tooltip))
					WriteStringValue("tooltip", EncodeXmlChars(drawingObject.HyperlinkClick.Tooltip));
				if(!string.IsNullOrEmpty(drawingObject.HyperlinkClick.TargetFrame))
					WriteStringValue("tgtFrame", drawingObject.HyperlinkClick.TargetFrame);
			}
			finally {
				WriteEndElement();
			}
		}
		string GetHyperlinkRelationId(XlsxPicture drawingObject) {
			string hyperlinkUrl = drawingObject.HyperlinkClick.TargetUri;
			string hyperlinkRelationId;
			if(exportedPictureHyperlinkTable.TryGetValue(hyperlinkUrl, out hyperlinkRelationId))
				return hyperlinkRelationId;
			hyperlinkRelationId = this.drawingRelations.GenerateId();
			exportedPictureHyperlinkTable.Add(hyperlinkUrl, hyperlinkRelationId);
			OpenXmlRelation relation = new OpenXmlRelation();
			relation.Id = hyperlinkRelationId;
			relation.Target = Uri.EscapeUriString(hyperlinkUrl);
			relation.Type = officeHyperlinkType;
			if(drawingObject.HyperlinkClick.IsExternal)
				relation.TargetMode = "External";
			drawingRelations.Add(relation);
			return hyperlinkRelationId;
		}
		void WriteNonVisualPictureDrawingProperties(XlsxPicture drawingObject) {
			WriteStartElement("cNvPicPr", spreadsheetDrawingNamespace);
			try {
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteBlipFill(XlsxPicture drawingObject) {
			WriteStartElement("blipFill", spreadsheetDrawingNamespace);
			try {
				WriteBoolValue("rotWithShape", true);
				WriteStartElement("blip", drawingMLNamespace);
				try {
					WriteStringAttr("xmlns", "r", null, XlsxPackageBuilder.RelsNamespace);
					WriteStringAttr("r", "embed", XlsxPackageBuilder.RelsNamespace, drawingObject.RelationId);
				}
				finally {
					WriteEndElement();
				}
				WriteStretch();
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteStretch() {
			WriteStartElement("stretch", drawingMLNamespace);
			try {
				WriteStartElement("fillRect", drawingMLNamespace);
				WriteEndElement();
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteShapeProperties() {
			WriteStartElement("spPr", spreadsheetDrawingNamespace);
			try {
				WritePresetGeometry();
			}
			finally {
				WriteEndElement();
			}
		}
		void WritePresetGeometry() {
			WriteStartElement("prstGeom", drawingMLNamespace);
			try {
				WriteStringValue("prst", "rect");
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteClientData(XlDrawingObjectBase drawingObject) {
			WriteStartElement("clientData", spreadsheetDrawingNamespace);
			try {
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		#region GenerateDrawingRelations
		void GenerateDrawingRelations() {
			string fileName = string.Format(@"xl\drawings\_rels\drawing{0}.xml.rels", drawingId);
			BeginWriteXmlContent();
			Builder.GenerateRelationsContent(writer, drawingRelations);
			AddPackageContent(fileName, EndWriteXmlContent());
		}
		#endregion
		long PixelsToEMU(int value) {
			return (long)((double)value * GraphicsDpi.EMU / GraphicsDpi.Pixel);
		}
	}
}
