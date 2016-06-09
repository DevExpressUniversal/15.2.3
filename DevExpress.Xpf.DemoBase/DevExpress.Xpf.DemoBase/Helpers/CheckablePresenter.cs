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
namespace DevExpress.Xpf.DemoBase.Helpers {
	class CheckablePresenter : ContentPresenter {
		#region Dependency Properties
		public static readonly DependencyProperty ContentOpacityProperty;
		public static readonly DependencyProperty CheckedProperty;
		static CheckablePresenter() {
			Type ownerType = typeof(CheckablePresenter);
			ContentOpacityProperty = DependencyProperty.Register("ContentOpacity", typeof(double), ownerType, new PropertyMetadata(1.0,
				(d, e) => ((CheckablePresenter)d).RaiseContentOpacityChanged(e)));
			CheckedProperty = DependencyProperty.Register("Checked", typeof(bool), ownerType, new PropertyMetadata(false,
				(d, e) => ((CheckablePresenter)d).RaiseCheckedChanged(e)));
		}
		#endregion
		public CheckablePresenter() : base() { }
		public bool Checked { get { return (bool)GetValue(CheckedProperty); } set { SetValue(CheckedProperty, value); } }
		public double ContentOpacity { get { return (double)GetValue(ContentOpacityProperty); } set { SetValue(ContentOpacityProperty, value); } }
		void RaiseContentOpacityChanged(DependencyPropertyChangedEventArgs e) {
			if(!Checked) {
				Opacity = (double)e.NewValue;
			}
		}
		void RaiseCheckedChanged(DependencyPropertyChangedEventArgs e) {
			if((bool)e.NewValue) {
				Opacity = 1.0;
			}
		}
	}
}
