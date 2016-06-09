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

using System.Collections.Generic;
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Office.Layout;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using System;
namespace DevExpress.XtraSpreadsheet.Drawing {
	#region SpreadsheetGroupItemsPainter (abstract class)
	public abstract class SpreadsheetGroupItemsPainter {
		const int lineWidth = 2;
		#region Fields
		readonly SpreadsheetControl control;
		DocumentLayoutUnitConverter layoutUnitConverter;
		AppearanceDefault defaultAppearance;
		AppearanceObject appearance;
		#endregion
		protected SpreadsheetGroupItemsPainter(SpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			Initialize();
		}
		#region Properties
		public DocumentLayoutUnitConverter LayoutUnitConverter { get { return layoutUnitConverter; } }
		public SpreadsheetControl Control { get { return control; } }
		#endregion
		public virtual void Initialize() {
			this.defaultAppearance = GetDefaultAppearance();
		}
		protected internal virtual AppearanceDefault GetDefaultAppearance() {
			return new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, SystemColors.ControlDarkDark, SystemColors.ActiveBorder, HorzAlignment.Center, VertAlignment.Center);
		}
		public void Draw(GraphicsCache cache, GroupItemsPage groupPage, DocumentLayoutUnitConverter layoutUnitConverter) {
			this.layoutUnitConverter = layoutUnitConverter;
			this.appearance = new AppearanceObject(defaultAppearance);
			if (!control.Enabled)
				appearance.ForeColor = SystemColors.GrayText;
			appearance.Font = GetActualFont(appearance.Font);
			DrawHeaders(cache, groupPage);
			DrawDots(cache, groupPage.Dots);
			DrawLines(cache, groupPage.Lines);
			DrawButtons(cache, groupPage);
		}
		void DrawHeaders(GraphicsCache cache, GroupItemsPage groupPage) {
			DrawPageBackground(cache, groupPage);
		}
		void DrawDots(GraphicsCache cache, List<Point> dots) {
			foreach (Point dot in dots)
				DrawDot(cache, dot);
		}
		void DrawLines(GraphicsCache cache, List<OutlineLevelLine> lines) {
			foreach (OutlineLevelLine line in lines)
				DrawLine(cache, line);
		}
		void DrawButtons(GraphicsCache cache, GroupItemsPage groupPage) {
			foreach (OutlineLevelBox button in groupPage.Buttons) {
				if (button.IsHeaderButton)
					DrawHeaderButton(cache, button);
				else DrawButton(cache, button);
			}
		}
		Font GetActualFont(Font font) {
			return new Font(font.Name, layoutUnitConverter.PixelsToLayoutUnitsF(font.Size, DocumentModel.Dpi), font.Unit);
		}
		int GetBoxImageIndex(OutlineLevelBoxSelectType selectType) {
			if (!Control.Enabled)
				return 0;
			switch (selectType) {
				case OutlineLevelBoxSelectType.None:
					return 0;
				case OutlineLevelBoxSelectType.Hovered:
					return 1;
				case OutlineLevelBoxSelectType.Pressed:
					return 2;
				default:
					return 0;
			}
		}
		protected internal void DrawText(GraphicsCache cache, string text, Rectangle layoutBounds) {
			appearance.DrawString(cache, text, layoutBounds);
		}
		protected internal void DrawBackground(GraphicsCache cache, Rectangle bounds) {
			DrawBackgroundCore(cache, bounds);
		}
		protected internal void DrawHeaderButton(GraphicsCache cache, OutlineLevelBox box) {
			Rectangle bounds = box.Bounds;
			bounds.Offset(box.IsRowButton ? 1 : 0, box.IsRowButton ? 0 : 1);
			DrawButtonBackgroundCore(cache, bounds, GetBoxImageIndex(box.OutlineLevelBoxSelectType));
			DrawText(cache, box.Text, bounds);
		}
		protected internal void DrawButton(GraphicsCache cache, OutlineLevelBox box) {
			int imageIndex = GetBoxImageIndex(box.OutlineLevelBoxSelectType);
			DrawButtonBackgroundCore(cache, box.Bounds, imageIndex);
			DrawButtonTextCore(cache, box.Text, box.Bounds, box.IsCollapsedButton);
		}
		protected internal void DrawDot(GraphicsCache cache, Point dot) {
			int layoutDotSize = layoutUnitConverter.PixelsToLayoutUnits(2, DocumentModel.Dpi);
			int layoutDotOffset = layoutUnitConverter.PixelsToLayoutUnits(1, DocumentModel.Dpi);
			DrawDotCore(cache, new Rectangle(dot.X - layoutDotOffset, dot.Y - layoutDotOffset, layoutDotSize, layoutDotSize));
		}
		protected internal void DrawLine(GraphicsCache cache, OutlineLevelLine line) {
			Size lineSize = new Size(Math.Max(line.LinePoint2.X - line.LinePoint1.X, lineWidth), Math.Max(line.LinePoint2.Y - line.LinePoint1.Y, lineWidth));
			Size tailSize = new Size(Math.Max(line.TailPoint2.X - line.TailPoint1.X, lineWidth), Math.Max(line.TailPoint2.Y - line.TailPoint1.Y, lineWidth));
			Rectangle lineBounds = new Rectangle(line.LinePoint1, lineSize);
			Rectangle tailBounds = new Rectangle(line.TailPoint1, tailSize);
			DrawLineCore(cache, lineBounds, tailBounds);
		}
		protected internal void DrawPageBackground(GraphicsCache cache, GroupItemsPage groupPage) {
			DrawPageBackgroundCore(cache, groupPage.ColumnBounds, groupPage.RowBounds);
		}
		protected internal virtual void DrawBackgroundCore(GraphicsCache cache, Rectangle bounds) {
			cache.FillRectangle(appearance.BackColor, bounds);
		}
		protected internal virtual void DrawButtonBackgroundCore(GraphicsCache cache, Rectangle bounds, int imageIndex) {
			DrawBackgroundCore(cache, bounds);
		}
		protected internal virtual void DrawButtonTextCore(GraphicsCache cache, string text, Rectangle bounds, bool collapsed) {
			DrawText(cache, text, bounds);
		}
		protected internal virtual void DrawDotCore(GraphicsCache cache, Rectangle dotBounds) {
			cache.FillRectangle(appearance.ForeColor, dotBounds);
		}
		protected internal virtual void DrawLineCore (GraphicsCache cache, Rectangle lineBounds, Rectangle tailBounds) {
			cache.FillRectangle(appearance.ForeColor, lineBounds);
			cache.FillRectangle(appearance.ForeColor, tailBounds);
		}
		protected internal virtual void DrawHeaderLineCore(GraphicsCache cache, Rectangle lineBounds) {
			cache.FillRectangle(appearance.BorderColor, lineBounds);
		}
		protected internal virtual void DrawPageBackgroundCore(GraphicsCache cache, Rectangle columnBounds, Rectangle rowBounds) {
			cache.FillRectangle(appearance.BackColor, columnBounds);
			cache.FillRectangle(appearance.BackColor, rowBounds);
		}
	}
	#endregion
	#region SpreadsheetGroupItemsFlatPainter
	public class SpreadsheetGroupItemsFlatPainter : SpreadsheetGroupItemsPainter {
		public SpreadsheetGroupItemsFlatPainter(SpreadsheetControl control)
			: base(control) {
		}
	}
	#endregion
	#region SpreadsheetGroupItemsUltraFlatPainter
	public class SpreadsheetGroupItemsUltraFlatPainter : SpreadsheetGroupItemsPainter {
		public SpreadsheetGroupItemsUltraFlatPainter(SpreadsheetControl control)
			: base(control) {
		}
	}
	#endregion
	#region SpreadsheetGroupItemsStyle3DPainter
	public class SpreadsheetGroupItemsStyle3DPainter : SpreadsheetGroupItemsPainter {
		public SpreadsheetGroupItemsStyle3DPainter(SpreadsheetControl control)
			: base(control) {
		}
	}
	#endregion
	#region SpreadsheetGroupItemsOffice2003Painter
	public class SpreadsheetGroupItemsOffice2003Painter : SpreadsheetGroupItemsPainter {
		public SpreadsheetGroupItemsOffice2003Painter(SpreadsheetControl control)
			: base(control) {
		}
	}
	#endregion
	#region SpreadsheetGroupItemsWindowsXPPainter
	public class SpreadsheetGroupItemsWindowsXPPainter : SpreadsheetGroupItemsPainter {
		public SpreadsheetGroupItemsWindowsXPPainter(SpreadsheetControl control)
			: base(control) {
		}
	}
	#endregion
	#region SpreadsheetGroupItemsSkinPainter
	public class SpreadsheetGroupItemsSkinPainter : SpreadsheetGroupItemsPainter {
		#region Fields
		UserLookAndFeel lookAndFeel;
		#endregion
		public SpreadsheetGroupItemsSkinPainter(SpreadsheetControl control, UserLookAndFeel lookAndFeel)
			: base(control) {
			this.lookAndFeel = lookAndFeel;
		}
		#region Properties
		public UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		#endregion
		protected internal override void DrawButtonBackgroundCore(GraphicsCache cache, Rectangle bounds, int imageIndex) {
			SkinPaintHelper.DrawSkinElement(EditorsSkins.GetSkin(LookAndFeel), cache, EditorsSkins.SkinEditorButton, bounds, imageIndex);
		}
		protected internal override void DrawButtonTextCore(GraphicsCache cache, string text, Rectangle bounds, bool collapsed) {
			Rectangle symbolBounds = bounds;
			symbolBounds.Offset(1, 1);
			SkinPaintHelper.DrawSkinElement(EditorsSkins.GetSkin(LookAndFeel), cache, EditorsSkins.SkinNavigator, symbolBounds, collapsed ? 6 : 7);
		}
		protected internal override void DrawBackgroundCore(GraphicsCache cache, Rectangle bounds) {
			SkinPaintHelper.DrawSkinElement(LookAndFeel, cache, GridSkins.SkinHeader, bounds, 0);
		}
		protected internal override void DrawDotCore(GraphicsCache cache, Rectangle dotBounds) {
			Color dotColor = CommonSkins.GetSkin(LookAndFeel).Colors[CommonColors.DisabledText];
			cache.FillRectangle(dotColor, dotBounds);
		}
		protected internal override void DrawLineCore(GraphicsCache cache, Rectangle lineBounds, Rectangle tailBounds) {
			Color lineColor = CommonSkins.GetSkin(LookAndFeel).Colors[CommonColors.DisabledText];
			cache.FillRectangle(lineColor, lineBounds);
			cache.FillRectangle(lineColor, tailBounds);
		}
		protected internal override void DrawHeaderLineCore(GraphicsCache cache, Rectangle lineBounds) {
			SkinElement element = SkinPaintHelper.GetSkinElement(LookAndFeel, GridSkins.SkinGridLine);
			Color lineColor = element.Color.BackColor;
			cache.FillRectangle(lineColor, lineBounds);
		}
		protected internal override void DrawPageBackgroundCore(GraphicsCache cache, Rectangle columnBounds, Rectangle rowBounds) {
			Color backgroundColor = CommonSkins.GetSkin(LookAndFeel).Colors[CommonColors.Control];
			cache.FillRectangle(backgroundColor, columnBounds);
			cache.FillRectangle(backgroundColor, rowBounds);
		}
		protected internal override AppearanceDefault GetDefaultAppearance() {
			AppearanceDefault appearance = base.GetDefaultAppearance();
			SkinElement element = SkinPaintHelper.GetSkinElement(LookAndFeel, GridSkins.SkinHeader);
			if (element != null)
				element.ApplyForeColorAndFont(appearance);
			return appearance;
		}
	}
	#endregion
}
