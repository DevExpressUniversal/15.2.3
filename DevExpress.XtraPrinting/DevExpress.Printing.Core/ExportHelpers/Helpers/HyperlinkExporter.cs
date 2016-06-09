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
using DevExpress.Utils;
using DevExpress.XtraExport.Helpers;
using DevExpress.Export.Xl;
namespace DevExpress.Printing.ExportHelpers {
	internal class HyperlinkExporter {
		public static void SetHyperlink(string hyperlinkValue,string textFormat, IXlCell cell, IXlSheet sheet) {
			if(string.IsNullOrEmpty(hyperlinkValue)) return;
			if(Find(sheet.Hyperlinks, cell, hyperlinkValue)) return;
			XlHyperlink hyperlink = new XlHyperlink {
				Reference = new XlCellRange(new XlCellPosition(cell.ColumnIndex, cell.RowIndex))
			};
			GetHyperlinkCellText(cell, textFormat);
			SetData(cell, hyperlink, hyperlinkValue);
			FormatExport.SetHyperlinkFormat(cell.Formatting);
			sheet.Hyperlinks.Add(hyperlink);
		}
		static void GetHyperlinkCellText(IXlCell cell, string textFormat){
			if(!string.IsNullOrEmpty(textFormat)){
				if(!string.IsNullOrEmpty(cell.Formatting.NetFormatString)) {
					textFormat = cell.Formatting.NetFormatString;
				}
				var value = string.Format(textFormat, GetCellValue(cell));
				if(value != textFormat)
					cell.Value = XlVariantValue.FromObject(value);
			}
		}
		static object GetCellValue(IXlCell cell){
			switch(cell.Value.Type){
				case XlVariantValueType.DateTime:
					return cell.Value.DateTimeValue;
				case XlVariantValueType.Numeric:
					return cell.Value.NumericValue;
				case XlVariantValueType.Text:
					return cell.Value.TextValue;
				case XlVariantValueType.Boolean:
					return cell.Value.BooleanValue;
				default:
					return string.Empty;
			}
		}
		public static bool CanExportHyperlink(bool isEventHandled, string hyperlink, ColumnEditTypes columnEditType, DefaultBoolean allowHyperLinks){
			if(isEventHandled) 
				return !string.IsNullOrEmpty(hyperlink);
			return allowHyperLinks != DefaultBoolean.False && columnEditType == ColumnEditTypes.Hyperlink && !string.IsNullOrEmpty(hyperlink);
		}
		static bool Find(IList<XlHyperlink> hyperlinks, IXlCell cell, string value) {
			bool isFind = false;
			foreach(XlHyperlink hyperlink in hyperlinks)
				if(IsCoincidence(hyperlink.Reference, cell)) {
					SetData(cell, hyperlink, value);
					isFind = true;
					break;
				}
			return isFind;
		}
		static void SetData(IXlCell cell, XlHyperlink hyperlink, string value) {
			hyperlink.DisplayText = value;
			hyperlink.TargetUri = value;
			hyperlink.Tooltip = value;
			if(cell.Value == XlVariantValue.Empty || cell.Value.TextValue == string.Empty) {
				cell.Value = XlVariantValue.FromObject(value);
			}
		}
		static bool IsCoincidence(XlCellRange range, IXlCell cell) {
			return range.TopLeft.Column == cell.ColumnIndex && range.TopLeft.Row == cell.RowIndex
				&& range.BottomRight.Column == cell.ColumnIndex && range.BottomRight.Row == cell.RowIndex;
		}
	}
}
