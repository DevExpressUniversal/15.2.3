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
using System.Windows;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Bars {
	public static class RadialMenuMeasurerHelper {
		public static double CalcSectorWidth(double sectorAngle, double sectorRadius) {
			if(sectorAngle > Math.PI)
				throw new InvalidOperationException("Invalid sectorAngle parameter");
			return 2d * Math.Sin(sectorAngle / 2) * sectorRadius;
		}
		public static Size CalcSectorSize(double sectorAngle, double sectorRadius) {
			return new Size(CalcSectorWidth(sectorAngle, sectorRadius), sectorRadius);
		}
		public static Size CalcAvailableItemSize(Size availableSize, double sectorAngle) {
			if(availableSize.Width.IsNotNumber() && availableSize.Height.IsNumber()) {
				return CalcSectorSize(sectorAngle, availableSize.Height / 2);
			}
			if(availableSize.Height.IsNotNumber() && availableSize.Width.IsNumber()) {
				return CalcSectorSize(sectorAngle, availableSize.Width / 2);
			}
			if(availableSize.Height.IsNumber() && availableSize.Width.IsNumber()) {
				return CalcSectorSize(sectorAngle, Math.Min(availableSize.Width, availableSize.Height) / 2);				
			}
			return new Size(double.PositiveInfinity, double.PositiveInfinity);
		}
	}
}
