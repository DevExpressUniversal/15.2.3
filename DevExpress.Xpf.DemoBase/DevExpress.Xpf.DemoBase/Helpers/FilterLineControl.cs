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
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Editors;
namespace DevExpress.Xpf.DemoBase.Helpers {
	public class FilterLineControl : Control {
		public event EventHandler ClearFocus;
		TextEdit filterEdit;
		public string FilterText {
			get { return (string)GetValue(FilterTextProperty); }
			set { SetValue(FilterTextProperty, value); }
		}
		public static readonly DependencyProperty FilterTextProperty =
			DependencyProperty.Register("FilterText", typeof(string), typeof(FilterLineControl), new PropertyMetadata(string.Empty));
		public override void OnApplyTemplate() {
			filterEdit = (TextEdit) GetTemplateChild("filterEdit");
			filterEdit.MouseUp += FilterLineControl_MouseUp;
			filterEdit.KeyUp += FilterLineControl_KeyUp;
			base.OnApplyTemplate();
		}
		void FilterLineControl_KeyUp(object sender, KeyEventArgs e) {
			if(e.Key == Key.Escape) {
				FilterText = "";
				ClearFocus.Do(cf => cf(this, null));
			}
		}
		void FilterLineControl_MouseUp(object sender, MouseButtonEventArgs e) {
			e.Handled = true;
		}
		static FilterLineControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(FilterLineControl), new FrameworkPropertyMetadata(typeof(FilterLineControl)));
		}
		public void FocusEditor() {
			filterEdit.Focus();
		}
	}
}
