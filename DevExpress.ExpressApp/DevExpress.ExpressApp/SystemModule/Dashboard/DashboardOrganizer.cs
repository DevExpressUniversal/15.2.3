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
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using System.Collections.Generic;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.SystemModule {
	public class DashboardModelUpdater {
		private IModelDashboardView modelDashboardView;
		private DashboardOrganizer dashboardOrganizer;
		private DashboardOrganizationItem FindDasboardOrganizationItem(IModelViewItem modelViewItem) {
			if(modelViewItem != null) {
				foreach(DashboardOrganizationItem item in dashboardOrganizer.ViewItems) {
					if(item.Id == modelViewItem.Id) {
						return item;
					}
				}
			}
			return null;
		}
		private IModelList<IModelViewLayoutElement> FindTargetGroup() {
			foreach(IModelViewLayoutElement element in modelDashboardView.Layout) {
				if(element is IModelLayoutGroup) {
					return (IModelLayoutGroup)element;
				}
			}
			return modelDashboardView.Layout;
		}
		private void ProcessRemovedItems() {
			List<IModelViewItem> itemsToRemove = new List<IModelViewItem>();
			foreach(IModelViewItem item in modelDashboardView.Items) {
				if(FindDasboardOrganizationItem(item) == null) {
					itemsToRemove.Add(item);
				}
			}
			List<IModelLayoutViewItem> layoutItemsToRemove = new List<IModelLayoutViewItem>();
			foreach(IModelLayoutViewItem item in ModelLayoutGroupLogic.GetLayoutItems<IModelLayoutViewItem>(modelDashboardView.Layout)) {
				DashboardOrganizationItem organizationItem = FindDasboardOrganizationItem(item.ViewItem);
				if(itemsToRemove.Contains(item.ViewItem) || (organizationItem != null && organizationItem.Visibility == ViewItemVisibility.Hide)) {
					layoutItemsToRemove.Add(item);
				}
			}
			foreach(IModelLayoutViewItem item in layoutItemsToRemove) {
				item.Remove();
			}
			foreach(IModelViewItem item in itemsToRemove) {
				item.Remove();
			}
		}
		private void SetupViewItems(IList<DashboardOrganizationItem> itemsToCreate) {
			IModelList<IModelViewLayoutElement> targetGroup = FindTargetGroup();
			IList<IModelViewItem> usedInLayout = new List<IModelViewItem>(ModelLayoutViewItemLogic.GetUsedViewItems(modelDashboardView.Layout));
			int index = 0;
			foreach(DashboardOrganizationItem itemToCreate in itemsToCreate) {
				IModelViewItem modelViewItem = modelDashboardView.Items[itemToCreate.Id];
				if(modelViewItem == null) {
					modelViewItem = itemToCreate.CreateDashboardViewItem(modelDashboardView.Items, itemToCreate.Id);
				}
				else {
					itemToCreate.SetupItem(modelViewItem);
				}
				if(itemToCreate.Visibility == ViewItemVisibility.Show && !usedInLayout.Contains(modelViewItem)) {
					IModelLayoutViewItem item = ((IModelNode)targetGroup).AddNode<IModelLayoutViewItem>(modelViewItem.Id);
					item.ViewItem = modelViewItem;
					item.Index = index++;
				}
			}
		}
		public DashboardModelUpdater(IModelDashboardView modelDashboardView, DashboardOrganizer dashboardOrganizer) {
			Guard.ArgumentNotNull(modelDashboardView, "modelDashboardView");
			Guard.ArgumentNotNull(dashboardOrganizer, "dashboardOrganizer");
			this.modelDashboardView = modelDashboardView;
			this.dashboardOrganizer = dashboardOrganizer;
		}
		public void UpdateModel() {
			ProcessRemovedItems();
			SetupViewItems(dashboardOrganizer.ViewItems);
		}
	}
	[DomainComponent]
	public class DashboardOrganizer {
		private IModelDashboardView modelDashboardView;
		private BindingList<DashboardOrganizationItem> viewItems = new BindingList<DashboardOrganizationItem>();
		private Type FindGenericType(Type type) {
			if(type == null) return null;
			if(type.IsGenericType) return type;
			return FindGenericType(type.BaseType);
		}
		private DashboardOrganizationItem CreateDashboardOrganizationItem(IModelViewItem modelViewItem, ViewItemVisibility visibility) {
			ITypeInfo typeToCreate = null;
			Type modelViewItemType = null;
			foreach(ITypeInfo descendant in XafTypesInfo.Instance.FindTypeInfo(typeof(DashboardOrganizationItem)).Descendants) {
				Type[] arguments = descendant.Type.GetGenericArguments();
				Type genericType = FindGenericType(descendant.Type);
				if(genericType != null) {
					arguments = genericType.GetGenericArguments();
				}
				if(!descendant.IsAbstract && arguments.Length == 1 && arguments[0] != typeof(IModelViewItem) && arguments[0].IsAssignableFrom(modelViewItem.GetType())) {
					if(typeToCreate == null || modelViewItemType.IsAssignableFrom(arguments[0])) {
						typeToCreate = descendant;
						modelViewItemType = arguments[0];
					}
				}
			}
			if(typeToCreate != null) {
				DashboardOrganizationItem result = (DashboardOrganizationItem)Activator.CreateInstance(typeToCreate.Type, modelViewItem.Application);
				result.InitializeFromViewItem(modelViewItem);
				result.Visibility = visibility;
				return result;
			}
			return null;
		}
		private void InitializeFromModel(IModelDashboardView modelDashboardView) {
			viewItems.Clear();
			if(modelDashboardView != null) {
				IList<string> usedItemsIds = new List<string>();
				foreach(IModelViewItem item in ModelLayoutViewItemLogic.GetUsedViewItems(modelDashboardView.Layout)) {
					viewItems.Add(CreateDashboardOrganizationItem(item, ViewItemVisibility.Show));
					usedItemsIds.Add(item.Id);
				}
				foreach(IModelViewItem item in modelDashboardView.Items) {
					if(!usedItemsIds.Contains(item.Id)) {
						viewItems.Add(CreateDashboardOrganizationItem(item, ViewItemVisibility.Hide));
					}
				}
			}
		}
		public DashboardOrganizer() {
			viewItems.AllowNew = true;
			viewItems.AllowRemove = true;
		}
		public DashboardOrganizer(IModelDashboardView modelDashboardView) : this() {
			Guard.ArgumentNotNull(modelDashboardView, "modelDashboardView");
			this.modelDashboardView = modelDashboardView;
			InitializeFromModel(this.modelDashboardView);
		}
		public void SaveDashboardChangesToModel(IModelDashboardView targetModel) {
			Guard.ArgumentNotNull(targetModel, "targetModel");
			DashboardModelUpdater modelProcessor = new DashboardModelUpdater(targetModel, this);
			modelProcessor.UpdateModel();
		}
		public void SaveDashboardChangesToModel() {
			if(modelDashboardView != null) {
				SaveDashboardChangesToModel(modelDashboardView);
			}
		}
		public BindingList<DashboardOrganizationItem> ViewItems {
			get { return viewItems; }
		}
	}
}
