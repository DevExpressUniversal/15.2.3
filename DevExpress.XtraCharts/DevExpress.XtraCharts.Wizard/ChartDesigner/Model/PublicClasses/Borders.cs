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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DevExpress.Utils.Design;
namespace DevExpress.XtraCharts.Designer.Native {
	public abstract class BorderBaseModel : DesignerChartElementModelBase {
		readonly BorderBase border;
		protected BorderBase Border { get { return border; } }
		protected internal override ChartElement ChartElement { get { return border; } }
		[PropertyForOptions,
		DesignerDisplayName("Border Visibility"),
		TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean Visibility {
			get { return Border.Visibility; }
			set { SetProperty("Visibility", value); }
		}
		[PropertyForOptions,
		DependentUpon("Visibility"),
		DesignerDisplayName("Border Color")]
		public Color Color {
			get { return Border.Color; }
			set { SetProperty("Color", value); }
		}
		[PropertyForOptions,
		DependentUpon("Visibility"),
		DesignerDisplayName("Border Thickness")]
		public int Thickness {
			get { return Border.Thickness; }
			set { SetProperty("Thickness", value); }
		}
		public BorderBaseModel(BorderBase border, CommandManager commandManager)
			: base(commandManager) {
			this.border = border;
		}
	}
	public abstract class RectangularBorderModel : BorderBaseModel {
		protected new RectangularBorder Border { get { return (RectangularBorder)base.Border; } }
		public RectangularBorderModel(BorderBase border, CommandManager commandManager)
			: base(border, commandManager) {
		}
	}
	[ModelOf(typeof(InsideRectangularBorder))]
	public class InsideRectangularBorderModel : RectangularBorderModel {
		protected new InsideRectangularBorder Border { get { return (InsideRectangularBorder)base.Border; } }
		public InsideRectangularBorderModel(InsideRectangularBorder border, CommandManager commandManager)
			: base(border, commandManager) {
		}
	}
	[ModelOf(typeof(OutsideRectangularBorder))]
	public class OutsideRectangularBorderModel : RectangularBorderModel {
		protected new OutsideRectangularBorder Border { get { return (OutsideRectangularBorder)base.Border; } }
		public OutsideRectangularBorderModel(OutsideRectangularBorder border, CommandManager commandManager)
			: base(border, commandManager) {
		}
	}
	[ModelOf(typeof(CustomBorder))]
	public class CustomBorderModel : BorderBaseModel {
		protected new CustomBorder Border { get { return (CustomBorder)base.Border; } }
		public CustomBorderModel(CustomBorder border, CommandManager commandManager)
			: base(border, commandManager) {
		}
	}
}
