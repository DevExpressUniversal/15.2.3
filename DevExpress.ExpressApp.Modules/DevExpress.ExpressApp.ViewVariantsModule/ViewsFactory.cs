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

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.ExpressApp.ViewVariantsModule {
	public interface IViewsFactory {
		PropertyCollectionSource CreatePropertyCollectionSource(IObjectSpace objectSpace, Type masterObjectType, Object masterObject, IMemberInfo memberInfo, String listViewID, CollectionSourceMode collectionMode);
		CollectionSourceBase CreateCollectionSource(IObjectSpace objectSpace, Type objectType, string listViewID);
		ListView CreateListView(string listViewId, CollectionSourceBase collectionSource, bool isRoot);
		DetailView CreateDetailView(IObjectSpace objectSpace, string viewId, bool isRoot, object currentObject);
		DashboardView CreateDashboardView(IObjectSpace objectSpace, string viewId, bool isRoot);
		event EventHandler<ViewCreatingEventArgs> ViewCreating;
		event EventHandler<ViewCreatedEventArgs> ViewCreated;
	}
	public class XafApplicationViewsFactory : IViewsFactory {
		private void application_ViewCreating(object sender, ViewCreatingEventArgs e) {
			if(ViewCreating != null) {
				ViewCreating(this, e);
			}
		}
		private void application_ViewCreated(object sender, ViewCreatedEventArgs e) {
			if(ViewCreated != null) {
				ViewCreated(this, e);
			}
		}
		public XafApplicationViewsFactory(XafApplication application) {
			Guard.ArgumentNotNull(application, "application");
			this.XafApplication = application;
			this.XafApplication.ViewCreating += application_ViewCreating;
			this.XafApplication.ViewCreated += application_ViewCreated;
		}
		public DetailView CreateDetailView(IObjectSpace objectSpace, string viewId, bool isRoot, object currentObject) {
			return XafApplication.CreateDetailView(objectSpace, viewId, isRoot, currentObject);
		}
		public PropertyCollectionSource CreatePropertyCollectionSource(IObjectSpace objectSpace, Type masterObjectType, object masterObject, IMemberInfo memberInfo, string listViewID, CollectionSourceMode collectionMode) {
			return XafApplication.CreatePropertyCollectionSource(objectSpace, masterObjectType, masterObject, memberInfo, listViewID, collectionMode);
		}
		public CollectionSourceBase CreateCollectionSource(IObjectSpace objectSpace, Type objectType, string listViewID) {
			return XafApplication.CreateCollectionSource(objectSpace, objectType, listViewID);
		}
		public ListView CreateListView(string listViewId, CollectionSourceBase collectionSource, bool isRoot) {
			return XafApplication.CreateListView(listViewId, collectionSource, isRoot);
		}
		public DashboardView CreateDashboardView(IObjectSpace objectSpace, string viewId, bool isRoot) {
			return XafApplication.CreateDashboardView(objectSpace, viewId, isRoot);
		}
		public XafApplication XafApplication { get; private set; }
		public event EventHandler<ViewCreatingEventArgs> ViewCreating;
		public event EventHandler<ViewCreatedEventArgs> ViewCreated;
	}
}
