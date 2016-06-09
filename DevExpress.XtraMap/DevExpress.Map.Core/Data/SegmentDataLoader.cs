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

using DevExpress.Data;
using DevExpress.Utils;
namespace DevExpress.Map.Native {
	public interface IPieSegmentDataLoader {
		void LoadItem(IMapDataItem mapItem, object sum, int[] rowIndices);
	}
	public interface IMapDataController {
		int ListSourceRowCount { get; }
		int GroupRowCount { get; }
		SummaryItemCollection GroupSummary { get; }
		int GroupedColumnCount { get; }
		GroupRowInfoCollection GroupInfo { get; }
		void SetMapItemValueSummary(IMapDataItem child, object summary);
		void SetItemVisibleRowIndex(IMapDataItem child, int rowIndex, int[] listSourceIndices);
		void LoadItemAttributeArray(IMapDataItem child, int[] rowIndices);
		void LoadItemProperties(IMapDataItem child, int rowIndex);
		int GetListSourceRowIndex(int controllerRow);
		object GetRow(int sourceRowIndex);
	}
	public interface IMapDataAggregator {
		string SummaryColumn { get; set; }
		GroupInfoCollection AggregationGroups { get; }
		void Aggregate(DataController controller);
	}
	public abstract class PieSegmentDataLoaderBase : IPieSegmentDataLoader {
		readonly IMapDataController dataController;
		protected IMapDataController DataController { get { return dataController; } }
		protected PieSegmentDataLoaderBase(IMapDataController dataController) {
			Guard.ArgumentNotNull(dataController, "dataController");
			this.dataController = dataController;
		}
		public abstract void LoadItem(IMapDataItem mapItem, object summary, int[] rowIndexes);
	}
	public class MapChartDataItemSegmentDataLoader : PieSegmentDataLoaderBase {
		PieSegmentDataEnumerator en;
		public MapChartDataItemSegmentDataLoader(IMapDataController dataController)
			: base(dataController) {
		}
		public override void LoadItem(IMapDataItem mapItem, object summary, int[] rowIndices) {
			IMapContainerDataItem container = mapItem as IMapContainerDataItem;
			if(container == null)
				return;
			IMapChartDataItem child = container.CreateSegment();
			if(rowIndices != null && rowIndices.Length > 0) {
				DataController.LoadItemProperties(child, rowIndices[0]);
				DataController.LoadItemAttributeArray(child, rowIndices);
				if(en != null)
					DataController.SetItemVisibleRowIndex(child, rowIndices[0], en.GetListSourceRowIndices());
			}
			DataController.SetMapItemValueSummary(child, summary);
			container.AddSegment(child);
		}
		internal void SetEnumerator(PieSegmentDataEnumerator en) {
			this.en = en;
		}
	} 
}
