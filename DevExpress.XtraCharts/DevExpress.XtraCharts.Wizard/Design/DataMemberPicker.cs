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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Native;
using DevExpress.XtraTreeList;
namespace DevExpress.XtraCharts.Design {
	[DXToolboxItem(false)]
	public class DataMemberPicker : DevExpress.XtraEditors.XtraUserControl {
		class ChartPickManager : TreeListPickManager {
			bool filterOnlyLists = false;
			ScaleType[] filterScaleTypes = new ScaleType[] {};
			public bool FilterOnlyLists { get { return filterOnlyLists; } }
			protected override string NoneNodeText { get { return ChartLocalizer.GetString(ChartStringId.WizDataMemberNoneString); } }
			public void SetFilterCriteria(ScaleType[] filterScaleTypes, bool filterOnlyLists) {
				this.filterScaleTypes = filterScaleTypes; 
				this.filterOnlyLists = filterOnlyLists;
			}
			protected override IPropertiesProvider CreateProvider() {
				IDataContextService service = GetDataContextService();
				return service == null ? new CustomPropertiesProvider(filterOnlyLists, filterScaleTypes) : 
					new CustomPropertiesProvider(service.CreateDataContext(new DataContextOptions(true, true), false), service, GetTypeSpecificsService(), filterOnlyLists, filterScaleTypes);
			}
			public override void FindDataMemberNode(IList nodes, string dataMember, Action<INode> callback) {
				FindDataMemberNodeCore(nodes, dataMember, 0, callback, true);
			} 
		}
		class CustomPropertiesProvider : DataSortedPropertiesNativeProvider {
			bool filterOnlyLists;
			ScaleType[] filterScaleTypes;
			public CustomPropertiesProvider(bool filterOnlyLists, ScaleType[] filterScaleTypes) {
				Init(filterOnlyLists, filterScaleTypes);
			}
			public CustomPropertiesProvider(DataContext dataContext, IDataContextService serv, TypeSpecificsService typeSpecificsService, bool filterOnlyLists, ScaleType[] filterScaleTypes)
				: base(dataContext, serv, typeSpecificsService) {
				Init(filterOnlyLists, filterScaleTypes);
			}
			private void Init(bool filterOnlyLists, ScaleType[] filterScaleTypes) {
				this.filterOnlyLists = filterOnlyLists;
				this.filterScaleTypes = filterScaleTypes;
			}
			protected override bool CanProcessProperty(IPropertyDescriptor property) {
				Type propertyType = GetPropertyType(property);
				if (filterOnlyLists) {
					if (ListTypeHelper.IsListType(propertyType) && !propertyType.IsArray)
						return base.CanProcessProperty(property);
				}
				else {
					if (filterScaleTypes.Length == 0)
						return base.CanProcessProperty(property);
					foreach (ScaleType filterScaleType in filterScaleTypes)
						if (DevExpress.XtraCharts.Native.BindingHelper.CheckDataType(filterScaleType, propertyType))
							return base.CanProcessProperty(property);
					if ((propertyType.IsClass && propertyType != typeof(string) && propertyType != typeof(DateTime)) || propertyType.IsInterface)
						return base.CanProcessProperty(property);
				}
				return false;
			}
		}
		protected class ChartDataSourceTreeView : DesignTreeListBindingPicker {
			#region inner classes
			class ExpandingKey {
				object dataSource;
				string text;
				public ExpandingKey(object dataSource, string text) {
					this.dataSource = dataSource;
					this.text = text;
				}
				public override bool Equals(object obj) {
					ExpandingKey key = obj as ExpandingKey;
					return key != null &&
						this.dataSource == key.dataSource &&
						this.text == key.text;
				}
				public override int GetHashCode() {
					return dataSource.GetHashCode() ^ text.GetHashCode();
				}
			}
			class ExpandingHash {
				Hashtable hash = new Hashtable();
				public bool AddIfNeeded(ExpandingKey key) {
					if(!hash.ContainsKey(key)) {
						hash.Add(key, null);
						return true;
					}
					else
						return false;
				}
				public void Clear() {
					hash.Clear();
				}
			}
			#endregion
			ExpandingHash expandingHash = new ExpandingHash();
			int locking = 0;
			public ChartDataSourceTreeView() : base(new ChartPickManager()) {
			}
			protected override string NoneNodeText { get { return ChartLocalizer.GetString(ChartStringId.WizDataMemberNoneString); } }
			public void Lock() {
				if (locking++ == 0)
					BeginUpdate();
			}
			public void Unlock() {
				if (locking > 0 && --locking == 0)
					EndUpdate();
			}
			public void ClearExpandingHash() {
				expandingHash.Clear();
			}
			protected override void OnNodeExpand(object sender, BeforeExpandEventArgs e) {
				DataMemberListNode dataMemberNode = e.Node as DataMemberListNode;
				if (locking > 0 && dataMemberNode != null && !dataMemberNode.Expanded) {
					bool nodeInHash = !expandingHash.AddIfNeeded(new ExpandingKey(dataMemberNode.DataSource, dataMemberNode.Text));
					if (nodeInHash || dataMemberNode.Level > 2) {
						e.CanExpand = false;
						return;
					}
				}
				base.OnNodeExpand(sender, e);
			}
		}
		ChartDataSourceTreeView treeView;
		IContainer components;
		protected ChartDataSourceTreeView TreeView { get { return treeView; } }
		public string DataMember { 
			get { 
				DataMemberListNode node = treeView.SelectedNode as DataMemberListNode;
				return node == null ? String.Empty : node.DataMember;
			}
		}
		public bool IsNoneNode {
			get {
				DataMemberListNodeBase node = treeView.SelectedNode as DataMemberListNodeBase;
				return node != null && node.IsEmpty;
			}
		}
		public bool IsDataMemberNode {
			get {
				DataMemberListNode node = treeView.SelectedNode as DataMemberListNode;
				return node != null && !node.IsList;
			}
		}
		public void SetServiceProvider(IServiceProvider serviceProvider) {
			treeView.PickManager.ServiceProvider = serviceProvider;
		}
		public event EventHandler TreeViewDoubleClick;
		public event EventHandler SelectionChanged;
		public DataMemberPicker() {
			InitializeComponent();
		}
		protected override void Dispose(bool disposing) {
			if (disposing && components != null)
				components.Dispose();
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataMemberPicker));
			this.treeView = new DevExpress.XtraCharts.Design.DataMemberPicker.ChartDataSourceTreeView();
			((System.ComponentModel.ISupportInitialize)(this.treeView)).BeginInit();
			this.SuspendLayout();
			this.treeView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.treeView, "treeView");
			this.treeView.DraggedNode = null;
			this.treeView.Name = "treeView";
			this.treeView.ShowParametersNode = true;
			this.treeView.AfterFocusNode += new DevExpress.XtraTreeList.NodeEventHandler(this.treeView_AfterFocusNode);
			this.treeView.SelectionChanged += new System.EventHandler(this.treeView_SelectionChanged);
			this.treeView.DoubleClick += new System.EventHandler(this.treeView_DoubleClick);
			this.treeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView_KeyDown);
			this.Controls.Add(this.treeView);
			this.Name = "DataMemberPicker";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.treeView)).EndInit();
			this.ResumeLayout(false);
		}
		void treeView_AfterFocusNode(object sender, NodeEventArgs e) {
			if (SelectionChanged != null)
				SelectionChanged(this, EventArgs.Empty);
		}
		#endregion
		public void SetFilterCriteria(ScaleType[] filterScaleTypes, bool filterOnlyLists) {
			((ChartPickManager)treeView.PickManager).SetFilterCriteria(filterScaleTypes, filterOnlyLists); 
		}
		public void FillDataSource(object dataSource, string chartDataMember) {
			Collection<Pair<object, string>> dataSources = new Collection<Pair<object, string>>();
			dataSources.Add(new Pair<object, string>(dataSource, chartDataMember));
			treeView.PickManager.FillContent(treeView.Nodes, dataSources, true);
		}
		public void Clear() {
			treeView.Nodes.Clear();
		}
		public void Start() {
			treeView.Start();
		}
		public void Stop() {
			treeView.Stop();
		}
		public void SelectDataMember(object dataSource, string dataMember) {
			treeView.SelectDataMemberNode(dataSource, dataMember);
		}
		public void SelectNoneDataMember() {
			treeView.SelectNoneNode();
		}
		void ClosePickerIfNeed() {
			if (TreeViewDoubleClick != null) {
				DataMemberListNodeBase node = treeView.DataMemberNode;
				if (node != null) {
					DataMemberListNode dataMember = node as DataMemberListNode;
					if (dataMember == null || ((ChartPickManager)treeView.PickManager).FilterOnlyLists || !dataMember.IsList)
						TreeViewDoubleClick(this, EventArgs.Empty);
				}
			}
		}
		private void treeView_DoubleClick(object sender, System.EventArgs e) {
			ClosePickerIfNeed();
		}
		private void treeView_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter)
				ClosePickerIfNeed();
		}
		private void treeView_SelectionChanged(object sender, System.EventArgs e) {
			if (SelectionChanged != null)		
				SelectionChanged(this, EventArgs.Empty);
		}
	}
}
