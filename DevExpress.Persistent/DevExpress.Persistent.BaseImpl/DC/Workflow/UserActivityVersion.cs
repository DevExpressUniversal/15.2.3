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
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
namespace DevExpress.ExpressApp.Workflow.Versioning {
	[DomainComponent, XafDisplayName("User Activity Version")]
	public interface IUserActivityVersion : IUserActivityVersionBase {
		new string WorkflowUniqueId { get; set; }
		[Size(SizeAttribute.Unlimited)]
		new string Xaml { get; set; }
		new int Version { get; set; }
		new string GetVersionId();
	}
	public class UserActivityVersion : IUserActivityVersion {
		public string WorkflowUniqueId { get; set; }
		public string Xaml { get; set; }
		public int Version { get; set; }
		public string GetVersionId() {
			return UserActivityVersionLogic.GetVersionId(this);
		}
	}
	[DomainLogic(typeof(IUserActivityVersion))]
	public class UserActivityVersionLogic {
		public static string GetVersionId(IUserActivityVersion userActivityVersion) {
			return string.Format("{0}_{1}", userActivityVersion.WorkflowUniqueId, userActivityVersion.Version);
		}
	}
}
