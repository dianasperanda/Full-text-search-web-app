Razvojno okru�enje kori�teno za izradu rada je Microsoft Visual Studio 2017. Za izradu programske podr�ke odabran je programski jezik C#. 
Programski jezik C# je objektno orijentirani programski jezik namijenjen za opcu upotrebu, razvijen od strane Microsofta za .NET platformu. 
Za razvoj programske podr�ke kori�tena je trenutno aktualna verzija 4.7 platforme .NET i verzija 6 programskog jezika C#.
Za razvoj baze podataka kori�ten je sustav za upravljanje bazama podataka posgreSQL.

Prije prvog pokretanja potrebno je pokrenuti posgreSQL na adresi 192.168.56.12, portu 5432.
Stvoriti korisnika postgres s lozinkom reverse, te izvrsiti SQL skriptu danu u mapi uz ovu datoteku imena movie.sql..
Kod svih ostalih pokretanja dovoljno je pokrenuti postgreSQL i iz razvojnog okruzenja MS Visual Studio pokrenuti aplikaciju sa F5 ili Ctrl + F5.

Nakon �to se stvori nova baza podataka u PgAdminu, potrebno je uz pomoc naredbe pgsql -d nmbp2 -f movie.sql izvr�iti skriptu movie.sql.
Skripta ce stvoriti tablice te unjeti pocetne podatke.
