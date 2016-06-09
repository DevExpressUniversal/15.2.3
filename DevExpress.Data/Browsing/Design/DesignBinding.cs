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
using System.Globalization;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using DevExpress.Data;
using DevExpress.Data.Native;
using DevExpress.Data.Browsing;
using DevExpress.XtraPrinting.Localization;
namespace DevExpress.Data.Browsing.Design {
	public interface IDataSourceCollectionProvider {
		object[] GetDataSourceCollection(IServiceProvider serviceProvider);
	}
	[
	Editor("DevExpress.XtraReports.Design.DesignBindingEditor," + AssemblyInfo.SRAssemblyUtilsUI, typeof(UITypeEditor)),
	TypeConverter(typeof(DesignBindingConverterBase))
	]
	public class DesignBinding {
		public static DesignBinding Null = new DesignBinding(null, null);
		object dataSource;
		string dataMember = "";
		[Browsable(false)]
		public object DataSource { get { return dataSource; } }
		[Browsable(false)]
		public string DataMember { get { return dataMember; } }
		[Browsable(false)]
		public bool IsNull { get { return dataSource == null && dataMember == null; } }
		public DesignBinding() {
		}
		public DesignBinding(object dataSource, string dataMember) {
			Assign(dataSource, dataMember);
		}
		protected void Assign(object dataSource, string dataMember) {
			this.dataSource = dataSource;
			this.dataMember = dataMember;
		}
		bool Equals(object dataSource, string dataMember) {
			if(!ReferenceEquals(this.dataSource, dataSource))
				return false;
			return String.Compare(dataMember, this.dataMember, true, CultureInfo.InvariantCulture) == 0;
		}
		public override bool Equals(object value) {
			DesignBinding binding = value as DesignBinding;
			return binding != null && Equals(binding.dataSource, binding.dataMember);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public string GetDisplayName(IServiceProvider serviceProvider) {
			IDataContextService dataContextService = (IDataContextService)serviceProvider.GetService(typeof(IDataContextService));
			string name = string.Empty;
			DataContextHelper.DataContextAction(dataContextService, new DataContextOptions(true, true), delegate(DataContext dataContext) {
				name = dataContext.GetDataMemberDisplayName(dataSource, dataMember);
			});
			return name;
		}
	}
	public class DesignBindingConverterBase : TypeConverter {
		protected virtual string NoneString {
			get {
				return PreviewLocalizer.GetString(PreviewStringId.NoneString);
			}
		}
		protected virtual string UntypedDataSourceName {
		   get {
				return "(List)";
		   }
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(string);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			DesignBinding binding = value as DesignBinding;
			if(binding == null || String.IsNullOrEmpty(binding.DataMember))
				return NoneString;
			string s = String.Empty;
			if (binding.DataSource is IDisplayNameProvider)
				s = ((IDisplayNameProvider)binding.DataSource).GetDataSourceDisplayName();
			else {
				IComponent comp = binding.DataSource as IComponent;
				if (comp != null)
					s = BindingHelper.GetDataSourceName(comp);
			}				
			if(String.IsNullOrEmpty(s))
				s = UntypedDataSourceName;
			s += " - " + binding.GetDisplayName(context as IServiceProvider);
			return s;
		}
	}
}
