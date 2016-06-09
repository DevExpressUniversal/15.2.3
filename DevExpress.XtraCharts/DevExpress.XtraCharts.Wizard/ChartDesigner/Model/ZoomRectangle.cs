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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.Utils.Design;
namespace DevExpress.XtraCharts.Designer.Native {
	[ModelOf(typeof(ZoomRectangle))]
	public class ZoomRectangleModel : DesignerChartElementModelBase {
		readonly ZoomRectangle zoomRectangle;
		LineStyleModel borderLineStyleModel;
		protected ZoomRectangle ZoomRectangle { get { return zoomRectangle; } }
		protected internal override ChartElement ChartElement { get { return zoomRectangle; } }
		[PropertyForOptions, TypeConverter(typeof(ColorConverter))]
		public Color Color {
			get { return ZoomRectangle.Color; }
			set { SetProperty("Color", value); }
		}
		[PropertyForOptions, TypeConverter(typeof(ColorConverter))]
		public Color BorderColor {
			get { return ZoomRectangle.BorderColor; }
			set { SetProperty("BorderColor", value); }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions]
		public LineStyleModel BorderLineStyle { get { return borderLineStyleModel; } }
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool BorderVisible {
			get { return ZoomRectangle.BorderVisible; }
			set { SetProperty("BorderVisible", value); }
		}
		public ZoomRectangleModel(ZoomRectangle zoomRectangle, CommandManager commandManager)
			: base(commandManager) {
			this.zoomRectangle = zoomRectangle;
			Update();
		}
		protected override void AddChildren() {
			if(borderLineStyleModel != null)
				Children.Add(borderLineStyleModel);
			base.AddChildren();
		}
		public override void Update() {
			this.borderLineStyleModel = new LineStyleModel(ZoomRectangle.BorderLineStyle, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
}
