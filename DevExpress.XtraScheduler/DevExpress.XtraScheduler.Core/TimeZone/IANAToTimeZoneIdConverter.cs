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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
namespace DevExpress.XtraScheduler.Native{
	public static class IANAToTimeZoneIdConverter{
		private static Dictionary<string, string> zones;
		static IANAToTimeZoneIdConverter() {
			zones = new Dictionary<string, string>();
			InitTimeZones();
		}
		private static void InitTimeZones(){
			zones.Add("Etc/GMT+12", "Dateline Standard Time");
			zones.Add("Etc/GMT+11", "UTC-11");
			zones.Add("Pacific/Pago_Pago", "UTC-11");
			zones.Add("Pacific/Niue", "UTC-11");
			zones.Add("Pacific/Midway", "UTC-11");
			zones.Add("Pacific/Honolulu", "Hawaiian Standard Time");
			zones.Add("Pacific/Rarotonga", "Hawaiian Standard Time");
			zones.Add("Pacific/Tahiti", "Hawaiian Standard Time");
			zones.Add("Pacific/Johnston", "Hawaiian Standard Time");
			zones.Add("Etc/GMT+10", "Hawaiian Standard Time");
			zones.Add("America/Anchorage", "Alaskan Standard Time");
			zones.Add("America/Juneau", "Alaskan Standard Time");
			zones.Add("America/Nome", "Alaskan Standard Time");
			zones.Add("America/Sitka", "Alaskan Standard Time");
			zones.Add("America/Yakutat", "Alaskan Standard Time");
			zones.Add("America/Santa_Isabel", "Pacific Standard Time (Mexico)");
			zones.Add("America/Los_Angeles", "Pacific Standard Time");
			zones.Add("America/Vancouver", "Pacific Standard Time");
			zones.Add("America/Dawson", "Pacific Standard Time");
			zones.Add("America/Whitehorse", "Pacific Standard Time");
			zones.Add("America/Tijuana", "Pacific Standard Time");
			zones.Add("PST8PDT", "Pacific Standard Time");
			zones.Add("America/Phoenix", "US Mountain Standard Time");
			zones.Add("America/Dawson_Creek", "US Mountain Standard Time");
			zones.Add("America/Creston", "US Mountain Standard Time");
			zones.Add("America/Hermosillo", "US Mountain Standard Time");
			zones.Add("Etc/GMT+7", "US Mountain Standard Time");
			zones.Add("America/Chihuahua", "Mountain Standard Time (Mexico)");
			zones.Add("America/Mazatlan", "Mountain Standard Time (Mexico)");
			zones.Add("America/Denver", "Mountain Standard Time");
			zones.Add("America/Edmonton", "Mountain Standard Time");
			zones.Add("America/Cambridge_Bay", "Mountain Standard Time");
			zones.Add("America/Inuvik", "Mountain Standard Time");
			zones.Add("America/Yellowknife", "Mountain Standard Time");
			zones.Add("America/Ojinaga", "Mountain Standard Time");
			zones.Add("America/Boise", "Mountain Standard Time");
			zones.Add("MST7MDT", "Mountain Standard Time");
			zones.Add("America/Guatemala", "Central America Standard Time");
			zones.Add("America/Belize", "Central America Standard Time");
			zones.Add("America/Costa_Rica", "Central America Standard Time");
			zones.Add("Pacific/Galapagos", "Central America Standard Time");
			zones.Add("America/Tegucigalpa", "Central America Standard Time");
			zones.Add("America/Managua", "Central America Standard Time");
			zones.Add("America/El_Salvador", "Central America Standard Time");
			zones.Add("Etc/GMT+6", "Central America Standard Time");
			zones.Add("America/Chicago", "Central Standard Time");
			zones.Add("America/Winnipeg", "Central Standard Time");
			zones.Add("America/Rainy_River", "Central Standard Time");
			zones.Add("America/Rankin_Inlet", "Central Standard Time");
			zones.Add("America/Resolute", "Central Standard Time");
			zones.Add("America/Matamoros", "Central Standard Time");
			zones.Add("America/Indiana/Knox", "Central Standard Time");
			zones.Add("America/Indiana/Tell_City", "Central Standard Time");
			zones.Add("America/Menominee", "Central Standard Time");
			zones.Add("America/North_Dakota/Beulah", "Central Standard Time");
			zones.Add("America/North_Dakota/Center", "Central Standard Time");
			zones.Add("America/North_Dakota/New_Salem", "Central Standard Time");
			zones.Add("CST6CDT", "Central Standard Time");
			zones.Add("America/Mexico_City", "Central Standard Time (Mexico)");
			zones.Add("America/Bahia_Banderas", "Central Standard Time (Mexico)");
			zones.Add("America/Merida", "Central Standard Time (Mexico)");
			zones.Add("America/Monterrey", "Central Standard Time (Mexico)");
			zones.Add("America/Regina", "Canada Central Standard Time");
			zones.Add("America/Swift_Current", "Canada Central Standard Time");
			zones.Add("America/Bogota", "SA Pacific Standard Time");
			zones.Add("America/Rio_Branco", "SA Pacific Standard Time");
			zones.Add("America/Eirunepe", "SA Pacific Standard Time");
			zones.Add("America/Coral_Harbour", "SA Pacific Standard Time");
			zones.Add("America/Guayaquil", "SA Pacific Standard Time");
			zones.Add("America/Jamaica", "SA Pacific Standard Time");
			zones.Add("America/Cayman", "SA Pacific Standard Time");
			zones.Add("America/Panama", "SA Pacific Standard Time");
			zones.Add("America/Lima", "SA Pacific Standard Time");
			zones.Add("Etc/GMT+5", "SA Pacific Standard Time");
			zones.Add("America/Cancun", "Eastern Standard Time (Mexico)");
			zones.Add("America/New_York", "Eastern Standard Time");
			zones.Add("America/Nassau", "Eastern Standard Time");
			zones.Add("America/Toronto", "Eastern Standard Time");
			zones.Add("America/Iqaluit", "Eastern Standard Time");
			zones.Add("America/Montreal", "Eastern Standard Time");
			zones.Add("America/Nipigon", "Eastern Standard Time");
			zones.Add("America/Pangnirtung", "Eastern Standard Time");
			zones.Add("America/Thunder_Bay", "Eastern Standard Time");
			zones.Add("America/Havana", "Eastern Standard Time");
			zones.Add("America/Port-au-Prince", "Eastern Standard Time");
			zones.Add("America/Detroit", "Eastern Standard Time");
			zones.Add("America/Indiana/Petersburg", "Eastern Standard Time");
			zones.Add("America/Indiana/Vincennes", "Eastern Standard Time");
			zones.Add("America/Indiana/Winamac", "Eastern Standard Time");
			zones.Add("America/Kentucky/Monticello", "Eastern Standard Time");
			zones.Add("America/Louisville", "Eastern Standard Time");
			zones.Add("EST5EDT", "Eastern Standard Time");
			zones.Add("America/Indianapolis", "US Eastern Standard Time");
			zones.Add("America/Indiana/Marengo", "US Eastern Standard Time");
			zones.Add("America/Indiana/Vevay", "US Eastern Standard Time");
			zones.Add("America/Caracas", "Venezuela Standard Time");
			zones.Add("America/Asuncion", "Paraguay Standard Time");
			zones.Add("America/Halifax", "Atlantic Standard Time");
			zones.Add("Atlantic/Bermuda", "Atlantic Standard Time");
			zones.Add("America/Glace_Bay", "Atlantic Standard Time");
			zones.Add("America/Goose_Bay", "Atlantic Standard Time");
			zones.Add("America/Moncton", "Atlantic Standard Time");
			zones.Add("America/Thule", "Atlantic Standard Time");
			zones.Add("America/Cuiaba", "Central Brazilian Standard Time");
			zones.Add("America/Campo_Grande", "Central Brazilian Standard Time");
			zones.Add("America/La_Paz", "SA Western Standard Time");
			zones.Add("America/Antigua", "SA Western Standard Time");
			zones.Add("America/Anguilla", "SA Western Standard Time");
			zones.Add("America/Aruba", "SA Western Standard Time");
			zones.Add("America/Barbados", "SA Western Standard Time");
			zones.Add("America/St_Barthelemy", "SA Western Standard Time");
			zones.Add("America/Kralendijk", "SA Western Standard Time");
			zones.Add("America/Manaus", "SA Western Standard Time");
			zones.Add("America/Boa_Vista", "SA Western Standard Time");
			zones.Add("America/Porto_Velho", "SA Western Standard Time");
			zones.Add("America/Blanc-Sablon", "SA Western Standard Time");
			zones.Add("America/Curacao", "SA Western Standard Time");
			zones.Add("America/Dominica", "SA Western Standard Time");
			zones.Add("America/Santo_Domingo", "SA Western Standard Time");
			zones.Add("America/Grenada", "SA Western Standard Time");
			zones.Add("America/Guadeloupe", "SA Western Standard Time");
			zones.Add("America/Guyana", "SA Western Standard Time");
			zones.Add("America/St_Kitts", "SA Western Standard Time");
			zones.Add("America/St_Lucia", "SA Western Standard Time");
			zones.Add("America/Marigot", "SA Western Standard Time");
			zones.Add("America/Martinique", "SA Western Standard Time");
			zones.Add("America/Montserrat", "SA Western Standard Time");
			zones.Add("America/Puerto_Rico", "SA Western Standard Time");
			zones.Add("America/Lower_Princes", "SA Western Standard Time");
			zones.Add("America/Grand_Turk", "SA Western Standard Time");
			zones.Add("America/Port_of_Spain", "SA Western Standard Time");
			zones.Add("America/St_Vincent", "SA Western Standard Time");
			zones.Add("America/Tortola", "SA Western Standard Time");
			zones.Add("America/St_Thomas", "SA Western Standard Time");
			zones.Add("Etc/GMT+4", "SA Western Standard Time");
			zones.Add("America/St_Johns", "Newfoundland Standard Time");
			zones.Add("America/Sao_Paulo", "E. South America Standard Time");
			zones.Add("America/Buenos_Aires", "Argentina Standard Time");
			zones.Add("America/Argentina/La_Rioja", "Argentina Standard Time");
			zones.Add("America/Argentina/Rio_Gallegos", "Argentina Standard Time");
			zones.Add("America/Argentina/Salta", "Argentina Standard Time");
			zones.Add("America/Argentina/San_Juan", "Argentina Standard Time");
			zones.Add("America/Argentina/San_Luis", "Argentina Standard Time");
			zones.Add("America/Argentina/Tucuman", "Argentina Standard Time");
			zones.Add("America/Argentina/Ushuaia", "Argentina Standard Time");
			zones.Add("America/Catamarca", "Argentina Standard Time");
			zones.Add("America/Cordoba", "Argentina Standard Time");
			zones.Add("America/Jujuy", "Argentina Standard Time");
			zones.Add("America/Mendoza", "Argentina Standard Time");
			zones.Add("America/Cayenne", "SA Eastern Standard Time");
			zones.Add("Antarctica/Rothera", "SA Eastern Standard Time");
			zones.Add("America/Fortaleza", "SA Eastern Standard Time");
			zones.Add("America/Araguaina", "SA Eastern Standard Time");
			zones.Add("America/Belem", "SA Eastern Standard Time");
			zones.Add("America/Maceio", "SA Eastern Standard Time");
			zones.Add("America/Recife", "SA Eastern Standard Time");
			zones.Add("America/Santarem", "SA Eastern Standard Time");
			zones.Add("Atlantic/Stanley", "SA Eastern Standard Time");
			zones.Add("America/Paramaribo", "SA Eastern Standard Time");
			zones.Add("Etc/GMT+3", "SA Eastern Standard Time");
			zones.Add("America/Godthab", "Greenland Standard Time");
			zones.Add("America/Montevideo", "Montevideo Standard Time");
			zones.Add("America/Bahia", "Bahia Standard Time");
			zones.Add("America/Santiago", "Pacific SA Standard Time");
			zones.Add("Antarctica/Palmer", "Pacific SA Standard Time");
			zones.Add("Etc/GMT+2", "UTC-02");
			zones.Add("America/Noronha", "UTC-02");
			zones.Add("Atlantic/South_Georgia", "UTC-02");
			zones.Add("Atlantic/Azores", "Azores Standard Time");
			zones.Add("America/Scoresbysund", "Azores Standard Time");
			zones.Add("Atlantic/Cape_Verde", "Cape Verde Standard Time");
			zones.Add("Etc/GMT+1", "Cape Verde Standard Time");
			zones.Add("Africa/Casablanca", "Morocco Standard Time");
			zones.Add("Africa/El_Aaiun", "Morocco Standard Time");
			zones.Add("Etc/GMT", "UTC");
			zones.Add("America/Danmarkshavn", "UTC");
			zones.Add("Europe/London", "GMT Standard Time");
			zones.Add("Atlantic/Canary", "GMT Standard Time");
			zones.Add("Atlantic/Faeroe", "GMT Standard Time");
			zones.Add("Europe/Guernsey", "GMT Standard Time");
			zones.Add("Europe/Dublin", "GMT Standard Time");
			zones.Add("Europe/Isle_of_Man", "GMT Standard Time");
			zones.Add("Europe/Jersey", "GMT Standard Time");
			zones.Add("Europe/Lisbon", "GMT Standard Time");
			zones.Add("Atlantic/Madeira", "GMT Standard Time");
			zones.Add("Atlantic/Reykjavik", "Greenwich Standard Time");
			zones.Add("Africa/Ouagadougou", "Greenwich Standard Time");
			zones.Add("Africa/Abidjan", "Greenwich Standard Time");
			zones.Add("Africa/Accra", "Greenwich Standard Time");
			zones.Add("Africa/Banjul", "Greenwich Standard Time");
			zones.Add("Africa/Conakry", "Greenwich Standard Time");
			zones.Add("Africa/Bissau", "Greenwich Standard Time");
			zones.Add("Africa/Monrovia", "Greenwich Standard Time");
			zones.Add("Africa/Bamako", "Greenwich Standard Time");
			zones.Add("Africa/Nouakchott", "Greenwich Standard Time");
			zones.Add("Atlantic/St_Helena", "Greenwich Standard Time");
			zones.Add("Africa/Freetown", "Greenwich Standard Time");
			zones.Add("Africa/Dakar", "Greenwich Standard Time");
			zones.Add("Africa/Sao_Tome", "Greenwich Standard Time");
			zones.Add("Africa/Lome", "Greenwich Standard Time");
			zones.Add("Europe/Berlin", "W. Europe Standard Time");
			zones.Add("Europe/Andorra", "W. Europe Standard Time");
			zones.Add("Europe/Vienna", "W. Europe Standard Time");
			zones.Add("Europe/Zurich", "W. Europe Standard Time");
			zones.Add("Europe/Busingen", "W. Europe Standard Time");
			zones.Add("Europe/Gibraltar", "W. Europe Standard Time");
			zones.Add("Europe/Rome", "W. Europe Standard Time");
			zones.Add("Europe/Vaduz", "W. Europe Standard Time");
			zones.Add("Europe/Luxembourg", "W. Europe Standard Time");
			zones.Add("Europe/Monaco", "W. Europe Standard Time");
			zones.Add("Europe/Malta", "W. Europe Standard Time");
			zones.Add("Europe/Amsterdam", "W. Europe Standard Time");
			zones.Add("Europe/Oslo", "W. Europe Standard Time");
			zones.Add("Europe/Stockholm", "W. Europe Standard Time");
			zones.Add("Arctic/Longyearbyen", "W. Europe Standard Time");
			zones.Add("Europe/San_Marino", "W. Europe Standard Time");
			zones.Add("Europe/Vatican", "W. Europe Standard Time");
			zones.Add("Europe/Budapest", "Central Europe Standard Time");
			zones.Add("Europe/Tirane", "Central Europe Standard Time");
			zones.Add("Europe/Prague", "Central Europe Standard Time");
			zones.Add("Europe/Podgorica", "Central Europe Standard Time");
			zones.Add("Europe/Belgrade", "Central Europe Standard Time");
			zones.Add("Europe/Ljubljana", "Central Europe Standard Time");
			zones.Add("Europe/Bratislava", "Central Europe Standard Time");
			zones.Add("Europe/Paris", "Romance Standard Time");
			zones.Add("Europe/Brussels", "Romance Standard Time");
			zones.Add("Europe/Copenhagen", "Romance Standard Time");
			zones.Add("Europe/Madrid", "Romance Standard Time");
			zones.Add("Africa/Ceuta", "Romance Standard Time");
			zones.Add("Europe/Warsaw", "Central European Standard Time");
			zones.Add("Europe/Sarajevo", "Central European Standard Time");
			zones.Add("Europe/Zagreb", "Central European Standard Time");
			zones.Add("Europe/Skopje", "Central European Standard Time");
			zones.Add("Africa/Lagos", "W. Central Africa Standard Time");
			zones.Add("Africa/Luanda", "W. Central Africa Standard Time");
			zones.Add("Africa/Porto-Novo", "W. Central Africa Standard Time");
			zones.Add("Africa/Kinshasa", "W. Central Africa Standard Time");
			zones.Add("Africa/Bangui", "W. Central Africa Standard Time");
			zones.Add("Africa/Brazzaville", "W. Central Africa Standard Time");
			zones.Add("Africa/Douala", "W. Central Africa Standard Time");
			zones.Add("Africa/Algiers", "W. Central Africa Standard Time");
			zones.Add("Africa/Libreville", "W. Central Africa Standard Time");
			zones.Add("Africa/Malabo", "W. Central Africa Standard Time");
			zones.Add("Africa/Niamey", "W. Central Africa Standard Time");
			zones.Add("Africa/Ndjamena", "W. Central Africa Standard Time");
			zones.Add("Africa/Tunis", "W. Central Africa Standard Time");
			zones.Add("Etc/GMT-1", "W. Central Africa Standard Time");
			zones.Add("Africa/Windhoek", "Namibia Standard Time");
			zones.Add("Asia/Amman", "Jordan Standard Time");
			zones.Add("Europe/Bucharest", "GTB Standard Time");
			zones.Add("Asia/Nicosia", "GTB Standard Time");
			zones.Add("Europe/Athens", "GTB Standard Time");
			zones.Add("Europe/Chisinau", "GTB Standard Time");
			zones.Add("Asia/Beirut", "Middle East Standard Time");
			zones.Add("Africa/Cairo", "Egypt Standard Time");
			zones.Add("Asia/Damascus", "Syria Standard Time");
			zones.Add("Africa/Johannesburg", "South Africa Standard Time");
			zones.Add("Africa/Bujumbura", "South Africa Standard Time");
			zones.Add("Africa/Gaborone", "South Africa Standard Time");
			zones.Add("Africa/Lubumbashi", "South Africa Standard Time");
			zones.Add("Africa/Maseru", "South Africa Standard Time");
			zones.Add("Africa/Blantyre", "South Africa Standard Time");
			zones.Add("Africa/Maputo", "South Africa Standard Time");
			zones.Add("Africa/Kigali", "South Africa Standard Time");
			zones.Add("Africa/Mbabane", "South Africa Standard Time");
			zones.Add("Africa/Lusaka", "South Africa Standard Time");
			zones.Add("Africa/Harare", "South Africa Standard Time");
			zones.Add("Etc/GMT-2", "South Africa Standard Time");
			zones.Add("Europe/Kiev", "FLE Standard Time");
			zones.Add("Europe/Mariehamn", "FLE Standard Time");
			zones.Add("Europe/Sofia", "FLE Standard Time");
			zones.Add("Europe/Tallinn", "FLE Standard Time");
			zones.Add("Europe/Helsinki", "FLE Standard Time");
			zones.Add("Europe/Vilnius", "FLE Standard Time");
			zones.Add("Europe/Riga", "FLE Standard Time");
			zones.Add("Europe/Uzhgorod", "FLE Standard Time");
			zones.Add("Europe/Zaporozhye", "FLE Standard Time");
			zones.Add("Europe/Istanbul", "Turkey Standard Time");
			zones.Add("Asia/Jerusalem", "Israel Standard Time");
			zones.Add("Europe/Kaliningrad", "Kaliningrad Standard Time");
			zones.Add("Africa/Tripoli", "Libya Standard Time");
			zones.Add("Asia/Baghdad", "Arabic Standard Time");
			zones.Add("Asia/Riyadh", "Arab Standard Time");
			zones.Add("Asia/Bahrain", "Arab Standard Time");
			zones.Add("Asia/Kuwait", "Arab Standard Time");
			zones.Add("Asia/Qatar", "Arab Standard Time");
			zones.Add("Asia/Aden", "Arab Standard Time");
			zones.Add("Europe/Minsk", "Belarus Standard Time");
			zones.Add("Europe/Moscow", "Russian Standard Time");
			zones.Add("Europe/Simferopol", "Russian Standard Time");
			zones.Add("Europe/Volgograd", "Russian Standard Time");
			zones.Add("Africa/Nairobi", "E. Africa Standard Time");
			zones.Add("Antarctica/Syowa", "E. Africa Standard Time");
			zones.Add("Africa/Djibouti", "E. Africa Standard Time");
			zones.Add("Africa/Asmera", "E. Africa Standard Time");
			zones.Add("Africa/Addis_Ababa", "E. Africa Standard Time");
			zones.Add("Indian/Comoro", "E. Africa Standard Time");
			zones.Add("Indian/Antananarivo", "E. Africa Standard Time");
			zones.Add("Africa/Khartoum", "E. Africa Standard Time");
			zones.Add("Africa/Mogadishu", "E. Africa Standard Time");
			zones.Add("Africa/Juba", "E. Africa Standard Time");
			zones.Add("Africa/Dar_es_Salaam", "E. Africa Standard Time");
			zones.Add("Africa/Kampala", "E. Africa Standard Time");
			zones.Add("Indian/Mayotte", "E. Africa Standard Time");
			zones.Add("Etc/GMT-3", "E. Africa Standard Time");
			zones.Add("Asia/Tehran", "Iran Standard Time");
			zones.Add("Asia/Dubai", "Arabian Standard Time");
			zones.Add("Asia/Muscat", "Arabian Standard Time");
			zones.Add("Etc/GMT-4", "Arabian Standard Time");
			zones.Add("Asia/Baku", "Azerbaijan Standard Time");
			zones.Add("Europe/Samara", "Russia Time Zone 3");
			zones.Add("Indian/Mauritius", "Mauritius Standard Time");
			zones.Add("Indian/Reunion", "Mauritius Standard Time");
			zones.Add("Indian/Mahe", "Mauritius Standard Time");
			zones.Add("Asia/Tbilisi", "Georgian Standard Time");
			zones.Add("Asia/Yerevan", "Caucasus Standard Time");
			zones.Add("Asia/Kabul", "Afghanistan Standard Time");
			zones.Add("Asia/Tashkent", "West Asia Standard Time");
			zones.Add("Antarctica/Mawson", "West Asia Standard Time");
			zones.Add("Asia/Oral", "West Asia Standard Time");
			zones.Add("Asia/Aqtau", "West Asia Standard Time");
			zones.Add("Asia/Aqtobe", "West Asia Standard Time");
			zones.Add("Indian/Maldives", "West Asia Standard Time");
			zones.Add("Indian/Kerguelen", "West Asia Standard Time");
			zones.Add("Asia/Dushanbe", "West Asia Standard Time");
			zones.Add("Asia/Ashgabat", "West Asia Standard Time");
			zones.Add("Asia/Samarkand", "West Asia Standard Time");
			zones.Add("Etc/GMT-5", "West Asia Standard Time");
			zones.Add("Asia/Yekaterinburg", "Ekaterinburg Standard Time");
			zones.Add("Asia/Karachi", "Pakistan Standard Time");
			zones.Add("Asia/Calcutta", "India Standard Time");
			zones.Add("Asia/Colombo", "Sri Lanka Standard Time");
			zones.Add("Asia/Katmandu", "Nepal Standard Time");
			zones.Add("Asia/Almaty", "Central Asia Standard Time");
			zones.Add("Antarctica/Vostok", "Central Asia Standard Time");
			zones.Add("Asia/Urumqi", "Central Asia Standard Time");
			zones.Add("Indian/Chagos", "Central Asia Standard Time");
			zones.Add("Asia/Bishkek", "Central Asia Standard Time");
			zones.Add("Asia/Qyzylorda", "Central Asia Standard Time");
			zones.Add("Etc/GMT-6", "Central Asia Standard Time");
			zones.Add("Asia/Dhaka", "Bangladesh Standard Time");
			zones.Add("Asia/Thimphu", "Bangladesh Standard Time");
			zones.Add("Asia/Novosibirsk", "N. Central Asia Standard Time");
			zones.Add("Asia/Omsk", "N. Central Asia Standard Time");
			zones.Add("Asia/Rangoon", "Myanmar Standard Time");
			zones.Add("Indian/Cocos", "Myanmar Standard Time");
			zones.Add("Asia/Bangkok", "SE Asia Standard Time");
			zones.Add("Antarctica/Davis", "SE Asia Standard Time");
			zones.Add("Indian/Christmas", "SE Asia Standard Time");
			zones.Add("Asia/Jakarta", "SE Asia Standard Time");
			zones.Add("Asia/Pontianak", "SE Asia Standard Time");
			zones.Add("Asia/Phnom_Penh", "SE Asia Standard Time");
			zones.Add("Asia/Vientiane", "SE Asia Standard Time");
			zones.Add("Asia/Hovd", "SE Asia Standard Time");
			zones.Add("Asia/Saigon", "SE Asia Standard Time");
			zones.Add("Etc/GMT-7", "SE Asia Standard Time");
			zones.Add("Asia/Krasnoyarsk", "North Asia Standard Time");
			zones.Add("Asia/Novokuznetsk", "North Asia Standard Time");
			zones.Add("Asia/Shanghai", "China Standard Time");
			zones.Add("Asia/Hong_Kong", "China Standard Time");
			zones.Add("Asia/Macau", "China Standard Time");
			zones.Add("Asia/Irkutsk", "North Asia East Standard Time");
			zones.Add("Asia/Chita", "North Asia East Standard Time");
			zones.Add("Asia/Singapore", "Singapore Standard Time");
			zones.Add("Asia/Brunei", "Singapore Standard Time");
			zones.Add("Asia/Makassar", "Singapore Standard Time");
			zones.Add("Asia/Kuala_Lumpur", "Singapore Standard Time");
			zones.Add("Asia/Kuching", "Singapore Standard Time");
			zones.Add("Asia/Manila", "Singapore Standard Time");
			zones.Add("Etc/GMT-8", "Singapore Standard Time");
			zones.Add("Australia/Perth", "W. Australia Standard Time");
			zones.Add("Antarctica/Casey", "W. Australia Standard Time");
			zones.Add("Asia/Taipei", "Taipei Standard Time");
			zones.Add("Asia/Ulaanbaatar", "Ulaanbaatar Standard Time");
			zones.Add("Asia/Choibalsan", "Ulaanbaatar Standard Time");
			zones.Add("Asia/Tokyo", "Tokyo Standard Time");
			zones.Add("Asia/Jayapura", "Tokyo Standard Time");
			zones.Add("Pacific/Palau", "Tokyo Standard Time");
			zones.Add("Asia/Dili", "Tokyo Standard Time");
			zones.Add("Etc/GMT-9", "Tokyo Standard Time");
			zones.Add("Asia/Seoul", "Korea Standard Time");
			zones.Add("Asia/Pyongyang", "Korea Standard Time");
			zones.Add("Asia/Yakutsk", "Yakutsk Standard Time");
			zones.Add("Asia/Khandyga", "Yakutsk Standard Time");
			zones.Add("Australia/Adelaide", "Cen. Australia Standard Time");
			zones.Add("Australia/Broken_Hill", "Cen. Australia Standard Time");
			zones.Add("Australia/Darwin", "AUS Central Standard Time");
			zones.Add("Australia/Brisbane", "E. Australia Standard Time");
			zones.Add("Australia/Lindeman", "E. Australia Standard Time");
			zones.Add("Australia/Sydney", "AUS Eastern Standard Time");
			zones.Add("Australia/Melbourne", "AUS Eastern Standard Time");
			zones.Add("Pacific/Port_Moresby", "West Pacific Standard Time");
			zones.Add("Antarctica/DumontDUrville", "West Pacific Standard Time");
			zones.Add("Pacific/Truk", "West Pacific Standard Time");
			zones.Add("Pacific/Guam", "West Pacific Standard Time");
			zones.Add("Pacific/Saipan", "West Pacific Standard Time");
			zones.Add("Etc/GMT-10", "West Pacific Standard Time");
			zones.Add("Australia/Hobart", "Tasmania Standard Time");
			zones.Add("Australia/Currie", "Tasmania Standard Time");
			zones.Add("Asia/Magadan", "Magadan Standard Time");
			zones.Add("Asia/Vladivostok", "Vladivostok Standard Time");
			zones.Add("Asia/Sakhalin", "Vladivostok Standard Time");
			zones.Add("Asia/Ust-Nera", "Vladivostok Standard Time");
			zones.Add("Asia/Srednekolymsk", "Russia Time Zone 10");
			zones.Add("Pacific/Guadalcanal", "Central Pacific Standard Time");
			zones.Add("Antarctica/Macquarie", "Central Pacific Standard Time");
			zones.Add("Pacific/Ponape", "Central Pacific Standard Time");
			zones.Add("Pacific/Kosrae", "Central Pacific Standard Time");
			zones.Add("Pacific/Noumea", "Central Pacific Standard Time");
			zones.Add("Pacific/Bougainville", "Central Pacific Standard Time");
			zones.Add("Pacific/Efate", "Central Pacific Standard Time");
			zones.Add("Etc/GMT-11", "Central Pacific Standard Time");
			zones.Add("Asia/Kamchatka", "Russia Time Zone 11");
			zones.Add("Asia/Anadyr", "Russia Time Zone 11");
			zones.Add("Pacific/Auckland", "New Zealand Standard Time");
			zones.Add("Antarctica/McMurdo", "New Zealand Standard Time");
			zones.Add("Etc/GMT-12", "UTC+12");
			zones.Add("Pacific/Tarawa", "UTC+12");
			zones.Add("Pacific/Majuro", "UTC+12");
			zones.Add("Pacific/Kwajalein", "UTC+12");
			zones.Add("Pacific/Nauru", "UTC+12");
			zones.Add("Pacific/Funafuti", "UTC+12");
			zones.Add("Pacific/Wake", "UTC+12");
			zones.Add("Pacific/Wallis", "UTC+12");
			zones.Add("Pacific/Fiji", "Fiji Standard Time");
			zones.Add("Pacific/Tongatapu", "Tonga Standard Time");
			zones.Add("Pacific/Enderbury", "Tonga Standard Time");
			zones.Add("Pacific/Fakaofo", "Tonga Standard Time");
			zones.Add("Etc/GMT-13", "Tonga Standard Time");
			zones.Add("Pacific/Apia", "Samoa Standard Time");
			zones.Add("Pacific/Kiritimati", "Line Islands Standard Time");
			zones.Add("Etc/GMT-14", "Line Islands Standard Time");
		}
		public static string GetTimeZone(string timeZone){
			if (zones.ContainsKey(timeZone))
				return zones[timeZone];
			else
				return string.Empty; 
		}
	}
}
