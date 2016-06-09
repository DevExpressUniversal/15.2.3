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
using System.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Native;
using System.Collections;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.XtraScheduler.Drawing {
	#region DependencyTableType
	public enum DependencyTableType {
		IncomingInStart,
		IncomingInFinish,
		OutcomingFromStart,
		OutcomingFromFinish
	}
	#endregion
	#region DependencyCounter
	internal struct DependencyCounter {
		#region Fields
		int incomingInStart;
		int incomingInFinish;
		int outcomingFromStart;
		int outcomingFromFinish;
		int maxDependenciesCount;
		DependencyTableType type;
		#endregion
		#region Properties
		public int IncomingInStart { get { return incomingInStart; } set { incomingInStart = value; } }
		public int IncomingInFinish { get { return incomingInFinish; } set { incomingInFinish = value; } }
		public int OutcomingFromStart { get { return outcomingFromStart; } set { outcomingFromStart = value; } }
		public int OutcomingFromFinish { get { return outcomingFromFinish; } set { outcomingFromFinish = value; } }
		public int StartCount { get { return incomingInStart + outcomingFromStart; } }
		public int FinishCount { get { return incomingInFinish + outcomingFromFinish; } }
		public int MaxDependenciesCount { get { return maxDependenciesCount; } }
		public DependencyTableType Type { get { return type; } }
		#endregion
		internal void CalculateMaxInfo() {
			bool shouldUseAdditionalRate = false;
			DependencyTableType oldType = type;
			type = DependencyTableType.IncomingInFinish;
			maxDependenciesCount = IncomingInFinish;
			if (IncomingInStart > maxDependenciesCount) {
				maxDependenciesCount = IncomingInStart;
				type = DependencyTableType.IncomingInStart;
			} else if (IncomingInFinish == maxDependenciesCount)
				shouldUseAdditionalRate = true;
			if (OutcomingFromFinish > maxDependenciesCount) {
				maxDependenciesCount = OutcomingFromFinish;
				shouldUseAdditionalRate = false;
				type = DependencyTableType.OutcomingFromFinish;
			} else if (OutcomingFromFinish == maxDependenciesCount)
				shouldUseAdditionalRate = true;
			if (OutcomingFromStart > maxDependenciesCount) {
				maxDependenciesCount = OutcomingFromStart;
				shouldUseAdditionalRate = false;
				type = DependencyTableType.OutcomingFromStart;
			} else if (OutcomingFromStart == maxDependenciesCount)
				shouldUseAdditionalRate = true;
			if (!shouldUseAdditionalRate)
				return;
			if (StartCount > FinishCount)
				if (OutcomingFromStart > IncomingInStart) {
					type = DependencyTableType.OutcomingFromStart;
					return;
				} else {
					type = DependencyTableType.IncomingInStart;
					return;
				}
			if (OutcomingFromFinish == IncomingInFinish) {
				type = oldType;
				return;
			}
			if (OutcomingFromFinish > IncomingInFinish)
				type = DependencyTableType.OutcomingFromFinish;
			else
				type = DependencyTableType.IncomingInFinish;
		}
	}
	#endregion
	#region DependencyCountInfo
	internal class DependencyCountInfo {
		#region Fields
		List<int> indexes;
		int dependencyCount;
		#endregion
		public DependencyCountInfo() {
			this.indexes = new List<int>();
		}
		#region Properties
		public List<int> Indexes { get { return indexes; } set { indexes = value; } }
		public int DependencyCount { get { return dependencyCount; } set { dependencyCount = value; } }
		#endregion
	}
	#endregion
	#region DependencyGroup
	public class DependencyGroup {
		#region Fields
		AppointmentDependencyCollection dependencies;
		GanttAppointmentViewInfoWrapperCollection parentViewInfos;
		GanttAppointmentViewInfoWrapperCollection dependentViewInfos;
		int index;
		#endregion
		public DependencyGroup(int index) {
			this.index = index;
			dependencies = new AppointmentDependencyCollection();
			parentViewInfos = new GanttAppointmentViewInfoWrapperCollection();
			dependentViewInfos = new GanttAppointmentViewInfoWrapperCollection();
		}
		#region Properties
		public AppointmentDependencyCollection Dependencies { get { return dependencies; } }
		public GanttAppointmentViewInfoWrapperCollection ParentViewInfos { get { return parentViewInfos; } }
		public GanttAppointmentViewInfoWrapperCollection DependentViewInfos { get { return dependentViewInfos; } }
		public int Index { get { return index; } }
		#endregion
	}
	#endregion
	#region DependencyGroupCollection
	public class DependencyGroupCollection : DXCollection<DependencyGroup> {
		public DependencyGroup Last {
			get {
				XtraSchedulerDebug.Assert(Count > 0);
				return this[Count - 1];
			}
		}
	}
	#endregion
	#region GanttDependencyGroupingHelper
	public class DependencyGroupingHelper {
		#region DependencyGroupTranslationTable
		internal class DependencyGroupTranslationTable {
			#region Fields
			Dictionary<int, GanttAppointmentViewInfoWrapper> appointmentViewInfos;
			Dictionary<GanttAppointmentViewInfoWrapper, int> appointmentIndexes;
			#endregion
			public DependencyGroupTranslationTable() {
				this.appointmentIndexes = new Dictionary<GanttAppointmentViewInfoWrapper, int>();
				this.appointmentViewInfos = new Dictionary<int, GanttAppointmentViewInfoWrapper>();
			}
			#region Properties
			public Dictionary<int, GanttAppointmentViewInfoWrapper> AppointmentViewInfos { get { return appointmentViewInfos; } }
			public Dictionary<GanttAppointmentViewInfoWrapper, int> AppointmentIndexes { get { return appointmentIndexes; } }
			#endregion
			public void Initialize(GanttAppointmentViewInfoWrapperCollection aptViewInfos) {
				Guard.ArgumentNotNull(aptViewInfos, "aptViewInfos");
				int count = aptViewInfos.Count;
				for (int i = 0; i < count; i++) {
					AppointmentViewInfos.Add(i, aptViewInfos[i]);
					AppointmentIndexes.Add(aptViewInfos[i], i);
				}
			}
			public GanttAppointmentViewInfoWrapper GetViewInfoByIndex(int index) {
				GanttAppointmentViewInfoWrapper viewInfo;
				if (AppointmentViewInfos.TryGetValue(index, out viewInfo))
					return viewInfo;
				return null;
			}
		}
		#endregion
		#region Fields
		const int MinDependencyDistance = 2;
		const int StartGroupIndex = 2;
		int arrowWidthWithIndent;
		int currentGroupIndex;
		int[][] incomingInStartTable;
		int[][] incomingInFinishTable;
		int[][] outcomingFromStartTable;
		int[][] outcomingFromFinishTable;
		DependencyCounter[] counters;
		DependencyGroupTranslationTable translationTable;
		GanttAppointmentViewInfoWrapperCollection ganttAptViewInfos;
		DependencyGroupCollection groups;
		AppointmentDependencyCollection dependencies;
		#endregion
		public DependencyGroupingHelper(int arrowWidth, AppointmentDependencyCollection dependencies) {
			this.translationTable = new DependencyGroupTranslationTable();
			this.groups = new DependencyGroupCollection();
			this.currentGroupIndex = StartGroupIndex;
			this.arrowWidthWithIndent = arrowWidth + 2;
			this.dependencies = dependencies;
		}
		#region Properties
		protected internal int[][] IncomingInStartTable { get { return incomingInStartTable; } }
		protected internal int[][] IncomingInFinishTable { get { return incomingInFinishTable; } }
		protected internal int[][] OutcomingFromStartTable { get { return outcomingFromStartTable; } }
		protected internal int[][] OutcomingFromFinishTable { get { return outcomingFromFinishTable; } }
		internal DependencyCounter[] Counters { get { return counters; } }
		internal GanttAppointmentViewInfoWrapperCollection GanttAppointmentViewInfos { get { return ganttAptViewInfos; } }
		internal DependencyGroupTranslationTable TranslationTable { get { return translationTable; } }
		internal AppointmentDependencyCollection Dependencies { get { return dependencies; } }
		public DependencyGroupCollection Groups { get { return groups; } }
		#endregion
		public virtual void Calculate(GanttAppointmentViewInfoCollection aptViewInfos, Rectangle visibleBounds) {
			CalculateDependencyGroups(aptViewInfos);
			CalculateConnectionPoints(visibleBounds);
		}
		public DependencyTableType GetViewInfoTableTypeByGroupIndex(int groupIndex, GanttAppointmentViewInfoWrapper viewInfo, bool isParent) {
			int viewInfoIndex = TranslationTable.AppointmentIndexes[viewInfo];
			int[] row = (isParent) ? OutcomingFromFinishTable[viewInfoIndex] : IncomingInFinishTable[viewInfoIndex];
			int count = row.Length;
			for (int i = 0; i < count; i++) {
				if (row[i] == groupIndex)
					return (isParent) ? DependencyTableType.OutcomingFromFinish : DependencyTableType.IncomingInFinish;
			}
			return (isParent) ? DependencyTableType.OutcomingFromStart : DependencyTableType.IncomingInStart;
		}
		public void GroupAppointments() {
			int index = GetAptIndexWithMaxInOutValency();
			while (index >= 0) {
				JoinAppointmentsInGroup(index);
				index = GetAptIndexWithMaxInOutValency();
			}
			UpdateCountersAfterGroupingFinished();
		}
		public virtual void CalculateConnectionPoints(Rectangle visibleBounds) {
			int count = GanttAppointmentViewInfos.Count;
			for (int i = 0; i < count; i++) {
				GanttAppointmentViewInfoWrapper info = GanttAppointmentViewInfos[i];
				int counterIndex = TranslationTable.AppointmentIndexes[info];
				CalculateConnectionPointsSingleViewInfo(Counters[counterIndex], info, visibleBounds);
			}
		}
		protected internal virtual void CalculateDependencyGroups(GanttAppointmentViewInfoCollection aptViewInfos) {
			ClearAll();
			CalculateGroupingTables(aptViewInfos);
		}
		protected internal void ClearAll() {
			this.translationTable = new DependencyGroupTranslationTable();
			this.groups = new DependencyGroupCollection();
			this.currentGroupIndex = StartGroupIndex;
		}
		protected internal virtual void CalculateGroupingTables(GanttAppointmentViewInfoCollection aptViewInfos) {
			PopulateDependencyTables(aptViewInfos);
			GroupAppointments();
		}
		protected internal virtual void PopulateDependencyTables(GanttAppointmentViewInfoCollection aptViewInfos) {
			this.ganttAptViewInfos = CreateWrappersForNormalAppointments(aptViewInfos);
			InitializeDependencyTables();
			IndexAppointments();
			ProcessAppointments();
		}
		protected internal virtual int GetMaxLocationsCount(int height, int minDistance) {
			return (height + minDistance) / minDistance;
		}
		GanttAppointmentViewInfoWrapperCollection CreateWrappersForNormalAppointments(GanttAppointmentViewInfoCollection source) {
			GanttAppointmentViewInfoWrapperCollection result = new GanttAppointmentViewInfoWrapperCollection();
			int count = source.Count;
			for (int i = 0; i < count; i++) {
				if (source[i].Appointment.Type == AppointmentType.Normal)
					result.Add(new GanttAppointmentViewInfoWrapper(source[i]));
			}
			return result;
		}
		void InitializeDependencyTables() {
			int count = GanttAppointmentViewInfos.Count;
			counters = new DependencyCounter[count];
			this.incomingInStartTable = new int[count][];
			this.incomingInFinishTable = new int[count][];
			this.outcomingFromStartTable = new int[count][];
			this.outcomingFromFinishTable = new int[count][];
			InitializeDependencyTableCore(incomingInStartTable, count);
			InitializeDependencyTableCore(incomingInFinishTable, count);
			InitializeDependencyTableCore(outcomingFromStartTable, count);
			InitializeDependencyTableCore(outcomingFromFinishTable, count);
		}
		void InitializeDependencyTableCore(int[][] targetTable, int count) {
			for (int i = 0; i < count; i++) {
				targetTable[i] = new int[count];
			}
		}
		void IndexAppointments() {
			TranslationTable.Initialize(GanttAppointmentViewInfos);
		}
		void ProcessAppointments() {
			int count = GanttAppointmentViewInfos.Count;
			for (int aptIndex = 0; aptIndex < count; aptIndex++) {
				object currentAptId = GanttAppointmentViewInfos[aptIndex].Id;
				DXCollection<AppointmentDependency> aptDependencies = Dependencies.GetDependenciesByParentId(currentAptId);
				ProcessAppointmentDependencies(aptIndex, aptDependencies);
			}
		}
		void ProcessAppointmentDependencies(int aptIndex, DXCollection<AppointmentDependency> aptDependencies) {
			int dependenciesCount = aptDependencies.Count;
			for (int dependencyIndex = 0; dependencyIndex < dependenciesCount; dependencyIndex++) {
				AppointmentDependency currentDependency = aptDependencies[dependencyIndex];
				DXCollection<GanttAppointmentViewInfoWrapper> dependentViewInfos = GanttAppointmentViewInfos.GetAppointmentViewInfosById(currentDependency.DependentId);
				int viewInfosCount = dependentViewInfos.Count;
				for (int dependentViewInfoIndex = 0; dependentViewInfoIndex < viewInfosCount; dependentViewInfoIndex++) {
					int dependentAptIndex;
					if (TranslationTable.AppointmentIndexes.TryGetValue(dependentViewInfos[dependentViewInfoIndex], out dependentAptIndex))
						UpdateDependencyTable(aptIndex, dependentAptIndex, currentDependency.Type);
				}
			}
			CalculateMaxDependenciesInfo();
		}
		void UpdateDependencyTable(int parentIndex, int dependentIndex, AppointmentDependencyType type) {
			switch (type) {
				case AppointmentDependencyType.FinishToFinish:
					OutcomingFromFinishTable[parentIndex][dependentIndex]++;
					IncomingInFinishTable[dependentIndex][parentIndex]++;
					Counters[parentIndex].OutcomingFromFinish++;
					Counters[dependentIndex].IncomingInFinish++;
					break;
				case AppointmentDependencyType.FinishToStart:
					OutcomingFromFinishTable[parentIndex][dependentIndex]++;
					IncomingInStartTable[dependentIndex][parentIndex]++;
					Counters[parentIndex].OutcomingFromFinish++;
					Counters[dependentIndex].IncomingInStart++;
					break;
				case AppointmentDependencyType.StartToFinish:
					OutcomingFromStartTable[parentIndex][dependentIndex]++;
					IncomingInFinishTable[dependentIndex][parentIndex]++;
					Counters[parentIndex].OutcomingFromStart++;
					Counters[dependentIndex].IncomingInFinish++;
					break;
				case AppointmentDependencyType.StartToStart:
					OutcomingFromStartTable[parentIndex][dependentIndex]++;
					IncomingInStartTable[dependentIndex][parentIndex]++;
					Counters[parentIndex].OutcomingFromStart++;
					Counters[dependentIndex].IncomingInStart++;
					break;
			}
		}
		void CalculateMaxDependenciesInfo() {
			int count = Counters.Length;
			for (int i = 0; i < count; i++) {
				Counters[i].CalculateMaxInfo();
			}
		}
		internal int GetAptIndexWithMaxInOutValency() {
			DependencyCountInfo info = GetMaxDependencyCountInfo();
			if (info.Indexes.Count == 0)
				return -1;
			if (info.Indexes.Count == 1)
				return info.Indexes[0];
			return GetAptIndexWithMaxStartFinishValency(info);
		}
		DependencyCountInfo GetMaxDependencyCountInfo() {
			DependencyCountInfo result = new DependencyCountInfo();
			int count = Counters.Length;
			for (int i = 0; i < count; i++) {
				CheckDependencyCounter(i, result);
			}
			return result;
		}
		void CheckDependencyCounter(int index, DependencyCountInfo info) {
			DependencyCounter current = Counters[index];
			int maxDependenciesCount = current.MaxDependenciesCount;
			if (maxDependenciesCount > info.DependencyCount) {
				info.DependencyCount = maxDependenciesCount;
				info.Indexes = new List<int>();
				info.Indexes.Add(index);
				return;
			}
			int dependencyCount = info.DependencyCount;
			if (maxDependenciesCount == dependencyCount && dependencyCount > 0)
				info.Indexes.Add(index);
		}
		int GetAptIndexWithMaxStartFinishValency(DependencyCountInfo info) {
			int count = info.Indexes.Count;
			XtraSchedulerDebug.Assert(count > 0);
			DependencyCountInfo result = new DependencyCountInfo();
			int maxStartFinishValency = 0;
			result.DependencyCount = Counters[0].MaxDependenciesCount;
			for (int i = 0; i < count; i++) {
				int index = info.Indexes[i];
				int max;
				DependencyCounter counter = Counters[index];
				if (counter.Type == DependencyTableType.IncomingInFinish || counter.Type == DependencyTableType.OutcomingFromFinish)
					max = counter.FinishCount;
				else
					max = counter.StartCount;
				if (max > maxStartFinishValency) {
					maxStartFinishValency = max;
					result.Indexes = new List<int>();
					result.Indexes.Add(index);
				}
			}
			if (result.Indexes.Count == 0)
				return -1;
			return result.Indexes[0];
		}
		void JoinAppointmentsInGroup(int index) {
			XtraSchedulerDebug.Assert(index < Counters.Length);
			DependencyTableType type = Counters[index].Type;
			int[][] table = GetTableByType(type);
			JoinAppointmentsInGroupCore(table, index, type);
			UpdateCountersAfterSingleGroupingIteration();
			this.currentGroupIndex++;
		}
		internal int[][] GetTableByType(DependencyTableType type) {
			switch (type) {
				case DependencyTableType.IncomingInStart:
					return IncomingInStartTable;
				case DependencyTableType.IncomingInFinish:
					return IncomingInFinishTable;
				case DependencyTableType.OutcomingFromStart:
					return OutcomingFromStartTable;
				case DependencyTableType.OutcomingFromFinish:
					return OutcomingFromFinishTable;
			}
			Exceptions.ThrowInternalException();
			return null;
		}
		void JoinAppointmentsInGroupCore(int[][] table, int aptIndex, DependencyTableType type) {
			Groups.Add(new DependencyGroup(this.currentGroupIndex));
			int count = GanttAppointmentViewInfos.Count;
			for (int columnIndex = 0; columnIndex < count; columnIndex++) {
				UpdateTables(table, aptIndex, columnIndex, type);
			}
			for (int rowIndex = 0; rowIndex < count; rowIndex++) {
				if (CompareWithSameTable(table[aptIndex], table[rowIndex]))
					CheckGroupingPattern(table, aptIndex, rowIndex);
			}
		}
		void UpdateTables(int[][] table, int rowIndex, int columnIndex, DependencyTableType type) {
			if (table[rowIndex][columnIndex] != 1)
				return;
			bool isIncoming = (type == DependencyTableType.IncomingInFinish || type == DependencyTableType.IncomingInStart);
			DependencyGroup group = Groups.Last;
			GanttAppointmentViewInfoWrapper parent = (isIncoming) ? TranslationTable.GetViewInfoByIndex(columnIndex) : TranslationTable.GetViewInfoByIndex(rowIndex);
			GanttAppointmentViewInfoWrapper dependent = (isIncoming) ? TranslationTable.GetViewInfoByIndex(rowIndex) : TranslationTable.GetViewInfoByIndex(columnIndex);
#if DEBUG || DEBUGTEST
			XtraSchedulerDebug.Assert(parent != null);
			XtraSchedulerDebug.Assert(dependent != null);
#endif
			group.ParentViewInfos.Add(parent);
			group.DependentViewInfos.Add(dependent);
			AppointmentDependency dependency = Dependencies.GetDependencyByIds(parent.Id, dependent.Id);
			if (!AppointmentDependencyInstance.IsNullOrEmpty(dependency))
				group.Dependencies.Add(dependency);
			table[rowIndex][columnIndex] = this.currentGroupIndex;
			if (isIncoming) {
				UpdateTablesCore(columnIndex, rowIndex, DependencyTableType.OutcomingFromFinish);
				UpdateTablesCore(columnIndex, rowIndex, DependencyTableType.OutcomingFromStart);
			} else {
				UpdateTablesCore(columnIndex, rowIndex, DependencyTableType.IncomingInFinish);
				UpdateTablesCore(columnIndex, rowIndex, DependencyTableType.IncomingInStart);
			}
		}
		void UpdateTablesCore(int rowIndex, int columnIndex, DependencyTableType type) {
			int[][] table = GetTableByType(type);
			if (table[rowIndex][columnIndex] == 1)
				table[rowIndex][columnIndex] = this.currentGroupIndex;
		}
		void CheckGroupingPattern(int[][] table, int aptIndex, int rowIndex) {
			int count = GanttAppointmentViewInfos.Count;
			DependencyTableType type = Counters[aptIndex].Type;
			if (CompareWithCorrespondingTables(aptIndex, rowIndex, type)) {
				for (int columnIndex = 0; columnIndex < count; columnIndex++) {
					UpdateTables(table, rowIndex, columnIndex, type);
				}
			}
		}
		void UpdateCountersAfterSingleGroupingIteration() {
			UpdateCountersAfterSingleGroupingIterationCore(IncomingInFinishTable, DependencyTableType.IncomingInFinish);
			UpdateCountersAfterSingleGroupingIterationCore(IncomingInStartTable, DependencyTableType.IncomingInStart);
			UpdateCountersAfterSingleGroupingIterationCore(OutcomingFromFinishTable, DependencyTableType.OutcomingFromFinish);
			UpdateCountersAfterSingleGroupingIterationCore(OutcomingFromStartTable, DependencyTableType.OutcomingFromStart);
		}
		void UpdateCountersAfterSingleGroupingIterationCore(int[][] table, DependencyTableType type) {
			int count = Counters.Length;
			for (int rowIndex = 0; rowIndex < count; rowIndex++) {
				for (int columnIndex = 0; columnIndex < count; columnIndex++) {
					if (table[rowIndex][columnIndex] == this.currentGroupIndex)
						DecreaseCounter(type, rowIndex);
				}
			}
		}
		void UpdateCountersAfterGroupingFinished() {
			UpdateCountersAfterGroupingFinishedCore(IncomingInFinishTable, DependencyTableType.IncomingInFinish);
			UpdateCountersAfterGroupingFinishedCore(IncomingInStartTable, DependencyTableType.IncomingInStart);
			UpdateCountersAfterGroupingFinishedCore(OutcomingFromFinishTable, DependencyTableType.OutcomingFromFinish);
			UpdateCountersAfterGroupingFinishedCore(OutcomingFromStartTable, DependencyTableType.OutcomingFromStart);
		}
		void UpdateCountersAfterGroupingFinishedCore(int[][] table, DependencyTableType type) {
			int count = Counters.Length;
			List<int> includedGroups;
			for (int rowIndex = 0; rowIndex < count; rowIndex++) {
				includedGroups = new List<int>();
				for (int columnIndex = 0; columnIndex < count; columnIndex++) {
					int groupIndex = table[rowIndex][columnIndex];
					if (groupIndex != 0 && !includedGroups.Contains(groupIndex)) {
						includedGroups.Add(groupIndex);
						IncreaseCounter(type, rowIndex);
					}
				}
			}
		}
		internal void DecreaseCounter(DependencyTableType type, int index) {
			UpdateCounter(type, index, -1);
		}
		void IncreaseCounter(DependencyTableType type, int index) {
			UpdateCounter(type, index, 1);
		}
		void UpdateCounter(DependencyTableType type, int index, int corrective) {
			switch (type) {
				case DependencyTableType.IncomingInStart:
					Counters[index].IncomingInStart += corrective;
					break;
				case DependencyTableType.IncomingInFinish:
					Counters[index].IncomingInFinish += corrective;
					break;
				case DependencyTableType.OutcomingFromStart:
					Counters[index].OutcomingFromStart += corrective;
					break;
				case DependencyTableType.OutcomingFromFinish:
					Counters[index].OutcomingFromFinish += corrective;
					break;
			}
			Counters[index].CalculateMaxInfo();
		}
		bool CompareWithSameTable(int[] pattern, int[] value) {
			XtraSchedulerDebug.Assert(pattern.Length == value.Length);
			int count = pattern.Length;
			for (int i = 0; i < count; i++) {
				if (pattern[i] == 0 && value[i] != 0)
					return false;
				if (pattern[i] == currentGroupIndex && value[i] != 1)
					return false;
			}
			return true;
		}
		bool CompareWithCorrespondingTables(int parentIndex, int dependentIndex, DependencyTableType type) {
			int count = GanttAppointmentViewInfos.Count;
			int[] startValues;
			int[] finishValues;
			int[] startValuesPattern;
			int[] finishValuesPattern;
			if (type == DependencyTableType.IncomingInFinish || type == DependencyTableType.IncomingInStart) {
				startValues = OutcomingFromStartTable[dependentIndex];
				finishValues = OutcomingFromFinishTable[dependentIndex];
				startValuesPattern = OutcomingFromStartTable[parentIndex];
				finishValuesPattern = OutcomingFromFinishTable[parentIndex];
			} else {
				startValues = new int[count];
				finishValues = new int[count];
				startValuesPattern = new int[count];
				finishValuesPattern = new int[count];
				for (int i = 0; i < count; i++) {
					startValues[i] = IncomingInStartTable[i][dependentIndex];
					finishValues[i] = IncomingInFinishTable[i][dependentIndex];
					startValuesPattern[i] = IncomingInStartTable[i][parentIndex];
					finishValuesPattern[i] = IncomingInFinishTable[i][parentIndex];
				}
			}
			for (int i = 0; i < count; i++) {
				if (startValuesPattern[i] == this.currentGroupIndex && startValues[i] != 1)
					return false;
				if (finishValuesPattern[i] == this.currentGroupIndex && finishValues[i] != 1)
					return false;
			}
			return true;
		}
		void CalculateConnectionPointsSingleViewInfo(DependencyCounter counter, GanttAppointmentViewInfoWrapper info, Rectangle visibleBounds) {
			CalculateConnectionPointsForHorizontalBounds(info, visibleBounds);
			CalculateConnectionPointsForVerticalBounds(counter, info, visibleBounds);
		}
		void CalculateConnectionPointsForVerticalBounds(DependencyCounter counter, GanttAppointmentViewInfoWrapper info, Rectangle visibleBounds) {
			ConnectionPointsInfo connectionInfo = info.ConnectionPointsInfo;
			int maxLocationsCount = GetMaxLocationsCount(info.Bounds.Height, MinDependencyDistance);
			if (info.Bounds.Right <= visibleBounds.Left) {
				CalculateConnectionPointsForVerticalBoundsCore(connectionInfo.Right, maxLocationsCount, counter.StartCount + counter.FinishCount, info.Bounds.Right, info.Bounds);
				return;
			}
			if (info.Bounds.Left >= visibleBounds.Right) {
				CalculateConnectionPointsForVerticalBoundsCore(connectionInfo.Left, maxLocationsCount, counter.StartCount + counter.FinishCount, info.Bounds.Left, info.Bounds);
				return;
			}
			CalculateConnectionPointsForVerticalBoundsCore(connectionInfo.Left, maxLocationsCount, counter.StartCount, info.Bounds.Left, info.Bounds);
			CalculateConnectionPointsForVerticalBoundsCore(connectionInfo.Right, maxLocationsCount, counter.FinishCount, info.Bounds.Right, info.Bounds);
		}
		void CalculateConnectionPointsForVerticalBoundsCore(PointStack stack, int maxLocationsCount, int desiredLocationsCount, int x, Rectangle bounds) {
			int top = bounds.Top;
			int distance = bounds.Height / (desiredLocationsCount + 1);
			if (desiredLocationsCount == 0)
				return;
			if (distance < MinDependencyDistance) {
				for (int i = 0; i < maxLocationsCount; i++) {
					stack.Push(new Point(x, top + MinDependencyDistance * i));
				}
			} else {
				for (int i = 0; i < desiredLocationsCount; i++) {
					stack.Push(new Point(x, top + distance * (i + 1)));
				}
			}
		}
		void CalculateConnectionPointsForHorizontalBounds(GanttAppointmentViewInfoWrapper info, Rectangle visibleBounds) {
			ConnectionPointsInfo connInfo = info.ConnectionPointsInfo;
			Rectangle bounds = info.Bounds;
			if (bounds.Right >= visibleBounds.Right)
				bounds.Width = bounds.Width - (bounds.Right - visibleBounds.Right);
			int availableWidth = bounds.Width / 2;
			int count = availableWidth / arrowWidthWithIndent;
			CalculateConnectionPointsForHorizontalBoundsCore(connInfo.TopLeft, bounds.Left + availableWidth, bounds.Left, bounds.Top, count);
			CalculateConnectionPointsForHorizontalBoundsCore(connInfo.BottomLeft, bounds.Left + availableWidth, bounds.Left, bounds.Bottom, count);
			CalculateConnectionPointsForHorizontalBoundsCore(connInfo.TopRight, bounds.Left + availableWidth, bounds.Right, bounds.Top, count);
			CalculateConnectionPointsForHorizontalBoundsCore(connInfo.BottomRight, bounds.Left + availableWidth, bounds.Right, bounds.Bottom, count);
		}
		void CalculateConnectionPointsForHorizontalBoundsCore(PointStack stack, int start, int end, int y, int count) {
			if (count == 0)
				stack.Push(new Point((start + end) / 2, y));
			else {
				int sign = Math.Sign(end - start);
				start = end - (sign * arrowWidthWithIndent * count);
				for (int i = 0; i < count; i++) {
					stack.Push(new Point(start + i * sign * arrowWidthWithIndent, y));
				}
			}
		}
	}
	#endregion
}
