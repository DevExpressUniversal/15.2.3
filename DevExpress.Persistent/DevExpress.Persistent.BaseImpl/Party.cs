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
using System.Drawing;
using System.ComponentModel;
using DevExpress.Xpo.Metadata;
using DevExpress.Persistent.Base;
namespace DevExpress.Persistent.BaseImpl {
	[MapInheritance(MapInheritanceType.OwnTable)]
	[DefaultProperty("DisplayName")]
	public abstract class Party : BaseObject {
		private Address address1;
		private Address address2;
		protected Party(Session session) : base(session) { }
		public override string ToString() {
			return DisplayName;
		}
		[Size(SizeAttribute.Unlimited), Delayed(true)]
		[ImageEditor]
		public byte[] Photo {
			get { return GetDelayedPropertyValue<byte[]>("Photo"); }
			set { SetDelayedPropertyValue<byte[]>("Photo", value); }
		}
		[Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
		public Address Address1 {
			get { return address1; }
			set { SetPropertyValue("Address1", ref address1, value); }
		}
		[Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
		public Address Address2 {
			get { return address2; }
			set { SetPropertyValue("Address2", ref address2, value); }
		}
		[ObjectValidatorIgnoreIssue(typeof(ObjectValidatorDefaultPropertyIsVirtual), typeof(ObjectValidatorDefaultPropertyIsNonPersistentNorAliased))] 
		public abstract string DisplayName {
			get;
		}
		[Aggregated, Association("Party-PhoneNumbers")]
		public XPCollection<PhoneNumber> PhoneNumbers {
			get { return GetCollection<PhoneNumber>("PhoneNumbers"); }
		}
	}
}
