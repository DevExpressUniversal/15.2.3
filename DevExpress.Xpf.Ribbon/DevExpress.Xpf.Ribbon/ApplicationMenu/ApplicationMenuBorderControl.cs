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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils.Themes;
using System.Windows;
using DevExpress.Xpf.Core.Native;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
namespace DevExpress.Xpf.Ribbon {
	public class ApplicationMenuPopupBorderControl : PopupBorderControl {
		public ApplicationMenuPopupBorderControl(ApplicationMenu owner) {
			DefaultStyleKey = typeof(ApplicationMenuPopupBorderControl);
			ApplicationMenu = owner;
			SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
		}
		public ApplicationMenuPopupBorderControl() {
			DefaultStyleKey = typeof(ApplicationMenuPopupBorderControl);		
			SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
		}
		public void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			if(ApplicationMenu != null)
				ApplicationMenu.UpdateVerticalOffset();
		}
		public RibbonApplicationButtonControl ApplicationButton {
			get { return applicationButton; }
			protected internal set {
				if(applicationButton == value) return;
				var oldValue = applicationButton;
				applicationButton = value;
				OnApplicationButtonChanged(oldValue);
			}
		}
		private void OnApplicationButtonChanged(RibbonApplicationButtonControl oldValue) {
			if(oldValue != null)
				oldValue.Click -= OnApplicationButtonClick;
			if(ApplicationButton != null) {
				ApplicationButton.Click += new EventHandler(OnApplicationButtonClick);
				if(ApplicationMenu != null && ApplicationMenu.Ribbon != null) {
					ApplicationButton.Ribbon = ApplicationMenu.Ribbon;
					ApplicationButton.Ribbon.SetCurrentValue(RibbonControl.RibbonStyleProperty, ApplicationMenu.RibbonStyle);
				} else {
					ApplicationButton.RibbonStyle = RibbonStyle.Office2010;
				}
				ApplicationButton.UpdateLayout();
			}
		}
		public ApplicationMenu ApplicationMenu { get; protected internal set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ApplicationButton = GetTemplateChild("PART_ApplicationButton") as RibbonApplicationButtonControl;
		}
		public void UpdateAppButtonPosition(FrameworkElement target) {
			if(target == null || ApplicationButton == null)
				return;
			TranslateTransform transform = new TranslateTransform();
			UpdateLayout();
			int flDirectCoef = 1;
			if(FlowDirection != FlowDirection.LeftToRight)
				flDirectCoef = -1;
			Point targetOffset = ScreenHelper.GetScreenPoint(target);
			Point childOffset = ScreenHelper.GetScreenPoint(this);
			transform.X = (targetOffset.X - childOffset.X) * flDirectCoef;
			transform.Y = targetOffset.Y - childOffset.Y;
			ApplicationButton.RenderTransform = transform;
		}
		public void UpdateAppButtonProperties(bool isPressed) {
			if(ApplicationMenu == null || ApplicationButton == null)
				return;
			ApplicationButton.IsChecked = isPressed;
			ApplicationButton.Ribbon = ApplicationMenu.Ribbon;
			ApplicationButton.RibbonStyle = ApplicationMenu.RibbonStyle;
			ApplicationButton.ApplyTemplate();
		}
		void OnApplicationButtonClick(object sender, EventArgs e) {
			if(ApplicationMenu != null)
				ApplicationMenu.IsOpen = false;
		}
		RibbonApplicationButtonControl applicationButton;
	}
}
