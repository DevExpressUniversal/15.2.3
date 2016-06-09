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

namespace DevExpress.Design.DataAccess.Wpf {
	using System;
	using System.Collections.Generic;
	using DevExpress.Data.Utils;
	public sealed class EditingContextTypesProvider : BaseDataAccessTechnologyTypesProvider, IEnumTypesProviderInfo {
		public EditingContextTypesProvider()
			: base(Metadata.AvailableTypes.Local(ValidForDataAccessTechnologyConstraints)) {
		}
		protected override IEnumerator<System.Type> GetEnumeratorCore() {
			var dteTypes = DevExpress.Design.UI.Platform.GetTypes();
			if(dteTypes != null && dteTypes.Length > 0) {
				types = Metadata.AvailableTypes.All(dteTypes, ValidForDataAccessTechnologyConstraints);
			}
			else {
				typesCacheReady.WaitOne();
				if(typesCache != null)
					types = Metadata.AvailableTypes.All(typesCache, ValidForDataAccessTechnologyConstraints);
			}
			return base.GetEnumeratorCore();
		}
		#region IEnumTypesProviderInfo
		System.Type[] IEnumTypesProviderInfo.RootTypes {
			get {
				return new System.Type[] { 
					typeof(System.Windows.Application), 
					typeof(System.Windows.Controls.Control) 
				};
			}
		}
		string[] IEnumTypesProviderInfo.SkipList {
			get {
				return new string[] { 
					"DataGrid" 
				};
			}
		}
		#endregion IEnumTypesProviderInfo
		static IList<System.Type> typesCache;
		static System.Threading.ManualResetEvent typesCacheReady;
		internal static void Release() {
			typesCache = null;
			if(typesCacheReady != null)
				typesCacheReady.Dispose();
			typesCacheReady = null;
		}
		internal static void QueueTypesDiscovering(Microsoft.Windows.Design.EditingContext context) {
			typesCache = null;
			if(typesCacheReady == null)
				typesCacheReady = new System.Threading.ManualResetEvent(false);
			else
				typesCacheReady.Reset();
			System.Threading.ThreadPool.QueueUserWorkItem(DiscoverTypes, context);
		}
		static void DiscoverTypes(object state) {
			var editingContext = ((Microsoft.Windows.Design.EditingContext)state);
			try {
				typesCache = new List<System.Type>();
				try {
					var context = DevExpress.Xpf.Core.Design.XpfEditingContext.FromEditingContext(editingContext);
					foreach(IDXTypeMetadata typeMetadata in context.Services.GetService<IDXTypeDiscoveryService>().GetTypes(
							a => DevExpress.Design.UI.Platform.IsProjectAssembly(a.Name),
							t => ValidForDataAccessTechnologyConstraints(t.GetRuntimeType()), false)) {
						typesCache.Add(typeMetadata.GetRuntimeType());
					}
				} catch {
					typesCache = null;
				}
			} finally {
				typesCacheReady.Set(); }
		}
	}
	public sealed class DataAccessTechnologyTypesProviderFactory : IDataAccessTechnologyTypesProviderFactory {
		public IDataAccessTechnologyTypesProvider Create(System.IServiceProvider serviceProvider) {
			if(serviceProvider is Microsoft.Windows.Design.ServiceManager) {
				return new EditingContextTypesProvider();
			}
			return new DefaultDataAccessTechnologyLocalTypesProvider();
		}
	}
}
