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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Native;
using System.Windows.Controls;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler;
using DevExpress.Xpo.DB;
#if WPF|| SL
using DevExpress.Xpf.Scheduler;
using DevExpress.Xpf.Core;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	#region HeaderImageAlign
	public enum HeaderImageAlign {
		Left = 0,
		Top = 1,
		Right = 2,
		Bottom = 3
	};
	#endregion
	#region HeaderImageSizeMode
	public enum HeaderImageSizeMode {
		Normal = 0,
		CenterImage = 1,
		StretchImage = 2,
		ZoomImage = 3
	};
	#endregion
	#region SchedulerHeader (abstract)
	public abstract class SchedulerHeader : XPFContentControl, IDisposable {
		#region Fields
		bool isDisposed;
		string caption = String.Empty;
		bool hasSeparator;
		bool alternate;
		#endregion
		protected SchedulerHeader() {
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal bool IsDisposed { get { return isDisposed; } }
		protected internal virtual bool AllowAbbreviatedCaption { get { return false; } }
		public bool HasSeparator { get { return hasSeparator; } set { hasSeparator = value; } }
		public bool Alternate { get { return alternate; } set { alternate = value; } }
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
			}
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~SchedulerHeader() {
			Dispose(false);
		}
		#endregion
	}
	#endregion
	#region SchedulerHeaderCollection
	public class SchedulerHeaderCollection : List<SchedulerHeader> {
	}
	#endregion
	#region DayHeader
	public class DayHeader : SchedulerHeader {
		public DayHeader() {
			DefaultStyleKey = typeof(DayHeader);
		}
		protected internal override bool AllowAbbreviatedCaption { get { return true; } }
	}
	#endregion
	public class DayOfWeekHeaderCollection : AssignableCollection<DayOfWeekHeader> {
	}
	#region DayHeaderContainer
	public class DayHeaderContainer {
		#region Fields
		DayOfWeekHeaderCollection dayHeaders;
		Resource resource;
		ResourceBrushes brushes;
		readonly WeekViewBase view;
		#endregion
		public DayHeaderContainer(DayOfWeek[] daysOfWeek, WeekViewBase view, Resource resource, ResourceBrushes brushes) {
			Guard.ArgumentNotNull(view, "view");
			Guard.ArgumentNotNull(resource, "resource");
			Guard.ArgumentNotNull(daysOfWeek, "daysOfWeek");
			Guard.ArgumentPositive(daysOfWeek.Length, "daysOfWeek.Length");
			this.view = view;
			this.resource = resource;
			this.brushes = brushes;
			bool compressWeekend = view.InnerView.CompressWeekendInternal;
			if(UseCompressedHeaders(daysOfWeek, compressWeekend))
				this.dayHeaders = CreateCompressedDayHeaders(daysOfWeek);
			else
				this.dayHeaders = CreateOrdinaryDayHeaders(daysOfWeek);
		}
		#region Properties
		public DayOfWeekHeaderCollection DayHeaders { 
			get { return dayHeaders; } 
			private set {
				dayHeaders = value;
			}
		}
		public Resource Resource { 
			get { return resource; } 
			set {
				resource = value;
			}
		}
		public ResourceBrushes Brushes { 
			get { return brushes; }
			set {
				brushes = value;
			}
		}
		#endregion
		bool UseCompressedHeaders(DayOfWeek[] daysOfWeek, bool compressWeekend) {
			if(!compressWeekend)
				return false;
			int sundayIndex = DateTimeHelper.FindDayOfWeekIndex(daysOfWeek, DayOfWeek.Sunday);
			int saturdayIndex = DateTimeHelper.FindDayOfWeekIndex(daysOfWeek, DayOfWeek.Saturday);
			return sundayIndex >= 0 && saturdayIndex >= 0;
		}
		DayOfWeekHeaderCollection CreateCompressedDayHeaders(DayOfWeek[] daysOfWeek) {
			DayOfWeekHeaderCollection result = new DayOfWeekHeaderCollection();
			int count = daysOfWeek.Length;
			for(int i = 0; i < count; i++) {
				DayOfWeek dayOfWeek = daysOfWeek[i];
				if (dayOfWeek == DayOfWeek.Sunday)
					continue;
				bool isCompressed = dayOfWeek == DayOfWeek.Saturday;
				result.Add(new DayOfWeekHeader( dayOfWeek, isCompressed, Resource, Brushes));
			}
			return result;
		}
		DayOfWeekHeaderCollection CreateOrdinaryDayHeaders(DayOfWeek[] daysOfWeek) {
			DayOfWeekHeaderCollection result = new DayOfWeekHeaderCollection();
			int count = daysOfWeek.Length;
			for(int i = 0; i < count; i++)
				result.Add(new DayOfWeekHeader( daysOfWeek[i], false, Resource, Brushes));
			return result;
		}
	}
	#endregion
	#region DayOfWeekHeader
	public class DayOfWeekHeader : ResourceCellBase {
		DayOfWeek dayOfWeek;
		string caption;
		bool isCompressed;
		public DayOfWeekHeader( DayOfWeek dayOfWeek, bool isCompressed, Resource resource, ResourceBrushes brushes)
			: base(resource, brushes) {
			this.dayOfWeek = dayOfWeek;
			this.isCompressed = isCompressed;
		}
		public DayOfWeek DayOfWeek { 
			get { return dayOfWeek; }
			private set {
				if(DayOfWeek == value)
					return;
				dayOfWeek = value;
			}
		}
		public bool IsCompressed {
			get { return isCompressed; }
			private set {
				if (IsCompressed == value)
					return;
				isCompressed = value;
			}
		}
		public string Caption {
			get { return caption; }
			set {
				if (caption == value)
					return;
				this.caption = value;
			}
		}	   
	}
	#endregion
}
