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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Pdf;
using DevExpress.Pdf.Drawing;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.ViewInfo;
namespace DevExpress.XtraPdfViewer.Native {
	[DXToolboxItem(false)]
	public class PdfOutlineViewerTreeList : TreeList {
		class PdfOutlineViewerTreeListViewInfo : TreeListViewInfo {
			public PdfOutlineViewerTreeListViewInfo(TreeList treeList) : base(treeList) {
			}
			protected override void CalcRowIndentInfo(RowInfo rowInfo) {
				base.CalcRowIndentInfo(rowInfo);
				int heightMargin = (rowInfo.Appearance.FontHeight + additionalNodeHeight) / 2;;
				rowInfo.ButtonBounds.Y = rowInfo.Bounds.Y + heightMargin - rowInfo.ButtonBounds.Height / 2;
				IndentInfo indentInfo = rowInfo.IndentInfo;
				Rectangle indentBounds = indentInfo.Bounds;
				int indentHeight = indentBounds.Height;
				indentInfo.Bounds = new Rectangle(indentBounds.X, indentBounds.Y + heightMargin - indentHeight, indentBounds.Width, indentHeight * 2);
			}
		}
		const int additionalNodeHeight = 10;
		const int leftNodeIndent = 5;
		readonly GraphicsInfo graphicsInfo = new GraphicsInfo();
		PdfOutlineViewerSettings settings;
		TreeListColumn Column {
			get {
				TreeListColumnCollection columns = Columns;
				return columns.Count > 0 ? columns[0] : null;
			}
		}
		public PdfOutlineViewerTreeList() {
		}
		public void UpdateSettings(PdfOutlineViewerSettings settings) {
			this.settings = settings;
			SetTextSize(settings.TextSize);
			SetLongLinesWrapping(settings.WrapLongLines);
		}
		public void SetTextSize(PdfOutlineNodeTextSize textSize) {
			Appearance.Row.FontSizeDelta = (int)textSize;
		}
		public void SetLongLinesWrapping(bool wrapping) {
			TreeListColumn column = Column;
			if (column != null)
				if (wrapping) {
					OptionsView.AutoWidth = true;
					column.AppearanceCell.TextOptions.WordWrap = WordWrap.Wrap;
				}
				else {
					column.AppearanceCell.TextOptions.WordWrap = WordWrap.NoWrap;
					OptionsView.AutoWidth = false;
					column.BestFit();
				}
		}
		protected override TreeListNode CreateNode(int nodeId, TreeListNodes owner, object tag) {
			return new PdfOutlineViewerTreeListNode(nodeId, owner);
		}
		protected override TreeListViewInfo CreateViewInfo() {
			return new PdfOutlineViewerTreeListViewInfo(this);
		}
		protected override AppearanceObject RaiseGetCustomNodeCellStyle(GetCustomNodeCellStyleEventArgs e) {
			AppearanceObject appearance = base.RaiseGetCustomNodeCellStyle(e);
			PdfOutlineViewerNode outlineViewerNode = GetDataRecordByNode(e.Node) as PdfOutlineViewerNode;
			if (settings != null && outlineViewerNode != null && appearance != null) {
				appearance.BeginUpdate();
				try {
					if (settings.UseOutlinesForeColor) {
						PdfColor foreColor = outlineViewerNode.ForeColor;
						if (foreColor != null) {
							double[] components = foreColor.Components;
							appearance.ForeColor = Color.FromArgb(Convert.ToInt32(components[0] * 255), Convert.ToInt32(components[1] * 255), Convert.ToInt32(components[2] * 255));
						}
					}
					FontStyle style = FontStyle.Regular;
					if (outlineViewerNode.Bold)
						style |= FontStyle.Bold;
					if (outlineViewerNode.Italic)
						style |= FontStyle.Italic;
					appearance.Font = new Font(appearance.Font, style);
				}
				finally {
					appearance.EndUpdate();
				}
			}
			return appearance;
		}
		protected override void RaiseCalcNodeHeight(TreeListNode node, ref int nodeHeight) {
			base.RaiseCalcNodeHeight(node, ref nodeHeight);
			TreeListColumn column = Column;
			if (column != null) {
				TreeListViewInfo viewInfo = ViewInfo;
				CellInfo cellInfo = new CellInfo(new ColumnInfo(column), new RowInfo(viewInfo, node));
				viewInfo.UpdateRowCellPaintAppearance(cellInfo);
				graphicsInfo.AddGraphics(null);
				try {
					Size textSize = cellInfo.PaintAppearance.CalcTextSizeInt(graphicsInfo.Cache, cellInfo.Value.ToString(), 
						column.VisibleWidth - viewInfo.RC.LevelWidth * (OptionsView.ShowRoot ? (node.Level + 1) : node.Level) - leftNodeIndent);
					nodeHeight = textSize.Height + additionalNodeHeight;
				}
				finally {
					graphicsInfo.ReleaseGraphics();
				}
			}
		}
		protected override void RaiseCustomDrawNodeCell(CustomDrawNodeCellEventArgs e) {
			base.RaiseCustomDrawNodeCell(e);
			AppearanceObject appereance = e.Appearance;
			if (appereance != null) {
				appereance.BeginUpdate();
				try {
					Rectangle bounds = e.Bounds;
					appereance.DrawString(e.Cache, e.CellText, new Rectangle(bounds.X + leftNodeIndent, bounds.Y + additionalNodeHeight / 2, bounds.Width - leftNodeIndent, bounds.Height - additionalNodeHeight), 
						appereance.GetStringFormat(appereance.TextOptions));
				}
				finally {
					appereance.EndUpdate();
				}
				e.Handled = true;
			}
		}
	}
}
