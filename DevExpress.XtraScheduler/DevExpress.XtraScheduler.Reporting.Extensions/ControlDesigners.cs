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
using DevExpress.Utils.Design;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;
using System.Reflection;
using System.Drawing.Drawing2D;
using DevExpress.XtraReports.Design;
using DevExpress.XtraScheduler.Reporting.Native;
using DevExpress.Utils;
using DevExpress.XtraReports.Design.Behaviours;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraScheduler.Reporting.Design {
	#region SchedulerReportAutomaticBindHelper
	static class SchedulerReportAutomaticBindHelper {
		public static bool BindToReportView(ComponentDesigner designer, Type viewType) {
			return AutomaticBindHelper.BindToComponent(designer, DesignSR.View, viewType);
		}
		public static void UnbindFromReportView(ComponentDesigner designer, IComponent componentGoingToRemove) {
			AutomaticBindHelper.UnbindFromRemovedComponent(designer, componentGoingToRemove, DesignSR.View, typeof(ReportViewBase));
		}
		public static bool BindToTimeCellsControl(ComponentDesigner designer, Type cellType) {
			return AutomaticBindHelper.BindToComponent(designer, DesignSR.TimeCells, cellType);
		}
		public static void UnbindFromTimeCellsControl(ComponentDesigner designer, IComponent componentGoingToRemove, Type cellType) {
			AutomaticBindHelper.UnbindFromRemovedComponent(designer, componentGoingToRemove, DesignSR.TimeCells, cellType);
		}
	}
	#endregion
	#region ReportViewBaseDesigner
	public class ReportViewBaseDesigner : ComponentDesigner {
		public ReportViewBaseDesigner() {
		}
		public ReportViewBase View { get { return Component as ReportViewBase; } }
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			IServiceProvider provider = Component.Site as IServiceProvider;
			IDesignerHost host = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
			XtraSchedulerReport report = host.RootComponent as XtraSchedulerReport;
			if (report != null)
				report.Views.Add(View);
		}
		public override void InitializeNewComponent(System.Collections.IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
		}
	}
	#endregion
	#region ReportViewControlBaseDesigner (abstract class)
	public abstract class ReportViewControlBaseDesigner : XRControlDesigner {
		protected ReportViewControlBaseDesigner() {
		}
		public ReportViewControlBase ViewControl { get { return (ReportViewControlBase)Component; } }
		protected virtual Type SupportedViewType { get { return ViewControl != null ? ViewControl.SupportedViewTypes[0] : typeof(ReportViewBase); } }
		protected internal virtual bool IsViewAllowed { get { return true; } }
		protected override void OnLoadComplete(EventArgs e) {
			base.OnLoadComplete(e);
			ViewControl.DropPrintController();
		}
		public override void InitializeNewComponent(System.Collections.IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			BindToReportView();
			changeService.ComponentAdded += new ComponentEventHandler(HandlerComponentAdded);
			changeService.ComponentRemoved += new ComponentEventHandler(HandleComponentRemoved);
			if (ViewControl.View == null)
				AssignNewReportView();
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			base.RegisterActionLists(list);
			list.Add(CreateViewActionList());
		}
		protected virtual ReportViewControlActionList CreateViewActionList() {
			if (SupportedViewType == typeof(ReportDayView))
				return new DayViewControlActionList(this);
			if (SupportedViewType == typeof(ReportWeekView))
				return new WeekViewControlActionList(this);
			if (SupportedViewType == typeof(ReportTimelineView))
				return new TimelineViewControlActionList(this);
			return new ReportViewControlActionList(this);
		}
		protected void BindToReportView() {
			SchedulerReportAutomaticBindHelper.BindToReportView(this, SupportedViewType);
		}
		protected virtual void HandleComponentRemoved(object sender, ComponentEventArgs e) {
			SchedulerReportAutomaticBindHelper.UnbindFromReportView(this, e.Component);
		}
		protected virtual void HandlerComponentAdded(object sender, ComponentEventArgs e) {
			BindToReportView();
		}
		protected virtual void AssignNewReportView() {
			ReportViewBase view = CreateReportView();
			DesignerHost.Container.Add(view);
			ViewControl.View = view;
		}
		protected virtual ReportViewBase CreateReportView() {
			Type viewType = SupportedViewType;
			if (viewType == typeof(ReportViewBase))
				viewType = typeof(ReportDayView);
			return Activator.CreateInstance(viewType) as ReportViewBase;
		}
		protected internal List<Type> GetSupportedHorizontalMasterControlTypes() {
			List<Type> result = new List<Type>();
			FillSupportedHorizontalMasterControlTypes(result);
			return result;
		}
		protected virtual void FillSupportedHorizontalMasterControlTypes(List<Type> supportedTypes) {
			supportedTypes.Add(typeof(ReportViewControlBase));
		}
	}
	#endregion
	#region ReportViewControlDesigner
	public class ReportViewControlDesigner : ReportViewControlBaseDesigner {
		public ReportViewControlDesigner()
			: base() {
		}
		protected override Type SupportedViewType { get { return typeof(ReportViewBase); } }
		protected override ReportViewBase CreateReportView() {
			return new ReportDayView();
		}
	}
	#endregion
	#region ReportRelatedControlDesigner
	[DesignerBehaviour(typeof(ReportRelatedBehaviour))]
	public class ReportRelatedControlDesigner : ReportViewControlBaseDesigner {
		public ReportRelatedControlDesigner() {
		}
		#region Properties
		public new ReportRelatedControlBase ViewControl { get { return (ReportRelatedControlBase)base.ViewControl; } }
		public HorizontalHeadersControlBase HorizontalHeaders {
			get { return ViewControl.HorizontalHeaders; }
			set {
				HorizontalHeadersControlBase oldControl = ViewControl.HorizontalHeaders;
				ViewControl.HorizontalHeaders = value;
				if (oldControl != value)
					OnHorizontalMasterControlAssigned();
			}
		}
		public VerticalHeadersControlBase VerticalHeaders {
			get { return ViewControl.VerticalHeaders; }
			set {
				VerticalHeadersControlBase oldControl = ViewControl.VerticalHeaders;
				ViewControl.VerticalHeaders = value;
				if (oldControl != value)
					OnVerticalMasterControlAssigned();
			}
		}
		public TimeCellsControlBase TimeCells {
			get { return ViewControl.TimeCells; }
			set {
				TimeCellsControlBase oldControl = ViewControl.TimeCells;
				ViewControl.TimeCells = value;
				if (oldControl != value) {
					OnTimeCellsControlAssigned();
				}
			}
		}
		#endregion
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			base.RegisterActionLists(list);
			list.Add(new ReportRelatedControlActionList(this));
		}
		private void OnHorizontalMasterControlAssigned() {
			ReportViewControlBase masterControl = ViewControl.LayoutOptionsHorizontal.MasterControl;
			if (masterControl != null) {
				DesignUtils.ArrangeControlBottom(masterControl, ViewControl);
			}
		}
		private void OnVerticalMasterControlAssigned() {
			ReportViewControlBase masterControl = ViewControl.LayoutOptionsVertical.MasterControl;
			if (masterControl != null) {
				DesignUtils.ArrangeControlRight(masterControl, ViewControl);
			}
		}
		protected virtual void OnTimeCellsControlAssigned() {
			ReportViewControlBase masterControl = ViewControl.TimeCells;
			if (masterControl != null) {
				if (ViewControl.IsHorizontalTimeCells) {
					DesignUtils.ArrangeControlTop(masterControl, ViewControl);
				}
				else {
					DesignUtils.ArrangeControlLeft(masterControl, ViewControl);
				}
			}
		}
		protected internal virtual bool IsValueAllowed(string propertyName, Type valueType) {
			PropertyInfo propInfo = DesignUtils.GetAllowedProperty(ViewControl.GetType(), propertyName);
			if (propInfo == null)
				return false;
			Type validType = propInfo.PropertyType;
			return validType.IsAssignableFrom(valueType);
		}
	}
	public class ReportRelatedBehaviour : DesignerBehaviour {
		static string relationImageName = "DevExpress.XtraScheduler.Reporting.Extensions.Images.Relation.png";
		static Image relationImage = ResourceImageHelper.CreateImageFromResources(relationImageName, Assembly.GetExecutingAssembly());
		new ReportRelatedControlDesigner Designer {
			get { return (ReportRelatedControlDesigner)base.Designer; }
		}
		public ReportRelatedBehaviour(IServiceProvider servProvider)
			: base(servProvider) {
		}
		public override void AdornControl(ControlDrawEventArgs e) {
			if(Designer.ViewControl.HasHorizontalMaster || Designer.ViewControl.HasVerticalMaster)
				DrawBoundControlImage(XRControl, e.Graph, e.Bounds, relationImage);
		}
		void DrawBoundControlImage(XRControl xrControl, Graphics gr, RectangleF bounds, Image image) {
			Region savedClipRegion = gr.Clip;
			GraphicsUnit savedPageUnit = gr.PageUnit;
			Matrix savedTransform = gr.Transform;
			Rectangle viewClientRect = Rectangle.Round(BandViewSvc.GetControlViewClientBounds(xrControl));
			using(Region clipRegion = new Region(viewClientRect)) {
				gr.ResetTransform();
				gr.PageUnit = GraphicsUnit.Pixel;
				gr.Clip = clipRegion;
				try {
					int x = viewClientRect.Right - image.Width - 2;
					int y = viewClientRect.Top + 2;
					gr.DrawImage(image, x, y);
				} finally {
					gr.MultiplyTransform(savedTransform);
					gr.PageUnit = savedPageUnit;
					gr.Clip = savedClipRegion;
				}
			}
		}
	}
	#endregion
	#region DataDependentControlDesigner
	public class DataDependentControlDesigner : ReportRelatedControlDesigner {
		protected internal override bool IsViewAllowed { get { return false; } }
		protected internal virtual bool IsPrintContentModeAllowed { get { return false; } }
		public virtual PrintContentMode PrintContentMode { get { return PrintContentMode.AllColumns; } set { ; } }
		public override void InitializeNewComponent(System.Collections.IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			SchedulerReportAutomaticBindHelper.BindToTimeCellsControl(this, typeof(TimeCellsControlBase));
		}
		protected override void HandleComponentRemoved(object sender, ComponentEventArgs e) {
			base.HandleComponentRemoved(sender, e);
			SchedulerReportAutomaticBindHelper.UnbindFromTimeCellsControl(this, e.Component, typeof(TimeCellsControlBase));
		}
		protected override void HandlerComponentAdded(object sender, ComponentEventArgs e) {
			base.HandlerComponentAdded(sender, e);
			SchedulerReportAutomaticBindHelper.BindToTimeCellsControl(this, typeof(TimeCellsControlBase));
		}
		protected override void OnTimeCellsControlAssigned() {
		}
		protected override void AssignNewReportView() {
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			base.RegisterActionLists(list);
			list.Add(new DataDependentControlActionList(this));
		}
	}
	#endregion
	#region DayViewTimeCellsDesigner
	public class DayViewTimeCellsDesigner : ReportRelatedControlDesigner {
		public DayViewTimeCellsDesigner() {
		}
		public new DayViewTimeCells ViewControl { get { return (DayViewTimeCells)base.ViewControl; } }
		public TimeSpan VisibleTimeStart { get { return ViewControl.VisibleTime.Start; } set { ViewControl.VisibleTime.Start = value; } }
		public TimeSpan VisibleTimeEnd { get { return ViewControl.VisibleTime.End; } set { ViewControl.VisibleTime.End = value; } }
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			base.RegisterActionLists(list);
			list.Add(new DayViewTimeCellsActionList(this));
		}
	}
	#endregion
	#region HorizontalWeekDesigner
	public class HorizontalWeekDesigner : ReportRelatedControlDesigner {
		public HorizontalWeekDesigner() {
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			base.RegisterActionLists(list);
			list.Add(new HorizontalWeekActionList(this));
		}
	}
	#endregion
	#region TimeIntervalInfoDesigner
	public class TimeIntervalInfoDesigner : DataDependentControlDesigner {
		public TimeIntervalInfoDesigner() {
		}
		public new TimeIntervalInfo ViewControl { get { return (TimeIntervalInfo)base.ViewControl; } }
		protected internal override bool IsPrintContentModeAllowed { get { return true; } }
		public override PrintContentMode PrintContentMode { get { return ViewControl.PrintContentMode; } set { ViewControl.PrintContentMode = value; } }
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			base.RegisterActionLists(list);
			list.Add(new TimeIntervalInfoActionList(this));
		}
	}
	#endregion
	#region FormatTimeIntervalInfoDesigner
	public class FormatTimeIntervalInfoDesigner : DataDependentControlDesigner {
		public FormatTimeIntervalInfoDesigner() {
		}
		public new FormatTimeIntervalInfo ViewControl { get { return (FormatTimeIntervalInfo)base.ViewControl; } }
		protected internal override bool IsPrintContentModeAllowed { get { return true; } }
		public override PrintContentMode PrintContentMode { get { return ViewControl.PrintContentMode; } set { ViewControl.PrintContentMode = value; } }
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			base.RegisterActionLists(list);
			list.Add(new FormatTimeIntervalInfoActionList(this));
		}
	}
	#endregion
	#region CorneredControlDesigner (abstract class)
	public abstract class CorneredControlDesigner : ReportRelatedControlDesigner {
		protected CorneredControlDesigner() {
		}
		public abstract int TopCornerIndent { get; set; }
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			base.RegisterActionLists(list);
			list.Add(new CorneredControlActionList(this));
		}
	}
	#endregion
	#region DayViewTimeRulerDesigner
	public class DayViewTimeRulerDesigner : CorneredControlDesigner {
		public DayViewTimeRulerDesigner() {
		}
		public new DayViewTimeRuler ViewControl { get { return (DayViewTimeRuler)base.ViewControl; } }
		public override int TopCornerIndent { get { return ViewControl.Corners.Top; } set { ViewControl.Corners.Top = value; } }
		public override void InitializeNewComponent(System.Collections.IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			SchedulerReportAutomaticBindHelper.BindToTimeCellsControl(this, typeof(DayViewTimeCells));
		}
		protected override void HandleComponentRemoved(object sender, ComponentEventArgs e) {
			base.HandleComponentRemoved(sender, e);
			SchedulerReportAutomaticBindHelper.UnbindFromTimeCellsControl(this, e.Component, typeof(DayViewTimeCells));
		}
		protected override void HandlerComponentAdded(object sender, ComponentEventArgs e) {
			base.HandlerComponentAdded(sender, e);
			SchedulerReportAutomaticBindHelper.BindToTimeCellsControl(this, typeof(DayViewTimeCells));
		}
	}
	#endregion
	#region VerticalResourceHeadersDesigner
	public class VerticalResourceHeadersDesigner : CorneredControlDesigner {
		public VerticalResourceHeadersDesigner() {
		}
		public new VerticalResourceHeaders ViewControl { get { return (VerticalResourceHeaders)base.ViewControl; } }
		public override int TopCornerIndent { get { return ViewControl.Corners.Top; } set { ViewControl.Corners.Top = value; } }
	}
	#endregion
}
