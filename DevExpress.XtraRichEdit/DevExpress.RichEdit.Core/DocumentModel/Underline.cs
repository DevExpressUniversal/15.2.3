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
using System.Runtime.InteropServices;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Utils;
using DevExpress.Office.Model;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Model {
	#region IUnderlinePainter
	public interface IUnderlinePainter : IPatternLinePainter<UnderlineType> {
		void DrawUnderline(UnderlineSingle underline, RectangleF bounds, Color color);
		void DrawUnderline(UnderlineDotted underline, RectangleF bounds, Color color);
		void DrawUnderline(UnderlineDashed underline, RectangleF bounds, Color color);
		void DrawUnderline(UnderlineDashSmallGap underline, RectangleF bounds, Color color);
		void DrawUnderline(UnderlineDashDotted underline, RectangleF bounds, Color color);
		void DrawUnderline(UnderlineDashDotDotted underline, RectangleF bounds, Color color);
		void DrawUnderline(UnderlineDouble underline, RectangleF bounds, Color color);
		void DrawUnderline(UnderlineHeavyWave underline, RectangleF bounds, Color color);
		void DrawUnderline(UnderlineLongDashed underline, RectangleF bounds, Color color);
		void DrawUnderline(UnderlineThickSingle underline, RectangleF bounds, Color color);
		void DrawUnderline(UnderlineThickDotted underline, RectangleF bounds, Color color);
		void DrawUnderline(UnderlineThickDashed underline, RectangleF bounds, Color color);
		void DrawUnderline(UnderlineThickDashDotted underline, RectangleF bounds, Color color);
		void DrawUnderline(UnderlineThickDashDotDotted underline, RectangleF bounds, Color color);
		void DrawUnderline(UnderlineThickLongDashed underline, RectangleF bounds, Color color);
		void DrawUnderline(UnderlineDoubleWave underline, RectangleF bounds, Color color);
		void DrawUnderline(UnderlineWave underline, RectangleF bounds, Color color);
	}
	#endregion
	#region ICharacterLinePainter
	public interface ICharacterLinePainter : IUnderlinePainter, IStrikeoutPainter {
	}
	#endregion
	#region Underline (abstract class)
	public abstract class Underline : PatternLine<UnderlineType> {
		class NoneUnderline : Underline {
			public override UnderlineType Id { get { return UnderlineType.None; } }
			public override Rectangle CalcLineBounds(Rectangle r, int thickness) {
				return Rectangle.Empty;
			}
			public override void Draw(IPatternLinePainter<UnderlineType> painter, RectangleF bounds, Color color) {
			}
			public override void Draw(IUnderlinePainter painter, RectangleF bounds, Color color) {
			}
			public override string ToString() {
				return XtraRichEditLocalizer.GetString(XtraRichEditStringId.UnderlineNone);
			}
		}
		static readonly Underline underlineNone = new NoneUnderline();
		public static Underline UnderlineNone { get { return underlineNone; } }
		public override void Draw(IPatternLinePainter<UnderlineType> painter, RectangleF bounds, Color color) {
			IUnderlinePainter underlinePainter = painter as IUnderlinePainter;
			if (underlinePainter != null)
				Draw(underlinePainter, bounds, color);
		}
		public abstract void Draw(IUnderlinePainter painter, RectangleF bounds, Color color);
	}
	#endregion
	#region UnderlineCollection
	public class UnderlineCollection : List<Underline> {
	}
	#endregion
	#region UnderlineRepository
	public class UnderlineRepository : PatternLineRepository<UnderlineType, Underline, UnderlineCollection> {
		protected override void PopulateRepository() {
			RegisterPatternLine(Underline.UnderlineNone);
			RegisterPatternLine(new UnderlineSingle());
			RegisterPatternLine(new UnderlineDotted());
			RegisterPatternLine(new UnderlineDashed());
			RegisterPatternLine(new UnderlineDashDotted());
			RegisterPatternLine(new UnderlineDashDotDotted());
			RegisterPatternLine(new UnderlineDouble());
			RegisterPatternLine(new UnderlineHeavyWave());
			RegisterPatternLine(new UnderlineLongDashed());
			RegisterPatternLine(new UnderlineThickSingle());
			RegisterPatternLine(new UnderlineThickDotted());
			RegisterPatternLine(new UnderlineThickDashed());
			RegisterPatternLine(new UnderlineThickDashDotted());
			RegisterPatternLine(new UnderlineThickDashDotDotted());
			RegisterPatternLine(new UnderlineThickLongDashed());
			RegisterPatternLine(new UnderlineDoubleWave());
			RegisterPatternLine(new UnderlineWave());
			RegisterPatternLine(new UnderlineDashSmallGap());
		}
	}
	#endregion
	#region UnderlineType
	public enum UnderlineType {
		None = 0,
		Single = 1,
		Dotted = 2,
		Dashed = 3,
		DashDotted = 4,
		DashDotDotted = 5,
		Double = 6,
		HeavyWave = 7,
		LongDashed = 8,
		ThickSingle = 9,
		ThickDotted = 10,
		ThickDashed = 11,
		ThickDashDotted = 12,
		ThickDashDotDotted = 13,
		ThickLongDashed = 14,
		DoubleWave = 15,
		Wave = 16,
		DashSmallGap = 17,
	}
	#endregion
	#region UnderlineThinSize (abstract class)
	public abstract class UnderlineThinSize : Underline {
		public override Rectangle CalcLineBounds(Rectangle r, int thickness) {
			return new Rectangle(r.X, r.Bottom - thickness, r.Width, thickness);
		}
	}
	#endregion
	#region UnderlineThickSize (abstract class)
	public abstract class UnderlineThickSize : Underline {
		public override Rectangle CalcLineBounds(Rectangle r, int thickness) {
			return new Rectangle(r.X, r.Y, r.Width, r.Height - thickness / 2);
		}
	}
	#endregion
	#region UnderlineFullSize (abstract class)
	public abstract class UnderlineFullSize : Underline {
		public override Rectangle CalcLineBounds(Rectangle r, int thickness) {
			return r;
		}
	}
	#endregion
	#region UnderlineSingle
	public class UnderlineSingle : UnderlineThinSize {
		public override UnderlineType Id { get { return UnderlineType.Single; } }
		public override void Draw(IUnderlinePainter painter, RectangleF bounds, Color color) {
			painter.DrawUnderline(this, bounds, color);
		}
	}
	#endregion
	#region UnderlineDotted
	public class UnderlineDotted : UnderlineThinSize {
		public override UnderlineType Id { get { return UnderlineType.Dotted; } }
		public override void Draw(IUnderlinePainter painter, RectangleF bounds, Color color) {
			painter.DrawUnderline(this, bounds, color);
		}
	}
	#endregion
	#region UnderlineDashed
	public class UnderlineDashed : UnderlineThinSize {
		public override UnderlineType Id { get { return UnderlineType.Dashed; } }
		public override void Draw(IUnderlinePainter painter, RectangleF bounds, Color color) {
			painter.DrawUnderline(this, bounds, color);
		}
	}
	#endregion
	#region UnderlineDashSmallGap
	public class UnderlineDashSmallGap : UnderlineThinSize {
		public override UnderlineType Id { get { return UnderlineType.DashSmallGap; } }
		public override void Draw(IUnderlinePainter painter, RectangleF bounds, Color color) {
			painter.DrawUnderline(this, bounds, color);
		}
	}
	#endregion
	#region UnderlineDashDotted
	public class UnderlineDashDotted : UnderlineThinSize {
		public override UnderlineType Id { get { return UnderlineType.DashDotted; } }
		public override void Draw(IUnderlinePainter painter, RectangleF bounds, Color color) {
			painter.DrawUnderline(this, bounds, color);
		}
	}
	#endregion
	#region UnderlineDashDotDotted
	public class UnderlineDashDotDotted : UnderlineThinSize {
		public override UnderlineType Id { get { return UnderlineType.DashDotDotted; } }
		public override void Draw(IUnderlinePainter painter, RectangleF bounds, Color color) {
			painter.DrawUnderline(this, bounds, color);
		}
	}
	#endregion
	#region UnderlineDouble
	public class UnderlineDouble : UnderlineFullSize {
		public override UnderlineType Id { get { return UnderlineType.Double; } }
		public override float CalcLinePenVerticalOffset(RectangleF lineBounds) {
			return 0;
		}
		public override void Draw(IUnderlinePainter painter, RectangleF bounds, Color color) {
			painter.DrawUnderline(this, bounds, color);
		}
	}
	#endregion
	#region UnderlineHeavyWave
	public class UnderlineHeavyWave : UnderlineFullSize {
		public override UnderlineType Id { get { return UnderlineType.HeavyWave; } }
		public override float CalcLinePenVerticalOffset(RectangleF lineBounds) {
			return 0;
		}
		public override void Draw(IUnderlinePainter painter, RectangleF bounds, Color color) {
			painter.DrawUnderline(this, bounds, color);
		}
	}
	#endregion
	#region UnderlineLongDashed
	public class UnderlineLongDashed : UnderlineThinSize {
		public override UnderlineType Id { get { return UnderlineType.LongDashed; } }
		public override void Draw(IUnderlinePainter painter, RectangleF bounds, Color color) {
			painter.DrawUnderline(this, bounds, color);
		}
	}
	#endregion
	#region UnderlineThickSingle
	public class UnderlineThickSingle : UnderlineThickSize {
		public override UnderlineType Id { get { return UnderlineType.ThickSingle; } }
		public override void Draw(IUnderlinePainter painter, RectangleF bounds, Color color) {
			painter.DrawUnderline(this, bounds, color);
		}
	}
	#endregion
	#region UnderlineThickDotted
	public class UnderlineThickDotted : UnderlineThickSize {
		public override UnderlineType Id { get { return UnderlineType.ThickDotted; } }
		public override void Draw(IUnderlinePainter painter, RectangleF bounds, Color color) {
			painter.DrawUnderline(this, bounds, color);
		}
	}
	#endregion
	#region UnderlineThickDashed
	public class UnderlineThickDashed : UnderlineThickSize {
		public override UnderlineType Id { get { return UnderlineType.ThickDashed; } }
		public override void Draw(IUnderlinePainter painter, RectangleF bounds, Color color) {
			painter.DrawUnderline(this, bounds, color);
		}
	}
	#endregion
	#region UnderlineThickDashDotted
	public class UnderlineThickDashDotted : UnderlineThickSize {
		public override UnderlineType Id { get { return UnderlineType.ThickDashDotted; } }
		public override void Draw(IUnderlinePainter painter, RectangleF bounds, Color color) {
			painter.DrawUnderline(this, bounds, color);
		}
	}
	#endregion
	#region UnderlineThickDashDotDotted
	public class UnderlineThickDashDotDotted : UnderlineThickSize {
		public override UnderlineType Id { get { return UnderlineType.ThickDashDotDotted; } }
		public override void Draw(IUnderlinePainter painter, RectangleF bounds, Color color) {
			painter.DrawUnderline(this, bounds, color);
		}
	}
	#endregion
	#region UnderlineThickLongDashed
	public class UnderlineThickLongDashed : UnderlineThickSize {
		public override UnderlineType Id { get { return UnderlineType.ThickLongDashed; } }
		public override void Draw(IUnderlinePainter painter, RectangleF bounds, Color color) {
			painter.DrawUnderline(this, bounds, color);
		}
	}
	#endregion
	#region UnderlineDoubleWave
	public class UnderlineDoubleWave : UnderlineFullSize {
		public override UnderlineType Id { get { return UnderlineType.DoubleWave; } }
		public override float CalcLinePenVerticalOffset(RectangleF lineBounds) {
			return 0;
		}
		public override void Draw(IUnderlinePainter painter, RectangleF bounds, Color color) {
			painter.DrawUnderline(this, bounds, color);
		}
	}
	#endregion
	#region UnderlineWave
	public class UnderlineWave : UnderlineFullSize {
		public override UnderlineType Id { get { return UnderlineType.Wave; } }
		public override float CalcLinePenVerticalOffset(RectangleF lineBounds) {
			return 0;
		}
		public override void Draw(IUnderlinePainter painter, RectangleF bounds, Color color) {
			painter.DrawUnderline(this, bounds, color);
		}
	}
	#endregion
}
