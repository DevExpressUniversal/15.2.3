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
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.Web.FilterControl;
using DevExpress.Utils;
using DevExpress.Web.ASPxPivotGrid.Html;
namespace DevExpress.Web.ASPxPivotGrid.HtmlControls {
	public class PivotGridHtmlGroupHeader : InternalTable {
		List<Image> fSeparators = new List<Image>();
		PivotGridWebData data;
		PivotGroupItem group;
		InternalTableRow Row;
		public PivotGridHtmlGroupHeader(PivotGridWebData data, PivotGroupItem group) {
			this.data = data;
			this.group = group;
		}
		protected PivotGridWebData Data { get { return data; } }
		protected ScriptHelper ScriptHelper { get { return Data.PivotGrid.ScriptHelper; } }
		protected PivotGroupItem Group { get { return group; } }
		protected virtual ISupportsFieldsCustomization Control { get { return null; } }
		protected bool isInFieldList { get { return Control != null; } }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			ID = ScriptHelper.GetGroupHeaderID(Group);
			Row = new InternalTableRow();
			Controls.Add(Row);
			PopulateHeaders();
		}
		void PopulateHeaders() {
			foreach(PivotFieldItemBase field in Group.Fields) {
				if(!field.IsFieldVisibleInGroup)
					continue;
				Row.Controls.Add(CreateHeaderCell(Data, field));
				if(field.IsNextVisibleFieldInSameGroup)
					Row.Controls.Add(GetHorizontalLine());
			}
		}
		InternalTableCell CreateHeaderCell(PivotGridWebData data, PivotFieldItemBase field) {
			InternalTableCell cell = new InternalTableCell();
			cell.Controls.Add(new PivotGridHtmlHeaderContent(Data, field, DefaultBoolean.Default, Control));
			return cell;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			foreach(Image image in fSeparators)
				Data.PivotGrid.RenderHelper.GetGroupSeparatorImage().AssignToControl(image, Data.IsDesignMode);
			CellPadding = 0;
			CellSpacing = 0;
			RenderUtils.SetStyleStringAttribute(this, "width", "100%");
			if(Group.Fields.Count > 0 && (isInFieldList && Group.Fields[0].CanDragInCustomizationForm ||
										  !isInFieldList && Group.Fields[0].CanDrag))
				Attributes.Add(TouchUtils.TouchMouseDownEventName, ScriptHelper.GetHeaderMouseDown());
		}
		InternalTableCell GetHorizontalLine() {
			InternalTableCell cell = new InternalTableCell();
			Image image = RenderUtils.CreateImage();
			fSeparators.Add(image);
			cell.Controls.Add(image);
			return cell;
		}
	}
	public class PivotGridHtmlSolidGroupHeader : PivotGridHtmlGroupHeader {
		InternalTableCell fCell;
		ISupportsFieldsCustomization control;
		InternalTableCell sortButton;
		PivotGridHeaderTemplateItem item;
		PivotGridHtmlHeaderFilter filterButton;
		public PivotGridHtmlSolidGroupHeader(PivotGridWebData data, PivotGroupItem group, ISupportsFieldsCustomization control)
			: base(data, group) {
			this.control = control;
			if(Group != null && Group.Fields.Count > 0)
				this.item = new PivotGridHeaderTemplateItem(Data, Group.Fields[0], DefaultBoolean.Default, control);
		}
		protected PivotGridHeaderTemplateItem Item { get { return item; } }
		protected PivotGridHtmlHeaderFilter FilterButton { get { return filterButton; } }
		protected override ISupportsFieldsCustomization Control {
			get { return control; }
		}
		protected override void CreateControlHierarchy() {
			ID = ScriptHelper.GetGroupHeaderID(Group);
			InternalTableRow row = new InternalTableRow();
			fCell = new InternalTableCell();
			fCell.Text = Group.ToString();
			fCell.ID = ScriptHelper.GetGroupHeaderTextCellID(Group);
			if(string.IsNullOrEmpty(fCell.Text))
				fCell.Text = "&nbsp;";
			row.Controls.Add(fCell);
			if(control != null) {
				if(Item != null && Item.IsSortButtonVisible) {
					sortButton = new PivotGridHtmlHeaderSort(Item);
					row.Controls.Add(sortButton);
				}
				if(Item != null && Item.IsFilterButtonVisible) {
					this.filterButton = new PivotGridHtmlHeaderFilter(Item);
					row.Controls.Add(this.filterButton);
				}
			}
			Rows.Add(row);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Data.GetHeaderTextStyle((PivotFieldItem)Group.Fields[0]).AssignToControl(fCell);
			Data.GetHeaderStyle((PivotFieldItem)Group.Fields[0]).AssignToControl(this);
			BorderWidth = 1;
			if(control != null) {
				if(Group.Fields.Count > 0)
					Data.GetHeaderTableStyle((PivotFieldItem)Group.Fields[0]).AssignToControl(this);
				if(RenderUtils.Browser.IsOpera)
					RenderUtils.SetStyleStringAttribute(this, "border-collapse", "separate");   
				if(this.filterButton != null)
					this.filterButton.HorizontalAlign = HorizontalAlign.Right;
				if(fCell != null && !Item.IsAccessibilityCompliant && (Item.CanSort || Item.Field.CanSortOLAP))
					fCell.Attributes.Add("onclick", Item.HeaderClickScript);
				if(sortButton != null && !Item.IsAccessibilityCompliant)
					sortButton.Attributes.Add("onclick", Item.HeaderClickScript);
			}
		}
	}
}
