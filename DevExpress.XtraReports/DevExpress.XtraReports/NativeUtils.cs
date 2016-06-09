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
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Export.Rtf;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Localization;
using System.Security.Permissions;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.OleDb;
namespace DevExpress.XtraReports.Native {
	public static class PropInfoAccessor {
		public static PropertyInfo GetProperty(object obj, string propName) {
			return GetProperty(obj, propName, true);
		}
		public static PropertyInfo GetProperty(object obj, string propName, bool throwOnError) {
			return PropInfoAccessorBase.GetProperty(obj, propName, throwOnError);
		}
		public static object GetPropertyValue(object obj, string propName) {
			PropertyInfo property = GetProperty(obj, propName);
			return property.GetValue(obj, null);
		}
		public static void SetPropertyValue(object obj, string propName, object value) {
			PropertyInfo property = GetProperty(obj, propName);
			property.SetValue(obj, value, null);
		}		
		public static void ChangePropertyValue(object obj, string propName, object value) {
			PropertyInfo property = GetProperty(obj, propName);
			object oldValue = property.GetValue(obj, null);
			if(oldValue == value) return;
			property.SetValue(obj, value, null);
		}
		public static bool IsPropertyExists(XRControl control, string propName) {
			return control != null ? TypeDescriptor.GetProperties(control)[propName] != null : false;
		}
		public static void SetPropertyFromResource(System.Resources.ResourceManager rm, string resourceName, object targetObj, string targetPropName) {
			if(rm == null || targetObj == null)
				return;
			try {
				object val = rm.GetObject(resourceName);
				if(val != null)
					SetPropertyValue(targetObj, targetPropName, val);
			} catch {}
		}
		public static void SetPropertyFromResource(System.Resources.ResourceSet resourceSet, string resourceName, object targetObj, string targetPropName) {
			if(resourceSet == null || targetObj == null)
				return;
			try {
				object val = resourceSet.GetObject(resourceName);
				if(val != null)
					SetPropertyValue(targetObj, targetPropName, val);
			} catch {}
		}
		public static void SetDesignProperty(object obj, IDictionary properties, string propName) {
			PropertyDescriptor propDesc = CreateProperty(obj, propName, new Attribute[] { CategoryAttribute.Design } );
			if(propDesc != null)
				properties[propName] = propDesc;
		}
		public static PropertyDescriptor CreateProperty(object obj, string propName, Attribute[] attributes) {
			PropertyInfo property = GetProperty(obj, propName);
			return (property != null) ? TypeDescriptor.CreateProperty(obj.GetType(), propName, property.PropertyType, attributes) : null; 
		}
		public static PropertyDescriptor CreateProperty(object obj, string propName, Type propType, Attribute[] attributes) {
			PropertyInfo property = PropInfoAccessorBase.GetProperty(obj, propName, propType);
			return (property != null) ? TypeDescriptor.CreateProperty(obj.GetType(), propName, property.PropertyType, attributes) : null;
		}
	}
	public class Divider {	
		#region inner classes
		class FloorDivider : Divider {
			protected override float GetDivisibleSingle(float val, float divis) {
				return (float)(Math.Floor((double)val / divis) * divis);
			}
		}
		class CeilingDivider : Divider {
			protected override float GetDivisibleSingle(float val, float divis) {
				return (float)(Math.Ceiling((double)val / divis) * divis);
			}
		}
		#endregion
		static Divider defaultInstance = new Divider();
		static Divider floorInstance = new FloorDivider();
		static Divider ceilingInstance = new CeilingDivider();
		public static Divider DefaultInstance {
			get { return defaultInstance; }
		}
		public static Divider FloorInstance {
			get { return floorInstance; }
		}
		public static Divider CeilingInstance {
			get { return ceilingInstance; }
		}
		public static float GetDivisibleValue(float val, float divis) {
			return defaultInstance.GetDivisibleSingle(val, divis);
		}
		public static Size GetDivisibleValue(Size val, Size divis) {
			return defaultInstance.GetDivisibleSize(val, divis);
		}
		public static Point GetDivisibleValue(Point val, Size divis) {
			return defaultInstance.GetDivisiblePoint(val, divis);
		}
		public static SizeF GetDivisibleValue(SizeF val, SizeF divis) {
			return defaultInstance.GetDivisibleSizeF(val, divis);
		}
		public static PointF GetDivisibleValue(PointF val, SizeF divis) {
			return defaultInstance.GetDivisiblePointF(val, divis);
		}
		public Size GetDivisibleSize(Size val, Size divis) {
			int x = GetDivisibleInt(val.Width, divis.Width);
			int y = GetDivisibleInt(val.Height, divis.Height);
			return new Size(x, y);
		}
		public Point GetDivisiblePoint(Point val, Size divis) {
			int x = GetDivisibleInt(val.X, divis.Width);
			int y = GetDivisibleInt(val.Y, divis.Height);
			return new Point(x, y);
		}
		public SizeF GetDivisibleSizeF(SizeF val, SizeF divis) {
			float x = GetDivisibleSingle(val.Width, divis.Width);
			float y = GetDivisibleSingle(val.Height, divis.Height);
			return new SizeF(x, y);
		}
		public PointF GetDivisiblePointF(PointF val, SizeF divis) {
			float x = GetDivisibleSingle(val.X, divis.Width);
			float y = GetDivisibleSingle(val.Y, divis.Height);
			return new PointF(x, y);
		}
		int GetDivisibleInt(float val, float divis) {
			return Convert.ToInt32(GetDivisibleSingle(val, divis));
		}
		protected virtual float GetDivisibleSingle(float val, float divis) {
			return (float)(Math.Round((double)val / divis) * divis);
		}
	}
	public static class NativeMethods {
		public static float GetValidValue(float min, float max, float val) {
			return Math.Min( Math.Max(min,val), max );
		}
		public static SizeF GetMaxSize(SizeF size, SizeF minSize) {
			return new SizeF(Math.Max(size.Width, minSize.Width), Math.Max(size.Height, minSize.Height));
		}
		public static PointF OffsetPointF(PointF pt, float x, float y) {
			return new PointF(pt.X + x, pt.Y + y);
		}
		public static Point OffsetPoint(Point pt, int x, int y) {
			return new Point(pt.X + x, pt.Y + y);
}
		public static void GetLinePoints(ref PointF pt1, ref PointF pt2, RectangleF rect, LineDirection lineDirection, float lineWidth) {
			float halfWidth = lineWidth / 2;
			if(lineDirection == LineDirection.Horizontal) {
				float vertCenter = (rect.Top + rect.Bottom) / 2;
				pt1 = new PointF(rect.Left, vertCenter);
				pt2 = new PointF(rect.Right, vertCenter);
			} else if(lineDirection == LineDirection.Vertical) {
				float horCenter = (rect.Left + rect.Right) / 2;
				pt1 = new PointF(horCenter, rect.Top);
				pt2 = new PointF(horCenter, rect.Bottom);
			} else if(lineDirection == LineDirection.Slant) {
				pt1 = new PointF(rect.Left, rect.Bottom);
				pt2 = new PointF(rect.Right, rect.Top);
				if(rect.Width > rect.Height) {
					pt1.Y = pt1.Y - halfWidth;
					pt2.Y = pt2.Y + halfWidth;
				} else {
					pt1.X = pt1.X + halfWidth;
					pt2.X = pt2.X - halfWidth;
				}
			} else if(lineDirection == LineDirection.BackSlant) {
				pt1 = new PointF(rect.Left, rect.Top);
				pt2 = new PointF(rect.Right, rect.Bottom);
				if(rect.Width > rect.Height) {
					pt1.Y = pt1.Y + halfWidth;
					pt2.Y = pt2.Y - halfWidth;
				} else {
					pt1.X = pt1.X + halfWidth;
					pt2.X = pt2.X - halfWidth;
				}
			}
		}
		public static Type[] GetObjectTypes(IList objects) {
			Type[] objTypes = (Type[])Array.CreateInstance(typeof(Type), objects.Count);
			for(int i = 0; i < objects.Count; i++)  
				objTypes[i] = objects[i].GetType();
			return objTypes;
		}
		public static bool IsAssignableTypes(Type[] baseTypes, Type type) {
			for(int i = 0; i < baseTypes.Length; i++)
				if( baseTypes[i].IsAssignableFrom(type) ) return true;
			return false;
		}
		public static ArrayList MergeLists(params IList[] lists) {
			ArrayList baseList = new ArrayList();
			foreach(IList list in lists) baseList.AddRange(list);
			return baseList;
		}
		public static string HexEscape(string input) {
			return Regex.Replace(input, "[., =]", new MatchEvaluator(EscapeEvaluator));
		}
		public static string HexUnescape(string input) {
			return input != null ? Regex.Replace(input, @"%\w\w", new MatchEvaluator(UnescapeEvaluator)) : null;
		}
		private static string EscapeEvaluator(Match m) {
			return Uri.HexEscape( m.Value[0] );
		}
		private static string UnescapeEvaluator(Match m) {
			int index = 0;
			return Uri.HexUnescape(m.Value, ref index).ToString();
		}
		public static void CloseProcess(Process process) {
			if(process != null) {
				try {
					if(!process.CloseMainWindow())
						process.Kill();
				} catch { }
			}
		}
		public static string MakeFieldDisplayName(string fieldName) {
			fieldName = fieldName.Replace('_', ' ');
			fieldName = fieldName.Replace('.', ' ');
			char prevChar = ' ';
			int count = fieldName.Length;
			System.Text.StringBuilder sb = new System.Text.StringBuilder(count);
			for (int i = 0; i < count; i++) {
				char ch = fieldName[i];
				if (Char.IsLower(prevChar) && Char.IsUpper(ch))
					sb.Append(' ');
				sb.Append(ch);
				prevChar = ch;
			}
			return sb.ToString();
		}
		public static void ClearUnvalidatedControl(ContainerControl form) {
			if(form == null) return;
			try {
				System.Reflection.FieldInfo[] fields = typeof(ContainerControl).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				if(fields == null) return;
				foreach(System.Reflection.FieldInfo fi in fields) {
					if(fi.Name == "unvalidatedControl" || fi.Name == "focusedControl") {
						Control control = fi.GetValue(form) as Control;
						if(control != null)
							fi.SetValue(form, null);
					}
				}
			}
			catch {
			}
		}
		public static object GetCloneValue(object val) {
			return val is ICloneable ? ((ICloneable)val).Clone() : val;
		}
	}
	public class WeightingFactorComparer : System.Collections.Generic.IComparer<XRControl> {
		public static int MakeComparison(XRControl x, XRControl y) {
			return Comparer.Default.Compare(x.GetWeightingFactor(), y.GetWeightingFactor());
		}
		public int Compare(XRControl x, XRControl y) {
			return MakeComparison(x, y);		
		}
	}
	public class NestedWeightingFactorComparer : IComparer {
		public static NestedWeightingFactorComparer Default = new NestedWeightingFactorComparer();
		public int Compare(object x, object y) {
			XRControl firstControl = x as XRControl;
			XRControl secondControl = y as XRControl;
			if(firstControl == null || secondControl == null)
				throw new InvalidOperationException();
			return CompareWeights(firstControl, secondControl);
		}
		static int CompareWeights(XRControl firstControl, XRControl secondControl) {
			EqualizeLevels(ref firstControl, ref secondControl);
			return WeightingFactorComparer.MakeComparison(firstControl, secondControl);
		}
		internal static void EqualizeLevels(ref XRControl first, ref XRControl second) {
			if(first == null || second == null)
				throw new InvalidOperationException();
			while(true) {
				int firstLevel = first.NestedLevel;
				int secondLevel = second.NestedLevel;
				if(firstLevel == secondLevel && first.Parent == second.Parent)
					break;
				if(firstLevel >= secondLevel)
					first = first.Parent;
				if(secondLevel >= firstLevel)
					second = second.Parent;
			}
		}
	}
	public class ControlTopComparer : IComparer 
	{
		public static int CompareCore(XRControl x, XRControl y) {
			return Comparer.Default.Compare(x.TopF, y.TopF);
		}
		public int Compare(object x, object y) {
			return CompareCore((XRControl)x, (XRControl)y);
		}
	}
	public class ControlZOrderComparer : IComparer {
		public int Compare(object x, object y) {
			return ((XRControl)y).Index - ((XRControl)x).Index;
		}
	}
	[ToolboxItem(false)]
	public class FormEx : Form 
	{
		protected override CreateParams CreateParams {
			get {
				CreateParams createParams = base.CreateParams;
				createParams.ExStyle |=  Win32.WS_EX_TOOLWINDOW;
				return createParams;
			}
		}
		protected override bool ShowWithoutActivation { get { return true; } }
		public FormEx() {
			ShowInTaskbar = false;
		}
	}
}
