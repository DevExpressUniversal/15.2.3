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
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
namespace DevExpress.Xpf.Core.Serialization {
	public class DXXmlSerializer : XmlXtraSerializer {
		protected override Dictionary<string, string> GetAttributes(XtraPropertyInfo pInfo) {
			Dictionary<string, string> attributes = base.GetAttributes(pInfo);
			AttachedPropertyInfo apInfo = pInfo as AttachedPropertyInfo;
			if(apInfo != null) {
				attributes.Add("OwnerType", apInfo.OwnerType.FullName);
				attributes.Add("DependencyPropertyType", apInfo.DependencyPropertyType.FullName);
			}
			return attributes;
		}
		protected override XtraPropertyInfo CreateXtraPropertyInfo(string name, Type propType, bool isKey, Dictionary<string, string> attributes) {
			string _ownerType, _dPropType;
			if(attributes.TryGetValue("OwnerType", out _ownerType) && attributes.TryGetValue("DependencyPropertyType", out _dPropType)) {
				Type ownerType = GetType(_ownerType);
				Type dPropType = GetType(_dPropType);
				return new AttachedPropertyInfo(name, propType, dPropType, ownerType, null, isKey);
			}
			return base.CreateXtraPropertyInfo(name, propType, isKey, attributes);
		}
		static IDictionary<string, Type> typeCache = new Dictionary<string, Type>();
		static Type GetType(string typeName) {
			Type result;
			if(!typeCache.TryGetValue(typeName, out result)) {
				if(TryGetTypeFromLoadedAssemblies(typeName, out result))
					typeCache.Add(typeName, result);
			}
			return result;
		}
		static bool TryGetTypeFromLoadedAssemblies(string typeName, out Type type) {
			type = Type.GetType(typeName, false);
			if(type == null) {
				System.Reflection.Assembly[] assemblies = GetAssemblies();
				for(int i = 0; i < assemblies.Length; i++) {
					type = assemblies[i].GetType(typeName, false);
					if(type != null) break;
				}
			}
			return type != null;
		}
#if !SILVERLIGHT
		static System.Reflection.Assembly[] GetAssemblies() {
			return AppDomain.CurrentDomain.GetAssemblies();
		}
		static DXXmlSerializer() {
			var converter = DevExpress.Utils.Serializing.Helpers.ObjectConverter.Instance;
			converter.RegisterConverter(BrushConverter<System.Windows.Media.Brush>.Instance);
			converter.RegisterConverter(BrushConverter<System.Windows.Media.LinearGradientBrush>.Instance);
			converter.RegisterConverter(BrushConverter<System.Windows.Media.RadialGradientBrush>.Instance);
			converter.RegisterConverter(BrushConverter<System.Windows.Media.ImageBrush>.Instance);
			converter.RegisterConverter(BrushConverter<System.Windows.Media.DrawingBrush>.Instance);
			converter.RegisterConverter(TextDecorationsConverter.Instance);
		}
#else
		static System.Reflection.Assembly[] GetAssemblies() {
			return assemblies.ToArray();
		}
		static List<System.Reflection.Assembly> assemblies = new List<System.Reflection.Assembly>();
		public static void RegisterAssembly(System.Reflection.Assembly assembly) {
			if(!assemblies.Contains(assembly))
				assemblies.Add(assembly);
		}
		static DXXmlSerializer() {
			ObjectConverter.Instance.RegisterConverter(GridLengthConverter.Instance);
			ObjectConverter.Instance.RegisterConverter(RectConverter.Instance);
			ObjectConverter.Instance.RegisterConverter(ThicknessTypeConverter.Instance);
			RegisterAssembly(typeof(DXXmlSerializer).Assembly);
		}
#endif
	}
}
