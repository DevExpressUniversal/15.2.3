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
using System.Windows.Media;
#if !SILVERLIGHT
using DevExpress.Xpf.Utils;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid {
	public class DragIndicatorPanel : Panel {
		public static readonly DependencyProperty DropPlaceOrientationProperty;
		static DragIndicatorPanel() {
			DropPlaceOrientationProperty = DependencyProperty.Register("DropPlaceOrientation", typeof(Orientation), typeof(DragIndicatorPanel), new PropertyMetadata(Orientation.Horizontal, (d, e) => ((DragIndicatorPanel)d).InvalidateMeasure()));
		}
		public Orientation DropPlaceOrientation {
			get { return (Orientation)GetValue(DropPlaceOrientationProperty); }
			set { SetValue(DropPlaceOrientationProperty, value); }
		}
		protected override Size MeasureOverride(Size availableSize) {
			if(Children.Count == 0)
				return Size.Empty;
			UIElement child = Children[0];
			child.Measure(GetCorrectSize(availableSize));
			return GetCorrectSize(child.DesiredSize);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if(Children.Count == 0)
				return Size.Empty;
			UIElement child = Children[0];
			Size arrangeSize = GetCorrectSize(finalSize);
			child.Arrange(new Rect(0, 0, arrangeSize.Width, arrangeSize.Height));
			RenderTransform = GetTransform();
			return GetCorrectSize(arrangeSize);
		}
		Size GetCorrectSize(Size size) {
			return DropPlaceOrientation == Orientation.Horizontal ? size : new Size(size.Height, size.Width);
		}
		Transform GetTransform() {
			return DropPlaceOrientation == Orientation.Horizontal ? GetTransformHorizontal() : GetTransformVertical();
		}
		Transform GetTransformVertical() {
			return new RotateTransform() { Angle = -90, CenterX = 0.5, CenterY = 0.5 };
		}
		Transform GetTransformHorizontal() {
			return new MatrixTransform();
		}
	}
	public class DragGridColumnHeader : BaseGridHeader {
		public DragGridColumnHeader() {
			this.SetDefaultStyleKey(typeof(DragGridColumnHeader));
		}
		protected override void UpdateHasTopElement() {
			HasTopElement = false;
		}
#if !SL
		protected override DevExpress.Xpf.Core.XPFContentControl CreateCustomHeaderPresenter() {
			return new HeaderContentControl();
		}
#endif
	}
}
