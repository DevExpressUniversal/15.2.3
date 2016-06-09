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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office;
using System.IO;
using System.Text;
using System.Xml;
using DevExpress.Utils;
using System.Globalization;
using DevExpress.Utils.Zip;
using DevExpress.Export.Xl;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region Fields
		const string pointsFormat = "{0}pt";
		int vmlDrawingCounter;
		int shapeIdCounter = 1;
		VmlDrawing currentVmlDrawing;
		#endregion
		#region Translation tables
		internal static readonly Dictionary<VmlExtensionHandlingBehavior, string> VmlExtensionHandlingBehaviorTable = CreateVmlExtensionHandlingBehaviorTable();
		internal static readonly Dictionary<VmlObjectType, string> VmlObjectTypeTable = CreateVmlObjectTypeTable();
		internal static readonly Dictionary<VmlConnectType, string> VmlConnectTypeTable = CreateVmlConnectTypeTable();
		internal static readonly Dictionary<VmlStrokeJoinStyle, string> VmlStrokeJoinStyleTable = CreateVmlStrokeJoinStyleTable();
		internal static readonly Dictionary<VmlInsetMode, string> VmlInsetModeTable = CreateVmlInsetModeTable();
		internal static readonly Dictionary<VmlFillType, string> VmlFillTypeTable = CreateVmlFillTypeTable();
		internal static readonly Dictionary<VmlFillMethod, string> VmlFillMethodTable = CreateVmlFillMethodTable();
		internal static readonly Dictionary<VmlImageAspect, string> VmlImageAspectTable = CreateVmlImageAspectTable();
		internal static readonly Dictionary<XlHorizontalAlignment, string> VmlTextHAlignTable = CreateVmlTextHAlignTable();
		internal static readonly Dictionary<XlVerticalAlignment, string> VmlTextVAlignTable = CreateVmlTextVAlignTable();
		internal static readonly Dictionary<ClipboardFormat, string> VmlClipboardFormatTable = CreateVmlClipboardFormatTable();
		internal static readonly Dictionary<OfficeImageFormat, string> VmlOfficeImageFormatTable = CreateVmlOfficeImageFormatTable();
		static Dictionary<VmlExtensionHandlingBehavior, string> CreateVmlExtensionHandlingBehaviorTable() {
			Dictionary<VmlExtensionHandlingBehavior, string> result = new Dictionary<VmlExtensionHandlingBehavior, string>();
			result.Add(VmlExtensionHandlingBehavior.View, "view");
			result.Add(VmlExtensionHandlingBehavior.Edit, "edit");
			result.Add(VmlExtensionHandlingBehavior.BackwardCompatible, "backwardCompatible");
			return result;
		}
		static Dictionary<VmlObjectType, string> CreateVmlObjectTypeTable() {
			Dictionary<VmlObjectType, string> result = new Dictionary<VmlObjectType, string>();
			result.Add(VmlObjectType.Button, "Button");
			result.Add(VmlObjectType.Checkbox, "Checkbox");
			result.Add(VmlObjectType.Dialog, "Dialog");
			result.Add(VmlObjectType.Drop, "Drop");
			result.Add(VmlObjectType.Edit, "Edit");
			result.Add(VmlObjectType.GBox, "GBox");
			result.Add(VmlObjectType.Group, "Group");
			result.Add(VmlObjectType.Label, "Label");
			result.Add(VmlObjectType.LineA, "LineA");
			result.Add(VmlObjectType.List, "List");
			result.Add(VmlObjectType.Movie, "Movie");
			result.Add(VmlObjectType.Note, "Note");
			result.Add(VmlObjectType.Pict, "Pict");
			result.Add(VmlObjectType.Radio, "Radio");
			result.Add(VmlObjectType.Rect, "Rect");
			result.Add(VmlObjectType.RectA, "RectA");
			result.Add(VmlObjectType.Scroll, "Scroll");
			result.Add(VmlObjectType.Shape, "Shape");
			result.Add(VmlObjectType.Spin, "Spin");
			return result;
		}
		static Dictionary<VmlConnectType, string> CreateVmlConnectTypeTable() {
			Dictionary<VmlConnectType, string> result = new Dictionary<VmlConnectType, string>();
			result.Add(VmlConnectType.Custom, "custom");
			result.Add(VmlConnectType.None, "none");
			result.Add(VmlConnectType.Rect, "rect");
			result.Add(VmlConnectType.Segments, "segments");
			return result;
		}
		static Dictionary<VmlStrokeJoinStyle, string> CreateVmlStrokeJoinStyleTable() {
			Dictionary<VmlStrokeJoinStyle, string> result = new Dictionary<VmlStrokeJoinStyle, string>();
			result.Add(VmlStrokeJoinStyle.Bevel, "bevel");
			result.Add(VmlStrokeJoinStyle.Miter, "miter");
			result.Add(VmlStrokeJoinStyle.Round, "round");
			return result;
		}
		static Dictionary<VmlInsetMode, string> CreateVmlInsetModeTable() {
			Dictionary<VmlInsetMode, string> result = new Dictionary<VmlInsetMode, string>();
			result.Add(VmlInsetMode.Auto, "auto");
			result.Add(VmlInsetMode.Custom, "custom");
			return result;
		}
		static Dictionary<VmlFillType, string> CreateVmlFillTypeTable() {
			Dictionary<VmlFillType, string> result = new Dictionary<VmlFillType, string>();
			result.Add(VmlFillType.Frame, "frame");
			result.Add(VmlFillType.Gradient, "gradient");
			result.Add(VmlFillType.GradientRadial, "gradientRadial");
			result.Add(VmlFillType.Pattern, "pattern");
			result.Add(VmlFillType.Solid, "solid");
			result.Add(VmlFillType.Tile, "tile");
			return result;
		}
		static Dictionary<VmlFillMethod, string> CreateVmlFillMethodTable() {
			Dictionary<VmlFillMethod, string> result = new Dictionary<VmlFillMethod, string>();
			result.Add(VmlFillMethod.Any, "any");
			result.Add(VmlFillMethod.Linear, "linear");
			result.Add(VmlFillMethod.LinearSigma, "linear sigma");
			result.Add(VmlFillMethod.None, "none");
			result.Add(VmlFillMethod.Sigma, "sigma");
			return result;
		}
		static Dictionary<VmlImageAspect, string> CreateVmlImageAspectTable() {
			Dictionary<VmlImageAspect, string> result = new Dictionary<VmlImageAspect, string>();
			result.Add(VmlImageAspect.AtLeast, "atLeast");
			result.Add(VmlImageAspect.AtMost, "atMost");
			result.Add(VmlImageAspect.Ignore, "ignore");
			return result;
		}
		static Dictionary<XlHorizontalAlignment, string> CreateVmlTextHAlignTable() {
			Dictionary<XlHorizontalAlignment, string> result = new Dictionary<XlHorizontalAlignment, string>();
			result.Add(XlHorizontalAlignment.Left, "Left");
			result.Add(XlHorizontalAlignment.Justify, "Justify");
			result.Add(XlHorizontalAlignment.Center, "Center");
			result.Add(XlHorizontalAlignment.Right, "Right");
			result.Add(XlHorizontalAlignment.Distributed, "Distributed");
			return result;
		}
		static Dictionary<XlVerticalAlignment, string> CreateVmlTextVAlignTable() {
			Dictionary<XlVerticalAlignment, string> result = new Dictionary<XlVerticalAlignment, string>();
			result.Add(XlVerticalAlignment.Top, "Top");
			result.Add(XlVerticalAlignment.Justify, "Justify");
			result.Add(XlVerticalAlignment.Center, "Center");
			result.Add(XlVerticalAlignment.Bottom, "Bottom");
			result.Add(XlVerticalAlignment.Distributed, "Distributed");
			return result;
		}
		static Dictionary<ClipboardFormat, string> CreateVmlClipboardFormatTable() {
			Dictionary<ClipboardFormat, string> result = new Dictionary<ClipboardFormat, string>();
			result.Add(ClipboardFormat.Bitmap, "Bitmap");
			result.Add(ClipboardFormat.Pict, "Pict");
			result.Add(ClipboardFormat.PictOld, "PictOld");
			result.Add(ClipboardFormat.PictPrint, "PictPrint");
			result.Add(ClipboardFormat.PictScreen, "PictScreen");
			return result;
		}
		static Dictionary<OfficeImageFormat, string> CreateVmlOfficeImageFormatTable() {
			Dictionary<OfficeImageFormat, string> result = new Dictionary<OfficeImageFormat, string>();
			result.Add(OfficeImageFormat.Jpeg, "image/jpeg");
			result.Add(OfficeImageFormat.Png, "image/png");
			result.Add(OfficeImageFormat.Bmp, "image/bitmap");
			result.Add(OfficeImageFormat.Tiff, "image/tiff");
			result.Add(OfficeImageFormat.Gif, "image/gif");
			result.Add(OfficeImageFormat.Icon, "image/x-icon");
			result.Add(OfficeImageFormat.Wmf, "application/x-msmetafile"); 
			result.Add(OfficeImageFormat.Emf, "image/x-emf");
			return result;
		}
		#endregion
		protected internal void SetCurrentVmlDrawing(VmlDrawing drawing) {
			this.currentVmlDrawing = drawing;
		}
		protected internal void SetShapeIdCounter(int value) {
			this.shapeIdCounter = value;
		}
		protected internal virtual void GenerateLegacyDrawingContent() {
			if (!ShouldExportVmlDrawing(ActiveSheet))
				return;
			WriteShStartElement("legacyDrawing");
			try {
				string id = PopulateVmlDrawingsTable(ActiveSheet);
				WriteStringAttr(RelsPrefix, "id", null, id);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual bool ShouldExportVmlDrawing(Worksheet sheet) {
			return (sheet.VmlDrawing != null && sheet.VmlDrawing.Shapes.Count > 0) || sheet.Comments.Count > 0;
		}
		protected internal virtual string PopulateVmlDrawingsTable(Worksheet sheet) {
			this.vmlDrawingCounter++;
			string fileName = String.Format("vmlDrawing{0}.vml", this.vmlDrawingCounter);
			string relationTarget = "../drawings/" + fileName;
			OpenXmlRelationCollection relations = SheetRelationsTable[ActiveSheet.Name];
			string id = GenerateIdByCollection(relations);
			OpenXmlRelation relation = new OpenXmlRelation(id, relationTarget, RelsVmlDrawingNamepace);
			relations.Add(relation);
			VmlDrawing drawing = sheet.VmlDrawing;
			string path = @"xl\drawings\" + fileName;
			VmlDrawingPathsTable.Add(drawing, path);
			return id;
		}
		protected internal virtual void AddVmlDrawingPackageContent() {
			foreach (KeyValuePair<VmlDrawing, string> pair in VmlDrawingPathsTable) {
				AddPackageContent(pair.Value, ExportVmlDrawing(pair.Key));
			}
		}
		protected internal virtual CompressedStream ExportVmlDrawing(VmlDrawing drawing) {
			this.currentVmlDrawing = drawing;
			XmlWriterSettings xmlWriterSettings = CreateVmlWriterSettings();
			return CreateXmlContent(GenerateVmlDrawingXmlContent, xmlWriterSettings);
		}
		protected internal XmlWriterSettings CreateVmlWriterSettings() {
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true; 
			settings.Encoding = DXEncoding.UTF8NoByteOrderMarks;
			settings.CheckCharacters = true;
			settings.OmitXmlDeclaration = false;
			return settings;
		}
		protected internal virtual void GenerateVmlDrawingXmlContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			GenerateVmlDrawingContent();
		}
		protected internal virtual void GenerateVmlDrawingContent() {
			WriteStartElement("xml", null);
			try {
				GenerateVmlDrawingAttributesContent();
				GenerateVmlShapeLayoutContent(currentVmlDrawing.ShapeLayout);
				GenerateVmlShapeTypesContent(currentVmlDrawing.ShapeTypes);
				GenerateVmlShapesContent(currentVmlDrawing);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual void GenerateVmlDrawingAttributesContent() {
			WriteStringAttr("xmlns", "v", null, VmlDrawingNamespace);
			WriteStringAttr("xmlns", "o", null, OfficeNamespaceConst);
			WriteStringAttr("xmlns", "x", null, VmlDrawingExcelNamespace);
		}
		protected internal virtual void GenerateVmlShapeLayoutContent(VmlShapeLayout shapeLayout) {
			WriteStartElement("shapelayout", OfficeNamespaceConst);
			try {
				WriteStringAttr(String.Empty, "ext", VmlDrawingNamespace, VmlExtensionHandlingBehaviorTable[shapeLayout.ExtensionHandlingBehavior]);
				GenerateVmlShapeIdMapContent(shapeLayout.Idmap);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual void GenerateVmlShapeIdMapContent(VmlShapeIdMap idMap) {
			WriteStartElement("idmap", OfficeNamespaceConst);
			try {
				WriteStringAttr(String.Empty, "ext", VmlDrawingNamespace, VmlExtensionHandlingBehaviorTable[idMap.ExtensionHandlingBehavior]);
				if (!String.IsNullOrEmpty(idMap.Data))
					WriteStringValue("data", idMap.Data);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual void GenerateVmlShapeTypesContent(VmlShapeTypeCollection shapeTypes) {
			shapeTypes.ForEach(GenerateVmlShapeTypeContent);
		}
		protected internal virtual void GenerateVmlShapeTypeContent(VmlShapeType shapeType) {
			if (!ShouldExportShapeType(shapeType))
				return;
			GenerateVmlShapeTypeContentCore(shapeType);
		}
		protected internal virtual void GenerateVmlShapeTypeContentCore(VmlShapeType shapeType) {
			WriteStartElement("shapetype", VmlDrawingNamespace);
			try {
				if (!String.IsNullOrEmpty(shapeType.Id))
					WriteStringValue("id", shapeType.Id);
				if (shapeType.CoordsizeX != 1000 || shapeType.CoordsizeY != 1000)
					WriteStringValue("coordsize", string.Concat(shapeType.CoordsizeX, ',', shapeType.CoordsizeY));
				if (shapeType.Spt != 0)
					WriteStringAttr(String.Empty, "spt", OfficeNamespaceConst, shapeType.Spt.ToString(CultureInfo.InvariantCulture));
				if (!String.IsNullOrEmpty(shapeType.Path))
					WriteStringValue("path", shapeType.Path);
				GenerateVmlLineStrokeSettingsContent(shapeType.Stroke);
				GenerateVmlSingleFormulasCollectionContent(shapeType.Formulas);
				GenerateVmlShapePathContent(shapeType.ShapePath);
				GenerateVmlShapeProtectionsContent(shapeType.ShapeProtections);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual void GenerateVmlShapeProtectionsContent(VmlShapeProtections protections) {
			if (protections == null)
				return;
			WriteStartElement("lock", VmlDrawingOfficeNamespace);
			try {
				WriteEnumValue("ext", VmlDrawingNamespace, protections.Ext, VmlExtensionHandlingBehaviorTable);
				WriteVmlNullableBoolAttr("position", protections.Position);
				WriteVmlNullableBoolAttr("selection", protections.Selection);
				WriteVmlNullableBoolAttr("grouping", protections.Grouping);
				WriteVmlNullableBoolAttr("ungrouping", protections.Ungrouping);
				WriteVmlNullableBoolAttr("rotation", protections.Rotation);
				WriteVmlNullableBoolAttr("cropping", protections.Cropping);
				WriteVmlNullableBoolAttr("verticies", protections.Verticies);
				WriteVmlNullableBoolAttr("adjusthandles", protections.Adjusthandles);
				WriteVmlNullableBoolAttr("text", protections.Text);
				WriteVmlNullableBoolAttr("aspectratio", protections.Aspectratio);
				WriteVmlNullableBoolAttr("shapetype", protections.Shapetype);			   
			} finally {
				WriteEndElement();
			}
		}
		protected internal virtual void GenerateVmlSingleFormulasCollectionContent(VmlSingleFormulasCollection formulas) {
			if (formulas == null)
				return;
			WriteStartElement("formulas", VmlDrawingNamespace);
			try {
				formulas.ForEach(GenerateVmlSingleFormulaContent);
			} finally {
				WriteEndElement();
			}
		}
		protected internal virtual void GenerateVmlSingleFormulaContent(VmlSingleFormula formula) {
			WriteStartElement("f", VmlDrawingNamespace);
			try {
				WriteStringValue("eqn", formula.Equation);
			} finally {
				WriteEndElement();
			}
		}
		protected internal virtual bool ShouldExportShapeType(VmlShapeType shapeType) {
			string shapeTypeId = shapeType.Id;
			VmlShapeCollection shapes = currentVmlDrawing.Shapes;
			if (shapes.Count <= 0)
				return false;
			foreach (VmlShape shape in shapes) {
				if (ShouldExportVmlShape(shape) && StringExtensions.CompareInvariantCultureIgnoreCase(shape.Type, shapeTypeId) == 0)
					return true;
			}
			return false;
		}
		protected internal virtual void GenerateVmlLineStrokeSettingsContent(VmlLineStrokeSettings stroke) {
			WriteStartElement("stroke", VmlDrawingNamespace);
			try {
				WriteStringAttr(String.Empty, "joinstyle", null, VmlStrokeJoinStyleTable[stroke.Joinstyle]);
				if (!String.IsNullOrEmpty(stroke.Title))
					WriteStringAttr(String.Empty, "title", OfficeNamespaceConst, stroke.Title);
				if (stroke.Filltype != VmlFillType.Solid)
					WriteStringAttr(String.Empty, "filltype", null, VmlFillTypeTable[stroke.Filltype]);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual void GenerateVmlShapePathContent(VmlShapePath shapePath) {
			WriteStartElement("path", VmlDrawingNamespace);
			try {
				if (shapePath.Gradientshapeok)
					WriteVmlBoolAttr("gradientshapeok", null, shapePath.Gradientshapeok);
				WriteStringAttr(String.Empty, "connecttype", OfficeNamespaceConst, VmlConnectTypeTable[shapePath.Connecttype]);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual void GenerateVmlShapesContent(VmlDrawing drawing) {
			foreach (VmlShape shape in drawing.Shapes)
				if (ShouldExportVmlShape(shape))
					GenerateVmlShapeContent(shape);
		}
		protected internal virtual bool ShouldExportVmlShape(VmlShape shape) {
			VmlClientData clientData = shape.ClientData;
			if (clientData == null)
				return false;
			return 
				(clientData.Column >= 0 && clientData.Row >= 0) ||
				(clientData.CameraTool && !String.IsNullOrEmpty(clientData.CameraSourceReference)); 
		}
		protected internal virtual void GenerateVmlShapeContent(VmlShape shape) {
			WriteStartElement("shape", VmlDrawingNamespace);
			try {
				GenerateVmlShapeAttributesContent(shape);
				GenerateVmlFillPropertiesContent(shape.Fill);
				GenerateVmlShadowEffectContent(shape.Shadow);
				GenerateVmlShapePathContent(shape.Path);
				GenerateVmlTextBoxContent(shape.Textbox);
				GenerateVmlImageDataContent(shape.ImageData);
				GenerateVmlClientDataContent(shape.ClientData);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual void GenerateVmlImageDataContent(VmlShapeImageData imageData) {
			if (imageData == null)
				return;
			WriteStartElement("imagedata", VmlDrawingNamespace);
			try {
				if (!String.IsNullOrEmpty(imageData.Althref))
					WriteStringAttr(String.Empty, "althref", VmlDrawingOfficeNamespace, imageData.Althref);
				if (imageData.Bilevel.HasValue)
					WriteStringValue("bilevel", GetVmlBooleanShortStringValue(imageData.Bilevel.Value));
				WriteStringValue("blacklevel", imageData.Blacklevel, !String.IsNullOrEmpty(imageData.Blacklevel));
				if (!imageData.Chromakey.IsEmpty)
					WriteStringValue("chromakey", DXColor.ToHtml(imageData.Chromakey));
				WriteStringValue("cropbottom", imageData.Cropbottom, !String.IsNullOrEmpty(imageData.Cropbottom));
				WriteStringValue("cropleft", imageData.Cropleft, !String.IsNullOrEmpty(imageData.Cropleft));
				WriteStringValue("cropright", imageData.Cropright, !String.IsNullOrEmpty(imageData.Cropright));
				WriteStringValue("croptop", imageData.Croptop, !String.IsNullOrEmpty(imageData.Croptop));
				if(imageData.Detectmouseclick.HasValue)
					WriteVmlBoolAttr("detectmouseclick", VmlDrawingOfficeNamespace, imageData.Detectmouseclick.Value);
				if(!imageData.Embosscolor.IsEmpty)
					WriteStringValue("embosscolor", DXColor.ToHtml(imageData.Embosscolor));
				WriteStringValue("gain", imageData.Gain, !String.IsNullOrEmpty(imageData.Gain));
				WriteStringValue("gamma", imageData.Gamma, !String.IsNullOrEmpty(imageData.Gamma));
				if (imageData.Grayscale.HasValue)
					WriteStringValue("grayscale", GetVmlBooleanShortStringValue(imageData.Grayscale.Value));
				if (!String.IsNullOrEmpty(imageData.Href))
					WriteStringAttr(String.Empty, "href", VmlDrawingOfficeNamespace, imageData.Href);
				WriteStringValue("id", imageData.Id, !String.IsNullOrEmpty(imageData.Id));
				if (imageData.Movie.HasValue)
					WriteStringAttr(String.Empty, "movie",VmlDrawingOfficeNamespace, imageData.Movie.Value.ToString(CultureInfo.InvariantCulture));
				if (imageData.Oleid.HasValue)
					WriteStringAttr(String.Empty, "oleid", VmlDrawingOfficeNamespace, imageData.Oleid.Value.ToString(CultureInfo.InvariantCulture));
				if(!imageData.Recolortarget.IsEmpty)
					WriteStringValue("recolortarget", DXColor.ToHtml(imageData.Recolortarget));
				WriteStringValue("src", imageData.Src, !String.IsNullOrEmpty(imageData.Src));				
				if (imageData.Image != null) 
					ExportVmlOfficeImage(imageData.Image);
				if (imageData.Title != null)
					WriteStringAttr(String.Empty, "title", VmlDrawingOfficeNamespace, imageData.Title);
			} finally {
				WriteEndElement();
			}
		}
		void ExportVmlOfficeImage(OfficeImage officeImage) {
			string imageRelationId = ExportVmlOfficeImageCore(Workbook, officeImage);
			string fileName = String.Format("vmlDrawing{0}.vml", this.vmlDrawingCounter);
			if (!DrawingRelationPathTable.ContainsKey(fileName)) {
				DrawingRelationPathTable.Add(fileName, "xl\\drawings\\_rels\\" + fileName + ".rels");
				DrawingRelationsTable.Add(fileName, currentRelations);
			}
			WriteStringAttr(String.Empty, "relid", VmlDrawingOfficeNamespace, imageRelationId);
		}
		string ExportVmlOfficeImageCore(DocumentModel workbook, OfficeImage officeImage) {
			string imageRelationId;
			OfficeImage rootImage = officeImage.RootImage;
			if (ExportedImageTable.TryGetValue(rootImage, out imageRelationId))
				return imageRelationId;
			OpenXmlRelation relation = new OpenXmlRelation();
			string imageId = GenerateImageId();
			string imageFileName = ExportVmlOfficeImageCore(rootImage, imageId);
			imageRelationId = GenerateDrawingRelationId();
			ExportedImageTable.Add(rootImage, imageRelationId);
			relation.Id = imageRelationId;
			relation.Target = "../media/" + imageFileName;
			relation.Type = RelsImagesNamespace;
			currentRelations.Add(relation);
			return imageRelationId;
		}
		string ExportVmlOfficeImageCore(OfficeImage image, string imageId) {
			string extenstion = GetImageExtension(image);
			string contentType;
			if (!Builder.UsedContentTypes.TryGetValue(extenstion, out contentType)) {
				if (!VmlOfficeImageFormatTable.TryGetValue(image.RawFormat, out contentType))
					contentType = VmlOfficeImageFormatTable[OfficeImageFormat.Png];
				Builder.UsedContentTypes.Add(extenstion, contentType);
			}
			string imageFileName;
			if (ExportedImages.TryGetValue(image, out imageFileName))
				return imageFileName;
			imageFileName = imageId + "." + extenstion;
			byte[] imageBytes = image.GetImageBytesSafe(image.RawFormat);
			Package.Add(RootImagePath + @"\media\" + imageFileName, Builder.Now, imageBytes);
			ExportedImages.Add(image, imageFileName);
			return imageFileName;
		}
		protected internal virtual void GenerateVmlShapeAttributesContent(VmlShape shape) {
			WriteIntValue("id", shapeIdCounter++);
			if (!String.IsNullOrEmpty(shape.Type))
				WriteStringValue("type", "#" + shape.Type);
			if (!String.IsNullOrEmpty(shape.Style))
				WriteStringValue("style", shape.Style);
			if (shape.Filled.HasValue)
				WriteVmlBoolAttr("filled", null, shape.Filled.Value);
			if(shape.Fillcolor != DXColor.White)
				WriteStringValue("fillcolor", DXColor.ToHtml(shape.Fillcolor));
			if(shape.Strokecolor != DXColor.Black)
				WriteStringValue("strokecolor", DXColor.ToHtml(shape.Strokecolor));
			if(shape.Strokeweight != Workbook.UnitConverter.PointsToModelUnits(1)) {
				float strokeWeight = Workbook.UnitConverter.ModelUnitsToPointsF(shape.Strokeweight);
				WriteStringValue("strokeweight", strokeWeight.ToString(CultureInfo.InvariantCulture) + "pt");
			}
			if(!shape.Stroked)
				WriteVmlBoolAttr("stroked", null, shape.Stroked);
			if(shape.InsetMode != VmlInsetMode.Custom)
				WriteStringAttr(String.Empty, "insetmode", OfficeNamespaceConst, VmlInsetModeTable[shape.InsetMode]);
		}
		protected internal virtual void GenerateVmlFillPropertiesContent(VmlShapeFillProperties properties) {
			WriteStartElement("fill", VmlDrawingNamespace);
			try {
				if (properties.Color != DXColor.White)
					WriteStringValue("color", DXColor.ToHtml(properties.Color));
				if (properties.Color2 != DXColor.White)
					WriteStringValue("color2", DXColor.ToHtml(properties.Color2));
				if (properties.Opacity != 1)
					WriteStringAttr(String.Empty, "opacity", null, properties.Opacity.ToString(CultureInfo.InvariantCulture));
				if (properties.Opacity2 != 1)
					WriteStringAttr(String.Empty, "opacity2", OfficeNamespaceConst, properties.Opacity2.ToString(CultureInfo.InvariantCulture));
				if (properties.Recolor)
					WriteVmlBoolAttr("recolor", null, properties.Recolor);
				if (properties.Rotate)
					WriteVmlBoolAttr("rotate", null, properties.Rotate);
				if (properties.Method != VmlFillMethod.Sigma)
					WriteStringAttr(String.Empty, "method", null, VmlFillMethodTable[properties.Method]);
				if (properties.Type != VmlFillType.Solid)
					WriteStringAttr(String.Empty, "type", null, VmlFillTypeTable[properties.Type]);
				if (properties.Focus != 0)
					WritePercentageAttr("focus", properties.Focus);
				if (!String.IsNullOrEmpty(properties.Title))
					WriteStringAttr(String.Empty, "title", OfficeNamespaceConst, properties.Title);
				if (properties.OriginX != 0 || properties.OriginY != 0)
					WriteStringAttr(null, "origin", null, String.Concat(properties.OriginX, ',', properties.OriginY));
				if (properties.SizeX != 0 || properties.SizeY != 0)
					WriteStringAttr(null, "size", null, String.Concat(properties.SizeX, ',', properties.SizeY));
				if (properties.PositionX != 0 || properties.PositionY != 0)
					WriteStringAttr(null, "position", null, String.Concat(properties.PositionX, ',', properties.PositionY));
				if (properties.Aspect != VmlImageAspect.Ignore)
					WriteStringAttr(String.Empty, "aspect", null, VmlImageAspectTable[properties.Aspect]);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual void GenerateVmlShadowEffectContent(VmlShadowEffect shadow) {
			if (shadow.IsDefault)
				return;
			WriteStartElement("shadow", VmlDrawingNamespace);
			try {
				if (shadow.Color != DXColor.Gray)
					WriteStringValue("color", DXColor.ToHtml(shadow.Color));
				if (shadow.Obscured)
					WriteVmlBoolAttr("obscured", null, shadow.Obscured);
				WriteVmlBoolAttr("on", null, shadow.On);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual void GenerateVmlTextBoxContent(VmlTextBox textbox) {
			WriteStartElement("textbox", VmlDrawingNamespace);
			try {
				if (!String.IsNullOrEmpty(textbox.Style))
					WriteStringAttr(String.Empty, "style", null, textbox.Style);
				string inset = GetInsetString(textbox.Inset, Workbook);
				if (!String.IsNullOrEmpty(inset))
					WriteStringAttr(String.Empty, "inset", null, inset);
				if (!String.IsNullOrEmpty(textbox.HtmlContent))
					DocumentContentWriter.WriteRaw(textbox.HtmlContent);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual void GenerateVmlClientDataContent(VmlClientData clientData) {
			WriteStartElement("ClientData", VmlDrawingExcelNamespace);
			try {
				WriteStringAttr(String.Empty, "ObjectType", null, VmlObjectTypeTable[clientData.ObjectType]);
				if(!clientData.MoveWithCells)
					WriteVmlTrueFalseBlankValue("MoveWithCells", VmlDrawingExcelNamespace, clientData.MoveWithCells, false);
				if(!clientData.SizeWithCells)
					WriteVmlTrueFalseBlankValue("SizeWithCells", VmlDrawingExcelNamespace, clientData.SizeWithCells, false);
				if(!clientData.AutoFill)
					WriteVmlTrueFalseBlankValue("AutoFill", VmlDrawingExcelNamespace, clientData.AutoFill, true);
				if (clientData.Anchor != null)
					WriteString("Anchor", VmlDrawingExcelNamespace, clientData.Anchor.ToString());
				if (clientData.Row >= 0)
					WriteString("Row", VmlDrawingExcelNamespace, clientData.Row.ToString());
				if (clientData.Column >= 0)
					WriteString("Column", VmlDrawingExcelNamespace, clientData.Column.ToString());
				if (!clientData.Locked)
					WriteVmlTrueFalseBlankValue("Locked", VmlDrawingExcelNamespace, clientData.Locked, true);
				if (!clientData.LockText)
					WriteVmlTrueFalseBlankValue("LockText", VmlDrawingExcelNamespace, clientData.LockText, true);
				if (clientData.Visible)
					WriteVmlTrueFalseBlankValue("Visible", VmlDrawingExcelNamespace, clientData.Visible, true);
				if(clientData.TextHAlign != XlHorizontalAlignment.Left)
					WriteString("TextHAlign", VmlDrawingExcelNamespace, VmlTextHAlignTable[clientData.TextHAlign]);
				if(clientData.TextVAlign != XlVerticalAlignment.Top)
					WriteString("TextVAlign", VmlDrawingExcelNamespace, VmlTextVAlignTable[clientData.TextVAlign]);
				if (!String.IsNullOrEmpty(clientData.CameraSourceReference))
					WriteString("FmlaPict", VmlDrawingExcelNamespace, clientData.CameraSourceReference);
				if (clientData.ClipboardFormat != ClipboardFormat.Empty)
					WriteString("CF", VmlDrawingExcelNamespace, VmlClipboardFormatTable[clientData.ClipboardFormat]);
				if (clientData.CameraTool)
					WriteVmlTrueFalseBlankValue("Camera", VmlDrawingExcelNamespace, clientData.CameraTool, true);
			}
			finally {
				WriteEndElement();
			}
		}
		string GetInsetString(VmlInsetData data, DocumentModel workbook) {
			DocumentModelUnitConverter unitConverter = workbook.UnitConverter;
			int defaultMarginX = (int)unitConverter.InchesToModelUnitsF(0.1f);
			int defaultMarginY = (int)unitConverter.InchesToModelUnitsF(0.05f);
			StringBuilder sb = new StringBuilder();
			if(data.LeftMargin != defaultMarginX)
				sb.AppendFormat(CultureInfo.InvariantCulture, pointsFormat, unitConverter.ModelUnitsToPointsF(data.LeftMargin));
			sb.Append(",");
			if(data.TopMargin != defaultMarginY)
				sb.AppendFormat(CultureInfo.InvariantCulture, pointsFormat, unitConverter.ModelUnitsToPointsF(data.TopMargin));
			sb.Append(",");
			if(data.RightMargin != defaultMarginX)
				sb.AppendFormat(CultureInfo.InvariantCulture, pointsFormat, unitConverter.ModelUnitsToPointsF(data.RightMargin));
			sb.Append(",");
			if(data.BottomMargin != defaultMarginY)
				sb.AppendFormat(CultureInfo.InvariantCulture, pointsFormat, unitConverter.ModelUnitsToPointsF(data.BottomMargin));
			string result = sb.ToString();
			if(result == ",,,")
				result = string.Empty;
			return result;
		}
	}
}
