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
#if SL
using ApplicationException = System.Exception;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
#if !SL
	[Serializable]
	public class InfinitePivotGridSizeException : ApplicationException {
		public const string InfiniteWidthMessage = "By default, an infinite grid width is not allowed since all grid columns will be rendered and hence the grid will work very slowly. To fix this issue, you should place the grid into a container that will give a finite width to the grid, or manually specify the grid's Width or MaxWidth. Note that you can also avoid this exception by setting the PivotGridControl.AllowInfiniteGridSize static property to True, but in that case the grid will run slowly.";
		public const string InfiniteHeightMessage = "By default, an infinite grid height is not allowed since all grid rows will be rendered and hence the grid will work very slowly. To fix this issue, you should place the grid into a container that will give a finite height to the grid, or you should manually specify the grid's Height or MaxHeight. Note that you can also avoid this exception by setting the PivotGridControl.AllowInfiniteGridSize static property to True, but in that case the grid will run slowly.";
		public static void ValidateDefineSize(Size defineSize) {
			if(!PivotGridControl.AllowInfiniteGridSize &&
					(double.IsPositiveInfinity(defineSize.Width) || double.IsPositiveInfinity(defineSize.Height))) {
				string message = double.IsPositiveInfinity(defineSize.Width) ? InfiniteWidthMessage : InfiniteHeightMessage;
				throw new InfinitePivotGridSizeException(message);
			}
		}
		public InfinitePivotGridSizeException(string message)
			: base(message) {
		}
	}
#endif
}
