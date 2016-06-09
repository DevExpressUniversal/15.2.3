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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using System.Collections.Specialized;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using System.Globalization;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Export.Html {
	#region HtmlTableStyleHelper
	public class HtmlTableStyleHelper {
		readonly DocumentModelUnitConverter unitConverter;
		readonly HtmlStyleHelper styleHelper;
		public HtmlTableStyleHelper(DocumentModelUnitConverter unitConverter, HtmlStyleHelper styleHelper) {
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			Guard.ArgumentNotNull(styleHelper, "styleHelper");
			this.unitConverter = unitConverter;
			this.styleHelper = styleHelper;
		}
		public DocumentModelUnitConverter UnitConverter { get { return unitConverter; } }
		public HtmlStyleHelper HtmlStyleHelper { get { return styleHelper; } }
		public string GetHtmlTableStyle(Table table) {
			DXCssStyleCollection style = new DXCssStyleCollection();
			GetHtmlTableStyle(table, style);
			return HtmlStyleHelper.CreateCssStyle(style, ';');
		}
		public void GetHtmlTableStyle(Table table, DXCssStyleCollection style) {
			Guard.ArgumentNotNull(table, "table");
			float tableIndent = ConvertWidthUnitToPointsF(table.TableIndent);
			if (tableIndent != 0)
				style.Add("margin-left", HtmlStyleHelper.GetHtmlSizeInPoints(tableIndent));
			if (table.TableProperties.Info.UseBackgroundColor)
				style.Add("background-color", HtmlConvert.ToHtml(table.BackgroundColor));
		}
		internal string GetHtmlBorder(BorderBase border) {
			if (IsBorderNil(border))
				return "none";
			string htmlColor;
			if (DXColor.IsEmpty(border.Color))
				htmlColor = "windowtext";
			else
				htmlColor = HtmlConvert.ToHtml(border.Color);
			double width = Math.Max(1.0, Math.Round(UnitConverter.ModelUnitsToPointsF(border.Width), 1));
			string borderWidth = String.Format(CultureInfo.InvariantCulture, "{0}pt", width);
			string htmlBorderStyle = GetHtmlBorderStyle(border.Style);
			return borderWidth + " " + htmlColor + " " + htmlBorderStyle;
		}
		bool IsBorderNil(BorderBase border) {
			return border.Style == BorderLineStyle.None || border.Style == BorderLineStyle.Nil || border.Style == BorderLineStyle.Disabled;
		}
		string GetHtmlBorderStyle(BorderLineStyle borderStyle) {
			switch (borderStyle) {
				case BorderLineStyle.Dotted:
					return "dotted";
				case BorderLineStyle.Dashed:
					return "dashed";
				case BorderLineStyle.Double:
					return "double";
				case BorderLineStyle.Inset:
					return "inset";
				case BorderLineStyle.Outset:
					return "outset";
				default: return "solid";
			}
		}
		protected internal string GetHtmlTableCellStyle(TableCell cell, BorderBase actualBottomBorderCell) {
			DXCssStyleCollection style = new DXCssStyleCollection();
			GetHtmlTableCellStyle(cell, style, actualBottomBorderCell);
			return HtmlStyleHelper.CreateCssStyle(style, ';');
		}
		public void GetHtmlTableCellStyle(TableCell cell, DXCssStyleCollection style, BorderBase actualBottomBorderCell) {
			Guard.ArgumentNotNull(cell, "tableCell");
			if (cell.Properties.UsePreferredWidth) {
				string widthContent = ConvertWidthUnitToPoints(cell.PreferredWidth);
				if (!String.IsNullOrEmpty(widthContent))
					style.Add("width", widthContent);
			}
			style.Add("padding",
				HtmlStyleHelper.GetHtmlSizeInPoints(UnitConverter.ModelUnitsToPointsF(cell.GetActualTopMargin().Value)) + " " +
				HtmlStyleHelper.GetHtmlSizeInPoints(UnitConverter.ModelUnitsToPointsF(cell.GetActualRightMargin().Value)) + " " +
				HtmlStyleHelper.GetHtmlSizeInPoints(UnitConverter.ModelUnitsToPointsF(cell.GetActualBottomMargin().Value)) + " " +
				HtmlStyleHelper.GetHtmlSizeInPoints(UnitConverter.ModelUnitsToPointsF(cell.GetActualLeftMargin().Value))
				);
			if (cell.Properties.Info.UseBackgroundColor)
				style.Add("background-color", HtmlConvert.ToHtml(cell.BackgroundColor));
			style.Add("border-top", GetHtmlBorder(cell.GetActualTopCellBorder()));
			style.Add("border-right", GetHtmlBorder(cell.GetActualRightCellBorder()));
			if (actualBottomBorderCell == null)
				actualBottomBorderCell = cell.GetActualBottomCellBorder();
			style.Add("border-bottom", GetHtmlBorder(actualBottomBorderCell));
			style.Add("border-left", GetHtmlBorder(cell.GetActualLeftCellBorder()));
		}
		protected internal string ConvertWidthUnitToPixels(WidthUnit widthUnit) {
			if (widthUnit.Type == WidthUnitType.ModelUnits)
				return UnitConverter.ModelUnitsToPixels(widthUnit.Value).ToString(CultureInfo.InvariantCulture);
			else if (widthUnit.Type == WidthUnitType.FiftiethsOfPercent)
				return HtmlStyleHelper.GetHtmlSizeInPercents(widthUnit.Value / 50);
			else
				return "0";
		}
		protected internal string ConvertWidthUnitToPoints(WidthUnit widthUnit) {
			if (widthUnit.Type == WidthUnitType.ModelUnits)
				return  HtmlStyleHelper.GetHtmlSizeInPoints(UnitConverter.ModelUnitsToPointsF(widthUnit.Value));
			else if (widthUnit.Type == WidthUnitType.FiftiethsOfPercent)
				return HtmlStyleHelper.GetHtmlSizeInPercents(widthUnit.Value / 50);
			return String.Empty;
		}
		protected internal float ConvertWidthUnitToPointsF(WidthUnit widthUnit) {
			if (widthUnit.Type == WidthUnitType.ModelUnits)
				return UnitConverter.ModelUnitsToPointsF(widthUnit.Value);
			else if (widthUnit.Type == WidthUnitType.FiftiethsOfPercent)
				return widthUnit.Value / 50;
			else
				return 0;
		}
		protected internal string GetHtmlVerticalAlignment(VerticalAlignment vAlignment) {
			switch (vAlignment) {
				case VerticalAlignment.Top:
					return "top";
				case VerticalAlignment.Bottom:
					return "bottom";
				default:
					return "middle";
			}
		}
		protected internal virtual string GetHtmlTableAlignment(TableRowAlignment tableAlignment) {
			switch( tableAlignment){
				case TableRowAlignment.Center:
					return "center";
				case TableRowAlignment.Right:
					return "right";
				default:
					return "left";
			}
		}
	}
	#endregion
}
