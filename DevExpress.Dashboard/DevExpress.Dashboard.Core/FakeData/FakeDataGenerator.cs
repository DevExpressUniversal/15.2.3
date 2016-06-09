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
namespace DevExpress.DashboardCommon.Native {
	abstract class FakeDataGeneratorBase {
		public int UniqueCount { get; set; }
		public string FieldName { get; protected set; }
		public int SpreadParam { get; set; }
		public int ShuffleParam { get; set; }
		public FakeDataGeneratorBase(int uniqueCount) {
			this.UniqueCount = uniqueCount;
		}
		public abstract IEnumerable<object> GetValues();
		public static FakeDataGeneratorBase MakeGenerator(Type dataType, int uniqueCount, string fieldName) {
			FakeDataGeneratorBase obj = null;
			if (dataType == typeof(string))
				obj = new StringDataGenerator(uniqueCount, fieldName);
			else if (dataType == typeof(int))
				obj = new IntDataGenerator(uniqueCount, fieldName);
			else if (dataType == typeof(float))
				obj = new FloatDataGenerator(uniqueCount, fieldName);
			else if (dataType == typeof(double))
				obj = new DoubleDataGenerator(uniqueCount, fieldName);
			else if (dataType == typeof(decimal))
				obj = new DecimalDataGenerator(uniqueCount, fieldName);
			else if (dataType == typeof(DateTime))
				obj = new DateTimeDataGenerator(uniqueCount, fieldName);
			else if (dataType == typeof(bool))
				obj = new BoolDataGenerator(uniqueCount, fieldName);
			else
				obj = new StubDataGenerator(uniqueCount, fieldName);
			return obj;
		}
	}
	abstract class FakeDataGenerator<T> : FakeDataGeneratorBase {
		List<T> values = new List<T>();
		protected int CurrGeneratedCount { get; set; }
		public FakeDataGenerator(int uniqueCount, string fieldName)
			: base(uniqueCount) {
			this.FieldName = fieldName;
			this.CurrGeneratedCount = 0;
		}
		public override IEnumerable<object> GetValues() {
			if (CurrGeneratedCount == 0)
				InitilaizeValue();
			return values.Select(x => (object)x);
		}
		protected abstract T GetNextValue();
		void InitilaizeValue() {
			List<T> generatedValues = new List<T>();
			for (int i = 0; i < UniqueCount; i++) {
				generatedValues.Add(GetNextValue());
				CurrGeneratedCount++;
			}
			IEnumerable<T> shuffledValues = Helper.ShuffleEnum(generatedValues, ShuffleParam);
			values.AddRange(shuffledValues);
		}
	}
	abstract class NumericDataGenerator : FakeDataGenerator<double> {
		protected abstract Type NumericType { get; }
		public NumericDataGenerator(int uniqueCount, string fieldName) : base(uniqueCount, fieldName) { }
		public override IEnumerable<object> GetValues() {
			IEnumerable<object> result = base.GetValues();
			foreach (double value in result) {
				if (NumericType == typeof(float))
					yield return (float)value;
				else if (NumericType == typeof(decimal))
					yield return (decimal)value;
				else if (NumericType == typeof(double))
					yield return value;
				else if (NumericType == typeof(int))
					yield return (int)Math.Round(value);
			}
		}
	}
	abstract class SimpleNumericDataGenerator : NumericDataGenerator {		
		public SimpleNumericDataGenerator(int uniqueCount, string fieldName) : base(uniqueCount, fieldName) { }
		protected int NumericOffset { get { return 5 * UniqueCount + 3 * SpreadParam; } }
		protected int NextNumericValue { get { return CurrGeneratedCount + 1; } }
		protected override double GetNextValue() {
			return NumericOffset + NextNumericValue * 0.99;
		}
	}
	class IntDataGenerator : SimpleNumericDataGenerator {
		protected override Type NumericType { get { return typeof(int);} }
		public IntDataGenerator(int uniqueCount, string fieldName) : base(uniqueCount, fieldName) { }		
	}
	class FloatDataGenerator : SimpleNumericDataGenerator {
		protected override Type NumericType { get { return typeof(float); } }
		public FloatDataGenerator(int uniqueCount, string fieldName) : base(uniqueCount, fieldName) { }
	}
	class DoubleDataGenerator : SimpleNumericDataGenerator {
		protected override Type NumericType { get { return typeof(double); } }
		public DoubleDataGenerator(int uniqueCount, string fieldName) : base(uniqueCount, fieldName) { }
	}
	class DecimalDataGenerator : SimpleNumericDataGenerator {
		protected override Type NumericType { get { return typeof(decimal); } }
		public DecimalDataGenerator(int uniqueCount, string fieldName) : base(uniqueCount, fieldName) { }
	}
	class DateTimeDataGenerator : FakeDataGenerator<DateTime> {
		DateTime start { get { return new DateTime(DateTime.Now.Year - 2, 1, 1); } }
		public DateTimeDataGenerator(int uniqueCount, string fieldName) : base(uniqueCount, fieldName) { }
		protected override DateTime GetNextValue() {
			if (start.AddYears(NextYear()).AddMonths(NextMonth()).Year == DateTime.Now.Year)
				throw new Exception();
			return start.AddYears(NextYear()).AddMonths(NextMonth()).AddDays(NextDay()).
				AddHours(NextHour()).AddMinutes(NextMinute()).AddSeconds(NextSecond());
		}
		int GetSubStepNumber(int partCount) {
			return CurrGeneratedCount % partCount;
		}
		int GetLongStepNumber(int skipCount) {
			return CurrGeneratedCount / skipCount;
		}
		int NextYear() {
			return CurrGeneratedCount < (UniqueCount / 2) ? 0 : 1;
		}
		int NextMonth() {
			int quartComponent = 3 * GetSubStepNumber(4);
			int monthComponent = GetLongStepNumber(4) % 3;
			int val = quartComponent + monthComponent;
			return val;
		}
		int NextDay() {
			return GetSubStepNumber(28);
		}
		int NextHour() {
			return GetSubStepNumber(24);
		}
		int NextMinute() {
			return GetSubStepNumber(60);
		}
		int NextSecond() {
			return GetSubStepNumber(60);
		}
	}
	class StringDataGenerator : FakeDataGenerator<string> {
		public StringDataGenerator(int uniqueCount, string fieldName) : base(uniqueCount, fieldName) { }
		protected override string GetNextValue() {
			return FieldName + (CurrGeneratedCount + 1).ToString();
		}
	}
	class BoolDataGenerator : FakeDataGenerator<bool> {
		public BoolDataGenerator(int uniqueCount, string fieldName) : base(uniqueCount, fieldName) { }
		protected override bool GetNextValue() {
			return CurrGeneratedCount % 2 == 0;
		}
	}
	class StubDataGenerator : FakeDataGenerator<object> {
		public StubDataGenerator(int uniqueCount, string fieldName) : base(uniqueCount, fieldName) { }
		protected override object GetNextValue() {
			return null;
		}
	}
	class StringListDataGenerator : FakeDataGenerator<string> {
		readonly string[] names;
		public StringListDataGenerator(IEnumerable<string> names, string fieldName)
			: base(names.Count(), fieldName) {
			this.names = names.ToArray();
		}
		protected override string GetNextValue() {
			return names[CurrGeneratedCount];
		}
	}
	class RangeDataGenerator : NumericDataGenerator {
		readonly double max;
		readonly double min;
		readonly Type numericType;
		protected override Type NumericType { get { return numericType; } }
		public RangeDataGenerator(double min, double max, int uniqueCount, Type numericType, string fieldName)
			: base(uniqueCount, fieldName) {
			this.min = min;
			this.max = max;
			this.numericType = numericType;
		}
		public RangeDataGenerator(double min, double max, int uniqueCount, Type numericType)
			: this(min, max, uniqueCount, numericType, "") {
		}
		protected override double GetNextValue() {
			double rangeFactor = 1 - Convert.ToDouble(SpreadParam % 10) / 11;
			double range = (max - min) * rangeFactor;
			return min + (CurrGeneratedCount + 1) * range / (UniqueCount + 1);
		}
		public override IEnumerable<object> GetValues() {
			foreach (object value in base.GetValues())
				if (NumericType == typeof(int))
					yield return Math.Max((int)Math.Round(min) + 1, Math.Min((int)value, (int)Math.Round(max) - 1));
				else
					yield return value;
		}
	}
}
