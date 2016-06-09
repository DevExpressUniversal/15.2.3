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
using System.ComponentModel.Design;
using System.Drawing;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.HtmlPropertyEditor.Web.Localization;
using DevExpress.ExpressApp.Updating;
using DevExpress.Utils;
namespace DevExpress.ExpressApp.HtmlPropertyEditor.Web {
	[DXToolboxItem(true)]
	[ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[Description("Contains the Property Editor which allows end-users to format string properties' values using Hyper Text Markup Language (HTML).")]
	[ToolboxBitmap(typeof(HtmlPropertyEditorAspNetModule), "Resources.Toolbox_Module_HtmlPropertyEditor_Web.bmp")]
	[ToolboxItemFilter("Xaf.Platform.Web")]
	[Designer("DevExpress.ExpressApp.HtmlPropertyEditor.Web.Design.HtmlPropertyEditorAspNetModuleDesigner, DevExpress.ExpressApp.HtmlPropertyEditor.Web.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(IDesigner))]
	[DesignerCategory("Component")]
	public sealed class HtmlPropertyEditorAspNetModule : ModuleBase {
		protected override IEnumerable<Type> GetRegularTypes() {
			return null;
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return Type.EmptyTypes;
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] { typeof(DevExpress.ExpressApp.HtmlPropertyEditor.Web.GridViewDisableHtmlEncodingViewController) };
		}
		protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) {
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.HtmlPropertyEditor, typeof(String), typeof(ASPxHtmlPropertyEditor), true)));
		}
		public override ICollection<Type> GetXafResourceLocalizerTypes() {
			return new Type[] {
				typeof(ASPxHtmlEditorResourceLocalizer),
				typeof(ASPxSpellCheckerResourceLocalizer)
			};
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
	}
}
