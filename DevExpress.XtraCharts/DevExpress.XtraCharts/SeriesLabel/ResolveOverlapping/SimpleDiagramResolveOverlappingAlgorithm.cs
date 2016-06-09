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
using System.Collections.Generic;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public static class ResolveOverlappingHelper {
		public static void ProcessForSimpleDiagram(IList<SeriesLabelLayoutList> labelLayoutLists, int resolveOverlappingMinIndent) {
			List<ILabelLayout> container = new List<ILabelLayout>();
			foreach(SimpleDiagramSeriesLabelLayoutList labelLayoutList in labelLayoutLists) {
				PieSeriesViewBase view = labelLayoutList.View as PieSeriesViewBase;
				if (view == null)
					return;
				if(labelLayoutList.Domain == null)
					continue;
				switch(((PieSeriesLabel)view.Label).Position) {
					case PieSeriesLabelPosition.TwoColumns:
						List<IPieLabelLayout> labelsLayout = new List<IPieLabelLayout>();
						foreach (ILabelLayout layout in labelLayoutList)
							labelsLayout.Add(layout as IPieLabelLayout);
						GRealRect2D labelsBounds = new GRealRect2D(labelLayoutList.Domain.LabelsBounds.Left, labelLayoutList.Domain.LabelsBounds.Top, labelLayoutList.Domain.LabelsBounds.Width, labelLayoutList.Domain.LabelsBounds.Height);
						SimpleDiagrammResiolveOverlapping.ArrangeByColumn(labelsLayout, labelsBounds, resolveOverlappingMinIndent);
						break;
					case PieSeriesLabelPosition.Outside:						
						ZPlaneRectangle rect = (ZPlaneRectangle)labelLayoutList.Domain.ElementBounds;
						int lineLength = ((PieSeriesLabel)view.Label).LineLength;
						PointsSweepDirection direction = (PointsSweepDirection)view.SweepDirection;
						rect.Inflate(lineLength, lineLength);
						labelsLayout = new List<IPieLabelLayout>();
						foreach (ILabelLayout layout in labelLayoutList)
							labelsLayout.Add(layout as IPieLabelLayout);
						labelsBounds = new GRealRect2D(labelLayoutList.Domain.LabelsBounds.Left, labelLayoutList.Domain.LabelsBounds.Top, labelLayoutList.Domain.LabelsBounds.Width, labelLayoutList.Domain.LabelsBounds.Height);
						SimpleDiagrammResiolveOverlapping.ArrangeByEllipse(labelsLayout, new GRealEllipse(new GRealPoint2D(rect.Center.X, rect.Center.Y), rect.Width / 2.0, rect.Height / 2.0), resolveOverlappingMinIndent, direction, labelsBounds);						
						break;
					default:
						break;
				}
			}
		}
	}
}
