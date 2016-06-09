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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Internal {
	public class CCControlBase : ASPxInternalWebControl {
		private ASPxCloudControl fCloudControl = null;
		protected ASPxCloudControl CloudControl {
			get { return fCloudControl; }
		}
		public CCControlBase(ASPxCloudControl control) {
			fCloudControl = control;
		}
	}
	public class CCControl : CCControlBase {		
		private Table fMainTable = null;
		private TableCell fMainCell = null;
		public CCControl(ASPxCloudControl control) 
			: base(control) {			
		}
		protected override void ClearControlFields() {
			fMainTable = null;
			fMainCell = null;
		}
		protected override void CreateControlHierarchy() {
			fMainTable = RenderUtils.CreateTable();
			Controls.Add(fMainTable);
			TableRow mainRow = RenderUtils.CreateTableRow();
			fMainTable.Rows.Add(mainRow);
			fMainCell = RenderUtils.CreateTableCell();
			mainRow.Cells.Add(fMainCell);
			foreach (CloudControlItem item in CloudControl.ItemsInternal)
				fMainCell.Controls.Add(new ItemControl(item));
		}
		protected override void PrepareControlHierarchy() {
			AppearanceStyleBase controlStyle = CloudControl.GetControlStyle();
			RenderUtils.AssignAttributes(CloudControl, fMainTable);
			controlStyle.AssignToControl(fMainTable, AttributesRange.Common | AttributesRange.Font);
			RenderUtils.AppendDefaultDXClassName(fMainTable, CloudControlStyles.ControlSystemStyleName);
			RenderUtils.SetVisibility(fMainTable, CloudControl.IsClientVisible(), true);
			controlStyle.AssignToControl(fMainCell, AttributesRange.Cell);
			if (CloudControl.HasItemOnClick())
				RenderUtils.SetStringAttribute(fMainCell, "onclick", CloudControl.GetControlOnClick());
			RenderUtils.SetPaddings(fMainCell, CloudControl.GetPaddings());
			RenderUtils.SetHorizontalAlign(fMainCell, CloudControl.HorizontalAlign);
		}
	}
	public class ItemControl : CCControlBase {
		private CloudControlItem fItem = null;
		private HyperLink fHyperlink = null;
		private WebControl fItemWrapper = null;
		private WebControl fValueSpan = null;		
		private WebControl fBeginTextSpan = null;
		private WebControl fEndTextSpan = null;
		public ItemControl(CloudControlItem item)
			: base(item.Collection.Owner as ASPxCloudControl) {
			fItem = item;
		}
		protected override void ClearControlFields() {		   
			fHyperlink = null;
			fItemWrapper = null;
			fValueSpan = null;			
			fBeginTextSpan = null;
			fEndTextSpan = null;
		}
		protected override void CreateControlHierarchy() {
			fHyperlink = RenderUtils.CreateHyperLink();			
			if (CloudControl.ShowValues || CloudControl.ItemBeginText.Length + CloudControl.ItemEndText.Length > 0) {
				fItemWrapper = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);				
				if (CloudControl.ItemBeginText != "") {
					fBeginTextSpan = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
					fBeginTextSpan.Controls.Add(RenderUtils.CreateLiteralControl(CloudControl.ItemBeginText));
					fItemWrapper.Controls.Add(fBeginTextSpan);
				}
				fItemWrapper.Controls.Add(fHyperlink);
				if (CloudControl.ItemEndText != "") {
					fEndTextSpan = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
					fEndTextSpan.Controls.Add(RenderUtils.CreateLiteralControl(CloudControl.ItemEndText));
					fItemWrapper.Controls.Add(fEndTextSpan);
				}
				if (CloudControl.ShowValues) {
					fValueSpan = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);					
					fValueSpan.Controls.Add(RenderUtils.CreateLiteralControl(CloudControl.GetFormattedValue(fItem)));
					fItemWrapper.Controls.Add(fValueSpan);
				}
				Controls.Add(fItemWrapper);
			}
			else
				Controls.Add(fHyperlink);		   
			Controls.Add(RenderUtils.CreateLiteralControl(" "));
		}
		protected override void PrepareControlHierarchy() {
			string text = CloudControl.HtmlEncode(fItem.Text).Replace(" ", "&nbsp;");
			string url = CloudControl.GetNavigateUrl(fItem);
			RenderUtils.PrepareHyperLink(fHyperlink, text, url, CloudControl.Target, fItem.ToolTip, CloudControl.IsEnabled());
			CloudControl.GetRankLinkStyle(fItem.Rank, fItemWrapper != null).AssignToHyperLink(fHyperlink);
			if(CloudControl.IsEnabled())
				RenderUtils.AppendDefaultDXClassName(fHyperlink, CloudControlStyles.LinkStyleName);
			if(fItemWrapper != null) {
				CloudControl.GetRankStyle(fItem.Rank).AssignToControl(fItemWrapper,
					AttributesRange.Font | AttributesRange.Common, true);
				if(CloudControl.HasItemOnClick()) {
					fItemWrapper.ID = CloudControl.GetItemElementID(fItem);
				}
				fItemWrapper.ToolTip = fItem.ToolTip;
			} else {
				fHyperlink.CssClass = RenderUtils.CombineCssClasses(fHyperlink.CssClass, CloudControl.GetRankStyle(fItem.Rank).CssClass);
				if(CloudControl.HasItemOnClick()) {
					fHyperlink.ID = CloudControl.GetItemElementID(fItem);
				}
				fHyperlink.ToolTip = fItem.ToolTip;
			}
			if (CloudControl.SpacerFontSize.IsEmpty || Browser.IsOpera) {
				Unit height = CloudControl.GetRankFontSize(CloudControl.RankCount-1).Unit;
				if (fItemWrapper != null)
					RenderUtils.SetLineHeight(fItemWrapper, height);
				else
					RenderUtils.SetLineHeight(fHyperlink, height);
			}
			if (fValueSpan != null) {
				RenderUtils.SetHorizontalMargins(fValueSpan, CloudControl.ValueSpacing, Unit.Empty);
				CloudControl.GetRankValueStyle(fItem.Rank).AssignToControl(fValueSpan);
				RenderUtils.AppendDefaultDXClassName(fValueSpan, CloudControlStyles.ValueStyleName);
			}
			Color beginEndTextColor = CloudControl.GetItemBeginEndTextColor(fItem.Rank);
			if(fBeginTextSpan != null) {
				CloudControl.GetItemBeginEndTextStyle(fItem.Rank).AssignToControl(fBeginTextSpan);
				RenderUtils.AppendDefaultDXClassName(fBeginTextSpan, CloudControlStyles.BeginEndTextStyleName);
			}
			if(fEndTextSpan != null) {
				CloudControl.GetItemBeginEndTextStyle(fItem.Rank).AssignToControl(fEndTextSpan);
				RenderUtils.AppendDefaultDXClassName(fEndTextSpan, CloudControlStyles.BeginEndTextStyleName);
			}
			if(IsRightToLeft()) {
				if(fHyperlink != null)
					fHyperlink.Attributes["dir"] = "rtl";
				if(fItemWrapper != null)
					fItemWrapper.Attributes["dir"] = "rtl";
			}
		}
		bool IsRightToLeft() {
			return (CloudControl as ISkinOwner).IsRightToLeft();
		}
	}
}
