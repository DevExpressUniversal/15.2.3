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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp {
	public interface IExportedTypeHelper {
		void Init(ITypesInfo typesInfo);
		Boolean IsExportedType(Type type);
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class CachedExportedTypeHelper : IExportedTypeHelper {
		private readonly Dictionary<Type, Boolean> cache = new Dictionary<Type, Boolean>();
		protected abstract Boolean IsExportedTypeCore(Type type);
		public CachedExportedTypeHelper() {
		}
		public virtual void Init(ITypesInfo typesInfo) { }
		public virtual Boolean IsExportedType(Type type) {
			Boolean result = false;
			if(type != null) {
				if(!cache.TryGetValue(type, out result)) {
					lock(cache) {
						if(!cache.TryGetValue(type, out result)) {
							result = IsExportedTypeCore(type);
							cache.Add(type, result);
						}
					}
				}
			}
			return result;
		}
	}
	public static class ExportedTypeHelpers {
		class RegistrationEntry {
			public RegistrationEntry(String assemblyName, String typeFullName) {
				AssemblyName = assemblyName;
				TypeFullName = typeFullName;
			}
			public String AssemblyName { get; private set; }
			public String TypeFullName { get; private set; }
		}
		private static ITypesInfo typesInfo;
		private static Boolean isExportedTypeHelpersCollected;
		private readonly static List<RegistrationEntry> registrationEntries;
		private readonly static List<IExportedTypeHelper> exportedTypeHelpers;
		static ExportedTypeHelpers() {
			typesInfo = XafTypesInfo.Instance;
			registrationEntries = new List<RegistrationEntry>();
			exportedTypeHelpers = new List<IExportedTypeHelper>();
			AddExportedTypeHelper(new NonPersistentExportedTypeHelper());
			AddExportedTypeHelper("DevExpress.ExpressApp.Xpo" + XafAssemblyInfo.VersionSuffix, "DevExpress.ExpressApp.Xpo.XpoExportedTypeHelper");
			AddExportedTypeHelper("DevExpress.ExpressApp.EF" + XafAssemblyInfo.VersionSuffix, "DevExpress.ExpressApp.EF.EFExportedTypeHelper");
			AddExportedTypeHelper("DevExpress.ExpressApp.EF.45" + XafAssemblyInfo.VersionSuffix, "DevExpress.ExpressApp.EF.EFExportedTypeHelper");
			if(IsDesignMode) {
				AddExportedTypeHelper(new DevExpress.ExpressApp.Design.EFDesignTimeExportedTypeHelper());
			}
		}
		private static bool IsDesignMode {
			get {
				bool designMode = false;
				try { 
					designMode = DevExpress.Utils.Design.DesignTimeTools.IsDesignMode;
				}
				catch(System.Security.SecurityException) { }
				return designMode;
			}
		}
		private static IList<IExportedTypeHelper> GetExportedTypeHelpers() {
			if(!isExportedTypeHelpersCollected) {
				foreach(RegistrationEntry registration in registrationEntries) {
					Type exportedTypeHelperType;
					if(AssemblyHelper.TryGetType(registration.AssemblyName, registration.TypeFullName, out exportedTypeHelperType)) {
						IExportedTypeHelper exportedTypeHelper = (IExportedTypeHelper)TypeHelper.CreateInstance(exportedTypeHelperType);
						AddExportedTypeHelper(exportedTypeHelper);
					}
				}
				isExportedTypeHelpersCollected = true;
			}
			return exportedTypeHelpers;
		}
		public static void Init(ITypesInfo typesInfo) {
			ExportedTypeHelpers.typesInfo = typesInfo;
			foreach(IExportedTypeHelper exportedTypeHelper in exportedTypeHelpers) {
				exportedTypeHelper.Init(typesInfo);
			}
		}
		public static void AddExportedTypeHelper(IExportedTypeHelper exportedTypeHelper) {
			exportedTypeHelper.Init(typesInfo);
			exportedTypeHelpers.Add(exportedTypeHelper);
		}
		public static void AddExportedTypeHelper(String assemblyName, String typeFullName) {
			registrationEntries.Add(new RegistrationEntry(assemblyName, typeFullName));
		}
		public static Boolean IsExportedType(Type type) {
			Boolean result = false;
			foreach(IExportedTypeHelper exportedTypeHelper in GetExportedTypeHelpers()) {
				if(exportedTypeHelper.IsExportedType(type)) {
					result = true;
					break;
				}
			}
			return result;
		}
	}
}
