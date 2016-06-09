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

using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using System.Drawing;
using DevExpress.Web.ASPxScheduler.Internal;
using System.Web;
using DevExpress.Web.ASPxScheduler.Localization;
using DevExpress.Utils;
using System.ComponentModel;
using System;
namespace DevExpress.Web.ASPxScheduler {
	[
	DXWebToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(ToolboxBitmapAccess),
	ToolboxBitmapAccess.BitmapPath + "ASPxViewVisibleInterval.bmp")
	]
	public class ASPxViewVisibleInterval : ASPxSchedulerRelatedControl {
		#region Fields
		protected internal const string ScriptResourceName = "Scripts.SchedulerViewVisibleInterval.js";
		LiteralControl textControl;
		#endregion
		public ASPxViewVisibleInterval() {
			ClientIDHelper.SetClientIDModeToAutoID(this);
		}
		protected internal override ASPxSchedulerChangeAction RenderActions { get { return ASPxSchedulerChangeAction.NotifyVisibleIntervalsChanged | ASPxSchedulerChangeAction.StringFormatsChanged; } }
		protected override StylesBase CreateStyles() {
			return new ViewVisibleIntervalStyles(this);
		}
		protected internal override void CreateControlContentHierarchy() {
			this.textControl = new LiteralControl();
			this.MainCell.Controls.Add(textControl);
		}
		protected internal override void PrepareControlContentHierarchy() {
			ASPxScheduler scheduler = SchedulerControl;
			if (scheduler != null) {
				RenderUtils.AssignAttributes(this, MainTable);
				TimeInterval interval = scheduler.ActiveView.GetVisibleIntervals().Interval;
				SchedulerVisibleIntervalFormatter formatter = new SchedulerVisibleIntervalFormatter(scheduler.ActiveView.InnerView);
				formatter.WhiteSpace = "&nbsp;";
				textControl.Text = formatter.Format(interval, ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Caption_ViewVisibleInterval_Format));
				GetControlStyle().AssignToControl(MainCell);
			}
		}
		protected override Style CreateControlStyle() {
			return new AppearanceStyle();
		}
		#region Client scripts support
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptCommonResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxViewVisibleInterval.ScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientSchedulerViewVisibleInterval";
		}
		#endregion
	}
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	public class ViewVisibleIntervalBlock : ASPxSchedulerControlBlock {
		ASPxViewVisibleInterval intervalControl;
		TemplateContainerBase templateContainer;
		public ViewVisibleIntervalBlock(ASPxScheduler owner)
			: base(owner) {
		}
		public override string ContentControlID { get { return "viewVisibleIntervalBlock"; } }
		public override ASPxSchedulerChangeAction RenderActions { get { return ASPxSchedulerChangeAction.NotifyVisibleIntervalsChanged | ASPxSchedulerChangeAction.StringFormatsChanged; } }
		protected internal override void CreateControlHierarchyCore(Control parent) {
			this.templateContainer = TemplatesHelper.GetToolbarViewVisibleIntervalTemplateContainer(Owner);
			if(this.templateContainer != null) {
				parent.Controls.Add(this.templateContainer);
			}
			else {
				this.intervalControl = CreateViewVisibleInterval();
				SetupRelatedControl(intervalControl);
				parent.Controls.Add(intervalControl);
			}
		}
		protected virtual ASPxViewVisibleInterval CreateViewVisibleInterval(){
			return new ASPxViewVisibleInterval();
		}
		protected internal override void FinalizeCreateControlHierarchyCore(Control parent) {
		}
		protected internal override void PrepareControlHierarchyCore() {
			if(this.intervalControl == null)
				return;
			this.intervalControl.ControlStyle.CopyFrom(Owner.Styles.GetViewVisibleIntervalStyle());			
		}
	}
}
