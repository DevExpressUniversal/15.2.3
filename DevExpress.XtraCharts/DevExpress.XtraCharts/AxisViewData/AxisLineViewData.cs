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
namespace DevExpress.XtraCharts.Native {
	public class AxisLineViewData : AxisElementViewDataBase {
		readonly Rectangle bounds;
		public Rectangle Bounds { get { return bounds; } }
		public AxisLineViewData(Axis2D axis, AxisIntervalMapping mapping, int axisOffset, int elementOffset) : base(axis, mapping, axisOffset, elementOffset, AxisElementLocation.Outside) {
			Point p1 = GetScreenPoint(Double.NegativeInfinity, 0, 0);
			Point p2 = GetScreenPoint(Double.PositiveInfinity, 0, HitState.Normal ? Axis.Thickness - 1 : Axis.Thickness);
			bounds = GraphicUtils.MakeRectangle(p1, p2);
		}
		public override void CalculateDiagramBoundsCorrection(RectangleCorrection correction) {
			correction.Update(bounds);
		}
		public override void Render(IRenderer renderer) {
			if (!bounds.AreWidthAndHeightPositive())
				return;
			Axis2D axis = Axis;
			renderer.FillRectangle(bounds, axis.ActualColor);
			renderer.ProcessHitTestRegion(HitTestController, axis, null, new HitRegion(bounds));
		}
	}
}
