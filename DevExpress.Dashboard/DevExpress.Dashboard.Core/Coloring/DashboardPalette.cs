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
	public class DashboardPalette {
		public int ColorsCount { get { return Colors.Length; } }
		Color[] Colors { 
			get { 
				return new[] {
					Color.FromArgb(95, 139, 149),
					Color.FromArgb(186, 77, 81),
					Color.FromArgb(175, 138, 83),
					Color.FromArgb(149, 95, 113),
					Color.FromArgb(133, 150, 102),
					Color.FromArgb(126, 104, 140),
					Color.FromArgb(95, 109, 149),
					Color.FromArgb(172, 86, 156),
					Color.FromArgb(166, 175, 83),
					Color.FromArgb(149, 113, 95),
					Color.FromArgb(93, 158, 129),
					Color.FromArgb(166, 119, 155),
					Color.FromArgb(74, 161, 170),
					Color.FromArgb(113, 129, 197),
					Color.FromArgb(196, 133, 61),
					Color.FromArgb(187, 194, 137),
					Color.FromArgb(157, 83, 97),
					Color.FromArgb(157, 160, 173),
					Color.FromArgb(92, 87, 126),
					Color.FromArgb(227, 166, 83)
				};
			} 
		}
		public DashboardPalette() {
		}
		public Color GetColor(int index) {
			return Colors[index % ColorsCount];
		}
	}
}
