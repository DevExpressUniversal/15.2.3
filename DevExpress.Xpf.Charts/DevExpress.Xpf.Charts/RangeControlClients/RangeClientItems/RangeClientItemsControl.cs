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

using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Charts.RangeControlClient.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Charts.RangeControlClient {
	public class RangeClientItemsControl : ItemsControl {
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return false;
		}
	}
	public class RangeClientItemsPanel : Panel {
		const double defaultWidth = 300;
		const double defaultHeight = 50;
		Size? clientSize = null;
		bool CanPerformLayout { get { return GetClient() != null; } }
		ChartRangeControlClient GetClient() { return DataContext as ChartRangeControlClient; }
		IRangeClientItem GetLayout(UIElement element) {
			if (element is ContentPresenter)
				return ((ContentPresenter)element).Content as IRangeClientItem;
			return null;
		}
		internal void UpdateClientSize(Size clientSize) {
			if (!this.clientSize.HasValue || this.clientSize.Value != clientSize) {
				this.clientSize = clientSize;
				InvalidateMeasure();
			}
		}
		protected override Size MeasureOverride(Size availableSize) {
			if (!CanPerformLayout)
				return base.MeasureOverride(availableSize);
			double constraintWidth = clientSize.HasValue ? clientSize.Value.Width : defaultWidth;
			double constraintHeight = clientSize.HasValue ? clientSize.Value.Height : defaultHeight;
			Size constraint = new Size(constraintWidth, constraintHeight);
			foreach (UIElement element in Children)
				element.Measure(constraint);
			return constraint;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if (!CanPerformLayout)
				return base.ArrangeOverride(finalSize);
			ChartRangeControlClient client = GetClient();
			IRangeClientScaleMap map = client.UpdateScaleMap(finalSize.Width);
			double maxHeight = 0;
			foreach (UIElement element in Children) {
				IRangeClientItem layout = GetLayout(element);
				if (layout != null && !layout.IsMaster)
					maxHeight = Math.Max(maxHeight, element.DesiredSize.Height);
			}
			Rect clientBounds = new Rect(0, 0, finalSize.Width, finalSize.Height - maxHeight);
			if (((DevExpress.Xpf.Editors.RangeControl.IRangeControlClient)client).ClientBounds != clientBounds) {
				client.SetClientSize(new Rect(0, 0, finalSize.Width, finalSize.Height - maxHeight));
				return finalSize;
			}
			List<UIElement> elements = new List<UIElement>();
			foreach (UIElement element in Children) {
				IRangeClientItem layout = GetLayout(element);
				if (layout != null && !layout.IsMaster) {
					layout.CalculateLayout(map, finalSize, element.DesiredSize);
					element.Arrange(new Rect(layout.Location, layout.Size));
				}
				else
					elements.Add(element);
			}
			foreach (UIElement element in elements) {
				IRangeClientItem layout = GetLayout(element);
				if (layout != null) {
					layout.CalculateLayout(map, new Size(finalSize.Width, Math.Max(0, finalSize.Height - maxHeight)), element.DesiredSize);
					element.Arrange(new Rect(layout.Location, layout.Size));
				}
				else {
					element.Arrange(new Rect(0, 0, finalSize.Width, Math.Max(0, finalSize.Height - maxHeight)));
				}
			}
			client.SetClientSize(new Rect(0, 0, finalSize.Width, finalSize.Height - maxHeight));
			return finalSize;
		}
	}
	public class RangeClientItemTemplateSelector : DataTemplateSelector {
		[Category(Categories.Behavior)]
		public DataTemplate LabelTemplate { get; set; }
		[Category(Categories.Behavior)]
		public DataTemplate GridLinesTemplate { get; set; }
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			if (item is RangeClientAxisLabelItem)
				return LabelTemplate;
			if (item is RangeClientGridLinesItem)
				return GridLinesTemplate;
			return null;
		}
	}
}
