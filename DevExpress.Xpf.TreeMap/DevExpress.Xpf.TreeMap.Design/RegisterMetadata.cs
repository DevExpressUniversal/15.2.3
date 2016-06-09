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
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
using DevExpress.Xpf.Utils;
using System;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Features;
using DevExpress.Xpf.Design;
namespace DevExpress.Xpf.TreeMap.Design {
	internal class RegisterHelper {
		public static readonly Type[] TreeMapDataAdapterTypes = new Type[] {
			typeof(TreeMapFlatDataAdapter), 
			typeof(TreeMapHierarchicalDataAdapter),
			typeof(TreeMapItemStorage)
		};
		public static readonly Type[] TreeMapLayoutAlgorithmTypes = new Type[] {
			typeof(SquarifiedLayoutAlgorithm), 
			typeof(StripedLayoutAlgorithm),
			typeof(SliceAndDiceLayoutAlgorithm)
		};
		public static readonly Type[] TreeMapColorizerTypes = new Type[] {
			typeof(TreeMapGradientColorizer), 
			typeof(TreeMapGroupGradientColorizer),
			typeof(TreeMapPaletteColorizer),
			typeof(TreeMapRangeColorizer)
		};
		public static readonly Type[] TreeMapPaletteTypes = new Type[] {
			typeof(OfficePalette), 
			typeof(Office2013Palette),
			typeof(Office2016Palette),
			typeof(DXTreeMapPalette),
			typeof(ChameleonPalette),
			typeof(InAFogPalette),
			typeof(NatureColorsPalette),
			typeof(NorthernLightsPalette),
			typeof(PastelKitPalette),
			typeof(TerracottaPiePalette),
			typeof(TheTreesPalette),
			typeof(BlueWarmPalette),
			typeof(BluePalette),
			typeof(BlueIIPalette),
			typeof(BlueGreenPalette),
			typeof(GreenPalette),
			typeof(GreenYellowPalette),
			typeof(YellowPalette),
			typeof(YellowOrangePalette),
			typeof(OrangePalette),
			typeof(OrangeRedPalette),
			typeof(RedOrangePalette),
			typeof(RedPalette),
			typeof(RedVioletPalette),
			typeof(VioletPalette),
			typeof(VioletIIPalette),
			typeof(MarqueePalette),
			typeof(SlipstreamPalette)
		};
		public static readonly Type[] ToolTipPositionTypes = new Type[] {
			typeof(ToolTipMousePosition),
			typeof(ToolTipRelativePosition)
		};
		public static void PrepareAttributeTable(AttributeTableBuilder builder) {
			builder.AddCustomAttributes(typeof(TreeMapControl), TreeMapControl.DataAdapterProperty.GetName(), new NewItemTypesAttribute(TreeMapDataAdapterTypes));
			builder.AddCustomAttributes(typeof(TreeMapControl), TreeMapControl.LayoutAlgorithmProperty.GetName(), new NewItemTypesAttribute(TreeMapLayoutAlgorithmTypes));
			builder.AddCustomAttributes(typeof(TreeMapControl), TreeMapControl.ColorizerProperty.GetName(), new NewItemTypesAttribute(TreeMapColorizerTypes));
			builder.AddCustomAttributes(typeof(TreeMapPaletteColorizerBase), TreeMapPaletteColorizerBase.PaletteProperty.GetName(), new NewItemTypesAttribute(TreeMapPaletteTypes));
			builder.AddCustomAttributes(typeof(ToolTipOptions), ToolTipOptions.PositionProperty.GetName(), new NewItemTypesAttribute(ToolTipPositionTypes));
			builder.AddCustomAttributes(typeof(TreeMapControl), new FeatureAttribute(typeof(TreeMapControlInitializer)));
		}
	}
	internal class RegisterMetadata : MetadataProviderBase {
		protected override void PrepareAttributeTable(AttributeTableBuilder builder) {
			RegisterHelper.PrepareAttributeTable(builder);
		}
		protected override Assembly RuntimeAssembly { get { return typeof(TreeMapControl).Assembly; } }
		protected override string ToolboxCategoryPath { get { return AssemblyInfo.DXTabNameData; } }
	}
	public class TreeMapControlInitializer : DefaultInitializer {
		public override void InitializeDefaults(ModelItem item) {
			ModelItem colorizerItem = ModelFactory.CreateItem(item.Context, typeof(TreeMapPaletteColorizer));
			item.Properties[TreeMapControl.ColorizerProperty.Name].SetValue(colorizerItem);
			InitializerHelper.Initialize(item);			
		}
	}
}
