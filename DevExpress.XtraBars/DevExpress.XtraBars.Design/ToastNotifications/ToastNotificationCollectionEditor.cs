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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using DevExpress.XtraBars.ToastNotifications;
namespace DevExpress.XtraBars.Design {
	public class ToastNotificationCollectionEditor : DevExpress.Utils.Design.DXCollectionEditorBase {
		public ToastNotificationCollectionEditor(Type type)
			: base(type) {
		}
		protected override Type CreateCollectionItemType() {
			return typeof(ToastNotification);
		}
		protected override Type[] CreateNewItemTypes() {
			return new Type[] { typeof(ToastNotification) };
		}
		protected override object CreateCustomInstance(Type itemType) {
			return ToastNotificationsManagerDesigner.CreateDefaultNotification(ToastNotificationTemplate.Text01);
		}
		protected override string GetDisplayText(object value) {
			ToastNotification element = value as ToastNotification;
			if(element == null) return base.GetDisplayText(value);
			return "Toast" + element.Template.ToString();
		}
	}
	public sealed class ToastNotificationTypeConverter : ExpandableObjectConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor)) return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null)
				throw new ArgumentNullException("destinationType");
			if(destinationType == typeof(InstanceDescriptor) && (value is ToastNotification)) {
				ConstructorInfo ctor = typeof(ToastNotification).GetConstructor(new Type[] { 
						typeof(object), typeof(Image), typeof(string), typeof(string), typeof(string), typeof(ToastNotificationTemplate) });
				if(ctor != null) {
					ToastNotification toast = (ToastNotification)value;
					return new InstanceDescriptor(ctor, new object[] { 
						toast.ID, toast.Image, toast.Header, toast.Body, toast.Body2, toast.Template });
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
