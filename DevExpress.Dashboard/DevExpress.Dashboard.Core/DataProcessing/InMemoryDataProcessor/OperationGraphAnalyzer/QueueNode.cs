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
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor {
	public interface IQueueNode {
		IEnumerable<CustomOperation> SortedOperations { get; }
		IEnumerable<IQueueNode> PreviousNodes { get; }
		IEnumerable<IQueueNode> NextNodes { get; }
	}
	public class QueueNodeTopologicalComparer : IComparer<IQueueNode> {
		public int Compare(IQueueNode x, IQueueNode y) {
			if(x.PreviousNodes.Contains(y))
				return 1;
			if(y.PreviousNodes.Contains(x))
				return -1;
			return 0;
		}
	}
	class QueueNode : IQueueNode {
		HashSet<CustomOperation> queueOperations;
		List<CustomOperation> sortedOperations;
		HashSet<QueueNode> previousNodes;
		HashSet<QueueNode> nextNodes;
		public IEnumerable<CustomOperation> SortedOperations { get { return sortedOperations; } }
		public IEnumerable<CustomOperation> QueueOperations { get { return queueOperations; } }
		public IEnumerable<IQueueNode> PreviousNodes { get { return previousNodes; } }
		public IEnumerable<IQueueNode> NextNodes { get { return nextNodes; } }
		public QueueNode() {
			this.previousNodes = new HashSet<QueueNode>();
			this.nextNodes = new HashSet<QueueNode>();
			this.queueOperations = new HashSet<CustomOperation>();
		}
		public void AddQueueOperation(CustomOperation operation) {
			queueOperations.Add(operation);
		}
		public void RegisterPreviousNode(QueueNode node) {
			previousNodes.Add(node);
			node.nextNodes.Add(this);
		}
		public void MergeWith(QueueNode node) {
			queueOperations = new HashSet<CustomOperation>(queueOperations.Concat(node.queueOperations));
			previousNodes = new HashSet<QueueNode>(previousNodes.Concat(node.previousNodes));
			nextNodes = new HashSet<QueueNode>(nextNodes.Concat(node.nextNodes));
			foreach(var n in node.previousNodes) {
				n.nextNodes.Remove(node);
				n.nextNodes.Add(this);
			}
			foreach(var n in node.nextNodes) {
				n.previousNodes.Remove(node);
				n.previousNodes.Add(this);
			}
			node.queueOperations = null;
			node.previousNodes = null;
			node.nextNodes = null;
		}
		public void QueueSort() {
			sortedOperations = QueryPlanAnalyzer.TopologicalSort(queueOperations).ToList();
		}
	}
}
