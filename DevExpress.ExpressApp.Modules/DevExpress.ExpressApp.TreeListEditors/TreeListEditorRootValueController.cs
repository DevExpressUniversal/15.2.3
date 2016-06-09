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
using DevExpress.ExpressApp.Controls;
using DevExpress.Persistent.Base.General;
namespace DevExpress.ExpressApp.TreeListEditors {
	public class TreeListEditorRootValueController : ViewController {
		public PropertyCollectionSource propertyCollectionSource;
		private NodeObjectAdapter adapter;
		private void propertyCollectionSource_MasterObjectChanged(object sender, EventArgs e) {
			SetRootValue();
		}
		private void propertyCollectionSource_CollectionChanged(object sender, EventArgs e) {
			SetRootValue();
		}
		private void View_ControlsCreated(object sender, EventArgs e) {
			INodeObjectAdapterProvider adapterProvider = ((ListView)View).Editor as INodeObjectAdapterProvider;
			if(adapterProvider != null) {
				SetAdapter(adapterProvider.Adapter);
			}
		}
		protected virtual void SetAdapter(NodeObjectAdapter adapter) {
			this.adapter = adapter;
			SetRootValue();
		}
		protected virtual void SetRootValue() {
			if(adapter != null) {
				object rootValue = null;
				if(propertyCollectionSource != null) {
					object owner = propertyCollectionSource.MemberInfo.GetOwnerInstance(propertyCollectionSource.MasterObject);
					if(owner != null && propertyCollectionSource.MemberInfo.ListElementType != null && (owner.GetType().IsAssignableFrom(propertyCollectionSource.MemberInfo.ListElementType) ||
						 propertyCollectionSource.MemberInfo.ListElementType.IsAssignableFrom(owner.GetType()))) {
						rootValue = owner;
					}
				}
				adapter.RootValue = rootValue;
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			View.ControlsCreated += new EventHandler(View_ControlsCreated);
			propertyCollectionSource = ((ListView)View).CollectionSource as PropertyCollectionSource;
			if(propertyCollectionSource != null) {
				propertyCollectionSource.CollectionChanged += new EventHandler(propertyCollectionSource_CollectionChanged);
				propertyCollectionSource.MasterObjectChanged += new EventHandler(propertyCollectionSource_MasterObjectChanged);
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			View.ControlsCreated -= new EventHandler(View_ControlsCreated);
			if(propertyCollectionSource != null) {
				propertyCollectionSource.CollectionChanged -= new EventHandler(propertyCollectionSource_CollectionChanged);
				propertyCollectionSource.MasterObjectChanged -= new EventHandler(propertyCollectionSource_MasterObjectChanged);
				propertyCollectionSource = null;
			}
		}
		public TreeListEditorRootValueController()
			: base() {
			TargetViewType = ViewType.ListView;
			TargetViewNesting = Nesting.Nested;
			propertyCollectionSource = null;
		}
	}
}
