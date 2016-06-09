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

using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Validation.Native;
namespace DevExpress.Xpf.Editors.Native {
	public static class LookUpEditHelper {
		public static IPopupContentOwner GetPopupContentOwner(PopupBaseEdit baseEdit) {
			return baseEdit.PopupContentOwner;
		}
		public static void SetHighlightedText(TextEditSettings settings, string text, FilterCondition filterCondition) {
			settings.HighlightedText = text;
			settings.HighlightedTextCriteria = GetHighlightedTextCriteria(filterCondition);
		}
		internal static HighlightedTextCriteria GetHighlightedTextCriteria(FilterCondition filterCondition) {
			return filterCondition == FilterCondition.StartsWith ? HighlightedTextCriteria.StartsWith : HighlightedTextCriteria.Contains;
		}
		public static CriteriaOperator GetActualFilterCriteria(LookUpEditBase editor) {
			return editor.ItemsProvider.ActualFilterCriteria;
		}
		public static bool HasPopupContent(PopupBaseEdit baseEdit) {
			IPopupContentOwner owner = GetPopupContentOwner(baseEdit);
			return owner != null && owner.Child != null;
		}
		public static void RaisePopupContentSelectionChangedEvent(LookUpEditBase editor, IList removed, IList added) {
			editor.RaisePopupContentSelectionChanged(removed, added);
		}
		public static bool GetIsSingleSelection(LookUpEditBase editor) {
			return editor.Settings.StyleSettings == null || ((BaseLookUpStyleSettings)editor.Settings.StyleSettings).GetSelectionMode(editor) == SelectionMode.Single;
		}
		public static bool GetIsAllowItemHighlighting(LookUpEditBase editor) {
			var settings = editor.PropertyProvider.StyleSettings;
			bool selectOnMouseEnter = ((BaseLookUpStyleSettings)settings).GetSelectionEventMode(editor) == SelectionEventMode.MouseEnter;
			return selectOnMouseEnter && editor.AllowItemHighlighting;
		}
		public static VisualClientOwner GetVisualClient(PopupBaseEdit editor) {
			return editor.VisualClient;
		}
		public static void FocusEditCore(LookUpEditBase editor) {
			editor.FocusCore();
		}
		public static object GetSelectedItem(ISelectorEdit editor) {
			return editor.GetCurrentSelectedItem();
		}
		public static object GetEditValue(ISelectorEdit editor) {
			object value;
			if (editor.EditStrategy.ProvideEditValue(editor.EditStrategy.EditValue, out value, UpdateEditorSource.TextInput))
				return value;
			return null;
		}
		public static int GetSelectedIndex(LookUpEditBase editor) {
			return editor.EditStrategy.SelectorUpdater.GetIndexFromEditValue(editor.EditStrategy.EditValue);
		}
		public static IEnumerable GetSelectedItems(LookUpEditBase editor) {
			return ((ISelectorEdit)editor).GetCurrentSelectedItems();
		}
		public static IItemsProvider2 GetItemsProvider(LookUpEditBase editor) {
			return editor.ItemsProvider;
		}
		public static void UpdatePopupButtons(LookUpEditBase editor) {
			editor.UpdatePopupButtons();
		}
		public static IEnumerable<string> GetHighlightedColumns(LookUpEditBase editor) {
			var propertyProvider = (LookUpEditBasePropertyProvider)editor.PropertyProvider;
			return propertyProvider.GetHighlightedColumns();
		}
		public static bool GetIsAsyncServerMode(LookUpEditBase editor) {
			return editor.ItemsProvider.IsAsyncServerMode;
		}
		public static bool GetIsServerMode(LookUpEditBase editor) {
			return editor.ItemsProvider.IsServerMode;
		}
		public static bool IsInLookUpMode(LookUpEditSettingsBase settings) {
			return !string.IsNullOrEmpty(settings.ValueMember) || !string.IsNullOrEmpty(settings.DisplayMember);
		}
		public static bool GetClosePopupOnMouseUp(LookUpEditBase editor) {
			return ((BaseLookUpStyleSettings)editor.PropertyProvider.StyleSettings).GetClosePopupOnMouseUp(editor);
		}
	}
}
