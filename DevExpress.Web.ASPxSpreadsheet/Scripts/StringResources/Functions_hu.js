ASPxClientSpreadsheet.Functions = [
	{
		name: "AB.ÁTLAG",
		description: "Egy lista- vagy adatbázisoszlopban lévő azon értékek átlagát számítja ki, melyek megfelelnek a megadott feltételeknek.",
		arguments: [
			{
				name: "adatbázis",
				description: "a listát vagy adatbázist alkotó cellatartomány. Az adatbázis egymással kapcsolatos adatokból álló lista"
			},
			{
				name: "mező",
				description: "vagy az oszlopfelirat idézőjelek között, vagy a listában az oszlop helyét megadó szám"
			},
			{
				name: "kritérium",
				description: "a megadott feltételeket tartalmazó cellatartomány. A tartományban szerepel egy oszlopfelirat és alatta egy cellával a feltételt megadó címke"
			}
		]
	},
	{
		name: "AB.DARAB",
		description: "Az adatbázis adott feltételeknek eleget tevő rekordjaiban megszámolja, hány darab szám van egy adott mezőben (oszlopban).",
		arguments: [
			{
				name: "adatbázis",
				description: "a listát vagy adatbázist alkotó cellatartomány. Az adatbázis egymással kapcsolatos adatokból álló lista."
			},
			{
				name: "mező",
				description: "vagy az oszlopfelirat idézőjelek között, vagy a listában az oszlop helyét megadó szám"
			},
			{
				name: "kritérium",
				description: "a megadott feltételeket tartalmazó cellatartomány. A tartományban szerepel egy oszlopfelirat és alatta egy cellával a feltételt megadó címke."
			}
		]
	},
	{
		name: "AB.DARAB2",
		description: "Az adatbázis adott feltételeknek eleget tevő rekordjaiban megszámolja, hány darab nem üres cella van egy adott mezőben (oszlopban).",
		arguments: [
			{
				name: "adatbázis",
				description: "a listát vagy adatbázist alkotó cellatartomány. Az adatbázis egymással kapcsolatos adatokból álló lista."
			},
			{
				name: "mező",
				description: "vagy az oszlopfelirat idézőjelek között, vagy a listában az oszlop helyét megadó szám"
			},
			{
				name: "kritérium",
				description: "a megadott feltételeket tartalmazó cellatartomány. A tartományban szerepel egy oszlopfelirat és alatta egy cellával a feltételt megadó címke."
			}
		]
	},
	{
		name: "AB.MAX",
		description: "Az adatbázis adott feltételeknek eleget tevő rekordjaiból álló mezőben (oszlopban) lévő legnagyobb számot adja eredményül.",
		arguments: [
			{
				name: "adatbázis",
				description: "a listát vagy adatbázist alkotó cellatartomány. Az adatbázis egymással kapcsolatos adatokból álló lista"
			},
			{
				name: "mező",
				description: "vagy az oszlopfelirat idézőjelek között, vagy a listában az oszlop helyét megadó szám"
			},
			{
				name: "kritérium",
				description: "a megadott feltételeket tartalmazó cellatartomány. A tartományban szerepel egy oszlopfelirat és alatta egy cellával a feltételt megadó címke"
			}
		]
	},
	{
		name: "AB.MEZŐ",
		description: "Egy adatbázisból egyetlen olyan mezőt vesz ki, amely megfelel a megadott feltételeknek.",
		arguments: [
			{
				name: "adatbázis",
				description: "a listát vagy adatbázist alkotó cellatartomány. Az adatbázis egymással kapcsolatos adatokból álló lista."
			},
			{
				name: "mező",
				description: "vagy az oszlopfelirat idézőjelek között, vagy a listában az oszlop helyét megadó szám"
			},
			{
				name: "kritérium",
				description: "a megadott feltételeket tartalmazó cellatartomány. A tartományban szerepel egy oszlopfelirat és alatta egy cellával a feltételt megadó címke."
			}
		]
	},
	{
		name: "AB.MIN",
		description: "Az adatbázis adott feltételeknek eleget tevő rekordjaiból álló mezőben (oszlopban) lévő legkisebb számot adja eredményül.",
		arguments: [
			{
				name: "adatbázis",
				description: "a listát vagy adatbázist alkotó cellatartomány. Az adatbázis egymással kapcsolatos adatokból álló lista."
			},
			{
				name: "mező",
				description: "vagy az oszlopfelirat idézőjelek között, vagy a listában az oszlop helyét megadó szám"
			},
			{
				name: "kritérium",
				description: "a megadott feltételeket tartalmazó cellatartomány. A tartományban szerepel egy oszlopfelirat és alatta egy cellával a feltételt megadó címke."
			}
		]
	},
	{
		name: "AB.SZÓRÁS",
		description: "Az adatbázis kiválasztott elemei mint minta alapján becslést ad a szórásra.",
		arguments: [
			{
				name: "adatbázis",
				description: "a listát vagy adatbázist alkotó cellatartomány. Az adatbázis egymással kapcsolatos adatokból álló lista."
			},
			{
				name: "mező",
				description: "vagy az oszlopfelirat idézőjelek között, vagy a listában az oszlop helyét megadó szám"
			},
			{
				name: "kritérium",
				description: "a megadott feltételeket tartalmazó cellatartomány. A tartományban szerepel egy oszlopfelirat és alatta egy cellával a feltételt megadó címke."
			}
		]
	},
	{
		name: "AB.SZÓRÁS2",
		description: "Az adatbázis szűrt rekordjainak megadott mezőjében található értékek (nem mint minta, hanem a teljes sokaság) alapján kiszámítja a szórást.",
		arguments: [
			{
				name: "adatbázis",
				description: "a listát vagy adatbázist alkotó cellatartomány. Az adatbázis egymással kapcsolatos adatokból álló lista."
			},
			{
				name: "mező",
				description: "vagy az oszlopfelirat idézőjelek között, vagy a listában az oszlop helyét megadó szám"
			},
			{
				name: "kritérium",
				description: "a megadott feltételeket tartalmazó cellatartomány. A tartományban szerepel egy oszlopfelirat és alatta egy cellával a feltételt megadó címke."
			}
		]
	},
	{
		name: "AB.SZORZAT",
		description: "Az adatbázis adott feltételeknek eleget tevő rekordjaiból álló mezőben (oszlopban) összeszorozza az értékeket.",
		arguments: [
			{
				name: "adatbázis",
				description: "a listát vagy adatbázist alkotó cellatartomány. Az adatbázis egymással kapcsolatos adatokból álló lista."
			},
			{
				name: "mező",
				description: "vagy az oszlopfelirat idézőjelek között, vagy a listában az oszlop helyét megadó szám"
			},
			{
				name: "kritérium",
				description: "a megadott feltételeket tartalmazó cellatartomány. A tartományban szerepel egy oszlopfelirat és alatta egy cellával a feltételt megadó címke."
			}
		]
	},
	{
		name: "AB.SZUM",
		description: "Az adatbázis adott feltételeknek eleget tevő rekordjaiból álló mezőben (oszlopban) összeadja az értékeket.",
		arguments: [
			{
				name: "adatbázis",
				description: "a listát vagy adatbázist alkotó cellatartomány. Az adatbázis egymással kapcsolatos adatokból álló lista."
			},
			{
				name: "mező",
				description: "vagy az oszlopfelirat idézőjelek között, vagy a listában az oszlop helyét megadó szám"
			},
			{
				name: "kritérium",
				description: "a megadott feltételeket tartalmazó cellatartomány. A tartományban szerepel egy oszlopfelirat és alatta egy cellával a feltételt megadó címke."
			}
		]
	},
	{
		name: "AB.VAR",
		description: "Az adatbázis szűrt rekordjainak megadott mezőjében található értékek mint minta alapján becslést ad a varianciára; eredményül e becsült értéket adja.",
		arguments: [
			{
				name: "adatbázis",
				description: "a listát vagy adatbázist alkotó cellatartomány. Az adatbázis egymással kapcsolatos adatokból álló lista."
			},
			{
				name: "mező",
				description: "vagy az oszlopfelirat idézőjelek között, vagy a listában az oszlop helyét megadó szám"
			},
			{
				name: "kritérium",
				description: "a megadott feltételeket tartalmazó cellatartomány. A tartományban szerepel egy oszlopfelirat és alatta egy cellával a feltételt megadó címke."
			}
		]
	},
	{
		name: "AB.VAR2",
		description: "Az adatbázis szűrt rekordjainak megadott mezőjében található értékek (nem mint minta, hanem a teljes sokaság) alapján kiszámítja a varianciát.",
		arguments: [
			{
				name: "adatbázis",
				description: "a listát vagy adatbázist alkotó cellatartomány. Az adatbázis egymással kapcsolatos adatokból álló lista."
			},
			{
				name: "mező",
				description: "vagy az oszlopfelirat idézőjelek között, vagy a listában az oszlop helyét megadó szám"
			},
			{
				name: "kritérium",
				description: "a megadott feltételeket tartalmazó cellatartomány. A tartományban szerepel egy oszlopfelirat és alatta egy cellával a feltételt megadó címke."
			}
		]
	},
	{
		name: "ABS",
		description: "Egy szám abszolút értékét adja eredményül (a számot előjel nélkül).",
		arguments: [
			{
				name: "szám",
				description: "az a valós szám, amelynek az abszolút értékére kíváncsiak vagyunk"
			}
		]
	},
	{
		name: "ACOSH",
		description: "Egy szám area koszinusz hiperbolikusát számítja ki.",
		arguments: [
			{
				name: "szám",
				description: "tetszőleges 1-nél nem kisebb valós szám"
			}
		]
	},
	{
		name: "ALAP",
		description: "Átalakít egy számot a megadott alapú (számrendszerű) szöveges alakra.",
		arguments: [
			{
				name: "szám",
				description: "az átalakítandó szám"
			},
			{
				name: "alap",
				description: "az átalakítás után kapott szám alapszáma"
			},
			{
				name: "min_hossz",
				description: "a visszaadott karakterlánc minimális hossza. Ha nincs megadva, a bevezető nullák kimaradnak"
			}
		]
	},
	{
		name: "ÁR.LESZÁM",
		description: "Egy 100 Ft névértékű leszámítolt értékpapír árát adja eredményül.",
		arguments: [
			{
				name: "kiegyenlítés",
				description: "az értékpapír kifizetésének időpontja dátumértékként kifejezve"
			},
			{
				name: "lejárat",
				description: "az értékpapír lejáratának időpontja dátumértékként kifejezve"
			},
			{
				name: "leszámítolás",
				description: "az értékpapír leszámítolási rátája"
			},
			{
				name: "visszaváltás",
				description: "a 100 Ft névértékű értékpapír visszaváltási árfolyama"
			},
			{
				name: "alap",
				description: "az évtöredék számításának alapja"
			}
		]
	},
	{
		name: "ARAB",
		description: "Római számot arab számmá alakít át.",
		arguments: [
			{
				name: "szöveg",
				description: "az átalakítandó római szám"
			}
		]
	},
	{
		name: "ARCCOS",
		description: "Egy szám arkusz koszinuszát adja meg radiánban, a 0 - Pi tartományban. Az arkusz koszinusz az a szög, amelynek a koszinusza a megadott szám.",
		arguments: [
			{
				name: "szám",
				description: "a keresett szög koszinusza; -1 és +1 közé kell esnie"
			}
		]
	},
	{
		name: "ARCCOT",
		description: "Egy szám arkusz kotangensét adja meg radiánban, a 0–Pi tartományban.",
		arguments: [
			{
				name: "szám",
				description: "a keresett szög kotangense"
			}
		]
	},
	{
		name: "ARCCOTH",
		description: "Egy szám inverz hiperbolikus kotangensét adja meg.",
		arguments: [
			{
				name: "szám",
				description: "a keresett szög hiperbolikus kotangense"
			}
		]
	},
	{
		name: "ARCSIN",
		description: "Egy szám arkusz szinuszát adja meg radiánban, a -Pi/2 - Pi/2 tartományban.",
		arguments: [
			{
				name: "szám",
				description: "a keresett szög szinusza; -1 és +1 közé kell esnie"
			}
		]
	},
	{
		name: "ARCTAN",
		description: "Egy szám arkusz tangensét adja meg radiánban, a -Pi/2 -Pi/2 tartományban.",
		arguments: [
			{
				name: "szám",
				description: "a keresett szög tangense"
			}
		]
	},
	{
		name: "ARCTAN2",
		description: "A megadott x- és y-koordináták alapján számítja ki az arkusz tangens értéket radiánban -Pi és Pi között, -Pi kivételével.",
		arguments: [
			{
				name: "x_szám",
				description: "a pont x-koordinátája"
			},
			{
				name: "y_szám",
				description: "a pont y-koordinátája"
			}
		]
	},
	{
		name: "ASINH",
		description: "Egy szám area szinusz hiperbolikusát számítja ki.",
		arguments: [
			{
				name: "szám",
				description: "tetszőleges 1-nél nem kisebb valós szám"
			}
		]
	},
	{
		name: "ATANH",
		description: "A szám tangens hiperbolikusát számítja ki.",
		arguments: [
			{
				name: "szám",
				description: "tetszőleges 1-nél nem kisebb valós szám"
			}
		]
	},
	{
		name: "ÁTL.ELTÉRÉS",
		description: "Az adatpontoknak átlaguktól való átlagos abszolút eltérését számítja ki. Az argumentumok számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások lehetnek.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "ezek azok az adatpontok, amelyeknek átlagos abszolút eltérését ki kell számítani; számuk 1 és 255 között lehet"
			},
			{
				name: "szám2",
				description: "ezek azok az adatpontok, amelyeknek átlagos abszolút eltérését ki kell számítani; számuk 1 és 255 között lehet"
			}
		]
	},
	{
		name: "ÁTLAG",
		description: "Argumentumainak átlagát (számtani közepét) számítja ki, az argumentumok nevek, tömbök vagy számokat tartalmazó hivatkozások lehetnek.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "ezek azok az argumentumok (legfeljebb 255), amelyek átlagát ki kell számítani"
			},
			{
				name: "szám2",
				description: "ezek azok az argumentumok (legfeljebb 255), amelyek átlagát ki kell számítani"
			}
		]
	},
	{
		name: "ÁTLAGA",
		description: "Argumentumainak átlagát (számtani közepét) adja meg, a szöveget és a HAMIS értéket 0-nak veszi; az IGAZ értéket 1-nek. Az argumentumok számok, nevek, tömbök vagy hivatkozások lehetnek.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "érték1",
				description: "azon argumentumok (számuk 1 és 255 között lehet), amelyek átlagát ki kell számítani"
			},
			{
				name: "érték2",
				description: "azon argumentumok (számuk 1 és 255 között lehet), amelyek átlagát ki kell számítani"
			}
		]
	},
	{
		name: "ÁTLAGHA",
		description: "Az adott feltétel vagy kritérium által meghatározott cellák átlagát (számtani közepét) számítja ki.",
		arguments: [
			{
				name: "tartomány",
				description: "a kiértékelendő cellatartomány"
			},
			{
				name: "kritérium",
				description: "a feltétel vagy kritérium egy az átlagolandó cellákat definiáló szám, kifejezés vagy szöveg formájában"
			},
			{
				name: "átlag_tartomány",
				description: "maguk a cellák, amelyeknek az átlagát ki kell számítani. Ha ez az adat nincs megadva, a számítás a tartomány által meghatározott cellákat használja "
			}
		]
	},
	{
		name: "ÁTLAGHATÖBB",
		description: "Az adott feltétel- vagy kritériumkészlet által meghatározott cellák átlagát (számtani közepét) számítja ki.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "átlag_tartomány",
				description: "maguk a cellák, amelyeknek az átlagát ki kell számítani."
			},
			{
				name: "kritériumtartomány",
				description: "az adott feltétellel kiértékelni kívánt cellák tartománya"
			},
			{
				name: "kritérium",
				description: "a feltétel vagy kritérium egy az átlagolandó cellákat definiáló szám, kifejezés vagy szöveg formájában"
			}
		]
	},
	{
		name: "AZONOS",
		description: "Két szövegrészt hasonlít össze, az eredmény IGAZ vagy HAMIS. Az AZONOS függvény megkülönbözteti a kis- és nagybetűket.",
		arguments: [
			{
				name: "szöveg1",
				description: "az első szövegrész"
			},
			{
				name: "szöveg2",
				description: "a második szövegrész"
			}
		]
	},
	{
		name: "BAHTSZÖVEG",
		description: "Számot szöveggé alakít át (baht).",
		arguments: [
			{
				name: "szám",
				description: "az átalakítandó szám"
			}
		]
	},
	{
		name: "BAL",
		description: "Egy szövegrész elejétől megadott számú karaktert ad eredményül.",
		arguments: [
			{
				name: "szöveg",
				description: "az a szövegrész, amelyből ki kell venni a karaktereket"
			},
			{
				name: "hány_karakter",
				description: "azt határozza meg, hogy a BAL függvény hány karaktert adjon eredményül; elhagyása esetén értéke 1"
			}
		]
	},
	{
		name: "BESSELI",
		description: "Az In(x) módosított Bessel-függvény értékét adja eredményül.",
		arguments: [
			{
				name: "x",
				description: "az az érték, amelyre a függvény értékét ki kell számítani"
			},
			{
				name: "n",
				description: "a Bessel-függvény rendje"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "A Jn(x) Bessel-függvény értékét adja eredményül.",
		arguments: [
			{
				name: "x",
				description: "az az érték, amelyre a függvény értékét ki kell számítani"
			},
			{
				name: "n",
				description: "a Bessel-függvény rendje"
			}
		]
	},
	{
		name: "BESSELK",
		description: "A Kn(x) módosított Bessel-függvény értékét adja eredményül.",
		arguments: [
			{
				name: "x",
				description: "az az érték, amelyre a függvény értékét ki kell számítani"
			},
			{
				name: "n",
				description: "a Bessel-függvény rendje"
			}
		]
	},
	{
		name: "BESSELY",
		description: "Az Yn(x) módosított Bessel-függvény értékét adja eredményül.",
		arguments: [
			{
				name: "x",
				description: "az az érték, amelyre a függvény értékét ki kell számítani"
			},
			{
				name: "n",
				description: "a Bessel-függvény rendje"
			}
		]
	},
	{
		name: "BÉTA.ELOSZL",
		description: "A béta valószínűségi eloszlás értékét számítja ki.",
		arguments: [
			{
				name: "x",
				description: "az az A és B közötti érték, amelynél a függvény értékét ki kell számítani"
			},
			{
				name: "alfa",
				description: "az eloszlás paramétere, melynek nullánál nagyobbnak kell lennie"
			},
			{
				name: "béta",
				description: " az eloszlás paramétere, melynek nullánál nagyobbnak kell lennie"
			},
			{
				name: "eloszlásfv",
				description: "a függvény fajtáját megadó logikai érték: ha IGAZ, akkor az eloszlásfüggvény értékét számítja ki; ha HAMIS, a sűrűségfüggvényét"
			},
			{
				name: "A",
				description: "az x-ek intervallumának alsó határa; nem kötelező megadni. Ha elhagyjuk, A = 0."
			},
			{
				name: "B",
				description: "az x-ek intervallumának felső határa; nem kötelező megadni. Ha elhagyjuk, B = 1"
			}
		]
	},
	{
		name: "BÉTA.ELOSZLÁS",
		description: "A bétaeloszlás sűrűségfüggvényének értékét számítja ki.",
		arguments: [
			{
				name: "x",
				description: "az az A és B közé eső érték, amelyre a függvény értékét ki kell számítani"
			},
			{
				name: "alfa",
				description: "az eloszlás paramétere, nullánál nagyobbnak kell lennie"
			},
			{
				name: "béta",
				description: "az eloszlás paramétere, nullánál nagyobbnak kell lennie"
			},
			{
				name: "A",
				description: "az x-ek intervallumának alsó határa; nem kötelező megadni. Ha elhagyjuk, A = 0."
			},
			{
				name: "B",
				description: "az x-ek intervallumának felső határa; nem kötelező megadni. Ha elhagyjuk, B = 1."
			}
		]
	},
	{
		name: "BÉTA.INVERZ",
		description: "A bétaeloszlás sűrűségfüggvénye (BÉTA.ELOSZL) inverzét számítja ki.",
		arguments: [
			{
				name: "valószínűség",
				description: "a bétaeloszláshoz tartozó valószínűségérték"
			},
			{
				name: "alfa",
				description: "az eloszlás paramétere, nullánál nagyobbnak kell lennie"
			},
			{
				name: "béta",
				description: "az eloszlás paramétere, nullánál nagyobbnak kell lennie"
			},
			{
				name: "A",
				description: "az x-ek intervallumának alsó határa; nem kötelező megadni. Ha elhagyjuk, A = 0"
			},
			{
				name: "B",
				description: "az x-ek intervallumának felső határa; nem kötelező megadni. Ha elhagyjuk, B = 1"
			}
		]
	},
	{
		name: "BIN.DEC",
		description: "Bináris számot decimálissá alakít át.",
		arguments: [
			{
				name: "szám",
				description: "az átalakítandó bináris szám"
			}
		]
	},
	{
		name: "BIN.HEX",
		description: "Bináris számot hexadecimálissá alakít át.",
		arguments: [
			{
				name: "szám",
				description: "az átalakítandó bináris szám"
			},
			{
				name: "jegyek",
				description: "a használandó karakterek száma"
			}
		]
	},
	{
		name: "BIN.OKT",
		description: "Bináris számot oktálissá alakít át.",
		arguments: [
			{
				name: "szám",
				description: "az átalakítandó bináris szám"
			},
			{
				name: "jegyek",
				description: "a használandó karakterek száma"
			}
		]
	},
	{
		name: "BINOM.ELOSZL",
		description: "A diszkrét binomiális eloszlás valószínűségértékét számítja ki.",
		arguments: [
			{
				name: "sikeresek",
				description: "a sikeres kísérletek száma"
			},
			{
				name: "kísérletek",
				description: "a független kísérletek száma"
			},
			{
				name: "siker_valószínűsége",
				description: "a siker valószínűsége az egyes kísérletek esetén"
			},
			{
				name: "eloszlásfv",
				description: "a függvény fajtáját megadó logikai érték: ha IGAZ, akkor a BINOM.ELOSZLÁS függvény az eloszlásfüggvény értékét számítja ki; ha HAMIS, a sűrűségfüggvényét."
			}
		]
	},
	{
		name: "BINOM.ELOSZL.TART",
		description: "Egy adott kimenetel valószínűségét számítja ki binomiális eloszlás esetén.",
		arguments: [
			{
				name: "kísérletek",
				description: "a független kísérletek száma"
			},
			{
				name: "siker_valószínűsége",
				description: "a siker valószínűsége az egyes kísérletek esetén"
			},
			{
				name: "sikeresek",
				description: "a sikeres kísérletek száma"
			},
			{
				name: "sikeresek_2",
				description: "ha meg van adva, a függvény annak a valószínűségét számítja ki, hogy a sikeres kísérletek száma a sikeresek és a sikeresek_2 érték közé esik"
			}
		]
	},
	{
		name: "BINOM.ELOSZLÁS",
		description: "A diszkrét binomiális eloszlás valószínűségértékét számítja ki.",
		arguments: [
			{
				name: "sikeresek",
				description: "a sikeres kísérletek száma"
			},
			{
				name: "kísérletek",
				description: "a független kísérletek száma"
			},
			{
				name: "siker_valószínűsége",
				description: "a siker valószínűsége az egyes kísérletek esetén"
			},
			{
				name: "eloszlásfv",
				description: "a függvény fajtáját megadó logikai érték: ha IGAZ, akkor az eloszlásfüggvény értékét számítja ki; ha HAMIS, a tömegfüggvényét."
			}
		]
	},
	{
		name: "BINOM.INVERZ",
		description: "Azt a legkisebb számot adja eredményül, amelyre a binomiális eloszlásfüggvény értéke nem kisebb egy adott határértéknél.",
		arguments: [
			{
				name: "kísérletek",
				description: "a Bernoulli-kísérletek száma"
			},
			{
				name: "siker_valószínűsége",
				description: "a siker valószínűsége az egyes kísérletek esetén; 0 és 1 közti szám, a végpontokat is beleértve"
			},
			{
				name: "alfa",
				description: "a határérték; 0 és 1 közti szám, a végpontokat is beleértve"
			}
		]
	},
	{
		name: "BIT.BAL.ELTOL",
		description: "Az eltolás_mértéke bittel balra tolt számot adja vissza.",
		arguments: [
			{
				name: "szám",
				description: "a kiértékelendő bináris szám decimális alakja"
			},
			{
				name: "eltolás_mértéke",
				description: "ahány bittel balra szeretné tolni a számot"
			}
		]
	},
	{
		name: "BIT.ÉS",
		description: "Két számmal bitenkénti „és” műveletet végez.",
		arguments: [
			{
				name: "szám1",
				description: "a kiértékelendő bináris szám decimális alakja"
			},
			{
				name: "szám2",
				description: "a kiértékelendő bináris szám decimális alakja"
			}
		]
	},
	{
		name: "BIT.JOBB.ELTOL",
		description: "Az eltolás_mértéke bittel jobbra tolt számot adja vissza.",
		arguments: [
			{
				name: "szám",
				description: "a kiértékelendő bináris szám decimális alakja"
			},
			{
				name: "eltolás_mértéke",
				description: "ahány bittel jobbra szeretné tolni a számot"
			}
		]
	},
	{
		name: "BIT.VAGY",
		description: "Két számmal bitenkénti „vagy” műveletet végez.",
		arguments: [
			{
				name: "szám1",
				description: "a kiértékelendő bináris szám decimális alakja"
			},
			{
				name: "szám2",
				description: "a kiértékelendő bináris szám decimális alakja"
			}
		]
	},
	{
		name: "BIT.XVAGY",
		description: "Két számmal bitenkénti „kizárólagos vagy” műveletet végez.",
		arguments: [
			{
				name: "szám1",
				description: "a kiértékelendő bináris szám decimális alakja"
			},
			{
				name: "szám2",
				description: "a kiértékelendő bináris szám decimális alakja"
			}
		]
	},
	{
		name: "BMR",
		description: "A megadott pénzáramlás-számsor (cash flow) belső megtérülési rátáját számítja ki.",
		arguments: [
			{
				name: "értékek",
				description: "egy tömb vagy egy, a pénzáramlás azon értékeit tartalmazó cellákra való hivatkozás, amelyekre a belső megtérülési rátát ki kell számítani"
			},
			{
				name: "becslés",
				description: "egy olyan szám, amely várhatóan közel esik az eredményhez; ha elhagyjuk, 0,1 (10 százalék)."
			}
		]
	},
	{
		name: "CELLA",
		description: "Hivatkozás olvasási sorrend szerinti első cellájának formázására, helyére vagy tartalmára vonatkozó információt ad vissza.",
		arguments: [
			{
				name: "infótípus",
				description: "szöveges érték, amely a celláról megkapni kívánt információ típusát határozza meg."
			},
			{
				name: "hivatkozás",
				description: "az a cella, amelyről információt kíván kapni"
			}
		]
	},
	{
		name: "CÍM",
		description: "A megadott sor- és oszlopszám alapján cellahivatkozást hoz létre szöveges formában.",
		arguments: [
			{
				name: "sor_szám",
				description: "a cellahivatkozásban használt sor száma: az 1-es sor esetén a sor_szám = 1"
			},
			{
				name: "oszlop_szám",
				description: "a cellahivatkozásban használt oszlop száma, pl. a D oszlop esetén az oszlop_szám = 4"
			},
			{
				name: "típus",
				description: "a hivatkozás típusát határozza meg: abszolút = 1; abszolút sor/relatív oszlop = 2; relatív sor/abszolút oszlop = 3; relatív =4"
			},
			{
				name: "a1",
				description: "a hivatkozás típusát meghatározó logikai érték: A1 típus = 1 vagy IGAZ; S1O1 típus = 0 vagy HAMIS"
			},
			{
				name: "munkalapnév",
				description: "a munkalap külső hivatkozásként használható szövegként megadott neve"
			}
		]
	},
	{
		name: "COS",
		description: "Egy szög koszinuszát számítja ki.",
		arguments: [
			{
				name: "szám",
				description: "az a radiánban megadott szög, amelynek koszinuszát keresi"
			}
		]
	},
	{
		name: "COSH",
		description: "Egy szám koszinusz hiperbolikusát számítja ki.",
		arguments: [
			{
				name: "szám",
				description: "tetszőleges valós szám"
			}
		]
	},
	{
		name: "COT",
		description: "Egy szög kotangensét számítja ki.",
		arguments: [
			{
				name: "szám",
				description: "az a radiánban megadott szög, amelynek a kotangensét keresi"
			}
		]
	},
	{
		name: "COTH",
		description: "Egy szám hiperbolikus kotangensét számítja ki.",
		arguments: [
			{
				name: "szám",
				description: "az a radiánban megadott szög, amelynek a hiperbolikus kotangensét keresi"
			}
		]
	},
	{
		name: "CSC",
		description: "Egy szög koszekánsát számítja ki.",
		arguments: [
			{
				name: "szám",
				description: "az a radiánban megadott szög, amelynek a koszekánsát keresi"
			}
		]
	},
	{
		name: "CSCH",
		description: "Egy szög hiperbolikus koszekánsát számítja ki.",
		arguments: [
			{
				name: "szám",
				description: "az a radiánban megadott szög, amelynek a hiperbolikus koszekánsát keresi"
			}
		]
	},
	{
		name: "CSERE",
		description: "Szövegdarab megadott részét eltérő szövegdarabbal cseréli ki.",
		arguments: [
			{
				name: "régi_szöveg",
				description: "az a szöveg, amelyben néhány karaktert ki kell cserélni"
			},
			{
				name: "honnantól",
				description: "az a régi_szövegbeli karakterhely, amelytől kezdve a karaktereket az új_szövegre ki kell cserélni"
			},
			{
				name: "hány_karakter",
				description: "a régi_szövegben kicserélendő karakterek száma"
			},
			{
				name: "új_szöveg",
				description: "az a szövegdarab, amely a régi_szövegbeli karaktereket helyettesíteni fogja"
			}
		]
	},
	{
		name: "CSONK",
		description: "Egy számot egésszé csonkít úgy, hogy a szám tizedes- vagy törtrészét eltávolítja.",
		arguments: [
			{
				name: "szám",
				description: "a csonkítandó szám"
			},
			{
				name: "hány_számjegy",
				description: "a csonkítás pontosságát megadó számjegyek száma, elhagyása esetén 0 (zérus)"
			}
		]
	},
	{
		name: "CSÚCSOSSÁG",
		description: "Egy adathalmaz csúcsosságát számítja ki.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "azon számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások, amelyek csúcsosságát ki kell számítani; számuk 1 és 255 között lehet"
			},
			{
				name: "szám2",
				description: "azon számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások, amelyek csúcsosságát ki kell számítani; számuk 1 és 255 között lehet"
			}
		]
	},
	{
		name: "DARAB",
		description: "Megszámolja, hogy hány olyan cella van egy tartományban, amely számot tartalmaz.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "érték1",
				description: "legfeljebb 255 argumentum, amely többféle típusú adatot tartalmazhat vagy jelölhet meg, a program azonban csak a számokat veszi figyelembe"
			},
			{
				name: "érték2",
				description: "legfeljebb 255 argumentum, amely többféle típusú adatot tartalmazhat vagy jelölhet meg, a program azonban csak a számokat veszi figyelembe"
			}
		]
	},
	{
		name: "DARAB2",
		description: "Megszámolja, hogy hány nem üres cella található egy tartományban.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "érték1",
				description: "argumentumok (számuk 1 és 255 között lehet), amelyek az összeszámolni kívánt értékeket és cellákat jelzik; az érték bármilyen típusú információ lehet."
			},
			{
				name: "érték2",
				description: "argumentumok (számuk 1 és 255 között lehet), amelyek az összeszámolni kívánt értékeket és cellákat jelzik; az érték bármilyen típusú információ lehet."
			}
		]
	},
	{
		name: "DARABHATÖBB",
		description: "Egy adott feltétel- vagy kritériumkészlet által meghatározott cellatartomány celláinak számát állapítja meg.",
		arguments: [
			{
				name: "kritériumtartomány",
				description: "az adott feltétellel kiértékelni kívánt cellák tartománya"
			},
			{
				name: "kritérium",
				description: "a feltétel egy a megszámolandó cellákat definiáló szám, kifejezés vagy szöveg formájában"
			}
		]
	},
	{
		name: "DARABTELI",
		description: "Egy tartományban összeszámolja azokat a nem üres cellákat, amelyek eleget tesznek a megadott feltételeknek.",
		arguments: [
			{
				name: "tartomány",
				description: "az a cellatartomány, amelyben a nem üres cellákat meg kell számolni"
			},
			{
				name: "kritérium",
				description: "az összeszámolandó cellákat meghatározó számként, kifejezésként vagy szövegként megadott feltétel"
			}
		]
	},
	{
		name: "DARABÜRES",
		description: "Kijelölt cellatartományban megszámlálja az üres cellákat.",
		arguments: [
			{
				name: "tartomány",
				description: "az a tartomány, amelyben az üres cellákat meg kell számlálni"
			}
		]
	},
	{
		name: "DÁTUM",
		description: "Eredménye a dátumot Spreadsheet dátum- és időértékben megadó szám.",
		arguments: [
			{
				name: "év",
				description: "egy 1900 és 9999 közötti évszám a Windows rendszerhez készült Spreadsheet esetén, illetve 1904 és 9999 közötti szám a Macintosh rendszerhez készült Spreadsheet esetén"
			},
			{
				name: "hónap",
				description: "a hónap száma az éven belül (1-12)"
			},
			{
				name: "nap",
				description: "a nap száma a hónapon belül (1-31)"
			}
		]
	},
	{
		name: "DÁTUMÉRTÉK",
		description: "Szövegként megadott dátumot olyan számmá alakít át, amely Spreadsheet dátum- és időértékben adja meg a dátumot.",
		arguments: [
			{
				name: "dátum_szöveg",
				description: "a Spreadsheet valamely dátumformátumában szövegesen megadott dátum, Windows esetén 1900. 01. 01. és 9999. 12. 31. között"
			}
		]
	},
	{
		name: "DÁTUMTÓLIG",
		description: "",
		arguments: [
		]
	},
	{
		name: "DEC.BIN",
		description: "Decimális számot binárissá alakít át.",
		arguments: [
			{
				name: "szám",
				description: "az átalakítandó decimális egész szám"
			},
			{
				name: "jegyek",
				description: "a használandó karakterek száma"
			}
		]
	},
	{
		name: "DEC.HEX",
		description: "Decimális számot hexadecimálissá alakít át.",
		arguments: [
			{
				name: "szám",
				description: "az átalakítandó decimális egész szám"
			},
			{
				name: "jegyek",
				description: "a használandó karakterek száma"
			}
		]
	},
	{
		name: "DEC.OKT",
		description: "Decimális számot oktálissá alakít át.",
		arguments: [
			{
				name: "szám",
				description: "az átalakítandó decimális egész szám"
			},
			{
				name: "jegyek",
				description: "a használandó karakterek száma"
			}
		]
	},
	{
		name: "DELTA",
		description: "Azt vizsgálja, hogy két érték egyenlő-e.",
		arguments: [
			{
				name: "szám1",
				description: "az első szám"
			},
			{
				name: "szám2",
				description: "a második szám"
			}
		]
	},
	{
		name: "ÉCSRI",
		description: "Egy tárgyi eszköz amortizációját számítja ki egy megadott vagy részidőszakra a dupla gyorsaságú csökkenő egyenleg módszerének, vagy más megadott módszernek az alkalmazásával.",
		arguments: [
			{
				name: "költség",
				description: "az eszköz beszerzési ára"
			},
			{
				name: "maradványérték",
				description: "az eszköz maradványértéke a leírási idő eltelte után"
			},
			{
				name: "leírási_idő",
				description: "a leírási időszakok teljes száma (azaz az eszköz hasznos élettartama)"
			},
			{
				name: "kezdő_időszak",
				description: "az első leírási időszak; ugyanazt a mértékegységet kell használni, mint az élettartam megadásánál"
			},
			{
				name: "záró_időszak",
				description: "az utolsó leírási időszak; ugyanazt a mértékegységet kell használni, mint az élettartam megadásánál"
			},
			{
				name: "faktor",
				description: "a leírás gyorsasága; ha elhagyjuk, 2 (dupla gyorsaságú)"
			},
			{
				name: "nem_vált",
				description: "logikai érték. Ha HAMIS vagy elhagyjuk: át kell térni a lineáris leírási modellre, ha az nagyobb leírandó értéket adna, mint a csökkenő leírási módszer. Ha IGAZ, nem kell áttérni."
			}
		]
	},
	{
		name: "ELŐJEL",
		description: "Egy szám előjelét határozza meg: pozitív szám esetén 1, zérus esetén 0, negatív szám esetén -1.",
		arguments: [
			{
				name: "szám",
				description: "tetszőleges valós szám"
			}
		]
	},
	{
		name: "ELŐREJELZÉS",
		description: "Az ismert értékek alapján lineáris regresszió segítségével jövőbeli értéket számít ki vagy becsül meg.",
		arguments: [
			{
				name: "x",
				description: "az az adat, amelyre előrejelzést kíván kapni; számértéknek kell lennie"
			},
			{
				name: "ismert_y",
				description: "a függő számértékeket tartalmazó tömb vagy adattartomány"
			},
			{
				name: "ismert_x",
				description: "a független számértékeket tartalmazó tömb vagy tartomány"
			}
		]
	},
	{
		name: "ELSŐ.SZELVÉNYDÁTUM",
		description: "A kifizetést követő legelső szelvénydátumot adja eredményül.",
		arguments: [
			{
				name: "kiegyenlítés",
				description: "az értékpapír kifizetésének időpontja dátumértékként kifejezve"
			},
			{
				name: "lejárat",
				description: "az értékpapír lejáratának időpontja dátumértékként kifejezve"
			},
			{
				name: "gyakoriság",
				description: "a kamat- vagy osztalékszelvényekre történő kifizetések száma egy év alatt"
			},
			{
				name: "alap",
				description: "az évtöredék számításának alapja"
			}
		]
	},
	{
		name: "ELTOLÁS",
		description: "Megadott magasságú és szélességű hivatkozást ad meg egy hivatkozástól számított megadott sornyi és oszlopnyi távolságra.",
		arguments: [
			{
				name: "hivatkozás",
				description: "az a hivatkozás, amelyhez képest az eredményül kapott hivatkozás helyzetét az argumentumok meghatározzák; cellára vagy egymás melletti cellák tartományára való hivatkozás"
			},
			{
				name: "sorok",
				description: "az eredmény bal felső cellája és a hivatkozás közötti függőleges távolság a sorok számában kifejezve"
			},
			{
				name: "oszlopok",
				description: "az eredmény bal felső cellája és a hivatkozás közötti vízszintes távolság az oszlopok számában kifejezve"
			},
			{
				name: "magasság",
				description: "az eredményül kapott hivatkozás magassága a sorok számában mérve; elhagyása esetén a hivatkozás magasságával azonos"
			},
			{
				name: "szélesség",
				description: "az eredményül kapott hivatkozás szélessége az oszlopok számában mérve; elhagyása esetén a hivatkozás szélességével azonos"
			}
		]
	},
	{
		name: "ÉRTÉK",
		description: "Számot ábrázoló szöveget számmá alakít át.",
		arguments: [
			{
				name: "szöveg",
				description: "az átalakítandó szöveg, idézőjelek közé zárva, ill. az átalakítandó szöveget tartalmazó cellára való hivatkozás"
			}
		]
	},
	{
		name: "ÉS",
		description: "Megvizsgálja, hogy minden argumentumára érvényes-e az IGAZ, és ha minden argumentuma IGAZ, eredménye IGAZ.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logikai1",
				description: "a megvizsgálandó feltételek; logikai értékek, tömbök vagy hivatkozások, mindegyik értéke IGAZ vagy HAMIS, számuk pedig 1 és 255 közötti lehet"
			},
			{
				name: "logikai2",
				description: "a megvizsgálandó feltételek; logikai értékek, tömbök vagy hivatkozások, mindegyik értéke IGAZ vagy HAMIS, számuk pedig 1 és 255 közötti lehet"
			}
		]
	},
	{
		name: "ÉSZÖ",
		description: "Egy tárgyi eszköz értékcsökkenését számítja ki adott időszakra az évek számjegyösszegével dolgozó módszer alapján.",
		arguments: [
			{
				name: "költség",
				description: "az eszköz beszerzési ára"
			},
			{
				name: "maradványérték",
				description: "az eszköz maradványértéke a leírási idő eltelte után"
			},
			{
				name: "leírási_idő",
				description: "a leírási időszakok teljes száma (azaz az eszköz hasznos élettartama)"
			},
			{
				name: "időszak",
				description: "az az időszak, amelyre az értékcsökkenés mértékét ki kell számítani; ugyanazt a mértékegységet kell használni, mint az élettartam megadásánál"
			}
		]
	},
	{
		name: "ÉV",
		description: "Kiszámítja, hogy az adott dátum melyik évre esik (1900 és 9999 közötti egész szám).",
		arguments: [
			{
				name: "időérték",
				description: "Spreadsheet dátum- és időértékben megadott szám"
			}
		]
	},
	{
		name: "EXP.ELOSZL",
		description: "Az exponenciális eloszlás értékét számítja ki.",
		arguments: [
			{
				name: "x",
				description: "az az érték, amelynél az eloszlást ki kell számítani, nem negatív szám"
			},
			{
				name: "lambda",
				description: "az eloszlás paramétere, pozitív szám"
			},
			{
				name: "eloszlásfv",
				description: "a függvény fajtáját megadó logikai érték: ha IGAZ, az eloszlásfüggvény értékét számítja ki, ha HAMIS, a sűrűségfüggvényét"
			}
		]
	},
	{
		name: "EXP.ELOSZLÁS",
		description: "Az exponenciális eloszlás értékét számítja ki.",
		arguments: [
			{
				name: "x",
				description: "az az érték, amelynél az eloszlást ki kell számítani, nem negatív szám"
			},
			{
				name: "lambda",
				description: "az eloszlás paramétere, pozitív szám"
			},
			{
				name: "eloszlásfv",
				description: "a függvény fajtáját megadó logikai érték: ha IGAZ, az eloszlásfüggvény értékét számítja ki, ha HAMIS, a sűrűségfüggvényét"
			}
		]
	},
	{
		name: "F.ELOSZL",
		description: "A balszélű F-eloszlás értékét (az eltérés fokát) számítja ki két adathalmazra.",
		arguments: [
			{
				name: "x",
				description: "az az érték, amelynél a függvény értékét ki kell számítani, nem negatív szám"
			},
			{
				name: "szabadságfok1",
				description: "a számláló szabadságfoka, 1 és 10^10 közötti szám a 10^10 kivételével"
			},
			{
				name: "szabadságfok2",
				description: "a nevező szabadságfoka, 1 és 10^10 közötti szám a 10^10 kivételével"
			},
			{
				name: "eloszlásfv",
				description: "a függvény fajtáját megadó logikai érték: ha IGAZ, az eloszlásfüggvény értékét számítja ki, ha HAMIS, a sűrűségfüggvényét"
			}
		]
	},
	{
		name: "F.ELOSZLÁS",
		description: "Az F-eloszlás (jobbszélű) értékét (az eltérés fokát) számítja ki két adathalmazra.",
		arguments: [
			{
				name: "x",
				description: "az az érték, amelynél a függvény értékét ki kell számítani, nem negatív szám"
			},
			{
				name: "szabadságfok1",
				description: "a számláló szabadságfoka, 1 és 10^10 közötti szám a 10^10 kivételével"
			},
			{
				name: "szabadságfok2",
				description: "a nevező szabadságfoka, 1 és 10^10 közötti szám a 10^10 kivételével"
			}
		]
	},
	{
		name: "F.ELOSZLÁS.JOBB",
		description: "A jobbszélű F-eloszlás értékét (az eltérés fokát) számítja ki két adathalmazra.",
		arguments: [
			{
				name: "x",
				description: "az az érték, amelynél a függvény értékét ki kell számítani, nem negatív szám"
			},
			{
				name: "szabadságfok1",
				description: "a számláló szabadságfoka, 1 és 10^10 közötti szám a 10^10 kivételével"
			},
			{
				name: "szabadságfok2",
				description: "a nevező szabadságfoka, 1 és 10^10 közötti szám a 10^10 kivételével"
			}
		]
	},
	{
		name: "F.INVERZ",
		description: "A balszélű F-eloszlás inverzének értékét számítja ki: ha p = F.ELOSZL(x,...), akkor F.INVERZ(p,...) = x.",
		arguments: [
			{
				name: "valószínűség",
				description: "az F-eloszláshoz tartozó valószínűségérték; 0 és 1 közti szám, a végpontokat is beleértve"
			},
			{
				name: "szabadságfok1",
				description: "a számláló szabadságfoka, 1 és 10^10 közötti szám a 10^10 kivételével"
			},
			{
				name: "szabadságfok2",
				description: "a nevező szabadságfoka, 1 és 10^10 közötti szám a 10^10 kivételével"
			}
		]
	},
	{
		name: "F.INVERZ.JOBB",
		description: "A jobbszélű F-eloszlás inverzének értékét számítja ki: ha p = F.ELOSZLÁS.JOBB(x,...), akkor F.INVERZ.JOBB(p,...) = x.",
		arguments: [
			{
				name: "valószínűség",
				description: "az F-eloszláshoz tartozó valószínűségérték; 0 és 1 közti szám, a végpontokat is beleértve"
			},
			{
				name: "szabadságfok1",
				description: "a számláló szabadságfoka, 1 és 10^10 közötti szám a 10^10 kivételével"
			},
			{
				name: "szabadságfok2",
				description: "a nevező szabadságfoka, 1 és 10^10 közötti szám a 10^10 kivételével"
			}
		]
	},
	{
		name: "F.PRÓB",
		description: "Az F-próba értékét adja eredményül (annak a kétszélű valószínűségét, hogy a két tömb varianciája nem tér el szignifikánsan).",
		arguments: [
			{
				name: "tömb1",
				description: "az első adattömb vagy adattartomány; elemei számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások lehetnek (az üres cellák nem számítanak)"
			},
			{
				name: "tömb2",
				description: "a második adattömb vagy adattartomány; elemei számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások lehetnek (az üres cellák nem számítanak)"
			}
		]
	},
	{
		name: "F.PRÓBA",
		description: "Az F-próba értékét adja eredményül (annak a kétszélű valószínűségét, hogy a két tömb varianciája nem tér el szignifikánsan).",
		arguments: [
			{
				name: "tömb1",
				description: "az első adattömb vagy adattartomány; elemei számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások lehetnek (az üres cellák nem számítanak)"
			},
			{
				name: "tömb2",
				description: "a második adattömb vagy adattartomány; elemei számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások lehetnek (az üres cellák nem számítanak)"
			}
		]
	},
	{
		name: "FAKT",
		description: "Egy szám faktoriálisát számítja ki. A szám faktoriálisa = 1*2*3*...*szám.",
		arguments: [
			{
				name: "szám",
				description: "az a nemnegatív szám, amelynek faktoriálisát ki kell számítani"
			}
		]
	},
	{
		name: "FAKTDUPLA",
		description: "Egy szám dupla faktoriálisát adja eredményül.",
		arguments: [
			{
				name: "szám",
				description: "az az érték, amelynek dupla faktoriálisát keressük"
			}
		]
	},
	{
		name: "FERDESÉG",
		description: "Egy eloszlás ferdeségét határozza meg; a ferdeség az eloszlás átlaga körül vett aszimmetria mértékét adja meg.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "azon számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások (számuk 1 és 255 között lehet), amelyekre a ferdeséget meg kell határozni."
			},
			{
				name: "szám2",
				description: "azon számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások (számuk 1 és 255 között lehet), amelyekre a ferdeséget meg kell határozni."
			}
		]
	},
	{
		name: "FERDESÉG.P",
		description: "Egy eloszlás ferdeségét határozza meg egy sokaság alapján; a ferdeség az eloszlás átlaga körül vett aszimmetria mértékét adja meg.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "azon számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások (számuk 1 és 254 között lehet), amelyekre a sokaság ferdeségét meg kell határozni"
			},
			{
				name: "szám2",
				description: "azon számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások (számuk 1 és 254 között lehet), amelyekre a sokaság ferdeségét meg kell határozni"
			}
		]
	},
	{
		name: "FI",
		description: "A standard normális eloszlás sűrűségfüggvényének értékét számítja ki.",
		arguments: [
			{
				name: "x",
				description: "az a szám, amelynél a standard normális eloszlás sűrűségfüggvényének értékére kíváncsi"
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
		name: "FISHER",
		description: "Fisher-transzformációt hajt végre.",
		arguments: [
			{
				name: "x",
				description: "az a számérték, amelyre a transzformációt végre kell hajtani, -1 és 1 közötti szám a -1 és 1 kivételével"
			}
		]
	},
	{
		name: "FIX",
		description: "Egy számot adott számú tizedesjegyre kerekít és szöveg formában adja vissza ezreselválasztó jelekkel vagy azok nélkül.",
		arguments: [
			{
				name: "szám",
				description: "az a szám, amelyet kerekítés után szöveggé kell átalakítani"
			},
			{
				name: "tizedesek",
				description: "a tizedesjegyek száma. Ha elhagyjuk, a tizedesjegyek száma 2 lesz."
			},
			{
				name: "nincs_pont",
				description: "logikai érték: IGAZ esetén a FIX függvény eredményében nem szerepelnek ezreseket elválasztó jelek; ha HAMIS vagy elhagyjuk, szerepelnek"
			}
		]
	},
	{
		name: "FKERES",
		description: "Egy tábla bal szélső oszlopában megkeres egy értéket, majd annak sora és a megadott oszlop metszéspontjában levő értéket adja eredményül. Alapesetben a táblázatnak növekvő sorrendbe rendezettnek kell lennie.",
		arguments: [
			{
				name: "keresési_érték",
				description: "a tábla első oszlopában megkeresendő érték; érték, hivatkozás vagy szövegdarab lehet"
			},
			{
				name: "tábla",
				description: "az a szöveget, számokat vagy logikai értékeket tartalmazó tábla, amelyben a keresést végre kell hajtani. Lehet tartományra való hivatkozás vagy tartománynév is"
			},
			{
				name: "oszlop_szám",
				description: "a tábla azon oszlopának a táblán belüli sorszáma, amelyből az eredményt meg kívánja kapni. A tábla első értékoszlopa az 1-es számú oszlop"
			},
			{
				name: "tartományban_keres",
				description: "logikai érték: HAMIS esetén pontos egyezés szükséges; ha IGAZ, vagy elhagyjuk, az első oszlopban lévő legjobb közelítést adja meg, növekvő rendezés esetén"
			}
		]
	},
	{
		name: "FOK",
		description: "Radiánt fokká alakít át.",
		arguments: [
			{
				name: "szög",
				description: "az a radiánban megadott szögérték, amelyet fokokba kell átszámítani"
			}
		]
	},
	{
		name: "FORINT",
		description: "Egy számot pénznem formátumú szöveggé alakít át.",
		arguments: [
			{
				name: "szám",
				description: "egy szám vagy egy számot ill. számértéket eredményező képletet tartalmazó cellára való hivatkozás"
			},
			{
				name: "tizedesek",
				description: "a tizedesjegyek száma. A szám szükség szerint kerekítve lesz; ha elhagyjuk, a tizedesjegyek száma 2 lesz."
			}
		]
	},
	{
		name: "FORINT.DEC",
		description: "Közönséges törtként megadott számot tizedes törtté alakít át.",
		arguments: [
			{
				name: "tört_érték",
				description: "a törtszám"
			},
			{
				name: "tört_nevező",
				description: "a tört nevezőjében használt egész szám"
			}
		]
	},
	{
		name: "FORINT.TÖRT",
		description: "Tizedes törtként megadott számot közönséges törtté alakít át.",
		arguments: [
			{
				name: "tizedes_érték",
				description: "tizedes szám"
			},
			{
				name: "tört_nevező",
				description: "a tört nevezőjében használt egész szám"
			}
		]
	},
	{
		name: "GAMMA",
		description: "A Gamma-függvény értékét számítja ki.",
		arguments: [
			{
				name: "x",
				description: "az az érték, amelyhez ki szeretné számítani a Gamma-értéket"
			}
		]
	},
	{
		name: "GAMMA.ELOSZL",
		description: "A gammaeloszlás értékét számítja ki.",
		arguments: [
			{
				name: "x",
				description: "az az érték, amelynél az eloszlást ki kell számítani, nem negatív szám"
			},
			{
				name: "alfa",
				description: "az eloszlás paramétere, pozitív szám"
			},
			{
				name: "béta",
				description: "az eloszlás paramétere, pozitív szám. Ha béta = 1, a GAMMA.ELOSZL a standard gammaeloszlás értékét adja eredményül"
			},
			{
				name: "eloszlásfv",
				description: "a függvény fajtáját megadó logikai érték; ha IGAZ, a GAMMA.ELOSZL az eloszlásfüggvény értékét számítja ki; ha HAMIS vagy elhagyjuk, a sűrűségfüggvényét"
			}
		]
	},
	{
		name: "GAMMA.ELOSZLÁS",
		description: "A gammaeloszlás értékét számítja ki.",
		arguments: [
			{
				name: "x",
				description: "az az érték, amelynél az eloszlást ki kell számítani, nem negatív szám"
			},
			{
				name: "alfa",
				description: "az eloszlás paramétere, pozitív szám"
			},
			{
				name: "béta",
				description: "az eloszlás paramétere, pozitív szám. Ha béta = 1, a GAMMA.ELOSZLÁS a standard gammaeloszlás értékét adja eredményül"
			},
			{
				name: "eloszlásfv",
				description: "a függvény fajtáját megadó logikai érték; ha IGAZ, a GAMMA.ELOSZLÁS az eloszlásfüggvény értékét számítja ki; ha HAMIS vagy elhagyjuk, a tömegfüggvényét"
			}
		]
	},
	{
		name: "GAMMA.INVERZ",
		description: "A gammaeloszlás eloszlásfüggvénye inverzének értékét számítja ki: ha p = GAMMA.ELOSZL(x,...), akkor GAMMA.INVERZ(p,...) = x.",
		arguments: [
			{
				name: "valószínűség",
				description: "a gammaeloszláshoz tartozó valószínűség; 0 és 1 közti szám, a végpontokat is beleértve"
			},
			{
				name: "alfa",
				description: "az eloszlás paramétere, pozitív szám"
			},
			{
				name: "béta",
				description: "az eloszlás paramétere, pozitív szám. Ha béta = 1, akkor a GAMMA.INVERZ a standard gammaeloszlással számol"
			}
		]
	},
	{
		name: "GAMMALN",
		description: "A gamma-függvény természetes logaritmusát számítja ki.",
		arguments: [
			{
				name: "x",
				description: "az az érték (pozitív szám), amelyre a GAMMALN függvény értékét ki kell számítani"
			}
		]
	},
	{
		name: "GAMMALN.PONTOS",
		description: "A gamma-függvény természetes logaritmusát számítja ki.",
		arguments: [
			{
				name: "x",
				description: "Az az érték (pozitív szám), amelyre a GAMMALN.PONTOS függvény értékét ki kell számítani."
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
		name: "GYAKORISÁG",
		description: "A gyakorisági vagy empirikus eloszlás értékét (milyen gyakran fordulnak elő az értékek egy értéktartományban) a csoport_tömbnél eggyel több elemet tartalmazó függőleges tömbként adja eredményül.",
		arguments: [
			{
				name: "adattömb",
				description: "azon adatokat tartalmazó tömb ill. azon adatokra való hivatkozás, amelyek gyakorisági eloszlását meg kell határozni (az üres és a szöveges cellák nem lesznek figyelembevéve)"
			},
			{
				name: "csoport_tömb",
				description: "azon intervallumokat tartalmazó tömb ill. azon intervallumokra való hivatkozás, amelyekbe az adattömbbeli értékeket csoportosítani kell"
			}
		]
	},
	{
		name: "GYÖK",
		description: "Egy szám négyzetgyökét számítja ki.",
		arguments: [
			{
				name: "szám",
				description: "az a szám, amelynek négyzetgyökét ki kell számítani"
			}
		]
	},
	{
		name: "GYÖKPI",
		description: "A (szám * pi) érték négyzetgyökét adja vissza.",
		arguments: [
			{
				name: "szám",
				description: "az a szám, amellyel pi-t meg kell szorozni"
			}
		]
	},
	{
		name: "HA",
		description: "Ellenőrzi a feltétel megfelelését, és ha a megadott feltétel IGAZ , az egyik értéket adja vissza, ha HAMIS, akkor a  másikat.",
		arguments: [
			{
				name: "logikai_vizsgálat",
				description: "olyan érték vagy kifejezés, amely kiértékeléskor IGAZ vagy HAMIS értéket vesz fel"
			},
			{
				name: "érték_ha_igaz",
				description: "ezt az értéket adja a függvény eredményül, ha a logikai_vizsgálat eredménye IGAZ. Ha elhagyjuk, az eredmény IGAZ lesz. Legfeljebb hét HA ágyazható egymásba"
			},
			{
				name: "érték_ha_hamis",
				description: "ezt az értéket adja a függvény eredményül, ha a logikai_vizsgálat eredménye HAMIS. Ha elhagyjuk, az eredmény HAMIS lesz"
			}
		]
	},
	{
		name: "HAHIÁNYZIK",
		description: "A megadott értéket adja vissza, ha a kifejezés #HIÁNYZIK eredményt ad, egyébként a kifejezés eredményét adja vissza.",
		arguments: [
			{
				name: "érték",
				description: "tetszőleges érték, kifejezés vagy hivatkozás"
			},
			{
				name: "érték_ha_hiányzik",
				description: "tetszőleges érték, kifejezés vagy hivatkozás"
			}
		]
	},
	{
		name: "HAHIBA",
		description: "Ha a kifejezés hiba, akkor az érték_hiba_esetén értéket, máskülönben magát a kifejezés értékét adja vissza.",
		arguments: [
			{
				name: "érték",
				description: "tetszőleges érték, kifejezés vagy hivatkozás"
			},
			{
				name: "érték_hiba_esetén",
				description: "tetszőleges érték, kifejezés vagy hivatkozás"
			}
		]
	},
	{
		name: "HAMIS",
		description: "A HAMIS logikai értéket adja eredményül.",
		arguments: [
		]
	},
	{
		name: "HARM.KÖZÉP",
		description: "Pozitív számok halmazának harmonikus átlagát számítja ki: a számok reciprokai számtani közepének a reciprokát.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások (számuk 1 és 255 között lehet), amelyek harmonikus középértékét ki kell számítani"
			},
			{
				name: "szám2",
				description: "számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások (számuk 1 és 255 között lehet), amelyek harmonikus középértékét ki kell számítani"
			}
		]
	},
	{
		name: "HATVÁNY",
		description: "Egy szám adott kitevőjű hatványát számítja ki.",
		arguments: [
			{
				name: "szám",
				description: "az alap (szám), bármilyen valós szám lehet"
			},
			{
				name: "kitevő",
				description: "a kitevő (amelyre az alapot emelni kell)"
			}
		]
	},
	{
		name: "HELYETTE",
		description: "Egy szövegdarabban a régi_szöveg előfordulásait az új_szövegre cseréli ki.",
		arguments: [
			{
				name: "szöveg",
				description: "az a szöveg vagy arra a szöveget tartalmazó cellára való hivatkozás, amelyben a karaktereket ki kell cserélni"
			},
			{
				name: "régi_szöveg",
				description: "a kicserélendő karaktersorozat. Ha a régi szövegben a kis- és nagybetűk nem felelnek meg az új szövegnek, a függvény nem végzi el a cserét."
			},
			{
				name: "új_szöveg",
				description: "az a szövegdarab, amelyre a régi_szöveget ki kell cserélni"
			},
			{
				name: "melyiket",
				description: "a régi_szöveg azon előfordulásának a sorszámát adja meg, amelyet ki kell cserélni. Ha elhagyjuk, a régi_szöveg minden példánya cserélve lesz."
			}
		]
	},
	{
		name: "HÉT.NAPJA",
		description: "Egy dátumhoz a hét egy napját azonosító számot ad eredményül, 1-től 7-ig.",
		arguments: [
			{
				name: "időérték",
				description: "dátumot jelölő szám"
			},
			{
				name: "eredmény_típusa",
				description: "szám: vasárnap=1 -- szombat=7 számozáshoz 1;hétfő=1 -- vasárnap=7 számozáshoz 2; hétfő=0 -- vasárnap=6 számozáshoz 3"
			}
		]
	},
	{
		name: "HÉT.SZÁMA",
		description: "Megadja, hogy az adott dátum hányadik hétre esik az évben.",
		arguments: [
			{
				name: "dátumérték",
				description: "a Spreadsheet által a dátum- és időértékek számolásánál használt dátumérték"
			},
			{
				name: "vissza_típus",
				description: "szám (1 vagy 2), amely a hét kezdőnapját határozza meg"
			}
		]
	},
	{
		name: "HEX.BIN",
		description: "Hexadecimális számot binárissá alakít át.",
		arguments: [
			{
				name: "szám",
				description: "az átalakítandó hexadecimális szám"
			},
			{
				name: "jegyek",
				description: "a használandó karakterek száma"
			}
		]
	},
	{
		name: "HEX.DEC",
		description: "Hexadecimális számot decimálissá alakít át.",
		arguments: [
			{
				name: "szám",
				description: "az átalakítandó hexadecimális szám"
			}
		]
	},
	{
		name: "HEX.OKT",
		description: "Hexadecimális számot oktálissá alakít át.",
		arguments: [
			{
				name: "szám",
				description: "az átalakítandó hexadecimális szám"
			},
			{
				name: "jegyek",
				description: "a használandó karakterek száma"
			}
		]
	},
	{
		name: "HIÁNYZIK",
		description: "Eredménye a #HIÁNYZIK (az érték nem áll rendelkezésre) hibaérték.",
		arguments: [
		]
	},
	{
		name: "HIBA.E",
		description: "Megvizsgálja, hogy az érték valamelyik hibaérték-e ( #ÉRTÉK!, #HIV!, #ZÉRÓOSZTÓ!, #SZÁM!, #NÉV? vagy #NULLA!), a #HIÁNYZIK kivétel, és IGAZ vagy HAMIS értéket ad.",
		arguments: [
			{
				name: "érték",
				description: "a megvizsgálni kívánt érték. Az érték hivatkozhat cellára, képletre vagy olyan névre, amely cellára, képletre vagy értékre hivatkozik"
			}
		]
	},
	{
		name: "HIBA.TÍPUS",
		description: "Eredményül egy hibaértékhez tartozó számot ad vissza.",
		arguments: [
			{
				name: "hibaérték",
				description: "az a hibaérték, amelynek azonosítószámát meg kívánja kapni. Tényleges hibaérték vagy hibaértéket tartalmazó cellára történő hivatkozás lehet"
			}
		]
	},
	{
		name: "HIBAF",
		description: "A hibaintegrál vagy hibafüggvény értékét adja eredményül.",
		arguments: [
			{
				name: "alsó_határ",
				description: "az integrál alsó határa"
			},
			{
				name: "felső_határ",
				description: "az integrál felső határa"
			}
		]
	},
	{
		name: "HIBAF.KOMPLEMENTER",
		description: "A hibaintegrál komplemensének értékét adja eredményül.",
		arguments: [
			{
				name: "x",
				description: "az improprius integrál alsó határa"
			}
		]
	},
	{
		name: "HIBAF.PONTOS",
		description: "A hibaintegrál értékét adja eredményül.",
		arguments: [
			{
				name: "x",
				description: "A HIBAF.PONTOS függvény integrálásának alsó határa."
			}
		]
	},
	{
		name: "HIBAFKOMPLEMENTER.PONTOS",
		description: "A hibaintegrál komplemensének értékét adja eredményül.",
		arguments: [
			{
				name: "x",
				description: "A HIBAFKOMPLEMENTER.PONTOS függvény integrálásának alsó határa."
			}
		]
	},
	{
		name: "HIBÁS",
		description: "Megvizsgálja, hogy az érték valamelyik hibaérték-e (#HIÁNYZIK, #ÉRTÉK!, #HIV!, #ZÉRÓOSZTÓ!, #SZÁM!, #NÉV? vagy #NULLA!), és IGAZ vagy HAMIS értéket ad.",
		arguments: [
			{
				name: "érték",
				description: "a megvizsgálni kívánt érték. Az érték hivatkozhat cellára, képletre vagy olyan névre, amely cellára, képletre vagy értékre hivatkozik"
			}
		]
	},
	{
		name: "HIPERGEOM.ELOSZLÁS",
		description: "A hipergeometriai eloszlás értékét számítja ki.",
		arguments: [
			{
				name: "minta_s",
				description: "a mintabeli sikeres kísérletek száma"
			},
			{
				name: "hány_minta",
				description: "a minta mérete"
			},
			{
				name: "sokaság_s",
				description: "a statisztikai sokaságbeli sikeres kísérletek száma"
			},
			{
				name: "sokaság_mérete",
				description: "a statisztikai sokaság mérete"
			}
		]
	},
	{
		name: "HIPERHIVATKOZÁS",
		description: "Helyi menüt vagy ugróhivatkozást létesít a merevlemezen, hálózati kiszolgálón vagy az Interneten tárolt dokumentum megnyitásához.",
		arguments: [
			{
				name: "hivatkozott_hely",
				description: "szöveg, mely megadja a megnyitandó dokumentum elérési útvonalát és a fájlnevet; lehet merevlemez-hely, UNC cím vagy URL elérési út."
			},
			{
				name: "rövid_név",
				description: "a cellában megjelenő szöveg vagy szám. Ha elhagyja, a cellában a hivatkozott_hely szövege jelenik meg."
			}
		]
	},
	{
		name: "HIPGEOM.ELOSZLÁS",
		description: "A hipergeometriai eloszlás értékét számítja ki.",
		arguments: [
			{
				name: "minta_s",
				description: "a mintabeli sikeres kísérletek száma"
			},
			{
				name: "hány_minta",
				description: "a minta mérete"
			},
			{
				name: "sokaság_s",
				description: "a statisztikai sokaságbeli sikeres kísérletek száma"
			},
			{
				name: "sokaság_mérete",
				description: "a statisztikai sokaság mérete"
			},
			{
				name: "eloszlásfv",
				description: "a függvény fajtáját megadó logikai érték: ha IGAZ, az eloszlásfüggvény értékét számítja ki, ha HAMIS, a sűrűségfüggvényét"
			}
		]
	},
	{
		name: "HIVATKOZÁS",
		description: "Megvizsgálja, hogy az érték hivatkozás-e, és IGAZ vagy HAMIS értéket ad vissza.",
		arguments: [
			{
				name: "érték",
				description: "a megvizsgálni kívánt érték. Az érték hivatkozhat cellára, képletre vagy olyan névre, amely cellára, képletre vagy értékre hivatkozik"
			}
		]
	},
	{
		name: "HOL.VAN",
		description: "Egy adott értéknek megfelelő tömbelem viszonylagos helyét adja meg adott sorrendben.",
		arguments: [
			{
				name: "keresési_érték",
				description: "az az érték, amelynek segítségével a tömbben a keresett érték megtalálható; szám, szöveg, logikai érték vagy ezek egyikére való hivatkozás"
			},
			{
				name: "tábla",
				description: "lehetséges keresési értékeket tartalmazó összefüggő cellatartomány, értékekből álló tömb vagy tömbhivatkozás "
			},
			{
				name: "egyezés_típus",
				description: "értéke -1, 0 vagy 1 lehet, a visszaadandó értéket jelzi."
			}
		]
	},
	{
		name: "HÓNAP",
		description: "Megadja a hónapot, egy 1 (január) és 12 (december) közötti számmal.",
		arguments: [
			{
				name: "időérték",
				description: "Spreadsheet dátum- és időértékben megadott szám"
			}
		]
	},
	{
		name: "HÓNAP.UTOLSÓ.NAP",
		description: "A kezdő_dátum-nál hónapok hónappal korábbi vagy későbbi hónap utolsó napjának dátumértékét adja eredményül.",
		arguments: [
			{
				name: "kezdő_dátum",
				description: "a kezdő_dátum dátumértéke"
			},
			{
				name: "hónapok",
				description: "a hónapok száma a kezdő_dátum előtt vagy után"
			}
		]
	},
	{
		name: "HOSSZ",
		description: "Egy szöveg karakterekben mért hosszát adja eredményül.",
		arguments: [
			{
				name: "szöveg",
				description: "az a szöveg, amelynek a hosszát meg kell határozni. A szóköz is karakternek számít."
			}
		]
	},
	{
		name: "HOZAM.LESZÁM",
		description: "Egy leszámítolt értékpapír (például kincstárjegy) éves hozamát adja eredményül.",
		arguments: [
			{
				name: "kiegyenlítés",
				description: "az értékpapír kifizetésének időpontja dátumértékként kifejezve"
			},
			{
				name: "lejárat",
				description: "az értékpapír lejáratának időpontja dátumértékként kifejezve"
			},
			{
				name: "ár",
				description: "a 100 Ft névértékű értékpapír ára"
			},
			{
				name: "visszaváltás",
				description: "a 100 Ft névértékű értékpapír visszaváltási árfolyama"
			},
			{
				name: "alap",
				description: "az évtöredék számításának alapja"
			}
		]
	},
	{
		name: "IDŐ",
		description: "Az óra, perc, másodperc alakban megadott időpont időértékét adja meg időformátum alakban.",
		arguments: [
			{
				name: "óra",
				description: "az órát jelölő szám, 0 és 23 között lehet"
			},
			{
				name: "perc",
				description: "a percet jelölő szám, 0 és 59 között lehet"
			},
			{
				name: "másodperc",
				description: "a másodpercet jelölő szám, 0 és 59 között lehet"
			}
		]
	},
	{
		name: "IDŐÉRTÉK",
		description: "A szövegként megadott időpontot időértékké, azaz 0 (0:00:00, azaz éjfél) és 0,999988426 (este 11:59:59) közötti számmá alakít át. A képlet beírását követően időformátumot kell hozzárendelni.",
		arguments: [
			{
				name: "idő_szöveg",
				description: "a Spreadsheet valamely időformátumában szövegként megadott időpont (a szövegben lévő dátumadatok nem lesznek figyelembe véve)"
			}
		]
	},
	{
		name: "IGAZ",
		description: "Az IGAZ logikai értéket adja eredményül.",
		arguments: [
		]
	},
	{
		name: "INDEX",
		description: "Értéket vagy hivatkozást ad vissza egy adott tartomány bizonyos sorának és oszlopának metszéspontjában lévő cellából.",
		arguments: [
			{
				name: "tömb",
				description: "cellatartomány vagy tömbkonstans."
			},
			{
				name: "sor_szám",
				description: "kijelöli a tömb vagy hivatkozás azon sorát, amelyből az értéket vissza kell adni. Ha elhagyja, az oszlop_szám megadása szükséges"
			},
			{
				name: "oszlop_szám",
				description: "kijelöli a tömb vagy hivatkozás azon oszlopát, amelyből az értéket vissza kell adni. Ha elhagyja, a sor_szám megadása szükséges"
			}
		]
	},
	{
		name: "INDIREKT",
		description: "Szövegdarab által meghatározott hivatkozást ad eredményül.",
		arguments: [
			{
				name: "hiv_szöveg",
				description: "olyan cellára való hivatkozás, amely egy A1 vagy S1O1 típusú hivatkozást, hivatkozásként definiált nevet vagy pedig szövegként szereplő cellahivatkozást tartalmaz"
			},
			{
				name: "a1",
				description: "a hiv_szöveg argumentum által meghatározott hivatkozás típusát megadó logikai érték: S1O1 típus esetén HAMIS, A1 típus esetén IGAZ vagy elhagyható"
			}
		]
	},
	{
		name: "INFÓ",
		description: "A rendszerkörnyezet pillanatnyi állapotáról ad felvilágosítást.",
		arguments: [
			{
				name: "típus_szöveg",
				description: "a kívánt információ típusát határozza meg."
			}
		]
	},
	{
		name: "INT",
		description: "Egy számot lefelé kerekít a legközelebbi egészre.",
		arguments: [
			{
				name: "szám",
				description: "a kerekítendő szám"
			}
		]
	},
	{
		name: "INVERZ.BÉTA",
		description: "A bétaeloszlás sűrűségfüggvény (BÉTA.ELOSZLÁS) inverzét számítja ki.",
		arguments: [
			{
				name: "valószínűség",
				description: "a bétaeloszláshoz tartozó valószínűségérték"
			},
			{
				name: "alfa",
				description: "az eloszlás paramétere, nullánál nagyobbnak kell lennie"
			},
			{
				name: "béta",
				description: "az eloszlás paramétere, nullánál nagyobbnak kell lennie"
			},
			{
				name: "A",
				description: "az x-ek intervallumának alsó határa; nem kötelező megadni. Ha elhagyjuk, A = 0."
			},
			{
				name: "B",
				description: "az x-ek intervallumának felső határa; nem kötelező megadni. Ha elhagyjuk, B = 1."
			}
		]
	},
	{
		name: "INVERZ.F",
		description: "Az F-eloszlás inverzének (jobbszélű) értékét számítja ki: ha p = F.ELOSZLÁS(x,...), akkor INVERZ.F(p,...) = x.",
		arguments: [
			{
				name: "valószínűség",
				description: "az F-eloszláshoz tartozó valószínűségérték; 0 és 1 közti szám a végpontokat is beleértve"
			},
			{
				name: "szabadságfok1",
				description: "a számláló szabadságfoka, 1 és 10^10 közötti szám a 10^10 kivételével"
			},
			{
				name: "szabadságfok2",
				description: "a nevező szabadságfoka, 1 és 10^10 közötti szám a 10^10 kivételével"
			}
		]
	},
	{
		name: "INVERZ.FISHER",
		description: "A Fisher-transzformáció inverzét hajtja végre: ha y = FISHER(x), akkor INVERZ.FISHER(y) = x.",
		arguments: [
			{
				name: "y",
				description: "az az érték, amelyre a transzformáció inverzét végre kell hajtani"
			}
		]
	},
	{
		name: "INVERZ.GAMMA",
		description: "A gammaeloszlás eloszlásfüggvénye inverzének értékét számítja ki: ha p = GAMMA.ELOSZLÁS(x,...), akkor INVERZ.GAMMA(p,...) = x.",
		arguments: [
			{
				name: "valószínűség",
				description: "a gammaeloszláshoz tartozó valószínűség; 0 és 1 közti szám, a végpontokat is beleértve"
			},
			{
				name: "alfa",
				description: "az eloszlás paramétere, pozitív szám"
			},
			{
				name: "béta",
				description: "az eloszlás paramétere, pozitív szám. Ha béta = 1, akkor az INVERZ.GAMMA a standard gammaeloszlással számol"
			}
		]
	},
	{
		name: "INVERZ.KHI",
		description: "A khi-négyzet eloszlás jobbszélű inverzét számítja ki.",
		arguments: [
			{
				name: "valószínűség",
				description: "a khi-négyzet eloszláshoz tartozó valószínűségérték; 0 és 1 közti érték a végpontokat is beleértve"
			},
			{
				name: "szabadságfok",
				description: "a szabadságfokok száma, 1 és 10^10 közötti szám a 10^10 kivételével"
			}
		]
	},
	{
		name: "INVERZ.LOG.ELOSZLÁS",
		description: "A lognormális eloszlás inverzét számítja ki x-re; ln(x) normális eloszlását a középérték és szórás paraméterei adják meg.",
		arguments: [
			{
				name: "valószínűség",
				description: "a lognormális eloszláshoz tartozó valószínűség; 0 és 1 közti szám, a végpontokat is beleértve"
			},
			{
				name: "középérték",
				description: "az ln(x) középértéke"
			},
			{
				name: "szórás",
				description: "az ln(x) szórása, pozitív szám"
			}
		]
	},
	{
		name: "INVERZ.MÁTRIX",
		description: "Egy tömbben tárolt mátrix inverz mátrixát adja eredményül.",
		arguments: [
			{
				name: "tömb",
				description: "egy csak számokat tartalmazó négyzetes (ugyanannyi sorból és oszlopból álló) tömb; cellatartomány vagy tömbkonstans lehet."
			}
		]
	},
	{
		name: "INVERZ.NORM",
		description: "A normális eloszlás eloszlásfüggvénye inverzének értékét számítja ki a megadott középérték és szórás esetén.",
		arguments: [
			{
				name: "valószínűség",
				description: "a normális eloszláshoz tartozó valószínűség; 0 és 1 közötti szám a végpontokat is beleértve"
			},
			{
				name: "középérték",
				description: "az eloszlás középértéke (várható értéke)"
			},
			{
				name: "szórás",
				description: "az eloszlás szórása, pozitív szám"
			}
		]
	},
	{
		name: "INVERZ.STNORM",
		description: "A standard normális eloszlás eloszlásfüggvénye inverzének értékét számítja ki. A standard normális eloszlás középértéke 0, szórása 1.",
		arguments: [
			{
				name: "valószínűség",
				description: "a normális eloszláshoz tartozó valószínűség; 0 és 1 közötti szám a végpontokat is beleértve"
			}
		]
	},
	{
		name: "INVERZ.T",
		description: "A Student-féle t-eloszlás kétszélű inverzét számítja ki.",
		arguments: [
			{
				name: "valószínűség",
				description: "a Student-féle t-eloszláshoz tartozó valószínűség; 0 és 1 közötti szám a végpontokat is beleértve"
			},
			{
				name: "szabadságfok",
				description: "az eloszlás szabadságfokának számát jelző pozitív szám"
			}
		]
	},
	{
		name: "ISO.HÉT.SZÁMA",
		description: "Az év ISO-hétszámát adja vissza egy megadott dátumhoz.",
		arguments: [
			{
				name: "dátum",
				description: "a dátum/idő kód a Spreadsheet dátum- és időpontszámításhoz használt formátumában"
			}
		]
	},
	{
		name: "ISO.PLAFON",
		description: "Egy számot a legközelebbi egészre vagy a pontosságként megadott érték legközelebb eső többszörösére kerekít fel.",
		arguments: [
			{
				name: "szám",
				description: "a kerekítendő szám"
			},
			{
				name: "pontosság",
				description: "az az opcionális szám, amelynek legközelebbi többszörösére kerekíteni kell"
			}
		]
	},
	{
		name: "JBÉ",
		description: "Egy befektetés jövőbeli értékét számítja ki, ismétlődő állandó kifizetéseket és állandó kamatlábat véve alapul.",
		arguments: [
			{
				name: "ráta",
				description: "az időszakonkénti kamatláb. Például 6%-os éves kamatláb negyedévenkénti fizetéssel 6%/4"
			},
			{
				name: "időszakok_száma",
				description: "a fizetési időszakok száma a befektetési időszakban"
			},
			{
				name: "részlet",
				description: "a fizetési időszakokban esedékes kifizetés; nagysága a befektetési időszak egészében változatlan"
			},
			{
				name: "mai_érték",
				description: "a jövőbeli kifizetések jelenértéke, vagyis az a jelenbéli egyösszegű kifizetés, amely egyenértékű a jövőbeli kifizetések összegével. Ha elhagyjuk, a jelenérték = 0"
			},
			{
				name: "típus",
				description: "a résztörlesztések esedékességét megadó érték: 1 esetén a kifizetés az időszak elején történik, 0 vagy elhagyása esetén az időszak végén"
			}
		]
	},
	{
		name: "JOBB",
		description: "Egy szövegrész végétől megadott számú karaktert ad eredményül.",
		arguments: [
			{
				name: "szöveg",
				description: "az a szövegrész, amelyből ki kell venni a karaktereket"
			},
			{
				name: "hány_karakter",
				description: "azt határozza meg, hogy hány karaktert adjon eredményül; elhagyása esetén értéke 1"
			}
		]
	},
	{
		name: "KALK.DÁTUM",
		description: "A kezdő_dátum-nál hónapok hónappal korábbi vagy későbbi dátum dátumértékét adja eredményül.",
		arguments: [
			{
				name: "kezdő_dátum",
				description: "a kezdő_dátum dátumértéke"
			},
			{
				name: "hónapok",
				description: "a hónapok száma a kezdő_dátum előtt vagy után"
			}
		]
	},
	{
		name: "KALK.MUNKANAP",
		description: "A kezdő_dátum-nál napok munkanappal korábbi vagy későbbi dátum dátumértékét adja eredményül.",
		arguments: [
			{
				name: "kezdő_dátum",
				description: "a kezdő_dátum dátumértéke"
			},
			{
				name: "napok",
				description: "napok száma ünnep- és munkaszüneti napok nélkül a kezdő_dátum előtt vagy után"
			},
			{
				name: "ünnepek",
				description: "a nem munkanap napok (állami, egyházi stb. ünnepek) dátumértékét tartalmazó tömb"
			}
		]
	},
	{
		name: "KALK.MUNKANAP.INTL",
		description: "A megadott számú munkanap előtti vagy utáni dátumértéket adja meg, az egyéni hétvége-paraméterek mellett.",
		arguments: [
			{
				name: "kezdő_dátum",
				description: "a kezdő_dátum dátumértéke"
			},
			{
				name: "napok",
				description: "a nem hétvégére vagy ünnepre eső napok száma a kezdő_dátum előtt vagy után"
			},
			{
				name: "hétvége",
				description: "a hétvégéket megadó szám vagy karakterlánc"
			},
			{
				name: "ünnepek",
				description: "a nem munkanapok (állami, egyházi stb. ünnepek) dátumértékét tartalmazó, nem kötelező halmaz"
			}
		]
	},
	{
		name: "KAMATÉRZ.PER",
		description: "Kiszámítja az ahhoz szükséges időszakok számát, hogy egy befektetés elérjen egy megadott értéket.",
		arguments: [
			{
				name: "ráta",
				description: "az időszakonkénti kamatláb"
			},
			{
				name: "jelenérték",
				description: "a befektetés jelenértéke"
			},
			{
				name: "jövőérték",
				description: "a befektetéssel elérni kívánt jövőérték"
			}
		]
	},
	{
		name: "KAMATRÁTA",
		description: "Egy lejáratig teljesen lekötött értékpapír kamatrátáját adja eredményül.",
		arguments: [
			{
				name: "kiegyenlítés",
				description: "az értékpapír kifizetésének időpontja dátumértékként kifejezve"
			},
			{
				name: "lejárat",
				description: "az értékpapír lejáratának időpontja dátumértékként kifejezve"
			},
			{
				name: "befektetés",
				description: "az értékpapírba fektetett összeg"
			},
			{
				name: "visszaváltás",
				description: "a lejáratkor esedékes összeg"
			},
			{
				name: "alap",
				description: "az évtöredék számításának alapja"
			}
		]
	},
	{
		name: "KAPOTT",
		description: "Egy lejáratig teljesen lekötött értékpapír lejáratakor kapott összeget adja eredményül.",
		arguments: [
			{
				name: "kiegyenlítés",
				description: "az értékpapír kifizetésének időpontja dátumértékként kifejezve"
			},
			{
				name: "lejárat",
				description: "az értékpapír lejáratának időpontja dátumértékként kifejezve"
			},
			{
				name: "befektetés",
				description: "az értékpapírba fektetett összeg"
			},
			{
				name: "leszámítolás",
				description: "az értékpapír leszámítolási rátája"
			},
			{
				name: "alap",
				description: "az évtöredék számításának alapja"
			}
		]
	},
	{
		name: "KARAKTER",
		description: "A kódszám által meghatározott karaktert adja eredményül a számítógépen beállított karakterkészletből.",
		arguments: [
			{
				name: "szám",
				description: "a kívánt karakter kódja; 1 és 255 közé eső egész szám"
			}
		]
	},
	{
		name: "KCS2",
		description: "Eredményül egy eszköz adott időszak alatti értékcsökkenését számítja ki a lineáris leírási modell alkalmazásával.",
		arguments: [
			{
				name: "költség",
				description: "az eszköz beszerzési ára"
			},
			{
				name: "maradványérték",
				description: "az eszköz maradványértéke a leírási idő eltelte után"
			},
			{
				name: "leírási_idő",
				description: "a leírási időszakok teljes száma (azaz az eszköz hasznos élettartama)"
			},
			{
				name: "időszak",
				description: "az az időszak, amelyre az értékcsökkenés mértékét ki kell számítani; ugyanazt a mértékegységet kell használni, mint az élettartam megadásánál"
			},
			{
				name: "hónap",
				description: "a leírás első évében számításba veendő hónapok száma. Ha nincs megadva, akkor a KCS2 függvény ezt 12-nek tekinti."
			}
		]
	},
	{
		name: "KCSA",
		description: "Egy eszköz értékcsökkenését számítja ki egy adott időszakra vonatkozóan a progresszív vagy egyéb megadott leírási modell alkalmazásával.",
		arguments: [
			{
				name: "költség",
				description: "az eszköz beszerzési ára"
			},
			{
				name: "maradványérték",
				description: "az eszköz maradványértéke a leírási idő eltelte után"
			},
			{
				name: "leírási_idő",
				description: "a leírási időszakok teljes száma (azaz az eszköz hasznos élettartama)"
			},
			{
				name: "időszak",
				description: "az az időszak, amelyre az értékcsökkenés mértékét ki kell számítani; ugyanazt a mértékegységet kell használni, mint az élettartam megadásánál"
			},
			{
				name: "faktor",
				description: "a leírás gyorsasága. Ha nincs megadva, akkor értékét a Spreadsheet 2-nek veszi, azaz progresszív leírási modellel számol."
			}
		]
	},
	{
		name: "KÉPLET",
		description: "Megvizsgálja, hogy egy hivatkozás képletet tartalmazó cellára mutat-e, és IGAZ vagy HAMIS értéket ad vissza.",
		arguments: [
			{
				name: "hivatkozás",
				description: "a vizsgálandó cellára mutató hivatkozás, amely lehet cellahivatkozás, képlet vagy cellára hivatkozó név"
			}
		]
	},
	{
		name: "KÉPLETSZÖVEG",
		description: "Egy képletet karakterláncként ad vissza.",
		arguments: [
			{
				name: "hivatkozás",
				description: "egy képletre mutató hivatkozás"
			}
		]
	},
	{
		name: "KÉPZ.ABSZ",
		description: "Komplex szám abszolút értékét (modulusát) adja eredményül.",
		arguments: [
			{
				name: "k_szám",
				description: "az a komplex szám, amelynek abszolút értékét keressük"
			}
		]
	},
	{
		name: "KÉPZ.ARGUMENT",
		description: "A komplex szám radiánban kifejezett argumentumát adja eredményül.",
		arguments: [
			{
				name: "k_szám",
				description: "az a komplex szám, amelynek argumentumát keressük"
			}
		]
	},
	{
		name: "KÉPZ.COS",
		description: "Komplex szám koszinuszát adja eredményül.",
		arguments: [
			{
				name: "k_szám",
				description: "az a komplex szám, amelynek a koszinuszát keressük"
			}
		]
	},
	{
		name: "KÉPZ.COSH",
		description: "Egy komplex szám hiperbolikus koszinuszát számítja ki.",
		arguments: [
			{
				name: "k_szám",
				description: "az a komplex szám, amelynek a hiperbolikus koszinuszát keresi"
			}
		]
	},
	{
		name: "KÉPZ.COT",
		description: "Egy komplex szám kotangensét számítja ki.",
		arguments: [
			{
				name: "k_szám",
				description: "az a komplex szám, amelynek a kotangensét keresi"
			}
		]
	},
	{
		name: "KÉPZ.CSC",
		description: "Egy komplex szám koszekánsát számítja ki.",
		arguments: [
			{
				name: "k_szám",
				description: "az a komplex szám, amelynek a koszekánsát keresi"
			}
		]
	},
	{
		name: "KÉPZ.CSCH",
		description: "Egy komplex szám hiperbolikus koszekánsát számítja ki.",
		arguments: [
			{
				name: "k_szám",
				description: "az a komplex szám, amelynek a hiperbolikus koszekánsát keresi"
			}
		]
	},
	{
		name: "KÉPZ.EXP",
		description: "Az e szám komplex kitevőjű hatványát adja eredményül.",
		arguments: [
			{
				name: "k_szám",
				description: "az a komplex szám, amelyre e-t emelni kívánjuk"
			}
		]
	},
	{
		name: "KÉPZ.GYÖK",
		description: "Komplex szám négyzetgyökét adja eredményül.",
		arguments: [
			{
				name: "k_szám",
				description: "az a komplex szám, amelynek négyzetgyökét keressük"
			}
		]
	},
	{
		name: "KÉPZ.HÁNYAD",
		description: "Két komplex szám hányadosát adja eredményül.",
		arguments: [
			{
				name: "k_szám1",
				description: "a komplex számláló vagy osztandó"
			},
			{
				name: "k_szám2",
				description: "a komplex nevező vagy osztó"
			}
		]
	},
	{
		name: "KÉPZ.HATV",
		description: "Komplex szám hatványát adja eredményül.",
		arguments: [
			{
				name: "k_szám",
				description: "a hatványozandó komplex szám"
			},
			{
				name: "szám",
				description: "a hatványkitevő"
			}
		]
	},
	{
		name: "KÉPZ.KONJUGÁLT",
		description: "Komplex szám komplex konjugáltját adja eredményül.",
		arguments: [
			{
				name: "k_szám",
				description: "az a komplex szám, amelynek a konjugáltját keressük"
			}
		]
	},
	{
		name: "KÉPZ.KÜL",
		description: "Két komplex szám különbségét adja eredményül.",
		arguments: [
			{
				name: "k_szám1",
				description: "az a komplex szám, amelyből k_szám2-t ki kell vonni"
			},
			{
				name: "k_szám2",
				description: "az a komplex szám, amelyet a k_szám1-ből ki kell vonni"
			}
		]
	},
	{
		name: "KÉPZ.LN",
		description: "Komplex szám természetes logaritmusát adja eredményül.",
		arguments: [
			{
				name: "k_szám",
				description: "az a komplex szám, amelynek természetes logaritmusát keressük"
			}
		]
	},
	{
		name: "KÉPZ.LOG10",
		description: "Komplex szám tízes alapú logaritmusát adja eredményül.",
		arguments: [
			{
				name: "k_szám",
				description: "az a komplex szám, amelynek tízes alapú logaritmusát keressük"
			}
		]
	},
	{
		name: "KÉPZ.LOG2",
		description: "Komplex szám kettes alapú logaritmusát adja eredményül.",
		arguments: [
			{
				name: "k_szám",
				description: "az a komplex szám, amelynek kettes alapú logaritmusát keressük"
			}
		]
	},
	{
		name: "KÉPZ.ÖSSZEG",
		description: "Komplex számok összegének visszakeresése.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "k_szám1",
				description: "az összeadandó 1–255 komplex szám"
			},
			{
				name: "k_szám2",
				description: "az összeadandó 1–255 komplex szám"
			}
		]
	},
	{
		name: "KÉPZ.SEC",
		description: "Egy komplex szám szekánsát számítja ki.",
		arguments: [
			{
				name: "k_szám",
				description: "az a komplex szám, amelynek a szekánsát keresi"
			}
		]
	},
	{
		name: "KÉPZ.SECH",
		description: "Egy komplex szám hiperbolikus szekánsát számítja ki.",
		arguments: [
			{
				name: "k_szám",
				description: "az a komplex szám, amelynek a hiperbolikus szekánsát keresi"
			}
		]
	},
	{
		name: "KÉPZ.SIN",
		description: "Komplex szám szinuszát adja eredményül.",
		arguments: [
			{
				name: "k_szám",
				description: "az a komplex szám, amelynek a szinuszát keressük"
			}
		]
	},
	{
		name: "KÉPZ.SINH",
		description: "Egy komplex szám hiperbolikus szinuszát számítja ki.",
		arguments: [
			{
				name: "k_szám",
				description: "az a komplex szám, amelynek a hiperbolikus szinuszát keresi"
			}
		]
	},
	{
		name: "KÉPZ.SZORZAT",
		description: "1–255 komplex szám szorzatának kiszámítása.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "k_szám1",
				description: "k_szám1, k_szám2,... : az összeszorzandó 1–255 komplex szám."
			},
			{
				name: "k_szám2",
				description: "k_szám1, k_szám2,... : az összeszorzandó 1–255 komplex szám."
			}
		]
	},
	{
		name: "KÉPZ.TAN",
		description: "Egy komplex szám tangensét számítja ki.",
		arguments: [
			{
				name: "k_szám",
				description: "az a komplex szám, amelynek a tangensét keresi"
			}
		]
	},
	{
		name: "KÉPZ.VALÓS",
		description: "Komplex szám valós részét adja eredményül.",
		arguments: [
			{
				name: "k_szám",
				description: "az a komplex szám, amelynek valós részét keressük"
			}
		]
	},
	{
		name: "KÉPZETES",
		description: "Komplex szám képzetes részét adja eredményül.",
		arguments: [
			{
				name: "k_szám",
				description: "az a komplex szám, amelynek képzetes részét keressük"
			}
		]
	},
	{
		name: "KEREK.FEL",
		description: "Egy számot felfelé, a nullától távolabbra kerekít.",
		arguments: [
			{
				name: "szám",
				description: "a kerekítendő valós szám"
			},
			{
				name: "hány_számjegy",
				description: "azon számjegyek száma, amennyi jegyre kerekíteni kell. Negatív érték esetén a tizedesponttól balra eső részhez kerekít; zérus vagy elhagyása esetén a legközelebbi egészre"
			}
		]
	},
	{
		name: "KEREK.LE",
		description: "Egy számot lefelé, a nulla felé kerekít.",
		arguments: [
			{
				name: "szám",
				description: "a kerekítendő valós szám"
			},
			{
				name: "hány_számjegy",
				description: "azon számjegyek száma, amennyi jegyre kerekíteni kell. Negatív érték esetén a tizedesponttól balra eső részhez kerekít; zérus vagy elhagyása esetén a legközelebbi egészre"
			}
		]
	},
	{
		name: "KEREKÍTÉS",
		description: "Egy számot adott számú számjegyre kerekít.",
		arguments: [
			{
				name: "szám",
				description: "a kerekítendő szám"
			},
			{
				name: "hány_számjegy",
				description: "azon számjegyek száma, amennyi jegyre kerekíteni kell. Negatív érték esetén a tizedesponttól balra eső részhez kerekít; zérus esetén a legközelebbi egészre"
			}
		]
	},
	{
		name: "KERES",
		description: "Egy sorból vagy egy oszlopból álló tartományban vagy tömbben keres meg értékeket. A korábbi verziókkal való kompatibilitásra szolgál.",
		arguments: [
			{
				name: "keresési_érték",
				description: "az az érték, amelyet a KERES függvény a keresési_vektorban keres; lehet szám, szöveg, logikai érték vagy értékre hivatkozó név vagy hivatkozás"
			},
			{
				name: "keresési_vektor",
				description: "egyetlen sorból vagy egyetlen oszlopból álló tartomány, mely szöveget, számokat vagy logikai értékeket tartalmaz, növekvő sorrendben elhelyezve"
			},
			{
				name: "eredmény_vektor",
				description: "egyetlen sorból vagy egyetlen oszlopból álló tartomány, mérete azonos a keresési_vektor méretével"
			}
		]
	},
	{
		name: "KHI.ELOSZLÁS",
		description: "A khi-négyzet eloszlás jobbszélű valószínűségértékét számítja ki.",
		arguments: [
			{
				name: "x",
				description: "az az érték (nem negatív szám), amelynél az eloszlást ki kell számítani"
			},
			{
				name: "szabadságfok",
				description: "a szabadságfokok száma, 1 és 10^10 közötti szám a 10^10 kivételével"
			}
		]
	},
	{
		name: "KHI.PRÓBA",
		description: "Függetlenségvizsgálatot hajt végre, eredményül a khi-négyzet eloszláshoz rendelt értéket adja a statisztika és a szabadságfokok megfelelő száma szerint.",
		arguments: [
			{
				name: "tényleges_tartomány",
				description: "az az adattartomány, amely a várható értékekkel összehasonlítandó megfigyelt adatokat tartalmazza"
			},
			{
				name: "várható_tartomány",
				description: "az az adattartomány, amely a sorösszegek és oszlopösszegek szorzatának a teljes összeghez viszonyított arányát tartalmazza"
			}
		]
	},
	{
		name: "KHINÉGYZET.ELOSZLÁS",
		description: "A khi-négyzet eloszlás balszélű valószínűségértékét számítja ki.",
		arguments: [
			{
				name: "x",
				description: "az az érték (nem negatív szám), amelynél az eloszlást ki kell számítani"
			},
			{
				name: "szabadságfok",
				description: "a szabadságfokok száma, 1 és 10^10 közötti szám a 10^10 kivételével"
			},
			{
				name: "eloszlásfv",
				description: "a függvény által visszaadott logikai érték: ha IGAZ, az eloszlásfüggvény értékét számítja ki, ha HAMIS, a sűrűségfüggvényét"
			}
		]
	},
	{
		name: "KHINÉGYZET.ELOSZLÁS.JOBB",
		description: "A khi-négyzet eloszlás jobbszélű valószínűségértékét számítja ki.",
		arguments: [
			{
				name: "x",
				description: "az az érték (nem negatív szám), amelynél az eloszlást ki kell számítani"
			},
			{
				name: "szabadságfok",
				description: "a szabadságfokok száma, 1 és 10^10 közötti szám a 10^10 kivételével"
			}
		]
	},
	{
		name: "KHINÉGYZET.INVERZ",
		description: "A khi-négyzet eloszlás balszélű inverzét számítja ki.",
		arguments: [
			{
				name: "valószínűség",
				description: "a khi-négyzet eloszláshoz tartozó valószínűségérték; 0 és 1 közti érték, a végpontokat is beleértve"
			},
			{
				name: "szabadságfok",
				description: "a szabadságfokok száma, 1 és 10^10 közötti szám a 10^10 kivételével"
			}
		]
	},
	{
		name: "KHINÉGYZET.INVERZ.JOBB",
		description: "A khi-négyzet eloszlás jobbszélű inverzét számítja ki.",
		arguments: [
			{
				name: "valószínűség",
				description: "a khi-négyzet eloszláshoz tartozó valószínűségérték; 0 és 1 közti érték, a végpontokat is beleértve"
			},
			{
				name: "szabadságfok",
				description: "a szabadságfokok száma, 1 és 10^10 közötti szám a 10^10 kivételével"
			}
		]
	},
	{
		name: "KHINÉGYZET.PRÓBA",
		description: "Függetlenségvizsgálatot hajt végre, eredményül a khi-négyzet eloszláshoz rendelt értéket adja a statisztika és a szabadságfokok megfelelő száma szerint.",
		arguments: [
			{
				name: "tényleges_tartomány",
				description: "az az adattartomány, amely a várható értékekkel összehasonlítandó megfigyelt adatokat tartalmazza"
			},
			{
				name: "várható_tartomány",
				description: "az az adattartomány, amely a sorösszegek és oszlopösszegek szorzatának a teljes összeghez viszonyított arányát tartalmazza"
			}
		]
	},
	{
		name: "KICSI",
		description: "Egy adathalmaz k-adik legkisebb elemét adja eredményül. Például az ötödik legkisebb számot.",
		arguments: [
			{
				name: "tömb",
				description: "az a tömb vagy adattartomány, amelynek k-adik legkisebb értékét keresi"
			},
			{
				name: "k",
				description: "azt adja meg, hogy (a legkisebbtől kezdve visszafelé) hányadik legkisebb elemet kell megkeresni"
			}
		]
	},
	{
		name: "KIMETSZ",
		description: "Egy szövegből eltávolítja az összes szóközt a szavak közti egyszeres szóközök kivételével.",
		arguments: [
			{
				name: "szöveg",
				description: "az a szöveg, amelyből a szóközöket el kell távolítani"
			}
		]
	},
	{
		name: "KIMUTATÁSADATOT.VESZ",
		description: "Kimutatásban tárolt adatokat gyűjt ki.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "adat_mező",
				description: "annak az adatmezőnek a neve, amelyből az adatokat ki szeretné gyűjteni"
			},
			{
				name: "kimutatás",
				description: "hivatkozás a kimutatás azon cellájára vagy cellatartományára, amely a beolvasandó adatokat tartalmazza"
			},
			{
				name: "mező",
				description: "hivatkozott mező"
			},
			{
				name: "tétel",
				description: "hivatkozott tétel"
			}
		]
	},
	{
		name: "KISBETŰ",
		description: "Egy szövegrészben lévő összes betűt kisbetűvé alakít át.",
		arguments: [
			{
				name: "szöveg",
				description: "a kisbetűssé átalakítandó szöveg. A szöveg azon karakterei, melyek nem betűk, nem változnak meg"
			}
		]
	},
	{
		name: "KITEVŐ",
		description: "e-nek adott kitevőjű hatványát számítja ki.",
		arguments: [
			{
				name: "szám",
				description: "az e alap kitevője. Az e konstans értéke 2.718281182845904, ez a természetes logaritmus alapja."
			}
		]
	},
	{
		name: "KJÉ",
		description: "A kezdőtőke adott kamatlábak szerint megnövelt jövőbeli értékét adja eredményül.",
		arguments: [
			{
				name: "tőke",
				description: "a jelenlegi érték"
			},
			{
				name: "ütemezés",
				description: "a kamatlábak tömbje"
			}
		]
	},
	{
		name: "KJEGY.ÁR",
		description: "Egy 100 Ft névértékű kincstárjegy árát adja eredményül.",
		arguments: [
			{
				name: "kiegyenlítés",
				description: "a kincstárjegy kifizetésének időpontja dátumértékként kifejezve"
			},
			{
				name: "lejárat",
				description: "a kincstárjegy lejáratának időpontja dátumértékként kifejezve"
			},
			{
				name: "leszámítolás",
				description: "a kincstárjegy leszámítolási rátája"
			}
		]
	},
	{
		name: "KJEGY.EGYENÉRT",
		description: "Egy kincstárjegy kötvény-egyenértékű hozamát adja eredményül.",
		arguments: [
			{
				name: "kiegyenlítés",
				description: "a kincstárjegy kifizetésének időpontja dátumértékként kifejezve"
			},
			{
				name: "lejárat",
				description: "a kincstárjegy lejáratának időpontja dátumértékként kifejezve"
			},
			{
				name: "leszámítolás",
				description: "a kincstárjegy leszámítolási rátája"
			}
		]
	},
	{
		name: "KJEGY.HOZAM",
		description: "Egy kincstárjegy hozamát adja eredményül.",
		arguments: [
			{
				name: "kiegyenlítés",
				description: "a kincstárjegy kifizetésének időpontja dátumértékként kifejezve"
			},
			{
				name: "lejárat",
				description: "a kincstárjegy lejáratának időpontja dátumértékként kifejezve"
			},
			{
				name: "ár",
				description: "egy 100 Ft névértékű kincstárjegy ára"
			}
		]
	},
	{
		name: "KÓD",
		description: "Egy szövegdarab első karakterének numerikus kódját adja eredményül a számítógépen által használt karakterkészlet szerint.",
		arguments: [
			{
				name: "szöveg",
				description: "e szöveg első karakterének kódját kívánja megkapni"
			}
		]
	},
	{
		name: "KOMBINÁCIÓK",
		description: "Adott számú elem összes lehetséges kombinációinak számát számítja ki.",
		arguments: [
			{
				name: "szám",
				description: "az összes elem száma"
			},
			{
				name: "hány_kiválasztott",
				description: "az egyes kombinációkban szereplő elemek száma"
			}
		]
	},
	{
		name: "KOMBINÁCIÓK.ISM",
		description: "Adott számú elem összes lehetséges ismétléses kombinációinak számát számítja ki.",
		arguments: [
			{
				name: "szám",
				description: "az összes elem száma"
			},
			{
				name: "hány_kiválasztott",
				description: "az egyes kombinációkban szereplő elemek száma"
			}
		]
	},
	{
		name: "KOMPLEX",
		description: "Valós és képzetes részekből komplex számot képez.",
		arguments: [
			{
				name: "valós_szám",
				description: "a komplex szám valós része"
			},
			{
				name: "képzetes_szám",
				description: "a komplex szám képzetes része"
			},
			{
				name: "képz_jel",
				description: "a képzetes egység jele"
			}
		]
	},
	{
		name: "KONVERTÁLÁS",
		description: "Mértékegységeket vált át.",
		arguments: [
			{
				name: "szám",
				description: "a miből mértékegységben megadott, átváltandó szám"
			},
			{
				name: "miből",
				description: "az a mértékegység, amelyben a szám meg van adva"
			},
			{
				name: "mibe",
				description: "az eredmény mértékegysége"
			}
		]
	},
	{
		name: "KORREL",
		description: "Két adathalmaz korrelációs együtthatóját számítja ki.",
		arguments: [
			{
				name: "tömb1",
				description: "az egyik értékhalmaz cellatartománya. Az értékek számok, nevek, tömbök vagy számokat tartalmazó hivatkozások lehetnek"
			},
			{
				name: "tömb2",
				description: "a másik értékhalmaz cellatartománya. Az értékek számok, nevek, tömbök vagy számokat tartalmazó hivatkozások lehetnek"
			}
		]
	},
	{
		name: "KOVAR",
		description: "A kovarianciát, azaz két adathalmaz minden egyes adatpontpárja esetén vett eltérések szorzatának átlagát számítja ki.",
		arguments: [
			{
				name: "tömb1",
				description: "egész számokat tartalmazó első cellatartomány; elemei csak számok, tömbök vagy számokat tartalmazó hivatkozások lehetnek"
			},
			{
				name: "tömb2",
				description: "egész számokat tartalmazó második cellatartomány; elemei csak számok, tömbök vagy számokat tartalmazó hivatkozások lehetnek"
			}
		]
	},
	{
		name: "KOVARIANCIA.M",
		description: "A minta kovarianciáját, azaz a két adathalmaz minden egyes adatpontpárja esetén vett eltérések szorzatának átlagát számítja ki.",
		arguments: [
			{
				name: "tömb1",
				description: "egész számokat tartalmazó első cellatartomány; elemei számok, tömbök vagy számokat tartalmazó hivatkozások kell, hogy legyenek"
			},
			{
				name: "tömb2",
				description: "egész számokat tartalmazó második cellatartomány; elemei számok, tömbök vagy számokat tartalmazó hivatkozások kell, hogy legyenek"
			}
		]
	},
	{
		name: "KOVARIANCIA.S",
		description: "A sokaság kovarianciáját, azaz a két adathalmaz minden egyes adatpontpárja esetén vett eltérések szorzatának átlagát számítja ki.",
		arguments: [
			{
				name: "tömb1",
				description: "egész számokat tartalmazó első cellatartomány; elemei számok, tömbök vagy számokat tartalmazó hivatkozások kell, hogy legyenek"
			},
			{
				name: "tömb2",
				description: "egész számokat tartalmazó második cellatartomány; elemei számok, tömbök vagy számokat tartalmazó hivatkozások kell, hogy legyenek"
			}
		]
	},
	{
		name: "KÖZÉP",
		description: "Eredményként megadott számú karaktert ad egy szövegből a megadott sorszámú karaktertől kezdődően.",
		arguments: [
			{
				name: "szöveg",
				description: "az a szövegdarab, amely a kívánt karaktereket tartalmazza"
			},
			{
				name: "honnantól",
				description: "a szöveg e karakterétől kezdve kell adott számút kiolvasni. A szöveg első karaktere az 1-es számú karakter"
			},
			{
				name: "hány_karakter",
				description: "a kívánt eredmény karaktereinek száma"
			}
		]
	},
	{
		name: "KRITBINOM",
		description: "Azt a legkisebb számot adja eredményül, amelyre a binomiális eloszlásfüggvény értéke nem kisebb egy adott határértéknél.",
		arguments: [
			{
				name: "kísérletek",
				description: "a Bernoulli-kísérletek száma"
			},
			{
				name: "siker_valószínűsége",
				description: "a siker valószínűsége az egyes kísérletek esetén; 0 és 1 közti szám a végpontokat is beleértve"
			},
			{
				name: "alfa",
				description: "a határérték; 0 és 1 közti szám a végpontokat is beleértve"
			}
		]
	},
	{
		name: "KÜSZÖBNÉL.NAGYOBB",
		description: "Azt vizsgálja, hogy egy szám nagyobb-e egy adott küszöbértéknél.",
		arguments: [
			{
				name: "szám",
				description: "a vizsgálandó érték"
			},
			{
				name: "küszöb",
				description: "a küszöbérték"
			}
		]
	},
	{
		name: "KVARTILIS",
		description: "Egy adathalmaz kvartilisét (negyedszintjét) számítja ki.",
		arguments: [
			{
				name: "tömb",
				description: "azon számértékek tömbje vagy cellatartománya, amelyek kvartilisét meg kell határozni"
			},
			{
				name: "kvart",
				description: "azt jelzi, hogy melyik kvartilist kell kiszámítani: minimumérték = 0; első kvartilis = 1; medián = 2; harmadik kvartilis = 3; maximumérték = 4"
			}
		]
	},
	{
		name: "KVARTILIS.KIZÁR",
		description: "Egy adathalmaz kvartilisét (negyedszintjét) számítja ki az értékek percentilise, azaz százalékosztálya alapján (0 és 1 között, a végpontok nélkül).",
		arguments: [
			{
				name: "tömb",
				description: "azon számértékek tömbje vagy cellatartománya, amelyek kvartilisét meg kell határozni"
			},
			{
				name: "kvart",
				description: "azt jelzi, hogy melyik kvartilist kell kiszámítani: minimumérték = 0; első kvartilis = 1; medián = 2; harmadik kvartilis = 3; maximumérték = 4"
			}
		]
	},
	{
		name: "KVARTILIS.TARTALMAZ",
		description: "Egy adathalmaz kvartilisét (negyedszintjét) számítja ki az értékek percentilise, azaz százalékosztálya alapján (0 és 1 között, a végpontokat is beleértve).",
		arguments: [
			{
				name: "tömb",
				description: "azon számértékek tömbje vagy cellatartománya, amelyek kvartilisét meg kell határozni"
			},
			{
				name: "kvart",
				description: "azt jelzi, hogy melyik kvartilist kell kiszámítani: minimumérték = 0; első kvartilis = 1; medián = 2; harmadik kvartilis = 3; maximumérték = 4"
			}
		]
	},
	{
		name: "KVÓCIENS",
		description: "Egy hányados egész részét adja eredményül.",
		arguments: [
			{
				name: "számláló",
				description: "az osztandó"
			},
			{
				name: "nevező",
				description: "az osztó"
			}
		]
	},
	{
		name: "LAP",
		description: "A hivatkozott lap lapszámát adja vissza.",
		arguments: [
			{
				name: "érték",
				description: "a lap neve vagy egy hivatkozás, amelynek a lapszámát keresi; ha nincs megadva, a függvény annak a lapnak a számát adja vissza, amelyen szerepel"
			}
		]
	},
	{
		name: "LAPOK",
		description: "A hivatkozásban szereplő lapok számát adja vissza.",
		arguments: [
			{
				name: "hivatkozás",
				description: "az a hivatkozás, amelyről meg szeretné tudni, hogy hány lapot tartalmaz; ha nincs megadva, a függvény annak a munkafüzetnek a lapszámát adja vissza, amelyben szerepel"
			}
		]
	},
	{
		name: "LCSA",
		description: "Egy tárgyi eszköz egy időszakra eső amortizációját adja meg, bruttó érték szerinti lineáris leírási kulcsot alkalmazva.",
		arguments: [
			{
				name: "költség",
				description: "az eszköz beszerzési ára"
			},
			{
				name: "maradványérték",
				description: "az eszköz maradványértéke a leírási idő eltelte után"
			},
			{
				name: "leírási_idő",
				description: "a leírási időszakok teljes száma (azaz az eszköz hasznos élettartama)"
			}
		]
	},
	{
		name: "LEJÁRATI.KAMAT",
		description: "Lejáratkor kamatozó értékpapír felszaporodott kamatát adja eredményül.",
		arguments: [
			{
				name: "kibocsátás",
				description: "az értékpapír kibocsátásának dátuma dátumértékként kifejezve"
			},
			{
				name: "kiegyenlítés",
				description: "az értékpapír lejáratának időpontja dátumértékként kifejezve"
			},
			{
				name: "ráta",
				description: "az értékpapír éves kamat- vagy osztalékszelvény-fizetési rátája"
			},
			{
				name: "névérték",
				description: "az értékpapír névértéke"
			},
			{
				name: "alap",
				description: "az évtöredék számításának alapja"
			}
		]
	},
	{
		name: "LESZÁM",
		description: "Egy értékpapír leszámítolási rátáját adja eredményül.",
		arguments: [
			{
				name: "kiegyenlítés",
				description: "az értékpapír kifizetésének időpontja dátumértékként kifejezve"
			},
			{
				name: "lejárat",
				description: "az értékpapír lejáratának időpontja dátumértékként kifejezve"
			},
			{
				name: "ár",
				description: "a 100 Ft névértékű értékpapír ára"
			},
			{
				name: "visszaváltás",
				description: "a 100 Ft névértékű értékpapír visszaváltási árfolyama"
			},
			{
				name: "alap",
				description: "az évtöredék számításának alapja"
			}
		]
	},
	{
		name: "LIN.ILL",
		description: "Visszatérési értéke a statisztikai adatok olyan lineáris trendje, amely ismert adatpontok egyeztetésével a legkisebb négyzetek módszerével az adatokra legjobban illeszkedő egyenes paramétereit tartalmazza.",
		arguments: [
			{
				name: "ismert_y",
				description: "az y = mx + b összefüggésből már ismert y-értékek"
			},
			{
				name: "ismert_x",
				description: "az y = mx + b összefüggésből már ismert x-értékek, nem kötelező megadni"
			},
			{
				name: "konstans",
				description: "logikai érték: ha IGAZ vagy elhagyjuk, a b állandó kiszámítása a szokásos módon történik; ha HAMIS, a b állandó kötelezően 0 lesz."
			},
			{
				name: "stat",
				description: "logikai érték: ha IGAZ, a regresszióra vonatkozó további statisztikai adatok is megjelennek; ha HAMIS, vagy elhagyjuk, csak az m együtthatók és a b konstans lesz az eredmény"
			}
		]
	},
	{
		name: "LKO",
		description: "A legnagyobb közös osztót számítja ki.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "1–255 érték"
			},
			{
				name: "szám2",
				description: "1–255 érték"
			}
		]
	},
	{
		name: "LKT",
		description: "A legkisebb közös többszöröst számítja ki.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "az az 1–255 érték, amelynek legkisebb közös többszörösét ki kell számítani"
			},
			{
				name: "szám2",
				description: "az az 1–255 érték, amelynek legkisebb közös többszörösét ki kell számítani"
			}
		]
	},
	{
		name: "LN",
		description: "Egy szám természetes logaritmusát számítja ki.",
		arguments: [
			{
				name: "szám",
				description: "az a pozitív valós szám, amelynek természetes logaritmusát ki kell számítani"
			}
		]
	},
	{
		name: "LOG",
		description: "Egy szám megadott alapú logaritmusát számítja ki.",
		arguments: [
			{
				name: "szám",
				description: "az a pozitív valós szám, amelynek logaritmusát ki kell számítani"
			},
			{
				name: "alap",
				description: "a logaritmus alapja; ha elhagyjuk, értéke 10 lesz."
			}
		]
	},
	{
		name: "LOG.ELOSZLÁS",
		description: "A lognormális eloszlásfüggvény értékét számítja ki x-re; ln(x) normális eloszlását a középérték és szórás paraméterei adják meg.",
		arguments: [
			{
				name: "x",
				description: "az az érték, amelynél a függvény értékét ki kell számítani, pozitív szám"
			},
			{
				name: "középérték",
				description: "az ln(x) középértéke"
			},
			{
				name: "szórás",
				description: "az ln(x) szórása, pozitív szám"
			}
		]
	},
	{
		name: "LOG.ILL",
		description: "Visszatérési értéke az ismert adatpontokhoz legjobban illeszkedő  exponenciális görbét leíró statisztikai adatok.",
		arguments: [
			{
				name: "ismert_y",
				description: "az y = b*m^x összefüggésből már ismert y-értékek"
			},
			{
				name: "ismert_x",
				description: "az y = b*m^x összefüggésből már ismert x-értékek, melyeket nem kötelező megadni"
			},
			{
				name: "konstans",
				description: "logikai érték: ha IGAZ vagy elhagyjuk, a b állandó kiszámítása a szokásos módon történik; ha HAMIS, a b állandó kötelezően 1 lesz"
			},
			{
				name: "stat",
				description: "egy logikai érték: ha IGAZ, a regresszióra vonatkozó további statisztikai adatok is megjelennek; ha HAMIS, vagy elhagyjuk, csak az m együtthatók és a b konstans lesz az eredmény"
			}
		]
	},
	{
		name: "LOG10",
		description: "Egy szám 10-es alapú logaritmusát számítja ki.",
		arguments: [
			{
				name: "szám",
				description: "az a pozitív valós szám, amelynek 10-es alapú logaritmusát ki kell számítani"
			}
		]
	},
	{
		name: "LOGIKAI",
		description: "Megvizsgálja, hogy az érték logikai érték-e (IGAZ vagy HAMIS), és IGAZ vagy HAMIS értéket ad vissza.",
		arguments: [
			{
				name: "érték",
				description: "a megvizsgálni kívánt érték. Az érték hivatkozhat cellára, képletre vagy olyan névre, amely cellára, képletre vagy értékre hivatkozik"
			}
		]
	},
	{
		name: "LOGNORM.ELOSZLÁS",
		description: "A lognormális eloszlásfüggvény értékét számítja ki x-re; ln(x) normális eloszlását a középérték és szórás paraméterek adják meg.",
		arguments: [
			{
				name: "x",
				description: "az az érték, amelynél a függvény értékét ki kell számítani, pozitív szám"
			},
			{
				name: "középérték",
				description: "az ln(x) középértéke"
			},
			{
				name: "szórás",
				description: "az ln(x) szórása, pozitív szám"
			},
			{
				name: "eloszlásfv",
				description: "a függvény fajtáját megadó logikai érték: ha IGAZ, az eloszlásfüggvény értékét számítja ki, ha HAMIS, a sűrűségfüggvényét"
			}
		]
	},
	{
		name: "LOGNORM.INVERZ",
		description: "A lognormális eloszlás inverzét számítja ki x-re; ln(x) normális eloszlását a középérték és szórás paraméterek adják meg.",
		arguments: [
			{
				name: "valószínűség",
				description: "a lognormális eloszláshoz tartozó valószínűség; 0 és 1 közti szám, a végpontokat is beleértve"
			},
			{
				name: "középérték",
				description: "az ln(x) középértéke"
			},
			{
				name: "szórás",
				description: "az ln(x) szórása, pozitív szám"
			}
		]
	},
	{
		name: "LRÉSZLETKAMAT",
		description: "Eredményül a befektetési időszak adott időszakára eső kamatösszeget adja vissza.",
		arguments: [
			{
				name: "ráta",
				description: "időszakra vonatkozó kamatláb. Például 6% éves kamatláb esetén negyedévre 6%/4"
			},
			{
				name: "időszak",
				description: "az az időszak, amelyre a kamatot ki kell számítani"
			},
			{
				name: "időszakok_száma",
				description: "a fizetési időszakok száma a befektetési időszakban"
			},
			{
				name: "mai_érték",
				description: "a jelenérték nagysága, az az összeg, amennyit az összes jövőben esedékes kifizetés ma ér"
			}
		]
	},
	{
		name: "MA",
		description: "Visszatérési értéke az aktuális dátum dátumként formázva.",
		arguments: [
		]
	},
	{
		name: "MARADÉK",
		description: "A számnak az osztóval való elosztása után kapott maradékát adja eredményül.",
		arguments: [
			{
				name: "szám",
				description: "az a szám, melynek az osztás elvégzése utáni maradékát ki kell számítani"
			},
			{
				name: "osztó",
				description: "az a szám, amellyel a szám argumentumot el kell osztani"
			}
		]
	},
	{
		name: "MAX",
		description: "Egy értékhalmazban szereplő legnagyobb számot adja meg. A logikai értékeket és szövegeket figyelmen kívül hagyja.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "azok a számok, üres cellák, logikai értékek vagy szövegformában lévő számok (számuk 1 és 255 közé eshet), amelyek legnagyobbikát keresi"
			},
			{
				name: "szám2",
				description: "azok a számok, üres cellák, logikai értékek vagy szövegformában lévő számok (számuk 1 és 255 közé eshet), amelyek legnagyobbikát keresi"
			}
		]
	},
	{
		name: "MAX2",
		description: "Egy értékhalmazban szereplő legnagyobb értéket adja eredményül. A logikai értékeket és a szövegeket is figyelembe veszi.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "érték1",
				description: "azok a számok, üres cellák, logikai értékek vagy szövegformában lévő számok (számuk 1 és 255 közé eshet), melyek közül a legnagyobbat keresi"
			},
			{
				name: "érték2",
				description: "azok a számok, üres cellák, logikai értékek vagy szövegformában lévő számok (számuk 1 és 255 közé eshet), melyek közül a legnagyobbat keresi"
			}
		]
	},
	{
		name: "MDETERM",
		description: "Egy tömb mátrix-determinánsát számítja ki.",
		arguments: [
			{
				name: "tömb",
				description: "egy csak számokat tartalmazó négyzetes (ugyanannyi sorból és oszlopból álló) tömb; cellatartomány vagy tömbkonstans lehet."
			}
		]
	},
	{
		name: "MÉ",
		description: "Egy befektetés jelenértékét számítja ki: azt a jelenbeli egyösszegű kifizetést, amely egyenértékű a jövőbeli kifizetések összegével.",
		arguments: [
			{
				name: "ráta",
				description: "az időszakonkénti kamatláb. Például 6%-os éves kamatláb negyedévenkénti fizetéssel 6%/4"
			},
			{
				name: "időszakok_száma",
				description: "a fizetési időszakok száma a befektetési időszakban"
			},
			{
				name: "részlet",
				description: "a fizetési időszakokban esedékes kifizetés; nagysága a befektetési időszak egészében változatlan"
			},
			{
				name: "jövőbeli_érték",
				description: "a jövőbeli érték vagy az utolsó részlet kifizetése után elérni kívánt összeg"
			},
			{
				name: "típus",
				description: "logikai érték: ha 1, a résztörlesztés az időszak elején történik; ha 0 vagy elhagyjuk, az időszak végén"
			}
		]
	},
	{
		name: "MEDIÁN",
		description: "Adott számhalmaz mediánját (a halmaz közepén lévő számot) számítja ki.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "azok a számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások (számuk 1 és 255 közé eshet), amelyek mediánját keresi"
			},
			{
				name: "szám2",
				description: "azok a számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások (számuk 1 és 255 közé eshet), amelyek mediánját keresi"
			}
		]
	},
	{
		name: "MEGBÍZHATÓSÁG",
		description: "Egy statisztikai sokaság várható értékének megbízhatósági intervallumát adja eredményül normál eloszlás használatával.",
		arguments: [
			{
				name: "alfa",
				description: "a megbízhatósági szint kiszámításához használt pontossági szint, 0-nál nagyobb és 1-nél kisebb szám"
			},
			{
				name: "szórás",
				description: "a sokaságnak az adattartományon vett szórása; feltételezzük, hogy ismert. A szórásnak nullánál nagyobbnak kell lennie"
			},
			{
				name: "méret",
				description: "a minta mérete"
			}
		]
	},
	{
		name: "MEGBÍZHATÓSÁG.NORM",
		description: "Egy statisztikai sokaság várható értékének megbízhatósági intervallumát adja eredményül a normális eloszlás használatával.",
		arguments: [
			{
				name: "alfa",
				description: "a megbízhatósági szint kiszámításához használt pontossági szint, 0-nál nagyobb és 1-nél kisebb szám"
			},
			{
				name: "szórás",
				description: "a sokaságnak az adattartományon vett szórása; feltételezzük, hogy ismert. A szórásnak nullánál nagyobbnak kell lennie"
			},
			{
				name: "méret",
				description: "a minta mérete"
			}
		]
	},
	{
		name: "MEGBÍZHATÓSÁG.T",
		description: "Egy statisztikai sokaság várható értékének megbízhatósági intervallumát adja eredményül a Student-féle t-próba használatával.",
		arguments: [
			{
				name: "alfa",
				description: "a megbízhatósági szint kiszámításához használt pontossági szint, 0-nál nagyobb és 1-nél kisebb szám"
			},
			{
				name: "szórás",
				description: "a sokaságnak az adattartományon vett szórása; feltételezzük, hogy ismert. A szórásnak nullánál nagyobbnak kell lennie"
			},
			{
				name: "méret",
				description: "a minta mérete"
			}
		]
	},
	{
		name: "MEGTÉRÜLÉS",
		description: "A befektetés belső megtérülési rátáját számítja ki ismétlődő pénzáramlások esetén; a befektetés költségét és az újrabefektetett összegek után járó kamatot is figyelembevéve.",
		arguments: [
			{
				name: "értékek",
				description: "egy számokat tartalmazó tömb vagy cellahivatkozás; az elemek negatív előjel esetén kifizetést, pozitív előjel esetén bevételt jelentenek szabályos időközönként"
			},
			{
				name: "hitelkamat",
				description: "a kifizetett összegekre fizetett kamat"
			},
			{
				name: "újra-befektetési_ráta",
				description: "az újra befektetett összegek után kapott kamat"
			}
		]
	},
	{
		name: "MEREDEKSÉG",
		description: "A megadott adatpontokon át húzható lineáris regressziós egyenes meredekségét számítja ki.",
		arguments: [
			{
				name: "ismert_y",
				description: "a függő értékeket tartalmazó tömb vagy cellatartomány; az elemek lehetnek számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások"
			},
			{
				name: "ismert_x",
				description: "a független értékek halmaza; az elemek lehetnek számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások"
			}
		]
	},
	{
		name: "MÉRTANI.KÖZÉP",
		description: "Pozitív számértékekből álló tömb vagy tartomány mértani középértékét számítja ki.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások (számuk 1 és 255 között lehet), amelyek mértani középértékét ki kell számítani"
			},
			{
				name: "szám2",
				description: "számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások (számuk 1 és 255 között lehet), amelyek mértani középértékét ki kell számítani"
			}
		]
	},
	{
		name: "METSZ",
		description: "Az ismert x és y értékekre legjobban illeszkedő regressziós egyenes segítségével az egyenes y-tengellyel való metszéspontját határozza meg.",
		arguments: [
			{
				name: "ismert_y",
				description: "a függő változók vagy megfigyelések halmaza; elemei lehetnek számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások"
			},
			{
				name: "ismert_x",
				description: "a független változók vagy megfigyelések halmaza; elemei lehetnek számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások"
			}
		]
	},
	{
		name: "MIN",
		description: "Egy értékhalmazban lévő legkisebb számot adja meg. A logikai értékeket és a szövegeket figyelmen kívül hagyja.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "azok a számok, üres cellák, logikai értékek vagy szövegformában lévő számok (számuk 1 és 255 közé eshet), amelyek legkisebbikét keresi"
			},
			{
				name: "szám2",
				description: "azok a számok, üres cellák, logikai értékek vagy szövegformában lévő számok (számuk 1 és 255 közé eshet), amelyek legkisebbikét keresi"
			}
		]
	},
	{
		name: "MIN2",
		description: "Egy értékhalmazban szereplő legkisebb értéket adja eredményül. A logikai értékeket és a szövegeket is figyelembe veszi.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "érték1",
				description: "azok a számok, üres cellák, logikai értékek vagy szövegformában lévő számok (számuk 1 és 255 közé eshet), melyek közül a legkisebbet keresi"
			},
			{
				name: "érték2",
				description: "azok a számok, üres cellák, logikai értékek vagy szövegformában lévő számok (számuk 1 és 255 közé eshet), melyek közül a legkisebbet keresi"
			}
		]
	},
	{
		name: "MMÁTRIX",
		description: "A megadott dimenziójú egységmátrixot adja vissza.",
		arguments: [
			{
				name: "dimenzió",
				description: "a visszaadandó egységmátrix dimenzióját megadó egész szám"
			}
		]
	},
	{
		name: "MÓDUSZ",
		description: "Egy tömbből vagy adattartományból kiválasztja a leggyakrabban előforduló vagy ismétlődő számot.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "azon számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások, amelyekre a függvényt ki kell számítani; számuk 1 és 255 között lehet"
			},
			{
				name: "szám2",
				description: "azon számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások, amelyekre a függvényt ki kell számítani; számuk 1 és 255 között lehet"
			}
		]
	},
	{
		name: "MÓDUSZ.EGY",
		description: "Egy tömbből vagy adattartományból kiválasztja a leggyakrabban előforduló vagy ismétlődő számot.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "azon számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások, amelyekre a függvényt ki kell számítani; számuk 1 és 255 között lehet"
			},
			{
				name: "szám2",
				description: "azon számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások, amelyekre a függvényt ki kell számítani; számuk 1 és 255 között lehet"
			}
		]
	},
	{
		name: "MÓDUSZ.TÖBB",
		description: "Egy tömbben vagy adattartományban leggyakrabban szereplő vagy ismétlődő értékek egy függőleges tömbjét adja vissza. Vízszintes tömbhöz használja a =TRANSZPONÁLÁS(MÓDUSZ.TÖBB(szám1,szám2,...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "azon számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások, amelyekre a függvényt ki kell számítani; számuk 1 és 255 között lehet"
			},
			{
				name: "szám2",
				description: "azon számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások, amelyekre a függvényt ki kell számítani; számuk 1 és 255 között lehet"
			}
		]
	},
	{
		name: "MOST",
		description: "Az aktuális dátumot és időpontot adja meg dátum és idő formátumban.",
		arguments: [
		]
	},
	{
		name: "MPERC",
		description: "A másodpercet adja meg 0 és 59 közötti számmal.",
		arguments: [
			{
				name: "időérték",
				description: "Spreadsheet dátum- és időértékben megadott szám, illetve időformátumú szöveg, például 16:48:23 vagy 4:48:47 du."
			}
		]
	},
	{
		name: "MR",
		description: "Kiszámít egy befektetés növekedésével egyenértékű kamatlábat.",
		arguments: [
			{
				name: "időszakok_száma",
				description: "a befektetési időszakok száma"
			},
			{
				name: "jelenérték",
				description: "a befektetés jelenértéke"
			},
			{
				name: "jövőérték",
				description: "a befektetés jövőbeli értéke"
			}
		]
	},
	{
		name: "MSZORZAT",
		description: "Két tömb mátrix-szorzatát adja meg. Az eredménytömbnek ugyanannyi sora lesz, mint tömb1-nek és ugyanannyi oszlopa, mint tömb2-nek.",
		arguments: [
			{
				name: "tömb1",
				description: "a szorzásban szereplő első számtömb, amelynek ugyanannyi oszlopa kell, hogy legyen, mint ahány sora tömb2-nek van"
			},
			{
				name: "tömb2",
				description: "a szorzásban szereplő első számtömb, amelynek ugyanannyi oszlopa kell, hogy legyen, mint ahány sora tömb2-nek van"
			}
		]
	},
	{
		name: "NAGY",
		description: "Egy adathalmaz k-adik legnagyobb elemét adja eredményül. Például az ötödik legnagyobb számot.",
		arguments: [
			{
				name: "tömb",
				description: "az a tömb vagy adattartomány, amelynek k-adik legnagyobb értékét keresi"
			},
			{
				name: "k",
				description: "azt adja meg, hogy (a legnagyobbtól kezdve visszafelé) hányadik legnagyobb elemet kell megkeresni"
			}
		]
	},
	{
		name: "NAGYBETŰS",
		description: "Szövegrészt nagybetűssé alakít át.",
		arguments: [
			{
				name: "szöveg",
				description: "a nagybetűssé átalakítandó szöveg; lehet hivatkozás vagy szövegrész"
			}
		]
	},
	{
		name: "NAP",
		description: "Kiszámítja a hónap napját (1 és 31 közötti szám).",
		arguments: [
			{
				name: "időérték",
				description: "Spreadsheet dátum- és időértékben megadott szám"
			}
		]
	},
	{
		name: "NAP360",
		description: "Két dátum közé eső napok számát adja meg a 360 napos naptár (tizenkét 30 napos hónap) alapján.",
		arguments: [
			{
				name: "kezdő_dátum",
				description: "a kezdő_dátum és a záró_dátum az a két dátum, amelynek napban mért távolságát meg szeretné tudni"
			},
			{
				name: "záró_dátum",
				description: "a kezdő_dátum és a záró_dátum az a két dátum, amelynek napban mért távolságát meg szeretné tudni"
			},
			{
				name: "módszer",
				description: "a számítási módszert meghatározó logikai érték: ha HAMIS vagy elhagyja, USA-beli módszer; ha IGAZ, európai módszer."
			}
		]
	},
	{
		name: "NAPOK",
		description: "A két dátum közötti napok számát adja eredményül.",
		arguments: [
			{
				name: "kezdő_dátum",
				description: "A kezdő_dátum és a záró_dátum az a két dátum, amelyek között tudni szeretné a napok számát"
			},
			{
				name: "záró_dátum",
				description: "A kezdő_dátum és a záró_dátum az a két dátum, amelyek között tudni szeretné a napok számát"
			}
		]
	},
	{
		name: "NEGBINOM.ELOSZL",
		description: "A negatív binomiális eloszlás értékét adja meg; annak a valószínűsége, hogy megadott számú kudarc lesz a sikerek megadott számú bekövetkezése előtt a siker adott valószínűsége esetén.",
		arguments: [
			{
				name: "kudarc_szám",
				description: "a kudarcok száma"
			},
			{
				name: "sikeresek",
				description: "a siker küszöbértéke"
			},
			{
				name: "valószínűség",
				description: "a siker valószínűsége; 0 és 1 közötti szám"
			}
		]
	},
	{
		name: "NEGBINOM.ELOSZLÁS",
		description: "A negatív binomiális eloszlás értékét adja meg; annak a valószínűségét, hogy adott számú kudarc lesz a sikerek adott számú bekövetkezése előtt a siker adott valószínűsége esetén.",
		arguments: [
			{
				name: "kudarc_szám",
				description: "a kudarcok száma"
			},
			{
				name: "sikeresek",
				description: "a siker küszöbértéke"
			},
			{
				name: "valószínűség",
				description: "a siker valószínűsége; 0 és 1 közötti szám"
			},
			{
				name: "eloszlásfv",
				description: "logikai érték: ha IGAZ, az eloszlásfüggvény értékét számítja ki, ha HAMIS, a tömegfüggvényét"
			}
		]
	},
	{
		name: "NÉGYZETÖSSZEG",
		description: "Argumentumai négyzetének összegét számítja ki. Az argumentumok számok, nevek, tömbök vagy számokat tartalmazó hivatkozások lehetnek.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "azon számok, tömbök, nevek vagy tömbhivatkozások, amelyekre a négyzetösszeget ki kell számítani; számuk 1 és 255 között lehet."
			},
			{
				name: "szám2",
				description: "azon számok, tömbök, nevek vagy tömbhivatkozások, amelyekre a négyzetösszeget ki kell számítani; számuk 1 és 255 között lehet."
			}
		]
	},
	{
		name: "NEM",
		description: "Az IGAZ vagy HAMIS értéket az ellenkezőjére váltja.",
		arguments: [
			{
				name: "logikai",
				description: "olyan érték vagy kifejezés, amelyik kiértékelésekor IGAZ vagy HAMIS eredményt ad"
			}
		]
	},
	{
		name: "NEM.SZÖVEG",
		description: "Megvizsgálja, hogy az érték tényleg nem szöveg (az üres cellák nem számítanak szövegnek), és IGAZ vagy HAMIS értéket ad eredményül.",
		arguments: [
			{
				name: "érték",
				description: "a megvizsgálandó érték: cella; képlet; vagy olyan név, amely cellára, képletre vagy értékre hivatkozik"
			}
		]
	},
	{
		name: "NÉVLEGES",
		description: "Az éves névleges kamatláb értékét adja eredményül.",
		arguments: [
			{
				name: "tényleges_kamatláb",
				description: "a tényleges kamatláb"
			},
			{
				name: "időszak_per_év",
				description: "a részidőszakok száma évenként"
			}
		]
	},
	{
		name: "NINCS",
		description: "Megvizsgálja, hogy az érték a #HIÁNYZIK, és IGAZ vagy HAMIS értéket ad vissza.",
		arguments: [
			{
				name: "érték",
				description: "a megvizsgálni kívánt érték. Az érték hivatkozhat cellára, képletre vagy olyan névre, amely cellára, képletre vagy értékre hivatkozik"
			}
		]
	},
	{
		name: "NMÉ",
		description: "Egy befektetés nettó jelenértékét számítja ki ismert diszkontráta és jövőbeli kifizetések (negatív értékek) ill. bevételek (pozitív értékek) mellett.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "ráta",
				description: "az egy időszakra érvényes diszkontráta"
			},
			{
				name: "érték1",
				description: "a jövőbeli bevételek és kifizetések (számuk 1 és 254 között lehet), melyek egyenlő időközönként, az időszakok végén történnek"
			},
			{
				name: "érték2",
				description: "a jövőbeli bevételek és kifizetések (számuk 1 és 254 között lehet), melyek egyenlő időközönként, az időszakok végén történnek"
			}
		]
	},
	{
		name: "NORM.ELOSZL",
		description: "A normális eloszlás értékét számítja ki a megadott középérték és szórás esetén.",
		arguments: [
			{
				name: "x",
				description: "az az érték, amelynél az eloszlást ki kell számítani"
			},
			{
				name: "középérték",
				description: "az eloszlás középértéke (várható értéke)"
			},
			{
				name: "szórás",
				description: "az eloszlás szórása, pozitív szám"
			},
			{
				name: "eloszlásfv",
				description: "a függvény fajtáját megadó logikai érték: ha IGAZ, akkor az eloszlásfüggvény értékét számítja ki; ha HAMIS, a sűrűségfüggvényét."
			}
		]
	},
	{
		name: "NORM.ELOSZLÁS",
		description: "A normál eloszlás eloszlásfüggvényének értékét számítja ki a megadott középérték és szórás esetén.",
		arguments: [
			{
				name: "x",
				description: "az az érték, amelynél az eloszlást ki kell számítani"
			},
			{
				name: "középérték",
				description: "az eloszlás középértéke (várható értéke)"
			},
			{
				name: "szórás",
				description: "az eloszlás szórása, pozitív szám"
			},
			{
				name: "eloszlásfv",
				description: "logikai érték: ha IGAZ, az eloszlásfüggvény értékét számítja ki, ha HAMIS, a sűrűségfüggvényét"
			}
		]
	},
	{
		name: "NORM.INVERZ",
		description: "A normális eloszlás eloszlásfüggvénye inverzének értékét számítja ki a megadott középérték és szórás esetén.",
		arguments: [
			{
				name: "valószínűség",
				description: "a normális eloszláshoz tartozó valószínűség; 0 és 1 közötti szám, a végpontokat is beleértve"
			},
			{
				name: "középérték",
				description: "az eloszlás középértéke (várható értéke)"
			},
			{
				name: "szórás",
				description: "az eloszlás szórása, pozitív szám"
			}
		]
	},
	{
		name: "NORM.S.ELOSZLÁS",
		description: "A standard normális eloszlás eloszlásfüggvényének értékét számítja ki (a standard normális eloszlás középértéke 0, szórása 1).",
		arguments: [
			{
				name: "z",
				description: "az érték, amelynél az eloszlást ki kell számítani"
			},
			{
				name: "eloszlásfv",
				description: "a függvény által visszaadott logikai érték: ha IGAZ, az eloszlásfüggvény értékét számítja ki, ha HAMIS, a sűrűségfüggvényét"
			}
		]
	},
	{
		name: "NORM.S.INVERZ",
		description: "A standard normális eloszlás eloszlásfüggvénye inverzének értékét számítja ki. A standard normális eloszlás középértéke 0, szórása 1.",
		arguments: [
			{
				name: "valószínűség",
				description: "a normális eloszláshoz tartozó valószínűség; 0 és 1 közötti szám, a végpontokat is beleértve"
			}
		]
	},
	{
		name: "NORMALIZÁLÁS",
		description: "Középértékkel és szórással megadott eloszlásból normalizált értéket ad eredményül.",
		arguments: [
			{
				name: "x",
				description: "a normalizálandó érték"
			},
			{
				name: "középérték",
				description: "az eloszlás számtani középértéke"
			},
			{
				name: "szórás",
				description: "az eloszlás szórása, pozitív szám"
			}
		]
	},
	{
		name: "NÖV",
		description: "Az eredmény az ismert adatpontoknak megfelelő, exponenciális trend szerint növekvő számok sorozata.",
		arguments: [
			{
				name: "ismert_y",
				description: "az y = b*m^x összefüggésből már ismert y-értékek, pozitív számokból álló tömb vagy tartomány"
			},
			{
				name: "ismert_x",
				description: "az y = b*m^x összefüggésből már ismert x-értékek, melyeket nem kötelező megadni, az ismert_y méretével azonos méretű tömb vagy tartomány"
			},
			{
				name: "új_x",
				description: "a megadott új x-értékeknek az a csoportja, amelyekre ki kell számítani a megfelelő y-értékeket"
			},
			{
				name: "konstans",
				description: "logikai érték: ha IGAZ, a b állandó kiszámítása a szokásos módon történik; ha HAMIS vagy elhagyjuk, a b állandó kötelezően 1 lesz"
			}
		]
	},
	{
		name: "OKT.BIN",
		description: "Oktális számot binárissá alakít át.",
		arguments: [
			{
				name: "szám",
				description: "az átalakítandó oktális szám"
			},
			{
				name: "jegyek",
				description: "a használandó karakterek száma"
			}
		]
	},
	{
		name: "OKT.DEC",
		description: "Oktális számot decimálissá alakít át.",
		arguments: [
			{
				name: "szám",
				description: "az átalakítandó oktális szám"
			}
		]
	},
	{
		name: "OKT.HEX",
		description: "Oktális számot hexadecimálissá alakít át.",
		arguments: [
			{
				name: "szám",
				description: "az átalakítandó oktális szám"
			},
			{
				name: "jegyek",
				description: "a használandó karakterek száma"
			}
		]
	},
	{
		name: "ÓRA",
		description: "Az órát adja meg 0 (0:00, azaz éjfél) és 23 (este 11:00) közötti számmal.",
		arguments: [
			{
				name: "időérték",
				description: "Spreadsheet dátum- és időértékben megadott szám, illetve időformátumú szöveg, például 16:48:00 vagy 4:48:00 du."
			}
		]
	},
	{
		name: "ÖSSZ.MUNKANAP",
		description: "Azt adja meg, hogy a két dátum között hány teljes munkanap van.",
		arguments: [
			{
				name: "kezdő_dátum",
				description: "a kezdő_dátum dátumértéke"
			},
			{
				name: "vég_dátum",
				description: "a vég_dátum dátumértéke"
			},
			{
				name: "ünnepek",
				description: "a nem munkanap napok (állami, egyházi stb. ünnepek) dátumértékét tartalmazó tömb"
			}
		]
	},
	{
		name: "ÖSSZ.MUNKANAP.INTL",
		description: "Azt adja meg, hogy a két dátum között és az egyéni hétvége-paraméterek mellett hány teljes munkanap van.",
		arguments: [
			{
				name: "kezdő_dátum",
				description: "a kezdő_dátum dátumértéke"
			},
			{
				name: "vég_dátum",
				description: "a vég_dátum dátumértéke"
			},
			{
				name: "hétvége",
				description: "egy szám, illetve a hétvégéket jelző szöveg"
			},
			{
				name: "ünnepek",
				description: "a nem munkanapok (állami, egyházi stb. ünnepek) dátumértékét tartalmazó, nem kötelező halmaz"
			}
		]
	},
	{
		name: "ÖSSZEFŰZ",
		description: "Több szövegdarabot egyetlen szöveggé fűz össze.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szöveg1",
				description: "az a legalább 1, legfeljebb 255 szövegdarab, amelyet egyetlen szöveggé kell összefűzni; lehetnek szövegdarabok, számok vagy egy cellára való hivatkozások"
			},
			{
				name: "szöveg2",
				description: "az a legalább 1, legfeljebb 255 szövegdarab, amelyet egyetlen szöveggé kell összefűzni; lehetnek szövegdarabok, számok vagy egy cellára való hivatkozások"
			}
		]
	},
	{
		name: "ÖSSZES.KAMAT",
		description: "Két fizetési időszak között kifizetett kamat halmozott értékét adja eredményül.",
		arguments: [
			{
				name: "ráta",
				description: "kamatláb"
			},
			{
				name: "időszakok_száma",
				description: "a fizetési időszakok teljes száma"
			},
			{
				name: "mai_érték",
				description: "a jelenlegi érték"
			},
			{
				name: "kezdő_periódus",
				description: "a számításban szereplő első időszak"
			},
			{
				name: "vég_periódus",
				description: "a számításban szereplő utolsó időszak"
			},
			{
				name: "típus",
				description: "mikor esedékes a fizetés"
			}
		]
	},
	{
		name: "ÖSSZES.TŐKERÉSZ",
		description: "Két fizetési időszak között kifizetett részletek halmozott (kamatot nem tartalmazó) értékét adja eredményül.",
		arguments: [
			{
				name: "ráta",
				description: "kamatláb"
			},
			{
				name: "időszakok_száma",
				description: "a fizetési időszakok teljes száma"
			},
			{
				name: "mai_érték",
				description: "a jelenlegi érték"
			},
			{
				name: "kezdő_periódus",
				description: "a számításban szereplő első időszak"
			},
			{
				name: "vég_periódus",
				description: "a számításban szereplő utolsó időszak"
			},
			{
				name: "típus",
				description: "mikor esedékes a fizetés"
			}
		]
	},
	{
		name: "OSZLOP",
		description: "Egy hivatkozás oszlopszámát adja eredményül.",
		arguments: [
			{
				name: "hivatkozás",
				description: "az a cella vagy egymással határos cellákból álló tartomány, amelynek oszlopszámát meg kívánja kapni. Elhagyása esetén az OSZLOP függvényt tartalmazó cella lesz felhasználva"
			}
		]
	},
	{
		name: "OSZLOPOK",
		description: "Tömbben vagy hivatkozásban található oszlopok számát adja eredményül.",
		arguments: [
			{
				name: "tömb",
				description: "az a tömb, tömbképlet vagy cellatartományra való hivatkozás, amelyben található oszlopok számát meg kívánja kapni"
			}
		]
	},
	{
		name: "PADLÓ",
		description: "Egy számot lefelé kerekít, a pontosságként megadott érték legközelebb eső többszörösére.",
		arguments: [
			{
				name: "szám",
				description: "a kerekítendő szám"
			},
			{
				name: "pontosság",
				description: "az a szám, amelynek legközelebbi többszörösére kerekíteni kell. A számnak és a pontosságnak azonos előjelűnek kell lennie."
			}
		]
	},
	{
		name: "PADLÓ.MAT",
		description: "Egy számot a legközelebbi egészre vagy a pontosságként megadott érték legközelebb eső többszörösére kerekít le.",
		arguments: [
			{
				name: "szám",
				description: "a kerekítendő szám"
			},
			{
				name: "pontosság",
				description: "az a szám, amelynek legközelebbi többszörösére kerekíteni kell"
			},
			{
				name: "mód",
				description: "nem nulla érték megadása esetén a függvény nulla felé kerekít"
			}
		]
	},
	{
		name: "PADLÓ.PONTOS",
		description: "Egy számot lefelé, a legközelebbi egész felé kerekít, a pontosságként megadott érték legközelebb eső többszörösére.",
		arguments: [
			{
				name: "szám",
				description: "a kerekítendő szám"
			},
			{
				name: "pontosság",
				description: "az a szám, amelynek legközelebbi többszörösére kerekíteni kell. "
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
		name: "PÁRATLAN",
		description: "Egy pozitív számot felfelé, egy negatív számot pedig lefelé kerekít a legközelebbi páratlan egész számra.",
		arguments: [
			{
				name: "szám",
				description: "a kerekítendő szám"
			}
		]
	},
	{
		name: "PÁRATLANE",
		description: "A függvény eredménye IGAZ, ha a szám páratlan.",
		arguments: [
			{
				name: "szám",
				description: "a vizsgált érték"
			}
		]
	},
	{
		name: "PÁROS",
		description: "Egy pozitív számot felfelé, egy negatív számot pedig lefelé kerekít a legközelebbi páros egész számra.",
		arguments: [
			{
				name: "szám",
				description: "a kerekítendő szám"
			}
		]
	},
	{
		name: "PÁROSE",
		description: "A függvény eredménye IGAZ, ha a szám páros.",
		arguments: [
			{
				name: "szám",
				description: "a vizsgált érték"
			}
		]
	},
	{
		name: "PEARSON",
		description: "A Pearson-féle korrelációs együtthatót (r) számítja ki .",
		arguments: [
			{
				name: "tömb1",
				description: "a független értékek halmaza"
			},
			{
				name: "tömb2",
				description: "a függő értékek halmaza"
			}
		]
	},
	{
		name: "PER.SZÁM",
		description: "A befektetési időszakok számát adja meg ismert, adott nagyságú konstans részletfizetések és állandó kamatláb mellett.",
		arguments: [
			{
				name: "ráta",
				description: "az időszakonkénti kamatláb. Például 6%-os éves kamatláb negyedévenkénti fizetéssel 6%/4"
			},
			{
				name: "részlet",
				description: "a fizetési időszakokban esedékes kifizetés; nagysága a befektetési időszak egészében változatlan"
			},
			{
				name: "mai_érték",
				description: "a jövőbeli kifizetések jelenértéke, vagyis az a jelenbeli egyösszegű kifizetés, amely egyenértékű a jövőbeli kifizetések összegével"
			},
			{
				name: "jövőbeli_érték",
				description: "a jövőbeli érték vagy az utolsó részlet kifizetése után elérni kívánt összeg. Ha elhagyjuk, zérus lesz"
			},
			{
				name: "típus",
				description: "logikai érték: ha 1, a résztörlesztés az időszak elején történik; ha 0 vagy elhagyjuk, az időszak végén"
			}
		]
	},
	{
		name: "PERCEK",
		description: "A percet adja meg 0 és 59 közötti számmal.",
		arguments: [
			{
				name: "időérték",
				description: "Spreadsheet dátum- és időértékben megadott szám, illetve időformátumú szöveg, például 16:48:00 vagy 4:48:00 du."
			}
		]
	},
	{
		name: "PERCENTILIS",
		description: "Egy tartományban található értékek k-adik percentilisét, azaz százalékosztályát adja eredményül.",
		arguments: [
			{
				name: "tömb",
				description: "az egymáshoz viszonyítandó adatokat tartalmazó tömb vagy tartomány"
			},
			{
				name: "k",
				description: "a százalékosztály száma 0 és 1 között, a végpontokat is beleértve"
			}
		]
	},
	{
		name: "PERCENTILIS.KIZÁR",
		description: "Egy tartományban található értékek k-adik percentilisét, azaz százalékosztályát adja eredményül, ahol k a 0 és 1 közötti tartományban található, a végpontok nélkül.",
		arguments: [
			{
				name: "tömb",
				description: "az egymáshoz viszonyítandó adatokat tartalmazó tömb vagy tartomány"
			},
			{
				name: "k",
				description: "a százalékosztály száma 0 és 1 között, a végpontokat is beleértve"
			}
		]
	},
	{
		name: "PERCENTILIS.TARTALMAZ",
		description: "Egy tartományban található értékek k-adik percentilisét, azaz százalékosztályát adja eredményül, ahol k a 0 és 1 közötti tartományban található, a végpontokat is beleértve.",
		arguments: [
			{
				name: "tömb",
				description: "az egymáshoz viszonyítandó adatokat tartalmazó tömb vagy tartomány"
			},
			{
				name: "k",
				description: "a százalékosztály száma 0 és 1 között, a végpontokat is beleértve"
			}
		]
	},
	{
		name: "PI",
		description: "A pi értékét adja vissza 15 jegy pontossággal (3,14159265358979).",
		arguments: [
		]
	},
	{
		name: "PLAFON",
		description: "Egy számot a pontosságként megadott érték legközelebb eső többszörösére kerekít fel.",
		arguments: [
			{
				name: "szám",
				description: "a kerekítendő szám"
			},
			{
				name: "pontosság",
				description: "az a szám, amelynek legközelebbi többszörösére kerekíteni kell"
			}
		]
	},
	{
		name: "PLAFON.MAT",
		description: "Egy számot a legközelebbi egészre vagy a pontosságként megadott érték legközelebb eső többszörösére kerekít fel.",
		arguments: [
			{
				name: "szám",
				description: "a kerekítendő szám"
			},
			{
				name: "pontosság",
				description: "az a szám, amelynek legközelebbi többszörösére kerekíteni kell"
			},
			{
				name: "mód",
				description: "nem nulla érték megadása esetén a függvény nullától elkerekít"
			}
		]
	},
	{
		name: "PLAFON.PONTOS",
		description: "Egy számot a legközelebbi egészre vagy a pontosságként megadott érték legközelebb eső többszörösére kerekít fel.",
		arguments: [
			{
				name: "szám",
				description: "a kerekítendő szám"
			},
			{
				name: "pontosság",
				description: "az a szám, amelynek legközelebbi többszörösére kerekíteni kell"
			}
		]
	},
	{
		name: "POISSON",
		description: "A Poisson-eloszlás értékét számítja ki.",
		arguments: [
			{
				name: "x",
				description: "az események száma"
			},
			{
				name: "középérték",
				description: "a várható érték, pozitív szám"
			},
			{
				name: "eloszlásfv",
				description: "a függvény fajtáját megadó logikai érték: ha IGAZ, akkor a Poisson-eloszlásfüggvény értékét számítja ki; ha HAMIS, a Poisson-tömegfüggvényét."
			}
		]
	},
	{
		name: "POISSON.ELOSZLÁS",
		description: "A Poisson-eloszlás értékét számítja ki.",
		arguments: [
			{
				name: "x",
				description: "az események száma"
			},
			{
				name: "középérték",
				description: "a várható érték, pozitív szám"
			},
			{
				name: "eloszlásfv",
				description: "a függvény fajtáját megadó logikai érték: ha IGAZ, akkor a POISSON függvény az eloszlásfüggvény értékét számítja ki; ha HAMIS, a sűrűségfüggvényét"
			}
		]
	},
	{
		name: "PRÉSZLET",
		description: "Egy befektetésen belül a tőketörlesztés nagyságát számítja ki egy adott időszakra, ismétlődő állandó részfizetések és állandó kamatláb mellett.",
		arguments: [
			{
				name: "ráta",
				description: "az időszakonkénti kamatláb. Például évi 6% kamat és negyedéves részlet esetén 6%/4"
			},
			{
				name: "időszak",
				description: "egy időszakot jelöl ki, értékének 1 és az időszakok_száma közé kell esnie"
			},
			{
				name: "időszakok_száma",
				description: "a fizetési időszakok száma a befektetési időszakban"
			},
			{
				name: "mai_érték",
				description: "a jövőbeli kifizetések jelenértéke, vagyis az a jelenbeli egyösszegű kifizetés, amely egyenértékű a jövőbeli kifizetések összegével"
			},
			{
				name: "jövőbeli_érték",
				description: "a jövőbeli érték vagy az utolsó részlet kifizetése után elérni kívánt összeg"
			},
			{
				name: "típus",
				description: "logikai érték: ha 1, a résztörlesztés az időszak elején történik; ha 0 vagy elhagyjuk, az időszak végén"
			}
		]
	},
	{
		name: "RADIÁN",
		description: "Fokot radiánná alakít át.",
		arguments: [
			{
				name: "szög",
				description: "az átalakítandó szög értéke fokokban"
			}
		]
	},
	{
		name: "RANG.ÁTL",
		description: "Kiszámítja, hogy egy szám nagysága alapján hányadik egy számsorozatban; ha több érték sorszáma azonos, a sorszámok átlagát adja vissza.",
		arguments: [
			{
				name: "szám",
				description: "az a szám, amelyről meg kell állapítani, hányadik"
			},
			{
				name: "hiv",
				description: "egy számsorozatot tartalmazó tömb vagy egy számsorozatra való hivatkozás. A nem számértékeket a függvény nem veszi számításba"
			},
			{
				name: "sorrend",
				description: "a számok sorba rendezését megadó számérték: ha 0 vagy elhagyjuk, csökkenő sorrend; bármely nem nulla érték esetén növekvő sorrend"
			}
		]
	},
	{
		name: "RANG.EGY",
		description: "Kiszámítja, hogy egy szám nagysága alapján hányadik egy számsorozatban; ha több érték sorszáma azonos, a halmazhoz tartozó legmagasabb sorszámot adja vissza.",
		arguments: [
			{
				name: "szám",
				description: "az a szám, amelyről meg kell állapítani, hányadik"
			},
			{
				name: "hiv",
				description: "egy számsorozatot tartalmazó tömb vagy egy számsorozatra való hivatkozás. A nem számértékeket a függvény nem veszi számításba"
			},
			{
				name: "sorrend",
				description: "a számok sorba rendezését megadó számérték: ha 0 vagy elhagyjuk, csökkenő sorrend; bármely nem nulla érték esetén növekvő sorrend"
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
		name: "RÁTA",
		description: "Egy kölcsön vagy befektetési időszak esetén az egy időszakra eső kamatláb nagyságát számítja ki. Például 6% éves kamatláb negyedéves törlesztése 6%/4.",
		arguments: [
			{
				name: "időszakok_száma",
				description: "a fizetési időszakok száma a kölcsön esetén vagy a befektetési időszakban"
			},
			{
				name: "részlet",
				description: "a fizetési időszakokban esedékes kifizetés; nagysága a kölcsön vagy a befektetési időszak folyamán változatlan"
			},
			{
				name: "jelen_érték",
				description: "a jövőbeli kifizetések jelenértéke, vagyis az a jelenbeli egyösszegű kifizetés, amely egyenértékű a jövőbeli kifizetések összegével"
			},
			{
				name: "jövőbeli_érték",
				description: "a jövőbeli érték vagy az utolsó részlet kifizetése után elérni kívánt összeg. Ha elhagyjuk, a jövőbeli_érték = 0"
			},
			{
				name: "típus",
				description: "logikai érték: ha 1, a résztörlesztés az időszak elején történik; ha 0 vagy elhagyjuk, az időszak végén"
			},
			{
				name: "becslés",
				description: "az Ön becslése a kamatláb nagyságára; elhagyása esetén a becslés 0,1 (10 százalék) lesz"
			}
		]
	},
	{
		name: "RÉSZÁTLAG",
		description: "Egy adatértékekből álló halmaz középső részének átlagát számítja ki.",
		arguments: [
			{
				name: "tömb",
				description: "az a tartomány vagy tömb, amelynek egy részét átlagolni kell"
			},
			{
				name: "százalék",
				description: "az adathalmaz elejéről és végéről elhagyandó adatok százalékos aránya"
			}
		]
	},
	{
		name: "RÉSZLET",
		description: "A kölcsönre vonatkozó törlesztési összeget számítja ki állandó nagyságú törlesztőrészletek és állandó kamatláb esetén.",
		arguments: [
			{
				name: "ráta",
				description: "az időszakonkénti kamatláb a kölcsön esetére. Például 6% éves kamatláb negyedéves törlesztése 6%/4"
			},
			{
				name: "időszakok_száma",
				description: "a kölcsön összes törlesztőrészletének száma"
			},
			{
				name: "mai_érték",
				description: "a jövőbeli kifizetések jelenértéke, vagyis az a jelenbeli egyösszegű kifizetés, amely egyenértékű a jövőbeli kifizetések összegével"
			},
			{
				name: "jövőbeli_érték",
				description: "a jövőbeli érték vagy az utolsó részlet kifizetése után elérni kívánt összeg, 0 (nulla), ha elhagyjuk"
			},
			{
				name: "típus",
				description: "logikai érték: ha 1, a résztörlesztés az időszak elején történik; ha 0 vagy elhagyjuk, az időszak végén"
			}
		]
	},
	{
		name: "RÉSZÖSSZEG",
		description: "Listában vagy adatbázisban részösszeget ad vissza.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "függv_szám",
				description: "egy szám 1-től 11-ig, amely a részösszeghez használt összegzési függvényt határozza meg."
			},
			{
				name: "hiv1",
				description: "azok a tartományok vagy hivatkozások (1-től 254-ig), amelyekre a részösszeget ki szeretné számítani"
			}
		]
	},
	{
		name: "RNÉGYZET",
		description: "Kiszámítja a Pearson-féle szorzatmomentum korrelációs együtthatójának négyzetét a megadott adatpontok esetén.",
		arguments: [
			{
				name: "ismert_y",
				description: "az adatokat tartalmazó tömb vagy tartomány; lehetnek számok, nevek, tömbök vagy számokat tartalmazó hivatkozások"
			},
			{
				name: "ismert_x",
				description: "az adatokat tartalmazó tömb vagy tartomány; lehetnek számok, nevek, tömbök vagy számokat tartalmazó hivatkozások"
			}
		]
	},
	{
		name: "RÓMAI",
		description: "Egy arab számot szövegként római számra alakít át.",
		arguments: [
			{
				name: "szám",
				description: "az átalakítandó arab szám"
			},
			{
				name: "alak",
				description: "a római szám kívánt típusát megadó szám."
			}
		]
	},
	{
		name: "RRÉSZLET",
		description: "Egy befektetésen belül a részletfizetés nagyságát számítja ki egy adott időszakra, adott nagyságú konstans részletek és állandó kamatláb mellett.",
		arguments: [
			{
				name: "ráta",
				description: "az időszakonkénti kamatláb. Például évi 6% kamat és negyedéves részlet esetén 6%/4"
			},
			{
				name: "időszak",
				description: "annak az időszaknak a száma, amelyre a részletfizetést ki kell számítani; 1 és az időszakok_száma közé kell esnie"
			},
			{
				name: "időszakok_száma",
				description: "a fizetési időszakok száma a befektetési időszakban"
			},
			{
				name: "mai_érték",
				description: "a jövőbeli kifizetések jelenértéke, vagyis az a jelenbeli egyösszegű kifizetés, amely egyenértékű a jövőbeli kifizetések összegével"
			},
			{
				name: "jövőbeli_érték",
				description: "a jövőbeli érték vagy az utolsó részlet kifizetése után elérni kívánt összeg. Ha elhagyjuk, a jövőbeli_érték = 0"
			},
			{
				name: "típus",
				description: "a részfizetések esedékességét megadó logikai érték: 0 vagy elhagyható, ha a törlesztés az időszak végén történik; 1, ha az időszak elején"
			}
		]
	},
	{
		name: "S",
		description: "A nem szám értéket számmá, a dátumot dátumértékké alakítja, az IGAZ értékből 1, bármi egyébből 0 (zérus) lesz.",
		arguments: [
			{
				name: "érték",
				description: "a számmá átalakítani kívánt érték"
			}
		]
	},
	{
		name: "SEC",
		description: "Egy szög szekánsát számítja ki.",
		arguments: [
			{
				name: "szám",
				description: "az a radiánban megadott szög, amelynek a szekánsát keresi"
			}
		]
	},
	{
		name: "SECH",
		description: "Egy szög hiperbolikus szekánsát számítja ki.",
		arguments: [
			{
				name: "szám",
				description: "az a radiánban megadott szög, amelynek a hiperbolikus szekánsát keresi"
			}
		]
	},
	{
		name: "SIN",
		description: "Egy szög szinuszát számítja ki.",
		arguments: [
			{
				name: "szám",
				description: "az a radiánban megadott szög, amelynek szinuszát ki kell számítani. Átszámítás: fok * PI()/180 = radián"
			}
		]
	},
	{
		name: "SINH",
		description: "Egy szám szinusz hiperbolikusát számítja ki.",
		arguments: [
			{
				name: "szám",
				description: "tetszőleges valós szám"
			}
		]
	},
	{
		name: "SOKSZOR",
		description: "Megadott alkalommal megismétel egy szövegdarabot. A SOKSZOR függvény segítségével egy szövegdarab számos példányával tölthet fel egy cellát.",
		arguments: [
			{
				name: "szöveg",
				description: "a megismétlendő szövegdarab"
			},
			{
				name: "hányszor",
				description: "az ismétlések számát megadó pozitív szám"
			}
		]
	},
	{
		name: "SOR",
		description: "Egy hivatkozás sorának számát adja meg.",
		arguments: [
			{
				name: "hivatkozás",
				description: "az a cella vagy cellatartomány, amely sorának számát meg kell állapítani; ha elhagyjuk, a SOR függvényt tartalmazó cellát adja eredményül"
			}
		]
	},
	{
		name: "SOROK",
		description: "Egy hivatkozás vagy tömb sorainak számát adja meg.",
		arguments: [
			{
				name: "tömb",
				description: "ebben a tömbben, tömbképletben vagy cellatartományra való hivatkozásban található sorok számát kell meghatározni"
			}
		]
	},
	{
		name: "SORÖSSZEG",
		description: "Hatványsor összegét adja eredményül.",
		arguments: [
			{
				name: "x",
				description: "az a hely, ahol a hatványsor összegét ki kell számítani"
			},
			{
				name: "n",
				description: "a kezdő hatványkitevő, amelyre x-et emelni kell"
			},
			{
				name: "m",
				description: "lépésköz, amellyel n értéke tagonként növekszik"
			},
			{
				name: "koefficiensek",
				description: "az x egyes hatványainak együtthatói"
			}
		]
	},
	{
		name: "SORSZÁM",
		description: "Kiszámítja, hogy egy szám nagysága alapján hányadik egy számsorozatban.",
		arguments: [
			{
				name: "szám",
				description: "az a szám, amelyről meg kell állapítani, hányadik"
			},
			{
				name: "hiv",
				description: "egy számsorozatot tartalmazó tömb vagy egy számsorozatra való hivatkozás. A nem szám értékeket a függvény nem veszi számításba."
			},
			{
				name: "sorrend",
				description: "a számok sorba rendezését megadó számérték: ha 0 vagy elhagyjuk, csökkenő sorrend; bármely nem nulla érték esetén növekvő sorrend"
			}
		]
	},
	{
		name: "SQ",
		description: "Az egyes adatpontok középértéktől való eltérésnégyzeteinek összegét adja eredményül.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: " argumentumok (legalább 1 és legfeljebb 255 darab), ill. egy tömb vagy tömbhivatkozás, amelyre a számítást el kell végezni."
			},
			{
				name: "szám2",
				description: " argumentumok (legalább 1 és legfeljebb 255 darab), ill. egy tömb vagy tömbhivatkozás, amelyre a számítást el kell végezni."
			}
		]
	},
	{
		name: "STHIBAYX",
		description: "A regresszióban az egyes x-értékek alapján meghatározott y-értékek standard hibáját számítja ki.",
		arguments: [
			{
				name: "ismert_y",
				description: "a függő értékeket tartalmazó tömb vagy tartomány; az elemek lehetnek számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások"
			},
			{
				name: "ismert_x",
				description: "a független értékeket tartalmazó tömb vagy tartomány; az elemek lehetnek számok vagy számokat tartalmazó nevek, tömbök vagy hivatkozások"
			}
		]
	},
	{
		name: "STNORMELOSZL",
		description: "A standard normális eloszlás eloszlásfüggvényének értékét számítja ki. A standard normális eloszlás középértéke 0, szórása 1.",
		arguments: [
			{
				name: "z",
				description: "az az érték, amelynél az eloszlást ki kell számítani"
			}
		]
	},
	{
		name: "SZÁM",
		description: "Megvizsgálja, hogy az érték szám-e, és IGAZ vagy HAMIS értéket ad vissza.",
		arguments: [
			{
				name: "érték",
				description: "a megvizsgálni kívánt érték. Az érték hivatkozhat cellára, képletre vagy olyan névre, amely cellára, képletre vagy értékre hivatkozik"
			}
		]
	},
	{
		name: "SZÁMÉRTÉK",
		description: "Szöveget konvertál számmá területi beállítástól független módon.",
		arguments: [
			{
				name: "szöveg",
				description: "a konvertálni kívánt számot képviselő karakterlánc"
			},
			{
				name: "tizedes_elv",
				description: "a karakterláncban tizedes elválasztóként használt karakter"
			},
			{
				name: "ezres_elv",
				description: "a karakterláncban ezreselválasztóként használt karakter"
			}
		]
	},
	{
		name: "SZÁZALÉKRANG",
		description: "Egy értéknek egy adathalmazon belül vett százalékos rangját (elhelyezkedését) adja meg.",
		arguments: [
			{
				name: "tömb",
				description: "az egymáshoz viszonyítandó számadatokat tartalmazó tömb vagy tartomány"
			},
			{
				name: "x",
				description: "az az érték, amelynek rangját meg kell határozni"
			},
			{
				name: "pontosság",
				description: "az eredményül kapott százalékérték értékes jegyeinek számát határozza meg; nem kötelező megadni; he elhagyjuk, 3 értékes jegy lesz (0,xxx%)"
			}
		]
	},
	{
		name: "SZÁZALÉKRANG.KIZÁR",
		description: "Egy értéknek egy adathalmazon belül vett százalékos rangját (elhelyezkedését) adja meg (0 és 1 között, a végpontok nélkül).",
		arguments: [
			{
				name: "tömb",
				description: "az egymáshoz viszonyítandó számadatokat tartalmazó tömb vagy tartomány"
			},
			{
				name: "x",
				description: "az az érték, amelynek rangját meg kell határozni"
			},
			{
				name: "pontosság",
				description: "az eredményül kapott százalékérték értékes jegyeinek számát határozza meg; nem kötelező megadni; he elhagyjuk, 3 értékes jegy lesz (0,xxx%)"
			}
		]
	},
	{
		name: "SZÁZALÉKRANG.TARTALMAZ",
		description: "Egy értéknek egy adathalmazon belül vett százalékos rangját (elhelyezkedését) adja meg (0 és 1 között, a végpontokat is beleértve).",
		arguments: [
			{
				name: "tömb",
				description: "az egymáshoz viszonyítandó számadatokat tartalmazó tömb vagy tartomány"
			},
			{
				name: "x",
				description: "az az érték, amelynek rangját meg kell határozni"
			},
			{
				name: "pontosság",
				description: "az eredményül kapott százalékérték értékes jegyeinek számát határozza meg; nem kötelező megadni; he elhagyjuk, 3 értékes jegy lesz (0,xxx%)"
			}
		]
	},
	{
		name: "SZELVÉNYIDŐ.KEZDETTŐL",
		description: "Egy kamat- vagy osztalékszelvény-periódus kezdetétől a kifizetés időpontjáig összeszámolja a napokat.",
		arguments: [
			{
				name: "kiegyenlítés",
				description: "az értékpapír kifizetésének időpontja dátumértékként kifejezve"
			},
			{
				name: "lejárat",
				description: "az értékpapír lejáratának időpontja dátumértékként kifejezve"
			},
			{
				name: "gyakoriság",
				description: "a kamat- vagy osztalékszelvényekre történő kifizetések száma egy év alatt"
			},
			{
				name: "alap",
				description: "az évtöredék számításának alapja"
			}
		]
	},
	{
		name: "SZELVÉNYSZÁM",
		description: "A kifizetés és a lejárat időpontja között kifizetendő szelvények számát adja eredményül.",
		arguments: [
			{
				name: "kiegyenlítés",
				description: "az értékpapír kifizetésének időpontja dátumértékként kifejezve"
			},
			{
				name: "lejárat",
				description: "az értékpapír lejáratának időpontja dátumértékként kifejezve"
			},
			{
				name: "gyakoriság",
				description: "a kamat- vagy osztalékszelvényekre történő kifizetések száma egy év alatt"
			},
			{
				name: "alap",
				description: "az évtöredék számításának alapja"
			}
		]
	},
	{
		name: "SZÓR.M",
		description: "Minta alapján becslést ad a szórásra (a mintában lévő logikai értékeket és szöveget nem veszi figyelembe).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "a statisztikai mintát reprezentáló argumentumok, számuk 1 és 255 között lehet; lehetnek számok vagy számokat tartalmazó hivatkozások"
			},
			{
				name: "szám2",
				description: "a statisztikai mintát reprezentáló argumentumok, számuk 1 és 255 között lehet; lehetnek számok vagy számokat tartalmazó hivatkozások"
			}
		]
	},
	{
		name: "SZÓR.S",
		description: "Az argumentumokkal megadott statisztikai sokaság egészéből kiszámítja annak szórását (a logikai értékeket és a szövegeket figyelmen kívül hagyja).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "a statisztikai sokaságot reprezentáló argumentumok, számuk 1 és 255 között lehet; lehetnek számok vagy számokat tartalmazó hivatkozások"
			},
			{
				name: "szám2",
				description: "a statisztikai sokaságot reprezentáló argumentumok, számuk 1 és 255 között lehet; lehetnek számok vagy számokat tartalmazó hivatkozások"
			}
		]
	},
	{
		name: "SZÓRÁS",
		description: "Minta alapján becslést ad a szórásra (a mintában lévő logikai értékeket és szöveget nem veszi figyelembe).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "a statisztikai mintát reprezentáló argumentumok, számuk 1 és 255 között lehet; lehetnek számok vagy számokat tartalmazó hivatkozások"
			},
			{
				name: "szám2",
				description: "a statisztikai mintát reprezentáló argumentumok, számuk 1 és 255 között lehet; lehetnek számok vagy számokat tartalmazó hivatkozások"
			}
		]
	},
	{
		name: "SZÓRÁSA",
		description: "Minta alapján becslést ad a sokaság szórására, a logikai értékek és a szövegek figyelembevételével. A szöveg és a HAMIS logikai érték 0-nak, az IGAZ logikai érték 1-nek számít.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "érték1",
				description: "a sokaságból vett minta értékei, számuk 1 és 255 között lehet; értékek, nevek vagy értékhivatkozások lehetnek"
			},
			{
				name: "érték2",
				description: "a sokaságból vett minta értékei, számuk 1 és 255 között lehet; értékek, nevek vagy értékhivatkozások lehetnek"
			}
		]
	},
	{
		name: "SZÓRÁSP",
		description: "Az argumentumokkal megadott statisztikai sokaság egészéből kiszámítja annak szórását (a logikai értékeket és a szövegeket figyelmen kívül hagyja).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "a statisztikai sokaságot reprezentáló argumentumok, számuk 1 és 255 között lehet; lehetnek számok vagy számokat tartalmazó hivatkozások"
			},
			{
				name: "szám2",
				description: "a statisztikai sokaságot reprezentáló argumentumok, számuk 1 és 255 között lehet; lehetnek számok vagy számokat tartalmazó hivatkozások"
			}
		]
	},
	{
		name: "SZÓRÁSPA",
		description: "A statisztikai sokaság egészéből kiszámítja a szórást, a logikai értékek és a szövegek figyelembevételével. A szöveg és a HAMIS logikai érték 0-nak, az IGAZ logikai érték 1-nek számít.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "érték1",
				description: "a sokasághoz tartozó értékek, számuk 1 és 255 között lehet; értékek, nevek, tömbök vagy értéket tartalmazó hivatkozások lehetnek"
			},
			{
				name: "érték2",
				description: "a sokasághoz tartozó értékek, számuk 1 és 255 között lehet; értékek, nevek, tömbök vagy értéket tartalmazó hivatkozások lehetnek"
			}
		]
	},
	{
		name: "SZORHÁNYFAKT",
		description: "Egy számkészlet polinomját számítja ki.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "az az 1–255 érték, amely polinomját ki kell számítani"
			},
			{
				name: "szám2",
				description: "az az 1–255 érték, amely polinomját ki kell számítani"
			}
		]
	},
	{
		name: "SZORZAT",
		description: "Az összes argumentumként megadott szám szorzatát számítja ki.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "az összeszorzandó számok, logikai értékek vagy szöveges formában megadott számok; számuk 1 és 255 között lehet"
			},
			{
				name: "szám2",
				description: "az összeszorzandó számok, logikai értékek vagy szöveges formában megadott számok; számuk 1 és 255 között lehet"
			}
		]
	},
	{
		name: "SZORZATÖSSZEG",
		description: "Eredményül a megadott tartományok vagy tömbök számelemei szorzatának az összegét adja.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tömb1",
				description: "azok a tömbök (számuk 2 és 255 között lehet), amelyek megfelelő elemeit össze kell szorozni, majd a szorzatokat össze kell adni. Minden tömbnek azonos méretűnek kell lennie"
			},
			{
				name: "tömb2",
				description: "azok a tömbök (számuk 2 és 255 között lehet), amelyek megfelelő elemeit össze kell szorozni, majd a szorzatokat össze kell adni. Minden tömbnek azonos méretűnek kell lennie"
			},
			{
				name: "tömb3",
				description: "azok a tömbök (számuk 2 és 255 között lehet), amelyek megfelelő elemeit össze kell szorozni, majd a szorzatokat össze kell adni. Minden tömbnek azonos méretűnek kell lennie"
			}
		]
	},
	{
		name: "SZÖVEG",
		description: "Egy számértéket alakít át adott számformátumú szöveggé.",
		arguments: [
			{
				name: "érték",
				description: "szám vagy számértéket adó képlet, vagy pedig egy számértéket tartalmazó cellára való hivatkozás"
			},
			{
				name: "formátum_szöveg",
				description: "a Cellák formázása párbeszédpanel Szám lapján lévő Kategória mező egyik számformája szöveges formában (nem az Általános)"
			}
		]
	},
	{
		name: "SZÖVEG.E",
		description: "Megvizsgálja, hogy az érték szöveg-e, és IGAZ vagy HAMIS értéket ad vissza.",
		arguments: [
			{
				name: "érték",
				description: "a megvizsgálni kívánt érték. Az érték hivatkozhat cellára, képletre vagy olyan névre, amely cellára, képletre vagy értékre hivatkozik"
			}
		]
	},
	{
		name: "SZÖVEG.KERES",
		description: "Azt a karaktersorszámot adja meg, ahol egy adott karakter vagy szövegdarab először fordul elő balról jobbra haladva, a kis- és nagybetűket azonosnak tekintve.",
		arguments: [
			{
				name: "keres_szöveg",
				description: "a megkeresendő szövegdarab. Használhatja a ? vagy * joker karaktereket; a ? és a * kereséséhez használja a ~? és a ~* kombinációkat"
			},
			{
				name: "szöveg",
				description: "az a szöveg, amelyben a keres_szöveget meg kell keresni"
			},
			{
				name: "kezdet",
				description: "a szövegnek az a balról számított karakterhelye, amelytől a keresést el kell kezdeni. Ha elhagyjuk, 1 lesz"
			}
		]
	},
	{
		name: "SZÖVEG.TALÁL",
		description: "Megkeres egy szövegrészt egy másikban, eredményül a talált szövegrész kezdőpozíciójának számát adja, a kis- és nagybetűket megkülönbözteti.",
		arguments: [
			{
				name: "keres_szöveg",
				description: "a megkeresendő szövegrész. Két idézőjel (üres szöveg) segítségével a szöveg első karaktere található meg; helyettesítő karakterek nem használhatók"
			},
			{
				name: "szöveg",
				description: "az a szövegrész, amelyben keresni kell"
			},
			{
				name: "kezdet",
				description: "azt a karaktert adja meg, amelytől a keresést el kell kezdeni. A szöveg első karaktere az 1-es számú karakter. Ha elhagyjuk, a kezdet = 1"
			}
		]
	},
	{
		name: "SZUM",
		description: "Egy cellatartományban lévő összes számot összeadja.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "az összeadandó argumentumok, számuk 1 és 255 között lehet. A cellákban lévő logikai értékeket és szövegeket nem veszi figyelembe, az argumentumként beírtakat igen"
			},
			{
				name: "szám2",
				description: "az összeadandó argumentumok, számuk 1 és 255 között lehet. A cellákban lévő logikai értékeket és szövegeket nem veszi figyelembe, az argumentumként beírtakat igen"
			}
		]
	},
	{
		name: "SZUMHA",
		description: "A megadott feltételnek vagy kritériumnak eleget tevő cellákban található értékeket adja össze.",
		arguments: [
			{
				name: "tartomány",
				description: "a kiértékelendő cellatartomány"
			},
			{
				name: "kritérium",
				description: "az összeadandó cellákat meghatározó számként, kifejezésként vagy szövegként megadott feltétel vagy kritérium"
			},
			{
				name: "összeg_tartomány",
				description: "a ténylegesen összeadandó cellák. Ha elhagyjuk, a tartomány összes cellája fel lesz használva"
			}
		]
	},
	{
		name: "SZUMHATÖBB",
		description: "A megadott feltétel- vagy kritériumkészletnek eleget tevő cellákban található értékeket adja össze.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "összegtartomány",
				description: "az összegzendő cellák."
			},
			{
				name: "kritériumtartomány",
				description: "az adott feltétellel kiértékelni kívánt cellák tartománya"
			},
			{
				name: "kritérium",
				description: "a feltétel vagy kritérium egy az összeadandó cellákat definiáló szám, kifejezés vagy szöveg formájában"
			}
		]
	},
	{
		name: "SZUMX2BŐLY2",
		description: "Két tartomány vagy tömb megfelelő elemei négyzeteinek a különbségét összegezi.",
		arguments: [
			{
				name: "tömb_x",
				description: "a feldolgozandó számok első tartománya vagy tömbje; lehet szám vagy számokat tartalmazó név, tömb vagy hivatkozás"
			},
			{
				name: "tömb_y",
				description: "a feldolgozandó értékek második tartománya vagy tömbje; lehet szám vagy számokat tartalmazó név, tömb vagy hivatkozás"
			}
		]
	},
	{
		name: "SZUMX2MEGY2",
		description: "Két tartomány vagy tömb megfelelő elemei összegének a négyzetösszegét összegezi.",
		arguments: [
			{
				name: "tömb_x",
				description: "a feldolgozandó értékek első tartománya vagy tömbje; lehet szám vagy számokat tartalmazó név, tömb vagy hivatkozás"
			},
			{
				name: "tömb_y",
				description: "a feldolgozandó értékek második tartománya vagy tömbje; lehet szám vagy számokat tartalmazó név, tömb vagy hivatkozás"
			}
		]
	},
	{
		name: "SZUMXBŐLY2",
		description: "Két tartomány vagy tömb megfelelő elemei különbségének négyzetösszegét számítja ki.",
		arguments: [
			{
				name: "tömb_x",
				description: "a feldolgozandó értékek első tartománya vagy tömbje; lehet szám vagy számokat tartalmazó név, tömb vagy hivatkozás"
			},
			{
				name: "tömb_y",
				description: "a feldolgozandó értékek második tartománya vagy tömbje; lehet szám vagy számokat tartalmazó név, tömb vagy hivatkozás"
			}
		]
	},
	{
		name: "T",
		description: "Megvizsgálja, hogy az érték szöveg-e, és ha igen, az eredmény a szöveg; ha nem szöveg, az eredmény kettős idézőjel (üres szöveg).",
		arguments: [
			{
				name: "érték",
				description: "a vizsgálandó érték"
			}
		]
	},
	{
		name: "T.ELOSZL",
		description: "A Student-féle balszélű t-eloszlás értékét számítja ki.",
		arguments: [
			{
				name: "x",
				description: "az a szám, amelynél a függvény értékét ki kell számítani"
			},
			{
				name: "szabadságfok",
				description: "az eloszlást jellemző szabadságfokok száma"
			},
			{
				name: "eloszlásfv",
				description: "logikai érték; ha IGAZ, akkor az eloszlásfüggvény értékét számítja ki; ha HAMIS, a sűrűségfüggvényét"
			}
		]
	},
	{
		name: "T.ELOSZLÁS",
		description: "A Student-féle t-eloszlás értékét számítja ki.",
		arguments: [
			{
				name: "x",
				description: "az a szám, amelynél a függvény értékét ki kell számítani"
			},
			{
				name: "szabadságfok",
				description: "az eloszlást jellemző szabadságfokok száma"
			},
			{
				name: "szél",
				description: "az eloszlásszélek száma: egyszélű eloszlás esetén 1, kétszélű esetén 2"
			}
		]
	},
	{
		name: "T.ELOSZLÁS.2SZ",
		description: "A Student-féle t-eloszlás kétszélű értékét számítja ki.",
		arguments: [
			{
				name: "x",
				description: "az a szám, amelynél a függvény értékét ki kell számítani"
			},
			{
				name: "szabadságfok",
				description: "az eloszlást jellemző szabadságfokok száma"
			}
		]
	},
	{
		name: "T.ELOSZLÁS.JOBB",
		description: "A Student-féle t-eloszlás jobbszélű értékét számítja ki.",
		arguments: [
			{
				name: "x",
				description: "az a szám, amelynél a függvény értékét ki kell számítani"
			},
			{
				name: "szabadságfok",
				description: "az eloszlást jellemző szabadságfokok száma"
			}
		]
	},
	{
		name: "T.INVERZ",
		description: "A Student-féle t-eloszlás balszélű inverzét számítja ki.",
		arguments: [
			{
				name: "valószínűség",
				description: "a Student-féle t-eloszláshoz tartozó valószínűség; 0 és 1 közötti szám, a végpontokat is beleértve"
			},
			{
				name: "szabadságfok",
				description: "az eloszlás szabadságfokának számát jelző pozitív szám"
			}
		]
	},
	{
		name: "T.INVERZ.2SZ",
		description: "A Student-féle t-eloszlás kétszélű inverzét számítja ki.",
		arguments: [
			{
				name: "valószínűség",
				description: "a Student-féle t-eloszláshoz tartozó valószínűség; 0 és 1 közötti szám, a végpontokat is beleértve"
			},
			{
				name: "szabadságfok",
				description: "az eloszlás szabadságfokának számát jelző pozitív szám"
			}
		]
	},
	{
		name: "T.PRÓB",
		description: "A Student-féle t-próbához tartozó valószínűséget számítja ki.",
		arguments: [
			{
				name: "tömb1",
				description: "az első adathalmaz"
			},
			{
				name: "tömb2",
				description: "a második adathalmaz"
			},
			{
				name: "szél",
				description: "az eloszlásszélek száma: egyszélű eloszlás esetén 1, kétszélű esetén 2"
			},
			{
				name: "típus",
				description: "a végrehajtandó t-próba fajtája: párosított = 1, kétmintás, egyenlő varianciájú (homoscedasztikus) = 2, kétmintás, nem egyenlő varianciájú = 3"
			}
		]
	},
	{
		name: "T.PRÓBA",
		description: "A Student-féle t-próbához tartozó valószínűséget számítja ki.",
		arguments: [
			{
				name: "tömb1",
				description: "az első adathalmaz"
			},
			{
				name: "tömb2",
				description: "a második adathalmaz"
			},
			{
				name: "szél",
				description: "az eloszlásszélek száma: egyszélű eloszlás esetén 1, kétszélű esetén 2"
			},
			{
				name: "típus",
				description: "a végrehajtandó t-próba fajtája: párosított = 1, kétmintás, egyenlő varianciájú (homoscedasztikus) = 2, kétmintás, nem egyenlő varianciájú = 3"
			}
		]
	},
	{
		name: "TAN",
		description: "Egy szög tangensét számítja ki.",
		arguments: [
			{
				name: "szám",
				description: "az a radiánban megadott szög, amelynek tangensét ki kell számítani. Átszámítás: fok * PI()/180 = radián"
			}
		]
	},
	{
		name: "TANH",
		description: "Eredményül egy szám tangens hiperbolikusát adja vissza.",
		arguments: [
			{
				name: "szám",
				description: "tetszőleges valós szám"
			}
		]
	},
	{
		name: "TÉNYLEGES",
		description: "Az éves tényleges kamatláb értékét adja eredményül.",
		arguments: [
			{
				name: "névleges_kamatláb",
				description: "a névleges kamatláb"
			},
			{
				name: "időszak_per_év",
				description: "a részidőszakok száma évenként"
			}
		]
	},
	{
		name: "TERÜLET",
		description: "Egy hivatkozásban lévő területek számát adja eredményül. A terület lehet egyetlen cella, vagy egymással határos cellákból álló tartomány.",
		arguments: [
			{
				name: "hivatkozás",
				description: "egy cellára vagy cellákból álló tartományra való hivatkozás; többszörös területre való hivatkozás is lehet"
			}
		]
	},
	{
		name: "TÍPUS",
		description: "Egy érték adattípusát jelölő egész számot ad eredményül: szám = 1; szöveg = 2; logikai érték = 4; hibaérték = 16; tömb = 64.",
		arguments: [
			{
				name: "érték",
				description: "bármilyen érték lehet"
			}
		]
	},
	{
		name: "TISZTÍT",
		description: "A szövegből eltünteti az összes nem kinyomtatható karaktert.",
		arguments: [
			{
				name: "szöveg",
				description: "a munkalapon található bármely olyan információ, amelyet meg kell tisztítani a nem kinyomtatható karakterektől"
			}
		]
	},
	{
		name: "TIZEDES",
		description: "Egy szám megadott számrendszerbeli szöveges alakját decimális számmá alakítja.",
		arguments: [
			{
				name: "szám",
				description: "az átalakítandó szám"
			},
			{
				name: "alap",
				description: "az átalakítandó szám alapszáma"
			}
		]
	},
	{
		name: "TNÉV",
		description: "Egy szövegrész minden szavának kezdőbetűjét nagybetűre, az összes többi betűt pedig kisbetűre cseréli.",
		arguments: [
			{
				name: "szöveg",
				description: "idézőjelek közé zárt szöveg, szöveget eredményül adó képlet vagy szöveget tartalmazó cellára való hivatkozás"
			}
		]
	},
	{
		name: "TÖBBSZ.KEREKÍT",
		description: "A pontosság legközelebbi többszörösére kerekített értéket ad eredményül.",
		arguments: [
			{
				name: "szám",
				description: "a kerekítendő érték"
			},
			{
				name: "pontosság",
				description: "az a szám, amelynek a többszörösére az értéket kerekíteni kell"
			}
		]
	},
	{
		name: "TÖRTÉV",
		description: "A kezdő_dátum és a vég_dátum közötti teljes napok számát törtévként fejezi ki.",
		arguments: [
			{
				name: "kezdő_dátum",
				description: "a kezdő_dátum dátumértéke"
			},
			{
				name: "vég_dátum",
				description: "a vég_dátum dátumértéke"
			},
			{
				name: "alap",
				description: "az évtöredék számításának alapja"
			}
		]
	},
	{
		name: "TRANSZPONÁLÁS",
		description: "Függőleges cellatartományból vízszinteset állít elő, vagy fordítva.",
		arguments: [
			{
				name: "tömb",
				description: "a munkalap egy cellatartománya vagy egy értéktömb, amit transzponálni kíván"
			}
		]
	},
	{
		name: "TREND",
		description: "Visszatérési érték a legkisebb négyzetek módszere szerint az ismert adatpontokra fektetett egyenes segítségével lineáris trend számértéke.",
		arguments: [
			{
				name: "ismert_y",
				description: "az y = mx + b összefüggésből már ismert y-értékekből álló tartomány vagy tömb"
			},
			{
				name: "ismert_x",
				description: "az y = mx + b összefüggésből már ismert x-értékekből álló tartomány vagy tömb, nem kötelező megadni. Mérete azonos az ismert y-értékekből álló tömb vagy tartomány méretével"
			},
			{
				name: "új_x",
				description: "az új x-értékekből álló azon tartomány vagy tömb, amelyre a TREND függvénynek a megfelelő y-értékeket ki kell számítani"
			},
			{
				name: "konstans",
				description: "logikai érték: ha IGAZ vagy elhagyjuk, a b állandó kiszámítása a szokásos módon történik; ha HAMIS, a b állandó kötelezően 0 lesz"
			}
		]
	},
	{
		name: "UNICODE",
		description: "A szöveg első karakterének megfelelő számot (kódpontot) adja eredményül.",
		arguments: [
			{
				name: "szöveg",
				description: "az a szám, amelynek az Unicode-értékét keresi"
			}
		]
	},
	{
		name: "ÜRES",
		description: "Megvizsgálja, hogy a hivatkozás üres cellára mutat-e, és IGAZ vagy HAMIS értéket ad vissza.",
		arguments: [
			{
				name: "érték",
				description: "a megvizsgálandó cellára hivatkozó cella vagy név"
			}
		]
	},
	{
		name: "URL.KÓDOL",
		description: "Egy URL-kódolt karakterláncot ad vissza.",
		arguments: [
			{
				name: "szöveg",
				description: "egy karakterlánc, amelyet URL-ként szeretne kódolni"
			}
		]
	},
	{
		name: "UTOLSÓ.SZELVÉNYDÁTUM",
		description: "A kifizetés előtti utolsó szelvénydátumot adja eredményül.",
		arguments: [
			{
				name: "kiegyenlítés",
				description: "az értékpapír kifizetésének időpontja dátumértékként kifejezve"
			},
			{
				name: "lejárat",
				description: "az értékpapír lejáratának időpontja dátumértékként kifejezve"
			},
			{
				name: "gyakoriság",
				description: "a kamat- vagy osztalékszelvényekre történő kifizetések száma egy év alatt"
			},
			{
				name: "alap",
				description: "az évtöredék számításának alapja"
			}
		]
	},
	{
		name: "VAGY",
		description: "Megvizsgálja, hogy valamelyik értékére érvényes-e az IGAZ, és IGAZ vagy HAMIS eredményt ad vissza. Eredménye csak akkor HAMIS, ha minden argumentuma HAMIS.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logikai1",
				description: "a megvizsgálandó feltételek; mindegyik értéke IGAZ vagy HAMIS lehet, számuk pedig 1 és 255 között lehet"
			},
			{
				name: "logikai2",
				description: "a megvizsgálandó feltételek; mindegyik értéke IGAZ vagy HAMIS lehet, számuk pedig 1 és 255 között lehet"
			}
		]
	},
	{
		name: "VÁLASZT",
		description: "Értékek egy listájából választ ki egy elemet vagy végrehajtandó műveletet, indexszám alapján.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "index",
				description: "a kiválasztott argumentumot határozza meg. Az indexszámnak 1 és 254 közötti számnak vagy 1 és 254 közötti számot adó képletnek vagy hivatkozásnak kell lennie"
			},
			{
				name: "érték1",
				description: "legfeljebb 254 szám, cellahivatkozás, definiált név, képlet, függvény vagy szöveges argumentum; a VÁLASZT ezek közül választ"
			},
			{
				name: "érték2",
				description: "legfeljebb 254 szám, cellahivatkozás, definiált név, képlet, függvény vagy szöveges argumentum; a VÁLASZT ezek közül választ"
			}
		]
	},
	{
		name: "VALÓSZÍNŰSÉG",
		description: "Annak valószínűségét számítja ki, hogy adott értékek két határérték közé esnek, vagy az alsó határértékkel egyenlőek.",
		arguments: [
			{
				name: "x_tartomány",
				description: "azon számértékeket tartalmazó tartomány, amelyekhez ismertek a valószínűségértékek"
			},
			{
				name: "val_tartomány",
				description: "az x_tartományban található számokhoz rendelt valószínűségek; 0 és 1 közötti értékek a 0 kivételével"
			},
			{
				name: "alsó_határ",
				description: "az adatok alsó határa a valószínűség kiszámításához"
			},
			{
				name: "felső_határ",
				description: "az adatok felső határa (nem kötelező). Ha elhagyjuk, a függvény annak a valószínűségét adja meg, hogy az x-tartomány elemei az alsó-határral egyenlőek."
			}
		]
	},
	{
		name: "VAR",
		description: "Minta alapján becslést ad a varianciára (a mintában lévő logikai értékeket és szövegeket figyelmen kívül hagyja).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "a statisztikai mintát reprezentáló numerikus argumentumok, számuk 1 és 255 között lehet"
			},
			{
				name: "szám2",
				description: "a statisztikai mintát reprezentáló numerikus argumentumok, számuk 1 és 255 között lehet"
			}
		]
	},
	{
		name: "VAR.M",
		description: "Minta alapján becslést ad a varianciára (a mintában lévő logikai értékeket és szövegeket figyelmen kívül hagyja).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "a statisztikai mintát reprezentáló numerikus argumentumok, számuk 1 és 255 között lehet"
			},
			{
				name: "szám2",
				description: "a statisztikai mintát reprezentáló numerikus argumentumok, számuk 1 és 255 között lehet"
			}
		]
	},
	{
		name: "VAR.S",
		description: "Egy statisztikai sokaság varianciáját számítja ki (a sokaságban lévő logikai értékeket és szövegeket figyelmen kívül hagyja).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "a statisztikai sokaságot reprezentáló numerikus argumentumok, számuk 1 és 255 között lehet"
			},
			{
				name: "szám2",
				description: "a statisztikai sokaságot reprezentáló numerikus argumentumok, számuk 1 és 255 között lehet"
			}
		]
	},
	{
		name: "VARA",
		description: "Minta alapján becslést ad a sokaság varianciájára, a logikai értékek és a szövegek figyelembevételével. A szöveg és a HAMIS logikai érték 0-nak, az IGAZ logikai érték 1-nek számít.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "érték1",
				description: "a sokaságból vett minta értékei, számuk 1 és 255 között lehet"
			},
			{
				name: "érték2",
				description: "a sokaságból vett minta értékei, számuk 1 és 255 között lehet"
			}
		]
	},
	{
		name: "VARIÁCIÓK",
		description: "Adott számú objektum k-ad osztályú ismétlés nélküli variációinak számát számítja ki.",
		arguments: [
			{
				name: "szám",
				description: "az összes objektum száma"
			},
			{
				name: "hány_kiválasztott",
				description: "az egy-egy alkalommal kiválasztott objektumok száma"
			}
		]
	},
	{
		name: "VARIÁCIÓK.ISM",
		description: "Adott számú objektum k-ad osztályú ismétléses variációinak számát számítja ki.",
		arguments: [
			{
				name: "szám",
				description: "az összes objektum száma"
			},
			{
				name: "hány_kiválasztott",
				description: "az egy-egy alkalommal kiválasztott objektumok száma"
			}
		]
	},
	{
		name: "VARP",
		description: "Egy statisztikai sokaság varianciáját számítja ki (a sokaságban lévő logikai értékeket és szövegeket figyelmen kívül hagyja).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "szám1",
				description: "a statisztikai sokaságot reprezentáló numerikus argumentumok, számuk 1 és 255 között lehet"
			},
			{
				name: "szám2",
				description: "a statisztikai sokaságot reprezentáló numerikus argumentumok, számuk 1 és 255 között lehet"
			}
		]
	},
	{
		name: "VARPA",
		description: "A statisztikai sokaság egészéből kiszámítja a varianciát, a logikai értékek és a szövegek figyelembevételével. A szöveg és a HAMIS logikai érték 0-nak, az IGAZ logikai érték 1-nek számít.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "érték1",
				description: "a sokasághoz tartozó értékek, számuk 1 és 255 között lehet"
			},
			{
				name: "érték2",
				description: "a sokasághoz tartozó értékek, számuk 1 és 255 között lehet"
			}
		]
	},
	{
		name: "VÉL",
		description: "0-nál nagyobb vagy azzal egyenlő és 1-nél kisebb egyenletesen elosztott véletlenszámot ad eredményül (az újraszámítástól függően).",
		arguments: [
		]
	},
	{
		name: "VÉLETLEN.KÖZÖTT",
		description: "Két adott szám közé eső véletlen számot állít elő.",
		arguments: [
			{
				name: "alsó",
				description: "az alsó, egész értékű határérték"
			},
			{
				name: "felső",
				description: "a felső, egész értékű határérték"
			}
		]
	},
	{
		name: "VIA",
		description: "Valós idejű adatok visszakeresése COM automatizmust támogató programból.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "progID",
				description: "a regisztrált beépülő COM automatizmus programazonosítójának a neve. A nevet tegye idézőjelek közé"
			},
			{
				name: "kiszolgáló",
				description: "annak a kiszolgálónak a neve, ahol a beépülőnek működni kellene A nevet tegye idézőjelek közé. Ha a beépülő helyileg fut, adjon meg üres karakterláncot"
			},
			{
				name: "téma1",
				description: "az adatdarabokat megadó paraméterek, számuk 1 és 38 közötti"
			},
			{
				name: "téma2",
				description: "az adatdarabokat megadó paraméterek, számuk 1 és 38 közötti"
			}
		]
	},
	{
		name: "VKERES",
		description: "A tábla vagy értéktömb felső sorában megkeresi az értéket, és a megtalált elem oszlopából a megadott sorban elhelyezkedő értéket adja eredményül.",
		arguments: [
			{
				name: "keresési_érték",
				description: "az az érték, amelyet a függvény a tábla első sorában keres. Érték, hivatkozás vagy szöveg lehet."
			},
			{
				name: "tábla",
				description: "az a szövegekből, számokból vagy logikai értékekből álló tábla, amelyben a keresés történik. A tábla lehet tartományra vagy tartománynévre való hivatkozás is."
			},
			{
				name: "sor_szám",
				description: "a táblázat azon sorának száma, amely sorbeli elemet adja vissza a függvény a megtalált oszlopból. A táblázat első értéksora az 1-es számú sor."
			},
			{
				name: "tartományban_keres",
				description: "logikai érték: HAMIS esetén pontos egyezés szükséges; ha IGAZ, vagy elhagyjuk, a felső sorban lévő legjobb közelítést adja meg, növekvő rendezés esetén"
			}
		]
	},
	{
		name: "WEIBULL",
		description: "A Weibull-féle eloszlás értékét számítja ki.",
		arguments: [
			{
				name: "x",
				description: "az az érték, amelynél a függvény értékét ki kell számítani, nem negatív szám"
			},
			{
				name: "alfa",
				description: "az eloszlás paramétere, pozitív szám"
			},
			{
				name: "béta",
				description: "az eloszlás paramétere, pozitív szám"
			},
			{
				name: "eloszlásfv",
				description: "a függvény fajtáját megadó logikai érték: ha IGAZ, akkor az eloszlásfüggvény értékét számítja ki; ha HAMIS, a tömegfüggvényét"
			}
		]
	},
	{
		name: "WEIBULL.ELOSZLÁS",
		description: "A Weibull-féle eloszlás értékét számítja ki.",
		arguments: [
			{
				name: "x",
				description: "az az érték, amelynél a függvény értékét ki kell számítani, nem negatív szám"
			},
			{
				name: "alfa",
				description: "az eloszlás paramétere, pozitív szám"
			},
			{
				name: "béta",
				description: "az eloszlás paramétere, pozitív szám"
			},
			{
				name: "eloszlásfv",
				description: "a függvény fajtáját megadó logikai érték: ha IGAZ, akkor az eloszlásfüggvény értékét számítja ki; ha HAMIS, a sűrűségfüggvényét"
			}
		]
	},
	{
		name: "XBMR",
		description: "Ütemezett készpénzforgalom (cash flow) belső megtérülési kamatrátáját adja eredményül.",
		arguments: [
			{
				name: "értékek",
				description: "az egyes fizetési dátumokhoz tartozó be- vagy kifizetések"
			},
			{
				name: "dátumok",
				description: "az egyes be- vagy kifizetésekhez tartozó dátumok"
			},
			{
				name: "becslés",
				description: "az XIRR függvény eredményének becsült értéke"
			}
		]
	},
	{
		name: "XNJÉ",
		description: "Ütemezett készpénzforgalom (cash flow) nettó jelenlegi értékét adja eredményül.",
		arguments: [
			{
				name: "ráta",
				description: "a be- vagy kifizetéseknél alkalmazott leszámítolási ráta"
			},
			{
				name: "értékek",
				description: "az egyes fizetési dátumokhoz tartozó be- vagy kifizetések"
			},
			{
				name: "dátumok",
				description: "az egyes be- vagy kifizetésekhez tartozó dátumok"
			}
		]
	},
	{
		name: "XVAGY",
		description: "Logikai „kizárólagos vagy” műveletet végez az összes argumentummal.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logikai1",
				description: "a megvizsgálandó feltételek; logikai értékek, tömbök vagy hivatkozások, mindegyik értéke IGAZ vagy HAMIS, számuk pedig 1 és 254 közötti lehet"
			},
			{
				name: "logikai2",
				description: "a megvizsgálandó feltételek; logikai értékek, tömbök vagy hivatkozások, mindegyik értéke IGAZ vagy HAMIS, számuk pedig 1 és 254 közötti lehet"
			}
		]
	},
	{
		name: "Z.PRÓB",
		description: "Az egyszélű z-próbával kapott P-értéket (az aggregált elsőfajú hiba nagyságát) számítja ki.",
		arguments: [
			{
				name: "tömb",
				description: "az x-szel összevetendő adatokat tartalmazó tömb vagy tartomány"
			},
			{
				name: "x",
				description: "a vizsgálandó érték"
			},
			{
				name: "szigma",
				description: "a sokaság (ismert) szórása. Ha elhagyja, a minta szórása lesz felhasználva"
			}
		]
	},
	{
		name: "Z.PRÓBA",
		description: "Az egyszélű z-próbával kapott P-értéket (az aggregált elsőfajú hiba nagyságát) számítja ki.",
		arguments: [
			{
				name: "tömb",
				description: "az x-szel összevetendő adatokat tartalmazó tömb vagy tartomány"
			},
			{
				name: "x",
				description: "a vizsgálandó érték"
			},
			{
				name: "szigma",
				description: "a sokaság (ismert) szórása. Ha elhagyja, a minta szórása lesz felhasználva"
			}
		]
	}
];