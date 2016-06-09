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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Scheduler.Native;
using System.Windows.Controls;
namespace DevExpress.Xpf.Scheduler.Drawing {
	#region SchedulerContentControl
	public class SchedulerContentControl : XPFContentControl, IVisualElement {
		public SchedulerContentControl() {
			DefaultStyleKey = typeof(SchedulerContentControl);
		}
		internal IVisualComponent Owner { get; set; }
		protected override sealed Size MeasureOverride(Size constraint) {
			Size desiredSize = DesiredSize;
			Size size = MeasureInternal(constraint);
			if (Owner != null && !SchedulerSizeHelper.AreClose(desiredSize, size))
				Owner.OnChildrenChanged(this);
			return size;
		}
		protected virtual Size MeasureInternal(Size constraint) {
			return base.MeasureOverride(constraint);
		}
	}
	#endregion
	#region SchedulerContentPresenter
	public class SchedulerContentPresenter : XPFContentPresenter, IVisualElement {
	}
	#endregion
	#region SchedulerAreaControl
	public class SchedulerAreaControl : Control {
	}
	#endregion
	#region SchedulerButton
	public class SchedulerButton : Button, IVisualElement {
		public SchedulerButton() {
			DefaultStyleKey = typeof(SchedulerButton);
		}
		internal IVisualComponent Owner { get; set; }
		protected override sealed Size MeasureOverride(Size constraint) {
			Size size = MeasureInternal(constraint);
			if (Owner != null)
				Owner.OnChildrenChanged(this);
			return size;
		}
		protected virtual Size MeasureInternal(Size constraint) {
			return base.MeasureOverride(constraint);
		}
	}
	#endregion
}
