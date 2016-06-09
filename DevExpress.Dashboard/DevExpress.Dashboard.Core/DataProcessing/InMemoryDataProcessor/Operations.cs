#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data.Filtering;
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.XtraPivotGrid;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor {
	public abstract class CustomOperation {
		int hashCode = int.MinValue;
		public abstract IEnumerable<CustomOperation> Operands { get; }
		public virtual bool CanReturnEndOfFlow { get { return false; } }
		public int GetSimpleHashCode() {
			if (hashCode == int.MinValue)
				hashCode = HashcodeHelper.GetCompositeHashCode<int>(OperationSpecificHashCode(), OperationParametersHashCode());
			return hashCode;
		}
		public bool SimpleEquals(CustomOperation other) {
			if (object.ReferenceEquals(this, other))
				return true;
			else
				return GetType() == other.GetType()
					&& EqualsParameters(other)
					&& OperationSpecificEquals(other);
		}
		public override string ToString() {
			string operands = "";
			foreach (CustomOperation op in Operands)
				operands += (string.IsNullOrEmpty(operands) ? "" : ", ") + op.ToString();
			string parameters = GetParamsString();
			parameters = String.IsNullOrEmpty(parameters) ? "" : (parameters + (String.IsNullOrEmpty(operands) ? "" : ", "));
			return String.Format("{0}({1}{2})", GetType().Name, parameters, operands);
		}
		public string ParamsToString() {
			return GetParamsString();
		}
		public abstract void ReplaceOperand(CustomOperation oldOperand, CustomOperation newOperand);
		public abstract CustomOperation Clone(Func<CustomOperation, CustomOperation> replace);
		public virtual IEnumerable<IEnumerable<CustomOperation>> GetOperandsCardinalityGroups() {
			yield return Operands;
		}
		protected virtual string GetParamsString() { return ""; }
		protected abstract bool EqualsParameters(CustomOperation other);
		protected abstract bool OperationSpecificEquals(CustomOperation other);
		protected abstract int OperationSpecificHashCode();
		protected abstract int OperationParametersHashCode();
		protected static T Cast<T>(CustomOperation op) where T : CustomOperation { 
			return (T)op;
		}
	}
	public abstract class FlowOperation : CustomOperation { }
	public abstract class BlockOperation : CustomOperation { }
	public abstract class SingleFlowOperation : FlowOperation {
		public abstract Type OperationType { get; }
		protected override bool OperationSpecificEquals(CustomOperation other) {
			return OperationType == ((SingleFlowOperation)other).OperationType;
		}
		protected override int OperationSpecificHashCode() {
			return OperationType.GetHashCode();
		}
	}
	public abstract class MultiFlowOperation : FlowOperation {
		public abstract Type[] OperationTypes { get; }
		protected override bool OperationSpecificEquals(CustomOperation other) {
			return OperationTypes.SequenceEqual(((MultiFlowOperation)other).OperationTypes);
		}
		protected override int OperationSpecificHashCode() {
			return HashcodeHelper.GetCompositeHashCode<Type>(OperationTypes);
		}
	}
	public abstract class SingleBlockOperation : BlockOperation {
		public abstract Type OperationType { get; }
		protected override bool OperationSpecificEquals(CustomOperation other) {
			return OperationType == ((SingleBlockOperation)other).OperationType;
		}
		protected override int OperationSpecificHashCode() {
			return OperationType.GetHashCode();
		}
	}
	public abstract class MultiBlockOperation : BlockOperation {
		public abstract Type[] OperationTypes { get; }
		protected override bool OperationSpecificEquals(CustomOperation other) {
			return OperationTypes.SequenceEqual(((MultiBlockOperation)other).OperationTypes);
		}
		protected override int OperationSpecificHashCode() {
			return HashcodeHelper.GetCompositeHashCode<Type>(OperationTypes);
		}
	}
	public abstract class ScanBase : SingleFlowOperation {
		public override bool CanReturnEndOfFlow { get { return true; } }
		public IStorage Storage { get; private set; }
		public string ColumnName { get; private set; }
		public override IEnumerable<CustomOperation> Operands { get { return Flows; } }
		protected SingleFlowOperation[] Flows { get; set; }
		protected ScanBase(string columnName, IStorage storage, params SingleFlowOperation[] flows) {
			this.ColumnName = columnName;
			this.Storage = storage;
			this.Flows = flows;
		}
		protected override string GetParamsString() { return ColumnName; }
		protected override bool EqualsParameters(CustomOperation other) {
			ScanBase scan = (ScanBase)other;
			return ColumnName == scan.ColumnName && Storage == scan.Storage;
		}
		protected override int OperationParametersHashCode() {
			return HashcodeHelper.GetCompositeHashCode<object>(ColumnName, Storage);
		}
		public override void ReplaceOperand(CustomOperation oldOperand, CustomOperation newOperand) {
			ReplaceFlow(oldOperand, newOperand);
		}
		protected virtual void ReplaceFlow(CustomOperation oldOperand, CustomOperation newOperand) {
			for (int i = 0; i < Flows.Length; i++)
				if (Flows[i] == oldOperand)
					Flows[i] = (SingleFlowOperation)newOperand;
		}
	}
	public class Scan : ScanBase {
		public override Type OperationType { get { return Storage != null ? Storage.GetColumnType(ColumnName) : typeof(object); } }
		public Scan(string columnName) : this(columnName, null) { }
		public Scan(string columnName, IStorage storage) : base(columnName, storage) { }
		public override CustomOperation Clone(Func<CustomOperation, CustomOperation> replace) {
			return new Scan(ColumnName, Storage);
		}
	}
	public class SurrogateScan : ScanBase {
		public override Type OperationType { get { return typeof(int); } }
		public SurrogateScan(string columnName, IStorage storage) : base(columnName, storage) { }
		public override CustomOperation Clone(Func<CustomOperation, CustomOperation> replace) {
			return new SurrogateScan(ColumnName, Storage);
		}
	}
	public abstract class SelectScanBase : ScanBase {
		public SingleFlowOperation FilterFlow { get; private set; }
		protected SelectScanBase(string columnName, IStorage storage, SingleFlowOperation filterFlow)
			: base(columnName, storage, filterFlow) {
			this.FilterFlow = filterFlow;
		}
	}
	public class SelectScan : SelectScanBase {
		public override Type OperationType { get { return Storage != null ? Storage.GetColumnType(ColumnName) : typeof(object); } }
		public SelectScan(string columnName, IStorage storage, SingleFlowOperation filterFlow) : base(columnName, storage, filterFlow) { }
		public override CustomOperation Clone(Func<CustomOperation, CustomOperation> replace) {
			return new SelectScan(ColumnName, Storage, replace(FilterFlow) as SingleFlowOperation);
		}
	}
	public class SurrogateSelectScan : SelectScanBase {
		public override Type OperationType { get { return typeof(int); } }
		public SurrogateSelectScan(string columnName, IStorage storage, SingleFlowOperation filterFlow) : base(columnName, storage, filterFlow) { }
		public override CustomOperation Clone(Func<CustomOperation, CustomOperation> replace) {
			return new SurrogateSelectScan(ColumnName, Storage, replace(FilterFlow) as SingleFlowOperation);
		}
	}
	public class ScanBuffer : SingleFlowOperation {
		public override bool CanReturnEndOfFlow { get { return true; } }
		public SingleBlockOperation Buffer { get; set; }
		public override Type OperationType { get { return Buffer.OperationType; } }
		public override IEnumerable<CustomOperation> Operands { get { return new[] { Buffer }; } }
		public ScanBuffer(SingleBlockOperation buffer) {
			Buffer = buffer;
		}
		protected override bool EqualsParameters(CustomOperation other) { return true; }
		protected override int OperationParametersHashCode() { return 0; }
		public override void ReplaceOperand(CustomOperation oldOperand, CustomOperation newOperand) {
			if (Buffer == oldOperand)
				Buffer = newOperand as SingleBlockOperation;
		}
		public override CustomOperation Clone(Func<CustomOperation, CustomOperation> replace) {
			return new ScanBuffer((SingleBlockOperation)replace(Buffer));
		}
	}
	public class MultiScanBuffer : MultiFlowOperation {
		public override bool CanReturnEndOfFlow { get { return true; } }
		public MultiBlockOperation Buffer { get; set; }
		public override Type[] OperationTypes { get { return Buffer.OperationTypes; } }
		public override IEnumerable<CustomOperation> Operands { get { return new[] { Buffer }; } }
		public MultiScanBuffer(MultiBlockOperation buffer) {
			Buffer = buffer;
		}
		protected override bool EqualsParameters(CustomOperation other) { return true; }
		protected override int OperationParametersHashCode() { return 0; }
		public override void ReplaceOperand(CustomOperation oldOperand, CustomOperation newOperand) {
			if (Buffer == oldOperand)
				Buffer = (MultiBlockOperation)newOperand;
		}
		public override CustomOperation Clone(Func<CustomOperation, CustomOperation> replace) {
			return new MultiScanBuffer((MultiBlockOperation)replace(Buffer));
		}
	}
	public class ConstScan : SingleFlowOperation { 
		Type operationType;
		public object Value { get; private set; }
		public int Count { get; private set; }
		public override Type OperationType { get { return operationType; } }
		public override bool CanReturnEndOfFlow { get { return true; } }
		public override IEnumerable<CustomOperation> Operands { get { return new CustomOperation[0]; } }
		public ConstScan(Type type, object value, int count)
			: base() {
			this.Value = value;
			this.Count = count;
			this.operationType = type;
		}
		protected override string GetParamsString() { return String.Format("{0}, {1}", Value.ToString(), Count.ToString()); }
		protected override bool EqualsParameters(CustomOperation other) {
			ConstScan scan = (ConstScan)other;
			return Value.Equals(scan.Value) && Count == scan.Count;
		}
		protected override int OperationParametersHashCode() { return HashcodeHelper.GetCompositeHashCode<object>(Value, Count); }
		public override void ReplaceOperand(CustomOperation oldOperand, CustomOperation newOperand) { }
		public override CustomOperation Clone(Func<CustomOperation, CustomOperation> replace) {
			return new ConstScan(operationType, Value, Count);
		}
	}
	public class Project : SingleFlowOperation {
		Type operationType;
		public string ExpressionString { get; private set; }
		public SingleFlowOperation[] Arguments { get; set; }
		public override Type OperationType { get { return operationType; } }
		public override IEnumerable<CustomOperation> Operands { get { return Arguments; } }
		public Project(string expression, params SingleFlowOperation[] arguments) {
			this.ExpressionString = expression;
			this.Arguments = arguments;
			this.operationType = ProjectExpressionConstructor.InferResultType(
				expression,
				Arguments.Select(o => o.OperationType).ToArray());
		}
		protected override string GetParamsString() { return ExpressionString; }
		protected override bool EqualsParameters(CustomOperation other) { return ExpressionString == ((Project)other).ExpressionString; }
		protected override int OperationParametersHashCode() { return ExpressionString.GetHashCode(); }
		public override void ReplaceOperand(CustomOperation oldOperand, CustomOperation newOperand) {
			for (int i = 0; i < Arguments.Length; i++)
				if (Arguments[i] == oldOperand)
					Arguments[i] = (SingleFlowOperation)newOperand;
		}
		public override CustomOperation Clone(Func<CustomOperation, CustomOperation> replace) {
			SingleFlowOperation[] arg = new SingleFlowOperation[Arguments.Length];
			for (int i = 0; i < Arguments.Length; i++)
				arg[i] = replace(Arguments[i]) as SingleFlowOperation;
			return new Project(ExpressionString, arg);
		}
	}
	public class ProjectDateTime : SingleFlowOperation {
		public PivotGroupInterval GroupInterval { get; private set; }
		public SingleFlowOperation Argument { get; set; }
		public override Type OperationType { get { return GetOperationType(GroupInterval); } }
		public override IEnumerable<CustomOperation> Operands { get { return new[] { Argument }; } }
		public ProjectDateTime(SingleFlowOperation argument, PivotGroupInterval groupInterval) {
			DXContract.Requires(DateGroupIntervals.Contains(groupInterval));
			DXContract.Equals(argument.OperationType, typeof(DateTime));
			this.Argument = argument;
			this.GroupInterval = groupInterval;
		}
		public static IEnumerable<PivotGroupInterval> DateGroupIntervals {
			get {
				return IntGroupIntervals.Concat(DayOfWeekGroupIntervals).Concat(DateTimeGroupIntervals);
			}
		}
		public static PivotGroupInterval[] IntGroupIntervals = new PivotGroupInterval[] {
				PivotGroupInterval.DateDay,
				PivotGroupInterval.DateDayOfYear,
				PivotGroupInterval.Hour,
				PivotGroupInterval.Minute,
				PivotGroupInterval.DateMonth,
				PivotGroupInterval.DateQuarter,
				PivotGroupInterval.Second,
				PivotGroupInterval.DateWeekOfMonth,
				PivotGroupInterval.DateWeekOfYear,
				PivotGroupInterval.DateYear
			};
		public static PivotGroupInterval[] DayOfWeekGroupIntervals = new PivotGroupInterval[] {
				PivotGroupInterval.DateDayOfWeek,
			};
		public static PivotGroupInterval[] DateTimeGroupIntervals = new PivotGroupInterval[] {
				PivotGroupInterval.DateHour,
				PivotGroupInterval.DateHourMinute,
				PivotGroupInterval.DateHourMinuteSecond,
				PivotGroupInterval.Date,
				PivotGroupInterval.DateMonthYear,
				PivotGroupInterval.DateQuarterYear,
			};
		Type GetOperationType(PivotGroupInterval groupInterval) {
			if (IntGroupIntervals.Contains(groupInterval))
				return typeof(int);
			if (DayOfWeekGroupIntervals.Contains(groupInterval))
				return typeof(DayOfWeek);
			if (DateTimeGroupIntervals.Contains(groupInterval))
				return typeof(DateTime);
			return typeof(object);
		}
		protected override bool EqualsParameters(CustomOperation other) {
			return GroupInterval == ((ProjectDateTime)other).GroupInterval;
		}
		protected override string GetParamsString() {
			return GroupInterval.ToString();
		}
		protected override int OperationParametersHashCode() {
			return GroupInterval.GetHashCode();
		}
		public override void ReplaceOperand(CustomOperation oldOperand, CustomOperation newOperand) {
			if (oldOperand == Argument)
				Argument = (SingleFlowOperation)newOperand;
		}
		public override CustomOperation Clone(Func<CustomOperation, CustomOperation> replace) {
			return new ProjectDateTime(replace(Argument) as SingleFlowOperation, GroupInterval);
		}
	}
	public class Select : SingleFlowOperation {
		public SingleFlowOperation ValuesFlow { get; set; }
		public SingleFlowOperation FilterFlow { get; set; }
		public override Type OperationType { get { return ValuesFlow.OperationType; } }
		public override bool CanReturnEndOfFlow { get { return true; } }
		public override IEnumerable<CustomOperation> Operands { get { return new[] { ValuesFlow, FilterFlow }; } }
		public Select(SingleFlowOperation valuesFlow, SingleFlowOperation filterFlow) {
			DXContract.Requires(filterFlow.OperationType == typeof(bool));
			this.ValuesFlow = valuesFlow;
			this.FilterFlow = filterFlow;
		}
		protected override bool EqualsParameters(CustomOperation other) { return true; }
		protected override int OperationParametersHashCode() { return 0; }
		public override void ReplaceOperand(CustomOperation oldOperand, CustomOperation newOperand) {
			SingleFlowOperation operand = (SingleFlowOperation)newOperand;
			if (ValuesFlow == oldOperand)
				ValuesFlow = operand;
			else if (FilterFlow == oldOperand)
				FilterFlow = operand;
		}
		public override CustomOperation Clone(Func<CustomOperation, CustomOperation> replace) {
			return new Select(replace(ValuesFlow) as SingleFlowOperation, replace(FilterFlow) as SingleFlowOperation);
		}
	}
	public class Group : MultiBlockOperation {
		public SingleFlowOperation[] Dimensions { get; set; }
		public override Type[] OperationTypes { get { return Dimensions.Select(op => op.OperationType).ToArray(); } }
		public override IEnumerable<CustomOperation> Operands { get { return Dimensions; } }
		public Group(params SingleFlowOperation[] dimensions) {
			this.Dimensions = dimensions;
		}
		protected override bool EqualsParameters(CustomOperation other) { return true; }
		protected override int OperationParametersHashCode() { return 0; }
		public override void ReplaceOperand(CustomOperation oldOperand, CustomOperation newOperand) {
			for (int i = 0; i < Dimensions.Length; i++)
				if (Dimensions[i] == oldOperand)
					Dimensions[i] = (SingleFlowOperation)newOperand;
		}
		public override CustomOperation Clone(Func<CustomOperation, CustomOperation> replace) {
			SingleFlowOperation[] dim = new SingleFlowOperation[Dimensions.Length];
			for (int i = 0; i < Dimensions.Length; i++)
				dim[i] = (SingleFlowOperation)replace(Dimensions[i]);
			return new Group(dim);
		}
	}
	public enum SortOrder { Ascending, Descending }
	public class Sort : MultiBlockOperation {
		public SingleFlowOperation[] Values { get; set; }
		public int[] SortBy { get; protected set; }
		public SortOrder[] SortOrder { get; protected set; }
		public override Type[] OperationTypes { get { return Values.Select(op => op.OperationType).ToArray(); } }
		public override IEnumerable<CustomOperation> Operands { get { return Values; } }
		public Sort(SingleFlowOperation[] values, int[] sortBy, SortOrder[] sortOrder) {
			DXContract.Requires(values.Length > 0);
			this.Values = values;
			this.SortBy = sortBy;
			this.SortOrder = sortOrder;
		}
		protected override bool EqualsParameters(CustomOperation other) {
			Sort sort = (Sort)other;
			return SortBy.SequenceEqual(sort.SortBy)
				&& SortOrder.SequenceEqual(sort.SortOrder);
		}
		protected override int OperationParametersHashCode() {
			return HashcodeHelper.GetCompositeHashCode(SortBy)
				^ HashcodeHelper.GetCompositeHashCode(SortOrder);
		}
		public override void ReplaceOperand(CustomOperation oldOperand, CustomOperation newOperand) {
			for (int i = 0; i < Values.Length; i++)
				if (Values[i] == oldOperand)
					Values[i] = (SingleFlowOperation)newOperand;
		}
		public override CustomOperation Clone(Func<CustomOperation, CustomOperation> replace) {
			SingleFlowOperation[] val = new SingleFlowOperation[Values.Length];
			for (int i = 0; i < Values.Length; i++)
				val[i] = replace(Values[i]) as SingleFlowOperation;
			return new Sort(val, SortBy, SortOrder);
		}
	}
	public abstract class AggregateBase : SingleBlockOperation {
		static SummaryType[] numericSummaryTypes = new[] { SummaryType.Sum, SummaryType.Average, SummaryType.StdDev, SummaryType.StdDevp, SummaryType.Var, SummaryType.Varp };
		static Type[] numericTypes = new[] { typeof(Byte), typeof(SByte), 
				typeof(Int16), typeof(Int32), typeof(Int64), 
				typeof(UInt16), typeof(UInt32), typeof(UInt64), 
				typeof(float), typeof(double), typeof(decimal) };
		public SummaryType SummaryType { get; private set; }
		public SingleFlowOperation ValuesFlow { get; set; }
		public override IEnumerable<CustomOperation> Operands { get { return new CustomOperation[] { ValuesFlow }; } }
		public override Type OperationType { get { return GetResultType(); } }
		protected AggregateBase(SingleFlowOperation valuesFlow, SummaryType summaryType) {
			this.SummaryType = summaryType;
			this.ValuesFlow = valuesFlow;
			if (!IsNumericAggregationSupported(summaryType, valuesFlow.OperationType))
				throw new ArgumentException(String.Format("Can not aggregate {0} values of type {1}", summaryType.ToString(), valuesFlow.OperationType.Name));
		}
		Type GetResultType() {
			switch (SummaryType) {
				case SummaryType.Sum: return GetTypeSumCalculator(ValuesFlow.OperationType);
				case SummaryType.Min:
				case SummaryType.Max: return ValuesFlow.OperationType;
				case SummaryType.Average: return GetTypeAverageCalculator(ValuesFlow.OperationType);
				case SummaryType.Count:
				case SummaryType.CountDistinct: return typeof(int);
				case SummaryType.StdDev:
				case SummaryType.StdDevp:
				case SummaryType.Var:
				case SummaryType.Varp: return typeof(double);
				default:
					throw new NotSupportedException();
			}
		}
		Type GetTypeSumCalculator(Type operationType) {
			if (operationType == typeof(UInt16) || operationType == typeof(UInt32) || operationType == typeof(UInt64))
				return typeof(UInt64);
			else if (operationType == typeof(float))
				return typeof(float);
			else if (operationType == typeof(double))
				return typeof(double);
			else if (operationType == typeof(decimal))
				return typeof(decimal);
			else
				return typeof(Int64);
		}
		Type GetTypeAverageCalculator(Type operationType) {
			if (operationType == typeof(float))
				return typeof(float);
			else if (operationType == typeof(decimal))
				return typeof(decimal);
			else
				return typeof(double);
		}
		protected override bool EqualsParameters(CustomOperation other) { return SummaryType == ((GrandTotalAggregate)other).SummaryType; }
		protected override string GetParamsString() {
			return SummaryType.ToString();
		}
		protected override int OperationParametersHashCode() { return SummaryType.GetHashCode(); }
		public override void ReplaceOperand(CustomOperation oldOperand, CustomOperation newOperand) {
			ValuesFlow = (SingleFlowOperation)newOperand;
		}
		public override CustomOperation Clone(Func<CustomOperation, CustomOperation> replace) {
			return new GrandTotalAggregate(replace(ValuesFlow) as SingleFlowOperation, SummaryType);
		}
		public static bool IsNumericAggregationSupported(SummaryType summaryType, Type argumentType) {
			return numericSummaryTypes.Contains(summaryType) ? numericTypes.Contains((argumentType)) : true;
		}
	}
	public class GrandTotalAggregate : AggregateBase {
		public GrandTotalAggregate(SingleFlowOperation valuesFlow, SummaryType summaryType) : base(valuesFlow, summaryType) { }
	}
	public class GroupAggregate : AggregateBase {
		public SingleFlowOperation Indexes { get; set; }
		public override IEnumerable<CustomOperation> Operands { get { return new CustomOperation[] { Indexes, ValuesFlow }; } }
		public GroupAggregate(SingleFlowOperation indexes, SingleFlowOperation valuesFlow, SummaryType summaryType)
			: base(valuesFlow, summaryType) {
			this.Indexes = indexes;
		}
		protected override bool EqualsParameters(CustomOperation other) { return SummaryType == ((GroupAggregate)other).SummaryType; }
		public override IEnumerable<IEnumerable<CustomOperation>> GetOperandsCardinalityGroups() {
			yield return new[] { ValuesFlow };
			yield return new[] { Indexes };
		}
		public override void ReplaceOperand(CustomOperation oldOperand, CustomOperation newOperand) {
			SingleFlowOperation newOperandSingleFlow = newOperand as SingleFlowOperation;
			if (ValuesFlow == oldOperand)
				ValuesFlow = newOperandSingleFlow;
			else if (Indexes == oldOperand) {
				if (newOperandSingleFlow != null)
					Indexes = newOperandSingleFlow;
			}
		}
		public override CustomOperation Clone(Func<CustomOperation, CustomOperation> replace) {
			return new GroupAggregate(replace(Indexes) as SingleFlowOperation, replace(ValuesFlow) as SingleFlowOperation, SummaryType);
		}
	}
	public class Extract : SingleFlowOperation {
		public MultiFlowOperation BlockOperation { get; set; }
		public int ResultNumber { get; private set; }
		public override Type OperationType { get { return BlockOperation.OperationTypes[ResultNumber]; } }
		public override IEnumerable<CustomOperation> Operands { get { return new[] { BlockOperation }; } }
		public Extract(MultiFlowOperation blockOperation, int flowNumber) {
			this.BlockOperation = blockOperation;
			this.ResultNumber = flowNumber;
		}
		protected override string GetParamsString() { return ResultNumber.ToString(); }
		protected override bool EqualsParameters(CustomOperation other) { return ResultNumber == ((Extract)other).ResultNumber; }
		protected override int OperationParametersHashCode() { return ResultNumber.GetHashCode(); }
		public override void ReplaceOperand(CustomOperation oldOperand, CustomOperation newOperand) {
			if (BlockOperation == oldOperand)
				BlockOperation = (MultiFlowOperation)newOperand;
		}
		public override CustomOperation Clone(Func<CustomOperation, CustomOperation> replace) {
			return new Extract(replace(BlockOperation) as MultiFlowOperation, ResultNumber);
		}
	}
	public class BlockExtract : SingleBlockOperation {
		public MultiBlockOperation BlockOperation { get; set; }
		public int ResultNumber { get; private set; }
		public override Type OperationType { get { return BlockOperation.OperationTypes[ResultNumber]; } }
		public override IEnumerable<CustomOperation> Operands { get { return new[] { BlockOperation }; } }
		public BlockExtract(MultiBlockOperation multiBlockOperation, int resultNumber) {
			this.BlockOperation = multiBlockOperation;
			this.ResultNumber = resultNumber;
		}
		protected override string GetParamsString() { return ResultNumber.ToString(); }
		protected override bool EqualsParameters(CustomOperation other) { return ResultNumber == ((BlockExtract)other).ResultNumber; }
		protected override int OperationParametersHashCode() { return ResultNumber.GetHashCode(); }
		public override void ReplaceOperand(CustomOperation oldOperand, CustomOperation newOperand) {
			if (BlockOperation == oldOperand)
				BlockOperation = (MultiBlockOperation)newOperand;
		}
		public override CustomOperation Clone(Func<CustomOperation, CustomOperation> replace) {
			return new BlockExtract(replace(BlockOperation) as MultiBlockOperation, ResultNumber);
		}
	}
	public class Buffer : SingleBlockOperation {
		public SingleFlowOperation Argument { get; set; }
		public override Type OperationType { get { return Argument.OperationType; } }
		public override IEnumerable<CustomOperation> Operands { get { return new[] { Argument }; } }
		public Buffer(SingleFlowOperation argument) {
			this.Argument = argument;
		}
		protected override bool EqualsParameters(CustomOperation other) { return true; }
		protected override int OperationParametersHashCode() { return 0; }
		public override void ReplaceOperand(CustomOperation oldOperand, CustomOperation newOperand) {
			if (Argument == oldOperand)
				Argument = (SingleFlowOperation)newOperand;
		}
		public override CustomOperation Clone(Func<CustomOperation, CustomOperation> replace) {
			return new Buffer(replace(Argument) as SingleFlowOperation);
		}
	}
	public class Materialize : SingleFlowOperation {
		public SingleFlowOperation SurrogatesFlow { get; set; }
		public IStorage Storage { get; private set; }
		public string ColumnName { get; private set; }
		public override IEnumerable<CustomOperation> Operands { get { return new[] { SurrogatesFlow }; } }
		public Materialize(string columnName, IStorage storage, SingleFlowOperation surrogatesFlow) {
			ColumnName = columnName;
			Storage = storage;
			SurrogatesFlow = surrogatesFlow;
		}
		public override Type OperationType { get { return Storage != null ? Storage.GetColumnType(ColumnName) : typeof(object); } }
		protected override bool EqualsParameters(CustomOperation other) {
			Materialize m = (Materialize)other;
			return ColumnName == m.ColumnName && Storage == m.Storage;
		}
		protected override string GetParamsString() {
			return ColumnName;
		}
		protected override int OperationParametersHashCode() {
			return HashcodeHelper.GetCompositeHashCode<object>(ColumnName, Storage);
		}
		public override void ReplaceOperand(CustomOperation oldOperand, CustomOperation newOperand) {
			if (SurrogatesFlow == oldOperand)
				SurrogatesFlow = (SingleFlowOperation)newOperand;
		}
		public override CustomOperation Clone(Func<CustomOperation, CustomOperation> replace) {
			return new Materialize(ColumnName, Storage, replace(SurrogatesFlow) as SingleFlowOperation);
		}
	}
	public class Join : MultiFlowOperation {
		public SingleFlowOperation[] Criteria1 { get; private set; }
		public SingleBlockOperation[] Criteria2 { get; private set; }
		public SingleBlockOperation[] DataFlow { get; private set; }
		public override Type[] OperationTypes {
			get {
				Type[] typeOperations = new Type[DataFlow.Length];
				int i = 0;
				foreach (SingleBlockOperation op in DataFlow)
					typeOperations[i++] = op.OperationType;
				return typeOperations;
			}
		}
		public override IEnumerable<CustomOperation> Operands {
			get {
				List<CustomOperation> operands = new List<CustomOperation>();
				foreach (SingleFlowOperation op in Criteria1)
					operands.Add(op);
				foreach (SingleBlockOperation op in Criteria2)
					operands.Add(op);
				foreach (SingleBlockOperation op in DataFlow)
					operands.Add(op);
				return operands;
			}
		}
		public Join(SingleFlowOperation[] criteriaFlow, SingleBlockOperation[] joinFlow, SingleBlockOperation[] dataFlow) {
			this.Criteria1 = criteriaFlow;
			this.Criteria2 = joinFlow;
			this.DataFlow = dataFlow;
		}
		protected override bool EqualsParameters(CustomOperation other) { return true; }
		protected override int OperationParametersHashCode() { return 0; }
		public override void ReplaceOperand(CustomOperation oldOperand, CustomOperation newOperand) {
			for(int i = 0; i < Criteria1.Length; i++)
				if(Criteria1[i] == oldOperand)
					Criteria1[i] = Cast<SingleFlowOperation>(newOperand);
			for(int i = 0; i < Criteria2.Length; i++)
				if(Criteria2[i] == oldOperand)
					Criteria2[i] = Cast<SingleBlockOperation>(newOperand);
			for(int i = 0; i < DataFlow.Length; i++)
				if(DataFlow[i] == oldOperand)
					DataFlow[i] = Cast<SingleBlockOperation>(newOperand);
		}
		public override CustomOperation Clone(Func<CustomOperation, CustomOperation> replace) {
			SingleFlowOperation[] nCriteriaFlow = new SingleFlowOperation[Criteria1.Length];
			for(int i = 0; i < Criteria1.Length; i++)
				nCriteriaFlow[i] = Cast<SingleFlowOperation>(replace(Criteria1[i]));
			SingleBlockOperation[] nJoinFlow = new SingleBlockOperation[Criteria2.Length];
			for(int i = 0; i < Criteria2.Length; i++)
				nJoinFlow[i] = Cast<SingleBlockOperation>(replace(Criteria2[i]));
			SingleBlockOperation[] nDataFlow = new SingleBlockOperation[DataFlow.Length];
			for(int i = 0; i < DataFlow.Length; i++)
				nDataFlow[i] = Cast<SingleBlockOperation>(replace(DataFlow[i]));
			return new Join(nCriteriaFlow, nJoinFlow, nDataFlow);
		}
		public override IEnumerable<IEnumerable<CustomOperation>> GetOperandsCardinalityGroups() {
			List<CustomOperation> criteria1 = new List<CustomOperation>();
			criteria1.AddRange(Criteria1);
			List<CustomOperation> operands = new List<CustomOperation>();
			operands.AddRange(Criteria2);
			operands.AddRange(DataFlow);
			return new List<List<CustomOperation>>() { criteria1, operands };
		}
	}
	public enum TopNMode { Top, Bottom }
	public abstract class TopNBase : SingleBlockOperation {
		public TopNMode Mode { get; private set; }
		public override Type OperationType { get { return typeof(bool); } }
		protected TopNBase(TopNMode mode) {
			this.Mode = mode;
		}
	}
	public class TopN : TopNBase {
		public SingleFlowOperation InputFlow { get; private set; }
		public int TopNCount { get; private set; }
		public override IEnumerable<CustomOperation> Operands { get { return new[] { InputFlow }; } }
		public TopN(SingleFlowOperation inputFlow, int topNCount, TopNMode mode)
			: base(mode) {
			DXContract.Requires(topNCount > 0, "N value must be greater than 0");
			this.InputFlow = inputFlow;
			this.TopNCount = topNCount;
		}
		protected override bool EqualsParameters(CustomOperation other) {
			TopN tn = (TopN)other;
			return TopNCount == tn.TopNCount && Mode == tn.Mode;
		}
		protected override string GetParamsString() {
			return String.Format("{0}, {1}", Mode.ToString(), TopNCount.ToString());
		}
		protected override int OperationParametersHashCode() {
			return HashcodeHelper.GetCompositeHashCode<object>(TopNCount, Mode);
		}
		public override void ReplaceOperand(CustomOperation oldOperand, CustomOperation newOperand) {
			if (InputFlow == oldOperand)
				InputFlow = (SingleFlowOperation)newOperand;
		}
		public override CustomOperation Clone(Func<CustomOperation, CustomOperation> replace) {
			return new TopN(replace(InputFlow) as SingleFlowOperation, TopNCount, Mode);
		}
	}
	public class TopNPercent : TopNBase {
		public SingleBlockOperation InputFlow { get; private set; }
		public double Percent { get; private set; }
		public override IEnumerable<CustomOperation> Operands { get { return new[] { InputFlow }; } }
		public TopNPercent(SingleBlockOperation inputFlow, double topNPercent, TopNMode mode)
			: base(mode) {
			DXContract.Requires(topNPercent <= 1 && topNPercent > 0, "Percent value must be in (0..1] range");
			this.InputFlow = inputFlow;
			this.Percent = topNPercent;
		}
		protected override string GetParamsString() {
			return String.Format("{0}, {1}", Mode.ToString(), Percent.ToString());
		}
		protected override bool EqualsParameters(CustomOperation other) {
			TopNPercent tn = (TopNPercent)other;
			return Percent == tn.Percent && Mode == tn.Mode;
		}
		protected override int OperationParametersHashCode() {
			return HashcodeHelper.GetCompositeHashCode<object>(Percent, Mode);
		}
		public override void ReplaceOperand(CustomOperation oldOperand, CustomOperation newOperand) {
			if (InputFlow == oldOperand)
				InputFlow = (SingleBlockOperation)newOperand;
		}
		public override CustomOperation Clone(Func<CustomOperation, CustomOperation> replace) {
			return new TopNPercent(replace(InputFlow) as SingleBlockOperation, Percent, Mode);
		}
	}
	public class ProjectOthers : SingleFlowOperation {
		public SingleFlowOperation InputFlow { get; private set; }
		public SingleFlowOperation OthersFlow { get; private set; }
		public bool OthersValue { get; private set; }
		public override Type OperationType { get { return InputFlow.OperationType; } }
		public override IEnumerable<CustomOperation> Operands { get { return new[] { InputFlow, OthersFlow }; } }
		public ProjectOthers(SingleFlowOperation inputFlow, SingleFlowOperation othersFlow, bool othersValue) {
			this.InputFlow = inputFlow;
			this.OthersFlow = othersFlow;
			this.OthersValue = othersValue;
		}
		public override void ReplaceOperand(CustomOperation oldOperand, CustomOperation newOperand) {
			if (InputFlow == oldOperand)
				InputFlow = Cast<SingleFlowOperation>(newOperand);
			else if (OthersFlow == oldOperand)
				OthersFlow = Cast<SingleFlowOperation>(newOperand);
		}
		public override CustomOperation Clone(Func<CustomOperation, CustomOperation> replace) {
			return new ProjectOthers(replace(InputFlow) as SingleFlowOperation, replace(OthersFlow) as SingleFlowOperation, OthersValue);
		}
		protected override bool EqualsParameters(CustomOperation other) {
			ProjectOthers po = (ProjectOthers)other;
			return OthersValue == po.OthersValue;
		}
		protected override int OperationParametersHashCode() {
			return OthersValue.GetHashCode();
		}
		public override IEnumerable<IEnumerable<CustomOperation>> GetOperandsCardinalityGroups() {
			return new List<List<CustomOperation>>() { new List<CustomOperation>() { InputFlow, OthersFlow } };
		}
		protected override string GetParamsString() {
			return String.Format("{0}", OthersValue.ToString());
		}
	}
	public class ExtractIndexes : SingleBlockOperation {
		public Group Group { get; private set; }
		public override Type OperationType { get { return typeof(int); } }
		public override IEnumerable<CustomOperation> Operands { get { yield return Group; } }
		public ExtractIndexes(Group group) {
			this.Group = group;
		}
		public override void ReplaceOperand(CustomOperation oldOperand, CustomOperation newOperand) {
			if (Group == oldOperand)
				Group = (Group)newOperand;
		}
		public override CustomOperation Clone(Func<CustomOperation, CustomOperation> replace) {
			return new ExtractIndexes(replace(Group) as Group);
		}
		protected override bool EqualsParameters(CustomOperation other) {
			return true;
		}
		protected override int OperationParametersHashCode() {
			return 0;
		}
	}
	public class JoinIndexes : SingleFlowOperation {
		public SingleFlowOperation Group1Indexes { get; private set; }
		public SingleBlockOperation Group2Indexes { get; private set; }
		public override Type OperationType { get { return typeof(int); } }
		public override IEnumerable<CustomOperation> Operands {
			get {
				yield return Group1Indexes;
				yield return Group2Indexes;
			}
		}
		public JoinIndexes(SingleFlowOperation group1Indexes, SingleBlockOperation group2Indexes) {
			this.Group1Indexes = group1Indexes;
			this.Group2Indexes = group2Indexes;
		}
		public override IEnumerable<IEnumerable<CustomOperation>> GetOperandsCardinalityGroups() {
			yield return new[] { Group1Indexes };
			yield return new[] { Group2Indexes };
		}
		public override void ReplaceOperand(CustomOperation oldOperand, CustomOperation newOperand) {
			if (oldOperand == Group1Indexes)
				Group1Indexes = (SingleFlowOperation)newOperand;
			if (oldOperand == Group2Indexes)
				Group2Indexes = (SingleBlockOperation)newOperand;
		}
		public override CustomOperation Clone(Func<CustomOperation, CustomOperation> replace) {
			return new JoinIndexes(replace(Group1Indexes) as SingleFlowOperation, replace(Group2Indexes) as SingleBlockOperation);
		}
		protected override bool EqualsParameters(CustomOperation other) {
			return true;
		}
		protected override int OperationParametersHashCode() {
			return 0;
		}
	}
	class ConvertType : SingleFlowOperation {
		public enum ConvertTypeCode { DateTime, Boolean, Byte, Char, Decimal, Double, Int16, Int32, Int64, SByte, Single, String, UInt16, UInt32, UInt64 }
		public SingleFlowOperation Argument { get; private set; }
		public Type ResultType { get; private set; }
		public override Type OperationType { get { return ResultType; } }
		public override IEnumerable<CustomOperation> Operands { get { return new[] { Argument }; } }
		public ConvertType(SingleFlowOperation argument, Type resultType) {
			DXContract.Requires(typeof(IConvertible).IsAssignableFrom(argument.OperationType) || argument.OperationType == typeof(object), "The argument operation type of ConvertType operation should implements IConvertiable interface.");
			this.Argument = argument;
			this.ResultType = resultType;
		}
		public override void ReplaceOperand(CustomOperation oldOperand, CustomOperation newOperand) {
			if (oldOperand == Argument)
				Argument = (SingleFlowOperation)newOperand;
		}
		public override CustomOperation Clone(Func<CustomOperation, CustomOperation> replace) {
			return new ConvertType((SingleFlowOperation)replace(Argument), ResultType);
		}
		protected override bool EqualsParameters(CustomOperation other) {
			return ResultType == ((ConvertType)other).ResultType;
		}
		protected override int OperationParametersHashCode() {
			return ResultType.GetHashCode();
		}
		protected override string GetParamsString() {
			return ResultType.Name;
		}
	}
	class OperationParametersEqualityComparer : EqualityComparer<CustomOperation> {
		static OperationParametersEqualityComparer defaultInstance;
		public static new OperationParametersEqualityComparer Default {
			get {
				if (defaultInstance == null)
					defaultInstance = new OperationParametersEqualityComparer();
				return defaultInstance;
			}
		}
		public override bool Equals(CustomOperation x, CustomOperation y) {
			return x.SimpleEquals(y);
		}
		public override int GetHashCode(CustomOperation obj) {
			return obj.GetSimpleHashCode();
		}
	}
}
