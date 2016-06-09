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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraWaitForm;
using DevExpress.ReportServer.ServiceModel.DataContracts;
using DevExpress.Utils.Extensions.Helpers;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList;
namespace DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Views {
	[ToolboxItem(false)]
	public partial class ChooseRemoteReportView : UserControl, IChooseRemoteReportView {
		string IPageView.HeaderText {
			get { return "Avaliable Reports"; }
		}
		string IPageView.DescriptionText {
			get { return "Select an existing report from one of the Report Server categories"; }
		}
		public CategoryDto SelectedCategory {
			get {
				return categoriesList.GetDataRecordByNode(categoriesList.FocusedNode) as CategoryDto;
			}
			set {
				var node = FindNode(categoriesList, value);
				if(node != null)
					categoriesList.FocusedNode = node;
			}
		}
		TreeListNode FindNode(TreeList treeList, object value) {
			TreeListNode node = null;
			foreach(TreeListNode treeListNode in treeList.Nodes) {
				if(node != null)
					continue;
				var data = treeList.GetDataRecordByNode(treeListNode);
				if(data == value)
					node = treeListNode;
			}
			return node;
		}
		public ReportCatalogItemDto SelectedReport {
			get {
				return reportsList.GetDataRecordByNode(reportsList.FocusedNode) as ReportCatalogItemDto; ;
			}
			set {
				var node = FindNode(reportsList, value);
				if(node != null)
					reportsList.FocusedNode = node; ;
			}
		}
		public event EventHandler SelectedCategoryChanged;
		public event EventHandler SelectedReportChanged;
		public ChooseRemoteReportView() {
			InitializeComponent();
			this.VisibleChanged += ChooseRemoteReportView_VisibleChanged;
		}
		void ChooseRemoteReportView_VisibleChanged(object sender, EventArgs e) {
			reportsList.Focus();
		}
		public void FillCategories(IEnumerable<ReportServer.ServiceModel.DataContracts.CategoryDto> categories) {
			categoriesList.DataSource = categories;
			categoriesList.Refresh();
		}
		public void FillReports(IEnumerable<ReportServer.ServiceModel.DataContracts.ReportCatalogItemDto> reports) {
			reportsList.DataSource = reports;
		}
		public void EnableWaitPanel(bool enable) {
			if(enable)
				SplashScreenManager.ShowForm(this, typeof(DemoWaitForm), true, true, SplashFormStartPosition.Default, Point.Empty);
			else SplashScreenManager.CloseForm(false, 0, this.FindForm());
		}
		void categoriesList_FocusedNodeChanged(object sender, XtraTreeList.FocusedNodeChangedEventArgs e) {
			SelectedCategoryChanged.SafeRaise(this);
		}
		void reportsList_FocusedNodeChanged(object sender, XtraTreeList.FocusedNodeChangedEventArgs e) {
			SelectedReportChanged.SafeRaise(this);
		}
	}
}
