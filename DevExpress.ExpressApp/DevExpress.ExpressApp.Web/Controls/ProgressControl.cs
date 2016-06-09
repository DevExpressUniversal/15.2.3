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
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.ExpressApp.Web.Controls {
	#region Obsolete 14.2
	[ToolboxItem(false)]
	[Obsolete(ObsoleteMessages.TypeIsNotUsedAnymore, true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ProgressControl : WebControl, INamingContainer {
		private Panel coverPanel;
		private Table table;
		private TableCell progressCell;
		private TableCell textCell;
		private Image progressImage;
		public ProgressControl() {
			coverPanel = new Panel();
			coverPanel.Attributes["ondblclick"] = "stopProgress()";
			coverPanel.Style.Value = "visibility:hidden;position:absolute;";
			coverPanel.ControlStyle.CssClass = "ProgressHover";
			Controls.Add(coverPanel);
			table = RenderHelper.CreateTable();
			table.Style.Value = "visibility:hidden;position:absolute;";
			table.CellPadding = 0;
			table.CellSpacing = 0;
			textCell = new TableCell();
			textCell.Style["text-align"] = "right";
			progressCell = new TableCell();
			progressCell.Style["text-align"] = "left";
			progressImage = new Image();
			progressImage.Attributes["ondblclick"] = "stopProgress()";
			progressCell.Controls.Add(progressImage);
			table.Rows.Add(new TableRow());
			table.Rows[0].Cells.Add(textCell);
			table.Rows[0].Cells.Add(progressCell);
			Controls.Add(table);
			table.CssClass = "Progress";
			Text = "Process";
		}
		public override void RenderBeginTag(HtmlTextWriter writer) { }
		public override void RenderEndTag(HtmlTextWriter writer) { }
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			if(!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "progressText")) {
				if(string.IsNullOrEmpty(ImageName)) {
					progressImage.Visible = false;
					progressCell.Text = "...";
					Page.ClientScript.RegisterClientScriptBlock(GetType(), "progressText", @"
					function updateProgress() {
						var progressText = document.getElementById('" + progressCell.ClientID + @"');
						progressText.progressIndex++;
						if(progressText.progressIndex >= 7) {
							progressText.progressIndex = 0;
						}
						progressText.innerText = '';
						for(i=0;i<progressText.progressIndex;i++) {
							progressText.innerText += '.';
						}
						window.setTimeout('updateProgress()', 150);
					}", true);
				}
				else {
					Page.ClientScript.RegisterClientScriptBlock(GetType(), "progressText", @"
function updateProgress() {}
", true);
				}
			}
			if(!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "progress")) {
				Page.ClientScript.RegisterClientScriptBlock(GetType(), "progress",
					@"  var isCancelProgress = false;
						function positioning() {
							var viewportwidth;
							var viewportheight;
							if (typeof window.innerWidth != 'undefined') {
							  viewportwidth = window.innerWidth;
							  viewportheight = window.innerHeight;
							}
							else if (typeof document.documentElement != 'undefined'
							 && typeof document.documentElement.clientWidth !=
							 'undefined' && document.documentElement.clientWidth != 0) {
							   viewportwidth = document.documentElement.clientWidth;
							   viewportheight = document.documentElement.clientHeight;
							} 
							else {
							   viewportwidth = document.getElementsByTagName('body')[0].clientWidth,
							   viewportheight = document.getElementsByTagName('body')[0].clientHeight
							}
							var scrollX = document.body.scrollTop;
							if(scrollX == 0)
								scrollX = document.documentElement.scrollTop;
							var scrollY = document.body.scrollLeft;
							if(scrollY == 0)
								scrollY = document.documentElement.scrollLeft;

							var table = document.getElementById('" + table.ClientID + @"');
							table.style.top = Math.round(scrollX + (viewportheight - table.offsetHeight)/2).toString() + ""px"";
							table.style.left = Math.round(scrollY + (viewportwidth - table.offsetWidth)/2).toString() + ""px"";
						}
						function cancelProgress() {
                            //isCancelProgress = true;
                        }
						function startProgress() {
                            if(isCancelProgress) {
                                isCancelProgress = false;
                                return;
                            }
							document.getElementById('" + table.ClientID + @"').style.visibility = 'visible';
							var wait = document.getElementById('" + coverPanel.ClientID + @"');
							wait.style.visibility = 'visible';
							wait.style.left = 0;
							wait.style.top = 0;
							wait.style.width = document.body.scrollWidth;
							wait.style.height = document.body.scrollHeight;
							document.getElementById('" + progressCell.ClientID + @"').progressIndex = 0;
							document.body.onscroll = positioning;
							document.body.onresize = positioning;
							document.onstop = stopProgress;
							window.onblur = stopProgress;
							positioning();
							window.setTimeout('updateProgress()', 150);
						}
						function stopProgress() {
							document.getElementById('" + table.ClientID + @"').style.visibility = 'hidden';
							var wait = document.getElementById('" + coverPanel.ClientID + @"');
							wait.style.visibility = 'hidden';
							wait.started = false;
							document.body.onscroll = null;
							document.body.onresize = null;
						}
						function runProgressWithDelay() {
                            if(isCancelProgress) {
                                isCancelProgress = false;
                                return true;
                            }
							var wait = document.getElementById('" + coverPanel.ClientID + @"');
							if(!wait.started) {
								wait.started = true;
								window.setTimeout('startProgress()', 1);
								return true;
							}
							return false;
						}", true);
			}
			if(!Page.ClientScript.IsStartupScriptRegistered(GetType(), "progressHook")) {
				Page.ClientScript.RegisterStartupScript(GetType(), "progressHook",
					@"
					window.old_onunload = window.onunload;
					window.onunload = function (e) {
						stopProgress();
						if(window.old_onunload) {
							window.old_onunload();
						}
					};
					", true);
			}
		}
		[DefaultValue("Process")]
		public string Text {
			get { return textCell.Text; }
			set { textCell.Text = value; }
		}
		[Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), DefaultValue(""), UrlProperty]
		public string ImageName {
			get { return progressImage.ImageUrl; }
			set { progressImage.ImageUrl = value; }
		}
		public new string CssClass {
			get { return table.CssClass; }
			set { table.CssClass = value; }
		}
	}
	#endregion
}
