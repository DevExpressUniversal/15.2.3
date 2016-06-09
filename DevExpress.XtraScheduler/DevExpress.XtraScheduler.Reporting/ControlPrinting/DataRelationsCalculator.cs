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
using DevExpress.Utils;
using DevExpress.XtraReports.UI;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Reporting.Native {
	#region ViewControlCollection
	public class ViewControlCollection : List<ReportViewControlBase> {
	}
	#endregion
	#region ViewControlCollectionsList
	public class ViewControlCollectionsList : List<ViewControlCollection> {
	}
	#endregion
	#region DataRelationCalculatorResult
	public class DataRelationCalculatorResult {
		ControlDataRelationsCollection relations;
		ViewControlCollection emptyDataControls;
		public DataRelationCalculatorResult() {
			relations = new ControlDataRelationsCollection();
			emptyDataControls = new ViewControlCollection();
		}
		public DataRelationCalculatorResult(ControlDataRelationsCollection relations, ViewControlCollection emptyDataControls) {
			if (relations == null)
				Exceptions.ThrowArgumentNullException("relations");
			if (emptyDataControls == null)
				Exceptions.ThrowArgumentNullException("emptyDataControls");
			this.relations = new ControlDataRelationsCollection();
			this.emptyDataControls = new ViewControlCollection();
			this.relations.AddRange(relations);
			this.emptyDataControls.AddRange(emptyDataControls);
		}
		public ControlDataRelationsCollection Relations { get { return relations; } }
		public ViewControlCollection EmptyDataControls { get { return emptyDataControls; } }
		protected internal virtual void Merge(DataRelationCalculatorResult result) {
			EmptyDataControls.AddRange(result.EmptyDataControls);
			Relations.AddRange(result.Relations);
		}
	}
	#endregion
	#region ControlDataRelationCalculator
	public class ControlDataRelationCalculator {
		protected internal virtual DataRelationCalculatorResult Calculate(ViewControlCollection controls) {
			DataRelationCalculatorResult result = new DataRelationCalculatorResult();
			ViewControlCollectionsList relatedControlGroups = CalculateRelatedGroups(controls);
			int count = relatedControlGroups.Count;
			for (int i = 0; i < count; i++)
				result.Merge(ProcessRelatedGroup(relatedControlGroups[i]));
			return result;
		}
		protected internal virtual DataRelationCalculatorResult ProcessRelatedGroup(ViewControlCollection relatedControls) {
			ControlDataRelationsCollection relations = CalculateControlRelations(relatedControls);
			ViewControlCollection emptyDataControls = CalculateSingleEmptyDataControls(relatedControls);
			return new DataRelationCalculatorResult(relations, emptyDataControls);
		}
		protected internal virtual ViewControlCollection CalculateSingleEmptyDataControls(ViewControlCollection relatedControls) {
			int count = relatedControls.Count;
			for (int i = 0; i < count; i++) {
				if (relatedControls[i] is ISchedulerResourceIterator)
					return new ViewControlCollection();
				if (relatedControls[i] is ISchedulerDateIterator)
					return new ViewControlCollection();
			}
			return relatedControls;
		}
		protected internal virtual ViewControlCollectionsList CalculateRelatedGroups(ViewControlCollection controls) {
			RelatedControlsGroupCalculator relatedGroupsCalculator = new RelatedControlsGroupCalculator();
			ViewControlCollectionsList relatedControlGroups = relatedGroupsCalculator.CalculateGroups(controls);
			return relatedControlGroups;
		}
		protected internal virtual ControlDataRelationsCollection CalculateControlRelations(ViewControlCollection relatedControls) {
			ViewControlCollection sortedControls = SortControls(relatedControls);
			ISupportDataIterationPriority dataPriorityControl = FindSupportIterationPriorityControl(sortedControls);
			ControlDataRelationsCollection result = new ControlDataRelationsCollection();
			ControlDataRelation relation = CreateControlRelation(dataPriorityControl, sortedControls);
			if (relation != null) {
				result.Add(relation);
			}
			return result;
		}
		protected internal virtual ViewControlCollection SortControls(ViewControlCollection relatedControls) {
			IList<ReportViewControlBase> sortedControls = Algorithms.TopologicalSort<ReportViewControlBase>(relatedControls, new ReportViewControlComparer());
			ViewControlCollection result = new ViewControlCollection();
			result.AddRange(sortedControls);
			return result;
		}
		protected internal virtual ISupportDataIterationPriority FindSupportIterationPriorityControl(ViewControlCollection controls) {
			int count = controls.Count;
			for (int i = 0; i < count; i++) {
				ISupportDataIterationPriority control = controls[i] as ISupportDataIterationPriority;
				if (control != null)
					return control;
			}
			return null;
		}
		protected internal virtual ControlDataRelation CreateControlRelation(ISupportDataIterationPriority iterationPriorityControl, ViewControlCollection sortedControls) {
			ControlDataRelation result;
			ISchedulerDateIterator dateIterator;
			if (iterationPriorityControl != null) {
				dateIterator = iterationPriorityControl.GetDateIterator();
				ISchedulerResourceIterator resourceIterator = iterationPriorityControl.GetResourceIterator();
				result = CreateIterationPriorityControlRelation(sortedControls, resourceIterator, dateIterator, iterationPriorityControl.IterationPriority);
			}
			else {
				int count = sortedControls.Count;
				XtraSchedulerDebug.Assert(count > 0);
				ReportViewControlBase control = sortedControls[count - 1];
				dateIterator = control.GetDateIterator();
				ISchedulerResourceIterator resourceIterator = control.GetResourceIterator();
				result = CreateDefaultRelation(sortedControls, resourceIterator, dateIterator);
			}
			InitializeRelationColumn(result, dateIterator);
			return result;
		}
		protected internal virtual ControlDataRelation CreateDefaultRelation(ViewControlCollection relatedControls, ISchedulerResourceIterator resourceIterator, ISchedulerDateIterator dateIterator) {
			return CreateRelationCore(GetResourceDataCache(resourceIterator), GetTimeIntervalDataCache(dateIterator), relatedControls);
		}
		protected virtual void InitializeRelationColumn(ControlDataRelation relation, ISchedulerDateIterator dateIterator) {
			if (relation != null && dateIterator != null) {
				relation.VisibleIntervalColumnCount = dateIterator.VisibleIntervalColumnCount;
				relation.ColumnArrangement = dateIterator.ColumnArrangement;
			}
		}
		protected PrintDataCache GetTimeIntervalDataCache(ISchedulerDateIterator iterator) {
			return iterator != null ? iterator.GetTimeIntervalDataCache() : null;
		}
		protected PrintDataCache GetResourceDataCache(ISchedulerResourceIterator iterator) {
			return iterator != null ? iterator.GetResourceDataCache() : null;
		}
		protected internal virtual ControlDataRelation CreateIterationPriorityControlRelation(ViewControlCollection relatedControls, ISchedulerResourceIterator resourceIterator, ISchedulerDateIterator dateIterator, SchedulerDataIterationPriority iterationPriority) {
			TimeIntervalDataCache intervalCache = dateIterator.GetTimeIntervalDataCache();
			ResourceDataCache resourceCache = resourceIterator.GetResourceDataCache();
			if (iterationPriority == SchedulerDataIterationPriority.Date)
				return CreateRelationCore(intervalCache, resourceCache, relatedControls);
			else
				return CreateRelationCore(resourceCache, intervalCache, relatedControls);
		}
		protected internal virtual ControlDataRelation CreateRelationCore(PrintDataCache primaryCache, PrintDataCache secondaryCache, ViewControlCollection relatedControls) {
			if (primaryCache == null)
				return CreateRelationInstance(secondaryCache, EmptyPrintDataCache.Instance, relatedControls);
			if (secondaryCache == null)
				return CreateRelationInstance(primaryCache, EmptyPrintDataCache.Instance, relatedControls);
			return CreateRelationInstance(primaryCache, secondaryCache, relatedControls);
		}
		protected internal virtual ControlDataRelation CreateRelationInstance(PrintDataCache primaryCache, PrintDataCache secondaryCache, ViewControlCollection relatedControls) {
			return primaryCache != null ? new ControlDataRelation(primaryCache, secondaryCache, relatedControls) : null;
		}
	}
	#endregion
	#region ControlGroupDataRelationCalculator
	public class RelatedControlsGroupCalculator {
		ViewControlCollectionsList groups;
		public RelatedControlsGroupCalculator() {
			groups = new ViewControlCollectionsList();
		}
		public ViewControlCollectionsList Groups { get { return groups; } }
		protected internal virtual ViewControlCollectionsList CalculateGroups(ViewControlCollection controls) {
			Groups.Clear();
			int count = controls.Count;
			for (int i = 0; i < count; i++)
				ProcessControl(controls[i]);
			return EnsureControlsView(Groups);
		}
		protected internal virtual ViewControlCollectionsList EnsureControlsView(ViewControlCollectionsList controlGroups) {
			ViewControlCollectionsList result = new ViewControlCollectionsList();
			int count = Groups.Count;
			for (int i = 0; i < count; i++) {
				ViewControlCollection group = controlGroups[i];
				if (EnsureRelatedControlsView(group))
					result.Add(group);
			}
			return result;
		}
		protected internal bool EnsureRelatedControlsView(ViewControlCollection relatedControls) {
			int count = relatedControls.Count;
			for (int i = 0; i < count; i++) {
				ReportViewControlBase control = relatedControls[i];
				if (control.View == null)
					return false;
			}
			return true;
		}
		protected internal virtual void ProcessControl(ReportViewControlBase control) {
			if (control.View != null) {
				control.DropPrintController();
				control.EnsurePrintController();
			}
			ViewControlCollection group = FindGroup(control);
			if (group == null)
				group = CreateGroup(control);
			MergeMasterControlsGroup(control, group);
		}
		protected internal virtual ViewControlCollection CreateGroup(ReportViewControlBase control) {
			ViewControlCollection group = new ViewControlCollection();
			group.Add(control);
			Groups.Add(group);
			return group;
		}
		protected internal virtual void MergeMasterControlsGroup(ReportViewControlBase control, ViewControlCollection controlGroup) {
			ReportRelatedControlBase relatedControl = control as ReportRelatedControlBase;
			if (relatedControl == null)
				return;
			ReportViewControlBase horizontalMaster = relatedControl.LayoutOptionsHorizontal.MasterControl;
			if (horizontalMaster != null)
				Merge(horizontalMaster, controlGroup);
			ReportViewControlBase verticalMaster = relatedControl.LayoutOptionsVertical.MasterControl;
			if (verticalMaster != null)
				Merge(verticalMaster, controlGroup);
		}
		protected internal virtual void Merge(ReportViewControlBase control, ViewControlCollection destinationGroup) {
			ViewControlCollection controlGroup = FindGroup(control);
			if (controlGroup == null)
				destinationGroup.Add(control);
			else
				Merge(controlGroup, destinationGroup);
		}
		protected internal virtual void Merge(ViewControlCollection source, ViewControlCollection destination) {
			if (source != destination) {
				destination.AddRange(source);
				bool result = Groups.Remove(source);
				XtraSchedulerDebug.Assert(result);
			}
		}
		protected internal virtual ViewControlCollection FindGroup(ReportViewControlBase control) {
			int count = Groups.Count;
			for (int i = 0; i < count; i++) {
				ViewControlCollection group = Groups[i];
				if (group.Contains(control))
					return group;
			}
			return null;
		}
	}
	#endregion
}
