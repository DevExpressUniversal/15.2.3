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

using System.Windows;
namespace DevExpress.Xpf.WindowsUI.Internal {
	public interface ILayoutCell {
		double StretchRatio { get; }
		Size DesiredSize { get; }
		LayoutAlignment DesiredAlignment { get; }
		bool IsAutoSize { get; }
		double GetLength(bool horizontal);
	}
	class LayoutCell : ILayoutCell {
		internal LayoutCell(Size desiredSize, double ratio, bool isAutoSize)
			: this(desiredSize, LayoutAlignment.Default, ratio, isAutoSize) {
		}
		public LayoutCell(Size desiredSize, LayoutAlignment desiredAlignment, double ratio, bool isAutoSize) {
			DesiredSize = desiredSize;
			DesiredAlignment = desiredAlignment;
			StretchRatio = ratio;
			IsAutoSize = isAutoSize;
		}
		public double StretchRatio { get; private set; }
		public Size DesiredSize { get; private set; }
		public LayoutAlignment DesiredAlignment { get; private set; }
		public bool IsAutoSize { get; private set; }
		public double GetLength(bool horizontal) {
			return horizontal ? DesiredSize.Width : DesiredSize.Height;
		}
	}
}
