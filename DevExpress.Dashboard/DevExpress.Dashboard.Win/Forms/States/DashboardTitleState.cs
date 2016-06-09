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

using System;
using DevExpress.DashboardCommon;
namespace DevExpress.DashboardWin.Native {
	public class DashboardTitleState {
		public bool Visible { get; set; }
		public string Text { get; set; }
		public DashboardTitleAlignment Alignment { get; set; }
		public byte[] ImageData { get; set; }
		public string ImageUrl { get; set; }
		public bool IncludeMasterFilterState { get; set; }
		public DashboardTitleState() {
		}
		public DashboardTitleState(DashboardTitle title) {
			Visible = title.Visible;
			Text = title.Text;
			Alignment = title.Alignment;
			ImageUrl = title.ImageUrl;
			ImageData = title.ImageData;
			IncludeMasterFilterState = title.ShowMasterFilterState;
		}
		public void AssignTo(DashboardTitle title) {
			title.Visible = Visible;
			title.Text = Text;
			title.Alignment = Alignment;
			if(string.IsNullOrEmpty(ImageUrl) && ImageData == null) {
				title.ImageUrl = null;
				title.ImageData = null;
			}
			if(!string.IsNullOrEmpty(ImageUrl))
				title.ImageUrl = ImageUrl;
			if(ImageData != null)
				title.ImageData = ImageData;
			title.ShowMasterFilterState = IncludeMasterFilterState;
		}
		public override bool Equals(object obj) {
			DashboardTitleState state = obj as DashboardTitleState;
			if(state == null)
				return false;
			return state.Visible == Visible &&
				state.Text == Text &&
				state.Alignment == Alignment &&
				state.ImageData == ImageData &&
				state.ImageUrl == ImageUrl &&
				state.IncludeMasterFilterState == IncludeMasterFilterState;
		}
		public override int GetHashCode() {
			return Text != null ? Text.GetHashCode() : 0;
		}
	}
}
