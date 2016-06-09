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
	public class SimpleDiagram3DViewData : SimpleDiagramViewData {
		public SimpleDiagram3DViewData(Diagram diagram, Rectangle bounds, List<SeriesLayout> seriesLayoutList, List<SeriesLabelLayoutList> labelLayoutLists, List<AnnotationLayout> annotationsAnchorPointsLayout)
			: base(diagram, bounds, seriesLayoutList, labelLayoutLists, annotationsAnchorPointsLayout) { 
		}
		public override void CalculateSeriesAndLabelLayout(TextMeasurer textMeasurer, ChartDrawingHelper drawingHelper) {
			ResolveOverlappingHelper.ProcessForSimpleDiagram(GetLabelsForResolveOverlapping(), Diagram.LabelsResolveOverlappingMinIndent);
		}
		public override GraphicsCommand CreateGraphicsCommand() {
			GraphicsCommand command = new ContainerGraphicsCommand();
			foreach (SimpleDiagramSeriesLayoutBase seriesLayout in SeriesLayoutList) {
				SimpleDiagram3DSeriesLayout layout = seriesLayout as SimpleDiagram3DSeriesLayout;
				if (layout != null)
					command.AddChildCommand(layout.CreateGraphicsCommand());
			}
			return command;
		}
		public override void Render(IRenderer renderer) {			
		}
		public override void RenderAbove(IRenderer renderer) {
			foreach (SimpleDiagram3DSeriesLayout seriesLayout in SeriesLayoutList)
				seriesLayout.RenderTitles(renderer);
			foreach (SimpleDiagram3DSeriesLayout seriesLayout in SeriesLayoutList) {
				if (seriesLayout.Domain != null) {
					renderer.SetClipping(seriesLayout.Domain.Bounds);
					seriesLayout.RenderExclamation(renderer);
					seriesLayout.LabelLayoutList.RenderSeriesLabelConnectors(renderer);
					seriesLayout.LabelLayoutList.RenderSeriesLabels(renderer);
					renderer.RestoreClipping();
					seriesLayout.RenderAnnotations(renderer);
				}
			}
		}
	}
}
