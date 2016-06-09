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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.BandedGrid.Drawing;
using DevExpress.XtraGrid.Views.Grid.Handler;
using DevExpress.XtraGrid.Views.Base.Handler;
using DevExpress.XtraGrid.Views.BandedGrid.Handler;
using DevExpress.XtraGrid.Views.Card.Handler;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Views.Card.Drawing;
using DevExpress.XtraGrid.Design;
using DevExpress.XtraGrid.Views.Layout.Drawing;
using DevExpress.XtraGrid.Views;
using DevExpress.XtraGrid.Views.Layout.ViewInfo;
using DevExpress.XtraGrid.Views.Layout.Designer;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraLayout.Painting;
using DevExpress.XtraLayout;
namespace DevExpress.XtraGrid.Registrator {
	[ListBindable(false)]
	public class NamedCollection : CollectionBase {
		Hashtable names = new Hashtable();
		protected override void OnClearComplete() {
			base.OnClearComplete();
			Names.Clear();
		}
		protected override void OnClear() {
			for(int n = Count - 1; n >= 0; n--) RemoveAt(n);
			Names.Clear();
		}
		protected virtual string GetName(object val) { return ""; }
		protected override void OnInsertComplete(int position, object val) {
			base.OnInsertComplete(position, val);
			Names.Add(GetName(val), val);
		}
		protected override void OnSetComplete(int position, object oldValue, object newValue) {
			base.OnSetComplete(position, oldValue, newValue);
			Names.Remove(GetName(oldValue));
			Names.Add(GetName(newValue), newValue);
		}
		protected override void OnRemoveComplete(int position, object val) {
			base.OnRemoveComplete(position, val);
			Names.Remove(GetName(val));
		}
		protected virtual Hashtable Names { get { return names; } }
		public virtual bool Contains(string name) { return Names.Contains(name); }
		public virtual int IndexOf(object info) {
			return List.IndexOf(info);
		}
		public virtual void Remove(object info) {
			List.Remove(info);
		}
	}
	public class BorderInfo {
		string name;
		BorderPainter painter;
		public BorderInfo(string name, BorderPainter painter) {
			this.name = name;
			this.painter = painter;
		}
		public BorderPainter Painter { get { return painter; } }
		public string Name { get { return name; } }
	}
	public class BorderCollection : NamedCollection {
		public virtual BorderInfo this[int index] { get { return List[index] as BorderInfo; } }
		public virtual BorderInfo this[string name] { get { return Names[name] as BorderInfo; } }
		protected override string GetName(object val) { 
			BorderInfo reg = val as BorderInfo;
			if(reg == null) return "";
			return reg.Name; 
		}
		public virtual void Add(BorderInfo regInfo) {
			if(regInfo == null || Contains(regInfo.Name)) return;
			this.List.Add(regInfo);
		}
	}
	public class InfoCollection : NamedCollection {
		BorderCollection borders;
		public InfoCollection() {
			this.borders = new BorderCollection();
		}
		BaseInfoRegistrator CreateRegistrator(int index, FakeBaseInfoRegistrator fake) {
			BaseInfoRegistrator reg = fake.Create();
			List[index] = reg;
			return reg;
		}
		public virtual BaseInfoRegistrator this[int index] {
			get {
				BaseInfoRegistrator reg = (BaseInfoRegistrator)List[index];
				FakeBaseInfoRegistrator fake = reg as FakeBaseInfoRegistrator;
				if(fake != null)
					reg = CreateRegistrator(index, fake);
				return reg;
			}
		}
		public virtual BaseInfoRegistrator this[string name] {
			get {
				BaseInfoRegistrator reg = (BaseInfoRegistrator)Names[name];
				FakeBaseInfoRegistrator fake = reg as FakeBaseInfoRegistrator;
				if(fake != null)
					reg = CreateRegistrator(List.IndexOf(reg), fake);
				return reg;
			}
		}
		protected override string GetName(object val) { 
			BaseInfoRegistrator reg = val as BaseInfoRegistrator;
			if(reg == null) return "";
			return reg.ViewName; 
		}
		public virtual void Add(BaseInfoRegistrator regInfo) {
			if(regInfo == null || Contains(regInfo.ViewName)) return;
			this.List.Add(regInfo);
		}
		public virtual BorderCollection Borders { get { return borders; } }
		public virtual BaseInfoRegistrator[] GetByStyleCreator(string styleCreator) {
			ArrayList list = new ArrayList();
			for(int i = 0; i < List.Count; i++ ) {
				BaseInfoRegistrator reg = (BaseInfoRegistrator)List[i];
				if(reg.StyleOwnerName == styleCreator) {
					list.Add(this[i]);
				}
			}
			return list.ToArray(typeof(BaseInfoRegistrator)) as BaseInfoRegistrator[];
		}
		protected internal void UpdateTheme() {
			foreach(BaseInfoRegistrator info in this) info.UpdateTheme();
		}
		protected override void OnRemoveComplete(int index, object item) {
			base.OnRemoveComplete(index, item);
			BaseInfoRegistrator reg = item as BaseInfoRegistrator;
			if(reg != null) reg.Dispose();
		}
		protected override void OnClear() {
			for(int n = Count - 1; n >= 0; n--) RemoveAt(n);
		}
	}
	public class PaintStyleCollection : NamedCollection {
		BaseInfoRegistrator owner;
		public PaintStyleCollection(BaseInfoRegistrator owner) {
			this.owner = owner;
		}
		public BaseInfoRegistrator Owner { get { return owner; } }
		ViewPaintStyle CreateRegistrator(int index, FakeViewPaintStyle fake) {
			ViewPaintStyle reg = fake.Create();
			List[index] = reg;
			return reg;
		}
		public virtual ViewPaintStyle this[int index] {
			get {
				ViewPaintStyle reg = (ViewPaintStyle)List[index];
				FakeViewPaintStyle fake = reg as FakeViewPaintStyle;
				if(fake != null)
					reg = CreateRegistrator(index, fake);
				return reg;
			}
		}
		public virtual ViewPaintStyle this[string name] {
			get {
				ViewPaintStyle reg = (ViewPaintStyle)Names[name];
				FakeViewPaintStyle fake = reg as FakeViewPaintStyle;
				if(fake != null)
					reg = CreateRegistrator(List.IndexOf(reg), fake);
				return reg;
			}
		}
		protected override string GetName(object val) { 
			ViewPaintStyle vs = val as ViewPaintStyle;
			if(vs == null) return "";
			return vs.Name; 
		}
		protected override void OnRemoveComplete(int index, object item) {
			base.OnRemoveComplete(index, item);
			ViewPaintStyle ps = item as ViewPaintStyle;
			if(ps != null) ps.Dispose();
		}
		protected override void OnInsertComplete(int index, object item) {
			base.OnInsertComplete(index, item);
			ViewPaintStyle ps = item as ViewPaintStyle;
			if(ps != null) ps.SetOwner(Owner);
		}
		public virtual void Add(ViewPaintStyle ps) {
			if(ps == null || Contains(ps.Name)) return;
			this.List.Add(ps);
		}
		protected internal void UpdateTheme() {
			foreach(ViewPaintStyle ps in this) ps.UpdateTheme();
		}
	}
	public class GridEmbeddedLookAndFeel : EmbeddedLookAndFeel {
		BaseView view;
		public GridEmbeddedLookAndFeel(BaseView view) {
			this.view = view;
		}
		public override string SkinName { 
			get { return ((ISkinProvider)view).SkinName; }
			set { base.SkinName = value; }
		}
		public override Color GetMaskColor() {
			return ((ISkinProviderEx)view).GetMaskColor();
		}
		public override Color GetMaskColor2() {
			return ((ISkinProviderEx)view).GetMaskColor2();
		}
		internal void SetSkinStyleCore(UserLookAndFeel lookAndFeel) {
			string skinName = lookAndFeel.SkinName;
			this.styleChangedFired = false;
			SetSkinStyle(skinName);
			if(!styleChangedFired && lastSkinName != null && skinName != lastSkinName) OnStyleChanged();
			this.lastSkinName = SkinName;
		}
		string lastSkinName = null;
		bool styleChangedFired = false;
		protected override void OnStyleChanged() {
			this.lastSkinName = SkinName;
			this.styleChangedFired = true;
			base.OnStyleChanged();
		}
	}
	delegate ViewPaintStyle CreateViewPaintStyle();
	class FakeViewPaintStyle : ViewPaintStyle {
		CreateViewPaintStyle creator;
		string name;
		public ViewPaintStyle Create() {
			return creator();
		}
		public FakeViewPaintStyle(CreateViewPaintStyle creator, string name) {
			this.creator = creator;
			this.name = name;
		}
		public override string Name {
			get {
				return name;
			}
		}
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(BaseView view) {
			throw new InvalidOperationException();
		}
	}
	public abstract class ViewPaintStyle : IDisposable {
		public const string DefaultPaintStyleName = "Default";
		BaseInfoRegistrator owner;
		public virtual void UpdateElementsLookAndFeel(BaseView view) {
		}
		public virtual void Dispose() {
			this.owner = null;
		}
		protected internal void SetOwner(BaseInfoRegistrator owner) {
			this.owner = owner;
		}
		public virtual bool IsSkin { get { return false; } }
		public BaseInfoRegistrator Owner { get { return owner; } }
		public virtual bool CanUsePaintStyle { get { return true; } }
		public virtual string Name { get { return ""; } }
		public virtual BaseViewPainter CreatePainter(BaseView view) { return null; }
		public virtual BaseViewInfo CreateViewInfo(BaseView view) { return null; }
		public abstract AppearanceDefaultInfo[] GetAppearanceDefaultInfo(BaseView view);
		protected internal virtual void UpdateTheme() { }
		public virtual BorderPainter GetBorderPainter(BaseView view, BorderStyles border) { return null; }
	}
	delegate BaseInfoRegistrator CreateInfoRegistrator();
	class FakeBaseInfoRegistrator : BaseInfoRegistrator {
		CreateInfoRegistrator creator;
		string name;
		public BaseInfoRegistrator Create() {
			return creator();
		}
		public FakeBaseInfoRegistrator(CreateInfoRegistrator creator, string name) {
			this.creator = creator;
			this.name = name;
		}
		protected override void RegisterViewPaintStyles() {
		}
		public override string ViewName {
			get {
				return name;
			}
		}
	}
	public abstract class BaseInfoRegistrator : IDisposable {
		PaintStyleCollection paintStyles;
		BaseGridDesigner designer;		
		public BaseInfoRegistrator() {
			this.paintStyles = new PaintStyleCollection(this);
			this.designer = CreateDesigner();
			RegisterViewPaintStyles();
		}
		public virtual void Dispose() {
			if(Designer != null) Designer.Dispose();
			PaintStyles.Clear();
		}
		protected internal void UpdateTheme() {
			PaintStyles.UpdateTheme();
		}
		public BaseGridDesigner Designer { get { return designer; } }
		protected virtual BaseGridDesigner CreateDesigner() { return new BaseGridDesigner(); } 
		protected abstract void RegisterViewPaintStyles();
		public virtual ViewPaintStyle PaintStyleByView(BaseView view) {
			if(view != null) {
				ViewPaintStyle ps = PaintStyleByLookAndFeel(view.LookAndFeel, view.PaintStyleName);
				if(ps != null) return ps;
			}
			return PaintStyles[0];
		}
		public virtual ViewPaintStyle PaintStyleByLookAndFeel(UserLookAndFeel lookAndFeel, string name) {
			if(name == ViewPaintStyle.DefaultPaintStyleName) {
				switch(lookAndFeel.ActiveStyle) {
					case ActiveLookAndFeelStyle.WindowsXP : name = "WindowsXP"; break;
					case ActiveLookAndFeelStyle.Skin: name = "Skin"; break;
					case ActiveLookAndFeelStyle.Flat: name = "Flat"; break;
					case ActiveLookAndFeelStyle.Style3D : name = "Style3D"; break;
					case ActiveLookAndFeelStyle.Office2003 : name = "Office2003"; break;
					case ActiveLookAndFeelStyle.UltraFlat : name = "UltraFlat"; break;
				}
			}
			ViewPaintStyle res = PaintStyles[name];
			if(res != null && !res.CanUsePaintStyle) res = null;
			if(res == null) return PaintStyles[0];
			return res;
		}
		public virtual PaintStyleCollection PaintStyles { get { return paintStyles; } }
		public virtual bool IsInternalView { get { return true; } }
		public override string ToString() { return ViewName; } 
		public virtual string ViewName { get { return "BaseView"; } }
		public virtual string StyleOwnerName { get { return "BaseView"; } }
		public virtual BaseView CreateView(GridControl grid) { return null; } 
		public virtual BaseViewHandler CreateHandler(BaseView view) { return null; }
		public virtual BaseViewPainter CreatePainter(BaseView view) { return PaintStyleByView(view).CreatePainter(view); } 
		public virtual BaseViewInfo CreateViewInfo(BaseView view) { return PaintStyleByView(view).CreateViewInfo(view); } 
		public virtual AppearanceDefaultInfo[] GetDefaultPrintAppearance() { return new AppearanceDefaultInfo[0]; }
	}
	public class CardWindowsXPPaintStyle : CardPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = true;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.Flat;
		}
		public override bool CanUsePaintStyle { get { return DevExpress.Utils.WXPaint.WXPPainter.Default.ThemesEnabled; } }
		public override string Name { get { return "WindowsXP"; } }
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(BaseView view) { return CardOfficeAppearances; }
		public override CardElementsPainter CreateElementsPainter(BaseView view) { return new CardElementsPainterOffice2003(view);	}
	}
	public class CardMixedXPPaintStyle : CardPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.SetFlatStyle();
		}
		public override bool CanUsePaintStyle { get { return DevExpress.Utils.WXPaint.WXPPainter.Default.ThemesEnabled; } }
		public override string Name { get { return "MixedXP"; } }
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(BaseView view) { return CardOfficeAppearances; }
		public override CardElementsPainter CreateElementsPainter(BaseView view) { return new CardElementsPainterOffice(view);	}
	}
	public class CardSkinPaintStyle : CardPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.SetSkinStyleCore(view.LookAndFeel);
		}
		public override bool IsSkin { get { return true; } }
		public override BorderPainter GetBorderPainter(BaseView view, BorderStyles border) { 
			if(border == BorderStyles.NoBorder) return null;
			return new SkinGridBorderPainter(view);
		}
		public override string Name { get { return "Skin"; } }
		public override CardElementsPainter CreateElementsPainter(BaseView view) { return new DevExpress.XtraGrid.Skins.SkinCardElementsPainter(view);	}
		protected internal static AppearanceDefault UpdateAppearance(BaseView view, string elementName, AppearanceDefault info) {
			return GridSkinPaintStyle.UpdateAppearance(view, elementName, info);
		}
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(BaseView view) { 
			return new AppearanceDefaultInfo[] {
			new AppearanceDefaultInfo("EmptySpace", GridSkinPaintStyle.UpdateAppearanceEx(view, GridSkins.SkinGridEmptyArea, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Near, VertAlignment.Center))),
			new AppearanceDefaultInfo("CardCaption", UpdateAppearanceFore(view, GridSkins.SkinCardCaption, new AppearanceDefault(SystemColors.InactiveCaptionText, SystemColors.InactiveCaption, SystemColors.InactiveBorder, Color.Empty, HorzAlignment.Center, VertAlignment.Center))),
			new AppearanceDefaultInfo("ViewCaption", UpdateAppearanceFore(view, GridSkins.SkinViewCaption, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center))),
			new AppearanceDefaultInfo("FieldCaption", UpdateAppearanceFore(view, GridSkins.SkinCard, new AppearanceDefault(SystemColors.WindowText, Color.Empty, HorzAlignment.Near, VertAlignment.Top))),
			new AppearanceDefaultInfo("FieldValue", UpdateAppearanceFore(view, GridSkins.SkinCard, new AppearanceDefault(SystemColors.WindowText, Color.Empty, HorzAlignment.Default, VertAlignment.Default))),
			new AppearanceDefaultInfo("FocusedCardCaption", UpdateAppearanceFore(view, GridSkins.SkinCardCaptionSelected, new AppearanceDefault(SystemColors.ActiveCaptionText, SystemColors.ActiveCaption, SystemColors.InactiveCaption, Color.Empty, HorzAlignment.Center, VertAlignment.Center))),
			new AppearanceDefaultInfo("SelectedCardCaption", UpdateAppearanceFore(view, GridSkins.SkinCardCaptionSelected, new AppearanceDefault(SystemColors.ActiveCaptionText, SystemColors.ActiveCaption, SystemColors.InactiveBorder, Color.Empty, HorzAlignment.Center, VertAlignment.Center))),
			new AppearanceDefaultInfo("HideSelectionCardCaption", UpdateAppearanceFore(view, GridSkins.SkinCardCaptionHideSelection, GridSkins.SkinCardCaptionSelected, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, SystemColors.InactiveBorder, Color.Empty, HorzAlignment.Center, VertAlignment.Center))),
			new AppearanceDefaultInfo("SeparatorLine", UpdateAppearance(view, GridSkins.SkinCardSeparator, new AppearanceDefault(SystemColors.ActiveBorder, SystemColors.ActiveBorder, HorzAlignment.Near, VertAlignment.Center))),
			new AppearanceDefaultInfo("CardButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("CardExpandButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("Card", UpdateAppearance(view, GridSkins.SkinCard, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, SystemColors.WindowFrame, Color.Empty, HorzAlignment.Near, VertAlignment.Center))),
			new AppearanceDefaultInfo("FilterPanel", UpdateAppearance(view, GridSkins.SkinGridFilterPanel, new AppearanceDefault(SystemColors.ControlLightLight, SystemColors.ControlDark, Color.Empty, SystemColors.ControlLight, LinearGradientMode.ForwardDiagonal, HorzAlignment.Near, VertAlignment.Center))),
			new AppearanceDefaultInfo("FilterCloseButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("SelectionFrame", UpdateSystemColors(view, new AppearanceDefault(Color.Black, Color.Blue, Color.White,Color.Empty, LinearGradientMode.Horizontal)))
		 };
		}
		protected internal static AppearanceDefault UpdateSystemColors(BaseView view, AppearanceDefault info) {
			return GridSkinPaintStyle.UpdateSystemColors(view, info);
		}
		protected AppearanceDefault UpdateAppearanceFore(BaseView view, string elementName, string altElement, AppearanceDefault info) {
			SkinElement element = GridSkins.GetSkin(view)[elementName];
			if(element == null) elementName = altElement;
			return UpdateAppearanceFore(view, elementName, info);
		}
		protected AppearanceDefault UpdateAppearanceFore(BaseView view, string elementName, AppearanceDefault info) {
			SkinElement element = GridSkins.GetSkin(view)[elementName];
			if(element == null) return UpdateSystemColors(view, info);
			if(element.Color.FontBold) {
				info.Font = new Font(info.Font == null ? AppearanceObject.DefaultFont : info.Font, FontStyle.Bold);
			}
			if(element.Color.GetForeColor() != Color.Empty) {
				info.ForeColor = element.Color.GetForeColor();
			}
			return info;
		}
	}
	public class Card3DPaintStyle : CardPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.SetStyle3D();
		}
		public override string Name { get { return "Style3D"; } }
	}
	public class CardUltraFlatPaintStyle : CardPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = false;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.UltraFlat;
		}
		public override string Name { get { return "UltraFlat"; } }
	}
	public class CardOffice2003PaintStyle : CardPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = false;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.Office2003;
		}
		public override string Name { get { return "Office2003"; } }
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(BaseView view) { return CardOfficeAppearances; }
		public override CardElementsPainter CreateElementsPainter(BaseView view) { return new CardElementsPainterOffice2003(view);	}
	}
	public class CardPaintStyle : ViewPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.SetFlatStyle();
		}
		public override string Name { get { return "Flat"; } }
		public virtual CardElementsPainter CreateElementsPainter(BaseView view) { return new CardElementsPainter(view);	}
		public override BaseViewPainter CreatePainter(BaseView view) { return new CardPainter(view as DevExpress.XtraGrid.Views.Card.CardView); }
		public override BaseViewInfo CreateViewInfo(BaseView view) { return new DevExpress.XtraGrid.Views.Card.ViewInfo.CardViewInfo(view as DevExpress.XtraGrid.Views.Card.CardView); }
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(BaseView view) { return cardAppearances; }
		static AppearanceDefaultInfo[] cardAppearances = new AppearanceDefaultInfo[] {
			new AppearanceDefaultInfo("EmptySpace", new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("ViewCaption", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("CardCaption", new AppearanceDefault(SystemColors.InactiveCaptionText, SystemColors.InactiveCaption, SystemColors.InactiveBorder, Color.Empty, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("FieldCaption", new AppearanceDefault(SystemColors.WindowText, Color.Empty, HorzAlignment.Near, VertAlignment.Top)),
			new AppearanceDefaultInfo("FieldValue", new AppearanceDefault(SystemColors.WindowText, Color.Empty, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("FocusedCardCaption", new AppearanceDefault(SystemColors.ActiveCaptionText, SystemColors.ActiveCaption, SystemColors.InactiveCaption, Color.Empty, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("SelectedCardCaption", new AppearanceDefault(SystemColors.ActiveCaptionText, SystemColors.ActiveCaption, SystemColors.InactiveBorder, Color.Empty, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("HideSelectionCardCaption", new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, SystemColors.InactiveBorder, Color.Empty, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("SeparatorLine", new AppearanceDefault(SystemColors.ActiveBorder, SystemColors.ActiveBorder, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("CardButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("CardExpandButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("Card", new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, SystemColors.WindowFrame, Color.Empty, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("FilterPanel", new AppearanceDefault(SystemColors.ControlLightLight, SystemColors.ControlDark, Color.Empty, SystemColors.ControlLight, LinearGradientMode.ForwardDiagonal, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("FilterCloseButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("SelectionFrame", new AppearanceDefault(Color.Black, Color.Blue, Color.White,Color.Empty, LinearGradientMode.Horizontal))
		};
		static AppearanceDefaultInfo[] cardOfficeAppearances = null;
		protected static AppearanceDefaultInfo[] CardOfficeAppearances {
			get {
				if(cardOfficeAppearances == null) cardOfficeAppearances = CreateCardOfficeAppearances();
				return cardOfficeAppearances;
			}
		}
		static AppearanceDefaultInfo[] CreateCardOfficeAppearances() { return new AppearanceDefaultInfo[] {
			new AppearanceDefaultInfo("EmptySpace", new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("ViewCaption", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("CardCaption", new AppearanceDefault(SystemColors.ControlText, Office2003Colors.Default[Office2003Color.Button1], Office2003Colors.Default[Office2003Color.Button2], 
													Office2003Colors.Default[Office2003Color.Button2], LinearGradientMode.Vertical, HorzAlignment.Center, VertAlignment.Center)), 
			new AppearanceDefaultInfo("FieldCaption", new AppearanceDefault(SystemColors.WindowText, Color.Empty, HorzAlignment.Near, VertAlignment.Top)),
			new AppearanceDefaultInfo("FieldValue", new AppearanceDefault(SystemColors.WindowText, Color.Empty, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("FocusedCardCaption", new AppearanceDefault(SystemColors.ControlText, Office2003Colors.Default[Office2003Color.Button1Pressed], Office2003Colors.Default[Office2003Color.Button2Pressed], 
													Office2003Colors.Default[Office2003Color.Button2Pressed], LinearGradientMode.Vertical, HorzAlignment.Center, VertAlignment.Center)), 
			new AppearanceDefaultInfo("SelectedCardCaption", new AppearanceDefault(SystemColors.ControlText, Office2003Colors.Default[Office2003Color.Button1Pressed], Office2003Colors.Default[Office2003Color.Button2], 
													Office2003Colors.Default[Office2003Color.Button2Pressed], LinearGradientMode.Vertical, HorzAlignment.Center, VertAlignment.Center)), 
			new AppearanceDefaultInfo("HideSelectionCardCaption", new AppearanceDefault(SystemColors.ControlText, Office2003Colors.Default[Office2003Color.Button1], Office2003Colors.Default[Office2003Color.Button2Pressed], 
													Office2003Colors.Default[Office2003Color.Button2], LinearGradientMode.Vertical, HorzAlignment.Center, VertAlignment.Center)), 
			new AppearanceDefaultInfo("SeparatorLine", new AppearanceDefault(SystemColors.ActiveBorder, SystemColors.ActiveBorder, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("CardButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("CardExpandButton", new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("Card", new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, SystemColors.WindowFrame, Color.Empty, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("FilterPanel", new AppearanceDefault(SystemColors.ControlLightLight, SystemColors.ControlDark, Color.Empty, SystemColors.ControlLight, LinearGradientMode.ForwardDiagonal, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("FilterCloseButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("SelectionFrame", new AppearanceDefault(Color.Black, Color.Blue, Color.White,Color.Empty, LinearGradientMode.Horizontal))
		  };
		}
		protected internal override void UpdateTheme() {
			cardOfficeAppearances = null;
			base.UpdateTheme();
		}
	}
	public class CardInfoRegistrator : BaseInfoRegistrator {
		protected override void RegisterViewPaintStyles() {
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new CardPaintStyle(); }, "Flat"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new CardWindowsXPPaintStyle(); }, "WindowsXP"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new Card3DPaintStyle(); }, "Style3D"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new CardUltraFlatPaintStyle(); }, "UltraFlat"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new CardOffice2003PaintStyle(); }, "Office2003"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new CardMixedXPPaintStyle(); }, "MixedXP"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new CardSkinPaintStyle(); }, "Skin"));
		}
		protected override BaseGridDesigner CreateDesigner() { return new CardViewDesigner(); } 
		public override bool IsInternalView { get { return false; } }
		public override string ViewName { get { return "CardView"; } }
		public override string StyleOwnerName { get { return "CardView"; } }
		public override BaseView CreateView(GridControl grid) { 
			BaseView view = new DevExpress.XtraGrid.Views.Card.CardView(); 
			view.SetGridControl(grid);
			return view;
		}
		public override BaseViewHandler CreateHandler(BaseView view) { return new DevExpress.XtraGrid.Views.Card.Handler.CardHandler(view as DevExpress.XtraGrid.Views.Card.CardView); }
		static AppearanceDefaultInfo[] cardPrintAppearances = new AppearanceDefaultInfo[] {
			new AppearanceDefaultInfo("CardCaption", new AppearanceDefault(Color.Black, Color.Silver, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("FieldCaption", new AppearanceDefault(Color.Black, Color.LightGray, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("FieldValue", new AppearanceDefault(Color.Black, Color.White, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("Card", new AppearanceDefault(Color.Transparent, Color.Transparent, Color.Gray))
		};
		public override AppearanceDefaultInfo[] GetDefaultPrintAppearance() {  return cardPrintAppearances; }
	}
	public class GridPaintStyle : ViewPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = false;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.Flat;
		}
		public virtual bool GetDefaultShowVerticalLines(BaseView view) { return true; }
		public virtual bool GetDefaultShowPreviewRowLines(BaseView view) { return true; }
		public virtual bool GetDefaultShowHorizontalLines(BaseView view) { return true; }
		public virtual GroupDrawMode GetGroupDrawMode(BaseView view) { return GroupDrawMode.Standard; }
		public virtual GridElementsPainter CreateElementsPainter(BaseView view) { return new GridElementsPainter(view);	}
		public override string Name { get { return "Flat"; } }
		public override BaseViewPainter CreatePainter(BaseView view) { return new GridPainter(view as DevExpress.XtraGrid.Views.Grid.GridView); }
		public override BaseViewInfo CreateViewInfo(BaseView view) { return new DevExpress.XtraGrid.Views.Grid.ViewInfo.GridViewInfo(view as DevExpress.XtraGrid.Views.Grid.GridView); }
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(BaseView view) { return gridAppearance; }
		static AppearanceDefaultInfo[] gridAppearance = new AppearanceDefaultInfo[] {
			new AppearanceDefaultInfo("HeaderPanel", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("HeaderPanelBackground", new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupPanel", new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Empty, SystemColors.ControlLightLight, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("FooterPanel", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Far, VertAlignment.Center)),
			new AppearanceDefaultInfo("ViewCaption", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("TopNewRow", new AppearanceDefault(SystemColors.GrayText, SystemColors.Window, HorzAlignment.Center, VertAlignment.Default)),
			new AppearanceDefaultInfo("Row", new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("RowSeparator", new AppearanceDefault(SystemColors.ControlDark, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupRow", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("EvenRow", new AppearanceDefault(Color.Empty, Color.LightSkyBlue, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("OddRow", new AppearanceDefault(Color.Empty, Color.NavajoWhite, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("HorzLine", new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDark, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("VertLine", new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDark, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("Preview", new AppearanceDefault(Color.Blue, SystemColors.Window, HorzAlignment.Near, VertAlignment.Top)),
			new AppearanceDefaultInfo("FocusedRow", new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("FocusedCell", new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("GroupButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("DetailTip", new AppearanceDefault(SystemColors.InfoText, SystemColors.Info, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("FilterPanel", new AppearanceDefault(SystemColors.ControlLight, SystemColors.ControlDark, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("FilterCloseButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupFooter", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Far, VertAlignment.Center)),
			new AppearanceDefaultInfo("Empty", new AppearanceDefault(SystemColors.Window, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("SelectedRow", new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("HideSelectionRow", new AppearanceDefault(SystemColors.InactiveCaptionText, SystemColors.InactiveCaption, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("ColumnFilterButton", new AppearanceDefault(SystemColors.ControlDark, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ColumnFilterButtonActive", new AppearanceDefault(Color.Blue, SystemColors.Control, Color.Empty, SystemColors.ControlLightLight, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("FixedLine", new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDarkDark, HorzAlignment.Default, VertAlignment.Center)), 
			new AppearanceDefaultInfo("CustomizationFormHint", new AppearanceDefault(SystemColors.ControlText, Color.Empty))
		};
	}
	public class GridWebPaintStyle : GridPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = false;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.Office2003;
		}
		public override GroupDrawMode GetGroupDrawMode(BaseView view) { return GroupDrawMode.Standard; }
		public override GridElementsPainter CreateElementsPainter(BaseView view) { return new GridWebElementsPainter(view);	}
		public override string Name { get { return "Web"; } }
		public override BaseViewPainter CreatePainter(BaseView view) { return new GridPainter(view as DevExpress.XtraGrid.Views.Grid.GridView); }
		public override BaseViewInfo CreateViewInfo(BaseView view) { return new DevExpress.XtraGrid.Views.Grid.ViewInfo.GridViewInfo(view as DevExpress.XtraGrid.Views.Grid.GridView); }
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(BaseView view) { return gridAppearance; }
		static AppearanceDefaultInfo[] gridAppearance = new AppearanceDefaultInfo[] {
			new AppearanceDefaultInfo("HeaderPanel", new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, SystemColors.ControlLight, Color.Empty, new Font(AppearanceObject.DefaultFont, FontStyle.Bold), HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("HeaderPanelBackground", new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupPanel", new AppearanceDefault(SystemColors.ControlText, SystemColors.Window, Color.Empty, SystemColors.ControlLight, LinearGradientMode.Vertical, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("FooterPanel", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Far, VertAlignment.Center)),
			new AppearanceDefaultInfo("ViewCaption", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("TopNewRow", new AppearanceDefault(SystemColors.GrayText, SystemColors.Window, HorzAlignment.Center, VertAlignment.Default)),
			new AppearanceDefaultInfo("Row", new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("RowSeparator", new AppearanceDefault(SystemColors.ControlDark, SystemColors.Window, HorzAlignment.Default)),
			new AppearanceDefaultInfo("GroupRow", new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("EvenRow", new AppearanceDefault(Color.Empty, Color.LightSkyBlue, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("OddRow", new AppearanceDefault(Color.Empty, Color.NavajoWhite, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("HorzLine", new AppearanceDefault(SystemColors.ControlLight, SystemColors.ControlLight)),
			new AppearanceDefaultInfo("VertLine", new AppearanceDefault(SystemColors.ControlLight, SystemColors.ControlLight)),
			new AppearanceDefaultInfo("Preview", new AppearanceDefault(Color.Blue, SystemColors.Window, HorzAlignment.Near, VertAlignment.Top)),
			new AppearanceDefaultInfo("FocusedRow", new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("FocusedCell", new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("GroupButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("DetailTip", new AppearanceDefault(SystemColors.InfoText, SystemColors.Info, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("FilterPanel", new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, SystemColors.ControlLight, Color.Empty, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("FilterCloseButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupFooter", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Far, VertAlignment.Center)),
			new AppearanceDefaultInfo("Empty", new AppearanceDefault(SystemColors.Window, SystemColors.Window)),
			new AppearanceDefaultInfo("SelectedRow", new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("HideSelectionRow", new AppearanceDefault(SystemColors.InactiveCaptionText, SystemColors.InactiveCaption, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("ColumnFilterButton", new AppearanceDefault(SystemColors.ControlDark, SystemColors.Control, SystemColors.ControlLight, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ColumnFilterButtonActive", new AppearanceDefault(Color.Blue, SystemColors.Control, Color.Empty, SystemColors.ControlLightLight, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("FixedLine", new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDarkDark)),
			new AppearanceDefaultInfo("CustomizationFormHint", new AppearanceDefault(SystemColors.WindowText, Color.Empty))
		};
	}
	public class GridInfoRegistrator : BaseInfoRegistrator {
		public GridInfoRegistrator() {
		}
		protected override BaseGridDesigner CreateDesigner() { return new GridViewDesigner(); } 
		protected override void RegisterViewPaintStyles() {
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new GridPaintStyle(); }, "Flat"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new GridWindowsXPPaintStyle(); }, "WindowsXP"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new Grid3DPaintStyle(); }, "Style3D"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new GridUltraFlatPaintStyle(); }, "UltraFlat"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new GridOffice2003PaintStyle(); }, "Office2003"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new GridMixedXPPaintStyle(); }, "MixedXP"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new GridWebPaintStyle(); }, "Web"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new GridSkinPaintStyle(); }, "Skin"));
		}
		public override bool IsInternalView { get { return false; } }
		public override string StyleOwnerName { get { return "Grid"; } }
		public override string ViewName { get { return "GridView"; } }
		public override BaseView CreateView(GridControl grid) { 
			BaseView view = new DevExpress.XtraGrid.Views.Grid.GridView(); 
			view.SetGridControl(grid);
			return view;
		}
		public override BaseViewHandler CreateHandler(BaseView view) { return new DevExpress.XtraGrid.Views.Grid.Handler.GridHandler(view as DevExpress.XtraGrid.Views.Grid.GridView); }
		static AppearanceDefaultInfo[] gridPrintAppearances = new AppearanceDefaultInfo[] {
			new AppearanceDefaultInfo("HeaderPanel", new AppearanceDefault(Color.Black, Color.LightGray, Color.DarkGray, Color.Empty, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("Row", new AppearanceDefault(Color.Black, Color.White, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("EvenRow", new AppearanceDefault(Color.Empty, Color.LightSkyBlue, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("OddRow", new AppearanceDefault(Color.Empty, Color.NavajoWhite, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("GroupRow", new AppearanceDefault(Color.Black, Color.Gainsboro, HorzAlignment.Near, VertAlignment.Default)),
			new AppearanceDefaultInfo("Lines", new AppearanceDefault(Color.DarkGray, Color.DarkGray, HorzAlignment.Default)),
			new AppearanceDefaultInfo("Preview", new AppearanceDefault(Color.DimGray, Color.White, HorzAlignment.Near, VertAlignment.Top)),
			new AppearanceDefaultInfo("FilterPanel", new AppearanceDefault(Color.White, Color.Gray, HorzAlignment.Near, VertAlignment.Top)),
			new AppearanceDefaultInfo("FooterPanel", new AppearanceDefault(Color.Black, Color.DarkGray, HorzAlignment.Far, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupFooter", new AppearanceDefault(Color.Black, Color.LightGray, HorzAlignment.Far, VertAlignment.Center))
		};
		public override AppearanceDefaultInfo[] GetDefaultPrintAppearance() {  return gridPrintAppearances; }
	}
	public class GridWindowsXPPaintStyle : GridPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = true;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.Flat;
		}
		public override bool CanUsePaintStyle { get { return DevExpress.Utils.WXPaint.WXPPainter.Default.ThemesEnabled; } }
		public override GridElementsPainter CreateElementsPainter(BaseView view) { return new DevExpress.XtraGrid.Views.Grid.Drawing.GridWindowsXPElementsPainter(view);}
		public override string Name { get { return "WindowsXP"; } }
	}
	public class GridMixedXPPaintStyle : GridPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = true;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.Flat;
		}
		public override bool CanUsePaintStyle { get { return DevExpress.Utils.WXPaint.WXPPainter.Default.ThemesEnabled; } }
		public override string Name { get { return "MixedXP"; } }
		public override GridElementsPainter CreateElementsPainter(BaseView view) { return new DevExpress.XtraGrid.Views.Grid.Drawing.GridMixedXPElementsPainter(view);}
	}
	public class Grid3DPaintStyle : GridPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = false;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.Style3D;
		}
		public override GridElementsPainter CreateElementsPainter(BaseView view) { return new DevExpress.XtraGrid.Views.Grid.Drawing.GridStyle3DElementsPainter(view);}
		public override string Name { get { return "Style3D"; } }
	}
	public class GridSkinPaintStyle : GridPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			var lookAndFeel = view.GridControl == null ? view.ElementsLookAndFeel : view.GridControl.LookAndFeel.ActiveLookAndFeel;
			view.ElementsLookAndFeel.SetSkinStyleCore(lookAndFeel);
		}
		public override GroupDrawMode GetGroupDrawMode(BaseView view) { return GetGroupDrawModeCore(view); }
		internal static GroupDrawMode GetGroupDrawModeCore(BaseView view) {
			object prop = GridSkins.GetSkin(view).Properties[GridSkins.OptGridGroupDrawMode];
			if(prop == null) return GroupDrawMode.Standard;
			return (GroupDrawMode)Enum.Parse(typeof(GroupDrawMode), prop.ToString());
		}
		public override bool IsSkin { get { return true; } }
		public override BorderPainter GetBorderPainter(BaseView view, BorderStyles border) { 
			if(border == BorderStyles.NoBorder) return null;
			return new SkinGridBorderPainter(view);
		}
		public override GridElementsPainter CreateElementsPainter(BaseView view) { return new DevExpress.XtraGrid.Skins.GridSkinElementsPainter(view);}
		public override string Name { get { return "Skin"; } }
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(BaseView view) {
			return new AppearanceDefaultInfo[] {
			new AppearanceDefaultInfo("HeaderPanel", UpdateAppearanceEx(view, GridSkins.SkinHeader, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Near, VertAlignment.Center))),
			new AppearanceDefaultInfo("HeaderPanelBackground", UpdateAppearanceEx(view, GridSkins.SkinHeader, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, HorzAlignment.Near, VertAlignment.Center))),
			new AppearanceDefaultInfo("GroupPanel", UpdateAppearanceEx(view, GridSkins.SkinGridGroupPanel, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Empty, SystemColors.ControlLightLight, HorzAlignment.Near, VertAlignment.Center))),
			new AppearanceDefaultInfo("FooterPanel", UpdateAppearanceEx(view, GridSkins.SkinFooterPanel, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Far, VertAlignment.Center))),
			new AppearanceDefaultInfo("ViewCaption", UpdateAppearanceEx(view, GridSkins.SkinViewCaption, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center))),
			new AppearanceDefaultInfo("TopNewRow", UpdateAppearanceEx(view, GridSkins.SkinGridTopNewRow, new AppearanceDefault(SystemColors.GrayText, SystemColors.Window, HorzAlignment.Center, VertAlignment.Default))),
			new AppearanceDefaultInfo("Row", UpdateAppearance(view, GridSkins.SkinGridRow, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Default))),
			new AppearanceDefaultInfo("RowSeparator", UpdateSystemColors(view, new AppearanceDefault(SystemColors.ControlDark, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center))),
			new AppearanceDefaultInfo("GroupRow", UpdateAppearance(view, GridSkins.SkinGridGroupRow, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Near, VertAlignment.Center))),
			new AppearanceDefaultInfo("EvenRow", UpdateAppearance(view, GridSkins.SkinGridEvenRow, new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Default))),
			new AppearanceDefaultInfo("OddRow", UpdateAppearance(view, GridSkins.SkinGridOddRow, new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Default))),
			new AppearanceDefaultInfo("HorzLine", UpdateAppearance(view, GridSkins.SkinGridLine, new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDark))),
			new AppearanceDefaultInfo("VertLine", UpdateAppearance(view, GridSkins.SkinGridLine, new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDark))),
			new AppearanceDefaultInfo("Preview", UpdateAppearanceEx(view, GridSkins.SkinGridPreview, new AppearanceDefault(Color.Blue, SystemColors.Window, HorzAlignment.Near, VertAlignment.Top))),
			new AppearanceDefaultInfo("FocusedRow", UpdateSystemColors(view, new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight, HorzAlignment.Default, VertAlignment.Default))),
			new AppearanceDefaultInfo("FocusedCell", UpdateAppearance(view, GridSkins.SkinGridRow, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Default))),
			new AppearanceDefaultInfo("GroupButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("DetailTip", UpdateSystemColors(view, new AppearanceDefault(SystemColors.InfoText, SystemColors.Info, HorzAlignment.Default, VertAlignment.Center))),
			new AppearanceDefaultInfo("FilterPanel", UpdateAppearance(view, GridSkins.SkinGridFilterPanel, new AppearanceDefault(SystemColors.ControlLightLight, SystemColors.ControlDark, Color.Empty, SystemColors.ControlLight, LinearGradientMode.ForwardDiagonal, HorzAlignment.Near, VertAlignment.Center))),
			new AppearanceDefaultInfo("FilterCloseButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupFooter", UpdateAppearance(view, GridSkins.SkinFooterPanel, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Far, VertAlignment.Center))),
			new AppearanceDefaultInfo("Empty", UpdateAppearanceEx(view, GridSkins.SkinGridEmptyArea, new AppearanceDefault(SystemColors.Window, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center))),
			new AppearanceDefaultInfo("SelectedRow", UpdateSystemColors(view, new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight, HorzAlignment.Default, VertAlignment.Default))),
			new AppearanceDefaultInfo("HideSelectionRow", UpdateSystemColors(view, new AppearanceDefault(SystemColors.InactiveCaptionText, SystemColors.InactiveCaption, HorzAlignment.Default, VertAlignment.Default))),
			new AppearanceDefaultInfo("ColumnFilterButton", new AppearanceDefault(SystemColors.ControlDark, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ColumnFilterButtonActive", new AppearanceDefault(Color.Blue, SystemColors.Control, Color.Empty, SystemColors.ControlLightLight, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("FixedLine", UpdateAppearance(view, GridSkins.SkinGridFixedLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDarkDark, HorzAlignment.Default, VertAlignment.Center))),
			new AppearanceDefaultInfo("CustomizationFormHint", UpdateAppearanceEx(view, CommonSkins.SkinLabel, new AppearanceDefault(SystemColors.ControlText, Color.Empty)))
			};
		}
		public Skin GetSkin(BaseView view) { return GridSkins.GetSkin(view); }
		protected internal static AppearanceDefault UpdateSystemColors(BaseView view, AppearanceDefault info) {
			info.ForeColor = CommonSkins.GetSkin(view).TranslateColor(info.ForeColor);
			info.BackColor = CommonSkins.GetSkin(view).TranslateColor(info.BackColor);
			return info;
		}
		protected internal static AppearanceDefault UpdateAppearanceEx(BaseView view, string elementName, AppearanceDefault info) {
			info = UpdateSystemColors(view, info);
			return UpdateAppearance(view, elementName, info);
		}
		protected internal static AppearanceDefault UpdateAppearance(BaseView view, SkinElement element, AppearanceDefault info) {
			if(element == null) return info;
			element.Apply(info);
			return info;
		}
		protected internal static AppearanceDefault UpdateAppearance(BaseView view, string elementName, AppearanceDefault info) {
			SkinElement element = GridSkins.GetSkin(view)[elementName] ?? CommonSkins.GetSkin(view)[elementName];
			return UpdateAppearance(view, element, info);
		}
		public override bool GetDefaultShowVerticalLines(BaseView view) { return GetSkin(view).Properties.GetBoolean(GridSkins.OptShowVerticalLines, true); }
		public override bool GetDefaultShowPreviewRowLines(BaseView view) { return GetSkin(view).Properties.GetBoolean(GridSkins.OptShowPreviewRowLines, true); }
		public override bool GetDefaultShowHorizontalLines(BaseView view) { return GetSkin(view).Properties.GetBoolean(GridSkins.OptShowHorizontalLines, true); }
	}
	public class BandedGridSkinPaintStyle : BandedGridPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			var lookAndFeel = view.GridControl == null ? view.ElementsLookAndFeel : view.GridControl.LookAndFeel.ActiveLookAndFeel;
			view.ElementsLookAndFeel.SetSkinStyleCore(lookAndFeel);
		}
		public override bool IsSkin { get { return true; } }
		public override GroupDrawMode GetGroupDrawMode(BaseView view) { return GridSkinPaintStyle.GetGroupDrawModeCore(view); }
		public override BorderPainter GetBorderPainter(BaseView view, BorderStyles border) { 
			if(border == BorderStyles.NoBorder) return null;
			return new SkinGridBorderPainter(view);
		}
		public override GridElementsPainter CreateElementsPainter(BaseView view) { return new DevExpress.XtraGrid.Skins.BandedGridSkinElementsPainter(view);}
		public override string Name { get { return "Skin"; } }
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(BaseView view) {
			return new AppearanceDefaultInfo[] {
				new AppearanceDefaultInfo("BandPanel", UpdateAppearanceEx(view, GridSkins.SkinHeader, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo("HeaderPanel", UpdateAppearanceEx(view, GridSkins.SkinHeader, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo("HeaderPanelBackground", UpdateAppearanceEx(view, GridSkins.SkinHeader, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo("GroupPanel", UpdateAppearanceEx(view, GridSkins.SkinGridGroupPanel, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Empty, SystemColors.ControlLightLight, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo("FooterPanel", UpdateAppearanceEx(view, GridSkins.SkinFooterPanel, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Far, VertAlignment.Center))),
				new AppearanceDefaultInfo("ViewCaption", UpdateAppearanceEx(view, GridSkins.SkinViewCaption, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center))),
				new AppearanceDefaultInfo("TopNewRow", UpdateAppearanceEx(view, GridSkins.SkinGridTopNewRow, new AppearanceDefault(SystemColors.GrayText, SystemColors.Window, HorzAlignment.Center, VertAlignment.Default))),
				new AppearanceDefaultInfo("Row", UpdateAppearance(view, GridSkins.SkinGridRow, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Default))),
				new AppearanceDefaultInfo("RowSeparator", UpdateSystemColors(view, new AppearanceDefault(SystemColors.ControlDark, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center))),
				new AppearanceDefaultInfo("GroupRow", UpdateAppearance(view, GridSkins.SkinGridGroupRow, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo("EvenRow", UpdateAppearance(view, GridSkins.SkinGridEvenRow, new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Default))),
				new AppearanceDefaultInfo("OddRow", UpdateAppearance(view, GridSkins.SkinGridOddRow, new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Default))),
				new AppearanceDefaultInfo("HorzLine", UpdateAppearance(view, GridSkins.SkinGridLine, new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDark))),
				new AppearanceDefaultInfo("VertLine", UpdateAppearance(view, GridSkins.SkinGridLine, new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDark))),
				new AppearanceDefaultInfo("Preview", UpdateAppearanceEx(view, GridSkins.SkinGridPreview, new AppearanceDefault(Color.Blue, SystemColors.Window, HorzAlignment.Near, VertAlignment.Top))),
				new AppearanceDefaultInfo("FocusedRow", UpdateSystemColors(view, new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight, HorzAlignment.Default, VertAlignment.Default))),
				new AppearanceDefaultInfo("FocusedCell", UpdateAppearance(view, GridSkins.SkinGridRow, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Default))),
				new AppearanceDefaultInfo("GroupButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo("DetailTip", UpdateSystemColors(view, new AppearanceDefault(SystemColors.InfoText, SystemColors.Info, HorzAlignment.Default, VertAlignment.Center))),
				new AppearanceDefaultInfo("FilterPanel", UpdateAppearance(view, GridSkins.SkinGridFilterPanel, new AppearanceDefault(SystemColors.ControlLightLight, SystemColors.ControlDark, Color.Empty, SystemColors.ControlLight, LinearGradientMode.ForwardDiagonal, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo("FilterCloseButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo("GroupFooter", UpdateAppearance(view, GridSkins.SkinFooterPanel, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Far, VertAlignment.Center))),
				new AppearanceDefaultInfo("Empty", UpdateAppearanceEx(view, GridSkins.SkinGridEmptyArea, new AppearanceDefault(SystemColors.Window, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center))),
				new AppearanceDefaultInfo("SelectedRow", UpdateSystemColors(view, new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight, HorzAlignment.Default, VertAlignment.Default))),
				new AppearanceDefaultInfo("HideSelectionRow", UpdateSystemColors(view, new AppearanceDefault(SystemColors.InactiveCaptionText, SystemColors.InactiveCaption, HorzAlignment.Default, VertAlignment.Default))),
				new AppearanceDefaultInfo("ColumnFilterButton", new AppearanceDefault(SystemColors.ControlDark, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo("ColumnFilterButtonActive", new AppearanceDefault(Color.Blue, SystemColors.Control, Color.Empty, SystemColors.ControlLightLight, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo("FixedLine", UpdateAppearance(view, GridSkins.SkinGridFixedLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDarkDark, HorzAlignment.Default, VertAlignment.Center))),
				new AppearanceDefaultInfo("CustomizationFormHint", UpdateAppearanceEx(view, CommonSkins.SkinLabel, new AppearanceDefault(SystemColors.ControlText, Color.Empty)))
			};
		}
		public Skin GetSkin(BaseView view) { return GridSkins.GetSkin(view); }
		protected internal static AppearanceDefault UpdateSystemColors(BaseView view, AppearanceDefault info) {
			return GridSkinPaintStyle.UpdateSystemColors(view, info);
		}
		protected internal static AppearanceDefault UpdateAppearanceEx(BaseView view, string elementName, AppearanceDefault info) {
			return GridSkinPaintStyle.UpdateAppearanceEx(view, elementName, info);
		}
		protected internal static AppearanceDefault UpdateAppearance(BaseView view, string elementName, AppearanceDefault info) {
			return GridSkinPaintStyle.UpdateAppearance(view, elementName, info);
		}
		public override bool GetDefaultShowVerticalLines(BaseView view) { return GetSkin(view).Properties.GetBoolean(GridSkins.OptShowVerticalLines, true); }
		public override bool GetDefaultShowPreviewRowLines(BaseView view) { return GetSkin(view).Properties.GetBoolean(GridSkins.OptShowPreviewRowLines, true); }
		public override bool GetDefaultShowHorizontalLines(BaseView view) { return GetSkin(view).Properties.GetBoolean(GridSkins.OptShowHorizontalLines, true); }
	}
	public class AdvBandedGridSkinPaintStyle : AdvBandedGridPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			var lookAndFeel = view.GridControl == null ? view.ElementsLookAndFeel : view.GridControl.LookAndFeel.ActiveLookAndFeel;
			view.ElementsLookAndFeel.SetSkinStyleCore(lookAndFeel);
		}
		public override GroupDrawMode GetGroupDrawMode(BaseView view) { return GridSkinPaintStyle.GetGroupDrawModeCore(view); }
		public override bool IsSkin { get { return true; } }
		public override BorderPainter GetBorderPainter(BaseView view, BorderStyles border) { 
			if(border == BorderStyles.NoBorder) return null;
			return new SkinGridBorderPainter(view);
		}
		public override GridElementsPainter CreateElementsPainter(BaseView view) { return new DevExpress.XtraGrid.Skins.AdvBandedGridSkinElementsPainter(view);}
		public override string Name { get { return "Skin"; } }
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(BaseView view) {
			return new AppearanceDefaultInfo[] {
				new AppearanceDefaultInfo("BandPanel", GridSkinPaintStyle.UpdateAppearanceEx(view, GridSkins.SkinHeader, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo("HeaderPanel", GridSkinPaintStyle.UpdateAppearanceEx(view, GridSkins.SkinHeader, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo("HeaderPanelBackground", GridSkinPaintStyle.UpdateAppearanceEx(view, GridSkins.SkinHeader, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo("GroupPanel", GridSkinPaintStyle.UpdateAppearanceEx(view, GridSkins.SkinGridGroupPanel, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Empty, SystemColors.ControlLightLight, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo("FooterPanel", GridSkinPaintStyle.UpdateAppearanceEx(view, GridSkins.SkinFooterPanel, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Far, VertAlignment.Center))),
				new AppearanceDefaultInfo("ViewCaption", GridSkinPaintStyle.UpdateAppearanceEx(view, GridSkins.SkinViewCaption, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center))),
				new AppearanceDefaultInfo("TopNewRow", GridSkinPaintStyle.UpdateAppearanceEx(view, GridSkins.SkinGridTopNewRow, new AppearanceDefault(SystemColors.GrayText, SystemColors.Window, HorzAlignment.Center, VertAlignment.Default))),
				new AppearanceDefaultInfo("Row", UpdateAppearance(view, GridSkins.SkinGridRow, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Default))),
				new AppearanceDefaultInfo("RowSeparator", UpdateSystemColors(view, new AppearanceDefault(SystemColors.ControlDark, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center))),
				new AppearanceDefaultInfo("GroupRow", UpdateAppearance(view, GridSkins.SkinGridGroupRow, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo("EvenRow", UpdateAppearance(view, GridSkins.SkinGridEvenRow, new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Default))),
				new AppearanceDefaultInfo("OddRow", UpdateAppearance(view, GridSkins.SkinGridOddRow, new AppearanceDefault(Color.Empty, Color.Empty, HorzAlignment.Default, VertAlignment.Default))),
				new AppearanceDefaultInfo("HorzLine", UpdateAppearance(view, GridSkins.SkinGridLine, new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDark))),
				new AppearanceDefaultInfo("VertLine", UpdateAppearance(view, GridSkins.SkinGridLine, new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDark))),
				new AppearanceDefaultInfo("Preview", GridSkinPaintStyle.UpdateAppearanceEx(view, GridSkins.SkinGridPreview, new AppearanceDefault(Color.Blue, SystemColors.Window, HorzAlignment.Near, VertAlignment.Top))),
				new AppearanceDefaultInfo("FocusedRow", UpdateSystemColors(view, new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight, HorzAlignment.Default, VertAlignment.Default))),
				new AppearanceDefaultInfo("FocusedCell", UpdateAppearance(view, GridSkins.SkinGridRow, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Default))),
				new AppearanceDefaultInfo("GroupButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo("DetailTip", UpdateSystemColors(view, new AppearanceDefault(SystemColors.InfoText, SystemColors.Info, HorzAlignment.Default, VertAlignment.Center))),
				new AppearanceDefaultInfo("FilterPanel", UpdateAppearance(view, GridSkins.SkinGridFilterPanel, new AppearanceDefault(SystemColors.ControlLightLight, SystemColors.ControlDark, Color.Empty, SystemColors.ControlLight, LinearGradientMode.ForwardDiagonal, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo("FilterCloseButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo("GroupFooter", UpdateAppearance(view, GridSkins.SkinFooterPanel, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Far, VertAlignment.Center))),
				new AppearanceDefaultInfo("Empty", GridSkinPaintStyle.UpdateAppearanceEx(view, GridSkins.SkinGridEmptyArea, new AppearanceDefault(SystemColors.Window, SystemColors.Window, HorzAlignment.Default, VertAlignment.Center))),
				new AppearanceDefaultInfo("SelectedRow", UpdateSystemColors(view, new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight, HorzAlignment.Default, VertAlignment.Default))),
				new AppearanceDefaultInfo("HideSelectionRow", UpdateSystemColors(view, new AppearanceDefault(SystemColors.InactiveCaptionText, SystemColors.InactiveCaption, HorzAlignment.Default, VertAlignment.Default))),
				new AppearanceDefaultInfo("ColumnFilterButton", new AppearanceDefault(SystemColors.ControlDark, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo("ColumnFilterButtonActive", new AppearanceDefault(Color.Blue, SystemColors.Control, Color.Empty, SystemColors.ControlLightLight, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo("FixedLine", UpdateAppearance(view, GridSkins.SkinGridFixedLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDarkDark, HorzAlignment.Default, VertAlignment.Center))),
				new AppearanceDefaultInfo("CustomizationFormHint", GridSkinPaintStyle.UpdateAppearanceEx(view, CommonSkins.SkinLabel, new AppearanceDefault(SystemColors.ControlText, Color.Empty)))
			};
		}
		public Skin GetSkin(BaseView view) { return GridSkins.GetSkin(view); }
		protected internal static AppearanceDefault UpdateSystemColors(BaseView view, AppearanceDefault info) {
			return GridSkinPaintStyle.UpdateSystemColors(view, info);
		}
		protected internal static AppearanceDefault UpdateAppearance(BaseView view, string elementName, AppearanceDefault info) {
			return GridSkinPaintStyle.UpdateAppearance(view, elementName, info);
		}
		public override bool GetDefaultShowVerticalLines(BaseView view) { return GetSkin(view).Properties.GetBoolean(GridSkins.OptShowVerticalLines, true); }
		public override bool GetDefaultShowPreviewRowLines(BaseView view) { return GetSkin(view).Properties.GetBoolean(GridSkins.OptShowPreviewRowLines, true); }
		public override bool GetDefaultShowHorizontalLines(BaseView view) { return GetSkin(view).Properties.GetBoolean(GridSkins.OptShowHorizontalLines, true); }
	}
	public class GridUltraFlatPaintStyle : GridPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = false;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.UltraFlat;
		}
		public override GridElementsPainter CreateElementsPainter(BaseView view) { return new DevExpress.XtraGrid.Views.Grid.Drawing.GridUltraFlatElementsPainter(view);}
		public override string Name { get { return "UltraFlat"; } }
	}
	public class GridOffice2003PaintStyle : GridPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = false;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.Office2003;
		}
		public override GroupDrawMode GetGroupDrawMode(BaseView view) { return GroupDrawMode.Office2003; }
		public override GridElementsPainter CreateElementsPainter(BaseView view) { return new DevExpress.XtraGrid.Views.Grid.Drawing.GridOffice2003ElementsPainter(view);}
		public override string Name { get { return "Office2003"; } }
		static AppearanceDefaultInfo[] gridOfficeAppearance;
		static AppearanceDefaultInfo[] GridOfficeAppearance { 
			get { if(gridOfficeAppearance == null) gridOfficeAppearance = CreateOfficeAppearance(); 
				return gridOfficeAppearance;
			}
		}
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(BaseView view) { return GridOfficeAppearance; }
		static AppearanceDefault Get(Office2003GridAppearance appCode) { 
			AppearanceDefault res = Office2003Colors.Default[appCode].Clone() as AppearanceDefault;
			if(appCode == Office2003GridAppearance.FooterPanel)
				res.HAlignment = HorzAlignment.Far;
			return res;
		}
		static AppearanceDefault GetViewCaption() {
			AppearanceDefault res = Get(Office2003GridAppearance.Header);
			res.HAlignment = HorzAlignment.Center;
			return res;
		}
		static AppearanceDefaultInfo[] CreateOfficeAppearance() {
			return new AppearanceDefaultInfo[] {
			new AppearanceDefaultInfo("HeaderPanel", Get(Office2003GridAppearance.Header)),
			new AppearanceDefaultInfo("HeaderPanelBackground", new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupPanel", Get(Office2003GridAppearance.GroupPanel)),
			new AppearanceDefaultInfo("FooterPanel", Get(Office2003GridAppearance.FooterPanel)),
			new AppearanceDefaultInfo("ViewCaption", GetViewCaption()),
			new AppearanceDefaultInfo("TopNewRow", new AppearanceDefault(SystemColors.GrayText, SystemColors.Window, HorzAlignment.Center, VertAlignment.Default)),
			new AppearanceDefaultInfo("Row", new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("GroupRow", new AppearanceDefault(SystemColors.ControlText, Office2003Colors.Default[Office2003Color.GroupRow], HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("EvenRow", new AppearanceDefault(Color.Empty, Color.LightSkyBlue, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("OddRow", new AppearanceDefault(Color.Empty, Color.NavajoWhite, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("RowSeparator", new AppearanceDefault(SystemColors.ControlDark, SystemColors.Window)),
			new AppearanceDefaultInfo("HorzLine", Get(Office2003GridAppearance.GridLine)),
			new AppearanceDefaultInfo("VertLine", Get(Office2003GridAppearance.GridLine)),
			new AppearanceDefaultInfo("Preview", new AppearanceDefault(Color.Blue, SystemColors.Window, HorzAlignment.Near, VertAlignment.Top)),
			new AppearanceDefaultInfo("FocusedRow", new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("FocusedCell", new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("GroupButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("DetailTip", new AppearanceDefault(SystemColors.InfoText, SystemColors.Info, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("FilterPanel", Get(Office2003GridAppearance.FilterPanel)),
			new AppearanceDefaultInfo("FilterCloseButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupFooter", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Far, VertAlignment.Center)),
			new AppearanceDefaultInfo("Empty", new AppearanceDefault(SystemColors.Window, SystemColors.Window)),
			new AppearanceDefaultInfo("SelectedRow", new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("HideSelectionRow", new AppearanceDefault(SystemColors.InactiveCaptionText, SystemColors.InactiveCaption, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("ColumnFilterButton", new AppearanceDefault(SystemColors.ControlDark, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ColumnFilterButtonActive", new AppearanceDefault(Color.Blue, SystemColors.Control, Color.Empty, SystemColors.ControlLightLight, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("FixedLine", new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDarkDark)),
			new AppearanceDefaultInfo("CustomizationFormHint", new AppearanceDefault(SystemColors.ControlText, Color.Empty))
			};
		}
		protected internal override void UpdateTheme() {
			gridOfficeAppearance = null;
			base.UpdateTheme();
		}
	}
	public class BandedGridPaintStyle : GridPaintStyle {
		public override GridElementsPainter CreateElementsPainter(BaseView view) { return new BandedGridElementsPainter(view);	}
		public override string Name { get { return "Flat"; } }
		public override BaseViewPainter CreatePainter(BaseView view) { return new DevExpress.XtraGrid.Views.BandedGrid.Drawing.BandedGridPainter(view as DevExpress.XtraGrid.Views.BandedGrid.BandedGridView); }
		public override BaseViewInfo CreateViewInfo(BaseView view) { return new DevExpress.XtraGrid.Views.BandedGrid.ViewInfo.BandedGridViewInfo(view as DevExpress.XtraGrid.Views.BandedGrid.BandedGridView); }
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(BaseView view) { return gridAppearance; }
		static AppearanceDefaultInfo[] gridAppearance = new AppearanceDefaultInfo[] {
			new AppearanceDefaultInfo("BandPanel", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("BandPanelBackground", new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("HeaderPanelBackground", new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("HeaderPanel", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupPanel", new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Empty, SystemColors.ControlLightLight, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("FooterPanel", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Far, VertAlignment.Center)),
			new AppearanceDefaultInfo("ViewCaption", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("TopNewRow", new AppearanceDefault(SystemColors.GrayText, SystemColors.Window, HorzAlignment.Center, VertAlignment.Default)),
			new AppearanceDefaultInfo("Row", new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("GroupRow", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("EvenRow", new AppearanceDefault(Color.Empty, Color.LightSkyBlue, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("OddRow", new AppearanceDefault(Color.Empty, Color.NavajoWhite, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("RowSeparator", new AppearanceDefault(SystemColors.ControlDark, SystemColors.Window)),
			new AppearanceDefaultInfo("HorzLine", new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDark)),
			new AppearanceDefaultInfo("VertLine", new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDark)),
			new AppearanceDefaultInfo("Preview", new AppearanceDefault(Color.Blue, SystemColors.Window, HorzAlignment.Near, VertAlignment.Top)),
			new AppearanceDefaultInfo("FocusedRow", new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("FocusedCell", new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("GroupButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("DetailTip", new AppearanceDefault(SystemColors.InfoText, SystemColors.Info, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("FilterPanel", new AppearanceDefault(SystemColors.ControlLightLight, SystemColors.ControlDark, Color.Empty, SystemColors.ControlLight, LinearGradientMode.ForwardDiagonal, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupFooter", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Far, VertAlignment.Center)),
			new AppearanceDefaultInfo("Empty", new AppearanceDefault(SystemColors.Window, SystemColors.Window)),
			new AppearanceDefaultInfo("SelectedRow", new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("HideSelectionRow", new AppearanceDefault(SystemColors.InactiveCaptionText, SystemColors.InactiveCaption, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("ColumnFilterButton", new AppearanceDefault(SystemColors.ControlDark, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ColumnFilterButtonActive", new AppearanceDefault(Color.Blue, SystemColors.Control, Color.Empty, SystemColors.ControlLightLight, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("FilterCloseButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("FixedLine", new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDarkDark)),
			new AppearanceDefaultInfo("CustomizationFormHint", new AppearanceDefault(SystemColors.ControlText, Color.Empty))
		};
	}
	public class BandedGridInfoRegistrator : GridInfoRegistrator {
		protected override void RegisterViewPaintStyles() {
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new BandedGridPaintStyle(); }, "Flat"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new BandedGridWindowsXPPaintStyle(); }, "WindowsXP"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new BandedGrid3DPaintStyle(); }, "Style3D"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new BandedGridUltraFlatPaintStyle(); }, "UltraFlat"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new BandedGridOffice2003PaintStyle(); }, "Office2003"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new BandedGridMixedXPPaintStyle(); }, "MixedXP"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new BandedGridSkinPaintStyle(); }, "Skin"));
		}
		protected override BaseGridDesigner CreateDesigner() { return new BandedGridViewDesigner(); } 
		public override bool IsInternalView { get { return false; } }
		public override string ViewName { get { return "BandedGridView"; } }
		public override BaseView CreateView(GridControl grid) { 
			BaseView view = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridView(); 
			view.SetGridControl(grid);
			return view;
		}
		public override BaseViewHandler CreateHandler(BaseView view) { return new DevExpress.XtraGrid.Views.BandedGrid.Handler.BandedGridHandler(view as DevExpress.XtraGrid.Views.BandedGrid.BandedGridView); }
		static AppearanceDefaultInfo[] bandedGridPrintAppearances = new AppearanceDefaultInfo[] {
			new AppearanceDefaultInfo("BandPanel", new AppearanceDefault(Color.Black, Color.LightGray, Color.DarkGray, Color.Empty, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("HeaderPanel", new AppearanceDefault(Color.Black, Color.LightGray, Color.DarkGray, Color.Empty, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("EvenRow", new AppearanceDefault(Color.Empty, Color.LightSkyBlue, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("OddRow", new AppearanceDefault(Color.Empty, Color.NavajoWhite, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("Row", new AppearanceDefault(Color.Black, Color.White, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("GroupRow", new AppearanceDefault(Color.Black, Color.Gainsboro, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("Lines", new AppearanceDefault(Color.DarkGray, Color.DarkGray, HorzAlignment.Default)),
			new AppearanceDefaultInfo("Preview", new AppearanceDefault(Color.DimGray, Color.White, HorzAlignment.Near, VertAlignment.Top)),
			new AppearanceDefaultInfo("FilterPanel", new AppearanceDefault(Color.White, Color.Gray, HorzAlignment.Near, VertAlignment.Top)),
			new AppearanceDefaultInfo("FooterPanel", new AppearanceDefault(Color.Black, Color.DarkGray, HorzAlignment.Far, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupFooter", new AppearanceDefault(Color.Black, Color.LightGray, HorzAlignment.Far, VertAlignment.Center))
		};
		public override AppearanceDefaultInfo[] GetDefaultPrintAppearance() {  return bandedGridPrintAppearances; }
	}
	public class BandedGridWindowsXPPaintStyle : BandedGridPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = true;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.Flat;
		}
		public override bool CanUsePaintStyle { get { return DevExpress.Utils.WXPaint.WXPPainter.Default.ThemesEnabled; } }
		public override GridElementsPainter CreateElementsPainter(BaseView view) { return new DevExpress.XtraGrid.Views.BandedGrid.Drawing.BandedGridWindowsXPElementsPainter(view);}
		public override string Name { get { return "WindowsXP"; } }
	}
	public class BandedGridMixedXPPaintStyle : BandedGridPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = true;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.Flat;
		}
		public override bool CanUsePaintStyle { get { return DevExpress.Utils.WXPaint.WXPPainter.Default.ThemesEnabled; } }
		public override string Name { get { return "MixedXP"; } }
		public override GridElementsPainter CreateElementsPainter(BaseView view) { return new DevExpress.XtraGrid.Views.BandedGrid.Drawing.BandedGridMixedXPElementsPainter(view);}
	}
	public class BandedGrid3DPaintStyle : BandedGridPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = false;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.Style3D;
		}
		public override GridElementsPainter CreateElementsPainter(BaseView view) { return new DevExpress.XtraGrid.Views.BandedGrid.Drawing.BandedGridStyle3DElementsPainter(view);}
		public override string Name { get { return "Style3D"; } }
	}
	public class BandedGridUltraFlatPaintStyle : BandedGridPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = false;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.UltraFlat;
		}
		public override GridElementsPainter CreateElementsPainter(BaseView view) { return new DevExpress.XtraGrid.Views.BandedGrid.Drawing.BandedGridUltraFlatElementsPainter(view);}
		public override string Name { get { return "UltraFlat"; } }
	}
	public class BandedGridOffice2003PaintStyle : BandedGridPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = false;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.Office2003;
		}
		public override GridElementsPainter CreateElementsPainter(BaseView view) { return new DevExpress.XtraGrid.Views.BandedGrid.Drawing.BandedGridOffice2003ElementsPainter(view);}
		public override string Name { get { return "Office2003"; } }
		public override GroupDrawMode GetGroupDrawMode(BaseView view) { return GroupDrawMode.Office2003; }
		internal static AppearanceDefaultInfo[] bandedGridOfficeAppearance;
		internal static AppearanceDefaultInfo[] BandedGridOfficeAppearance { 
			get { if(bandedGridOfficeAppearance == null) bandedGridOfficeAppearance = CreateOfficeAppearance(); 
				return bandedGridOfficeAppearance;
			}
		}
		static AppearanceDefault Get(Office2003GridAppearance appCode) { 
			AppearanceDefault res = Office2003Colors.Default[appCode].Clone() as AppearanceDefault;
			if(appCode == Office2003GridAppearance.FooterPanel)
				res.HAlignment = HorzAlignment.Far;
			return res;
		}
		static AppearanceDefault GetViewCaption() {
			AppearanceDefault res = Get(Office2003GridAppearance.Header);
			res.HAlignment = HorzAlignment.Center;
			return res;
		}
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(BaseView view) { return BandedGridOfficeAppearance; }
		static AppearanceDefaultInfo[] CreateOfficeAppearance() {
			return new AppearanceDefaultInfo[] {
			new AppearanceDefaultInfo("BandPanel", new AppearanceDefault(SystemColors.ControlText, Office2003Colors.Default[Office2003Color.Header], Color.Empty, Office2003Colors.Default[Office2003Color.Header2], HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("BandPanelBackground", new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("HeaderPanel", Get(Office2003GridAppearance.Header)),
			new AppearanceDefaultInfo("HeaderPanelBackground", new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupPanel", Get(Office2003GridAppearance.GroupPanel)),
			new AppearanceDefaultInfo("FooterPanel", Get(Office2003GridAppearance.FooterPanel)),
			new AppearanceDefaultInfo("ViewCaption", GetViewCaption()),
			new AppearanceDefaultInfo("TopNewRow", new AppearanceDefault(SystemColors.GrayText, SystemColors.Window, HorzAlignment.Center, VertAlignment.Default)),
			new AppearanceDefaultInfo("Row", new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("GroupRow", new AppearanceDefault(SystemColors.ControlText, Office2003Colors.Default[Office2003Color.GroupRow], HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("EvenRow", new AppearanceDefault(Color.Empty, Color.LightSkyBlue, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("OddRow", new AppearanceDefault(Color.Empty, Color.NavajoWhite, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("RowSeparator", new AppearanceDefault(SystemColors.ControlDark, SystemColors.Window)),
			new AppearanceDefaultInfo("HorzLine", Get(Office2003GridAppearance.GridLine)),
			new AppearanceDefaultInfo("VertLine", Get(Office2003GridAppearance.GridLine)),
			new AppearanceDefaultInfo("Preview", new AppearanceDefault(Color.Blue, SystemColors.Window, HorzAlignment.Near, VertAlignment.Top)),
			new AppearanceDefaultInfo("FocusedRow", new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("FocusedCell", new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("GroupButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("DetailTip", new AppearanceDefault(SystemColors.InfoText, SystemColors.Info, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("FilterPanel", Get(Office2003GridAppearance.FilterPanel)),
			new AppearanceDefaultInfo("FilterCloseButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("GroupFooter", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Far, VertAlignment.Center)),
			new AppearanceDefaultInfo("Empty", new AppearanceDefault(SystemColors.Window, SystemColors.Window)),
			new AppearanceDefaultInfo("SelectedRow", new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("HideSelectionRow", new AppearanceDefault(SystemColors.InactiveCaptionText, SystemColors.InactiveCaption, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("ColumnFilterButton", new AppearanceDefault(SystemColors.ControlDark, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("ColumnFilterButtonActive", new AppearanceDefault(Color.Blue, SystemColors.Control, Color.Empty, SystemColors.ControlLightLight, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("FixedLine", new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDarkDark)),
			new AppearanceDefaultInfo("CustomizationFormHint", new AppearanceDefault(SystemColors.ControlText, Color.Empty))
			};
		}
		protected internal override void UpdateTheme() {
			bandedGridOfficeAppearance = null;
			base.UpdateTheme();
		}
	}
	public class AdvBandedGridPaintStyle : BandedGridPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = false;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.Flat;
		}
		public override GridElementsPainter CreateElementsPainter(BaseView view) { return new DevExpress.XtraGrid.Views.BandedGrid.Drawing.AdvBandedGridElementsPainter(view);}
		public override string Name { get { return "Flat"; } }
		public override BaseViewPainter CreatePainter(BaseView view) { return new DevExpress.XtraGrid.Views.BandedGrid.Drawing.AdvBandedGridPainter(view as DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView); }
		public override BaseViewInfo CreateViewInfo(BaseView view) { return new DevExpress.XtraGrid.Views.BandedGrid.ViewInfo.AdvBandedGridViewInfo(view as DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView); }
	}
	public class AdvBandedGridWindowsXPPaintStyle : AdvBandedGridPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = true;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.Flat;
		}
		public override bool CanUsePaintStyle { get { return DevExpress.Utils.WXPaint.WXPPainter.Default.ThemesEnabled; } }
		public override GridElementsPainter CreateElementsPainter(BaseView view) { return new DevExpress.XtraGrid.Views.BandedGrid.Drawing.AdvBandedGridWindowsXPElementsPainter(view);}
		public override string Name { get { return "WindowsXP"; } }
	}
	public class AdvBandedGridMixedXPPaintStyle : AdvBandedGridPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = true;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.Flat;
		}
		public override bool CanUsePaintStyle { get { return DevExpress.Utils.WXPaint.WXPPainter.Default.ThemesEnabled; } }
		public override string Name { get { return "MixedXP"; } }
		public override GridElementsPainter CreateElementsPainter(BaseView view) { return new DevExpress.XtraGrid.Views.BandedGrid.Drawing.AdvBandedGridMixedXPElementsPainter(view);}
	}
	public class AdvBandedGrid3DPaintStyle : AdvBandedGridPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = false;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.Style3D;
		}
		public override GridElementsPainter CreateElementsPainter(BaseView view) { return new DevExpress.XtraGrid.Views.BandedGrid.Drawing.AdvBandedGridStyle3DElementsPainter(view);}
		public override string Name { get { return "Style3D"; } }
	}
	public class AdvBandedGridUltraFlatPaintStyle : AdvBandedGridPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = false;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.UltraFlat;
		}
		public override GridElementsPainter CreateElementsPainter(BaseView view) { return new DevExpress.XtraGrid.Views.BandedGrid.Drawing.AdvBandedGridUltraFlatElementsPainter(view);}
		public override string Name { get { return "UltraFlat"; } }
	}
	public class AdvBandedGridOffice2003PaintStyle : AdvBandedGridPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = false;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.Office2003;
		}
		public override GridElementsPainter CreateElementsPainter(BaseView view) { return new DevExpress.XtraGrid.Views.BandedGrid.Drawing.AdvBandedGridOffice2003ElementsPainter(view);}
		public override string Name { get { return "Office2003"; } }
		public override GroupDrawMode GetGroupDrawMode(BaseView view) { return GroupDrawMode.Office2003; }
		protected internal override void UpdateTheme() {
			BandedGridOffice2003PaintStyle.bandedGridOfficeAppearance = null;
			base.UpdateTheme();
		}
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(BaseView view) { return BandedGridOffice2003PaintStyle.BandedGridOfficeAppearance; }
	}
	public class AdvBandedGridInfoRegistrator : BandedGridInfoRegistrator {
		protected override void RegisterViewPaintStyles() {
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new AdvBandedGridPaintStyle(); }, "Flat"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new AdvBandedGridWindowsXPPaintStyle(); }, "WindowsXP"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new AdvBandedGrid3DPaintStyle(); }, "Style3D"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new AdvBandedGridUltraFlatPaintStyle(); }, "UltraFlat"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new AdvBandedGridOffice2003PaintStyle(); }, "Office2003"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new AdvBandedGridMixedXPPaintStyle(); }, "MixedXP"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new AdvBandedGridSkinPaintStyle(); }, "Skin"));
		}
		public override bool IsInternalView { get { return false; } }
		public override string ViewName { get { return "AdvBandedGridView"; } }
		public override BaseView CreateView(GridControl grid) { 
			BaseView view = new DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView(); 
			view.SetGridControl(grid);
			return view;
		}
		public override BaseViewHandler CreateHandler(BaseView view) { return new DevExpress.XtraGrid.Views.BandedGrid.Handler.AdvBandedGridHandler(view as DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView); }
	}
	public class LayoutViewInfoRegistrator : BaseInfoRegistrator {
		public LayoutViewInfoRegistrator() {
		}
		protected override BaseGridDesigner CreateDesigner() { return new LayoutViewDesigner (); }
		protected override void RegisterViewPaintStyles() {
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new LayoutViewPaintStyle(); }, "Flat"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new LayoutViewWindowsXPPaintStyle(); }, "WindowsXP"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new LayoutViewUltraFlatPaintStyle(); }, "UltraFlat"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new LayoutViewOffice2003PaintStyle(); }, "Office2003"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new LayoutViewStyle3DPaintStyle(); }, "Style3D"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new LayoutViewMixedXPPaintStyle(); }, "MixedXP"));
			PaintStyles.Add(new FakeViewPaintStyle(delegate { return new LayoutViewSkinPaintStyle(); }, "Skin"));
		}
		public override bool IsInternalView { get { return false; } }
		public override string StyleOwnerName { get { return "Layout"; } }
		public override string ViewName { get { return "LayoutView"; } }
		public override BaseView CreateView(GridControl grid) {
			BaseView view = new DevExpress.XtraGrid.Views.Layout.LayoutView();
			view.SetGridControl(grid);
			return view;
		}
		public override BaseViewHandler CreateHandler(BaseView view) { return new DevExpress.XtraGrid.Views.Layout.Handler.LayoutViewHandler(view as DevExpress.XtraGrid.Views.Layout.LayoutView); }
		static AppearanceDefaultInfo[] cardPrintAppearances = new AppearanceDefaultInfo[] {
			new AppearanceDefaultInfo("CardCaption", new AppearanceDefault(Color.Black, Color.Silver, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("FieldCaption", new AppearanceDefault(Color.Black, Color.LightGray, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("FieldValue", new AppearanceDefault(Color.Black, Color.White, HorzAlignment.Default, VertAlignment.Default)),
			new AppearanceDefaultInfo("Card", new AppearanceDefault(Color.Transparent, Color.White, Color.Gray))
		};
		public override AppearanceDefaultInfo[] GetDefaultPrintAppearance() { return cardPrintAppearances; }
	}
	public class LayoutViewPaintStyle : ViewPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = false;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.Flat;
		}
		public override string Name { get { return "Flat"; } }
		public override BaseViewPainter CreatePainter(BaseView view) { return new LayoutViewPainter(view as LayoutView); }
		public override BaseViewInfo CreateViewInfo(BaseView view) { return new LayoutViewInfo(view as LayoutView); }
		public virtual LayoutViewElementsPainter CreateElementsPainter(BaseView view) { return new LayoutViewElementsPainter(view); }
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(BaseView view) { return appearance; }
		static AppearanceDefaultInfo[] appearance = new AppearanceDefaultInfo[] { 
			new AppearanceDefaultInfo("ViewBackground", new AppearanceDefault(Color.Black, Color.White, Color.Empty, Color.Empty)),
			new AppearanceDefaultInfo("HeaderPanel", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control)),
			new AppearanceDefaultInfo("SeparatorLine", new AppearanceDefault(Color.Empty, Color.Empty, Color.Silver, Color.Empty)),
			new AppearanceDefaultInfo("ViewCaption", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("CardCaption", new AppearanceDefault(SystemColors.InactiveCaptionText, SystemColors.InactiveCaption, SystemColors.InactiveBorder, Color.Empty, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("FieldCaption", new AppearanceDefault(SystemColors.WindowText, Color.Empty, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("FieldValue", new AppearanceDefault(SystemColors.WindowText, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("FieldEditingValue", new AppearanceDefault(SystemColors.WindowText, Color.Empty, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("FocusedCardCaption", new AppearanceDefault(SystemColors.ActiveCaptionText, SystemColors.ActiveCaption, SystemColors.InactiveCaption, SystemColors.GradientActiveCaption, HorzAlignment.Center, VertAlignment.Center)),			
			new AppearanceDefaultInfo("SelectedCardCaption", new AppearanceDefault(SystemColors.ActiveCaptionText, SystemColors.ActiveCaption, SystemColors.InactiveBorder, Color.Empty, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("HideSelectionCardCaption", new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, SystemColors.InactiveBorder, SystemColors.GradientInactiveCaption, HorzAlignment.Center, VertAlignment.Center)),
			new AppearanceDefaultInfo("Card", new AppearanceDefault(SystemColors.InactiveCaptionText, SystemColors.Window, SystemColors.WindowFrame, Color.Empty, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("FilterPanel", new AppearanceDefault(SystemColors.ControlLightLight, SystemColors.ControlDark, Color.Empty, SystemColors.ControlLight, LinearGradientMode.ForwardDiagonal, HorzAlignment.Near, VertAlignment.Center)),
			new AppearanceDefaultInfo("FilterCloseButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
			new AppearanceDefaultInfo("SelectionFrame", new AppearanceDefault(Color.Black, Color.Blue, Color.White,Color.Empty, LinearGradientMode.Horizontal))
		};
		protected internal virtual bool GetAllowHotTrackCards(LayoutView view) {  return false;  }
	}
	public class LayoutViewWindowsXPPaintStyle : LayoutViewPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = true;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.Flat;
		}
		public override string Name { get { return "WindowsXP"; } }
		public override LayoutViewElementsPainter CreateElementsPainter(BaseView view) { 
			return new LayoutViewWindowsXPElementsPainter(view); 
		}
	}
	public class LayoutViewMixedXPPaintStyle : LayoutViewPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = true;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.Flat;
		}
		public override string Name { get { return "MixedXP"; } }
		public override LayoutViewElementsPainter CreateElementsPainter(BaseView view) {
			return new LayoutViewMixedXPElementsPainter(view);
		}
	}
	public class LayoutViewUltraFlatPaintStyle : LayoutViewPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = false;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.UltraFlat;
		}
		public override string Name { get { return "UltraFlat"; } }
		public override LayoutViewElementsPainter CreateElementsPainter(BaseView view) { return new LayoutViewUltraFlatElementsPainter(view); }
	}
	public class LayoutViewOffice2003PaintStyle : LayoutViewPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = false;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.Office2003;
		}
		public override string Name { get { return "Office2003"; } }
		public override LayoutViewElementsPainter CreateElementsPainter(BaseView view) { return new LayoutViewOffice2003ElementsPainter(view); }
	}
	public class LayoutViewStyle3DPaintStyle : LayoutViewPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			view.ElementsLookAndFeel.UseDefaultLookAndFeel = false;
			view.ElementsLookAndFeel.UseWindowsXPTheme = false;
			view.ElementsLookAndFeel.Style = LookAndFeelStyle.Style3D;
		}
		public override string Name { get { return "Style3D"; } }
		public override LayoutViewElementsPainter CreateElementsPainter(BaseView view) { return new LayoutViewStyle3DElementsPainter(view); }
	}
	public class LayoutViewSkinPaintStyle : LayoutViewPaintStyle {
		public override void UpdateElementsLookAndFeel(BaseView view) {
			var lookAndFeel = view.GridControl == null ? view.ElementsLookAndFeel : view.GridControl.LookAndFeel.ActiveLookAndFeel;
			view.ElementsLookAndFeel.SetSkinStyleCore(lookAndFeel);
		}
		public override bool IsSkin { get { return true; } }
		protected internal override bool GetAllowHotTrackCards(LayoutView view) {
			return GetAllowHotTrackCards(view.ElementsLookAndFeel);
		}
		protected internal static bool GetAllowHotTrackCards(ISkinProvider provider) {
			return GridSkins.GetSkin(provider)[GridSkins.SkinLayoutViewCard] != null;
		}
		public override BorderPainter GetBorderPainter(BaseView view, BorderStyles border) {
			if(border == BorderStyles.NoBorder) return null;
			return new SkinGridBorderPainter(view);
		}
		public override string Name { get { return "Skin"; } }
		public override LayoutViewElementsPainter CreateElementsPainter(BaseView view) {
			return new LayoutViewSkinElementsPainter(view);
		}
		protected internal static AppearanceDefault UpdateAppearance(BaseView view, SkinElement element, AppearanceDefault info) {
			return GridSkinPaintStyle.UpdateAppearance(view, element, info);
		}
		protected internal static AppearanceDefault UpdateAppearance(BaseView view, string elementName, AppearanceDefault info) {
			return GridSkinPaintStyle.UpdateAppearance(view, elementName, info);
		}
		protected AppearanceDefault UpdateNormalAppearanceFore(BaseView view, SkinElement element, AppearanceDefault info) {
			if(element == null) return UpdateSystemColors(view, info);
			element.ApplyForeColorAndFont(info);
			return info;
		}
		protected AppearanceDefault UpdateSelectedAppearanceFore(BaseView view, SkinElement element, AppearanceDefault info) {
			if(element == null) return UpdateSystemColors(view, info);
			element.ApplyForeColorAndFont(info);
			if(element.Properties["SelectedForeColor"] != null) {
				info.ForeColor = element.Properties.GetColor("SelectedForeColor");
			}
			if(element.Properties["SelectedFontBold"] != null) {
				if(element.Properties.GetBoolean("SelectedFontBold")) {
					info.Font = new Font(info.Font == null ? AppearanceObject.DefaultFont : info.Font, FontStyle.Bold);
				}
			}
			return info;
		}
		protected AppearanceDefault UpdateSelectedInactiveAppearanceFore(BaseView view, SkinElement element, AppearanceDefault info) {
			if(element == null) return UpdateSystemColors(view, info);
			element.ApplyForeColorAndFont(info);
			if(element.Properties["SelectedInactiveForeColor"] != null) {
				info.ForeColor = element.Properties.GetColor("SelectedInactiveForeColor");
			}
			if(element.Properties["SelectedInactiveFontBold"] != null) {
				if(element.Properties.GetBoolean("SelectedInactiveFontBold")) {
					info.Font = new Font(info.Font == null ? AppearanceObject.DefaultFont : info.Font, FontStyle.Bold);
				}
			}
			return info;
		}
		protected AppearanceDefault UpdateAppearanceFore(BaseView view, string elementName, AppearanceDefault info) {
			SkinElement element = GridSkins.GetSkin(view)[elementName];
			return UpdateNormalAppearanceFore(view, element, info);
		}
		protected AppearanceDefault UpdateAppearanceFore(BaseView view, SkinElement element, AppearanceDefault info) {
			return UpdateNormalAppearanceFore(view, element, info);
		}
		protected internal static AppearanceDefault UpdateSystemColors(BaseView view, AppearanceDefault info) {
			return GridSkinPaintStyle.UpdateSystemColors(view, info);
		}
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(BaseView view) {
			return new AppearanceDefaultInfo[] { 
				new AppearanceDefaultInfo("ViewBackground", GridSkinPaintStyle.UpdateAppearanceEx(view, GridSkins.SkinCardEmptyArea, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo("HeaderPanel", UpdateAppearanceFore(view, BarSkins.SkinBar, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control))),
				new AppearanceDefaultInfo("SeparatorLine", UpdateAppearance(view, GridSkins.SkinCardSeparator, new AppearanceDefault(SystemColors.ActiveBorder, SystemColors.ActiveBorder, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo("ViewCaption", UpdateAppearanceFore(view, GridSkins.SkinViewCaption, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Center, VertAlignment.Center))),
				new AppearanceDefaultInfo("CardCaption", UpdateNormalAppearanceFore(view, GetCardCaptionSkinElement(view), new AppearanceDefault(SystemColors.InactiveCaptionText, SystemColors.InactiveCaption, SystemColors.InactiveBorder, Color.Empty, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo("FieldCaption", UpdateAppearanceFore(view,  GetCardSkinElement(view), new AppearanceDefault(SystemColors.WindowText, Color.Empty, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo("FieldValue", UpdateAppearanceFore(view, GetCardSkinElement(view), new AppearanceDefault(SystemColors.WindowText, Color.Empty, HorzAlignment.Default, VertAlignment.Center))),
				new AppearanceDefaultInfo("FieldEditingValue", UpdateAppearanceFore(view, GetCardSkinElement(view), new AppearanceDefault(SystemColors.WindowText, Color.Empty, HorzAlignment.Default, VertAlignment.Center))),
				new AppearanceDefaultInfo("FocusedCardCaption", UpdateSelectedAppearanceFore(view, GetCardCaptionSkinElement(view), new AppearanceDefault(SystemColors.ActiveCaptionText, SystemColors.ActiveCaption, SystemColors.InactiveCaption, Color.Empty, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo("SelectedCardCaption", UpdateSelectedAppearanceFore(view, GetCardCaptionSkinElement(view), new AppearanceDefault(SystemColors.ActiveCaptionText, SystemColors.ActiveCaption, SystemColors.InactiveBorder, Color.Empty, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo("HideSelectionCardCaption", UpdateSelectedInactiveAppearanceFore(view, GetCardCaptionSkinElement(view), new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, SystemColors.InactiveBorder, Color.Empty, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo("Card", UpdateAppearance(view, GetCardSkinElement(view), new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, SystemColors.WindowFrame, Color.Empty, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo("FilterPanel", UpdateAppearance(view, GridSkins.SkinGridFilterPanel, new AppearanceDefault(SystemColors.ControlLightLight, SystemColors.ControlDark, Color.Empty, SystemColors.ControlLight, LinearGradientMode.ForwardDiagonal, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo("FilterCloseButton", new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo("SelectionFrame", new AppearanceDefault(Color.Black, Color.Blue, Color.White,Color.Empty, LinearGradientMode.Horizontal))
			};
		}
		SkinElement GetCardCaptionSkinElement(ISkinProvider provider) {
			return GetAllowHotTrackCards(provider) ?
				(GridSkins.GetSkin(provider)[GridSkins.SkinLayoutViewCardCaption] ?? CommonSkins.GetSkin(provider)[CommonSkins.SkinGroupPanelCaptionTop]) :
				CommonSkins.GetSkin(provider)[CommonSkins.SkinGroupPanelCaptionTop];
		}
		SkinElement GetCardSkinElement(ISkinProvider provider) {
			return GetAllowHotTrackCards(provider) ?
				(GridSkins.GetSkin(provider)[GridSkins.SkinLayoutViewCard] ?? CommonSkins.GetSkin(provider)[CommonSkins.SkinGroupPanelTop]) :
				CommonSkins.GetSkin(provider)[CommonSkins.SkinGroupPanelTop];
		}
	}
}
