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
using System.Globalization;
namespace DevExpress.Mvvm.DataAnnotations {
	public abstract class NestedBuilderBase<T, TProperty, TAttribute, TBuilder, TParentBuilder>
		where TBuilder : NestedBuilderBase<T, TProperty, TAttribute, TBuilder, TParentBuilder>
		where TAttribute : Attribute, new()
		where TParentBuilder : PropertyMetadataBuilderBase<T, TProperty, TParentBuilder> {
		readonly TParentBuilder parent;
		public NestedBuilderBase(TParentBuilder parent) {
			this.parent = parent;
		}
		protected TBuilder ChangeAttribute(Action<TAttribute> action) {
			parent.AddOrModifyAttribute<TAttribute>(action);
			return (TBuilder)this;
		}
		protected TParentBuilder EndCore() {
			return parent;
		}
	}
	public abstract class MaskBuilderBase<T, TProperty, TMaskAttribute, TBuilder, TParentBuilder> :
		NestedBuilderBase<T, TProperty, TMaskAttribute, TBuilder, TParentBuilder>
		where TBuilder : MaskBuilderBase<T, TProperty, TMaskAttribute, TBuilder, TParentBuilder>
		where TMaskAttribute : MaskAttributeBase, new()
		where TParentBuilder : PropertyMetadataBuilderBase<T, TProperty, TParentBuilder> {
		public MaskBuilderBase(TParentBuilder parent)
			: base(parent) {
		}
		internal TBuilder MaskCore(string mask, bool useMaskAsDisplayFormat) {
			return ChangeAttribute(x => { x.Mask = mask; x.UseAsDisplayFormat = useMaskAsDisplayFormat; });
		}
		public TParentBuilder EndMask() {
			return EndCore();
		}
	}
	public class NumericMaskBuilder<T, TProperty, TParentBuilder> :
		MaskBuilderBase<T, TProperty, NumericMaskAttribute, NumericMaskBuilder<T, TProperty, TParentBuilder>, TParentBuilder>
		where TParentBuilder : PropertyMetadataBuilderBase<T, TProperty, TParentBuilder> {
		public NumericMaskBuilder(TParentBuilder parent)
			: base(parent) {
		}
		public NumericMaskBuilder<T, TProperty, TParentBuilder> MaskCulture(CultureInfo culture) {
			return ChangeAttribute(x => x.CultureInfo = culture);
		}
	}
	public class DateTimeMaskBuilder<T, TParentBuilder> :
		MaskBuilderBase<T, DateTime, DateTimeMaskAttribute, DateTimeMaskBuilder<T, TParentBuilder>, TParentBuilder>
		where TParentBuilder : PropertyMetadataBuilderBase<T, DateTime, TParentBuilder> {
		public DateTimeMaskBuilder(TParentBuilder parent)
			: base(parent) {
		}
		public DateTimeMaskBuilder<T, TParentBuilder> MaskCulture(CultureInfo culture) {
			return ChangeAttribute(x => x.CultureInfo = culture);
		}
		public DateTimeMaskBuilder<T, TParentBuilder> MaskAutomaticallyAdvanceCaret() {
			return ChangeAttribute(x => x.AutomaticallyAdvanceCaret = true);
		}
	}
	public class RegExMaskBuilderBase<T, TProperty, TMaskAttribute, TBuilder, TParentBuilder> :
		MaskBuilderBase<T, TProperty, TMaskAttribute, TBuilder, TParentBuilder>
		where TBuilder : RegExMaskBuilderBase<T, TProperty, TMaskAttribute, TBuilder, TParentBuilder>
		where TMaskAttribute : RegExMaskAttributeBase, new()
		where TParentBuilder : PropertyMetadataBuilderBase<T, TProperty, TParentBuilder> {
		public RegExMaskBuilderBase(TParentBuilder parent)
			: base(parent) {
		}
		public TBuilder MaskDoNotIgnoreBlank() {
			return ChangeAttribute(x => x.IgnoreBlank = false);
		}
		public TBuilder MaskPlaceHolder(char placeHolder) {
			return ChangeAttribute(x => x.PlaceHolder = placeHolder);
		}
	}
	public class SimpleMaskBuilder<T, TProperty, TParentBuilder> :
		RegExMaskBuilderBase<T, TProperty, SimpleMaskAttribute, SimpleMaskBuilder<T, TProperty, TParentBuilder>, TParentBuilder>
		where TParentBuilder : PropertyMetadataBuilderBase<T, TProperty, TParentBuilder> {
		public SimpleMaskBuilder(TParentBuilder parent)
			: base(parent) {
		}
		public SimpleMaskBuilder<T, TProperty, TParentBuilder> MaskDoNotSaveLiteral() {
			return ChangeAttribute(x => x.SaveLiteral = false);
		}
	}
	public class RegularMaskBuilder<T, TProperty, TParentBuilder> :
		RegExMaskBuilderBase<T, TProperty, RegularMaskAttribute, RegularMaskBuilder<T, TProperty, TParentBuilder>, TParentBuilder>
		where TParentBuilder : PropertyMetadataBuilderBase<T, TProperty, TParentBuilder> {
		public RegularMaskBuilder(TParentBuilder parent)
			: base(parent) {
		}
		public RegularMaskBuilder<T, TProperty, TParentBuilder> MaskDoNotSaveLiteral() {
			return ChangeAttribute(x => x.SaveLiteral = false);
		}
	}
	public class RegExMaskBuilder<T, TProperty, TParentBuilder> :
		RegExMaskBuilderBase<T, TProperty, RegExMaskAttribute, RegExMaskBuilder<T, TProperty, TParentBuilder>, TParentBuilder>
		where TParentBuilder : PropertyMetadataBuilderBase<T, TProperty, TParentBuilder> {
		public RegExMaskBuilder(TParentBuilder parent)
			: base(parent) {
		}
		public RegExMaskBuilder<T, TProperty, TParentBuilder> MaskDoNotShowPlaceHolders() {
			return ChangeAttribute(x => x.ShowPlaceHolders = false);
		}
	}
}
