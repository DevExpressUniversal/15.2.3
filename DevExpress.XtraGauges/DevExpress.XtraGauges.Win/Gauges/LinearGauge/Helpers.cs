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

using System.Drawing;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Primitive;
namespace DevExpress.XtraGauges.Win.Gauges.Linear {
	static class DesignerElementVisualizerHelpers {
		static Pen centerPen;
		static Pen scalePen;
		static Pen dimPen1;
		static Pen dimPen2;
		static DesignerElementVisualizerHelpers() {
			centerPen = new Pen(Color.FromArgb(230, 255, 0, 0), 0.1f);
			scalePen = new Pen(Color.FromArgb(230, 255, 0, 0), 0.1f);
			dimPen1 = new Pen(Color.FromArgb(230, 0, 0, 255), 0.5f);
			dimPen2 = new Pen(Color.FromArgb(230, 0, 0, 0), 1f);
			scalePen.DashPattern = new float[] { 15, 5, 3, 5 };
			scalePen.DashCap = System.Drawing.Drawing2D.DashCap.Round;
		}
		public static void DrawScaleDesignerElements(Graphics g, ILinearScale scale) {
			if(scale == null || g == null) return;
			float len = (float)MathHelper.CalcVectorLength(scale.StartPoint, scale.EndPoint);
			DrawCross(g, scale.StartPoint, len);
			DrawCross(g, scale.EndPoint, len);
			g.DrawLine(scalePen, scale.StartPoint, scale.EndPoint);
		}
		static void DrawCross(Graphics g, PointF2D pt, float len) {
			PointF2D pt1 = pt - new SizeF(len * 0.05f, len * 0.05f);
			PointF2D pt2 = pt + new SizeF(len * 0.05f, len * 0.05f);
			PointF2D pt3 = pt - new SizeF(len * 0.05f, -len * 0.05f);
			PointF2D pt4 = pt + new SizeF(len * 0.05f, -len * 0.05f);
			g.DrawLine(centerPen, pt1, pt2);
			g.DrawLine(centerPen, pt3, pt4);
		}
	}
}
