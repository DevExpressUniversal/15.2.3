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
using System.ComponentModel;
using DevExpress.XtraVerticalGrid.Data;
using DevExpress.Entity.Model;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using System.Linq;
namespace DevExpress.Xpf.PropertyGrid.Internal {
	public class DescriptorContextFactory {
		internal protected DescriptorContext Create(object obj, PropertyDescriptor pd, string fieldName, DescriptorContext parentContext, bool isMultiSource, IServiceProvider serviceProvider) {
			DescriptorContext context = CreateInternal(obj, pd, fieldName, parentContext, isMultiSource, serviceProvider);
			context.FieldName = fieldName;
			return context;
		}
		DescriptorContext CreateInternal(object obj, PropertyDescriptor pd, string fieldName, DescriptorContext parentContext, bool isMultiSource, IServiceProvider serviceProvider) {
			if (parentContext == null) {
				return new RootDescriptorContext(obj, serviceProvider, isMultiSource) { RowHandle = RowHandle.Root };
			}
			if (parentContext != null && parentContext.PropertyDescriptor != null && (parentContext.PropertyDescriptor.Converter.GetCreateInstanceSupported() || parentContext.PropertyType.IsValueType)) {
				return new ImmutableDescriptorContext(obj, pd, serviceProvider, isMultiSource) { RowHandle = CreateRowHandle() };
			}
			if (ListConverter.IsItemProperty(pd)) {
				return new ListItemDescriptorContext(obj, pd, serviceProvider, isMultiSource) { RowHandle = CreateRowHandle() };
			}
			if (pd != null)
				return new PropertyDescriptorContext(obj, pd, serviceProvider, isMultiSource) { RowHandle = CreateRowHandle() };
			return new PropertyDescriptorContext(null, null, serviceProvider, isMultiSource) { RowHandle = RowHandle.Invalid };
		}
		RowHandle CreateRowHandle() {
			return new RowHandle();
		}
		internal protected void AddContext(DescriptorContext context, bool isMultiSource, PGridDataModeHelperContextCache cache, DescriptorContext parentContext) {
			cache[isMultiSource, context.RowHandle] = context;
			cache[isMultiSource, context.FieldName] = context;
			context.ParentContext = parentContext;
		}
	}
}
