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

using System.Collections;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Web.ASPxRichEdit.Export {
	public class WebSectionProperties : IHashtableProvider {
		float marginLeft;
		float marginTop;
		float marginRight;
		float marginBottom;
		float headerOffset;
		float footerOffset;
		int columnCount;
		int space;
		bool equalWidthColumns;
		ColumnInfoCollection columns;
		int pageWidth;
		int pageHeight;
		SectionStartType startType;
		bool landscape;
		bool differentFirstPage;
		public WebSectionProperties(Section section) {
			this.marginLeft = section.Margins.Left;
			this.marginTop = section.Margins.Top;
			this.marginBottom = section.Margins.Bottom;
			this.marginRight = section.Margins.Right;
			this.headerOffset = section.Margins.HeaderOffset;
			this.footerOffset = section.Margins.FooterOffset;
			this.columnCount = section.Columns.ColumnCount;
			this.space = section.Columns.Space;
			this.equalWidthColumns = section.Columns.EqualWidthColumns;
			this.columns = section.Columns.GetColumns();
			this.startType = section.GeneralSettings.StartType;
			this.landscape = section.Page.Landscape;
			this.pageWidth = section.Page.Width;
			this.pageHeight = section.Page.Height;
			this.differentFirstPage = section.GeneralSettings.DifferentFirstPage;
		}
		public void FillHashtable(Hashtable result) {
			result[((int)JSONSectionProperty.MarginLeft).ToString()] = marginLeft;
			result[((int)JSONSectionProperty.MarginTop).ToString()] = marginTop;
			result[((int)JSONSectionProperty.MarginBottom).ToString()] = marginBottom;
			result[((int)JSONSectionProperty.MarginRight).ToString()] = marginRight;
			result[((int)JSONSectionProperty.ColumnCount).ToString()] = columnCount;
			result[((int)JSONSectionProperty.Space).ToString()] = space;
			result[((int)JSONSectionProperty.EqualWidthColumns).ToString()] = equalWidthColumns;
			result[((int)JSONSectionProperty.ColumnsInfo).ToString()] = CreateColumnsHashtable();
			result[((int)JSONSectionProperty.PageWidth).ToString()] = pageWidth;
			result[((int)JSONSectionProperty.PageHeight).ToString()] = pageHeight;
			result[((int)JSONSectionProperty.StartType).ToString()] = (int)startType;
			result[((int)JSONSectionProperty.Landscape).ToString()] = landscape;
			result[((int)JSONSectionProperty.DifferentFirstPage).ToString()] = differentFirstPage;
			result[((int)JSONSectionProperty.HeaderOffset).ToString()] = headerOffset;
			result[((int)JSONSectionProperty.FooterOffset).ToString()] = footerOffset;
		}
		ArrayList CreateColumnsHashtable() {
			var results = new ArrayList();
			foreach (var column in columns) {
				results.Add(new Hashtable() {
					{ "width", column.Width },
					{ "space", column.Space }
				});
			}
			return results;
		}
		public override bool Equals(object obj) {
			var prop = obj as WebSectionProperties;
			if (prop == null)
				return false;
			return
				prop.columnCount == columnCount &&
				EqualsColumns(prop.columns) &&
				prop.equalWidthColumns == equalWidthColumns &&
				prop.marginBottom == marginBottom &&
				prop.marginLeft == marginLeft &&
				prop.marginRight == marginRight &&
				prop.marginTop == marginTop &&
				prop.pageHeight == pageHeight &&
				prop.pageWidth == pageWidth &&
				prop.space == space &&
				prop.startType == startType &&
				prop.landscape == landscape &&
				prop.differentFirstPage == differentFirstPage &&
				prop.headerOffset == headerOffset &&
				prop.footerOffset == footerOffset;
		}
		bool EqualsColumns(ColumnInfoCollection columns) {
			if (columns.Count != this.columns.Count)
				return false;
			for (int i = 0; i < columns.Count; i++) {
				if (columns[i].Width != this.columns[i].Width || columns[i].Space != this.columns[i].Space)
					return false;
			}
			return true;
		}
		public override int GetHashCode() {
			return
				marginLeft.GetHashCode() ^
				marginBottom.GetHashCode() ^
				marginRight.GetHashCode() ^
				marginTop.GetHashCode() ^
				columnCount ^
				space ^
				(equalWidthColumns ? 1 : 0) ^
				GetColumnsHashCode() ^
				pageWidth ^
				pageHeight ^
				(int)startType ^
				(landscape ? 1 : 0) ^
				(differentFirstPage ? 1 : 0) ^
				headerOffset.GetHashCode() ^
				footerOffset.GetHashCode();
		}
		int GetColumnsHashCode() {
			var result = 0;
			foreach (var column in columns) {
				result ^= column.Space ^ column.Width;
			}
			return result;
		}
	}
	public class WebSectionPropertiesExporter {
		public void RestoreInfo(Hashtable source, Section section) {
			section.Margins.Left = (int)source[((int)JSONSectionProperty.MarginLeft).ToString()];
			section.Margins.Top = (int)source[((int)JSONSectionProperty.MarginTop).ToString()];
			section.Margins.Right = (int)source[((int)JSONSectionProperty.MarginRight).ToString()];
			section.Margins.Bottom = (int)source[((int)JSONSectionProperty.MarginBottom).ToString()];
			section.Margins.HeaderOffset = (int)source[((int)JSONSectionProperty.HeaderOffset).ToString()];
			section.Margins.FooterOffset = (int)source[((int)JSONSectionProperty.FooterOffset).ToString()];
			section.Columns.ColumnCount = (int)source[((int)JSONSectionProperty.ColumnCount).ToString()];
			section.Columns.Space = (int)source[((int)JSONSectionProperty.Space).ToString()];
			section.Columns.EqualWidthColumns = (bool)source[((int)JSONSectionProperty.EqualWidthColumns).ToString()];
			ArrayList columnsInfo = (ArrayList)source[((int)JSONSectionProperty.ColumnsInfo).ToString()];
			ColumnInfoCollection columns = new ColumnInfoCollection();
			for (int i = 0; i < columnsInfo.Count; i++) {
				Hashtable ht = (Hashtable)columnsInfo[i];
				ColumnInfo info = new ColumnInfo();
				info.Space = (int)ht["space"];
				info.Width = (int)ht["width"];
				columns.Add(info);
			}
			section.Columns.SetColumns(columns);
			section.Page.Width = (int)source[((int)JSONSectionProperty.PageWidth).ToString()];
			section.Page.Height = (int)source[((int)JSONSectionProperty.PageHeight).ToString()];
			section.Page.Landscape = (bool)source[((int)JSONSectionProperty.Landscape).ToString()];
			section.GeneralSettings.StartType = (SectionStartType)source[((int)JSONSectionProperty.StartType).ToString()];
			section.GeneralSettings.DifferentFirstPage = (bool)source[((int)JSONSectionProperty.DifferentFirstPage).ToString()];
		}
	}
	public class WebSectionPropertiesCache : WebModelPropertiesCacheBase<WebSectionProperties> {
	}
}
