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
using System.Text;
using System.Drawing;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Base;
namespace DevExpress.XtraGauges.Win.Gauges.Digital {
	static class DesignerElementVisualizerHelpers {
		static Pen pointPen;
		static Pen boundsPen;
		static DesignerElementVisualizerHelpers() {
			pointPen = new Pen(Color.FromArgb(230, 255, 0, 0), 0.1f);
			boundsPen = new Pen(Color.FromArgb(230, 255, 0, 0), 0.1f);
			boundsPen.DashPattern = new float[] { 15, 5, 3, 5 };
			boundsPen.DashCap = System.Drawing.Drawing2D.DashCap.Round;
		}
		public static void DrawDesignerElements<T>(Graphics g, IDigitalLayer<T> layer) {
			float len = (float)MathHelper.CalcVectorLength(layer.TopLeft, layer.BottomRight);
			DrawCross(g, layer.TopLeft, len);
			DrawCross(g, layer.BottomRight, len);
			g.DrawRectangle(boundsPen, layer.TopLeft.X, layer.TopLeft.Y, layer.BottomRight.X - layer.TopLeft.X, layer.BottomRight.Y - layer.TopLeft.Y);
		}
		static void DrawCross(Graphics g, PointF2D pt, float len) {
			PointF2D pt1 = pt - new SizeF(len * 0.05f, len * 0.05f);
			PointF2D pt2 = pt + new SizeF(len * 0.05f, len * 0.05f);
			PointF2D pt3 = pt - new SizeF(len * 0.05f, -len * 0.05f);
			PointF2D pt4 = pt + new SizeF(len * 0.05f, -len * 0.05f);
			g.DrawLine(pointPen, pt1, pt2);
			g.DrawLine(pointPen, pt3, pt4);
		}
	}
}
