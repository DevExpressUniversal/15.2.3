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
using DevExpress.Office.Drawing;
using DevExpress.Office.Layout;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Export.Xl;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.ComponentModel;
using System.Drawing;
namespace DevExpress.XtraSpreadsheet.Forms.Design {
	#region BorderLineStyleListBox
	[DXToolboxItem(false)]
	public class BorderLineStyleListBox : ImageListBoxControl {
		#region Fields
		DocumentModel documentModel;
		#endregion
		public BorderLineStyleListBox() {
			this.ColumnWidth = 70;
			this.MultiColumn = true;
		}
		protected internal void InitializeViewModel(FormatCellsViewModel viewModel) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			this.documentModel = viewModel.Control.InnerControl.DocumentModel;
		}
		public DocumentModel DocumentModel {
			get {
				return documentModel;
			}
			set {
				documentModel = value;
			}
		}
		protected override XtraEditors.ViewInfo.BaseStyleControlViewInfo CreateViewInfo() {
			return new BorderLineStyleImageListBoxViewInfo(this);
		}
	}
	#endregion
	public class BorderLineStyleImageListBoxViewInfo : ImageListBoxViewInfo {
		public BorderLineStyleImageListBoxViewInfo(BaseImageListBoxControl listBox)
			: base(listBox) {
		}
		public DocumentModel DocumentModel {
			get {
				if (OwnerControl == null)
					return null;
				BorderLineStyleListBox edit = OwnerControl as BorderLineStyleListBox;
				if (edit == null)
					return null;
				return edit.DocumentModel;
			}
		}
		protected override BaseListBoxItemPainter CreateItemPainter() {
			if (IsSkinnedHighlightingEnabled)
				return new BorderLineStyleListBoxSkinItemPainter(OwnerControl as BorderLineStyleListBox);
			return new BorderLineStyleListBoxItemPainter(OwnerControl as BorderLineStyleListBox);
		}
		protected override int CalcItemMinHeight() {
			int height = 0;
			return Math.Max(base.CalcItemMinHeight(), height);
		}
		protected override ListBoxItemObjectInfoArgs CreateListBoxItemInfoArgs() {
			return new BorderLineStyleValueListBoxItemObjectInfoArgs(this, null, Rectangle.Empty);
		}
	}
	public class BorderLineStyleValueListBoxItemObjectInfoArgs : ListBoxItemObjectInfoArgs {
		public BorderLineStyleValueListBoxItemObjectInfoArgs(BaseListBoxViewInfo viewInfo, GraphicsCache cache, Rectangle bounds)
			: base(viewInfo, cache, bounds) {
		}
		public override void AssignFromItemInfo(BaseListBoxViewInfo.ItemInfo item) {
			base.AssignFromItemInfo(item);
		}
	}
	public class BorderLineStyleListBoxSkinItemPainter : ListBoxSkinItemPainter, IPatternLinePaintingSupport {
		#region Fields
		Graphics graphics;
		BorderLine borderLine = null;
		BorderLineStyleListBox control;
		#endregion
		public BorderLineStyleListBoxSkinItemPainter(BorderLineStyleListBox control) {
			this.control = control;
		}
		protected override void DrawItemText(ListBoxItemObjectInfoArgs e) {
			BorderLineStyleImageListBoxViewInfo viewInfo = e.ViewInfo as BorderLineStyleImageListBoxViewInfo;
			DocumentLayoutUnitConverter converter = viewInfo.DocumentModel.LayoutUnitConverter;
			SpreadsheetHorizontalPatternLinePainter horizontalLinePainter = new SpreadsheetHorizontalPatternLinePainter(this, converter);
			Rectangle rectItem = e.TextRect;
			graphics = e.Graphics;
			int padding = 5;
			if (viewInfo == null || viewInfo.DocumentModel == null)
				return;
			borderLine = DocumentModel.DefaultBorderLineRepository.GetPatternLineByType(GetBorderLineStyle(e.ItemText));
			if ((int)GetBorderLineStyle(e.ItemText) < 7)
				borderLine.Draw(horizontalLinePainter, new Point(rectItem.X + padding, rectItem.Y + rectItem.Height / 2), new Point(rectItem.Width - padding, rectItem.Y + rectItem.Height / 2), control.ForeColor, BorderInfo.LinePixelThicknessTable[GetBorderLineStyle(e.ItemText)]);
			else
				borderLine.Draw(horizontalLinePainter, new Point(rectItem.X + padding, rectItem.Y + rectItem.Height / 2), new Point(rectItem.Width * 2 - padding, rectItem.Y + rectItem.Height / 2), control.ForeColor, BorderInfo.LinePixelThicknessTable[GetBorderLineStyle(e.ItemText)]);
		}
		public XlBorderLineStyle GetBorderLineStyle(string lineStyle) {
			return (XlBorderLineStyle)Enum.Parse(typeof(XlBorderLineStyle), lineStyle);
		}
		#region IPatternLinePaintingSupport Members
		public void DrawLine(Pen pen, float x1, float y1, float x2, float y2) {
			graphics.DrawLine(pen, x1, y1, x2, y2);
		}
		public void DrawLines(Pen pen, PointF[] points) {
			graphics.DrawLines(pen, points);
		}
		public Brush GetBrush(Color color) {
			return new SolidBrush(color);
		}
		public Pen GetPen(Color color, float thickness) {
			return new Pen(color, thickness);
		}
		public Pen GetPen(Color color) {
			return new Pen(color);
		}
		public void ReleaseBrush(Brush brush) {
			brush.Dispose();
		}
		public void ReleasePen(Pen pen) {
			pen.Dispose();
		}
		#endregion
	}
	public class BorderLineStyleListBoxItemPainter : ListBoxItemPainter, IPatternLinePaintingSupport {
		#region Fields
		Graphics graphics;
		BorderLine borderLine = null;
		BorderLineStyleListBox control;
		#endregion
		public BorderLineStyleListBoxItemPainter(BorderLineStyleListBox control) {
			this.control = control;
		}
		protected override void DrawItemText(ListBoxItemObjectInfoArgs e) {
			BorderLineStyleImageListBoxViewInfo viewInfo = e.ViewInfo as BorderLineStyleImageListBoxViewInfo;
			DocumentLayoutUnitConverter converter = viewInfo.DocumentModel.LayoutUnitConverter;
			SpreadsheetHorizontalPatternLinePainter horizontalLinePainter = new SpreadsheetHorizontalPatternLinePainter(this, converter);
			Rectangle rectItem = e.TextRect;
			graphics = e.Graphics;
			int padding = 5;
			if (viewInfo == null || viewInfo.DocumentModel == null)
				return;
			borderLine = DocumentModel.DefaultBorderLineRepository.GetPatternLineByType(GetBorderLineStyle(e.ItemText));
			if ((int)GetBorderLineStyle(e.ItemText) < 7)
				borderLine.Draw(horizontalLinePainter, new Point(rectItem.X + padding, rectItem.Y + rectItem.Height / 2), new Point(rectItem.Width - padding, rectItem.Y + rectItem.Height / 2), control.ForeColor, BorderInfo.LinePixelThicknessTable[GetBorderLineStyle(e.ItemText)]);
			else
				borderLine.Draw(horizontalLinePainter, new Point(rectItem.X + padding, rectItem.Y + rectItem.Height / 2), new Point(rectItem.Width * 2 - padding, rectItem.Y + rectItem.Height / 2), control.ForeColor, BorderInfo.LinePixelThicknessTable[GetBorderLineStyle(e.ItemText)]);
		}
		public XlBorderLineStyle GetBorderLineStyle(string lineStyle) {
			return (XlBorderLineStyle)Enum.Parse(typeof(XlBorderLineStyle), lineStyle);
		}
		#region IPatternLinePaintingSupport Members
		public void DrawLine(Pen pen, float x1, float y1, float x2, float y2) {
			graphics.DrawLine(pen, x1, y1, x2, y2);
		}
		public void DrawLines(Pen pen, PointF[] points) {
			graphics.DrawLines(pen, points);
		}
		public Brush GetBrush(Color color) {
			return new SolidBrush(color);
		}
		public Pen GetPen(Color color, float thickness) {
			return new Pen(color, thickness);
		}
		public Pen GetPen(Color color) {
			return new Pen(color);
		}
		public void ReleaseBrush(Brush brush) {
			brush.Dispose();
		}
		public void ReleasePen(Pen pen) {
			pen.Dispose();
		}
		#endregion
	}
}
