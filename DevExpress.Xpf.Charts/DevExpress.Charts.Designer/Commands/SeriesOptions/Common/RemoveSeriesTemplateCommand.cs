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
using System.Security;
using DevExpress.Xpf.Charts;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
namespace DevExpress.Charts.Designer.Native {
	public class RemoveSeriesTemplateCommand : SeriesOptionsCommandBase {
		public override string Caption {
			get { return null; }
		}
		public override string ImageName {
			get { return null; }
		}
		public RemoveSeriesTemplateCommand(WpfChartModel chartModel)
			: base(chartModel) { }
		protected override bool CanExecute(object parameter) {
			return true;
		}
		[ SecuritySafeCritical ]
		protected override void DesignTimeApplyInternal(IModelItem seriesAccess, object value) { }
		protected override void RedoInternal(WpfChartSeriesModel model, object newValue) {
			ChartModel.DiagramModel.Diagram.SeriesTemplate = null;
		}
		protected override void RuntimeApplyInternal(Series series, object value) { }
		protected override object RuntimeRedo(HistoryItem historyItem) {
			ChartModel.DiagramModel.Diagram.SeriesTemplate = null;
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			Series seriesTemplate = (Series)historyItem.OldValue;
			ChartModel.DiagramModel.Diagram.SeriesTemplate = seriesTemplate;
			return seriesTemplate;
		}
		protected override void UndoInternal(WpfChartSeriesModel model, object oldValue) { }
		[ SecuritySafeCritical ]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].ClearValue();
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			chartControl.Diagram.SeriesTemplate = (Series)historyItem.NewValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			Series oldValue = SeriesModel.Diagram.Diagram.SeriesTemplate;
			SeriesModel.Diagram.Diagram.SeriesTemplate = null;
			ElementIndexItem[] indexItems = new ElementIndexItem[2];
			indexItems[0] = new ElementIndexItem(SeriesIndexKey, -1);
			indexItems[1] = new ElementIndexItem(SeriesDesigntimeIndexKey, -1);
			return new CommandResult(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, null, oldValue, null));
		}
	}
}
