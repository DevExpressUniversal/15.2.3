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
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Native.Excel;
namespace DevExpress.DataAccess.Wizard.Model {
	public class ExcelDataSourceModel : DataSourceModelBase, IExcelDataSourceModel {
		public ExcelDataSourceModel() { }
		public ExcelDataSourceModel(ExcelDataSourceModel other)
			: base(other) {
			ShouldSavePassword = other.ShouldSavePassword;
			SourceOptions = other.SourceOptions != null ? other.SourceOptions.Clone() : null;
			FileName = other.FileName;
			Schema = other.Schema;
		}
		#region Overrides of DataSourceModelBase
		public override object Clone() {
			return new ExcelDataSourceModel(this);
		}
		public override bool Equals(object obj) {
			ExcelDataSourceModel other = obj as ExcelDataSourceModel;
			return other != null
				   && base.Equals(obj)
				   && ShouldSavePassword == other.ShouldSavePassword
				   && Equals(SourceOptions, other.SourceOptions)
				   && string.Equals(FileName, other.FileName)
				   && SchemaEquals(Schema, other.Schema);
		}
		public override int GetHashCode() {
			return 0;
		}
		bool SchemaEquals(FieldInfo[] x, FieldInfo[] y) {
			if(ReferenceEquals(x, y))
				return true;
			if(x == null || y == null || x.GetType() != y.GetType())
				return false;
			if(x.Length != y.Length)
				return false;
			FieldInfo[] copyX = new FieldInfo[x.Length];
			Array.Copy(x, copyX, x.Length);
			FieldInfo[] copyY = new FieldInfo[y.Length];
			Array.Copy(y, copyY, y.Length);
			var comparer = new FieldInfoComparer();
			Array.Sort(copyX, comparer);
			Array.Sort(copyY, comparer);
			for(int i = 0; i < x.Length; i++) {
				if(!(copyX[i].Equals(copyY[i])))
					return false;
			}
			return true;
		}
		#endregion
		#region Implementation of IExcelDataSourceModel
		public bool ShouldSavePassword { get; set; }
		public ExcelSourceOptionsBase SourceOptions { get; set; }
		public string FileName { get; set; }
		public FieldInfo[] Schema { get; set; }
		#endregion
	}
}
