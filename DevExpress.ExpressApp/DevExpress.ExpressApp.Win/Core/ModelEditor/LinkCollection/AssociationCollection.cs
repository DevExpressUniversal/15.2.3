#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public class AssociationCollection : IDisposable {
		private readonly object lockObject = new object();
		private Dictionary<string, AssociationItem> collection = new Dictionary<string, AssociationItem>();
		private AssociationInfoCollection associationInfos = new AssociationInfoCollection();
		public void ClearAssociation() {
			lock(lockObject) {
				ClearAssociationCore();
				collection = new Dictionary<string, AssociationItem>();
				associationInfos = new AssociationInfoCollection();
			}
		}
		public bool RemoveLink(ModelNode rootNode, ModelNode modelNode, string propertyName) {
			string key = GetNodeKey(rootNode);
			lock(lockObject) {
				AssociationInfo associationInfo = associationInfos.GetAssociation(modelNode.GetType(), propertyName);
				AssociationItem associationItem;
				if(collection.TryGetValue(key, out associationItem)) {
					return associationItem.RemoveLink(associationInfo, modelNode);
				}
			}
			return false;
		}
		public IEnumerable<AssociationInfo> GetAssociations(Type rootType, Type collectionElementType) {
			lock(lockObject) {
				return associationInfos.GetAssociations(rootType, collectionElementType);
			}
		}
		public IEnumerable<AssociationInfo> GetAssociations(Type collectionElementType) {
			lock(lockObject) {
				return associationInfos.GetAssociations(collectionElementType);
			}
		}
		public void AddAssociation(Type rootType, Type collectionElementType, string propertyName, string[] path) {
			lock(lockObject) {
				associationInfos.AddAssociation(rootType, collectionElementType, propertyName, path);
			}
		}
		public bool AddLink(ModelNode rootNode, ModelNode modelNode, string propertyName) {
			AssociationInfo associationInfo;
			lock(lockObject) {
				associationInfo = associationInfos.AddAssociation(modelNode.GetType(), rootNode.GetType(), propertyName, null);
				AssociationItem associationItem;
				string key = GetNodeKey(rootNode);
				if(!collection.TryGetValue(key, out associationItem)) {
					associationItem = new AssociationItem();
					collection.Add(key, associationItem);
				}
				return associationItem.AddLink(associationInfo, modelNode);
			}
		}
		public AssociationItem GetLinks(ModelNode rootNode) {
			AssociationItem item;
			collection.TryGetValue(GetNodeKey(rootNode), out item);
			return item;
		}
		private void ClearAssociationCore() {
			lock(lockObject) {
				if(collection != null) {
					foreach(AssociationItem item in collection.Values) {
						item.Dispose();
					}
					collection.Clear();
				}
				if(associationInfos != null) {
					associationInfos.Dispose();
				}
				associationInfos = null;
				collection = null;
			}
		}
		private string GetNodeKey(ModelNode node) {
			if(node is IModelApplication) {
				return typeof(IModelApplication).Name;
			}
			return ModelEditorHelper.GetModelNodePath(node) + node.Id;
		}
		#region IDisposable Members
		public void Dispose() {
			ClearAssociationCore();
		}
		#endregion
		private class AssociationInfoCollection : IDisposable {
			private readonly object lockObject = new object();
			List<AssociationInfo> aliasesCollection = new List<AssociationInfo>();
			public AssociationInfo AddAssociation(Type rootType, Type propertyType, string propertyName, string[] path) {
				lock(lockObject) {
					AssociationInfo result = null;
					foreach(AssociationInfo aliases in aliasesCollection) {
						if(aliases.RootType == rootType && aliases.AssociationProperty == propertyName && aliases.CollectionElementType == propertyType) {
							result = aliases;
						}
					}
					if(result == null) {
						result = new AssociationInfo(rootType, propertyType, propertyName, path);
						aliasesCollection.Add(result);
					}
					return result;
				}
			}
			public AssociationInfo GetAssociation(Type rootType, string propertyName) {
				lock(lockObject) {
					foreach(AssociationInfo aliases in aliasesCollection) {
						if(aliases.RootType == rootType && aliases.AssociationProperty == propertyName) {
							return aliases;
						}
					}
				}
				return null;
			}
			public IEnumerable<AssociationInfo> GetAssociations(Type rootType, Type collectionElementType) {
				lock(lockObject) {
					foreach(AssociationInfo aliases in aliasesCollection) {
						if(aliases.RootType == rootType && aliases.CollectionElementType == collectionElementType) {
							yield return aliases;
						}
					}
				}
			}
			public IEnumerable<AssociationInfo> GetAssociations(Type collectionElementType) {
				lock(lockObject) {
					foreach(AssociationInfo aliases in aliasesCollection) {
						if(aliases.CollectionElementType == collectionElementType) {
							yield return aliases;
						}
					}
				}
			}
			#region IDisposable Members
			public void Dispose() {
				if(aliasesCollection != null) {
					foreach(AssociationInfo item in aliasesCollection) {
						item.Dispose();
					}
					aliasesCollection.Clear();
					aliasesCollection = null;
				}
			}
			#endregion
		}
#if DebugTest
		public int DebugTest_AssociationCount {
			get { return collection.Count; }
		}
#endif
	}
	public class AssociationItem : IDisposable {
		private Dictionary<AssociationInfo, List<ModelNode>> links = new Dictionary<AssociationInfo, List<ModelNode>>();
		public bool AddLink(AssociationInfo associationInfo, ModelNode item) {
			List<ModelNode> list;
			if(!Links.TryGetValue(associationInfo, out list)) {
				list = new List<ModelNode>();
				Links.Add(associationInfo, list);
			}
			foreach(ModelNode node in list) {
				if(ModelEditorHelper.IsNodeEqual(item, node)) {
					return false;
				}
			}
			list.Add(item);
			return true;
		}
		public bool RemoveLink(AssociationInfo associationInfo, ModelNode item) {
			if(associationInfo == null) { return false; }
			List<ModelNode> list;
			if(Links.TryGetValue(associationInfo, out list)) {
				ModelNode itemToRemove = item;
				foreach(ModelNode node in list) {
					if(ModelEditorHelper.IsNodeEqual(item, node)) {
						itemToRemove = node;
						break;
					}
				}
				return list.Remove(itemToRemove);
			}
			return false;
		}
		public Dictionary<AssociationInfo, List<ModelNode>> Links {
			get {
				return links;
			}
		}
		#region IDisposable Members
		public void Dispose() {
			if(links != null) {
				foreach(KeyValuePair<AssociationInfo, List<ModelNode>> item in links) {
					item.Value.Clear();
					item.Key.Dispose();
				}
				links = null;
			}
		}
		#endregion
	}
	public class AssociationInfo : IDisposable {
		private static AssociationInfo dummyAssociationInfo = null;
		private Type rootType;
		private Type collectionElementType;
		private string associationProperty;
		private string[] path;
		public AssociationInfo(Type rootType, Type collectionElementType, string associationProperty, string[] path) {
			this.rootType = rootType;
			this.collectionElementType = collectionElementType;
			this.associationProperty = associationProperty;
			this.path = path;
		}
		public Type RootType {
			get {
				return rootType;
			}
		}
		public Type CollectionElementType {
			get {
				return collectionElementType;
			}
		}
		public string AssociationProperty {
			get {
				return associationProperty;
			}
		}
		public string[] Path {
			get {
				return path;
			}
		}
		public static AssociationInfo DummyAssociationInfo {
			get {
				if(dummyAssociationInfo == null) {
					dummyAssociationInfo = new AssociationInfo(null, null, null, null);
				}
				return dummyAssociationInfo;
			}
		}
		public override string ToString() {
			return String.Format("{0} -> {1} -> {2}", RootType.Name, AssociationProperty, CollectionElementType.Name);
		}
		#region IDisposable Members
		public void Dispose() {
			dummyAssociationInfo = null;
		}
		#endregion
	}
}
