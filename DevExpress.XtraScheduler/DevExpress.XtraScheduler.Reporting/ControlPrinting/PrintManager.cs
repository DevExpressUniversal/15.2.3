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
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Reporting.Native {
	public enum ControlPrintState { None, Reset, Print, KeepCurrent, EndOfData };
	#region ReportPrintManager
	public class ReportPrintManager {
		XtraSchedulerReport report;
		ControlDataRelationsCollection controlRelations;
		ViewControlCollection emptyDataControls;
		ControlPrintStateCalculator printStateCalculator;
		int printDetailIndex = -1;
		TimeIntervalDataCache smartSyncIntervalCache;
		ResourceDataCache smartSyncResourceCache;
		public ReportPrintManager(XtraSchedulerReport report) {
			if (report == null)
				Exceptions.ThrowArgumentNullException("report");
			this.report = report;
			this.controlRelations = new ControlDataRelationsCollection();
			this.emptyDataControls = new ViewControlCollection();
			this.printStateCalculator = new ControlPrintStateCalculator();
			Initialize();
		}
		protected internal XtraSchedulerReport Report { get { return report; } }
		protected internal int PrintDetailIndex { get { return printDetailIndex; } }
		protected internal ControlPrintStateCalculator PrintStateCalculator { get { return printStateCalculator; } }
		protected internal ControlDataRelationsCollection ControlRelations { get { return controlRelations; } }
		protected internal ViewControlCollection EmptyDataControls { get { return emptyDataControls; } }
		protected internal TimeIntervalDataCache SmartSyncIntervalCache { get { return smartSyncIntervalCache; } }
		protected internal ResourceDataCache SmartSyncResourceCache { get { return smartSyncResourceCache; } }
		protected bool IsSmartSyncPrinting { get { return AllowSmartSync && SmartSyncIntervalCache != null; } }
		protected bool AllowSmartSync { get { return report.ActualSchedulerAdapter.EnableSmartSync; } }
		protected virtual void Initialize() {
			SubscribeReportEvents();
		}
		protected virtual void SubscribeReportEvents() {
		}
		protected virtual void UnsubscribeReportEvents() {
		}
		public virtual bool ShouldCreateDetail(int index) {
			this.printDetailIndex = index;
			if (ShouldCreateControlRelations(index)) {
				CalculateControlRelations();
				ClearSmartSync();
			}			
			SynchronizeMasterControlsProperties();
			if (ShouldUpdateResourceColorSchemasCache(index))
				UpdateResourceColorSchemasCache();
			if (!Report.IsDesignMode) {
				if (CanInitSmartSync())
					InitSmartSync(); 
				UpdateRelatedControlState();
				if (IsSmartSyncPrinting)
					SynchronizeRelations();
				UpdateEmptyDataControls();
			}
			return index == 0 || !IsPrintingComplete();
		}
		private void SynchronizeMasterControlsProperties() {
			ViewControlCollection controls = GetBandViewControls();
			int count = controls.Count;
			for (int i = 0; i < count; i++) 
				controls[i].SynchronizeMasterControlsProperties();			
		}
		protected internal virtual bool CanInitSmartSync() {
			return PrintDetailIndex > 0 && AllowSmartSync && SmartSyncIntervalCache == null;
		}
		protected internal virtual void ClearSmartSync() {
			smartSyncIntervalCache = null;
			smartSyncResourceCache = null;
		}
		protected internal virtual void InitSmartSync() {
			int count = ControlRelations.Count;
			for (int i = 0; i < count; i++) {
				ControlDataRelation relation = ControlRelations[i];
				TimeIntervalDataCache currentCache = relation.GetIntervalDataCache();
				if (currentCache == null)
					continue;
				if (smartSyncIntervalCache != null) {
					TimeInterval triFoldMasterInterval = smartSyncIntervalCache.PrintTimeIntervals.Interval;
					if (triFoldMasterInterval.Contains(currentCache.PrintTimeIntervals.Interval)) { 
						this.smartSyncIntervalCache = currentCache;
						this.smartSyncResourceCache = relation.GetResourceDataCache();
					}
				} else {
					this.smartSyncIntervalCache = currentCache;
					this.smartSyncResourceCache = relation.GetResourceDataCache();
				}
			}
		}
		protected internal virtual void ProcessSmartSyncRelations(ControlDataRelation relation) {
			XtraSchedulerDebug.Assert(smartSyncIntervalCache != null);
			TimeIntervalDataCache relatedCache = relation.GetIntervalDataCache();
			if (SmartSyncIntervalCache.Equals(relatedCache))
				return;
			ControlPrintState rState = SmartSyncResourceCache.PrintState;
			ControlPrintState iState = SmartSyncIntervalCache.PrintState;
			if (relatedCache == null) {
				relation.UpdateIteratorControlsState(rState, iState);
				return;
			}
			if (rState == ControlPrintState.EndOfData && iState == ControlPrintState.EndOfData) {
				relation.UpdateGroupStateForAll(ControlPrintState.EndOfData);
				return;
			}
			if (iState == ControlPrintState.Print) {
				TimeInterval currentRelatedInterval = relatedCache.GetSmartSyncTimeInterval();
				TimeInterval nextMasterInterval = smartSyncIntervalCache.GetNextTimeInterval();
				ControlPrintState intervalState = CalcSyncRelatedIntervalPrintState(currentRelatedInterval, nextMasterInterval.Start);
				XtraSchedulerDebug.Assert(rState == ControlPrintState.Reset || rState == ControlPrintState.Print || rState == ControlPrintState.KeepCurrent);
				relation.UpdateIteratorControlsState(rState, intervalState);
				relation.UpdateNotIteratorControlsState(ControlPrintState.KeepCurrent);
				return;
			}
			if (rState == ControlPrintState.Print) {
				XtraSchedulerDebug.Assert(iState == ControlPrintState.Reset || iState == ControlPrintState.KeepCurrent);
				relation.UpdateIteratorControlsState(rState, iState);
				relation.UpdateNotIteratorControlsState(ControlPrintState.KeepCurrent);
			}
		}
		protected internal virtual ControlPrintState CalcSyncRelatedIntervalPrintState(TimeInterval currentRelatedInterval, DateTime nextMasterStart) {
			if (currentRelatedInterval.IntersectsWithExcludingBounds(new TimeInterval(nextMasterStart, TimeSpan.Zero))) {
				if (CanFreezeSecondaryRelationPrint())
					return ControlPrintState.KeepCurrent;
			}
			return ControlPrintState.Print;
		}
		protected virtual bool CanFreezeSecondaryRelationPrint() {
			return !SmartSyncIntervalCache.IsEndOfData();
		}
		protected virtual void UpdateEmptyDataControls() {
			int count = EmptyDataControls.Count;
			for (int i = 0; i < count; i++) {
				ControlPrintControllerBase controller = EmptyDataControls[i].PrintController;
				XtraSchedulerDebug.Assert(controller != null);
				PrintDataCache dataCache = controller.DataCache;
				ControlPrintState state = dataCache.PrintState == ControlPrintState.None ? ControlPrintState.KeepCurrent : ControlPrintState.EndOfData;
				dataCache.SetPrintState(state);
			}
		}
		protected internal virtual bool ShouldCreateControlRelations(int printIndex) {
			return !Report.IsDesignMode && printIndex == 0;
		}
		protected internal virtual void UpdateResourceColorSchemasCache() {
			Report.UpdateResourceColorSchemasCache();
		}
		protected internal virtual bool ShouldUpdateResourceColorSchemasCache(int printIndex) {
			return printIndex == 0;
		}
		protected internal virtual void CalculateControlRelations() {
			ControlRelations.Clear();
			EmptyDataControls.Clear();
			ViewControlCollection controls = GetBandViewControls();
			ControlDataRelationCalculator calculator = new ControlDataRelationCalculator();
			DataRelationCalculatorResult result = calculator.Calculate(controls);
			ControlRelations.AddRange(result.Relations);
			EmptyDataControls.AddRange(result.EmptyDataControls);
		}
		protected internal virtual ViewControlCollection GetBandViewControls() {
			XRControlCollection bandControls = GetBandControls(typeof(DetailBand));
			ViewControlCollection result = new ViewControlCollection();
			int count = bandControls.Count;
			for (int i = 0; i < count; i++) {
				ReportViewControlBase control = bandControls[i] as ReportViewControlBase;
				if (control != null)
					result.Add(control);
			}
			return result;
		}
		protected virtual void UpdateRelatedControlState() {
			int count = ControlRelations.Count;
			for (int i = 0; i < count; i++) {
				ControlDataRelation relation = ControlRelations[i];
				if (PrintDetailIndex == 0) {
					InitializeCacheColumnData(relation.PrimaryDataCache, relation, true);
					InitializeCacheColumnData(relation.SecondaryDataCache, relation, true);
				}
				PrintStateCalculator.UpdatePrintStates(relation);
			}
		}
		protected virtual void SynchronizeRelations() {
			int count = ControlRelations.Count;
			for (int i = 0; i < count; i++)
				ProcessSmartSyncRelations(ControlRelations[i]);
		}
		protected virtual void InitializeCacheColumnData(PrintDataCache dataCache, ControlDataRelation relation, bool val) {
			dataCache.VisibleIntervalColumnCount = relation.VisibleIntervalColumnCount;
			dataCache.PageBreakBeforeNextColumn = val;
			dataCache.ColumnArrangement = relation.ColumnArrangement;			
		}
		protected bool IsPrintingComplete() {
			int count = ControlRelations.Count;
			for (int i = 0; i < count; i++) {
				if (!ControlRelations[i].IsPrintingComplete())
					return false;
			}
			return true;
		}
		protected virtual XRControlCollection GetBandControls(Type bandType) {
			Band band = Report.Bands.GetBandByType(bandType);
			return band != null ? band.Controls : new XRControlCollection(null);
		}
		protected virtual bool IsControlPrintingComplete(XRControl xrControl) {
			ReportViewControlBase control = xrControl as ReportViewControlBase;
			if (control == null)
				return true;
			return control.IsPrintingComplete();
		}
		protected internal int GetPrintColumnIndex(ReportViewControlBase control) {
			int count = ControlRelations.Count;
			for (int i = 0; i < count; i++) {
				ControlDataRelation relation = ControlRelations[i];
				if (relation.RelatedControls.Contains(control))
					return relation.PrimaryDataCache.ColumnIndex;
			}
			return -1;
		}
	}
	#endregion
	#region ControlPrintStateCalculator
	public class ControlPrintStateCalculator {
		ControlDataRelation controlRelation;
		public ControlPrintStateCalculator() {
		}
		#region Properties
		protected internal ControlDataRelation ControlRelation { get { return controlRelation; } }
		protected PrintDataCache PrimaryDataCache { get { return ControlRelation.PrimaryDataCache; } }
		protected PrintDataCache SecondaryDataCache { get { return ControlRelation.SecondaryDataCache; } }
		#endregion
		public void UpdatePrintStates(ControlDataRelation controlRelation) {
			Initialize(controlRelation);
			UpdatePrintStatesCore();
		}
		protected virtual void Initialize(ControlDataRelation controlRelation) {
			if (controlRelation == null)
				Exceptions.ThrowArgumentNullException("controlRelation");
			this.controlRelation = controlRelation;
		}
		protected virtual void UpdatePrintStatesCore() {
			ControlPrintState primaryState = GetValidPrintState(PrimaryDataCache);
			ControlPrintState secondaryState = GetValidPrintState(SecondaryDataCache);
			if (primaryState == ControlPrintState.Reset)
				PrimaryDataCache.SetPrintState(ControlPrintState.Reset);
			if (secondaryState == ControlPrintState.Reset)
				SecondaryDataCache.SetPrintState(ControlPrintState.Reset);
			if (primaryState == ControlPrintState.Reset) {
				ControlRelation.UpdateNotIteratorControlsState(ControlPrintState.KeepCurrent);
				return;
			}
			if (primaryState == ControlPrintState.Print) {
				PrimaryDataCache.SetPrintState(primaryState);
				SecondaryDataCache.SetPrintState(ControlPrintState.KeepCurrent);
				return;
			}
			if (primaryState == ControlPrintState.EndOfData) {
				if (secondaryState == ControlPrintState.EndOfData) {
					FinishPrinting();
				} else {
					PrimaryDataCache.SetPrintState(ControlPrintState.Reset);
					SecondaryDataCache.SetPrintState(ControlPrintState.Print);
				}
			}
		}
		protected virtual void FinishPrinting() {
			PrimaryDataCache.SetPrintState(ControlPrintState.EndOfData);
			SecondaryDataCache.SetPrintState(ControlPrintState.EndOfData);
			ControlRelation.UpdateNotIteratorControlsState(ControlPrintState.EndOfData);
		}
		protected internal virtual ControlPrintState GetValidPrintState(PrintDataCache dataCache) {
			ControlPrintState state = dataCache.PrintState;
			if (state == ControlPrintState.None) {
				return ControlPrintState.Reset;
			}
			if (state == ControlPrintState.Reset) {
				state = ControlPrintState.Print;
			}
			if (dataCache.IsEndOfData())
				state = ControlPrintState.EndOfData;
			return state;
		}
	}
	#endregion
}
