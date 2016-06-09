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
using System.Collections.Generic;
using DevExpress.XtraScheduler;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
#if SILVERLIGHT
using DevExpress.Xpf.Editors.Controls;
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
#else
using System.Windows.Controls;
using DevExpress.Xpf.Scheduler.UI;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class HorizontalAppointmentLayoutInfo : IAppointmentIntermediateLayoutViewInfo {
		public HorizontalAppointmentLayoutInfo() {
		}
		public double Height { get; set; }
		public double RelativePosition { get; set; }
		public int FirstCellIndex { get; set; }
		public int LastCellIndex { get; set; }
		public bool Visible { get; set; }
		public AppointmentCellIndexes CellIndexes { get; set; }
		public int FirstIndexPosition { get; set; }
		public int LastIndexPosition { get; set; }
		public int MaxIndexInGroup { get; set; }
		public Appointment Appointment { get; set; }
		public TimeInterval Interval { get; set; }
	}
	public class HorizontalAppointmentLayoutInfoCollection : List<HorizontalAppointmentLayoutInfo>, IAppointmentIntermediateLayoutViewInfoCoreCollection {
		#region IAppointmentIntermediateLayoutViewInfoCoreCollection Members
		IAppointmentIntermediateLayoutViewInfo IAppointmentIntermediateLayoutViewInfoCoreCollection.this[int index] { get { return this[index]; } }
		#endregion
	}
}
namespace DevExpress.Xpf.Scheduler.Native {
	#region CalculateAppointmentPositionResult
	public enum CalculateAppointmentPositionResult { AppointmentNotFitted, AppointmentPartialFitted, AppointmentFitted };
	#endregion
}
