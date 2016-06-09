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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using DevExpress.XtraScheduler;
using System.Text.RegularExpressions;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.iCalendar.Native;
using DevExpress.Utils;
using DevExpress.XtraScheduler.iCalendar.Internal;
namespace DevExpress.XtraScheduler.iCalendar.Components {
	public class iCalendarContainer : ICalendarBodyItemContainer {
		iCalendarCollection calendars;
		public iCalendarContainer() {
			calendars = new iCalendarCollection();
		}
		public iCalendarComponent this[int index] { get { return Calendars[index]; } }
		public int Count { get { return Calendars.Count; } }
		protected iCalendarCollection Calendars { get { return calendars; } }
		#region ICalendarObjectContainer Members
		public void AddObject(iCalendarBodyItem item) {
			iCalendarComponent component = item as iCalendarComponent;
			if (component != null)
				Calendars.Add(component);
		}
		#endregion
		public void Clear() {
			Calendars.Clear();
		}
	}
	public class iCalendarCollection : DXCollection<iCalendarComponent> {
	}
	public class iCalendarComponent : iCalendarComponentBase {
		public const string TokenName = "VCALENDAR";
		static readonly Dictionary<Type, ObtainPropertyDelegate> supportedPropertyHandlers = new Dictionary<Type, ObtainPropertyDelegate>();
		static readonly Dictionary<Type, ObtainComponentDelegate> supportedComponentHandlers = new Dictionary<Type, ObtainComponentDelegate>();
		static iCalendarComponent() {
			supportedPropertyHandlers.Add(typeof(CalendarScaleProperty), AddCalendarScale);
			supportedPropertyHandlers.Add(typeof(MethodProperty), AddMethod);
			supportedPropertyHandlers.Add(typeof(ProductIdentifierProperty), AddProductIdentifier);
			supportedPropertyHandlers.Add(typeof(VersionProperty), AddVersion);
			supportedComponentHandlers.Add(typeof(VEvent), AddEvent);
			supportedComponentHandlers.Add(typeof(VTimeZone), AddTimeZone);
		}
		#region Fields
		CalendarScaleProperty calscale;
		MethodProperty method;
		ProductIdentifierProperty prodId;
		VersionProperty version;
		VEventCollection events;
		VTimeZoneCollection timeZones;
		TimeZoneInfo clientTimeZone;
		int sourceObjectCount = -1;
		#endregion
		#region ctor
		public iCalendarComponent() : this(0) {
		}
		public iCalendarComponent(int eventCapacity) {
			this.events = eventCapacity < 1 ? new VEventCollection() : new VEventCollection(eventCapacity);
			this.timeZones = new VTimeZoneCollection();
			this.calscale = new CalendarScaleProperty(null, string.Empty);
			this.method = new MethodProperty(null, string.Empty);
			this.prodId = new ProductIdentifierProperty(null, DevExpress.XtraScheduler.VCalendar.VCalendarConsts.DefaultCalendarProductID);
			this.version = new VersionProperty(null, DevExpress.XtraScheduler.VCalendar.VCalendarConsts.VCalendar20Version);
			this.clientTimeZone = TimeZoneEngine.Local;
		}
		#endregion
		#region Properties
		public TimeZoneInfo ClientTimeZone {
			get { return clientTimeZone; }
			set {
				if (value != null)
					clientTimeZone = value;
			}
		}
		public int SourceObjectCount {
			get {
				if (sourceObjectCount == -1)
					sourceObjectCount = CalculateSourceObjectCount();
				return sourceObjectCount; ;
			}
		}
		public CalendarScaleProperty Calscale { get { return calscale; } }
		public MethodProperty Method { get { return method; } }
		public ProductIdentifierProperty ProductIdentifier { get { return prodId; } }
		public VersionProperty Version { get { return version; } }
		public VEventCollection Events { get { return events; } }
		public VTimeZoneCollection TimeZones { get { return timeZones; } }
		protected internal override Dictionary<Type, ObtainComponentDelegate> SupportedComponentHandlers {
			get { return supportedComponentHandlers; }
		}
		protected internal override Dictionary<Type, ObtainPropertyDelegate> SupportedPropertyHandlers {
			get { return supportedPropertyHandlers; }
		}
		public override string Name { get { return TokenName; } }
		#endregion
		#region Supported Property Handlers
		static void AddCalendarScale(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((iCalendarComponent)container).calscale = value as CalendarScaleProperty;
		}
		static void AddMethod(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((iCalendarComponent)container).method = value as MethodProperty;
		}
		static void AddProductIdentifier(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((iCalendarComponent)container).prodId = value as ProductIdentifierProperty;
		}
		static void AddVersion(ICalendarBodyItemContainer container, iCalendarPropertyBase value) {
			((iCalendarComponent)container).version = value as VersionProperty;
		}
		#endregion
		#region Supported Component Handlers
		static void AddEvent(ICalendarBodyItemContainer container, iCalendarComponentBase value) {
			((iCalendarComponent)container).Events.Add((VEvent)value);
		}
		static void AddTimeZone(ICalendarBodyItemContainer container, iCalendarComponentBase value) {
			((iCalendarComponent)container).TimeZones.Add((VTimeZone)value);
		}
		#endregion
		#region ApplyTimeZone
		protected override void ApplyTimeZone(TimeZoneManager timeZoneManager) {
			int customComponentCount = CustomComponents.Count;
			for (int i = 0; i < customComponentCount; i++) {
				ISupportCalendarTimeZone impl = CustomComponents[i] as ISupportCalendarTimeZone;
				if (impl == null)
					continue;
				impl.ApplyTimeZone(timeZoneManager);
			}
			int eventCount = Events.Count;
			for (int i = 0; i < eventCount; i++) {
				ISupportCalendarTimeZone impl = Events[i] as ISupportCalendarTimeZone;
				if (impl == null)
					continue;
				impl.ApplyTimeZone(timeZoneManager);
			}
		}
		#endregion
		#region CalculateSourceObjectCount
		protected virtual internal int CalculateSourceObjectCount() {
			int result = 0;
			int count = Events.Count;
			for (int i = 0; i < count; i++) {
				result += Events[i].CalculateSourceObjectCount();
			}
			return result;
		}
		#endregion
		#region WriteComponents
		protected override void WriteComponents(iCalendarWriter cw) {
			WriteTimeZonesToStrem(cw);
			WriteEventsToStream(cw);
		}
		#endregion
		#region WriteProperties
		protected override void WriteProperties(iCalendarWriter cw) {
			ProductIdentifier.WriteToStream(cw);
			Version.WriteToStream(cw);
			Method.WriteToStream(cw);
		}
		#endregion
		#region WriteEventsToStream
		protected void WriteEventsToStream(iCalendarWriter cw) {
			int count = Events.Count;
			for (int i = 0; i < count; i++)
				Events[i].WriteToStream(cw);
		}
		#endregion
		#region WriteTimeZonesToStrem
		protected void WriteTimeZonesToStrem(iCalendarWriter cw) {
			int count = TimeZones.Count;
			for (int i = 0; i < count; i++)
				TimeZones[i].WriteToStream(cw);
		}
		#endregion
	}
	public class CalendarScaleProperty : StringPropertyBase {
		public const string TokenName = "CALSCALE";
		internal CalendarScaleProperty(iContentLineParameters parameters, string value)
			: base(parameters, value) {
		}
		public override string Name { get { return TokenName; } }
		#region WriteParametersToStream
		protected override void WriteParametersToStream(iCalendarWriter cw) {
		}
		#endregion
	}
	public class MethodProperty : StringPropertyBase {
		public const string TokenName = "METHOD";
		internal MethodProperty(iContentLineParameters parameters, string value)
			: base(parameters, value) {
		}
		public override string Name { get { return TokenName; } }
		#region WriteParametersToStream
		protected override void WriteParametersToStream(iCalendarWriter cw) {
		}
		#endregion
	}
	public class ProductIdentifierProperty : StringPropertyBase {
		public const string TokenName = "PRODID";
		internal ProductIdentifierProperty(iContentLineParameters parameters, string value)
			: base(parameters, value) {
		}
		public override string Name { get { return TokenName; } }
		#region WriteParametersToStream
		protected override void WriteParametersToStream(iCalendarWriter cw) {
		}
		#endregion
	}
	public class VersionProperty : StringPropertyBase {
		public const string TokenName = "VERSION";
		public VersionProperty(iContentLineParameters parameters, string value)
			: base(parameters, value) {
		}
		public override string Name { get { return TokenName; } }
		#region WriteParametersToStream
		protected override void WriteParametersToStream(iCalendarWriter cw) {
		}
		#endregion
	}
	public class iCalendarComponentEndItem : iCalendarBodyItem {
		#region Properties
		public override iCalendarBodyItemType BodyItemType { get { return iCalendarBodyItemType.ComponentEnd; } }
		public override string Name { get { return String.Empty; } }
		#endregion
		#region ApplyTimeZone
		protected override void ApplyTimeZone(TimeZoneManager timeZoneManager) {
		}
		#endregion
		#region WriteToStream
		protected override void WriteToStream(iCalendarWriter cw) {
		}
		#endregion
	}
}
