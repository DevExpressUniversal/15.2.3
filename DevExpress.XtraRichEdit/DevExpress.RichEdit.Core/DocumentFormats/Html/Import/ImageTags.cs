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
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using System.Drawing;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Utils.Internal;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Import.Html {
	#region AreaTag
	public class AreaTag : TagBase {
		public AreaTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
	#region MapTag
	public class MapTag : TagBase {
		public MapTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
	public enum HtmlImageAlignment {
		None,
		Left,
		Right,
		Middle,
		Top,
		Bottom
	}
	#region ImageTag
	public class ImageTag : TagBase {		
		string uri = String.Empty;
		HtmlImageAlignment alignment = HtmlImageAlignment.None;
		ValueInfo widthInfo;
		ValueInfo heightInfo;
		static AttributeKeywordTranslatorTable attributeTable = AddAttributes();
		static Dictionary<string, HtmlImageAlignment> alignments = CreateAllignmentsTable();
		static Dictionary<string, HtmlImageAlignment> CreateAllignmentsTable() {
			Dictionary<string, HtmlImageAlignment> result = new Dictionary<string, HtmlImageAlignment>();
			result.Add("left", HtmlImageAlignment.Left);
			result.Add("right", HtmlImageAlignment.Right);
			result.Add("top", HtmlImageAlignment.Top);
			result.Add("bottom", HtmlImageAlignment.Bottom);
			result.Add("middle", HtmlImageAlignment.Middle);
			return result;
		}
		static AttributeKeywordTranslatorTable AddAttributes() {
			AttributeKeywordTranslatorTable table = CreateAttributeTable();
			table.Add(ConvertKeyToUpper("align"), ImageAlignmentKeyword);
			table.Add(ConvertKeyToUpper("alt"), AlternativeTextKeyword);
			table.Add(ConvertKeyToUpper("border"), BorderWidthKeyword);
			table.Add(ConvertKeyToUpper("height"), ImageHeightKeyword);
			table.Add(ConvertKeyToUpper("hspace"), HorizontalIndentKeyword);
			table.Add(ConvertKeyToUpper("ismap"), IsImageOnServerKeyword);
			table.Add(ConvertKeyToUpper("lowsec"), PreImageURLKeyword);
			table.Add(ConvertKeyToUpper("src"), ImageURLKeyword);
			table.Add(ConvertKeyToUpper("vspace"), VerticalIndentKeyword);
			table.Add(ConvertKeyToUpper("width"), ImageWidthKeyword);
			return table;
		}
		static internal void ImageAlignmentKeyword(HtmlImporter importer, string value, TagBase tag) {
			HtmlImageAlignment alignment;
			if (!alignments.TryGetValue(value.ToLower(), out alignment))
				alignment = HtmlImageAlignment.None;
			ImageTag imageTag = (ImageTag)tag;
			imageTag.alignment = alignment;
		}
		static internal void AlternativeTextKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		static internal void BorderWidthKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		static internal void ImageHeightKeyword(HtmlImporter importer, string value, TagBase tag) {
			ImageTag imageTag = (ImageTag)tag;
			imageTag.heightInfo = CalculateValueInfo(importer, value);
		}
		static internal void HorizontalIndentKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		static internal void IsImageOnServerKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		static internal void PreImageURLKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		static protected internal void ImageURLKeyword(HtmlImporter importer, string value, TagBase tag) {
			ImageTag imageTag = (ImageTag)tag;
			if (StringExtensions.StartsWithInvariantCultureIgnoreCase(value, "data:")) {
				imageTag.uri = value;
				return;
			}
			try {
				imageTag.uri = tag.GetAbsoluteUri(importer.AbsoluteBaseUri, System.Uri.UnescapeDataString(value));
			}
			catch { 
			}
		}
		static internal void VerticalIndentKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		static internal void ImageWidthKeyword(HtmlImporter importer, string value, TagBase tag) {
			ImageTag imageTag = (ImageTag)tag;
			imageTag.widthInfo = CalculateValueInfo(importer, value);
		}
		static ValueInfo CalculateValueInfo(HtmlImporter importer, string value) {
			int valueInPixels;
			LengthValueParser parsedValue;
			if (Int32.TryParse(value, out valueInPixels))
				parsedValue = new LengthValueParser(value + "px", importer.DocumentModel.ScreenDpi);
			else
				parsedValue = new LengthValueParser(value, importer.DocumentModel.ScreenDpi);
			if (parsedValue.IsRelativeUnit || !parsedValue.IsDigit)
				return new ValueInfo(parsedValue.Value, parsedValue.Unit);
			return new ValueInfo(parsedValue.PointsValue, "pt");
		}
		public ImageTag(HtmlImporter importer)
			: base(importer) {
			this.uri = String.Empty;
		}
		protected internal override AttributeKeywordTranslatorTable AttributeTable { get { return attributeTable; } }
		protected internal override bool ApplyStylesToInnerHtml { get { return false; } }
		protected virtual string Uri {
			get { return uri; }
			set { uri = value; }
		}
		protected internal override void ApplyTagProperties() {
			WidthUnitInfo width = CalculateWidthUnitInfo(this.widthInfo);
				Importer.Position.ImageProperties.Width = width;
			WidthUnitInfo height = CalculateWidthUnitInfo(this.heightInfo);
				Importer.Position.ImageProperties.Height = height;
			Importer.Position.ImageProperties.Alignment = this.alignment;
		}
		WidthUnitInfo CalculateWidthUnitInfo(ValueInfo info) {
			WidthUnitInfo result = new WidthUnitInfo();
			if (info != null) {
				if (info.Unit != "pt") {
					int fontSize = Importer.Position.CharacterFormatting.DoubleFontSize / 2;
					int rootDoubleFontSize = Importer.RootDoubleFontSize;
					return TagBaseHelper.GetRelativeWidth(Importer.UnitConverter, info.Unit, info.Value, fontSize, rootDoubleFontSize);
				}
				result.Value = Math.Max(0, (int)Math.Round(Importer.UnitConverter.PointsToModelUnitsF(info.Value)));
				result.Type = WidthUnitType.ModelUnits;
				return result;				
			}
			return result;
		}
		protected internal override void OpenTagProcessCore() {
			WidthUnitInfo width = Importer.Position.ImageProperties.Width;
			int widthInModelUnits = width.Type == WidthUnitType.ModelUnits ? width.Value : 0;
			WidthUnitInfo height = Importer.Position.ImageProperties.Height;
			int heightInModelUnits = height.Type == WidthUnitType.ModelUnits ? height.Value : 0;
			int widthInPixels = Importer.UnitConverter.ModelUnitsToPixels(widthInModelUnits);
			int heightInPixels = Importer.UnitConverter.ModelUnitsToPixels(heightInModelUnits);
			if (Importer.DocumentModel.DocumentCapabilities.InlinePicturesAllowed) {
				OfficeImage image = Importer.CreateUriBasedRichEditImage(Uri, widthInPixels, heightInPixels);
				if (image != null) {
					UriBasedOfficeImageBase uriBasedImage = image as UriBasedOfficeImageBase;
					float scaleX = 100.0f;
					float scaleY = 100.0f;
					Size desiredSizeInModelUnits = new Size(widthInModelUnits, heightInModelUnits);
					HtmlImageAlignment alignment = GetActualAlignment();
					if (Importer.Options.IgnoreFloatProperty || (alignment != HtmlImageAlignment.Left && alignment != HtmlImageAlignment.Right))
						Importer.AppendInlineImage(image, scaleX, scaleY, Size.Empty);
					else
						AppendPictureFloatingObjectContent(image, alignment, desiredSizeInModelUnits);
				}
			}
			else
				Importer.AppendText(" ");
			Importer.Position.CopyFrom(Importer.TagsStack[Importer.TagsStack.Count - 1].OldPosition);
		}
		HtmlImageAlignment GetActualAlignment() {
			if (Importer.Position.CssFloat == HtmlCssFloat.NotSet)
				return this.alignment;
			else if (Importer.Position.CssFloat == HtmlCssFloat.Left)
				return HtmlImageAlignment.Left;
			else if (Importer.Position.CssFloat == HtmlCssFloat.Right)
				return HtmlImageAlignment.Right;
			return HtmlImageAlignment.None;				
		}
		void AppendPictureFloatingObjectContent(OfficeImage image, HtmlImageAlignment alignment, Size desiredSizeInModelUnits) {
			PieceTable pieceTable = Importer.PieceTable;
			FloatingObjectAnchorRun run = pieceTable.InsertFloatingObjectAnchorCore(Importer.Position);
			Importer.SetAppendObjectProperty();
			run.FloatingObjectProperties.BeginUpdate();
			run.FloatingObjectProperties.TextWrapType = FloatingObjectTextWrapType.Square;
			run.FloatingObjectProperties.VerticalPositionType = FloatingObjectVerticalPositionType.Line;
			if (alignment == HtmlImageAlignment.Left) {
				run.FloatingObjectProperties.TextWrapSide = FloatingObjectTextWrapSide.Right;
				run.FloatingObjectProperties.HorizontalPositionAlignment = FloatingObjectHorizontalPositionAlignment.Left;
				run.FloatingObjectProperties.HorizontalPositionType = FloatingObjectHorizontalPositionType.Column;
			}
			else {
				run.FloatingObjectProperties.TextWrapSide = FloatingObjectTextWrapSide.Left;
				run.FloatingObjectProperties.HorizontalPositionAlignment = FloatingObjectHorizontalPositionAlignment.Right;
				run.FloatingObjectProperties.HorizontalPositionType = FloatingObjectHorizontalPositionType.Column;
			}
			run.FloatingObjectProperties.EndUpdate();
			PictureFloatingObjectContent content = new PictureFloatingObjectContent(run, image);
			Size originalSize = InternalOfficeImageHelper.CalculateImageSizeInModelUnits(image, DocumentModel.UnitConverter);
			content.SetOriginalSize(originalSize);
			if (desiredSizeInModelUnits.Width == 0 && desiredSizeInModelUnits.Height == 0)
				desiredSizeInModelUnits = originalSize;
			run.FloatingObjectProperties.ActualSize = desiredSizeInModelUnits;
			run.SetContent(content);
			if (image.IsLoaded) {
				UriBasedOfficeImageBase uriBasedOfficeImage = image as UriBasedOfficeImageBase;
				if (uriBasedOfficeImage != null)
					InternalUriBasedOfficeImageBaseHelper.RaiseEventsWhenImageLoaded(uriBasedOfficeImage);
			}
			else
				content.InvalidActualSize = true;
		}
		protected internal override void EmptyTagProcess() {
			Importer.OpenProcess(this);
			Importer.CloseProcess(this);
		}
	}
	#endregion
}
