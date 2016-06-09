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
using System.Linq;
using System.Text;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Designer.Native {
	public class AddSecondaryAxisXCommand : AddCommandBase<SecondaryAxisX> {
		readonly SecondaryAxisXCollection secondaryAxisXCollection;
		protected override ChartCollectionBase ChartCollection { get { return secondaryAxisXCollection; } }
		public AddSecondaryAxisXCommand(CommandManager commandManager, SecondaryAxisXCollection secondaryAxisXCollection)
			: base(commandManager) {
			this.secondaryAxisXCollection = secondaryAxisXCollection;
		}
		protected override SecondaryAxisX CreateChartElement(object parameter) {
			return new SecondaryAxisX();
		}
		protected override void AddToCollection(SecondaryAxisX chartElement) {
			secondaryAxisXCollection.Add(chartElement);
		}
	}
	public class DeleteSecondaryAxisXCommand : DeleteCommandBase<SecondaryAxisX> {
		readonly SecondaryAxisXCollection secondaryAxisXCollection;
		readonly Chart chart;
		protected override ChartCollectionBase ChartCollection { get { return secondaryAxisXCollection; } }
		public DeleteSecondaryAxisXCommand(CommandManager commandManager, SecondaryAxisXCollection secondaryAxisXCollection, Chart chart)
			: base(commandManager) {
			this.secondaryAxisXCollection = secondaryAxisXCollection;
			this.chart = chart;
		}
		protected override void InsertIntoCollection(int index, SecondaryAxisX chartElement) {
			secondaryAxisXCollection.Insert(index, chartElement);
		}
		protected override object CreateCollectionPropertiesCache(SecondaryAxisX chartElement) {
			return CommonUtils.FindViewsByAxisX(chartElement, chart);
		}
		protected override void RestoreCollectionProperties(SecondaryAxisX chartElement, object properties) {
			List<XYDiagram2DSeriesViewBase> xyViewList = (List<XYDiagram2DSeriesViewBase>)properties;
			foreach (XYDiagramSeriesViewBase view in xyViewList) {
				view.AxisX = chartElement;
			}
		}
	}
	public class AddSecondaryAxisYCommand : AddCommandBase<SecondaryAxisY> {
		readonly SecondaryAxisYCollection secondaryAxisYCollection;
		protected override ChartCollectionBase ChartCollection { get { return secondaryAxisYCollection; } }
		public AddSecondaryAxisYCommand(CommandManager commandManager, SecondaryAxisYCollection secondaryAxisYCollection)
			: base(commandManager) {
			this.secondaryAxisYCollection = secondaryAxisYCollection;
		}
		protected override SecondaryAxisY CreateChartElement(object parameter) {
			return new SecondaryAxisY();
		}
		protected override void AddToCollection(SecondaryAxisY chartElement) {
			secondaryAxisYCollection.Add(chartElement);
		}
	}
	public class DeleteSecondaryAxisYCommand : DeleteCommandBase<SecondaryAxisY> {
		readonly SecondaryAxisYCollection secondaryAxisYCollection;
		readonly Chart chart;
		protected override ChartCollectionBase ChartCollection { get { return secondaryAxisYCollection; } }
		public DeleteSecondaryAxisYCommand(CommandManager commandManager, SecondaryAxisYCollection secondaryAxisYCollection, Chart chart)
			: base(commandManager) {
			this.secondaryAxisYCollection = secondaryAxisYCollection;
			this.chart = chart;
		}
		protected override void InsertIntoCollection(int index, SecondaryAxisY chartElement) {
			secondaryAxisYCollection.Insert(index, chartElement);
		}
		protected override object CreateCollectionPropertiesCache(SecondaryAxisY chartElement) {
			return CommonUtils.FindViewsByAxisY(chartElement, chart);
		}
		protected override void RestoreCollectionProperties(SecondaryAxisY chartElement, object properties) {
			List<XYDiagram2DSeriesViewBase> xyViewList = (List<XYDiagram2DSeriesViewBase>)properties;
			foreach (XYDiagramSeriesViewBase view in xyViewList) {
				view.AxisY = chartElement;
			}
		}
	}
	public class AddSwiftPlotSecondaryAxisXCommand : AddCommandBase<SwiftPlotDiagramSecondaryAxisX> {
		readonly SwiftPlotDiagramSecondaryAxisXCollection secondaryAxisXCollection;
		protected override ChartCollectionBase ChartCollection { get { return secondaryAxisXCollection; } }
		public AddSwiftPlotSecondaryAxisXCommand(CommandManager commandManager, SwiftPlotDiagramSecondaryAxisXCollection secondaryAxisXCollection)
			: base(commandManager) {
			this.secondaryAxisXCollection = secondaryAxisXCollection;
		}
		protected override SwiftPlotDiagramSecondaryAxisX CreateChartElement(object parameter) {
			return new SwiftPlotDiagramSecondaryAxisX();
		}
		protected override void AddToCollection(SwiftPlotDiagramSecondaryAxisX chartElement) {
			secondaryAxisXCollection.Add(chartElement);
		}
	}
	public class DeleteSwiftPlotSecondaryAxisXCommand : DeleteCommandBase<SwiftPlotDiagramSecondaryAxisX> {
		readonly SwiftPlotDiagramSecondaryAxisXCollection secondaryAxisXCollection;
		readonly Chart chart;
		protected override ChartCollectionBase ChartCollection { get { return secondaryAxisXCollection; } }
		public DeleteSwiftPlotSecondaryAxisXCommand(CommandManager commandManager, SwiftPlotDiagramSecondaryAxisXCollection secondaryAxisXCollection, Chart chart)
			: base(commandManager) {
			this.secondaryAxisXCollection = secondaryAxisXCollection;
			this.chart = chart;
		}
		protected override void InsertIntoCollection(int index, SwiftPlotDiagramSecondaryAxisX chartElement) {
			secondaryAxisXCollection.Insert(index, chartElement);
		}
		protected override object CreateCollectionPropertiesCache(SwiftPlotDiagramSecondaryAxisX chartElement) {
			return CommonUtils.FindViewsByAxisX(chartElement, chart);
		}
		protected override void RestoreCollectionProperties(SwiftPlotDiagramSecondaryAxisX chartElement, object properties) {
			List<XYDiagram2DSeriesViewBase> xyViewList = (List<XYDiagram2DSeriesViewBase>)properties;
			foreach (SwiftPlotSeriesViewBase view in xyViewList) {
				view.AxisX = chartElement;
			}
		}
	}
	public class AddSwiftPlotSecondaryAxisYCommand : AddCommandBase<SwiftPlotDiagramSecondaryAxisY> {
		readonly SwiftPlotDiagramSecondaryAxisYCollection secondaryAxisYCollection;
		protected override ChartCollectionBase ChartCollection { get { return secondaryAxisYCollection; } }
		public AddSwiftPlotSecondaryAxisYCommand(CommandManager commandManager, SwiftPlotDiagramSecondaryAxisYCollection secondaryAxisYCollection)
			: base(commandManager) {
			this.secondaryAxisYCollection = secondaryAxisYCollection;
		}
		protected override SwiftPlotDiagramSecondaryAxisY CreateChartElement(object parameter) {
			return new SwiftPlotDiagramSecondaryAxisY();
		}
		protected override void AddToCollection(SwiftPlotDiagramSecondaryAxisY chartElement) {
			secondaryAxisYCollection.Add(chartElement);
		}
	}
	public class DeleteSwiftPlotSecondaryAxisYCommand : DeleteCommandBase<SwiftPlotDiagramSecondaryAxisY> {
		readonly SwiftPlotDiagramSecondaryAxisYCollection secondaryAxisYCollection;
		readonly Chart chart;
		protected override ChartCollectionBase ChartCollection { get { return secondaryAxisYCollection; } }
		public DeleteSwiftPlotSecondaryAxisYCommand(CommandManager commandManager, SwiftPlotDiagramSecondaryAxisYCollection secondaryAxisYCollection, Chart chart)
			: base(commandManager) {
			this.secondaryAxisYCollection = secondaryAxisYCollection;
			this.chart = chart;
		}
		protected override void InsertIntoCollection(int index, SwiftPlotDiagramSecondaryAxisY chartElement) {
			secondaryAxisYCollection.Insert(index, chartElement);
		}
		protected override object CreateCollectionPropertiesCache(SwiftPlotDiagramSecondaryAxisY chartElement) {
			return CommonUtils.FindViewsByAxisY(chartElement, chart);
		}
		protected override void RestoreCollectionProperties(SwiftPlotDiagramSecondaryAxisY chartElement, object properties) {
			List<XYDiagram2DSeriesViewBase> xyViewList = (List<XYDiagram2DSeriesViewBase>)properties;
			foreach (SwiftPlotSeriesViewBase view in xyViewList) {
				view.AxisY = chartElement;
			}
		}
	}
}
