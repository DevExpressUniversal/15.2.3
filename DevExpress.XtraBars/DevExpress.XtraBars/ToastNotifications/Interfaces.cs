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

using System.Drawing;
namespace DevExpress.XtraBars.ToastNotifications {
	public enum ToastNotificationDuration {
		Default = 0,
		Long = 1,
	}
	public enum ToastNotificationTemplate {
		ImageAndText01 = 0,
		ImageAndText02 = 1,
		ImageAndText03 = 2,
		ImageAndText04 = 3,
		Text01 = 4,
		Text02 = 5,
		Text03 = 6,
		Text04 = 7,
	}
	public enum ToastNotificationSound {
		Default = 0,
		NoSound = 1,
		IM = 2,
		Mail = 3,
		Reminder = 4,
		SMS = 5,
		Looping_Alarm = 6,
		Looping_Alarm2 = 7,
		Looping_Alarm3 = 8,
		Looping_Alarm4 = 9,
		Looping_Alarm5 = 10,
		Looping_Alarm6 = 11,
		Looping_Alarm7 = 12,
		Looping_Alarm8 = 13,
		Looping_Alarm9 = 14,
		Looping_Alarm10 = 15,
		Looping_Call = 16,
		Looping_Call2 = 17,
		Looping_Call3 = 18,
		Looping_Call4 = 19,
		Looping_Call5 = 20,
		Looping_Call6 = 21,
		Looping_Call7 = 22,
		Looping_Call8 = 23,
		Looping_Call9 = 24,
		Looping_Call10 = 25,
	}
	public interface IToastNotificationProperties : DevExpress.Utils.Base.IBaseProperties {
		ToastNotificationTemplate Template { get; set; }
		ToastNotificationDuration Duration { get; set; }
		ToastNotificationSound Sound { get; set; }
		object ID { get; set; }
		Image Image { get; set; }
		string Header { get; set; }
		string Body { get; set; }
		string Body2 { get; set; }
		bool HasImage { get; }
	}
}
