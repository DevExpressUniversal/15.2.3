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
using DevExpress.Utils.Frames;
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.XtraBars.Design.Frames {
	[ToolboxItem(false)]
	public partial class MainFrame : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		protected override string DescriptionText { 
			get { return "You can convert the current view to another type, assign a new view to the DocumentManager main view."; } 
		}
		public MainFrame()
			: base(1) {
			InitializeComponent();
			pgMain.BringToFront();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(DocumentManagerInfo != null)
					DocumentManagerInfo.SelectionChanged -= OnInfo_SelectionChanged;
			}
			if(components != null) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override bool AllowGlobalStore { get { return false; } }
		BaseView EditingView {
			get { return EditingObject as BaseView; }
		}
		void btnAdd_Click(object sender, System.EventArgs e) {
		}
		void btnDelete_Click(object sender, System.EventArgs e) {
		}
		EditingDocumentManagerInfo documentManagerInfo;
		protected EditingDocumentManagerInfo DocumentManagerInfo {
			get { return documentManagerInfo; }
		}
		public override void InitComponent() {
			this.documentManagerInfo = InfoObject as EditingDocumentManagerInfo;
			DocumentManagerInfo.SelectionChanged += OnInfo_SelectionChanged;
			viewSelector.Manager = DocumentManagerInfo.EditingDocumentManager;
			viewSelector.SelectionChanged += new EventHandler(viewSelector_SelectionChanged);
			RefreshPropertyGrid();
		}
		void viewSelector_SelectionChanged(object sender, EventArgs e) {
			DocumentManagerInfo.SelectedObject = viewSelector.SelectedObject;
		}
		protected override object SelectedObject {
			get { return DocumentManagerInfo.SelectedObject; }
		}
		protected virtual void OnInfo_SelectionChanged(object sender, EventArgs e) {
			RaiseRefreshWizard("", "ChangedView");
			RefreshPropertyGrid();
		}
	}
}
