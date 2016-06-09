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
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
namespace DevExpress.ExpressApp.Notifications.Web {
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[Description("Provides notifications functionality in ASP.NET XAF applications.")]
	[ToolboxBitmap(typeof(NotificationsAspNetModule), "Resources.Toolbox_Module_Notifications_Web")]
	[ToolboxItemFilter("Xaf.Platform.Web")]
	public sealed class NotificationsAspNetModule : ModuleBase {
		protected override ModuleTypeList GetRequiredModuleTypesCore() {
			ModuleTypeList result = base.GetRequiredModuleTypesCore();
			result.Add(typeof(DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule));
			result.Add(typeof(DevExpress.ExpressApp.Notifications.NotificationsModule));
			return result;
		}
		protected override IEnumerable<Type> GetRegularTypes() {
			return null;
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return Type.EmptyTypes;
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] {
				typeof(WebNotificationsController),
				typeof(WebNotificationsDialogViewController),
				typeof(WebNotificationsMessageListViewController),
			};
		}
	}
}
