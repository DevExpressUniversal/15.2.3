#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
[assembly: AssemblyTitle("DevExpress.ExpressApp")]
[assembly: AssemblyDescription("eXpressApp Framework core components")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyProduct("eXpressApp Framework")]
namespace DevExpress.ExpressApp {
	public class XafAssemblyInfo {
		public const string
			AssemblyCompany = AssemblyInfo.AssemblyCompany,
			AssemblyCopyright = AssemblyInfo.AssemblyCompany,
			VersionSuffix = AssemblyInfo.VSuffix;
		public const string Version = AssemblyInfo.Version;
		public const string AssemblyNamePostfix = AssemblyInfo.FullAssemblyVersionExtension;
		public const string PublicKeyToken =
#if DEBUG
 "79868b8147b5eae4";
#else
 "b88d1754d700e49a";		
#endif
		public const string DXExperienceVersion = AssemblyInfo.Version;
		public const string DXExperiencePublicKeyToken = "b88d1754d700e49a";
		public const string DXTabXafActions = "DX." + AssemblyInfo.VersionShort + ": XAF Actions";
		public const string DXTabXafActionContainers = "DX." + AssemblyInfo.VersionShort + ": XAF Action Containers";
		public const string DXTabXafTemplates = "DX." + AssemblyInfo.VersionShort + ": XAF Templates";
		public const string DXTabXafSecurity = "DX." + AssemblyInfo.VersionShort + ": XAF Security";
		public const string DXTabXafModules = "DX." + AssemblyInfo.VersionShort + ": XAF Modules";
		public const string DXTabXafReports = "DX." + AssemblyInfo.VersionShort + ": XAF Data Sources for Reports";
		public const string DXTabXafWebDataSource = "DX." + AssemblyInfo.VersionShort + ": XAF Web Data Sources";
	}
}
