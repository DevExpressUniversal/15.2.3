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
using System.Linq;
using System.Text;
using DevExpress.XtraPrinting.Native;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections;
using DevExpress.Data.Native;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Data.Access;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Data.Browsing.Design {
	public class FakedPropertyDescriptor : IPropertyDescriptor {
		public static FakedPropertyDescriptor[] ToFakedProperties(IList properties, ITypeSpecificsService serv) {
			FakedPropertyDescriptor[] result = new FakedPropertyDescriptor[properties.Count];
			for(int i = 0; i < result.Length; i++) {
				PropertyDescriptor property = (PropertyDescriptor)properties[i];
				TypeSpecifics specific = serv != null ? serv.GetTypeSpecifics(property.PropertyType) : TypeSpecifics.Default;
				result[i] = new FakedPropertyDescriptor(property, specific);
			}
			return result;
		}
		bool isComplex;
		string displayName;
		TypeSpecifics specific;
		PropertyDescriptor propertyDescriptor;
		public PropertyDescriptor RealProperty { get { return propertyDescriptor; } }
		public FakedPropertyDescriptor(PropertyDescriptor propertyDescriptor, TypeSpecifics kind) 
			: this(propertyDescriptor, propertyDescriptor.DisplayName, kind) {
		}
		public FakedPropertyDescriptor(PropertyDescriptor propertyDescriptor, string displayName, TypeSpecifics kind)
			: this(propertyDescriptor, displayName, false, kind) {
		}
		public FakedPropertyDescriptor(PropertyDescriptor propertyDescriptor, string displayName, bool isComplex, TypeSpecifics specific) {
			this.propertyDescriptor = propertyDescriptor;
			this.displayName = displayName;
			this.isComplex = isComplex;
			this.specific = specific;
		}
		public TypeSpecifics Specifics {
			get { return specific; }
		}
		public bool IsComplex {
			get { return isComplex; }
		}
		public bool IsListType {
			get { return IsList(propertyDescriptor.PropertyType); }
		}
		static bool IsList(Type type) {
			return !typeof(Byte[]).Equals(type) && ListTypeHelper.IsListType(type);
		}
		public string Name {
			get { return propertyDescriptor.Name; }
		}
		public string DisplayName {
			get { return displayName; }
		}
	}
	public class PropertiesProvider : IPropertiesProvider, IDisposable {
		DataContext dataContext;
		bool disposeDataContext;
		ITypeSpecificsService serv;
		ITypeSpecificsService Service {
			get {
				if(serv == null)
					serv = new TypeSpecificsService();
				return serv;
			}
		}
		public DataContext DataContext {
			get { return dataContext; }
		}
		public PropertiesProvider() : this(new DataContext(), null) {
			disposeDataContext = true;
		}
		public PropertiesProvider(DataContext dataContext, ITypeSpecificsService serv) {
			this.dataContext = dataContext;
			this.serv = serv;
		}
		public virtual void Dispose() {
			if(disposeDataContext)
				dataContext.Dispose();
		}
		public virtual void GetItemProperties(object dataSource, string dataMember, EventHandler<GetPropertiesEventArgs> action) {
			PropertyDescriptorCollection properties = dataContext.GetItemProperties(dataSource, dataMember);
			IPropertyDescriptor[] args = ProcessProperties(properties, dataSource, dataMember);
			action(this, CreatePropertiesEventArgs(args));
		}
		protected virtual GetPropertiesEventArgs CreatePropertiesEventArgs(IPropertyDescriptor[] args) {
			return new GetPropertiesEventArgs(args);
		}
		IPropertyDescriptor[] ProcessProperties(PropertyDescriptorCollection properties, object dataSource, string dataMember) {
			if(properties != null) {
				PropertyDescriptor[] filteredProperties = FilterProperties(properties, dataSource, dataMember);
				FakedPropertyDescriptor[] fakedProperties = ToFakedProperties(dataSource, dataMember, filteredProperties);
				List<IPropertyDescriptor> postFilteredProperties = PostFilterProperties(fakedProperties);
				IPropertyDescriptor[] result = postFilteredProperties.ToArray();
				SortProperties(result);
				return result;
			}
			return new IPropertyDescriptor[] { };
		}
		public static string GetFullName(string dataMember, string name) {
			return BindingHelper.JoinStrings(".", dataMember, name);
		}
		public void GetDataSourceDisplayName(object dataSource, string dataMember, EventHandler<GetDataSourceDisplayNameEventArgs> callback) {
			string displayName = dataContext.GetDataSourceDisplayName(dataSource, dataMember);
			callback(this, new GetDataSourceDisplayNameEventArgs(displayName));
		}
		public virtual void GetListItemProperties(object dataSource, string dataMember, EventHandler<GetPropertiesEventArgs> action) {
			PropertyDescriptorCollection properties = dataContext.GetListItemProperties(dataSource, dataMember);
			IPropertyDescriptor[] args = ProcessProperties(properties, dataSource, dataMember);
			action(this, CreatePropertiesEventArgs(args));
		}
		public FakedPropertyDescriptor[] ToFakedProperties(object dataSource, string dataMember, PropertyDescriptor[] properties) {
			FakedPropertyDescriptor[] result = new FakedPropertyDescriptor[properties.Length];
			for(int i = 0; i < result.Length; i++) {
				PropertyDescriptor prop = properties[i];
				string displayName = GetPropertyDisplayName(dataSource, dataMember, prop);
				result[i] = new FakedPropertyDescriptor(prop, displayName, IsComplexProperty(prop, dataSource, dataMember), Service.GetTypeSpecifics(prop.PropertyType));
			}
			return result;
		}
		bool IsComplexProperty(PropertyDescriptor property, object dataSource, string dataMember) {
			PropertyDescriptorCollection properties = dataContext.GetListItemProperties(dataSource, GetFullName(dataMember, property.Name));
			return properties.Count > 0;
		}
		string GetPropertyDisplayName(object dataSource, string dataMember, PropertyDescriptor property) {
			if(dataSource is IDisplayNameProvider)
				return dataContext.GetDataMemberDisplayName(dataSource, dataMember, GetFullName(dataMember, property.Name));
			return DevExpress.Data.Helpers.MasterDetailHelper.GetDisplayNameCore(property);
		}
		protected virtual void SortProperties(IPropertyDescriptor[] properties) {
			Array.Sort<IPropertyDescriptor>(properties, delegate(IPropertyDescriptor x, IPropertyDescriptor y) {
				return string.Compare(x.DisplayName, y.DisplayName);
			});
		}
		protected virtual PropertyDescriptor[] FilterProperties(ICollection properties, object dataSource, string dataMember) {
			return new DevExpress.XtraReports.Native.Data.PropertyAggregator(DataContext).Aggregate(properties, dataSource, dataMember);
		}
		protected List<IPropertyDescriptor> PostFilterProperties(ICollection<IPropertyDescriptor> properties) {
			List<IPropertyDescriptor> outProperties = new List<IPropertyDescriptor>();
			foreach(IPropertyDescriptor property in properties) {
				string displayName = property.DisplayName;
				if(!string.IsNullOrEmpty(displayName) && CanProcessProperty(property)) {
					outProperties.Add(property);
				}
			}
			return outProperties;
		}
		protected virtual bool CanProcessProperty(IPropertyDescriptor property) {
			return !GetProperty(property).Attributes.Contains(BrowsableAttribute.No);
		}
		static string ToString(IPropertyDescriptor property) {
			PropertyDescriptor propertyDescriptor = GetProperty(property);
			return propertyDescriptor.ComponentType.ToString() + "." + propertyDescriptor.Name;
		}
		protected static Type GetPropertyType(IPropertyDescriptor property) {
			return GetProperty(property).PropertyType;
		}
		protected static PropertyDescriptor GetProperty(IPropertyDescriptor property) {
			return ((FakedPropertyDescriptor)property).RealProperty;
		}
	}
	public class DataSortedPropertiesNativeProvider : PropertiesProvider {
		IDataContextService serv;
		public DataSortedPropertiesNativeProvider() : base() { 
		}
		public DataSortedPropertiesNativeProvider(DataContext dataContext, IDataContextService serv, TypeSpecificsService typeSpecificsService)
			: base(dataContext, typeSpecificsService) {
			this.serv = serv;
		}
		protected override void SortProperties(IPropertyDescriptor[] properties) {
			if (serv != null)
				serv.SortProperties(properties);
			else
				base.SortProperties(properties);
		}
		protected override PropertyDescriptor[] FilterProperties(ICollection properties, object dataSource, string dataMember) {
			PropertyDescriptor[] filteredProperties = base.FilterProperties(properties, dataSource, dataMember);
			return serv != null ? serv.FilterProperties(filteredProperties, dataSource, dataMember, DataContext) : filteredProperties;
		}
	}
	public static class DataContextHelper {
		public static void DataContextAction(IDataContextService dataContextService, DataContextOptions options, Action<DataContext> action) {
			if(dataContextService == null) {
				using(DataContext dataContext = new DataContext()) {
					action(dataContext);
				}
			} else {
				DataContext context = dataContextService.CreateDataContext(options);
				Debug.Assert(context != null);
				action(context);
			}
		}
	}
	public class DataContextFilterPropertiesEventArgs : EventArgs {
		public IList<PropertyDescriptor> Properties { get; private set; }
		public object DataSource { get; private set; } 
		public string DataMember { get; private set; }
		public DataContext DataContext { get; private set; }
		public DataContextFilterPropertiesEventArgs(IList<PropertyDescriptor> properties, object dataSource, string dataMember, DataContext dataContext) {
			Properties = properties;
			DataSource = dataSource;
			DataMember = dataMember;
			DataContext = dataContext;
		}
	}
	public interface IDataContextService {
		event EventHandler<DataContextFilterPropertiesEventArgs> PrefilterProperties;
		DataContext CreateDataContext(DataContextOptions options);
		DataContext CreateDataContext(DataContextOptions options, bool useDictionary);
		void SortProperties(IPropertyDescriptor[] properties);
		PropertyDescriptor[] FilterProperties(PropertyDescriptor[] properties, object dataSource, string dataMember, DataContext dataContext);
	}
	public abstract class PickManager : PickManagerBase {
#if !DXPORTABLE
		public static void FillDataSourceImageList(ImageList imageList) {
			Bitmap bitmap = null;
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.Data.Utils.DataPickerImages.bmp");
			if(stream != null)
				try {
					bitmap = new Bitmap(stream);
					if(bitmap != null) {
						bitmap.MakeTransparent(Color.Lime);
						imageList.Images.AddStrip(bitmap);
					}
				} finally {
					stream.Close();
				}
		}
#endif
		Func<IPropertiesProvider> providerCreator;
		public Func<IPropertiesProvider> ProviderCreator {
			get { return providerCreator; }
			set { providerCreator = value; }
		}
		protected PickManager() {
		}
		public DataInfo[] GetData(object dataSource, string dataMember, Predicate<IPropertyDescriptor> shouldCreateDataInfo) {
			IPropertyDescriptor[] properties = null;
			IPropertiesProvider provider = CreateProvider();
			provider.GetListItemProperties(dataSource, dataMember, (s, e) => properties = e.Properties);
			DisposeObject(provider);
			List<DataInfo> data = new List<DataInfo>(properties.Length);
			foreach(IPropertyDescriptor property in properties) {
				if(shouldCreateDataInfo(property))
					data.Add(new DataInfo(dataSource, GetFullName(dataMember, property.Name), property.DisplayName));
			}
			return data.ToArray();
		}
		public DataInfo[] GetData(object dataSource, string dataMember) {
			return GetData(dataSource, dataMember, ShouldCreateDataInfo);
		}
		protected virtual bool ShouldCreateDataInfo(IPropertyDescriptor propertyDescriptor) {
			return !propertyDescriptor.IsListType;
		}
		protected override IPropertiesProvider CreateProvider() {
			return providerCreator != null ? providerCreator() : new PropertiesProvider(new DataContext(), null);
		}
		protected static Type GetPropertyType(IPropertyDescriptor property) {
			return ((FakedPropertyDescriptor)property).RealProperty.PropertyType;
		} 
	}
}
