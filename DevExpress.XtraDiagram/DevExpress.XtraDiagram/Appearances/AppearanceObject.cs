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
using System.ComponentModel;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
namespace DevExpress.XtraDiagram {
	public class DiagramAppearanceObject : AppearanceObject, IFontTraits {
		internal const string OptUseBorderSize = "UseBorderSize";
		internal int borderSize = 1;
		public DiagramAppearanceObject() {
		}
		public DiagramAppearanceObject(AppearanceDefault appearanceDefault)
			: base(appearanceDefault) {
		}
		public DiagramAppearanceObject(AppearanceObject main, AppearanceObject defaultAppearance)
			: base(main, defaultAppearance) {
		}
		public DiagramAppearanceObject(IAppearanceOwner owner, AppearanceObject parentAppearance, string name)
			: base(owner, parentAppearance, name) {
		}
		[DefaultValue(1)]
		public int BorderSize {
			get { return borderSize; }
			set {
				if(BorderSize == value) return;
				borderSize = value;
				if(!IsLoading) {
					try { Options.BeginUpdate(); Options.UseBorderSize = (value != 1); }
					finally { Options.CancelUpdate(); }
				}
				OnPaintChanged();
			}
		}
		protected override AppearanceOptions CreateOptions() {
			return new DiagramAppearanceOptions();
		}
		public new DiagramAppearanceOptions Options { get { return (DiagramAppearanceOptions)base.Options; } }
		public override object Clone() {
			DiagramAppearanceObject obj = new DiagramAppearanceObject();
			obj.Assign(this);
			return obj;
		}
		public override void Assign(AppearanceObject val) {
			base.Assign(val);
			DiagramAppearanceObject other = val as DiagramAppearanceObject;
			if(other != null)
				this.borderSize = other.BorderSize;
		}
		public override bool IsEqual(AppearanceObject val) {
			DiagramAppearanceObject other = val as DiagramAppearanceObject;
			if(other == null)
				return false;
			return base.IsEqual(val) && BorderSize == other.BorderSize;
		}
		protected internal new DiagramAppearanceObject GetAppearanceByOption(string option, AppearanceObject defaultAppearance) {
			return (DiagramAppearanceObject)base.GetAppearanceByOption(option, 0, defaultAppearance);
		}
		protected internal new bool GetOptionState(string option, int level, AppearanceObject defaultAppearance) {
			return base.GetOptionState(option, level, defaultAppearance);
		}
		#region Default
		internal static readonly DiagramAppearanceObject Default = new DiagramAppearanceObject();
		#endregion
		#region IFontTraits
		double IFontTraits.FontSize {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		bool IFontTraits.IsFontBold {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		bool IFontTraits.IsFontItalic {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		bool IFontTraits.IsFontStrikethrough {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		bool IFontTraits.IsFontUnderline {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		bool IFontTraits.AllowEdit { get { return true; } }
		#endregion
	}
	public class DiagramAppearanceOptions : AppearanceOptions {
		bool useBorderSize;
		public DiagramAppearanceOptions() {
			this.useBorderSize = false;
		}
		protected override void ResetOptions() {
			base.ResetOptions();
			this.useBorderSize = false;
		}
		[DefaultValue(false), TypeConverter(typeof(BooleanTypeConverter))]
		public virtual bool UseBorderSize {
			get { return useBorderSize; }
			set {
				if(UseBorderSize == value) return;
				bool prevValue = UseBorderSize;
				useBorderSize = value;
				OnChanged(DiagramAppearanceObject.OptUseBorderSize, prevValue, UseBorderSize);
			}
		}
		public override bool IsEqual(AppearanceOptions options) {
			DiagramAppearanceOptions other = options as DiagramAppearanceOptions;
			if(other == null) return base.IsEqual(options);
			return base.IsEqual(options) && UseBorderSize == other.UseBorderSize;
		}
		protected override bool GetOptionValue(string name) {
			if(IsEqual(name, DiagramAppearanceObject.OptUseBorderSize)) return UseBorderSize;
			return base.GetOptionValue(name);
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				DiagramAppearanceOptions other = options as DiagramAppearanceOptions;
				if(other != null)
					this.useBorderSize = other.UseBorderSize;
			}
			finally {
				EndUpdate();
			}
		}
		protected override bool ShouldSerialize(IComponent owner) {
			return base.ShouldSerialize(owner) && UseBorderSize;
		}
	}
}
