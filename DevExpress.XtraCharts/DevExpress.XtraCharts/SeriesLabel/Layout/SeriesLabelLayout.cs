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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class SeriesLabelLayout : IBoundsProvider {
		readonly RefinedPointData pointData;
		readonly Color color;
		readonly TextPainterBase textPainter;
		readonly ConnectorPainterBase connectorPainter;
		readonly ResolveOverlappingMode resolveOverlappingMode;
		bool visible = true;
		bool ShouldApplyCorrection {
			get { return resolveOverlappingMode == ResolveOverlappingMode.None || resolveOverlappingMode == ResolveOverlappingMode.HideOverlapped; }
		}
		protected Series Series { get { return pointData.Series; } }
		protected DiagramPoint? ToolTipRelativePosition { get { return pointData.ToolTipRelativePosition; } }
		protected ResolveOverlappingMode OverlappingMode { get { return resolveOverlappingMode; } }
		public ISeriesPoint Point { get { return pointData.SeriesPoint; } }
		public TextPainterBase Painter { get { return textPainter; } }
		public ConnectorPainterBase ConnectorPainter { get { return connectorPainter; } }
		public RectangleF Bounds { get { return textPainter.Bounds; } }
		public bool Visible { get { return visible; } set { visible = value; } }
		public GRect2D LabelBounds {
			get {
				TextPainter painter = textPainter as TextPainter;
				return GraphicUtils.ConvertRect(GraphicUtils.RoundRectangle(painter == null ? textPainter.Bounds : painter.BoundsWithBorder));
			}
			set { Offset(value.Left - LabelBounds.Left, value.Top - LabelBounds.Top); }
		}
		public SeriesLabelLayout(RefinedPointData pointData, Color color, TextPainterBase textPainter, ConnectorPainterBase connectorPainter, ResolveOverlappingMode resolveOverlappingMode) {
			this.pointData = pointData;
			this.color = color;
			this.textPainter = textPainter;
			this.connectorPainter = connectorPainter;
			this.resolveOverlappingMode = resolveOverlappingMode;
		}
		void Offset(double dx, double dy) {
			if (dx != 0 || dy != 0) {
				textPainter.Offset(dx, dy);
				if (connectorPainter != null)
					connectorPainter.Offset(dx, dy);
			}
		}		
		public void CalculateDiagramBoundsCorrection(RectangleCorrection correction) {
			if (ShouldApplyCorrection)
				correction.Update(textPainter.RoundedBounds);
		}
		public void Render(IRenderer renderer, HitTestController hitTestController) {
			if (!visible)
				return;
			textPainter.Render(renderer, hitTestController, pointData.RefinedPoint, color);
			if (hitTestController.PointToolTipEnabled(Series))
				renderer.ProcessHitTestRegion(hitTestController, textPainter.PropertiesProvider, new ChartFocusedArea(Point, ToolTipRelativePosition), textPainter.GetHitRegion(), true);
		}
		public void Render(IRenderer renderer, HitTestController hitTestController, Rectangle clipBounds) {
			if (!visible)
				return;
			textPainter.Render(renderer, hitTestController, pointData.RefinedPoint, color, clipBounds);
			if (hitTestController.PointToolTipEnabled(Series))
				renderer.ProcessHitTestRegion(hitTestController, textPainter.PropertiesProvider, new ChartFocusedArea(Point, ToolTipRelativePosition), textPainter.GetHitRegion(), true);
		}
		public void RenderConnector(IRenderer renderer, SeriesLabelBase label) {
			if (visible && connectorPainter != null)
				connectorPainter.Render(renderer, label, color);
		}
		public void FillPrimitives(SeriesLabelBase label, PrimitivesContainer container) {
			if (visible && connectorPainter != null)
				connectorPainter.FillPrimitives(label, color, container);
		}
		#region IBoundsProvider
		bool IBoundsProvider.Enable {
			get { return ShouldApplyCorrection; }
		}
		GRealRect2D IBoundsProvider.GetBounds() {
			RectangleF bounds = textPainter.Bounds;
			return new GRealRect2D(bounds.Left, bounds.Top, bounds.Width, bounds.Height);
		}
		#endregion
	}
}
