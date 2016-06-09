#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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

using System.Windows;
using System.Collections.Specialized;
using System.Collections;
using DevExpress.Xpf.TreeMap.Native;
namespace DevExpress.Xpf.TreeMap {
	public class TreeMapItemCollection : FreezableCollection<TreeMapItem>, IOwnedElement {
		object owner;
		protected object Owner { get { return owner; } }
		protected TreeMapControl TreeMap { get { return Owner as TreeMapControl; } }
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				RemoveChildren(this);
				owner = value;
				AddChildren(this);
			}
		}
		protected virtual void AddChildren(IList children) {
			SetOwnerForChildren(children, owner);
		}
		protected virtual void RemoveChildren(IList children) {			
			SetOwnerForChildren(children, null);
		}
		void SetOwnerForChildren(IList children, object owner) {
			foreach (object child in children) {
				IOwnedElement element = child as IOwnedElement;
				if (element != null)
					element.Owner = owner;
				ColorizeItems();
			}			
		}
		#endregion
		public TreeMapItemCollection() {
			((INotifyCollectionChanged)this).CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
		}
		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (e.OldItems != null && (e.Action == NotifyCollectionChangedAction.Reset || e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace))
				RemoveChildren(e.OldItems);
			if (e.NewItems != null && (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)) {
				AddChildren(e.NewItems);
			}
		}
		void ColorizeItems() { 
			if (TreeMap != null) 
				TreeMap.ColorizeItems();
		}
	}
}
