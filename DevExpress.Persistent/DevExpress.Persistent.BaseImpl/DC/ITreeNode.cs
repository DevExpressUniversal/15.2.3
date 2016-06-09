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
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base.General;
namespace DomainComponents.Common {
	[DomainComponent]
	public interface IDCTreeNode : ITreeNode {
		new String Name { get; set; }
		IDCTreeNode Owner { get; set; }
		IList<IDCTreeNode> Nodes { get; }
	}
	[DomainLogic(typeof(IDCTreeNode))]
	public class IDCTreeNodeLogic {
		private IBindingList children;
		public ITreeNode Get_Parent(IDCTreeNode instance) {
			return instance.Owner;
		}
		public IBindingList Get_Children(IDCTreeNode instance, IObjectSpace objectSpace) {
			if(children == null) {
				children = new DCTreeNodeBindingList(objectSpace, instance.Nodes);
			}
			return children;
		}
	}
	public sealed class DCTreeNodeBindingList : BindingList<IDCTreeNode> {
		private Boolean isBusy;
		private readonly IObjectSpace objectSpace;
		private readonly HashSet<IDCTreeNode> cache;
		private void objectSpace_ObjectChanged(Object sender, ObjectChangedEventArgs e) {
			OnObjectChanged(e.Object);
		}
		private void objectSpace_ObjectReloaded(object sender, ObjectManipulatingEventArgs e) {
			OnObjectChanged(e.Object);
		}
		private void objectSpace_Reloaded(object sender, EventArgs e) {
			objectSpace.ObjectChanged -= new EventHandler<ObjectChangedEventArgs>(objectSpace_ObjectChanged);
			objectSpace.ObjectReloaded -= new EventHandler<ObjectManipulatingEventArgs>(objectSpace_ObjectReloaded);
			objectSpace.Reloaded -= new EventHandler(objectSpace_Reloaded);
		}
		private void OnObjectChanged(Object obj) {
			if(isBusy) return;
			IDCTreeNode changedObject = obj as IDCTreeNode;
			if(changedObject != null) {
				Int32 index = IndexOf(changedObject);
				if(index < 0) {
					if(cache.Contains(changedObject)) {
						cache.Remove(changedObject);
						OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, -1));
					}
				}
				else {
					if(cache.Contains(changedObject)) {
						OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
					}
					else {
						cache.Add(changedObject);
						OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
					}
				}
			}
		}
		protected override void InsertItem(int index, IDCTreeNode item) {
			try {
				isBusy = true;
				cache.Add(item);
				base.InsertItem(index, item);
			}
			finally {
				isBusy = false;
			}
		}
		protected override void RemoveItem(int index) {
			try {
				isBusy = true;
				IDCTreeNode node = this[index];
				if(cache.Contains(node)) {
					cache.Remove(node);
				}
				base.RemoveItem(index);
			}
			finally {
				isBusy = false;
			}
		}
		public DCTreeNodeBindingList(IObjectSpace objectSpace, IList<IDCTreeNode> list)
			: base(list) {
			this.objectSpace = objectSpace;
			cache = new HashSet<IDCTreeNode>(list);
			objectSpace.ObjectChanged += new EventHandler<ObjectChangedEventArgs>(objectSpace_ObjectChanged);
			objectSpace.ObjectReloaded += new EventHandler<ObjectManipulatingEventArgs>(objectSpace_ObjectReloaded);
			objectSpace.Reloaded += new EventHandler(objectSpace_Reloaded);
		}
	}
}
