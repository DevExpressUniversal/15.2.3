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

namespace DevExpress.Utils.MVVM {
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	public static class MVVMAssemblyProxy {
		public static Type GetMvvmType(ref Type typeRef, string typeName) {
			if(typeRef == null)
				typeRef = GetMvvmType(typeName);
			return typeRef;
		}
#if !DEBUGTEST
		const string typePrefix = "DevExpress.Mvvm.";
#else
		static string typePrefix = "DevExpress.Mvvm.";
		public static void SetUp() {
			mvvmAssembly = typeof(MVVMAssemblyProxy).Assembly;
			typePrefix = "DevExpress.Utils.MVVM.Tests.";
		}
		public static void SetUpMetadataHelperAttributes(IEnumerable<Attribute> attributes) {
			Tests.Native.MetadataHelper.attributes = attributes;
		}
		public static void ResetMetadataHelperAttributes() {
			Tests.Native.MetadataHelper.attributes = null;
		}
		public static void SetUpMetadataHelperFilteringAttributes(IEnumerable<Attribute> attributes) {
			Tests.Native.MetadataHelper.filteringAttributes = attributes;
		}
		public static void ResetMetadataHelperFilteringAttributes() {
			Tests.Native.MetadataHelper.filteringAttributes = null;
		}
#endif
		static Assembly mvvmAssembly;
		static Assembly GetMVVMAssembly() {
			if(mvvmAssembly == null)
				EnsureMvvmAssemblyLoaded();
			return mvvmAssembly;
		}
		static void EnsureMvvmAssemblyLoaded() {
			mvvmAssembly =
				AssemblyHelper.GetLoadedAssembly(AssemblyInfo.SRAssemblyMvvm) ??
				Assembly.Load(AssemblyInfo.SRAssemblyMvvmFull);
		}
		static Type GetMvvmType(string typeName) {
			var mvvmAssembly = GetMVVMAssembly();
			if(mvvmAssembly != null)
				return (mvvmAssembly != null) ? mvvmAssembly.GetType(typePrefix + typeName) : null;
			return null;
		}
		public static void Reset() {
			mvvmAssembly = null;
#if DEBUGTEST
			typePrefix = "DevExpress.Mvvm.";
			Tests.Native.MetadataHelper.filteringAttributes = Tests.Native.MetadataHelper.attributes = null;
#endif
		}
	}
	#region MVVM Proxy Classes For Tests
#if DEBUGTEST
	namespace Tests {
		using NUnit.Framework;
		public class MVVMDependentTest {
			[TestFixtureSetUp]
			public virtual void FixtureSetUp() {
				MVVMAssemblyProxy.SetUp();
			}
			[TestFixtureTearDown]
			public virtual void FixtureTearDown() {
				MVVMAssemblyProxy.Reset();
			}
		}
		namespace Native {
			static class MetadataHelper {
				internal static IEnumerable<Attribute> attributes = null;
				public static IEnumerable<Attribute> GetExternalAndFluentAPIAttrbutes(Type componentType, string memberName) {
					return attributes ?? new Attribute[0];
				}
				internal static IEnumerable<Attribute> filteringAttributes = null;
				public static IEnumerable<Attribute> GetExternalAndFluentAPIFilteringAttrbutes(Type componentType, string memberName) {
					return filteringAttributes ?? attributes ?? new Attribute[0];
				}
			}
		}
		namespace DataAnnotations {
			[AttributeUsage(AttributeTargets.Method)]
			public class CommandAttribute : Attribute {
				public string Name { get; set; }
			}
		}
	}
#endif
	#endregion
}
