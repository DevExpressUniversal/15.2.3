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
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class VisualHorizontalWeekViewResource : VisualResource {
		#region DayOfWeekHeaders
		static readonly DependencyPropertyKey DayOfWeekHeadersPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualHorizontalWeekViewResource, VisualDayOfWeekHeaderContentCollection>("DayOfWeekHeaders", null);
		public static readonly DependencyProperty DayOfWeekHeadersProperty = DayOfWeekHeadersPropertyKey.DependencyProperty;
		public VisualDayOfWeekHeaderContentCollection DayOfWeekHeaders { get { return (VisualDayOfWeekHeaderContentCollection)GetValue(DayOfWeekHeadersProperty); } protected set { this.SetValue(DayOfWeekHeadersPropertyKey, value); } }
		#endregion
		protected override VisualSimpleResourceInterval CreateVisualSimpleResourceInterval() {
			return new VisualHorizontalWeek();
		}
		protected override void CopyFromCore(SingleResourceViewInfo source) {
			base.CopyFromCore(source);			
			WeekBasedSingleResourceViewInfo weekBasedSource = source as WeekBasedSingleResourceViewInfo;			
			if (weekBasedSource.DayOfWeekHeaders != null) {
				if (DayOfWeekHeaders == null)
					DayOfWeekHeaders = new VisualDayOfWeekHeaderContentCollection();
				CollectionCopyHelper.Copy(DayOfWeekHeaders, weekBasedSource.DayOfWeekHeaders, CreateVisualDayOfWeekHeader);				
			}
			else {
				DayOfWeekHeaders = null;
			}
		}
		protected override CellContainerCollection GetCellContainers(SingleResourceViewInfo source) {
			return source.CellContainers;
		}
		protected override VisualResourceHeaderBaseContent CreateResourceHeaderCore() {
			return new VisualResourceHeaderContent();
		}
		protected virtual VisualDayOfWeekHeaderContent CreateVisualDayOfWeekHeader() {
			VisualDayOfWeekHeaderContent result = new VisualDayOfWeekHeaderContent();
			return result;
		}
	}
}
