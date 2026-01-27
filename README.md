# Pozorište — Repertoar (pozoriste-repertoar)

Kratak opis
-----------
ASP.NET Core MVC aplikacija za upravljanje repertoarom pozorišta, rezervacijama i administracijom predstava.

Brzo pokretanje (lokalno)
--------------------------
1. Preduvjeti:
   - .NET 7 (ili verzija koju projekat koristi)
   - SQL Server (lokalno ili kroz Docker)
   - (opciono) Docker & docker-compose za brz lokalni razvoj

2. Konfiguracija:
   - Napravi lokalni fajl za konfiguraciju ili postavi Environment varijable.
     Primjeri (Linux/macOS):
     ```
     export ConnectionStrings__DefaultConnection="Server=localhost;Database=PozoristeRepertoar;User Id=sa;Password=YourStrong!Passw0rd"
     export POZ_ADMIN_PASSWORD="Admin123!"
     ```
   - Alternativno kopiraj `appsettings.Development.json.example` u `appsettings.Development.json` i popuni vrednosti (NE kommituj taj fajl).

3. Migracije + Seed:
   - Pokreni migracije:
     ```
     dotnet ef database update --project src/PozoristeRepertoar/Pozoriste.DataAccess --startup-project src/PozoristeRepertoar/Pozoriste.Web
     ```
   - (Ako seed logika postoji u Program.cs, aplikacija može automatski seed-ovati admin nalog pri prvom pokretanju.)

4. Pokretanje aplikacije:
   ```
   cd src/PozoristeRepertoar/Pozoriste.Web
   dotnet run
   ```
   Aplikacija æe biti dostupna na `https://localhost:5001` (ili port koji je konfigurisan).

Tajni podaci i lozinke
----------------------
NE hardkodovati lozinke (npr. admin lozinka) u kodu. Korišæenje:
- Environment varijabli
- User Secrets (lokalno): `dotnet user-secrets`
- Tajni menadžeri (Azure Key Vault, AWS Secrets Manager) za produkciju

Git higijena i preporuke
------------------------
- Dodan je `.gitignore` za .NET i tipiène artefakte.
- Preporuèuje se da se vendor paketi (wwwroot/lib) NE èuvaju u repou — koristi LibMan/CDN/nuget/npm.
- Preporuèuje se da se veliki fajlovi uklone iz istorije ako su veæ commit-ovani (BFG / git filter-repo).

CI / Testovi
------------
- U repou je predložen GitHub Actions workflow (`.github/workflows/ci.yml`) koji:
  - radi restore, build i test (kad su testovi dostupni)
  - kešira NuGet pakete

Doprinos i pravila za commit poruke
-----------------------------------
- Koristi jasne commit poruke. Primeri:
  - feat: dodaj funkcionalnost rezervacije
  - fix: popravi NullReference u TerminRepository
  - chore: formatiranje koda i dodavanje .editorconfig
- Branching:
  - feature/* za nove funkcionalnosti
  - fix/* za ispravke grešaka
  - chore/* za dev ops i konfig izmene
- Otvaraj Pull Request-ove i traži review pre merge-a.

Kako ukloniti velike fajlove iz istorije (ako treba)
---------------------------------------------------
- Ako su veliki vendor fajlovi veæ committovani i treba ih izbrisati iz istorije:
  - Upotrebi BFG ili git filter-repo (ovo menja istoriju i zahteva force push i koordinaciju sa timom).

Kontakt
-------
Za pitanja ostavi issue u repozitorijumu ili kontaktiraj autora putem GitHub profila.
