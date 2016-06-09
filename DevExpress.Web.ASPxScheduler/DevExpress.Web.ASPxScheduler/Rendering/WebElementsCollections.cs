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

using System.Collections.Generic;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.Web.ASPxScheduler.Drawing;
namespace DevExpress.Web.ASPxScheduler.Rendering {
	public class WebTimeRulerMinuteItemCollection : List<WebTimeRulerMinuteItem> {
	}
	public class WebTimeRulerElementCollection : List<WebTimeRulerElement> {
	}
	public class WebTimeRulerCollection : List<WebTimeRuler> {
	}
	public class WebDayViewColumnCollection : List<WebDayViewColumn> {
	}
	public class WebTimeCellCollection : SchedulerViewCellBaseCollectionCore<WebTimeCell>, IWebTimeIntervalCollection {
		#region ITimeIntervalCollection Members
		public override ITimeIntervalCollection CreateEmptyCollection() {
			return new WebTimeCellCollection();
		}
		#endregion
		#region IWebTimeIntervalCollection Members
		public string GetIdByIndex(int index) {
			if(index >= Count)
				Exceptions.ThrowArgumentException("index", index);
			return this[index].Cell.ID;
		}
		IWebTimeCell IWebTimeIntervalCollection.this[int index] { get { return this[index]; } }
		#endregion
	}
	public class SchedulerWebViewInfoCollection : List<IWebViewInfo> {
	}
	public class WebDayOfWeekHeaderCollection : List<WebDayOfWeekHeader> {
		internal WebDayOfWeekHeaderCollection GetItemsWithDefaultContent() {
			WebDayOfWeekHeaderCollection collection = new WebDayOfWeekHeaderCollection();
			for(int i = 0; i < Count; i++) {
				WebDayOfWeekHeader header = this[i];
				if (header.UseDefaultContent)
					collection.Add(header);
			}
			return collection;
		}
	}
	public class WebSeparatorCollection : List<WebGroupSeparator> {
	}
	public class WebHorizontalResourceHeaderCollection : List<WebHorizontalResourceHeader> {
	}
	public class WebDateCellCollection : SchedulerViewCellBaseCollectionCore<WebDateCell>, IWebTimeIntervalCollection {
		#region ITimeIntervalCollection Members
		public override ITimeIntervalCollection CreateEmptyCollection() {
			return new WebDateCellCollection();
		}
		#endregion
		#region IWebTimeIntervalCollection Members
		public string GetIdByIndex(int index) {
			if (index >= Count)
				Exceptions.ThrowArgumentException("index", index);
			return this[index].Content.Cell.ID;
		}
		IWebTimeCell IWebTimeIntervalCollection.this[int index] { get { return this[index]; } }
	#endregion
	}
	public class WebWeekCollection : List<WebWeekBase> {
	}
	public class WebAllDayAreaCellCollection : SchedulerViewCellBaseCollectionCore<WebAllDayAreaCell>, IWebTimeIntervalCollection {
		public override ITimeIntervalCollection CreateEmptyCollection() {
			return new WebAllDayAreaCellCollection();
		}
		#region IWebTimeIntervalCollection Members
		public string GetIdByIndex(int index) {
			if(index >= Count)
				Exceptions.ThrowArgumentException("index", index);
			return this[index].Cell.ID;
		}
		IWebTimeCell IWebTimeIntervalCollection.this[int index] { get { return this[index]; } }
		#endregion
	}
	public class WebCellContainerCollection : List<IWebCellContainer> {
	}
}
