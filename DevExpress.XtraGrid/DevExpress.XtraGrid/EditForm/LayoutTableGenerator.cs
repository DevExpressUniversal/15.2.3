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
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
namespace DevExpress.XtraGrid.EditForm.Layout {
	public class LayoutTableControlGenerator : LayoutControlGeneratorCore {
		public LayoutTableControlGenerator(EditFormOwner owner) : base(owner) { }
		public override Control Generate(EditFormOwner owner) {
			List<List<GridViewEditFormLayoutItem>> layout = GridViewEditFormLayout.CreateLayout(owner);
			TableLayoutPanel panel = new TableLayoutPanel();
			int maxColumnCount = 0, maxRowCount = 0;
			panel.ColumnCount = owner.EditFormColumnCount * 2;
			int rowCount = 0;
			foreach(var level in layout) {
				int col = 0;
				int maxRowSpan = 0;
				foreach(var item in level) {
					int cCount = level.Sum(q => q.ColSpan);
					while(panel.ColumnStyles.Count < cCount) panel.ColumnStyles.Add(new ColumnStyle() { SizeType = SizeType.AutoSize });
					if(item.Type == GridViewEditFormLayoutItemType.Empty) {
						var cell = CreateEmptyControl();
						if(cell != null) {
							panel.Controls.Add(cell);
							panel.SetCellPosition(cell, new TableLayoutPanelCellPosition(col, rowCount));
							panel.SetColumnSpan(cell, item.ColSpan);
						}
					}
					if(item.Type == GridViewEditFormLayoutItemType.Caption) {
						var cell = CreateCaption(item);
						cell.Padding = new Padding(item.ColIndex > 0 ? 10 : 0, 0, 0, 0);
						panel.Controls.Add(cell);
						panel.SetCellPosition(cell, new TableLayoutPanelCellPosition(item.ColIndex, rowCount));
						panel.SetColumnSpan(cell, 1);
						panel.SetRowSpan(cell, 1);
					}
					if(item.Type == GridViewEditFormLayoutItemType.Editor) {
						CreateEditorItem(panel, item, rowCount);
						maxRowSpan = Math.Max(maxRowSpan, item.RowSpan);
					}
					col += item.ColSpan;
					maxColumnCount = Math.Max(col, maxColumnCount);
				}
				rowCount += 1;
				maxRowCount = Math.Max(maxRowCount, rowCount + maxRowSpan - 1);
			}
			panel.RowCount = maxRowCount + 1;
			panel.ColumnCount = GetColumnCount(layout);
			return panel;
		}
		void CreateEditorItem(TableLayoutPanel panel, GridViewEditFormLayoutItem item, int rowCount) {
			Control cell = null;
			if(item.CaptionLocation == EditFormColumnCaptionLocation.Top) {
				cell = CreateTopCaptionItem(item);
			}
			else {
				cell = CreateEditor(item);
				cell.Height = GetMinRowHeight() * item.RowSpan;
			}
			panel.Controls.Add(cell);
			panel.SetCellPosition(cell, new TableLayoutPanelCellPosition(item.ColIndex, rowCount));
			panel.SetColumnSpan(cell, item.ColSpan);
			panel.SetRowSpan(cell, item.RowSpan);
			int widthColumn = item.ColIndex;
			if(item.ColSpan > 1) widthColumn = widthColumn + (widthColumn % 2 == 0 ? 1 : 0);
			panel.ColumnStyles[widthColumn].SizeType = SizeType.Percent;
			if(panel.ColumnStyles[widthColumn].Width == 0)
				panel.ColumnStyles[widthColumn].Width = (float)item.WidthPercent;
			else
				panel.ColumnStyles[widthColumn].Width = Math.Min((float)item.WidthPercent, panel.ColumnStyles[widthColumn].Width);
		}
		int GetColumnCount(List<List<GridViewEditFormLayoutItem>> layout) {
			var totalCellCount = 1;
			if(layout.Count > 0) {
				var firstLevel = layout[0];
				totalCellCount = firstLevel.Sum(i => {
					if(i.Type == GridViewEditFormLayoutItemType.Editor || i.Type == GridViewEditFormLayoutItemType.Empty)
						return i.ColSpan;
					return 1;
				});
			}
			return totalCellCount;
		}
		Control CreateTopCaptionItem(GridViewEditFormLayoutItem item) {
			TableLayoutPanel cellPanel = new TableLayoutPanel();
			cellPanel.Height = item.RowSpan * GetMinRowHeight() + (item.RowSpan - 1) * (3*2); 
			cellPanel.Dock = DockStyle.Fill;
			cellPanel.Margin = new Padding(0);
			cellPanel.Padding = new Padding(0);
			cellPanel.ColumnCount = 1;
			cellPanel.RowCount = 2;
			var caption = CreateCaption(item);
			((LabelControl)caption).AutoSizeMode = LabelAutoSizeMode.None;
			caption.Dock = DockStyle.Fill;
			caption.Margin = new Padding(3, 0, 3, 0);
			caption.Height = GetMinRowHeight();
			cellPanel.Controls.Add(caption);
			cellPanel.SetCellPosition(caption, new TableLayoutPanelCellPosition(0, 0));
			var editor = CreateEditor(item);
			cellPanel.Controls.Add(editor);
			cellPanel.SetCellPosition(editor, new TableLayoutPanelCellPosition(0, 1));
			return cellPanel;
		}
		int minRowHeight = -1;
		int GetMinRowHeight() {
			if(minRowHeight < 0) {
				using(TextEdit tb = new TextEdit()) {
					tb.LookAndFeel.Assign(Owner.ElementsLookAndFeel);
					minRowHeight = tb.Height;
				}
			}
			return minRowHeight;
		}
		public override int GetHeight(EditFormOwner owner, Control panel) {
			TableLayoutPanel p = (TableLayoutPanel)panel;
			var res = p.GetRowHeights();
			return res.Length > 1 ? res.Take(res.Length - 1).Sum() : 16;
		}
	}
}
