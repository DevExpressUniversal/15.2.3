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

using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System;
using System.Windows.Media;
#if !DXWINDOW
using DevExpress.Xpf.Core.Native;
#endif
#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#else
namespace DevExpress.Xpf.Core {
#endif
	[ContentProperty("Child")]
	public class NonLogicalDecorator : FrameworkElement, IAddChild {
		UIElement child;
		protected override Size ArrangeOverride(Size arrangeSize) {
			UIElement child = this.Child;
			if(child != null)
				child.Arrange(new Rect(arrangeSize));
			return arrangeSize;
		}
		protected override Visual GetVisualChild(int index) {
			if(this.child == null || index != 0) {
				throw new ArgumentOutOfRangeException("index");
			}
			return this.child;
		}
		protected override Size MeasureOverride(Size constraint) {
			UIElement child = this.Child;
			if(child != null) {
				child.Measure(constraint);
				return child.DesiredSize;
			}
			return new Size();
		}
		public virtual UIElement Child {
			get { return this.child; }
			set {
				if(this.child == value)
					return;
				base.RemoveVisualChild(this.child);
				this.child = value;
				base.AddVisualChild(value);
				base.InvalidateMeasure();
			}
		}
		protected override int VisualChildrenCount { get { return this.child != null ? 1 : 0; } }
		#region IAddChild
		void IAddChild.AddChild(object value) {
			if(!(value is UIElement))
				throw new ArgumentException("value");
			if(this.Child != null)
				throw new ArgumentException();
			this.Child = (UIElement)value;
		}
		void IAddChild.AddText(string text) { }
		#endregion
	}
}
