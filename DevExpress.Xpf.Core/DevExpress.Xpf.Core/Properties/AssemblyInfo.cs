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

using System.Windows.Markup;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Resources;
#if !SL
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System;
using System.Security;
using DevExpress.Utils;
#if DEBUGTEST
using NUnit.Framework;
#endif
#else
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using DevExpress.Internal;
using System.Runtime.CompilerServices;
#endif
#if !SL
#endif
[assembly: AssemblyTitle("DevExpress.Xpf.Core")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(AssemblyInfo.AssemblyCompany)]
[assembly: AssemblyProduct("DevExpress.Xpf.Core")]
[assembly: AssemblyCopyright("Copyright (c) 2000-2015 Developer Express Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]
#if !SL
[assembly: AllowPartiallyTrustedCallers]
[assembly: XmlnsDefinition(XmlNamespaceConstants.UtilsNamespaceDefinition, XmlNamespaceConstants.ThemesNamespace)]
#endif
[assembly: SatelliteContractVersion(AssemblyInfo.SatelliteContractVersion)]
[assembly: XmlnsPrefix(XmlNamespaceConstants.UtilsNamespaceDefinition, XmlNamespaceConstants.UtilsPrefix)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.UtilsNamespaceDefinition, XmlNamespaceConstants.UtilsNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.UtilsInternalNamespaceDefinition, XmlNamespaceConstants.UtilsNativeNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.UtilsNamespaceDefinition, XmlNamespaceConstants.ServerModeNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.UtilsNamespaceDefinition, XmlNamespaceConstants.DataSourcesNamespace)]
#if !SL
[assembly: XmlnsDefinition(XmlNamespaceConstants.UtilsThemeKeysNamespaceDefinition, XmlNamespaceConstants.UtilsThemesNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.EditorsNamespaceDefinition, XmlNamespaceConstants.EditorsFlyoutNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.EditorsInternalNamespaceDefinition, XmlNamespaceConstants.EditorsFlyoutInternalNamespace)]
#endif
[assembly: XmlnsDefinition(XmlNamespaceConstants.UtilsNamespaceDefinition, XmlNamespaceConstants.CoreNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.UtilsNamespaceDefinition, XmlNamespaceConstants.SerializationNamespace)]
[assembly: XmlnsPrefix(XmlNamespaceConstants.PrintingNamespaceDefinition, XmlNamespaceConstants.PrintingPrefix)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.PrintingNamespaceDefinition, XmlNamespaceConstants.PrintingNamespace)]
[assembly: XmlnsPrefix(XmlNamespaceConstants.EditorsNamespaceDefinition, XmlNamespaceConstants.EditorsPrefix)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.EditorsNamespaceDefinition, XmlNamespaceConstants.EditorsNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.EditorsNamespaceDefinition, XmlNamespaceConstants.EditorsFilteringNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.EditorsNamespaceDefinition, XmlNamespaceConstants.EditorsExpressionEditorNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.EditorsNamespaceDefinition, XmlNamespaceConstants.EditorsDateNavigatorNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.EditorsNamespaceDefinition, XmlNamespaceConstants.EditorsDataPagerNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.EditorsInternalNamespaceDefinition, XmlNamespaceConstants.EditorsHelpersNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.EditorsInternalNamespaceDefinition, XmlNamespaceConstants.EditorsInternalNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.EditorsInternalNamespaceDefinition, XmlNamespaceConstants.EditorsDateNavigatorControlsNamespace)]
#if !SL
[assembly: XmlnsDefinition(XmlNamespaceConstants.EditorsNamespaceDefinition, XmlNamespaceConstants.EditorsSettingsExtensionNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.EditorsThemeKeysNamespaceDefinition, XmlNamespaceConstants.EditorsThemesNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.EditorsInternalNamespaceDefinition, XmlNamespaceConstants.EditorsRangeControlInternalNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.EditorsNamespaceDefinition, XmlNamespaceConstants.EditorsRangeControlNamespace)]
#endif
[assembly: XmlnsDefinition(XmlNamespaceConstants.EditorsNamespaceDefinition, XmlNamespaceConstants.EditorsSettingsNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.EditorsNamespaceDefinition, XmlNamespaceConstants.EditorsValidationNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.EditorsNamespaceDefinition, XmlNamespaceConstants.EditorsPopupsNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.EditorsNamespaceDefinition, XmlNamespaceConstants.EditorsCalendarNamespace)]
[assembly: XmlnsPrefix(XmlNamespaceConstants.BarsNamespaceDefinition, XmlNamespaceConstants.BarsPrefix)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.BarsNamespaceDefinition, XmlNamespaceConstants.BarsNamespace)]
#if !SL
[assembly: XmlnsDefinition(XmlNamespaceConstants.BarsThemeKeysNamespaceDefinition, XmlNamespaceConstants.BarsThemeKeysNamespace)]
#endif
[assembly: XmlnsDefinition(XmlNamespaceConstants.BarsInternalNamespaceDefinition, XmlNamespaceConstants.BarsHelpersNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.BarsInternalNamespaceDefinition, XmlNamespaceConstants.BarsCustomizationNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.RichEditNamespaceDefinition, XmlNamespaceConstants.OfficeUINamespace)]
[assembly: XmlnsPrefix(XmlNamespaceConstants.OfficeNamespaceDefinition, XmlNamespaceConstants.OfficePrefix)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.OfficeNamespaceDefinition, XmlNamespaceConstants.OfficeUINamespace)]
[assembly: XmlnsPrefix(XmlNamespaceConstants.MvvmNamespaceDefinition, XmlNamespaceConstants.MvvmPrefix)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.MvvmNamespaceDefinition, XmlNamespaceConstants.MvvmUINamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.MvvmNamespaceDefinition, XmlNamespaceConstants.MvvmInteractivityNamespace)]
[assembly: XmlnsPrefix(XmlNamespaceConstants.MvvmInternalNamespaceDefinition, XmlNamespaceConstants.MvvmIntenalPrefix)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.MvvmInternalNamespaceDefinition, XmlNamespaceConstants.MvvmInteractivityInternalNamespace)]
[assembly: XmlnsPrefix(XmlNamespaceConstants.WizardFrameworkNamespaceDefinition, XmlNamespaceConstants.WizardFrameworkPrefix)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.WizardFrameworkNamespaceDefinition, XmlNamespaceConstants.WizardFrameworkNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.UtilsNamespaceDefinition, XmlNamespaceConstants.ConditionalFormattingNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.UtilsInternalNamespaceDefinition, XmlNamespaceConstants.ConditionalFormattingNativeNamespace)]
[assembly: XmlnsDefinition(XmlNamespaceConstants.UtilsThemeKeysNamespaceDefinition, XmlNamespaceConstants.ConditionalFormattingThemeKeysNamespace)]
[assembly: ComVisible(false)]
[assembly: NeutralResourcesLanguage("en-US")]
#if !SL
[assembly: ThemeInfo(
	ResourceDictionaryLocation.None, 
	ResourceDictionaryLocation.SourceAssembly 
)]
#endif
[assembly: AssemblyVersion(AssemblyInfo.Version)]
[assembly: AssemblyFileVersion(AssemblyInfo.FileVersion)]
#if !SL
[assembly: ToolboxTabName(AssemblyInfo.DXTabWpfCommon)]
#endif
#if DEBUGTEST
[assembly: InternalsVisibleTo("DevExpress.Xpf.Navigation.HeavyTests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100dfcd8cadc2dd24a7cd4ce95c4a9c1b8e7cb1dc2d665120556b4b0ec35495fddb2bd6eed0ca1e56480276295a225ba2a9746f3d3e1a04547ccf5b26acc3f96eb2a13ac467512497aa79208e32f242fd0618014d53c95a36e5de0e891873841fa8f559566e38e968426488b4aa4d0f0b59e59f38dcf3fbccf25d990ab19c27ddc2")]
[assembly: InternalsVisibleTo("DevExpress.Xpf.Core.HeavyTests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100dfcd8cadc2dd24a7cd4ce95c4a9c1b8e7cb1dc2d665120556b4b0ec35495fddb2bd6eed0ca1e56480276295a225ba2a9746f3d3e1a04547ccf5b26acc3f96eb2a13ac467512497aa79208e32f242fd0618014d53c95a36e5de0e891873841fa8f559566e38e968426488b4aa4d0f0b59e59f38dcf3fbccf25d990ab19c27ddc2")]
[assembly: InternalsVisibleTo("DevExpress.Mvvm.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100dfcd8cadc2dd24a7cd4ce95c4a9c1b8e7cb1dc2d665120556b4b0ec35495fddb2bd6eed0ca1e56480276295a225ba2a9746f3d3e1a04547ccf5b26acc3f96eb2a13ac467512497aa79208e32f242fd0618014d53c95a36e5de0e891873841fa8f559566e38e968426488b4aa4d0f0b59e59f38dcf3fbccf25d990ab19c27ddc2")]
#endif
