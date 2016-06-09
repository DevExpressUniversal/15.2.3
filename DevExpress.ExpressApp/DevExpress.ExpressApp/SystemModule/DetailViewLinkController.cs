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
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.SystemModule {
	internal class LinkToListViewInitializer {
		private Link link;
		private XafApplication application;
		private Frame sourceFrame;
		private void Application_ViewShown(Object sender, ViewShownEventArgs e) {
			if((application != null) && (link != null) && (sourceFrame == e.SourceFrame)
					&& (e.TargetFrame.GetController<LinkToListViewController>() != null)) {
				e.TargetFrame.GetController<LinkToListViewController>().Link = link;
				application.ViewShown -= new EventHandler<ViewShownEventArgs>(Application_ViewShown);
				link = null;
				sourceFrame = null;
				application = null;
			}
		}
		public LinkToListViewInitializer(XafApplication application, Link link, Frame sourceFrame) {
			this.link = link;
			this.application = application;
			this.sourceFrame = sourceFrame;
			application.ViewShown += new EventHandler<ViewShownEventArgs>(Application_ViewShown);
		}
	}
	public class LinkToListViewController : Controller {
		private Link link;
		private View previousView;
		private void Application_ViewShowing(Object sender, ViewShowingEventArgs e) {
			if((e.TargetFrame is Window) && (e.SourceFrame == Frame)) {
				if((Frame.View is ObjectView)
						&& (e.View is DetailView)
						&& (Link != null) && (Link.ListView != null)
						&& (((ObjectView)Frame.View).ObjectTypeInfo.IsAssignableFrom(((DetailView)e.View).ObjectTypeInfo))) {
					new LinkToListViewInitializer(Application, Link, Frame);
				}
			}
		}
		private void Frame_ViewChanging(Object sender, ViewChangingEventArgs e) {
			previousView = Frame.View;
		}
		private void Frame_ViewChanged(Object sender, ViewChangedEventArgs e) {
			if(link != null) {
				if(previousView is ListView) {
					link.ListView = null;
				}
				previousView = null;
				link = null;
				OnLinkChanged();
			}
			View view = Frame.View;
			if(view is ListView) {
				link = new Link((ListView)view);
				OnLinkChanged();
			}
		}
		protected virtual void OnLinkChanged() {
			if(LinkChanged != null) {
				LinkChanged(this, EventArgs.Empty);
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			Frame.ViewChanging += new EventHandler<ViewChangingEventArgs>(Frame_ViewChanging);
			Frame.ViewChanged += new EventHandler<ViewChangedEventArgs>(Frame_ViewChanged);
			if(Frame.View != null && Frame.View is ListView) {
				link = new Link((ListView)Frame.View);
				OnLinkChanged();
			}
			if(Frame != null && Frame.Application != null) {
				Frame.Application.ViewShowing += new EventHandler<ViewShowingEventArgs>(Application_ViewShowing);
			}
		}
		protected override void OnDeactivated() {
			Frame.ViewChanging -= new EventHandler<ViewChangingEventArgs>(Frame_ViewChanging);
			Frame.ViewChanged -= new EventHandler<ViewChangedEventArgs>(Frame_ViewChanged);
		}
		protected override void Dispose(Boolean disposing) {
			if(Frame != null && Frame.Application != null) {
				Frame.Application.ViewShowing -= new EventHandler<ViewShowingEventArgs>(Application_ViewShowing);
			}
			if(link != null) {
				if(Frame.View is ListView) {
					link.ListView = null;
				}
				link = null;
			}
			LinkChanged = null;
			previousView = null;
			base.Dispose(disposing);
		}
		public Link Link {
			get { return link; }
			set {
				if(Frame.View is ListView) {
					throw new InvalidOperationException();
				}
				link = value;
				OnLinkChanged();
			}
		}
		public event EventHandler LinkChanged;
		public static CollectionSourceBase FindCollectionSource(Frame frame) {
			if(frame != null) {
				LinkToListViewController linkToListViewController = frame.GetController<LinkToListViewController>();
				if(linkToListViewController != null && linkToListViewController.Link != null && linkToListViewController.Link.ListView != null) {
					return linkToListViewController.Link.ListView.CollectionSource;
				}
			}
			return null;
		}
	}
	public class DetailViewLinkController : ViewController {
		private ArrayList deletedObjects = new ArrayList();
		private ArrayList modifiedObjects = new ArrayList();
		private HashSet<Object> potentiallyUsedInListViewObjects;
		private Boolean skipCurrentObjectReloading;
		private Boolean skipCurrentObjectMoving;
		private DetailView DetailView {
			get { return (DetailView)base.View; }
		}
		private void View_CurrentObjectChanged(Object sender, EventArgs e) {
			if(IsLinkInitialized()) {
				Link.ListView.CurrentObject = Link.ListView.GetObject(DetailView.CurrentObject);
			}
		}
		private Boolean IsLinkInitialized() {
			return (Link != null) && (Link.ListView != null) && !Link.ListView.IsDisposed;
		}
		private Boolean NeedToLinkCurrentObject() {
			return (Link != null) && (Link.PropertyCollectionSourceLink != null) && (!Link.PropertyCollectionSourceLink.LinkNewObjectToParentImmediately);
		}
		private void FillPotentiallyUsedInListViewObjects() {
			if((Link.ListView.Model != null) && (Link.ListView.Model.Columns != null)) {
				Object detailViewObject = DetailView.CurrentObject;
				ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(detailViewObject.GetType());
				foreach(IModelColumn columnInfo in Link.ListView.Model.Columns) {
					if(!columnInfo.Index.HasValue || (columnInfo.Index >= 0)) {
						Object currentObject = detailViewObject;
						IMemberInfo descriptor = typeInfo.FindMember(columnInfo.PropertyName);
						if(descriptor != null) {
							foreach(IMemberInfo memberInfo in descriptor.GetPath()) {
								Object memberValue = memberInfo.GetValue(currentObject);
								if(memberValue == null) {
									break;
								}
								if(memberInfo.MemberTypeInfo.IsPersistent) {
									potentiallyUsedInListViewObjects.Add(memberValue);
								}
								currentObject = memberValue;
							}
						}
					}
				}
			}
		}
		private void ClearPotentiallyUsedInListViewObjects() {
			potentiallyUsedInListViewObjects.Clear();
		}
		private Boolean IsPotentiallyUsedInListViewObject(Object obj) {
			return potentiallyUsedInListViewObjects.Contains(obj);
		}
		private void RefreshAllDataInListView() {
			if(IsLinkInitialized() && !skipCurrentObjectReloading && !skipCurrentObjectMoving) {
				Link.ListView.CollectionSource.Reload();
			}
		}
		private void RefreshDeletedObjectsInListView() {
			if(IsLinkInitialized() && !(ObjectSpace is INestedObjectSpace)) {
				CollectionSourceBase collectionSource = Link.ListView.CollectionSource;
				Boolean collectionSource_DeleteObjectOnRemove = collectionSource.DeleteObjectOnRemove;
				if(collectionSource.DeleteObjectOnRemove) {
					collectionSource.DeleteObjectOnRemove = false;
				}
				try {
					if(collectionSource.AllowRemove) {
						ITypesInfo typesInfo = XafTypesInfo.Instance;
						IObjectSpace objectSpace = collectionSource.ObjectSpace;
						foreach(Object obj in deletedObjects) {
							Type objType = obj.GetType();
							ITypeInfo objTypeInfo = typesInfo.FindTypeInfo(objType);
							if(ReflectionHelper.IsTypeAssignableFrom(collectionSource.ObjectTypeInfo, objTypeInfo)) {
								Object currentObj = objectSpace.GetObject(obj);
								if(currentObj != null) {
									if(objectSpace.IsDeletionDeferredType(objType)) {
										objectSpace.ReloadObject(currentObj);
									}
									collectionSource.Remove(currentObj);
								}
							}
						}
					}
				}
				finally {
					if(collectionSource_DeleteObjectOnRemove) {
						collectionSource.DeleteObjectOnRemove = true;
					}
				}
			}
		}
		private void RefreshModifiedObjectsInListView() {
			if(IsLinkInitialized() && (ObjectSpace != Link.ListView.ObjectSpace) && !deletedObjects.Contains(DetailView.CurrentObject)) {
				Object listViewObject = null;
				listViewObject = Link.ListView.ObjectSpace.GetObject(DetailView.CurrentObject);
				if(listViewObject != null) {
					if(!(ObjectSpace is INestedObjectSpace)) {
						if(!skipCurrentObjectReloading) {
							Link.ListView.ObjectSpace.ReloadObject(listViewObject);
						}
						for(Int32 i = modifiedObjects.Count - 1; i >= 0; i--) {
							Link.ListView.ObjectSpace.ReloadObject(modifiedObjects[i]);
						}
					}
					CollectionSourceBase collectionSource = Link.ListView.CollectionSource;
					if(!skipCurrentObjectMoving && collectionSource.CanApplyCriteria) {
						Boolean? isObjectFitForCollection = collectionSource.IsObjectFitForCollection(listViewObject);
						if(isObjectFitForCollection != null) {
							if(isObjectFitForCollection.Value) {
								collectionSource.Add(listViewObject);
							}
							else if(NeedToLinkCurrentObject()) {
								collectionSource.Add(listViewObject);
								Link.ListView.ObjectSpace.SetModified(((PropertyCollectionSource)Link.ListView.CollectionSource).MasterObject);
							}
							else {
								collectionSource.Remove(listViewObject);
							}
						}
					}
				}
			}
		}
		private void ObjectSpace_Committing(Object sender, CancelEventArgs e) {
			modifiedObjects.Clear();
			ClearPotentiallyUsedInListViewObjects();
			if(IsLinkInitialized()
				&& !(ObjectSpace is INestedObjectSpace)
				&& (ObjectSpace != Link.ListView.ObjectSpace)
				&& !deletedObjects.Contains(DetailView.CurrentObject)
				&& DetailView.CurrentObject != null) {
				FillPotentiallyUsedInListViewObjects();
			}
			if(!IsLinkInitialized() && NeedToLinkCurrentObject()) {
				Link.PropertyCollectionSourceLink.GetPropertyCollectionSource(Application, sender as IObjectSpace).Add(DetailView.CurrentObject);
			}
		}
		private void ObjectSpace_ObjectSaved(Object sender, ObjectManipulatingEventArgs e) {
			Object obj = e.Object;
			if(IsPotentiallyUsedInListViewObject(obj)) {
				modifiedObjects.Add(obj);
			}
		}
		private void ObjectSpace_ObjectDeleted(Object sender, ObjectsManipulatingEventArgs e) {
			deletedObjects.AddRange(e.Objects);
		}
		private void ObjectSpace_Committed(Object sender, EventArgs e) {
			if(IsLinkInitialized()) {
				if(Link.ListView.CollectionSource.DataAccessMode == CollectionSourceDataAccessMode.DataView) {
					RefreshAllDataInListView();
				}
				else {
					RefreshDeletedObjectsInListView();
					RefreshModifiedObjectsInListView();
				}
			}
			modifiedObjects.Clear();
			deletedObjects.Clear();
		}
		protected override void OnActivated() {
			base.OnActivated();
			DetailView.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
			ObjectSpace.Committed += new EventHandler(ObjectSpace_Committed);
			ObjectSpace.Committing += new EventHandler<CancelEventArgs>(ObjectSpace_Committing);
			ObjectSpace.ObjectSaved += new EventHandler<ObjectManipulatingEventArgs>(ObjectSpace_ObjectSaved);
			ObjectSpace.ObjectDeleted += new EventHandler<ObjectsManipulatingEventArgs>(ObjectSpace_ObjectDeleted);
		}
		protected override void OnDeactivated() {
			ObjectSpace.Committed -= new EventHandler(ObjectSpace_Committed);
			ObjectSpace.Committing -= new EventHandler<CancelEventArgs>(ObjectSpace_Committing);
			ObjectSpace.ObjectSaved -= new EventHandler<ObjectManipulatingEventArgs>(ObjectSpace_ObjectSaved);
			DetailView.CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
			ObjectSpace.ObjectDeleted -= new EventHandler<ObjectsManipulatingEventArgs>(ObjectSpace_ObjectDeleted);
			deletedObjects.Clear();
			modifiedObjects.Clear();
			potentiallyUsedInListViewObjects.Clear();
			base.OnDeactivated();
		}
		protected virtual Link Link {
			get {
				if((Frame != null) && (Frame.GetController<LinkToListViewController>() != null)) {
					return Frame.GetController<LinkToListViewController>().Link;
				}
				else {
					return null;
				}
			}
		}
		public DetailViewLinkController()
			: base() {
			TypeOfView = typeof(DetailView);
			potentiallyUsedInListViewObjects = new HashSet<Object>();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Boolean SkipCurrentObjectReloading {
			get { return skipCurrentObjectReloading; }
			set { skipCurrentObjectReloading = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Boolean SkipCurrentObjectMoving {
			get { return skipCurrentObjectMoving; }
			set { skipCurrentObjectMoving = value; }
		}
	}
}
