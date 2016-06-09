#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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

namespace DevExpress.XtraReports.Web.Azure.WebDocumentViewer.Native {
	public static class ServiceConstatns {
		public const string ReportStorageItemName = "dxxrreports";
		public const string DocumentStorageItemName = "dxxrdocuments";
		public const string CommonStorageItemName = "dxxrcommonstorage";
		public const string DocumentEntityHotRowKey = "hot";
		public const string DocumentEntityColdRowKey = "generated";
		public const string EntityId = "entityid";
		public const string BlobPathPropertyName = "blob-file-path";
		public const string ActionNamePropertyName = "actionName";
		public const string JsonArgumentPropertyName = "jsonArguments";
		public const string FaultMessagePropertyName = "faultMessage";
		public const string DefaultRequestsTopicName = "dxxrtopic";
		public const string DefaultRequestsQueueName = "dxxrrequests";
		public const string DefaultResponsesQueueName = "dxxrresponses";
		public const string DefaultTopicSubscriptionName = "dxxrsubscription";
	}
}
