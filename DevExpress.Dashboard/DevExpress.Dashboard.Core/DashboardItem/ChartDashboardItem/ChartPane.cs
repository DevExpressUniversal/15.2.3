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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DataAccess.Native;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	public class ChartPane : INamedItem, INameContainer, ISupportPrefix, IChartAxisContainer {
		internal const string XmlPane = "Pane";
		const string xmlName = "Name";
		const string xmlPrimaryAxisY = "AxisY";
		const string xmlSecondaryAxisY = "SecondaryAxisY";
		readonly ChartSeriesCollection series = new ChartSeriesCollection();
		readonly ChartAxisY primaryAxisY;
		readonly ChartSecondaryAxisY secondaryAxisY;
		string name;
		ChartDashboardItem dashboardItem;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartPaneSeries"),
#endif
		Category(CategoryNames.Elements),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(TypeNames.ChartSeriesCollectionEditor, typeof(UITypeEditor))
		]
		public ChartSeriesCollection Series { get { return series; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartPanePrimaryAxisY"),
#endif
		Category(CategoryNames.Elements),
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public ChartAxisY PrimaryAxisY { get { return primaryAxisY; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartPaneSecondaryAxisY"),
#endif
		Category(CategoryNames.Elements),
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public ChartSecondaryAxisY SecondaryAxisY { get { return secondaryAxisY; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartPaneName"),
#endif
		Category(CategoryNames.General),
		DefaultValue(null),
		Localizable(false)
		]
		public string Name { 
			get { return name; }
			set { 
				if (value != name) {
					if (nameChanging != null)
						nameChanging(this, new NameChangingEventArgs(value));
					name = value;
					if (dashboardItem != null)
						dashboardItem.OnChanged(ChangeReason.View, this);
				}
			}
		}
		internal ChartDashboardItem DashboardItem { get { return dashboardItem; } set { dashboardItem = value; } }
		string ISupportPrefix.Prefix { get { return DashboardLocalizer.GetString(DashboardStringId.DefaultNameChartPane); } }
		event EventHandler<NameChangingEventArgs> nameChanging;
		event EventHandler<NameChangingEventArgs> INameContainer.NameChanging { add { nameChanging += value; } remove { nameChanging -= value; } }
		public ChartPane(string name) {
			this.name = name;
			primaryAxisY = new ChartAxisY(this, true);
			secondaryAxisY = new ChartSecondaryAxisY(this);
			series.CollectionChanged += (sender, e) => OnSeriesCollectionChanged(e.AddedItems, e.RemovedItems);
		}
		public ChartPane() : this(null) {
		}
		public string GetAxisYTitle(bool isSecondary) {
			Measure defaultNameMeasure = null;
			foreach(ChartSeries item in Series) {
				if(item.PlotOnSecondaryAxis == isSecondary) {
					foreach(Measure measure in item.Measures) {
						if(measure != null) {
							if(defaultNameMeasure == null)
								defaultNameMeasure = measure;
							else
								return DashboardLocalizer.GetString(DashboardStringId.AxisYNameValues);
						}
					}
				}
			}
			if(defaultNameMeasure != null)
				return defaultNameMeasure.DisplayName;
			return String.Empty;
		}
		internal ChartPaneViewModel CreateViewModel() {
			if (dashboardItem == null)
				return null;
			IList<ChartSeriesTemplateViewModel> seriesTemplates = new List<ChartSeriesTemplateViewModel>();
			foreach(ChartSeries chartSeries in series) {
				if(chartSeries.DataItemsCount > 0) {
					ChartSeriesTemplateViewModel viewModel = chartSeries.CreateSeriesTemplateViewModel();
					viewModel.ColorMeasureID = dashboardItem.ColorMeasuresByHue ? ChartDashboardItemBase.CorrectColorMeasureId(chartSeries.ColorDefinitionName) : ChartDashboardItemBase.ColorMeasure;
					ConfigurePointMarkers(viewModel);
					seriesTemplates.Add(viewModel);
				}
			}
			if(seriesTemplates.Count > 0) {
				ChartPaneViewModel chartPaneViewModel = new ChartPaneViewModel {
					SeriesTemplates = seriesTemplates,
					Name = Name,
					SpecifySeriesTitlesWithSeriesName = Series.Count > 1 && dashboardItem.SeriesDimensions.Count > 0,
					PrimaryAxisY = primaryAxisY.CreateViewModel()
				};
				if(HasSecondaryAxis()) {
					chartPaneViewModel.SecondaryAxisY = secondaryAxisY.CreateViewModel();
					if(!HasPrimaryAxis()) {
						chartPaneViewModel.PrimaryAxisY.Visible = false;
						chartPaneViewModel.PrimaryAxisY.ShowGridLines = false;
					}
				}
				return chartPaneViewModel;
			}
			return null;
		}
		bool HasSecondaryAxis() {
			return Series.FindFirst(x => x.PlotOnSecondaryAxis) != null;
		}
		bool HasPrimaryAxis() {
			return Series.FindFirst(x => !x.PlotOnSecondaryAxis) != null;
		}
		internal XElement SaveToXml() {
			XElement element = new XElement(XmlPane);
			if(!string.IsNullOrEmpty(Name))
				element.Add(new XAttribute(xmlName, name));
			if(primaryAxisY.ShouldSerialize()) {
				XElement primaryAxisYElement = new XElement(xmlPrimaryAxisY);
				primaryAxisY.SaveToXml(primaryAxisYElement);
				element.Add(primaryAxisYElement);
			}
			if(secondaryAxisY.ShouldSerialize()) {
				XElement secondaryAxisYElement = new XElement(xmlSecondaryAxisY);
				secondaryAxisY.SaveToXml(secondaryAxisYElement);
				element.Add(secondaryAxisYElement);
			}
			series.SaveToXml(element);
			return element;
		}
		internal void LoadFromXml(XElement element) {
			name = XmlHelper.GetAttributeValue(element, xmlName);
			XElement primaryAxisYElement = element.Element(xmlPrimaryAxisY);
			if(primaryAxisYElement != null)
				primaryAxisY.LoadFromXml(primaryAxisYElement);
			XElement secondaryAxisYElement = element.Element(xmlSecondaryAxisY);
			if(secondaryAxisYElement != null)
				secondaryAxisY.LoadFromXml(secondaryAxisYElement);
			series.LoadFromXml(element);
		}
		internal void OnEndLoading() {
			series.OnEndLoading(dashboardItem);
		}
		void OnSeriesCollectionChanged(IEnumerable<ChartSeries> addedSeries, IEnumerable<ChartSeries> removedSeries) {
			foreach (ChartSeries item in addedSeries)
				item.Pane = this;
			foreach (ChartSeries item in removedSeries)
				item.Pane = null;
			if (dashboardItem != null)
				dashboardItem.OnSeriesCollectionChanged(addedSeries, removedSeries);
		}
		void ConfigurePointMarkers(ChartSeriesTemplateViewModel viewModel) {
			if(!viewModel.ShowPointMarkers) {
				List<ChartSeriesViewModelType> showPointMarkersChartSeriesTypes = new List<ChartSeriesViewModelType> { 
					ChartSeriesViewModelType.Area, 
					ChartSeriesViewModelType.FullStackedArea, 
					ChartSeriesViewModelType.FullStackedLine, 
					ChartSeriesViewModelType.FullStackedSplineArea, 
					ChartSeriesViewModelType.Line,
					ChartSeriesViewModelType.Spline,
					ChartSeriesViewModelType.SplineArea,
					ChartSeriesViewModelType.StackedArea,
					ChartSeriesViewModelType.StackedLine,
					ChartSeriesViewModelType.StackedArea,
					ChartSeriesViewModelType.StackedSplineArea,
					ChartSeriesViewModelType.StepArea,
					ChartSeriesViewModelType.StepLine,
					ChartSeriesViewModelType.RangeArea
				};
				ChartInteractivityOptions options = dashboardItem.InteractivityOptions;
				bool masterFilterOnArguments = dashboardItem.IsMasterFilterEnabled && options.TargetDimensions.HasFlag(TargetDimensions.Arguments);
				bool drillDownOnArguments = dashboardItem.IsDrillDownEnabled && options.TargetDimensions.HasFlag(TargetDimensions.Arguments);
				viewModel.ShowPointMarkers = showPointMarkersChartSeriesTypes.Contains(viewModel.SeriesType) && 
					(masterFilterOnArguments || drillDownOnArguments || dashboardItem.Arguments.Any(dim => dim.ColorByHue));
			}
		}
		#region IChartAxisContainer Members
		bool IChartAxisContainer.isReverseRequiredForContinuousScale {
			get { return false; }
		}
		void IChartAxisContainer.OnChanged(ChangeReason reason, object context) {
			if(DashboardItem != null)
				DashboardItem.OnChanged(reason, context);
		}
		string IChartAxisContainer.GetTitle(bool isSecondary) {
			return GetAxisYTitle(isSecondary);
		}
		#endregion
	}
}
