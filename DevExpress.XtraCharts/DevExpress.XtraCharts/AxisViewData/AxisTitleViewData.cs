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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class AxisTitleViewData : AxisElementViewDataBase {
		readonly RotatedTextPainterNearLine painter;
		readonly Rectangle boundsForCorrection;
		readonly int size;
		public Rectangle Bounds { get { return painter.RoundedBounds; } }
		public int Size { get { return size; } }
		public AxisTitleViewData(TextMeasurer textMeasurer, Axis2D axis, AxisMapping mapping, int axisOffset, int elementOffset) : base(axis, mapping, axisOffset, elementOffset, AxisElementLocation.Outside) {
			AxisTitle title = axis.Title;
			string text = title.Text;
			SizeF textSize = textMeasurer.MeasureString(text, title.Font);
			double locationOnAxis;
			int offsetOnAxis;
			switch (axis.Title.Alignment) {
				case StringAlignment.Near:
					locationOnAxis = Double.NegativeInfinity;
					offsetOnAxis = (int)Math.Floor(textSize.Width / 2.0);
					break;
				case StringAlignment.Far:
					locationOnAxis = Double.PositiveInfinity;
					offsetOnAxis = -(int)Math.Floor(textSize.Width / 2.0);
					break;
				default:
					IMinMaxValues visualRange = (IMinMaxValues)axis.VisualRangeData;
					locationOnAxis = (visualRange.Min + visualRange.Max) / 2;
					offsetOnAxis = 0;
					break;
			}
			painter = new RotatedTextPainterNearLine(GetScreenPoint(locationOnAxis, offsetOnAxis, 0),
				text, textSize, title, axis.GetNearTextPosition(), axis.GetAxisTitleActualAngle(), false, true, textMeasurer);
			boundsForCorrection = painter.RoundedBounds;
			size = axis.GetTextSize(boundsForCorrection);
			if (axis.IsVertical)
				boundsForCorrection.Height = 0;
			else
				boundsForCorrection.Width = 0;
		}
		public override void CalculateDiagramBoundsCorrection(RectangleCorrection correction) {
			correction.Update(boundsForCorrection);
		}
		public override void Render(IRenderer renderer) {
			painter.Render(renderer, HitTestController, Axis.Title);
		}
	}
}
