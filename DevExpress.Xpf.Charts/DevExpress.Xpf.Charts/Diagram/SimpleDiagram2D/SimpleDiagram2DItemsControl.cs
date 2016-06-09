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
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class SimpleDiagram2DItemsControl : ChartItemsControl {
		public static readonly DependencyProperty DiagramProperty = DependencyPropertyManager.Register("Diagram",
			typeof(SimpleDiagram2D), typeof(SimpleDiagram2DItemsControl));
		[
		Category(Categories.Common)]
		public SimpleDiagram2D Diagram {
			get { return (SimpleDiagram2D)GetValue(DiagramProperty); }
			set { SetValue(DiagramProperty, value); }
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new ChartContentPresenter();
		}
		ChartElementBase GetPresentation(object item) {
			if (item is SeriesItem) {
				SeriesItem seriesItem = (SeriesItem)item;
				Type seriesType = seriesItem.Series.GetType();
				if (seriesType == typeof(PieSeries2D)) {
					var presentation = new PieSeries2DPresentation(seriesItem);
					((PieSeries2D)seriesItem.Series).Presentation = presentation;
					return presentation;
				}
				else if (seriesType == typeof(FunnelSeries2D))
					return new FunnelSeriesPresentation(seriesItem);
				else {
					string method = MethodInfo.GetCurrentMethod().DeclaringType + "." + MethodInfo.GetCurrentMethod().Name;
					throw new NotImplementedException("The " + method + " method can't operate with " + item.GetType().Name);
				}
			}
			else if (item is NestedDonut2DGroup)
				return new NestedDonut2DGroupPresentation((NestedDonut2DGroup)item);
			else {
				string method = MethodInfo.GetCurrentMethod().DeclaringType + "." + MethodInfo.GetCurrentMethod().Name;
				throw new NotImplementedException("The " + method + " method can't operate with " + item.GetType().Name);
			}
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {		  
			var contentPresenter = (ChartContentPresenter)element;
			Binding binding = new Binding("DataContext") { Source = Diagram.ChartControl };
			contentPresenter.SetBinding(FrameworkElement.DataContextProperty, binding);
			contentPresenter.Content = GetPresentation(item);
		}
	}
	public class SimpleDiagram2DPanel : Panel {
		readonly Thickness CellMargin = new Thickness(-1);
		List<GRect2D> groupBounds;
		SimpleDiagram2D GetDiagram() {
			SimpleDiagram2DItemsControl parentItemsControl = LayoutHelper.FindParentObject<SimpleDiagram2DItemsControl>(this);
			return parentItemsControl != null ? parentItemsControl.Diagram : null;
		}
		bool UseDefaultMeasure(Size constraint) {
			Diagram diagram = GetDiagram();
			return double.IsInfinity(constraint.Width) ||
				   double.IsInfinity(constraint.Height) ||
				   diagram == null ||
				   Children.Count == 0;
		}
		protected override Size MeasureOverride(Size constraint) {
			if (UseDefaultMeasure(constraint))
				return base.MeasureOverride(constraint);
			GRect2D diagramBounds = new GRect2D(0, 0, MathUtils.StrongRound(constraint.Width), MathUtils.StrongRound(constraint.Height));
			groupBounds = SimpleDiagramLayout.Calculate(GetDiagram(), diagramBounds, Children.Count);
			ChartDebug.Assert(groupBounds.Count == Children.Count);
			for (int i = 0; i < groupBounds.Count; i++) {
				groupBounds[i] = groupBounds[i].Inflate(CellMargin);
				Children[i].Measure(groupBounds[i].ToRect().Size());
			}
			return constraint;
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			SimpleDiagram2D diagram = GetDiagram();
			if (diagram != null && Children.Count > 0)
				for (int i = 0; i < groupBounds.Count; i++)
					Children[i].Arrange(groupBounds[i].ToRect());
			return arrangeBounds;
		}
	}
}
