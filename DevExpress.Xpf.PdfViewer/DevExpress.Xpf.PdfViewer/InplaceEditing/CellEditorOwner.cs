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
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.PdfViewer {
	public class CellEditorOwner : InplaceEditorOwnerBase {
		PdfPresenterControl Presenter { get { return (PdfPresenterControl)owner; } }
		PdfDocumentViewModel Document { get { return (PdfDocumentViewModel)Presenter.Document; } }
		public CellEditorOwner(FrameworkElement owner)
			: base(owner, false) {
		}
		protected override void EnqueueImmediateAction(IAction action) {
			Presenter.ImmediateActionsManager.EnqueueAction(action);
		}
		public override void ProcessKeyDown(KeyEventArgs e) {
			if(e.Key == Key.Tab) {
				if(ModifierKeysHelper.IsShiftPressed(ModifierKeysHelper.GetKeyboardModifiers(e)))
					Document.DocumentStateController.TabBackward();
				else
					Document.DocumentStateController.TabForward();
				e.Handled = true;
			}
		}
		protected override string GetDisplayText(InplaceEditorBase inplaceEditor, string originalDisplayText, object value) {
			return originalDisplayText;
		}
		protected override bool? GetDisplayText(InplaceEditorBase inplaceEditor, string originalDisplayText, object value, out string displayText) {
			displayText = originalDisplayText;
			return true;
		}
		protected override bool PerformNavigationOnLeftButtonDown(MouseButtonEventArgs e) {
			throw new NotImplementedException();
		}
		protected override bool CommitEditing() {
			bool postResult = CurrentCellEditor.Return(x => x.PostEditor(), () => false);
			CurrentCellEditor.Do(x => x.HideEditor(true));
			return postResult;
		}
		protected override FrameworkElement FocusOwner {
			get { throw new NotImplementedException(); }
		}
		protected override EditorShowMode EditorShowMode {
			get { return EditorShowMode.MouseDown; }
		}
		protected override bool EditorSetInactiveAfterClick { get { return false; } }
		protected override Type OwnerBaseType { get { return typeof(PdfPresenterControl); } }
		public PdfPageControl Page { get; set; }
		public PdfPageViewModel PageModel { get; set; }
		public System.Windows.Controls.Grid VisualHost { get; internal set; }
	}
}
