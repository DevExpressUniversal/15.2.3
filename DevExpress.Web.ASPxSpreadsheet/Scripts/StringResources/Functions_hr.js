ASPxClientSpreadsheet.Functions = [
	{
		name: "ABS",
		description: "Vraa apsolutnu vrijednost broja, tj. broj bez svog predznaka.",
		arguments: [
			{
				name: "broj",
				description: "je realan broj iju apsolutnu vrijednost tražite"
			}
		]
	},
	{
		name: "ACCRINTM",
		description: "Vraa obraunsku kamatu za vrijednosnicu ije se kamate isplauju po dospijeu.",
		arguments: [
			{
				name: "izdavanje",
				description: "je datum izdavanja vrijednosnice, izražen kao serijski broj datuma"
			},
			{
				name: "isplata",
				description: "je datum dospijea vrijednosnice, izražen kao serijski broj datuma"
			},
			{
				name: "stopa",
				description: "je godiánja kuponska stopa vrijednosnice"
			},
			{
				name: "nom_vrijed",
				description: "je nominalna vrijednost vrijednosnice"
			},
			{
				name: "temelj",
				description: "je vrsta brojanja dana koja se koristi"
			}
		]
	},
	{
		name: "ACOS",
		description: "Vraa arkus-kosinus broja u radijanima u rasponu od 0 do pi. Arkus-kosinus je kut iji je kosinus broj.",
		arguments: [
			{
				name: "broj",
				description: "je kosinus zadanog kuta i mora biti od -1 do 1"
			}
		]
	},
	{
		name: "ACOSH",
		description: "Vraa inverzni hiperbolni kosinus broja.",
		arguments: [
			{
				name: "broj",
				description: "je bilo koji realni broj jednak ili vei od 1"
			}
		]
	},
	{
		name: "ACOT",
		description: "Vraa arkus kotangens broja, u radijanima raspona od 0 do Pi.",
		arguments: [
			{
				name: "broj",
				description: "jest kotangens željenoga kuta"
			}
		]
	},
	{
		name: "ACOTH",
		description: "Vraa inverzni hiperboliki kotangens broja.",
		arguments: [
			{
				name: "broj",
				description: "je hiperboliki kotangens željenoga kuta"
			}
		]
	},
	{
		name: "ADDRESS",
		description: "Stvara adresu elije kao tekst, ako su zadani brojevi retka i stupca.",
		arguments: [
			{
				name: "broj_retka",
				description: "je broj retka za upotrebu u adresi elije: broj_retka = 1 za 1. redak"
			},
			{
				name: "broj_stupca",
				description: "je broj stupca za upotrebu u adresi elije. Npr. broj_stupca = 4 za stupac D"
			},
			{
				name: "aps_broj",
				description: "navodi vrstu adrese koja se vraa: apsolutna = 1; apsolutni redak/relativni stupac = 2; relativni redak/apsolutni stupac = 3; relativni = 4"
			},
			{
				name: "a1",
				description: "je logika vrijednost koja odreuje vrstu adrese: A1 stil = 1 ili TRUE; R1C1 stil = 0 ili FALSE"
			},
			{
				name: "list_tekst",
				description: "je tekst koji odreuje naziv radnog lista koji e se koristiti kao vanjska adresa"
			}
		]
	},
	{
		name: "AND",
		description: "Provjerava jesu li svi argumenti TRUE i vraa TRUE ako su svi argumenti TRUE.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logika1",
				description: "su 1 do 255 uvjeta koje želite provjeriti, mogu biti ili TRUE ili FALSE, a zadani su kao logike vrijednosti, polja ili reference"
			},
			{
				name: "logika2",
				description: "su 1 do 255 uvjeta koje želite provjeriti, mogu biti ili TRUE ili FALSE, a zadani su kao logike vrijednosti, polja ili reference"
			}
		]
	},
	{
		name: "ARABIC",
		description: "Pretvara rimske brojeve u arapske.",
		arguments: [
			{
				name: "text",
				description: "je rimski broj koji želite pretvoriti"
			}
		]
	},
	{
		name: "AREAS",
		description: "Vraa broj podruja u referenci. Podruje je raspon susjednih elija ili jedna elija.",
		arguments: [
			{
				name: "referenca",
				description: "je referenca na eliju ili raspon elija i može se odnositi na viáe podruja"
			}
		]
	},
	{
		name: "ASIN",
		description: "Vraa arkus-sinus zadanog broja u radijanima u rasponu od -pi/2 do pi/2.",
		arguments: [
			{
				name: "broj",
				description: "je sinus zadanog kuta i mora biti u rasponu od -1 do 1"
			}
		]
	},
	{
		name: "ASINH",
		description: "Vraa inverzni hiperbolni sinus broja.",
		arguments: [
			{
				name: "broj",
				description: "je bilo koji realni broj jednak ili vei od 1"
			}
		]
	},
	{
		name: "ATAN",
		description: "Vraa arkus-tangens broja u radijanima u rasponu od -p/2 do p/2.",
		arguments: [
			{
				name: "broj",
				description: "je tangens zadanog kuta"
			}
		]
	},
	{
		name: "ATAN2",
		description: "Vraa arkus-tangens u oznaenim koordinatama x i y izražen u radijanima u rasponu izmeu -pi i pi, s izuzetkom -pi.",
		arguments: [
			{
				name: "x_broj",
				description: "je koordinata x toke"
			},
			{
				name: "y_broj",
				description: "je koordinata y toke"
			}
		]
	},
	{
		name: "ATANH",
		description: "Vraa inverzni hiperbolni tangens broja.",
		arguments: [
			{
				name: "broj",
				description: "je bilo koji realni broj izmeu -1 i 1 iskljuujui -1 i 1"
			}
		]
	},
	{
		name: "AVEDEV",
		description: "Vraa prosjek apsolutnih odstupanja podataka od njihove srednje vrijednosti. Argumenti mogu biti brojevi ili nazivi, polja ili reference brojeva.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su 1 do 255 argumenata za koje želite srednju vrijednost odstupanja"
			},
			{
				name: "broj2",
				description: "su 1 do 255 argumenata za koje želite srednju vrijednost odstupanja"
			}
		]
	},
	{
		name: "AVERAGE",
		description: "Vraa prosjek (aritmetiku sredinu) argumenata koji mogu biti brojevi ili nazivi, polja ili reference koje sadrže brojeve.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su od 1 do 255 brojanih argumenata za koji tražite prosjek"
			},
			{
				name: "broj2",
				description: "su od 1 do 255 brojanih argumenata za koji tražite prosjek"
			}
		]
	},
	{
		name: "AVERAGEA",
		description: "Vraa prosjek (aritmetiku sredinu) argumenata, vrednujui tekst i FALSE u argumentima kao 0; a TRUE kao 1. Argumenti mogu biti brojevi, nazivi, polja ili reference.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrijednost1",
				description: "su 1 do 255 argumenata za koje želite izraunati prosjek"
			},
			{
				name: "vrijednost2",
				description: "su 1 do 255 argumenata za koje želite izraunati prosjek"
			}
		]
	},
	{
		name: "AVERAGEIF",
		description: "Pronalazi prosjek (aritmetiku sredinu) za elije odreene navedenim uvjetom ili kriterijem.",
		arguments: [
			{
				name: "raspon",
				description: "je raspon elija koje želite provjeriti"
			},
			{
				name: "kriteriji",
				description: "je uvjet ili kriterij u obliku broja, izraza ili teksta, koji odreuje koje elije e se koristiti u odreivanju prosjeka"
			},
			{
				name: "prosjeni_raspon",
				description: "su elije koje e se koristiti za odreivanje prosjeka. Ako se ta vrijednost izostavi, koristit e se elije u rasponu"
			}
		]
	},
	{
		name: "AVERAGEIFS",
		description: "Pronalazi prosjek (aritmetiku sredinu) za elije odreene navedenim skupom uvjeta ili kriterija.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "prosjeni_raspon",
				description: "su elije koje e se koristiti za odreivanje prosjeka."
			},
			{
				name: "raspon_kriterija",
				description: "je raspon elija koje želite provjeriti u odnosu na odreeni uvjet"
			},
			{
				name: "kriteriji",
				description: "je uvjet ili kriterij u obliku broja, izraza ili teksta, koji odreuje koje e elije se koristiti u odreivanju prosjeka"
			}
		]
	},
	{
		name: "BAHTTEXT",
		description: "Pretvara broj u tekst (baht).",
		arguments: [
			{
				name: "broj",
				description: "je broj koji želite pretvoriti"
			}
		]
	},
	{
		name: "BASE",
		description: "Pretvara broj u tekstni prikaz s danim radixom (bazom).",
		arguments: [
			{
				name: "number",
				description: "je broj koji želite pretvoriti"
			},
			{
				name: "radix",
				description: "je temeljni Radix u koji želite pretvoriti broj"
			},
			{
				name: "min_length",
				description: "je najmanja mogua duljina vraenog niza. Ako su ispuátene, vodee se nule ne dodaju"
			}
		]
	},
	{
		name: "BESSELI",
		description: "Vraa izmijenjenu Besselovu funkciju In(x).",
		arguments: [
			{
				name: "x",
				description: "je vrijednost na kojoj se funkcija izraunava"
			},
			{
				name: "n",
				description: "je redoslijed Besselove funkcije"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "Vraa Besselovu funkciju Jn(x).",
		arguments: [
			{
				name: "x",
				description: "je vrijednost na kojoj se funkcija izraunava"
			},
			{
				name: "n",
				description: "je redoslijed Besselove funkcije"
			}
		]
	},
	{
		name: "BESSELK",
		description: "Vraa izmijenjenu Besselovu funkciju Kn(x).",
		arguments: [
			{
				name: "x",
				description: "je vrijednost na kojoj se funkcija izraunava"
			},
			{
				name: "n",
				description: "je redoslijed funkcije"
			}
		]
	},
	{
		name: "BESSELY",
		description: "Vraa Besselovu funkciju Yn(x).",
		arguments: [
			{
				name: "x",
				description: "je vrijednost na kojoj se funkcija izraunava"
			},
			{
				name: "n",
				description: "je redoslijed funkcije"
			}
		]
	},
	{
		name: "BETA.DIST",
		description: "Vraa funkciju distribucije beta-vjerojatnosti.",
		arguments: [
			{
				name: "x",
				description: "je vrijednost izmeu A i B na kojoj se izraunava funkcija"
			},
			{
				name: "alfa",
				description: "je parametar distribucije i mora biti vei od 0"
			},
			{
				name: "beta",
				description: "je parametar distribucije i mora biti vei od 0"
			},
			{
				name: "kumulativna",
				description: "je logika vrijednost: za kumulativnu funkciju distribucije koristite TRUE, za funkciju gustoe vjerojatnosti koristite FALSE"
			},
			{
				name: "A",
				description: "je neobavezna donja granica intervala od x. Ako se ispusti, A = 0"
			},
			{
				name: "B",
				description: "je neobavezna gornja granica intervala od x. Ako se ispusti, B = 1"
			}
		]
	},
	{
		name: "BETA.INV",
		description: "Vraa inverziju kumulativne beta-funkcije gustoe vjerojatnosti (BETA.DIST).",
		arguments: [
			{
				name: "vjerojatnost",
				description: "je vjerojatnost povezana s beta-distribucijom"
			},
			{
				name: "alfa",
				description: "je parametar distribucije i mora biti vei od 0"
			},
			{
				name: "beta",
				description: "je parametar distribucije i mora biti vei od 0"
			},
			{
				name: "A",
				description: "je neobavezna donja granica intervala od x. Ako se izostavi, A = 0"
			},
			{
				name: "B",
				description: "je neobavezna gornja granica intervala od x. Ako se izostavi, B = 1"
			}
		]
	},
	{
		name: "BETADIST",
		description: "Vraa kumulativnu beta-funkciju gustoe vjerojatnosti.",
		arguments: [
			{
				name: "x",
				description: "je vrijednost izmeu A i B za koju se izraunava funkcija"
			},
			{
				name: "alfa",
				description: "je parametar distribucije i mora biti vei od 0"
			},
			{
				name: "beta",
				description: "je parametar distribucije i mora biti vei od 0"
			},
			{
				name: "A",
				description: "je neobavezna donja granica intervala od x. Ako se izostavi, A = 0"
			},
			{
				name: "B",
				description: "je neobavezna gornja granica intervala od x. Ako se izostavi, B = 1"
			}
		]
	},
	{
		name: "BETAINV",
		description: "Vraa inverziju kumulativne beta-funkcije gustoe vjerojatnosti (BETADIST).",
		arguments: [
			{
				name: "vjerojatnost",
				description: "je vjerojatnost povezana s beta-distribucijom"
			},
			{
				name: "alfa",
				description: "je parametar distribucije i mora biti vei od 0"
			},
			{
				name: "beta",
				description: "je parametar distribucije i mora biti vei od 0"
			},
			{
				name: "A",
				description: "je neobavezna donja granica intervala od x. Ako se izostavi, A = 0"
			},
			{
				name: "B",
				description: "je neobavezna gornja granica intervala od x. Ako se izostavi, B = 1"
			}
		]
	},
	{
		name: "BIN2DEC",
		description: "Binarni broj pretvara u decimalni.",
		arguments: [
			{
				name: "broj",
				description: "je binarni broj koji želite pretvoriti"
			}
		]
	},
	{
		name: "BIN2HEX",
		description: "Binarni broj pretvara u heksadecimalni.",
		arguments: [
			{
				name: "broj",
				description: "je binarni broj koji želite pretvoriti"
			},
			{
				name: "mjesta",
				description: "je broj znakova koji e se koristiti"
			}
		]
	},
	{
		name: "BIN2OCT",
		description: "Binarni broj pretvara u oktalni.",
		arguments: [
			{
				name: "broj",
				description: "je binarni broj koji želite pretvoriti"
			},
			{
				name: "mjesta",
				description: "je broj znakova koji e se koristiti"
			}
		]
	},
	{
		name: "BINOM.DIST",
		description: "Vraa vjerojatnost binomne distribucije jednokratnog dogaaja.",
		arguments: [
			{
				name: "broj_s",
				description: "je broj uspjeánih pokuáaja"
			},
			{
				name: "pokuáaji",
				description: "je broj neovisnih pokuáaja"
			},
			{
				name: "vjerojatnost_s",
				description: "je vjerojatnost uspjeha svakog pokuáaja"
			},
			{
				name: "kumulativna",
				description: "je logika vrijednost: za kumulativnu funkciju distribucije koristite TRUE, za funkciju mase vjerojatnosti koristite FALSE"
			}
		]
	},
	{
		name: "BINOM.DIST.RANGE",
		description: "Vraa vjerojatnost pokusnoga rezultata pomou binomske distribucije.",
		arguments: [
			{
				name: "trials",
				description: "je broj neovisnih pokusa"
			},
			{
				name: "probability_s",
				description: "je vjerojatnost uspjeha svakog pokusa"
			},
			{
				name: "number_s",
				description: "je broj uspjeha u pokusima"
			},
			{
				name: "number_s2",
				description: "ako postoji, ova funkcija vraa vjerojatnost da e broj uspjeánih pokusa biti izmeu vrijednosti number_s i number_s2"
			}
		]
	},
	{
		name: "BINOM.INV",
		description: "Vraa najmanju vrijednost za koju kumulativna binomna distribucija nije manja od vrijednosti kriterija.",
		arguments: [
			{
				name: "pokuáaji",
				description: "je broj Bernoullijevih pokuáaja"
			},
			{
				name: "vjerojatnost_s",
				description: "je vjerojatnost uspjeánosti za svaki pokuáaj, broj izmeu 0 i 1, ukljuujui oba ta broja"
			},
			{
				name: "alfa",
				description: "je vrijednost kriterija, broj izmeu 0 i 1, ukljuujui oba ta broja"
			}
		]
	},
	{
		name: "BINOMDIST",
		description: "Vraa vjerojatnost binomne distribucije jednokratnog dogaaja.",
		arguments: [
			{
				name: "broj_s",
				description: "je broj uspjeánih pokuáaja"
			},
			{
				name: "pokuáaji",
				description: "je broj neovisnih pokuáaja"
			},
			{
				name: "vjerojatnost_s",
				description: "je vjerojatnost uspjeha svakog pokuáaja"
			},
			{
				name: "kumulativna",
				description: "je logika vrijednost: za kumulativnu funkciju distribucije koristite TRUE; za funkciju mase vjerojatnosti koristite FALSE"
			}
		]
	},
	{
		name: "BITAND",
		description: "Vraa bitwise 'i' dvaju broja.",
		arguments: [
			{
				name: "number1",
				description: "je decimalni prikaz binarnoga broja koji želite procijeniti"
			},
			{
				name: "number2",
				description: "je decimalni prikaz binarnoga broja koji želite procijeniti"
			}
		]
	},
	{
		name: "BITLSHIFT",
		description: "Vraa broj pomaknut ulijevo za iznos_pomaka bitova.",
		arguments: [
			{
				name: "number",
				description: "je decimalni prikaz binarnoga broja koji želite procijeniti"
			},
			{
				name: "shift_amount",
				description: "je broj bitova za koji želite broj pomaknuti ulijevo"
			}
		]
	},
	{
		name: "BITOR",
		description: "Vraa bitwise 'ili' dvaju broja.",
		arguments: [
			{
				name: "number1",
				description: "je decimalni prikaz binarnoga broja koji želite procijeniti"
			},
			{
				name: "number2",
				description: "je decimalni prikaz binarnoga broja koji želite procijeniti"
			}
		]
	},
	{
		name: "BITRSHIFT",
		description: "Vraa broj pomaknut udesno za iznos_pomaka bitova.",
		arguments: [
			{
				name: "number",
				description: "je decimalni prikaz binarnoga broja koji želite procijeniti"
			},
			{
				name: "shift_amount",
				description: "je broj bitova za koji želite broj pomaknuti udesno"
			}
		]
	},
	{
		name: "BITXOR",
		description: "Vraa bitwise 'ekskluzivno ili' dvaju broja.",
		arguments: [
			{
				name: "number1",
				description: "je decimalni prikaz binarnoga broja koji želite procijeniti"
			},
			{
				name: "number2",
				description: "je decimalni prikaz binarnoga broja koji želite procijeniti"
			}
		]
	},
	{
		name: "CEILING",
		description: "Zaokružuje broj prema gore na najbliži viáekratnik argumenta Znaajnost.",
		arguments: [
			{
				name: "broj",
				description: "je vrijednost koju želite zaokružiti"
			},
			{
				name: "znaajnost",
				description: "je viáekratnik na koji želite zaokružiti"
			}
		]
	},
	{
		name: "CEILING.MATH",
		description: "Zaokružuje broj na najbliži cijeli broj ili na najbliži znaajni viáekratnik.",
		arguments: [
			{
				name: "number",
				description: "je vrijednost koju želite zaokružiti"
			},
			{
				name: "significance",
				description: "je viáekratnik na koji želite zaokružiti"
			},
			{
				name: "mode",
				description: "kada joj se pruži vrijednost koja nije nula, ova e funkcija pri zaokruživanju izbjegavati nulu"
			}
		]
	},
	{
		name: "CEILING.PRECISE",
		description: "Zaokružuje broj prema gore na vrijednost udaljeniju od nule koja je najbliži viáekratnik argumenta znaajnost.",
		arguments: [
			{
				name: "broj",
				description: "je vrijednost koju želite zaokružiti"
			},
			{
				name: "znaajnost",
				description: "je viáekratnik na koji želite zaokružiti"
			}
		]
	},
	{
		name: "CELL",
		description: "Vraa informaciju o oblikovanju, mjestu ili sadržaju prve elije u referenci, s obzirom na redoslijed itanja lista.",
		arguments: [
			{
				name: "informacija_vrsta",
				description: "je tekstna vrijednost koja odreuje  željenu vrstu informacija o eliji."
			},
			{
				name: "referenca",
				description: "je elija o kojoj želite informacije"
			}
		]
	},
	{
		name: "CHAR",
		description: "Vraa znak naveden kodnim brojem iz skupa znakova za vaáe raunalo.",
		arguments: [
			{
				name: "broj",
				description: "je broj izmeu 1 i 255 koji oznaava znak koji želite"
			}
		]
	},
	{
		name: "CHIDIST",
		description: "Vraa desnokraku vjerojatnost hi-kvadrirane distribucije.",
		arguments: [
			{
				name: "x",
				description: "je vrijednost na kojoj želite izraunati distribuciju, broj koji nije negativan"
			},
			{
				name: "stupanj_slobode",
				description: "je broj stupnjeva slobode, broj izmeu 1 i 10^10, ne ukljuujui 10^10"
			}
		]
	},
	{
		name: "CHIINV",
		description: "Vraa inverziju desnokrake vjerojatnosti hi-kvadrirane distribucije.",
		arguments: [
			{
				name: "vjerojatnost",
				description: "je vjerojatnost povezana s hi-kvadriranom distribucijom, vrijednost izmeu 0 i 1, ukljuujui ta dva broja"
			},
			{
				name: "stupanj_slobode",
				description: "je broj stupnjeva slobode, broj izmeu 1 i 10^10, ne ukljuujui 10^10"
			}
		]
	},
	{
		name: "CHISQ.DIST",
		description: "Vraa ljevokraku vjerojatnost hi-kvadrirane distribucije.",
		arguments: [
			{
				name: "x",
				description: "je vrijednost na kojoj želite vrednovati distribuciju, broj koji nije negativan"
			},
			{
				name: "stupanj_slobode",
				description: "je broj stupnjeva slobode, broj izmeu 1 i 10^10, ne ukljuujui 10^10"
			},
			{
				name: "kumulativna",
				description: "je logika funkcija za vraanje: kumulativna funkcija distribucije = TRUE, funkcija gustoe vjerojatnosti = FALSE"
			}
		]
	},
	{
		name: "CHISQ.DIST.RT",
		description: "Vraa desnokraku vjerojatnost hi-kvadrirane distribucije.",
		arguments: [
			{
				name: "x",
				description: "je vrijednost na kojoj želite izraunati distribuciju, broj koji nije negativan"
			},
			{
				name: "stupanj_slobode",
				description: "je broj stupnjeva slobode, broj izmeu 1 i 10^10, ne ukljuujui 10^10"
			}
		]
	},
	{
		name: "CHISQ.INV",
		description: "Vraa inverziju ljevokrake vjerojatnosti hi-kvadrirane distribucije.",
		arguments: [
			{
				name: "vjerojatnost",
				description: "je vjerojatnost povezana s hi-kvadriranom distribucijom, vrijednost izmeu 0 i 1, ukljuujui ta dva broja"
			},
			{
				name: "stupanj_slobode",
				description: "je broj stupnjeva slobode, broj izmeu 1 i 10^10, ne ukljuujui 10^10"
			}
		]
	},
	{
		name: "CHISQ.INV.RT",
		description: "Vraa inverziju desnokrake vjerojatnosti hi-kvadrirane distribucije.",
		arguments: [
			{
				name: "vjerojatnost",
				description: "je vjerojatnost povezana s hi-kvadriranom distribucijom, vrijednost izmeu 0 i 1, ukljuujui ta dva broja"
			},
			{
				name: "stupanj_slobode",
				description: "je broj stupnjeva slobode, broj izmeu 1 i 10^10, ne ukljuujui 10^10"
			}
		]
	},
	{
		name: "CHISQ.TEST",
		description: "Vraa test nezavisnosti: vrijednost hi-kvadrirane distribucije za statistike i odgovarajue stupnjeve slobode.",
		arguments: [
			{
				name: "stvaran_raspon",
				description: "je raspon podataka koji sadrži promatrane vrijednosti za test s oekivanim vrijednostima"
			},
			{
				name: "oekivani_raspon",
				description: "je raspon podataka koji sadrži omjer umnoáka ukupnih vrijednosti u recima i u stupcima te sveukupne vrijednosti"
			}
		]
	},
	{
		name: "CHITEST",
		description: "Vraa test nezavisnosti: vrijednost hi-kvadrirane distribucije za statistike i odgovarajue stupnjeve slobode.",
		arguments: [
			{
				name: "stvarni_raspon",
				description: "je raspon podataka koji sadrži promatrane vrijednosti za test s oekivanim vrijednostima"
			},
			{
				name: "oekivani_raspon",
				description: "je raspon podataka koji sadrži omjer umnoáka ukupnih vrijednosti u recima i u stupcima i sveukupne vrijednosti"
			}
		]
	},
	{
		name: "CHOOSE",
		description: "Na temelju broja indeksa odabire vrijednost ili akciju za izvoenje iz popisa vrijednosti.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "indeks_broj",
				description: "navodi koji je argument vrijednosti izdvojen. Indeks_broj mora biti broj izmeu 1 i 254 ili formula, odnosno referenca na eliju koja sadrži broj izmeu 1 i 254"
			},
			{
				name: "vrijednost1",
				description: "su 1 do 254 broja, referenci elija, definiranih naziva, formula, funkcija ili tekstnih argumenata od kojih e CHOOSE izabrati vrijednost"
			},
			{
				name: "vrijednost2",
				description: "su 1 do 254 broja, referenci elija, definiranih naziva, formula, funkcija ili tekstnih argumenata od kojih e CHOOSE izabrati vrijednost"
			}
		]
	},
	{
		name: "CLEAN",
		description: "Uklanja sve neispisive znakove iz teksta.",
		arguments: [
			{
				name: "tekst",
				description: "je bilo koji podatak iz radnog lista iz kojeg želite ukloniti neispisive znakove"
			}
		]
	},
	{
		name: "CODE",
		description: "Vraa brojani kod za prvi znak u tekstnom nizu za skup znakova koji se koristi na vaáem raunalu.",
		arguments: [
			{
				name: "tekst",
				description: "je tekst za koji tražite kod prvog znaka"
			}
		]
	},
	{
		name: "COLUMN",
		description: "Vraa broj stupca reference.",
		arguments: [
			{
				name: "referenca",
				description: "je elija ili neprekinuti raspon elija za koje želite broj stupca. Ako se ispusti, vraa eliju koja sadrži koriátenu funkciju COLUMN"
			}
		]
	},
	{
		name: "COLUMNS",
		description: "Vraa broj stupaca u polju ili referenci.",
		arguments: [
			{
				name: "polje",
				description: "je polje, formula polja ili referenca na raspon elija za koje želite broj stupaca"
			}
		]
	},
	{
		name: "COMBIN",
		description: "Rauna broj kombinacija za dani broj stavki.",
		arguments: [
			{
				name: "broj",
				description: " je ukupan broj stavki"
			},
			{
				name: "odabrani_broj",
				description: "je broj stavki u svakoj kombinaciji"
			}
		]
	},
	{
		name: "COMBINA",
		description: "Vraa broj kombinacija s ponavljanjem za odreeni broj stavki.",
		arguments: [
			{
				name: "number",
				description: "je ukupan broj stavki"
			},
			{
				name: "number_chosen",
				description: "je broj stavki u svakoj kombinaciji"
			}
		]
	},
	{
		name: "COMPLEX",
		description: "Realne i imaginarne koeficijente pretvara u kompleksni broj.",
		arguments: [
			{
				name: "realni_broj",
				description: "je stvarni koeficijent kompleksnog broja"
			},
			{
				name: "imaginarni_broj",
				description: "je imaginarni koeficijent kompleksnog broja"
			},
			{
				name: "sufiks",
				description: "je sufiks imaginarne komponente kompleksnog broja"
			}
		]
	},
	{
		name: "CONCATENATE",
		description: "Spaja nekoliko tekstnih nizova u jedan tekstni niz.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tekst1",
				description: "su 1 do 255 tekstnih nizova koji se spajaju u jedan a to mogu biti tekstni nizovi, brojevi ili reference pojedinanih elija"
			},
			{
				name: "tekst2",
				description: "su 1 do 255 tekstnih nizova koji se spajaju u jedan a to mogu biti tekstni nizovi, brojevi ili reference pojedinanih elija"
			}
		]
	},
	{
		name: "CONFIDENCE",
		description: "Rauna interval pouzdanosti za srednju vrijednost populacije na temelju normalne distribucije.",
		arguments: [
			{
				name: "alfa",
				description: "je razina znaajnosti koja se koristi za izraun razine pouzdanosti, broj vei od 0 i manji od 1"
			},
			{
				name: "standardna_dev",
				description: "je standardna devijacija populacije za raspon podataka i pretpostavlja se da je poznata. Standardna_dev mora biti vei od 0"
			},
			{
				name: "veliina",
				description: "je veliina uzorka"
			}
		]
	},
	{
		name: "CONFIDENCE.NORM",
		description: "Vraa interval pouzdanosti za srednju vrijednost populacije pomou normalne distribucije.",
		arguments: [
			{
				name: "alfa",
				description: "je razina znaajnosti koja se koristi za izraun razine pouzdanosti, broj vei od 0 i manji od 1"
			},
			{
				name: "standardna_dev",
				description: "je standardna devijacija populacije za raspon podataka i pretpostavlja se da je poznata. Standardna_dev mora biti vei od 0"
			},
			{
				name: "veliina",
				description: "je veliina uzorka"
			}
		]
	},
	{
		name: "CONFIDENCE.T",
		description: "Vraa interval pouzdanosti za srednju vrijednost populacije pomou Studentove T-distribucije.",
		arguments: [
			{
				name: "alfa",
				description: "je razina znaajnosti koja se koristi za izraun razine pouzdanosti, broj vei od 0 i manji od 1"
			},
			{
				name: "standardna_dev",
				description: "je standardna devijacija populacije za raspon podataka i pretpostavlja se da je poznata. Standardna_dev mora biti vei od 0"
			},
			{
				name: "veliina",
				description: "je veliina uzorka"
			}
		]
	},
	{
		name: "CONVERT",
		description: "Pretvara broj iz jednog mjernog sustava u drugi.",
		arguments: [
			{
				name: "broj",
				description: "je vrijednost koja e se pretvarati"
			},
			{
				name: "polazna_jedinca",
				description: "su jedinice za broj"
			},
			{
				name: "ciljna_jedinica",
				description: "su jedinice rezultata"
			}
		]
	},
	{
		name: "CORREL",
		description: "Vraa koeficijent korelacije izmeu dva skupa podataka.",
		arguments: [
			{
				name: "polje1",
				description: "je raspon vrijednosti elija. Vrijednosti bi trebale biti brojevi, nazivi, polja ili reference koje sadrže brojeve"
			},
			{
				name: "polje2",
				description: "je drugi raspon vrijednosti elija. Vrijednosti bi trebale biti brojevi, nazivi, polja ili reference koje sadrže brojeve"
			}
		]
	},
	{
		name: "COS",
		description: "Vraa kosinus zadanog kuta.",
		arguments: [
			{
				name: "broj",
				description: "je kut u radijanima za koji tražite kosinus"
			}
		]
	},
	{
		name: "COSH",
		description: "Vraa hiperbolni kosinus broja.",
		arguments: [
			{
				name: "broj",
				description: "je bilo koji realni broj"
			}
		]
	},
	{
		name: "COT",
		description: "Vraa kotangens kuta.",
		arguments: [
			{
				name: "broj",
				description: "je kut u radijanima za koji želite kotangens"
			}
		]
	},
	{
		name: "COTH",
		description: "Vraa hiperboliki kotangens broja.",
		arguments: [
			{
				name: "broj",
				description: "je kut u radijanima za koji želite hiperboliki kotangens"
			}
		]
	},
	{
		name: "COUNT",
		description: "Broji elije u rasponu koji sadrži brojeve.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrijednost1",
				description: "su 1 do 255 argumenata koji mogu sadržavati ili se odnositi na mnogo razliitih vrsta podataka, ali se broje samo brojevi"
			},
			{
				name: "vrijednost2",
				description: "su 1 do 255 argumenata koji mogu sadržavati ili se odnositi na mnogo razliitih vrsta podataka, ali se broje samo brojevi"
			}
		]
	},
	{
		name: "COUNTA",
		description: "Broji elije u nizu koje nisu prazne.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrijednost1",
				description: "su 1 do 255 argumenata koje predstavljaju vrijednosti koje želite prebrojiti. Vrijednosti mogu biti bilo koja vrsta podataka"
			},
			{
				name: "vrijednost2",
				description: "su 1 do 255 argumenata koje predstavljaju vrijednosti koje želite prebrojiti. Vrijednosti mogu biti bilo koja vrsta podataka"
			}
		]
	},
	{
		name: "COUNTBLANK",
		description: "Broji prazne elije u navedenom rasponu elija.",
		arguments: [
			{
				name: "raspon",
				description: "je raspon u kojem želite prebrojiti prazne elije"
			}
		]
	},
	{
		name: "COUNTIF",
		description: "Broji elije koje unutar zadanog raspona ispunjavaju zadani kriterij.",
		arguments: [
			{
				name: "raspon",
				description: "je raspon elija u kojem želite prebrojiti neprazne elije"
			},
			{
				name: "kriteriji",
				description: "je uvjet u obliku broja, izraza ili teksta koji definira koje e elije biti prebrojene"
			}
		]
	},
	{
		name: "COUNTIFS",
		description: "Broji elije odreene danim skupom uvjeta ili kriterija.",
		arguments: [
			{
				name: "raspon_kriterija",
				description: "je raspon elija koji želite provjeriti u odnosu na odreeni uvjet"
			},
			{
				name: "kriteriji",
				description: "je uvjet u obliku broja, izraza ili teksta, koji odreuje koje elije e se brojati"
			}
		]
	},
	{
		name: "COUPDAYBS",
		description: "Vraa broj dana od poetka kuponskog razdoblja do datuma plaanja.",
		arguments: [
			{
				name: "isplata",
				description: "je datum plaanja vrijednosnice, izražen kao serijski broj datuma"
			},
			{
				name: "dospijee",
				description: "je datum dospijea vrijednosnice, izražen kao serijski broj datuma"
			},
			{
				name: "uestalost",
				description: "je godiánji broj kuponskih isplata"
			},
			{
				name: "temelj",
				description: "je vrsta brojanja dana koja se koristi"
			}
		]
	},
	{
		name: "COUPNCD",
		description: "Vraa datum sljedeeg kuponskog razdoblja nakon datuma plaanja.",
		arguments: [
			{
				name: "isplata",
				description: "je datum plaanja vrijednosnice, izražen kao serijski broj datuma"
			},
			{
				name: "dospijee",
				description: "je datum dospijea vrijednosnice, izražen kao serijski broj datuma"
			},
			{
				name: "uestalost",
				description: "je godiánji broj kuponskih isplata"
			},
			{
				name: "temelj",
				description: "je vrsta brojanja dana koja se koristi"
			}
		]
	},
	{
		name: "COUPNUM",
		description: "Vraa broj kupona dospjelih izmeu datuma plaanja i datuma dospijea.",
		arguments: [
			{
				name: "isplata",
				description: "je datum plaanja vrijednosnice, izražen kao serijski broj datuma"
			},
			{
				name: "dospijee",
				description: "je datum dospijea vrijednosnice, izražen kao serijski broj datuma"
			},
			{
				name: "uestalost",
				description: "je godiánji broj kuponskih isplata"
			},
			{
				name: "temelj",
				description: "je vrsta brojanja dana koja se koristi"
			}
		]
	},
	{
		name: "COUPPCD",
		description: "Vraa datum prethodne kuponske isplate prije datuma plaanja.",
		arguments: [
			{
				name: "isplata",
				description: "je datum plaanja vrijednosnice, izražen kao serijski broj datuma"
			},
			{
				name: "dospijee",
				description: "je datum dospijea vrijednosnice, izražen kao serijski broj datuma"
			},
			{
				name: "uestalost",
				description: "je godiánji broj kuponskih isplata"
			},
			{
				name: "temelj",
				description: "je vrsta brojanja dana koja se koristi"
			}
		]
	},
	{
		name: "COVAR",
		description: "Vraa kovarijancu, prosjek umnožaka devijacija za svaki par toaka u skupu podataka.",
		arguments: [
			{
				name: "polje1",
				description: "je prvi raspon elija s cijelim brojevima i mora sadržavati brojeve, polja ili reference koje sadržavaju brojeve"
			},
			{
				name: "polje2",
				description: "je drugi raspon elija s cijelim brojevima i mora sadržavati brojeve, polja ili reference koje sadržavaju brojeve"
			}
		]
	},
	{
		name: "COVARIANCE.P",
		description: "Vraa kovarijancu populacije, prosjenu vrijednost umnožaka devijacija za svaki par toaka podataka u dva skupa podataka.",
		arguments: [
			{
				name: "array1",
				description: "je prvi raspon elija s cijelim brojevima, a to moraju biti brojevi, polja ili reference koje sadrže brojeve"
			},
			{
				name: "array2",
				description: "je drugi raspon elija s cijelim brojevima, a to moraju biti brojevi, polja ili reference koje sadrže brojeve"
			}
		]
	},
	{
		name: "COVARIANCE.S",
		description: "Vraa kovarijancu uzorka, prosjek umnožaka devijacija za svaki par toaka podataka u dva skupa podataka.",
		arguments: [
			{
				name: "array1",
				description: "je prvi raspon elija s cijelim brojevima, a to moraju biti brojevi, polja ili reference koje sadrže brojeve"
			},
			{
				name: "array2",
				description: "je drugi raspon elija s cijelim brojevima, a to moraju biti brojevi, polja ili reference koje sadrže brojeve"
			}
		]
	},
	{
		name: "CRITBINOM",
		description: "Vraa najmanju vrijednost za koju kumulativna binomna distribucija nije manja od vrijednosti kriterija.",
		arguments: [
			{
				name: "pokuáaji",
				description: "je broj Bernoullijevih pokuáaja"
			},
			{
				name: "vjerojatnost_s",
				description: "je vjerojatnost uspjeánosti za svaki pokuáaj, broj izmeu 0 i 1, ukljuujui oba ta broja"
			},
			{
				name: "alfa",
				description: "je vrijednost kriterija, broj izmeu 0 i 1, ukljuujui oba ta broja"
			}
		]
	},
	{
		name: "CSC",
		description: "Vraa kosekans kuta.",
		arguments: [
			{
				name: "broj",
				description: "je kut u radijanima za koji želite kosekans"
			}
		]
	},
	{
		name: "CSCH",
		description: "Vraa hiperboliki kosekans kuta.",
		arguments: [
			{
				name: "broj",
				description: "je kut u radijanima za koji želite hiperboliki kosekans"
			}
		]
	},
	{
		name: "CUMIPMT",
		description: "Vraa kumulativnu kamatnu stopu plaenu po zajmu izmeu dva razdoblja.",
		arguments: [
			{
				name: "stopa",
				description: "je kamatna stopa"
			},
			{
				name: "brrazd",
				description: "je ukupni broj razdoblja plaanja"
			},
			{
				name: "sv",
				description: "je trenutna vrijednost"
			},
			{
				name: "poetno_razdoblje",
				description: "je prvo razdoblje izrauna"
			},
			{
				name: "zavráno_razdoblje",
				description: "je posljednje razdoblje izrauna"
			},
			{
				name: "vrsta",
				description: "je obraun vremena plaanja"
			}
		]
	},
	{
		name: "CUMPRINC",
		description: "Vraa kumulativnu glavnicu zajma plaenu izmeu dva razdoblja.",
		arguments: [
			{
				name: "stopa",
				description: "je kamatna stopa"
			},
			{
				name: "brrazd",
				description: "je ukupni broj razdoblja plaanja"
			},
			{
				name: "sv",
				description: "je trenutna vrijednost"
			},
			{
				name: "poetno_razdoblje",
				description: "je prvo razdoblje izrauna"
			},
			{
				name: "zavráno_razdoblje",
				description: "je posljednje razdoblje izrauna"
			},
			{
				name: "vrsta",
				description: "je obraun vremena plaanja"
			}
		]
	},
	{
		name: "DATE",
		description: "Vraa broj koji predstavlja datum u kodu datuma i vremena programa Spreadsheet.",
		arguments: [
			{
				name: "godina",
				description: "je broj od 1900 do 9999 u programu Spreadsheet za Windows ili od 1904 do 9999 u programu Spreadsheet za Macintosh"
			},
			{
				name: "mjesec",
				description: "je broj od 1 do 12 koji predstavlja mjesec u godini"
			},
			{
				name: "dan",
				description: "je broj od 1 do 31 koji predstavlja dan u mjesecu"
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
		description: "Pretvara datum iz tekstnog oblika u broj koji u programu Spreadsheet predstavlja datum u kodu datuma i vremena.",
		arguments: [
			{
				name: "datum_tekst",
				description: "je tekst koji u obliku datuma programa Spreadsheet predstavlja datum izmeu 1. 1. 1900. (Windows) ili 1. 1. 1904. (Macintosh) i 31. 12. 9999."
			}
		]
	},
	{
		name: "DAVERAGE",
		description: "Rauna prosjek vrijednosti u stupcu s popisa ili iz baze podataka koja ispunjava zadane uvjete.",
		arguments: [
			{
				name: "bazapodataka",
				description: "je raspon elija koji tvori popis ili bazu podataka. Baza podataka je popis povezanih podataka"
			},
			{
				name: "polje",
				description: "je natpis stupca u dvostrukim navodnicima ili broj koji predstavlja smjeátaj stupca unutar popisa"
			},
			{
				name: "kriteriji",
				description: "je raspon elija koje sadrže navedene uvjete. Raspon sadrži natpis stupca i jednu eliju ispod natpisa stupca za svaki uvjet"
			}
		]
	},
	{
		name: "DAY",
		description: "Vraa dan u mjesecu kao broj od 1 do 31.",
		arguments: [
			{
				name: "redni_broj",
				description: "je broj u kodu datuma i vremena koji koristi Spreadsheet"
			}
		]
	},
	{
		name: "DAYS",
		description: "Vraa broj dana izmeu dva datuma.",
		arguments: [
			{
				name: "end_date",
				description: "start_date i end_date su dva datuma izmeu kojih želite prebrojati dane"
			},
			{
				name: "start_date",
				description: "start_date i end_date su dva datuma izmeu kojih želite prebrojati dane"
			}
		]
	},
	{
		name: "DAYS360",
		description: "Vraa broj dana izmeu dva datuma temeljenih na godini od 360 dana (dvanaest mjeseci po 30 dana).",
		arguments: [
			{
				name: "poetni_datum",
				description: "poetni_datum i zavráni_datum su dva datuma izmeu kojih želite znati broj dana"
			},
			{
				name: "zavráni_datum",
				description: "poetni_datum i zavráni_datum su dva datuma izmeu kojih želite znati broj dana"
			},
			{
				name: "metoda",
				description: "je logika vrijednost koja navodi metodu izrauna: SAD (NASD) = FALSE ili ispuátena; Europska = TRUE."
			}
		]
	},
	{
		name: "DB",
		description: "Vraa amortizaciju imovine za navedeno razdoblje, koristei metodu amortizacije fiksnom stopom.",
		arguments: [
			{
				name: "cijena",
				description: "je poetna cijena imovine"
			},
			{
				name: "likvidacija",
				description: "je vrijednost imovine na kraju amortizacije"
			},
			{
				name: "vijek",
				description: "je broj razdoblja tijekom kojih se imovine amortizira (ponekad se naziva vrijeme upotrebljivosti imovine)"
			},
			{
				name: "razdoblje",
				description: "je razdoblje za koje želite izraunati amortizaciju. Razdoblje mora koristiti iste jedinice kao i vijek"
			},
			{
				name: "mjesec",
				description: "je broj mjeseci u prvoj godini. Ako je ispuáten, pretpostavlja se da je 12"
			}
		]
	},
	{
		name: "DCOUNT",
		description: "Broji elije u stupcu popisa ili baze podataka koje sadrže brojeve koji ispunjavaju zadane uvjete.",
		arguments: [
			{
				name: "bazapodataka",
				description: "je raspon elija koji tvori popis ili bazu podataka. Baza podataka je popis povezanih podataka"
			},
			{
				name: "polje",
				description: "je ili tekst s natpisom stupca u dvostrukim navodnicima ili broj koji predstavlja smjeátaj stupca unutar popisa"
			},
			{
				name: "kriteriji",
				description: "je raspon elija koji sadrži uvjete koje ste odredili. Raspon sadrži natpis stupca i jednu eliju ispod natpisa stupca za svaki uvjet"
			}
		]
	},
	{
		name: "DCOUNTA",
		description: "Broji sve elije u polju (stupcu) slogova u bazi podataka koje nisu prazne i koje ispunjavaju zadane uvjete.",
		arguments: [
			{
				name: "bazapodataka",
				description: "je raspon elija koji ine popis ili bazu podataka. Baza podataka je popis povezanih podataka"
			},
			{
				name: "polje",
				description: "je natpis stupca u dvostrukim navodnicima ili broj koji predstavlja položaj stupca u popisu"
			},
			{
				name: "kriteriji",
				description: "je raspon elija koji sadrži uvjete koje ste odredili. Raspon sadrži natpis stupca i jednu eliju ispod natpisa stupca za odreivanje uvjeta za stupac"
			}
		]
	},
	{
		name: "DDB",
		description: "Izraunava amortizaciju imovine za odreeno razdoblje, metodom ubrzane deprecijacije ili nekom drugom metodom koju navedete.",
		arguments: [
			{
				name: "cijena",
				description: "je poetna cijena imovine"
			},
			{
				name: "likvidacija",
				description: "je vrijednost imovine na kraju amortizacije"
			},
			{
				name: "vijek",
				description: "je broj razdoblja tijekom kojih se imovine amortizira (ponekad se naziva vrijeme upotrebljivosti imovine)"
			},
			{
				name: "razdoblje",
				description: "je razdoblje za koje želite izraunati amortizaciju. Razdoblje mora koristiti iste jedinice kao i vijek"
			},
			{
				name: "faktor",
				description: "je stopa po kojoj se umanjuje knjigovodstvena vrijednost opreme. Ako je faktor ispuáten, pretpostavlja se da je 2 (metoda ubrzane deprecijacije)"
			}
		]
	},
	{
		name: "DEC2BIN",
		description: "Decimalni broj pretvara u binarni.",
		arguments: [
			{
				name: "broj",
				description: "je decimalni cijeli broj koji želite pretvoriti"
			},
			{
				name: "mjesta",
				description: "je broj znakova koji e se koristiti"
			}
		]
	},
	{
		name: "DEC2HEX",
		description: "Decimalni broj pretvara u heksadecimalni.",
		arguments: [
			{
				name: "broj",
				description: "je decimalni cijeli broj koji želite pretvoriti"
			},
			{
				name: "mjesta",
				description: "je broj znakova koji e se koristiti"
			}
		]
	},
	{
		name: "DEC2OCT",
		description: "Decimalni broj pretvara u oktalni.",
		arguments: [
			{
				name: "broj",
				description: "je decimalni cijeli broj koji želite pretvoriti"
			},
			{
				name: "mjesta",
				description: "je broj znakova koji e se koristiti"
			}
		]
	},
	{
		name: "DECIMAL",
		description: "Pretvara tekstni prikaz broja u sklopu dane baze u decimalni broj.",
		arguments: [
			{
				name: "number",
				description: "je broj koji želite pretvoriti"
			},
			{
				name: "radix",
				description: "je bazni Radix broja koji pretvarate"
			}
		]
	},
	{
		name: "DEGREES",
		description: "Pretvara radijane u stupnjeve.",
		arguments: [
			{
				name: "kut",
				description: "je kut koji želite pretvoriti, zadan u radijanima"
			}
		]
	},
	{
		name: "DELTA",
		description: "Provjerava jesu li dva broja jednaka.",
		arguments: [
			{
				name: "broj1",
				description: "je prvi broj"
			},
			{
				name: "broj2",
				description: "je drugi broj"
			}
		]
	},
	{
		name: "DEVSQ",
		description: "Vraa zbroj kvadrata odstupanja podataka od njihove srednje vrijednosti uzorka.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su 1 do 255 argumenata ili polje ili referenca polja za koje želite izraunati DEVSQ"
			},
			{
				name: "broj2",
				description: "su 1 do 255 argumenata ili polje ili referenca polja za koje želite izraunati DEVSQ"
			}
		]
	},
	{
		name: "DGET",
		description: "Izdvaja pojedinanu vrijednost iz stupca na popisu ili u bazi podataka koja ispunjava zadane uvjete.",
		arguments: [
			{
				name: "bazapodataka",
				description: "je raspon elija koji tvori popis ili bazu podataka. Baza podataka je popis povezanih podataka"
			},
			{
				name: "polje",
				description: "je ili natpis stupca u dvostrukim navodnicima ili broj koji predstavlja položaj stupca u popisu"
			},
			{
				name: "kriteriji",
				description: "je raspon elija koje sadrže uvijete koje ste naveli. Raspon sadrži natpis stupca i jednu eliju ispod natpisa stupca za odreivanje uvjeta za stupac"
			}
		]
	},
	{
		name: "DISC",
		description: "Vraa diskontnu stopu za vrijednosnicu.",
		arguments: [
			{
				name: "isplata",
				description: "je datum plaanja vrijednosnice, izražen kao serijski broj datuma"
			},
			{
				name: "dospijee",
				description: "je datum dospijea vrijednosnice, izražen kao serijski broj datuma"
			},
			{
				name: "cijena",
				description: "je cijena vrijednosnice za svakih 100 kn njezine nominalne vrijednosti"
			},
			{
				name: "otkup",
				description: "je vrijednost po dospijeu za svakih 100 kn nominalne vrijednosti"
			},
			{
				name: "temelj",
				description: "je vrsta brojanja dana koja se koristi"
			}
		]
	},
	{
		name: "DMAX",
		description: "Pronalazi najvei broj u polju (stupcu) slogova baze podataka koji ispunjava zadani uvjet.",
		arguments: [
			{
				name: "bazapodataka",
				description: "je raspon elija koji tvori popis ili bazu podataka. Baza podataka je popis povezanih podataka"
			},
			{
				name: "polje",
				description: "je ili natpis stupca u dvostrukim navodnicima ili broj koji predstavlja smjeátaj stupca unutar popisa"
			},
			{
				name: "kriteriji",
				description: "je raspon elija koji sadrži navedene uvjete. Raspon sadrži jedan natpis stupca i jednu eliju ispod natpisa stupca za svaki uvjet"
			}
		]
	},
	{
		name: "DMIN",
		description: "Vraa najmanji broj u polju (stupcu) slogova u bazi podataka koji ispunjava uvjete koje ste naveli.",
		arguments: [
			{
				name: "bazapodataka",
				description: "je raspon elija koji tvori popis ili bazu podataka. Baza podataka je popis povezanih podataka"
			},
			{
				name: "polje",
				description: "je ili natpis stupca u dvostrukim navodnicima ili broj koji predstavlja smjeátaj stupca unutar popisa"
			},
			{
				name: "kriteriji",
				description: "je raspon elija koje sadrže navedene uvjete. Raspon sadrži natpis stupca i jednu eliju ispod natpisa stupca za svaki uvjet"
			}
		]
	},
	{
		name: "DOLLAR",
		description: "Pretvara broj u tekst koristei valutni oblik.",
		arguments: [
			{
				name: "broj",
				description: "je broj, referenca na eliju koja sadrži broj ili formula koja daje brojani rezultat"
			},
			{
				name: "decimale",
				description: "je broj znamenki desno od decimalnog zareza. Broj se po potrebi zaokružuje; ako ispustite argument decimala, pretpostavlja se da je 2"
			}
		]
	},
	{
		name: "DOLLARDE",
		description: "Dolarsku vrijednost izraženu u obliku razlomka pretvara u vrijednost izraženu u decimalnom obliku.",
		arguments: [
			{
				name: "dolar_razlomak",
				description: "je broj u obliku razlomka"
			},
			{
				name: "razlomak",
				description: "je cjelobrojna vrijednost koja e se koristiti kao nazivnik razlomka"
			}
		]
	},
	{
		name: "DOLLARFR",
		description: "pretvara cijenu u dolarima izraženu kao decimalni broj u cijenu u dolarima izraženu kao razlomak.",
		arguments: [
			{
				name: "dolar_decimalni",
				description: "je decimalni broj"
			},
			{
				name: "razlomak",
				description: "je cjelobrojna vrijednost koja e se koristiti kao nazivnik razlomka"
			}
		]
	},
	{
		name: "DPRODUCT",
		description: "Množi vrijednosti u polju (stupcu) zapisa u bazi podataka koje ispunjavaju zadane uvjete.",
		arguments: [
			{
				name: "bazapodataka",
				description: "je raspon elija koji tvori popis ili bazu podataka. Baza podataka je popis povezanih podataka"
			},
			{
				name: "polje",
				description: "je ili natpis stupca u dvostrukim navodnicima ili broj koji predstavlja položaj stupca u popisu"
			},
			{
				name: "kriteriji",
				description: "je raspon elija koji sadrži uvjete koje ste odredili. Raspon sadrži natpis stupca i jednu eliju ispod natpisa stupca za odreivanje uvjeta za stupac"
			}
		]
	},
	{
		name: "DSTDEV",
		description: "Procjenjuje standardnu devijaciju populacije prema uzorku na temelju odabranih stavki iz baze podataka.",
		arguments: [
			{
				name: "bazapodataka",
				description: "je raspon elija koji tvori popis ili bazu podataka. Baza podataka je popis povezanih podataka"
			},
			{
				name: "polje",
				description: "je ili natpis stupca u dvostrukim navodnicima ili broj koji predstavlja smjeátaj stupca unutar popisa"
			},
			{
				name: "kriteriji",
				description: "je raspon elija koji sadrži navedene uvjete. Raspon sadrži natpis stupca i jednu eliju ispod natpisa stupca za svaki uvjet"
			}
		]
	},
	{
		name: "DSTDEVP",
		description: "Izraunava standardnu devijaciju na temelju cijele populacije odabranih stavki baze podataka.",
		arguments: [
			{
				name: "bazapodataka",
				description: "je raspon elija koji tvori popis ili bazu podataka. Baza podataka je popis povezanih podataka"
			},
			{
				name: "polje",
				description: "je natpis stupca u dvostrukim navodnicima ili broj koji predstavlja položaj stupca u popisu"
			},
			{
				name: "kriteriji",
				description: "je raspon elija koji sadrži uvjete koje ste odredili. Raspon sadrži natpis stupca i jednu eliju ispod natpisa stupca za odreivanje uvjeta za stupac"
			}
		]
	},
	{
		name: "DSUM",
		description: "Zbraja brojeve u polju (stupcu) zapisa u bazi podataka koji ispunjavaju zadane uvjete.",
		arguments: [
			{
				name: "bazapodataka",
				description: "je raspon elija koji tvori popis ili bazu podataka. Baza podataka je popis povezanih podataka"
			},
			{
				name: "polje",
				description: "je ili natpis stupca u dvostrukim navodnicima ili broj koji predstavlja smjeátaj stupca unutar popisa"
			},
			{
				name: "kriteriji",
				description: "je raspon elija koji sadrži uvjete koje ste odredili. Raspon sadrži natpis stupca i jednu eliju ispod natpisa stupca za svaki uvjet"
			}
		]
	},
	{
		name: "DVAR",
		description: "Procjenjuje varijancu populacije prema uzorku na temelju odabranih stavki iz baze podataka.",
		arguments: [
			{
				name: "bazapodataka",
				description: "je raspon elija koji tvori popis ili bazu podataka. Baza podataka je popis povezanih podataka"
			},
			{
				name: "polje",
				description: "je natpis stupca u dvostrukim navodnicima ili broj koji predstavlja smjeátaj stupca unutar popisa"
			},
			{
				name: "kriteriji",
				description: "je raspon elija koji sadrži navedene uvjete. Raspon sadrži natpis stupca i jednu eliju ispod natpisa stupca za svaki uvjet"
			}
		]
	},
	{
		name: "DVARP",
		description: "Rauna varijancu populacije na temelju cijele populacije odabranih stavki baze podataka.",
		arguments: [
			{
				name: "bazapodataka",
				description: "je raspon elija koji ine popis ili bazu podataka. Baza podataka je popis povezanih podataka"
			},
			{
				name: "polje",
				description: "je natpis stupca u dvostrukim navodnicima ili broj koji predstavlja položaj stupca u popisu"
			},
			{
				name: "kriteriji",
				description: "je raspon elija koji sadrži uvjete koje ste odredili. Raspon sadrži natpis stupca i jednu eliju ispod natpisa stupca za odreivanje uvjeta za stupac"
			}
		]
	},
	{
		name: "EDATE",
		description: "Vraa serijski broj datuma koji se nalazi u naznaenom broj mjeseci prije ili poslije poetnog datuma.",
		arguments: [
			{
				name: "poetni_datum",
				description: "je serijski broj datuma koji oznaava poetni datum"
			},
			{
				name: "mjeseci",
				description: "je broj mjeseci prije ili poslije poetnog datuma"
			}
		]
	},
	{
		name: "EFFECT",
		description: "Vraa efektivnu godiánju kamatnu stopu.",
		arguments: [
			{
				name: "nominalna_stopa",
				description: "je nominalna kamatna stopa"
			},
			{
				name: "brrazdgod",
				description: "je broj razdoblja u godini"
			}
		]
	},
	{
		name: "ENCODEURL",
		description: "Vraa niz sa áifriranim URL-om .",
		arguments: [
			{
				name: "text",
				description: "niz je u kojem e se URL áifrirati"
			}
		]
	},
	{
		name: "EOMONTH",
		description: "Vraa serijski broj posljednjeg dana u mjesecu prije ili poslije odreenog broja mjeseci.",
		arguments: [
			{
				name: "poetni_datum",
				description: "je serijski broj datuma koji oznaava poetni datum"
			},
			{
				name: "mjeseci",
				description: "je broj mjeseci prije ili poslije poetnog datuma"
			}
		]
	},
	{
		name: "ERF",
		description: "Vraa funkciju pogreáke.",
		arguments: [
			{
				name: "donja_granica",
				description: "je donja granica za integraciju ERF"
			},
			{
				name: "gornja_granica",
				description: "je gornja granica za integraciju ERF"
			}
		]
	},
	{
		name: "ERF.PRECISE",
		description: "Vraa funkciju pogreáke.",
		arguments: [
			{
				name: "X",
				description: "je donja granica za integraciju ERF.PRECISE"
			}
		]
	},
	{
		name: "ERFC",
		description: "Vraa dopunsku funkciju pogreáke.",
		arguments: [
			{
				name: "x",
				description: "je donja granica za integraciju ERF"
			}
		]
	},
	{
		name: "ERFC.PRECISE",
		description: "Vraa komplementarnu funkciju pogreáke.",
		arguments: [
			{
				name: "X",
				description: "je donja granica za integraciju ERFC.PRECISE"
			}
		]
	},
	{
		name: "ERROR.TYPE",
		description: "Vraa broj koji odgovara vrijednosti pogreáke.",
		arguments: [
			{
				name: "vrijednost_pogreáke",
				description: "je vrijednost pogreáke iji identifikacijski broj želite pronai, a može biti stvarna vrijednost pogreáke ili referenca elije koja sadrži vrijednost pogreáke"
			}
		]
	},
	{
		name: "EVEN",
		description: "Pozitivan broj zaokružuje na najbliži vei, a negativan broj na najbliži manji parni cijeli broj.",
		arguments: [
			{
				name: "broj",
				description: "je vrijednost koja se zaokružuje"
			}
		]
	},
	{
		name: "EXACT",
		description: "Provjerava jesu li dva tekstna niza jednaka i kao rezultat daje TRUE ili FALSE. EXACT razlikuje velika i mala slova.",
		arguments: [
			{
				name: "tekst1",
				description: "je prvi tekstni niz"
			},
			{
				name: "tekst2",
				description: "je drugi tekstni niz"
			}
		]
	},
	{
		name: "EXP",
		description: "Vraa e na potenciju zadanog broja.",
		arguments: [
			{
				name: "broj",
				description: "je eksponent primijenjen na bazu e. Konstanta e je 2,71828182845904, baza prirodnog logaritma"
			}
		]
	},
	{
		name: "EXPON.DIST",
		description: "Vraa eksponencijalnu distribuciju.",
		arguments: [
			{
				name: "x",
				description: "je vrijednost funkcije, pozitivan broj"
			},
			{
				name: "lambda",
				description: "je vrijednost parametra, pozitivan broj"
			},
			{
				name: "kumulativna",
				description: "je logika vrijednost koja pokazuje koji oblik eksponencijalne funkcije treba vratiti: kumulativna funkcija distribucije = TRUE, funkcija gustoe vjerojatnosti = FALSE"
			}
		]
	},
	{
		name: "EXPONDIST",
		description: "Rauna eksponencijalnu distribuciju.",
		arguments: [
			{
				name: "x",
				description: "je vrijednost funkcije, pozitivan broj"
			},
			{
				name: "lambda",
				description: "je vrijednost parametra, pozitivan broj"
			},
			{
				name: "kumulativna",
				description: "je logika vrijednost koju funkcija treba vratiti: kumulativna funkcija distribucije = TRUE; funkcija gustoe vjerojatnosti = FALSE"
			}
		]
	},
	{
		name: "F.DIST",
		description: "Vraa (ljevokraku) distribuciju F vjerojatnosti (stupanj diversifikacije) za dva skupa podataka.",
		arguments: [
			{
				name: "x",
				description: "je vrijednost na kojoj se izraunava funkcija, broj koji nije negativan"
			},
			{
				name: "stupanj_slobode1",
				description: "je broj stupnjeva slobode brojnika, broj izmeu 1 i 10^10, ne ukljuujui 10^10"
			},
			{
				name: "stupanj_slobode2",
				description: "je broj stupnjeva slobode nazivnika, broj izmeu 1 i 10^10, ne ukljuujui 10^10"
			},
			{
				name: "kumulativna",
				description: "je logika vrijednost koju vraa funkcija: kumulativna funkcija distribucije = TRUE, funkcija gustoe vjerojatnosti = FALSE"
			}
		]
	},
	{
		name: "F.DIST.RT",
		description: "Vraa (desnokraku) distribuciju F vjerojatnosti (stupanj diversifikacije) za dva skupa podataka.",
		arguments: [
			{
				name: "x",
				description: "je vrijednost na kojoj se izraunava funkcija, broj koji nije negativan"
			},
			{
				name: "stupanj_slobode1",
				description: "je broj stupnjeva slobode brojnika, broj izmeu 1 i 10^10, ne ukljuujui 10^10"
			},
			{
				name: "stupanj_slobode2",
				description: "je broj stupnjeva slobode nazivnika, broj izmeu 1 i 10^10, ne ukljuujui 10^10"
			}
		]
	},
	{
		name: "F.INV",
		description: "Vraa inverziju (ljevokrake) distribucije F vjerojatnosti: ako je p = F.DIST(x,...), tada je F.INV(p,...) = x.",
		arguments: [
			{
				name: "vjerojatnost",
				description: "je vjerojatnost povezana s kumulativnom F distribucijom, broj izmeu 0 i 1, ukljuujui oba ta broja"
			},
			{
				name: "stupanj_slobode1",
				description: "je stupanj slobode brojnika, broj izmeu 1 i 10^10, ne ukljuujui 10^10"
			},
			{
				name: "stupanj_slobode2",
				description: "je stupanj slobode nazivnika, broj izmeu 1 i 10^10, ne ukljuujui 10^10"
			}
		]
	},
	{
		name: "F.INV.RT",
		description: "Vraa inverziju (desnokrake) distribucije F vjerojatnosti: ako je  p = F.DIST.RT(x,...), onda je F.INV.RT(p,...) = x.",
		arguments: [
			{
				name: "vjerojatnost",
				description: "je vjerojatnost povezana s kumulativnom F distribucijom, broj izmeu 0 i 1, ukljuujui ta dva broja"
			},
			{
				name: "stupanj_slobode1",
				description: "je broj stupnjeva slobode brojnika, broj izmeu 1 i 10^10, ne ukljuujui 10^10"
			},
			{
				name: "stupanj_slobode2",
				description: "je broj stupnjeva slobode nazivnika, broj izmeu 1 i 10^10, ne ukljuujui 10^10"
			}
		]
	},
	{
		name: "F.TEST",
		description: "Vraa vrijednost F-testa, dvokraku vjerojatnost da se varijance u argumentima Polje1 i Polje2 znaajno ne razlikuju.",
		arguments: [
			{
				name: "polje1",
				description: "je prvo polje ili raspon podataka, a to mogu biti brojevi ili nazivi, polja ili reference koje sadrže brojeve (praznine se zanemaruju)"
			},
			{
				name: "polje2",
				description: "je drugo polje ili raspon podataka, a to mogu biti brojevi ili nazivi, polja ili reference koje sadrže brojeve (praznine se zanemaruju)"
			}
		]
	},
	{
		name: "FACT",
		description: "Vraa faktorijel broja jednak 1*2*3*...* broj.",
		arguments: [
			{
				name: "broj",
				description: "je pozitivan broj iji faktorijel želite"
			}
		]
	},
	{
		name: "FACTDOUBLE",
		description: "Vraa dvostruki umnožak broja.",
		arguments: [
			{
				name: "broj",
				description: "je vrijednost za koju e se vratiti dvostruki umnožak"
			}
		]
	},
	{
		name: "FALSE",
		description: "Vraa logiku vrijednost FALSE.",
		arguments: [
		]
	},
	{
		name: "FDIST",
		description: "Vraa (desnokraku) distribuciju F vjerojatnosti (stupnjeve varijabilnosti) za dva skupa podataka.",
		arguments: [
			{
				name: "x",
				description: "je vrijednost na kojoj se vrednuje funkcija, ne može biti negativan broj"
			},
			{
				name: "stupanj_slobode1",
				description: "je stupanj slobode brojnika, broj izmeu 1 i 10^10, ne ukljuujui 10^10"
			},
			{
				name: "stupanj_slobode2",
				description: "je stupanj slobode nazivnika, broj izmeu 1 i 10^10, ne ukljuujui 10^10"
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
		description: "Daje poetni položaj jednog tekstnog niza u drugom tekstnom nizu. FIND razlikuje velika i mala slova.",
		arguments: [
			{
				name: "pronai_tekst",
				description: "je tekst koji želite pronai. Dvostruke navodnike (prazni tekst) možete koristiti za pronalaženje prvog znaka u nizu u kojem tražimo; zamjenski znakovi nisu dopuáteni"
			},
			{
				name: "u_tekstu",
				description: "je tekst koji sadrži tekst koji želite pronai"
			},
			{
				name: "poetni_broj",
				description: "navodi znak od kojeg poinje traženje. Prvi znak u parametru u_tekstu je znak broj 1. Ako ispustite poetni_broj, pretpostavlja se da je 1"
			}
		]
	},
	{
		name: "FINV",
		description: "Rauna inverziju (desnokraku) distribuciju F vjerojatnosti: ako je p = FDIST(x,...), tada je FINV(p,...) = x.",
		arguments: [
			{
				name: "vjerojatnost",
				description: "je vjerojatnost povezana s kumulativnom F distribucijom, broj izmeu 0 i 1, ukljuujui oba ta broja"
			},
			{
				name: "stupanj_slobode1",
				description: "je stupanj slobode brojnika, broj izmeu 1 i 10^10, ne ukljuujui 10^10"
			},
			{
				name: "stupanj_slobode2",
				description: "je stupanj slobode nazivnika, broj izmeu 1 i 10^10, ne ukljuujui 10^10"
			}
		]
	},
	{
		name: "FISHER",
		description: "Rauna Fisherovu transformaciju.",
		arguments: [
			{
				name: "x",
				description: "je brojana vrijednost za koju želite transformaciju, broj izmeu -1 i 1, iskljuujui -1 i 1"
			}
		]
	},
	{
		name: "FISHERINV",
		description: "Rauna inverznu vrijednost Fisherove transformacije: ako y = FISHER(x), tada je FISHERINV(y) = x.",
		arguments: [
			{
				name: "y",
				description: "je vrijednost za koju želite dobiti inverznu transformaciju"
			}
		]
	},
	{
		name: "FIXED",
		description: "Zaokružuje broj na navedeni broj decimalnih mjesta i vraa rezultat kao tekst sa zarezima ili bez njih.",
		arguments: [
			{
				name: "broj",
				description: "je broj koji želite zaokružiti i pretvoriti u tekst"
			},
			{
				name: "decimale",
				description: "je broj znamenaka s desne strane decimalne toke. Ako je ispuáten, broj decimala =2"
			},
			{
				name: "bez_zareza",
				description: "je logika vrijednost: ne prikazuj zareze u vraenom tekstu = TRUE; prikazuj zareze u vraenom tekstu = FALSE ili ispuáteno"
			}
		]
	},
	{
		name: "FLOOR",
		description: "Zaokružuje broj na vrijednost bližu nuli, na najbliži viáekratnik argumenta Znaajnost.",
		arguments: [
			{
				name: "broj",
				description: "je brojana vrijednost koju želite zaokružiti"
			},
			{
				name: "znaajnost",
				description: "je viáekratnik na koji želite zaokružiti. Argumenti Broj i znaajnost moraju biti ili oba pozitivna ili oba negativna"
			}
		]
	},
	{
		name: "FLOOR.MATH",
		description: "Zaokružuje broj na najbliži cijeli broj ili na najbliži znaajni viáekratnik.",
		arguments: [
			{
				name: "number",
				description: "je vrijednost koju želite zaokružiti"
			},
			{
				name: "significance",
				description: "je viáekratnik na koji želite zaokružiti"
			},
			{
				name: "mode",
				description: "kada joj se pruži vrijednost koja nije nula, ova e funkcija pri zaokruživanju težiti nuli"
			}
		]
	},
	{
		name: "FLOOR.PRECISE",
		description: "Zaokružuje broj na najbliži cijeli broj ili na najbliži viáekratnik argumenta znaajnost.",
		arguments: [
			{
				name: "broj",
				description: "je brojana vrijednost koju želite zaokružiti"
			},
			{
				name: "znaajnost",
				description: "je viáekratnik na koji želite zaokružiti. "
			}
		]
	},
	{
		name: "FORECAST",
		description: "Izraunava ili predvia buduu vrijednost duž linearnog trenda pomou postojeih vrijednosti.",
		arguments: [
			{
				name: "x",
				description: "je toka podatka za koji želite predvidjeti vrijednost i mora biti brojana vrijednost"
			},
			{
				name: "poznati_y",
				description: "je zavisno polje ili raspon brojanog podataka"
			},
			{
				name: "poznati_x",
				description: "je nezavisno polje ili raspon brojanog podataka. Varijanca argumenta poznati_x ne smije biti nula"
			}
		]
	},
	{
		name: "FORMULATEXT",
		description: "Vraa formulu kao niz.",
		arguments: [
			{
				name: "reference",
				description: "je referenca na formulu"
			}
		]
	},
	{
		name: "FREQUENCY",
		description: "Rauna koliko se esto vrijednosti pojavljuju u rasponu vrijednosti te vraa okomito polje brojeva koje ima jedan element viáe od parametra polje_klasa.",
		arguments: [
			{
				name: "podatkovno_polje",
				description: "je polje ili referenca na skup vrijednosti za koje želite izraunati uestalost (praznine i tekst se zanemaruju)"
			},
			{
				name: "polje_klasa",
				description: "je polje intervala ili referenca na intervale u koje želite grupirati vrijednosti u parametru podatkovno_polje"
			}
		]
	},
	{
		name: "FTEST",
		description: "Vraa vrijednost F-testa, dvokraku vjerojatnost da varijance u argumentima Polje1 i Polje2 nisu znaajno razliite.",
		arguments: [
			{
				name: "polje1",
				description: "je prvo polje ili raspon podataka i može sadržavati brojeve ili nazive, polje ili reference koje sadrže brojeve (praznine se zanemaruju)"
			},
			{
				name: "polje2",
				description: "je drugo polje ili raspon podataka i može sadržavati brojeve ili nazive, polja ili reference koje sadrže brojeve (praznine se zanemaruju)"
			}
		]
	},
	{
		name: "FV",
		description: "Rauna buduu vrijednost ulaganja temeljenu na periodinoj, konstantnoj otplati i konstantnoj kamati.",
		arguments: [
			{
				name: "stopa",
				description: "je kamatna stopa za razdoblje. Npr. koristite 6%/4 za kvartalna plaanja uz 6% godiánje kamate"
			},
			{
				name: "brrazd",
				description: "je ukupni broj razdoblja plaanja u investiciji"
			},
			{
				name: "rata",
				description: "je vrijednost otplate rate u svakom razdoblju; ne može se mijenjati tijekom trajanja investicije"
			},
			{
				name: "sv",
				description: "je sadaánja vrijednost ili sadaánja ukupna vrijednost niza buduih novanih iznosa. Ako je sv ispuáten, pretpostavlja se da je 0"
			},
			{
				name: "vrsta",
				description: "je vrijednost koja predstavlja dospijee plaanja: uplata na poetku razdoblja = 1; uplata na kraju razdoblja = 0 ili je izostavljena"
			}
		]
	},
	{
		name: "FVSCHEDULE",
		description: "Vraa buduu vrijednost poetne glavnice nakon primijenjenog skupa složenih kamatnih stopa.",
		arguments: [
			{
				name: "glavnica",
				description: "je sadaánja vrijednost"
			},
			{
				name: "raspored",
				description: "je polje kamatnih stopa koje e se primijeniti"
			}
		]
	},
	{
		name: "GAMMA",
		description: "Vraa vrijednost gama funkcije.",
		arguments: [
			{
				name: "x",
				description: "je vrijednost za koju želite izraunati gamu"
			}
		]
	},
	{
		name: "GAMMA.DIST",
		description: "Vraa gama-distribuciju.",
		arguments: [
			{
				name: "x",
				description: "je vrijednost na kojoj se izraunava distribucija, broj koji nije negativan"
			},
			{
				name: "alfa",
				description: "je parametar distribucije, pozitivan broj"
			},
			{
				name: "beta",
				description: "je parametar distribucije, pozitivan broj. Ako je beta = 1, GAMA.DIST vraa standardnu gama-distribuciju"
			},
			{
				name: "kumulativna",
				description: "je logika vrijednost: za vraanje kumulativne funkcije distribucije = TRUE, za vraanje funkcije mase vjerojatnosti = FALSE ili se izostavlja"
			}
		]
	},
	{
		name: "GAMMA.INV",
		description: "Vraa inverziju kumulativne gama-distribucije. Ako je p = GAMA.DIST(x,...), onda GAMA.INV(p,...) = x.",
		arguments: [
			{
				name: "vjerojatnost",
				description: "je vjerojatnost pridružena gama-distribuciji, broj izmeu 0 i 1, ukljuujui ta dva broja"
			},
			{
				name: "alfa",
				description: "je parametar distribucije, pozitivan broj"
			},
			{
				name: "beta",
				description: "je parametar distribucije, pozitivan broj. Ako je beta = 1, GAMMA.INV vraa inverziju standardne gama-distribucije"
			}
		]
	},
	{
		name: "GAMMADIST",
		description: "Rauna gama-distribuciju.",
		arguments: [
			{
				name: "x",
				description: "je vrijednost na kojoj želite izraunati distribuciju, broj koji nije negativan"
			},
			{
				name: "alfa",
				description: "je parametar distribucije, pozitivan broj"
			},
			{
				name: "beta",
				description: "je parametar distribucije, pozitivan broj. Ako je beta = 1, GAMADIST vraa standardnu gama-distribuciju"
			},
			{
				name: "kumulativna",
				description: "je logika vrijednost: vraa kumulativnu funkciju distribucije = TRUE; vraa funkciju mase vjerojatnosti = FALSE ili se izostavlja"
			}
		]
	},
	{
		name: "GAMMAINV",
		description: "Vraa inverziju kumulativne gama-distribucije: ako je p = GAMADIST(x,...), tada je GAMAINV(p,...) = x.",
		arguments: [
			{
				name: "vjerojatnost",
				description: "je vjerojatnost povezana s gama-distribucijom, broj izmeu 0 i 1, ukljuujui oba ta broja"
			},
			{
				name: "alfa",
				description: "je parametar distribucije, pozitivan broj"
			},
			{
				name: "beta",
				description: "je parametar distribucije, pozitivan broj. Ako je beta = 1, GAMAINV vraa standardnu gama-distribuciju"
			}
		]
	},
	{
		name: "GAMMALN",
		description: "Rauna prirodni logaritam gama-funkcije.",
		arguments: [
			{
				name: "x",
				description: "je vrijednost za koju želite izraunati GAMMALN, pozitivni broj"
			}
		]
	},
	{
		name: "GAMMALN.PRECISE",
		description: "Vraa prirodni algoritam gama-funkcije.",
		arguments: [
			{
				name: "x",
				description: "je vrijednost za koju želite izraunati GAMMALN.PRECISE, pozitivan broj"
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
		description: "Vraa najvei zajedniki djelitelj.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su 1 do 255 vrijednosti"
			},
			{
				name: "broj2",
				description: "su 1 do 255 vrijednosti"
			}
		]
	},
	{
		name: "GEOMEAN",
		description: "Vraa geometrijsku sredinu polja ili raspona pozitivnih podataka.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su 1 do 255 brojeva ili naziva, polja ili referenci koje sadrže brojeve za koje želite izraunati srednju vrijednost"
			},
			{
				name: "broj2",
				description: "su 1 do 255 brojeva ili naziva, polja ili referenci koje sadrže brojeve za koje želite izraunati srednju vrijednost"
			}
		]
	},
	{
		name: "GESTEP",
		description: "Provjerava je li broj vei od granine vrijednosti.",
		arguments: [
			{
				name: "broj",
				description: "je vrijednost koja se provjerava"
			},
			{
				name: "korak",
				description: "je granina vrijednost"
			}
		]
	},
	{
		name: "GETPIVOTDATA",
		description: "Izdvaja podatke spremljene u zaokretnoj tablici.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "podatkovno_polje",
				description: "je naziv podatkovnog polja iz kojeg se podaci izdvajaju"
			},
			{
				name: "zaokretna_tablica",
				description: "je referenca elije ili raspona elija u zaokretnoj tablici u kojem se nalaze podaci koje želite izdvojiti"
			},
			{
				name: "polje",
				description: "polje za referencu"
			},
			{
				name: "stavka",
				description: "stavka polja za referencu"
			}
		]
	},
	{
		name: "GROWTH",
		description: "Rauna brojeve u trendu eksponencijalnog rasta pomou postojeih toaka podataka.",
		arguments: [
			{
				name: "poznati_y",
				description: "je skup vrijednosti y koje ve znate u odnosu y = b*m^x, polje ili raspon pozitivnih brojeva"
			},
			{
				name: "poznati_x",
				description: "je neobavezan skup vrijednosti x koje možete znati u odnosu y = b*m^x, polje ili raspon jednake veliine kao i poznati_y"
			},
			{
				name: "novi_x",
				description: "su nove vrijednosti x za koje želite da GROWTH vrati odgovarajue vrijednosti y"
			},
			{
				name: "konst",
				description: "je logika vrijednost: konstanta b rauna se normalno ako je konst = TRUE; b je jednak 1 ako je konst = FALSE ili se izostavi"
			}
		]
	},
	{
		name: "HARMEAN",
		description: "Vraa harmonijsku srednju vrijednost skupa pozitivnih brojeva: reciprona vrijednost aritmetike sredine recipronih vrijednosti.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su 1 do 255 brojeva ili naziva, polja ili referenci koje sadrže brojeve za koje želite izraunati harmonijsku srednju vrijednost"
			},
			{
				name: "broj2",
				description: "su 1 do 255 brojeva ili naziva, polja ili referenci koje sadrže brojeve za koje želite izraunati harmonijsku srednju vrijednost"
			}
		]
	},
	{
		name: "HEX2BIN",
		description: "Heksadecimalni broj pretvara u binarni.",
		arguments: [
			{
				name: "broj",
				description: "je heksadecimalni broj koji želite pretvoriti"
			},
			{
				name: "mjesta",
				description: "je broj znakova koji e se koristiti"
			}
		]
	},
	{
		name: "HEX2DEC",
		description: "Heksadecimalni broj pretvara u decimalni.",
		arguments: [
			{
				name: "broj",
				description: "je heksadecimalni broj koji želite pretvoriti"
			}
		]
	},
	{
		name: "HEX2OCT",
		description: "Heksadecimalni broj pretvara u oktalni.",
		arguments: [
			{
				name: "broj",
				description: "je heksadecimalni broj koji želite pretvoriti"
			},
			{
				name: "mjesta",
				description: "je broj znakova koji e se koristiti"
			}
		]
	},
	{
		name: "HLOOKUP",
		description: "Traži vrijednost u najviáem retku tablice ili polje vrijednosti i vraa vrijednost u istom stupcu iz retka koji navedete.",
		arguments: [
			{
				name: "vrijednost_pretraživanja",
				description: "je vrijednost koju treba  pronai u prvom retku tablice, a može biti vrijednost, referenca, ili tekstni niz"
			},
			{
				name: "polje_tablica",
				description: "je tablica teksta, brojeva ili logikih vrijednosti u kojoj se traže podaci. Polje_tablica može biti referenca na raspon ili naziv raspona"
			},
			{
				name: "indeks_retka",
				description: "je broj retka u parametru polje_tablica iz kojeg e se vraati uparene vrijednosti. Prvi redak vrijednosti u tablici je 1"
			},
			{
				name: "raspon_pretraživanja",
				description: "je logika vrijednost: za pronalaženje najbliže odgovarajue vrijednosti u gornjem retku (sortirano po uzlaznom redoslijedu) = TRUE ili izostavljeno; za traženje identine vrijednosti = FALSE"
			}
		]
	},
	{
		name: "HOUR",
		description: "Vraa sat kao broj od 0 (00.00) do 23 (23.00).",
		arguments: [
			{
				name: "redni_broj",
				description: "je broj u kodu datuma i vremena koji koristi Spreadsheet ili tekst u obliku zapisa vremena, primjerice 16.48:00 ili 4.48:00 PM"
			}
		]
	},
	{
		name: "HYPERLINK",
		description: "Stvara preac ili skok koji otvara dokument spremljen na disku, mrežnom poslužitelju ili na internetu.",
		arguments: [
			{
				name: "veza_mjesto",
				description: "je tekst koji daje put i naziv datoteke za dokument koji treba otvoriti, mjesto na disku, UNC adresu ili URL put"
			},
			{
				name: "neslužbeni_naziv",
				description: "je tekst ili broj koji je prikazan u eliji. Ako se izostavi, elija prikazuje tekst veza_mjesto"
			}
		]
	},
	{
		name: "HYPGEOM.DIST",
		description: "Vraa hipergeometrijsku distribuciju.",
		arguments: [
			{
				name: "uzorak_s",
				description: "je broj uspjeánih pokuáaja u uzorku"
			},
			{
				name: "broj_uzorak",
				description: "je veliina uzorka"
			},
			{
				name: "populacija_s",
				description: "je broj uspjeánih pokuáaja u populaciji"
			},
			{
				name: "broj_pop",
				description: "je veliina populacije"
			},
			{
				name: "kumulativna",
				description: "je logika vrijednost: za kumulativnu funkciju distribucije koristite TRUE, za funkciju gustoe vjerojatnosti koristite FALSE"
			}
		]
	},
	{
		name: "HYPGEOMDIST",
		description: "Vraa hipergeometrijsku distribuciju.",
		arguments: [
			{
				name: "uzorak_u",
				description: "je broj uspjeánih pokuáaja u uzorku"
			},
			{
				name: "broj_uzorak",
				description: "je veliina uzorka"
			},
			{
				name: "populacija_u",
				description: "je broj uspjeánih pokuáaja u populaciji"
			},
			{
				name: "broj_pop",
				description: "je veliina populacije"
			}
		]
	},
	{
		name: "IF",
		description: "Provjerava je li uvjet ispunjen i vraa jednu vrijednost ako je TRUE i drugu vrijednost ako je FALSE.",
		arguments: [
			{
				name: "logiki_test",
				description: "je bilo koja vrijednost ili izraz koji se može vrednovati kao TRUE ili FALSE"
			},
			{
				name: "vrijednost_ako_je_true",
				description: "je vrijednost koja se vraa ako je logiki_test TRUE. Ako je ispuátena, vraa se TRUE. Možete ugnijezditi do sedam IF funkcija"
			},
			{
				name: "vrijednost_ako_je_false",
				description: "je vrijednost koja se vraa ako je logiki_test FALSE. Ako je ispuátena, vraa se FALSE"
			}
		]
	},
	{
		name: "IFERROR",
		description: "Vraa vrijednost_u_sluaju_pogreáke ako je izraz pogreáka, a vrijednost samog izraza nije.",
		arguments: [
			{
				name: "vrijednost",
				description: "je bilo koja vrijednost, izraz ili referenca"
			},
			{
				name: "vrijednost_u_sluaju_pogreáke",
				description: "je bilo koja vrijednost, izraz ili referenca"
			}
		]
	},
	{
		name: "IFNA",
		description: "Vraa vrijednost koju navodite ako se izraz razrjeáava kao #N/A, a inae vraa rezultat izraza.",
		arguments: [
			{
				name: "value",
				description: "je bilo koja vrijednost ili izraz ili referenca"
			},
			{
				name: "value_if_na",
				description: "je bilo koja vrijednost ili izraz ili referenca"
			}
		]
	},
	{
		name: "IMABS",
		description: "Vraa apsolutnu vrijednost (modul) kompleksnog broja.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj za koji želite apsolutnu vrijednost"
			}
		]
	},
	{
		name: "IMAGINARY",
		description: "Vraa imaginarni koeficijent kompleksnog broja.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj ini imaginarni koeficijent želite izraunati"
			}
		]
	},
	{
		name: "IMARGUMENT",
		description: "Vraa argument q, kut izražen u radijanima.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj za koji želite izraunati argument"
			}
		]
	},
	{
		name: "IMCONJUGATE",
		description: "Vraa kompleksni konjugat kompleksnog broja.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj iji kompleksni konjugat želite izraunati"
			}
		]
	},
	{
		name: "IMCOS",
		description: "Vraa kosinus kompleksnog broja.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj za koji želite izraunati kosinus"
			}
		]
	},
	{
		name: "IMCOSH",
		description: "Vraa hiperboliki sinus kompleksnog broja.",
		arguments: [
			{
				name: "inumber",
				description: "je kompleksni broj za koji želite hiperboliki sinus"
			}
		]
	},
	{
		name: "IMCOT",
		description: "Vraa kotangens kompleksnog broja.",
		arguments: [
			{
				name: "inumber",
				description: "je kompleksni broj iji kotangens želite"
			}
		]
	},
	{
		name: "IMCSC",
		description: "Vraa kosekans kompleksnog broja.",
		arguments: [
			{
				name: "inumber",
				description: "je kompleksni broj iji kosekans želite"
			}
		]
	},
	{
		name: "IMCSCH",
		description: "Vraa hiperboliki kosekans kompleksnog broja.",
		arguments: [
			{
				name: "inumber",
				description: "je kompleksni broj iji hiperboliki kosekans želite"
			}
		]
	},
	{
		name: "IMDIV",
		description: "Vraa kvocijent dva kompleksna broja.",
		arguments: [
			{
				name: "ibroj1",
				description: "je kompleksni brojnik ili djelitelj"
			},
			{
				name: "ibroj2",
				description: "je kompleksni nazivnik ili djeljenik"
			}
		]
	},
	{
		name: "IMEXP",
		description: "Vraa eksponencijalnu vrijednost kompleksnog broja.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj iju eksponencijalnu vrijednost želite izraunati"
			}
		]
	},
	{
		name: "IMLN",
		description: "Vraa prirodni logaritam kompleksnog broja.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj za koji želite izraunati prirodni logaritam"
			}
		]
	},
	{
		name: "IMLOG10",
		description: "Vraa logaritam baze 10 za kompleksni broj.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj za koji želite izraunati obini logaritam"
			}
		]
	},
	{
		name: "IMLOG2",
		description: "Vraa logaritam baze 2 za kompleksni broj.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj za koji želite izraunati logaritam baze 2"
			}
		]
	},
	{
		name: "IMPOWER",
		description: "Vraa kompleksni broj na cjelobrojnu potenciju.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj koji želite podii na potenciju"
			},
			{
				name: "broj",
				description: "je potencija na koju želite podii kompleksni broj"
			}
		]
	},
	{
		name: "IMPRODUCT",
		description: "Vraa umnožak 1 do 255 kompleksnih brojeva.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "ibroj1",
				description: "Ibroj1, Ibroj2,... su 1 do 255 kompleksnih brojeva za množenje."
			},
			{
				name: "ibroj2",
				description: "Ibroj1, Ibroj2,... su 1 do 255 kompleksnih brojeva za množenje."
			}
		]
	},
	{
		name: "IMREAL",
		description: "Vraa stvarni koeficijent kompleksnog broja.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj iji stvarni koeficijent želite izraunati"
			}
		]
	},
	{
		name: "IMSEC",
		description: "Vraa seksans kompleksnog broja.",
		arguments: [
			{
				name: "inumber",
				description: "je kompleksni broj iji sekans želite"
			}
		]
	},
	{
		name: "IMSECH",
		description: "Vraa hiperboliki sekans kompleksnog broja.",
		arguments: [
			{
				name: "inumber",
				description: "je kompleksni broj iji hiperboliki sekans želite"
			}
		]
	},
	{
		name: "IMSIN",
		description: "Vraa sinus kompleksnog broja.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj za koji želite izraunati sinus"
			}
		]
	},
	{
		name: "IMSINH",
		description: "Vraa hiperboliki sinus kompleksnog broja.",
		arguments: [
			{
				name: "inumber",
				description: "je kompleksni broj za koji želite hiperboliki sinus"
			}
		]
	},
	{
		name: "IMSQRT",
		description: "Vraa drugi korijen kompleksnog broja.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj za koji želite izraunati drugi korijen"
			}
		]
	},
	{
		name: "IMSUB",
		description: "Vraa razliku izmeu dva kompleksna broja.",
		arguments: [
			{
				name: "ibroj1",
				description: "je kompleksni broj od kojeg želite oduzeti ibroj2"
			},
			{
				name: "ibroj2",
				description: "je kompleksni broj koji želite oduzeti od argumenta ibroj1"
			}
		]
	},
	{
		name: "IMSUM",
		description: "Vraa zbroj kompleksnih brojeva.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "ibroj1",
				description: "je 1 do 255 kompleksnih brojeva za zbrajanje"
			},
			{
				name: "ibroj2",
				description: "je 1 do 255 kompleksnih brojeva za zbrajanje"
			}
		]
	},
	{
		name: "IMTAN",
		description: "Vraa tangens komplesnoga broja.",
		arguments: [
			{
				name: "inumber",
				description: "je kompleksni broj iji tangens želite"
			}
		]
	},
	{
		name: "INDEX",
		description: "Vraa vrijednost ili referencu elije na presjeku odreenog retka i stupca u zadanom rasponu.",
		arguments: [
			{
				name: "polje",
				description: " je raspon elija ili polja konstanti."
			},
			{
				name: "broj_retka",
				description: "oznaava redak u polju ili referenci iz kojega e se vratiti vrijednost. Ako je broj_retka izostavljen, broj_stupca je obavezan"
			},
			{
				name: "broj_stupca",
				description: "oznaava stupac u polju ili referenci iz kojega e se vratiti vrijednost. Ako je broj_stupca izostavljen, broj_retka je obavezan"
			}
		]
	},
	{
		name: "INDIRECT",
		description: "Vraa referencu navedenu tekstnim nizom.",
		arguments: [
			{
				name: "ref_tekst",
				description: "je referenca na eliju koja sadrži referencu stila A1, referencu stila R1C1, naziv odreen kao referencu ili referencu na eliju kao tekstni niz"
			},
			{
				name: "a1",
				description: "je logika vrijednost koja odreuje koja vrsta reference je sadržana u parametru ref_tekst: stil R1C1 = FALSE; stil A1 = TRUE ili ispuáten"
			}
		]
	},
	{
		name: "INFO",
		description: "Daje podatke o trenutnom radnom okruženju.",
		arguments: [
			{
				name: "vrsta_tekst",
				description: "jest tekst koji navodi vrstu podataka koju želite dobiti."
			}
		]
	},
	{
		name: "INT",
		description: "Zaokružuje broj na najbliži manji cijeli broj.",
		arguments: [
			{
				name: "broj",
				description: "je realni broj koji želite zaokružiti na manji cijeli broj"
			}
		]
	},
	{
		name: "INTERCEPT",
		description: "Izraunava toku u kojoj e pravac presjei os y, pomou pravca regresije s najmanjim odstupanjem iscrtanim kroz poznate vrijednosti x i y.",
		arguments: [
			{
				name: "poznati_y",
				description: "je zavisni skup promatranja ili podataka, a mogu biti brojevi ili nazivi, polja ili reference koje sadrže brojeve"
			},
			{
				name: "poznati_x",
				description: "je nezavisni skup promatranja ili podataka i mogu biti brojevi ili nazivi, polja ili reference koje sadrže brojeve"
			}
		]
	},
	{
		name: "INTRATE",
		description: "Vraa kamatnu stopu potpuno investirane vrijednosnice.",
		arguments: [
			{
				name: "isplata",
				description: "je datum plaanja vrijednosnice, izražen kao serijski broj datuma"
			},
			{
				name: "dospijee",
				description: "je datum dospijea vrijednosnice, izražen kao serijski broj datuma"
			},
			{
				name: "ulaganje",
				description: "je iznos investiran u vrijednosnicu"
			},
			{
				name: "otkup",
				description: "je iznos koji e se isplatiti po dospijeu vrijednosnice"
			},
			{
				name: "temelj",
				description: "je vrsta brojanja dana koja se koristi"
			}
		]
	},
	{
		name: "IPMT",
		description: "Rauna iznos kamate za zadano razdoblje, za ulaganje temeljeno na periodinim, konstantnim uplatama i konstantnoj kamatnoj stopi.",
		arguments: [
			{
				name: "stopa",
				description: "je kamatna stopa za razdoblje. Npr., koristite 6%/4 za kvartalna plaanja uz 6% godiánje kamate"
			},
			{
				name: "razd",
				description: "je razdoblje za koje želite izraunati kamatu, a mora biti u rasponu od 1 do brrazd"
			},
			{
				name: "brrazd",
				description: "je ukupan broj razdoblja plaanja u jednoj investiciji"
			},
			{
				name: "sv",
				description: "je sadaánja vrijednost ukupnog iznosa buduih uplata"
			},
			{
				name: "bv",
				description: "je budua vrijednost ili saldo koji želite postii nakon zadnje uplate. Ako je bv izostavljen, pretpostavlja se da je 0"
			},
			{
				name: "vrsta",
				description: "je logika vrijednost koja predstavlja vrijeme plaanja: na kraju razdoblja = 0 ili izostavljena, na poetku razdoblja = 1"
			}
		]
	},
	{
		name: "IRR",
		description: "Vraa internu stopu profitabilnosti za nizove novanih tokova.",
		arguments: [
			{
				name: "vrijednosti",
				description: "je polje ili referenca na elije koje sadrže brojeve za koje želite izraunati internu stopu profitabilnosti"
			},
			{
				name: "procjena",
				description: "je broj za koji procjenjujete da je blizak rezultatu funkcije IRR; 0,1 (10 posto) ako je izostavljen"
			}
		]
	},
	{
		name: "ISBLANK",
		description: "Provjerava upuuje li adresa na praznu eliju i kao rezultat daje TRUE ili FALSE.",
		arguments: [
			{
				name: "vrijednost",
				description: "je elija ili naziv koji upuuje na eliju koju želite provjeriti"
			}
		]
	},
	{
		name: "ISERR",
		description: "Provjerava da li je vrijednost bilo koja vrijednost pogreáke (#VALUE!, #REF!, #DIV/0!, #NUM!, #NAME? ili #NULL!), iskljuujui #N/A, i daje TRUE ili FALSE.",
		arguments: [
			{
				name: "vrijednost",
				description: "je vrijednost koju želite provjeriti. Vrijednost se može odnositi na eliju, formulu ili na naziv koji se odnosi na eliju, formulu ili vrijednost"
			}
		]
	},
	{
		name: "ISERROR",
		description: "Provjerava da li je vrijednost jednaka bilo kojoj vrijednosti pogreáke (#N/A, #VALUE!, #REF!, #DIV/0!, #NUM!, #NAME? ili #NULL!) i daje TRUE ili FALSE.",
		arguments: [
			{
				name: "vrijednost",
				description: "je vrijednost koju želite provjeriti. Vrijednost se može odnositi na eliju, formulu ili vrijednost"
			}
		]
	},
	{
		name: "ISEVEN",
		description: "Vraa TRUE ako je broj paran.",
		arguments: [
			{
				name: "broj",
				description: "je vrijednost koja se provjerava"
			}
		]
	},
	{
		name: "ISFORMULA",
		description: "Provjerava odnosi li se referenca na eliju koja sadrži formulu i potom vraa vrijednost TRUE ili FALSE.",
		arguments: [
			{
				name: "reference",
				description: "je referenca na eliju koju želite provjeriti. Referenca može biti na eliju, formulu ili naziv koji se odnosi na eliju"
			}
		]
	},
	{
		name: "ISLOGICAL",
		description: "Provjerava je li vrijednost logika vrijednost (TRUE ili FALSE) i kao rezultat daje TRUE ili FALSE.",
		arguments: [
			{
				name: "vrijednost",
				description: "je vrijednost koju želite provjeriti. Vrijednost se može odnositi na eliju, formulu ili naziv koji upuuje na eliju, formulu ili vrijednost"
			}
		]
	},
	{
		name: "ISNA",
		description: "Provjerava je li vrijednost #N/D i kao rezultat daje TRUE ili FALSE.",
		arguments: [
			{
				name: "vrijednost",
				description: "je vrijednost koju želite provjeriti. Vrijednost se može odnositi na eliju, formulu ili na naziv koji upuuje na eliju, formulu ili vrijednost"
			}
		]
	},
	{
		name: "ISNONTEXT",
		description: "Provjerava je li vrijednost vrsta koja nije tekst (prazne elije nisu tekst) i kao rezultat daje TRUE ili FALSE.",
		arguments: [
			{
				name: "vrijednost",
				description: "je vrijednost koju želite provjeriti: elija, formula ili naziv koji upuuje na eliju, formulu ili vrijednost"
			}
		]
	},
	{
		name: "ISNUMBER",
		description: "Provjerava je li vrijednost broj i kao rezultat daje TRUE ili FALSE.",
		arguments: [
			{
				name: "vrijednost",
				description: "je vrijednost koju želite provjeriti. Vrijednost se može odnositi na eliju, formulu ili na naziv koji upuuje na eliju, formulu ili vrijednost"
			}
		]
	},
	{
		name: "ISO.CEILING",
		description: "Zaokružuje broj na najbliži cijeli broj ili najbliži viáekratnik znaajnosti.",
		arguments: [
			{
				name: "broj",
				description: "je vrijednost koju želite zaokružiti"
			},
			{
				name: "znaajnost",
				description: "je neobavezni viáekratnik na koji želite zaokružiti"
			}
		]
	},
	{
		name: "ISODD",
		description: "Vraa TRUE ako je broj neparan.",
		arguments: [
			{
				name: "broj",
				description: "je vrijednost koja se provjerava"
			}
		]
	},
	{
		name: "ISOWEEKNUM",
		description: "Vraa broj ISO tjedna u godini za odreeni datum.",
		arguments: [
			{
				name: "date",
				description: "je datumsko-vremenski kod koji Spreadsheet koristi za izraun datuma i vremena"
			}
		]
	},
	{
		name: "ISPMT",
		description: "Rauna iznos kamate u toku odreenog perioda investicije. Npr., koristite 6%/4 za kvartalna plaanja uz 6% godiánje kamate.",
		arguments: [
			{
				name: "stopa",
				description: "kamatna stopa za razdoblje"
			},
			{
				name: "razd",
				description: "razdoblje za koje želite izraunati kamatu"
			},
			{
				name: "brrazd",
				description: "je ukupan broj razdoblja plaanja investicije"
			},
			{
				name: "sv",
				description: "sadaánja ukupna vrijednost niza buduih novanih iznosa"
			}
		]
	},
	{
		name: "ISREF",
		description: "Provjerava je li vrijednost referenca i kao rezultat daje TRUE ili FALSE.",
		arguments: [
			{
				name: "vrijednost",
				description: "je vrijednost koju želite provjeriti. Vrijednost se može odnositi na eliju, formulu ili naziv koji upuuje na eliju, formulu ili vrijednost"
			}
		]
	},
	{
		name: "ISTEXT",
		description: "Provjerava je li vrijednost tekst i kao rezultat daje TRUE ili FALSE.",
		arguments: [
			{
				name: "vrijednost",
				description: "je vrijednost koju želite provjeriti. Vrijednost se može odnositi na eliju, formulu ili na naziv koji upuuje na eliju, formulu ili vrijednost"
			}
		]
	},
	{
		name: "KURT",
		description: "Rauna spljoátenost (kurtozis) skupa podataka.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su 1 do 255 brojeva ili naziva, polja ili referenci koje sadrže brojeve za koje želite izraunati spljoátenost (kurtozis)"
			},
			{
				name: "broj2",
				description: "su 1 do 255 brojeva ili naziva, polja ili referenci koje sadrže brojeve za koje želite izraunati spljoátenost (kurtozis)"
			}
		]
	},
	{
		name: "LARGE",
		description: "Vraa k-tu najveu vrijednost u skupu podataka. Primjerice, peti najvei broj.",
		arguments: [
			{
				name: "polje",
				description: "je polje ili raspon podataka za koji želite odrediti k-tu najveu vrijednost"
			},
			{
				name: "k",
				description: "je mjesto koje tražena vrijednost zauzima (raunajui od najvee) u polju ili rasponu elija"
			}
		]
	},
	{
		name: "LCM",
		description: "Vraa najmanji zajedniki nazivnik.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su 1 do 255 vrijednosti za koje želite izraunati najmanji zajedniki nazivnik"
			},
			{
				name: "broj2",
				description: "su 1 do 255 vrijednosti za koje želite izraunati najmanji zajedniki nazivnik"
			}
		]
	},
	{
		name: "LEFT",
		description: "Vraa zadani broj znakova od poetka tekstnog niza.",
		arguments: [
			{
				name: "tekst",
				description: "je tekstni niz sa znakovima koje želite izdvojiti"
			},
			{
				name: "broj_znakova",
				description: "odreuje broj znakova koji e biti izdvojeni funkcijom LEFT; 1 ako je ispuáten"
			}
		]
	},
	{
		name: "LEN",
		description: "Vraa broj znakova u tekstnom nizu.",
		arguments: [
			{
				name: "tekst",
				description: "je tekst iju duljinu tražite. Razmaci se broje kao znakovi"
			}
		]
	},
	{
		name: "LINEST",
		description: "Rauna statistiku koja opisuje linearni trend koji najbolje odgovara poznatim podacima, prilagoavajui ravnu liniju metodom najmanjih kvadrata.",
		arguments: [
			{
				name: "poznati_y",
				description: "je skup vrijednosti y koje ve poznajete u odnosu y = mx + b"
			},
			{
				name: "poznati_x",
				description: "je neobavezan skup vrijednosti x koje možda znati u odnosu y = mx + b"
			},
			{
				name: "konst",
				description: "je logika vrijednost: konstanta b rauna se normalno ako je konst = TRUE ili ispuátena; b=0 ako je konst = FALSE"
			},
			{
				name: "stat",
				description: "je logika vrijednost: ako je stat = TRUE, rauna dodatnu regresijsku statistiku;ako je stat =FALSE ili izostavljen, rauna samo koeficijent m i konstantu b"
			}
		]
	},
	{
		name: "LN",
		description: "Vraa prirodni logaritam zadanog broja.",
		arguments: [
			{
				name: "broj",
				description: "je pozitivan realan broj za koji tražite prirodni logaritam"
			}
		]
	},
	{
		name: "LOG",
		description: "Vraa logaritam broja za zadanu bazu.",
		arguments: [
			{
				name: "broj",
				description: "je pozitivan realan broj za koji želite logaritam"
			},
			{
				name: "baza",
				description: "je baza logaritma; 10 ako je baza izostavljena"
			}
		]
	},
	{
		name: "LOG10",
		description: "Vraa logaritam zadanog broja za bazu 10.",
		arguments: [
			{
				name: "broj",
				description: "je pozitivan realan broj za koji želite izraunati logaritam s bazom 10"
			}
		]
	},
	{
		name: "LOGEST",
		description: "Rauna statistiku koja opisuje eksponencijalnu krivulju koja najbolje odgovara poznatim tokama podataka.",
		arguments: [
			{
				name: "poznati_y",
				description: "je skup vrijednosti y koje su vam ve poznate u odnosu: y = b*m^x"
			},
			{
				name: "poznati_x",
				description: " je neobvezan skup vrijednosti x koje mogu biti ve poznate u odnosu: y = b*m^x"
			},
			{
				name: "konst",
				description: "je logika vrijednost: konstanta b rauna se normalno ako je konst = TRUE ili ispuáten; b je jednaka 1 ako je konst = FALSE"
			},
			{
				name: "stat",
				description: "je logika vrijednost: rauna dodatne statistike regresije ako je stat = TRUE; rauna samo vrijednosti koeficijenata m i konstante b ako je stat =FALSE ili ispuáten"
			}
		]
	},
	{
		name: "LOGINV",
		description: "Vraa inverziju logaritamske normalne kumulativne funkcije distribucije od x, gdje se ln(x) distribuira s parametrima Srednja i Standardna_dev.",
		arguments: [
			{
				name: "vjerojatnost",
				description: "je vjerojatnost pridružena logaritamskoj normalnoj distribuciji, broj izmeu 0 i 1, ukljuujui oba ta broja"
			},
			{
				name: "srednja",
				description: "je srednja vrijednost od ln(x)"
			},
			{
				name: "standardna_dev",
				description: "je standardna devijacija od ln(x), pozitivan broj"
			}
		]
	},
	{
		name: "LOGNORM.DIST",
		description: "Vraa logaritamsku normalnu distribuciju od x, gdje je ln(x) normalno distribuiran s parametrima Srednja i Standardna_dev.",
		arguments: [
			{
				name: "x",
				description: "je vrijednost na kojoj se izraunava funkcija, pozitivan broj"
			},
			{
				name: "srednja",
				description: "je srednja vrijednost od ln(x)"
			},
			{
				name: "standardna_dev",
				description: "je standardna devijacija od ln(x), pozitivan broj"
			},
			{
				name: "kumulativna",
				description: "je logika vrijednost: za kumulativnu funkciju distribucije koristite TRUE, za funkciju gustoe vjerojatnosti koristite FALSE"
			}
		]
	},
	{
		name: "LOGNORM.INV",
		description: "Vraa inverziju logaritamske normalne kumulativne distribucije od x, gdje je ln(x) distribuiran s parametrima Srednja i Standardna_dev.",
		arguments: [
			{
				name: "vjerojatnost",
				description: "je vjerojatnost pridružena logaritamskoj normalnoj distribuciji, broj izmeu 0 i 1, ukljuujui oba ta broja"
			},
			{
				name: "srednja",
				description: "je srednja vrijednost od ln(x)"
			},
			{
				name: "standardna_dev",
				description: "je standardna devijacija od ln(x), pozitivan broj"
			}
		]
	},
	{
		name: "LOGNORMDIST",
		description: "Vraa kumulativnu normalnu logaritamsku razdiobu od x, gdje je ln(x) normalno distribuiran s parametrima Srednja i Standardna_dev.",
		arguments: [
			{
				name: "x",
				description: "je vrijednost za koju vrednujete funkciju, pozitivan broj"
			},
			{
				name: "srednja",
				description: "je srednja vrijednost od ln(x)"
			},
			{
				name: "standardna_dev",
				description: "je standardna devijacija od ln(x), pozitivan broj"
			}
		]
	},
	{
		name: "LOOKUP",
		description: "Traži vrijednost bilo iz raspona od jednog retka ili jednog stupca ili iz polja. Podržana je kompatibilnost sa starijim verzijama.",
		arguments: [
			{
				name: "vrijednost_pretraživanja",
				description: "je vrijednost koju LOOKUP traži u parametru vektor_pretraživanja, a može biti broj, tekst, logika vrijednost ili naziv ili adresa na vrijednost"
			},
			{
				name: "vektor_pretraživanja",
				description: "je raspon koji sadrži samo jedan redak ili jedan stupac teksta, brojeva ili logikih vrijednosti poredanih uzlaznim redoslijedom"
			},
			{
				name: "rezultat_vektor",
				description: "je raspon koji sadrži samo jedan redak ili jedan stupac iste veliine kao i vektor_pretraživanja"
			}
		]
	},
	{
		name: "LOWER",
		description: "Sva slova tekstnog niza pretvara u mala.",
		arguments: [
			{
				name: "tekst",
				description: "je tekst u kojem sva slova pretvarate u mala slova. Znakovi koji nisu slova ostaju nepromijenjeni"
			}
		]
	},
	{
		name: "MATCH",
		description: "Daje relativni položaj stavke u polju koja odgovara navedenoj vrijednosti u navedenom poretku.",
		arguments: [
			{
				name: "vrijednost_pretraživanja",
				description: "je vrijednost koju koristite za pronalaženje vrijednosti koju želite u polju, broju, tekstu ili logikoj vrijednosti ili pak u referenci na jedno od toga"
			},
			{
				name: "polje_pretraživanja",
				description: "je susjedni raspon elija koje sadrže mogue vrijednosti pretraživanja, polje vrijednosti ili referenca na polje"
			},
			{
				name: "vrsta_podudaranja",
				description: "je broj 1, 0, ili -1 koji oznaava vrijednost koju treba vratiti."
			}
		]
	},
	{
		name: "MAX",
		description: "Vraa najvei broj iz skupa zadanih brojeva. Zanemaruje logike vrijednosti i tekst.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su 1 do 255 brojeva, praznih elija, logikih vrijednosti ili brojeva u tekstnom obliku za koje želite maksimum"
			},
			{
				name: "broj2",
				description: "su 1 do 255 brojeva, praznih elija, logikih vrijednosti ili brojeva u tekstnom obliku za koje želite maksimum"
			}
		]
	},
	{
		name: "MAXA",
		description: "Vraa najveu vrijednost u skupu vrijednosti. Ne zanemaruje logike vrijednosti i tekst.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrijednost1",
				description: "su 1 do 255 brojeva, praznih elija, logikih vrijednosti ili tekstnih prikaza brojeva za koje želite pronai najveu vrijednost"
			},
			{
				name: "vrijednost2",
				description: "su 1 do 255 brojeva, praznih elija, logikih vrijednosti ili tekstnih prikaza brojeva za koje želite pronai najveu vrijednost"
			}
		]
	},
	{
		name: "MDETERM",
		description: "Izraunava determinantu matrice zadane poljem.",
		arguments: [
			{
				name: "polje",
				description: "je brojano polje s jednakim brojem redaka i stupaca, a može biti raspon elija ili polje konstanti"
			}
		]
	},
	{
		name: "MEDIAN",
		description: "Vraa medijan ili broj u sredini skupa zadanih brojeva.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su 1 do 255 brojeva, naziva, polja ili referenci koje sadrže brojeve za koje želite medijan"
			},
			{
				name: "broj2",
				description: "su 1 do 255 brojeva, naziva, polja ili referenci koje sadrže brojeve za koje želite medijan"
			}
		]
	},
	{
		name: "MID",
		description: "Vraa znakove iz sredine tekstnog niza, uz zadano poetno mjesto i duljinu.",
		arguments: [
			{
				name: "tekst",
				description: "je tekstni niz sa znakovima koje želite izdvojiti"
			},
			{
				name: "poetni_broj",
				description: "je mjesto prvog znaka koji izdvajate iz teksta. Prvi znak u tekstu ima poetni_broj jednak 1"
			},
			{
				name: "broj_znakova",
				description: "navodi broj znakova koje treba izdvojiti iz zadanog teksta"
			}
		]
	},
	{
		name: "MIN",
		description: "Vraa najmanji broj iz skupa zadanih brojeva. Zanemaruje logike vrijednosti i tekst.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su 1 do 255 brojeva, praznih elija, logikih vrijednosti ili brojeva u tekstnom obliku za koje želite minimum"
			},
			{
				name: "broj2",
				description: "su 1 do 255 brojeva, praznih elija, logikih vrijednosti ili brojeva u tekstnom obliku za koje želite minimum"
			}
		]
	},
	{
		name: "MINA",
		description: "Vraa najmanju vrijednost u skupu vrijednosti. Ne zanemaruje logike vrijednosti i tekst.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrijednost1",
				description: "su 1 do 255 brojeva, praznih elija, logikih vrijednosti ili tekstnih prikaza brojeva za koje želite pronai najmanju vrijednost"
			},
			{
				name: "vrijednost2",
				description: "su 1 do 255 brojeva, praznih elija, logikih vrijednosti ili tekstnih prikaza brojeva za koje želite pronai najmanju vrijednost"
			}
		]
	},
	{
		name: "MINUTE",
		description: "Vraa minute kao broj od 0 do 59.",
		arguments: [
			{
				name: "redni_broj",
				description: "je broj u kodu datuma i vremena koji koristi Spreadsheet ili tekst u obliku zapisa vremena, primjerice 16.48:00 ili 4.48:00 PM"
			}
		]
	},
	{
		name: "MINVERSE",
		description: "Vraa inverznu matricu matrice zadane poljem.",
		arguments: [
			{
				name: "polje",
				description: "je brojano polje s jednakim brojem redaka i stupaca, a može biti raspon elija ili polje konstanti"
			}
		]
	},
	{
		name: "MIRR",
		description: "Vraa modificiranu internu stopu dobiti za slijed periodinih novanih tokova, uzimajui u obzir i iznos uloga i kamate dobivene za ponovno ulaganje.",
		arguments: [
			{
				name: "vrijednosti",
				description: "je polje ili referenca na elije koje sadrže brojeve koji predstavljaju niz periodinih izdataka (negativne vrijednosti) i primitaka (pozitivne vrijednosti) u odreenom razdoblju"
			},
			{
				name: "stopa_financiranja",
				description: "je kamatna stopa koju plaate na vrijednost novanog iznosa koriátenog u novanim tokovima"
			},
			{
				name: "stopa_reinvestiranja",
				description: "je kamatna stopa koju dobivate na temelju ponovnog novanog ulaganja"
			}
		]
	},
	{
		name: "MMULT",
		description: "Vraa matrini umnožak dvaju polja, polje s jednakim brojem redaka kao polje1 i jednakim brojem stupaca kao polje2.",
		arguments: [
			{
				name: "polje1",
				description: "je prvo polje brojeva za množenje koje mora imati onoliki broj stupaca koliko polje2 ima redaka"
			},
			{
				name: "polje2",
				description: "je prvo polje brojeva za množenje koje mora imati onoliki broj stupaca koliko polje2 ima redaka"
			}
		]
	},
	{
		name: "MOD",
		description: "Vraa ostatak nakon dijeljenja broja djeliteljem. Rezultat ima isti predznak kao djelitelj.",
		arguments: [
			{
				name: "broj",
				description: "je broj za koji tražite ostatak nakon dijeljenja"
			},
			{
				name: "djelitelj",
				description: "je broj kojim dijelite zadani broj"
			}
		]
	},
	{
		name: "MODE",
		description: "Vraa vrijednost koja se najeáe pojavljuje, odnosno ponavlja u zadanom polju ili rasponu podataka.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su od 1 do 255 brojeva, naziva, polja ili referenci koje sadrže brojeve za koje želite modalitet"
			},
			{
				name: "broj2",
				description: "su od 1 do 255 brojeva, naziva, polja ili referenci koje sadrže brojeve za koje želite modalitet"
			}
		]
	},
	{
		name: "MODE.MULT",
		description: "Vraa vertikalno polje najeáih ili najeáe ponavljanih vrijednosti u polju ili rasponu podataka. Za horizontalno polje koristite =TRANSPOZICIJA(MOD.VIàE(broj1,broj2,...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su od 1 do 255 brojeva ili naziva, polja ili referenci koje sadrže brojeve za koje želite modalitet"
			},
			{
				name: "broj2",
				description: "su od 1 do 255 brojeva ili naziva, polja ili referenci koje sadrže brojeve za koje želite modalitet"
			}
		]
	},
	{
		name: "MODE.SNGL",
		description: "Vraa najeáu ili najeáe ponavljanu vrijednost u zadanom polju ili rasponu podataka.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su od 1 do 255 brojeva, naziva, polja ili referenci koje sadrže brojeve za koje želite modalitet"
			},
			{
				name: "broj2",
				description: "su od 1 do 255 brojeva, naziva, polja ili referenci koje sadrže brojeve za koje želite modalitet"
			}
		]
	},
	{
		name: "MONTH",
		description: "Vraa mjesec kao cijeli broj od 1 (sijeanj) do 12 (prosinac).",
		arguments: [
			{
				name: "redni_broj",
				description: "je broj u kodu datuma i vremena koji koristi Spreadsheet"
			}
		]
	},
	{
		name: "MROUND",
		description: "Vraa broj zaokružen na željeni viáekratnik.",
		arguments: [
			{
				name: "broj",
				description: "je vrijednost koja se zaokružuje"
			},
			{
				name: "viáekratnik",
				description: "je viáekratnik na koji želite zaokružiti broj"
			}
		]
	},
	{
		name: "MULTINOMIAL",
		description: "Vraa multinomni koeficijent skupa brojeva.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su 1 do 255 vrijednosti za koje želite izraunati multinomni koeficijent"
			},
			{
				name: "broj2",
				description: "su 1 do 255 vrijednosti za koje želite izraunati multinomni koeficijent"
			}
		]
	},
	{
		name: "MUNIT",
		description: "Vraa matrinu jedinicu za navedenu dimenziju.",
		arguments: [
			{
				name: "dimension",
				description: "je cijeli broj koji  navodi dimenziju jedinine matrice koju želite vratiti"
			}
		]
	},
	{
		name: "N",
		description: "Nebrojane vrijednosti pretvara u brojeve, datume u serijske brojeve, TRUE u 1, a sve ostalo u 0 (nula).",
		arguments: [
			{
				name: "vrijednost",
				description: "je vrijednost koju želite pretvoriti"
			}
		]
	},
	{
		name: "NA",
		description: "Vraa vrijednost pogreáke #N/D (vrijednost nije dostupna).",
		arguments: [
		]
	},
	{
		name: "NEGBINOM.DIST",
		description: "Vraa negativnu binomnu distribuciju, vjerojatnost da e prije broj_u-tog uspjeánog pokuáaja, biti broj_n neuspjeánih pokuáaja uz vjerojatnost uspjeha vjerojatnost_u.",
		arguments: [
			{
				name: "broj_n",
				description: "je broj neuspjeánih pokuáaja"
			},
			{
				name: "broj_u",
				description: "je broj praga uspjeha"
			},
			{
				name: "vjerojatnost_u",
				description: "je vjerojatnost uspjeha, broj izmeu 0 i 1"
			},
			{
				name: "kumulativna",
				description: "je logika vrijednost: za kumulativnu funkciju distribucije koristite TRUE, za funkciju mase vjerojatnosti koristite FALSE"
			}
		]
	},
	{
		name: "NEGBINOMDIST",
		description: "Vraa negativnu binomnu distribuciju, vjerojatnost da e se dogoditi broj_n neuspjeánih pokuáaja prije broj_u-tog uspjeánog pokuáaja, s vjerojatnoáu uspjeha vjerojatnost_u.",
		arguments: [
			{
				name: "broj_n",
				description: "je broj neuspjeánih pokuáaja"
			},
			{
				name: "broj_u",
				description: "je broj praga uspjeánih pokuáaja"
			},
			{
				name: "vjerojatnost_u",
				description: "je vjerojatnost uspjeha, broj izmeu 0 i 1"
			}
		]
	},
	{
		name: "NETWORKDAYS",
		description: "Vraa broj cijelih radnih dana izmeu dva datuma.",
		arguments: [
			{
				name: "poetni_datum",
				description: "je serijski broj datuma koji predstavlja poetni datum"
			},
			{
				name: "zavráni_datum",
				description: "je serijskih broj dana koji predstavlja zavráni datum"
			},
			{
				name: "praznici",
				description: "je neobavezan skup od jednog ili viáe serijskih brojeva datuma koji e biti iskljueni iz radnog kalendara, poput državnih i kliznih praznika"
			}
		]
	},
	{
		name: "NETWORKDAYS.INTL",
		description: "Vraa broj cijelih radnih dana izmeu dvaju datuma s prilagoenim parametrima vikenda.",
		arguments: [
			{
				name: "poetni_datum",
				description: "je serijski broj datuma koji oznaava poetni datum"
			},
			{
				name: "zavráni_datum",
				description: "je serijski broj datuma koji oznaava zavráni datum"
			},
			{
				name: "vikend",
				description: "je broj ili niz koji odreuje kada dolaze vikendi"
			},
			{
				name: "praznici",
				description: "je neobavezan skup jednog ili viáe serijskih brojeva datuma koje valja izuzeti iz kalendara radnih dana, npr. državne i pomine praznike"
			}
		]
	},
	{
		name: "NOMINAL",
		description: "Vraa nominalnu godiánju kamatnu stopu.",
		arguments: [
			{
				name: "efektivna_stopa",
				description: "je efektivna kamatna stopa"
			},
			{
				name: "brrazdgod",
				description: "je broj razdoblja u godini"
			}
		]
	},
	{
		name: "NORM.DIST",
		description: "Vraa normalnu distribuciju za navedenu srednju vrijednost i standardnu devijaciju.",
		arguments: [
			{
				name: "x",
				description: "je vrijednost za koju želite distribuciju"
			},
			{
				name: "srednja",
				description: "je aritmetika sredina distribucije"
			},
			{
				name: "standardna_dev",
				description: "je standardna devijacija distribucije, pozitivan broj"
			},
			{
				name: "kumulativna",
				description: "je logika vrijednost: za kumulativnu funkciju distribucije koristite TRUE, za funkciju gustoe vjerojatnosti koristite FALSE"
			}
		]
	},
	{
		name: "NORM.INV",
		description: "Vraa inverziju kumulativne normalne distribucije za navedenu srednju vrijednost i standardnu devijaciju.",
		arguments: [
			{
				name: "vjerojatnost",
				description: "je vjerojatnost u odnosu na normalnu distribuciju, broj izmeu 0 i 1"
			},
			{
				name: "srednja",
				description: "je aritmetika sredina distribucije"
			},
			{
				name: "standardna_dev",
				description: "je standardna devijacija distribucije, pozitivan broj"
			}
		]
	},
	{
		name: "NORM.S.DIST",
		description: "Vraa standardnu normalnu distribuciju (ima srednju vrijednost nula i standardnu devijaciju jedan).",
		arguments: [
			{
				name: "z",
				description: "je vrijednost za koju želite distribuciju"
			},
			{
				name: "kumulativna",
				description: "je logika vrijednost koju vraa funkcija: kumulativna funkcija distribucije = TRUE, funkcija gustoe vjerojatnosti = FALSE"
			}
		]
	},
	{
		name: "NORM.S.INV",
		description: "Vraa inverziju standardne normalne kumulativne distribucije (ima srednju vrijednost 0 i standardnu devijaciju 1).",
		arguments: [
			{
				name: "vjerojatnost",
				description: "je vjerojatnost koja odgovara normalnoj distribuciji, broj izmeu 0 i 1, ukljuujui oba ta broja"
			}
		]
	},
	{
		name: "NORMDIST",
		description: "Vraa kumulativnu normalnu distribuciju za navedenu srednju vrijednost i standardnu devijaciju.",
		arguments: [
			{
				name: "x",
				description: "je vrijednost za koju želite distribuciju"
			},
			{
				name: "srednja",
				description: "je aritmetika sredina distribucije"
			},
			{
				name: "standardna_dev",
				description: "je standardna devijacija distribucije, pozitivan broj"
			},
			{
				name: "kumulativna",
				description: "je logika vrijednost: za kumulativnu funkciju distribucije koristite TRUE, za funkciju gustoe vjerojatnosti koristite FALSE"
			}
		]
	},
	{
		name: "NORMINV",
		description: "Vraa inverziju kumulativne normalne distribucije za navedenu srednju vrijednost i standardnu devijaciju.",
		arguments: [
			{
				name: "vjerojatnost",
				description: "je vjerojatnost u odnosu na normalnu distribuciju, broj izmeu 0 i 1"
			},
			{
				name: "srednja",
				description: "je aritmetika sredina distribucije"
			},
			{
				name: "standardna_dev",
				description: "je standardna devijacija distribucije, pozitivan broj"
			}
		]
	},
	{
		name: "NORMSDIST",
		description: "Vraa standardnu normalnu kumulativnu distribuciju (ima srednju vrijednost 0 i standardnu devijaciju 1).",
		arguments: [
			{
				name: "z",
				description: "je vrijednost za koju želite distribuciju"
			}
		]
	},
	{
		name: "NORMSINV",
		description: "Vraa inverziju standardne normalne kumulativne distribucije (ima srednju vrijednost 0 i standardnu devijaciju 1).",
		arguments: [
			{
				name: "vjerojatnost",
				description: "je vjerojatnost vezana uz normalnu distribuciju, broj izmeu 0 i 1, ukljuujui oba ta broja"
			}
		]
	},
	{
		name: "NOT",
		description: "Mijenja FALSE u TRUE, ili TRUE u FALSE.",
		arguments: [
			{
				name: "logika",
				description: "je vrijednost ili izraz koji se može ocijeniti kao TRUE ili FALSE"
			}
		]
	},
	{
		name: "NOW",
		description: "Daje toan datum i vrijeme oblikovano kao datum i vrijeme.",
		arguments: [
		]
	},
	{
		name: "NPER",
		description: "Rauna broj razdoblja za ulaganje koje se temelji na periodinim i konstantnim uplatama i konstantnoj kamatnoj stopi.",
		arguments: [
			{
				name: "stopa",
				description: " je kamatna stopa po razdoblju. Npr. koristite 6%/4 za kvartalna plaanja uz 6% godiánje kamate"
			},
			{
				name: "rata",
				description: "je vrijednost uplate koja se obavlja u svakom razdoblju; ne može se mijenjati tijekom trajanja investicije"
			},
			{
				name: "sv",
				description: "je sadaánja vrijednost ili sadaánja ukupna vrijednost niza buduih uplata"
			},
			{
				name: "bv",
				description: "je budua vrijednost ili saldo koji želite postii nakon zadnje uplate. Ako je bv izostavljen, pretpostavlja se da je 0"
			},
			{
				name: "vrsta",
				description: "je logika vrijednost: uplata na poetku razdoblja = 1; uplata na kraju razdoblja = 0 ili je izostavljena"
			}
		]
	},
	{
		name: "NPV",
		description: "Vraa istu sadaánju vrijednost ulaganja, prema diskontnoj stopi i skupu buduih izdataka (negativne vrijednosti) i primitaka (pozitivne vrijednosti).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "stopa",
				description: " diskontna je stopa tijekom jednog razdoblja"
			},
			{
				name: "vrijednost1",
				description: "jesu 1 do 254 plaanja koja oznaavaju novane izdatke i primitke, a koji se javljaju u jednakim vremenskim razmacima i na kraju svakog razdoblja"
			},
			{
				name: "vrijednost2",
				description: "jesu 1 do 254 plaanja koja oznaavaju novane izdatke i primitke, a koji se javljaju u jednakim vremenskim razmacima i na kraju svakog razdoblja"
			}
		]
	},
	{
		name: "NUMBERVALUE",
		description: "Pretvara tekst u broj na nain neovisan o regionalnim postavkama.",
		arguments: [
			{
				name: "text",
				description: "je niz koji prikazuje broj koji želite pretvoriti"
			},
			{
				name: "decimal_separator",
				description: "je znak koji se koristi kao decimalni razdjelnik u nizu"
			},
			{
				name: "group_separator",
				description: "je znak koji se koristi kao grupni razdjelnik u nizu"
			}
		]
	},
	{
		name: "OCT2BIN",
		description: "Oktalni broj pretvara u binarni.",
		arguments: [
			{
				name: "broj",
				description: "je oktalni broj koji želite pretvoriti"
			},
			{
				name: "mjesta",
				description: "je broj znakova koji e se koristiti"
			}
		]
	},
	{
		name: "OCT2DEC",
		description: "Oktalni broj pretvara u decimalni.",
		arguments: [
			{
				name: "broj",
				description: "je oktalni broj koji želite pretvoriti"
			}
		]
	},
	{
		name: "OCT2HEX",
		description: "Oktalni broj pretvara u heksadecimalni.",
		arguments: [
			{
				name: "broj",
				description: "je oktalni broj koji želite pretvoriti"
			},
			{
				name: "mjesta",
				description: "je broj znakova koji e se koristiti"
			}
		]
	},
	{
		name: "ODD",
		description: "Zaokružuje pozitivni broj na najbliži vei i negativni na najbliži manji neparni cijeli broj.",
		arguments: [
			{
				name: "broj",
				description: "je vrijednost koju zaokružujete"
			}
		]
	},
	{
		name: "OFFSET",
		description: "Vraa referencu na raspon koji je udaljen zadani broj redaka i stupaca od navedene reference.",
		arguments: [
			{
				name: "referenca",
				description: "je referenca na kojoj želite temeljiti pomak, referenca na eliju ili raspon susjednih elija"
			},
			{
				name: "reci",
				description: "je broj redaka, nagore ili nadolje, na koje želite da se odnosi gornja lijeva elija rezultata"
			},
			{
				name: "stupci",
				description: "je broj stupaca, nalijevo ili nadesno, na koje želite da se odnosi gornja lijeva elija rezultata"
			},
			{
				name: "visina",
				description: "je visina vraene reference izražena u broju redaka, ista kao referenca ako se izostavi"
			},
			{
				name: "áirina",
				description: "je željena áirina vraene reference izražena u broju stupaca, ista kao referenca ako se izostavi"
			}
		]
	},
	{
		name: "OR",
		description: "Provjerava je li neki od argumenta TRUE i kao rezultat daje TRUE ili FALSE. Rezultat je FALSE samo ako su svi argumenti FALSE.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logika1",
				description: "su 1 do 255 uvjeta koje želite provjeriti, a mogu biti TRUE ili FALSE"
			},
			{
				name: "logika2",
				description: "su 1 do 255 uvjeta koje želite provjeriti, a mogu biti TRUE ili FALSE"
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
		description: "Vraa broj razdbolja koja su potrebna da bi ulaganje dosegnulo odreenu vrijednost.",
		arguments: [
			{
				name: "rate",
				description: "je kamata po razdoblju."
			},
			{
				name: "pv",
				description: "je trenutana vrijednost ulaganja"
			},
			{
				name: "fv",
				description: "je željena budua vrijednost ulaganja"
			}
		]
	},
	{
		name: "PEARSON",
		description: "Rauna koeficijent korelacije Pearsonova produkta momenta, r.",
		arguments: [
			{
				name: "polje1",
				description: "je skup nezavisnih vrijednosti"
			},
			{
				name: "polje2",
				description: "je skup zavisnih vrijednosti"
			}
		]
	},
	{
		name: "PERCENTILE",
		description: "Vraa k-ti percentil vrijednosti u rasponu.",
		arguments: [
			{
				name: "polje",
				description: "je polje ili raspon podataka koji definiraju relativni odnos"
			},
			{
				name: "k",
				description: "je percentilna vrijednost u rasponu 0 do 1, ukljuujui oba ta broja"
			}
		]
	},
	{
		name: "PERCENTILE.EXC",
		description: "Vraa k-ti percentil vrijednosti u rasponu, pri emu je k u rasponu 0..1, ne ukljuujui ta dva broja.",
		arguments: [
			{
				name: "polje",
				description: "je polje ili raspon podataka koje definira relativni položaj"
			},
			{
				name: "k",
				description: "je vrijednost percentila izmeu 0 i 1, ukljuujui ta dva broja"
			}
		]
	},
	{
		name: "PERCENTILE.INC",
		description: "Vraa k-ti percentil vrijednosti u rasponu, pri emu je k u rasponu 0..1, ukljuujui ta dva broja.",
		arguments: [
			{
				name: "polje",
				description: "je polje ili raspon podataka koji definira relativni položaj"
			},
			{
				name: "k",
				description: "je vrijednost percentila u rasponu od 0 do 1, ukljuujui ta dva broja"
			}
		]
	},
	{
		name: "PERCENTRANK",
		description: "Vraa položaj vrijednosti u skupu podataka kao postotak skupa podataka.",
		arguments: [
			{
				name: "polje",
				description: "je polje ili raspon podataka s brojanom vrijednoáu koja definira relativni položaj"
			},
			{
				name: "x",
				description: "je vrijednost za koju želite saznati položaj"
			},
			{
				name: "znaajnost",
				description: "je neobvezna vrijednost koja odreuje broj znaajnih brojki za vraenu vrijednost postotka, ako se izostavi, koriste se tri brojke (0,xxx%)"
			}
		]
	},
	{
		name: "PERCENTRANK.EXC",
		description: "Vraa rang vrijednosti u skupu podataka kao postotak skupa podataka (0...1, ne ukljuujui ta dva broja).",
		arguments: [
			{
				name: "polje",
				description: "je polje ili raspon podataka s numerikim vrijednostima koji definira relativni položaj"
			},
			{
				name: "x",
				description: "je vrijednost za koju želite saznati rang"
			},
			{
				name: "znaajnost",
				description: "je neobavezna vrijednost koja odreuje broj znaajnih brojki za vraeni postotak, tri brojke ako se izostavi (0,xxx%)"
			}
		]
	},
	{
		name: "PERCENTRANK.INC",
		description: "Vraa rang vrijednosti skupu podataka kao postotak skupa podataka (0..1, ukljuujui ta dva broja).",
		arguments: [
			{
				name: "polje",
				description: "je polje ili raspon podataka s numerikim vrijednostima koji definira relativni položaj"
			},
			{
				name: "x",
				description: "je vrijednost za koju želite saznati rang"
			},
			{
				name: "znaajnost",
				description: "je neobavezna vrijednost koja odreuje broj znaajnih brojki za vraeni postotak, tri brojke ako se izostavi (0,xxx%)"
			}
		]
	},
	{
		name: "PERMUT",
		description: "Vraa broj permutacija za zadani broj objekata koji mogu biti odabrani meu zadanim ukupnim brojem objekata.",
		arguments: [
			{
				name: "broj",
				description: "je ukupni broj objekata"
			},
			{
				name: "odabrani_broj",
				description: "je broj objekata u svakoj permutaciji"
			}
		]
	},
	{
		name: "PERMUTATIONA",
		description: "Vraa broj permutacija za odreeni broj objekata (s ponavljanjem) koji može biti odabran meu svim objektima.",
		arguments: [
			{
				name: "number",
				description: "je ukupan broj objekata"
			},
			{
				name: "number_chosen",
				description: "je broj objekata u svakoj permutaciji"
			}
		]
	},
	{
		name: "PHI",
		description: "Vraa vrijednost gustoe funkcije za standardnu normalnu distribuciju.",
		arguments: [
			{
				name: "x",
				description: "je broj za koji želite gustou standardne normalne distribucije"
			}
		]
	},
	{
		name: "PI",
		description: "Vraa broj 3,14159265358979, vrijednost matematike konstante pi, s tonoáu od 15 znamenaka.",
		arguments: [
		]
	},
	{
		name: "PMT",
		description: "Rauna vrijednost uplata za zajam, koji se temelji na konstantnim uplatama i konstantnoj kamatnoj stopi.",
		arguments: [
			{
				name: "stopa",
				description: "je kamatna stopa po razdoblju za zajam. Npr. koristite 6%/4 za kvartalna plaanja uz 6% godiánje kamate"
			},
			{
				name: "brrazd",
				description: "je ukupan broj rata u zajmu"
			},
			{
				name: "sv",
				description: "je sadaánja vrijednost ukupnog iznosa niza buduih uplata"
			},
			{
				name: "bv",
				description: "je budua vrijednost ili saldo koji želite postii nakon zadnje uplate, 0 (nula) ako je izostavljen"
			},
			{
				name: "vrsta",
				description: "je logika vrijednost: uplata na poetku razdoblja = 1; uplata na kraju razdoblja = 0 ili je izostavljena"
			}
		]
	},
	{
		name: "POISSON",
		description: "Vraa Poissonovu distribuciju.",
		arguments: [
			{
				name: "x",
				description: "je broj dogaaja"
			},
			{
				name: "srednja",
				description: "je oekivana brojana vrijednost, pozitivan broj"
			},
			{
				name: "kumulativna",
				description: "je logika vrijednost: za kumulativnu Poissonovu vjerojatnost koristite TRUE, za Poissonovu funkciju mase vjerojatnosti koristite FALSE"
			}
		]
	},
	{
		name: "POISSON.DIST",
		description: "Vraa Poissonovu distribuciju.",
		arguments: [
			{
				name: "x",
				description: "je broj dogaaja"
			},
			{
				name: "srednja",
				description: "je oekivana brojana vrijednost, pozitivan broj"
			},
			{
				name: "kumulativna",
				description: "je logika vrijednost: za kumulativnu Poissonovu vjerojatnost koristite TRUE, za Poissonovu funkciju mase vjerojatnosti koristite FALSE"
			}
		]
	},
	{
		name: "POWER",
		description: "Vraa rezultat potenciranja broja na zadanu potenciju.",
		arguments: [
			{
				name: "broj",
				description: "je baza broja, bilo koji realni broj"
			},
			{
				name: "potencija",
				description: "je eksponent na koji se podiže baza"
			}
		]
	},
	{
		name: "PPMT",
		description: "Rauna otplatu glavnice za zadano razdoblje, za ulaganje koje se temelji na periodinim i stalnim uplatama i konstantnoj kamatnoj stopi.",
		arguments: [
			{
				name: "stopa",
				description: "je kamatna stopa po razdoblju. Npr., koristite 6%/4 za kvartalna plaanja uz 6% godiánje kamate"
			},
			{
				name: "razd",
				description: "navodi razdoblje i mora biti u rasponu izmeu 1 i brrazd"
			},
			{
				name: "brrazd",
				description: "je ukupan broj platnih razdoblja u investiciji"
			},
			{
				name: "sv",
				description: "je sadaánja vrijednost: ukupna vrijednost koju niz buduih uplata ima sada"
			},
			{
				name: "bv",
				description: "je budua vrijednost ili saldo koji želite postii nakon zadnje uplate"
			},
			{
				name: "vrsta",
				description: "je logika vrijednost: plaanje na poetku razdoblja = 1; plaanje na kraju razdoblja = 0 ili izostavljena"
			}
		]
	},
	{
		name: "PRICEDISC",
		description: "Vraa cijenu diskontirane vrijednosnice za svakih 100 kn njezine nominalne vrijednosti.",
		arguments: [
			{
				name: "isplata",
				description: "je datum plaanja vrijednosnice, izražen kao serijski broj datuma"
			},
			{
				name: "dospijee",
				description: "je datum dospijea vrijednosnice, izražen kao serijski broj datuma"
			},
			{
				name: "diskont",
				description: "je diskontna stopa za vrijednosnicu"
			},
			{
				name: "otkup",
				description: "je vrijednost po dospijeu za svakih 100 kn nominalne vrijednosti"
			},
			{
				name: "temelj",
				description: "je vrsta brojanja dana koja se koristi"
			}
		]
	},
	{
		name: "PROB",
		description: "Vraa vjerojatnost da su vrijednosti u rasponu izmeu dviju granica ili jednake donja_granica.",
		arguments: [
			{
				name: "x_raspon",
				description: "je raspon brojanih vrijednosti x s kojima su povezane vjerojatnosti"
			},
			{
				name: "vjer_raspon",
				description: "je skup vjerojatnosti povezanih s vrijednostima u x_raspon, vrijednosti izmeu 0 i 1 iskljuujui 0"
			},
			{
				name: "donja_granica",
				description: "je donja granica vrijednosti za koje želite vjerojatnost"
			},
			{
				name: "gornja_granica",
				description: "je neobavezna gornja granica vrijednosti za koje želite vjerojatnost. Ako je ispuátena, PROB vraa vjerojatnost da je vrijednost x_raspon jednaka donja_granica"
			}
		]
	},
	{
		name: "PRODUCT",
		description: "Množi sve brojeve zadane kao argumente.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "je niz od 1 do najviáe 255 brojeva, logikih vrijednosti ili tekstnih oblika brojeva koje želite pomnožiti"
			},
			{
				name: "broj2",
				description: "je niz od 1 do najviáe 255 brojeva, logikih vrijednosti ili tekstnih oblika brojeva koje želite pomnožiti"
			}
		]
	},
	{
		name: "PROPER",
		description: "Pretvara tekstni niz u ispravnu veliinu slova; u veliko slovo prvo slovo u svakoj rijei, a sva ostala slova u mala slova.",
		arguments: [
			{
				name: "tekst",
				description: "je tekst u navodnicima, formula koja vraa tekst ili referenca na eliju koja sadrži tekst koji želite djelomino postaviti u velika slova"
			}
		]
	},
	{
		name: "PV",
		description: "Rauna sadaánju vrijednost investicije: sadaánja vrijednost je trenutna vrijednost ukupnog iznosa niza buduih novanih izdataka.",
		arguments: [
			{
				name: "stopa",
				description: "je kamatna stopa po pojedinom razdoblju. Npr. koristite 6%/4 za kvartalna plaanja uz 6% godiánje kamate"
			},
			{
				name: "brrazd",
				description: "je ukupan broj razdoblja plaanja za jednu investiciju"
			},
			{
				name: "rata",
				description: "je iznos koji uplaujete u svakom razdoblju i ne može se mijenjati tijekom investicije"
			},
			{
				name: "bv",
				description: "je budua vrijednost ili saldo koji želite postii nakon zadnje uplate"
			},
			{
				name: "vrsta",
				description: "je logika vrijednost: uplata na poetku razdoblja = 1; uplata na kraju razdoblja = 0 ili je izostavljena"
			}
		]
	},
	{
		name: "QUARTILE",
		description: "Vraa kvartil skupa podataka.",
		arguments: [
			{
				name: "polje",
				description: "je polje ili raspon elija brojanih vrijednosti za koji želite vrijednost kvartila"
			},
			{
				name: "kvart",
				description: "pokazuje koja se vrijednost vraa: minimalna vrijednost = 0, 1. kvartil = 1, vrijednost medijana = 2, 3. kvartil = 3, maksimalna vrijednost = 4"
			}
		]
	},
	{
		name: "QUARTILE.EXC",
		description: "Vraa kvartil skupa podataka na temelju vrijednosti percentila iz 0..1, ne ukljuujui ta dva broja.",
		arguments: [
			{
				name: "polje",
				description: "je polje ili raspon elija s numerikim vrijednostima za koje želite vrijednost kvartila"
			},
			{
				name: "kvart",
				description: "je broj: minimalna vrijednost = 0, 1. kvartil = 1, vrijednost medijana = 2, 3. kvartil = 3, maksimalna vrijednost = 4"
			}
		]
	},
	{
		name: "QUARTILE.INC",
		description: "Vraa kvartil skupa podataka na temelju vrijednosti percentila iz 0..1, ukljuujui ta dva broja.",
		arguments: [
			{
				name: "polje",
				description: "je polje ili raspon elija s numerikim vrijednostima za koje želite vrijednost kvartila"
			},
			{
				name: "kvart",
				description: "je broj: minimalna vrijednost = 0, 1. kvartil = 1, vrijednost medijana = 2, 3. kvartil = 3, maksimalna vrijednost = 4"
			}
		]
	},
	{
		name: "QUOTIENT",
		description: "Vraa cjelobrojni dio dijeljenja.",
		arguments: [
			{
				name: "brojnik",
				description: "je djeljenik"
			},
			{
				name: "nazivnik",
				description: "je djelitelj"
			}
		]
	},
	{
		name: "RADIANS",
		description: "Pretvara stupnjeve u radijane.",
		arguments: [
			{
				name: "kut",
				description: "je kut koji želite pretvoriti, zadan u stupnjevima"
			}
		]
	},
	{
		name: "RAND",
		description: "Generira sluajni broj vei ili jednak 0 i manji od 1 s jednolikom razdiobom (nakon svakog izraunavanja radnog lista generira se novi sluajni broj).",
		arguments: [
		]
	},
	{
		name: "RANDBETWEEN",
		description: "Vraa sluajni broj izmeu brojeva koje ste odredili.",
		arguments: [
			{
				name: "dno",
				description: "je najmanji cijeli broj koji e vratiti RANDBETWEEN"
			},
			{
				name: "vrh",
				description: "je najvei cijeli broj koji e vratiti RANDBETWEEN"
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
		description: "Vraa položaj broja na popisu brojeva: njegovu relativnu veliinu u odnosu na ostale vrijednosti na popisu.",
		arguments: [
			{
				name: "broj",
				description: "je broj kojemu želite pronai rang"
			},
			{
				name: "ref",
				description: "je raspon ili referenca popisa brojeva. Vrijednosti koje nisu numerike zanemaruju se"
			},
			{
				name: "redoslijed",
				description: "je broj: rang na popisu sortiranom od najviáe vrijednosti = 0 ili se izostavlja, rang na popisu sortiranom od najniže vrijednosti = bilo koji broj razliit od nule"
			}
		]
	},
	{
		name: "RANK.AVG",
		description: "Vraa rang broja na popisu brojeva: njegovu veliinu u odnosu na ostale vrijednosti na popisu. Ako viáe vrijednosti ima isti rang, vraa se prosjeni rang.",
		arguments: [
			{
				name: "broj",
				description: "je broj kojemu želite pronai rang"
			},
			{
				name: "ref",
				description: "je polje ili referenca popisa brojeva. Vrijednosti koje nisu brojane zanemaruju se"
			},
			{
				name: "redoslijed",
				description: "je broj: rang na popisu sortiranom silazno = 0 ili se ispuáta, rang na popisu sortiranom uzlazno = bilo koji broj koji nije nula"
			}
		]
	},
	{
		name: "RANK.EQ",
		description: "Vraa rang broja na popisu brojeva: njegovu veliinu u odnosu na ostale vrijednosti na popisu. Ako viáe vrijednosti ima isti rang, vraa se najviái rang tog skupa vrijednosti.",
		arguments: [
			{
				name: "broj",
				description: "je broj za koji želite saznati rang"
			},
			{
				name: "ref",
				description: "je polje ili referenca popisa brojeva. Vrijednosti koje nisu brojane zanemaruju se"
			},
			{
				name: "redoslijed",
				description: "je broj: rang na popisu sortiranom silazno = 0 ili se ispuáta, rang na popisu sortiranom uzlazno = bilo koji broj koji nije nula"
			}
		]
	},
	{
		name: "RATE",
		description: "Rauna kamatnu stopu po razdoblju zajma ili investicije. Npr. koristite 6%/4 za kvartalna plaanja uz 6% godiánje kamate.",
		arguments: [
			{
				name: "brrazd",
				description: " je ukupan broj razdoblja plaanja za zajam ili investiciju"
			},
			{
				name: "rata",
				description: "je iznos koji se uplauje u svakom razdoblju i ne može se mijenjati tijekom trajanja investicije"
			},
			{
				name: "sv",
				description: "je sadaánja vrijednost: sadaánja ukupna vrijednost niza buduih novanih iznosa"
			},
			{
				name: "bv",
				description: "je budua vrijednost ili saldo koji želite postii nakon zadnje uplate. Ako je izostavljena, koristi se bv =0"
			},
			{
				name: "vrsta",
				description: "je logika vrijednost: uplata na poetku razdoblja = 1; uplata na kraju razdoblja = 0 ili je izostavljena"
			},
			{
				name: "procjena",
				description: "je vaáa procjena stope; ako je izostavljena, procjena = 0,1 (10 posto)"
			}
		]
	},
	{
		name: "RECEIVED",
		description: "Vraa iznos koji se isplauje po dospijeu potpuno investirane vrijednosnice.",
		arguments: [
			{
				name: "isplata",
				description: "je datum plaanja vrijednosnice, izražen kao serijski broj datuma"
			},
			{
				name: "dospijee",
				description: "je datum dospijea vrijednosnice, izražen kao serijski broj datuma"
			},
			{
				name: "ulaganje",
				description: "je iznos investiran u vrijednosnicu"
			},
			{
				name: "diskont",
				description: "je diskontna stopa vrijednosnice"
			},
			{
				name: "temelj",
				description: "je vrsta brojanja dana koja se koristi"
			}
		]
	},
	{
		name: "REPLACE",
		description: "Zamijeni dio tekstnog niza drugim tekstnim nizom.",
		arguments: [
			{
				name: "stari_tekst",
				description: "je tekst u kojemu želite zamijeniti neke znakove"
			},
			{
				name: "poetni_broj",
				description: "je položaj znaka u starom tekstu koji želite zamijeniti novim tekstom"
			},
			{
				name: "broj_znakova",
				description: "je broj znakova u starom tekstu koji želite zamijeniti novim tekstom"
			},
			{
				name: "novi_tekst",
				description: "je tekst koji e zamijeniti znakove starog teksta"
			}
		]
	},
	{
		name: "REPT",
		description: "Ponavlja tekst zadani broj puta. Funkciju REPT koristite za popunjavanje elije odreenim brojem ponavljanja tekstnog niza.",
		arguments: [
			{
				name: "tekst",
				description: "je tekst koji želite ponoviti"
			},
			{
				name: "broj_ponavljanja",
				description: "je pozitivan broj koji odreuje broj ponavljanja teksta"
			}
		]
	},
	{
		name: "RIGHT",
		description: "Vraa zadani broj znakova od kraja tekstnog niza.",
		arguments: [
			{
				name: "tekst",
				description: "je tekstni niz sa znakovima koje želite izdvojiti"
			},
			{
				name: "broj_znakova",
				description: "odreuje broj znakova koje želite izdvojiti; 1 ako je izostavljen"
			}
		]
	},
	{
		name: "ROMAN",
		description: "Pretvara arapski broj u rimski, i to kao tekst.",
		arguments: [
			{
				name: "broj",
				description: "je arapski broj koji želite pretvoriti"
			},
			{
				name: "oblik",
				description: "je broj koji odreuje vrstu rimskog broja."
			}
		]
	},
	{
		name: "ROUND",
		description: "Zaokružuje broj na zadani broj znamenki.",
		arguments: [
			{
				name: "broj",
				description: "je broj koji želite zaokružiti"
			},
			{
				name: "broj_znamenki",
				description: "je broj znamenki na koji želite zaokružiti broj. Ako je manji od nule, broj se zaokružuje nalijevo od decimalnog zareza, ako je nula, broj se zaokružuje na najbliži cijeli broj"
			}
		]
	},
	{
		name: "ROUNDDOWN",
		description: "Zaokružuje zadani broj na broj bliži nuli.",
		arguments: [
			{
				name: "broj",
				description: " je bilo koji realni broj koji želite zaokružiti nadolje"
			},
			{
				name: "broj_znamenki",
				description: "je broj znamenki na koji želite zaokružiti broj. Negativni broj se zaokružuje na lijevo od decimalne toke; ako je nula ili izostavljen, zaokružuje se na najbliži cijeli broj"
			}
		]
	},
	{
		name: "ROUNDUP",
		description: "Zaokružuje na gore na broj udaljeniji od nule.",
		arguments: [
			{
				name: "broj",
				description: "je bilo koji realni broj koji želite zaokružiti"
			},
			{
				name: "broj_znamenki",
				description: "je broj znamenki na koji želite zaokružiti broj. Negativni broj zaokružuje se na lijevo od decimalne toke; ako je nula ili izostavljen, zaokružuje se na najbliži cijeli broj"
			}
		]
	},
	{
		name: "ROW",
		description: "Vraa broj retka reference.",
		arguments: [
			{
				name: "referenca",
				description: "je elija ili raspon elija za koje želite broj retka; ako je izostavljena, vraa eliju koja sadrži funkciju ROW"
			}
		]
	},
	{
		name: "ROWS",
		description: "Vraa broj redaka u referenci ili polju.",
		arguments: [
			{
				name: "polje",
				description: "je polje, formula polja ili referenca na raspon elija za koji želite broj redaka"
			}
		]
	},
	{
		name: "RRI",
		description: "Vraa ekvivlent kamate za rast ulaganja.",
		arguments: [
			{
				name: "nper",
				description: "je broj razdoblja za ulaganje"
			},
			{
				name: "pv",
				description: "je trenutana vrijednost ulaganja"
			},
			{
				name: "fv",
				description: "je budua vrijednost ulaganja"
			}
		]
	},
	{
		name: "RSQ",
		description: "Vraa kvadrat Pearsonovog produkt-momenta korelacijskog koeficijenta kroz toke podataka.",
		arguments: [
			{
				name: "poznati_y",
				description: "je polje ili raspon toaka podataka i mogu biti brojevi ili nazivi, polja ili reference koje sadrže brojeve"
			},
			{
				name: "poznati_x",
				description: "je polje ili raspon toaka podataka i mogu biti brojevi ili nazivi, polja ili reference koje sadrže brojeve"
			}
		]
	},
	{
		name: "RTD",
		description: "Vraa podatke u stvarnom vremenu iz programa koji podržava COM automatizaciju.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "progID",
				description: "je naziv argumenta ProgID registriranog dodatka COM automatizacije. Naziv stavite u navodnike"
			},
			{
				name: "poslužitelj",
				description: "je naziv poslužitelja na kojem e se dodatak pokretati. Naziv stavite u navodnike. Ako je dodatak pokrenut lokalno, koristite prazan niz"
			},
			{
				name: "tema1",
				description: "su 1 do 28 parametara koji navode odreeni podatak"
			},
			{
				name: "tema2",
				description: "su 1 do 28 parametara koji navode odreeni podatak"
			}
		]
	},
	{
		name: "SEARCH",
		description: "Vraa broj znaka na kojem je odreeni znak ili tekstni niz prvi put pronaen, itajui slijeva nadesno (velika i mala slova se ne razlikuju).",
		arguments: [
			{
				name: "pronai_tekst",
				description: "je tekst koji želite pronai. U parametru pronai_tekst možete koristiti zamjenske znakove ? i * ; za pronalaženje znakova ? i * koristite ~? i ~*"
			},
			{
				name: "u_tekstu",
				description: "je tekst u kojem želite tražiti tekst naveden u parametru pronai_tekst"
			},
			{
				name: "poetni_broj",
				description: "je broj znaka u parametru u_tekstu, brojei s lijeva, na kojem želite poeti traženje. Ako je izostavljen, koristi se 1"
			}
		]
	},
	{
		name: "SEC",
		description: "Vraa sekans kuta.",
		arguments: [
			{
				name: "broj",
				description: "je kut u radijanima za koji želite sekans"
			}
		]
	},
	{
		name: "SECH",
		description: "Vraa hiperboliki sekans kuta.",
		arguments: [
			{
				name: "broj",
				description: "je kut u radijanima za koji želite hiperboliki sekans"
			}
		]
	},
	{
		name: "SECOND",
		description: "Vraa sekunde kao broj u rasponu od 0 do 59.",
		arguments: [
			{
				name: "redni_broj",
				description: "je broj u kodu datuma i vremena koji koristi Spreadsheet ili tekst u obliku zapisa vremena, primjerice 16.48:23 ili 4.48:47 PM"
			}
		]
	},
	{
		name: "SERIESSUM",
		description: "Vraa zbroj skupa potencija utemeljenih na formuli.",
		arguments: [
			{
				name: "x",
				description: "je ulazna vrijednost skupa potencija"
			},
			{
				name: "n",
				description: "je poetna potencija na koju želite podii broj x"
			},
			{
				name: "m",
				description: "je stopa po kojoj ete poveavati n za svaki lan skupa"
			},
			{
				name: "koeficijenti",
				description: "je skup koeficijenata s kojim se množi svaka uzastopna potencija broja x"
			}
		]
	},
	{
		name: "SHEET",
		description: "Vraa broj lista na koji se odnosi.",
		arguments: [
			{
				name: "value",
				description: "je naziv lista ili referenca iji broj lista želite. Ako je izostavljen, vraa se broj lista koji sadrži funkciju"
			}
		]
	},
	{
		name: "SHEETS",
		description: "Vraa broj listova u referenci.",
		arguments: [
			{
				name: "reference",
				description: "je referenca iji broj listova želite saznati. Ako je izostavljen, vraa se broj listova u radnoj knjizi koja sadrži funkciju"
			}
		]
	},
	{
		name: "SIGN",
		description: "Utvruje predznak broja: vraa 1 ako je broj pozitivan, nula ako je broj jednak 0 i -1 ako je broj negativan.",
		arguments: [
			{
				name: "broj",
				description: "je bilo koji realan broj"
			}
		]
	},
	{
		name: "SIN",
		description: "Vraa sinus zadanog kuta.",
		arguments: [
			{
				name: "broj",
				description: "je kut zadan u radijanima, za koji tražite sinus. Stupnjevi * PI()/180 = radijani"
			}
		]
	},
	{
		name: "SINH",
		description: "Vraa hiperbolni sinus broja.",
		arguments: [
			{
				name: "broj",
				description: "je bilo koji realni broj"
			}
		]
	},
	{
		name: "SKEW",
		description: "Vraa asimetriju razdiobe: obilježava stupanj asimetrije razdiobe oko njezine sredine.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su 1 do 255 brojeva ili naziva, polja ili referenci koje sadrže brojeve za koje želite izraunati asimetriju"
			},
			{
				name: "broj2",
				description: "su 1 do 255 brojeva ili naziva, polja ili referenci koje sadrže brojeve za koje želite izraunati asimetriju"
			}
		]
	},
	{
		name: "SKEW.P",
		description: "Vraa zakrivljenost distribucije na temelju populacije: svojstvo stupnja asimetrije distribucije oko njezine srednje vrijednosti.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "su 1 do 254 brojeva ili naziva, polja ili referenci koje sadrže brojeve za koje želite zakrivljenost populacije"
			},
			{
				name: "number2",
				description: "su 1 do 254 brojeva ili naziva, polja ili referenci koje sadrže brojeve za koje želite zakrivljenost populacije"
			}
		]
	},
	{
		name: "SLN",
		description: "Vraa linearnu amortizaciju imovine za jedno razdoblje.",
		arguments: [
			{
				name: "cijena",
				description: "je inicijalna vrijednost imovine"
			},
			{
				name: "likvidacija",
				description: "je vrijednost na kraju amortizacije"
			},
			{
				name: "vijek",
				description: "je broj razdoblja za vrijeme kojih je sredstvo amortizirano (ponekad zvano korisni vijek trajanja sredstva)"
			}
		]
	},
	{
		name: "SLOPE",
		description: "Vraa nagib pravca linearne regresije kroz toke podataka.",
		arguments: [
			{
				name: "poznati_y",
				description: "je polje ili raspon elija zavisnih brojanih toaka podataka, a mogu biti brojevi ili nazivi, polja ili reference koje sadrže brojeve"
			},
			{
				name: "poznati_x",
				description: "je skup nezavisnih toaka podataka, a mogu biti brojevi ili nazivi, polja ili reference koje sadrže brojeve"
			}
		]
	},
	{
		name: "SMALL",
		description: "Vraa k-tu najmanju vrijednost u skupu podataka. Primjerice, peti najvei broj.",
		arguments: [
			{
				name: "polje",
				description: "je polje ili raspon brojanih podataka za koji želite odrediti k-tu najmanju vrijednost"
			},
			{
				name: "k",
				description: "je mjesto koje tražena vrijednost zauzima (raunajui od najmanje) u polju ili rasponu vrijednosti"
			}
		]
	},
	{
		name: "SQRT",
		description: "Rauna kvadratni korijen zadanog broja.",
		arguments: [
			{
				name: "broj",
				description: "je broj za koji tražite kvadratni korijen"
			}
		]
	},
	{
		name: "SQRTPI",
		description: "Vraa dvostruki korijen broja (broj * Pi).",
		arguments: [
			{
				name: "broj",
				description: "je vrijednost s kojom se množi broj p"
			}
		]
	},
	{
		name: "STANDARDIZE",
		description: "Vraa normaliziranu vrijednost razdiobe odreene srednjom vrijednosti i standardnom devijacijom.",
		arguments: [
			{
				name: "x",
				description: "je vrijednost koju želite normalizirati"
			},
			{
				name: "srednja",
				description: "je aritmetika sredina razdiobe"
			},
			{
				name: "standardna_dev",
				description: "je standardna devijacija razdiobe, pozitivan broj"
			}
		]
	},
	{
		name: "STDEV",
		description: "Procjenjuje standardnu devijaciju na temelju uzorka (zanemaruje logike vrijednosti i tekst u uzorku).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su od 1 do 255 brojeva koji odgovaraju uzorku populacije i mogu biti brojevi ili reference koje sadrže brojeve"
			},
			{
				name: "broj2",
				description: "su od 1 do 255 brojeva koji odgovaraju uzorku populacije i mogu biti brojevi ili reference koje sadrže brojeve"
			}
		]
	},
	{
		name: "STDEV.P",
		description: "Izraunava standardnu devijaciju na temelju cijele populacije koja je dana u obliku argumenata (zanemaruje logike vrijednosti i tekst).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su od 1 od 255 brojeva koji odgovaraju populaciji, a mogu biti brojevi ili reference koje sadrže brojeve"
			},
			{
				name: "broj2",
				description: "su od 1 od 255 brojeva koji odgovaraju populaciji, a mogu biti brojevi ili reference koje sadrže brojeve"
			}
		]
	},
	{
		name: "STDEV.S",
		description: "Procjenjuje standardnu devijaciju na temelju uzorka (zanemaruje logike vrijednosti i tekst u uzorku).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su od 1 do 255 brojeva koji odgovaraju uzorku populacije, a mogu biti brojevi ili reference koje sadrže brojeve"
			},
			{
				name: "broj2",
				description: "su od 1 do 255 brojeva koji odgovaraju uzorku populacije, a mogu biti brojevi ili reference koje sadrže brojeve"
			}
		]
	},
	{
		name: "STDEVA",
		description: "Procjenjuje standardnu devijaciju prema uzorku, ukljuujui logike vrijednosti i tekst. Tekst i logika vrijednost FALSE vrijede 0; logika vrijednost TRUE vrijedi 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrijednost1",
				description: "su 1 do 255 vrijednosti koje odgovaraju uzorku populacije i mogu biti vrijednosti ili nazivi ili reference na vrijednosti"
			},
			{
				name: "vrijednost2",
				description: "su 1 do 255 vrijednosti koje odgovaraju uzorku populacije i mogu biti vrijednosti ili nazivi ili reference na vrijednosti"
			}
		]
	},
	{
		name: "STDEVP",
		description: "Izraunava standardnu devijaciju na temelju cijele populacije dane u obliku argumenata (zanemaruje logike vrijednosti i tekst).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su od 1 do 255 brojeva koji odgovaraju populaciji, a mogu biti brojevi ili reference koje sadrže brojeve"
			},
			{
				name: "broj2",
				description: "su od 1 do 255 brojeva koji odgovaraju populaciji, a mogu biti brojevi ili reference koje sadrže brojeve"
			}
		]
	},
	{
		name: "STDEVPA",
		description: "Rauna standardnu devijaciju za cijelu populaciju, ukljuujui logike vrijednosti i tekst. Tekst i logika vrijednost FALSE vrijede 0; logika vrijednost TRUE vrijedi 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrijednost1",
				description: "su 1 do 255 vrijednosti koje odgovaraju populaciji, a mogu biti vrijednosti, nazivi, polja ili reference koje sadrže vrijednosti"
			},
			{
				name: "vrijednost2",
				description: "su 1 do 255 vrijednosti koje odgovaraju populaciji, a mogu biti vrijednosti, nazivi, polja ili reference koje sadrže vrijednosti"
			}
		]
	},
	{
		name: "STEYX",
		description: "Vraa standardnu pogreáku predviene vrijednosti y za svaki x u regresiji.",
		arguments: [
			{
				name: "poznati_y",
				description: "je polje ili raspon zavisnih toaka podataka i mogu biti brojevi ili nazivi, polja ili reference koje sadrže brojeve"
			},
			{
				name: "poznati_x",
				description: "je polje ili raspon nezavisnih toaka podataka i mogu biti brojevi ili nazivi, polja ili reference koje sadrže brojeve"
			}
		]
	},
	{
		name: "SUBSTITUTE",
		description: "Zamjenjuje postojei tekst novim tekstom u tekstnom nizu.",
		arguments: [
			{
				name: "tekst",
				description: "je tekst ili referenca na eliju koja sadrži tekst za koji želite zamijeniti znakove"
			},
			{
				name: "stari_tekst",
				description: "je postojei tekst koji želite zamijeniti. Ako veliina slova starog teksta ne odgovara veliini slova teksta, SUBSTITUTE nee zamijeniti tekst"
			},
			{
				name: "novi_tekst",
				description: "je tekst kojim zamjenjujete stari tekst"
			},
			{
				name: "broj_instance",
				description: "navodi koju instancu starog teksta želite zamijeniti novim tekstom. Ako ga izostavite, bit e zamijenjene sve instance starog teksta"
			}
		]
	},
	{
		name: "SUBTOTAL",
		description: "Vraa podzbroj u popisu ili bazi podataka.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "funkcija_broj",
				description: "je broj od 1 do 11 koji odreuje koja e se funkcija koristiti za izraun podzbrojeva unutar popisa."
			},
			{
				name: "ref1",
				description: "su 1 do 254 raspona ili referenci za koje želite podzbroj"
			}
		]
	},
	{
		name: "SUM",
		description: "Zbraja sve brojeve u rasponu elija.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "je niz od 1 do 255 argumenata zbroja. Logike vrijednosti i tekst se zanemaruju, ak i ako ih upiáete kao argumente"
			},
			{
				name: "broj2",
				description: "je niz od 1 do 255 argumenata zbroja. Logike vrijednosti i tekst se zanemaruju, ak i ako ih upiáete kao argumente"
			}
		]
	},
	{
		name: "SUMIF",
		description: "Zbraja elije odreene danim kriterijem.",
		arguments: [
			{
				name: "raspon",
				description: "je raspon elija koje želite vrednovati"
			},
			{
				name: "kriteriji",
				description: "je uvjet ili kriterij u obliku broja, izraza ili teksta koji definira koje elije e biti zbrojene"
			},
			{
				name: "raspon_zbroja",
				description: "su elije koje treba zbrojiti. Ako se izostavi, zbrajaju se sve elije u rasponu"
			}
		]
	},
	{
		name: "SUMIFS",
		description: "Zbraja elije odreene danim skupom uvjeta ili kriterija.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "raspon_zbroja",
				description: "su elije koje e se zbrajati"
			},
			{
				name: "raspon_kriterija",
				description: "je raspon elija koji želite provjeravati u odnosu na neki uvjet"
			},
			{
				name: "kriteriji",
				description: "je uvjet ili kriterij u obliku broja, izraza ili teksta,koji odreuje koje e se elije zbrajati"
			}
		]
	},
	{
		name: "SUMPRODUCT",
		description: "Rauna zbroj umnožaka podudarajuih raspona ili polja.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "polje1",
				description: "je niz od 2 do 255 polja ije komponente želite pomnožiti i zatim zbrojiti. Sva polja moraju biti jednakih dimenzija"
			},
			{
				name: "polje2",
				description: "je niz od 2 do 255 polja ije komponente želite pomnožiti i zatim zbrojiti. Sva polja moraju biti jednakih dimenzija"
			},
			{
				name: "polje3",
				description: "je niz od 2 do 255 polja ije komponente želite pomnožiti i zatim zbrojiti. Sva polja moraju biti jednakih dimenzija"
			}
		]
	},
	{
		name: "SUMSQ",
		description: "Vraa zbroj kvadrata argumenata. Argumenti mogu biti brojevi, polja, nazivi ili reference na elije koje sadrže brojeve.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su 1 do 255 brojeva, polja, naziva ili referenci na polja za koje želite zbroj kvadrata"
			},
			{
				name: "broj2",
				description: "su 1 do 255 brojeva, polja, naziva ili referenci na polja za koje želite zbroj kvadrata"
			}
		]
	},
	{
		name: "SUMX2MY2",
		description: "Zbraja razlike kvadrata dvaju raspona ili polja.",
		arguments: [
			{
				name: "polje_x",
				description: "je prvi raspon ili polje brojeva i može biti broj ili naziv, polje ili referenca koja sadrži brojeve"
			},
			{
				name: "polje_y",
				description: "je drugi raspon ili polje brojeva i može biti broj ili naziv, polje ili referenca koja sadrži brojeve"
			}
		]
	},
	{
		name: "SUMX2PY2",
		description: "Rauna ukupni zbroj zbrojeva kvadrata brojeva iz dvaju raspona ili polja.",
		arguments: [
			{
				name: "polje_x",
				description: "je prvi raspon ili polje brojeva i može biti broj ili naziv, polje ili referenca koja sadrži brojeve"
			},
			{
				name: "polje_y",
				description: "je drugi raspon ili polje brojeva i može biti broj ili naziv, polje ili referenca koja sadrži brojeve"
			}
		]
	},
	{
		name: "SUMXMY2",
		description: "Zbraja kvadrate razlika dvaju raspona ili polja.",
		arguments: [
			{
				name: "polje_x",
				description: "je prvi raspon ili polje vrijednosti, a može biti broj ili naziv, polje ili referenca koja sadrži brojeve"
			},
			{
				name: "polje_y",
				description: "je drugi raspon ili polje vrijednosti, a može biti broj ili naziv, polje ili referenca koja sadrži brojeve"
			}
		]
	},
	{
		name: "SYD",
		description: "Vraa znamenke zbroja godina amortizacije imovine za navedeno razdoblje.",
		arguments: [
			{
				name: "cijena",
				description: "je poetna vrijednost sredstva"
			},
			{
				name: "likvidacija",
				description: "je vrijednost na kraju amortizacije"
			},
			{
				name: "vijek",
				description: "je broj razdoblja za vrijeme kojih je sredstvo amortizirano (ponekad zvano korisni vijek trajanja sredstva)"
			},
			{
				name: "razd",
				description: "je razdoblje i mora koristiti iste jedinice kao vijek"
			}
		]
	},
	{
		name: "T",
		description: "Provjerava je li vrijednost tekst; ako jest, vraa tekst, a ako nije, dvostruke navodnike (prazan tekst).",
		arguments: [
			{
				name: "vrijednost",
				description: "je vrijednost koju želite provjeriti"
			}
		]
	},
	{
		name: "T.DIST",
		description: "Vraa ljevokraku Studentovu t-distribuciju.",
		arguments: [
			{
				name: "x",
				description: "je numerika vrijednost na kojoj se vrednuje distribucija"
			},
			{
				name: "stupanj_slobode",
				description: "je cijeli broj koji oznaava broj stupnjeva slobode koji karakteriziraju distribuciju"
			},
			{
				name: "kumulativna",
				description: "je logika vrijednost: za kumulativnu funkciju distribucije koristite TRUE, za funkciju gustoe vjerojatnosti koristite FALSE"
			}
		]
	},
	{
		name: "T.DIST.2T",
		description: "Vraa dvokraku Studentovu t-distribuciju.",
		arguments: [
			{
				name: "x",
				description: "je numerika vrijednost na kojoj se vrednuje distribucija"
			},
			{
				name: "stupanj_slobode",
				description: "je cijeli broj koji oznaava stupnjeve slobode koji karakteriziraju distribuciju"
			}
		]
	},
	{
		name: "T.DIST.RT",
		description: "Vraa desnokraku Studentovu t-distribuciju.",
		arguments: [
			{
				name: "x",
				description: "je numerika vrijednost na kojoj se vrednuje distribucija"
			},
			{
				name: "stupanj_slobode",
				description: "je cijeli broj koji oznaava broj stupnjeva slobode koji karakteriziraju distribuciju"
			}
		]
	},
	{
		name: "T.INV",
		description: "Vraa ljevokraku inverziju Studentove t-distribucije.",
		arguments: [
			{
				name: "vjerojatnost",
				description: "je vjerojatnost pridružena dvokrakoj Studentovoj t-distribuciji, broj izmeu 0 i 1, ukljuujui ta dva broja"
			},
			{
				name: "stupanj_slobode",
				description: "je pozitivan cijeli broj koji oznaava broj stupnjeva slobode koji karakteriziraju distribuciju"
			}
		]
	},
	{
		name: "T.INV.2T",
		description: "Vraa dvokraku inverziju Studentove t-distribucije.",
		arguments: [
			{
				name: "vjerojatnost",
				description: "je vjerojatnost pridružena dvokrakoj Studentovoj t-distribuciji, broj izmeu 0 i 1, ukljuujui ta dva broja"
			},
			{
				name: "stupanj_slobode",
				description: "je pozitivan cijeli broj koji oznaava broj stupnjeva slobode koji karakteriziraju distribuciju"
			}
		]
	},
	{
		name: "T.TEST",
		description: "Vraa vjerojatnost povezanu sa Studentovim t-testom.",
		arguments: [
			{
				name: "polje1",
				description: "je prvi skup podataka"
			},
			{
				name: "polje2",
				description: "je drugi skup podataka"
			},
			{
				name: "krakovi",
				description: "odreuje broj krakova koje treba vratiti: jednokraka distribucija = 1, dvokraka distribucija = 2"
			},
			{
				name: "vrsta",
				description: "je vrsta t-testa: upareni = 1, jednaka varijanca dvaju uzoraka (homoskedastini) = 2, razliite varijance dvaju uzoraka = 3"
			}
		]
	},
	{
		name: "TAN",
		description: "Vraa tangens zadanog kuta.",
		arguments: [
			{
				name: "broj",
				description: "je kut zadan u radijanima za koji tražite tangens. Stupnjevi * PI()/180 = radijani"
			}
		]
	},
	{
		name: "TANH",
		description: "Rauna hiperbolni tangens broja.",
		arguments: [
			{
				name: "broj",
				description: "je bilo koji realni broj"
			}
		]
	},
	{
		name: "TBILLEQ",
		description: "Vraa ekvivalent kamati na obveznice za državnu obveznicu.",
		arguments: [
			{
				name: "isplata",
				description: "je datum plaanja državne obveznice, izražen kao serijski broj datuma"
			},
			{
				name: "dospijee",
				description: "je datum dospijea državne obveznice, izražen kao serijski broj datuma"
			},
			{
				name: "diskont",
				description: "je diskontna stopa državne obveznice"
			}
		]
	},
	{
		name: "TBILLPRICE",
		description: "Vraa cijenu državne obveznice za svakih 100 kn njezine nominalne vrijednosti.",
		arguments: [
			{
				name: "isplata",
				description: "je datum plaanja državne obveznice, izražen kao serijski broj datuma"
			},
			{
				name: "dospijee",
				description: "je datum dospijea državne obveznice, izražen kao serijski broj datuma"
			},
			{
				name: "diskont",
				description: "je diskontna stopa državne obveznice"
			}
		]
	},
	{
		name: "TBILLYIELD",
		description: "Vraa prinos po državnoj obveznici.",
		arguments: [
			{
				name: "isplata",
				description: "je datum plaanja državne obveznice, izražen kao serijski broj datuma"
			},
			{
				name: "dospijee",
				description: "je datum dospijea državne obveznice, izražen kao serijski broj datuma"
			},
			{
				name: "cijena",
				description: "je cijena državne obveznice za svakih 100 kn njezine nominalne vrijednosti"
			}
		]
	},
	{
		name: "TDIST",
		description: "Vraa Studentovu t-razdiobu.",
		arguments: [
			{
				name: "x",
				description: "je brojana vrijednost na kojoj se vrednuje razdioba"
			},
			{
				name: "stupanj_slobode",
				description: "je cijeli broj koji oznaava broj stupnjeva slobode koji karakteriziraju razdiobu"
			},
			{
				name: "krakovi",
				description: "navodi broj krakova razdiobe koje treba vratiti: jednokraka razdioba = 1; dvokraka razdioba = 2"
			}
		]
	},
	{
		name: "TEXT",
		description: "Pretvara vrijednost u tekst po odreenom brojanom obliku.",
		arguments: [
			{
				name: "vrijednost",
				description: "je broj, formula koja izraunava brojanu vrijednost ili referenca na eliju koja sadrži brojanu vrijednost"
			},
			{
				name: "oblik_tekst",
				description: "je oblik broja zadan u tekstnom obliku iz okvira Kategorija na kartici Broj u dijaloákom okviru Oblikovanje elija (ne Openito)"
			}
		]
	},
	{
		name: "TIME",
		description: "Brojani oblik sati, minuta i sekundi pretvara u serijski broj programa Spreadsheet u obliku vremenskog zapisa.",
		arguments: [
			{
				name: "sat",
				description: "je broj od 0 do 23 koji predstavlja sat"
			},
			{
				name: "minuta",
				description: "je broj od 0 do 59 koji predstavlja minute"
			},
			{
				name: "sekunda",
				description: "je broj od 0 do 59 koji predstavlja sekunde"
			}
		]
	},
	{
		name: "TIMEVALUE",
		description: "Pretvara vrijeme u tekstnom obliku u serijski broj programa Spreadsheet za vrijeme kao broj od 0 (00.00:00) do 0,999988426 (23.59:59). Oblikuje broj u obliku zapisa vremena nakon unosa formule.",
		arguments: [
			{
				name: "vrijeme_tekst",
				description: "je tekstni niz koji vraa vrijeme u bilo kojem obliku zapisa vremena programa Spreadsheet (zanemaruju se informacije o datumu u nizu)"
			}
		]
	},
	{
		name: "TINV",
		description: "Vraa inverziju Studentove t-distribucije za navedeni stupanj slobode.",
		arguments: [
			{
				name: "vjerojatnost",
				description: "je vjerojatnost pridružena dvokrakoj Studentovoj t-distribuciji, broj izmeu 0 i 1, ukljuujui ta dva broja"
			},
			{
				name: "stupanj_slobode",
				description: "je pozitivan cijeli broj koji oznaava broj stupnjeva slobode koji karakteriziraju distribuciju"
			}
		]
	},
	{
		name: "TODAY",
		description: "Vraa trenutni datum oblikovan kao datum.",
		arguments: [
		]
	},
	{
		name: "TRANSPOSE",
		description: "Okomiti raspon elija pretvara u vodoravni ili obrnuto.",
		arguments: [
			{
				name: "polje",
				description: "je raspon elija na radnom listu ili polje koje želite transponirati"
			}
		]
	},
	{
		name: "TREND",
		description: "Rauna brojeve u linearnom trendu podudaranjem poznatih toaka podataka metodom najmanjih kvadrata.",
		arguments: [
			{
				name: "poznati_y",
				description: "je raspon ili polje vrijednosti y koje su vam ve poznate u odnosu y = mx + b"
			},
			{
				name: "poznati_x",
				description: "je neobavezni raspon ili polje vrijednosti x koje su vam ve poznate u odnosu y = mx + b, jedno polje jednake veliine kao i poznati_y"
			},
			{
				name: "novi_x",
				description: "je raspon ili polje vrijednosti x za koje želite da TREND vrati odgovarajue vrijednosti y"
			},
			{
				name: "konst",
				description: "je logika vrijednost: konstanta b rauna se normalno ako je konst = TRUE ili je izostavljen; b = 0 ako je konst = FALSE"
			}
		]
	},
	{
		name: "TRIM",
		description: "Uklanja sve razmake iz teksta ostavljajui po jedan razmak izmeu rijei.",
		arguments: [
			{
				name: "tekst",
				description: "je tekst iz kojeg želite ukloniti razmake"
			}
		]
	},
	{
		name: "TRIMMEAN",
		description: "Vraa srednju vrijednost unutraánjosti skupa podataka.",
		arguments: [
			{
				name: "polje",
				description: "je polje ili raspon vrijednosti u kojem želite odbaciti neke vrijednosti i izraunati prosjek"
			},
			{
				name: "postotak",
				description: "je decimalni broj toaka podataka koje e biti iskljuene s vrha i dna skupa podataka"
			}
		]
	},
	{
		name: "TRUE",
		description: "Vraa logiku vrijednost TRUE.",
		arguments: [
		]
	},
	{
		name: "TRUNC",
		description: "Odbacuje decimalni dio broja pretvarajui ga u cijeli broj.",
		arguments: [
			{
				name: "broj",
				description: "je broj kojemu odbacujete decimale"
			},
			{
				name: "broj_znamenki",
				description: "je broj koji navodi preciznost kraenja, 0 (nula) ako je izostavljen"
			}
		]
	},
	{
		name: "TTEST",
		description: "Vraa vjerojatnost povezanu sa Studentovim t-testom.",
		arguments: [
			{
				name: "polje1",
				description: "je prvi skup podataka"
			},
			{
				name: "polje2",
				description: "je drugi skup podataka"
			},
			{
				name: "krakovi",
				description: "odreuje broj krakova distribucije koje treba vratiti: jednokraka distribucija = 1, dvokraka distribucija = 2"
			},
			{
				name: "vrsta",
				description: "je vrsta t-testa: upareni = 1, jednaka varijanca dvaju uzorka (homoskedastini) = 2, razliite varijance dvaju uzoraka = 3"
			}
		]
	},
	{
		name: "TYPE",
		description: "Vraa cijeli broj koji oznaava vrstu podatka: broj = 1; tekst = 2; logika vrijednost = 4; vrijednost pogreáke = 16; polje = 64.",
		arguments: [
			{
				name: "vrijednost",
				description: "može biti bilo koja vrijednost"
			}
		]
	},
	{
		name: "UNICODE",
		description: "Vraa broj (kodnu toku) koji odgovara prvom znaku teksta.",
		arguments: [
			{
				name: "text",
				description: "je znak iju Unicode vrijednost želite"
			}
		]
	},
	{
		name: "UPPER",
		description: "Pretvara tekstni niz u sva velika slova.",
		arguments: [
			{
				name: "tekst",
				description: "je tekst koji želite pretvoriti u velika slova, referencu ili tekstni niz"
			}
		]
	},
	{
		name: "VALUE",
		description: "Broj u obliku tekstnog niza pretvara u broj.",
		arguments: [
			{
				name: "tekst",
				description: "je tekst u navodnicima ili referenca na eliju koja sadrži tekst koji želite pretvoriti"
			}
		]
	},
	{
		name: "VAR",
		description: "Procjenjuje varijancu na temelju uzorka (zanemaruje logike vrijednosti i tekst u uzorku).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su od 1 do 255 brojanih argumenata koji odgovaraju uzorku populacije"
			},
			{
				name: "broj2",
				description: "su od 1 do 255 brojanih argumenata koji odgovaraju uzorku populacije"
			}
		]
	},
	{
		name: "VAR.P",
		description: "Izraunava varijancu na temelju cijele populacije (zanemaruje logike vrijednosti i tekst u populaciji).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su od 1 do 255 brojanih argumenata koji odgovaraju populaciji"
			},
			{
				name: "broj2",
				description: "su od 1 do 255 brojanih argumenata koji odgovaraju populaciji"
			}
		]
	},
	{
		name: "VAR.S",
		description: "Procjenjuje varijancu na temelju uzorka (zanemaruje logike vrijednosti i tekst u uzorku).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su od 1 do 255 brojanih argumenata koji odgovaraju uzorku populacije"
			},
			{
				name: "broj2",
				description: "su od 1 do 255 brojanih argumenata koji odgovaraju uzorku populacije"
			}
		]
	},
	{
		name: "VARA",
		description: "Procjenjuje varijancu na temelju uzorka, ukljuujui logike vrijednosti i tekst. Tekst i logika vrijednost FALSE vrijede 0; logika vrijednost TRUE vrijedi 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrijednost1",
				description: "su 1 do 255 vrijednosti argumenata koje odgovaraju uzorku populacije"
			},
			{
				name: "vrijednost2",
				description: "su 1 do 255 vrijednosti argumenata koje odgovaraju uzorku populacije"
			}
		]
	},
	{
		name: "VARP",
		description: "Izraunava varijancu na temelju cijele populacije (zanemaruje logike vrijednosti i tekst u populaciji).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su od 1 do 255 brojanih argumenata koji odgovaraju populaciji"
			},
			{
				name: "broj2",
				description: "su od 1 do 255 brojanih argumenata koji odgovaraju populaciji"
			}
		]
	},
	{
		name: "VARPA",
		description: "Rauna varijancu utemeljenu na cijeloj populaciji, ukljuujui logike vrijednosti i tekst. Tekst i logika vrijednost FALSE vrijede 0; logika vrijednosti TRUE vrijedi 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrijednost1",
				description: "su 1 do 255 vrijednosti argumenata koje odgovaraju populaciji"
			},
			{
				name: "vrijednost2",
				description: "su 1 do 255 vrijednosti argumenata koje odgovaraju populaciji"
			}
		]
	},
	{
		name: "VDB",
		description: "Vraa amortizaciju imovine za svako razdoblje, pa i parcijalna razdoblja, koristei fiksnu stopu od 200% stope otpisa ili neku drugu metodu.",
		arguments: [
			{
				name: "cijena",
				description: "je poetna vrijednost imovine"
			},
			{
				name: "likvidacija",
				description: "je vrijednost na kraju amortizacije"
			},
			{
				name: "vijek",
				description: "je broj razdoblja tijekom kojih se imovina amortizira (ponekad se naziva vrijeme upotrebljivosti imovine)"
			},
			{
				name: "poetno_razdoblje",
				description: "je poetno razdoblje za koje želite izraunati amortizaciju u istim jedinicama kao i vijek"
			},
			{
				name: "zavráno_razdoblje",
				description: "je posljednje razdoblje za koje želite izraunati amortizaciju u istim jedinicama kao i vijek"
			},
			{
				name: "faktor",
				description: "je stopa po kojoj se umanjuje knjigovodstvena vrijednost imovine, 2 (metoda ubrzane deprecijacije) ako je ispuáten"
			},
			{
				name: "bez_prebacivanja",
				description: "prebacivanje na linearnu amortizaciju kada je amortizacija vea od prorauna amortizacije fiksnom stopom = FALSE ili izostavljeno; ne prebacuje = TRUE"
			}
		]
	},
	{
		name: "VLOOKUP",
		description: "Traži vrijednost u krajnjem lijevom stupcu tablice, a vraa vrijednost u istom retku iz navedenog stupca. Tablica mora biti sortirana uzlazno.",
		arguments: [
			{
				name: "vrijednost_pretraživanja",
				description: "je vrijednost koja se traži u prvom stupcu tablice, a može biti vrijednost, referenca ili tekstni niz"
			},
			{
				name: "polje_tablica",
				description: "je tablica teksta, brojeva ili logikih vrijednosti u kojoj se traže podaci. Polje_tablica može biti referenca na raspon ili naziv raspona"
			},
			{
				name: "indeks_retka",
				description: "je broj stupca u parametru polje_tablica iz kojeg e se vraati uparene vrijednosti. Prvi stupac vrijednosti u tablici je 1"
			},
			{
				name: "raspon_pretraživanja",
				description: "je logika vrijednost: za pronalaženje najbliže odgovarajue vrijednosti u prvom stupcu (sortirano po uzlaznom redoslijedu) = TRUE ili izostavljeno; za traženje identine vrijednosti = FALSE"
			}
		]
	},
	{
		name: "WEEKDAY",
		description: "Vraa broj od 1 do 7 koji oznaava dan u tjednu datuma.",
		arguments: [
			{
				name: "redni_broj",
				description: "je broj koji predstavlja datum"
			},
			{
				name: "vrsta_rezultata",
				description: "je broj: za nedjelja=1 do subote=7 koristite 1; za ponedjeljak=1 do nedjelje=7 koristite 2; za ponedjeljak=0 do nedjelje=6 koristite 3"
			}
		]
	},
	{
		name: "WEEKNUM",
		description: "Vraa broj tjedna u godini.",
		arguments: [
			{
				name: "redni_broj",
				description: "je kod datuma i vremena koji se u programu Spreadsheet koristi za izraune datuma i vremena"
			},
			{
				name: "vrsta_rezultata",
				description: "je broj (1 ili 2) koji odreuje vrstu vraene vrijednosti"
			}
		]
	},
	{
		name: "WEIBULL",
		description: "Vraa Weibullovu distribuciju.",
		arguments: [
			{
				name: "x",
				description: "je vrijednost na kojoj se vrednuje funkcija, ne može biti negativan broj"
			},
			{
				name: "alfa",
				description: "je parametar distribucije, pozitivan broj"
			},
			{
				name: "beta",
				description: "je parametar distribucije, pozitivan broj"
			},
			{
				name: "kumulativna",
				description: "je logika vrijednost: za kumulativnu funkciju vjerojatnosti koristite TRUE; za funkciju mase vjerojatnosti koristite FALSE"
			}
		]
	},
	{
		name: "WEIBULL.DIST",
		description: "Rauna Weibullovu distribuciju.",
		arguments: [
			{
				name: "x",
				description: "je vrijednost na kojoj se izraunava funkcija, ne može biti negativan broj"
			},
			{
				name: "alfa",
				description: "je parametar distribucije, pozitivan broj"
			},
			{
				name: "beta",
				description: "je parametar distribucije, pozitivan broj"
			},
			{
				name: "kumulativna",
				description: "je logika vrijednost: za kumulativnu funkciju vjerojatnosti koristite TRUE; za funkciju mase vjerojatnosti koristite  FALSE"
			}
		]
	},
	{
		name: "WORKDAY",
		description: "Vraa serijski broj datuma koji se nalazi prije ili poslije odreenog broja radnih dana.",
		arguments: [
			{
				name: "poetni_datum",
				description: "je serijski broj datuma koji predstavlja poetni datum"
			},
			{
				name: "dani",
				description: "je broj radnih dana prije ili nakon poetnog datuma"
			},
			{
				name: "praznici",
				description: "je neobavezno polje od jednog ili viáe serijskih brojeva datuma koji e biti iskljueni iz radnog kalendara, poput državnih i kliznih praznika"
			}
		]
	},
	{
		name: "WORKDAY.INTL",
		description: "Vraa redni broj datuma prije ili nakon navedenog broja radnih dana s prilagoenim parametrima vikenda.",
		arguments: [
			{
				name: "poetni_datum",
				description: "je redni broj datuma koji oznaava poetni datum"
			},
			{
				name: "dani",
				description: "je broj dana koji nisu vikendi ni blagdani prije ili nakon datuma navedenog u argumentu poetni_datum"
			},
			{
				name: "vikend",
				description: "je broj niza koji odreuje kada dolaze vikendi"
			},
			{
				name: "praznici",
				description: "je neobavezno polje jednog ili viáe serijskih brojeva datuma koje valja izuzeti iz kalendara radnih dana, npr. državne i pomine praznike"
			}
		]
	},
	{
		name: "XIRR",
		description: "Vraa internu stopu povrata za raspored novanih tokova.",
		arguments: [
			{
				name: "vrijednosti",
				description: "je skup novanih tokova koji odgovara rasporedu plaanja u datumima"
			},
			{
				name: "datumi",
				description: "je raspored datuma plaanja koji odgovara uplatama novanih tokova"
			},
			{
				name: "procjena",
				description: "je broj za koji pretpostavljate da je blizu vrijednosti interne stope povrata XIRR"
			}
		]
	},
	{
		name: "XNPV",
		description: "Vraa trenutnu vrijednost za raspored novanih tokova.",
		arguments: [
			{
				name: "stopa",
				description: "je diskontna stopa koja e se primijeniti na novane tokove"
			},
			{
				name: "vrijednosti",
				description: "je skup novanih tokova koji odgovara rasporedu plaanja u datumima"
			},
			{
				name: "datumi",
				description: "je raspored plaanja koji odgovara uplatama novanih tokova"
			}
		]
	},
	{
		name: "XOR",
		description: "Vraa logiki 'ekskluzivni ili' svih argumenata.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "je 1 do 254 uvjeta koje želite testirati i ija vrijednost može biti TRUE ili FALSE i može biti logika vrijednost, polje ili referenca"
			},
			{
				name: "logical2",
				description: "je 1 do 254 uvjeta koje želite testirati i ija vrijednost može biti TRUE ili FALSE i može biti logika vrijednost, polje ili referenca"
			}
		]
	},
	{
		name: "YEAR",
		description: "Vraa godinu iz datuma kao cijeli broj u rasponu od 1900 do 9999.",
		arguments: [
			{
				name: "redni_broj",
				description: "je broj u kodu datuma i vremena koji koristi Spreadsheet"
			}
		]
	},
	{
		name: "YEARFRAC",
		description: "Vraa dio godine koji predstavlja broj cijelih dana izmeu poetnog datuma i zavránog datuma.",
		arguments: [
			{
				name: "poetni_datum",
				description: "je serijski broj datuma koji predstavlja poetni datum"
			},
			{
				name: "zavráni_datum",
				description: "je serijski broj datuma koji predstavlja zavráni datum"
			},
			{
				name: "temelj",
				description: "je vrsta brojanja dana koja se koristi"
			}
		]
	},
	{
		name: "YIELDDISC",
		description: "Vraa godiánji prinos diskontirane vrijednosnice. Na primjer, državne obveznice.",
		arguments: [
			{
				name: "isplata",
				description: "je datum plaanja vrijednosnice, izražen kao serijski broj datuma"
			},
			{
				name: "dospijee",
				description: "je datum dospijea vrijednosnice, izražen kao serijski broj datuma"
			},
			{
				name: "cijena",
				description: "je cijena vrijednosnice za svakih 100 kn njezine nominalne vrijednosti"
			},
			{
				name: "otkup",
				description: "je vrijednost po dospijeu za svakih 100 kn nominalne vrijednosti"
			},
			{
				name: "temelj",
				description: "je vrsta brojanja dana koja se koristi"
			}
		]
	},
	{
		name: "Z.TEST",
		description: "Vraa jednokraku P-vrijednost z-testa.",
		arguments: [
			{
				name: "polje",
				description: "je polje ili raspon podataka prema kojima testiramo X"
			},
			{
				name: "x",
				description: "je vrijednost koju testiramo"
			},
			{
				name: "sigma",
				description: "je (poznata) standardna devijacija populacije. Ako se izostavi, koristi se standardna devijacija uzorka"
			}
		]
	},
	{
		name: "ZTEST",
		description: "Vraa P-vrijednost z-testa.",
		arguments: [
			{
				name: "polje",
				description: "je polje ili raspon podataka prema kojima se testira X"
			},
			{
				name: "x",
				description: "je vrijednost koja se testira"
			},
			{
				name: "sigma",
				description: "je (poznata) standardna devijacija populacije. Ako se izostavi, koristi se standardna devijacija uzorka"
			}
		]
	}
];