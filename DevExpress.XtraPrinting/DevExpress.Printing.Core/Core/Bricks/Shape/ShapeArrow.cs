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
	public class ShapeArrow : FilletShapeBase {
		int arrowWidth = 50;
		int arrowHeight = 50;
		internal override PreviewStringId ShapeStringId {
			get { return PreviewStringId.Shapes_Arrow; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ShapeArrowArrowWidth"),
#endif
 DefaultValue(50),
	   DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.Shape.ShapeArrow.ArrowWidth"),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public int ArrowWidth {
			get { return arrowWidth; }
			set { arrowWidth = ShapeHelper.ValidatePercentageValue(value, "ArrowWidth"); }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ShapeArrowArrowHeight"),
#endif
 DefaultValue(50),
	   DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.Shape.ShapeArrow.ArrowHeight"),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public int ArrowHeight {
			get { return arrowHeight; }
			set { arrowHeight = ShapeHelper.ValidatePercentageValue(value, "ArrowHeight"); }
		}
		public ShapeArrow() {
		}
		ShapeArrow(ShapeArrow source)
			: base(source) {
			arrowHeight = source.ArrowHeight;
			arrowWidth = source.ArrowWidth;
		}
		protected override ShapeBase CloneShape() {
			return new ShapeArrow(this);
		}
		protected internal override PointF[] CreatePoints(RectangleF bounds, int angle) {
			PointF clientBoundsCenter = RectHelper.CenterOf(bounds);
			double angleInRad = Math.PI * angle / 180;
			float height = (float)(bounds.Height * Math.Abs(Math.Cos(angleInRad)) + bounds.Width * Math.Abs(Math.Sin(angleInRad)));
			float width = (float)(bounds.Width * Math.Abs(Math.Cos(angleInRad)) + bounds.Height * Math.Abs(Math.Sin(angleInRad)));
			PointF imageCenter = new PointF(bounds.X + width / 2, bounds.Y + height / 2);
			bounds.Offset(clientBoundsCenter.X - imageCenter.X, clientBoundsCenter.Y - imageCenter.Y);
			float arrowHeaderHeight = height * ShapeHelper.PercentsToRatio(arrowHeight);
			float arrowBottom = bounds.Y + height;
			float arrowTaleWidth = width * ShapeHelper.PercentsToRatio(arrowWidth);
			float arrowTaleTop = bounds.Y + arrowHeaderHeight;
			float arrowTaleRight = bounds.X + (width + arrowTaleWidth) / 2;
			float arrowTaleLeft = bounds.X + (width - arrowTaleWidth) / 2;
			return new PointF[] {   
									new PointF(bounds.X, arrowTaleTop),
									new PointF(bounds.X + width / 2, bounds.Y),
									new PointF(bounds.X + width, arrowTaleTop),
									new PointF(arrowTaleRight, arrowTaleTop),
									new PointF(arrowTaleRight, arrowBottom),
									new PointF(arrowTaleLeft, arrowBottom),
									new PointF(arrowTaleLeft, arrowTaleTop)
								};
		}
	}
}
