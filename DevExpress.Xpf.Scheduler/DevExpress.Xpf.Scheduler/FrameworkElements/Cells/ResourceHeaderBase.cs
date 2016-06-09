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
using System.Collections.ObjectModel;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public abstract class VisualResourceHeaderBase : VisualResourceCellBase, ISupportCopyFrom<ResourceHeaderBase> {
		static VisualResourceHeaderBase() {
		}
		protected VisualResourceHeaderBase() {
			DefaultStyleKey = typeof(VisualResourceHeaderBase);
		}
		#region ISupportCopyFrom<ResourceHeaderBase> Members
		void ISupportCopyFrom<ResourceHeaderBase>.CopyFrom(ResourceHeaderBase source) {
			CopyFrom(source);
		}
		#endregion
	}
	public abstract class VisualResourceHeaderBaseContent : VisualResourceCellBaseContent, ISupportCopyFrom<ResourceHeaderBase> {
		#region Start
		public static readonly DependencyProperty StartProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualResourceHeaderBaseContent, DateTime>("Start", DateTime.MinValue, (d, e) => d.OnStartChanged(e.OldValue, e.NewValue));
		DateTime lastStart = DateTime.MinValue;
		public DateTime Start {
			get { return (DateTime)GetValue(StartProperty); }
			set {
				if (value == lastStart)
					return;
				SetValue(StartProperty, value);
			}
		}
		protected virtual void OnStartChanged(DateTime oldValue, DateTime newValue) {
			lastStart = newValue;
		}
		#endregion
		#region End
		public static readonly DependencyProperty EndProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualResourceHeaderBaseContent, DateTime>("End", DateTime.MinValue, (d, e) => d.OnEndChanged(e.OldValue, e.NewValue));
		DateTime lastEnd = DateTime.MinValue;
		public DateTime End {
			get { return (DateTime)GetValue(EndProperty); }
			set {
				if (value == lastEnd)
					return;
				SetValue(EndProperty, value);
			}
		}
		protected virtual void OnEndChanged(DateTime oldValue, DateTime newValue) {
			lastEnd = newValue;
		}
		#endregion
		#region ISupportCopyFrom<ResourceHeaderBase> Members
		void ISupportCopyFrom<ResourceHeaderBase>.CopyFrom(ResourceHeaderBase source) {
			CopyFrom(source);
		}
		#endregion
		protected internal override bool CopyFromCore(ResourceCellBase source) {
			bool wasChanged = base.CopyFromCore(source);
			ResourceHeaderBase sourceHeader = source as ResourceHeaderBase;
			if (sourceHeader == null)
				return wasChanged;
			Start = sourceHeader.Interval.Start;
			End = sourceHeader.Interval.End;
			return wasChanged;
		}
	}
	public class VisualResourceHeadersCollection : ObservableCollection<VisualResourceHeaderBase> {
	}
	public class VisualResourceHeaderBaseContentCollection : ObservableCollectionWithFirstAndLast<VisualResourceHeaderBaseContent> {
	}
}
