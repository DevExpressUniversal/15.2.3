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
		static List<Module> Create_DXDiagram_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new WpfModule(demo,
					name: "Decision Flowchart",
					displayName: @"Decision Flowchart",
					group: "Demos",
					type: "DiagramDemo.FlowchartModule",
					shortDescription: @"This demo shows the flowchart created using the Diagram control.",
					description: @"
                        <Paragraph>
                        Use basic flowchart shapes and connectors to create a flowchart. You can switch to the Design tab and apply a color scheme to the diagram.
                        </Paragraph>",
					addedIn: KnownDXVersion.V152,
					isFeatured: false
				),
				new WpfModule(demo,
					name: "Custom Shapes",
					displayName: @"Custom Shapes",
					group: "Demos",
					type: "DiagramDemo.OfficePlanModule",
					shortDescription: @"This demo illustrates the usage of custom shapes for creating a floor plan.",
					description: @"
                        <Paragraph>
                        In this demo, custom shapes created via templates represent objects in the floor plan.
                        </Paragraph>",
					addedIn: KnownDXVersion.V152,
					isFeatured: false
				),
				new WpfModule(demo,
					name: "Product Flow Diagram",
					displayName: @"Product Flow Diagram",
					group: "Demos",
					type: "DiagramDemo.ProductFlowDiagramModule",
					shortDescription: @"A diagram that represents business data.",
					description: @"
                        <Paragraph>
                        The diagram represents the correlation between customers and categories of products. You can filter the grid's contents by clicking the diagram shapes.
                        </Paragraph>",
					addedIn: KnownDXVersion.V152,
					isFeatured: false
				),
				new WpfModule(demo,
					name: "OrgChart",
					displayName: @"OrgChart",
					group: "Demos",
					type: "DiagramDemo.OrgChartModule",
					shortDescription: @"This demo shows an organizational chart generated from a hierarchical data source.",
					description: @"
                        <Paragraph>
                        This demo illustrates usage of the DiagramOrgChartBehavior that allows you to use shapes to represent data objects.
                        </Paragraph>",
					addedIn: KnownDXVersion.V152,
					isFeatured: false
				),
				new WpfModule(demo,
					name: "Relationship Diagram",
					displayName: @"Relationship Diagram",
					group: "Demos",
					type: "DiagramDemo.RelationshipDiagramModule",
					shortDescription: @"Demonstrates a relationship diagram.",
					description: @"
                        <Paragraph>
                        Relationship diagrams show how items are related to one another.
                        </Paragraph>
                        <Paragraph>
                        Using the Diagram Control, you and your end-users can create diagrams of various types and styles. In this demo, shapes represent people, and connectors represent relations between people. The panel on the right shows friends and acquaintances of a selected person.
                        </Paragraph>",
					addedIn: KnownDXVersion.V152,
					isFeatured: false
				),
				new WpfModule(demo,
					name: "MainDemoModule",
					displayName: @"Designer Control",
					group: "Demos",
					type: "DiagramDemo.MainDemoModule",
					shortDescription: @"Diagram sandbox.",
					description: @"
                        <Paragraph>
                        Click the 'Launch DiagramDesignerControl' button.
                        </Paragraph>
                        <Paragraph>
                        Select a shape from one of the predefined sets displayed at the left and drag-drop it to the working area. You can search for a specific shape using the search field. After you have selected a shape on the working area, you can edit its properties using the property grid at the right. The ribbon at the top allows you to perform clipboard operations, format text, create connectors and basic shapes, rearrange and apply color schemes to them.
                        </Paragraph>
                        <Paragraph>
                        The created diagram can be saved to a file.
                        </Paragraph>",
					addedIn: KnownDXVersion.V152,
					isFeatured: false
				),
			};
		}
	}
}
