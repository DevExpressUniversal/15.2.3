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
namespace DevExpress.Web.ASPxTreeList {
	public enum TreeListPagerMode { 
		ShowAllNodes, 
		ShowPager 
	}
	public enum TreeListDataCacheMode { 
		Enabled, 
		Disabled, 
		Auto 
	}
	public enum TreeListRowKind {
		Header,
		Data, 
		Preview, 
		GroupFooter, 
		Footer,
		InlineEdit,
		EditForm,
		Error
	}
	public enum TreeListEditMode {
		Inline,
		EditForm,
		EditFormAndDisplayNode,
		PopupEditForm
	}
	public enum TreeListExpandCollapseAction {
		Button = 0,
		NodeClick = 1,
		NodeDblClick = 2
	}
	public enum TreeListColumnEditCaptionLocation {
		Near,
		Top,
		None
	}
	public enum TreeListCommandColumnButtonType {
		Edit,
		New,
		Delete,
		Update,
		Cancel,
		Custom
	}
	public enum TreeListEditFormTemplateReplacementType {
		Content,
		Editors,		
		CancelButton, 
		UpdateButton
	}
	public enum TreeListCustomButtonVisibility {
		Hidden,
		BrowsableNode,
		EditableNode,
		AllNodes
	}
	public enum TreeListEditingOperation {
		Insert,
		Update,
		Delete
	}
}
namespace DevExpress.Web.ASPxTreeList.Internal {
	public enum TreeListCommandId {
		Empty = 0,
		Expand = 1,
		Collapse = 2,
		Pager = 3,
		CustomDataCallback = 4,
		MoveColumn = 5,
		Sort = 6,		
		Dummy = 8,
		ExpandAll = 9,
		CollapseAll = 10,
		CustomCallback = 11,
		StartEdit = 12,
		UpdateEdit = 14,
		CancelEdit = 15,
		MoveNode = 16,
		DeleteNode = 17,
		StartEditNewNode = 18,
		GetNodeValues = 20
	}
	public enum TreeListRowIndentType { 
		None, 
		First, 
		Last, 
		Middle, 
		Root 
	}
	[Flags]
	public enum TreeListPendingChange {
		Empty = 0,
		FocusedNode = 1,
		Selection = 4
	}
	public enum TreeListGetNodeValuesCommandMode {
		ByKey = 0,
		Visible = 1,
		SelectedAll = 2,
		SelectedVisible = 3
	}
	[Flags]
	public enum TreeListRowsCreationFlags {
		Empty = 0,
		IgnorePaging = 1,
		ExpandAll = 2
	}
	public enum TreeListDataTableRenderPart { 
		Header,
		Content,
		Footer,
		All
	}
}
