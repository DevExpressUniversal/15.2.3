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
using System.Drawing;
using System.Xml;
using DevExpress.XtraRichEdit.Export.OpenDocument;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Office.Utils.Internal;
using DevExpress.Compatibility.System.Drawing;
#if !SL
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.OpenDocument {
	#region DestinationFrame
	public class FrameDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("image", OnImageDestination);
			result.Add("p", OnParagraph);
			result.Add("text-box", OnTextBox);
			return result;
		}
		readonly FloatingObjectImportInfo floatingObjectImportInfo;
		public FrameDestination(OpenDocumentTextImporter importer)
			: base(importer) {
			this.floatingObjectImportInfo = new FloatingObjectImportInfo(importer.PieceTable);
			this.floatingObjectImportInfo.TextBoxContent = new TextBoxContentType(DocumentModel);
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public FloatingObjectImportInfo FloatingObjectImportInfo { get { return floatingObjectImportInfo; } }
		public OfficeImage Image { get { return FloatingObjectImportInfo.Image; } set { FloatingObjectImportInfo.Image = value; } }
		public FloatingObjectProperties FloatingObject { get { return FloatingObjectImportInfo.FloatingObjectProperties; } }
		public Shape Shape { get { return FloatingObjectImportInfo.Shape; } }
		static FrameDestination GetThis(OpenDocumentTextImporter importer) {
			return (FrameDestination)importer.PeekDestination();
		}
		static Destination OnImageDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new ImageDestination(importer, GetThis(importer));
		}
		static Destination OnParagraph(OpenDocumentTextImporter importer, XmlReader reader) {
			GetThis(importer).FloatingObjectImportInfo.IsTextBox = true;
			return new ParagraphDestination(importer);
		}
		static Destination OnTextBox(OpenDocumentTextImporter importer, XmlReader reader) {
			GetThis(importer).FloatingObjectImportInfo.IsTextBox = true;
			return new TextBoxDestination(importer, GetThis(importer).FloatingObjectImportInfo.TextBoxProperties);
		}
		void ReadAnchorType(XmlReader reader) {
			FloatingObjectVerticalPositionType vPositionType = FloatingObjectVerticalPositionType.Page;
			FloatingObjectHorizontalPositionType hPositionType = FloatingObjectHorizontalPositionType.Page;
			switch (ImportHelper.GetTextStringAttribute(reader, "anchor-type")) {
				case "page":
					vPositionType = FloatingObjectVerticalPositionType.Page;
					hPositionType = FloatingObjectHorizontalPositionType.Page;
					FloatingObjectImportInfo.IsFloatingObject = true;
					break;
				case "paragraph":
					vPositionType = FloatingObjectVerticalPositionType.Paragraph;
					hPositionType = FloatingObjectHorizontalPositionType.Column;
					FloatingObjectImportInfo.IsFloatingObject = true;
					break;
				case "frame":
					vPositionType = FloatingObjectVerticalPositionType.Paragraph;
					hPositionType = FloatingObjectHorizontalPositionType.Column;
					FloatingObjectImportInfo.IsFloatingObject = true;
					break;
				case "char":
					vPositionType = FloatingObjectVerticalPositionType.Paragraph;
					hPositionType = FloatingObjectHorizontalPositionType.Character;
					FloatingObjectImportInfo.IsFloatingObject = true;
					break;
				case "as-char":
					FloatingObjectImportInfo.IsFloatingObject = false;
					break;
			}
			if (!FloatingObject.UseHorizontalPositionType)
				FloatingObject.HorizontalPositionType = hPositionType;
			if (!FloatingObject.UseVerticalPositionType)
				FloatingObject.VerticalPositionType = vPositionType;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			FloatingObject.BeginUpdate();
			try {
				ApplyFloatingObjectStyle(reader);
				ValueInfo width = ImportHelper.GetSvgAttributeInfo(reader, "width");
				ValueInfo height = ImportHelper.GetSvgAttributeInfo(reader, "height");
				if (!width.IsEmpty)
					FloatingObject.ActualWidth = (int)Math.Round(UnitsConverter.ValueUnitToModelUnitsF(width));
				if (!height.IsEmpty)
					FloatingObject.ActualHeight = (int)Math.Round(UnitsConverter.ValueUnitToModelUnitsF(height));
				ReadOffset(reader);
				ReadZOrder(reader);
				ReadAnchorType(reader);
				string name = reader.GetAttribute("name", OpenDocumentHelper.DrawNamespace);
				if (!String.IsNullOrEmpty(name))
					FloatingObjectImportInfo.Name = name;
			}
			finally {
				FloatingObject.EndUpdate();
			}
			Importer.PushCurrentPieceTable(FloatingObjectImportInfo.TextBoxContent.PieceTable);
		}
		void ReadZOrder(XmlReader reader) {
			int zOrder = ImportHelper.GetDrawIntegerAttribute(reader, "z-index", Int32.MinValue);
			if (zOrder != Int32.MinValue)
				FloatingObject.ZOrder = zOrder;
		}
		void ReadOffset(XmlReader reader) {
			ValueInfo xCoordinate = ImportHelper.GetSvgAttributeInfo(reader, "x");
			ValueInfo yCoordinate = ImportHelper.GetSvgAttributeInfo(reader, "y");
			if (!xCoordinate.IsEmpty) {
				int x = (int)Math.Round(UnitsConverter.ValueUnitToModelUnitsF(xCoordinate));
				FloatingObject.Offset = new Point(x, 0);
				FloatingObject.HorizontalPositionAlignment = FloatingObjectHorizontalPositionAlignment.None;
				FloatingObject.Info.Options.UseHorizontalPositionAlignment = false;
			}
			if (!yCoordinate.IsEmpty) {
				int y = (int)Math.Round(UnitsConverter.ValueUnitToModelUnitsF(yCoordinate));
				FloatingObject.Offset = (xCoordinate.IsEmpty ? new Point(0, y) : new Point((int)Math.Round(UnitsConverter.ValueUnitToModelUnitsF(xCoordinate)), y));
				FloatingObject.VerticalPositionAlignment = FloatingObjectVerticalPositionAlignment.None;
				FloatingObject.Info.Options.UseVerticalPositionAlignment = false;
			}
		}
		void ApplyFloatingObjectStyle(XmlReader reader) {
			string styleName = ImportHelper.GetDrawStringAttribute(reader, "style-name");
			if (!Importer.FrameAutoStyles.ContainsKey(styleName))
				return;
			FloatingObjectImportInfo styleFloatingObjectImportInfo = Importer.FrameAutoStyles[styleName];
			FloatingObjectImportInfo.FloatingObjectProperties.CopyFrom(styleFloatingObjectImportInfo.FloatingObjectProperties.Info);
			FloatingObjectImportInfo.Shape.CopyFrom(styleFloatingObjectImportInfo.Shape.Info);
			FloatingObjectImportInfo.TextBoxProperties.CopyFrom(styleFloatingObjectImportInfo.TextBoxProperties.Info);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (FloatingObjectImportInfo.IsTextBox) {
				Importer.PieceTable.FixLastParagraph();
				Importer.PieceTable.CheckIntegrity();
			}
			Importer.PopCurrentPieceTable();
			if (!FloatingObjectImportInfo.IsFloatingObject) {
				if (Image == null)
					return;
				if (!DocumentFormatsHelper.ShouldInsertPicture(Importer.DocumentModel)) {
					Importer.PieceTable.InsertTextCore(Importer.InputPosition, " ");
					return;
				}
				Size originalSize = InternalOfficeImageHelper.CalculateImageSizeInModelUnits(Image, Importer.DocumentModel.UnitConverter);
				Size desizedSize = new Size(FloatingObject.ActualWidth, FloatingObject.ActualHeight);
				SizeF scale = ImageScaleCalculator.GetScale(desizedSize, originalSize, 100f);
				Importer.PieceTable.AppendImage(Importer.InputPosition, Image, scale.Width, scale.Height);
			}
			else {
				if (Importer.PieceTable.IsTextBox) 
					return;
				OpenDocumentImportBoundsAdjuster adjuster = new OpenDocumentImportBoundsAdjuster(FloatingObject, Shape, FloatingObjectImportInfo.IsTextBox);
				Point adjustedOffset = adjuster.AdjustOffset();
				if (adjustedOffset != FloatingObject.Offset)
					FloatingObject.Offset = adjustedOffset;
				Size adjustedActualSize = adjuster.AdjustActualSize();
				if (adjustedActualSize != FloatingObject.ActualSize)
					FloatingObject.ActualSize = adjustedActualSize;
				FloatingObjectImportInfo.InsertFloatingObject(Importer.InputPosition);
			}
		}
	}
	#endregion
	#region TextBoxDestination
	public class TextBoxDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		readonly TextBoxProperties textBoxProperties;
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("p", OnParagraph);
			return result;
		}
		public TextBoxDestination(OpenDocumentTextImporter importer, TextBoxProperties textBoxProperties)
			: base(importer) {
				this.textBoxProperties = textBoxProperties;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public TextBoxProperties TextBoxProperties { get { return textBoxProperties; } }
		static Destination OnParagraph(OpenDocumentTextImporter importer, XmlReader reader) {
			return new ParagraphDestination(importer);
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ValueInfo height = ImportHelper.GetFoAttributeInfo(reader, "min-height");
			if (!height.IsEmpty)
				TextBoxProperties.ResizeShapeToFitText = true;
		}
	}
	#endregion
	#region OpenDocumentBoundsAdjuster (abstract class)
	public abstract class OpenDocumentBoundsAdjuster {
		#region Fields
		readonly FloatingObjectProperties floatingObjectProperties;
		readonly Shape shape;
		readonly bool isTextBox;
		#endregion
		protected OpenDocumentBoundsAdjuster(FloatingObjectProperties floatingObjectProperties, Shape shape, bool isTextBox) {
			Guard.ArgumentNotNull(floatingObjectProperties, "floatingObject");
			Guard.ArgumentNotNull(shape, "shape");
			this.floatingObjectProperties = floatingObjectProperties;
			this.shape = shape;
			this.isTextBox = isTextBox;
		}
		#region Properties
		public FloatingObjectProperties FloatingObjectProperties { get { return floatingObjectProperties; } }
		public Shape Shape { get { return shape; } }
		public bool IsTextBox { get { return isTextBox; } }
		protected internal abstract int Multiplier { get; }
		#endregion
		public Size AdjustActualSize() {
			if (IsTextBox) {
				if (!DXColor.IsTransparentOrEmpty(Shape.OutlineColor))
					return new Size(FloatingObjectProperties.ActualWidth + Multiplier * Shape.OutlineWidth, FloatingObjectProperties.ActualHeight);
			}
			else {
				if (!DXColor.IsTransparentOrEmpty(Shape.OutlineColor))
					return new Size(FloatingObjectProperties.ActualWidth - 2 * Multiplier * Shape.OutlineWidth, FloatingObjectProperties.ActualHeight - 2 * Multiplier * Shape.OutlineWidth);
			}
			return FloatingObjectProperties.ActualSize;
		}
		public Point AdjustOffset() {
			if (IsTextBox) {
				if (!DXColor.IsTransparentOrEmpty(Shape.OutlineColor))
					return new Point(FloatingObjectProperties.Offset.X, FloatingObjectProperties.Offset.Y - Multiplier * Shape.OutlineWidth / 2);
			}
			else {
				if (!DXColor.IsTransparentOrEmpty(Shape.OutlineColor))
					return new Point(FloatingObjectProperties.Offset.X + Multiplier * Shape.OutlineWidth, FloatingObjectProperties.Offset.Y + Multiplier * Shape.OutlineWidth);
			}
			return FloatingObjectProperties.Offset;
		}
	}
	#endregion
	#region OpenDocumentImportBoundsAdjuster
	public class OpenDocumentImportBoundsAdjuster : OpenDocumentBoundsAdjuster {
		public OpenDocumentImportBoundsAdjuster(FloatingObjectProperties floatingObjectProperties, Shape shape, bool isTextBox)
			: base(floatingObjectProperties, shape, isTextBox) {
		}
		protected internal override int Multiplier { get { return 1; } }
	}
	#endregion
	public class OpenDocumentExportBoundsAdjuster : OpenDocumentBoundsAdjuster {
		public OpenDocumentExportBoundsAdjuster(FloatingObjectProperties floatingObjectProperties, Shape shape, bool isTextBox)
			: base(floatingObjectProperties, shape, isTextBox) {
		}
		protected internal override int Multiplier { get { return -1; } }
	}
	#region ImageDestination
	public class ImageDestination : LeafElementDestination {
		readonly FrameDestination frameDestination;
		public ImageDestination(OpenDocumentTextImporter importer, FrameDestination frameDestination)
			: base(importer) {
			Guard.ArgumentNotNull(frameDestination, "DestinationFrame");
			this.frameDestination = frameDestination;
		}
		public OfficeImage FrameImage { get { return frameDestination.Image; } set { frameDestination.Image = value; } }
		public FloatingObjectProperties FloatingObject { get { return frameDestination.FloatingObject; } }
		public override void ProcessElementOpen(XmlReader reader) {
			string filePath = ImportHelper.GetXlinkStringAttribute(reader, "href");
			if (String.IsNullOrEmpty(filePath))
				return;
			if (!DocumentFormatsHelper.ShouldInsertPicture(DocumentModel))
				return;
			OfficeImage image = Importer.LookUpImageByFileName(filePath);
			if (image != null)
				FrameImage = image;
			if (Importer.IsInTable())
				FloatingObject.LayoutInTableCell = true;
		}
	}
	#endregion
}
