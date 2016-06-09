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
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.DemoBase.Helpers {
	[ContentProperty("PopupContent")]
	class CornerPopup : Control {
		#region Dependecy Properties
		public static readonly DependencyProperty PopupContentProperty;
		public static readonly DependencyProperty PopupViewPortWidthProperty;
		public static readonly DependencyProperty PopupViewPortHeightProperty;
		public static readonly DependencyProperty PopupLeftProperty;
		public static readonly DependencyProperty PopupTopProperty;
		static CornerPopup() {
			Type ownerType = typeof(CornerPopup);
			PopupContentProperty = DependencyProperty.Register("PopupContent", typeof(object), ownerType, new PropertyMetadata(null));
			PopupViewPortWidthProperty = DependencyProperty.Register("PopupViewPortWidth", typeof(double), ownerType, new PropertyMetadata(0.0));
			PopupViewPortHeightProperty = DependencyProperty.Register("PopupViewPortHeight", typeof(double), ownerType, new PropertyMetadata(0.0));
			PopupLeftProperty = DependencyProperty.Register("PopupLeft", typeof(double), ownerType, new PropertyMetadata(0.0));
			PopupTopProperty = DependencyProperty.Register("PopupTop", typeof(double), ownerType, new PropertyMetadata(0.0));
		}
		#endregion
		ContentPresenter popupContentPresenter;
		RectangleGeometry clipRectangle;
		public CornerPopup() {
			this.SetDefaultStyleKey(typeof(CornerPopup));
		}
		public object PopupContent { get { return GetValue(PopupContentProperty); } set { SetValue(PopupContentProperty, value); } }
		public double PopupViewPortWidth { get { return (double)GetValue(PopupViewPortWidthProperty); } set { SetValue(PopupViewPortWidthProperty, value); } }
		public double PopupViewPortHeight { get { return (double)GetValue(PopupViewPortHeightProperty); } set { SetValue(PopupViewPortHeightProperty, value); } }
		public double PopupLeft { get { return (double)GetValue(PopupLeftProperty); } set { SetValue(PopupLeftProperty, value); } }
		public double PopupTop { get { return (double)GetValue(PopupTopProperty); } set { SetValue(PopupTopProperty, value); } }
		void OnPopupContentSizeChanged(object sender, SizeChangedEventArgs e) { UpdatePopupViewPort(); }
		void UpdatePopupViewPort() {
			PopupViewPortWidth = popupContentPresenter.ActualWidth;
			PopupViewPortHeight = popupContentPresenter.ActualHeight;
			PopupLeft = PopupViewPortWidth - popupContentPresenter.ActualWidth;
			PopupTop = PopupViewPortHeight - popupContentPresenter.ActualHeight;
			clipRectangle.Rect = new Rect(0, 0, PopupViewPortWidth, PopupViewPortHeight);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			popupContentPresenter = (ContentPresenter)GetTemplateChild("PopupContentPresenter");
			clipRectangle = (RectangleGeometry)GetTemplateChild("ClipRectangle");
			if(popupContentPresenter != null)
				popupContentPresenter.SizeChanged += OnPopupContentSizeChanged;
		}
	}
}
