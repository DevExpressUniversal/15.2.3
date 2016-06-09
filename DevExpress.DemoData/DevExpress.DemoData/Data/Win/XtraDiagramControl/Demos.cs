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
using System.Linq;
using System.Text;
namespace DevExpress.DemoData.Model {
	public static partial class Repository {
		static List<Demo> CreateXtraDiagramControlDemos(Product product) {
			return new List<Demo> {
				new SimpleDemo(product,
					p => new List<Module>(),
					name: "DiagramDesigner",
					displayName: @"Diagram Designer",
					csSolutionPath: @"\WinForms\CS\DiagramDesigner\DiagramDesigner.sln",
					vbSolutionPath: @"\WinForms\VB\DiagramDesigner\DiagramDesigner.sln",
					launchPath: @"\WinForms\Bin\DiagramDesigner.exe",
					addedIn: KnownDXVersion.V152
				),
				new SimpleDemo(product,
					Create_XtraDiagram_Tutorial_Modules,
					name: "MainDemo",
					displayName: @"Main Demo",
					csSolutionPath: @"\WinForms\CS\DiagramMainDemo\DiagramMainDemo.sln",
					vbSolutionPath: @"\WinForms\VB\DiagramMainDemo\DiagramMainDemo.sln",
					launchPath: @"\WinForms\Bin\DiagramMainDemo.exe",
					addedIn: KnownDXVersion.V152
				),
			};
		}
		static List<Module> Create_XtraDiagram_Tutorial_Modules(Demo demo) {
			return new List<Module> {
				new SimpleModule(demo,
					name: "About",
					displayName: @"DevExpress XtraDiagram %MarketingVersion%",
					group: "About",
					type: "DevExpress.XtraDiagram.Demos.About",
					description: @"",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "FlowChart",
					displayName: @"Flow Chart",
					group: "Features",
					type: "DevExpress.XtraDiagram.Demos.FlowChartModule",
					description: @"This example shows a flow chart created using the Diagram Control. All the basic and advanced operations on the diagram are supported. You can move, resize, rotate and delete diagram shapes, change their text (double-click a shape), perform cut-copy-paste operations and zoom the diagram with the CTRL+Mouse Wheel gesture. Note how shapes are snapped to the grid and other shapes when you drag or resize them.",
					addedIn: KnownDXVersion.V152,
					newUpdatedPriority: 1
				),
			   new SimpleModule(demo,
					name: "CycleDiagram",
					displayName: @"Cycle Diagram",
					group: "Features",
					type: "DevExpress.XtraDiagram.Demos.CycleDiagramModule",
					description: @"This example demonstrates a sample diagram created using the Diagram Control. Visio-inspired themes are enabled with the AllowThemes option. This allows you to customize the appearance of shapes with a single property setting.",
					addedIn: KnownDXVersion.V152,
					newUpdatedPriority: 5
				),
			   new SimpleModule(demo,
					name: "CodeFirst",
					displayName: @"Code First",
					group: "API Code Examples",
					type: "DevExpress.XtraDiagram.Demos.CodeFirstModule",
					description: @"This section includes examples demonstrating the Diagram Control's API. The examples show how to create and customize diagram shapes and connectors, run the diagram designer, perform various operations on the diagram (zooming, cut-copy-paste, undo/redo), change appearances, etc. You can modify the code snippets at runtime and immediately see the result.",
					addedIn: KnownDXVersion.V152,
					isCodeExample: true,
					newUpdatedPriority: int.MinValue
				),
			};
		}
	}
}
