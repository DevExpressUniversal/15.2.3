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
using System.Collections.Generic;
namespace DevExpress.XtraCharts.Native {
	public abstract class DiagramViewData {
		readonly Diagram diagram;
		readonly Rectangle bounds;
		public Diagram Diagram { get { return diagram; } }
		public Rectangle Bounds { get { return bounds; } }
		public virtual Rectangle LegendMappingBounds { get { return bounds; } }
		public abstract IList<SeriesLabelLayoutList> LabelLayoutLists { get; }
		public abstract List<AnnotationLayout> AnnotationsAnchorPointsLayout { get;}
		public virtual List<AnnotationLayout> AnnotationsShapesLayout { get { return null; } }
		public virtual bool IsEmpty { get { return false; } }
		public DiagramViewData(Diagram diagram, Rectangle bounds) {
			this.diagram = diagram;
			this.bounds = bounds;
		}
		public abstract void CalculateSeriesAndLabelLayout(TextMeasurer textMeasurer, ChartDrawingHelper drawingHelper);
		public abstract GraphicsCommand CreateGraphicsCommand();
		public abstract void Render(IRenderer renderer);
		public abstract void RenderMiddle(IRenderer renderer);
		public abstract void RenderAbove(IRenderer renderer);
	}
}
