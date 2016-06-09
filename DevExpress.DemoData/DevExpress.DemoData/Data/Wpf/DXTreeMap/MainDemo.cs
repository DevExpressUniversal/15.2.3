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

using System;
using System.Collections.Generic;
namespace DevExpress.DemoData.Model {
	public static partial class Repository {
		static List<Module> Create_DXTreeMap_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new WpfModule(demo,
					name: "Flat Data Adapter",
					displayName: @"Flat Data Adapter",
					group: "",
					type: "TreeMapDemo.Grouping",
					shortDescription: @"This demo illustrates the use of the Flat Data Adapter.",
					description: @"
                        <Paragraph>
                        The TreeMap Control allows you to group flat data using the Flat Data Adapter. Groups are based on the specified data source member. In this demo, the billionaire statistics data is grouped initially by countries and then by their age.
                        </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.V152
				),
				new WpfModule(demo,
					name: "Selection",
					displayName: @"Selection",
					group: "",
					type: "TreeMapDemo.Selection",
					shortDescription: @"This demo shows the interaction capabilities of the TreeMap Control.",
					description: @"
                        <Paragraph>
                        The TreeMap supports highlighting and selection of tree map items. In this demo, the selection is used to demonstrate the GDP dynamics for a selected country.
                        </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.V152
				),
				new WpfModule(demo,
					name: "Colorizers",
					displayName: @"Colorizers",
					group: "",
					type: "TreeMapDemo.Colorizer",
					shortDescription: @"This demo illustrates Colorizers shipped with the TreeMap Control.",
					description: @"
                        <Paragraph>
                        The TreeMap Control allows you to specify the color for tree map items generated from a data source using Colorizers. This demo illustrates Colorizers shipped with the TreeMap Control.
                        </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.V152
				),
				new WpfModule(demo,
					name: "Customization",
					displayName: @"Customization",
					group: "",
					type: "TreeMapDemo.Customizing",
					shortDescription: @"This demo shows the customization capabilities of the TreeMap Control.",
					description: @"
                        <Paragraph>
                        Using the leaf and group headers data templates, it is possible to completely customize the appearance of the TreeMap Control. This demo illustrates the result of the data templates specification.
                        </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.V152
				),
				new WpfModule(demo,
					name: "Hierarchical Data Adapter",
					displayName: @"Hierarchical Data Adapter",
					group: "",
					type: "TreeMapDemo.Hierarchical",
					shortDescription: @"This demo illustrates the use of the Hierarchical Data Adapter.",
					description: @"
                        <Paragraph>
                        The TreeMap Control supports the hierarchical data visualization. This demo shows the Tree Map that is plotted based on hierarchical data.
                        </Paragraph>",
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.V152
				)
			};
		}
	}
}
