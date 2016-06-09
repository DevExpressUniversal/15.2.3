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
using System.Windows.Controls;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Docking.VisualElements {
	[DXToolboxBrowsable(false)]
	public class DockHintButton : psvControl {
		#region static
		public static readonly DependencyProperty IsHotProperty;
		public static readonly DependencyProperty IsAvailableProperty;
		static DockHintButton() {
			var dProp = new DependencyPropertyRegistrator<DockHintButton>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("IsHot", ref IsHotProperty, false,
				(dObj, ea) => ((DockHintButton)dObj).OnIsHotChanged((bool)ea.NewValue));
			dProp.Register("IsAvailable", ref IsAvailableProperty, true,
				(dObj, ea) => ((DockHintButton)dObj).OnIsAvailableChanged((bool)ea.NewValue));
		}
		#endregion
		public DockHintButton() {
			IsEnabledChanged += new DependencyPropertyChangedEventHandler(DockHintButton_IsEnabledChanged);
		}
		void DockHintButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			UpdateVisualState();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateVisualState();
		}
		protected virtual void OnIsHotChanged(bool hot) {
			UpdateVisualState();
		}
		protected virtual void OnIsAvailableChanged(bool available) {
			UpdateVisualState();
		}
		void UpdateVisualState() {
			if(IsEnabled) {
				if(IsAvailable) {
					VisualStateManager.GoToState(this, "Available", false);
					VisualStateManager.GoToState(this, IsHot ? "MouseOver" : "Normal", false);
				}
				else {
					VisualStateManager.GoToState(this, "Normal", false);
					VisualStateManager.GoToState(this, "Unavailable", false);
				}
			}
			else {
				VisualStateManager.GoToState(this, "Normal", false);
				VisualStateManager.GoToState(this, "Disabled", false);
			}
		}
		public bool IsHot {
			get { return (bool)GetValue(IsHotProperty); }
			set { SetValue(IsHotProperty, value); }
		}
		public bool IsAvailable {
			get { return (bool)GetValue(IsAvailableProperty); }
			set { SetValue(IsAvailableProperty, value); }
		}
	}
}
