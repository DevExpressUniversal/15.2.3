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

using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.Web.ASPxScheduler.Internal {
	public class NavigationButtonsBlock : ASPxSchedulerControlBlock {
		List<ASPxSchedulerNavigationButton> navigationButtons;
		public NavigationButtonsBlock(ASPxScheduler control)
			: base(control) {
			this.navigationButtons = new List<ASPxSchedulerNavigationButton>();
		}
		public override string ContentControlID { get { return "navButtonsBlock"; } }
		public override ASPxSchedulerChangeAction RenderActions { get { return ASPxSchedulerChangeAction.RenderNavigationButtons; } }
		protected internal override void CreateControlHierarchyCore(Control parent) {
			this.navigationButtons.Clear();
			ASPxNavigationButtonCalculator calc = new ASPxNavigationButtonCalculator(Owner);
			PrevNextAppointmentIntervalPairCollection data = calc.Calculate();
			if (data == null)
				return;
			int count = data.Count;
			for (int i = 0; i < count; i++) {
				PrevNextAppointmentIntervalPair item = data[i];
				XtraScheduler.Resource resource = item.Resource;
				ASPxSchedulerNavigationButton back = new ASPxSchedulerBackwardNavigationButton(Owner, item.PrevAppointmentInterval, resource);
				parent.Controls.Add(back);
				back.ID = SchedulerIdHelper.GenerateNavigationButtonDivId(2 * i);
				ASPxSchedulerNavigationButton next = new ASPxSchedulerForwardNavigationButton(Owner, item.NextAppointmentInterval, resource);
				parent.Controls.Add(next);
				next.ID = SchedulerIdHelper.GenerateNavigationButtonDivId(2 * i + 1);
				navigationButtons.Add(back);
				navigationButtons.Add(next);
			}
		}
		protected internal override void FinalizeCreateControlHierarchyCore(Control parent) {
		}
		protected internal override void PrepareControlHierarchyCore() {
		}
		public override void RenderCommonScript(StringBuilder sb, string localVarName, string clientName) {
			base.RenderCommonScript(sb, localVarName, clientName);
			int count = navigationButtons.Count;
			for (int i = 0; i < count; i++) {
				ASPxSchedulerNavigationButton button = navigationButtons[i];
				string resourceId = SchedulerIdHelper.GenerateScriptResourceId(button.Resource);
				sb.AppendFormat("{0}.AddNavigationButton(\"{1}\", {2}, \"{3}\");\n", localVarName, button.InnerId, resourceId, button.Anchor);
			}
		}
		protected override bool IsCollapsedToZeroSize() {
			return true;
		}
		protected override bool IsHiddenInitially() {
			return true;
		}
	}
}
