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
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
using DevExpress.Office.Layout;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Layout;
namespace DevExpress.XtraSpreadsheet.Drawing {
	#region SpreadsheetHeaderPainter (abstract class)
	public abstract class SpreadsheetHeaderPainter {
		#region Fields
		const int selectAllTriangleOffsetInPixels = 4; 
		const int selectAllTriangleWidthInPixels = 8; 
		readonly SpreadsheetControl control;
		DocumentLayoutUnitConverter layoutUnitConverter;
		AppearanceDefault defaultAppearance;
		AppearanceObject appearance;
		#endregion
		protected SpreadsheetHeaderPainter(SpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
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
		public void Draw(GraphicsCache cache, HeaderPage headerPage, DocumentLayoutUnitConverter layoutUnitConverter) {
			this.layoutUnitConverter = layoutUnitConverter;
			this.appearance = new AppearanceObject(defaultAppearance);
			if (!control.Enabled)
				appearance.ForeColor = SystemColors.GrayText;
			appearance.Font = GetActualFont(appearance.Font);
				DrawSelectAllBox(cache, headerPage.SelectAllButton);
			DrawColumnBoxes(cache, headerPage.ColumnBoxes);
			DrawRowBoxes(cache, headerPage.RowBoxes);
		}
		protected internal void DrawSelectAllBox(GraphicsCache cache, HeaderTextBox box) {
			if (!control.Options.View.ShowRowHeaders || !control.Options.View.ShowColumnHeaders)
				return;
			DrawSelectAllBoxCore(cache, box);
			DrawSelectAllTriangle(cache, box.Bounds);
		}
		protected internal virtual void DrawSelectAllBoxCore(GraphicsCache cache, HeaderTextBox box) {
			DrawBackground(cache, box);
		}
		protected internal virtual void DrawSelectAllTriangle(GraphicsCache cache, Rectangle bounds) {
			int selectAllTriangleOffset = layoutUnitConverter.PixelsToLayoutUnits(selectAllTriangleOffsetInPixels, DocumentModel.Dpi);
			Point bottomRight = new Point(bounds.Right - selectAllTriangleOffset, bounds.Bottom - selectAllTriangleOffset);
			int selectAllTriangleWidth = layoutUnitConverter.PixelsToLayoutUnits(selectAllTriangleWidthInPixels, DocumentModel.Dpi);
			Point bottomLeft = new Point(bottomRight.X - selectAllTriangleWidth, bottomRight.Y);
			Point top = new Point(bottomRight.X, bottomRight.Y - selectAllTriangleWidth);
			Brush brush = cache.GetSolidBrush(GetSelectAllTriangleColor());
			cache.Graphics.FillPolygon(brush, new Point[] { top, bottomRight, bottomLeft });
		}
		protected internal virtual Color GetSelectAllTriangleColor() {
			return appearance.ForeColor;
		}
		#region Draw Rows
		protected internal virtual void DrawRowBoxes(GraphicsCache cache, List<HeaderTextBox> boxes) {
			foreach (HeaderTextBox box in boxes) {
				PerformDrawRowBackground(cache, box);
				PerformDrawRowText(cache, box);
			}
		}
		void PerformDrawRowBackground(GraphicsCache cache, HeaderTextBox box) {
			Action defaultDraw = () => { DrawBackground(cache, box); };
			if (RaiseCustomDrawRowHeaderBackground(cache, box, defaultDraw))
				return;
			defaultDraw();
		}
		void PerformDrawRowText(GraphicsCache cache, HeaderTextBox box) {
			Action defaultDraw = () => { DrawText(cache, box); };
			if (RaiseCustomDrawRowHeader(cache, box, defaultDraw))
				return;
			defaultDraw();
		}
		bool RaiseCustomDrawRowHeaderBackground(GraphicsCache cache, HeaderTextBox box, Action defaultDraw) {
			if (!control.HasCustomDrawRowHeaderBackground)
				return false;
			CustomDrawRowHeaderBackgroundEventArgs args = new CustomDrawRowHeaderBackgroundEventArgs(control, box, appearance, cache, defaultDraw);
			control.RaiseCustomDrawRowHeaderBackground(args);
			return args.Handled;
		}
		bool RaiseCustomDrawRowHeader(GraphicsCache cache, HeaderTextBox box, Action defaultDraw) {
			if (!control.HasCustomDrawRowHeader)
				return false;
			CustomDrawRowHeaderEventArgs args = new CustomDrawRowHeaderEventArgs(control, box, appearance, cache, defaultDraw);
			control.RaiseCustomDrawRowHeader(args);
			return args.Handled;
		}
		#endregion
		#region Draw Columns
		protected internal virtual void DrawColumnBoxes(GraphicsCache cache, List<HeaderTextBox> boxes) {
			foreach (HeaderTextBox box in boxes) {
				PerformDrawColumnBackground(cache, box);
				PerformDrawColumnText(cache, box);
			}
		}
		void PerformDrawColumnBackground(GraphicsCache cache, HeaderTextBox box) {
			Action defaultDraw = () => { DrawBackground(cache, box); };
			if (RaiseCustomDrawColumnHeaderBackground(cache, box, defaultDraw))
				return;
			defaultDraw();
		}
		void PerformDrawColumnText(GraphicsCache cache, HeaderTextBox box) {
			Action defaultDraw = () => { DrawText(cache, box); };
			if (RaiseCustomDrawColumnHeader(cache, box, defaultDraw))
				return;
			defaultDraw();
		}
		bool RaiseCustomDrawColumnHeaderBackground(GraphicsCache cache, HeaderTextBox box, Action defaultDraw) {
			if (!control.HasCustomDrawColumnHeaderBackground)
				return false;
			CustomDrawColumnHeaderBackgroundEventArgs args = new CustomDrawColumnHeaderBackgroundEventArgs(control, box, appearance, cache, defaultDraw);
			control.RaiseCustomDrawColumnHeaderBackground(args);
			return args.Handled;
		}
		bool RaiseCustomDrawColumnHeader(GraphicsCache cache, HeaderTextBox box, Action defaultDraw) {
			if (!control.HasCustomDrawColumnHeader)
				return false;
			CustomDrawColumnHeaderEventArgs args = new CustomDrawColumnHeaderEventArgs(control, box, appearance, cache, defaultDraw);
			control.RaiseCustomDrawColumnHeader(args);
			return args.Handled;
		}
		#endregion
		protected internal virtual void DrawBackground(GraphicsCache cache, HeaderTextBox box) {
			DrawBackgroundCore(cache, box.Bounds);
		}
		void DrawBackgroundCore(GraphicsCache cache, Rectangle bounds) {
			cache.FillRectangle(appearance.BackColor, bounds);
			cache.DrawRectangle(Pens.Black, bounds);
		}
		protected internal void DrawText(GraphicsCache cache, HeaderTextBox box) {
			appearance.DrawString(cache, box.Text, box.Bounds);
		}
		Font GetActualFont(Font font) {
			return new Font(font.Name, layoutUnitConverter.PixelsToLayoutUnitsF(font.Size, DocumentModel.Dpi), font.Unit);
		}
		protected internal virtual void DrawRightHeader(GraphicsCache cache, Rectangle rightHeaderBounds) {
			if (rightHeaderBounds.IsEmpty)
				return;
			DrawBackgroundCore(cache, rightHeaderBounds);
		}
	}
	#endregion
	#region SpreadsheetHeaderFlatPainter
	public class SpreadsheetHeaderFlatPainter : SpreadsheetHeaderPainter {
		public SpreadsheetHeaderFlatPainter(SpreadsheetControl control)
			: base(control) {
		}
	}
	#endregion
	#region SpreadsheetHeaderUltraFlatPainter
	public class SpreadsheetHeaderUltraFlatPainter : SpreadsheetHeaderPainter {
		public SpreadsheetHeaderUltraFlatPainter(SpreadsheetControl control)
			: base(control) {
		}
	}
	#endregion
	#region SpreadsheetHeaderStyle3DPainter
	public class SpreadsheetHeaderStyle3DPainter : SpreadsheetHeaderPainter {
		public SpreadsheetHeaderStyle3DPainter(SpreadsheetControl control)
			: base(control) {
		}
	}
	#endregion
	#region SpreadsheetHeaderOffice2003Painter
	public class SpreadsheetHeaderOffice2003Painter : SpreadsheetHeaderPainter {
		public SpreadsheetHeaderOffice2003Painter(SpreadsheetControl control)
			: base(control) {
		}
	}
	#endregion
	#region SpreadsheetHeaderWindowsXPPainter
	public class SpreadsheetHeaderWindowsXPPainter : SpreadsheetHeaderPainter {
		public SpreadsheetHeaderWindowsXPPainter(SpreadsheetControl control)
			: base(control) {
		}
	}
	#endregion
	#region SpreadsheetHeaderSkinPainter
	public class SpreadsheetHeaderSkinPainter : SpreadsheetHeaderPainter {
		#region Fields
		UserLookAndFeel lookAndFeel;
		#endregion
		public SpreadsheetHeaderSkinPainter(SpreadsheetControl control, UserLookAndFeel lookAndFeel)
			: base(control) {
			this.lookAndFeel = lookAndFeel;
		}
		#region Properties
		public UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		#endregion
		protected internal override void DrawSelectAllBoxCore(GraphicsCache cache, HeaderTextBox box) {
			SkinPaintHelper.DrawSkinElement(LookAndFeel, cache, GridSkins.SkinHeaderLeft, box.Bounds);
		}
		protected internal override Color GetSelectAllTriangleColor() {
			Color color = SpreadsheetSkins.GetSkin(LookAndFeel).Colors[SpreadsheetSkins.ColorSelectAllTriangle];
			if (!DXColor.IsTransparentOrEmpty(color))
				return color;
			return base.GetSelectAllTriangleColor();
		}
		protected internal override void DrawBackground(GraphicsCache cache, HeaderTextBox box) {
			int imageIndex = GetHeaderImageIndex(box.SelectType);
			SkinPaintHelper.DrawSkinElement(LookAndFeel, cache, GridSkins.SkinHeader, box.Bounds, imageIndex);
		}
		int GetHeaderImageIndex(HeaderBoxSelectType selectType) {
			if (!Control.Enabled)
				return 0;
			switch (selectType) {
				case HeaderBoxSelectType.None:
					return 0;
				case HeaderBoxSelectType.Active:
					return 1;
				case HeaderBoxSelectType.Single:
					return 2;
				case HeaderBoxSelectType.Interval:
					return 2;
				default:
					return 0;
			}
		}
		protected internal override void DrawRightHeader(GraphicsCache cache, Rectangle rightHeaderBounds) {
			SkinPaintHelper.DrawSkinElement(LookAndFeel, cache, GridSkins.SkinHeaderRight, rightHeaderBounds);
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
