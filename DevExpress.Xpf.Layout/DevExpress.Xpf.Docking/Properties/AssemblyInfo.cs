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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core.Native;
using DevExpress.Utils;
using System.Resources;
#if DEBUGTEST
using NUnit.Framework;
#endif
[assembly: AssemblyTitle("DevExpress.Xpf.Docking")]
[assembly: AssemblyDescription("Docking Suite")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Developer Express Inc.")]
[assembly: AssemblyProduct("DXDocking SUITE SOFTWARE COMPONENT PRODUCT")]
[assembly: AssemblyCopyright("Copyright (c) 2000-2015 Developer Express Inc.")]
[assembly: AssemblyTrademark("DXDocking Suite")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]
[assembly: AllowPartiallyTrustedCallers]
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: SatelliteContractVersion(AssemblyInfo.SatelliteContractVersion)]
[assembly: XmlnsPrefix(XmlNamespaceConstants.DockingNamespaceDefinition, XmlNamespaceConstants.DockingPrefix)]
[assembly: XmlnsPrefix(XmlNamespaceConstants.DockingVisualElementsNamespaceDefinition, XmlNamespaceConstants.DockingVisualElementsPrefix)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.DockingNamespaceDefinition, XmlNamespaceConstants.DockingNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.DockingThemeKeysNamespaceDefinition, XmlNamespaceConstants.DockingThemeKeysNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.DockingPlatformNamespaceDefinition, XmlNamespaceConstants.DockingPlatformNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.DockingVisualElementsNamespaceDefinition, XmlNamespaceConstants.DockingVisualElementsNamespace)]
#if DEBUGTEST
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DevExpress.Xpf.Navigation.HeavyTests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100dd3415ad127e2479d518586804419e99231acd687f889e897fb021bec3d90d53781811bb9d569e032d00362413298930c553dfd43a24e699c6a3d4922824f3c987fc01524b94059de1ccfbef1ff6aedc86055d56c4c3c92c550c84a1410b0c0e891e8f2f0fa193e1532f25727ae634055808129b901bdc24cb517e95fb8815b5")]
#endif
[assembly: ComVisible(false)]
[assembly: ThemeInfo(
	ResourceDictionaryLocation.None, 
	ResourceDictionaryLocation.SourceAssembly 
)]
[assembly: AssemblyVersion(AssemblyInfo.Version)]
[assembly: AssemblyFileVersion(AssemblyInfo.FileVersion)] 
[assembly: ToolboxTabName(AssemblyInfo.DXTabWpfNavigation)]
#if DXBUILD2010
#if DEBUGTEST
#endif
#endif
