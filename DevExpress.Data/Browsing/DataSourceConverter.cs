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
using System.ComponentModel;
using System.Collections;
using System.Globalization;
using DevExpress.XtraPrinting.Localization;
namespace DevExpress.Data.Design {
	public interface IDataContainerService {
		bool AreAppropriate(IDataContainerBase dataContainer, object dataSource);
	}
	public interface IDataSourceCollectorService {
		object[] GetDataSources();
	}
	public class DataSourceConverter : ReferenceConverter {
		public static bool IsListBindable(object obj) {
			return obj != null ? IsListBindable(obj.GetType()) : false;
		}
		public static bool IsListBindable(Type type) {
			ListBindableAttribute attr = TypeDescriptor.GetAttributes(type)[typeof(ListBindableAttribute)] as ListBindableAttribute;
			return attr == null || attr.ListBindable;
		}
		static List<IComponent> ConvertToActualDataSources(ICollection lists, IDataContainerService svc, IDataContainerBase dataContainer) {
			List<IComponent> listsList = new List<IComponent>();
			foreach(object list in lists)
				if(IsListBindable(list) && ((svc == null || dataContainer == null) || svc.AreAppropriate(dataContainer, list))) {
					IComponent component = list as IComponent;
					if(component != null)
						listsList.Add(component);
				}
			return listsList;
		}
		ReferenceConverter listConverter = new ReferenceConverter(typeof(IList));
		public DataSourceConverter()
			: base(typeof(IListSource)) {
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			StandardValuesCollection listSources = base.GetStandardValues(context);
			StandardValuesCollection lists = listConverter.GetStandardValues(context);
			IDataContainerBase dataContainer = context != null ? context.Instance as IDataContainerBase : null;
			IDataContainerService svc = context != null ? context.GetService(typeof(IDataContainerService)) as IDataContainerService : null;
			List<IComponent> listsList = ConvertToActualDataSources(listSources, svc, dataContainer);
			listsList.AddRange(ConvertToActualDataSources(lists, svc, dataContainer));
			listsList.Add(null);
			return new StandardValuesCollection(listsList);
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			object result = base.ConvertTo(context, culture, value, destinationType);
			if(destinationType != typeof(string))
				return result;
			if(value == null)
				return PreviewLocalizer.GetString(PreviewStringId.NoneString);
			IDisplayNameProvider displayNameProvider = value as IDisplayNameProvider;
			if (displayNameProvider != null)
				return displayNameProvider.GetDataSourceDisplayName();
			return result;
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string str = value as string;
			if(str != null && str == PreviewLocalizer.GetString(PreviewStringId.NoneString))
				return null;
			return base.ConvertFrom(context, culture, value);
		}
	}
}
