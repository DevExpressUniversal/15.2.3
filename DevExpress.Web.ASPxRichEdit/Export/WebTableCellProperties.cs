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

using DevExpress.Office;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Export {
	public enum JSONTableCellProperty {
		CellMargins = 0,
		PreferredWidth = 1,
		Borders = 2,
		HideCellMark = 3,
		NoWrap = 4,
		FitText = 5,
		TextDirection = 6,
		VerticalAlignment = 7,
		BackgroundColor = 8,
		ForegroundColor = 9,
		Shading = 10,
		UseValue = 11,
		ColumnSpan = 12,
		VerticalMerging = 13
	}
	public class WebTableCellProperties : IHashtableProvider {
		CellMargins cellMargins;
		TableCellBorders borders;
		bool hideCellMark;
		bool noWrap;
		bool fitText;
		TextDirection textDirection;
		VerticalAlignment verticalAlignment;
		Color backgroundColor;
		Color foregroundColor;
		ShadingPattern shadingPattern;
		TableCellPropertiesOptions.Mask useValue;
		public WebTableCellProperties(TableCellProperties properties) {
			cellMargins = properties.CellMargins;
			borders = properties.Borders;
			hideCellMark = properties.HideCellMark;
			noWrap = properties.NoWrap;
			fitText = properties.FitText;
			textDirection = properties.TextDirection;
			verticalAlignment = properties.VerticalAlignment;
			backgroundColor = properties.BackgroundColor;
			foregroundColor = properties.ForegroundColor;
			shadingPattern = properties.ShadingPattern;
			useValue = properties.UseValue;
		}
		public Hashtable ToHashtable() {
			var result = new Hashtable();
			FillHashtable(result);
			return result;
		}
		public void FillHashtable(Hashtable result) {
			result.Add((int)JSONTableCellProperty.CellMargins, TableCellMarginsExporter.ToHashtable(cellMargins));
			result.Add((int)JSONTableCellProperty.Borders, TableCellBordersExporter.ToHashtable(borders));
			result.Add((int)JSONTableCellProperty.HideCellMark, hideCellMark);
			result.Add((int)JSONTableCellProperty.NoWrap, noWrap);
			result.Add((int)JSONTableCellProperty.FitText, fitText);
			result.Add((int)JSONTableCellProperty.TextDirection, (int)textDirection);
			result.Add((int)JSONTableCellProperty.VerticalAlignment, (int)verticalAlignment);
			result.Add((int)JSONTableCellProperty.BackgroundColor, backgroundColor.ToArgb());
			result.Add((int)JSONTableCellProperty.ForegroundColor, foregroundColor.ToArgb());
			result.Add((int)JSONTableCellProperty.Shading, (int)shadingPattern);
			result.Add((int)JSONTableCellProperty.UseValue, (int)useValue);
		}
		public override bool Equals(object obj) {
			var prop = obj as WebTableCellProperties;
			if(prop == null)
				return false;
			return WebTableProperties.EqualsCellMargins(cellMargins, prop.cellMargins) &&
				EqualsTableCellBorders(borders, prop.borders) &&
				hideCellMark == prop.hideCellMark &&
				noWrap == prop.noWrap &&
				fitText == prop.fitText &&
				textDirection == prop.textDirection &&
				verticalAlignment == prop.verticalAlignment &&
				backgroundColor == prop.backgroundColor &&
				foregroundColor == prop.foregroundColor &&
				shadingPattern == prop.shadingPattern &&
				useValue == prop.useValue;
		}
		public override int GetHashCode() {
			return GetTableCellBordersHashCode() ^
				GetCellMarginsHashCode() ^
				(hideCellMark ? 1 : 0) ^
				(noWrap ? 1 : 0) ^
				(fitText ? 1 : 0) ^
				(int)textDirection ^
				(int)verticalAlignment ^
				backgroundColor.GetHashCode() ^
				foregroundColor.GetHashCode() ^
				(int)shadingPattern ^
				(int)useValue;
		}
		bool EqualsTableCellBorders(TableCellBorders borders1, TableCellBorders borders2) {
			return WebTableProperties.EqualsBorders(borders1.BottomBorder, borders2.BottomBorder) &&
				WebTableProperties.EqualsBorders(borders1.LeftBorder, borders2.LeftBorder) &&
				WebTableProperties.EqualsBorders(borders1.RightBorder, borders2.RightBorder) &&
				WebTableProperties.EqualsBorders(borders1.TopBorder, borders2.TopBorder) &&
				WebTableProperties.EqualsBorders(borders1.TopLeftDiagonalBorder, borders2.TopLeftDiagonalBorder) &&
				WebTableProperties.EqualsBorders(borders1.TopRightDiagonalBorder, borders2.TopRightDiagonalBorder);
		}
		int GetTableCellBordersHashCode() {
			return WebTableProperties.GetBorderHashCode(borders.BottomBorder) ^
				WebTableProperties.GetBorderHashCode(borders.LeftBorder) ^
				WebTableProperties.GetBorderHashCode(borders.RightBorder) ^
				WebTableProperties.GetBorderHashCode(borders.TopBorder) ^
				WebTableProperties.GetBorderHashCode(borders.TopLeftDiagonalBorder) ^
				WebTableProperties.GetBorderHashCode(borders.TopRightDiagonalBorder);
		}
		int GetCellMarginsHashCode() {
			return WebTableProperties.GetWidthUnitHashCode(cellMargins.Bottom) ^
				WebTableProperties.GetWidthUnitHashCode(cellMargins.Left) ^
				WebTableProperties.GetWidthUnitHashCode(cellMargins.Right) ^
				WebTableProperties.GetWidthUnitHashCode(cellMargins.Top);
		}
	}
	public class WebTableCellPropertiesCache : WebModelPropertiesCacheBase<WebTableCellProperties> {
	}
	public static class TableCellPropertiesExporter {
		public static void ConvertFromHashtable(Hashtable source, TableCellProperties props) {
			TableCellMarginsExporter.FromHashtable((Hashtable)source[((int)JSONTableCellProperty.CellMargins).ToString()], props.CellMargins);
			TableCellBordersExporter.FromHashtable((Hashtable)source[((int)JSONTableCellProperty.Borders).ToString()], props.Borders);
			props.HideCellMark = (bool)source[((int)JSONTableCellProperty.HideCellMark).ToString()];
			props.NoWrap = (bool)source[((int)JSONTableCellProperty.NoWrap).ToString()];
			props.FitText = (bool)source[((int)JSONTableCellProperty.FitText).ToString()];
			props.TextDirection = (TextDirection)source[((int)JSONTableCellProperty.TextDirection).ToString()];
			props.VerticalAlignment = (VerticalAlignment)source[((int)JSONTableCellProperty.VerticalAlignment).ToString()];
			props.BackgroundColor = System.Drawing.Color.FromArgb((int)source[((int)JSONTableCellProperty.BackgroundColor).ToString()]);
			props.ForegroundColor = System.Drawing.Color.FromArgb((int)source[((int)JSONTableCellProperty.ForegroundColor).ToString()]);
			props.ShadingPattern = (ShadingPattern)source[((int)JSONTableCellProperty.Shading).ToString()];
			props.UseValue = (TableCellPropertiesOptions.Mask)source[((int)JSONTableCellProperty.UseValue).ToString()];
		}
	}
}
