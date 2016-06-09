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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.ExpressApp.ReportsV2.Win.Editors {
	public class PropertyNameEditor : UITypeEditor {
		DataMemberListNode selectedNode = null;
		XRDesignFieldList treeView = null;
		IWindowsFormsEditorService edSvc = null;
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object obj) {
			DevExpress.ExpressApp.ReportsV2.Win.Editors.SortingCollectionEditor.Sorting ci = context.Instance as DevExpress.ExpressApp.ReportsV2.Win.Editors.SortingCollectionEditor.Sorting;
			if(context != null && ci != null && provider != null) {
				edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if(ci.PropertyDescriptorProvider != null && edSvc != null) {
					ITypedList dataSource = ci.PropertyDescriptorProvider.SortingProperties;
					treeView = new XRDesignFieldList();
					treeView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(treeView_MouseDoubleClick);
					treeView.PickManager.FillNodes(dataSource, String.Empty, treeView.Nodes);
					treeView.Start();
					edSvc.DropDownControl(treeView);
					edSvc.CloseDropDown();
					treeView.MouseDoubleClick -= new System.Windows.Forms.MouseEventHandler(treeView_MouseDoubleClick);
					if(selectedNode != null) {
						string s = selectedNode.DataMember;
						if((string)obj != s)
							Closed();
						return s;
					}
				}
			}
			Closed();
			return obj;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void SetSelectedNode(TreeListNode node) {
			if(!IsRootNode(node)) {
				selectedNode = node as DataMemberListNode;
				edSvc.CloseDropDown();
			}
		}
		private void Closed() {
			selectedNode = null;
			treeView = null;
			edSvc = null;
		}
		private void treeView_MouseDoubleClick(object sender, MouseEventArgs e) {
			TreeListHitInfo hitInfo = treeView.CalcHitInfo(new Point(e.X, e.Y));
			if(hitInfo.Node != null && hitInfo.Column != null) {
				SetSelectedNode(hitInfo.Node);
			}
		}
		private bool IsRootNode(TreeListNode node) {
			return (treeView.Nodes.Count > 0 && node == treeView.Nodes[0]);
		}
	}
}
