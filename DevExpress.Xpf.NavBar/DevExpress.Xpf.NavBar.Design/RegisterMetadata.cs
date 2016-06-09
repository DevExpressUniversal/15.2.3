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

#if SL
extern alias Platform;
#endif
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.ComponentModel;
using System.Windows.Media.Media3D;
using System.Windows;
using Microsoft.Windows.Design.Model;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.Xpf.NavBar.Design.SmartTags;
#if SL
using Platform::DevExpress.Xpf.Core.Design;
using Platform::DevExpress.Xpf.Core;
using Platform::DevExpress.Xpf.NavBar;
using Platform::DevExpress.Xpf.Core.WPFCompatibility;
using Platform::DevExpress.Xpf.Utils;
using Platform::System.Windows.Media;
using Platform::DevExpress.Xpf.Core.Design.SmartTags;
#else
using DevExpress.Xpf.Core.Design.SmartTags;
using DevExpress.Xpf.Design;
using DevExpress.Xpf.Core.Design;
#endif
namespace DevExpress.Xpf.NavBar.Design {
	internal class RegisterMetadata : MetadataProviderBase {
		internal static Type[] ViewTypes = new Type[] { typeof(ExplorerBarView), typeof(NavigationPaneView), typeof(SideBarView) };
		protected override Assembly RuntimeAssembly { get { return typeof(NavBarControl).Assembly; } }
#if SILVERLIGHT
		protected override string ToolboxCategoryPath { get { return Platform::AssemblyInfo.DXTabNameNavigationAndLayout; } }
#else
		protected override string ToolboxCategoryPath { get { return AssemblyInfo.DXTabNameNavigationAndLayout; } }
#endif
		protected override void PrepareAttributeTable(AttributeTableBuilder builder) {
			base.PrepareAttributeTable(builder);
#if SILVERLIGHT
			builder.AddCustomAttributes(typeof(NavBarControl), NavBarControl.ViewProperty.GetName(),
				new NewItemTypesAttribute(typeof(ExplorerBarView), typeof(NavigationPaneView), typeof(SideBarView)));
#else
			builder.AddCustomAttributes(typeof(NavBarControl), NavBarControl.ViewProperty.Name,
				new NewItemTypesAttribute(ViewTypes));
#endif
			builder.AddCustomAttributes(typeof(NavBarGroup), "Items", new NewItemTypesAttribute(typeof(NavBarItem)));
#if SL
			builder.AddCustomAttributes(typeof(NavBarControl), new FeatureAttribute(typeof(NavBarControlDesignModeValueProvider)));
#endif
			AddDefaultInitializers(builder);
			AddContextMenus(builder);
			AddAdorners(builder);
			TypeDescriptor.AddAttributes(typeof(NavBarItem), new DesignTimeParentAttribute(typeof(NavBarControl), typeof(NavBarViewProvider)));
			TypeDescriptor.AddAttributes(typeof(NavBarGroup), new DesignTimeParentAttribute(typeof(NavBarControl), typeof(NavBarViewProvider)));
			TypeDescriptor.AddAttributes(typeof(NavBarViewBase), new DesignTimeParentAttribute(typeof(NavBarControl)));
			NavBarPropertyLinesRegistrator.RegisterPropertyLines();
			TypeDescriptor.AddAttributes(typeof(NavBarViewBase), new UseParentPropertyLinesAttribute(typeof(NavBarControl)));
		}
		void AddAdorners(AttributeTableBuilder builder) {
			builder.AddCustomAttributes(typeof(NavBarControl), new FeatureAttribute(typeof(NavBarControlSelectionAdornerProvider)));
			builder.AddCustomAttributes(typeof(NavBarControl), new FeatureAttribute(typeof(HookAdornerProvider)));
			TypeDescriptor.AddAttributes(typeof(NavBarGroupControl), new DesignerHitTestInfoAttribute(typeof(NavBarModelItemProvider)));
			TypeDescriptor.AddAttributes(typeof(NavBarItemControl), new DesignerHitTestInfoAttribute(typeof(NavBarModelItemProvider)));
			TypeDescriptor.AddAttributes(typeof(NavBarItem), new DesignTimeParentAttribute(typeof(NavBarControl)));
			TypeDescriptor.AddAttributes(typeof(NavBarGroup), new DesignTimeParentAttribute(typeof(NavBarControl)));
		}
		void AddDefaultInitializers(AttributeTableBuilder builder) {
			builder.AddCustomAttributes(typeof(NavBarControl), new FeatureAttribute(typeof(NavBarControlInitializer)));
			builder.AddCustomAttributes(typeof(NavBarGroup), new FeatureAttribute(typeof(NavBarGroupInitializer)));
			builder.AddCustomAttributes(typeof(NavBarItem), new FeatureAttribute(typeof(NavBarItemInitializer)));
		}
		void AddContextMenus(AttributeTableBuilder builder) {
			builder.AddCustomAttributes(typeof(NavBarControl), new FeatureAttribute(typeof(NavBarContextMenuProvider)));
			builder.AddCustomAttributes(typeof(NavBarGroup), new FeatureAttribute(typeof(NavBarGroupContextMenuProvider)));
		}
	}
#if SL
	public class NavBarControlDesignModeValueProvider : DesignModeValueProvider {
		public NavBarControlDesignModeValueProvider() {
			Properties.Add(typeof(NavBarControl), "Background");
		}
		public override object TranslatePropertyValue(ModelItem item, PropertyIdentifier identifier, object value) {
			if(identifier.Name == "Background" && value == null)
				return new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
			return base.TranslatePropertyValue(item, identifier, value);
		}
	}
#endif
}
