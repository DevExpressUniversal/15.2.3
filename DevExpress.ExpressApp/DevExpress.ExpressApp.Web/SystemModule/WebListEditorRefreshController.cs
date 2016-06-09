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
namespace DevExpress.ExpressApp.Web.SystemModule {
	public class WebListEditorRefreshController : ViewController<ListView> {
		private CollectionSourceBase collectionSource;
		private void collectionSource_Disposed(object sender, EventArgs e) {
			Dispose();
		}
		private void collectionSource_CriteriaApplied(object sender, EventArgs e) {
			if(!((CollectionSourceBase)sender).IsCollectionResetting) {
				OnDataSourceChanged();
			}
		}
		private void objectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e) {
			OnObjectChanged(e.Object);
		}
		private void OnObjectChanged(object obj) {
			if(ObjectSpace != null 
				&& !ObjectSpace.IsNewObject(obj)
				&& collectionSource != null 
				&& collectionSource.ObjectTypeInfo.IsAssignableFrom(XafTypesInfo.Instance.FindTypeInfo(obj.GetType()))
				) {
				OnDataSourceChanged();
			}
		}
		private void OnDataSourceChanged() {
			View.Editor.Refresh();
		}
		protected override void OnActivated() {
			base.OnActivated();
			this.collectionSource = View.CollectionSource;
			collectionSource.CriteriaApplied += new EventHandler(collectionSource_CriteriaApplied);
			collectionSource.Disposed += new EventHandler(collectionSource_Disposed);
			ObjectSpace.ObjectChanged += new EventHandler<ObjectChangedEventArgs>(objectSpace_ObjectChanged);
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			if(collectionSource != null) {
				collectionSource.CriteriaApplied -= new EventHandler(collectionSource_CriteriaApplied);
				collectionSource.Disposed -= new EventHandler(collectionSource_Disposed);
				collectionSource = null;
			}
			if(ObjectSpace != null) {
				ObjectSpace.ObjectChanged -= new EventHandler<ObjectChangedEventArgs>(objectSpace_ObjectChanged);
			}
		}
		public CollectionSourceBase CollectionSource {
			get { return collectionSource; }
		}
	}
}
