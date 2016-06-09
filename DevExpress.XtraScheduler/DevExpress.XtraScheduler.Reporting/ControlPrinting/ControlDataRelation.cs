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
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
using System.Collections;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Reporting.Native {
	public interface ISupportDataIterationPriority {
		SchedulerDataIterationPriority IterationPriority { get; }
		ISchedulerResourceIterator GetResourceIterator();
		ISchedulerDateIterator GetDateIterator();
	}
	public interface ISchedulerResourceIterator {
		ResourceDataCache GetResourceDataCache();
	}
	public interface ISchedulerDateIterator {
		TimeIntervalDataCache GetTimeIntervalDataCache();
		int VisibleIntervalColumnCount { get; }
		ColumnArrangementMode ColumnArrangement { get; }
	}
	#region ControlDataRelation
	public class ControlDataRelation {
		PrintDataCache primaryDataCache;
		PrintDataCache secondaryDataCache;
		ViewControlCollection relatedControls;
		int visibleIntervalColumnCount = ReportViewBase.DefaultVisibleIntervalColumnCount;
		ColumnArrangementMode columnArrangment = ReportViewBase.DefaultColumnArrangement;
		public ControlDataRelation(PrintDataCache primaryDataCache, PrintDataCache secondaryDataCache, ViewControlCollection relatedControls) {
			if (primaryDataCache == null)
				Exceptions.ThrowArgumentNullException("primaryCache");
			if (secondaryDataCache == null)
				Exceptions.ThrowArgumentNullException("secondaryCache");
			this.primaryDataCache = primaryDataCache;
			this.secondaryDataCache = secondaryDataCache;
			this.relatedControls = new ViewControlCollection();
			this.relatedControls.AddRange(relatedControls);
		}
		public PrintDataCache PrimaryDataCache { get { return primaryDataCache; } }
		public PrintDataCache SecondaryDataCache { get { return secondaryDataCache; } }
		public ViewControlCollection RelatedControls { get { return relatedControls; } }
		public int VisibleIntervalColumnCount { get { return visibleIntervalColumnCount; } set { visibleIntervalColumnCount = value; } }
		public ColumnArrangementMode ColumnArrangement { get { return columnArrangment; } set { columnArrangment = value; } }
		public bool IsPrintingComplete() {
			return PrimaryDataCache.IsPrintingComplete() && SecondaryDataCache.IsPrintingComplete();
		}
		public virtual void UpdateGroupStateForAll(ControlPrintState state) {
			int count = RelatedControls.Count;
			for (int i = 0; i < count; i++) {
				RelatedControls[i].PrintController.SetPrintState(state);
			}
		}
		protected internal virtual TimeIntervalDataCache GetIntervalDataCache() {
			TimeIntervalDataCache result = PrimaryDataCache as TimeIntervalDataCache;
			if (result != null)
				return result;
			return SecondaryDataCache as TimeIntervalDataCache;
		}
		protected internal virtual ResourceDataCache GetResourceDataCache() {
			ResourceDataCache result = PrimaryDataCache as ResourceDataCache;
			if (result != null)
				return result;
			return SecondaryDataCache as ResourceDataCache;
		}
		public virtual void UpdateIteratorControlsState(ControlPrintState resourceState, ControlPrintState intervalState) {
			ResourceDataCache resourceCache = GetResourceDataCache();
			if (resourceCache != null)
				resourceCache.SetPrintState(resourceState);
			TimeIntervalDataCache intervalCache = GetIntervalDataCache();
			if (intervalCache != null)
				intervalCache.SetPrintState(intervalState);
		}
		public virtual void UpdateNotIteratorControlsState(ControlPrintState state) {
			int count = RelatedControls.Count;
			for (int i = 0; i < count; i++) {
				ReportViewControlBase control = RelatedControls[i];
				if (ShouldUpdateState(control))
					control.PrintController.SetPrintState(state);
			}
		}
		protected internal virtual bool ShouldUpdateState(ReportViewControlBase control) {
			ISchedulerResourceIterator resIterator = control as ISchedulerResourceIterator;
			if (resIterator != null) {
				ResourceDataCache resourceCache = resIterator.GetResourceDataCache();
				if (resourceCache == primaryDataCache || resourceCache == secondaryDataCache)
					return false;
			}
			ISchedulerDateIterator dateIterator = control as ISchedulerDateIterator;
			if (dateIterator != null) {
				TimeIntervalDataCache dateCache = dateIterator.GetTimeIntervalDataCache();
				if (dateCache == primaryDataCache || dateCache == secondaryDataCache)
					return false;
			}
			XtraSchedulerDebug.Assert(control.PrintController != null);
			return true;
		}
	}
	#endregion
	#region ControlDataRelationsCollection
	public class ControlDataRelationsCollection : DXCollection<ControlDataRelation> {
	}
	#endregion
}
