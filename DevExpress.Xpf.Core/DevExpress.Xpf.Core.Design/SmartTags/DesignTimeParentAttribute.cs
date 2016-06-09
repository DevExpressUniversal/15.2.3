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

extern alias Platform;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Microsoft.Windows.Design.Model;
using System.Linq;
#if SL
using FrameworkElement = Platform::System.Windows.FrameworkElement;
#endif
namespace DevExpress.Xpf.Core.Design.SmartTags {
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public sealed class DesignTimeParentAttribute : Attribute {
		readonly Type designTimeParentType;
		readonly Type viewProvider;
		public DesignTimeParentAttribute(Type designTimeParentType, Type viewProvider = null) {
			if(designTimeParentType == null)
				throw new ArgumentNullException("designTimeParentType");
			this.designTimeParentType = designTimeParentType;
			this.viewProvider = viewProvider;
		}
		public Type ParentType {
			get { return designTimeParentType; }
		}
		public Type ViewProvider {
			get { return viewProvider; }
		}
		public override object TypeId {
			get { return ParentType.GetHashCode(); }
		}
	}
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public sealed class UseParentPropertyLinesAttribute : Attribute {
		readonly Type parentType;
		public UseParentPropertyLinesAttribute(Type parentType) {
			this.parentType = parentType;
		}
		public Type ParentType {
			get { return parentType; }
		}
		public override object TypeId {
			get { return ParentType.GetHashCode(); }
		}
	}
	public interface IViewProvider {
		FrameworkElement ProvideView(ModelItem item);
	}
	public static class AttributeHelper {
		public static IEnumerable<T> GetAttributes<T>(Type itemType) where T : Attribute {
			List<T> result = new List<T>();
			while(itemType != null) {
				var collection = itemType.GetCustomAttributes(typeof(T), true).Union(TypeDescriptor.GetProvider(itemType).GetTypeDescriptor(itemType).GetAttributes().OfType<T>());
				result.AddRange(collection.Cast<T>());
				itemType = itemType.BaseType;
			}
			return result;
		}
	}
}
