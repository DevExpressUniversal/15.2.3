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

namespace DevExpress.Design.Metadata {
	using System;
	static class AttributeHelper {
		internal static TValue GetAttributeValue<TValue, TAttribute>(Type t, Converter<TAttribute, TValue> converter) {
			var attributes = t.GetCustomAttributes(typeof(TAttribute), true);
			return (attributes.Length > 0) ? converter((TAttribute)attributes[0]) : default(TValue);
		}
		internal static TValue[] GetAttributeValues<TValue, TAttribute>(Type t, Converter<TAttribute, TValue> converter) {
			var attributes = t.GetCustomAttributes(typeof(TAttribute), true);
			TValue[] result = new TValue[attributes.Length];
			for(int i = 0; i < result.Length; i++)
				result[i] = converter((TAttribute)attributes[i]);
			return result;
		}
		internal static T[] CastAttributes<T, TAttribute>(Type t)
			where T : class
			where TAttribute : T {
			return GetAttributeValues<T, TAttribute>(t, (a) => a as T);
		}
	}
}
