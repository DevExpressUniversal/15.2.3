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

using System;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.SystemModule {
	[DomainComponent]
	public class AboutInfo {
		private static AboutInfo instance;
		public static AboutInfo Instance {
			get {
				if(instance == null)
					instance = new AboutInfo();
				return instance;
			}
		}
		private string productName = null;
		private string version = null;
		private string copyright = null;
		private string company = null;
		private string logoImageName = null;
		private string description = null;
		private string aboutInfoString = null;
		string modelCompany;
		string modelCopyright;
		string modelDescription;
		string modelLogoImageName;
		string modelProductName;
		string modelVersionFormat;
		string modelAboutInfoString;
		private Version applicationAssemblyVersion;
		public void Init(IModelApplication model, Version applicationAssemblyVersion) {
			Init(model.Company, model.Copyright, model.Description, model.Logo, model.Title, model.VersionFormat, model.AboutInfoString, applicationAssemblyVersion);
		}
		public void Init(string modelCompany, string modelCopyright, string modelDescription, string modelLogoImageName, string modelProductName, string modelVersionFormat, string modelAboutInfoString, Version applicationAssemblyVersion) {
			this.modelCompany = modelCompany;
			this.modelCopyright = modelCopyright;
			this.modelDescription = modelDescription;
			this.modelLogoImageName = modelLogoImageName;
			this.modelProductName = modelProductName;
			this.modelVersionFormat = modelVersionFormat;
			this.modelAboutInfoString = modelAboutInfoString;
			this.applicationAssemblyVersion = applicationAssemblyVersion;
		}
		public void Init(XafApplication application) {
			Init(application.Model, application.GetType().Assembly.GetName().Version);
		}
		public AboutInfo() {
		}
		public string ProductName {
			get {
				if(productName != null) {
					return productName;
				}
				return modelProductName;
			}
			set {
				productName = value;
			}
		}
		public string Version {
			get {
				if(version != null) {
					return version;
				}
				string result = modelVersionFormat;
				if(!string.IsNullOrEmpty(result) && applicationAssemblyVersion != null) {
					result = string.Format(result, applicationAssemblyVersion.Major, applicationAssemblyVersion.Minor,
						applicationAssemblyVersion.Build, applicationAssemblyVersion.Revision);
				}
				return result;
			}
			set {
				version = value;
			}
		}
		public string Copyright {
			get {
				if(copyright != null) {
					return copyright;
				}
				string result = modelCopyright;
				if(!string.IsNullOrEmpty(result)) {
					result = string.Format(new ObjectFormatter(), result, this);
				}
				return result;
			}
			set {
				copyright = value;
			}
		}
		public string Company {
			get {
				if(company != null) {
					return company;
				}
				return modelCompany;
			}
			set {
				company = value;
			}
		}
		public string LogoImageName {
			get {
				if(logoImageName != null) {
					return logoImageName;
				}
				return modelLogoImageName;
			}
			set {
				logoImageName = value;
			}
		}
		public string Description {
			get {
				if(description != null) {
					return description;
				}
				return modelDescription;
			}
			set {
				description = value;
			}
		}
		public string AboutInfoString {
			get {
				if(aboutInfoString != null) {
					return aboutInfoString;
				}
				string result = "";
				if(modelAboutInfoString != null) {
					result = string.Format(new ObjectFormatter(), modelAboutInfoString, this);
					while(result.EndsWith("<br>")) {
						result = result.Substring(0, result.Length - 4);
					}
				}
				return result;
			}
			set {
				aboutInfoString = value;
			}
		}
	}
}
