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
using System.Windows.Data;
using DevExpress.Mvvm.UI.Interactivity;
namespace DevExpress.Xpf.Diagram {
	public class ActualWidthBehavior : Behavior<FrameworkElement> {
		public static readonly DependencyProperty ActualControlWidthProperty =
						DependencyProperty.Register("ActualControlWidth", typeof(double), typeof(ActualWidthBehavior), new PropertyMetadata(0.0));
		static readonly DependencyProperty InternalActualWidthProperty =
						DependencyProperty.Register("InternalActualWidth", typeof(double), typeof(ActualWidthBehavior),
							new PropertyMetadata(0.0, (d, e) => ((ActualWidthBehavior)d).OnInternalActualWidthChanged(e)));
		public double ActualControlWidth {
			get { return (double)GetValue(ActualControlWidthProperty); }
			set { SetValue(ActualControlWidthProperty, value); }
		}
		protected override void OnAttached() {
			base.OnAttached();
			BindingOperations.SetBinding(this, InternalActualWidthProperty, new Binding("ActualWidth") { Source = AssociatedObject, Mode = BindingMode.OneWay });
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			BindingOperations.ClearBinding(this, InternalActualWidthProperty);
		}
		void OnInternalActualWidthChanged(DependencyPropertyChangedEventArgs e) {
			SetValue(ActualControlWidthProperty, e.NewValue);
		}
	}
}
