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
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.Utils {
	public interface ICustomObjectSerialize {
		void ReadPropertyValues(SettingsStorage storage);
		void WritePropertyValues(SettingsStorage storage);
	}
	public class ObjectSerializer {
		private DetailView detailView;
		private SettingsStorage storage;
		public ObjectSerializer(DetailView detailView, SettingsStorage storage) {
			this.detailView = detailView;
			this.storage = storage;
		}
		public void StoreObject(object obj) {
			WriteObjectPropertyValues(detailView, storage, obj);
		}
		public void RestoreObject(object obj) {
			ReadObjectPropertyValues(storage, obj);
		}
		private static IEnumerable<IMemberInfo> GetPersistentMembers(ITypeInfo typeInfo) {
			return Enumerator.Filter<IMemberInfo>(typeInfo.Members, delegate(IMemberInfo member) { return member.IsPersistent; });
		}
		public static void ReadObjectPropertyValues(SettingsStorage storage, object obj) {
			Guard.ArgumentNotNull(storage, "storage");
			Guard.ArgumentNotNull(obj, "obj");
			if(obj is ICustomObjectSerialize) {
				((ICustomObjectSerialize)obj).ReadPropertyValues(storage);
				return;
			}
			ITypeInfo objectType = XafTypesInfo.Instance.FindTypeInfo(obj.GetType());
			foreach(IMemberInfo md in GetPersistentMembers(objectType)) {
				try {
					string val = storage.LoadOption("", md.Name);
					if(!string.IsNullOrEmpty(val)) {
						md.SetValue(obj, md.DeserializeValue(val));
					}
				}
				catch(Exception e) {
					Tracing.Tracer.LogError(new Exception("An error occurs while the '" + md.Name + "' property value reading", e));
				}
			}
		}
		public static void WriteObjectPropertyValues(DetailView detailView, SettingsStorage storage, object obj) {
			Guard.ArgumentNotNull(storage, "storage");
			Guard.ArgumentNotNull(obj, "obj");
			if (obj is ICustomObjectSerialize) {
				((ICustomObjectSerialize)obj).WritePropertyValues(storage);
				return;
			}
			if(detailView == null) {
				ITypeInfo objectType = XafTypesInfo.Instance.FindTypeInfo(obj.GetType());
				foreach (IMemberInfo md in GetPersistentMembers(objectType)) {
					storage.SaveOption("", md.Name, Convert.ToString(md.GetValue(obj)));
				}
			}
			else {
				foreach(ViewItem item in detailView.Items) {
					PropertyEditor editor = item as PropertyEditor;
					if(editor != null && !editor.IsPassword) {
						storage.SaveOption("", editor.PropertyName, Convert.ToString(editor.MemberInfo.GetValue(obj)));
					}
				}
			}
		}
	}
}
