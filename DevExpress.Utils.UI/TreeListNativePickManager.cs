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

using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Data.Browsing.Design;
using System.ComponentModel;
using DevExpress.XtraTreeList.Nodes;
using System.Collections;
using DevExpress.Data.Browsing;
using System.Collections.ObjectModel;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraTreeList.Native;
using DevExpress.XtraReports.Native.Data;
using DevExpress.Utils.UI.Localization;
namespace DevExpress.XtraReports.Native {
	public class TreeListPickManager : PickManager {
		#region inner classes
		struct NodeInfo {
			string text;
			int level;
			public NodeInfo(string text, int level) {
				this.text = text;
				this.level = level;
			}
		}
		protected class DummyListNode : DataMemberListNodeBase {
			public DummyListNode(TreeListNodes owner)
				: base(owner) {
			}
			public override bool IsDummyNode { get { return true; } }
		}
		#endregion
		protected DataContextOptions options;
		IServiceProvider serviceProvider;
		public IServiceProvider ServiceProvider {
			get { return serviceProvider; }
			set { serviceProvider = value; }
		}
		public TreeListPickManager()
			: base() {
		}
		public TreeListPickManager(DataContextOptions options) {
			this.options = options;
		}
		protected override INode CreateDataMemberNode(object dataSource, string dataMember, string displayName, bool isList, object owner, IPropertyDescriptor property) {
			PropertyDescriptor propertyDescriptor = ((FakedPropertyDescriptor)property).RealProperty;
			TypeSpecifics specifics = GetTypeSpecificsService().GetPropertyTypeSpecifics(propertyDescriptor);
			int index = ColumnImageProvider.Instance.GetColumnImageIndex(propertyDescriptor, specifics != TypeSpecifics.ListSource ? specifics : TypeSpecifics.List);
			return isList ? new DataMemberListNode(dataSource, dataMember, displayName, this, (TreeListNodes)owner, propertyDescriptor, index) :
				new DataMemberListNode(dataSource, dataMember, displayName, null, (TreeListNodes)owner, propertyDescriptor, index);
		}
		protected virtual TypeSpecificsService GetTypeSpecificsService() {
			return new XRTypeSpecificService();
		}
		protected override INode CreateDataSourceNode(object dataSource, string dataMember, string name, object owner) {
			TypeSpecifics specifics = TypeSpecifics.List;
			if(dataSource != null) {
				Type type = GetDataSourceType(dataSource, dataMember);
				specifics = GetTypeSpecificsService().GetTypeSpecifics(type);
			}
			int index = ColumnImageProvider.Instance.GetDataSourceImageIndex(dataSource, specifics);
			return CreateDataSourceNodeCore(dataSource, name, (TreeListNodes)owner, index);
		}
		protected virtual INode CreateDataSourceNodeCore(object dataSource, string name, TreeListNodes owner, int index) {
			return new DataSourceListNode(dataSource, name, this, (TreeListNodes)owner, index);
		}
		Type GetDataSourceType(object dataSource, string dataMember) {
			if(ServiceProvider != null && !string.IsNullOrEmpty(dataMember)) {
				IDataContextService dataContextService = (IDataContextService)ServiceProvider.GetService(typeof(IDataContextService));
				if(dataContextService != null) {
					DataContext dataContext = dataContextService.CreateDataContext(new DataContextOptions());
					ListBrowser dataBrowser = dataContext.GetDataBrowser(dataSource, dataMember, true) as ListBrowser;
					if(dataBrowser != null)
						return dataBrowser.DataSourceType;
				}
			}
			return dataSource.GetType();
		}
		protected override INode CreateDummyNode(object owner) {
			return new DummyListNode((TreeListNodes)owner);
		}
		protected override object CreateNoneNode(object owner) {
			int index = ColumnImageProvider.Instance.GetNoneImageIndex();
			return new DataMemberListNodeBase(NoneNodeText, index, index, (TreeListNodes)owner, null);
		}
		protected virtual string NoneNodeText {
			get { return UtilsUILocalizer.GetString(UtilsUIStringId.NonePickerNodeText); }
		}
		protected override bool NodeIsEmpty(INode node) {
			return node is DataMemberListNodeBase && node.IsEmpty;
		}
		public override void FillContent(IList nodes, Collection<Pair<object, string>> dataSources, bool addNoneNode) {
			Dictionary<NodeInfo, bool> nodeExpandList = new Dictionary<NodeInfo, bool>();
			CollectNodeExpandList(nodes, nodeExpandList, 0);
			base.FillContent(nodes, dataSources, addNoneNode);
			SetNodeExpandList(nodes, nodeExpandList, 0);
		}
		void CollectNodeExpandList(IList nodes, Dictionary<NodeInfo, bool> nodeExpandList, int level) {
			foreach (XtraListNode node in nodes) {
				if (node == null || string.IsNullOrEmpty(node.Text))
					continue;
				NodeInfo key = new NodeInfo(node.Text, level);
				if (!nodeExpandList.ContainsKey(key))
					nodeExpandList.Add(key, node.Expanded);
				if (node.Expanded)
					CollectNodeExpandList((IList)node.Nodes, nodeExpandList, level + 1);
			}
		}
		void SetNodeExpandList(IList nodes, Dictionary<NodeInfo, bool> nodeExpandList, int level) {
			foreach (XtraListNode node in nodes) {
				if (node == null || string.IsNullOrEmpty(node.Text))
					continue;
				NodeInfo key = new NodeInfo(node.Text, level);
				bool expanded = false;
				if (nodeExpandList.TryGetValue(key, out expanded)) {
					node.Expanded = expanded;
					if (expanded) {
						if (node.Nodes.Count == 1 && ((INode)node.Nodes[0]).IsDummyNode)
							OnNodeExpand(((DataMemberListNodeBase)node).DataSource, ((INode)node));
						SetNodeExpandList((IList)node.Nodes, nodeExpandList, level + 1);
					}
				}
			}
		}
		protected IDataContextService GetDataContextService() {
			return ServiceProvider != null ? (IDataContextService)ServiceProvider.GetService(typeof(IDataContextService)) : null;
		}
		protected override IPropertiesProvider CreateProvider() {
			IDataContextService service = GetDataContextService();
			return service != null ? new DataSortedPropertiesNativeProvider(service.CreateDataContext(options), service, GetTypeSpecificsService()) :
				base.CreateProvider();
		}
	}
}
