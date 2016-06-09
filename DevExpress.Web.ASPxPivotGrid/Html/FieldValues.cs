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
namespace DevExpress.Web.ASPxPivotGrid.HtmlControls {
	public abstract class PivotGridHtmlFieldValueCellBase : PivotGridHtmlTableCell {
		internal static string GetFieldValueCellID(PivotFieldValueItem valueItem) {
			return (valueItem.IsColumn ? "C" : "R") + valueItem.UniqueIndex.ToString();
		}
		PivotFieldValueItem item;
		Image collapsedButtonImage, sortByColumnImage;
		LiteralControl textControl;
		HyperLink link;
		List<PivotGridFieldPair> sortedFields;
		public PivotGridHtmlFieldValueCellBase(PivotGridWebData data, PivotFieldValueItem item, List<PivotGridFieldPair> sortedFields)
			: base(data) {
			this.item = item;
			this.sortedFields = sortedFields;
			ID = GetFieldValueCellID(Item);
		}
		protected override HtmlTextWriterTag TagKey {
			get { return PivotGrid.IsAccessibilityCompliantRender() ? HtmlTextWriterTag.Th : base.TagKey; }
		}
		protected List<PivotGridFieldPair> SortedFields { get { return sortedFields; } }
		protected bool IsAnyFieldSortedByThisSummary { get { return SortedFields != null && SortedFields.Count > 0; } }
		protected internal PivotFieldValueItem Item { get { return item; } }
		public PivotFieldItem Field {
			get {
				PivotFieldItem webField = Item.Field as PivotFieldItem;
				return webField;
			}
		}
		public Image CollapsedButtonImage { get { return collapsedButtonImage; } }
		public Image SortByColumnImage { get { return sortByColumnImage; } }
		public HyperLink Link { get { return link; } }
		public LiteralControl TextControl { get { return textControl; } }
		protected void CreateHierarchyCore() {
			if(Item.ShowCollapsedButton) {
				this.collapsedButtonImage = RenderUtils.CreateImage();
				if(PivotGrid.IsAccessibilityCompliantRender() && PivotGrid.IsEnabled()) {
					this.link = RenderUtils.CreateHyperLink();
					Controls.Add(this.link);
					Link.Controls.Add(this.collapsedButtonImage);
				} else
					if(RenderUtils.Browser.IsOpera && Data.OptionsCustomization.AllowExpandOnDoubleClick) {
						WebControl ctrl = RenderUtils.CreateDiv();
						ctrl.Controls.Add(collapsedButtonImage);
						ctrl.Style.Add("display", "inline");
						Controls.Add(ctrl);
					} else {
						Controls.Add(collapsedButtonImage);
					}
			}
			PivotGrid.RenderHelper.AddFieldValueContextMenu(ID, Item, SortedFields);
			CreateTextControl();
			if(IsAnyFieldSortedByThisSummary) {
				this.sortByColumnImage = RenderUtils.CreateImage();
				Controls.Add(sortByColumnImage);
			}
		}
		void CreateTextControl() {
			this.textControl = RenderUtils.CreateLiteralControl();
			Controls.Add(TextControl);
			string displayText = PivotGrid.HtmlEncode(Item.Text);
			TextControl.Text = !string.IsNullOrEmpty(displayText) ? displayText : EmptyNodeText;
		}
		protected virtual string EmptyNodeText { get { return "&nbsp;"; } }
		protected void PrepareHierarchyCore() {
			PivotFieldValueStyle fieldValueStyle = Data.GetFieldValueStyle(Item, Field);
			fieldValueStyle.AssignToControl(this);
			if(!fieldValueStyle.Paddings.IsEmpty)
				RenderUtils.SetPaddings(this, fieldValueStyle.Paddings);
			if(CollapsedButtonImage != null) {
				PivotGrid.RenderHelper.GetFieldValueCollapsedImage(Item.IsCollapsed).AssignToControl(CollapsedButtonImage, Data.IsDesignMode);
				Data.GetCollapsedButtonStyle().AssignToControl(CollapsedButtonImage, true);
				if(!fieldValueStyle.ImageSpacing.IsEmpty)
					RenderUtils.SetMargins(CollapsedButtonImage, new Paddings(0, 0, fieldValueStyle.ImageSpacing, 0));
			}
			if(SortByColumnImage != null) {
				PivotGrid.RenderHelper.GetSortByColumnImage().AssignToControl(SortByColumnImage, Data.IsDesignMode);
				Data.GetSortByColumnImageStyle().AssignToControl(SortByColumnImage, true);
				if(!fieldValueStyle.ImageSpacing.IsEmpty)
					RenderUtils.SetMargins(SortByColumnImage, new Paddings(fieldValueStyle.ImageSpacing, 0, 0, 0));
			}
			if(Item.IsLastFieldLevel)
				RenderUtils.AppendDefaultDXClassName(this, "lastLevel");
			if(PivotGrid.IsEnabled() && Item.AllowExpand) {
				string js = ScriptHelper.GetCollapsedImageOnClick(Item, true);
				if(Link != null) {
					Link.NavigateUrl = string.Format("javascript:{0}", js);
				} else {
					if(Item.AllowExpandOnDoubleClick && Data.OptionsCustomization.AllowExpandOnDoubleClick)
						Attributes.Add("ondblclick", "(function(){ ASPx.Selection.Clear(); " + js + "})();");
					if(CollapsedButtonImage != null)
						CollapsedButtonImage.Attributes.Add(Data.GetCollapsedImageEvent(), js);
				}
			}
			PivotGrid.RaiseHtmlFieldValuePrepared(this);
		}
	}
	public class PivotGridHtmlTemplatedFieldValueCell : PivotGridHtmlFieldValueCellBase {
		public PivotGridHtmlTemplatedFieldValueCell(PivotGridWebData data, PivotFieldValueItem item, List<PivotGridFieldPair> sortedFields)
			: base(data, item, sortedFields) {
		}
		ITemplate ValueTemplate {
			get {
				if(Field == null) return Data.FieldValueTemplate;
				return Field.ValueTemplate != null ? Field.ValueTemplate : Data.FieldValueTemplate;
			}
		}
		protected override void CreateControlHierarchy() {
			if(ValueTemplate != null) {
				PivotGridFieldValueTemplateItem templateItem = new PivotGridFieldValueTemplateItem(ID, Data.GetField(Field), Item, SortedFields);
				PivotGridFieldValueTemplateContainer templateContainer = new PivotGridFieldValueTemplateContainer(templateItem);
				Controls.Add(templateContainer);
				Data.SetupTemplateContainer(templateContainer, ValueTemplate);
			} else
				CreateHierarchyCore();
		}
		protected override void PrepareControlHierarchy() {
			PrepareHierarchyCore();
		}
	}
	public class PivotGridHtmlColumnFieldCell : PivotGridHtmlTemplatedFieldValueCell {
		public PivotGridHtmlColumnFieldCell(PivotGridWebData data, PivotFieldValueItem item, List<PivotGridFieldPair> sortedFields)
			: base(data, item, sortedFields) { }
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(Item.SpanCount > 1) ColumnSpan = Item.SpanCount;
			if(Item.CellLevelCount > 1) RowSpan = Item.CellLevelCount;
			if(PivotGrid.IsAccessibilityCompliantRender())
				RenderUtils.SetStringAttribute(this, "scope", "col");
		}
		protected bool IsLastGrandTotal(PivotFieldValueItem Item) {
			if(Item == null || Item.Data == null || Item.Data.Fields == null) return false;
			PivotGridFieldCollection Fields = Item.Data.Fields as PivotGridFieldCollection;
			if(Fields == null) return false;
			for(int i = 0; i < Fields.Count; i++)
				if(Fields[i].Area == PivotArea.DataArea && Fields[i].AreaIndex > Item.DataIndex)
					return false;
			return true;
		}
	}
	public class PivotGridHtmlRowFieldCell : PivotGridHtmlTemplatedFieldValueCell {
		public PivotGridHtmlRowFieldCell(PivotGridWebData data, PivotFieldValueItem item, List<PivotGridFieldPair> sortedFields)
			: base(data, item, sortedFields) { }
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(Item.SpanCount > 1) RowSpan = Item.SpanCount;
			if(Item.CellLevelCount > 1) ColumnSpan = Item.CellLevelCount;
			if(Data.Styles.FieldValueStyle.TopAlignedRowValues && Item.SpanCount > 1)
				VerticalAlign = VerticalAlign.Top;
			if(PivotGrid.IsAccessibilityCompliantRender())
				RenderUtils.SetStringAttribute(this, "scope", "row");
		}
	}
	public class PivotGridHtmlRowTreeInvisibleFieldCell : PivotGridHtmlRowFieldCell {
		public PivotGridHtmlRowTreeInvisibleFieldCell(PivotGridWebData data, PivotFieldValueItem item, List<PivotGridFieldPair> sortedFields)
			: base(data, item, sortedFields) { }
		WebControl spacerDiv;
		WebControl SpacerDiv {
			get{
				if(spacerDiv == null) {
					spacerDiv = new WebControl(HtmlTextWriterTag.Div);
					LiteralControl emptyText = new LiteralControl();
					emptyText.Text = base.EmptyNodeText;
					spacerDiv.Controls.Add(emptyText);
				}
				return spacerDiv;
			}
		}
		protected override string EmptyNodeText { get { return string.Empty; } }
		protected override void CreateControlHierarchy() {
			this.Controls.Add(SpacerDiv);
			base.CreateControlHierarchy();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.SetStyleUnitAttribute(SpacerDiv, "width", new Unit(Data.OptionsView.RowTreeOffset, UnitType.Pixel));
			RenderUtils.SetStyleUnitAttribute(this, "width", new Unit(Data.OptionsView.RowTreeOffset, UnitType.Pixel));
		}
	}
	public static class PivotGridHtmlFieldCellCreator {
		public static PivotGridHtmlTemplatedFieldValueCell CreateFieldValueCell(
				PivotGridWebData data, PivotFieldValueItem item, List<PivotGridFieldPair> sortedFields) {
			if(item.IsColumn) {
				return new PivotGridHtmlColumnFieldCell(data, item, sortedFields);
			}
			else {
				if(item.IsRowTree && !item.IsVisible)
					return new PivotGridHtmlRowTreeInvisibleFieldCell(data, item, sortedFields);
				else
					return new PivotGridHtmlRowFieldCell(data, item, sortedFields);
			}
		}
		public static int AddMaxLastLevelIndexCell(int maxLastLevelIndex, int itemMaxLastLevelIndex, IList<PivotGridHtmlTemplatedFieldValueCell> lastCells, PivotGridHtmlTemplatedFieldValueCell cell) {
			if(itemMaxLastLevelIndex > maxLastLevelIndex) {
				lastCells.Clear();
				maxLastLevelIndex = itemMaxLastLevelIndex;
			}
			if(itemMaxLastLevelIndex >= maxLastLevelIndex) {
				lastCells.Add(cell);
			}
			return maxLastLevelIndex;
		}
	}
}
