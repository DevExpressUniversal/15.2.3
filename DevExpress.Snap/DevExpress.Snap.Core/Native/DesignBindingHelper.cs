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
using DevExpress.Data.Browsing.Design;
using DevExpress.Data.Browsing;
using DevExpress.Snap.Core.Native.Services;
namespace DevExpress.Snap.Core.Native {
	public static class DesignBindingHelper {
		public static string GetDataMember(SnapFieldInfo fieldInfo, DesignBinding designBinding) {
			return GetDataMember(new SnapListFieldInfo(fieldInfo.PieceTable, fieldInfo.Field.Parent), designBinding);
		}
		public static string GetDataMember(SnapListFieldInfo listFieldInfo, DesignBinding designBinding) {
			DesignBinding listDesignBinding = FieldsHelper.GetFieldDesignBinding(listFieldInfo.PieceTable.DocumentModel.DataSourceDispatcher, new SnapFieldInfo(listFieldInfo.PieceTable, listFieldInfo.Field));
			string dataMember = designBinding.DataMember;
			if (listDesignBinding != null && !string.IsNullOrEmpty(listDesignBinding.DataMember) && !string.IsNullOrEmpty(designBinding.DataMember) && designBinding.DataMember.StartsWith(listDesignBinding.DataMember))
				dataMember = designBinding.DataMember.Substring(listDesignBinding.DataMember.Length + 1);
			return dataMember;
		}
		public static string GetDataMember(string path, out string fieldName) {
			int lastIndexOfDot = path.LastIndexOf(".");
			fieldName = (lastIndexOfDot > 0) ? path.Substring(lastIndexOfDot + 1, path.Length - lastIndexOfDot - 1) : path;
			return (lastIndexOfDot > 0) ? path.Substring(0, lastIndexOfDot) : String.Empty;
		}
		public static string GetListName(DataContext dataContext, object dataSource, string dataMember) {
			string fieldName;
			DataBrowser listBrowser = dataContext.GetDataBrowser(dataSource, DesignBindingHelper.GetDataMember(dataMember, out fieldName), true);
			return listBrowser != null ? listBrowser.GetListName() : string.Empty;
		}
	}
}
