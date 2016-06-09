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
		static List<Module> Create_MVVM_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new SimpleModule(demo,
					name: "About",
					displayName: @"DevExpress MVVM Practices %MarketingVersion%",
					group: "About",
					type: "DevExpress.MVVM.Demos.About",
					description: @"The advanced learning center, dedicated to building WinForms MVVM demos using the DevExpress MVVM Framework.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "BindingsAPI",
					displayName: @"Data Bindings",
					group: "API Code Examples",
					type: "DevExpress.MVVM.Demos.CodeExamples.BindingsModule",
					description: @"This section contains samples that illustrate binding UI elements to properties, defined within ViewModels. Samples under the 'UI Triggers' group demonstrate how to tie two UI elements together without the necessity to use intermediate properties.",
					addedIn: KnownDXVersion.Before142,
					isCodeExample: true
				),
				new SimpleModule(demo,
					name: "CommandingAPI",
					displayName: @"Commanding",
					group: "API Code Examples",
					type: "DevExpress.MVVM.Demos.CodeExamples.CommandingModule",
					description: @"Samples of this group illustrate how to implement MVVM commanding functionality, natively absent in the WinForms platform. Different samples illustrate commands of different types - from simple commands to commands with 'CanExecute' condition and parameters.",
					addedIn: KnownDXVersion.Before142,
					isCodeExample: true
				),
				new SimpleModule(demo,
					name: "ServicesAPI",
					displayName: @"Services",
					group: "API Code Examples",
					type: "DevExpress.MVVM.Demos.CodeExamples.ServicesModule",
					description: @"This section illustrates the use of services - tools that allow you to perform certain actions within a View while working with ViewModel's code without breaking the concept of separated layers. DevExpress services from these samples demonstrate calling message and dialog boxes from ViewModels, all staying within the 'clean' MVVM application scope.",
					addedIn: KnownDXVersion.Before142,
					isCodeExample: true
				),
				new SimpleModule(demo,
					name: "BehaviorsAPI",
					displayName: @"Behaviors",
					group: "API Code Examples",
					type: "DevExpress.MVVM.Demos.CodeExamples.BehaviorsModule",
					description: @"Behaviors allow you to implement additional functionality for a UI element without modifying this element itself. For instance, an attempt to perform a cancelable action (e.g. modify the CheckEdit value) will prompt the confirmation dialog that allows you to undo this action. The 'Event-To-Command Behavior' group illustrates how to bind commands to third-party controls.",
					addedIn: KnownDXVersion.Before142,
					isCodeExample: true
				),
				new SimpleModule(demo,
					name: "MessengerAPI",
					displayName: @"Messenger",
					group: "API Code Examples",
					type: "DevExpress.MVVM.Demos.CodeExamples.MessengerModule",
					description: @"Samples from this section illustrate the functionality of the Messenger - a DevExpress tool that provides ViewModels' communication. The sender ViewModel uses the 'Send' method to send a message of any type. Any ViewModel that calls the 'Register' method will be able to receive these messages and use data within them.",
					addedIn: KnownDXVersion.Before142,
					isCodeExample: true
				),
				new SimpleModule(demo,
					name: "DocumentManagerNavigation",
					displayName: @"DocumentManager",
					group: "Navigation",
					type: "DevExpress.MVVM.Demos.Navigation.DocumentManagerModule",
					description: @"This module illustrates the sample 'Expenses' application, built using the Document Manager Service. The page of each category is opened in a separate document, docked as a tab. End-users can re-arrange these tabs, close them or drag to make them floating.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "NavigationFrameNavigation",
					displayName: @"NavigationFrame",
					group: "Navigation",
					type: "DevExpress.MVVM.Demos.Navigation.NavigationFrameModule",
					description: @"This module illustrates the sample 'Expenses' application, built using the Navigation Frame Service. The central content area of the application can display only one application screen at a time. Navigating to every new application screen, be it major application page (Accounts, Categories or Transactions) or a detailed page (e.g., adding new account), overlaps the previously displayed screen.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "TabControlNavigation",
					displayName: @"XtraTabControl",
					group: "Navigation",
					type: "DevExpress.MVVM.Demos.Navigation.TabControlModule",
					description: @"This module illustrates the sample 'Expenses' application, built using the XtraTabControl Service. Similar to the Document Manager Service, this service allows you to create a simple tabbed UI that does not support any end-user operations on tabs, such as reordering or dragging.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "WindowedNavigation",
					displayName: @"Separate Windows",
					group: "Navigation",
					type: "DevExpress.MVVM.Demos.Navigation.WindowedModule",
					description: @"This module illustrates the sample 'Expenses' application, built using the Windowed Document Manager Service. Each category is opened within a floating window, that cannot be docked. All possible application screens can be opened simultaneously.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "TableCollectionView",
					displayName: @"Table (Collection View)",
					group: "Views",
					type: "DevExpress.MVVM.Demos.Views.TableCollectionViewModule",
					description: @"This demo demonstrates a separate application screen from the sample 'Expenses' application. All records are displayed in a traditional table layout.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "CardCollectionView",
					displayName: @"Cards (Collection View)",
					group: "Views",
					type: "DevExpress.MVVM.Demos.Views.CardCollectionViewModule",
					description: @"This demo demonstrates a separate application screen from the sample 'Expenses' application. Data source records are presented as tabs due to the Layout View applied to the Grid Control.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "TileLayoutView",
					displayName: @"Tiles (Collection View)",
					group: "Views",
					type: "DevExpress.MVVM.Demos.Views.TileCollectionViewModule",
					description: @"This demo demonstrates a separate application screen from the sample 'Expenses' application. Data source records are represented by Tiles within the Tile Control.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "DataLayoutView",
					displayName: @"Data Layout (Single Object View)",
					group: "Views",
					type: "DevExpress.MVVM.Demos.Views.DataLayoutViewModule",
					description: @"In this module, individual editors are bound to specific data sources. End-users may view these data source records within the editors' drop-down lists. Data sources are automatically updated when the 'Save' button is clicked.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "DataLayoutDetailCollectionView",
					displayName: @"Data Layout (Detail Collection)",
					group: "Views",
					type: "DevExpress.MVVM.Demos.Views.DataLayoutDetailCollectionViewModule",
					description: @"This demo illustrates a separate module from the sample 'Expenses' application. This module displays data source records in a traditional table layout. Individual records can be created or modified in a separate customizable edit form.",
					addedIn: KnownDXVersion.Before142
				),
			};
		}
	}
}
