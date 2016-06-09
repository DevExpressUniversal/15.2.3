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

using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.DashboardCommon.Native {
	public abstract class StyleSettingsInfo {
		public virtual Color ForeColor { get; set; }
		public virtual Color BackColor { get; set; }
		public virtual Font Font { get; set; }
		public Image Image { get; set; }
		public BarStyleSettingsInfo Bar { get; set; }
	}
	public interface IMinMaxInfo {
		decimal AbsoluteMinimum { get; }
		decimal AbsoluteMaximum { get; }
		decimal Minimum { get; }
		decimal Maximum { get; }
		DashboardFormatConditionValueType MinimumType { get; }
		DashboardFormatConditionValueType MaximumType { get; }
	}
	public interface IBarStyleSettingsInfo {
		Color Color { get; set; }
		bool ShowBarOnly { get; set; }
		bool AllowNegativeAxis { get; set; }
		bool DrawAxis { get; set; }
		decimal NormalizedValue { get; set; }
		decimal ZeroPosition { get; set; }
	}
	public class BarStyleSettingsInfo : IBarStyleSettingsInfo {
		public Color Color { get; set; }
		public bool ShowBarOnly { get; set; }
		public bool AllowNegativeAxis { get; set; }
		public bool DrawAxis { get; set; }
		public decimal NormalizedValue { get; set; }
		public decimal ZeroPosition { get; set; }
	}
}
