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
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Model.Core {
	public sealed class ModelValueInfo {
		internal static readonly ModelValueInfo IdValueInfo;
		internal static readonly ModelValueInfo IndexValueInfo;
		internal static readonly ModelValueInfo IsNewNodeValueInfo;
		internal static readonly ModelValueInfo IsRemovedNodeValueInfo;
		string name;
		Type propertyType;
		object defaultValue;
		bool isLocalizable;
		bool isReadOnly;
		string persistentPath;
		string converterTypeName;
		TypeConverter typeConverter;
		static ModelValueInfo() {
			IdValueInfo = new ModelValueInfo(ModelValueNames.Id, typeof(string), false, false, string.Empty, null);
			IndexValueInfo = new ModelValueInfo(ModelValueNames.Index, typeof(int?), false, false, string.Empty, null);
			IsNewNodeValueInfo = new ModelValueInfo(ModelValueNames.IsNewNode, typeof(bool), false, false, string.Empty, null);
			IsRemovedNodeValueInfo = new ModelValueInfo(ModelValueNames.IsRemovedNode, typeof(bool), false, false, string.Empty, null);
		}
		public ModelValueInfo(string name, Type propertyType, bool isLocalizable, bool isReadOnly, string persistentPath, string converterTypeName) {
			Guard.ArgumentNotNullOrEmpty(name, "name");
			Guard.ArgumentNotNull(propertyType, "propertyType");
			this.name = name;
			this.propertyType = propertyType;
			this.defaultValue = propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null;
			this.isLocalizable = isLocalizable;
			this.isReadOnly = isReadOnly;
			this.persistentPath = persistentPath;
			this.converterTypeName = converterTypeName;
		}
		public string Name { get { return name; } }
		public Type PropertyType { get { return propertyType; } }
		public object DefaultValue { get { return defaultValue; } }
		public bool IsLocalizable { get { return isLocalizable; } }
		public bool IsReadOnly { get { return isReadOnly; } }
		public string PersistentPath { get { return persistentPath; } }
		public string ConverterTypeName { get { return converterTypeName; } }
		public TypeConverter TypeConverter {
			get {
				if(typeConverter == null && !string.IsNullOrEmpty(converterTypeName)) {
					typeConverter = (TypeConverter)ReflectionHelper.CreateObject(converterTypeName);
				}
				return typeConverter;
			}
		}
		public bool CanPersistentPathBeUsed {
			get { return PropertyType != null && PropertyType != typeof(string) && (PropertyType.IsClass || PropertyType.IsInterface); }
		}
	}
}
