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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraDiagram.InplaceEditing {
	public class DiagramInplaceEditController : IDisposable {
		DiagramEditItemInfo editItemInfo;
		IDiagramInplaceEditOwner container;
		DiagramEditorsRepository repository;
		public DiagramInplaceEditController(IDiagramInplaceEditOwner container) {
			this.editItemInfo = null;
			this.container = container;
			this.repository = CreateEditorsRepository(container);
		}
		public virtual void ShowEditor(DiagramItem item, DiagramEditInfoArgs editInfoArgs) {
			DiagramMaskBox edit = UpdateEditor(editInfoArgs, true);
			if(edit != null) {
				edit.EditTextChanged += OnEditTextChanged;
				edit.KeyDown += OnEditKeyDown;
				edit.KeyUp += OnEditKeyUp;
				edit.MouseWheel += OnEditMouseWheel;
			}
			Container.AddControl(edit);
			edit.Visible = true;
			Form form = Container.FindForm();
			if(form is IFocusablePopupForm && !((IFocusablePopupForm)form).AllowFocus)
				XtraForm.SuppressDeactivation = true;
			edit.Focus();
			XtraForm.SuppressDeactivation = false;
			this.editItemInfo = new DiagramEditItemInfo(edit, item, editInfoArgs);
		}
		public virtual void UpdateEditorBounds(DiagramEditInfoArgs editInfoArgs) {
			DiagramMaskBox edit = ActiveEditor;
			if(edit == null) return;
			UpdateEditorRects(edit, editInfoArgs);
			this.editItemInfo.UpdateEditArgs(editInfoArgs);
		}
		public virtual void RefreshEditor(DiagramEditInfoArgs editInfoArgs) {
			DiagramMaskBox edit = ActiveEditor;
			if(edit == null) return;
			UpdateEditor(editInfoArgs, false);
			this.editItemInfo.UpdateEditArgs(editInfoArgs);
		}
		public virtual void HideEditor() {
			if(!IsEditorVisible) return;
			DiagramMaskBox edit = ActiveEditor;
			if(edit != null) {
				edit.EditTextChanged -= OnEditTextChanged;
				edit.KeyDown -= OnEditKeyDown;
				edit.KeyUp -= OnEditKeyUp;
				edit.MouseWheel -= OnEditMouseWheel;
			}
			Container.Focus();
			edit.Visible = false;
			edit.DestroyHandle();
			Container.RemoveControl(edit);
			this.editItemInfo = null;
		}
		public bool PostEditor() {
			if(!IsEditorVisible) return false;
			EditItemInfo.EditItem.EditValue = EditValue;
			return true;
		}
		public DiagramEditItemInfo EditItemInfo { get { return editItemInfo; } }
		protected virtual DiagramMaskBox UpdateEditor(DiagramEditInfoArgs editArgs, bool allowReset) {
			DiagramMaskBox edit = repository.GetEditor();
			Color backColor = editArgs.EditView.BackColor;
			if(backColor == Color.Empty) {
				backColor = SystemColors.Window;
			}
			Color foreColor = editArgs.EditView.ForeColor;
			if(foreColor == Color.Empty) {
				foreColor = SystemColors.WindowText;
			}
			edit.RightToLeft = editArgs.RightToLeft ? RightToLeft.Yes : RightToLeft.No;
			backColor = Color.FromArgb(255, backColor);
			foreColor = Color.FromArgb(255, foreColor);
			edit.TabStop = false;
			edit.CausesValidation = false;
			if(allowReset) {
				edit.Reset();
				edit.MaskBoxSelectAll();
			}
			edit.BackColor = backColor;
			edit.ForeColor = foreColor;
			edit.Font = editArgs.EditView.Font;
			UpdateEditorRects(edit, editArgs);
			edit.CreateControl();
			edit.EditValue = editArgs.EditValue;
			edit.Modified = false;
			return edit;
		}
		protected virtual void UpdateEditorRects(DiagramMaskBox edit, DiagramEditInfoArgs editArgs) {
			Rectangle editorBounds = editArgs.EditorRects.EditorBounds;
			if(edit.Bounds != editorBounds) {
				edit.Bounds = editorBounds;
			}
			edit.SetTextRect(editArgs.EditorRects.TextRect);
			edit.SetRegion(editArgs.EditorRects.ClipRect);
		}
		protected virtual DiagramEditorsRepository CreateEditorsRepository(IDiagramInplaceEditOwner container) {
			return new DiagramEditorsRepository(container);
		}
		#region Handlers
		protected virtual void OnEditKeyDown(object sender, KeyEventArgs e) {
			Container.OnEditKeyDown(e);
		}
		protected virtual void OnEditKeyUp(object sender, KeyEventArgs e) {
			Container.OnEditKeyUp(e);
		}
		protected virtual void OnEditTextChanged(object sender, EventArgs e) {
			string editValue = ActiveEditor.MaskBoxText;
			EditItemInfo.UpdateEditValue(editValue);
			Container.OnEditTextChanged(editValue);
		}
		protected virtual void OnEditMouseWheel(object sender, MouseEventArgs e) {
			Container.OnEditMouseWheel(e);
		}
		#endregion
		public bool IsEditorVisible { get { return ActiveEditor != null && ActiveEditor.Visible; } }
		public DiagramMaskBox ActiveEditor {
			get { return EditItemInfo != null ? EditItemInfo.Editor : null; }
		}
		public string EditValue {
			get {
				if(ActiveEditor == null) {
					throw new InvalidOperationException("ActiveEditor isn't created");
				}
				return ActiveEditor.EditText;
			}
		}
		public IDiagramInplaceEditOwner Container { get { return container; } }
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(this.repository != null) this.repository.Dispose();
			}
			this.repository = null;
			this.container = null;
			this.editItemInfo = null;
		}
		#endregion
	}
	public class DiagramEditorsRepository : IDisposable {
		DiagramMaskBox editor;
		IDiagramInplaceEditOwner container;
		public DiagramEditorsRepository(IDiagramInplaceEditOwner container) {
			this.editor = null;
			this.container = container;
		}
		public DiagramMaskBox GetEditor() {
			return editor ?? (editor = container.CreateDiagramEditor());
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(this.editor != null) this.editor.Dispose();
			}
			this.editor = null;
			this.container = null;
		}
		#endregion
	}
}
