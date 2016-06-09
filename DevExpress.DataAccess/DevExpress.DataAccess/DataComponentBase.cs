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
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Xml.Linq;
using DevExpress.Data;
using DevExpress.Data.Utils;
using DevExpress.DataAccess.Native;
using DevExpress.Services.Internal;
using DevExpress.Utils.Serializing;
using DevExpress.XtraReports.Native;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design;
using DevExpress.Utils;
namespace DevExpress.DataAccess {
	public abstract class DataComponentBase : Component, DevExpress.XtraPrinting.Native.IObject, IServiceContainer, IDataComponent, IParametersRenamer {
		readonly ServiceManager serviceManager = new ServiceManager();
		string name = string.Empty;
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[XtraSerializableProperty(-1)]
		public string ObjectType {
			get {
				Type type = GetType();
				return string.Format("{0},{1}", type.FullName, type.GetAssembly().GetName().Name);
			}
		}
		[DefaultValue("")]
		[Browsable(false)]
		[XtraSerializableProperty(-1)]
		public virtual string Name {
			get { return Site != null ? Site.Name : this.name; }
			set {
				try {
					if(Site == null)
						this.name = value;
					else
						Site.Name = value;
				}
				catch(Exception e) {
					Debug.WriteLine(e);
				}
			}
		}
		protected IExtensionsProvider ExtensionsProvider { get { return this.GetService<IExtensionsProvider>(); } }
		protected abstract IEnumerable<IParameter> AllParameters { get; }
		void IDataComponent.Fill(IEnumerable<IParameter> sourceParameters) {
			Fill(sourceParameters);
		}
		protected abstract void Fill(IEnumerable<IParameter> sourceParameters);
		string IDataComponent.DataMember {
			get { return GetDataMember(); }
		}
		public abstract XElement SaveToXml();
		public abstract void LoadFromXml(XElement element);
		protected void ReplaceServiceFromProvider(Type type, IServiceProvider serviceProvider) {
			if(serviceProvider != null) {
				object service = serviceProvider.GetService(type);
				if(service != null) {
					((IServiceContainer)this).RemoveService(type);
					((IServiceContainer)this).AddService(type, service);
				}
			}
		}
		protected abstract string GetDataMember();
		#region IParameterRenamer
		public void RenameParameters(IDictionary<string, string> renamingMap) {
			new ParametersRenamingHelper(renamingMap).Process(AllParameters);
		}
		public void RenameParameter(string oldName, string newName) {
			RenameParameters(new Dictionary<string, string> {{oldName, newName}});
		}
		#endregion
		#region IServiceContainer Members
		void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) { this.serviceManager.AddService(serviceType, callback, promote); }
		void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback) { this.serviceManager.AddService(serviceType, callback); }
		void IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote) { this.serviceManager.AddService(serviceType, serviceInstance, promote); }
		void IServiceContainer.AddService(Type serviceType, object serviceInstance) { this.serviceManager.AddService(serviceType, serviceInstance); }
		void IServiceContainer.RemoveService(Type serviceType, bool promote) { this.serviceManager.RemoveService(serviceType, promote); }
		void IServiceContainer.RemoveService(Type serviceType) { this.serviceManager.RemoveService(serviceType); }
		#endregion
		#region IServiceProvider Members
		public new object GetService(Type serviceType) { return this.serviceManager.GetService(serviceType); } 
		object IServiceProvider.GetService(Type serviceType) { return this.GetService(serviceType); }
		#endregion    
	}
}
