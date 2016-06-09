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

#if SILVERLIGHT
extern alias SL;
#endif
using System.Linq;
using System.Reflection;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using DevExpress.Xpf.Controls.Design.Features;
#if SILVERLIGHT
using SL::DevExpress.Xpf.Core.Design;
using SL::DevExpress.Xpf.Controls;
using SL::DevExpress.Xpf.Controls.Internal;
using SL::DevExpress.Xpf.WindowsUI;
using AssemblyInfo = SL::AssemblyInfo;
#else
using System.ComponentModel;
using DevExpress.Xpf.WindowsUI.Navigation;
using DevExpress.Xpf.Core.Design.Services;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.WindowsUI;
using Microsoft.Windows.Design.PropertyEditing;
using DevExpress.Xpf.Navigation;
#endif
namespace DevExpress.Xpf.Controls.Design {
	internal class RegisterMetadata : MetadataProviderBase {
		protected override Assembly RuntimeAssembly { get { return typeof(Book).Assembly; } }
		protected override string ToolboxCategoryPath { get { return AssemblyInfo.DXTabNameNavigationAndLayout; } }
		protected override void PrepareAttributeTable(AttributeTableBuilder builder) {
			base.PrepareAttributeTable(builder);
			RegisterFeatures(builder);
		}
		void RegisterFeatures(AttributeTableBuilder builder) {
			builder.AddCustomAttributes(typeof(Book), new FeatureAttribute(typeof(BookInitializer)));
			builder.AddCustomAttributes(typeof(SlideView), new FeatureAttribute(typeof(SlideViewInitializer)));
			builder.AddCustomAttributes(typeof(PageView), new FeatureAttribute(typeof(PageViewInitializer)));
			builder.AddCustomAttributes(typeof(PageView), new FeatureAttribute(typeof(PageViewDesignModeValueProvider)));
			builder.AddCustomAttributes(typeof(PageViewItem), new FeatureAttribute(typeof(PageViewItemAdornerProvider)));
			builder.AddCustomAttributes(typeof(FlipView), new FeatureAttribute(typeof(FlipViewInitializer)));
			builder.AddCustomAttributes(typeof(AppBar), new FeatureAttribute(typeof(AppBarInitializer)));
			builder.AddCustomAttributes(typeof(AppBar), new FeatureAttribute(typeof(AppBarDesignModeValueProvider)));
			builder.AddCustomAttributes(typeof(AppBar), new FeatureAttribute(typeof(AppBarAdornerProvider)));
#if !SILVERLIGHT
#pragma warning disable 612, 618
			builder.AddCustomAttributes(typeof(BarCodeControl), new FeatureAttribute(typeof(BarCodeControlInitializer)));
			builder.AddCustomAttributes(typeof(BarCodeControl), BarCodeControl.SymbologyProperty.Name, new NewItemTypesAttribute(BarCodeSymbologyStorage.GetSymbologyTypes().ToArray()));
#pragma warning restore 612, 618
			builder.AddCustomAttributes(typeof(TileBar), new FeatureAttribute(typeof(TileBarInitializer)));
			builder.AddCustomAttributes(typeof(TileNavPane), new FeatureAttribute(typeof(TileNavPaneInitializer)));
			builder.AddCustomAttributes(typeof(TileNavPane), new FeatureAttribute(typeof(TileNavPaneContextMenuProvider)));
			builder.AddCustomAttributes(typeof(OfficeNavigationBar), new FeatureAttribute(typeof(OfficeNavigationBarInitializer)));
#endif
			ControlsPropertyLineRegistrator.RegisterPropertyLines();
		}
	}
}
