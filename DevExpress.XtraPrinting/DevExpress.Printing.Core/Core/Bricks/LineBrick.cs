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

using DevExpress.XtraPrinting.Native;
using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraReports.UI;
#if SL
using System.Windows.Media;
using DevExpress.Xpf.Drawing.Drawing2D;
using DevExpress.Xpf.Windows.Forms;
using DevExpress.Xpf.Drawing;
#else
using System.Drawing.Drawing2D;
using DevExpress.XtraPrinting.BrickExporters;
#endif
namespace DevExpress.XtraPrinting {
	public enum HtmlLineDirection { Default, Vertical, Horizontal }
#if !SL
	[
	BrickExporter(typeof(LineBrickExporter))
	]
#endif
	public class LineBrick : VisualBrick, ILineBrick {
		internal static LineDirection GetDirection(PointF pt1, PointF pt2) {
			LineDirection direction = LineDirection.Slant;
			if(pt1.X == pt2.X)
				direction = LineDirection.Vertical;
			else if(pt1.Y == pt2.Y)
				direction = LineDirection.Horizontal;
			else if((pt1.Y - pt2.Y) * (pt1.X - pt2.X) > 0)
				direction = LineDirection.BackSlant;
			return direction;
		}
		private DashStyle lineStyle = DashStyle.Solid;
		private float width = GraphicsUnitConverter.PixelToDoc(1);
		HtmlLineDirection htmlLineDirection = HtmlLineDirection.Default;
		LineDirection lineDirection = LineDirection.Horizontal;
		#region hidden properties
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string Text { get { return base.Text; } set { base.Text = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override object TextValue { get { return base.TextValue; } set { base.TextValue = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string TextValueFormatString { get { return base.TextValueFormatString; } set { base.TextValueFormatString = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string XlsxFormatString { get { return base.XlsxFormatString; } set { base.XlsxFormatString = value; } }
		#endregion
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("LineBrickLineDirection"),
#endif
		XtraSerializableProperty,
		DefaultValue(LineDirection.Horizontal),
		]
		public LineDirection LineDirection { get { return lineDirection; } set { lineDirection = value; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("LineBrickForeColor")]
#endif
		public Color ForeColor { get { return Style.ForeColor; } set { Style = BrickStyleHelper.ChangeForeColor(Style, value); } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("LineBrickLineStyle"),
#endif
		XtraSerializableProperty,
		DefaultValue(DashStyle.Solid),
		]
		public DashStyle LineStyle { get { return lineStyle; } set { lineStyle = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("LineBrickLineWidth"),
#endif
		XtraSerializableProperty,
		]
		public float LineWidth {
			get { return GraphicsUnitConverter.DocToPixel(width); }
			set { width = GraphicsUnitConverter.PixelToDoc(value); }
		}
		internal float DocWidth { get { return width; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("LineBrickHtmlLineDirection"),
#endif
		XtraSerializableProperty,
		DefaultValue(HtmlLineDirection.Default),
		]
		public HtmlLineDirection HtmlLineDirection { get { return htmlLineDirection; } set { htmlLineDirection = value; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("LineBrickBrickType")]
#endif
		public override string BrickType { get { return BrickTypes.Line; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("LineBrickNoClip")]
#endif
		public override bool NoClip {
			get { return true; }
		}
		internal bool ShouldExportAsImage {
			get {
				return LineStyle != DashStyle.Solid || LineDirection == LineDirection.Slant || LineDirection == LineDirection.BackSlant;
			}
		}
		public LineBrick(IBrickOwner brickOwner) 
			: base(brickOwner) { 
		}
		public LineBrick()
			: this(NullBrickOwner.Instance) {
		}
		internal LineBrick(PrintingSystemBase ps, PointF pt1, PointF pt2, float width)
			: base() {
			PrintingSystem = ps;
			this.width = width;
			InitialRect = RectF.FromPoints(pt1, pt2);
			if(base.Width > base.Height)
				base.Height = Math.Max(base.Height, width);
			else
				base.Width = Math.Max(base.Width, width);
			this.lineDirection = GetDirection(pt1, pt2);
		}
		LineBrick(LineBrick lineBrick) : base(lineBrick) {
			LineStyle = lineBrick.LineStyle;
			LineWidth = lineBrick.LineWidth;
			LineDirection = lineBrick.LineDirection;
			HtmlLineDirection = lineBrick.HtmlLineDirection;
		}
		public override object Clone() {
			return new LineBrick(this);
		}
		#region ILineBrick Members
		void ILineBrick.CalculateDirection(PointF pt1, PointF pt2) {			
			LineDirection = GetDirection(pt1, pt2);
		}
		#endregion
		protected override float ValidatePageBottomInternal(float pageBottom, RectangleF rect, IPrintingSystemContext context) {
			return pageBottom;
		}
		internal PointF GetPoint1(RectangleF rect) {
			if(lineDirection == LineDirection.Horizontal)
				return new PointF(rect.Left, rect.Top + rect.Height / 2);
			if(lineDirection == LineDirection.Vertical)
				return new PointF(rect.Left + rect.Width / 2, rect.Top);
			if(lineDirection == LineDirection.Slant)
				return new PointF(rect.Left, rect.Bottom);
			return new PointF(rect.Left, rect.Top);
		}
		internal PointF GetPoint2(RectangleF rect) {
			if(lineDirection == LineDirection.Horizontal)
				return new PointF(rect.Right, rect.Top + rect.Height / 2);
			if(lineDirection == LineDirection.Vertical)
				return new PointF(rect.Left + rect.Width / 2, rect.Bottom);
			if(lineDirection == LineDirection.Slant)
				return new PointF(rect.Right, rect.Top);
			return new PointF(rect.Right, rect.Bottom);
		}
		protected internal override void Scale(double scaleFactor) {
			base.Scale(scaleFactor);
			width = MathMethods.Scale(width, scaleFactor);
		}
	}
}
