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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Charts {
	public class VisualContainer : ChartElementBase {
		public static readonly DependencyProperty DiagramProperty = DependencyProperty.Register("Diagram", 
			typeof(Diagram), typeof(VisualContainer), new PropertyMetadata(DiagramChanged));
		[
		Category(Categories.Common)
		]		
		public Diagram Diagram {
			get { return (Diagram)GetValue(DiagramProperty); }
			set { SetValue(DiagramProperty, value); }
		}
		static void DiagramChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (e.OldValue != null)
				return;
			Diagram3D diagram = (Diagram3D)e.NewValue;
			Diagram3D parent = LayoutHelper.FindParentObject<Diagram3D>(d) as Diagram3D;
			bool shouldAddVisualContainer = false;
			if(diagram is XYDiagram3D) {
				if(parent == null || object.ReferenceEquals(parent, diagram))
					shouldAddVisualContainer = true;
			}
			else if(diagram is SimpleDiagram3D) {
				if(parent != null && object.ReferenceEquals(parent, diagram))
					shouldAddVisualContainer = true;
			}
			if (shouldAddVisualContainer) {
				diagram.VisualContainers.RemoveAll((container) => { return !(container.DataContext is Series); });
				diagram.VisualContainers.Add((VisualContainer)d);
				diagram.InvalidateVisual();
			}
		}
		DrawingVisual visual2D = new DrawingVisual();
		Point prevOffset = new Point(0, 0);
		protected override int VisualChildrenCount { get { return 1; } }
		internal Rect Bounds { get { return new Rect(new Point(), DesiredSize); } }
		public DrawingVisual Visual2D { get { return visual2D; } }
		public VisualContainer() {
			AddVisualChild(visual2D);
			AddLogicalChild(visual2D);
		}		   
		protected T GetParent<T>(DependencyObject obj) where T : class {
			if (obj == null)
				return null;
			if (obj is T)
				return obj as T;
			DependencyObject parent = VisualTreeHelper.GetParent(obj);
			return GetParent<T>(parent);
		}
		protected virtual void Initialize() {
		}
		protected override Visual GetVisualChild(int index) {
			return (Visual)visual2D;
		}
		protected override void OnRender(DrawingContext drawingContext) {
			drawingContext.DrawRectangle(Brushes.Transparent, null, Bounds);
			base.OnRender(drawingContext);
		}
		protected override Size MeasureOverride(Size availableSize) {
			Size size = new Size();
			size.Width = double.IsInfinity(availableSize.Width) ? 0 : Math.Floor(availableSize.Width);
			size.Height = double.IsInfinity(availableSize.Height) ? 0 : Math.Floor(availableSize.Height);
			return size;
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			return arrangeBounds;
		}
		public void Clear() {
			foreach(Visual visual in visual2D.Children) {
				DrawingVisual drawingVisual = visual as DrawingVisual;
				if(drawingVisual != null)
					drawingVisual.Children.Clear();
			}
			visual2D.Children.Clear();
		}
		public void AddVisual(Visual visual) {
			visual2D.Children.Add(visual);
		}
	}
}
