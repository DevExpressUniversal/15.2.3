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
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	[NonCategorized]
	public abstract class SeriesPresentationBase : ChartElementBase {
		readonly SeriesItem seriesItem;
		public Series Series { get { return seriesItem.Series; } }
		public SeriesItem SeriesItem { get { return seriesItem; } }
		protected SeriesPresentationBase(SeriesItem seriesItem) {
			this.seriesItem = seriesItem;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			XYSeries series = Series as XYSeries;
			if (series != null)
				series.AdditionalGeometryHolder = GetTemplateChild("PART_AdditionalGeometryHolder") as ChartContentPresenter;
		}
	}
	public class SeriesPresentation : SeriesPresentationBase {
		internal SeriesPresentation(SeriesItem seriesItem)
			: base(seriesItem) {
			DefaultStyleKey = typeof(SeriesPresentation);
		}
	}
	public class LineAreaSeriesPresentation : SeriesPresentationBase {
		internal LineAreaSeriesPresentation(SeriesItem seriesItem)
			: base(seriesItem) {
			DefaultStyleKey = typeof(LineAreaSeriesPresentation);
		}
	}
	public class AreaStackedSeriesPresentation : SeriesPresentationBase {
		internal AreaStackedSeriesPresentation(SeriesItem seriesItem)
			: base(seriesItem) {
			DefaultStyleKey = typeof(AreaStackedSeriesPresentation);
		}
	}
	[TemplatePart(Name = "PART_TitlesControl",	 Type = typeof(TitlesLayoutControl)),
	 TemplatePart(Name = "PART_PointsContainer",   Type = typeof(ItemsControl)),
	 TemplatePart(Name = "PART_ElementsContainer", Type = typeof(PieSeries2DItemsControl)),
	 TemplatePart(Name = "PART_PointsPanel",	   Type = typeof(Panel))]
	public class PieSeries2DPresentation : SeriesPresentationBase {
		ItemsControl pointsContainer;
		public ItemsControl PointsContainer {
			get { return pointsContainer; }
		}
		internal PieSeries2DPresentation(SeriesItem seriesItem)
			: base(seriesItem) {
			DefaultStyleKey = typeof(PieSeries2DPresentation);
		}
		public override void OnApplyTemplate() {
			var titlesControl = (TitlesLayoutControl)GetTemplateChild("PART_TitlesControl");
			PieSeries2DItemsControl pieItemsControl = (PieSeries2DItemsControl)titlesControl.MasterElement; 
			pointsContainer = pieItemsControl.PointsContainer;											  
		}
	}
	public class FunnelSeriesPresentation : SeriesPresentationBase {
		internal FunnelSeriesPresentation(SeriesItem seriesItem)
			: base(seriesItem) {
			DefaultStyleKey = typeof(FunnelSeriesPresentation);
		}
	}
	public class NestedDonut2DGroupPresentation : ChartElementBase {
		NestedDonut2DGroup group;
		public NestedDonut2DGroup NestedDonutGroup {
			get { return group; }
		}
		public TitleCollection Titles {
			get { return ((NestedDonutSeries2D)group[0].Series).Titles; }
		}
		public NestedDonut2DGroupPresentation(NestedDonut2DGroup group) {
			DefaultStyleKey = typeof(NestedDonut2DGroupPresentation);
			if (group.Count == 0 || group[0].Series.GetType() != typeof(NestedDonutSeries2D))
				throw new ArgumentException("SimpleDiagram2DSeriesGroup sholuld contain at least one element. Each element shold be SeriesItem for NestedDonutSeries2D.");
			this.group = group;
		}
	}
	public class PointsContainer : ChartItemsControl {
		protected override System.Windows.DependencyObject GetContainerForItemOverride() {
			return new SeriesPointItemPresentation();
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			SeriesPointItemPresentation presentation = element as SeriesPointItemPresentation;
			if (presentation != null)
				presentation.PointItem = item as SeriesPointItem;
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is UIElement;
		}
	}
	public class PointsContainerPresenter : Control {
		public static readonly DependencyProperty ItemsSourceProperty = DependencyPropertyManager.Register("ItemsSource", typeof(IEnumerable), typeof(PointsContainerPresenter), new PropertyMetadata(null));
		[Category(Categories.Data)]
		public IEnumerable ItemsSource {
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
		public PointsContainerPresenter() {
			DefaultStyleKey = typeof(PointsContainerPresenter);
		}
	}
}
