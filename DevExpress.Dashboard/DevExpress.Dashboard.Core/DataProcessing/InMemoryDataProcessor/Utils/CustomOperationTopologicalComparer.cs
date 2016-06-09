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
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor {
	public class CustomOperationTopologicalComparer : IComparer<CustomOperation> {
		struct AdjacencyNodesKey : IEquatable<AdjacencyNodesKey> {
			CustomOperation o1;
			CustomOperation o2;
			int hash;
			public AdjacencyNodesKey(CustomOperation o1, CustomOperation o2) {
				this.o1 = o1;
				this.o2 = o2;
				unchecked {
					int FNV_prime_32 = 16777619;
					int FNV_offset_basis_32 = (int)2166136261;
					hash = FNV_offset_basis_32;
					hash = (hash ^ o1.GetHashCode()) * FNV_prime_32;
					hash = (hash ^ o2.GetHashCode()) * FNV_prime_32;					
				}
			}
			public bool Equals(AdjacencyNodesKey other) {
				return o1.Equals(other.o1)
					&& o2.Equals(other.o2);
			}
			public override int GetHashCode() {
				return hash;
			}
		}
		Dictionary<AdjacencyNodesKey, int> adjacencyMatrix = new Dictionary<AdjacencyNodesKey, int>();
		public CustomOperationTopologicalComparer(IEnumerable<CustomOperation> allNodes) {
			foreach(CustomOperation operation in allNodes)
				foreach(CustomOperation operand in operation.Operands) {
					adjacencyMatrix[new AdjacencyNodesKey(operation, operand)] = 1;
					adjacencyMatrix[new AdjacencyNodesKey(operand, operation)] = -1;
				}
		}
		public int Compare(CustomOperation x, CustomOperation y) {
			int result;
			if(adjacencyMatrix.TryGetValue(new AdjacencyNodesKey(x, y), out result))
				return result;
			return 0;
		}
	}
}
