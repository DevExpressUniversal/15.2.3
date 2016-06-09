#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.ExpressApp.SystemModule.Notifications {
	[DomainComponent, DefaultProperty("RemindInText")]
	public class PostponeTime {
		private string defaultCaption;
		public static IList<PostponeTime> CreateDefaultPostponeTimesList() {
			return new List<PostponeTime> { 
				new PostponeTime("FiveMinutes", TimeSpan.FromMinutes(5), "5 minutes"),
				new PostponeTime("TenMinutes", TimeSpan.FromMinutes(10),"10 minutes"),
				new PostponeTime("FifteenMinutes", TimeSpan.FromMinutes(15),"15 minutes"),
				new PostponeTime("ThirtyMinutes", TimeSpan.FromMinutes(30),"30 minutes"),
				new PostponeTime("OneHour", TimeSpan.FromHours(1),"1 hour"),
				new PostponeTime("TwoHours", TimeSpan.FromHours(2),"2 hours"),
				new PostponeTime("ThreeHours", TimeSpan.FromHours(3),"3 hours"),
				new PostponeTime("FourHours", TimeSpan.FromHours(4),"4 hours"),
				new PostponeTime("FiveHours", TimeSpan.FromHours(5),"5 hours"),
				new PostponeTime("SixHours", TimeSpan.FromHours(6),"6 hours"),
				new PostponeTime("SevenHours", TimeSpan.FromHours(7),"7 hours"),
				new PostponeTime("EightHours", TimeSpan.FromHours(8),"8 hours"),
				new PostponeTime("NineHours", TimeSpan.FromHours(9),"9 hours"),
				new PostponeTime("TenHours", TimeSpan.FromHours(10),"10 hours"),
				new PostponeTime("ElevenHours", TimeSpan.FromHours(11),"11 hiurs"),
				new PostponeTime("HalfDay", TimeSpan.FromHours(12),"0,5 days"),
				new PostponeTime("OneDay", TimeSpan.FromDays(1),"1 day"),
				new PostponeTime("TwoDays", TimeSpan.FromDays(2),"2 days"),
				new PostponeTime("ThreeDays", TimeSpan.FromDays(3),"3 days"),
				new PostponeTime("FourDays", TimeSpan.FromDays(4),"4 days"),
				new PostponeTime("OneWeek", TimeSpan.FromDays(7),"1 week"),
				new PostponeTime("TwoWeeks", TimeSpan.FromDays(14),"2 weeks")
			};
		}
		public static void SortPostponeTimesList(IList<PostponeTime> result){
		 ((List<PostponeTime>)result).Sort(delegate(PostponeTime a, PostponeTime b) { 
				if(!a.RemindIn.HasValue) {
					return b.RemindIn.HasValue ? -1 : 0;
				}
				return b.RemindIn.HasValue ? a.RemindIn.Value.CompareTo(b.RemindIn.Value) : 1; 
			});
		}
		public PostponeTime(string Id, TimeSpan? remindIn, string defaultCaption) {
			this.ID = Id;
			this.RemindIn = remindIn;
			this.defaultCaption = defaultCaption;
		}
		[XafDisplayName("Remind In")]
		public string RemindInText {
			get {
				return CaptionHelper.GetLocalizedText("NotificationsPostponeTimesList", ID, defaultCaption);
			}
		}
		public string ID { get; private set; }
		public TimeSpan? RemindIn { get; private set; }
	}
	public class PostponeTimeListViewController : ObjectViewController<ListView, PostponeTime> {
		protected override void OnViewControlsCreated() {
			base.OnViewControlsCreated();
			UpdateListViewColumnOptions();
		}
		protected virtual void UpdateListViewColumnOptions() {
			ColumnsListEditor columnsListEditor = View.Editor as ColumnsListEditor;
			if(columnsListEditor != null) {
				foreach(ColumnWrapper columnWrapper in columnsListEditor.Columns) {
					columnWrapper.AllowSortingChange = false;
				}
			}
		}
	}
}
