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

using DevExpress.Diagram.Core.Layout;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Diagram.Core {
	public interface IOrgChartBehavior {
		IDiagramControl Diagram { get; }
		string ChildrenPath { get; }
		IChildrenSelector ChildrenSelector { get; }
		IEnumerable ItemsSource { get; }
		string KeyMember { get; }
		string ParentMember { get; }
		IDiagramItem CreateItem(object dataItem);
		double Margin { get; }
	}
	public class OrgChartController {
		readonly IOrgChartBehavior behavior;
		IDiagramControl Diagram { get { return behavior.Diagram; } }
		public OrgChartController(IOrgChartBehavior behavior) {
			this.behavior = behavior;
		}
		public void OnItemsSourceChanged() {
			PopulateDiagram();
		}
		public void OnAttached() {
			PopulateDiagram();
		}
		public void OnDetaching() {
		}
		void PopulateDiagram() {
			if(behavior.ItemsSource == null || Diagram == null)
				return;
			var provider = CreateProvider();
			var dataToItemMap = provider.AllItems
				.Cast<object>()
				.ToDictionary(dataItem => dataItem, dataItem => {
					var item = behavior.CreateItem(dataItem);
					Diagram.Items().Add(item);
					return item;
				});
			Diagram.UpdateLayout();
			foreach(var dataObject in behavior.ItemsSource) {
				var parentItem = dataToItemMap[dataObject];
				var children = provider.GetChildren(dataObject).Select(x => dataToItemMap[x]);
				foreach(var childItem in children) {
					var connector = Diagram.CreateConnector();
					connector.Type = ConnectorType.Straight;
					connector.BeginItem = parentItem;
					connector.EndItem = childItem;
					Diagram.RootItem().NestedItems.Add(connector);
				}
			}
			Diagram.LayoutTreeDiagram(new TreeLayoutSettings(40, 40, margin: behavior.Margin));
		}
		IHierarchyDataProvider CreateProvider() {
			if(!string.IsNullOrEmpty(behavior.ParentMember) && !string.IsNullOrEmpty(behavior.KeyMember))
				return new KeyParentModeDataProvider(behavior.ItemsSource, behavior.KeyMember, behavior.ParentMember);
			if(behavior.ChildrenSelector != null)
				return new ChildrenSelectorDataProvider(behavior.ItemsSource, behavior.ChildrenSelector.GetChildren);
			if(!string.IsNullOrEmpty(behavior.ChildrenPath))
				return new ChildrenSelectorDataProvider(behavior.ItemsSource, x => (IEnumerable)PropertyDescriptorHelper.GetValue(x, behavior.ChildrenPath));
			return new PlainListDataProvider(behavior.ItemsSource);
		}
	}
}
