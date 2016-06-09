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

using DevExpress.DemoData.Core;
using DevExpress.DemoData.Helpers;
using DevExpress.DemoData.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.DemoData.Model {
	public enum RequirementCheckResultType {
		Satisfied, Error
	}
	public class RequirementCheckResult {
		public RequirementCheckResult(string message, RequirementCheckResultType type) {
			Message = message;
			Type = type;
		}
		static RequirementCheckResult() {
			Satisfied = new RequirementCheckResult("", RequirementCheckResultType.Satisfied);
		}
		public string Message { get; private set; }
		public RequirementCheckResultType Type { get; private set; }
		public static RequirementCheckResult Satisfied { get; private set; }
	}
	public abstract class Requirement {
		Lazy<RequirementCheckResult> result;
		public Requirement() {
			result = new Lazy<RequirementCheckResult>(Check);
		}
		public RequirementCheckResult GetResult() { return result.Value; }
		protected abstract RequirementCheckResult Check();
	}
	public class SqlServerRequirement : Requirement {
		const string message = @"
            <Paragraph TextAlignment=""Center"" FontSize=""12"">
            To run this demo, your computer should have SQL Express installed. You can download it from the official Microsoft site:<LineBreak/><Hyperlink Foreground=""Black"" NavigateUri=""http://www.microsoft.com/express/sql/"" xml:space=""preserve"">http://www.microsoft.com/</Hyperlink>.
            </Paragraph>";
		protected override RequirementCheckResult Check() {
			if(DbEngineDetector.IsSqlExpressInstalled || DbEngineDetector.LocalDbVersion != null)
				return RequirementCheckResult.Satisfied;
			return new RequirementCheckResult(message, RequirementCheckResultType.Error);
		}
	}
	public class DeveloperServerRequirement : Requirement {
		const string message = @"
            <Paragraph TextAlignment=""Center"" FontSize=""12"">
            Thank you for evaluating our {2} product line.
            <LineBreak/><LineBreak/>
            The {2} demos included in this distribution require a local web server but we were unable to detect one installed on this machine. To execute our demos locally, we recommend that you install Visual Studio. You can download Visual Studio here:
            <LineBreak/>
            <Hyperlink Foreground=""Black"" NavigateUri=""http://www.visualstudio.com/en-us/downloads"" xml:space=""preserve"">http://www.visualstudio.com/en-us/downloads</Hyperlink>
            <LineBreak/><LineBreak/>
            Once Visual Studio is installed, please re-install our distribution to make certain that our products are registered inside Visual Studio.
            <LineBreak/><LineBreak/>
            If you are unable to install Visual Studio, you can view all of our ASP.NET demos online by visiting:
            <LineBreak/>
            <Hyperlink Foreground=""Black"" NavigateUri=""{1}://demos.devexpress.com/{0}"" xml:space=""preserve"">{1}://demos.devexpress.com/{0}</Hyperlink>
            <LineBreak/><LineBreak/>
            We are here to help. Should you have any questions or require further assistance, feel free to contact us by Email at:
            <Hyperlink Foreground=""Black"" NavigateUri=""mailto:support@devexpress.com"" xml:space=""preserve"">support@devexpress.com</Hyperlink>
            </Paragraph>";
		string demosName;
		string url;
		public DeveloperServerRequirement(string demosName, string url) {
			this.demosName = demosName;
			this.url = url;
		}
		protected override RequirementCheckResult Check() {
			if (!WebDevServerHelper.WebDevWebServerInstalled() && !WebDevServerHelper.IISExpressInstalled()) {
				var formattedMessage = string.Format(message, url, "https", demosName);
				return new RequirementCheckResult(formattedMessage, RequirementCheckResultType.Error);
			}
			return RequirementCheckResult.Satisfied;
		}
	}
	public class Windows8Requirement : Requirement {
		const string message = @"
            <Paragraph TextAlignment=""Center"" FontSize=""12"">
            This demo can be run only under Windows 8
            </Paragraph>
        ";
		protected override RequirementCheckResult Check() {
			var badResult = new RequirementCheckResult(message, RequirementCheckResultType.Error);
			if(Environment.OSVersion.Version.Major > 6) return RequirementCheckResult.Satisfied;
			if(Environment.OSVersion.Version.Major < 6) return badResult;
			return Environment.OSVersion.Version.Minor >= 2 ? RequirementCheckResult.Satisfied : badResult;
		}
	}
	public class WinformsRequirement : Requirement {
		const string message = @"
            <Paragraph TextAlignment=""Center"" FontSize=""12"">
            This demo requires the DevExpress WinForms controls to be installed. Please re-run the setup and install the WinForms controls.
            </Paragraph>";
		protected override RequirementCheckResult Check() {
			if(Linker.IsProductInstalled("Windows Forms"))
				return RequirementCheckResult.Satisfied;
			return new RequirementCheckResult(message, RequirementCheckResultType.Error);
		}
	}
	public class MvcRequirement : Requirement {
		const string message = @"
            <Paragraph TextAlignment=""Center"" FontSize=""12"">
            To run this demo, your computer should have ASP.NET MVC version 3 or 4 installed. Download the required framework from the official Microsoft site:<LineBreak/><Hyperlink Foreground=""Black"" NavigateUri=""http://www.asp.net/mvc/mvc3"" xml:space=""preserve"">http://www.asp.net/downloads[/Hyperlink].
            </Paragraph>";
		const string Key = "Microsoft/ASP.NET MVC {0}";
		string url;
		public MvcRequirement(string url = "") {
			this.url = url;
		}
		protected override RequirementCheckResult Check() {
			string mvc3RegistryPath = string.Format(Key, 3);
			string mvc4RegistryPath = string.Format(Key, 4);
			if(RegistryViewer.Current.IsKeyExists(RegistryHive.LocalMachine, AssemblyHelper.CombineUri("Software", mvc3RegistryPath)))
				return RequirementCheckResult.Satisfied;
			if(RegistryViewer.Current.IsKeyExists(RegistryHive.LocalMachine, AssemblyHelper.CombineUri("Software", mvc4RegistryPath)))
				return RequirementCheckResult.Satisfied;
			return new RequirementCheckResult(message, RequirementCheckResultType.Error);
		}
	}
}
