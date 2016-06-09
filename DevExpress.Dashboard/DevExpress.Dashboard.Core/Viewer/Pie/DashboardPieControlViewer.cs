#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraCharts;
namespace DevExpress.DashboardCommon.Viewer {
	public class DashboardPieControlViewer : DashboardChartControlViewerBase, IContentProvider {
		public static readonly Size DefaultPieProportions = new Size(3, 4);
		public static readonly Size DefaultPieAndLabelProportions = new Size(5, 3);
		public const int PieMinSize = 100;
		public const int PieAndLabelMinSize = 200;
		readonly CustomSimpleDiagramPanel simpleDiagramPanel = new CustomSimpleDiagramPanel();
		PieLabelsConfigurator labelsConfigurator;
		PieDashboardItemViewModel PieViewModel { get { return (PieDashboardItemViewModel)ViewModel; } }
		ViewType SeriesViewType { get { return PieViewModel.PieType == PieType.Donut ? ViewType.Doughnut : ViewType.Pie; } }
		ICollection IContentProvider.Items { get { return ChartControl.Series; } }
		Size IContentProvider.ItemProportions { get { return GetItemProportions(); } }
		Size IContentProvider.ItemMargin { get { return new Size(5, 5); } }
		int IContentProvider.ItemMinWidth { 
			get {
				if(ViewModel == null)
					return PieMinSize;
				return PieViewModel.LabelContentType == PieValueType.None ? PieMinSize : PieAndLabelMinSize; 
			}
		}
		decimal IContentProvider.BorderProportion { get { return 0; } }
		public override bool ShouldProcessInteractivity { get { return true; } }
		public override ChartLabelsConfiguratorBase LabelsConfigurator { get { return labelsConfigurator; } }
		PieViewerDataControllerBase PieDataController { get { return (PieViewerDataControllerBase)DataController; } }
		event EventHandler<ContentProviderEventArgs> Changed;
		event EventHandler<ContentProviderEventArgs> IContentProvider.Changed {
			add { Changed = (EventHandler<ContentProviderEventArgs>)Delegate.Combine(Changed, value); }
			remove { Changed = (EventHandler<ContentProviderEventArgs>)Delegate.Remove(Changed, value); }
		}
		public DashboardPieControlViewer(IDashboardChartControl chartControl)
			: base(chartControl) {
				labelsConfigurator = new PieLabelsConfigurator(this);
		}
		Size GetItemProportions() {
			if(ViewModel == null)
				return new Size(1, 1);
			if(PieViewModel.LabelContentType == PieValueType.None)
				return PieViewModel.ShowPieCaptions ? DefaultPieProportions : new Size(1, 1);
			return DefaultPieAndLabelProportions;			
		}
		void RaiseChanged() {
			if(Changed != null)
				Changed(this, new ContentProviderEventArgs(ContentProviderChangeReason.ItemPropertiesAndData));
		}
		protected override void UpdateViewModelInternal() {
			SetLegendPosition();
		}
		protected override void UpdateDataInternal() {
			labelsConfigurator.Update(PieViewModel, DataController.ArgumentType);
			ClearSeries();
			if(!DataController.IsEmpty)
				AddSeries();
		}
		void IContentProvider.SetSize(Size size) {
			ChartControl.Size = size;
		}
		void IContentProvider.BeginUpdate() {
		}
		void IContentProvider.EndUpdate() {
			ChartControl.Invalidate();
		}
		void SetLegendPosition() {
			IDashboardChartLegend legend = ChartControl.Legend;
			PieLegendPosition legendPosition = PieViewModel.LegendPosition;
			switch(legendPosition) {
				case PieLegendPosition.Left:
					legend.AlignmentVertical = LegendAlignmentVertical.Center;
					legend.AlignmentHorizontal = LegendAlignmentHorizontal.LeftOutside;
					break;
				case PieLegendPosition.Right:
					legend.AlignmentVertical = LegendAlignmentVertical.Center;
					legend.AlignmentHorizontal = LegendAlignmentHorizontal.RightOutside;
					break;
				case PieLegendPosition.Top:
					legend.AlignmentVertical = LegendAlignmentVertical.TopOutside;
					legend.AlignmentHorizontal = LegendAlignmentHorizontal.Center;
					break;
				case PieLegendPosition.Bottom:
					legend.AlignmentVertical = LegendAlignmentVertical.BottomOutside;
					legend.AlignmentHorizontal = LegendAlignmentHorizontal.Center;
					break;
				case PieLegendPosition.TopRight:
					legend.AlignmentVertical = LegendAlignmentVertical.TopOutside;
					legend.AlignmentHorizontal = LegendAlignmentHorizontal.RightOutside;
					break;
				case PieLegendPosition.None:
					break;
			}
			legend.Visibility = legendPosition == PieLegendPosition.None ? Utils.DefaultBoolean.False : Utils.DefaultBoolean.True;
		}
		PieSeries PrepareSeries(IList selectionValues, AxisPoint axisPoint, string valueDataMember, string seriesName, string argumentDataMember) {
			PieSeries series = new PieSeries(SeriesViewType, selectionValues, valueDataMember);
			series.Tag = axisPoint;			
			series.Name = seriesName;
			AddSeries(series, null, null);
			SeriesTitleCollection titles = ((PieSeriesViewBase)series.View).Titles;
			if(PieViewModel.ShowPieCaptions) {
				SeriesTitle title = new SeriesTitle();
				if(!String.IsNullOrEmpty(seriesName)) {
					title.Visibility = Utils.DefaultBoolean.True;
					title.Text = seriesName;
					titles.Add(title);
				}
			}
			series.ArgumentDataMember = argumentDataMember;
			series.ColorDataMember = ChartColorMultiDimensionalDataPropertyDescriptor.ColorMember;
			return series;
		}
		void AddSeries() {
			if(PieViewModel.ValueDataMembers.Length == 0)
				return;
			SimpleDiagram simpleDiagram = ChartControl.Diagram as SimpleDiagram;
			if(simpleDiagram == null) {
				simpleDiagram = new SimpleDiagram();
				ChartControl.Diagram = simpleDiagram;
			}
			simpleDiagram.EqualPieSize = true;
			if(!(ChartControl is ISimpleDiagramPanel))
				simpleDiagram.CustomPanel = simpleDiagramPanel;
			string argumentDataMember = PieViewModel.Argument.SummaryArgumentMember ?? ChartArgumentMultiDimensionalDataPropertyDescriptor.EmptyArgumentMember;
			string seriesDataMember = PieViewModel.SummarySeriesMember;
			IEnumerable<AxisPoint> axisPoints = GetSeriesAxisPoints(seriesDataMember);
			foreach(AxisPoint axisPoint in axisPoints) {
				IList selectionValues = PieDataController.GetRootPathUniqueValues(axisPoint);
				string[] valueDataMembers = PieDataController.GetAllValueDataMembers();
				string[] valueDisplayNames = PieDataController.GetValueDisplayNames(axisPoint);
				IList<PieSeries> seriesCollection = new List<PieSeries>();
				for(int i = 0; i < valueDataMembers.Length; i++)
					seriesCollection.Add(PrepareSeries(selectionValues, axisPoint, valueDataMembers[i], valueDisplayNames[i], argumentDataMember));
				foreach(PieSeries series in seriesCollection)
					series.DataSource = PieDataController.GetDataSource(series, null);
			}
		}
		public override void Update(DashboardItemViewModel viewModel, MultiDimensionalData data, IDictionary<string, IList> drillDownState, bool isDesignMode) {
			base.Update(viewModel, data, drillDownState, isDesignMode);
			RaiseChanged();
		}
		public string GetFormattedLabel(Series series, SeriesPoint seriesPoint, PieValueType valueType) {
			return labelsConfigurator.GetFormattedLabel(series, seriesPoint, valueType);
		}
	}
}
