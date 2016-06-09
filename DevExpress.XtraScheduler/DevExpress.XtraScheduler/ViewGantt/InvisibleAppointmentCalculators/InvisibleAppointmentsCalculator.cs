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
using DevExpress.XtraScheduler.Internal;
using DevExpress.XtraScheduler.Internal.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.XtraScheduler.Drawing {
	public class InvisibleAppointmentsCalculator {
		GanttViewInfo viewInfo;
		public InvisibleAppointmentsCalculator(GanttViewInfo viewInfo) {
			Guard.ArgumentNotNull(viewInfo, "viewInfo");
			this.viewInfo = viewInfo;
		}
		public GanttViewInfo ViewInfo { get { return viewInfo; } }
		public virtual AppointmentBaseCollection Calculate(HashSet<object> invisibleAppointmentIdCollection, AppointmentBaseCollection nonRecurringApts) {
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			foreach (object aptId in invisibleAppointmentIdCollection) {
				if (ViewInfo.CancellationToken.IsCancellationRequested) {
					result.Clear();
					return result;
				}
				Appointment apt = GetActualAppointment(aptId, nonRecurringApts);
				if (apt == null)
					continue;
				result.Add(apt);
			}
			return result;
		}
		protected internal virtual Appointment GetActualAppointment(object aptId, AppointmentBaseCollection nonRecurringApts) {
			return GetAppointmentById(nonRecurringApts, aptId);
		}
		protected internal virtual Appointment GetAppointmentById(AppointmentBaseCollection apts, object aptId) {
			int count = apts.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = apts[i];
				if (aptId.Equals(apt.Id))
					return apt;
			}
			return null;
		}
		protected internal virtual Appointment GetNonRecurringAppointment(AppointmentViewInfoCollection visibleViewInfos, object aptId) {
			AppointmentViewInfoCollection viewInfos = visibleViewInfos.GetAppointmentViewInfosById(aptId);
			if (viewInfos.Count == 0)
				return null;
			Appointment apt = viewInfos[0].Appointment;
			if (apt.Type != AppointmentType.Normal)
				return null;
			return apt;
		}
		protected internal virtual bool IsAppointmentInvisible(Appointment apt) {
			AppointmentViewInfoCollection viewInfos = GetAppointmentViewInfosById(apt.Id);
			if (ViewInfo.View.GroupType == SchedulerGroupType.None)
				return viewInfos.Count == 0;
			AppointmentResourceIdCollection aptResourceIds = AppointmentResourceIdHelper.GetActualResourceIds(apt, ViewInfo.View.Control.ResourceSharing);
			ResourceBaseCollection viewInfosResources = GetAppointmentViewInfosResources(viewInfos);
			return HasAppointmentInvisibleResource(aptResourceIds, viewInfosResources);
		}
		protected internal virtual bool HasAppointmentInvisibleResource(AppointmentResourceIdCollection aptResourceIds, ResourceBaseCollection viewInfosResources) {
			int count = aptResourceIds.Count;
			for (int i = 0; i < count; i++)
				if (IsAppointmentResourceInvisible(aptResourceIds[i], viewInfosResources))
					return true;
			return false;
		}
		protected internal virtual bool IsAppointmentResourceInvisible(object resId, ResourceBaseCollection viewInfosResources) {
			Resource res = ViewInfo.View.FilteredResources.GetResourceById(resId);
			if (res == ResourceBase.Empty) 
				return false;
			if (!((IInternalResource)res).IsExpanded)
				return false;
			return !viewInfosResources.Contains(res);
		}
		protected internal virtual ResourceBaseCollection GetAppointmentViewInfosResources(AppointmentViewInfoCollection viewInfos) {
			ResourceBaseCollection result = new ResourceBaseCollection();
			int count = viewInfos.Count;
			for (int i = 0; i < count; i++)
				result.Add(viewInfos[i].Resource);
			return result;
		}
		protected internal virtual AppointmentViewInfoCollection GetAppointmentViewInfosById(object aptId) {
			AppointmentViewInfoCollection result = new AppointmentViewInfoCollection();
			result.AddRange(this.ViewInfo.CopyAllAppointmentViewInfos().Where((Func<AppointmentViewInfo, bool>)(vi => object.Equals(vi.Appointment.Id, aptId))));
			return result;
		}
	}
}
