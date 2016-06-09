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
using System.Web;
namespace DevExpress.ExpressApp.Web {
	public class DeviceDetector {
		static DeviceDetector() {
			Instance = new DeviceDetector();
		}
		public static DeviceDetector Instance {
			get;
			set;
		}
		public virtual DeviceCategory GetDeviceCategory() {
			if(HttpContext.Current != null && HttpContext.Current.Request != null) {
				return GetDeviceCategory(HttpContext.Current.Request.UserAgent);
			}
			else {
				return DeviceCategory.Desktop;
			}
		}
		internal virtual DeviceCategory GetDeviceCategory(string userAgent) {
			if(!string.IsNullOrEmpty(userAgent)) {
				if(userAgent.IndexOf("Mobile", StringComparison.OrdinalIgnoreCase) > 0) {
					if(userAgent.IndexOf("iPhone", StringComparison.OrdinalIgnoreCase) > 0) {
						return DeviceCategory.Mobile;
					}
					if(userAgent.IndexOf("iPad", StringComparison.OrdinalIgnoreCase) > 0) {
						return DeviceCategory.Tablet;
					}
					if(userAgent.IndexOf("Android", StringComparison.OrdinalIgnoreCase) > 0) {
						return DeviceCategory.Mobile;
					}
					return DeviceCategory.Mobile;
				}
				else {
					if(userAgent.IndexOf("Android", StringComparison.OrdinalIgnoreCase) > 0) {
						return DeviceCategory.Tablet;
					}
					if(userAgent.IndexOf("Tablet PC", StringComparison.OrdinalIgnoreCase) > 0) {
						return DeviceCategory.Tablet;
					}
					if(userAgent.IndexOf("Touch", StringComparison.OrdinalIgnoreCase) > 0) {
						return DeviceCategory.Tablet;
					}
				}
			}
			return DeviceCategory.Desktop;
		}
	}
	public enum DeviceCategory { Desktop, Tablet, Mobile }
}
