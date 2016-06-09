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
using System.Resources;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Internal;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
#if !SL
#endif
[assembly: AssemblyTitle("DevExpress.Xpf.RichEdit")]
[assembly: AssemblyDescription("DXRichEdit")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(AssemblyInfo.AssemblyCompany)]
[assembly: AssemblyProduct("DevExpress.DXRichEdit")]
[assembly: AssemblyCopyright("Copyright (c) 2000-2015 Developer Express Inc.")]
[assembly: AssemblyTrademark("DXRichEdit")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]
[assembly: AllowPartiallyTrustedCallers]
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: SatelliteContractVersion(AssemblyInfo.SatelliteContractVersion)]
[assembly: XmlnsPrefix(XmlNamespaceConstants.RichEditNamespaceDefinition, XmlNamespaceConstants.RichEditPrefix)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.RichEditNamespaceDefinition, XmlNamespaceConstants.RichEditNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.RichEditNamespaceDefinition, XmlNamespaceConstants.RichEditXpfNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.RichEditNamespaceDefinition, XmlNamespaceConstants.RichEditUINamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.RichEditNamespaceDefinition, XmlNamespaceConstants.RichEditMenuNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.RichEditInternalNamespaceDefinition, XmlNamespaceConstants.RichEditInternalControlsNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.RichEditThemeKeysNamespaceDefinition, XmlNamespaceConstants.RichEditThemeKeysNamespace)]
[assembly: Guid("94e9b4c8-30d4-44e4-9413-272e2f5e2281")]
[assembly: ThemeInfo(
	ResourceDictionaryLocation.None, 
	ResourceDictionaryLocation.SourceAssembly 
)]
#if !SL
[assembly: ToolboxTabName(AssemblyInfo.DXTabWpfCommon)]
#endif
[assembly: AssemblyVersion(AssemblyInfo.Version)]
[assembly: AssemblyFileVersion(AssemblyInfo.FileVersion)]
