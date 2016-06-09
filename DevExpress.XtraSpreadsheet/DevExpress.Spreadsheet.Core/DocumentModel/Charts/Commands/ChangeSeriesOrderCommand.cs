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
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public enum ChangeSeriesOrderType {
		BringToFront,
		BringForward,
		SendToBack,
		SendBackward
	}
	public class ChangeSeriesOrderCommand : ErrorHandledWorksheetCommand {
		#region Fields
		readonly ISeries series;
		int steps;
		int seriesCount;
		#endregion
		public ChangeSeriesOrderCommand(IErrorHandler errorHandler, ISeries series, ChangeSeriesOrderType type)
			: base(series.View.Parent.DocumentModel, errorHandler) {
			this.series = series;
			this.seriesCount = CalculateSeriesCount();
			this.steps = CalculateSteps(type);
		}
		public ChangeSeriesOrderCommand(IErrorHandler errorHandler, ISeries series, int steps)
			: base(series.View.Parent.DocumentModel, errorHandler) {
			this.series = series;
			this.seriesCount = CalculateSeriesCount();
			this.steps = steps;
		}
		#region Properties
		int OldOrder { get { return series.Order; } }
		ChartViewCollection Views { get { return series.View.Parent.Views; } }
		int ViewsCount { get { return Views.Count; } }
		#endregion
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdate();
			try {
				if (steps == 0)
					return;
				int newOrder = OldOrder + steps;
				if (newOrder >= 0 && newOrder < seriesCount)
					ChangeOrder(newOrder);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		int CalculateSeriesCount() {
			int result = 0;
			for (int i = 0; i < ViewsCount; i++)
				result += Views[i].Series.Count;
			return result;
		}
		void ChangeOrder(int newOrder) {
			if (steps > 0)
				ChangeOtherSeriesOrder(MoveBackwards);
			else
				ChangeOtherSeriesOrder(MoveForwards);
			series.Order = newOrder;
		}
		void ChangeOtherSeriesOrder(Action<SeriesBase, int> action) {
			for (int i = 0; i < ViewsCount; i++) {
				SeriesCollection series = Views[i].Series;
				int seriesCount = series.Count;
				for (int j = 0; j < seriesCount; j++) {
					SeriesBase currentSeries = series[j] as SeriesBase;
					action(currentSeries, currentSeries.Order - OldOrder);
				}
			}
		}
		void MoveBackwards(SeriesBase currentSeries, int orderDifference) {
			if (orderDifference > 0 && orderDifference <= steps)
				currentSeries.Order -= 1;
		}
		void MoveForwards(SeriesBase currentSeries, int orderDifference) {
			if (orderDifference < 0 && orderDifference >= steps)
				currentSeries.Order += 1;
		}
		int CalculateSteps(ChangeSeriesOrderType type) {
			if (type == ChangeSeriesOrderType.BringForward)
				return -1;
			else if (type == ChangeSeriesOrderType.SendBackward)
				return 1;
			else if (type == ChangeSeriesOrderType.BringToFront)
				return -OldOrder;
			return seriesCount - OldOrder - 1;
		}
	}
}
