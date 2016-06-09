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

using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing.Native {
	class TableViewInfo : IPrintableObject {
		const int RowLastColumnPadding = 1;
		List<RowPrintViewInfo> rows;
		Size minSize;
		Rectangle bounds;
		bool applyRowLastColumnPadding;
		public TableViewInfo() {
			rows = new List<RowPrintViewInfo>();
		}
		public Size MinSize { get { return minSize; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		internal List<RowPrintViewInfo> Rows { get { return rows; } }
		protected internal bool ApplyRowLastColumnPadding { get { return applyRowLastColumnPadding; } set { applyRowLastColumnPadding = value; } }
		public virtual void AddRow(RowPrintViewInfo row) {
			if (row == null)
				Exceptions.ThrowArgumentNullException("row");
			rows.Add(row);
		}
		public virtual void Print(GraphicsInfoArgs graphicsInfoArgs) {
			int count = rows.Count;
			for (int i = 0; i < count; i++)
				rows[i].Print(graphicsInfoArgs);
		}
		public virtual Size Measure(GraphicsCache cache, int maxWidth) {
			int count = rows.Count;
			for (int i = 0; i < count; i++)
				rows[i].Measure(cache, maxWidth);
			int width = 0;
			int height = 0;
			for (int i = 0; i < count; i++) {
				width = Math.Max(width, rows[i].MinSize.Width);
				height += MinSize.Height;
			}
			minSize = new Size(width, height);
			return minSize;
		}
		public virtual void Arrange(Rectangle bounce) {
			int count = rows.Count;
			int currentVerticalPossition = 0;
			int currentHorisontalPossition = 0;
			List<int> alignColumnsWidth = CalculateAllignColumnsWidth(bounce);
			for (int i = 0; i < count; i++) {
				RowPrintViewInfo row = rows[i];
				currentHorisontalPossition = 0;
				Point position;
				Size size;
				for (int j = 0; j < row.Count - 1; j++) {
					position = new Point(currentHorisontalPossition, currentVerticalPossition);
					size = new Size(alignColumnsWidth[j], row[j].MinSize.Height);
					currentHorisontalPossition += alignColumnsWidth[j];
					row[j].Bounds = new Rectangle(position, size);
				}
				position = new Point(currentHorisontalPossition, currentVerticalPossition);
				size = new Size(bounce.Width - currentHorisontalPossition, row[row.Count - 1].MinSize.Height);
				if (ApplyRowLastColumnPadding)  
					size.Width -= RowLastColumnPadding;
				row[row.Count - 1].Bounds = new Rectangle(position, size);
				currentVerticalPossition += row.MinSize.Height;
			}
		}
		public int[] CalculateColumnsWidth(RowPrintViewInfo row, Rectangle bounce) {
			int count = row.Count;
			int[] columnsWidth = new int[count];
			int columnsWidthExceptLastColumns = 0;
			for (int i = 0; i < count - 1; i++) {
				columnsWidth[i] = row[i].MinSize.Width;
				columnsWidthExceptLastColumns += columnsWidth[i];
			}
			columnsWidth[count - 1] = bounce.Width - columnsWidthExceptLastColumns;
			return columnsWidth;
		}
		List<int> CalculateAllignColumnsWidth(Rectangle bounce) {
			List<int> alignColumnsWidth = new List<int>();
			int count = rows.Count;
			for (int i = 0; i < count; i++) {
				int[] columnsWidth = CalculateColumnsWidth(rows[i], bounce);
				for (int j = alignColumnsWidth.Count; j < columnsWidth.Length; j++) {
					alignColumnsWidth.Add(0);
				}
				for (int j = 0; j < columnsWidth.Length - 1; j++) {
					alignColumnsWidth[j] = Math.Max(alignColumnsWidth[j], columnsWidth[j]);
				}
			}
			return alignColumnsWidth;
		}
	}
}
