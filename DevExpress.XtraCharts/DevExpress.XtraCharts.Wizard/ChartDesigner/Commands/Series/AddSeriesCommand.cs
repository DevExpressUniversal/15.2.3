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

using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Designer.Native {
	public class AddSeriesCommand : AddCommandBase<Series> {
		readonly Chart chart;
		protected override ChartCollectionBase ChartCollection { get { return chart.Series; } }
		public AddSeriesCommand(CommandManager commandManager, Chart chart) : base(commandManager) {
			this.chart = chart;
		}
		protected override Series CreateChartElement(object parameter) {
			return new Series(string.Empty, (ViewType)parameter);
		}
		protected override void GenerateName(Series chartElement) {
			chartElement.Name = chart.Series.GenerateName();
		}
		protected override void AddToCollection(Series chartElement) {
			chart.Series.Add(chartElement);
		}
		public override bool CanExecute(object parameter) {
			if (parameter is ViewType)
				if (chart.Diagram == null)
					return true;
				else {
					ViewType viewType = (ViewType)parameter;
					foreach (Series series in chart.Series) {
						SeriesViewBase existingView =  series.View;
						SeriesViewBase newView = SeriesViewFactory.CreateInstance(viewType);
						return existingView.DiagramType == newView.DiagramType;
					}
				}
			return false;
		}
	}
}
