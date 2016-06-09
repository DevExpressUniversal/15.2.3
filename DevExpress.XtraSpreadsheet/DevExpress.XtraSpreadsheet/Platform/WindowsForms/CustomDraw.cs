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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraSpreadsheet.Layout;
using System.ComponentModel;
namespace DevExpress.XtraSpreadsheet {
	#region CustomDrawCellEventHandler
	public delegate void CustomDrawCellEventHandler(object sender, CustomDrawCellEventArgs e);
	#endregion
	#region CustomDrawCellEventArgs
	public class CustomDrawCellEventArgs : CustomDrawCellEventArgsBase {
		readonly CellForegroundDisplayFormat displayFormat;
		internal CustomDrawCellEventArgs(CellForegroundDisplayFormat displayFormat, SpreadsheetControl control, Page page, ICellTextBox box, GraphicsCache cache, Action defaultDraw)
			: base(control, page, box, cache, defaultDraw) {
			Guard.ArgumentNotNull(displayFormat, "displayFormat");
			this.displayFormat = displayFormat;
		}
		public Font Font { get { return displayFormat.FontInfo.Font; } }
		public Color ForeColor { get { return displayFormat.ForeColor; } set { displayFormat.ForeColor = value; } }
		public string Text { get { return displayFormat.Text; } set { displayFormat.Text = value; } }
		protected override Rectangle CalculateBounds() {
			return displayFormat.Bounds;
		}
	}
	#endregion
	#region CustomDrawCellBackgroundEventHandler
	public delegate void CustomDrawCellBackgroundEventHandler(object sender, CustomDrawCellBackgroundEventArgs e);
	#endregion
	#region CustomDrawCellBackgroundEventArgs
	public class CustomDrawCellBackgroundEventArgs : CustomDrawCellEventArgsBase {
		readonly CellBackgroundDisplayFormat displayFormat;
		internal CustomDrawCellBackgroundEventArgs(CellBackgroundDisplayFormat displayFormat, SpreadsheetControl control, Page page, ICellTextBox box, GraphicsCache cache, Action defaultDraw)
			: base(control, page, box, cache, defaultDraw) {
			Guard.ArgumentNotNull(displayFormat, "displayFormat");
			this.displayFormat = displayFormat;
		}
		public Color BackColor { get { return displayFormat.BackColor; } set { displayFormat.BackColor = value; } }
		protected override Rectangle CalculateBounds() {
			return FillBounds;
		}
	}
	#endregion
	#region CustomDrawRowHeaderEventHandler
	public delegate void CustomDrawRowHeaderEventHandler(object sender, CustomDrawRowHeaderEventArgs e);
	#endregion
	#region CustomDrawRowHeaderEventArgs
	public class CustomDrawRowHeaderEventArgs : CustomDrawHeaderEventArgsBase {
		internal CustomDrawRowHeaderEventArgs(SpreadsheetControl control, HeaderTextBox box, AppearanceObject appearance, GraphicsCache cache, Action defaultDraw)
			: base(control, box, appearance, cache, defaultDraw) {
		}
		#region Properties
		public int RowIndex { get { return Box.ModelIndex; } }
		#endregion
	}
	#endregion
	#region CustomDrawColumnHeaderEventHandler
	public delegate void CustomDrawColumnHeaderEventHandler(object sender, CustomDrawColumnHeaderEventArgs e);
	#endregion
	#region CustomDrawColumnHeaderEventArgs
	public class CustomDrawColumnHeaderEventArgs : CustomDrawHeaderEventArgsBase {
		internal CustomDrawColumnHeaderEventArgs(SpreadsheetControl control, HeaderTextBox box, AppearanceObject appearance, GraphicsCache cache, Action defaultDraw)
			: base(control, box, appearance, cache, defaultDraw) {
		}
		#region Properties
		public int ColumnIndex { get { return Box.ModelIndex; } }
		#endregion
	}
	#endregion
	#region CustomDrawRowHeaderBackgroundEventHandler
	public delegate void CustomDrawRowHeaderBackgroundEventHandler(object sender, CustomDrawRowHeaderBackgroundEventArgs e);
	#endregion
	#region CustomDrawRowHeaderBackgroundEventArgs
	public class CustomDrawRowHeaderBackgroundEventArgs : CustomDrawHeaderEventArgsBase {
		internal CustomDrawRowHeaderBackgroundEventArgs(SpreadsheetControl control, HeaderTextBox box, AppearanceObject appearance, GraphicsCache cache, Action defaultDraw)
			: base(control, box, appearance, cache, defaultDraw) {
		}
		#region Properties
		public int RowIndex { get { return Box.ModelIndex; } }
		#endregion
	}
	#endregion
	#region CustomDrawColumnHeaderBackgroundEventHandler
	public delegate void CustomDrawColumnHeaderBackgroundEventHandler(object sender, CustomDrawColumnHeaderBackgroundEventArgs e);
	#endregion
	#region CustomDrawColumnHeaderBackgroundEventArgs
	public class CustomDrawColumnHeaderBackgroundEventArgs : CustomDrawHeaderEventArgsBase {
		internal CustomDrawColumnHeaderBackgroundEventArgs(SpreadsheetControl control, HeaderTextBox box, AppearanceObject appearance, GraphicsCache cache, Action defaultDraw)
			: base(control, box, appearance, cache, defaultDraw) {
		}
		#region Properties
		public int ColumnIndex { get { return Box.ModelIndex; } }
		#endregion
	}
	#endregion
	#region CustomDrawFrozenPaneBorderEventHandler
	public delegate void CustomDrawFrozenPaneBorderEventHandler(object sender, CustomDrawFrozenPaneBorderEventArgs e);
	#endregion
	#region CustomDrawFrozenPaneBorderEventArgs
	public enum FrozenPaneBorderType {
		Vertical,
		Horizontal
	}
	public class CustomDrawFrozenPaneBorderEventArgs : CustomDrawObjectEventsArgs {
		#region Fields
		Point point1;
		Point point2;
		float width;
		Color color;
		#endregion
		internal CustomDrawFrozenPaneBorderEventArgs(Point point1, Point point2, float width, Color color, GraphicsCache cache, Action defaultDraw)
			: base(cache, defaultDraw) {
			this.point1 = point1;
			this.point2 = point2;
			this.width = width;
			this.color = color;
		}
		#region Properties
		public Point Point1 { get { return point1; } }
		public Point Point2 { get { return point2; } }
		public float Width { get { return width; } }
		public Color ForeColor { get { return color; } }
		public FrozenPaneBorderType Type { get { return point1.X == point2.X ? FrozenPaneBorderType.Vertical : FrozenPaneBorderType.Horizontal; } }
		#endregion
	}
	#endregion
	#region CustomDrawHeaderEventArgsBase
	public class CustomDrawHeaderEventArgsBase : CustomDrawRectangularObjectEventsArgs {
		readonly SpreadsheetControl control;
		readonly HeaderTextBox box;
		readonly AppearanceObject appearance;
		protected CustomDrawHeaderEventArgsBase(SpreadsheetControl control, HeaderTextBox box, AppearanceObject appearance, GraphicsCache cache, Action defaultDraw)
			: base(cache, defaultDraw) {
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(box, "box");
			Guard.ArgumentNotNull(appearance, "appearance");
			this.control = control;
			this.box = box;
			this.appearance = appearance;
		}
		#region Properties
		protected HeaderTextBox Box { get { return box; } }
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("CustomDrawHeaderEventArgsBaseAppearance")]
#endif
		public AppearanceObject Appearance { get { return appearance; } }
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("CustomDrawHeaderEventArgsBaseForeColor")]
#endif
		public Color ForeColor { get { return appearance.ForeColor; } }
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("CustomDrawHeaderEventArgsBaseBackColor")]
#endif
		public Color BackColor { get { return appearance.BackColor; } }
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("CustomDrawHeaderEventArgsBaseFont")]
#endif
		public Font Font { get { return appearance.Font; } }
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("CustomDrawHeaderEventArgsBaseBounds")]
#endif
		public override Rectangle Bounds { get { return box.Bounds; } }
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("CustomDrawHeaderEventArgsBaseText")]
#endif
		public string Text { get { return box.Text; } }
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("CustomDrawHeaderEventArgsBaseIsHovered")]
#endif
		public bool IsHovered { get { return box.SelectType == HeaderBoxSelectType.Active; } }
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("CustomDrawHeaderEventArgsBaseIsSelected")]
#endif
		public bool IsSelected { get { return box.SelectType == HeaderBoxSelectType.Single || box.SelectType == HeaderBoxSelectType.Interval; } }
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("CustomDrawHeaderEventArgsBaseControl")]
#endif
		public SpreadsheetControl Control { get { return control; } }
		#endregion
	}
	#endregion
	#region CustomDrawCellEventArgsBase (abstract class)
	public abstract class CustomDrawCellEventArgsBase : CustomDrawRectangularObjectEventsArgs {
		#region Fields
		readonly SpreadsheetControl control;
		readonly DevExpress.XtraSpreadsheet.Layout.Page page;
		readonly ICellTextBox box;
		Rectangle bounds;
		Rectangle fillBounds;
		DevExpress.Spreadsheet.Cell apiCell;
		#endregion
		protected CustomDrawCellEventArgsBase(SpreadsheetControl control, DevExpress.XtraSpreadsheet.Layout.Page page, ICellTextBox box, GraphicsCache cache, Action defaultDraw)
			: base(cache, defaultDraw) {
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(page, "page");
			Guard.ArgumentNotNull(box, "box");
			this.control = control;
			this.page = page;
			this.box = box;
		}
		#region Properties
		protected DevExpress.XtraSpreadsheet.Layout.Page Page { get { return page; } }
		protected ICellTextBox Box { get { return box; } }
		protected SpreadsheetControl Control { get { return control; } }
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("CustomDrawCellEventArgsBaseBounds")]
#endif
		public override Rectangle Bounds {
			get {
				if (bounds == Rectangle.Empty)
					bounds = CalculateBounds();
				return bounds;
			}
		}
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("CustomDrawCellEventArgsBaseFillBounds")]
#endif
		public Rectangle FillBounds {
			get {
				if (fillBounds == Rectangle.Empty)
					fillBounds = Box.GetFillBounds(Page);
				return fillBounds;
			}
		}
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("CustomDrawCellEventArgsBaseCell")]
#endif
		public DevExpress.Spreadsheet.Cell Cell {
			get {
				if (apiCell == null) {
					DevExpress.XtraSpreadsheet.Model.ICell modelCell = ModelCell;
					apiCell = control.Document.Worksheets[page.Sheet.Name].Cells[modelCell.RowIndex, modelCell.ColumnIndex];
				}
				return apiCell;
			}
		}
		protected DevExpress.XtraSpreadsheet.Model.ICell ModelCell { get { return box.GetCell(page.GridColumns, page.GridRows, page.Sheet); } }
		#endregion
		protected abstract Rectangle CalculateBounds();
	}
	#endregion
	#region CustomDrawObjectEventsArgs (abstract class)
	public abstract class CustomDrawObjectEventsArgs : EventArgs {
		#region Fields
		readonly Action defaultDraw;
		readonly GraphicsCache cache;
		#endregion
		protected CustomDrawObjectEventsArgs(GraphicsCache cache, Action defaultDraw) {
			Guard.ArgumentNotNull(cache, "cache");
			Guard.ArgumentNotNull(defaultDraw, "defaultDraw");
			this.cache = cache;
			this.defaultDraw = defaultDraw;
		}
		#region Properties
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("CustomDrawObjectEventsArgsHandled")]
#endif
		public bool Handled { get; set; }
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("CustomDrawObjectEventsArgsCache")]
#endif
		public GraphicsCache Cache { get { return cache; } }
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("CustomDrawObjectEventsArgsGraphics")]
#endif
		public Graphics Graphics { get { return Cache.Graphics; } }
		#endregion
		public void DrawDefault() {
			if (defaultDraw != null) {
				using (GraphicsClipState clipState = cache.ClipInfo.SaveClip()) {
					try {
						defaultDraw();
					}
					finally {
						cache.ClipInfo.RestoreClip(clipState);
					}
				}
			}
		}
	}
	#endregion
	#region CustomDrawRectangularObjectEventsArgs (abstract class)
	public abstract class CustomDrawRectangularObjectEventsArgs : CustomDrawObjectEventsArgs {
		protected CustomDrawRectangularObjectEventsArgs(GraphicsCache cache, Action defaultDraw)
			: base(cache, defaultDraw) {
		}
		#region Properties
		public abstract Rectangle Bounds { get; }
		#endregion
	}
	#endregion
}
