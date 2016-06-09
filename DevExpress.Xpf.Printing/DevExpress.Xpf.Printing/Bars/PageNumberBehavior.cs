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
using System.Windows.Input;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Bars;
using System;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Printing {
	public class PageNumberBehavior : Behavior<BarEditItem> {
		public static readonly DependencyProperty FocusTargetProperty =
			DependencyProperty.Register("FocusTarget", typeof(UIElement), typeof(PageNumberBehavior), new PropertyMetadata(null));
		public UIElement FocusTarget {
			get { return (FrameworkElement)GetValue(FocusTargetProperty); }
			set { SetValue(FocusTargetProperty, value); }
		}
		protected override void OnAttached() {
			base.OnAttached();
			AssociatedObject.Links.CollectionChanged += Links_CollectionChanged;
			foreach(BarEditItemLink newLink in AssociatedObject.Links) {
				newLink.LinkControlLoaded += link_LinkControlLoaded;
			}
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			AssociatedObject.Links.CollectionChanged -= Links_CollectionChanged;
			foreach(BarEditItemLink link in AssociatedObject.Links) {
				link.LinkControlLoaded -= link_LinkControlLoaded;
				link.Editor.Do(x => {
					x.GotKeyboardFocus -= OnEditorGotFocus;
					x.KeyUp -= OnKeyUp;
					x.LostKeyboardFocus -= OnEditorLostFocus;
				});
			}
		}
		void Links_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add) {
				foreach(BarEditItemLink newLink in e.NewItems) {
					newLink.LinkControlLoaded -= link_LinkControlLoaded;
					newLink.LinkControlLoaded += link_LinkControlLoaded;
				}
			} else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove) {
				foreach(BarEditItemLink oldLink in e.OldItems) {
					oldLink.LinkControlLoaded -= link_LinkControlLoaded;
				}
			}
		}
		void link_LinkControlLoaded(object sender, BarItemLinkControlLoadedEventArgs e) {
			BarEditItemLink editorLink = sender as BarEditItemLink;
			if(editorLink == null)
				return;
			editorLink.Editor.GotKeyboardFocus += OnEditorGotFocus;
			editorLink.Editor.KeyUp += OnKeyUp;
			editorLink.Editor.LostKeyboardFocus += OnEditorLostFocus;
		}
		void OnEditorGotFocus(object sender, KeyboardFocusChangedEventArgs e) {
			var editor = sender as BaseEdit;
			editor.SelectAll();
		}
		void OnEditorLostFocus(object sender, KeyboardFocusChangedEventArgs e) {
			RevertEditValue(AssociatedObject);
		}
		void OnKeyUp(object sender, KeyEventArgs e) {
			if(e.Key == Key.Escape)
				FocusEditor();
			else if(e.Key == Key.Enter) {
				GetTextBindingExpression().UpdateSource();
				FocusEditor();
			}
		}
		void RevertEditValue(BarEditItem item) {
#if SL
			item.EditValue = RevertValue;
#else
			item.GetBindingExpression(BarEditItem.EditValueProperty).UpdateTarget();
#endif
		}
		void FocusEditor() {
			if(FocusTarget != null)
				FocusTarget.Focus();
		}
		BindingExpression GetTextBindingExpression() {
#if SL
			return AssociatedObject.GetBindingExpression(BarEditItem.EditValueProperty);
#else
			return BindingOperations.GetBindingExpression(AssociatedObject, BarEditItem.EditValueProperty);
#endif
		}
	}
}
