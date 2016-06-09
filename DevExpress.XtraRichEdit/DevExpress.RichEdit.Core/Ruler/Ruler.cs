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
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraRichEdit.Ruler {
	#region SectionProperties
	public class SectionProperties {
		#region Field
		int pageWidth;
		int pageHeight;
		int leftMargin;
		int rightMargin;
		int topMargin;
		int bottomMargin;
		ColumnInfoCollection columnInfoCollection;
		bool equalWidthColumns;
		int columnCount;
		Section section;
		int space;
		#endregion
		public SectionProperties(Section section) {
			Guard.ArgumentNotNull(section, "section");
			this.section = section;
			this.pageWidth = section.Page.Width;
			this.pageHeight = section.Page.Height;
			this.leftMargin = section.Margins.Left;
			this.rightMargin = section.Margins.Right;
			this.TopMargin = section.Margins.Top;
			this.BottomMargin = section.Margins.Bottom;
			this.columnInfoCollection = section.Columns.GetColumns();
			this.equalWidthColumns = section.Columns.EqualWidthColumns;
			this.columnCount = section.Columns.ColumnCount;
			this.space = section.Columns.Space;
		}
		#region Properties
		public Section Section { get { return section; } }
		public ColumnInfoCollection ColumnInfoCollection { get { return columnInfoCollection; } }
		public bool EqualWidthColumns { get { return equalWidthColumns; } set { equalWidthColumns = value; } }
		public int ColumnCount { get { return columnCount; } }
		public int Space { get { return space; } set { space = value; } }
		public int PageWidth { get { return pageWidth; } set { pageWidth = value; } }
		public int PageHeight { get { return pageHeight; } set { pageHeight = value; } }
		public int LeftMargin { get { return leftMargin; } set { leftMargin = value; } }
		public int RightMargin { get { return rightMargin; } set { rightMargin = value; } }
		public int TopMargin { get { return topMargin; } set { topMargin = value; } }
		public int BottomMargin { get { return bottomMargin; } set { bottomMargin = value; } }
		#endregion
		protected internal void CopyToSection(Section section) {
			section.Margins.Left = this.LeftMargin;
			section.Margins.Right = this.RightMargin;
			section.Margins.Top = this.TopMargin;
			section.Margins.Bottom = this.BottomMargin;
			section.Columns.SetColumns(this.ColumnInfoCollection);
			section.Columns.ColumnCount = this.ColumnCount;
			section.Columns.EqualWidthColumns = this.EqualWidthColumns;
			section.Columns.Space = this.Space;
			section.Page.Width = this.PageWidth;
			section.Page.Height = this.PageHeight;
		}
	}
	#endregion
}
