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

using DevExpress.Mvvm.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
namespace DevExpress.Mvvm.Native {
	public class ToolBarItemAttributeBuilderProvider : CustomAttributeBuilderProviderBase<ToolBarItemAttribute> {
		internal override Expression<Func<ToolBarItemAttribute>> GetConstructorExpression() {
			return () => new ToolBarItemAttribute();
		}
		internal override IEnumerable<Tuple<PropertyInfo, object>> GetPropertyValuePairs(ToolBarItemAttribute attribute) {
			if(attribute.GetOrder() != null)
				yield return GetPropertyValuePair(attribute, x => x.Order);
			yield return GetPropertyValuePair(attribute, x => x.Page);
			yield return GetPropertyValuePair(attribute, x => x.PageGroup);
		}
	}
	public class ContextMenuItemAttributeBuilderProvider : CustomAttributeBuilderProviderBase<ContextMenuItemAttribute> {
		internal override Expression<Func<ContextMenuItemAttribute>> GetConstructorExpression() {
			return () => new ContextMenuItemAttribute();
		}
		internal override IEnumerable<Tuple<PropertyInfo, object>> GetPropertyValuePairs(ContextMenuItemAttribute attribute) {
			if(attribute.GetOrder() != null)
				yield return GetPropertyValuePair(attribute, x => x.Order);
			yield return GetPropertyValuePair(attribute, x => x.Group);
		}
	}
	public class CommandParameterAttributeBuilderProvider : CustomAttributeBuilderProviderBase<CommandParameterAttribute> {
		internal override Expression<Func<CommandParameterAttribute>> GetConstructorExpression() {
			return () => new CommandParameterAttribute(null);
		}
		internal override IEnumerable<object> GetConstructorParameters(CommandParameterAttribute attribute) {
			yield return attribute.CommandParameter;
		}
	}
	public class DXImageAttributeBuilderProvider : CustomAttributeBuilderProviderBase<DXImageAttribute> {
		internal override Expression<Func<DXImageAttribute>> GetConstructorExpression() {
			return () => new DXImageAttribute(null);
		}
		internal override IEnumerable<object> GetConstructorParameters(DXImageAttribute attribute) {
			yield return attribute.ImageName;
		}
		internal override IEnumerable<Tuple<PropertyInfo, object>> GetPropertyValuePairs(DXImageAttribute attribute) {
			yield return GetPropertyValuePair(attribute, x => x.LargeImageUri);
			yield return GetPropertyValuePair(attribute, x => x.SmallImageUri);
		}
	}
	public class ImageAttributeBuilderProvider : CustomAttributeBuilderProviderBase<ImageAttribute> {
		internal override Expression<Func<ImageAttribute>> GetConstructorExpression() {
			return () => new ImageAttribute(null);
		}
		internal override IEnumerable<object> GetConstructorParameters(ImageAttribute attribute) {
			yield return attribute.ImageUri;
		}
		internal override IEnumerable<Tuple<PropertyInfo, object>> GetPropertyValuePairs(ImageAttribute attribute) {
			yield return GetPropertyValuePair(attribute, x => x.ImageUri);
		}
	}
}
