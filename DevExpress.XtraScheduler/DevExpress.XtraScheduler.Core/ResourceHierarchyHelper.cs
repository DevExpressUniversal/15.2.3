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

using System;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Utils;
using System.Collections;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.XtraScheduler.Internal;
#if !SL
using System.Windows.Forms;
#endif
namespace DevExpress.XtraScheduler.Native {
	#region ResourceDependences (enum)
	internal enum ResourceDependences {
		NoCycles,
		CycleDependences
	}
	#endregion
	#region ResourceHierarchyCreatorHelper
	public class ResourceHierarchyHelper {
		public ResourceBaseCollection GetSortedResourcesTree(ResourceBaseCollection storageResources, ISchedulerStorageBase storage) {
			PopulateChildResources(storageResources);
			ResourceBaseCollection result = GetRootResources(storageResources);
			if (((IInternalResourceStorage)storage.Resources).ResourcesTreeSortedColumns.Count > 0) {
				ResourcesTreeComparer comparer = new ResourcesTreeComparer(storage);
				SortResources(result, comparer);
			}
			return result;
		}
		private void SortResources(ResourceBaseCollection resources, ResourcesTreeComparer comparer) {			
			foreach (Resource resource in resources) {
				SortResources(((IInternalResource)resource).ChildResources, comparer);
			}
			resources.Sort(comparer);
		}
		protected internal void PopulateChildResources(ResourceBaseCollection resources) {
			InvalidateChildResources(resources);
			int count = resources.Count;
			for (int i = 0; i < count; i++) {
				Resource current = resources[i];
				PopulateChildResourcesCore(current, resources);
			}
			PrecalculateCountInnerChildResources(resources);
		}
		void PopulateChildResourcesCore(Resource resource, ResourceBaseCollection resources) {
			if (!IsValidParentId(resource, resources))
				return;
			Resource parent = resources.GetResourceById(resource.ParentId);
			if (CheckCycleDependences(parent, resource) == ResourceDependences.NoCycles)
				AddResourceIntoParentChildCollection(resource, parent);
			else
				ProcessCycle(resource, parent);
		}
		protected virtual internal bool IsValidParentId(Resource current, ResourceBaseCollection resources) {
			return current.ParentId != null && current != resources.GetResourceById(current.ParentId)
				&& resources.GetResourceById(current.ParentId) != ResourceBase.Empty;
		}
		void InvalidateChildResources(ResourceBaseCollection resources) {
			int cound = resources.Count;
			for (int i = 0; i < cound; i++) {
				IInternalResource internalResource = (IInternalResource)resources[i];
				internalResource.ChildResources.Clear();
				internalResource.AllChildResourcesCount = 0;
			}
		}		
		protected virtual void AddResourceIntoParentChildCollection(Resource resource, Resource parent) {
			((IInternalResource)parent).ChildResources.Add(resource);
		}
		protected virtual void ProcessCycle(Resource resource, Resource parent) {
		}
		ResourceDependences CheckCycleDependences(Resource parent, Resource nested) {
			if (nested == parent)
				return ResourceDependences.CycleDependences;
			ResourceBaseCollection nestedResources = ((IInternalResource)nested).ChildResources;
			for (int i = 0; nestedResources != null && i < nestedResources.Count; i++) {
				if (CheckCycleDependences(parent, nestedResources[i]) == ResourceDependences.CycleDependences)
					return ResourceDependences.CycleDependences;
			}
			return ResourceDependences.NoCycles;
		}
		public static bool IsResourceAddedToParentChildCollection(ResourceBaseCollection filteredResources, Resource resource) {
			Guard.ArgumentNotNull(filteredResources, "filteredResources");
			Guard.ArgumentNotNull(resource, "resource");
			if (resource.ParentId == null)
				return false;
			Resource parentResource = filteredResources.GetResourceById(resource.ParentId);
			if (parentResource == null)
				return false;
			ResourceBaseCollection childResources = ((IInternalResource)parentResource).ChildResources;
			return childResources.Contains(resource);
		}
		public ResourceBaseCollection GetExpandedResources(ResourceBaseCollection rootResources) {
			ResourceBaseCollection result = new ResourceBaseCollection(DXCollectionUniquenessProviderType.MaximizePerformance);
			foreach (Resource resource in rootResources) 
				result.AddRange(GetExpandedResourcesRecursive(resource));
			return result;
		}
		protected internal virtual ResourceBaseCollection GetExpandedResourcesRecursive(Resource resource) {
			ResourceBaseCollection result = new ResourceBaseCollection();
			result.Add(resource);
			IInternalResource internalResource = (IInternalResource)resource;
			if (!(internalResource.IsExpanded))
				return result;
			foreach (Resource child in internalResource.ChildResources) {
				ResourceBaseCollection childResources = GetExpandedResourcesRecursive(child);
				result.AddRange(childResources);
			}
			return result;
		}
		protected internal virtual int PrecalculateCountInnerChildResources(ResourceBaseCollection resources) {
			int cound = resources.Count;
			int result = 0;
			for (int i = 0; i < cound; i++) {
				IInternalResource internalResource = (IInternalResource)resources[i];
				internalResource.AllChildResourcesCount = internalResource.ChildResources.Count;
				if (internalResource.ChildResources.Count == 0)
					continue;
				internalResource.AllChildResourcesCount += PrecalculateCountInnerChildResources(internalResource.ChildResources);
				result += internalResource.AllChildResourcesCount;
			}
			return result;
		}	   
		public static object GetResourceFieldValue(Resource resource, string fieldName, ISchedulerStorageBase storage)
		{
			object result = resource.CustomFields[fieldName];
			if (result == null)
				result = resource.GetValue(storage, fieldName);
			return result;
		}
		public ResourceBaseCollection GetRootResources(ResourceBaseCollection resources) {
			ResourceBaseCollection result = new ResourceBaseCollection(DXCollectionUniquenessProviderType.MaximizePerformance);
			int count = resources.Count;
			for (int i = 0; i < count; i++) {
				Resource resource = resources[i];
				bool isRootResource = resource.ParentId == null;
				bool isInvalidParentId = resources.GetResourceById(resource.ParentId) == ResourceBase.Empty;
				bool isResourceCauseCycle = !IsResourceAddedToParentChildCollection(resources, resource);
				if (isRootResource || isInvalidParentId || isResourceCauseCycle)
					result.Add(resource);
			}
			return result;
		}
	}
	#endregion
#if SILVERLIGHT
	public enum SortOrder { None, Ascending, Descending };
#endif
	public class ResourcesTreeColumnInfo {
		public ResourcesTreeColumnInfo(string fieldName, SortOrder sortOrder)
		{
			FieldName = fieldName;
			SortOrder = sortOrder;
		}
		public string FieldName { get; set; }
		public SortOrder SortOrder { get; set; }
	}
	public class ResourcesTreeColumnInfos : NotificationCollection<ResourcesTreeColumnInfo>
	{
	}
	public class ResourcesTreeComparer : IComparer<Resource>
	{
		ISchedulerStorageBase storage;
		public ResourcesTreeComparer(ISchedulerStorageBase storage) {
			this.storage = storage;
		}
		internal ISchedulerStorageBase Storage { get { return storage; } }
		internal ResourcesTreeColumnInfos SortedColumns { get { return ((IInternalResourceStorage)storage.Resources).ResourcesTreeSortedColumns; } }
		internal static int InternalCompare(object x, object y)
		{
			if (x == y) return 0;
			if (x == null || x == DBNull.Value) return -1;
			if (y == null || y == DBNull.Value) return 1;
			IComparable xComp = x as IComparable;
			if (x == null) xComp = (IComparable)x.ToString();
			IComparable yComp = y as IComparable;
			if (y == null) yComp = (IComparable)y.ToString();
			int res = 0;
			try {
				res = xComp.CompareTo(yComp);
			}
			catch { }
			return res;
		}
		int IComparer<Resource>.Compare(Resource x, Resource y)
		{
			int res = 0;
			foreach (ResourcesTreeColumnInfo columnID in SortedColumns)
			{
				object v1 = ResourceHierarchyHelper.GetResourceFieldValue(x, columnID.FieldName, Storage);
				object v2 = ResourceHierarchyHelper.GetResourceFieldValue(y, columnID.FieldName, Storage);
				res = InternalCompare(v1, v2);
				if (res == 0) continue;
				if (columnID.SortOrder == SortOrder.Ascending)
					return res;
				res = (res > 0 ? -1 : 1);
				return res;
			}
			if (res == 0 && true) {
				int index1 = this.storage.Resources.Items.IndexOf(x);
				int index2 = this.storage.Resources.Items.IndexOf(y);
				res = index1 - index2;
			}
			return res;
		}
	}
}
