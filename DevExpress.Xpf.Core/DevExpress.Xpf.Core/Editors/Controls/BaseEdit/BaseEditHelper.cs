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
namespace DevExpress.Xpf.Editors.Helpers {
	public static partial class BaseEditHelper {
#if !SL
		public static DependencyPropertyKey GetValidationErrorPropertyKey() {
			return BaseEdit.ValidationErrorPropertyKey;
		}
#endif
		public static IBaseEdit GetBaseEdit(IBaseEdit edit) {
#if !SL
			var inplace = edit as IInplaceBaseEdit;
			if (inplace != null)
				return inplace.BaseEdit;
#endif
			return edit;
		}
		public static object GetEditValue(IBaseEdit edit) {
			if (edit is BaseEdit)
				return ((BaseEdit)edit).PropertyProvider.EditValue;
			return edit.EditValue;
		}
		public static bool GetIsActivatingKey(IBaseEdit edit, KeyEventArgs e) {
			return edit.IsActivatingKey(GetKey(e), ModifierKeysHelper.GetKeyboardModifiers(e));
		}
		public static void ProcessActivatingKey(IBaseEdit edit, KeyEventArgs e) {
			edit.ProcessActivatingKey(GetKey(e), ModifierKeysHelper.GetKeyboardModifiers(e));
		}
		public static bool GetNeedsKey(IBaseEdit edit, KeyEventArgs e) {
			return edit.NeedsKey(GetKey(e), ModifierKeysHelper.GetKeyboardModifiers(e));
		}
		public static bool GetShowEditorButtons(IBaseEdit edit) {
			return edit.GetShowEditorButtons();
		}
		public static void SetShowEditorButtons(IBaseEdit edit, bool show) {
			edit.SetShowEditorButtons(show);
		}
		public static Key GetKey(KeyEventArgs e) {
#if !SL
			return e.Key == Key.System ? e.SystemKey : e.Key;
#else
			return e.Key;
#endif
		}
		public static bool GetIsChildElement(IBaseEdit edit, IInputElement element) {
			return edit.IsChildElement(element);
		}
		public static BaseEditSettings GetEditSettings(IBaseEdit edit) {
			return edit.Settings;
		}
		public static bool GetIsValueChanged(IBaseEdit edit) {
			return edit.IsValueChanged;
		}
		public static bool SetIsValueChanged(IBaseEdit edit, bool value) {
			return edit.IsValueChanged = value;
		}
		public static void FlushPendingEditActions(IBaseEdit edit) {
			edit.FlushPendingEditActions();
		}
		public static BaseValidationError GetValidationError(DependencyObject d) {
			var baseEdit = d as IBaseEdit;
			if (baseEdit != null)
				return baseEdit.ValidationError;
			return BaseEdit.GetValidationError(d);
		}
		public static void SetValidationError(DependencyObject d, BaseValidationError error) {
			var baseEdit = d as IBaseEdit;
			if (baseEdit != null)
				baseEdit.ValidationError = error;
			else
				BaseEdit.SetValidationError(d, error);				
		}
		public static void SetValidationErrorTemplate(DependencyObject d, DataTemplate template) {
			var baseEdit = d as IBaseEdit;
			if (baseEdit != null)
				baseEdit.ValidationErrorTemplate = template;
			else
				BaseEdit.SetValidationErrorTemplate(d, template);
		}
		public static void SetDisplayTextProvider(IBaseEdit edit, IDisplayTextProvider displayTextProvider) {
#if !SL
			InplaceBaseEdit inplaceBaseEdit = edit as InplaceBaseEdit;
			if(inplaceBaseEdit != null)
				inplaceBaseEdit.SetDisplayTextProvider(displayTextProvider);
#endif
			BaseEdit newEdit = edit as BaseEdit;
			if(newEdit != null)
				newEdit.SetDisplayTextProvider(displayTextProvider);
		}
		public static bool GetRequireDisplayTextSorting(BaseEditSettings editSettings) {
			return editSettings != null && editSettings.RequireDisplayTextSorting;
		}
		public static void AssignViewInfoProperties(BaseEdit edit, BaseEditSettings settings, IDefaultEditorViewInfo defaultViewInfo) {
			settings.AssignViewInfoProperties(edit, defaultViewInfo);
		}
		public static void UpdateHighlightingText(IBaseEdit edit, TextHighlightingProperties highlightingProperties) {
			TextEdit textEdit = edit as TextEdit;
			if(textEdit == null)
				return;
			if(highlightingProperties == null)
				highlightingProperties = DevExpress.Xpf.Editors.Native.SearchControlHelper.GetDefaultTextHighlightingProperties();
			textEdit.HighlightedText = highlightingProperties.Text ?? String.Empty;
			textEdit.HighlightedTextCriteria = DevExpress.Xpf.Editors.Native.LookUpEditHelper.GetHighlightedTextCriteria(highlightingProperties.FilterCondition);
		}
		public static void SetCurrentValue(BaseEdit edit, DependencyProperty property, object value) {
#if !SL
			edit.SetCurrentValue(property, value);
#else
			edit.SetValue(property, value);
#endif
		}
#if DEBUGTEST
		public static int GetMeasureCount(IBaseEdit edit) {
#if !SL
			if(edit is InplaceBaseEdit)
				return ((InplaceBaseEdit)edit).MeasureCount;
#endif
			return ((BaseEdit)edit).MeasureCount;
		}
#endif
		public static void ToggleCheckEdit(CheckEdit edit) {
			edit.OnToggle();
		}
		public static void RaiseDefaultButtonClick(ButtonEditSettings settings) {
			settings.RaiseDefaultButtonClick(settings, new RoutedEventArgs());
		}
		public static void ApplySettings(IBaseEdit edit, BaseEditSettings settings, IDefaultEditorViewInfo editorColumn) {
#if !SL
			if (edit is IInplaceBaseEdit && settings != null)
				settings.ApplyToEdit(edit, true, editorColumn);
#endif
		}
		public static void ResetEditorCache(IBaseEdit edit) {
			BaseEdit baseEdit = edit as BaseEdit;
			if(baseEdit != null)
				baseEdit.EditStrategy.PropertyUpdater.ResetSyncValue();
		}
		public static void SetEditValue(IBaseEdit edit, object editValue) {
			if(editValue == null && edit.EditValue == null) ResetEditorCache(edit);
			edit.EditValue = editValue;
		}
		public static void SetTextDecorations(DependencyObject d, TextDecorationCollection textDecorations) {
			var inplaceBaseEdit = d as IInplaceBaseEdit;
			if(inplaceBaseEdit != null)
				inplaceBaseEdit.TextDecorations = textDecorations;
			var textEdit = d as TextEdit;
			if(textEdit != null)
				textEdit.TextDecorations = textDecorations;
		}
	}
}
