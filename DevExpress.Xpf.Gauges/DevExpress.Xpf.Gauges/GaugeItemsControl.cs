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

using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Gauges {
	[NonCategorized]
	public class GaugeItemsControl : ItemsControl {
		bool stretchItemsToAvailableSize = true;
		public bool StretchItemsToAvailableSize {
			get { return stretchItemsToAvailableSize; }
			set { stretchItemsToAvailableSize = value; }
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new ElementInfoContainer() { StretchToAvailableSize = stretchItemsToAvailableSize };
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			ElementInfoContainer container = element as ElementInfoContainer;
			if (container != null) 
				container.ElementInfo = item as ElementInfoBase;
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is UIElement;
		}
	}
	public class GaugeBaseLayoutElement : Panel {
		Size arrangeSize = Size.Empty;
		bool measureInvalidated = false;
		void ArrangeChildren(Size finalSize) {
			foreach (UIElement child in Children)
				child.Arrange(new Rect(new Point(0, 0), finalSize));
		}
		protected override Size MeasureOverride(Size availableSize) {
			Size childSize = measureInvalidated ? arrangeSize : availableSize;
			foreach (UIElement child in Children)
				child.Measure(childSize);
			return availableSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if (arrangeSize != finalSize) {
				arrangeSize = finalSize;
				measureInvalidated = true;
				InvalidateMeasure();
			}
			else {
				arrangeSize = Size.Empty;
				measureInvalidated = false;
			}
			ArrangeChildren(finalSize);
			return finalSize;
		}
	}
}
