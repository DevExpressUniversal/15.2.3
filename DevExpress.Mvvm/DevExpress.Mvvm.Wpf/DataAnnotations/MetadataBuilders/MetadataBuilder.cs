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

using DevExpress.Mvvm.Native;
using System;
using System.Linq.Expressions;
using System.Windows.Input;
namespace DevExpress.Mvvm.DataAnnotations {
	public interface IMetadataProvider<T> {
		void BuildMetadata(MetadataBuilder<T> builder);
	}
	public abstract class ClassMetadataBuilder<T> : MetadataBuilderBase<T, ClassMetadataBuilder<T>> {
		TBuilder GetBuilder<TProperty, TBuilder>(Expression<Func<T, TProperty>> propertyExpression, Func<MemberMetadataStorage, TBuilder> createBuilderCallBack) where TBuilder : IPropertyMetadataBuilder {
			return GetBuilder(GetPropertyName(propertyExpression), createBuilderCallBack);
		}
		protected internal PropertyMetadataBuilder<T, TProperty> PropertyCore<TProperty>(Expression<Func<T, TProperty>> propertyExpression) {
			return GetBuilder(propertyExpression, x => new PropertyMetadataBuilder<T, TProperty>(x, this));
		}
#if !FREE
		protected internal FilteringPropertyMetadataBuilder<T, TProperty> FilteringPropertyCore<TProperty>(Expression<Func<T, TProperty>> propertyExpression) {
			return GetBuilder(propertyExpression, x => new FilteringPropertyMetadataBuilder<T, TProperty>(x, this));
		}
#endif
		protected internal CommandMethodMetadataBuilder<T> CommandFromMethodCore(Expression<Action<T>> methodExpression) {
			return CommandFromMethodInternal(methodExpression).AddOrModifyAttribute<CommandAttribute>();
		}
		internal CommandMethodMetadataBuilder<T> CommandFromMethodInternal(Expression<Action<T>> methodExpression) {
			string methodName = GetMethod(methodExpression).Name;
			return GetBuilder(methodName, x => new CommandMethodMetadataBuilder<T>(x, this, methodName));
		}
#if !FREE
		protected internal CommandMetadataBuilder<T> CommandCore(Expression<Func<T, ICommand>> propertyExpression) {
			return GetBuilder(propertyExpression, x => new CommandMetadataBuilder<T>(x, this));
		}
		protected TableGroupContainerLayoutBuilder<T> TableLayoutCore() {
			return new TableGroupContainerLayoutBuilder<T>(this);
		}
		protected ToolBarLayoutBuilder<T> ToolBarLayoutCore() {
			return new ToolBarLayoutBuilder<T>(this);
		}
		internal int CurrentDisplayAttributeOrder { get; set; }
		internal int CurrentDataFormLayoutOrder { get; set; }
		internal int CurrentTableLayoutOrder { get; set; }
		internal int CurrentToolbarLayoutOrder { get; set; }
		internal int CurrentContextMenuLayoutOrder { get; set; }
#endif
	}
	public class MetadataBuilder<T> : ClassMetadataBuilder<T> {
		public PropertyMetadataBuilder<T, TProperty> Property<TProperty>(Expression<Func<T, TProperty>> propertyExpression) {
			return PropertyCore(propertyExpression);
		}
		public CommandMethodMetadataBuilder<T> CommandFromMethod(Expression<Action<T>> methodExpression) {
			return CommandFromMethodCore(methodExpression);
		}
#if !FREE
		public CommandMetadataBuilder<T> Command(Expression<Func<T, ICommand>> propertyExpression) {
			return CommandCore(propertyExpression);
		}
		public DataFormLayoutBuilder<T, MetadataBuilder<T>> DataFormLayout() {
			return new DataFormLayoutBuilder<T, MetadataBuilder<T>>(this);
		}
		public TableGroupContainerLayoutBuilder<T> TableLayout() { return TableLayoutCore(); }
		public ToolBarLayoutBuilder<T> ToolBarLayout() { return ToolBarLayoutCore(); }
		public GroupBuilder<T, MetadataBuilder<T>> Group(string groupName) {
			return new GroupBuilder<T, MetadataBuilder<T>>(this, groupName);
		}
#endif
	}
}
