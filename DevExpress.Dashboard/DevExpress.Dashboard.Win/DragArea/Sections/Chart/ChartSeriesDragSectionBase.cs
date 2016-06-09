#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.XtraEditors;
namespace DevExpress.DashboardWin.Native {
	public abstract class ChartSeriesDragSectionBase<TSeries> : HolderCollectionDragSection<TSeries>, IChartSeriesSection where TSeries : ChartSeries {
		static string GetSeriesImageName(TSeries series) {
			return String.Format("Groups.{0}", series.SeriesImageName);
		}
		readonly ChartSeriesDescription<TSeries> seriesDescription;
		public IEnumerable<SeriesViewGroup> SeriesViewGroups { get { return seriesDescription.SeriesViewGroups; } }
		public ChartSeries DefaultSeries { get { return NewGroup.Holder; } }
		protected ChartSeriesDragSectionBase(DragArea area, string caption, string itemName, ChartSeriesDescription<TSeries> seriesDescription)
			: this(area, caption, itemName, null, seriesDescription) {
		}
		protected ChartSeriesDragSectionBase(DragArea area, string caption, string itemName, string itemNamePlural, ChartSeriesDescription<TSeries> seriesDescription)
			: base(area, caption, itemName, itemNamePlural, seriesDescription.Series) {
			this.seriesDescription = seriesDescription;
		}
		public void SelectGroup(int groupIndex) {
			if(groupIndex < 0)
				NewGroup.Select();
			else
				Groups[groupIndex].Select();
		}
		protected abstract XtraForm CreateOptionsForm(ChartSelectorContext context, IEnumerable<SeriesViewGroup> seriesViewGroups);
		protected override HolderCollectionDragGroup<TSeries> CreateGroupInternal(TSeries series) {
			TSeries actualSeries = series ?? (TSeries)seriesDescription.ChartSeriesConverter.CreateSeries();
			return new HolderCollectionDragGroup<TSeries>(GetSeriesImageName(actualSeries), actualSeries);
		}
		public override XtraForm CreateOptionsForm(DashboardDesigner designer, DragGroup group) {
			HolderCollectionDragGroup<TSeries> seriesGroup = group as HolderCollectionDragGroup<TSeries>;
			if(seriesGroup != null) {
				TSeries series;
				int groupIndex;
				if(seriesGroup.IsNewGroup) {
					groupIndex = -1;
					series = NewGroup.Holder;
				}
				else {
					groupIndex = seriesGroup.GroupIndex;
					series = Groups[groupIndex].Holder;
				}
				ChartSelectorContext selectorContext = new ChartSelectorContext(designer, this, series, groupIndex, DashboardWinStringId.HistoryItemChartSeriesType);
				return CreateOptionsForm(selectorContext, SeriesViewGroups);
			}
			return null;
		}
	}
}
