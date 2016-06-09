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
	public class VisualWeekBase : VisualSimpleResourceInterval {
		#region CellContainer
		public static readonly DependencyProperty CellContainerProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualWeekBase, VisualWeekCellContainer>("CellContainer", null);
		public VisualWeekCellContainer CellContainer { get { return (VisualWeekCellContainer)GetValue(CellContainerProperty); } set { SetValue(CellContainerProperty, value); } }
		#endregion
		protected override void CopyFromCore(ICellContainer source) {
			WeekBase weekSource = (WeekBase)source;
			if (CellContainer == null)
				CellContainer = CreateWeekCellContainer();
			((ISupportCopyFrom<ICellContainer>)CellContainer).CopyFrom(source);
			this.View = weekSource.View;
			this.IntervalStart = weekSource.Interval.Start;
			this.IntervalEnd = weekSource.Interval.End;
			if (Brushes == null)
				Brushes = new VisualResourceBrushes();
			Brushes.CopyFrom(weekSource.Brushes, (object)source.Resource.Id);
		}
		public override void CopyAppointmentsFrom(ICellContainer cellContainer) {
			if (CellContainer == null)
				return;
			CellContainer.CopyAppointmentsFrom(cellContainer);
		}
		protected internal virtual VisualWeekCellContainer CreateWeekCellContainer() {
			return new VisualWeekCellContainer();
		}
	}
}
