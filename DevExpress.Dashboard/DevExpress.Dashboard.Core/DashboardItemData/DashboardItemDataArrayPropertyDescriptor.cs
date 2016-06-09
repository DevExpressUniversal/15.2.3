#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DataAccess.Native.Data;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon.Data {
	public class DashboardItemDataArrayPropertyDescriptor : ReadOnlyPropertyDescriptor {
		readonly int index;
		readonly Type propertyType;
		readonly string name;
		readonly string displayName;
		public override Type PropertyType { get { return propertyType; } }
		public DashboardItemDataArrayPropertyDescriptor(int index, PropertyDescriptor sourcePropertyDescriptor)
			: this(index, sourcePropertyDescriptor.PropertyType, sourcePropertyDescriptor.Name, sourcePropertyDescriptor.DisplayName) {
		}
		internal DashboardItemDataArrayPropertyDescriptor(int index, Type propertyType, string name, string displayName)
			: base(name, displayName) {
			this.index = index;
			this.propertyType = propertyType;
			this.name = name;
			this.displayName = displayName;
		}
		public override object GetValue(object component) {
			return ((object[])component)[index];
		}
		public DashboardItemDataArrayPropertyDescriptor CreateNewPropertyDescriptor(Type propertyType) {
			return new DashboardItemDataArrayPropertyDescriptor(index, propertyType, name, displayName);
		}
   }
}
