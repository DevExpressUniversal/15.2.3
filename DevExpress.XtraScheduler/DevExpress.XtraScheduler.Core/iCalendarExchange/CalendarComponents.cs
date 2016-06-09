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
using System.ComponentModel;
using System.Text;
using DevExpress.XtraScheduler.iCalendar.Native;
using DevExpress.XtraScheduler.iCalendar.Internal;
namespace DevExpress.XtraScheduler.iCalendar.Components {
	public delegate void ObtainPropertyDelegate(ICalendarBodyItemContainer container, iCalendarPropertyBase value);
	public delegate void ObtainComponentDelegate(ICalendarBodyItemContainer container, iCalendarComponentBase value);
	#region iCalendarComponentCollection
	public class iCalendarComponentCollection : List<iCalendarComponentBase> {
	}
	#endregion
	#region iCalendarComponentBase (abstract class)
	public abstract class iCalendarComponentBase : iCalendarBodyItem, ICalendarBodyItemContainer {
		static readonly Dictionary<Type, ObtainPropertyDelegate> emptyPropertyHandlers;
		static readonly Dictionary<Type, ObtainComponentDelegate> emptyComponentHandlers;
		static iCalendarComponentBase() {
			emptyPropertyHandlers = new Dictionary<Type, ObtainPropertyDelegate>();
			emptyComponentHandlers = new Dictionary<Type, ObtainComponentDelegate>();
		}
		readonly iCalendarPropertyCollection customProperties;
		readonly iCalendarComponentCollection customComponents;
		protected iCalendarComponentBase() {
			customProperties = new iCalendarPropertyCollection();
			customComponents = new iCalendarComponentCollection();
		}
		protected internal virtual Dictionary<Type, ObtainPropertyDelegate> SupportedPropertyHandlers { get { return emptyPropertyHandlers; } }
		protected internal virtual Dictionary<Type, ObtainComponentDelegate> SupportedComponentHandlers { get { return emptyComponentHandlers; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("iCalendarComponentBaseBodyItemType")]
#endif
		public override iCalendarBodyItemType BodyItemType { get { return iCalendarBodyItemType.ComponentStart; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("iCalendarComponentBaseCustomProperties")]
#endif
		public iCalendarPropertyCollection CustomProperties { get { return customProperties; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("iCalendarComponentBaseCustomComponents")]
#endif
		public iCalendarComponentCollection CustomComponents { get { return customComponents; } }
		#region ICalendarBodyItemContainer Members
		public void AddObject(iCalendarBodyItem item) {
			iCalendarPropertyBase property = item as iCalendarPropertyBase;
			if (property != null)
				AddProperty(property);
			iCalendarComponentBase component = item as iCalendarComponentBase;
			if (component != null)
				AddComponent(component);
		}
		protected virtual void AddProperty(iCalendarPropertyBase value) {
			ObtainPropertyDelegate handler;
			if (SupportedPropertyHandlers.TryGetValue(value.GetType(), out handler))
				handler(this, value);
			else
				CustomProperties.Add((iCalendarPropertyBase)value);
		}
		protected virtual void AddComponent(iCalendarComponentBase value) {
			ObtainComponentDelegate handler;
			if (SupportedComponentHandlers.TryGetValue(value.GetType(), out handler))
				handler(this, value);
			else
				CustomComponents.Add((iCalendarComponentBase)value);
		}
		#endregion
		#region WriteToStream
		protected override void WriteToStream(iCalendarWriter cw) {
			cw.WriteLine(String.Format("BEGIN:{0}", Name));
			WriteProperties(cw);
			WriteComponents(cw);
			WriteCustomComponents(cw);
			WriteCustomProperties(cw);
			cw.WriteLine(String.Format("END:{0}", Name));
		}
		#endregion
		#region WriteCustomProperties
		void WriteCustomProperties(iCalendarWriter cw) {
			int count = CustomProperties.Count;
			for (int i = 0; i < count; i++) {
				iCalendarPropertyBase property = CustomProperties[i];
				property.WriteToStream(cw);
			}
		}
		#endregion
		#region WriteCustomComponents
		void WriteCustomComponents(iCalendarWriter cw) {
			int count = CustomComponents.Count;
			for (int i = 0; i < count; i++) {
				iCalendarComponentBase component = CustomComponents[i];
				component.WriteToStream(cw);
			}
		}
		#endregion
		protected abstract void WriteComponents(iCalendarWriter cw);
		protected abstract void WriteProperties(iCalendarWriter cw);
	}
	#endregion
	#region CustomComponent
	public class CustomComponent : iCalendarComponentBase {
		string name;
		public CustomComponent(string name) {
			this.name = name;
		}
		public override string Name { get { return name; } }
		#region ApplyTimeZone
		protected override void ApplyTimeZone(TimeZoneManager timeZoneManager) {
		}
		#endregion
		#region WriteProperties
		protected override void WriteProperties(iCalendarWriter cw) {
		}
		#endregion
		#region WriteComponents
		protected override void WriteComponents(iCalendarWriter cw) {
		}
		#endregion
	}
	#endregion
}
