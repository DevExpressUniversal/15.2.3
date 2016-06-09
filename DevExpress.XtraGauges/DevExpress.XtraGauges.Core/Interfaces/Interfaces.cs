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
using System.ComponentModel;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Base;
using System.Drawing;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.Utils.Serializing;
using DevExpress.Data;
using DevExpress.XtraGauges.Core.Customization;
using DevExpress.XtraGauges.Base;
using DevExpress.Utils.Serializing.Helpers;
using System.IO;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Imaging;
#if !DXPORTABLE
using System.Drawing.Design;
using System.Drawing.Imaging;
#endif
namespace DevExpress.XtraGauges.Base {
	public enum GaugeType { Circular, Linear, Digital, StateIndicator }
	public interface IPlatformTypeProvider {
		bool IsWin { get; }
	}
	public interface IGauge : IComponent, ISupportAcceptOrder , INamed{
		IGaugeContainer Container { get;}
		BaseGaugeModel Model { get;}
		void SetContainer(IGaugeContainer container);
		string Category { get;}
		Rectangle Bounds { get; set;}
		bool SuppressDrawBorder { get; set;}
		bool CanRemoveGaugeElement(BaseElement<IRenderableElement> element);
		BaseElement<IRenderableElement> DuplicateElement(BaseElement<IRenderableElement> element);
		void RemoveGaugeElement(BaseElement<IRenderableElement> element);
		void AddGaugeElement(BaseElement<IRenderableElement> element);
		void CheckEnabledState(bool enabled);
		List<string> GetNames();
		void Clear();
		void ForceUpdateChildren();
		void BeginUpdate();
		void EndUpdate();
		void InitializeDefault();
		ColorScheme GetColorScheme();
	}
	public interface IGaugeContainer {
		GaugeCollection Gauges { get; }
		IGauge AddGauge(GaugeType type);
		string Name { get;}
		Rectangle Bounds { get;}
		void UpdateRect(RectangleF bounds);
		void InvalidateRect(RectangleF bounds);
		void SetCursor(CursorInfo cursorInfo);
		void DesignerLoaded();
		void OnModelChanged(BaseGaugeModel oldModel, BaseGaugeModel newModel);
		event EventHandler ModelChanged;
		void AddPrimitive(IElement<IRenderableElement> primitive);
		void RemovePrimitive(IElement<IRenderableElement> primitive);
		CustomizationFrameBase[] OnCreateCustomizeFrames(IGauge gauge, CustomizationFrameBase[] frames);
		void ComponentChanging(IComponent component, string property);
		void ComponentChanged(IComponent component, string property, object oldValue, object newValue);
		bool AutoLayout { get; set;}
		bool Enabled { get; set;}
#if !DXPORTABLE
		CustomizeManager CustomizeManager { get;}
#endif
		bool EnableCustomizationMode { get; set;}
		bool DesignMode { get;}
		bool ForceClearOnRestore { get; set;}
		ColorScheme ColorScheme { get; }
		Image GetImage(int width, int height);
		BasePrimitiveHitInfo CalcHitInfo(Point p);
		void AddToParentCollection(ISerizalizeableElement element, ISerizalizeableElement parent);
		ISerizalizeableElement CreateSerializableInstance(XtraPropertyInfo info, XtraPropertyInfo infoType);
		void SaveLayoutToXml(string path);
		void SaveLayoutToStream(Stream stream);
		void RestoreLayoutFromStream(Stream stream);
		void RestoreLayoutFromXml(string path);
		void UpdateGaugesZOrder();
		void InitializeDefault(object parameter);
	}
	public interface IGaugeContainerEx : IGaugeContainer {
		Image GetImage(int width, int height, Color backColor);
		Metafile GetMetafile(Stream stream, int width, int height);
		DevExpress.XtraPrinting.IPrintable Printable { get; }
	}
#if !DXPORTABLE
	[Editor("DevExpress.XtraGauges.Design.GaugeCollectionEditor, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(UITypeEditor))]
#endif
	public class GaugeCollection :
		BaseChangeableList<IGauge> {
		public void ShallowClear() {
			List.Clear();
		}
	}
	public interface IGaugeDesigner {
		bool IsUndoInProgress { get;}
	}
}
