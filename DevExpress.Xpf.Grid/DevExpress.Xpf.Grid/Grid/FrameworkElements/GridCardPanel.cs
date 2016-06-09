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
#if !SILVERLIGHT
using DevExpress.Xpf.Utils;
using System.Windows.Media;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid {
#if !SL
	public class GridCardPanel : Panel {
		public static readonly DependencyProperty HeaderProperty;
		public static readonly DependencyProperty BodyProperty;
		public static readonly DependencyProperty IsExpandedProperty;
		public static readonly DependencyProperty RotateOnCollapseProperty;
		static GridCardPanel() {
			HeaderProperty = DependencyProperty.Register("Header", typeof(UIElement), typeof(GridCardPanel), new PropertyMetadata(null, (d, e) => ((GridCardPanel)d).OnChildChanged((UIElement)e.OldValue, (UIElement)e.NewValue)));
			BodyProperty = DependencyProperty.Register("Body", typeof(UIElement), typeof(GridCardPanel), new PropertyMetadata(null, (d, e) => ((GridCardPanel)d).OnBodyChanged((UIElement)e.OldValue)));
			IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(GridCardPanel), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => ((GridCardPanel)d).UpdateBodyVisibility()));
			RotateOnCollapseProperty = DependencyProperty.Register("RotateOnCollapse", typeof(bool), typeof(GridCardPanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));
		}
		public UIElement Header {
			get { return (UIElement)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
		public UIElement Body {
			get { return (UIElement)GetValue(BodyProperty); }
			set { SetValue(BodyProperty, value); }
		}
		public bool IsExpanded {
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		public bool RotateOnCollapse {
			get { return (bool)GetValue(RotateOnCollapseProperty); }
			set { SetValue(RotateOnCollapseProperty, value); }
		}
		void OnBodyChanged(UIElement oldValue) {
			OnChildChanged(oldValue, Body);
			UpdateBodyVisibility();
		}
		void OnChildChanged(UIElement oldValue, UIElement newValue) {
			if(oldValue != null)
				Children.Remove(oldValue);
			if(newValue != null)
				Children.Add(newValue);
		}
		void UpdateBodyVisibility() {
			if(Body == null)
				return;
			if(IsExpanded)
				Body.Visibility = System.Windows.Visibility.Visible;
			else
				Body.Visibility = System.Windows.Visibility.Hidden;
		}
		protected override Size MeasureOverride(Size availableSize) {
			Header.Measure(availableSize);
			Body.Measure(new Size(availableSize.Width, availableSize.Height - Header.DesiredSize.Height));
			if(RotateOnCollapse && !IsExpanded) {
				Header.InvalidateMeasure();
				Header.Measure(new Size(Header.DesiredSize.Height + Body.DesiredSize.Height, double.PositiveInfinity));
				return new Size(Header.DesiredSize.Height, Header.DesiredSize.Height + Body.DesiredSize.Height);
			}
			return new Size(Math.Max(Header.DesiredSize.Width, Body.DesiredSize.Width), Header.DesiredSize.Height + (IsExpanded ? Body.DesiredSize.Height : 0));
		}
		protected override Size ArrangeOverride(Size finalSize) {
			TransformGroup transformGroup = null;
			if(!IsExpanded && RotateOnCollapse) {
				transformGroup = new TransformGroup();
				transformGroup.Children.Add(new RotateTransform() { Angle = -90 });
				transformGroup.Children.Add(new TranslateTransform() { Y = finalSize.Height });
			}
			Header.RenderTransform = transformGroup ?? Transform.Identity;
			Rect rect = IsExpanded || !RotateOnCollapse ? new Rect(0, 0, finalSize.Width, Header.DesiredSize.Height) : new Rect(0, 0, finalSize.Height, finalSize.Width);
			Header.Arrange(rect);
			rect = IsExpanded ? new Rect(0, Header.DesiredSize.Height, finalSize.Width, finalSize.Height - Header.DesiredSize.Height) : new Rect();
			Body.Arrange(rect);
			return finalSize;
		}
	}
#endif
}
