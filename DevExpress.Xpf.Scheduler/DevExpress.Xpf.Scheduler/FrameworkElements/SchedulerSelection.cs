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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using DevExpress.XtraScheduler;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.Xpf.Scheduler.Internal;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
#if SILVERLIGHT
using DevExpress.Xpf.Editors.Controls;
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Collections.Specialized;
using System.Windows.Input;
using DevExpress.XtraScheduler.Drawing;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class SchedulerSelection : Control, IVisualElement {
		List<Rect> bounds;
		public SchedulerSelection() {
			bounds = new List<Rect>();
			DefaultStyleKey = typeof(SchedulerSelection);
		}
		public Panel Root { get; private set; }
		public PathGeometry Geometry { get; private set; }
		public FrameworkElement SingleItemSelection { get; private set; }
		#region Resource
		public static readonly DependencyProperty ResourceProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerSelection, Resource>("Resource", null);
		public Resource Resource {
			get { return (Resource)GetValue(ResourceProperty); }
			set { SetValue(ResourceProperty, value); }
		}
		#endregion
		#region Interval
		public static readonly DependencyProperty IntervalProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerSelection, TimeInterval>("Interval", null);
		public TimeInterval Interval {
			get { return (TimeInterval)GetValue(IntervalProperty); }
			set { SetValue(IntervalProperty, value); }
		}
		#endregion
		public void ClearBounds() {
			bounds.Clear();
		}
		public void AddRect(Rect rect) {
			bounds.Add(rect);
		}
		void AddPath(PathSegmentCollection segments) {
			if (segments.Count == 0)
				return;
			PathFigure path = new PathFigure();
			path.IsClosed = true;
			path.IsFilled = true;
			path.StartPoint = (segments[0] as LineSegment).Point;
			segments.RemoveAt(0);
			path.Segments = segments;
			Geometry.Figures.Add(path);
			Root.Opacity = 0.99;
			Root.Opacity = 1;
		}
		public Rect GetCellBounds(int cellIndex) {
			return cellIndex >= 0 && cellIndex < bounds.Count ? bounds[cellIndex] : Rect.Empty;
		}
		void AddNewRect(PathSegmentCollection segments, Rect rect) {
			if (rect.Left == rect.Right || rect.Top == rect.Bottom)
				return;
			segments.Add(new LineSegment() { Point = new System.Windows.Point(rect.Left, rect.Bottom) });
			segments.Add(new LineSegment() { Point = new System.Windows.Point(rect.Left, rect.Top) });
			segments.Add(new LineSegment() { Point = new System.Windows.Point(rect.Right, rect.Top) });
			segments.Add(new LineSegment() { Point = new System.Windows.Point(rect.Right, rect.Bottom) });
		}
		public void Recalculate() {
			ApplyTemplate();
			RecalculateCore();
		}
		private void RecalculateCore() {
			if (Geometry == null)
				return;
			Geometry.Figures.Clear();
			Rect prev = new Rect();
			PathSegmentCollection segments = new PathSegmentCollection();
			foreach (Rect rect in bounds) {
				if (segments.Count == 0) {
					AddNewRect(segments, rect);
				}
				else {
					if (rect.Left == rect.Right || rect.Top == rect.Bottom)
						continue;
					AddPath(segments);
					segments = new PathSegmentCollection();
					AddNewRect(segments, rect);
				}
				prev = rect;
			}
			AddPath(segments);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			CreateRoot();
			CreateGeometry();
			Recalculate();
		}
		private void CreateGeometry() {
			Geometry = GetTemplateChild("Geometry") as PathGeometry;
		}
		private void CreateRoot() {
			Root = VisualTreeHelper.GetChild(this, 0) as Panel;
		}
	}
	public class MonthViewSelectionPresenter : SelectionPresenter {
		protected override CellsRectCalculator CreateCellsRectCalculator() {
			return new MonthViewSelectionCellsRectCalculator(PanelController);
		}		
	}
	public class MonthViewSelectionCellsRectCalculator : CellsRectCalculator {
		public MonthViewSelectionCellsRectCalculator(PanelController panelController)
			: base(panelController) {
		}
		protected override Rect GetCellRectCore(VisualTimeCellBase cell) {
			GeneralTransform transform = cell.TransformToVisual(PanelController.OwnerPanel);
			Rect cellRect = new Rect(0, 0, cell.ActualWidth, cell.ActualHeight);
			return transform.TransformBounds(cellRect);
		}		
	}
}
