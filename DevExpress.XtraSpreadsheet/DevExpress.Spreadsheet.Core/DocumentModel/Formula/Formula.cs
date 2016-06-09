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
using System.Diagnostics;
using System.Globalization;
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.History;
using DevExpress.Office.Services;
using DevExpress.XtraSpreadsheet.Internal;
namespace DevExpress.XtraSpreadsheet.Model {
	public interface IFormula {
		FormulaType Type { get; }
		ParsedExpression Expression { get; }
		FormulaDataState DataState { get; }
		string GetBody(ICell hostCell);
		VariantValue Calculate(ICell cell, WorkbookDataContext context);
		FormulaProperties GetProperties();
	}
	#region FormulaDataState
	public enum FormulaDataState {
		ExpressionReady = 0,
		Binary = 1,
		String = 2,
		XlsBinary = 3
	}
	#endregion
	#region FormulaBase
	public abstract class FormulaBase : IFormula {
		public static readonly byte FormulaTypeMask = 0x3;		  
		public static readonly byte DataStateMask = 0x1C;		   
		public static readonly byte CalculateAlwaysMask = 0x20;	 
		public static readonly byte DataStateOffset = 2;
		public static readonly byte CalculateAlwaysOffset = 5;
		bool calculateAlways;
		protected FormulaBase() {
		}
		#region Properties
		public abstract ParsedExpression Expression { get; protected set; }
		protected virtual int BinaryFormulaOffset { get { return 1; } }
		public FormulaDataState DataState { get { return GetDataState(); } }
		public abstract FormulaType Type { get; }
		public bool CalculateAlways { get { return calculateAlways; } set { SetCalculateAlways(value); } }
		#endregion
		#region Body
		public abstract string GetBody(ICell hostCell);
		#endregion
		#region Notification
		protected internal virtual void UpdateFormula(ReplaceThingsPRNVisitor walker, ICell ownerCell) {
		}
		public virtual void OnBeforeSheetRemoved(RemoveRangeNotificationContext notificationContext, WorkbookDataContext context) {
		}
		public virtual void OnRangeInsertingShiftRight(InsertRangeNotificationContext notificationContext, WorkbookDataContext dataContext, ICell ownerCell) {
			UpdateFormula(notificationContext.Visitor, ownerCell);
		}
		public virtual void OnRangeInsertingShiftDown(InsertRangeNotificationContext notificationContext, WorkbookDataContext dataContext, ICell ownerCell) {
			UpdateFormula(notificationContext.Visitor, ownerCell);
		}
		public virtual void OnRangeRemovingShiftLeft(RemoveRangeNotificationContext notificationContext, WorkbookDataContext dataContext, ICell ownerCell) {
			UpdateFormula(notificationContext.Visitor, ownerCell);
		}
		public virtual void OnRangeRemovingShiftUp(RemoveRangeNotificationContext notificationContext, WorkbookDataContext dataContext, ICell ownerCell) {
			UpdateFormula(notificationContext.Visitor, ownerCell);
		}
		public virtual void OnRangeRemovingNoShift(RemoveRangeNotificationContext notificationContext, WorkbookDataContext dataContext, ICell ownerCell) {
			UpdateFormula(new RemovedNoShiftRPNVisitor(notificationContext, dataContext), ownerCell);
		}
		#endregion
		#region CheckIntegrity
		public abstract void CheckIntegrity();
		public abstract void CheckIntegrity(ICell cell);
		#endregion
		public static bool IsFormula(string text) {
			return text.StartsWith("=", StringComparison.Ordinal) && text.Length > 1;
		}
		public abstract ParsedExpression GetExpression(WorkbookDataContext context);
		public abstract VariantValue Calculate(ICell cell, WorkbookDataContext context);
		public abstract FormulaDataState GetDataState();
		public abstract void InitializeFromBinary(byte[] binaryFormula, Worksheet sheet);
		public abstract byte[] GetBinary(WorkbookDataContext context);
		public virtual void ApplyDataToCell(ICell cell) {
			cell.FormulaInfo.BinaryFormula = GetBinary(cell.Context);
		}
		protected virtual void ReadAdditionalDataFromBinary(Worksheet sheet, byte[] data) {
			byte flagByte = data[0];
			CalculateAlways = PackedValues.GetBoolBitValue(flagByte, FormulaBase.CalculateAlwaysMask);
		}
		protected void ApplyDataToCellTransacted(ICell cell) {
			DocumentHistory history = cell.Worksheet.Workbook.History;
			byte[] newBinaryFormula = GetBinary(cell.Context);
			SpreadsheetHistoryItem historyItem = null;
			if (newBinaryFormula != null)
				historyItem = new CellFormulaModifiedHistoryItem(cell.Worksheet, cell, cell.FormulaInfo.BinaryFormula, newBinaryFormula);
			else
				historyItem = new CellFormulaReplacedWithValueHistoryItem(cell.Worksheet, cell, cell.FormulaInfo.BinaryFormula, cell.Value);
			history.Add(historyItem);
			historyItem.Execute();
		}
		public abstract List<CellRangeBase> GetPrecedents(ICell cell);
		public abstract List<PrecedentPair> GetInvolvedCellRanges(ICell cell);
		public abstract FormulaProperties GetProperties();
		public bool IsVolatile() {
			return (GetProperties() & FormulaProperties.HasVolatileFunction) > 0;
		}
		public bool ContainsFunctionRTD() {
			return (GetProperties() & FormulaProperties.HasFunctionRTD) > 0;
		}
		public abstract void PushSettingsToContext(WorkbookDataContext context, ICell cell);
		public abstract void PopSettingsFromContext(WorkbookDataContext context);
		protected virtual void SetCalculateAlways(bool value) {
			this.calculateAlways = value;
		}
	}
	#endregion
	#region Formula
	public class Formula : FormulaBase {
		#region Fields
		ParsedExpression expression;
		byte[] binaryFormula;
		ICell cell;
		#endregion
		public Formula(ICell cell) {
			this.cell = cell;
		}
		public Formula(ICell cell, ParsedExpression expression)
			: this(cell) {
			Debug.Assert(expression != null);
			this.expression = expression;
		}
		public Formula(ICell cell, string body)
			: this(cell) {
			if (String.IsNullOrEmpty(body) || body[0] != '=')
				ThrowErrorFormula(body, cell.Context.Culture);
			ParseExpression(body, cell, true);
		}
		#region Properties
		public ICell Cell { get { return cell; } set { cell = value; } }
		public override ParsedExpression Expression { get { return GetExpression(Context); } protected set { expression = value; } }
		protected byte[] BinaryFormula { get { return binaryFormula; } set { binaryFormula = value; } }
		protected internal WorkbookDataContext Context { get { return cell == null ? null : cell.Sheet.Workbook.DataContext; } }
		public override FormulaType Type { get { return FormulaType.Normal; } }
		#endregion
		public override FormulaDataState GetDataState() {
			if (expression != null || binaryFormula == null)
				return FormulaDataState.ExpressionReady;
			int dataStateCode = (binaryFormula[0] & DataStateMask) >> DataStateOffset;
			return (FormulaDataState)dataStateCode;
		}
		protected override void SetCalculateAlways(bool value) {
			base.SetCalculateAlways(value);
			if (DataState == FormulaDataState.Binary && binaryFormula.Length > 0) {
				byte flagByte = binaryFormula[0];
				PackedValues.SetBoolBitValue(ref flagByte, FormulaBase.CalculateAlwaysMask, value);
				binaryFormula[0] = flagByte;
			}
		}
		#region PrepareExpression
		public override ParsedExpression GetExpression(WorkbookDataContext context) {
			FormulaDataState dataState = DataState;
			switch (dataState) {
				case FormulaDataState.ExpressionReady:
					break; 
				case FormulaDataState.Binary:
					expression = PrepareExpressionFromBinary(context);
					break;
				case FormulaDataState.String:
					expression = PrepareExpressionFromString(cell, context);
					binaryFormula = null;
					if (cell != null)
						ApplyDataToCell(cell);
					break;
				default:
					throw new InvalidOperationException("Invalid FormulaDataState : " + dataState.ToString());
			}
			return expression;
		}
		ParsedExpression PrepareExpressionFromBinary(WorkbookDataContext context) {
			return ReadParsedExpression(this.binaryFormula, BinaryFormulaOffset, context);
		}
		ParsedExpression PrepareExpressionFromString(ICell cell, WorkbookDataContext context) {
			Debug.Assert((binaryFormula[0] & DataStateMask) == (byte)((int)FormulaDataState.String << DataStateOffset));
			context.SetImportExportSettings();
			try {
				if (binaryFormula != null && binaryFormula.Length > BinaryFormulaOffset) {
					string body = Encoding.Unicode.GetString(binaryFormula, BinaryFormulaOffset, binaryFormula.Length - BinaryFormulaOffset);
					if (!ParseExpression(body, cell, false)) {
						string position = (new CellPosition(cell.ColumnIndex, cell.RowIndex)).ToString();
						context.Workbook.LogMessageFormat(LogCategory.Warning, XtraSpreadsheetStringId.Msg_InvalidFormulaRemoved, position, cell.Sheet.Name);
					}
				}
				return expression;
			}
			finally {
				context.SetWorkbookDefinedSettings();
			}
		}
		public override FormulaProperties GetProperties() {
			FormulaDataState dataState = DataState;
			switch (dataState) {
				case FormulaDataState.String:
				case FormulaDataState.ExpressionReady:
					ParsedExpression expression = Expression;
					if (expression == null)
						return FormulaProperties.None;
					return expression.GetProperties();
				case FormulaDataState.Binary:
					return GetFormulaPropertiesFromBinary();
				default:
					throw new InvalidOperationException("Invalid FormulaDataState : " + dataState.ToString());
			}
		}
		FormulaProperties GetFormulaPropertiesFromBinary() {
			if (binaryFormula == null || binaryFormula.Length < BinaryFormulaOffset + 3)
				return FormulaProperties.None;
			if (binaryFormula[BinaryFormulaOffset] == 0x19 && binaryFormula[BinaryFormulaOffset + 1] == 1)
				return (FormulaProperties)binaryFormula[BinaryFormulaOffset + 2];
			return FormulaProperties.None;
		}
		#endregion
		#region Body
		public override string GetBody(ICell hostCell) {
			WorkbookDataContext context = hostCell.Context;
			context.PushCurrentCell(hostCell);
			try {
				string result = GetUpdatedBody(hostCell, context);
				return result == null ? string.Empty : "=" + result;
			}
			finally {
				context.PopCurrentCell();
			}
		}
		string GetUpdatedBody(ICell cell, WorkbookDataContext context) {
			FormulaDataState dataState = DataState;
			if (dataState != FormulaDataState.Binary && dataState != FormulaDataState.ExpressionReady) {
				GetExpression(Context);
				if (expression == null)
					return null;
			}
			if (expression != null)
				return expression.BuildExpressionString(context);
			else
				return context.RPNContext.BuildExpressionString(binaryFormula, BinaryFormulaOffset);
		}
		#endregion
		#region Binary <-> ParsedExpression conversion
		public override void InitializeFromBinary(byte[] binaryFormula, Worksheet sheet) {
			Guard.ArgumentNotNull(binaryFormula, "binaryFormula");
			Guard.ArgumentNotNull(sheet, "sheet");
			this.binaryFormula = binaryFormula;
			ReadAdditionalDataFromBinary(sheet, binaryFormula);
		}
		public override void ApplyDataToCell(ICell cell) {
			byte[] binary = GetBinary(cell.Worksheet.DataContext);
			if (binary != null)
				cell.FormulaInfo.BinaryFormula = binary;
			else {
				cell.Worksheet.Workbook.CalculationChain.UnRegisterCell(cell);
				cell.FormulaInfo = null;
			}
		}
		protected virtual ParsedExpression ReadParsedExpression(byte[] formulaPtgs, int startIndex, WorkbookDataContext context) {
			return context.RPNContext.BinaryToExpression(formulaPtgs, startIndex);
		}
		public override byte[] GetBinary(WorkbookDataContext context) {
			if (binaryFormula != null)
				return binaryFormula;
			if (expression == null)
				return null;
			return GetBinaryCore(context);
		}
		protected virtual byte[] GetBinaryCore(WorkbookDataContext context) {
			byte[] formulaPtgs = context.RPNContext.ExpressionToBinary(expression);
			byte[] result = new byte[formulaPtgs.Length + BinaryFormulaOffset];
			WriteAdditionalDataToBinary(result, FormulaDataState.Binary);
			formulaPtgs.CopyTo(result, BinaryFormulaOffset);
			return result;
		}
		protected virtual void WriteAdditionalDataToBinary(byte[] data, FormulaDataState dataState) {
			byte flagByte = (byte)Type;
			PackedValues.SetIntBitValue(ref flagByte, FormulaBase.DataStateMask, FormulaBase.DataStateOffset, (byte)dataState);
			PackedValues.SetBoolBitValue(ref flagByte, FormulaBase.CalculateAlwaysMask, CalculateAlways);
			data[0] = flagByte;
		}
		protected internal void SetBodyTemporarily(string formula, ICell cell) {
			this.binaryFormula = GetTemporarilyBody(formula);
		}
		protected internal byte[] GetTemporarilyBody(string formula) {
			byte[] result = new byte[BinaryFormulaOffset + System.Text.Encoding.Unicode.GetByteCount(formula)];
			WriteAdditionalDataToBinary(result, FormulaDataState.String);
			System.Text.Encoding.Unicode.GetBytes(formula, 0, formula.Length, result, BinaryFormulaOffset);
			return result;
		}
		#endregion
		#region ParseExpression
		protected bool ParseExpression(string body, ICell cell, bool throwExceptionOnErrorFormula) {
			WorkbookDataContext context = cell.Context;
			context.PushCurrentCell(cell);
			try {
				expression = ParseExpressionCore(body, context);
			}
			finally {
				context.PopCurrentCell();
			}
			if (expression == null) {
				if (throwExceptionOnErrorFormula)
					ThrowErrorFormula(body, context.Culture);
				else
					return false;
			}
			return true;
		}
		protected void ThrowErrorFormula(string formula, CultureInfo culture) {
			string cultureName = culture == System.Globalization.CultureInfo.InvariantCulture ? "Invariant" : culture.Name;
			string message = string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorFormula), formula, cultureName);
			throw new ArgumentException(message);
		}
		protected virtual ParsedExpression ParseExpressionCore(string body, WorkbookDataContext context) {
			return context.ParseExpression(body, OperandDataType.Value, true);
		}
		#endregion
		#region Calculate
		public override VariantValue Calculate(ICell cell, WorkbookDataContext context) {
			FormulaDataState dataState = DataState;
			if (dataState != FormulaDataState.Binary && dataState != FormulaDataState.ExpressionReady) {
				GetExpression(Context);
			}
			if (expression != null)
				return CalculateCore(expression, cell, context);
			if(binaryFormula != null)
				return CalculateCore(binaryFormula, cell, context);
			return VariantValue.Missing; 
		}
		protected virtual VariantValue CalculateCore(ParsedExpression expression, ICell cell, WorkbookDataContext context) {
			return FormulaCalculator.Calculate(expression, cell, context);
		}
		protected virtual VariantValue CalculateCore(byte[] binaryFormula, ICell cell, WorkbookDataContext context) {
			return FormulaCalculator.Calculate(binaryFormula, BinaryFormulaOffset, cell, context);
		}
		#endregion
		#region GetInvolvedCellRanges
		public override List<CellRangeBase> GetPrecedents(ICell cell) {
			ParsedExpression expression = GetExpression(Context);
			WorkbookDataContext context = cell.Context;
			context.PushCurrentCell(cell);
			try {
				return Expression.GetInvolvedCellRanges(context);
			}
			finally {
				context.PopCurrentCell();
			}
		}
		public override List<PrecedentPair> GetInvolvedCellRanges(ICell cell) {
			ParsedExpression expression = GetExpression(Context);
			if (expression == null)
				return new List<PrecedentPair>();
			WorkbookDataContext context = cell.Context;
			context.PushCurrentCell(cell);
			try {
				return GetInvolvedCellRangesCore(cell);
			}
			finally {
				context.PopCurrentCell();
			}
		}
		protected virtual List<PrecedentPair> GetInvolvedCellRangesCore(ICell cell) {
			return CalculateChainPrecedents(cell.Context, cell);
		}
		protected List<PrecedentPair> CalculateChainPrecedents(WorkbookDataContext context, IChainPrecedent precedent) {
			List<CellRangeBase> involvedRanges = Expression.GetInvolvedCellRanges(context);
			List<PrecedentPair> result = new List<PrecedentPair>();
			PrepareChainPrecedents(involvedRanges, result, precedent);
			return result;
		}
		void PrepareChainPrecedents(List<CellRangeBase> ranges, IList<PrecedentPair> result, IChainPrecedent precedent) {
			foreach (CellRangeBase innerRange in ranges) {
				if (innerRange.RangeType == CellRangeType.UnionRange) {
					PrepareChainPrecedents(((CellUnion)innerRange).InnerCellRanges, result, precedent);
				}
				else {
					CellRange range = (CellRange)innerRange;
					result.Add(new PrecedentPair(range, precedent));
				}
			}
		}
		#endregion
		#region Notifications
		protected internal override void UpdateFormula(ReplaceThingsPRNVisitor walker, ICell ownerCell) {
			ParsedExpression preparedExpression = GetExpression(Context);
			if (expression == null)
				return;
			expression = walker.Process(preparedExpression);
			if (walker.FormulaChanged) {
				binaryFormula = null;
				ApplyDataToCellTransacted(ownerCell);
			}
		}
		#endregion
		public override void PushSettingsToContext(WorkbookDataContext context, ICell cell) {
		}
		public override void PopSettingsFromContext(WorkbookDataContext context) {
		}
		#region CheckIntegrity
		public override void CheckIntegrity() {
			Debug.Assert(expression != null || binaryFormula != null);
		}
		public override void CheckIntegrity(ICell cell) {
			CheckIntegrity();
		}
		#endregion
	}
	#endregion
	#region FormulaCalculator
	public static partial class FormulaCalculator {
		#region static
		static NotExistingFunction notExistingFunction = new NotExistingFunction("UNKNOWN");
		public static NotExistingFunction NotExistingFunction { get { return notExistingFunction; } }
		public static readonly int FuncSumCode = GetFunctionByInvariantName("SUM").Code;
		public static readonly int FuncRTDCode = GetFunctionByInvariantName("RTD").Code;
		public static readonly int FuncAndCode = GetFunctionByInvariantName("AND").Code;
		public static readonly int FuncOrCode = GetFunctionByInvariantName("OR").Code;
		public static readonly int FuncSubtotalCode = GetFunctionByInvariantName("SUBTOTAL").Code;
		public static readonly int FuncAggregateCode = GetFunctionByInvariantName("AGGREGATE").Code;
		[ThreadStatic]
		static BuildInFunctionProvider functionProvider;
		[ThreadStatic]
		static CustomFunctionProvider customFunctionProvider;
		public static BuildInFunctionProvider FunctionProvider {
			get {
				if (functionProvider == null)
					functionProvider = new BuildInFunctionProvider(CreateFunctionsTable());
				return functionProvider;
			}
		}
		internal static CustomFunctionProvider CustomFunctionProvider {
			get {
				if (customFunctionProvider == null)
					customFunctionProvider = new CustomFunctionProvider();
				return customFunctionProvider;
			}
		}
		#region CreateFunctionTable
		static Dictionary<string, ISpreadsheetFunction> CreateFunctionsTable() {
			Dictionary<string, ISpreadsheetFunction> result = new Dictionary<string, ISpreadsheetFunction>(StringExtensions.ComparerInvariantCultureIgnoreCase);
			AddCompatibilityFunctions(result);
			AddCubeFunctions(result);
			AddDatabaseFunctions(result);
			AddDateTimeFunctions(result);
			AddEngineeringFunctions(result);
			AddFinancialFunctions(result);
			AddInformationalFunctions(result);
			AddLogicalFunctions(result);
			AddLookupAndReferenceFunctions(result);
			AddMathAndTrigonometryFunctions(result);
			AddStatisticalFunctions(result);
			AddTextFunctions(result);
			AddUserDefinedFunctions(result);
			AddWebFunctions(result);
			AddInternalFunctions(result);
			return result;
		}
		static void AddFunction(Dictionary<string, ISpreadsheetFunction> where, ISpreadsheetFunction function) {
			System.Diagnostics.Debug.Assert(function.Name == function.Name.ToUpperInvariant());
			where.Add(function.Name, function);
		}
#endregion
		public static ISpreadsheetFunction GetFunctionByInvariantName(string name) {
			ISpreadsheetFunction function = FunctionProvider.GetFunctionByInvariantName(name);
			if (function != null)
				return function;
			function = CustomFunctionProvider.GetFunctionByInvariantName(name);
			if (function != null)
				return function;
			return new NotExistingFunction(name);
		}
		public static ISpreadsheetFunction GetFunctionByInvariantName(string name, WorkbookDataContext context) {
			ISpreadsheetFunction function = context.Workbook.BuiltInOverridesFunctionProvider.GetFunctionByInvariantName(name);
			if (function != null)
				return function;
			function = FunctionProvider.GetFunctionByInvariantName(name);
			if (function != null)
				return function;
			function = CustomFunctionProvider.GetFunctionByInvariantName(name);
			if (function != null)
				return function;
			function = context.Workbook.CustomFunctionProvider.GetFunctionByInvariantName(name);
			if (function != null)
				return function;
			return new NotExistingFunction(name);
		}
		public static string GetFunctionName(string invariantName, WorkbookDataContext context) {
			if (context.Workbook.BuiltInOverridesFunctionProvider.IsFunctionRegistered(invariantName))
				return context.Workbook.BuiltInOverridesFunctionProvider.GetFunctionName(invariantName, context);
			if (FunctionProvider.IsFunctionRegistered(invariantName))
				return FunctionProvider.GetFunctionName(invariantName, context);
			if (CustomFunctionProvider.IsFunctionRegistered(invariantName))
				return CustomFunctionProvider.GetFunctionName(invariantName, context);
			return context.Workbook.CustomFunctionProvider.GetFunctionName(invariantName, context);
		}
		public static string GetLocalizedFunctionName(string invariantName, WorkbookDataContext context) {
			if (context.Workbook.BuiltInOverridesFunctionProvider.IsFunctionRegistered(invariantName))
				return context.Workbook.BuiltInOverridesFunctionProvider.GetLocalizedFunctionName(invariantName, context);
			if (FunctionProvider.IsFunctionRegistered(invariantName))
				return FunctionProvider.GetLocalizedFunctionName(invariantName, context);
			if (CustomFunctionProvider.IsFunctionRegistered(invariantName))
				return CustomFunctionProvider.GetLocalizedFunctionName(invariantName, context);
			return context.Workbook.CustomFunctionProvider.GetLocalizedFunctionName(invariantName, context);
		}
		public static ISpreadsheetFunction GetFunctionByName(string name, WorkbookDataContext context) {
			ISpreadsheetFunction function = context.Workbook.BuiltInOverridesFunctionProvider.GetFunctionByName(name, context);
			if (function != null)
				return function;
			function = FunctionProvider.GetFunctionByName(name, context);
			if (function != null)
				return function;
			function = CustomFunctionProvider.GetFunctionByName(name, context);
			if (function != null)
				return function;
			function = context.Workbook.CustomFunctionProvider.GetFunctionByName(name, context);
			if (function != null)
				return function;
			return new NotExistingFunction(name);
		}
		public static ISpreadsheetFunction GetFunctionByCode(int code) {
			return FunctionProvider.GetFunctionByCode(code);
		}
		public static bool IsFunctionRegistered(string name) {
			return FunctionProvider.IsFunctionRegistered(name);
		}
		public static string GetFunctionParameterName(XtraSpreadsheetFunctionNameStringId stringId, string invariantName, WorkbookDataContext context) {
			switch (context.Workbook.BehaviorOptions.FunctionNameCulture) {
				default:
				case FunctionNameCulture.English:
					return invariantName;
				case FunctionNameCulture.Local:
				case FunctionNameCulture.Auto:
					string result = XtraSpreadsheetFunctionNameLocalizer.GetString(stringId);
					if (String.IsNullOrEmpty(result))
						result = invariantName;
					return result;
			}
		}
		public static string GetCalculateModeName(XtraSpreadsheetFunctionNameStringId stringId, string invariantName, WorkbookDataContext context) {
			return GetFunctionParameterName(stringId, invariantName, context);
		}
#endregion
		public static VariantValue Calculate(byte[] binaryFormula, int startIndex, ICell cell, WorkbookDataContext context) {
			Guard.ArgumentNotNull(binaryFormula, "binaryFormula");
			VariantValue result;
			context.PushCurrentCell(cell);
			try {
				result = context.RPNContext.EvaluateBinaryExpression(binaryFormula, startIndex, context);
				if (context.Workbook.CalculationChain.Enabled || !context.ArrayFormulaProcessing)
					result = context.DereferenceValue(result, true);
			}
			finally {
				context.PopCurrentCell();
			}
			return result;
		}
		public static VariantValue Calculate(ParsedExpression expression, ICell cell, WorkbookDataContext context) {
			Guard.ArgumentNotNull(expression, "expression");
			VariantValue result;
			context.PushCurrentCell(cell);
			try {
				result = expression.Evaluate(context);
				if (context.Workbook.CalculationChain.Enabled || !context.ArrayFormulaProcessing)
					result = context.DereferenceValue(result, true);
			}
			finally {
				context.PopCurrentCell();
			}
			return result;
		}
	}
#endregion
#region FunctionProvider
	public abstract class FunctionProvider {
#region Fields
		CultureInfo lastCulture;
		readonly Dictionary<string, ISpreadsheetFunction> functionsByInvariantName;
		Dictionary<string, ISpreadsheetFunction> functionsByLocalizedName;
		Dictionary<ISpreadsheetFunction, string> localizedFunctionNames;
#endregion
		protected FunctionProvider(Dictionary<string, ISpreadsheetFunction> functions) {
			functionsByInvariantName = functions;
		}
		protected FunctionProvider() {
			functionsByInvariantName = new Dictionary<string, ISpreadsheetFunction>(StringExtensions.ComparerInvariantCultureIgnoreCase);
		}
#region Properties
		public Dictionary<string, ISpreadsheetFunction> FunctionsByInvariantName { get { return functionsByInvariantName; } }
		protected internal Dictionary<string, ISpreadsheetFunction> FunctionsByLocalizedName { get { return functionsByLocalizedName; } set { functionsByLocalizedName = value; } }
		protected internal Dictionary<ISpreadsheetFunction, string> LocalizedFunctionNames { get { return localizedFunctionNames; } set { localizedFunctionNames = value; } }
		protected internal CultureInfo LastCulture { get { return lastCulture; } set { lastCulture = value; } }
#endregion
		public bool IsFunctionRegistered(string name) {
			return functionsByInvariantName.ContainsKey(name);
		}
		Dictionary<string, ISpreadsheetFunction> GetLocalizedFunctionNameTable(WorkbookDataContext context) {
			CultureInfo culture = context.Culture;
			if (context.Culture == CultureInfo.InvariantCulture)
				return functionsByInvariantName;
			PrepareLocalizationTablesForCulture(culture);
			return functionsByLocalizedName;
		}
		protected internal void PrepareLocalizationTablesForCulture(CultureInfo culture) {
			if (lastCulture != culture) {
				functionsByLocalizedName = null;
				lastCulture = culture;
			}
			if (functionsByLocalizedName != null)
				return;
			functionsByLocalizedName = new Dictionary<string, ISpreadsheetFunction>(StringExtensions.ComparerInvariantCultureIgnoreCase);
			localizedFunctionNames = new Dictionary<ISpreadsheetFunction, string>();
			FillLocalizedTables(functionsByLocalizedName, localizedFunctionNames, culture);
		}
		protected internal abstract void FillLocalizedTables(Dictionary<string, ISpreadsheetFunction> functionsByLocalizedName, Dictionary<ISpreadsheetFunction, string> localizedFunctionNames, CultureInfo culture);
		public ISpreadsheetFunction GetFunctionByInvariantName(string name) {
			ISpreadsheetFunction function;
			if (functionsByInvariantName.TryGetValue(name, out function))
				return function;
			return null;
		}
		public string GetFunctionName(string invariantName, WorkbookDataContext context) {
			switch (context.Workbook.BehaviorOptions.FunctionNameCulture) {
				default:
				case FunctionNameCulture.English:
					return invariantName;
				case FunctionNameCulture.Local: 
				case FunctionNameCulture.Auto:
					string result = GetLocalizedFunctionName(invariantName, context);
					if (String.IsNullOrEmpty(result))
						result = invariantName;
					return result;
			}
		}
		public string GetLocalizedFunctionName(string invariantName, WorkbookDataContext context) {
			ISpreadsheetFunction function = GetFunctionByInvariantName(invariantName);
			if (function == null || context.Culture == CultureInfo.InvariantCulture)
				return invariantName;
			PrepareLocalizationTablesForCulture(context.Culture);
			string name;
			if (!localizedFunctionNames.TryGetValue(function, out name))
				return invariantName;
			else
				return name;
		}
		public ISpreadsheetFunction GetFunctionByName(string name, WorkbookDataContext context) {
			if (context.ImportExportMode) {
				if (functionsByInvariantName.ContainsKey(name))
					return functionsByInvariantName[name];
				return null;
			}
			ISpreadsheetFunction function;
			switch (context.Workbook.BehaviorOptions.FunctionNameCulture) {
				default:
				case FunctionNameCulture.English:
					if (functionsByInvariantName.TryGetValue(name, out function))
						return function;
					break;
				case FunctionNameCulture.Local:
					if (GetLocalizedFunctionNameTable(context).TryGetValue(name, out function))
						return function;
					break;
				case FunctionNameCulture.Auto:
					if (GetLocalizedFunctionNameTable(context).TryGetValue(name, out function))
						return function;
					if (functionsByInvariantName.TryGetValue(name, out function))
						return function;
					break;
			}
			return null;
		}
	}
#endregion
#region BuildInFunctionProvider
	public class BuildInFunctionProvider : FunctionProvider {
		Dictionary<int, ISpreadsheetFunction> functionsByCode;
		public BuildInFunctionProvider(Dictionary<string, ISpreadsheetFunction> functions)
			: base(functions) {
			functionsByCode = CreateFunctionsByCodeTable();
		}
		Dictionary<int, ISpreadsheetFunction> CreateFunctionsByCodeTable() {
			Dictionary<int, ISpreadsheetFunction> result = new Dictionary<int, ISpreadsheetFunction>();
			foreach (ISpreadsheetFunction function in FunctionsByInvariantName.Values)
				result.Add(function.Code, function);
			result.Add(FormulaCalculator.NotExistingFunction.Code, FormulaCalculator.NotExistingFunction);
			return result;
		}
		public ISpreadsheetFunction GetFunctionByCode(int code) {
			ISpreadsheetFunction function;
			if (functionsByCode.TryGetValue(code, out function))
				return function;
			return FormulaCalculator.NotExistingFunction;
		}
		protected internal override void FillLocalizedTables(Dictionary<string, ISpreadsheetFunction> functionsByLocalizedName, Dictionary<ISpreadsheetFunction, string> localizedFunctionNames, CultureInfo culture) {
			foreach (XtraSpreadsheetFunctionNameStringId stringId in Enum.GetValues(typeof(XtraSpreadsheetFunctionNameStringId))) {
				int code = (int)stringId;
				if ((code & 0x2000) == 0) { 
					ISpreadsheetFunction function;
					if (!functionsByCode.TryGetValue(code, out function))
						continue;
					string name = XtraSpreadsheetFunctionNameResLocalizer.GetString(stringId, culture);
					if (!String.IsNullOrEmpty(name)) {
						if (functionsByLocalizedName.ContainsKey(name)) {
							string cultureName = culture == System.Globalization.CultureInfo.InvariantCulture ? "Invariant" : culture.Name;
							string message = string.Format("A function with the \"{0}\" name already exists in the localization library for the {1} culture. Function names should be unique.", name, cultureName);
							throw new ArgumentException(message);
						}
						functionsByLocalizedName.Add(name, function);
						localizedFunctionNames.Add(function, name);
					}
				}
			}
		}
		protected internal string GetFunctionNameForCulture(ISpreadsheetFunction function, CultureInfo culture) {
			if (function == null || culture == CultureInfo.InvariantCulture)
				return function.Name;
			XtraSpreadsheetFunctionNameStringId stringId = (XtraSpreadsheetFunctionNameStringId)function.Code;
			return XtraSpreadsheetFunctionNameResLocalizer.GetString(stringId, culture);
		}
	}
	#endregion
#if DXPORTABLE
	public
#else
	internal
#endif
		interface ICustomFunctionProvider : DevExpress.Office.ISupportsCopyFrom<ICustomFunctionProvider> {
		ISpreadsheetFunction GetFunctionByName(string name, WorkbookDataContext context);
		string GetFunctionName(string invariantName, WorkbookDataContext context);
		string GetLocalizedFunctionName(string invariantName, WorkbookDataContext context);
		ISpreadsheetFunction GetFunctionByInvariantName(string name);
		bool RegisterFunction(string name, CustomFunction function, CultureInfo culture);
		bool UnregisterFunction(string name);
		bool IsFunctionRegistered(string name);
		Dictionary<string, Model.ISpreadsheetFunction> FunctionsByInvariantName { get; }
		void ClearFunctions();
	}
#region CustomFunctionProvider
	internal class CustomFunctionProvider : FunctionProvider, ICustomFunctionProvider {
		Dictionary<string, ISpreadsheetFunction> ICustomFunctionProvider.FunctionsByInvariantName { get { return base.FunctionsByInvariantName; ; } }
		protected internal override void FillLocalizedTables(Dictionary<string, ISpreadsheetFunction> functionsByLocalizedName, Dictionary<ISpreadsheetFunction, string> localizedFunctionNames, CultureInfo culture) {
			foreach (ISpreadsheetFunction function in FunctionsByInvariantName.Values) {
				CustomFunction customFunction = function as CustomFunction;
				if (customFunction == null)
					continue;
				FillLocalizationTables(customFunction, functionsByLocalizedName, localizedFunctionNames, culture);
			}
		}
		void FillLocalizationTables(CustomFunction function, Dictionary<string, ISpreadsheetFunction> functionsByLocalizedName, Dictionary<ISpreadsheetFunction, string> localizedFunctionNames, CultureInfo culture) {
			string name = function.NativeCustomFunction.GetName(culture);
			if (String.IsNullOrEmpty(name))
				name = function.Name;
			if (!functionsByLocalizedName.ContainsKey(name))
				functionsByLocalizedName.Add(name, function);
			if (!localizedFunctionNames.ContainsKey(function))
				localizedFunctionNames.Add(function, name);
		}
		public bool RegisterFunction(string name, CustomFunction function, CultureInfo culture) {
			if (FunctionsByInvariantName.ContainsKey(name))
				return false;
			FunctionsByInvariantName.Add(name, function);
			if (LastCulture != culture) {
				FunctionsByLocalizedName = null;
				LocalizedFunctionNames = null;
			}
			else {
				if (FunctionsByLocalizedName != null)
					FillLocalizationTables(function, FunctionsByLocalizedName, LocalizedFunctionNames, culture);
			}
			return true;
		}
		public bool UnregisterFunction(string name) {
			if (String.IsNullOrEmpty(name))
				return false;
			ISpreadsheetFunction function;
			if (!FunctionsByInvariantName.TryGetValue(name, out function))
				return false;
			if (FunctionsByLocalizedName != null)
				FunctionsByLocalizedName.Remove(name);
			if (LocalizedFunctionNames != null)
				LocalizedFunctionNames.Remove(function);
			return FunctionsByInvariantName.Remove(name);
		}
		void ICustomFunctionProvider.ClearFunctions() {
			FunctionsByInvariantName.Clear();
			FunctionsByLocalizedName = null;
			LocalizedFunctionNames = null;
		}
#region ISupportsCopyFrom<ICustomFunctionProvider> Members
		public void CopyFrom(ICustomFunctionProvider value) {
			foreach (KeyValuePair<string, ISpreadsheetFunction> pair in value.FunctionsByInvariantName)
				FunctionsByInvariantName.Add(pair.Key, pair.Value);
		}
#endregion
	}
#endregion
#region CalculationsInfo
	public class CalculationsInfo {
		readonly Dictionary<ICell, CellValueCalculationInfo> cellValueCalculations = new Dictionary<ICell, CellValueCalculationInfo>();
		bool isCycleDetected;
		public bool IsCycleDetected { get { return isCycleDetected; } set { isCycleDetected = value; } }
		public CellValueCalculationInfo Add(ICell cell) {
			CellValueCalculationInfo result = new CellValueCalculationInfo();
			cellValueCalculations.Add(cell, result);
			return result;
		}
		public void Remove(ICell cell) {
			cellValueCalculations.Remove(cell);
			if (cellValueCalculations.Count == 0)
				isCycleDetected = false;
		}
		public CellValueCalculationInfo TryGetCalculationInfo(ICell cell) {
			CellValueCalculationInfo result;
			if (!cellValueCalculations.TryGetValue(cell, out result))
				return null;
			else
				return result;
		}
	}
#endregion
#region CellValueCalculationInfo
	public class CellValueCalculationInfo {
		int iterationCount;
		double iterationDelta;
		public int IterationCount { get { return iterationCount; } set { iterationCount = value; } }
		public double IterationDelta { get { return iterationDelta; } set { iterationDelta = value; } }
	}
#endregion
#region FormulaFactory
	public static class FormulaFactory {
		public static bool GetFormulaCalculateAlways(ICell cell) {
			if (!cell.HasFormula)
				return false;
			return PackedValues.GetBoolBitValue(cell.FormulaInfo.BinaryFormula[0], FormulaBase.CalculateAlwaysMask);
		}
		public static void SetFormulaCalculateAlways(ICell cell, bool value) {
			if (!cell.HasFormula)
				return;
			byte formulaFlags = cell.FormulaInfo.BinaryFormula[0];
			PackedValues.SetBoolBitValue(ref formulaFlags, FormulaBase.CalculateAlwaysMask, value);
			cell.FormulaInfo.BinaryFormula[0] = formulaFlags;
		}
		public static FormulaType GetFormulaType(ICell cell) {
			if (!cell.HasFormula)
				return FormulaType.None;
			byte[] binaryFormula = cell.FormulaInfo.BinaryFormula;
			Debug.Assert(binaryFormula != null && binaryFormula.Length > 0);
			int typeCode = binaryFormula[0] & FormulaBase.FormulaTypeMask;
			return (FormulaType)typeCode;
		}
		public static FormulaBase CreateInstance(ICell cell) {
			FormulaType formulaType = GetFormulaType(cell);
			return CreateInstanceCore(cell, formulaType);
		}
		public static FormulaBase CreateInstanceCore(ICell cell, FormulaType formulaType) {
			FormulaBase result;
			switch (formulaType) {
				case FormulaType.Normal:
					result = new Formula(cell);
					break;
				case FormulaType.Array:
					result = new ArrayFormula(cell);
					break;
				case FormulaType.ArrayPart:
					result = new ArrayFormulaPart(cell);
					break;
				case FormulaType.Shared:
					result = new SharedFormulaRef(cell);
					break;
				default:
					throw new ArgumentException("Unknown formula type");
			}
			result.InitializeFromBinary(cell.FormulaInfo.BinaryFormula, cell.Worksheet);
			return result;
		}
		public static Formula CreateTableFormulaInstance(ICell cell, byte[] binaryFormula, Worksheet sheet) {
			Debug.Assert(binaryFormula != null && binaryFormula.Length > 0);
			int typeCode = binaryFormula[0] & FormulaBase.FormulaTypeMask;
			Formula result;
			switch (typeCode) {
				case 0:
					result = new Formula(cell);
					break;
				case 1:
					result = new ArrayFormula(cell);
					break;
				default:
					throw new ArgumentException("Unknown formula type");
			}
			result.InitializeFromBinary(binaryFormula, sheet);
			return result;
		}
	}
#endregion
#region CustomFunctionsDescriptions
	public class CustomFunctionsDescriptions : Dictionary<string, CustomFunctionDescription> {}
#endregion
#region CustomFunctionDescription
	public class CustomFunctionDescription {
		public string Description { get; set; }
		public List<string> ParametersName { get; private set; }
		public List<string> ParametersDescription { get; private set; }
		public List<string> ReturnTypes { get; private set; }
		public CustomFunctionDescription() {
			ParametersName = new List<string>();
			ParametersDescription = new List<string>();
			ReturnTypes = new List<string>();
		}
	}
#endregion
}
