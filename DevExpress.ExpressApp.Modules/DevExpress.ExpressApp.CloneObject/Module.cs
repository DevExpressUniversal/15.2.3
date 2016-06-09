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
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Updating;
using DevExpress.Utils;
namespace DevExpress.ExpressApp.CloneObject {
	[DXToolboxItem(true)]
	[ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[Description("Includes an Action that allows end-users to quickly create similar objects by cloning them and changing the required properties.")]
	[ToolboxBitmap(typeof(DevExpress.ExpressApp.CloneObject.CloneObjectModule), "Resources.Toolbox_Module_CloneObject.ico")]
	public sealed class CloneObjectModule : ModuleBase, IModelXmlConverter {
		protected override IEnumerable<Type> GetRegularTypes() {
			return new Type[]{
			typeof(IModelClassCloneable)
			};
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return Type.EmptyTypes;
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] { typeof(CloneObjectViewController) };
		}
		protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) { }
		public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			extenders.Add<IModelClass, IModelClassCloneable>();
			base.ExtendModelInterfaces(extenders);
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
		#region IModelXmlConverter Members
		public void ConvertXml(ConvertXmlParameters parameters) {
			if(parameters.XmlNodeName == "Class") {
				string oldProperty = "IsClonable";
				string newProperty = "IsCloneable";
				if(parameters.ContainsKey(oldProperty)) {
					parameters.Values[newProperty] = parameters.Values[oldProperty];
					parameters.Values.Remove(oldProperty);
				}
			}
		}
		#endregion
	}
	public interface IModelClassCloneable {
		[ModelValueCalculator("((IModelClassCloneable)BaseClass)", "IsCloneable")]
		[
#if !SL
	DevExpressExpressAppCloneObjectLocalizedDescription("IModelClassCloneableIsCloneable"),
#endif
 Category("Behavior")]
		bool IsCloneable { get; set; }
	}
}
