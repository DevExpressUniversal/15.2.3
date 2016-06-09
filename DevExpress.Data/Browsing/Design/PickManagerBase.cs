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
using System.ComponentModel;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Data.Browsing.Design {
	public class TypeSpecificsService : ITypeSpecificsService {
		public virtual TypeSpecifics GetPropertyTypeSpecifics(PropertyDescriptor property) {
			return GetTypeSpecifics(property.PropertyType);
		}
		public virtual TypeSpecifics GetTypeSpecifics(Type type) {
			if(typeof(IListSource).IsAssignableFrom(type))
				return TypeSpecifics.ListSource;
			if(type.IsArray)
				return TypeSpecifics.Array;
			if(ListTypeHelper.IsListType(type))
				return TypeSpecifics.List;
			Type validType = ValidateType(type);
			if(validType == typeof(System.String))
				return TypeSpecifics.String;
			if(PSNativeMethods.IsNumericalType(validType))
				return TypeSpecifics.Integer;
			if(PSNativeMethods.IsFloatType(validType))
				return TypeSpecifics.Float;
			if(validType == typeof(bool))
				return TypeSpecifics.Bool;
			if(validType == typeof(DateTime) || validType == typeof(TimeSpan))
				return TypeSpecifics.Date;
			if(validType == typeof(Guid))
				return TypeSpecifics.Guid;
			return TypeSpecifics.Default;
		}
		protected static Type ValidateType(Type value) {
			if(value.IsGenericType() && value.GetGenericTypeDefinition() == typeof(Nullable<>))
				return value.GetGenericArguments()[0];
			return value;
		}
	}
	public interface ITypeSpecificsService {
		TypeSpecifics GetPropertyTypeSpecifics(PropertyDescriptor property);
		TypeSpecifics GetTypeSpecifics(Type type);
	}
	public enum TypeSpecifics {
		None,
		List,
		ListSource,
		ListOfParameters,
		Default,
		String,
		Integer,
		Float,
		Date,
		Bool,
		Guid,
		Array,
		CalcDefault,
		CalcString,
		CalcInteger,
		CalcFloat,
		CalcDate,
		CalcBool,
		CalcGuid
	}
	public interface INode {
		bool IsDummyNode { get; }
		bool IsDataMemberNode { get; }
		bool IsDataSourceNode { get; }
		bool IsEmpty { get; }
		bool IsList { get; }
		bool IsComplex { get; }
		IList ChildNodes { get; }
		object Parent { get; }
		string DataMember { get; }
		void Expand(EventHandler callback);
		bool HasDataSource(object dataSource);
	}
	public interface IPropertyDescriptor {
		TypeSpecifics Specifics { get; }
		bool IsComplex { get; }
		bool IsListType { get; }
		string Name { get; }
		string DisplayName { get; }
	}
	public class GetPropertiesEventArgs : EventArgs {
		public IPropertyDescriptor[] Properties { get; private set; }
		public GetPropertiesEventArgs(IPropertyDescriptor[] properties) {
			Properties = properties;
		}
	}
	public class GetDataSourceDisplayNameEventArgs : EventArgs {
		public string DataSourceDisplayName { get; private set; }
		public GetDataSourceDisplayNameEventArgs(string dataSourceDisplayName) {
			DataSourceDisplayName = dataSourceDisplayName;
		}
	}
	public interface IPropertiesProvider {
		void GetItemProperties(object dataSource, string dataMember, EventHandler<GetPropertiesEventArgs> callback);
		void GetListItemProperties(object dataSource, string dataMember, EventHandler<GetPropertiesEventArgs> callback);
		void GetDataSourceDisplayName(object dataSource, string dataMember, EventHandler<GetDataSourceDisplayNameEventArgs> callback);
	}
	public class ActionExecutor {
		class ActionEnumerator : IEnumerator {
			readonly ActionExecutor executor;
			public ActionEnumerator(ActionExecutor executor) {
				this.executor = executor;
			} 
			public object Current {
				get { throw new NotSupportedException(); }
			}
			public bool MoveNext() {
				return executor.RunNextAction();
			}
			public void Reset() {
				throw new NotSupportedException();
			}
		}
		readonly Queue<Action<IEnumerator>> actions = new Queue<Action<IEnumerator>>();
		public void AddAction(Action<IEnumerator> action) {
			actions.Enqueue(action);
			if(actions.Count == 1)
				RunAction();
		}
		public void AddAction(Action action) {
			AddAction(delegate(IEnumerator e) {
				action();
				e.MoveNext();
			});
		}
		bool RunAction() {
			if(actions.Count > 0) {
				Action<IEnumerator> action = actions.Peek();
				action(new ActionEnumerator(this));
				return true;
			}
			return false;
		}
		bool RunNextAction() {
			actions.Dequeue();
			return RunAction();
		}
	}
	public abstract class PickManagerBase {
		readonly ActionExecutor executor = new ActionExecutor();
		public ActionExecutor Executor {
			get { return executor; }
		}
		protected PickManagerBase() {
		}
		public virtual void FillNodes(object dataSource, string dataMember, IList nodes) {
			executor.AddAction(enumerator => {
				IPropertiesProvider provider = CreateProvider();
				GetDataSourceName(dataSource, dataMember, provider, (s1, e1) => {
					INode sourceNode = CreateDataSourceNode(dataSource, dataMember, e1.DataSourceDisplayName, nodes);
					nodes.Add(sourceNode);
					provider.GetItemProperties(dataSource, dataMember, (s2, e2) => {
						if(e2.Properties.Length > 0)
							AddChildNodes(e2.Properties, dataSource, dataMember, sourceNode.ChildNodes);
						DisposeObject(provider);
						enumerator.MoveNext();
					});
				});
			});
		}
		protected static void DisposeObject(object obj) {
			if(obj is IDisposable)
				((IDisposable)obj).Dispose();
		}
		void AddChildNodes(IList<IPropertyDescriptor> properties, object dataSource, string dataMember, IList nodes) {
			foreach(IPropertyDescriptor property in properties) {
				INode node = CreateChildNode(dataSource, GetFullName(dataMember, property.Name), property.DisplayName, nodes, property);
				nodes.Add(node);
			}
		}
		protected static string GetFullName(string dataMember, string name) {
			return dataMember != null && dataMember.Length > 0 ? String.Concat(dataMember, ".", name) : name;
		}
		static bool IsListSource(object obj) {
			return obj is IListSource || obj is IList;
		}
		INode CreateChildNode(object dataSource, string dataMember, string displayName, object owner, IPropertyDescriptor property) {
			INode node = CreateDataMemberNode(dataSource, dataMember, displayName, property.IsListType, owner, property);
			if(ShouldAddDummyNode(property))
				node.ChildNodes.Add(CreateDummyNode(node.ChildNodes));
			return node;
		}
		protected virtual bool ShouldAddDummyNode(IPropertyDescriptor property) {
			return property.IsComplex;
		}
		public static bool ContainsDummyNode(IList list) {
			return list.Count == 1 && ((INode)list[0]).IsDummyNode;
		}
		public bool AreContainDummyNode(IList nodes) {
			return ContainsDummyNode(nodes);
		}
		public void OnNodeExpand(object dataSource, INode node) {
			IList childNodes = node.ChildNodes;
			if(!ContainsDummyNode(childNodes))
				return;
			executor.AddAction(enumerator => {
				string dataMember = GetDataMember(node);
				IPropertiesProvider provider = CreateProvider();
				provider.GetListItemProperties(dataSource, dataMember, (s, e) => {
					childNodes.RemoveAt(0);
					AddChildNodes(e.Properties, dataSource, dataMember, childNodes);
					DisposeObject(provider);
					enumerator.MoveNext();
				});
			});
		}
		public virtual void GetDataSourceName(object dataSource, string dataMember, IPropertiesProvider provider, EventHandler<GetDataSourceDisplayNameEventArgs> callback) {
			provider.GetDataSourceDisplayName(dataSource, dataMember, callback);
		}
		public virtual void FillContent(IList nodes, Collection<Pair<object, string>> dataSources, bool addNoneNode) {
			nodes.Clear();
			Collection<Pair<object, string>> filterValues = FilterDataSources(dataSources);
			foreach(Pair<object, string> pair in filterValues)
				FillContentCore(nodes, pair.First, pair.Second);
			executor.AddAction(delegate() {
				if(addNoneNode)
					nodes.Add(CreateNoneNode(nodes));
				if(nodes.Count > 0)
					((INode)nodes[0]).Expand(null);
			});
		}
		public void FillContent(IList nodes, IList dataSources, bool addNoneNode) {
			Collection<Pair<object, string>> dataSourcesPairs = new Collection<Pair<object, string>>();
			foreach(object dataSource in dataSources)
				dataSourcesPairs.Add(new Pair<object, string>(dataSource, string.Empty));
			FillContent(nodes, dataSourcesPairs, addNoneNode);
		}
		void FillContentCore(IList nodes, object dataSource, string dataMember) {
			if(FindSourceNode(nodes, dataSource) == null)
				FillNodes(dataSource, dataMember, nodes);
		}
		protected virtual Collection<Pair<object, string>> FilterDataSources(Collection<Pair<object, string>> dataSources) {
			Collection<Pair<object, string>> filterValues = new Collection<Pair<object, string>>();
			foreach(Pair<object, string> pair in dataSources)
				if(IsListSource(pair.First))
					filterValues.Add(pair);
			return filterValues;
		}
		public INode FindSourceNode(IList nodes, object dataSource) {
			foreach(INode node in nodes)
				if(node.HasDataSource(dataSource))
					return node;
			return null;
		}
		public INode GetDataSourceNode(INode node) {
			while(!node.IsDataSourceNode)
				node = (INode)node.Parent;
			return node.IsDataSourceNode ? node : null;
		}
		public object FindNoneNode(IList nodes) {
			foreach(INode node in nodes)
				if(NodeIsEmpty(node))
					return node;
			return null;
		}
		public object FindDataMemberNode(IList nodes, object dataSource, string dataMember) {
			INode dataSourceNode = FindSourceNode(nodes, dataSource);
			if(dataSourceNode != null)
				return FindDataMemberNode(dataSourceNode.ChildNodes, dataMember);
			return null;
		}
		public virtual INode FindDataMemberNode(IList nodes, string dataMember) {
			INode result = null;
			FindDataMemberNode(nodes, dataMember, node => result = node);
			return result;
		}
		public virtual void FindDataMemberNode(IList nodes, string dataMember, Action<INode> callback) {
			FindDataMemberNodeCore(nodes, dataMember, 0, callback, false);
		}
		protected void FindDataMemberNodeCore(IList nodes, string dataMember, int i, Action<INode> callback, bool joinItems) {
			string[] items = dataMember.Split('.');
			if(i >= items.Length) {
				InvokeCallback(callback, null);
				return;
			}
			INode node = FindNode(nodes, dataMember);
			if(node != null) {
				InvokeCallback(callback, node);
				return;
			}
			node = FindNode(nodes, String.Join(".", items, 0, i + 1));
			if(joinItems)
				while(node == null && i < items.Length - 1)
					node = FindNode(nodes, String.Join(".", items, 0, ++i + 1));
			if(node == null || i == items.Length - 1) {
				InvokeCallback(callback, node);
				return;
			}
			node.Expand((sender, e) => {
				nodes = node.ChildNodes;
				FindDataMemberNodeCore(nodes, dataMember, i + 1, callback, joinItems);
			});
		}
		static void InvokeCallback(Action<INode> callback, INode node) {
			if(callback != null)
				callback(node);
		}
		protected INode FindNode(IList nodes, string dataMember) {
			foreach(INode node in nodes)
				if(GetDataMember(node) == dataMember)
					return node;
			return null;
		}
		protected virtual string GetDataMember(INode node) {
			return node.IsDataMemberNode ? node.DataMember : string.Empty;
		}
		protected abstract IPropertiesProvider CreateProvider();
		protected abstract INode CreateDataSourceNode(object dataSource, string dataMember, string name, object owner);
		protected abstract INode CreateDataMemberNode(object dataSource, string dataMember, string displayName, bool isList, object owner, IPropertyDescriptor property);
		protected abstract INode CreateDummyNode(object owner);
		protected abstract object CreateNoneNode(object owner);
		protected abstract bool NodeIsEmpty(INode node);
	}
}
#if !SL
namespace DevExpress.XtraReports.Native.Data {
	using DevExpress.Data.Browsing.Design;
	using DevExpress.Data.Browsing;
	public class XRTypeSpecificService : TypeSpecificsService {
		Dictionary<TypeSpecifics, TypeSpecifics> dict = new Dictionary<TypeSpecifics, TypeSpecifics>();
		public XRTypeSpecificService() {
			dict.Add(TypeSpecifics.Bool, TypeSpecifics.CalcBool);
			dict.Add(TypeSpecifics.Date, TypeSpecifics.CalcDate);
			dict.Add(TypeSpecifics.Default, TypeSpecifics.CalcDefault);
			dict.Add(TypeSpecifics.Float, TypeSpecifics.CalcFloat);
			dict.Add(TypeSpecifics.Integer, TypeSpecifics.CalcInteger);
			dict.Add(TypeSpecifics.String, TypeSpecifics.CalcString);
			dict.Add(TypeSpecifics.Guid, TypeSpecifics.CalcGuid);
		}
		public override TypeSpecifics GetPropertyTypeSpecifics(PropertyDescriptor property) {
			TypeSpecifics result = base.GetPropertyTypeSpecifics(property);
			if (property is CalculatedPropertyDescriptorBase) {
				TypeSpecifics calcResult = TypeSpecifics.Default;
				if (dict.TryGetValue(result, out calcResult))
					return calcResult;
			}
			return result;
		}
		public override TypeSpecifics GetTypeSpecifics(Type type) {
			if (typeof(DevExpress.XtraReports.Native.Parameters.ParametersDataSource).IsAssignableFrom(type))
				return TypeSpecifics.ListOfParameters;
			TypeSpecifics result = base.GetTypeSpecifics(type);
			if (typeof(CalculatedPropertyDescriptorBase).IsAssignableFrom(type)) {
				TypeSpecifics calcResult = TypeSpecifics.Default;
				if (dict.TryGetValue(result, out calcResult))
					return calcResult;
			}
			return result;
		}
	}
}
#endif
