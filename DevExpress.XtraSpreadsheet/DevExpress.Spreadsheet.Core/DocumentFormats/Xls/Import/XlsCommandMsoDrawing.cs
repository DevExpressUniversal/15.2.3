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
using System.Drawing;
using System.IO;
using DevExpress.Office.Services;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraExport.Xls;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Export.Xl;
using DevExpress.Office;
using DevExpress.Office.DrawingML;
using DevExpress.Utils;
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandMsoDrawingGroup
	public class XlsCommandMsoDrawingGroup : XlsCommandRecordBase {
		static short[] typeCodes = new short[] {
			0x00eb, 
			0x003c  
		};
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			if(contentBuilder.ContentType == XlsContentType.WorkbookGlobals) {
				using(XlsCommandStream artStream = new XlsCommandStream(reader, typeCodes, Size)) {
					using(BinaryReader artReader = new BinaryReader(artStream)) {
						contentBuilder.DrawingGroup = OfficeArtDrawingGroupContainer.FromStream(artReader);
					}
				}
			}
			else {
				base.ReadCore(reader, contentBuilder);
			}
		}
		protected override void CheckPosition(XlsReader reader, XlsContentBuilder contentBuilder, long initialPosition, long expectedPosition) {
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandMsoDrawing
	public class XlsCommandMsoDrawing : XlsCommandRecordBase {
		static short[] typeCodes = new short[] {
			0x00ec, 
			0x003c, 
			0x005d, 
			0x007f, 
			0x01b6  
		};
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			if(contentBuilder.ContentType == XlsContentType.Sheet) {
				using(XlsCommandStream artStream = new XlsCommandStream(contentBuilder, reader, typeCodes, Size)) {
					using(BinaryReader artReader = new BinaryReader(artStream)) {
						contentBuilder.DrawingObjects = OfficeArtDrawingObjectsContainer.FromStream(artReader, new OfficeArtRecordFactorySheet(), contentBuilder);
					}
				}
			}
			else {
				base.ReadCore(reader, contentBuilder);
			}
		}
		protected override void CheckPosition(XlsReader reader, XlsContentBuilder contentBuilder, long initialPosition, long expectedPosition) {
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsDrawingObjectsHelper
	public static class XlsMsoDrawingHelper {
		static bool shouldImportCharts = true;
		public static void HandleShapeGroup(XlsContentBuilder contentBuilder, OfficeDrawingPartBase part) {
			OfficeArtShapeGroupContainer group = part as OfficeArtShapeGroupContainer;
			if(group == null) return;
			for(int i = 0; i < group.Items.Count; i++) {
				OfficeDrawingPartBase item = group.Items[i];
				if(i == 0) {
					if(item.HeaderTypeCode != OfficeArtTypeCodes.ShapeContainer)
						contentBuilder.ThrowInvalidFile("First container of OfficeArt shape group is not a shape!");
					if(!HandleTopmostShape(contentBuilder, item))
						break;
				}
				else {
					if(item.HeaderTypeCode == OfficeArtTypeCodes.ShapeContainer)
						HandleShape(contentBuilder, item);
					else if(item.HeaderTypeCode == OfficeArtTypeCodes.ShapeGroupContainer)
						HandleShapeGroup(contentBuilder, item);
				}
			}
			if(contentBuilder.CoordinateSystems.Count > 0)
				contentBuilder.CoordinateSystems.Pop();
		}
		static bool HandleTopmostShape(XlsContentBuilder contentBuilder, OfficeDrawingPartBase part) {
			OfficeArtShapeContainer shape = part as OfficeArtShapeContainer;
			if(shape == null) return false;
			OfficeArtShapeGroupCoordinateSystem coordinateSystem = shape.CoordinateSystem;
			if(coordinateSystem != null)
				contentBuilder.CoordinateSystems.Push(coordinateSystem);
			else
				contentBuilder.ThrowInvalidFile("First container of group is not a topmost shape (has not coordinate system)!");
			return !shape.IsDeleted;
		}
		static void HandleShape(XlsContentBuilder contentBuilder, OfficeDrawingPartBase part) {
			OfficeArtShapeContainer shape = part as OfficeArtShapeContainer;
			if(shape == null) return;
			if(shape.IsDeleted) return;
			OfficeArtClientData clientData = shape.ClientData;
			if(clientData != null) {
				if(clientData.Data.ObjType == XlsObjType.Picture)
					HandlePicture(contentBuilder, shape);
				else if(clientData.Data.ObjType == XlsObjType.Note)
					HandleNote(contentBuilder, shape);
				else if(clientData.Data.ObjType == XlsObjType.Chart)
					HandleChart(contentBuilder, shape);
				else if(IsModelShape(shape)) {
					HandleModelShape(contentBuilder, shape);
				}
				else {}
			}
			else {
				OfficeArtClientTextbox clientTextbox = shape.ClientTextbox;
				if(clientTextbox != null) {
				}
			}
		}
		static bool IsModelShape(OfficeArtShapeContainer shape) {
			OfficeArtShapeRecord shapeRecord = shape.ShapeRecord;
			if(shapeRecord == null)
				return false;
			return shapeRecord.HeaderInstanceInfo != ShapeTypeCode.PictureFrame &&
				   shapeRecord.HeaderInstanceInfo != ShapeTypeCode.HostControl;
		}
		static void HandlePicture(XlsContentBuilder contentBuilder, OfficeArtShapeContainer shape) {
			MergeDrawingGroupProperties(contentBuilder, shape);
			OfficeArtClientAnchorSheet clientAnchor = shape.ClientAnchor as OfficeArtClientAnchorSheet;
			if(clientAnchor != null) {
				if(!clientAnchor.IsValid())
					return;
				if (!contentBuilder.DocumentModel.DocumentCapabilities.PicturesAllowed)
					return;
				Worksheet sheet = contentBuilder.CurrentSheet;
				DrawingObject drawingObject = CreateDrawingObject(sheet, clientAnchor);
				OfficeImage image = GetImage(contentBuilder, shape);
				bool placeHolder = image == null;
				if(placeHolder)
					image = UriBasedOfficeImageBase.CreatePlaceHolder(contentBuilder.DocumentModel, 28, 28);
				else {
					image = sheet.Workbook.ImageCache.AddImage(image.EncapsulatedOfficeNativeImage);
				}
				Picture picture = new Picture(drawingObject);
				picture.Image = image;
				SetupPictureProperties(picture, contentBuilder, shape, placeHolder);
				picture.CheckRotationAndSwapBox();
				sheet.DrawingObjects.Add(picture);
			}
			else {
				OfficeArtChildAnchor childAnchor = shape.ChildAnchor;
				if(childAnchor != null) {
				}
			}
		}
		static void HandleNote(XlsContentBuilder contentBuilder, OfficeArtShapeContainer shape) {
			MergeDrawingGroupProperties(contentBuilder, shape);
			OfficeArtClientData clientData = shape.ClientData;
			if(clientData != null) {
				int objId = clientData.Data.ObjId;
				if(!contentBuilder.CommentObjects.ContainsKey(objId))
					contentBuilder.CommentObjects.Add(objId, shape);
			}
		}
		static void HandleChart(XlsContentBuilder contentBuilder, OfficeArtShapeContainer shape) {
			MergeDrawingGroupProperties(contentBuilder, shape);
			OfficeArtClientAnchorSheet clientAnchor = shape.ClientAnchor as OfficeArtClientAnchorSheet;
			if (clientAnchor != null) {
				if (!clientAnchor.IsValid())
					return;
				if (!contentBuilder.DocumentModel.DocumentCapabilities.ChartsAllowed || !shouldImportCharts)
					return;
				Worksheet sheet = contentBuilder.CurrentSheet;
				DrawingObject drawingObject = CreateDrawingObject(sheet, clientAnchor);
				OfficeArtClientData clientData = shape.ClientData;
				if (clientData != null) {
					Chart chart = clientData.Data.EmbeddedChart;
					if (chart != null) {
						chart.DrawingObject.CopyFrom(drawingObject);
						SetupChartProperties(chart, contentBuilder, shape);
						sheet.DrawingObjects.Add(chart);
					}
				}
			}
		}
		static void SetupChartProperties(Chart chart, XlsContentBuilder contentBuilder, OfficeArtShapeContainer shape) {
			chart.DrawingObject.Properties.Id = shape.ShapeId;
			chart.DrawingObject.Properties.Name = string.IsNullOrEmpty(shape.ShapeName) ? string.Format("Chart {0}", shape.ShapeId & 0x0fff) : shape.ShapeName;
			OfficeArtClientData clientData = shape.ClientData;
			if (clientData != null) {
				XlsObjCommon commonProperties = clientData.Data.CommonProperties;
				if (commonProperties != null) {
					chart.LocksWithSheet = commonProperties.Locked;
					chart.PrintsWithSheet = commonProperties.Print;
				}
			}
			OfficeArtShapeRecord shapeRecord = shape.ShapeRecord;
			if (shapeRecord != null) {
				chart.ShapeProperties.Transform2D.FlipH = shapeRecord.FlipH;
				chart.ShapeProperties.Transform2D.FlipV = shapeRecord.FlipV;
				chart.ShapeProperties.ShapeType = GetShapeType(shapeRecord.HeaderInstanceInfo, ShapeType.Rect);
			}
			OfficeArtProperties artProperties = shape.ArtProperties;
			if (artProperties != null) {
				chart.DrawingObject.Properties.Description = artProperties.ShapeDescription;
				DrawingGroupShapeBooleanProperties bpGroupShape = artProperties.GetGroupShapeBooleanProperties();
				if (bpGroupShape != null) {
					if (bpGroupShape.UseHidden)
						chart.DrawingObject.Properties.Hidden = bpGroupShape.Hidden;
				}
			}
		}
		static void HandleModelShape(XlsContentBuilder contentBuilder, OfficeArtShapeContainer shape) {
			MergeDrawingGroupProperties(contentBuilder, shape);
			OfficeArtClientAnchorSheet clientAnchor = shape.ClientAnchor as OfficeArtClientAnchorSheet;
			if(clientAnchor == null)
				return;
			if(!clientAnchor.IsValid())
				return;
			if(!contentBuilder.DocumentModel.DocumentCapabilities.ShapesAllowed)
				return;
			Worksheet sheet = contentBuilder.CurrentSheet;
			DrawingObject drawingObject = CreateDrawingObject(sheet, clientAnchor);
			OfficeArtClientData clientData = shape.ClientData;
			if(clientData == null)
				return;
			ModelShape modelShape = new ModelShape(drawingObject);
			SetupModelShapeProperties(modelShape, contentBuilder, shape);
			sheet.DrawingObjects.Add(modelShape);
		}
		static void SetupModelShapeProperties(ModelShape modelShape, XlsContentBuilder contentBuilder, OfficeArtShapeContainer shape) {
			modelShape.DrawingObject.Properties.Id = shape.ShapeId;
			modelShape.DrawingObject.Properties.Name = shape.ShapeName;
			OfficeArtClientData clientData = shape.ClientData;
			if(clientData != null) {
				XlsObjCommon commonProperties = clientData.Data.CommonProperties;
				if(commonProperties != null) {
					modelShape.LocksWithSheet = commonProperties.Locked;
					modelShape.PrintsWithSheet = commonProperties.Print;
				}
			}
			OfficeArtShapeRecord shapeRecord = shape.ShapeRecord;
			if(shapeRecord != null) {
				modelShape.ShapeProperties.Transform2D.FlipH = shapeRecord.FlipH;
				modelShape.ShapeProperties.Transform2D.FlipV = shapeRecord.FlipV;
				modelShape.ShapeProperties.ShapeType = GetShapeType(shapeRecord.HeaderInstanceInfo, ShapeType.None);
			}
			OfficeArtProperties artProperties = shape.ArtProperties;
			if(artProperties != null) {
				modelShape.DrawingObject.Properties.Description = artProperties.ShapeDescription;
				modelShape.ShapeProperties.BlackAndWhiteMode = ConvertBlackWhiteMode(artProperties.GetBlackAndWhiteMode());
				DrawingGroupShapeBooleanProperties bpGroupShape = artProperties.GetGroupShapeBooleanProperties();
				if(bpGroupShape != null) {
					if(bpGroupShape.UseHidden)
						modelShape.DrawingObject.Properties.Hidden = bpGroupShape.Hidden;
				}
				SetupOutlineProperties(modelShape.ShapeProperties, contentBuilder, artProperties);
				SetupModelShapeProtectionProperties(modelShape.Locks, artProperties);
				SetupShapeTransform(modelShape, contentBuilder, artProperties);
				SetupHyperlink(modelShape.DrawingObject, contentBuilder, artProperties);
			}
			SetupModelShapeFillProperties(modelShape.ShapeProperties, contentBuilder, shape);
			SetupHyperlinkTooltip(modelShape.DrawingObject, shape.TertiaryArtProperties);
			ModelShapePathImportHelper.SetupShapeGeometry(modelShape, shape);
			SetupShapeText(contentBuilder, modelShape, shape);
			modelShape.CheckRotationAndSwapBox();
		}
		static void SetupShapeText(XlsContentBuilder contentBuilder, ModelShape modelShape, OfficeArtShapeContainer shape) {
			OfficeArtProperties artProperties = shape.ArtProperties;
			if(artProperties != null) {
				DrawingTextBooleanProperties textProperties = artProperties.GetTextBooleanProperties();
				if(textProperties != null) {
					if(textProperties.UseFitShapeToText && textProperties.FitShapeToText)
						modelShape.TextProperties.BodyProperties.AutoFit = DrawingTextAutoFit.Shape;
				}
			}
			ModelShapeTextHelper.SetupModelShapeText(contentBuilder, shape, modelShape);
		}
		static void MergeDrawingGroupProperties(XlsContentBuilder contentBuilder, OfficeArtShapeContainer shape) {
			OfficeArtProperties artProperties = shape.ArtProperties;
			if(artProperties != null)
				artProperties.Merge(contentBuilder.DrawingGroup.ArtProperties);
		}
		static DrawingObject CreateDrawingObject(Worksheet sheet, OfficeArtClientAnchorSheet clientAnchor) {
			DrawingObject drawingObject = new DrawingObject(sheet);
			drawingObject.AnchorType = AnchorType.TwoCell;
			if(clientAnchor.KeepOnResize && clientAnchor.KeepOnMove)
				drawingObject.ResizingBehavior = AnchorType.Absolute;
			else if(clientAnchor.KeepOnResize)
				drawingObject.ResizingBehavior = AnchorType.OneCell;
			else
				drawingObject.ResizingBehavior = AnchorType.TwoCell;
			PageGridCalculator calculator = new PageGridCalculator(sheet, Rectangle.Empty);
			float columnOffset = CalculateOffsetX(sheet, calculator, clientAnchor.TopLeft.Column, clientAnchor.DeltaXLeft);
			float rowOffset = CalculateOffsetY(sheet, calculator, clientAnchor.TopLeft.Row, clientAnchor.DeltaYTop);
			drawingObject.From = new AnchorPoint(sheet.SheetId, clientAnchor.TopLeft.Column, clientAnchor.TopLeft.Row, columnOffset, rowOffset);
			columnOffset = CalculateOffsetX(sheet, calculator, clientAnchor.BottomRight.Column, clientAnchor.DeltaXRight);
			rowOffset = CalculateOffsetY(sheet, calculator, clientAnchor.BottomRight.Row, clientAnchor.DeltaYBottom);
			drawingObject.To = new AnchorPoint(sheet.SheetId, clientAnchor.BottomRight.Column, clientAnchor.BottomRight.Row, columnOffset, rowOffset);
			return drawingObject;
		}
		static float CalculateOffsetX(Worksheet sheet, PageGridCalculator calculator, int columnIndex, int delta) {
			float layoutColumnWidth = calculator.InnerCalculator.CalculateColumnWidth(sheet, columnIndex, calculator.MaxDigitWidth, calculator.MaxDigitWidthInPixels);
			float modelUnitsColumnWidth = sheet.Workbook.ToDocumentLayoutUnitConverter.ToModelUnits(layoutColumnWidth);
			return modelUnitsColumnWidth * delta / 1024;
		}
		static float CalculateOffsetY(Worksheet sheet, PageGridCalculator calculator, int rowIndex, int delta) {
			float layoutRowHeight = calculator.InnerCalculator.CalculateRowHeight(sheet, rowIndex);
			float modelUnitsRowHeight = sheet.Workbook.ToDocumentLayoutUnitConverter.ToModelUnits(layoutRowHeight);
			return modelUnitsRowHeight * delta / 256;
		}
		static OfficeImage GetImage(XlsContentBuilder contentBuilder, OfficeArtShapeContainer shape) {
			int blipIndex = shape.BlipIndex;
			if(blipIndex != 0)
				return GetBlipImage(contentBuilder, blipIndex);
			string uri = GetImageUri(shape);
			if(!string.IsNullOrEmpty(uri))
				return GetExternalImage(contentBuilder, uri);
			OfficeArtClientData clientData = shape.ClientData;
			if(clientData != null) {
				XlsObjImageData imageData = clientData.Data.EmbeddedImageData;
				if(imageData.HasData)
					return GetEmbeddedImage(imageData.Data);
			}
			return null;
		}
		static OfficeImage GetBlipImage(XlsContentBuilder contentBuilder, int blipIndex) {
			OfficeImage image = null;
			List<BlipBase> blips = contentBuilder.DrawingGroup.BlipContainer.Blips;
			if(blipIndex > 0 && blipIndex <= blips.Count)
				image = blips[blipIndex - 1].Image;
			return image;
		}
		static OfficeImage GetExternalImage(XlsContentBuilder contentBuilder, string uri) {
			try {
				UriBasedOfficeImage loadedImage;
				if(contentBuilder.UniqueUriBasedImages.TryGetValue(uri, out loadedImage))
					return new UriBasedOfficeReferenceImage(loadedImage, 0, 0);
				UriBasedOfficeImage uriImage = new UriBasedOfficeImage(uri, 0, 0, contentBuilder.DocumentModel, false);
				contentBuilder.UniqueUriBasedImages.Add(uri, uriImage);
				return uriImage;
			}
			catch {}
			return null;
		}
		static OfficeImage GetEmbeddedImage(byte[] data) {
			OfficeImage image = null;
			try {
				using(MemoryStream ms = new MemoryStream(data, false)) {
					image = OfficeImage.CreateImage(ms);
				}
			}
			catch {}
			return image;
		}
		static string GetImageUri(OfficeArtShapeContainer shape) {
			OfficeArtProperties artProperties = shape.ArtProperties;
			if(artProperties != null) {
				DrawingBlipFlags blipFlags = artProperties.GetBlipFlagsProperties();
				if(blipFlags != null && blipFlags.LinkToFile)
					return artProperties.BlipName;
			}
			return string.Empty;
		}
		static void SetupPictureProperties(Picture picture, XlsContentBuilder contentBuilder, OfficeArtShapeContainer shape, bool placeHolder) {
			picture.DrawingObject.Properties.Id = shape.ShapeId;
			picture.DrawingObject.Properties.Name = shape.ShapeName;
			picture.PictureFill.Stretch = !placeHolder;
			SetupBlip(picture.PictureFill.Blip, shape);
			OfficeArtClientData clientData = shape.ClientData;
			if(clientData != null) {
				XlsObjCommon commonProperties = clientData.Data.CommonProperties;
				if(commonProperties != null) {
					picture.LocksWithSheet = commonProperties.Locked;
					picture.PrintsWithSheet = commonProperties.Print;
				}
			}
			OfficeArtShapeRecord shapeRecord = shape.ShapeRecord;
			if(shapeRecord != null) {
				picture.ShapeProperties.Transform2D.FlipH = shapeRecord.FlipH;
				picture.ShapeProperties.Transform2D.FlipV = shapeRecord.FlipV;
				picture.ShapeProperties.ShapeType = GetShapeType(shapeRecord.HeaderInstanceInfo, ShapeType.Rect);
			}
			OfficeArtProperties artProperties = shape.ArtProperties;
			if(artProperties != null) {
				picture.DrawingObject.Properties.Description = artProperties.ShapeDescription;
				picture.ShapeProperties.BlackAndWhiteMode = ConvertBlackWhiteMode(artProperties.GetBlackAndWhiteMode());
				DrawingGroupShapeBooleanProperties bpGroupShape = artProperties.GetGroupShapeBooleanProperties();
				if(bpGroupShape != null) {
					if(bpGroupShape.UseHidden)
						picture.DrawingObject.Properties.Hidden = bpGroupShape.Hidden;
				}
				SetupOutlineProperties(picture.ShapeProperties, contentBuilder, artProperties);
				SetupPictureProtectionProperties(picture.Locks, artProperties);
				SetupPictureTransform(picture, contentBuilder, artProperties);
				SetupHyperlink(picture.DrawingObject, contentBuilder, artProperties);
			}
			SetupHyperlinkTooltip(picture.DrawingObject, shape.TertiaryArtProperties);
		}
		static void SetupBlip(DrawingBlip blip, OfficeArtShapeContainer shape) {
			string uri = GetImageUri(shape);
			if(shape.BlipIndex > 0 || string.IsNullOrEmpty(uri))
				blip.SetEmbedded();
			else
				blip.SetExternal(uri);
		}
		static void SetupOutlineProperties(ShapeProperties shapeProperties, XlsContentBuilder contentBuilder, OfficeArtProperties artProperties) {
			shapeProperties.Outline.BeginUpdate();
			try {
				if(artProperties.UseLine && artProperties.Line) {
					shapeProperties.OutlineType = OutlineType.Solid;
					shapeProperties.Outline.CompoundType = artProperties.LineCompoundType;
					shapeProperties.Outline.EndCapStyle = artProperties.LineCapStyle;
					shapeProperties.Outline.Dashing = artProperties.LineDashing;
					DrawingLineColor outlineColor = artProperties.GetLineColorProperties();
					DocumentModel documentModel = contentBuilder.DocumentModel;
					if(outlineColor != null) {
						if(outlineColor.ColorRecord.SystemColorUsed) {
							SystemColorValues systemColor = SystemColorValues.Empty;
							if(Enum.IsDefined(typeof(SystemColorValues), outlineColor.ColorRecord.SystemColorIndex))
								systemColor = (SystemColorValues) outlineColor.ColorRecord.SystemColorIndex;
							shapeProperties.OutlineColor.OriginalColor.System = systemColor;
						}
						else {
							Color color = outlineColor.ColorRecord.Color;
							if(outlineColor.ColorRecord.ColorSchemeUsed)
								color = ColorModelInfo.Create(outlineColor.ColorRecord.ColorSchemeIndex).ToRgb(documentModel.StyleSheet.Palette, documentModel.OfficeTheme.Colors);
							shapeProperties.OutlineColor.OriginalColor.Rgb = color;
						}
					}
					else
						shapeProperties.OutlineColor.OriginalColor.Rgb = artProperties.LineColor;
					shapeProperties.Outline.Width = documentModel.UnitConverter.EmuToModelUnits(artProperties.LineWidthInEMUs);
				}
				else {
					shapeProperties.OutlineType = OutlineType.None;
					shapeProperties.Outline.CompoundType = OutlineCompoundType.Single;
					shapeProperties.Outline.Width = 0;
				}
				shapeProperties.Outline.JoinStyle = artProperties.LineJoinStyle;
				shapeProperties.Outline.MiterLimit = DrawingValueConverter.ToPercentage(artProperties.LineMiterLimit);
			}
			finally {
				shapeProperties.Outline.EndUpdate();
			}
		}
		static void SetupModelShapeFillProperties(ShapeProperties shapeProperties, XlsContentBuilder contentBuilder, OfficeArtShapeContainer shape) {
			ModelShapeImportFillHelper.SetupShapeProperties(contentBuilder, shapeProperties, shape.ArtProperties, shape.TertiaryArtProperties);
		}
		static void SetupCommonDrawingProtectionProperties(IDrawingLocks drawingLocks, OfficeArtProperties artProperties) {
			DrawingBooleanProtectionProperties bpProtection = artProperties.GetProtectionProperties();
			if(bpProtection == null)
				return;
			if(bpProtection.UseLockGroup)
				drawingLocks.NoGroup = bpProtection.LockGroup;
			if(bpProtection.UseLockAdjustHandles)
				drawingLocks.NoAdjustHandles = bpProtection.LockAdjustHandles;
			if(bpProtection.UseLockVertices)
				drawingLocks.NoEditPoints = bpProtection.LockVertices;
			if(bpProtection.UseLockSelect)
				drawingLocks.NoSelect = bpProtection.LockSelect;
			if(bpProtection.UseLockPosition)
				drawingLocks.NoMove = bpProtection.LockPosition;
			if(bpProtection.UseLockAspectRatio)
				drawingLocks.NoChangeAspect = bpProtection.LockAspectRatio;
			if(bpProtection.UseLockRotation)
				drawingLocks.NoRotate = bpProtection.LockRotation;
		}
		static void SetupPictureProtectionProperties(IPictureLocks locks, OfficeArtProperties artProperties) {
			SetupCommonDrawingProtectionProperties(locks, artProperties);
			DrawingBooleanProtectionProperties bpProtection = artProperties.GetProtectionProperties();
			if(bpProtection == null)
				return;
			if(bpProtection.UseLockCropping)
				locks.NoCrop = bpProtection.LockCropping;
		}
		static void SetupModelShapeProtectionProperties(IShapeLocks locks, OfficeArtProperties artProperties) {
			SetupCommonDrawingProtectionProperties(locks, artProperties);
			DrawingBooleanProtectionProperties bpProtection = artProperties.GetProtectionProperties();
			if(bpProtection == null)
				return;
			if(bpProtection.UseLockText)
				locks.NoTextEdit = bpProtection.LockText;
		}
		static void SetupTransformCore(IDrawingLocks locks, ShapeProperties shapeProperties, XlsContentBuilder contentBuilder, OfficeArtProperties artProperties, DrawingShapeBooleanProperties bpShape) {
			if(bpShape != null) {
				if(bpShape.UseLockShapeType)
					locks.NoChangeShapeType = bpShape.LockShapeType;
				if(bpShape.UseFlipHOverride)
					shapeProperties.Transform2D.FlipH = bpShape.FlipHOverride;
				if(bpShape.UseFlipVOverride)
					shapeProperties.Transform2D.FlipV = bpShape.FlipVOverride;
			}
			if(artProperties.Rotation != 0.0)
				shapeProperties.RotateCore(contentBuilder.DocumentModel.UnitConverter.DegreeToModelUnits((float) artProperties.Rotation));
		}
		static void SetupShapeTransform(ModelShape shape, XlsContentBuilder contentBuilder, OfficeArtProperties artProperties) {
			DrawingShapeBooleanProperties bpShape = artProperties.GetShapeBooleanProperties();
			SetupTransformCore(shape.Locks, shape.ShapeProperties, contentBuilder, artProperties, bpShape);
		}
		static void SetupPictureTransform(Picture picture, XlsContentBuilder contentBuilder, OfficeArtProperties artProperties) {
			DrawingShapeBooleanProperties bpShape = artProperties.GetShapeBooleanProperties();
			if(bpShape != null) {
				if(bpShape.UsePreferRelativeResize)
					picture.Properties.PreferRelativeResize = bpShape.PreferRelativeResize;
			}
			SetupTransformCore(picture.Locks, picture.ShapeProperties, contentBuilder, artProperties, bpShape);
		}
		static void SetupHyperlink(DrawingObject drawingObject, XlsContentBuilder contentBuilder, OfficeArtProperties artProperties) {
			DrawingShapeHyperlink prop = artProperties.GetHyperlinkProperties();
			if(prop != null) {
				XlsHyperlinkObject hyperlink = XlsHyperlinkObject.FromData(prop.HyperlinkData);
				if(hyperlink.HasMoniker) {
					if(hyperlink.IsMonkerSavedAsString) {
						Uri uri;
						if(Uri.TryCreate(hyperlink.Moniker, UriKind.RelativeOrAbsolute, out uri))
							SetupHyperlink(drawingObject, contentBuilder, hyperlink, hyperlink.Moniker, true);
					}
					else {
						if(hyperlink.OleMoniker.ClassId == XlsHyperlinkMonikerFactory.CLSID_URLMoniker) {
							XlsHyperlinkURLMoniker urlMoniker = hyperlink.OleMoniker as XlsHyperlinkURLMoniker;
							SetupHyperlink(drawingObject, contentBuilder, hyperlink, urlMoniker.Url, true);
						}
						else if(hyperlink.OleMoniker.ClassId == XlsHyperlinkMonikerFactory.CLSID_FileMoniker) {
							XlsHyperlinkFileMoniker fileMoniker = hyperlink.OleMoniker as XlsHyperlinkFileMoniker;
							SetupHyperlink(drawingObject, contentBuilder, hyperlink, fileMoniker.Path, true);
						}
					}
				}
				else if(hyperlink.HasLocationString) {
					string location = hyperlink.Location;
					if(!location.StartsWith("#"))
						location = "#" + location;
					SetupHyperlink(drawingObject, contentBuilder, hyperlink, location, false);
				}
			}
		}
		static void SetupHyperlink(DrawingObject drawingObject, XlsContentBuilder contentBuilder, XlsHyperlinkObject hyperlink, string targetUri, bool isExternal) {
			if(isExternal) {
				Uri uri;
				if(!Uri.TryCreate(targetUri, UriKind.RelativeOrAbsolute, out uri)) {
					string msg = string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_InvalidHyperlinkRemoved), targetUri);
					contentBuilder.LogMessage(LogCategory.Warning, msg);
					return;
				}
				if(hyperlink.HasLocationString)
					targetUri += "#" + hyperlink.Location;
			}
			drawingObject.BeginUpdate();
			try {
				drawingObject.Properties.HyperlinkClickUrl = targetUri;
				drawingObject.Properties.HyperlinkClickIsExternal = isExternal;
				drawingObject.Properties.HyperlinkClickTargetFrame = hyperlink.HasFrameName ? hyperlink.FrameName : string.Empty;
			}
			finally {
				drawingObject.EndUpdate();
			}
		}
		static void SetupHyperlinkTooltip(DrawingObject drawingObject, OfficeArtTertiaryProperties artProperties) {
			if(artProperties == null || string.IsNullOrEmpty(drawingObject.Properties.HyperlinkClickUrl))
				return;
			DrawingShapeTooltip property = OfficeArtPropertiesHelper.GetPropertyByType(artProperties, typeof(DrawingShapeTooltip)) as DrawingShapeTooltip;
			if(property != null)
				drawingObject.Properties.HyperlinkClickTooltip = property.Data.TrimEnd(new char[] {'\0'});
		}
		static OpenXmlBlackWhiteMode ConvertBlackWhiteMode(BlackWhiteMode mode) {
			switch(mode) {
				case BlackWhiteMode.Normal:
					return OpenXmlBlackWhiteMode.Clr;
				case BlackWhiteMode.GrayScale:
					return OpenXmlBlackWhiteMode.Gray;
				case BlackWhiteMode.LightGrayScale:
					return OpenXmlBlackWhiteMode.LtGray;
				case BlackWhiteMode.InverseGray:
					return OpenXmlBlackWhiteMode.InvGray;
				case BlackWhiteMode.GrayOutline:
					return OpenXmlBlackWhiteMode.GrayWhite;
				case BlackWhiteMode.BlackTextLine:
					return OpenXmlBlackWhiteMode.BlackGray;
				case BlackWhiteMode.HighContrast:
					return OpenXmlBlackWhiteMode.BlackWhite;
				case BlackWhiteMode.Black:
					return OpenXmlBlackWhiteMode.Black;
				case BlackWhiteMode.White:
					return OpenXmlBlackWhiteMode.White;
				case BlackWhiteMode.DontShow:
					return OpenXmlBlackWhiteMode.Hidden;
			}
			return OpenXmlBlackWhiteMode.Auto;
		}
		#region GetShapeType
		static ShapeType GetShapeType(int value, ShapeType defaultShapeType) {
			switch(value) {
				case ShapeTypeCode.Rectangle:
					return ShapeType.Rect;
				case ShapeTypeCode.RoundRect:
					return ShapeType.RoundRect;
				case ShapeTypeCode.Ellipse:
					return ShapeType.Ellipse;
				case ShapeTypeCode.Diamond:
					return ShapeType.Diamond;
				case ShapeTypeCode.IsocelesTriangle:
					return ShapeType.Triangle;
				case ShapeTypeCode.RightTriangle:
					return ShapeType.RtTriangle;
				case ShapeTypeCode.Parallelogram:
					return ShapeType.Parallelogram;
				case ShapeTypeCode.Trapezoid:
					return ShapeType.Trapezoid;
				case ShapeTypeCode.Hexagon:
					return ShapeType.Hexagon;
				case ShapeTypeCode.Octagon:
					return ShapeType.Octagon;
				case ShapeTypeCode.Plus:
					return ShapeType.Plus;
				case ShapeTypeCode.Star:
					return ShapeType.Star5;
				case ShapeTypeCode.Arrow:
					return ShapeType.RightArrow;
				case ShapeTypeCode.HomePlate:
					return ShapeType.HomePlate;
				case ShapeTypeCode.Cube:
					return ShapeType.Cube;
				case ShapeTypeCode.Balloon:
					return ShapeType.Rect;
				case ShapeTypeCode.Seal:
					return ShapeType.Star16;
				case ShapeTypeCode.Arc:
					return ShapeType.Arc;
				case ShapeTypeCode.Line:
					return ShapeType.Line;
				case ShapeTypeCode.Plaque:
					return ShapeType.Plaque;
				case ShapeTypeCode.Can:
					return ShapeType.Can;
				case ShapeTypeCode.Donut:
					return ShapeType.Donut;
				case ShapeTypeCode.StraightConnector1:
					return ShapeType.StraightConnector1;
				case ShapeTypeCode.BentConnector2:
					return ShapeType.BentConnector2;
				case ShapeTypeCode.BentConnector3:
					return ShapeType.BentConnector3;
				case ShapeTypeCode.BentConnector4:
					return ShapeType.BentConnector4;
				case ShapeTypeCode.BentConnector5:
					return ShapeType.BentConnector5;
				case ShapeTypeCode.CurvedConnector2:
					return ShapeType.CurvedConnector2;
				case ShapeTypeCode.CurvedConnector3:
					return ShapeType.CurvedConnector3;
				case ShapeTypeCode.CurvedConnector4:
					return ShapeType.CurvedConnector4;
				case ShapeTypeCode.CurvedConnector5:
					return ShapeType.CurvedConnector5;
				case ShapeTypeCode.Callout1:
					return ShapeType.Callout1;
				case ShapeTypeCode.Callout2:
					return ShapeType.Callout2;
				case ShapeTypeCode.Callout3:
					return ShapeType.Callout3;
				case ShapeTypeCode.AccentCallout1:
					return ShapeType.AccentCallout1;
				case ShapeTypeCode.AccentCallout2:
					return ShapeType.AccentCallout2;
				case ShapeTypeCode.AccentCallout3:
					return ShapeType.AccentCallout3;
				case ShapeTypeCode.BorderCallout1:
					return ShapeType.BorderCallout1;
				case ShapeTypeCode.BorderCallout2:
					return ShapeType.BorderCallout2;
				case ShapeTypeCode.BorderCallout3:
					return ShapeType.BorderCallout3;
				case ShapeTypeCode.AccentBorderCallout1:
					return ShapeType.AccentBorderCallout1;
				case ShapeTypeCode.AccentBorderCallout2:
					return ShapeType.AccentBorderCallout2;
				case ShapeTypeCode.AccentBorderCallout3:
					return ShapeType.AccentBorderCallout3;
				case ShapeTypeCode.Ribbon:
					return ShapeType.Ribbon;
				case ShapeTypeCode.Ribbon2:
					return ShapeType.Ribbon2;
				case ShapeTypeCode.Chevron:
					return ShapeType.Chevron;
				case ShapeTypeCode.Pentagon:
					return ShapeType.Pentagon;
				case ShapeTypeCode.NoSmoking:
					return ShapeType.NoSmoking;
				case ShapeTypeCode.Seal8:
					return ShapeType.Star8;
				case ShapeTypeCode.Seal16:
					return ShapeType.Star16;
				case ShapeTypeCode.Seal32:
					return ShapeType.Star32;
				case ShapeTypeCode.WedgeRectCallout:
					return ShapeType.WedgeRectCallout;
				case ShapeTypeCode.WedgeRRectCallout:
					return ShapeType.WedgeRoundRectCallout;
				case ShapeTypeCode.WedgeEllipseCallout:
					return ShapeType.WedgeEllipseCallout;
				case ShapeTypeCode.Wave:
					return ShapeType.Wave;
				case ShapeTypeCode.FoldedCorner:
					return ShapeType.FoldedCorner;
				case ShapeTypeCode.LeftArrow:
					return ShapeType.LeftArrow;
				case ShapeTypeCode.DownArrow:
					return ShapeType.DownArrow;
				case ShapeTypeCode.UpArrow:
					return ShapeType.UpArrow;
				case ShapeTypeCode.LeftRightArrow:
					return ShapeType.LeftRightArrow;
				case ShapeTypeCode.UpDownArrow:
					return ShapeType.UpDownArrow;
				case ShapeTypeCode.IrregularSeal1:
					return ShapeType.IrregularSeal1;
				case ShapeTypeCode.IrregularSeal2:
					return ShapeType.IrregularSeal2;
				case ShapeTypeCode.LightningBolt:
					return ShapeType.LightningBolt;
				case ShapeTypeCode.Heart:
					return ShapeType.Heart;
				case ShapeTypeCode.QuadArrow:
					return ShapeType.QuadArrow;
				case ShapeTypeCode.LeftArrowCallout:
					return ShapeType.LeftArrowCallout;
				case ShapeTypeCode.RightArrowCallout:
					return ShapeType.RightArrowCallout;
				case ShapeTypeCode.UpArrowCallout:
					return ShapeType.UpArrowCallout;
				case ShapeTypeCode.DownArrowCallout:
					return ShapeType.DownArrowCallout;
				case ShapeTypeCode.LeftRightArrowCallout:
					return ShapeType.LeftRightArrowCallout;
				case ShapeTypeCode.UpDownArrowCallout:
					return ShapeType.UpDownArrowCallout;
				case ShapeTypeCode.QuadArrowCallout:
					return ShapeType.QuadArrowCallout;
				case ShapeTypeCode.Bevel:
					return ShapeType.Bevel;
				case ShapeTypeCode.LeftBracket:
					return ShapeType.LeftBracket;
				case ShapeTypeCode.RightBracket:
					return ShapeType.RightBracket;
				case ShapeTypeCode.LeftBrace:
					return ShapeType.LeftBrace;
				case ShapeTypeCode.RightBrace:
					return ShapeType.RightBrace;
				case ShapeTypeCode.LeftUpArrow:
					return ShapeType.LeftUpArrow;
				case ShapeTypeCode.BentUpArrow:
					return ShapeType.BentUpArrow;
				case ShapeTypeCode.BentArrow:
					return ShapeType.BentArrow;
				case ShapeTypeCode.Seal24:
					return ShapeType.Star24;
				case ShapeTypeCode.StripedRightArrow:
					return ShapeType.StripedRightArrow;
				case ShapeTypeCode.NotchedRightArrow:
					return ShapeType.NotchedRightArrow;
				case ShapeTypeCode.BlockArc:
					return ShapeType.BlockArc;
				case ShapeTypeCode.SmileyFace:
					return ShapeType.SmileyFace;
				case ShapeTypeCode.VerticalScroll:
					return ShapeType.VerticalScroll;
				case ShapeTypeCode.HorizontalScroll:
					return ShapeType.HorizontalScroll;
				case ShapeTypeCode.CircularArrow:
					return ShapeType.CircularArrow;
				case ShapeTypeCode.UturnArrow:
					return ShapeType.UturnArrow;
				case ShapeTypeCode.CurvedRightArrow:
					return ShapeType.CurvedRightArrow;
				case ShapeTypeCode.CurvedLeftArrow:
					return ShapeType.CurvedLeftArrow;
				case ShapeTypeCode.CurvedUpArrow:
					return ShapeType.CurvedUpArrow;
				case ShapeTypeCode.CurvedDownArrow:
					return ShapeType.CurvedDownArrow;
				case ShapeTypeCode.CloudCallout:
					return ShapeType.CloudCallout;
				case ShapeTypeCode.EllipseRibbon:
					return ShapeType.EllipseRibbon;
				case ShapeTypeCode.EllipseRibbon2:
					return ShapeType.EllipseRibbon2;
				case ShapeTypeCode.FlowChartProcess:
					return ShapeType.FlowChartProcess;
				case ShapeTypeCode.FlowChartDecision:
					return ShapeType.FlowChartDecision;
				case ShapeTypeCode.FlowChartInputOutput:
					return ShapeType.FlowChartInputOutput;
				case ShapeTypeCode.FlowChartPredefinedProcess:
					return ShapeType.FlowChartPredefinedProcess;
				case ShapeTypeCode.FlowChartInternalStorage:
					return ShapeType.FlowChartInternalStorage;
				case ShapeTypeCode.FlowChartDocument:
					return ShapeType.FlowChartDocument;
				case ShapeTypeCode.FlowChartMultidocument:
					return ShapeType.FlowChartMultidocument;
				case ShapeTypeCode.FlowChartTerminator:
					return ShapeType.FlowChartTerminator;
				case ShapeTypeCode.FlowChartPreparation:
					return ShapeType.FlowChartPreparation;
				case ShapeTypeCode.FlowChartManualInput:
					return ShapeType.FlowChartManualInput;
				case ShapeTypeCode.FlowChartManualOperation:
					return ShapeType.FlowChartManualOperation;
				case ShapeTypeCode.FlowChartConnector:
					return ShapeType.FlowChartConnector;
				case ShapeTypeCode.FlowChartPunchedCard:
					return ShapeType.FlowChartPunchedCard;
				case ShapeTypeCode.FlowChartPunchedTape:
					return ShapeType.FlowChartPunchedTape;
				case ShapeTypeCode.FlowChartSummingJunction:
					return ShapeType.FlowChartSummingJunction;
				case ShapeTypeCode.FlowChartOr:
					return ShapeType.FlowChartOr;
				case ShapeTypeCode.FlowChartCollate:
					return ShapeType.FlowChartCollate;
				case ShapeTypeCode.FlowChartSort:
					return ShapeType.FlowChartSort;
				case ShapeTypeCode.FlowChartExtract:
					return ShapeType.FlowChartExtract;
				case ShapeTypeCode.FlowChartMerge:
					return ShapeType.FlowChartMerge;
				case ShapeTypeCode.FlowChartOfflineStorage:
					return ShapeType.FlowChartOfflineStorage;
				case ShapeTypeCode.FlowChartOnlineStorage:
					return ShapeType.FlowChartOnlineStorage;
				case ShapeTypeCode.FlowChartMagneticTape:
					return ShapeType.FlowChartMagneticTape;
				case ShapeTypeCode.FlowChartMagneticDisk:
					return ShapeType.FlowChartMagneticDisk;
				case ShapeTypeCode.FlowChartMagneticDrum:
					return ShapeType.FlowChartMagneticDrum;
				case ShapeTypeCode.FlowChartDisplay:
					return ShapeType.FlowChartDisplay;
				case ShapeTypeCode.FlowChartDelay:
					return ShapeType.FlowChartDelay;
				case ShapeTypeCode.FlowChartAlternateProcess:
					return ShapeType.FlowChartAlternateProcess;
				case ShapeTypeCode.FlowChartOffpageConnector:
					return ShapeType.FlowChartOffpageConnector;
				case ShapeTypeCode.Callout90:
					return ShapeType.Callout1;
				case ShapeTypeCode.AccentCallout90:
					return ShapeType.AccentCallout1;
				case ShapeTypeCode.BorderCallout90:
					return ShapeType.BorderCallout1;
				case ShapeTypeCode.AccentBorderCallout90:
					return ShapeType.AccentBorderCallout1;
				case ShapeTypeCode.LeftRightUpArrow:
					return ShapeType.LeftRightUpArrow;
				case ShapeTypeCode.Sun:
					return ShapeType.Sun;
				case ShapeTypeCode.Moon:
					return ShapeType.Moon;
				case ShapeTypeCode.BracketPair:
					return ShapeType.BracketPair;
				case ShapeTypeCode.BracePair:
					return ShapeType.BracePair;
				case ShapeTypeCode.Seal4:
					return ShapeType.Star4;
				case ShapeTypeCode.DoubleWave:
					return ShapeType.DoubleWave;
				case ShapeTypeCode.ActionButtonBlank:
					return ShapeType.ActionButtonBlank;
				case ShapeTypeCode.ActionButtonHome:
					return ShapeType.ActionButtonHome;
				case ShapeTypeCode.ActionButtonHelp:
					return ShapeType.ActionButtonHelp;
				case ShapeTypeCode.ActionButtonInformation:
					return ShapeType.ActionButtonInformation;
				case ShapeTypeCode.ActionButtonForwardNext:
					return ShapeType.ActionButtonForwardNext;
				case ShapeTypeCode.ActionButtonBackPrevious:
					return ShapeType.ActionButtonBackPrevious;
				case ShapeTypeCode.ActionButtonEnd:
					return ShapeType.ActionButtonEnd;
				case ShapeTypeCode.ActionButtonBeginning:
					return ShapeType.ActionButtonBeginning;
				case ShapeTypeCode.ActionButtonReturn:
					return ShapeType.ActionButtonReturn;
				case ShapeTypeCode.ActionButtonDocument:
					return ShapeType.ActionButtonDocument;
				case ShapeTypeCode.ActionButtonSound:
					return ShapeType.ActionButtonSound;
				case ShapeTypeCode.ActionButtonMovie:
					return ShapeType.ActionButtonMovie;
				case ShapeTypeCode.Textbox:
					return ShapeType.Rect;
			}
			return defaultShapeType;
		}
		#endregion
	}
	#endregion
	#region ModelShapeTextHelper
	public static class ModelShapeTextHelper {
		public static void SetupModelShapeText(XlsContentBuilder contentBuilder, OfficeArtShapeContainer shape, ModelShape modelShape) {
			OfficeArtClientTextbox clientTextbox = shape.ClientTextbox;
			if(clientTextbox == null) {
				CreateDefaultParagrah(contentBuilder, modelShape);
				return;
			}
			XlsTextObjData textObjData = clientTextbox.Data;
			if(textObjData == null) {
				CreateDefaultParagrah(contentBuilder, modelShape);
				return;
			}
			string text = clientTextbox.Data.Text;
			if(string.IsNullOrEmpty(text)) {
				CreateDefaultParagrah(contentBuilder, modelShape);
				return;
			}
			IList<XlsFormatRun> formatRuns = clientTextbox.Data.FormatRuns;
			if(formatRuns.Count == 0)
				AddTextRun(modelShape, text, -1);
			else {
				int lastCharIndex = 0;
				int lastFontIndex = 0;
				int length = text.Length;
				foreach(XlsFormatRun formatRun in formatRuns) {
					if(formatRun.CharIndex >= length)
						break; 
					int runLength = formatRun.CharIndex - lastCharIndex;
					if(runLength > 0) {
						AddTextRun(modelShape, text.Substring(lastCharIndex, runLength), contentBuilder.StyleSheet.GetFontInfoIndex(lastFontIndex));
					}
					lastCharIndex = formatRun.CharIndex;
					lastFontIndex = formatRun.FontIndex;
					if(lastFontIndex == XlsDefs.UnusedFontIndex)
						contentBuilder.ThrowInvalidFile("Invalid font index in comment run");
					else if(lastFontIndex > XlsDefs.UnusedFontIndex)
						lastFontIndex--;
				}
				if((length - lastCharIndex) > 0) {
					AddTextRun(modelShape, text.Substring(lastCharIndex, length - lastCharIndex), contentBuilder.StyleSheet.GetFontInfoIndex(lastFontIndex));
				}
			}
			SetupBodyProperties(textObjData, modelShape.TextProperties.BodyProperties, shape.ArtProperties);
			SetupParagraphProperties(textObjData, modelShape.TextProperties);
		}
		static void CreateDefaultParagrah(XlsContentBuilder contentBuilder, ModelShape modelShape) {
			modelShape.TextProperties.Paragraphs.Add(new DrawingTextParagraph(contentBuilder.DocumentModel) {ApplyParagraphProperties = false, ApplyEndRunProperties = true});
		}
		static void AddTextRun(ModelShape modelShape, string runText, int fontIndex) {
			const string newLine = "\n";
			if(string.IsNullOrEmpty(runText))
				return;
			DocumentModel documentModel = modelShape.DocumentModel;
			int runTextLength = runText.Length;
			bool emptyLine = runText == newLine;
			bool lineBreak = runText.Substring(runTextLength - 1, 1) == newLine;
			TextProperties textProperties = modelShape.TextProperties;
			if(textProperties.Paragraphs.Count == 0) {
				textProperties.Paragraphs.Add(new DrawingTextParagraph(documentModel));
			}
			DrawingTextParagraph currentParagraph = textProperties.Paragraphs[textProperties.Paragraphs.Count - 1];
			if(lineBreak) {
				textProperties.Paragraphs.Add(new DrawingTextParagraph(documentModel));
			}
			DrawingTextRunCollection runs = currentParagraph.Runs;
			RunFontInfo fontInfo = documentModel.Cache.FontInfoCache[fontIndex];
			if(emptyLine) {
				XlsCharacterPropertiesHelper.SetupCharacterProperties(currentParagraph.EndRunProperties, fontInfo, documentModel);
				currentParagraph.ApplyEndRunProperties = true;
			}
			else {
				if(lineBreak)
					runText = runText.Substring(0, runTextLength - 1);
				runs.Add(new DrawingTextRun(documentModel, runText));
				DrawingTextRunBase drawingTextRunBase = runs[runs.Count - 1] as DrawingTextRunBase;
				if(drawingTextRunBase != null)
					XlsCharacterPropertiesHelper.SetupCharacterProperties(drawingTextRunBase.RunProperties, fontInfo, documentModel);
			}
		}
		static void SetupBodyProperties(XlsTextObjData textObjData, DrawingTextBodyProperties bodyProperties, OfficeArtProperties artProperties) {
			SetupAnchor(textObjData, bodyProperties);
			SetupInset(bodyProperties, artProperties);
		}
		static void SetupInset(DrawingTextBodyProperties bodyProperties, OfficeArtProperties artProperties) {
			if(artProperties == null)
				return;
			DocumentModelUnitConverter unitConverter = bodyProperties.DocumentModel.UnitConverter;
			bodyProperties.Inset.Left = unitConverter.EmuToModelUnits(artProperties.TextLeft);
			bodyProperties.Inset.Top = unitConverter.EmuToModelUnits(artProperties.TextTop);
			bodyProperties.Inset.Bottom = unitConverter.EmuToModelUnits(artProperties.TextBottom);
			bodyProperties.Inset.Right = unitConverter.EmuToModelUnits(artProperties.TextRight);
		}
		static void SetupAnchor(XlsTextObjData textObjData, DrawingTextBodyProperties bodyProperties) {
			switch(textObjData.VerticalAlignment) {
				case XlVerticalAlignment.Top:
					bodyProperties.Anchor = DrawingTextAnchoringType.Top;
					break;
				case XlVerticalAlignment.Center:
					bodyProperties.Anchor = DrawingTextAnchoringType.Center;
					break;
				case XlVerticalAlignment.Bottom:
					bodyProperties.Anchor = DrawingTextAnchoringType.Bottom;
					break;
				case XlVerticalAlignment.Justify:
					bodyProperties.Anchor = DrawingTextAnchoringType.Justified;
					break;
				case XlVerticalAlignment.Distributed:
					bodyProperties.Anchor = DrawingTextAnchoringType.Distributed;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		static void SetupParagraphProperties(XlsTextObjData textObjData, TextProperties textProperties) {
			foreach(DrawingTextParagraph paragraph in textProperties.Paragraphs) {
				paragraph.ApplyParagraphProperties = true;
				DrawingTextParagraphProperties paragraphProperties = paragraph.ParagraphProperties;
				switch(textObjData.HorizontalAlignment) {
					case XlHorizontalAlignment.General:
						break;
					case XlHorizontalAlignment.Left:
						paragraphProperties.TextAlignment = DrawingTextAlignmentType.Left;
						break;
					case XlHorizontalAlignment.Center:
						paragraphProperties.TextAlignment = DrawingTextAlignmentType.Center;
						break;
					case XlHorizontalAlignment.Right:
						paragraphProperties.TextAlignment = DrawingTextAlignmentType.Right;
						break;
					case XlHorizontalAlignment.Fill:
					case XlHorizontalAlignment.Justify:
						paragraphProperties.TextAlignment = DrawingTextAlignmentType.Justified;
						break;
					case XlHorizontalAlignment.CenterContinuous:
						break;
					case XlHorizontalAlignment.Distributed:
						paragraphProperties.TextAlignment = DrawingTextAlignmentType.Distributed;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}   
	}
	#endregion
	#region ModelShapeFillHelper
	public static class ModelShapeImportFillHelper {
		#region SetupShapeProperties
		public static void SetupShapeProperties( XlsContentBuilder contentBuilder, ShapeProperties properties, OfficeArtProperties artProperties, OfficeArtTertiaryProperties tertiaryProperties) {
			if(artProperties == null)
				return;
			if(!artProperties.UseFilled)
				return;
			if(!artProperties.Filled)
				return;
			OfficeDrawingFillType fillTypeProp = artProperties.GetPropertyByType(typeof(OfficeDrawingFillType)) as OfficeDrawingFillType;
			OfficeFillType fillType = fillTypeProp != null ? fillTypeProp.FillType : OfficeFillType.Solid;
			switch(fillType) {
				case OfficeFillType.Solid:
					SetupShapeSolidFill(properties, artProperties);
					break;
				case OfficeFillType.Pattern:
					SetupShapePatternFill(properties, artProperties, contentBuilder);
					break;
				case OfficeFillType.Texture:
				case OfficeFillType.Picture:
					SetupShapePictureFill(properties, fillType);
					break;
				case OfficeFillType.Shade:
				case OfficeFillType.ShadeCenter:
				case OfficeFillType.ShadeScale:
				case OfficeFillType.ShadeShape:
				case OfficeFillType.ShadeTile:
					SetupShapeGradientFill(properties, fillType, artProperties);
					break;
				case OfficeFillType.Background:
					properties.Fill = DrawingFill.Group;
					break;
			}
		}
		static void SetupShapeSolidFill(ShapeProperties properties, OfficeArtProperties artProperties) {
			DocumentModel documentModel = properties.DocumentModel;
			DrawingSolidFill fill = new DrawingSolidFill(documentModel);
			SetupFillColor(fill.Color, documentModel, artProperties);
			double opacity = GetFillOpacity(typeof(DrawingFillOpacity), artProperties);
			SetupFillOpacity(fill.Color, opacity);
			properties.Fill = fill;
		}
		static void SetupShapePatternFill(ShapeProperties properties, OfficeArtProperties artProperties, XlsContentBuilder contentBuilder) {
			DocumentModel documentModel = properties.DocumentModel;
			DrawingPatternFill fill = new DrawingPatternFill(documentModel);
			SetupColor(fill.ForegroundColor, GetFillColor(typeof(DrawingFillColor), artProperties), documentModel, artProperties);
			SetupColor(fill.BackgroundColor, GetFillColor(typeof(DrawingFillBackColor), artProperties), documentModel, artProperties);
			SetupFillOpacity(fill.ForegroundColor, GetFillOpacity(typeof(DrawingFillOpacity), artProperties));
			SetupFillOpacity(fill.BackgroundColor, GetFillOpacity(typeof(DrawingFillBackOpacity), artProperties));
			fill.PatternType = GetDrawingPatternType(artProperties, contentBuilder);
			properties.Fill = fill;
		}
		static void SetupShapePictureFill(ShapeProperties properties, OfficeFillType fillType) {
		}
		static void SetupShapeGradientFill(ShapeProperties properties, OfficeFillType fillType, OfficeArtProperties artProperties) {
			DocumentModel documentModel = properties.DocumentModel;
			DrawingGradientFill fill = new DrawingGradientFill(documentModel);
			switch(fillType) {
				case OfficeFillType.ShadeShape:
					fill.GradientType = GradientType.Shape;
					fill.FillRect = new RectangleOffset(50000, 50000, 50000, 50000);
					break;
				case OfficeFillType.ShadeCenter:
					fill.GradientType = GradientType.Rectangle;
					SetupFillRect(fill, artProperties);
					break;
				default:
					fill.GradientType = GradientType.Linear;
					break;
			}
			SetupGradientStops(fill, documentModel, artProperties);
			SetupGradientAngle(fill, artProperties);
			properties.Fill = fill;
		}
		#region Color
		static void SetupColor(IDrawingColor drawingColor, OfficeColorRecord officeColor, DocumentModel documentModel, OfficeArtProperties artProperties) {
			if(officeColor == null || officeColor.IsDefault)
				drawingColor.Rgb = DXColor.FromArgb(0xff, 0xff, 0xff);
			else if(officeColor.SystemColorUsed)
				SetupSystemColor(drawingColor, officeColor, documentModel, artProperties);
			else if(officeColor.ColorSchemeUsed)
				drawingColor.Rgb = ColorModelInfo.Create(officeColor.ColorSchemeIndex).ToRgb(documentModel.StyleSheet.Palette, documentModel.OfficeTheme.Colors);
			else
				drawingColor.Rgb = officeColor.Color;
		}
		static void SetupSystemColor(IDrawingColor drawingColor, OfficeColorRecord officeColor, DocumentModel documentModel, OfficeArtProperties artProperties) {
			if(officeColor.ColorUse == OfficeColorUse.None) {
				SystemColorValues systemColor = SystemColorValues.Empty;
				if(Enum.IsDefined(typeof(SystemColorValues), officeColor.SystemColorIndex))
					systemColor = (SystemColorValues) officeColor.SystemColorIndex;
				drawingColor.System = systemColor;
			}
			else
				SetupSpecialColor(drawingColor, officeColor, documentModel, artProperties);
		}
		static void SetupSpecialColor(IDrawingColor drawingColor, OfficeColorRecord officeColor, DocumentModel documentModel, OfficeArtProperties artProperties) {
			switch(officeColor.ColorUse) {
				case OfficeColorUse.UseFillColor:
					SetupRgbColor(drawingColor, GetFillColor(typeof(DrawingFillColor), artProperties), documentModel);
					break;
				case OfficeColorUse.UseFillBackColor:
					SetupRgbColor(drawingColor, GetFillColor(typeof(DrawingFillBackColor), artProperties), documentModel);
					break;
				case OfficeColorUse.UseFillOrLineColor: {
					OfficeColorRecord colorRecord = GetFillColor(typeof(DrawingFillColor), artProperties);
					SetupRgbColor(drawingColor, colorRecord ?? GetFillColor(typeof(DrawingLineColor), artProperties), documentModel);
				}
					break;
				case OfficeColorUse.UseLineOrFillColor: {
					OfficeColorRecord colorRecord = GetFillColor(typeof(DrawingLineColor), artProperties);
					SetupRgbColor(drawingColor, colorRecord ?? GetFillColor(typeof(DrawingFillColor), artProperties), documentModel);
				}
					break;
				case OfficeColorUse.UseLineColor:
					SetupRgbColor(drawingColor, GetFillColor(typeof(DrawingLineColor), artProperties), documentModel);
					break;
				default:
					SetupRgbColor(drawingColor, null, documentModel);
					break;
			}
			if(officeColor.Transform == OfficeColorTransform.Darken && officeColor.TransformValue < 0xff) {
				drawingColor.Transforms.Add(new GammaColorTransform());
				drawingColor.Transforms.Add(new ShadeColorTransform(officeColor.TransformValue * DrawingValueConstants.ThousandthOfPercentage / 0xff));
				drawingColor.Transforms.Add(new InverseGammaColorTransform());
			}
			if(officeColor.Transform == OfficeColorTransform.Lighten && officeColor.TransformValue < 0xff) {
				drawingColor.Transforms.Add(new GammaColorTransform());
				drawingColor.Transforms.Add(new TintColorTransform(officeColor.TransformValue * DrawingValueConstants.ThousandthOfPercentage / 0xff));
				drawingColor.Transforms.Add(new InverseGammaColorTransform());
			}
		}
		static void SetupRgbColor(IDrawingColor drawingColor, OfficeColorRecord officeColor, DocumentModel documentModel) {
			if(officeColor == null || officeColor.IsDefault || officeColor.SystemColorUsed)
				drawingColor.Rgb = DXColor.FromArgb(0xff, 0xff, 0xff);
			else if(officeColor.ColorSchemeUsed)
				drawingColor.Rgb = ColorModelInfo.Create(officeColor.ColorSchemeIndex).ToRgb(documentModel.StyleSheet.Palette, documentModel.OfficeTheme.Colors);
			else
				drawingColor.Rgb = officeColor.Color;
		}
		static void SetupFillColor(IDrawingColor drawingColor, DocumentModel documentModel, OfficeArtProperties artProperties) {
			DrawingFillColor propColor = artProperties.GetFillColorProperties();
			SetupColor(drawingColor, propColor == null ? null : propColor.ColorRecord, documentModel, artProperties);
		}
		static OfficeColorRecord GetFillColor(Type propType, OfficeArtProperties artProperties) {
			OfficeDrawingColorPropertyBase propColor = artProperties.GetPropertyByType(propType) as OfficeDrawingColorPropertyBase;
			return propColor != null ? propColor.ColorRecord : null;
		}
		#endregion
		#region Opacity
		static void SetupFillOpacity(IDrawingColor drawingColor, double opacity) {
			if(opacity >= 1.0)
				return;
			if(opacity < 0.0)
				opacity = 0.0;
			drawingColor.Transforms.Add(new AlphaColorTransform((int) (opacity * DrawingValueConstants.ThousandthOfPercentage)));
		}
		static double GetFillOpacity(Type propType, OfficeArtProperties artProperties) {
			OfficeDrawingFixedPointPropertyBase propOpacity = artProperties.GetPropertyByType(propType) as OfficeDrawingFixedPointPropertyBase;
			return propOpacity != null ? propOpacity.Value : 1.0;
		}
		#endregion
		#region Pattern
		static Dictionary<int, DrawingPatternType> patternsByTag = new Dictionary<int, DrawingPatternType> {
			{196, DrawingPatternType.Percent5},
			{197, DrawingPatternType.Percent50},
			{198, DrawingPatternType.LightDownwardDiagonal},
			{199, DrawingPatternType.LightVertical},
			{200, DrawingPatternType.DashedDownwardDiagonal},
			{201, DrawingPatternType.ZigZag},
			{202, DrawingPatternType.Divot},
			{203, DrawingPatternType.SmallGrid},
			{204, DrawingPatternType.Percent10},
			{205, DrawingPatternType.Percent60},
			{206, DrawingPatternType.LightUpwardDiagonal},
			{207, DrawingPatternType.LightHorizontal},
			{208, DrawingPatternType.DashedUpwardDiagonal},
			{209, DrawingPatternType.Wave},
			{210, DrawingPatternType.DottedGrid},
			{211, DrawingPatternType.LargeGrid},
			{212, DrawingPatternType.Percent20},
			{213, DrawingPatternType.Percent70},
			{214, DrawingPatternType.DarkDownwardDiagonal},
			{215, DrawingPatternType.NarrowVertical},
			{216, DrawingPatternType.DashedHorizontal},
			{217, DrawingPatternType.DiagonalBrick},
			{218, DrawingPatternType.DottedDiamond},
			{219, DrawingPatternType.SmallCheckerBoard},
			{220, DrawingPatternType.Percent25},
			{221, DrawingPatternType.Percent75},
			{222, DrawingPatternType.DarkUpwardDiagonal},
			{223, DrawingPatternType.NarrowHorizontal},
			{224, DrawingPatternType.DashedVertical},
			{225, DrawingPatternType.HorizontalBrick},
			{226, DrawingPatternType.Shingle},
			{227, DrawingPatternType.LargeCheckerBoard},
			{228, DrawingPatternType.Percent30},
			{229, DrawingPatternType.Percent80},
			{230, DrawingPatternType.WideDownwardDiagonal},
			{231, DrawingPatternType.DarkVertical},
			{232, DrawingPatternType.SmallConfetti},
			{233, DrawingPatternType.Weave},
			{234, DrawingPatternType.Trellis},
			{235, DrawingPatternType.OpenDiamond},
			{236, DrawingPatternType.Percent40},
			{237, DrawingPatternType.Percent90},
			{238, DrawingPatternType.WideUpwardDiagonal},
			{239, DrawingPatternType.DarkHorizontal},
			{240, DrawingPatternType.LargeConfetti},
			{241, DrawingPatternType.Plaid},
			{242, DrawingPatternType.Sphere},
			{243, DrawingPatternType.SolidDiamond},
		};
		static DrawingPatternType GetDrawingPatternType(OfficeArtProperties artProperties, XlsContentBuilder contentBuilder) {
			DrawingPatternType defaultPattern = DrawingPatternType.Percent50;
			DrawingFillBlipIdentifier prop = artProperties.GetPropertyByType(typeof(DrawingFillBlipIdentifier)) as DrawingFillBlipIdentifier;
			if(prop == null || contentBuilder.DrawingGroup == null || contentBuilder.DrawingGroup.BlipContainer.Blips.Count < prop.Value)
				return defaultPattern;
			BlipBase patternBlip = contentBuilder.DrawingGroup.BlipContainer.Blips[prop.Value - 1];
			DrawingPatternType result;
			return patternsByTag.TryGetValue(patternBlip.TagValue, out result) ? result : defaultPattern;
		}
		#endregion
		#region Gradient
		static void SetupGradientStops(DrawingGradientFill gradientFill, DocumentModel documentModel, OfficeArtProperties artProperties) {
			DrawingFillShadeColors prop = artProperties.GetPropertyByType(typeof(DrawingFillShadeColors)) as DrawingFillShadeColors;
			List<OfficeShadeColor> shadeColors = new List<OfficeShadeColor>();
			List<double> shadeOpacities = new List<double>();
			if(prop != null && prop.ComplexData.Length > 0)
				GetShadeColors(shadeColors, shadeOpacities, prop.ComplexData, artProperties);
			else
				GetShadeColors(shadeColors, shadeOpacities, artProperties);
			for(int i = 0; i < shadeColors.Count; i++) {
				DrawingGradientStop gradientStop = new DrawingGradientStop(documentModel);
				SetupColor(gradientStop.Color, shadeColors[i].ColorRecord, documentModel, artProperties);
				gradientStop.Position = (int) (shadeColors[i].Position * DrawingValueConstants.ThousandthOfPercentage);
				SetupFillOpacity(gradientStop.Color, shadeOpacities[i]);
				gradientFill.GradientStops.Add(gradientStop);
			}
		}
		static void GetShadeColors(List<OfficeShadeColor> shadeColors, List<double> shadeOpacities, byte[] data, OfficeArtProperties artProperties) {
			int count;
			using(MemoryStream ms = new MemoryStream(data, false)) {
				using(BinaryReader reader = new BinaryReader(ms)) {
					count = reader.ReadUInt16();
					reader.ReadUInt16(); 
					reader.ReadUInt16(); 
					for(int i = 0; i < count; i++)
						shadeColors.Add(OfficeShadeColor.FromStream(reader));
				}
			}
			double firstOpacity = GetFillOpacity(typeof(DrawingFillOpacity), artProperties);
			double lastOpacity = GetFillOpacity(typeof(DrawingFillBackOpacity), artProperties);
			double firstPosition = shadeColors[0].Position;
			double lastPosition = shadeColors[count - 1].Position;
			double k = (lastPosition == firstPosition) ? 0.0 : ((lastOpacity - firstOpacity) / (lastPosition - firstPosition));
			for(int i = 0; i < count; i++) {
				double opacity = firstOpacity + k * (shadeColors[i].Position - firstPosition);
				shadeOpacities.Add(opacity);
			}
		}
		static void GetShadeColors(List<OfficeShadeColor> shadeColors, List<double> shadeOpacities, OfficeArtProperties artProperties) {
			int focus = 100;
			DrawingFillFocus propFocus = artProperties.GetPropertyByType(typeof(DrawingFillFocus)) as DrawingFillFocus;
			if(propFocus != null)
				focus = propFocus.Value;
			double fillOpacity = GetFillOpacity(typeof(DrawingFillOpacity), artProperties);
			double fillBackOpacity = GetFillOpacity(typeof(DrawingFillBackOpacity), artProperties);
			if(focus <= -100 || focus >= 100) {
				shadeColors.Add(new OfficeShadeColor() { ColorRecord = GetFillColor(typeof(DrawingFillColor), artProperties), Position = 0.0 });
				shadeColors.Add(new OfficeShadeColor() { ColorRecord = GetFillColor(typeof(DrawingFillBackColor), artProperties), Position = 1.0 });
				shadeOpacities.Add(fillOpacity);
				shadeOpacities.Add(fillBackOpacity);
			}
			else if(focus < 0) {
				shadeColors.Add(new OfficeShadeColor() { ColorRecord = GetFillColor(typeof(DrawingFillColor), artProperties), Position = 0.0 });
				shadeColors.Add(new OfficeShadeColor() { ColorRecord = GetFillColor(typeof(DrawingFillBackColor), artProperties), Position = -focus / 100.0 });
				shadeColors.Add(new OfficeShadeColor() { ColorRecord = GetFillColor(typeof(DrawingFillColor), artProperties), Position = 1.0 });
				shadeOpacities.Add(fillOpacity);
				shadeOpacities.Add(fillBackOpacity);
				shadeOpacities.Add(fillOpacity);
			}
			else if(focus > 0) {
				shadeColors.Add(new OfficeShadeColor() { ColorRecord = GetFillColor(typeof(DrawingFillBackColor), artProperties), Position = 0.0 });
				shadeColors.Add(new OfficeShadeColor() { ColorRecord = GetFillColor(typeof(DrawingFillColor), artProperties), Position = focus / 100.0 });
				shadeColors.Add(new OfficeShadeColor() { ColorRecord = GetFillColor(typeof(DrawingFillBackColor), artProperties), Position = 1.0 });
				shadeOpacities.Add(fillBackOpacity);
				shadeOpacities.Add(fillOpacity);
				shadeOpacities.Add(fillBackOpacity);
			}
			else {
				shadeColors.Add(new OfficeShadeColor() { ColorRecord = GetFillColor(typeof(DrawingFillBackColor), artProperties), Position = 0.0 });
				shadeColors.Add(new OfficeShadeColor() { ColorRecord = GetFillColor(typeof(DrawingFillColor), artProperties), Position = 1.0 });
				shadeOpacities.Add(fillBackOpacity);
				shadeOpacities.Add(fillOpacity);
			}
		}
		static void SetupGradientAngle(DrawingGradientFill gradientFill, OfficeArtProperties artProperties) {
			double angle = 0.0;
			DrawingFillAngle prop = artProperties.GetPropertyByType(typeof(DrawingFillAngle)) as DrawingFillAngle;
			if(prop != null)
				angle = prop.Value;
			if(angle >= 0)
				angle = 90.0 - angle;
			else
				angle = -(angle + 90.0);
			if(angle < 0.0)
				angle += 360.0;
			if(angle > 360.0)
				angle -= 360.0;
			gradientFill.Angle = (int) (angle * DrawingValueConstants.OnePositiveFixedAngle);
		}
		static void SetupFillRect(DrawingGradientFill fill, OfficeArtProperties artProperties) {
			double left = GetRelativePosition(typeof(DrawingFillToLeft), artProperties);
			double top = GetRelativePosition(typeof(DrawingFillToTop), artProperties);
			double right = 1.0 - GetRelativePosition(typeof(DrawingFillToRight), artProperties);
			double bottom = 1.0 - GetRelativePosition(typeof(DrawingFillToBottom), artProperties);
			int leftOffset = (int) (left * DrawingValueConstants.ThousandthOfPercentage);
			int topOffset = (int) (top * DrawingValueConstants.ThousandthOfPercentage);
			int rightOffset = (int) (right * DrawingValueConstants.ThousandthOfPercentage);
			int bottomOffset = (int) (bottom * DrawingValueConstants.ThousandthOfPercentage);
			fill.FillRect = new RectangleOffset(bottomOffset, leftOffset, rightOffset, topOffset);
		}
		static double GetRelativePosition(Type propType, OfficeArtProperties artProperties) {
			OfficeDrawingFixedPointPropertyBase prop = artProperties.GetPropertyByType(propType) as OfficeDrawingFixedPointPropertyBase;
			if(prop != null)
				return prop.Value;
			return 0.0;
		}
		#endregion
		#endregion
	}
	#endregion
	#region ModelShapePathImportHelper
	public static class ModelShapePathImportHelper {
		public static void SetupShapeGeometry(ModelShape modelShape, OfficeArtShapeContainer shape) {
			if(modelShape.ShapeProperties.ShapeType == ShapeType.None)
				SetupShapeCustomGeometry(modelShape, shape);
			else
				SetupPresetGeometry(modelShape, shape);
		}
		static void SetupPresetGeometry(ModelShape modelShape, OfficeArtShapeContainer shape) {
		}
		static Rectangle GetShapeGeometrySpace(ModelShape modelShape, OfficeArtShapeContainer shape) {
			OfficeArtProperties artProperties = shape.ArtProperties;
			if(artProperties == null)
				return new Rectangle();
			int geometrySpaceLeft = 0;
			DocumentModelUnitConverter unitConverter = modelShape.DocumentModel.UnitConverter;
			int geometrySpaceRight = unitConverter.ModelUnitsToEmuF(Math.Max(0, modelShape.Width));
			int geometrySpaceTop = 0;
			int geometrySpaceBottom = unitConverter.ModelUnitsToEmuF(Math.Max(0, modelShape.Height));
			foreach(IOfficeDrawingProperty drawingProperty in artProperties.Properties) {
				OfficeDrawingIntPropertyBase intProperty = drawingProperty as OfficeDrawingIntPropertyBase;
				if(intProperty == null)
					continue;
				if(drawingProperty is DrawingGeometryLeft)
					geometrySpaceLeft = intProperty.Value;
				else if(drawingProperty is DrawingGeometryRight)
					geometrySpaceRight = intProperty.Value;
				else if(drawingProperty is DrawingGeometryTop)
					geometrySpaceTop = intProperty.Value;
				else if(drawingProperty is DrawingGeometryBottom)
					geometrySpaceBottom = intProperty.Value;
			}
			return Rectangle.FromLTRB(geometrySpaceLeft, geometrySpaceTop, geometrySpaceRight, geometrySpaceBottom);
		}
		static void SetupShapeCustomGeometry(ModelShape modelShape, OfficeArtShapeContainer shape) {
			OfficeArtProperties artProperties = shape.ArtProperties;
			if(artProperties == null)
				return;
			Rectangle shapeGeometrySpace = GetShapeGeometrySpace(modelShape, shape);
			int geometrySpaceWidth = shapeGeometrySpace.Width;
			int geometrySpaceHeight = shapeGeometrySpace.Height;
			ModelShapePath path = new ModelShapePath(modelShape.DocumentModel);
			path.Width = geometrySpaceWidth;
			path.Height = geometrySpaceHeight;
			modelShape.ShapeProperties.CustomGeometry.Paths.Add(path);
			ShapePathType pathType = artProperties.ShapePathType;
			switch(pathType) {
				case ShapePathType.Lines:
					CreateLinesPath(artProperties, false, path);
					break;
				case ShapePathType.LinesClosed:
					CreateLinesPath(artProperties, true, path);
					break;
				case ShapePathType.Curves:
					CreateCurvesPath(artProperties, false, path);
					break;
				case ShapePathType.CurvesClosed:
					CreateCurvesPath(artProperties, true, path);
					break;
				case ShapePathType.Complex:
					CreateComplexPath(modelShape, artProperties, path);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		static void CreateComplexPath(ModelShape modelShape, OfficeArtProperties artProperties, ModelShapePath path) {
			DrawingGeometryVertices vertices = artProperties.GetGeometryVertices();
			if(vertices == null)
				return;
			DrawingGeometrySegmentInfo segmentsInfo = artProperties.GetGeometrySegmentInfo();
			if(segmentsInfo == null)
				return;
			Point[] points = vertices.GetElements();
			MsoPathInfo[] msoPathInfos = segmentsInfo.GetElements();
			CreateComplexPathCore(modelShape, msoPathInfos, points, path);
		}
		internal static void CreateComplexPathCore(ModelShape modelShape, MsoPathInfo[] msoPathInfos, Point[] points, ModelShapePath path) {
			int pointsIndex = 0;
			foreach(MsoPathInfo pathInfo in msoPathInfos) {
				switch(pathInfo.PathType) {
					case MsoPathType.LineTo:
						AddLineToSegments(path, pathInfo, ref pointsIndex, points);
						break;
					case MsoPathType.CurveTo:
						AddCurveToSegments(path, pathInfo, ref pointsIndex, points);
						break;
					case MsoPathType.MoveTo:
						if(pointsIndex < points.Length) {
							path.Instructions.Add(new PathMove(points[pointsIndex].X.ToString(), points[pointsIndex].Y.ToString()));
							pointsIndex++;
						}
						break;
					case MsoPathType.Close:
						path.Instructions.Add(new PathClose());
						break;
					case MsoPathType.End:
						path = GetPathCopy(modelShape, path);
						break;
					case MsoPathType.Escape:
					case MsoPathType.ClientEscape:
						ProcessEscapeSegments(modelShape, ref path, pathInfo, points, ref pointsIndex);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
		internal static void ProcessEscapeSegments(ModelShape modelShape, ref ModelShapePath path, MsoPathInfo pathInfo, Point[] points, ref int pointsIndex) {
			switch(pathInfo.PathEscape) {
				case MsoPathEscape.Extension:
					break;
				case MsoPathEscape.AngleEllipseTo:
					AddAngleEllipse(path, pathInfo, ref pointsIndex, points);
					break;
				case MsoPathEscape.AngleEllipse:
					path = GetPathCopy(modelShape, path);
					AddAngleEllipse(path, pathInfo, ref pointsIndex, points);
					break;
				case MsoPathEscape.ArcTo:
					AddArcTo(path, pathInfo, ref pointsIndex, points, false);
					break;
				case MsoPathEscape.Arc:
					break;
				case MsoPathEscape.ClockwiseArcTo:
					AddArcTo(path, pathInfo, ref pointsIndex, points, true);
					break;
				case MsoPathEscape.ClockwiseArc:
					break;
				case MsoPathEscape.EllipticalQuadrantX:
					break;
				case MsoPathEscape.EllipticalQuadrantY:
					break;
				case MsoPathEscape.QuadraticBezier:
					AddQuadraticBezier(path, pathInfo, ref pointsIndex, points);
					break;
				case MsoPathEscape.NoFill:
					path.FillMode = PathFillMode.None;
					break;
				case MsoPathEscape.NoLine:
					path.Stroke = false;
					break;
				case MsoPathEscape.AutoLine:
					break;
				case MsoPathEscape.AutoCurve:
					break;
				case MsoPathEscape.CornerLine:
					break;
				case MsoPathEscape.CornerCurve:
					break;
				case MsoPathEscape.SmoothLine:
					break;
				case MsoPathEscape.SmoothCurve:
					break;
				case MsoPathEscape.SymmetricLine:
					break;
				case MsoPathEscape.SymmetricCurve:
					break;
				case MsoPathEscape.Freeform:
					break;
				case MsoPathEscape.FillColor:
					break;
				case MsoPathEscape.LineColor:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		static void AddQuadraticBezier(ModelShapePath path, MsoPathInfo pathInfo, ref int pointsIndex, Point[] points) {
			for(int i = 0; i < pathInfo.Segments && pointsIndex + 1 < points.Length; i++) {
				Point first = points[pointsIndex++];
				Point second = points[pointsIndex++];
				path.Instructions.Add(new PathQuadraticBezier(path.DocumentModelPart, first.X.ToString(), first.Y.ToString(), second.X.ToString(), second.Y.ToString()));
			}
		}
		static void AddArcTo(ModelShapePath path, MsoPathInfo pathInfo, ref int pointsIndex, Point[] points, bool clockWise) {
			const int fullCircle = 360 * 60000;
			for(int i = 0; i < pathInfo.Segments / 4 && pointsIndex + 3 < points.Length; i++) {
				Point tlPoint = points[pointsIndex++];
				Point brPoint = points[pointsIndex++];
				int w = brPoint.X - tlPoint.X;
				int h = brPoint.Y - tlPoint.Y;
				Point v1 = points[pointsIndex++];
				Point v2 = points[pointsIndex++];
				if(w == 0 || h == 0)
					continue;
				int centerX = (tlPoint.X + w / 2);
				int centerY = (tlPoint.Y + h / 2);
				int startAngle = AngleByVector(v1.X - centerX, v1.Y - centerY);
				int swingAngle = AngleBetweenVectors(v1.X - centerX, v1.Y - centerY, v2.X - centerX, v2.Y - centerY);
				if(!clockWise) {
					swingAngle -= fullCircle;
					swingAngle %= fullCircle;
				}
				int wR = (Math.Abs(w / 2));
				int hR = (Math.Abs(h / 2));
				path.Instructions.Add(new PathArc(wR.ToString(), hR.ToString(), startAngle.ToString(), swingAngle.ToString()));
			}
		}
		static int AngleByVector(int x, int y) {
			double atan = Math.Atan2(y, x);
			double degrees = atan * 180 / Math.PI;
			if(degrees < 0)
				degrees += 360;
			return (int) (degrees * 60000);
		}
		static int AngleBetweenVectors(int x1, int y1, int x2, int y2) {
			double scalar = (double) x1 * x2 + (double) y1 * y2;
			double wedge = (double) x1 * y2 - (double) x2 * y1;
			double atan = Math.Atan(wedge / scalar);
			double degrees = atan * 180 / Math.PI;
			if(scalar == 0 && wedge > 0)
				degrees = 90;
			if(scalar == 0 && wedge < 0)
				degrees = -90;
			if(scalar < 0 && wedge >= 0)
				degrees += 180;
			if(scalar < 0 && wedge < 0)
				degrees += 180;
			return (int)(degrees * 60000);
		}
		static void AddAngleEllipse(ModelShapePath path, MsoPathInfo pathInfo, ref int pointsIndex, Point[] points) {
			for(int i = 0; i < pathInfo.Segments && pointsIndex + 2 < points.Length; i++) {
				Point center = points[pointsIndex];
				Point radiuses = points[pointsIndex + 1];
				Point angles = points[pointsIndex + 2];
				try {
					double x1 = radiuses.X * Math.Cos(angles.X / 180d * Math.PI);
					double y1 = radiuses.X * Math.Sin(angles.X / 180d * Math.PI);
					double x2 = radiuses.Y * Math.Cos(angles.Y / 180d * Math.PI);
					double y2 = radiuses.Y * Math.Sin(angles.Y / 180d * Math.PI);
					path.Instructions.Add(new PathMove((x1 + center.X).ToString(), (y1 + center.Y).ToString()));
					double hR2 = (x2 * x2 * y1 * y1 - y2 * y2 * x1 * x1) / (x2 * x2 - x1 * x1);
					double wR = Math.Sqrt((x1 * x1 * hR2) / (hR2 - y1 * y1));
					double hR = Math.Sqrt(hR2);
					path.Instructions.Add(new PathArc(wR.ToString(), hR.ToString(), angles.X.ToString(), (angles.X - angles.Y).ToString()));
				}
				catch {
				}
				pointsIndex += 3;
			}
		}
		static ModelShapePath GetPathCopy(ModelShape modelShape, ModelShapePath path) {
			ModelShapePath newPath = new ModelShapePath(modelShape.DocumentModel);
			newPath.Width = path.Width;
			newPath.Height = path.Height;
			modelShape.ShapeProperties.CustomGeometry.Paths.Add(newPath);
			return newPath;
		}
		static void AddCurveToSegments(ModelShapePath path, MsoPathInfo pathInfo, ref int pointsIndex, Point[] points) {
			for(int i = 0; i < pathInfo.Segments && pointsIndex + 2 < points.Length; i++) {
				path.Instructions.Add(new PathCubicBezier(path.DocumentModelPart,
					points[pointsIndex].X.ToString(), points[pointsIndex].Y.ToString(),
					points[pointsIndex + 1].X.ToString(), points[pointsIndex + 1].Y.ToString(),
					points[pointsIndex + 2].X.ToString(), points[pointsIndex + 2].Y.ToString()
					));
				pointsIndex += 3;
			}
		}
		static void AddLineToSegments(ModelShapePath path, MsoPathInfo pathInfo, ref int pointsIndex, Point[] points) {
			for(int i = 0; i < pathInfo.Segments && pointsIndex < points.Length; i++) {
				path.Instructions.Add(new PathLine(points[pointsIndex].X.ToString(), points[pointsIndex].Y.ToString()));
				pointsIndex++;
			}
		}
		static void CreateLinesPath(OfficeArtProperties artProperties, bool closed, ModelShapePath path) {
			DrawingGeometryVertices vertices = artProperties.GetGeometryVertices();
			if(vertices == null)
				return;
			Point[] points = vertices.GetElements();
			path.Instructions.Add(new PathMove(points[0].X.ToString(), points[0].Y.ToString()));
			for(int i = 1; i < points.Length; i++) {
				path.Instructions.Add(new PathLine(points[i].X.ToString(), points[i].Y.ToString()));
			}
			if(closed)
				path.Instructions.Add(new PathClose());
		}
		static void CreateCurvesPath(OfficeArtProperties artProperties, bool closed, ModelShapePath path) {
			DrawingGeometryVertices vertices = artProperties.GetGeometryVertices();
			if(vertices == null)
				return;
			Point[] points = vertices.GetElements();
			path.Instructions.Add(new PathMove(points[0].X.ToString(), points[0].Y.ToString()));
			for(int i = 1; i + 2 < points.Length; i += 3) {
				path.Instructions.Add(new PathCubicBezier(path.DocumentModelPart, points[i].X.ToString(), points[i].Y.ToString(), points[i + 1].X.ToString(), points[i + 1].Y.ToString(), points[i + 2].X.ToString(), points[i + 2].Y.ToString()));
			}
			if(closed)
				path.Instructions.Add(new PathClose());
		}
	}
	#endregion
}
