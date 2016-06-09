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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class XYDiagramMappingList {
		readonly List<XYDiagramMappingContainer> mappingContainers = new List<XYDiagramMappingContainer>();
		readonly XYDiagram2D diagram;
		readonly AxisIntervalsLayoutRepository intervalsLayoutRepository;
		readonly Rectangle maximumMappingBounds;
		public IEnumerable<XYDiagramMappingContainer> MappingContainers { get { return mappingContainers; } }
		public Rectangle MappingBounds { get { return intervalsLayoutRepository.MappingBounds; } }
		public Rectangle MaximumMappingBounds { get { return maximumMappingBounds; } }
		public XYDiagramMappingList(XYDiagram2D diagram, AxisIntervalsLayoutRepository intervalsLayoutRepository, Rectangle maximumMappingBounds) {
			this.diagram = diagram;
			this.intervalsLayoutRepository = intervalsLayoutRepository;
			this.maximumMappingBounds = maximumMappingBounds;
		}
		public XYDiagramMappingContainer FindMappingContainer(IAxisData axisX, IAxisData axisY) {
			foreach (XYDiagramMappingContainer mappingContainer in mappingContainers)
				if (mappingContainer.AxisX == axisX && mappingContainer.AxisY == axisY)
					return mappingContainer;
			return null;			
		}
		public XYDiagramMappingContainer GetMappingContainer(Axis2D axisX, Axis2D axisY) {
			XYDiagramMappingContainer mappingContainer = FindMappingContainer(axisX, axisY);
			if (mappingContainer == null) {
				mappingContainer = new XYDiagramMappingContainer(intervalsLayoutRepository, diagram, axisX, axisY);
				mappingContainers.Add(mappingContainer);
			}
			return mappingContainer;
		}
		public XYDiagramMappingContainer GetMappingContainer(RefinedSeriesData seriesData) {
			XYDiagram2DSeriesViewBase view = seriesData.Series.View as XYDiagram2DSeriesViewBase;
			if (view == null) {
				ChartDebug.Fail("The XYDiagram2DSeriesViewBase expected.");
				return null;
			}
			return GetMappingContainer(view.ActualAxisX, view.ActualAxisY);			
		}
	}
}
