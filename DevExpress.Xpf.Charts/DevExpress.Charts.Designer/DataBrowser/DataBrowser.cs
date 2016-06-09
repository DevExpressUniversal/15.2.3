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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Data.Browsing.Design;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Charts.Designer.Native {
	public static class DataPickerBehaviour {
		public static readonly DependencyProperty CloseOnDoubleClickProperty = DependencyProperty.RegisterAttached("CloseOnDoubleClick",
			typeof(bool), typeof(DataPickerBehaviour), new PropertyMetadata(false, OnCloseOnDoubleClickChanged));
		public static bool GetCloseOnDoubleClick(DependencyObject d) {
			return (bool)d.GetValue(CloseOnDoubleClickProperty);
		}
		public static void SetCloseOnDoubleClick(DependencyObject obj, bool value) {
			obj.SetValue(CloseOnDoubleClickProperty, value);
		}
		static void OnCloseOnDoubleClickChanged(object sender, DependencyPropertyChangedEventArgs e) {
			Control control = (Control)sender;
			bool closeOnDoubleClick = (bool)(e.NewValue);
			if (closeOnDoubleClick)
				control.MouseDoubleClick += OnMouseDoubleClick;
			else
				control.MouseDoubleClick -= OnMouseDoubleClick;
		}
		static void OnMouseDoubleClick(object sender, MouseButtonEventArgs e) {
			GridControl grid = sender as GridControl;
			if (grid != null) {
				PopupBaseEdit popupEdit = (PopupBaseEdit)BaseEdit.GetOwnerEdit(grid);
				DataBrowserTreeNode selectedNode = grid.SelectedItem as DataBrowserTreeNode;
				if (popupEdit != null && selectedNode != null) {
					popupEdit.EditValue = selectedNode.IsEmpty ? null : new DataMemberInfo(selectedNode.GetFullDisplayName(), ((INode)selectedNode).DataMember);
					popupEdit.ClosePopup();
				}
			}
		}
	}
	public class DataBrowserChildNodeSelector : IChildNodesSelector {
		public IEnumerable SelectChildren(object item) {
			((DataBrowserTreeNode)item).InvalidateNodes();
			return ((INode)item).ChildNodes;
		}
	}
	public class DataMemberInfo {
		readonly string displayName;
		readonly string dataMember;
		public string DisplayName { get { return displayName; } }
		public string DataMember { get { return dataMember; } }
		public DataMemberInfo()
			: this(String.Empty, String.Empty) {
		}
		public DataMemberInfo(string displayName, string dataMember) {
			this.displayName = displayName;
			this.dataMember = dataMember;
		}
		public override string ToString() {
			return displayName;
		}
	}
	public class DataBrowserTreeNode : INode {
		readonly DataBrowserTreeNodeList childNodes;
		string id;
		ImageSource glyphSource;
		object dataSource;
		string dataMember;
		ScaleType[] allowedScaleTypes;
		DataBrowserTreeNode parent;
		public string ID { get { return id; } }
		public ImageSource GlyphSource { get { return glyphSource; } }
		public DataBrowserTreeNode(string id) : this(id, null, String.Empty, null, null) {
		}
		public DataBrowserTreeNode(string id, object dataSource, string dataMember, ScaleType[] allowedScaleTypes, DataBrowserTreeNode parent) {
			this.id = id;
			this.dataSource = dataSource;
			this.dataMember = dataMember;
			this.glyphSource = null;
			this.allowedScaleTypes = allowedScaleTypes;
			this.parent = parent;
			childNodes = new DataBrowserTreeNodeList(this);
		}
		void RemoveDummyNodes() {
			List<DataBrowserTreeNode> dummyNodeList = new List<DataBrowserTreeNode>();
			foreach (DataBrowserTreeNode node in childNodes)
				if (node.IsDummyNode || parent == null && node.dataMember.Contains("."))
					dummyNodeList.Add(node);
			foreach (DataBrowserTreeNode dummyNode in dummyNodeList)
				childNodes.Remove(dummyNode);
		}
		public void SetParent(DataBrowserTreeNode parent) {
			if (this.parent != parent)
				this.parent = parent;
		}
		void FillChildNodes() {
			if (childNodes.Count == 1 && childNodes[0].IsDummyNode) {
				childNodes.Clear();
				Collection<Pair<object, string>> dataSources = new Collection<Pair<object, string>>();
				dataSources.Add(new Pair<object, string>(dataSource, dataMember));
				List<DataBrowserTreeNode> dataSourceNodes = new List<DataBrowserTreeNode>();
				new DataBrowserPickManager(allowedScaleTypes).FillContent(dataSourceNodes, dataSources, false);
				if (dataSourceNodes.Count > 0)
					childNodes.AddRange((DataBrowserTreeNodeList)dataSourceNodes[0].ChildNodes);
			}
		}
		public void InvalidateNodes() {
			RemoveDummyNodes();
			foreach (DataBrowserTreeNode node in childNodes)
				node.FillChildNodes();
		}
		public string GetFullDisplayName() {
			if (parent != null) {
				string parentPath = parent.GetFullDisplayName();
				if (!String.IsNullOrEmpty(parentPath))
					return parentPath + "." + id;
				else
					return id;
			}
			else
				return String.Empty;
		}
		#region INode Members
		public IList ChildNodes {
			get {
				return childNodes;
			}
		}
		string INode.DataMember {
			get { return dataMember; }
		}
		void INode.Expand(EventHandler callback) {
			;
		}
		bool INode.HasDataSource(object dataSource) {
			return this.dataSource != null && this.dataSource.Equals(dataSource);
		}
		bool INode.IsComplex {
			get { throw new NotImplementedException(); }
		}
		bool INode.IsDataMemberNode {
			get { throw new NotImplementedException(); }
		}
		bool INode.IsDataSourceNode {
			get { throw new NotImplementedException(); }
		}
		public virtual bool IsDummyNode {
			get { return false; }
		}
		public virtual bool IsEmpty {
			get { return false; }
		}
		bool INode.IsList {
			get { throw new NotImplementedException(); }
		}
		object INode.Parent {
			get { throw new NotImplementedException(); }
		}
		#endregion
	}
	public class DummyDataBrowserTreeNode : DataBrowserTreeNode {
		public override bool IsDummyNode { get { return true; } }
		public DummyDataBrowserTreeNode() : base("Dummy") {
		}
	}
	public class NoneDataBrowserTreeNode : DataBrowserTreeNode {
		public override bool IsEmpty { get { return true; } }
		public NoneDataBrowserTreeNode() : base("None") {
		}
	}
	public class DataBrowserTreeNodeList : List<DataBrowserTreeNode> {
		DataBrowserTreeNode owner;
		public DataBrowserTreeNode Owner { get { return owner; } }
		public DataBrowserTreeNodeList(DataBrowserTreeNode owner) {
			this.owner = owner;
		}
		public new void AddRange(IEnumerable<DataBrowserTreeNode> collection) {
			foreach (DataBrowserTreeNode item in collection)
				item.SetParent(owner);
			base.AddRange(collection);
		}
	}
	public class DataBrowserPickManager : PickManager {
		ScaleType[] allowedScaleTypes;
		public DataBrowserPickManager(ScaleType[] allowedScaleTypes) {
			this.allowedScaleTypes = allowedScaleTypes;
		}
		bool IsAllowedDataMember(TypeSpecifics dataMemberType, ScaleType[] allowedScaleTypes) {
			bool dataMemberAllowed = false;
			foreach (ScaleType scaleType in allowedScaleTypes) {
				switch (scaleType) {
					case ScaleType.Qualitative:
						dataMemberAllowed = true;
						break;
					case ScaleType.Numerical:
						dataMemberAllowed = dataMemberType == TypeSpecifics.CalcFloat ||
							dataMemberType == TypeSpecifics.CalcInteger ||
							dataMemberType == TypeSpecifics.Float ||
							dataMemberType == TypeSpecifics.Integer ||
							dataMemberType == TypeSpecifics.Default;
						break;
					case ScaleType.DateTime:
						dataMemberAllowed = dataMemberType == TypeSpecifics.CalcDate ||
							dataMemberType == TypeSpecifics.Date ||
							dataMemberType == TypeSpecifics.Default;
						break;
				}
				if (dataMemberAllowed)
					return true;
			}
			return false;
		}
		protected override INode CreateDataMemberNode(object dataSource, string dataMember, string displayName, bool isList, object owner, IPropertyDescriptor property) {
			DataBrowserTreeNode parent = owner is DataBrowserTreeNodeList ? ((DataBrowserTreeNodeList)owner).Owner : null;
			if (property.Specifics == TypeSpecifics.List)
				return new DataBrowserTreeNode(displayName, dataSource, dataMember, allowedScaleTypes, parent);
			if (IsAllowedDataMember(property.Specifics, allowedScaleTypes))
				return new DataBrowserTreeNode(displayName, dataSource, dataMember, allowedScaleTypes, parent);
			else
				return new DummyDataBrowserTreeNode();
		}
		protected override INode CreateDataSourceNode(object dataSource, string dataMember, string name, object owner) {
			return new DataBrowserTreeNode(name);
		}
		protected override INode CreateDummyNode(object owner) {
			DataBrowserTreeNodeList childNodes = owner as DataBrowserTreeNodeList;
			return new DummyDataBrowserTreeNode();
		}
		protected override object CreateNoneNode(object owner) {
			return new NoneDataBrowserTreeNode();
		}
		protected override bool NodeIsEmpty(INode node) {
			throw new NotImplementedException();
		}
	}
}
