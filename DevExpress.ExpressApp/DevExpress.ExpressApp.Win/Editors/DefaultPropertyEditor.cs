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
using System.Collections;
using DevExpress.XtraGrid;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.ExpressApp.Win.Editors {
	public class DefaultPropertyEditor : WinPropertyEditor, IInplaceEditSupport {
		public DefaultPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
		protected override void UpdateControlEnabled(bool enabled) {
			if(Control is StringEdit) {
				((StringEdit)Control).Properties.ReadOnly = true;
			}
			else {
				((GridView)((GridControl)Control).MainView).OptionsBehavior.Editable = enabled;
			}
		}
		protected override object CreateControlCore() {
			Control result;
			if(IsCollectionMember()) {
				GridControl editor = new GridControl();
				editor.TabStop = true;
				GridView gridView = new GridView();
				editor.MainView = gridView;
				gridView.OptionsView.ShowIndicator = false;
				gridView.OptionsBehavior.AllowIncrementalSearch = true;
				gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
				gridView.OptionsView.ShowGroupPanel = false;
				gridView.OptionsView.ShowColumnHeaders = false;
				ControlBindingProperty = "DataSource";
				result = editor;
			}
			else {
				ControlBindingProperty = "Text";
				StringEdit editor = new StringEdit(Model.MaxLength);
				result = editor;
			}
			return result;
		}
		private bool IsCollectionMember() {
			return !typeof(string).IsAssignableFrom(MemberInfo.MemberType) &&
							(typeof(IEnumerable).IsAssignableFrom(MemberInfo.MemberType)
							|| typeof(ITypedList).IsAssignableFrom(MemberInfo.MemberType));
		}
		public RepositoryItem CreateRepositoryItem() {
			if(!IsCollectionMember()) {
				ControlBindingProperty = "EditValue";
				RepositoryItemTextEdit repositoryItemTextEdit = new RepositoryItemTextEdit();
				repositoryItemTextEdit.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(Properties_CustomDisplayText);
				repositoryItemTextEdit.ReadOnly = true;
				return repositoryItemTextEdit;
			}
			return null;
		}
		private void Properties_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e) {
			if((e.Value != null) && (e.DisplayText == e.Value.ToString())) {
				e.DisplayText = ReflectionHelper.GetObjectDisplayText(e.Value);
			}
		}
		protected override void RefreshReadOnly() {
			AllowEdit["AlwaysReadOnly"] = false;
			base.RefreshReadOnly();
		}
		protected override void ReadValueCore() {
			base.ReadValueCore();
			if(Control is GridControl) {
				GridView view = (GridView)((GridControl)Control).MainView;
				view.OptionsView.ShowColumnHeaders = view.Columns.Count > 1;
			}
		}
		protected override void SetTestTag() {
			if(Control is GridControl) {
				((GridControl)Control).Tag = DevExpress.ExpressApp.Utils.EasyTestTagHelper.FormatTestTable(Caption);
			}
			else {
				base.SetTestTag();
			}
		}
	}
}
