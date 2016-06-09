﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Web;
namespace DevExpress.Web {
	public class RibbonClientSideEvents : ClientSideEvents {
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonClientSideEventsCommandExecuted"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string CommandExecuted
		{
			get { return GetEventHandler("CommandExecuted"); }
			set { SetEventHandler("CommandExecuted", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonClientSideEventsActiveTabChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ActiveTabChanged
		{
			get { return GetEventHandler("ActiveTabChanged"); }
			set { SetEventHandler("ActiveTabChanged", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonClientSideEventsMinimizationStateChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string MinimizationStateChanged
		{
			get { return GetEventHandler("MinimizationStateChanged"); }
			set { SetEventHandler("MinimizationStateChanged", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonClientSideEventsFileTabClicked"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string FileTabClicked
		{
			get { return GetEventHandler("FileTabClicked"); }
			set { SetEventHandler("FileTabClicked", value); }
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string DialogBoxLauncherClicked {
			get { return GetEventHandler("DialogBoxLauncherClicked"); }
			set { SetEventHandler("DialogBoxLauncherClicked", value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("CommandExecuted");
			names.Add("ActiveTabChanged");
			names.Add("MinimizationStateChanged");
			names.Add("FileTabClicked");
			names.Add("DialogBoxLauncherClicked");
		}
	}
}
