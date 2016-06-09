#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	public class ImageOpenHistoryItem : DashboardItemHistoryItem<ImageDashboardItem> {
		class LoadImageState {
			readonly string uri;
			readonly byte[] data;
			public string Url { get { return uri; } }
			public byte[] Data { get { return data; } }
			public LoadImageState (string uri, byte[] data) {
				this.uri = uri;
				this.data = data;
			}
		}
		readonly LoadImageState prev;
		readonly LoadImageState next;
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemImageOpen; } }
		public ImageOpenHistoryItem(ImageDashboardItem dashboardItem, string uri, byte[] data) : base(dashboardItem) {
			prev = new LoadImageState(dashboardItem.Url, dashboardItem.Data);
			next = new LoadImageState(uri, data);
		}
		void ApplyState(LoadImageState state) {
			ImageDashboardItem dashboardItem = DashboardItem;
			dashboardItem.Data = state.Data;
			dashboardItem.Url = state.Url;
		}
		protected override void PerformUndo() {
			ApplyState(prev);
		}
		protected override void PerformRedo() {
			ApplyState(next);
		}
	}
}
