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
using System.Globalization;
using System.Text;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraRichEdit.Model {
	#region MathTokenType
	public enum MathTokenType {
		Constant,
		Operator
	}
	#endregion
	#region OperationPriority
	public enum OperationPriority {
		Lowest,
		Low,
		Medium,
		High,
		Highest
	}
	#endregion
	#region OperationType
	public enum OperationType {
		Addition,
		Subtraction,
		Multiplication,
		Division,
		Power,
		Percentage,
		Negation
	}
	#endregion
	#region Association
	public enum Association {
		Left,
		Right
	}
	#endregion
	#region IMathToken
	public interface IMathToken {
		MathTokenType Type { get; }
	}
	#endregion
	#region OperationBase (abstract class)
	public abstract class OperationBase : IMathToken {
		protected OperationBase() {
		}
		public abstract OperationType Type { get; }
		public abstract OperationPriority Priority { get; }
		public abstract Association Association { get; }
		#region IMathToken Members
		MathTokenType IMathToken.Type { get { return MathTokenType.Operator; } }
		#endregion
		public override string ToString() {
			return Type.ToString();
		}
	}
	#endregion
	#region AdditionOperation
	public class AdditionOperation : OperationBase {
		public override OperationPriority Priority { get { return OperationPriority.Low; } }
		public override OperationType Type { get { return OperationType.Addition; } }
		public override Association Association { get { return Association.Left; } }
	}
	#endregion
	#region SubtractionOperation
	public class SubtractionOperation : OperationBase {
		public override OperationPriority Priority { get { return OperationPriority.Low; } }
		public override OperationType Type { get { return OperationType.Subtraction; } }
		public override Association Association { get { return Association.Left; } }
	}
	#endregion
	#region MultiplicationOperation
	public class MultiplicationOperation : OperationBase {
		public override OperationPriority Priority { get { return OperationPriority.Medium; } }
		public override OperationType Type { get { return OperationType.Multiplication; } }
		public override Association Association { get { return Association.Left; } }
	}
	#endregion
	#region DivisionOperation
	public class DivisionOperation : OperationBase {
		public override OperationPriority Priority { get { return OperationPriority.Medium; } }
		public override OperationType Type { get { return OperationType.Division; } }
		public override Association Association { get { return Association.Left; } }
	}
	#endregion
	#region PowerOperation
	public class PowerOperation : OperationBase {
		public override OperationPriority Priority { get { return OperationPriority.High; } }
		public override OperationType Type { get { return OperationType.Power; } }
		public override Association Association { get { return Association.Right; } }
	}
	#endregion
	#region PercentageOperation
	public class PercentageOperation : OperationBase {
		public override OperationPriority Priority { get { return OperationPriority.Highest; } }
		public override OperationType Type { get { return OperationType.Percentage; } }
		public override Association Association { get { return Association.Right; } }
	}
	#endregion
	#region NegationOperation
	public class NegationOperation : OperationBase {
		public override OperationPriority Priority { get { return OperationPriority.Highest; } }
		public override OperationType Type { get { return OperationType.Negation; } }
		public override Association Association { get { return Association.Right; } }
	}
	#endregion
	#region Constant
	public class Constant : IMathToken {
		readonly double value;
		public Constant(double value) {
			this.value = value;
		}
		public double Value { get { return value; } }
		#region IMathToken Members
		MathTokenType IMathToken.Type { get { return MathTokenType.Constant; } }
		#endregion
		public override string ToString() {
			return String.Format("Constant: {0}", this.value);
		}
	}
	#endregion
	public class MathTokenCollection : List<IMathToken> { }
	public class OperationStack : Stack<OperationBase> { }
	#region InfixNotationParserState
	public enum InfixNotationParserState {
		Start,
		Constant,
		AfterOperand,
		AfterPercentage,
		WaitOperator,
		Operator,
		Operand,
		OpenBracket,
		CloseBracket
	}
	#endregion
	#region ParsingErrorType
	public enum ParsingErrorType {
		Syntax,
		MissingOperation
	}
	#endregion
	#region InfixNotationParser
	public class InfixNotationParser {
		#region Fields
		CultureInfo culture;
		OperationStack currentExpression;
		MathTokenCollection tokens;
		InfixNotationParserStateBase state;
		Stack<OperationStack> outerExpressions;
		#endregion
		public InfixNotationParser(CultureInfo culture) {
			Guard.ArgumentNotNull(culture, "culture");
			this.culture = culture;
			this.state = new StartInfixNotationParserState(this);
		}
		#region Properties
		public InfixNotationParserStateBase State { get { return state; } }
		public CultureInfo Culture { get { return culture; } }
		#endregion
		public virtual MathTokenCollection Parse(string notation) {
			MathTokenCollection result;
			BeginParse();
			try {
				ParseCore(notation);
			}
			finally {
				EndParse(out result);
			}
			return result;
		}
		protected internal virtual void ParseCore(string notation) {
			int index = 0;
			int notationLength = notation.Length;
			while (index < notationLength) {
				if (this.state.ProcessChar(notation, index))
					index++;
			}
		}
		protected internal virtual void BeginParse() {
			this.tokens = new MathTokenCollection();
			this.currentExpression = new OperationStack();
			this.outerExpressions = new Stack<OperationStack>();
		}
		protected internal virtual void EndParse(out MathTokenCollection result) {
			AddTokensFromCurrentExpression();
			result = this.tokens;
			this.tokens = null;
			this.currentExpression = null;
			EnsureOuterExpressionsAreFinished();
			this.outerExpressions = null;
		}
		void EnsureOuterExpressionsAreFinished() {
			if (this.outerExpressions.Count > 0)
				FieldFormatter.ThrowUnexpectedEndOfFormulaError();
		}
		protected internal virtual void ChangeState(InfixNotationParserState type) {
			switch (type) {
				case InfixNotationParserState.Start:
					this.state = new StartInfixNotationParserState(this);
					break;
				case InfixNotationParserState.Constant:
					this.state = new ConstantInfixNotationParserState(this);
					break;
				case InfixNotationParserState.AfterOperand:
					this.state = new AfterOperandInfixNotationParserState(this);
					break;
				case InfixNotationParserState.AfterPercentage:
					this.state = new AfterPercentageInfixNotationParserState(this);
					break;
				case InfixNotationParserState.WaitOperator:
					this.state = new WaitOperatorInfixNotationParserState(this);
					break;
				case InfixNotationParserState.Operator:
					this.state = new OperatorInfixNotationParserState(this);
					break;
				case InfixNotationParserState.Operand:
					this.state = new OperandInfixNotationParserState(this);
					break;
				case InfixNotationParserState.OpenBracket:
					this.state = new OpenBracketInfixNotationParserState(this);
					break;
				case InfixNotationParserState.CloseBracket:
					this.state = new CloseBracketInfixNotationParserState(this);
					break;
				default:
					Exceptions.ThrowInternalException();
					break;
			}
		}
		protected internal virtual void AddOperation(OperationBase operation) {
			while (this.currentExpression.Count > 0 && ShouldEjectOperation(operation))
				AddToken(this.currentExpression.Pop());
			this.currentExpression.Push(operation);
		}
		protected internal virtual bool ShouldEjectOperation(OperationBase operation) {
			if (operation.Association == Association.Left)
				return GetTopOperator().Priority >= operation.Priority;
			else
				return GetTopOperator().Priority > operation.Priority;
		}
		protected internal virtual OperationBase GetTopOperator() {
			return this.currentExpression.Peek();
		}
		protected internal virtual void AddToken(IMathToken token) {
			this.tokens.Add(token);
		}
		protected internal virtual void AddTokensFromCurrentExpression() {
			while (this.currentExpression.Count > 0)
				AddToken(this.currentExpression.Pop());
		}
		protected internal virtual void StartInnerExpression() {
			this.outerExpressions.Push(this.currentExpression);
			this.currentExpression = new OperationStack();
		}
		protected internal virtual void EndInnerExpression() {
			AddTokensFromCurrentExpression();
			if (this.outerExpressions.Count > 0)
				this.currentExpression = this.outerExpressions.Pop();
			else
				FieldFormatter.ThrowUnexpectedEndOfFormulaError();
		}
	}
	#endregion
	#region InfixNotationParserStateBase (abstract class)
	public abstract class InfixNotationParserStateBase {
		static List<char> delimiters = CreateDelimitersTable();
		static List<char> CreateDelimitersTable() {
			List<char> result = new List<char>();
			result.Add(' ');
			result.Add('\t');
			result.Add('\n');
			result.Add('\r');
			result.Add(Characters.EmSpace);
			result.Add(Characters.EnSpace);
			result.Add(Characters.QmSpace);
			return result;
		}
		readonly InfixNotationParser parser;
		protected InfixNotationParserStateBase(InfixNotationParser parser) {
			Guard.ArgumentNotNull(parser, "parser");
			this.parser = parser;
		}
		public InfixNotationParser Parser { get { return parser; } }
		public CultureInfo Culture { get { return Parser.Culture; } }
		public abstract bool ProcessChar(string notation, int index);
		protected void ChangeState(InfixNotationParserState type) {
			Parser.ChangeState(type);
		}
		protected bool IsDelimiter(char ch) {
			return delimiters.Contains(ch);
		}
	}
	#endregion
	#region StartInfixNotationParserState
	public class StartInfixNotationParserState : InfixNotationParserStateBase {
		public StartInfixNotationParserState(InfixNotationParser parser)
			: base(parser) {
		}
		public override bool ProcessChar(string notation, int index) {
			if (IsDelimiter(notation[index]))
				return true;
			if (!TryChangeState(notation[index])) {
				FieldFormatter.ThrowSyntaxError(notation[index]);
			}
			return false;
		}
		protected internal virtual bool TryChangeState(char ch) {
			char decimalSeparator = Culture.NumberFormat.NumberDecimalSeparator[0];
			if (ch == '-') {
				ChangeState(InfixNotationParserState.Operand);
				return true;
			}
			if (Char.IsDigit(ch) || ch == decimalSeparator) {
				ChangeState(InfixNotationParserState.Constant);
				return true;
			}
			if (ch == '(') {
				ChangeState(InfixNotationParserState.OpenBracket);
				return true;
			}
			return false;
		}
	}
	#endregion
	#region ConstantInfixNotationParserState
	public class ConstantInfixNotationParserState : InfixNotationParserStateBase {
		readonly StringBuilder number;
		public ConstantInfixNotationParserState(InfixNotationParser parser)
			: base(parser) {
			this.number = new StringBuilder();
		}
		public override bool ProcessChar(string notation, int index) {
			char ch = notation[index];
			char decimalSeparator = Culture.NumberFormat.NumberDecimalSeparator[0];
			if (Char.IsDigit(ch) || ch == decimalSeparator) {
				this.number.Append(ch);
				if (index == notation.Length - 1)
					AddConstantToken();
				return true;
			}
			else {
				AddConstantToken();
				ChangeState(InfixNotationParserState.AfterOperand);
			}
			return false;
		}
		protected internal virtual void AddConstantToken() {
			Parser.AddToken(new Constant(Double.Parse(this.number.ToString(), Culture)));
		}
	}
	#endregion
	#region AfterOperandInfixNotationParserState
	public class AfterOperandInfixNotationParserState : InfixNotationParserStateBase {
		public AfterOperandInfixNotationParserState(InfixNotationParser parser)
			: base(parser) {
		}
		public override bool ProcessChar(string notation, int index) {
			if (IsDelimiter(notation[index]))
				return true;
			if (Char.IsDigit(notation[index])) {
				FieldFormatter.ThrowMissingOperatorError();
			}
			else
				ChangeState(notation[index]);
			return false;
		}
		protected internal virtual void ChangeState(char ch) {
			if (ch == ')')
				ChangeState(InfixNotationParserState.CloseBracket);
			else
				ChangeState(InfixNotationParserState.WaitOperator);
		}
	}
	#endregion
	#region AfterPercentageInfixNotationParserState
	public class AfterPercentageInfixNotationParserState : AfterOperandInfixNotationParserState {
		public AfterPercentageInfixNotationParserState(InfixNotationParser parser)
			: base(parser) {
		}
		public override bool ProcessChar(string notation, int index) {
			if (notation[index] == '%') {
				FieldFormatter.ThrowSyntaxError(notation[index]);
				return false;
			}
			return base.ProcessChar(notation, index);
		}
	}
	#endregion
	#region OperandInfixNotationParserState
	public class OperandInfixNotationParserState : InfixNotationParserStateBase {
		public OperandInfixNotationParserState(InfixNotationParser parser)
			: base(parser) {
		}
		public override bool ProcessChar(string notation, int index) {
			if (IsDelimiter(notation[index]))
				return true;
			if (notation[index] == '-') {
				Parser.AddOperation(new NegationOperation());
				return true;
			}
			else if (!TryChangeState(notation[index])) {
				FieldFormatter.ThrowSyntaxError(notation[index]);
			}
			return false;
		}
		protected internal virtual bool TryChangeState(char ch) {
			char decimalSeparator = Culture.NumberFormat.NumberDecimalSeparator[0];
			if (Char.IsDigit(ch) || ch == decimalSeparator) {
				ChangeState(InfixNotationParserState.Constant);
				return true;
			}
			if (ch == '(') {
				ChangeState(InfixNotationParserState.OpenBracket);
				return true;
			}
			return false;
		}
	}
	#endregion
	#region WaitOperatorInfixNotationParserState
	public class WaitOperatorInfixNotationParserState : InfixNotationParserStateBase {
		public WaitOperatorInfixNotationParserState(InfixNotationParser parser)
			: base(parser) {
		}
		public override bool ProcessChar(string notation, int index) {
			char ch = notation[index];
			if (IsDelimiter(ch)) {
				ChangeState(InfixNotationParserState.Operand);
				return true;
			}
			if (Char.IsSymbol(ch) || Char.IsPunctuation(ch)) {
				ChangeState(InfixNotationParserState.Operator);
				return false;
			}
			else {
				FieldFormatter.ThrowSyntaxError(ch);
				return false;
			}
		}
	}
	#endregion
	#region OperatorInfixNotationParserState
	public class OperatorInfixNotationParserState : InfixNotationParserStateBase {
		#region Operations Table
		static Dictionary<string, OperationType> operations = CreateOperaqtionTable();
		static Dictionary<string, OperationType> CreateOperaqtionTable() {
			Dictionary<string, OperationType> result = new Dictionary<string, OperationType>();
			result.Add("*", OperationType.Multiplication);
			result.Add("+", OperationType.Addition);
			result.Add("-", OperationType.Subtraction);
			result.Add("/", OperationType.Division);
			result.Add("%", OperationType.Percentage);
			result.Add("^", OperationType.Power);
			return result;
		}
		#endregion
		readonly StringBuilder operation;
		public OperatorInfixNotationParserState(InfixNotationParser parser)
			: base(parser) {
			this.operation = new StringBuilder();
		}
		public override bool ProcessChar(string notation, int index) {
			char ch = notation[index];
			if (Char.IsSymbol(ch) || Char.IsPunctuation(ch))
				TryAddOperation(ch);
			else {
				FieldFormatter.ThrowSyntaxError(this.operation[0]);
				return false;
			}
			return true;
		}
		protected internal virtual void TryAddOperation(char ch) {
			this.operation.Append(ch);
			OperationBase operation = CreateOperation(this.operation.ToString());
			if (operation != null) {
				Parser.AddOperation(operation);
				ChangeStateByOperationType(operation.Type);
			}
		}
		protected internal virtual void ChangeStateByOperationType(OperationType type) {
			if (type == OperationType.Percentage)
				ChangeState(InfixNotationParserState.AfterPercentage);
			else
				ChangeState(InfixNotationParserState.Operand);
		}
		protected internal virtual OperationBase CreateOperation(string operation) {
			if (operations.ContainsKey(operation))
				return CreateOperationCore(operations[operation]);
			return null;
		}
		protected internal virtual OperationBase CreateOperationCore(OperationType type) {
			switch (type) {
				case OperationType.Addition:
					return new AdditionOperation();
				case OperationType.Subtraction:
					return new SubtractionOperation();
				case OperationType.Multiplication:
					return new MultiplicationOperation();
				case OperationType.Division:
					return new DivisionOperation();
				case OperationType.Power:
					return new PowerOperation();
				case OperationType.Percentage:
					return new PercentageOperation();
				default:
					Exceptions.ThrowInternalException();
					return null;
			}
		}
	}
	#endregion
	#region OpenBracketInfixNotationParserState
	public class OpenBracketInfixNotationParserState : InfixNotationParserStateBase {
		public OpenBracketInfixNotationParserState(InfixNotationParser parser)
			: base(parser) {
		}
		public override bool ProcessChar(string notation, int index) {
			Parser.StartInnerExpression();
			ChangeState(InfixNotationParserState.Start);
			return true;
		}
	}
	#endregion
	#region CloseBracketInfixNotationParserState
	public class CloseBracketInfixNotationParserState : InfixNotationParserStateBase {
		public CloseBracketInfixNotationParserState(InfixNotationParser parser)
			: base(parser) {
		}
		public override bool ProcessChar(string notation, int index) {
			Parser.EndInnerExpression();
			ChangeState(InfixNotationParserState.AfterOperand);
			return true;
		}
	}
	#endregion
	#region MathematicalCalculator
	public class MathematicalCalculator {
		static Dictionary<OperationType, OperationHandler> operations = CreateOperationsTable();
		#region CreateOperationsTable
		static Dictionary<OperationType, OperationHandler> CreateOperationsTable() {
			Dictionary<OperationType, OperationHandler> result = new Dictionary<OperationType, OperationHandler>();
			result.Add(OperationType.Addition, ProcessAdditionOperation);
			result.Add(OperationType.Division, ProcessDivisionOperation);
			result.Add(OperationType.Multiplication, ProcessMultiplicationOperation);
			result.Add(OperationType.Negation, ProcessNegationOperation);
			result.Add(OperationType.Percentage, ProcessPercentageOperation);
			result.Add(OperationType.Power, ProcessPowerOperation);
			result.Add(OperationType.Subtraction, ProcessSubstractionOperation);
			return result;
		}
		static void ProcessAdditionOperation(Stack<double> operands) {
			SetOperand(GetOperand(operands) + GetOperand(operands), operands);
		}
		static void ProcessSubstractionOperation(Stack<double> operands) {
			double secondOperand = GetOperand(operands);
			double firstOperand = GetOperand(operands);
			SetOperand(firstOperand - secondOperand, operands);
		}
		static void ProcessMultiplicationOperation(Stack<double> operands) {
			SetOperand(GetOperand(operands) * GetOperand(operands), operands);
		}
		static void ProcessDivisionOperation(Stack<double> operands) {
			double secondOperand = GetOperand(operands);
			double firstOperand = GetOperand(operands);
			double result = firstOperand / secondOperand;
			if (!Double.IsInfinity(result))
				SetOperand(result, operands);
			else
				FieldFormatter.ThrowZeroDivideError();
		}
		static void ProcessPowerOperation(Stack<double> operands) {
			double secondOperand = GetOperand(operands);
			double firstOperand = GetOperand(operands);
			SetOperand(Math.Pow(firstOperand, secondOperand), operands);
		}
		static void ProcessPercentageOperation(Stack<double> operands) {
			SetOperand(GetOperand(operands) / 100, operands);
		}
		static void ProcessNegationOperation(Stack<double> operands) {
			SetOperand(-GetOperand(operands), operands);
		}
		static double GetOperand(Stack<double> operands) {
			if (operands.Count == 0)
				FieldFormatter.ThrowUnexpectedEndOfFormulaError();
			return operands.Pop();
		}
		static void SetOperand(double value, Stack<double> operands) {
			operands.Push(value);
		}
		#endregion
		protected delegate void OperationHandler(Stack<double> operands);
		public virtual double Calculate(string notation, CultureInfo culture) {
			double[] outputConstants = ProcessTokens(ParseInfixNotation(notation, culture));
			double result = 0;
			foreach (double value in outputConstants)
				result += value;
			return result;
		}
		MathTokenCollection ParseInfixNotation(string notation, CultureInfo culture) {
			InfixNotationParser parser = new InfixNotationParser(culture);
			return parser.Parse(notation);
		}
		double[] ProcessTokens(MathTokenCollection result) {
			Stack<double> operands = new Stack<double>();
			foreach (IMathToken token in result)
				ProcessToken(token, operands);
			return operands.ToArray();
		}
		void ProcessToken(IMathToken token, Stack<double> operands) {
			if (token.Type == MathTokenType.Operator) {
				OperationType type = ((OperationBase)token).Type;
				operations[type](operands);
			}
			else
				operands.Push(((Constant)token).Value);
		}
	}
	#endregion
}
