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
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Export.Rtf;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region ShapeInstanceDestination
	public class ShapeInstanceDestination : DestinationBase {
		#region CreateKeywordTable
		static KeywordTranslatorTable keywordHT = CreateKeywordTable();
		static internal KeywordTranslatorTable CreateKeywordTable() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			table.Add("bin", OnBinKeyword);
			table.Add("shptxt", OnShapeTextKeyword);
			table.Add("shpleft", OnShapeLeftKeyword);
			table.Add("shpright", OnShapeRightKeyword);
			table.Add("shptop", OnShapeTopKeyword);
			table.Add("shpbottom", OnShapeBottomKeyword);
			table.Add("shpz", OnShapeZOrderKeyword);
			table.Add("shpbxpage", OnShapeHorizontalPositionTypePageKeyword);
			table.Add("shpbxmargin", OnShapeHorizontalPositionTypeMarginKeyword);
			table.Add("shpbxcolumn", OnShapeHorizontalPositionTypeColumnKeyword);
			table.Add("shpbypage", OnShapeVerticalPositionTypePageKeyword);
			table.Add("shpbymargin", OnShapeVerticalPositionTypeMarginKeyword);
			table.Add("shpbypara", OnShapeVerticalPositionTypeParagraphKeyword);
			table.Add("shpwr", OnShapeWrapTextTypeKeyword);
			table.Add("shpfblwtxt", OnShapeWrapTextTypeZOrderKeyword);
			table.Add("shpwrk", OnShapeWrapTextSideKeyword);
			table.Add("shplockanchor", OnShapeLockedKeyword);
			table.Add("sp", OnShapePropertyKeyword);
			return table;
		}
		#endregion
		readonly RtfFloatingObjectImportInfo floatingObjectImportInfo;
		readonly RtfShapeProperties properties;
		static void OnShapeTextKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			ShapeInstanceDestination destination = GetThis(importer);
			destination.FloatingObjectImportInfo.IsTextBox = true;
			destination.ConvertToFloatingObject();
			importer.Destination = new ShapeTextDestination(importer, destination.TextBoxContent);
		}
		static void OnShapePropertyKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new ShapePropertyDestination(importer, GetThis(importer).properties);
		}
		static void OnShapeLockedKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			GetThis(importer).FloatingObject.Locked = true;
		}
		static void OnShapeWrapTextSideKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (hasParameter)
				GetThis(importer).FloatingObject.TextWrapSide = ConvertFloatingObjectTextWrapSide(parameterValue);
		}
		static FloatingObjectTextWrapSide ConvertFloatingObjectTextWrapSide(int parameterValue) {
			FloatingObjectTextWrapSide textWrapSide = FloatingObjectTextWrapSide.Both;
			Dictionary<FloatingObjectTextWrapSide, int> table = RtfExportSR.FloatingObjectTextWrapSideTable;
			foreach (FloatingObjectTextWrapSide key in table.Keys)
				if (parameterValue == table[key])
					textWrapSide = key;
			return textWrapSide;  
		}
		static void OnShapeWrapTextTypeZOrderKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (hasParameter){
				if (GetThis(importer).FloatingObject.TextWrapType == FloatingObjectTextWrapType.None)
					GetThis(importer).FloatingObject.IsBehindDoc = (parameterValue != 0);
			}
		}
		static void OnShapeWrapTextTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (hasParameter) {
				FloatingObjectTextWrapType newWrapType = ConvertFloatingObjectTextWrapType(parameterValue);
				if (newWrapType == FloatingObjectTextWrapType.None)
					return;
				GetThis(importer).FloatingObject.TextWrapType = newWrapType;
			}
		}
		static FloatingObjectTextWrapType ConvertFloatingObjectTextWrapType(int parameterValue) {
			FloatingObjectTextWrapType textWrapType = FloatingObjectTextWrapType.None;
			Dictionary<FloatingObjectTextWrapType, int> table = RtfExportSR.FloatingObjectTextWrapTypeTable;
			foreach (FloatingObjectTextWrapType key in table.Keys)
				if (parameterValue == table[key])
					textWrapType = key;
			return textWrapType;	
		}
		static void OnShapeHorizontalPositionTypePageKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			GetThis(importer).FloatingObject.HorizontalPositionType = FloatingObjectHorizontalPositionType.Page;
		}
		static void OnShapeHorizontalPositionTypeMarginKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			GetThis(importer).FloatingObject.HorizontalPositionType = FloatingObjectHorizontalPositionType.Margin;
		}
		static void OnShapeHorizontalPositionTypeColumnKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			GetThis(importer).FloatingObject.HorizontalPositionType = FloatingObjectHorizontalPositionType.Column;
		}
		static void OnShapeVerticalPositionTypePageKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			GetThis(importer).FloatingObject.VerticalPositionType = FloatingObjectVerticalPositionType.Page;
		}
		static void OnShapeVerticalPositionTypeMarginKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			GetThis(importer).FloatingObject.VerticalPositionType = FloatingObjectVerticalPositionType.Margin;
		}
		static void OnShapeVerticalPositionTypeParagraphKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			GetThis(importer).FloatingObject.VerticalPositionType = FloatingObjectVerticalPositionType.Paragraph;
		}
		static void OnShapeLeftKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (hasParameter)
				GetThis(importer).OnShapeLeftKeyword(parameterValue);
		}
		static void OnShapeTopKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (hasParameter)
				GetThis(importer).OnShapeTopKeyword(parameterValue);
		}
		static void OnShapeRightKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (hasParameter)
				GetThis(importer).OnShapeRightKeyword(parameterValue);
		}
		static void OnShapeBottomKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (hasParameter)
				GetThis(importer).OnShapeBottomKeyword(parameterValue);
		}
		static void OnShapeZOrderKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			GetThis(importer).FloatingObject.ZOrder = parameterValue;
		}
		static internal ShapeInstanceDestination GetThis(RtfImporter rtfImporter) {
			return (ShapeInstanceDestination)rtfImporter.Destination;
		}
		public ShapeInstanceDestination(RtfImporter rtfImporter)
			: base(rtfImporter) {
			this.floatingObjectImportInfo = new RtfFloatingObjectImportInfo(rtfImporter.PieceTable);
			this.floatingObjectImportInfo.TextBoxContent = new TextBoxContentType(rtfImporter.DocumentModel);
			this.properties = new RtfShapeProperties();
		}
		public ShapeInstanceDestination(RtfImporter rtfImporter, RtfFloatingObjectImportInfo floatingObjectImportInfo, RtfShapeProperties properties)
			: base(rtfImporter) {
			this.floatingObjectImportInfo = floatingObjectImportInfo;
			this.properties = properties;
		}
		#region Properties
		public RtfFloatingObjectImportInfo FloatingObjectImportInfo { get { return floatingObjectImportInfo; } }
		public FloatingObjectProperties FloatingObject { get { return FloatingObjectImportInfo.FloatingObjectProperties; } }
		public Shape Shape { get { return FloatingObjectImportInfo.Shape; } }
		public TextBoxProperties TextBoxProperties { get { return FloatingObjectImportInfo.TextBoxProperties; } }
		public TextBoxContentType TextBoxContent { get { return FloatingObjectImportInfo.TextBoxContent; } }
		public RtfShapeProperties ShapeProperties { get { return properties; } }
		protected override ControlCharTranslatorTable ControlCharHT { get { return null; } }
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		#endregion
		protected override void ProcessControlCharCore(char ch) {
		}
		protected override bool ProcessKeywordCore(string keyword, int parameterValue, bool hasParameter) {
			TranslateKeywordHandler translator;
			if (KeywordHT.TryGetValue(keyword, out translator)) {
				translator(Importer, parameterValue, hasParameter);
				return true;
			}
			return false;
		}
		protected override void ProcessCharCore(char ch) {
		}
		protected override DestinationBase CreateClone() {
			return new ShapeInstanceDestination(Importer, FloatingObjectImportInfo, properties);
		}
		void ConvertToFloatingObject() {
			FloatingObjectImportInfo.IsFloatingObject = true;
		}
		void ImportFloatingObjectHorizontalPositionAlignment(int propertyValue) {
			Dictionary<FloatingObjectHorizontalPositionAlignment, int> table = RtfExportSR.FloatingObjectHorizontalPositionAlignmentTable;
			foreach (FloatingObjectHorizontalPositionAlignment key in table.Keys)
				if (propertyValue == table[key])
					FloatingObject.HorizontalPositionAlignment = key;
		}
		void ImportFloatingObjectHorizontalPositionType(int propertyValue) {
			Dictionary<FloatingObjectHorizontalPositionType, int> table = RtfExportSR.FloatingObjectHorizontalPositionTypeTable;
			foreach (FloatingObjectHorizontalPositionType key in table.Keys)
				if (propertyValue == table[key])
					FloatingObject.HorizontalPositionType = key;
		}
		void ImportFloatingObjectVerticalPositionAlignment(int propertyValue) {
			Dictionary<FloatingObjectVerticalPositionAlignment, int> table = RtfExportSR.FloatingObjectVerticalPositionAlignmentTable;
			foreach (FloatingObjectVerticalPositionAlignment key in table.Keys)
				if (propertyValue == table[key])
					FloatingObject.VerticalPositionAlignment = key;
		}
		void ImportFloatingObjectVerticalPositionType(int propertyValue) {
			Dictionary<FloatingObjectVerticalPositionType, int> table = RtfExportSR.FloatingObjectVerticalPositionTypeTable;
			foreach (FloatingObjectVerticalPositionType key in table.Keys)
				if (propertyValue == table[key])
					FloatingObject.VerticalPositionType = key;
		}
		FloatingObjectRelativeFromHorizontal ImportFloatingObjectRelaitveSizeHorizontalRelation(int propertyValue) {
			Dictionary<FloatingObjectRelativeFromHorizontal, int> table = RtfExportSR.FloatingObjectRelativeFromHorizontalTable;
			foreach (FloatingObjectRelativeFromHorizontal key in table.Keys)
				if (propertyValue == table[key])
					return key;
			return FloatingObjectRelativeFromHorizontal.Margin;
		}
		FloatingObjectRelativeFromVertical ImportFloatingObjectRelaitveSizeVerticalRelation(int propertyValue) {
			Dictionary<FloatingObjectRelativeFromVertical, int> table = RtfExportSR.FloatingObjectRelativeFromVerticalTable;
			foreach (FloatingObjectRelativeFromVertical key in table.Keys)
				if (propertyValue == table[key])
					return key;
			return FloatingObjectRelativeFromVertical.Margin;
		}		
		void OnShapeLeftKeyword(int parameterValue) {			
			FloatingObjectImportInfo.Left = parameterValue;
			ConvertToFloatingObject();
		}
		void OnShapeTopKeyword(int parameterValue) {
			FloatingObjectImportInfo.Top = parameterValue;
			ConvertToFloatingObject();
		}
		void OnShapeRightKeyword(int parameterValue) {
			FloatingObjectImportInfo.Right = parameterValue;
			ConvertToFloatingObject();
		}
		void OnShapeBottomKeyword(int parameterValue) {
			FloatingObjectImportInfo.Bottom = parameterValue;
			ConvertToFloatingObject();
		}
		public override void BeforePopRtfState() {
			if (FloatingObjectImportInfo.ShouldIgnore)
				return;
			if (HasIntegerProperty("posh"))
				ImportFloatingObjectHorizontalPositionAlignment(GetIntegerPropertyValue("posh"));
			if (HasIntegerProperty("posrelh"))
				ImportFloatingObjectHorizontalPositionType(GetIntegerPropertyValue("posrelh"));
			if (HasIntegerProperty("pctHorizPos"))
			   FloatingObject.PercentOffsetX = GetIntegerPropertyValue("pctHorizPos") * 100;
			if (HasIntegerProperty("pctVertPos"))
				FloatingObject.PercentOffsetY = GetIntegerPropertyValue("pctVertPos") * 100;
			if (HasIntegerProperty("posv"))
				ImportFloatingObjectVerticalPositionAlignment(GetIntegerPropertyValue("posv"));
			if (HasIntegerProperty("posrelv"))
				ImportFloatingObjectVerticalPositionType(GetIntegerPropertyValue("posrelv"));
			if (HasBoolProperty("fLayoutInCell"))
				FloatingObject.LayoutInTableCell = GetBoolPropertyValue("fLayoutInCell");
			if (HasBoolProperty("fAllowOverlap"))
				FloatingObject.AllowOverlap = GetBoolPropertyValue("fAllowOverlap");
			if (HasBoolProperty("fBehindDocument"))
				FloatingObject.IsBehindDoc = GetBoolPropertyValue("fBehindDocument");
			if (HasBoolProperty("fPseudoInline"))
				FloatingObject.PseudoInline = GetBoolPropertyValue("fPseudoInline");
			if (HasBoolProperty("fHidden"))
				FloatingObject.Hidden = GetBoolPropertyValue("fHidden");
			if (HasIntegerProperty("dxWrapDistLeft"))
				FloatingObject.LeftDistance = Importer.UnitConverter.EmuToModelUnits(GetIntegerPropertyValue("dxWrapDistLeft"));
			if (HasIntegerProperty("dxWrapDistRight"))
				FloatingObject.RightDistance = Importer.UnitConverter.EmuToModelUnits(GetIntegerPropertyValue("dxWrapDistRight"));
			if (HasIntegerProperty("dyWrapDistTop"))
				FloatingObject.TopDistance = Importer.UnitConverter.EmuToModelUnits(GetIntegerPropertyValue("dyWrapDistTop"));
			if (HasIntegerProperty("dyWrapDistBottom"))
				FloatingObject.BottomDistance = Importer.UnitConverter.EmuToModelUnits(GetIntegerPropertyValue("dyWrapDistBottom"));
			if (HasBoolProperty("fLockAspectRatio"))
				FloatingObject.LockAspectRatio = GetBoolPropertyValue("fLockAspectRatio");
			int rawRotation = 0;
			if (HasIntegerProperty("rotation")) {
				rawRotation = GetIntegerPropertyValue("rotation");
				Shape.Rotation = Importer.UnitConverter.FDToModelUnits(rawRotation);
			}
			int left = Importer.UnitConverter.TwipsToModelUnits(FloatingObjectImportInfo.Left);
			int top = Importer.UnitConverter.TwipsToModelUnits(FloatingObjectImportInfo.Top);
			int width = Math.Max(1, Importer.UnitConverter.TwipsToModelUnits(FloatingObjectImportInfo.Right - FloatingObjectImportInfo.Left));
			int height = Math.Max(1, Importer.UnitConverter.TwipsToModelUnits(FloatingObjectImportInfo.Bottom - FloatingObjectImportInfo.Top));
			if (!RichEditControlCompatibility.CompatibleRtfImportForRotatedImages && ShouldSwapSize(rawRotation)) {
				left -= width;
				top += width;
				FloatingObject.ActualSize = new Size(height, width);
			}
			else
				FloatingObject.ActualSize = new Size(width, height);
			FloatingObject.Offset = new Point(left, top);
			if (HasIntegerProperty("dxTextLeft"))
				TextBoxProperties.LeftMargin = Importer.UnitConverter.EmuToModelUnits(GetIntegerPropertyValue("dxTextLeft"));
			if (HasIntegerProperty("dxTextRight"))
				TextBoxProperties.RightMargin = Importer.UnitConverter.EmuToModelUnits(GetIntegerPropertyValue("dxTextRight"));
			if (HasIntegerProperty("dyTextTop"))
				TextBoxProperties.TopMargin = Importer.UnitConverter.EmuToModelUnits(GetIntegerPropertyValue("dyTextTop"));
			if (HasIntegerProperty("dyTextBottom"))
				TextBoxProperties.BottomMargin = Importer.UnitConverter.EmuToModelUnits(GetIntegerPropertyValue("dyTextBottom"));
			if (HasBoolProperty("fFitShapeToText"))
				TextBoxProperties.ResizeShapeToFitText = GetBoolPropertyValue("fFitShapeToText");
			if (HasIntegerProperty("WrapText"))
				TextBoxProperties.WrapText = GetIntegerPropertyValue("WrapText") != 2; 
			if (HasIntegerProperty("pctHoriz")) {
				int percentageWidth = GetIntegerPropertyValue("pctHoriz") * 100;
				if (HasIntegerProperty("sizerelh")) {
					FloatingObjectRelativeFromHorizontal relativeFrom = ImportFloatingObjectRelaitveSizeHorizontalRelation(GetIntegerPropertyValue("sizerelh"));
					FloatingObject.RelativeWidth = new FloatingObjectRelativeWidth(relativeFrom, percentageWidth);
				}
				else
					FloatingObject.RelativeWidth = new FloatingObjectRelativeWidth(FloatingObjectRelativeFromHorizontal.Page, percentageWidth);
			}
			if (HasIntegerProperty("pctVert")) {
				int percentageHeight = GetIntegerPropertyValue("pctVert") * 100;
				if (HasIntegerProperty("sizerelv")) {
					FloatingObjectRelativeFromVertical relativeFrom = ImportFloatingObjectRelaitveSizeVerticalRelation(GetIntegerPropertyValue("sizerelv"));
					FloatingObject.RelativeHeight = new FloatingObjectRelativeHeight(relativeFrom, percentageHeight);
				}
				else
					FloatingObject.RelativeHeight = new FloatingObjectRelativeHeight(FloatingObjectRelativeFromVertical.Page, percentageHeight);
			}
			if ((!HasBoolProperty("fLine") || GetBoolPropertyValue("fLine"))) {
				if (HasIntegerProperty("lineWidth"))
					Shape.OutlineWidth = Importer.UnitConverter.EmuToModelUnits(GetIntegerPropertyValue("lineWidth"));
				else
					Shape.OutlineWidth = Importer.UnitConverter.EmuToModelUnits(9525); 
				if (HasColorProperty("lineColor"))
					Shape.OutlineColor = GetColorPropertyValue("lineColor");
				else
					Shape.OutlineColor = DXColor.Black;
			}
			if ((!HasBoolProperty("fFilled") || GetBoolPropertyValue("fFilled")) && HasColorProperty("fillColor"))
				Shape.FillColor = GetColorPropertyValue("fillColor");
			if (HasStringProperty("wzName")) {
				string name = GetStringPropertyValue("wzName");
				if (!String.IsNullOrEmpty(name))
					FloatingObjectImportInfo.Name = name;
			}
			if (properties.ContainsKey("pib") && (properties["pib"] is RtfImageInfo)) {
				RtfImageInfo imageInfo = (RtfImageInfo)properties["pib"];
				FloatingObjectImportInfo.Image = imageInfo != null ? imageInfo.RtfImage : null;
				if (!FloatingObjectImportInfo.InsertFloatingObject(Importer.Position))
					DefaultDestination.InsertImage(Importer, imageInfo);
				FinalizeInsertion();
				FloatingObjectImportInfo.ShouldIgnore = true;
			}
			else if (FloatingObjectImportInfo.IsTextBox && TextBoxContent != null) {
				FloatingObjectImportInfo.InsertFloatingObject(Importer.Position);
				FinalizeInsertion();
				FloatingObjectImportInfo.ShouldIgnore = true;
			}
		}
		public static bool ShouldSwapSize(int rtfAngle) {
			rtfAngle = rtfAngle % (360 * 65536);
			if (rtfAngle < 0)
				rtfAngle += 360 * 65536;
			return ((rtfAngle >= 45 * 65536 && rtfAngle < 135 * 65536) || (rtfAngle >= 225 * 65536 && rtfAngle < 315 * 65536));
		}
		bool HasBoolProperty(string name) {
			return properties.HasBoolProperty(name);
		}
		bool HasIntegerProperty(string name) {
			return properties.HasIntegerProperty(name);
		}
		bool HasStringProperty(string name) {
			return properties.HasStringProperty(name);
		}
		protected internal bool HasColorProperty(string name) {
			return properties.HasColorProperty(name);
		}
		int GetIntegerPropertyValue(string name) {
			return properties.GetIntegerPropertyValue(name);
		}
		string GetStringPropertyValue(string name) {
			return properties.GetStringPropertyValue(name);
		}
		bool GetBoolPropertyValue(string name) {
			return properties.GetBoolPropertyValue(name);
		}
		protected internal Color GetColorPropertyValue(string name) {
			return properties.GetColorPropertyValue(name);
		}
		void FinalizeInsertion() {
			DefaultDestination.OnResetParagraphPropertiesKeyword(Importer, 0, false);
			if (!this.FloatingObjectImportInfo.IsFloatingObject)
				Importer.InsertParagraph();
		}
		public override void NestedGroupFinished(DestinationBase nestedDestination) {
			DestinationPieceTable pieceTableDestination = nestedDestination as DestinationPieceTable;
			if (pieceTableDestination != null) {
				if (!Object.ReferenceEquals(pieceTableDestination.PieceTable, PieceTable))
					pieceTableDestination.FinalizePieceTableCreation();
			}
		}
	}
	#endregion
	public class RtfFloatingObjectImportInfo : FloatingObjectImportInfo {
		public RtfFloatingObjectImportInfo(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public int Left { get; set; }
		public int Right { get; set; }
		public int Top { get; set; }
		public int Bottom { get; set; }
		public int RtfRotation { get; set; }
	}
	#region ShapeTextDestination
	public class ShapeTextDestination : DestinationPieceTable {
		static KeywordTranslatorTable keywordHT = CreateKeywordTable();
		static KeywordTranslatorTable CreateKeywordTable() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			AddCommonCharacterKeywords(table);
			AddCommonParagraphKeywords(table);
			AddCommonSymbolsAndObjectsKeywords(table);
			AddCommonTabKeywords(table);
			AddCommonNumberingListsKeywords(table);
			AppendTableKeywords(table);
			return table;
		}
		public ShapeTextDestination(RtfImporter importer, TextBoxContentType textBoxContentType)
			: base(importer, textBoxContentType.PieceTable) {
		}
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		protected override DestinationBase CreateClone() {
			return new ShapeTextDestination(Importer, (TextBoxContentType)PieceTable.ContentType);
		}
	}
	#endregion
	#region RtfShapeProperties
	public class RtfShapeProperties : Dictionary<string, object> {
		public bool HasBoolProperty(string name) {
			return HasIntegerProperty(name);
		}
		public bool HasIntegerProperty(string name) {
			return ContainsKey(name) && (this[name] is int);
		}
		public bool HasStringProperty(string name) {
			return ContainsKey(name) && (this[name] is string);
		}
		public bool HasColorProperty(string name) {
			return HasIntegerProperty(name);
		}
		public int GetIntegerPropertyValue(string name) {
			return (int)this[name];
		}
		public string GetStringPropertyValue(string name) {
			return (string)this[name];
		}
		public bool GetBoolPropertyValue(string name) {
			return GetIntegerPropertyValue(name) != 0;
		}
		public Color GetColorPropertyValue(string name) {
			int colorValue = GetIntegerPropertyValue(name);
			return DXColor.FromArgb(colorValue & 0x000000FF, (colorValue & 0x0000FF00) >> 8, (colorValue & 0x00FF0000) >> 16);
		}
	}
	#endregion
}
