#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Drawing;
namespace DevExpress.DashboardWin.Native {
	public abstract class DragAreaStylePainterBase : DragAreaPainter {
		readonly int leftMargin;
		readonly int topMargin;
		readonly int horizontalMargins;
		readonly int verticalMargins;
		public override int HorizontalMargins { get { return horizontalMargins; } }
		protected DragAreaStylePainterBase(Padding contentMargins) {
			leftMargin = contentMargins.Left;
			horizontalMargins = leftMargin + contentMargins.Right;
			topMargin = contentMargins.Top;
			verticalMargins = topMargin + contentMargins.Bottom;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return new Rectangle(client.X - leftMargin, client.Y - topMargin, client.Width + horizontalMargins, client.Height + verticalMargins);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle bounds = e.Bounds;
			return new Rectangle(bounds.X + leftMargin, bounds.Y + topMargin, bounds.Width - horizontalMargins, bounds.Height - verticalMargins);
		}
	}
	public class DragAreaStylePainter : DragAreaStylePainterBase, IDragAreaPainter {
		readonly int sectionIndent;
		readonly Color sectionHeaderColor;
		public int SectionIndent { get { return sectionIndent; } }
		public Color SectionHeaderColor { get { return sectionHeaderColor; } }
		public DragAreaStylePainter(Padding contentMargins, int sectionIndent, Color sectionHeaderColor) : base(contentMargins) {
			this.sectionIndent = sectionIndent;
			this.sectionHeaderColor = sectionHeaderColor;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			GetStyle(e).DrawBackground(e.Cache, e.Bounds);
		}
	}
	public class DragGroupStylePainter : DragAreaStylePainterBase, IDragGroupPainter {
		readonly int groupIndent;
		readonly int itemIndent;
		readonly int groupOptionsButtonIndent;
		readonly Color dropIndicatorColor;
		readonly Color selectedBorderColor;
		readonly int selectedBorderThickness;
		public int GroupIndent { get { return groupIndent; } }
		public int ItemIndent { get { return itemIndent; } }
		public int ButtonIndent { get { return groupOptionsButtonIndent; } }
		public Color DropIndicatorColor { get { return dropIndicatorColor; } }
		public DragGroupStylePainter(Padding contentMargins, int groupIndent, int itemIndent,int groupOptionsButtonIndent, Color dropIndicatorColor, Color selectedBorderColor, int selectedBorderThickness) : base(contentMargins) {
			this.groupIndent = groupIndent;
			this.itemIndent = itemIndent;
			this.groupOptionsButtonIndent = groupOptionsButtonIndent;
			this.dropIndicatorColor = dropIndicatorColor;
			this.selectedBorderColor = selectedBorderColor;
			this.selectedBorderThickness = selectedBorderThickness;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			AppearanceObject appearance = GetStyle(e);
			GraphicsCache cache = e.Cache;
			Rectangle bounds = e.Bounds;
			appearance.DrawBackground(cache, bounds);
			GroupInfoArgs groupInfoArgs = e as GroupInfoArgs;
			using(Pen borderPen = (groupInfoArgs != null && (groupInfoArgs.State == DragGroupState.Selected || groupInfoArgs.State == DragGroupState.Hot)) ? new Pen(selectedBorderColor, selectedBorderThickness) : new Pen(appearance.BorderColor))
				cache.DrawRectangle(borderPen, bounds);
		}
	}
	public class DragGroupSelectorStylePainter : DragAreaStylePainterBase {
		public DragGroupSelectorStylePainter() : base(new Padding()) {
		}
		public override void DrawObject(ObjectInfoArgs e) {
			AppearanceObject appearance = GetStyle(e);
			GraphicsCache cahce = e.Cache;
			Rectangle bounds = e.Bounds;
			appearance.DrawBackground(cahce, bounds);
			DragAreaButtonInfoArgs groupSelectorInfoArgs = e as DragAreaButtonInfoArgs;
			if (groupSelectorInfoArgs != null) {
				int left = bounds.Left;
				int top = bounds.Top;
				int width = bounds.Width;
				int height = bounds.Height;
				if (groupSelectorInfoArgs.ButtonState != DragAreaButtonState.Normal)
					using (Pen borderPen = new Pen(appearance.BorderColor))
						cahce.DrawRectangle(borderPen, new Rectangle(left + 1, top, width - 1, height));
				Image glyph = groupSelectorInfoArgs.Glyph;
				if (glyph != null) {
					int glyphWidth = glyph.Width;
					int glyphHeight = glyph.Height;
					e.Graphics.DrawImage(glyph, left + (width - glyphWidth) / 2, top + (height - glyphHeight) / 2, glyphWidth, glyphHeight);
				}
			}
		}
	}
	public class DragItemStylePainter : DragAreaStylePainterBase {
		readonly Color dropIndicatorColor;
		readonly int dropIndicatorThickness;
		public DragItemStylePainter(Padding contentMargins, Color dropIndicatorColor, int dropIndicatorThickness) : base(contentMargins) {
			this.dropIndicatorColor = dropIndicatorColor;
			this.dropIndicatorThickness = dropIndicatorThickness;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			AppearanceObject appearance = GetStyle(e);
			GraphicsCache cache = e.Cache;
			Rectangle bounds = e.Bounds;
			DragItemInfoArgs dragItemInfoArgs = e as DragItemInfoArgs;
			DragItemState itemState = dragItemInfoArgs == null ? DragItemState.Normal : dragItemInfoArgs.ItemState;
			if (itemState == DragItemState.Selected)
				cache.FillRectangle(appearance.BackColor2, bounds);
			else if (itemState == DragItemState.Normal || itemState == DragItemState.Hot || itemState == DragItemState.DropTarget)
				cache.FillRectangle(appearance.BackColor, bounds);
			using (Pen borderPen = new Pen(appearance.BorderColor))
				cache.DrawRectangle(borderPen, bounds);
			if (itemState == DragItemState.DropTarget || itemState == DragItemState.PlaceHolderDropDestination)
				using (Pen selectionPen = new Pen(dropIndicatorColor, dropIndicatorThickness))
					cache.DrawRectangle(selectionPen, bounds);
		}
	}
	public class DragItemOptionsButtonStylePainter : DragAreaStylePainterBase, IDragItemOptionsButtonPainter {
		public DragItemOptionsButtonStylePainter() : base(new Padding()) {
		}
		public Rectangle GetActualBounds(Rectangle bounds) {
			return bounds;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			Rectangle bounds = e.Bounds;
			int left = bounds.Left + bounds.Width / 2 - 3;
			int top = bounds.Top + bounds.Height / 2 - 1;
			int bottom = bounds.Bottom;
			ElementWithButtonInfoArgs buttonInfoArgs = e as ElementWithButtonInfoArgs;
			if (buttonInfoArgs != null && buttonInfoArgs.ButtonState == DragAreaButtonState.Selected) {
				top++;
				left++;
			}
			GraphicsCache cache = e.Cache;
			XPaint paint = cache.Paint;
			Graphics graphics = e.Graphics;
			Brush brush = GetStyle(e).GetForeBrush(cache);
			for (int i = top, x = left, w = 7; i < bottom && w > 0; i++, x++, w -= 2)
				paint.FillRectangle(graphics, brush, x, i, w, 1);
		}
	}
	public class SplitterDragSectionStylePainter : StyleObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			e.Cache.FillRectangle(GetStyle(e).GetForeBrush(e.Cache), new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, 1));
		}
	}
	public class DragAreaStylePainters : DragAreaPainters {
		public DragAreaStylePainters() {
			Color highlightColor = SystemColors.Highlight;
			AreaPainter = new DragAreaStylePainter(new Padding(15), 15, highlightColor);
			GroupPainter = new DragGroupStylePainter(new Padding(5), 4, 4, 0, highlightColor, highlightColor, 3);
			GroupSelectorPainter = new DragGroupSelectorStylePainter();
			DragItemPainter = new DragItemStylePainter(new Padding(16, 6, 16, 6), highlightColor, 3);
			DragItemOptionsButtonPainter = new DragItemOptionsButtonStylePainter();
			SplitterPainter = new SplitterDragSectionStylePainter();
		}
	}
}
