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

using System.Collections.Generic;
using System.Drawing;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Native;
namespace DevExpress.Charts.Native {
	public interface ISimpleDiagramPanelInternal : ISimpleDiagramPanel {
		IList<Rectangle> Arrange(IEnumerable<IRefinedSeries> seriesList, Rectangle diagramBounds);
	}
	public class SimpleDiagramPanel : ISimpleDiagramPanelInternal {
		ISimpleDiagram simpleDiagram;
		public const int IndentFromDomainBounds = 3;
		public SimpleDiagramPanel(ISimpleDiagram simpleDiagram) {
			this.simpleDiagram = simpleDiagram;
		}
		#region ISimpleDiagramPanel implementation
		public IList<Rectangle> Arrange(IList<Series> seriesList, Rectangle diagramBounds) {
			return new List<Rectangle>();
		}
		#endregion
		#region ISimpleDiagramPanelInternal implementation
		public IList<Rectangle> Arrange(IEnumerable<IRefinedSeries> seriesList, Rectangle diagramBounds) {
			List<IRefinedSeries> actualSeriesList = new List<IRefinedSeries>();
			foreach (IRefinedSeries series in seriesList)
				if (series.Points.Count > 0)
					actualSeriesList.Add(series);
			int simpleDiagramDomainCount = actualSeriesList.Count == 0 ? 1 : actualSeriesList.Count;
			List<GRect2D> gRectList = SimpleDiagramLayout.Calculate(simpleDiagram, GraphicUtils.ConvertRect(diagramBounds), simpleDiagramDomainCount);
			List<Rectangle> result = new List<Rectangle>(gRectList.Count);
			foreach (GRect2D gRect in gRectList) {
				Rectangle rectDerivativeFromGRect = new Rectangle(gRect.Left, gRect.Top, gRect.Width, gRect.Height);
				rectDerivativeFromGRect = GraphicUtils.InflateRect(rectDerivativeFromGRect, -IndentFromDomainBounds, -IndentFromDomainBounds);
				result.Add(rectDerivativeFromGRect);
			}
			return result;
		}
		#endregion
	}
}
