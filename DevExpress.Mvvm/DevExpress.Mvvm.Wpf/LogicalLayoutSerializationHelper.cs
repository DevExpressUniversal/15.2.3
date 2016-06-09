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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
namespace DevExpress.Mvvm {
	public interface ISupportLogicalLayout {
		bool CanSerialize { get; }
		IDocumentManagerService DocumentManagerService { get; }
		IEnumerable<object> LookupViewModels { get; }
	}
	public interface ISupportLogicalLayout<T> : ISupportLogicalLayout {
		T SaveState();
		void RestoreState(T state);
	}
	public static class LogicalLayoutSerializationHelper {
		[DataContract]
		class SerializedDocument {
			[DataMember]
			public string DocumentType { get; set; }
			[DataMember]
			public string ViewModelState { get; set; }
			[DataMember]
			public string DocumentId { get; set; }
			[DataMember]
			public string DocumentTitle { get; set; }
			[DataMember]
			public bool IsVisible { get; set; }
			[DataMember]
			public List<SerializedDocument> Children { get; set; }
			public SerializedDocument() { }
			public SerializedDocument(string documentType, string viewModelState, string documentId, string documentTitle, bool isVisible) {
				DocumentType = documentType;
				ViewModelState = viewModelState;
				DocumentId = documentId;
				DocumentTitle = documentTitle;
				IsVisible = isVisible;
				Children = new List<SerializedDocument>();
			}
		}
		static string Serialize<S, T>(S serializer, T value, Action<S, Stream, T> serialize) {
			using(var ms = new MemoryStream()) {
				serialize(serializer, ms, value);
				ms.Seek(0, SeekOrigin.Begin);
				using(var reader = new StreamReader(ms)) {
					return reader.ReadToEnd();
				}
			}
		}
		static T Deserialize<S, T>(S serializer, string value, Func<S, Stream, object> deserialize) {
			try {
				using(var ms = new MemoryStream(Encoding.UTF8.GetBytes(value))) {
					return (T)deserialize(serializer, ms);
				}
			} catch {
				return default(T);
			}
		}
		static string Serialize<T>(XmlSerializer serializer, T value) {
			return Serialize<XmlSerializer, T>(
				serializer,
				value,
				(s, stream, v) => s.Serialize(stream, v));
		}
		static string SerializeDataContract<T>(T value) {
			return Serialize<DataContractSerializer, T>(
				new DataContractSerializer(typeof(T)),
				value,
				(s, stream, v) => s.WriteObject(stream, v));
		}
		static T Deserialize<T>(XmlSerializer serializer, string value) {
			return Deserialize<XmlSerializer, T>(
				serializer,
				value,
				(s, stream) => s.Deserialize(stream));
		}
		static T DeserializeDataContract<T>(string value) {
			if(string.IsNullOrEmpty(value)) return default(T);
			return Deserialize<DataContractSerializer, T>(
				new DataContractSerializer(typeof(T)),
				value,
				(s, stream) => s.ReadObject(stream));
		}
		static object InvokeInterfaceMethod(object obj, Type interfaceType, string name, params object[] arguments) {
			var map = obj.GetType().GetInterfaceMap(interfaceType);
			MethodInfo method = null;
			for(int i = 0; i < map.InterfaceMethods.Length; i++) {
				if(map.InterfaceMethods[i].Name == name) {
					method = map.TargetMethods[i];
				}
			}
			return method.Invoke(obj, arguments);
		}
		static IDocument GetDocument(object viewModel) {
			var parent = GetParent(viewModel);
			if(parent == null)
				return null;
			var service = GetService(parent);
			return service.Documents.FirstOrDefault(d => d.Content == viewModel);
		}
		static IDocumentManagerService GetService(object viewModel) {
			IDocumentManagerService service = null;
			while(service == null) {
				var typed = viewModel as ISupportLogicalLayout;
				service = typed.DocumentManagerService;
				viewModel = GetParent(viewModel);
			}
			return service;
		}
		static IEnumerable<object> GetPath(object viewModel) {
			while(viewModel != null) {
				yield return viewModel;
				viewModel = GetParent(viewModel);
			}
		}
		static object GetParent(object viewModel) {
			var typed = viewModel as ISupportParentViewModel;
			if(typed == null || typed == typed.ParentViewModel)
				return null;
			return typed.ParentViewModel;
		}
		class LogicalNode {
			ISupportLogicalLayout primaryViewModel;
			public LogicalNode(LogicalNode parent, IDocument document, IDocumentManagerService service, ISupportLogicalLayout primaryViewModel = null) {
				Parent = parent;
				Document = document;
				Service = service;
				Children = new List<LogicalNode>();
				this.primaryViewModel = primaryViewModel;
			}
			public LogicalNode Parent { get; private set; }
			public IDocument Document { get; private set; }
			public IDocumentManagerService Service { get; private set; }
			public List<LogicalNode> Children { get; private set; }
			IDocumentInfo DocumentInfo { get { return Document as IDocumentInfo; } }
			public string DocumentType {
				get {
					return DocumentInfo == null ? null : DocumentInfo.DocumentType;
				}
			}
			public void Cull() {
				if(Parent != null) {
					Parent.Children.Remove(this);
				}
			}
			public bool IsVisible {
				get {
					return DocumentInfo != null && DocumentInfo.State == DocumentState.Visible;
				}
			}
			public ISupportLogicalLayout PrimaryViewModel {
				get { return primaryViewModel ?? Document.Content as ISupportLogicalLayout; }
			}
		}
		static void DepthFirstSearch(LogicalNode tree, Action<LogicalNode> action) {
			tree.Children.ForEach(c => DepthFirstSearch(c, action));
			action(tree);
		}
		static IEnumerable<LogicalNode> GetPath(LogicalNode node) {
			while(node != null) {
				yield return node;
				node = node.Parent;
			}
		}
		static LogicalNode BuildTree(IDocument document, ISupportLogicalLayout primaryViewModel, LogicalNode parent = null) {
			var viewModels = new List<object>();
			viewModels.Add(primaryViewModel);
			if(primaryViewModel.LookupViewModels != null) {
				viewModels.AddRange(primaryViewModel.LookupViewModels);
			}
			var node = new LogicalNode(parent, document, primaryViewModel.DocumentManagerService, primaryViewModel);
			var primaryViewModelPath = GetPath(node).Skip(1).Select(n => n.PrimaryViewModel).ToList();
			if (!primaryViewModelPath.Contains(primaryViewModel)) { 
				node.Children.AddRange(
					from childDoc in GetImmediateChildren(primaryViewModel, viewModels)
					let childViewModel = childDoc.Content as ISupportLogicalLayout
					where childDoc != document && childViewModel != null && childViewModel.CanSerialize
					select BuildTree(childDoc, childViewModel, node));
			}
			return node;
		}
		static IEnumerable<IDocument> CollectDocuments(LogicalNode tree) {
			var documents = new List<IDocument>();
			DepthFirstSearch(tree, n => documents.Add(n.Document));
			return documents;
		}
		static SerializedDocument SerializeTree(LogicalNode node) {
			Type logicalLayoutType = GetISupportLogicalLayout(node.PrimaryViewModel);
			string state = null;
			if(logicalLayoutType != null) {
				state = Serialize(node.PrimaryViewModel, logicalLayoutType);
			}
			if(node.DocumentType == null)
				return null;
			var serialized = new SerializedDocument(node.DocumentType, state, (string)node.Document.Id, node.Document.Title as string, node.IsVisible);
			serialized.Children.AddRange(node.Children.Select(SerializeTree).Where(child => child != null));
			return serialized;
		}
		public static List<IDocument> GetOrphanedDocuments(this ISupportLogicalLayout viewModel) {
			return TrimLogicalTree(BuildTree(null, viewModel), viewModel).Select(n => n.Document).ToList();
		}
		static List<LogicalNode> GetOrphanedLeafs(LogicalNode root, ISupportLogicalLayout viewModel) {
			var orphans = new List<LogicalNode>();
			VisitOrphans(root, n => {
				if(n.Document != null) {
					orphans.Add(n);
				}
			});
			return orphans;
		}
		static List<LogicalNode> TrimLogicalTree(LogicalNode root, ISupportLogicalLayout viewModel) {
			var allOrphans = new List<LogicalNode>();
			List<LogicalNode> orphans;
			while((orphans = GetOrphanedLeafs(root, viewModel)).Any()) {
				allOrphans.AddRange(orphans);
				orphans.ForEach(n => n.Cull());
			}
			return allOrphans;
		}
		static List<SerializedDocument> SerializeViews(ISupportLogicalLayout viewModel) {
			LogicalNode tree = BuildTree(null, viewModel);
			TrimLogicalTree(tree, viewModel);
			return tree.Children.Select(SerializeTree).ToList();
		}
		static void VisitOrphans(LogicalNode tree, Action<LogicalNode> action) {
			DepthFirstSearch(tree, n => {
				if(!n.IsVisible && !n.Children.Any()) {
					action(n);
				}
			});
		}
		static string Serialize(ISupportLogicalLayout content, Type logicalLayoutType) {
			object objState = InvokeInterfaceMethod(content, logicalLayoutType, "SaveState");
			return Serialize(new XmlSerializer(logicalLayoutType.GetGenericArguments().Single()), objState);
		}
		static Type GetISupportLogicalLayout(object content) {
			if(content == null)
				return null;
			return content.GetType().GetInterfaces().FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ISupportLogicalLayout<>));
		}
		public static IEnumerable<IDocument> GetImmediateChildren(this ISupportLogicalLayout parent, IEnumerable<object> viewModels = null) {
			if (parent == null)
				yield break;
			var service = parent.DocumentManagerService;
			if(viewModels == null) {
				viewModels = new List<object> { parent };
			}
			if(parent.DocumentManagerService == null)
				yield break;
			foreach(var document in service.Documents) {
				var viewModel = document.Content as ISupportParentViewModel;
				if(viewModel != null && viewModels.Contains(viewModel.ParentViewModel)) {
					yield return document;
				}
			}
		}
		[Obsolete]
		public static void RestoreDocumentManagerService(string state, ISupportLogicalLayout parent) {
			parent.RestoreDocumentManagerService(state);
		}
		public static void RestoreDocumentManagerService(this ISupportLogicalLayout parent, string state) {
			var list = DeserializeDataContract<List<SerializedDocument>>(state);
			if(list == null)
				return;
			RestoreDocumentManagerService(list, parent);
		}
		public static string SerializeDocumentManagerService(this ISupportLogicalLayout viewModel) {
			if(viewModel == null || viewModel.DocumentManagerService == null)
				return string.Empty;
			var views = SerializeViews(viewModel);
			return SerializeDataContract(views);
		}
		static void RestoreDocumentManagerService(List<SerializedDocument> children, ISupportLogicalLayout rootViewModel) {
			foreach(var document in GetImmediateChildren(rootViewModel).ToList()) {
				document.Close();
			}
			foreach(var child in children) {
				IDocument document = rootViewModel.DocumentManagerService.CreateDocument(child.DocumentType, null, rootViewModel);
				document.Id = child.DocumentId;
				document.Title = child.DocumentTitle;
				if(child.IsVisible) {
					document.DestroyOnClose = false;
					document.Show();
				}
				Type logicalLayoutType = GetISupportLogicalLayout(document.Content);
				if(logicalLayoutType != null) {
					Deserialize((ISupportLogicalLayout)document.Content, logicalLayoutType, child.ViewModelState);
				}
				var viewModel = document.Content as ISupportLogicalLayout;
				if (viewModel != null) {
					RestoreDocumentManagerService(child.Children, viewModel);
				}
			}
		}
		public static string Serialize(this ISupportLogicalLayout content) {
			Type logicalLayoutType = GetISupportLogicalLayout(content);
			if(logicalLayoutType == null)
				throw new InvalidOperationException();
			return Serialize(content, logicalLayoutType);
		}
		static void Deserialize(ISupportLogicalLayout content, string state) {
			Type logicalLayoutType = GetISupportLogicalLayout(content);
			if(logicalLayoutType == null)
				throw new InvalidOperationException();
			Deserialize(content, logicalLayoutType, state);
		}
		static void Deserialize(ISupportLogicalLayout content, Type logicalLayoutType, string state) {
			var deserialized = Deserialize<object>(new XmlSerializer(logicalLayoutType.GetGenericArguments().Single()), state);
			InvokeInterfaceMethod(content, logicalLayoutType, "RestoreState", deserialized);
		}
		[DataContract]
		class StringPair {
			[DataMember]
			public string Key { get; set; }
			[DataMember]
			public string Value { get; set; }
		}
		public static string Serialize(Dictionary<string, string> dictionary) {
			var list = dictionary.Select(p => new StringPair { Key = p.Key, Value = p.Value }).ToList();
			return SerializeDataContract(list);
		}
		public static Dictionary<string, string> Deserialize(string serialized) {
			var res = DeserializeDataContract<List<StringPair>>(serialized);
			if(res == null) return new Dictionary<string, string>();
			return res.ToDictionary(p => p.Key, p => p.Value);
		}
	}
}
