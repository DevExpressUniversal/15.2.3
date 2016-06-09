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
using System.Text;
using DevExpress.Utils;
using System.Collections;
using DevExpress.XtraScheduler.Drawing;
using System.Drawing;
using DevExpress.Utils.Drawing;
using System.ComponentModel;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils.Serializing;
using System.Drawing.Design;
using DevExpress.XtraScheduler.Internal;
using System.Linq;
using System.Diagnostics;
namespace DevExpress.XtraScheduler {
	public class GanttView : TimelineView {
		public GanttView(SchedulerControl control)
			: base(control) {
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GanttViewInfo ViewInfo { get { return (GanttViewInfo)base.ViewInfo; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool TimelineScrollBarVisible {
			get { return false; }
			set { }
		}
		protected internal override SchedulerScrollBarVisibility ContainerScrollBarVisibility {
			get { return SchedulerScrollBarVisibility.Never; }
			set { }
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("GanttViewScales"),
#endif
Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraScheduler.Design.TimeScaleCollectionEditor," + AssemblyInfo.SRAssemblySchedulerDesign, typeof(UITypeEditor)),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)
		]
		public new TimeScaleCollection Scales {
			get {
				if (InnerView != null)
					return InnerView.Scales;
				else
					return null;
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("GanttViewAppearance"),
#endif
 Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GanttViewAppearance Appearance { get { return (GanttViewAppearance)base.Appearance; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("GanttViewAppointmentDisplayOptions"),
#endif
 Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public new GanttViewAppointmentDisplayOptions AppointmentDisplayOptions { get { return (GanttViewAppointmentDisplayOptions)base.AppointmentDisplayOptions; } }
		protected internal new InnerGanttView InnerView { get { return (InnerGanttView)base.InnerView; } }
		#endregion
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("GanttViewType")]
#endif
		public override SchedulerViewType Type { get { return SchedulerViewType.Gantt; } }
		protected internal override InnerSchedulerViewBase CreateInnerView() {
			return new InnerGanttView(this, new GanttViewProperties());
		}
		protected internal override SchedulerViewInfoBase CreateViewInfoCore() {
			return new GanttViewInfo(this);
		}
		protected internal override ViewFactoryHelper CreateFactoryHelper() {
			return new GanttViewFactoryHelper();
		}
		protected internal override BaseViewAppearance CreateAppearance() {
			return new GanttViewAppearance();
		}
		protected internal override AppointmentDisplayOptions CreateAppointmentDisplayOptionsCore() {
			return new GanttViewAppointmentDisplayOptions();
		}
		protected internal override ViewPainterBase CreatePainter(SchedulerPaintStyle paintStyle) {
			if (paintStyle == null)
				Exceptions.ThrowArgumentException("paintStyle", paintStyle);
			return paintStyle.CreateGanttViewPainter();
		}
		public virtual void ChangeDependencySelection(AppointmentDependency dep) {
			AppointmentDependencySelectionController controller = Control.AppointmentDependencySelectionController;
			if (controller == null)
				return;
			controller.SelectSingleAppointmentDependency(dep);
		}
		public virtual void SelectDependency(AppointmentDependency dep) {
			if (dep == null)
				Exceptions.ThrowArgumentException("dependency", dep);
			SelectDependencyCore(dep);
		}
		protected internal virtual void SelectDependencyCore(AppointmentDependency dependency) {
			if (Control.Selection == null)
				return;
			Control.SuspendSelectionPaint();
			try {
				ChangeDependencySelection(dependency);
			} finally {
				Control.ResumeSelectionPaint();
			}
		}
	}
}
namespace DevExpress.XtraScheduler.Drawing {
	public class GanttViewFactoryHelper : TimelineViewFactoryHelper {
	}
	#region GanttViewInfo
	public class GanttViewInfo : TimelineViewInfo {
		#region Fields
		DependencyViewInfoCollection dependencyViewInfos;
		#endregion
		public GanttViewInfo(GanttView view)
			: base(view) {
			this.dependencyViewInfos = new DependencyViewInfoCollection();
		}
		#region Properties
		protected internal new GanttViewPainter Painter { get { return (GanttViewPainter)base.Painter; } }
		public DependencyViewInfoCollection DependencyViewInfos { get { return dependencyViewInfos; } }
		internal InnerSchedulerControl InnerControl { get { return View.Control.InnerControl; } }
		public new GanttView View { get { return (GanttView)base.View; } }
		#endregion
		protected internal override void CalcFinalLayoutCore(GraphicsCache cache) {
			base.CalcFinalLayoutCore(cache);
			if (!View.Control.OptionsBehavior.UseAsyncMode)
				ExecuteDependenciesLayoutCalculator();
			else
				View.ThreadManager.Run(() => ExecuteDependenciesLayoutCalculator());
		}
		void ExecuteDependenciesLayoutCalculator() {
			ISchedulerStorageBase storage = View.Control.DataStorage;
			if (storage == null || storage.Appointments.Count == 0 || CellContainers.Count == 0)
				return;
			DependencyViewInfoCollection dependencies = CalculateDependencyViewInfos(GraphicsCachePool.GetCache());
			if (dependencies.Count == 0)
				return;
			DependencyContentLayoutCalculator layoutCalculator = new DependencyContentLayoutCalculator(Painter.DependencyPainter);
			layoutCalculator.CalculateContentLayout(this.CancellationToken.Token, View.Appearance, (GanttViewAppearance)PaintAppearance, dependencies);
			dependencies.CalculateBounds(this.CancellationToken.Token);
			UpdateDependenciesSelection(dependencies);
			if (this.CancellationToken.IsCancellationRequested)
				return;
			lock (DependencyViewInfos)
				DependencyViewInfos.AddRange(dependencies);
			View.Invalidate();
		}
		DependencyViewInfoCollection CalculateDependencyViewInfos(GraphicsCache cache) {
			AppointmentDependencyCollection aptDependencies = new AppointmentDependencyCollection();
			HashSet<object> invisibleAppointmentIdCollection = new HashSet<object>();
			int dependencyCount = 0;
			lock (InnerControl.Storage.AppointmentDependencies)
				dependencyCount = InnerControl.Storage.AppointmentDependencies.Count;
			if (dependencyCount == 0)
				return new DependencyViewInfoCollection();
			AppointmentViewInfoCollection visibleAptViewInfos = CopyAllAppointmentViewInfos();
			CalculateVisibilityDependenciesAndInvisibleAppointmentIdCollection(visibleAptViewInfos, aptDependencies, invisibleAppointmentIdCollection);
			if (aptDependencies.Count == 0)
				return new DependencyViewInfoCollection();
			AppointmentViewInfoCollection invisibleAptViewInfos = GetInvisibleAppointmentViewInfos(invisibleAppointmentIdCollection, cache);
			if (this.CancellationToken.IsCancellationRequested)
				return new DependencyViewInfoCollection();
			GanttAppointmentViewInfoCollection viewInfos = MergeAppointmentViewInfoCollection(visibleAptViewInfos, invisibleAptViewInfos);
			Rectangle visibleBounds = GetVisibleBounds();
			DependencyViewInfosCalculator calculator = new DependencyViewInfosCalculator(Painter.DependencyPainter, visibleBounds, MoreButtons, aptDependencies);
			DependencyViewInfoCollection dependencies = calculator.CalculateDependencyViewInfos(this.CancellationToken.Token, viewInfos);
			return dependencies;
		}
		void CalculateVisibilityDependenciesAndInvisibleAppointmentIdCollection(AppointmentViewInfoCollection allAptViewInfos, AppointmentDependencyBaseCollection aptDependencies, HashSet<object> invisibleAppointmentIdCollection) {
			TimeInterval visibleInternval = View.GetVisibleIntervals().Interval;
			List<AppointmentDependency> dependenciesCopy;
			lock (InnerControl.Storage.AppointmentDependencies)
				dependenciesCopy = new List<AppointmentDependency>(InnerControl.Storage.AppointmentDependencies.Items);
			int count = dependenciesCopy.Count;
			for (int i = 0; i < count; i++) {
				if (this.CancellationToken.IsCancellationRequested) {
					aptDependencies.Clear();
					invisibleAppointmentIdCollection.Clear();
					return;
				}
				AppointmentDependency dependency = dependenciesCopy[i];
				if (!((IInternalSchedulerStorage)InnerControl.Storage).IsDependencyValid(dependency))
					continue;
				bool parentVisible = allAptViewInfos.Any((Func<AppointmentViewInfo, bool>)(vi => dependency.ParentId.Equals(vi.Appointment.Id)));
				bool dependentVisible = allAptViewInfos.Any((Func<AppointmentViewInfo, bool>)(vi => dependency.DependentId.Equals(vi.Appointment.Id)));
				if (!parentVisible && !dependentVisible)
					if (!((IInternalSchedulerStorage)InnerControl.Storage).IsDependencyIntersectsInterval(dependency, visibleInternval))
						continue;
				if (!parentVisible)
					invisibleAppointmentIdCollection.Add(dependency.ParentId);
				if (!dependentVisible)
					invisibleAppointmentIdCollection.Add(dependency.DependentId);
				aptDependencies.Add(dependency);
			}
			aptDependencies = ((IInternalSchedulerStorage)InnerControl.Storage).FilterDependencies(aptDependencies);
		}
		protected internal virtual Rectangle GetVisibleBounds() {
			SchedulerViewCellContainer firstContainer = CellContainers[0];
			SchedulerViewCellContainer lastContainer = CellContainers[CellContainers.Count - 1];
			Rectangle visibleBounds = Rectangle.FromLTRB(firstContainer.Bounds.Left, firstContainer.Bounds.Top, lastContainer.Bounds.Right, lastContainer.Bounds.Bottom);
			return visibleBounds;
		}
		protected internal virtual AppointmentViewInfoCollection GetInvisibleAppointmentViewInfos(HashSet<object> invisibleAppointmentIdCollection, GraphicsCache cache) {
			InvisibleAppointmentViewInfosDispatchCalculator calculator = new InvisibleAppointmentViewInfosDispatchCalculator(this);
			AppointmentViewInfoCollection invisibleViewInfos = calculator.Calculate(invisibleAppointmentIdCollection, cache);
			return invisibleViewInfos;
		}
		protected internal GanttAppointmentViewInfoCollection MergeAppointmentViewInfoCollection(AppointmentViewInfoCollection collection1, AppointmentViewInfoCollection collection2) {
			GanttAppointmentViewInfoCollection result = new GanttAppointmentViewInfoCollection();
			int count = collection1.Count;
			for (int i = 0; i < count; i++)
				result.Add(collection1[i]);
			count = collection2.Count;
			for (int i = 0; i < count; i++)
				result.Add(collection2[i]);
			return result;
		}
		protected internal override void PrepareAppointmentLayout() {
			base.PrepareAppointmentLayout();
			DependencyViewInfos.Clear();
		}
		protected internal override SchedulerHitInfo CalculateDependencyHitInfo(Point pt, SchedulerHitInfo previousHitInfo) {
			int count = DependencyViewInfos.Count;
			for (int i = 0; i < count; i++) {
				SchedulerHitInfo hitInfo = DependencyViewInfos[i].CalculateHitInfo(pt, previousHitInfo);
				if (hitInfo != previousHitInfo)
					return hitInfo;
			}
			return previousHitInfo;
		}
		protected internal virtual void UpdateDependenciesSelection(DependencyViewInfoCollection dependencies) {
			int count = dependencies.Count;
			for (int i = 0; i < count; i++) {
				if (this.CancellationToken.IsCancellationRequested)
					return;
				DependencyViewInfo viewInfo = dependencies[i];
				viewInfo.Selected = IsSelectedViewInfo(viewInfo.Dependencies);
			}
		}
		protected internal virtual bool IsSelectedViewInfo(AppointmentDependencyCollection dependencies) {
			int count = dependencies.Count;
			for (int i = 0; i < count; i++) {
				if (InnerControl.AppointmentDependencySelectionController.IsAppointmentDependencySelected(dependencies[i]))
					return true;
			}
			return false;
		}
		protected internal override TimelineViewAppointmentsLayoutAutoHeightStrategy CreateAppointmentsLayoutAutoHeightStrategy() {
			return new GanttViewAppointmentsLayoutAutoHeightStrategy(View);
		}
		protected internal override TimelineViewAppointmentsLayoutFixedHeightStrategy CreateAppointmentLayoutFixedHeightStrategy() {
			return new GanttViewAppointmentsLayoutFixedHeightStrategy(View);
		}
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (dependencyViewInfos != null) {
						dependencyViewInfos.Clear();
						dependencyViewInfos = null;
					}
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Internal {
	public class GanttViewProperties : TimelineViewProperties, IGanttViewProperties {
	}
}
