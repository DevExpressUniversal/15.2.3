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
	public class AddSurrogateOperationAlgorithm : IOptimazerAlgorithm {
		enum DataType {
			Surrogate,
			Materialize,
			Unknown
		}
		struct Edge {
			public DataFlowNode SurrogateScan;
			public DataFlowNode Start;
			public DataFlowNode End;
		}
		class DataFlowNode : OperationNode {
			DataType dataType = DataType.Unknown;
			DataFlowNode dataSource;
			public DataType DataType {
				get { return dataType; }
				set { dataType = value; }
			}
			public DataFlowNode DataSource {
				get { return dataSource; }
				set { dataSource = value; }
			}
			public DataFlowNode(CustomOperation operation) : base(operation) { }
			public bool IsSupportSurrogate() {
				if (Wrapped is GroupAggregate) {
					GroupAggregate groupAggregate = Wrapped as GroupAggregate;
					return groupAggregate.SummaryType == SummaryType.Count || groupAggregate.SummaryType == SummaryType.CountDistinct;
				}
				return !(Wrapped is Project) && !(Wrapped is ProjectOthers) && !(Wrapped is ProjectDateTime) && !(Wrapped is AggregateBase) && !(Wrapped is ConvertType);
			}
			public bool IsSurrogatePreferable() {
				if (Wrapped is Group || Wrapped is Sort || Wrapped is Join)
					return true;
				GroupAggregate aggr = Wrapped as GroupAggregate;
				if (aggr == null)
					return false;
				return aggr.SummaryType == SummaryType.Count || aggr.SummaryType == SummaryType.CountDistinct;
			}
			public bool DoesNotTransferDataTo(OperationNode other) {
				Join join = other.Wrapped as Join;
				SingleBlockOperation blockOperation = this.Wrapped as SingleBlockOperation;
				if (join != null) {
					if (blockOperation == null)
						return true;
					return !join.DataFlow.Contains(blockOperation);
				}
				else if (other.Wrapped is Select) {
					Select select = other.Wrapped as Select;
					return select.FilterFlow == this.Wrapped;
				}
				else
					return (this.Wrapped is Group && other.Wrapped is AggregateBase) || (this.Wrapped is Project && other.Wrapped is SelectScanBase) ||
						(this.Wrapped is Group && other.Wrapped is ExtractIndexes);
			}
			public int GetExpectIndex() {
				if (Wrapped is Extract)
					return (Wrapped as Extract).ResultNumber;
				return -1;
			}
			public List<int> GetOperationIndex(OperationNode topNode) {
				MultiBlockOperation blockOperation = Wrapped as MultiBlockOperation;
				List<int> indexs = new List<int>();
				if (blockOperation != null) {
					int index = 0;
					foreach (CustomOperation operand in blockOperation.Operands) {
						if (operand == topNode.Wrapped)
							indexs.Add(index);
						index++;
					}
				}
				Join join = Wrapped as Join;
				if (join != null) {
					int index = 0;
					foreach (CustomOperation operand in join.DataFlow) {
						if (operand == topNode.Wrapped)
							indexs.Add(index);
						index++;
					}
				}
				if (indexs.Count == 0)
					indexs.Add(-1);
				return indexs;
			}
		}
		List<CustomOperation> roots;
		DirectedGraph<DataFlowNode> graph;
		List<DataFlowNode> scansForReplace = new List<DataFlowNode>();
		List<Edge> edgesToAddMaterialize;
		public AddSurrogateOperationAlgorithm(List<CustomOperation> roots) {
			this.roots = roots;
		}
		void AddSurrogateOperation() {
			List<DataFlowNode> surrogatePreferableOperations = graph.Select(x => x.IsSurrogatePreferable());
			List<DataFlowNode> allScans = graph.FindStartNodes();
			foreach (DataFlowNode scan in allScans) {
				SingleFlowOperation sb = scan.Wrapped as SingleFlowOperation;
				if (sb != null && sb.OperationType == typeof(int))
					continue;
				foreach (DataFlowNode operation in surrogatePreferableOperations) {
					if (graph.HasPath(scan, operation, (x, y) => y.IsSupportSurrogate())) {
						scansForReplace.Add(scan);
						break;
					}
				}
			}
			if (scansForReplace.Count == 0)
				return;
			List<DataFlowNode> startNodes = graph.Select(x => (x.Wrapped is ScanBase) || (x.Wrapped is ConstScan));
			foreach (DataFlowNode start in startNodes) 
				TraceToFindEdgesForMaterialize(start, -1, DataType.Surrogate, start);
			foreach (Edge edge in edgesToAddMaterialize)
				TryInsertMaterialize(edge);
			ReplaceToSurrogateScans();
		}
		void TraceToFindEdgesForMaterialize(DataFlowNode currentNode, int index, DataType threadType, DataFlowNode dataSource) {
			int expectIndex = currentNode.GetExpectIndex();
			if (expectIndex != -1) {
				if (index == -1)
					throw new Exception();
				if (expectIndex != index)
					return;
				index = -1;
			}
			threadType = MutateDataType(currentNode, threadType);
			if (currentNode.Wrapped is Join)
				CheckDataFlowIncomingToJoin(currentNode);
			else if (currentNode.DataType == DataType.Unknown) {
				currentNode.DataType = threadType;
				currentNode.DataSource = dataSource;
			}
			List<DataFlowNode> nextNodes = graph.Connected(currentNode);
			foreach (DataFlowNode nextNode in nextNodes) {
				if (currentNode.DoesNotTransferDataTo(nextNode))
					continue;
				if (threadType == DataType.Surrogate && (!nextNode.IsSupportSurrogate() || IsTerminalNode(nextNode)))
					AddEdgeToInsertMaterialize(currentNode, nextNode);
				int[] indexes = (index == -1) ? nextNode.GetOperationIndex(currentNode).ToArray() : new int[] {index};
				foreach (int indexX in indexes)
					TraceToFindEdgesForMaterialize(nextNode, indexX, threadType, dataSource);
			}
		}
		void TryInsertMaterialize(Edge edge) {
			DataFlowNode newNode = null;
			Scan scan = edge.SurrogateScan.Wrapped as Scan;
			if (edge.Start.Wrapped is Scan || edge.Start.Wrapped is SelectScan) {
				List<DataFlowNode> list = graph.Connected(edge.Start);
				if (list.Count == 1) {
					scansForReplace.Remove(edge.Start);
					return;
				}
				Scan clone = new Scan(scan.ColumnName, scan.Storage);
				newNode = new DataFlowNode(clone);
			}
			else if (scansForReplace.Contains(edge.SurrogateScan)) {
				Materialize materialize = new Materialize(scan.ColumnName, scan.Storage, edge.Start.Wrapped as SingleFlowOperation);
				newNode = new DataFlowNode(materialize);
			}
			if (newNode == null)
				return;
			graph.AddNode(newNode);
			PlanOptimizerHelper.ReconnectAndReplace(newNode, edge.Start, edge.End, graph);
		}
		void ReplaceToSurrogateScans() {
			foreach (DataFlowNode scanNode in scansForReplace) {
				List<DataFlowNode> connected = graph.Connected(scanNode).FindAll(x => x.IsSupportSurrogate());
				if (connected.Count == 0)
					continue;
				Scan scan = scanNode.Wrapped as Scan;
				DataFlowNode surrogateScan = new DataFlowNode(new SurrogateScan(scan.ColumnName, scan.Storage));
				graph.AddNode(surrogateScan);
				foreach (DataFlowNode child in connected)
					PlanOptimizerHelper.ReconnectAndReplace(surrogateScan, scanNode, child, graph);
				if (graph.Connected(scanNode).Count == 0)
					graph.RemoveNode(scanNode);
			}
		}		
		void CheckDataFlowIncomingToJoin(DataFlowNode currentNode) {
			Join join = currentNode.Wrapped as Join;
			List<DataFlowNode> incoming = graph.Incoming(currentNode);
			DataFlowNode[] joinNodes = new DataFlowNode[join.Criteria2.Length];
			DataFlowNode[] criteriaNodes = new DataFlowNode[join.Criteria1.Length];
			for (int i = 0; i < join.Criteria1.Length; i++)
				criteriaNodes[i] = incoming.Find(x => x.Wrapped == join.Criteria1[i]);
			for (int i = 0; i < join.Criteria2.Length; i++)
				joinNodes[i] = incoming.Find(x => x.Wrapped == join.Criteria2[i]);
			for (int i = 0; i < join.Criteria1.Length; i++) {
				if (criteriaNodes[i].DataType == DataType.Materialize && (joinNodes[i].DataType == DataType.Unknown || joinNodes[i].DataType == DataType.Surrogate)) {
					List<DataFlowNode> incomingForJoinNode = graph.Incoming(joinNodes[i]);
#if DEBUGTEST
					if (incomingForJoinNode.Count > 1)
						throw new Exception();
#endif
					AddEdgeToInsertMaterialize(incomingForJoinNode[0], joinNodes[i]);
				}
				if ((criteriaNodes[i].DataType == DataType.Unknown || criteriaNodes[i].DataType == DataType.Surrogate) && joinNodes[i].DataType == DataType.Materialize)
					AddEdgeToInsertMaterialize(criteriaNodes[i], currentNode);
				if (criteriaNodes[i].DataType == DataType.Unknown || joinNodes[i].DataType == DataType.Unknown)
					return;	 
			}
		}		
		DataType MutateDataType(DataFlowNode node, DataType dataType) {
			if (node.Wrapped is GroupAggregate && dataType == DataType.Surrogate)
				return DataType.Materialize;
			if (node.Wrapped is ConstScan)
				return DataType.Materialize;
			if (node.Wrapped is Scan && ((Scan)node.Wrapped).OperationType == typeof(int))
				return DataType.Materialize;
			if (node.Wrapped is SelectScan)
				return DataType.Materialize;
			if (node.Wrapped is SurrogateScan)
				return DataType.Materialize;
			if (node.Wrapped is Materialize && dataType == DataType.Surrogate)
				return DataType.Materialize;
			if (dataType == DataType.Surrogate && !node.IsSupportSurrogate())
				return DataType.Materialize;
			return dataType;
		}
		bool IsTerminalNode(DataFlowNode nextNode) {
			return graph.FindTerminalNodes().Contains(nextNode);
		}
		void AddEdgeToInsertMaterialize(DataFlowNode startNode, DataFlowNode endNode) {
			if (graph.Connected(startNode).Contains(endNode) && startNode.DataSource != null)
				edgesToAddMaterialize.Add(new Edge() {
					SurrogateScan = startNode.DataSource,
					Start = startNode,
					End = endNode
				});
		}
		public void Optimize() {
			graph = PlanOptimizerHelper.BuildWrapperGraph(roots, x => new DataFlowNode(x), true);
			edgesToAddMaterialize = new List<Edge>();
			scansForReplace = new List<DataFlowNode>();
			AddSurrogateOperation();
		}
	}
}
