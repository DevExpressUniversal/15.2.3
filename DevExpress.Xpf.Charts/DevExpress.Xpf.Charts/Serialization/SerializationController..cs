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
using System.Linq;
using System.Text;
using System.Reflection;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public class ChartSerializationController {
		const string publicNamespace = "DevExpress.Xpf.Charts.";
		public const string DefaultID = "ChartControl";
		static Assembly ExecutingAssembly { get { return Assembly.GetExecutingAssembly(); } }
		public static bool OnShouldSerializeCollectionItem(object propertyValue) {
			if (propertyValue is Series && ((Series)propertyValue).IsAutoSeries)
				return false;
			if (propertyValue is SeriesPoint && ((SeriesPoint)propertyValue).Series.IsAutoPointsAdded)
				return false;
			return true;
		}
		public static object OnCreateContentPropertyValue(XtraCreateContentPropertyValueEventArgs e) {
			object propertyValue = XtraSerializingUtils.GetContentPropertyInstance(e.Item, ExecutingAssembly, publicNamespace);
			e.PropertyDescriptor.SetValue(e.Owner, propertyValue);
			return propertyValue;
		}
		public static object OnCreateCollectionItem(XtraCreateCollectionItemEventArgs e){
			object propertyValue;
			if (e.CollectionName == "Colors")
				propertyValue = new System.Windows.Media.Color();
			else
				propertyValue = XtraSerializingUtils.GetContentPropertyInstance(e.Item, ExecutingAssembly, publicNamespace);
			((System.Collections.IList)e.Collection).Add(propertyValue);
			return propertyValue;
		}
		public static void OnClearCollection(XtraItemRoutedEventArgs e) {
			((System.Collections.IList)e.Collection).Clear();
		}
	}
}
