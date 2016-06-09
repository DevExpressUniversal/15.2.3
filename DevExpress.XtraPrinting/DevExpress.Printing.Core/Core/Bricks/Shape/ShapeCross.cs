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
using System.Drawing;
using DevExpress.XtraPrinting.Shape.Native;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.Localization;
namespace DevExpress.XtraPrinting.Shape {
	public class ShapeCross : FilletShapeBase {
		#region fields & properties
		int verticalLineWidth = 50;
		int horizontalLineWidth = 50;
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ShapeCrossVerticalLineWidth"),
#endif
	   DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.Shape.ShapeCross.VerticalLineWidth"),
		NotifyParentProperty(true),
		RefreshProperties(RefreshProperties.All),
		DefaultValue(50),
		XtraSerializableProperty,
		]
		public int VerticalLineWidth {
			get { return verticalLineWidth; }
			set {
				verticalLineWidth = ShapeHelper.ValidatePercentageValue(value, "verticalLineWidth");
			}
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ShapeCrossHorizontalLineWidth"),
#endif
	   DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.Shape.ShapeCross.HorizontalLineWidth"),
		NotifyParentProperty(true),
		RefreshProperties(RefreshProperties.All),
		DefaultValue(50),
		XtraSerializableProperty,
		]
		public int HorizontalLineWidth {
			get { return horizontalLineWidth; }
			set {
				horizontalLineWidth = ShapeHelper.ValidatePercentageValue(value, "horizontalLineWidth");
			}
		}
		internal override PreviewStringId ShapeStringId {
			get { return PreviewStringId.Shapes_Cross; }
		}
		#endregion
		public ShapeCross() {
		}
		ShapeCross(ShapeCross source)
			: base(source) {
			horizontalLineWidth = source.HorizontalLineWidth;
			verticalLineWidth = source.VerticalLineWidth;
		}
		protected override ShapeBase CloneShape() {
			return new ShapeCross(this);
		}
		protected internal override PointF[] CreatePoints(RectangleF bounds, int angle) {
			PointF centerPoint = RectHelper.CenterOf(bounds);
			float verticalWidth = (float)Math.Round(bounds.Width * verticalLineWidth / 200);
			float horizontalWidth = (float)Math.Round(bounds.Height * horizontalLineWidth / 200);
			RectangleF verticalLine = new RectangleF(new PointF(centerPoint.X - verticalWidth, bounds.Y), new SizeF(2 * verticalWidth, bounds.Height));
			RectangleF horizontalLine = new RectangleF(new PointF(bounds.X, centerPoint.Y - horizontalWidth), new SizeF(bounds.Width, 2 * horizontalWidth));
			RectangleF center = RectangleF.FromLTRB(verticalLine.Left, horizontalLine.Top, verticalLine.Right, horizontalLine.Bottom);
			return new PointF[] {
									verticalLine.Location,
									new PointF(verticalLine.Right, verticalLine.Top),
									new PointF(center.Right, center.Top),
									new PointF(horizontalLine.Right, horizontalLine.Top),
									new PointF(horizontalLine.Right, horizontalLine.Bottom),
									new PointF(center.Right, center.Bottom),
									new PointF(verticalLine.Right, verticalLine.Bottom),
									new PointF(verticalLine.Left, verticalLine.Bottom),
									new PointF(center.Left, center.Bottom),
									new PointF(horizontalLine.Left, horizontalLine.Bottom),
									horizontalLine.Location,
									center.Location
			};
		}
	}
}
