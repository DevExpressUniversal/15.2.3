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
		static List<Module> Create_Xpo_XpoTutorials_Modules(Demo demo) {
			return new List<Module> {
				new SimpleModule(demo,
					name: "About",
					displayName: @"DevExpress Xpo %MarketingVersion%",
					group: "About",
					type: "DevExpress.Xpo.Demos.About",
					description: @"",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "GridServerMode",
					displayName: @"Grid Instant Feedback",
					group: "Connecting to a Data Store",
					type: "DevExpress.Xpo.Demos.GridServerMode",
					description: @"This tutorial demonstrates the XPInstantFeedbackSource control, functioning in Instant Feedback mode. The control is bound to a large amount of data. However, this does not affect the control's responsiveness. Instant Feedback mode is an asynchronous server mode, where data is loaded in a background thread from a server on demand. This ensures that the control does not freeze at all when you perform a ""heavy"" data operation.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "NestedUnitsOfWork",
					displayName: @"NestedUnitsOfWork",
					group: "Data Exchange and Manipulation",
					type: "DevExpress.Xpo.Demos.NestedUnitsOfWork",
					description: @"""This tutorial shows how to use transactions. To change the project's data (state, type, etc.) end-users must click the data navigator's 'Edit Record' button to invoke a modal dialog. Here they can change the project's data. To accept the changes (to save them to the database) the 'Save' button must be pressed.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "DeletingPersistentObjects",
					displayName: @"Deleting Persistent Objects",
					group: "Data Exchange and Manipulation",
					type: "DevExpress.Xpo.Demos.DeletingPersistentObjects",
					description: @"XPO supports deferred and immediate object deletion. When deferred deletion is enabled XPO does not physically delete the record in the underlying data store when the corresponding persistent object is deleted. Instead, it marks the record as deleted. Deleted objects can be restored later on. Immediate object deletion is an alternative to deferred deletion, the record in the underlying data store is deleted immediately after the object has been deleted.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "Images",
					displayName: @"Images",
					group: "Data Representation",
					type: "DevExpress.Xpo.Demos.Images",
					description: @"This tutorial shows how to store images. For this purpose, the 'Car.Picture' property has been implemented. ImageValueConverter is used to convert the value of this property to an array of bytes when saving it in a data store.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "Pagination",
					displayName: @"Pagination",
					group: "Data Representation",
					type: "DevExpress.Xpo.Demos.Pagination",
					description: @"This tutorial shows how to use the XPPageSelector component to split all the objects within the XPCollection into several pages.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "QueryingData",
					displayName: @"Querying Data",
					group: "Querying a Data Store",
					type: "DevExpress.Xpo.Demos.QueryingData",
					description: @"This tutorial shows how to filter both the data store and retrieved data (client). Querying a data store allows you to retrieve specific data that matches the specific criteria. Data that is stored in a temporary storage (e.g. XPCollection) can also be filtered.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "OneToManyRelations",
					displayName: @"One To Many Relations",
					group: "Object Relational Mapping",
					type: "DevExpress.Xpo.Demos.OneToManyRelations",
					description: @"This tutorial shows how to implement One-to-Many relationship. In this tutorial, orders can be associated with a specific customer by creating a relationship between the Orders property in the Customer object (the primary key) and the Customer property in the Orders object (the foreign key). As a result, each customer can have multiple orders.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "SimpleDataAwareApplication",
					displayName: @"Simple Data Aware Application",
					group: "Data Representation",
					type: "DevExpress.Xpo.Demos.SimpleDataAwareApplication",
					description: @"This tutorial shows how to create a simple application to enter and view customer's details. Data records are represented by the Customer persistent objects. Data is stored in a MS Access database which resides in the application folder.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "UnitsOfWork",
					displayName: @"UnitsOfWork",
					group: "Data Exchange and Manipulation",
					type: "DevExpress.Xpo.Demos.UnitsOfWork",
					description: @"In this tutorial you can select multiple projects (grid records) and change their status by pressing the 'Change Status of Selected Projects'. A Unit of Work keeps track of every change to every persistent object that can affect a data store. With a single call to the UnitOfWork.CommitChanges() method all the changes made to the persistent objects are automatically saved to a data store. When working with common sessions you need to save each persistent object individually.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "MSSQLServer",
					displayName: @"MS SQL Server",
					group: "Connecting to a Data Store",
					type: "DevExpress.Xpo.Demos.MSSQLServer",
					description: @"This tutorial shows how to use MS SQL Server (local) as a persistent data store. After the connection has been established, XPO searches for the XPOProjects database to store the data. If the database isn't found, XPO creates it. If the connection with the SQL Server cannot be established, the MS Access OLEDB provider is used instead.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "Validation",
					displayName: @"Validation (error notifications)",
					group: "Data Exchange and Manipulation",
					type: "DevExpress.Xpo.Demos.Validation",
					description: @"This tutorial demonstrates how to provide error notifications so that to display error and warning icons within bound controls with invalid data. To support error notifications, the 'Contact' persistent class implements the IDXDataErrorInfo interface. A validation procedure which verifies data is implemented in the GetPropertyError method.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "XPViewDemo",
					displayName: @"XPView",
					group: "Data Representation",
					type: "DevExpress.Xpo.Demos.XPViewDemo",
					description: @"This tutorial demonstrates how to work with XPView objects to retrieve specific data from the database. XPView objects can be selected via the 'Views' combo box. Once selected, the XPView object is bound to the XtraGrid control.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "GeneratingPersistentClasses",
					displayName: @"Generating Persistent Classes for an Existing Database",
					group: "Querying a Data Store",
					type: "DevExpress.Xpo.Demos.GeneratingPersistentClasses",
					description: @"This tutorial demonstrates how to generate persistent classes for an existing database. Enter the name of the SQL Server, select the desired database and click Generate. The C# code that represents a persistent class hierarchy will be generated and displayed within 'Generated Persistent Classes'.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "Functions",
					displayName: @"Additional and Custom Functions",
					group: "Data Representation",
					type: "DevExpress.Xpo.Demos.Functions",
					description: @"This tutorial demonstrates how to use additional and custom functions in XPView and PersistentAliasAttribute. Data is stored in the InMemoryDataStore.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "AsyncLoading",
					displayName: @"Async Operations",
					group: "Data Exchange and Manipulation",
					type: "DevExpress.Xpo.Demos.AsyncLoading",
					description: @"This tutorial demonstrates how to load and commit data asynchronously, using XPCollection and UnitOfWork. Data is stored in the InMemoryDataStore. Request to datastore execute with 1 second delay.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "FreeJoin",
					displayName: @"Free Join",
					group: "Querying a Data Store",
					type: "DevExpress.Xpo.Demos.FreeJoin",
					description: @"This tutorial demonstrates how to use JoinOperand. Data is stored in the InMemoryDataStore.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "Single",
					displayName: @"Single",
					group: "Querying a Data Store",
					type: "DevExpress.Xpo.Demos.Single",
					description: @"This tutorial demonstrates how to use the Single aggregate type. The InMemoryDataStore data storage is used.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ObjectsInAlias",
					displayName: @"Objects In Alias",
					group: "Object Relational Mapping",
					type: "DevExpress.Xpo.Demos.ObjectsInAlias",
					description: @"This tutorial demonstrates how to use wire expressions for persistent aliases with object references, 'Iif' and 'IsNull' functions and Single aggregate types. The InMemoryDataStore data storage is used.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "StoredProcedures",
					displayName: @"Stored Procedures",
					group: "Querying a Data Store",
					type: "DevExpress.Xpo.Demos.StoredProcedures",
					description: @"This tutorial demonstrates how to generate classes and methods for stored procedures in an existing database. Enter the name of the SQL Server, select the desired database, and click Generate. The C# code that represents classes to work with stored procedures will be generated and displayed within 'Generated Classes And Methods'.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "DataCaching",
					displayName: @"Data Caching",
					group: "Connecting to a Data Store",
					type: "DevExpress.Xpo.Demos.DataCaching",
					description: @"This tutorial demonstrates how to create a cached data store. Requests are cached only for the Employees table. Data is stored in the InMemoryDataStore.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "DataServices",
					displayName: @"WCF Service for Data Store",
					group: "Transferring Data via WCF Services",
					type: "DevExpress.Xpo.Demos.DataServices",
					description: @"This tutorial demonstrates how to create and use a WCF service for IDataStore implementations. Data is stored in the InMemoryDataStore.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ObjectLayerServices",
					displayName: @"WCF Service for Object Layer",
					group: "Transferring Data via WCF Services",
					type: "DevExpress.Xpo.Demos.ObjectLayerServices",
					description: @"This tutorial demonstrates how to create and use a WCF service for ISerializableObjectLayer implementations. Data is stored in the InMemoryDataStore.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "InTransactionMode",
					displayName: @"InTransaction Mode",
					group: "Data Exchange and Manipulation",
					type: "DevExpress.Xpo.Demos.InTransactionMode",
					description: @"This tutorial demonstrates how to use InTransaction mode with a session or unit of work. XPView loads only employees located in London. Data is stored in the InMemoryDataStore.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "SpecifiedTypeObject",
					displayName: @"Checking Object Types in Criteria",
					group: "Data Representation",
					type: "DevExpress.Xpo.Demos.SpecifiedTypeObject",
					description: @"This tutorial demonstrates how to check object types in criteria strings via the IsExactType and IsInstanceOfType functions. IsExactType determines whether a particular object has a specified type. IsInstanceOfType determines whether a particular object is of a specified type or derives from it.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ODataV3Service",
					displayName: @"XPO OData V3 Service",
					group: "Transferring Data via WCF Services",
					type: "DevExpress.Xpo.Demos.ODataV3Service",
					description: @"This tutorial demonstrates how to create and use the XPO OData service for OData V3. Data is stored in the InMemoryDataStore.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModifiedProperties",
					displayName: @"Track Properties Modifications",
					group: "Data Exchange and Manipulation",
					type: "DevExpress.Xpo.Demos.ModifiedProperties",
					description: @"This tutorial demonstrates capabilities of the MergeCollision optimistic locking behavior.",
					addedIn: KnownDXVersion.Before142
				)
			};
		}
	}
}
