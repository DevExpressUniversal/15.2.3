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
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.ExpressApp.Web.Editors;
namespace DevExpress.ExpressApp.Web {
	public class UpdateListEditorSelectedObjectsController : ViewController<ListView> {
		private IBindingList previousCollection;
		private List<IBindingList> subscribedLists = new List<IBindingList>();
		private void ObjectSpace_ObjectDeleted(object sender, ObjectsManipulatingEventArgs e) {
			IObjectSpace objectSpace = (IObjectSpace)sender;
			ComplexWebListEditor listEditor = View.Editor as ComplexWebListEditor;
			ISupportSelectionOperations controlSelectionProvider = listEditor as ISupportSelectionOperations;
			if(listEditor != null && controlSelectionProvider != null && listEditor.Control != null && listEditor.CanSelectRows) {
				ArrayList objectsToUnselect = new ArrayList(e.Objects);
				controlSelectionProvider.BeginUpdateSelection();
				foreach(object obj in objectsToUnselect) {
					controlSelectionProvider.UnselectRowByKey(objectSpace.GetKeyValue(obj));
				}
				controlSelectionProvider.EndUpdateSelection();
				listEditor.Refresh();
			}
		}
		private void CollectionSource_CollectionChanging(object sender, EventArgs e) {
			previousCollection = null;
			if(View.CollectionSource.IsLoaded) {
				previousCollection = View.CollectionSource.Collection as IBindingList;
			}
		}
		private void CollectionSource_CollectionChanged(object sender, EventArgs e) {
			if(previousCollection != null) {
				previousCollection.ListChanged -= new ListChangedEventHandler(list_ListChanged);
				subscribedLists.Remove(previousCollection);
				previousCollection = null;
			}
			Subscribe();
		}
		private void CollectionSource_CriteriaApplied(object sender, EventArgs e) {
			if(!((CollectionSourceBase)sender).IsCollectionResetting) {
				ResetSelection();
			}
		}
		private void PropertyCollection_MasterObjectChanged(object sender, EventArgs e) {
			ResetSelection();
		}
		private void Subscribe() {
			IBindingList list = View.CollectionSource.List as IBindingList;
			if(list != null) {
				list.ListChanged += new ListChangedEventHandler(list_ListChanged);
				subscribedLists.Add(list);
			}
		}
		private void list_ListChanged(object sender, ListChangedEventArgs e) {
			if(e.ListChangedType == ListChangedType.ItemDeleted) {
				ResetSelection();
			}
		}
		private void ResetSelection() {
			ComplexWebListEditor listEditor = View.Editor as ComplexWebListEditor;
			ISupportSelectionOperations controlSelectionProvider = listEditor as ISupportSelectionOperations;
			if(controlSelectionProvider != null) {
				if(listEditor.CanSelectRows) {
					controlSelectionProvider.UnselectAll();
				}
				listEditor.Refresh();
			}
		}
		protected override void OnDeactivated() {
			ComplexWebListEditor listEditor = View.Editor as ComplexWebListEditor;
			if(listEditor != null) {
				if(View.CollectionSource is PropertyCollectionSource) {
					((PropertyCollectionSource)View.CollectionSource).MasterObjectChanged -= PropertyCollection_MasterObjectChanged;
				}
				View.CollectionSource.CriteriaApplied -= CollectionSource_CriteriaApplied;
				View.CollectionSource.CollectionChanged -= new EventHandler(CollectionSource_CollectionChanged);
				View.CollectionSource.CollectionChanging -= new EventHandler(CollectionSource_CollectionChanging);
				if(View.ObjectSpace != null) {
					View.ObjectSpace.ObjectDeleted -= new EventHandler<ObjectsManipulatingEventArgs>(ObjectSpace_ObjectDeleted);
				}
				foreach(IBindingList bindingList in subscribedLists) {
					bindingList.ListChanged -= new ListChangedEventHandler(list_ListChanged);
				}
				subscribedLists.Clear();
			}
			previousCollection = null;
			base.OnDeactivated();
		}
		protected override void OnActivated() {
			base.OnActivated();
			ComplexWebListEditor listEditor = View.Editor as ComplexWebListEditor;
			if(listEditor != null) {
				if(View.CollectionSource is PropertyCollectionSource) {
					((PropertyCollectionSource)View.CollectionSource).MasterObjectChanged += PropertyCollection_MasterObjectChanged;
				}
				View.CollectionSource.CriteriaApplied += CollectionSource_CriteriaApplied;
				View.CollectionSource.CollectionChanged += new EventHandler(CollectionSource_CollectionChanged);
				View.CollectionSource.CollectionChanging += new EventHandler(CollectionSource_CollectionChanging);
				View.ObjectSpace.ObjectDeleted += new EventHandler<ObjectsManipulatingEventArgs>(ObjectSpace_ObjectDeleted);
				Subscribe();
			}
		}
		public UpdateListEditorSelectedObjectsController() {
			TargetViewNesting = Nesting.Any;
		}
	}
	public interface ISupportSelectionOperations {
		void UnselectAll();
		void UnselectRowByKey(object key);
		void BeginUpdateSelection();
		void EndUpdateSelection();
	}
}
