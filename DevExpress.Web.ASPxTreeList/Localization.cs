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

using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Web.Localization;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxTreeList.Localization {
	public enum ASPxTreeListStringId {
		CustomizationWindowCaption, PopupEditFormCaption,
		CommandEdit, CommandNew, CommandDelete, CommandUpdate, CommandCancel,
		ConfirmDelete, RecursiveDeleteError,		
		Alt_Expand, Alt_Collapse, Alt_SortedAscending, Alt_SortedDescending,
		Alt_DragAndDropHideColumnIcon
	}
	public class ASPxTreeListResLocalizer : ASPxResLocalizerBase<ASPxTreeListStringId> {
		public ASPxTreeListResLocalizer() 
			: base(new ASPxTreeListLocalizer()) {
		}
		protected override string GlobalResourceAssemblyName {
			get { return AssemblyInfo.SRAssemblyTreeListWeb; }
		}
		protected override string ResxName {
			get { return "DevExpress.Web.ASPxTreeList.LocalizationRes"; }
		}
	}
	public class ASPxTreeListLocalizer : XtraLocalizer<ASPxTreeListStringId> {
		static ASPxTreeListLocalizer() {			
			ASPxActiveLocalizerProvider<ASPxTreeListStringId> provider = new ASPxActiveLocalizerProvider<ASPxTreeListStringId>(CreateResLocalizerInstance);
			SetActiveLocalizerProvider(provider);
		}
		static XtraLocalizer<ASPxTreeListStringId> CreateResLocalizerInstance() {
			return new ASPxTreeListResLocalizer();
		}
		public static string GetString(ASPxTreeListStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<ASPxTreeListStringId> CreateResXLocalizer() {
			return CreateResLocalizerInstance();
		}
		protected override void PopulateStringTable() {
			AddString(ASPxTreeListStringId.CustomizationWindowCaption, StringResources.CustomizationWindowCaption);
			AddString(ASPxTreeListStringId.PopupEditFormCaption, StringResources.PopupEditFormCaption);
			AddString(ASPxTreeListStringId.CommandEdit, StringResources.DataEditing_CommandEdit);
			AddString(ASPxTreeListStringId.CommandNew, StringResources.DataEditing_CommandNew);
			AddString(ASPxTreeListStringId.CommandDelete, StringResources.DataEditing_CommandDelete);
			AddString(ASPxTreeListStringId.CommandUpdate, StringResources.DataEditing_CommandUpdate);
			AddString(ASPxTreeListStringId.CommandCancel, StringResources.DataEditing_CommandCancel);
			AddString(ASPxTreeListStringId.ConfirmDelete, StringResources.DataEditing_ConfirmDelete);
			AddString(ASPxTreeListStringId.RecursiveDeleteError, StringResources.TreeList_RecursiveDeleteError);
			AddString(ASPxTreeListStringId.Alt_Expand, StringResources.Alt_ExpandButton);
			AddString(ASPxTreeListStringId.Alt_Collapse, StringResources.Alt_CollapseButton);
			AddString(ASPxTreeListStringId.Alt_SortedAscending, StringResources.Alt_SortedAscending);
			AddString(ASPxTreeListStringId.Alt_SortedDescending, StringResources.Alt_SortedDescending);
			AddString(ASPxTreeListStringId.Alt_DragAndDropHideColumnIcon, StringResources.Alt_DragAndDropHideColumnIcon);			
		}
	}
}
