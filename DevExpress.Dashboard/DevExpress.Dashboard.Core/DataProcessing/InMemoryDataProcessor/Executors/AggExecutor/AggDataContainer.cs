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
	public class AggDataContainer<TInput, TOutput> {
		int groupCount = 1;
		DataVector<int> groupIndexes = null;
		DataVector<TInput> inputVector;
		DataVector<TOutput> resultVector;
		public int GroupCount {
			get { return groupCount; }
			set { groupCount = value; }
		}
		public DataVector<int> GroupIndexes { get { return groupIndexes; } }
		public DataVector<TInput> InputVector { get { return inputVector; } }
		public DataVector<TOutput> ResultVector { get { return resultVector; } }
		public virtual void InitializeContainer(int groupCount, DataVector<int> groupIndexes, DataVectorBase values) {
			this.groupCount = groupCount;
			this.groupIndexes = groupIndexes;
			this.inputVector = (DataVector<TInput>)values;
			this.resultVector = new DataVector<TOutput>(groupCount);
			this.resultVector.Count = groupCount;
		}
		public virtual void Extend(int groupCount) {
			if (groupCount > ResultVector.MaxCount)
				ResultVector.Extend(groupCount);
			ResultVector.Count = groupCount;
			GroupCount = groupCount;
		}
		internal T[] Extend<T>(T[] extendeble, int count) {
			T[] extended = new T[groupCount];
			Array.Copy(extendeble, extended, extendeble.Length);
			return extended;
		}
	}
	public class MinMaxAggDataCointainer<T> : AggDataContainer<T, T> {
		Comparer<T> comparer;
		bool[] isAssign;
		public Comparer<T> Comparer { get { return comparer; } }
		public bool[] IsAssign { get { return isAssign; } }
		public override void InitializeContainer(int groupCount, DataVector<int> groupIndexes, DataVectorBase values) {
			base.InitializeContainer(groupCount, groupIndexes, values);
			this.comparer = ComparerFactory.Get<T>();
			this.isAssign = new bool[groupCount];
		}
		public override void Extend(int groupCount) {
			base.Extend(groupCount);
			isAssign = Extend(isAssign, groupCount);
		}
	}
	public class CountDAggDataCointainer<T> : AggDataContainer<T, int> {
		HashSet<T>[] distinctValues;
		public HashSet<T>[] DistinctValues { get { return distinctValues; } }
		public override void InitializeContainer(int groupCount, DataVector<int> groupIndexes, DataVectorBase values) {
			base.InitializeContainer(groupCount, groupIndexes, values);
			distinctValues = new HashSet<T>[groupCount];
			for (int i = 0; i < groupCount; i++)
				distinctValues[i] = new HashSet<T>(EqualityComparerFactory.Get<T>());
		}
		public bool Add(int resultIndex, T value) {
			return distinctValues[resultIndex].Add(value);
		}
		public override void Extend(int groupCount) {
			base.Extend(groupCount);
			HashSet<T>[] newDistinctValues = new HashSet<T>[groupCount];
			for (int i = 0; i < groupCount; i++) {
				if (i < distinctValues.Length)
					newDistinctValues[i] = distinctValues[i];
				else
					newDistinctValues[i] = new HashSet<T>(EqualityComparerFactory.Get<T>());
			}
			distinctValues = newDistinctValues;
		}
	}
	public class AverageAggDataCointainer<TInput, TOutput> : AggDataContainer<TInput, TOutput> {
		int[] counts;
		public int[] Counts { get { return counts; } }
		public override void InitializeContainer(int groupCount, DataVector<int> groupIndexes, DataVectorBase values) {
			base.InitializeContainer(groupCount, groupIndexes, values);
			counts = new int[groupCount];
		}
		public override void Extend(int groupCount) {
			base.Extend(groupCount);
			counts = Extend(counts, groupCount);
		}
	}
	public abstract class StdDevAndVarAggDataCointainer<T> : AggDataContainer<T, double> {
		decimal[] sum;
		double[] sqrSum;
		int[] counts;
		public abstract SummaryType Type { get; }
		public decimal[] Sum { get { return sum; } }
		public double[] SqrSum { get { return sqrSum; } }
		public int[] Counts { get { return counts; } }
		public override void InitializeContainer(int groupCount, DataVector<int> groupIndexes, DataVectorBase values) {
			base.InitializeContainer(groupCount, groupIndexes, values);
			sum = new decimal[groupCount];
			sqrSum = new double[groupCount];
			counts = new int[groupCount];
		}
		public override void Extend(int groupCount) {
			base.Extend(groupCount);
			sum = Extend(sum, groupCount);
			sqrSum = Extend(sqrSum, groupCount);
			counts = Extend(counts, groupCount);
		}
	}
	public class StdDevCointainer<T> : StdDevAndVarAggDataCointainer<T> {
		public override SummaryType Type { get { return SummaryType.StdDev; } }
	}
	public class StdDevpCointainer<T> : StdDevAndVarAggDataCointainer<T> {
		public override SummaryType Type { get { return SummaryType.StdDevp; } }
	}
	public class VarCointainer<T> : StdDevAndVarAggDataCointainer<T> {
		public override SummaryType Type { get { return SummaryType.Var; } }
	}
	public class VarpCointainer<T> : StdDevAndVarAggDataCointainer<T> {
		public override SummaryType Type { get { return SummaryType.Varp; } }
	}
}
