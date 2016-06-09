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
using System.Collections;
using System.Globalization;
using System.Reflection;
using DevExpress.XtraReports.Serialization;
namespace DevExpress.Serialization.Services {
	public class XRDesignerResourceService : System.ComponentModel.Design.IResourceService, IDisposable {
		public string FileName = String.Empty;
		public string NameSpace = String.Empty;
		public string RootType = String.Empty;
		SResourceWriter resourceWriter;
		protected IServiceProvider serviceProvider;
		public XRDesignerResourceService(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
		}
		#region System.ComponentModel.Design.IResourceService interface implementation
		public System.Resources.IResourceWriter GetResourceWriter(System.Globalization.CultureInfo info) {
			try {
				if(resourceWriter == null) {
					resourceWriter = new SResourceWriter(serviceProvider);
				}
				return resourceWriter;
			} catch (Exception e) {
				ShowError(e.ToString());
				return null;
			}
		}
		protected void ShowError(string message) {
			throw new Exception(message);
		}
		public System.Resources.IResourceReader GetResourceReader(System.Globalization.CultureInfo info) {
			return null;
		}
		#endregion
		public void Save() {
		}
		public void SerializationStarted(bool serialize) {
		}
		public string SerializationEnded(bool serialize) {
			string result = "";
			if(resourceWriter != null) {
				if(serialize) {
					resourceWriter.Close();
					result = resourceWriter.Resources;
				}
			}
			return result;
		}
		public void Dispose() {
			SerializationEnded(false);
		}
	}
	public class SEventBindingService : IEventBindingService {
		#region inner classes
		class EventTypeConverter : TypeConverter {
			IServiceProvider serviceProvider = null;
			SEventBindingService eventBindingService = null;
			public EventTypeConverter(IServiceProvider serviceProvider, SEventBindingService eventBindingService) {
				this.serviceProvider = serviceProvider;
				this.eventBindingService = eventBindingService;
			}
			public override bool CanConvertFrom(ITypeDescriptorContext context,Type type) {
				return (type == typeof(string));
			}
			public override bool CanConvertTo(ITypeDescriptorContext context,Type type) {
				return (type == typeof(string));
			}
			public override object ConvertFrom(ITypeDescriptorContext context,CultureInfo culture,object value) {
				if (context != null) {
					return value;
				}
				else return base.ConvertFrom(context,culture,value);
			}
			public override object ConvertTo(ITypeDescriptorContext context,CultureInfo culture,object value, Type type) {
				if (value != null) {
					if (value.GetType() == typeof(string)) {
						return value;
					}
				}
				if (context != null) {
					EventPropertyDescriptor ed = context.PropertyDescriptor as EventPropertyDescriptor;
					if (ed != null) {
						return ed.GetValue(context.Instance);
					}
				}
				return base.ConvertTo(context, culture, value, type);
			}
			public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
				return true;
			}
			public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
				return false;
			}
			public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
				EventPropertyDescriptor ed = context.PropertyDescriptor as EventPropertyDescriptor;
				if (ed == null || ed.eventBindingService == null) {
					ISelectionService selectionService = (ISelectionService)this.serviceProvider.GetService(typeof(ISelectionService));
					EventInfo eventInfo = selectionService.PrimarySelection.GetType().GetEvent(context.PropertyDescriptor.Name);
					return new StandardValuesCollection(eventBindingService.GetCompatibleMethods(eventInfo));
				}
				ICollection col = ((IEventBindingService)ed.eventBindingService).GetCompatibleMethods(((EventPropertyDescriptor)context.PropertyDescriptor).Event);
				return new StandardValuesCollection(col);
			}
			public override bool IsValid(ITypeDescriptorContext context, object o) {
				return true;
			}
		}
		class EventPropertyDescriptor : PropertyDescriptor {
			protected EventDescriptor baseDescriptor = null;
			public SEventBindingService eventBindingService = null;
			public EventPropertyDescriptor(EventDescriptor eventDesc, SEventBindingService service): base(eventDesc) {
				baseDescriptor = eventDesc;
				eventBindingService = service;
			}
			public EventDescriptor Event {
				get { return baseDescriptor; }
			}
			public override Type ComponentType {
				get { return Event.ComponentType; }
			}
			public override TypeConverter Converter {
				get { return eventBindingService.eventTypeConverter; }
			}
			public override bool IsReadOnly {
				get { return false; }
			}
			public override Type PropertyType {
				get { return typeof(string); }
			}
			public override bool CanResetValue(object component) {
				Console.WriteLine("CanResetValue");
				return true;
			}
			public override object GetValue(object component) {
				IDictionary events = eventBindingService.components[component] as IDictionary;
				if (events == null) {
					return null;
				}
				return events[Name];
			}
			public override void ResetValue(object component) {
				SetValue(component,null);
			}
			public override void SetValue(object component, object value) {
				if((value as string).Length == 0) {
					value = null;
				}
				IDictionary events = eventBindingService.components[component] as IDictionary;
				string oldValue = null;
				if (events != null) {
					oldValue = (string)events[Name];
				} else if (value != null) {
					events = new Hashtable();
					eventBindingService.components[component] = (Hashtable)events;
				}
				if (String.Compare(oldValue,(string)value) != 0) {
					events[Name] = value;
				}
			}
			public override bool ShouldSerializeValue(object component) {
				return false;
			}
		}
		#endregion
		EventTypeConverter eventTypeConverter;
		Hashtable components = new Hashtable();
		public SEventBindingService(IServiceProvider serviceProvider) {
			eventTypeConverter = new EventTypeConverter(serviceProvider, this);
		}
		#region IEventBindingService implementation
		string IEventBindingService.CreateUniqueMethodName(IComponent component, EventDescriptor e) {
			return String.Format("{0}{1}", Char.ToUpper(component.Site.Name[0]) + component.Site.Name.Substring(1), e.DisplayName);
		}
		ICollection IEventBindingService.GetCompatibleMethods(EventDescriptor e) {
			return new string[] {};
		}
		ICollection GetCompatibleMethods(EventInfo e) {
			return new string[] {};
		}
		EventDescriptor IEventBindingService.GetEvent(PropertyDescriptor property) {
			EventPropertyDescriptor eventProp = property as EventPropertyDescriptor;
			if (eventProp == null) {
				return null;
			}
			return eventProp.Event;
		}
		PropertyDescriptorCollection IEventBindingService.GetEventProperties(EventDescriptorCollection events) {
			PropertyDescriptor[] props = new PropertyDescriptor[events.Count];
			for (int i = 0; i < events.Count; ++i) {
				props[i] = ((IEventBindingService)this).GetEventProperty(events[i]);
			}
			return new System.ComponentModel.PropertyDescriptorCollection(props);
		}
		PropertyDescriptor IEventBindingService.GetEventProperty(EventDescriptor e) {
			return new EventPropertyDescriptor(e, this);
		}
		bool IEventBindingService.ShowCode() {
			return false;
		}
		bool IEventBindingService.ShowCode(int lineNumber) {
			return false;
		}
		bool IEventBindingService.ShowCode(IComponent component, EventDescriptor edesc) {
			return false;
		}
		#endregion
	}
}
