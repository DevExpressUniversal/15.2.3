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

#if SILVERLIGHT
extern alias Platform;
using AssemblyInfo = Platform::AssemblyInfo;
#endif
#if !SL
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.Xpf.Core.Design.CoreUtils {
	public class RuntimeTypes {
#if !SL
		public static DXTypeInfo Interaction = new DXTypeInfo("Interaction", "DevExpress.Mvvm.UI.Interactivity", AssemblyInfo.SRAssemblyXpfCore);
		public static DXTypeInfo EventTrigger = new DXTypeInfo("EventTrigger", "DevExpress.Mvvm.UI.Interactivity", AssemblyInfo.SRAssemblyXpfCore);
		public static DXTypeInfo EventToCommand = new DXTypeInfo("EventToCommand", XmlNamespaceConstants.MvvmUINamespace, AssemblyInfo.SRAssemblyXpfCore);
		public static DXTypeInfo ServiceBase = new DXTypeInfo("ServiceBase", XmlNamespaceConstants.MvvmUINamespace, AssemblyInfo.SRAssemblyXpfCore);
		public static DXTypeInfo TabbedDocumentUIServiceTypeInfo = new DXTypeInfo("TabbedDocumentUIService", XmlNamespaceConstants.DockingNamespace, AssemblyInfo.SRAssemblyXpfDocking);
		public static DXTypeInfo IEventArgsConverter = new DXTypeInfo("IEventArgsConverter", XmlNamespaceConstants.MvvmUINamespace, AssemblyInfo.SRAssemblyXpfCore);
		public static DXTypeInfo ViewModelExtensions = new DXTypeInfo("ViewModelExtensions", XmlNamespaceConstants.MvvmUINamespace, AssemblyInfo.SRAssemblyXpfCore);
		public static DXTypeInfo DXMessageBoxService = new DXTypeInfo("DXMessageBoxService", XmlNamespaceConstants.XpfCoreNamespace, AssemblyInfo.SRAssemblyXpfCore);
		public static DXTypeInfo WindowedDocumentUIService = new DXTypeInfo("WindowedDocumentUIService", XmlNamespaceConstants.XpfCoreNamespace, AssemblyInfo.SRAssemblyXpfCore);
		public static DXTypeInfo NavigationService = new DXTypeInfo("FrameNavigationService", XmlNamespaceConstants.WindowsUINavigationNamespace, AssemblyInfo.SRAssemblyXpfControls);
		public static DXTypeInfo XPCollectionDataSource = new DXTypeInfo("XPCollectionDataSource", XmlNamespaceConstants.DataSourcesNamespace, AssemblyInfo.SRAssemblyXpfCoreExtensions);
		public static DXTypeInfo XPViewDataSource = new DXTypeInfo("XPViewDataSource", XmlNamespaceConstants.DataSourcesNamespace, AssemblyInfo.SRAssemblyXpfCoreExtensions);
		public static DXTypeInfo XPViewDataSourceProperty = new DXTypeInfo("XPViewDataSourceProperty", XmlNamespaceConstants.DataSourcesNamespace, AssemblyInfo.SRAssemblyXpfCoreExtensions);
		public static DXTypeInfo XPViewDataSourceSortProperty = new DXTypeInfo("XPViewDataSourceSortProperty", XmlNamespaceConstants.DataSourcesNamespace, AssemblyInfo.SRAssemblyXpfCoreExtensions); 
		public static DXTypeInfo XPServerCollectionDataSource = new DXTypeInfo("XPServerCollectionDataSource", XmlNamespaceConstants.DataSourcesNamespace, AssemblyInfo.SRAssemblyXpfCoreExtensions);
		public static DXTypeInfo XPInstantFeedbackDataSource = new DXTypeInfo("XPInstantFeedbackDataSource", XmlNamespaceConstants.DataSourcesNamespace, AssemblyInfo.SRAssemblyXpfCoreExtensions);
		public static DXTypeInfo RoutedEventArgsToDataRowConverter = new DXTypeInfo("RoutedEventArgsToDataRowConverter", XmlNamespaceConstants.XpfCoreNamespace, AssemblyInfo.SRAssemblyXpfCore);
		public static DXTypeInfo RoutedEventArgsToDataCellConverter = new DXTypeInfo("RoutedEventArgsToDataCellConverter", XmlNamespaceConstants.XpfCoreNamespace, AssemblyInfo.SRAssemblyXpfCore);
		public static DXTypeInfo DXRibbonWindow = new DXTypeInfo("DXRibbonWindow", XmlNamespaceConstants.RibbonNamespace, AssemblyInfo.SRAssemblyXpfRibbon);
#endif
		public static DXTypeInfo LookUpEditSettings = new DXTypeInfo("LookUpEditSettings", "DevExpress.Xpf.Grid.LookUp", AssemblyInfo.SRAssemblyXpfGrid);
		public static DXTypeInfo BarButtonGroup = new DXTypeInfo("BarButtonGroup", "DevExpress.Xpf.Ribbon", AssemblyInfo.SRAssemblyXpfRibbon);
		public static DXTypeInfo RibbonGalleryBarItem = new DXTypeInfo("RibbonGalleryBarItem", "DevExpress.Xpf.Ribbon", AssemblyInfo.SRAssemblyXpfRibbon);
	}
}
