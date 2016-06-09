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
using System.ComponentModel;
using DevExpress.Xpo;
using DevExpress.Persistent.Base.General;
namespace DevExpress.Persistent.BaseImpl {
	[DefaultProperty("Name")]
	public class Country : BaseObject, ICountry {
		private string name;
		private string phoneCode;
		public Country(Session session) : base(session) { }
		public override string ToString() {
			return Name;
		}
		public string Name {
			get { return name; }
			set { SetPropertyValue("Name", ref name, value); }
		}
		public string PhoneCode {
			get { return phoneCode; }
			set { SetPropertyValue("PhoneCode", ref phoneCode, value); }
		}
	}
	[DefaultProperty("LongName")]
	public class State : BaseObject {
		private string shortName = "";
		private string longName = "";
		public State(Session session) : base(session) { }
		public string ShortName {
			get { return shortName; }
			set { SetPropertyValue("ShortName", ref shortName, value); }
		}
		public string LongName {
			get { return longName; }
			set { SetPropertyValue("LongName", ref longName, value); }
		}
	}
}
