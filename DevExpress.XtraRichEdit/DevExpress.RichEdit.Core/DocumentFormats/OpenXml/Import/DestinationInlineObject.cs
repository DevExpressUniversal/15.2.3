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
using System.Xml;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Export.OpenXml;
using DevExpress.XtraRichEdit.Import.Html;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Import.OpenDocument;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Utils.Internal;
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	#region InlineObjectDestination
	public class InlineObjectDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("shape", OnShape);
			return result;
		}
		static readonly Dictionary<FloatingObjectHorizontalPositionType, WordProcessingMLValue> horizontalPositionTypeAttributeTable = CreateHorizontalPositionTypeAttributeTable();
		static readonly Dictionary<FloatingObjectVerticalPositionType, WordProcessingMLValue> verticalPositionTypeAttributeTable = CreateVerticalPositionTypeAttributeTable();
		#region Translation tables
		#region CreateFloatingObjectHorizontalPositionTypeAttributeTable
		static Dictionary<FloatingObjectHorizontalPositionType, WordProcessingMLValue> CreateHorizontalPositionTypeAttributeTable() {
			Dictionary<FloatingObjectHorizontalPositionType, WordProcessingMLValue> result = new Dictionary<FloatingObjectHorizontalPositionType, WordProcessingMLValue>();
			Add(result, FloatingObjectHorizontalPositionType.Column, "text");
			Add(result, FloatingObjectHorizontalPositionType.Margin, "margin");
			Add(result, FloatingObjectHorizontalPositionType.Page, "page");
			Add(result, FloatingObjectHorizontalPositionType.Character, "char");
			Add(result, FloatingObjectHorizontalPositionType.LeftMargin, "left-margin-area");
			Add(result, FloatingObjectHorizontalPositionType.RightMargin, "right-margin-area");
			Add(result, FloatingObjectHorizontalPositionType.InsideMargin, "inner-margin-area");
			Add(result, FloatingObjectHorizontalPositionType.OutsideMargin, "outer-margin-area");
			return result;
		}
		#endregion
		#region CreateFloatingObjectVerticalPositionTypeAttributeTable
		static Dictionary<FloatingObjectVerticalPositionType, WordProcessingMLValue> CreateVerticalPositionTypeAttributeTable() {
			Dictionary<FloatingObjectVerticalPositionType, WordProcessingMLValue> result = new Dictionary<FloatingObjectVerticalPositionType, WordProcessingMLValue>();
			Add(result, FloatingObjectVerticalPositionType.Margin, "margin");
			Add(result, FloatingObjectVerticalPositionType.Page, "page");
			Add(result, FloatingObjectVerticalPositionType.Line, "line");
			Add(result, FloatingObjectVerticalPositionType.Paragraph, "text");
			Add(result, FloatingObjectVerticalPositionType.TopMargin, "top-margin-area");
			Add(result, FloatingObjectVerticalPositionType.BottomMargin, "bottom-margin-area");
			Add(result, FloatingObjectVerticalPositionType.InsideMargin, "inner-margin-area");
			Add(result, FloatingObjectVerticalPositionType.OutsideMargin, "outer-margin-area");
			return result;
		}
		#endregion
		protected static void Add<T>(Dictionary<T, WordProcessingMLValue> table, T key, string openXmlValue) {
			table.Add(key, new WordProcessingMLValue(openXmlValue));
		}
		#endregion
		readonly FloatingObjectImportInfo floatingObjectImportInfo;
		string style;
		public InlineObjectDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
			this.floatingObjectImportInfo = new FloatingObjectImportInfo(importer.PieceTable);
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public OfficeImage Image { get { return FloatingObjectImportInfo.Image; } set { floatingObjectImportInfo.Image = value; } }
		public FloatingObjectProperties FloatingObject { get { return floatingObjectImportInfo.FloatingObjectProperties; } }
		public string Style { get { return style; } set { style = value; } }
		public FloatingObjectImportInfo FloatingObjectImportInfo { get { return floatingObjectImportInfo; } }
		protected virtual Dictionary<FloatingObjectHorizontalPositionType, WordProcessingMLValue> HorizontalPositionTypeAttributeTable { get { return horizontalPositionTypeAttributeTable; } }
		protected virtual Dictionary<FloatingObjectVerticalPositionType, WordProcessingMLValue> VerticalPositionTypeAttributeTable { get { return verticalPositionTypeAttributeTable; } }
		internal static Dictionary<FloatingObjectHorizontalPositionType, WordProcessingMLValue> OpenXmlHorizontalPositionTypeAttributeTable { get { return horizontalPositionTypeAttributeTable; } }
		internal static Dictionary<FloatingObjectVerticalPositionType, WordProcessingMLValue> OpenXmlVerticalPositionTypeAttributeTable { get { return verticalPositionTypeAttributeTable; } }
		static InlineObjectDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (InlineObjectDestination)importer.PeekDestination();
		}
		static Destination OnShape(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new VMLShapeDestination(importer, GetThis(importer));
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (FloatingObjectImportInfo.IsTextBox && FloatingObjectImportInfo.TextBoxContent != null) {
				FloatingObjectImportInfo.IsFloatingObject = true;
				InlinePictureCssParser cssParser = GetCssParser(new Size(0, 0));
				ImportFloatingObject(cssParser);
			}
			else {
				if (Image == null)
					return;
				if (!ShouldInsertPicture()) {
					Importer.PieceTable.InsertTextCore(Importer.Position, " ");
					return;
				}
				Size originalSize = InternalOfficeImageHelper.CalculateImageSizeInModelUnits(Image, Importer.DocumentModel.UnitConverter);
				InlinePictureCssParser cssParser = GetCssParser(originalSize);
				if (IsFloatingObject()) {
					FloatingObjectImportInfo.IsFloatingObject = true;
					if (FloatingObjectImportInfo.IsFloatingObject)
						ImportFloatingObject(cssParser);
					else {
						SizeF scale = CalculateScale(originalSize, cssParser);
						AppendImage(Importer.PieceTable, Importer.Position, Image, scale.Width, scale.Height);
						return;
					}
				}
				else {
					SizeF scale = CalculateScale(originalSize, cssParser);
					AppendImage(Importer.PieceTable, Importer.Position, Image, scale.Width, scale.Height);
					return;
				}
			}
			FloatingObjectImportInfo.InsertFloatingObject(Importer.Position);
		}
		protected virtual void AppendImage(PieceTable pieceTable, ImportInputPosition position, OfficeImage image, float scaleX, float scaleY) {
			pieceTable.AppendImage(position, image, scaleX, scaleY);
		}
		InlinePictureCssParser GetCssParser(Size size) {
			InlinePictureCssParser cssParser = new InlinePictureCssParser(Importer.DocumentModel, size);
			using (StringReader reader = new StringReader(style)) {
				cssParser.ParseAttribute(reader);
			}
			return cssParser;
		}
		void ImportFloatingObject(InlinePictureCssParser cssParser) {
			FloatingObject.ZOrder = cssParser.ZOrder;
			FloatingObject.ActualSize = new Size(cssParser.Size.Width, cssParser.Size.Height);
			FloatingObject.TopDistance = cssParser.TopDistance;
			FloatingObject.BottomDistance = cssParser.BottomDistance;
			FloatingObject.LeftDistance = cssParser.LeftDistance;
			FloatingObject.RightDistance = cssParser.RightDistance;
			FloatingObject.Offset = cssParser.Offset;
			if (cssParser.HorizontalPositionAlignment != FloatingObjectHorizontalPositionAlignment.None)
				FloatingObject.HorizontalPositionAlignment = cssParser.HorizontalPositionAlignment;
			if (cssParser.VerticalPositionAlignment != FloatingObjectVerticalPositionAlignment.None)
				FloatingObject.VerticalPositionAlignment = cssParser.VerticalPositionAlignment;
			if (cssParser.UseRelativeWidth)
				FloatingObject.RelativeWidth = new FloatingObjectRelativeWidth(cssParser.FromWidth, cssParser.WidthPercent);
			if (cssParser.UseRelativeHeight)
				FloatingObject.RelativeHeight = new FloatingObjectRelativeHeight(cssParser.FromHeight, cssParser.HeightPercent);
			FloatingObject.HorizontalPositionType = ConvertToHorizontalPositionType(cssParser.HorizontalPositionType);
			FloatingObject.VerticalPositionType = ConvertToVerticalPositionType(cssParser.VerticalPositionType);
			if(cssParser.TextBoxVerticalAlignment != VerticalAlignment.Top)
				FloatingObjectImportInfo.TextBoxProperties.VerticalAlignment = cssParser.TextBoxVerticalAlignment;
			if (cssParser.UseWrapText)
				FloatingObjectImportInfo.TextBoxProperties.WrapText = cssParser.WrapText;
			if (cssParser.UseRotation)
				FloatingObjectImportInfo.Shape.Rotation = cssParser.Rotation;
		}
		protected internal virtual FloatingObjectHorizontalPositionType ConvertToHorizontalPositionType(string value) {
			Dictionary<FloatingObjectHorizontalPositionType, WordProcessingMLValue> table = HorizontalPositionTypeAttributeTable;
			foreach (FloatingObjectHorizontalPositionType key in table.Keys) {
				if ((value == table[key].OpenXmlValue))
					return key;
			}
			return FloatingObjectHorizontalPositionType.Column;
		}
		protected internal virtual FloatingObjectVerticalPositionType ConvertToVerticalPositionType(string value) {
			Dictionary<FloatingObjectVerticalPositionType, WordProcessingMLValue> table = VerticalPositionTypeAttributeTable;
			foreach (FloatingObjectVerticalPositionType key in table.Keys) {
				if ((value == table[key].OpenXmlValue))
					return key;
			}
			return FloatingObjectVerticalPositionType.Paragraph;
		}
		internal bool ShouldInsertPicture() {
			return Importer.DocumentModel.DocumentCapabilities.InlinePicturesAllowed;
		}
		protected internal SizeF CalculateScale(Size originalSize, InlinePictureCssParser cssParser) {
			if (String.IsNullOrEmpty(style))
				return new Size(100, 100); 
			float scaleX = Math.Max(1, 100f * cssParser.Size.Width / originalSize.Width);
			float scaleY = Math.Max(1, 100f * cssParser.Size.Height / originalSize.Height);
			return new SizeF(scaleX, scaleY);
		}
		protected internal bool IsFloatingObject() {
			return Style.Contains("position:absolute");
		}
	}
	#endregion
}
