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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
namespace DomainComponents.Common {
	[DomainComponent]
	[ImageName("BO_Scheduler")]
	[XafDefaultProperty("Subject")]
	[DefaultListViewOptions(true, NewItemRowPosition.None)]
	public interface IPersistentEvent : IEvent {
		new string Subject { get; set; }
		[FieldSize(4000)]
		new string Description { get; set; }
		[XafDisplayName("Start Date/Time"), ModelDefault("DisplayFormat", "{0:G}"), ModelDefault("EditMask", "G")]
		new DateTime StartOn { get; set; }
		[XafDisplayName("End Date/Time"), ModelDefault("DisplayFormat", "{0:G}"), ModelDefault("EditMask", "G")]
		new DateTime EndOn { get; set; }
		new bool AllDay { get; set; }
		new string Location { get; set; }
		new int Label { get; set; }
		new int Status { get; set; }
		[Browsable(false)]
		new int Type { get; set; }
		[FieldSize(4000)]
		[Browsable(false)]
		new string ResourceId { get; set; }
		IList<IPersistentResource> Resources { get; }
	}
	[DomainLogic(typeof(IPersistentEvent))]
	public class PersistentEventLogic {
		public static object Get_AppointmentId(IPersistentEvent persistentEvent, IObjectSpace objectSpace) {
			return objectSpace.GetKeyValue(persistentEvent);
		}
	}
	[DomainComponent]
	[DefaultListViewOptions(true, NewItemRowPosition.None)]
	public interface IPersistentRecurrentEvent : IPersistentEvent, IRecurrentEvent {
		IPersistentRecurrentEvent Pattern { get; set; }
		[EditorAlias(EditorAliases.SchedulerRecurrenceInfoPropertyEditor)]
		[XafDisplayName("Recurrence"), FieldSize(FieldSizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
		new string RecurrenceInfoXml { get; set; }
	}
	[DomainLogic(typeof(IPersistentRecurrentEvent))]
	public class IPersistentRecurrentEventLogic {
		public IRecurrentEvent Get_RecurrencePattern(IPersistentRecurrentEvent instance) {
			return instance.Pattern;
		}
		public void Set_RecurrencePattern(IPersistentRecurrentEvent instance, IRecurrentEvent value) {
			instance.Pattern = (IPersistentRecurrentEvent)value;
		}
	}
	[DomainComponent]
	[XafDefaultProperty("Caption")]
	public interface IPersistentResource : IResource {
		new string Caption { get; set; }
		IList<IPersistentEvent> Events { get; }
		[ValueConverter(typeof(ColorToIntConverter))]
		Color Color { get; set; }
	}
	[DomainLogic(typeof(IPersistentResource))]
	public class IPersistentResource_Logic {
		public static void AfterConstruction(IPersistentResource obj) {
			obj.Color = Color.White;
		}
		public static object Get_Id(IPersistentResource obj, IObjectSpace objectSpace) {
			return objectSpace.GetKeyValue(obj);
		}
		public static Int32 Get_OleColor(IPersistentResource obj) {
			return ColorTranslator.ToOle(obj.Color);
		}
	}
	public class ColorToIntConverter : ValueConverter {
		public override object ConvertFromStorageType(object obj) {
			if(obj == null) {
				return null;
			}
			return Color.FromArgb((Int32)obj);
		}
		public override object ConvertToStorageType(object obj) {
			if(obj == null) {
				return null;
			}
			return ((Color)obj).ToArgb();
		}
		public override Type StorageType {
			get { return typeof(Int32); }
		}
	}
}
