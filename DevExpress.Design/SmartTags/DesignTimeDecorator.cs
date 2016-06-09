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
using DevExpress.Utils.Design;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections;
using System.Windows.Media;
using DevExpress.Design.UI;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
namespace DevExpress.Design.SmartTags {
	public abstract class DesignTimeDecorator<T> : Decorator where T : Control {
		static void ProcessEnter(KeyEventArgs e, Control control, DependencyProperty property, TextBox textBox) {
			if(e.Key == Key.Enter) {
				BindingExpression expression = control.GetBindingExpression(property);
				if(expression != null) {
					expression.UpdateSource();
					ClearUndo(textBox);
					e.Handled = true;
				}
			}
		}
		static void ClearUndo(TextBox textBox) {
			if(textBox == null)
				return;
			int limit = textBox.UndoLimit;
			textBox.UndoLimit = 0;
			textBox.UndoLimit = limit;
		}
		protected T Control { get { return Child as T; } }
		public override UIElement Child {
			set {
				if(Control != null) {
					UnsubscribeControlEvents();
				}
				base.Child = value;
				if(Control != null) {
					SubscribeControlEvents();
				}
			}
		}
		protected abstract DependencyProperty TextDependencyProperty { get; }
		protected abstract TextBox InnerTextBox { get; }
		protected virtual void SubscribeControlEvents() {
			Control.KeyDown += new KeyEventHandler(OnControlKeyDown);
		}
		protected virtual void UnsubscribeControlEvents() {
			Control.KeyDown -= new KeyEventHandler(OnControlKeyDown);
		}
		void OnControlKeyDown(object sender, KeyEventArgs e) {
			ProcessEnter(e, Control, TextDependencyProperty, InnerTextBox);
		}
	}
}
