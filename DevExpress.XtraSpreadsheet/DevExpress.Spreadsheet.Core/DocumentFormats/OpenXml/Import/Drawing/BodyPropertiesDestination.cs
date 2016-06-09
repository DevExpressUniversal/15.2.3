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

using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using System;
using System.Globalization;
using DevExpress.Office.DrawingML;
using DevExpress.Office.Drawing;
using DevExpress.Office.Import.OpenXml;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region DrawingTextBodyPropertiesDestination
	public class DrawingTextBodyPropertiesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("flatTx", OnFlatText);
			result.Add("noAutofit", OnNoAutoFit);
			result.Add("normAutofit", OnNormalAutoFit);
			result.Add("scene3d", OnScene3D);
			result.Add("sp3d", OnShape3D);
			result.Add("spAutoFit", OnShapeAutoFit);
			return result;
		}
		#endregion
		static DrawingTextBodyPropertiesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (DrawingTextBodyPropertiesDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnScene3D(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new Scene3DPropertiesDestination(importer, GetThis(importer).properties.Scene3D);
		}
		static Destination OnShape3D(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DrawingTextBodyPropertiesDestination destination = GetThis(importer);
			Shape3DProperties shape3d = new Shape3DProperties(importer.ActualDocumentModel);
			destination.properties.Text3D = shape3d;
			return new Shape3DPropertiesDestination(importer, shape3d);
		}
		static Destination OnNoAutoFit(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DrawingTextBodyPropertiesDestination destination = GetThis(importer);
			destination.properties.AutoFit = DrawingTextAutoFit.None;
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		static Destination OnShapeAutoFit(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DrawingTextBodyPropertiesDestination destination = GetThis(importer);
			destination.properties.AutoFit = DrawingTextAutoFit.Shape;
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		static Destination OnNormalAutoFit(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DrawingTextBodyPropertiesDestination destination = GetThis(importer);
			DrawingTextNormalAutoFit autoFit = new DrawingTextNormalAutoFit(importer.ActualDocumentModel);
			destination.properties.AutoFit = autoFit;
			return new DrawingTextNormalAutoFitDestination(importer, autoFit);
		}
		static Destination OnFlatText(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DrawingText3DFlatTextDestination(importer, GetThis(importer).properties);
		}
		#endregion
		#endregion
		readonly DrawingTextBodyProperties properties;
		public DrawingTextBodyPropertiesDestination(SpreadsheetMLBaseImporter importer, DrawingTextBodyProperties properties)
			: base(importer) {
			this.properties = properties;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		int ConvertInsetEmuToModel(int value) {
			DrawingValueChecker.CheckCoordinate32(value, "InsetCoordinate");
			return Importer.DocumentModel.UnitConverter.EmuToModelUnits(value);
		}
		public override void ProcessElementOpen(XmlReader reader) {
			properties.BeginUpdate();
			DrawingTextAnchoringType? anchor = Importer.GetWpEnumOnOffNullValue(reader, "anchor", OpenXmlExporter.AnchoringTypeTable);
			if (anchor.HasValue)
				properties.Anchor = anchor.Value;
			bool? anchorCenter = Importer.GetWpSTOnOffNullValue(reader, "anchorCtr");
			if (anchorCenter.HasValue)
				properties.AnchorCenter = anchorCenter.Value;
			int? bottom = Importer.GetIntegerNullableValue(reader, "bIns");
			if (bottom.HasValue)
				properties.Inset.Bottom = ConvertInsetEmuToModel(bottom.Value);
			int? top = Importer.GetIntegerNullableValue(reader, "tIns");
			if (top.HasValue)
				properties.Inset.Top = ConvertInsetEmuToModel(top.Value);
			int? left = Importer.GetIntegerNullableValue(reader, "lIns");
			if (left.HasValue)
				properties.Inset.Left = ConvertInsetEmuToModel(left.Value);
			int? right = Importer.GetIntegerNullableValue(reader, "rIns");
			if (right.HasValue)
				properties.Inset.Right = ConvertInsetEmuToModel(right.Value);
			bool? compatibleLineSpacing = Importer.GetWpSTOnOffNullValue(reader, "compatLnSpc");
			if (compatibleLineSpacing.HasValue)
				properties.CompatibleLineSpacing = compatibleLineSpacing.Value;
			bool? forceAntiAlias = Importer.GetWpSTOnOffNullValue(reader, "forceAA");
			if (forceAntiAlias.HasValue)
				properties.ForceAntiAlias = forceAntiAlias.Value;
			bool? fromWordArt = Importer.GetWpSTOnOffNullValue(reader, "fromWordArt");
			if (fromWordArt.HasValue)
				properties.FromWordArt = fromWordArt.Value;
			DrawingTextHorizontalOverflowType? horizontalOverflow = Importer.GetWpEnumOnOffNullValue(reader, "horzOverflow", OpenXmlExporter.HorizontalOverflowTypeTable);
			if (horizontalOverflow.HasValue)
				properties.HorizontalOverflow = horizontalOverflow.Value;
			int? numberOfColumns = Importer.GetIntegerNullableValue(reader, "numCol");
			if (numberOfColumns.HasValue)
				properties.NumberOfColumns = numberOfColumns.Value;
			int? rotation = Importer.GetIntegerNullableValue(reader, "rot");
			if (rotation.HasValue)
				properties.Rotation = rotation.Value;
			bool? rightToLeftColumns = Importer.GetWpSTOnOffNullValue(reader, "rtlCol");
			if (rightToLeftColumns.HasValue)
				properties.RightToLeftColumns = rightToLeftColumns.Value;
			int? spaceBetweenColumns = Importer.GetIntegerNullableValue(reader, "spcCol");
			if (spaceBetweenColumns.HasValue) {
				DrawingValueChecker.CheckPositiveCoordinate32(spaceBetweenColumns.Value, "SpaceBetweenColumns");
				properties.SpaceBetweenColumns = Importer.DocumentModel.UnitConverter.EmuToModelUnits(spaceBetweenColumns.Value);
			}
			bool? paragraphSpacing = Importer.GetWpSTOnOffNullValue(reader, "spcFirstLastPara");
			if (paragraphSpacing.HasValue)
				properties.ParagraphSpacing = paragraphSpacing.Value;
			properties.UprightText = Importer.GetOnOffValue(reader, "upright", DrawingTextBodyInfo.DefaultInfo.UprightText);
			DrawingTextVerticalTextType? verticalText = Importer.GetWpEnumOnOffNullValue(reader, "vert", OpenXmlExporter.VerticalTextTypeTable);
			if (verticalText.HasValue)
				properties.VerticalText = verticalText.Value;
			DrawingTextVerticalOverflowType? verticalOverflow = Importer.GetWpEnumOnOffNullValue(reader, "vertOverflow", OpenXmlExporter.VerticalOverflowTypeTable);
			if (verticalOverflow.HasValue)
				properties.VerticalOverflow = verticalOverflow.Value;
			DrawingTextWrappingType? textWrapping = Importer.GetWpEnumOnOffNullValue(reader, "wrap", OpenXmlExporter.TextWrappingTypeTable);
			if (textWrapping.HasValue)
				properties.TextWrapping = textWrapping.Value;
		}
		public override void ProcessElementClose(XmlReader reader) {
			properties.EndUpdate();
		}
	}
	#endregion
	#region DrawingTextNormalAutoFitDestination
	public class DrawingTextNormalAutoFitDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly DrawingTextNormalAutoFit autoFit;
		public DrawingTextNormalAutoFitDestination(SpreadsheetMLBaseImporter importer, DrawingTextNormalAutoFit autoFit)
			: base(importer) {
			this.autoFit = autoFit;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			autoFit.FontScale = Importer.GetIntegerValue(reader, "fontScale", DrawingTextNormalAutoFit.DefaultFontScale);
			autoFit.LineSpaceReduction = Importer.GetIntegerValue(reader, "lnSpcReduction", 0);
		}
	}
	#endregion
	#region DrawingText3DFlatTextDestination
	public class DrawingText3DFlatTextDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly DrawingTextBodyProperties properties;
		public DrawingText3DFlatTextDestination(SpreadsheetMLBaseImporter importer, DrawingTextBodyProperties properties)
			: base(importer) {				
			this.properties = properties;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			long z = Importer.GetLongValue(reader, "z", 0);
			DrawingValueChecker.CheckCoordinate(z, "FlatTextCoordinate");
			properties.Text3D = new DrawingText3DFlatText(Importer.DocumentModel.UnitConverter.EmuToModelUnitsL(z));
		}
	}
	#endregion
}
