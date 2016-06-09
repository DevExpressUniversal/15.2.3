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
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office;
using DevExpress.Office.Model;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region StyleSheet
	public class StyleSheet : ISupportsCopyFrom<StyleSheet> {
		#region Fields
		readonly DocumentModel workbook;
		CellStyleCollection cellStyles;
		Palette palette;
		List<Color> customColors;
		int lastCustomStyleNameId;
		int lastCustomTableStyleNameId;
		TableStyleCollection tableStyles;
		int defaultCellFormatIndex;
		#endregion
		public StyleSheet(DocumentModel workbook) {
			this.workbook = workbook;
			this.cellStyles = new CellStyleCollection();
			this.palette = new Palette();
			this.customColors = new List<Color>();
			this.tableStyles = new TableStyleCollection(workbook);
			InitializeDefaultStyle();
		}
		#region Properties
		public DocumentModel Workbook { get { return workbook; } }
		public CellStyleCollection CellStyles { get { return this.cellStyles; } }
		public NumberFormatCollection NumberFormats { get { return workbook.Cache.NumberFormatCache; } }
		public Palette Palette { get { return this.palette; } }
		public List<Color> CustomColors { get { return this.customColors; } }
		public TableStyleCollection TableStyles { get { return tableStyles; } }
		public int DefaultCellFormatIndex { get { return defaultCellFormatIndex; } set { defaultCellFormatIndex = value; } }
		public FormatBase DefaultCellFormat { get { return workbook.Cache.CellFormatCache[defaultCellFormatIndex]; } }
		public NumberFormat DefaultNumberFormat { get { return DefaultCellFormat.NumberFormatInfo; } }
		#endregion
		public void Clear() {
			cellStyles.Clear();
			palette.Reset();
			customColors.Clear();
			tableStyles.ClearAllCore();
		}
		public void Initialize() {
			cellStyles.BeginUpdate();
			try {
				Clear();
				InitializeDefaultStyle();
			}
			finally {
				cellStyles.EndUpdate();
			}
		}
		void InitializeDefaultCellFormatIndex() {
			defaultCellFormatIndex = CellFormatCache.DefaultItemIndex;
		}
		void InitializeDefaultStyle() {
			AddDefaultCellStyle();
			InitializeDefaultCellFormatIndex();
			tableStyles.AddDefaultStyle();
		}
		void AddDefaultCellStyle() {
			CellStyles.Add(new BuiltInCellStyle(workbook, 0));
		}
		public string GenerateCustomStyleName() {
			return "Style " + ++lastCustomStyleNameId;
		}
		public string GenerateCustomTableStyleName() {
			return "Table Style " + ++lastCustomTableStyleNameId;
		}
		public void CopyFrom(StyleSheet source) {
			if (!Object.ReferenceEquals(Workbook, source.DefaultCellFormat.DocumentModel)) {
				CellFormat defaultFormat = new CellFormat(Workbook);
				defaultFormat.CopyFrom(source.DefaultCellFormat);
				defaultCellFormatIndex = Workbook.Cache.CellFormatCache.GetItemIndex(defaultFormat);
			}
			CellStyles.CopyFrom(Workbook, source.CellStyles);
			Palette.CopyFrom(source.Palette);
			CustomColors.Clear();
			CustomColors.AddRange(source.CustomColors);
			lastCustomStyleNameId = 0;
			lastCustomTableStyleNameId = 0;
			tableStyles.CopyFrom(Workbook, source.TableStyles);
		}
	}
	#endregion
}
