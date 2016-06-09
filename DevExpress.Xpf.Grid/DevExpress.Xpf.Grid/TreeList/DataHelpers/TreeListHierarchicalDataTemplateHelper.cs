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

using System.Collections;
using System.Windows;
using System.Windows.Data;
using System.Reflection;
using System.ComponentModel;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Data.Browsing;
using HierarchicalDataTemplate = DevExpress.Xpf.Core.HierarchicalDataTemplate;
#else
using System.Collections.Generic;
using System;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid.Native;
#endif
namespace DevExpress.Xpf.Grid.TreeList {
	public class TreeListHierarchicalDataTemplateHelper : TreeListHierarchicalDataHelper {
		public TreeListHierarchicalDataTemplateHelper(TreeListDataProvider provider, object dataSource)
			: base(provider, dataSource) {
#if !SL
				ImplicitTemplatesDictionary = new Dictionary<Type, DataTemplate>();
#endif
		}
		protected override IEnumerable GetChildren(TreeListNode node) {
			if(node == null)
				return null;
			object content = node.Content;
			if(content == null)
				return null;
			HierarchicalDataTemplate hierarchicalDataTemplate;
			if(node.Template != null) {
				hierarchicalDataTemplate = node.Template as HierarchicalDataTemplate;
				if(hierarchicalDataTemplate != null) {
					return GetChildrenByTemplate(node, hierarchicalDataTemplate);
				}
			}
			if(View.DataRowTemplate != null || View.DataRowTemplateSelector != null) {
				hierarchicalDataTemplate = GetActualTemplateForNode(node) as HierarchicalDataTemplate;
				if(hierarchicalDataTemplate != null)
					return GetChildrenByTemplate(node, hierarchicalDataTemplate);
			}
#if !SL
			if(View.DataRowTemplateSelector == null && View.DataRowTemplate == null)
				return GetChildrenByImplicitDataTemplate(node);
#endif
			return null;
		}
		DataTemplate GetActualTemplateForNode(TreeListNode node) {
			return (((TreeListView)View).ActualDataRowTemplateSelector as TreeListRowTemplateSelectorWrapper).SelectTemplateCore(GetRowData(node), View, false);
		}
		private IEnumerable GetChildrenByTemplate(TreeListNode node, HierarchicalDataTemplate hierarchicalDataTemplate) {
			node.ItemTemplate =
#if SL
				hierarchicalDataTemplate.GetActualItemTemplate() ?? 
#endif
				hierarchicalDataTemplate.ItemTemplate;
			return GetBindingValue(
#if SL
				hierarchicalDataTemplate.GetActualItemSource() ?? 
#endif
				hierarchicalDataTemplate.ItemsSource,
				node.Content);
		}
		IEnumerable GetBindingValue(BindingBase binding, object content) {
			if(binding == null || content == null)
				return null;
			BindingBase bnd = BindingCloneHelper.Clone(binding, content);
			BindingValueEvaluator evl = new BindingValueEvaluator(bnd);
			return evl.Value as IEnumerable;
		}
#if !SL
		protected override void AddNode(TreeListNodeCollection nodes, TreeListNode node) {
			AssignTemplate(node);
			base.AddNode(nodes, node);
		}
		void AssignTemplate(TreeListNode node) {
			if(node.Template != null)
				return;
			DataTemplate template = GetActualTemplateForNode(node);
			if(template != null && template is HierarchicalDataTemplate) {
				node.Template = template;
				return;
			}
			if(node.ParentNode != null && node.ParentNode.ItemTemplate != null) {
				node.Template = node.ParentNode.ItemTemplate;
				return;
			}
			AsignImplicitDataTemplate(node);
		}
		protected override DataTemplate GetItemTemplate(TreeListNode treeListNode) {
			if(treeListNode.ItemTemplate != null)
				return treeListNode.ItemTemplate;
			HierarchicalDataTemplate template = treeListNode.Template as HierarchicalDataTemplate;
			if(template != null) {
				if(template.ItemTemplateSelector == null)
					return template.ItemTemplate;
				DataTemplate itemTemplate = template.ItemTemplateSelector.SelectTemplate(GetRowData(treeListNode), View);
				return itemTemplate ?? template.ItemTemplate;
			}
			return null;
		}
		protected override void AsignImplicitDataTemplate(TreeListNode node) {
			DataTemplate template = TryFindDataTemplate(node);
			if(template != null) {
				node.Template = template;
				HierarchicalDataTemplate hTemplate = template as HierarchicalDataTemplate;
				if(hTemplate != null) 
					node.ItemTemplate = hTemplate.ItemTemplate;
			}
		}
		IEnumerable GetChildrenByImplicitDataTemplate(TreeListNode node) {
			HierarchicalDataTemplate template = TryFindDataTemplate(node) as HierarchicalDataTemplate;
			if(template != null) {
				BindingValueEvaluator evaluator = new BindingValueEvaluator(template.ItemsSource);
				return evaluator.Value as IEnumerable;
			}
			return null;
		}
		protected DataTemplate TryFindDataTemplate(TreeListNode node) {
			return TryFindDataTemplateCore(node.Content);
		}
		protected DataTemplate TryFindDataTemplateCore(object content) {
			if(content == null) return null;
			Type type = content.GetType(); 
			if(type == typeof(object)) return null;
			if(ImplicitTemplatesDictionary.ContainsKey(type))
				return ImplicitTemplatesDictionary[type];
			DataTemplate template = DefaultTemplateSelector.Instance.SelectTemplate(content, View.DataControl);
			if(template != null) {
				ImplicitTemplatesDictionary.Add(type, template);
				return template;
			}
			return null;
		}
		protected Dictionary<Type, DataTemplate> ImplicitTemplatesDictionary { get; set; }
		public override void LoadData() {
			ImplicitTemplatesDictionary.Clear();
			base.LoadData();
		}
#endif
	}
 }
