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
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		const int defaultLineWidthInEMUs = 0x00002535;
		public bool ShouldExportPictures { get { return Workbook.DocumentCapabilities.PicturesAllowed; } }
		protected virtual string RootImagePath { get { return "xl"; } }
		OfficeImage currentImage;
		public void SetCurrentImage(OfficeImage image) { currentImage = image; }
		public void ExportPictureCore(Picture picture) {
			WriteStartElement("pic", SpreadsheetDrawingNamespace);
			try {
				if (!string.IsNullOrEmpty(picture.Macro))
					WriteStringValue("macro", picture.Macro);
				currentImage = picture.Image;
				ExportPictureNonVisualProperties(picture);
				ExportBlipFill(picture.PictureFill);
				ExportShapeProperties(picture.ShapeProperties);
			}
			finally {
				WriteEndElement();
			}
		}
		#region Export BlipFill
		protected internal virtual void ExportBlipFill(DrawingBlipFill blipFill) {
			WriteStartElement("blipFill", SpreadsheetDrawingNamespace);
			try {
				ExportBlipFillAttributes(blipFill);
				if (!blipFill.Blip.IsEmpty || (currentImage != null))
					GenerateDrawingBlipContentCore(blipFill.Blip, currentImage);
				if (!blipFill.SourceRectangle.Equals(RectangleOffset.Empty))
					ExportSourceRectangle(blipFill.SourceRectangle);
				if (blipFill.Stretch)
					ExportStretch(blipFill.FillRectangle);
			}
			finally {
				WriteEndElement();
			}
		}
		void ExportStretch(RectangleOffset rectangleOffset) {
			WriteStartElement("stretch", DrawingMLNamespace);
			try {
				WriteStartElement("fillRect", DrawingMLNamespace);
				try {
					if (rectangleOffset.BottomOffset != 0 && rectangleOffset.LeftOffset != 0 && rectangleOffset.RightOffset != 0 && rectangleOffset.TopOffset != 0)
						WriteRectangleOffsetAttributes(rectangleOffset);
				}
				finally {
					WriteEndElement();
				}
			}
			finally {
				WriteEndElement();
			}
		}
		void ExportSourceRectangle(RectangleOffset rectangleOffset) {
			WriteStartElement("srcRect", DrawingMLNamespace);
			try {
				if (rectangleOffset.BottomOffset != 0 && rectangleOffset.LeftOffset != 0 && rectangleOffset.RightOffset != 0 && rectangleOffset.TopOffset != 0)
					WriteRectangleOffsetAttributes(rectangleOffset);
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteRectangleOffsetAttributes(RectangleOffset rectangleOffset) {
			WriteIntValue("b", rectangleOffset.BottomOffset);
			WriteIntValue("l", rectangleOffset.LeftOffset);
			WriteIntValue("r", rectangleOffset.RightOffset);
			WriteIntValue("t", rectangleOffset.TopOffset);
		}
		void ExportBlipFillAttributes(DrawingBlipFill blipFill) {
			if (blipFill.Dpi != 0)
				WriteIntValue("dpi", blipFill.Dpi);
			if (blipFill.RotateWithShape)
				WriteBoolValue("rotWithShape", blipFill.RotateWithShape);
		}
		protected internal virtual string GenerateImageId() {
			ImageCounter++;
			return "image" + ImageCounter;
		}
		protected internal virtual string GenerateDrawingRelationId() {
			return GenerateIdByCollection(currentRelations);
		}
		protected internal virtual string ExportImageData(IDocumentModel documentModel, OfficeImage image) {
			string imageRelationId;
			OfficeImage rootImage = image.RootImage;
			if (ExportedImageTable.TryGetValue(rootImage, out imageRelationId))
				return imageRelationId;
			OpenXmlRelation relation = new OpenXmlRelation();
			string imageId = GenerateImageId();
			string imageFileName = ExportImageData(rootImage, imageId);
			imageRelationId = GenerateDrawingRelationId();
			ExportedImageTable.Add(rootImage, imageRelationId);
			relation.Id = imageRelationId;
			relation.Target = "../media/" + imageFileName;
			relation.Type = RelsImagesNamespace;
			currentRelations.Add(relation);
			return imageRelationId;
		}
		protected internal virtual string ExportExternalImageData(string imageLink, OfficeImage image) {
			string imageRelationId;
			if (ExportedExternalImageTable.TryGetValue(imageLink, out imageRelationId))
				return imageRelationId;
			imageRelationId = GenerateDrawingRelationId();
			ExportedExternalImageTable.Add(imageLink, imageRelationId);
			OpenXmlRelation relation = new OpenXmlRelation();
			relation.Id = imageRelationId;
			relation.Type = RelsImagesNamespace;
			relation.Target = imageLink;
			relation.TargetMode = "External";
			currentRelations.Add(relation);
			return imageRelationId;
		}
		protected internal virtual string GetImageExtension(OfficeImage image) {
			string result = OfficeImage.GetExtension(image.RawFormat);
			if (String.IsNullOrEmpty(result))
				result = "png";
			return result;
		}
		protected internal virtual string ExportImageData(OfficeImage image, string imageId) {
			string extenstion = GetImageExtension(image);
			string contentType;
			if (!Builder.UsedContentTypes.TryGetValue(extenstion, out contentType)) {
				contentType = OfficeImage.GetContentType(image.RawFormat);
				if (String.IsNullOrEmpty(contentType)) {
					contentType = OfficeImage.GetContentType(OfficeImageFormat.Png);
				}
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
		#endregion
		#region Export PictureNonVisualProperties
		string GetHyperlinkRelationId(IDrawingObjectNonVisualProperties properties) {
			string hyperlinkUrl = properties.HyperlinkClickUrl;
			string hyperlinkRelationId;
			if (ExportedPictureHyperlinkTable.TryGetValue(hyperlinkUrl, out hyperlinkRelationId))
				return hyperlinkRelationId;
			hyperlinkRelationId = GenerateDrawingRelationId();
			ExportedPictureHyperlinkTable.Add(hyperlinkUrl, hyperlinkRelationId);
			OpenXmlRelation relation = new OpenXmlRelation();
			relation.Id = hyperlinkRelationId;
			relation.Target = hyperlinkUrl;
			relation.Type = OfficeHyperlinkType;
			if (properties.HyperlinkClickIsExternal)
				relation.TargetMode = "External";
			currentRelations.Add(relation);
			return hyperlinkRelationId;
		}
		protected internal virtual void ExportPictureNonVisualProperties(Picture picture) {
			WriteStartElement("nvPicPr", SpreadsheetDrawingNamespace);
			try {
				ExportNonVisualDrawingProperties(picture.DrawingObject.Properties);
				ExportNonVisualPictureDrawingProperties(picture);
			}
			finally {
				WriteEndElement();
			}
		}
		void ExportNonVisualDrawingProperties(IDrawingObjectNonVisualProperties properties) {
			WriteStartElement("cNvPr", SpreadsheetDrawingNamespace);
			try {
				WriteIntValue("id", properties.Id);
				WriteStringValue("name", properties.Name);
				if (!String.IsNullOrEmpty(properties.Description))
					WriteStringValue("descr", EncodeXmlChars(properties.Description));
				if (properties.Hidden != false)
					WriteBoolValue("hidden", properties.Hidden);
				if (!String.IsNullOrEmpty(properties.HyperlinkClickUrl))
					ExportHyperlinkClick(properties);
			}
			finally {
				WriteEndElement();
			}
		}
		void ExportHyperlinkClick(IDrawingObjectNonVisualProperties properties) {
			string hyperlinkRelationId = GetHyperlinkRelationId(properties);
			WriteStartElement("hlinkClick", DrawingMLNamespace);
			try {
				WriteStringAttr("xmlns", "r", null, RelsNamespace);
				WriteStringAttr("r", "id", RelsNamespace, hyperlinkRelationId);
				if (!String.IsNullOrEmpty(properties.HyperlinkClickTooltip))
					WriteStringValue("tooltip", EncodeXmlChars(properties.HyperlinkClickTooltip));
				if (!String.IsNullOrEmpty(properties.HyperlinkClickTargetFrame))
					WriteStringValue("tgtFrame", properties.HyperlinkClickTargetFrame);
			}
			finally {
				WriteEndElement();
			}
		}
		void ExportNonVisualPictureDrawingProperties(Picture picture) {
			WriteStartElement("cNvPicPr", SpreadsheetDrawingNamespace);
			try {
				if (picture.Properties.PreferRelativeResize != true)
					WriteBoolValue("preferRelativeResize", picture.Properties.PreferRelativeResize);
				if (!picture.Locks.IsEmpty)
					ExportPictureLocks(picture.Locks);
			}
			finally {
				WriteEndElement();
			}
		}
		void ExportCommonDrawingLocks(ICommonDrawingLocks commonDrawingLocks) {
			WriteBoolValue("noGrp", commonDrawingLocks.NoGroup, false);
			WriteBoolValue("noSelect", commonDrawingLocks.NoSelect, false);
			WriteBoolValue("noChangeAspect", commonDrawingLocks.NoChangeAspect, false);
			WriteBoolValue("noMove", commonDrawingLocks.NoMove, false);		  
		}
		void ExportDrawingLocks(IDrawingLocks drawingLocks) {
			ExportCommonDrawingLocks(drawingLocks);
			WriteBoolValue("noRot", drawingLocks.NoRotate, false);
			WriteBoolValue("noResize", drawingLocks.NoResize, false);
			WriteBoolValue("noEditPoints", drawingLocks.NoEditPoints, false);
			WriteBoolValue("noAdjustHandles", drawingLocks.NoAdjustHandles, false);
			WriteBoolValue("noChangeShapeType", drawingLocks.NoChangeShapeType, false);
			WriteBoolValue("noChangeArrowheads", drawingLocks.NoChangeArrowheads, false);
		}
		void ExportPictureLocks(IPictureLocks pictureLocks) {
			WriteStartElement("picLocks", DrawingMLNamespace);
			try {
				ExportDrawingLocks(pictureLocks);
				WriteBoolValue("noCrop", pictureLocks.NoCrop, false);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
	}
}
