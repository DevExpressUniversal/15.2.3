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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	[TemplatePart(Name = "PART_LogicalTreeHolder", Type = typeof(ChartItemsControl))]
	public class SimpleDiagram2D : Diagram2D, ISimpleDiagram, IHitTestableElement {
		#region Dependency properties
		public static readonly DependencyProperty DimensionProperty = DependencyPropertyManager.Register("Dimension",
			typeof(int), typeof(SimpleDiagram2D), new FrameworkPropertyMetadata(3, ChartElementHelper.Update), new ValidateValueCallback(DimensionValidation));
		public static readonly DependencyProperty LayoutDirectionProperty = DependencyPropertyManager.Register("LayoutDirection",
			typeof(LayoutDirection), typeof(SimpleDiagram2D), new FrameworkPropertyMetadata(LayoutDirection.Horizontal, ChartElementHelper.Update));
		static readonly DependencyPropertyKey ItemsPropertyKey = DependencyPropertyManager.RegisterReadOnly("Items",
			typeof(ReadOnlyObservableCollection<object>), typeof(SimpleDiagram2D), new PropertyMetadata(null));
		public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ElementsProperty = DependencyPropertyManager.Register("Elements", typeof(ObservableCollection<object>), typeof(SimpleDiagram2D));
		#endregion
		static object DefaultGroup = new object();
		static bool DimensionValidation(object dimension) {
			return (int)dimension > 0 && (int)dimension < 100;
		}
		protected internal override Rect ActualViewport {
			get {
				double width = Math.Max(DesiredSize.Width - Margin.Left - BorderThickness.Left - Padding.Left - Padding.Right - BorderThickness.Right - Margin.Right, 0);
				double height = Math.Max(DesiredSize.Height - Margin.Top - BorderThickness.Top - Padding.Top - Padding.Bottom - BorderThickness.Bottom - Margin.Bottom, 0);
				return new Rect(0, 0, width, height);
			}
		}
		protected internal override CompatibleViewType CompatibleViewType {
			get { return CompatibleViewType.SimpleView; }
		}
		protected internal override bool IsManipulationNavigationEnabled {
			get { return ChartControl != null && ChartControl.IsManipulationEnabled; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ObservableCollection<object> Elements {
			get { return (ObservableCollection<object>)GetValue(ElementsProperty); }
			set { SetValue(ElementsProperty, value); }
		}
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ReadOnlyObservableCollection<object> Items {
			get { return (ReadOnlyObservableCollection<object>)GetValue(ItemsProperty); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SimpleDiagram2DDimension"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public int Dimension {
			get { return (int)GetValue(DimensionProperty); }
			set { SetValue(DimensionProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SimpleDiagram2DLayoutDirection"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public LayoutDirection LayoutDirection {
			get { return (LayoutDirection)GetValue(LayoutDirectionProperty); }
			set { SetValue(LayoutDirectionProperty, value); }
		}
		public SimpleDiagram2D() {
			DefaultStyleKey = typeof(SimpleDiagram2D);
			EndInit();
		}
		#region ISimpleDiagram implementation
		SimpleDiagramLayoutDirection ISimpleDiagram.LayoutDirection {
			get { return (SimpleDiagramLayoutDirection)LayoutDirection; }
		}
		#endregion
		#region IHitTestableElement implementation
		Object IHitTestableElement.Element { get { return this; } }
		Object IHitTestableElement.AdditionalElement { get { return null; } }
		#endregion
		#region ShouldSerialize
		public bool ShouldSerializeItems(XamlDesignerSerializationManager manager) {
			return false;
		}
		public bool ShouldSerializeElements(XamlDesignerSerializationManager manager) {
			return false;
		}
		#endregion
		ObservableCollection<object> GroupSeries(IList<IRefinedSeries> activeRefinedSeries) {
			Dictionary<object, NestedDonut2DGroup> groupDictionary = new Dictionary<object, NestedDonut2DGroup>();
			ObservableCollection<object> items = new ObservableCollection<object>();
			for (int i = 0; i < activeRefinedSeries.Count; i++) {
				Series series = (Series)activeRefinedSeries[i].Series;
				SeriesItem seriesItem = ((Series)series).Item;
				ISupportSeriesGroups seriesWithGroup = series as ISupportSeriesGroups;
				if (seriesWithGroup != null) {
					object groupPropertyValue = seriesWithGroup.SeriesGroup ?? DefaultGroup;
					NestedDonut2DGroup seriesGroup;
					bool exists = groupDictionary.TryGetValue(groupPropertyValue, out seriesGroup);
					if (exists)
						seriesGroup.Add(seriesItem);
					else {
						seriesGroup = new NestedDonut2DGroup(seriesItem);
						items.Add(seriesGroup);
						groupDictionary.Add(groupPropertyValue, seriesGroup);
					}
				}
				else
					items.Add(seriesItem);
			}
			return items;
		}
		protected internal override bool ManipulationStart(Point pt) {
			return IsManipulationNavigationEnabled;
		}
		protected override void EnsureSeriesItems() {
			ObservableCollection<object> newItems = GroupSeries(ViewController.ActiveRefinedSeries);
			int newCount = newItems.Count;
			ReadOnlyObservableCollection<object> oldItems = Items;
			if (oldItems == null || oldItems.Count != newCount) {
				this.SetValue(ItemsPropertyKey, new ReadOnlyObservableCollection<object>(newItems));
				return;
			}
			for (int i = 0; i < newCount; i++) {
				if (!Object.ReferenceEquals(oldItems[i], newItems[i]) && oldItems[i].GetType() != typeof(NestedDonut2DGroup)) {
					this.SetValue(ItemsPropertyKey, new ReadOnlyObservableCollection<object>(newItems));
					return;
				}
				if (oldItems[i] is NestedDonut2DGroup) {
					NestedDonut2DGroup oldGroup = (NestedDonut2DGroup)oldItems[i];
					NestedDonut2DGroup newGroup = (NestedDonut2DGroup)newItems[i];
					if (oldGroup.Count != newGroup.Count) {
						this.SetValue(ItemsPropertyKey, new ReadOnlyObservableCollection<object>(newItems));
						return;
					}
					for (int j = 0; j < newGroup.Count; j++)
						if (!Object.ReferenceEquals(oldGroup[j], newGroup[j])) {
							this.SetValue(ItemsPropertyKey, new ReadOnlyObservableCollection<object>(newItems));
							return;
						}
				}
			}
		}
		protected internal override List<VisibilityLayoutRegion> GetElementsForAutoLayout(Size size) {
			List<VisibilityLayoutRegion> models = new List<VisibilityLayoutRegion>();
			if (Series.Count == 0)
				return models;
			List<GRect2D> elementsBounds = SimpleDiagramLayout.Calculate(this, new GRect2D(0, 0, (int)size.Width, (int)size.Height), Series.Count);
			elementsBounds = CorrectElementsBounds(elementsBounds);
			for (int i = 0; i < elementsBounds.Count; i++) {
				Series series = Series[i];
				Size seriesSize = elementsBounds[i].ToRect().Size;
				models.Add(new VisibilityLayoutRegion(new GRealSize2D(seriesSize.Width, seriesSize.Height), GetElementsForAutoLayout(series)));
			}
			return models;
		}
		List<GRect2D> CorrectElementsBounds(List<GRect2D> elementsBounds) {
			elementsBounds.Sort(new RectComparer());
			GRect2D previous = elementsBounds[0];
			for (int i = 1; i < elementsBounds.Count; i++) {
				if (InFrame(previous, elementsBounds[i]))
					elementsBounds[i] = previous;
				else
					previous = elementsBounds[i];
			}
			return elementsBounds;
		}
		bool InFrame(GRect2D pattern, GRect2D rect) {
			return rect.Height < pattern.Height * 1.05 && rect.Height > pattern.Height * 0.95 &&
				rect.Width < pattern.Width * 1.05 && rect.Width > pattern.Width * 0.95;
		}
		List<ISupportVisibilityControlElement> GetElementsForAutoLayout(Series series) {
			List<ISupportVisibilityControlElement> elements = new List<ISupportVisibilityControlElement>();
			TitleCollection titles = null;
			FunnelSeries2D funnelSeries2D = series as FunnelSeries2D;
			if (funnelSeries2D != null)
				titles = funnelSeries2D.Titles;
			PieSeries pieSeries = series as PieSeries;
			if (pieSeries != null)
				titles = pieSeries.Titles;
			if (titles != null)
				foreach (Title title in titles)
					if (!title.Visible.HasValue)
						elements.Add(title);
			return elements;
		}
		protected override Size MeasureOverride(Size constraint) {
			if (ChartControl == null || ChartControl.ActualAutoLayout) {
				int seriesCount = Items.Count;
				this.Dimension = SimpleDiagramAutoLayoutHelper.CalculateDimension((int)constraint.Width, (int)constraint.Height, seriesCount);
				this.LayoutDirection = Charts.LayoutDirection.Horizontal;
			}
			if (Elements == null) {
				ObservableCollection<object> elements = new ObservableCollection<object>();
				foreach (var series in Series)
					elements.Add(series.ActualLabel);
				Elements = elements;
				var holder = (ItemsControl)GetTemplateChild("PART_LogicalTreeHolder");
				holder.ItemsSource = Elements;
			}
			var result = base.MeasureOverride(constraint);
			return result;
		}
	}
}
namespace DevExpress.Xpf.Charts.Native {
	public class RectComparer : IComparer<GRect2D> {
		int IComparer<GRect2D>.Compare(GRect2D x, GRect2D y) {
			if (x.Height == x.Height && x.Width == x.Width)
				return 0;
			if (x.Width > x.Width)
				return 1;
			if (x.Height > x.Height)
				return 1;
			return -1;
		}
	}
}
