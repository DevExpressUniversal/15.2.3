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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
namespace DevExpress.ExpressApp.FileAttachments.Web {
	[DXToolboxItem(true)]
	[ToolboxItemFilter("Xaf.Platform.Web")]
	[ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[Description("Includes Property Editors and Controllers to attach, save, open and download files in ASP.NET Web applications. Works with the Business Class Library's file-related data types.")]
	[ToolboxBitmap(typeof(FileAttachmentsAspNetModule), "Resources.Toolbox_Module_FileAttachment_Web.ico")]
	public sealed class FileAttachmentsAspNetModule : ModuleBase {
		public static IFileData CreateFileData(IObjectSpace objectSpace, IMemberInfo memberDescriptor) {
			IFileData result = null;
			if(!memberDescriptor.IsReadOnly && DataManipulationRight.CanInstantiate(memberDescriptor.MemberType, objectSpace)) {
				result = objectSpace.CreateObject(memberDescriptor.MemberType) as IFileData;
			}
			else {
				throw new InvalidOperationException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.UnableToInitializeFileDataProperty, memberDescriptor.Name, memberDescriptor.Owner.Type, memberDescriptor.MemberType));
			}
			return result;
		}
		protected override ModuleTypeList GetRequiredModuleTypesCore() {
			ModuleTypeList list = base.GetRequiredModuleTypesCore();
			list.Add(typeof(DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule));
			return list;
		}
		protected override IEnumerable<Type> GetRegularTypes() {
			return null;
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return Type.EmptyTypes;
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] { typeof(FileAttachmentController) };
		}
		protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) {
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.FileDataPropertyEditor, typeof(IFileData), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.FileDataPropertyEditor, typeof(IFileData), typeof(FileDataPropertyEditor), true)));
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
	}
}
