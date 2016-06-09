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
	public class SetSeriesLabelConnectorThicknessCommand : SeriesOptionsCommandBase {
		readonly CreateSeriesLabelCommand createSeriesLabelCommand;
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_ConnectorThickness); }
		}
		public override string ImageName {
			get { return null; }
		}
		public SetSeriesLabelConnectorThicknessCommand(WpfChartModel chartModel)
			: base(chartModel) {
			this.createSeriesLabelCommand = new CreateSeriesLabelCommand(chartModel);
		}
		protected override bool CanExecute(object parameter) {
			return base.CanExecute(parameter) && ((SeriesModel.ComplexLabelPosition != ComplexLabelPosition.Bar2DCenter) && (SeriesModel.ComplexLabelPosition != ComplexLabelPosition.PieInside) && (SeriesModel.ComplexLabelPosition != ComplexLabelPosition.FunnelCenter) && (SeriesModel.ComplexLabelPosition != ComplexLabelPosition.Marker3DCenter) && (SeriesModel.ComplexLabelPosition != ComplexLabelPosition.Bubble2DCenter) && (SeriesModel.ComplexLabelPosition != ComplexLabelPosition.Disabled) && !(SeriesModel.Series is FinancialSeries2D) && !(SeriesModel.Series is AreaSeries3D));
		}
		[SecuritySafeCritical]
		protected override void DesignTimeApplyInternal(IModelItem seriesAccess, object value) {
			seriesAccess.Properties["Label"].Value.Properties["ConnectorThickness"].SetValue(value);
		}
		protected override void RedoInternal(WpfChartSeriesModel model, object newValue) {
			model.LabelConnectorThickness = (int)newValue;
		}
		protected override void RuntimeApplyInternal(Series series, object value) {
			series.ActualLabel.ConnectorThickness = (int)value;
		}
		protected override void UndoInternal(WpfChartSeriesModel model, object oldValue) {
			model.LabelConnectorThickness = (int)oldValue;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem compositeHistory = new CompositeHistoryItem();
			if (SeriesModel.Series.Label == null)
				compositeHistory.HistoryItems.Add(createSeriesLabelCommand.RuntimeExecute(SeriesModel).HistoryItem);
			int oldValue = SeriesModel.LabelConnectorThickness;
			SeriesModel.LabelConnectorThickness = (int)parameter;
			ElementIndexItem[] indexItems = new ElementIndexItem[2];
			indexItems[0] = new ElementIndexItem(SeriesIndexKey, SeriesModel.GetSelfIndex());
			indexItems[1] = new ElementIndexItem(SeriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
			compositeHistory.HistoryItems.Add(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, SeriesModel, oldValue, (int)parameter));
			return new CommandResult(compositeHistory, SeriesModel.Series);
		}
	}
}
