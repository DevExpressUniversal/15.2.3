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
using DevExpress.XtraReports.Extensions;
namespace DevExpress.ExpressApp.Reports {
	public class CreateCustomReportExtensionEventArgs : EventArgs {
		public CreateCustomReportExtensionEventArgs() {
		}
		public ReportDesignExtension ReportDesignExtension { get; set; }
	}
	public class CustomizeReportExtensionEventArgs : EventArgs {
		public CustomizeReportExtensionEventArgs(ReportDesignExtension reportDesignExtension) {
			this.ReportDesignExtension = reportDesignExtension;
		}
		public ReportDesignExtension ReportDesignExtension { get; private set; }
		public XafReportDataTypeProvider XafReportDataTypeProvider { get; set; }
	}
	public static class ReportDesignExtensionManager {
		public static bool Initialize(XafApplication application) {
			if(ReportExtension.Application == null) {
				ReportExtension.Application = application;
			}
			if(!IsInitialized) {
				CreateCustomReportExtensionEventArgs args = new CreateCustomReportExtensionEventArgs();
				if(CreateCustomReportExtension != null) {
					CreateCustomReportExtension(null, args);
				}
				ReportDesignExtension reportDesignExtension = args.ReportDesignExtension ?? new ReportExtension();
				CustomizeReportExtensionEventArgs args2 = new CustomizeReportExtensionEventArgs(reportDesignExtension);
				ReportExtension extension = reportDesignExtension as ReportExtension;
				if(extension != null) {
					args2.XafReportDataTypeProvider = new XafReportDataTypeProvider();
				}
				if(CustomizeReportExtension != null) {
					CustomizeReportExtension(null, args2);
				}
				if(args2.XafReportDataTypeProvider != null && extension != null) {
					args2.XafReportDataTypeProvider.Attach(extension);
				}
				ReportDesignExtension.RegisterExtension(reportDesignExtension, XafReport.XafReportContextName);
				IsInitialized = true;
				return true;
			}
			return false;
		}
		public static bool IsInitialized { get; private set; }
		public static event EventHandler<CreateCustomReportExtensionEventArgs> CreateCustomReportExtension;
		public static event EventHandler<CustomizeReportExtensionEventArgs> CustomizeReportExtension;
	}
}
