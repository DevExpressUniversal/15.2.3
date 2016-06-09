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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Reports.UserDesigner.FieldList;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Parameters;
using System.Linq;
using DevExpress.Mvvm.Native;
using System.ComponentModel;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel.Native {
	public static class FieldListDataSource<TOwner> {
		abstract class ParameterNodeBase : FieldListNodeBase<TOwner>, INode {
			readonly string displayName;
			readonly TOwner owner;
			public ParameterNodeBase(TOwner owner, string displayName) {
				this.owner = owner;
				this.displayName = displayName;
			}
			public override string DisplayName { get { return displayName; } }
			public override TOwner Owner { get { return owner; } }
			public override string DataMember { get { return string.Empty; } }
			protected abstract IList ChildNodes { get; }
			#region INode
			public virtual bool IsDataMemberNode { get { return true; } }
			public override bool IsDataSourceNode { get { return false; } }
			object INode.Parent { get { throw new NotImplementedException(); } }
			bool INode.IsDummyNode { get { return false; } }
			bool INode.IsDataSourceNode { get { throw new NotImplementedException(); } }
			bool INode.IsEmpty { get { throw new NotImplementedException(); } }
			bool INode.IsList { get { return false; } }
			bool INode.IsComplex { get { return false; } }
			string INode.DataMember { get { return null; } }
			IList INode.ChildNodes { get { return ChildNodes; } }
			void INode.Expand(EventHandler callback) { }
			bool INode.HasDataSource(object dataSource) { return dataSource != null && dataSource.Equals(dataSource); }
			#endregion
		}
		class ParametersRootNode : ParameterNodeBase {
			readonly FieldListParameterNode[] childNodes;
			public ParametersRootNode(TOwner owner, FieldListParameterNode[] childNodes)
				: base(owner, "Parameters") {
				this.childNodes = childNodes;
				foreach(var childNode in childNodes)
					childNode.SetParent(this);
			}
			public override IEnumerable<FieldListNodeBase<TOwner>> Children { get { return childNodes; } }
			protected override IList ChildNodes { get { return childNodes; } }
			public override ImageSource Icon { get; set; }
			public override object DataSource { get { return null; } }
			public override string FullPath { get { return "Parameters"; } }
			public override bool IsDataSourceNode { get { return true; } }
			public override FieldListNodeBase<TOwner> Parent { get { return null; } }
			public override TypeSpecifics TypeSpecifics { get { return TypeSpecifics.ListOfParameters; } }
		}
		class FieldListParameterNode : ParameterNodeBase {
			readonly Parameter parameter;
			readonly TypeSpecifics typeSpecifics;
			ParametersRootNode parent;
			TypeSpecifics GetTypeSpecifics(Type parameterType) {
				if(parameterType == typeof(string)) return TypeSpecifics.String;
				if(parameterType == typeof(DateTime)) return TypeSpecifics.Date;
				if(parameterType == typeof(int) || parameterType == typeof(long) || parameterType == typeof(double) || parameterType == typeof(decimal) || parameterType == typeof(float))
					return TypeSpecifics.Integer;
				if(parameterType == typeof(bool)) return TypeSpecifics.Bool;
				return TypeSpecifics.Default;
			}
			public FieldListParameterNode(TOwner owner, string displayName, Parameter parameter)
				: base(owner, displayName) {
				this.parameter = parameter;
				this.typeSpecifics = parameter == null ? TypeSpecifics.ListOfParameters : GetTypeSpecifics(parameter.Type);
			}
			public void SetParent(ParametersRootNode parent) {
				this.parent = parent;
			}
			public override FieldListNodeBase<TOwner> Parent { get { return parent; } }
			public override string FullPath { get { return DisplayName; } }
			public override object DataSource { get { return parameter; } }
			public override ImageSource Icon { get; set; }
			public override TypeSpecifics TypeSpecifics { get { return typeSpecifics; } }
			public override IEnumerable<FieldListNodeBase<TOwner>> Children { get { return Enumerable.Empty<FieldListNodeBase<TOwner>>(); } }
			protected override IList ChildNodes { get { return new ArrayList(); } }
		}
		class FieldListNode : FieldListNodeBase<TOwner>, INode {
			readonly object dataSource;
			readonly string dataMember;
			readonly string displayName;
			string fullPath;
			readonly FieldListPickManager pickManager;
			ObservableCollection<FieldListNode> childNodes;
			readonly TOwner owner;
			readonly TypeSpecifics typeSpecifics;
			FieldListNodeBase<TOwner> parent;
			public FieldListNode(TOwner owner, FieldListPickManager pickManager, object dataSource, string dataMember, string displayName, TypeSpecifics typeSpecifics, params FieldListNode[] childNodes) {
				this.owner = owner;
				this.pickManager = pickManager;
				this.dataSource = dataSource;
				this.dataMember = dataMember;
				this.displayName = displayName;
				this.childNodes = new ObservableCollection<FieldListNode>(childNodes);
				this.typeSpecifics = typeSpecifics;
			}
			public void SetParent(FieldListNodeBase<TOwner> parent) {
				this.parent = parent;
				FieldListNodeBase<TOwner> listNode = this;
				listNode = LinqExtensions.Unfold(listNode, x => x.Parent, x => x == null).Last();
				fullPath = listNode == this ? DisplayName : string.Format("{0} - {1}", listNode.DisplayName, dataMember);
			}
			public override FieldListNodeBase<TOwner> Parent { get { return parent; } }
			public override string DisplayName { get { return displayName; } }
			public override TOwner Owner { get { return owner; } }
			public override ImageSource Icon { get; set; }
			public override string DataMember { get { return dataMember; } }
			public override object DataSource { get { return dataSource; } }
			public override TypeSpecifics TypeSpecifics { get { return typeSpecifics; } }
			public override string FullPath { get { return fullPath; } }
			IEnumerable<FieldListNode> children;
			public override IEnumerable<FieldListNodeBase<TOwner>> Children {
				get {
					if(children != null) return children;
					children = GetChildren();
					return children;
				}
			}
			ObservableCollection<FieldListNode> GetChildren() {
				if(pickManager == null) return new ObservableCollection<FieldListNode>();
				var nodes = new List<FieldListNode>();
				pickManager.FillNodes(dataSource, dataMember, nodes);
				foreach(var node in nodes[0].childNodes)
					node.SetParent(this);
				return nodes[0].childNodes;
			}
			#region INode
			public virtual bool IsDataMemberNode { get { return true; } }
			public override bool IsDataSourceNode { get { return false; } }
			public IList ChildNodes { get { return childNodes; } }
			object INode.Parent { get { throw new NotImplementedException(); } }
			bool INode.IsDummyNode { get { return false; } }
			bool INode.IsDataSourceNode { get { throw new NotImplementedException(); } }
			bool INode.IsEmpty { get { return dataSource == null; } }
			bool INode.IsList { get { return false; } }
			bool INode.IsComplex { get { return false; } }
			string INode.DataMember { get { return dataMember; } }
			void INode.Expand(EventHandler callback) { }
			bool INode.HasDataSource(object dataSource) { return dataSource != null && dataSource.Equals(dataSource); }
			#endregion
		}
		class FieldListPickManager : PickManagerBase {
			class FieldListRootNode : FieldListNode {
				public FieldListRootNode(TOwner parent, FieldListPickManager pickManager, object dataSource, string displayName) : base(parent, pickManager, dataSource, string.Empty, displayName, TypeSpecifics.ListSource) { }
				public override bool IsDataMemberNode { get { return false; } }
				public override bool IsDataSourceNode { get { return true; } }
			}
			class DesignTimeDataContextService : XRDataContextServiceBase {
				public DesignTimeDataContextService(XtraReport report) : base(report) { }
				protected override DataContext CreateDataContextInternal(DataContextOptions options) {
					return new XRDataContextBase(options.UseCalculatedFields ? CalculatedFields : null, SuppressListFilling);
				}
			}
			readonly XtraReport report;
			readonly TOwner owner;
			public FieldListPickManager(XtraReport report, TOwner owner) {
				this.report = report;
				this.owner = owner;
			}
			protected override INode CreateDataMemberNode(object dataSource, string dataMember, string displayName, bool isList, object parent, IPropertyDescriptor property) {
				var realProperty = (property as FakedPropertyDescriptor).Return(x=> x.RealProperty, ()=> null);
				var typeSpecifics = realProperty.Return(x => new XRTypeSpecificService().GetPropertyTypeSpecifics(x), () => property.Specifics);
				return new FieldListNode(this.owner, (isList || property.IsComplex) ? this : null, dataSource, dataMember, displayName, typeSpecifics);
			}
			protected override INode CreateDataSourceNode(object dataSource, string dataMember, string name, object parent) {
				return new FieldListRootNode(this.owner, this, dataSource, name);
			}
			protected override INode CreateDummyNode(object parent) {
				return null;
			}
			protected override object CreateNoneNode(object parent) {
				throw new NotSupportedException();
			}
			protected override IPropertiesProvider CreateProvider() {
				var service = new DesignTimeDataContextService(report);
				return new DataSortedPropertiesNativeProvider(service.CreateDataContext(new DataContextOptions(true, true)), service, new XRTypeSpecificService());
			}
			protected override bool NodeIsEmpty(INode node) {
				return node.IsEmpty;
			}
		}
		public static IEnumerable<FieldListNodeBase<TOwner>> GetDataSources(XtraReport report, TOwner owner) {
			var pickManager = new FieldListPickManager(report, owner);
			var nodes = new List<FieldListNode>();
			foreach(var datasource in report.GetDataSources()) {
				var dataSourceRootNode = new List<FieldListNode>();
				pickManager.FillContent(dataSourceRootNode, new List<object>() { datasource }, false);
				var dataSourceNode = dataSourceRootNode.FirstOrDefault();
				if(dataSourceNode != null) {
					nodes.Add(dataSourceNode);
					dataSourceNode.SetParent(null);
				}
			}
			return nodes;
		}
		public static FieldListNodeBase<TOwner> GetParameters(XtraReport report, TOwner owner) {
			return new ParametersRootNode(owner, report.GetParameters().Select(x => new FieldListParameterNode(owner, x.Name, x)).ToArray());
		}
	}
}
