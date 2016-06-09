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
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
using DevExpress.Office.Layout;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Layout;
namespace DevExpress.XtraSpreadsheet.Drawing {
	#region FrozenPanesSeparatorPainter (abstract class)
	public abstract class FrozenPanesSeparatorPainter {
		#region Fields
		const int separatorWidthInPixels = 1;
		readonly SpreadsheetControl control;
		GraphicsCache cache;
		#endregion
		protected FrozenPanesSeparatorPainter(SpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		#region Properties
		public DocumentLayoutUnitConverter UnitConverter { get { return control.DocumentModel.LayoutUnitConverter; } }
		#endregion
		public virtual void Initialize() {
		}
		public void Draw(GraphicsCache cache, Rectangle clientBounds, Page topLeftPage, float zoomFactor) {
			this.cache = cache;
			float separatorWidth = UnitConverter.PixelsToLayoutUnitsF(separatorWidthInPixels, DocumentModel.Dpi);
			using (Pen colorPen = new Pen(GetSeparatorColor(), separatorWidth)) {
				DrawCore(colorPen, clientBounds, topLeftPage, zoomFactor);
			}
		}
		protected internal virtual Color GetSeparatorColor() {
			return Color.Black;
		}
		void DrawCore(Pen pen, Rectangle clientBounds, Page topLeftPage, float zoomFactor) {
			Worksheet sheet = topLeftPage.Sheet;
			if (sheet.IsOnlyColumnsFrozen())
				PerformDrawVerticalLine(pen, clientBounds, topLeftPage, zoomFactor);
			else if (sheet.IsOnlyRowsFrozen())
				PerformDrawHorizontalLine(pen, clientBounds, topLeftPage, zoomFactor);
			else {
				PerformDrawVerticalLine(pen, clientBounds, topLeftPage, zoomFactor);
				PerformDrawHorizontalLine(pen, clientBounds, topLeftPage, zoomFactor);
			}
		}
		void PerformDrawHorizontalLine(Pen pen, Rectangle clientBounds, Page topLeftPage, float zoomFactor) {
			int y = CalculateVerticalPosition(topLeftPage);
			int x2 = CalculateLineWidth(clientBounds, zoomFactor);
			Point point1 = new Point(0, y);
			Point point2 = new Point(x2, y);
			Action defaultDraw = () => { DrawLine(pen, point1, point2); };
			if (RaiseCustomDrawFrozenPaneBorder(point1, point2, pen.Width, pen.Color, defaultDraw))
				return;
			defaultDraw();
		}
		bool RaiseCustomDrawFrozenPaneBorder(Point point1, Point point2, float width, Color color, Action defaultDraw) {
			if (!control.HasCustomDrawFrozenPaneBorder)
				return false;
			CustomDrawFrozenPaneBorderEventArgs args = new CustomDrawFrozenPaneBorderEventArgs(point1, point2, width, color, cache, defaultDraw);
			control.RaiseCustomDrawFrozenPaneBorder(args);
			return args.Handled;
		}
		int CalculateVerticalPosition(Page topLeftPage) {
			return topLeftPage.Bounds.Bottom - 1;
		}
		int CalculateLineWidth(Rectangle clientBounds, float zoomFactor) {
			int width = (int)Math.Round(clientBounds.Width / zoomFactor);
			return UnitConverter.PixelsToLayoutUnits(width, DocumentModel.Dpi);
		}
		void PerformDrawVerticalLine(Pen pen, Rectangle clientBounds, Page topLeftPage, float zoomFactor) {
			int x = CalculateHorizontalPosition(topLeftPage);
			int y2 = CalculateLineHeight(clientBounds, zoomFactor);
			Point point1 = new Point(x, 0);
			Point point2 = new Point(x, y2);
			Action defaultDraw = () => { DrawLine(pen, point1, point2); };
			if (RaiseCustomDrawFrozenPaneBorder(point1, point2, pen.Width, pen.Color, defaultDraw))
				return;
			defaultDraw();
		}
		int CalculateHorizontalPosition(Page topLeftPage) {
			return topLeftPage.Bounds.Right - 1;
		}
		int CalculateLineHeight(Rectangle clientBounds, float zoomFactor) {
			int height = (int)Math.Round(clientBounds.Height / zoomFactor);
			return UnitConverter.PixelsToLayoutUnits(height, DocumentModel.Dpi);
		}
		void DrawLine(Pen pen, Point point1, Point point2) {
			cache.Graphics.DrawLine(pen, point1, point2);
		}
	}
	#endregion
	#region FrozenPanesSeparatorFlatPainter
	public class FrozenPanesSeparatorFlatPainter : FrozenPanesSeparatorPainter {
		public FrozenPanesSeparatorFlatPainter(SpreadsheetControl control)
			: base(control) {
		}
	}
	#endregion
	#region FrozenPanesSeparatorFlatPainter
	public class FrozenPanesSeparatorUltraFlatPainter : FrozenPanesSeparatorPainter {
		public FrozenPanesSeparatorUltraFlatPainter(SpreadsheetControl control)
			: base(control) {
		}
	}
	#endregion
	#region FrozenPanesSeparatorStyle3DPainter
	public class FrozenPanesSeparatorStyle3DPainter : FrozenPanesSeparatorPainter {
		public FrozenPanesSeparatorStyle3DPainter(SpreadsheetControl control)
			: base(control) {
		}
	}
	#endregion
	#region FrozenPanesSeparatorOffice2003Painter
	public class FrozenPanesSeparatorOffice2003Painter : FrozenPanesSeparatorPainter {
		public FrozenPanesSeparatorOffice2003Painter(SpreadsheetControl control)
			: base(control) {
		}
	}
	#endregion
	#region FrozenPanesSeparatorWindowsXPPainter
	public class FrozenPanesSeparatorWindowsXPPainter : FrozenPanesSeparatorPainter {
		public FrozenPanesSeparatorWindowsXPPainter(SpreadsheetControl control)
			: base(control) {
		}
	}
	#endregion
	#region FrozenPanesSeparatorSkinPainter
	public class FrozenPanesSeparatorSkinPainter : FrozenPanesSeparatorPainter {
		#region Fields
		UserLookAndFeel lookAndFeel;
		Color separatorColor;
		#endregion
		public FrozenPanesSeparatorSkinPainter(SpreadsheetControl control, UserLookAndFeel lookAndFeel)
			: base(control) {
			this.lookAndFeel = lookAndFeel;
		}
		#region Properties
		public UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		#endregion
		public override void Initialize() {
			base.Initialize();
			this.separatorColor = SpreadsheetSkins.GetSkin(LookAndFeel).Colors.GetColor(SpreadsheetSkins.ColorFrozenPanesSeparator, Color.Empty);
		}
		protected internal override Color GetSeparatorColor() {
			if (separatorColor != Color.Empty)
				return separatorColor;
			return base.GetSeparatorColor();
		}
	}
	#endregion
}
