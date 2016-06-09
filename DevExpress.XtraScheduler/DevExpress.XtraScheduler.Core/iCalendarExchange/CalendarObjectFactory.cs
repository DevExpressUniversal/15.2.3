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
using System.Text;
using System.Collections;
using DevExpress.XtraScheduler.iCalendar.Components;
using DevExpress.XtraScheduler.iCalendar.Native;
using System.Reflection;
namespace DevExpress.XtraScheduler.iCalendar.Native {
	public static class iCalendarObjectFactory {
		internal static Dictionary<string, Type> supportedTypes = new Dictionary<string, Type>();
		static iCalendarObjectFactory() {
			supportedTypes = new Dictionary<string, Type>();
			RegisterObjectTypes();
		}
		#region Supported types registration
		private static void RegisterObjectTypes() {
			supportedTypes[iCalendarComponent.TokenName] = typeof(iCalendarComponent);
			RegisterCalendarProperties();
			RegisterComponents();
			RegisterEventProperties();
			RegisterTimeZoneProperties();
			RegisterAlarmProperties();
		}
		private static void RegisterAlarmProperties() {
			supportedTypes[TriggerProperty.TokenName] = typeof(TriggerProperty);
		}
		private static void RegisterCalendarProperties() {
			supportedTypes[CalendarScaleProperty.TokenName] = typeof(CalendarScaleProperty);
			supportedTypes[MethodProperty.TokenName] = typeof(MethodProperty);
			supportedTypes[ProductIdentifierProperty.TokenName] = typeof(ProductIdentifierProperty);
			supportedTypes[VersionProperty.TokenName] = typeof(VersionProperty);
		}
		private static void RegisterComponents() {
			supportedTypes[VEvent.TokenName] = typeof(VEvent);
			supportedTypes[VTimeZone.TokenName] = typeof(VTimeZone);
			supportedTypes[TimeZoneDaylight.TokenName] = typeof(TimeZoneDaylight);
			supportedTypes[TimeZoneStandard.TokenName] = typeof(TimeZoneStandard);
			supportedTypes[VAlarm.TokenName] = typeof(VAlarm);
		}
		private static void RegisterEventProperties() {
			supportedTypes[DateTimeCreatedProperty.TokenName] = typeof(DateTimeCreatedProperty);
			supportedTypes[DateTimeStampProperty.TokenName] = typeof(DateTimeStampProperty);
			supportedTypes[LastModifiedProperty.TokenName] = typeof(LastModifiedProperty);
			supportedTypes[DateTimeStartProperty.TokenName] = typeof(DateTimeStartProperty);
			supportedTypes[DateTimeEndProperty.TokenName] = typeof(DateTimeEndProperty);
			supportedTypes[UniqueIdentifierProperty.TokenName] = typeof(UniqueIdentifierProperty);
			supportedTypes[DescriptionProperty.TokenName] = typeof(DescriptionProperty);
			supportedTypes[LocationProperty.TokenName] = typeof(LocationProperty);
			supportedTypes[SummaryProperty.TokenName] = typeof(SummaryProperty);
			supportedTypes[RecurrenceRuleProperty.TokenName] = typeof(RecurrenceRuleProperty);
			supportedTypes[ExceptionRuleProperty.TokenName] = typeof(ExceptionRuleProperty);
			supportedTypes[ExceptionDateTimesProperty.TokenName] = typeof(ExceptionDateTimesProperty);
			supportedTypes[RecurrenceIdProperty.TokenName] = typeof(RecurrenceIdProperty);
			supportedTypes[CategoriesProperty.TokenName] = typeof(CategoriesProperty);
		}
		private static void RegisterTimeZoneProperties() {
			supportedTypes[TimeZoneIdentifierProperty.TokenName] = typeof(TimeZoneIdentifierProperty);
			supportedTypes[TimeZoneNameProperty.TokenName] = typeof(TimeZoneNameProperty);
			supportedTypes[TimeZoneOffsetFromProperty.TokenName] = typeof(TimeZoneOffsetFromProperty);
			supportedTypes[TimeZoneOffsetToProperty.TokenName] = typeof(TimeZoneOffsetToProperty);
			supportedTypes[RecurrenceDateTimeProperty.TokenName] = typeof(RecurrenceDateTimeProperty);
		}
		#endregion
		public static iCalendarBodyItem CreateBodyItem(string name, iContentLineParameters parameters, string value) {
			if (name == "BEGIN")
				return CreateComponent(value, new string[0]) as iCalendarBodyItem;
			if (name == "END")
				return new iCalendarComponentEndItem();
			return CreateProperty(name, parameters, value);
		}
		private static iCalendarBodyItem CreateProperty(string name, iContentLineParameters parameters, string value) {
			Type resultType = CalculateObjectType(name);
			if (resultType == null)
				return new CustomProperty(name, parameters, value);
			try {
				return (iCalendarBodyItem)Activator.CreateInstance(resultType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { parameters, value }, null);
			} catch (TargetInvocationException e) {
				throw e.InnerException;
			}
		}
		static iCalendarObject CreateComponent(string name, string[] args) {
			if (string.IsNullOrEmpty(name))
				return null;
			Type resultType = CalculateObjectType(name);
			return resultType != null ? (iCalendarObject)Activator.CreateInstance(resultType, args) :
				new CustomComponent(name);
		}
		internal static Type CalculateObjectType(string tokenValue) {
			return supportedTypes.ContainsKey(tokenValue) ? (Type)supportedTypes[tokenValue] : null;
		}
	}
}
