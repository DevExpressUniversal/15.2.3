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

using DevExpress.Compatibility.System.ComponentModel;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
namespace DevExpress.XtraRichEdit {
	#region AuthenticationOptions
	[ComVisible(true)]
	public class AuthenticationOptions : RichEditNotificationOptions {
		#region Fields
		string userName;
		string group;
		string eMail;
		string password;
		#endregion
		#region Properties
		#region UserName
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("AuthenticationOptionsUserName"),
#endif
DefaultValue("")]
		public string UserName {
			get { return userName; }
			set {
				if (userName == value)
					return;
				string oldValue = UserName;
				userName = value;
				OnChanged("UserName", oldValue, value);
			}
		}
		#endregion
		#region Group
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("AuthenticationOptionsGroup"),
#endif
DefaultValue("")]
		public string Group {
			get { return group; }
			set {
				if (group == value)
					return;
				string oldValue = Group;
				group = value;
				OnChanged("Group", oldValue, value);
			}
		}
		#endregion
		#region EMail
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("AuthenticationOptionsEMail"),
#endif
DefaultValue("")]
		public string EMail {
			get { return eMail; }
			set {
				if (eMail == value)
					return;
				string oldValue = EMail;
				eMail = value;
				OnChanged("EMail", oldValue, value);
			}
		}
		#endregion
		#region Password
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Password {
			get { return password; }
			set {
				if (password == value)
					return;
				string oldValue = Password;
				password = value;
				OnChanged("Password", oldValue, value);
			}
		}
		#endregion
		#endregion
		protected internal override void ResetCore() {
			UserName = String.Empty;
			Group = String.Empty;
			EMail = String.Empty;
			Password = String.Empty;
		}
	}
	#endregion
}
