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
using System.Drawing;
using System.IO;
using System.ComponentModel;
using System.Xml;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Data.Browsing;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraCharts.Commands;
using DevExpress.Utils.Commands;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraCharts.Native {
	public static class XtraSerializingHelper {
		[ThreadStatic]
		public static bool XtraSerializing = false;
		static List<XtraObjectInfo> CreateSerializeInfo(Chart chart) {
			List<XtraObjectInfo> serializeInfo = new List<XtraObjectInfo>();
			serializeInfo.Add(new XtraObjectInfo("Chart", chart));
			return serializeInfo;
		}
		static bool TryReadXml(XmlReader reader) {
			try {
				return reader.Read();
			}
			catch(XmlException) {
				return false;
			}
		}
		static bool TryReadAndOmitWhitespace(XmlReader reader) {
			do {
				if(!TryReadXml(reader))
					return false;
			} while(reader.NodeType == XmlNodeType.Whitespace);
			return true;
		}
		public static void LoadLayoutFromStream(Chart chart, Stream stream) {
			List<XtraObjectInfo> serializeInfo = CreateSerializeInfo(chart);
			XtraSerializer serializer = new ChartXmlSerializer();
			serializer.DeserializeObjects(chart, serializeInfo.ToArray(), stream, "", null);
		}
		public static void SaveLayoutToStream(Chart chart, Stream stream) {
			List<XtraObjectInfo> serializeInfo = CreateSerializeInfo(chart);
			serializeInfo.Add(new XtraObjectInfo("", null));
			XtraSerializer serializer = new ChartXmlSerializer();
			serializer.SerializeObjects(chart, serializeInfo.ToArray(), stream, "", null);
		}
		public static bool IsValidXml(Stream stream) {
			XmlReader reader = new XmlTextReader(stream);
			if(!TryReadAndOmitWhitespace(reader))
				return false;
			if(reader.NodeType != XmlNodeType.XmlDeclaration)
				return false;
			if(!TryReadAndOmitWhitespace(reader))
				return false;
			if(reader.NodeType != XmlNodeType.Element || reader.Name != ChartXmlSerializer.Name)
				return false;
			return true;
		}
	}
	public class EmptyChartContainer : IChartContainer {
		readonly ChartContainerType containerType;
		public EmptyChartContainer(ChartContainerType containerType) {
			this.containerType = containerType;
		}
		event EventHandler IChartContainer.EndLoading { add { } remove { } }
		IChartDataProvider IChartContainer.DataProvider { get { return null; } }
		IChartRenderProvider IChartContainer.RenderProvider { get { return null; } }
		IChartEventsProvider IChartContainer.EventsProvider { get { return null; } }
		IChartInteractionProvider IChartContainer.InteractionProvider { get { return null; } }
		void IChartContainer.Assign(Chart chart) { }
		bool IChartContainer.ShowDesignerHints { get { return true; } }
		Chart IChartContainer.Chart { get { return null; } }
		bool IChartContainer.IsDesignControl { get { return false; } }
		IServiceProvider IChartContainer.ServiceProvider { get { return null; } }
		ISite IChartContainer.Site { get { return null; } set { } }
		IComponent IChartContainer.Parent { get { return null; } }
		ChartContainerType IChartContainer.ControlType { get { return containerType; } }
		bool IChartContainer.DesignMode { get { return false; } }
		bool IChartContainer.IsEndUserDesigner { get { return false; } }
		bool IChartContainer.Loading { get { return false; } }
		bool IChartContainer.ShouldEnableFormsSkins { get { return false; } }
		bool IChartContainer.CanDisposeItems { get { return true; } }
		void IChartContainer.LockChangeService() { }
		void IChartContainer.UnlockChangeService() { }
		void IChartContainer.Changing() { }
		void IChartContainer.Changed() { }
		void IChartContainer.ShowErrorMessage(string message, string title) { }
		void IChartContainer.RaiseRangeControlRangeChanged(object min, object max, bool invalidate) { }
		bool IChartContainer.GetActualRightToLeft() { return false; }
		#region ISupportBarsInteraction implementation
		CommandBasedKeyboardHandler<ChartCommandId> ICommandAwareControl<ChartCommandId>.KeyboardHandler { get { return null; } }
		event EventHandler ICommandAwareControl<ChartCommandId>.BeforeDispose { add { } remove { } }
		event EventHandler ICommandAwareControl<ChartCommandId>.UpdateUI { add { } remove { } }
		Command ICommandAwareControl<ChartCommandId>.CreateCommand(ChartCommandId id) {
			return null;
		}
		bool ICommandAwareControl<ChartCommandId>.HandleException(Exception e) {
			return false;
		}
		void ICommandAwareControl<ChartCommandId>.Focus() {
		}
		void ICommandAwareControl<ChartCommandId>.CommitImeContent() {
		}
		void ISupportBarsInteraction.RaiseUIUpdated() { }
		object IServiceProvider.GetService(Type serviceType) {
			return null;
		}
		#endregion 
	}
}
