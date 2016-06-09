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
using System.Windows.Documents;
using System.Windows.Media;
#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#else
namespace DevExpress.Xpf.Core {
#endif
	public abstract class BaseSurfacedAdorner : Adorner, IDisposable {
		AdornerLayer adornerLayerCore;
		bool visibleCore;
		protected BaseSurfacedAdorner(UIElement container) :
			base(container) {
			Surface = CreateAdornerSurface();
			AddVisualChild(Surface);
		}
		bool isDisposing;
		protected bool IsDisposing { get { return isDisposing; } }
		void IDisposable.Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				OnDispose();
				adornerLayerCore = null;
			}
			GC.SuppressFinalize(this);
		}
		protected virtual void OnDispose() { }
		protected override void OnVisualParentChanged(DependencyObject oldParent) {
			base.OnVisualParentChanged(oldParent);
			if(oldParent != null)
				adornerLayerCore = null;
		}
		protected abstract BaseAdornerSurface CreateAdornerSurface();
		protected virtual AdornerLayer FindAdornerLayer(UIElement adornedElement) {
			return AdornerHelper.FindAdornerLayer(adornedElement);
		}
		protected AdornerLayer AdornerLayer {
			get {
				if(adornerLayerCore == null) {
					adornerLayerCore = FindAdornerLayer(AdornedElement);
					if(adornerLayerCore == null)
						throw new InvalidOperationException("Adorned element has no adorner layer");
				}
				return adornerLayerCore;
			}
		}
		public bool IsActivated {
			get { return adornerLayerCore != null; }
		}
		public void Activate() {
			if(IsActivated) return;
			if(AdornerLayer != null) {
				AdornerLayer.Add(this);
				OnActivated();
			}
		}
		public void Deactivate() {
			if(!IsActivated) return;
			if(AdornerLayer != null) {
				AdornerLayer.Remove(this);
				OnDeactivated();
			}
		}
		public void Update(bool visible) {
			Visible = visible;
			Update();
		}
		public void Update() {
			if(!IsActivated) return;
			Surface.InvalidateArrange();
		}
		protected virtual void OnActivated() { }
		protected virtual void OnDeactivated() { }
		protected BaseAdornerSurface Surface { get; private set; }
		protected override Size ArrangeOverride(Size finalSize) {
			Size elementSize = AdornedElement.RenderSize;
			Surface.Arrange(new Rect(0, 0, elementSize.Width, elementSize.Height));
			return finalSize;
		}
		protected override Size MeasureOverride(Size constraint) {
			return new Size(0.0, 0.0);
		}
		protected override Visual GetVisualChild(int index) {
			return Surface;
		}
		protected override int VisualChildrenCount {
			get { return 1; }
		}
		public bool Visible {
			get { return visibleCore; }
			set {
				visibleCore = value;
				CheckSurfaceVisibility();
			}
		}
		protected void CheckSurfaceVisibility() {
			Visibility needed = Visible ? Visibility.Visible : Visibility.Hidden;
			if(Surface.Visibility != needed) Surface.Visibility = needed;
		}
		#region Internal classes
		public abstract class BaseAdornerSurface : Panel {
			protected readonly BaseSurfacedAdorner BaseAdorner;
			protected BaseAdornerSurface(BaseSurfacedAdorner adorner) {
				BaseAdorner = adorner;
			}
		}
		#endregion
	}
}
