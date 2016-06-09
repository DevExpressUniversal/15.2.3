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
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class VisualTimeCell : VisualWorkTimeCellBase, ISupportHitTest {
		static VisualTimeCell() {
		}
		public VisualTimeCell() {
			DefaultStyleKey = typeof(VisualTimeCell);
		}
	}
	public class VisualTimeCellContent : VisualWorkTimeCellBaseContent {
		#region EndOfHour
		public static readonly DependencyProperty EndOfHourProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualTimeCellContent, bool>("EndOfHour", false, (d, e) => d.OnEndOfHourChanged(e.OldValue, e.NewValue));
		public bool EndOfHour { get { return (bool)GetValue(EndOfHourProperty); } set { SetValue(EndOfHourProperty, value); } }
		#endregion
		protected virtual void OnEndOfHourChanged(bool oldValue, bool newValue) {
			lastSettedEndOfHour = newValue;
		}
		bool lastSettedEndOfHour;
		protected internal override bool CopyFromCore(ResourceCellBase source) {
			bool wasChanged = base.CopyFromCore(source);
			TimeCell cell = (TimeCell)source;
			if (lastSettedEndOfHour != cell.EndOfHour) {
				this.EndOfHour = cell.EndOfHour;
				wasChanged = true;
			}
			return wasChanged;
		}
	}
}
