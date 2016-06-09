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
using System.ComponentModel;
using DevExpress.XtraReports.Design;
using DevExpress.Data.Browsing.Design;
using DevExpress.Snap.Core.API;
using System.Collections;
using System.Diagnostics;
namespace DevExpress.Snap.Extensions.Native {
	public class FieldNameEditor : DesignBindingEditor {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if (provider == null)
				return value;
			IDataSourceCollectionProvider dataSourceCollectionProvider = provider.GetService(typeof(IDataSourceCollectionProvider)) as IDataSourceCollectionProvider;
			if (dataSourceCollectionProvider == null)
				return value;
			IList dataSources = dataSourceCollectionProvider.GetDataSourceCollection(provider);
#if DEBUGTEST
			Debug.Assert(dataSources.Count == 1);
#endif
			value = new DesignBinding(dataSources[0], (string)value ?? string.Empty);
			return ((DesignBinding)base.EditValue(context, provider, value)).DataMember;
		}
	}
}
