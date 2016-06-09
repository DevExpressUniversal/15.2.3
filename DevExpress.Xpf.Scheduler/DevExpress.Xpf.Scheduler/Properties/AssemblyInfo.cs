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

using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System;
using System.Security;
using System.Windows.Markup;
using DevExpress.Xpf.Core.Native;
using DevExpress.Utils;
#if !SL
#endif
[assembly: AssemblyTitle("DevExpress.Xpf.Scheduler")]
[assembly: AssemblyDescription("DXScheduler Suite")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Developer Express Inc.")]
[assembly: AssemblyProduct("DXScheduler SUITE SOFTWARE COMPONENT PRODUCT")]
[assembly: AssemblyCopyright("Copyright (c) 2000-2015 Developer Express Inc.")]
[assembly: AssemblyTrademark("DXScheduler Suite")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]
#if !SL
[assembly: AllowPartiallyTrustedCallers]
#endif
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: SatelliteContractVersion(AssemblyInfo.SatelliteContractVersion)]
[assembly: ComVisible(false)]
[assembly: XmlnsPrefix(XmlNamespaceConstants.SchedulerNamespaceDefinition, XmlNamespaceConstants.SchedulerPrefix)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.SchedulerNamespaceDefinition, XmlNamespaceConstants.SchedulerNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.SchedulerNamespaceDefinition, XmlNamespaceConstants.SchedulerUINamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.SchedulerNamespaceDefinition, XmlNamespaceConstants.SchedulerCommandsNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.SchedulerInternalNamespaceDefinition, XmlNamespaceConstants.SchedulerDrawingNamespace)]
#if WPF
[assembly: XmlnsDefinition(XmlNamespaceConstants.SchedulerThemeKeysNamespaceDefinition, XmlNamespaceConstants.SchedulerThemeKeysNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.SchedulerNamespaceDefinition, XmlNamespaceConstants.SchedulerReportingNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.SchedulerInternalNamespaceDefinition, XmlNamespaceConstants.SchedulerReportingUINamespace)]
#endif
#if !SL
[assembly: ThemeInfo(
	ResourceDictionaryLocation.None, 
	ResourceDictionaryLocation.SourceAssembly 
)]
#endif
[assembly: AssemblyVersion(AssemblyInfo.Version)]
[assembly: AssemblyFileVersion(AssemblyInfo.FileVersion)]
[assembly: ToolboxTabName(AssemblyInfo.DXTabWpfScheduling)]
#if DEBUGTEST
#if !SL
[assembly: InternalsVisibleTo("DevExpress.Xpf.Scheduler.HeavyTests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100dfcd8cadc2dd24a7cd4ce95c4a9c1b8e7cb1dc2d665120556b4b0ec35495fddb2bd6eed0ca1e56480276295a225ba2a9746f3d3e1a04547ccf5b26acc3f96eb2a13ac467512497aa79208e32f242fd0618014d53c95a36e5de0e891873841fa8f559566e38e968426488b4aa4d0f0b59e59f38dcf3fbccf25d990ab19c27ddc2")]
#else
[assembly: InternalsVisibleTo("DevExpress.Xpf.Scheduler.HeavyTests.SL, PublicKey=0024000004800000940000000602000000240000525341310004000001000100dfcd8cadc2dd24a7cd4ce95c4a9c1b8e7cb1dc2d665120556b4b0ec35495fddb2bd6eed0ca1e56480276295a225ba2a9746f3d3e1a04547ccf5b26acc3f96eb2a13ac467512497aa79208e32f242fd0618014d53c95a36e5de0e891873841fa8f559566e38e968426488b4aa4d0f0b59e59f38dcf3fbccf25d990ab19c27ddc2")]
#endif
#endif
