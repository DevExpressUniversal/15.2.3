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
using System.Text;
using System.Data;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using System.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.Data.Browsing.Design;
namespace DevExpress.Data.Browsing.Design {
	public interface IColumnImageProvider {
		[Obsolete("Use the GetColumnImageIndex(PropertyDescriptor, TypeSpecifics) method instead")]
		int GetColumnImageIndex(PropertyDescriptor property, string dataMember, bool isList);
		[Obsolete("Use the GetDataSourceImageIndex(object, TypeSpecifics) method instead")]
		int GetDataSourceImageIndex(object dataSource);
		int GetColumnImageIndex(PropertyDescriptor property, TypeSpecifics specifics);
		int GetDataSourceImageIndex(object dataSource, TypeSpecifics specifics);
		int GetNoneImageIndex();
		ImageCollection CreateImageCollection();
	}
	public class ColumnImageProvider : IColumnImageProvider {
		const int
			LIST_IMAGE = 0,
			LIST_OF_PARAMETERS_IMAGE = 1,
			COLUMN_DEFAULT_IMAGE = 2,
			CALC_DEFAULT_IMAGE = 3,
			NONE_IMAGE = 4,
			LIST_SOURCE_IMAGE = 5,
			COLUMN_STRING_IMAGE = 6,
			COLUMN_INT_IMAGE = 7,
			COLUMN_FLOAT_IMAGE = 8,
			COLUMN_DATE_IMAGE = 9,
			COLUMN_BOOL_IMAGE = 10,
			COLUMN_ARRAY_IMAGE = 11,
			COLUMN_GUID_IMAGE = 12,
			CALC_STRING_IMAGE = 13,
			CALC_INT_IMAGE = 14,
			CALC_FLOAT_IMAGE = 15,
			CALC_DATE_IMAGE = 16,
			CALC_BOOL_IMAGE = 17,
			CALC_GUID_IMAGE = 18;
		static IColumnImageProvider instance;
		public static IColumnImageProvider Instance {
			get {
				if(instance == null)
					instance = new ColumnImageProvider();
				return instance;
			}
			set {
				instance = value;
			}
		}
		Dictionary<TypeSpecifics, int> dict = new Dictionary<TypeSpecifics, int>();
		public ColumnImageProvider() {
			dict.Add(TypeSpecifics.List, LIST_IMAGE);
			dict.Add(TypeSpecifics.ListSource, LIST_SOURCE_IMAGE);
			dict.Add(TypeSpecifics.ListOfParameters, LIST_OF_PARAMETERS_IMAGE);
			dict.Add(TypeSpecifics.None, NONE_IMAGE);
			dict.Add(TypeSpecifics.Array, COLUMN_ARRAY_IMAGE);
			dict.Add(TypeSpecifics.String, COLUMN_STRING_IMAGE);
			dict.Add(TypeSpecifics.Integer, COLUMN_INT_IMAGE);
			dict.Add(TypeSpecifics.Float, COLUMN_FLOAT_IMAGE);
			dict.Add(TypeSpecifics.Bool, COLUMN_BOOL_IMAGE);
			dict.Add(TypeSpecifics.Date, COLUMN_DATE_IMAGE);
			dict.Add(TypeSpecifics.Guid, COLUMN_GUID_IMAGE);
			dict.Add(TypeSpecifics.Default, COLUMN_DEFAULT_IMAGE);
			dict.Add(TypeSpecifics.CalcString, CALC_STRING_IMAGE);
			dict.Add(TypeSpecifics.CalcInteger, CALC_INT_IMAGE);
			dict.Add(TypeSpecifics.CalcFloat, CALC_FLOAT_IMAGE);
			dict.Add(TypeSpecifics.CalcBool, CALC_BOOL_IMAGE);
			dict.Add(TypeSpecifics.CalcDate, CALC_DATE_IMAGE);
			dict.Add(TypeSpecifics.CalcGuid, CALC_GUID_IMAGE);
			dict.Add(TypeSpecifics.CalcDefault, CALC_DEFAULT_IMAGE);
		}
		public virtual int GetNoneImageIndex() {
			return NONE_IMAGE;
		}
		[Obsolete("Use the GetDataSourceImageIndex(object, ITypeSpecificService) method instead")]
		public virtual int GetDataSourceImageIndex(object dataSource) {
			return GetDataSourceImageIndex(dataSource, TypeSpecifics.List);
		}
		[Obsolete("Use the GetColumnImageIndex(PropertyDescriptor, ITypeSpecificService) method instead")]
		public virtual int GetColumnImageIndex(PropertyDescriptor property, string dataMember, bool isList) {
			return GetColumnImageIndex(property, TypeSpecifics.Default);
		}
		public virtual int GetDataSourceImageIndex(object dataSource, TypeSpecifics specifics) {
			int result = COLUMN_DEFAULT_IMAGE;
			dict.TryGetValue(specifics, out result);
			return result;
		}
		public virtual int GetColumnImageIndex(PropertyDescriptor property, TypeSpecifics specifics) {
			int result = COLUMN_DEFAULT_IMAGE;
			dict.TryGetValue(specifics, out result);
			return result;
		}
		public virtual ImageCollection CreateImageCollection() {
			return ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraTreeList.Images.DataPickerImages.png", typeof(ColumnImageProvider).Assembly, new Size(16, 16));
		}
	}
}
