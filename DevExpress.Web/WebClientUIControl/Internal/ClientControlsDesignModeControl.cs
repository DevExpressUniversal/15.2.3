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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using Guard = DevExpress.Utils.Guard;
namespace DevExpress.Web.WebClientUIControl.Internal {
	[ToolboxItem(false)]
	public class ClientControlsDesignModeControl : ASPxWebControl {
		readonly ClientControlsDesignModeInfo info;
		InternalTable table;
		WebControl content;
		Image contentIcon;
		WebControl contentTitle;
		WebControl contentDescription;
		ClientControlsDesignModeImages Images {
			get { return (ClientControlsDesignModeImages)ImagesInternal; }
		}
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Div; }
		}
		public ClientControlsDesignModeControl(ClientControlsDesignModeInfo info, ASPxWebControl ownerControl)
			: base(ownerControl) {
			Guard.ArgumentNotNull(info, "info");
			this.info = info;
			EnableViewState = false;
			if(ownerControl != null) {
				Height = ownerControl.Height;
			}
		}
		protected override void CreateControlHierarchy() {
			table = RenderUtils.CreateTable(false);
			table.Height = Height;
			Controls.Add(table);
			var row = RenderUtils.CreateTableRow();
			table.Rows.Add(row);
			var cell = RenderUtils.CreateTableCell();
			row.Cells.Add(cell);
			content = RenderUtils.CreateDiv();
			cell.Controls.Add(content);
			contentIcon = new Image();
			content.Controls.Add(contentIcon);
			contentTitle = RenderUtils.CreateDiv();
			contentTitle.Controls.Add(new LiteralControl(info.Title));
			content.Controls.Add(contentTitle);
			contentDescription = RenderUtils.CreateDiv();
			contentDescription.Controls.Add(new LiteralControl(info.Description));
			content.Controls.Add(contentDescription);
		}
		protected override void PrepareControlHierarchy() {
			ControlStyle.CssClass = "dxwcuic-root";
			table.ControlStyle.CssClass = "dxwcuic-dt-table";
			content.ControlStyle.CssClass = "dxwcuic-dt-content";
			contentTitle.ControlStyle.CssClass = "title";
			contentDescription.ControlStyle.CssClass = "description";
			Images.GetDesignModeIconProperties(Page).AssignToControl(contentIcon, DesignMode);
		}
		protected internal override void ResetControlHierarchy() {
			table = null;
			content = null;
			contentTitle = null;
			contentDescription = null;
		}
		protected override void RegisterDefaultSpriteCssFile() {
		}
		protected override ImagesBase CreateImages() {
			return info.CreateImages();
		}
		protected override void RegisterSystemCssFile() {
			if(DesignMode) {
				ResourceManager.RegisterCssResource(Page, typeof(ClientControlsDesignModeControl), WebClientUIControlSystemDesignModeCssResourceName);
			}
		}
		protected override bool HasRootTag() {
			return true;
		}
	}
}
