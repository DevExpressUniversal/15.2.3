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

using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Data;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Core;
#if !SL
#else
#endif
namespace DevExpress.Xpf.Editors.Popups {
	[ToolboxItem(false)]
	public partial class EditorPopupBase : PopupBase {
#if DEBUGTEST
		internal Point? MousePointerPosition = null;
#endif
#if !SL
		Point GetMousePointerPosition(MouseEventArgs e, IInputElement relativeTo) {
#if DEBUGTEST
			if(MousePointerPosition.HasValue)
				return MousePointerPosition.Value;
#endif
			return e.GetPosition(relativeTo);
		}
#endif
		internal PopupBorderControl PopupBorderControl { get { return (PopupBorderControl)Child; } }
		public EditorPopupBase() {
			AllowsTransparency = true;
		}
		protected PopupBaseEdit OwnerEdit {
			get { return BaseEdit.GetOwnerEdit(this) as PopupBaseEdit; }
		}
		internal DependencyObject GetTemplateChildCore(string childName) {
			return GetTemplateChild(childName);
		}
#if !SL
		protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnMouseDown(e);
#else
		protected override void OnPopupMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnPopupMouseLeftButtonDown(e);
#endif
			if(Child == null)
				return;
			Rect rect = new Rect(0, 0, ((FrameworkElement)Child).ActualWidth, ((FrameworkElement)Child).ActualHeight);
			if(!rect.Contains(GetMousePointerPosition(e, Child)))
				OwnerEdit.ClosePopupOnClick();
		}
#if !SL
		protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsKeyboardFocusWithinChanged(e);
			if(OwnerEdit != null)
				OwnerEdit.OnPopupIsKeyboardFocusWithinChanged(this);
		}
#endif
		protected override PopupBorderControl CreateBorderControl() {
			return new PopupBorderControl() { Focusable = false };
		}
	}
}
