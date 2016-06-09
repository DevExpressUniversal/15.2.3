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
		static List<Module> Create_DXCarousel_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new WpfModule(demo,
					name: "VisibleItemCount",
					displayName: @"Visible Item Count",
					group: "Features",
					type: "CarouselDemo.VisibleItemCount",
					shortDescription: @"This demo illustrates how changing the item count affects the appearance of the Carousel Panel.",
					description: @"
                        <Paragraph>
                        This demo illustrates how changing the item count affects the appearance of the Carousel Panel. Increase the item count to see more items simultaneously.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "MovingPath",
					displayName: @"Moving Path",
					group: "Features",
					type: "CarouselDemo.MovingPath",
					shortDescription: @"In this example, you can select the path along which the items move.",
					description: @"
                        <Paragraph>
                        In this example, you can select the path along which the items move. You can also customize the padding options for the selected path.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "AnimationEffects",
					displayName: @"Animation Effects",
					group: "Features",
					type: "CarouselDemo.AnimationEffects",
					shortDescription: @"This demo illustrates the various animation effects that take place when scrolling through items.",
					description: @"
                        <Paragraph>
                        This demo illustrates the various animation effects that take place when scrolling through items. You can customize scale factors, rotation angles and opacity settings.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "PositionEffects",
					displayName: @"Position Effects",
					group: "Features",
					type: "CarouselDemo.PositionEffects",
					shortDescription: @"In this demo, you can change the settings controlling the layout of items along the moving path.",
					description: @"
                        <Paragraph>
                        In this demo, you can change the settings controlling the layout of items along the moving path. You can also enable the transparency effect for the items.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "PhotoGallery",
					displayName: @"Photo Gallery",
					group: "Samples",
					type: "CarouselDemo.PhotoGallery",
					shortDescription: @"This is a Photo Gallery application, utilizing the Carousel Panel to browse through photos.",
					description: @"
                        <Paragraph>
                        This is a Photo Gallery application, utilizing the Carousel Panel to browse through photos. Click any small photo to see its full-sized version.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "MediaStore",
					displayName: @"Media Store",
					group: "Samples",
					type: "CarouselDemo.MediaStore",
					shortDescription: @"This sample illustrates how to implement the 3D effect when browsing through a series of music albums.",
					description: @"
                        <Paragraph>
                        This sample illustrates how to implement the 3D effect when browsing through a series of music albums.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "LanguageSelector",
					displayName: @"Language Selector",
					group: "Samples",
					type: "CarouselDemo.LanguageSelector",
					shortDescription: @"This example shows how to implement language selection using the Carousel Panel.",
					description: @"
                        <Paragraph>
                        This example shows how to implement language selection using the Carousel Panel.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "DXBook",
					displayName: @"DXBook",
					group: "Samples",
					type: "CarouselDemo.DXBook",
					shortDescription: @"This sample demonstrates how to implement an e-book reader using the Carousel Panel.",
					description: @"
                        <Paragraph>
                        This sample demonstrates how to implement an e-book reader using the Carousel Panel. To turn a page, you can click it or click the Prev and Next buttons within the Navigation Panel.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				)
			};
		}
	}
}
