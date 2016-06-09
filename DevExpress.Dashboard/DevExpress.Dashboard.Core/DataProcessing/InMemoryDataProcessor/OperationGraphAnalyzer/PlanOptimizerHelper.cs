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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor {
	public static class PlanOptimizerHelper {
		public static DirectedGraph<OperationNode> BuildOperationGraph(List<CustomOperation> roots, bool inverted) {
			return BuildWrapperGraph<OperationNode>(roots, x => new OperationNode(x), inverted);
		}
		public static DirectedGraph<CustomOperation> BuildGraph(List<CustomOperation> roots, bool inverted) {
			return BuildWrapperGraph<CustomOperation>(roots, x => x, inverted);
		}
		public static DirectedGraph<T> BuildWrapperGraph<T>(List<CustomOperation> roots, Func<CustomOperation, T> createWrapper, bool inverted) {
			return BuildWrapperGraph<T, CustomOperation>(roots, x => x.Operands, createWrapper, inverted);
		}
		public static Dictionary<CustomOperation, CustomOperation> Clone(List<CustomOperation> roots) {
			Dictionary<CustomOperation, CustomOperation> oldToNewMapping = new Dictionary<CustomOperation, CustomOperation>();
			Func<CustomOperation, CustomOperation> replaceFunc = null;
			replaceFunc = x => {
				CustomOperation clone;
				if (oldToNewMapping.TryGetValue(x, out clone))
					return clone;
				clone = x.Clone(replaceFunc);
				oldToNewMapping.Add(x, clone);
				return clone;
			};
			foreach (CustomOperation operation in roots)
				oldToNewMapping.Add(operation, operation.Clone(replaceFunc));
			return oldToNewMapping.Where(pair => roots.Contains(pair.Key)).ToDictionary(pair => pair.Key, pair => pair.Value);
		}
		public static void ReconnectAndReplace<T>(T newNode, T oldNode, T nodeForConnect, DirectedGraph<T> graph) where T : OperationNode {
			graph.RemoveEdge(oldNode, nodeForConnect);
			graph.AddEdge(newNode, nodeForConnect);
			nodeForConnect.Wrapped.ReplaceOperand(oldNode.Wrapped, newNode.Wrapped);
		}
		internal static DirectedGraph<T> BuildWrapperGraph<T, N>(IList<N> nodes, Func<N, IEnumerable<N>> getChildren, Func<N, T> createWrapper, bool inverted) {
			DirectedGraph<T> graph = new DirectedGraph<T>();
			Queue<N> queue = new Queue<N>(nodes);
			HashSet<T> procesed = new HashSet<T>();
			while (queue.Count > 0) {
				N operation = queue.Dequeue();
				T operationNode = createWrapper(operation);
				if (procesed.Contains(operationNode))
					continue;
				graph.AddNode(operationNode);
				foreach (N c in getChildren(operation)) {
					T wraper = createWrapper(c);
					graph.AddNode(wraper);
					if (inverted)
						graph.AddEdge(wraper, operationNode);
					else
						graph.AddEdge(operationNode, wraper);
					queue.Enqueue(c);
				}
				procesed.Add(operationNode);
			}
			return graph;
		}
	}
}
