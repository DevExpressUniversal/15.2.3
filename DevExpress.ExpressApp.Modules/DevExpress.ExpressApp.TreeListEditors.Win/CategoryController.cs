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
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base.General;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Actions;
using System.Collections.Generic;
namespace DevExpress.ExpressApp.TreeListEditors.Win {
	public class CategoryController : ObjectViewController {
		private void NewObjectController_ObjectCreated(object sender, ObjectCreatedEventArgs e) {
			ICategorizedItem newItem = e.CreatedObject as ICategorizedItem;
			if(newItem != null) {
				ITreeNode category = null;
				if((View is DetailView) && (View.CurrentObject != null)) {
					category = (ITreeNode)e.ObjectSpace.GetObject(((ICategorizedItem)View.CurrentObject).Category);
				}
				else if(View is ListView) {
					CategorizedListEditor editor = ((ListView)View).Editor as CategorizedListEditor;
					if((editor != null) && (editor.CategoriesListView.CurrentObject != null)) {
						category = (ITreeNode)e.ObjectSpace.GetObject(editor.CategoriesListView.CurrentObject);
					}
				}
				if(category != null) {
					newItem.Category = category;
				}
			}
		}
		private NewObjectViewController NewObjectViewController {
			get {
				return Frame.GetController<NewObjectViewController>();
			}
		}
		protected override void OnViewChanging(View view) {
			base.OnViewChanging(view);
			Active.SetItemValue("ICategorizedItem",
				(view is ObjectView) && typeof(ICategorizedItem).IsAssignableFrom(((ObjectView)view).ObjectTypeInfo.Type));
		}
		protected override void OnActivated() {
			base.OnActivated();
			NewObjectViewController.ObjectCreated += new EventHandler<ObjectCreatedEventArgs>(NewObjectController_ObjectCreated);
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			NewObjectViewController.ObjectCreated -= new EventHandler<ObjectCreatedEventArgs>(NewObjectController_ObjectCreated);
		}
		public CategoryController() : base() { }
	}
}
