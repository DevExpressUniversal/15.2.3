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
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class VisualWeekCellContainer : VisualHorizontalCellContainer {
		#region CompressedIndex
		public static readonly DependencyProperty CompressedIndexProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualWeekCellContainer, int>("CompressedIndex", 0);
		public int CompressedIndex { get { return (int)GetValue(CompressedIndexProperty); } set { SetValue(CompressedIndexProperty, value); } }
		#endregion
		#region IsCompressed
		public static readonly DependencyProperty IsCompressedProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualWeekCellContainer, bool>("IsCompressed", false);
		public bool IsCompressed { get { return (bool)GetValue(IsCompressedProperty); } set { SetValue(IsCompressedProperty, value); } }
		#endregion
		protected override void CopyFromCore(ICellContainer source) {
			base.CopyFromCore(source);
			WeekBase weekBaseSource = (WeekBase)source;
			CompressedIndex = weekBaseSource.CompressedIndex;
			IsCompressed = weekBaseSource.IsCompressed;			
		}
		protected override VisualTimeCellBaseContent CreateVisualTimeCell() {
			return new VisualDateCellContent();
		}
	}
}
