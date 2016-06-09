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
namespace DevExpress.Xpf.DemoBase.Helpers {
	static class OpacityHelper {
		#region Dependency Properties
		public static readonly DependencyProperty OpacityProperty;
		static OpacityHelper() {
			Type ownerType = typeof(OpacityHelper);
			OpacityProperty = DependencyProperty.RegisterAttached("Opacity", typeof(double), ownerType, new PropertyMetadata(1.0, RaiseOpacityChanged));
		}
		#endregion
		public static double GetOpacity(UIElement uiElement) { return (double)uiElement.GetValue(OpacityProperty); }
		public static void SetOpacity(UIElement uiElement, double v) { uiElement.SetValue(OpacityProperty, v); }
		static void RaiseOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			UIElement uiElement = (UIElement)d;
			double newValue = (double)e.NewValue;
			uiElement.Opacity = newValue;
			uiElement.IsHitTestVisible = newValue > 0.0;
		}
	}
}
