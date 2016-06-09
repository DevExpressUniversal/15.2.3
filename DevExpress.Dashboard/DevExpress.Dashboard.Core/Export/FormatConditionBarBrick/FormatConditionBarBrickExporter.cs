#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using System.Text;
using DevExpress.DashboardCommon.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.DashboardExport {
	public class FormatConditionBarBrickExporter : TextBrickExporter {
		Color axisColor = Color.FromArgb(64, 0, 0, 0);
		FormatConditionBarBrick BarBrick { get { return Brick as FormatConditionBarBrick; } }
		protected override void DrawBackground(IGraphics gr, RectangleF rect) {
			gr.FillRectangle(new SolidBrush(Style.BackColor), rect);
			if(BorderSide.None != Style.Sides) {
				DrawBorders(gr, rect);
			}
		}
		protected override void DrawClientContent(IGraphics gr, RectangleF clientRect) {
			Rectangle cellBounds = Rectangle.Round(clientRect);
			if(!BarBrick.Color.IsEmpty) {
				Rectangle barBounds = BarBoundsCalculator.CalculateBounds(BarBrick, cellBounds, BarBrick.BarValue);
				gr.FillRectangle(new SolidBrush(BarBrick.Color), barBounds);
			}
			if(BarBrick.DrawAxis) {
				Point topPoint;
				Point bottomPoint;
				BarBoundsCalculator.CalculateAxisPoints(BarBrick, cellBounds, out topPoint, out bottomPoint);
				gr.DrawLine(new Pen(new SolidBrush(axisColor)), topPoint, bottomPoint);
			}
			if(!BarBrick.ShowBarOnly)
				gr.DrawString(BarBrick.Text, BarBrick.Style.Font, new SolidBrush(BarBrick.Style.ForeColor), clientRect, BarBrick.Style.StringFormat.Value);
		}
	}
}
