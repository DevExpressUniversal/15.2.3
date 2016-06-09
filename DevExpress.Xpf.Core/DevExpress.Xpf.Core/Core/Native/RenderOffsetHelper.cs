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
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Xpf.Core.Native {
	public class RenderOffsetHelper {
		public static Point GetCorrectedRenderOffset(UIElement element, UIElement topElement) {
			Rect r = LayoutHelper.GetRelativeElementRect(element, topElement);
			double xOffset = r.Left - Math.Floor(r.Left);
			double yOffset = r.Top - Math.Floor(r.Top);
			if (xOffset > 0.5)
				xOffset = 1.0 - xOffset;
			else
				xOffset = -xOffset;
			if (yOffset > 0.5)
				yOffset = 1.0 - yOffset;
			else
				yOffset = -yOffset;
			return new Point(xOffset, yOffset);
		}
#if !SL
		static bool IsRotatedLeft(Matrix mat) {
			return mat.M12 == -1 && mat.M21 == 1;
		}
		static bool IsRotatedRight(Matrix mat) {
			return mat.M12 == 1 && mat.M21 == -1;
		}
		static Point ApplyLeftRotation(Point pt) {
			return new Point(-pt.Y, pt.X);
		}
		static Point ApplyRightRotation(Point pt) {
			return new Point(pt.Y, -pt.X);
		}
		public static void UpdateRenderOffset(UIElement element, ref Point prevOffset) {
			UIElement topElement = LayoutHelper.GetTopLevelVisual(element);
			if (topElement == null)
				return;
			MatrixTransform mt = element.RenderTransform as MatrixTransform;
			if (element.RenderTransform != MatrixTransform.Identity) {
				if (mt != null && !mt.IsSealed)
					mt.Matrix = new Matrix(mt.Matrix.M11, mt.Matrix.M12, mt.Matrix.M21, mt.Matrix.M22, mt.Matrix.OffsetX - prevOffset.X, mt.Matrix.OffsetY - prevOffset.Y);
			}
			Point pt = GetCorrectedRenderOffset(element, topElement);
			if (Double.IsNaN(pt.X) || Double.IsNaN(pt.Y))
				return;
			if (pt.X != 0 || pt.Y != 0) {
				MatrixTransform gmt = element.TransformToVisual(topElement) as MatrixTransform;
				if(gmt != null) {
					if(IsRotatedLeft(gmt.Matrix)) {
						pt = ApplyLeftRotation(pt);
					} else if(IsRotatedRight(gmt.Matrix)) {
						pt = ApplyRightRotation(pt);
					}
					if(element.RenderTransform == MatrixTransform.Identity)
						element.RenderTransform = new MatrixTransform(new Matrix(1, 0, 0, 1, pt.X, pt.Y));
					else {
						mt = element.RenderTransform as MatrixTransform;
						if(mt != null && !mt.IsSealed)
							mt.Matrix = new Matrix(mt.Matrix.M11, mt.Matrix.M12, mt.Matrix.M21, mt.Matrix.M22, mt.Matrix.OffsetX + pt.X, mt.Matrix.OffsetY + pt.Y);
					}
				}
			}
			prevOffset = pt;
		}
#endif
	}
}
