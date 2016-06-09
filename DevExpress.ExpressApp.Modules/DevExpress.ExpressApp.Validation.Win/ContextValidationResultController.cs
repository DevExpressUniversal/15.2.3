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
using DevExpress.Xpo;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Validation.DiagnosticViews;
using DevExpress.ExpressApp.Win;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Validation.AllContextsView;
using DevExpress.ExpressApp.Utils;
using System.Drawing;
using DevExpress.ExpressApp.Actions;
using System.Collections.ObjectModel;
using DevExpress.XtraEditors.Repository;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Validation.Win {
	public class MemoEditStringPropertyEditor : StringPropertyEditor {
		public MemoEditStringPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
		protected override RepositoryItem CreateRepositoryItem() {
			return new RepositoryItemLargeStringEdit(Model.MaxLength);
		}
	}
	public class ContextValidationResultController : ViewController {
		private List<Type> targetTypes = new List<Type>();
		private string targetTypeKey;
		private void Editor_ControlsCreated(object sender, EventArgs e) {
			GridListEditor editor = sender as GridListEditor;
			if(editor != null && editor.GridView != null) {
				editor.GridView.OptionsView.RowAutoHeight = true;
			}
		}
		public ContextValidationResultController()
			: base() {
				Initialize();
		}
		private void Initialize() {
			targetTypes.Add(typeof(ContextValidationResult));
			targetTypeKey = "";
			foreach(Type type in targetTypes) {
				targetTypeKey += ',' + type.Name;
			}
		}
		protected override void OnViewChanging(View view) {
			Active.SetItemValue("IsListView", view is ListView);
			Active.SetItemValue(targetTypeKey, (view is ObjectView) && targetTypes.Contains(((ObjectView)view).ObjectTypeInfo.Type));
			base.OnViewChanging(view);
		}
		protected override void OnActivated() {
			base.OnActivated();
			((ListView)View).Editor.ControlsCreated += new EventHandler(Editor_ControlsCreated);
		}
		protected override void OnDeactivated() {
			if(((ListView)View).Editor != null) {
				((ListView)View).Editor.ControlsCreated -= new EventHandler(Editor_ControlsCreated);
			}
			base.OnDeactivated();
		}
	}	
}
