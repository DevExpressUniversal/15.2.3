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
		static List<Module> Create_DXPrinting_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new WpfModule(demo,
					name: "BadgesModule",
					displayName: @"Badges",
					group: "Printing Templates",
					type: "PrintingDemo.BadgesModule",
					shortDescription: @"This demo shows a badge report template.",
					description: @"
                        <Paragraph>
                        This demo illustrates a <Bold>Badge</Bold> report template. This template represents personal information in a badge-like form: a person's photo to the left and a person's details to the right.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "TableReportModule",
					displayName: @"Table",
					group: "Printing Templates",
					type: "PrintingDemo.TableReportModule",
					shortDescription: @"This demo illustrates a Table report template.",
					description: @"
                        <Paragraph>
                        This demo illustrates a <Bold>Table</Bold> report template. This template may be useful when it is necessary to print data rows with plain data in a tabular form, without grouping, multi-column and other layout options applied. When printing plain data, you simply need to specify two templates: the first for a page header (to print a table header) and the second for the detail section (to represent data records).
                        </Paragraph>
                        <Paragraph>
                        <Bold>Hint</Bold>: With DXPrinting, you can easily export data to PDF, XLS, XPS, HTML, TXT and all other popular formats, including images. To see this feature in action, choose <Bold>Export Document…</Bold> or <Bold>Send via e-mail…</Bold> on the Print Preview toolbar. Note that these features are not available in XBAP mode.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "GroupsModule",
					displayName: @"Groups",
					group: "Printing Templates",
					type: "PrintingDemo.GroupsModule",
					shortDescription: @"This demo illustrates a grouped report template.",
					description: @"
                        <Paragraph>
                        This demo illustrates a <Bold>Grouped</Bold> report template. In addition to plain data support, DXPrinting allows you to create grouped reports. In this mode, you can additionally provide templates and settings for group headers.
                        </Paragraph>
                        <Paragraph>
                        On the <Bold>Options</Bold> pane of this demo, you can specify different <Bold>Print Options</Bold>, which determine how a group should be printed. The following options are available:
                        </Paragraph>
                        <Paragraph>
                        • <Bold>Keep Together</Bold>. Indicates whether or not a group's content should be ""kept together"" on a single page, and whether or not splitting a group across several pages is allowed.
                        </Paragraph>
                        <Paragraph>
                        • <Bold>Repeat Header Every Page</Bold>. Indicates whether or not a group header must be repeated on top of every page, if a group is split across multiple pages.
                        </Paragraph>
                        <Paragraph>
                        • <Bold>Page Break After</Bold>. Indicates whether or not it is necessary to insert a page break after a group, so that each group starts from a new page.
                        </Paragraph>
                        <Paragraph>
                        <Bold>Note</Bold>: In this demo, every detail row is painted with either the odd or even style. This style is defined by a <Bold>Trigger</Bold> object defined in XAML.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "MultiColumnModule",
					displayName: @"Multi-Column",
					group: "Printing Templates",
					type: "PrintingDemo.MultiColumnModule",
					shortDescription: @"This demo illustrates a multi-column report template.",
					description: @"
                        <Paragraph>
                        This demo illustrates a <Bold>Multi-Column</Bold> report template. This may be useful, for example, when every detail row shows only a small amount of data, and it is necessary to print across the entire page width, or when creating cards or mailing labels.
                        </Paragraph>
                        <Paragraph>
                        On the <Bold>Options</Bold> pane of this demo, you can choose one of the available multi-column modes: <Bold>Across then down</Bold> (in rows) or <Bold>Down then across</Bold> (in columns).
                        </Paragraph>
                        <Paragraph>
                        <Bold>Note</Bold>: In this demo, every detail row is painted with either the odd or even style. This style is defined by a <Bold>Trigger</Bold> object defined in XAML.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "DrillDownReportModule",
					displayName: @"Drill-Down",
					group: "Printing Templates",
					type: "PrintingDemo.DrillDownReportModule",
					shortDescription: @"This demo illustrates the capability to create interactive reports with DXPrinting.",
					description: @"
                        <Paragraph>
                        This demo illustrates a drill-down report with collapsible document sections, which can be expanded by clicking an element in Document Preview.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				)
			};
		}
	}
}
