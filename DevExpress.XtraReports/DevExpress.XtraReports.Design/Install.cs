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
using Microsoft.Win32;
#if !DXCommon
#endif
namespace DevExpress.XtraReports.Install {
	public class SR {
		public const string
			InheritanceWizardGuid = "0EC60AAB-0929-456f-B8FC-F8E259A361B9",
			WizardDescription = "A class for creating reports using the visual designer",
			WizardUIDescription = "Create new XtraReport using XtraReport Wizard",
			WizardDisplayName = "XtraReport " + AssemblyInfo.VSuffixWithoutSeparator + " Class",
			WizardUIDisplayName = "XtraReport " + AssemblyInfo.VSuffixWithoutSeparator + " Wizard",
			FileName = "XtraReport",
			TemplateFileName = "NewXtraReport",
			DelphiAddinValue = "XtraReports " + AssemblyInfo.VSuffixWithoutSeparator + " Designer",
			DelphiDXAsmFolderKeyName = "DevExpress.Net" + AssemblyInfo.VSuffix,
			XtraReports_vsdir = "XtraReports" + AssemblyInfo.VSuffix + ".vsdir",
			XtraReportsWiz_vsdir = "XtraReportsWiz" + AssemblyInfo.VSuffix + ".vsdir",
			CSharpAddXtraReportWiz = "CSharpAddXtraReportWiz" + AssemblyInfo.VSuffix,
			CSharpAddXtraReportWizUI = "CSharpAddXtraReportWizUI" + AssemblyInfo.VSuffix,
			VBAddXtraReportWiz = "XtraReport" + AssemblyInfo.VSuffix,
			VBAddXtraReportWizUI = "XtraReportUI" + AssemblyInfo.VSuffix,
			VJSharpAddXtraReportWiz = "VJSharpAddXtraReportWiz" + AssemblyInfo.VSuffix,
			VJSharpAddXtraReportWizUI = "VJSharpAddXtraReportWizUI" + AssemblyInfo.VSuffix,
			VCAddXtraReportWiz = "VCAddXtraReportWiz" + AssemblyInfo.VSuffix,
			VCAddXtraReportWizUI = "VCAddXtraReportWizUI" + AssemblyInfo.VSuffix;
		}
}
