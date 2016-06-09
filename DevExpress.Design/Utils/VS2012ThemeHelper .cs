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
using EnvDTE;
namespace DevExpress.Utils.Design {
	public class VSThemeHelper {
		const string DarkStyleName = "Visual Studio 2013 Dark";
		const string LightStyleName = "Visual Studio 2013 Light";
		const string BlueStyleName = "Visual Studio 2013 Blue";
		const string VS2010StyleName = "VS2010";
		public static string GetAppropriateSkinName(IServiceProvider serviceProvider) {
			Version version = VSVersions.GetVersion(serviceProvider);
			if(version == VSVersions.VS2010)
				return VS2010StyleName;
			try {
				VSThemeHelper helper = new VSThemeHelper();
				string name = helper.GetCurrentThemeName(serviceProvider);
				return 
					string.Equals(name, "Dark", StringComparison.OrdinalIgnoreCase) ? DarkStyleName :
					string.Equals(name, "Blue", StringComparison.OrdinalIgnoreCase) ? BlueStyleName :
					LightStyleName;
			}
			catch { }
			return LightStyleName;
		}
		[Guid("0D915B59-2ED7-472A-9DE8-9161737EA1C5")]
		interface SVsColorThemeService { }
		string GetCurrentThemeName(IServiceProvider serviceProvider) {
			object themeService = serviceProvider.GetService(typeof(SVsColorThemeService));
			if(themeService == null)
				return null;
			Type themeServiceType = themeService.GetType();
			PropertyInfo propCurrentTheme = themeServiceType.GetProperty("CurrentTheme");
			if(propCurrentTheme == null)
				return null;
			object currentTheme = propCurrentTheme.GetValue(themeService, null);
			if(currentTheme == null)
				return null;
			Type themeType = currentTheme.GetType();
			PropertyInfo propName = themeType.GetProperty("Name");
			if(propName == null)
				return null;
			return propName.GetValue(currentTheme, null) as string;
		}
	}
}
