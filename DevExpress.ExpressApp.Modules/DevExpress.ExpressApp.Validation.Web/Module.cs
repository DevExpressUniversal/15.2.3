﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp.Updating;
namespace DevExpress.ExpressApp.Validation.Web {
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[Description("Extends the ValidationModule with Controllers that manage the client-side validation in ASP.NET applications.")]
	[ToolboxBitmap(typeof(ValidationAspNetModule), "Resources.Toolbox_Module_Validation_Web.ico")]
	[ToolboxItemFilter("Xaf.Platform.Web")]
	public sealed class ValidationAspNetModule : ModuleBase {
		protected override IEnumerable<Type> GetRegularTypes() {
			return null;
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return Type.EmptyTypes;
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] {
				typeof(DevExpress.ExpressApp.Validation.Web.InplaceEditorsValidationControllerWeb),
			};
		}
		protected override ModuleTypeList GetRequiredModuleTypesCore() {
			return new ModuleTypeList(
				typeof(DevExpress.ExpressApp.Validation.ValidationModule),
				typeof(DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule));
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
	}	
}
