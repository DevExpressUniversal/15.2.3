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
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using System.ComponentModel;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using System.Windows.Controls;
using System.Windows.Shapes;
using DevExpress.Diagram.Core;
using ResizeMode = DevExpress.Diagram.Core.ResizeMode;
using DevExpress.Mvvm.UI.Native;
namespace DevExpress.Xpf.Diagram.Native {
	public abstract class BoundsSnapLineAdorner : Control {
		static BoundsSnapLineAdorner() {
			DependencyPropertyRegistrator<BoundsSnapLineAdorner>.New()
				.Register(x => x.Extent, out ExtentProperty, 0, x => x.Update())
				.RegisterReadOnly(x => x.From, out FromPropertyKey, out FromProperty, default(Point))
				.RegisterReadOnly(x => x.To, out ToPropertyKey, out ToProperty, default(Point))
				.OverrideMetadata(Ruler.ZoomProperty, x => x.Update())
			;
		}
		public double Extent {
			get { return (double)GetValue(ExtentProperty); }
			set { SetValue(ExtentProperty, value); }
		}
		public static readonly DependencyProperty ExtentProperty;
		public Point From {
			get { return (Point)GetValue(FromProperty); }
			private set { SetValue(FromPropertyKey, value); }
		}
		public static readonly DependencyProperty FromProperty;
		static readonly DependencyPropertyKey FromPropertyKey;
		public Point To {
			get { return (Point)GetValue(ToProperty); }
			private set { SetValue(ToPropertyKey, value); }
		}
		public static readonly DependencyProperty ToProperty;
		static readonly DependencyPropertyKey ToPropertyKey;
		Orientation Orientation { get { return snapLine.Orientation; } }
		readonly BoundsSnapLine snapLine;
#if DEBUGTEST
		public BoundsSnapLine SnapLineForTests { get { return snapLine; } }
#endif
		protected BoundsSnapLineAdorner(BoundsSnapLine snapLine) {
			this.snapLine = snapLine;
			Update();
		}
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
			base.OnRenderSizeChanged(sizeInfo);
			Update();
		}
		void Update() {
			var length = snapLine.length * Ruler.GetZoom(this);
			Orientation.SetSize(this, length + Extent);
			var margin = Orientation.Rotate().MakeThickness(snapLine.Side == Side.Far ? -1 : 0, snapLine.Side == Side.Far ? 0 : -1);
			AdornerLayer.SetAdornerMargin(this, margin);
			var start = Orientation.Rotate().SetPoint(default(Point), 0.5);
			From = Orientation.SetPoint(start, -Extent);
			To = Orientation.SetPoint(start, length + Extent);
		}
	}
	public class HorizontalBoundsSnapLineAdorner : BoundsSnapLineAdorner {
		static HorizontalBoundsSnapLineAdorner() {
			DependencyPropertyRegistrator<HorizontalBoundsSnapLineAdorner>.New().OverrideDefaultStyleKey();
		}
		public HorizontalBoundsSnapLineAdorner(BoundsSnapLine snapLine)
			: base(snapLine) {
		}
	}
	public class VerticalBoundsSnapLineAdorner : BoundsSnapLineAdorner {
		static VerticalBoundsSnapLineAdorner() {
			DependencyPropertyRegistrator<VerticalBoundsSnapLineAdorner>.New().OverrideDefaultStyleKey();
		}
		public VerticalBoundsSnapLineAdorner(BoundsSnapLine snapLine)
			: base(snapLine) {
		}
	}
	public class HorizontalSizeSnapLineAdorner : Control<HorizontalSizeSnapLineAdorner> {
	}
	public class VerticalSizeSnapLineAdorner : Control<VerticalSizeSnapLineAdorner> {
	}
	public static class OrientationFrameworkExtensions {
		public static double GetSize(this Orientation orientation, FrameworkElement element) {
			if(orientation == Orientation.Horizontal)
				return element.Width;
			else
				return element.Height;
		}
		public static void SetSize(this Orientation orientation, FrameworkElement element, double value) {
			if(orientation == Orientation.Horizontal)
				element.Width = value;
			else
				element.Height = value;
		}
		public static double GetMinSize(this Orientation orientation, FrameworkElement element) {
			if(orientation == Orientation.Horizontal)
				return element.MinWidth;
			else
				return element.MinHeight;
		}
		public static void SetMinSize(this Orientation orientation, FrameworkElement element, double value) {
			if(orientation == Orientation.Horizontal)
				element.MinWidth = value;
			else
				element.MinHeight = value;
		}
	}
}
