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
using System.Text;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor.Executors {
	class ConvertTypeExecutor : ExecutorBase<ConvertType> {
		DataVectorBase inputVector;
		DataVectorBase resultVector;
		ConverterCalculatorBase calculator;
		public ConvertTypeExecutor(ConvertType operation, IExecutorQueueContext context)
			: base(operation, context) {
			this.inputVector = context.GetSingleFlowExecutorResultVector(operation.Argument);
			this.resultVector = DataVectorBase.New(operation.ResultType, context.VectorSize);
			this.calculator = CreateCalculator(operation.Argument.OperationType, operation.ResultType);
			this.calculator.InitializeCalculator(inputVector, resultVector);
			this.Result = new SingleFlowExecutorResult(resultVector);
		}
		ConverterCalculatorBase CreateCalculator(Type argumentType, Type resultType) {
			Func<Type, Type, ConverterCalculatorBase> create = (converterType, objectConverterType) => {
				if(argumentType == typeof(object))
					return (ConverterCalculatorBase)Activator.CreateInstance(objectConverterType);
				else
					return GenericActivator.New<ConverterCalculatorBase>(converterType, argumentType);
			};
			if(resultType == typeof(DateTime)) return create(typeof(ConverterToDateTime<>), typeof(ConverterObjectToDateTime));
			if(resultType == typeof(Boolean)) return create(typeof(ConverterToBool<>), typeof(ConverterObjectToBool));
			if(resultType == typeof(Byte)) return create(typeof(ConverterToByte<>), typeof(ConverterObjectToByte));
			if(resultType == typeof(Char)) return create(typeof(ConverterToChar<>), typeof(ConverterObjectToChar));
			if(resultType == typeof(Decimal)) return create(typeof(ConverterToDecimal<>), typeof(ConverterObjectToDecimal));
			if(resultType == typeof(Double)) return create(typeof(ConverterToDouble<>), typeof(ConverterObjectToDouble));
			if(resultType == typeof(Int16)) return create(typeof(ConverterToInt16<>), typeof(ConverterObjectToInt16));
			if(resultType == typeof(Int32)) return create(typeof(ConverterToInt32<>), typeof(ConverterObjectToInt32));
			if(resultType == typeof(Int64)) return create(typeof(ConverterToInt64<>), typeof(ConverterObjectToInt64));
			if(resultType == typeof(SByte)) return create(typeof(ConverterToSByte<>), typeof(ConverterObjectToSByte));
			if(resultType == typeof(Single)) return create(typeof(ConverterToSingle<>), typeof(ConverterObjectToSingle));
			if(resultType == typeof(String)) return create(typeof(ConverterToString<>), typeof(ConverterObjectToString));
			if(resultType == typeof(UInt16)) return create(typeof(ConverterToUInt16<>), typeof(ConverterObjectToUInt16));
			if(resultType == typeof(UInt32)) return create(typeof(ConverterToUInt32<>), typeof(ConverterObjectToUInt32));
			if(resultType == typeof(UInt64)) return create(typeof(ConverterToUInt64<>), typeof(ConverterObjectToUInt64));
			throw new NotSupportedException();
		}
		protected override ExecutorProcessResult Process() {
			calculator.Process();
			return ExecutorProcessResult.ResultReady;
		}
	}
	abstract class ConverterCalculatorBase {
		public abstract void Process();
		public abstract void InitializeCalculator(DataVectorBase inputVector, DataVectorBase resultVector);
	}
	abstract class ConverterCalculator<TIn, TOut> : ConverterCalculatorBase {
		DataVector<TIn> inputVector;
		DataVector<TOut> resultVector;
		public override void InitializeCalculator(DataVectorBase inputVector, DataVectorBase resultVector) {
			this.inputVector = (DataVector<TIn>)inputVector;
			this.resultVector = (DataVector<TOut>)resultVector;
		}
		public override void Process() {
			resultVector.Count = inputVector.Count;
			for(int i = 0; i < inputVector.Count; i++)
				if(inputVector.SpecialData[i] == SpecialDataValue.None)
					resultVector.Data[i] = Convert(inputVector.Data[i]);
				else
					resultVector.SpecialData[i] = inputVector.SpecialData[i];
		}
		protected abstract TOut Convert(TIn value);
	}
	class ConverterToDecimal<TIn> : ConverterCalculator<TIn, decimal> where TIn : IConvertible {
		protected override decimal Convert(TIn value) {
			return value.ToDecimal(null);
		}
	}
	class ConverterToDouble<TIn> : ConverterCalculator<TIn, Double> where TIn : IConvertible {
		protected override Double Convert(TIn value) {
			return value.ToDouble(null);
		}
	}
	class ConverterToDateTime<TIn> : ConverterCalculator<TIn, DateTime> where TIn : IConvertible {
		protected override DateTime Convert(TIn value) {
			return value.ToDateTime(null);
		}
	}
	class ConverterToBool<TIn> : ConverterCalculator<TIn, Boolean> where TIn : IConvertible {
		protected override Boolean Convert(TIn value) {
			return value.ToBoolean(null);
		}
	}
	class ConverterToByte<TIn> : ConverterCalculator<TIn, Byte> where TIn : IConvertible {
		protected override Byte Convert(TIn value) {
			return value.ToByte(null);
		}
	}
	class ConverterToChar<TIn> : ConverterCalculator<TIn, Char> where TIn : IConvertible {
		protected override Char Convert(TIn value) {
			return value.ToChar(null);
		}
	}
	class ConverterToInt16<TIn> : ConverterCalculator<TIn, Int16> where TIn : IConvertible {
		protected override Int16 Convert(TIn value) {
			return value.ToInt16(null);
		}
	}
	class ConverterToInt32<TIn> : ConverterCalculator<TIn, Int32> where TIn : IConvertible {
		protected override Int32 Convert(TIn value) {
			return value.ToInt32(null);
		}
	}
	class ConverterToInt64<TIn> : ConverterCalculator<TIn, Int64> where TIn : IConvertible {
		protected override Int64 Convert(TIn value) {
			return value.ToInt64(null);
		}
	}
	class ConverterToSByte<TIn> : ConverterCalculator<TIn, SByte> where TIn : IConvertible {
		protected override SByte Convert(TIn value) {
			return value.ToSByte(null);
		}
	}
	class ConverterToSingle<TIn> : ConverterCalculator<TIn, Single> where TIn : IConvertible {
		protected override Single Convert(TIn value) {
			return value.ToSingle(null);
		}
	}
	class ConverterToString<TIn> : ConverterCalculator<TIn, String> where TIn : IConvertible {
		protected override string Convert(TIn value) {
			return value.ToString(null);
		}
	}
	class ConverterToUInt16<TIn> : ConverterCalculator<TIn, UInt16> where TIn : IConvertible {
		protected override UInt16 Convert(TIn value) {
			return value.ToUInt16(null);
		}
	}
	class ConverterToUInt32<TIn> : ConverterCalculator<TIn, UInt32> where TIn : IConvertible {
		protected override UInt32 Convert(TIn value) {
			return value.ToUInt32(null);
		}
	}
	class ConverterToUInt64<TIn> : ConverterCalculator<TIn, UInt64> where TIn : IConvertible {
		protected override UInt64 Convert(TIn value) {
			return value.ToUInt64(null);
		}
	}
	abstract class ConverterObjectTo<TOut> : ConverterCalculator<object, TOut> {
		protected abstract TOut ConvertInterface(IConvertible value);
		protected override TOut Convert(object value) {
			return ConvertInterface((IConvertible)value);
		}
	}
	class ConverterObjectToDecimal : ConverterObjectTo<decimal> {
		protected override decimal ConvertInterface(IConvertible value) {
			return value.ToDecimal(null);
		}
	}
	class ConverterObjectToDouble : ConverterObjectTo<Double> {
		protected override Double ConvertInterface(IConvertible value) {
			return value.ToDouble(null);
		}
	}
	class ConverterObjectToBool : ConverterObjectTo<Boolean> {
		protected override Boolean ConvertInterface(IConvertible value) {
			return value.ToBoolean(null);
		}
	}
	class ConverterObjectToDateTime : ConverterObjectTo<DateTime> {
		protected override DateTime ConvertInterface(IConvertible value) {
			return value.ToDateTime(null);
		}
	}
	class ConverterObjectToByte : ConverterObjectTo<Byte> {
		protected override Byte ConvertInterface(IConvertible value) {
			return value.ToByte(null);
		}
	}
	class ConverterObjectToChar : ConverterObjectTo<Char> {
		protected override Char ConvertInterface(IConvertible value) {
			return value.ToChar(null);
		}
	}
	class ConverterObjectToInt16 : ConverterObjectTo<Int16> {
		protected override Int16 ConvertInterface(IConvertible value) {
			return value.ToInt16(null);
		}
	}
	class ConverterObjectToInt32 : ConverterObjectTo<Int32> {
		protected override Int32 ConvertInterface(IConvertible value) {
			return value.ToInt32(null);
		}
	}
	class ConverterObjectToInt64 : ConverterObjectTo<Int64> {
		protected override Int64 ConvertInterface(IConvertible value) {
			return value.ToInt64(null);
		}
	}
	class ConverterObjectToSByte : ConverterObjectTo<SByte> {
		protected override SByte ConvertInterface(IConvertible value) {
			return value.ToSByte(null);
		}
	}
	class ConverterObjectToSingle : ConverterObjectTo<Single> {
		protected override Single ConvertInterface(IConvertible value) {
			return value.ToSingle(null);
		}
	}
	class ConverterObjectToString : ConverterObjectTo<String> {
		protected override string ConvertInterface(IConvertible value) {
			return value.ToString(null);
		}
	}
	class ConverterObjectToUInt16 : ConverterObjectTo<UInt16> {
		protected override UInt16 ConvertInterface(IConvertible value) {
			return value.ToUInt16(null);
		}
	}
	class ConverterObjectToUInt32 : ConverterObjectTo<UInt32> {
		protected override UInt32 ConvertInterface(IConvertible value) {
			return value.ToUInt32(null);
		}
	}
	class ConverterObjectToUInt64 : ConverterObjectTo<UInt64> {
		protected override UInt64 ConvertInterface(IConvertible value) {
			return value.ToUInt64(null);
		}
	}
}
