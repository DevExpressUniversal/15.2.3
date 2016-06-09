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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor {
	public class OperationGraphViz {
		static string header = "digraph QueryPlan { \n node [style=filled]; \n rankdir=TB; \n\n";
		static string configFile = @"d:\gviz_conf.xml";
		Dictionary<CustomOperation, int> opCodes = new Dictionary<CustomOperation, int>();
		HashSet<CustomOperation> ends;
		IList<IQueueNode> queues;
		string GetGraphText(IEnumerable<CustomOperation> ends, bool showQueues) {
			this.queues = showQueues ? QueryPlanAnalyzer.Analyze(ends).ToList() : null;
			this.ends = new HashSet<CustomOperation>(ends);
			var nodes = QueryPlanAnalyzer.TopologicalSort(QueryPlanAnalyzer.GetAllQueryNodes(ends));
			GenerateCodes(nodes);
			string result = header;
			result += GetLinks(nodes) ;			
			result += queues == null ?
				GetNodes(nodes) :
				GetClusteredNodes(nodes);
			return result + "\n }";
		}
		string GetNodes(IEnumerable<CustomOperation> nodes) {
			string result = "";
			foreach(var node in nodes)
				result += String.Format("{0} [label=\"{1}\"; color=\"{2}\"; shape={3}];\n", GetNodeCodeName(node), GetNodeLabel(node), GetOperationColor(node), GetOperationShape(node));
			return result;
		}
		string GetClusteredNodes(IEnumerable<CustomOperation> nodes) {
			string result = "";
			int i = 0;
			foreach(var queue in queues) {
				result += String.Format("subgraph cluster{0}", i++) + " {\n";
				foreach(var node in queue.SortedOperations)
					result += String.Format("{0} [label=\"{1}\"; color=\"{2}\"; shape={3}];\n", GetNodeCodeName(node), GetNodeLabel(node), GetOperationColor(node), GetOperationShape(node));
				result += "}\n\n";
			}
			return result;
		}
		string GetOperationShape(CustomOperation node) {
			if(node is BlockOperation)
				return "box";
			return "ellipse";
		}
		string GetOperationColor(CustomOperation node) {
			if(ends.Contains(node))
				return "forestgreen";
			return "";
		}
		string GetLinks(IEnumerable<CustomOperation> nodes) {
			string result = "";
			foreach(var node in nodes)
				foreach(var operand in node.Operands)
					result += Link(operand, node);
			return result;
		}
		void GenerateCodes(IEnumerable<CustomOperation> nodes) {
			int i = 0;
			foreach(var n in nodes)
				opCodes.Add(n, i++);
		}
		string Link(CustomOperation op1, CustomOperation op2) {
			return String.Format("{0} -> {1} [color=\"0.650 0.700 0.700\"];\n", GetNodeCodeName(op1), GetNodeCodeName(op2));
		}
		string GetNodeLabel(CustomOperation o) {
			return String.Format("{0}({1})", o.GetType().Name, o.ParamsToString());
		}
		string GetNodeCodeName(CustomOperation o) {
			return String.Format("node_{0}_{1}", o.GetType().Name, opCodes[o]);
		}
		static string GetUniqueName(string baseName, string ext) {
			int i = 0;
			string result = baseName + ext;
			do {
				result = baseName + (i == 0 ? "" : String.Format("({0})", i)) + ext;
				i++;
			} while(File.Exists(result));
			return result;
		}
		public static void SaveQueues(IEnumerable<CustomOperation> ends) {
			Save(ends, "queues", true);
		}
		public static void SaveInitial(IEnumerable<CustomOperation> ends) {
			Save(ends, "initial", false);
		}
		public static void SaveOptimized(IEnumerable<CustomOperation> ends) {
			Save(ends, "optimized", false);
		}
		public static void Save(IEnumerable<CustomOperation> ends, string template, bool showQueues) {
#if DEBUGTEST 
			if(!File.Exists(configFile))
				return;
			XElement conf = XElement.Load(configFile);
			var enabledElement = conf.Element("enabled");
			if(enabledElement == null || enabledElement.Value.ToLower() != "true")
				return;
			string fileName = String.Format(@"{0}\{1}_{2}", conf.Element("outputpath").Value, DateTime.Now.ToString("d-M-y_h-mm-ss"), template);
			string textName = GetUniqueName(fileName, ".txt");
			string pngName = GetUniqueName(fileName, ".png");
			if(File.Exists(textName))
				File.Delete(textName);
			string graph = new OperationGraphViz().GetGraphText(ends, showQueues);
			SaveGraphText(graph, textName);
			var graphVizPathElem = conf.Element("graphvizbin");
			string graphVizPath = graphVizPathElem == null ? "" : graphVizPathElem.Value;
			if(!String.IsNullOrEmpty(graphVizPath)) {
				RunGraphViz(graphVizPath, textName, pngName);
				File.Delete(textName);
			}
#endif
		}
		static void SaveGraphText(string graph, string confFile) {
			if(File.Exists(confFile))
				File.Delete(confFile);
			using(var stream = File.Open(confFile, FileMode.CreateNew)) {
				stream.Position = 0;
				var writer = new StreamWriter(stream);
				writer.Write(graph);
				writer.Flush();
				stream.Flush();
			}
		}
		[System.Security.SecuritySafeCritical]
		static void RunGraphViz(string graphvizbin, string txtxName, string outputName) {
			Process process = new Process();
			process.StartInfo.FileName = String.Format(@"{0}\dot.exe", graphvizbin);
			process.StartInfo.Arguments = String.Format("-Tpng {0} -o {1}", txtxName, outputName);
#if !DXPORTABLE
			process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
#endif
			process.Start();
			process.WaitForExit();
		}
	}
}
