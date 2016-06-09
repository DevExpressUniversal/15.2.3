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
using System.Windows.Controls;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public class ConstantLineTitlePanel : Panel {
		protected override Size MeasureOverride(Size availableSize) {
			foreach (UIElement element in Children) {
				ConstantLineTitlePresentation titlePresentation = element as ConstantLineTitlePresentation;
				if (titlePresentation != null) {
					ConstantLineTitleItem titleItem = titlePresentation.ConstantLineTitleItem;
					titlePresentation.Measure((titleItem != null && titleItem.Visible) ? availableSize : new Size(0, 0));
				}
			}
			return new Size(MathUtils.ConvertInfinityToDefault(availableSize.Width, 0), MathUtils.ConvertInfinityToDefault(availableSize.Height, 0));
		}
		protected override Size ArrangeOverride(Size finalSize) {
			foreach (UIElement element in Children) {
				ConstantLineTitlePresentation titlePresentation = element as ConstantLineTitlePresentation;
				if (titlePresentation != null) {
					ConstantLineTitleItem titleItem = titlePresentation.ConstantLineTitleItem;
					titlePresentation.Arrange((titleItem != null && titleItem.Visible) ? titleItem.CalculateTitleRect(finalSize) : RectExtensions.Zero);		 
				}
			}
			return finalSize;
		}
	}
}
