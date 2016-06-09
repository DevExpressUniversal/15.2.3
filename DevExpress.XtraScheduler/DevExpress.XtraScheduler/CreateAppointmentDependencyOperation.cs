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
using System.Linq;
using System.Collections.Generic;
using DevExpress.XtraScheduler.Operations;
using DevExpress.Services.Internal;
using DevExpress.Utils.Commands;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.Utils.Drawing;
using System.Drawing;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.Native {
	#region AppointmentDependencyCreatingOperation
	public class AppointmentDependencyCreatingOperation : SchedulerAppointmentOperationBase {
		#region Fields
		Color defaultLineColor = Color.Gray;
		Appointment parentAppointment;
		AppointmentViewInfo parentAppointmentViewInfo;
		#endregion
		public AppointmentDependencyCreatingOperation(SchedulerControl scheduler, Appointment parentAppointment)
			: base(scheduler) {
			Guard.ArgumentNotNull(parentAppointment, "parentAppointment");
			this.parentAppointment = parentAppointment;
			this.parentAppointmentViewInfo = GetParentAppointmentViewInfo();
		}
		#region Properties
		protected internal Appointment ParentAppointment { get { return parentAppointment; } }
		#endregion
		AppointmentViewInfo GetParentAppointmentViewInfo() {
			IEnumerable<AppointmentViewInfo> aptViewInfos = ViewInfo.CellContainers.SelectMany(c => c.AppointmentViewInfos);
			foreach (AppointmentViewInfo currentAptViewInfo in aptViewInfos) {
				if (currentAptViewInfo.Appointment == ParentAppointment)
					return currentAptViewInfo;
			}
			return null;
		}
		protected internal override void StartCore() {
			SchedulerControl.Invalidate();
			SchedulerControl.Update();
		}
		protected internal override ICommandUIStateManagerService GetNewService() {
			return new CreateAppointmentDependencyOperationCommandUIStateManagerService();
		}
		public override bool IsSelected(Appointment apt) {
			Point mousePositoin = GetCurrentMousePosition();
			AppointmentViewInfo aptViewInfo = GetActiveAppointmentViewInfo(GetActualAppointmentViewInfos(), mousePositoin);
			if (aptViewInfo == null)
				return base.IsSelected(apt);
			if (aptViewInfo.Appointment != apt)
				return base.IsSelected(apt);
			return aptViewInfo.Bounds.Contains(mousePositoin) && AllowCreateDependency(apt);
		}
		protected internal Point GetCurrentMousePosition() {
			return SchedulerControl.PointToClient(Control.MousePosition);
		}
		protected internal override void OnSchedulerControlPaint(object sender, PaintEventArgs e) {
			Rectangle bounds = parentAppointmentViewInfo.Bounds;
			int y = bounds.Top + bounds.Height / 2;
			Pen pen = new Pen(GetLineColor());
			e.Graphics.DrawLine(pen, new Point(bounds.Right, y), GetCurrentMousePosition());
		}
		protected internal virtual Color GetLineColor() {
			GanttViewInfo ganttViewInfo = ViewInfo as GanttViewInfo;
			if (ganttViewInfo == null)
				return defaultLineColor;
			GanttViewAppearance paintAppearance = (GanttViewAppearance)ganttViewInfo.PaintAppearance;
			GanttDependenciesPainter painter = ganttViewInfo.Painter.DependencyPainter;
			AppearanceObject appearance = DependencyContentLayoutCalculator.CalculateAppearance(ganttViewInfo.View.Appearance, paintAppearance, painter);
			return appearance.ForeColor;
		}
		protected internal override void PrepareSchedulerProperties() {
			SchedulerControl.Capture = true;
		}
		public override void Finish() {
			base.Finish();
			SchedulerControl.Capture = false;
			RestoreCommandUIStateManagerService();
			UnsubscribeSchedulerControlEvents();
			SchedulerControl.Refresh();
		}
		protected internal override void Execute(Point mousePosition, AppointmentViewInfo appointmentViewInfo) {
			Appointment dependentAppointment = appointmentViewInfo.Appointment;
			if (!AllowCreateDependency(dependentAppointment))
				return;
			AppointmentDependency aptDependency = SchedulerControl.DataStorage.CreateAppointmentDependency(ParentAppointment.Id, dependentAppointment.Id, AppointmentDependencyType.FinishToStart);
			SchedulerControl.DataStorage.AppointmentDependencies.Add(aptDependency);
			Finish();
		}
		protected internal virtual bool AllowCreateDependency(Appointment dependentAppointment) {
			bool isAppointmentNormal = dependentAppointment.Type == AppointmentType.Normal;
			bool isValidAppointmentID = dependentAppointment.Id != null;
			IInternalAppointmentDependencyStorage internalDependencyStorageImpl = SchedulerControl.DataStorage.AppointmentDependencies as IInternalAppointmentDependencyStorage;
			bool isAppointmentDependent = internalDependencyStorageImpl.Contains(ParentAppointment.Id, dependentAppointment.Id);
			if (ParentAppointment == dependentAppointment || !isAppointmentNormal || isAppointmentDependent || !isValidAppointmentID)
				return false;
			return true;
		}
		protected internal override void OnMouseMove(MouseEventArgs e) {
			try {
				SchedulerControl.Invalidate();
				SchedulerControl.Update();
				if (ViewInfo != null) {
					foreach (SchedulerViewCellContainer cellContainer in ViewInfo.CellContainers) {
						ViewInfo.UpdateAppointmentsSelection(cellContainer);
					}
				}
			} catch {
				Finish();
			}
		}
		protected internal override void OnKeyDown(KeyEventArgs e) {
			if (e.KeyCode == Keys.Escape)
				Finish();
		}
		protected internal override bool IsAppointmentViewInfoHitCore(AppointmentViewInfo appointmentViewInfo, Point point, Rectangle bounds) {
			return bounds.Contains(point);
		}
		protected internal override void InitializeOperationCore() {
		}
		protected internal override IEnumerable<AppointmentViewInfo> GetActualAppointmentViewInfos() {
			return ViewInfo.CopyAllAppointmentViewInfos();
		}
	}
	#endregion
	#region CreateAppointmentDependencyOperationCommandUIStateManagerService
	public class CreateAppointmentDependencyOperationCommandUIStateManagerService : ICommandUIStateManagerService {
		public void UpdateCommandUIState(Command command, ICommandUIState state) {
			state.Enabled = false;
		}
	}
	#endregion
}
