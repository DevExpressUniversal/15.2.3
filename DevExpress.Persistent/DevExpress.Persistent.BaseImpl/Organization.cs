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
using DevExpress.Xpo;
using DevExpress.Persistent.Base;
namespace DevExpress.Persistent.BaseImpl {
	public class Organization : Party {
		string fullName = "";
		string profile = "";
		string email = "";
		string webSite = "";
		string description = "";
		string name = "";
		public Organization(Session session) : base(session) { }
		public string FullName {
			get { return fullName; }
			set { SetPropertyValue("FullName", ref fullName, value); }
		}
		public string Profile {
			get { return profile; }
			set { SetPropertyValue("Profile", ref profile, value); }
		}
		public string Email {
			get { return email; }
			set { SetPropertyValue("Email", ref email, value); }
		}
		public string WebSite {
			get { return webSite; }
			set { SetPropertyValue("WebSite", ref webSite, value); }
		}
		[Size(4096), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
		public string Description {
			get { return description; }
			set { SetPropertyValue("Description", ref description, value); }
		}
		public string Name {
			get { return name; }
			set { SetPropertyValue("Name", ref name, value); }
		}
		[ObjectValidatorIgnoreIssue(typeof(ObjectValidatorDefaultPropertyIsVirtual))] 
		public override string DisplayName {
			get { return Name; }
		}
	}
}
