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

using System.Windows;
using System.Windows.Markup;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DevExpress.Xpf.Core.Native;
using DevExpress.Internal;
#if !SL
using System.Security;
#endif
#if !SL
[assembly: System.Security.SecurityRules(System.Security.SecurityRuleSet.Level1)]
#endif
[assembly: AssemblyTitle("DevExpress.Xpf.Controls")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(AssemblyInfo.AssemblyCompany)]
[assembly: AssemblyProduct("DevExpress.Xpf.Controls")]
[assembly: AssemblyCopyright("Copyright (c) 2000-2015 Developer Express Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("2a326bc7-e363-49e0-a00f-4bb496caf984")]
[assembly: System.Resources.NeutralResourcesLanguage("en-US")]
[assembly: System.Resources.SatelliteContractVersion(AssemblyInfo.SatelliteContractVersion)]
[assembly: XmlnsPrefix(XmlNamespaceConstants.ControlsNamespaceDefinition, XmlNamespaceConstants.ControlsPrefix)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.ControlsNamespaceDefinition, XmlNamespaceConstants.ControlsNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.ControlsThemeKeysNamespaceDefinition, XmlNamespaceConstants.ControlsThemeKeysNamespace)]
#if !SL
[assembly: XmlnsPrefix(XmlNamespaceConstants.NavigationNamespaceDefinition, XmlNamespaceConstants.NavigationPrefix)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.NavigationNamespaceDefinition, XmlNamespaceConstants.NavigationNamespace)]
[assembly: XmlnsPrefix(XmlNamespaceConstants.NavigationInternalNamespaceDefinition, XmlNamespaceConstants.NavigationInternalPrefix)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.NavigationInternalNamespaceDefinition, XmlNamespaceConstants.NavigationInternalNamespace)]
#endif
[assembly: XmlnsPrefix(XmlNamespaceConstants.WindowsUINamespaceDefinition, XmlNamespaceConstants.WindowsUIPrefix)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.WindowsUINamespaceDefinition, XmlNamespaceConstants.WindowsUINamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.WindowsUIThemeKeysNamespaceDefinition, XmlNamespaceConstants.WindowsUIThemeKeysNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.WindowsUIInternalNamespaceDefinition, XmlNamespaceConstants.WindowsUIInternalNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.WindowsUINavigationNamespaceDefinition, XmlNamespaceConstants.WindowsUINavigationNamespace)]
#if DEBUGTEST
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DevExpress.Xpf.Navigation.HeavyTests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100dd3415ad127e2479d518586804419e99231acd687f889e897fb021bec3d90d53781811bb9d569e032d00362413298930c553dfd43a24e699c6a3d4922824f3c987fc01524b94059de1ccfbef1ff6aedc86055d56c4c3c92c550c84a1410b0c0e891e8f2f0fa193e1532f25727ae634055808129b901bdc24cb517e95fb8815b5")]
#endif
#if !SL
[assembly: AllowPartiallyTrustedCallers]
[assembly: ThemeInfo(
	ResourceDictionaryLocation.None, 
	ResourceDictionaryLocation.SourceAssembly 
)]
#endif
[assembly: System.CLSCompliant(true)]
[assembly: AssemblyVersion(AssemblyInfo.Version)]
[assembly: AssemblyFileVersion(AssemblyInfo.FileVersion)]
