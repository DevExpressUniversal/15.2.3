ASPxClientSpreadsheet.Functions = [
	{
		name: "ABS",
		description: "Grąžina skaičiaus modulį (skaičių be jo ženklo).",
		arguments: [
			{
				name: "skaičius",
				description: "- realusis skaičius, kurio modulis skaičiuojamas"
			}
		]
	},
	{
		name: "ACCRINTM",
		description: "Pateikia sukauptas palūkanas už vertybinius popierius, už kuriuos mokamos palūkanos suėjus mokėjimo terminui.",
		arguments: [
			{
				name: "išleid_data",
				description: "yra vertybinių popierių išleidimo data, išreikšta datų sekos numeriu"
			},
			{
				name: "sudengimas",
				description: "yra vertybinių popierių mokėjimo terminas, išreikštas datų sekos numeriu"
			},
			{
				name: "koeficientas",
				description: "yra vertybinių popierių kasmetinio kupono koeficientas"
			},
			{
				name: "nominalas",
				description: "yra vertybinių popierių nominalioji vertė"
			},
			{
				name: "pagrindas",
				description: "yra naudojamo dienų skaičiavimo pagrindo tipas"
			}
		]
	},
	{
		name: "ACOS",
		description: "Grąžina skaičiaus arkkosinusą radianais, diapazonas nuo 0 iki Pi. Arkkosinusas - tai kampas, kurio kosinusas lygus nurodytam skaičiui.",
		arguments: [
			{
				name: "skaičius",
				description: "- reikalingo kampo kosinusas, kurio reikšmė turi būti nuo -1 iki 1"
			}
		]
	},
	{
		name: "ACOSH",
		description: "Grąžina skaičiaus hiperbolinį areakosinusą.",
		arguments: [
			{
				name: "skaičius",
				description: "- bet koks realusis skaičius, lygus ar didesnis už 1"
			}
		]
	},
	{
		name: "ACOT",
		description: "Pateikia skaičiaus arkotangentą radianais nuo 0 iki Pi.",
		arguments: [
			{
				name: "skaičius",
				description: "yra jūsų norimo kampo kotangentas"
			}
		]
	},
	{
		name: "ACOTH",
		description: "Pateikia skaičiaus atvirkštinį hiperbolinį kontangentą.",
		arguments: [
			{
				name: "skaičius",
				description: "yra jūsų norimo kampo hiperbolinis kotangentas"
			}
		]
	},
	{
		name: "ADDRESS",
		description: "Kuria langelio nuorodą pagal nurodytus eilutės ir stulpelio numerius.",
		arguments: [
			{
				name: "eilutės_num",
				description: "- eilutės numeris naudojimui langelio nuorodoje: pirmai eilutei Eilutės_numeris = 1"
			},
			{
				name: "stulpelio_num",
				description: "- stulpelio numeris naudoti langelio nuorodoje. Pvz., stulpeliui D Stulpelio_numeris = 4"
			},
			{
				name: "abs_num",
				description: "- nurodo nuorodos tipą: absoliuti = 1; absoliuti į eilutę, santykinė į stulpelį = 2; santykinė į eilutę, absoliuti į stulpelį = 3; santykinė = 4"
			},
			{
				name: "a1",
				description: "- loginė reikšmė, rodanti nuorodos stilių: A1 stilius = 1 arba TRUE; R1C1 stilius = 0 arba FALSE"
			},
			{
				name: "lapas",
				description: "tekstas, rodantis darbalapio, naudojamo kaip išorinė nuoroda, vardą"
			}
		]
	},
	{
		name: "AND",
		description: "Tikrina, ar visi argumentai yra TRUE, ir grąžina TRUE, jei taip.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "log_reikšmė1",
				description: "– nuo 1 iki 255 sąlygų, kurias norite patikrinti ir kurios gali būti arba TRUE, arba FALSE; tai gali būti loginės reikšmės, masyvai arba nuorodos"
			},
			{
				name: "log_reikšmė2",
				description: "– nuo 1 iki 255 sąlygų, kurias norite patikrinti ir kurios gali būti arba TRUE, arba FALSE; tai gali būti loginės reikšmės, masyvai arba nuorodos"
			}
		]
	},
	{
		name: "ARABIC",
		description: "Konvertuoja romėniškus skaičius į arabiškus.",
		arguments: [
			{
				name: "tekstas",
				description: "yra norimas konvertuoti romėniškas skaičius"
			}
		]
	},
	{
		name: "AREAS",
		description: "Grąžina nuorodos sričių skaičių. Sritis - tai vientisas langelių diapazonas arba vienas langelis.",
		arguments: [
			{
				name: "nuoroda",
				description: "- nuoroda į langelį arba langelių diapazoną; ji gali būti susieta su keliomis sritimis"
			}
		]
	},
	{
		name: "ASIN",
		description: "Grąžina skaičiaus arksinusą radianais, diapazonas nuo -Pi/2 iki Pi/2.",
		arguments: [
			{
				name: "skaičius",
				description: "- reikalingo kampo sinusas, kurio reikšmė turi būti nuo -1 iki 1"
			}
		]
	},
	{
		name: "ASINH",
		description: "Grąžina skaičiaus hiperbolinį areasinusą.",
		arguments: [
			{
				name: "skaičius",
				description: "- bet koks realusis skaičius, lygus ar didesnis už 1"
			}
		]
	},
	{
		name: "ATAN",
		description: "Grąžina skaičiaus arktangentą, išreikštą radianais, diapazonas nuo -Pi/2 iki Pi/2.",
		arguments: [
			{
				name: "skaičius",
				description: "- kampo, kurį norite gauti, tangentas"
			}
		]
	},
	{
		name: "ATAN2",
		description: "Grąžina nurodytų koordinačių x ir y arktangentą radianais, diapazonas nuo -Pi iki Pi, be -Pi.",
		arguments: [
			{
				name: "x_koord",
				description: "- taško koordinatė x"
			},
			{
				name: "y_koord",
				description: "- taško koordinatė y"
			}
		]
	},
	{
		name: "ATANH",
		description: "Grąžina skaičiaus hiperbolinį areatangentą.",
		arguments: [
			{
				name: "skaičius",
				description: "- bet koks realusis skaičius tarp -1 ir 1, išskyrus -1 ir 1"
			}
		]
	},
	{
		name: "AVEDEV",
		description: "Grąžina duomenų taškų absoliučiųjų nuokrypių nuo vidurkio vidutinę reikšmę. Argumentai gali būti skaičiai arba pavadinimai, masyvai arba nuorodos į skaičius.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "– nuo 1 iki 255 argumentų, kurių absoliučiuosius nuokrypius reikia apskaičiuoti"
			},
			{
				name: "skaičius2",
				description: "– nuo 1 iki 255 argumentų, kurių absoliučiuosius nuokrypius reikia apskaičiuoti"
			}
		]
	},
	{
		name: "AVERAGE",
		description: "Grąžina vidurkį (aritmetinį vidurkį) argumentų, kurie gali būti skaičiai arba vardai, masyvai arba nuorodos, turinčios skaičius.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "– nuo 1 iki 255 skaitinių argumentų, kurių vidurkis skaičiuojamas"
			},
			{
				name: "skaičius2",
				description: "– nuo 1 iki 255 skaitinių argumentų, kurių vidurkis skaičiuojamas"
			}
		]
	},
	{
		name: "AVERAGEA",
		description: "Grąžina savo argumentų (aritmetinį) vidurkį, vertinant tekstą ir FALSE kaip 0; TRUE vertinama kaip 1. Argumentai gali būti skaičiai, vardai, masyvai arba nuorodos.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "reikšmė1",
				description: "– nuo 1 iki 255 argumentų, kurių vidurkį reikia apskaičiuoti"
			},
			{
				name: "reikšmė2",
				description: "– nuo 1 iki 255 argumentų, kurių vidurkį reikia apskaičiuoti"
			}
		]
	},
	{
		name: "AVERAGEIF",
		description: "Randa langelių, kurie nurodyti pagal tam tikrą sąlygą ar kriterijų, vidurkį (aritmetinę reikšmę).",
		arguments: [
			{
				name: "diapazonas",
				description: "yra langelių, kuriuos norite įvertinti, diapazoną"
			},
			{
				name: "kriterijai",
				description: "yra sąlyga arba kriterijus, išreikštas skaičiumi, išraiška ar tekstu, kuris nustato, kurie langeliai bus naudojami vidurkiui rasti"
			},
			{
				name: "vidurkio_diapazonas",
				description: "yra langeliai, kurie bus naudojami vidurkiui rasti. Jei praleista, bus naudojami diapazono langeliai "
			}
		]
	},
	{
		name: "AVERAGEIFS",
		description: "Randa langelių, kuriuos nurodo tam tikras sąlygų arba kriterijų rinkinys, vidurkį (aritmetinę reikšmę).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vidurkio_diapazonas",
				description: "yra langeliai, kurie bus naudojami vidurkiui apskaičiuoti."
			},
			{
				name: "kriterijų_diapazonas",
				description: "yra langelių, kuriuos norite įvertinti pagal tam tikrą sąlygą, diapazonas"
			},
			{
				name: "kriterijai",
				description: "yra sąlyga ar kriterijus, išreikštas skaičiumi, išraiška ar tekstu, nurodančiu, kurie langeliai bus naudojami vidurkiui apskaičiuoti"
			}
		]
	},
	{
		name: "BAHTTEXT",
		description: "Keičia skaičių į tekstą (bahti).",
		arguments: [
			{
				name: "skaičius",
				description: "- numeris, kurį reikia konvertuoti"
			}
		]
	},
	{
		name: "BASE",
		description: "Konvertuoja skaičių į tekstinį atvaizdavimą su skaičiavimo sistemos pagrindu.",
		arguments: [
			{
				name: "skaičius",
				description: "yra norimas konvertuoti skaičius"
			},
			{
				name: "pagrindas",
				description: "yra skaičiavimo sistemos pagrindas, į kurį norite konvertuoti skaičių"
			},
			{
				name: "min_ilgis",
				description: "yra minimalus grąžinamos eilutės ilgis. Jei nepridedami paleisti priekyje esantys nuliai"
			}
		]
	},
	{
		name: "BESSELI",
		description: "Pateikia modifikuotą Beselio funkciją In(x).",
		arguments: [
			{
				name: "x",
				description: "yra reikšmė, kuriai esant vertinama funkcija"
			},
			{
				name: "n",
				description: "yra Beselio funkcijos tvarka"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "Pateikia Beselio funkciją Jn(x).",
		arguments: [
			{
				name: "x",
				description: "yra reikšmė, kuriai esant reikšminama funkcija"
			},
			{
				name: "n",
				description: "yra Beselio funkcijos tvarka"
			}
		]
	},
	{
		name: "BESSELK",
		description: "Pateikia modifikuotą Beselio funkciją Kn(x).",
		arguments: [
			{
				name: "x",
				description: "yra reikšmė, kuriai esant reikšminama funkcija"
			},
			{
				name: "n",
				description: "yra funkcijos tvarka"
			}
		]
	},
	{
		name: "BESSELY",
		description: "Pateikia Beselio funkciją Yn(x).",
		arguments: [
			{
				name: "x",
				description: "yra reikšmė, kuriai esant vertinama funkcija"
			},
			{
				name: "n",
				description: "yra funkcijos tvarka"
			}
		]
	},
	{
		name: "BETA.DIST",
		description: "Grąžina beta tikimybės skirstinio funkciją.",
		arguments: [
			{
				name: "x",
				description: "- reikšmė tarp A ir B, pagal kurią reikia apskaičiuoti funkciją"
			},
			{
				name: "alfa",
				description: "- skirstinio parametras, kuris turi būti didesnis už 0"
			},
			{
				name: "beta",
				description: "- skirstinio parametras, kuris turi būti didesnis už 0"
			},
			{
				name: "integral",
				description: "-loginė reikšmė: integraliajai skirstinio funkcijai naudokite TRUE; tikimybės tankio funkcijai naudokite FALSE"
			},
			{
				name: "A",
				description: " x intervalo pasirinktinis apatinis rėžis. Jei nenurodyta, A = 0"
			},
			{
				name: "B",
				description: "- x intervalo pasirinktinis viršutinis rėžis. Jei nenurodyta, B = 1"
			}
		]
	},
	{
		name: "BETA.INV",
		description: "Grąžina atvirkštinę integraliosios beta tikimybės tankio funkciją (BETA.DIST).",
		arguments: [
			{
				name: "tikimybė",
				description: "- su beta skirstiniu susijusi tikimybė"
			},
			{
				name: "alfa",
				description: "- skirstinio parametras, kuris turi būti didesnis už 0"
			},
			{
				name: "beta",
				description: "- skirstinio parametras, kuris turi būti didesnis už 0"
			},
			{
				name: "A",
				description: "- x intervalo pasirinktinis apatinis rėžis. Jei nenurodyta, A = 0"
			},
			{
				name: "B",
				description: "- x intervalo pasirinktinis viršutinis rėžis. Jei nenurodyta, B = 1"
			}
		]
	},
	{
		name: "BETADIST",
		description: "Grąžina integraliosios beta tikimybės tankio funkciją.",
		arguments: [
			{
				name: "x",
				description: "- reikšmė tarp A ir B, pagal kurią reikia apskaičiuoti funkciją"
			},
			{
				name: "alfa",
				description: "- skirstinio parametras, kuris turi būti didesnis už 0"
			},
			{
				name: "beta",
				description: "- skirstinio parametras, kuris turi būti didesnis už 0"
			},
			{
				name: "A",
				description: "- x intervalo pasirinktinis apatinis rėžis. Jei nenurodyta, A = 0"
			},
			{
				name: "B",
				description: "- x intervalo pasirinktinis viršutinis rėžis. Jei nenurodyta, B = 1"
			}
		]
	},
	{
		name: "BETAINV",
		description: "Grąžina atvirkštinę integraliosios beta tikimybės tankio funkciją (BETADIST).",
		arguments: [
			{
				name: "tikimybė",
				description: "- su beta skirstiniu susijusi tikimybė"
			},
			{
				name: "alfa",
				description: "- skirstinio parametras, kuris turi būti didesnis už 0"
			},
			{
				name: "beta",
				description: "- skirstinio parametras, kuris turi būti didesnis už 0"
			},
			{
				name: "A",
				description: "- x intervalo pasirinktinis apatinis rėžis. Jei nenurodyta, A = 0"
			},
			{
				name: "B",
				description: "- x intervalo pasirinktinis viršutinis rėžis. Jei nenurodyta, B = 1"
			}
		]
	},
	{
		name: "BIN2DEC",
		description: "Dvejetainį skaičių konvertuoja į dešimtainį.",
		arguments: [
			{
				name: "skaičius",
				description: "yra dvejetainis skaičius, kurį norite konvertuoti"
			}
		]
	},
	{
		name: "BIN2HEX",
		description: "Dvejetainį skaičių konvertuoja į šešioliktainį.",
		arguments: [
			{
				name: "skaičiai",
				description: "yra dvejetainis skaičius, kurį norite konvertuoti"
			},
			{
				name: "vietos",
				description: "yra simbolių, kuriuos norite naudoti, skaičius"
			}
		]
	},
	{
		name: "BIN2OCT",
		description: "Dvejetainį skaičių konvertuoja į aštuntainį.",
		arguments: [
			{
				name: "skaičiai",
				description: "yra dvejetainis skaičius, kurį norite konvertuoti"
			},
			{
				name: "vietos",
				description: "yra simbolių, kuriuos norite naudoti, skaičius"
			}
		]
	},
	{
		name: "BINOM.DIST",
		description: "Grąžina atskiro nario binominį skirstinį.",
		arguments: [
			{
				name: "p_į_skaičius",
				description: "- palankių įvykių kiekis bandymuose"
			},
			{
				name: "bandymai",
				description: "- nepriklausomų bandymų kiekis"
			},
			{
				name: "p_į_tikimybė",
				description: "- palankaus įvykio tikimybė kiekviename bandyme"
			},
			{
				name: "integralioji",
				description: "- loginė reikšmė: integraliajai skirstinio funkcijai naudokite TRUE; tikimybės masės funkcijai naudokite FALSE"
			}
		]
	},
	{
		name: "BINOM.DIST.RANGE",
		description: "Pateikia bandomojo rezultato tikimybę, naudojant dvinarį paskirstymą.",
		arguments: [
			{
				name: "bandymai",
				description: "yra nepriklausomų bandymų skaičius"
			},
			{
				name: "tikimybė_s",
				description: "yra kiekvieno bandymo sėkmės tikimybė"
			},
			{
				name: "skaičius_s",
				description: "yra sėkmingų bandymų skaičius"
			},
			{
				name: "skaičius_s2",
				description: "jei pateikiama, ši funkcija pateikia tikimybę, kad sėkmingų bandymų skaičius bus tarp number_s ir number_s2"
			}
		]
	},
	{
		name: "BINOM.INV",
		description: "Grąžina mažiausią reikšmę, kurios integralusis binominis skirstinys yra didesnis ar lygus kriterijaus reikšmei.",
		arguments: [
			{
				name: "bandymai",
				description: "- Bernulio bandymų kiekis"
			},
			{
				name: "p_į_tikimybė",
				description: "- kiekvieno bandymo palankaus įvykio tikimybė, skaičius nuo 0 iki 1 imtinai"
			},
			{
				name: "alfa",
				description: "- kriterijaus reikšmė, skaičius nuo 0 iki 1 imtinai"
			}
		]
	},
	{
		name: "BINOMDIST",
		description: "Grąžina atskiro nario binominį skirstinį.",
		arguments: [
			{
				name: "p_į_skaičius",
				description: "- palankių įvykių kiekis bandymuose"
			},
			{
				name: "bandymai",
				description: "- nepriklausomų bandymų kiekis"
			},
			{
				name: "p_į_tikimybė",
				description: "- palankaus įvykio tikimybė kiekviename bandyme"
			},
			{
				name: "integralioji",
				description: "- loginė reikšmė: integraliajai skirstinio funkcijai naudokite TRUE; tikimybės masės funkcijai naudokite FALSE"
			}
		]
	},
	{
		name: "BITAND",
		description: "Pateikia dviejų skaičių bitų operatorių „Ir“.",
		arguments: [
			{
				name: "number1",
				description: "yra dešimtainis dvejetainio skaičiaus, kurį norite įvertinti, atvaizdavimas"
			},
			{
				name: "number2",
				description: "yra dešimtainis dvejetainio skaičiaus, kurį norite įvertinti, atvaizdavimas"
			}
		]
	},
	{
		name: "BITLSHIFT",
		description: "Pateikia skaičių, kuris buvo perkeltas į kairę per perkėlimo_kiekis bitų.",
		arguments: [
			{
				name: "skaičius",
				description: "yra dešimtainis dvejetainio skaičiaus, kurį norite įvertinti, atvaizdavimas"
			},
			{
				name: "perkėlimo_kiekis",
				description: "yra bitų skaičius, per kurį norite perkelti skaičių į kairę"
			}
		]
	},
	{
		name: "BITOR",
		description: "Pateikia dviejų skaičių bitų operatorių „Arba“.",
		arguments: [
			{
				name: "skaičius1",
				description: "yra dešimtainis dvejetainio skaičiaus, kurį norite įvertinti, atvaizdavimas"
			},
			{
				name: "skaičius2",
				description: "yra dešimtainis dvejetainio skaičiaus, kurį norite įvertinti, atvaizdavimas"
			}
		]
	},
	{
		name: "BITRSHIFT",
		description: "Pateikia skaičių, kuris buvo perkeltas į dešinę per perkėlimo_kiekis bitų.",
		arguments: [
			{
				name: "skaičius",
				description: "yra dešimtainis dvejetainio skaičiaus, kurį norite įvertinti, atvaizdavimas"
			},
			{
				name: "perkėlimo_kiekis",
				description: "yra bitų skaičius, per kurį norite perkelti skaičių į dešinę"
			}
		]
	},
	{
		name: "BITXOR",
		description: "Pateikia dviejų skaičių bitų operatorių „Išskirtinis arba“.",
		arguments: [
			{
				name: "skaičius1",
				description: "yra dešimtainis dvejetainio skaičiaus, kurį norite įvertinti, atvaizdavimas"
			},
			{
				name: "skaičius2",
				description: "yra dešimtainis dvejetainio skaičiaus, kurį norite įvertinti, atvaizdavimas"
			}
		]
	},
	{
		name: "CEILING",
		description: "Suapvalina skaičių iki artimiausio didesnio reikšmingumo kartotinio.",
		arguments: [
			{
				name: "skaičius",
				description: "– reikšmė, kurią norite suapvalinti"
			},
			{
				name: "reikšmingumas",
				description: "– kartotinis, iki kurio norite suapvalinti"
			}
		]
	},
	{
		name: "CEILING.MATH",
		description: "Suapvalina skaičių iki artimiausio sveikojo skaičiaus arba iki artimiausio reikšmingo kartotinio.",
		arguments: [
			{
				name: "skaičius",
				description: "reikšmė, kurią norite suapvalinti"
			},
			{
				name: "reikšmingas",
				description: "tai kartotinis, iki kurio norite suapvalinti"
			},
			{
				name: "režimas",
				description: "jei duota ne nulinė reišmė, ši funkcija suapvalins skaičių iki didesnio skaičiaus"
			}
		]
	},
	{
		name: "CEILING.PRECISE",
		description: "Suapvalina skaičių iki artimiausio didesnio sveiko skaičiaus arba artimiausio didesnio reikšmingumo kartotinio.",
		arguments: [
			{
				name: "skaičius",
				description: "- reikšmė, kurią reikia suapvalinti"
			},
			{
				name: "reikšmingumas",
				description: "- kartotinis, iki kurio reikia suapvalinti"
			}
		]
	},
	{
		name: "CELL",
		description: "Grąžina informaciją apie pirmojo nuorodos langelio formatavimą, vietą ar turinį pagal lapo skaitymo tvarką.",
		arguments: [
			{
				name: "info_tipas",
				description: "– teksto reikšmė, nurodanti, kokią langelio tipo informaciją norite gauti"
			},
			{
				name: "nuoroda",
				description: "yra langelis, apie kurį norite gauti informaciją"
			}
		]
	},
	{
		name: "CHAR",
		description: "Grąžina kodu nurodytą simbolį iš jūsų kompiuterio simbolių rinkinio.",
		arguments: [
			{
				name: "skaičius",
				description: "- skaičius nuo 1 iki 255, kuriuo nurodomas reikalingas simbolis"
			}
		]
	},
	{
		name: "CHIDIST",
		description: "Grąžina chi kvadrato skirstinio dešinės pusės tikimybę.",
		arguments: [
			{
				name: "x",
				description: "- reikšmė, pagal kurią reikia apskaičiuoti skirstinį, ne neigiamas skaičius"
			},
			{
				name: "laisv_laipsnis",
				description: "- laisvės laipsnių kiekis, skaičius tarp 1 ir 10^10, išskyrus 10^10"
			}
		]
	},
	{
		name: "CHIINV",
		description: "Grąžina atvirkštinę chi kvadrato skirstinio dešinės pusės tikimybę.",
		arguments: [
			{
				name: "tikimybė",
				description: "- tikimybė, susijusi su chi kvadrato skirstiniu, skaičius nuo 0 iki 1 imtinai"
			},
			{
				name: "laisv_laipsnis",
				description: "- laisvės laipsnių kiekis, skaičius nuo 1 iki 10^10, išskyrus 10^10"
			}
		]
	},
	{
		name: "CHISQ.DIST",
		description: "Grąžina chi kvadrato skirstinio kairės pusės tikimybę.",
		arguments: [
			{
				name: "x",
				description: "reikšmė, pagal kurią reikia apskaičiuoti skirstinį, ne neigiamas skaičius"
			},
			{
				name: "laisv_laipsnis",
				description: "laisvės laipsnių kiekis, skaičius tarp 1 ir 10^10, išskyrus 10^10"
			},
			{
				name: "integral",
				description: "loginė reikšmė, nurodanti funkcijai grąžinti: integraliąją skirstinio funkciją = TRUE; tikimybės tankio funkciją = FALSE"
			}
		]
	},
	{
		name: "CHISQ.DIST.RT",
		description: "Grąžina chi kvadrato skirstinio dešinės pusės tikimybę.",
		arguments: [
			{
				name: "x",
				description: "reikšmė, pagal kurią reikia apskaičiuoti skirstinį, ne neigiamas skaičius"
			},
			{
				name: "laisv_laipsnis",
				description: "laisvės laipsnių kiekis, skaičius tarp 1 ir 10^10, išskyrus 10^10"
			}
		]
	},
	{
		name: "CHISQ.INV",
		description: "Grąžina atvirkštinę chi kvadrato skirstinio kairės pusės tikimybę.",
		arguments: [
			{
				name: "tikimybė",
				description: "- tikimybė, susijusi su chi kvadrato skirstiniu, skaičius nuo 0 iki 1 imtinai"
			},
			{
				name: "laisv_laipsnis",
				description: "- laisvės laipsnių kiekis, skaičius nuo 1 iki 10^10, išskyrus 10^10"
			}
		]
	},
	{
		name: "CHISQ.INV.RT",
		description: "Grąžina atvirkštinę chi kvadrato skirstinio dešinės pusės tikimybę.",
		arguments: [
			{
				name: "tikimybė",
				description: "- tikimybė, susijusi su chi kvadrato skirstiniu, skaičius nuo 0 iki 1 imtinai"
			},
			{
				name: "laisv_laipsnis",
				description: "- laisvės laipsnių kiekis, skaičius nuo 1 iki 10^10, išskyrus 10^10"
			}
		]
	},
	{
		name: "CHISQ.TEST",
		description: "Grąžina nepriklausomumo požymius: statistikos chi kvadrato skirstinio reikšmę ir atitinkamus laisvės laipsnius.",
		arguments: [
			{
				name: "turimas_diapazonas",
				description: "- duomenų diapazonas su stebiniais, kuriuos reikia patikrinti su laukiamomis reikšmėmis"
			},
			{
				name: "laukiamas_diapazonas",
				description: "- duomenų diapazonas, kuriame pateiktas eilučių ir stulpelių sumų sandaugos santykis su bendra suma"
			}
		]
	},
	{
		name: "CHITEST",
		description: "Grąžina nepriklausomumo požymius: statistikos chi kvadrato skirstinio reikšmę ir atitinkamus laisvės laipsnius.",
		arguments: [
			{
				name: "turimas_diapazonas",
				description: "- duomenų diapazonas su stebiniais, kuriuos reikia patikrinti su laukiamomis reikšmėmis"
			},
			{
				name: "laukiamas_diapazonas",
				description: "- duomenų diapazonas, kuriame pateiktas eilučių ir stulpelių sumų sandaugos santykis su bendra suma"
			}
		]
	},
	{
		name: "CHOOSE",
		description: "Pasirenka pagal indekso numerį iš reikšmių sąrašo reikšmę arba veiksmą, kurį reikia atlikti.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "index_num",
				description: "– index_num turi būti nuo 1 iki 254 arba formulė, arba nuoroda į skaičių nuo 1 iki 254"
			},
			{
				name: "reikšmė1",
				description: "– skaičiai nuo 1 iki 254, langelių nuorodos, apibrėžti vardai, formulės, funkcijos ar teksto argumentai, iš kurių renkasi CHOOSE"
			},
			{
				name: "reikšmė2",
				description: "– skaičiai nuo 1 iki 254, langelių nuorodos, apibrėžti vardai, formulės, funkcijos ar teksto argumentai, iš kurių renkasi CHOOSE"
			}
		]
	},
	{
		name: "CLEAN",
		description: "Pašalina iš teksto visus nespausdintinus simbolius.",
		arguments: [
			{
				name: "tekstas",
				description: "- bet kokia darbalapio informacija, iš kurios reikia pašalinti nespausdintinus simbolius"
			}
		]
	},
	{
		name: "CODE",
		description: "Grąžina pirmo teksto eilutės simbolio skaitmeninį kodą pagal jūsų kompiuterio simbolių rinkinį.",
		arguments: [
			{
				name: "tekstas",
				description: "- tekstas, kurio pirmo simbolio kodas reikalingas"
			}
		]
	},
	{
		name: "COLUMN",
		description: "Grąžina nuorodos stulpelio numerį.",
		arguments: [
			{
				name: "nuoroda",
				description: "- langelis arba vienas diapazonas, kurio stulpelio numerį reikia sužinoti. Jei nėra, grąžinamas stulpelis, kuriame yra funkcija COLUMN"
			}
		]
	},
	{
		name: "COLUMNS",
		description: "Grąžina nuorodos arba masyvo stulpelių skaičių.",
		arguments: [
			{
				name: "masyvas",
				description: "- masyvas, masyvo formulė arba nuoroda į langelių diapazoną, kurio stulpelių skaičių reikia gauti"
			}
		]
	},
	{
		name: "COMBIN",
		description: "Grąžina nurodyto elementų kiekio kombinacijų skaičių.",
		arguments: [
			{
				name: "skaičius",
				description: "- bendras elementų kiekis"
			},
			{
				name: "pasirink_skaičius",
				description: "- elementų kiekis kiekvienoje kombinacijoje"
			}
		]
	},
	{
		name: "COMBINA",
		description: "Pateikia nurodyto elementų skaičiaus derinių su pasikartojimais kiekį.",
		arguments: [
			{
				name: "skaičius",
				description: "yra bendras elementų skaičius"
			},
			{
				name: "pasirinktų_skaičius",
				description: "yra elementų kiekviename derinyje skaičius"
			}
		]
	},
	{
		name: "COMPLEX",
		description: "Realųjį ir menamąjį koeficientus konvertuoja į kompleksinį skaičių.",
		arguments: [
			{
				name: "realusis_skaičius",
				description: "yra kompleksinio skaičiaus realusis koeficientas"
			},
			{
				name: "menam_skaičius",
				description: "yra kompleksinio skaičiaus menamasis koeficientas"
			},
			{
				name: "indeksas",
				description: "yra kompleksinio skaičiaus menamojo komponento indeksas"
			}
		]
	},
	{
		name: "CONCATENATE",
		description: "Sujungia kelias teksto eilutes į vieną.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tekstas1",
				description: "– nuo 1 iki 255 teksto eilučių, kurias reikia sujungti į vieną. Tai gali būti teksto eilutės, skaičiai arba nuorodos į vieną langelį"
			},
			{
				name: "tekstas2",
				description: "– nuo 1 iki 255 teksto eilučių, kurias reikia sujungti į vieną. Tai gali būti teksto eilutės, skaičiai arba nuorodos į vieną langelį"
			}
		]
	},
	{
		name: "CONFIDENCE",
		description: "Grąžina generalinės aibės vidurkio pasikliautinąjį intervalą.",
		arguments: [
			{
				name: "alfa",
				description: "- reikšmingumo lygmuo, taikomas pasikliovimo lygmeniui apskaičiuoti, didesnis už 0 ir mažesnis už 1 skaičius"
			},
			{
				name: "standart_nuokr",
				description: "- duomenų diapazono generalinės aibės standartinis nuokrypis, kuris laikomas iš anksto žinomu. Standart_nuokr turi būti didesnis už 0"
			},
			{
				name: "dydis",
				description: "- imties dydis"
			}
		]
	},
	{
		name: "CONFIDENCE.NORM",
		description: "Grąžina generalinės aibės vidurkio pasikliautinąjį intervalą; naudojamas normalusis skirstinys.",
		arguments: [
			{
				name: "alfa",
				description: "- reikšmingumo lygmuo, taikomas pasikliovimo lygmeniui apskaičiuoti, didesnis už 0 ir mažesnis už 1 skaičius"
			},
			{
				name: "standart_nuokr",
				description: "- duomenų diapazono generalinės aibės standartinis nuokrypis, kuris laikomas iš anksto žinomu. Standart_nuokr turi būti didesnis už 0"
			},
			{
				name: "dydis",
				description: "- imties dydis"
			}
		]
	},
	{
		name: "CONFIDENCE.T",
		description: "Grąžina generalinės aibės vidurkio pasikliautinąjį intervalą, naudojamas Studento t skirstinys.",
		arguments: [
			{
				name: "alfa",
				description: "reikšmingumo lygmuo, taikomas pasikliovimo lygmeniui apskaičiuoti, didesnis už 0 ir mažesnis už 1 skaičius"
			},
			{
				name: "standart_nuokr",
				description: "duomenų diapazono generalinės aibės standartinis nuokrypis, kuris laikomas iš anksto žinomu. Standart_nuokr turi būti didesnis už 0"
			},
			{
				name: "dydis",
				description: "- imties dydis"
			}
		]
	},
	{
		name: "CONVERT",
		description: "Konvertuoja skaičių iš vienos matavimo sistemos į kitą.",
		arguments: [
			{
				name: "skaičius",
				description: "yra konvertuojamo iš_vieneto vertė"
			},
			{
				name: "iš_vieneto",
				description: "yra skaičiaus matavimo vienetai"
			},
			{
				name: "į_vienetą",
				description: "yra rezultato matavimo vienetai"
			}
		]
	},
	{
		name: "CORREL",
		description: "Grąžina dviejų duomenų rinkinių koreliacijos koeficientą.",
		arguments: [
			{
				name: "masyvas1",
				description: "- reikšmių langelių diapazonas. Reikšmės turi būti skaičiai, pavadinimai, masyvai arba nuorodos, kuriose yra skaičių"
			},
			{
				name: "masyvas2",
				description: "- antras reikšmių langelių diapazonas. Reikšmės turi būti skaičiai, pavadinimai, masyvai arba nuorodos, kurios yra skaičių"
			}
		]
	},
	{
		name: "COS",
		description: "Grąžina kampo kosinusą.",
		arguments: [
			{
				name: "skaičius",
				description: "- radianais išreikštas kampas, kurio kosinusas skaičiuojamas"
			}
		]
	},
	{
		name: "COSH",
		description: "Grąžina skaičiaus hiperbolinį kosinusą.",
		arguments: [
			{
				name: "skaičius",
				description: "- bet koks realusis skaičius"
			}
		]
	},
	{
		name: "COT",
		description: "Pateikia kampo kontangentą.",
		arguments: [
			{
				name: "skaičius",
				description: "yra kampas, kurio kotangentą norite apskaičiuoti, radianais"
			}
		]
	},
	{
		name: "COTH",
		description: "Pateikia skaičiaus hiperbolinį kontangentą.",
		arguments: [
			{
				name: "skaičius",
				description: "yra kampas, kurio hiperbolinį kontangentą norite apskaičiuoti, radianais"
			}
		]
	},
	{
		name: "COUNT",
		description: "Skaičiuoja diapazono langelių su skaičiais kiekį.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "reikšmė1",
				description: "– nuo 1 iki 255 argumentų, kuriuose gali būti įvairių skirtingų tipų duomenų arba nuorodų į juos, bet skaičiuojami tik skaičiai"
			},
			{
				name: "reikšmė2",
				description: "– nuo 1 iki 255 argumentų, kuriuose gali būti įvairių skirtingų tipų duomenų arba nuorodų į juos, bet skaičiuojami tik skaičiai"
			}
		]
	},
	{
		name: "COUNTA",
		description: "Skaičiuoja netuščių langelių skaičių diapazone.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "reikšmė1",
				description: "– nuo 1 iki 255 argumentų, nurodančių reikšmes ir langelius, kuriuos reikia suskaičiuoti. Reikšmės gali būti bet kokio tipo informacija"
			},
			{
				name: "reikšmė2",
				description: "– nuo 1 iki 255 argumentų, nurodančių reikšmes ir langelius, kuriuos reikia suskaičiuoti. Reikšmės gali būti bet kokio tipo informacija"
			}
		]
	},
	{
		name: "COUNTBLANK",
		description: "Skaičiuoja tuščius nurodyto langelių diapazono langelius.",
		arguments: [
			{
				name: "diapazonas",
				description: "- diapazonas, kurio tuščius langelius reikia apskaičiuoti"
			}
		]
	},
	{
		name: "COUNTIF",
		description: "Skaičiuoja diapazono langelių, atitinkančių tam tikrą nurodytą sąlygą, skaičių.",
		arguments: [
			{
				name: "diapazonas",
				description: "- langelių diapazonas, kurio netuščius langelius reikia apskaičiuoti"
			},
			{
				name: "kriterijus",
				description: "- sąlyga, išreikšta kaip skaičius, išraiška ar tekstas, kuris nustato, kokie langeliai bus skaičiuojami"
			}
		]
	},
	{
		name: "COUNTIFS",
		description: "Skaičiuoja langelių, kuriuos nurodo tam tikras sąlygų ar kriterijų rinkinys, skaičių.",
		arguments: [
			{
				name: "kriterijų_diapazonas",
				description: "yra langelių, kuriuos norite įvertinti pagal tam tikrą sąlygą, diapazonas"
			},
			{
				name: "kriterijai",
				description: "yra sąlyga, išreikšta skaičiumi, išraiška ar tekstu, kuris nurodo, koks langelis bus apskaičiuojamas"
			}
		]
	},
	{
		name: "COUPDAYBS",
		description: "Pateikia dienų skaičių nuo kupono laikotarpio pradžios iki sudengimo datos.",
		arguments: [
			{
				name: "sudengimas",
				description: "yra vertybinių popierių sudengimo data, išreikšta datų sekos numeriu"
			},
			{
				name: "mok_term",
				description: "yra vertybinių popierių įmokėjimo terminas, išreikštas datų sekos numeriu"
			},
			{
				name: "dažnis",
				description: "yra metinių kupono išmokų skaičius"
			},
			{
				name: "pagrindas",
				description: "yra naudojamo dienų skaičiavimo pagrindo tipas"
			}
		]
	},
	{
		name: "COUPNCD",
		description: "Pateikia kitą kupono datą, esančią po sudengimo datos.",
		arguments: [
			{
				name: "sudengimas",
				description: "yra vertybinių popierių sudengimo data, išreikšta datų sekos numeriu"
			},
			{
				name: "mok_term",
				description: "yra vertybinių popierių mokėjimo terminas, išreikštas datų sekos numeriu"
			},
			{
				name: "dažnis",
				description: "yra metinių kupono išmokų skaičius"
			},
			{
				name: "pagrindas",
				description: "yra naudojamas dienų skaičiavimo pagrindo tipas"
			}
		]
	},
	{
		name: "COUPNUM",
		description: "Pateikia tarp sudengimo datos ir mokėjimo termino esančių mokėtinų kuponų skaičių.",
		arguments: [
			{
				name: "sudengimas",
				description: "yra vertybinių popierių sudengimo data, išreikšta datų sekos numeriu"
			},
			{
				name: "mok_term",
				description: "yra vertybinių popierių mokėjimo terminas, išreikštas datų sekos numeriu"
			},
			{
				name: "dažnis",
				description: "yra metinių kupono išmokų skaičius"
			},
			{
				name: "pagrindas",
				description: "yra naudojamas dienų skaičiavimo pagrindo tipas"
			}
		]
	},
	{
		name: "COUPPCD",
		description: "Pateikia ankstesnę kupono datą, esančią prieš sudengimo datą.",
		arguments: [
			{
				name: "sudengimas",
				description: "yra vertybinių popierių sudengimo data, išreikšta datų sekos numeriu"
			},
			{
				name: "mok_term",
				description: "yra vertybinių popierių mokėjimo terminas, išreikštas datų sekos numeriu"
			},
			{
				name: "dažnis",
				description: "yra metinių kupono išmokų skaičius"
			},
			{
				name: "pagrindas",
				description: "yra naudojamas dienų skaičiavimo pagrindo tipas"
			}
		]
	},
	{
		name: "COVAR",
		description: "Grąžina kovariaciją – dviejų duomenų rinkinių kiekvienos duomenų taškų poros nuokrypių sandaugų vidurkį.",
		arguments: [
			{
				name: "masyvas1",
				description: "- pirmas reikšmių langelių diapazonas. Reikšmės turi būti skaičiai, pavadinimai, masyvai arba nuorodos, kuriose yra skaičių"
			},
			{
				name: "masyvas2",
				description: "- antras reikšmių langelių diapazonas. Reikšmės turi būti skaičiai, pavadinimai, masyvai arba nuorodos, kuriose yra skaičių"
			}
		]
	},
	{
		name: "COVARIANCE.P",
		description: "Grąžina aibės kovariaciją - dviejų duomenų rinkinių kiekvienos duomenų taškų poros nuokrypių sandaugų vidurkį.",
		arguments: [
			{
				name: "masyvas1",
				description: "- pirmas reikšmių langelių diapazonas. Reikšmės turi būti skaičiai, pavadinimai, masyvai arba nuorodos, kuriose yra skaičių"
			},
			{
				name: "masyvas2",
				description: "- antras reikšmių langelių diapazonas. Reikšmės turi būti skaičiai, pavadinimai, masyvai arba nuorodos, kuriose yra skaičių"
			}
		]
	},
	{
		name: "COVARIANCE.S",
		description: "Grąžina imties kovariaciją, dviejų duomenų rinkinių kiekvienos duomenų taškų poros nuokrypių sandaugų vidurkį.",
		arguments: [
			{
				name: "masyvas1",
				description: "pirmas reikšmių langelių diapazonas. Reikšmės turi būti skaičiai, pavadinimai, masyvai arba nuorodos, kuriose yra skaičių"
			},
			{
				name: "masyvas2",
				description: "- antras reikšmių langelių diapazonas. Reikšmės turi būti skaičiai, pavadinimai, masyvai arba nuorodos, kuriose yra skaičių"
			}
		]
	},
	{
		name: "CRITBINOM",
		description: "Grąžina mažiausią reikšmę, kurios integralusis binominis skirstinys yra didesnis ar lygus kriterijaus reikšmei.",
		arguments: [
			{
				name: "bandymai",
				description: "- Bernulio bandymų kiekis"
			},
			{
				name: "p_į_tikimybė",
				description: "- kiekvieno bandymo palankaus įvykio tikimybė, skaičius nuo 0 iki 1 imtinai"
			},
			{
				name: "alfa",
				description: "- kriterijaus reikšmė, skaičius nuo 0 iki 1 imtinai"
			}
		]
	},
	{
		name: "CSC",
		description: "Pateikia kampo kosekantą.",
		arguments: [
			{
				name: "skaičius",
				description: "yra kampas, kurio kosekantą norite apskaičiuoti, radianais"
			}
		]
	},
	{
		name: "CSCH",
		description: "Pateikia kampo hiperbolinį kosekantą.",
		arguments: [
			{
				name: "skaičius",
				description: "yra kampas, kurio hiperbolinį kosekantą norite apskaičiuoti, radianais"
			}
		]
	},
	{
		name: "CUMIPMT",
		description: "Pateikia kaupiamąsias palūkanas, gaunamas tarp dviejų laikotarpių.",
		arguments: [
			{
				name: "koeficientas",
				description: "yra palūkanų norma"
			},
			{
				name: "laik_skaičius",
				description: "yra bendrasis mokėjimo laikotarpių skaičius"
			},
			{
				name: "ev",
				description: "yra dabartinė vertė"
			},
			{
				name: "prad_laikot",
				description: "yra pirmasis skaičiavimo laikotarpis"
			},
			{
				name: "pab_laikot",
				description: "yra paskutinis skaičiavimo laikotarpis"
			},
			{
				name: "tipas",
				description: "yra mokėjimų parinkimai"
			}
		]
	},
	{
		name: "CUMPRINC",
		description: "Pateikia kaupiamasis paskolos procentus tarp dviejų laikotarpių.",
		arguments: [
			{
				name: "koeficientas",
				description: "yra palūkanų norma"
			},
			{
				name: "laik_skaičius",
				description: "yra bendrasis mokėjimo laikotarpių skaičius"
			},
			{
				name: "ev",
				description: "yra dabartinė vertė"
			},
			{
				name: "prad_laikot",
				description: "yra pirmasis skaičiavimo laikotarpis"
			},
			{
				name: "pab_laikot",
				description: "yra paskutinis skaičiavimo laikotarpis"
			},
			{
				name: "tipas",
				description: "yra mokėjimų parinkimai"
			}
		]
	},
	{
		name: "DATE",
		description: "Grąžina skaičių, rodantį datą Spreadsheet datos ir laiko kodu.",
		arguments: [
			{
				name: "metai",
				description: "- skaičius nuo 1900 iki 9999 programoje Spreadsheet for Windows arba nuo 1904 iki 9999 programoje Spreadsheet for Macintosh"
			},
			{
				name: "mėnuo",
				description: "- skaičius nuo 1 iki 12, rodantis metų mėnesį"
			},
			{
				name: "diena",
				description: "- skaičius nuo 1 iki 31, rodantis mėnesio dieną"
			}
		]
	},
	{
		name: "DATEDIF",
		description: "",
		arguments: [
		]
	},
	{
		name: "DATEVALUE",
		description: "Konvertuoja datą, įrašytą teksto formatu, į skaičius Spreadsheet datos laiko kodu.",
		arguments: [
			{
				name: "datos_tekstas",
				description: "- tekstas, nurodantis datą Spreadsheet datos formatu nuo 1900.1.1 (Windows) ar 1904.1.1 (Macintosh) iki 9999.12.31"
			}
		]
	},
	{
		name: "DAVERAGE",
		description: "Skaičiuoja duomenų bazės lauko (stulpelio) įrašų, atitinkančių jūsų nurodytas sąlygas, reikšmių vidurkį.",
		arguments: [
			{
				name: "duom_bazė",
				description: "- langelių, kurie sudaro sąrašą arba duomenų bazę, diapazonas. Duomenų bazė - tai tarpusavyje susijusių duomenų sąrašas"
			},
			{
				name: "laukas",
				description: "stulpelio pavadinimas kabutėse arba skaičius, rodantis stulpelio poziciją sąraše"
			},
			{
				name: "kriterijus",
				description: "langelių diapazonas, kuriame yra jūsų nurodytos sąlygos. Diapazonas apima stulpelio pavadinimą, o vienas žemiau esantis stulpelis skiriamas sąlygai"
			}
		]
	},
	{
		name: "DAY",
		description: "Grąžina mėnesio dieną, skaičių nuo 1 iki 31.",
		arguments: [
			{
				name: "eilės_numeris",
				description: "- skaičius datos-laiko kodo formatu, kurį naudoja programa Spreadsheet"
			}
		]
	},
	{
		name: "DAYS",
		description: "Pateikia dienų tarp dviejų datų skaičių.",
		arguments: [
			{
				name: "pabaigos_data",
				description: "pradžios_data ir pabaigos_data yra tos dvi datos, tarp kurių esančių dienų skaičių norite žinoti"
			},
			{
				name: "pradžios_data",
				description: "pradžios_data ir pabaigos_data yra tos dvi datos, tarp kurių esančių dienų skaičių norite žinoti"
			}
		]
	},
	{
		name: "DAYS360",
		description: "Grąžina dienų skaičių tarp dviejų 360 dienų metų (dvylika mėnesių po 30 dienų) datų.",
		arguments: [
			{
				name: "pradžios_data",
				description: " pradžios_data ir pabaigos_data yra dvi datos, kurių apibrėžiamo laikotarpio dienų skaičių norite sužinoti"
			},
			{
				name: "pabaigos_data",
				description: " pradžios_data ir pabaigos_data yra dvi datos, kurių apibrėžiamo laikotarpio dienų skaičių norite sužinoti"
			},
			{
				name: "metodas",
				description: "- loginė reikšmė, nurodanti skaičiavimo metodą: JAV (NASD) = FALSE arba praleista; Europos = TRUE."
			}
		]
	},
	{
		name: "DB",
		description: "Grąžina turto amortizacijos reikšmę per nurodytą laikotarpį taikant fiksuoto mažėjimo balanso metodą.",
		arguments: [
			{
				name: "vertė",
				description: "- pradinė turto vertė"
			},
			{
				name: "likvidacinė_v",
				description: "- likvidacinė vertė turto eksploatacijos pabaigoje"
			},
			{
				name: "eksploatav",
				description: "- laikotarpių, per kuriuos amortizuojamas turtas, skaičius (kartais vadinama naudingu turto tarnavimu)"
			},
			{
				name: "laikotarpis",
				description: "- laikotarpis, kurio amortizaciją reikia apskaičiuoti. Laikotarpis turi būti išreikštas tais pačiais vienetais kaip ir Eksploatavimo laikotarpis"
			},
			{
				name: "mėn",
				description: "- pirmų metų mėnesių skaičius. Jei nenurodyta, laikoma, kad Mėn = 12"
			}
		]
	},
	{
		name: "DCOUNT",
		description: "Skaičiuoja duomenų bazės lauko (stulpelio) langelius, turinčius skaičius, atitinkančius jūsų nurodytas sąlygas.",
		arguments: [
			{
				name: "duom_bazė",
				description: "- langelių, kurie sudaro sąrašą arba duomenų bazę, diapazonas. Duomenų bazė - tai tarpusavyje susijusių duomenų sąrašas"
			},
			{
				name: "laukas",
				description: "stulpelio pavadinimas kabutėse arba skaičius, rodantis stulpelio poziciją sąraše"
			},
			{
				name: "kriterijus",
				description: "langelių diapazonas, kuriame yra jūsų nurodytos sąlygos. Diapazonas apima stulpelio pavadinimą, o vienas žemiau esantis langelis skiriamas sąlygai"
			}
		]
	},
	{
		name: "DCOUNTA",
		description: "Skaičiuoja duomenų bazės įrašų lauko (stulpelio) netuščius langelius, kurie atitinka jūsų nurodytas sąlygas.",
		arguments: [
			{
				name: "duom_bazė",
				description: "- langelių, kurie sudaro duomenų bazės sąrašą, diapazonas. Duomenų bazė - tai tarpusavy susietų duomenų sąrašas"
			},
			{
				name: "laukas",
				description: "- stulpelio pavadinimas dvigubose kabutėse arba skaičius, nurodantis stulpelio poziciją sąraše"
			},
			{
				name: "kriterijus",
				description: "- langelių, turinčių jūsų nurodytą sąlygą, diapazonas. Diapazone turi būti stulpelio pavadinimas ir vienas langelis žemiau pavadinimo sąlygai"
			}
		]
	},
	{
		name: "DDB",
		description: "Grąžina turto amortizacijos reikšmę per nurodytą laikotarpį, taikant dvigubo mažėjimo balanso ar kokį nors kitą jūsų nurodytą metodą.",
		arguments: [
			{
				name: "vertė",
				description: "- pradinė turto vertė"
			},
			{
				name: "likvidacinė_v",
				description: "- likvidacinė vertė turto eksploatavimo pabaigoje"
			},
			{
				name: "eksploatav",
				description: "- laikotarpių, per kuriuos amortizuojamas turtas, skaičius (kartais vadinama naudingu turto naudojimo laikotarpiu)"
			},
			{
				name: "laikotarpis",
				description: "- laikotarpis, kurio amortizaciją reikia apskaičiuoti. Laikotarpis turi būti išreikštas tais pačiais vienetais kaip ir Eksploatavimo laikotarpis"
			},
			{
				name: "koeficientas",
				description: "- balanso mažėjimo koeficientas. Jei koeficientas nenurodytas, laikoma, kad jis lygus 2 (dvigubo mažėjimo balanso metodas)"
			}
		]
	},
	{
		name: "DEC2BIN",
		description: "Dešimtainį skaičių konvertuoja į dvejetainį.",
		arguments: [
			{
				name: "skaičiai",
				description: "yra dešimtainis sveikasis skaičius, kurį norite konvertuoti"
			},
			{
				name: "vietos",
				description: "yra simbolių, kuriuos norite naudoti, skaičius"
			}
		]
	},
	{
		name: "DEC2HEX",
		description: "Dešimtainį skaičių konvertuoja į šešioliktainį.",
		arguments: [
			{
				name: "skaičiai",
				description: "yra dešimtainis sveikasis skaičius, kurį norite konvertuoti"
			},
			{
				name: "vietos",
				description: "yra simbolių, kuriuos norite naudoti, skaičius"
			}
		]
	},
	{
		name: "DEC2OCT",
		description: "Dešimtainį skaičių konvertuoja į aštuntainį.",
		arguments: [
			{
				name: "skaičiai",
				description: "yra dešimtainis sveikasis skaičius, kurį norite konvertuoti"
			},
			{
				name: "vietos",
				description: "yra simbolių, kuriuos norite naudoti, skaičius"
			}
		]
	},
	{
		name: "DECIMAL",
		description: "Konvertuoja tekstinį pateikto skaičiavimo sistemos pagrindo skaičiaus atvaizdavimą į dešimtainį skaičių.",
		arguments: [
			{
				name: "skaičius",
				description: "yra norimas konvertuoti skaičius"
			},
			{
				name: "pagrindas",
				description: "yra jūsų konvertuojamo skaičiaus pagrindas"
			}
		]
	},
	{
		name: "DEGREES",
		description: "Keičia radianus į laipsnius.",
		arguments: [
			{
				name: "kampas",
				description: "- kampas radianais, kurį reikia pakeisti į laipsnius"
			}
		]
	},
	{
		name: "DELTA",
		description: "Tikrina, ar du skaičiai yra lygūs.",
		arguments: [
			{
				name: "skaičius1",
				description: "yra pirmasis skaičius"
			},
			{
				name: "skaičius2",
				description: "yra antrasis skaičius"
			}
		]
	},
	{
		name: "DEVSQ",
		description: "Grąžina duomenų taškų nuokrypių nuo jų imties vidurkio kvadratų sumą.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "– nuo 1 iki 255 argumentų, masyvas arba masyvo nuoroda, kurių DEVSQ reikia skaičiuoti"
			},
			{
				name: "skaičius2",
				description: "– nuo 1 iki 255 argumentų, masyvas arba masyvo nuoroda, kurių DEVSQ reikia skaičiuoti"
			}
		]
	},
	{
		name: "DGET",
		description: "Gauna iš duomenų bazės vieną įrašą, kuris atitinka jūsų nurodytas sąlygas.",
		arguments: [
			{
				name: "duom_bazė",
				description: "- langelių, iš kurių sudaryta duomenų bazė ar sąrašas, diapazonas. Duomenų bazė - tai tarpusavyje susijusių duomenų sąrašas"
			},
			{
				name: "laukas",
				description: "- stulpelio pavadinimas kabutėse arba skaičius, rodantis stulpelio poziciją sąraše"
			},
			{
				name: "kriterijus",
				description: "- langelių su jūsų nurodytomis sąlygomis diapazonas. Diapazonas apima stulpelio pavadinimą, o vienas žemiau esantis langelis skiriamas sąlygai"
			}
		]
	},
	{
		name: "DISC",
		description: "Pateikia vertybinių popierių nuolaidų koeficientą.",
		arguments: [
			{
				name: "sudengimas",
				description: "yra vertybinių popierių sudengimo data, išreikšta datų sekos numeriu"
			},
			{
				name: "mok_term",
				description: "yra vertybinių popierių mokėjimo terminas, išreikštas datų sekos numeriu"
			},
			{
				name: "kaina",
				description: "yra 100 litų nominaliosios vertės vertybinių popierių kaina"
			},
			{
				name: "padengimas",
				description: "yra vertybinių popierių 100 litų nominaliosios vertės padengimo vertė"
			},
			{
				name: "pagrindas",
				description: "yra naudojamo dienų skaičiavimo pagrindo tipas"
			}
		]
	},
	{
		name: "DMAX",
		description: "Grąžina didžiausią jūsų nurodytas sąlygas atitinkantį skaičių, esantį duomenų bazės lauke (stulpelyje).",
		arguments: [
			{
				name: "duom_bazė",
				description: "- langelių, kurie sudaro sąrašą arba duomenų bazę, diapazonas. Duomenų bazė - tai tarpusavyje susijusių duomenų sąrašas"
			},
			{
				name: "laukas",
				description: "- stulpelio pavadinimas kabutėse arba skaičius, rodantis stulpelio poziciją sąraše"
			},
			{
				name: "kriterijus",
				description: "langelių diapazonas, kuriame yra jūsų nurodytos sąlygos. Diapazonas apima stulpelio pavadinimą, o vienas žemiau esantis langelis skiriamas sąlygai"
			}
		]
	},
	{
		name: "DMIN",
		description: "Grąžina mažiausią jūsų nurodytas sąlygas atitinkantį skaičių, esantį duomenų bazės lauke (stulpelyje).",
		arguments: [
			{
				name: "duom_bazė",
				description: "- langelių, kurie sudaro sąrašą arba duomenų bazę, diapazonas. Duomenų bazė - tai tarpusavyje susijusių duomenų sąrašas"
			},
			{
				name: "laukas",
				description: "- stulpelio pavadinimas kabutėse arba skaičius, rodantis stulpelio poziciją sąraše"
			},
			{
				name: "kriterijus",
				description: "langelių diapazonas, kuriame yra jūsų nurodytos sąlygos. Diapazonas apima stulpelio pavadinimą, o vienas žemiau esantis langelis skiriamas sąlygai"
			}
		]
	},
	{
		name: "DOLLAR",
		description: "Konvertuoja skaičius į tekstą naudojant valiutos formatą.",
		arguments: [
			{
				name: "skaičius",
				description: "- skaičius, nuoroda į langelį, turintį skaičių arba formulė, kurios rezultatas - skaičius"
			},
			{
				name: "dešimtain",
				description: "- skaitmenų po dešimtainio kablelio kiekis. Jei reikia, skaičius suapvalinamas; jei nėra, Dešimtain = 2"
			}
		]
	},
	{
		name: "DOLLARDE",
		description: "Konvertuoja dolerio kainą, išreikštą trupmena, į lito kainą, išreikštą dešimtainiais skaičiais.",
		arguments: [
			{
				name: "trupmeninis_doleris",
				description: "yra skaičius, išreikštas trupmena"
			},
			{
				name: "trupmena",
				description: "yra sveikasis skaičius, kuris bus naudojamas trupmenos vardiklyje"
			}
		]
	},
	{
		name: "DOLLARFR",
		description: "Konvertuoja kainą litais, išreikštą dešimtainiu skaičiumi, į kainą litais, išreikštą trupmena.",
		arguments: [
			{
				name: "dešimtainis_litas",
				description: "yra dešimtainis skaičius"
			},
			{
				name: "trupmena",
				description: "yra sveikasis skaičius, kuris bus naudojamas trupmenos vardiklyje"
			}
		]
	},
	{
		name: "DPRODUCT",
		description: "Sudaugina duomenų bazės įrašų lauko (stulpelio) reikšmes, kurios atitinka jūsų nurodytas sąlygas.",
		arguments: [
			{
				name: "duom_bazė",
				description: "- langelių, kurie sudaro duomenų bazės sąrašą, diapazonas. Duomenų bazė - tai tarpusavy susietų duomenų sąrašas"
			},
			{
				name: "laukas",
				description: "- stulpelio pavadinimas kabutėse arba skaičius, nurodantis stulpelio poziciją sąraše"
			},
			{
				name: "kriterijus",
				description: "- langelių, turinčių jūsų nurodytą sąlygą, diapazonas. Diapazone turi būti stulpelio pavadinimas ir vienas langelis žemiau pavadinimo sąlygai"
			}
		]
	},
	{
		name: "DSTDEV",
		description: "Skaičiuoja pasirinktų duomenų bazės įrašų imties standartinį nuokrypį.",
		arguments: [
			{
				name: "duom_bazė",
				description: "- langelių, kurie sudaro sąrašą arba duomenų bazę, diapazonas. Duomenų bazė - tai tarpusavyje susijusių duomenų sąrašas"
			},
			{
				name: "laukas",
				description: "stulpelio pavadinimas kabutėse arba skaičius, rodantis stulpelio poziciją sąraše"
			},
			{
				name: "kriterijus",
				description: "langelių diapazonas, kuriame yra jūsų nurodytos sąlygos. Diapazonas apima stulpelio pavadinimą, o vienas langelis žemiau skiriamas sąlygai"
			}
		]
	},
	{
		name: "DSTDEVP",
		description: "Skaičiuoja visos pasirinktų duomenų bazės įrašų generalinės aibės standartinį nuokrypį.",
		arguments: [
			{
				name: "duom_bazė",
				description: "- langelių, kurie sudaro sąrašą ar duomenų bazę, diapazonas. Duomenų bazė - tai tarpusavy susijusių duomenų sąrašas"
			},
			{
				name: "laukas",
				description: "- stulpelio pavadinimas dvigubose kabutėse arba skaičius, nurodantis stulpelio poziciją sąraše"
			},
			{
				name: "kriterijus",
				description: "- langelių, turinčių jūsų nurodytą sąlygą, diapazonas. Diapazone turi būti stulpelio pavadinimas ir vienas langelis žemiau pavadinimo sąlygai"
			}
		]
	},
	{
		name: "DSUM",
		description: "Sudeda duomenų bazės lauko (stulpelio) įrašų, atitinkančių jūsų nurodytas sąlygas, skaičius.",
		arguments: [
			{
				name: "duom_bazė",
				description: "- langelių, kurie sudaro sąrašą arba duomenų bazę, diapazonas. Duomenų bazė - tai tarpusavyje susijusių duomenų sąrašas"
			},
			{
				name: "laukas",
				description: "stulpelio pavadinimas kabutėse arba skaičius, rodantis stulpelio poziciją sąraše"
			},
			{
				name: "kriterijus",
				description: "langelių diapazonas, kuriame yra jūsų nurodytos sąlygos. Diapazonas apima stulpelio pavadinimą, o vienas žemiau esantis langelis skiriamas sąlygai"
			}
		]
	},
	{
		name: "DVAR",
		description: "Skaičiuoja pasirinktų duomenų bazės įrašų imties dispersiją.",
		arguments: [
			{
				name: "duom_bazė",
				description: "- langelių, kurie sudaro sąrašą arba duomenų bazę, diapazonas. Duomenų bazė - tai tarpusavyje susijusių duomenų sąrašas"
			},
			{
				name: "laukas",
				description: "stulpelio pavadinimas kabutėse arba skaičius, rodantis stulpelio poziciją sąraše"
			},
			{
				name: "kriterijus",
				description: "langelių diapazonas, kuriame yra jūsų nurodytos sąlygos. Diapazonas apima stulpelio pavadinimą, o vienas langelis žemiau skiriamas sąlygai"
			}
		]
	},
	{
		name: "DVARP",
		description: "Skaičiuoja visos pasirinktų duomenų bazės įrašų generalinės aibės dispersiją.",
		arguments: [
			{
				name: "duom_bazė",
				description: "- langelių, kurie sudaro sąrašą ar duomenų bazę, diapazonas. Duomenų bazė - tai tarpusavy susijusių duomenų sąrašas"
			},
			{
				name: "laukas",
				description: "- stulpelio pavadinimas dvigubose kabutėse arba skaičius, nurodantis stulpelio poziciją sąraše"
			},
			{
				name: "kriterijus",
				description: "- langelių, turinčių jūsų nurodytą sąlygą, diapazonas. Diapazone turi būti stulpelio pavadinimas ir vienas langelis žemiau pavadinimo sąlygai"
			}
		]
	},
	{
		name: "EDATE",
		description: "Pateikia datos, kuri yra nurodytas mėnesių skaičius prieš ar po pradžios datos, sekos numerį.",
		arguments: [
			{
				name: "pradžios_data",
				description: "yra datų sekos numeris, kuris reiškia pradžios datą"
			},
			{
				name: "mėnesiai",
				description: "yra mėnesių prieš arba po pradžios_datą skaičius"
			}
		]
	},
	{
		name: "EFFECT",
		description: "Pateikia faktinį metinių palūkanų koeficientą.",
		arguments: [
			{
				name: "nomin_koef",
				description: "yra nominalinis palūkanų koeficientas"
			},
			{
				name: "laik_skaičius",
				description: "yra sudedamųjų metų laikotarpių skaičius"
			}
		]
	},
	{
		name: "ENCODEURL",
		description: "Pateikia URL koduotą eilutę.",
		arguments: [
			{
				name: "text",
				description: "yra URL koduota eilutė"
			}
		]
	},
	{
		name: "EOMONTH",
		description: "Pateikia mėnesio, esančio prieš arba po nurodyto mėnesių skaičiaus, paskutinės dienos sekos numerį.",
		arguments: [
			{
				name: "pradžios_data",
				description: "yra datų sekos numeris, kuris reiškia pradžios datą"
			},
			{
				name: "mėnesiai",
				description: "yra mėnesių, esančių prieš arba po pradžios_datą skaičius"
			}
		]
	},
	{
		name: "ERF",
		description: "Pateikia klaidos funkciją.",
		arguments: [
			{
				name: "apatinė_riba",
				description: "yra apatinė KFU integravimo riba"
			},
			{
				name: "viršutinė_riba",
				description: "yra viršutinė KFU integravimo riba"
			}
		]
	},
	{
		name: "ERF.PRECISE",
		description: "Pateikia klaidos funkciją.",
		arguments: [
			{
				name: "X",
				description: "yra apatinė ERF.PRECISE integravimo riba"
			}
		]
	},
	{
		name: "ERFC",
		description: "Pateikia papildomos klaidos funkciją.",
		arguments: [
			{
				name: "x",
				description: "yra apatinė KFU integravimo riba"
			}
		]
	},
	{
		name: "ERFC.PRECISE",
		description: "Pateikia papildomos klaidos funkciją.",
		arguments: [
			{
				name: "X",
				description: "yra apatinė ERFC.PRECISE integravimo riba"
			}
		]
	},
	{
		name: "ERROR.TYPE",
		description: "Grąžina skaičių, atitinkantį klaidos reikšmę.",
		arguments: [
			{
				name: "klaidos_reikšmė",
				description: "- klaidos reikšmė, kurią atitinkantis skaičius yra reikalingas; tai gali būti tikroji klaidos reikšmė arba nuoroda į langelį su klaidos reikšme"
			}
		]
	},
	{
		name: "EVEN",
		description: "Suapvalina teigiamą skaičių iki artimiausio didesnio, o neigiamą iki artimiausio mažesnio sveiko skaičiaus.",
		arguments: [
			{
				name: "skaičius",
				description: "- suapvalinama reikšmė"
			}
		]
	},
	{
		name: "EXACT",
		description: "Tikrina, ar dvi teksto eilutės yra vienodos, ir grąžina TRUE arba FALSE. Funkcija EXACT skiria didžiąsias ir mažąsias raides.",
		arguments: [
			{
				name: "tekstas1",
				description: "- pirma teksto eilutė"
			},
			{
				name: "tekstas2",
				description: "- antra teksto eilutė"
			}
		]
	},
	{
		name: "EXP",
		description: "Grąžina e, pakeltą nurodytu laipsniu.",
		arguments: [
			{
				name: "skaičius",
				description: "- skaičiaus e laipsnio rodiklis. Konstanta e lygi 2,71828182845904, ji yra natūraliojo logaritmo pagrindas"
			}
		]
	},
	{
		name: "EXPON.DIST",
		description: "Grąžina eksponentinį pasiskirstymą.",
		arguments: [
			{
				name: "x",
				description: "- funkcijos reikšmė, ne neigiamas skaičius"
			},
			{
				name: "liambda",
				description: "- parametro reikšmė, teigiamas skaičius"
			},
			{
				name: "integral",
				description: "- loginė reikšmė, nurodanti funkcijai grąžinti: integraliąją skirstinio funkciją = TRUE; tikimybės tankio funkciją = FALSE"
			}
		]
	},
	{
		name: "EXPONDIST",
		description: "Grąžina eksponentinį pasiskirstymą.",
		arguments: [
			{
				name: "x",
				description: "- funkcijos reikšmė, ne neigiamas skaičius"
			},
			{
				name: "liambda",
				description: "- parametro reikšmė, teigiamas skaičius"
			},
			{
				name: "integral",
				description: "- loginė reikšmė, nurodanti funkcijai grąžinti: integraliąją skirstinio funkciją = TRUE; tikimybės tankio funkciją = FALSE"
			}
		]
	},
	{
		name: "F.DIST",
		description: "Grąžina dviejų rinkinių F tikimybės (kairės pusės) skirstinį (įvairovės laipsnį).",
		arguments: [
			{
				name: "x",
				description: "reikšmė, pagal kurią skaičiuojama funkcija, ne neigiamas skaičius"
			},
			{
				name: "laisvės_laipsnis1",
				description: "skaitiklio laisvės laipsniai, skaičius nuo 1 iki 10^10, išskyrus 10^10"
			},
			{
				name: "laisvės_laipsnis2",
				description: "- vardiklio laisvės laipsniai, skaičius nuo 1 iki 10^10, išskyrus 10^10"
			},
			{
				name: "integral",
				description: "loginė reikšmė, nurodanti funkcijai grąžinti: integraliąją skirstinio funkciją = TRUE; tikimybės tankio funkciją = FALSE"
			}
		]
	},
	{
		name: "F.DIST.RT",
		description: "Grąžina dviejų rinkinių F tikimybės (dešinės pusės) skirstinį (įvairovės laipsnį).",
		arguments: [
			{
				name: "x",
				description: "reikšmė, pagal kurią skaičiuojama funkcija, ne neigiamas skaičius"
			},
			{
				name: "laisvės_laipsnis1",
				description: "skaitiklio laisvės laipsniai, skaičius nuo 1 iki 10^10, išskyrus 10^10"
			},
			{
				name: "laisvės_laipsnis2",
				description: "- vardiklio laisvės laipsniai, skaičius nuo 1 iki 10^10, išskyrus 10^10"
			}
		]
	},
	{
		name: "F.INV",
		description: "Grąžina dviejų rinkinių F tikimybės (kairės pusės) skirstinį (įvairovės laipsnį): jei p = F.DIST(x,...), tada F.INV(p,...) = x.",
		arguments: [
			{
				name: "tikimybė",
				description: "tikimybė, susijusi su F integraliuoju skirstiniu, skaičius nuo 0 iki 1 imtinai"
			},
			{
				name: "laisvės_laipsnis1",
				description: "- skaitiklio laisvės laipsniai, skaičius nuo 1 iki 10^10, išskyrus 10^10"
			},
			{
				name: "laisvės_laipsnis2",
				description: "- vardiklio laisvės laipsniai, skaičius nuo 1 iki 10^10, išskyrus 10^10"
			}
		]
	},
	{
		name: "F.INV.RT",
		description: "Grąžina atvirkštinį F tikimybės skirstinį (dešinės pusės): jei p = F.DIST.RT(x,...), tada F.INV.RT(p,...) = x.",
		arguments: [
			{
				name: "tikimybė",
				description: "tikimybė, susijusi su F integraliuoju skirstiniu, skaičius nuo 0 iki 1 imtinai"
			},
			{
				name: "laisvės_laipsnis1",
				description: "- skaitiklio laisvės laipsniai, skaičius nuo 1 iki 10^10, išskyrus 10^10"
			},
			{
				name: "laisvės_laipsnis2",
				description: "- vardiklio laisvės laipsniai, skaičius nuo 1 iki 10^10, išskyrus 10^10"
			}
		]
	},
	{
		name: "F.TEST",
		description: "Grąžina F-testo rezultatą – dvipusė tikimybė, kad Masyvas1 ir Masyvas2 dispersijos skiriasi nežymiai.",
		arguments: [
			{
				name: "masyvas1",
				description: "– pirmas duomenų masyvas ar diapazonas, tai gali būti skaičiai arba pavadinimai, masyvai arba nuorodos, kuriose yra skaičių (tuščių langelių nepaisoma)"
			},
			{
				name: "masyvas2",
				description: "– antras duomenų masyvas ar diapazonas, tai gali būti skaičiai arba pavadinimai, masyvai arba nuorodos, kuriose yra skaičių (tuščių langelių nepaisoma)"
			}
		]
	},
	{
		name: "FACT",
		description: "Grąžina skaičiaus faktorialą, kuris lygus 1*2*3*...* skaičius.",
		arguments: [
			{
				name: "skaičius",
				description: "- ne neigiamas skaičius, kurio faktorialą reikia apskaičiuoti"
			}
		]
	},
	{
		name: "FACTDOUBLE",
		description: "Pateikia skaičiaus dvigubą faktorialą.",
		arguments: [
			{
				name: "skaičius",
				description: "yra reikšmė, kuriai pateikiamas dvigubas faktorialas"
			}
		]
	},
	{
		name: "FALSE",
		description: "Grąžina loginę reikšmę FALSE.",
		arguments: [
		]
	},
	{
		name: "FDIST",
		description: "Grąžina dviejų rinkinių F tikimybės (dešinės pusės) skirstinį (įvairovės laipsnį).",
		arguments: [
			{
				name: "x",
				description: "- reikšmė, pagal kurią skaičiuojama funkcija, ne neigiamas skaičius"
			},
			{
				name: "laisv_laips1",
				description: "- skaitiklio laisvės laipsniai, skaičius nuo 1 iki 10^10, išskyrus 10^10"
			},
			{
				name: "laisv_laips2",
				description: "- vardiklio laisvės laipsniai, skaičius nuo 1 iki 10^10, išskyrus 10^10"
			}
		]
	},
	{
		name: "FIELD",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "FIELDPICTURE",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "FIND",
		description: "Grąžina pradinę vienos teksto eilutės poziciją kitoje teksto eilutėje. FIND skiria raidžių registrą.",
		arguments: [
			{
				name: "ieškomas_tekstas",
				description: "- ieškomas tekstas. Norėdami rasti pirmąjį Ieškos_teksto simbolio atitikimą naudokite dvejas kabutes (tuščią tekstą); universalijos negalimos"
			},
			{
				name: "ieškos_tekstas",
				description: "- tekstas, kuriame yra ieškomas fragmentas"
			},
			{
				name: "prad_num",
				description: "- nurodo simbolį, nuo kurio reikia pradėti paiešką. Pirmas Ieškos teksto simbolis pažymėtas 1. Jei nenurodyta, Prad_num = 1"
			}
		]
	},
	{
		name: "FINV",
		description: "Grąžina atvirkštinį dešinės pusės F tikimybės skirstinį: jei p = FDIST(x,...), tada FINV(p,...) = x.",
		arguments: [
			{
				name: "tikimybė",
				description: "- tikimybė, susijusi su F integraliniu skirstiniu, skaičius nuo 0 iki 1 imtinai"
			},
			{
				name: "laisv_laips1",
				description: "- skaitiklio laisvės laipsniai, skaičius nuo 1 iki 10^10, išskyrus 10^10"
			},
			{
				name: "laisv_laips2",
				description: "- vardiklio laisvės laipsniai, skaičius nuo 1 iki 10^10, išskyrus 10^10"
			}
		]
	},
	{
		name: "FISHER",
		description: "Grąžina Fišerio transformaciją.",
		arguments: [
			{
				name: "x",
				description: "- reikšmė, kurios transformaciją reikia gauti, skaičius nuo -1 iki 1, išskyrus -1 ir 1"
			}
		]
	},
	{
		name: "FISHERINV",
		description: "Grąžina atvirkštinę Fišerio transformaciją: jei y = FISHER(x), tada FISHERINV(y) = x.",
		arguments: [
			{
				name: "y",
				description: "- reikšmė, kurios atvirkštinę transformaciją reikia gauti"
			}
		]
	},
	{
		name: "FIXED",
		description: "Suapvalina skaičių iki nurodyto skaitmenų po kablelio kiekio ir grąžina rezultatą kaip tekstą su kableliais ar be jų.",
		arguments: [
			{
				name: "skaičius",
				description: "- skaičius, kurį norite suapvalinti ir konvertuoti į tekstą"
			},
			{
				name: "dešimtainiai",
				description: "- dešimtainių skaitmenų po kablelio kiekis. Jei nėra, Dešimtainiai = 2"
			},
			{
				name: "be_kablelių",
				description: "- loginė reikšmė: nerodyti kablelių grąžinamame tekste = TRUE; rodyti kablelius grąžinamame tekste = FALSE arba nieko"
			}
		]
	},
	{
		name: "FLOOR",
		description: "Suapvalina reikšmę iki artimiausio mažesnio reikšmingumo kartotinio.",
		arguments: [
			{
				name: "skaičius",
				description: "– skaitinė reikšmė, kurią norite suapvalinti"
			},
			{
				name: "reikšmingumas",
				description: "– kartotinis, iki kurio norite suapvalinti. Ir skaičius, ir reikšmingumas turi būti arba abu teigiami, arba abu neigiami"
			}
		]
	},
	{
		name: "FLOOR.MATH",
		description: "Apvalina skaičių iki artimiausio mažesnio sveikojo skaičiaus arba artimiausio mažesnio reikšmingumo kartotinio.",
		arguments: [
			{
				name: "skaičius",
				description: "yra reikšmė, iki kurios norite apvalinti"
			},
			{
				name: "reikšmingumas",
				description: "yra kartotinis, iki kurio norite apvalinti"
			},
			{
				name: "būdas",
				description: "jei funkcijai bus pateiktas ne nulis, ji suapvalins link nulio"
			}
		]
	},
	{
		name: "FLOOR.PRECISE",
		description: "Suapvalina reikšmę iki artimiausio sveikojo skaičiaus arba artimiausio mažesnio reikšmingumo kartotinio.",
		arguments: [
			{
				name: "skaičius",
				description: "- skaitinė reikšmė, kurią reikia suapvalinti"
			},
			{
				name: "reikšmingumas",
				description: "- kartotinis, iki kurio reikia suapvalinti. "
			}
		]
	},
	{
		name: "FORECAST",
		description: "Skaičiuoja arba prognozuoja būsimą reikšmę pagal krypties liniją naudojant turimas reikšmes.",
		arguments: [
			{
				name: "x",
				description: "- duomenų taškas, kurio reikšmę reikia prognozuoti; jis turi būti skaitinė reikšmė"
			},
			{
				name: "žinomi_y",
				description: "- priklausomas skaitinių duomenų masyvas arba diapazonas"
			},
			{
				name: "žinomi_x",
				description: "- nepriklausomas skaitinių duomenų masyvas arba diapazonas. Žinomų_x dispersija turi būti nulis"
			}
		]
	},
	{
		name: "FORMULATEXT",
		description: "Formulę pateikia kaip eilutę.",
		arguments: [
			{
				name: "nuoroda",
				description: "yra formulės nuoroda"
			}
		]
	},
	{
		name: "FREQUENCY",
		description: "Skaičiuoja, kiek kartų tam tikra reikšmė pasitaiko reikšmių diapazone, ir grąžina vertikalų skaičių masyvą, kuriame yra vienu skaičiumi daugiau negu Dvejet_masyvas.",
		arguments: [
			{
				name: "duom_masyvas",
				description: "- reikšmių masyvas arba nuoroda į reikšmių, kurių dažnį reikia apskaičiuoti, rinkinį (tarpų ir tekstų nepaisoma)"
			},
			{
				name: "dvejet_masyvas",
				description: "- intervalų masyvas arba nuoroda į intervalus, į kuriuos reikia sugrupuoti reikšmes iš Duom_masyvo"
			}
		]
	},
	{
		name: "FTEST",
		description: "Grąžina F-testo rezultatą – dvipusė tikimybė, kad Masyvas1 ir Masyvas2 dispersijos skiriasi nežymiai.",
		arguments: [
			{
				name: "masyvas1",
				description: "– pirmas duomenų masyvas ar diapazonas, tai gali būti skaičiai arba pavadinimai, masyvai arba nuorodos, kuriose yra skaičių (tuščių langelių nepaisoma)"
			},
			{
				name: "masyvas2",
				description: "– antras duomenų masyvas ar diapazonas, tai gali būti skaičiai arba pavadinimai, masyvai arba nuorodos, kuriose yra skaičių (tuščių langelių nepaisoma)"
			}
		]
	},
	{
		name: "FV",
		description: "Grąžina būsimąją investicijos reikšmę pagal periodinius, pastovius mokėjimus ir pastovaus dydžio palūkanas.",
		arguments: [
			{
				name: "norma",
				description: "- laikotarpio palūkanų norma. Pavyzdžiui, naudokite 6%/4, jei kas ketvirtį mokama po 6% MPN"
			},
			{
				name: "laikot_sk",
				description: "- bendras investavimo mokėjimų laikotarpių skaičius"
			},
			{
				name: "išl",
				description: "- mokėjimas per kiekvieną laikotarpį, kuris nesikeičia per visą investavimo laiką"
			},
			{
				name: "dr",
				description: "- dabartinė reikšmė arba bendroji suma, kurios dabar vertos būsimų mokėjimų sekos. Jei šio argumento nėra, Dr = 0"
			},
			{
				name: "tipas",
				description: "- reikšmė, rodanti mokėjimų laiką: mokėjimas laikotarpio pradžioje = 1; mokėjimas laikotarpio pabaigoje = 0 arba argumento nėra"
			}
		]
	},
	{
		name: "FVSCHEDULE",
		description: "Pateikia pradinės pagrindinės sumos, pritaikius sudėtinių palūkanų koeficientą, būsimąją reikšmę.",
		arguments: [
			{
				name: "pagrindinė",
				description: "yra dabartinė reikšmė"
			},
			{
				name: "tvarkaraštis",
				description: "yra taikomų palūkanų koeficiento masyvas"
			}
		]
	},
	{
		name: "GAMMA",
		description: "Pateikia gama funkcijos reikšmę.",
		arguments: [
			{
				name: "x",
				description: "yra reikšmę, kuriai norite apskaičiuoti gama"
			}
		]
	},
	{
		name: "GAMMA.DIST",
		description: "Grąžina gama skirstinį.",
		arguments: [
			{
				name: "x",
				description: "reikšmė, pagal kurią reikia apskaičiuoti skirstinį, ne neigiamas skaičius"
			},
			{
				name: "alfa",
				description: "skirstinio parametras, teigiamas skaičius"
			},
			{
				name: "beta",
				description: "- skirstinio parametras, teigiamas skaičius. Jei beta = 1, GAMMA.DIST grąžina standartinį gama skirstinį"
			},
			{
				name: "integral",
				description: "- loginė reikšmė: grąžina integraliąją skirstinio funkciją = TRUE; grąžina tikimybės masės funkciją = FALSE arba nenurodoma"
			}
		]
	},
	{
		name: "GAMMA.INV",
		description: "Grąžina atvirkštinį gama integralųjį skirstinį: jei p = GAMMA.DIST(x,...), tada GAMMA.INV(p,...) = x.",
		arguments: [
			{
				name: "tikimybė",
				description: "- tikimybė, susijusi su gama skirstiniu, skaičius tarp 0 ir 1 imtinai"
			},
			{
				name: "alfa",
				description: "- skirstinio parametras, teigiamas skaičius"
			},
			{
				name: "beta",
				description: "- skirstinio parametras, teigiamas skaičius. Jei beta = 1, GAMMA.INV grąžina atvirkštinį standartinį gama skirstinį"
			}
		]
	},
	{
		name: "GAMMADIST",
		description: "Grąžina gama skirstinį.",
		arguments: [
			{
				name: "x",
				description: "- reikšmė, kurios skirstinį reikia apskaičiuoti, ne neigiamas skaičius"
			},
			{
				name: "alfa",
				description: "- skirstinio parametras, teigiamas skaičius"
			},
			{
				name: "beta",
				description: "- skirstinio parametras, teigiamas skaičius. Jei beta = 1, GAMMADIST grąžina standartinį gama skirstinį"
			},
			{
				name: "integral",
				description: "- loginė reikšmė: grąžina integraliąją skirstinio funkciją = TRUE; grąžina tikimybės masės funkciją = FALSE arba nenurodoma"
			}
		]
	},
	{
		name: "GAMMAINV",
		description: "Grąžina atvirkštinį gama integralųjį skirstinį: jei p = GAMMADIST(x,...), tada GAMMAINV(p,...) = x.",
		arguments: [
			{
				name: "tikimybė",
				description: "- tikimybė, susijusi su gama skirstiniu, skaičius tarp 0 ir 1 imtinai"
			},
			{
				name: "alfa",
				description: "- skirstinio parametras, teigiamas skaičius"
			},
			{
				name: "beta",
				description: "- skirstinio parametras, teigiamas skaičius. Jei beta = 1, GAMMAINV grąžina atvirkštinį standartinį gama skirstinį"
			}
		]
	},
	{
		name: "GAMMALN",
		description: "Grąžina gama funkcijos natūralųjį algoritmą.",
		arguments: [
			{
				name: "x",
				description: "- reikšmė, kurios GAMMALN reikia apskaičiuoti, teigiamas skaičius"
			}
		]
	},
	{
		name: "GAMMALN.PRECISE",
		description: "Grąžina gama funkcijos natūralųjį algoritmą.",
		arguments: [
			{
				name: "x",
				description: "- reikšmė, kurios GAMMALN.PRECISE reikia apskaičiuoti, teigiamas skaičius"
			}
		]
	},
	{
		name: "GAUSS",
		description: "",
		arguments: [
		]
	},
	{
		name: "GCD",
		description: "Pateikia didžiausią bendrąjį daliklį.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "yra reikšmės nuo 1 iki 255"
			},
			{
				name: "skaičius2",
				description: "yra reikšmės nuo 1 iki 255"
			}
		]
	},
	{
		name: "GEOMEAN",
		description: "Grąžina teigiamų skaitinių duomenų masyvo ar diapazono geometrinį vidurkį.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "– nuo 1 iki 255 skaičių arba pavadinimų, masyvų ar nuorodų į skaičius, kurių vidurkį reikia apskaičiuoti"
			},
			{
				name: "skaičius2",
				description: "– nuo 1 iki 255 skaičių arba pavadinimų, masyvų ar nuorodų į skaičius, kurių vidurkį reikia apskaičiuoti"
			}
		]
	},
	{
		name: "GESTEP",
		description: "Tikrina, ar skaičius yra didesnis už ribinę reikšmę.",
		arguments: [
			{
				name: "skaičius",
				description: "yra žingsnio tikrinimo reikšmė"
			},
			{
				name: "žingsnis",
				description: "yra ribinė reikšmė"
			}
		]
	},
	{
		name: "GETPIVOTDATA",
		description: "išskleisti duomenys laikomi PivotTable.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "duomenų_laukas",
				description: "– duomenų lauko, kurio duomenis reikia išskleisti, pavadinimas"
			},
			{
				name: "pivot_table",
				description: "– nuoroda į PivotTable langelį ar langelių diapazoną, kuriame yra reikalingi duomenys"
			},
			{
				name: "laukas",
				description: "nurodomas laukas"
			},
			{
				name: "elem",
				description: "nurodomo lauko elementas"
			}
		]
	},
	{
		name: "GROWTH",
		description: "Grąžina eksponentinio augimo tendencijos skaičius, atitinkančius žinomus duomenų taškus.",
		arguments: [
			{
				name: "žinomi_y",
				description: "- y reikšmių rinkinys, jau žinomas iš y = b*m^x, teigiamų skaičių masyvas ar diapazonas"
			},
			{
				name: "žinomi_x",
				description: "- nebūtinas x reikšmių rinkinys, jau žinomas iš y = b*m^x, masyvas arba diapazonas tokio pat dydžio kaip ir Žinomi_y"
			},
			{
				name: "nauji_x",
				description: "- naujos x reikšmės, kurių atitinkamas y reikšmes GROWTH turi apskaičiuoti"
			},
			{
				name: "konst",
				description: "- loginė reikšmė: konstanta b skaičiuojama įprastai, jei Konst = TRUE; b prilyginama 1, jei Konst = FALSE arba šio argumento nėra"
			}
		]
	},
	{
		name: "HARMEAN",
		description: "Grąžina teigiamų reikšmių duomenų rinkinio harmoninį vidurkį: atvirkštinių reikšmių aritmetinio vidurkio atvirkštinę reikšmę.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "– nuo 1 iki 255 skaičių arba pavadinimų, masyvų ar nuorodų į skaičius, kurių harmoninį vidurkį reikia apskaičiuoti"
			},
			{
				name: "skaičius2",
				description: "– nuo 1 iki 255 skaičių arba pavadinimų, masyvų ar nuorodų į skaičius, kurių harmoninį vidurkį reikia apskaičiuoti"
			}
		]
	},
	{
		name: "HEX2BIN",
		description: "Šešioliktainį skaičių konvertuoja į dvejetainį.",
		arguments: [
			{
				name: "skaičiai",
				description: "yra šešioliktainis skaičius, kurį norite konvertuoti"
			},
			{
				name: "vietos",
				description: "yra simbolių, kuriuos norite naudoti, skaičius"
			}
		]
	},
	{
		name: "HEX2DEC",
		description: "Šešioliktainį skaičių konvertuoja į dešimtainį.",
		arguments: [
			{
				name: "skaičius",
				description: "yra šešioliktainis skaičius, kurį norite konvertuoti"
			}
		]
	},
	{
		name: "HEX2OCT",
		description: "Šešioliktainį skaičių konvertuoja į aštuntainį.",
		arguments: [
			{
				name: "skaičiai",
				description: "yra šešioliktainis skaičius, kurį norite konvertuoti"
			},
			{
				name: "vietos",
				description: "yra simbolių, kuriuos norite naudoti, skaičius"
			}
		]
	},
	{
		name: "HLOOKUP",
		description: "Ieško lentelės ar masyvo viršutinėje eilutėje tam tikros reikšmės ir grąžina reikšmę, esančią tame pačiame jūsų nurodytos eilutės stulpelyje.",
		arguments: [
			{
				name: "ieškos_reikšmė",
				description: "- reikšmė, kurią reikia rasti pirmoje lentelės eilutėje, tai gali būti reikšmė, nuoroda arba teksto eilutė"
			},
			{
				name: "lentelė_masyvas",
				description: "- tekstų, skaičių ar loginių reikšmių lentelė, kurioje vykdoma ieška. Lentelė_masyvas gali būti nuoroda į diapazoną arba diapazono pavadinimas"
			},
			{
				name: "eil_indekso_num",
				description: "- lentelės arba masyvo (lentelė_masyvas) eilutės, iš kurios grąžinama reikšmė, numeris. Pirma lentelės reikšmių eilutė yra 1"
			},
			{
				name: "diapaz_ieškoti",
				description: "- loginė reikšmė: norėdami rasti viršutinėje eilutėje (surūšiuotoje didėjimo tvarka) panašiausią reikšmę, nurodykite TRUE arba nieko; norėdami rasti tikslų sutapimą, nurodykite FALSE"
			}
		]
	},
	{
		name: "HOUR",
		description: "Grąžina valandas kaip skaičių nuo 0 (vidurnaktis) iki 23 (23 val.).",
		arguments: [
			{
				name: "eilės_numeris",
				description: "- skaičius datos-laiko kodo formatu, naudojamu programos Spreadsheet, arba tekstas laiko formatu, pvz., 16:48:00"
			}
		]
	},
	{
		name: "HYPERLINK",
		description: "Kuria failo nuorodą arba peršokimo vietą, iš kurios atidaromas dokumentas, saugomas standžiajame diske, tinklo serveryje arba internete.",
		arguments: [
			{
				name: "saito_vieta",
				description: "- tekstas, nurodantis atidaromo dokumento kelią ir failo vardą, UNC adresą arba URL kelią"
			},
			{
				name: "lengv_vardas",
				description: "- langelyje rodomas tekstas arba skaičius. Jei nenurodyta, langelyje bus rodomas Saito_vietos tekstas"
			}
		]
	},
	{
		name: "HYPGEOM.DIST",
		description: "Grąžina hipergeometrinį skirstinį.",
		arguments: [
			{
				name: "imties_p_į",
				description: "- palankių įvykių skaičius imtyje"
			},
			{
				name: "skaič_imtis",
				description: "- imties dydis"
			},
			{
				name: "gen_aibės_p_į",
				description: "- palankių įvykių skaičius generalinėje aibėje"
			},
			{
				name: "skaič_gen_aibė",
				description: "- generalinės aibės dydis"
			},
			{
				name: "integral",
				description: "- loginė reikšmė: integraliajai skirstinio funkcijai naudokite TRUE; tikimybės tankio funkcijai naudokite FALSE"
			}
		]
	},
	{
		name: "HYPGEOMDIST",
		description: "Grąžina hipergeometrinį skirstinį.",
		arguments: [
			{
				name: "imties_p_į",
				description: "- palankių įvykių skaičius imtyje"
			},
			{
				name: "skaič_imtis",
				description: "- imties dydis"
			},
			{
				name: "gen_aibės_p_į",
				description: "- palankių įvykių skaičius generalinėje aibėje"
			},
			{
				name: "skaič_gen_aibė",
				description: "- generalinės aibės dydis"
			}
		]
	},
	{
		name: "IF",
		description: "Tikrina, ar tenkinama sąlyga, ir grąžinama viena reikšmė, jei sąlyga  yra TRUE, arba kita reikšmė, jei sąlyga yra FALSE.",
		arguments: [
			{
				name: "loginis_tikrinimas",
				description: "- bet kokia reikšmė ar išraiška, kuri gali būti įvertinta kaip TRUE arba FALSE"
			},
			{
				name: "reikšmė_jei_tiesa",
				description: "- reikšmė grąžinama, jei Loginis_tikrinimas yra TRUE. Jei praleista, grąžinama TRUE. Galima įdėti vieną į kitą iki septynių IF funkcijų"
			},
			{
				name: "reikšmė_jei_melas",
				description: "- reikšmė grąžinama, jei Loginis_tikrinimas yra FALSE. Jei praleista, grąžinama FALSE"
			}
		]
	},
	{
		name: "IFERROR",
		description: "Pateikia reikšmę_jei_klaida, jei išraiška yra klaidinga, jei ne – pateikiama pati išraiška.",
		arguments: [
			{
				name: "reikšmė",
				description: "yra bet kuri išraiškos ar nuorodos reikšmė"
			},
			{
				name: "reikšmė_jei_klaida",
				description: "yra bet kuri reikšmė, išraiška ar nuoroda"
			}
		]
	},
	{
		name: "IFNA",
		description: "Pateikia jūsų nurodytą reikšmę, jei reiškinys tampa #N/A, kitu atveju pateikia reiškinio rezultatą.",
		arguments: [
			{
				name: "reikšmė",
				description: "yra bet kokia reikšmė, reiškinys arba nuoroda"
			},
			{
				name: "value_if_na",
				description: "yra bet kokia reikšmė, reiškinys arba nuoroda"
			}
		]
	},
	{
		name: "IMABS",
		description: "Pateikia kompleksinio skaičiaus absoliučiąją reikšmę (modulį).",
		arguments: [
			{
				name: "menam_skaičius",
				description: "yra kompleksinis skaičius, kurio absoliučiąją reikšmę norite gauti"
			}
		]
	},
	{
		name: "IMAGINARY",
		description: "Pateikia kompleksinio skaičiaus menamąjį koeficientą.",
		arguments: [
			{
				name: "menam_skaičius",
				description: "yra kompleksinis skaičius, kurio menamasis koeficientas skaičiuojamas"
			}
		]
	},
	{
		name: "IMARGUMENT",
		description: "Pateikia q argumentą, t. y. radianais išreikštą kampą.",
		arguments: [
			{
				name: "menam_skaičius",
				description: "yra kompleksinis skaičius, kurio argumentą norite skaičiuoti"
			}
		]
	},
	{
		name: "IMCONJUGATE",
		description: "Pateikia kompleksinio skaičiaus kompleksinį junginį.",
		arguments: [
			{
				name: "menam_skaičius",
				description: "yra kompleksinis skaičius, kurio junginys skaičiuojamas"
			}
		]
	},
	{
		name: "IMCOS",
		description: "Pateikia kompleksinio skaičiaus kosinusą.",
		arguments: [
			{
				name: "menam_skaičius",
				description: "yra kompleksinis skaičius, kurio kosinusas skaičiuojamas"
			}
		]
	},
	{
		name: "IMCOSH",
		description: "Pateikia sudėtinio skaičiaus hiperbolinį kosinusą.",
		arguments: [
			{
				name: "skaičius",
				description: "yra sudėtinis skaičius, kurio hiperbolinį kosinusą norite gauti"
			}
		]
	},
	{
		name: "IMCOT",
		description: "Pateikia kompleksinio skaičiaus kotangentą.",
		arguments: [
			{
				name: "skaičius",
				description: "yra kompleksinis skaičius, kurio kotangentą norite apskaičiuoti"
			}
		]
	},
	{
		name: "IMCSC",
		description: "Pateikia kompleksinio skaičiaus kosekantą.",
		arguments: [
			{
				name: "skaičius",
				description: "yra kompleksinis skaičius, kurios kosekantą norite apskaičiuoti"
			}
		]
	},
	{
		name: "IMCSCH",
		description: "Pateikia kompleksinio skaičiaus hiperbolinį kosekantą.",
		arguments: [
			{
				name: "skaičius",
				description: "yra kompleksinis skaičius, kurio hiperbolinį kosekantą norite apskaičiuoti"
			}
		]
	},
	{
		name: "IMDIV",
		description: "Pateikia dviejų kompleksinių skaičių dalmenį.",
		arguments: [
			{
				name: "menam_skaičius1",
				description: "yra kompleksinis skaitiklis arba dalmuo"
			},
			{
				name: "menam_skaičius2",
				description: "yra kompleksinis vardiklis arba daliklis"
			}
		]
	},
	{
		name: "IMEXP",
		description: "Pateikia kompleksinio skaičiaus eksponentę.",
		arguments: [
			{
				name: "menam_skaičius",
				description: "yra kompleksinis skaičius, kurio eksponentė skaičiuojama"
			}
		]
	},
	{
		name: "IMLN",
		description: "Pateikia kompleksinio skaičiaus natūralųjį logaritmą.",
		arguments: [
			{
				name: "menam_skaičius",
				description: "yra kompleksinis skaičius, kurio natūralusis logaritmas skaičiuojamas"
			}
		]
	},
	{
		name: "IMLOG10",
		description: "Pateikia kompleksinio skaičiaus dešimtainį logaritmą.",
		arguments: [
			{
				name: "menam_skaičius",
				description: "yra kompleksinis skaičius, kurio bendrasis logaritmas skaičiuojamas"
			}
		]
	},
	{
		name: "IMLOG2",
		description: "Pateikia kompleksinio skaičiaus dvejetainį logaritmą.",
		arguments: [
			{
				name: "menam_skaičius",
				description: "yra kompleksinis skaičius, kurio dvejetainis logaritmas skaičiuojamas"
			}
		]
	},
	{
		name: "IMPOWER",
		description: "Pateikia kompleksinį skaičių, pakeltą sveikuoju skaičiumi.",
		arguments: [
			{
				name: "menam_skaičius",
				description: "yra kompleksinis skaičius, kurį norite pakelti sveikuoju skaičiumi"
			},
			{
				name: "skaičius",
				description: "yra sveikasis skaičius, kuriuo norite pakelti kompleksinį skaičių"
			}
		]
	},
	{
		name: "IMPRODUCT",
		description: "Pateikia nuo 1 iki 255 kompleksinių skaičių sandaugą.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "menam_skaičius1",
				description: "1menam_skaičius, 2menam_skaičius,... yra nuo 1 iki 255 dauginamų kompleksinių skaičių."
			},
			{
				name: "menam_skaičius2",
				description: "1menam_skaičius, 2menam_skaičius,... yra nuo 1 iki 255 dauginamų kompleksinių skaičių."
			}
		]
	},
	{
		name: "IMREAL",
		description: "Pateikia kompleksinio skaičiaus realųjį koeficientą.",
		arguments: [
			{
				name: "menam_skaičius",
				description: "yra kompleksinis skaičius, kurio realusis koeficientas skaičiuojamas"
			}
		]
	},
	{
		name: "IMSEC",
		description: "Pateikia kompleksinio skaičiaus sekantą.",
		arguments: [
			{
				name: "skaičius",
				description: "yra kompleksinis skaičius, kurio sekantą norite apskaičiuoti"
			}
		]
	},
	{
		name: "IMSECH",
		description: "Pateikia kompleksinio skaičiaus hiperbolinį sekantą.",
		arguments: [
			{
				name: "skaičius",
				description: "yra kompleksinis skaičius, kurio hiperbolinį sekantą norite apskaičiuoti"
			}
		]
	},
	{
		name: "IMSIN",
		description: "Pateikia kompleksinio skaičiaus sinusą.",
		arguments: [
			{
				name: "menam_skaičius",
				description: "yra kompleksinis skaičius, kurio sinusas skaičiuojamas"
			}
		]
	},
	{
		name: "IMSINH",
		description: "Pateikia sudėtinio skaičiaus hiperbolinį sinusą.",
		arguments: [
			{
				name: "skaičius",
				description: "yra sudėtinis skaičius, kurio hiperbolinį sinusą norite gauti"
			}
		]
	},
	{
		name: "IMSQRT",
		description: "Pateikia kompleksinio skaičiaus kvadratinę šaknį.",
		arguments: [
			{
				name: "menam_skaičius",
				description: "yra kompleksinis skaičius, kurio kvadratinė šaknis skaičiuojama"
			}
		]
	},
	{
		name: "IMSUB",
		description: "Pateikia dviejų kompleksinių skaičių skirtumą.",
		arguments: [
			{
				name: "menam_skaičius1",
				description: "yra kompleksinis skaičius, iš kurio atimamas menam_skaičius2"
			},
			{
				name: "menam_skaičius2",
				description: "yra kompleksinis skaičius, atimamas iš menam_skaičiaus1"
			}
		]
	},
	{
		name: "IMSUM",
		description: "Pateikia kompleksinių skaičių sumą.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "menam_skaičius1",
				description: " nuo 1 iki 255 sumuojamų kompleksinių skaičių"
			},
			{
				name: "menam_skaičius2",
				description: " nuo 1 iki 255 sumuojamų kompleksinių skaičių"
			}
		]
	},
	{
		name: "IMTAN",
		description: "Pateikia kompleksinio skaičiaus tangentą.",
		arguments: [
			{
				name: "skaičius",
				description: "yra kompleksinis skaičius, kurio tangentą norite apskaičiuoti"
			}
		]
	},
	{
		name: "INDEX",
		description: "Grąžina tam tikros eilutės ir stulpelio, esančių nurodytame diapazone, susikirtimo vietoje esančio langelio reikšmę ar nuorodą.",
		arguments: [
			{
				name: "masyvas",
				description: "-  langelių diapazonas arba masyvo konstanta."
			},
			{
				name: "eil_num",
				description: "pažymi masyvo ar nuorodos eilutę, iš kurios bus grąžinta reikšmė. Jei praleista, reikalingas Stulpelio_num"
			},
			{
				name: "stulp_num",
				description: "pažymi masyvo ar nuorodos stulpelį, iš kurio bus grąžinta reikšmė. Jei praleista, reikalingas Eil_num"
			}
		]
	},
	{
		name: "INDIRECT",
		description: "Grąžina nuorodą, nurodyta teksto eilute.",
		arguments: [
			{
				name: "nuor_tekstas",
				description: "- nuoroda į langelį, turintį A1 arba R1C1 stiliaus nuorodą, pavadinimą, apibrėžtą kaip nuoroda, arba nuorodą į langelį kaip teksto eilutę"
			},
			{
				name: "a1",
				description: "- loginė reikšmė, kuri nurodo nuorodos tipą Nuor_tekste: R1C1 stilius = FALSE; A1 stilius = TRUE ar nenurodoma"
			}
		]
	},
	{
		name: "INFO",
		description: "Grąžina informaciją apie dabartinę operacinę sistemą.",
		arguments: [
			{
				name: "tipo_tekstas",
				description: "- tekstas, nurodantis, kokio tipo informaciją norite gauti."
			}
		]
	},
	{
		name: "INT",
		description: "Suapvalina skaičių iki artimiausios sveikosios reikšmės.",
		arguments: [
			{
				name: "skaičius",
				description: "- realusis skaičius suapvalinamas iki sveikosios reikšmės"
			}
		]
	},
	{
		name: "INTERCEPT",
		description: "Apskaičiuoja tašką, kuriame linija kirs y ašį, naudodama geriausio išlyginimo regresijos liniją, nubrėžtą per žinomas x ir y reikšmes.",
		arguments: [
			{
				name: "žinomi_y",
				description: "- priklausomas stebinių arba duomenų rinkinys, kurį gali sudaryti skaičiai arba vardai, masyvai ar nuorodos į skaičius"
			},
			{
				name: "žinomi_x",
				description: "- nepriklausomas stebinių arba duomenų rinkinys, kurį gali sudaryti skaičiai arba vardai, masyvai ar nuorodos į skaičius"
			}
		]
	},
	{
		name: "INTRATE",
		description: "Pateikia visiškai investuotų vertybinių popierių palūkanų normą.",
		arguments: [
			{
				name: "sudengimas",
				description: "yra vertybinių popierių sudengimo data, išreikšta datų sekos numeriu"
			},
			{
				name: "mok_term",
				description: "yra vertybinių popierių mokėjimo terminas, išreikštas datų sekos numeriu"
			},
			{
				name: "investicija",
				description: "yra į vertybinius popierius investuota suma"
			},
			{
				name: "padengimas",
				description: "yra suėjus mokėjimo terminui gaunama suma"
			},
			{
				name: "pagrindas",
				description: "yra naudojamas dienų skaičiavimo pagrindo tipas"
			}
		]
	},
	{
		name: "IPMT",
		description: "Grąžina nurodyto laikotarpio investicijos palūkanų mokėjimą pagal periodinius, pastovius mokėjimus ir pastovią palūkanų normą.",
		arguments: [
			{
				name: "norma",
				description: "- palūkanų normą per laikotarpį. Pvz., trijų mėnesių mokėjimams naudokite 6%/4, jei MPN yra 6%"
			},
			{
				name: "laikot",
				description: "- laikotarpis, kuriam reikia nustatyti palūkanas; jis turi įeiti į diapazoną nuo 1 iki Laikot_sk"
			},
			{
				name: "laikot_sk",
				description: "- bendras investicijos mokėjimo laikotarpių skaičius"
			},
			{
				name: "dr",
				description: "- dabartinė reikšmė arba bendra suma, kurios dabar verti būsimi mokėjimai"
			},
			{
				name: "br",
				description: "- būsima reikšmė arba grynųjų balansas, kurį reikia pasiekti po paskutinio mokėjimo. Jei nenurodyta, Br = 0"
			},
			{
				name: "tipas",
				description: "- loginė reikšmė, nurodanti mokėjimo laiką: laikotarpio pabaigoje = 0 arba nenurodoma, laikotarpio pradžioje = 1"
			}
		]
	},
	{
		name: "IRR",
		description: "Grąžina pinigų srautų sekos vidinę įplaukų normą.",
		arguments: [
			{
				name: "reikšmė",
				description: "- masyvas arba nuoroda į langelius su skaičiais, kurių vidinę įplaukų normą reikia apskaičiuoti"
			},
			{
				name: "siūloma",
				description: "- skaičius, kuris turėtų būti artimas VĮN rezultatui; 0,1 (10 procentų), jei nenurodyta"
			}
		]
	},
	{
		name: "ISBLANK",
		description: "Tikrina, ar nuoroda yra į tuščią langelį, ir grąžina TRUE arba FALSE.",
		arguments: [
			{
				name: "reikšmė",
				description: "- langelis arba pavadinimas, nurodantis langelį, kurį reikia tikrinti"
			}
		]
	},
	{
		name: "ISERR",
		description: "Tikrina, ar reikšmė yra klaida (#VALUE!, #REF!, #DIV/0!, #NUM!, #NAME? arba #NULL!), išskyrus #N/A, ir grąžina TRUE arba FALSE.",
		arguments: [
			{
				name: "reikšmė",
				description: "– reikšmė, kurią reikia tikrinti. Reikšmė gali būti nuoroda į langelį, formulė arba pavadinimas, nurodantis langelį, formulę arba reikšmę"
			}
		]
	},
	{
		name: "ISERROR",
		description: "Tikrina, ar reikšmė yra klaida (#N/A, #VALUE!, #REF!, #DIV/0!, #NUM!, #NAME? arba #NULL!), ir grąžina TRUE arba FALSE.",
		arguments: [
			{
				name: "reikšmė",
				description: "– tikrinama reikšmė. Reikšmė gali būti nuoroda į langelį ar formulę arba vardas, nurodantis langelį, formulę arba reikšmę"
			}
		]
	},
	{
		name: "ISEVEN",
		description: "Jei skaičius lyginis, pateikia TRUE.",
		arguments: [
			{
				name: "skaičius",
				description: "yra tikrinama reikšmė"
			}
		]
	},
	{
		name: "ISFORMULA",
		description: "Tikrina, ar tai yra langelio su formule nuoroda, ir pateikia TRUE arba FALSE.",
		arguments: [
			{
				name: "nuoroda",
				description: "yra langelio, kurį norite patikrinti nuoroda. Nuoroda gali būti langelio nuoroda, formulė arba pavadinimas, nurodantis langelį"
			}
		]
	},
	{
		name: "ISLOGICAL",
		description: "Tikrina, ar reikšmė yra loginė (TRUE arba FALSE), ir grąžina TRUE arba FALSE.",
		arguments: [
			{
				name: "reikšmė",
				description: "- reikšmė, kurią reikia patikrinti. Reikšmė gali būti nuoroda į langelį, formulę arba vardą, kuris nurodo langelį, formulę ar reikšmę"
			}
		]
	},
	{
		name: "ISNA",
		description: "Tikrina, ar reikšmė yra #N/A, ir grąžina TRUE arba FALSE.",
		arguments: [
			{
				name: "reikšmė",
				description: "- tikrinama reikšmė. Reikšmė gali būti nuoroda į langelį ar formulę arba vardas, nurodantis langelį, formulę arba reikšmę"
			}
		]
	},
	{
		name: "ISNONTEXT",
		description: "Tikrina, ar reikšmė nėra tekstas (tušti langeliai nėra tekstas), ir grąžina TRUE arba FALSE.",
		arguments: [
			{
				name: "reikšmė",
				description: "- reikšmė, kurią reikia patikrinti: langelis; formulė arba vardas, nurodantis langelį, formulę ar reikšmę"
			}
		]
	},
	{
		name: "ISNUMBER",
		description: "Tikrina, ar reikšmė yra skaičius, ir grąžina TRUE arba FALSE.",
		arguments: [
			{
				name: "reikšmė",
				description: "- reikšmė, kurią reikia tikrinti. Reikšmė gali būti nuoroda į langelį, formulė arba pavadinimas, nurodantis langelį, formulę arba reikšmę"
			}
		]
	},
	{
		name: "ISO.CEILING",
		description: "Suapvalina skaičių iki artimiausio didesnio sveiko skaičiaus arba artimiausio didesnio reikšmingumo kartotinio.",
		arguments: [
			{
				name: "skaičius",
				description: "- reikšmė, kurią reikia suapvalinti"
			},
			{
				name: "reikšmingumas",
				description: "- kartotinis, iki kurio reikia suapvalinti"
			}
		]
	},
	{
		name: "ISODD",
		description: "Jei skaičius nelyginis, pateikia TRUE.",
		arguments: [
			{
				name: "skaičius",
				description: "yra tikrinama reikšmė"
			}
		]
	},
	{
		name: "ISOWEEKNUM",
		description: "Grąžina pateiktos datos ISO savaičių ir metų skaičių.",
		arguments: [
			{
				name: "data",
				description: "yra datos ir laiko kodas, naudojamas programoje „Spreadsheet“ datai ir laikui apskaičiuoti"
			}
		]
	},
	{
		name: "ISPMT",
		description: "Grąžina palūkanas, mokamas per nurodytą investicijų laikotarpį.",
		arguments: [
			{
				name: "norma",
				description: "- palūkanų norma per laikotarpį. Pvz., esant 6% MPN, keturių mėnesių mokėjimams naudokite 6%/4"
			},
			{
				name: "laikot",
				description: "- laikotarpis, kurio palūkanas reikia apskaičiuoti"
			},
			{
				name: "nlaikot",
				description: "- investicijos mokėjimo laikotarpių skaičius"
			},
			{
				name: "pv",
				description: "- būsimų mokėjimų sekos bendrosios sumos dydis"
			}
		]
	},
	{
		name: "ISREF",
		description: "Tikrina, ar reikšmė yra nuoroda, ir grąžina TRUE arba FALSE.",
		arguments: [
			{
				name: "reikšmė",
				description: "- reikšmė, kurią reikia tikrinti. Reikšmė gali būti nuoroda į langelį, į formulę ar į pavadinimą, kuris nurodo langelį, formulę arba reikšmę"
			}
		]
	},
	{
		name: "ISTEXT",
		description: "Tikrina, ar reikšmė yra tekstas, ir grąžina TRUE arba FALSE.",
		arguments: [
			{
				name: "reikšmė",
				description: "- reikšmė, kurią reikia tikrinti. Reikšmė gali būti nuoroda į langelį, formulė arba pavadinimas, nurodantis langelį, formulę arba reikšmę"
			}
		]
	},
	{
		name: "KURT",
		description: "Grąžina duomenų rinkinio ekscesą.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "– nuo 1 iki 255 skaičių arba vardų, masyvų ar nuorodų į skaičius, kurių ekscesą reikia apskaičiuoti"
			},
			{
				name: "skaičius2",
				description: "– nuo 1 iki 255 skaičių arba vardų, masyvų ar nuorodų į skaičius, kurių ekscesą reikia apskaičiuoti"
			}
		]
	},
	{
		name: "LARGE",
		description: "Grąžina duomenų rinkinio k-ąją didžiausiąją reikšmę. Pvz., penktą pagal dydį skaičių.",
		arguments: [
			{
				name: "masyvas",
				description: "- duomenų masyvas arba diapazonas, kurio k-ąją didžiausiąją reikšmę reikia nustatyti"
			},
			{
				name: "k",
				description: "- ieškomos reikšmės pozicija (nuo didžiausios reikšmės) masyve arba langelių diapazone"
			}
		]
	},
	{
		name: "LCM",
		description: "Pateikia mažiausią bendrąjį kartotinį.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "yra reikšmės nuo 1 iki 255, kurių mažiausią bendrąjį kartotinį norite skaičiuoti"
			},
			{
				name: "skaičius2",
				description: "yra reikšmės nuo 1 iki 255, kurių mažiausią bendrąjį kartotinį norite skaičiuoti"
			}
		]
	},
	{
		name: "LEFT",
		description: "Grąžina nurodytą simbolių skaičių iš teksto eilutės pradžios.",
		arguments: [
			{
				name: "tekstas",
				description: "- teksto eilutė su simboliais, kuriuos reikia gauti"
			},
			{
				name: "simb_kiekis",
				description: "- nurodo, kiek simbolių turi grąžinti LEFT, jei nieko nenurodyta, grąžinamas vienas"
			}
		]
	},
	{
		name: "LEN",
		description: "Grąžina teksto eilutės simbolių kiekį.",
		arguments: [
			{
				name: "tekstas",
				description: "- tekstas, kurio ilgį reikia rasti. Tarpai laikomi simboliais"
			}
		]
	},
	{
		name: "LINEST",
		description: "Grąžina statistiką, kuria aprašoma tiesinė krypties linija, atitinkanti žinomus duomenų taškus, pritaikant tiesią liniją ir taikant mažiausių kvadratų metodą.",
		arguments: [
			{
				name: "žinomi_y",
				description: "- y reikšmių rinkinys, kurį jau žinote iš y = mx + b"
			},
			{
				name: "žinomi_x",
				description: "- nebūtinas x reikšmių rinkinys, kurį jau žinote iš y = mx + b"
			},
			{
				name: "konst",
				description: "- loginė reikšmė: konstanta b skaičiuojama įprastai, jei argumentas Konst = TRUE arba jo nėra; b prilyginama 0, jei Konst = FALSE"
			},
			{
				name: "stats",
				description: "- loginė reikšmė: kai Stats = TRUE, grąžinama papildoma regresijos statistika; kai Stats = FALSE arba nenurodytas, grąžinamas koeficientas m ir konstanta b"
			}
		]
	},
	{
		name: "LN",
		description: "Grąžina skaičiaus natūralųjį logaritmą.",
		arguments: [
			{
				name: "skaičius",
				description: "- teigiamas realusis skaičius, kurio natūralusis logaritmas skaičiuojamas"
			}
		]
	},
	{
		name: "LOG",
		description: "Grąžina skaičiaus logaritmą nurodytu pagrindu.",
		arguments: [
			{
				name: "skaičius",
				description: "-teigiamas realusis skaičius, kurio logaritmą norite rasti"
			},
			{
				name: "pagrindas",
				description: "- logaritmo pagrindas; jei nenurodyta, imama 10"
			}
		]
	},
	{
		name: "LOG10",
		description: "Grąžina skaičiaus dešimtainį logaritmą.",
		arguments: [
			{
				name: "skaičius",
				description: "- teigiamas realusis skaičius, kurio dešimtainis logaritmas skaičiuojamas"
			}
		]
	},
	{
		name: "LOGEST",
		description: "Grąžina statistiką, kuria aprašoma eksponentinė kreivė, atitinkanti žinomus duomenų taškus.",
		arguments: [
			{
				name: "žinomi_y",
				description: "- y reikšmių rinkinys, jau žinomas iš y = b*m^x"
			},
			{
				name: "žinomi_x",
				description: "- nebūtinas x reikšmių rinkinys, jau žinomas iš y = b*m^x"
			},
			{
				name: "konst",
				description: "- loginė reikšmė: konstanta b skaičiuojama įprastai, jei Konst = TRUE ar šio argumento nėra; b prilyginama 1, jei Konst = FALSE"
			},
			{
				name: "stats",
				description: "- loginė reikšmė: Grąžina papildomą regresijos statistiką, jei Stats = TRUE; grąžina koeficientus m ir konstantą b, jei Stats = FALSE arba šio argumento nėra"
			}
		]
	},
	{
		name: "LOGINV",
		description: "Grąžina atvirkštinį lognormalųjį x skirstinį, čia ln(x) yra normaliai paskirstyta su parametrais Vidurkis ir Standart_nuokryp.",
		arguments: [
			{
				name: "tikimybė",
				description: "- reikšmė, susijusi su lognormaliuoju skirstiniu, skaičius nuo 0 iki 1 imtinai"
			},
			{
				name: "vidurkis",
				description: "- ln(x) vidurkis"
			},
			{
				name: "standart_nuokr",
				description: "- ln(x) standartinis nuokrypis, teigiamas skaičius"
			}
		]
	},
	{
		name: "LOGNORM.DIST",
		description: "Grąžina lognormalųjį x skirstinį, čia ln(x) yra normaliai paskirstyta su parametrais Vidurkis ir Standart_nuokryp.",
		arguments: [
			{
				name: "x",
				description: "- reikšmė, pagal kurią skaičiuojama funkcija, teigiamas skaičius"
			},
			{
				name: "vidurkis",
				description: "- ln(x) vidurkis"
			},
			{
				name: "standart_nuokr",
				description: "- ln(x) standartinis nuokrypis, teigiamas skaičius"
			},
			{
				name: "integral",
				description: "loginė reikšmė: integraliajai skirstinio funkcijai naudokite TRUE; tikimybės tankio funkcijai naudokite FALSE"
			}
		]
	},
	{
		name: "LOGNORM.INV",
		description: "Grąžina atvirkštinį lognormalųjį x skirstinį, čia ln(x) yra normaliai paskirstyta su parametrais Vidurkis ir Standart_nuokr.",
		arguments: [
			{
				name: "tikimybė",
				description: "- tikimybė, susijusi su lognormaliuoju skirstiniu, skaičius nuo 0 iki 1 imtinai"
			},
			{
				name: "vidurkis",
				description: "- ln(x) vidurkis"
			},
			{
				name: "standart_nuokr",
				description: "- ln(x) standartinis nuokrypis, teigiamas skaičius"
			}
		]
	},
	{
		name: "LOGNORMDIST",
		description: "Grąžina integralųjį lognormalųjį x skirstinį, čia ln(x) yra normaliai paskirstyta su parametrais Vidurkis ir Standart_nuokryp.",
		arguments: [
			{
				name: "x",
				description: "- reikšmė, pagal kurią skaičiuojama funkcija, teigiamas skaičius"
			},
			{
				name: "vidurkis",
				description: "- ln(x) vidurkis"
			},
			{
				name: "standart_nuokr",
				description: "- ln(x) standartinis nuokrypis, teigiamas skaičius"
			}
		]
	},
	{
		name: "LOOKUP",
		description: "Ieško reikšmės vienos eilutės ar vieno stulpelio diapazone arba masyve. Skirta atgaliniam suderinamumui.",
		arguments: [
			{
				name: "ieškos_reikšmė",
				description: "- reikšmė, kurios LOOKUP ieško Ieškos_vektoriuje; tai gali būti skaičius, loginė reikšmė, vardas ar nuoroda į reikšmę"
			},
			{
				name: "ieškos_vektorius",
				description: "- diapazonas, turintis tik vieną eilutę arba vieną stulpelį teksto, skaičių arba loginių reikšmių, išdėstytų mažėjimo tvarka"
			},
			{
				name: "rezultatų_vektorius",
				description: "- tokio pat dydžio, kaip Ieškos_vektoriaus diapazonas, turintis tik vieną eilutę arba stulpelį"
			}
		]
	},
	{
		name: "LOWER",
		description: "Pakeičia visas teksto eilutės raides mažosiomis.",
		arguments: [
			{
				name: "tekstas",
				description: "- tekstas, kurį reikia konvertuoti. Simboliai, kurie nėra raidės, nekonvertuojami"
			}
		]
	},
	{
		name: "MATCH",
		description: "Grąžina santykinę elemento poziciją masyve, kuri atitinka nurodytos sekos nurodytą reikšmę.",
		arguments: [
			{
				name: "ieškos_reikšmė",
				description: "- reikšmė, naudojama norimai reikšmei masyve rasti, skaičius, tekstas arba loginė reikšmė, arba nuoroda į juos"
			},
			{
				name: "ieškos_masyvas",
				description: "- vientisas tokių langelių diapazonas, kuriuose gali būti ieškomos reikšmės, arba reikšmių masyvas, arba nuoroda į masyvą"
			},
			{
				name: "atitinkantis_tipas",
				description: "- skaičius 1, 0, arba -1, rodantis, kurią reikšmę reikia grąžinti"
			}
		]
	},
	{
		name: "MAX",
		description: "Grąžina didžiausią skaičių iš reikšmių rinkinio. Loginių reikšmių ir teksto nepaisoma.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "– nuo 1 iki 255 skaičių, tuščių langelių, loginių reikšmių ar skaičių teksto pavidalu, kurių maksimumas skaičiuojamas"
			},
			{
				name: "skaičius2",
				description: "– nuo 1 iki 255 skaičių, tuščių langelių, loginių reikšmių ar skaičių teksto pavidalu, kurių maksimumas skaičiuojamas"
			}
		]
	},
	{
		name: "MAXA",
		description: "Grąžina didžiausią rinkinio reikšmę. Paisoma loginių reikšmių ir teksto.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "reikšmė1",
				description: "– nuo 1 iki 255 skaičių, tuščių langelių, loginių reikšmių arba teksto pavidalu užrašytų skaičių, kurių maksimumą reikia rasti"
			},
			{
				name: "reikšmė2",
				description: "– nuo 1 iki 255 skaičių, tuščių langelių, loginių reikšmių arba teksto pavidalu užrašytų skaičių, kurių maksimumą reikia rasti"
			}
		]
	},
	{
		name: "MDETERM",
		description: "Grąžina masyvo matricos determinantą.",
		arguments: [
			{
				name: "masyvas",
				description: "- skaitmeninis masyvas, turintis vienodus eilučių ir stulpelių kiekius - langelių diapazonas arba masyvo konstanta"
			}
		]
	},
	{
		name: "MEDIAN",
		description: "Grąžina medianą - reikšmę nurodytų skaičių rinkinio viduryje.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "– nuo 1 iki 255 skaičių arba pavadinimų, masyvų arba nuorodų į skaičius, kurių medianą reikia paskaičiuoti"
			},
			{
				name: "skaičius2",
				description: "– nuo 1 iki 255 skaičių arba pavadinimų, masyvų arba nuorodų į skaičius, kurių medianą reikia paskaičiuoti"
			}
		]
	},
	{
		name: "MID",
		description: "Grąžina simbolius iš teksto eilutės vidurio pagal pradinę poziciją ir ilgį.",
		arguments: [
			{
				name: "tekstas",
				description: "- teksto eilutė, kurios simbolius reikia grąžinti"
			},
			{
				name: "prad_num",
				description: "- pirmo simbolio, kurį reikia grąžinti, pozicija. Pirmas teksto simbolis yra 1"
			},
			{
				name: "simb_skaič",
				description: "- nurodo, kiek teksto simbolių grąžinti"
			}
		]
	},
	{
		name: "MIN",
		description: "Grąžina mažiausią skaičių iš reikšmių rinkinio. Loginių reikšmių ir teksto nepaisoma.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "– nuo 1 iki 255 skaičių, tuščių langelių, loginių reikšmių ar skaičių teksto pavidalu, kurių minimumas skaičiuojamas"
			},
			{
				name: "skaičius2",
				description: "– nuo 1 iki 255 skaičių, tuščių langelių, loginių reikšmių ar skaičių teksto pavidalu, kurių minimumas skaičiuojamas"
			}
		]
	},
	{
		name: "MINA",
		description: "Grąžina mažiausią reikšmę iš rinkinio. Paisoma loginių reikšmių ir teksto.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "reikšmė1",
				description: "- nuo 1 iki 255 skaičių, tuščių langelių, loginių reikšmių arba teksto pavidalu užrašytų skaičių, kurių minimumą reikia rasti"
			},
			{
				name: "reikšmė2",
				description: "- nuo 1 iki 255 skaičių, tuščių langelių, loginių reikšmių arba teksto pavidalu užrašytų skaičių, kurių minimumą reikia rasti"
			}
		]
	},
	{
		name: "MINUTE",
		description: "Grąžina minutes - skaičių nuo 0 iki 59.",
		arguments: [
			{
				name: "eilės_numeris",
				description: "- skaičius datos-laiko kodo formatu, naudojamu programos Spreadsheet, arba tekstas laiko formatu, pvz., 16:48:00"
			}
		]
	},
	{
		name: "MINVERSE",
		description: "Grąžina masyve saugomos matricos atvirkštinę matricą.",
		arguments: [
			{
				name: "masyvas",
				description: "- skaitmeninis masyvas, turintis vienodus eilučių ir stulpelių kiekius - langelių diapazonas arba masyvo konstanta"
			}
		]
	},
	{
		name: "MIRR",
		description: "Grąžina periodiškų grynųjų pinigų srautų sekų vidinę įplaukų normą, atsižvelgdama tiek į investicijų kainą, tiek į palūkanas, gaunamas pakartotinai investavus pinigus.",
		arguments: [
			{
				name: "reikšmės",
				description: "- masyvas ar nuoroda į langelius, kuriuose yra skaičiai, rodantys mokėjimų (neigiamos reikšmės) arba įplaukų (teigiamos reikšmės) sekas per reguliarius laikotarpius"
			},
			{
				name: "finans_norma",
				description: "- palūkanų norma, kurią mokate už pinigus, naudojamus grynųjų pinigų srauto"
			},
			{
				name: "pak_invest_norma",
				description: "- palūkanų norma, kurią gaunate už grynųjų srautą, kai pakartotinai juos investuojate"
			}
		]
	},
	{
		name: "MMULT",
		description: "Grąžina dviejų masyvų matricų sandaugą - masyvą, turintį tiek eilučių, kiek masyvas1, ir tiek stulpelių, kiek masyvas2.",
		arguments: [
			{
				name: "masyvas1",
				description: "– pirmas dauginamas skaičių masyvas; jame turi būti tiek stulpelių, kiek yra eilučių masyve Masyvas2"
			},
			{
				name: "masyvas2",
				description: "– pirmas dauginamas skaičių masyvas; jame turi būti tiek stulpelių, kiek yra eilučių masyve Masyvas2"
			}
		]
	},
	{
		name: "MOD",
		description: "Grąžina liekaną, gautą padalinus skaičių iš daliklio.",
		arguments: [
			{
				name: "skaičius",
				description: "- skaičius, kurio liekaną po dalybos reikia rasti"
			},
			{
				name: "daliklis",
				description: "- skaičius, iš kurio reikia dalinti Skaičių"
			}
		]
	},
	{
		name: "MODE",
		description: "Grąžina dažniausiai pasitaikančią arba pasikartojančią duomenų masyvo arba diapazono reikšmę.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "– nuo 1 iki 255 skaičių arba pavadinimų, masyvų ar nuorodų su skaičiais, kurių atsitiktinio dydžio reikšmę reikia apskaičiuoti"
			},
			{
				name: "skaičius2",
				description: "– nuo 1 iki 255 skaičių arba pavadinimų, masyvų ar nuorodų su skaičiais, kurių atsitiktinio dydžio reikšmę reikia apskaičiuoti"
			}
		]
	},
	{
		name: "MODE.MULT",
		description: "Grąžina dažniausiai pasitaikančių arba pasikartojančių masyvo reikšmių ar duomenų diapazono vertikalų masyvą. Horizontaliam masyvui naudokite =TRANSPOSE(MODE.MULT(skaičius1,skaičius2,...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "nuo 1 iki 255 skaičių arba pavadinimų, masyvų ar nuorodų su skaičiais, kurių atsitiktinio dydžio reikšmę reikia apskaičiuoti"
			},
			{
				name: "skaičius2",
				description: "nuo 1 iki 255 skaičių arba pavadinimų, masyvų ar nuorodų su skaičiais, kurių atsitiktinio dydžio reikšmę reikia apskaičiuoti"
			}
		]
	},
	{
		name: "MODE.SNGL",
		description: "Grąžina dažniausiai pasitaikančią arba pasikartojančią duomenų masyvo arba diapazono reikšmę.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "– nuo 1 iki 255 skaičių arba pavadinimų, masyvų ar nuorodų į skaičius, kurių atsitiktinio dydžio reikšmę reikia apskaičiuoti"
			},
			{
				name: "skaičius2",
				description: "– nuo 1 iki 255 skaičių arba pavadinimų, masyvų ar nuorodų į skaičius, kurių atsitiktinio dydžio reikšmę reikia apskaičiuoti"
			}
		]
	},
	{
		name: "MONTH",
		description: "Grąžina mėnesį, skaičių nuo 1 (sausis) iki 12 (gruodis).",
		arguments: [
			{
				name: "eilės_numeris",
				description: "- skaičius datos-laiko kodo formatu, kurį naudoja programa Spreadsheet"
			}
		]
	},
	{
		name: "MROUND",
		description: "Grąžina iki pageidaujamo kartotinio suapvalintą skaičių.",
		arguments: [
			{
				name: "skaičius",
				description: "yra apvalinimo reikšmė"
			},
			{
				name: "kartotinis",
				description: "Yra kartotinis, iki kurio norite suapvalinti skaičių"
			}
		]
	},
	{
		name: "MULTINOMIAL",
		description: "Pateikia skaičių rinkinio daugianarį.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "nuo 1 iki 255, kurių daugianarį norite skaičiuoti"
			},
			{
				name: "skaičius2",
				description: "nuo 1 iki 255, kurių daugianarį norite skaičiuoti"
			}
		]
	},
	{
		name: "MUNIT",
		description: "Nurodytai dimensijai pateikia vienetų matricą.",
		arguments: [
			{
				name: "dimensija",
				description: "yra sveikasis skaičius, nurodantis vienetų matricos, kurią norite grąžinti, dimensiją"
			}
		]
	},
	{
		name: "N",
		description: "Keičia neskaitinę reikšmę į skaičių, datas į eilės numerius, TRUE į 1, visa kita - į 0 (nulį).",
		arguments: [
			{
				name: "reikšmė",
				description: "- reikšmė, kurią reikia pakeisti"
			}
		]
	},
	{
		name: "NA",
		description: "Grąžina klaidos reikšmę #N/A (reikšmės nėra).",
		arguments: [
		]
	},
	{
		name: "NEGBINOM.DIST",
		description: "Grąžina neigiamą binominį skirstinį, tikimybę, kad prieš tam tikrąjį palankių įvykių skaičių (p_į_skaičius) bus tam tikras nepalankių įvykių skaičius (n_į_skaičius), kai palankaus įvykio tikimybė yra p_į_tikimybė.",
		arguments: [
			{
				name: "skaičius_f",
				description: "- nepalankiųjų įvykių skaičius"
			},
			{
				name: "skaičius_s",
				description: "- slenkstinis palankiųjų įvykių skaičius"
			},
			{
				name: "tikimybė_s",
				description: "- palankiojo įvykio tikimybė; skaičius nuo 0 iki 1"
			},
			{
				name: "integral",
				description: "loginė reikšmė: integraliajai skirstinio funkcijai naudokite TRUE; tikimybės masės funkcijai naudokite FALSE"
			}
		]
	},
	{
		name: "NEGBINOMDIST",
		description: "Grąžina neigiamą binominį skirstinį, tikimybę, kad prieš tam tikrąjį palankių įvykių skaičių (p_į_skaičius) bus tam tikras nepalankių įvykių skaičius (n_į_skaičius), kai palankaus įvykio tikimybė yra p_į_tikimybė.",
		arguments: [
			{
				name: "n_į_skaičius",
				description: "- nepalankiųjų įvykių skaičius"
			},
			{
				name: "p_į_skaičius",
				description: "- slenkstinis palankiųjų įvykių skaičius"
			},
			{
				name: "p_į_tikimybė",
				description: "- palankiojo įvykio tikimybė; skaičius nuo 0 iki 1"
			}
		]
	},
	{
		name: "NETWORKDAYS",
		description: "Pateikia darbo dienų, esančių tarp dviejų datų, skaičių.",
		arguments: [
			{
				name: "pradžios_data",
				description: "yra datų sekos numeris, išreiškiantis pradžios datą"
			},
			{
				name: "pabaigos_data",
				description: "yra datų sekos numeris, išreiškiantis pabaigos datą"
			},
			{
				name: "šventės",
				description: "yra pasirinktinis vienos ar kelių datų sekos numerių, kurie bus išbraukti iš darbo kalendoriaus, rinkinys, pvz., valstybinės šventės"
			}
		]
	},
	{
		name: "NETWORKDAYS.INTL",
		description: "Pateikia darbo dienų, esančių tarp dviejų datų, skaičių su pasirinktiniais savaitgalio parametrais.",
		arguments: [
			{
				name: "pradžios_data",
				description: "yra datų sekos numeris, kuris reiškia pradžios datą"
			},
			{
				name: "pabaigos_data",
				description: "yra datų sekos numeris, kuris reiškia pabaigos datą"
			},
			{
				name: "savaitgalis",
				description: "yra numeris arba eilutė, nurodanti, kada būna savaitgaliai"
			},
			{
				name: "šventės",
				description: "yra pasirinktinis vienos ar kelių datų sekos numerių, kurie bus pašalinti iš darbo kalendoriaus, rinkinys, pvz., valstybinės šventės arba šventės, kurių data nepastovi"
			}
		]
	},
	{
		name: "NOMINAL",
		description: "Pateikia metinį nominalųjį palūkanų koeficientą.",
		arguments: [
			{
				name: "fakt_koef",
				description: "yra faktinis palūkanų koeficientas"
			},
			{
				name: "laik_skaičius",
				description: "yra sudedamųjų metų laikotarpių skaičius"
			}
		]
	},
	{
		name: "NORM.DIST",
		description: "Grąžina normalųjį skirstinį pagal nurodytus vidurkį ir standartinį nuokrypį.",
		arguments: [
			{
				name: "x",
				description: "- reikšmė, kurios skirstinys reikalingas"
			},
			{
				name: "vidurkis",
				description: "- skirstinio aritmetinis vidurkis"
			},
			{
				name: "standart_nuokr",
				description: "- skirstinio standartinis nuokrypis, teigiamas skaičius"
			},
			{
				name: "integral",
				description: "- loginė reikšmė: integraliojo skirstinio funkcijai naudokite TRUE; tikimybės masės funkcijai naudokite FALSE"
			}
		]
	},
	{
		name: "NORM.INV",
		description: "Grąžina atvirkštinį normalųjį integralųjį skirstinį pagal nurodytus vidurkį ir standartinį nuokrypį.",
		arguments: [
			{
				name: "tikimybė",
				description: "- tikimybė, atitinkanti normalųjį skirstinį, skaičius nuo 0 iki 1 imtinai"
			},
			{
				name: "vidurkis",
				description: "- skirstinio aritmetinis vidurkis"
			},
			{
				name: "standart_nuokr",
				description: "- skirstinio standartinis nuokrypis, teigiamas skaičius"
			}
		]
	},
	{
		name: "NORM.S.DIST",
		description: "Grąžina standartinį normalųjį skirstinį (turi nulio ir standartinio nuokrypio vidurkį).",
		arguments: [
			{
				name: "z",
				description: "- reikšmė, kurios skirstinys reikalingas"
			},
			{
				name: "integral",
				description: "loginė reikšmė, nurodanti funkcijai grąžinti: integraliajai skirstinio funkcijai naudokite TRUE; tikimybės masės funkcijai naudokite FALSE"
			}
		]
	},
	{
		name: "NORM.S.INV",
		description: "Grąžina atvirkštinį standartinį normalųjį integralųjį skirstinį (turi nulio ir standartinio nuokrypio vidurkį).",
		arguments: [
			{
				name: "tikimybė",
				description: "tikimybė, atitinkanti normalųjį skirstinį, skaičius tarp 0 ir 1 imtinai"
			}
		]
	},
	{
		name: "NORMDIST",
		description: "Grąžina normalųjį integralųjį skirstinį pagal nurodytus vidurkį ir standartinį nuokrypį.",
		arguments: [
			{
				name: "x",
				description: "- reikšmė, kurios skirstinys reikalingas"
			},
			{
				name: "vidurkis",
				description: "- skirstinio aritmetinis vidurkis"
			},
			{
				name: "standart_nuokr",
				description: "- skirstinio standartinis nuokrypis, teigiamas skaičius"
			},
			{
				name: "integral",
				description: "- loginė reikšmė: integraliojo skirstinio funkcijai naudokite TRUE; tikimybės tankio funkcijai naudokite FALSE"
			}
		]
	},
	{
		name: "NORMINV",
		description: "Grąžina atvirkštinį normalųjį integralųjį skirstinį pagal nurodytus vidurkį ir standartinį nuokrypį.",
		arguments: [
			{
				name: "tikimybė",
				description: "- tikimybė, atitinkanti normalųjį skirstinį, skaičius nuo 0 iki 1 imtinai"
			},
			{
				name: "vidurkis",
				description: "- skirstinio aritmetinis vidurkis"
			},
			{
				name: "standart_nuokr",
				description: "- skirstinio standartinis nuokrypis, teigiamas skaičius"
			}
		]
	},
	{
		name: "NORMSDIST",
		description: "Grąžina standartinį normalųjį integralųjį skirstinį (turi nulio ir standartinio nuokrypio vidurkį).",
		arguments: [
			{
				name: "z",
				description: "- reikšmė, kurios skirstinys reikalingas"
			}
		]
	},
	{
		name: "NORMSINV",
		description: "Grąžina atvirkštinį standartinį normalųjį integralųjį skirstinį (turi nulio ir standartinio nuokrypio vidurkį).",
		arguments: [
			{
				name: "tikimybė",
				description: "- tikimybė, atitinkanti normalųjį skirstinį, skaičius tarp 0 ir 1 imtinai"
			}
		]
	},
	{
		name: "NOT",
		description: "Keičia FALSE į TRUE arba TRUE į FALSE.",
		arguments: [
			{
				name: "log_reikšmė",
				description: "- reikšmė arba išraiška, kurią galima įvertinti kaip  TRUE arba FALSE"
			}
		]
	},
	{
		name: "NOW",
		description: "Grąžina dabartinę datą ir laiką datos ir laiko formatu.",
		arguments: [
		]
	},
	{
		name: "NPER",
		description: "Grąžina investavimo laikotarpių kiekį pagal periodiškus nuolatinius mokėjimus ir pastovias palūkanas.",
		arguments: [
			{
				name: "norma",
				description: "- periodo palūkanų norma. Pavyzdžiui, naudokite 6%/4, jei kas ketvirtį mokama po 6% MPN"
			},
			{
				name: "išl",
				description: "- mokėjimai, vykdomi kiekvieną laikotarpį, kurie nesikeičia per visą investavimo laiką"
			},
			{
				name: "dr",
				description: "- dabartinė reikšmė arba bendroji suma, kurios dabar verti būsimi mokėjimai"
			},
			{
				name: "br",
				description: "- būsima reikšmė arba grynųjų balansas, kurio norite pasiekti po paskutinio mokėjimo. Jei šio argumento nėra, naudojamas nulis"
			},
			{
				name: "tipas",
				description: "- loginė reikšmė: mokėjimas laikotarpio pradžioje = 1; mokėjimas laikotarpio pabaigoje = 0 arba nėra argumento"
			}
		]
	},
	{
		name: "NPV",
		description: "Grąžina grynąją dabartinę investicijos reikšmę pagal nuolaidos koeficientą ir būsimų išlaidų (neigiamos reikšmės) bei įplaukų (teigiamos reikšmės) sekas.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "koeficientas",
				description: "- nuolaidos norma per vieną laikotarpį"
			},
			{
				name: "reikšmė1",
				description: "- nuo 1 iki 254 išlaidų ar įplaukų, gaunamų kiekvieno laikotarpio pabaigoje, reikšmių, tolygiai pasiskirsčiusių per tam tikrą laikotarpį"
			},
			{
				name: "reikšmė2",
				description: "- nuo 1 iki 254 išlaidų ar įplaukų, gaunamų kiekvieno laikotarpio pabaigoje, reikšmių, tolygiai pasiskirsčiusių per tam tikrą laikotarpį"
			}
		]
	},
	{
		name: "NUMBERVALUE",
		description: "Konvertuoja tekstą į skaičių nuo lokalės nepriklausančiu būdu.",
		arguments: [
			{
				name: "tekstas",
				description: "yra eilutė, vaizduojanti norimą konvertuoti skaičių"
			},
			{
				name: "dešimtainis_skyriklis",
				description: "yra simbolis, naudojamas kaip eilutės dešimtainis skyriklis"
			},
			{
				name: "grupės_skyriklis",
				description: "yra simbolis, naudojamas kaip eilutės grupės skyriklis"
			}
		]
	},
	{
		name: "OCT2BIN",
		description: "Aštuntainį skaičių konvertuoja į dvejetainį.",
		arguments: [
			{
				name: "skaičiai",
				description: "yra aštuntainis skaičius, kurį norite konvertuoti"
			},
			{
				name: "vietos",
				description: "yra simbolių, kuriuos norite naudoti, skaičius"
			}
		]
	},
	{
		name: "OCT2DEC",
		description: "Aštuntainį skaičių konvertuoja į dešimtainį.",
		arguments: [
			{
				name: "skaičius",
				description: "yra aštuntainis skaičius, kurį norite konvertuoti"
			}
		]
	},
	{
		name: "OCT2HEX",
		description: "Aštuntainį skaičių konvertuoja į šešioliktainį.",
		arguments: [
			{
				name: "skaičiai",
				description: "yra aštuntainis skaičius, kurį norite konvertuoti"
			},
			{
				name: "vietos",
				description: "yra simbolių, kuriuos norite naudoti, skaičius"
			}
		]
	},
	{
		name: "ODD",
		description: "Suapvalina teigiamą skaičių iki artimiausio didesnio, o neigiamą - iki artimiausio mažesnio nelyginio sveiko skaičiaus.",
		arguments: [
			{
				name: "skaičius",
				description: "- suapvalinama reikšmė"
			}
		]
	},
	{
		name: "OFFSET",
		description: "Grąžina nuorodą į diapazoną, kuris yra per nurodytą skaičių eilučių ir stulpelių nuo nurodytos nuorodos.",
		arguments: [
			{
				name: "nuoroda",
				description: "- nuoroda, nuo kurios skaičiuojamas poslinkis, nuoroda į langelį ar gretimų langelių diapazoną"
			},
			{
				name: "eilutės",
				description: "- eilučių, kurias turėtų nurodyti viršutinis kairysis rezultato langelis, skaičius viršuje ar apačioje"
			},
			{
				name: "stulpeliai",
				description: "- stulpelių, kuriuos turėtų nurodyti viršutinis kairysis rezultato langelis, skaičius kairėje ar dešinėje"
			},
			{
				name: "aukštis",
				description: "- norimas rezultato aukštis eilutėmis, toks pat kaip nuoroda, jei šis argumentas praleistas"
			},
			{
				name: "plotis",
				description: "-norimas rezultato plotis stulpeliais, toks pat kaip nuoroda, jei šis argumentas praleistas"
			}
		]
	},
	{
		name: "OR",
		description: "Tikrina, ar bent vienas argumentas yra TRUE, ir grąžina TRUE, jei taip, arba FALSE, jei visi argumentai yra FALSE.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "log_reikšmė1",
				description: "– nuo 1 iki 255 sąlygų, kurias norite patikrinti ir kurių reikšmės gali būti TRUE arba FALSE"
			},
			{
				name: "log_reikšmė2",
				description: "– nuo 1 iki 255 sąlygų, kurias norite patikrinti ir kurių reikšmės gali būti TRUE arba FALSE"
			}
		]
	},
	{
		name: "PARAMETER",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "PDURATION",
		description: "Pateikia investicijoms reikalingų laikotarpių skaičių, kad būtų pasiekta nurodyta vertė.",
		arguments: [
			{
				name: "norma",
				description: "yra palūkanų norma per laikotarpį."
			},
			{
				name: "dv",
				description: "yra dabartinė investicijų vertė"
			},
			{
				name: "bv",
				description: "yra siekiama investicijų vertė ateityje"
			}
		]
	},
	{
		name: "PEARSON",
		description: "Grąžina Pirsono sandaugos momentinės koreliacijos koeficientą - r.",
		arguments: [
			{
				name: "masyvas1",
				description: "- nepriklausomų reikšmių rinkinys"
			},
			{
				name: "masyvas2",
				description: "- priklausomų reikšmių rinkinys"
			}
		]
	},
	{
		name: "PERCENTILE",
		description: "Grąžina diapazono reikšmių k-ąjį procentilį.",
		arguments: [
			{
				name: "masyvas",
				description: "- duomenų masyvas arba diapazonas, kuris nustato santykinę padėtį"
			},
			{
				name: "k",
				description: "- procentilio reikšmė nuo 0 iki 1 imtinai"
			}
		]
	},
	{
		name: "PERCENTILE.EXC",
		description: "Grąžina diapazono reikšmių k-ąjį procentilį, kur k yra 0..1 diapazone neimtinai.",
		arguments: [
			{
				name: "masyvas",
				description: "- duomenų masyvas arba diapazonas, kuris nustato santykinę padėtį"
			},
			{
				name: "k",
				description: "- procentilio reikšmė nuo 0 iki 1 imtinai"
			}
		]
	},
	{
		name: "PERCENTILE.INC",
		description: "Grąžina diapazono reikšmių k-ąjį procentilį, kur k yra 0..1 diapazone imtinai.",
		arguments: [
			{
				name: "masyvas",
				description: "- duomenų masyvas arba diapazonas, kuris nustato santykinę padėtį"
			},
			{
				name: "k",
				description: "- procentilio reikšmė nuo 0 iki 1 imtinai"
			}
		]
	},
	{
		name: "PERCENTRANK",
		description: "Grąžina duomenų rinkinio reikšmės poziciją kaip duomenų rinkinio procentinį santykį.",
		arguments: [
			{
				name: "masyvas",
				description: "- duomenų masyvas arba diapazonas su skaitinėmis reikšmėmis, kurios nustato santykinę padėtį"
			},
			{
				name: "x",
				description: "- reikšmė, kurios pozicijos ieškoma"
			},
			{
				name: "reikšm",
				description: "- nebūtina reikšmė, kuri nustato ieškomo santykio reikšminių skaitmenų kiekį; jei nenurodyta, imami trys skaitmenys (0.xxx%)"
			}
		]
	},
	{
		name: "PERCENTRANK.EXC",
		description: "Grąžina duomenų rinkinio reikšmės poziciją kaip duomenų rinkinio procentinį santykį (0..1 neimtinai).",
		arguments: [
			{
				name: "masyvas",
				description: "- duomenų masyvas arba diapazonas su skaitinėmis reikšmėmis, kurios nustato santykinę padėtį"
			},
			{
				name: "x",
				description: "- reikšmė, kurios pozicijos ieškoma"
			},
			{
				name: "reikšm",
				description: "- nebūtina reikšmė, kuri nustato ieškomo santykio reikšminių skaitmenų kiekį; jei nenurodyta, imami trys skaitmenys (0.xxx%)"
			}
		]
	},
	{
		name: "PERCENTRANK.INC",
		description: "Grąžina duomenų rinkinio reikšmės poziciją kaip duomenų rinkinio procentinį santykį (0..1 imtinai).",
		arguments: [
			{
				name: "masyvas",
				description: "- duomenų masyvas arba diapazonas su skaitinėmis reikšmėmis, kurios nustato santykinę padėtį"
			},
			{
				name: "x",
				description: "- reikšmė, kurios pozicijos ieškoma"
			},
			{
				name: "reikšm",
				description: "- nebūtina reikšmė, kuri nustato ieškomo santykio reikšminių skaitmenų kiekį; jei nenurodyta, imami trys skaitmenys (0.xxx%)"
			}
		]
	},
	{
		name: "PERMUT",
		description: "Grąžina nurodyto objektų, kuriuos galima pasirinkti iš visų objektų, skaičiaus perstatų kiekį.",
		arguments: [
			{
				name: "skaičius",
				description: "- visų objektų skaičius"
			},
			{
				name: "pasirinktų_skaičius",
				description: "- objektų kiekvienoje perstatoje skaičius"
			}
		]
	},
	{
		name: "PERMUTATIONA",
		description: "Pateikia nurodyto objektų, kuriuos galima pasirinkti iš visų objektų, skaičiaus (su pasikartojimais) perstatų kiekį.",
		arguments: [
			{
				name: "skaičius",
				description: "yra bendras objektų skaičius"
			},
			{
				name: "pasirinktų_skaičius",
				description: "yra objektų kiekvienoje perstatoje skaičius"
			}
		]
	},
	{
		name: "PHI",
		description: "Pateikia tankio funkcijos reikšmę standariniam įprastam skirstiniui.",
		arguments: [
			{
				name: "x",
				description: "yra skaičius, kuriam norite taikyti standartinio įprasto skirstinio tankį"
			}
		]
	},
	{
		name: "PI",
		description: "Grąžina Pi reikšmę (3,14159265358979), suapvalintą penkiolikos ženklų tikslumu.",
		arguments: [
		]
	},
	{
		name: "PMT",
		description: "Skaičiuoja mokėjimą už paskolą pagal pastovius mokėjimus ir nekintančias palūkanas.",
		arguments: [
			{
				name: "norma",
				description: "- periodo palūkanų norma. Pavyzdžiui, naudokite 6%/4, jei kas ketvirtį mokama po 6% MPN"
			},
			{
				name: "laikot_sk",
				description: "- bendras mokėjimų už paskolą skaičius"
			},
			{
				name: "dr",
				description: "- dabartinė reikšmė arba bendra dabartinė būsimų mokėjimų vertė"
			},
			{
				name: "br",
				description: "- būsima reikšmė arba grynųjų balansas, kurio norite pasiekti po paskutinio mokėjimo; jei šio argumento nėra, naudojamas nulis"
			},
			{
				name: "tipas",
				description: "- loginė reikšmė: mokėjimas laikotarpio pradžioje = 1; mokėjimas laikotarpio pabaigoje = 0 arba nėra argumento"
			}
		]
	},
	{
		name: "POISSON",
		description: "Grąžina Puasono skirstinį.",
		arguments: [
			{
				name: "x",
				description: "- įvykių kiekis"
			},
			{
				name: "vidurkis",
				description: "- laukiama skaitinė reikšmė, teigiamas skaičius"
			},
			{
				name: "integral",
				description: "- loginė reikšmė: skaičiuodami integralųjį Puasono skirstinį naudokite TRUE; skaičiuodami Puasono tikimybės masės funkciją naudokite FALSE"
			}
		]
	},
	{
		name: "POISSON.DIST",
		description: "Grąžina Puasono skirstinį.",
		arguments: [
			{
				name: "x",
				description: "- įvykių kiekis"
			},
			{
				name: "vidurkis",
				description: "- laukiama skaitinė reikšmė, teigiamas skaičius"
			},
			{
				name: "integral",
				description: "- loginė reikšmė: skaičiuodami integralųjį Puasono skirstinį naudokite TRUE; skaičiuodami Puasono tikimybės masės funkciją naudokite FALSE"
			}
		]
	},
	{
		name: "POWER",
		description: "Grąžina skaičiaus kėlimo tam tikru laipsniu rezultatą.",
		arguments: [
			{
				name: "skaičius",
				description: "- pagrindo skaičius - bet koks realusis skaičius"
			},
			{
				name: "laipsnis",
				description: "- laipsnio, kuriuo reikia kelti, rodiklis"
			}
		]
	},
	{
		name: "PPMT",
		description: "Grąžina pagrindinės sumos investicijos mokėjimą pagal periodinius, pastovius mokėjimus ir pastovią palūkanų normą.",
		arguments: [
			{
				name: "norma",
				description: "- palūkanų normą per laikotarpį. Pavyzdžiui, naudokite 6%/4, jei kas ketvirtį mokama po 6% MPN"
			},
			{
				name: "laikot",
				description: "- nurodo laikotarpį, kuris turi įeiti į diapazoną nuo 1 iki Laikot_sk"
			},
			{
				name: "laikot_sk",
				description: "- bendras investavimo mokėjimo laikotarpių skaičius"
			},
			{
				name: "dr",
				description: "- dabartinė reikšmė: bendra suma, kurios dabar verti būsimi mokėjimai"
			},
			{
				name: "br",
				description: "- būsima reikšmė arba grynųjų balansas, kurį reikia pasiekti po paskutinio mokėjimo. Jei nenurodyta, Br = 0"
			},
			{
				name: "tipas",
				description: "- loginė reikšmė, nurodanti mokėjimo laiką: laikotarpio pabaigoje = 0 arba nenurodoma, laikotarpio pradžioje = 1"
			}
		]
	},
	{
		name: "PRICEDISC",
		description: "Pateikia vertybinių popierių su nuolaida 100 litų nominaliosios vertės kainą.",
		arguments: [
			{
				name: "sudengimas",
				description: "yra vertybinių popierių sudengimo data, išreikšta datų sekos numeriu"
			},
			{
				name: "mok_term",
				description: "yra vertybinių popierių mokėjimo terminas, išreikštas datų sekos numeriu"
			},
			{
				name: "nuolaida",
				description: "yra vertybinių popierių nuolaidos koeficientas"
			},
			{
				name: "padengimas",
				description: "yra 100 litų nominaliosios vertės vertybinių popierių padengimo vertė"
			},
			{
				name: "pagrindas",
				description: "yra naudojamo dienų skaičiavimo pagrindo tipas"
			}
		]
	},
	{
		name: "PROB",
		description: "Grąžina tikimybę, kad diapazono reikšmės yra tarp dviejų ribų arba lygios apatinei ribai.",
		arguments: [
			{
				name: "x_diapazonas",
				description: "- skaitinių x reikšmių, su kuriomis yra susijusios tikimybės, diapazonas"
			},
			{
				name: "tikim_diapazonas",
				description: "- tikimybių rinkinys, susijęs su X_diapazono reikšmėmis, reikšmės nuo 0 iki 1, išskyrus 0"
			},
			{
				name: "apatinė_riba",
				description: "- reikšmės, kurios tikimybė reikalinga, apatinis rėžis"
			},
			{
				name: "viršutinė_riba",
				description: "- nebūtinas viršutinis reikšmės rėžis. Jei nenurodyta, PROB grąžina tikimybę, kad X_diapazono reikšmės lygios Apatinei_ribai"
			}
		]
	},
	{
		name: "PRODUCT",
		description: "Sudaugina visus skaičius, nurodytus kaip argumentus.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "– nuo 1 iki 255 skaičių, loginių reikšmių arba skaičių teksto formatu, kuriuos reikia sudauginti"
			},
			{
				name: "skaičius2",
				description: "– nuo 1 iki 255 skaičių, loginių reikšmių arba skaičių teksto formatu, kuriuos reikia sudauginti"
			}
		]
	},
	{
		name: "PROPER",
		description: "Keičia teksto eilutę, taikant didžiąsias ir mažąsias raides tinkamose vietose; pirma kiekvieno žodžio raidė tampa didžiąja, visos kitos - mažosiomis.",
		arguments: [
			{
				name: "tekstas",
				description: "- tekstas kabutėse, formulė, kuri grąžina tekstą, arba nuoroda į langelį su tekstu, kuriam reikia taikyti funkciją"
			}
		]
	},
	{
		name: "PV",
		description: "Grąžina dabartinę investicijos vertę: bendrą sumą, kurios dabar verta būsimų mokėjimų seka.",
		arguments: [
			{
				name: "norma",
				description: "- laikotarpio palūkanų norma. Pavyzdžiui, naudokite 6%/4, jei kas ketvirtį mokama po 6% MPN"
			},
			{
				name: "laikot_sk",
				description: "- bendras investavimo mokėjimų laikotarpių skaičius"
			},
			{
				name: "išl",
				description: "- mokėjimas per kiekvieną laikotarpį, kuris nesikeičia per visą investavimo laiką"
			},
			{
				name: "br",
				description: "- būsimoji reikšmė arba grynųjų balansas, kurį norite pasiekti po paskutinio mokėjimo"
			},
			{
				name: "tipas",
				description: "- loginė reikšmė: mokėjimas laikotarpio pradžioje = 1; mokėjimas laikotarpio pabaigoje = 0 arba argumento nėra"
			}
		]
	},
	{
		name: "QUARTILE",
		description: "Grąžina duomenų rinkinio kvartilį.",
		arguments: [
			{
				name: "masyvas",
				description: "- skaitinių reikšmių masyvas arba langelių diapazonas, kurio kvartilio reikšmę reikia apskaičiuoti"
			},
			{
				name: "kvar",
				description: "- skaičius: minimali reikšmė = 0; 1-asis kvartilis = 1; medianos reikšmė = 2; 3-asis kvartilis = 3; maksimali reikšmė = 4"
			}
		]
	},
	{
		name: "QUARTILE.EXC",
		description: "Grąžina duomenų rinkinio kvartilį, pagrįstą procentilio reikšmėmis nuo 0 iki 1 neimtinai.",
		arguments: [
			{
				name: "masyvas",
				description: "- skaitinių reikšmių masyvas arba langelių diapazonas, kurio kvartilio reikšmę reikia apskaičiuoti"
			},
			{
				name: "kvar",
				description: "- skaičius: minimali reikšmė = 0; 1-asis kvartilis = 1; medianos reikšmė = 2; 3-asis kvartilis = 3; maksimali reikšmė = 4"
			}
		]
	},
	{
		name: "QUARTILE.INC",
		description: "Grąžina duomenų rinkinio kvartilį, pagrįstą procentilio reikšmėmis nuo 0 iki 1 imtinai.",
		arguments: [
			{
				name: "masyvas",
				description: "- skaitinių reikšmių masyvas arba langelių diapazonas, kurio kvartilio reikšmę reikia apskaičiuoti"
			},
			{
				name: "kvar",
				description: "- skaičius: minimali reikšmė = 0; 1-asis kvartilis = 1; medianos reikšmė = 2; 3-asis kvartilis = 3; maksimali reikšmė = 4"
			}
		]
	},
	{
		name: "QUOTIENT",
		description: "Pateikia sveikąją dalybos dalį.",
		arguments: [
			{
				name: "skaitiklis",
				description: "yra dalinys"
			},
			{
				name: "vardiklis",
				description: "yra daliklis"
			}
		]
	},
	{
		name: "RADIANS",
		description: "Keičia laipsnius į radianus.",
		arguments: [
			{
				name: "kampas",
				description: "- kampas laipsniais, kurį reikia pakeisti į radianus"
			}
		]
	},
	{
		name: "RAND",
		description: "Grąžina atsitiktinį skaičių, didesnį arba lygų 0 ir mažesnį už 1, tolygiai paskirstytą (perskaičiuojant keičiasi).",
		arguments: [
		]
	},
	{
		name: "RANDBETWEEN",
		description: "Pateikia atsitiktinį skaičių, esantį tarp jūsų nurodytų skaičių.",
		arguments: [
			{
				name: "apatinė_riba",
				description: "yra mažiausias sveikasis skaičius, kurį pateiks RANDBETWEEN"
			},
			{
				name: "viršutinė_riba",
				description: "yra didžiausias sveikasis skaičius, kurį grąžins RANDBETWEEN"
			}
		]
	},
	{
		name: "RANGE",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "RANK",
		description: "Grąžina skaičiaus poziciją skaičių sąraše: skaičiaus dydį, lyginant su kitomis reikšmėmis sąraše.",
		arguments: [
			{
				name: "skaičius",
				description: "- skaičius, kurio poziciją reikia rasti"
			},
			{
				name: "nuor",
				description: "- skaičių sąrašo masyvas arba nuoroda į jį. Neskaitinių reikšmių nepaisoma"
			},
			{
				name: "tvarka",
				description: "- skaičius: pozicija mažėjimo tvarka surūšiuotame sąraše = 0 ar nenurodyta; pozicija didėjimo tvarka surūšiuotame sąraše = bet koks skaičius, nelygus nuliui"
			}
		]
	},
	{
		name: "RANK.AVG",
		description: "Grąžina skaičiaus poziciją skaičių sąraše: skaičiaus dydį, lyginant su kitomis reikšmėmis sąraše; jeigu daugiau kaip viena reikšmė turi tokią pačią poziciją, grąžinama vidutinė pozicija.",
		arguments: [
			{
				name: "skaičius",
				description: "- skaičius, kurio poziciją reikia rasti"
			},
			{
				name: "nuor",
				description: "- skaičių sąrašo masyvas arba nuoroda į jį. Neskaitinių reikšmių nepaisoma"
			},
			{
				name: "tvarka",
				description: "- skaičius: pozicija mažėjimo tvarka surūšiuotame sąraše = 0 ar nenurodyta; pozicija didėjimo tvarka surūšiuotame sąraše = bet koks skaičius, nelygus nuliui"
			}
		]
	},
	{
		name: "RANK.EQ",
		description: "Grąžina skaičiaus poziciją skaičių sąraše: skaičiaus dydį, lyginant su kitomis reikšmėmis sąraše; jeigu daugiau kaip viena reikšmė turi tokią pačią poziciją, grąžinama viršutinė to reikšmių rinkinio pozicija.",
		arguments: [
			{
				name: "skaičius",
				description: "- skaičius, kurio poziciją reikia rasti"
			},
			{
				name: "nuor",
				description: "- skaičių sąrašo masyvas arba nuoroda į jį. Neskaitinių reikšmių nepaisoma"
			},
			{
				name: "tvarka",
				description: "- skaičius: pozicija mažėjimo tvarka surūšiuotame sąraše = 0 ar nenurodyta; pozicija didėjimo tvarka surūšiuotame sąraše = bet koks skaičius, nelygus nuliui"
			}
		]
	},
	{
		name: "RATE",
		description: "Grąžina palūkanų normas per paskolos ar investicijos laikotarpį. Pavyzdžiui, naudokite 6%/4, jei kas ketvirtį mokama po 6% MPN.",
		arguments: [
			{
				name: "laikot_sk",
				description: "- bendras investicijos arba paskolos mokėjimų  skaičius"
			},
			{
				name: "išl",
				description: "- mokėjimai per kiekvieną laikotarpį, kurie nesikeičia per visą investavimo arba paskolos laiką"
			},
			{
				name: "dr",
				description: "- dabartinė reikšmė arba bendroji suma, kurios dabar verti būsimi mokėjimai"
			},
			{
				name: "br",
				description: "būsima reikšmė arba grynųjų pinigų balansas, kurio norite pasiekti po paskutinio mokėjimo. Jei šio argumento nėra, naudojama Br = 0"
			},
			{
				name: "tipas",
				description: "- loginė reikšmė: mokėjimas laikotarpio pradžioje = 1; mokėjimas laikotarpio pabaigoje = 0 arba nėra argumento"
			},
			{
				name: "siūloma",
				description: "- jūsų siūloma norma; jei nėra, Siūloma = 0.1 (10 procentų)"
			}
		]
	},
	{
		name: "RECEIVED",
		description: "Pateikia padengiant visiškai investuotus vertybinius popierius gautą sumą.",
		arguments: [
			{
				name: "sudengimas",
				description: "yra vertybinių popierių sudengimo data, išreikšta datų sekos numeriu"
			},
			{
				name: "mok_term",
				description: "yra vertybinių popierių mokėjimo terminas, išreikštas datų sekos numeriu"
			},
			{
				name: "investicija",
				description: "yra į vertybinius popierius investuota suma"
			},
			{
				name: "nuolaida",
				description: "yra vertybinių popierių nuolaidos koeficientas"
			},
			{
				name: "pagrindas",
				description: "yra naudojamas dienų skaičiavimo pagrindo tipas"
			}
		]
	},
	{
		name: "REPLACE",
		description: "Keičia teksto eilutės dalį kita teksto eilute.",
		arguments: [
			{
				name: "senas_tekstas",
				description: "- tekstas, kuriame reikia keisti kelis simbolius"
			},
			{
				name: "prad_num",
				description: "- simbolio, kurį reikia pakeisti Nauju_tekstu, pozicija Sename_tekste"
			},
			{
				name: "simb_kiekis",
				description: "- simbolių skaičius  Sename_tekste, kurį reikia keisti"
			},
			{
				name: "naujas_tekstas",
				description: "- tekstas, kuriuo bus pakeisti Seno_teksto simboliai"
			}
		]
	},
	{
		name: "REPT",
		description: "Kartoja tekstą tiek kartų, kiek nurodyta. Naudokite REPT norėdami užpildyti langelį keliomis teksto eilutės kopijomis.",
		arguments: [
			{
				name: "tekstas",
				description: "- tekstas, kurį reikia pakartoti"
			},
			{
				name: "kiek_kartų",
				description: "- teigiamas skaičius, nurodantis, kiek kartų reikia kartoti tekstą"
			}
		]
	},
	{
		name: "RIGHT",
		description: "Grąžina nurodytą simbolių skaičių iš teksto pabaigos.",
		arguments: [
			{
				name: "tekstas",
				description: "- teksto eilutė su simboliais, kuriuos reikia gauti"
			},
			{
				name: "simb_kiekis",
				description: "- nurodo, kiek simbolių turi būti grąžinta; jei nieko nenurodyta, grąžinamas vienas"
			}
		]
	},
	{
		name: "ROMAN",
		description: "Keičia arabiškus skaitmenis į romėniškus kaip tekstą.",
		arguments: [
			{
				name: "skaičius",
				description: " - arabiškas skaičius, kurį reikia konvertuoti"
			},
			{
				name: "forma",
				description: "- skaičius, nurodantis  reikalingą romėniško skaičiaus tipą"
			}
		]
	},
	{
		name: "ROUND",
		description: "Suapvalina skaičių iki nurodyto dešimtainių skaitmenų kiekio.",
		arguments: [
			{
				name: "skaičius",
				description: "- skaičius, kurį reikia suapvalinti"
			},
			{
				name: "dešimt_kiekis",
				description: "- dešimtainių skaitmenų kiekis, iki kurio reikia suapvalinti. Jei ši reikšmė neigiama, suapvalinama kairėje nuo dešimtainio kablelio; jei nulis - iki artimiausios sveikosios reikšmės"
			}
		]
	},
	{
		name: "ROUNDDOWN",
		description: "Suapvalina skaičių iki mažesnio.",
		arguments: [
			{
				name: "skaičius",
				description: "bet koks realusis skaičius, kurį reikia suapvalinti"
			},
			{
				name: "dešimtainių",
				description: "dešimtainių skaitmenų skaičius, iki kurio reikia suapvalinti. Jei argumentas neigiamas, suapvalinama kairėje nuo dešimtainio kablelio; jei nulis ar praleistas, suapvalina iki artimiausio sveikojo skaičiaus"
			}
		]
	},
	{
		name: "ROUNDUP",
		description: "Suapvalina skaičių iki didesnio.",
		arguments: [
			{
				name: "skaičius",
				description: "- bet koks realusis skaičius, kurį reikia suapvalinti"
			},
			{
				name: "dešimtainių",
				description: "dešimtainių skaitmenų skaičius, iki kurio reikia suapvalinti. Jei argumentas neigiamas, suapvalinama kairėje nuo dešimtainio kablelio; jei nulis ar praleistas, suapvalina iki artimiausio sveikojo skaičiaus"
			}
		]
	},
	{
		name: "ROW",
		description: "Grąžina nuorodos eilutės numerį.",
		arguments: [
			{
				name: "nuoroda",
				description: "- langelis arba vienas diapazonas, kurio eilutės numerį reikia sužinoti. Jei nėra, grąžinama eilutė, kurioje yra funkcija ROW"
			}
		]
	},
	{
		name: "ROWS",
		description: "Grąžina nuorodos arba masyvo eilučių skaičių.",
		arguments: [
			{
				name: "masyvas",
				description: "- masyvas, masyvo formulė arba nuoroda į langelių diapazoną, kurio eilučių skaičių reikia gauti"
			}
		]
	},
	{
		name: "RRI",
		description: "Pateikia atitinkamą investicijų augimo palūkanų normą.",
		arguments: [
			{
				name: "nper",
				description: "yra investicijų laikotarpių skaičius"
			},
			{
				name: "pv",
				description: "yra dabartinė investicijų vertė"
			},
			{
				name: "fv",
				description: "yra būsima investicijų vertė"
			}
		]
	},
	{
		name: "RSQ",
		description: "Grąžina Pirsono sandaugos momentinės koreliacijos koeficiento kvadratą nurodytuose duomenų taškuose.",
		arguments: [
			{
				name: "žinomi_y",
				description: "- duomenų taškų masyvas arba diapazonas, tai gali būti skaičiai arba vardai, masyvai ar nuorodos į skaičius"
			},
			{
				name: "žinomi_x",
				description: "- duomenų taškų masyvas arba diapazonas, tai gali būti skaičiai arba vardai, masyvai ar nuorodos į skaičius"
			}
		]
	},
	{
		name: "RTD",
		description: "Gauna realiojo laiko duomenis iš programos, kuri palaiko COM automatiką.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "progID",
				description: "- registruoto COM automatinio priedo ProgID vardas. Paimkite vardą į kabutes"
			},
			{
				name: "server",
				description: "- serverio, kuriame turi veikti priedas, vardas. Paimkite vardą į kabutes. Jei priedas veikia vietiniame diske, naudokite tuščią eilutę"
			},
			{
				name: "pavad1",
				description: "- nuo 1 iki 38 parametrų, nurodančių duomenų fragmentą"
			},
			{
				name: "pavad2",
				description: "- nuo 1 iki 38 parametrų, nurodančių duomenų fragmentą"
			}
		]
	},
	{
		name: "SEARCH",
		description: "Grąžina simbolio, ties kuriuo pirmą kartą aptiktas tam tikras simbolis ar teksto eilutė, numerį, skaičiuojant iš kairės į dešinę (nepriklausomai nuo raidžių registro).",
		arguments: [
			{
				name: "ieškomas_tekstas",
				description: "- tekstas, kurio ieškote. Klaustuką (?) ir žvaigždutę (*) galima naudoti kaip universalijas; norėdami rasti klaustuką ar žvaigždutę naudokite ~? ir ~*"
			},
			{
				name: "ieškos_tekstas",
				description: "- tekstas, kuriame ieškoma Ieškomo_teksto"
			},
			{
				name: "prad_num",
				description: "- Ieškos_teksto simbolio numeris iš kairės, nuo kurio ketinama pradėti iešką. Jei šio argumento nėra, naudojama 1"
			}
		]
	},
	{
		name: "SEC",
		description: "Pateikia kampo sekantą.",
		arguments: [
			{
				name: "skaičius",
				description: "yra kampas, kurio sekantą norite apskaičiuoti, radianais"
			}
		]
	},
	{
		name: "SECH",
		description: "Pateikia kampo hiperbolinį sekantą.",
		arguments: [
			{
				name: "skaičius",
				description: "yra kampas, kurio hiperbolinį sekantą norite apskaičiuoti, radianais"
			}
		]
	},
	{
		name: "SECOND",
		description: "Grąžina sekundes - skaičių nuo 0 iki 59.",
		arguments: [
			{
				name: "eilės_numeris",
				description: "- skaičius datos-laiko kodo formatu, naudojamu programos Spreadsheet, arba tekstas laiko formatu, pvz., 16:48:23"
			}
		]
	},
	{
		name: "SERIESSUM",
		description: "Pateikia pagal formulę apskaičiuotą laipsnio rodiklio sumą.",
		arguments: [
			{
				name: "x",
				description: "yra laipsnio rodiklio įvesties vertė"
			},
			{
				name: "n",
				description: "yra pradinis rodiklis, kuriuo norite pakelti x"
			},
			{
				name: "m",
				description: "kiekvieno paskesnio laipsnio rodiklio didinimo žingsnis"
			},
			{
				name: "koeficientai",
				description: "yra koeficientų, iš kurių dauginamas kiekvienas paskesnis x laipsnis, rinkinys"
			}
		]
	},
	{
		name: "SHEET",
		description: "Pateikia nurodyto lapo numerį.",
		arguments: [
			{
				name: "reikšmė",
				description: "yra lapo arba nuorodos, kurios lapo numerio norite, pavadinimas. Jeigu pateikiamas lapo, kuriame yra funkcija, praleistas skaičius"
			}
		]
	},
	{
		name: "SHEETS",
		description: "Pateikia nurodomų lapų skaičių.",
		arguments: [
			{
				name: "nuoroda",
				description: "yra nuoroda, kurios lapų skaičių norite žinoti. Jei pateikiamas darbaknygės, kurioje yra funkcija, praleistas lapų skaičius"
			}
		]
	},
	{
		name: "SIGN",
		description: "Grąžina skaičiaus ženklą: 1, jei skaičius teigiamas, nulį, jei skaičius - nulis, arba -1, jei skaičius neigiamas.",
		arguments: [
			{
				name: "skaičius",
				description: "- bet koks realusis skaičius"
			}
		]
	},
	{
		name: "SIN",
		description: "Grąžina kampo sinusą.",
		arguments: [
			{
				name: "skaičius",
				description: "- kampas, kurio sinusas skaičiuojamas, radianais. Laipsniai * PI()/180 = radianai"
			}
		]
	},
	{
		name: "SINH",
		description: "Grąžina skaičiaus hiperbolinį sinusą.",
		arguments: [
			{
				name: "skaičius",
				description: "- bet koks realusis skaičius"
			}
		]
	},
	{
		name: "SKEW",
		description: "Grąžina skirstinio asimetriškumą: skirstinio asimetrijos pagal jo vidurkį laipsnio charakteristiką.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "– nuo 1 iki 255 skaičių arba pavadinimų, masyvų ar nuorodų į skaičius, kurių asimetriškumą reikia apskaičiuoti"
			},
			{
				name: "skaičius2",
				description: "– nuo 1 iki 255 skaičių arba pavadinimų, masyvų ar nuorodų į skaičius, kurių asimetriškumą reikia apskaičiuoti"
			}
		]
	},
	{
		name: "SKEW.P",
		description: "Pateikia asimetrinį skirstinį, atsižvelgiant į visumą: skirstinio asimetrijos laipsnio apibūdinamas apie jo vidurkį.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "yra 1–254 skaičių ar pavadinimų, duomenų masyvų ar nuorodų su skaičiais, kuriems norite pritaikyti visumos asimetriją"
			},
			{
				name: "skaičius2",
				description: "yra 1–254 skaičių ar pavadinimų, duomenų masyvų ar nuorodų su skaičiais, kuriems norite pritaikyti visumos asimetriją"
			}
		]
	},
	{
		name: "SLN",
		description: "Grąžina tiesinę turto amortizacijos reikšmę per vieną laikotarpį.",
		arguments: [
			{
				name: "vertė",
				description: "- pradinė turto vertė"
			},
			{
				name: "likvidacinė_v",
				description: "- likvidacinė vertė turto eksploatavimo pabaigoje"
			},
			{
				name: "eksploatav",
				description: "- laikotarpių, per kuriuos amortizuojamas turtas, kiekis (kartais vadinama naudingu turto tarnavimu)"
			}
		]
	},
	{
		name: "SLOPE",
		description: "Grąžina tiesinės regresijos linijos nuolydį nurodytuose duomenų taškuose.",
		arguments: [
			{
				name: "žinomi_y",
				description: "- skaitinių priklausomų duomenų taškų masyvas ar langelių diapazonas, tai gali būti skaičiai arba vardai, masyvai ar nuorodos į skaičius"
			},
			{
				name: "žinomi_x",
				description: "- nepriklausomų duomenų taškų rinkinys, tai gali būti skaičiai arba vardai, masyvai ar nuorodos į skaičius"
			}
		]
	},
	{
		name: "SMALL",
		description: "Grąžina duomenų rinkinio k-ąją mažiausiąją reikšmę. Pvz., penktą pagal mažumą skaičių.",
		arguments: [
			{
				name: "masyvas",
				description: "- duomenų masyvas arba diapazonas, kurio k-ąją mažiausiąją reikšmę reikia nustatyti"
			},
			{
				name: "k",
				description: "- ieškomos reikšmės pozicija (nuo mažiausios reikšmės) masyve arba langelių diapazone"
			}
		]
	},
	{
		name: "SQRT",
		description: "Grąžina skaičiaus kvadratinę šaknį.",
		arguments: [
			{
				name: "skaičius",
				description: "- skaičius, kurio kvadratinė šaknis skaičiuojama"
			}
		]
	},
	{
		name: "SQRTPI",
		description: "Pateikia (skaičius * Pi) kvadratinę šaknį.",
		arguments: [
			{
				name: "skaičius",
				description: "yra skaičius, iš kurio dauginamas pi"
			}
		]
	},
	{
		name: "STANDARDIZE",
		description: "Grąžina normalizuotą reikšmę iš skirstinio, apibrėžtą pagal vidurkį ir standartinį nuokrypį.",
		arguments: [
			{
				name: "x",
				description: "- reikšmė, kurią reikia normalizuoti"
			},
			{
				name: "vidurkis",
				description: "- skirstinio aritmetinis vidurkis"
			},
			{
				name: "standart_nuokr",
				description: "- skirstinio standartinis nuokrypis, teigiamas skaičius"
			}
		]
	},
	{
		name: "STDEV",
		description: "Įvertina imties standartinį nuokrypį (imties loginių reikšmių ir teksto nepaisoma).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "– nuo 1 iki 255 skaičių, atitinkančių generalinės aibės imtį. Tai gali būti skaičiai arba nuorodos su skaičiais"
			},
			{
				name: "skaičius2",
				description: "– nuo 1 iki 255 skaičių, atitinkančių generalinės aibės imtį. Tai gali būti skaičiai arba nuorodos su skaičiais"
			}
		]
	},
	{
		name: "STDEV.P",
		description: "Skaičiuoja visos argumentų generalinės aibės standartinį nuokrypį (loginių reikšmių ir teksto nepaisoma).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "– nuo 1 iki 255 skaičių, atitinkančių generalinę aibę, kurie gali būti skaičiai arba nuorodos, kuriose yra skaičių"
			},
			{
				name: "skaičius2",
				description: "– nuo 1 iki 255 skaičių, atitinkančių generalinę aibę, kurie gali būti skaičiai arba nuorodos, kuriose yra skaičių"
			}
		]
	},
	{
		name: "STDEV.S",
		description: "Įvertina imties standartinį nuokrypį (imties loginių reikšmių ir teksto nepaisoma).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "– nuo 1 iki 255 skaičių, atitinkančių generalinės aibės imtį. Tai gali būti skaičiai arba nuorodos, kuriose yra skaičių"
			},
			{
				name: "skaičius2",
				description: "– nuo 1 iki 255 skaičių, atitinkančių generalinės aibės imtį. Tai gali būti skaičiai arba nuorodos, kuriose yra skaičių"
			}
		]
	},
	{
		name: "STDEVA",
		description: "Skaičiuoja imties, įskaitant logines reikšmes ir tekstą, standartinį nuokrypį. Tekstas ir loginė reikšmė FALSE turi reikšmę 0; loginė reikšmė TRUE turi reikšmę 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "reikšmė1",
				description: "– nuo 1 iki 255 reikšmių, atitinkančių generalinę aibę. Tai gali būti reikšmės, vardai, masyvai arba nuorodos į reikšmes"
			},
			{
				name: "reikšmė2",
				description: "– nuo 1 iki 255 reikšmių, atitinkančių generalinę aibę. Tai gali būti reikšmės, vardai, masyvai arba nuorodos į reikšmes"
			}
		]
	},
	{
		name: "STDEVP",
		description: "Skaičiuoja visos argumentų generalinės aibės standartinį nuokrypį (loginių reikšmių ir teksto nepaisoma).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "– nuo 1 iki 255 skaičių, atitinkančių generalinę aibę, kurie gali būti skaičiai arba nuorodos su skaičiais"
			},
			{
				name: "skaičius2",
				description: "– nuo 1 iki 255 skaičių, atitinkančių generalinę aibę, kurie gali būti skaičiai arba nuorodos su skaičiais"
			}
		]
	},
	{
		name: "STDEVPA",
		description: "Skaičiuoja visos generalinės aibės, įskaitant logines reikšmes ir tekstą, standartinį nuokrypį. Tekstas ir loginė reikšmė FALSE turi reikšmę 0; loginė reikšmė TRUE turi reikšmę 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "reikšmė1",
				description: "– nuo 1 iki 255 reikšmių, atitinkančių generalinę aibę. Tai gali būti reikšmės, vardai, masyvai arba nuorodos į reikšmes"
			},
			{
				name: "reikšmė2",
				description: "– nuo 1 iki 255 reikšmių, atitinkančių generalinę aibę. Tai gali būti reikšmės, vardai, masyvai arba nuorodos į reikšmes"
			}
		]
	},
	{
		name: "STEYX",
		description: "Grąžina kiekvienai x reikšmei regresijoje prognozuotos y reikšmės standartinę paklaidą.",
		arguments: [
			{
				name: "žinomi_y",
				description: "- priklausomų duomenų taškų masyvas arba diapazonas, tai gali būti skaičiai arba pavadinimai, masyvai ar nuorodos į skaičius"
			},
			{
				name: "žinomi_x",
				description: "- nepriklausomų duomenų taškų masyvas arba diapazonas, tai gali būti skaičiai arba pavadinimai, masyvai ar nuorodos į skaičius"
			}
		]
	},
	{
		name: "SUBSTITUTE",
		description: "Keičia turimą teksto eilutės fragmentą nauju tekstu.",
		arguments: [
			{
				name: "tekstas",
				description: "- tekstas arba nuoroda į langelį su tekstu, kuriame reikia pakeisti simbolius"
			},
			{
				name: "senas_tekstas",
				description: "- tekstas, kurį reikia pakeisti. Jei Senas_tekstas turi kitokį raidžių registrą, SUBSTITUTE jo nekeis"
			},
			{
				name: "naujas_tekstas",
				description: "- tekstas, kuriuo reikia pakeisti Senas_tekstas"
			},
			{
				name: "fragmento_num",
				description: "- nurodo, kurį Senas_tekstas fragmentą reikia pakeisti. Jei nenurodyta, keičiami visi Senas_tekstas fragmentai"
			}
		]
	},
	{
		name: "SUBTOTAL",
		description: "Grąžina sąrašo ar duomenų bazės tarpinę sumą.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "funkcijos_num",
				description: "– skaičius nuo 1 iki 11, nurodantis tarpinės sumos suvestinės funkciją."
			},
			{
				name: "nuor1",
				description: "– nuo 1 iki 254 diapazonų ar nuorodų, kurių tarpinę sumą reikia apskaičiuoti"
			}
		]
	},
	{
		name: "SUM",
		description: "Sudeda langelių diapazono skaičius.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "– nuo 1 iki 255 sudedamų skaičių. Langeliuose, įtrauktuose į argumentus, loginių reikšmių ir teksto nepaisoma"
			},
			{
				name: "skaičius2",
				description: "– nuo 1 iki 255 sudedamų skaičių. Langeliuose, įtrauktuose į argumentus, loginių reikšmių ir teksto nepaisoma"
			}
		]
	},
	{
		name: "SUMIF",
		description: "Sudeda langelius, nurodytus pagal nurodytą sąlygą arba kriterijų.",
		arguments: [
			{
				name: "diapazonas",
				description: "- langelių, kuriuos norite įvertinti, diapazonas"
			},
			{
				name: "kriterijus",
				description: "- sąlyga arba kriterijus skaičiaus, išraiškos arba teksto, kuris nustato, kokie langeliai bus sudėti, pavidalu"
			},
			{
				name: "sum_diapazonas",
				description: "- langeliai, kurie turi būti sudėti. Jei nenurodyta, naudojami diapazono langeliai"
			}
		]
	},
	{
		name: "SUMIFS",
		description: "Sudeda langelius, kuriuos nurodo tam tikras sąlygų ar kriterijų rinkinys.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sumos_diapazonas",
				description: "langeliai, kurie turi būti sudėti."
			},
			{
				name: "kriterijų_diapazonas",
				description: "yra langelių, kuriuos norite įvertinti pagal tam tikrą sąlygą, diapazonas"
			},
			{
				name: "kriterijai",
				description: "yra sąlyga arba kriterijus, išreikštas skaičiumi, išraiška ar tekstu, kuris nustato, kurie langeliai bus sudedami"
			}
		]
	},
	{
		name: "SUMPRODUCT",
		description: "Grąžina atitinkamų diapazonų ar masyvų sandaugos sumą.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "masyvas1",
				description: "– nuo 2 iki 255 masyvų, kurių komponentus reikia sudauginti ir po to sudėti. Visi masyvai turi būti tų pačių dydžių"
			},
			{
				name: "masyvas2",
				description: "– nuo 2 iki 255 masyvų, kurių komponentus reikia sudauginti ir po to sudėti. Visi masyvai turi būti tų pačių dydžių"
			},
			{
				name: "masyvas3",
				description: "– nuo 2 iki 255 masyvų, kurių komponentus reikia sudauginti ir po to sudėti. Visi masyvai turi būti tų pačių dydžių"
			}
		]
	},
	{
		name: "SUMSQ",
		description: "Grąžina argumentų kvadratų sumą. Argumentai gali būti skaičiai, masyvai, pavadinimai arba nuorodos į langelius su skaičiais.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "– nuo 1 iki 255 skaičių arba pavadinimų, masyvų ar nuorodų į skaičius, kurių vidurkį reikia apskaičiuoti"
			},
			{
				name: "skaičius2",
				description: "– nuo 1 iki 255 skaičių arba pavadinimų, masyvų ar nuorodų į skaičius, kurių vidurkį reikia apskaičiuoti"
			}
		]
	},
	{
		name: "SUMX2MY2",
		description: "Sudeda dviejų atitinkančių masyvų diapazonų kvadratų skirtumus.",
		arguments: [
			{
				name: "masyvas_x",
				description: "- pirmas reikšmių diapazonas arba masyvas. Tai gali būti skaičius ar vardas, masyvas arba nuoroda į skaičius"
			},
			{
				name: "masyvas_y",
				description: "- antras reikšmių diapazonas arba masyvas. Tai gali būti skaičius ar vardas, masyvas arba nuoroda į skaičius"
			}
		]
	},
	{
		name: "SUMX2PY2",
		description: "Grąžina dviejuose atitinkančiuose vienas kitą masyvuose ar diapazonuose esančių skaičių kvadratų sumų bendrą sumą.",
		arguments: [
			{
				name: "masyvas_x",
				description: "- pirmas reikšmių diapazonas arba masyvas. Tai gali būti skaičius ar pavadinimas, masyvas arba nuoroda į skaičius"
			},
			{
				name: "masyvas_y",
				description: "- antras reikšmių diapazonas arba masyvas. Tai gali būti skaičius ar pavadinimas, masyvas arba nuoroda į skaičius"
			}
		]
	},
	{
		name: "SUMXMY2",
		description: "Sudeda dviejų atitinkančių masyvų diapazonų skirtumų kvadratus.",
		arguments: [
			{
				name: "masyvas_x",
				description: "- pirmas reikšmių diapazonas arba masyvas. Tai gali būti skaičius ar vardas, masyvas arba nuoroda į skaičius"
			},
			{
				name: "masyvas_y",
				description: "- antras reikšmių diapazonas arba masyvas. Tai gali būti skaičius ar vardas, masyvas arba nuoroda į skaičius"
			}
		]
	},
	{
		name: "SYD",
		description: "Grąžina turto amortizacijos vertę per nurodytą laikotarpį pagal eksploatavimo metų sumos metodą.",
		arguments: [
			{
				name: "vertė",
				description: "- pradinė turto vertė"
			},
			{
				name: "likvidacinė_v",
				description: "- likvidacinė vertė turto eksploatavimo pabaigoje"
			},
			{
				name: "eksploatav",
				description: "- laikotarpių, per kuriuos amortizuojamas turtas, skaičius (kartais vadinama naudingu turto naudojimo laikotarpiu)"
			},
			{
				name: "laikotarp",
				description: "- laikotarpis, kuris turi būti išreikštas tais pačiais vienetais kaip ir Eksploatavimo laikotarpis"
			}
		]
	},
	{
		name: "T",
		description: "Tikrina, ar reikšmė yra tekstas, ir grąžina tekstą, jei taip yra, arba dvi kabutes (tuščią tekstą), jei ne.",
		arguments: [
			{
				name: "reikšmė",
				description: "- tikrinama reikšmė"
			}
		]
	},
	{
		name: "T.DIST",
		description: "Grąžina kairės pusės Studento t skirstinį.",
		arguments: [
			{
				name: "x",
				description: "- skaitinė reikšmė, pagal kurią reikia skaičiuoti skirstinį"
			},
			{
				name: "laisv_laips",
				description: "- sveikasis skaičius, nurodantis, kiek laisvės laipsnių apibrėžia skirstinį"
			},
			{
				name: "integral",
				description: "loginė reikšmė: integraliajai skirstinio funkcijai naudokite TRUE; tikimybės tankio funkcijai naudokite FALSE"
			}
		]
	},
	{
		name: "T.DIST.2T",
		description: "Grąžina dvipusį Studento t skirstinį.",
		arguments: [
			{
				name: "x",
				description: "- skaitinė reikšmė, pagal kurią reikia skaičiuoti skirstinį"
			},
			{
				name: "laisv_laips",
				description: "sveikasis skaičius, nurodantis, kiek laisvės laipsnių apibrėžia skirstinį"
			}
		]
	},
	{
		name: "T.DIST.RT",
		description: "Grąžina dešinės pusės Studento t skirstinį.",
		arguments: [
			{
				name: "x",
				description: "skaitinė reikšmė, pagal kurią reikia skaičiuoti skirstinį"
			},
			{
				name: "laisv_laips",
				description: "sveikasis skaičius, nurodantis, kiek laisvės laipsnių apibrėžia skirstinį"
			}
		]
	},
	{
		name: "T.INV",
		description: "Grąžina atvirkštinį kairės pusės Studento t skirstinį.",
		arguments: [
			{
				name: "tikimybė",
				description: "- tikimybė, susijusi su dvipusiu Studento t skirstiniu, skaičius nuo 0 iki 1 imtinai"
			},
			{
				name: "laisv_laipsnis",
				description: "- teigiamas sveikasis skaičius, nurodantis skirstinį apibūdinantį laisvės laipsnių skaičių"
			}
		]
	},
	{
		name: "T.INV.2T",
		description: "Grąžina atvirkštinį dvipusį Studento t skirstinį.",
		arguments: [
			{
				name: "tikimybė",
				description: "- tikimybė, susijusi su dvipusiu Studento t skirstiniu, skaičius nuo 0 iki 1 imtinai"
			},
			{
				name: "laisv_laips",
				description: "- teigiamas sveikasis skaičius, nurodantis skirstinį apibūdinantį laisvės laipsnių skaičių"
			}
		]
	},
	{
		name: "T.TEST",
		description: "Grąžina tikimybę, susijusią su Studento t-testu.",
		arguments: [
			{
				name: "masyvas1",
				description: "- pirmas duomenų rinkinys"
			},
			{
				name: "masyvas2",
				description: "- antras duomenų rinkinys"
			},
			{
				name: "pusės",
				description: "- nurodoma, kiek skirstinio pusių reikia grąžinti: vienpusis skirstinys = 1; dvipusis skirstinys = 2"
			},
			{
				name: "tipas",
				description: "- t-testo tipas: suporintas = 1, dviejų imčių lygioji dispersija (homoskedastinis) = 2, dviejų imčių nelygioji dispersija = 3"
			}
		]
	},
	{
		name: "TAN",
		description: "Grąžina kampo tangentą.",
		arguments: [
			{
				name: "skaičius",
				description: "- radianais išreikštas kampas, kurio tangentas skaičiuojamas. Laipsniai * PI()/180 = radianai"
			}
		]
	},
	{
		name: "TANH",
		description: "Grąžina skaičiaus hiperbolinį tangentą.",
		arguments: [
			{
				name: "skaičius",
				description: "- bet koks realusis skaičius"
			}
		]
	},
	{
		name: "TBILLEQ",
		description: "Pateikia iždo vekselių obligacijų ekvivalento pelningumą.",
		arguments: [
			{
				name: "sudengimas",
				description: "yra iždo vekselių sudengimo data, išreikšta datų sekos numeriu"
			},
			{
				name: "mok_term",
				description: "yra iždo vekselių mokėjimo terminas, išreikštas datų sekos numeriu"
			},
			{
				name: "nuolaida",
				description: "yra iždo vekselių nuolaidos koeficientas"
			}
		]
	},
	{
		name: "TBILLPRICE",
		description: "Pateikia 100 litų nominaliosios vertės iždo vekselių kainą.",
		arguments: [
			{
				name: "sudengimas",
				description: "yra iždo vekselių sudengimo data, išreikšta datų sekos numeriu"
			},
			{
				name: "mok_term",
				description: "yra iždo vekselių mokėjimo terminas, išreikštas datų sekos numeriu"
			},
			{
				name: "nuolaida",
				description: "yra iždo vekselių nuolaidos koeficientas"
			}
		]
	},
	{
		name: "TBILLYIELD",
		description: "Pateikia iždo vekselių pelningumą.",
		arguments: [
			{
				name: "sudengimas",
				description: "yra iždo vekselių sudengimo data, išreikšta datų sekos numeriu"
			},
			{
				name: "mok_term",
				description: "yra iždo vekselių mokėjimo terminas, išreikštas datų sekos numeriu"
			},
			{
				name: "kaina",
				description: "yra iždo vekselių kaina už 100 litų nominaliosios vertės vertybinių popierių"
			}
		]
	},
	{
		name: "TDIST",
		description: "Pateikia Studento t-skirstinį.",
		arguments: [
			{
				name: "x",
				description: "- skaitinė reikšmė, pagal kurią reikia skaičiuoti skirstinį"
			},
			{
				name: "laisv_laips",
				description: "- sveikasis skaičius, nurodantis, kiek laisvės laipsnių apibrėžia skirstinį"
			},
			{
				name: "pusės",
				description: "- nurodo grąžinamų skirstinio pusių skaičių: vienpusis skirstinys = 1; dvipusis skirstinys = 2"
			}
		]
	},
	{
		name: "TEXT",
		description: "Konvertuoja reikšmę į tekstą tam tikru skaičiaus formatu.",
		arguments: [
			{
				name: "reikšmė",
				description: "- skaičius, formulė, kurios rezultatas - skaitinė reikšmė arba nuoroda į langelį su skaitine reikšme"
			},
			{
				name: "formato_tekstas",
				description: "- skaičiaus formatas teksto pavidalu, pasirinktas iš dialogo lango Langelių formatavimas skirtuko Skaičius laukelio Kategorija (ne Bendrasis)"
			}
		]
	},
	{
		name: "TIME",
		description: "Konvertuoja valandas, minutes ir sekundes, įrašytas kaip skaičius, į Spreadsheet eilės numerį, pritaikant laiko formatą.",
		arguments: [
			{
				name: "valandos",
				description: "- skaičius nuo 0 iki 23, rodantis valandas"
			},
			{
				name: "minutės",
				description: "- skaičius nuo 0 iki 59, rodantis minutes"
			},
			{
				name: "sekundės",
				description: "- skaičius nuo 0 iki 59, rodantis sekundes"
			}
		]
	},
	{
		name: "TIMEVALUE",
		description: "Keičia teksto formatu nurodytą laiką Spreadsheet laiko eilės numeriu nuo 0 (12:00:00 AM) iki 0,999988426 (11:59:59 PM). Įvedę formulę nustatykite jos laiko formatą.",
		arguments: [
			{
				name: "laiko_tekstas",
				description: "- teksto eilutė, nurodanti laiką bet kokiu Spreadsheet laiko formatu (datos informacijos eilutėje nepaisoma)"
			}
		]
	},
	{
		name: "TINV",
		description: "Grąžina dvipusį atvirkštinį Studento t skirstinį.",
		arguments: [
			{
				name: "tikimybė",
				description: "- tikimybė, susijusi su dvipusiu Studento t skirstiniu, skaičius nuo 0 iki 1 imtinai"
			},
			{
				name: "laisv_laips",
				description: "- teigiamas sveikasis skaičius, nurodantis skirstinį apibūdinantį laisvės laipsnių skaičių"
			}
		]
	},
	{
		name: "TODAY",
		description: "Pateikia šios dienos  datą datos formatu.",
		arguments: [
		]
	},
	{
		name: "TRANSPOSE",
		description: "Keičia vertikalųjį diapazoną į horizontalųjį ir atvirkščiai.",
		arguments: [
			{
				name: "masyvas",
				description: "- darbalapio langelių diapazonas arba reikšmių masyvas, kurį reikia keisti"
			}
		]
	},
	{
		name: "TREND",
		description: "Grąžina krypties linijos skaičius, atitinkančius žinomus duomenų taškus, taikant mažiausių kvadratų metodą.",
		arguments: [
			{
				name: "žinomi_y",
				description: "- y reikšmių diapazonas arba masyvas, jau žinomas iš y = mx + b"
			},
			{
				name: "žinomi_x",
				description: "- nebūtinas x reikšmių diapazonas arba masyvas, jau žinomas iš y = mx + b; masyvas yra tokio paties dydžio kaip ir Žinomi_y"
			},
			{
				name: "nauji_x",
				description: "- naujų x reikšmių, kurių atitinkamas y reikšmes turi grąžinti TREND, diapazonas arba masyvas"
			},
			{
				name: "konst",
				description: "- loginė reikšmė: konstanta b skaičiuojama įprastai, jei Konst = TRUE arba argumento nėra; b prilyginama 0, jei Konst = FALSE"
			}
		]
	},
	{
		name: "TRIM",
		description: "Iš teksto eilutės šalina visus tarpus, išskyrus viengubus tarpus tarp žodžių.",
		arguments: [
			{
				name: "tekstas",
				description: "- tekstas, kurio tarpus reikia šalinti"
			}
		]
	},
	{
		name: "TRIMMEAN",
		description: "Grąžina duomenų reikšmių rinkinio vidinės dalies vidurkį.",
		arguments: [
			{
				name: "masyvas",
				description: "- reikšmių masyvas arba diapazonas, kuris turi būti sutrumpintas ir kurio vidurkis turi būti nustatytas"
			},
			{
				name: "procentas",
				description: "- duomenų taškų, išimamų iš rinkinio viršaus ir apačios, trupmeninis skaičius"
			}
		]
	},
	{
		name: "TRUE",
		description: "Grąžina loginę reikšmę TRUE.",
		arguments: [
		]
	},
	{
		name: "TRUNC",
		description: "Suapvalina skaičių iki sveikojo, pašalindama dešimtainę ar trupmenos dalį.",
		arguments: [
			{
				name: "skaičius",
				description: "- skaičius, kurį reikia suapvalinti"
			},
			{
				name: "dešimt_kiekis",
				description: "- skaičius, nurodantis suapvalinimo tikslumą; 0 (nulis), jei nenurodyta"
			}
		]
	},
	{
		name: "TTEST",
		description: "Grąžina tikimybę, susijusią su Studento t-testu.",
		arguments: [
			{
				name: "masyvas1",
				description: "- pirmas duomenų rinkinys"
			},
			{
				name: "masyvas2",
				description: "- antras duomenų rinkinys"
			},
			{
				name: "pusės",
				description: "- nurodoma, kiek pusių reikia grąžinti: vienpusis skirstinys = 1; dvipusis skirstinys = 2"
			},
			{
				name: "tipas",
				description: "- t-testo tipas: suporintas = 1, dviejų imčių lygioji dispersija (homoskedastinis) = 2, dviejų imčių nelygioji dispersija = 3"
			}
		]
	},
	{
		name: "TYPE",
		description: "Grąžina sveiką skaičių, rodantį reikšmės duomenų tipą: skaičius = 1; tekstas = 2; loginė reikšmė = 4; klaidos reikšmė = 16; masyvas = 64.",
		arguments: [
			{
				name: "reikšmė",
				description: "- bet kokia reikšmė"
			}
		]
	},
	{
		name: "UNICODE",
		description: "Pateikia skaičių (simbolio kodą), atitinkantį pirmąjį teksto simbolį.",
		arguments: [
			{
				name: "tekstas",
				description: "yra simbolis, kurio „Unicode“ reikšmė reikalinga"
			}
		]
	},
	{
		name: "UPPER",
		description: "Pakeičia visas teksto eilutės raides didžiosiomis.",
		arguments: [
			{
				name: "tekstas",
				description: "- tekstas, kurį reikia konvertuoti. Simboliai, kurie nėra raidės, nekonvertuojami"
			}
		]
	},
	{
		name: "VALUE",
		description: "Konvertuoja teksto eilutę, atitinkančią skaičių, į skaičių.",
		arguments: [
			{
				name: "tekstas",
				description: "- tekstas tarp kabučių arba nuoroda į langelį, turintį tekstą, kurį reikia konvertuoti"
			}
		]
	},
	{
		name: "VAR",
		description: "Skaičiuoja imties dispersiją (imties loginių reikšmių ir teksto nepaisoma).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "– nuo 1 iki 255 skaitinių argumentų, atitinkančių generalinės aibės imtį"
			},
			{
				name: "skaičius2",
				description: "– nuo 1 iki 255 skaitinių argumentų, atitinkančių generalinės aibės imtį"
			}
		]
	},
	{
		name: "VAR.P",
		description: "Skaičiuoja visos generalinės aibės dispersiją (loginių reikšmių ir teksto aibėje nepaisoma).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "– nuo 1 iki 255 skaitinių argumentų, atitinkančių generalinę aibę"
			},
			{
				name: "skaičius2",
				description: "– nuo 1 iki 255 skaitinių argumentų, atitinkančių generalinę aibę"
			}
		]
	},
	{
		name: "VAR.S",
		description: "Skaičiuoja imties dispersiją (imties loginių reikšmių ir teksto nepaisoma).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "– nuo 1 iki 255 skaitinių argumentų, atitinkančių generalinės aibės imtį"
			},
			{
				name: "skaičius2",
				description: "– nuo 1 iki 255 skaitinių argumentų, atitinkančių generalinės aibės imtį"
			}
		]
	},
	{
		name: "VARA",
		description: "Skaičiuoja imties, įskaitant logines reikšmes ir tekstą, dispersiją. Tekstas ir loginė reikšmė FALSE turi reikšmę 0; loginė reikšmė TRUE turi reikšmę 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "reikšmė1",
				description: "– nuo 1 iki 255 reikšmių, atitinkančių generalinę aibę. Tai gali būti reikšmės, vardai, masyvai arba nuorodos į reikšmes"
			},
			{
				name: "reikšmė2",
				description: "– nuo 1 iki 255 reikšmių, atitinkančių generalinę aibę. Tai gali būti reikšmės, vardai, masyvai arba nuorodos į reikšmes"
			}
		]
	},
	{
		name: "VARP",
		description: "Skaičiuoja visos generalinės aibės dispersiją (loginių reikšmių ir teksto aibėje nepaisoma).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "skaičius1",
				description: "– nuo 1 iki 255 skaitinių argumentų, atitinkančių generalinę aibę"
			},
			{
				name: "skaičius2",
				description: "– nuo 1 iki 255 skaitinių argumentų, atitinkančių generalinę aibę"
			}
		]
	},
	{
		name: "VARPA",
		description: "Skaičiuoja visos generalinės aibės, įskaitant logines reikšmes ir tekstą, dispersiją. Tekstas ir loginė reikšmė FALSE turi reikšmę 0; loginė reikšmė TRUE turi reikšmę 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "reikšmė1",
				description: "– nuo 1 iki 255 reikšmių, atitinkančių generalinę aibę. Tai gali būti reikšmės, vardai, masyvai arba nuorodos į reikšmes"
			},
			{
				name: "reikšmė2",
				description: "– nuo 1 iki 255 reikšmių, atitinkančių generalinę aibę. Tai gali būti reikšmės, vardai, masyvai arba nuorodos į reikšmes"
			}
		]
	},
	{
		name: "VDB",
		description: "Grąžina turto amortizacijos reikšmę per bet kokį laikotarpį, įskaitant dalinius laikotarpius, taikant dvigubo mažėjimo balanso ar kokį nors kitą jūsų nurodytą metodą.",
		arguments: [
			{
				name: "vertė",
				description: "- pradinė turto vertė"
			},
			{
				name: "likvidacinė_v",
				description: "- likvidacinė vertė turto eksploatavimo pabaigoje"
			},
			{
				name: "eksploatav",
				description: "- laikotarpių, per kuriuos amortizuojamas turtas, kiekis (kartais vadinama naudingu turto naudojimo laikotarpiu)"
			},
			{
				name: "pradžios_laikot",
				description: "- pradžios laikotarpis, kurio amortizaciją reikia apskaičiuoti, išreikštas tais pačiais vienetais kaip ir Eksploatavimo laikotarpis"
			},
			{
				name: "pabaigos_laikot",
				description: "- pabaigos laikotarpis, kurio amortizaciją reikia apskaičiuoti, išreikštas tais pačiais vienetais kaip ir Eksploatavimo laikotarpis"
			},
			{
				name: "koeficientas",
				description: "- balanso mažėjimo koeficientas. Jei koeficientas nenurodytas, laikoma, kad jis lygus 2 (dvigubo mažėjimo balanso metodas)"
			},
			{
				name: "perjungiklis",
				description: "- persijungti į tiesinės amortizacijos metodą, kai amortizacijos reikšmė didesnė už mažėjimo balansą, = FALSE ar nenurodoma; nepersijungti = TRUE"
			}
		]
	},
	{
		name: "VLOOKUP",
		description: "Ieško kairiajame kraštiniame lentelės stulpelyje tam tikros reikšmės ir grąžina reikšmę, esančią jūsų nurodyto stulpelio toje pačioje eilutėje. Numatyta, kad lentelė turi būti surūšiuota didėjimo tvarka.",
		arguments: [
			{
				name: "ieškos_reikšmė",
				description: "- reikšmė, kurią reikia rasti pirmame lentelės stulpelyje; tai gali būti reikšmė, nuoroda arba teksto eilutė"
			},
			{
				name: "lentelė_masyvas",
				description: "- tekstų, skaičių ar loginių reikšmių lentelė, kurioje vykdoma ieška. Lentelė_masyvas gali būti nuoroda į diapazoną arba diapazono pavadinimas"
			},
			{
				name: "stulp_indekso_num",
				description: "- lentelės arba masyvo (lentelė_masyvas) stulpelio, iš kurio grąžinama reikšmė, numeris. Pirmas lentelės reikšmių stulpelis yra 1"
			},
			{
				name: "diapaz_ieškoti",
				description: "- loginė reikšmė: norėdami rasti pirmame stulpelyje (surūšiuotame didėjimo tvarka) panašiausią reikšmę, nurodykite TRUE arba nieko; norėdami rasti tikslų sutapimą, nurodykite FALSE"
			}
		]
	},
	{
		name: "WEEKDAY",
		description: "Grąžina skaičių nuo 1iki 7, identifikuojantį datos savaitės dieną.",
		arguments: [
			{
				name: "eilės_numeris",
				description: "- skaičius, reiškiantis datą"
			},
			{
				name: "grąžina_tipą",
				description: "- skaičius, reiškiantis: kai Sekmadienis=1 ir Šeštadienis=7, naudoti 1; kai Pirmadienis=1 ir Sekmadienis=7, naudoti 2; kai Pirmadienis =0 ir Šeštadienis=6, naudoti 3"
			}
		]
	},
	{
		name: "WEEKNUM",
		description: "Pateikia metų savaitės numerį.",
		arguments: [
			{
				name: "eilės_numeris",
				description: "yra datos-laiko kodas, kurį naudoja Spreadsheet datai ir laikui skaičiuoti"
			},
			{
				name: "grąžina_tipą",
				description: "yra skaičius (1 arba 2), kuris nurodo pateikiamos vertės tipą"
			}
		]
	},
	{
		name: "WEIBULL",
		description: "Grąžina Veibulio skirstinį.",
		arguments: [
			{
				name: "x",
				description: "- reikšmė, pagal kurią reikia apskaičiuoti skirstinį, ne neigiamas skaičius"
			},
			{
				name: "alfa",
				description: "- skirstinio parametras, teigiamas skaičius"
			},
			{
				name: "beta",
				description: "- skirstinio parametras, teigiamas skaičius"
			},
			{
				name: "integral",
				description: "- loginė reikšmė: skaičiuodami integraliąją skirstinio funkciją naudokite TRUE; skaičiuodami tikimybės masės funkciją naudokite FALSE"
			}
		]
	},
	{
		name: "WEIBULL.DIST",
		description: "Grąžina Veibulio skirstinį.",
		arguments: [
			{
				name: "x",
				description: "- reikšmė, pagal kurią reikia apskaičiuoti funkciją, ne neigiamas skaičius"
			},
			{
				name: "alfa",
				description: "- skirstinio parametras, teigiamas skaičius"
			},
			{
				name: "beta",
				description: "- skirstinio parametras, teigiamas skaičius"
			},
			{
				name: "integral",
				description: "- loginė reikšmė: skaičiuodami integraliąją skirstinio funkciją naudokite TRUE; skaičiuodami tikimybės masės funkciją naudokite FALSE"
			}
		]
	},
	{
		name: "WORKDAY",
		description: "Pateikia datos, esančios prieš ar po nurodyto darbo dienų skaičiaus, serijos numerį.",
		arguments: [
			{
				name: "pradžios_data",
				description: "yra serijos datų sekos numeris, išreiškiantis pradžios datą"
			},
			{
				name: "dienų_skaičius",
				description: "yra ne savaitgalių ir ne švenčių dienų skaičius prieš arba po pradžios_datos"
			},
			{
				name: "šventės",
				description: "yra pasirinktinis vienos ar kelių datų sekos numerių, kurie bus išbraukti iš darbo kalendoriaus, sąrašas, pvz., valstybinės šventės"
			}
		]
	},
	{
		name: "WORKDAY.INTL",
		description: "Pateikia datos, esančios prieš arba po nurodytų darbo dienų skaičiaus, sekos numerį su pasirinktiniais savaitgalio parametrais.",
		arguments: [
			{
				name: "pradžios_data",
				description: "yra datų sekos numeris, kuris reiškia pradžios datą"
			},
			{
				name: "dienos",
				description: "yra ne savaitgalių ir ne švenčių dienų skaičius prieš arba po kintamojo pradžios_data"
			},
			{
				name: "savaitgalis",
				description: "yra numeris arba eilutė, nurodanti, kada būna savaitgaliai"
			},
			{
				name: "šventės",
				description: "yra pasirinktinis vienos ar kelių datų sekos numerių, kurie bus pašalinti iš darbo kalendoriaus, masyvas, pvz., valstybinės šventės arba šventės, kurių data nepastovi"
			}
		]
	},
	{
		name: "XIRR",
		description: "Pateikia pinigų srautų tvarkaraščio vidinių įplaukų normą.",
		arguments: [
			{
				name: "reikšmės",
				description: "yra pinigų srautų seka, atitinkanti mokėjimų tvarkaraštį pagal datas"
			},
			{
				name: "datos",
				description: "yra mokėjimo datų, kurios atitinka pinigų srautų mokėjimus, tvarkaraštis"
			},
			{
				name: "spėjimas",
				description: "yra skaičius, kuris gali būti artimas XIRR rezultatui"
			}
		]
	},
	{
		name: "XNPV",
		description: "Pateikia grynąją dabartinę pinigų srautų tvarkaraščio reikšmę.",
		arguments: [
			{
				name: "koeficientas",
				description: "yra nuolaidos, taikomos pinigų srautams, koeficientas"
			},
			{
				name: "reikšmės",
				description: "yra pinigų srautų seka, atitinkanti mokėjimų tvarkaraštį pagal datas"
			},
			{
				name: "datos",
				description: "yra mokėjimų datų, atitinkančių pinigų srauto mokėjimus, tvarkaraštis"
			}
		]
	},
	{
		name: "XOR",
		description: "Pateikia visų argumentų loginį „Išskirtinį Arba“.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "loginis1",
				description: "yra 1–254 sąlygų, kurias norite patikrinti, kurios gali būti arba TRUE arba FALSE ir gali būti loginės reikšmės, duomenų masyvai arba nuorodos"
			},
			{
				name: "loginis2",
				description: "yra 1–254 sąlygų, kurias norite patikrinti, kurios gali būti arba TRUE arba FALSE ir gali būti loginės reikšmės, duomenų masyvai arba nuorodos"
			}
		]
	},
	{
		name: "YEAR",
		description: "Grąžina datos metus - sveiką skaičių nuo 1900 iki 9999.",
		arguments: [
			{
				name: "eilės_numeris",
				description: "- skaičius datos-laiko kodo formatu, kurį naudoja programa Spreadsheet"
			}
		]
	},
	{
		name: "YEARFRAC",
		description: "Pateikia metų trupmeną, kuri reiškia sveikų dienų, esančių tarp pradžios_datos ir pabaigos_datos, skaičių.",
		arguments: [
			{
				name: "pradžios_data",
				description: "yra datų sekos numeris, kuris reiškia pradžios datą"
			},
			{
				name: "pabaigos_data",
				description: "yra datų sekos numeris, kuris reiškia pabaigos datą"
			},
			{
				name: "pagrindas",
				description: "yra naudojamo dienų skaičiavimo pagrindo tipas"
			}
		]
	},
	{
		name: "YIELDDISC",
		description: "Pateikia vertybinių popierių, kuriems taikoma nuolaida, metinį pelningumą. Pavyzdžiui, iždo vekselis.",
		arguments: [
			{
				name: "sudengimas",
				description: "yra vertybinių popierių sudengimo data, išreikšta datų sekos numeriu"
			},
			{
				name: "mok_term",
				description: "yra vertybinių popierių mokėjimo terminas, išreikštas datų sekos numeriu"
			},
			{
				name: "kaina",
				description: "yra 100 litų nominaliosios vertės vertybinių popierių kaina"
			},
			{
				name: "padengimas",
				description: "yra 100 litų nominaliosios vertės vertybinių popierių padengimo vertė"
			},
			{
				name: "pagrindas",
				description: "yra naudojamas dienų skaičiavimo pagrindo tipas"
			}
		]
	},
	{
		name: "Z.TEST",
		description: "Grąžina vienpusę P reikšmę iš z-testo.",
		arguments: [
			{
				name: "masyvas",
				description: "- duomenų masyvas arba diapazonas, pagal kurį bandoma X"
			},
			{
				name: "x",
				description: "– bandoma reikšmė"
			},
			{
				name: "sigma",
				description: "– generalinės aibės (žinomas) standartinis nuokrypis. Jei nenurodyta, naudojamas imties standartinis nuokrypis"
			}
		]
	},
	{
		name: "ZTEST",
		description: "Grąžina vienpusę P reikšmę iš z-testo.",
		arguments: [
			{
				name: "masyvas",
				description: " duomenų masyvas arba diapazonas, pagal kurį bandoma X"
			},
			{
				name: "x",
				description: "– bandoma reikšmė"
			},
			{
				name: "sigma",
				description: "– generalinės aibės (žinomas) standartinis nuokrypis. Jei nenurodyta, naudojamas imties standartinis nuokrypis"
			}
		]
	}
];