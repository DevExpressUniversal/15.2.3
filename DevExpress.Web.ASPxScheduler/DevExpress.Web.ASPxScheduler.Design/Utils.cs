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
using System.Web.UI.Design;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Design;
namespace DevExpress.Web.ASPxScheduler.Design {
	#region ASPxDataFieldsProvider
	public class ASPxDataFieldsProvider : IDataFieldsProvider {
		ASPxSchedulerDataSourceViewSchemaAccessor accessor;
		public ASPxDataFieldsProvider(ASPxSchedulerDataSourceViewSchemaAccessor accessor) {
			this.accessor = accessor;
		}
		#region IDataFieldsProvider Members
		public bool UnboundMode {
			get {
				IDataSourceViewSchema schema = accessor.GetDataSourceViewSchema();
				if (schema == null)
					return true;
				IDataSourceFieldSchema[] fields = schema.GetFields();
				if (fields == null)
					return true;
				return fields.Length <= 0;
			}
		}
		public DataFieldInfoCollection GetDataFields() {
			DataFieldInfoCollection result = new DataFieldInfoCollection();
			IDataSourceViewSchema schema = accessor.GetDataSourceViewSchema();
			if (schema == null)
				return result;
			IDataSourceFieldSchema[] fields = schema.GetFields();
			if (fields == null)
				return result;
			int count = fields.Length;
			for (int i = 0; i < count; i++) {
				IDataSourceFieldSchema field = fields[i];
				result.Add(new DataFieldInfo(field.Name, field.DataType));
			}
			return result;
		}
		#endregion
	}
	#endregion
	#region EmptyUndoSupport
	public class EmptyUndoSupport : IDisposable {
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~EmptyUndoSupport() {
			Dispose(false);
		}
		#endregion
	}
	#endregion
}
