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
	public class AddPaneCommand : AddCommandBase<XYDiagramPane> {
		readonly XYDiagramPaneCollection paneCollection;
		protected override ChartCollectionBase ChartCollection { get { return paneCollection; } }
		public AddPaneCommand(CommandManager commandManager, XYDiagramPaneCollection paneCollection)
			: base(commandManager) {
			this.paneCollection = paneCollection;
		}
		protected override XYDiagramPane CreateChartElement(object parameter) {
			return new XYDiagramPane();
		}
		protected override void AddToCollection(XYDiagramPane chartElement) {
			paneCollection.Add(chartElement);
		}
	}
	public class DeletePaneCommand : DeleteCommandBase<XYDiagramPane> {
		readonly XYDiagramPaneCollection paneCollection;
		readonly Chart chart;
		protected override ChartCollectionBase ChartCollection { get { return paneCollection; } }
		public DeletePaneCommand(CommandManager commandManager, XYDiagramPaneCollection paneCollection, Chart chart)
			: base(commandManager) {
			this.paneCollection = paneCollection;
			this.chart = chart;
		}
		protected override void InsertIntoCollection(int index, XYDiagramPane chartElement) {
			paneCollection.Insert(index, chartElement);
		}
		protected override object CreateCollectionPropertiesCache(XYDiagramPane chartElement) {
			return CommonUtils.FindViewsByPane(chartElement, chart);
		}
		protected override void RestoreCollectionProperties(XYDiagramPane chartElement, object properties) {
			List<XYDiagram2DSeriesViewBase> xyViewList = (List<XYDiagram2DSeriesViewBase>)properties;
			foreach (XYDiagramSeriesViewBase view in xyViewList) {
				view.Pane = chartElement;
			}
		}
	}
}
