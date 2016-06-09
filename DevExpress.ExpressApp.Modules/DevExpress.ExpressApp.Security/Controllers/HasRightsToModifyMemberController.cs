#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections.ObjectModel;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Security {
	public class HasRightsToModifyMemberController : ObjectViewController {
		public const string AllowEditKey = "HasRightsToModifyMemberController";
		private bool isInUpdateItem = false;
		private void View_CurrentObjectChanged(object sender, EventArgs e) {
			UpdateReadOnlyForSecurityProperties();
		}
		private void HasRightsToModifyMemberController_ItemsChanged(object sender, ViewItemsChangedEventArgs e) {
			UpdateReadOnlyForSecurityProperties();
		}
		private void UpdatePropertyEditor(PropertyEditor editor) {
			if(isInUpdateItem) {
				return;
			}
			if(SecuritySystem.Instance is IRequestSecurity) {
				ViewItem targetEditor = editor;
				if(!(editor is DetailPropertyEditor || editor is ListPropertyEditor)) {
					bool isCurrentEditorAllowsRead = !(editor is IProtectedContentEditor);
					bool isReadGrantedBySecurity = DataManipulationRight.CanRead(View.ObjectTypeInfo.Type, editor.Model.PropertyName, View.CurrentObject, null, View.ObjectSpace);
					if(isCurrentEditorAllowsRead != isReadGrantedBySecurity) {
						targetEditor = Application.EditorFactory.CreateDetailViewEditor(!isReadGrantedBySecurity, editor.Model, View.ObjectTypeInfo.Type, Application, View.ObjectSpace);
						try {
							isInUpdateItem = true;
							View.UpdateItem(targetEditor);
						}
						finally {
							isInUpdateItem = false;
						}
					}
				}
				if(targetEditor is PropertyEditor) {
					bool isEditGrantedBySecurity = GetAllowEdit(editor);
					((PropertyEditor)targetEditor).AllowEdit.SetItemValue(AllowEditKey, isEditGrantedBySecurity);
				}
			} else {
				if(ObjectAccessComparer.CurrentComparer != null) {
					editor.AllowEdit.SetItemValue(AllowEditKey,
						!ObjectAccessComparer.CurrentComparer.IsMemberModificationDenied(View.CurrentObject, editor.MemberInfo));
				}
			}
		}
		private void editor_ControlCreated(object sender, EventArgs e) {
			Guard.ArgumentNotNull(View, "View");
			UpdatePropertyEditor((PropertyEditor)sender);
		}
		private void UpdateReadOnlyForSecurityProperties() {
			UpdatePropertyEditors();
		}
		protected virtual bool GetAllowEdit(PropertyEditor editor) {
			if(editor is ListPropertyEditor && SecuritySystem.Instance is IRequestSecurity) {
				return true; 
			}
			return DataManipulationRight.CanEdit(View.ObjectTypeInfo.Type, editor.Model.PropertyName, View.CurrentObject, null, View.ObjectSpace);
		}
		protected override void OnActivated() {
			base.OnActivated();
			View.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
			((DetailView)View).ItemsChanged += new EventHandler<ViewItemsChangedEventArgs>(HasRightsToModifyMemberController_ItemsChanged);
			if(View.CurrentObject != null) {
				UpdateReadOnlyForSecurityProperties();
			}
		}
		protected override void OnDeactivated() {
			if(View != null) {
				((DetailView)View).ItemsChanged -= new EventHandler<ViewItemsChangedEventArgs>(HasRightsToModifyMemberController_ItemsChanged);
				View.CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
				foreach(PropertyEditor editor in ((DetailView)View).GetItems<PropertyEditor>()) {
					editor.ControlCreated -= new EventHandler<EventArgs>(editor_ControlCreated);
					editor.AllowEdit.RemoveItem(AllowEditKey);
				}
			}
			delayedPropertyEditors = null;
			base.OnDeactivated();
		}
		public HasRightsToModifyMemberController() {
			TargetViewType = ViewType.DetailView;
		}
		private ReadOnlyCollection<PropertyEditor> delayedPropertyEditors;
		public void UpdatePropertyEditors() {
			if(View == null) {
				return;
			}
			if(delayedPropertyEditors != null) {
				foreach(PropertyEditor editor in delayedPropertyEditors) {
					editor.ControlCreated -= new EventHandler<EventArgs>(editor_ControlCreated);
				}
				delayedPropertyEditors = null;
			}
			ISupportUpdate updatableContainer = null;
			if(((DetailView)View).LayoutManager != null) {
				updatableContainer = ((DetailView)View).LayoutManager.Container as ISupportUpdate;
			}
			if(updatableContainer != null) {
				updatableContainer.BeginUpdate();
			}
			List<PropertyEditor> currentDelayedPropertyEditors = new List<PropertyEditor>();
			foreach(PropertyEditor editor in ((DetailView)View).GetItems<PropertyEditor>()) {
				if(editor.Control == null) {
					editor.ControlCreated += new EventHandler<EventArgs>(editor_ControlCreated);
					currentDelayedPropertyEditors.Add(editor);
				}
				else {
					UpdatePropertyEditor(editor);
				}
			}
			if(currentDelayedPropertyEditors.Count > 0) {
				delayedPropertyEditors = new ReadOnlyCollection<PropertyEditor>(currentDelayedPropertyEditors);
			}
			if(updatableContainer != null) {
				updatableContainer.EndUpdate();
			}
		}
#if DebugTest
		#region DebugTest
		public ReadOnlyCollection<PropertyEditor> DEBUGTEST_HandledPropertyEditors { get { return delayedPropertyEditors; } }
		#endregion
#endif
	}
}
