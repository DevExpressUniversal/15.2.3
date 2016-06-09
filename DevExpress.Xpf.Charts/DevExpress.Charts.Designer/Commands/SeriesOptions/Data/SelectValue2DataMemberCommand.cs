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
	public class SelectValue2DataMemberCommand : SelectDataMemberCommandBase {
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_Value2DataMember); }
		}
		public SelectValue2DataMemberCommand(WpfChartModel model)
			: base(model) {
		}
		protected override bool CanExecute(object parameter) {
			return base.CanExecute(parameter) && (SeriesModel.DataSource != null || SeriesModel.IsSeriesTemplate) && (SeriesModel.Series is RangeAreaSeries2D || SeriesModel.Series is RangeBarSeries2D);
		}
		[ SecuritySafeCritical ]
		protected override void DesignTimeApplyInternal(IModelItem seriesAccess, object value) {
			seriesAccess.Properties["Value2DataMember"].SetValue(((DataMemberInfo)value).DataMember);
		}
		protected override void DataMemberUndoInternal(WpfChartSeriesModel model, object oldValue) {
			model.Value2DataMember = oldValue as DataMemberInfo;
		}
		protected override void DataMemberRedoInternal(WpfChartSeriesModel model, object newValue) {
			model.Value2DataMember = newValue as DataMemberInfo;
		}
		protected override void RuntimeApplyInternal(Series series, object value) {
			if (series is RangeBarSeries2D)
				((RangeBarSeries2D)series).Value2DataMember = ((DataMemberInfo)value).DataMember;
			if (series is RangeAreaSeries2D)
				((RangeAreaSeries2D)series).Value2DataMember = ((DataMemberInfo)value).DataMember;
		}
		public override string GetCommandDataMember() {
			return SeriesModel.Value2DataMember.DisplayName;
		}
		public override CommandResult DataMemberRuntimeExecute(object parameter) {
			CompositeHistoryItem resultItem = new CompositeHistoryItem();
			DataMemberInfo value2DataMember = parameter as DataMemberInfo;
			if (String.IsNullOrEmpty(value2DataMember.DataMember) || value2DataMember.DataMember == "None")
				value2DataMember = new DataMemberInfo();
			DataMemberInfo oldValue = SeriesModel.Value2DataMember;
			if (value2DataMember != oldValue) {
				ElementIndexItem[] indexItems = new ElementIndexItem[2];
				indexItems[0] = new ElementIndexItem(SeriesIndexKey, SeriesModel.GetSelfIndex());
				indexItems[1] = new ElementIndexItem(SeriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
				Series selectedSeries = SeriesModel.Series;
				if (!SeriesModel.IsSeriesTemplate && SeriesModel.Series.Points.Count > 0 && !SeriesModel.IsAutoPointsAdded) {
					RemoveSeriesPointsCommand removePoints = new RemoveSeriesPointsCommand(ChartModel);
					resultItem.HistoryItems.Add(removePoints.RuntimeExecute(null).HistoryItem);
				}
				SeriesModel.Value2DataMember = value2DataMember;
				resultItem.HistoryItems.Add(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, null, oldValue, value2DataMember));
				if (SeriesModel.IsSeriesTemplate) {
					if (String.IsNullOrEmpty(value2DataMember.DataMember) && !AllDataMembersSet()) {
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
