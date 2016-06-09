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
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class VisualDayOfWeekHeader : VisualResourceCellBase, ISupportCopyFrom<DayOfWeekHeader> {
		static VisualDayOfWeekHeader() {
		}
		public VisualDayOfWeekHeader() {
			DefaultStyleKey = typeof(VisualDayOfWeekHeader);
		}
		#region ISupportCopyFrom<DayOfWeekHeader> Members
		public void CopyFrom(DayOfWeekHeader source) {
			CopyFromCore(source);
		}
		#endregion
	}
	public class VisualDayOfWeekHeaderContent : VisualResourceCellBaseContent, ISupportCopyFrom<DayOfWeekHeader> {
		#region DayOfWeek
		public static readonly DependencyProperty DayOfWeekProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDayOfWeekHeaderContent, DayOfWeek>("DayOfWeek", DayOfWeek.Sunday);
		public DayOfWeek DayOfWeek { get { return (DayOfWeek)GetValue(DayOfWeekProperty); } set { SetValue(DayOfWeekProperty, value); } }
		#endregion
		#region Caption
		public static readonly DependencyProperty CaptionProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDayOfWeekHeaderContent, string>("Caption", String.Empty);
		public string Caption { get { return (string)GetValue(CaptionProperty); } set { SetValue(CaptionProperty, value); } }
		#endregion
		#region IsCompressed
		public static readonly DependencyProperty IsCompressedProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDayOfWeekHeaderContent, bool>("IsCompressed", false);
		public bool IsCompressed { get { return (bool)GetValue(IsCompressedProperty); } set { SetValue(IsCompressedProperty, value); } }
		#endregion
		protected internal override bool CopyFromCore(ResourceCellBase source) {
			bool wasChanged = base.CopyFromCore(source);
			DayOfWeekHeader dayOfWeekHeaderSource = (DayOfWeekHeader)source;
			DayOfWeek = dayOfWeekHeaderSource.DayOfWeek;
			Caption = dayOfWeekHeaderSource.Caption;
			IsCompressed = dayOfWeekHeaderSource.IsCompressed;
			return wasChanged;
		}
		#region ISupportCopyFrom<DayOfWeekHeader> Members
		void ISupportCopyFrom<DayOfWeekHeader>.CopyFrom(DayOfWeekHeader source) {
			CopyFrom(source);
		}
		#endregion
	}
	public class VisualDayOfWeekHeadersCollection : ObservableCollection<VisualDayOfWeekHeader> {
	}
	public class VisualDayOfWeekHeaderContentCollection : ObservableCollection<VisualDayOfWeekHeaderContent> {
	}
}
