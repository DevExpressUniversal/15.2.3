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
using System;
using System.Windows.Documents;
using System.Windows.Media;
#if !DXWINDOW
using DevExpress.Xpf.Core.Native;
#endif
#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#else
namespace DevExpress.Xpf.Core {
#endif
	public class AdornerContainer : Adorner {
		UIElement child;
		internal UIElement Child { get { return child; } }
		public AdornerContainer(UIElement adornedElement, UIElement child)
			: base(adornedElement) {
			AddVisualChild(child);
			this.child = child;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			return LayoutHelper.ArrangeElementWithSingleChild(this, finalSize);
		}
		protected override int VisualChildrenCount { get { return 1; } }
		protected override Visual GetVisualChild(int index) {
			return child;
		}
	}
	public class PositionedAdornerContainer : AdornerContainer {
		Point position;
		public PositionedAdornerContainer(UIElement adornedElement, UIElement child)
			: base(adornedElement, child) {
		}
		public Point Position { get { return position; } }
		public void UpdateLocation(Point newPos) {
			if (position.Equals(newPos))
				return;
			position = newPos;
			AdornerLayer layer = Parent as AdornerLayer;
			if (layer != null)
				layer.Update(AdornedElement);
		}
		protected override Size MeasureOverride(Size constraint) {
			return LayoutHelper.MeasureElementWithSingleChild(this, constraint);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			Child.Arrange(new Rect(position, Child.DesiredSize));
			return finalSize;
		}
	}
	public class CompatibilityAdorner : Canvas {
		public CompatibilityAdorner(UIElement child) {
			IsHitTestVisible = false;
			Children.Add(child);
		}
		public void Destroy() {
			Children.Clear();
		}
		protected UIElement Child { get { return Children[0]; } }
		Point offset;
		public Point Offset {
			get { return offset; }
			set {
				if(Offset == value)
					return;
				offset = value;
				OnOffsetChanged();
			}
		}
		protected virtual void OnOffsetChanged() {
			SetLeft(Child, Offset.X);
			SetTop(Child, Offset.Y);
		}
	}
	public class PositionedCompatibilityAdorner : CompatibilityAdorner {
		public Point Position { get; private set; }
		public PositionedCompatibilityAdorner(UIElement child) : base(child) { }
		public void UpdateLocation(Point newPos) {
			if(Position.Equals(newPos))
				return;
			Position = newPos;
			Offset = newPos;
		}
	}
	public class CompatibilityAdornerContainer : Grid {
		public CompatibilityAdorner Adorner { get; private set; }
		public void Initialize(CompatibilityAdorner adorner) {
			Adorner = adorner;
			if(!Children.Contains(Adorner)) {
				Children.Add(Adorner);
			}
		}
		public void Destroy() {
			if (Adorner != null) {
				Adorner.Destroy();
				Children.Remove(Adorner);
			}
		}
	}
}
