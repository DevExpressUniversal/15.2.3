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
	public class ReorganizeGroupsAlgorithm : IOptimazerAlgorithm {
		class OperandsCoutnComparer : IComparer<OperationNode> {
			public int Compare(OperationNode x, OperationNode y) {
				if (x.Wrapped.Operands.Count() > y.Wrapped.Operands.Count())
					return 1;
				if (x.Wrapped.Operands.Count() < y.Wrapped.Operands.Count())
					return -1;
				return 0;
			}
		}
		class ReorganizeAggregations {
			DirectedGraph<OperationNode> graph;
			Dictionary<OperationNode, OperationNode> aggToData;
			public ReorganizeAggregations(DirectedGraph<OperationNode> graph) {
				this.graph = graph;
				aggToData = new Dictionary<OperationNode, OperationNode>();
				List<OperationNode> aggregations = graph.Select(x => x.Is<GroupAggregate>());
				foreach (OperationNode aggregation in aggregations) {
					aggToData.Add(aggregation, GetDataFlow(aggregation));
				}
			}
			public void Reorganize(DirectedGraph<OperationNode> fromLargestToSmallest) {
				foreach (OperationNode biggestGroup in fromLargestToSmallest.FindStartNodes()) {
					List<OperationNode> aggregations = GetAggregationsForGroup(biggestGroup);
					RecursiveReorganizeAggregations(biggestGroup, aggregations, fromLargestToSmallest);
				}
			}
			void RecursiveReorganizeAggregations(OperationNode group, List<OperationNode> aggregations, DirectedGraph<OperationNode> fromLargestToSmallest) {
				List<OperationNode> followers = fromLargestToSmallest.Connected(group);
				foreach (OperationNode follower in followers) {
					ReconnectAggregations(follower, aggregations);
					RecursiveReorganizeAggregations(follower, GetAggregationsForGroup(follower), fromLargestToSmallest);
				}
			}
			void ReconnectAggregations(OperationNode group, List<OperationNode> aggrFromBiggerGroup) {
				if (aggrFromBiggerGroup == null)
					return;
				List<OperationNode> aggregations = GetAggregationsForGroup(group);
				if (aggregations == null)
					return;
				foreach (OperationNode aggregation in aggregations) {
					GroupAggregate agg = aggregation.Cast<GroupAggregate>();
					foreach (OperationNode biggerAggregation in aggrFromBiggerGroup) {
						if (ProcessSameData(aggregation, agg, biggerAggregation))
							ReconnectAggregation(aggregation, biggerAggregation);
					}
				}
			}
			void ReconnectAggregation(OperationNode aggregation, OperationNode biggerAggregation) {
				List<OperationNode> aggOperands = graph.Incoming(aggregation);
				GroupAggregate agg = aggregation.Cast<GroupAggregate>();
				OperationNode indexes = aggOperands.Find(x => x.Wrapped == agg.Indexes);
				OperationNode aggregationData = aggOperands.Find(x => x.Wrapped == agg.ValuesFlow);
				if (indexes.Is<JoinIndexes>()) {
					OperationNode indexesScan = AddNewIndexesScanBufferToGraph(indexes);
					PlanOptimizerHelper.ReconnectAndReplace(indexesScan, indexes, aggregation, graph);
				}
				OperationNode biggerAggregationResult = graph.Connected(biggerAggregation).Find(x => x.Is<ScanBuffer>());
				if (biggerAggregationResult == null)
					biggerAggregationResult = AddNewAggregationResultToGraph(biggerAggregation);
				PlanOptimizerHelper.ReconnectAndReplace(biggerAggregationResult, aggregationData, aggregation, graph);
			}
			bool ProcessSameData(OperationNode aggregation, GroupAggregate agg, OperationNode biggerAggregation) {
				GroupAggregate biggerAgg = biggerAggregation.Cast<GroupAggregate>();
				return agg.SummaryType == biggerAgg.SummaryType && aggToData[aggregation] == aggToData[biggerAggregation];
			}
			OperationNode GetDataFlow(OperationNode aggregation) {
				List<OperationNode> aggOperands = graph.Incoming(aggregation);
				GroupAggregate agg = aggregation.Cast<GroupAggregate>();
				OperationNode aggregationData = aggOperands.Find(x => x.Wrapped == agg.ValuesFlow);
				return aggregationData;
			}
			OperationNode AddNewIndexesScanBufferToGraph(OperationNode joinIndexes) {
				OperationNode extractIndexesNode = graph.Incoming(joinIndexes).Find(x => x.Is<ExtractIndexes>());				
				ExtractIndexes extractIndexes = extractIndexesNode.Cast<ExtractIndexes>();
				ScanBuffer scanBuffer = new ScanBuffer(extractIndexes);
				OperationNode scanBufferNode = new OperationNode(scanBuffer);
				graph.AddNode(scanBufferNode);
				graph.AddEdge(extractIndexesNode, scanBufferNode);
				return scanBufferNode;
			}
			OperationNode AddNewAggregationResultToGraph(OperationNode aggregationNode) {
				GroupAggregate aggregation = aggregationNode.Cast<GroupAggregate>();
				ScanBuffer scanBuffer = new ScanBuffer(aggregation);
				OperationNode scanBufferNode = new OperationNode(scanBuffer);
				graph.AddNode(scanBufferNode);
				return scanBufferNode;
			}
			List<OperationNode> GetAggregationsForGroup(OperationNode group) {
				OperationNode extractIndexesNode = graph.Connected(group).Find(x => x.Is<ExtractIndexes>());
				if (extractIndexesNode == null)
					return null;
				List<OperationNode> indexesBufferNode = graph.Connected(extractIndexesNode).FindAll(x => x.Is<SingleFlowOperation>());
				if (indexesBufferNode == null || indexesBufferNode.Count == 0)
					return null;
				List<OperationNode> list = new List<OperationNode>();
				foreach (OperationNode indexes in indexesBufferNode)
					list.AddRange(graph.Connected(indexes).FindAll(x => x.Is<GroupAggregate>()));
				List<SummaryType> types = new List<SummaryType>();
				types.Add(SummaryType.Min);
				types.Add(SummaryType.Max);
				types.Add(SummaryType.Sum);
				list = list.FindAll(x => types.Contains(x.Cast<GroupAggregate>().SummaryType));
				if (list.Count == 0)
					return null;
				return list;
			}
		}
		class ReorganizeIndexesForAggregations {
			DirectedGraph<OperationNode> graph;
			public ReorganizeIndexesForAggregations(DirectedGraph<OperationNode> graph) {
				this.graph = graph;
			}
			public void Reorganize(DirectedGraph<OperationNode> fromLargestToSmallest) {
				foreach (OperationNode biggestGroup in fromLargestToSmallest.FindStartNodes()) {
					OperationNode indexes = GetIndexesForGroup(biggestGroup);
					if (indexes == null)
						indexes = AddIndexesBufferToGraph(biggestGroup);
					RecursiveReconnectIndexes(biggestGroup, indexes, fromLargestToSmallest);
				}
			}
			void RecursiveReconnectIndexes(OperationNode group, OperationNode indexesFromBiggerGroup, DirectedGraph<OperationNode> fromLargestToSmallest) {
				List<OperationNode> followers = fromLargestToSmallest.Connected(group);
				foreach (OperationNode follower in followers)
					ReconnectIndexes(follower, indexesFromBiggerGroup, fromLargestToSmallest);
			}
			void ReconnectIndexes(OperationNode group, OperationNode indexesFromBiggerGroup, DirectedGraph<OperationNode> fromLargestToSmallest) {
				OperationNode indexesBufferNode = GetIndexesForGroup(group);
				if (indexesBufferNode == null)
					indexesBufferNode = AddIndexesBufferToGraph(group);
				else {
					OperationNode joinIndexesNode = AddJoinIndexesToGraph(indexesBufferNode, indexesFromBiggerGroup);
					List<OperationNode> aggrNodes = graph.Connected(indexesBufferNode).FindAll(x => x.Is<GroupAggregate>());
					foreach (OperationNode aggrNode in aggrNodes)
						PlanOptimizerHelper.ReconnectAndReplace(joinIndexesNode, indexesBufferNode, aggrNode, graph);
					indexesBufferNode = joinIndexesNode;
				}
				RecursiveReconnectIndexes(group, indexesBufferNode, fromLargestToSmallest);
			}
			OperationNode AddJoinIndexesToGraph(OperationNode indexesBuffer, OperationNode indexesFromBiggerGroup) {
				OperationNode extractIndexesNode = graph.Incoming(indexesBuffer)[0];
				ExtractIndexes extractIndexes = extractIndexesNode.Cast<ExtractIndexes>();
				SingleFlowOperation fromBiggerGroup = indexesFromBiggerGroup.Cast<SingleFlowOperation>();
				JoinIndexes joinIndexes = new JoinIndexes(fromBiggerGroup, extractIndexes);
				OperationNode joinIndexesNode = new OperationNode(joinIndexes);
				graph.AddNode(joinIndexesNode);
				graph.AddEdge(indexesFromBiggerGroup, joinIndexesNode);
				graph.AddEdge(extractIndexesNode, joinIndexesNode);
				return joinIndexesNode;
			}
			OperationNode AddIndexesBufferToGraph(OperationNode groupNode) {
				Group group = groupNode.Cast<Group>();
				ExtractIndexes extractIndexes = new ExtractIndexes(group);
				OperationNode extractIndexesNode = new OperationNode(extractIndexes);
				graph.AddNode(extractIndexesNode);
				ScanBuffer indexesBuffer = new ScanBuffer(extractIndexes);				
				OperationNode indexesBufferNode = new OperationNode(indexesBuffer);
				graph.AddNode(indexesBufferNode);
				graph.AddEdge(groupNode, extractIndexesNode);
				graph.AddEdge(extractIndexesNode, indexesBufferNode);
				return indexesBufferNode;
			}
			OperationNode GetIndexesForGroup(OperationNode group) {
				OperationNode extractIndexesNode = graph.Connected(group).Find(x => x.Is<ExtractIndexes>());
				if (extractIndexesNode == null)
					return null;
				OperationNode indexesBufferNode = graph.Connected(extractIndexesNode).Find(x => x.Is<SingleFlowOperation>());
				if (indexesBufferNode == null)
					return null;
				return indexesBufferNode;
			}
		}
		class ReorganizeGroups {
			DirectedGraph<OperationNode> graph;
			public ReorganizeGroups(DirectedGraph<OperationNode> graph) {
				this.graph = graph;
			}
			public void Reorganize(DirectedGraph<OperationNode> fromLargestToSmallest) {
				foreach (OperationNode biggestGroup in fromLargestToSmallest.FindStartNodes()) {
					Dictionary<SingleFlowOperation, int> scanToExtractMapping = GetScanToExtractMapping(biggestGroup);
					ReconnectFollowers(biggestGroup, fromLargestToSmallest, scanToExtractMapping);
				}
			}
			void ReconnectFollowers(OperationNode leader, DirectedGraph<OperationNode> dependentGroups, Dictionary<SingleFlowOperation, int> inputToExtractMapping) {
				List<OperationNode> followers = dependentGroups.Connected(leader);
				foreach (OperationNode follower in followers) {
					Dictionary<SingleFlowOperation, int> inputToExtractMappingForFollower = GetScanToExtractMapping(follower);
					ConnectFollowerToLeader(follower, leader, inputToExtractMapping);
					ReconnectFollowers(follower, dependentGroups, inputToExtractMappingForFollower);
				}
			}
			void ConnectFollowerToLeader(OperationNode follower, OperationNode leader, Dictionary<SingleFlowOperation, int> inputToExtractMapping) {
				List<OperationNode> followerOperands = graph.Incoming(follower);
				Dictionary<OperationNode, OperationNode> scanToExtract = new Dictionary<OperationNode, OperationNode>();
				foreach (OperationNode inputNode in followerOperands) {
					OperationNode extract;
					if (scanToExtract.ContainsKey(inputNode))
						extract = scanToExtract[inputNode];
					else {
						SingleFlowOperation input = inputNode.Cast<SingleFlowOperation>();
						if (input == null)
							throw new Exception();
						int extractIndex = inputToExtractMapping[input];
						OperationNode bufferScan = graph.Connected(leader)[0];
						extract = graph.Connected(bufferScan).Find(x => x.Cast<Extract>().ResultNumber == extractIndex);
						if (extract == null)
							extract = AddNewExtractToGraph(bufferScan, extractIndex);
						scanToExtract.Add(inputNode, extract);
					}
					PlanOptimizerHelper.ReconnectAndReplace(extract, inputNode, follower, graph);
				}
			}
			Dictionary<SingleFlowOperation, int> GetScanToExtractMapping(OperationNode node) {
				List<OperationNode> followerOperands = graph.Incoming(node);
				Dictionary<SingleFlowOperation, int> mapping = new Dictionary<SingleFlowOperation, int>();
				foreach (OperationNode inputNode in followerOperands) {
					SingleFlowOperation input = inputNode.Cast<SingleFlowOperation>();
					if (input == null || mapping.ContainsKey(input))
						continue;
					List<int> indexis = GetOperationIndex(inputNode, node);
					int extractIndex = indexis[0];
					mapping.Add(input, extractIndex);
				}
				return mapping;
			}
			OperationNode AddNewExtractToGraph(OperationNode bufferScan, int extractIndex) {
				OperationNode extrtact = new OperationNode(new Extract(bufferScan.Cast<MultiFlowOperation>(), extractIndex));
				graph.AddNode(extrtact);
				graph.AddEdge(bufferScan, extrtact);
				return extrtact;
			}
			List<int> GetOperationIndex(OperationNode topNode, OperationNode node) {
				Group groupOperation = node.Cast<Group>();
				List<int> indexs = new List<int>();
				if (groupOperation != null) {
					int index = 0;
					foreach (CustomOperation operand in groupOperation.Operands) {
						if (operand == topNode.Wrapped)
							indexs.Add(index);
						index++;
					}
				}
				return indexs;
			}
		}
		List<CustomOperation> roots;
		DirectedGraph<OperationNode> graph;
		public ReorganizeGroupsAlgorithm(List<CustomOperation> roots) {
			this.roots = roots;
		}
		public void Optimize() {
			graph = PlanOptimizerHelper.BuildOperationGraph(roots, true);
			List<OperationNode> groupsSortedByOperands = graph.Select(x => x.Is<Group>());
			groupsSortedByOperands.Sort(new OperandsCoutnComparer());
			DirectedGraph<OperationNode> fromLargestToSmallest = FindDependentGroups(groupsSortedByOperands);
			ReorganizeGroups reorganizeGroups = new ReorganizeGroupsAlgorithm.ReorganizeGroups(graph);
			reorganizeGroups.Reorganize(fromLargestToSmallest);
			ReorganizeIndexesForAggregations reorganizeIndexesForAggregations = new ReorganizeIndexesForAggregations(graph);
			reorganizeIndexesForAggregations.Reorganize(fromLargestToSmallest);
			graph = PlanOptimizerHelper.BuildOperationGraph(roots, true);
			ReorganizeAggregations reorganizeAggregations = new ReorganizeAggregations(graph);
			reorganizeAggregations.Reorganize(fromLargestToSmallest);
		}
		DirectedGraph<OperationNode> FindDependentGroups(List<OperationNode> groupsSortedByOperands) {
			DirectedGraph<OperationNode> dependentGroups = new DirectedGraph<OperationNode>();
			for (int i = 0; i < groupsSortedByOperands.Count - 1; i++) {
				for (int j = i + 1; j < groupsSortedByOperands.Count; j++) {
					OperationNode group = groupsSortedByOperands[i];
					OperationNode biggerGroup = groupsSortedByOperands[j];
					if (group.Wrapped.Operands.Count() >= biggerGroup.Wrapped.Operands.Count())
						continue;
					if (IsSubsetOf(group, biggerGroup)) {
						dependentGroups.AddNode(group);
						dependentGroups.AddNode(biggerGroup);
						dependentGroups.AddEdge(biggerGroup, group);
						break;
					}
				}
			}
			return dependentGroups;
		}
		bool IsSubsetOf(OperationNode follower, OperationNode leader) {
			List<OperationNode> leaderOperands = graph.Incoming(leader);
			List<OperationNode> followerOperands = graph.Incoming(follower);
			foreach (OperationNode operand in followerOperands)
				if (!leaderOperands.Contains(operand))
					return false;
			if (LeaderContainsFolowerOperand(follower.Wrapped, leader.Wrapped))
				return false;			
			return true;
		}
		bool LeaderContainsFolowerOperand(CustomOperation follower, CustomOperation leader) {
			if (leader.Equals(follower))
				return true;
			foreach (CustomOperation operands in leader.Operands)
				if (LeaderContainsFolowerOperand(follower, operands))
					return true;
			return false;
		}
	}
}
