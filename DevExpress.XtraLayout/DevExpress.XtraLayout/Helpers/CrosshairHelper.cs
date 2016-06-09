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

using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout.Resizing;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraLayout.Localization;
using DevExpress.XtraLayout.Adapters;
using DevExpress.Utils.Drawing;
using DevExpress.XtraTab;
using System.Collections.Generic;
using DevExpress.XtraDashboardLayout;
using System.Linq;
namespace DevExpress.XtraLayout.Helpers {
	public class RectangleHelper {
		public static Point LeftTopCorner(Rectangle rect) { return rect.Location; }
		public static Point LeftBottomCorner(Rectangle rect) { return new Point(rect.Left, rect.Bottom); }
		public static Point RightTopCorner(Rectangle rect) { return new Point(rect.Right, rect.Top); }
		public static Point RightBottomCorner(Rectangle rect) { return new Point(rect.Right, rect.Bottom); }
	}
	public class CrosshairHelper {
		public static List<Crosshair> GetGroupCrosshairs(DashboardLayoutControlGroupBase group) {
			return group.CrosshairCollection;
		}
		public static List<Crosshair> GetItemCrosshairs(List<Crosshair> groupCrosshairs, BaseLayoutItem item) {
			List<Crosshair> result = new List<Crosshair>();
			foreach(Crosshair ch in groupCrosshairs) {
				foreach(BaseLayoutItem temp in ch.GetItems()) {
					if(temp == item) { result.Add(ch); break; }
				}
			}
			return result;
		}
	}
}
