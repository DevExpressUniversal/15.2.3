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
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	[Flags]
	public enum ModelDocumentSecurity {
		None = 0x0000,
		Password = 0x0001,
		ReadonlyRecommended = 0x0002,
		ReadonlyEnforced = 0x0004,
		Locked = 0x0008
	}
	public class ModelDocumentApplicationProperties : SpreadsheetNotificationOptions {
		#region Fields
		string application;
		string manager;
		string company;
		string version;
		ModelDocumentSecurity security;
		#endregion
		#region Properties
		#region Application
		public string Application {
			get { return application; }
			set {
				if (Application == value)
					return;
				string oldValue = Application;
				this.application = value;
				OnChanged("Application", oldValue, value);
			}
		}
		#endregion
		#region Manager
		public string Manager {
			get { return manager; }
			set {
				if (Manager == value)
					return;
				string oldValue = Manager;
				this.manager = value;
				OnChanged("Manager", oldValue, value);
			}
		}
		#endregion
		#region Company
		public string Company {
			get { return company; }
			set {
				if (Company == value)
					return;
				string oldValue = Company;
				this.company = value;
				OnChanged("Company", oldValue, value);
			}
		}
		#endregion
		#region Version
		public string Version {
			get { return version; }
			set {
				if (Version == value)
					return;
				string oldValue = Version;
				this.version = value;
				OnChanged("Version", oldValue, value);
			}
		}
		#endregion
		#region Security
		public ModelDocumentSecurity Security {
			get { return security; }
			set {
				if (Security == value)
					return;
				ModelDocumentSecurity oldValue = Security;
				this.security = value;
				OnChanged("Security", oldValue, value);
			}
		}
		#endregion
		#endregion
		protected internal override void ResetCore() {
			this.Application = String.Empty;
			this.Manager = String.Empty;
			this.Company = String.Empty;
			this.Version = String.Empty;
			this.Security = ModelDocumentSecurity.None;
		}
		protected internal void CopyFrom(ModelDocumentApplicationProperties value) {
			this.Application = value.Application;
			this.Manager = value.Manager;
			this.Company = value.Company;
			this.Version = value.Version;
			this.Security = value.Security;
		}
		public void CheckAppVersion() {
			if (Application == "Microsoft Excel") {
				int major = 0;
				if (!string.IsNullOrEmpty(Version)) {
					string[] parts = this.Version.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
					if (!Int32.TryParse(parts[0], out major))
						major = 0;
				}
				if (major < 14)
					this.Version = "14.0300";
			}
		}
	}
}
