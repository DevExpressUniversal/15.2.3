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
using System.Reflection;
using DevExpress.Snap.Core;
using DevExpress.Snap.Core.Native;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native.Lines;
using DevExpress.XtraReports.Design;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Extensions.Native.ActionLists;
using DevExpress.Snap.Core.Native.Data;
using System.ComponentModel;
using DevExpress.Snap.Core.Commands;
using DevExpress.XtraReports.Native.Parameters;
using DevExpress.LookAndFeel.DesignService;
namespace DevExpress.Snap.Extensions.Native {
	public static class LineControllerCreator {
		public static BaseLineController[] CreateMergeFieldLineControllers(SnapFieldInfo fieldInfo, CalculatedFieldBase parsedInfo, SNSmartTagService frSmartTagService) {
			Guard.ArgumentNotNull(fieldInfo, "fieldInfo");
			Guard.ArgumentNotNull(parsedInfo, "parsedInfo");
			object[] attributes = parsedInfo.GetType().GetCustomAttributes(typeof(ActionListDesignerAttribute), true);
			ActionListDesigner designer;
			if (attributes != null && attributes.Length == 1) {
				designer = (ActionListDesigner)Activator.CreateInstance(((ActionListDesignerAttribute)attributes[0]).DesignerActionListType, fieldInfo, parsedInfo, frSmartTagService);
			}
			else {
				ActionListAttribute[] actionListsAttributes = (ActionListAttribute[])parsedInfo.GetType().GetCustomAttributes(typeof(ActionListAttribute), true);
				List<ActionListAttribute> atrs = new List<ActionListAttribute>(actionListsAttributes);
				atrs.Sort((a1, a2) => { return a1.Order - a2.Order; });
				List<Type> result = new List<Type>();
				foreach (ActionListAttribute attribute in atrs) {
					result.Add(attribute.ActionListType);
				}
				designer = new MergeFieldActionListDesigner<CalculatedFieldBase>(result.ToArray(), fieldInfo, parsedInfo, frSmartTagService);
			}
			designer.Initialize(new ComponentImplementation(parsedInfo));
			return frSmartTagService.CreateLineControllers(designer);
		}
	}
	public class MergeFieldActionListDesigner<T> : ActionListDesigner, IServiceProvider where T : CalculatedFieldBase {
		readonly SnapFieldInfo fieldInfo;		
		readonly T parsedInfo;
		readonly LookAndFeelService lookAndFeelService = new LookAndFeelService();
		readonly SNSmartTagService frSmartTagService;
		readonly IDataSourceCollectionProvider dataSourceCollectionProvider;
		readonly FieldChanger fieldChanger;
		readonly IParsedInfoProvider parsedInfoProvider;
		public MergeFieldActionListDesigner(SnapFieldInfo fieldInfo, T parsedInfo, SNSmartTagService frSmartTagService)
			: this(null, fieldInfo, parsedInfo, frSmartTagService) {
		}		
		public MergeFieldActionListDesigner(Type[] actionListTypes, SnapFieldInfo fieldInfo, T parsedInfo, SNSmartTagService frSmartTagService)
			: base(actionListTypes) {
			Guard.ArgumentNotNull(fieldInfo, "fieldInfo");
			Guard.ArgumentNotNull(parsedInfo, "parsedInfo");
			Guard.ArgumentNotNull(frSmartTagService, "frSmartTagService");
			this.frSmartTagService = frSmartTagService;
			this.fieldInfo = fieldInfo;
			this.parsedInfo = parsedInfo;
			this.dataSourceCollectionProvider = GetDataSourceCollectionProvider();
			this.parsedInfoProvider = new ParsedInfoProvider(fieldInfo);
			this.fieldChanger = new FieldChanger(fieldInfo, parsedInfoProvider);
		}
		protected virtual IDataSourceCollectionProvider GetDataSourceCollectionProvider() {
			return new DataSourceCollectionProvider();
		}
		protected SnapFieldInfo FieldInfo { get { return fieldInfo; } }
		protected T ParsedInfo { get { return (T)parsedInfoProvider.GetParsedInfo(); } }
		protected override IDesignerActionList CreateActionList(Type actionListType) {
			Type[] fieldCtorArguments = new Type[] { typeof(SnapFieldInfo), typeof(IServiceProvider) };
			ConstructorInfo ci = actionListType.GetConstructor(fieldCtorArguments);
			if (ci != null)
				return (IDesignerActionList)ci.Invoke(new object[] { fieldInfo, this });
			return base.CreateActionList(actionListType);
		}
		#region IServiceProvider Members
		public object GetService(Type serviceType) {
			if (serviceType == typeof(IParsedInfoProvider))
				return parsedInfoProvider;
			if (serviceType == typeof(IFieldChanger))
				return fieldChanger;
			if (serviceType == typeof(SnapFieldInfo))
				return fieldInfo;
			if (serviceType == typeof(SnapDocumentModel))
				return fieldInfo.PieceTable.DocumentModel;
			if (serviceType == typeof(ILookAndFeelService))
				return lookAndFeelService;
			if (serviceType == typeof(IDataSourceCollectionProvider))
				return dataSourceCollectionProvider;
			return frSmartTagService.GetService(serviceType);
		}
		#endregion
	}
	public class DataSourceCollectionProvider : IDataSourceCollectionProvider {
		#region IDataSourceCollectionProvider Members
		public object[] GetDataSourceCollection(IServiceProvider serviceProvider) {
			SnapFieldInfo snapFieldInfo = (SnapFieldInfo)serviceProvider.GetService(typeof(SnapFieldInfo));
			object dataSource = FieldsHelper.GetFieldDesignBinding(snapFieldInfo.PieceTable.DocumentModel.DataSourceDispatcher, snapFieldInfo).DataSource;
			object parameters = snapFieldInfo.PieceTable.DocumentModel.ParametersDataSource;
			return new[] { dataSource, parameters };
		}
		#endregion
	}
	public class SparklineDataSourceCollectionProvider : IDataSourceCollectionProvider {
		#region IDataSourceCollectionProvider
		public object[] GetDataSourceCollection(IServiceProvider serviceProvider) {
			SnapFieldInfo snapFieldInfo = (SnapFieldInfo)serviceProvider.GetService(typeof(SnapFieldInfo));
			object dataSource = null;
			ICollection<Core.API.DataSourceInfo> infos = snapFieldInfo.PieceTable.DocumentModel.DataSourceDispatcher.GetInfos();
			if (infos.Count > 0) {
				foreach (Core.API.DataSourceInfo info in infos) {
					if (info.DataSource == null)
						continue;
					dataSource = info.DataSource;
					DataSourceName = info.DataSourceName;
					break;
				}
			}
			object parameters = snapFieldInfo.PieceTable.DocumentModel.ParametersDataSource;
			return new[] { dataSource, parameters };
		}
		#endregion
		public string DataSourceName { get; set; }
	}
}
