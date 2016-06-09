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
using System.Windows.Input;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Utils;
#if SL
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#endif
namespace DevExpress.Xpf.Printing.Native {
	public class GoToPageEditBehavior {
#if SL
		#region RevertValueProperty
		public static readonly DependencyProperty RevertValueProperty = DependencyPropertyManager.RegisterAttached("RevertValue", typeof(int), typeof(GoToPageEditBehavior), new PropertyMetadata(0));
		public static int GetRevertValue(DependencyObject obj) {
			return (int)obj.GetValue(RevertValueProperty);
		}
		public static void SetRevertValue(DependencyObject obj, int value) {
			obj.SetValue(RevertValueProperty, value);
		}
		#endregion
#endif
		public static readonly DependencyProperty ApplyProperty =
			DependencyPropertyManager.RegisterAttached("Apply", typeof(bool), typeof(GoToPageEditBehavior), new PropertyMetadata(false, OnApplyChanged));
		public static bool GetApply(DependencyObject obj) {
			return (bool)obj.GetValue(ApplyProperty);
		}
		public static void SetApply(DependencyObject obj, bool value) {
			obj.SetValue(ApplyProperty, value);
		}
		static void OnApplyChanged(object d, DependencyPropertyChangedEventArgs e) {
			BarEditItemLink link = d as BarEditItemLink;
			if(link == null)
				throw new NotSupportedException("GoToPageBehavior can be attached to a BarEditItemLink class instance only.");
			if((bool)e.NewValue == true) {
				link.LinkControlLoaded += link_LinkControlLoaded;
			}
		}
		static void link_LinkControlLoaded(object sender, BarItemLinkControlLoadedEventArgs e) {
			BarEditItem item = (BarEditItem)e.Item;
			BarEditItemLinkControl linkControl = (BarEditItemLinkControl)e.LinkControl;
			if(item == null || linkControl == null)
				return;
			var edit = linkControl.Edit;
#if !SL
			edit.GotMouseCapture += (o, args) => ((BaseEdit)o).SelectAll();			
			edit.LostKeyboardFocus += (o,args) => RevertEditValue(item);
#else
			edit.GotFocus += (o, args) => ((BaseEdit)o).SelectAll();
			edit.LostFocus += (o, args) => RevertEditValue(item);
#endif
			edit.KeyUp += (o, args) => OnKeyUp(args.Key, item);
		}
		static void OnKeyUp(Key key, BarEditItem item) {
			if(key == Key.Enter) {
				item.GetBindingExpression(BarEditItem.EditValueProperty).UpdateSource();
				UIElement element = GetReturnFocusTarget(item);
				if(element != null) {
					element.Focus();
				}
			}
		}
		static void RevertEditValue(BarEditItem item) {
#if SL
			item.EditValue = GetRevertValue(item);
#else
			item.GetBindingExpression(BarEditItem.EditValueProperty).UpdateTarget();
#endif
		}
		static UIElement GetReturnFocusTarget(BarItem item) {
			return DocumentViewer.GetDocumentViewer(BarManager.GetBarManager(item));
		}
	}
}
