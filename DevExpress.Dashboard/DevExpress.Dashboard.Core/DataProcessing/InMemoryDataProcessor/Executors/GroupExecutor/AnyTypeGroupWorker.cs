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
	abstract class GroupWorkerBase {
		public abstract DataVectorBase[] GroupedData { get; }
		public abstract DataVector<int> GroupIndexes { get; }
		public abstract int GroupCount { get; }
		public abstract void Process();
	}
	class AnyTypeGroupWorker : GroupWorkerBase {
		int groupCount;
		GroupedColumnBase[] columns;
		DataVector<int> groupIndexes;
		DataVectorBase[] groupedData;
		DataVectorBase firstInputVector;
		Dictionary<DataTuple, int> groups;
		public override DataVectorBase[] GroupedData { get { return groupedData; } }
		public override DataVector<int> GroupIndexes { get { return groupIndexes; } }
		public override int GroupCount { get { return groupCount; } }
		public AnyTypeGroupWorker(DataVectorBase[] inputVectors) {
			this.groupCount = 0;
			this.firstInputVector = inputVectors[0];
			this.columns = inputVectors.Select(v => NewColumn(v.DataType, v)).ToArray();
			this.groupIndexes = new DataVector<int>(DataProcessingOptions.DefaultGroupIndexesSize);
			this.groups = new Dictionary<DataTuple, int>(new DataTupleEqualityComparer(this.columns));
			this.groupedData = columns.Select(c => c.Buffer).ToArray();
		}
		GroupedColumnBase NewColumn(Type type, DataVectorBase vector) {
			return GenericActivator.New<GroupedColumnBase>(typeof(GroupedColumn<>), type, vector);
		}
		public override void Process() {
			for(int i = 0; i < firstInputVector.Count; i++) {
				int testIndex = -1;
				int currentGroupIndex;
				for(int j = 0; j < columns.Length; j++)
					testIndex = columns[j].SelectTestValue(i);
				DataTuple testTuple = new DataTuple(testIndex);
				if(!groups.TryGetValue(testTuple, out currentGroupIndex)) {
					currentGroupIndex = groupCount;
					groups.Add(testTuple, currentGroupIndex);
					groupCount++;
					for(int j = 0; j < columns.Length; j++)
						columns[j].AddTestValueToStorage();
				}
				if(groupIndexes.Count == groupIndexes.MaxCount)
					groupIndexes.Extend();
				groupIndexes.Data[groupIndexes.Count] = currentGroupIndex;
				groupIndexes.Count++;
			}
		}
	}
}
