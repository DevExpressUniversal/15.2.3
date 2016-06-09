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
using DevExpress.XtraReports.Native;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraReports.Parameters;
using DevExpress.Data;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.UI;
using DevExpress.Utils;
namespace DevExpress.XtraReports.UI {
	using DevExpress.LookAndFeel;
	using DevExpress.XtraReports.UserDesigner;
	using System.ComponentModel.Design;
	using DevExpress.Utils.About;
	public static class XtraReportExtensions {
		static IReportDesignTool GetDesignTool(XtraReport report) {
			IReportDesignTool tool = ((IServiceProvider)report).GetService(typeof(IReportDesignTool)) as IReportDesignTool;
			if(tool == null) {
				tool = new ReportDesignTool(report);
				IServiceContainer serv = ((IServiceProvider)report).GetService(typeof(IServiceContainer)) as IServiceContainer;
				serv.AddService(typeof(IReportDesignTool), tool);
			}
			return tool;
		}
		public static void ShowDesigner(this XtraReport report) {
			GetDesignTool(report).ShowDesigner();
		}
		public static void ShowDesignerDialog(this XtraReport report) {
			GetDesignTool(report).ShowDesignerDialog();
		}
		public static void ShowDesigner(this XtraReport report, UserLookAndFeel lookAndFeel) {
			GetDesignTool(report).ShowDesigner(lookAndFeel);
		}
		public static void ShowDesignerDialog(this XtraReport report, UserLookAndFeel lookAndFeel) {
			GetDesignTool(report).ShowDesignerDialog(lookAndFeel);
		}
		public static void ShowDesigner(this XtraReport report, UserLookAndFeel lookAndFeel, DesignDockPanelType hiddenPanels) {
			GetDesignTool(report).ShowDesigner(lookAndFeel, hiddenPanels);
		}
		public static void ShowDesignerDialog(this XtraReport report, UserLookAndFeel lookAndFeel, DesignDockPanelType hiddenPanels) {
			GetDesignTool(report).ShowDesignerDialog(lookAndFeel, hiddenPanels);
		}
		public static void ShowRibbonDesigner(this XtraReport report) {
			GetDesignTool(report).ShowRibbonDesigner();
		}
		public static void ShowRibbonDesignerDialog(this XtraReport report) {
			GetDesignTool(report).ShowRibbonDesignerDialog();
		}
		public static void ShowRibbonDesigner(this XtraReport report, UserLookAndFeel lookAndFeel) {
			GetDesignTool(report).ShowRibbonDesigner(lookAndFeel);
		}
		public static void ShowRibbonDesignerDialog(this XtraReport report, UserLookAndFeel lookAndFeel) {
			GetDesignTool(report).ShowRibbonDesignerDialog(lookAndFeel);
		}
		public static void ShowRibbonDesigner(this XtraReport report, UserLookAndFeel lookAndFeel, DesignDockPanelType hiddenPanels) {
			GetDesignTool(report).ShowRibbonDesigner(lookAndFeel, hiddenPanels);
		}
		public static void ShowRibbonDesignerDialog(this XtraReport report, UserLookAndFeel lookAndFeel, DesignDockPanelType hiddenPanels) {
			GetDesignTool(report).ShowRibbonDesignerDialog(lookAndFeel, hiddenPanels);
		}
		public static void About(this XtraReport report) {
			DevExpress.XtraReports.Extensions.ReportsAboutHelper.About();
		}
	}
}
namespace DevExpress.XtraReports.Extensions {
	[DXBrowsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public static class ReportsAboutHelper {
		public static void About() {
		}
	}
	public abstract class ReportDesignExtension : IRepositoryItemCreator, IDataSerializer {
		public static void AssociateReportWithExtension(XtraReport report, string contextName) {
			report.Extensions[SerializationService.Guid] = contextName;
			report.Extensions[DataEditorService.Guid] = contextName;
			report.Extensions[ParameterEditorService.Guid] = contextName;
		}
		public static void RegisterExtension(ReportDesignExtension extension, string contextName) {
			if(string.IsNullOrEmpty(contextName))
				throw new ArgumentException("contextName");
			if(extension == null)
				throw new ArgumentNullException("extension");
			SerializationService.RegisterSerializer(contextName, extension);
			Type[] editableTypes = extension.GetEditableDataTypes();
			foreach(Type type in editableTypes)
				DataEditorService.RegisterRepositoryItemCreator(contextName, type, extension);
			Dictionary<Type, string> parameterTypes = new Dictionary<Type, string>();
			extension.AddParameterTypes(parameterTypes);
			foreach(Type type in parameterTypes.Keys)
				ParameterEditorService.AddParameterType(contextName, type, parameterTypes[type]);
		}
		#region IRepositoryItemCreator Members
		RepositoryItem IRepositoryItemCreator.CreateItem(object instance, Type dataType, IEditingContext context) {
			if(instance is Parameter)
				return CreateRepositoryItem((Parameter)instance, dataType, context.RootComponent as XtraReport);
			if(instance is DataColumnInfo)
				return CreateRepositoryItem((DataColumnInfo)instance, dataType, context.RootComponent as XtraReport);
			return null;
		}
		protected virtual RepositoryItem CreateRepositoryItem(Parameter parameter, Type dataType, XtraReport report) {
			return null;
		}
		protected virtual RepositoryItem CreateRepositoryItem(DataColumnInfo dataColumnInfo, Type dataType, XtraReport report) {
			return null;
		}
		#endregion
		public virtual Type[] GetEditableDataTypes() {
			return new Type[] { };
		}
		public virtual void AddParameterTypes(IDictionary<Type, string> dictionary) {
		}
		[Obsolete("The GetSerializableDataTypes() method is obsolete now. Use the CanSerialize(object) and CanDeserialize(string, string) method instead.")]
		public virtual Type[] GetSerializableDataTypes() {
			return new Type[] { };
		}
		#region IDataSerializer Members
		bool IDataSerializer.CanDeserialize(string value, string typeName, object rootComponent) {
			return CanDeserialize(value, typeName);
		}
		protected virtual bool CanDeserialize(string value, string typeName) {
			return IterateTypes(delegate(Type type) {
				return type.FullName == typeName;
			});
		}
		bool IterateTypes(Predicate<Type> predicate) {
#pragma warning disable 618
			foreach(Type item in GetSerializableDataTypes()) {
				if(predicate(item))
					return true;
			}
			return false;
#pragma warning restore 618
		}
		object IDataSerializer.Deserialize(string value, string typeName, object rootComponent) {
			return DeserializeData(value, typeName, rootComponent as XtraReport);
		}
		protected virtual object DeserializeData(string value, string typeName, XtraReport report) {
			object result = null;
			IterateTypes(delegate(Type type) {
				if(type.FullName == typeName) {
#pragma warning disable 618
					result = DeserializeData(value, type, report);
#pragma warning restore 618
					return true;
				}
				return false;
			});
			return result;
		}
		[Obsolete("The DeserializeData(string, Type, XtraReport) method is obsolete now. Use the DeserializeData(string, string, XtraReport) method instead.")]
		protected virtual object DeserializeData(string value, Type destType, XtraReport report) {
			return null;
		}
		bool IDataSerializer.CanSerialize(object data, object rootComponent) {
			return CanSerialize(data);
		}
		protected virtual bool CanSerialize(object data) {
			if(data == null)
				return false;
			return IterateTypes(delegate(Type type) {
				return type.FullName == data.GetType().FullName;
			});
		}
		string IDataSerializer.Serialize(object data, object rootComponent) {
			return SerializeData(data, rootComponent as XtraReport);
		}
		protected virtual string SerializeData(object data, XtraReport report) {
			return string.Empty;
		}
		#endregion
	}
}
