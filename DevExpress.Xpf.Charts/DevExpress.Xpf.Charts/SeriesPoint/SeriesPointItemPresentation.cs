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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	[TemplatePart(Name = "PART_PointItemContainer", Type = typeof(SeriesPointItemContainer)),
	TemplatePart(Name = "PART_PointPresenter", Type = typeof(ChartContentPresenter))]
	public class SeriesPointItemPresentation : Panel, IHitTestableElement {
		public static readonly DependencyProperty PointItemProperty = DependencyPropertyManager.Register("PointItem",
			typeof(SeriesPointItem), typeof(SeriesPointItemPresentation), new FrameworkPropertyMetadata(null, PointItemPropertyChanged));
		public static readonly DependencyProperty ModelProperty = DependencyPropertyManager.Register("Model",
			typeof(PointModel), typeof(SeriesPointItemPresentation), new FrameworkPropertyMetadata(null, ModelControlPropertyChanged));
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public SeriesPointItem PointItem {
			get { return (SeriesPointItem)GetValue(PointItemProperty); }
			set { SetValue(PointItemProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public PointModel Model {
			get { return (PointModel)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}
		static void PointItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SeriesPointItemPresentation presentation = d as SeriesPointItemPresentation;
			if (presentation != null)
				presentation.UpdatePointItem(e.NewValue as SeriesPointItem);
		}
		static void ModelControlPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SeriesPointItemPresentation presentation = d as SeriesPointItemPresentation;
			if (presentation != null)
				presentation.UpdateModelControl(e.NewValue as PointModel);
		}
		ModelControl modelControl = null;
		Series Series {
			get { return PointItem != null ? PointItem.Series : null; }
		}
		internal bool CanLayout {
			get { return PointItem != null && PointItem.Layout != null && Series != null && modelControl != null; }
		}
		#region IHitTestableElement implementation
		Object IHitTestableElement.Element { get { return Series; } }
		Object IHitTestableElement.AdditionalElement { get { return PointItem != null ? PointItem.SeriesPointData.RefinedPoint : null; } }
		#endregion
		public SeriesPointItemPresentation() {
			Binding binding = new Binding("PointItem.Model") { Source = this };
			SetBinding(ModelProperty, binding);
		}
		void UpdateModelControl(PointModel model) {
			Children.Clear();
			modelControl = null;
			if (model != null) {
				modelControl = model.CreateModelControl();
				modelControl.SetPointItemBinding(PointItem);
				Children.Add(modelControl);
			}
		}
		void UpdatePointItem(SeriesPointItem pointItem) {
			if (modelControl != null)
				modelControl.SetPointItemBinding(PointItem);
			if (pointItem != null)
				pointItem.PointItemPresentation = this;
		}
		protected override Size MeasureOverride(Size availableSize) {
			SeriesPointItem item = PointItem;
			if (item != null)
				item.Opacity = item.SeriesPointData.GetPointOpacity();
			Size size;
			TransformGroup transform = new TransformGroup();
			Geometry clip = null;
			if (CanLayout) {
				Series.CompletePointLayout(item);
				size = new Size(item.Layout.Bounds.Width, item.Layout.Bounds.Height);
				Transform flippedTransform = modelControl.GetFlippedTransform(size);
				transform.Children.Add(flippedTransform);
				clip = item.Layout.ClipGeometry;
				if (clip != null)
					clip.Transform = flippedTransform;
				if (item.Layout.Transform != null)
					transform.Children.Add(item.Layout.Transform);
			}
			else
				size = new Size(0, 0);
			if (modelControl != null) {
				modelControl.Clip = clip;
				modelControl.RenderTransform = transform;
				if (modelControl is SimpleFunnel2DModelControl && CanLayout)
					((SimpleFunnel2DModelControl)modelControl).SetGeometries(PointItem.Layout.ClipGeometry, PointItem.Layout.ClipGeometry);
				modelControl.Measure(size);
				return new Size(MathUtils.ConvertInfinityToDefault(availableSize.Width, modelControl.DesiredSize.Width),
					MathUtils.ConvertInfinityToDefault(availableSize.Height, modelControl.DesiredSize.Height));
			}
			return new Size(MathUtils.ConvertInfinityToDefault(availableSize.Width, 0.0),
					MathUtils.ConvertInfinityToDefault(availableSize.Height, 0.0));
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if (CanLayout)
				modelControl.Arrange(PointItem.Layout.Bounds);
			else if (modelControl != null)
				modelControl.Arrange(RectExtensions.Zero);
			return finalSize;
		}
	}
}
