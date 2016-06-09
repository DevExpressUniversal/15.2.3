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

using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Export {
	public enum JSONTableProperty {
		CellMargins = 0,
		CellSpacing = 1,
		Indent = 2,
		PreferredWidth = 3,
		Borders = 4,
		TableStyleColBandSize = 5,
		TableStyleRowBandSize = 6,
		IsTableOverlap = 7,
		AvoidDoubleBorders = 8,
		LayoutType = 9,
		TableLookTypes = 10,
		BackgroundColor = 11,
		TableRowAlignment = 12,
		BottomFromText = 13,
		LeftFromText = 14,
		TopFromText = 15,
		RightFromText = 16,
		TableHorizontalPosition = 17,
		TableVerticalPosition = 18,
		HorizontalAlignMode = 19,
		VerticalAlignMode = 20,
		HorizontalAnchorType = 21,
		VerticalAnchorType = 22,
		TextWrapping = 23,
		UseValue = 24
	}
	public class WebTableProperties : IHashtableProvider {
		CellMargins cellMargins;
		CellSpacing cellSpacing;
		TableIndent tableIndent;
		TableBorders tableBorders;
		int tableStyleColBandSize;
		int tableStyleRowBandSize;
		bool isTableOverlap;
		bool avoidDoubleBorders;
		TableLayoutType tableLayout;
		Color backgroundColor;
		TableRowAlignment tableRowAlignment;
		int bottomFromText;
		int leftFromText;
		int topFromText;
		int rightFromText;
		int tableHorizontalPosition;
		int tableVerticalPosition;
		HorizontalAlignMode horizontalAlign;
		VerticalAlignMode verticalAlign;
		HorizontalAnchorTypes horizontalAnchor;
		VerticalAnchorTypes verticalAnchor;
		TextWrapping textWrapping;
		TablePropertiesOptions.Mask useValue;
		public WebTableProperties(TableProperties properties) {
			cellMargins = properties.CellMargins;
			cellSpacing = properties.CellSpacing;
			tableIndent = properties.TableIndent;
			tableBorders = properties.Borders;
			tableStyleColBandSize = properties.TableStyleColBandSize;
			tableStyleRowBandSize = properties.TableStyleRowBandSize;
			isTableOverlap = properties.IsTableOverlap;
			avoidDoubleBorders = properties.AvoidDoubleBorders;
			tableLayout = properties.TableLayout;
			backgroundColor = properties.BackgroundColor;
			tableRowAlignment = properties.TableAlignment;
			bottomFromText = properties.FloatingPosition.BottomFromText;
			leftFromText = properties.FloatingPosition.LeftFromText;
			topFromText = properties.FloatingPosition.TopFromText;
			rightFromText = properties.FloatingPosition.RightFromText;
			tableHorizontalPosition = properties.FloatingPosition.TableHorizontalPosition;
			tableVerticalPosition = properties.FloatingPosition.TableVerticalPosition;
			horizontalAlign = properties.FloatingPosition.HorizontalAlign;
			verticalAlign = properties.FloatingPosition.VerticalAlign;
			horizontalAnchor = properties.FloatingPosition.HorizontalAnchor;
			verticalAnchor = properties.FloatingPosition.VerticalAnchor;
			textWrapping = properties.FloatingPosition.TextWrapping;
			useValue = properties.UseValue;
		}
		public Hashtable ToHashtable() {
			var result = new Hashtable();
			FillHashtable(result);
			return result;
		}
		public void FillHashtable(Hashtable result) {
			result.Add((int)JSONTableProperty.CellMargins, TableCellMarginsExporter.ToHashtable(cellMargins));
			result.Add((int)JSONTableProperty.CellSpacing, WidthUnitExporter.ToHashtable(cellSpacing));
			result.Add((int)JSONTableProperty.Indent, WidthUnitExporter.ToHashtable(tableIndent));
			result.Add((int)JSONTableProperty.Borders, TableBordersExporter.ToHashtable(tableBorders));
			result.Add((int)JSONTableProperty.TableStyleColBandSize, tableStyleColBandSize);
			result.Add((int)JSONTableProperty.TableStyleRowBandSize, tableStyleRowBandSize);
			result.Add((int)JSONTableProperty.IsTableOverlap, isTableOverlap);
			result.Add((int)JSONTableProperty.AvoidDoubleBorders, avoidDoubleBorders);
			result.Add((int)JSONTableProperty.LayoutType, (int)tableLayout);
			result.Add((int)JSONTableProperty.BackgroundColor, backgroundColor.ToArgb());
			result.Add((int)JSONTableProperty.TableRowAlignment, (int)tableRowAlignment);
			result.Add((int)JSONTableProperty.BottomFromText, bottomFromText);
			result.Add((int)JSONTableProperty.LeftFromText, leftFromText);
			result.Add((int)JSONTableProperty.TopFromText, topFromText);
			result.Add((int)JSONTableProperty.RightFromText, rightFromText);
			result.Add((int)JSONTableProperty.TableHorizontalPosition, tableHorizontalPosition);
			result.Add((int)JSONTableProperty.TableVerticalPosition, tableVerticalPosition);
			result.Add((int)JSONTableProperty.HorizontalAlignMode, (int)horizontalAlign);
			result.Add((int)JSONTableProperty.VerticalAlignMode, (int)verticalAlign);
			result.Add((int)JSONTableProperty.HorizontalAnchorType, (int)horizontalAnchor);
			result.Add((int)JSONTableProperty.VerticalAnchorType, (int)verticalAnchor);
			result.Add((int)JSONTableProperty.TextWrapping, (int)textWrapping);
			result.Add((int)JSONTableProperty.UseValue, (int)useValue);
		}
		public override int GetHashCode() {
			return (avoidDoubleBorders ? 1 : 0) ^
				backgroundColor.GetHashCode() ^
				bottomFromText ^
				GetCellMarginsHashCode() ^
				GetWidthUnitHashCode(cellSpacing) ^
				(int)horizontalAlign ^
				(int)horizontalAnchor ^
				(isTableOverlap ? 1 : 0) ^
				leftFromText ^
				rightFromText ^
				GetBordersHashCode() ^
				tableHorizontalPosition ^
				GetWidthUnitHashCode(tableIndent) ^
				(int)tableLayout ^
				(int)tableRowAlignment ^
				tableStyleColBandSize ^
				tableStyleRowBandSize ^
				tableVerticalPosition ^
				(int)textWrapping ^
				topFromText ^
				(int)useValue ^
				(int)verticalAlign ^
				(int)verticalAnchor;
		}
		public override bool Equals(object obj) {
			var prop = obj as WebTableProperties;
			if(prop == null)
				return false;
			return prop.avoidDoubleBorders == avoidDoubleBorders &&
				prop.backgroundColor == backgroundColor &&
				prop.bottomFromText == bottomFromText &&
				EqualsCellMargins(prop.cellMargins, cellMargins) &&
				EqualsWidthUnit(prop.cellSpacing, cellSpacing) &&
				prop.horizontalAlign == horizontalAlign &&
				prop.horizontalAnchor == horizontalAnchor &&
				prop.isTableOverlap == isTableOverlap &&
				prop.leftFromText == leftFromText &&
				prop.rightFromText == rightFromText &&
				EqualsTableBorders(prop.tableBorders, tableBorders) &&
				prop.tableHorizontalPosition == tableHorizontalPosition &&
				EqualsWidthUnit(prop.tableIndent, tableIndent) &&
				prop.tableLayout == tableLayout &&
				prop.tableRowAlignment == tableRowAlignment &&
				prop.tableStyleColBandSize == tableStyleColBandSize &&
				prop.tableStyleRowBandSize == tableStyleRowBandSize &&
				prop.tableVerticalPosition == tableVerticalPosition &&
				prop.textWrapping == textWrapping &&
				prop.topFromText == topFromText &&
				prop.useValue == useValue &&
				prop.verticalAlign == verticalAlign &&
				prop.verticalAnchor == verticalAnchor;
		}
		bool EqualsTableBorders(TableBorders borders1, TableBorders borders2) {
			return EqualsBorders(borders1.BottomBorder, borders2.BottomBorder) && EqualsBorders(borders1.InsideHorizontalBorder, borders2.InsideHorizontalBorder) &&
				EqualsBorders(borders1.InsideVerticalBorder, borders2.InsideVerticalBorder) && EqualsBorders(borders1.LeftBorder, borders2.LeftBorder) &&
				EqualsBorders(borders1.RightBorder, borders2.RightBorder) && EqualsBorders(borders1.TopBorder, borders2.TopBorder);
		}
		internal static bool EqualsCellMargins(CellMargins cellMargins1, CellMargins cellMargins2) {
			return EqualsWidthUnit(cellMargins1.Bottom, cellMargins2.Bottom) &&
				EqualsWidthUnit(cellMargins1.Left, cellMargins2.Left) &&
				EqualsWidthUnit(cellMargins1.Right, cellMargins2.Right) &&
				EqualsWidthUnit(cellMargins1.Top, cellMargins2.Top);
		}
		internal static bool EqualsBorders(DevExpress.XtraRichEdit.Model.BorderBase border1, DevExpress.XtraRichEdit.Model.BorderBase border2) {
			return border1.Color == border2.Color &&
				border1.Frame == border2.Frame &&
				border1.Offset == border2.Offset &&
				border1.Shadow == border2.Shadow &&
				border1.Style == border2.Style &&
				border1.Width == border2.Width;
		}
		internal static bool EqualsWidthUnit(WidthUnit unit1, WidthUnit unit2) {
			return unit1.Type == unit2.Type &&
				unit1.Value == unit2.Value;
		}
		int GetCellMarginsHashCode() {
			return GetWidthUnitHashCode(cellMargins.Bottom) ^ GetWidthUnitHashCode(cellMargins.Left) ^ GetWidthUnitHashCode(cellMargins.Right) ^ GetWidthUnitHashCode(cellMargins.Top);
		}
		internal static int GetWidthUnitHashCode(WidthUnit width) {
			return (int)width.Type ^ width.Value;
		}
		int GetBordersHashCode() {
			return GetBorderHashCode(tableBorders.BottomBorder) ^
				GetBorderHashCode(tableBorders.InsideHorizontalBorder) ^
				GetBorderHashCode(tableBorders.InsideVerticalBorder) ^
				GetBorderHashCode(tableBorders.LeftBorder) ^
				GetBorderHashCode(tableBorders.RightBorder) ^
				GetBorderHashCode(tableBorders.TopBorder);
		}
		internal static int GetBorderHashCode(DevExpress.XtraRichEdit.Model.BorderBase border) {
			return border.Color.GetHashCode() ^
				(border.Frame ? 1 : 0) ^
				border.Offset ^
				(border.Shadow ? 1 : 0) ^
				(int)border.Style ^
				border.Width;
		}
	}
	public class WebTablePropertiesCache : WebModelPropertiesCacheBase<WebTableProperties> {
	}
	public static class TablePropertiesExporter {
		public static void ConvertFromHashtable(Hashtable source, TableProperties props) {
			TableCellMarginsExporter.FromHashtable((Hashtable)source[((int)JSONTableProperty.CellMargins).ToString()], props.CellMargins);
			WidthUnitExporter.FromHashtable((Hashtable)source[((int)JSONTableProperty.CellSpacing).ToString()], props.CellSpacing);
			WidthUnitExporter.FromHashtable((Hashtable)source[((int)JSONTableProperty.Indent).ToString()], props.TableIndent);
			TableBordersExporter.FromHashtable((Hashtable)source[((int)JSONTableProperty.Borders).ToString()], props.Borders);
			props.GeneralSettings.TableStyleColumnBandSize = (int)source[((int)JSONTableProperty.TableStyleColBandSize).ToString()];
			props.GeneralSettings.TableStyleRowBandSize = (int)source[((int)JSONTableProperty.TableStyleRowBandSize).ToString()];
			props.GeneralSettings.IsTableOverlap = (bool)source[((int)JSONTableProperty.IsTableOverlap).ToString()];
			props.GeneralSettings.AvoidDoubleBorders = (bool)source[((int)JSONTableProperty.AvoidDoubleBorders).ToString()];
			props.GeneralSettings.TableLayout = (TableLayoutType)source[((int)JSONTableProperty.LayoutType).ToString()];
			props.GeneralSettings.BackgroundColor = Color.FromArgb((int)source[((int)JSONTableProperty.BackgroundColor).ToString()]);
			props.GeneralSettings.TableAlignment = (TableRowAlignment)source[((int)JSONTableProperty.TableRowAlignment).ToString()];
			props.FloatingPosition.BottomFromText = (int)source[((int)JSONTableProperty.BottomFromText).ToString()];
			props.FloatingPosition.LeftFromText = (int)source[((int)JSONTableProperty.LeftFromText).ToString()];
			props.FloatingPosition.TopFromText = (int)source[((int)JSONTableProperty.TopFromText).ToString()];
			props.FloatingPosition.RightFromText = (int)source[((int)JSONTableProperty.RightFromText).ToString()];
			props.FloatingPosition.TableHorizontalPosition = (int)source[((int)JSONTableProperty.TableHorizontalPosition).ToString()];
			props.FloatingPosition.TableVerticalPosition = (int)source[((int)JSONTableProperty.TableVerticalPosition).ToString()];
			props.FloatingPosition.HorizontalAlign = (HorizontalAlignMode)source[((int)JSONTableProperty.HorizontalAlignMode).ToString()];
			props.FloatingPosition.VerticalAlign = (VerticalAlignMode)source[((int)JSONTableProperty.VerticalAlignMode).ToString()];
			props.FloatingPosition.HorizontalAnchor = (HorizontalAnchorTypes)source[((int)JSONTableProperty.HorizontalAnchorType).ToString()];
			props.FloatingPosition.VerticalAnchor = (VerticalAnchorTypes)source[((int)JSONTableProperty.VerticalAnchorType).ToString()];
			props.FloatingPosition.TextWrapping = (TextWrapping)source[((int)JSONTableProperty.TextWrapping).ToString()];
			props.UseValue = (TablePropertiesOptions.Mask)source[((int)JSONTableProperty.UseValue).ToString()];
		}
	}
}
