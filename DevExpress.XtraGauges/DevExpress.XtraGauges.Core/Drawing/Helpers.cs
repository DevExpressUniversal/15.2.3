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
using System.Collections;
using DevExpress.Utils;
using DevExpress.XtraGauges.Core.XAML;
using DevExpress.XtraGauges.Core.Base;
using System.IO;
using System.Reflection;
using DevExpress.XtraGauges.Core.SHP;
using DevExpress.XtraGauges.Core.Model;
using System.ComponentModel;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraGauges.Core.Drawing {
	public abstract class BaseShapeLoader : BaseObject, IShapeLoader {
		public abstract ComplexShape LoadFromStream(Stream stream);
		public virtual ComplexShape LoadFromFile(string path) {
			ComplexShape result = null;
			using(Stream stream = new FileStream(path, FileMode.Open)) {
				result = LoadFromStream(stream);
			}
			return result;
		}
		public virtual ComplexShape LoadFromResources(string path) {
			return LoadFromResources(path, GetType().GetAssembly());
		}
		public virtual ComplexShape LoadFromResources(string path, Assembly asm) {
			ComplexShape result = null;
			using(Stream stream = asm.GetManifestResourceStream(path)) {
				result = LoadFromStream(stream);
			}
			return result;
		}
	}
	public sealed class LineRectIntersect {
		public static bool Calc2(float LineX1, float LineY1, float LineX2, float LineY2, float RectX, float RectY, float RectWidth, float RectHeight,
			   out float intersectX1, out float intersectY1, out float intersectX2, out float intersectY2) {
			Calc(LineX1, LineY1, LineX2, LineY2, RectX, RectY, RectWidth, RectHeight, out intersectX1, out intersectY1);
			return Calc(LineX2, LineY2, LineX1, LineY1, RectX, RectY, RectWidth, RectHeight, out intersectX2, out intersectY2);
		}
		public static bool Calc(float LineX1, float LineY1, float LineX2, float LineY2, float RectX, float RectY, float RectWidth, float RectHeight,
			   out float intersectX, out float intersectY) {
			float b;
			float m;
			float x;
			float y;
			intersectX = -9999;
			intersectY = -9999;
			float step = 0.1f;
			bool wasIntersect;
			wasIntersect = false;
			if(LineY1 == LineY2) {
				if(LineX1 < LineX2) {
					for(x = LineX1; x <= LineX2; x += step) {
						if(x >= RectX & x <= RectX + RectWidth & LineY1 >= RectY & LineY1 <= RectY + RectHeight) {
							intersectX = x;
							intersectY = LineY1;
							wasIntersect = true;
							break;
						}
					}
				} else {
					for(x = LineX1; x >= LineX2; x -= step) {
						if(x >= RectX & x <= RectX + RectWidth & LineY1 >= RectY & LineY1 <= RectY + RectHeight) {
							intersectX = x;
							intersectY = LineY1;
							wasIntersect = true;
							break;
						}
					}
				}
			} else if(LineX1 == LineX2) {
				if(LineY1 < LineY2) {
					for(y = LineY1; y <= LineY2; y += step) {
						if(LineX1 >= RectX & LineX1 <= RectX + RectWidth & y >= RectY & y <= RectY + RectHeight) {
							intersectX = LineX1;
							intersectY = y;
							wasIntersect = true;
							break;
						}
					}
				} else {
					for(y = LineY1; y >= LineY2; y -= step) {
						if(LineX1 >= RectX & LineX1 <= RectX + RectWidth & y >= RectY & y <= RectY + RectHeight) {
							intersectX = LineX1;
							intersectY = y;
							wasIntersect = true;
							break;
						}
					}
				}
			} else {
				m = (LineY1 - LineY2) / (LineX1 - LineX2);
				b = LineY1 - (m * LineX1);
				if(LineX1 < LineX2) {
					for(x = LineX1; x <= LineX2; x += step) {
						y = (m * x) + b;
						if(x >= RectX & x <= RectX + RectWidth & y >= RectY & y <= RectY + RectHeight) {
							intersectX = x;
							intersectY = y;
							wasIntersect = true;
							break;
						}
					}
				} else {
					for(x = LineX1; x >= LineX2; x -= step) {
						y = (m * x) + b;
						if(x >= RectX & x <= RectX + RectWidth & y >= RectY & y <= RectY + RectHeight) {
							intersectX = x;
							intersectY = y;
							wasIntersect = true;
							break;
						}
					}
				}
				if(LineY1 < LineY2) {
					for(y = LineY1; y <= LineY2; y += step) {
						x = (y - b) / m;
						if(x >= RectX & x <= RectX + RectWidth & y >= RectY & y <= RectY + RectHeight) {
							intersectX = x;
							intersectY = y;
							wasIntersect = true;
							break;
						}
					}
				} else {
					for(y = LineY1; y >= LineY2; y -= step) {
						x = (y - b) / m;
						if(x >= RectX & x <= RectX + RectWidth & y >= RectY & y <= RectY + RectHeight) {
							intersectX = x;
							intersectY = y;
							wasIntersect = true;
							break;
						}
					}
				}
			}
			return wasIntersect;
		}
	}
	public sealed class FilterPropertiesHelper {
		public static PropertyDescriptorCollection FilterProperties(PropertyDescriptorCollection properties, string[] names) {
			ArrayList list = new ArrayList(properties.Count);
			foreach(PropertyDescriptor desc in properties) {
				bool found = false;
				foreach(string name in names) {
					if(desc.Name == name) {
						found = true;
						break;
					}
				}
				if(found) list.Add(desc);
			}
			return new PropertyDescriptorCollection((PropertyDescriptor[])list.ToArray(typeof(PropertyDescriptor)));
		}
		public static void PrefilterProperties(IDictionary properties, string[] names) {
			ArrayList propertiesToRemove = new ArrayList();
			foreach(DictionaryEntry entry in properties) {
				bool found = false;
				foreach(string name in names) {
					if((string)entry.Key == name) {
						found = true;
						break;
					}
				}
				if(!found) propertiesToRemove.Add(entry.Key);
			}
			foreach(object key in propertiesToRemove) properties.Remove(key);
		}
	}
}
namespace DevExpress.XtraGauges.Core.Base {
	public static class UniqueNameHelper {
		static List<string> MjNames = new List<string>();
		static List<string> MiNames = new List<string>();
		static List<string> LblNames = new List<string>();
		static List<string> ShapeNames = new List<string>();
		public static string GetMajorTickmarkUniqueName(MajorTickmarkCollection collection) {
			return GetUniqueName("MajorTickmark", collection, MjNames, collection.Count);
		}
		public static string GetMinorTickmarkUniqueName(MinorTickmarkCollection collection) {
			return GetUniqueName("MinorTickmark", collection, MiNames, collection.Count);
		}
		public static string GetShapeUniqueName(BaseShapeCollection collection) {
			return GetUniqueName("Shape", collection, ShapeNames, collection.Count);
		}
		public static string GetUniqueName(string prefix, List<string> names, int initialValue) {
			int count = initialValue;
			while(true) {
				string name = prefix + count++.ToString();
				if(!names.Contains(name)) return name;
			}
		}
		public static string GetUniqueName(string prefix, string[] names, int initialValue) {
			ArrayList list = new ArrayList(names);
			int count = initialValue;
			while(true) {
				string name = prefix + count++.ToString();
				if(!list.Contains(name)) return name;
			}
		}
		public static string GetUniqueName<T>(string prefix, BaseReadOnlyDictionary<T> names, List<string> cache, int initialValue) where T : class, ISupportAcceptOrder, IDisposable {
			int count = initialValue;
			while(true) {
				if(cache.Count <= count) {
					lock(cache) {
						if(cache.Count <= count) {
							cache.Add(prefix + count.ToString());
						}
					}
				}
				string name = cache[count++];
				if(!names.Contains(name)) return name;
			}
		}
		public static List<string> GetSiteNames(ISite site) {
			List<string> names = new List<string>();
			if(site != null) {
				foreach(IComponent c in site.Container.Components) names.Add(c.Site.Name);
			}
			return names;
		}
   }
}
