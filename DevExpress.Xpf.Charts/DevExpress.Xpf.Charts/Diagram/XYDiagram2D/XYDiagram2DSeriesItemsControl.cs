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
using System.Windows;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	[NonCategorized]
	public class DiagramSeriesItemsControl : ChartItemsControl {
		public static readonly DependencyProperty OwnerProperty = DependencyPropertyManager.Register("Owner", typeof(ChartElement), typeof(DiagramSeriesItemsControl));
		public ChartElement Owner {
			get { return (ChartElement)GetValue(OwnerProperty); }
			set { SetValue(OwnerProperty, value); }
		}
		static SeriesPresentationBase CreateSeriesPresentation(SeriesItem seriesItem) {
			Type seriesType = seriesItem.Series.GetType();
			if (seriesItem.Series is AreaStackedSeries2D)
				return new AreaStackedSeriesPresentation(seriesItem);
			if (seriesItem.Series is RangeAreaSeries2D || seriesItem.Series is AreaSeries2D ||
				seriesItem.Series is LineSeries2D || seriesItem.Series is CircularLineSeries2D ||
				seriesItem.Series is CircularAreaSeries2D)
				return new LineAreaSeriesPresentation(seriesItem);
			return new SeriesPresentation(seriesItem);
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new ChartContentPresenter();
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			ChartContentPresenter presenter = element as ChartContentPresenter;
			SeriesItem seriesItem = item as SeriesItem;
			if (presenter != null && item != null)
				presenter.Content = CreateSeriesPresentation(seriesItem);
		}
	}
}
