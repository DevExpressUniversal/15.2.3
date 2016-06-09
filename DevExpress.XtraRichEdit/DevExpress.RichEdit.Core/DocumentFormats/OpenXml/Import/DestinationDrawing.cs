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
using System.Globalization;
using System.Xml;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Export.OpenDocument;
using DevExpress.XtraRichEdit.Export.OpenXml;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Utils.Internal;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	#region DrawingDestination
	public class DrawingDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("inline", OnInline);
			result.Add("anchor", OnAnchor);
			return result;
		}
		readonly FloatingObjectImportInfo floatingObjectImportInfo;
		public DrawingDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
			this.floatingObjectImportInfo = new FloatingObjectImportInfo(importer.PieceTable);		   
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public OfficeImage Image { get { return floatingObjectImportInfo.Image; } set { floatingObjectImportInfo.Image = value; } }
		public TextBoxContentType TextBoxContent { get { return floatingObjectImportInfo.TextBoxContent; } set { floatingObjectImportInfo.TextBoxContent = value; } }
		public TextBoxProperties TextBoxProperties { get { return floatingObjectImportInfo.TextBoxProperties; } }
		public int Width { get { return floatingObjectImportInfo.Width; } set { floatingObjectImportInfo.Width = value; } }
		public int Height { get { return floatingObjectImportInfo.Height; } set { floatingObjectImportInfo.Height = value; } }
		public FloatingObjectProperties FloatingObject { get { return floatingObjectImportInfo.FloatingObjectProperties; } }
		public Shape Shape { get { return floatingObjectImportInfo.Shape; } }
		public FloatingObjectImportInfo FloatingObjectImportInfo { get { return floatingObjectImportInfo; } }
		static DrawingDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (DrawingDestination)importer.PeekDestination();
		}
		static Destination OnInline(WordProcessingMLBaseImporter importer, XmlReader reader) {
			GetThis(importer).floatingObjectImportInfo.IsFloatingObject = false;
			return new DrawingInlineDestination(importer, GetThis(importer).floatingObjectImportInfo);
		}
		static Destination OnAnchor(WordProcessingMLBaseImporter importer, XmlReader reader) {
			GetThis(importer).floatingObjectImportInfo.IsFloatingObject = true;
			return new DrawingAnchorDestination(importer, GetThis(importer).floatingObjectImportInfo);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (!floatingObjectImportInfo.IsFloatingObject) {
				if (Image == null)
					return;
				if (!ShouldInsertPicture()) {
					Importer.PieceTable.InsertTextCore(Importer.Position, " ");
					return;
				}
				Size originalSize = InternalOfficeImageHelper.CalculateImageSizeInModelUnits(Image, Importer.DocumentModel.UnitConverter);
				Size desiredSize = new Size(floatingObjectImportInfo.Width, floatingObjectImportInfo.Height);
				SizeF scale = ImageScaleCalculator.GetScale(desiredSize, originalSize, 100.0f);
				PieceTable.AppendImage(Importer.Position, Image, scale.Width, scale.Height);
			}
			else {
				if (Height != Int32.MinValue && Width != Int32.MinValue) {
					if (Image != null) {
						Size size = UnitConverter.TwipsToModelUnits(Image.SizeInTwips);
						int width = Width;
						if (width <= 0)
							width = Math.Max(1, size.Width);
						int height = Height;
						if (height <= 0)
							height = Math.Max(1, size.Height);
						FloatingObject.ActualSize = new Size(width, height);
					}
					else
						FloatingObject.ActualSize = new Size(Math.Max(1, Width), Math.Max(1, Height));
				}
				floatingObjectImportInfo.InsertFloatingObject(Importer.Position);
			}
		}
		internal bool ShouldInsertPicture() {
			return Importer.DocumentModel.DocumentCapabilities.InlinePicturesAllowed;
		}
	}
	#endregion
	#region DrawingInlineDestination
	public class DrawingInlineDestination :  ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		readonly FloatingObjectImportInfo FloatingObjectImportInfo;
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("extent", OnExtent);
			result.Add("graphic", OnGraphic);
			return result;
		}
		public DrawingInlineDestination(WordProcessingMLBaseImporter importer, FloatingObjectImportInfo FloatingObjectImportInfo)
			: base(importer) {
			Guard.ArgumentNotNull(FloatingObjectImportInfo, "FloatingObjectImportInfo");
			this.FloatingObjectImportInfo = FloatingObjectImportInfo;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static DrawingInlineDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (DrawingInlineDestination)importer.PeekDestination();
		}
		static Destination OnExtent(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingInlineExtentDestination(importer, GetThis(importer).FloatingObjectImportInfo);
		}
		static Destination OnGraphic(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingInlineGraphicDestination(importer, GetThis(importer).FloatingObjectImportInfo);
		}
	}
	#endregion
	#region DrawingAnchorDestination
	public class DrawingAnchorDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("extent", OnExtent);
			result.Add("graphic", OnGraphic);
			result.Add("simplePos", OnSimplePosition);
			result.Add("positionH", OnHorizontalPosition);
			result.Add("positionV", OnVerticalPosition);
			result.Add("wrapNone", OnWrapNone);
			result.Add("wrapSquare", OnWrapSquare);
			result.Add("wrapThrough", OnWrapThrough);
			result.Add("wrapTight", OnWrapTight);
			result.Add("wrapTopAndBottom", OnWrapTopAndBottom);
			result.Add("cNvGraphicFramePr", OnCNvGraphicFramePr);
			result.Add("docPr", OnAnchorDocumentProperties);
			result.Add("sizeRelH", OnAnchorHorizontalRelativeSize);
			result.Add("sizeRelV", OnAnchorVerticalRelativeSize);
			return result;
		}
		readonly FloatingObjectImportInfo floatingObjectImportInfo;
		bool useSimplePosition;
		public DrawingAnchorDestination(WordProcessingMLBaseImporter importer, FloatingObjectImportInfo floatingObjectImportInfo)
			: base(importer) {
			Guard.ArgumentNotNull(floatingObjectImportInfo, "floatingObjectImportInfo");
			this.floatingObjectImportInfo = floatingObjectImportInfo;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public bool UseSimplePosition { get { return useSimplePosition; } }
		public FloatingObjectProperties FloatingObject { get { return floatingObjectImportInfo.FloatingObjectProperties; } }
		public FloatingObjectImportInfo FloatingObjectImportInfo { get { return floatingObjectImportInfo; } }
		public override bool IsChoiceNamespaceSupported(string requeriesNamespaceUri) {
			if (String.Compare(requeriesNamespaceUri, OpenXmlExporter.Wp14Namespace, StringComparison.OrdinalIgnoreCase) == 0)
				return true;			
			return base.IsChoiceNamespaceSupported(requeriesNamespaceUri);
		}
		public override void ProcessElementOpen(XmlReader reader) {
			FloatingObjectProperties obj = FloatingObject;
			int value;
			value = Importer.GetIntegerValue(reader, "distT", int.MinValue);
			if (value != int.MinValue)
				obj.TopDistance = ConvertEmuToDocumentUnits(value);
			value = Importer.GetIntegerValue(reader, "distB", int.MinValue);
			if (value != int.MinValue)
				obj.BottomDistance = ConvertEmuToDocumentUnits(value);
			value = Importer.GetIntegerValue(reader, "distL", int.MinValue);
			if (value != int.MinValue)
				obj.LeftDistance = ConvertEmuToDocumentUnits(value);
			value = Importer.GetIntegerValue(reader, "distR", int.MinValue);
			if (value != int.MinValue)
				obj.RightDistance = ConvertEmuToDocumentUnits(value);
			value = Importer.GetIntegerValue(reader, "relativeHeight", int.MinValue);
			if (value != int.MinValue)
				obj.ZOrder = value;
			this.useSimplePosition = Importer.GetOnOffValue(reader, "simplePos", false);
			if (Importer.GetOnOffValue(reader, "allowOverlap", false))
				obj.AllowOverlap = true;
			if (Importer.GetOnOffValue(reader, "behindDoc", false))
				obj.IsBehindDoc = true;
			if (Importer.GetOnOffValue(reader, "hidden", false))
				obj.Hidden = true;
			if (Importer.GetOnOffValue(reader, "layoutInCell", false))
				obj.LayoutInTableCell = true;
			if (Importer.GetOnOffValue(reader, "locked", false))
				obj.Locked = true;
		}
		public int ConvertEmuToDocumentUnits(int value) {
			return (int)Math.Round(UnitConverter.MillimetersToModelUnitsF(value / 36000.0f));
		}
		static DrawingAnchorDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (DrawingAnchorDestination)importer.PeekDestination();
		}
		static Destination OnExtent(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingInlineExtentDestination(importer, GetThis(importer).floatingObjectImportInfo);
		}
		static Destination OnGraphic(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingInlineGraphicDestination(importer, GetThis(importer).floatingObjectImportInfo);
		}
		static Destination OnSimplePosition(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingAnchorSimplePositionDestination(importer, GetThis(importer));
		}
		static Destination OnAnchorHorizontalRelativeSize(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingAnchorHorizontalRelativeSizeDestination(importer, GetThis(importer));
		}
		static Destination OnAnchorVerticalRelativeSize(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingAnchorVerticalRelativeSizeDestination(importer, GetThis(importer));
		}
		static Destination OnHorizontalPosition(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingAnchorHorizontalPositionDestination(importer, GetThis(importer));
		}
		static Destination OnVerticalPosition(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingAnchorVerticalPositionDestination(importer, GetThis(importer));
		}
		static Destination OnWrapNone(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingAnchorWrapNoneDestination(importer, GetThis(importer));
		}
		static Destination OnWrapSquare(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingAnchorWrapSquareDestination(importer, GetThis(importer));
		}
		static Destination OnWrapThrough(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingAnchorWrapThroughDestination(importer, GetThis(importer));
		}
		static Destination OnWrapTight(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingAnchorWrapTightDestination(importer, GetThis(importer));
		}
		static Destination OnWrapTopAndBottom(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingAnchorWrapTopAndBottomDestination(importer, GetThis(importer));
		}
		static Destination OnCNvGraphicFramePr(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingAnchorGraphicFramePropertyDestination(importer, GetThis(importer));
		}
		static Destination OnAnchorDocumentProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingAnchorDocumentPropertiesDestination(importer, GetThis(importer));
		}
	}
	#endregion
	#region DrawingAnchorGraphicFramePropertyDestination
	public class DrawingAnchorGraphicFramePropertyDestination : ElementDestination {
		readonly FloatingObjectProperties floatingObject;
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("graphicFrameLocks", OnGraphicFrameLocks);
			return result;
		}
		public DrawingAnchorGraphicFramePropertyDestination(WordProcessingMLBaseImporter importer, DrawingAnchorDestination anchorDestination)
			:base(importer){
			Guard.ArgumentNotNull(anchorDestination, "cNvGraphicFramePr");
			this.floatingObject = anchorDestination.FloatingObject;
		}
		public FloatingObjectProperties FloatingObject { get { return floatingObject; } }
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static DrawingAnchorGraphicFramePropertyDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (DrawingAnchorGraphicFramePropertyDestination)importer.PeekDestination();
		}
		static Destination OnGraphicFrameLocks(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingGraphicFrameLocksDestination(importer, GetThis(importer));
		}
	}
	#endregion
	#region DrawingGraphicFrameLocksDestination
	public class DrawingGraphicFrameLocksDestination : LeafElementDestination {
		readonly FloatingObjectProperties floatingObject;
		public DrawingGraphicFrameLocksDestination(WordProcessingMLBaseImporter importer, DrawingAnchorGraphicFramePropertyDestination anchorDestination)
			: base(importer) {
			Guard.ArgumentNotNull(anchorDestination, "graphicFrameLocks");
			this.floatingObject = anchorDestination.FloatingObject;
		}
		public FloatingObjectProperties FloatingObject { get { return floatingObject; } }
		public override void ProcessElementOpen(XmlReader reader) {
			FloatingObjectProperties obj = FloatingObject;
			string value = reader.GetAttribute("noChangeAspect", null);
			if (!String.IsNullOrEmpty(value))
				obj.LockAspectRatio = Importer.ConvertToBool(value);
		}
	}
	#endregion
	#region DrawingInlineExtentDestination
	public class DrawingInlineExtentDestination : LeafElementDestination {
		readonly FloatingObjectImportInfo FloatingObjectImportInfo;
		public DrawingInlineExtentDestination(WordProcessingMLBaseImporter importer, FloatingObjectImportInfo FloatingObjectImportInfo)
			: base(importer) {
			Guard.ArgumentNotNull(FloatingObjectImportInfo, "FloatingObjectImportInfo");
			this.FloatingObjectImportInfo = FloatingObjectImportInfo;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string cx = reader.GetAttribute("cx", null);
			string cy = reader.GetAttribute("cy", null);
			FloatingObjectImportInfo.Width = (int)Math.Round(UnitConverter.MillimetersToModelUnitsF(Importer.GetIntegerValue(cx, NumberStyles.Integer, 0) / 36000.0f));
			FloatingObjectImportInfo.Height = (int)Math.Round(UnitConverter.MillimetersToModelUnitsF(Importer.GetIntegerValue(cy, NumberStyles.Integer, 0) / 36000.0f));
		}
	}
	#endregion
	#region DrawingAnchorDocumentPropertiesDestination
	public class DrawingAnchorDocumentPropertiesDestination : LeafElementDestination {
		readonly FloatingObjectImportInfo floatingObjectImportInfo;
		public DrawingAnchorDocumentPropertiesDestination(WordProcessingMLBaseImporter importer, DrawingAnchorDestination anchorDestination)
			: base(importer) {
			Guard.ArgumentNotNull(anchorDestination, "anchorDestination");
			this.floatingObjectImportInfo = anchorDestination.FloatingObjectImportInfo;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string name = reader.GetAttribute("name");
			if (!String.IsNullOrEmpty(name))
				floatingObjectImportInfo.Name = name;
		}
	}
	#endregion
	#region DrawingInlineGraphicDestination
	public class DrawingInlineGraphicDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		readonly FloatingObjectImportInfo FloatingObjectImportInfo;
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("graphicData", OnGraphicData);
			return result;
		}
		public DrawingInlineGraphicDestination(WordProcessingMLBaseImporter importer, FloatingObjectImportInfo FloatingObjectImportInfo)
			: base(importer) {
			Guard.ArgumentNotNull(FloatingObjectImportInfo, "FloatingObjectImportInfo");
			this.FloatingObjectImportInfo = FloatingObjectImportInfo;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static DrawingInlineGraphicDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (DrawingInlineGraphicDestination)importer.PeekDestination();
		}
		static Destination OnGraphicData(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingInlineGraphicDataDestination(importer, GetThis(importer).FloatingObjectImportInfo);
		}
	}
	#endregion
	#region DrawingInlineGraphicDataDestination
	public class DrawingInlineGraphicDataDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		readonly FloatingObjectImportInfo FloatingObjectImportInfo;
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("pic", OnPicture);
			result.Add("wsp", OnWordProcessingShape);
			return result;
		}
		public DrawingInlineGraphicDataDestination(WordProcessingMLBaseImporter importer, FloatingObjectImportInfo FloatingObjectImportInfo)
			: base(importer) {
			Guard.ArgumentNotNull(FloatingObjectImportInfo, "FloatingObjectImportInfo");
			this.FloatingObjectImportInfo = FloatingObjectImportInfo;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static DrawingInlineGraphicDataDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (DrawingInlineGraphicDataDestination)importer.PeekDestination();
		}
		static Destination OnPicture(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingInlineGraphicDataPictureDestination(importer, GetThis(importer).FloatingObjectImportInfo);
		}
		static Destination OnWordProcessingShape(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new WordProcessingShapeDestination(importer, GetThis(importer).FloatingObjectImportInfo);
		}
	}
	#endregion
	#region DrawingInlineGraphicDataPictureDestination
	public class DrawingInlineGraphicDataPictureDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		readonly FloatingObjectImportInfo FloatingObjectImportInfo;
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("blipFill", OnBlipFill);
			result.Add("AlternateContent", OnAlternateContent);
			result.Add("Choice", OnChoice);
			result.Add("Fallback", OnFallback);
			result.Add("spPr", OnShapeProperties);
			return result;
		}
		public DrawingInlineGraphicDataPictureDestination(WordProcessingMLBaseImporter importer, FloatingObjectImportInfo FloatingObjectImportInfo)
			: base(importer) {
			Guard.ArgumentNotNull(FloatingObjectImportInfo, "FloatingObjectImportInfo");
			this.FloatingObjectImportInfo = FloatingObjectImportInfo;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static DrawingInlineGraphicDataPictureDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (DrawingInlineGraphicDataPictureDestination)importer.PeekDestination();
		}
		static Destination OnBlipFill(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new PictureBlipFillDestination(importer, GetThis(importer).FloatingObjectImportInfo);
		}
		static Destination OnShapeProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new WordProcessingShapePropertiesDestination(importer, GetThis(importer).FloatingObjectImportInfo.Shape);
		}
		protected static Destination OnAlternateContent(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return GetThis(importer);
		}
		protected static Destination OnChoice(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return GetThis(importer);
		}
		protected static Destination OnFallback(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return GetThis(importer);
		}
	}
	#endregion
	#region PictureBlipFillDestination
	public class PictureBlipFillDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		readonly FloatingObjectImportInfo FloatingObjectImportInfo;
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("blip", OnBlip);
			return result;
		}
		public PictureBlipFillDestination(WordProcessingMLBaseImporter importer, FloatingObjectImportInfo FloatingObjectImportInfo)
			: base(importer) {
			Guard.ArgumentNotNull(FloatingObjectImportInfo, "FloatingObjectImportInfo");
			this.FloatingObjectImportInfo = FloatingObjectImportInfo;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static PictureBlipFillDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (PictureBlipFillDestination)importer.PeekDestination();
		}
		static Destination OnBlip(WordProcessingMLBaseImporter importer, XmlReader reader) {
			OpenXmlImporter openXmlImporter = importer as OpenXmlImporter;
			if (openXmlImporter == null)
				return null;
			return new PictureBlipDestination(openXmlImporter, GetThis(openXmlImporter).FloatingObjectImportInfo);
		}
	}
	#endregion
	#region PictureBlipDestination
	public class PictureBlipDestination : LeafElementDestination {
		readonly FloatingObjectImportInfo FloatingObjectImportInfo;
		public PictureBlipDestination(OpenXmlImporter importer, FloatingObjectImportInfo FloatingObjectImportInfo)
			: base(importer) {
			Guard.ArgumentNotNull(FloatingObjectImportInfo, "FloatingObjectImportInfo");
			this.FloatingObjectImportInfo = FloatingObjectImportInfo;
		}
		public new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		public override void ProcessElementOpen(XmlReader reader) {
			OfficeImage image;
			string id = reader.GetAttribute("embed", OpenXmlExporter.RelsNamespace);
			if (!String.IsNullOrEmpty(id)) {
				image = Importer.LookupImageByRelationId(id, Importer.DocumentRootFolder);
				if (image != null)
					FloatingObjectImportInfo.Image = image;
				return;
			}
			id = reader.GetAttribute("link", OpenXmlExporter.RelsNamespace);
			if (String.IsNullOrEmpty(id))
				return;
			image = (OfficeImage)Importer.LookupExternalImageByRelationId(id, Importer.DocumentRootFolder);
			if (image != null)
				FloatingObjectImportInfo.Image = image;
		}
	}
	#endregion
	#region DrawingAnchorSimplePositionDestination
	public class DrawingAnchorSimplePositionDestination : LeafElementDestination {
		readonly DrawingAnchorDestination anchorDestination;
		public DrawingAnchorSimplePositionDestination(WordProcessingMLBaseImporter importer, DrawingAnchorDestination anchorDestination)
			: base(importer) {
			Guard.ArgumentNotNull(anchorDestination, "anchorDestination");
			this.anchorDestination = anchorDestination;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			if (!anchorDestination.UseSimplePosition)
				return;
			int x = Importer.GetIntegerValue(reader, "x", int.MinValue);
			int y = Importer.GetIntegerValue(reader, "y", int.MinValue);
			if (x != int.MinValue && y != int.MinValue) {
				x = anchorDestination.ConvertEmuToDocumentUnits(x);
				y = anchorDestination.ConvertEmuToDocumentUnits(y);
				anchorDestination.FloatingObject.Offset = new Point(x, y);
				anchorDestination.FloatingObject.HorizontalPositionType = FloatingObjectHorizontalPositionType.Page;
				anchorDestination.FloatingObject.VerticalPositionType = FloatingObjectVerticalPositionType.Page;
				anchorDestination.FloatingObject.HorizontalPositionAlignment = FloatingObjectHorizontalPositionAlignment.Left;
				anchorDestination.FloatingObject.VerticalPositionAlignment = FloatingObjectVerticalPositionAlignment.Top;
			}
		}
	}
	#endregion
	#region DrawingAnchorRelativeSizeBaseDestination (abstract class)
	public abstract class DrawingAnchorRelativeSizeBaseDestination : ElementDestination {
		readonly DrawingAnchorDestination anchorDestination;
		int val = 100 * 1000;
		protected DrawingAnchorRelativeSizeBaseDestination(WordProcessingMLBaseImporter importer, DrawingAnchorDestination anchorDestination)
			: base(importer) {
			Guard.ArgumentNotNull(anchorDestination, "anchorDestination");
			this.anchorDestination = anchorDestination;
		}
		public int Value { get { return val; } set { val = value; } }		
		public DrawingAnchorDestination AnchorDestination { get { return anchorDestination; } }
		static DrawingAnchorRelativeSizeBaseDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (DrawingAnchorRelativeSizeBaseDestination)importer.PeekDestination();
		}
		public override void ProcessElementClose(XmlReader reader) {
			ProcessElementCloseCore(reader);
		}		
		protected internal abstract void ProcessElementCloseCore(XmlReader reader);
	}
	#endregion
	#region DrawingAnchorHorizontalRelativeSizeDestination
	public class DrawingAnchorHorizontalRelativeSizeDestination : DrawingAnchorRelativeSizeBaseDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("pctWidth", OnPictureWidth);
			return result;
		}
		public FloatingObjectRelativeFromHorizontal RelativeFrom { get; private set; }
		public DrawingAnchorHorizontalRelativeSizeDestination(WordProcessingMLBaseImporter importer, DrawingAnchorDestination anchorDestination)
			: base(importer, anchorDestination) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static DrawingAnchorHorizontalRelativeSizeDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (DrawingAnchorHorizontalRelativeSizeDestination)importer.PeekDestination();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			RelativeFrom = Importer.GetEnumValue(reader, "relativeFrom", OpenXmlExporter.floatingObjectRelativeFromHorizontalTable, FloatingObjectRelativeFromHorizontal.Page);
		}
		protected internal override void ProcessElementCloseCore(XmlReader reader) {
			FloatingObjectProperties obj = AnchorDestination.FloatingObject;
			obj.RelativeWidth = new FloatingObjectRelativeWidth(RelativeFrom, Value);
		}
		static Destination OnPictureWidth(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingAnchorRelativeSizeValueDestination(importer, GetThis(importer));
		}
	}
	#endregion
	#region DrawingAnchorHorizontalRelativeSizeDestination
	public class DrawingAnchorVerticalRelativeSizeDestination : DrawingAnchorRelativeSizeBaseDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("pctHeight", OnPictureHeight);
			return result;
		}
		public FloatingObjectRelativeFromVertical RelativeFrom { get; private set; }
		public DrawingAnchorVerticalRelativeSizeDestination(WordProcessingMLBaseImporter importer, DrawingAnchorDestination anchorDestination)
			: base(importer, anchorDestination) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static DrawingAnchorVerticalRelativeSizeDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (DrawingAnchorVerticalRelativeSizeDestination)importer.PeekDestination();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			RelativeFrom = Importer.GetEnumValue(reader, "relativeFrom", OpenXmlExporter.floatingObjectRelativeFromVerticalTable, FloatingObjectRelativeFromVertical.Page);
		}
		protected internal override void ProcessElementCloseCore(XmlReader reader) {
			FloatingObjectProperties obj = AnchorDestination.FloatingObject;
			obj.RelativeHeight = new FloatingObjectRelativeHeight(RelativeFrom, Value);
		}
		static Destination OnPictureHeight(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingAnchorRelativeSizeValueDestination(importer, GetThis(importer));
		}
	}
	#endregion
	#region DrawingAnchorPositionHorizontalAlignmentDestination
	public class DrawingAnchorRelativeSizeValueDestination : LeafElementDestination {
		readonly DrawingAnchorRelativeSizeBaseDestination sizeDestination;
		public DrawingAnchorRelativeSizeValueDestination(WordProcessingMLBaseImporter importer, DrawingAnchorRelativeSizeBaseDestination positionDestination)
			: base(importer) {
			Guard.ArgumentNotNull(positionDestination, "positionDestination");
			this.sizeDestination = positionDestination;
		}
		public override bool ProcessText(XmlReader reader) {
			string text = reader.Value;
			if (!String.IsNullOrEmpty(text))
				sizeDestination.Value = Importer.GetIntegerValue(text, NumberStyles.Integer, 100 * 1000);
			return true;
		}
	}
	#endregion    
	#region DrawingAnchorPositionBaseDestination (abstract class)
	public abstract class DrawingAnchorPositionBaseDestination : ElementDestination {
		readonly DrawingAnchorDestination anchorDestination;
		int offset = Int32.MinValue;
		int percentOffset = Int32.MinValue;
		protected DrawingAnchorPositionBaseDestination(WordProcessingMLBaseImporter importer, DrawingAnchorDestination anchorDestination)
			: base(importer) {
			Guard.ArgumentNotNull(anchorDestination, "anchorDestination");
			this.anchorDestination = anchorDestination;
		}
		public int Offset { get { return offset; } set { offset = value; } }
		public int PercentOffset { get { return percentOffset; } set { percentOffset = value; } }
		public DrawingAnchorDestination AnchorDestination { get { return anchorDestination; } }
		static DrawingAnchorPositionBaseDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (DrawingAnchorPositionBaseDestination)importer.PeekDestination();
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (anchorDestination.UseSimplePosition)
				return;
			ProcessElementCloseCore(reader);
		}
		protected static Destination OnPositionOffset(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingAnchorPositionOffsetDestination(importer, GetThis(importer));
		}
		protected static Destination OnPositionPercentOffset(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingAnchorPositionPercentOffsetDestination(importer, GetThis(importer));
		}
		protected internal abstract void ProcessElementCloseCore(XmlReader reader);
	}
	#endregion
	#region DrawingAnchorHorizontalPositionDestination
	public class DrawingAnchorHorizontalPositionDestination : DrawingAnchorPositionBaseDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("posOffset", OnPositionOffset);
			result.Add("pctPosHOffset", OnPositionPercentOffset);
			result.Add("align", OnHorizontalAlignment);
			return result;
		}
		FloatingObjectHorizontalPositionType relativeTo;
		FloatingObjectHorizontalPositionAlignment alignment;
		public DrawingAnchorHorizontalPositionDestination(WordProcessingMLBaseImporter importer, DrawingAnchorDestination anchorDestination)
			: base(importer, anchorDestination) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public FloatingObjectHorizontalPositionAlignment Alignment { get { return alignment; } set { alignment = value; } }
		static DrawingAnchorHorizontalPositionDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (DrawingAnchorHorizontalPositionDestination)importer.PeekDestination();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			relativeTo = Importer.GetEnumValue(reader, "relativeFrom", OpenXmlExporter.floatingObjectHorizontalPositionTypeTable, FloatingObjectHorizontalPositionType.Page);
		}
		protected internal override void ProcessElementCloseCore(XmlReader reader) {
			FloatingObjectProperties obj = AnchorDestination.FloatingObject;
			Point point = obj.Offset;
			point.X = AnchorDestination.ConvertEmuToDocumentUnits(Offset);
			if(Offset != Int32.MinValue)
				obj.Offset = point;
			Point percentOffset = obj.PercentOffset;
			percentOffset.X = PercentOffset;
			if (PercentOffset != Int32.MinValue)
				obj.PercentOffset = percentOffset;
			if (obj.HorizontalPositionAlignment != this.Alignment)
				obj.HorizontalPositionAlignment = this.Alignment;
			if (obj.HorizontalPositionType != relativeTo)
				obj.HorizontalPositionType = relativeTo;
		}
		static Destination OnHorizontalAlignment(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingAnchorPositionHorizontalAlignmentDestination(importer, GetThis(importer));
		}
	}
	#endregion
	#region DrawingAnchorVerticalPositionDestination
	public class DrawingAnchorVerticalPositionDestination : DrawingAnchorPositionBaseDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("posOffset", OnPositionOffset);
			result.Add("pctPosVOffset", OnPositionPercentOffset);
			result.Add("align", OnVerticalAlignment);
			return result;
		}
		FloatingObjectVerticalPositionType relativeTo;
		FloatingObjectVerticalPositionAlignment alignment;
		public DrawingAnchorVerticalPositionDestination(WordProcessingMLBaseImporter importer, DrawingAnchorDestination anchorDestination)
			: base(importer, anchorDestination) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public FloatingObjectVerticalPositionAlignment Alignment { get { return alignment; } set { alignment = value; } }
		static DrawingAnchorVerticalPositionDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (DrawingAnchorVerticalPositionDestination)importer.PeekDestination();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			relativeTo = Importer.GetEnumValue(reader, "relativeFrom", OpenXmlExporter.floatingObjectVerticalPositionTypeTable, FloatingObjectVerticalPositionType.Page);
		}
		protected internal override void ProcessElementCloseCore(XmlReader reader) {
			FloatingObjectProperties obj = AnchorDestination.FloatingObject;
			Point point = obj.Offset;
			point.Y = AnchorDestination.ConvertEmuToDocumentUnits(Offset);
			if(Offset != Int32.MinValue)
				obj.Offset = point;
			Point percentOffset = obj.PercentOffset;
			percentOffset.Y = PercentOffset;
			if (PercentOffset != Int32.MinValue)
				obj.PercentOffset = percentOffset;
			if (obj.VerticalPositionAlignment != this.Alignment)
				obj.VerticalPositionAlignment = this.Alignment;
			if (obj.VerticalPositionType != relativeTo)
				obj.VerticalPositionType = relativeTo;
		}
		static Destination OnVerticalAlignment(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DrawingAnchorPositionVerticalAlignmentDestination(importer, GetThis(importer));
		}
	}
	#endregion
	#region DrawingAnchorPositionOffsetDestination
	public class DrawingAnchorPositionOffsetDestination : LeafElementDestination {
		readonly DrawingAnchorPositionBaseDestination positionDestination;
		public DrawingAnchorPositionOffsetDestination(WordProcessingMLBaseImporter importer, DrawingAnchorPositionBaseDestination positionDestination)
			: base(importer) {
			Guard.ArgumentNotNull(positionDestination, "positionDestination");
			this.positionDestination = positionDestination;
		}
		public override bool ProcessText(XmlReader reader) {
			string text = reader.Value;
			positionDestination.Offset = Importer.GetIntegerValue(text, NumberStyles.Number, 0);
			return true;
		}
	}
	#endregion
	#region DrawingAnchorPositionPercentOffsetDestination
	public class DrawingAnchorPositionPercentOffsetDestination : LeafElementDestination {
		readonly DrawingAnchorPositionBaseDestination positionDestination;
		public DrawingAnchorPositionPercentOffsetDestination(WordProcessingMLBaseImporter importer, DrawingAnchorPositionBaseDestination positionDestination)
			: base(importer) {
			Guard.ArgumentNotNull(positionDestination, "positionDestination");
			this.positionDestination = positionDestination;
		}
		public override bool ProcessText(XmlReader reader) {
			string text = reader.Value;
			if (!String.IsNullOrEmpty(text))
				positionDestination.PercentOffset = Importer.GetIntegerValue(text, NumberStyles.Integer, 0);
			return true;
		}
	}
	#endregion
	#region DrawingAnchorPositionHorizontalAlignmentDestination
	public class DrawingAnchorPositionHorizontalAlignmentDestination : LeafElementDestination {
		readonly DrawingAnchorHorizontalPositionDestination positionDestination;
		public DrawingAnchorPositionHorizontalAlignmentDestination(WordProcessingMLBaseImporter importer, DrawingAnchorHorizontalPositionDestination positionDestination)
			: base(importer) {
			Guard.ArgumentNotNull(positionDestination, "positionDestination");
			this.positionDestination = positionDestination;
		}
		public override bool ProcessText(XmlReader reader) {
			string text = reader.Value;
			if (!String.IsNullOrEmpty(text))
				positionDestination.Alignment = Importer.GetWpEnumValueCore(text, OpenXmlExporter.floatingObjectHorizontalPositionAlignmentTable, FloatingObjectHorizontalPositionAlignment.Left);
			return true;
		}
	}
	#endregion
	#region DrawingAnchorPositionVerticalAlignmentDestination
	public class DrawingAnchorPositionVerticalAlignmentDestination : LeafElementDestination {
		readonly DrawingAnchorVerticalPositionDestination positionDestination;
		public DrawingAnchorPositionVerticalAlignmentDestination(WordProcessingMLBaseImporter importer, DrawingAnchorVerticalPositionDestination positionDestination)
			: base(importer) {
			Guard.ArgumentNotNull(positionDestination, "positionDestination");
			this.positionDestination = positionDestination;
		}
		public override bool ProcessText(XmlReader reader) {
			string text = reader.Value;
			if (!String.IsNullOrEmpty(text))
				positionDestination.Alignment = Importer.GetWpEnumValueCore(text, OpenXmlExporter.floatingObjectVerticalPositionAlignmentTable, FloatingObjectVerticalPositionAlignment.Top);
			return true;
		}
	}
	#endregion
	#region DrawingAnchorWrapNoneDestination
	public class DrawingAnchorWrapNoneDestination : LeafElementDestination {
		readonly DrawingAnchorDestination anchorDestination;
		public DrawingAnchorWrapNoneDestination(WordProcessingMLBaseImporter importer, DrawingAnchorDestination anchorDestination)
			: base(importer) {
			Guard.ArgumentNotNull(anchorDestination, "anchorDestination");
			this.anchorDestination = anchorDestination;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			anchorDestination.FloatingObject.TextWrapType = FloatingObjectTextWrapType.None;
		}
	}
	#endregion
	#region DrawingAnchorWrapTopAndBottomDestination
	public class DrawingAnchorWrapTopAndBottomDestination : LeafElementDestination {
		readonly DrawingAnchorDestination anchorDestination;
		public DrawingAnchorWrapTopAndBottomDestination(WordProcessingMLBaseImporter importer, DrawingAnchorDestination anchorDestination)
			: base(importer) {
			Guard.ArgumentNotNull(anchorDestination, "anchorDestination");
			this.anchorDestination = anchorDestination;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			anchorDestination.FloatingObject.TextWrapType = FloatingObjectTextWrapType.TopAndBottom;
		}
	}
	#endregion
	#region DrawingAnchorPolygonDestinationBase (abstract)
	public abstract class DrawingAnchorPolygonDestinationBase : LeafElementDestination {
		readonly DrawingAnchorDestination anchorDestination;
		protected DrawingAnchorPolygonDestinationBase(WordProcessingMLBaseImporter importer, DrawingAnchorDestination anchorDestination)
			: base(importer) {
			Guard.ArgumentNotNull(anchorDestination, "anchorDestination");
			this.anchorDestination = anchorDestination;
		}
		protected DrawingAnchorDestination AnchorDestination { get { return anchorDestination; } }
		public override void ProcessElementOpen(XmlReader reader) {
			if (!String.IsNullOrEmpty(reader.GetAttribute("wrapText", null)))
				AnchorDestination.FloatingObject.TextWrapSide = Importer.GetEnumValue(reader, "wrapText", OpenXmlExporter.floatingObjectTextWrapSideTable, FloatingObjectTextWrapSide.Both);
		}
	}
	#endregion
	#region DrawingAnchorWrapSquareDestination
	public class DrawingAnchorWrapSquareDestination : DrawingAnchorPolygonDestinationBase {
		public DrawingAnchorWrapSquareDestination(WordProcessingMLBaseImporter importer, DrawingAnchorDestination anchorDestination)
			: base(importer, anchorDestination) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			AnchorDestination.FloatingObject.TextWrapType = FloatingObjectTextWrapType.Square;
		}
	}
	#endregion
	#region DrawingAnchorWrapThroughDestination
	public class DrawingAnchorWrapThroughDestination : DrawingAnchorPolygonDestinationBase {
		public DrawingAnchorWrapThroughDestination(WordProcessingMLBaseImporter importer, DrawingAnchorDestination anchorDestination)
			: base(importer, anchorDestination) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			AnchorDestination.FloatingObject.TextWrapType = FloatingObjectTextWrapType.Through;
		}
	}
	#endregion
	#region DrawingAnchorWrapTightDestination
	public class DrawingAnchorWrapTightDestination : DrawingAnchorPolygonDestinationBase {
		public DrawingAnchorWrapTightDestination(WordProcessingMLBaseImporter importer, DrawingAnchorDestination anchorDestination)
			: base(importer, anchorDestination) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			AnchorDestination.FloatingObject.TextWrapType = FloatingObjectTextWrapType.Tight;
		}
	}
	#endregion
	#region WordProcessingShapeDestination
	public class WordProcessingShapeDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		readonly FloatingObjectImportInfo FloatingObjectImportInfo;
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("txbx", OnTextBox);
			result.Add("bodyPr", OnTextBoxProperties);
			result.Add("spPr", OnShapeProperties);
			return result;
		}
		public WordProcessingShapeDestination(WordProcessingMLBaseImporter importer, FloatingObjectImportInfo FloatingObjectImportInfo)
			: base(importer) {
			Guard.ArgumentNotNull(FloatingObjectImportInfo, "FloatingObjectImportInfo");
			this.FloatingObjectImportInfo = FloatingObjectImportInfo;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static WordProcessingShapeDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (WordProcessingShapeDestination)importer.PeekDestination();
		}
		static Destination OnTextBox(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TextBoxDestination(importer, GetThis(importer).FloatingObjectImportInfo);
		}
		static Destination OnTextBoxProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TextBoxPropertiesDestination(importer, GetThis(importer).FloatingObjectImportInfo.TextBoxProperties);
		}
		static Destination OnShapeProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new WordProcessingShapePropertiesDestination(importer, GetThis(importer).FloatingObjectImportInfo.Shape);
		}
	}
	#endregion
	#region IColorAccessor
	public interface IColorAccessor {
		Color Color { get; set; }
	}
	#endregion
	#region ISolidFillAccessor
	public interface ISolidFillAccessor : IColorAccessor {
		Color FillColor { get; set; }
	}
	#endregion
	#region IOutlineAccessor
	public interface IOutlineAccessor : ISolidFillAccessor, IColorAccessor {
		Color OutlineColor { get; set; }
		int OutlineWidth { get; set; }
	}
	#endregion
	#region ISystemColorAccessor
	public interface ISystemColorAccessor: IColorAccessor {
		Color LastColor { get; set; }
	}
	#endregion
	#region WordProcessingShapePropertiesDestination
	public class WordProcessingShapePropertiesDestination : ElementDestination {
		#region ShapeSolidFillAccessor
		public class ShapeSolidFillAccessor : ISolidFillAccessor {
			readonly Shape shape;
			public ShapeSolidFillAccessor(Shape shape) {
				this.shape = shape;
			}
			Color ISolidFillAccessor.FillColor { get { return shape.FillColor; } set { shape.FillColor = value; } }
			Color IColorAccessor.Color { get { return shape.FillColor; } set { shape.FillColor = value; } }
		}
		#endregion
		#region ShapeOutlineAccessor
		public class ShapeOutlineAccessor : IOutlineAccessor {
			readonly Shape shape;
			public ShapeOutlineAccessor(Shape shape) {
				this.shape = shape;
			}
			#region IOutlineAccessor Members
			public Color OutlineColor { get { return shape.OutlineColor; } set { shape.OutlineColor = value; } }
			public int OutlineWidth { get { return shape.OutlineWidth; } set { shape.OutlineWidth = value; } }
			#endregion
			#region ISolidFillAccessor Members
			public Color FillColor { get { return shape.OutlineColor; } set { shape.OutlineColor = value; } }
			#endregion
			#region IColorAccessor Members
			public Color Color { get { return shape.OutlineColor; } set { shape.OutlineColor = value; } }
			#endregion
		}
		#endregion
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("solidFill", OnSolidFill);
			result.Add("noFill", OnNoFill);
			result.Add("ln", OnOutline);
			result.Add("xfrm", OnGraphicFrame);
			return result;
		}
		readonly Shape Shape;
		public WordProcessingShapePropertiesDestination(WordProcessingMLBaseImporter importer, Shape shape)
			: base(importer) {
				Guard.ArgumentNotNull(shape, "shape");
			this.Shape = shape;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static WordProcessingShapePropertiesDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (WordProcessingShapePropertiesDestination)importer.PeekDestination();
		}
		static Destination OnSolidFill(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SolidFillDestination(importer, new ShapeSolidFillAccessor(GetThis(importer).Shape));
		}
		static Destination OnNoFill(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new NoFillDestination(importer, new ShapeSolidFillAccessor(GetThis(importer).Shape));
		}
		static Destination OnOutline(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new OutlineDestination(importer, new ShapeOutlineAccessor(GetThis(importer).Shape));
		}
		static Destination OnGraphicFrame(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new GraphicFrameDestination(importer, GetThis(importer).Shape);
		}
	}
	#endregion
	#region SolidFillDestination
	public class SolidFillDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("srgbClr", OnSRgbColor);
			result.Add("schemeClr", OnSchemeColor);
			result.Add("prstClr", OnPresetColor);
			return result;
		}		
		readonly ISolidFillAccessor solidFillAccessor;
		public SolidFillDestination(WordProcessingMLBaseImporter importer, ISolidFillAccessor solidFillAccessor)
			: base(importer) {
			Guard.ArgumentNotNull(solidFillAccessor, "solidFillAccessor");
			this.solidFillAccessor = solidFillAccessor;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static SolidFillDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (SolidFillDestination)importer.PeekDestination();
		}
		static Destination OnSRgbColor(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SRgbColorDestination(importer, GetThis(importer).solidFillAccessor);
		}
		private static Destination OnPresetColor(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new PresetColorDestination(importer, GetThis(importer).solidFillAccessor);
		}
		static Destination OnSchemeColor(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SchemeColorDestination(importer, GetThis(importer).solidFillAccessor);
		}
	}
	#endregion
	#region OutlineDestination
	public class OutlineDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("solidFill", OnSolidFill);
			result.Add("noFill", OnNoFill);
			return result;
		}
		readonly IOutlineAccessor outlineAccessor;
		public OutlineDestination(WordProcessingMLBaseImporter importer, IOutlineAccessor outlineAccessor)
			: base(importer) {
			Guard.ArgumentNotNull(outlineAccessor, "outlineAccessor");
			this.outlineAccessor = outlineAccessor;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			int value = Importer.GetIntegerValue(reader, "w", Int32.MinValue);
			if (value != Int32.MinValue)
				outlineAccessor.OutlineWidth = Importer.UnitConverter.EmuToModelUnits(value);
		}
		static OutlineDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (OutlineDestination)importer.PeekDestination();
		}
		static Destination OnSolidFill(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SolidFillDestination(importer, GetThis(importer).outlineAccessor);
		}
		static Destination OnNoFill(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new NoFillDestination(importer, GetThis(importer).outlineAccessor);
		}
	}
	#endregion
	#region SRgbColorDestination
	public class SRgbColorDestination : LeafElementDestination {
		readonly IColorAccessor colorAccessor;
		public SRgbColorDestination(WordProcessingMLBaseImporter importer, IColorAccessor colorAccessor)
			: base(importer) {
			Guard.ArgumentNotNull(colorAccessor, "colorAccessor");
			this.colorAccessor = colorAccessor;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color color = Importer.GetColorValue(reader, "val");
			if (color != DXColor.Empty)
				colorAccessor.Color = color;
		}
	}
	#endregion
	#region PresetColorDestination
	public class PresetColorDestination : LeafElementDestination {
		readonly IColorAccessor colorAccessor;
		public PresetColorDestination(WordProcessingMLBaseImporter importer, IColorAccessor colorAccessor)
			: base(importer) {
			Guard.ArgumentNotNull(colorAccessor, "colorAccessor");
			this.colorAccessor = colorAccessor;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color color = Importer.GetPresetColorValue(reader, "val");
			if (color != DXColor.Empty)
				colorAccessor.Color = color;
		}
	}
	#endregion
	#region NoFillDestination
	public class NoFillDestination : LeafElementDestination {
		readonly IColorAccessor colorAccessor;
		public NoFillDestination(WordProcessingMLBaseImporter importer, IColorAccessor colorAccessor)
			: base(importer) {
			Guard.ArgumentNotNull(colorAccessor, "colorAccessor");
			this.colorAccessor = colorAccessor;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			colorAccessor.Color = DXColor.Empty;
		}
	}
	#endregion
	public class ColorScheme {
		readonly Dictionary<string, Color> colors = new Dictionary<string, Color>();
		public ColorScheme() {
			colors.Add("accent1", DXColor.FromArgb(0xFF, 0x4F, 0x81, 0xBD));
			colors.Add("accent2", DXColor.FromArgb(0xFF, 0xC0, 0x50, 0x4D));
			colors.Add("accent3", DXColor.FromArgb(0xFF, 0x9B, 0xBB, 0x59));
			colors.Add("accent4", DXColor.FromArgb(0xFF, 0x80, 0x64, 0xA2));
			colors.Add("accent5", DXColor.FromArgb(0xFF, 0x4B, 0xAC, 0xC6));
			colors.Add("accent6", DXColor.FromArgb(0xFF, 0xF7, 0x96, 0x46));
			colors.Add("bg1", DXColor.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
			colors.Add("bg2", DXColor.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
			colors.Add("dk1", DXColor.FromArgb(0xFF, 0x00, 0x00, 0x00));
			colors.Add("dk2", DXColor.FromArgb(0xFF, 0x1F, 0x49, 0x7D));
			colors.Add("folHlink", DXColor.FromArgb(0xFF, 0x80, 0x00, 0x80));
			colors.Add("hlink", DXColor.FromArgb(0xFF, 0x00, 0x00, 0xFF));
			colors.Add("lt1", DXColor.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
			colors.Add("lt2", DXColor.FromArgb(0xFF, 0xEE, 0xEC, 0xE1));
			colors.Add("phClr", DXColor.FromArgb(0xFF, 0x00, 0x00, 0x00));
			colors.Add("tx1", DXColor.FromArgb(0xFF, 0x00, 0x00, 0x00));
			colors.Add("tx2", DXColor.FromArgb(0xFF, 0x00, 0x00, 0x00));
		}
		public Color GetColorByName(string name) {
			Color result;
			if (colors.TryGetValue(name, out result))
				return result;
			return DXColor.Empty;
		}
	}
	#region SchemeColorDestination
	public class SchemeColorDestination : ElementDestination {
		static ColorScheme colorScheme = new ColorScheme();
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("alphaOff", OnAlphaOffset);
			result.Add("alphaMod", OnAlphaModulation);
			result.Add("alpha", OnAlpha);
			return result;
		}
		static SchemeColorDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (SchemeColorDestination)importer.PeekDestination();
		}
		static Destination OnAlphaOffset(WordProcessingMLBaseImporter importer, XmlReader reader) {
			Action<int> setter = value => GetThis(importer).alphaOffset = value / 1000;
			return new PropertyDestination<int>(importer, setter);
		}
		static Destination OnAlpha(WordProcessingMLBaseImporter importer, XmlReader reader) {
			Action<int> setter = value => GetThis(importer).alpha = Math.Min(100, Math.Abs(value) / 1000);
			return new PropertyDestination<int>(importer, setter);
		}
		static Destination OnAlphaModulation(WordProcessingMLBaseImporter importer, XmlReader reader) {
			Action<int> setter = value => GetThis(importer).alphaModulation = Math.Abs(value) / 1000;
			return new PropertyDestination<int>(importer, setter);
		}
		readonly IColorAccessor colorAccessor;
		Color baseColor;
		float? alphaOffset;
		float? alphaModulation;
		float? alpha;
		public SchemeColorDestination(WordProcessingMLBaseImporter importer, IColorAccessor colorAccessor)
			: base(importer) {
			Guard.ArgumentNotNull(colorAccessor, "colorAccessor");
			this.colorAccessor = colorAccessor;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			string colorName = reader.GetAttribute("val");
			if (String.IsNullOrEmpty(colorName))
				return;
			this.baseColor = colorScheme.GetColorByName(colorName);
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			if (DXColor.IsTransparentOrEmpty(baseColor))
				return;
			int actualAlpha = CalculateActualAlpha(baseColor.A);
			colorAccessor.Color = DXColor.FromArgb(actualAlpha, baseColor.R, baseColor.G, baseColor.B);
		}
		int CalculateActualAlpha(int value) {
			float alphaFactor = CalculateColorFactor(alpha, alphaModulation, alphaOffset);
			return (int)(value * alphaFactor);
		}
		float CalculateColorFactor(float? value, float? modulation, float? offset) {
			float modulationFactor = (modulation ?? 100) / 100;
			float actualValue = Math.Min(100, (value ?? 100) * modulationFactor);
			if (offset.HasValue)
				actualValue += offset.Value;
			actualValue = Validate(0, 100, actualValue);
			return actualValue / 100;
		}
		T Validate<T>(T min, T max, T value) where T : IComparable<T> {
			if (value.CompareTo(min) < 0)
				return min;
			return (value.CompareTo(max) < 0) ? value : max;
		}
	}
	#endregion
	#region TextBoxDestination
	public class TextBoxDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		readonly FloatingObjectImportInfo floatingObjectImportInfo;
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("txbxContent", OnTextBoxContent);
			return result;
		}
		public TextBoxDestination(WordProcessingMLBaseImporter importer, FloatingObjectImportInfo floatingObjectImportInfo)
			: base(importer) {
			Guard.ArgumentNotNull(floatingObjectImportInfo, "floatingObjectImportInfo");
			this.floatingObjectImportInfo = floatingObjectImportInfo;
			this.floatingObjectImportInfo.IsTextBox = true;
			Importer.PushCurrentPieceTable(new TextBoxContentType(importer.DocumentModel).PieceTable);
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected FloatingObjectImportInfo FloatingObjectImportInfo { get { return floatingObjectImportInfo; } }
		public override void ProcessElementOpen(XmlReader reader) {
			string style = reader.GetAttribute("style");
			string inset = reader.GetAttribute("inset");
			TextBoxProperties textBoxProperties = FloatingObjectImportInfo.TextBoxProperties;
			if (!String.IsNullOrEmpty(style))
				textBoxProperties.ResizeShapeToFitText = GetResizeShapeToFitText(style);
			if (String.IsNullOrEmpty(inset))
				return;
			int[] margins = GetMargins(inset);
			textBoxProperties.LeftMargin = GetValidMarginValue(margins[0], UnitConverter.DocumentsToModelUnits(30));
			textBoxProperties.RightMargin = GetValidMarginValue(margins[1], UnitConverter.DocumentsToModelUnits(30));
			textBoxProperties.TopMargin = GetValidMarginValue(margins[2], UnitConverter.DocumentsToModelUnits(15));
			textBoxProperties.BottomMargin = GetValidMarginValue(margins[3], UnitConverter.DocumentsToModelUnits(15));
		}
		public override void ProcessElementClose(XmlReader reader) {
			TextBoxContentType textBoxContent = (TextBoxContentType)Importer.PieceTable.ContentType;
			Importer.PieceTable.CheckIntegrity();
			Importer.PieceTable.FixLastParagraph();
			Importer.InsertBookmarks();
			Importer.InsertRangePermissions();
			Importer.PieceTable.FixTables();
			Importer.PopCurrentPieceTable();
			FloatingObjectImportInfo.TextBoxContent = textBoxContent;
		}
		int[] GetMargins(string strMargins) {
			int[] result = new int[4];
			string[] margins = strMargins.Split(',');
			for (int i = 0; i < margins.Length; i++)
				result[i] = GetFloatValue(margins[i]);
			if (margins.Length < 4)
				for (int i = margins.Length; i < result.Length; i++)
					result[i] = UnitConverter.DocumentsToModelUnits(15 + ((i < 2) ? 15 : 0)); 
			return result;
		}
		bool GetResizeShapeToFitText(string style) {
			return style.Substring(style.Length - 1) == "t";
		}
		int GetValidMarginValue(int value, int defaultValue) {
			if (value < 0)
				return defaultValue;
			else
				return value;
		}
		int GetFloatValue(string number) {
			ValueInfo valueUnit = StringValueParser.TryParse(number);
			if (!valueUnit.IsValidNumber)
				return Int32.MinValue;
			UnitsConverter unitsConverter = new UnitsConverter(UnitConverter);
			return (int)unitsConverter.ValueUnitToModelUnitsF(valueUnit);
		}
		static Destination OnTextBoxContent(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TextBoxContentDestination(importer);
		}
	}
	#endregion
	#region TextBoxPropertiesDestination
	public class TextBoxPropertiesDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("noAutofit", OnDisableAutoFit);
			result.Add("spAutoFit", OnEnableAutoFit);
			return result;
		}
		readonly TextBoxProperties textBoxProperties;
		public TextBoxPropertiesDestination(WordProcessingMLBaseImporter importer, TextBoxProperties textBoxProperties)
			: base(importer) {
			Guard.ArgumentNotNull(textBoxProperties, "textBoxProperties");
			this.textBoxProperties = textBoxProperties;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			int value;
			value = Importer.GetIntegerValue(reader, "lIns", Int32.MinValue);
			if (value != Int32.MinValue)
				textBoxProperties.LeftMargin = UnitConverter.EmuToModelUnits(value);
			value = Importer.GetIntegerValue(reader, "rIns", Int32.MinValue);
			if (value != Int32.MinValue)
				textBoxProperties.RightMargin = UnitConverter.EmuToModelUnits(value);
			value = Importer.GetIntegerValue(reader, "tIns", Int32.MinValue);
			if (value != Int32.MinValue)
				textBoxProperties.TopMargin = UnitConverter.EmuToModelUnits(value);
			value = Importer.GetIntegerValue(reader, "bIns", Int32.MinValue);
			if (value != Int32.MinValue)
				textBoxProperties.BottomMargin = UnitConverter.EmuToModelUnits(value);
			string wrapType = reader.GetAttribute("wrap");
			if (wrapType == "square")
				textBoxProperties.WrapText = true;
			else if (wrapType == "none")
				textBoxProperties.WrapText = false;
			value = Importer.GetIntegerValue(reader, "upright", 0);
			if (value == 1)
				textBoxProperties.Upright = true;
			VerticalAlignment invalidValue = (VerticalAlignment)(-1);
			VerticalAlignment verticalAlignment = Importer.GetEnumValue<VerticalAlignment>(reader, "anchor", WordProcessingMLBaseExporter.textBoxVerticalAlignmentTable, invalidValue);
			if (verticalAlignment != invalidValue)
				textBoxProperties.VerticalAlignment = verticalAlignment;
		}
		static TextBoxPropertiesDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (TextBoxPropertiesDestination)importer.PeekDestination();
		}
		static Destination OnDisableAutoFit(WordProcessingMLBaseImporter importer, XmlReader reader) {
			GetThis(importer).textBoxProperties.ResizeShapeToFitText = false;
			return null;
		}
		static Destination OnEnableAutoFit(WordProcessingMLBaseImporter importer, XmlReader reader) {
			GetThis(importer).textBoxProperties.ResizeShapeToFitText = true;
			return null;
		}
	}
	#endregion
	#region TextBoxContentDestination
	public class TextBoxContentDestination : BodyDestinationBase {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("p", OnParagraph);
			result.Add("tbl", OnTable);
			result.Add("bookmarkStart", OnBookmarkStart);
			result.Add("bookmarkEnd", OnBookmarkEnd);
			result.Add("permStart", OnRangePermissionStart);
			result.Add("permEnd", OnRangePermissionEnd);
			result.Add("sdt", OnStructuredDocument);
			result.Add("altChunk", OnAltChunk);
			result.Add("customXml", OnCustomXml);
			return result;
		}
		public TextBoxContentDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnTable(WordProcessingMLBaseImporter importer, XmlReader reader) {
			if (importer.DocumentModel.DocumentCapabilities.TablesAllowed)
				return new TableDestination(importer);
			else
				return new TableDisabledDestination(importer);
		}
		static Destination OnParagraph(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return importer.CreateParagraphDestination();
		}
		static Destination OnAltChunk(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new AltChunkDestination(importer);
		}
	}
	#endregion
	#region GraphicFrameDestination
	public class GraphicFrameDestination : LeafElementDestination {
		readonly Shape Shape;
		public GraphicFrameDestination(WordProcessingMLBaseImporter importer, Shape shape)
			: base(importer) {
				Guard.ArgumentNotNull(shape, "shape");
				this.Shape = shape;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int value = Importer.GetIntegerValue(reader, "rot", Int32.MinValue);
			if (value != Int32.MinValue)
				Shape.Rotation = UnitConverter.AdjAngleToModelUnits(value);
		}
	}
	#endregion
}
