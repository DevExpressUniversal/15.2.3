ASPxClientSpreadsheet.Functions = [
	{
		name: "ABS",
		description: "Daje apsolutnu vrednost broja, tj. broj bez znaka.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja realni broj čiju apsolutnu vrednost želite"
			}
		]
	},
	{
		name: "ACCRINTM",
		description: "Daje nagomilanu kamatu za pokriće za koje se kamata plaća po dospeću.",
		arguments: [
			{
				name: "problem",
				description: "je datum izdavanja pokrića, izražen kao serijski broj datuma"
			},
			{
				name: "poravnanje",
				description: "je datum dospevanja pokrića, izražen kao serijski broj datuma"
			},
			{
				name: "stopa",
				description: "je godišnja stopa pokrića za kupone"
			},
			{
				name: "par",
				description: "je redovna vrednost pokrića"
			},
			{
				name: "osnova",
				description: "je tip osnovice za brojanje dana koji bi trebalo koristiti"
			}
		]
	},
	{
		name: "ACOS",
		description: "Daje arkus kosinus broja, u radijanima u opsegu od 0 do Pi. Arkus kosinus je ugao čiji je kosinus broj.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja kosinus ugla koji želite i njegova vrednost mora biti od -1 do 1"
			}
		]
	},
	{
		name: "ACOSH",
		description: "Daje inverzni kosinus hiperbolički broja.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja bilo koji realan broj jednak ili veći od 1"
			}
		]
	},
	{
		name: "ACOT",
		description: "Vraća arkus kotangens broja u radijanima u opsegu od 0 do pi.",
		arguments: [
			{
				name: "number",
				description: "je kotangens od ugla koji želite"
			}
		]
	},
	{
		name: "ACOTH",
		description: "Vraća recipročni hiperbolički kotangens broja.",
		arguments: [
			{
				name: "number",
				description: "je hiperbolički kotangens od ugla koji želite"
			}
		]
	},
	{
		name: "ADDRESS",
		description: "Kreira referencu na ćeliju kao tekst, na osnovu navedenog reda i kolone.",
		arguments: [
			{
				name: "br_reda",
				description: "predstavlja broj reda koji treba koristiti u referenci na ćeliju: Broj_reda = 1 za red 1"
			},
			{
				name: "br_kolone",
				description: "predstavlja broj kolone koju treba koristiti u referenci na ćeliju. Na primer, broj_kolone = 4 za kolonu D"
			},
			{
				name: "aps_br",
				description: "navodi tip reference: apsolutna = 1; apsolutni red/relativna kolona = 2; relativni red/apsolutna kolona = 3; relativna = 4"
			},
			{
				name: "a1",
				description: "predstavlja logičku vrednost koja navodi stil reference: A1 stil = 1 ili TRUE; R1C1 stil = 0 ili FALSE"
			},
			{
				name: "tekst_lista",
				description: "predstavlja tekst koji navodi ime radnog lista koji bi trebalo da se upotrebi kao spoljna referenca"
			}
		]
	},
	{
		name: "AND",
		description: "Kontroliše da li su svi argumenti TRUE i daje TRUE ako su svi argumenti TRUE.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logičko1",
				description: "predstavlja 1 do 255 uslova koje želite da proverite i koji mogu biti TRUE ili FALSE, a mogu biti logičke vrednosti, nizovi ili reference"
			},
			{
				name: "logičko2",
				description: "predstavlja 1 do 255 uslova koje želite da proverite i koji mogu biti TRUE ili FALSE, a mogu biti logičke vrednosti, nizovi ili reference"
			}
		]
	},
	{
		name: "ARABIC",
		description: "Konvertuje rimski broj u arapski.",
		arguments: [
			{
				name: "text",
				description: "je rimski broj koji želite da konvertujete"
			}
		]
	},
	{
		name: "AREAS",
		description: "Daje broj oblasti u referenci. Oblast predstavlja celovit opseg ćelija ili samo jednu ćeliju.",
		arguments: [
			{
				name: "referenca",
				description: "predstavlja referencu na jednu ćeliju ili opseg ćelija i može da se odnosi na više oblasti"
			}
		]
	},
	{
		name: "ASIN",
		description: "Daje arkus sinus broja, u radijanima u opsegu od -Pi/2 do Pi/2.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja sinus ugla koji želite i njegova vrednost mora biti od -1 do 1"
			}
		]
	},
	{
		name: "ASINH",
		description: "Daje inverzni sinus hiperbolički broja.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja bilo koji realan broj jednak ili veći od 1"
			}
		]
	},
	{
		name: "ATAN",
		description: "Daje arkus tangens broja izraženog u radijanima, u opsegu -Pi/2 u Pi/2.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja tangens ugla koji želite"
			}
		]
	},
	{
		name: "ATAN2",
		description: "Daje arkus tangens navedenih x- i y- koordinata, u radijanima od -Pi do Pi, isključujući -Pi.",
		arguments: [
			{
				name: "x_br",
				description: "predstavlja x-koordinatu tačke"
			},
			{
				name: "y_br",
				description: "predstavlja y-koordinatu tačke"
			}
		]
	},
	{
		name: "ATANH",
		description: "Daje inverzni tangens hiperbolički broja.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja bilo koji realan broj između -1 i 1, osim -1 i 1"
			}
		]
	},
	{
		name: "AVEDEV",
		description: "Daje srednju vrednost apsolutnog odstupanja tačaka sa podacima od njihove srednje vrednosti. Argumenti mogu biti brojevi ili imena, nizovi ili reference koje sadrže brojeve.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "predstavlja 1 do 255 argumenata za koje želite srednju vrednost apsolutnog odstupanja"
			},
			{
				name: "broj2",
				description: "predstavlja 1 do 255 argumenata za koje želite srednju vrednost apsolutnog odstupanja"
			}
		]
	},
	{
		name: "AVERAGE",
		description: "Daje prosek (aritmetičku sredinu) svojih argumenata koji mogu biti: brojevi, imena, nizovi ili reference koje sadrže brojeve.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "predstavlja 1 do 255 numeričkih argumenata čiju prosečnu vrednost želite da izračunate"
			},
			{
				name: "broj2",
				description: "predstavlja 1 do 255 numeričkih argumenata čiju prosečnu vrednost želite da izračunate"
			}
		]
	},
	{
		name: "AVERAGEA",
		description: "Daje prosek (aritmetičku sredinu) zadatih argumenata, smatrajući tekstualne i FALSE vrednosti u argumentima nulom, a TRUE jedinicom. Argumenti mogu biti brojevi, imena, nizovi ili reference.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrednost1",
				description: "predstavlja 1 do 255 argumenata čiju prosečnu vrednost želite"
			},
			{
				name: "vrednost2",
				description: "predstavlja 1 do 255 argumenata čiju prosečnu vrednost želite"
			}
		]
	},
	{
		name: "AVERAGEIF",
		description: "Pronalazi srednju vrednost (aritmetička sredina) za ćelije koje navodi dati skup uslova ili kriterijuma.",
		arguments: [
			{
				name: "opseg",
				description: "je opseg ćelija koje bi trebalo proceniti"
			},
			{
				name: "kriterijumi",
				description: "je uslov u obliku broja, izraza ili teksta koji definiše koje će se ćelije koristiti za pronalaženje srednje vrednosti"
			},
			{
				name: "prosečni_opseg",
				description: "su stvarne ćelije koje bi trebalo koristiti za pronalaženje srednje vrednosti. Ako se izostavi, koriste se ćelije iz opsega."
			}
		]
	},
	{
		name: "AVERAGEIFS",
		description: "Pronalazi srednju vrednost (aritmetička sredina) za ćelije koje navodi dati skup uslova ili kriterijuma.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "prosečni_opseg",
				description: "su stvarne ćelije koje bi trebalo koristiti za pronalaženje srednje vrednosti."
			},
			{
				name: "opseg_kriterijuma",
				description: "je opseg ćelija koje želite da procenite po određenom kriterijumu"
			},
			{
				name: "kriterijumi",
				description: "je uslov u obliku broja, izraza ili teksta koji definiše koje će se ćelije koristiti za pronalaženje srednje vrednosti"
			}
		]
	},
	{
		name: "BAHTTEXT",
		description: "Pretvara broj u tekst (baht).",
		arguments: [
			{
				name: "broj",
				description: "predstavlja broj koji želite da pretvorite"
			}
		]
	},
	{
		name: "BASE",
		description: "Konvertuje broj u tekstualnu reprezentaciju sa datom bazom (osnovom).",
		arguments: [
			{
				name: "number",
				description: "je broj koji želite da konvertujete"
			},
			{
				name: "radix",
				description: "je baza osnove u koju želite da konvertujete broj"
			},
			{
				name: "min_length",
				description: "je minimalna dužina vraćene niske.  Ako se izostave, početne nule se ne dodaju"
			}
		]
	},
	{
		name: "BESSELI",
		description: "Daje izmenjenu Beselovu funkciju In(x).",
		arguments: [
			{
				name: "x",
				description: "je vrednost na kojoj se procenjuje funkcija"
			},
			{
				name: "n",
				description: "je redosled Beselove funkcije"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "Daje Beselovu funkciju Jn(x).",
		arguments: [
			{
				name: "x",
				description: "je vrednost na kojoj se procenjuje funkcija"
			},
			{
				name: "n",
				description: "je redosled Beselove funkcije"
			}
		]
	},
	{
		name: "BESSELK",
		description: "Daje izmenjenu Beselovu funkciju Kn(x).",
		arguments: [
			{
				name: "x",
				description: "je vrednost na kojoj se procenjuje funkcija"
			},
			{
				name: "n",
				description: "je redosled funkcije"
			}
		]
	},
	{
		name: "BESSELY",
		description: "Daje Beselovu funkciju Yn(x).",
		arguments: [
			{
				name: "x",
				description: "je vrednost na kojoj se procenjuje funkcija"
			},
			{
				name: "n",
				description: "je redosled Beselove funkcije"
			}
		]
	},
	{
		name: "BETA.DIST",
		description: "Daje funkciju beta raspodele verovatnoće.",
		arguments: [
			{
				name: "x",
				description: "predstavlja vrednost između A i B na kojoj se procenjuje funkcija"
			},
			{
				name: "alfa",
				description: "predstavlja parametar raspodele i mora biti veći od 0"
			},
			{
				name: "beta",
				description: "predstavlja parametar raspodele i mora biti veći od 0"
			},
			{
				name: "kumulativno",
				description: "predstavlja logičku vrednost: za funkciju kumulativne raspodele koristite TRUE; za funkciju gustine verovatnoće koristite FALSE"
			},
			{
				name: "A",
				description: "predstavlja opcionalnu donju granicu intervala za x. Ako je izostavljen, A = 0"
			},
			{
				name: "B",
				description: "predstavlja opcionalnu gornju granicu intervala za x. Ako je izostavljen, B = 1"
			}
		]
	},
	{
		name: "BETA.INV",
		description: "Daje inverznu kumulativnu beta funkciju gustine verovatnoće(BETA.DIST).",
		arguments: [
			{
				name: "verovatnoća",
				description: "predstavlja verovatnoću pridruženu beta raspodeli"
			},
			{
				name: "alfa",
				description: "predstavlja parametar raspodele i mora biti veći od 0"
			},
			{
				name: "beta",
				description: "predstavlja parametar raspodele i mora biti veći od 0"
			},
			{
				name: "A",
				description: "predstavlja opcionalnu donju granicu intervala za x. Ako je izostavljen, A = 0"
			},
			{
				name: "B",
				description: "predstavlja opcionalnu gornju granicu intervala za x. Ako je izostavljen, B = 1"
			}
		]
	},
	{
		name: "BETADIST",
		description: "Daje kumulativnu funkciju beta raspodele verovatnoće.",
		arguments: [
			{
				name: "x",
				description: "predstavlja vrednost između A i B za koju treba izračunati funkciju"
			},
			{
				name: "alfa",
				description: "predstavlja parametar raspodele i mora biti veći od 0"
			},
			{
				name: "beta",
				description: "predstavlja parametar raspodele i mora biti veći od 0"
			},
			{
				name: "A",
				description: "predstavlja opcionalnu donju granicu intervala za x. Ako je izostavljen, A = 0"
			},
			{
				name: "B",
				description: "predstavlja opcionalnu gornju granicu intervala za x. Ako je izostavljen, B = 1"
			}
		]
	},
	{
		name: "BETAINV",
		description: "Daje inverznu kumulativnu funkciju beta raspodele verovatnoće (BETADIST).",
		arguments: [
			{
				name: "verovatnoća",
				description: "predstavlja verovatnoću pridruženu beta raspodeli"
			},
			{
				name: "alfa",
				description: "predstavlja parametar raspodele i mora biti veći od 0"
			},
			{
				name: "beta",
				description: "predstavlja parametar raspodele i mora biti veći od 0"
			},
			{
				name: "A",
				description: "predstavlja opcionalnu donju granicu intervala za x. Ako je izostavljen, A = 0"
			},
			{
				name: "B",
				description: "predstavlja opcionalnu gornju granicu intervala za x. Ako je izostavljen, B = 1"
			}
		]
	},
	{
		name: "BIN2DEC",
		description: "Konvertuje binarni broj u decimalni.",
		arguments: [
			{
				name: "broj",
				description: "je binarni broj koji želite da konvertujete"
			}
		]
	},
	{
		name: "BIN2HEX",
		description: "Konvertuje binarni broj u heksadecimalni.",
		arguments: [
			{
				name: "broj",
				description: "je binarni broj koji želite da konvertujete"
			},
			{
				name: "mesta",
				description: "je broj znakova koji bi trebalo koristiti"
			}
		]
	},
	{
		name: "BIN2OCT",
		description: "Konvertuje binarni broj u oktalni.",
		arguments: [
			{
				name: "broj",
				description: "je binarni broj koji želite da konvertujete"
			},
			{
				name: "mesta",
				description: "je broj znakova koji bi trebalo koristiti"
			}
		]
	},
	{
		name: "BINOM.DIST",
		description: "Daje binomnu funkciju raspodele.",
		arguments: [
			{
				name: "broj_s",
				description: "predstavlja broj uspešnih eksperimenata"
			},
			{
				name: "probe",
				description: "predstavlja broj nezavisnih eksperimenata"
			},
			{
				name: "verovatnoća_s",
				description: "predstavlja verovatnoću uspeha svakog eksperimenta"
			},
			{
				name: "kumulativno",
				description: "predstavlja logičku vrednost: za kumulativnu funkciju raspodele koristiti TRUE; za funkciju raspodele koristiti FALSE"
			}
		]
	},
	{
		name: "BINOM.DIST.RANGE",
		description: "Daje verovatnoću probnog eksperimenta koristeći binomnu raspodelu.",
		arguments: [
			{
				name: "trials",
				description: "je broj nezavisnih eksperimenata"
			},
			{
				name: "probability_s",
				description: "je verovatnoća uspešnosti za svaki eksperiment"
			},
			{
				name: "number_s",
				description: "je broj uspeha u eksperimentima"
			},
			{
				name: "number_s2",
				description: "ako je obezbeđena, ova funkcija vraća verovatnoću da li će broj uspešnih eksperimenata biti između number_s i number_s2"
			}
		]
	},
	{
		name: "BINOM.INV",
		description: "Vraća najmanju vrednost za koju je kumulativna binomna funkcija raspodele veća ili jednaka vrednosti kriterijuma.",
		arguments: [
			{
				name: "probe",
				description: "predstavlja broj Bernulijevih eksperimenata"
			},
			{
				name: "verovatnoća_s",
				description: "predstavlja verovatnoću uspeha svakog eksperimenta, broj između 0 i 1, uključujući i njih"
			},
			{
				name: "alfa",
				description: "predstavlja vrednost kriterijuma, broj između 0 i 1, uključujući i njih"
			}
		]
	},
	{
		name: "BINOMDIST",
		description: "Daje binomnu funkciju raspodele.",
		arguments: [
			{
				name: "broj_s",
				description: "predstavlja broj uspeha"
			},
			{
				name: "probe",
				description: "predstavlja broj nezavisnih eksperimenata"
			},
			{
				name: "verovatnoća_s",
				description: "predstavlja verovatnoću uspeha svakog eksperimenta"
			},
			{
				name: "kumulativno",
				description: "predstavlja logičku vrednost: za kumulativnu funkciju raspodele koristiti TRUE; za funkciju raspodele koristiti FALSE"
			}
		]
	},
	{
		name: "BITAND",
		description: "Daje „I“ na nivou bita od dva broja.",
		arguments: [
			{
				name: "number1",
				description: "je decimalna reprezentacija binarnog broja koji želite da proverite"
			},
			{
				name: "number2",
				description: "je decimalna reprezentacija binarnog broja koji želite da proverite"
			}
		]
	},
	{
		name: "BITLSHIFT",
		description: "Daje broj premešten ulevo za shift_amount bita.",
		arguments: [
			{
				name: "number",
				description: "je decimalna reprezentacija binarnog broja koji želite da proverite"
			},
			{
				name: "shift_amount",
				description: "je broj bita za koje želite da premestite broj ulevo za"
			}
		]
	},
	{
		name: "BITOR",
		description: "Daje „Ili“ na nivou bita od dva broja.",
		arguments: [
			{
				name: "number1",
				description: "je decimalna reprezentacija binarnog broja koji želite da proverite"
			},
			{
				name: "number2",
				description: "je decimalna reprezentacija binarnog broja koji želite da proverite"
			}
		]
	},
	{
		name: "BITRSHIFT",
		description: "Daje broj premešten udesno za shift_amount bita.",
		arguments: [
			{
				name: "number",
				description: "je decimalna reprezentacija binarnog broja koji želite da proverite"
			},
			{
				name: "shift_amount",
				description: "je broj bita za koje želite da premestite broj udesno za"
			}
		]
	},
	{
		name: "BITXOR",
		description: "Daje „Isključivo ili“ na nivou bita od dva broja.",
		arguments: [
			{
				name: "number1",
				description: "je decimalna reprezentacija binarnog broja koji želite da proverite"
			},
			{
				name: "number2",
				description: "je decimalna reprezentacija binarnog broja koji želite da proverite"
			}
		]
	},
	{
		name: "CEILING",
		description: "Zaokružuje broj naviše, na najbliži umnožak argumenta značaj.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja vrednost koju želite da zaokružite"
			},
			{
				name: "značaj",
				description: "predstavlja umnožak na koji želite da zaokružite broj"
			}
		]
	},
	{
		name: "CEILING.MATH",
		description: "Zaokružuje broj naviše na najbliži ceo broj ili na najbliži umnožak argumenta značaj.",
		arguments: [
			{
				name: "number",
				description: "predstavlja vrednost koju želite da zaokružite"
			},
			{
				name: "significance",
				description: "je umnožak na koji želite da zaokružite"
			},
			{
				name: "mode",
				description: "kada je data i nije nula ova funkcija će zaokružiti dalje od nule"
			}
		]
	},
	{
		name: "CEILING.PRECISE",
		description: "Zaokružuje broj naviše, na najbliži ceo broj ili na najbliži umnožak argumenta značaj.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja vrednost koju želite da zaokružite"
			},
			{
				name: "značaj",
				description: "predstavlja umnožak na koji želite da zaokružite broj"
			}
		]
	},
	{
		name: "CELL",
		description: "Daje informacije o oblikovanju, položaju ili sadržaju prve ćelije reference, prema redosledu za čitanje ovog lista.",
		arguments: [
			{
				name: "tip_informacija",
				description: "predstavlja tekstualnu vrednost koja navodi koju vrstu informacije o ćeliji želite."
			},
			{
				name: "referenca",
				description: "predstavlja ćeliju o kojoj želite informacije"
			}
		]
	},
	{
		name: "CHAR",
		description: "Daje znak određen kodnim brojem skupa znakova vašeg računara.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja broj od 1 do 255 koji određuje koji znak želite"
			}
		]
	},
	{
		name: "CHIDIST",
		description: "Daje desnu verovatnoću hi-kvadrata raspodele.",
		arguments: [
			{
				name: "x",
				description: "predstavlja nenegativan broj za koji se računa vrednost raspodele"
			},
			{
				name: "step_slobode",
				description: "predstavlja broj stepeni slobode između 1 i 10^10, isključujući 10^10"
			}
		]
	},
	{
		name: "CHIINV",
		description: "Daje inverznu desnu verovatnoću hi-kvadrata raspodele.",
		arguments: [
			{
				name: "verovatnoća",
				description: "predstavlja verovatnoću povezanu sa hi-kvadratom raspodele, vrednost između 0 i 1, uključujući i njih"
			},
			{
				name: "step_slobode",
				description: "predstavlja broj stepeni slobode, neki broj između 1 i 10^10, isključujući 10^10"
			}
		]
	},
	{
		name: "CHISQ.DIST",
		description: "Daje levu verovatnoću za hi-kvadrat raspodele.",
		arguments: [
			{
				name: "x",
				description: "predstavlja nenegativan broj za koji se računa vrednost raspodele"
			},
			{
				name: "step_slobode",
				description: "predstavlja broj stepeni slobode između 1 i 10^10, isključujući 10^10"
			},
			{
				name: "kumulativno",
				description: "predstavlja logičku vrednost koju funkcija treba da dâ: funkcija kumulativne raspodele = TRUE; funkcija gustine verovatnoće = FALSE"
			}
		]
	},
	{
		name: "CHISQ.DIST.RT",
		description: "Daje desnu verovatnoću hi-kvadrata raspodele.",
		arguments: [
			{
				name: "x",
				description: "predstavlja nenegativan broj za koji se računa vrednost raspodele"
			},
			{
				name: "step_slobode",
				description: "predstavlja broj stepeni slobode između 1 i 10^10, isključujući 10^10"
			}
		]
	},
	{
		name: "CHISQ.INV",
		description: "Daje inverznu levu verovatnoću za hi-kvadrat raspodele.",
		arguments: [
			{
				name: "verovatnoća",
				description: "predstavlja verovatnoću pridruženu hi-kvadratu raspodele, vrednost od 0 do 1, uključujući te brojeve"
			},
			{
				name: "step_slobode",
				description: "predstavlja broj stepeni slobode između 1 i 10^10, isključujući 10^10"
			}
		]
	},
	{
		name: "CHISQ.INV.RT",
		description: "Daje inverznu desnu verovatnoću za hi-kvadrat raspodelu.",
		arguments: [
			{
				name: "verovatnoća",
				description: "predstavlja verovatnoću pridruženu hi-kvadratu raspodele, vrednost od 0 do 1, uključujući te brojeve"
			},
			{
				name: "step_slobode",
				description: "predstavlja broj stepeni slobode između 1 i 10^10, isključujući 10^10"
			}
		]
	},
	{
		name: "CHISQ.TEST",
		description: "Daje test nezavisnosti: vrednost iz hi-kvadrata raspodele za statistiku i odgovarajuće stepene slobode.",
		arguments: [
			{
				name: "stvarni_opseg",
				description: "predstavlja opseg podataka koji sadrži posmatranja koja treba testirati prema očekivanim vrednostima"
			},
			{
				name: "očekivani_opseg",
				description: "predstavlja opseg podataka koji sadrži odnos ukupnih vrednosti redova i kolona i sveukupne vrednosti"
			}
		]
	},
	{
		name: "CHITEST",
		description: "Daje test nezavisnosti: vrednost iz hi-kvadrata raspodele za statistiku i odgovarajuće stepene slobode.",
		arguments: [
			{
				name: "stvarni_opseg",
				description: "predstavlja opseg podataka koji sadrži posmatranja koja treba testirati prema očekivanim vrednostima"
			},
			{
				name: "očekivani_opseg",
				description: "predstavlja opseg podataka koji sadrži odnos ukupnih vrednosti redova i kolona i sveukupne vrednosti"
			}
		]
	},
	{
		name: "CHOOSE",
		description: "Sa liste vrednosti odabira vrednost ili radnju koju će izvršiti, na osnovu indeksnog broja.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "indeksni_broj",
				description: "navodi koji argument vrednosti se bira. Indeksni_broj mora biti broj između 1 i 254, formula ili referenca na broj između 1 i 254"
			},
			{
				name: "vrednost1",
				description: "predstavlja 1 do 254 brojeva, referenci na ćelije, definisanih imena, formula, funkcija ili tekstualnih argumenata među kojima funkcija CHOOSE vrši izbor"
			},
			{
				name: "vrednost2",
				description: "predstavlja 1 do 254 brojeva, referenci na ćelije, definisanih imena, formula, funkcija ili tekstualnih argumenata među kojima funkcija CHOOSE vrši izbor"
			}
		]
	},
	{
		name: "CLEAN",
		description: "Uklanja iz teksta sve znakove koji ne mogu da budu odštampani.",
		arguments: [
			{
				name: "tekst",
				description: "predstavlja bilo koju informaciju o radnom listu iz koje želite da uklonite znakove koji ne mogu da budu odštampani"
			}
		]
	},
	{
		name: "CODE",
		description: "Daje numerički kôd prvog znaka tekstualne niske u skupu znakova koji koristi vaš računar.",
		arguments: [
			{
				name: "tekst",
				description: "predstavlja tekst za čiji prvi znak želite kôd"
			}
		]
	},
	{
		name: "COLUMN",
		description: "Daje broj kolona reference.",
		arguments: [
			{
				name: "referenca",
				description: "predstavlja ćeliju ili opseg susednih ćelija za koje želite da ustanovite broj kolona; daje ćeliju koja sadrži funkciju COLUMN ako je ćelija izostavljena"
			}
		]
	},
	{
		name: "COLUMNS",
		description: "Daje broj kolona u tabeli ili u referenci.",
		arguments: [
			{
				name: "niz",
				description: "predstavlja tabelu, formulu tabele ili referencu na opseg ćelija u kome želite da odredite broj kolona"
			}
		]
	},
	{
		name: "COMBIN",
		description: "Daje broj kombinacija za dati broj stavki.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja ukupan broj stavki"
			},
			{
				name: "odabrani_broj",
				description: "predstavlja broj stavki u svakoj kombinaciji"
			}
		]
	},
	{
		name: "COMBINA",
		description: "Daje broj kombinacija sa ponavljanjima za dati broj stavki.",
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
		description: "Konvertuje imaginarne i realne delove u kompleksni broj.",
		arguments: [
			{
				name: "real_br",
				description: "je realni deo kompleksnog broja"
			},
			{
				name: "i_br",
				description: "je imaginarni deo kompleksnog broja"
			},
			{
				name: "sufiks",
				description: "je parametar imaginarnog dela kompleksnog broja"
			}
		]
	},
	{
		name: "CONCATENATE",
		description: "Udružuje nekoliko tekstualnih niski u jednu.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tekst1",
				description: "predstavlja 1 do 255 tekstualnih niski koje bi trebalo udružiti u jednu i može biti dat kao tekstualne niske, brojevi ili reference na pojedinačne ćelije"
			},
			{
				name: "tekst2",
				description: "predstavlja 1 do 255 tekstualnih niski koje bi trebalo udružiti u jednu i može biti dat kao tekstualne niske, brojevi ili reference na pojedinačne ćelije"
			}
		]
	},
	{
		name: "CONFIDENCE",
		description: "Daje interval pouzdanosti za srednju vrednost populacije, uz normalnu raspodelu.",
		arguments: [
			{
				name: "alfa",
				description: "predstavlja nivo važnosti koji se koristi za izračunavanje nivoa pouzdanosti, broj veći od 0 i manji od 1"
			},
			{
				name: "standardna_dev",
				description: "predstavlja standardnu devijaciju populacije za opseg podataka i smatra se poznatim. Standardna_dev mora biti veća od 0"
			},
			{
				name: "veličina",
				description: "predstavlja veličinu uzorka"
			}
		]
	},
	{
		name: "CONFIDENCE.NORM",
		description: "Daje interval pouzdanosti za srednju vrednost populacije koristeći normalnu raspodelu.",
		arguments: [
			{
				name: "alfa",
				description: "predstavlja nivo važnosti koji se koristi za izračunavanje nivoa pouzdanosti, zadat kao broj veći od 0 i manji od 1"
			},
			{
				name: "standardna_dev",
				description: "predstavlja standardnu devijaciju populacije za opseg podataka i smatra se poznatim. Standardna_dev mora biti veća od 0"
			},
			{
				name: "veličina",
				description: "predstavlja veličinu uzorka"
			}
		]
	},
	{
		name: "CONFIDENCE.T",
		description: "Daje interval pouzdanosti za srednju vrednost populacije koristeći Studentovu t-raspodelu.",
		arguments: [
			{
				name: "alfa",
				description: "predstavlja nivo važnosti koji se koristi za izračunavanje nivoa pouzdanosti, zadat kao broj veći od 0 i manji od 1"
			},
			{
				name: "standardna_dev",
				description: "predstavlja standardnu devijaciju populacije za opseg podataka i smatra se poznatim. Standardna_devijacija mora biti veća od 0"
			},
			{
				name: "veličina",
				description: "predstavlja veličinu uzorka"
			}
		]
	},
	{
		name: "CONVERT",
		description: "Konvertuje broj iz jednog sistema mernih jedinica u drugi.",
		arguments: [
			{
				name: "broj",
				description: "je vrednost u „iz ovih jedinica“ koju bi trebalo konvertovati"
			},
			{
				name: "od_jedinice",
				description: "je merna jedinica za broj"
			},
			{
				name: "do_jedinice",
				description: "je merna jedinica za rezultat"
			}
		]
	},
	{
		name: "CORREL",
		description: "Daje koeficijent korelacije dva skupa podataka.",
		arguments: [
			{
				name: "niz1",
				description: "predstavlja opseg ćelija sa vrednostima. Vrednosti bi trebalo da budu brojevi, imena, nizovi ili reference koje sadrže brojeve"
			},
			{
				name: "niz2",
				description: "predstavlja drugi opseg ćelija sa vrednostima. Vrednosti bi trebalo da budu brojevi, imena, nizovi ili reference koje sadrže brojeve"
			}
		]
	},
	{
		name: "COS",
		description: "Daje kosinus ugla.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja ugao čija je vrednost data u radijanima i čiji kosinus želite"
			}
		]
	},
	{
		name: "COSH",
		description: "Daje kosinus hiperbolički broja.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja bilo koji realan broj"
			}
		]
	},
	{
		name: "COT",
		description: "Vraća kotangens ugla.",
		arguments: [
			{
				name: "number",
				description: "je ugao u radijanima za koji želite kotangens"
			}
		]
	},
	{
		name: "COTH",
		description: "Vraća hiperbolički kotangens broja.",
		arguments: [
			{
				name: "number",
				description: "je ugao u radijanima za koji želite hiperbolički kotangens"
			}
		]
	},
	{
		name: "COUNT",
		description: "Broji ćelije u opsegu koje sadrže brojeve.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrednost1",
				description: "su argumenti 1 do 255 koji mogu da sadrže ili da se odnose na mnoštvo različitih tipova podataka, ali se broje samo brojevi"
			},
			{
				name: "vrednost2",
				description: "su argumenti 1 do 255 koji mogu da sadrže ili da se odnose na mnoštvo različitih tipova podataka, ali se broje samo brojevi"
			}
		]
	},
	{
		name: "COUNTA",
		description: "Prebrojava ćelije u opsegu koje nisu prazne.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrednost1",
				description: "predstavlja 1 do 255 argumenata koji reprezentuju vrednosti i ćelije koje želite da prebrojite. Vrednosti mogu biti bilo koji tip podataka"
			},
			{
				name: "vrednost2",
				description: "predstavlja 1 do 255 argumenata koji reprezentuju vrednosti i ćelije koje želite da prebrojite. Vrednosti mogu biti bilo koji tip podataka"
			}
		]
	},
	{
		name: "COUNTBLANK",
		description: "Prebrojava prazne ćelije u navedenom opsegu ćelija.",
		arguments: [
			{
				name: "opseg",
				description: "predstavlja opseg u kojem želite da izbrojite prazne ćelije"
			}
		]
	},
	{
		name: "COUNTIF",
		description: "Prebrojava ćelije u opsegu koje zadovoljavaju dati uslov.",
		arguments: [
			{
				name: "opseg",
				description: "predstavlja opseg ćelija u kojem želite da prebrojite ćelije koje nisu prazne"
			},
			{
				name: "kriterijumi",
				description: "predstavlja uslov u obliku broja, izraza ili teksta koji navodi koje će se ćelije brojati"
			}
		]
	},
	{
		name: "COUNTIFS",
		description: "Broji ćelije koje navodi dati skup uslova ili kriterijuma.",
		arguments: [
			{
				name: "opseg_kriterijuma",
				description: "je opseg ćelija koje želite da budu procenjene po određenom uslovu"
			},
			{
				name: "kriterijumi",
				description: "je uslov u obliku broja, izraza ili teksta koji definiše koje će se ćelije brojati"
			}
		]
	},
	{
		name: "COUPDAYBS",
		description: "Daje broj dana od početka perioda za kupon do datuma obračunavanja.",
		arguments: [
			{
				name: "poravnanje",
				description: "je datum obračunavanja pokrića, izražen kao serijski broj datuma"
			},
			{
				name: "zrelost",
				description: "je datum dospevanja pokrića, izražen kao serijski broj datuma"
			},
			{
				name: "učestalost",
				description: "je godišnji broj uplata za kupone"
			},
			{
				name: "osnova",
				description: "je tip osnovice za brojanje dana koji bi trebalo koristiti"
			}
		]
	},
	{
		name: "COUPNCD",
		description: "Daje sledeći datum za kupon nakon datuma za obračun.",
		arguments: [
			{
				name: "poravnanje",
				description: "je datum obračunavanja pokrića, izražen kao serijski broj datuma"
			},
			{
				name: "zrelost",
				description: "je datum dospevanja pokrića, izražen kao serijski broj datuma"
			},
			{
				name: "učestalost",
				description: "je godišnji broj uplata za kupone"
			},
			{
				name: "osnova",
				description: "je tip osnovice za brojanje dana koji bi trebalo koristiti"
			}
		]
	},
	{
		name: "COUPNUM",
		description: "Daje broj kupona za koje je moguća isplata između datuma obračunavanja i datuma dospeća.",
		arguments: [
			{
				name: "poravnanje",
				description: "je datum obračunavanja pokrića, izražen kao serijski broj datuma"
			},
			{
				name: "zrelost",
				description: "je datum dospevanja pokrića, izražen kao serijski broj datuma"
			},
			{
				name: "učestalost",
				description: "je godišnji broj uplata za kupone"
			},
			{
				name: "osnova",
				description: "je tip osnovice za brojanje dana koji bi trebalo koristiti"
			}
		]
	},
	{
		name: "COUPPCD",
		description: "Daje datum prethodnog kupona pre datuma obračunavanja.",
		arguments: [
			{
				name: "poravnanje",
				description: "je datum obračunavanja pokrića, izražen kao serijski broj datuma"
			},
			{
				name: "zrelost",
				description: "je datum dospevanja pokrića, izražen kao serijski broj datuma"
			},
			{
				name: "učestalost",
				description: "je godišnji broj uplata za kupone"
			},
			{
				name: "osnova",
				description: "je tip osnovice za brojanje dana koji bi trebalo koristiti"
			}
		]
	},
	{
		name: "COVAR",
		description: "Daje kovarijansu, srednju vrednost proizvoda odstupanja za svaki par tačaka podataka iz dva skupa podataka.",
		arguments: [
			{
				name: "niz1",
				description: "predstavlja prvi opseg ćelija celih brojeva i mora biti dat kao brojevi, nizovi ili reference koje sadrže brojeve"
			},
			{
				name: "niz2",
				description: "predstavlja drugi opseg ćelija celih brojeva i mora biti dat kao brojevi, nizovi ili reference koje sadrže brojeve"
			}
		]
	},
	{
		name: "COVARIANCE.P",
		description: "Daje kovarijansu populacije, srednju vrednost proizvoda odstupanja za svaki par mesta podataka iz dva skupa podataka.",
		arguments: [
			{
				name: "niz1",
				description: "predstavlja prvi opseg ćelija celih brojeva i mora biti dat kao brojevi, nizovi ili reference koje sadrže brojeve"
			},
			{
				name: "niz2",
				description: "predstavlja drugi opseg ćelija celih brojeva i mora biti dat kao brojevi, nizovi ili reference koje sadrže brojeve"
			}
		]
	},
	{
		name: "COVARIANCE.S",
		description: "Daje kovarijansu uzorka, srednju vrednost proizvoda odstupanja za svaki par mesta podataka iz dva skupa podataka.",
		arguments: [
			{
				name: "niz1",
				description: "predstavlja prvi opseg ćelija celih brojeva i mora biti dat kao brojevi, nizovi ili reference koje sadrže brojeve"
			},
			{
				name: "niz2",
				description: "predstavlja drugi opseg ćelija celih brojeva i mora biti dat kao brojevi, nizovi ili reference koje sadrže brojeve"
			}
		]
	},
	{
		name: "CRITBINOM",
		description: "Daje najmanju vrednost za koju je kumulativna binomna funkcija raspodele veća ili jednaka vrednosti kriterijuma.",
		arguments: [
			{
				name: "probe",
				description: "predstavlja broj Bernulijevih eksperimenata"
			},
			{
				name: "verovatnoća_s",
				description: "predstavlja verovatnoću uspeha svakog eksperimenta, broj između 0 i 1, uključujući i njih"
			},
			{
				name: "alfa",
				description: "predstavlja vrednost kriterijuma, broj između 0 i 1, uključujući i njih"
			}
		]
	},
	{
		name: "CSC",
		description: "Vraća kosekans ugla.",
		arguments: [
			{
				name: "number",
				description: "je ugao u radijanima za koji želite kosekans"
			}
		]
	},
	{
		name: "CSCH",
		description: "Vraća hiperbolički kosekans ugla.",
		arguments: [
			{
				name: "number",
				description: "je ugao u radijanima za koji želite hiperbolički kosekans"
			}
		]
	},
	{
		name: "CUMIPMT",
		description: "daje ukupnu kamatu isplaćenu između dva perioda.",
		arguments: [
			{
				name: "stopa",
				description: "je kamatna stopa"
			},
			{
				name: "br_per",
				description: "je ukupan broj perioda za isplatu"
			},
			{
				name: "sad_vr",
				description: "je sadašnja vrednost"
			},
			{
				name: "početni_period",
				description: "je prvi period u izračunavanju"
			},
			{
				name: "završni_period",
				description: "je poslednji period za izračunavanje"
			},
			{
				name: "tip",
				description: "je tempiranje isplate"
			}
		]
	},
	{
		name: "CUMPRINC",
		description: "Daje ukupnu isplaćenu glavnicu za pozajmicu između dva perioda.",
		arguments: [
			{
				name: "stopa",
				description: "je kamatna stopa"
			},
			{
				name: "br_per",
				description: "je ukupan broj perioda za isplatu"
			},
			{
				name: "sad_vr",
				description: "je sadašnja vrednost"
			},
			{
				name: "početni_period",
				description: "je prvi period u izračunavanju"
			},
			{
				name: "završni_period",
				description: "je poslednji period za izračunavanje"
			},
			{
				name: "tip",
				description: "je tempiranje isplate"
			}
		]
	},
	{
		name: "DATE",
		description: "Vraća broj koji predstavlja datum u Spreadsheet kodu za datum-vreme.",
		arguments: [
			{
				name: "godina",
				description: "predstavlja broj od 1900 do 9999 u programu Spreadsheet za Windows ili od 1904 do 9999 u programu Spreadsheet za Macintosh"
			},
			{
				name: "mesec",
				description: "predstavlja broj od 1 do 12 koji predstavlja mesec u godini"
			},
			{
				name: "dan",
				description: "predstavlja broj od 1 do 31 koji predstavlja dan u mesecu"
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
		description: "Konvertuje datum u tekstualnom obliku u broj koji predstavlja datum u Spreadsheet kodu za datum-vreme.",
		arguments: [
			{
				name: "tekst_datuma",
				description: "predstavlja tekst koji predstavlja datum u Spreadsheet formatu za datum, između 1.1.1900 (Windows) ili 1.1.1904 (Macintosh) i 12.31.9999"
			}
		]
	},
	{
		name: "DAVERAGE",
		description: "Daje prosek vrednosti u koloni na listi ili u bazi podataka koji odgovaraju uslovima koje ste postavili.",
		arguments: [
			{
				name: "baza_podataka",
				description: "predstavlja opseg ćelija koji čini listu ili bazu podataka. Baza podataka je lista srodnih podataka"
			},
			{
				name: "polje",
				description: "predstavlja ili oznaku kolone između znakova navoda ili broj koji predstavlja položaj kolone na listi"
			},
			{
				name: "kriterijumi",
				description: "predstavlja opseg ćelija koji sadrži uslove koje ste postavili. Opseg uključuje oznaku kolone i jednu ćeliju za uslov, ispod nje"
			}
		]
	},
	{
		name: "DAY",
		description: "Vraća dan u mesecu, broj od 1 do 31.",
		arguments: [
			{
				name: "serijski_broj",
				description: "predstavlja broj u Spreadsheet kodu za datum-vreme"
			}
		]
	},
	{
		name: "DAYS",
		description: "Daje broj dana između dva datuma.",
		arguments: [
			{
				name: "end_date",
				description: "start_date i end_date su dva datuma između kojih želite da znate broj dana"
			},
			{
				name: "start_date",
				description: "start_date i end_date su dva datuma između kojih želite da znate broj dana"
			}
		]
	},
	{
		name: "DAYS360",
		description: "Daje broj dana između dva datuma, na osnovu godine od 360 dana (dvanaest meseci od po 30 dana).",
		arguments: [
			{
				name: "datum_početka",
				description: "datum_početka i datum_završetka su dva datuma za koje želite da izračunate broj dana koji ih deli"
			},
			{
				name: "datum_završetka",
				description: "datum_početka i datum_završetka su dva datuma za koje želite da izračunate broj dana koji ih deli"
			},
			{
				name: "metod",
				description: "predstavlja logičku vrednost koja navodi metod računanja: SAD (NASD) = FALSE ili je izostavljen; evropski = TRUE."
			}
		]
	},
	{
		name: "DB",
		description: "Daje amortizaciju sredstva za navedeni period koristeći geometrijsko-nazadujući metod.",
		arguments: [
			{
				name: "cena",
				description: "predstavlja početnu cenu sredstva"
			},
			{
				name: "rashod",
				description: "predstavlja amortizovanu vrednost sredstva po isteku perioda njegove upotrebe"
			},
			{
				name: "vek",
				description: "predstavlja broj perioda tokom kojih se vrši amortizacija sredstva (ponekad se naziva korisni vek sredstva)"
			},
			{
				name: "period",
				description: "predstavlja period za koji želite da izračunate amortizaciju. Ovaj period mora da koristi iste jedinice mere kao i korisni vek"
			},
			{
				name: "mesec",
				description: "Ako je argument mesec izostavljen, koristi se vrednost 12"
			}
		]
	},
	{
		name: "DCOUNT",
		description: "Broji ćelije koje sadrže brojeve u polju (koloni) zapisa u bazi podataka koje odgovaraju uslovima koje ste postavili.",
		arguments: [
			{
				name: "baza_podataka",
				description: "predstavlja opseg ćelija koji čini listu ili bazu podataka. Baza podataka je lista srodnih podataka"
			},
			{
				name: "polje",
				description: "predstavlja ili oznaku kolone između znaka navoda ili broj koji predstavlja položaj kolone na listi"
			},
			{
				name: "kriterijumi",
				description: "predstavlja opseg ćelija koji sadrži uslove koje ste postavili. Opseg uključuje oznaku kolone i jednu ćeliju za uslov, ispod nje"
			}
		]
	},
	{
		name: "DCOUNTA",
		description: "Prebrojava ćelije koje nisu prazne u polju (koloni) zapisa u bazi podataka koje odgovaraju uslovima koje navedete.",
		arguments: [
			{
				name: "baza_podataka",
				description: "predstavlja opseg ćelija koji čini listu ili bazu podataka. Baza podataka je lista srodnih podataka"
			},
			{
				name: "polje",
				description: "predstavlja oznaku kolone pod znacima navoda ili broj koji predstavlja položaj kolone na listi"
			},
			{
				name: "kriterijumi",
				description: "predstavlja opseg ćelija koje sadrže uslov koji ste postavili. Opseg uključuje oznaku kolone i jednu ćeliju za uslov, ispod nje"
			}
		]
	},
	{
		name: "DDB",
		description: "Daje amortizaciju sredstva za navedeni period koristeći metod dvostruke stope linearne amortizacije na opadajuću osnovu ili neki drugi metod koji navedete.",
		arguments: [
			{
				name: "cena",
				description: "predstavlja početnu cenu sredstva"
			},
			{
				name: "rashod",
				description: "predstavlja amortizovanu vrednost sredstva po isteku perioda njegove upotrebe"
			},
			{
				name: "vek",
				description: "predstavlja broj perioda tokom kojih se vrši amortizacija sredstva (ponekad se naziva korisni vek sredstva)"
			},
			{
				name: "period",
				description: "predstavlja period za koji želite da izračunate amortizaciju. Ovaj period mora da koristi iste jedinice mere kao i argument vek"
			},
			{
				name: "faktor",
				description: "predstavlja stopu opadanja preostale vrednosti. Ako je faktor izostavljen, pretpostavlja se da vrednost iznosi 2 (metod dvostruke stope linearne amortizacije na opadajuću osnovu)"
			}
		]
	},
	{
		name: "DEC2BIN",
		description: "Konvertuje decimalni broj u binarni.",
		arguments: [
			{
				name: "broj",
				description: "je decimalni ceo broj koji želite da konvertujete"
			},
			{
				name: "mesta",
				description: "je broj znakova koji bi trebalo koristiti"
			}
		]
	},
	{
		name: "DEC2HEX",
		description: "Konvertuje decimalni broj u heksadecimalni.",
		arguments: [
			{
				name: "broj",
				description: "je decimalni ceo broj koji želite da konvertujete"
			},
			{
				name: "mesta",
				description: "je broj znakova koji bi trebalo koristiti"
			}
		]
	},
	{
		name: "DEC2OCT",
		description: "Konvertuje decimalni broj u oktalni.",
		arguments: [
			{
				name: "broj",
				description: "je decimalni ceo broj koji želite da konvertujete"
			},
			{
				name: "mesta",
				description: "je broj znakova koji bi trebalo koristiti"
			}
		]
	},
	{
		name: "DECIMAL",
		description: "Konvertuje tekstualnu reprezentaciju broja u određenoj osnovi u decimalni broj.",
		arguments: [
			{
				name: "number",
				description: "je broj koji želite da konvertujete"
			},
			{
				name: "radix",
				description: "je baza osnove broja koji konvertujete"
			}
		]
	},
	{
		name: "DEGREES",
		description: "Pretvara radijane u stepene.",
		arguments: [
			{
				name: "ugao",
				description: "predstavlja ugao u radijanima koji želite da pretvorite"
			}
		]
	},
	{
		name: "DELTA",
		description: "Proverava da li su dva broja jednaka.",
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
		description: "Daje zbir kvadrata odstupanja tačaka podataka od srednje vrednosti uzorka.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "predstavlja 1 do 255 argumenata, niz ili referencu na niz, koje funkcija DEVSQ koristi kao ulazne podatke"
			},
			{
				name: "broj2",
				description: "predstavlja 1 do 255 argumenata, niz ili referencu na niz, koje funkcija DEVSQ koristi kao ulazne podatke"
			}
		]
	},
	{
		name: "DGET",
		description: "Izdvaja iz baze podataka pojedinačni zapis koji ispunjava uslove koje ste postavili.",
		arguments: [
			{
				name: "baza_podataka",
				description: "predstavlja opseg ćelija koji čini listu ili bazu podataka. Baza podataka je lista srodnih podataka"
			},
			{
				name: "polje",
				description: "predstavlja ili oznaku kolone između znaka navoda ili broj koji reprezentuje položaj kolone na listi"
			},
			{
				name: "kriterijumi",
				description: "predstavlja opseg ćelija koje sadrže uslov koji ste postavili. Opseg uključuje oznaku kolone i jednu ćeliju za uslov, ispod nje"
			}
		]
	},
	{
		name: "DISC",
		description: "Daje stopu popusta za pokriće.",
		arguments: [
			{
				name: "poravnanje",
				description: "je datum obračunavanja pokrića, izražen kao serijski broj datuma"
			},
			{
				name: "zrelost",
				description: "je datum dospevanja pokrića, izražen kao serijski broj datuma"
			},
			{
				name: "pr",
				description: "je cena pokrića na 100 Din nominalne vrednosti"
			},
			{
				name: "povraćaj",
				description: "je otkupna vrednost pokrića na 100 Din nominalne vrednosti"
			},
			{
				name: "osnova",
				description: "je tip osnovice za brojanje dana koji bi trebalo koristiti"
			}
		]
	},
	{
		name: "DMAX",
		description: "Daje najveći broj u polju (koloni) zapisa u bazi podataka koji odgovara uslovima koje ste postavili.",
		arguments: [
			{
				name: "baza_podataka",
				description: "predstavlja opseg ćelija koji čini listu ili bazu podataka. Baza podataka je lista srodnih podataka"
			},
			{
				name: "polje",
				description: "predstavlja ili oznaku kolone između znakova navoda ili broj koji predstavlja položaj kolone na listi"
			},
			{
				name: "kriterijumi",
				description: "predstavlja opseg ćelija koji sadrži uslov koji ste postavili. Opseg uključuje oznaku kolone i jednu ćeliju za uslov, ispod nje"
			}
		]
	},
	{
		name: "DMIN",
		description: "Daje najmanji broj u polju (koloni) zapisa u bazi podataka koji odgovara uslovima koje ste postavili.",
		arguments: [
			{
				name: "baza_podataka",
				description: "predstavlja opseg ćelija koji čini listu ili bazu podataka. Baza podataka je lista srodnih podataka"
			},
			{
				name: "polje",
				description: "predstavlja ili oznaku kolone između znakova navoda ili broj koji predstavlja položaj kolone na listi"
			},
			{
				name: "kriterijumi",
				description: "predstavlja opseg ćelija koji sadrži uslov koji ste postavili. Opseg uključuje oznaku kolone i jednu ćeliju za uslov, ispod nje"
			}
		]
	},
	{
		name: "DOLLAR",
		description: "Pretvara broj u tekst koristeći format za valutu.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja broj, referencu ćelije koja sadrži broj ili formulu koja je izražena brojem"
			},
			{
				name: "decimale",
				description: "predstavlja brojeve koji se nalaze desno od znaka za razdvajanje decimala. Broj se zaokružuje po potrebi; ako je izostavljen, argument decimale iznosi 2"
			}
		]
	},
	{
		name: "DOLLARDE",
		description: "Konvertuje cenu u dolarima, izraženu kao razlomak, u cenu u dolarima, izraženu kao decimalni broj.",
		arguments: [
			{
				name: "dolar_izražen_u_razlomcima",
				description: "je broj izražen kao razlomak"
			},
			{
				name: "razlomak",
				description: "je ceo broj koji bi trebalo koristiti kao imenilac razlomka"
			}
		]
	},
	{
		name: "DOLLARFR",
		description: "Konvertuje cenu u dolarima, izraženu kao decimalni broj u cenu u dolarima, izraženu kao razlomak.",
		arguments: [
			{
				name: "decimalni_dolar",
				description: "je decimalni broj"
			},
			{
				name: "razlomak",
				description: "je ceo broj koji bi trebalo koristiti kao imenilac razlomka"
			}
		]
	},
	{
		name: "DPRODUCT",
		description: "Množi vrednosti u polju (koloni) zapisa u bazi podataka koje odgovaraju uslovima koje navedete.",
		arguments: [
			{
				name: "baza_podataka",
				description: "predstavlja opseg ćelija koji čini listu ili bazu podataka. Baza podataka je lista srodnih podataka"
			},
			{
				name: "polje",
				description: "predstavlja oznaku kolone između navodnika ili broj koji predstavlja položaj kolone na listi"
			},
			{
				name: "kriterijumi",
				description: "predstavlja opseg ćelija koje sadrže uslove koje ste postavili. Opseg uključuje oznaku kolone i jednu ćeliju za uslov, ispod nje"
			}
		]
	},
	{
		name: "DSTDEV",
		description: "Izračunava standardnu devijaciju na osnovu uzorka iz izabranih unosa baze podataka.",
		arguments: [
			{
				name: "baza_podataka",
				description: "predstavlja opseg ćelija koji čini listu ili bazu podataka. Baza podataka je lista srodnih podataka"
			},
			{
				name: "polje",
				description: "predstavlja ili oznaku kolone između znaka navoda ili broj koji predstavlja položaj kolone na listi"
			},
			{
				name: "kriterijumi",
				description: "predstavlja opseg ćelija koji sadrži uslov koji ste postavili. Opseg uključuje oznaku kolone i jednu ćeliju za uslov, ispod nje"
			}
		]
	},
	{
		name: "DSTDEVP",
		description: "Izračunava standardnu devijaciju zasnovanu na ukupnoj populaciji izabranih stavki baze podataka.",
		arguments: [
			{
				name: "baza_podataka",
				description: "predstavlja opseg ćelija koji čini listu ili bazu podataka. Baza podataka je lista srodnih podataka"
			},
			{
				name: "polje",
				description: "predstavlja oznaku kolone pod znacima navoda ili broj koji predstavlja položaj kolone na listi"
			},
			{
				name: "kriterijumi",
				description: "predstavlja opseg ćelija koje sadrže uslove koje ste postavili. Opseg uključuje oznaku kolone i jednu ćeliju za uslov, ispod nje"
			}
		]
	},
	{
		name: "DSUM",
		description: "Sabira brojeve u polju (koloni) zapisa u bazi podataka koji odgovaraju uslovima koje ste postavili.",
		arguments: [
			{
				name: "baza_podataka",
				description: "predstavlja opseg ćelija koji čini listu ili bazu podataka. Baza podataka je lista srodnih podataka"
			},
			{
				name: "polje",
				description: "predstavlja ili oznaku kolone između znaka navoda ili broj koji predstavlja položaj kolone na listi"
			},
			{
				name: "kriterijumi",
				description: "predstavlja opseg ćelija koji sadrži uslove koje ste postavili. Opseg uključuje oznaku kolone i jednu ćeliju za uslov, ispod nje"
			}
		]
	},
	{
		name: "DVAR",
		description: "Procenjuje odstupanje na osnovu uzorka koji čine izabrane stavke iz baze podataka.",
		arguments: [
			{
				name: "baza_podataka",
				description: "predstavlja opseg ćelija koji čini listu ili bazu podataka. Baza podataka je lista srodnih podataka"
			},
			{
				name: "polje",
				description: "predstavlja ili oznaku kolone između znaka navoda ili broj koji predstavlja položaj kolone na listi"
			},
			{
				name: "kriterijumi",
				description: "predstavlja opseg ćelija koji sadrži uslov koji ste postavili. Opseg uključuje oznaku kolone i jednu ćeliju za uslov, ispod nje"
			}
		]
	},
	{
		name: "DVARP",
		description: "Izračunava odstupanje zasnovano na ukupnoj populaciji izabranih stavki baze podataka.",
		arguments: [
			{
				name: "baza_podataka",
				description: "predstavlja opseg ćelija koji čini listu ili bazu podataka. Baza podataka je lista srodnih podataka"
			},
			{
				name: "polje",
				description: "predstavlja ili oznaku kolone pod znacima navoda ili broj koji reprezentuje položaj kolone na listi"
			},
			{
				name: "kriterijumi",
				description: "predstavlja opseg ćelija koje sadrže uslove koje ste postavili. Opseg uključuje oznaku kolone i jednu ćeliju za uslov, ispod nje"
			}
		]
	},
	{
		name: "EDATE",
		description: "Daje serijski broj datuma koji predstavlja broj meseci pre ili posle početnog datuma.",
		arguments: [
			{
				name: "datum_početka",
				description: "je serijski broj datuma koji predstavlja početni datum"
			},
			{
				name: "meseci",
				description: "je broj meseci pre ili posle početnog datuma"
			}
		]
	},
	{
		name: "EFFECT",
		description: "Daje efektivnu godišnju kamatnu stopu.",
		arguments: [
			{
				name: "nominalna_stopa",
				description: "je nominalna kamatna stopa"
			},
			{
				name: "br_godišnje",
				description: "je godišnji broj perioda za spajanje"
			}
		]
	},
	{
		name: "ENCODEURL",
		description: "Vraća nisku šifrovanu URL adresom.",
		arguments: [
			{
				name: "text",
				description: "je niska koja će biti šifrovana URL adresom"
			}
		]
	},
	{
		name: "EOMONTH",
		description: "Daje serijski broj poslednjeg dana u mesecu pre ili posle navedenog broja meseci.",
		arguments: [
			{
				name: "datum_početka",
				description: "je serijski broj datuma koji predstavlja početni datum"
			},
			{
				name: "meseci",
				description: "je broj meseci pre ili posle početnog datuma"
			}
		]
	},
	{
		name: "ERF",
		description: "Daje funkciju greške.",
		arguments: [
			{
				name: "donja_granica",
				description: "je donja granica za ERF integraciju"
			},
			{
				name: "gornja_granica",
				description: "je gornja granica za ERF integraciju"
			}
		]
	},
	{
		name: "ERF.PRECISE",
		description: "Vraća funkciju greške.",
		arguments: [
			{
				name: "X",
				description: "predstavlja donju granicu za ERF.PRECISE integraciju"
			}
		]
	},
	{
		name: "ERFC",
		description: "Daje dopunsku funkciju greške.",
		arguments: [
			{
				name: "x",
				description: "je donja granica za ERF integraciju"
			}
		]
	},
	{
		name: "ERFC.PRECISE",
		description: "Vraća dopunsku funkciju greške.",
		arguments: [
			{
				name: "X",
				description: "predstavlja donju granicu za ERFC.PRECISE integraciju"
			}
		]
	},
	{
		name: "ERROR.TYPE",
		description: "Daje broj koji odgovara vrednosti greške.",
		arguments: [
			{
				name: "vrednost_greške",
				description: "predstavlja vrednost greške čiji identifikacioni broj želite i može biti stvarna vrednost greške ili referenca na ćeliju koja sadrži vrednost greške"
			}
		]
	},
	{
		name: "EVEN",
		description: "Zaokružuje pozitivni broj više i negativni naniže na najbliži ceo paran broj.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja vrednost koju treba zaokružiti"
			}
		]
	},
	{
		name: "EXACT",
		description: "Proverava da li su dve tekstualne niske identične i daje TRUE ili FALSE. EXACT razlikuje velika i mala slova.",
		arguments: [
			{
				name: "tekst1",
				description: "predstavlja prvu tekstualnu nisku"
			},
			{
				name: "tekst2",
				description: "predstavlja drugu tekstualnu nisku"
			}
		]
	},
	{
		name: "EXP",
		description: "Daje e podignuto na dati stepen.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja izložilac kojim se stepenuje baza e. Konstanta e iznosi 2,71828182845904 i predstavlja osnovu prirodnog logaritma"
			}
		]
	},
	{
		name: "EXPON.DIST",
		description: "Daje eksponencijalnu raspodelu.",
		arguments: [
			{
				name: "x",
				description: "predstavlja vrednost funkcije, nenegativan broj"
			},
			{
				name: "lambda",
				description: "predstavlja vrednost parametra, pozitivan broj"
			},
			{
				name: "kumulativno",
				description: "predstavlja logičku vrednost koja određuje šta funkcija treba da dâ: za kumulativnu funkciju raspodele treba da bude TRUE, a za funkciju gustine raspodele treba da bude FALSE"
			}
		]
	},
	{
		name: "EXPONDIST",
		description: "Daje eksponencijalnu raspodelu.",
		arguments: [
			{
				name: "x",
				description: "predstavlja vrednost funkcije, nenegativan broj"
			},
			{
				name: "lambda",
				description: "predstavlja vrednost parametra, pozitivan broj"
			},
			{
				name: "kumulativno",
				description: "predstavlja logičku vrednost koja određuje šta funkcija treba da dâ: za kumulativnu funkciju raspodele treba da bude TRUE, a za funkciju gustine raspodele treba da bude FALSE"
			}
		]
	},
	{
		name: "F.DIST",
		description: "Daje (levu) F-raspodelu verovatnoće (stepen različitosti) za dva skupa podataka.",
		arguments: [
			{
				name: "x",
				description: "predstavlja vrednost za koju se računa funkcija, i to nenegativan broj"
			},
			{
				name: "step_slobode1",
				description: "predstavlja stepen slobode brojioca, neki broj između 1 i 10^10, isključujući 10^10"
			},
			{
				name: "step_slobode2",
				description: "predstavlja stepen slobode imenioca, neki broj između 1 i 10^10, isključujući 10^10"
			},
			{
				name: "kumulativno",
				description: "predstavlja logičku vrednost koju funkcija treba da dâ: funkcija kumulativne raspodele = TRUE; funkcija gustine verovatnoće = FALSE"
			}
		]
	},
	{
		name: "F.DIST.RT",
		description: "Daje (desnu) F-raspodelu verovatnoće (stepen različitosti) za dva skupa podataka.",
		arguments: [
			{
				name: "x",
				description: "predstavlja vrednost za koju se računa funkcija i to nenegativan broj"
			},
			{
				name: "step_slobode1",
				description: "predstavlja stepen slobode brojioca, neki broj između 1 i 10^10, isključujući 10^10"
			},
			{
				name: "step_slobode2",
				description: "predstavlja stepen slobode imenioca, neki broj između 1 i 10^10, isključujući 10^10"
			}
		]
	},
	{
		name: "F.INV",
		description: "Daje inverznu (levu) F-raspodelu verovatnoće: ako je p = F.DIST(x,...), onda je F.INV(p,...) = x.",
		arguments: [
			{
				name: "verovatnoća",
				description: "predstavlja verovatnoću povezanu sa F kumulativnom raspodelom, broj od 0 do 1, uključujući te brojeve"
			},
			{
				name: "step_slobode1",
				description: "predstavlja stepen slobode brojioca, neki broj između 1 i 10^10, isključujući 10^10"
			},
			{
				name: "step_slobode2",
				description: "predstavlja stepen slobode imenioca, neki broj između 1 i 10^10, isključujući 10^10"
			}
		]
	},
	{
		name: "F.INV.RT",
		description: "Daje inverznu (desnu) F-raspodelu verovatnoće: ako je p = F.DIST.RT(x,...), onda je F.INV.RT(p,...) = x.",
		arguments: [
			{
				name: "verovatnoća",
				description: "predstavlja verovatnoću pridruženu F kumulativnom raspodelom, broj od 0 do 1, uključujući te brojeve"
			},
			{
				name: "step_slobode1",
				description: "predstavlja stepen slobode brojioca, neki broj između 1 i 10^10, isključujući 10^10"
			},
			{
				name: "step_slobode2",
				description: "predstavlja stepen slobode imenioca, neki broj između 1 i 10^10, isključujući 10^10"
			}
		]
	},
	{
		name: "F.TEST",
		description: "Daje rezultat F-testa, dvostranu verovatnoću da odstupanja u nizu1 i nizu2 nisu značajno različita.",
		arguments: [
			{
				name: "niz1",
				description: "predstavlja prvi niz ili opseg podataka i može biti dat kao brojevi ili imena, nizovi ili reference koje sadrže brojeve (prazne ćelije se zanemaruju)"
			},
			{
				name: "niz2",
				description: "predstavlja drugi niz ili opseg podataka i može biti dat kao brojevi ili imena, nizovi ili reference koje sadrže brojeve (prazne ćelije se zanemaruju)"
			}
		]
	},
	{
		name: "FACT",
		description: "Daje faktorijel broja, koji iznosi 1*2*3*...* Broj.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja nenegativan broj čiji faktorijel želite"
			}
		]
	},
	{
		name: "FACTDOUBLE",
		description: "Daje dupli faktorijel broja.",
		arguments: [
			{
				name: "broj",
				description: "je vrednost za koju se daje dupli faktorijel"
			}
		]
	},
	{
		name: "FALSE",
		description: "Daje logičku vrednost FALSE.",
		arguments: [
		]
	},
	{
		name: "FDIST",
		description: "Daje (desnu) F-raspodelu verovatnoće (stepen različitosti) za dva skupa podataka.",
		arguments: [
			{
				name: "x",
				description: "predstavlja vrednost za koju se računa funkcija, pozitivan broj"
			},
			{
				name: "step_slobode1",
				description: "predstavlja stepen slobode brojioca, neki broj između 1 i 10^10, isključujući 10^10"
			},
			{
				name: "step_slobode2",
				description: "predstavlja stepen slobode imenioca, neki broj između 1 i 10^10, isključujući 10^10"
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
		description: "Daje početni položaj neke tekstualne niske u okviru druge tekstualne niske. Funkcija FIND razlikuje mala i velika slova.",
		arguments: [
			{
				name: "pronalaženje_teksta",
				description: "predstavlja tekst koji želite da pronađete. Koristi dvostruke navodnike (prazan tekst) za podudaranje prvih znakova u parametru u_okviru_teksta; džoker znaci nisu dozvoljeni"
			},
			{
				name: "u_okviru_teksta",
				description: "predstavlja tekst koji sadrži deo teksta koji želite da pronađete"
			},
			{
				name: "poč_broj",
				description: "navodi znak od kojeg započinje pretraga. Prvi znak u parametru u_okviru_teksta jeste znak broj 1. Ako je izostavljen, poč_broj = 1"
			}
		]
	},
	{
		name: "FINV",
		description: "Daje inverznu (desnu) F-raspodelu verovatnoće: ako je p = FDIST(x,...), onda je FINV(p,...) = x.",
		arguments: [
			{
				name: "verovatnoća",
				description: "predstavlja verovatnoću povezanu sa kumulativnom F raspodelom, broj između 0 i 1, uključujući i njih"
			},
			{
				name: "step_slobode1",
				description: "predstavlja stepen slobode brojioca, neki broj između 1 i 10^10, isključujući 10^10"
			},
			{
				name: "step_slobode2",
				description: "predstavlja stepen slobode imenioca, neki broj između 1 i 10^10, isključujući 10^10"
			}
		]
	},
	{
		name: "FISHER",
		description: "Daje Fišerovu transformaciju.",
		arguments: [
			{
				name: "x",
				description: "predstavlja vrednost za koju želite da izvršite transformaciju, broj između -1 i 1, isključujući -1 i 1"
			}
		]
	},
	{
		name: "FISHERINV",
		description: "Daje inverznu Fišerovu transformaciju: ako je y = FISHER(x), onda je FISHERINV(y) = x.",
		arguments: [
			{
				name: "y",
				description: "predstavlja vrednost za koju želite da izvršite inverznu transformaciju"
			}
		]
	},
	{
		name: "FIXED",
		description: "Zaokružuje broj na određeni broj decimala i daje rezultat u obliku teksta sa zarezima ili bez njih.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja broj koji želite da zaokružite i pretvorite u tekst"
			},
			{
				name: "decimale",
				description: "predstavlja brojeve koji se nalaze desno od znaka za razdvajanje decimala. Ako je izostavljen, argument decimale iznosi 2"
			},
			{
				name: "bez_zareza",
				description: "predstavlja logičku vrednost: ne prikazuje zareze u datom tekstu = TRUE; prikazuje zareze u datom tekstu = FALSE ili izostavljeno"
			}
		]
	},
	{
		name: "FLOOR",
		description: "Zaokružuje broj naniže, na najbliži umnožak argumenta značaj.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja numeričku vrednost koju želite da zaokružite"
			},
			{
				name: "značaj",
				description: "predstavlja umnožak na koji želite da zaokružite broj. Argumenti broj i značaj moraju oba biti pozitivni ili negativni"
			}
		]
	},
	{
		name: "FLOOR.MATH",
		description: "Zaokružuje broj naniže na najbliži ceo broj ili na najbliži umnožak argumenta značaj.",
		arguments: [
			{
				name: "number",
				description: "predstavlja vrednost koju želite da zaokružite"
			},
			{
				name: "significance",
				description: "je umnožak na koji želite da zaokružite"
			},
			{
				name: "mode",
				description: "kada je data i nije nula ova funkcija će zaokružiti bliže nuli"
			}
		]
	},
	{
		name: "FLOOR.PRECISE",
		description: "Zaokružuje broj naniže, na najbliži ceo broj ili na najbliži umnožak argumenta značaj.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja numeričku vrednost koju želite da zaokružite"
			},
			{
				name: "značaj",
				description: "predstavlja umnožak na koji želite da zaokružite broj. "
			}
		]
	},
	{
		name: "FORECAST",
		description: "Izračunava ili na osnovu postojećih vrednosti predviđa buduću vrednost po linearnom trendu.",
		arguments: [
			{
				name: "x",
				description: "predstavlja tačku podataka čiju vrednost želite da predvidite i mora biti numerička vrednost"
			},
			{
				name: "poznati_y",
				description: "predstavlja zavisan niz ili opseg numeričkih podataka"
			},
			{
				name: "poznati_x",
				description: "predstavlja nezavisan niz ili opseg numeričkih podataka. Odstupanje vrednosti poznati_x ne sme biti nula"
			}
		]
	},
	{
		name: "FORMULATEXT",
		description: "Daje formulu kao nisku.",
		arguments: [
			{
				name: "reference",
				description: "je referenca za formulu"
			}
		]
	},
	{
		name: "FREQUENCY",
		description: "Prebrojava učestalost ponavljanja vrednosti u okviru opsega vrednosti, a zatim daje vertikalni niz brojeva koji ima jedan element više od niza_bin_vrednosti.",
		arguments: [
			{
				name: "niz_podataka",
				description: "predstavlja niz ili referencu na skup vrednosti čiju učestalost želite da ustanovite (prazne ćelije i tekst se zanemaruju)"
			},
			{
				name: "niz_bin_vrednosti",
				description: "predstavlja niz ili referencu na intervale u koje želite da grupišete vrednosti iz parametra niz_podataka"
			}
		]
	},
	{
		name: "FTEST",
		description: "Daje rezultat F-testa, dvostranu verovatnoću da odstupanja u nizu1 i nizu2 nisu znatno različita.",
		arguments: [
			{
				name: "niz1",
				description: "predstavlja prvi niz ili opseg podataka i može biti dat kao brojevi ili imena, nizovi ili reference koje sadrže brojeve (prazne ćelije se zanemaruju)"
			},
			{
				name: "niz2",
				description: "predstavlja drugi niz ili opseg podataka i može biti dat kao brojevi ili imena, nizovi ili reference koje sadrže brojeve (prazne ćelije se zanemaruju)"
			}
		]
	},
	{
		name: "FV",
		description: "Daje buduću vrednost investicije zasnovane na periodičnim, nepromenljivim otplatama i nepromenljivoj kamatnoj stopi.",
		arguments: [
			{
				name: "stopa",
				description: "predstavlja kamatnu stopu po periodu. Na primer, upotrebite 6%/4 za kvartalne rate, uz godišnju kamatu od 6%"
			},
			{
				name: "br_per",
				description: "predstavlja ukupan broj rata za neku investiciju"
			},
			{
				name: "rata",
				description: "predstavlja iznos rate za svaki period i ne može se menjati tokom trajanja investicije"
			},
			{
				name: "sad_vr",
				description: "predstavlja sadašnju vrednost tj. paušalnu sumu trenutne vrednosti niza budućih rata. Ako je izostavljen, sad_vr = 0"
			},
			{
				name: "tip",
				description: "je vrednost koja predstavlja raspored plaćanja: za ratu na početku perioda ima vrednost 1; za ratu na kraju perioda ima vrednost 0 ili je izostavljen"
			}
		]
	},
	{
		name: "FVSCHEDULE",
		description: "Daje buduću vrednost početne glavnice nakon primene grupe složenih kamatnih stopa.",
		arguments: [
			{
				name: "principal",
				description: "je sadašnja vrednost"
			},
			{
				name: "plan",
				description: "je niz kamatnih stopa koji bi trebalo primeniti"
			}
		]
	},
	{
		name: "GAMMA",
		description: "Daje vrednost gama funkcije.",
		arguments: [
			{
				name: "x",
				description: "je vrednost za koju želite da izračunate gamu"
			}
		]
	},
	{
		name: "GAMMA.DIST",
		description: "Daje gama raspodelu.",
		arguments: [
			{
				name: "x",
				description: "predstavlja nenegativan broj za koji želite da odredite vrednost raspodele"
			},
			{
				name: "alfa",
				description: "predstavlja parametar raspodele, pozitivan broj"
			},
			{
				name: "beta",
				description: "predstavlja parametar raspodele, pozitivan broj. Ako je beta = 1, GAMMA.DIST daje standardnu gama raspodelu"
			},
			{
				name: "kumulativno",
				description: "predstavlja logičku vrednost: vrati funkciju kumulativne raspodele = TRUE; vrati funkciju raspodele = FALSE ili izostavljeno"
			}
		]
	},
	{
		name: "GAMMA.INV",
		description: "Daje inverznu kumulativnu gama raspodelu: ako je p = GAMMA.DIST(x,...), onda je GAMMA.INV(p,...) = x.",
		arguments: [
			{
				name: "verovatnoća",
				description: "predstavlja verovatnoću pridruženu gama raspodeli, broj između 0 i 1, uključujući i njih"
			},
			{
				name: "alfa",
				description: "predstavlja parametar raspodele, neki pozitivan broj"
			},
			{
				name: "beta",
				description: "predstavlja parametar raspodele, pozitivan broj. Ako je beta = 1, GAMMA.INV daje inverznu standardnu gama raspodelu"
			}
		]
	},
	{
		name: "GAMMADIST",
		description: "Daje gama raspodelu.",
		arguments: [
			{
				name: "x",
				description: "predstavlja pozitivan broj za koji želite da odredite vrednost raspodele"
			},
			{
				name: "alfa",
				description: "predstavlja parametar raspodele, pozitivan broj"
			},
			{
				name: "beta",
				description: "predstavlja parametar raspodele, pozitivan broj. Ako je beta = 1, GAMMADIST daje standardnu gama raspodelu"
			},
			{
				name: "kumulativno",
				description: "predstavlja logičku vrednost koja određuje šta funkcija treba da dâ: za kumulativnu funkciju raspodele treba da bude TRUE, a za funkciju raspodele treba da bude FALSE ili izostavljeno"
			}
		]
	},
	{
		name: "GAMMAINV",
		description: "Daje inverznu kumulativnu gama raspodelu: ako je p = GAMMADIST(x,...), onda je GAMMAINV(p,...) = x.",
		arguments: [
			{
				name: "verovatnoća",
				description: "predstavlja verovatnoću povezanu sa gama raspodelom, broj između 0 i 1, uključujući i njih"
			},
			{
				name: "alfa",
				description: "predstavlja parametar raspodele, pozitivan broj"
			},
			{
				name: "beta",
				description: "predstavlja parametar raspodele, pozitivan broj. Ako je beta = 1, GAMMAINV, daje inverznu standardnu gama raspodelu"
			}
		]
	},
	{
		name: "GAMMALN",
		description: "Daje prirodni logaritam gama funkcije.",
		arguments: [
			{
				name: "x",
				description: "predstavlja neki pozitivan broj za koji želite da izračunate GAMMALN"
			}
		]
	},
	{
		name: "GAMMALN.PRECISE",
		description: "Vraća prirodni logaritam gama funkcije.",
		arguments: [
			{
				name: "x",
				description: "predstavlja vrednost u vidu pozitivnog broja za koju želite da izračunate GAMMALN.PRECISE"
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
		description: "Daje najveći zajednički delilac.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su vrednosti od 1 do 255"
			},
			{
				name: "broj2",
				description: "su vrednosti od 1 do 255"
			}
		]
	},
	{
		name: "GEOMEAN",
		description: "Daje geometrijsku srednju vrednost niza ili opsega pozitivnih numeričkih podataka.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "predstavlja 1 do 255 brojeva ili imena, nizova ili referenci koji sadrže brojeve čiju srednju vrednost tražite"
			},
			{
				name: "broj2",
				description: "predstavlja 1 do 255 brojeva ili imena, nizova ili referenci koji sadrže brojeve čiju srednju vrednost tražite"
			}
		]
	},
	{
		name: "GESTEP",
		description: "Proverava da li je broj veći od početne vrednosti.",
		arguments: [
			{
				name: "broj",
				description: "je vrednost za koju se proverava"
			},
			{
				name: "korak",
				description: "je početna vrednost"
			}
		]
	},
	{
		name: "GETPIVOTDATA",
		description: "Izdvaja podatke uskladištene u izvedenoj tabeli.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "polje_podataka",
				description: "predstavlja ime polja podatka iz kojeg bi trebalo izdvojiti podatke"
			},
			{
				name: "izvedena_tabela",
				description: "predstavlja referencu na ćeliju ili opseg ćelija izvedene tabele koje sadrže podatke koje želite da preuzmete"
			},
			{
				name: "polje",
				description: "polje na koje se odnosi"
			},
			{
				name: "stavka",
				description: "stavka polja na koju se odnosi"
			}
		]
	},
	{
		name: "GROWTH",
		description: "Daje brojeve u eksponencijalnom trendu rasta koji odgovaraju poznatim tačkama sa podacima.",
		arguments: [
			{
				name: "poznati_y",
				description: "predstavlja skup y-vrednosti koji vam je već poznat iz relacije y = b*m^x, niz ili opseg pozitivnih brojeva"
			},
			{
				name: "poznati_x",
				description: "predstavlja opcionalni skup x-vrednosti koji vam je možda već poznat iz relacije y = b*m^x, niz ili opseg iste veličine kao i poznati_y"
			},
			{
				name: "novi_x",
				description: "predstavlja nove x-vrednosti za koje želite da funkcija GROWTH dâ odgovarajuće y-vrednosti"
			},
			{
				name: "konstanta",
				description: "predstavlja logičku vrednosti: konstanta b normalno se računa ako argument konstanta = TRUE; b se postavlja na 1 ako argument konstanta ima vrednost FALSE ili je izostavljen"
			}
		]
	},
	{
		name: "HARMEAN",
		description: "Daje harmonijsku srednju vrednost skupa pozitivnih brojeva, tj. recipročnu vrednost aritmetičke sredine recipročnih vrednosti.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "predstavlja 1 do 255 brojeva ili imena, nizova ili referenci koji sadrže brojeve čiju harmonijsku srednju vrednost tražite"
			},
			{
				name: "broj2",
				description: "predstavlja 1 do 255 brojeva ili imena, nizova ili referenci koji sadrže brojeve čiju harmonijsku srednju vrednost tražite"
			}
		]
	},
	{
		name: "HEX2BIN",
		description: "Konvertuje heksadecimalni broj u binarni.",
		arguments: [
			{
				name: "broj",
				description: "je heksadecimalni broj koji želite da konvertujete"
			},
			{
				name: "mesta",
				description: "je broj znakova koji bi trebalo koristiti"
			}
		]
	},
	{
		name: "HEX2DEC",
		description: "Konvertuje heksadecimalni broj u decimalni.",
		arguments: [
			{
				name: "broj",
				description: "je heksadecimalni broj koji želite da konvertujete"
			}
		]
	},
	{
		name: "HEX2OCT",
		description: "Konvertuje heksadecimalni broj u oktalni.",
		arguments: [
			{
				name: "broj",
				description: "je heksadecimalni broj koji želite da konvertujete"
			},
			{
				name: "mesta",
				description: "je broj znakova koji bi trebalo koristiti"
			}
		]
	},
	{
		name: "HLOOKUP",
		description: "Traži vrednost u gornjem redu tabele ili niza vrednosti i daje je u istoj koloni, iz reda koji navedete.",
		arguments: [
			{
				name: "vrednost_za_pronalaženje",
				description: "predstavlja vrednost koja se traži u prvom redu tabele i može biti vrednost, referenca ili tekstualna niska"
			},
			{
				name: "niz_tabele",
				description: "predstavlja tekstualnu tabelu, brojeve ili logičke vrednosti u kojima se podaci traže. Niz_tabele može biti referenca opsega ili imena opsega"
			},
			{
				name: "indeksni_broj_reda",
				description: "predstavlja broj reda u nizu_tabele iz kojeg bi podudarna vrednost trebalo da bude data. Prvi red vrednosti u tabeli je red 1"
			},
			{
				name: "opseg_za_pronalaženje",
				description: "predstavlja logičku vrednost: za pronalaženje najpribližnije vrednosti u gornjem redu (sortiranom po rastućem redosledu) = TRUE ili je izostavljena; za pronalaženje istovetne vrednosti = FALSE"
			}
		]
	},
	{
		name: "HOUR",
		description: "Vraća čas kao broj od 0 (12:00 A.M.) do 23 (11:00 P.M.).",
		arguments: [
			{
				name: "serijski_broj",
				description: "predstavlja broj u Spreadsheet kodu za datum-vreme ili tekst u formatu vremena, kao što je 16:48:00 ili 4:48:00 PM"
			}
		]
	},
	{
		name: "HYPERLINK",
		description: "Kreira prečicu koja otvara neki dokument uskladišten na čvrstom disku, mrežnom serveru ili na Internetu.",
		arguments: [
			{
				name: "lokacija_veze",
				description: "predstavlja tekst koji sadrži putanju i ime datoteke dokumenta koji treba otvoriti, lokaciju na čvrstom disku, UNC adresu ili URL putanju"
			},
			{
				name: "prepoznatljivo_ime",
				description: "predstavlja tekst ili broj koji je prikazan u ćeliji. Ako je izostavljen, ćelija prikazuje tekst lokacije_veze"
			}
		]
	},
	{
		name: "HYPGEOM.DIST",
		description: "Daje hipergeometrijsku raspodelu.",
		arguments: [
			{
				name: "uzorak_s",
				description: "predstavlja broj uspeha u uzorku"
			},
			{
				name: "uzorak_broja",
				description: "predstavlja veličinu uzorka"
			},
			{
				name: "populacija_s",
				description: "predstavlja broj uspeha u populaciji"
			},
			{
				name: "broj_pop",
				description: "predstavlja veličinu populacije"
			},
			{
				name: "kumulativno",
				description: "predstavlja logičku vrednost: za funkciju kumulativne raspodele koristite TRUE; za funkciju gustine verovatnoće koristite FALSE"
			}
		]
	},
	{
		name: "HYPGEOMDIST",
		description: "Daje hipergeometrijsku raspodelu.",
		arguments: [
			{
				name: "uzorak_s",
				description: "predstavlja broj uspeha u uzorku"
			},
			{
				name: "broj_uzorak",
				description: "predstavlja veličinu uzorka"
			},
			{
				name: "populacija_s",
				description: "predstavlja broj uspeha u populaciji"
			},
			{
				name: "broj_pop",
				description: "predstavlja veličinu populacije"
			}
		]
	},
	{
		name: "IF",
		description: "Proverava da li su uslovi zadovoljeni, te daje neku vrednost ako je TRUE ili neku drugu vrednost ako je FALSE.",
		arguments: [
			{
				name: "logički_test",
				description: "predstavlja bilo koju vrednost ili izraz koji se može vrednovati kao TRUE ili FALSE"
			},
			{
				name: "vrednost_ako_je_tačno",
				description: "predstavlja vrednost koja se daje ako se logički_test odnosi na odrednicu TRUE. Daje vrednost TRUE ako je izostavljena. Možete ugnezditi najviše sedam IF funkcija"
			},
			{
				name: "vrednost_ako_je_netačno",
				description: "predstavlja vrednost koja se daje ako se logički_test odnosi na vrednosti FALSE. Daje vrednost FALSE ako je izostavljena"
			}
		]
	},
	{
		name: "IFERROR",
		description: "Daje vrednost_ako_greška ako je izraz greška ili vrednost samog izraza, inače.",
		arguments: [
			{
				name: "vrednost",
				description: "je bilo koja vrednost, izraz ili referenca"
			},
			{
				name: "vrednost_ako_greška",
				description: "je bilo koja vrednost, izraz ili referenca"
			}
		]
	},
	{
		name: "IFNA",
		description: "Daje vrednost koju navedete ako izraz da rezultat tipa #N/A, u suprotnom daje rezultat izraza.",
		arguments: [
			{
				name: "value",
				description: "je bilo koja vrednost ili izraz ili referenca"
			},
			{
				name: "value_if_na",
				description: "je bilo koja vrednost ili izraz ili referenca"
			}
		]
	},
	{
		name: "IMABS",
		description: "Daje apsolutnu vrednost (moduo) kompleksnog broja.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj čiju apsolutnu vrednost želite da dobijete"
			}
		]
	},
	{
		name: "IMAGINARY",
		description: "Daje imaginarni deo kompleksnog broja.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj za koji želite da dobijete imaginarni deo"
			}
		]
	},
	{
		name: "IMARGUMENT",
		description: "Daje argument q, ugao izražen u radijanima.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj za koji želite da dobijete argument"
			}
		]
	},
	{
		name: "IMCONJUGATE",
		description: "Daje konjugovani broj za kompleksni broj.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj za koji želite da dobijete konjugovani broj"
			}
		]
	},
	{
		name: "IMCOS",
		description: "Daje kosinus kompleksnog broja.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj za koji želite da dobijete kosinus"
			}
		]
	},
	{
		name: "IMCOSH",
		description: "Daje hiperbolički kosinus kompleksnog broja.",
		arguments: [
			{
				name: "inumber",
				description: "je kompleksni broj za koji vam je potreban hiperbolički kosinus"
			}
		]
	},
	{
		name: "IMCOT",
		description: "Vraća kotangens kompleksnog broja.",
		arguments: [
			{
				name: "inumber",
				description: "je kompleksni broj za koji želite kotangens"
			}
		]
	},
	{
		name: "IMCSC",
		description: "Vraća kosekans kompleksnog broja.",
		arguments: [
			{
				name: "inumber",
				description: "je kompleksni broj za koji želite kosekans"
			}
		]
	},
	{
		name: "IMCSCH",
		description: "Vraća hiperbolički kosekans kompleksnog broja.",
		arguments: [
			{
				name: "inumber",
				description: "je kompleksni broj za koji želite hiperbolički kosekans"
			}
		]
	},
	{
		name: "IMDIV",
		description: "Daje količnik dva kompleksna broja.",
		arguments: [
			{
				name: "ibroj1",
				description: "je kompleksni brojilac ili deljenik"
			},
			{
				name: "ibroj2",
				description: "je kompleksni imenilac ili delilac"
			}
		]
	},
	{
		name: "IMEXP",
		description: "Daje stepenovanu vrednost kompleksnog broja.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj za koji želite da dobijete stepenovanu vrednost"
			}
		]
	},
	{
		name: "IMLN",
		description: "Daje prirodni logaritam kompleksnog broja.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj čiji prirodni logaritam želite da dobijete"
			}
		]
	},
	{
		name: "IMLOG10",
		description: "Daje logaritam za osnovu 10 kompleksnog broja.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj za koji želite da dobijete uobičajeni logaritam"
			}
		]
	},
	{
		name: "IMLOG2",
		description: "Daje logaritam za osnovu 2 kompleksnog broja.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj čiji logaritam za osnovu 2 želite da dobijete"
			}
		]
	},
	{
		name: "IMPOWER",
		description: "Daje kompleksni broj podignut na stepen koji je ceo broj.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj koji želite da stepenujete"
			},
			{
				name: "broj",
				description: "je stepen na koji želite da dignete kompleksni broj"
			}
		]
	},
	{
		name: "IMPRODUCT",
		description: "Daje proizvod kompleksnih brojeva od 1 do 255.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "ibroj1",
				description: "Ibroj1, Ibroj2,… su kompleksni brojevi od 1 do 255 koji se množe."
			},
			{
				name: "ibroj2",
				description: "Ibroj1, Ibroj2,… su kompleksni brojevi od 1 do 255 koji se množe."
			}
		]
	},
	{
		name: "IMREAL",
		description: "Daje realni deo kompleksnog broja.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj za koji želite da dobijete realni deo"
			}
		]
	},
	{
		name: "IMSEC",
		description: "Vraća sekans kompleksnog broja.",
		arguments: [
			{
				name: "inumber",
				description: "je kompleksni broj za koji želite sekans"
			}
		]
	},
	{
		name: "IMSECH",
		description: "Vraća hiperbolički sekans kompleksnog broja.",
		arguments: [
			{
				name: "inumber",
				description: "je kompleksni broj za koji želite hiperbolički sekans"
			}
		]
	},
	{
		name: "IMSIN",
		description: "Daje sinus kompleksnog broja.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj za koji želite da dobijete sinus"
			}
		]
	},
	{
		name: "IMSINH",
		description: "Daje hiperbolički sinus kompleksnog broja.",
		arguments: [
			{
				name: "inumber",
				description: "je kompleksni broj za koji vam je potreban hiperbolički sinus"
			}
		]
	},
	{
		name: "IMSQRT",
		description: "Daje kvadratni koren kompleksnog broja.",
		arguments: [
			{
				name: "ibroj",
				description: "je kompleksni broj čiji kvadratni koren želite da dobijete"
			}
		]
	},
	{
		name: "IMSUB",
		description: "Daje razliku dva kompleksna broja.",
		arguments: [
			{
				name: "ibroj1",
				description: "je kompleksni broj od koga se oduzima ibroj2"
			},
			{
				name: "ibroj2",
				description: "je kompleksni broj koji se oduzima od parametra ibroj1"
			}
		]
	},
	{
		name: "IMSUM",
		description: "Daje zbir kompleksnih brojeva.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "ibroj1",
				description: "su kompleksni brojevi od 1 do 255 koji se sabiraju"
			},
			{
				name: "ibroj2",
				description: "su kompleksni brojevi od 1 do 255 koji se sabiraju"
			}
		]
	},
	{
		name: "IMTAN",
		description: "Vraća tangens kompleksnog broja.",
		arguments: [
			{
				name: "inumber",
				description: "je kompleksni broj za koji želite tangens"
			}
		]
	},
	{
		name: "INDEX",
		description: "Daje vrednost ili referencu ćelije u preseku određenog reda i kolone u okviru datog opsega.",
		arguments: [
			{
				name: "niz",
				description: "predstavlja opseg ćelija ili konstantan niz."
			},
			{
				name: "broj_reda",
				description: "bira red u nizu ili referenci iz kojeg će dati podatak. Ako se izostavi, neophodan je broj_kolone"
			},
			{
				name: "broj_kolone",
				description: "bira kolonu u nizu ili referenci iz koje će dati podatak. Ako se izostavi, neophodan je broj_reda"
			}
		]
	},
	{
		name: "INDIRECT",
		description: "Daje referencu navedenu tekstualnom niskom.",
		arguments: [
			{
				name: "ref_tekst",
				description: "predstavlja referencu ćelije koja sadrži A1- ili R1C1-referencu stila, ime definisano kao referenca ili referencu ćelije, kao što je tekstualna niska"
			},
			{
				name: "a1",
				description: "predstavlja logičku vrednost koja navodi tip reference u parametru ref_tekst: R1C1-stil = FALSE; A1-stil = TRUE ili je izostavljen"
			}
		]
	},
	{
		name: "INFO",
		description: "Daje informaciju o trenutnom operativnom okruženju.",
		arguments: [
			{
				name: "tip_teksta",
				description: "predstavlja tekst koji navodi koje informacije želite."
			}
		]
	},
	{
		name: "INT",
		description: "Zaokružuje broj naniže na najbliži ceo broj.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja realni broj koji želite da zaokružite naniže na ceo broj"
			}
		]
	},
	{
		name: "INTERCEPT",
		description: "Izračunava tačku u kojoj će linija da se ukrsti sa y-osom, ako je povučena metodom najmanje greške kroz poznate x i y-vrednosti.",
		arguments: [
			{
				name: "poznati_y",
				description: "predstavlja zavisni skup opažanja ili podataka i može biti dat kao brojevi ili imena, nizovi ili reference koje sadrže brojeve"
			},
			{
				name: "poznati_x",
				description: "predstavlja nezavisni skup opažanja ili podataka i može biti dat kao brojevi ili imena, nizovi ili reference koje sadrže brojeve"
			}
		]
	},
	{
		name: "INTRATE",
		description: "Daje kamatnu stopu za potpuno investirano pokriće.",
		arguments: [
			{
				name: "poravnanje",
				description: "je datum obračunavanja pokrića, izražen kao serijski broj datuma"
			},
			{
				name: "zrelost",
				description: "je datum dospevanja pokrića, izražen kao serijski broj datuma"
			},
			{
				name: "ulaganje",
				description: "je svota koja je investirana u pokriće"
			},
			{
				name: "povraćaj",
				description: "je svota koju ćete dobiti po dospeću"
			},
			{
				name: "osnova",
				description: "je tip osnovice za brojanje dana koji bi trebalo koristiti"
			}
		]
	},
	{
		name: "IPMT",
		description: "Daje sumu isplaćene kamate za neku investiciju u toku datog perioda, zasnovane na periodičnim nepromenljivim otplatama i nepromenljivoj kamatnoj stopi.",
		arguments: [
			{
				name: "stopa",
				description: "predstavlja kamatnu stopu po periodu. Na primer, upotrebite 6%/4 za kvartalne rate, uz godišnju kamatu od 6%"
			},
			{
				name: "po",
				description: "predstavlja period za koji želite da utvrdite kamatu i mora biti u opsegu od 1 do br_per"
			},
			{
				name: "br_per",
				description: "predstavlja ukupan broj rata za neku investiciju"
			},
			{
				name: "sad_vr",
				description: "predstavlja sadašnju vrednost tj. paušalnu sumu ukupne trenutne vrednosti niza budućih rata"
			},
			{
				name: "bud_vr",
				description: "predstavlja buduću vrednost ili gotovinski saldo koji želite da postignete nakon poslednje rate. Ako je izostavljen, bud_vr = 0"
			},
			{
				name: "tip",
				description: "je logička vrednost koja predstavlja raspored plaćanja: za ratu na kraju perioda ima vrednost 0 ili je izostavljen, a za ratu na početku perioda ima vrednost 1"
			}
		]
	},
	{
		name: "IRR",
		description: "Daje internu stopu prinosa niza periodičnih novčanih tokova.",
		arguments: [
			{
				name: "vrednosti",
				description: "predstavlja niz ili referencu ćelije koja sadrži brojeve za koje želite da izračunate unutrašnju stopu dobitka"
			},
			{
				name: "procena",
				description: "predstavlja broj za koji pretpostavljate da je približan rezultatu funkcije IRR; ako je izostavljen, koristi se vrednost 0,1 (10 procenata)"
			}
		]
	},
	{
		name: "ISBLANK",
		description: "Proverava da li referenca upućuje na praznu ćeliju i daje TRUE ili FALSE.",
		arguments: [
			{
				name: "vrednost",
				description: "predstavlja ćeliju ili ime koje se odnosi na ćeliju koju želite da proverite"
			}
		]
	},
	{
		name: "ISERR",
		description: "Proverava da li je vrednost greška (#VALUE!, #REF!, #DIV/0!, #NUM!, #NAME? ili #NULL!), isključujući #N/A, i daje TRUE ili FALSE.",
		arguments: [
			{
				name: "vrednost",
				description: "predstavlja vrednost koju želite da proverite. Vrednost može da se odnosi na ćeliju, formulu ili na ime koje se odnosi na ćeliju, formulu ili vrednost"
			}
		]
	},
	{
		name: "ISERROR",
		description: "Proverava da li je vrednost pogrešna (#N/A, #VALUE!, #REF!, #DIV/0!, #NUM!, #NAME?, ili #NULL!) i daje vrednosti TRUE ili FALSE.",
		arguments: [
			{
				name: "vrednost",
				description: "predstavlja vrednost koju želite da testirate. Ova vrednost može da se odnosi na ćeliju, na formulu ili na ime koje se odnosi na neku ćeliju, formulu ili vrednost"
			}
		]
	},
	{
		name: "ISEVEN",
		description: "Daje vrednost TRUE ako je broj paran.",
		arguments: [
			{
				name: "broj",
				description: "je vrednost koja se proverava"
			}
		]
	},
	{
		name: "ISFORMULA",
		description: "Proverava da li je referenca za ćeliju koja sadrži formulu, i vraća TRUE ili FALSE.",
		arguments: [
			{
				name: "reference",
				description: "je referenca za ćeliju koju želite da testirate.  Referenca može biti referenca ćelije, formula ili ime koje se odnosi na ćeliju"
			}
		]
	},
	{
		name: "ISLOGICAL",
		description: "Proverava da li je vrednost logička vrednost (TRUE ili FALSE) i daje TRUE ili FALSE.",
		arguments: [
			{
				name: "vrednost",
				description: "predstavlja vrednost koju želite da proverite. Vrednost može da se odnosi na ćeliju, formulu ili na ime koje se odnosi na ćeliju, formulu ili vrednost"
			}
		]
	},
	{
		name: "ISNA",
		description: "Proverava da li je vrednost #N/A i daje vrednosti TRUE ili FALSE.",
		arguments: [
			{
				name: "vrednost",
				description: "predstavlja vrednost koju želite da testirate. Ova vrednost može da se odnosi na ćeliju, na formulu ili na ime koje se odnosi na neku ćeliju, formulu ili vrednost"
			}
		]
	},
	{
		name: "ISNONTEXT",
		description: "Proverava da li vrednost nije tekst (prazne ćelije ne predstavljaju tekst) i daje TRUE ili FALSE.",
		arguments: [
			{
				name: "vrednost",
				description: "predstavlja vrednost koju želite da proverite: ćeliju; formulu; ili ime koje se odnosi na ćeliju, formulu ili vrednost"
			}
		]
	},
	{
		name: "ISNUMBER",
		description: "Proverava da li je vrednost broj i daje TRUE ili FALSE.",
		arguments: [
			{
				name: "vrednost",
				description: "predstavlja vrednost koju želite da proverite. Vrednost može da se odnosi na ćeliju, formulu ili na ime koje se odnosi na ćeliju, formulu ili vrednost"
			}
		]
	},
	{
		name: "ISO.CEILING",
		description: "Zaokružuje broj na najbliži ceo broj ili na najbliži umnožak argumenta značaj.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja vrednost koju želite da zaokružite"
			},
			{
				name: "značaj",
				description: "predstavlja opcionalni umnožak na koji želite da zaokružite"
			}
		]
	},
	{
		name: "ISODD",
		description: "Daje vrednost TRUE ako je broj neparan.",
		arguments: [
			{
				name: "broj",
				description: "je vrednost koja se proverava"
			}
		]
	},
	{
		name: "ISOWEEKNUM",
		description: "Daje broj od ISO broja sedmice od godine za dati datum.",
		arguments: [
			{
				name: "date",
				description: "je kod datum/vreme koji Spreadsheet koristi za računanje datuma i vremena"
			}
		]
	},
	{
		name: "ISPMT",
		description: "Daje kamatu uplaćenu tokom određenog perioda neke investicije.",
		arguments: [
			{
				name: "stopa",
				description: "kamatna stopa po periodu. Na primer, upotrebite 6%/4 za kvartalne rate, uz godišnju kamatu od 6%"
			},
			{
				name: "po",
				description: "period za koji želite da utvrdite iznos kamate"
			},
			{
				name: "br_per",
				description: "broj rata za neku investiciju"
			},
			{
				name: "sad_vr",
				description: "paušalna suma trenutne vrednosti niza budućih rata"
			}
		]
	},
	{
		name: "ISREF",
		description: "Proverava da li je vrednost referenca i daje TRUE ili FALSE.",
		arguments: [
			{
				name: "vrednost",
				description: " predstavlja vrednost koju želite da proverite. Vrednost može da se odnosi na ćeliju, formulu ili na ime koje se odnosi na ćeliju, formulu ili vrednost"
			}
		]
	},
	{
		name: "ISTEXT",
		description: "Proverava da li je vrednost tekst i daje TRUE ili FALSE.",
		arguments: [
			{
				name: "vrednost",
				description: "predstavlja vrednost koju želite da proverite. Vrednost može da se odnosi na ćeliju, formulu ili na ime koje se odnosi na ćeliju, formulu ili vrednost"
			}
		]
	},
	{
		name: "KURT",
		description: "Daje meru spljoštenosti (kurtozis) skupa podataka.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "predstavlja 1 do 255 brojeva ili imena, nizova ili referenci koji sadrže brojeve čiju meru spljoštenosti tražite"
			},
			{
				name: "broj2",
				description: "predstavlja 1 do 255 brojeva ili imena, nizova ili referenci koji sadrže brojeve čiju meru spljoštenosti tražite"
			}
		]
	},
	{
		name: "LARGE",
		description: "Daje k-tu po veličini (počev od najveće) vrednost skupa podataka. Na primer, peti broj po veličini.",
		arguments: [
			{
				name: "niz",
				description: "predstavlja niz ili opseg numeričkih vrednosti čiju k-tu po veličini vrednost želite da odredite"
			},
			{
				name: "k",
				description: "predstavlja položaj (počev od najvećeg) u nizu i ili opsegu ćelija vrednosti koja bi trebalo da bude data"
			}
		]
	},
	{
		name: "LCM",
		description: "Daje najmanji zajednički sadržalac.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "predstavlja broj vrednosti od 1 do 255 za koje želite da dobijete najmanji zajednički sadržalac"
			},
			{
				name: "broj2",
				description: "predstavlja broj vrednosti od 1 do 255 za koje želite da dobijete najmanji zajednički sadržalac"
			}
		]
	},
	{
		name: "LEFT",
		description: "Daje određeni broj znakova s početka tekstualne niske.",
		arguments: [
			{
				name: "tekst",
				description: "predstavlja tekstualnu nisku iz koje želite da izdvojite znakove"
			},
			{
				name: "broj_znakova",
				description: "navodi koliko znakova želite da bude izdvojeno funkcijom LEFT; 1 ako je izostavljen"
			}
		]
	},
	{
		name: "LEN",
		description: "Daje broj znakova u tekstualnoj niski.",
		arguments: [
			{
				name: "tekst",
				description: "predstavlja tekst čiju dužinu želite da ustanovite. Razmaci se broje kao znaci"
			}
		]
	},
	{
		name: "LINEST",
		description: "Daje statističke podatke koji opisuju linearni trend koji odgovara poznatim podacima o tačkama, tako što korišćenjem metoda najmanjih kvadrata povlači pravu liniju.",
		arguments: [
			{
				name: "poznati_y",
				description: "predstavlja skup y-vrednosti koji vam je već poznat iz relacije y = mx + b"
			},
			{
				name: "poznati_x",
				description: "predstavlja opcionalni skup x-vrednosti koji možda već poznajete iz relacije y = mx + b"
			},
			{
				name: "konstanta",
				description: "predstavlja logičku vrednost: konstanta b računa se normalno ako argument konstanta ima vrednost TRUE ili je izostavljen; b se postavlja na 0 ako je konstanta = FALSE"
			},
			{
				name: "statistika",
				description: "predstavlja logičku vrednost: daje dodatne statističke podatke o regresiji ako ima vrednost TRUE; daje m-koeficijente i konstantu b ako ima vrednost FALSE ili je izostavljen"
			}
		]
	},
	{
		name: "LN",
		description: "Daje prirodni logaritam broja.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja pozitivni realni broj čiji prirodni logaritam želite"
			}
		]
	},
	{
		name: "LOG",
		description: "Daje logaritam broja sa osnovom koju navedete.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja pozitivan realni broj čiji logaritam želite"
			},
			{
				name: "osnova",
				description: "predstavlja osnovu logaritma; ako je izostavljen, koristi se osnova 10"
			}
		]
	},
	{
		name: "LOG10",
		description: "Daje dekadni (obični) logaritam broja.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja pozitivni realni broj čiji dekadni logaritam želite"
			}
		]
	},
	{
		name: "LOGEST",
		description: "Daje statističke podatke koji opisuju eksponencijalnu krivu koja odgovara poznatim podacima.",
		arguments: [
			{
				name: "poznati_y",
				description: "predstavlja skup y-vrednosti koji vam je već poznat iz relacije y = b*m^x"
			},
			{
				name: "poznati_x",
				description: "predstavlja opcionalni skup x-vrednosti koji vam je možda već poznat iz relacije y = b*m^x"
			},
			{
				name: "konstanta",
				description: "predstavlja logičku vrednost: konstanta b normalno se računa ako argument konstanta ima vrednost TRUE ili je izostavljen; b se postavlja na 1 ako je konstanta = FALSE"
			},
			{
				name: "statistika",
				description: "predstavlja logičku vrednost: daje dodatne statističke podatke o regresiji ako ima vrednost TRUE; daje m-koeficijente i konstantu b ako ima vrednost FALSE ili je izostavljen"
			}
		]
	},
	{
		name: "LOGINV",
		description: "Daje inverznu kumulativnu log-normalnu funkciju raspodele za x, gde ln(x) ima normalnu raspodelu sa parametrima prosek i standardna_dev.",
		arguments: [
			{
				name: "verovatnoća",
				description: "predstavlja verovatnoću povezanu sa log-normalnom raspodelom, broj između 0 i 1, uključujući i njih"
			},
			{
				name: "prosek",
				description: "predstavlja srednju vrednost za ln(x)"
			},
			{
				name: "standardna_dev",
				description: "predstavlja standardnu devijaciju za ln(x), pozitivan broj"
			}
		]
	},
	{
		name: "LOGNORM.DIST",
		description: "Daje log-normalnu raspodelu za x, gde ln(x) ima normalnu raspodelu sa parametrima prosek i standardna_dev.",
		arguments: [
			{
				name: "x",
				description: "predstavlja vrednost za koju se računa funkcija, pozitivan broj"
			},
			{
				name: "prosek",
				description: "predstavlja srednju vrednost za ln(x)"
			},
			{
				name: "standardna_dev",
				description: "predstavlja standardnu devijaciju za ln(x), pozitivan broj"
			},
			{
				name: "kumulativno",
				description: ": predstavlja logičku vrednost: za funkciju kumulativne raspodele koristite TRUE; za funkciju gustine verovatnoće koristite FALSE"
			}
		]
	},
	{
		name: "LOGNORM.INV",
		description: "Daje inverznu kumulativnu log-normalnu funkciju raspodelu za x, gde ln(x) ima normalnu raspodelu sa parametrima prosek i standardna_dev.",
		arguments: [
			{
				name: "verovatnoća",
				description: "predstavlja verovatnoću koja je povezana sa log-normalnom raspodelom, broj između 0 i 1, uključujući i njih"
			},
			{
				name: "prosek",
				description: "predstavlja srednju vrednost za ln(x)"
			},
			{
				name: "standardna_dev",
				description: "predstavlja standardnu devijaciju za ln(x), pozitivan broj"
			}
		]
	},
	{
		name: "LOGNORMDIST",
		description: "Daje kumulativnu log-normalnu raspodelu za x, gde ln(x) ima normalnu raspodelu sa parametrima prosek i standardna_dev.",
		arguments: [
			{
				name: "x",
				description: "predstavlja vrednost za koju se računa funkcija, pozitivan broj"
			},
			{
				name: "prosek",
				description: "predstavlja srednju vrednost za ln(x)"
			},
			{
				name: "standardna_dev",
				description: "predstavlja standardnu devijaciju za ln(x), pozitivan broj"
			}
		]
	},
	{
		name: "LOOKUP",
		description: "Traži vrednost u opsegu jednog reda ili kolone odnosno u nizu. Postoji u cilju kompatibilnosti sa prethodnim verzijama.",
		arguments: [
			{
				name: "vrednost_za_pronalaženje",
				description: "predstavlja vrednost koju funkcija LOOKUP traži u vektoru_za_pronalaženje i može biti broj, tekst, logička vrednost, ime ili referenca vrednosti"
			},
			{
				name: "vektor_za_pronalaženje",
				description: "predstavlja opseg koji sadrži samo jedan red ili kolonu teksta, brojeva ili logičkih vrednosti, sortiranih po rastućem redosledu"
			},
			{
				name: "vektor_rezultata",
				description: "predstavlja opseg koji sadrži samo jedan red ili kolonu i iste je veličine kao vektor_za_pronalaženje"
			}
		]
	},
	{
		name: "LOWER",
		description: "Pretvara sva slova neke tekstualne niske u mala slova.",
		arguments: [
			{
				name: "tekst",
				description: " predstavlja tekst čija slova želite da pretvorite u mala slova. Znaci u tekstu koji ne predstavljaju slova ostaju nepromenjeni"
			}
		]
	},
	{
		name: "MATCH",
		description: "Daje relativni položaj stavke u nizu koja se podudara sa navedenom vrednošću u navedenom redosledu.",
		arguments: [
			{
				name: "vrednost_za_pronalaženje",
				description: "predstavlja vrednost koju koristite da biste pronašli željenu vrednost u nizu, neki broj, tekst, logičku vrednost ili njihovu referencu"
			},
			{
				name: "niz_za_pronalaženje",
				description: "predstavlja celovit opseg ćelija koje sadrže moguće tražene vrednosti, niz vrednosti ili referencu na neki niz"
			},
			{
				name: "tip_podudaranja",
				description: "predstavlja broj 1, 0 ili -1, koji ukazuje koja će vrednost biti data."
			}
		]
	},
	{
		name: "MAX",
		description: "Daje najveću vrednost u skupu vrednosti. Zanemaruje logičke vrednosti i tekstove.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "predstavlja 1 do 255 brojeva, praznih ćelija, logičkih vrednosti ili tekstualnih brojeva za koje želite da utvrdite maksimalnu vrednost"
			},
			{
				name: "broj2",
				description: "predstavlja 1 do 255 brojeva, praznih ćelija, logičkih vrednosti ili tekstualnih brojeva za koje želite da utvrdite maksimalnu vrednost"
			}
		]
	},
	{
		name: "MAXA",
		description: "Daje najveću vrednost u skupu vrednosti. Ne zanemaruje logičke vrednosti i tekst.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrednost1",
				description: "predstavlja 1 do 255 brojeva, praznih ćelija, logičkih vrednosti ili tekstualnih brojeva čiji maksimum želite"
			},
			{
				name: "vrednost2",
				description: "predstavlja 1 do 255 brojeva, praznih ćelija, logičkih vrednosti ili tekstualnih brojeva čiji maksimum želite"
			}
		]
	},
	{
		name: "MDETERM",
		description: "Daje matričnu determinantu niza.",
		arguments: [
			{
				name: "niz",
				description: "predstavlja numerički niz sa jednakim brojem redova i kolona, bilo da je reč o opsegu ćelija ili o konstantnom nizu"
			}
		]
	},
	{
		name: "MEDIAN",
		description: "Daje medijanu ili središnji broj skupa datih brojeva.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "predstavlja 1 do 255 brojeva ili imena, nizova ili referenci koje sadrže brojeve za koje želite da odredite medijanu"
			},
			{
				name: "broj2",
				description: "predstavlja 1 do 255 brojeva ili imena, nizova ili referenci koje sadrže brojeve za koje želite da odredite medijanu"
			}
		]
	},
	{
		name: "MID",
		description: "Daje znake iz sredine teksta ako su zadati početna pozicija i dužina.",
		arguments: [
			{
				name: "tekst",
				description: "predstavlja tekst iz kojeg želite da izdvojite neke znake"
			},
			{
				name: "početak_broj",
				description: "predstavlja poziciju prvog od znakova koji želite da izdvojite. Prvi znak u tekstu je 1"
			},
			{
				name: "broj_znakova",
				description: "određuje koliko će znakova iz teksta biti dato"
			}
		]
	},
	{
		name: "MIN",
		description: "Daje najmanji broj u skupu vrednosti. Zanemaruje logičke vrednosti i tekstove.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "predstavlja 1 do 255 brojeva, praznih ćelija, logičkih vrednosti ili tekstualnih brojeva za koje želite da utvrdite minimalnu vrednost"
			},
			{
				name: "broj2",
				description: "predstavlja 1 do 255 brojeva, praznih ćelija, logičkih vrednosti ili tekstualnih brojeva za koje želite da utvrdite minimalnu vrednost"
			}
		]
	},
	{
		name: "MINA",
		description: "Daje najmanju vrednost u skupu vrednosti. Ne zanemaruje logičke vrednosti i tekst.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrednost1",
				description: "predstavlja 1 do 255 brojeva, praznih ćelija, logičkih vrednosti ili tekstualnih brojeva čiji minimum želite"
			},
			{
				name: "vrednost2",
				description: "predstavlja 1 do 255 brojeva, praznih ćelija, logičkih vrednosti ili tekstualnih brojeva čiji minimum želite"
			}
		]
	},
	{
		name: "MINUTE",
		description: "Vraća minut, broj od 0 do 59.",
		arguments: [
			{
				name: "serijski_broj",
				description: "predstavlja broj u Spreadsheet kodu za datum-vreme ili tekst u formatu vremena, kao što je 16:48:00 ili 4:48:00 PM"
			}
		]
	},
	{
		name: "MINVERSE",
		description: "Daje inverznu matricu matrice pohranjene u nizu.",
		arguments: [
			{
				name: "niz",
				description: "predstavlja numerički niz sa jednakim brojem redova i kolona, bilo da je reč o opsegu ćelija ili o konstantnom nizu"
			}
		]
	},
	{
		name: "MIRR",
		description: "Daje internu stopu prinosa za niz periodičnih novčanih tokova, imajući u vidu i troškove investicije i kamatu na ponovo investiranu gotovinu.",
		arguments: [
			{
				name: "vrednosti",
				description: "predstavlja niz ili referencu ćelije sa brojevima koji predstavljaju niz rata (negativni) i prihoda (pozitivni) u redovnim periodima"
			},
			{
				name: "finansijska_stopa",
				description: "predstavlja kamatnu stopu koju plaćate na upotrebljeni gotovinski novac"
			},
			{
				name: "stopa_ponovnog_ulaganja",
				description: "predstavlja kamatnu stopu koju naplaćujete za novčane tokove kada ih ponovo investirate"
			}
		]
	},
	{
		name: "MMULT",
		description: "Daje matrični proizvod dva niza, niz sa brojem vrsta kao niz1 i brojem kolona kao niz2.",
		arguments: [
			{
				name: "niz1",
				description: "predstavlja prvi niz sa brojevima koji treba pomnožiti i on mora da ima tačno onoliko redova koliko niz2 ima kolona"
			},
			{
				name: "niz2",
				description: "predstavlja prvi niz sa brojevima koji treba pomnožiti i on mora da ima tačno onoliko redova koliko niz2 ima kolona"
			}
		]
	},
	{
		name: "MOD",
		description: "Daje ostatak pri deljenju broja deliocem.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja broj čiji ostatak pri deljenju želite da nađete"
			},
			{
				name: "delilac",
				description: "predstavlja broj kojim želite da podelite argument broj"
			}
		]
	},
	{
		name: "MODE",
		description: "Daje najčešću vrednost ili vrednost koja se najviše puta ponavlja u nizu ili opsegu podataka.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "predstavlja 1 do 255 brojeva, imena, nizova ili referenci koji sadrže brojeve čiju dominantnu vrednost tražite"
			},
			{
				name: "broj2",
				description: "predstavlja 1 do 255 brojeva, imena, nizova ili referenci koji sadrže brojeve čiju dominantnu vrednost tražite"
			}
		]
	},
	{
		name: "MODE.MULT",
		description: "Daje vertikalni niz vrednosti koje se najčešće pojavljuju ili se ponavljaju u nizu ili opsegu podataka. Za horizontalni niz koristite =TRANSPOSE(MODE.MULT(broj1,broj2,...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "predstavlja brojeve od 1 do 255 ili imena, nizove ili referenci koje sadrže brojeve za koje želite režim"
			},
			{
				name: "broj2",
				description: "predstavlja brojeve od 1 do 255 ili imena, nizove ili referenci koje sadrže brojeve za koje želite režim"
			}
		]
	},
	{
		name: "MODE.SNGL",
		description: "Daje najčešću vrednost ili vrednost koja se najviše puta ponavlja u nizu ili opsegu podataka.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "predstavlja brojeve od 1 do 255 ili imena, nizove ili reference koje sadrže brojeve čiju dominantnu vrednost tražite"
			},
			{
				name: "broj2",
				description: "predstavlja brojeve od 1 do 255 ili imena, nizove ili reference koje sadrže brojeve čiju dominantnu vrednost tražite"
			}
		]
	},
	{
		name: "MONTH",
		description: "Vraća mesec, broj od 1 (januar) do 12 (decembar).",
		arguments: [
			{
				name: "serijski_broj",
				description: "predstavlja broj u Spreadsheet kodu za datum-vreme"
			}
		]
	},
	{
		name: "MROUND",
		description: "Daje broj zaokružen na željeni umnožak.",
		arguments: [
			{
				name: "broj",
				description: "je vrednost koja se zaokružuje"
			},
			{
				name: "višestruki",
				description: "je umnožak na koji želite da zaokružite broj"
			}
		]
	},
	{
		name: "MULTINOMIAL",
		description: "Daje multinomial za skup brojeva.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "su vrednost od 1 do 255 za koje želite da dobijete"
			},
			{
				name: "broj2",
				description: "su vrednost od 1 do 255 za koje želite da dobijete"
			}
		]
	},
	{
		name: "MUNIT",
		description: "Daje matricu jedinice za navedenu dimenziju.",
		arguments: [
			{
				name: "dimension",
				description: "je ceo broj koji navodi dimenziju matrice jedinice koju želite da dobijete"
			}
		]
	},
	{
		name: "N",
		description: "Pretvara vrednosti koje nisu numeričke u broj, a datume u redne brojeve, TRUE u 1, sve ostalo u 0 (nulu).",
		arguments: [
			{
				name: "vrednost",
				description: "predstavlja vrednost koju želite da pretvorite"
			}
		]
	},
	{
		name: "NA",
		description: "Daje vrednost greške #N/A (vrednost nije dostupna).",
		arguments: [
		]
	},
	{
		name: "NEGBINOM.DIST",
		description: "Daje negativnu binomnu raspodelu, verovatnoću da će se desiti broj_f neuspeha pre broj_s-tog uspeha, uz verovatnoću uspeha verovatnoća_s.",
		arguments: [
			{
				name: "broj_f",
				description: "predstavlja broj neuspeha"
			},
			{
				name: "broj_s",
				description: "je broj uspeha koji predstavlja prag"
			},
			{
				name: "verovatnoća_s",
				description: "predstavlja verovatnoću uspeha; broj od 0 do 1"
			},
			{
				name: "kumulativno",
				description: "predstavlja logičku vrednost: za funkciju kumulativne raspodele koristite TRUE; za funkciju raspodele koristite FALSE"
			}
		]
	},
	{
		name: "NEGBINOMDIST",
		description: "Daje negativnu binomnu raspodelu, verovatnoću da će se desiti broj_f neuspeha pre broj_s-tog uspeha, uz verovatnoću uspeha verovatnoća_s.",
		arguments: [
			{
				name: "broj_f",
				description: "predstavlja broj neuspeha"
			},
			{
				name: "broj_s",
				description: "je broj uspeha koji predstavlja prag"
			},
			{
				name: "verovatnoća_s",
				description: "predstavlja verovatnoću uspeha; broj između 0 i 1"
			}
		]
	},
	{
		name: "NETWORKDAYS",
		description: "Daje broj celih radnih dana između dva datuma.",
		arguments: [
			{
				name: "datum_početka",
				description: "je serijski broj datuma koji predstavlja datum početka"
			},
			{
				name: "datum_završetka",
				description: "je serijski broj datuma koji predstavlja datum završetka"
			},
			{
				name: "praznici",
				description: "je opcionalni niz nekih serijskih brojeva datuma koje bi trebalo isključiti iz radnog kalendara, kao što su državni, savezni i promenljivi praznici"
			}
		]
	},
	{
		name: "NETWORKDAYS.INTL",
		description: "Vraća broj celih radnih dana između dva datuma sa prilagođenim parametrima vikenda.",
		arguments: [
			{
				name: "datum_početka",
				description: "je serijski broj datuma koji predstavlja datum početka"
			},
			{
				name: "datum_završetka",
				description: "je serijski broj datuma koji predstavlja datum završetka"
			},
			{
				name: "vikend",
				description: "predstavlja broj ili nisku koja navodi kada dolazi do vikenda"
			},
			{
				name: "praznici",
				description: "predstavlja opcionalni skup nekih serijskih brojeva datuma koje treba isključiti iz radnog kalendara kao što su državni, savezni i promenljivi praznici"
			}
		]
	},
	{
		name: "NOMINAL",
		description: "Daje nominalnu godišnju kamatnu stopu.",
		arguments: [
			{
				name: "efektivna_stopa",
				description: "je efektivna kamatna stopa"
			},
			{
				name: "br_godišnje",
				description: "je godišnji broj perioda za spajanje"
			}
		]
	},
	{
		name: "NORM.DIST",
		description: "Daje normalnu raspodelu za navedenu srednju vrednost i standardnu devijaciju.",
		arguments: [
			{
				name: "x",
				description: "predstavlja vrednost čiju raspodelu želite"
			},
			{
				name: "prosek",
				description: "predstavlja aritmetičku sredinu raspodele"
			},
			{
				name: "standardna_dev",
				description: "predstavlja standardnu devijaciju raspodele, neki pozitivan broj"
			},
			{
				name: "kumulativno",
				description: "predstavlja logičku vrednost: za funkciju kumulativne raspodele koristite TRUE; za funkciju gustine verovatnoće koristite FALSE"
			}
		]
	},
	{
		name: "NORM.INV",
		description: "Daje inverznu normalnu kumulativnu raspodele za navedenu srednju vrednost i standardnu devijaciju.",
		arguments: [
			{
				name: "verovatnoća",
				description: "predstavlja verovatnoću pridruženu normalnoj raspodeli, broj između 0 i 1, uključujući i njih"
			},
			{
				name: "prosek",
				description: "predstavlja aritmetičku srednju vrednost raspodele"
			},
			{
				name: "standardna_dev",
				description: "predstavlja standardnu devijaciju raspodele, pozitivan broj"
			}
		]
	},
	{
		name: "NORM.S.DIST",
		description: "Daje standardnu normalnu raspodelu (ima srednju vrednost jednaku nuli i standardnu devijaciju jednaku jedinici).",
		arguments: [
			{
				name: "z",
				description: "predstavlja vrednost za koju želite da izračunate raspodelu"
			},
			{
				name: "kumulativno",
				description: "predstavlja logičku vrednost koju funkcija treba da dâ: funkcija kumulativne raspodele = TRUE; funkcija gustine verovatnoće = FALSE"
			}
		]
	},
	{
		name: "NORM.S.INV",
		description: "Daje inverznu standardnu normalnu kumulativnu raspodelu (ima srednju vrednost jednaku nuli i standardnu devijaciju jednaku jedinici).",
		arguments: [
			{
				name: "verovatnoća",
				description: "predstavlja verovatnoću koja odgovara normalnoj raspodeli, broj između 0 i 1, uključujući i njih"
			}
		]
	},
	{
		name: "NORMDIST",
		description: "Daje normalnu kumulativnu raspodelu za navedenu srednju vrednost i standardnu devijaciju.",
		arguments: [
			{
				name: "x",
				description: "predstavlja vrednost čiju raspodelu želite"
			},
			{
				name: "prosek",
				description: "predstavlja aritmetičku srednju vrednost raspodele"
			},
			{
				name: "standardna_dev",
				description: "predstavlja standardnu devijaciju raspodele, pozitivan broj"
			},
			{
				name: "kumulativno",
				description: "predstavlja logičku vrednost: za kumulativnu funkciju raspodele koristiti TRUE; za funkciju raspodele koristi FALSE"
			}
		]
	},
	{
		name: "NORMINV",
		description: "Daje inverznu normalnu kumulativnu raspodelu za navedenu srednju vrednost i standardnu devijaciju.",
		arguments: [
			{
				name: "verovatnoća",
				description: "predstavlja verovatnoću koja odgovara normalnoj raspodeli, broj između 0 i 1, uključujući i njih"
			},
			{
				name: "prosek",
				description: "predstavlja aritmetičku srednju vrednost raspodele"
			},
			{
				name: "standardna_dev",
				description: "predstavlja standardnu devijaciju raspodele, pozitivan broj"
			}
		]
	},
	{
		name: "NORMSDIST",
		description: "Daje standardnu normalnu kumulativnu raspodelu (ima srednju vrednost jednaku nuli i standardnu devijaciju jednaku jedinici).",
		arguments: [
			{
				name: "z",
				description: "predstavlja vrednost za koju želite da izračunate raspodelu"
			}
		]
	},
	{
		name: "NORMSINV",
		description: "Daje inverznu standardnu normalnu kumulativnu raspodelu (ima srednju vrednost jednaku nuli i standardnu devijaciju jednaku jedan).",
		arguments: [
			{
				name: "verovatnoća",
				description: "predstavlja verovatnoću koja odgovara normalnoj raspodeli, broj između 0 i 1, uključujući i njih"
			}
		]
	},
	{
		name: "NOT",
		description: "Pretvara FALSE u TRUE ili TRUE u FALSE.",
		arguments: [
			{
				name: "logičko",
				description: "predstavlja vrednost ili izraz koji po izračunavanju može imati vrednost TRUE ili FALSE"
			}
		]
	},
	{
		name: "NOW",
		description: "Daje današnji datum i trenutno vreme oblikovane kao datum i vreme.",
		arguments: [
		]
	},
	{
		name: "NPER",
		description: "Daje broj perioda za investiciju zasnovanu na periodičnim, nepromenljivim otplatama i nepromenljivoj kamatnoj stopi.",
		arguments: [
			{
				name: "stopa",
				description: "predstavlja kamatnu stopu po periodu"
			},
			{
				name: "rata",
				description: " Na primer, upotrebite 6%/4 za kvartalne rate, uz godišnju kamatu od 6%"
			},
			{
				name: "sad_vr",
				description: "predstavlja iznos rate za svaki period i ne može se menjati tokom trajanja investicije"
			},
			{
				name: "bud_vr",
				description: "predstavlja sadašnju vrednost tj. paušalnu sumu trenutne vrednosti niza budućih rata ili gotovinski saldo koji želite da postignete nakon poslednje rate. Ako je izostavljen, upotrebljava se nula"
			},
			{
				name: "tip",
				description: "predstavlja logičku vrednost: za ratu na početku perioda ima vrednost 1; za ratu na kraju perioda ima vrednost 0 ili je izostavljen"
			}
		]
	},
	{
		name: "NPV",
		description: "Daje neto sadašnju vrednost investicije baziranu na diskontnoj stopi, grupi budućih rata (negativne vrednosti) i prihodu (pozitivne vrednosti).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "stopa",
				description: "predstavlja diskontnu stopu u toku jednog perioda"
			},
			{
				name: "vrednost1",
				description: "predstavlja 1 do 254 rata i prihoda, ravnomerno vremenski raspoređenih, koji se realizuju na kraju svakog perioda"
			},
			{
				name: "vrednost2",
				description: "predstavlja 1 do 254 rata i prihoda, ravnomerno vremenski raspoređenih, koji se realizuju na kraju svakog perioda"
			}
		]
	},
	{
		name: "NUMBERVALUE",
		description: "Konvertuje tekst u broj na način koji je nezavistan od lokalnog standarda.",
		arguments: [
			{
				name: "text",
				description: "je niska koja predstavlja broj koji želite da konvertujete"
			},
			{
				name: "decimal_separator",
				description: "je znak korišćen kao decimalni razdelnik u niski"
			},
			{
				name: "group_separator",
				description: "je znak korišćen kao razdelnik grupe u niski"
			}
		]
	},
	{
		name: "OCT2BIN",
		description: "Konvertuje oktalni broj u binarni.",
		arguments: [
			{
				name: "broj",
				description: "je oktalni broj koji želite da konvertujete"
			},
			{
				name: "mesta",
				description: "je broj znakova koji bi trebalo koristiti"
			}
		]
	},
	{
		name: "OCT2DEC",
		description: "Konvertuje oktalni broj u decimalni.",
		arguments: [
			{
				name: "broj",
				description: "je oktalni broj koji želite da konvertujete"
			}
		]
	},
	{
		name: "OCT2HEX",
		description: "Konvertuje oktalni broj u heksadecimalni.",
		arguments: [
			{
				name: "broj",
				description: "je oktalni broj koji želite da konvertujete"
			},
			{
				name: "mesta",
				description: "je broj znakova koji bi trebalo koristiti"
			}
		]
	},
	{
		name: "ODD",
		description: "Zaokružuje pozitivni broj naviše i negativni naniže na najbliži ceo neparan broj.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja vrednost koju treba zaokružiti"
			}
		]
	},
	{
		name: "OFFSET",
		description: "Daje referencu na opseg koji je dati broj redova i kolona date reference.",
		arguments: [
			{
				name: "referenca",
				description: "predstavlja referencu na kojoj želite da bazirate pomak, referencu jedne ili niza susednih ćelija"
			},
			{
				name: "redovi",
				description: "predstavlja broj redova, gornjih ili donjih, na koje želite da se odnosi gornja leva ćelija rezultata"
			},
			{
				name: "ćelije",
				description: "predstavlja određeni broj kolona, s leve ili sa desne strane, na koje želite da se odnosi gornja, leva ćelija rezultata"
			},
			{
				name: "visina",
				description: "predstavlja visinu željenog rezultata, izraženu u brojevima redova, a biće iste visine kao i argument referenca, ako je izostavljen"
			},
			{
				name: "širina",
				description: "predstavlja širinu željenog rezultata, izraženu u brojevima kolona, a biće iste širine kao i argument referenca, ako je izostavljen"
			}
		]
	},
	{
		name: "OR",
		description: "Proverava da li je bilo koji od argumenata TRUE i daje TRUE ili FALSE. Daje FALSE samo ako su svi argumenti FALSE.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logičko1",
				description: "predstavlja 1 do 255 uslova koje želite da testirate, a koji mogu biti TRUE ili FALSE"
			},
			{
				name: "logičko2",
				description: "predstavlja 1 do 255 uslova koje želite da testirate, a koji mogu biti TRUE ili FALSE"
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
		description: "Daje broj perioda potrebnih da investicija dostigne navedenu vrednost.",
		arguments: [
			{
				name: "rate",
				description: "je kamatna stopa po periodu."
			},
			{
				name: "pv",
				description: "je sadašnja vrednost investicije"
			},
			{
				name: "fv",
				description: "je željena buduća vrednost investicije"
			}
		]
	},
	{
		name: "PEARSON",
		description: "Daje Pirsonov koeficijent korelacije (proizvoda momenata), r.",
		arguments: [
			{
				name: "niz1",
				description: "predstavlja skup nezavisnih vrednosti"
			},
			{
				name: "niz2",
				description: "predstavlja skup zavisnih vrednosti"
			}
		]
	},
	{
		name: "PERCENTILE",
		description: "Daje k-ti percentil vrednosti iz opsega.",
		arguments: [
			{
				name: "niz",
				description: "predstavlja niz ili opseg podataka koji definiše relativni poredak"
			},
			{
				name: "k",
				description: "predstavlja vrednost percentila u rasponu od 0 do 1, uključujući i njih"
			}
		]
	},
	{
		name: "PERCENTILE.EXC",
		description: "Daje k-ti percentil vrednosti u opsegu, pri čemu se k nalazi u opsegu 0..1, isključujući te brojeve.",
		arguments: [
			{
				name: "niz",
				description: "predstavlja niz ili opseg podataka koji definiše relativni poredak"
			},
			{
				name: "k",
				description: "predstavlja vrednost percentila od 0 do 1, uključujući te brojeve"
			}
		]
	},
	{
		name: "PERCENTILE.INC",
		description: "Daje k-ti percentil vrednosti u opsegu, pri čemu se k nalazi u opsegu 0..1, uključujući te brojeve.",
		arguments: [
			{
				name: "niz",
				description: "predstavlja niz ili opseg podataka koji definiše relativni poredak"
			},
			{
				name: "k",
				description: "predstavlja vrednost percentila od 0 do 1, uključujući te brojeve"
			}
		]
	},
	{
		name: "PERCENTRANK",
		description: "Daje rang vrednosti u skupu podataka kao procenat skupa podataka.",
		arguments: [
			{
				name: "niz",
				description: "predstavlja niz ili opseg podataka sa numeričkim vrednostima koji definišu relativni poredak"
			},
			{
				name: "x",
				description: "predstavlja vrednost čiji rang želite"
			},
			{
				name: "značaj",
				description: "je opcionalna vrednost koja predstavlja broj značajnih cifara vraćenog procenta, a ako se izostavi, koriste se tri cifre (0,xxx%)"
			}
		]
	},
	{
		name: "PERCENTRANK.EXC",
		description: "Daje rang vrednosti u skupu podataka kao procenat skupa podataka kao procenat (0..1, isključujući te brojeve) skupa podataka.",
		arguments: [
			{
				name: "niz",
				description: "predstavlja niz ili opseg podataka sa numeričkim vrednostima koji definiše relativni poredak"
			},
			{
				name: "x",
				description: "predstavlja vrednost čiji rang želite"
			},
			{
				name: "značaj",
				description: "je opcionalna vrednost koja predstavlja broj značajnih cifara vraćenog procenta, a ako se izostavi, koriste se tri cifre (0.xxx%)"
			}
		]
	},
	{
		name: "PERCENTRANK.INC",
		description: "Daje rang vrednosti u skupu podataka kao procenat skupa podataka kao procenat (0..1, uključujući te brojeve) skupa podataka.",
		arguments: [
			{
				name: "niz",
				description: "predstavlja niz ili opseg podataka sa numeričkim vrednostima koji definiše relativni poredak"
			},
			{
				name: "x",
				description: "predstavlja vrednost čiji rang želite"
			},
			{
				name: "značaj",
				description: "je opcionalna vrednost koja predstavlja broj značajnih cifara vraćenog procenta, a ako se izostavi, koriste se tri cifre (0.xxx%)"
			}
		]
	},
	{
		name: "PERMUT",
		description: "Daje broj permutacija za dati broj objekata koji mogu biti izabrani iz svih objekata.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja ukupan broj objekata"
			},
			{
				name: "odabrani_broj",
				description: "predstavlja broj objekata svake permutacije"
			}
		]
	},
	{
		name: "PERMUTATIONA",
		description: "Daje broj permutacija za dati broj objekata (sa ponavljanjima) koji se može izabrati iz svih projekata.",
		arguments: [
			{
				name: "number",
				description: "je ukupni broj objekata"
			},
			{
				name: "number_chosen",
				description: "je broj objekata u svakoj permutaciji"
			}
		]
	},
	{
		name: "PHI",
		description: "Daje vrednost funkcije gustine za standardnu normalnu raspodelu.",
		arguments: [
			{
				name: "x",
				description: "je broj za koji vam je potrebna gustina standardne normalne raspodele"
			}
		]
	},
	{
		name: "PI",
		description: "Daje vrednost broja Pi, 3,14159265358979, sa 15 tačnih cifara.",
		arguments: [
		]
	},
	{
		name: "PMT",
		description: "Izračunava rate za kredit zasnovan na nepromenljivim otplatama i nepromenljivoj kamatnoj stopi.",
		arguments: [
			{
				name: "stopa",
				description: "predstavlja kamatnu kreditnu stopu po periodu. Na primer, upotrebite 6%/4 za kvartalne rate, uz godišnju kamatu od 6%"
			},
			{
				name: "br_per",
				description: "predstavlja ukupan broj uplata za kredit"
			},
			{
				name: "sad_vr",
				description: "predstavlja sadašnju vrednost: ukupnu trenutnu vrednost niza budućih rata"
			},
			{
				name: "bud_vr",
				description: "predstavlja buduću vrednost ili gotovinski saldo koji želite da postignete nakon poslednje rate, 0 (nula) ako je izostavljen"
			},
			{
				name: "tip",
				description: "predstavlja logičku vrednost: za ratu na početku perioda ima vrednost 1; za ratu na kraju perioda ima vrednost 0 ili je izostavljen"
			}
		]
	},
	{
		name: "POISSON",
		description: "Daje Puasonovu raspodelu.",
		arguments: [
			{
				name: "x",
				description: "predstavlja broj događaja"
			},
			{
				name: "prosek",
				description: "predstavlja očekivanu numeričku vrednost, pozitivan broj"
			},
			{
				name: "kumulativno",
				description: "predstavlja logičku vrednost: za Puasonovu kumulativnu funkciju raspodele koristiti TRUE, a za Puasonovu funkciju raspodele koristiti FALSE"
			}
		]
	},
	{
		name: "POISSON.DIST",
		description: "Daje Puasonovu raspodelu.",
		arguments: [
			{
				name: "x",
				description: "predstavlja broj događaja"
			},
			{
				name: "prosek",
				description: "predstavlja očekivanu numeričku vrednost, pozitivan broj"
			},
			{
				name: "kumulativno",
				description: "predstavlja logičku vrednost: za Puasonovu kumulativnu verovatnoću koristiti TRUE, a za Puasonovu funkciju raspodele koristiti FALSE"
			}
		]
	},
	{
		name: "POWER",
		description: "Predstavlja rezultat stepenovanja broja izložiocem.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja bazu, bilo koji realni broj"
			},
			{
				name: "stepen",
				description: "predstavlja izložilac, stepen na koji se podiže baza"
			}
		]
	},
	{
		name: "PPMT",
		description: "Daje iznos rate na osnovu početnog kapitala date investicije, zasnovane na periodičnim nepromenljivim otplatama i nepromenljivoj kamatnoj stopi.",
		arguments: [
			{
				name: "stopa",
				description: "predstavlja kamatnu stopu po periodu. Na primer, upotrebite 6%/4 za kvartalne rate, uz godišnju kamatu od 6%"
			},
			{
				name: "po",
				description: "navodi period i mora biti u opsegu od 1 do br_per"
			},
			{
				name: "br_per",
				description: "predstavlja ukupan broj rata za neku investiciju"
			},
			{
				name: "sad_vr",
				description: "predstavlja sadašnju vrednost tj. ukupnu trenutnu vrednost niza budućih rata"
			},
			{
				name: "bud_vr",
				description: "predstavlja buduću vrednost ili gotovinski saldo koji želite da postignete nakon poslednje rate"
			},
			{
				name: "tip",
				description: "predstavlja logičku vrednost: za ratu na početku perioda ima vrednost 1; za ratu na kraju perioda ima vrednost 0 ili je izostavljen"
			}
		]
	},
	{
		name: "PRICEDISC",
		description: "Daje cenu na 100 DIN nominalne vrednosti za pokriće sa popustom.",
		arguments: [
			{
				name: "poravnanje",
				description: "je datum obračunavanja pokrića, izražen kao serijski broj datuma"
			},
			{
				name: "zrelost",
				description: "je datum dospevanja pokrića, izražen kao serijski broj datuma"
			},
			{
				name: "popust",
				description: "je stopa popusta za pokriće"
			},
			{
				name: "povraćaj",
				description: "je otkupna vrednost pokrića na 100 Din nominalne vrednosti"
			},
			{
				name: "osnova",
				description: "je tip osnovice za brojanje dana koji bi trebalo koristiti"
			}
		]
	},
	{
		name: "PROB",
		description: "Daje verovatnoću da se vrednosti iz opsega nalaze između dve granične vrednosti ili su jednake donjoj granici.",
		arguments: [
			{
				name: "x_opseg",
				description: "predstavlja opseg numeričkih vrednosti za x na koje se verovatnoća odnosi"
			},
			{
				name: "opseg_verovatnoće",
				description: "predstavlja skup verovatnoća koje se odnose na vrednosti iz X_opsega, vrednosti između 0 i 1, uključujući 0"
			},
			{
				name: "donja_granica",
				description: "predstavlja donju granicu vrednosti čiju verovatnoću želite da ustanovite"
			},
			{
				name: "gornja_granica",
				description: "predstavlja opcionalnu gornju granicu vrednosti. Ako se izostavi, PROB daje verovatnoću da su vrednosti iz X_opsega jednake sa donjom_granicom"
			}
		]
	},
	{
		name: "PRODUCT",
		description: "Množi sve brojeve koji su zadati kao argumenti.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "predstavlja 1 do 255 brojeva, logičkih vrednosti ili tekstualnih reprezentacija brojeva koje želite da pomnožite"
			},
			{
				name: "broj2",
				description: "predstavlja 1 do 255 brojeva, logičkih vrednosti ili tekstualnih reprezentacija brojeva koje želite da pomnožite"
			}
		]
	},
	{
		name: "PROPER",
		description: "Pretvara slova tekstualne niske u normalna slova; prvo slovo svake reči veliko, a sva ostala mala.",
		arguments: [
			{
				name: "tekst",
				description: "predstavlja tekst između znaka navoda, formulu koja daje tekst ili referencu ćelije koja sadrži tekst u kojem bi početna slova reči trebalo pretvoriti u velika"
			}
		]
	},
	{
		name: "PV",
		description: "Daje sadašnju vrednost investicije:ukupan zbir trenutne vrednosti niza budućih rata.",
		arguments: [
			{
				name: "stopa",
				description: "predstavlja kamatnu stopu po periodu. Na primer, upotrebite 6%/4 za kvartalne rate, uz godišnju kamatu od 6%"
			},
			{
				name: "br_per",
				description: "predstavlja ukupan broj rata za investiciju"
			},
			{
				name: "rata",
				description: "predstavlja iznos rate za svaki period i ne može se menjati tokom trajanja investicije"
			},
			{
				name: "bud_vr",
				description: "predstavlja buduću vrednost ili gotovinski saldo koji želite da postignete nakon poslednje rate"
			},
			{
				name: "tip",
				description: "predstavlja logičku vrednost: za ratu na početku perioda ima vrednost 1; za ratu na kraju perioda ima vrednost 0 ili je izostavljen"
			}
		]
	},
	{
		name: "QUARTILE",
		description: "Daje kvartil skupa podataka.",
		arguments: [
			{
				name: "niz",
				description: "predstavlja niz ili opseg ćelija sa numeričkim vrednostima čiji kvartil želite da ustanovite"
			},
			{
				name: "kvart",
				description: "predstavlja broj: minimalna vrednost = 0; 1. kvartil = 1; medijana = 2; 3. kvartil = 3; maksimalna vrednost = 4"
			}
		]
	},
	{
		name: "QUARTILE.EXC",
		description: "Daje kvartil skupa podataka na osnovu vrednosti percentila od 0..1, isključujući te brojeve.",
		arguments: [
			{
				name: "niz",
				description: "predstavlja niz ili opseg ćelija sa numeričkim vrednostima čiju vrednost kvartila želite da ustanovite"
			},
			{
				name: "kvart",
				description: "predstavlja broj: minimalna vrednost = 0; 1. kvartil = 1; vrednost medijane = 2; 3. kvartil = 3; maksimalna vrednost = 4"
			}
		]
	},
	{
		name: "QUARTILE.INC",
		description: "Daje kvartil skupa podataka na osnovu vrednosti percentila od 0..1, uključujući te brojeve.",
		arguments: [
			{
				name: "niz",
				description: "predstavlja niz ili opseg ćelija sa numeričkim vrednostima čiju vrednost kvartila želite da ustanovite"
			},
			{
				name: "kvart",
				description: "predstavlja broj: minimalna vrednost = 0; 1. kvartil = 1; vrednost medijane = 2; 3. kvartil = 3; maksimalna vrednost = 4"
			}
		]
	},
	{
		name: "QUOTIENT",
		description: "Daje celobrojni ostatak deljenja.",
		arguments: [
			{
				name: "brojilac",
				description: "je deljenik"
			},
			{
				name: "imenilac",
				description: "je delilac"
			}
		]
	},
	{
		name: "RADIANS",
		description: "Pretvara stepene u radijane.",
		arguments: [
			{
				name: "ugao",
				description: "predstavlja ugao u stepenima koji želite da pretvorite"
			}
		]
	},
	{
		name: "RAND",
		description: "Daje slučajno generisan broj, veći ili jednak 0 i manji od 1, sa uniformnom raspodelom (menja se pri novom računanju).",
		arguments: [
		]
	},
	{
		name: "RANDBETWEEN",
		description: "Daje proizvoljan broj između brojeva koje navedete.",
		arguments: [
			{
				name: "dno",
				description: "je najmanji ceo broj koji će dati funkcija RANDBETWEEN"
			},
			{
				name: "vrh",
				description: "je najveći ceo broj koji će dati funkcija RANDBETWEEN"
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
		description: "Daje rang broja na listi brojeva: njegova veličina u odnosu na druge vrednosti na listi; ako više vrednosti ima isti rang, dobija se prosečni rang.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja broj za koji želite da pronađete rang"
			},
			{
				name: "ref",
				description: "predstavlja niz ili referencu za listu brojeva. Nenumeričke vrednosti se zanemaruju"
			},
			{
				name: "redosled",
				description: "predstavlja broj: rang na listi sortiranoj opadajućim redosledom = 0 ili izostavljeno; rang na listi sortiranoj rastućim redosledom= bilo koja vrednost različita od nule"
			}
		]
	},
	{
		name: "RANK.AVG",
		description: "Daje rang broja na listi brojeva: njegova veličina u odnosu na druge vrednosti na listi; ako više vrednosti ima isti rang, dobija se prosečni rang.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja broj za koji želite da pronađete rang"
			},
			{
				name: "ref",
				description: "predstavlja niz ili referencu za listu brojeva. Nenumeričke vrednosti se zanemaruju"
			},
			{
				name: "redosled",
				description: "predstavlja broj: rang na listi sortiranoj opadajućim redosledom = 0 ili izostavljeno; rang na listi sortiranoj rastućim redosledom= bilo koja vrednost različita od nule"
			}
		]
	},
	{
		name: "RANK.EQ",
		description: "Daje rang broja na listi brojeva: njegova veličina u odnosu na druge vrednosti na listi; ako više vrednosti ima isti rang, dobija se najveći rang tog skupa vrednosti.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja broj za koji želite da pronađete rang"
			},
			{
				name: "ref",
				description: "predstavlja niz ili referencu za listu brojeva. Nenumeričke vrednosti se zanemaruju"
			},
			{
				name: "redosled",
				description: "predstavlja broj: rang na listi sortiranoj opadajućim redosledom = 0 ili izostavljeno; rang na listi sortiranoj rastućim redosledom= bilo koja vrednost različita od nule"
			}
		]
	},
	{
		name: "RATE",
		description: "Daje kamatnu stopu po periodu zajma ili investicije. Na primer, upotrebite 6%/4 za kvartalne rate, uz godišnju kamatu od 6%.",
		arguments: [
			{
				name: "br_per",
				description: "predstavlja ukupan broj rata za otplatu zajma ili investicije"
			},
			{
				name: "rata",
				description: "predstavlja iznos rate za svaki period i ne može se menjati tokom trajanja kredita ili investicije"
			},
			{
				name: "sad_vr",
				description: "predstavlja sadašnju vrednost tj. ukupnu trenutnu vrednost niza budućih rata"
			},
			{
				name: "bud_vr",
				description: "predstavlja buduću vrednost ili gotovinski saldo koji želite da postignete nakon poslednje rate. Ako je izostavljen, za bud_vr se koristi vrednost 0"
			},
			{
				name: "tip",
				description: "predstavlja logičku vrednost: za ratu na početku perioda ima vrednost 1; za ratu na kraju perioda ima vrednost 0 ili je izostavljen"
			},
			{
				name: "procena",
				description: "predstavlja vašu pretpostavku iznosa stope; ako je izostavljen, procena iznosi 0,1 (10 procenata)"
			}
		]
	},
	{
		name: "RECEIVED",
		description: "Daje svotu koja se dobija po dospeću za potpuno investirano pokriće.",
		arguments: [
			{
				name: "poravnanje",
				description: "je datum obračunavanja pokrića, izražen kao serijski broj datuma"
			},
			{
				name: "zrelost",
				description: "je datum dospevanja pokrića, izražen kao serijski broj datuma"
			},
			{
				name: "ulaganje",
				description: "je svota koja je investirana u pokriće"
			},
			{
				name: "popust",
				description: "je stopa popusta za pokriće"
			},
			{
				name: "osnova",
				description: "je tip osnovice za brojanje dana koji bi trebalo koristiti"
			}
		]
	},
	{
		name: "REPLACE",
		description: "Zamenjuje deo tekstualne niske drugom tekstualnom niskom.",
		arguments: [
			{
				name: "stari_tekst",
				description: "predstavlja tekst u kojem želite da zamenite neke od znakova"
			},
			{
				name: "poč_br",
				description: "predstavlja položaj znakova u starom_tekstu koje želite da zamenite znakovima iz novog_teksta"
			},
			{
				name: "br_znakova",
				description: "predstavlja broj znakova u starom_tekstu koje želite da zamenite"
			},
			{
				name: "novi_tekst",
				description: "predstavlja tekst koji će zameniti znake u starom_tekstu"
			}
		]
	},
	{
		name: "REPT",
		description: "Ponavlja tekst utvrđeni broj puta. Koristite REPT da biste ispunili ćeliju određenim brojem instanci tekstualne niske.",
		arguments: [
			{
				name: "tekst",
				description: "predstavlja tekst koji treba da se ponavlja"
			},
			{
				name: "broj_puta",
				description: "predstavlja pozitivni broj koji određuje koliko se puta tekst ponavlja"
			}
		]
	},
	{
		name: "RIGHT",
		description: "Daje određeni broj znakova s kraja tekstualne niske.",
		arguments: [
			{
				name: "tekst",
				description: "predstavlja tekstualnu nisku iz koje želite da izdvojite znake"
			},
			{
				name: "broj_znakova",
				description: "navodi koliko znakova želite da bude izdvojeno; 1 ako je izostavljen"
			}
		]
	},
	{
		name: "ROMAN",
		description: "Pretvara arapski broj u rimski, kao tekst.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja arapski broj koji želite da pretvorite"
			},
			{
				name: "oblik",
				description: "predstavlja broj koji navodi koji tip rimskih brojeva želite."
			}
		]
	},
	{
		name: "ROUND",
		description: "Zaokružuje broj na određeni broj cifara.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja broj koji želite da zaokružite"
			},
			{
				name: "broj_cifara",
				description: "predstavlja broj cifara na koji želite da zaokružite dati broj. Ako je argument negativan, vrši se zaokruživanje s leve strane znaka za razdvajanje decimala; ako je nula ili izostavljen, zaokružuje se na najbliži ceo broj"
			}
		]
	},
	{
		name: "ROUNDDOWN",
		description: "Zaokružuje broj naniže, prema nuli.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja bilo koji realan broj koji želite da zaokružite naniže"
			},
			{
				name: "br_cifara",
				description: "predstavlja broj cifara zaokruženog broja. Ako je argument negativan, vrši se zaokruživanje s leve strane znaka za razdvajanje decimala; ako je nula ili izostavljen, zaokružuje se na najbliži ceo broj"
			}
		]
	},
	{
		name: "ROUNDUP",
		description: "Zaokružuje broj naviše, u suprotnom smeru od nule.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja bilo koji realan broj koji želite da zaokružite naviše"
			},
			{
				name: "br_cifara",
				description: "predstavlja broj cifara zaokruženog broja. Ako je argument negativan, vrši se zaokruživanje s leve strane znaka za razdvajanje decimala; ako je nula ili izostavljen, zaokružuje se na najbliži ceo broj"
			}
		]
	},
	{
		name: "ROW",
		description: "Daje broj redova reference.",
		arguments: [
			{
				name: "referenca",
				description: "predstavlja ćeliju ili opseg ćelija za koje želite da ustanovite broj redova; Daje ćeliju koja sadrži funkciju ROW ako je ćelija izostavljena"
			}
		]
	},
	{
		name: "ROWS",
		description: "Daje broj redova u referenci ili u tabeli.",
		arguments: [
			{
				name: "niz",
				description: "predstavlja tabelu, formulu tabele ili referencu na opseg ćelija u kome želite da odredite broj redova"
			}
		]
	},
	{
		name: "RRI",
		description: "Daje ekvivalentnu kamatnu stopu za rast investicije.",
		arguments: [
			{
				name: "nper",
				description: "je broj perioda za investiciju"
			},
			{
				name: "pv",
				description: "je trenutna vrednost investicije"
			},
			{
				name: "fv",
				description: "je buduća vrednost investicije"
			}
		]
	},
	{
		name: "RSQ",
		description: "Daje kvadrat Pirsonovog koeficijenta korelacije (proizvoda momenata), na osnovu datih tačaka podataka.",
		arguments: [
			{
				name: "poznati_y",
				description: "predstavlja neki niz ili opseg tačaka podataka i može biti dat kao brojevi ili imena, nizovi ili reference koje sadrže brojeve"
			},
			{
				name: "poznati_x",
				description: "predstavlja neki niz ili opseg tačaka podataka i može biti dat kao brojevi ili imena, nizovi ili reference koje sadrže brojeve"
			}
		]
	},
	{
		name: "RTD",
		description: "Preuzima podatke u realnom vremenu iz programa koji podržava COM automatizaciju.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "progID",
				description: "predstavlja ime ProgID-a, registrovanog programskog dodatka za COM automatizaciju. Stavite ime među znake navoda"
			},
			{
				name: "server",
				description: "predstavlja ime servera na kome treba pokrenuti programski dodatak. Stavite ime među znake navoda. Ako se programski dodatak pokreće na lokalnoj mašini, upotrebite praznu nisku"
			},
			{
				name: "tema1",
				description: "predstavlja 1 do 28 parametara koji navode deo podataka"
			},
			{
				name: "tema2",
				description: "predstavlja 1 do 28 parametara koji navode deo podataka"
			}
		]
	},
	{
		name: "SEARCH",
		description: "Daje broj znaka kod koga je prvi put pronađen određeni znak ili tekstualna niska, čitajući sleva nadesno (bez razlikovanja malih i velikih slova).",
		arguments: [
			{
				name: "pronalaženje_teksta",
				description: "predstavlja tekst koji želite da pronađete. možete da koristite džoker znake ? i * ; koristite ~? i ~* da biste pronašli znake ? i *"
			},
			{
				name: "u_okviru_teksta",
				description: "predstavlja tekst u kojem želite da tražite parametar pronalaženje_teksta"
			},
			{
				name: "početni_broj",
				description: "predstavlja broj znaka u parametru u_okviru_teksta, brojeći sleva, od kojeg želite da započnete pretragu. Ako je izostavljen, upotrebljava se 1"
			}
		]
	},
	{
		name: "SEC",
		description: "Vraća sekans ugla.",
		arguments: [
			{
				name: "number",
				description: "je ugao u radijanima za koji želite sekans"
			}
		]
	},
	{
		name: "SECH",
		description: "Vraća hiperbolički sekans ugla.",
		arguments: [
			{
				name: "number",
				description: "je ugao u radijanima za koji želite hiperbolički sekans"
			}
		]
	},
	{
		name: "SECOND",
		description: "Vraća sekundu, broj od 0 do 59.",
		arguments: [
			{
				name: "serijski_broj",
				description: "predstavlja broj u Spreadsheet kodu za datum-vreme ili tekst u formatu vremena, kao što je 16:48:23 ili 4:48:47 PM"
			}
		]
	},
	{
		name: "SERIESSUM",
		description: "Daje zbir potencijalnog reda na osnovu formule.",
		arguments: [
			{
				name: "x",
				description: "je ulazna vrednost potencijalnog reda"
			},
			{
				name: "n",
				description: "je početni stepen na koji želite da dignete x"
			},
			{
				name: "m",
				description: "je korak za koji bi trebalo uvećati n za svaki izraz u redu"
			},
			{
				name: "koeficijenti",
				description: "je skup koeficijenata kojima se množi svaki sledeći stepen od x"
			}
		]
	},
	{
		name: "SHEET",
		description: "Daje broj lista od lista koji je referenciran.",
		arguments: [
			{
				name: "value",
				description: "je ime lista ili reference za koju želite da dobijete broj.  Ako se izostavi, broj lista koji sadrži funkciju se dobija"
			}
		]
	},
	{
		name: "SHEETS",
		description: "Daje broj listova u referenci.",
		arguments: [
			{
				name: "reference",
				description: "je referenca za koju želite da dobijete broj listova koje sadrži.  Ako se izostavi, broj listova u radnoj svesci koji sadrže funkciju se dobija"
			}
		]
	},
	{
		name: "SIGN",
		description: "Daje signum broja: 1 ako je broj pozitivan, nulu ako je broj jednak nuli ili -1 ako je broj negativan.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja bilo koji realni broj"
			}
		]
	},
	{
		name: "SIN",
		description: "Daje sinus ugla.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja ugao čija je vrednost data u radijanima i čiji sinus želite. Stepeni * PI()/180 = radijani"
			}
		]
	},
	{
		name: "SINH",
		description: "Daje sinus hiperbolički broja.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja bilo koji realan broj"
			}
		]
	},
	{
		name: "SKEW",
		description: "Daje asimetriju neke raspodele: opis stepena asimetričnosti raspodele u blizini srednje vrednosti.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "predstavlja 1 do 255 brojeva ili imena, nizova ili referenci koji sadrže brojeve čiju meru asimetričnosti tražite"
			},
			{
				name: "broj2",
				description: "predstavlja 1 do 255 brojeva ili imena, nizova ili referenci koji sadrže brojeve čiju meru asimetričnosti tražite"
			}
		]
	},
	{
		name: "SKEW.P",
		description: "Daje asimetriju neke raspodele na osnovu populacije: opis stepena asimetričnosti raspodele u blizini srednje vrednosti.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "predstavlja 1 do 255 brojeva ili imena, nizova ili referenci koji sadrže brojeve čiju meru asimetričnosti populacije tražite"
			},
			{
				name: "number2",
				description: "predstavlja 1 do 255 brojeva ili imena, nizova ili referenci koji sadrže brojeve čiju meru asimetričnosti populacije tražite"
			}
		]
	},
	{
		name: "SLN",
		description: "Daje linearnu amortizaciju sredstva za jedan period.",
		arguments: [
			{
				name: "cena",
				description: "predstavlja početnu cenu sredstva"
			},
			{
				name: "rashod",
				description: "predstavlja amortizovanu vrednost sredstva po isteku perioda njegove upotrebe"
			},
			{
				name: "vek",
				description: "predstavlja broj perioda tokom kojih se vrši amortizacija sredstva (ponekad se naziva korisni vek sredstva)"
			}
		]
	},
	{
		name: "SLOPE",
		description: "Daje nagib linije linearne regresije koja prolazi kroz date tačke podataka.",
		arguments: [
			{
				name: "poznati_y",
				description: "predstavlja neki niz ili opseg ćelija sa numeričkim, zavisnim tačkama podataka i može biti dat kao brojevi ili imena, nizovi ili reference koje sadrže brojeve"
			},
			{
				name: "poznati_x",
				description: "predstavlja skup nezavisnih tačaka podataka i može biti dat kao brojevi ili imena, nizovi ili reference koje sadrže brojeve"
			}
		]
	},
	{
		name: "SMALL",
		description: "Daje k-tu po veličini (počev od najmanje) vrednost skupa podataka. Na primer, peti broj po veličini.",
		arguments: [
			{
				name: "niz",
				description: "predstavlja niz ili opseg numeričkih vrednosti čiju k-tu po veličini vrednost želite da odredite"
			},
			{
				name: "k",
				description: "predstavlja položaj (počev od najmanjeg) u nizu i ili opsegu ćelija vrednosti koja bi trebalo da bude data"
			}
		]
	},
	{
		name: "SQRT",
		description: "Daje kvadratni koren broja.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja broj čiji kvadratni koren želite"
			}
		]
	},
	{
		name: "SQRTPI",
		description: "Daje kvadratni koren od (broj * Pi).",
		arguments: [
			{
				name: "broj",
				description: "je broj kojim se množi p"
			}
		]
	},
	{
		name: "STANDARDIZE",
		description: "Daje normalizovanu vrednost iz raspodele koja kao parametre ima srednju vrednost i standardnu devijaciju.",
		arguments: [
			{
				name: "x",
				description: "predstavlja vrednost koju želite da normalizujete"
			},
			{
				name: "prosek",
				description: "predstavlja aritmetičku srednju vrednost raspodele"
			},
			{
				name: "standardna_dev",
				description: "predstavlja standardnu devijaciju raspodele, neki pozitivan broj"
			}
		]
	},
	{
		name: "STDEV",
		description: "Procenjuje standardnu devijaciju na osnovu uzorka (zanemaruje logičke vrednosti i tekst u uzorku).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "predstavlja brojeve od 1 do 255 koji odgovaraju uzorku populacije i mogu biti brojevi ili reference koje sadrže brojeve"
			},
			{
				name: "broj2",
				description: "predstavlja brojeve od 1 do 255 koji odgovaraju uzorku populacije i mogu biti brojevi ili reference koje sadrže brojeve"
			}
		]
	},
	{
		name: "STDEV.P",
		description: "Izračunava standardnu devijaciju na osnovu ukupne populacije koja je data u vidu argumenata (zanemaruje logičke vrednosti i tekst).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "predstavlja 1 do 255 brojeva koji odgovaraju populaciji i mogu biti brojevi ili reference koje sadrže brojeve"
			},
			{
				name: "broj2",
				description: "predstavlja 1 do 255 brojeva koji odgovaraju populaciji i mogu biti brojevi ili reference koje sadrže brojeve"
			}
		]
	},
	{
		name: "STDEV.S",
		description: "Procenjuje standardnu devijaciju na osnovu uzorka (zanemaruje logičke vrednosti i tekst u uzorku).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "predstavlja 1 do 255 brojeva koji odgovaraju uzorku populacije i mogu biti brojevi ili reference koje sadrže brojeve"
			},
			{
				name: "broj2",
				description: "predstavlja 1 do 255 brojeva koji odgovaraju uzorku populacije i mogu biti brojevi ili reference koje sadrže brojeve"
			}
		]
	},
	{
		name: "STDEVA",
		description: "Izračunava standardnu devijaciju na osnovu uzorka, uključujući logičke vrednosti i tekst. Tekst i logička vrednost FALSE imaju vrednost 0; logička vrednost TRUE ima vrednost 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrednost1",
				description: "predstavlja 1 do 255 vrednosti koje odgovaraju uzorku populacije i mogu biti vrednosti, imena ili reference vrednosti"
			},
			{
				name: "vrednost2",
				description: "predstavlja 1 do 255 vrednosti koje odgovaraju uzorku populacije i mogu biti vrednosti, imena ili reference vrednosti"
			}
		]
	},
	{
		name: "STDEVP",
		description: "Izračunava standardnu devijaciju na osnovu ukupne populacije izražene u argumentima (zanemaruje logičke vrednosti i tekst).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "predstavlja brojeve od 1 do 255 koji odgovaraju populaciji i mogu biti brojevi ili reference koje sadrže brojeve"
			},
			{
				name: "broj2",
				description: "predstavlja brojeve od 1 do 255 koji odgovaraju populaciji i mogu biti brojevi ili reference koje sadrže brojeve"
			}
		]
	},
	{
		name: "STDEVPA",
		description: "Izračunava standardnu devijaciju za čitavu populaciju, uključujući logičke vrednosti i tekst. Tekst i logička vrednost FALSE imaju vrednost 0; logička vrednost TRUE ima vrednost 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrednost1",
				description: "predstavlja 1 do 255 vrednosti koje predstavljaju populaciju i mogu biti vrednosti, imena, nizovi ili reference koje sadrže vrednosti"
			},
			{
				name: "vrednost2",
				description: "predstavlja 1 do 255 vrednosti koje predstavljaju populaciju i mogu biti vrednosti, imena, nizovi ili reference koje sadrže vrednosti"
			}
		]
	},
	{
		name: "STEYX",
		description: "Daje standardnu grešku regresije predviđene y-vrednosti za svako x.",
		arguments: [
			{
				name: "poznati_y",
				description: "predstavlja niz ili opseg zavisnih tačaka podataka i može biti dat kao brojevi ili imena, nizovi ili reference koje sadrže brojeve"
			},
			{
				name: "poznati_x",
				description: "predstavlja niz ili opseg nezavisnih tačaka podataka i može biti dat kao brojevi ili imena, nizovi ili reference koje sadrže brojeve"
			}
		]
	},
	{
		name: "SUBSTITUTE",
		description: "Zamenjuje postojeći tekst tekstualne niske novim tekstom.",
		arguments: [
			{
				name: "tekst",
				description: "predstavlja tekst ili referencu ćelije koja sadrži tekst čije znake želite da zamenite"
			},
			{
				name: "stari_tekst",
				description: "predstavlja postojeći tekst koji želite da zamenite. Ako se veličina slova za stari_tekst ne podudara sa veličinom slova teksta, funkcija SUBSTITUTE neće zameniti tekst"
			},
			{
				name: "novi_tekst",
				description: "predstavlja tekst kojim želite da zamenite stari_tekst"
			},
			{
				name: "broj_instance",
				description: "navodi mesta na kojima želite da zamenite stari_tekst. Ako je izostavljen, stari_tekst će biti zamenjen svuda gde se pojavljuje"
			}
		]
	},
	{
		name: "SUBTOTAL",
		description: "Daje međuvrednost neke liste ili baze podataka.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "br_funkcije",
				description: "predstavlja broj od 1 do 11 koji navodi funkciju koju treba koristiti pri izračunavanju međuvrednosti."
			},
			{
				name: "ref1",
				description: "predstavlja 1 do 254 opsega ili referenci za koje želite da izračunate međuvrednost"
			}
		]
	},
	{
		name: "SUM",
		description: "Sabira sve brojeve u opsegu ćelija.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "predstavlja 1 do 255 brojeva koji se sabiraju. Logičke vrednosti i tekst u ćelijama se zanemaruju čak i kada su otkucani kao argumenti"
			},
			{
				name: "broj2",
				description: "predstavlja 1 do 255 brojeva koji se sabiraju. Logičke vrednosti i tekst u ćelijama se zanemaruju čak i kada su otkucani kao argumenti"
			}
		]
	},
	{
		name: "SUMIF",
		description: "Dodaje ćelije navedene datim uslovom ili kriterijumom.",
		arguments: [
			{
				name: "opseg",
				description: "predstavlja opseg ćelija koje želite da procenite"
			},
			{
				name: "kriterijumi",
				description: "predstavlja uslov ili kriterijum u obliku broja, izraza ili teksta koji navode koje ćelije će biti sabrane"
			},
			{
				name: "opseg_zbira",
				description: "predstavlja ćelije koje bi trebalo sabrati. Ako je izostavljen, koriste se ćelije iz opsega"
			}
		]
	},
	{
		name: "SUMIFS",
		description: "Dodaje ćelije koje navodi dati skup uslova ili kriterijuma.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "ukupni_opseg",
				description: "su oktalne ćelije koje se sabiraju."
			},
			{
				name: "opseg_kriterijuma",
				description: "je opseg ćelija koje želite da budu procenjene po određenom uslovu"
			},
			{
				name: "kriterijumi",
				description: "je uslov u obliku broja, izraza ili teksta koji definiše koje će se ćelije brojati"
			}
		]
	},
	{
		name: "SUMPRODUCT",
		description: "Daje zbir proizvoda odgovarajućih opsega ili nizova.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "niz1",
				description: "predstavlja 2 do 255 nizova čije komponente želite najpre da pomnožite, a zatim i da ih saberete. Svi nizovi moraju imati iste dimenzije"
			},
			{
				name: "niz2",
				description: "predstavlja 2 do 255 nizova čije komponente želite najpre da pomnožite, a zatim i da ih saberete. Svi nizovi moraju imati iste dimenzije"
			},
			{
				name: "niz3",
				description: "predstavlja 2 do 255 nizova čije komponente želite najpre da pomnožite, a zatim i da ih saberete. Svi nizovi moraju imati iste dimenzije"
			}
		]
	},
	{
		name: "SUMSQ",
		description: "Daje zbir kvadrata argumenata. Argumenti mogu biti brojevi, nizovi, imena ili reference na ćelije koje sadrže brojeve.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "predstavlja 1 do 255 brojeva, nizova, imena ili referenci na nizove čiji zbir kvadrata želite"
			},
			{
				name: "broj2",
				description: "predstavlja 1 do 255 brojeva, nizova, imena ili referenci na nizove čiji zbir kvadrata želite"
			}
		]
	},
	{
		name: "SUMX2MY2",
		description: "Sabira razlike kvadrata dva odgovarajuća opsega ili niza.",
		arguments: [
			{
				name: "niz_x",
				description: "predstavlja prvi opseg ili niz brojeva i može biti broj ili ime, niz ili referenca koja sadrži brojeve"
			},
			{
				name: "niz_y",
				description: "predstavlja drugi opseg ili niz vrednosti i može biti broj ili ime, niz ili referenca koja sadrži brojeve"
			}
		]
	},
	{
		name: "SUMX2PY2",
		description: "Daje sumu zbirova kvadrata brojeva u dva odgovarajuća opsega ili niza.",
		arguments: [
			{
				name: "niz_x",
				description: "predstavlja prvi opseg ili niz brojeva i može biti broj ili ime, niz ili referenca koja sadrži brojeve"
			},
			{
				name: "niz_y",
				description: "predstavlja drugi opseg ili niz brojeva i može biti broj ili ime, niz ili referenca koja sadrži brojeve"
			}
		]
	},
	{
		name: "SUMXMY2",
		description: "Sabira kvadrate razlika dva odgovarajuća opsega ili niza.",
		arguments: [
			{
				name: "niz_x",
				description: "predstavlja prvi opseg ili niz vrednosti i može biti broj ili ime, niz ili referenca koja sadrži brojeve"
			},
			{
				name: "niz_y",
				description: "predstavlja drugi opseg ili niz vrednosti i može biti broj ili ime, niz ili referenca koja sadrži brojeve"
			}
		]
	},
	{
		name: "SYD",
		description: "Daje aritmetičko-nazadujuću amortizaciju sredstva za navedeni period.",
		arguments: [
			{
				name: "cena",
				description: "predstavlja početnu cenu sredstva"
			},
			{
				name: "rashod",
				description: "predstavlja amortizovanu vrednost sredstva po isteku perioda njegove upotrebe"
			},
			{
				name: "vek",
				description: "predstavlja broj perioda tokom kojih se vrši amortizacija sredstva (ponekad se naziva korisni vek sredstva)"
			},
			{
				name: "po",
				description: "predstavlja period i mora da koristi iste jedinice mere kao i argument vek"
			}
		]
	},
	{
		name: "T",
		description: "Proverava da li je vrednost tekst i ukoliko jeste, daje tekst; u suprotnom daje dvostruke navodnike (prazan tekst).",
		arguments: [
			{
				name: "vrednost",
				description: "predstavlja vrednost koju treba proveriti"
			}
		]
	},
	{
		name: "T.DIST",
		description: "Daje levu Studentovu t-raspodelu.",
		arguments: [
			{
				name: "x",
				description: "predstavlja numeričku vrednost za koju se izračunava vrednost raspodele"
			},
			{
				name: "step_slobode",
				description: "je ceo broj koji predstavlja stepen slobode koji karakteriše raspodelu"
			},
			{
				name: "kumulativno",
				description: "predstavlja logičku vrednost: za funkciju kumulativne raspodele koristite TRUE; za funkciju gustine verovatnoće koristite FALSE"
			}
		]
	},
	{
		name: "T.DIST.2T",
		description: "Daje dvostranu Studentovu t-raspodelu.",
		arguments: [
			{
				name: "x",
				description: "predstavlja numeričku vrednost za koju se izračunava vrednost raspodele"
			},
			{
				name: "step_slobode",
				description: "je ceo broj koji predstavlja stepen slobode koji karakteriše raspodelu"
			}
		]
	},
	{
		name: "T.DIST.RT",
		description: "Daje desnu Studentovu t-raspodelu.",
		arguments: [
			{
				name: "x",
				description: "predstavlja numeričku vrednost za koju se izračunava vrednost raspodele"
			},
			{
				name: "step_slobode",
				description: "je ceo broj koji predstavlja stepen slobode koji karakteriše raspodelu"
			}
		]
	},
	{
		name: "T.INV",
		description: "Daje levu inverznu Studentovu t-raspodelu.",
		arguments: [
			{
				name: "verovatnoća",
				description: "predstavlja verovatnoću pridruženu dvostranoj Studentovoj t-raspodeli, broj od 0 do 1, uključujući te brojeve"
			},
			{
				name: "step_slobode",
				description: "predstavlja pozitivan ceo broj koji pokazuje stepen slobode koji opisuje raspodelu"
			}
		]
	},
	{
		name: "T.INV.2T",
		description: "Daje dvostranu inverznu Studentovu t-raspodelu.",
		arguments: [
			{
				name: "verovatnoća",
				description: "predstavlja verovatnoću pridruženu dvostranoj Studentovoj t-raspodeli, broj od 0 do 1, uključujući te brojeve"
			},
			{
				name: "step_slobode",
				description: "predstavlja pozitivan ceo broj koji pokazuje stepen slobode koji opisuje raspodelu"
			}
		]
	},
	{
		name: "T.TEST",
		description: "Daje verovatnoću koja je povezana sa Studentovim t-testom.",
		arguments: [
			{
				name: "niz1",
				description: "predstavlja prvi skup podataka"
			},
			{
				name: "niz2",
				description: "predstavlja drugi skup podataka"
			},
			{
				name: "praćenja",
				description: "navodi broj strana raspodele koji bi trebalo da bude dat: jednostrana raspodela = 1; dvostrana raspodela = 2"
			},
			{
				name: "tip",
				description: "predstavlja vrstu t-testa: test parova podataka = 1, dva uzorka sa istim odstupanjima (homoskedastični) = 2, dva uzorka sa različitim odstupanjima = 3"
			}
		]
	},
	{
		name: "TAN",
		description: "Daje tangens ugla.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja ugao čija je vrednost data u radijanima i čiji tangens želite. Stepeni * PI()/180 = radijani"
			}
		]
	},
	{
		name: "TANH",
		description: "Daje tangens hiperbolički broja.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja bilo koji realan broj"
			}
		]
	},
	{
		name: "TBILLEQ",
		description: "Daje dobit koja je jednaka sa vrednošću državnih obveznica.",
		arguments: [
			{
				name: "poravnanje",
				description: "je datum obračunavanja državnih obveznica, izražen kao serijski broj datuma"
			},
			{
				name: "zrelost",
				description: "je datum dospevanja državnih obveznica, izražen kao serijski broj datuma"
			},
			{
				name: "popust",
				description: "je stopa popusta za državne obveznice"
			}
		]
	},
	{
		name: "TBILLPRICE",
		description: "Daje cenu na 100 Din nominalne vrednosti za državne obveznice.",
		arguments: [
			{
				name: "poravnanje",
				description: "je datum obračunavanja državnih obveznica, izražen kao serijski broj datuma"
			},
			{
				name: "zrelost",
				description: "je datum dospeća državnih obveznica, izražen kao serijski broj datuma"
			},
			{
				name: "popust",
				description: "je stopa popusta za državne obveznice"
			}
		]
	},
	{
		name: "TBILLYIELD",
		description: "Daje dobit za državne obveznice.",
		arguments: [
			{
				name: "poravnanje",
				description: "je datum obračunavanja državnih obveznica, izražen kao serijski broj datuma"
			},
			{
				name: "zrelost",
				description: "je datum dospevanja državnih obveznica, izražen kao serijski broj datuma"
			},
			{
				name: "pr",
				description: "je cena državnih obveznica na 100 Din nominalne vrednosti"
			}
		]
	},
	{
		name: "TDIST",
		description: "Daje vrednost studentove t-raspodele.",
		arguments: [
			{
				name: "x",
				description: "predstavlja numeričku vrednost za koju se izračunava vrednost raspodele"
			},
			{
				name: "step_slobode",
				description: "je ceo broj koji predstavlja stepen slobode koji karakteriše raspodelu"
			},
			{
				name: "pokušaji",
				description: "navodi broj strana raspodele: jednostrana raspodela = 1; dvostrana raspodela = 2"
			}
		]
	},
	{
		name: "TEXT",
		description: "Pretvara neku vrednost u tekst u specifičnom numeričkom formatu.",
		arguments: [
			{
				name: "vrednost",
				description: "predstavlja broj, formulu koja daje numeričku vrednost ili referencu na ćeliju koja sadrži numeričku vrednost"
			},
			{
				name: "oblik_teksta",
				description: "predstavlja format broja u tekstualnom obliku u okviru „Kategorija“ koji se nalazi na kartici „Broj“ dijaloga „Oblikovanje ćelija“ (ne „Opšti format“)"
			}
		]
	},
	{
		name: "TIME",
		description: "Pretvara časove, minute i sekunde, date kao brojeve, u Spreadsheet redni broj, oblikovan pomoću formata za vreme.",
		arguments: [
			{
				name: "čas",
				description: "predstavlja broj od 0 do 23 koji reprezentuje časove"
			},
			{
				name: "minut",
				description: "predstavlja broj od 0 do 59, koji reprezentuje minute"
			},
			{
				name: "sekunda",
				description: "predstavlja broj od 0 do 59, koji reprezentuje sekunde"
			}
		]
	},
	{
		name: "TIMEVALUE",
		description: "Konvertuje tekstualno prikazano vreme u Spreadsheet redni broj za vreme, broj od 0 (12:00:00 AM) do 0,999988426 (11:59:59 PM). Oblikuje broj pomoću formata za vreme nakon unošenja formule.",
		arguments: [
			{
				name: "tekst_vremena",
				description: "predstavlja tekst koji prikazuje vreme u bilo kom Spreadsheet formatu vremena (informacije o datumu u nisci se zanemaruju)"
			}
		]
	},
	{
		name: "TINV",
		description: "Daje dvostranu inverznu studentovu t-raspodelu.",
		arguments: [
			{
				name: "verovatnoća",
				description: "predstavlja verovatnoću koja je povezana sa dvostranom studentovom t-raspodelom, broj između 0 i 1, uključujući i njih"
			},
			{
				name: "step_slobode",
				description: "predstavlja pozitivan ceo broj koji pokazuje stepen slobode koji opisuje raspodelu"
			}
		]
	},
	{
		name: "TODAY",
		description: "Daje trenutne podatke oblikovane kao datum.",
		arguments: [
		]
	},
	{
		name: "TRANSPOSE",
		description: "Pretvara vertikalni opseg ćelija u horizontalni ili obrnuto.",
		arguments: [
			{
				name: "niz",
				description: "predstavlja opseg ćelija na radnom listu ili niz vrednosti koje želite da transponujete"
			}
		]
	},
	{
		name: "TREND",
		description: "Daje brojeve za linearni trend koji odgovara poznatim podacima o tačkama, a koristi metod najmanjih kvadrata.",
		arguments: [
			{
				name: "poznati_y",
				description: "predstavlja opseg niza y-vrednosti koji vam je već poznat iz relacije y = mx + b"
			},
			{
				name: "poznati_x",
				description: "predstavlja opcionalni opseg niza x-vrednosti koji vam je poznat iz relacije y = mx + b, niz iste veličine kao i poznati_y"
			},
			{
				name: "novi_x",
				description: "predstavlja opseg niza novih x-vrednosti za koje želite da funkcija TREND dâ odgovarajuće y-vrednosti"
			},
			{
				name: "konstanta",
				description: "predstavlja logičku vrednost: konstanta b normalno se računa ako argument konstanta ima vrednost TRUE ili je izostavljen; b se postavlja na 0 ako je konstanta = FALSE"
			}
		]
	},
	{
		name: "TRIM",
		description: "Uklanja sve razmake u tekstu osim pojedinačnih razmaka između reči.",
		arguments: [
			{
				name: "tekst",
				description: "predstavlja tekst iz kojeg želite da uklonite razmake"
			}
		]
	},
	{
		name: "TRIMMEAN",
		description: "Daje srednju vrednost unutrašnjeg dela skupa podataka.",
		arguments: [
			{
				name: "niz",
				description: "predstavlja opseg ili niz podataka koji bi trebalo odseći i odrediti im prosek"
			},
			{
				name: "procenat",
				description: "predstavlja broj tačaka podataka izražen u procentima koje treba izuzeti od najviših i najnižih vrednosti iz skupa podataka"
			}
		]
	},
	{
		name: "TRUE",
		description: "Daje logičku vrednost TRUE.",
		arguments: [
		]
	},
	{
		name: "TRUNC",
		description: "Odseca broj i pretvara ga u ceo broj, uklanjajući iz njega decimale ili razlomak.",
		arguments: [
			{
				name: "broj",
				description: "predstavlja broj koji želite da odsečete"
			},
			{
				name: "br_cifara",
				description: "predstavlja broj koji daje tačnost odsecanja. Ako je izostavljen, koristi se nula"
			}
		]
	},
	{
		name: "TTEST",
		description: "Daje verovatnoću koja je povezana sa studentovim t-testom.",
		arguments: [
			{
				name: "niz1",
				description: "predstavlja prvi skup podataka"
			},
			{
				name: "niz2",
				description: "predstavlja drugi skup podataka"
			},
			{
				name: "praćenja",
				description: "navodi broj strana raspodele koji bi trebalo da bude dat: jednostrana raspodela = 1; dvostrana raspodela = 2"
			},
			{
				name: "tip",
				description: "predstavlja vrstu t-testa: test parova podataka = 1, dva uzorka sa istim odstupanjima (homoskedastični) = 2, dva uzorka sa različitim odstupanjima = 3"
			}
		]
	},
	{
		name: "TYPE",
		description: "Daje ceo broj koji predstavlja tip podataka neke vrednosti: broj = 1; tekst = 2; logička vrednost = 4; greška = 16; niz = 64.",
		arguments: [
			{
				name: "vrednost",
				description: "može biti bilo koja vrednost"
			}
		]
	},
	{
		name: "UNICODE",
		description: "Daje broj (tačku koda) koji odgovara prvom znaku teksta.",
		arguments: [
			{
				name: "text",
				description: "je znak za koji vam je potreban unikod vrednost"
			}
		]
	},
	{
		name: "UPPER",
		description: "Pretvara tekstualnu nisku u velika slova.",
		arguments: [
			{
				name: "tekst",
				description: "predstavlja tekst koji želite da pretvorite u velika slova, referencu ili tekstualnu nisku"
			}
		]
	},
	{
		name: "VALUE",
		description: "Pretvara tekstualnu nisku koja predstavlja broj u broj.",
		arguments: [
			{
				name: "tekst",
				description: "predstavlja tekst pod znacima navoda ili referencu ćelije koja sadrži tekst koji želite da pretvorite"
			}
		]
	},
	{
		name: "VAR",
		description: "Procenjuje odstupanje na osnovu uzorka (zanemaruje logičke vrednosti i tekst u uzorku).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "predstavlja numeričke argumente od 1 do 255 koji odgovaraju uzorku populacije"
			},
			{
				name: "broj2",
				description: "predstavlja numeričke argumente od 1 do 255 koji odgovaraju uzorku populacije"
			}
		]
	},
	{
		name: "VAR.P",
		description: "Izračunava odstupanje na osnovu ukupne populacije (zanemaruje logičke vrednosti i tekst u podacima o populaciji).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "predstavlja numeričke argumente od 1 do 255 koji odgovaraju populaciji"
			},
			{
				name: "broj2",
				description: "predstavlja numeričke argumente od 1 do 255 koji odgovaraju populaciji"
			}
		]
	},
	{
		name: "VAR.S",
		description: "Procenjuje odstupanje na osnovu uzorka (zanemaruje logičke vrednosti i tekst u uzorku).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "predstavlja numeričke argumente od 1 do 255 koji odgovaraju uzorku populacije"
			},
			{
				name: "broj2",
				description: "predstavlja numeričke argumente od 1 do 255 koji odgovaraju uzorku populacije"
			}
		]
	},
	{
		name: "VARA",
		description: "Izračunava odstupanje na osnovu uzorka, uključujući logičke vrednosti i tekst. Tekst i logička vrednost FALSE imaju vrednost 0; logička vrednost TRUE ima vrednost 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrednost1",
				description: "predstavlja 1 do 255 argumenata vrednosti koji odgovaraju uzorku populacije"
			},
			{
				name: "vrednost2",
				description: "predstavlja 1 do 255 argumenata vrednosti koji odgovaraju uzorku populacije"
			}
		]
	},
	{
		name: "VARP",
		description: "Izračunava odstupanje na osnovu ukupne populacije (zanemaruje logičke vrednosti i tekst u podacima o populaciji).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "broj1",
				description: "predstavlja numeričke argumente od 1 do 255 koji odgovaraju populaciji"
			},
			{
				name: "broj2",
				description: "predstavlja numeričke argumente od 1 do 255 koji odgovaraju populaciji"
			}
		]
	},
	{
		name: "VARPA",
		description: "Izračunava odstupanje za čitavu populaciju, uključujući logičke vrednosti i tekst. Tekst i logička vrednost FALSE imaju vrednost 0; logička vrednost TRUE ima vrednost 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrednost1",
				description: "predstavlja 1 do 255 argumenata vrednosti koji predstavljaju populaciju"
			},
			{
				name: "vrednost2",
				description: "predstavlja 1 do 255 argumenata vrednosti koji predstavljaju populaciju"
			}
		]
	},
	{
		name: "VDB",
		description: "Daje amortizaciju sredstva za bilo koji period koji navedete, uključujući i parcijalne periode, koristeći metod dvostruke stope linearne amortizacije na opadajuću osnovu ili neki drugi navedeni metod.",
		arguments: [
			{
				name: "cena",
				description: "predstavlja početnu cenu sredstva"
			},
			{
				name: "rashod",
				description: "predstavlja amortizovanu vrednost sredstva po isteku perioda njegove upotrebe"
			},
			{
				name: "vek",
				description: "predstavlja broj perioda tokom kojih se vrši amortizacija sredstva (ponekad se naziva korisni vek sredstva)"
			},
			{
				name: "poč_period",
				description: "predstavlja početni period za koji želite da izračunate amortizaciju, koristeći iste jedinice mere kao i za korisni vek"
			},
			{
				name: "završ_period",
				description: "predstavlja stopu opadanja preostale vrednosti"
			},
			{
				name: "faktor",
				description: " 2 (metod dvostruke stope linearne amortizacije na opadajuću osnovu) ako je izostavljen"
			},
			{
				name: "bez_komutatora",
				description: "prebacivanje na stabilnu amortizaciju, kada je ona veća od balansirane amortizacije = FALSE ili je izostavljena; bez prebacivanja = TRUE"
			}
		]
	},
	{
		name: "VLOOKUP",
		description: "Traži vrednost u krajnjoj levoj koloni tabele i daje je u istom redu kolone koju navedete. Tabela podrazumevano mora biti sortirana po rastućem redosledu.",
		arguments: [
			{
				name: "vrednost_za_pronalaženje",
				description: " predstavlja vrednost koja se traži u prvoj koloni tabele i može biti vrednost, referenca ili tekstualna niska"
			},
			{
				name: "niz_tabele",
				description: "predstavlja tekstualnu tabelu, brojeve ili logičku vrednost u koju se preuzimaju podaci. Niz_tabele može biti referenca opsega ili imena opsega"
			},
			{
				name: "indeksni_broj_kolone",
				description: "predstavlja broj kolone u nizu_tabele iz koje bi podudarna vrednost trebalo da bude data. Prva kolona vrednosti u tabeli je kolona 1"
			},
			{
				name: "opseg_za_pronalaženje",
				description: "predstavlja logičku vrednost: za pronalaženje najpribližnije vrednosti u prvoj koloni (sortiranoj po rastućem redosledu) = TRUE ili je izostavljena; za pronalaženje istovetne vrednosti = FALSE"
			}
		]
	},
	{
		name: "WEEKDAY",
		description: "Daje broj od 1 do 7 koji predstavlja dan u sedmici za neki datum.",
		arguments: [
			{
				name: "serijski_broj",
				description: "predstavlja broj koji reprezentuje datum"
			},
			{
				name: "tip_vraćanja",
				description: "predstavlja broj : za nedelju=1, pa sve do subote=7, koristite 1; za ponedeljak=1, pa sve do nedelje=7, koristite 2; za ponedeljak=0, pa sve do nedelje=7, koristite 3"
			}
		]
	},
	{
		name: "WEEKNUM",
		description: "Daje broj sedmica u godini.",
		arguments: [
			{
				name: "serijski_broj",
				description: "je datum-vreme kôd koji koristi program Spreadsheet za izračunavanje datuma i vremena"
			},
			{
				name: "tip_povraćaja",
				description: "je broj (1 ili 2) koji određuje tip date vrednosti"
			}
		]
	},
	{
		name: "WEIBULL",
		description: "Daje Vejbulovu raspodelu.",
		arguments: [
			{
				name: "x",
				description: "predstavlja nenegativnu vrednost u kojoj treba izračunati raspodelu"
			},
			{
				name: "alfa",
				description: "predstavlja parametar raspodele, pozitivan broj"
			},
			{
				name: "beta",
				description: "predstavlja parametar raspodele, pozitivan broj"
			},
			{
				name: "kumulativno",
				description: "predstavlja logičku vrednost: za kumulativnu funkciju raspodele koristiti TRUE; za funkciju raspodele koristiti FALSE"
			}
		]
	},
	{
		name: "WEIBULL.DIST",
		description: "Daje Vejbulovu raspodelu.",
		arguments: [
			{
				name: "x",
				description: "predstavlja vrednost za koju se računa funkcija, i to nenegativan broj"
			},
			{
				name: "alfa",
				description: "predstavlja parametar raspodele, pozitivan broj"
			},
			{
				name: "beta",
				description: "predstavlja parametar raspodele, pozitivan broj"
			},
			{
				name: "kumulativno",
				description: "predstavlja logičku vrednost: za kumulativnu funkciju raspodele koristiti TRUE; za funkciju raspodele koristiti FALSE"
			}
		]
	},
	{
		name: "WORKDAY",
		description: "Daje serijski broj datuma pre ili posle navedenog broja radnih dana.",
		arguments: [
			{
				name: "datum_početka",
				description: "je serijski broj datuma koji predstavlja datum početka"
			},
			{
				name: "dani",
				description: "je broj dana koji nisu vikendi ni praznici pre ili posle datuma početka"
			},
			{
				name: "praznici",
				description: "je opcionalni niz nekih serijskih brojeva datuma koje bi trebalo isključiti iz radnog kalendara, kao što su državni, savezni i promenljivi praznici"
			}
		]
	},
	{
		name: "WORKDAY.INTL",
		description: "Vraća serijski broj datuma pre ili posle navedenog broja radnih dana sa prilagođenim parametrima vikenda.",
		arguments: [
			{
				name: "datum_početka",
				description: "je serijski broj datuma koji predstavlja datum početka"
			},
			{
				name: "dani",
				description: "je broj dana koji nisu vikendi ni praznici pre ili posle datuma start_date"
			},
			{
				name: "vikend",
				description: "predstavlja broj ili nisku koja navodi kada dolazi do vikenda"
			},
			{
				name: "praznici",
				description: "predstavlja opcionalni niz nekih serijskih brojeva datuma koje treba isključiti iz radnog kalendara kao što su državni, savezni i promenljivi praznici"
			}
		]
	},
	{
		name: "XIRR",
		description: "Daje unutrašnju stopu prinosa za praćenje tokova novca.",
		arguments: [
			{
				name: "vrednosti",
				description: "je grupa tokova novca koja odgovara praćenju uplata po datumima"
			},
			{
				name: "datumi",
				description: "je praćenje datuma uplata koji odgovaraju uplatama u tokovima novca"
			},
			{
				name: "procena",
				description: "je broj za koji pretpostavljate da je približan rezultatu XIRR"
			}
		]
	},
	{
		name: "XNPV",
		description: "Daje sadašnju neto vrednost za praćenje tokova novca.",
		arguments: [
			{
				name: "stopa",
				description: "je stopa popusta koja se primenjuje na tokove novca"
			},
			{
				name: "vrednosti",
				description: "je grupa tokova novca koja odgovara praćenju uplata po datumima"
			},
			{
				name: "datumi",
				description: "je praćenje datuma uplata koji odgovaraju uplatama u tokovima novca"
			}
		]
	},
	{
		name: "XOR",
		description: "Daje logičko „Isključivo ili“ svih argumenata.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "su od 1 do 254 uslova koje želite da testirate i koji mogu da budu ili TRUE ili FALSE i mogu da budu logičke vrednosti, nizovi ili reference"
			},
			{
				name: "logical2",
				description: "su od 1 do 254 uslova koje želite da testirate i koji mogu da budu ili TRUE ili FALSE i mogu da budu logičke vrednosti, nizovi ili reference"
			}
		]
	},
	{
		name: "YEAR",
		description: "Vraća godinu u datumu, ceo broj iz opsega od 1900 do 9999.",
		arguments: [
			{
				name: "serijski_broj",
				description: "predstavlja broj u Spreadsheet kodu za datum-vreme"
			}
		]
	},
	{
		name: "YEARFRAC",
		description: "Daje razlomak na nivou godine koji predstavlja broj celih dana između početnog datuma i završnog datuma.",
		arguments: [
			{
				name: "datum_početka",
				description: "je serijski broj datuma koji predstavlja početni datum"
			},
			{
				name: "datum_završetka",
				description: "je serijski broj datuma koji predstavlja završni datum"
			},
			{
				name: "osnova",
				description: "je tip osnovice za brojanje dana koji bi trebalo koristiti"
			}
		]
	},
	{
		name: "YIELDDISC",
		description: "Daje godišnju dobit za pokriće sa popustom. Na primer, državne obveznice.",
		arguments: [
			{
				name: "poravnanje",
				description: "je datum obračunavanja pokrića, izražen kao serijski broj datuma"
			},
			{
				name: "zrelost",
				description: "je datum dospevanja pokrića, izražen kao serijski broj datuma"
			},
			{
				name: "pr",
				description: "je cena pokrića na 100 Din nominalne vrednosti"
			},
			{
				name: "povraćaj",
				description: "je otkupna vrednost pokrića na 100 Din nominalne vrednosti"
			},
			{
				name: "osnova",
				description: "je tip osnovice za brojanje dana koji bi trebalo koristiti"
			}
		]
	},
	{
		name: "Z.TEST",
		description: "Daje jednostranu P-vrednost z-testa.",
		arguments: [
			{
				name: "niz",
				description: "predstavlja niz ili opseg podataka u odnosu na koje bi trebalo testirati X"
			},
			{
				name: "x",
				description: "predstavlja vrednost testa"
			},
			{
				name: "sigma",
				description: "predstavlja standardnu devijaciju za populaciju (poznato). Ako je izostavljena, koristi se uzorak standardne devijacije"
			}
		]
	},
	{
		name: "ZTEST",
		description: "Daje jednostranu P-vrednost z-testa.",
		arguments: [
			{
				name: "niz",
				description: "predstavlja niz ili opseg podataka u odnosu na koje bi trebalo testirati X"
			},
			{
				name: "x",
				description: "predstavlja vrednost testa"
			},
			{
				name: "sigma",
				description: "predstavlja standardnu devijaciju za populaciju (poznato). Ako je izostavljena, koristi se uzorak standardne devijacije"
			}
		]
	}
];