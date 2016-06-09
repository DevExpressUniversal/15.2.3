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
	abstract class GroupedColumnBase {
		public abstract DataVectorBase Buffer { get; }
		public abstract int SelectTestValue(int index);
		public abstract void AddTestValueToStorage();
		public abstract int ValueHashCode(int index);
		public abstract bool ValueEquals(int index1, int index2);
	}
	class GroupedColumn<T> : GroupedColumnBase {
		EqualityComparer<T> comparer;
		DataVector<T> inputVector;
		DataVector<T> buffer;
		public int Count { get { return buffer.Count; } }
		public override DataVectorBase Buffer { get { return buffer; } }
		public GroupedColumn(DataVector<T> vector) {
			this.inputVector = vector;
			this.buffer = new DataVector<T>(DataProcessingOptions.DefaultGroupStorageCapacity);
			this.comparer = EqualityComparerFactory.Get<T>();
		}
		public override int SelectTestValue(int index) {
			if(buffer.MaxCount - buffer.Count < 1)
				buffer.Extend();
			int testIndex = buffer.Count;
			buffer.Data[testIndex] = inputVector.Data[index];
			buffer.SpecialData[testIndex] = inputVector.SpecialData[index];
			return testIndex;
		}
		public override void AddTestValueToStorage() {
			buffer.Count++;
		}
		public override int ValueHashCode(int index) {
			SpecialDataValue specialValue = buffer.SpecialData[index];
			if(specialValue == SpecialDataValue.None)
				return comparer.GetHashCode(buffer.Data[index]);
			else
				return specialValue.GetHashCode();
		}
		public override bool ValueEquals(int index1, int index2) {
			SpecialDataValue spec1 = buffer.SpecialData[index1];
			SpecialDataValue spec2 = buffer.SpecialData[index2];
			if(spec1 == SpecialDataValue.None && spec2 == SpecialDataValue.None)
				return comparer.Equals(buffer.Data[index1], buffer.Data[index2]);
			else
				return spec1 == spec2;
		}
	}
}
