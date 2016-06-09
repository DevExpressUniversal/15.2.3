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

using System.Reflection;
using DevExpress.Xpf.Core.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
namespace DevExpress.Xpf.Docking.Design {
	internal class RegisterMetadata : MetadataProviderBase {
		protected override Assembly RuntimeAssembly { get { return typeof(DockLayoutManager).Assembly; } }
		protected override string ToolboxCategoryPath { get { return AssemblyInfo.DXTabNameNavigationAndLayout; } }
		protected override void PrepareAttributeTable(AttributeTableBuilder builder) {
			base.PrepareAttributeTable(builder);
			builder.AddCustomAttributes(typeof(DockLayoutManager), new FeatureAttribute(typeof(DockLayoutManagerInitializer)));
			builder.AddCustomAttributes(typeof(DockLayoutManager), new FeatureAttribute(typeof(DockLayoutManagerAdornerProvider)));
			builder.AddCustomAttributes(typeof(DockLayoutManager), new FeatureAttribute(typeof(DockLayoutManagerParentAdapter)));
			builder.AddCustomAttributes(typeof(DockLayoutManager), new FeatureAttribute(typeof(DockLayoutManagerNewElementAdornerProvider)));
			builder.AddCustomAttributes(typeof(DockLayoutManager), new FeatureAttribute(typeof(DockLayoutManagerContextMenuProvider)));
			builder.AddCustomAttributes(typeof(LayoutPanel), new FeatureAttribute(typeof(LayoutPanelContextMenuProvider)));
			builder.AddCustomAttributes(typeof(LayoutControlItem), new FeatureAttribute(typeof(LayoutControlItemContextMenuProvider)));
			builder.AddCustomAttributes(typeof(LayoutGroup), new FeatureAttribute(typeof(LayoutGroupContextMenuProvider)));
			builder.AddCustomAttributes(typeof(FixedItem), new FeatureAttribute(typeof(FixedItemContextMenuProvider)));
			if(DesignTimeHelper.IsVS2012OrGreater)
				builder.AddCustomAttributes(typeof(BaseLayoutItem), new FeatureAttribute(typeof(BaseLayoutItemPlacementAdapter)));
			else builder.AddCustomAttributes(typeof(BaseLayoutItem), new FeatureAttribute(typeof(EmptyPlacementAdapter)));
			builder.AddCustomAttributes(typeof(BaseLayoutItem), new FeatureAttribute(typeof(LayoutGroupChildAdornerProvider)));
			builder.AddCustomAttributes(typeof(BaseLayoutItem), new FeatureAttribute(typeof(ControlItemsHostChildAdornerProvider)));
			builder.AddCustomAttributes(typeof(LayoutGroup), new FeatureAttribute(typeof(LayoutGroupAdornerProvider)));
			builder.AddCustomAttributes(typeof(LayoutGroup), new FeatureAttribute(typeof(LayoutGroupDesignModeValueProvider)));
			DockingPropertyLineRegistrator.RegisterPropertyLines();
		}
	}
}
