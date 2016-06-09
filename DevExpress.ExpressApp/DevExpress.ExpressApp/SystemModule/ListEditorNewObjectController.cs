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
using System.Text;
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.SystemModule {
	public class ListEditorNewObjectController : ListViewControllerBase {
		private NewObjectViewController newObjectViewController;
		private void Editor_NewObjectAdding(object sender, NewObjectAddingEventArgs e) {
			if(newObjectViewController != null) {
				e.AddedObject = newObjectViewController.CreateObject();
			}
		}
		private void Editor_NewObjectCreated(object sender, EventArgs e) {
			if(View.CurrentObject != null) {
				if(newObjectViewController != null) {
					newObjectViewController.RaiseObjectCreated(View.CurrentObject);
				}
				if((View.CollectionSource is PropertyCollectionSource) && View.CollectionSource.IsServerMode) {
					View.CollectionSource.Add(View.CurrentObject);
				}
			}
		}
		private void Editor_NewObjectCanceled(object sender, EventArgs e) {
			Object obj = View.CurrentObject;
			if(ObjectSpace.IsNewObject(obj)) {
				ObjectSpace.Delete(obj);
				ObjectSpace.RemoveFromModifiedObjects(obj);
				if((View.CollectionSource is PropertyCollectionSource) && View.CollectionSource.IsServerMode) {
					View.CollectionSource.Remove(obj);
				}
			}
		}
		protected override void SubscribeToListEditorEvent() {
			base.SubscribeToListEditorEvent();
			if(View.Editor != null) {
				View.Editor.NewObjectAdding += new EventHandler<NewObjectAddingEventArgs>(Editor_NewObjectAdding);
				View.Editor.NewObjectCreated += new EventHandler(Editor_NewObjectCreated);
				View.Editor.NewObjectCanceled += new EventHandler(Editor_NewObjectCanceled);
			}
		}
		protected override void UnsubscribeToListEditorEvent() {
			base.UnsubscribeToListEditorEvent();
			if(View.Editor != null) {
				View.Editor.NewObjectAdding -= new EventHandler<NewObjectAddingEventArgs>(Editor_NewObjectAdding);
				View.Editor.NewObjectCreated -= new EventHandler(Editor_NewObjectCreated);
				View.Editor.NewObjectCanceled -= new EventHandler(Editor_NewObjectCanceled);
			}
		}
		protected override void OnActivated() {
			if(Frame != null) {
				newObjectViewController = Frame.GetController<NewObjectViewController>();
			}
			base.OnActivated();
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			newObjectViewController = null;
		}
	}
}
