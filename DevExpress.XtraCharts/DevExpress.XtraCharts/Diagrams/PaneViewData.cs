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
	public abstract class PaneViewData : DiagramViewData {
		readonly IList<SeriesLayout> seriesLayoutList;
		readonly IList<SeriesLabelLayoutList> labelLayoutLists;
		readonly bool isLabelsResolveOverlapping;
		public IList<SeriesLayout> SeriesLayoutList { get { return seriesLayoutList; } }
		public override IList<SeriesLabelLayoutList> LabelLayoutLists { get { return labelLayoutLists; } }		
		protected bool IsLabelsResolveOverlapping { get { return isLabelsResolveOverlapping; } }
		public PaneViewData(Diagram diagram, Rectangle bounds, IList<SeriesLayout> seriesLayoutList, IList<SeriesLabelLayoutList> labelLayoutLists) : base(diagram, bounds) {
			this.seriesLayoutList = seriesLayoutList;
			this.labelLayoutLists = labelLayoutLists;
			isLabelsResolveOverlapping = false;
			foreach (SeriesLabelLayoutList labelLayoutList in labelLayoutLists) {
				SeriesLabelBase label = labelLayoutList.Label;
				if (label != null && label.SeriesBase.ActualLabelsVisibility && label.ResolveOverlappingMode != ResolveOverlappingMode.None){
					isLabelsResolveOverlapping = true;
					break;
				}
			}
		}		
		protected IList<SeriesLabelLayoutList> GetLabelsForResolveOverlapping() {
			List<SeriesLabelLayoutList> result = new List<SeriesLabelLayoutList>();
			foreach (SeriesLabelLayoutList labelLayoutList in labelLayoutLists) {
				SeriesLabelBase label = labelLayoutList.Label;
				if (label != null && label.SeriesBase.ActualLabelsVisibility)
					result.Add(labelLayoutList);
			}
			return result;
		}
		protected void RenderSeriesLabels(IRenderer renderer) {
			if (labelLayoutLists.Count == 0)
				return;
			foreach (SeriesLabelLayoutList labelLayoutList in labelLayoutLists)
				labelLayoutList.RenderSeriesLabelConnectors(renderer);
			foreach (SeriesLabelLayoutList labelLayoutList in labelLayoutLists)
				labelLayoutList.RenderSeriesLabels(renderer);
		}
	}
}
