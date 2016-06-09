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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class XYDiagramResolveOverlappingHelper {
		static void FillNonOverlappingSeriesLabels(List<IXYDiagramLabelLayout> labels, SeriesLabelLayoutList labelLayoutList) {
			SeriesLabelBase label = labelLayoutList.Label;
			int index = labelLayoutList.Count / 2;
			if (label.ResolveOverlappingMode != ResolveOverlappingMode.None && label.ResolveOverlappingMode == ResolveOverlappingMode.HideOverlapped)
				for (int i = 0, k = labelLayoutList.Count - 1; i <= k; i++, k--) {
					labels.Add((IXYDiagramLabelLayout)labelLayoutList[i]);
					if (k != i)
						labels.Add((IXYDiagramLabelLayout)labelLayoutList[k]);
				}
			else for (int i = index, k = index - 1; i < labelLayoutList.Count || k >= 0; i++, k--) {
					if (i < labelLayoutList.Count)
						labels.Add((IXYDiagramLabelLayout)labelLayoutList[i]);
					if (k >= 0)
						labels.Add((IXYDiagramLabelLayout)labelLayoutList[k]);
				}
		}
		public static void Process(IList<SeriesLabelLayoutList> labelLayoutLists, ZPlaneRectangle bounds, int resolveOverlappingMinIndent) {
			List<IXYDiagramLabelLayout> labels = new List<IXYDiagramLabelLayout>();
			int index = labelLayoutLists.Count / 2;
			for(int i = index, k = index - 1; i < labelLayoutLists.Count || k >= 0; i++, k--) {
				if(i < labelLayoutLists.Count)
					FillNonOverlappingSeriesLabels(labels, labelLayoutLists[i]);
				if (k >= 0)
					FillNonOverlappingSeriesLabels(labels, labelLayoutLists[k]);
			}
			XYDiagramResolveOverlappingAlgorithm.Process(labels, bounds == null ? GRect2D.Empty : (GRect2D)bounds, resolveOverlappingMinIndent);
		}
	}
}
