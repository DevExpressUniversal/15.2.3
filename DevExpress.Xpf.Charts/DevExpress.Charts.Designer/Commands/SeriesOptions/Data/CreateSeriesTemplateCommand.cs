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

extern alias Platform;
using System;
using System.Security;
using DevExpress.Xpf.Charts;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
namespace DevExpress.Charts.Designer.Native {
	public class CreateSeriesTemplateCommand : SeriesOptionsCommandBase {
		public override string Caption {
			get { return null; }
		}
		public override string ImageName {
			get { return null; }
		}
		public CreateSeriesTemplateCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return true;
		}
		[ SecuritySafeCritical ]
		protected override void DesignTimeApplyInternal(IModelItem seriesAccess, object value) {}
		protected override void RedoInternal(WpfChartSeriesModel model, object newValue) {
			Series seriesTemplate = (Series)newValue;
			ChartModel.DiagramModel.Diagram.SeriesTemplate = seriesTemplate;
		}
		protected override void RuntimeApplyInternal(Series series, object value) { }
		protected override void UndoInternal(WpfChartSeriesModel model, object oldValue) {
			ChartModel.DiagramModel.Diagram.SeriesTemplate = null;
		}
		[ SecuritySafeCritical ]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			IModelItem seriesTemplateItem = chartModelItem.Context.CreateItem(historyItem.NewValue.GetType());
			chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].SetValue(seriesTemplateItem);
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			chartControl.Diagram.SeriesTemplate = (Series)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			ElementIndexItem[] indexItems = new ElementIndexItem[2];
			indexItems[0] = new ElementIndexItem(SeriesIndexKey, SeriesModel.GetSelfIndex());
			indexItems[1] = new ElementIndexItem(SeriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
			Series seriesTemplate = (Series)Activator.CreateInstance(SeriesModel.Series.GetType());
			SeriesModel.Diagram.Diagram.SeriesTemplate = seriesTemplate;
			return new CommandResult(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, null, null, seriesTemplate), seriesTemplate);
		}
	}
}
