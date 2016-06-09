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
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.Import.OpenXml;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.XtraSpreadsheet.Internal;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region DrawingTextParagraphPropertiesDestinationBase (abstract class)
	public abstract class DrawingTextParagraphPropertiesDestinationBase : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly DrawingTextParagraphProperties properties;
		protected DrawingTextParagraphPropertiesDestinationBase(SpreadsheetMLBaseImporter importer, DrawingTextParagraphProperties properties)
			: base(importer) {
			this.properties = properties;
		}
		protected DrawingTextParagraphProperties Properties { get { return properties; } }
	}
	#endregion
	#region DrawingTextParagraphPropertiesDestination
	public class DrawingTextParagraphPropertiesDestination : DrawingTextParagraphPropertiesDestinationBase {
		#region Static Members
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("buAutoNum", OnBulletAutoNum);
			result.Add("buBlip", OnBulletBlip);
			result.Add("buChar", OnBulletCharacter);
			result.Add("buClr", OnBulletColor);
			result.Add("buClrTx", OnBulletColorFollowText);
			result.Add("buFont", OnBulletFont);
			result.Add("buFontTx", OnBulletFontFollowText);
			result.Add("buNone", OnBulletNone);
			result.Add("buSzPct", OnBulletSizePercentage);
			result.Add("buSzPts", OnBulletSizePoints);
			result.Add("buSzTx", OnBulletSizeFollowText);
			result.Add("defRPr", OnDefaultCharacterProperties);
			result.Add("lnSpc", OnLineSpacing);
			result.Add("spcAft", OnSpaceAfter);
			result.Add("spcBef", OnSpaceBefore);
			result.Add("tabLst", OnTabStopList);
			return result;
		}
		#endregion
		static DrawingTextParagraphPropertiesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (DrawingTextParagraphPropertiesDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnBulletAutoNum(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DrawingBulletAutoNumberedDestination(importer, GetThis(importer).Properties);
		}
		static Destination OnBulletBlip(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DrawingBulletPictureDestination(importer, GetThis(importer).Properties);
		}
		static Destination OnBulletCharacter(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DrawingBulletCharacterDestination(importer, GetThis(importer).Properties);
		}
		static Destination OnBulletColor(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DrawingColor color = new DrawingColor(importer.ActualDocumentModel);
			GetThis(importer).Properties.Bullets.Color = color;
			return new DrawingColorDestination(importer, color);
		}
		static Destination OnBulletColorFollowText(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			GetThis(importer).Properties.Bullets.Color = DrawingBullet.ColorFollowText;
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		static Destination OnBulletFont(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DrawingTextFont font = new DrawingTextFont(importer.ActualDocumentModel);
			GetThis(importer).Properties.Bullets.Typeface = font;
			return new DrawingTextFontDestination(importer, font);
		}
		static Destination OnBulletFontFollowText(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			GetThis(importer).Properties.Bullets.Typeface = DrawingBullet.TypefaceFollowText;
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		static Destination OnBulletNone(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			GetThis(importer).Properties.Bullets.Common = DrawingBullet.NoBullets;
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		static Destination OnBulletSizePercentage(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DrawingBulletSizePercentageDestination(importer, GetThis(importer).Properties);
		}
		static Destination OnBulletSizePoints(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DrawingBulletSizePointsDestination(importer, GetThis(importer).Properties);
		}
		static Destination OnBulletSizeFollowText(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			GetThis(importer).Properties.Bullets.Size = DrawingBullet.SizeFollowText;
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		static Destination OnDefaultCharacterProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DrawingTextParagraphProperties properties = GetThis(importer).Properties;
			properties.ApplyDefaultCharacterProperties = true;
			return new DrawingTextCharacterPropertiesDestination(importer, properties.DefaultCharacterProperties);
		}
		static Destination OnLineSpacing(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DrawingTextSpacingDestination(importer, GetThis(importer).Properties, DrawingTextParagraphProperties.LineTextSpacingIndex);
		}
		static Destination OnSpaceAfter(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DrawingTextSpacingDestination(importer, GetThis(importer).Properties, DrawingTextParagraphProperties.SpaceAfterTextSpacingIndex);
		}
		static Destination OnSpaceBefore(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DrawingTextSpacingDestination(importer, GetThis(importer).Properties, DrawingTextParagraphProperties.SpaceBeforeTextSpacingIndex);
		}
		static Destination OnTabStopList(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DrawingTextTabStopListDestination(importer, GetThis(importer).Properties.TabStopList);
		}
		#endregion
		#endregion
		public DrawingTextParagraphPropertiesDestination(SpreadsheetMLBaseImporter importer, DrawingTextParagraphProperties properties)
			: base(importer, properties) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			Properties.BeginUpdate();
			DrawingTextAlignmentType? textAlignment = Importer.GetWpEnumOnOffNullValue(reader, "algn", OpenXmlExporter.DrawingTextAlignmentTypeTable);
			if (textAlignment.HasValue)
				Properties.TextAlignment = textAlignment.Value;
			DrawingFontAlignmentType? fontAlignment = Importer.GetWpEnumOnOffNullValue(reader, "fontAlgn", OpenXmlExporter.DrawingFontAlignmentTypeTable);
			if (fontAlignment.HasValue)
				Properties.FontAlignment = fontAlignment.Value;
			int? indent = Importer.GetIntegerNullableValue(reader, "indent");
			if (indent.HasValue)
				Properties.Indent = indent.Value;
			bool? latinLineBreak = Importer.GetWpSTOnOffNullValue(reader, "latinLnBrk");
			if (latinLineBreak.HasValue)
				Properties.LatinLineBreak = latinLineBreak.Value;
			bool? eastAsianLineBreak = Importer.GetWpSTOnOffNullValue(reader, "eaLnBrk");
			if (eastAsianLineBreak.HasValue)
				Properties.EastAsianLineBreak = eastAsianLineBreak.Value;
			bool? hangingPunctuation = Importer.GetWpSTOnOffNullValue(reader, "hangingPunct");
			if (hangingPunctuation.HasValue)
				Properties.HangingPunctuation = hangingPunctuation.Value;
			bool? rightToLeft = Importer.GetWpSTOnOffNullValue(reader, "rtl");
			if (rightToLeft.HasValue)
				Properties.RightToLeft = rightToLeft.Value;
			int? leftMargin = Importer.GetIntegerNullableValue(reader, "marL");
			if (leftMargin.HasValue)
				Properties.Margin.Left = leftMargin.Value;
			int? rightMargin = Importer.GetIntegerNullableValue(reader, "marR");
			if (rightMargin.HasValue)
				Properties.Margin.Right = rightMargin.Value;
			int? defaultTabSize = Importer.GetIntegerNullableValue(reader, "defTabSz");
			if (defaultTabSize.HasValue)
				Properties.DefaultTabSize = defaultTabSize.Value;
			int? textIndentLevel = Importer.GetIntegerNullableValue(reader, "lvl");
			if (textIndentLevel.HasValue) {
				DrawingValueChecker.CheckTextIndentLevelValue(textIndentLevel.Value);
				Properties.TextIndentLevel = textIndentLevel.Value;
			}
		}
		public override void ProcessElementClose(XmlReader reader) {
			Properties.EndUpdate();
		}
	}
	#endregion
}
