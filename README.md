# Pozorište — Repertoar

ASP.NET Core MVC web aplikacija za upravljanje repertoarom pozorišta.  
Aplikacija sadrži korisnički deo za pregled predstava i administrativni deo za upravljanje sadržajem.

---

## Tehnologije

- ASP.NET Core MVC (.NET 8)
- Entity Framework Core (Code First + migracije)
- SQL Server
- ASP.NET Identity
- Razor Views

---

## Funkcionalnosti

### Korisnički deo
- Pregled repertoara predstava
- Prikaz detalja o predstavi
- Pregled termina igranja

### Administratorski deo
- Dodavanje, izmena i brisanje predstava
- Upload slika za predstave
- Upravljanje terminima
- Role-based autorizacija (Admin)

---

## Pokretanje aplikacije (lokalno)

### Preduslovi

- Visual Studio 2022
- .NET 8 SDK
- SQL Server (LocalDB ili lokalna instanca)

---

### Konfiguracija baze podataka

1. U root folderu projekta pronađi fajl:

appsettings.Development.json.example

2. Napravi kopiju i preimenuj fajl u:

appsettings.Development.json

3. U tom fajlu podesi connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=PozoristeRepertoar;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

NAPOMENA: Ovaj fajl se ne commit-uje u Git repozitorijum jer sadrži lokalne konfiguracije.

---

### Migracije baze

U Package Manager Console ili terminalu pokreni:

```bash
dotnet ef database update
```

Ova komanda automatski kreira bazu podataka i tabele.

---

### Pokretanje aplikacije

U Visual Studio okruženju:

- Pokreni aplikaciju klikom na Run (F5)

ILI preko terminala:

```bash
dotnet run
```

Aplikacija će biti dostupna na adresi:

https://localhost:5001

(Port može da se razlikuje u zavisnosti od lokalne konfiguracije)

---

## Seed podaci

Prilikom prvog pokretanja aplikacije automatski se kreiraju osnovne role i administratorski nalog (ukoliko već ne postoje).

Administratorski podaci se ne hardkoduju u kodu već se čitaju iz konfiguracije ili environment varijabli.

---

## Bezbednost

- Lozinke i connection string podaci nisu hardkodovani u aplikaciji
- Osetljivi podaci se čuvaju u lokalnim konfiguracionim fajlovima ili environment varijablama
- Implementirana je osnovna validacija prilikom uploada fajlova

---

## Struktura projekta

PozoristeRepertoar  
│  
├── Pozoriste.Web          -> UI sloj (Controllers, Views, Identity, Areas)  
├── Pozoriste.DataAccess   -> EF Core, DbContext, Repositories, Migracije  
├── Pozoriste.Models       -> Entity modeli i ViewModel-i  

---

## Git higijena

- Build fajlovi (bin, obj, .vs) nisu verzionisani
- .gitignore je konfigurisan za .NET projekte
- GitHub Actions CI workflow automatski proverava build aplikacije

---

## Autor

Projekat je razvijen kao studentski rad u okviru kursa iz oblasti softverskog inženjerstva.
