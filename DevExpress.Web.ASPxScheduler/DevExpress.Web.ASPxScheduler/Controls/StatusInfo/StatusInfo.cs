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
using System.Drawing;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxScheduler.Controls {
	[ToolboxItem(false),
	ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxSchedulerStatusInfo.bmp")]
	public class ASPxSchedulerStatusInfo : ASPxWebControl {
		#region Fields
		protected internal const string ScriptResourceName = "Scripts.SchedulerStatusInfo.js";
		WebControl mainDiv;
		ASPxScheduler scheduler;
		#endregion
		public ASPxSchedulerStatusInfo() {
		}
		public ASPxSchedulerStatusInfo(ASPxScheduler scheduler) {
			this.scheduler = scheduler;
		}
		#region Properties
		[DefaultValue(""), Themeable(false), AutoFormatDisable(),
		IDReferenceProperty(typeof(ASPxScheduler)),
		TypeConverter("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerIDConverter, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull)]
		public virtual string MasterControlID {
			get { return GetStringProperty("MasterCtrlId", String.Empty); }
			set {
				SetStringProperty("MasterCtrlId", String.Empty, value);
				LayoutChanged();
			}
		}
		[DefaultValue(0)]
		public int Priority {
			get {
				return GetIntProperty("Priority", 0);
			}
			set {
				SetIntProperty("Priority", 0, value);
			}
		}
		#endregion
		protected override void CreateControlHierarchy() {			
			this.mainDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			Controls.Add(mainDiv);
		}
		WebControl GetOuterContainer() {
			TableCell parentCell = this.Parent as TableCell;
			if (parentCell == null || parentCell.Controls.Count != 1)
				return this.mainDiv;
			TableRow parentRow = parentCell.Parent as TableRow;
			if (parentRow != null && parentRow.Cells.Count == 1)
				return parentRow;
			else
				return this.mainDiv;
		}
		bool IsInsideRow() {
			TableCell parentCell = this.Parent as TableCell;
			if (parentCell == null || parentCell.Controls.Count != 1)
				return false;
			TableRow parentRow = parentCell.Parent as TableRow;
			return (parentRow != null && parentRow.Cells.Count == 1);
		}
		protected override void PrepareControlHierarchy() {
			this.mainDiv.ID = "mainDiv";
			RenderUtils.SetStyleStringAttribute(GetOuterContainer(), "display", "none");
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
			if(this.scheduler == null && !IsMvcRender())
				this.scheduler = FindControlHelper.LookupControl(this, MasterControlID) as ASPxScheduler;
			if (scheduler != null || IsMvcRender()) {
				string schedulerClientId = scheduler == null && IsMvcRender() ? MasterControlID : scheduler.ClientID;
				if (!String.IsNullOrEmpty(schedulerClientId)) {
					sb.AppendFormat("{0}.schedulerControlId='{1}';\n", localVarName, schedulerClientId);
					sb.AppendFormat("{0}.priority={1};\n", localVarName, Priority);
				}
				bool isInsideRow = IsInsideRow();
				sb.AppendFormat("{0}.isInsideRow={1};\n", localVarName, HtmlConvertor.ToScript(isInsideRow));
			}
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.SchedulerStatusInfo";
		}
		#endregion
	}
}
