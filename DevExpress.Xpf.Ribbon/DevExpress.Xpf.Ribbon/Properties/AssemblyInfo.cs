﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
#if DEBUGTEST
using NUnit.Framework;
#endif
[assembly: AssemblyTitle("DevExpress.Xpf.Ribbon")]
[assembly: AssemblyDescription("Xpf.Ribbon Library")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Developer Express Inc.")]
[assembly: AssemblyProduct("DevExpress.Xpf.Ribbon")]
[assembly: AssemblyCopyright("Copyright (c) 2000-2015 Developer Express Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]
#if DEBUGTEST
[assembly: InternalsVisibleTo("DevExpress.Xpf.Navigation.HeavyTests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100dfcd8cadc2dd24a7cd4ce95c4a9c1b8e7cb1dc2d665120556b4b0ec35495fddb2bd6eed0ca1e56480276295a225ba2a9746f3d3e1a04547ccf5b26acc3f96eb2a13ac467512497aa79208e32f242fd0618014d53c95a36e5de0e891873841fa8f559566e38e968426488b4aa4d0f0b59e59f38dcf3fbccf25d990ab19c27ddc2")]
#endif
[assembly: AllowPartiallyTrustedCallers]
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: SatelliteContractVersion(AssemblyInfo.SatelliteContractVersion)]
[assembly: XmlnsPrefix(XmlNamespaceConstants.RibbonNamespaceDefinition, XmlNamespaceConstants.RibbonPrefix)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.RibbonNamespaceDefinition, XmlNamespaceConstants.RibbonNamespace)]
[assembly: XmlnsPrefix(XmlNamespaceConstants.RibbonInternalNamespaceDefinition, XmlNamespaceConstants.RibbonInternalPrefix)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.RibbonInternalNamespaceDefinition, XmlNamespaceConstants.RibbonInternalNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.RibbonThemeKeysNamespaceDefinition, XmlNamespaceConstants.RibbonThemeKeysNamespace)]
[assembly: ComVisible(false)]
[assembly: ThemeInfo(
	ResourceDictionaryLocation.None, 
	ResourceDictionaryLocation.SourceAssembly 
)]
[assembly: AssemblyVersion(AssemblyInfo.Version)]
[assembly: AssemblyFileVersion(AssemblyInfo.FileVersion)]
[assembly: ToolboxTabName(AssemblyInfo.DXTabWpfNavigation)]
#pragma warning disable 1699
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile(@"..\..\..\..\Devexpress.Key\StrongKey.snk")]
[assembly: AssemblyKeyName("")]
#pragma warning restore 1699
