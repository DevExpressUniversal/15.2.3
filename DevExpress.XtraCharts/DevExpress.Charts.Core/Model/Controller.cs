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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
namespace DevExpress.Charts.Model {
	public interface IModelElementContainer {
		void Clear();
		void Register(object viewObject, ModelElement element);
		ModelElement FindModelElement(object viewObject);
		object FindViewObject(ModelElement element);
	}
	public interface IModelListener {
		void OnModelUpdated(UpdateInfo update);
	}
	public interface IChartRenderContext {
		ModelRect Bounds { get; }
	}
	[StructLayout(LayoutKind.Sequential), ComVisible(false)]
	public struct ModelRect {
		public static readonly ModelRect Empty = new ModelRect(0, 0, 0, 0);
		public static bool operator ==(ModelRect rect1, ModelRect rect2) {
			return rect1.Left == rect2.left && rect1.top == rect2.top && rect1.width == rect2.width && rect1.height == rect2.height; 
		}
		public static bool operator !=(ModelRect rect1, ModelRect rect2) {
			return !(rect1 == rect2);
		}
		public static bool Equals(ModelRect rect1, ModelRect rect2) {
			return rect1.left.Equals(rect2.left) && rect1.top.Equals(rect2.top) && rect1.width.Equals(rect2.width) && rect1.height.Equals(rect2.height);
		}
		double left;
		double top;
		double width;
		double height;
		public double Left { get { return left; } set { left = value; } }
		public double Top { get { return top; } set { top = value; } }
		public double Width { get { return width; } set { width = value; } }
		public double Height { get { return height; } set { height = value; } }
		public ModelRect(double left, double top, double width, double height) {
			this.left = left;
			this.top = top;
			this.height = height;
			this.width = width;
		}
		public override bool Equals(object obj) {
			if (obj is ModelRect)
				return ModelRect.Equals(this, (ModelRect)obj);
			return false;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	public abstract class ModelControllerFactoryBase {
		public abstract Controller CreateController();
		public abstract IChartRenderContext CreateRenderContext(ModelRect bounds, params object[] renderParams);
	}
	public abstract class Controller : IModelListener {
		Chart chartModel;
		IModelElementContainer elementContainer;
		protected internal IModelElementContainer ElementContainer { get { return elementContainer; } }
		public Chart ChartModel {
			get { return chartModel; }
			set {
				if(Object.Equals(ChartModel, value))
					return;
				UpdateModelListener(chartModel, null);
				this.chartModel = value;
				UpdateModelListener(chartModel, this);
				ClearElementContainer();
				OnModelChanged();
			}
		}
		protected Controller() {
			elementContainer = new ModelElementContainer();
		}
		protected abstract void OnModelChanged();
		void ClearElementContainer() {
			ElementContainer.Clear();
		}
		public event ModelUpdatedEventHandler ModelUpdated;
		void IModelListener.OnModelUpdated(UpdateInfo update) {
			OnModelUpdated(update);
		}
		protected virtual void OnModelUpdated(UpdateInfo update) {
			RaiseModelUpdated(update);
		}
		protected void RaiseModelUpdated(UpdateInfo update) {
			if(ModelUpdated != null) ModelUpdated(this, new ModelUpdatedEventArgs(update));
		}
		void UpdateModelListener(Chart model, IModelListener listener) {
			if(model != null) model.SetListener(listener);
		}
		public abstract HitInfo CalcHitInfo(double x, double y);
		public abstract void Select(ModelElement element);
		public abstract void Unselect(ModelElement element);
		public abstract void ClearSelection();
		public abstract void RenderChart(IChartRenderContext renderContext);
		protected HitInfo CreateHitInfo(object hitObject) { 
			if (hitObject == null)
				return null;
			ModelElement el = ElementContainer.FindModelElement(hitObject);
			return (el != null) ? new HitInfo(el) : HitInfo.Empty;
		}
	}
	public class HitInfo {
		public static readonly HitInfo Empty = new HitInfo(null);
		ModelElement element;
		public HitInfo(ModelElement element) {
			this.element = element;
		}
		public ModelElement Element { get { return element; } }
		public override bool Equals(object obj) {
			HitInfo hitInfo = obj as HitInfo;
			ModelElement el = hitInfo != null ? hitInfo.Element : null;
			return ModelElement.Equals(Element, el);
		}
		public override int GetHashCode() {
			return Element != null ? Element.GetHashCode() : base.GetHashCode();
		}
	}
	public class ModelElementContainer : IModelElementContainer {
		readonly Dictionary<object, Model.ModelElement> modelElements;
		readonly Dictionary<Model.ModelElement, object> viewObjects;
		public ModelElementContainer() {
			modelElements = new Dictionary<object, Model.ModelElement>();
			viewObjects = new Dictionary<ModelElement, object>();
		}
		public void Clear() {
			modelElements.Clear();
			viewObjects.Clear();
		}
		public void Register(object viewObject, ModelElement element) {
			if (!modelElements.ContainsKey(viewObject))
				modelElements[viewObject] = element;
			if(!viewObjects.ContainsKey(element))
				viewObjects[element] = viewObject;
		}
		public ModelElement FindModelElement(object viewObject) {
			ModelElement result = null;
			if (modelElements.TryGetValue(viewObject, out result))
				return result;
			return null;
		}
		public object FindViewObject(ModelElement element) {
			object result = null;
			if(viewObjects.TryGetValue(element, out result))
				return result;
			return null;
		}
	}
	public delegate void ModelUpdatedEventHandler(object sender, ModelUpdatedEventArgs e);
	public class ModelUpdatedEventArgs {
		UpdateInfo update;
		public UpdateInfo Update { get { return update; } }
		public ModelUpdatedEventArgs(UpdateInfo update) {
			this.update = update;
		}
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct ColorARGB : IComparable<ColorARGB> {
		public static ColorARGB Empty = new ColorARGB();
		public static ColorARGB Parse(string argbColor) {
			if(argbColor.Length != 8)
				return ColorARGB.Empty;
			byte a = byte.Parse(argbColor.Substring(0, 2), NumberStyles.HexNumber);
			byte r = byte.Parse(argbColor.Substring(2, 2), NumberStyles.HexNumber);
			byte g = byte.Parse(argbColor.Substring(4, 2), NumberStyles.HexNumber);
			byte b = byte.Parse(argbColor.Substring(6, 2), NumberStyles.HexNumber);
			return new ColorARGB(a, b, g, r);
		}
		public static bool operator ==(ColorARGB color1, ColorARGB color2) {
			return color1.argb == color2.argb;
		}
		public static bool operator !=(ColorARGB color1, ColorARGB color2) {
			return !(color1 == color2);
		}
		public static bool Equals(ColorARGB color1, ColorARGB color2) {
			if(color1 != null && color2 != null)
				return color1 == color2;
			return false;
		}
		readonly UInt32 argb;
		public byte A { get { return (byte)(argb >> 0x18); } }
		public byte R { get { return (byte)(argb >> 0x10); } }
		public byte G { get { return (byte)(argb >> 8); } }
		public byte B { get { return (byte)(argb); } }
		public bool IsEmpty { get { return this == ColorARGB.Empty; } }
		public ColorARGB(byte alpha, byte red, byte green, byte blue) {
			this.argb = (UInt32)(alpha << 0x18 | red << 0x10 | green << 8 | blue);
		}
		public override bool Equals(object obj) {
			ColorARGB? nullableValue = obj as ColorARGB?;
			return nullableValue.HasValue && ColorARGB.Equals(this, nullableValue.Value);
		}
		public override int GetHashCode() {
			return this.argb.GetHashCode();
		}
		public int CompareTo(ColorARGB other) {
			return this.argb.CompareTo(other.argb);
		}
	}
}
