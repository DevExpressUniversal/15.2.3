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
using DevExpress.Web;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using System.Drawing;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Utils;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxScheduler {
	[
	DXWebToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(ToolboxBitmapAccess),
	ToolboxBitmapAccess.BitmapPath + "ASPxTimeZoneEdit.bmp")
	]
	public class ASPxTimeZoneEdit : ASPxSchedulerRelatedControl {
		#region Fields
		protected internal const string ScriptResourceName = "Scripts.TimeZoneEdit.js";
		ASPxComboBox combo;
		protected internal override ASPxSchedulerChangeAction RenderActions { get { return ASPxSchedulerChangeAction.NotifyTimeZoneChanged; } }
		#endregion
		public ASPxTimeZoneEdit() {
			ClientIDHelper.SetClientIDModeToAutoID(this);
		}
		protected internal override void CreateControlContentHierarchy() {
			this.combo = new ASPxComboBox();
			InitializeComboBox(combo);
			MainCell.Controls.Add(combo);
		}
		protected virtual void InitializeComboBox(ASPxComboBox combo) {
			combo.EnableViewState = false;
		}
		protected internal override void PrepareControlContentHierarchy() {
			combo.ID = "cmd";
			combo.ParentSkinOwner = ParentSkinOwner;
			combo.ValueType = typeof(string);
			combo.Width = Unit.Percentage(100);
			PopulateItems();
			combo.ClientSideEvents.SelectedIndexChanged = GetComboOnChange();
		}
		protected internal virtual void PopulateItems() {
			if (SchedulerControl == null)
				return;
			System.Collections.ObjectModel.ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
			int count = timeZones.Count;
			for (int i = 0; i < count; i++) {
				TimeZoneInfo timeZone = timeZones[i];
				combo.Items.Add(timeZone.DisplayName, timeZone.Id.ToString());
			}
			string timeZoneId = GetActualTimeZone();
			combo.Value = timeZoneId.ToString();
		}
		public virtual string GetActualTimeZone() {
			return SchedulerControl.OptionsBehavior.ClientTimeZoneId;
		}
		#region Client scripts support
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptCommonResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxTimeZoneEdit.ScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder sb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(sb, localVarName, clientName);
			if(SchedulerControl != null)
				sb.AppendFormat("{0}.schedulerControlId='{1}';", localVarName, SchedulerControl.ClientID);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientTimeZoneEdit";
		}
		protected internal virtual string GetComboOnChange() {
			return String.Format("function(s, e) {{ ASPx.TimeZoneEditComboSelectedIndexChanged('{0}', s.GetValue()); }}", ClientID);
		}
		#endregion
	}
}
