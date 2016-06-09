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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Map.Native;
namespace DevExpress.Xpf.Map {
	[NonCategorized]
	public class OverlayPresentationControl : Control {
		public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content",
			typeof(object), typeof(OverlayPresentationControl), new PropertyMetadata());
		public object Content {
			get { return GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		internal IOverlayInfo OverlayInfo { get; set; }
		public OverlayPresentationControl() {
			DefaultStyleKey = typeof(OverlayPresentationControl);
		}
	}
	public class MapOverlaysPanel : Panel {
		const double defaultWidth = 300.0;
		const double defaultHeight = 300.0;
		IOverlayInfo GetOverlayInfo(FrameworkElement element) { 
			OverlayPresentationControl presentation = element as OverlayPresentationControl;
			return presentation != null ? presentation.OverlayInfo : element as IOverlayInfo;
		}
		protected override Size MeasureOverride(Size availableSize) {
			double constraintWidth = double.IsInfinity(availableSize.Width) ? defaultWidth : availableSize.Width;
			double constraintHeight = double.IsInfinity(availableSize.Height) ? defaultHeight : availableSize.Height;
			Size constraint = new Size(constraintWidth, constraintHeight);
			OverlayLayoutCalculator calculator = new OverlayLayoutCalculator();
			foreach (FrameworkElement child in Children) {
				IOverlayInfo overlay = GetOverlayInfo(child);
				if (overlay != null) {
					child.Measure(constraint);
					overlay.Layout.Size = child.DesiredSize;
					calculator.PushOverlay(overlay);
				}
			}
			calculator.CalculateLayout(constraint);
			return constraint;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			foreach (FrameworkElement child in Children) {
				IOverlayInfo overlay = GetOverlayInfo(child);
				if (overlay != null) {
					MapOverlayLayout layout = overlay.Layout;
					child.Arrange(new Rect(layout.Location, layout.Size));
				}
			}
			return finalSize;
		}
	}
	public class OverlayItemsControl : ItemsControl {
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			IOverlayInfo overlay = item as IOverlayInfo;
			if (overlay != null) {
				Control presentationControl = overlay.GetPresentationControl();
				OverlayPresentationControl presenter = element as OverlayPresentationControl;
				if (presenter != null) {
					presenter.Content = presentationControl;
					presenter.OverlayInfo = overlay;
				}
			}
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new OverlayPresentationControl();
		}
	}
}
