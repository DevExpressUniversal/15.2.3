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
using System.Windows.Media;
using DevExpress.Xpf.Editors.Helpers;
namespace DevExpress.Xpf.Editors.Internal {
	public static class BrushConverter {
		public static Brush ToBrushType(object brush, BrushType brushType) {
			if (brushType == BrushType.None)
				return null;
			if (brushType == BrushType.SolidColorBrush)
				return brush.ToSolidColorBrush();
			if (brushType == BrushType.LinearGradientBrush)
				return brush.ToLinearGradientBrush();
			if (brushType == BrushType.RadialGradientBrush)
				return brush.ToRadialGradientBrush();
			return brush as Brush;
		}
		public static BrushType GetBrushType(object editValue, BrushType brushType) {
			if (brushType == BrushType.AutoDetect) {
				Type type = editValue != null ? editValue.GetType() : null;
				if (type == null || type == typeof(SolidColorBrush))
					return BrushType.SolidColorBrush;
				if (type == typeof(LinearGradientBrush))
					return BrushType.LinearGradientBrush;
				if (type == typeof(RadialGradientBrush))
					return BrushType.RadialGradientBrush;
				return BrushType.SolidColorBrush;
			}
			return brushType;
		}
	}
}
