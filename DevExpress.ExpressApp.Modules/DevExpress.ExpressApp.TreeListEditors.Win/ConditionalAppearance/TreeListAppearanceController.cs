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
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base.General;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Actions;
using System.Collections.Generic;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Editors;
using DevExpress.XtraTreeList;
namespace DevExpress.ExpressApp.TreeListEditors.Win {
	public class TreeListAppearanceController : ListViewControllerBase, ISupportAppearanceCustomization {
		void Editor_ControlsCreated(object sender, EventArgs e) {
			OnTreeListChanged();
		}
		void TreeList_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e) {
			NodeCellStyleHandler(sender, e);
		}
		void Frame_ViewControllersActivated(object sender, EventArgs e) {
			OnEditorChanged();
		}
		protected virtual void OnTreeListChanged() {
			if(Active && View != null && View.Editor != null && View.Editor is TreeListEditor && ((TreeListEditor)View.Editor).TreeList != null) {
				((TreeListEditor)View.Editor).TreeList.NodeCellStyle -= new GetCustomNodeCellStyleEventHandler(TreeList_NodeCellStyle); 
				((TreeListEditor)View.Editor).TreeList.NodeCellStyle += new GetCustomNodeCellStyleEventHandler(TreeList_NodeCellStyle);
			}
		}
		protected virtual void OnEditorChanged() {
			if(Active && View != null && View.Editor != null && View.Editor is TreeListEditor) {
				((TreeListEditor)View.Editor).ControlsCreated -= new EventHandler(Editor_ControlsCreated);
				((TreeListEditor)View.Editor).ControlsCreated += new EventHandler(Editor_ControlsCreated);
				OnTreeListChanged();
			}
		}
		protected virtual void NodeCellStyleHandler(object sender, GetCustomNodeCellStyleEventArgs e) {
			if(Active && e.Node is ObjectTreeListNode) {
				NodeCellStyleEventArgsAppearanceObjectAdapter adapter = new NodeCellStyleEventArgsAppearanceObjectAdapter((TreeList)sender, e);
				OnCustomizeAppearance(new CustomizeAppearanceEventArgs(e.Column.FieldName, "ViewItem", adapter, ((ObjectTreeListNode)e.Node).Object, ViewInfo.FromView(View)));
			}
		}
		protected virtual void OnCustomizeAppearance(CustomizeAppearanceEventArgs args) {
			if(CustomizeAppearance != null) {
				CustomizeAppearance(this, args);
			}
		}
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			Frame.ViewControllersActivated += new EventHandler(Frame_ViewControllersActivated);
		}
		protected override void SubscribeToListEditorEvent() {
			base.SubscribeToListEditorEvent();
			OnEditorChanged();
		}
		protected override void UnsubscribeToListEditorEvent() {
			base.UnsubscribeToListEditorEvent();
			if(View.Editor != null) {
				View.Editor.ControlsCreated -= new EventHandler(Editor_ControlsCreated);
				if(View.Editor is TreeListEditor && ((TreeListEditor)View.Editor).TreeList != null) {
					((TreeListEditor)View.Editor).TreeList.NodeCellStyle -= new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(TreeList_NodeCellStyle);
				}
			}
		}
		#region ISupportAppearanceCustomization Members
		public event EventHandler<CustomizeAppearanceEventArgs> CustomizeAppearance;
		#endregion
	}
	public class NodeCellStyleEventArgsAppearanceObjectAdapter : AppearanceObjectAdapter {
		public NodeCellStyleEventArgsAppearanceObjectAdapter(TreeList treeList, GetCustomNodeCellStyleEventArgs args)
			: base(args.Appearance) {
			this.Args = args;
			this.TreeList = treeList;
		}
		public TreeList TreeList { get; private set; }
		public GetCustomNodeCellStyleEventArgs Args { get; private set; }
	}
}
