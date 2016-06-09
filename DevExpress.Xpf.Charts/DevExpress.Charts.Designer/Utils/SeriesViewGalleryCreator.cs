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
using System.Linq;
using System.Reflection;
using DevExpress.Xpf.Charts;
using System.Collections.Generic;
namespace DevExpress.Charts.Designer.Native {
	public abstract class SeriesViewGalleryCreatorBase {
		bool hideGroupWhenAllItemsDisabled;
		public SeriesViewGalleryCreatorBase(bool hideGroupWhenAllItemsDisabled = false){
			this.hideGroupWhenAllItemsDisabled = hideGroupWhenAllItemsDisabled;
		}
		public void CreateSeriesViewGallery(WpfChartModel chartModel, GalleryViewModelBase ownerModel) {
			GalleryGroupViewModel galleryGroupAddBarSereis = new GalleryGroupViewModel(hideGroupWhenAllItemsDisabled);
			ownerModel.Groups.Add(galleryGroupAddBarSereis);
			galleryGroupAddBarSereis.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ChartViewGallery_BarSeries);
			GalleryGroupViewModel galleryGroupAddAreaSereis = new GalleryGroupViewModel(hideGroupWhenAllItemsDisabled);
			ownerModel.Groups.Add(galleryGroupAddAreaSereis);
			galleryGroupAddAreaSereis.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ChartViewGallery_Area);
			GalleryGroupViewModel galleryGroupAddLineSereis = new GalleryGroupViewModel(hideGroupWhenAllItemsDisabled);
			ownerModel.Groups.Add(galleryGroupAddLineSereis);
			galleryGroupAddLineSereis.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ChartViewGallery_LineSeries);
			GalleryGroupViewModel galleryGroupAddRangeSereis = new GalleryGroupViewModel(hideGroupWhenAllItemsDisabled);
			ownerModel.Groups.Add(galleryGroupAddRangeSereis);
			galleryGroupAddRangeSereis.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ChartViewGallery_RangeSeries);
			GalleryGroupViewModel galleryGroupAddFinancialSereis = new GalleryGroupViewModel(hideGroupWhenAllItemsDisabled);
			ownerModel.Groups.Add(galleryGroupAddFinancialSereis);
			galleryGroupAddFinancialSereis.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ChartViewGallery_Financial);
			GalleryGroupViewModel galleryGroupAddPointBubbleSereis = new GalleryGroupViewModel(hideGroupWhenAllItemsDisabled);
			ownerModel.Groups.Add(galleryGroupAddPointBubbleSereis);
			galleryGroupAddPointBubbleSereis.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ChartViewGallery_PointBubble);
			GalleryGroupViewModel galleryGroupAddPieDoughnutSeries = new GalleryGroupViewModel(hideGroupWhenAllItemsDisabled);
			ownerModel.Groups.Add(galleryGroupAddPieDoughnutSeries);
			galleryGroupAddPieDoughnutSeries.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ChartViewGallery_Pie);
			GalleryGroupViewModel galleryGroupAddFunnelSeries = new GalleryGroupViewModel(hideGroupWhenAllItemsDisabled);
			ownerModel.Groups.Add(galleryGroupAddFunnelSeries);
			galleryGroupAddFunnelSeries.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ChartViewGallery_Funnel);
			GalleryGroupViewModel galleryGroupAddPolarSeries = new GalleryGroupViewModel(hideGroupWhenAllItemsDisabled);
			ownerModel.Groups.Add(galleryGroupAddPolarSeries);
			galleryGroupAddPolarSeries.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ChartViewGallery_Polar);
			GalleryGroupViewModel galleryGroupAddRadarSeries = new GalleryGroupViewModel(hideGroupWhenAllItemsDisabled);
			ownerModel.Groups.Add(galleryGroupAddRadarSeries);
			galleryGroupAddRadarSeries.Caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ChartViewGallery_Radar);
			List<Type> seriesTypes = new List<Type>(Assembly.GetAssembly(typeof(ChartControl)).GetTypes().Where(x => x.IsSubclassOf(typeof(Series))));
			seriesTypes.Remove(typeof(BarSideBySideSeries2D));
			seriesTypes.Insert(0, typeof(BarSideBySideSeries2D));
			foreach (Type seriesType in seriesTypes) {
				if (seriesType.IsAbstract)
					continue;
				if (seriesType.Name.Contains("Test"))
					continue;
				BarButtonViewModel galleryItemModel = CreateGalleryItem(chartModel, seriesType, ownerModel);
				galleryItemModel.CommandParameter = seriesType;
				if (seriesType.Name.Contains("Range"))
					galleryGroupAddRangeSereis.Items.Add(galleryItemModel);
				else if (seriesType.Name.Contains("Polar"))
					galleryGroupAddPolarSeries.Items.Add(galleryItemModel);
				else if (seriesType.Name.Contains("Radar"))
					galleryGroupAddRadarSeries.Items.Add(galleryItemModel);
				else if (seriesType.Name.Contains("Bar"))
					galleryGroupAddBarSereis.Items.Add(galleryItemModel);
				else if (seriesType.Name.Contains("Area"))
					galleryGroupAddAreaSereis.Items.Add(galleryItemModel);
				else if (seriesType.Name.Contains("Line") || seriesType.Name.Contains("Spline"))
					galleryGroupAddLineSereis.Items.Add(galleryItemModel);
				else if (seriesType.IsSubclassOf(typeof(FinancialSeries2D)))
					galleryGroupAddFinancialSereis.Items.Add(galleryItemModel);
				else if (seriesType.Name.Contains("Point") || seriesType.Name.Contains("Bubble"))
					galleryGroupAddPointBubbleSereis.Items.Add(galleryItemModel);
				else if (seriesType.Name.Contains("Pie") || seriesType.Name.Contains("Donut"))
					galleryGroupAddPieDoughnutSeries.Items.Add(galleryItemModel);
				else if (seriesType.Name.Contains("Funnel"))
					galleryGroupAddFunnelSeries.Items.Add(galleryItemModel);
				else
					throw new ChartDesignerException("XpfChartModel can't put '" + seriesType.Name + "' in group.");
			}
		}
		protected abstract BarButtonViewModel CreateGalleryItem(WpfChartModel chartModel, Type seriesType, GalleryViewModelBase ownerModel);
	}
	public class AddSeriesGalleryCreator : SeriesViewGalleryCreatorBase {
		protected override BarButtonViewModel CreateGalleryItem(WpfChartModel chartModel, Type seriesType, GalleryViewModelBase gallery) {
			ChartCommandBase command = new AddSeriesCommand(chartModel, seriesType);
			SplitButtonGalleryItemViewModel item = new SplitButtonGalleryItemViewModel(command, (BarSplitButtonGalleryViewModel)gallery);
			return item;
		}
	}
	public class ChangeSeriesViewGalleryCreator : SeriesViewGalleryCreatorBase {
		bool hideItemsWhenDesabled;			
		public ChangeSeriesViewGalleryCreator(bool hideItemsWhenDesabled) 
			:base(hideItemsWhenDesabled){
			this.hideItemsWhenDesabled = hideItemsWhenDesabled;
		}
		protected override BarButtonViewModel CreateGalleryItem(WpfChartModel chartModel, Type seriesType, GalleryViewModelBase gallery) {
			ChartCommandBase command = new ChangeSeriesTypeCommand(chartModel, seriesType);
			IRibbonBehavior behavior = hideItemsWhenDesabled ? new HideBehavior() : null;
			BarEditValueItemViewModel item = new BarEditValueItemViewModel(command, chartModel, "SelectedModel", new SeriesTypeConverter(), behavior);
			return item;
		}
	}
	public class ChangeChartTypeGalleryCreator : SeriesViewGalleryCreatorBase {
		protected override BarButtonViewModel CreateGalleryItem(WpfChartModel chartModel, Type seriesType, GalleryViewModelBase gallery) {
			ChartCommandBase command = new ChangeChartTypeCommand(chartModel, seriesType);
			BarButtonViewModel item = new BarButtonViewModel(command);
			return item;
		}
	}
}
