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
using System.ComponentModel;
using System.Drawing;
namespace DevExpress.XtraBars.ToastNotifications {
	[TypeConverter("DevExpress.XtraBars.Design.ToastNotificationTypeConverter, " + AssemblyInfo.SRAssemblyBarsDesign)]
	public class ToastNotification : Docking2010.Base.BaseProperties, IToastNotificationProperties {
		public ToastNotification() {
			SetDefaultValueCore<ToastNotificationTemplate>("Template", ToastNotificationTemplate.Text01);
		}
		public ToastNotification(object id, Image image, string header, string body, string body2, ToastNotificationTemplate template) {
			ID = id;
			Image = image;
			Header = header;
			Body = body;
			Body2 = body2;
			Template = template;
		}
		[DefaultValue(ToastNotificationTemplate.Text01)]
		public ToastNotificationTemplate Template {
			get { return GetValue<ToastNotificationTemplate>("Template"); }
			set { SetValueCore("Template", value); }
		}
		[DefaultValue(ToastNotificationDuration.Default)]
		public ToastNotificationDuration Duration {
			get { return GetValue<ToastNotificationDuration>("Duration"); }
			set { SetValueCore("Duration", value); }
		}
		[DefaultValue(ToastNotificationSound.Default)]
		public ToastNotificationSound Sound {
			get { return GetValue<ToastNotificationSound>("Sound"); }
			set { SetValueCore("Sound", value); }
		}
		public object ID {
			get { return GetValue<object>("ID"); }
			set { SetValueCore("ID", value); }
		}
		[DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public Image Image {
			get { return GetValue<Image>("Image"); }
			set { SetValueCore("Image", value); }
		}
		[DefaultValue(null)]
		public string Header {
			get { return GetValue<string>("Header"); }
			set { SetValueCore("Header", value); }
		}
		[DefaultValue(null)]
		public string Body {
			get { return GetValue<string>("Body"); }
			set { SetValueCore("Body", value); }
		}
		[DefaultValue(null)]
		public string Body2 {
			get { return GetValue<string>("Body2"); }
			set { SetValueCore("Body2", value); }
		}
		protected override DevExpress.Utils.Base.IBaseProperties CloneCore() {
			return new ToastNotification();
		}
		[Browsable(false)]
		public bool HasImage {
			get { return IsImageTempate(Template) && !IsDefault("Image"); }
		}
		bool IsImageTempate(ToastNotificationTemplate template) {
			return
				((int)template >= (int)ToastNotificationTemplate.ImageAndText01) &&
				((int)template <= (int)ToastNotificationTemplate.ImageAndText04);
		}
	}
	public class ToastNotificationCollection : Docking2010.Base.BaseMutableListEx<IToastNotificationProperties> {
		IDictionary<object, IToastNotificationProperties> mapping;
		public ToastNotificationCollection() {
			mapping = new Dictionary<object, IToastNotificationProperties>();
		}
		protected override bool CanAdd(IToastNotificationProperties element) {
			return !mapping.ContainsKey(element.ID) && base.CanAdd(element);
		}
		protected override void OnElementAdded(IToastNotificationProperties element) {
			mapping.Add(element.ID, element);
			base.OnElementAdded(element);
		}
		protected override void OnElementRemoved(IToastNotificationProperties element) {
			base.OnElementRemoved(element);
			mapping.Remove(element.ID);
		}
		public bool TryGetValue(object id, out IToastNotificationProperties element) {
			return mapping.TryGetValue(id, out element);
		}
	}
}
