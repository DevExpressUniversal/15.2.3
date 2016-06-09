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
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base.General;
using System.Collections.Generic;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.Scheduler.Win {
	public class SchedulerDetailViewController : ObjectViewController {
		private bool needRefreshResources = false;
		private SchedulerListEditor GetEditor() {
			LinkToListViewController linkController = Frame.GetController<LinkToListViewController>();
			if(linkController.Link != null && linkController.Link.ListView != null) {
				return linkController.Link.ListView.Editor as SchedulerListEditor;
			}
			return null;
		}
		private IObjectSpace GetListViewObjectSpace() {
			IObjectSpace result = null;
			LinkToListViewController linkController = Frame.GetController<LinkToListViewController>();
			if(linkController.Link != null && linkController.Link.ListView != null) {
				result = linkController.Link.ListView.ObjectSpace;
			}
			return result;
		}
		private void RefreshListEditorResources() {
			SchedulerListEditor editor = GetEditor();
			if(editor != null) {
				editor.TryRefreshResourcesDataSource();
			}
		}
		private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e) {
			if(e.Object == View.CurrentObject) {
				if(e.PropertyName == "ResourceId") {
					needRefreshResources = true;
				}
			}
		}
		private void ObjectSpace_Committed(object sender, EventArgs e) {
			if(needRefreshResources) {
				needRefreshResources = false;
				RefreshListEditorResources();
			}
		}
		private void View_Closing(object sender, EventArgs e) {
			RefreshListEditorResources();
		}
		protected override void OnActivated() {
			base.OnActivated();
			View.Closing += new EventHandler(View_Closing);
			ObjectSpace.ObjectChanged += new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
			ObjectSpace.Committed += new EventHandler(ObjectSpace_Committed);
		}
		protected override void OnDeactivated() {
			View.Closing -= new EventHandler(View_Closing);
			ObjectSpace.ObjectChanged -= new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
			ObjectSpace.Committed -= new EventHandler(ObjectSpace_Committed);
			base.OnDeactivated();
		}
		public SchedulerDetailViewController() {
			TargetObjectType = typeof(IEvent);
			TargetViewType = ViewType.DetailView;
		}
	}
}
