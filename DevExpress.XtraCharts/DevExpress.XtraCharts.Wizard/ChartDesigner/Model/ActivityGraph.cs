#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using DevExpress.XtraCharts.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
namespace DevExpress.XtraCharts.Designer.Native {
	public class ActivityGraph {
		class Edge {
			readonly string propertyName;
			readonly Func<object, bool> func;
			readonly EditorActivity activity;
			public string PropertyName { get { return propertyName; } }
			public Edge(string name, EditorActivity activity, Func<object, bool> func) {
				this.propertyName = name;
				this.activity = activity;
				this.func = func;
			}
			public ActivityMessage CalculateActivity(object newValue) {
				return new ActivityMessage(propertyName, activity, func(newValue));
			}
		}
		Dictionary<string, List<Edge>> graph;
		ElementsOptionsControlBase optionsControl;
		protected DesignerChartElementModelBase Model { get { return optionsControl.Model; } }
		bool GetValue(string changedProperty, out object value) {
			Type modelType = Model.GetType();
			PropertyInfo info = ReflectionHelper.GetComplexProperty(modelType, changedProperty);
			if(info == null) {
				value = null;
				return false;
			}
			value = ReflectionHelper.GetComplexPropertyValue(changedProperty, Model);
			return true;
		}
		public ActivityGraph(ElementsOptionsControlBase optionsControl) {
			Guard.ArgumentNotNull(optionsControl, "OptionsControl");
			this.optionsControl = optionsControl;
			this.graph = new Dictionary<string, List<Edge>>();
		}
		public bool CheckEdgeExistance(string mainProperty, string childProperty) {
			if(!graph.ContainsKey(mainProperty))
				return false;
			List<Edge> edges = graph[mainProperty];
			foreach(Edge edge in edges)
				if(string.Equals(edge.PropertyName, childProperty))
					return true;
			return false;
		}
		public void AddEdge<T>(string mainProperty, string childProperty, EditorActivity activity, Func<T, bool> func) {
			List<Edge> edges;
			if(!graph.ContainsKey(mainProperty)) {
				edges = new List<Edge>();
				graph.Add(mainProperty, edges);
			} else
				edges = graph[mainProperty];
			edges.Add(new Edge(childProperty, activity, (object x) => { return func((T)x); }));
		}
		public List<ActivityMessage> CalculateActivityChanges(string changedProperty) {
			if(!graph.ContainsKey(changedProperty))
				return null;
			List<ActivityMessage> changes = new List<ActivityMessage>();
			List<Edge> edges = graph[changedProperty];
			object value = null;
			if(!GetValue(changedProperty, out value))
				return null;
			foreach(Edge edge in edges)
				changes.Add(edge.CalculateActivity(value));
			return changes;
		}
		public List<ActivityMessage> CalculateActivity() {
			List<ActivityMessage> changes = new List<ActivityMessage>();
			foreach(string property in graph.Keys) {
				List<Edge> edges = graph[property];
				object value = null;
				if(!GetValue(property, out value))
					continue;
				foreach(Edge edge in edges)
					changes.Add(edge.CalculateActivity(value));
			}
			return changes;
		}
	}
}
