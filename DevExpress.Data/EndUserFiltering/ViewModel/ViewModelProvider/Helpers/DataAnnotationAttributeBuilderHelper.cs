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

namespace DevExpress.Utils.Filtering.Internal {
	using System.Collections.Generic;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Runtime.CompilerServices;
	static class DataAnnotationAttributeBuilderHelper {
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		internal static void CheckDataAnnotations_ConditionallyAPTCAIssue<TAttrbute>() {
		}
		static object[] constructorArgs = new object[] { };
		internal static CustomAttributeBuilder Build(ConstructorInfo attributeCtor, object[] values) {
			if(attributeCtor == null)
				return null;
			return new CustomAttributeBuilder(attributeCtor, values);
		}
		internal static CustomAttributeBuilder Build(ConstructorInfo attributeCtor, PropertyInfo[] attributeProperties, object[] values) {
			if(attributeCtor == null)
				return null;
			PropertyInfo[] namedProperties;
			object[] propertyValues;
			BuildAttributeArgs(attributeProperties, values, out namedProperties, out propertyValues);
			return new CustomAttributeBuilder(attributeCtor, constructorArgs, namedProperties, propertyValues);
		}
		static void BuildAttributeArgs(PropertyInfo[] attributeProperties, object[] values, out PropertyInfo[] properties, out object[] attributeValues) {
			List<PropertyInfo> resProperties = new List<PropertyInfo>();
			List<object> resValues = new List<object>();
			for(int i = 0; i < values.Length; i++) {
				if(Equals(values[i], null) || Equals(values[i], 0))
					continue;
				resProperties.Add(attributeProperties[i]);
				resValues.Add(values[i]);
			}
			properties = resProperties.ToArray();
			attributeValues = resValues.ToArray();
		}
	}
}
