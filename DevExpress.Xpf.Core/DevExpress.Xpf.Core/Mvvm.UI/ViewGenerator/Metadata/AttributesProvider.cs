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
using System.Linq;
using System.Text;
using DevExpress.Mvvm.Native;
using DevExpress.Entity.Model.Metadata;
using DevExpress.Entity.Model;
using DevExpress.Utils.Filtering;
namespace DevExpress.Xpf.Core.Mvvm.UI.ViewGenerator.Metadata {
	public class DataColumnAttributesProvider: IDataColumnAttributesProvider {		
		internal static AttributeCollection GetAttributesCore(PropertyDescriptor property, Type ownerType) {
			IEnumerable<Attribute> externalAndFluentAPIAttrbutes;
			if(property.Attributes.OfType<FilterAttribute>().FirstOrDefault() != null)
				externalAndFluentAPIAttrbutes = MetadataHelper.GetExternalAndFluentAPIFilteringAttrbutes(ownerType, property.Name);
			else externalAndFluentAPIAttrbutes = MetadataHelper.GetExternalAndFluentAPIAttrbutes(ownerType, property.Name);
			return CombineAttributes(property.Attributes, externalAndFluentAPIAttrbutes);
		}
		public static AttributeCollection CombineAttributes(AttributeCollection collection, IEnumerable<Attribute> newAttributes) {
			return new AttributeCollection(collection.Cast<Attribute>().Concat(newAttributes).ToArray());
		}
		public static DataColumnAttributes GetAttributes(PropertyDescriptor property, Type ownerType = null) {
			return new DataColumnAttributes(GetAttributesCore(property, ownerType ?? property.ComponentType), () => property.Converter);
		}
		DataColumnAttributes IDataColumnAttributesProvider.GetAtrributes(PropertyDescriptor property, Type ownerType) {
			return GetAttributes(property, ownerType);
		}
	}
}
