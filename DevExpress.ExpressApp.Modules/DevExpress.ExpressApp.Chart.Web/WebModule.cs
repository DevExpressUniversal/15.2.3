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
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Updating;
using DevExpress.Utils;
namespace DevExpress.ExpressApp.Chart.Web {
	[DXToolboxItem(true)]
	[ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[ToolboxBitmap(typeof(ChartAspNetModule), "Resources.Toolbox_Module_ChartListEditor_Web.ico")]
	[Description("Uses the XtraCharts controls suite to display object lists as a chart in ASP.NET XAF applications.")]
	[ToolboxItemFilter("Xaf.Platform.Web")]
	public sealed partial class ChartAspNetModule : ModuleBase {
		public const int DefaultChartPreferredWidth = 320;
		public const int DefaultChartPreferredHeight = 240;
		protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) {
			editorDescriptors.Add(new ListEditorDescriptor(new AliasRegistration(ChartListEditorBase.Alias, typeof(object), false)));
			editorDescriptors.Add(new ListEditorDescriptor(new EditorTypeRegistration(ChartListEditorBase.Alias, typeof(object), typeof(ASPxChartListEditor), false)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration("ChartPropertyEditor", typeof(IChartDataSourceProvider), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration("SparklinePropertyEditor", typeof(ISparklineProvider), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration("ChartPropertyEditor", typeof(IChartDataSourceProvider), typeof(ChartPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration("SparklinePropertyEditor", typeof(ISparklineProvider), typeof(SparklinePropertyEditor), true)));
		}
		protected override ModuleTypeList GetRequiredModuleTypesCore() {
			ModuleTypeList result = base.GetRequiredModuleTypesCore();
			result.Add(typeof(DevExpress.ExpressApp.Chart.ChartModule));
			return result;
		}
		protected override IEnumerable<Type> GetRegularTypes() {
			return new Type[]{
			typeof(IModelWebChartSettings)
			};
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return Type.EmptyTypes;
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return Type.EmptyTypes;
		}
		public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
			base.AddGeneratorUpdaters(updaters);
			updaters.Add(new ListViewDataAccessModeGeneratorUpdater(typeof(ASPxChartListEditor)));
		}
		public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			base.ExtendModelInterfaces(extenders);
			extenders.Add<IModelChartSettings, IModelWebChartSettings>();
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
	}
	public interface IModelWebChartSettings : IModelNode {
		[DefaultValue(ChartAspNetModule.DefaultChartPreferredWidth)]
#if !SL
	[DevExpressExpressAppChartWebLocalizedDescription("IModelWebChartSettingsPreferredWidth")]
#endif
		int PreferredWidth { get; set; }
		[DefaultValue(ChartAspNetModule.DefaultChartPreferredHeight)]
#if !SL
	[DevExpressExpressAppChartWebLocalizedDescription("IModelWebChartSettingsPreferredHeight")]
#endif
		int PreferredHeight { get; set; }
	}
}
