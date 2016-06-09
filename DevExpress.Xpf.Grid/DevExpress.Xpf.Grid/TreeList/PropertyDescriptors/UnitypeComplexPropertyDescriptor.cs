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
using System.ComponentModel;
using DevExpress.Data;
#if SL
using DevExpress.Data.Browsing;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.Xpf.Grid.TreeList {
	public class TreeListComplexPropertyDescriptor : DevExpress.Data.Access.ComplexPropertyDescriptorReflection {
		protected TreeListDataProvider Provider;
		public TreeListComplexPropertyDescriptor(TreeListDataProvider provider, object sourceObject, string path)
			: base(sourceObject, path) {
			Provider = provider;
		}
		public TreeListComplexPropertyDescriptor(TreeListDataProvider provider, DataControllerBase controller, string path)
			: base(controller, path) {
			Provider = provider;
		}
		protected override PropertyDescriptor GetDescriptor(string name, object obj, Type type) {
			if(Provider != null) {
				DataColumnInfo columnInfo = Provider.Columns[name];
				if(columnInfo != null)
					return columnInfo.PropertyDescriptor;
			}
			return base.GetDescriptor(name, obj, type);
		}
	}
	public class UnitypeComplexPropertyDescriptor : TreeListComplexPropertyDescriptor {
		public UnitypeComplexPropertyDescriptor(TreeListDataProvider provider, object sourceObject, string path) : base(provider, sourceObject, path) { }
		public UnitypeComplexPropertyDescriptor(TreeListDataProvider provider, DataControllerBase controller, string path) : base(provider, controller, path) { }
		protected override PropertyDescriptor GetDescriptor(string name, object obj, Type type) {
			PropertyDescriptor descriptor = base.GetDescriptor(name, obj, type);
			if(descriptor != null)
				return new UnitypeDataPropertyDescriptor(descriptor);
			return null;
		}
	}
}
