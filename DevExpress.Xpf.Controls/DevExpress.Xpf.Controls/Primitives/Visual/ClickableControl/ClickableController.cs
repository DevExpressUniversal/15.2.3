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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.WindowsUI.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Xpf.Controls.Primitives {
	public class ClickableController : ControlControllerBase {
		IClickableControl IClickableControl { get { return Control as IClickableControl; } }
		public ClickableController(IControl control)
			: base(control) {
		}
		public event EventHandler Click;
		protected virtual void OnClick() {
			if(Click != null)
				Click(Control, new RoutedEventArgs());
			if(IClickableControl != null) IClickableControl.OnClick();
		}
		public void InvokeClick() {
			OnClick();
		}
		protected virtual bool FocusOnMouseDown { get { return false; } }
		#region Keyboard and Mouse Handling
		protected override void OnMouseLeftButtonDown(DXMouseButtonEventArgs e) {
			if(FocusOnMouseDown) Control.Focus();
			if(ClickOnMouseDown) {
				OnClick();
				e.Handled = true;
			}
			if(CaptureMouseOnDown)
				Control.CaptureMouse();
			base.OnMouseLeftButtonDown(e);
		}
		protected override void OnMouseLeftButtonUp(DXMouseButtonEventArgs e) {
			if(e != null && e.Handled) return;
			var isClick = IsMouseLeftButtonDown && IsMouseEntered && !ClickOnMouseDown;
			base.OnMouseLeftButtonUp(e);
			if(isClick && Control.IsMouseOver) {
				OnClick();
				if(e != null) e.Handled = true;
			}
		}
		protected bool ClickOnMouseDown { get; set; }
		#endregion Keyboard and Mouse Handling
	}
}
