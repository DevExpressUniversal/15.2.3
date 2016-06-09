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
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.UI.Native;
using System.Linq;
using System.Collections.ObjectModel;
using DevExpress.Mvvm.Native;
using System.Collections.Specialized;
namespace DevExpress.Xpf.Diagram {
	public class DiagramToolboxControl : Control<DiagramToolboxControl> {
		const double layoutDefaultError = 2.0;
		TimeSpan calculationTimerDelay = TimeSpan.FromMilliseconds(600);
		TimeSpan calculationInterval;
		DispatcherTimer calculationTimer;
		bool enableCalculateOnItemChanged;
		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel",
			typeof(DiagramToolboxControlViewModel), typeof(DiagramToolboxControl), new PropertyMetadata(null, (d, e) => ((DiagramToolboxControl)d).OnViewModelChanged(e)));
		public static readonly DependencyProperty DiagramControlProperty = DependencyProperty.Register("DiagramControl",
			typeof(DiagramControl), typeof(DiagramToolboxControl), new PropertyMetadata(null));
		public static readonly DependencyProperty DefaultStencilsProperty = DependencyProperty.Register("DefaultStencils",
			typeof(StencilCollection), typeof(DiagramToolboxControl), new PropertyMetadata(null, (d, e) => ((DiagramToolboxControl)d).OnDefaultStencilsChanged((StencilCollection)e.OldValue)));
		public static readonly DependencyProperty IsCompactProperty = DependencyProperty.Register("IsCompact",
			typeof(bool), typeof(DiagramToolboxControl), new PropertyMetadata(false, (d, e)=> ((DiagramToolboxControl)d).OnIsCompactChanged()));
		public static readonly DependencyProperty LayoutErrorProperty =
			DependencyProperty.Register("LayoutError", typeof(double), typeof(DiagramToolboxControl), new PropertyMetadata(0.0));
		public static readonly DependencyProperty MenuViewModeProperty =
			DependencyProperty.Register("MenuViewMode", typeof(ToolboxMenuViewMode), typeof(DiagramToolboxControl), new PropertyMetadata(ToolboxMenuViewMode.ViewSelector));
		static readonly DependencyProperty ShapeContainerWidthProperty = DependencyProperty.Register("ShapeContainerWidth",
			typeof(double), typeof(DiagramToolboxControl), new PropertyMetadata(0.0d, (d, e) => ((DiagramToolboxControl)d).OnItemWidthChanged(e)));
		static readonly DependencyProperty ShapeItemWidthProperty = DependencyProperty.Register("ShapeItemWidth",
			typeof(double), typeof(DiagramToolboxControl), new PropertyMetadata(0.0d, (d, e) => ((DiagramToolboxControl)d).OnWidthChanged(e)));
		static readonly DependencyProperty VisualWidthProperty = DependencyProperty.Register("VisualWidth",
			typeof(double), typeof(DiagramToolboxControl), new PropertyMetadata(0.0d, (d, e) => ((DiagramToolboxControl)d).OnWidthChanged(e)));
		static readonly DependencyProperty ScrollingLengthProperty = DependencyProperty.Register("ScrollingLength",
			typeof(double), typeof(DiagramToolboxControl), new PropertyMetadata(0.0d, (d, e) => ((DiagramToolboxControl)d).OnItemWidthChanged(e)));
		static readonly DependencyProperty ShapePreviewModeProperty = DependencyProperty.Register("ShapePreviewMode",
			typeof(ShapeToolboxPreviewMode), typeof(DiagramToolboxControl), new PropertyMetadata(ShapeToolboxPreviewMode.IconsAndNames, (d, e) => ((DiagramToolboxControl)d).CalculateLayoutError()));
		public DiagramToolboxControlViewModel ViewModel {
			get { return (DiagramToolboxControlViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}
		public DiagramControl DiagramControl {
			get { return (DiagramControl)GetValue(DiagramControlProperty); }
			set { SetValue(DiagramControlProperty, value); }
		}
		public StencilCollection DefaultStencils {
			get { return (StencilCollection)GetValue(DefaultStencilsProperty); }
			set { SetValue(DefaultStencilsProperty, value); }
		}
		public bool IsCompact {
			get { return (bool)GetValue(IsCompactProperty); }
			set { SetValue(IsCompactProperty, value); }
		}
		public double LayoutError {
			get { return (double)GetValue(LayoutErrorProperty); }
			set { SetValue(LayoutErrorProperty, value); }
		}
		public ToolboxMenuViewMode MenuViewMode {
			get { return (ToolboxMenuViewMode)GetValue(MenuViewModeProperty); }
			set { SetValue(MenuViewModeProperty, value); }
		}
		void OnDefaultStencilsChanged(StencilCollection oldValue) {
			oldValue.Do(x => x.CollectionChanged -= OnDefaultStencilsCollectionChanged);
			DefaultStencils.Do(x => x.CollectionChanged += OnDefaultStencilsCollectionChanged);
			if(ViewModel != null)
				UpdateCheckedStencils();
		}
		void OnDefaultStencilsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateCheckedStencils();
		}
		void OnViewModelChanged(DependencyPropertyChangedEventArgs e) {
			if (e.OldValue != null)
				BindingOperations.ClearBinding(this, ShapePreviewModeProperty);
			if (e.NewValue != null) {
				UpdateCheckedStencils();
				BindingOperations.SetBinding(this, ShapePreviewModeProperty, new Binding("ShapePreviewMode") { Source = ViewModel, Mode = BindingMode.OneWay });
			}
		}
		void UpdateCheckedStencils() {
			ViewModel.CheckStencils(DefaultStencils.With(x => x.ToArray()));
		}
		void CalculateLayoutError() {
			LayoutError = GetLayoutError();
			enableCalculateOnItemChanged = true;
			calculationInterval = TimeSpan.FromMilliseconds(0);
			calculationTimer.Start();
		}
		void OnItemWidthChanged(DependencyPropertyChangedEventArgs e) {
			if (enableCalculateOnItemChanged)
				LayoutError = GetLayoutError();
		}
		void OnWidthChanged(DependencyPropertyChangedEventArgs e) {
			LayoutError = GetLayoutError();
		}
		public DiagramToolboxControl() {
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			calculationTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(100) };
			calculationTimer.Tick += Tick;
			this.Loaded += ToolboxControl_Loaded;
			this.Unloaded += ToolboxControl_Unloaded;
		}
		double GetLayoutError() {
			double visualWidth = (double)GetValue(VisualWidthProperty);
			double shapeItemWidth = (double)GetValue(ShapeItemWidthProperty);
			double shapeContainerWidth = (double)GetValue(ShapeContainerWidthProperty);
			double scrollingLength = (double)GetValue(ScrollingLengthProperty);
			if (shapeItemWidth == 0.0d || shapeContainerWidth == 0.0d || visualWidth == 0.0d) {
				return 0;
			}
			double freeSpace = visualWidth - shapeContainerWidth;
			double columnQuantity = Math.Truncate(shapeContainerWidth / shapeItemWidth);
			if (columnQuantity == 0) columnQuantity = 1;
			double calculatedShapeItemWidth = shapeContainerWidth / columnQuantity;
			double layoutError = (freeSpace < 0.5 * calculatedShapeItemWidth) ? -freeSpace : calculatedShapeItemWidth - freeSpace;
			if (columnQuantity == 1 && scrollingLength > 0)
				layoutError = scrollingLength;
			return layoutError;
		}
		void Tick(object sender, EventArgs e) {
			calculationInterval = (((int)GetLayoutError()) == 0) ? calculationInterval + calculationTimer.Interval : TimeSpan.FromMilliseconds(0);
			if (calculationInterval > calculationTimerDelay) {
				calculationTimer.Stop();
				enableCalculateOnItemChanged = false;
			}
		}
		void OnIsCompactChanged() {
			calculationTimer.Stop();
			enableCalculateOnItemChanged = false;
		}
		void ToolboxControl_Loaded(object sender, RoutedEventArgs e) {
			LayoutError = layoutDefaultError;
		}
		void ToolboxControl_Unloaded(object sender, RoutedEventArgs e) {
			calculationTimer.Tick -= Tick;
			this.Loaded -= ToolboxControl_Loaded;
			this.Unloaded -= ToolboxControl_Unloaded;
		}
	}
	public enum ToolboxMenuViewMode {
		ViewSelector,
		ViewAndOrderSelector,
		StencilsSelector,
		StensilsCompactSelector
	}
}
