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
	public class AddSelectScanAlgorithm : IOptimazerAlgorithm {
		List<CustomOperation> roots;
		public AddSelectScanAlgorithm(List<CustomOperation> roots) {
			this.roots = roots;
		}
		public void Optimize() {
			DirectedGraph<OperationNode> invertGraph = PlanOptimizerHelper.BuildOperationGraph(roots, true);
			List<OperationNode> scans = invertGraph.Select(x => x.Wrapped is Scan);
			foreach (OperationNode scanNode in scans) {
				List<OperationNode> list = invertGraph.Connected(scanNode).FindAll(x => x.Wrapped is Select);
				if (list.Count != 1)
					continue;
				OperationNode selectNode = list[0];
				DirectedGraph<OperationNode> graph = invertGraph.GetReversedGraph();
				graph.RemoveEdge(selectNode, scanNode);
				if (!graph.HasPath(selectNode, scanNode)) {
					Scan scan = scanNode.Wrapped as Scan;
					Select select = selectNode.Wrapped as Select;
					SelectScan selectScan = new SelectScan(scan.ColumnName, scan.Storage, select.FilterFlow);
					OperationNode selectScanNode = new OperationNode(selectScan);
					invertGraph.AddNode(selectScanNode);
					foreach (OperationNode nodeForConnect in invertGraph.Connected(selectNode))
						PlanOptimizerHelper.ReconnectAndReplace(selectScanNode, selectNode, nodeForConnect, invertGraph);
					invertGraph.ForConnected(selectNode, x => PlanOptimizerHelper.ReconnectAndReplace(selectScanNode, selectNode, x, invertGraph));
				}
			}
		}
	}
}
