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

using System.ComponentModel;
using DevExpress.Xpo;
using System;
using DevExpress.Workflow.Store;
namespace DevExpress.Workflow.Xpo {	
	[Persistent, System.ComponentModel.DisplayName("Workflow Instance")]
	public class XpoWorkflowInstance : XPObject, IWorkflowInstance {
		public XpoWorkflowInstance(Session session) : base(session) { }
		public string Owner {
			get { return GetPropertyValue<string>("Owner"); }
			set { SetPropertyValue<string>("Owner", value); }
		}
 		public Guid InstanceId {
			get { return GetPropertyValue<Guid>("InstanceId"); }
			set { SetPropertyValue<Guid>("InstanceId", value); }
		}
		public ActivityInstanceState Status {
			get { return GetPropertyValue<ActivityInstanceState>("Status"); }
			set { SetPropertyValue<ActivityInstanceState>("Status", value); }
		}
		[Delayed, Persistent("Content"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Size(-1)]
		public string Content {
			get { return GetDelayedPropertyValue<string>("Content"); }
			set { SetDelayedPropertyValue<string>("Content", value); }
		}
		[Delayed, Persistent("Metadata"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Size(-1)]
		public string Metadata {
			get { return GetDelayedPropertyValue<string>("Metadata"); }
			set { SetDelayedPropertyValue<string>("Metadata", value); }
		}
		public DateTime? ExpirationDateTime {
			get { return GetPropertyValue<DateTime?>("ExpirationDateTime"); }
			set { SetPropertyValue<DateTime?>("ExpirationDateTime", value); }
		}
	}
}
