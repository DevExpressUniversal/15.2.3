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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using DevExpress.Utils;
namespace DevExpress.ExpressApp.ScriptRecorder.Win {
	[DXToolboxItem(true)]
	[ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[ToolboxItemFilter("Xaf.Platform.Win")]
	[ToolboxBitmap(typeof(ScriptRecorderWindowsFormsModule), "Resources.Toolbox_Module_ScriptRecorder_Win.ico")]
	[Description("Provides the capability to record actions performed on a Windows Forms XAF application's UI, and save them to an EasyTest script file.")]
	public sealed class ScriptRecorderWindowsFormsModule : ModuleBase {
		private void Application_CreateCustomLogonWindowControllers(object sender, CreateCustomLogonWindowControllersEventArgs e) {
			e.Controllers.Add(new WinScriptRecorderController());
		}
		protected override ModuleTypeList GetRequiredModuleTypesCore() {
			ModuleTypeList requiredModuleTypes = base.GetRequiredModuleTypesCore();
			requiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule));
			requiredModuleTypes.Add(typeof(ScriptRecorderModuleBase));
			return requiredModuleTypes;
		}
		protected override void Dispose(bool disposing) {
			if(Application != null) {
				Application.CreateCustomLogonWindowControllers -= new EventHandler<CreateCustomLogonWindowControllersEventArgs>(Application_CreateCustomLogonWindowControllers);
			}
			base.Dispose(disposing);
		}
		protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) { }
		protected override IEnumerable<Type> GetRegularTypes() {
			return null;
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return Type.EmptyTypes;
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] { typeof(WinScriptRecorderController) };
		}
		public override void Setup(XafApplication application) {
			base.Setup(application);
			application.CreateCustomLogonWindowControllers += new EventHandler<CreateCustomLogonWindowControllersEventArgs>(Application_CreateCustomLogonWindowControllers);
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
	}
}
