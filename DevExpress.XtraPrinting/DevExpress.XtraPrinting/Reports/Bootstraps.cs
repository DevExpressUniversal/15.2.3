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
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.Utils.Design;
namespace DevExpress.XtraReports.UserDesigner.Native {
	[
	DXToolboxItem(true),
	ToolboxItemFilter("System.Windows.Forms"),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabReporting),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "StandardReportDesigner.bmp"),
	Description("A component to create a full-featured end-user reporting application with a BarManager main menu."),
	Designer("DevExpress.XtraReports.Design.StandardReportDesignerBootstrap, " + AssemblyInfo.SRAssemblyReportsDesign, typeof(System.ComponentModel.Design.IDesigner)),
	EditorBrowsable(EditorBrowsableState.Never),
	InitAssemblyResolver,
	]
	public class StandardReportDesigner : Component {
		public StandardReportDesigner()
			: base() {
				Subcribe();
		}
		[System.Security.SecuritySafeCritical]
		void Subcribe() {
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
		}
		System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args) {
			return null;
		}
	}
	[
	DXToolboxItem(true),
	ToolboxItemFilter("System.Windows.Forms"),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabReporting),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "RibbonReportDesigner.bmp"),
	Description("A component to create a full-featured end-user reporting application with a Ribbon main menu."),
	Designer("DevExpress.XtraReports.Design.RibbonReportDesignerBootstrap, " + AssemblyInfo.SRAssemblyReportsDesign, typeof(System.ComponentModel.Design.IDesigner)),
	EditorBrowsable(EditorBrowsableState.Never),
	InitAssemblyResolver,
	]
	public class RibbonReportDesigner : Component {
		public RibbonReportDesigner()
			: base() {
		}
	}
}
