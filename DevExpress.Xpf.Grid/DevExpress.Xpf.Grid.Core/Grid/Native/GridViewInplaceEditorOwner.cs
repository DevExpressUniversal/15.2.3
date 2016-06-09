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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
namespace DevExpress.Xpf.Grid.Native {
	public class GridViewInplaceEditorOwner : InplaceEditorOwnerBase {
		DataViewBase View { get { return (DataViewBase)owner; } }
		public GridViewInplaceEditorOwner(DataViewBase view) : base(view, false) {
		}
		protected override bool IsNavigationKey(Key key) {
			if(key == Key.Enter && (View.EnterMoveNextColumn || View.IsNewItemRowFocused))
				return true;
			return base.IsNavigationKey(key);
		}
		public override bool EditorWasClosed {
			get { return View.IsRootView ? base.EditorWasClosed : View.RootView.InplaceEditorOwner.EditorWasClosed; }
			set {
				if(View.IsRootView)
					base.EditorWasClosed = value;
				else {
					View.RootView.InplaceEditorOwner.EditorWasClosed = value;
				}
			}
		}
		public override FrameworkElement TopOwner { get { return View.RootView; } }
		protected override FrameworkElement FocusOwner { get { return View.RootView.DataControl; } }
		protected override Type OwnerBaseType { get { return typeof(DataViewBase); } }
		protected override EditorShowMode EditorShowMode { get { return View.RootView.EditorShowMode; } }
		protected override bool EditorSetInactiveAfterClick { get { return View.EditorSetInactiveAfterClick; } }
		protected override bool UseMouseUpFocusedEditorShowModeStrategy { get { return View.UseMouseUpFocusedEditorShowModeStrategy; } }
		protected override bool CanCommitEditing { get { return base.CanCommitEditing || View.DataProviderBase.IsCurrentRowEditing; } }
		protected override bool CanFocusEditor { 
			get {
				if(FloatingContainer.IsModalContainerOpened) return false;
				return base.CanFocusEditor && !View.IsColumnFilterOpened && View.IsFocusedView && !View.IsKeyboardFocusInSearchPanel() && (!View.IsKeyboardFocusInHeadersPanel() || View.Navigation.NavigationMouseLocker.IsLocked);
			} 
		}
		protected override void EnqueueImmediateAction(IAction action) {
			View.ImmediateActionsManager.EnqueueAction(action);
		}
		public override void ProcessKeyDown(KeyEventArgs e) {
			View.ProcessKeyDown(e);
		}
		protected override string GetDisplayText(InplaceEditorBase inplaceEditor, string originalDisplayText, object value) {
			return string.Empty;
		}
		protected override bool? GetDisplayText(InplaceEditorBase inplaceEditor, string originalDisplayText, object value, out string displayText) {
			displayText = null;
			if(View.DataControl == null)
				return false;
			CellEditorBase cellEditor = (CellEditorBase)inplaceEditor;
			if(cellEditor.CellData == null)
				return false;
			return View.RaiseCustomDisplayText(cellEditor.RowHandle, null, cellEditor.CellData.ColumnCore, value, originalDisplayText, out displayText);
		}
		protected override bool PerformNavigationOnLeftButtonDown(MouseButtonEventArgs e) {
			DependencyObject originalSource = e.OriginalSource as DependencyObject;
			var rowElement = View.GetRowElementByTreeElement(originalSource);
			object rowIdentifier = GetRowIdentifierByTreeElement(View, rowElement);
			View.PerformNavigationOnLeftButtonDownCore(e);
			View.UpdateRowsState();
			return object.Equals(rowIdentifier, GetRowIdentifierByTreeElement(View, rowElement));
		}
		object GetRowIdentifierByTreeElement(DataViewBase view, DependencyObject originalSource) {
			int rowHandle = view.GetRowHandleByTreeElement(originalSource);
			return view.DataProviderBase.GetNodeIdentifier(rowHandle);
		}
		protected override bool CommitEditing() {
			return View.CommitEditing();
		}
		protected override bool CommitEditorForce() {
			return View.CommitEditing(true, true);
		}
		protected override void OnActiveEditorChanged() {
			View.SetActiveEditor();
		}
		protected override bool IsChildOfCurrentEditor(DependencyObject source) {
			if(!Optimized) {
				return IsCurrentCellChild(source);
			}
			if(View.ActualAllowCellMerge) {
				return View.RootView.CalcHitInfoCore(source).IsRowCell;
			}
			var cellData = GetCellData(source);
			if(cellData == null || cellData.Column.DisplayTemplate == null) {
				return IsCurrentCellChild(source);
			}
			int row = cellData.RowData.RowHandle.Value;
			var col = cellData.Column;
			return View.FocusedRowHandle == row && View.DataControl.CurrentColumn == col;
		}
		bool Optimized {
			get {
				var view = View as ITableView;
				return view != null && (view.UseLightweightTemplates == null || view.UseLightweightTemplates.Value != UseLightweightTemplates.None);
			}
		}
		EditGridCellData GetCellData(object o) {
			object context = null;
			if(o is FrameworkElement) {
				context = (o as FrameworkElement).DataContext;
			}
			if(o is FrameworkContentElement) {
				context = (o as FrameworkContentElement).DataContext;
			}
			return context as EditGridCellData;
		}
		bool IsCurrentCellChild(object source) {
			return View.CurrentCell != null && LayoutHelper.IsChildElement(View.CurrentCell, source as DependencyObject);
		}
		protected override bool ShowEditor(bool selectAll) {
			View.OnOpeningEditor();
			return base.ShowEditor(selectAll);
		}
	}
}
