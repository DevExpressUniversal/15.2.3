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
namespace DevExpress.Snap.Core.Native {
	public static class StyleHelper {
		public static void ApplyTableCellStyle(SnapDocumentModel styleSourceModel, SnapDocumentModel result, string baseStyleName, int level) {
			string styleName = StyleHelper.GetCellStyleName(baseStyleName, level, styleSourceModel);
			TableCellStyle tableStyleList1 = styleSourceModel.TableCellStyles.GetStyleByName(styleName);
			if (tableStyleList1 != null) {
				int newStyleIndex =  tableStyleList1.Copy(result);
				TableCellStyle newStyle = result.TableCellStyles[newStyleIndex];
				foreach (Table table in result.MainPieceTable.Tables) {
					table.Rows.ForEach(row => row.Cells.ForEach(cell => ApplyTableCellStyleCore(newStyle.StyleName, cell)));
				}
			}
		}
		public static void ApplyTableCellStyleCore(string styleName, TableCell cell) {
			cell.StyleIndex = cell.DocumentModel.TableCellStyles.GetStyleIndexByName(styleName);
		}
		public static void ApplyTableCellStyleDirect(string styleName, TableCell cell) {
			cell.SetTableCellStyleIndexCore(cell.DocumentModel.TableCellStyles.GetStyleIndexByName(styleName));
		}
		public static string GetStyleName(string baseStyleName, int level, DocumentModel styleSourceModel) {
			while (level >= 0) {
				string styleName = baseStyleName + level;
				TableStyle style = styleSourceModel.TableStyles.GetStyleByName(styleName);
				if (style != null)
					return styleName;
				level--;
			}
			return string.Empty;
		}
		public static string GetCellStyleName(string baseStyleName, int level, DocumentModel styleSourceModel) { 
			while (level >= 0) {
					string styleName = "List" + level + "-" + baseStyleName;
					TableCellStyle style = styleSourceModel.TableCellStyles.GetStyleByName(styleName);
					if (style != null)
						return styleName;
				level--;
			}
			return string.Empty;
		}
		public static string GetGroupStyleName(string baseStyleName, int listLevel, int level, DocumentModel styleSourceModel) { 
			int initialListLevel = listLevel;
			while (level > 0) {
				while (listLevel > 0) {
					string styleName = "List" + listLevel + "-" + baseStyleName + level;
					TableCellStyle style = styleSourceModel.TableCellStyles.GetStyleByName(styleName);
					if (style != null)
						return styleName;
					listLevel--;
				}
				listLevel = initialListLevel;
				level--;
			}
			return string.Empty;
		}
	}
}
