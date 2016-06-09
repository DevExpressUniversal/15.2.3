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
using System.Windows;
using DevExpress.Design.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Mvvm.Native;
using DevExpress.Design;
using DevExpress.Design.SmartTags;
using DevExpress.Xpf.Core.Design.Wizards.DataAccessTechnologies;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public static class DataContextInfoHelper {
		public static Type GetDataContextType(IModelItem item) {
			IModelProperty property = ModelPropertyHelper.GetPropertyByName(item, FrameworkElement.DataContextProperty.Name, null);
			if(property == null) return null;
			IModelItem propertyValue = property.Value;
			Type propertyValueType = propertyValue.With(v => v.ItemType);
			if(!typeof(ViewModelSourceExtension).IsAssignableFrom(propertyValueType)) return propertyValueType;
			return (Type)ModelPropertyHelper.GetPropertyValue(propertyValue, "Type");
		}
		public static void SetDataContext(IModelItem item, IDXTypeMetadata type) {
			ModelPropertyHelper.SetPropertyValue(item, FrameworkElement.DataContextProperty.Name, () => DesignTimeObjectModelCreateService.CreateViewModelItem(item, type), null);
		}
	}
}
