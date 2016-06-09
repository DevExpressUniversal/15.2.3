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

using System.Collections.Generic;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.DrillDown;
namespace DevExpress.XtraReports.Service.Native.Services.Transient {
	public struct ReportConfiguration {
		static readonly ReportConfiguration defaultInstance = new ReportConfiguration(ReportLifetimeConfiguration.DefaultInstance);
		public static ReportConfiguration DefaultInstance {
			get { return defaultInstance; }
		}
		public ReportLifetimeConfiguration Lifetime { get; private set; }
		public PageData PageData { get; private set; }
		public Watermark Watermark { get; private set; }
		public Dictionary<DrillDownKey, bool> DrillDownKeys { get; private set; }
		public object CustomArgs { get; private set; }
		public ReportConfiguration(ReportLifetimeConfiguration lifetime)
			: this() {
			Lifetime = lifetime;
		}
		public ReportConfiguration(PageData pageData)
			: this(ReportLifetimeConfiguration.DefaultInstance) {
			PageData = pageData;
		}
		public ReportConfiguration(ReportLifetimeConfiguration lifetime, PageData pageData, Watermark watermark, Dictionary<DrillDownKey, bool> drillDownKeys)
			: this(lifetime) {
			PageData = pageData;
			Watermark = watermark;
			DrillDownKeys = drillDownKeys;
		}
		public ReportConfiguration(ReportLifetimeConfiguration lifetime, PageData pageData, Watermark watermark, Dictionary<DrillDownKey, bool> drillDownKeys, object customArgs)
			: this(lifetime, pageData, watermark, drillDownKeys) {
			CustomArgs = customArgs;
		}
	}
}
