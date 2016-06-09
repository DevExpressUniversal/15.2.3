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
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Xpf.Gauges.Native {
	public class ArcScaleLayoutCalculator {
		readonly ArcScale scale;
		readonly Rect initialBounds;
		public ArcScaleLayoutCalculator(ArcScale scale, Rect initialBounds) {
			this.scale = scale;
			this.initialBounds = initialBounds;
		}
		void CalculateLayoutParams(Rect bounds, out Point ellipseCenter, out double ellipseWidth, out double ellipseHeight) {
			double centerX = 0.5 * (bounds.Left + bounds.Right);
			double centerY = 0.5 * (bounds.Top + bounds.Bottom);
			ellipseCenter = new Point(centerX, centerY);
			ellipseWidth = bounds.Width;
			ellipseHeight = bounds.Height;
		}
		void CalculateCircleLayoutParams(out Rect bounds, out Point ellipseCenter, out double ellipseWidth, out double ellipseHeight) {
			Size size = new Size(Math.Min(initialBounds.Height, initialBounds.Width), Math.Min(initialBounds.Height, initialBounds.Width));
			bounds = new Rect(0, 0, size.Width, size.Height);
			CalculateLayoutParams(bounds, out ellipseCenter, out ellipseWidth, out ellipseHeight);
		}
		void CalculateEllipseLayoutParams(out Rect bounds, out Point ellipseCenter, out double ellipseWidth, out double ellipseHeight) {
			bounds = initialBounds;
			CalculateLayoutParams(bounds, out ellipseCenter, out ellipseWidth, out ellipseHeight);
		}
		void CalculateHalfTopLayoutParams(out Rect bounds, out Point ellipseCenter, out double ellipseWidth, out double ellipseHeight) {
			bounds = initialBounds;
			Thickness layoutMargin = GetLayoutMargin();
			if (2 * (initialBounds.Height - layoutMargin.Top - layoutMargin.Bottom) + layoutMargin.Left + layoutMargin.Right <= initialBounds.Width) {
				ellipseWidth = Math.Max(2 * (initialBounds.Height - layoutMargin.Top - layoutMargin.Bottom), 0.0);
				bounds.Width = ellipseWidth + layoutMargin.Left + layoutMargin.Right;
			}
			else {
				ellipseWidth = Math.Max(initialBounds.Width - layoutMargin.Left - layoutMargin.Right, 0.0);
				bounds.Height = ellipseWidth / 2 + layoutMargin.Top + layoutMargin.Bottom;
			}
			ellipseHeight = ellipseWidth;
			double centerX = 0.5 * ellipseWidth + layoutMargin.Left;
			double centerY = 0.5 * ellipseHeight + layoutMargin.Top;
			ellipseCenter = new Point(centerX, centerY);
		}
		void CalculateQuarterLayoutParams(out Rect bounds, out Point ellipseCenter, out double ellipseWidth, out double ellipseHeight, ArcScaleLayoutMode layoutMode) {
			Thickness layoutMargin = GetLayoutMargin();
			bounds = initialBounds;
			if (initialBounds.Height - layoutMargin.Top - layoutMargin.Bottom + layoutMargin.Left + layoutMargin.Right <= initialBounds.Width) {
				ellipseWidth = Math.Max(2 * (initialBounds.Height - layoutMargin.Top - layoutMargin.Bottom), 0.0);
				bounds.Width = 0.5 * ellipseWidth + layoutMargin.Left + layoutMargin.Right;
			}
			else {
				ellipseWidth = Math.Max(2 * (initialBounds.Width - layoutMargin.Left - layoutMargin.Right), 0.0);
				bounds.Height = 0.5 * ellipseWidth + layoutMargin.Top + layoutMargin.Bottom;
			}
			ellipseHeight = ellipseWidth;
			double centerX = 0.0;
			switch (layoutMode) {
				case ArcScaleLayoutMode.QuarterTopLeft:
					centerX = 0.5 * ellipseWidth + layoutMargin.Left;
					break;
				case ArcScaleLayoutMode.QuarterTopRight:
					centerX = layoutMargin.Left;
					break;
			}
			double centerY = 0.5 * ellipseHeight + layoutMargin.Top;
			ellipseCenter = new Point(centerX, centerY);
		}
		Thickness GetLayoutMargin() {
			if (scale.Gauge.ActualModel.GetScaleLayerModel(scale.ActualLayoutMode, 0) != null)
				return scale.Gauge.ActualModel.GetScaleModel(scale.ActualLayoutMode, 0).LayoutMargin;
			return new Thickness(0.0);
		}
		public ArcScaleLayout Calculate() {
			Rect bounds;
			Point ellipseCenter;
			double ellipseWidth;
			double ellipseHeight;
			switch (scale.ActualLayoutMode) {
				case ArcScaleLayoutMode.Ellipse:
					CalculateEllipseLayoutParams(out bounds, out ellipseCenter, out ellipseWidth, out ellipseHeight);
					break;
				case ArcScaleLayoutMode.HalfTop:
					CalculateHalfTopLayoutParams(out bounds, out ellipseCenter, out ellipseWidth, out ellipseHeight);
					break;
				case ArcScaleLayoutMode.QuarterTopLeft:
				case ArcScaleLayoutMode.QuarterTopRight:
					CalculateQuarterLayoutParams(out bounds, out ellipseCenter, out ellipseWidth, out ellipseHeight, scale.ActualLayoutMode);
					break;
				default:
					CalculateCircleLayoutParams(out bounds, out ellipseCenter, out ellipseWidth, out ellipseHeight);
					break;
			}
			return new ArcScaleLayout(bounds, ellipseCenter, ellipseWidth, ellipseHeight);
		}
	}
}
