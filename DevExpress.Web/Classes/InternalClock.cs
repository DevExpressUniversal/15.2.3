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

using System.ComponentModel;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System.Web.UI;
namespace DevExpress.Web.Internal {
	[ToolboxItem(false)]
	public class InternalClock : ASPxWebControl {
		internal const string InternalClockScriptResourceName = WebScriptsResourcePath + "InternalClock.js";
		const string 
			ClockFaceID = "D",
			HourHandID = "H",
			MinuteHandID = "M",
			SecondHandID = "S";
		Image clockFace, hourHand, minuteHand, secondHand;
		ImageProperties clockFaceImage, hourHandImage, minuteHandImage, secondHandImage;
		public InternalClock()
			: base() {
		}
		public bool ShowHourHand {
			get { return GetBoolProperty("ShowHourHand", true); }
			set {
				if(ShowHourHand != value) {
					SetBoolProperty("ShowHourHand", true, value);
					LayoutChanged();
				}
			}
		}
		public bool ShowMinuteHand {
			get { return GetBoolProperty("ShowMinuteHand", true); }
			set { 
				if(ShowMinuteHand != value) {
					SetBoolProperty("ShowMinuteHand", true, value);
					LayoutChanged();
				}
			}
		}
		public bool ShowSecondHand {
			get { return GetBoolProperty("ShowSecondHand", true); }
			set {
				if(ShowSecondHand != value) {
					SetBoolProperty("ShowSecondHand", true, value); 
					LayoutChanged();
				}
			}
		}
		public ImageProperties ClockFaceImage {
			get {
				if(clockFaceImage == null)
					clockFaceImage = new ImageProperties();
				return clockFaceImage; 
			}
		}
		public ImageProperties HourHandImage {
			get {
				if(hourHandImage == null)
					hourHandImage = new ImageProperties();
				return hourHandImage;
			}
		}
		public ImageProperties MinuteHandImage {
			get {
				if(minuteHandImage == null)
					minuteHandImage = new ImageProperties();
				return minuteHandImage;
			}
		}
		public ImageProperties SecondHandImage {
			get {
				if(secondHandImage == null)
					secondHandImage = new ImageProperties();
				return secondHandImage;
			}
		}
		protected Image ClockFace { get { return clockFace; } }
		protected Image HourHand { get { return hourHand; } }
		protected Image MinuteHand { get { return minuteHand; } }
		protected Image SecondHand { get { return secondHand; } }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected override bool HasRootTag() {
			return true;
		}
		protected override bool HasClientInitialization() {
			return true;
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.InternalClock";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(InternalClock), InternalClockScriptResourceName);
		}
		protected override bool HasRenderCssFile() {
			return false;
		}
		protected override void CreateControlHierarchy() {
			this.clockFace = CreateImage(ClockFaceID);
			if(ShowHourHand)
				this.hourHand = CreateImage(HourHandID);
			if(ShowMinuteHand)
				this.minuteHand = CreateImage(MinuteHandID);
			if(ShowSecondHand)
				this.secondHand = CreateImage(SecondHandID);
		}
		protected Image CreateImage(string id) {
			Image img = RenderUtils.CreateImage();
			img.ID = id;
			Controls.Add(img);
			return img;
		}
		protected override void PrepareControlHierarchy() {
			ClockFaceImage.AssignToControl(ClockFace, DesignMode);
			if(HourHand != null)
				HourHandImage.AssignToControl(HourHand, DesignMode);
			if(MinuteHand != null)
				MinuteHandImage.AssignToControl(MinuteHand, DesignMode);
			if(SecondHand != null)
				SecondHandImage.AssignToControl(SecondHand, DesignMode);
		}
	}
}
