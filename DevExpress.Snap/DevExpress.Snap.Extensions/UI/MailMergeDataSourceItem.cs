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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections;
using System.Collections.ObjectModel;
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Commands;
using DevExpress.Snap.Core.Native.Services;
using DevExpress.Snap.Core.Options;
using DevExpress.Snap.Extensions.Native;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.Snap.Extensions.UI {
	public class MailMergeDataSourceItem : ControlCommandBarButtonItem<RichEditControl, RichEditCommandId> {
		SNPopupControlContainer container;
		SnapFieldListTreeView picker;
		public MailMergeDataSourceItem() { 
		}
		public MailMergeDataSourceItem(BarManager manager)
			: base(manager) { 
		}
		public MailMergeDataSourceItem(string caption)
			: base(caption) { 
		}
		public MailMergeDataSourceItem(BarManager manager, string caption)
			: base(manager, caption) { 
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarButtonStyle ButtonStyle { get { return BarButtonStyle.DropDown; } set { } }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override PopupControl DropDownControl { get { return container; } set { } }
		protected override RichEditCommandId CommandId { get { return SnapCommandId.MailMergeDataSource; } }
		protected override void Initialize() {
			base.Initialize();
			InitializePopupControl();
		}
		protected virtual void InitializePopupControl() {
			this.container = new SNPopupControlContainer() {
				CloseOnOuterMouseClick = false,
				AutoSize = true,
				AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink,
				MinimumSize = new Size(10, 10)
			};
			this.picker = new DataSourceTreeView() { SnapControl = Control as SnapControl };
			this.container.Controls.Add(this.picker);
			container.Popup += container_Popup;
			container.CloseUp += container_CloseUp;
		}
		void container_CloseUp(object sender, EventArgs e) {
			picker.SelectionChanged -= picker_SelectionChanged;
		}
		protected override void OnControlChanged() {
			base.OnControlChanged();
			this.picker.SnapControl = Control as SnapControl;
		}
		void picker_SelectionChanged(object sender, EventArgs e) {
			var selection = this.picker.SelectedNode as DevExpress.XtraReports.Native.DataMemberListNodeBase;
			SnapMailMergeVisualOptions mailMergeOptions = ((ISnapControlOptions)Control.Options).SnapMailMergeVisualOptions;
			if(selection.DataSource == null) {
				mailMergeOptions.DataSourceName = null;
				mailMergeOptions.DataMember = null;
			}
			else {
				if(!SnapFieldListTreeView.CanBeMailMergeDataSource(selection))
					return;
				mailMergeOptions.DataSource = selection.DataSource;
				mailMergeOptions.DataMember = selection.DataMember;
			}
			this.container.HidePopup();
			((IRichEditControl)Control).InnerControl.OnUpdateUI();
		}
		void container_Popup(object sender, EventArgs e) {
			picker.SelectionChanged -= picker_SelectionChanged;
			SnapControl snapControl = Control as SnapControl;
			if(snapControl != null) {
				object[] dataSources = snapControl.DocumentModel.DataSourceDispatcher.GetDataSources().ToArray();
				picker.UpdateDataSource(snapControl, dataSources);
			}
			else
				picker.ClearNodes();
			picker.Selection.Clear();
			PlaceSelection();
			picker.SelectionChanged += picker_SelectionChanged;
		}
		void PlaceSelection() {
			foreach(TreeListNode node in this.picker.Nodes)
				if(PlaceSelection(node))
					return;
			this.picker.Nodes.LastNode.Selected = true;
		}
		bool PlaceSelection(TreeListNode node) {
			DevExpress.XtraReports.Native.DataMemberListNodeBase dataMemberNode = node as DevExpress.XtraReports.Native.DataMemberListNodeBase;
			if(picker.IsMailMergeDataSource(dataMemberNode)) {
				node.Selected = true;
				return true;
			}
			foreach(TreeListNode child in node.Nodes)
				if(PlaceSelection(child))
					return true;
			return false;
		}
		class DataSourceTreeView : SnapFieldListTreeView {
			public DataSourceTreeView()
				: this(new DataSourcePickManager(new DataContextOptions(false, false))) {
			}
			protected DataSourceTreeView(TreeListPickManager pickManager)
				: base(pickManager) {
			}
			protected override XtraReports.Design.MenuItemDescriptionCollection CreateMenuItems(DataMemberListNodeBase node) {
				return new NoNodeDataSourceContextMenuHelper(node).CreateMenuItems();
			}
		}
		class DataSourcePickManager : SnapTreeListPickManager {
			public DataSourcePickManager(DataContextOptions options)
				: base(options) {
			}
			protected override IPropertiesProvider CreateProvider() {
				IDataContextService service = GetDataContextService();
				return service != null ? new DataSourcesPropertiesProvider(service.CreateDataContext(options), service, GetTypeSpecificsService()) :
					base.CreateProvider();
			}
			public override void FillContent(IList nodes, Collection<Pair<object, string>> dataSources, bool addNoneNode) {
				base.FillContent(nodes, dataSources, true);
			}
			protected override bool ShouldAddDummyNode(IPropertyDescriptor property) {
				return false;
			}
		}
		class DataSourcesPropertiesProvider : SnapPropertiesProvider {
			public DataSourcesPropertiesProvider(DataContext dataContext, IDataContextService serv, TypeSpecificsService typeSpecificsService)
				: base(dataContext, serv, typeSpecificsService) {
			}
			protected override PropertyDescriptor[] FilterProperties(ICollection properties, object dataSource, string dataMember) {
				if (!string.IsNullOrEmpty(dataMember))
					return new PropertyDescriptor[0];
				if (IsDataSet(dataSource) || dataSource is DevExpress.DataAccess.IDataComponent)
					return base.FilterProperties(properties, dataSource, dataMember);
				return new PropertyDescriptor[0];
			}
			bool IsDataSet(object dataSource) {
				if (dataSource is DataSet)
					return true;
				BindingSource bs = dataSource as BindingSource;
				if (Object.ReferenceEquals(bs, null))
					return false;
				return bs.DataSource is DataSet && string.IsNullOrEmpty(bs.DataMember);
			}
		}
	}
}
