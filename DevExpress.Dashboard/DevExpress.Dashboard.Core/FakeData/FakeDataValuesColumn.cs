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
	class ValuesColumn {
		public virtual IEnumerable<ValuesContainer> Rows { get { return null; } }
		public ValuesColumn ProductIndependent(ValuesColumn second) {
			var values1 = this.GetProcessedValues(1, 1);
			var values2 = second.GetProcessedValues(1, 1);
			values2 = Helper.ShiftRight(values2, 1);
			values1 = Helper.EnsureLength(values1, values2.Count());
			return JoinColumnsEnum(values1, values2);
		}
		public ValuesColumn ProductSubcategory(ValuesColumn second, int groupCount) {
			var values1 = this.GetProcessedValues(groupCount, 1);
			var values2 = second.GetProcessedValues(1, groupCount);
			return JoinColumnsEnum(values1, values2);
		}
		public ValuesColumn ProductCartesian(ValuesColumn second) {
			var values1 = this.GetProcessedValues(1, 1);
			var values2 = second.GetProcessedValues(1, 1);
			int count1 = values1.Count();
			int count2 = values2.Count();
			values1 = Helper.EnsureLength(values1, count1 * count2);
			values2 = Helper.Duplicate(values2, count1);
			return JoinColumnsEnum(values1, values2);
		}
		public ValuesColumn DuplicateRows(int minCount, int maxCount) {
			RangeDataGenerator generator = new RangeDataGenerator(minCount, maxCount, Rows.Count(), typeof(int));
			IEnumerable<int> dupCounts = generator.GetValues().Cast<int>();
			return new ValuesColumnCustom(Helper.Duplicate(Rows, dupCounts));
		}
		protected virtual IEnumerable<ValuesContainer> GetProcessedValues(int dublicatesFactor, int uniqueFactor) {
			return Helper.Duplicate(Rows, dublicatesFactor);
		}
		ValuesColumn JoinColumnsEnum(IEnumerable<ValuesContainer> values1, IEnumerable<ValuesContainer> values2) {
			return new ValuesColumnCustom(values1.Zip(values2, (x, y) => x.JoinWith(y)));
		}
	}
	class ValuesColumnWithGenerator : ValuesColumn {
		List<ValuesContainer> column = new List<ValuesContainer>();
		FakeDataGeneratorBase generator;
		public override IEnumerable<ValuesContainer> Rows {
			get {
				if (column.Count == 0)
					foreach (object value in generator.GetValues())
						column.Add(new ValuesContainer(generator.FieldName, value));
				return column;
			}
		}
		public ValuesColumnWithGenerator(FakeDataGeneratorBase generator) {
			if (string.IsNullOrEmpty(generator.FieldName))
				throw new Exception("generator.FieldName must be defined");
			this.generator = generator;
		}
		protected override IEnumerable<ValuesContainer> GetProcessedValues(int dublicatesFactor, int uniqueFactor) {
			generator.UniqueCount *= uniqueFactor;
			return base.GetProcessedValues(dublicatesFactor, uniqueFactor);
		}
	}
	class ValuesColumnCustom : ValuesColumn {
		IEnumerable<ValuesContainer> columns;
		public override IEnumerable<ValuesContainer> Rows { get { return columns; } }
		public ValuesColumnCustom(IEnumerable<ValuesContainer> columns) {
			this.columns = columns;
		}
	}
}
