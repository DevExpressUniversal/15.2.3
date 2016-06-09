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
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxTreeList.Internal;
using System.Collections.Specialized;
namespace DevExpress.Web.ASPxTreeList {
	public class TreeListTemplates {
		IWebControlObject owner;
		ITemplate headerCaption, dataCell, preview, groupFooterCell, footerCell, editForm;
		public TreeListTemplates(IWebControlObject owner) {
			this.owner = owner;
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListTemplatesHeaderCaption"),
#endif
		NotifyParentProperty(true), DefaultValue(null), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(TreeListHeaderTemplateContainer))]
		public ITemplate HeaderCaption {
			get { return headerCaption; }
			set {
				if(value != HeaderCaption) {
					headerCaption = value;
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListTemplatesDataCell"),
#endif
		NotifyParentProperty(true), DefaultValue(null), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(TreeListDataCellTemplateContainer))]
		public ITemplate DataCell {
			get { return dataCell; }
			set {
				if(value != DataCell) {
					dataCell = value;
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListTemplatesPreview"),
#endif
		NotifyParentProperty(true), DefaultValue(null), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(TreeListPreviewTemplateContainer))]
		public ITemplate Preview {
			get { return preview; }
			set {
				if(value != Preview) {
					preview = value;
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListTemplatesGroupFooterCell"),
#endif
		NotifyParentProperty(true), DefaultValue(null), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(TreeListFooterCellTemplateContainer))]
		public ITemplate GroupFooterCell {
			get { return groupFooterCell; }
			set {
				if(value != GroupFooterCell) {
					groupFooterCell = value;
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListTemplatesFooterCell"),
#endif
		NotifyParentProperty(true), DefaultValue(null), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(TreeListFooterCellTemplateContainer))]
		public ITemplate FooterCell {
			get { return footerCell; }
			set {
				if(value != FooterCell) {
					footerCell = value;
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListTemplatesEditForm"),
#endif
		NotifyParentProperty(true), DefaultValue(null), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(TreeListEditFormTemplateContainer), BindingDirection.TwoWay)]
		public ITemplate EditForm {
			get { return editForm; }
			set {
				if(value != EditForm) {
					editForm = value;
					Changed();
				}
			}
		}
		protected IWebControlObject Owner { get { return owner; } }
		void Changed() {
			if(Owner != null)
				Owner.TemplatesChanged();
		}
	}
	public abstract class TreeListTemplateContainerBase : TemplateContainerBase {
		ASPxTreeList treeList;
		internal TreeListTemplateContainerBase(ASPxTreeList treeList, int itemIndex, object dataItem) 
			: base(itemIndex, dataItem) {
			this.treeList = treeList;
		}
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListTemplateContainerBaseTreeList")]
#endif
		public ASPxTreeList TreeList { get { return treeList; } }
		protected TreeListDataHelper TreeDataHelper { get { return TreeList.TreeDataHelper; } }
		protected TreeListRenderHelper RenderHelper { get { return TreeList.RenderHelper; } }
	}
	public class TreeListHeaderTemplateContainer : TreeListTemplateContainerBase {
		internal TreeListHeaderTemplateContainer(ASPxTreeList treeLsit, TreeListColumn column) 
			: base(treeLsit, column.Index, column) {
		}
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListHeaderTemplateContainerColumn")]
#endif
		public TreeListColumn Column { get { return DataItem as TreeListColumn; } }
	}
	public abstract class TreeListCellTemplateContainerBase : TreeListTemplateContainerBase {
		TreeListRowInfo rowInfo;
		internal TreeListCellTemplateContainerBase(ASPxTreeList treeList, TreeListRowInfo rowInfo)
			: base(treeList, -1, new TreeListTemplateDataItem(rowInfo)) {
			this.rowInfo = rowInfo;
		}
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListCellTemplateContainerBaseExpanded")]
#endif
		public bool Expanded { get { return RowInfo.Expanded; } }
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListCellTemplateContainerBaseSelected")]
#endif
		public bool Selected { get { return RowInfo.Selected; } }
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListCellTemplateContainerBaseExpandable")]
#endif
		public bool Expandable { get { return RowInfo.HasButton; } }
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListCellTemplateContainerBaseLevel")]
#endif
		public int Level { get { return RenderHelper.GetLevelFromIndentCount(RowInfo.Indents.Count); } }
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListCellTemplateContainerBaseNodeKey")]
#endif
		public string NodeKey { get { return RowInfo.NodeKey; } }
		protected TreeListRowInfo RowInfo { get { return rowInfo; } }
		public string GetText(TreeListDataColumn column) { return RenderHelper.GetDataCellText(RowInfo, column, false); }
		public object GetValue(TreeListDataColumn column) { return GetValue(column.FieldName); }
		public object GetValue(string fieldName) { return RowInfo.GetValue(fieldName); }
		public override void DataBind() {
			base.DataBind();
			NameValueCollection postCollection = ((IPostDataCollection)TreeList).PostDataCollection;
			if(!TreeList.InternalIsCallbacksEnabled() || postCollection == null)
				return;
			if(!TreeList.HasDataSource() || Page != null && Page.IsCallback) 
				RenderUtils.LoadPostDataRecursive(this, postCollection, true);
		}
	}
	public class TreeListDataCellTemplateContainer : TreeListCellTemplateContainerBase {
		TreeListDataColumn column;
		internal TreeListDataCellTemplateContainer(ASPxTreeList treeList, TreeListRowInfo rowInfo, TreeListDataColumn column)
			: base(treeList, rowInfo) {
			this.column = column;
		}
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListDataCellTemplateContainerColumn")]
#endif
		public TreeListDataColumn Column { get { return column; } }
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListDataCellTemplateContainerText")]
#endif
		public string Text { get { return GetText(Column); } }
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListDataCellTemplateContainerValue")]
#endif
		public object Value { get { return GetValue(Column); } }
	}
	public class TreeListPreviewTemplateContainer : TreeListCellTemplateContainerBase {
		internal TreeListPreviewTemplateContainer(ASPxTreeList treeList, TreeListRowInfo row) 
			: base(treeList, row) {
		}
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListPreviewTemplateContainerText")]
#endif
		public string Text { get { return RenderHelper.GetPreviewText(RowInfo, false); } }
	}
	public class TreeListFooterCellTemplateContainer : TreeListCellTemplateContainerBase {
		TreeListColumn column;		
		internal TreeListFooterCellTemplateContainer(ASPxTreeList treeList, TreeListRowInfo rowInfo, TreeListColumn column) 
			: base(treeList, rowInfo) {
			this.column = column;			
		}
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListFooterCellTemplateContainerColumn")]
#endif
		public TreeListColumn Column { get { return column; } }
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListFooterCellTemplateContainerText")]
#endif
		public string Text { get { return RenderHelper.GetFooterText(RowInfo, Column); } }
	}
	public class TreeListEditCellTemplateContainer : TreeListDataCellTemplateContainer {
		internal TreeListEditCellTemplateContainer(ASPxTreeList treeList, TreeListRowInfo rowInfo, TreeListDataColumn column) 
			: base(treeList, rowInfo, column) {
		}
	}
	public class TreeListEditFormTemplateContainer : TreeListCellTemplateContainerBase {
		int rowIndex;
		internal TreeListEditFormTemplateContainer(ASPxTreeList treeList, TreeListRowInfo rowInfo, int rowIndex) 
			: base(treeList, rowInfo) {
			this.rowIndex = rowIndex;
		}
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListEditFormTemplateContainerUpdateAction")]
#endif
		public string UpdateAction { get { return RenderHelper.GetCommandButtonOnClick(TreeListCommandColumnButtonType.Update, RowInfo.NodeKey); } }
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListEditFormTemplateContainerCancelAction")]
#endif
		public string CancelAction { get { return RenderHelper.GetCommandButtonOnClick(TreeListCommandColumnButtonType.Cancel, RowInfo.NodeKey); } }
		public override void DataBind() {
			ProcessReplacements(this);
			base.DataBind();
		}
		void ProcessReplacements(Control parent) {
			foreach(Control control in parent.Controls) {
				ASPxTreeListTemplateReplacement replacement = control as ASPxTreeListTemplateReplacement;
				if(replacement != null && CanCreateReplacement(replacement.ReplacementType)) {
					Control stuffing = CreateReplacementStuffing(replacement.ReplacementType);
					replacement.Controls.Clear();
					replacement.Controls.Add(stuffing);
					if(!DesignMode)
						RenderUtils.EnsureChildControlsRecursive(replacement, false);
				} else {
					ProcessReplacements(control);
				}					
			}
		}
		Control CreateReplacementStuffing(TreeListEditFormTemplateReplacementType type) {
			switch(type) {
				case TreeListEditFormTemplateReplacementType.Content:
					return new TreeListEditFormTable(RenderHelper, this.rowIndex, true);
				case TreeListEditFormTemplateReplacementType.Editors:
					return new TreeListEditFormTable(RenderHelper, this.rowIndex, false);
				case TreeListEditFormTemplateReplacementType.UpdateButton:
					return TreeListButtonInfo.Create(RenderHelper, TreeListUtils.FindCommandColumnForEditForm(TreeList), TreeListCommandColumnButtonType.Update, RowInfo.NodeKey).CreateControl();
				case TreeListEditFormTemplateReplacementType.CancelButton:
					return TreeListButtonInfo.Create(RenderHelper, TreeListUtils.FindCommandColumnForEditForm(TreeList), TreeListCommandColumnButtonType.Cancel, RowInfo.NodeKey).CreateControl();
				default:
					throw new NotImplementedException();
			}
		}
		bool CanCreateReplacement(TreeListEditFormTemplateReplacementType type) {
			if(type == TreeListEditFormTemplateReplacementType.UpdateButton || type == TreeListEditFormTemplateReplacementType.CancelButton)
				return RenderHelper.SettingsDataSecurity.AllowEdit || RenderHelper.SettingsDataSecurity.AllowInsert;
			return true;
		}
	}
	[DXWebToolboxItem(true), DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData),
	System.Drawing.ToolboxBitmap(typeof(ASPxTreeListTemplateReplacement), "Bitmaps256.ASPxTreeListTemplateReplacement.bmp")]
	public class ASPxTreeListTemplateReplacement : ASPxWebComponent, IStopLoadPostDataOnCallbackMarker {
		TreeListEditFormTemplateReplacementType type;
		public ASPxTreeListTemplateReplacement() 
			: base() {
			this.type = TreeListEditFormTemplateReplacementType.Content;
		}
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("ASPxTreeListTemplateReplacementReplacementType")]
#endif
		public TreeListEditFormTemplateReplacementType ReplacementType {
			get { return type; }
			set { type = value; }
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(!DesignMode && FindTemplateContainer() == null)
				throw new InvalidOperationException("A control of the 'ASPxTreeListTemplateReplacement' type can only be placed inside ASPxTreeList's EditForm template.");
		}
		TreeListEditFormTemplateContainer FindTemplateContainer() {
			Control current = Parent;
			while(current != null) {
				TreeListEditFormTemplateContainer result = current as TreeListEditFormTemplateContainer;
				if(result != null)
					return result;
				current = current.Parent;
			}
			return null;
		}   
	}
}
