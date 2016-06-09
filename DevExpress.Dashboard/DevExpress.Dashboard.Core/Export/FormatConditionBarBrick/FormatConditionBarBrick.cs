#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon.Native;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.DashboardExport {
	public static class DashboardBrickType {
		public const string FormatConditionBarBrick = "FormatConditionBarBrick";
	}
	[BrickExporter(typeof(FormatConditionBarBrickExporter))]
	public class FormatConditionBarBrick : TextBrick, IBarStyleSettingsInfo {
		[
		XtraSerializableProperty]
		public override string BrickType { get { return DashboardBrickType.FormatConditionBarBrick; } }
		[
		XtraSerializableProperty]
		public object BarValue { get; set; }
		[
		XtraSerializableProperty]
		public Color Color { get; set; }
		[
		XtraSerializableProperty]
		public bool ShowBarOnly { get; set; }
		[
		XtraSerializableProperty]
		public bool AllowNegativeAxis { get; set; }
		[
		XtraSerializableProperty]
		public bool DrawAxis { get; set; }
		[
		XtraSerializableProperty]
		public decimal NormalizedValue { get; set; }
		[
		XtraSerializableProperty]
		public decimal ZeroPosition { get; set; }
		public FormatConditionBarBrick()
			: base() {
		}
		public FormatConditionBarBrick(BrickStyle style)
			: base(style) {
		}
		public void Assign(IBarStyleSettingsInfo barInfo) {
			Color = barInfo.Color;
			ShowBarOnly = barInfo.ShowBarOnly;
			AllowNegativeAxis = barInfo.AllowNegativeAxis;
			DrawAxis = barInfo.DrawAxis;
			NormalizedValue = barInfo.NormalizedValue;
			ZeroPosition = barInfo.ZeroPosition;
		}
	}
	static class DashboardBrickResolver {
		public static void RegisterBrickFactory() {
			BrickFactory.RegisterFactory(DashboardBrickType.FormatConditionBarBrick, new DefaultBrickFactory<FormatConditionBarBrick>());
		}
	}
}
