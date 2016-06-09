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
using System.Collections.Generic;
using DevExpress.Utils;
using System.Drawing;
using DevExpress.XtraRichEdit.Export.OpenXml;
using DevExpress.XtraRichEdit.Import.Html;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
#if !SL
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	#region InlinePictureDestination
	public class InlinePictureDestination : InlineObjectDestination {
		public InlinePictureDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
	}
	#endregion
	#region VMLShapeDestination
	public class VMLShapeDestination : ElementDestination{
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		readonly InlineObjectDestination inlineObjectDestination;
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("imagedata", OnImageData);
			result.Add("wrap", OnWrap);
			result.Add("lock", OnLock);
			result.Add("textbox", OnTextBox);
			result.Add("anchorlock", OnAnchorLock);
			return result;
		}
		bool isStroked;
		bool isFilled;
		Color outlineColor;
		Color fillColor;
		float outlineWidth;
		public VMLShapeDestination(WordProcessingMLBaseImporter importer, InlineObjectDestination inlineObjectDestination)
			: base(importer) {
			Guard.ArgumentNotNull(inlineObjectDestination, "inlineObjectDestination");
			this.inlineObjectDestination = inlineObjectDestination;
		}
		public FloatingObjectProperties FloatingObject { get { return FloatingObjectImportInfo.FloatingObjectProperties; } }
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public OfficeImage Image { get { return FloatingObjectImportInfo.Image; } set { FloatingObjectImportInfo.Image = value; } }
		public FloatingObjectImportInfo FloatingObjectImportInfo { get { return inlineObjectDestination.FloatingObjectImportInfo; } }
		public string Style { get { return inlineObjectDestination.Style; } }
		static VMLShapeDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (VMLShapeDestination)importer.PeekDestination();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			inlineObjectDestination.Style = reader.GetAttribute("style");
			ReadFloatingObjectProperties(reader);
			ReadShapeProperties(reader);
			string name = reader.GetAttribute("id");
			if (!String.IsNullOrEmpty(name) && String.IsNullOrEmpty(FloatingObjectImportInfo.Name))
				FloatingObjectImportInfo.Name = name;
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			Shape shape = FloatingObjectImportInfo.Shape;
			shape.BeginUpdate();
			try {
				ApplyShapeProperties(shape);
			}
			finally {
				shape.EndUpdate();
			}
		}
		void ApplyShapeProperties(Shape shape) {
			if (FloatingObjectImportInfo.IsTextBox)
				ApplyTextBoxShapeProperties(shape);
			else
				ApplyPictureShapeProperties(shape);
		}
		void ApplyTextBoxShapeProperties(Shape shape) {
			if (isStroked) {
				if (outlineColor == DXColor.Empty)
					outlineColor = DXColor.Black;
				shape.OutlineColor = outlineColor;
			}
			if (isFilled) {
				if (fillColor != DXColor.Empty)
					shape.FillColor = fillColor;
			}
			if (outlineWidth == float.MinValue)
				outlineWidth = 0.75f;
			shape.OutlineWidth = (int)Math.Round(UnitConverter.PointsToModelUnitsF(outlineWidth));
		}
		void ApplyPictureShapeProperties(Shape shape) {
			if (isStroked) {
				if (outlineColor != DXColor.Empty)
					shape.OutlineColor = outlineColor;
			}
			if (isFilled) {
				if (fillColor != DXColor.Empty)
					shape.FillColor = fillColor;
			}
			if (outlineWidth != float.MinValue)
				shape.OutlineWidth = (int)Math.Round(UnitConverter.PointsToModelUnitsF(outlineWidth));
		}
		void ReadFloatingObjectProperties(XmlReader reader) {
			FloatingObjectProperties properties = FloatingObjectImportInfo.FloatingObjectProperties;
			properties.BeginUpdate();
			try {
				ReadFloatingObjectProperties(reader, properties);
			}
			finally {
				properties.EndUpdate();
			}
		}
		void ReadShapeProperties(XmlReader reader) {
			Shape shape = FloatingObjectImportInfo.Shape;
			shape.BeginUpdate();
			try {
				ReadShapeProperties(reader, shape);
			}
			finally {
				shape.EndUpdate();
			}
		}
		void ReadFloatingObjectProperties(XmlReader reader, FloatingObjectProperties properties) {
			string typeId = reader.GetAttribute("type");
			if (!String.IsNullOrEmpty(typeId)) {
				bool useLockAspectRatio;
				bool lockAspectRatio = Importer.LockAspectRatioTableGetValue(typeId, out useLockAspectRatio);
				if (useLockAspectRatio)
					properties.LockAspectRatio = lockAspectRatio;
			}
			if (!String.IsNullOrEmpty(reader.GetAttribute("side")))
				properties.TextWrapSide = Importer.GetEnumValue(reader, "wrapText", OpenXmlExporter.floatingObjectTextWrapSideTable, FloatingObjectTextWrapSide.Both);
			string layoutInTableCell = reader.GetAttribute("allowincell", OpenXmlExporter.OfficeNamespaceConst);
			if (!String.IsNullOrEmpty(layoutInTableCell))
				properties.LayoutInTableCell = GetBoolValue(layoutInTableCell);
			else
				properties.LayoutInTableCell = true;
			string allowOverlap = reader.GetAttribute("allowoverlap", OpenXmlExporter.OfficeNamespaceConst);
			if (!String.IsNullOrEmpty(allowOverlap))
				properties.AllowOverlap = GetBoolValue(allowOverlap);
		}
		void ReadShapeProperties(XmlReader reader, Shape shape) {
			string strokedAttribute = reader.GetAttribute("stroked");
			this.isStroked = strokedAttribute != "f" && strokedAttribute != "false";
			string filledAttribute = reader.GetAttribute("filled");
			this.isFilled = filledAttribute != "f" && filledAttribute != "false";
			this.outlineColor = Importer.GetMSWordColorValue(reader, "strokecolor");
			this.fillColor = Importer.GetMSWordColorValue(reader, "fillcolor");
			this.outlineWidth = Importer.GetFloatValueInPoints(reader, "strokeweight", float.MinValue);
		}
		bool GetBoolValue(string value) {
			return value == "t";
		}
		static Destination OnImageData(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new VMLImageDataDestination(importer, GetThis(importer).FloatingObjectImportInfo, GetThis(importer).Style);
		}
		static Destination OnWrap(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DevExpress.XtraRichEdit.Import.WordML.WrapDestination(importer, (GetThis(importer)).FloatingObject);
		}
		static Destination OnLock(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ShapeLockDestination(importer, GetThis(importer).FloatingObject);
		}
		static Destination OnTextBox(WordProcessingMLBaseImporter importer, XmlReader reader) {
			GetThis(importer).FloatingObjectImportInfo.IsTextBox = true;
			return new TextBoxDestination(importer, GetThis(importer).FloatingObjectImportInfo);
		}
		static Destination OnAnchorLock(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new AnchorLockDestination(importer, GetThis(importer).FloatingObject);
		}
	}
	#endregion
	#region AnchorLockDestination
	public class AnchorLockDestination : LeafElementDestination {
		readonly FloatingObjectProperties floatingObjectProperties;
		public AnchorLockDestination(WordProcessingMLBaseImporter importer, FloatingObjectProperties floatingObjectProperties)
			: base(importer) {
			this.floatingObjectProperties = floatingObjectProperties;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			floatingObjectProperties.Locked = true;
		}
	}
	#endregion
	#region ShapeLockDestination
	public class ShapeLockDestination : LeafElementDestination {
		readonly FloatingObjectProperties floatingObjectProperties;
		public ShapeLockDestination(WordProcessingMLBaseImporter importer, FloatingObjectProperties floatingObjectProperties)
			: base(importer) {
			this.floatingObjectProperties = floatingObjectProperties;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string aspectRatioValue = reader.GetAttribute("aspectratio");
			if (!String.IsNullOrEmpty(aspectRatioValue))
				floatingObjectProperties.LockAspectRatio = (aspectRatioValue == "t");
		}
	}
	#endregion
	#region LockAspectRatioTable
	public class LockAspectRatioTable : Dictionary<string, string> { }
	#endregion
	#region VMLImageDataDestination
	public class VMLImageDataDestination : LeafElementDestination {
		readonly FloatingObjectImportInfo floatingObjectImportInfo;
		string style;
		public VMLImageDataDestination(WordProcessingMLBaseImporter importer, FloatingObjectImportInfo floatingObjectImportInfo)
			: this(importer, floatingObjectImportInfo, String.Empty) { }
		public VMLImageDataDestination(WordProcessingMLBaseImporter importer, FloatingObjectImportInfo floatingObjectImportInfo, string style)
			: base(importer) {
			Guard.ArgumentNotNull(floatingObjectImportInfo, "floatingObjectImportInfo");
			this.floatingObjectImportInfo = floatingObjectImportInfo;
			this.style = style;
		}
		public FloatingObjectImportInfo FloatingObjectImportInfo { get { return floatingObjectImportInfo; } }
		public override void ProcessElementOpen(XmlReader reader) {
			string id = reader.GetAttribute("id", OpenXmlExporter.RelsNamespace);
			if (String.IsNullOrEmpty(id) || !Importer.DocumentModel.DocumentCapabilities.InlinePicturesAllowed)	   
				return;
			ProcessElementOpenCore(id);
		}
		void ProcessElementOpenCore(string id) {
			OpenXmlImporter openXmlImporter = (OpenXmlImporter)Importer;
			OfficeImage image = null;
			string fileName = openXmlImporter.LookupRelationTargetById(openXmlImporter.DocumentRelations, id, openXmlImporter.DocumentRootFolder, String.Empty);
			if (Path.GetExtension(fileName) == ".wmf") {
				if (!String.IsNullOrEmpty(this.style)) {
					InlinePictureCssParser cssParser = new InlinePictureCssParser(Importer.DocumentModel, new Size(100, 100));
					using (StringReader stringReader = new StringReader(this.style)) {
						cssParser.ParseAttribute(stringReader);
					}
					image = openXmlImporter.LookupMetafileByRelationId(id, openXmlImporter.DocumentRootFolder, cssParser.Size.Width, cssParser.Size.Height);
				}
			}
			else
				image = openXmlImporter.LookupImageByRelationId(id, openXmlImporter.DocumentRootFolder);
			if (image != null)
				floatingObjectImportInfo.Image = image;
		}
	}
	#endregion
	#region InlinePictureCssParser
	public class InlinePictureCssParser : CssParser {
		readonly CssKeywordTranslatorTable cssKeywordTable;
		readonly Size originalSize;
		int topDistance;
		int leftDistance;
		int rightDistance;
		int bottomDistance;
		FloatingObjectHorizontalPositionAlignment horizontalPositionAlignment;
		FloatingObjectVerticalPositionAlignment verticalPositionAlignment;
		VerticalAlignment textBoxVerticalAlignment;
		bool wrapText;
		bool useWrapText;
		string horizontalPositionType;
		string verticalPositionType;
		Point offset;
		Size size;
		int zOrder;
		int rotation;
		bool useRotation;
		int widthPercent;
		FloatingObjectRelativeFromHorizontal fromWidth;
		bool useRelativeWidth;
		int heightPercent;
		FloatingObjectRelativeFromVertical fromHeight;
		bool useRelativeHeight;
		public InlinePictureCssParser(DocumentModel documentModel, Size originalSize)
			: base(documentModel) {
			this.originalSize = originalSize;
			this.size = originalSize;
			this.cssKeywordTable = CreateCssKeywordTable();
			this.leftDistance = documentModel.UnitConverter.TwipsToModelUnits(180); 
			this.rightDistance = documentModel.UnitConverter.TwipsToModelUnits(180); 
			this.fromWidth = FloatingObjectRelativeFromHorizontal.Page;
			this.fromHeight = FloatingObjectRelativeFromVertical.Page;
		}
		public Size Size { get { return size; } }
		public int ZOrder { get { return zOrder; } }
		public int TopDistance { get { return topDistance; } }
		public int LeftDistance { get { return leftDistance; } }
		public int RightDistance { get { return rightDistance; } }
		public int BottomDistance { get { return bottomDistance; } }
		public Point Offset { get { return offset; } }
		public FloatingObjectHorizontalPositionAlignment HorizontalPositionAlignment { get { return horizontalPositionAlignment; } }
		public FloatingObjectVerticalPositionAlignment VerticalPositionAlignment { get { return verticalPositionAlignment; } }
		public VerticalAlignment TextBoxVerticalAlignment { get { return textBoxVerticalAlignment; } }
		public string HorizontalPositionType { get { return horizontalPositionType; } }
		public string VerticalPositionType { get { return verticalPositionType; } }
		public bool WrapText { get { return wrapText; } }
		public bool UseWrapText { get { return useWrapText; } }
		public int Rotation { get { return rotation; } }
		public bool UseRotation { get { return useRotation; } }
		public int WidthPercent { get { return widthPercent; } }
		public FloatingObjectRelativeFromHorizontal FromWidth { get { return fromWidth; } }
		public bool UseRelativeWidth { get { return useRelativeWidth; } }
		public int HeightPercent { get { return heightPercent; } }
		public FloatingObjectRelativeFromVertical FromHeight { get { return fromHeight; } }
		public bool UseRelativeHeight { get { return useRelativeHeight; } }
		protected internal override CssKeywordTranslatorTable CssKeywordTable { get { return cssKeywordTable; } }
		CssKeywordTranslatorTable CreateCssKeywordTable() {
			CssKeywordTranslatorTable cssKeywordTable = new CssKeywordTranslatorTable();
			cssKeywordTable.Add(ConvertKeyToUpper("width"), CssWidth);
			cssKeywordTable.Add(ConvertKeyToUpper("height"), CssHeight);
			cssKeywordTable.Add(ConvertKeyToUpper("z-index"), CssZOrder);
			cssKeywordTable.Add(ConvertKeyToUpper("mso-wrap-distance-top"), CssTopDistance);
			cssKeywordTable.Add(ConvertKeyToUpper("mso-wrap-distance-left"), CssLeftDistance);
			cssKeywordTable.Add(ConvertKeyToUpper("mso-wrap-distance-right"), CssRightDistance);
			cssKeywordTable.Add(ConvertKeyToUpper("mso-wrap-distance-bottom"), CssBottomDistance);
			cssKeywordTable.Add(ConvertKeyToUpper("margin-left"), CssOffsetX);
			cssKeywordTable.Add(ConvertKeyToUpper("margin-top"), CssOffsetY);
			cssKeywordTable.Add(ConvertKeyToUpper("mso-position-horizontal"), CssHorizontalPositionAlignment);
			cssKeywordTable.Add(ConvertKeyToUpper("mso-position-vertical"), CssVerticalPositionAlignment);
			cssKeywordTable.Add(ConvertKeyToUpper("mso-position-vertical-relative"), CssVerticalPositionType);
			cssKeywordTable.Add(ConvertKeyToUpper("mso-position-horizontal-relative"), CssHorizontalPositionType);
			cssKeywordTable.Add(ConvertKeyToUpper("v-text-anchor"), CssTextBoxVerticalAlignment);
			cssKeywordTable.Add(ConvertKeyToUpper("mso-wrap-style"), CssTextBoxWrapType);
			cssKeywordTable.Add(ConvertKeyToUpper("rotation"), CssRotation);
			cssKeywordTable.Add(ConvertKeyToUpper("mso-width-percent"), CssMsoWidthPercent);
			cssKeywordTable.Add(ConvertKeyToUpper("mso-height-percent"), CssMsoHeightPercent);
			cssKeywordTable.Add(ConvertKeyToUpper("mso-width-relative"), CssMsoWidthRelative);
			cssKeywordTable.Add(ConvertKeyToUpper("mso-height-relative"), CssMsoHeightRelative);
			return cssKeywordTable;
		}
		protected internal virtual void CssWidth(CssProperties cssProperties, List<string> propertiesValue) {
			DXUnit value = new DXUnit(propertiesValue[0]);
			UnitConversionParameters parameters = UnitConversionParameters.Empty;
			parameters.OriginalValue = originalSize.Width;
			UnitConverter converter = new UnitConverter(cssProperties.UnitConverter);
			size.Width = converter.ToModelUnits(value, parameters);
		}
		protected internal virtual void CssHeight(CssProperties cssProperties, List<string> propertiesValue) {
			DXUnit value = new DXUnit(propertiesValue[0]);
			UnitConversionParameters parameters = UnitConversionParameters.Empty;
			parameters.OriginalValue = originalSize.Height;
			UnitConverter converter = new UnitConverter(cssProperties.UnitConverter);
			size.Height = converter.ToModelUnits(value, parameters);
		}
		protected internal virtual void CssVerticalPositionType(CssProperties cssProperties, List<string> propertiesValue) {
			verticalPositionType = propertiesValue[0];
		}
		protected internal virtual void CssHorizontalPositionType(CssProperties cssProperties, List<string> propertiesValue) {
			horizontalPositionType = propertiesValue[0];
		}
		FloatingObjectHorizontalPositionAlignment GetFloatingObjectHorizontalPositionAlignment(string value) {
			Dictionary<FloatingObjectHorizontalPositionAlignment, WordProcessingMLValue> table = WordProcessingMLBaseExporter.floatingObjectHorizontalPositionAlignmentTable;
			foreach (FloatingObjectHorizontalPositionAlignment key in table.Keys) {
				if (value == table[key].WordMLValue || value == table[key].OpenXmlValue)
					return key;
			}
			return FloatingObjectHorizontalPositionAlignment.None;
		}
		FloatingObjectVerticalPositionAlignment GetFloatingObjectVerticalPositionAlignment(string value) {
			Dictionary<FloatingObjectVerticalPositionAlignment, WordProcessingMLValue> table = WordProcessingMLBaseExporter.floatingObjectVerticalPositionAlignmentTable;
			foreach (FloatingObjectVerticalPositionAlignment key in table.Keys) {
				if (value == table[key].WordMLValue || value == table[key].OpenXmlValue)
					return key;
			}
			return FloatingObjectVerticalPositionAlignment.None;
		}
		protected internal virtual void CssHorizontalPositionAlignment(CssProperties cssProperties, List<string> propertiesValue) {
			horizontalPositionAlignment = GetFloatingObjectHorizontalPositionAlignment(propertiesValue[0]);
		}
		protected internal virtual void CssVerticalPositionAlignment(CssProperties cssProperties, List<string> propertiesValue) {
			verticalPositionAlignment = GetFloatingObjectVerticalPositionAlignment(propertiesValue[0]);
		}
		protected internal virtual void CssTextBoxVerticalAlignment(CssProperties cssProperties, List<string> propertiesValue) {
			textBoxVerticalAlignment = GetFloatingObjectVerticalAlignment(propertiesValue[0]);
		}
		protected internal virtual void CssTextBoxWrapType(CssProperties cssProperties, List<string> propertiesValue) {
			wrapText = GetWrapText(propertiesValue[0]);
			useWrapText = true;
		}
		bool GetWrapText(string value) {
			return value == "square";
		}
		VerticalAlignment GetFloatingObjectVerticalAlignment(string value) {
			Dictionary<VerticalAlignment, WordProcessingMLValue> table = WordProcessingMLBaseExporter.textBoxVerticalAlignmentTable;
			foreach (VerticalAlignment key in table.Keys) {
				if (value == table[key].WordMLValue || value == table[key].OpenXmlValue)
					return key;
			}
			return VerticalAlignment.Top;
		}
		protected internal virtual void CssZOrder(CssProperties cssProperties, List<string> propertiesValue) {
			DXUnit value = new DXUnit(propertiesValue[0], int.MinValue, int.MaxValue);
			zOrder = (int)value.Value;
		}
		int GetModelUnitsFromUnitOrEmuValue(DocumentModelUnitConverter cssUnitConverter, string propertyValue) {
			int emuValue;
			UnitConverter converter = new UnitConverter(cssUnitConverter);
			if (!Int32.TryParse(propertyValue, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out emuValue)) {
				DXUnit value = new DXUnit(propertyValue);
				return converter.ToModelUnits(value);
			}
			else {
				return cssUnitConverter.EmuToModelUnits(emuValue);
			}
		}
		protected internal virtual void CssTopDistance(CssProperties cssProperties, List<string> propertiesValue) {
			topDistance = GetModelUnitsFromUnitOrEmuValue(cssProperties.UnitConverter, propertiesValue[0]);
		}
		protected internal virtual void CssLeftDistance(CssProperties cssProperties, List<string> propertiesValue) {
			leftDistance = GetModelUnitsFromUnitOrEmuValue(cssProperties.UnitConverter, propertiesValue[0]);
		}
		protected internal virtual void CssRightDistance(CssProperties cssProperties, List<string> propertiesValue) {
			rightDistance = GetModelUnitsFromUnitOrEmuValue(cssProperties.UnitConverter, propertiesValue[0]);
		}
		protected internal virtual void CssBottomDistance(CssProperties cssProperties, List<string> propertiesValue) {
			bottomDistance = GetModelUnitsFromUnitOrEmuValue(cssProperties.UnitConverter, propertiesValue[0]);
		}
		protected internal virtual void CssMsoWidthPercent(CssProperties cssProperties, List<string> propertiesValue) {
			widthPercent = GetPercentValue(propertiesValue[0]);
			useRelativeWidth = true;
		}
		protected internal virtual void CssMsoHeightPercent(CssProperties cssProperties, List<string> propertiesValue) {
			heightPercent = GetPercentValue(propertiesValue[0]);
			useRelativeHeight = true;
		}
		protected internal virtual void CssMsoWidthRelative(CssProperties cssProperties, List<string> propertiesValue) {
			fromWidth = GetFloatingObjectRelativeFromHorizontal(propertiesValue[0]);
			useRelativeWidth = true;
		}
		protected internal virtual void CssMsoHeightRelative(CssProperties cssProperties, List<string> propertiesValue) {
			fromHeight = GetFloatingObjectRelativeFromVertical(propertiesValue[0]);
			useRelativeHeight = true;
		}
		FloatingObjectRelativeFromHorizontal GetFloatingObjectRelativeFromHorizontal(string value) {
			Dictionary<FloatingObjectRelativeFromHorizontal, WordProcessingMLValue> table = WordProcessingMLBaseExporter.floatingObjectCssRelativeFromHorizontalTable;
			foreach (FloatingObjectRelativeFromHorizontal key in table.Keys) {
				if (value == table[key].WordMLValue || value == table[key].OpenXmlValue)
					return key;
			}
			return FloatingObjectRelativeFromHorizontal.Margin;
		}
		FloatingObjectRelativeFromVertical GetFloatingObjectRelativeFromVertical(string value) {
			Dictionary<FloatingObjectRelativeFromVertical, WordProcessingMLValue> table = WordProcessingMLBaseExporter.floatingObjectCssRelativeFromVerticalTable;
			foreach (FloatingObjectRelativeFromVertical key in table.Keys) {
				if (value == table[key].WordMLValue || value == table[key].OpenXmlValue)
					return key;
			}
			return FloatingObjectRelativeFromVertical.Margin;
		}
		int GetPercentValue(string value) {
			value = value.Trim();
			if (value.EndsWith("%")) {
				double result;
				if (Double.TryParse(value.Substring(0, value.Length - 1), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out result))
					return (int)(result* 1000);
			}
			else {
				int result;
				if (Int32.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out result))
					return (int)(result * 100);
			}
			return 100 * 1000;
		}
		protected internal virtual void CssOffsetX(CssProperties cssProperties, List<string> propertiesValue) {
			DXUnit value = new DXUnit(propertiesValue[0]);
			UnitConverter converter = new UnitConverter(cssProperties.UnitConverter);
			offset.X = converter.ToModelUnits(value);
		}
		protected internal virtual void CssOffsetY(CssProperties cssProperties, List<string> propertiesValue) {
			DXUnit value = new DXUnit(propertiesValue[0]);
			UnitConverter converter = new UnitConverter(cssProperties.UnitConverter);
			offset.Y = converter.ToModelUnits(value);
		}
		protected internal virtual void CssRotation(CssProperties cssProperties, List<string> propertiesValue) {
			useRotation = true;
			DXRotationUnit value = new DXRotationUnit(propertiesValue[0]);
			UnitConverter converter = new UnitConverter(cssProperties.UnitConverter);
			if(value.Type == DXUnitType.Deg)
				rotation = converter.DegreeToModelUnits(value);
			else
				rotation = converter.FDToModelUnits(value);
		}
	}
	#endregion
}
