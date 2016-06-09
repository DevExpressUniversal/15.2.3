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

using DevExpress.Diagram.Core;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Diagram.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Mvvm.UI.Native;
namespace DevExpress.Xpf.Diagram {
	public class AdornerLayer : Canvas, ILayer {
		protected class Adorner : IAdorner {
			readonly AdornerLayer adornerLayer;
			public readonly FrameworkElement AdornerElement;
			public Adorner(AdornerLayer adornerLayer, FrameworkElement adornerElement) {
				this.adornerLayer = adornerLayer;
				this.AdornerElement = adornerElement;
			}
			void IAdorner.Destroy() {
				adornerLayer.Destroy(this);
			}
			void IAdorner.MakeTopmost() {
				Panel.SetZIndex(AdornerElement, int.MaxValue);
			}
			Rect bounds;
			public Rect Bounds {
				get { return bounds; }
				set {
					if(bounds != value) {
						bounds = value;
						adornerLayer.UpdateAdorner(this);
					}
				}
			}
			double angleCore;
			public double Angle {
				get { return angleCore; }
				set {
					if(angleCore != value) {
						angleCore = value;
						adornerLayer.UpdateAdorner(this);
					}
				}
			}
		}
		class AdornerEx<T> : Adorner, IAdorner<T> where T : class {
			readonly T model;
			public AdornerEx(AdornerLayer adornerLayer, FrameworkElement adornerElement, T model)
				: base(adornerLayer, adornerElement) {
				this.model = model;
			}
			T IAdorner<T>.Model {
				get { return model; }
			}
		}
		public static double GetZoom(DependencyObject obj) {
			return (double)obj.GetValue(ZoomProperty);
		}
		public static void SetZoom(DependencyObject obj, double value) {
			obj.SetValue(ZoomProperty, value);
		}
		public static readonly DependencyProperty ZoomProperty;
		public static Point GetOffset(DependencyObject obj) {
			return (Point)obj.GetValue(OffsetProperty);
		}
		public static void SetOffset(DependencyObject obj, Point value) {
			obj.SetValue(OffsetProperty, value);
		}
		public static readonly DependencyProperty OffsetProperty;
		public static Point GetLocation(DependencyObject obj) {
			return (Point)obj.GetValue(LocationProperty);
		}
		public static void SetLocation(DependencyObject obj, Point value) {
			obj.SetValue(LocationProperty, value);
		}
		public static readonly DependencyProperty LocationProperty;
		public static Thickness GetAdornerMargin(DependencyObject obj) {
			return (Thickness)obj.GetValue(AdornerMarginProperty);
		}
		public static void SetAdornerMargin(DependencyObject obj, Thickness value) {
			obj.SetValue(AdornerMarginProperty, value);
		}
		public static readonly DependencyProperty AdornerMarginProperty;
		public static Size GetViewport(DependencyObject obj) {
			return (Size)obj.GetValue(ViewportProperty);
		}
		public static void SetViewport(DependencyObject obj, Size value) {
			obj.SetValue(ViewportProperty, value);
		}
		public static readonly DependencyProperty ViewportProperty;
		public static bool GetForceBoundsRounding(FrameworkElement obj) {
			return (bool)obj.GetValue(ForceBoundsRoundingProperty);
		}
		public static void SetForceBoundsRounding(FrameworkElement obj, bool value) {
			obj.SetValue(ForceBoundsRoundingProperty, value);
		}
		public static readonly DependencyProperty ForceBoundsRoundingProperty;
		static AdornerLayer() {
			DependencyPropertyRegistrator<AdornerLayer>.New()
				.RegisterAttached((FrameworkElement x) => GetAdornerMargin(x), out AdornerMarginProperty, default(Thickness))
				.RegisterAttached((FrameworkElement x) => GetZoom(x), out ZoomProperty, 1)
				.RegisterAttached((FrameworkElement x) => GetOffset(x), out OffsetProperty, default(Point))
				.RegisterAttached((FrameworkElement x) => GetViewport(x), out ViewportProperty, default(Size))
				.RegisterAttached((FrameworkElement x) => GetForceBoundsRounding(x), out ForceBoundsRoundingProperty, false)
				.RegisterAttached((FrameworkElement x) => GetLocation(x), out LocationProperty, default(Point))
				;
		}
		protected readonly DiagramControl diagram;
		List<Adorner> adorners = new List<Adorner>();
		double zoom = 1;
		Point offset = default(Point);
		Point location = default(Point);
		Size viewport = default(Size);
		public AdornerLayer(DiagramControl diagram) {
			this.diagram = diagram;
		}
		public IAdorner CreateAdorner(FrameworkElement control) {
			var adorner = new Adorner(this, control);
			return InitAdorner(control, adorner);
		}
		internal IAdorner<TInterface> CreateAdornerEx<TObject, TInterface>(TObject control)
			where TObject : FrameworkElement, TInterface
			where TInterface : class {
			var adorner = new AdornerEx<TInterface>(this, control, control);
			return InitAdorner(control, adorner);
		}
		TAdorner InitAdorner<TAdorner>(FrameworkElement control, TAdorner adorner) where TAdorner : Adorner {
			adorners.Add(adorner);
			Children.Add(control);
			return adorner;
		}
		void Destroy(Adorner adorner) {
			Children.Remove(adorner.AdornerElement);
			adorners.Remove(adorner);
		}
		void ILayer.Update(Point offset, Point location, Size viewport, double zoomFactor) {
			this.zoom = zoomFactor;
			this.offset = offset;
			this.viewport = viewport;
			this.location = location;
			adorners.ForEach(x => UpdateAdorner(x));
		}
		void UpdateAdorner(Adorner adorner) {
			if(diagram.layersHost != null) {
				Rect displayRect = GetBounds(adorner);
				adorner.AdornerElement.SetBounds(displayRect);
				adorner.AdornerElement.SetRotateTransform(adorner.Angle, displayRect.SetLocation(new Point()).GetCenter());
				SetZoom(adorner.AdornerElement, zoom);
				SetOffset(adorner.AdornerElement, offset);
				SetLocation(adorner.AdornerElement, location);
				SetViewport(adorner.AdornerElement, viewport);
			}
		}
		protected Rect GetBoundsDefault(Adorner adorner) {
			var bounds = adorner.Bounds.TransformRect(diagram.layersHost.Controller.CreateLogicToDisplayTransform());
			var margin = GetAdornerMargin(adorner.AdornerElement);
			bounds = bounds.InflateRect(margin.Invert());
			if(!GetForceBoundsRounding(adorner.AdornerElement)) return bounds;
			var top = Math.Round(bounds.Top);
			var left = Math.Round(bounds.Left);
			return new Rect(left, top, Math.Round(bounds.Right) - left, Math.Round(bounds.Bottom) - top);
		}
		protected virtual Rect GetBounds(Adorner adorner) {
			return GetBoundsDefault(adorner);
		}
#if DEBUGTEST
		public IAdorner GetAdornerForTests(FrameworkElement element) {
			return adorners.SingleOrDefault(x => x.AdornerElement == element);
		}
#endif
	}
}
