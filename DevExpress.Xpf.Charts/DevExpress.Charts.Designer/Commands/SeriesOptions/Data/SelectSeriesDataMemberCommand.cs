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
using DevExpress.Xpf.Charts.Native;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
namespace DevExpress.Charts.Designer.Native {
	public class SelectSeriesDataMemberCommand : SelectDataMemberCommandBase {
		public override string Caption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesOptions_SeriesDataMember); }
		}
		public SelectSeriesDataMemberCommand(WpfChartModel model)
			: base(model) {
		}
		Diagram GetDiagram(Series series) {
			IOwnedElement element = series as IOwnedElement;
			if (element != null)
				return element.Owner as Diagram;
			return null;
		}
		protected override bool CanExecute(object parameter) {
			return base.CanExecute(parameter) && (SeriesModel.IsSeriesTemplate || SeriesModel.IsSeriesTemplatePreview || SeriesModel.DataSource != null && SeriesModel.Series.DataSource == null && String.IsNullOrEmpty(SeriesModel.Diagram.SeriesDataMember.DataMember));
		}
		protected override void DesignTimeApplyInternal(IModelItem seriesAccess, object value) { }
		protected override void DataMemberUndoInternal(WpfChartSeriesModel model, object oldValue) {
			ChartModel.DiagramModel.Diagram.SeriesDataMember = ((DataMemberInfo)oldValue).DataMember;
		}
		protected override void DataMemberRedoInternal(WpfChartSeriesModel model, object newValue) {
			string seriesDataMember = ((DataMemberInfo)newValue).DataMember;
			ChartModel.DiagramModel.Diagram.SeriesDataMember = seriesDataMember;
			if (!String.IsNullOrEmpty(seriesDataMember) && !model.IsSeriesTemplate)
				model.IsSeriesTemplatePreview = true;
		}
		protected override void RuntimeApplyInternal(Series series, object value) {
			Diagram diagram = GetDiagram(series);
			if (diagram != null)
				diagram.SeriesDataMember = ((DataMemberInfo)value).DataMember;
		}
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			chartModelItem.Properties["Diagram"].Value.Properties["SeriesDataMember"].SetValue(historyItem.NewValue);
		}
		public override string GetCommandDataMember() {
			return SeriesModel.Diagram.SeriesDataMember.DisplayName;
		}
		public override ScaleType GetCommandScaleType() {
			return ScaleType.Auto;
		}
		public override CommandResult DataMemberRuntimeExecute(object parameter) {
			CompositeHistoryItem resultItem = new CompositeHistoryItem();
			DataMemberInfo seriesDataMember = parameter as DataMemberInfo;
			if (String.IsNullOrEmpty(seriesDataMember.DataMember) || seriesDataMember.DataMember == "None")
				seriesDataMember = new DataMemberInfo();
			DataMemberInfo oldValue = SeriesModel.Diagram.SeriesDataMember;
			if (seriesDataMember != oldValue) {
				ElementIndexItem[] indexItems = new ElementIndexItem[2];
				indexItems[0] = new ElementIndexItem(SeriesIndexKey, SeriesModel.GetSelfIndex());
				indexItems[1] = new ElementIndexItem(SeriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
				Series selectedSeries = SeriesModel.Series;
				SeriesModel.Diagram.SeriesDataMember = seriesDataMember;
				resultItem.HistoryItems.Add(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, null, oldValue, seriesDataMember));
				if (SeriesModel.IsSeriesTemplate) {
					if (String.IsNullOrEmpty(seriesDataMember.DataMember)) {
						ReplaceSeriesTempalteBySeries(resultItem);
						SeriesModel.IsSeriesTemplatePreview = false;
						selectedSeries = SeriesModel.Series;
					}
				}
				else if (!String.IsNullOrEmpty(seriesDataMember.DataMember)) {
					SeriesModel.IsSeriesTemplatePreview = true;
					if (ShouldReplaceSeriesBySeriesTemplate()) {
						ReplaceSeriesBySeriesTempalte(resultItem);
						selectedSeries = SeriesModel.Series;
					}
				}
				else
					SeriesModel.IsSeriesTemplatePreview = false;
				return new CommandResult(resultItem, selectedSeries, RibbonPagesNames.SeriesOptionsDataPage);
			}
			return null;
		}
	}
}
