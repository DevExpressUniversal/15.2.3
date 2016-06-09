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
using System.Text.RegularExpressions;
namespace DevExpress.Xpf.Core {
	public class DXButton : ContentControlBase {
		public new DXButtonController Controller { get { return (DXButtonController)base.Controller; } }
		public event RoutedEventHandler Click {
			add { Controller.Click += value; }
			remove { Controller.Click -= value; }
		}
		protected override ControlControllerBase CreateController() {
			return new DXButtonController(this);
		}
	}
	public class DXButtonController : ControlControllerBase {
		public DXButtonController(IControl control)
			: base(control) {
			CaptureMouseOnDown = true;
		}
		public event RoutedEventHandler Click;
		protected virtual void OnClick() {
			if (Click != null)
				Click(Control, new RoutedEventArgs());
		}
		#region Keyboard and Mouse Handling
		protected override void OnMouseLeftButtonDown(DXMouseButtonEventArgs e) {
			if (ClickOnMouseDown) {
				OnClick();
				e.Handled = true;
			}
			base.OnMouseLeftButtonDown(e);
		}
		protected override void OnMouseLeftButtonUp(DXMouseButtonEventArgs e) {
			var isClick = IsMouseLeftButtonDown && IsMouseEntered && !ClickOnMouseDown;
			base.OnMouseLeftButtonUp(e);
			if (isClick) {
				OnClick();
				e.Handled = true;
			}
		}
		protected bool ClickOnMouseDown { get; set; }
		#endregion Keyboard and Mouse Handling
	}
#if SILVERLIGHT
	[DXToolboxBrowsable(false)]
	public class ToolButton : Button {
		public ToolButton() {
			DefaultStyleKey = typeof(ToolButton);
		}
	}
	public class HyperlinkNavigator : System.Windows.Controls.HyperlinkButton {
		const string ShortUrlPattern = @"^[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(/\S*)?$";
		const string UrlPrefix = @"http://";
		public void Navigate(string to, string target) {
			try {
				NavigateCore(to, target);
			}
			catch {
				Regex regex = new Regex(ShortUrlPattern);
				if (regex.IsMatch(to)) {
					try {
						NavigateCore(UrlPrefix + to, target);
					}
					catch {
					}
				}
			}
		}
		protected virtual void NavigateCore(string to, string target) {
			this.NavigateUri = new Uri(to);
			this.TargetName = target;
			OnClick();
		}
		public void Navigate(string to) {
			Navigate(to, String.Empty);
		}
	}
#endif
}
