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

using DevExpress.Xpf.Scheduler;
using System;
using DevExpress.Xpf.Scheduler.UI;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using System.Windows.Input;
using DevExpress.Xpf.Scheduler.Drawing;
#if SL
using PlatformIndependenDragDropEffects = DevExpress.Utils.DragDropEffects;
using DevExpress.Xpf.Scheduler.WPFCompatibility;
#else
using PlatformIndependenDragDropEffects = System.Windows.Forms.DragDropEffects;
#endif
namespace DevExpress.XtraScheduler.Native {
	public enum AppointmentDragState {
		None,
		Begin,
		Drag,
		Cancel,
		Commit,
	}
#if SILVERLIGHT
	public static class TranslatePointExtension {
		public static Point TranslatePoint(this FrameworkElement elem, Point pt, UIElement to) {
			GeneralTransform t = elem.TransformToVisual(to ?? Application.Current.RootVisual);
			return t.Transform(pt);
		}
		public static Point TranslatePoint(this FrameworkElement elem, Point pt, FrameworkElement to) {
			GeneralTransform t = elem.TransformToVisual(to ?? Application.Current.RootVisual);
			return t.Transform(pt);
		}
	}
#endif
	public class VisualAppointmentInfo {
		VisualAppointmentControl container;
		Rect bounds;
		UIElement parent;
		IAppointmentsPanel panel;
		public VisualAppointmentInfo(VisualAppointmentControl container, Rect bounds, UIElement parent, IAppointmentsPanel panel) {
			this.bounds = bounds;
			this.parent = parent;
			this.container = container;
			this.panel = panel;
		}
		public VisualAppointmentControl Container { get { return container; } }
		public Rect Bounds { get { return bounds; } }
		public UIElement Parent { get { return parent; } }
		public IAppointmentsPanel Panel { get { return panel; } }
		public void SetBounds(Rect bounds, UIElement relativeTo) {
			this.bounds = bounds;
			this.parent = relativeTo;
		}
	}
	public class RequestVisualAppointmentInfoEventArgs : EventArgs {
		AppointmentDragState dragState;
		List<VisualAppointmentInfo> appointmentInfos;
		public RequestVisualAppointmentInfoEventArgs(AppointmentDragState dragEventType)  {
			this.dragState = dragEventType;
			this.appointmentInfos = new List<VisualAppointmentInfo>();
		}
		public AppointmentDragState DragState { get { return dragState; } }
		public List<VisualAppointmentInfo> AppointmentInfos { get { return appointmentInfos; } }
		public AppointmentBaseCollection SourceAppointments { get; set; }
		public Appointment PrimaryAppointment { get; set; }
		public bool Copy { get; set; }
	}
	public delegate void RequestVisualAppointmentInfoEventHandler(object sender, RequestVisualAppointmentInfoEventArgs e);	
	public class XpfAppointmentChangeHelper : AppointmentChangeHelper {
		public XpfAppointmentChangeHelper(SchedulerControl control)
			: base(control.InnerControl) {
		}	   
		protected internal override AppointmentBaseCollection GetActualVisibleAppointments(AppointmentBaseCollection sourceAppointments) {
			if (!(State is DragAppointmentChangeHelperState))
				return base.GetActualVisibleAppointments(sourceAppointments);
			AppointmentBaseCollection appointments = new AppointmentBaseCollection();
			appointments.AddRange(sourceAppointments);
			if (!Active || ShouldUndoChanges)
				return appointments;
			if (State is EditNewAppointmentViaInplaceEditorChangeHelperState)
				appointments.AddRange(GetVisibleEditedAppointments());
			return appointments;
		}
	}
}
