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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.PropertyGrid {
	public class CellEditorOwner : InplaceEditorOwnerBase {
		public CellEditorOwner(PropertyGridView owner)
			: base(owner) {
		}
		public PropertyGridView View { get { return (PropertyGridView)owner; } }
		internal bool InternalCanCommitEditing { get { return CanCommitEditing; } }
		internal bool CanHideEditor { get { return CurrentCellEditor != null && CurrentCellEditor.IsEditorVisible; } }
		protected override bool EditorSetInactiveAfterClick {
			get { throw new NotImplementedException(); }
		}
		protected override EditorShowMode EditorShowMode {
			get { return EditorShowMode.MouseDown; }
		}
		protected override FrameworkElement FocusOwner {
			get { return View.With(x => x.PropertyGrid) ?? (FrameworkElement)View; }
		}
		protected override Type OwnerBaseType {
			get { return typeof(PropertyGridView); }
		}
		protected override bool CommitEditing() {
			return View.CommitEditing();
		}
		protected override void EnqueueImmediateAction(IAction action) {
			View.ImmediateActionsManager.EnqueueAction(action);
		}
		protected override string GetDisplayText(InplaceEditorBase inplaceEditor, string originalDisplayText, object value) {
			var cellEditor = inplaceEditor as CellEditor;
			TrimDisplayTextIfNeeded(cellEditor, ref originalDisplayText);
			if (cellEditor != null && cellEditor.RowData != null && View.PropertyGrid != null)
				return View.PropertyGrid.RaiseCustomDisplayText(cellEditor.RowData, originalDisplayText);
			return originalDisplayText;
		}
		protected override bool? GetDisplayText(InplaceEditorBase inplaceEditor, string originalDisplayText, object value, out string displayText) {
			displayText = GetDisplayText(inplaceEditor, originalDisplayText, value);
			return true;
		}
		void TrimDisplayTextIfNeeded(CellEditor cellEditor, ref string originalDisplayText) {
			var value = cellEditor.With(x => x.RowData).With(x => x.Value);
			if (value == null || !View.With(x => x.PropertyGrid).If(x => x.TrimDisplayText).ReturnSuccess())
				return;
			var valueType = value.GetType();
			if (originalDisplayText.If(x => x.StartsWith(valueType.FullName)).ReturnSuccess())
				originalDisplayText = originalDisplayText.Replace(valueType.FullName, valueType.Name);
		}
		protected override bool PerformNavigationOnLeftButtonDown(MouseButtonEventArgs e) {
			return View.PerformNavigationOnLeftButtonDown(e.OriginalSource as DependencyObject);
		}
		public override void ProcessKeyDown(KeyEventArgs e) {
			if (IsNavigationKey(e.Key) && ActiveEditor != null && ActiveEditor.NeedsKey(e.Key, ModifierKeysHelper.GetKeyboardModifiers(e)))
				return;
			View.ProcessKeyDown(e);
		}
		public override bool IsActiveEditorHaveValidationError() {
			if (ActiveEditor == null)
				return false;
			IBaseEdit activeEdit = ActiveEditor as IBaseEdit;
			if (activeEdit == null)
				return false;
			return activeEdit.HasValidationError;
		}
	}
}
