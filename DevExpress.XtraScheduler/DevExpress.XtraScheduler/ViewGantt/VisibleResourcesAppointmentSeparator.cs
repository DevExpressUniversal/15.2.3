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

using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public class VisibleResourcesAppointmentSeparator {
		CellsAppointmentsInfo topInvisible;
		CellsAppointmentsInfo bottomInvisible;
		CellsAppointmentsInfoCollection visible;
		ResourceBaseCollection resources;
		bool resourceSharing;
		public VisibleResourcesAppointmentSeparator(ResourceBaseCollection resources, bool resourceSharing) {
			Guard.ArgumentNotNull(resources, "resources");
			this.resources = resources;
			this.resourceSharing = resourceSharing;
		}
		public CellsAppointmentsInfo TopInvisible { get { return topInvisible; } }
		public CellsAppointmentsInfo BottomInvisible { get { return bottomInvisible; } }
		public CellsAppointmentsInfoCollection Visible { get { return visible; } }
		protected internal ResourceBaseCollection FilteredResources { get { return resources; } }
		protected internal bool ResourceSharing { get { return resourceSharing; } }
		public void Process(AppointmentBaseCollection appointments, VisuallyContinuousCellsInfoCollection cellsInfos) {
			if (cellsInfos.Count == 0)
				return;
			Initialize(cellsInfos);
			if (IsGroupedByResource(cellsInfos)) {
				ResourceIdCollection resourceIds = GetResourceIdCollection(cellsInfos);
				ProcessTopAndBottom(appointments, resourceIds);
			}
			ProcessVisibleResources(appointments, cellsInfos);
		}
		public CellsAppointmentsInfoCollection MergeAppointmentCellsInfos() {
			CellsAppointmentsInfoCollection result = new CellsAppointmentsInfoCollection();
			if (TopInvisible != null)
				result.Add(TopInvisible);
			if (BottomInvisible != null)
				result.Add(BottomInvisible);
			result.AddRange(Visible);
			return result;
		}
		protected internal virtual ResourceIdCollection GetResourceIdCollection(VisuallyContinuousCellsInfoCollection cellsInfos) {
			ResourceIdCollection result = new ResourceIdCollection();
			int count = cellsInfos.Count;
			for (int i = 0; i < count; i++) {
				result.Add(cellsInfos[i].Resource.Id);
			}
			return result;
		}
		protected internal virtual void ProcessTopAndBottom(AppointmentBaseCollection appointments, ResourceIdCollection resourceIds) {
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
				Appointment currentApt = appointments[i];
				ResourceIdCollection actualResIds = AppointmentResourceIdHelper.GetActualResourceIds(currentApt, ResourceSharing);
				if (IsTopInvisible(actualResIds, TopInvisible.CellsInfo.Resource))
					TopInvisible.Appointments.Add(currentApt);
				if (IsBottomInvisible(actualResIds, BottomInvisible.CellsInfo.Resource))
					BottomInvisible.Appointments.Add(currentApt);
			}
		}
		protected internal virtual bool IsTopInvisible(ResourceIdCollection aptResIds, Resource res) {
			int count = aptResIds.Count;
			int topVisibleResourceIndex = FilteredResources.IndexOf(res);
			for (int i = 0; i < count; i++) {
				object appointmentResId = aptResIds[i];
				if (!FilteredResources.ResourceExists(appointmentResId))
					continue;
				if (FilteredResources.IndexOfById(appointmentResId) < topVisibleResourceIndex)
					return true;
			}
			return false;
		}
		protected internal virtual bool IsBottomInvisible(ResourceIdCollection aptResIds, Resource res) {
			int count = aptResIds.Count;
			int bottomVisibleResourceIndex = FilteredResources.IndexOf(res);
			for (int i = 0; i < count; i++) {
				object appointmentResId = aptResIds[i];
				if (!FilteredResources.ResourceExists(appointmentResId))
					continue;
				if (FilteredResources.IndexOfById(appointmentResId) > bottomVisibleResourceIndex)
					return true;
			}
			return false;
		}
		void Initialize(VisuallyContinuousCellsInfoCollection cellsInfos) {
			this.visible = new CellsAppointmentsInfoCollection();
			VisuallyContinuousCellsInfo firstCellInfo = (VisuallyContinuousCellsInfo)cellsInfos[0];
			VisuallyContinuousCellsInfo lastCellInfo = (VisuallyContinuousCellsInfo)cellsInfos[cellsInfos.Count - 1];
			this.topInvisible = new CellsAppointmentsInfo(firstCellInfo);
			this.bottomInvisible = new CellsAppointmentsInfo(lastCellInfo);
		}
		bool IsGroupedByResource(VisuallyContinuousCellsInfoCollection cellsInfos) {
			return !((cellsInfos.Count == 1) && (cellsInfos[0].Resource == ResourceInstance.Empty));
		}
		void ProcessVisibleResources(AppointmentBaseCollection appointments, VisuallyContinuousCellsInfoCollection cellsInfos) {
			int count = cellsInfos.Count;
			for (int i = 0; i < count; i++)
				ProcessVisibleResource(appointments, (VisuallyContinuousCellsInfo)cellsInfos[i]);
		}
		void ProcessVisibleResource(AppointmentBaseCollection appointments, VisuallyContinuousCellsInfo cellsInfo) {
			CellsAppointmentsInfo info = new CellsAppointmentsInfo(cellsInfo);
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
				Appointment currentApt = appointments[i];
				ResourceIdCollection actualResIds = AppointmentResourceIdHelper.GetActualResourceIds(currentApt, ResourceSharing);
				if (ResourceBase.InternalMatchIdToResourceIdCollection(actualResIds, cellsInfo.Resource.Id))
					info.Appointments.Add(currentApt);
			}
			Visible.Add(info);
		}
	}
}
