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
using System.Text;
using System.ComponentModel.Design;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Collections;
using DevExpress.XtraReports.Native;
using System.Globalization;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Design.Commands;
namespace DevExpress.XtraReports.Design.Designers {
	public class ParameterDesigner : XRComponentDesignerBase {
	}
	public class CalculatedFieldDesigner : XRComponentDesignerBase {
	}
	public class FormatingRuleDesigner : XRComponentDesignerBase {
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			Verbs.Add(new XRDesignerVerb(ReportLocalizer.GetString(ReportStringId.Cmd_AddFormattingRule), OnXRDesignerVerbInvoke, FormattingComponentCommands.AddFormattingRule, true, false));
			Verbs.Add(new XRDesignerVerb(ReportLocalizer.GetString(ReportStringId.Cmd_EditFormattingRules), OnXRDesignerVerbInvoke, FormattingComponentCommands.EditFormattingRules, true, false));
			Verbs.Add(new XRDesignerVerb(ReportLocalizer.GetString(ReportStringId.Cmd_SelectControlsWithFormattingRule), OnXRDesignerVerbInvoke, FormattingComponentCommands.SelectControlsWithFormattingRule, true, false));
		}
	}
}
