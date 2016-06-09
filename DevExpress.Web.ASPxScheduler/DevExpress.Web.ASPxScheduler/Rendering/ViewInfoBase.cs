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
using DevExpress.Web.ASPxScheduler.Drawing;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.Web.ASPxScheduler.Rendering {
	public class ResourcesCellContainers<T> : List<List<T>> where T : IWebViewInfo {
	}
	public abstract class WebViewInfoBase<T> : ISchedulerWebViewInfoBase, ISupportAppointmentsBase
		where T : IWebViewInfo {
		internal static readonly ResourceBaseCollection EmptyResourceCollection = CreateResourceCollection(ResourceBase.Empty);
		static ResourceBaseCollection CreateResourceCollection(XtraScheduler.Resource resource) {
			ResourceBaseCollection result = new ResourceBaseCollection();
			result.Add(resource);
			return result;
		}
		#region Fields
		readonly SchedulerViewBase view;
		readonly MergingContainerBase mergingContainer;
		ResourcesCellContainers<T> resourcesCellContainers;
		List<MergingContainerBase> resourceWebContainers;
		readonly HeaderFormatSeparatorBase headerFormatSeparator;
		IAppointmentComparerProvider appointmentComparerProvider;
		SchedulerCancellationTokenSource tokenSource;
		#endregion
		protected WebViewInfoBase(SchedulerViewBase view) {
			if (view == null)
				Exceptions.ThrowArgumentNullException("view");
			this.view = view;
			this.headerFormatSeparator = CreateHeadersFormatSeparator();
			this.tokenSource = new SchedulerCancellationTokenSource();
			Initialize();
			this.mergingContainer = CreateMergingContainer();
		}
		#region Properties
		public SchedulerViewBase View { get { return view; } }
		public TimeIntervalCollection VisibleIntervals { get { return View.InnerVisibleIntervals; } }
		public AppointmentDisplayOptions AppointmentDisplayOptions { get { return View.AppointmentDisplayOptionsInternal; } }
		public virtual ResourceBaseCollection Resources { get { return View.VisibleResources; } }
		protected internal MergingContainerBase MergingContainer { get { return mergingContainer; } }
		protected internal ResourcesCellContainers<T> ResourcesCellContainers { get { return resourcesCellContainers; } }
		protected internal List<MergingContainerBase> ResourcesWebContainers { get { return resourceWebContainers; } }
		public HeaderFormatSeparatorBase HeaderFormatSeparator { get { return headerFormatSeparator; } }
		IAppointmentComparerProvider ISupportAppointmentsBase.AppointmentComparerProvider {
			get {
				if (appointmentComparerProvider == null && View.Control != null) {
					appointmentComparerProvider = View.Control.GetService(typeof(IAppointmentComparerProvider)) as IAppointmentComparerProvider;
				}
				if (appointmentComparerProvider == null)
					appointmentComparerProvider = new AppointmentComparerProvider(View.Control);
				return appointmentComparerProvider;
			}
		}
		#endregion
		protected internal virtual void Initialize() {
			this.resourcesCellContainers = new ResourcesCellContainers<T>();
			this.resourceWebContainers = new List<MergingContainerBase>();
		}
		public virtual void Create() {
			IWebViewInfo additionalViewElements = CreateAdditionalViewElements();
			IWebViewInfo resourcesView = CreateResourcesView();
			MergingContainer.WebObjects.Add(additionalViewElements);
			MergingContainer.WebObjects.Add(resourcesView);
		}
		protected internal virtual IWebViewInfo CreateResourcesView() {
			MergingContainerBase result = CreateMergingContainer();
			int count = Resources.Count;
			for (int i = 0; i < count; i++) {
				IWebViewInfo resourceView = CreateSingleResourceView(Resources[i]);
				result.WebObjects.Add(resourceView);
				if (i + 1 < count) {
					CreateGroupSeparator(result.WebObjects, resourceView);
				}
			}
			return result;
		}
		protected internal virtual void CreateGroupSeparator(SchedulerWebViewInfoCollection webObjects, IWebViewInfo resourceView) {
			WebGroupSeparator separator = new WebGroupSeparatorHorizontal();
			webObjects.Add(separator);
		}
		protected internal virtual IWebViewInfo CreateSingleResourceView(XtraScheduler.Resource resource) {
			IWebViewInfo resourceHeader = CreateResourceHeader(resource);
			List<T> cellContainers = CreateResourceCellContainers(resource);
			ResourcesCellContainers.Add(cellContainers);
			MergingContainerBase cellContainersViewInfo = CreateResourceCellContainersViewInfo(cellContainers);
			MergingContainerBase mergingContainer = CreateResourceHeaderAndCellContainersMergingContainer();
			mergingContainer.WebObjects.Add(resourceHeader);
			mergingContainer.WebObjects.Add(cellContainersViewInfo);
			ResourcesWebContainers.Add(mergingContainer);
			return mergingContainer;
		}
		protected internal virtual IWebViewInfo CreateResourceHeader(XtraScheduler.Resource resource) {
			return new WebHorizontalResourceHeader(VisibleIntervals, resource);
		}
		protected internal virtual MergingContainerBase CreateResourceCellContainersViewInfo(List<T> containers) {
			MergingContainerBase result = CreateCellContainersMergingContainer();
			int count = containers.Count;
			for (int i = 0; i < count; i++)
				result.WebObjects.Add(CreateResourceCellContainerViewInfo(containers[i]));
			return result;
		}
		protected internal virtual IWebViewInfo CreateResourceCellContainerViewInfo(T container) {
			return container;
		}
		protected internal virtual List<T> CreateResourceCellContainers(XtraScheduler.Resource resource) {
			List<T> result = new List<T>();
			int count = VisibleIntervals.Count;
			for (int i = 0; i < count; i++) {
				T container = CreateCellContainer(VisibleIntervals[i], resource);
				result.Add(container);
			}
			return result;
		}
		public virtual void AssignCellsIds() {
			AssignCellsIdsCore();
		}
		protected internal virtual void AssignCellsIdsCore() {
			WebCellContainerCollection containers = GetContainers();
			int count = containers.Count;
			int horizontalContainerCount = 0;
			int verticalContainerCount = 0;
			for (int i = 0; i < count; i++) {
				IWebCellContainer container = containers[i];
				string containerId;
				if (container.ContainerType == CellContainerType.Horizontal) {
					containerId = GenerateContainerId(container, horizontalContainerCount);
					horizontalContainerCount++;
				} else {
					containerId = GenerateContainerId(container, verticalContainerCount);
					verticalContainerCount++;
				}
				AssignContainerCellsIds(container, containerId);
			}
		}
		protected internal virtual string GenerateContainerId(IWebCellContainer container, int containerIndex) {
			string containerType = container.ContainerType == CellContainerType.Horizontal ? "h" : "v";
			return String.Format("DXCnt{0}{1}_", containerType, containerIndex);
		}
		protected internal virtual void AssignContainerCellsIds(IWebCellContainer container, string containerId) {
			int count = container.CellCount;
			for (int i = 0; i < count; i++) {
				string cellId = GenerateCellId(containerId, i);
				container[i].AssignId(cellId);
			}
		}
		protected internal virtual string GenerateCellId(string containerId, int cellIndex) {
			return containerId + cellIndex.ToString();
		}
		protected internal abstract MergingContainerBase CreateMergingContainer();
		protected internal abstract IWebViewInfo CreateAdditionalViewElements();
		protected internal abstract MergingContainerBase CreateResourceHeaderAndCellContainersMergingContainer();
		protected internal abstract MergingContainerBase CreateCellContainersMergingContainer();
		protected internal abstract T CreateCellContainer(TimeInterval interval, XtraScheduler.Resource resource);
		#region ISchedulerWebViewInfoBase Members
		public virtual WebCellContainerCollection GetContainers() {
			WebCellContainerCollection result = new WebCellContainerCollection();
			int resourceContainersCount = resourcesCellContainers.Count;
			for (int i = 0; i < resourceContainersCount; i++) {
				List<T> resourceContainer = resourcesCellContainers[i];
				result.AddRange(GetWebCellContainers(resourceContainer));
			}
			return result;
		}
		protected internal virtual WebCellContainerCollection GetWebCellContainers(List<T> resourceCellContainers) {
			WebCellContainerCollection result = new WebCellContainerCollection();
			int count = resourceCellContainers.Count;
			for (int j = 0; j < count; j++)
				result.Add(GetWebCellContainer(resourceCellContainers[j]));
			return result;
		}
		protected internal virtual IWebCellContainer GetWebCellContainer(T resourceContainer) {
			return (IWebCellContainer)resourceContainer;
		}
		#endregion
		#region IWebViewInfo Members
		public virtual SchedulerTable CreateTable() {
			return MergingContainer.CreateTable();
		}
		#endregion
		#region ISupportAnchors Members
		public virtual void CreateLeftTopAnchor(AnchorCollection anchors) {
			int count = ResourcesWebContainers.Count;
			for (int i = 0; i < count; i++)
				ResourcesWebContainers[i].CreateLeftTopAnchor(anchors);
		}
		public virtual void CreateRightBottomAnchor(AnchorCollection anchors) {
			int count = ResourcesWebContainers.Count;
			for (int i = 0; i < count; i++)
				ResourcesWebContainers[i].CreateRightBottomAnchor(anchors);
		}
		#endregion
		public virtual int GetResourceColorIndex(XtraScheduler.Resource resource) {
			int index = CalculateVisibleResourceIndex(resource);
			return CalculateResourceColorIndex(index);
		}
		protected internal virtual int CalculateVisibleResourceIndex(XtraScheduler.Resource resource) {
			int count = Resources.Count;
			for (int i = 0; i < count; i++) {
				if (ResourceBase.MatchIds(resource, Resources[i]))
					return i;
			}
			return 0;
		}
		protected internal virtual int CalculateResourceColorIndex(int visibleIndex) {
			return View.ActualFirstVisibleResourceIndex + visibleIndex;
		}
		protected virtual HeaderFormatSeparatorBase CreateHeadersFormatSeparator() {
			return null;
		}
		#region ISupportAppointmentsBase Members
		TimeInterval ISupportAppointmentsBase.GetVisibleInterval() {
			return VisibleIntervals.Interval;
		}
		bool ISupportAppointmentsBase.UseAsyncMode {
			get { return false; }
		}
		SchedulerCancellationTokenSource ISupportAppointmentsBase.CancellationToken {
			get { return this.tokenSource; }
		}
		#endregion
	}
}
