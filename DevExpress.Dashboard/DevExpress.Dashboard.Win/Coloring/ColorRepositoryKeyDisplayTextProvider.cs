#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.Coloring {
	public static class ColorRepositoryKeyDisplayTextProvider {
		public static string GetDisplayText(ColorRepositoryKey key, IDataSourceInfoProvider dataInfoProvider) {
			System.Text.StringBuilder str = new System.Text.StringBuilder();
			DataSourceInfo dataInfo = dataInfoProvider.GetDataSourceInfo(key.DataSourceName, key.DataMember);
			IDataSourceSchema dataSchemaProvider = dataInfo != null ? dataInfo.DataSource.GetDataSourceSchema(key.DataMember) : null;
			if (dataSchemaProvider != null)
				str.Append(dataSchemaProvider.DataSourceDisplayName).Append(": ");
			for (int i = 0; i < key.DimensionDefinitions.Count; i++) {
				if (i > 0)
					str.Append(" | ");
				DimensionDefinition definition = key.DimensionDefinitions[i];
				str.Append(DataItemDefinitionDisplayTextProvider.GetDimensionDefinitionDisplayText(definition, dataSchemaProvider));
			}
			return str.ToString();
		}
	}
}
