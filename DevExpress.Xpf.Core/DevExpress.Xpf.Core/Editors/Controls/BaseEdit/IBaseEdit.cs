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
using DevExpress.Xpf.Editors.Internal;
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
#if !SL
	public interface IBaseEdit : IInputElement {
#else
	public partial interface IBaseEdit: System.Windows.IInputElement {
#endif
		bool ShouldDisableExcessiveUpdatesInInplaceInactiveMode { get; set; }
		bool? DisableExcessiveUpdatesInInplaceInactiveMode { get; set; }
		string DisplayText { get; }
		EditMode EditMode { get; set; }
		object EditValue { get; set; }
		bool IsReadOnly { get; set; }
		bool IsEditorActive { get; }
		event EditValueChangedEventHandler EditValueChanged;
		event RoutedEventHandler EditorActivated;
		event RoutedEventHandler Loaded;
		event RoutedEventHandler Unloaded;
		object DataContext { get; set; }
		ControlTemplate DisplayTemplate { get; set; }
		ControlTemplate EditTemplate { get; set; }
		HorizontalAlignment HorizontalContentAlignment { get; set; }
		VerticalAlignment VerticalContentAlignment { get; set; }
		InvalidValueBehavior InvalidValueBehavior { get; set; }
		IValueConverter DisplayTextConverter { get; set; }
		string DisplayFormatString { get; set; }
		BaseEditSettings Settings { get; }
		bool CanAcceptFocus { get; set; }
		double MaxWidth { get; set; }
		bool HasValidationError { get; }
		bool ShowEditorButtons { get; set; }
		BaseValidationError ValidationError { get; set; }
		DataTemplate ValidationErrorTemplate { get; set; }
		void SetSettings(BaseEditSettings settings);
		void BeginInit();
		void EndInit();
		void BeginInit(bool callBase);
		void EndInit(bool callBase, bool shouldSync = true);
		void ForceInitialize(bool callBase);
		bool IsPrintingMode { get; set; }
		bool IsValueChanged { get; set; }
		object GetValue(DependencyProperty d);
		BindingExpressionBase SetBinding(DependencyProperty dp, BindingBase binding);
		void ClearValue(DependencyProperty dp);
		bool DoValidate();
		void SelectAll();
		bool IsChildElement(IInputElement element, DependencyObject root = null);
		bool GetShowEditorButtons();
		void SetShowEditorButtons(bool show);
		bool NeedsKey(Key key, ModifierKeys modifiers);
		bool IsActivatingKey(Key key, ModifierKeys modifiers);
		void ProcessActivatingKey(Key key, ModifierKeys modifiers);
		void FlushPendingEditActions();
		string GetDisplayText(object editValue, bool applyFormatting);
		FrameworkElement EditCore { get; }
		string NullText { get; set; }
		object NullValue { get; set; }
		void ClearError();
		void SetInplaceEditingProvider(IInplaceEditingProvider provider);
#if !SL
		new void RaiseEvent(RoutedEventArgs e);
#else
		void RaiseEvent(RoutedEventArgs e);
#endif
	}
#if !SL
	public interface IInplaceBaseEdit : IBaseEdit, IChrome {
		IBaseEdit BaseEdit { get; }
		void RaiseEditValueChanged(object oldValue, object newValue);
		bool ShowBorder { get; set; }
		bool IsNullTextVisible { get; }
		bool HasTextDecorations { get; set; }
		TextDecorationCollection TextDecorations { get; set; }
		TextTrimming TextTrimming { get; set; }
		TextWrapping TextWrapping { get; set; }
		HighlightedTextCriteria HighlightedTextCriteria { get; set; }
		string HighlightedText { get; set; }
		IEnumerable<ButtonInfoBase> GetSortedButtons();
		bool AllowDefaultButton { get; set; }
		RenderTemplate BorderTemplate { get; set; }
		Style ActiveEditorStyle { get; set; }
		bool ShowToolTipForTrimmedText { get; set; }
	}
#endif
}
