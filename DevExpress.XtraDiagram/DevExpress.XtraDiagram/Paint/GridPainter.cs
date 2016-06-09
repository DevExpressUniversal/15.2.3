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
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.Diagram.Core;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraDiagram.ViewInfo;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Utils;
namespace DevExpress.XtraDiagram.Paint {
	#region PaintArgs
	public class DiagramGridObjectInfoArgs : ObjectInfoArgs {
		MeasureUnit measureUnit;
		SizeF? gridSize;
		double zoomFactor;
		Rectangle gridDrawArea;
		Point gridContentLocation;
		Color majorAxisLineColor;
		Color minorAxisLineColor;
		public DiagramGridObjectInfoArgs() {
		}
		public virtual void Initialize(DiagramControlViewInfo viewInfo) {
			this.gridDrawArea = viewInfo.GridDrawArea;
			this.gridContentLocation = viewInfo.GridContentLocation;
			this.measureUnit = viewInfo.MeasureUnit;
			this.gridSize = viewInfo.GridSize;
			this.zoomFactor = viewInfo.ZoomFactor;
			this.majorAxisLineColor = viewInfo.DefaultAppearances.GetMajorGridAxisLineColor();
			this.minorAxisLineColor = viewInfo.DefaultAppearances.GetMinorGridAxisLineColor();
		}
		public virtual void Clear() {
		}
		public Color MajorAxisLineColor { get { return majorAxisLineColor; } }
		public Color MinorAxisLineColor { get { return minorAxisLineColor; } }
		public Rectangle GridDrawArea { get { return gridDrawArea; } }
		public Point GridContentLocation { get { return gridContentLocation; } }
		public MeasureUnit MeasureUnit { get { return measureUnit; } }
		public SizeF? GridSize { get { return gridSize; } }
		public double ZoomFactor { get { return zoomFactor; } }
	}
	#endregion
	#region Painters
	public class DiagramGridPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			DiagramGridObjectInfoArgs args = (DiagramGridObjectInfoArgs)e;
			base.DrawObject(e);
			DrawGrid(args);
		}
		protected virtual void DrawGrid(DiagramGridObjectInfoArgs args) {
			DrawMinorGrid(args);
			DrawMajorGrid(args);
		}
		protected virtual void DrawMajorGrid(DiagramGridObjectInfoArgs args) {
			RulerRenderHelper.DrawGrid(
				args.MeasureUnit,
				args.GridSize.ToPlatformNullableSize(),
				args.GridDrawArea.ToPlatformRect(),
				args.ZoomFactor,
				true,
				(axisLine) => DrawAxisLine(args, axisLine, true)
			);
		}
		protected virtual void DrawMinorGrid(DiagramGridObjectInfoArgs args) {
			RulerRenderHelper.DrawGrid(
				args.MeasureUnit,
				args.GridSize.ToPlatformNullableSize(),
				args.GridDrawArea.ToPlatformRect(),
				args.ZoomFactor,
				false,
				(axisLine) => DrawAxisLine(args, axisLine, false)
			);
		}
		protected virtual void DrawAxisLine(DiagramGridObjectInfoArgs args, AxisLine axisLine, bool major) {
			args.Graphics.DrawLine(GetAxisPen(args, major), GetStartPoint(args, axisLine), GetEndPoint(args, axisLine));
		}
		protected Pen GetAxisPen(DiagramGridObjectInfoArgs args, bool major) {
			Color color = major ? args.MajorAxisLineColor : args.MinorAxisLineColor;
			return args.Cache.GetPen(color);
		}
		protected Point GetStartPoint(DiagramGridObjectInfoArgs args, AxisLine axisLine) {
			Point start = axisLine.From.ToWinPoint();
			return PointUtils.ApplyOffset(start, args.GridContentLocation);
		}
		protected Point GetEndPoint(DiagramGridObjectInfoArgs args, AxisLine axisLine) {
			Point end = axisLine.To.ToWinPoint();
			return PointUtils.ApplyOffset(end, args.GridContentLocation);
		}
	}
	#endregion
}
