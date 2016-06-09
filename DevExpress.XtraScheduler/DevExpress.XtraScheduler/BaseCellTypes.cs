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
using System.Drawing;
using System.Collections;
using DevExpress.Utils.Drawing;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Native;
using System.Collections.Generic;
using DevExpress.XtraEditors;
using System.ComponentModel;
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class SchedulerViewCellContainer : SchedulerViewCellBase, IAppointmentViewInfoContainer<AppointmentViewInfoCollection>, IDisposable {
		SchedulerViewCellBaseCollection cells;
		SchedulerColorSchema colorSchema;
		ScrollContainerController scrollController;
		bool isDisposed;
		AppointmentViewInfoCollection appointmentViewInfoCollection;
		protected SchedulerViewCellContainer(SchedulerColorSchema colorSchema) {
			this.colorSchema = colorSchema;
			this.cells = CreateCells();
			this.scrollController = CreateScrollController();
			this.appointmentViewInfoCollection = new AppointmentViewInfoCollection();
		}
		internal SchedulerColorSchema ColorSchema { get { return colorSchema; } }
		public SchedulerViewCellBaseCollection Cells { get { return cells; } }
		internal bool IsDisposed { get { return isDisposed; } }
		public ScrollContainerController ScrollController { get { return scrollController; } }
		public AppointmentViewInfoCollection AppointmentViewInfos {
			get { return appointmentViewInfoCollection; }
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (scrollController != null) {
					scrollController.Dispose();
					scrollController = null;
				}
				if (appointmentViewInfoCollection != null) {
					foreach (AppointmentViewInfo vi in appointmentViewInfoCollection)
						vi.Dispose();
					appointmentViewInfoCollection.Clear();
					appointmentViewInfoCollection = null;
				}
			}
			this.isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~SchedulerViewCellContainer() {
			Dispose(false);
		}
		#endregion
		protected internal virtual ScrollContainerController CreateScrollController() {
			return new ScrollContainerController(this);
		}
		public virtual SchedulerViewCellBase CreateCell(TimeInterval interval) {
			SchedulerViewCellBase cell = CreateCellInstance();
			InitializeCell(cell, interval);
			return cell;
		}
		protected internal abstract SchedulerViewCellBase CreateCellInstance();
		protected internal virtual void InitializeCell(SchedulerViewCellBase cell, TimeInterval interval) {
			cell.Resource = Resource;
			cell.Interval = interval;
		}
		protected internal override void CalculateFinalAppearance(BaseViewAppearance appearance, SchedulerColorSchema colorSchema) {
			base.CalculateFinalAppearance(appearance, colorSchema);
			CalculateCellsFinalAppearance(appearance, colorSchema);
		}
		protected internal virtual void CalculateCellsFinalAppearance(BaseViewAppearance appearance, SchedulerColorSchema colorSchema) {
			int count = Cells.Count;
			for (int i = 0; i < count; i++) {
				Cells[i].CalculateFinalAppearance(appearance, colorSchema);
			}
		}
		protected internal virtual SchedulerViewCellBaseCollection CreateCells() {
			return new SchedulerViewCellBaseCollection();
		}
		protected internal override void UpdateSelection(SchedulerViewSelection selection) {
			int count = Cells.Count;
			for (int i = 0; i < count; i++) {
				Cells[i].UpdateSelection(selection);
			}
		}
		public override SchedulerHitInfo CalculateHitInfo(Point pt, SchedulerHitInfo nextHitInfo) {
			if (!Bounds.Contains(pt))
				return nextHitInfo;
			int count = cells.Count;
			SchedulerHitInfo cellContainerHitInfo = new SchedulerHitInfo(this, HitTestType, nextHitInfo);
			for (int i = 0; i < count; i++) {
				SchedulerHitInfo hitInfo = cells[i].CalculateHitInfo(pt, cellContainerHitInfo);
				if (hitInfo != cellContainerHitInfo)
					return hitInfo;
			}
			return cellContainerHitInfo;
		}
		#region Changed
		EventHandler onScrollBarValueChanged;
		internal event EventHandler ScrollBarValueChanged { add { onScrollBarValueChanged += value; } remove { onScrollBarValueChanged -= value; } }
		protected internal virtual void RaiseScrollBarValueChanged() {
			if (onScrollBarValueChanged != null)
				onScrollBarValueChanged(this, EventArgs.Empty);
		}
		#endregion
		protected internal virtual void CreateScrollBar(SchedulerControl schedulerControl) {
			ScrollController.CreateScrollBar(schedulerControl);
		}
		protected internal virtual void UpdateScrollBar(AppointmentViewInfoCollection appointments, Rectangle bounds, int gapBetweenAppointments) {
			ScrollController.UpdateScrollBar(appointments, bounds, gapBetweenAppointments);
		}
		protected internal virtual int CalculateScrollOffset() {
			return ScrollController.CalculateScrollOffset();
		}
		protected internal bool MakeAppointmentViewInfoVisible(AppointmentViewInfo appointmentViewInfo, AppointmentViewInfoCollection viewInfos) {
			return ScrollController.MakeAppointmentViewInfoVisible(appointmentViewInfo, viewInfos);
		}
	}
	#region SchedulerViewCellContainerCollection
	public class SchedulerViewCellContainerCollection : DXCollection<SchedulerViewCellContainer> {
		public int TotalAppointmentViewInfoCount {
			get {
				return this.Sum(c => {
					lock (c.AppointmentViewInfos)
						return c.AppointmentViewInfos.Count;
				});
			}
		}
	}
	#endregion
	public abstract class SchedulerViewCellBase : BorderObjectViewInfo, ITimeCell, ISchedulerObjectAnchor {
		AppearanceObject appearance;
		AppearanceObject selectionAppearance;
		protected SchedulerViewCellBase() {
			this.appearance = new AppearanceObject();
			this.selectionAppearance = new AppearanceObject();
		}
		public virtual Rectangle ContentBounds { get { return Bounds; } }
		public AppearanceObject Appearance { get { return appearance; } }
		public AppearanceObject SelectionAppearance { get { return selectionAppearance; } }
		public override SchedulerHitTest HitTestType { get { return SchedulerHitTest.Cell; } }
		protected internal virtual void UpdateSelection(SchedulerViewSelection selection) {
			Selected = selection.Interval.IntersectsWithExcludingBounds(Interval) && Object.Equals(Resource.Id, selection.Resource.Id);
		}
		protected internal virtual bool RaiseCustomDrawEvent(GraphicsCache cache, ISupportCustomDraw customDrawProvider, DefaultDrawDelegate defaultDrawDelegate) {
			this.Cache = cache;
			try {
				CustomDrawObjectEventArgs args = new CustomDrawObjectEventArgs(this, this.Bounds, defaultDrawDelegate);
				customDrawProvider.RaiseCustomDrawTimeCell(args);
				return args.Handled;
			} finally {
				this.Cache = null;
			}
		}
		protected internal virtual void CalculateFinalAppearance(BaseViewAppearance appearance, SchedulerColorSchema colorSchema) {
			Appearance.BackColor = CalculateAppearanceBackColor(colorSchema);
			Appearance.BorderColor = CalculateAppearanceBorderColor(colorSchema);
			SelectionAppearance.Combine(appearance.Selection);
		}
		protected internal virtual Color CalculateAppearanceBackColor(SchedulerColorSchema colorSchema) {
			return colorSchema.CellLight;
		}
		protected internal virtual Color CalculateAppearanceBorderColor(SchedulerColorSchema colorSchema) {
			return colorSchema.CellBorder;
		}
	}
	#region TimeCell
	public class TimeCell : SchedulerViewCellBase {
		bool isWorkTime;
		bool endOfHour;
		public TimeCell() {
		}
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("TimeCellIsWorkTime")]
#endif
		public bool IsWorkTime { get { return isWorkTime; } set { isWorkTime = value; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("TimeCellEndOfHour")]
#endif
		public bool EndOfHour { get { return endOfHour; } set { endOfHour = value; } }
		protected internal override Color CalculateAppearanceBackColor(SchedulerColorSchema colorSchema) {
			return IsWorkTime ? colorSchema.CellLight : colorSchema.Cell;
		}
		protected internal override Color CalculateAppearanceBorderColor(SchedulerColorSchema colorSchema) {
			if (IsWorkTime) {
				return EndOfHour ? colorSchema.CellLightBorderDark : colorSchema.CellLightBorder;
			} else {
				return EndOfHour ? colorSchema.CellBorderDark : colorSchema.CellBorder;
			}
		}
	}
	#endregion
	#region SchedulerViewCellBaseStartDateComparer
	public class SchedulerViewCellBaseStartDateComparer : TimeCellStartDateComparerCore {
	}
	#endregion
	#region SchedulerViewCellBaseEndDateComparer
	public class SchedulerViewCellBaseEndDateComparer : TimeCellEndDateComparerCore {
	}
	#endregion
	public class SchedulerViewCellBaseCollection : SchedulerViewCellBaseCollectionCore<SchedulerViewCellBase>, ISchedulerObjectAnchorCollection {
		public override ITimeIntervalCollection CreateEmptyCollection() {
			return new SchedulerViewCellBaseCollection();
		}
		#region ISchedulerObjectAnchorCollection Members
		ISchedulerObjectAnchor ISchedulerObjectAnchorCollection.this[int index] {
			get { return this[index]; }
		}
		#endregion
	}
}
