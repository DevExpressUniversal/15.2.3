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
		static List<Demo> CreateXafDemos(Product product) {
			return new List<Demo> {
				new FrameworkDemo(product,
					d => new List<Module>(),
					name: "Xcrm",
					displayName: @"XCRM",
					csSolutionPath: @"eXpressApp Framework\XCRM\CS\XCRM.sln",
					vbSolutionPath: null,
					shortDescription: @"This demo is a simple XAF application used for managing a company's interactions with customers, clients and sales prospects.",
					group: @"Showcase Demos",
					winPath: @"eXpressApp Framework\XCRM\Bin\XCRM.Win.exe",
					webPath: @"eXpressApp Framework\XCRM\Bin\XCRM.Web.ur1",
					buildItPath: null
				),
				new FrameworkDemo(product,
					d => new List<Module>(),
					name: "XVideoRental",
					displayName: @"XVideoRental",
					csSolutionPath: @"eXpressApp Framework\XVideoRental\CS\XVideoRental.sln",
					vbSolutionPath: null,
					shortDescription: @"XVideoRental is a real world application (RWA) that is designed as a clone of our WinForms VideoRental application. Although this demo shows only a small part of XAF capabilities, it is interesting because it follows the (no code)/ (design at runtime) approach.",
					group: @"Showcase Demos",
					winPath: @"eXpressApp Framework\XVideoRental\Bin\XVideoRental.Win.exe",
					webPath: null,
					buildItPath: @"OpenLink:http://community.devexpress.com/blogs/eaf/archive/tags/XVideoRental/default.aspx"
				),
				new FrameworkDemo(product,
					d => new List<Module>(),
					name: "SimpleProjectManager",
					displayName: @"Project Manager",
					csSolutionPath: @"eXpressApp Framework\SimpleProjectManager\CS\SimpleProjectManager.sln",
					vbSolutionPath: null,
					shortDescription: @"This demo shows how to quickly build a project management application that runs on Windows and the Web with the common code base. You can get started really fast, because XAF ships with a rich set of typical LOB functionality packed into built-in modules.",
					group: @"Getting Started",
					winPath: @"eXpressApp Framework\SimpleProjectManager\Bin\SimpleProjectManager.Win.exe",
					webPath: @"eXpressApp Framework\SimpleProjectManager\Bin\SimpleProjectManager.Web.ur1",
					buildItPath: @"OpenLink:https://documentation.devexpress.com/#Xaf/CustomDocument3496"
				),
				new FrameworkDemo(product,
					d => new List<Module>(),
					name: "EFDemo",
					displayName: @"Main Demo",
					csSolutionPath: @"eXpressApp Framework\EFDemoCodeFirst\CS\EFDemoCodeFirst.sln",
					vbSolutionPath: null,
					shortDescription: @"This demo illustrates a basic application you can get using XAF and Entity Framework. This application is used to store contacts, tasks, events, reports and other related objects.",
					group: @"Getting Started",
					winPath: @"eXpressApp Framework\EFDemoCodeFirst\Bin\EFDemo.Win.exe",
					webPath: @"eXpressApp Framework\EFDemoCodeFirst\Bin\EFDemo.Web.ur1",
					buildItPath: @"OpenLink:http://documentation.devexpress.com/#Xaf/CustomDocument3639"
				),
				new FrameworkDemo(product,
					d => new List<Module>(),
					name: "FeatureCenter",
					displayName: @"Feature Center",
					csSolutionPath: @"eXpressApp Framework\FeatureCenter\CS\FeatureCenter.sln",
					vbSolutionPath: null,
					shortDescription: @"This demo has been designed to help you quickly get acquainted with XAF and start using it effectively.",
					group: @"Main Features",
					winPath: @"eXpressApp Framework\FeatureCenter\Bin\FeatureCenter.Win.exe",
					webPath: @"eXpressApp Framework\FeatureCenter\Bin\FeatureCenter.Web.ur1",
					buildItPath: @"OpenLink:http://documentation.devexpress.com/#Xaf/CustomDocument2682"
				),
				new FrameworkDemo(product,
					d => new List<Module>(),
					name: "SecurityDemo",
					displayName: @"Security Demo",
					csSolutionPath: @"eXpressApp Framework\SecurityDemo\CS\SecurityDemo.sln",
					vbSolutionPath: null,
					shortDescription: @"This demo illustrates how an XAF application can be secured using a built-in Security System module. Log in as an administrator to set up roles and distribute them between users.",
					group: @"Main Features",
					winPath: @"eXpressApp Framework\SecurityDemo\Bin\SecurityDemo.Win.exe",
					webPath: @"eXpressApp Framework\SecurityDemo\Bin\SecurityDemo.Web.ur1",
					buildItPath: @"OpenLink:http://documentation.devexpress.com/#Xaf/CustomDocument3361"
				),
				new FrameworkDemo(product,
					d => new List<Module>(),
					name: "WorkflowDemo",
					displayName: @"Workflow Demo",
					csSolutionPath: @"eXpressApp Framework\WorkflowDemo\CS\WorkflowDemo.sln",
					vbSolutionPath: null,
					shortDescription: @"This demo illustrates how to integrate Windows Workflow Foundation (WF) support into an XAF application.",
					group: @"Main Features",
					winPath: @"eXpressApp Framework\WorkflowDemo\Bin\WorkflowDemo.Win.exe",
					webPath: null,
					buildItPath: @"OpenLink:http://documentation.devexpress.com/#xaf/CustomDocument3356"
				)
			};
		}
	}
}
