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
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Office;
namespace DevExpress.Spreadsheet.Charts {
	public interface ChartViewCollection : ISimpleCollection<ChartView> {
		bool Contains(ChartView view);
		int IndexOf(ChartView view);
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Charts;
	using DevExpress.Office.API.Internal;
	using DevExpress.Utils;
	partial class NativeChartViewCollection : NativeChartCollectionBase<ChartView, NativeChartView, Model.IChartView>, ChartViewCollection {
		readonly NativeChart nativeChart;
		public NativeChartViewCollection(Model.ChartViewCollection modelCollection, NativeChart nativeChart)
			: base(modelCollection) {
			this.nativeChart = nativeChart;
			CreateViewSeries();
		}
		protected override NativeChartView CreateNativeObject(Model.IChartView modelItem) {
			NativeChartView result = new NativeChartView(modelItem, nativeChart);
			Guard.ArgumentNotNull(result.Series, "viewSeries");
			return result;
		}
		void CreateViewSeries() {
			InnerList.ForEach(CreateViewSeriesCore);
		}
		void CreateViewSeriesCore(NativeChartView nativeView) {
			Guard.ArgumentNotNull(nativeView.Series, "viewSeries");
		}
		#region ChartViewCollection Members
		public bool Contains(ChartView view) {
			CheckValid();
			return IndexOf(view) != -1;
		}
		public int IndexOf(ChartView view) {
			CheckValid();
			NativeChartView nativeView = view as NativeChartView;
			if (nativeView != null)
				return InnerList.IndexOf(nativeView);
			return -1;
		}
		#endregion
	}
}
