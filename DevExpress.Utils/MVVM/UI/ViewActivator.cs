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

namespace DevExpress.Utils.MVVM.UI {
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Reflection;
	using System.Windows.Forms;
	sealed class ViewActivator : IViewActivator, IViewActivatorCache {
		#region static
		internal static readonly IViewActivator Instance = new ViewActivator();
		static Assembly entryAssemblyCore;
		static Assembly EntryAssembly {
			get {
				if(entryAssemblyCore == null)
					entryAssemblyCore = Assembly.GetEntryAssembly();
				return entryAssemblyCore;
			}
		}
		static List<Assembly> contextAssemblies = new List<Assembly>();
		internal static void RegisterContextAssembly(Assembly assembly) {
			if(assembly == null || assembly == EntryAssembly || assembly == typeof(ViewActivator).Assembly
				|| contextAssemblies.Contains(assembly)) return;
			contextAssemblies.Add(assembly);
			((ViewActivator)Instance).Reset();
		}
		internal void Reset() {
			Ref.Dispose(ref typesEnumerator);
			types.Clear();
		}
		static string GetTypeName(Type type) {
			var attributes = type.GetCustomAttributes(typeof(UI.ViewTypeAttribute), true);
			if(attributes.Length > 0)
				return ((ViewTypeAttribute)attributes[0]).Name;
			return type.Name;
		}
		static Type[] GetTypes(Assembly asm) {
			try { return asm.GetTypes(); }
			catch(ReflectionTypeLoadException e) { return e.Types; }
		}
		#endregion static
		Assembly[] assembliesCore;
		TypesActivator typesActivator = new TypesActivator();
		ViewActivator(params Assembly[] assemblies) {
			this.assembliesCore = assemblies;
		}
		void IViewActivatorCache.Reset() {
			typesActivator.ResetCache();
		}
		void IViewActivatorCache.Reset(Type type) {
			typesActivator.ResetCache(type);
		}
		object IViewActivator.CreateView(string viewType, params object[] parameters) {
			Type type = ResolveViewType(viewType);
			if(type == null || !CtorHelper.HasConstructor(type, parameters))
				return new ViewPlaceholder() { Text = viewType };
			if(parameters == null || parameters.Length == 0)
				return typesActivator.Create(type);
			else
				return Activator.CreateInstance(type, parameters);
		}
		IEnumerator<Type> typesEnumerator;
		IDictionary<string, Type> types = new Dictionary<string, Type>();
		Type ResolveViewType(string viewType) {
			if(string.IsNullOrEmpty(viewType))
				return null;
			Type type;
			if(types.TryGetValue(viewType, out type))
				return type;
			if(typesEnumerator == null)
				typesEnumerator = GetTypes();
			while(typesEnumerator.MoveNext()) {
				type = typesEnumerator.Current;
				string typeName = GetTypeName(type);
				if(!types.ContainsKey(typeName))
					types[typeName] = type;
				if(typeName == viewType)
					return type;
			}
			return null;
		}
		IEnumerable<Assembly> GetAssemblies() {
			if(assembliesCore == null || assembliesCore.Length == 0)
				return EnumerateAssemblies();
			return assembliesCore;
		}
		IEnumerable<Assembly> EnumerateAssemblies() {
			if(EntryAssembly != null)
				yield return EntryAssembly;
			foreach(Assembly contextAsm in contextAssemblies)
				yield return contextAsm;
		}
		IEnumerator<Type> GetTypes() {
			var assemblies = GetAssemblies();
			foreach(Assembly asm in assemblies) {
				Type[] types = GetTypes(asm);
				foreach(Type type in types)
					yield return type;
			}
		}
		#region ViewPlaceholder
		[System.ComponentModel.ToolboxItem(false)]
		internal class ViewPlaceholder : Panel {
			#region Appearance
			static int currentColor;
			static Color[] forecolors = new Color[] { 
				Color.Red, Color.Blue, Color.Green
			};
			SolidBrush bg;
			SolidBrush fg;
			StringFormat sf;
			static Font font = GetSegoeUIFont(11f);
			static Font GetSegoeUIFont(float sizeGrow = 0) {
				float defaultSize = DevExpress.Utils.AppearanceObject.DefaultFont.Size;
				return GetFont("Segoe UI", defaultSize + sizeGrow);
			}
			static Font GetFont(string familyName, float size) {
				try {
					var family = FindFontFamily(familyName);
					return new Font(family ?? FontFamily.GenericSansSerif, size);
				}
				catch(ArgumentException) { return DevExpress.Utils.AppearanceObject.DefaultFont; }
			}
			static FontFamily FindFontFamily(string familyName) {
				return Array.Find(FontFamily.Families, (f) => f.Name == familyName);
			}
			#endregion Appearance
			public ViewPlaceholder() {
				sf = new StringFormat()
				{
					Alignment = StringAlignment.Center,
					LineAlignment = StringAlignment.Center,
					Trimming = StringTrimming.EllipsisCharacter,
					FormatFlags = StringFormatFlags.NoWrap
				};
				DoubleBuffered = true;
				ForeColor = forecolors[(currentColor++) % forecolors.Length];
				SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
				SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
				Size = new Size(300, 300);
			}
			protected override void Dispose(bool disposing) {
				if(disposing) {
					Ref.Dispose(ref bg);
					Ref.Dispose(ref fg);
					Ref.Dispose(ref sf);
				}
				base.Dispose(disposing);
			}
			protected override void OnPaint(PaintEventArgs e) {
				if(bg == null)
					bg = new SolidBrush(Color.FromArgb(100, ForeColor));
				if(fg == null)
					fg = new SolidBrush(Color.FromArgb(200, ForeColor));
				e.Graphics.FillRectangle(bg, ClientRectangle);
				e.Graphics.DrawString(Text, font, fg, ClientRectangle, sf);
			}
		}
		#endregion ViewPlaceholder
	}
}
