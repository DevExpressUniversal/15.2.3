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
using System.Drawing;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office.Model;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Layout {
	#region IBorderLinePainter
	public interface IBorderLinePainter : IPatternLinePainter<XlBorderLineStyle> {
		void DrawBorderLine(BorderLineThin border, RectangleF bounds, Color color);
		void DrawBorderLine(BorderLineMedium border, RectangleF bounds, Color color);
		void DrawBorderLine(BorderLineDashed border, RectangleF bounds, Color color);
		void DrawBorderLine(BorderLineDotted border, RectangleF bounds, Color color);
		void DrawBorderLine(BorderLineThick border, RectangleF bounds, Color color);
		void DrawBorderLine(BorderLineDouble border, RectangleF bounds, Color color);
		void DrawBorderLine(BorderLineHair border, RectangleF bounds, Color color);
		void DrawBorderLine(BorderLineMediumDashed border, RectangleF bounds, Color color);
		void DrawBorderLine(BorderLineDashDot border, RectangleF bounds, Color color);
		void DrawBorderLine(BorderLineMediumDashDot border, RectangleF bounds, Color color);
		void DrawBorderLine(BorderLineDashDotDot border, RectangleF bounds, Color color);
		void DrawBorderLine(BorderLineMediumDashDotDot border, RectangleF bounds, Color color);
		void DrawBorderLine(BorderLineSlantDashDot border, RectangleF bounds, Color color);
		void DrawBorderLine(BorderLineThin border, PointF from, PointF to, Color color, float thickness);
		void DrawBorderLine(BorderLineMedium border, PointF from, PointF to, Color color, float thickness);
		void DrawBorderLine(BorderLineDashed border, PointF from, PointF to, Color color, float thickness);
		void DrawBorderLine(BorderLineDotted border, PointF from, PointF to, Color color, float thickness);
		void DrawBorderLine(BorderLineThick border, PointF from, PointF to, Color color, float thickness);
		void DrawBorderLine(BorderLineDouble border, PointF from, PointF to, Color color, float thickness);
		void DrawBorderLine(BorderLineHair border, PointF from, PointF to, Color color, float thickness);
		void DrawBorderLine(BorderLineMediumDashed border, PointF from, PointF to, Color color, float thickness);
		void DrawBorderLine(BorderLineDashDot border, PointF from, PointF to, Color color, float thickness);
		void DrawBorderLine(BorderLineMediumDashDot border, PointF from, PointF to, Color color, float thickness);
		void DrawBorderLine(BorderLineDashDotDot border, PointF from, PointF to, Color color, float thickness);
		void DrawBorderLine(BorderLineMediumDashDotDot border, PointF from, PointF to, Color color, float thickness);
		void DrawBorderLine(BorderLineSlantDashDot border, PointF from, PointF to, Color color, float thickness);
	}
	#endregion
	#region BorderLine (abstract class)
	public abstract class BorderLine : PatternLine<XlBorderLineStyle> {
		#region BorderLineNone
		class BorderLineNone : BorderLine {
			public override XlBorderLineStyle Id { get { return XlBorderLineStyle.None; } }
			public override Rectangle CalcLineBounds(Rectangle r, int thickness) {
				return Rectangle.Empty;
			}
			public override void Draw(IPatternLinePainter<XlBorderLineStyle> painter, RectangleF bounds, Color color) {
			}
			public override void Draw(IBorderLinePainter painter, RectangleF bounds, Color color) {
			}
			public override void Draw(IBorderLinePainter painter, PointF from, PointF to, Color color, float thickness) {
			}
		}
		#endregion
		static readonly BorderLineNone none = new BorderLineNone();
		public static BorderLine None { get { return none; } }
		public override Rectangle CalcLineBounds(Rectangle r, int thickness) {
			return r;
		}
		public override void Draw(IPatternLinePainter<XlBorderLineStyle> painter, RectangleF bounds, Color color) {
			IBorderLinePainter borderLinePainter = painter as IBorderLinePainter;
			if (borderLinePainter != null)
				Draw(borderLinePainter, bounds, color);
		}
		public abstract void Draw(IBorderLinePainter painter, RectangleF bounds, Color color);
		public abstract void Draw(IBorderLinePainter painter, PointF from, PointF to, Color color, float thickness);
	}
	#endregion
	#region BorderLineCollection
	public class BorderLineCollection : List<BorderLine> {
	}
	#endregion
	#region BorderLineRepository
	public class BorderLineRepository : PatternLineRepository<XlBorderLineStyle, BorderLine, BorderLineCollection> {
		protected override void PopulateRepository() {
			RegisterPatternLine(BorderLine.None);
			RegisterPatternLine(new BorderLineThin());
			RegisterPatternLine(new BorderLineMedium());
			RegisterPatternLine(new BorderLineDashed());
			RegisterPatternLine(new BorderLineDotted());
			RegisterPatternLine(new BorderLineThick());
			RegisterPatternLine(new BorderLineDouble());
			RegisterPatternLine(new BorderLineHair());
			RegisterPatternLine(new BorderLineMediumDashed());
			RegisterPatternLine(new BorderLineDashDot());
			RegisterPatternLine(new BorderLineMediumDashDot());
			RegisterPatternLine(new BorderLineDashDotDot());
			RegisterPatternLine(new BorderLineMediumDashDotDot());
			RegisterPatternLine(new BorderLineSlantDashDot());
			RegisterPatternLine(new BorderLineForcedNone());
			RegisterPatternLine(new BorderLineNoneOverrideDefaultGrid());
			RegisterPatternLine(new BorderLineDefaultGrid());
			RegisterPatternLine(new BorderLinePrintGrid());
			RegisterPatternLine(new BorderLineComplexInside());
			RegisterPatternLine(new BorderLineComplexOutside());
		}
	}
	#endregion
	#region BorderLineThin
	public class BorderLineThin : BorderLine {
		public override XlBorderLineStyle Id { get { return XlBorderLineStyle.Thin; } }
		public override void Draw(IBorderLinePainter painter, RectangleF bounds, Color color) {
			painter.DrawBorderLine(this, bounds, color);
		}
		public override void Draw(IBorderLinePainter painter, PointF from, PointF to, Color color, float thickness) {
			painter.DrawBorderLine(this, from, to, color, thickness);
		}
	}
	#endregion
	#region BorderLineMedium
	public class BorderLineMedium : BorderLine {
		public override XlBorderLineStyle Id { get { return XlBorderLineStyle.Medium; } }
		public override void Draw(IBorderLinePainter painter, RectangleF bounds, Color color) {
			painter.DrawBorderLine(this, bounds, color);
		}
		public override void Draw(IBorderLinePainter painter, PointF from, PointF to, Color color, float thickness) {
			painter.DrawBorderLine(this, from, to, color, thickness);
		}
	}
	#endregion
	#region BorderLineDashed
	public class BorderLineDashed : BorderLine {
		public override XlBorderLineStyle Id { get { return XlBorderLineStyle.Dashed; } }
		public override void Draw(IBorderLinePainter painter, RectangleF bounds, Color color) {
			painter.DrawBorderLine(this, bounds, color);
		}
		public override void Draw(IBorderLinePainter painter, PointF from, PointF to, Color color, float thickness) {
			painter.DrawBorderLine(this, from, to, color, thickness);
		}
	}
	#endregion
	#region BorderLineDotted
	public class BorderLineDotted : BorderLine {
		public override XlBorderLineStyle Id { get { return XlBorderLineStyle.Dotted; } }
		public override void Draw(IBorderLinePainter painter, RectangleF bounds, Color color) {
			painter.DrawBorderLine(this, bounds, color);
		}
		public override void Draw(IBorderLinePainter painter, PointF from, PointF to, Color color, float thickness) {
			painter.DrawBorderLine(this, from, to, color, thickness);
		}
	}
	#endregion
	#region BorderLineThick
	public class BorderLineThick : BorderLine {
		public override XlBorderLineStyle Id { get { return XlBorderLineStyle.Thick; } }
		public override void Draw(IBorderLinePainter painter, RectangleF bounds, Color color) {
			painter.DrawBorderLine(this, bounds, color);
		}
		public override void Draw(IBorderLinePainter painter, PointF from, PointF to, Color color, float thickness) {
			painter.DrawBorderLine(this, from, to, color, thickness);
		}
	}
	#endregion
	#region BorderLineDouble
	public class BorderLineDouble : BorderLine {
		public override XlBorderLineStyle Id { get { return XlBorderLineStyle.Double; } }
		public override void Draw(IBorderLinePainter painter, RectangleF bounds, Color color) {
			painter.DrawBorderLine(this, bounds, color);
		}
		public override void Draw(IBorderLinePainter painter, PointF from, PointF to, Color color, float thickness) {
			painter.DrawBorderLine(this, from, to, color, thickness);
		}
	}
	#endregion
	#region BorderLineHair
	public class BorderLineHair : BorderLine {
		public override XlBorderLineStyle Id { get { return XlBorderLineStyle.Hair; } }
		public override void Draw(IBorderLinePainter painter, RectangleF bounds, Color color) {
			painter.DrawBorderLine(this, bounds, color);
		}
		public override void Draw(IBorderLinePainter painter, PointF from, PointF to, Color color, float thickness) {
			painter.DrawBorderLine(this, from, to, color, thickness);
		}
	}
	#endregion
	#region BorderLineMediumDashed
	public class BorderLineMediumDashed : BorderLine {
		public override XlBorderLineStyle Id { get { return XlBorderLineStyle.MediumDashed; } }
		public override void Draw(IBorderLinePainter painter, RectangleF bounds, Color color) {
			painter.DrawBorderLine(this, bounds, color);
		}
		public override void Draw(IBorderLinePainter painter, PointF from, PointF to, Color color, float thickness) {
			painter.DrawBorderLine(this, from, to, color, thickness);
		}
	}
	#endregion
	#region BorderLineDashDot
	public class BorderLineDashDot : BorderLine {
		public override XlBorderLineStyle Id { get { return XlBorderLineStyle.DashDot; } }
		public override void Draw(IBorderLinePainter painter, RectangleF bounds, Color color) {
			painter.DrawBorderLine(this, bounds, color);
		}
		public override void Draw(IBorderLinePainter painter, PointF from, PointF to, Color color, float thickness) {
			painter.DrawBorderLine(this, from, to, color, thickness);
		}
	}
	#endregion
	#region BorderLineMediumDashDot
	public class BorderLineMediumDashDot : BorderLine {
		public override XlBorderLineStyle Id { get { return XlBorderLineStyle.MediumDashDot; } }
		public override void Draw(IBorderLinePainter painter, RectangleF bounds, Color color) {
			painter.DrawBorderLine(this, bounds, color);
		}
		public override void Draw(IBorderLinePainter painter, PointF from, PointF to, Color color, float thickness) {
			painter.DrawBorderLine(this, from, to, color, thickness);
		}
	}
	#endregion
	#region BorderLineDashDotDot
	public class BorderLineDashDotDot : BorderLine {
		public override XlBorderLineStyle Id { get { return XlBorderLineStyle.DashDotDot; } }
		public override void Draw(IBorderLinePainter painter, RectangleF bounds, Color color) {
			painter.DrawBorderLine(this, bounds, color);
		}
		public override void Draw(IBorderLinePainter painter, PointF from, PointF to, Color color, float thickness) {
			painter.DrawBorderLine(this, from, to, color, thickness);
		}
	}
	#endregion
	#region BorderLineMediumDashDotDot
	public class BorderLineMediumDashDotDot : BorderLine {
		public override XlBorderLineStyle Id { get { return XlBorderLineStyle.MediumDashDotDot; } }
		public override void Draw(IBorderLinePainter painter, RectangleF bounds, Color color) {
			painter.DrawBorderLine(this, bounds, color);
		}
		public override void Draw(IBorderLinePainter painter, PointF from, PointF to, Color color, float thickness) {
			painter.DrawBorderLine(this, from, to, color, thickness);
		}
	}
	#endregion
	#region BorderLineSlantDashDot
	public class BorderLineSlantDashDot : BorderLine {
		public override XlBorderLineStyle Id { get { return XlBorderLineStyle.SlantDashDot; } }
		public override void Draw(IBorderLinePainter painter, RectangleF bounds, Color color) {
			painter.DrawBorderLine(this, bounds, color);
		}
		public override void Draw(IBorderLinePainter painter, PointF from, PointF to, Color color, float thickness) {
			painter.DrawBorderLine(this, from, to, color, thickness);
		}
	}
	#endregion
	#region BorderLineForcedNone
	public class BorderLineForcedNone : BorderLine {
		public override XlBorderLineStyle Id { get { return SpecialBorderLineStyle.ForcedNone; } }
		public override void Draw(IPatternLinePainter<XlBorderLineStyle> painter, RectangleF bounds, Color color) {
		}
		public override void Draw(IBorderLinePainter painter, RectangleF bounds, Color color) {
		}
		public override void Draw(IBorderLinePainter painter, PointF from, PointF to, Color color, float thickness) {
		}
	}
	#endregion
	#region BorderLineNoneOverrideDefaultGrid
	public class BorderLineNoneOverrideDefaultGrid : BorderLine {
		public override XlBorderLineStyle Id { get { return SpecialBorderLineStyle.NoneOverrideDefaultGrid; } }
		public override void Draw(IPatternLinePainter<XlBorderLineStyle> painter, RectangleF bounds, Color color) {
		}
		public override void Draw(IBorderLinePainter painter, RectangleF bounds, Color color) {
		}
		public override void Draw(IBorderLinePainter painter, PointF from, PointF to, Color color, float thickness) {
		}
	}
	#endregion
	#region BorderLineDefaultGrid
	public class BorderLineDefaultGrid : BorderLineThin {
		public override XlBorderLineStyle Id { get { return SpecialBorderLineStyle.DefaultGrid; } }
		public override void Draw(IBorderLinePainter painter, RectangleF bounds, Color color) {
			painter.DrawBorderLine(this, bounds, color);
		}
		public override void Draw(IBorderLinePainter painter, PointF from, PointF to, Color color, float thickness) {
			painter.DrawBorderLine(this, from, to, color, thickness);
		}
	}
	#endregion
	#region BorderLinePrintGrid
	public class BorderLinePrintGrid : BorderLineDotted {
		public override XlBorderLineStyle Id { get { return SpecialBorderLineStyle.PrintGrid; } }
		public override void Draw(IBorderLinePainter painter, RectangleF bounds, Color color) {
			painter.DrawBorderLine(this, bounds, color);
		}
		public override void Draw(IBorderLinePainter painter, PointF from, PointF to, Color color, float thickness) {
			painter.DrawBorderLine(this, from, to, color, thickness);
		}
	}
	#endregion
	#region BorderLineComplexInside
	public class BorderLineComplexInside : BorderLineThin {
		public override XlBorderLineStyle Id { get { return SpecialBorderLineStyle.InsideComplexBorder; } }
		public override void Draw(IBorderLinePainter painter, RectangleF bounds, Color color) {
			painter.DrawBorderLine(this, bounds, color);
		}
		public override void Draw(IBorderLinePainter painter, PointF from, PointF to, Color color, float thickness) {
			painter.DrawBorderLine(this, from, to, color, thickness);
		}
	}
	#endregion
	#region BorderLineComplexOutside
	public class BorderLineComplexOutside : BorderLineThin {
		public override XlBorderLineStyle Id { get { return SpecialBorderLineStyle.OutsideComplexBorder; } }
		public override void Draw(IBorderLinePainter painter, RectangleF bounds, Color color) {
			painter.DrawBorderLine(this, bounds, color);
		}
		public override void Draw(IBorderLinePainter painter, PointF from, PointF to, Color color, float thickness) {
			painter.DrawBorderLine(this, from, to, color, thickness);
		}
	}
	#endregion
}
