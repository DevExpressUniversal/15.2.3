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
	public class SelectWeightDataMemberCommand : SelectDataMemberCommandBase {
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_WeightDataMember); }
		}
		public SelectWeightDataMemberCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			return base.CanExecute(parameter) && (SeriesModel.DataSource != null || SeriesModel.IsSeriesTemplate) && (SeriesModel.Series is BubbleSeries2D || SeriesModel.Series is BubbleSeries3D);
		}
		[ SecuritySafeCritical ]
		protected override void DesignTimeApplyInternal(IModelItem seriesAccess, object value) {
			seriesAccess.Properties["WeightDataMember"].SetValue(((DataMemberInfo)value).DataMember);
		}
		protected override void DataMemberUndoInternal(WpfChartSeriesModel model, object oldValue) {
			model.WeightDataMember = oldValue as DataMemberInfo;
		}
		protected override void DataMemberRedoInternal(WpfChartSeriesModel model, object newValue) {
			model.WeightDataMember = newValue as DataMemberInfo;
		}
		protected override void RuntimeApplyInternal(Series series, object value) {
			if (series is BubbleSeries2D)
				((BubbleSeries2D)series).WeightDataMember = ((DataMemberInfo)value).DataMember;
			if (series is BubbleSeries3D)
				((BubbleSeries3D)series).WeightDataMember = ((DataMemberInfo)value).DataMember;
		}
		public override string GetCommandDataMember() {
			return SeriesModel.WeightDataMember.DisplayName;
		}
		public override CommandResult DataMemberRuntimeExecute(object parameter) {
			CompositeHistoryItem resultItem = new CompositeHistoryItem();
			DataMemberInfo weightDataMember = parameter as DataMemberInfo;
			if (String.IsNullOrEmpty(weightDataMember.DataMember) || weightDataMember.DataMember == "None")
				weightDataMember = new DataMemberInfo();
			DataMemberInfo oldValue = SeriesModel.WeightDataMember;
			if (weightDataMember != oldValue) {
				Series selectedSeries = SeriesModel.Series;
				ElementIndexItem[] indexItems = new ElementIndexItem[2];
				indexItems[0] = new ElementIndexItem(SeriesIndexKey, SeriesModel.GetSelfIndex());
				indexItems[1] = new ElementIndexItem(SeriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
				if (!SeriesModel.IsSeriesTemplate && SeriesModel.Series.Points.Count > 0 && !SeriesModel.IsAutoPointsAdded) {
					RemoveSeriesPointsCommand removePoints = new RemoveSeriesPointsCommand(ChartModel);
					resultItem.HistoryItems.Add(removePoints.RuntimeExecute(null).HistoryItem);
				}
				SeriesModel.WeightDataMember = weightDataMember;
				resultItem.HistoryItems.Add(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, null, oldValue, weightDataMember));
				if (SeriesModel.IsSeriesTemplate) {
					if (String.IsNullOrEmpty(weightDataMember.DataMember) && !AllDataMembersSet()) {
						ReplaceSeriesTempalteBySeries(resultItem);
						selectedSeries = SeriesModel.Series;
					}
				}
				else if (ShouldReplaceSeriesBySeriesTemplate()) {
					ReplaceSeriesBySeriesTempalte(resultItem);
					selectedSeries = SeriesModel.Series;
				}
				return new CommandResult(resultItem, selectedSeries, RibbonPagesNames.SeriesOptionsDataPage);
			}
			return null;
		}
	}
}
