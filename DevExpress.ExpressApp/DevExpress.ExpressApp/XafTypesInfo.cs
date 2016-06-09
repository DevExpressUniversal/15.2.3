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
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp {
	public sealed class XafTypesInfo {
		private static TypesInfo instance;
		static XafTypesInfo() {
			instance = CreateTypesInfo();
		}
		private XafTypesInfo() { }
		private static TypesInfo CreateTypesInfo() {
			TypesInfo typesInfo = new TypesInfo();
			typesInfo.AddEntityStore(new NonPersistentEntityStore(typesInfo));
			return typesInfo;
		}
		public static ITypesInfo Instance {
			get { return instance; }
		}
		public static void Reset() {
			instance.Reset();
		}
		public static void HardReset() {
			instance.HardReset();
		}
		public static Type CastTypeInfoToType(ITypeInfo info) {
			return info.Type;
		}
		public static ITypeInfo CastTypeToTypeInfo(Type type) {
			return instance.FindTypeInfo(type);
		}
#if DebugTest
		public static void DebugTest_Recreate() {
			instance.Dispose();
			instance = CreateTypesInfo();
			ExportedTypeHelpers.Init(instance);
		}
		public static void DebugTest_SetInstance(TypesInfo instance) {
			XafTypesInfo.instance = instance;
		}
#endif
		#region Obsolete 15.1
		[Obsolete("Use the '((TypesInfo)XafTypesInfo.Instance).FindEntityStore(Type entityStoreType)' method instead.."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static IEntityStore PersistentEntityStore {
			get {
				IEntityStore result = null;
				if(instance != null) {
					result = instance.FindEntityStore("XpoTypeInfoSource");
				}
				return result;
			}
		}
		[Obsolete("Use the '((TypesInfo)XafTypesInfo.Instance).AddEntityStore(IEntityStore entityStore)' method instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetPersistentEntityStore(IEntityStore persistentEntityStore) {
			if(instance != null) {
				instance.AddEntityStore(persistentEntityStore);
			}
		}
		#endregion
	}
}
