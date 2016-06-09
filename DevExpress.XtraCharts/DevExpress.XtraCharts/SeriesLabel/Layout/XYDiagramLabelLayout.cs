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
	public class XYDiagramSeriesLabelLayout : SeriesLabelLayout, IXYDiagramLabelLayout {
		public static XYDiagramSeriesLabelLayout Create(RefinedPointData pointData, Color color, TextPainterBase textPainter, ConnectorPainterBase connectorPainter, ResolveOverlappingMode resolveOverlappingMode, DiagramPoint anchorPoint) {
			return new XYDiagramSeriesLabelLayout(pointData, color, textPainter, connectorPainter, anchorPoint, resolveOverlappingMode, RectangleF.Empty, RectangleF.Empty);
		}
		public static XYDiagramSeriesLabelLayout CreateWithValidRectangle(RefinedPointData pointData, Color color, TextPainterBase textPainter, ConnectorPainterBase connectorPainter, ResolveOverlappingMode resolveOverlappingMode, DiagramPoint anchorPoint, RectangleF validRectangle) {
			return new XYDiagramSeriesLabelLayout(pointData, color, textPainter, connectorPainter, anchorPoint, resolveOverlappingMode, validRectangle, RectangleF.Empty);
		}
		public static XYDiagramSeriesLabelLayout CreateWithExcludedRectangle(RefinedPointData pointData, Color color, TextPainterBase textPainter, ConnectorPainterBase connectorPainter, ResolveOverlappingMode resolveOverlappingMode, DiagramPoint anchorPoint, RectangleF excludedRectangle) {
			return new XYDiagramSeriesLabelLayout(pointData, color, textPainter, connectorPainter, anchorPoint, resolveOverlappingMode, RectangleF.Empty, excludedRectangle);
		}
		readonly GPoint2D anchorPoint;
		readonly GRect2D validRectangle;
		readonly GRect2D excludedRectangle;
		public GPoint2D AnchorPoint { get { return anchorPoint; } }
		public GRect2D ValidRectangle { get { return validRectangle; } }
		public ResolveOverlappingModeCore ResolveOverlappingMode { get { return (ResolveOverlappingModeCore)OverlappingMode; } }
		public GRect2D ExcludedRectangle { get { return excludedRectangle; } }
		XYDiagramSeriesLabelLayout(RefinedPointData pointData, Color color, TextPainterBase textPainter, ConnectorPainterBase connectorPainter, DiagramPoint anchorPoint, ResolveOverlappingMode resolveOverlappingMode, RectangleF validRectangle, RectangleF excludedRectangle) : base(pointData, color, textPainter, connectorPainter, resolveOverlappingMode) {
			this.anchorPoint = new GPoint2D(MathUtils.StrongRound(anchorPoint.X), MathUtils.StrongRound(anchorPoint.Y));
			if(validRectangle.IsEmpty || 
				double.IsNaN(validRectangle.X) || double.IsNaN(validRectangle.Y) ||
				double.IsNaN(validRectangle.Width) || double.IsNaN(validRectangle.Height))
				this.validRectangle = GRect2D.Empty;
			else
				this.validRectangle = MathUtils.MakeGRect2D(validRectangle);
			this.excludedRectangle = excludedRectangle.IsEmpty ? GRect2D.Empty : MathUtils.MakeGRect2D(excludedRectangle);
		}
	}
}
