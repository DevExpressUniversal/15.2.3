ASPxClientSpreadsheet.Functions = [
	{
		name: "ACOS",
		description: "Palauttaa luvun arcuskosinin radiaaneina väliltä 0 - pii. Arcuskosini on kulma, jonka kosini on luku.",
		arguments: [
			{
				name: "luku",
				description: "on laskettavan kulman kosini ja sen arvon on oltava välillä -1 - 1"
			}
		]
	},
	{
		name: "ACOSH",
		description: "Palauttaa luvun hyperbolisen kosinin käänteisfunktion arvon.",
		arguments: [
			{
				name: "luku",
				description: "on mikä tahansa reaaliluku, joka on suurempi tai yhtä suuri kuin 1"
			}
		]
	},
	{
		name: "ACOT",
		description: "Palauttaa luvun arkuskotangentin radiaaneina 0 - pii.",
		arguments: [
			{
				name: "luku",
				description: "on haluamasi kulman kotangentti"
			}
		]
	},
	{
		name: "ACOTH",
		description: "Palauttaa luvun käänteisen hyperbolisen kotangentin.",
		arguments: [
			{
				name: "luku",
				description: "on haluamasi hyperbolisen kotangentin kulma"
			}
		]
	},
	{
		name: "AIKA",
		description: "Muuntaa lukuina annetut tunnit, minuutit ja sekunnit Spreadsheetin aikamuotoilluksi järjestysnumeroksi.",
		arguments: [
			{
				name: "tunnit",
				description: "on luku 0 - 23, joka vastaa tunteja"
			},
			{
				name: "minuutit",
				description: "on luku 0 - 59, joka vastaa minuutteja"
			},
			{
				name: "sekunnit",
				description: "on luku 0 - 59, joka vastaa sekunteja"
			}
		]
	},
	{
		name: "AIKA_ARVO",
		description: "Muuntaa tekstimuotoisen ajan Spreadsheetin aikaa ilmaisevaksi järjestysnumeroksi. Numero 0 (0:00:00) muunnetaan muotoon 0,999988426 (23:59:59). Muotoile numero aikamuotoon kaavan kirjoittamisen jälkeen.",
		arguments: [
			{
				name: "aika_teksti",
				description: "on teksti, joka määrittää ajan jossakin Spreadsheetin tunnistamista ajan esitysmuodoista (tekstissä olevia päivämäärätietoja ei huomioida)"
			}
		]
	},
	{
		name: "ALUEET",
		description: "Palauttaa viittauksessa olevien alueiden määrän. Alue on yhtenäinen joukko soluja tai yksittäinen solu.",
		arguments: [
			{
				name: "viittaus",
				description: "on viittaus soluun tai solualueeseen. Se voi viitata myös useisiin alueisiin"
			}
		]
	},
	{
		name: "ARABIA",
		description: "Muuntaa roomalaiset numerot arabialaisiksi numeroiksi.",
		arguments: [
			{
				name: "teksti",
				description: "on muunnettava roomalainen numero"
			}
		]
	},
	{
		name: "ARVO",
		description: "Muuntaa lukuarvoa kuvaavan merkkijonon luvuksi.",
		arguments: [
			{
				name: "teksti",
				description: "on lainausmerkeissä oleva teksti tai viittaus muunnettavan tekstin sisältävään soluun"
			}
		]
	},
	{
		name: "ARVON.MUKAAN",
		description: "Palauttaa luvun sijainnin lukuarvoluettelossa. Luvun arvon suhteessa muihin luettelon lukuihin.",
		arguments: [
			{
				name: "luku",
				description: "on luku, jonka järjestyksen suhteessa muihin haluat tietää"
			},
			{
				name: "viittaus",
				description: "on lukuarvomatriisi tai viittaus lukuarvoluetteloon. Funktio ohittaa tässä argumentissa määritetyt arvot, jotka eivät ole lukuarvoja"
			},
			{
				name: "järjestys",
				description: "on luku, joka määrittää funktion käyttämän lajittelujärjestyksen. Jos arvo on 0 tai puuttuu, luettelo lajitellaan laskevaan järjestykseen. Jos arvo poikkeaa nollasta, luettelo lajitellaan nousevaan järjestykseen"
			}
		]
	},
	{
		name: "ARVON.MUKAAN.KESKIARVO",
		description: "Palauttaa luvun sijainnin lukuarvoluettelossa, eli sen arvon suhteessa muihin luettelon lukuihin. Jos useammalla kuin yhdellä arvolla on sama sijainti, funktio palauttaa keskimääräisen sijainnin.",
		arguments: [
			{
				name: "luku",
				description: "on luku, jonka järjestyksen suhteessa muihin haluat tietää"
			},
			{
				name: "viittaus",
				description: "on lukuarvomatriisi tai viittaus lukuarvoluetteloon. Funktio ohittaa tässä argumentissa määritetyt arvot, jotka eivät ole lukuarvoja"
			},
			{
				name: "järjestys",
				description: "on luku, joka määrittää funktion käyttämän lajittelujärjestyksen. Jos arvo on 0 tai puuttuu, luettelo lajitellaan laskevaan järjestykseen. Jos arvo poikkeaa nollasta, luettelo lajitellaan nousevaan järjestykseen"
			}
		]
	},
	{
		name: "ARVON.MUKAAN.TASAN",
		description: "Palauttaa luvun sijainnin lukuarvoluettelossa, eli sen arvon suhteessa muihin luettelon lukuihin. Jos useammalla kuin yhdellä arvolla on sama sijainti, funktio palauttaa arvojoukon arvojen korkeimman sijainnin.",
		arguments: [
			{
				name: "luku",
				description: "on luku, jonka järjestyksen suhteessa muihin haluat tietää"
			},
			{
				name: "viittaus",
				description: "on lukuarvomatriisi tai viittaus lukuarvoluetteloon. Funktio ohittaa tässä argumentissa määritetyt arvot, jotka eivät ole lukuarvoja"
			},
			{
				name: "järjestys",
				description: "on luku, joka määrittää funktion käyttämän lajittelujärjestyksen. Jos arvo on 0 tai puuttuu, luettelo lajitellaan laskevaan järjestykseen. Jos arvo poikkeaa nollasta, luettelo lajitellaan nousevaan järjestykseen"
			}
		]
	},
	{
		name: "ASIN",
		description: "Palauttaa luvun arcussinin radiaaneina välillä -pii/2 - pii/2.",
		arguments: [
			{
				name: "luku",
				description: "on halutun kulman sini, ja sen arvo on välillä -1 - 1"
			}
		]
	},
	{
		name: "ASINH",
		description: "Palauttaa luvun hyperbolisen sinin käänteisfunktion arvon.",
		arguments: [
			{
				name: "luku",
				description: "on mikä tahansa reaaliluku, joka on suurempi tai yhtä suuri kuin 1"
			}
		]
	},
	{
		name: "ASTEET",
		description: "Muuntaa radiaanit asteiksi.",
		arguments: [
			{
				name: "kulma",
				description: "on muunnettava kulma radiaaneina"
			}
		]
	},
	{
		name: "ATAN",
		description: "Palauttaa luvun arcustangentin radiaaneina. Tulos on välillä -pii/2 - pii/2.",
		arguments: [
			{
				name: "luku",
				description: "on halutun kulman tangentti"
			}
		]
	},
	{
		name: "ATAN2",
		description: "Palauttaa pisteen (x,y) arcustangentin radiaaneina. Tulos on välillä -pii - pii, poislukien -pii.",
		arguments: [
			{
				name: "x_luku",
				description: "on pisteen x-koordinaatti"
			},
			{
				name: "y_luku",
				description: "on pisteen y-koordinaatti"
			}
		]
	},
	{
		name: "ATANH",
		description: "Palauttaa luvun hyperbolisen tangentin käänteisfunktion arvon.",
		arguments: [
			{
				name: "luku",
				description: "on mikä tahansa reaaliluku, joka on suurempi kuin -1 tai pienempi kuin 1"
			}
		]
	},
	{
		name: "BAHTTEKSTI",
		description: "Muuntaa luvun tekstiksi (baht).",
		arguments: [
			{
				name: "luku",
				description: "on luku, jonka haluat muuntaa"
			}
		]
	},
	{
		name: "BEETA.JAKAUMA",
		description: "Palauttaa beeta-todennäköisyysjakaumafunktion arvon.",
		arguments: [
			{
				name: "x",
				description: "on arvo väliltä A - B, jossa funktion arvo lasketaan"
			},
			{
				name: "alfa",
				description: "on jakauman parametri. Arvon täytyy olla positiivinen"
			},
			{
				name: "beeta",
				description: "on jakauman parametri. Arvon täytyy olla positiivinen"
			},
			{
				name: "kertymä",
				description: "on looginen arvo. Jos arvo on TOSI, funktio laskee todennäköisyyden kertymäfunktion. Jos arvo on EPÄTOSI, funktio laskee todennäköisyystiheysfunktion"
			},
			{
				name: "A",
				description: "on valinnainen alaraja x:n arvoille. Jos arvoa ei määritetä, ohjelma käyttää arvoa A = 0"
			},
			{
				name: "B",
				description: "on valinnainen yläraja x:n arvoille. Jos arvoa ei määritetä, ohjelma käyttää arvoa B = 1"
			}
		]
	},
	{
		name: "BEETA.KÄÄNT",
		description: "Palauttaa kumulatiivisen beeta-todennäköisyystiheysfunktion (BEETA.JAKAUMA) käänteisarvon.",
		arguments: [
			{
				name: "todennäköisyys",
				description: "on beetajakaumaan liittyvä todennäköisyys"
			},
			{
				name: "alfa",
				description: "on jakauman parametri. Arvon täytyy olla positiivinen"
			},
			{
				name: "beeta",
				description: "on jakauman parametri. Arvon täytyy olla positiivinen"
			},
			{
				name: "A",
				description: "on valinnainen alaraja x:n arvoille. Jos arvoa ei määritetä, ohjelma käyttää arvoa A = 0"
			},
			{
				name: "B",
				description: "on valinnainen yläraja x:n arvoille. Jos arvoa ei määritetä, ohjelma käyttää arvoa B = 1"
			}
		]
	},
	{
		name: "BEETAJAKAUMA",
		description: "Palauttaa kumulatiivisen beeta-todennäköisyystiheysfunktion arvon.",
		arguments: [
			{
				name: "x",
				description: "on arvo väliltä A - B, jossa funktion arvo lasketaan"
			},
			{
				name: "alfa",
				description: "on jakauman parametri. Arvon täytyy olla positiivinen"
			},
			{
				name: "beeta",
				description: "on jakauman parametri. Arvon täytyy olla positiivinen"
			},
			{
				name: "A",
				description: "on valinnainen alaraja x:n arvoille. Jos arvoa ei määritetä, ohjelma käyttää arvoa A = 0"
			},
			{
				name: "B",
				description: "on valinnainen yläraja x:n arvoille. Jos arvoa ei määritetä, ohjelma käyttää arvoa B = 1"
			}
		]
	},
	{
		name: "BEETAJAKAUMA.KÄÄNT",
		description: "Palauttaa kumulatiivisen beeta-todennäköisyystiheysfunktion (BEETAJAKAUMA) käänteisarvon.",
		arguments: [
			{
				name: "todennäköisyys",
				description: "on beetajakaumaan liittyvä todennäköisyys"
			},
			{
				name: "alfa",
				description: "on jakauman parametri. Arvon täytyy olla positiivinen"
			},
			{
				name: "beeta",
				description: "on jakauman parametri. Arvon täytyy olla positiivinen"
			},
			{
				name: "A",
				description: "on valinnainen alaraja x:n arvoille. Jos arvoa ei määritetä, ohjelma käyttää arvoa A = 0"
			},
			{
				name: "B",
				description: "on valinnainen yläraja x:n arvoille. Jos arvoa ei määritetä, ohjelma käyttää arvoa B = 1"
			}
		]
	},
	{
		name: "BESSELI",
		description: "Palauttaa muutetun Besselin funktion ln(x).",
		arguments: [
			{
				name: "x",
				description: "on arvo, jossa funktion arvo lasketaan"
			},
			{
				name: "n",
				description: "on Besselin funktion kertaluokka"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "Palauttaa Besselin funktion Jn(x).",
		arguments: [
			{
				name: "x",
				description: "on arvo, jossa funktion arvo lasketaan"
			},
			{
				name: "n",
				description: "on Besselin funktion kertaluokka"
			}
		]
	},
	{
		name: "BESSELK",
		description: "Palauttaa muutetun Besselin funktion Kn(x).",
		arguments: [
			{
				name: "x",
				description: "on arvo, jossa funktion arvo lasketaan"
			},
			{
				name: "n",
				description: "on funktion kertaluokka"
			}
		]
	},
	{
		name: "BESSELY",
		description: "Palauttaa Besselin funktion Yn(x).",
		arguments: [
			{
				name: "x",
				description: "on arvo, jossa funktion arvo lasketaan"
			},
			{
				name: "n",
				description: "on funktion kertaluokka"
			}
		]
	},
	{
		name: "BINDES",
		description: "Muuntaa binaariluvun desimaaliluvuksi.",
		arguments: [
			{
				name: "luku",
				description: "on binaariluku, jonka haluat muuntaa"
			}
		]
	},
	{
		name: "BINHEKSA",
		description: "Muuntaa binaariluvun heksadesimaaliluvuksi.",
		arguments: [
			{
				name: "luku",
				description: "on binaariluku, jonka haluat muuntaa"
			},
			{
				name: "merkit",
				description: "on käytettävien merkkien lukumäärä"
			}
		]
	},
	{
		name: "BINOKT",
		description: "Muuntaa binaariluvun oktaaliluvuksi.",
		arguments: [
			{
				name: "luku",
				description: "on binaariluku, jonka haluat muuntaa"
			},
			{
				name: "merkit",
				description: "on käytettävien merkkien lukumäärä"
			}
		]
	},
	{
		name: "BINOMI.JAKAUMA",
		description: "Palauttaa yksittäisen termin binomijakauman todennäköisyyden.",
		arguments: [
			{
				name: "luku_tot",
				description: "on onnistuneiden yritysten määrä"
			},
			{
				name: "yritykset",
				description: "on toisistaan riippumattomien yritysten määrä"
			},
			{
				name: "todennäköisyys_tot",
				description: "on yrityksen onnistumisen todennäköisyys"
			},
			{
				name: "kumulatiivinen",
				description: "on totuusarvo. Jos arvo on TOSI, jakaumalle lasketaan  kertymäfunktio. Jos arvo on EPÄTOSI, jakaumalle lasketaan todennäköisyysmassafunktio"
			}
		]
	},
	{
		name: "BINOMI.JAKAUMA.ALUE",
		description: "Palauttaa kokeen tuloksen todennäköisyyden binomijakaumaa käyttämällä.",
		arguments: [
			{
				name: "kokeet",
				description: "on itsenäisten kokeiden luku"
			},
			{
				name: "todennäköisyys_s",
				description: "on onnistumisen todennäköisyys jokaisessa kokeessa"
			},
			{
				name: "luku_s",
				description: "on onnistumisen luku kokeissa"
			},
			{
				name: "luku_s2",
				description: "jos toiminto on käytettävissä, se palauttaa todennäköisyyden, että onnistuneiden kokeiden luku on lukujen luku_s ja luku_s2 välillä"
			}
		]
	},
	{
		name: "BINOMI.JAKAUMA.NEG",
		description: "Palauttaa negatiivisen binomijakauman eli todennäköisyyden, että ennen luku_tot:ttä onnistumista tapahtuu luku_epäon epäonnistumista onnistumistodennäköisyydellä todennäköisyys_tot.",
		arguments: [
			{
				name: "luku_epäon",
				description: "on epäonnistumisten määrä"
			},
			{
				name: "luku_tot",
				description: "on onnistumisten kynnysarvo"
			},
			{
				name: "todennäköisyys_tot",
				description: "on onnistumisen todennäköisyyttä osoittava luku välillä 0 - 1"
			},
			{
				name: "kertymä",
				description: "on looginen arvo. Jos arvo on TOSI, jakaumalle lasketaan kertymäfunktio. Jos arvo on EPÄTOSI, jakaumalle lasketaan todennäköisyysmassafunktio"
			}
		]
	},
	{
		name: "BINOMIJAKAUMA",
		description: "Palauttaa yksittäisen termin binomijakauman todennäköisyyden.",
		arguments: [
			{
				name: "luku_tot",
				description: "on onnistuneiden yritysten määrä"
			},
			{
				name: "yritykset",
				description: "on toisistaan riippumattomien yritysten määrä"
			},
			{
				name: "todennäköisyys_tot",
				description: "on yrityksen onnistumisen todennäköisyys"
			},
			{
				name: "kumulatiivinen",
				description: "on totuusarvo. Jos arvo on TOSI, jakaumalle lasketaan kertymäfunktio. Jos arvo on EPÄTOSI, jakaumalle lasketaan todennäköisyysmassafunktio"
			}
		]
	},
	{
		name: "BINOMIJAKAUMA.KÄÄNT",
		description: "Palauttaa pienimmän arvon, jossa binomijakauman kertymäfunktion arvo on pienempi tai yhtä suuri kuin ehtoarvo.",
		arguments: [
			{
				name: "yritykset",
				description: "on Bernoulli-yritysten määrä"
			},
			{
				name: "todennäköisyys_tot",
				description: "on yrityksen onnistumisen todennäköisyyttä osoittava luku välillä 0 - 1, päätepisteet mukaan lukien"
			},
			{
				name: "alfa",
				description: "on ehtoarvoa osoittava luku, joka on välillä 0 - 1, päätepisteet mukaan lukien"
			}
		]
	},
	{
		name: "BINOMIJAKAUMA.KRIT",
		description: "Palauttaa pienimmän arvon, jossa binomijakauman kertymäfunktion arvo on pienempi tai yhtä suuri kuin ehtoarvo.",
		arguments: [
			{
				name: "yritykset",
				description: "on Bernoulli-yritysten määrä"
			},
			{
				name: "todennäköisyys_tot",
				description: "on yrityksen onnistumisen todennäköisyyttä osoittava luku välillä 0 - 1, päätepisteet mukaan lukien"
			},
			{
				name: "alfa",
				description: "on ehtoarvoa osoittava luku, joka on välillä 0 - 1, päätepisteet mukaan lukien"
			}
		]
	},
	{
		name: "BINOMIJAKAUMA.NEG",
		description: "Palauttaa negatiivisen binomijakauman eli todennäköisyyden, että ennen luku_tot:ttä onnistumista tapahtuu luku_epäon epäonnistumista onnistumistodennäköisyydellä todennäköisyys_tot.",
		arguments: [
			{
				name: "luku_epäon",
				description: "on epäonnistumisten määrä"
			},
			{
				name: "luku_tot",
				description: "on onnistumisten kynnysarvo"
			},
			{
				name: "todennäköisyys_tot",
				description: "on onnistumisen todennäköisyyttä osoittava luku välillä 0 - 1"
			}
		]
	},
	{
		name: "BITTI.EHDOTON.TAI",
		description: "Palauttaa kahden luvun bittitason 'Poissulkeva Tai'.",
		arguments: [
			{
				name: "luku1",
				description: "on desimaalimuoto binääriluvusta, jonka haluat laskea"
			},
			{
				name: "luku2",
				description: "on desimaalimuoto binääriluvusta, jonka haluat laskea"
			}
		]
	},
	{
		name: "BITTI.JA",
		description: "Paluttaa kahden luvun bittitason 'Ja'.",
		arguments: [
			{
				name: "luku1",
				description: "on desimaalimuoto binääriluvusta, jonka haluat laskea"
			},
			{
				name: "luku2",
				description: "on desimaalimuoto binääriluvusta, jonka haluat laskea"
			}
		]
	},
	{
		name: "BITTI.SIIRTO.O",
		description: "Palauttaa siirrettävän_määrän bittimäärän verran oikealle siirretyn luvun.",
		arguments: [
			{
				name: "luku",
				description: "on desimaalimuoto binääriluvusta, jonka haluat laskea"
			},
			{
				name: "siirrettävä_määrä",
				description: "on bittiluku, jonka verran haluat siirtää Lukua oikealle"
			}
		]
	},
	{
		name: "BITTI.SIIRTO.V",
		description: "Palauttaa siirrettävän_määrän bittimäärän verran vasemmalle siirretyn luvun.",
		arguments: [
			{
				name: "luku",
				description: "on desimaalimuoto binääriluvusta, jonka haluat laskea"
			},
			{
				name: "siirrettävä_määrä",
				description: "on bittiluku, jonka verran haluat siirtää Lukua vasemmalle"
			}
		]
	},
	{
		name: "BITTI.TAI",
		description: "Palauttaa kahden luvun bittitason 'Tai'.",
		arguments: [
			{
				name: "luku1",
				description: "on desimaalimuoto binääriluvusta, jonka haluat laskea"
			},
			{
				name: "luku2",
				description: "on desimaalimuoto binääriluvusta, jonka haluat laskea"
			}
		]
	},
	{
		name: "CHIJAKAUMA",
		description: "Palauttaa oikeanpuolisen chi-neliön jakauman todennäköisyyden.",
		arguments: [
			{
				name: "x",
				description: "on ei-negatiivinen arvo, jossa haluat laskea jakauman"
			},
			{
				name: "vapausasteet",
				description: "on vapausasteiden määrää osoittava luku välillä 1 ja 10^10 (pois lukien 10^10)"
			}
		]
	},
	{
		name: "CHIJAKAUMA.KÄÄNT",
		description: "Palauttaa chi-neliön oikeanpuoleisen jakauman käänteisarvon.",
		arguments: [
			{
				name: "todennäköisyys",
				description: "on chi-neliön jakaumaan liittyvä todennäköisyys, jonka arvo on välillä 0 - 1 päätepisteet mukaan lukien"
			},
			{
				name: "vapausasteet",
				description: "on vapausasteiden määrää osoittava luku välillä 1 ja 10^10 (pois lukien 10^10)"
			}
		]
	},
	{
		name: "CHINELIÖ.JAKAUMA",
		description: "Palauttaa chi-neliön vasenhäntäisen jakauman todennäköisyyden.",
		arguments: [
			{
				name: "x",
				description: "on ei-negatiivinen arvo, jossa haluat laskea jakauman"
			},
			{
				name: "vapausasteet",
				description: "on vapausasteiden määrää osoittava luku välillä 1 - 10^10 (pois lukien 10^10)"
			},
			{
				name: "kertymä",
				description: "on looginen arvo. Jos arvo on TOSI, jakaumalle lasketaan kertymäfunktio. Jos arvo on EPÄTOSI, jakaumalle lasketaan todennäköisyystiheysfunktio"
			}
		]
	},
	{
		name: "CHINELIÖ.JAKAUMA.OH",
		description: "Palauttaa oikeahäntäisen chi-neliön jakauman todennäköisyyden.",
		arguments: [
			{
				name: "x",
				description: "on ei-negatiivinen arvo, jossa haluat laskea jakauman"
			},
			{
				name: "vapausasteet",
				description: "on vapausasteiden määrää osoittava luku välillä 1 - 10^10 (pois lukien 10^10)"
			}
		]
	},
	{
		name: "CHINELIÖ.KÄÄNT",
		description: "Palauttaa chi-neliön vasenhäntäisen jakauman käänteisarvon.",
		arguments: [
			{
				name: "todennäköisyys",
				description: "on chi-neliön jakaumaan liittyvä todennäköisyys, jonka arvo on välillä 0 - 1 päätepisteet mukaan lukien"
			},
			{
				name: "vapausasteet",
				description: "on vapausasteiden määrää osoittava luku välillä 1 - 10^10 (pois lukien 10^10)"
			}
		]
	},
	{
		name: "CHINELIÖ.KÄÄNT.OH",
		description: "Palauttaa chi-neliön vasenhäntäisen jakauman käänteisarvon.",
		arguments: [
			{
				name: "todennäköisyys",
				description: "on chi-neliön jakaumaan liittyvä todennäköisyys, jonka arvo on välillä 0 - 1 päätepisteet mukaan lukien"
			},
			{
				name: "vapausasteet",
				description: "on vapausasteiden määrää osoittava luku välillä 1 - 10^10 (pois lukien 10^10)"
			}
		]
	},
	{
		name: "CHINELIÖ.TESTI",
		description: "Palauttaa riippumattomuustestin tuloksen, eli chi-neliöjakauman arvo ja vapausasteiden määrän.",
		arguments: [
			{
				name: "todellinen_alue",
				description: "on tietoalue, joka sisältää havaitut arvot, joita verrataan odotettuihin arvoihin"
			},
			{
				name: "odotettu_alue",
				description: "on tietoalue, joka sisältää rivi- ja sarakesummien tulon suhteen loppusummaan"
			}
		]
	},
	{
		name: "CHITESTI",
		description: "Palauttaa riippumattomuustestin tuloksen, eli chi-neliöjakauman arvon ja vapausasteiden määrän.",
		arguments: [
			{
				name: "todellinen_alue",
				description: "on tietoalue, joka sisältää havaitut arvot, joita verrataan odotettuihin arvoihin"
			},
			{
				name: "odotettu_alue",
				description: "on tietoalue, joka sisältää rivi- ja sarakesummien tulon suhteen loppusummaan"
			}
		]
	},
	{
		name: "COS",
		description: "Palauttaa annetun kulman kosinin.",
		arguments: [
			{
				name: "luku",
				description: "on radiaaneina annettu kulma, jonka kosini halutaan laskea"
			}
		]
	},
	{
		name: "COSH",
		description: "Palauttaa luvun hyperbolisen kosinin.",
		arguments: [
			{
				name: "luku",
				description: "on mikä tahansa reaaliluku"
			}
		]
	},
	{
		name: "COT",
		description: "Palauttaa kulman kotangentin.",
		arguments: [
			{
				name: "luku",
				description: "on radiaaneina kulma, jolle haluat laskea kotangentin"
			}
		]
	},
	{
		name: "COTH",
		description: "Palauttaa luvun hyperbolisen kotangentin.",
		arguments: [
			{
				name: "luku",
				description: "on radiaaneina kulma, jolle haluat laskea hyperbolisen kotangentin"
			}
		]
	},
	{
		name: "DB",
		description: "Palauttaa kauden kirjanpidollisen poiston amerikkalaisen DB-menetelmän (Fixed-declining balance) mukaan.",
		arguments: [
			{
				name: "kustannus",
				description: "on sijoituksen alkuperäinen hankintahinta"
			},
			{
				name: "loppuarvo",
				description: "on sijoituksen arvo käyttöajan lopussa (jäännösarvo)"
			},
			{
				name: "aika",
				description: "on kausien määrä, joiden aikana sijoitus poistetaan (kutsutaan myös sijoituksen käyttöiäksi)"
			},
			{
				name: "kausi",
				description: "on kausi, jolta poisto halutaan laskea. Argumenteilla kausi ja aika on oltava sama yksikkö"
			},
			{
				name: "kuukausi",
				description: "on ensimmäisen vuoden kuukausien lukumäärä. Jos et määritä tätä argumenttia, funktio käyttää oletusarvoa 12"
			}
		]
	},
	{
		name: "DDB",
		description: "Palauttaa kauden kirjanpidollisen poiston amerikkalaisen DDB-menetemän (Double-Declining Balance) tai jonkun muun määrittämäsi menetelmän mukaan.",
		arguments: [
			{
				name: "kustannus",
				description: "on sijoituksen alkuperäinen hankintahinta"
			},
			{
				name: "loppuarvo",
				description: "on sijoituksen arvo käyttöajan lopussa (jäännösarvo)"
			},
			{
				name: "aika",
				description: "on kausien määrä, joiden aikana sijoitus poistetaan (kutsutaan myös sijoituksen käyttöiäksi)"
			},
			{
				name: "kausi",
				description: "on kausi, jolta poisto halutaan laskea. Argumenteilla kausi ja aika on oltava sama yksikkö"
			},
			{
				name: "kerroin",
				description: "on poistonopeus. Jos et määritä argumenttia kerroin, funktio käyttää oletusarvoa 2 (Double-Declining Balance)"
			}
		]
	},
	{
		name: "DESBIN",
		description: "Muuntaa kymmenjärjestelmän luvun binaariluvuksi.",
		arguments: [
			{
				name: "luku",
				description: "on kymmenjärjestelmän luku, jonka haluat muuntaa"
			},
			{
				name: "merkit",
				description: "on käytettävien merkkien lukumäärä"
			}
		]
	},
	{
		name: "DESHEKSA",
		description: "Muuntaa kymmenjärjestelmän luvun heksadesimaaliluvuksi.",
		arguments: [
			{
				name: "luku",
				description: "on kymmenjärjestelmän luku, jonka haluat muuntaa"
			},
			{
				name: "merkit",
				description: "on käytettävien merkkien lukumäärä"
			}
		]
	},
	{
		name: "DESIMAALI",
		description: "Muuntaa annetun kannan luvun tekstimuodon desimaaliluvuksi.",
		arguments: [
			{
				name: "luku",
				description: "on luku, jonka haluat muuntaa"
			},
			{
				name: "kantaluku",
				description: "on muunnettavan luvun kantaluku"
			}
		]
	},
	{
		name: "DESOKT",
		description: "Muuntaa kymmenjärjestelmän luvun oktaaliluvuksi.",
		arguments: [
			{
				name: "luku",
				description: "on kymmenjärjestelmän luku, jonka haluat muuntaa"
			},
			{
				name: "merkit",
				description: "on käytettävien merkkien lukumäärä"
			}
		]
	},
	{
		name: "DISKONTTOKORKO",
		description: "Palauttaa arvopaperin diskonttokoron.",
		arguments: [
			{
				name: "tilityspvm",
				description: "on arvopaperin tilityspäivämäärä järjestysnumerona"
			},
			{
				name: "erääntymispvm",
				description: "on arvopaperin erääntymispäivämäärä järjestysnumerona"
			},
			{
				name: "hinta",
				description: "on arvopaperin hinta (100 euron nimellisarvo)"
			},
			{
				name: "lunastushinta",
				description: "on arvopaperin lunastushinta (100 euron nimellisarvo)"
			},
			{
				name: "peruste",
				description: "on käytettävä päivien laskentaperuste"
			}
		]
	},
	{
		name: "EHDOTON.TAI",
		description: "Palauttaa argumenttien totuuden 'Poissulkeva Tai'.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "totuus1",
				description: "ovat 1 - 254 ehtoa, jotka haluat testata, ja ne voivat olla joko TOSI tai EPÄTOSI ja totuusarvoja, matriiseja tai viittauksia"
			},
			{
				name: "totuus2",
				description: "ovat 1 - 254 ehtoa, jotka haluat testata, ja ne voivat olla joko TOSI tai EPÄTOSI ja totuusarvoja, matriiseja tai viittauksia"
			}
		]
	},
	{
		name: "EI",
		description: "Kääntää EPÄTOSI-arvon TOSI-arvoksi tai TOSI-arvon EPÄTOSI-arvoksi.",
		arguments: [
			{
				name: "totuus",
				description: "on arvo tai lauseke, jolle saadaan arvo TOSI tai EPÄTOSI"
			}
		]
	},
	{
		name: "EKSPONENTIAALI.JAKAUMA",
		description: "Palauttaa eksponentiaalijakauman.",
		arguments: [
			{
				name: "x",
				description: "on funktion arvo ei-negatiivisena lukuna"
			},
			{
				name: "lambda",
				description: "on parametrin arvo. Argumentin täytyy olla positiivinen"
			},
			{
				name: "kumulatiivinen",
				description: "määrittää palautettavan eksponentiaalifunktion lajin: TOSI = jakauman kertymäfunktio; EPÄTOSI =  todennäköisyystiheysfunktio"
			}
		]
	},
	{
		name: "EKSPONENTIAALIJAKAUMA",
		description: "Palauttaa eksponentiaalijakauman.",
		arguments: [
			{
				name: "x",
				description: "on funktion arvo ei-negatiivisena lukuna"
			},
			{
				name: "lambda",
				description: "on parametrin arvo. Argumentin täytyy olla positiivinen"
			},
			{
				name: "kumulatiivinen",
				description: "määrittää palautettavan eksponentiaalifunktion lajin: TOSI = jakauman kertymäfunktio; EPÄTOSI =  todennäköisyystiheysfunktio"
			}
		]
	},
	{
		name: "EKSPONENTTI",
		description: "Palauttaa e:n korotettuna annettuun potenssiin.",
		arguments: [
			{
				name: "luku",
				description: "on e:n eksponentti. Kantaluvun e arvo on 2.71828182845904, mikä on luonnollisen logaritmin kantaluku"
			}
		]
	},
	{
		name: "ENNUSTE",
		description: "Laskee, tai ennustaa, arvojen lineaarisen trendin aiempien arvojen perusteella.",
		arguments: [
			{
				name: "x",
				description: "on arvopiste, jolle haluat ennustaa arvon. Argumentin täytyy olla numeerinen arvo"
			},
			{
				name: "tunnettu_y",
				description: "on riippuva matriisi tai tietoalue"
			},
			{
				name: "tunnettu_x",
				description: "riippumaton matriisi tai tietoalue. Tunnettujen arvojen varianssi ei saa olla nolla"
			}
		]
	},
	{
		name: "EPÄSUORA",
		description: "Palauttaa merkkijonon määrittämän viittauksen.",
		arguments: [
			{
				name: "viittaus_teksti",
				description: "on viittaus soluun, joka sisältää A1-tyyppisen viittauksen, R1C1-tyyppisen viittauksen tai nimen, joka on määritetty viittaukseksi, tai merkkijonomuotoisen viittauksen soluun"
			},
			{
				name: "a1",
				description: "on totuusarvo, joka määrittää, minkä tyyppinen viittaus on argumentissa viittaus_teksti. R1C1 =  EPÄTOSI, A1 = TOSI tai jätetty pois"
			}
		]
	},
	{
		name: "EPÄTOSI",
		description: "Palauttaa totuusarvon EPÄTOSI.",
		arguments: [
		]
	},
	{
		name: "ERISNIMI",
		description: "Muuntaa jokaisen tekstimuotoisen sanan ensimmäisen kirjaimen isoksi kirjaimeksi ja kaikki muut kirjaimet pieniksi.",
		arguments: [
			{
				name: "teksti",
				description: "on lainausmerkein rajattu teksti, kaava, joka palauttaa tekstiä, tai viittaus soluun, jossa on tekstiä, jonka alkukirjaimet haluat muuttaa isoiksi"
			}
		]
	},
	{
		name: "EROTUSTEN.NELIÖSUMMA",
		description: "Laskee kahden alueen tai matriisin toisiaan vastaavien arvojen erotusten neliösumman.",
		arguments: [
			{
				name: "matriisi_x",
				description: "on ensimmäinen matriisi tai arvoalue. Argumentti voi olla luku, nimi, matriisi tai viittaus, jossa on lukuja"
			},
			{
				name: "matriisi_y",
				description: "on toinen matriisi tai arvoalue. Argumentti voi olla luku, nimi, matriisi tai viittaus, jossa on lukuja"
			}
		]
	},
	{
		name: "ETSI",
		description: "Palauttaa kohdan, josta toisen merkkijonon sisällä oleva merkkijono alkaa. FIND-arvo ottaa huomioon kirjainkoon.",
		arguments: [
			{
				name: "etsittävä_teksti",
				description: "on etsittävä teksti. Käytä lainausmerkkejä (tyhjä teksti) ensimmäisen merkin tunnistamiseen. Yleismerkkejä (?, *) ei voi käyttää"
			},
			{
				name: "tekstissä",
				description: "on teksti, josta haluat etsiä merkkijonon"
			},
			{
				name: "aloitusnro",
				description: "määrittää sen merkin järjestysnumeron, josta haluat aloittaa etsinnän. Tekstin ensimmäisen merkin numero on 1. Jos tämä argumentti jätetään pois, etsintä aloitetaan ensimmäisestä merkistä"
			}
		]
	},
	{
		name: "ETUMERKKI",
		description: "Palauttaa luvun etumerkin. Jos luku on positiivinen, arvo on 1. Jos luku on nolla, funktio palauttaa arvon 0. Jos luku on negatiivinen, funktio palauttaa arvon -1.",
		arguments: [
			{
				name: "luku",
				description: "on mikä tahansa reaaliluku"
			}
		]
	},
	{
		name: "F.JAKAUMA",
		description: "Palauttaa (vasenhäntäisen) F-todennäköisyysjakauman (hajonnan aste) kahdesta tietosarjasta.",
		arguments: [
			{
				name: "x",
				description: "on arvopiste, jossa funktion arvo lasketaan. Argumentin täytyy olla positiivinen luku"
			},
			{
				name: "vapausaste1",
				description: "on osoittajan vapausasteita osoittava luku, joka on välillä 1 - 10^10, pois lukien 10^10"
			},
			{
				name: "vapausaste2",
				description: "on nimittäjän vapausasteita osoittava luku välillä 1 - 10^10, pois lukien 10^10"
			},
			{
				name: "kertymäfunktio",
				description: "on looginen arvo. Jos arvo on TOSI, funktion palauttaa jakauman kertymäfunktion. Jos arvo on EPÄTOSI, funktio palauttaa  todennäköisyystiheysfunktion"
			}
		]
	},
	{
		name: "F.JAKAUMA.OH",
		description: "Palauttaa (oikeahäntäisen) F-todennäköisyysjakauman (hajonnan aste) kahdesta tietosarjasta.",
		arguments: [
			{
				name: "x",
				description: "on arvopiste, jossa funktion arvo lasketaan. Argumentin täytyy olla positiivinen luku"
			},
			{
				name: "vapausaste1",
				description: "on osoittajan vapausasteita osoittava luku, joka on välillä 1 - 10^10, pois lukien 10^10"
			},
			{
				name: "vapausaste2",
				description: "on nimittäjän vapausasteita osoittava luku välillä 1 - 10^10, pois lukien 10^10"
			}
		]
	},
	{
		name: "F.KÄÄNT",
		description: "Palauttaa käänteisen (vasenhäntäisen) F-todennäköisyysjakauman. Jos p = F.JAKAUMA(x,...), niin F.KÄÄNT(p,...) = x.",
		arguments: [
			{
				name: "todennäköisyys",
				description: "on F-jakauman kertymäfunktioon liittyvää todennäköisyyttä osoittava luku välillä 0 - 1 päätepisteet mukaan lukien"
			},
			{
				name: "vapausaste1",
				description: "on osoittajan vapausasteita osoittava luku välillä 1 - 10^10 pois lukien 10^10"
			},
			{
				name: "vapausaste2",
				description: "on nimittäjän vapausasteita osoittava luku välillä 1 - 10^10 pois lukien 10^10"
			}
		]
	},
	{
		name: "F.KÄÄNT.OH",
		description: "Palauttaa käänteisen (oikeahäntäisen) F-todennäköisyysjakauman. Jos p = F.JAKAUMA.OH(x,...), niin F.KÄÄNT.OH(p,...) = x.",
		arguments: [
			{
				name: "todennäköisyys",
				description: "on F-jakauman kertymäfunktioon liittyvää todennäköisyyttä osoittava luku välillä 0-1 päätepisteet mukaan lukien"
			},
			{
				name: "vapausaste1",
				description: "on osoittajan vapausasteita osoittava luku välillä 1 - 10^10 pois lukien 10^10"
			},
			{
				name: "vapausaste2",
				description: "on nimittäjän vapausasteita osoittava luku välillä 1 - 10^10 pois lukien 10^10"
			}
		]
	},
	{
		name: "F.TESTI",
		description: "Palauttaa F-testin tuloksen eli kaksisuuntaisen todennäköisyyden sille, että matriisien 1 ja 2 varianssit eivät ole merkittävästi erilaisia.",
		arguments: [
			{
				name: "matriisi1",
				description: "on ensimmäinen matriisi tai tietoalue. Arvot voivat olla lukuja tai lukuja sisältäviä nimiä, matriiseja tai viittauksia (tyhjiä soluja ei huomioida)"
			},
			{
				name: "matriisi2",
				description: "on toinen matriisi tai tietoalue. Arvot voivat olla lukuja tai lukuja sisältäviä nimiä, matriiseja tai viittauksia (tyhjiä soluja ei huomioida)"
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
		name: "FII",
		description: "Palauttaa tiheysfunktion arvon normaalille vakiojakaumalle.",
		arguments: [
			{
				name: "x",
				description: "on luku, jolle haluat laskea normaalin vakiojakauman tiheyden"
			}
		]
	},
	{
		name: "FISHER",
		description: "Palauttaa Fisherin muunnoksen.",
		arguments: [
			{
				name: "x",
				description: "on -1 ja 1 välissä (päätepisteet poislukien) oleva luku, jolle haluat suorittaa muunnoksen"
			}
		]
	},
	{
		name: "FISHER.KÄÄNT",
		description: "Palauttaa käänteisen Fisherin muunnoksen. Jos y = FISHER(x), niin FISHER.KÄÄNT(y) = x.",
		arguments: [
			{
				name: "y",
				description: "on arvo, jolle haluat suorittaa käänteismuunnoksen"
			}
		]
	},
	{
		name: "FJAKAUMA",
		description: "Palauttaa F-todennäköisyysjakauman (hajonnan aste) kahdesta tietosarjasta.",
		arguments: [
			{
				name: "x",
				description: "on arvopiste, jossa funktion arvo lasketaan. Argumentin täytyy olla positiivinen luku"
			},
			{
				name: "vapausaste1",
				description: "on osoittajan vapausasteita osoittava luku, joka on välillä 1 - 10^10, pois lukien 10^10"
			},
			{
				name: "vapausaste2",
				description: "on nimittäjän vapausasteita osoittava luku välillä 1 - 10^10, pois lukien 10^10"
			}
		]
	},
	{
		name: "FJAKAUMA.KÄÄNT",
		description: "Palauttaa käänteisen (oikeanpuoleisen) F-todennäköisyysjakauman. Jos p = FJAKAUMA(x,...), niin FJAKAUMA.KÄÄNT(p,...) = x.",
		arguments: [
			{
				name: "todennäköisyys",
				description: "on F-jakauman kertymäfunktioon liittyvää todennäköisyyttä osoittava luku välillä 0-1 päätepisteet mukaan lukien"
			},
			{
				name: "vapausaste1",
				description: "on osoittajan vapausasteita osoittava luku välillä 1 - 10^10 pois lukien 10^10"
			},
			{
				name: "vapausaste2",
				description: "on nimittäjän vapausasteita osoittava luku välillä 1 - 10^10 pois lukien 10^10"
			}
		]
	},
	{
		name: "FTESTI",
		description: "Palauttaa F-testin tuloksen eli kaksisuuntaisen todennäköisyyden sille, että matriisien 1 ja 2 varianssit eivät ole merkittävästi erilaisia.",
		arguments: [
			{
				name: "matriisi1",
				description: "on ensimmäinen matriisi tai tietoalue. Arvot voivat olla lukuja tai lukuja sisältäviä nimiä, matriiseja tai viittauksia (tyhjiä soluja ei huomioida)"
			},
			{
				name: "matriisi2",
				description: "on toinen matriisi tai tietoalue. Arvot voivat olla lukuja tai lukuja sisältäviä nimiä, matriiseja tai viittauksia (tyhjiä soluja ei huomioida)"
			}
		]
	},
	{
		name: "GAMMA",
		description: "Palauttaa Gamma-toiminnon arvon.",
		arguments: [
			{
				name: "x",
				description: "on arvo, jolle haluat laskea gamman"
			}
		]
	},
	{
		name: "GAMMA.JAKAUMA",
		description: "Palauttaa gamma-jakauman.",
		arguments: [
			{
				name: "x",
				description: "on ei-negatiivinen arvo, jossa haluat laskea jakauman"
			},
			{
				name: "alfa",
				description: "on jakauman parametri. Arvon täytyy olla positiivinen"
			},
			{
				name: "beeta",
				description: "on positiivinen jakauman parametri. Jos beeta = 1, funktio palauttaa vakiomuotoisen gamma-jakauman"
			},
			{
				name: "kumulatiivinen",
				description: "on totuusarvo, joka määrittää, kumpi jakauma palautetaan. Jos arvo on TOSI, ohjelma palauttaa jakauman kertymäfunktion. Jos arvo on EPÄTOSI tai puuttuu, ohjelma palauttaa todennäköisyysmassafunktion"
			}
		]
	},
	{
		name: "GAMMA.JAKAUMA.KÄÄNT",
		description: "Palauttaa käänteisen gamma-jakauman kertymäfunktion: jos p = GAMMA.JAKAUMA(x,...), niin GAMMA.JAKAUMA.KÄÄNT(p,...) = x.",
		arguments: [
			{
				name: "todennäköisyys",
				description: "on gamma-jakaumaan liittyvää todennäköisyyttä osoittava luku. Luku on välillä 0 - 1 (päätepisteet mukaan lukien)"
			},
			{
				name: "alfa",
				description: "on jakauman parametri. Parametrin on oltava positiivinen luku"
			},
			{
				name: "beeta",
				description: "on jakauman parametria osoittava positiivinen luku. Jos beeta = 1, GAMMA.JAKAUMA.KÄÄNT-funktio palauttaa käänteisen vakiomuotoisen gamma-jakauman"
			}
		]
	},
	{
		name: "GAMMAJAKAUMA",
		description: "Palauttaa gamma-jakauman.",
		arguments: [
			{
				name: "x",
				description: "on ei-negatiivinen arvo, jossa haluat laskea jakauman"
			},
			{
				name: "alfa",
				description: "on jakauman parametri. Arvon täytyy olla positiivinen"
			},
			{
				name: "beeta",
				description: "on positiivinen jakauman parametri. Jos beeta = 1, funktio palauttaa vakiomuotoisen gamma-jakauman"
			},
			{
				name: "kumulatiivinen",
				description: "on totuusarvo, joka määrittää, kumpi jakauma palautetaan. Jos arvo on TOSI, ohjelma palauttaa jakauman kertymäfunktion. Jos arvo on EPÄTOSI tai puuttuu, ohjelma palauttaa todennäköisyysmassafunktion"
			}
		]
	},
	{
		name: "GAMMAJAKAUMA.KÄÄNT",
		description: "Palauttaa käänteisen gamma-jakauman kertymäfunktion: jos p = GAMMAJAKAUMA(x,...), niin GAMMAJAKAUMA.KÄÄNT(p,...) = x.",
		arguments: [
			{
				name: "todennäköisyys",
				description: "on gamma-jakaumaan liittyvää todennäköisyyttä osoittava luku. Luku on välillä 0 ja 1 (päätepisteet mukaan lukien)"
			},
			{
				name: "alfa",
				description: "on jakauman parametri. Parametrin on oltava positiivinen luku"
			},
			{
				name: "beeta",
				description: "on jakauman parametria osoittava positiivinen luku. Jos beeta = 1, funktio palauttaa käänteisen vakiomuotoisen gamma-jakauman"
			}
		]
	},
	{
		name: "GAMMALN",
		description: "Palauttaa gamma-funktion luonnollisen logaritmin.",
		arguments: [
			{
				name: "x",
				description: "on funktion GAMMALN argumentti. Arvon täytyy olla positiivinen"
			}
		]
	},
	{
		name: "GAMMALN.TARKKA",
		description: "Palauttaa gamma-funktion luonnollisen logaritmin.",
		arguments: [
			{
				name: "x",
				description: "on arvo, jolle haluat laskea kohteen GAMMALN.TARKKA, positiivisen luvun"
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
		name: "HAKU",
		description: "Etsii arvoja yhden rivin tai sarakkeen kokoisesta alueesta tai matriisista.",
		arguments: [
			{
				name: "hakuarvo",
				description: "on arvo, jota HAKU-funktio etsii ensimmäisestä vektorista. Arvo voi olla luku, merkkijono, totuusarvo, nimi tai viittaus"
			},
			{
				name: "hakuvektori",
				description: "on alue, jossa on vain yksi rivi tai sarake tekstiä, lukuja tai loogisia arvoja, jotka ovat nousevassa järjestyksessä"
			},
			{
				name: "tulosvektori",
				description: "on vain yhden rivin tai sarakkeen sisältävä alue"
			}
		]
	},
	{
		name: "HEKSABIN",
		description: "Muuntaa heksadesimaaliluvun binaariluvuksi.",
		arguments: [
			{
				name: "luku",
				description: "on heksadesimaaliluku, jonka haluat muuntaa"
			},
			{
				name: "merkit",
				description: "on käytettävien merkkien lukumäärä"
			}
		]
	},
	{
		name: "HEKSADES",
		description: "Muuntaa heksadesimaaliluvun desimaaliluvuksi.",
		arguments: [
			{
				name: "luku",
				description: "on heksadesimaaliluku, jonka haluat muuntaa"
			}
		]
	},
	{
		name: "HEKSAOKT",
		description: "Muuntaa heksadesimaaliluvun oktaaliluvuksi.",
		arguments: [
			{
				name: "luku",
				description: "on heksadesimaaliluku, jonka haluat muuntaa"
			},
			{
				name: "merkit",
				description: "on käytettävien merkkien lukumäärä"
			}
		]
	},
	{
		name: "HINTA.DISK",
		description: "Palauttaa arvopaperin hinnan (100 euron nimellisarvo).",
		arguments: [
			{
				name: "tilityspvm",
				description: "on arvopaperin tilityspäivämäärä järjestysnumerona"
			},
			{
				name: "erääntymispvm",
				description: "on arvopaperin erääntymispäivämäärä järjestysnumerona"
			},
			{
				name: "diskonttokorko",
				description: "on arvopaperin diskonttokorko"
			},
			{
				name: "lunastushinta",
				description: "on arvopaperin lunastushinta (100 euron nimellisarvo)"
			},
			{
				name: "peruste",
				description: "on käytettävä päivien laskentaperuste"
			}
		]
	},
	{
		name: "HYPERGEOM.JAKAUMA",
		description: "Palauttaa hypergeometrisen jakauman.",
		arguments: [
			{
				name: "otos_tot",
				description: "on onnistumisten määrä otoksessa"
			},
			{
				name: "luku_otos",
				description: "on otoksen koko"
			},
			{
				name: "populaatio_tot",
				description: "on onnistumisten määrä populaatiossa"
			},
			{
				name: "populaatio_luku",
				description: "on populaation koko"
			}
		]
	},
	{
		name: "HYPERGEOM_JAKAUMA",
		description: "Palauttaa hypergeometrisen jakauman.",
		arguments: [
			{
				name: "otos_tot",
				description: "on onnistumisten määrä otoksessa"
			},
			{
				name: "luku_otos",
				description: "on otoksen koko"
			},
			{
				name: "populaatio_tot",
				description: "on onnistumisten määrä populaatiossa"
			},
			{
				name: "populaatio_luku",
				description: "on populaation koko"
			},
			{
				name: "kertymä",
				description: "on looginen arvo. Jos arvo on TOSI, jakaumalle lasketaan kertymäfunktio. Jos arvo on EPÄTOSI, jakaumalle lasketaan todennäköisyystiheysfunktio"
			}
		]
	},
	{
		name: "HYPERLINKKI",
		description: "Luo linkin, joka avaa kiintolevyllä, verkkopalvelimella tai Internetissä olevan asiakirjan.",
		arguments: [
			{
				name: "Linkin_kuvaus",
				description: "on teksti, joka määrittää linkin kautta avattavan tiedoston tiedostonimen ja sijainnin kiintolevyllä, UNC-osoitteen tai URL-polun"
			},
			{
				name: "Linkin_tiedot",
				description: "on numero tai teksti, joka tulee näkyviin linkin solussa. Jos tämä jätetään pois, solussa näkyy linkin_sijainti-argumentin teksti"
			}
		]
	},
	{
		name: "IMCOSH",
		description: "Palauttaa kompleksiluvun hyperbolisen kosinin.",
		arguments: [
			{
				name: "luku",
				description: "on kompleksiluku, jolle haluat laskea hyperbolisen kosinin"
			}
		]
	},
	{
		name: "INDEKSI",
		description: "Palauttaa solun arvon tai viittauksen tietyn alueen rivin ja sarakkeen yhtymäkohdasta.",
		arguments: [
			{
				name: "matriisi",
				description: "on solualue tai matriisivakio."
			},
			{
				name: "rivi_nro",
				description: "valitsee matriisista tai viittauksesta rivin, josta funktio palauttaa arvon. Jos argumenttia ei määritetä, tarvitaan argumentti sarake_nro"
			},
			{
				name: "sarake_nro",
				description: "valitsee matriisista tai viittauksesta sarakkeen, josta funktio palauttaa arvon. Jos argumenttia ei määritetä, argumentille rivi_nro täytyy määrittää arvo"
			}
		]
	},
	{
		name: "IPMT",
		description: "Palauttaa sijoitukselle tiettynä ajanjaksona kertyvän koron, joka pohjautuu säännöllisiin vakioeriin ja kiinteään korkoprosenttiin.",
		arguments: [
			{
				name: "korko",
				description: "on kauden korkokanta. Käytä esimerkiksi neljännesvuosittaisina maksukausina arvoa 6 %/4, kun vuosikorko on 6 %"
			},
			{
				name: "kausi",
				description: "on kausi, jolta haluat tietää koron. Argumentin arvon on oltava välillä 1 - kaudet_yht"
			},
			{
				name: "kaudet_yht",
				description: "on sijoituksen maksukausien kokonaismäärä"
			},
			{
				name: "nykyarvo",
				description: "on nykyarvo eli tulevien maksujen yhteisarvo tällä hetkellä"
			},
			{
				name: "ta",
				description: "on tuleva arvo eli arvo, jonka haluat saavuttaa, kun viimeinen maksuerä on hoidettu. Jos argumenttia ei määritetä, ta = 0"
			},
			{
				name: "laji",
				description: "on totuusarvo, joka osoittaa, milloin maksuerät erääntyvät. Jos arvo on = 0 tai jätetty pois, maksut erääntyvät kauden lopussa. Jos arvo = 1, maksut erääntyvät kauden alussa"
			}
		]
	},
	{
		name: "ISO.PYÖRISTÄ.KERR.YLÖS",
		description: "Pyöristää luvun ylöspäin lähimpään kokonaislukuun tai tarkkuuden kerrannaiseen.",
		arguments: [
			{
				name: "luku",
				description: "on pyöristettävä luku"
			},
			{
				name: "tarkkuus",
				description: "on valinnainen kerrannainen, johon haluat pyöristää luvun"
			}
		]
	},
	{
		name: "ISOT",
		description: "Muuntaa tekstin isoin kirjaimin kirjoitetuksi.",
		arguments: [
			{
				name: "teksti",
				description: "on isoin kirjaimin kirjoitetuksi muutettava teksti (joko teksti- tai viittaus merkkijonoon)"
			}
		]
	},
	{
		name: "ISPMT",
		description: "Palauttaa tietyn sijoituskauden lainanmaksukoron.",
		arguments: [
			{
				name: "korko",
				description: "kauden korko. Käytä esimerkiksi neljännesvuosittaisina maksukausina arvoa 6 %/4, kun vuosikorko on 6 %"
			},
			{
				name: "kausi",
				description: "Kausi, jolle etsit korkoa"
			},
			{
				name: "kaudet_yht",
				description: "Maksukausien kokonaismäärä"
			},
			{
				name: "nykyarvo",
				description: "Summa, joka vastaa maksettavien erien arvoa nyt"
			}
		]
	},
	{
		name: "ITSEISARVO",
		description: "Palauttaa luvun itseisarvon eli luvun ilman etumerkkiä.",
		arguments: [
			{
				name: "luku",
				description: "on reaaliluku, jonka itseisarvon haluat laskea."
			}
		]
	},
	{
		name: "JA",
		description: "Tarkistaa, onko kaikkien argumenttien totuusarvo TOSI, ja palauttaa totuusarvon TOSI, jos näin on.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "totuus1",
				description: "ovat 1 - 255 ehtoa, jotka voivat testattaessa saada arvon TOSI tai EPÄTOSI. Ehdot voivat olla loogisia arvoja, matriiseja tai viittauksia"
			},
			{
				name: "totuus2",
				description: "ovat 1 - 255 ehtoa, jotka voivat testattaessa saada arvon TOSI tai EPÄTOSI. Ehdot voivat olla loogisia arvoja, matriiseja tai viittauksia"
			}
		]
	},
	{
		name: "JAKAUMAN.VINOUS",
		description: "Palauttaa jakauman vinouden. Arvo kuvaa jakauman keskittymistä keskiarvon ympärille.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 lukua, nimeä, matriisia tai viittausta lukuihin, joista haluat laskea vinouden"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 lukua, nimeä, matriisia tai viittausta lukuihin, joista haluat laskea vinouden"
			}
		]
	},
	{
		name: "JAKAUMAN.VINOUS.POP",
		description: "Palauttaa jakauman vinouden populaatioon perustuen: arvo kuvaa jakauman keskittymistä keskiarvon ympärille.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 254 lukua, nimeä, matriisia tai viittausta lukuihin, joista haluat laskea populaation vinouden"
			},
			{
				name: "luku2",
				description: "ovat 1 - 254 lukua, nimeä, matriisia tai viittausta lukuihin, joista haluat laskea populaation vinouden"
			}
		]
	},
	{
		name: "JAKOJ",
		description: "Palauttaa jakojäännöksen.",
		arguments: [
			{
				name: "luku",
				description: "on luku, jonka jakojäännöksen haluat laskea"
			},
			{
				name: "jakaja",
				description: "on arvo, jolla haluat jakaa luvun"
			}
		]
	},
	{
		name: "JOS",
		description: "Tarkistaa, täyttyykö määrittämäsi ehto. Palauttaa yhden arvon, jos ehto on TOSI ja toisen arvon, jos ehto on EPÄTOSI.",
		arguments: [
			{
				name: "totuus_testi",
				description: "on arvon tosi tai epätosi palauttava arvo tai lauseke"
			},
			{
				name: "arvo_jos_tosi",
				description: "on palautettava arvo, jos totuus_testi on TOSI. Jos arvoa ei määritetä, funktio palauttaa arvon TOSI. Voit määrittää 7 sisäkkäistä JOS-funktiota"
			},
			{
				name: "arvo_jos_epätosi",
				description: "on palautettava arvo, jos totuus_testi on EPÄTOSI. Jos arvoa ei määritetä, funktio palauttaa arvon EPÄTOSI"
			}
		]
	},
	{
		name: "JOSPUUTTUU",
		description: "Palauttaa määritetyn arvon, jos lauseke antaa ratkaisuksi #Ei mitään, muutoin se palauttaa lausekkeen tuloksen.",
		arguments: [
			{
				name: "arvo",
				description: "on mikä tahansa arvo, lauseke tai viittaus"
			},
			{
				name: "arvo_jos_ei_mitään",
				description: "on mikä tahansa arvo, lauseke tai viittaus"
			}
		]
	},
	{
		name: "JOSVIRHE",
		description: "Palauttaa arvo_jos_virheen, jos lauseke on virhe ja lausekkeen arvo jokin muu.",
		arguments: [
			{
				name: "arvo",
				description: "on mikä tahansa arvo tai lauseke tai viittaus"
			},
			{
				name: "arvo_jos_virhe",
				description: "on mikä tahansa arvo tai lauseke tai viittaus"
			}
		]
	},
	{
		name: "KAAVA.TEKSTI",
		description: "Palauttaa kaavan merkkijonona.",
		arguments: [
			{
				name: "viittaus",
				description: "on viittaus kaavaan"
			}
		]
	},
	{
		name: "KASVU",
		description: "Palauttaa eksponentiaalisen kasvutrendin luvut, jotka vastaavat tunnettuja tietopisteitä.",
		arguments: [
			{
				name: "tunnettu_y",
				description: "on joukko y-arvoja, jotka tunnetaan yhtälöstä y = b*m^x. Arvot ovat matriisi tai joukko positiivisia lukuja"
			},
			{
				name: "tunnettu_x",
				description: "on valinnainen joukko x-arvoja, jotka tunnetaan yhtälöstä y = b*m^x. Arvot ovat samankokoinen matriisi tai arvojoukko kuin tunnetut y-arvot"
			},
			{
				name: "uusi_x",
				description: "ovat uudet x:n arvot, joille KASVU-funktion halutaan palauttavan vastaavat y:n arvot"
			},
			{
				name: "vakio",
				description: "on totuusarvo, joka määrittää, mikä tulee b:n arvoksi. Jos arvo on TOSI, vakio b lasketaan normaalisti. Jos arvo puuttuu tai on EPÄTOSI, b:n arvoksi tulee 1"
			}
		]
	},
	{
		name: "KATKAISE",
		description: "Katkaisee luvun kokonaisluvuksi poistamalla desimaali- tai murto-osan.",
		arguments: [
			{
				name: "luku",
				description: "on katkaistava luku"
			},
			{
				name: "numerot",
				description: "on katkaisutarkkuuden määrittävä luku. Jos arvoa ei määritetä, ohjelma käyttää arvoa 0 (nolla)"
			}
		]
	},
	{
		name: "KÄY.LÄPI",
		description: "Palauttaa sen merkin numeron, jossa etsittävä merkki tai merkkijono esiintyy ensimmäisen kerran. Merkkiä tai merkkijonoa etsitään vasemmalta oikealle, eikä kirjainkokoa oteta huomioon.",
		arguments: [
			{
				name: "etsittävä_teksti",
				description: "on etsittävä teksti. Voit käyttää yleismerkkejä ? ja *. Jos haluat etsiä merkkejä ? tai *, käytä ~? tai ~* merkkejä"
			},
			{
				name: "tekstissä",
				description: "on teksti, josta haluat etsiä etsittävä_teksti argumentin arvoa"
			},
			{
				name: "aloitusnro",
				description: "on sen argumentin tekstissä määrittämän merkin järjestysnumero (vasemmalta oikealle), josta haluat aloittaa etsinnän. Jos arvoa ei määritetä, ohjelma käyttää arvoa 1"
			}
		]
	},
	{
		name: "KERTOMA",
		description: "Palauttaa luvun kertoman, eli 1*2*3*...*luku.",
		arguments: [
			{
				name: "luku",
				description: "on positiivinen luku, josta haluat laskea kertoman"
			}
		]
	},
	{
		name: "KERTOMA.OSA",
		description: "Palauttaa luvun osakertoman.",
		arguments: [
			{
				name: "luku",
				description: "on arvo, jolle haluat laskea osakertoman"
			}
		]
	},
	{
		name: "KERTYNYT.KORKO.LOPUSSA",
		description: "Palauttaa eräpäivänä korkoa maksavalle arvopaperille kertyneen koron.",
		arguments: [
			{
				name: "asettamispvm",
				description: "on arvopaperin liikkeellelaskupäivämäärä järjestysnumerona"
			},
			{
				name: "tilityspvm",
				description: "on arvopaperin erääntymispäivämäärä järjestysnumerona"
			},
			{
				name: "korko",
				description: "on arvopaperin vuosittainen korkokanta"
			},
			{
				name: "nimellisarvo",
				description: "on arvopaperin nimellisarvo"
			},
			{
				name: "peruste",
				description: "on käytettävä päivien laskentaperuste"
			}
		]
	},
	{
		name: "KESKIARVO",
		description: "Palauttaa argumenttien aritmeettisen keskiarvon. Argumentit voivat olla lukuja, nimiä, matriiseja tai viittauksia, jotka kohdistuvat lukuihin.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 numeerista argumenttia, joista haluat laskea aritmeettisen keskiarvon"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 numeerista argumenttia, joista haluat laskea aritmeettisen keskiarvon"
			}
		]
	},
	{
		name: "KESKIARVO.GEOM",
		description: "Palauttaa matriisin tai positiivisten lukuarvojen geometrisen keskiarvon.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 lukua, nimeä, matriisia tai viittausta lukuihin, joille haluat laskea geometrisen keskiarvon"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 lukua, nimeä, matriisia tai viittausta lukuihin, joille haluat laskea geometrisen keskiarvon"
			}
		]
	},
	{
		name: "KESKIARVO.HARM",
		description: "Palauttaa harmonisen keskiarvon positiivisesta lukujoukosta.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 lukua, nimeä, matriisia tai viittausta lukuihin, joille haluat laskea harmonisen keskiarvon"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 lukua, nimeä, matriisia tai viittausta lukuihin, joille haluat laskea harmonisen keskiarvon"
			}
		]
	},
	{
		name: "KESKIARVO.JOS",
		description: "Määrittää tiettyjen ehtojen määrittämille soluille aritmeettisen keskiarvon.",
		arguments: [
			{
				name: "alue",
				description: "on solualue, jonka haluat laskea"
			},
			{
				name: "ehdot",
				description: "on ehto, joka voi olla keskiarvon laskemisessa käytettävät solut määrittävä luku, lauseke tai teksti"
			},
			{
				name: "keskiarvoalue",
				description: "ovat todelliset solut, joiden avulla keskiarvo lasketaan. Jos arvo puuttuu, käytetään alueen soluja "
			}
		]
	},
	{
		name: "KESKIARVO.JOS.JOUKKO",
		description: "Määrittää tiettyjen ehtojen määrittämille soluille aritmeettisen keskiarvon.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "keskiarvoalue",
				description: "ovat todelliset solut, joiden avulla keskiarvo lasketaan."
			},
			{
				name: "ehtoalue",
				description: "ion solualue, jonka haluat laskea tietyllä ehdolla"
			},
			{
				name: "ehdot",
				description: "on ehto, joka voi olla keskiarvon laskemiseen käytettävät solut määrittävä luku, lauseke tai teksti"
			}
		]
	},
	{
		name: "KESKIARVO.TASATTU",
		description: "Palauttaa tasatun keskiarvon.",
		arguments: [
			{
				name: "matriisi",
				description: "on matriisi tai arvoalue, jonka haluat tasata ja jonka keskiarvon haluat sitten laskea"
			},
			{
				name: "prosentti",
				description: "on murtoluku, joka määrittää, kuinka suuri osa arvopisteistä jätetään laskennan ulkopuolelle arvojoukon alusta ja lopusta"
			}
		]
	},
	{
		name: "KESKIARVOA",
		description: "Palauttaa argumenttien (aritmeettisen) keskiarvon. Argumentin teksti ja arvo EPÄTOSI lasketaan arvona 0. Arvo TOSI lasketaan arvona 1. Argumentit voivat olla lukuja, nimiä, matriiseja tai viittauksia.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arvo1",
				description: "ovat 1 - 255 arvoa, joista keskiarvo lasketaan"
			},
			{
				name: "arvo2",
				description: "ovat 1 - 255 arvoa, joista keskiarvo lasketaan"
			}
		]
	},
	{
		name: "KESKIHAJONTA",
		description: "Arvioi populaation keskihajonnan otoksen perusteella (funktio ei huomioi näytteessä olevia totuusarvoja tai tekstiä).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 lukua, jotka muodostavat näytteen populaatiosta. Arvot voivat olla lukuja tai viittauksia lukuihin"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 lukua, jotka muodostavat näytteen populaatiosta. Arvot voivat olla lukuja tai viittauksia lukuihin"
			}
		]
	},
	{
		name: "KESKIHAJONTA.P",
		description: "Laskee populaation keskihajonnan koko argumentteina annetun populaation perusteella (funktio ei huomioi totuusarvoja tai tekstiä).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 lukua, jotka vastaavat populaatiota. Arvot voivat olla lukuja tai viittauksia lukuihin"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 lukua, jotka vastaavat populaatiota. Arvot voivat olla lukuja tai viittauksia lukuihin"
			}
		]
	},
	{
		name: "KESKIHAJONTA.S",
		description: "Arvioi populaation keskihajonnan otoksen perusteella (funktio ei huomioi näytteessä olevia totuusarvoja tai tekstiä).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 lukua, jotka muodostavat näytteen populaatiosta. Arvot voivat olla lukuja tai viittauksia lukuihin"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 lukua, jotka muodostavat näytteen populaatiosta. Arvot voivat olla lukuja tai viittauksia lukuihin"
			}
		]
	},
	{
		name: "KESKIHAJONTAA",
		description: "Arvioi keskipoikkeamaa näytteen pohjalta. Funktio huomioi myös totuusarvot ja tekstin. Teksti ja totuusarvo EPÄTOSI lasketaan arvona 0. Totuusarvo TOSI lasketaan arvona 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arvo1",
				description: "ovat 1 - 255 populaation näytettä. Argumentit voivat olla lukuja, nimiä tai viittauksia lukuihin"
			},
			{
				name: "arvo2",
				description: "ovat 1 - 255 populaation näytettä. Argumentit voivat olla lukuja, nimiä tai viittauksia lukuihin"
			}
		]
	},
	{
		name: "KESKIHAJONTAP",
		description: "Laskee populaation keskihajonnan koko populaation perusteella (funktio ei huomioi totuusarvoja tai tekstiä).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 lukua, jotka vastaavat populaatiota. Arvot voivat olla lukuja tai viittauksia lukuihin"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 lukua, jotka vastaavat populaatiota. Arvot voivat olla lukuja tai viittauksia lukuihin"
			}
		]
	},
	{
		name: "KESKIHAJONTAPA",
		description: "Laskee koko populaation keskipoikkeaman. Funktio ottaa myös huomioon totuusarvot ja tekstin. Teksti ja totuusarvo EPÄTOSI lasketaan arvona 0. Totuusarvo TOSI lasketaan arvona 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arvo1",
				description: "ovat 1 - 255  populaatioon liittyvää arvoa. Argumentit voivat olla lukuja, nimiä, matriiseja tai viittauksia lukuihin"
			},
			{
				name: "arvo2",
				description: "ovat 1 - 255  populaatioon liittyvää arvoa. Argumentit voivat olla lukuja, nimiä, matriiseja tai viittauksia lukuihin"
			}
		]
	},
	{
		name: "KESKIPOIKKEAMA",
		description: "Palauttaa hajontojen itseisarvojen keskiarvon. Argumentit voivat olla lukuja, nimiä, matriiseja tai viittauksia lukuihin.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 argumenttia, joista haluat laskea keskipoikkeamien keskiarvon"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 argumenttia, joista haluat laskea keskipoikkeamien keskiarvon"
			}
		]
	},
	{
		name: "KESKIVIRHE",
		description: "Palauttaa jokaista x-arvoa vastaavan ennustetun y-arvon keskivirheen.",
		arguments: [
			{
				name: "tunnettu_y",
				description: "on riippuva matriisi tai arvopisteiden alue. Arvot voivat olla nimiä, matriiseja tai viittauksia lukuihin"
			},
			{
				name: "tunnettu_x",
				description: "on riippumaton matriisi tai arvopisteiden alue. Arvot voivat olla nimiä, matriiseja tai viittauksia lukuihin"
			}
		]
	},
	{
		name: "KESTO.JAKSO",
		description: "Palauttaa sijoituksessa tarvittavien jaksojen määrän, jotta määritetty arvo voidaan saavuttaa.",
		arguments: [
			{
				name: "korko",
				description: "on kauden korkokanta."
			},
			{
				name: "nykyarvo",
				description: "on sijoituksen nykyarvo"
			},
			{
				name: "tuleva_arvo",
				description: "on haluttu sijoituksen tuleva arvo"
			}
		]
	},
	{
		name: "KETJUTA",
		description: "Yhdistää erilliset merkkijonot yhdeksi merkkijonoksi.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "teksti1",
				description: "ovat 1 - 255 yhteenliitettävää merkkijonoa. Argumentit voivat olla merkkijonoja, lukuja tai viittauksia yksittäisiin soluihin"
			},
			{
				name: "teksti2",
				description: "ovat 1 - 255 yhteenliitettävää merkkijonoa. Argumentit voivat olla merkkijonoja, lukuja tai viittauksia yksittäisiin soluihin"
			}
		]
	},
	{
		name: "KIINTEÄ",
		description: "Muotoilee luvun tekstiksi, jossa on kiinteä määrä desimaaleja ja palauttaa tuloksen tekstinä.",
		arguments: [
			{
				name: "luku",
				description: "on luku, jonka haluat pyöristää ja muuntaa tekstiksi"
			},
			{
				name: "desimaalit",
				description: "on desimaalipilkun oikealla puolella olevien numeroiden määrä. Jos arvoa ei ole määritetty, desimaalit = 2"
			},
			{
				name: "ei_erotinta",
				description: "on totuusarvo, joka määrittää, näytetäänkö erottimet luvussa. Jos arvo on TOSI, erottimia ei näytetä palautettavassa tekstissä. Jos arvo puuttuu tai on EPÄTOSI, erottimet näytetään"
			}
		]
	},
	{
		name: "KOKONAISLUKU",
		description: "Pyöristää luvun alaspäin lähimpään kokonaislukuun.",
		arguments: [
			{
				name: "luku",
				description: "on reaaliluku, jonka haluat pyöristää alaspäin kokonaisluvuksi"
			}
		]
	},
	{
		name: "KOMBINAATIO",
		description: "Palauttaa annettujen objektien kombinaatioiden määrän.",
		arguments: [
			{
				name: "luku",
				description: "on objektien määrä"
			},
			{
				name: "valittu_luku",
				description: "on objektien määrä jokaisessa yhdistelmässä"
			}
		]
	},
	{
		name: "KOMBINAATIOA",
		description: "Palauttaa yhdistelmien määrän toistoineen tietylle määrälle kohteita.",
		arguments: [
			{
				name: "luku",
				description: "on kohteiden kokonaismäärä"
			},
			{
				name: "valittu_luku",
				description: "on kohteiden määrä jokaisessa yhdistelmässä"
			}
		]
	},
	{
		name: "KOMPLEKSI",
		description: "Muuntaa reaali- ja imaginaarikertoimet kompleksiluvuksi.",
		arguments: [
			{
				name: "reaaliosa",
				description: "on kompleksiluvun reaalikerroin"
			},
			{
				name: "imag_osa",
				description: "on kompleksiluvun imaginaarikerroin"
			},
			{
				name: "suffiksi",
				description: "on kompleksiluvun imaginaariosan loppuliite"
			}
		]
	},
	{
		name: "KOMPLEKSI.ABS",
		description: "Palauttaa kompleksiluvun itseisarvon.",
		arguments: [
			{
				name: "kompleksiluku",
				description: "on kompleksiluku, jolle haluat laskea itseisarvon"
			}
		]
	},
	{
		name: "KOMPLEKSI.ARG",
		description: "Palauttaa argumentin q, joka on kulma radiaaneina.",
		arguments: [
			{
				name: "kompleksiluku",
				description: "on kompleksiluku, jolle haluat laskea argumentin theeta"
			}
		]
	},
	{
		name: "KOMPLEKSI.COS",
		description: "Palauttaa kompleksiluvun kosinin.",
		arguments: [
			{
				name: "kompleksiluku",
				description: "on kompleksiluku, jolle haluat laskea kosinin"
			}
		]
	},
	{
		name: "KOMPLEKSI.COT",
		description: "Palauttaa kompleksiluvun kotangentin.",
		arguments: [
			{
				name: "luku",
				description: "on kompleksiluku, jolle haluat laskea kotangentin"
			}
		]
	},
	{
		name: "KOMPLEKSI.EKSP",
		description: "Palauttaa kompleksiluvun eksponentin.",
		arguments: [
			{
				name: "kompleksiluku",
				description: "on kompleksiluku, jolle haluat laskea eksponentin"
			}
		]
	},
	{
		name: "KOMPLEKSI.EROTUS",
		description: "Palauttaa kahden kompleksiluvun erotuksen.",
		arguments: [
			{
				name: "kompleksiluku1",
				description: "on kompleksiluku, josta haluat vähentää kompleksiluku2:n"
			},
			{
				name: "kompleksiluku2",
				description: "on kompleksiluku, jonka haluat vähentää kompleksi1:stä"
			}
		]
	},
	{
		name: "KOMPLEKSI.IMAG",
		description: "Palauttaa kompleksiluvun imaginaariosan kertoimen.",
		arguments: [
			{
				name: "kompleksiluku",
				description: "on kompleksiluku, jolle haluat laskea imaginaarikertoimen"
			}
		]
	},
	{
		name: "KOMPLEKSI.KONJ",
		description: "Palauttaa kompleksiluvun konjugaattiluvun.",
		arguments: [
			{
				name: "kompleksiluku",
				description: "on kompleksiluku, jolle haluat laskea konjugaattiluvun"
			}
		]
	},
	{
		name: "KOMPLEKSI.KOSEK",
		description: "Palauttaa kompleksiluvun kosekantin.",
		arguments: [
			{
				name: "luku",
				description: "on kompleksiluku, jolle haluat laskea kosekantin"
			}
		]
	},
	{
		name: "KOMPLEKSI.KOSEKH",
		description: "Palauttaa kompleksiluvun hyperbolisen kosekantin.",
		arguments: [
			{
				name: "luku",
				description: "on kompleksiluku, jolle haluat laskea hyperbolisen kosekantin"
			}
		]
	},
	{
		name: "KOMPLEKSI.LN",
		description: "Palauttaa kompleksiluvun luonnollisen logaritmin.",
		arguments: [
			{
				name: "kompleksiluku",
				description: "on kompleksiluku, jolle haluat laskea luonnollisen logaritmin"
			}
		]
	},
	{
		name: "KOMPLEKSI.LOG10",
		description: "Palauttaa kompleksiluvun kymmenkantaisen logaritmin.",
		arguments: [
			{
				name: "kompleksiluku",
				description: "on kompleksiluku, jolle haluat laskea kymmenkantaisen logaritmin"
			}
		]
	},
	{
		name: "KOMPLEKSI.LOG2",
		description: "Palauttaa kompleksiluvun kaksikantaisen logaritmin.",
		arguments: [
			{
				name: "kompleksiluku",
				description: "on kompleksiluku, jolle haluat laskea kaksikantaisen logaritmin"
			}
		]
	},
	{
		name: "KOMPLEKSI.NELIÖJ",
		description: "Palauttaa kompleksiluvun neliöjuuren.",
		arguments: [
			{
				name: "kompleksiluku",
				description: "on kompleksiluku, jolle haluat laskea neliöjuuren"
			}
		]
	},
	{
		name: "KOMPLEKSI.OSAM",
		description: "Palauttaa kahden kompleksiluvun osamäärän.",
		arguments: [
			{
				name: "kompleksiluku1",
				description: "on jaettava kompleksiluku"
			},
			{
				name: "kompleksiluku2",
				description: "on jakajana oleva kompleksiluku"
			}
		]
	},
	{
		name: "KOMPLEKSI.POT",
		description: "Palauttaa kompleksiluvun korotettuna kokonaislukupotenssiin.",
		arguments: [
			{
				name: "kompleksiluku",
				description: "on kompleksiluku, jonka haluat korottaa potenssiin"
			},
			{
				name: "luku",
				description: "on potenssi, johon haluat korottaa kompleksiluvun"
			}
		]
	},
	{
		name: "KOMPLEKSI.REAALI",
		description: "Palauttaa kompleksiluvun reaaliosan kertoimen.",
		arguments: [
			{
				name: "kompleksiluku",
				description: "on kompleksiluku, josta haluat laskea reaalikertoimen"
			}
		]
	},
	{
		name: "KOMPLEKSI.SEK",
		description: "Palauttaa kompleksiluvun sekantin.",
		arguments: [
			{
				name: "luku",
				description: "on kompleksiluku, jolle haluat laskea sekantin"
			}
		]
	},
	{
		name: "KOMPLEKSI.SEKH",
		description: "Palauttaa kompleksiluvun hyperbolisen sekantin.",
		arguments: [
			{
				name: "luku",
				description: "on kompleksiluku, jolle haluat laskea hyperbolisen sekantin"
			}
		]
	},
	{
		name: "KOMPLEKSI.SIN",
		description: "Palauttaa kompleksiluvun sinin.",
		arguments: [
			{
				name: "kompleksiluku",
				description: "on kompleksiluku, jolle haluat laskea sinin"
			}
		]
	},
	{
		name: "KOMPLEKSI.SINH",
		description: "Palauttaa kompleksiluvun hyperbolisen sinin.",
		arguments: [
			{
				name: "luku",
				description: "on kompleksiluku, jolle haluat laskea hyperbolisen sinin"
			}
		]
	},
	{
		name: "KOMPLEKSI.SUM",
		description: "Palauttaa kompleksilukujen summan.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "iluku1",
				description: "ovat 1 - 255 kompleksilukua, jotka haluat lisätä"
			},
			{
				name: "iluku2",
				description: "ovat 1 - 255 kompleksilukua, jotka haluat lisätä"
			}
		]
	},
	{
		name: "KOMPLEKSI.TAN",
		description: "Palauttaa kompleksiluvun tangentin.",
		arguments: [
			{
				name: "iluku",
				description: "on kompleksiluku, jolle haluat laskea tangentin"
			}
		]
	},
	{
		name: "KOMPLEKSI.TULO",
		description: "Palauttaa 1 - 255 kompleksiluvun tulon.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "iluku1",
				description: "Iluku1, Iluku2,... ovat 1 - 255 kompleksilukua, jotka haluat kertoa."
			},
			{
				name: "iluku2",
				description: "Iluku1, Iluku2,... ovat 1 - 255 kompleksilukua, jotka haluat kertoa."
			}
		]
	},
	{
		name: "KOODI",
		description: "Palauttaa tekstijonon ensimmäisen merkin numerokoodin tietokoneen käyttämässä merkistössä.",
		arguments: [
			{
				name: "teksti",
				description: "on teksti, jonka ensimmäisen merkin koodin haluat saada selville"
			}
		]
	},
	{
		name: "KORKO",
		description: "Palauttaa sijoituksen tai lainan kausittaisen korkokannan. Käytä esimerkiksi neljännesvuosittaisina maksukausina arvoa 6 %/4, kun vuosikorko on 6 %.",
		arguments: [
			{
				name: "kaudet_yht",
				description: "on sijoituksen tai lainan maksukausien kokonaismäärä"
			},
			{
				name: "erä",
				description: "on kunkin kauden maksuerä, joka ei voi muuttua sijoitus- tai lainakauden aikana"
			},
			{
				name: "nykyarvo",
				description: "on nykyarvo eli tulevien maksujen yhteisarvo tällä hetkellä"
			},
			{
				name: "ta",
				description: "on tuleva arvo eli arvo, jonka haluat saavuttaa, kun viimeinen maksuerä on hoidettu. Jos arvoa ei määritetä, ta = 0"
			},
			{
				name: "laji",
				description: "on luku 0 tai 1, joka osoittaa, milloin maksuerät erääntyvät. Jos maksut erääntyvät kauden alussa, arvo = 1. Jos maksut erääntyvät kauden lopussa, arvo = 0 tai jätetty pois"
			},
			{
				name: "arvaus",
				description: "on arvioimasi korkokanta. Jos arvoa ei määritetä, ohjelma käyttää arvoa 0,1 (10 prosenttia)"
			}
		]
	},
	{
		name: "KORKO.ARVOPAPERI",
		description: "Palauttaa arvopaperin korkokannan.",
		arguments: [
			{
				name: "tilityspvm",
				description: "on arvopaperin tilityspäivämäärä järjestysnumerona"
			},
			{
				name: "erääntymispvm",
				description: "on arvopaperin erääntymispäivämäärä järjestysnumerona"
			},
			{
				name: "sijoitus",
				description: "on arvopaperiin sijoitettu rahamäärä"
			},
			{
				name: "lunastushinta",
				description: "on erääntymispäivänä saatava rahamäärä"
			},
			{
				name: "peruste",
				description: "on käytettävä päivien laskentaperuste"
			}
		]
	},
	{
		name: "KORKO.EFEKT",
		description: "Palauttaa efektiivisen vuosikorkokannan.",
		arguments: [
			{
				name: "nimelliskorko",
				description: "on nimelliskorkokanta"
			},
			{
				name: "korkojaksot",
				description: "on korkojaksojen lukumäärä vuodessa"
			}
		]
	},
	{
		name: "KORKO.VUOSI",
		description: "Palauttaa vuosittaisen nimelliskoron.",
		arguments: [
			{
				name: "tod_korko",
				description: "on efektiivinen korkokanta"
			},
			{
				name: "korkojaksot",
				description: "on korkojaksojen lukumäärä vuodessa"
			}
		]
	},
	{
		name: "KORKOPÄIVÄ.EDELLINEN",
		description: "Palauttaa tilityspäivää edeltävän korkopäivän.",
		arguments: [
			{
				name: "tilityspvm",
				description: "on arvopaperin tilityspäivämäärä järjestysnumerona"
			},
			{
				name: "erääntymispvm",
				description: "on arvopaperin erääntymispäivämäärä järjestysnumerona"
			},
			{
				name: "korkojakso",
				description: "on koronmaksukausien lukumäärä vuodessa"
			},
			{
				name: "peruste",
				description: "on käytettävä päivien laskentaperuste"
			}
		]
	},
	{
		name: "KORKOPÄIVÄ.JAKSOT",
		description: "Palauttaa maksettavien korkosuoritusten lukumäärän tilitys- ja erääntymispäivämäärän välillä.",
		arguments: [
			{
				name: "tilityspvm",
				description: "on arvopaperin tilityspäivämäärä järjestysnumerona"
			},
			{
				name: "erääntymispvm",
				description: "on arvopaperin erääntymispäivämäärä järjestysnumerona"
			},
			{
				name: "korkojakso",
				description: "on koronmaksukausien lukumäärä vuodessa"
			},
			{
				name: "peruste",
				description: "on käytettävä päivien laskentaperuste"
			}
		]
	},
	{
		name: "KORKOPÄIVÄ.SEURAAVA",
		description: "Palauttaa tilityspäivämäärää seuraavan koronmaksupäivän.",
		arguments: [
			{
				name: "tilityspvm",
				description: "on arvopaperin tilityspäivämäärä järjestysnumerona"
			},
			{
				name: "erääntymispvm",
				description: "on arvopaperin erääntymispäivämäärä järjestysnumerona"
			},
			{
				name: "korkojakso",
				description: "on koronmaksukausien lukumäärä vuodessa"
			},
			{
				name: "peruste",
				description: "on käytettävä päivien laskentaperuste"
			}
		]
	},
	{
		name: "KORKOPÄIVÄT.ALUSTA",
		description: "Palauttaa koronmaksupäivien lukumäärän korkokauden alusta tilityspäivämäärään asti.",
		arguments: [
			{
				name: "tilityspvm",
				description: "on arvopaperin tilityspäivämäärä järjestysnumerona"
			},
			{
				name: "erääntymispvm",
				description: "on arvopaperin erääntymispäivämäärä järjestysnumerona"
			},
			{
				name: "korkojakso",
				description: "on koronmaksukausien lukumäärä vuodessa"
			},
			{
				name: "peruste",
				description: "on käytettävä päivien laskentaperuste"
			}
		]
	},
	{
		name: "KORRELAATIO",
		description: "Palauttaa kahden tietoalueen välisen korrelaatiokertoimen.",
		arguments: [
			{
				name: "matriisi1",
				description: "on arvojen solualue. Arvot voivat olla lukuja, nimiä, matriiseja tai viittauksia lukuihin"
			},
			{
				name: "matriisi2",
				description: "on toinen arvojen solualue. Arvot voivat olla lukuja, nimiä, matriiseja tai viittauksia lukuihin"
			}
		]
	},
	{
		name: "KORVAA",
		description: "Korvaa merkkejä tekstissä.",
		arguments: [
			{
				name: "vanha_teksti",
				description: "on teksti, josta haluat korvata tietyn määrän merkkejä"
			},
			{
				name: "aloitusnro",
				description: "on tekstin vanha_teksti merkin järjestysnumero, josta merkkien korvaus tekstin uusi_teksti merkeillä alkaa"
			},
			{
				name: "merkit_luku",
				description: "on niiden vanha_teksti merkkien määrä, jotka haluat korvata tekstillä uusi_teksti"
			},
			{
				name: "uusi_teksti",
				description: "on teksti, joka korvaa tekstin vanha_teksti"
			}
		]
	},
	{
		name: "KOSEK",
		description: "Palauttaa kulman kosekantin.",
		arguments: [
			{
				name: "luku",
				description: "on radiaaneina kulma, jolle haluat laskea kosekantin"
			}
		]
	},
	{
		name: "KOSEKH",
		description: "Palauttaa kulman hyperbolisen kosekantin.",
		arguments: [
			{
				name: "luku",
				description: "on radiaaneina kulma, jolle haluat laskea hyperbolisen kosekantin"
			}
		]
	},
	{
		name: "KOVARIANSSI",
		description: "Palauttaa kovarianssin, eli kahden arvojoukon kaikkien arvopisteparien hajontojen tulojen keskiarvon.",
		arguments: [
			{
				name: "matriisi1",
				description: "ensimmäinen kokonaislukualue. Arvojen täytyy olla lukuja, matriiseja tai viittauksia lukuihin"
			},
			{
				name: "matriisi2",
				description: "toinen kokonaislukualue. Arvojen täytyy olla lukuja, matriiseja tai viittauksia lukuihin"
			}
		]
	},
	{
		name: "KOVARIANSSI.P",
		description: "Palauttaa populaation kovarianssin, eli kahden arvojoukon kaikkien arvopisteparien hajontojen tulojen keskiarvon.",
		arguments: [
			{
				name: "matriisi1",
				description: "on ensimmäinen kokonaislukualue. Arvojen täytyy olla lukuja, matriiseja tai viittauksia lukuihin"
			},
			{
				name: "matriisi2",
				description: "on toinen kokonaislukualue. Arvojen täytyy olla lukuja, matriiseja tai viittauksia lukuihin"
			}
		]
	},
	{
		name: "KOVARIANSSI.S",
		description: "Palauttaa näytteen kovarianssin, eli kahden arvojoukon kaikkien arvopisteparien hajontojen tulojen keskiarvon.",
		arguments: [
			{
				name: "matriisi1",
				description: "on ensimmäinen kokonaislukualue. Arvojen täytyy olla lukuja, matriiseja tai viittauksia lukuihin"
			},
			{
				name: "matriisi2",
				description: "on toinen kokonaislukualue. Arvojen täytyy olla lukuja, matriiseja tai viittauksia lukuihin"
			}
		]
	},
	{
		name: "KULMAKERROIN",
		description: "Palauttaa lineaarisen regressiosuoran kulmakertoimen, joka on laskettu annettujen arvopisteiden pohjalta.",
		arguments: [
			{
				name: "tunnettu_y",
				description: "on matriisi tai solualue, jossa on numeerisia riippuvia arvopisteitä. Arvot voivat olla nimiä, matriiseja tai viittauksia lukuihin"
			},
			{
				name: "tunnettu_x",
				description: "on joukko riippumattomia arvopisteitä. Arvot voivat olla nimiä, matriiseja tai viittauksia lukuihin"
			}
		]
	},
	{
		name: "KURT",
		description: "Palauttaa tietojoukon kurtosis-arvon.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 lukua, nimeä, matriisia tai viittausta lukuihin, joista haluat laskea kurtosis-arvon"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 lukua, nimeä, matriisia tai viittausta lukuihin, joista haluat laskea kurtosis-arvon"
			}
		]
	},
	{
		name: "KUUKAUSI",
		description: "Palauttaa kuukauden järjestysnumeron 1 (tammikuu)–12 (joulukuu).",
		arguments: [
			{
				name: "järjestysnro",
				description: "on luku Spreadsheetin käyttämässä päivämäärä- ja kellonaikamuodossa"
			}
		]
	},
	{
		name: "KUUKAUSI.LOPPU",
		description: "Palauttaa kuukauden viimeisen päivän järjestysnumeron, joka on määrätyn kuukausimäärän päässä ennen tai jälkeen aloituspäivämäärästä laskien.",
		arguments: [
			{
				name: "aloituspäivä",
				description: "on aloituspäivän järjestysnumero"
			},
			{
				name: "kuukaudet",
				description: "on kuukausien lukumäärä ennen tai jälkeen aloituspäivän"
			}
		]
	},
	{
		name: "KUVAUS",
		description: "Palauttaa tietoja käytössä olevasta käyttöympäristöstä.",
		arguments: [
			{
				name: "laji_teksti",
				description: "on teksti, joka määrittää, minkätyyppisiä tietoja haluat funktion palauttavan."
			}
		]
	},
	{
		name: "LASKE",
		description: "Laskee alueen lukuja sisältävien solujen määrän.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arvo1",
				description: "ovat 1 - 255 argumenttia, jotka voivat sisältää useita erilaisia tietolajeja tai viitata niihin. Toiminto laskee kuitenkin vain luvut"
			},
			{
				name: "arvo2",
				description: "ovat 1 - 255 argumenttia, jotka voivat sisältää useita erilaisia tietolajeja tai viitata niihin. Toiminto laskee kuitenkin vain luvut"
			}
		]
	},
	{
		name: "LASKE.A",
		description: "Laskee alueen tietoja sisältävien solujen määrän.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arvo1",
				description: "ovat 1 - 255 argumenttia, jotka vastaavat laskettavia arvoja ja soluja. Argumentit voivat olla mitä tahansa tietotyyppiä"
			},
			{
				name: "arvo2",
				description: "ovat 1 - 255 argumenttia, jotka vastaavat laskettavia arvoja ja soluja. Argumentit voivat olla mitä tahansa tietotyyppiä"
			}
		]
	},
	{
		name: "LASKE.JOS",
		description: "Laskee annetun alueen solut, jotka täyttävät annetut ehdot.",
		arguments: [
			{
				name: "alue",
				description: "on alue, jolta lasketaan tietoja sisältävät solut"
			},
			{
				name: "ehdot",
				description: "on luvun, lausekkeen tai tekstin muodossa oleva ehto, joka määrittää laskettavat solut"
			}
		]
	},
	{
		name: "LASKE.JOS.JOUKKO",
		description: "Laskee määritettyjen ehtojen palauttamien solujen määrän.",
		arguments: [
			{
				name: "ehtoalue",
				description: "on solualue, jonka haluat laskea tietyllä ehdolla"
			},
			{
				name: "ehdot",
				description: "on ehto, joka voi olla laskettavat solut määrittävä luku, lauseke tai teksti"
			}
		]
	},
	{
		name: "LASKE.TYHJÄT",
		description: "Laskee alueella olevien tyhjien solujen määrän.",
		arguments: [
			{
				name: "alue",
				description: "on alue, jolta tyhjät solut lasketaan"
			}
		]
	},
	{
		name: "LEIKKAUSPISTE",
		description: "Palauttaa lineaarisen regressiosuoran ja  y-akselin leikkauspisteen. Regressiosuora piirretään tunnettujen x-arvojen ja y-arvojen avulla.",
		arguments: [
			{
				name: "tunnettu_y",
				description: "on riippuva joukko havaintoja tai tietoja. Arvot voivat olla nimiä, matriiseja tai viittauksia lukuihin (tyhjiä soluja ei huomioida)"
			},
			{
				name: "tunnettu_x",
				description: "on riippumaton joukko havaintoja tai tietoja. Arvot voivat olla nimiä, matriiseja tai viittauksia lukuihin (tyhjiä soluja ei huomioida)"
			}
		]
	},
	{
		name: "LINREGR",
		description: "Palauttaa tilastotiedot, jotka kuvaavat tietopisteisiin sovitettua lineaarista trendiä. Suora on laskettu neliösummamenetelmällä.",
		arguments: [
			{
				name: "tunnettu_y",
				description: "on joukko y-arvoja, jotka tunnetaan yhtälöstä y = mx + b"
			},
			{
				name: "tunnettu_x",
				description: "on valinnainen joukko x-arvoja, jotka ehkä tunnetaan yhtälöstä y = mx + b"
			},
			{
				name: "vakio",
				description: "on totuusarvo, joka määrittää, miten vakion b arvo lasketaan. Jos arvo on TOSI tai jätetty pois, b:n arvo lasketaan normaalisti. Jos arvo on EPÄTOSI, b = 0"
			},
			{
				name: "tilasto",
				description: "on totuusarvo. Jos arvo on TOSI, funktio palauttaa lisää regressiotilastoja. Jos arvo puuttuu tai on EPÄTOSI, funktio palauttaa m-kertoimen ja b:n arvon"
			}
		]
	},
	{
		name: "LOG",
		description: "Palauttaa luvun logaritmin käyttämällä annettua kantalukua.",
		arguments: [
			{
				name: "luku",
				description: "on positiivinen reaaliluku, jolle haluat laskea logaritmin"
			},
			{
				name: "kanta",
				description: "on logaritmin kantaluku. Jos arvoa ei määritetä, käytetään arvoa 10"
			}
		]
	},
	{
		name: "LOG10",
		description: "Palauttaa luvun 10-kantaisen logaritmin.",
		arguments: [
			{
				name: "luku",
				description: "on positiivinen reaaliluku, jolle haluat laskea 10-kantaisen logaritmin"
			}
		]
	},
	{
		name: "LOGNORM.JAKAUMA",
		description: "Palauttaa x:n kumulatiivisen ja logaritmisen normaalijakauman, jossa ln(x) jakautuu normaalijakauman mukaisesti parametrien Keskiarvo ja Keskipoikkeama osoittamalla tavalla.",
		arguments: [
			{
				name: "x",
				description: "on positiivinen lukuarvo, jossa funktion arvo lasketaan"
			},
			{
				name: "keskiarvo",
				description: "on ln(x):n keskiarvo"
			},
			{
				name: "keskihajonta",
				description: "on ln(x):n keskihajonta. Tulos on positiivinen luku"
			}
		]
	},
	{
		name: "LOGNORM.JAKAUMA.KÄÄNT",
		description: "Palauttaa x:n käänteisjakauman kumulatiivisesta ja logaritmisesta normaalijakaumasta, jossa ln(x) jakautuu normaalijakauman mukaisesti parametrien Keskiarvo ja Keskipoikkeama osoittamalla tavalla.",
		arguments: [
			{
				name: "todennäköisyys",
				description: "on logaritmiseen normaalijakaumaan liittyvää todennäköisyyttä osoittava luku väliltä 0 - 1 välin päätepisteet mukaan lukien"
			},
			{
				name: "keskiarvo",
				description: "on ln(x):n keskiarvo"
			},
			{
				name: "keskihajonta",
				description: "on ln(x):n keskihajonta. Tulos on positiivinen luku"
			}
		]
	},
	{
		name: "LOGNORM.KÄÄNT",
		description: "Palauttaa x:n käänteisjakauman kumulatiivisesta ja logaritmisesta normaalijakaumasta, jossa ln(x) jakautuu normaalijakauman mukaisesti parametrien Keskiarvo ja Keskihajonta osoittamalla tavalla.",
		arguments: [
			{
				name: "todennäköisyys",
				description: "on logaritmiseen normaalijakaumaan liittyvää todennäköisyyttä osoittava luku väliltä 0 - 1 välin päätepisteet mukaan lukien"
			},
			{
				name: "keskiarvo",
				description: "on ln(x):n keskiarvo"
			},
			{
				name: "keskihajonta",
				description: "on ln(x):n keskihajonta, positiivinen luku"
			}
		]
	},
	{
		name: "LOGNORM_JAKAUMA",
		description: "Palauttaa x:n logaritmisen normaalijakauman, jossa ln(x) jakautuu normaalijakauman mukaisesti parametrien Keskiarvo ja Keskihajonta osoittamalla tavalla.",
		arguments: [
			{
				name: "x",
				description: "on positiivinen lukuarvo, jossa funktion arvo lasketaan"
			},
			{
				name: "keskiarvo",
				description: "on ln(x):n keskiarvo"
			},
			{
				name: "keskihajonta",
				description: "on ln(x):n keskihajonta, positiivinen luku"
			},
			{
				name: "kertymä",
				description: "on looginen arvo. Jos arvo on TOSI, jakaumalle lasketaan kertymäfunktio. Jos arvo on EPÄTOSI, jakaumalle lasketaan todennäköisyystiheysfunktio"
			}
		]
	},
	{
		name: "LOGREGR",
		description: "Palauttaa annettuihin tietopisteisiin sovitetun eksponentiaalisen käyrän tilastotiedot.",
		arguments: [
			{
				name: "tunnettu_y",
				description: "on joukko y-arvoja, jotka tunnetaan yhtälöstä y = b*m^x"
			},
			{
				name: "tunnettu_x",
				description: "on valinnainen joukko x-arvoja, jotka ehkä tunnetaan yhtälöstä y = b*m^x"
			},
			{
				name: "vakio",
				description: "on totuusarvo, joka määrittää, miten vakion b arvo lasketaan. Jos arvo on TOSI tai jätetty pois, b:n arvo lasketaan normaalisti. Jos arvo on EPÄTOSI, b = 1"
			},
			{
				name: "tilasto",
				description: "on totuusarvo. Jos arvo on TOSI, funktio palauttaa lisää regressiotilastoja. Jos arvo puuttuu tai on EPÄTOSI, funktio palauttaa m-kertoimen ja b:n arvon"
			}
		]
	},
	{
		name: "LUONNLOG",
		description: "Palauttaa luvun luonnollisen logaritmin.",
		arguments: [
			{
				name: "luku",
				description: "on positiivinen reaaliluku, jolle haluat laskea luonnollisen logaritmin"
			}
		]
	},
	{
		name: "LUOTTAMUSVÄLI",
		description: "Palauttaa populaation keskiarvon luottamusvälin normaalijakaumaan käyttäen.",
		arguments: [
			{
				name: "alfa",
				description: "on luottamusväliä laskettaessa käytettävää merkitsevyystasoa osoittava luku. Arvo on suurempi kuin 0 ja pienempi kuin 1"
			},
			{
				name: "keskihajonta",
				description: "on populaation keskihajonta tietoalueella ja sen oletetaan olevan tunnettu. Argumentin täytyy olla positiivinen"
			},
			{
				name: "koko",
				description: "on otoksen koko"
			}
		]
	},
	{
		name: "LUOTTAMUSVÄLI.NORM",
		description: "Palauttaa populaation keskiarvon luottamusvälin normaalijakaumaa käyttäen.",
		arguments: [
			{
				name: "alfa",
				description: "on luottamusväliä laskettaessa käytettävää merkitsevyystasoa osoittava luku. Arvo on suurempi kuin 0 ja pienempi kuin 1"
			},
			{
				name: "keskihajonta",
				description: "on populaation keskihajonta tietoalueella ja sen oletetaan olevan tunnettu. Keskihajonta-argumentin täytyy olla positiivinen"
			},
			{
				name: "koko",
				description: "on otoksen koko"
			}
		]
	},
	{
		name: "LUOTTAMUSVÄLI.T",
		description: "Palauttaa populaation keskiarvon luottamusvälin Studentin T-jakaumaa käyttäen.",
		arguments: [
			{
				name: "alfa",
				description: "on luottamusväliä laskettaessa käytettävää merkitsevyystasoa osoittava luku. Arvo on suurempi kuin 0 ja pienempi kuin 1"
			},
			{
				name: "keskihajonta",
				description: "on populaation keskihajonta tietoalueella ja sen oletetaan olevan tunnettu. Keskihajonta-argumentin täytyy olla positiivinen"
			},
			{
				name: "koko",
				description: "on otoksen koko"
			}
		]
	},
	{
		name: "MAKS",
		description: "Palauttaa suurimman luvun arvojoukosta. Totuusarvot ja merkkijonot jätetään huomioimatta.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 lukua, tyhjää solua, totuusarvoa tai tekstimuotoista lukua, joiden joukosta haluat löytää suurimman arvon"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 lukua, tyhjää solua, totuusarvoa tai tekstimuotoista lukua, joiden joukosta haluat löytää suurimman arvon"
			}
		]
	},
	{
		name: "MAKSA",
		description: "Palauttaa arvojoukon suurimman arvon. Funktio ottaa myös huomioon loogiset arvot ja tekstin.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arvo1",
				description: "ovat 1 - 255 lukua, tyhjää solua, totuusarvoa tai tekstimuotoista lukua, joista etsitään suurinta arvoa"
			},
			{
				name: "arvo2",
				description: "ovat 1 - 255 lukua, tyhjää solua, totuusarvoa tai tekstimuotoista lukua, joista etsitään suurinta arvoa"
			}
		]
	},
	{
		name: "MAKSETTU.KORKO",
		description: "Palauttaa kahden kauden välillä maksetun kumulatiivisen koron.",
		arguments: [
			{
				name: "korko",
				description: "on kauden korkokanta"
			},
			{
				name: "kaudet_yht",
				description: "on maksuerien kokonaismäärä"
			},
			{
				name: "nykyarvo",
				description: "on nykyarvo"
			},
			{
				name: "ens_kausi",
				description: "on ensimmäinen laskentakausi"
			},
			{
				name: "viim_kausi",
				description: "on viimeinen laskentakausi"
			},
			{
				name: "laji",
				description: "on maksun ajankohta"
			}
		]
	},
	{
		name: "MAKSETTU.LYHENNYS",
		description: "Palauttaa kumulatiivisen lyhennyksen kahden jakson välillä.",
		arguments: [
			{
				name: "korko",
				description: "on kauden korkokanta"
			},
			{
				name: "kaudet_yht",
				description: "on maksukausien kokonaismäärä"
			},
			{
				name: "nykyarvo",
				description: "on nykyarvo"
			},
			{
				name: "ens_kausi",
				description: "on ensimmäinen laskentakausi"
			},
			{
				name: "viim_kausi",
				description: "on viimeinen laskentakausi"
			},
			{
				name: "laji",
				description: "on maksun ajankohta"
			}
		]
	},
	{
		name: "MAKSU",
		description: "Palauttaa lainan kausittaisen maksun. Laina perustuu tasaeriin ja kiinteään korkoon.",
		arguments: [
			{
				name: "korko",
				description: "on lainakauden korkokanta. Käytä esimerkiksi neljännesvuosittaisina maksukausina arvoa 6 %/4, kun vuosikorko on 6 %"
			},
			{
				name: "kaudet_yht",
				description: "on lainan maksuerien kokonaismäärä"
			},
			{
				name: "nykyarvo",
				description: "on nykyarvo eli tulevien maksujen yhteisarvo tällä hetkellä"
			},
			{
				name: "ta",
				description: "on tuleva arvo eli arvo, jonka haluat saavuttaa, kun viimeinen maksuerä on hoidettu. Jos arvoa ei määritetä, ohjelma käyttää arvoa 0 (nolla)"
			},
			{
				name: "laji",
				description: "on luku 0 tai 1, joka osoittaa, milloin maksuerät erääntyvät. Jos maksut erääntyvät kauden alussa, arvo = 1. Jos maksut erääntyvät kauden lopussa, arvo = 0 tai jätetty pois"
			}
		]
	},
	{
		name: "MDETERM",
		description: "Palauttaa matriisin matriisideterminantin.",
		arguments: [
			{
				name: "matriisi",
				description: "on lukumatriisi, jossa on yhtä monta riviä ja saraketta. Arvo voi olla solualue tai matriisivakio"
			}
		]
	},
	{
		name: "MEDIAANI",
		description: "Palauttaa annettujen lukujen mediaanin eli annetun lukujoukon keskimmäisen arvon.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 lukua, nimeä, matriisia tai lukuihin kohdistuvaa viittausta, joista haluat etsiä mediaanin"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 lukua, nimeä, matriisia tai lukuihin kohdistuvaa viittausta, joista haluat etsiä mediaanin"
			}
		]
	},
	{
		name: "MERKKI",
		description: "Palauttaa tietokoneen merkistössä annettua lukua vastaavan merkin.",
		arguments: [
			{
				name: "luku",
				description: "on luku väliltä 1 - 255, joka määrittää halutun merkin"
			}
		]
	},
	{
		name: "MIN",
		description: "Palauttaa pienimmän luvun arvojoukosta. Jättää  totuusarvot ja tekstin huomiotta.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1  - 255 lukua, tyhjää solua, totuusarvoa tai tekstimuotoista lukua, joista haluat etsiä pienimmän"
			},
			{
				name: "luku2",
				description: "ovat 1  - 255 lukua, tyhjää solua, totuusarvoa tai tekstimuotoista lukua, joista haluat etsiä pienimmän"
			}
		]
	},
	{
		name: "MINA",
		description: "Palauttaa arvojoukon pienimmän arvon. Funktio ottaa myös huomioon loogiset arvot ja tekstin.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arvo1",
				description: "ovat 1 - 255 lukua, tyhjää solua, totuusarvoa tai tekstimuotoista lukua, joista etsitään pienintä arvoa"
			},
			{
				name: "arvo2",
				description: "ovat 1 - 255 lukua, tyhjää solua, totuusarvoa tai tekstimuotoista lukua, joista etsitään pienintä arvoa"
			}
		]
	},
	{
		name: "MINUUTIT",
		description: "Palauttaa minuutin lukuna väliltä 0–59.",
		arguments: [
			{
				name: "järjestysnro",
				description: "on luku Spreadsheetin käyttämässä päivämäärä- ja kellonaikamuodossa tai teksti aikamuodossa, esimerkiksi 16:48:00"
			}
		]
	},
	{
		name: "MKÄÄNTEINEN",
		description: "Palauttaa matriisin käänteismatriisin.",
		arguments: [
			{
				name: "matriisi",
				description: "on lukumatriisi, jossa on yhtä monta riviä ja saraketta. Tämä voi olla solualue tai matriisivakio"
			}
		]
	},
	{
		name: "MKERRO",
		description: "Palauttaa kahden matriisin tulon. Tuloksessa on yhtä monta riviä kuin matriisissa 1 ja yhtä monta saraketta kuin matriisissa 2.",
		arguments: [
			{
				name: "matriisi1",
				description: "on ensimmäinen matriisi, jonka haluat kertoa. Matriisissa täytyy olla yhtä monta saraketta kuin matriisissa 2 on rivejä"
			},
			{
				name: "matriisi2",
				description: "on ensimmäinen matriisi, jonka haluat kertoa. Matriisissa täytyy olla yhtä monta saraketta kuin matriisissa 2 on rivejä"
			}
		]
	},
	{
		name: "MOODI",
		description: "Palauttaa matriisissa tai tietoalueella useimmin tai toistuvasti esiintyvän arvon.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 lukua, nimeä, matriisia tai viittausta lukuihin, joista haluat laskea moodin"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 lukua, nimeä, matriisia tai viittausta lukuihin, joista haluat laskea moodin"
			}
		]
	},
	{
		name: "MOODI.USEA",
		description: "Palauttaa pystymatriisin matriisissa tai tietoalueella useimmin tai toistuvasti esiintyvistä arvoista. Jos haluat vaakamatriisin, käytä =TRANSPONOI(MOODI.USEA(luku1,luku2),...)-funktiota.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 lukua, nimeä, matriisia tai viittausta lukuihin, joista haluat laskea moodin"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 lukua, nimeä, matriisia tai viittausta lukuihin, joista haluat laskea moodin"
			}
		]
	},
	{
		name: "MOODI.YKSI",
		description: "Palauttaa matriisissa tai tietoalueella useimmin tai toistuvasti esiintyvän arvon.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 lukua, nimeä, matriisia tai viittausta lukuihin, joista haluat laskea moodin"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 lukua, nimeä, matriisia tai viittausta lukuihin, joista haluat laskea moodin"
			}
		]
	},
	{
		name: "MSISÄINEN",
		description: "Palauttaa sisäisen korkokannan sarjalle jatkuvia kassavirtoja, joissa huomioidaan myös sijoituksen arvo ja uudelleen sijoittamisen korko.",
		arguments: [
			{
				name: "arvot",
				description: "on matriisi tai viittaus soluihin, jotka sisältävät tasaisin väliajoin toistuvat maksut (negatiiviset) ja tulot (positiiviset)"
			},
			{
				name: "pääoma_korko",
				description: "on kassavirroissa käytettävästä rahasta maksettava korko"
			},
			{
				name: "uudinvest_korko",
				description: "on uudelleen sijoitetuista kassavirroista saatava korko"
			}
		]
	},
	{
		name: "MULTINOMI",
		description: "Palauttaa lukujoukon multinomin.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 arvoa, joille haluat multinomin"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 arvoa, joille haluat multinomin"
			}
		]
	},
	{
		name: "MUUNNA",
		description: "Muuttaa luvun toiseen järjestelmään.",
		arguments: [
			{
				name: "luku",
				description: "on muunnettava arvo"
			},
			{
				name: "yksiköstä",
				description: "on muutettavan luvun yksikkö"
			},
			{
				name: "yksiköksi",
				description: "on muunnoksen yksikkö"
			}
		]
	},
	{
		name: "N",
		description: "Muuntaa muun kuin numeroarvon numeroksi, päivämäärät järjestysnumeroksi, TOSI-arvon arvoksi 1 ja kaikki muut arvot arvoksi 0 (nolla).",
		arguments: [
			{
				name: "arvo",
				description: "on muunnettava arvo"
			}
		]
	},
	{
		name: "NA",
		description: "Palauttaa sijoituksen nykyarvon.",
		arguments: [
			{
				name: "korko",
				description: "on kauden korkokanta. Käytä esimerkiksi neljännesvuosittaisina maksukausina arvoa 6 %/4, kun vuosikorko on 6 %"
			},
			{
				name: "kaudet_yht",
				description: "on sijoituksen maksukausien kokonaismäärä"
			},
			{
				name: "erä",
				description: "on kunkin kauden maksuerä, joka ei voi muuttua sijoituskauden aikana"
			},
			{
				name: "ta",
				description: "on tuleva arvo eli arvo, jonka haluat saavuttaa, kun viimeinen maksuerä on hoidettu"
			},
			{
				name: "laji",
				description: "on luku 0 tai 1, joka osoittaa, milloin maksuerät erääntyvät. Jos maksut erääntyvät kauden alussa, arvo = 1. Jos maksut erääntyvät kauden lopussa, arvo = 0 tai jätetty pois"
			}
		]
	},
	{
		name: "NELIÖJUURI",
		description: "Palauttaa luvun neliöjuuren.",
		arguments: [
			{
				name: "luku",
				description: "on luku, josta haluat laskea neliöjuuren"
			}
		]
	},
	{
		name: "NELIÖJUURI.PII",
		description: "Palauttaa yhtälön (luku*pii) neliöjuuren.",
		arguments: [
			{
				name: "luku",
				description: "on luku, jolla pii kerrotaan"
			}
		]
	},
	{
		name: "NELIÖSUMMA",
		description: "Palauttaa argumenttien neliösumman. Arvot voivat olla lukuja, nimiä, matriiseja tai viittauksia lukuihin.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 lukua, matriisia, nimeä tai viittausta lukuihin, joista haluat laskea neliösumman"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 lukua, matriisia, nimeä tai viittausta lukuihin, joista haluat laskea neliösumman"
			}
		]
	},
	{
		name: "NELIÖSUMMIEN.EROTUS",
		description: "Laskee kahden alueen tai matriisin toisiaan vastaavien arvojen neliösummien erotuksen.",
		arguments: [
			{
				name: "matriisi_x",
				description: "on ensimmäinen matriisi tai arvoalue. Argumentti voi olla luku, nimi, matriisi tai viittaus, jossa on lukuja"
			},
			{
				name: "matriisi_y",
				description: "on toinen matriisi tai arvoalue. Argumentti voi olla luku, nimi, matriisi tai viittaus, jossa on lukuja"
			}
		]
	},
	{
		name: "NELIÖSUMMIEN.SUMMA",
		description: "Palauttaa kahden alueen tai matriisin toisiaan vastaavien arvojen neliösummien summan.",
		arguments: [
			{
				name: "matriisi_x",
				description: "on ensimmäinen matriisi tai arvoalue. Argumentti voi olla luku, nimi, matriisi tai viittaus, jossa on lukuja"
			},
			{
				name: "matriisi_y",
				description: "on toinen matriisi tai arvoalue. Argumentti voi olla luku, nimi, matriisi tai viittaus, jossa on lukuja"
			}
		]
	},
	{
		name: "NELJÄNNES",
		description: "Palauttaa tietoalueen neljänneksen.",
		arguments: [
			{
				name: "matriisi",
				description: "on lukumatriisi tai lukuja sisältävä solualue, jolle haluat laskea neljännesarvon"
			},
			{
				name: "pal_neljännes",
				description: "on luku: 0 = minimiarvo; 1 = 1. neljännes; 2 = mediaani; 3 = 3. neljännes; 4 = maksimiarvo"
			}
		]
	},
	{
		name: "NELJÄNNES.SIS",
		description: "Palauttaa tietoalueen neljänneksen perustuen prosenttiarvoihin välillä 0 - 1, päätepisteet pois lukien.",
		arguments: [
			{
				name: "matriisi",
				description: "on lukumatriisi tai lukuja sisältävä solualue, jolle haluat laskea neljännesarvon"
			},
			{
				name: "pal_neljännes",
				description: "on luku: 0 = minimiarvo; 1 = 1. neljännes; 2 = mediaani; 3 = 3. neljännes; 4 = maksimiarvo"
			}
		]
	},
	{
		name: "NELJÄNNES.ULK",
		description: "Palauttaa tietoalueen neljänneksen perustuen prosenttiarvoihin välillä 0 - 1, päätepisteet pois lukien.",
		arguments: [
			{
				name: "matriisi",
				description: "on lukumatriisi tai lukuja sisältävä solualue, jolle haluat laskea neljännesarvon"
			},
			{
				name: "pal_neljännes",
				description: "on luku: 0 = minimiarvo; 1 = 1. neljännes; 2 = mediaani; 3 = 3. neljännes; 4 = maksimiarvo"
			}
		]
	},
	{
		name: "NJAKSO",
		description: "Palauttaa kausien määrän sijoitukselle, joka perustuu tasavälisiin, kiinteisiin maksuihin ja kiinteään korkoprosenttiin.",
		arguments: [
			{
				name: "korko",
				description: "on kauden korkokanta. Käytä esimerkiksi neljännesvuosittaisina maksukausina arvoa 6 %/4, kun vuosikorko on 6 %"
			},
			{
				name: "erä",
				description: "on kunkin kauden maksuerä. Maksuerän suuruus on kiinteä"
			},
			{
				name: "nykyarvo",
				description: "on nykyarvo eli tulevien maksujen yhteisarvo tällä hetkellä"
			},
			{
				name: "ta",
				description: "on tuleva arvo eli arvo, jonka haluat saavuttaa, kun viimeinen maksuerä on hoidettu. Jos arvoa ei määritetä, käytetään arvoa 0"
			},
			{
				name: "laji",
				description: "on luku 0 tai 1, joka osoittaa, milloin maksuerät erääntyvät. Jos maksut erääntyvät kauden alussa, arvo = 1. Jos maksut erääntyvät kauden lopussa, arvo = 0 tai jätetty pois"
			}
		]
	},
	{
		name: "NNA",
		description: "Palauttaa nettonykyarvon sijoitukselle, joka perustuu toistuvista kassavirroista muodostuvaan sarjaan (maksuihin ja tuloihin)  ja korkokantaan.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "korko",
				description: "on diskonttokorko yhden kauden ajalta"
			},
			{
				name: "arvo1",
				description: "ovat 1 - 254 maksua tai tuloa, jotka jakaantuvat tasaisesti ja sijoittuvat aina kauden loppuun"
			},
			{
				name: "arvo2",
				description: "ovat 1 - 254 maksua tai tuloa, jotka jakaantuvat tasaisesti ja sijoittuvat aina kauden loppuun"
			}
		]
	},
	{
		name: "NNA.JAKSOTON",
		description: "Palauttaa maksusuoritusten sarjan nykyarvon.",
		arguments: [
			{
				name: "korko",
				description: "on diskonttokorko, jota käytetään rahasuoritusten diskonttaamiseen"
			},
			{
				name: "arvot",
				description: "on sarja rahasuorituksia, jotka tapahtuvat argumentin päivät määrittäminä päivinä"
			},
			{
				name: "päivät",
				description: "on sarja maksupäiviä, joina argumentin arvot maksusuoritukset tapahtuvat"
			}
		]
	},
	{
		name: "NORM.JAKAUMA",
		description: "Palauttaa normaalijakauman kertymäfunktion määritetylle keskiarvolle ja -hajonnalle.",
		arguments: [
			{
				name: "x",
				description: "on arvo, jolle haluat jakauman"
			},
			{
				name: "keskiarvo",
				description: "on jakauman aritmeettinen keskiarvo"
			},
			{
				name: "keskihajonta",
				description: "on jakauman keskihajonta. Arvo on positiivisena luku"
			},
			{
				name: "kumulatiivinen",
				description: "on totuusarvo. Jos arvo on TOSI, jakaumalle lasketaan  kertymäfunktio. Jos arvo on EPÄTOSI, jakaumalle lasketaan todennäköisyyden tiheysfunktio"
			}
		]
	},
	{
		name: "NORM.JAKAUMA.KÄÄNT",
		description: "Palauttaa käänteisen normaalijakauman kertymäfunktion määritetylle keskiarvolle ja -hajonnalle.",
		arguments: [
			{
				name: "todennäköisyys",
				description: "on normaalijakaumaan liittyvä todennäköisyyttä osoittava luku välillä 0 - 1 päätepisteet mukaan lukien"
			},
			{
				name: "keskiarvo",
				description: "on jakauman aritmeettinen keskiarvo"
			},
			{
				name: "keskihajonta",
				description: "on jakauman keskihajontaa osoittava positiivinen luku"
			}
		]
	},
	{
		name: "NORM.JAKAUMA.NORMIT",
		description: "Palauttaa normitetun normaalijakauman kertymäfunktion, jonka keskiarvo on 0 ja keskihajonta 1.",
		arguments: [
			{
				name: "z",
				description: "on arvo, jolle haluat jakauman"
			}
		]
	},
	{
		name: "NORM.JAKAUMA.NORMIT.KÄÄNT",
		description: "Palauttaa käänteisen normitetun normaalijakauman kertymäfunktion, jonka keskiarvo on nolla ja keskihajonta 1.",
		arguments: [
			{
				name: "todennäköisyys",
				description: "on normaalijakaumaan liittyvää todennäköisyyttä osoittava luku välillä 0 - 1 päätepisteet mukaan lukien"
			}
		]
	},
	{
		name: "NORM_JAKAUMA.KÄÄNT",
		description: "Palauttaa käänteisen normitetun normaalijakauman kertymäfunktion, jonka keskiarvo on nolla ja keskihajonta 1.",
		arguments: [
			{
				name: "todennäköisyys",
				description: "on normaalijakaumaan liittyvää todennäköisyyttä osoittava luku välillä 0 - 1 päätepisteet mukaan lukien"
			}
		]
	},
	{
		name: "NORM_JAKAUMA.NORMIT",
		description: "Palauttaa normitetun normaalijakauman (keskiarvo 0 ja keskihajonta 1).",
		arguments: [
			{
				name: "z",
				description: "on arvo, jolle haluat jakauman"
			},
			{
				name: "kertymä",
				description: "on looginen arvo. Jos arvo on TOSI, funktio palauttaa kertymäfunktion. Jos arvo on EPÄTOSI, funktio palauttaa todennäköisyystiheysfunktion"
			}
		]
	},
	{
		name: "NORMAALI.JAKAUMA",
		description: "Palauttaa normaalijakauman määritetylle keskiarvolle ja -hajonnalle.",
		arguments: [
			{
				name: "x",
				description: "on arvo, jolle haluat jakauman"
			},
			{
				name: "keskiarvo",
				description: "on jakauman aritmeettinen keskiarvo"
			},
			{
				name: "keskihajonta",
				description: "on jakauman keskihajonta. Arvo on positiivinen luku"
			},
			{
				name: "kumulatiivinen",
				description: "on totuusarvo. Jos arvo on TOSI, jakaumalle lasketaan kertymäfunktio. Jos arvo on EPÄTOSI, jakaumalle lasketaan todennäköisyystiheysfunktio"
			}
		]
	},
	{
		name: "NORMAALI.JAKAUMA.KÄÄNT",
		description: "Palauttaa käänteisen normaalijakauman kertymäfunktion määritetylle keskiarvolle ja -hajonnalle.",
		arguments: [
			{
				name: "todennäköisyys",
				description: "on normaalijakaumaan liittyvä todennäköisyyttä osoittava luku välillä 0 - 1 päätepisteet mukaan lukien"
			},
			{
				name: "keskiarvo",
				description: "on jakauman aritmeettinen keskiarvo"
			},
			{
				name: "keskihajonta",
				description: "on jakauman keskihajontaa osoittava positiivinen luku"
			}
		]
	},
	{
		name: "NORMITA",
		description: "Palauttaa normitetun arvon keskiarvon ja -hajonnan määrittämästä jakaumasta.",
		arguments: [
			{
				name: "x",
				description: "on arvo, jonka haluat normittaa"
			},
			{
				name: "keskiarvo",
				description: "on jakauman aritmeettinen keskiarvo"
			},
			{
				name: "keskihajonta",
				description: "on jakauman keskihajontaa osoittava positiivinen luku"
			}
		]
	},
	{
		name: "NOUDA.PIVOT.TIEDOT",
		description: "Poimii Pivot-taulukkoon tallennettuja tietoja.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tietokenttä",
				description: "on sen kentän nimi, josta tiedot poimitaan"
			},
			{
				name: "pivot_taulukko",
				description: "on viittaus Pivot-taulukon soluun tai solualueeseen, joka sisältää poimittavat tiedot"
			},
			{
				name: "kenttä",
				description: "kenttä, johon viitataan"
			},
			{
				name: "osa",
				description: "kentän osa, johon viitataan"
			}
		]
	},
	{
		name: "NROARVO",
		description: "Muuntaa tekstin luvuksi maa-asetuksen itsenäisellä tavalla.",
		arguments: [
			{
				name: "teksti",
				description: "on merkkijono, joka esittää muunnettavaa lukua"
			},
			{
				name: "desimaalierotin",
				description: "on merkki, jota käytetään merkkijonossa desimaalierottimena"
			},
			{
				name: "ryhmäerotin",
				description: "on merkki, jota käytetään merkkijonossa ryhmäerottimena"
			}
		]
	},
	{
		name: "NYT",
		description: "Palauttaa nykyisen päivän ja ajan päivämäärän ja ajan muodossa.",
		arguments: [
		]
	},
	{
		name: "OBLIG.HINTA",
		description: "Palauttaa obligaation hinnan (100 euron nimellisarvo).",
		arguments: [
			{
				name: "tilityspvm",
				description: "on obligaation tilityspäivämäärä järjestysnumerona"
			},
			{
				name: "erääntymispvm",
				description: "on obligaation erääntymispäivämäärä järjestysnumerona"
			},
			{
				name: "diskonttokorko",
				description: "on obligaation diskonttokorko"
			}
		]
	},
	{
		name: "OBLIG.TUOTTO",
		description: "Palauttaa obligaation tuoton.",
		arguments: [
			{
				name: "tilityspvm",
				description: "on obligaation tilityspäivämäärä järjestysnumerona"
			},
			{
				name: "erääntymispvm",
				description: "on obligaation erääntymispäivämäärä järjestysnumerona"
			},
			{
				name: "hinta",
				description: "on obligaation hinta (100 euron nimellisarvo)"
			}
		]
	},
	{
		name: "OBLIG.TUOTTOPROS",
		description: "Palauttaa obligaation tuoton.",
		arguments: [
			{
				name: "tilityspvm",
				description: "on obligaation tilityspäivämäärä järjestysnumerona"
			},
			{
				name: "erääntymispvm",
				description: "on obligaation erääntymispäivämäärä järjestysnumerona"
			},
			{
				name: "diskonttokorko",
				description: "on obligaation diskonttokorko"
			}
		]
	},
	{
		name: "OIKAISTU.NELIÖSUMMA",
		description: "Palauttaa keskipoikkeamien neliösumman.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 argumenttia, matriisia tai matriisiviittausta, joista haluat laskea keskipoikkeamien neliösumman"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 argumenttia, matriisia tai matriisiviittausta, joista haluat laskea keskipoikkeamien neliösumman"
			}
		]
	},
	{
		name: "OIKEA",
		description: "Palauttaa määritetyn määrän merkkejä tekstimerkkijonon lopusta lukien.",
		arguments: [
			{
				name: "teksti",
				description: "on tekstimerkkijono, joka sisältää poimittavat merkit"
			},
			{
				name: "merkit_luku",
				description: "määrittää poimittavien merkkien määrän. Jos arvoa ei määritetä, ohjelma käyttää arvoa 1"
			}
		]
	},
	{
		name: "OKTBIN",
		description: "Muuntaa oktaaliluvun binaariluvuksi.",
		arguments: [
			{
				name: "luku",
				description: "on oktaaliluku, jonka haluat muuntaa"
			},
			{
				name: "merkit",
				description: "on käytettävien merkkien lukumäärä"
			}
		]
	},
	{
		name: "OKTDES",
		description: "Muuntaa oktaaliluvun desimaaliluvuksi.",
		arguments: [
			{
				name: "luku",
				description: "on oktaaliluku, jonka haluat muuntaa"
			}
		]
	},
	{
		name: "OKTHEKSA",
		description: "Muuntaa oktaaliluvun heksadesimaaliluvuksi.",
		arguments: [
			{
				name: "luku",
				description: "on oktaaliluku, jonka haluat muuntaa"
			},
			{
				name: "merkit",
				description: "on käytettävien merkkien lukumäärä"
			}
		]
	},
	{
		name: "ONEI_TEKSTI",
		description: "Tarkistaa, onko arvo muuta kuin tekstiä (tyhjät solut eivät ole tekstiä), ja palauttaa arvon TOSI tai EPÄTOSI.",
		arguments: [
			{
				name: "arvo",
				description: "on arvo, jonka haluat testata. Arvo voi olla solu tai kaava tai nimi, joka viittaa soluun, kaavaan tai arvoon"
			}
		]
	},
	{
		name: "ONKAAVA",
		description: "Tarkistaa, onko viittaus kaavan sisältävä solu, ja palauttaa arvon TOSI tai EPÄTOSI.",
		arguments: [
			{
				name: "viittaus",
				description: "on viittaus testattavaan soluun.  Viittaus voi olla soluviittaus, kaava tai soluun viittaava nimi"
			}
		]
	},
	{
		name: "ONLUKU",
		description: "Tarkistaa, onko arvo luku, ja palauttaa arvon TOSI tai EPÄTOSI.",
		arguments: [
			{
				name: "arvo",
				description: "on arvo, jonka haluat testata. Arvo voi olla viittaus soluun tai kaavaan tai nimeen, joka viittaa soluun, kaavaan tai arvoon"
			}
		]
	},
	{
		name: "ONPARILLINEN",
		description: "Palauttaa arvon TOSI, jos luku on parillinen.",
		arguments: [
			{
				name: "luku",
				description: "on testattava arvo"
			}
		]
	},
	{
		name: "ONPARITON",
		description: "Palauttaa arvon TOSI, jos luku on pariton.",
		arguments: [
			{
				name: "luku",
				description: "on testattava arvo"
			}
		]
	},
	{
		name: "ONPUUTTUU",
		description: "Tarkistaa, onko arvo #PUUTTUU, ja palauttaa TOSI- tai EPÄTOSI-arvon.",
		arguments: [
			{
				name: "arvo",
				description: "on arvo, jonka haluat testata. Arvo voi olla viittaus soluun tai kaavaan tai nimeen, joka viittaa soluun, kaavaan tai arvoon"
			}
		]
	},
	{
		name: "ONTEKSTI",
		description: "Tarkistaa, onko arvo tekstiä, ja palauttaa arvon TOSI tai EPÄTOSI.",
		arguments: [
			{
				name: "arvo",
				description: "on arvo, jonka haluat testata. Arvo voi olla viittaus soluun tai kaavaan tai nimeen, joka viittaa soluun, kaavaan tai arvoon"
			}
		]
	},
	{
		name: "ONTOTUUS",
		description: "Tarkistaa, onko arvo totuusarvo (TOSI tai EPÄTOSI), ja palauttaa arvon TOSI tai EPÄTOSI.",
		arguments: [
			{
				name: "arvo",
				description: "on arvo, jonka haluat testata. Arvo voi olla viittaus soluun tai kaavaan tai nimeen, joka viittaa soluun, kaavaan tai arvoon"
			}
		]
	},
	{
		name: "ONTYHJÄ",
		description: "Tarkistaa, onko viittauksen kohteena tyhjä solu, ja palauttaa arvon TOSI tai EPÄTOSI.",
		arguments: [
			{
				name: "arvo",
				description: "on solu tai nimi, joka viittaa testattavaan soluun"
			}
		]
	},
	{
		name: "ONVIITT",
		description: "Tarkistaa, onko arvo viittaus, ja palauttaa arvon TOSI tai EPÄTOSI.",
		arguments: [
			{
				name: "arvo",
				description: "on arvo, jonka haluat testata. Arvo voi olla viittaus soluun tai kaavaan tai nimeen, joka viittaa soluun, kaavaan tai arvoon"
			}
		]
	},
	{
		name: "ONVIRH",
		description: "Tarkistaa, onko arvo jokin muu virhearvo (#ARVO!, #VIITTAUS!, #JAKO/0!, #LUKU!, #NIMI? tai #TYHJÄ!) kuin #PUUTTUU, ja palauttaa arvon TOSI tai EPÄTOSI.",
		arguments: [
			{
				name: "arvo",
				description: "on arvo, jonka haluat testata. Arvo voi olla viittaus soluun, kaavaan tai nimeen, joka viittaa soluun, kaavaan tai arvoon"
			}
		]
	},
	{
		name: "ONVIRHE",
		description: "Tarkistaa, onko arvo mikä tahansa virhearvo (#PUUTTUU, #ARVO!, #VIITTAUS!, #JAKO/0!, #LUKU!, #NIMI? tai #TYHJÄ!), ja palauttaa TOSI- tai EPÄTOSI-arvon.",
		arguments: [
			{
				name: "arvo",
				description: "on arvo, jonka haluat testata. Arvo voi olla viittaus soluun tai kaavaan tai nimeen, joka viittaa soluun, kaavaan tai arvoon"
			}
		]
	},
	{
		name: "OSAMÄÄRÄ",
		description: "Palauttaa osamäärän.",
		arguments: [
			{
				name: "osoittaja",
				description: "on jaettava"
			},
			{
				name: "nimittäjä",
				description: "on jakaja"
			}
		]
	},
	{
		name: "OSOITE",
		description: "Palauttaa soluviittauksen tekstimuodossa, kun argumentteina annetaan rivien ja sarakkeiden numerot.",
		arguments: [
			{
				name: "rivi_nro",
				description: "on soluviittauksessa käytettävän rivin numero: Rivinumero=1 riville 1"
			},
			{
				name: "sarake_nro",
				description: "on soluviittauksessa käytettävän sarakkeen numero. Esimerkiksi sarakkeelle D Sarakenumero=4"
			},
			{
				name: "viittauslaji",
				description: "määrittää viittauksen lajin:  suora = 1; suora viittaus riviin, suhteellinen viittaus sarakkeeseen = 2, suhteellinen viittaus riviin, suora viittaus sarakkeeseen = 3, suhteellinen viittaus = 4"
			},
			{
				name: "a1",
				description: "on totuusarvo, joka määrittää käytetäänkö suoraa vai suhteellista viittausta: A1 (suora) viittaus = 1 tai TOSI; R1C1 (suhteellinen) viittaus = 0 tai EPÄTOSI"
			},
			{
				name: "taulukko_teksti",
				description: "on ulkoisena viittauksena käytettävän laskentataulukon nimi"
			}
		]
	},
	{
		name: "PÄIVÄ",
		description: "Palauttaa kuukauden päivän, luvun väliltä 1–31.",
		arguments: [
			{
				name: "järjestysnro",
				description: "on luku Spreadsheetin käyttämässä päivämäärä- ja kellonaikamuodossa"
			}
		]
	},
	{
		name: "PÄIVÄ.KUUKAUSI",
		description: "Palauttaa sen päivän järjestysnumeron, joka on määrätyn kuukausimäärän verran ennen tai jälkeen alkupäivää.",
		arguments: [
			{
				name: "aloituspäivä",
				description: "on aloituspäivän järjestysnumero"
			},
			{
				name: "kuukaudet",
				description: "on kuukausien lukumäärä ennen tai jälkeen aloituspäivän"
			}
		]
	},
	{
		name: "PÄIVÄT",
		description: "Palauttaa päivän luvun kahden päivämäärän väliltä.",
		arguments: [
			{
				name: "lopetuspäivä",
				description: "aloituspäivä ja lopetuspäivä ovat kaksi päivämäärää, joiden välillä olevien päivien määrän haluat tietää"
			},
			{
				name: "aloituspäivä",
				description: "aloituspäivä ja lopetuspäivä ovat kaksi päivämäärää, joiden välillä olevien päivien määrän haluat tietää"
			}
		]
	},
	{
		name: "PÄIVÄT360",
		description: "Palauttaa kahden päivämäärän välisen päivien lukumäärän käyttämällä 360-päiväistä vuotta (12 kuukautta, joissa on 30 päivää).",
		arguments: [
			{
				name: "aloituspäivä",
				description: "aloituspäivä ja lopetuspäivä ovat päivämäärät, joiden välisten päivien määrän haluat laskea"
			},
			{
				name: "lopetuspäivä",
				description: "aloituspäivä ja lopetuspäivä ovat päivämäärät, joiden välisten päivien määrän haluat laskea"
			},
			{
				name: "menetelmä",
				description: "määrittää käytettävän laskentamenetelmän: EPÄTOSI tai jätetty pois = amerikkalainen (NASD) menetelmä; TOSI = eurooppalainen menetelmä"
			}
		]
	},
	{
		name: "PÄIVÄYS",
		description: "Palauttaa annetun päivämäärän järjestysnumeron Spreadsheetin päivämäärä-aika-koodissa.",
		arguments: [
			{
				name: "vuosi",
				description: "on Spreadsheetin Windows-versiossa luku 1900–9999 tai Spreadsheetin Macintosh-versiossa luku 1904–9999"
			},
			{
				name: "kuukausi",
				description: "on kuukauden järjestysnumero välillä 1–12"
			},
			{
				name: "päivä",
				description: "on päivän järjestysnumero välillä 1–31"
			}
		]
	},
	{
		name: "PÄIVÄYSARVO",
		description: "Muuntaa päivämäärän tekstistä järjestysnumeroksi, joka vastaa päivämäärää Spreadsheetin päivämäärä-aika-koodauksessa.",
		arguments: [
			{
				name: "päivämäärä_teksti",
				description: "on teksti, joka vastaa Spreadsheetin päivämäärämuotoa. Arvot voivat olla 1.1.1900–31.12.9999 (Windows) tai 1.1.1904–31.12.9999  (Macintosh)"
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
		name: "PARILLINEN",
		description: "Pyöristää positiivisen luvun ylöspäin ja negatiivisen luvun alaspäin lähimpään parilliseen kokonaislukuun.",
		arguments: [
			{
				name: "luku",
				description: "on pyöristettävä arvo"
			}
		]
	},
	{
		name: "PARITON",
		description: "Pyöristää positiivisen luvun ylöspäin ja negatiivisen luvun alaspäin lähimpään parittomaan kokonaislukuun.",
		arguments: [
			{
				name: "luku",
				description: "on pyöristettävä arvo"
			}
		]
	},
	{
		name: "PEARSON",
		description: "Palauttaa Pearsonin tulomomenttikorrelaatiokertoimen.",
		arguments: [
			{
				name: "matriisi1",
				description: "on joukko riippumattomia arvoja"
			},
			{
				name: "matriisi2",
				description: "on joukko riippuvia arvoja"
			}
		]
	},
	{
		name: "PEARSON.NELIÖ",
		description: "Palauttaa Pearsonin tulomomenttikorrelaatiokertoimen neliön, joka on laskettu annettujen arvopisteiden pohjalta.",
		arguments: [
			{
				name: "tunnettu_y",
				description: "on matriisi tai arvopisteiden alue. Arvot voivat olla nimiä, matriiseja tai viittauksia lukuihin"
			},
			{
				name: "tunnettu_x",
				description: "on matriisi tai arvopisteiden alue. Arvot voivat olla nimiä, matriiseja tai viittauksia lukuihin"
			}
		]
	},
	{
		name: "PERMUTAATIO",
		description: "Palauttaa permutaatioiden määrän kaikista valittavissa olevista objekteista valituille objekteille.",
		arguments: [
			{
				name: "luku",
				description: "on objektien määrä"
			},
			{
				name: "valittu_luku",
				description: "on objektien määrä jokaisessa permutaatiossa"
			}
		]
	},
	{
		name: "PERMUTAATIOA",
		description: "Palauttaa permutaatioiden määrän kaikista valittavissa olevista objekteista valituille objekteille (toistojen kanssa).",
		arguments: [
			{
				name: "luku",
				description: "on objektien kokonaismäärä"
			},
			{
				name: "valittu_luku",
				description: "on objektien määrä jokaisessa permutaatiossa"
			}
		]
	},
	{
		name: "PERUS",
		description: "Muuntaa luvun tekstimuotoon annetulla kantaluvulla (kanta).",
		arguments: [
			{
				name: "luku",
				description: "on luku, jonka haluat muuntaa"
			},
			{
				name: "kantaluku",
				description: "on kantaluku, joksi haluat muuntaa luvun"
			},
			{
				name: "vähimmäispituus",
				description: "on palautettavan merkkijonon vähimmäispituus.  Jos pois jätettyjä etunollia ei ole lisätty"
			}
		]
	},
	{
		name: "PHAKU",
		description: "Hakee solun arvoa taulukon vasemmanpuoleisimmasta sarakkeesta ja palauttaa arvon samalla rivillä määritetystä sarakkeesta. Oletusarvoisesti taulukon tulee olla lajiteltu nousevassa järjestyksessä.",
		arguments: [
			{
				name: "hakuarvo",
				description: "on arvo, jonka haluat hakea taulukon ensimmäisestä sarakkeesta. Argumentti voi olla arvo, viittaus tai merkkijono"
			},
			{
				name: "taulukko_matriisi",
				description: "on teksti-, luku- tai totuusarvotaulukko, josta ohjelma hakee tietoa. Taulukko_matriisi voi olla viittaus alueeseen tai alueen nimeen"
			},
			{
				name: "sar_indeksi_nro",
				description: "on ensimmäinen sarake taulukko_matriisissa, josta ohjelma palauttaa hakuarvoa vastaavan luvun. Taulukon 1. arvosarake on sarake 1"
			},
			{
				name: "alue_haku",
				description: "määrittää, miten arvo etsitään: TOSI = etsitään 1. sarakkeesta (nousevasti lajiteltu) lähin vastine; EPÄTOSI = etsitään täsmällinen vastine"
			}
		]
	},
	{
		name: "PIENET",
		description: "Muuntaa kaikki tekstissä olevat isot kirjaimet pieniksi.",
		arguments: [
			{
				name: "teksti",
				description: "on teksti, jonka haluat muuttaa pieniksi kirjaimiksi. Merkkejä, jotka eivät ole kirjaimia, ei muunneta"
			}
		]
	},
	{
		name: "PIENI",
		description: "Palauttaa k:nneksi pienimmän arvon tietoalueelta. Esimerkiksi viidenneksi pienimmän arvon.",
		arguments: [
			{
				name: "matriisi",
				description: "on numeerista tietoa sisältävä matriisi tai alue, josta haluat löytää k:nneksi pienimmän arvon"
			},
			{
				name: "k",
				description: "on palautettavan arvon sijainti (pienimmästä lähtien) matriisissa tai tietoalueella"
			}
		]
	},
	{
		name: "PIENIN.YHT.JAETTAVA",
		description: "Palauttaa pienimmän yhteisen kertojan.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 arvoa, joille haluat pienimmän yhteisen kertojan"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 arvoa, joille haluat pienimmän yhteisen kertojan"
			}
		]
	},
	{
		name: "PII",
		description: "Palauttaa piin likiarvon 15 numeron tarkkuudella (3,14159265358979).",
		arguments: [
		]
	},
	{
		name: "PITUUS",
		description: "Palauttaa tekstimerkkijonon merkkien määrän.",
		arguments: [
			{
				name: "teksti",
				description: "on teksti, jonka pituuden haluat selvittää. Välilyönnit lasketaan merkeiksi"
			}
		]
	},
	{
		name: "POIMI.TEKSTI",
		description: "Palauttaa tekstin keskeltä määritetyn määrän merkkejä aloittaen määrittämästäsi kohdasta.",
		arguments: [
			{
				name: "teksti",
				description: "on tekstimerkkijono, joka sisältää poimittavat merkit"
			},
			{
				name: "aloitusnro",
				description: "on ensimmäisen tekstistä poimittavan merkin sijainti. Tekstin ensimmäinen merkki on 1"
			},
			{
				name: "merkit_luku",
				description: "määrittää, montako merkkiä funktio poimii tekstistä"
			}
		]
	},
	{
		name: "POISSON",
		description: "Palauttaa Poissonin jakauman.",
		arguments: [
			{
				name: "x",
				description: "on tapahtumien lukumäärä"
			},
			{
				name: "keskiarvo",
				description: "on odotettua numeerista arvoa osoittava positiivinen luku"
			},
			{
				name: "kumulatiivinen",
				description: "on totuusarvo. Jos arvo on TOSI, lasketaan Poissonin todennäköisyyden kertymäfunktio. Jos arvo on EPÄTOSI, lasketaan Poissonin todennäköisyysmassafunktio"
			}
		]
	},
	{
		name: "POISSON.JAKAUMA",
		description: "Palauttaa Poissonin jakauman.",
		arguments: [
			{
				name: "x",
				description: "on tapahtumien lukumäärä"
			},
			{
				name: "keskiarvo",
				description: "on odotettua numeerista arvoa osoittava positiivinen luku"
			},
			{
				name: "kumulatiivinen",
				description: "on totuusarvo. Jos arvo on TOSI, lasketaan Poissonin todennäköisyyden kertymäfunktio. Jos arvo on EPÄTOSI, lasketaan Poissonin todennäköisyysmassafunktio"
			}
		]
	},
	{
		name: "POISTA.VÄLIT",
		description: "Poistaa välit tekstimerkkijonosta paitsi yksittäiset sanojen välissä olevat välit.",
		arguments: [
			{
				name: "teksti",
				description: "on teksti, josta haluat poistaa välit"
			}
		]
	},
	{
		name: "POTENSSI",
		description: "Palauttaa luvun korotettuna haluttuun potenssiin.",
		arguments: [
			{
				name: "luku",
				description: "on kantaluku, joka voi olla mikä tahansa reaaliluku"
			},
			{
				name: "potenssi",
				description: "on eksponentti, johon kantaluku korotetaan"
			}
		]
	},
	{
		name: "PPMT",
		description: "Palauttaa pääoman lyhennyksen annetulla kaudella, kun käytetään tasaeriä ja kiinteää korkoa.",
		arguments: [
			{
				name: "korko",
				description: "on kauden korkokanta. Käytä esimerkiksi neljännesvuosittaisina maksukausina arvoa 6 %/4, kun vuosikorko on 6 %"
			},
			{
				name: "kausi",
				description: "määrittää maksukauden, jonka on oltava välillä 1 - kaudet_yht"
			},
			{
				name: "kaudet_yht",
				description: "on sijoituksen maksukausien kokonaismäärä"
			},
			{
				name: "nykyarvo",
				description: "on nykyarvo eli tulevien maksujen yhteisarvo tällä hetkellä"
			},
			{
				name: "ta",
				description: "on tuleva arvo eli arvo, jonka haluat saavuttaa, kun viimeinen maksuerä on hoidettu"
			},
			{
				name: "laji",
				description: "on luku 0 tai 1, joka osoittaa, milloin maksuerät erääntyvät. Jos maksut erääntyvät kauden alussa, arvo = 1. Jos maksut erääntyvät kauden lopussa, arvo = 0 tai jätetty pois"
			}
		]
	},
	{
		name: "PROSENTTIJÄRJESTYS",
		description: "Palauttaa arvon prosenttijärjestyksen tietojoukossa.",
		arguments: [
			{
				name: "matriisi",
				description: "on numeerinen matriisi tai tietoalue, joka määrittää suhteellisen sijainnin"
			},
			{
				name: "x",
				description: "on arvo, jolle haluat löytää sijoituksen"
			},
			{
				name: "tarkkuus",
				description: "on valinnainen arvo, joka määrittää, montako merkitsevää numeroa palautettavassa prosenttiluvussa on. Jos arvoa ei määritetä, ohjelma käyttää kolmea numeroa (0,xxx%)"
			}
		]
	},
	{
		name: "PROSENTTIJÄRJESTYS.SIS",
		description: "Palauttaa arvon prosenttijärjestyksen tietojoukossa prosenttijärjestyksenä (0 - 1, päätepisteet mukaan lukien) tietojoukossa.",
		arguments: [
			{
				name: "matriisi",
				description: "on numeerinen matriisi tai tietoalue, joka määrittää suhteellisen sijainnin"
			},
			{
				name: "x",
				description: "on arvo, jolle haluat löytää sijoituksen"
			},
			{
				name: "tarkkuus",
				description: "on valinnainen arvo, joka määrittää, montako merkitsevää numeroa palautettavassa prosenttiluvussa on. Jos arvoa ei määritetä, ohjelma käyttää kolmea numeroa (0,xxx%)"
			}
		]
	},
	{
		name: "PROSENTTIJÄRJESTYS.ULK",
		description: "Palauttaa arvon prosenttijärjestyksen tietojoukossa prosenttijärjestyksenä (0 - 1, päätepisteet pois lukien) tietojoukossa.",
		arguments: [
			{
				name: "matriisi",
				description: "on numeerinen matriisi tai tietoalue, joka määrittää suhteellisen sijainnin"
			},
			{
				name: "x",
				description: "on arvo, jolle haluat löytää sijoituksen"
			},
			{
				name: "tarkkuus",
				description: "on valinnainen arvo, joka määrittää, montako merkitsevää numeroa palautettavassa prosenttiluvussa on. Jos arvoa ei määritetä, ohjelma käyttää kolmea numeroa (0,xxx%)"
			}
		]
	},
	{
		name: "PROSENTTIPISTE",
		description: "Palauttaa k:nnen prosenttiosuuden alueen arvoista.",
		arguments: [
			{
				name: "matriisi",
				description: "on matriisi tai tietoalue, joka määrittää suhteellisen sijainnin"
			},
			{
				name: "k",
				description: "on prosenttipistearvo väliltä 0 - 1 (0 ja 1 mukaan lukien)"
			}
		]
	},
	{
		name: "PROSENTTIPISTE.SIS",
		description: "Palauttaa k:nnen prosenttiosuuden alueen arvoista, jossa k on välillä 0 - 1 päätepisteet mukaan lukien.",
		arguments: [
			{
				name: "matriisi",
				description: "on matriisi tai tietoalue, joka määrittää suhteellisen sijainnin"
			},
			{
				name: "k",
				description: "on prosenttipistearvo väliltä 0 - 1 päätepisteet mukaan lukien"
			}
		]
	},
	{
		name: "PROSENTTIPISTE.ULK",
		description: "Palauttaa k:nnen prosenttiosuuden alueen arvoista, jossa k on välillä 0 - 1 päätepisteet pois lukien.",
		arguments: [
			{
				name: "matriisi",
				description: "on matriisi tai tietoalue, joka määrittää suhteellisen sijainnin"
			},
			{
				name: "k",
				description: "on prosenttipistearvo väliltä 0 - 1 päätepisteet mukaan lukien"
			}
		]
	},
	{
		name: "PUUTTUU",
		description: "Palauttaa virhearvon #PUUTTUU (arvo ei ole käytettävissä).",
		arguments: [
		]
	},
	{
		name: "PVMERO",
		description: "",
		arguments: [
		]
	},
	{
		name: "PYÖRISTÄ",
		description: "Pyöristää luvun annettuun määrään desimaaleja.",
		arguments: [
			{
				name: "luku",
				description: "on pyöristettävä luku"
			},
			{
				name: "numerot",
				description: "on käytettävien desimaalien määrä. Negatiivinen arvo pyöristää desimaalipilkun vasemmalle puolelle, nolla lähimpään kokonaislukuun"
			}
		]
	},
	{
		name: "PYÖRISTÄ.DES.ALAS",
		description: "Pyöristää luvun alaspäin (nollaa kohti).",
		arguments: [
			{
				name: "luku",
				description: "on reaaliluku, jonka haluat pyöristää alaspäin"
			},
			{
				name: "numerot",
				description: "on numeroiden määrä, johon haluat pyöristää luvun. Negatiivisella arvolla pyöristäminen tapahtuu desimaalipilkun vasemmalle puolelle. Jos arvo on nolla tai se puuttuu, ohjelma pyöristää lähimpään kokonaislukuun"
			}
		]
	},
	{
		name: "PYÖRISTÄ.DES.YLÖS",
		description: "Pyöristää luvun ylöspäin (nollasta poispäin).",
		arguments: [
			{
				name: "luku",
				description: "on reaaliluku, jonka haluat pyöristää ylöspäin"
			},
			{
				name: "numerot",
				description: "on numeroiden määrä, johon haluat pyöristää luvun. Negatiivisella arvolla pyöristäminen tapahtuu desimaalipilkun vasemmalle puolelle. Jos arvo on nolla tai se puuttuu, ohjelma pyöristää lähimpään kokonaislukuun"
			}
		]
	},
	{
		name: "PYÖRISTÄ.KERR",
		description: "Palauttaa luvun pyöristettynä haluttuun kerrannaiseen.",
		arguments: [
			{
				name: "luku",
				description: "on pyöristettävä arvo"
			},
			{
				name: "kerrannainen",
				description: "on kerrannainen, johon haluat pyöristää luvun"
			}
		]
	},
	{
		name: "PYÖRISTÄ.KERR.ALAS",
		description: "Pyöristää luvun alaspäin lähimpään tarkkuuden kerrannaiseen.",
		arguments: [
			{
				name: "luku",
				description: "on numeerinen arvo, jonka haluat pyöristää"
			},
			{
				name: "tarkkuus",
				description: "on kerrannainen, johon haluat pyöristää luvun. Luvun ja tarkkuuden tulee olla molempien joko positiivisia tai negatiivisia"
			}
		]
	},
	{
		name: "PYÖRISTÄ.KERR.ALAS.MATEMAATTINEN",
		description: "Pyöristää luvun alaspäin seuraavaan kokonaislukuun tai seuraavaan tarkkuuden monikertaan.",
		arguments: [
			{
				name: "luku",
				description: "on pyöristettävä arvo"
			},
			{
				name: "tarkkuus",
				description: "on monikerta, johon luku halutaan pyöristää"
			},
			{
				name: "tila",
				description: "kun se on annettu ja poikkeaa nollasta, toiminto pyöristää luvun nollaa kohti"
			}
		]
	},
	{
		name: "PYÖRISTÄ.KERR.ALAS.TARKKA",
		description: "Pyöristää luvun alaspäin lähimpään kokonaislukuun tai tarkkuuden kerrannaiseen.",
		arguments: [
			{
				name: "luku",
				description: "on numeerinen arvo, jonka haluat pyöristää"
			},
			{
				name: "tarkkuus",
				description: "on kerrannainen, johon haluat pyöristää luvun. "
			}
		]
	},
	{
		name: "PYÖRISTÄ.KERR.YLÖS",
		description: "Pyöristää luvun ylöspäin lähimpään tarkkuuden kerrannaiseen.",
		arguments: [
			{
				name: "luku",
				description: "on pyöristettävä luku"
			},
			{
				name: "tarkkuus",
				description: "on kerrannainen, johon haluat pyöristää luvun"
			}
		]
	},
	{
		name: "PYÖRISTÄ.KERR.YLÖS.MATEMAATTINEN",
		description: "Pyöristää luvun ylöspäin seuraavaan kokonaislukuun tai seuraavaan tarkkuuden monikertaan.",
		arguments: [
			{
				name: "luku",
				description: "on pyöristettävä arvo"
			},
			{
				name: "tarkkuus",
				description: "on monikerta, johon luku halutaan pyöristää"
			},
			{
				name: "tila",
				description: "kun se on annettu ja poikkeaa nollasta, toiminto pyöristää luvun nollasta poispäin"
			}
		]
	},
	{
		name: "PYÖRISTÄ.KERR.YLÖS.TARKKA",
		description: "Pyöristää luvun ylöspäin lähimpään kokonaislukuun tai tarkkuuden kerrannaiseen.",
		arguments: [
			{
				name: "luku",
				description: "on pyöristettävä luku"
			},
			{
				name: "tarkkuus",
				description: "on kerrannainen, johon haluat pyöristää luvun"
			}
		]
	},
	{
		name: "RADIAANIT",
		description: "Muuntaa asteet radiaaneiksi.",
		arguments: [
			{
				name: "kulma",
				description: "on muunnettava kulma asteina"
			}
		]
	},
	{
		name: "RAJA",
		description: "Testaa onko luku suurempi kuin raja-arvo.",
		arguments: [
			{
				name: "luku",
				description: "on arvo, jota testataan raja-arvoa vastaan"
			},
			{
				name: "raja_arvo",
				description: "on raja-arvo"
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
		name: "RIVI",
		description: "Palauttaa viittauksen rivinumeron.",
		arguments: [
			{
				name: "viittaus",
				description: "on solu tai solualue, jonka rivinumeron haluat saada. Jos arvoa ei määritetä, funktio palauttaa viittauksen siihen soluun, joka sisältää RIVI-funktion"
			}
		]
	},
	{
		name: "RIVIT",
		description: "Palauttaa viittauksessa tai matriisissa olevien rivien määrän.",
		arguments: [
			{
				name: "matriisi",
				description: "on matriisi, matriisikaava tai viittaus solualueeseen, jonka rivimäärän haluat tietää"
			}
		]
	},
	{
		name: "ROMAN",
		description: "Muuntaa arabialaiset numerot roomalaisiksi numeroiksi.",
		arguments: [
			{
				name: "luku",
				description: "on arabialainen luku, jonka haluat muuntaa"
			},
			{
				name: "muoto",
				description: "on luku, joka määrittää, minkätyyppisen roomalaisen luvun haluat."
			}
		]
	},
	{
		name: "RTD",
		description: "Hakee reaaliaikaiset tiedot ohjelmasta, joka tukee COM-automaatiota.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "progID",
				description: "on rekisteröidyn COM-automaatioapuohjelman ProgID-tunnus. Kirjoita nimi lainausmerkkien sisään"
			},
			{
				name: "palvelin",
				description: "on sen palvelimen nimi, jossa apuohjelma tulisi suorittaa. Kirjoita nimi lainausmerkkien sisään. Jos apuohjelma suoritetaan paikallisesti, käytä tyhjää merkkijonoa"
			},
			{
				name: "aihe1",
				description: "ovat vähintään 1 ja enintään 38 parametria, joilla määritetään tiedot"
			},
			{
				name: "aihe2",
				description: "ovat vähintään 1 ja enintään 38 parametria, joilla määritetään tiedot"
			}
		]
	},
	{
		name: "SAATU.HINTA",
		description: "Palauttaa arvopaperista erääntymispäivänä saatavan rahasumman.",
		arguments: [
			{
				name: "tilityspvm",
				description: "on arvopaperin tilityspäivämäärä järjestysnumerona"
			},
			{
				name: "erääntymispvm",
				description: "on arvopaperin erääntymispäivämäärä järjestysnumerona"
			},
			{
				name: "sijoitus",
				description: "on arvopaperiin sijoitettu rahamäärä"
			},
			{
				name: "diskonttokorko",
				description: "on arvopaperin diskonttokorko"
			},
			{
				name: "peruste",
				description: "on käytettävä päivien laskentaperuste"
			}
		]
	},
	{
		name: "SAMA.ARVO",
		description: "Testaa ovatko kaksi lukua yhtäsuuret.",
		arguments: [
			{
				name: "luku1",
				description: "on ensimmäinen luku"
			},
			{
				name: "luku2",
				description: "on toinen luku"
			}
		]
	},
	{
		name: "SARAKE",
		description: "Palauttaa annettua viittausta vastaavan sarakkeen numeron.",
		arguments: [
			{
				name: "viittaus",
				description: "on solu tai yhtenäinen solualue, jonka sarakenumeron haluat saada selville. Jos tätä arvoa ei anneta, käytetään solua, joka sisältää SARAKE-funktion"
			}
		]
	},
	{
		name: "SARAKKEET",
		description: "Palauttaa viittauksessa tai matriisissa olevien sarakkeiden määrän.",
		arguments: [
			{
				name: "matriisi",
				description: "on matriisi, matriisikaava tai viittaus solualueeseen, jonka sarakkeiden määrän haluat laskea"
			}
		]
	},
	{
		name: "SARJA.SUMMA",
		description: "Palauttaa potenssisarjan summan.",
		arguments: [
			{
				name: "x",
				description: "on potenssisarjan syöttöarvo"
			},
			{
				name: "n",
				description: "on ensimmäinen potenssi, johon haluat korottaa argumentin x"
			},
			{
				name: "m",
				description: "on askel, jonka verran potenssi kasvaa tekijästä toiseen"
			},
			{
				name: "kertoimet",
				description: "on joukko kertoimia, joilla jokainen peräkkäinen argumentin x arvo kerrotaan"
			}
		]
	},
	{
		name: "SATUNNAISLUKU",
		description: "Palauttaa tasaisesti jakautuneen satunnaisluvun, joka on yhtä suuri tai suurempi kuin 0 ja pienempi kuin 1.",
		arguments: [
		]
	},
	{
		name: "SATUNNAISLUKU.VÄLILTÄ",
		description: "Palauttaa satunnaisluvun määritettyjen arvojen väliltä.",
		arguments: [
			{
				name: "ala",
				description: "on pienin kokonaisluku, jonka SATUNNAISLUKU.VÄLILTÄ-funktio palauttaa"
			},
			{
				name: "ylä",
				description: "on suurin kokonaisluku, jonka SATUNNAISLUKU.VÄLILTÄ-funktio palauttaa"
			}
		]
	},
	{
		name: "SEK",
		description: "Palauttaa kulman sekantin.",
		arguments: [
			{
				name: "luku",
				description: "on radiaaneina kulma, jolle haluat laskea sekantin"
			}
		]
	},
	{
		name: "SEKH",
		description: "Palauttaa kulman hyperbolisen sekantin.",
		arguments: [
			{
				name: "luku",
				description: "on radiaaneina kulma, jolle haluat laskea hyperbolisen sekantin"
			}
		]
	},
	{
		name: "SEKUNNIT",
		description: "Palauttaa sekunnin lukuna väliltä 0–59.",
		arguments: [
			{
				name: "järjestysnro",
				description: "on luku Spreadsheetin käyttämässä päivämäärä- ja kellonaikamuodossa tai teksti aikamuodossa, esimerkiksi 16:48:23"
			}
		]
	},
	{
		name: "SIIRTYMÄ",
		description: "Palauttaa viittauksen alueeseen, joka on annetun etäisyyden (sarakkeina ja riveinä) päässä annetusta viittauksesta.",
		arguments: [
			{
				name: "viittaus",
				description: "on viittaus, johon siirtymä perustuu. Viittauksen täytyy kohdistua soluun tai yhtenäiseen solualueeseen"
			},
			{
				name: "rivit",
				description: "on rivien lukumäärä, joka osoittaa, kuinka monta riviä ylös- tai alaspäin vasemmassa yläkulmassa oleva solu viittaa"
			},
			{
				name: "sarakkeet",
				description: "on sarakkeiden lukumäärä, joka osoittaa, kuinka monta saraketta vasemmalle tai oikealle vasemmassa yläkulmassa oleva solu viittaa"
			},
			{
				name: "korkeus",
				description: "on luku, joka osoittaa palautettavan viittauksen korkeuden riveinä. Jos arvoa ei määritetä, ohjelma käyttää samaa arvoa kuin viittauksessa"
			},
			{
				name: "leveys",
				description: "on luku, joka osoittaa palautettavan viittauksen leveyden. Jos arvoa ei määritetä, ohjelma käyttää samaa arvoa kuin viittauksessa"
			}
		]
	},
	{
		name: "SIIVOA",
		description: "Poistaa tekstistä kaikki merkit, jotka eivät tulostu.",
		arguments: [
			{
				name: "teksti",
				description: "on mikä tahansa laskentataulukon tieto, josta haluat poistaa tulostumattomat merkit"
			}
		]
	},
	{
		name: "SIN",
		description: "Palauttaa annetun kulman sinin.",
		arguments: [
			{
				name: "luku",
				description: "on kulma radiaaneina, josta haluat laskea sinin. Asteet*(pii/180)=radiaanit"
			}
		]
	},
	{
		name: "SINH",
		description: "Palauttaa luvun hyperbolisen sinin.",
		arguments: [
			{
				name: "luku",
				description: "on mikä tahansa reaaliluku"
			}
		]
	},
	{
		name: "SISÄINEN.KORKO",
		description: "Palauttaa sisäisen korkokannan toistuvista kassavirroista muodostuvalle sarjalle.",
		arguments: [
			{
				name: "arvot",
				description: "on matriisi tai soluviittaus, jossa oleville luvuille funktio laskee sisäisen korkokannan"
			},
			{
				name: "arvaus",
				description: "on luku, jonka arvaat olevan lähellä funktion SISÄINEN.KORKO antamaa tulosta. Jos argumenttia ei anneta, arvo on 0,1 (10 prosenttia)"
			}
		]
	},
	{
		name: "SISÄINEN.KORKO.JAKSOTON",
		description: "Palauttaa rahasuoritusten sarjan sisäisen korkokannan.",
		arguments: [
			{
				name: "arvot",
				description: "on sarja rahasuorituksia, jotka tapahtuvat argumentin päivät määrittäminä päivinä"
			},
			{
				name: "päivät",
				description: "on sarja maksupäiviä, joina argumentin arvot maksusuoritukset tapahtuvat"
			},
			{
				name: "arvaus",
				description: "on arvo, jonka arvaat olevan lähellä funktion SISÄINEN.KORKO.JAKSOTON laskemaa sisäistä korkokantaa"
			}
		]
	},
	{
		name: "SOLU",
		description: "Palauttaa tietoja viittauksen ensimmäisen solun (laskentataulukon lukusuunnan mukaan) muotoilusta, sijainnista tai sisällöstä.",
		arguments: [
			{
				name: "kuvaus_laji",
				description: "on tekstiarvo, joka määrittää, minkä tyyppisiä solutietoja haetaan."
			},
			{
				name: "viittaus",
				description: "on solu, josta halutaan tietoja"
			}
		]
	},
	{
		name: "STP",
		description: "Palauttaa sijoituksen tasapoiston yhdeltä kaudelta.",
		arguments: [
			{
				name: "kustannus",
				description: "on sijoituksen alkuperäinen hankintahinta"
			},
			{
				name: "loppuarvo",
				description: "on sijoituksen arvo käyttöajan lopussa (jäännösarvo)"
			},
			{
				name: "aika",
				description: "on kausien määrä, joiden aikana sijoitus poistetaan (kutsutaan myös sijoituksen käyttöiäksi)"
			}
		]
	},
	{
		name: "SUMMA",
		description: "Laskee solualueessa olevien lukujen summan.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 yhteenlaskettavaa lukua. Soluissa olevat totuusarvot ja teksti jätetään huomioimatta. Totuusarvot ja teksti huomioidaan vain, jos ne kirjoitetaan argumentteina"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 yhteenlaskettavaa lukua. Soluissa olevat totuusarvot ja teksti jätetään huomioimatta. Totuusarvot ja teksti huomioidaan vain, jos ne kirjoitetaan argumentteina"
			}
		]
	},
	{
		name: "SUMMA.JOS",
		description: "Laskee ehdot täyttävien solujen summan.",
		arguments: [
			{
				name: "alue",
				description: "on solualue, jonka haluat testata"
			},
			{
				name: "ehdot",
				description: "on ehto, joka määrittää, mitkä solut lasketaan yhteen. Ehto voi olla luku, lauseke tai merkkijono"
			},
			{
				name: "summa_alue",
				description: "ovat solut, jotka lasketaan yhteen. Jos argumenttia ei määritetä, kaikki solualueen solut lasketaan yhteen"
			}
		]
	},
	{
		name: "SUMMA.JOS.JOUKKO",
		description: "Lisää tiettyjen ehtojen määrittämät solut.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "summa-alue",
				description: "ovat todelliset yhteenlaskettavat solut."
			},
			{
				name: "ehtoalue",
				description: "on solualue, jonka haluat laskea tietyllä ehdolla"
			},
			{
				name: "ehdot",
				description: "on ehto, joka voi olla lisättävät solut määrittävä luku, lauseke tai teksti"
			}
		]
	},
	{
		name: "SUUNTAUS",
		description: "Palauttaa lineaarisen trendin numerot sovittamalla tunnetut tietopisteet ja käyttäen pienemmän neliön menetelmää.",
		arguments: [
			{
				name: "tunnettu_y",
				description: "on yhtälöstä y = mx + b jo tunnettujen y-arvojen alue tai matriisi"
			},
			{
				name: "tunnettu_x",
				description: "on valinnainen x-arvojen alue tai matriisi, jotka tunnetaan yhtälöstä y = mx + b. Saman kokoinen matriisi kuin tunnettu_y"
			},
			{
				name: "uusi_x",
				description: "on niiden uusien x-arvojen alue tai matriisi, joille haluat SUUNTAUS-funktion palauttavan vastaavat y-arvot"
			},
			{
				name: "vakio",
				description: "on totuusarvo. Vakio b lasketaan normaalisti, jos vakio = TOSI tai jos sitä ei ole. Vakion b arvo on 0, jos vakio = EPÄTOSI"
			}
		]
	},
	{
		name: "SUURI",
		description: "Palauttaa tietoalueen k:nneksi suurimman arvon. Esimerkiksi viidenneksi suurimman arvon.",
		arguments: [
			{
				name: "matriisi",
				description: "on matriisi tai tietoalue, josta haluat määrittää k:nneksi suurimman arvon"
			},
			{
				name: "k",
				description: "on palautettavan arvon sijainti (suurimmasta pienimpään) matriisissa tai solualueessa"
			}
		]
	},
	{
		name: "SUURIN.YHT.TEKIJÄ",
		description: "Palauttaa suurimman yhteisen jakajan.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 arvoa"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 arvoa"
			}
		]
	},
	{
		name: "T",
		description: "Tarkistaa, onko arvo tekstiä. Jos arvo on tekstiä, funktio palauttaa tekstin. Jos arvo ei ole tekstiä, funktio palauttaa tyhjät lainausmerkit.",
		arguments: [
			{
				name: "arvo",
				description: "on testattava arvo"
			}
		]
	},
	{
		name: "T.JAKAUMA",
		description: "Palauttaa vasenhäntäisen Studentin t-jakauman.",
		arguments: [
			{
				name: "x",
				description: "on numeerinen arvo, jossa haluat laskea jakauman"
			},
			{
				name: "vapausasteet",
				description: "on kokonaisluku, joka määrittää jakaumaa kuvaavat vapausasteet"
			},
			{
				name: "kertymä",
				description: "On looginen arvo. Jos arvo on TOSI, funktio palauttaa jakauman kertymäfunktion. Jos arvo on EPÄTOSI, funktio palauttaa todennäköisyystiheysfunktion"
			}
		]
	},
	{
		name: "T.JAKAUMA.2S",
		description: "Palauttaa kaksisuuntaisen Studentin t-jakauman.",
		arguments: [
			{
				name: "x",
				description: "on numeerinen arvo, jossa haluat laskea jakauman"
			},
			{
				name: "vapausasteet",
				description: "on kokonaisluku, joka määrittää jakaumaa kuvaavat vapausasteet"
			}
		]
	},
	{
		name: "T.JAKAUMA.OH",
		description: "Palauttaa oikeahäntäisen Studentin t-jakauman.",
		arguments: [
			{
				name: "x",
				description: "on numeerinen arvo, jossa haluat laskea jakauman"
			},
			{
				name: "vapausasteet",
				description: "on kokonaisluku, joka määrittää jakaumaa kuvaavat vapausasteet"
			}
		]
	},
	{
		name: "T.KÄÄNT",
		description: "Palauttaa käänteisen vasenhäntäisen t-jakauman.",
		arguments: [
			{
				name: "todennäköisyys",
				description: "on kaksisuuntaiseen t-jakaumaan liitetty todennäköisyys. Todennäköisyys on luku välillä 0 - 1 (1 mukaan lukien)"
			},
			{
				name: "vapausasteet",
				description: "on jakaumaa kuvaava vapausasteiden määrä. Arvon täytyy olla positiivinen kokonaisluku"
			}
		]
	},
	{
		name: "T.KÄÄNT.2S",
		description: "Palauttaa käänteisen kaksisuuntaisen t-jakauman.",
		arguments: [
			{
				name: "todennäköisyys",
				description: "on kaksisuuntaiseen t-jakaumaan liitetty todennäköisyys. Todennäköisyys on luku välillä 0 - 1 (1 mukaan lukien)"
			},
			{
				name: "vapausasteet",
				description: "on jakaumaa kuvaava vapausasteiden määrä. Arvon täytyy olla positiivinen kokonaisluku"
			}
		]
	},
	{
		name: "T.TESTI",
		description: "Palauttaa t-testiin liittyvän todennäköisyyden.",
		arguments: [
			{
				name: "matriisi1",
				description: "on ensimmäinen arvojoukko"
			},
			{
				name: "matriisi2",
				description: "on toinen arvojoukko"
			},
			{
				name: "suunta",
				description: "määrittää onko jakauma yksi, vai kaksisuuntainen. Jos arvo = 1, jakauma on yksisuuntainen. Jos arvo = 2, jakauma on kaksisuuntainen"
			},
			{
				name: "laji",
				description: "on suoritettavan t-testin tyyppi. Jos arvo = 1, testi on parittainen. Jos arvo = 2, kahden näytteen varianssit ovat samat (homoskedastiset). Jos arvo = 3, kahden näytteen varianssit ovat erilaiset"
			}
		]
	},
	{
		name: "TAAJUUS",
		description: "Laskee, kuinka usein arvot esiintyvät arvoalueessa ja palauttaa pystymatriisin, jossa on yksi elementti enemmän kuin Bins_matriisissa.",
		arguments: [
			{
				name: "tieto_matriisi",
				description: "on arvosarjan matriisi tai viittaus arvosarjaan, jonka taajuusjakauman haluat laskea (tyhjät alueet ja teksti ohitetaan)"
			},
			{
				name: "lohko_matriisi",
				description: "on välien matriisi tai viittaus väleihin, joihin haluat ryhmitellä tieto_matriisin arvot"
			}
		]
	},
	{
		name: "TAI",
		description: "Tarkistaa, onko minkään argumentin totuusarvo TOSI, ja palauttaa TOSI- tai EPÄTOSI-arvon. Jos kaikkien argumenttien arvo on EPÄTOSI, funktio palauttaa arvon EPÄTOSI.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "totuus1",
				description: "ovat 1 - 255 ehtoa, jotka voivat testattaessa saada arvon TOSI tai EPÄTOSI"
			},
			{
				name: "totuus2",
				description: "ovat 1 - 255 ehtoa, jotka voivat testattaessa saada arvon TOSI tai EPÄTOSI"
			}
		]
	},
	{
		name: "TÄMÄ.PÄIVÄ",
		description: "Palauttaa nykyisen päivämäärän päivämäärämuodossa.",
		arguments: [
		]
	},
	{
		name: "TAN",
		description: "Palauttaa kulman tangentin.",
		arguments: [
			{
				name: "luku",
				description: "on radiaaneina se kulma, josta haluat laskea tangentin. Asteet*(pii/180)=radiaanit"
			}
		]
	},
	{
		name: "TANH",
		description: "Palauttaa luvun hyperbolisen tangentin.",
		arguments: [
			{
				name: "luku",
				description: "on mikä tahansa reaaliluku"
			}
		]
	},
	{
		name: "TAULUKKO",
		description: "Palauttaa viitattavan taulukon numeron.",
		arguments: [
			{
				name: "arvo",
				description: "on taulukon nimi tai viittaus, josta haluat taulukon luvun. Jos se jätetään pois, funktion sisältävän taulukon luku palautetaan"
			}
		]
	},
	{
		name: "TAULUKOT",
		description: "Palauttaa viittauksessa olevien taulukoiden määrän.",
		arguments: [
			{
				name: "viittaus",
				description: "on viittaus, josta haluat tietää sen sisältämien taulukoiden määrän. Jos jätetään pois, työkirjan funktion sisältävien taulukoiden määrä palautetaan"
			}
		]
	},
	{
		name: "TEKSTI",
		description: "Muotoilee luvun ja muuntaa sen tekstiksi.",
		arguments: [
			{
				name: "arvo",
				description: "on luku, luvun tuottava kaava tai viittaus luvun sisältävään soluun"
			},
			{
				name: "muoto_teksti",
				description: "on tekstimuotoinen luvun esitysmuoto Muotoile luku -valintaikkunasta"
			}
		]
	},
	{
		name: "TJAKAUMA",
		description: "Palauttaa Studentin t-jakauman.",
		arguments: [
			{
				name: "x",
				description: "on numeerinen arvo, jossa haluat laskea jakauman"
			},
			{
				name: "vapausasteet",
				description: "on kokonaisluku, joka määrittää jakaumaa kuvaavat vapausasteet"
			},
			{
				name: "suunta",
				description: "määrittää jakauman suuntien lukumäärän. Yksisuuntainen jakauma = 1; kaksisuuntainen jakauma = 2"
			}
		]
	},
	{
		name: "TJAKAUMA.KÄÄNT",
		description: "Palauttaa käänteisen t-jakauman.",
		arguments: [
			{
				name: "todennäköisyys",
				description: "on kaksisuuntaiseen t-jakaumaan liitetty todennäköisyys. Todennäköisyys on luku välillä 0 - 1 (1 mukaan lukien)"
			},
			{
				name: "vapausasteet",
				description: "on jakaumaa kuvaava vapausasteiden määrä. Arvon täytyy olla positiivinen kokonaisluku"
			}
		]
	},
	{
		name: "TKESKIARVO",
		description: "Palauttaa valittujen tietokantakenttien arvojen keskiarvon.",
		arguments: [
			{
				name: "tietokanta",
				description: "on solualue, joka muodostaa luettelon tai tietokannan. Tietokanta on joukko toisiinsa liittyviä tietoja"
			},
			{
				name: "kenttä",
				description: "on joko lainausmerkeissä oleva sarakkeen otsikko tai luku, joka määrittää sarakkeen sijainnin luettelossa"
			},
			{
				name: "ehdot",
				description: "on solualue, joka sisältää määrittämäsi ehdot. Alue sisältää sarakeotsikon ja otsikon alapuolella olevan solun, joka sisältää ehdot"
			}
		]
	},
	{
		name: "TKESKIHAJONTA",
		description: "Laskee populaation keskipoikkeaman otoksen perusteella käyttäen ehdon täyttävissä tietokantakentissä olevia arvoja.",
		arguments: [
			{
				name: "tietokanta",
				description: "on solualue, joka muodostaa luettelon tai tietokannan. Tietokanta on joukko toisiinsa liittyviä tietoja"
			},
			{
				name: "kenttä",
				description: "on joko lainausmerkeissä oleva sarakkeen otsikko tai luku, joka määrittää sarakkeen sijainnin luettelossa"
			},
			{
				name: "ehdot",
				description: "on solualue, joka sisältää määrittämäsi ehdot. Alue sisältää sarakeotsikon ja otsikon alapuolella olevan solun, joka sisältää ehdot"
			}
		]
	},
	{
		name: "TKESKIHAJONTAP",
		description: "Laskee keskihajonnan koko populaatiosta käyttäen ehdon täyttävissä tietokantakentissä olevia arvoja.",
		arguments: [
			{
				name: "tietokanta",
				description: "on solualue, joka muodostaa luettelon tai tietokannan. Tietokanta on joukko toisiinsa liittyviä tietoja"
			},
			{
				name: "kenttä",
				description: "on joko lainausmerkeissä oleva sarakkeen otsikko tai luku, joka määrittää sarakkeen sijainnin luettelossa"
			},
			{
				name: "ehdot",
				description: "on solualue, joka sisältää määrittämäsi ehdot. Alue sisältää sarakeotsikon ja otsikon alapuolella olevan solun, joka sisältää ehdot"
			}
		]
	},
	{
		name: "TLASKE",
		description: "Laskee, monessako annetun tietokannan solussa on ehdot täyttävä luku.",
		arguments: [
			{
				name: "tietokanta",
				description: "on solualue, joka muodostaa luettelon tai tietokannan. Tietokanta on joukko toisiinsa liittyviä tietoja"
			},
			{
				name: "kenttä",
				description: "on joko lainausmerkeissä oleva sarakkeen otsikko tai luku, joka määrittää sarakkeen sijainnin luettelossa"
			},
			{
				name: "ehdot",
				description: "on solualue, joka sisältää määrittämäsi ehdot. Alue sisältää sarakeotsikon ja otsikon alapuolella olevan solun, joka sisältää ehdot"
			}
		]
	},
	{
		name: "TLASKEA",
		description: "Laskee, moniko tietokannan tietoja sisältävä solu vastaa määrittämiäsi ehtoja.",
		arguments: [
			{
				name: "tietokanta",
				description: "on solualue, joka muodostaa luettelon tai tietokannan. Tietokanta on joukko toisiinsa liittyviä tietoja"
			},
			{
				name: "kenttä",
				description: "on joko lainausmerkeissä oleva sarakkeen otsikko tai luku, joka määrittää sarakkeen sijainnin luettelossa"
			},
			{
				name: "ehdot",
				description: "on solualue, joka sisältää määrittämäsi ehdot. Alue sisältää sarakeotsikon ja otsikon alapuolella olevan solun, joka sisältää ehdot"
			}
		]
	},
	{
		name: "TMAKS",
		description: "Palauttaa valittujen tietokannan kenttien suurimman, määritettyjä ehtoja vastaavan arvon.",
		arguments: [
			{
				name: "tietokanta",
				description: "on solualue, joka muodostaa luettelon tai tietokannan. Tietokanta on joukko toisiinsa liittyviä tietoja"
			},
			{
				name: "kenttä",
				description: "on joko lainausmerkeissä oleva sarakkeen otsikko tai luku, joka määrittää sarakkeen sijainnin luettelossa"
			},
			{
				name: "ehdot",
				description: "on solualue, joka sisältää määrittämäsi ehdot. Alue sisältää sarakeotsikon ja otsikon alapuolella olevan solun, joka sisältää ehdot"
			}
		]
	},
	{
		name: "TMIN",
		description: "Palauttaa valittujen tietokannan kenttien pienimmän, määritetyt ehdot täyttävän arvon.",
		arguments: [
			{
				name: "tietokanta",
				description: "on solualue, joka muodostaa luettelon tai tietokannan. Tietokanta on joukko toisiinsa liittyviä tietoja"
			},
			{
				name: "kenttä",
				description: "on joko lainausmerkeissä oleva sarakkeen otsikko tai luku, joka määrittää sarakkeen sijainnin luettelossa"
			},
			{
				name: "ehdot",
				description: "on solualue, joka sisältää määrittämäsi ehdot. Alue sisältää sarakeotsikon ja otsikon alapuolella olevan solun, joka sisältää ehdot"
			}
		]
	},
	{
		name: "TNOUDA",
		description: "Poimii yksittäisiä ehdot täyttäviä tietueita tietokannasta.",
		arguments: [
			{
				name: "tietokanta",
				description: "on solualue, joka muodostaa luettelon tai tietokannan. Tietokanta on joukko toisiinsa liittyviä tietoja"
			},
			{
				name: "kenttä",
				description: "on joko lainausmerkeissä oleva sarakkeen otsikko tai luku, joka määrittää sarakkeen sijainnin luettelossa."
			},
			{
				name: "ehdot",
				description: "on solualue, joka sisältää määrittämäsi ehdot. Alue sisältää sarakeotsikon ja otsikon alapuolella olevan solun, joka sisältää ehdot"
			}
		]
	},
	{
		name: "TODENNÄKÖISYYS",
		description: "Palauttaa todennäköisyyden sille, että alueen arvot ovat kahden rajan välissä tai yhtä suuria kuin alaraja.",
		arguments: [
			{
				name: "x_alue",
				description: "on niiden numeeristen x-arvojen alue, joille on olemassa todennäköisyys"
			},
			{
				name: "todnäk_alue",
				description: "on niiden todennäköisyyksien alue, jonka arvot liittyvät x_alueen arvoihin. Arvot ovat välillä 0 - 1, poislukien 0"
			},
			{
				name: "alaraja",
				description: "on alaraja arvolle, jolle haluat todennäköisyyden"
			},
			{
				name: "yläraja",
				description: "on valinnainen yläraja arvolle, jolle haluat todennäköisyyden. Jos arvoa ei määritetä, funktio palauttaa todennäköisyyden sille, x_alueen arvot ovat yhtä suuria kuin alarajan arvot"
			}
		]
	},
	{
		name: "TOISTA",
		description: "Toistaa tekstin antamasi määrän kertoja. Voit käyttää funktiota saman tekstin kirjoittamiseen soluun useita kertoja.",
		arguments: [
			{
				name: "teksti",
				description: "on teksti, jonka haluat toistaa"
			},
			{
				name: "kerrat_luku",
				description: "on positiivinen luku, joka määrittää tekstin toistokertojen määrän"
			}
		]
	},
	{
		name: "TOSI",
		description: "Palauttaa totuusarvon TOSI.",
		arguments: [
		]
	},
	{
		name: "TOT.ROI",
		description: "Palauttaa sijoituksen kasvun vastaavan korkokannan.",
		arguments: [
			{
				name: "nper",
				description: "on sijoituksen kausien määrä"
			},
			{
				name: "pv",
				description: "on sijoituksen nykyarvo"
			},
			{
				name: "fv",
				description: "on sijoituksen tuleva arvo"
			}
		]
	},
	{
		name: "TRANSPONOI",
		description: "Muuntaa vertikaalisen solualueen horisontaaliseksi ja päinvastoin.",
		arguments: [
			{
				name: "matriisi",
				description: "on laskentataulukossa oleva solualue tai arvomatriisi, jonka haluat transponoida"
			}
		]
	},
	{
		name: "TSUMMA",
		description: "Laskee ehdon täyttävissä tietokantakentissä olevien arvojen summan.",
		arguments: [
			{
				name: "tietokanta",
				description: "on solualue, joka muodostaa luettelon tai tietokannan. Tietokanta on joukko toisiinsa liittyviä tietoja"
			},
			{
				name: "kenttä",
				description: "on joko lainausmerkeissä oleva sarakkeen otsikko tai luku, joka määrittää sarakkeen sijainnin luettelossa"
			},
			{
				name: "ehdot",
				description: "on solualue, joka sisältää määrittämäsi ehdot. Alue sisältää sarakeotsikon ja otsikon alapuolella olevan solun, joka sisältää ehdot"
			}
		]
	},
	{
		name: "TTESTI",
		description: "Palauttaa t-testiin liittyvän todennäköisyyden.",
		arguments: [
			{
				name: "matriisi1",
				description: "on ensimmäinen arvojoukko"
			},
			{
				name: "matriisi2",
				description: "on toinen arvojoukko"
			},
			{
				name: "suunta",
				description: "määrittää onko jakauma yksi, vai kaksisuuntainen. Jos arvo = 1, jakauma on yksisuuntainen. Jos arvo = 2, jakauma on kaksisuuntainen"
			},
			{
				name: "laji",
				description: "on suoritettavan t-testin tyyppi. Jos arvo = 1, testi on parittainen. Jos arvo = 2, kahden näytteen varianssit ovat samat. Jos arvo = 3, kahden näytteen varianssit ovat erilaiset"
			}
		]
	},
	{
		name: "TTULO",
		description: "Laskee määritetyt ehdot täyttävien tietokantakenttien tulon.",
		arguments: [
			{
				name: "tietokanta",
				description: "on solualue, joka muodostaa luettelon tai tietokannan. Tietokanta on joukko toisiinsa liittyviä tietoja"
			},
			{
				name: "kenttä",
				description: "on joko lainausmerkeissä oleva sarakkeen otsikko tai luku, joka määrittää sarakkeen sijainnin luettelossa"
			},
			{
				name: "ehdot",
				description: "on solualue, joka sisältää määrittämäsi ehdot. Alue sisältää sarakeotsikon ja otsikon alapuolella olevan solun, joka sisältää ehdot"
			}
		]
	},
	{
		name: "TULEVA.ARVO",
		description: "Palauttaa tasavälisiin vakiomaksueriin ja kiinteään korkoon perustuvan lainan tai sijoituksen tulevan arvon.",
		arguments: [
			{
				name: "korko",
				description: "on kauden korkokanta. Käytä esimerkiksi neljännesvuosittaisina maksukausina arvoa 6 %/4, kun vuosikorko on 6 %"
			},
			{
				name: "kaudet_yht",
				description: "on sijoituksen maksukausien kokonaismäärä"
			},
			{
				name: "erä",
				description: "on kullakin kaudella maksettava maksuerä. Erä on vakio sijoituksen aikana"
			},
			{
				name: "nykyarvo",
				description: "on nykyarvo eli tulevien maksujen yhteisarvo tällä hetkellä. Jos arvo jätetään pois, nykyarvo = 0"
			},
			{
				name: "laji",
				description: "on luku 0 tai 1, joka osoittaa, milloin maksuerät erääntyvät. Jos maksuerä erääntyy kauden alussa, arvo = 1. Jos arvo on nolla tai puuttuu, maksuerä erääntyy kauden lopussa"
			}
		]
	},
	{
		name: "TULEVA.ARVO.ERIKORKO",
		description: "Palauttaa tulevan arvon, joka on saatu käyttämällä erilaisia korkokantoja.",
		arguments: [
			{
				name: "nykyarvo",
				description: "on nykyarvo"
			},
			{
				name: "korot",
				description: "on käytettävien korkokantojen matriisi"
			}
		]
	},
	{
		name: "TULO",
		description: "Palauttaa argumenttien tulon.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 lukua, totuusarvoa tai tekstimuotoista lukua, jotka haluat kertoa keskenään"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 lukua, totuusarvoa tai tekstimuotoista lukua, jotka haluat kertoa keskenään"
			}
		]
	},
	{
		name: "TULOJEN.SUMMA",
		description: "Palauttaa toisiaan vastaavien alueiden tai matriisin osien tulojen summan.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "matriisi1",
				description: "ovat 2 - 255 matriisia, joiden osat haluat ensin kertoa keskenään ja sen jälkeen laskea yhteen. Kaikkien matriisien on oltava samankokoisia"
			},
			{
				name: "matriisi2",
				description: "ovat 2 - 255 matriisia, joiden osat haluat ensin kertoa keskenään ja sen jälkeen laskea yhteen. Kaikkien matriisien on oltava samankokoisia"
			},
			{
				name: "matriisi3",
				description: "ovat 2 - 255 matriisia, joiden osat haluat ensin kertoa keskenään ja sen jälkeen laskea yhteen. Kaikkien matriisien on oltava samankokoisia"
			}
		]
	},
	{
		name: "TUNNIT",
		description: "Palauttaa tunnin kokonaislukumuodossa. Arvo on välillä 0 (0:00)–23 (23:00).",
		arguments: [
			{
				name: "järjestysnro",
				description: "on luku Spreadsheetin käyttämässä päivämäärä- ja kellonaikamuodossa tai teksti aikamuodossa, esimerkiksi 16:48:00"
			}
		]
	},
	{
		name: "TUOTTO.DISK",
		description: "Palauttaa vuosittaisen tuoton diskontatulle arvopaperille.",
		arguments: [
			{
				name: "tilityspvm",
				description: "on arvopaperin tilityspäivämäärä järjestysnumerona"
			},
			{
				name: "erääntymispvm",
				description: "on arvopaperin erääntymispäivämäärä järjestysnumerona"
			},
			{
				name: "hinta",
				description: "on arvopaperin hinta (100 euron nimellisarvo)"
			},
			{
				name: "lunastushinta",
				description: "on arvopaperin lunastushinta (100 euron nimellisarvo)"
			},
			{
				name: "peruste",
				description: "on käytettävä päivien laskentaperuste"
			}
		]
	},
	{
		name: "TVARIANSSI",
		description: "Laskee populaation varianssin otoksen perusteella käyttäen ehdot täyttävissä tietokantakentissä olevia arvoja.",
		arguments: [
			{
				name: "tietokanta",
				description: "on solualue, joka muodostaa luettelon tai tietokannan. Tietokanta on joukko toisiinsa liittyviä tietoja"
			},
			{
				name: "kenttä",
				description: "on joko lainausmerkeissä oleva sarakkeen otsikko tai luku, joka määrittää sarakkeen sijainnin luettelossa"
			},
			{
				name: "ehdot",
				description: "on solualue, joka sisältää määrittämäsi ehdot. Alue sisältää sarakeotsikon ja otsikon alapuolella olevan solun, joka sisältää ehdot"
			}
		]
	},
	{
		name: "TVARIANSSIP",
		description: "Laskee populaation varianssin koko populaation perusteella käyttäen ehdot täyttävissä tietokantakentissä olevia arvoja.",
		arguments: [
			{
				name: "tietokanta",
				description: "on solualue, joka muodostaa luettelon tai tietokannan. Tietokanta on joukko toisiinsa liittyviä tietoja"
			},
			{
				name: "kenttä",
				description: "on joko lainausmerkeissä oleva sarakkeen otsikko tai luku, joka määrittää sarakkeen sijainnin luettelossa"
			},
			{
				name: "ehdot",
				description: "on solualue, joka sisältää määrittämäsi ehdot. Alue sisältää sarakeotsikon ja otsikon alapuolella olevan solun, joka sisältää ehdot"
			}
		]
	},
	{
		name: "TYÖPÄIVÄ",
		description: "Palauttaa sen päivän järjestysnumeron, joka määritettyjen työpäivien päässä aloituspäivästä.",
		arguments: [
			{
				name: "aloituspäivä",
				description: "on aloituspäivän järjestysnumero"
			},
			{
				name: "päivät",
				description: "on työpäivien lukumäärä ennen tai jälkeen aloituspäivän"
			},
			{
				name: "loma",
				description: "on vaihtoehtoinen matriisi päivien järjestysnumeroita, jotka poistetaan työpäivistä. Tällaisia päiviä ovat esim. yleiset vapaapäivät, eivät kuitenkaan tavalliset viikonloput"
			}
		]
	},
	{
		name: "TYÖPÄIVÄ.KANSVÄL",
		description: "Palauttaa sen päivän järjestysnumeron, joka on määritettyjen työpäivien päässä aloituspäivästä, mukautettava viikonloppuparametri huomioiden.",
		arguments: [
			{
				name: "aloituspäivä",
				description: "on aloituspäivän järjestysnumero"
			},
			{
				name: "päivät",
				description: "on muiden kuin viikonlopun ja lomapäivien määrä ennen aloituspäivää tai sen jälkeen"
			},
			{
				name: "viikonloppu",
				description: "on luku tai merkkijono, joka ilmaisee, milloin viikonloput ovat"
			},
			{
				name: "loma",
				description: "on vaihtoehtoinen matriisi sellaisten päivien järjestysnumeroita, jotka poistetaan työpäivistä. Tällaisia päiviä ovat esimerkiksi yleiset vapaapäivät"
			}
		]
	},
	{
		name: "TYÖPÄIVÄT",
		description: "Palauttaa työpäivien lukumäärän kahden päivämäärän väliltä.",
		arguments: [
			{
				name: "aloituspäivä",
				description: "on aloituspäivän järjestysnumero"
			},
			{
				name: "lopetuspäivä",
				description: "on lopetuspäivän järjestysnumero"
			},
			{
				name: "loma",
				description: "on vaihtoehtoinen joukko päivän järjestysnumeroita, jotka kuvaavat yhtä tai useampaa päivää, jotka eivät ole työpäiviä. Tällaisiä päiviä ovat esim. yleiset vapaapäivät, eivät kuitenkaan normaalit viikonloput"
			}
		]
	},
	{
		name: "TYÖPÄIVÄT.KANSVÄL",
		description: "Palauttaa työpäivien lukumäärän kahden päivämäärän väliltä mukautettava viikonloppuparametri huomioiden.",
		arguments: [
			{
				name: "aloituspäivä",
				description: "on aloituspäivän järjestysnumero"
			},
			{
				name: "lopetuspäivä",
				description: "on lopetuspäivän järjestysnumero"
			},
			{
				name: "viikonloppu",
				description: "on numero tai merkkijono, joka ilmaisee, milloin viikonloput ovat"
			},
			{
				name: "loma",
				description: "on vaihtoehtoinen joukko päivän järjestysnumeroita, jotka kuvaavat yhtä tai useaa päivää, joka ei ole työpäivä. Tällaisia päiviä ovat esimerkiksi yleiset vapaapäivät"
			}
		]
	},
	{
		name: "TYYPPI",
		description: "Palauttaa kokonaisluvun, joka vastaa arvon tietotyyppiä: numero = 1, teksti = 2, totuusarvo = 4, virhearvo = 16, matriisi = 64.",
		arguments: [
			{
				name: "arvo",
				description: "voi olla mikä tahansa arvo"
			}
		]
	},
	{
		name: "UNICODE",
		description: "Palauttaa luvun (koodipiste), joka vastaa tekstin ensimmäistä merkkiä.",
		arguments: [
			{
				name: "teksti",
				description: "on merkki, josta haluat Unicode-arvon"
			}
		]
	},
	{
		name: "URLKOODAUS",
		description: "Palauttaa URL-koodatun merkkijonon.",
		arguments: [
			{
				name: "text",
				description: "on URL-koodattava merkkijono"
			}
		]
	},
	{
		name: "VAIHDA",
		description: "Korvaa tekstissä olevan vanhan tekstin uudella tekstillä.",
		arguments: [
			{
				name: "teksti",
				description: "on teksti tai viittaus soluun, joka sisältää muutettavan tekstin"
			},
			{
				name: "vanha_teksti",
				description: "on teksti, jonka haluat vaihtaa. Jos korvattavan tekstin kirjainkoko ei vastaa korvaavan tekstin kirjainkokoa, funktio ei korvaa tekstiä"
			},
			{
				name: "uusi_teksti",
				description: "on teksti, jonka haluat vaihtaa vanha_teksti-arvon tilalle"
			},
			{
				name: "esiintymä_nro",
				description: "määrittää, minkä vanha_teksti-argumentin esiintymän haluat korvata. Jos argumenttia ei määritetä, kaikki esiintymät korvataan"
			}
		]
	},
	{
		name: "VÄLISUMMA",
		description: "Laskee välisumman luettelosta tai tietokannasta.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "funktio_nro",
				description: "on välisummien laskennassa käytettävän funktion määrittävä luku välillä 1 - 11."
			},
			{
				name: "viittaus1",
				description: "ovat 1 - 254 aluetta tai viittausta, joista välisumma lasketaan"
			}
		]
	},
	{
		name: "VALITSE.INDEKSI",
		description: "Valitsee arvon tai suoritettavan toimen indeksiluetteloon perustuvasta arvoluettelosta.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "indeksi_luku",
				description: "määrittää, mikä arvoargumentti valitaan. Indeksiluvun täytyy olla välillä 1 - 254 tai kaava tai viittaus soluun, jonka arvo on välillä 1 - 254"
			},
			{
				name: "arvo1",
				description: "ovat 1 - 254 lukua, soluviittausta, määritettyä nimeä, kaavaa, funktiota tai tekstiargumenttia, jotka määrittävät, mistä VALITSE.INDEKSI-funktio valitsee arvon tai toiminnon, jonka se suorittaa"
			},
			{
				name: "arvo2",
				description: "ovat 1 - 254 lukua, soluviittausta, määritettyä nimeä, kaavaa, funktiota tai tekstiargumenttia, jotka määrittävät, mistä VALITSE.INDEKSI-funktio valitsee arvon tai toiminnon, jonka se suorittaa"
			}
		]
	},
	{
		name: "VALUUTTA",
		description: "Muuntaa luvun valuuttamuotoiseksi tekstiksi.",
		arguments: [
			{
				name: "luku",
				description: "on luku, viittaus luvun sisältävään soluun tai kaava, joka antaa tulokseksi luvun"
			},
			{
				name: "desimaalit",
				description: "on desimaalipilkun oikealla puolella olevien numeroiden määrä. Luku pyöristetään tarvittaessa. Jos lukua ei anneta, oletusarvona on luku 2"
			}
		]
	},
	{
		name: "VALUUTTA.DES",
		description: "Muuntaa murtolukuna esitetyn luvun kymmenjärjestelmän luvuksi.",
		arguments: [
			{
				name: "valuutta_murtoluku",
				description: "on murtolukuna esitetty luku"
			},
			{
				name: "nimittäjä",
				description: "on kokonaisluku, jota käytetään muunnoksessa nimittäjänä"
			}
		]
	},
	{
		name: "VALUUTTA.MURTO",
		description: "Muuntaa kymmenjärjestelmän luvun murtoluvuksi.",
		arguments: [
			{
				name: "valuutta_des",
				description: "on desimaaliluku"
			},
			{
				name: "nimittäjä",
				description: "on kokonaisluku, jota käytetään muunnoksessa nimittäjänä"
			}
		]
	},
	{
		name: "VAR",
		description: "Arvioi populaation varianssin otoksen perusteella (jättää huomiotta otoksessa olevat totuusarvot ja tekstit).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 lukua, jotka vastaavat populaation otosta"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 lukua, jotka vastaavat populaation otosta"
			}
		]
	},
	{
		name: "VAR.P",
		description: "Laskee varianssin koko populaation perusteella (jättää huomiotta populaatiossa olevat totuusarvot ja tekstit).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 lukua, jotka muodostavat populaation"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 lukua, jotka muodostavat populaation"
			}
		]
	},
	{
		name: "VAR.S",
		description: "Arvioi populaation varianssin otoksen perusteella (jättää huomiotta otoksessa olevat totuusarvot ja tekstit).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 lukua, jotka vastaavat populaation otosta"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 lukua, jotka vastaavat populaation otosta"
			}
		]
	},
	{
		name: "VARA",
		description: "Arvioi varianssia näytteen pohjalta. Funktio ottaa myös huomioon  totuusarvot ja tekstin. Teksti ja totuusarvo EPÄTOSI lasketaan arvona 0. Totuusarvo TOSI lasketaan arvona 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arvo1",
				description: "ovat 1 - 255 populaation näytteeseen liittyvää argumenttia"
			},
			{
				name: "arvo2",
				description: "ovat 1 - 255 populaation näytteeseen liittyvää argumenttia"
			}
		]
	},
	{
		name: "VARP",
		description: "Laskee varianssin koko populaation perusteella (jättää huomiotta populaatiossa olevat totuusarvot ja tekstit).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "luku1",
				description: "ovat 1 - 255 lukua, jotka muodostavat populaation"
			},
			{
				name: "luku2",
				description: "ovat 1 - 255 lukua, jotka muodostavat populaation"
			}
		]
	},
	{
		name: "VARPA",
		description: "Laskee koko populaation varianssin. Funktio ottaa myös huomioon totuusarvot ja tekstin. Teksti ja totuusarvo EPÄTOSI lasketaan arvona 0. Totuusarvo TOSI lasketaan arvona 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arvo1",
				description: "ovat 1 - 255 populaatioon liittyvää argumenttia"
			},
			{
				name: "arvo2",
				description: "ovat 1 - 255 populaatioon liittyvää argumenttia"
			}
		]
	},
	{
		name: "VASEN",
		description: "Palauttaa määritetyn määrän merkkejä tekstimerkkijonon alusta lukien.",
		arguments: [
			{
				name: "teksti",
				description: "on tekstimerkkijono, joka sisältää poimittavat merkit"
			},
			{
				name: "merkit_luku",
				description: "määrittää, montako merkkiä haluat VASEN-funktion poimivan. Funktio poimii vain yhden merkin, jos tätä arvoa ei määritetä"
			}
		]
	},
	{
		name: "VASTINE",
		description: "Palauttaa sen matriisin osan suhteellisen sijainnin, joka vastaa määritettyjä ehtoja.",
		arguments: [
			{
				name: "hakuarvo",
				description: "on arvo, jota vastaavan arvon haluat etsiä taulukosta. Arvo voi olla luku, merkkijono, totuusarvo tai viittaus"
			},
			{
				name: "haku_matriisi",
				description: "on jatkuva solualue, joka sisältää mahdolliset etsittävät arvot, matriisin tai viittauksen matriisiin"
			},
			{
				name: "vastine_laji",
				description: "on luku 1, 0 tai -1, joka ilmoittaa palautettavan arvon."
			}
		]
	},
	{
		name: "VDB",
		description: "Palauttaa sijoituksen kaksinkertaisen kirjanpidon tai muun määritetyn menetelmän mukaisen poiston millä hyvänsä annetulla kaudella, mukaanlukien osittaiset kaudet.",
		arguments: [
			{
				name: "kustannus",
				description: "on sijoituksen alkuperäinen hankintahinta"
			},
			{
				name: "loppuarvo",
				description: "on jäännösarvo sijoituksen käyttöiän lopussa"
			},
			{
				name: "aika",
				description: "on kausien määrä, joiden aikana sijoitus poistetaan (kutsutaan myös sijoituksen käyttöiäksi)"
			},
			{
				name: "ens_kausi",
				description: "on ensimmäinen kausi, jolle haluat laskea poiston. Aikayksikön täytyy olla sama kuin argumentissa Aika"
			},
			{
				name: "viim_kausi",
				description: "on viimeinen kausi, jolle haluat laskea poiston. Aikayksikön täytyy olla sama kuin argumentissa Aika"
			},
			{
				name: "kerroin",
				description: "on poiston laskennassa käytettävä kerroin. Jos arvoa ei määritetä, ohjelma käyttää arvoa 2 (kaksinkertaisen kirjanpidon saldo)"
			},
			{
				name: "ei_siirtoa",
				description: "Jos arvo puuttuu tai on EPÄTOSI, ohjelma siirtyy käyttämään tasapoistoa, kun poisto on suurempi kuin DDB-poistotavalla laskettu poisto. Jos argumentin arvo on TOSI, ohjelma ei siirry käyttämään tasapoistoa"
			}
		]
	},
	{
		name: "VERTAA",
		description: "Tarkistaa, ovatko kaksi merkkijonoa samat, ja palauttaa arvon TOSI tai EPÄTOSI. EXACT-funktio ottaa huomioon kirjainkoon.",
		arguments: [
			{
				name: "teksti1",
				description: "on ensimmäinen tekstijono"
			},
			{
				name: "teksti2",
				description: "on toinen tekstijono"
			}
		]
	},
	{
		name: "VHAKU",
		description: "Hakee annettua arvoa matriisin tai taulukon ylimmältä riviltä ja palauttaa samassa sarakkeessa ja määrittämälläsi rivillä olevan arvon.",
		arguments: [
			{
				name: "hakuarvo",
				description: "on arvo, jonka haluat hakea taulukon ensimmäiseltä riviltä. Hakuarvo voi olla arvo, viittaus tai lainausmerkeissä oleva merkkijono"
			},
			{
				name: "taulukko_matriisi",
				description: "on tekstiä, lukuja tai totuusarvoja sisältävä taulukko, josta ohjelma hakee tietoja. Taulukko_matriisi voi olla viittaus alueeseen tai alueen nimi"
			},
			{
				name: "rivi_indeksi_nro",
				description: "on taulukko_matriisin rivinumero, josta ohjelma palauttaa hakuarvoa vastaavan arvon. Taulukon ensimmäinen tietoja sisältävä rivi on rivi 1"
			},
			{
				name: "alue_haku",
				description: "määrittää, miten arvo etsitään: TOSI tai ei arvoa = etsitään 1. riviltä lähin vastaava (nousevasti lajiteltu); EPÄTOSI = täsmällinen vastine"
			}
		]
	},
	{
		name: "VIIKKO.ISO.NRO",
		description: "Palauttaa annetun päivämäärän mukaisen vuoden ISO-viikonnumeron numeron.",
		arguments: [
			{
				name: "päivämäärä",
				description: "on ajan ja päivämäärän koodi, jota Spreadsheet käyttää päivämäärien ja aikojen laskennassa"
			}
		]
	},
	{
		name: "VIIKKO.NRO",
		description: "Palauttaa viikon numeron.",
		arguments: [
			{
				name: "järjestysnro",
				description: "on päivämäärä- ja kellonaikakoodi, jota Spreadsheet käyttää päivämäärän ja kellonajan laskemisessa"
			},
			{
				name: "palauta_tyyppi",
				description: "on luku (1 tai 2), joka määrittää palautettavan arvon tyypin"
			}
		]
	},
	{
		name: "VIIKONPÄIVÄ",
		description: "Palauttaa viikonpäivän määrittävän numeron välillä 1 - 7.",
		arguments: [
			{
				name: "järjestysnro",
				description: "on päivämäärää kuvaava luku"
			},
			{
				name: "palauta_tyyppi",
				description: "argumentti määrittää, miten viikonpäivä muutetaan luvuksi. Jos arvo on 1, sunnuntai = 1 ... lauantai = 7. Jos arvo on 2, maanantai = 1 ... sunnuntai = 7. Jos arvo on 3, maanantai = 0 ... sunnuntai = 6"
			}
		]
	},
	{
		name: "VIRHEEN.LAJI",
		description: "Palauttaa virhearvoa vastaavan luvun.",
		arguments: [
			{
				name: "virhearvo",
				description: "on virhearvo, jonka tunnistenumeron haluat löytää.Virhearvo voi olla todellinen virhearvo tai viittaus soluun, joka sisältää virhearvon"
			}
		]
	},
	{
		name: "VIRHEFUNKTIO",
		description: "Palauttaa virhefunktion.",
		arguments: [
			{
				name: "alaraja",
				description: "on alaraja virhefunktion integraatiolle"
			},
			{
				name: "yläraja",
				description: "on yläräja virhefunktion integraatiolle"
			}
		]
	},
	{
		name: "VIRHEFUNKTIO.KOMPLEMENTTI",
		description: "Palauttaa virhefunktion komplementin.",
		arguments: [
			{
				name: "x",
				description: "on alaraja virhefunktion integraatiolle"
			}
		]
	},
	{
		name: "VIRHEFUNKTIO.KOMPLEMENTTI.TARKKA",
		description: "Palauttaa virhefunktion komplementin.",
		arguments: [
			{
				name: "X",
				description: "on alaraja seuraavan kohteen integraatiolle: VIRHEFUNKTIO.KOMPLEMENTTI.TARKKA"
			}
		]
	},
	{
		name: "VIRHEFUNKTIO.TARKKA",
		description: "Palauttaa virhefunktion.",
		arguments: [
			{
				name: "X",
				description: "on alaraja seuraavan kohteen integraatiolle: ERF.PRECISE"
			}
		]
	},
	{
		name: "VUOSI",
		description: "Palauttaa vuoden kokonaislukuna välillä 1900–9999.",
		arguments: [
			{
				name: "järjestysnro",
				description: "on luku Spreadsheetin käyttämässä päivämäärä- ja kellonaikamuodossa"
			}
		]
	},
	{
		name: "VUOSI.OSA",
		description: "Palauttaa arvon, joka ilmoittaa kuinka suuri osa vuodesta kuuluu aloituspäivän ja lopetuspäivän väliseen aikaan.",
		arguments: [
			{
				name: "aloituspäivä",
				description: "on aloituspäivän järjestysnumero"
			},
			{
				name: "lopetuspäivä",
				description: "on lopetuspäivän järjestysnumero"
			},
			{
				name: "peruste",
				description: "on käytettävä päivien laskentaperuste"
			}
		]
	},
	{
		name: "VUOSIPOISTO",
		description: "Palauttaa sijoituksen vuosipoiston annettuna kautena käyttäen amerikkalaista SYD-menetelmää (Sum-of-Year's Digits).",
		arguments: [
			{
				name: "kustannus",
				description: "on sijoituksen alkuperäinen hankintahinta"
			},
			{
				name: "loppuarvo",
				description: "on sijoituksen arvo käyttöajan lopussa (jäännösarvo)"
			},
			{
				name: "aika",
				description: "on kausien määrä, joiden aikana sijoitus poistetaan (kutsutaan myös sijoituksen käyttöiäksi)"
			},
			{
				name: "kausi",
				description: "on laskentakausi, jonka yksikön on oltava sama kuin argumentin aikayksikön"
			}
		]
	},
	{
		name: "WEIBULL",
		description: "Palauttaa Weibullin jakauman.",
		arguments: [
			{
				name: "x",
				description: "on ei-negatiivinen lukuarvo, jossa funktion arvo lasketaan"
			},
			{
				name: "alfa",
				description: "on jakauman parametri. Arvon täytyy olla positiivinen"
			},
			{
				name: "beeta",
				description: "on jakauman parametri. Arvon täytyy olla positiivinen"
			},
			{
				name: "kumulatiivinen",
				description: "on totuusarvo. Jos arvo on TOSI, lasketaan jakauman kertymäfunktio. Jos arvo on EPÄTOSI, lasketaan todennäköisyysmassafunktio"
			}
		]
	},
	{
		name: "WEIBULL.JAKAUMA",
		description: "Palauttaa Weibullin jakauman.",
		arguments: [
			{
				name: "x",
				description: "on ei-negatiivinen lukuarvo, jossa funktion arvo lasketaan"
			},
			{
				name: "alfa",
				description: "on jakauman parametri. Arvon täytyy olla positiivinen"
			},
			{
				name: "beeta",
				description: "on jakauman parametri. Arvon täytyy olla positiivinen"
			},
			{
				name: "kumulatiivinen",
				description: "on totuusarvo. Jos arvo on TOSI, lasketaan jakauman kertymäfunktio. Jos arvo on EPÄTOSI, lasketaan todennäköisyysmassafunktio"
			}
		]
	},
	{
		name: "YKSIKKÖM",
		description: "Palauttaa valitun ulottuvuuden yksikön matriisin.",
		arguments: [
			{
				name: "ulottuvuus",
				description: "on kokonaisluku, joka määrittää palautettavan yksikön matriisin ulottuvuuden"
			}
		]
	},
	{
		name: "Z.TESTI",
		description: "Palauttaa yksisuuntaisen z-testin P-arvon.",
		arguments: [
			{
				name: "matriisi",
				description: "on matriisi tai tietoalue jota vastaan arvoa x testataan"
			},
			{
				name: "x",
				description: "on testattava arvo"
			},
			{
				name: "sigma",
				description: "on populaation (tunnettu) keskihajonta. Jos arvoa ei määritetä, ohjelma käyttää näytteen keskihajontaa"
			}
		]
	},
	{
		name: "ZTESTI",
		description: "Palauttaa yksisuuntaisen z-testin P-arvon.",
		arguments: [
			{
				name: "matriisi",
				description: "on matriisi tai tietoalue jota vastaan arvoa x testataan"
			},
			{
				name: "x",
				description: "on testattava arvo"
			},
			{
				name: "sigma",
				description: "on populaation (tunnettu) keskihajonta. Jos arvoa ei määritetä, ohjelma käyttää näytteen keskihajontaa"
			}
		]
	}
];