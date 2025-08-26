# Petstagram

## Migrations
from root(Petsgram) folder:

**for adding migration:**
```
dotnet ef migrations add CheckMigration --project src/Petsgram.Infrastructure --startup-project src/Petsgram.WebAPI
``` 

**for removing migration:**
```
dotnet ef migrations remove --project src/Petsgram.Infrastructure --startup-project src/Petsgram.WebAPI --force
```

## Docker Compose

**for building a project run:**
```
docker compose up -d --build
```