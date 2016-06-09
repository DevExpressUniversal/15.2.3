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
using System.Windows.Forms;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.Widget;
using DevExpress.XtraTab;
namespace DevExpress.XtraBars.Design.Frames {
	[ToolboxItem(false)]
	public partial class TableGroupLayoutFrame : LayoutFrame {
		List<BaseRelativeLengthElementPage> elementsPages;
		public TableGroupLayoutFrame() {
			InitializeComponent();
			elementsPages = new List<BaseRelativeLengthElementPage>();
		}
		protected override string DescriptionText {
			get { return "Modify the view's layout and click the Apply button to apply the modifications to the current view. You can also save the layout to an XML file (this can be loaded and applied to other views at design time and runtime)."; }
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				PreviewView.DocumentActivated -= OnDocumentActivated;
			}
			base.Dispose(disposing);
		}
		public override void InitComponent() {
			base.InitComponent();
			InitTabControl();
		}
		void InitEditingView() {
			if(EditingView != null)
				((IDesignTimeSupport)EditingView).Load();
		}
		string[] DocumentProperties = new string[] { "Name", "RowIndex", "ColumnIndex", "RowSpan", "ColumnSpan" };
		void InitTabControl() {
			pnlTab.Visible = true;
			splitterControl1.Visible = true;
			if((PreviewView as WidgetView).LayoutMode == LayoutMode.TableLayout) {
				PreviewView.DocumentActivated += OnDocumentActivated;
				BaseRelativeLengthElementPage viewDocumentsPage = new RelativeLengthElementPage<BaseDocument, Document>(PreviewView.Documents, DocumentProperties);
				BaseRelativeLengthElementPage viewColumnsPage = new RelativeLengthElementPage<ColumnDefinition, ColumnDefinition>((PreviewView as WidgetView).Columns, null);
				BaseRelativeLengthElementPage viewRowsPage = new RelativeLengthElementPage<RowDefinition, RowDefinition>((PreviewView as WidgetView).Rows, null);
				CreatePage(viewDocumentsPage, "Documents");
				viewDocumentsPage.SetCollectionModify(false);
				CreatePage(viewRowsPage, "Rows");
				CreatePage(viewColumnsPage, "Columns");
			}
			if((PreviewView as WidgetView).LayoutMode == LayoutMode.StackLayout) {
				var view = PreviewView as WidgetView;
				BaseRelativeLengthElementPage viewStackGroupPage = new RelativeLengthElementPage<StackGroup, StackGroup>(view.StackGroups, null);
				view.StackGroups.CollectionChanged += StackGroups_CollectionChanged;
				CreatePage(viewStackGroupPage, "StackGroups");
			}
			if((PreviewView as WidgetView).LayoutMode == LayoutMode.FlowLayout) {
				pnlTab.Visible = false;
				splitterControl1.Visible = false;
			}
		}
		void StackGroups_CollectionChanged(Docking2010.Base.CollectionChangedEventArgs<StackGroup> ea) {
			if(ea.ChangedType == Docking2010.Base.CollectionChangedType.ElementAdded && (PreviewView as WidgetView).StackGroups.Count == 1) {
				foreach(var item in (PreviewView as WidgetView).Documents) {
					(PreviewView as WidgetView).StackGroups[0].Items.Add(item as Document);
				}
			}
		}
		void CreatePage(BaseRelativeLengthElementPage page, string text) {
			XtraTabPage tabPage = new XtraTabPage();
			tabPage.Text = page.Text = text;
			page.UpdateImages(imageList1);
			tabPage.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
			page.Dock = DockStyle.Fill;
			page.Parent = tabPage;
			elementsPages.Add(page);
			xtraTabControl1.TabPages.Add(tabPage);
		}
		void OnDocumentActivated(object sender, DocumentEventArgs e) {
			elementsPages.ForEach(
			 new Action<BaseRelativeLengthElementPage>(page =>
			 {
				 page.UpdateSelection(e.Document);
			 }));
		}
		protected override void SetLayoutChanged(bool enabled) {
			base.SetLayoutChanged(enabled);
			elementsPages.ForEach(
				new Action<BaseRelativeLengthElementPage>(page =>
				{
					page.UpdateTreeView();
				}));
		}
		public override void StoreLocalProperties(DevExpress.Utils.Design.PropertyStore localStore) {
			base.StoreLocalProperties(localStore);
			localStore.AddProperty("TabPropertiesLength", pnlTab.Width);
		}
		public override void RestoreLocalProperties(DevExpress.Utils.Design.PropertyStore localStore) {
			base.RestoreLocalProperties(localStore);
			pnlTab.Width = localStore.RestoreIntProperty("TabPropertiesLength", xtraTabControl1.Width);
		}
	}
}
