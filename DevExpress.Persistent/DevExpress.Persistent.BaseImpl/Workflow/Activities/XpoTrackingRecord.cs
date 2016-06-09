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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;
namespace DevExpress.Workflow.Xpo {
	[Persistent, System.ComponentModel.DisplayName("Tracking Record")]
	public class XpoTrackingRecord : XPObject, ITrackingRecord {
		public XpoTrackingRecord(Session session) : base(session) { }
		[VisibleInListView(false)]
		public Guid InstanceId {
			get { return GetPropertyValue<Guid>("InstanceId"); }
			set { SetPropertyValue<Guid>("InstanceId", value); }
		}
		public DateTime DateTime {
			get { return GetPropertyValue<DateTime>("DateTime"); }
			set { SetPropertyValue<DateTime>("DateTime", value); }
		}
		[Size(SizeAttribute.Unlimited)]
		public string Data {
			get { return GetPropertyValue<string>("Data"); }
			set { SetPropertyValue<string>("Data", value); }
		}
		public string ActivityId {
			get { return GetPropertyValue<string>("ActivityId"); }
			set { SetPropertyValue<string>("ActivityId", value); }
		}
	}
}
