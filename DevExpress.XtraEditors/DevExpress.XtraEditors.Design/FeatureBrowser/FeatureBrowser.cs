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
using System.Reflection;
using System.Xml;
namespace DevExpress.XtraEditors.FeatureBrowser {
	public class FeatureBrowserItemPage {
		string name;
		string selectedPropertyOnStart;
		string[] expandedPropertiesOnStart;
		string description;
		bool expandAll;
		string[] properties;
		public FeatureBrowserItemPage(string name) {
			this.name = name;
			this.selectedPropertyOnStart = string.Empty;
			this.expandedPropertiesOnStart = new string[] {};
			this.description = string.Empty;
			this.expandAll = false;
			this.properties = new string[] {};
		}
		public string Name { get { return name; } }
		public string SelectedPropertyOnStart { get { return selectedPropertyOnStart; } set { selectedPropertyOnStart = value;} }
		public string[] ExpandedPropertiesOnStart { get { return expandedPropertiesOnStart; } set { expandedPropertiesOnStart = value; } }
		public string Description { get { return description; } set { description = value; } }
		public bool ExpandAll { get {return expandAll; } set { expandAll = value; } }
		public string[] Properties { get { return properties; } set { properties = value; } }
	}
	public class FeatureBrowserItemPageCollection : CollectionBase {
		public FeatureBrowserItemPageCollection() {}
		public FeatureBrowserItemPage this[int index] { get { return InnerList[index] as FeatureBrowserItemPage; } }
		public FeatureBrowserItemPage this[string name] { 
			get { 
				name = name.ToUpper();
				for(int i = 0; i < Count; i ++) {
					if(this[i].Name.ToUpper() == name)
						return this[i];
				}
				return null;
			} 
		}
		public FeatureBrowserItemPage Add() {
			return Add(string.Empty);
		}
		public FeatureBrowserItemPage Add(string name) {
			FeatureBrowserItemPage page = new FeatureBrowserItemPage(name);
			InnerList.Add(page);
			return page;
		}
	}
	public class FeatureBrowserItem : CollectionBase, IComparer {
		string id;
		string name;
		FeatureBrowserItem parent;
		Type featureSelectorFormType;
		string sourceProperty;
		string referenceId;
		FeatureBrowserItem referenceItem;
		FeatureBrowserItemPageCollection pages;
		Type sampleControlCustomization;
		string sampleControlCustomizationName;
		bool sorted;
		public FeatureBrowserItem() : this(string.Empty) {}
		public FeatureBrowserItem(string id) : this(id, null) {}
		public FeatureBrowserItem(string id, Type featureSelectorFormType) : this(id, "", featureSelectorFormType) {}
		public FeatureBrowserItem(string id, string name, Type featureSelectorFormType) {
			this.id = id;
			this.name = name;
			this.sourceProperty = string.Empty;
			this.pages = new FeatureBrowserItemPageCollection();
			this.sorted = false;
			this.referenceItem = null;
			this.referenceId = string.Empty;
			if(featureSelectorFormType != null) { 
				if(!featureSelectorFormType.IsSubclassOf(typeof(FeatureBrowserFormBase))) {
					throw new ArgumentException("feature frame class has to be inherited from FeatureBrowserFormBase");																									  
				}
				ConstructorInfo constructorInfoObj = featureSelectorFormType.GetConstructor(Type.EmptyTypes);
				if (constructorInfoObj == null) 
					throw new ApplicationException(featureSelectorFormType.FullName + " doesn't have public constructor with empty parameters");					
			}
			this.featureSelectorFormType = featureSelectorFormType;
			this.parent = null;
		}
		public FeatureBrowserItemPageCollection Pages { get { return pages; } }
		public FeatureBrowserItem this[int index] { get { return List[index] as FeatureBrowserItem; } }
		public string Id { get { return id; } }
		public string Name { get { return name != "" ? name : Id; } }
		public FeatureBrowserItem Parent { get { return parent; } }
		public Type FeatureSelectorFormType { get { return featureSelectorFormType; } }
		public bool Sorted { get { return sorted; } set { sorted = value; } }
		public Type SampleControlCustomization { 
			get { 
				if(sampleControlCustomization == null && sampleControlCustomizationName != string.Empty) {
					sampleControlCustomization = GetTypeByName(sampleControlCustomizationName);
				}
				return sampleControlCustomization; } 
		}
		public string SampleControlCustomizationName {
			get { return sampleControlCustomizationName; }
			set {
				if(sampleControlCustomizationName == value) return;
				sampleControlCustomizationName = value;
				this.sampleControlCustomization = null;
			}
		}
		public FeatureBrowserItem Add(string id) {
			return Add(id, null);
		}
		public FeatureBrowserItem Add(string id, Type featureSelectorFormType) {
			return Add(id, "", featureSelectorFormType);
		}
		public FeatureBrowserItem Add(string id, string name, Type featureSelectorFormType) {
			FeatureBrowserItem item = CreateItem(id, name, featureSelectorFormType);
			List.Add(item);
			return item;
		}
		public FeatureBrowserItem Insert(int index, string id) {
			return Insert(index, id, null);
		}
		public FeatureBrowserItem Insert(int index, string id, Type featureSelectorFormType) {
			return Insert(index, id, "", featureSelectorFormType);
		}
		public FeatureBrowserItem Insert(int index, string id, string name, Type featureSelectorFormType) {
			FeatureBrowserItem item = CreateItem(id, name, featureSelectorFormType);
			if(index < 0 || index >= Count)
				List.Add(item);
			else List.Insert(index, item);
			return item;
		}
		public string SourceProperty { get { return sourceProperty; } set { sourceProperty = value; } }
		public string ReferenceId { get { return referenceId; } set { referenceId = value; } }
		public FeatureBrowserItem ReferenceItem { get { return referenceItem; }  set { referenceItem = value; } }
		public void Sort() {
			if(Sorted)
				InnerList.Sort(this);
			for(int i = 0; i < Count; i ++)
				this[i].Sort();
		}
		public FeatureBrowserItem FindItemById(string featureId) {
			if(featureId == string.Empty) return null;
			for(int i = 0; i < Count; i ++) {
				if(this[i].Id == featureId)
					return this[i];
				FeatureBrowserItem item = this[i].FindItemById(featureId);
				if(item != null)
					return item;
			}
			return null;
		}
		int IComparer.Compare(object x, object y) {
			FeatureBrowserItem item1 = x as FeatureBrowserItem;
			FeatureBrowserItem item2 = y as FeatureBrowserItem;
			return string.Compare(item1.Name, item2.Name);
		}
		protected FeatureBrowserItem CreateItem(string id, string name, Type featureSelectorFormType) {
			FeatureBrowserItem item = new FeatureBrowserItem(id, name, featureSelectorFormType);
			item.parent = this;
			return item;
		}
		Type GetTypeByName(string typeName) {
			if(typeName == "") return null;
			Type type = Type.GetType(typeName);
			if(type == null && featureSelectorFormType != null) {
				type = featureSelectorFormType.Assembly.GetType(typeName);
			}
			return type;
		}
	}
	public class FeatureBrowserItemsXmlCreator : XmlFeaturesReaderBase {
		string[] xmlResourceFullNames;
		Type featureBrowserFormBase;
		Type sourceType;
		FeatureBrowserItem root;
		public FeatureBrowserItemsXmlCreator(string[] xmlResourceFullNames, Type featureBrowserFormBase, Type sourceType) : base(sourceType) {
			this.xmlResourceFullNames = xmlResourceFullNames;
			this.featureBrowserFormBase = featureBrowserFormBase;
			this.sourceType = sourceType;
			this.root = new FeatureBrowserItem();
		}
		public FeatureBrowserItem Root { get { return root; } }
		public string[] XmlResourceFullNames { get { return xmlResourceFullNames; } }
		public Type FeatureBrowserFormBase { get { return featureBrowserFormBase; } }
		protected override string ItemTagName { get { return "FeatureItem"; } }
		public void LoadFromXml() {
			Root.Clear();
			if(FeatureBrowserFormBase != null) {
				LoadFromXmlFiles(XmlResourceFullNames);
			}
			UpdateReferences(Root, Root);
			Root.Sort();
		}
		protected override void AddXmlNodeCore(XmlNode node) {
			if(IsItemSupported(node)) 
				CreateItem(root, node);
		}
		void UpdateReferences(FeatureBrowserItem root, FeatureBrowserItem item) {
			if(item.ReferenceId != string.Empty)  {
				item.ReferenceItem = root.FindItemById(item.ReferenceId);
			}
			for(int i = 0; i < item.Count; i ++)
				UpdateReferences(root, item[i]);
		}
		void CreateItem(FeatureBrowserItem root, XmlNode node) {
			string id = GetNodeAttributeValue(node, "ID").Trim();
			if(id == string.Empty) return;
			string name = GetNodeAttributeValue(node, "Name").Trim();
			string category = GetNodeAttributeValue(node, "Category").Trim();
			FeatureBrowserItem item = null;
			if(category == "") 
				item = AddCategory(root, id, name, FeatureBrowserFormBase);
			else {
				FeatureBrowserItem categoryItem = GetCategoryItem(root, category);
				if(categoryItem != null)  {
					string indexValue = GetNodeAttributeValue(node, "Index");
					int index = indexValue != "" ? int.Parse(indexValue) : -1;
					item = categoryItem.Insert(index, id, name, FeatureBrowserFormBase);
				}
			}
			item.Sorted = GetNodeAttributeValue(node, "Sorted") == "True";
			item.SampleControlCustomizationName = GetNodeAttributeValue(node, "SampleSetupClass");
			item.SourceProperty = GetNodeAttributeValue(node, "SourceProperty");
			item.ReferenceId = GetNodeAttributeValue(node, "ReferenceId");
			CreatePages(item, node);
		}
		bool IsItemSupported(XmlNode node) {
			if(SourceType == null) return true;
			foreach(XmlNode childNode in node.ChildNodes) {
				if(childNode.Name == "SupportedComponents") {
					return GetNodeAttributeValue(childNode, SourceType.Name) == "True";
				}
			}
			return true;
		}
		FeatureBrowserItem GetCategoryItem(FeatureBrowserItem root, string category) {
			FeatureBrowserItem item = FindCategoryItemById(root, category);
			if(item != null)
				return item;
			else return root.Add(category);
		}
		FeatureBrowserItem AddCategory(FeatureBrowserItem root, string id, string name, Type featureBrowserFormBase) {
			FeatureBrowserItem item = FindCategoryItemById(root, id);
			if(item == null)
				item = root.Add(id, name, featureBrowserFormBase);
			return item;
		}
		FeatureBrowserItem FindCategoryItemById(FeatureBrowserItem root, string category) {
			for(int i = 0; i < root.Count; i ++) {
				FeatureBrowserItem item = root[i];
				if(item.Id == category)
					return item;
				item = FindCategoryItemById(item, category);
				if(item != null)
					return item;
			}
			return null;
		}
		protected void CreatePages(FeatureBrowserItem item, XmlNode node) {
			CreatePage(item.Pages.Add(), node);
			for(int i = 0; i < node.ChildNodes.Count; i ++)
				if(node.ChildNodes[i].Name == "Pages") {
					CreatePages(item, node.ChildNodes[i].ChildNodes);
				}
		}
		protected void CreatePages(FeatureBrowserItem item, XmlNodeList nodes) {
			for(int i = 0; i < nodes.Count; i ++)
				CreatePage(item.Pages.Add(nodes[i].Name), nodes[i]);
		}
		protected void CreatePage(FeatureBrowserItemPage page, XmlNode node) {
			page.Description = GetNodeDescription(node);
			page.ExpandAll = GetNodeAttributeValue(node, "ExpandAll") == "True";
			page.SelectedPropertyOnStart = GetNodeAttributeValue(node, "SelectedPropertyOnStart");
			page.Properties = GetArrayValues(node, "Properties", "Name");
			page.ExpandedPropertiesOnStart = GetArrayValues(node, "ExpandedPropertiesOnStart", "Name");
		}
	}
}
