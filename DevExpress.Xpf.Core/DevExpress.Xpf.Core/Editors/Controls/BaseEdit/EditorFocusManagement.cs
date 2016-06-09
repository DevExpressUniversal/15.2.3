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
using System.Windows.Markup;
using System.IO;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Documents;
using DevExpress.Utils;
using System.Collections;
using System.ComponentModel;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Printing;
using System.Windows.Media;
using DevExpress.Xpf.Printing.Native;
#if !SL
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Core;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Collections.Specialized;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using IInputElement = System.Windows.UIElement;
using Keyboard = DevExpress.Xpf.Editors.WPFCompatibility.SLKeyboard;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Editors {
	class EditorFocusManagement {
		BaseEdit Editor { get; set; }
		public bool IsFocusWithin { get; private set; }
		public EditorFocusManagement(BaseEdit editor) {
			Editor = editor;
			Keyboard.AddGotKeyboardFocusHandler(Editor, GotFocus);
			Keyboard.AddLostKeyboardFocusHandler(Editor, LostFocus);
			Keyboard.AddPreviewLostKeyboardFocusHandler(Editor, PreviewLostFocus);
#if !SL
			Editor.Loaded += Editor_Loaded;
			SetFocusAction = new PostponedAction(() => !Editor.IsLoaded);
#endif
		}
#if !SL
		PostponedAction SetFocusAction { get; set; }
		void Editor_Loaded(object sender, RoutedEventArgs e) {
			SetFocusAction.Perform();
		}
#endif
		void GotFocus(object sender, KeyboardFocusChangedEventArgs args) {
			if(LayoutHelper.IsChildElement(Editor, args.NewFocus as DependencyObject)) {
				if(IsFocusWithin)
					return;
				IsFocusWithin = true;
				Editor.EditStrategy.OnGotFocus();
				Editor.EditStrategy.AfterOnGotFocus();
			}
		}
		void LostFocus(object sender, KeyboardFocusChangedEventArgs args) {
#if !SL
			if (Editor.EditMode == EditMode.Standalone && !Editor.IsLoaded && !Editor.IsChildElement(args.NewFocus as DependencyObject)) {
				SetFocusAction.PerformPostpone(() => Editor.Focus());
				return;
			}
#endif
			if (args.NewFocus != null && !Editor.IsChildElement(args.NewFocus as DependencyObject)) {
				IsFocusWithin = false;
				Editor.EditStrategy.OnLostFocus();
			}
		}
		void PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs args) {
			args.Handled = Editor.HandlePreviewLostKeyboardFocus((DependencyObject)args.OldFocus, (DependencyObject)args.NewFocus);
		}
	}
}
