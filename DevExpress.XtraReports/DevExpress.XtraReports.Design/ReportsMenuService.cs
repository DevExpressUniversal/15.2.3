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
using System.ComponentModel.Design;
using DevExpress.Design.VSIntegration;
using DevExpress.XtraReports.Design.Commands;
using Microsoft.VisualStudio.CommandBars;
namespace DevExpress.XtraReports.Design {
	public class ToolWindowMenuItem : VSToolWindowMenuItem {
		protected override Type ResFinderType {
			get { return typeof(DevExpress.XtraReports.Design.ResFinder); }
		}
		public ToolWindowMenuItem(string caption, string bitmapResourceName, VSToolWindow toolWindow)
			: base(caption, bitmapResourceName, toolWindow) {
		}
	}
	public class CommandMenuItem : VSMenuItem {
		Action callback;
		public bool BeginGroup { get; set; }
		protected override Type ResFinderType {
			get { return typeof(DevExpress.XtraReports.Design.ResFinder); }
		}
		public CommandMenuItem(string caption, Action callback)
			: base(caption, null) {
			this.callback = callback;
		}
		protected override void Create(CommandBarControls parentCollection) {
			base.Create(parentCollection);
			Button.BeginGroup = BeginGroup;
		}
		protected override void OnButtonClick() {
			callback();
		}
	}
	public class CommandBarMenuItem : VSMenuItem {
		CommandBarFontService commandBarService;
		protected override Type ResFinderType {
			get { return typeof(DevExpress.XtraReports.Design.ResFinder); }
		}
		public CommandBarMenuItem(CommandBarFontService commandBarService)
			: base("Toolbar", null) {
			this.commandBarService = commandBarService;
		}
		protected override void Create(CommandBarControls parentCollection) {
			base.Create(parentCollection);
			Button.BeginGroup = true;
		}
		protected override void OnButtonClick() {
			commandBarService.ToggleCommandBarVisibility();
		}
	}
}
