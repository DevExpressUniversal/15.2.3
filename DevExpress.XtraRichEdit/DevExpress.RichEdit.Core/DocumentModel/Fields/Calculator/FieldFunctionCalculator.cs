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
using DevExpress.XtraRichEdit.Model;
using System.Reflection;
namespace DevExpress.XtraRichEdit.Fields {
	public interface IFieldFunctionCalculatorFactory {
		IFunctionCalculator GetCalculator(string functionName, FormulaNodeBase[] arguments);
	}
	public interface IFunctionCalculator {
		double CalculateValue(FormulaNodeBase[] arguments);
		bool BeginFunctionResultIteration(PieceTable pieceTable, Field field, FormulaNodeBase[] arguments);
		bool MoveNextFunctionResult();
		void EndFunctionResultIteration();
	}
	public class FieldFunctionCalculatorFactory : IFieldFunctionCalculatorFactory {
		static readonly Object[] constructorParameters = new Object[0];
		static readonly Type[] constructorParameterTypes = new Type[0];
		Dictionary<string, ConstructorInfo> table;
		public FieldFunctionCalculatorFactory() {
			table = new Dictionary<string, ConstructorInfo>();
			PopulateFunctionTable();
		}
		protected virtual void PopulateFunctionTable() {
			AddOrReplaceCalculatorConstructor("SUM", typeof(SumFunctionCalculator));
		}
		protected virtual void AddOrReplaceCalculatorConstructor(string functionName, Type calculatorType) {
			table[functionName.ToUpperInvariant()] = calculatorType.GetConstructor(constructorParameterTypes);
		}
		protected virtual IFunctionCalculator CreateCalculator(string functionName, FormulaNodeBase[] arguments) {
			ConstructorInfo ci;
			if (table.TryGetValue(functionName.ToUpperInvariant(), out ci))
				return (IFunctionCalculator)ci.Invoke(constructorParameters);
			else
				return null;
		}
		public IFunctionCalculator GetCalculator(string functionName, FormulaNodeBase[] arguments) {
			return CreateCalculator(functionName, arguments);
		}
	}
	public abstract class ScalarFunctionCalculatorBase : IFunctionCalculator {
		FormulaNodeBase argument;
		public double CalculateValue(FormulaNodeBase[] arguments) {
			if (argument == null)
				return 0;
			return CalculateValueCore(argument.GetValue());
		}
		public abstract double CalculateValueCore(double argument);
		public bool BeginFunctionResultIteration(PieceTable pieceTable, Field field, FormulaNodeBase[] arguments) {
			if (arguments.Length < 1)
				return false;
			argument = arguments[0];
			return argument.BeginDataIteration(pieceTable, field);			
		}
		public bool MoveNextFunctionResult() {
			if (argument != null)
				return argument.MoveNext();
			else
				return false;
		}
		public void EndFunctionResultIteration() {
			if(argument != null)
				argument.EndDataIteration();
		}
	}
	public abstract class SummaryFunctionCalculatorBase : IFunctionCalculator {
		PieceTable pieceTable;
		Field field;
		protected abstract void AggregateValue(double value);
		protected abstract double GetAggregatedValue();
		protected abstract void ResetAggregatedValue();
		public virtual double CalculateValue(FormulaNodeBase[] arguments) {
			ResetAggregatedValue();
			foreach (FormulaNodeBase argument in arguments) {
				if (argument.BeginDataIteration(pieceTable, field)) {
					try {
						do {
							AggregateValue(argument.GetValue());
						} while (argument.MoveNext());
					}
					finally {
						argument.EndDataIteration();
					}
				}
			}
			return GetAggregatedValue();
		}
		public bool BeginFunctionResultIteration(PieceTable pieceTable, Field field, FormulaNodeBase[] arguments) {
			this.pieceTable = pieceTable;
			this.field = field;
			return true;
		}
		public bool MoveNextFunctionResult() {
			return false;
		}
		public void EndFunctionResultIteration() {
			pieceTable = null;
			field = null;
		}
	}
	public class SumFunctionCalculator : SummaryFunctionCalculatorBase {
		double sum;
		protected override void AggregateValue(double value) {
			sum += value;
		}
		protected override double GetAggregatedValue() {
			return sum;
		}
		protected override void ResetAggregatedValue() {
			sum = 0;
		}
	}
}
