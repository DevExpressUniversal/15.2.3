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
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
using System.Drawing;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraScheduler.Drawing {
	#region CellsAppointmentsInfo
	public class CellsAppointmentsInfo {
		#region Fields
		AppointmentBaseCollection appointments;
		AppointmentViewInfoCollection aptViewInfos;
		VisuallyContinuousCellsInfo cellsInfo;
		#endregion
		public CellsAppointmentsInfo(VisuallyContinuousCellsInfo cellsInfo) {
			Guard.ArgumentNotNull(cellsInfo, "cellsInfo");
			this.cellsInfo = cellsInfo;
			this.appointments = new AppointmentBaseCollection();
			this.aptViewInfos = new AppointmentViewInfoCollection();
		}
		public VisuallyContinuousCellsInfo CellsInfo { get { return cellsInfo; } }
		public AppointmentBaseCollection Appointments { get { return appointments; } }
		public AppointmentViewInfoCollection AptViewInfos { get { return aptViewInfos; } }
	}
	#endregion
	#region CellsAppointmentsInfoCollection
	public class CellsAppointmentsInfoCollection : List<CellsAppointmentsInfo> {
	}
	#endregion
	#region AppointmentResourceIdHelper
	public static class AppointmentResourceIdHelper {
		public static AppointmentResourceIdCollection GetActualResourceIds(Appointment apt, bool resourceSharing) {
			if (resourceSharing)
				return apt.ResourceIds;
			AppointmentResourceIdCollection result = new AppointmentResourceIdCollection();
			result.Add(apt.ResourceId);
			return result;
		}
	}
	#endregion  
	public interface IGanttAppointmentViewInfo {
		Rectangle Bounds { get; }
		Appointment Appointment { get; }
		AppointmentViewInfoVisibility Visibility { get; }
		TimeInterval Interval { get; }
		Resource Resource { get; }
	}
	public class GanttAppointmentViewInfoCollection : List<IGanttAppointmentViewInfo> {
	}
	#region GanttAppointmentWrapper
	public class GanttAppointmentViewInfoWrapper {
		#region Fields
		ConnectionPointsInfo connectionPointsInfo;
		DependencyTableType tableType;
		IGanttAppointmentViewInfo innerViewInfo;
		#endregion
		public GanttAppointmentViewInfoWrapper(IGanttAppointmentViewInfo viewInfo) {
			this.connectionPointsInfo = new ConnectionPointsInfo();
			innerViewInfo = viewInfo;
		}
		public object Id { get { return Appointment.Id; } }
		public Appointment Appointment { get { return ViewInfo.Appointment; } }
		public ConnectionPointsInfo ConnectionPointsInfo { get { return connectionPointsInfo; } }
		public DependencyTableType TableType { get { return tableType; } set { tableType = value; } }
		public bool IsParent { get { return TableType == DependencyTableType.OutcomingFromFinish || TableType == DependencyTableType.OutcomingFromStart; } }
		public IGanttAppointmentViewInfo ViewInfo { get { return innerViewInfo; } }		
		public Rectangle Bounds { get { return innerViewInfo.Bounds; } }		
		public AppointmentViewInfoVisibility Visibility { get { return innerViewInfo.Visibility; } }
		internal Point GetNextDependencyConnectionPoint() {
			if (TableType == DependencyTableType.IncomingInFinish || TableType == DependencyTableType.OutcomingFromFinish)
				return GetNextRightConnectionPoint();
			return GetNextLeftConnectionPoint();
		}
		protected internal Point GetNextRightConnectionPoint() {
			Point pt = ConnectionPointsInfo.Right.PeekNext();
			if (pt == Point.Empty)
				pt = ConnectionPointsInfo.Left.PeekNext();
			if (pt == Point.Empty)
				pt = new Point(Bounds.Right, Bounds.Top);
			return pt;
		}
		protected internal Point GetNextLeftConnectionPoint() {
			Point pt = ConnectionPointsInfo.Left.PeekNext();
			if (pt == Point.Empty)
				pt = ConnectionPointsInfo.Right.PeekNext();
			if (pt == Point.Empty)
				pt = new Point(Bounds.Left, Bounds.Top);
			return pt;
		}
		internal ConnectorPosition ConnectorPosition {
			get {
				if (TableType == DependencyTableType.IncomingInStart || TableType == DependencyTableType.OutcomingFromStart)
					return ConnectorPosition.Start;
				return ConnectorPosition.Finish;
			}
		}		
	}
	#endregion
	#region GanttAppointmentViewInfoCollection
	public class GanttAppointmentViewInfoWrapperCollection : DXCollection<GanttAppointmentViewInfoWrapper> {
		public GanttAppointmentViewInfoWrapperCollection GetAppointmentViewInfosById(object id) {
			GanttAppointmentViewInfoWrapperCollection result = new GanttAppointmentViewInfoWrapperCollection();
			int count = Count;
			for (int i = 0; i < count; i++) {
				if (Object.Equals(id, this[i].Id))
					result.Add(this[i]);
			}
			return result;
		}
	}
	#endregion
}
