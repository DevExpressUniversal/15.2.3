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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxScheduler.Controls;
using DevExpress.Web.ASPxScheduler.Localization;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxScheduler.Internal {
	[ToolboxItem(false)]
	public class ASPxSchedulerStatusInfoManager : ASPxSchedulerRelatedControl {
		#region Fields
		protected internal const string ScriptResourceName = "Scripts.SchedulerStatusInfo.js";
		WebControl contentDiv;
		WebControl subject;
		WebControl detailInfo;
		HyperLink showDetailInfoLink;
		Table table;
		TableRow subjectRow;
		TableCell imageCell;
		TableCell subjectCell;
		Image exceptionImage;
		Image errorImage;
		Image warningImage;
		Image infoImage;
		#endregion
		public ASPxSchedulerStatusInfoManager() {
		}
		protected internal override ASPxSchedulerChangeAction RenderActions { get { return ASPxSchedulerChangeAction.Any; } }
		protected internal override void CreateControlContentHierarchy() {
			this.contentDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			MainCell.Controls.Add(contentDiv);
			table = RenderUtils.CreateTable();
			contentDiv.Controls.Add(table);
			contentDiv.ID = "contentDiv";
			subjectRow = RenderUtils.CreateTableRow();
			table.Rows.Add(subjectRow);
			imageCell = RenderUtils.CreateTableCell();
			subjectRow.Cells.Add(imageCell);
			subjectCell = RenderUtils.CreateTableCell();
			subjectRow.Cells.Add(subjectCell);
			infoImage = RenderUtils.CreateImage();
			warningImage = RenderUtils.CreateImage();
			errorImage = RenderUtils.CreateImage();
			exceptionImage = RenderUtils.CreateImage();
			imageCell.Controls.Add(infoImage);
			imageCell.Controls.Add(warningImage);
			imageCell.Controls.Add(errorImage);
			imageCell.Controls.Add(exceptionImage);
			showDetailInfoLink = RenderUtils.CreateHyperLink();
			subject = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			detailInfo = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			subjectCell.Controls.Add(subject);
			subjectCell.Controls.Add(showDetailInfoLink);
			this.contentDiv.Controls.Add(detailInfo);
		}
		protected internal virtual ImageProperties GetExceptionImage() {
			return SchedulerControl.Images.StatusInfo.GetImageProperties(Page, StatusInfoImages.ErrorImageName);
		}
		protected internal virtual ImageProperties GetErrorImage() {
			return SchedulerControl.Images.StatusInfo.GetImageProperties(Page, StatusInfoImages.ErrorImageName);
		}
		protected internal virtual ImageProperties GetWarningImage() {
			return SchedulerControl.Images.StatusInfo.GetImageProperties(Page, StatusInfoImages.WarningImageName);
		}
		protected internal virtual ImageProperties GetInfoImage() {
			return SchedulerControl.Images.StatusInfo.GetImageProperties(Page, StatusInfoImages.InfoImageName);
		}
		protected internal override void PrepareControlContentHierarchy() {
			bool designMode = DesignMode;
			if (designMode) {
				this.contentDiv.Visible = false;
				return;
			}
			if (SchedulerControl.IsCallback  && SchedulerControl.StateBlock.PostDataLoaded && !SchedulerControl.StateBlock.IsFirstTimeRendered ) {
				this.contentDiv.Visible = false;
				return;
			}
			GetExceptionImage().AssignToControl(exceptionImage, designMode);
			GetErrorImage().AssignToControl(errorImage, designMode);
			GetWarningImage().AssignToControl(warningImage, designMode);
			GetInfoImage().AssignToControl(infoImage, designMode);
			exceptionImage.Style.Add(HtmlTextWriterStyle.Display, "none");
			errorImage.Style.Add(HtmlTextWriterStyle.Display, "none");
			warningImage.Style.Add(HtmlTextWriterStyle.Display, "none");
			infoImage.Style.Add(HtmlTextWriterStyle.Display, "none");
			subject.ID = "subject";
			detailInfo.ID = "detailInfo";
			exceptionImage.ID = "exceptionImg";
			errorImage.ID = "errorImg";
			warningImage.ID = "warningImg";
			infoImage.ID = "infoImg";
			showDetailInfoLink.ID = "detailInfoLink";
			showDetailInfoLink.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_DetailInfo);
			showDetailInfoLink.NavigateUrl = String.Format("javascript:{{ASPx.SchedulerChangeElementVisibility(\"{0}\");}}", detailInfo.ClientID);
			detailInfo.Style.Add(HtmlTextWriterStyle.Display, "none");
			RenderUtils.AppendDefaultDXClassName(detailInfo, "dxscSIDetail");
			RenderUtils.AppendDefaultDXClassName(imageCell, "dxscSIImageCell");
			RenderUtils.AppendDefaultDXClassName(subjectCell, "dxscSISubjectCell");
			RenderUtils.AppendDefaultDXClassName(showDetailInfoLink, "dxscSIDetailInfoLink");
		}
		#region Client scripts support
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptCommonResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxSchedulerStatusInfo.ScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder sb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(sb, localVarName, clientName);
			if(SchedulerControl != null) {
				sb.AppendFormat("{0}.schedulerControlId='{1}';", localVarName, SchedulerControl.ClientID);
				sb.AppendFormat("{0}.RegisterScriptsRestartHandler();\n", localVarName);
				SchedulerStatusInfoCollection infos = SchedulerStatusInfoHelper.GetSortedSchedulerStatusInfos(SchedulerControl);
				if (infos != null && infos.Count > 0) {
					sb.AppendFormat("{0}.SetInfo(\"{1}\",\"{2}\",\"{3}\");\n", localVarName, infos[0].InfoType.ToString(), infos[0].Subject, infos[0].Detail);
				}
				else {
					sb.AppendFormat("{0}.ClearInfo();\n", localVarName);
				}
			}
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.SchedulerStatusInfoManager";
		}
		#endregion
		protected override string GetSkinControlName() {
			return "Scheduler";
		}
		protected override string[] GetChildControlNames() {
			return new string[] { "Editors" };
		}
	}
}
