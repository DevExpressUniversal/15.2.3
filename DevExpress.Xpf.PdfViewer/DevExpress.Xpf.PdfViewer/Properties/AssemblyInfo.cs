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

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows;
using System;
using System.Security;
using System.Windows.Markup;
using DevExpress.Xpf.Core.Native;
using DevExpress.Utils;
#if !SL
#endif
[assembly: AssemblyTitle("DevExpress.Xpf.PdfViewer")]
[assembly: AssemblyDescription("DXPdfViewer Suite")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(AssemblyInfo.AssemblyCompany)]
[assembly: AssemblyProduct("DevExpress.Xpf.PdfViewer")]
[assembly: AssemblyCopyright("Copyright (c) 2000-2015 Developer Express Inc.")]
[assembly: AssemblyTrademark("DXPdfViewer Suite")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]
#if !SILVERLIGHT
[assembly: AllowPartiallyTrustedCallers]
#endif
[assembly: SatelliteContractVersion(AssemblyInfo.SatelliteContractVersion)]
[assembly: XmlnsPrefix(XmlNamespaceConstants.PdfViewerNamespaceDefinition, XmlNamespaceConstants.PdfViewerPrefix)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.PdfViewerNamespaceDefinition, XmlNamespaceConstants.PdfViewerNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.PdfViewerThemeKeysNamespaceDefinition, XmlNamespaceConstants.PdfViewerThemesNamespace)]
[assembly: XmlnsPrefix(XmlNamespaceConstants.PdfViewerInternalNamespaceDefinition, XmlNamespaceConstants.PdfViewerInternalPrefix)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.PdfViewerInternalNamespaceDefinition, XmlNamespaceConstants.PdfViewerInternalNamespace)]
[assembly: ComVisible(false)]
[assembly: NeutralResourcesLanguage("en-US")]
#if !SILVERLIGHT
[assembly: ThemeInfo(
	ResourceDictionaryLocation.None, 
	ResourceDictionaryLocation.SourceAssembly 
)]
#endif
[assembly: AssemblyVersion(AssemblyInfo.Version)]
[assembly: AssemblyFileVersion(AssemblyInfo.FileVersion)] 
[assembly: ToolboxTabName(AssemblyInfo.DXTabWpfData)]
#if !SL
#pragma warning disable 1699
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile(@"..\..\..\..\Devexpress.Key\StrongKey.snk")]
[assembly: AssemblyKeyName("")]
#pragma warning restore 1699
#endif
