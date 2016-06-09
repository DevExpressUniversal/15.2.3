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

using System.ComponentModel;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.ASPxTreeList;
	using DevExpress.Web.ASPxTreeList.Export;
	public class MVCxTreeListSettings : DevExpress.Web.ASPxTreeList.TreeListSettings {
		public MVCxTreeListSettings()
			: this(null) {
		}
		protected internal MVCxTreeListSettings(MVCxTreeList treeList)
			: base(treeList) {
		}
	}
	public class MVCxTreeListSettingsBehavior : TreeListSettingsBehavior {
		public MVCxTreeListSettingsBehavior()
			: this(null) {
		}
		protected internal MVCxTreeListSettingsBehavior(MVCxTreeList treeList)
			: base(treeList) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool ProcessFocusedNodeChangedOnServer { get { return base.ProcessFocusedNodeChangedOnServer; } set { base.ProcessFocusedNodeChangedOnServer = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool ProcessSelectionChangedOnServer { get { return base.ProcessSelectionChangedOnServer; } set { base.ProcessSelectionChangedOnServer = value; } }
	}
	public class MVCxTreeListSettingsCookies : TreeListSettingsCookies {
		public MVCxTreeListSettingsCookies()
			: this(null) {
		}
		protected internal MVCxTreeListSettingsCookies(MVCxTreeList treeList)
			: base(treeList) {
		}
	}
	public class MVCxTreeListSettingsCustomizationWindow : TreeListSettingsCustomizationWindow {
		public MVCxTreeListSettingsCustomizationWindow()
			: this(null) {
		}
		protected internal MVCxTreeListSettingsCustomizationWindow(MVCxTreeList treeList)
			: base(treeList) {
		}
	}
	public class MVCxTreeListSettingsEditing : TreeListSettingsEditing {
		public MVCxTreeListSettingsEditing()
			: this(null) {
		}
		protected internal MVCxTreeListSettingsEditing(MVCxTreeList treeList)
			: base(treeList) {
			ShowModelErrorsForEditors = true;
		}
		public object AddNewNodeRouteValues { get; set; }
		public object UpdateNodeRouteValues { get; set; }
		public object DeleteNodeRouteValues { get; set; }
		public object NodeDragDropRouteValues { get; set; }
		public bool ShowModelErrorsForEditors { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool AllowNodeDragDrop { get { return base.AllowNodeDragDrop; } set { base.AllowNodeDragDrop = value; } }
		protected internal new bool IsPopupEditForm { get { return base.IsPopupEditForm; } }
		public override void Assign(TreeListSettingsBase source) {
			base.Assign(source);
			MVCxTreeListSettingsEditing src = source as MVCxTreeListSettingsEditing;
			if(src != null) {
				AddNewNodeRouteValues = src.AddNewNodeRouteValues;
				UpdateNodeRouteValues = src.UpdateNodeRouteValues;
				DeleteNodeRouteValues = src.DeleteNodeRouteValues;
				NodeDragDropRouteValues = src.NodeDragDropRouteValues;
				ShowModelErrorsForEditors = src.ShowModelErrorsForEditors;
			}
		}
	}
	public class MVCxTreeListSettingsLoadingPanel : TreeListSettingsLoadingPanel {
		public MVCxTreeListSettingsLoadingPanel()
			: base(null) {
		}
	}
	public class MVCxTreeListSettingsPager : TreeListSettingsPager {
		public MVCxTreeListSettingsPager()
			: base(null) {
		}
	}
	public class MVCxTreeListSettingsPopupEditForm : TreeListSettingsPopupEditForm {
		public MVCxTreeListSettingsPopupEditForm()
			: this(null) {
		}
		protected internal MVCxTreeListSettingsPopupEditForm(MVCxTreeList treeList)
			: base(treeList) {
		}
	}
	public class MVCxTreeListSettingsDataSecurity : TreeListSettingsDataSecurity {
		public MVCxTreeListSettingsDataSecurity()
			: this(null) {
		}
		protected internal MVCxTreeListSettingsDataSecurity(MVCxTreeList treeList)
			: base(treeList) {
		}
	}
	public class MVCxTreeListSettingsSelection : TreeListSettingsSelection {
		public MVCxTreeListSettingsSelection()
			: this(null) {
		}
		protected internal MVCxTreeListSettingsSelection(MVCxTreeList treeList)
			: base(treeList) {
		}
	}
	public class MVCxTreeListSettingsText : TreeListSettingsText {
		public MVCxTreeListSettingsText()
			: this(null) {
		}
		protected internal MVCxTreeListSettingsText(MVCxTreeList treeList)
			: base(treeList) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string CustomizationWindowCaption { get { return base.CustomizationWindowCaption; } set { base.CustomizationWindowCaption = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string LoadingPanelText { get { return base.LoadingPanelText; } set { base.LoadingPanelText = value; } }
	}
	public class MVCxTreeListSettingsExport {
		ASPxTreeListPrintSettings printSettings;
		TreeListExportStyles styles;
		public MVCxTreeListSettingsExport() {
			this.printSettings = new ASPxTreeListPrintSettings();
			this.styles = new TreeListExportStyles(null);
		}
		public string FileName { get; set; }
		public ASPxTreeListPrintSettings PrintSettings { get { return printSettings; } }
		public TreeListExportStyles Styles { get { return styles; } }
		public ASPxTreeListRenderBrickEventHandler RenderBrick { get; set; }
	}
	public class MVCxTreeListPrintSettings : ASPxTreeListPrintSettings {
		public MVCxTreeListPrintSettings()
			: base() {
		}
	}
}
