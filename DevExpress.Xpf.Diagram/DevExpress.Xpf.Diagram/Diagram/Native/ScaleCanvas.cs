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
using DevExpress.Xpf.Diagram.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Mvvm.UI.Native;
namespace DevExpress.Xpf.Diagram {
	public class ScaleCanvas : Panel {
		public static DependencyProperty PositionProperty;
		public static Point GetPosition(FrameworkElement obj) {
			return (Point)obj.GetValue(PositionProperty);
		}
		public static void SetPosition(FrameworkElement obj, Point value) {
			obj.SetValue(PositionProperty, value);
		}
		public static readonly DependencyProperty ScaleProperty;
		public double Scale {
			get { return (double)GetValue(ScaleProperty); }
			set { SetValue(ScaleProperty, value); }
		}
		static ScaleCanvas() {
			DependencyPropertyRegistrator<ScaleCanvas>.New()
				.Register(x => x.Scale, out ScaleProperty, 1, FrameworkPropertyMetadataOptions.AffectsMeasure)
				.RegisterAttached((FrameworkElement x) => GetPosition(x), out PositionProperty, default(Point), FrameworkPropertyMetadataOptions.AffectsParentMeasure)
				;
		}
		protected override Size MeasureOverride(Size constraint) {
			Size childConstraint = new Size(double.PositiveInfinity, double.PositiveInfinity);
			foreach(UIElement child in InternalChildren) {
				child.Measure(childConstraint);
			}
			return default(Size);
		}
		protected override Size ArrangeOverride(Size arrangeSize) {
			foreach(FrameworkElement child in InternalChildren) {
				var position = GetPosition(child)
					.ScalePoint(Scale)
					.OffsetPoint(child.DesiredSize.ToPoint().ScalePoint(0.5).FloorPoint().InvertPoint());
				child.Arrange(new Rect(position, child.DesiredSize));
			}
			return arrangeSize;
		}
		protected override Geometry GetLayoutClip(Size layoutSlotSize) {
			if(ClipToBounds)
				return new RectangleGeometry(new Rect(RenderSize));
			else
				return null;
		}
	}
	public class ScaleItemsControl : ItemsControl {
		public static readonly DependencyProperty ScaleProperty;
		public double Scale {
			get { return (double)GetValue(ScaleProperty); }
			set { SetValue(ScaleProperty, value); }
		}
		static ScaleItemsControl() {
			DependencyPropertyRegistrator<ScaleItemsControl>.New()
				.Register(x => x.Scale, out ScaleProperty, 1, FrameworkPropertyMetadataOptions.AffectsMeasure)
				.OverrideDefaultStyleKey()
				;
		}
	}
}
