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
using System.Linq;
using DevExpress.ExpressApp.SystemModule;
namespace DevExpress.ExpressApp.Security {
	public interface IOwnerInitializer {
		void SetMasterObject(object masterObject);
	}
	public class OwnerInitializingController : ViewController<ListView> {
		private NewObjectViewController newObjectViewController;
		private ListViewProcessCurrentObjectController listViewProcessCurrentObjectController;
		public OwnerInitializingController() {
			TargetViewNesting = Nesting.Nested;
			TargetObjectType = typeof(IOwnerInitializer);
		}
		protected override void OnActivated() {
			base.OnActivated();
			newObjectViewController = Frame.GetController<NewObjectViewController>();
			listViewProcessCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
			if(newObjectViewController != null) {
				newObjectViewController.ObjectCreated += MasterObjectInitializingController_ObjectCreated;
				newObjectViewController.ObjectCreating += newObjectViewController_ObjectCreating;
			}
			if(listViewProcessCurrentObjectController != null) {
				listViewProcessCurrentObjectController.CustomProcessSelectedItem += listViewProcessCurrentObjectController_CustomProcessSelectedItem;
			}
		}
		void newObjectViewController_ObjectCreating(object sender, ObjectCreatingEventArgs e) {
			ObjectSpace.CommitChanges();
		}
		void listViewProcessCurrentObjectController_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e) {
			ObjectSpace.CommitChanges();
		}
		void MasterObjectInitializingController_ObjectCreated(object sender, ObjectCreatedEventArgs e) {
			IOwnerInitializer obj = e.CreatedObject as IOwnerInitializer;
			PropertyCollectionSource collectionSource = View.CollectionSource as PropertyCollectionSource;
			if(obj != null && collectionSource != null) {
				obj.SetMasterObject(e.ObjectSpace.GetObject(collectionSource.MasterObject));
			}
		}
		protected override void OnDeactivated() {
			if(newObjectViewController != null) {
				newObjectViewController.ObjectCreated -= MasterObjectInitializingController_ObjectCreated;
			}
		}
	}
}
