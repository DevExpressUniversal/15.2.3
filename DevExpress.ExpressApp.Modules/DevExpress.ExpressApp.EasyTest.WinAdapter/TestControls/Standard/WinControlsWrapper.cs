#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text;
using System.Windows.Forms;
using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WinAdapter;
using System.Runtime.InteropServices;
using System.Threading;
using DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls;
namespace DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.Standard {
	public class WinControlEnabled<T> : TestControlInterfaceImplementerBase<T>, IControlEnabled where T: Control{
		public WinControlEnabled(T control) : base(control) { }
		#region IControlEnabled Members
		public virtual bool Enabled {
			get {
				return control.Enabled;
			}
		}
		#endregion
	}
	public class WinControlEnabled : WinControlEnabled<Control> {
		public WinControlEnabled(Control control)
			: base(control) {
		}
	}
	public class WinTextEditableControl : TestControlInterfaceImplementerBase<Control>, IControlReadOnlyText {
		public WinTextEditableControl(Control control) : base(control) { }
		#region IControlReadOnlyText Members
		public string Text {
			get { return control.Text; }
		}
		#endregion
	}
	public class TextBoxBaseWinControl : TestControlTextValidated<TextBoxBase> {
		public TextBoxBaseWinControl(TextBoxBase control)
			: base(control) { }
		protected override string Validate(string text) {
			string result = ValidateMaxLength(control.MaxLength, text);
			if(string.IsNullOrEmpty(result)) {
				return base.Validate(text);
			}
			return result;
		}
		protected override void InternalSetText(string text) {
			control.Text = text;
			control.Modified = true;
		}
	}
	public class ButtonControl : TestControlInterfaceImplementerBase<Button>, IControlAct {
		public ButtonControl(Button control) : base(control) { }
		#region IControlAct Members
		public void Act(string value) {
			control.PerformClick();
		}
		#endregion
	}
	public class TabPageControl : TestControlInterfaceImplementerBase<TabPage>, IControlEnabled, IControlAct {
		public TabPageControl(TabPage control) : base(control) { }
		#region IControlEnabled Members
		public bool Enabled {
			get { return ((TabControl)(control.Parent)).Enabled; }
		}
		#endregion
		#region IControlAct Members
		public void Act(string value) {
			((TabControl)(control.Parent)).SelectedTab = control;
		}
		#endregion
	}
}
