﻿#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security;
using System.Web.UI;
using DevExpress.DashboardWeb.Mvc;
#if(MVC_FULL_TRUST)
[assembly: AssemblyTitle("DevExpress.Dashboard.Web.Mvc5")]
#else
[assembly: AllowPartiallyTrustedCallers]
[assembly: AssemblyTitle("DevExpress.Dashboard.Web.Mvc")]
#endif
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: AssemblyDescription("Dashboard Suite")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Developer Express Inc.")]
[assembly: AssemblyProduct("Dashboard(tm) SUITE SOFTWARE COMPONENT PRODUCT")]
[assembly: AssemblyCopyright("Copyright (c) 2000-2015 Developer Express Inc.")]
[assembly: AssemblyTrademark("Dashboard(tm) SUITE SOFTWARE COMPONENT PRODUCT")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]
[assembly: Guid("2f797ac9-bb93-4dfa-aa46-b1aaa7fcb4bc")]
[assembly: AssemblyVersion(AssemblyInfo.Version)]
[assembly: AssemblyFileVersion(AssemblyInfo.FileVersion)]
[assembly: SatelliteContractVersion(AssemblyInfo.SatelliteContractVersion)]
#pragma warning disable 1699
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile(@"..\..\..\..\Devexpress.Key\StrongKey.snk")]
[assembly: AssemblyKeyName("")]
#pragma warning restore 1699
[assembly: WebResource(MVCxDashboardViewer.MvcDashboardViewerScriptResourceName, "text/javascript")]
