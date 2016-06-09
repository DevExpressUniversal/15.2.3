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

using DevExpress.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.DemoData {
	public static class DemoDataSettings {
		static string Version = AssemblyInfo.Version.Remove(AssemblyInfo.Version.LastIndexOf('.'));
		public static string
			GetSupportLink = AssemblyInfo.DXLinkGetSupport, 
			GetStartedLink = AssemblyInfo.DXLinkGetStarted,
			DevExtremeDemosLink = "https://www.devexpress.com/Products/HTML-JS/demos.xml",
			UniversalSubscriptionLink = "https://go.devexpress.com/Demo_UniversalSubscription.aspx",
			SubscriptionsBuyLink = "https://go.devexpress.com/Demo_Subscriptions_Buy.aspx",
			FeaturedDemoNotFoundMessage = "This demo was not installed during Setup. To install, run the DevExpress Unified installer in Modify mode.",
			ShowCasesTitle = "Sample Applications",
			ProductDemosTitle = "Product Demos",
			DevExpressDemoCenterTitle = "DevExpress Demo Center {0}",
			WhatsNewLink = string.Format("http://www.devexpress.com/Support/WhatsNew/DXperience/files/{0}.xml", Version),
			BreakingChangesLink = string.Format("http://www.devexpress.com/Support/WhatsNew/DXperience/files/{0}.bc.xml", Version),
			KnownIssuesLink = string.Format("http://www.devexpress.com/Support/WhatsNew/DXperience/files/{0}.ki.xml", Version);
	}
}
