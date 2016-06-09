﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

using DevExpress.Xpf.Charts.Native;
using System;
using System.ComponentModel;
using System.Windows;
namespace DevExpress.Xpf.Charts.RangeControlClient.Native {
	public class RangeClientAxisLabelItem : RangeClientItem, IRangeClientItem {
		Point location;
		Size size;
		string text;
		DataTemplate template;
		[Category(Categories.Data)]
		public string Text {
			get { return text; }
			set {
				if (text != value) {
					text = value;
					RaisePropertyChanged("Text");
				}
			}
		}
		[Category(Categories.Data)]
		public DataTemplate Template {
			get { return template; }
			set {
				if (template != value) {
					template = value;
					RaisePropertyChanged("Template");
				}
			}
		}
		[Category(Categories.Data)]
		public object Value { get; set; }
		[Category(Categories.Data)]
		public double InternalValue { get; set; }
		[Category(Categories.Data)]
		public bool IsMaster { get { return false; } }
		[Category(Categories.Data)]
		public bool Visible { get; set; }
		public Point Location {
			get { return location; }
		}
		public Size Size {
			get { return Visible ? size : new Size(0, 0); }
		}
		public RangeClientAxisLabelItem() {
			this.Visible = true;
		}
		void IRangeClientItem.CalculateLayout(IRangeClientScaleMap map, Size availableSize, Size desiredSize) {
			size = desiredSize;
			location = new Point(map.GetScreenPoint(InternalValue) - desiredSize.Width / 2, Math.Max(0, availableSize.Height - desiredSize.Height));
		}
	}
}
