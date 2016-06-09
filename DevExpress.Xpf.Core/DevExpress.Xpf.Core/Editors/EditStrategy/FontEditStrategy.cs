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
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Validation.Native;
namespace DevExpress.Xpf.Editors.EditStrategy {
	public class FontEditStrategy : ComboBoxEditStrategy {
		public FontEditStrategy(FontEdit editor)
			: base(editor) {
		}
		new FontEdit Editor { get { return base.Editor as FontEdit; } }
		protected override void RegisterUpdateCallbacks() {
			base.RegisterUpdateCallbacks();
			PropertyUpdater.Register(FontEdit.FontProperty, baseValue => GetEditValueFromFont(baseValue), baseValue => GetFontFromEditValue(baseValue));
		}
		object GetEditValueFromFont(object baseValue) {
			return baseValue;
		}
		object GetFontFromEditValue(object baseValue) {
			if (baseValue is IList && ((IList)baseValue).Count > 0)
				baseValue = ((IList)baseValue)[0];
			return GetFontFromValue(baseValue);
		}
		bool IsValueInItemsSource(object value) {
			return value != null && FontEditSettings.CachedFonts.Contains(GetFontFromValue(value));
		}
		FontFamily GetFontFromValue(object value) {
			if (value == null)
				return null;
			return value is FontFamily ? (FontFamily)value : new FontFamily(value.ToString());
		}
		public virtual void FontChanged(FontFamily oldValue, FontFamily newValue) {
			if (ShouldLockUpdate)
				return;
			SyncWithValue(FontEdit.FontProperty, oldValue, newValue);
		}
		public virtual FontFamily CoerceFont(FontFamily baseValue) {
			return (FontFamily)CoerceValue(FontEdit.FontProperty, baseValue);
		}
		public bool IsUserInput(UpdateEditorSource updateSource) {
			return updateSource == UpdateEditorSource.EnterKeyPressed || updateSource == UpdateEditorSource.LostFocus || updateSource == UpdateEditorSource.TextInput;
		}
		public bool ShouldConfirm(object value, UpdateEditorSource updateSource) {
			return Editor.AllowConfirmFontUseDialog && !IsValueInItemsSource(value) && IsUserInput(updateSource);
		}
		string GetConfirmationDialogMessage() {
			return EditorLocalizer.GetString(EditorStringId.ConfirmationDialogMessage);
		}
		string GetConfirmationDialogCaption() {
			return EditorLocalizer.GetString(EditorStringId.ConfirmationDialogCaption);
		}
		public bool Confirm(object newValue) {
			return MessageBox.Show(string.Format(GetConfirmationDialogMessage(), newValue), GetConfirmationDialogCaption(), MessageBoxButton.OKCancel
#if !SL
			, MessageBoxImage.Question
#endif
			) == MessageBoxResult.OK;
		}
		public override bool ProvideEditValue(object value, out object provideValue, UpdateEditorSource updateSource) {
			LookUpEditableItem item = value as LookUpEditableItem;
			if (!IsInLookUpMode)
				if (item != null) {
					if (ShouldConfirm(value, updateSource)) {
						provideValue = Confirm(item.DisplayValue) ? item.DisplayValue : item.EditValue;
						return true;
					}
					item.EditValue = item.DisplayValue;
				}
			return base.ProvideEditValue(value, out provideValue, updateSource);
		}
	}
}
