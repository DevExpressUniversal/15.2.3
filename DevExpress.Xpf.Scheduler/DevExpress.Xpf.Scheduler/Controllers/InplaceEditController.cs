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
using System.Linq;
using System.Text;
using DevExpress.XtraScheduler;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Commands;
using DevExpress.Xpf.Scheduler;
using DevExpress.Xpf.Scheduler.Native;
using System.Windows;
using DevExpress.Xpf.Scheduler.Drawing;
using System.Windows.Media;
using DevExpress.XtraScheduler.Services.Internal;
using DevExpress.Xpf.Scheduler.UI;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
using System.Windows.Controls;
using System.Windows.Threading;
using DevExpress.Xpf.Scheduler.Internal;
#if SL
using DevExpress.Xpf.Scheduler.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Scheduler.Native {
	#region SchedulerInplaceEditControllerEx
	public class SchedulerInplaceEditControllerEx : SchedulerInplaceEditController {
		ScrollViewer scrollViewer;
		Rect inplaceBounds;
		ISchedulerInplaceEditorEx inplaceEditForm;
		public SchedulerInplaceEditControllerEx(InnerSchedulerControl control)
			: base(control) {
		}
		#region Properties
		SchedulerControl Control { get { return (SchedulerControl)InnerControl.Owner; } }
		#endregion
		public void DoCommit() {
			if (Editor == null)
				return;
			Editor.ApplyChanges();
			Deactivate();
			RaiseCommitChanges();
		}
		protected internal override void BeforeEditorActivate() {
			OpenInplaceEditorContainer();
			InnerControl.MouseHandler.SetCurrentCursor(CursorTypes.Default);
		}		
		protected internal override void AfterEditorDeactivate() {
			CloseInplaceEditorContainer();
		}
		protected virtual void CloseInplaceEditorContainer() {
			CloseInplaceEditorForm();
			if (this.scrollViewer != null) {
				ScrollViewerSubscriber.Unsubscribe(scrollViewer, OnScrollViewerScrollChanged);
				this.scrollViewer = null;
			}
		}
		void CloseInplaceEditorForm() {
			if (this.inplaceEditForm == null)
				return;
			UserControl form = this.inplaceEditForm as UserControl;
			if (form != null)
				SchedulerFormBehavior.Close(form, false);
			this.inplaceEditForm = null;
		}
		protected virtual void OpenInplaceEditorContainer() {
			ShowInplaceEditorForm();
			if (scrollViewer != null)
				ScrollViewerSubscriber.Subscribe(scrollViewer, OnScrollViewerScrollChanged);
		}
		void ShowInplaceEditorForm() {
			if (this.inplaceEditForm == null)
				return;
			UserControl form = inplaceEditForm as UserControl;
			form.FlowDirection = Control.FlowDirection;
			if (form == null)
				return;
			Control.FormManager.ShowInplacementForm(form, this.inplaceBounds);
		}
		protected internal override ISchedulerInplaceEditorEx CreateAppointmentInplaceEditor(Appointment apt) {
			CloseInplaceEditorContainer();
			List<VisualAppointmentInfo> infos = GetVisualAppointmentInfos();
			int count = infos.Count;
			if (count == 0)
				return null;
			this.inplaceBounds = Rect.Empty;
			for (int i = 0; i < count; i++) {
				VisualAppointmentInfo info = infos[i];
				ScrollViewer scrollViewer = LayoutHelper.FindParentObject<ScrollViewer>(info.Panel.Visual) as ScrollViewer;
				if (scrollViewer == null) {
					Point pt = info.Parent.TranslatePoint(RectHelper.Location(info.Bounds), Control);
					this.inplaceBounds = new Rect(pt, RectHelper.Size(info.Bounds));
					break;
				}
				DayViewAppointmentsScrollCalculator calculator = new DayViewAppointmentsScrollCalculator(scrollViewer);
				Rect visibleBounds = calculator.GetVisibleBounds(info, Control);
				if (!visibleBounds.IsEmpty) {
					this.scrollViewer = scrollViewer;
					this.inplaceBounds = visibleBounds;
					break;
				}
			}
			if (this.inplaceBounds.IsEmpty)
				return null;			
			return CreateEditor(EditedAppointment, inplaceBounds);
		}
		protected List<VisualAppointmentInfo> GetVisualAppointmentInfos() {
			Control.UpdateLayout();
			RequestVisualAppointmentInfoEventArgs args = new RequestVisualAppointmentInfoEventArgs(AppointmentDragState.None);
			args.SourceAppointments = new AppointmentBaseCollection();
			args.SourceAppointments.Add(EditedAppointment);
			Control.RaiseRequestVisualAppointmentInfo(args);
			return args.AppointmentInfos;
		}
		protected internal virtual ISchedulerInplaceEditorEx CreateEditor(Appointment apt, Rect bounds) {
			InplaceEditorEventArgs args = new InplaceEditorEventArgs(apt);
			args.Bounds = bounds;
			AppointmentInplaceEditor defaultInplaceEditorEx = Control.FormManager.CreateInplaceEditorForm(apt, RectHelper.Size(bounds)); 
			args.InplaceEditor = defaultInplaceEditorEx;
			Control.RaiseInplaceEditorShowing(args);
			this.inplaceBounds = args.Bounds;
			this.inplaceEditForm = args.InplaceEditor;
			return this.inplaceEditForm;
		}				
		void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e) {
			CloseInplaceEditorContainer();
		}
	}
	#endregion
}
