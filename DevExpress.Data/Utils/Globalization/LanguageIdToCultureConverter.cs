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
using System.Globalization;
namespace DevExpress.Utils
{
	public static class LanguageIdToCultureConverter {
		readonly static Dictionary<int, CultureInfo> cultureTable = CreateCultureTable();
#if DXRESTRICTED
		readonly static Dictionary<CultureInfo, int> lcidTable = CreateLcidTable();
#endif
		public static CultureInfo Convert(int lcid) {
			CultureInfo result;
			if (!cultureTable.TryGetValue(lcid, out result))
				return null;
			return result;
		}
		public static int Convert(CultureInfo cultureInfo) {
#if DXRESTRICTED
			int result;
			if (!lcidTable.TryGetValue(cultureInfo, out result))
				return 0;
			return result;
#else
			return cultureInfo.LCID;
#endif
		}
#if DXRESTRICTED
		static Dictionary<CultureInfo,int> CreateLcidTable() {
 			Dictionary<CultureInfo,int> result = new Dictionary<CultureInfo,int>();
			foreach (int key in cultureTable.Keys)
				result[cultureTable[key]] = key;
			return result;
		}
#endif
#if !DXRESTRICTED
		static Dictionary<int, CultureInfo> CreateCultureTable() {
			CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
			Dictionary<int, CultureInfo> result = new Dictionary<int, CultureInfo>();
			foreach (CultureInfo culture in cultures)
				result[culture.LCID] = culture;
			return result;
		}
#else
		static void AddCulture(Dictionary<int, CultureInfo> where, string name, int lcid) {
			try {
				CultureInfo culture = new CultureInfo(name);
				where[lcid] = culture;
			}
			catch {
			}
		}
		static Dictionary<int, CultureInfo> CreateCultureTable() {
			Dictionary<int, CultureInfo> result = new Dictionary<int, CultureInfo>();
			#region Append cultures
			AddCulture(result, "ar-SA", 1025);
			AddCulture(result, "bg-BG", 1026);
			AddCulture(result, "ca-ES", 1027);
			AddCulture(result, "zh-TW", 1028);
			AddCulture(result, "cs-CZ", 1029);
			AddCulture(result, "da-DK", 1030);
			AddCulture(result, "de-DE", 1031);
			AddCulture(result, "el-GR", 1032);
			AddCulture(result, "en-US", 1033);
			AddCulture(result, "fi-FI", 1035);
			AddCulture(result, "fr-FR", 1036);
			AddCulture(result, "he-IL", 1037);
			AddCulture(result, "hu-HU", 1038);
			AddCulture(result, "is-IS", 1039);
			AddCulture(result, "it-IT", 1040);
			AddCulture(result, "ja-JP", 1041);
			AddCulture(result, "ko-KR", 1042);
			AddCulture(result, "nl-NL", 1043);
			AddCulture(result, "nb-NO", 1044);
			AddCulture(result, "pl-PL", 1045);
			AddCulture(result, "pt-BR", 1046);
			AddCulture(result, "rm-CH", 1047);
			AddCulture(result, "ro-RO", 1048);
			AddCulture(result, "ru-RU", 1049);
			AddCulture(result, "hr-HR", 1050);
			AddCulture(result, "sk-SK", 1051);
			AddCulture(result, "sq-AL", 1052);
			AddCulture(result, "sv-SE", 1053);
			AddCulture(result, "th-TH", 1054);
			AddCulture(result, "tr-TR", 1055);
			AddCulture(result, "ur-PK", 1056);
			AddCulture(result, "id-ID", 1057);
			AddCulture(result, "uk-UA", 1058);
			AddCulture(result, "be-BY", 1059);
			AddCulture(result, "sl-SI", 1060);
			AddCulture(result, "et-EE", 1061);
			AddCulture(result, "lv-LV", 1062);
			AddCulture(result, "lt-LT", 1063);
			AddCulture(result, "tg-Cyrl-TJ", 1064);
			AddCulture(result, "fa-IR", 1065);
			AddCulture(result, "vi-VN", 1066);
			AddCulture(result, "hy-AM", 1067);
			AddCulture(result, "az-Latn-AZ", 1068);
			AddCulture(result, "eu-ES", 1069);
			AddCulture(result, "hsb-DE", 1070);
			AddCulture(result, "mk-MK", 1071);
			AddCulture(result, "tn-ZA", 1074);
			AddCulture(result, "xh-ZA", 1076);
			AddCulture(result, "zu-ZA", 1077);
			AddCulture(result, "af-ZA", 1078);
			AddCulture(result, "ka-GE", 1079);
			AddCulture(result, "fo-FO", 1080);
			AddCulture(result, "hi-IN", 1081);
			AddCulture(result, "mt-MT", 1082);
			AddCulture(result, "se-NO", 1083);
			AddCulture(result, "ms-MY", 1086);
			AddCulture(result, "kk-KZ", 1087);
			AddCulture(result, "ky-KG", 1088);
			AddCulture(result, "sw-KE", 1089);
			AddCulture(result, "tk-TM", 1090);
			AddCulture(result, "uz-Latn-UZ", 1091);
			AddCulture(result, "tt-RU", 1092);
			AddCulture(result, "bn-IN", 1093);
			AddCulture(result, "pa-IN", 1094);
			AddCulture(result, "gu-IN", 1095);
			AddCulture(result, "or-IN", 1096);
			AddCulture(result, "ta-IN", 1097);
			AddCulture(result, "te-IN", 1098);
			AddCulture(result, "kn-IN", 1099);
			AddCulture(result, "ml-IN", 1100);
			AddCulture(result, "as-IN", 1101);
			AddCulture(result, "mr-IN", 1102);
			AddCulture(result, "sa-IN", 1103);
			AddCulture(result, "mn-MN", 1104);
			AddCulture(result, "bo-CN", 1105);
			AddCulture(result, "cy-GB", 1106);
			AddCulture(result, "km-KH", 1107);
			AddCulture(result, "lo-LA", 1108);
			AddCulture(result, "gl-ES", 1110);
			AddCulture(result, "kok-IN", 1111);
			AddCulture(result, "syr-SY", 1114);
			AddCulture(result, "si-LK", 1115);
			AddCulture(result, "chr-Cher-US", 1116);
			AddCulture(result, "iu-Cans-CA", 1117);
			AddCulture(result, "am-ET", 1118);
			AddCulture(result, "ne-NP", 1121);
			AddCulture(result, "fy-NL", 1122);
			AddCulture(result, "ps-AF", 1123);
			AddCulture(result, "fil-PH", 1124);
			AddCulture(result, "dv-MV", 1125);
			AddCulture(result, "ha-Latn-NG", 1128);
			AddCulture(result, "yo-NG", 1130);
			AddCulture(result, "quz-BO", 1131);
			AddCulture(result, "nso-ZA", 1132);
			AddCulture(result, "ba-RU", 1133);
			AddCulture(result, "lb-LU", 1134);
			AddCulture(result, "kl-GL", 1135);
			AddCulture(result, "ig-NG", 1136);
			AddCulture(result, "ti-ET", 1139);
			AddCulture(result, "haw-US", 1141);
			AddCulture(result, "ii-CN", 1144);
			AddCulture(result, "arn-CL", 1146);
			AddCulture(result, "moh-CA", 1148);
			AddCulture(result, "br-FR", 1150);
			AddCulture(result, "ug-CN", 1152);
			AddCulture(result, "mi-NZ", 1153);
			AddCulture(result, "oc-FR", 1154);
			AddCulture(result, "co-FR", 1155);
			AddCulture(result, "gsw-FR", 1156);
			AddCulture(result, "sah-RU", 1157);
			AddCulture(result, "qut-GT", 1158);
			AddCulture(result, "rw-RW", 1159);
			AddCulture(result, "wo-SN", 1160);
			AddCulture(result, "prs-AF", 1164);
			AddCulture(result, "gd-GB", 1169);
			AddCulture(result, "ku-Arab-IQ", 1170);
			AddCulture(result, "ar-IQ", 2049);
			AddCulture(result, "ca-ES-valencia", 2051);
			AddCulture(result, "zh-CN", 2052);
			AddCulture(result, "de-CH", 2055);
			AddCulture(result, "en-GB", 2057);
			AddCulture(result, "es-MX", 2058);
			AddCulture(result, "fr-BE", 2060);
			AddCulture(result, "it-CH", 2064);
			AddCulture(result, "nl-BE", 2067);
			AddCulture(result, "nn-NO", 2068);
			AddCulture(result, "pt-PT", 2070);
			AddCulture(result, "sr-Latn-CS", 2074);
			AddCulture(result, "sv-FI", 2077);
			AddCulture(result, "az-Cyrl-AZ", 2092);
			AddCulture(result, "dsb-DE", 2094);
			AddCulture(result, "tn-BW", 2098);
			AddCulture(result, "se-SE", 2107);
			AddCulture(result, "ga-IE", 2108);
			AddCulture(result, "ms-BN", 2110);
			AddCulture(result, "uz-Cyrl-UZ", 2115);
			AddCulture(result, "bn-BD", 2117);
			AddCulture(result, "pa-Arab-PK", 2118);
			AddCulture(result, "ta-LK", 2121);
			AddCulture(result, "mn-Mong-CN", 2128);
			AddCulture(result, "sd-Arab-PK", 2137);
			AddCulture(result, "iu-Latn-CA", 2141);
			AddCulture(result, "tzm-Latn-DZ", 2143);
			AddCulture(result, "ff-Latn-SN", 2151);
			AddCulture(result, "quz-EC", 2155);
			AddCulture(result, "ti-ER", 2163);
			AddCulture(result, "ar-EG", 3073);
			AddCulture(result, "zh-HK", 3076);
			AddCulture(result, "de-AT", 3079);
			AddCulture(result, "en-AU", 3081);
			AddCulture(result, "es-ES", 3082);
			AddCulture(result, "fr-CA", 3084);
			AddCulture(result, "sr-Cyrl-CS", 3098);
			AddCulture(result, "se-FI", 3131);
			AddCulture(result, "quz-PE", 3179);
			AddCulture(result, "ar-LY", 4097);
			AddCulture(result, "zh-SG", 4100);
			AddCulture(result, "de-LU", 4103);
			AddCulture(result, "en-CA", 4105);
			AddCulture(result, "es-GT", 4106);
			AddCulture(result, "fr-CH", 4108);
			AddCulture(result, "hr-BA", 4122);
			AddCulture(result, "smj-NO", 4155);
			AddCulture(result, "tzm-Tfng-MA", 4191);
			AddCulture(result, "ar-DZ", 5121);
			AddCulture(result, "zh-MO", 5124);
			AddCulture(result, "de-LI", 5127);
			AddCulture(result, "en-NZ", 5129);
			AddCulture(result, "es-CR", 5130);
			AddCulture(result, "fr-LU", 5132);
			AddCulture(result, "bs-Latn-BA", 5146);
			AddCulture(result, "smj-SE", 5179);
			AddCulture(result, "ar-MA", 6145);
			AddCulture(result, "en-IE", 6153);
			AddCulture(result, "es-PA", 6154);
			AddCulture(result, "fr-MC", 6156);
			AddCulture(result, "sr-Latn-BA", 6170);
			AddCulture(result, "sma-NO", 6203);
			AddCulture(result, "ar-TN", 7169);
			AddCulture(result, "en-ZA", 7177);
			AddCulture(result, "es-DO", 7178);
			AddCulture(result, "sr-Cyrl-BA", 7194);
			AddCulture(result, "sma-SE", 7227);
			AddCulture(result, "ar-OM", 8193);
			AddCulture(result, "en-JM", 8201);
			AddCulture(result, "es-VE", 8202);
			AddCulture(result, "bs-Cyrl-BA", 8218);
			AddCulture(result, "sms-FI", 8251);
			AddCulture(result, "ar-YE", 9217);
			AddCulture(result, "en-029", 9225);
			AddCulture(result, "es-CO", 9226);
			AddCulture(result, "sr-Latn-RS", 9242);
			AddCulture(result, "smn-FI", 9275);
			AddCulture(result, "ar-SY", 10241);
			AddCulture(result, "en-BZ", 10249);
			AddCulture(result, "es-PE", 10250);
			AddCulture(result, "sr-Cyrl-RS", 10266);
			AddCulture(result, "ar-JO", 11265);
			AddCulture(result, "en-TT", 11273);
			AddCulture(result, "es-AR", 11274);
			AddCulture(result, "sr-Latn-ME", 11290);
			AddCulture(result, "ar-LB", 12289);
			AddCulture(result, "en-ZW", 12297);
			AddCulture(result, "es-EC", 12298);
			AddCulture(result, "sr-Cyrl-ME", 12314);
			AddCulture(result, "ar-KW", 13313);
			AddCulture(result, "en-PH", 13321);
			AddCulture(result, "es-CL", 13322);
			AddCulture(result, "ar-AE", 14337);
			AddCulture(result, "es-UY", 14346);
			AddCulture(result, "ar-BH", 15361);
			AddCulture(result, "es-PY", 15370);
			AddCulture(result, "ar-QA", 16385);
			AddCulture(result, "en-IN", 16393);
			AddCulture(result, "es-BO", 16394);
			AddCulture(result, "en-MY", 17417);
			AddCulture(result, "es-SV", 17418);
			AddCulture(result, "en-SG", 18441);
			AddCulture(result, "es-HN", 18442);
			AddCulture(result, "es-NI", 19466);
			AddCulture(result, "es-PR", 20490);
			AddCulture(result, "es-US", 21514);
			#endregion
			return result;
		}
#endif
	}
}
