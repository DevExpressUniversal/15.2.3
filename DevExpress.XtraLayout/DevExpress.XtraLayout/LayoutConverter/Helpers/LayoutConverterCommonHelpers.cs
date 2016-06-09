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
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using System.ComponentModel.Design;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraEditors;
using DevExpress.Utils;
namespace DevExpress.XtraLayout.Converter{
	public enum LayoutLocationType { Top, Bottom, Left, Right, LeftTop, LeftBottom, RightTop, RightBottom, Unknown }
	public class ConvertToXtraLayoutItemInfo {
		Control targetConrtol, labelControl;
		Locations labelLayout;
		public ConvertToXtraLayoutItemInfo(Locations labelLayout, Control targetConrtol, Control labelControl) {
			this.labelLayout = labelLayout;
			this.targetConrtol = targetConrtol;
			this.labelControl = labelControl;
		}
		public Locations LabelLayout {
			get { return labelLayout; }
			set { labelLayout = value; }
		}
		public Control TargetControl {
			get { return targetConrtol; }
			set { targetConrtol = value; }
		}
		public Control LabelControl {
			get { return labelControl; }
			set { labelControl = value; }
		}
	}
	public class RectangleDistanceCalculator {
		public static Locations ConvertLayoutLocationType(LayoutLocationType location) {
			switch(location) {
				case LayoutLocationType.Top: return Locations.Top;
				case LayoutLocationType.Left: return Locations.Left;
				case LayoutLocationType.Right: return Locations.Right;
				case LayoutLocationType.Bottom: return Locations.Bottom;
				default: return Locations.Default;
			}
		}
		public static LayoutLocationType CalcLayoutLocation(Rectangle rect1, Rectangle rect2) {
			LayoutLocationType layoutLocation;
			if(rect1.Right <= rect2.X) {
				if(rect1.Bottom <= rect2.Y)
					layoutLocation = LayoutLocationType.LeftTop;
				else if(rect1.Y >= rect2.Bottom)
					layoutLocation = LayoutLocationType.LeftBottom;
				else
					layoutLocation = LayoutLocationType.Left;
			}
			else if(rect1.X >= rect2.Right) {
				if(rect1.Bottom <= rect2.Y)
					layoutLocation = LayoutLocationType.RightTop;
				else if(rect1.Y >= rect2.Bottom)
					layoutLocation = LayoutLocationType.RightBottom;
				else
					layoutLocation = LayoutLocationType.Right;
			}
			else if(rect1.Bottom <= rect2.Y)
				layoutLocation = LayoutLocationType.Top;
			else if(rect1.Y >= rect2.Bottom)
				layoutLocation = LayoutLocationType.Bottom;
			else
				layoutLocation = LayoutLocationType.Unknown;
			return layoutLocation;
		}
		public static int Calc2RectDistance(Rectangle rect1, Rectangle rect2) {
			LayoutLocationType layoutLocation = CalcLayoutLocation(rect1, rect2);
			if(layoutLocation == LayoutLocationType.Unknown) return int.MaxValue;
			switch(layoutLocation) {
				case LayoutLocationType.Bottom:
					return rect1.Y - rect2.Bottom;
				case LayoutLocationType.Top:
					return rect2.Y - rect1.Bottom;
				case LayoutLocationType.Left:
					return (rect2.X - rect1.Right)/5;
				case LayoutLocationType.Right:
					return rect1.X - rect2.Right;
				default: return int.MaxValue;
			}
		}
	}
	public class LayoutConversionHelper {
		protected static bool IsLabelControl(Control control) {
			Label label = control as Label;
			LabelControl labelControl = control as LabelControl;
			if(label != null || labelControl != null) return true;
			return false;
		}
		protected static bool IsProcessed(Control control, List<ConvertToXtraLayoutItemInfo> list) {
			foreach(ConvertToXtraLayoutItemInfo info in list) {
				if(info.LabelControl == control || info.TargetControl == control) return true;
			}
			return false;
		}
		protected static bool IsLabelForControl(Control labelControl, Control notLabelControl, ArrayList list) {
			if(ToXtraLayoutConverterHelper.IsTabControl(notLabelControl) || ToXtraLayoutConverterHelper.IsGroup(notLabelControl)) return false;
			int minDistance = int.MaxValue;
			Control nearestControl = null;
			foreach(Control tempControl in list) {
				if(labelControl == tempControl) continue;
				if(!IsLabelControl(tempControl)) {
					int distance = RectangleDistanceCalculator.Calc2RectDistance(labelControl.Bounds, tempControl.Bounds);
					if(distance < minDistance){
						minDistance = distance;
						nearestControl = tempControl;
					}
				}
			}
			if(notLabelControl == nearestControl) return true;
			return false;
		}
		static ArrayList stopList;
		protected static ArrayList StopList {
			get {
				if(stopList == null) {
					stopList = new ArrayList();
					stopList.Add("BarDockControl");
					stopList.Add("StatusBar");
					stopList.Add("RibbonStatusBar");
					stopList.Add("RibbonControl");
					stopList.Add("DockPanel");
					stopList.Add("ZIndexControl");
					stopList.Add("AutoHideContainer");
				}
				return stopList;
			}
		}
		protected static bool CanProcessControl(Control control) {
			if (!AllowCreateLayoutItemForControl(control)) return false;
			if(control.GetType().ToString().EndsWith("StandaloneBarDockControl")) return true;
			foreach(string typeName in StopList) {
				if(control.GetType().ToString().EndsWith(typeName)) return false;
			}
			return true;
		}
		public static bool AllowCreateLayoutItemForControl(Control control) {
			if (control is DevExpress.XtraEditors.PopupContainerControl) return false;
			if (control is DevExpress.Utils.Menu.IDXDropDownControl) return false;
			return true;
		}
		public static  List<ConvertToXtraLayoutItemInfo> CalculateConversionInfo(Control control, Control excludeControl) {
			ArrayList list = new ArrayList();
			if(ToXtraLayoutConverterHelper.IsTabControl(control)) list.Add(control);
			else  list.AddRange(control.Controls); 
			List<ConvertToXtraLayoutItemInfo> result = new List<ConvertToXtraLayoutItemInfo>();
			foreach(Control notLablelControl in list) {
				if(notLablelControl == excludeControl) continue;
				if(IsLabelControl(notLablelControl)) continue;
				if(IsProcessed(notLablelControl, result)) continue;
				if(!CanProcessControl(notLablelControl)) continue;
				foreach(Control labelControl in list) {
					if(!IsLabelControl(labelControl)) continue;
					if(IsProcessed(labelControl, result)) continue;
					if(labelControl == excludeControl) continue;
					if(!CanProcessControl(labelControl)) continue;
					if(IsLabelForControl(labelControl, notLablelControl, list)) {
						Locations location = RectangleDistanceCalculator.ConvertLayoutLocationType(RectangleDistanceCalculator.CalcLayoutLocation(labelControl.Bounds, notLablelControl.Bounds));
						result.Add(new ConvertToXtraLayoutItemInfo(location, notLablelControl, labelControl));
						break;
					}
				}
			}
			foreach(Control anyControl in list) {
				if(anyControl == excludeControl) continue;
				if(IsProcessed(anyControl, result)) continue;
				if(!CanProcessControl(anyControl)) continue;
				result.Add(new ConvertToXtraLayoutItemInfo(Locations.Default, anyControl, null));
			}
			return result;
		}
	}
}
